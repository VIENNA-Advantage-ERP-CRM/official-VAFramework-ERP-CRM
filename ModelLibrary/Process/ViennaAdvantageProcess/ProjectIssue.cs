using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage.Classes;

using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class ProjectIssue : SvrProcess
    {
        #region Private Varibles
        /**	Project - Mandatory Parameter		*/
        private int m_C_Project_ID = 0;
        /**	Receipt - Option 1					*/
        private int m_M_InOut_ID = 0;
        /**	Expenses - Option 2					*/
        private int m_S_TimeExpense_ID = 0;
        /** Locator - Option 3,4				*/
        private int m_M_Locator_ID = 0;
        /** Project Line - Option 3				*/
        private int m_C_ProjectLine_ID = 0;
        /** Product - Option 4					*/
        private int m_M_Product_ID = 0;
        /** Attribute - Option 4				*/
        private int m_M_AttributeSetInstance_ID = 0;
        /** Qty - Option 4						*/
        private Decimal? m_MovementQty = null;
        /** Date - Option						*/
        private DateTime? m_MovementDate = null;
        /** Description - Option				*/
        private String m_Description = null;

        /**	The Project to be received			*/
        private MProject m_project = null;
        /**	The Project to be received			*/
        private MProjectIssue[] m_projectIssues = null;
        #endregion

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Project_ID"))
                {
                    // m_C_Project_ID = ((Decimal)para[i].GetParameter()).intValue();
                    m_C_Project_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_InOut_ID"))
                {
                    //m_M_InOut_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_M_InOut_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("S_TimeExpense_ID"))
                {
                    // m_S_TimeExpense_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_S_TimeExpense_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Locator_ID"))
                {
                    //m_M_Locator_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_M_Locator_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_ProjectLine_ID"))
                {
                    // m_C_ProjectLine_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_C_ProjectLine_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Product_ID"))
                {
                    //m_M_Product_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_M_Product_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_AttributeSetInstance_ID"))
                {
                    //m_M_AttributeSetInstance_ID = ((BigDecimal)para[i].GetParameter()).intValue();
                    m_M_AttributeSetInstance_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("MovementQty"))
                {
                    m_MovementQty = Util.GetValueOfDecimal(para[i].GetParameter());
                }
                else if (name.Equals("MovementDate"))
                {
                    m_MovementDate = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("Description"))
                {
                    m_Description = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /**
	    *  Perrform process.
	    *  @return Message (clear text)
	    *  @throws Exception if not successful
	    */
        protected override string DoIt()
        {
            //	Check Parameter
            m_project = new MProject(GetCtx(), m_C_Project_ID, Get_TrxName());
            if (!(MProject.PROJECTCATEGORY_WorkOrderJob.Equals(m_project.GetProjectCategory())
                || MProject.PROJECTCATEGORY_AssetProject.Equals(m_project.GetProjectCategory())))
            {
                throw new ArgumentException("Project not Work Order or Asset =" + m_project.GetProjectCategory());
            }
            log.Info(m_project.ToString());

            //
            if (m_M_InOut_ID != 0)
            {
                return IssueReceipt();
            }
            if (m_S_TimeExpense_ID != 0)
            {
                return IssueExpense();
            }
            if (m_M_Locator_ID == 0)
            {
                throw new ArgumentException("Locator missing");
            }
            if (m_C_ProjectLine_ID != 0)
            {
                return IssueProjectLine();
            }
            return IssueInventory();
            //return "";
        }


        /**
	 *	Issue Receipt
	 *	@return Message (clear text)
	 */
        private String IssueReceipt()
        {
            MInOut inOut = new MInOut(GetCtx(), m_M_InOut_ID, null);
            if (inOut.IsSOTrx() || !inOut.IsProcessed()
                || !(MInOut.DOCSTATUS_Completed.Equals(inOut.GetDocStatus()) || MInOut.DOCSTATUS_Closed.Equals(inOut.GetDocStatus())))
            {
                throw new ArgumentException("Receipt not valid - " + inOut);
            }
            log.Info(inOut.ToString());
            //	Set Project of Receipt
            if (inOut.GetC_Project_ID() == 0)
            {
                inOut.SetC_Project_ID(m_project.GetC_Project_ID());
                inOut.Save();
            }
            else if (inOut.GetC_Project_ID() != m_project.GetC_Project_ID())
            {
                throw new ArgumentException("Receipt for other Project (" + inOut.GetC_Project_ID() + ")");
            }

            MInOutLine[] inOutLines = inOut.GetLines(false);
            int counter = 0;
            for (int i = 0; i < inOutLines.Length; i++)
            {
                //	Need to have a Product
                if (inOutLines[i].GetM_Product_ID() == 0)
                    continue;
                //	Need to have Quantity
                if ( Env.Signum(inOutLines[i].GetMovementQty()) == 0)
                    continue;
                //	not issued yet
                if (ProjectIssueHasReceipt(inOutLines[i].GetM_InOutLine_ID()))
                    continue;
                //	Create Issue
                MProjectIssue pi = new MProjectIssue(m_project);
                pi.SetMandatory(inOutLines[i].GetM_Locator_ID(), inOutLines[i].GetM_Product_ID(), inOutLines[i].GetMovementQty());
                if (m_MovementDate != null)		//	default today
                {
                    pi.SetMovementDate(m_MovementDate);
                }
                if (m_Description != null && m_Description.Length > 0)
                {
                    pi.SetDescription(m_Description);
                }
                else if (inOutLines[i].GetDescription() != null)
                {
                    pi.SetDescription(inOutLines[i].GetDescription());
                }
                else if (inOut.GetDescription() != null)
                {
                    pi.SetDescription(inOut.GetDescription());
                }
                pi.SetM_InOutLine_ID(inOutLines[i].GetM_InOutLine_ID());
                pi.Process();

                //	Find/Create Project Line
                MProjectLine pl = null;
                MProjectLine[] pls = m_project.GetLines();
                for (int ii = 0; ii < pls.Length; ii++)
                {
                    //	The Order we generated is the same as the Order of the receipt
                    if (pls[ii].GetC_OrderPO_ID() == inOut.GetC_Order_ID()
                        && pls[ii].GetM_Product_ID() == inOutLines[i].GetM_Product_ID()
                        && pls[ii].GetC_ProjectIssue_ID() == 0)		//	not issued
                    {
                        pl = pls[ii];
                        break;
                    }
                }
                if (pl == null)
                    pl = new MProjectLine(m_project);
                pl.SetMProjectIssue(pi);		//	setIssue
                pl.Save();
                AddLog(pi.GetLine(), pi.GetMovementDate(), pi.GetMovementQty(), null);
                counter++;
            }	//	all InOutLines

            return "@Created@ " + counter;
        }	//	issueReceipt


        /**
         *	Issue Expense Report
         *	@return Message (clear text)
         */
        private String IssueExpense()
        {
            //	Get Expense Report
            MTimeExpense expense = new MTimeExpense(GetCtx(), m_S_TimeExpense_ID, Get_TrxName());
            if (!expense.IsProcessed())
            {
                throw new ArgumentException("Time+Expense not processed - " + expense);
            }

            //	for all expense lines
            MTimeExpenseLine[] expenseLines = expense.GetLines(false);
            int counter = 0;
            for (int i = 0; i < expenseLines.Length; i++)
            {
                //	Need to have a Product
                if (expenseLines[i].GetM_Product_ID() == 0)
                    continue;
                //	Need to have Quantity
                if ( Env.Signum(expenseLines[i].GetQty()) == 0)
                    continue;
                //	Need to the same project
                if (expenseLines[i].GetC_Project_ID() != m_project.GetC_Project_ID())
                    continue;
                //	not issued yet
                if (ProjectIssueHasExpense(expenseLines[i].GetS_TimeExpenseLine_ID()))
                    continue;

                //	Find Location
                int M_Locator_ID = 0;
                //	MProduct product = new MProduct (getCtx(), expenseLines[i].getM_Product_ID());
                //	if (product.isStocked())
                M_Locator_ID = MStorage.GetM_Locator_ID(expense.GetM_Warehouse_ID(),
                    expenseLines[i].GetM_Product_ID(), 0, 	//	no ASI
                    expenseLines[i].GetQty(), null);
                if (M_Locator_ID == 0)	//	Service/Expense - get default (and fallback)
                    M_Locator_ID = expense.GetM_Locator_ID();

                //	Create Issue
                MProjectIssue pi = new MProjectIssue(m_project);
                pi.SetMandatory(M_Locator_ID, expenseLines[i].GetM_Product_ID(), expenseLines[i].GetQty());
                if (m_MovementDate != null)		//	default today
                    pi.SetMovementDate(m_MovementDate);
                if (m_Description != null && m_Description.Length > 0)
                    pi.SetDescription(m_Description);
                else if (expenseLines[i].GetDescription() != null)
                    pi.SetDescription(expenseLines[i].GetDescription());
                pi.SetS_TimeExpenseLine_ID(expenseLines[i].GetS_TimeExpenseLine_ID());
                pi.Process();
                //	Find/Create Project Line
                MProjectLine pl = new MProjectLine(m_project);
                pl.SetMProjectIssue(pi);		//	setIssue
                pl.Save();
                AddLog(pi.GetLine(), pi.GetMovementDate(), pi.GetMovementQty(), null);
                counter++;
            }	//	allExpenseLines

            return "@Created@ " + counter;
        }	//	issueExpense


        /**
         *	Issue Project Line
         *	@return Message (clear text)
         */
        private String IssueProjectLine()
        {
            MProjectLine pl = new MProjectLine(GetCtx(), m_C_ProjectLine_ID, Get_TrxName());
            if (pl.GetM_Product_ID() == 0)
            {
                throw new ArgumentException("Projet Line has no Product");
            }
            if (pl.GetC_ProjectIssue_ID() != 0)
            {
                throw new ArgumentException("Projet Line already been issued");
            }
            if (m_M_Locator_ID == 0)
            {
                throw new ArgumentException("No Locator");
            }
            //	Set to Qty 1
            if ( Env.Signum(pl.GetPlannedQty()) == 0)
            {
                pl.SetPlannedQty(Env.ONE);
            }
            //
            MProjectIssue pi = new MProjectIssue(m_project);
            pi.SetMandatory(m_M_Locator_ID, pl.GetM_Product_ID(), pl.GetPlannedQty());
            if (m_MovementDate != null)		//	default today
            {
                pi.SetMovementDate(m_MovementDate);
            }
            if (m_Description != null && m_Description.Length > 0)
            {
                pi.SetDescription(m_Description);
            }
            else if (pl.GetDescription() != null)
            {
                pi.SetDescription(pl.GetDescription());
            }
            pi.Process();

            //	Update Line
            pl.SetMProjectIssue(pi);
            pl.Save();
            AddLog(pi.GetLine(), pi.GetMovementDate(), pi.GetMovementQty(), null);
            return "@Created@ 1";
        }	//	issueProjectLine


        /**
         *	Issue from Inventory
         *	@return Message (clear text)
         */
        private String IssueInventory()
        {
            if (m_M_Locator_ID == 0)
            {
                throw new ArgumentException("No Locator");
            }
            if (m_M_Product_ID == 0)
            {
                throw new ArgumentException("No Product");
            }
            //	Set to Qty 1
            if (m_MovementQty == null || (m_MovementQty) == 0)
            {
                m_MovementQty = Env.ONE;
            }
            //
            MProjectIssue pi = new MProjectIssue(m_project);
            pi.SetMandatory(m_M_Locator_ID, m_M_Product_ID, m_MovementQty.Value);
            if (m_MovementDate != null)		//	default today
            {
                pi.SetMovementDate(m_MovementDate);
            }
            if (m_Description != null && m_Description.Length > 0)
            {
                pi.SetDescription(m_Description);
            }
            pi.Process();

            //	Create Project Line
            MProjectLine pl = new MProjectLine(m_project);
            pl.SetMProjectIssue(pi);
            pl.Save();
            AddLog(pi.GetLine(), pi.GetMovementDate(), pi.GetMovementQty(), null);
            return "@Created@ 1";
        }	//	issueInventory

        /**
         * 	Check if Project Issue already has Expense
         *	@param S_TimeExpenseLine_ID line
         *	@return true if exists
         */
        private Boolean ProjectIssueHasExpense(int S_TimeExpenseLine_ID)
        {
            if (m_projectIssues == null)
            {
                m_projectIssues = m_project.GetIssues();
            }
            for (int i = 0; i < m_projectIssues.Length; i++)
            {
                if (m_projectIssues[i].GetS_TimeExpenseLine_ID() == S_TimeExpenseLine_ID)
                {
                    return true;
                }
            }
            return false;
        }	//	projectIssueHasExpense

        /**
         * 	Check if Project Isssye already has Receipt
         *	@param M_InOutLine_ID line
         *	@return true if exists
         */
        private Boolean ProjectIssueHasReceipt(int M_InOutLine_ID)
        {
            if (m_projectIssues == null)
            {
                m_projectIssues = m_project.GetIssues();
            }
            for (int i = 0; i < m_projectIssues.Length; i++)
            {
                if (m_projectIssues[i].GetM_InOutLine_ID() == M_InOutLine_ID)
                {
                    return true;
                }
            }
            return false;
        }	//	projectIssueHasReceipt

    }
}
