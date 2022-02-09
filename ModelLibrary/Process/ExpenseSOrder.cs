/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ExpenseSOrder
 * Purpose        : Create Sales Orders from Expense Reports
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           2-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ExpenseSOrder : ProcessEngine.SvrProcess
    {
        /**	 BPartner				*/
        private int _C_BPartner_ID = 0;
        /** Date Drom				*/
        private DateTime? _DateFrom = null;
        /** Date To					*/
        private DateTime? _DateTo = null;

        /** No SO generated			*/
        private int _noOrders = 0;
        /**	Current Order			*/
        private MOrder _order = null;
        private string message = "";
        // Time Expense Line
        MTimeExpenseLine tel = null;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
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
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateExpense"))
                {
                    _DateFrom = (DateTime?)para[i].GetParameter();
                    _DateTo = (DateTime?)para[i].GetParameter_To();
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }   //	prepare


        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message to be translated</returns>
        protected override String DoIt()
        {
            int index = 1;
            StringBuilder sql = new StringBuilder("SELECT * FROM S_TimeExpenseLine el "
                + "WHERE el.AD_Client_ID=@param1"                       //	#1
                + " AND el.C_BPartner_ID>0 AND el.IsInvoiced='Y'"   //	Business Partner && to be invoiced
                + " AND el.C_OrderLine_ID IS NULL"                  //	not invoiced yet
                + " AND EXISTS (SELECT * FROM S_TimeExpense e "     //	processed only
                    + "WHERE el.S_TimeExpense_ID=e.S_TimeExpense_ID AND e.Processed='Y')");
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                index++;
                sql.Append(" AND el.C_BPartner_ID=@param2");            //	#2
            }
            if (_DateFrom != null || _DateTo != null)
            {

                sql.Append(" AND EXISTS (SELECT * FROM S_TimeExpense e "
                    + "WHERE el.S_TimeExpense_ID=e.S_TimeExpense_ID");
                if (_DateFrom != null)
                {
                    index++;
                    sql.Append(" AND e.DateReport >= @param3");     //	#3
                }
                if (_DateTo != null)
                {
                    index++;
                    sql.Append(" AND e.DateReport <= @param4");     //	#4
                }

                sql.Append(")");
            }
            sql.Append(" ORDER BY el.C_BPartner_ID, el.C_Project_ID, el.S_TimeExpense_ID, el.Line");

            //
            MBPartner oldBPartner = null;
            int old_Project_ID = -1;
            MTimeExpense te = null;
            //
            SqlParameter[] param = new SqlParameter[index];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql.toString(), get_TrxName());
                int par = 0;
                //pstmt.setInt(par++, getAD_Client_ID());
                param[par] = new SqlParameter("@param1", GetAD_Client_ID());
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    //pstmt.setInt(par++, _C_BPartner_ID);
                    par++;
                    param[par] = new SqlParameter("@param2", _C_BPartner_ID);
                }
                if (_DateFrom != null)
                {
                    par++;
                    //pstmt.setTimestamp(par++, _DateFrom);
                    param[par] = new SqlParameter("@param3", _DateFrom);
                }
                if (_DateTo != null)
                {
                    //pstmt.setTimestamp(par++, _DateTo);
                    par++;
                    param[par] = new SqlParameter("@param4", _DateTo);
                }
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)             //	********* Expense Line Loop
                    {
                        tel = new MTimeExpenseLine(GetCtx(), dr, Get_TrxName());
                        if (!tel.IsInvoiced())
                        {
                            continue;
                        }

                        //	New BPartner - New Order
                        if (oldBPartner == null
                            || oldBPartner.GetC_BPartner_ID() != tel.GetC_BPartner_ID())
                        {
                            CompleteOrder();
                            oldBPartner = new MBPartner(GetCtx(), tel.GetC_BPartner_ID(), Get_TrxName());
                        }
                        //	New Project - New Order
                        if (old_Project_ID != tel.GetC_Project_ID())
                        {
                            CompleteOrder();
                            old_Project_ID = tel.GetC_Project_ID();
                        }
                        if (te == null || te.GetS_TimeExpense_ID() != tel.GetS_TimeExpense_ID())
                            te = new MTimeExpense(GetCtx(), tel.GetS_TimeExpense_ID(), Get_TrxName());
                        //
                        ProcessLine(te, tel, oldBPartner);
                        //after processLine if message not null it will return
                        if (!string.IsNullOrEmpty(message))
                        {
                            return message;
                        }
                    }
                    //	********* Expense Line Loop
                }
                else
                {
                    message = Msg.GetMsg(GetCtx(), "NoRecForSO");
                }
                dt = null;
                // dt.Clear();
            }
            catch (Exception e)
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }

                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }
            }
            CompleteOrder();
            if (_noOrders > 0)
            {
                message = "" + _noOrders + " " + Msg.GetMsg(GetCtx(), "OrderCrtdTimeRep");
            }
            return message;
        }   //	doIt

        /// <summary>
        /// Process Expense Line
        /// </summary>
        /// <param name="te">header</param>
        /// <param name="tel">line</param>
        /// <param name="bp">bp</param>
        private void ProcessLine(MTimeExpense te, MTimeExpenseLine tel, MBPartner bp)
        {
            if (_order == null)
            {
                log.Info("New Order for " + bp + ", Project=" + tel.GetC_Project_ID());
                _order = new MOrder(GetCtx(), 0, Get_TrxName());
                _order.SetAD_Org_ID(tel.GetAD_Org_ID());
                _order.SetC_DocTypeTarget_ID(MOrder.DocSubTypeSO_Standard);
                //
                _order.SetBPartner(bp);
                if (_order.GetC_BPartner_Location_ID() == 0)
                {
                    log.Log(Level.SEVERE, "No BP Location: " + bp);
                    AddLog(0, te.GetDateReport(),
                        null, "No Location: " + te.GetDocumentNo() + " " + bp.GetName());
                    _order = null;
                    return;
                }
                _order.SetM_Warehouse_ID(te.GetM_Warehouse_ID());
                //Bhupendra: Add payment term 
                // to check for if payment term is null
                if (bp.GetC_PaymentTerm_ID() == 0)
                {
                    // set the default payment method as check
                    int payTerm = GetPaymentTerm();
                    if (payTerm <= 0)
                    {
                        message = Msg.GetMsg(GetCtx(), "IsActivePaymentTerm");
                        return;
                    }
                    else
                    {
                        _order.SetC_PaymentTerm_ID(payTerm);
                    }
                }
                else
                {
                    //check weather paymentterm is active or not
                    if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM C_PaymentTerm WHERE C_PaymentTerm_ID=" + bp.GetC_PaymentTerm_ID(), null, Get_Trx())).Equals("Y"))
                    {
                        _order.SetC_PaymentTerm_ID(bp.GetC_PaymentTerm_ID());
                    }
                    else
                    {
                        message = Msg.GetMsg(GetCtx(), "IsActivePaymentTerm");
                        return;
                    }
                }
                // Bhupendra: added a cond to check for payment method if null
                // Added by mohit - to set payment method and sales rep id.
                if (bp.GetVA009_PaymentMethod_ID() == 0)
                {
                    // set the default payment method as check
                    int paymethod = GetPaymentMethod();
                    if (paymethod <= 0)
                    {
                        message = Msg.GetMsg(GetCtx(), "IsActivePaymentMethod");
                        return;
                    }
                    else
                    {
                        _order.SetVA009_PaymentMethod_ID(paymethod);
                    }
                }
                else
                {
                    //check weather the PaymentMethod is active or not
                    if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID=" + bp.GetVA009_PaymentMethod_ID(), null, Get_Trx())).Equals("Y"))
                    {
                        _order.SetVA009_PaymentMethod_ID(bp.GetVA009_PaymentMethod_ID());
                    }
                    else
                    {
                        message = Msg.GetMsg(GetCtx(), "IsActivePaymentMethod");
                        return;
                    }
                }
                _order.SetSalesRep_ID(te.GetDoc_User_ID());

                ////Added By Arpit asked by Surya Sir..................29-12-2015
                //_order.SetSalesRep_ID(GetCtx().GetAD_User_ID());
                //End
                if (tel.GetC_Activity_ID() != 0)
                {
                    _order.SetC_Activity_ID(tel.GetC_Activity_ID());
                }
                if (tel.GetC_Campaign_ID() != 0)
                {
                    _order.SetC_Campaign_ID(tel.GetC_Campaign_ID());
                }
                if (tel.GetC_Project_ID() != 0)
                {
                    _order.SetC_Project_ID(tel.GetC_Project_ID());
                    //	Optionally Overwrite BP Price list from Project
                    MProject project = new MProject(GetCtx(), tel.GetC_Project_ID(), Get_TrxName());
                    if (project.GetM_PriceList_ID() != 0)
                        //check weather the PriceList is active or not
                        if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM M_PriceList WHERE M_PriceList_ID=" + project.GetM_PriceList_ID(), null, Get_Trx())).Equals("Y"))
                        {
                            _order.SetM_PriceList_ID(project.GetM_PriceList_ID());
                        }
                        else
                        {
                            message = Msg.GetMsg(GetCtx(), "IsActivePriceList");
                            return;
                        }
                }
                else
                {
                    if (bp.GetM_PriceList_ID() != 0)
                        if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM M_PriceList WHERE M_PriceList_ID=" + bp.GetM_PriceList_ID(), null, Get_Trx())).Equals("Y"))
                        {
                            _order.SetM_PriceList_ID(bp.GetM_PriceList_ID());
                        }
                        else
                        {
                            message = Msg.GetMsg(GetCtx(), "IsActivePriceList");
                            return;
                        }
                }
                _order.SetSalesRep_ID(te.GetDoc_User_ID());
                //
                if (!_order.Save())
                {
                    Rollback();
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        message = pp.GetName();
                        //if GetName is Empty then it will check GetValue
                        if (string.IsNullOrEmpty(message))
                        {
                            message = Msg.GetMsg("", pp.GetValue());
                        }
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        message = Msg.GetMsg(GetCtx(), "CantSaveOrder");
                    }
                    return;
                    //throw new Exception("Cannot save Order");
                }
            }
            else
            {
                //	Update Header info
                if (tel.GetC_Activity_ID() != 0 && tel.GetC_Activity_ID() != _order.GetC_Activity_ID())
                {
                    _order.SetC_Activity_ID(tel.GetC_Activity_ID());
                }
                if (tel.GetC_Campaign_ID() != 0 && tel.GetC_Campaign_ID() != _order.GetC_Campaign_ID())
                {
                    _order.SetC_Campaign_ID(tel.GetC_Campaign_ID());
                }
                if (!_order.Save())
                {
                    Rollback();
                    //get error message from ValueNamePair
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        message = pp.GetName();
                        //if GetName is Empty then it will check GetValue
                        if (string.IsNullOrEmpty(message))
                        {
                            message = Msg.GetMsg("", pp.GetValue());
                        }
                    }
                    //it will check message is null or not
                    if (string.IsNullOrEmpty(message))
                    {
                        message = Msg.GetMsg(GetCtx(), "CantSaveOrder");
                    }
                    return;
                    //new Exception("Cannot save Order");
                }
            }

            //	OrderLine
            MOrderLine ol = new MOrderLine(_order);
                        
            if (tel.GetM_Product_ID() != 0)
            {
                ol.SetM_Product_ID(tel.GetM_Product_ID(),
                    tel.GetC_UOM_ID());
                //190 - Get Print description and set                                
                if (ol.Get_ColumnIndex("PrintDescription") >= 0)
                {
                    string printDesc = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocumentNote FROM M_Product WHERE M_Product_ID=" + tel.GetM_Product_ID()));
                    ol.Set_Value("PrintDescription", printDesc);
                }
            }
            if (tel.GetS_ResourceAssignment_ID() != 0)
            {
                ol.SetS_ResourceAssignment_ID(tel.GetS_ResourceAssignment_ID());
            }
            // Set charge ID
            if (tel.GetC_Charge_ID() != 0)
            {
                ol.SetC_Charge_ID(tel.GetC_Charge_ID());
                ol.SetPriceActual(tel.GetExpenseAmt());
                ol.SetQty(tel.GetQty());
                //190 - Get Print description and set                
                if (ol.Get_ColumnIndex("PrintDescription") >= 0)
                {
                    string printDesc = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT PrintDescription FROM C_Charge WHERE C_Charge_ID=" + tel.GetC_Charge_ID()));
                    ol.Set_Value("PrintDescription", printDesc);
                }
            }
            ol.SetQty(tel.GetQtyInvoiced());        //	
            ol.SetDescription(tel.GetDescription());
            //
            ol.SetC_Project_ID(tel.GetC_Project_ID());
            ol.SetC_ProjectPhase_ID(tel.GetC_ProjectPhase_ID());
            ol.SetC_ProjectTask_ID(tel.GetC_ProjectTask_ID());
            ol.SetC_Activity_ID(tel.GetC_Activity_ID());
            ol.SetC_Campaign_ID(tel.GetC_Campaign_ID());
            //
            Decimal price = tel.GetPriceInvoiced(); //	
            if (price.CompareTo(Env.ZERO) != 0)
            {
                if (tel.GetC_Currency_ID() != _order.GetC_Currency_ID())
                    price = MConversionRate.Convert(GetCtx(), price,
                        tel.GetC_Currency_ID(), _order.GetC_Currency_ID(),
                        _order.GetAD_Client_ID(), _order.GetAD_Org_ID());
                ol.SetPrice(price);
                // added by Bhupendra to set the entered price
                ol.SetPriceEntered(price);
            }
            else
            {
                ol.SetPrice();
            }
            if (tel.GetC_UOM_ID() != 0 && ol.GetC_UOM_ID() == 0)
            {
                ol.SetC_UOM_ID(tel.GetC_UOM_ID());
            }
            ol.SetTax();
            if (!ol.Save())
            {
                Rollback();
                //get error message from ValueNamePair
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    message = pp.GetName();
                    //if GetName is Empty then it will check GetValue
                    if (string.IsNullOrEmpty(message))
                    {
                        message = Msg.GetMsg("", pp.GetValue());
                    }
                }
                //it will check message is null or not
                if (string.IsNullOrEmpty(message))
                {
                    message = Msg.GetMsg(GetCtx(), "CantSaveOrderLine");
                }
                return;
                //throw new Exception("Cannot save Order Line");
            }
            //	Update TimeExpense Line
            tel.SetC_OrderLine_ID(ol.GetC_OrderLine_ID());
            if (tel.Save())
            {
                log.Fine("Updated " + tel + " with C_OrderLine_ID");
            }
            else
            {
                log.Log(Level.SEVERE, "Not Updated " + tel + " with C_OrderLine_ID");
            }

        }   //	processLine

        /// <summary>
        /// Complete Order
        /// </summary>
        private void CompleteOrder()
        {
            if (_order == null)
            {
                return;
            }
            // commnted by Mohit- asked by puneet to create the order in drafted mode.
            // _order.SetDocAction(DocActionVariables.ACTION_PREPARE);
            //_order.ProcessIt(DocActionVariables.ACTION_PREPARE);
            //if (!_order.Save())
            //{
            //    throw new Exception("Cannot save Order");
            //}
            _noOrders++;
            AddLog(_order.Get_ID(), _order.GetDateOrdered(), _order.GetGrandTotal(), _order.GetDocumentNo());
            _order = null;
        }   //	completeOrder
        /// <summary>
        /// to get the payment method if no payment method found on the business partner
        /// </summary>
        /// <returns>returns payment meyhod ID</returns>
        public int GetPaymentMethod()
        {
            //get organisation default 
            //added IsActive condition to check weather the paymentmethod is active or not
            string sql = "SELECT VA009_PaymentMethod_ID FROM VA009_PaymentMethod WHERE VA009_PAYMENTBASETYPE='S' AND AD_ORG_ID IN(@param1,0) AND AD_Client_ID=" + tel.GetAD_Client_ID() + "  AND IsActive='Y' ORDER BY AD_ORG_ID DESC, VA009_PAYMENTMETHOD_ID DESC";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param1", tel.GetAD_Org_ID());
            dynamic pri = DataBase.DB.ExecuteScalar(sql, param, Get_TrxName());
            return Convert.ToInt32(pri);
        }
        /// <summary>
        ///   to get the payment method if no payment term found on the business partner
        /// </summary>
        /// <returns> returns payment term ID</returns>
        public int GetPaymentTerm()
        {
            //added IsActive condition to check weather the term is active or not
            string sql = "SELECT C_PaymentTerm_ID FROM C_PaymentTerm WHERE ISDEFAULT='Y' AND AD_ORG_ID IN(@param1,0) AND IsActive='Y' AND AD_Client_ID=" + tel.GetAD_Client_ID() + " ORDER BY AD_ORG_ID DESC, C_PaymentTerm_ID DESC";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param1", tel.GetAD_Org_ID());
            dynamic pri = DataBase.DB.ExecuteScalar(sql, param, Get_TrxName());
            return Convert.ToInt32(pri);
        }
    }	//	ExpenseSOrder

}
