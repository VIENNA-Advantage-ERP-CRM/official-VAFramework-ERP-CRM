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
        private int m_VAB_Project_ID = 0;
        /**	Receipt - Option 1					*/
        private int m_VAM_Inv_InOut_ID = 0;
        /**	Expenses - Option 2					*/
        private int m_VAS_ExpenseReport_ID = 0;
        /** Locator - Option 3,4				*/
        private int m_VAM_Locator_ID = 0;
        /** Project Line - Option 3				*/
        private int m_VAB_ProjectLine_ID = 0;
        /** Product - Option 4					*/
        private int m_VAM_Product_ID = 0;
        /** Attribute - Option 4				*/
        private int m_VAM_PFeature_SetInstance_ID = 0;
        /** Qty - Option 4						*/
        private Decimal? VAM_InventoryTransferQty = null;
        /** Date - Option						*/
        private DateTime? VAM_InventoryTransferDate = null;
        /** Description - Option				*/
        private String m_Description = null;

        /**	The Project to be received			*/
        private MVABProject m_project = null;
        /**	The Project to be received			*/
        private MVABProjectSupply[] m_projectIssues = null;
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
                else if (name.Equals("VAB_Project_ID"))
                {
                    // m_VAB_Project_ID = ((Decimal)para[i].GetParameter()).intValue();
                    m_VAB_Project_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_Inv_InOut_ID"))
                {
                    //m_VAM_Inv_InOut_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_VAM_Inv_InOut_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAS_ExpenseReport_ID"))
                {
                    // m_VAS_ExpenseReport_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_VAS_ExpenseReport_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_Locator_ID"))
                {
                    //m_VAM_Locator_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_VAM_Locator_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_ProjectLine_ID"))
                {
                    // m_VAB_ProjectLine_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_VAB_ProjectLine_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_Product_ID"))
                {
                    //m_VAM_Product_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    m_VAM_Product_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_PFeature_SetInstance_ID"))
                {
                    //m_VAM_PFeature_SetInstance_ID = ((BigDecimal)para[i].GetParameter()).intValue();
                    m_VAM_PFeature_SetInstance_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("MovementQty"))
                {
                    VAM_InventoryTransferQty = Util.GetValueOfDecimal(para[i].GetParameter());
                }
                else if (name.Equals("MovementDate"))
                {
                    VAM_InventoryTransferDate = Util.GetValueOfDateTime(para[i].GetParameter());
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
            m_project = new MVABProject(GetCtx(), m_VAB_Project_ID, Get_TrxName());
            if (!(MVABProject.PROJECTCATEGORY_WorkOrderJob.Equals(m_project.GetProjectCategory())
                || MVABProject.PROJECTCATEGORY_AssetProject.Equals(m_project.GetProjectCategory())))
            {
                throw new ArgumentException("Project not Work Order or Asset =" + m_project.GetProjectCategory());
            }
            log.Info(m_project.ToString());

            //
            if (m_VAM_Inv_InOut_ID != 0)
            {
                return IssueReceipt();
            }
            if (m_VAS_ExpenseReport_ID != 0)
            {
                return IssueExpense();
            }
            if (m_VAM_Locator_ID == 0)
            {
                throw new ArgumentException("Locator missing");
            }
            if (m_VAB_ProjectLine_ID != 0)
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
            MVAMInvInOut inOut = new MVAMInvInOut(GetCtx(), m_VAM_Inv_InOut_ID, null);
            if (inOut.IsSOTrx() || !inOut.IsProcessed()
                || !(MVAMInvInOut.DOCSTATUS_Completed.Equals(inOut.GetDocStatus()) || MVAMInvInOut.DOCSTATUS_Closed.Equals(inOut.GetDocStatus())))
            {
                throw new ArgumentException("Receipt not valid - " + inOut);
            }
            log.Info(inOut.ToString());
            //	Set Project of Receipt
            if (inOut.GetVAB_Project_ID() == 0)
            {
                inOut.SetVAB_Project_ID(m_project.GetVAB_Project_ID());
                inOut.Save();
            }
            else if (inOut.GetVAB_Project_ID() != m_project.GetVAB_Project_ID())
            {
                throw new ArgumentException("Receipt for other Project (" + inOut.GetVAB_Project_ID() + ")");
            }

            MVAMInvInOutLine[] inOutLines = inOut.GetLines(false);
            int counter = 0;
            for (int i = 0; i < inOutLines.Length; i++)
            {
                //	Need to have a Product
                if (inOutLines[i].GetVAM_Product_ID() == 0)
                    continue;
                //	Need to have Quantity
                if ( Env.Signum(inOutLines[i].GetMovementQty()) == 0)
                    continue;
                //	not issued yet
                if (ProjectIssueHasReceipt(inOutLines[i].GetVAM_Inv_InOutLine_ID()))
                    continue;
                //	Create Issue
                MVABProjectSupply pi = new MVABProjectSupply(m_project);
                pi.SetMandatory(inOutLines[i].GetVAM_Locator_ID(), inOutLines[i].GetVAM_Product_ID(), inOutLines[i].GetMovementQty());
                if (VAM_InventoryTransferDate != null)		//	default today
                {
                    pi.SetMovementDate(VAM_InventoryTransferDate);
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
                pi.SetVAM_Inv_InOutLine_ID(inOutLines[i].GetVAM_Inv_InOutLine_ID());
                pi.Process();

                //	Find/Create Project Line
                MVABProjectLine pl = null;
                MVABProjectLine[] pls = m_project.GetLines();
                for (int ii = 0; ii < pls.Length; ii++)
                {
                    //	The Order we generated is the same as the Order of the receipt
                    if (pls[ii].GetVAB_OrderPO_ID() == inOut.GetVAB_Order_ID()
                        && pls[ii].GetVAM_Product_ID() == inOutLines[i].GetVAM_Product_ID()
                        && pls[ii].GetVAB_ProjectSupply_ID() == 0)		//	not issued
                    {
                        pl = pls[ii];
                        break;
                    }
                }
                if (pl == null)
                    pl = new MVABProjectLine(m_project);
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
            MVASExpenseReport expense = new MVASExpenseReport(GetCtx(), m_VAS_ExpenseReport_ID, Get_TrxName());
            if (!expense.IsProcessed())
            {
                throw new ArgumentException("Time+Expense not processed - " + expense);
            }

            //	for all expense lines
            MVASExpenseReportLine[] expenseLines = expense.GetLines(false);
            int counter = 0;
            for (int i = 0; i < expenseLines.Length; i++)
            {
                //	Need to have a Product
                if (expenseLines[i].GetVAM_Product_ID() == 0)
                    continue;
                //	Need to have Quantity
                if ( Env.Signum(expenseLines[i].GetQty()) == 0)
                    continue;
                //	Need to the same project
                if (expenseLines[i].GetVAB_Project_ID() != m_project.GetVAB_Project_ID())
                    continue;
                //	not issued yet
                if (ProjectIssueHasExpense(expenseLines[i].GetVAS_ExpenseReportLine_ID()))
                    continue;

                //	Find Location
                int VAM_Locator_ID = 0;
                //	MVAMProduct product = new MVAMProduct (getCtx(), expenseLines[i].getVAM_Product_ID());
                //	if (product.isStocked())
                VAM_Locator_ID = MVAMStorage.GetVAM_Locator_ID(expense.GetVAM_Warehouse_ID(),
                    expenseLines[i].GetVAM_Product_ID(), 0, 	//	no ASI
                    expenseLines[i].GetQty(), null);
                if (VAM_Locator_ID == 0)	//	Service/Expense - get default (and fallback)
                    VAM_Locator_ID = expense.GetVAM_Locator_ID();

                //	Create Issue
                MVABProjectSupply pi = new MVABProjectSupply(m_project);
                pi.SetMandatory(VAM_Locator_ID, expenseLines[i].GetVAM_Product_ID(), expenseLines[i].GetQty());
                if (VAM_InventoryTransferDate != null)		//	default today
                    pi.SetMovementDate(VAM_InventoryTransferDate);
                if (m_Description != null && m_Description.Length > 0)
                    pi.SetDescription(m_Description);
                else if (expenseLines[i].GetDescription() != null)
                    pi.SetDescription(expenseLines[i].GetDescription());
                pi.SetVAS_ExpenseReportLine_ID(expenseLines[i].GetVAS_ExpenseReportLine_ID());
                pi.Process();
                //	Find/Create Project Line
                MVABProjectLine pl = new MVABProjectLine(m_project);
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
            MVABProjectLine pl = new MVABProjectLine(GetCtx(), m_VAB_ProjectLine_ID, Get_TrxName());
            if (pl.GetVAM_Product_ID() == 0)
            {
                throw new ArgumentException("Projet Line has no Product");
            }
            if (pl.GetVAB_ProjectSupply_ID() != 0)
            {
                throw new ArgumentException("Projet Line already been issued");
            }
            if (m_VAM_Locator_ID == 0)
            {
                throw new ArgumentException("No Locator");
            }
            //	Set to Qty 1
            if ( Env.Signum(pl.GetPlannedQty()) == 0)
            {
                pl.SetPlannedQty(Env.ONE);
            }
            //
            MVABProjectSupply pi = new MVABProjectSupply(m_project);
            pi.SetMandatory(m_VAM_Locator_ID, pl.GetVAM_Product_ID(), pl.GetPlannedQty());
            if (VAM_InventoryTransferDate != null)		//	default today
            {
                pi.SetMovementDate(VAM_InventoryTransferDate);
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
            if (m_VAM_Locator_ID == 0)
            {
                throw new ArgumentException("No Locator");
            }
            if (m_VAM_Product_ID == 0)
            {
                throw new ArgumentException("No Product");
            }
            //	Set to Qty 1
            if (VAM_InventoryTransferQty == null || (VAM_InventoryTransferQty) == 0)
            {
                VAM_InventoryTransferQty = Env.ONE;
            }
            //
            MVABProjectSupply pi = new MVABProjectSupply(m_project);
            pi.SetMandatory(m_VAM_Locator_ID, m_VAM_Product_ID, VAM_InventoryTransferQty.Value);
            if (VAM_InventoryTransferDate != null)		//	default today
            {
                pi.SetMovementDate(VAM_InventoryTransferDate);
            }
            if (m_Description != null && m_Description.Length > 0)
            {
                pi.SetDescription(m_Description);
            }
            pi.Process();

            //	Create Project Line
            MVABProjectLine pl = new MVABProjectLine(m_project);
            pl.SetMProjectIssue(pi);
            pl.Save();
            AddLog(pi.GetLine(), pi.GetMovementDate(), pi.GetMovementQty(), null);
            return "@Created@ 1";
        }	//	issueInventory

        /**
         * 	Check if Project Issue already has Expense
         *	@param VAS_ExpenseReportLine_ID line
         *	@return true if exists
         */
        private Boolean ProjectIssueHasExpense(int VAS_ExpenseReportLine_ID)
        {
            if (m_projectIssues == null)
            {
                m_projectIssues = m_project.GetIssues();
            }
            for (int i = 0; i < m_projectIssues.Length; i++)
            {
                if (m_projectIssues[i].GetVAS_ExpenseReportLine_ID() == VAS_ExpenseReportLine_ID)
                {
                    return true;
                }
            }
            return false;
        }	//	projectIssueHasExpense

        /**
         * 	Check if Project Isssye already has Receipt
         *	@param VAM_Inv_InOutLine_ID line
         *	@return true if exists
         */
        private Boolean ProjectIssueHasReceipt(int VAM_Inv_InOutLine_ID)
        {
            if (m_projectIssues == null)
            {
                m_projectIssues = m_project.GetIssues();
            }
            for (int i = 0; i < m_projectIssues.Length; i++)
            {
                if (m_projectIssues[i].GetVAM_Inv_InOutLine_ID() == VAM_Inv_InOutLine_ID)
                {
                    return true;
                }
            }
            return false;
        }	//	projectIssueHasReceipt

    }
}
