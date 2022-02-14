/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CreateReleaseDocFromBO
 * Purpose        : Create Release Document against Blanket SO/PO
 * Class Used     : SvrProcess
 * Chronological  : Development
 * Arpit/Amit     : 06-Mar-2018
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using ViennaAdvantage.Model;
using VAdvantage.Model;

namespace VAdvantage.Process
{
    public class CreateReleaseDocFromBO : SvrProcess
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(CreateReleaseDocFromBO).FullName);
        private Int32 order_ID = 0;

        protected override string DoIt()
        {
            try
            {
                if (order_ID == 0)
                {
                    throw new ArgumentException("No Order");
                }

                MOrder from = new MOrder(GetCtx(), order_ID, Get_TrxName());

                // Check Validity date of Blanket Order.
                if (from.GetOrderValidTo() < DateTime.Now.Date)
                {
                    return Msg.GetMsg(GetCtx(), "VIS_BlanketNotValid");
                }

                MDocType dt = MDocType.Get(GetCtx(), from.GetC_DocType_ID());

                //Document Type against Release Order
                if (dt.GetDocumentTypeforReleases() == 0)
                {
                    return Msg.GetMsg(GetCtx(), "VIS_ReleaseDocumentnotFound");
                }

                if (from.GetDocStatus() == MOrder.DOCSTATUS_Drafted ||
                    from.GetDocStatus() == MOrder.DOCSTATUS_InProgress ||
                    from.GetDocStatus() == MOrder.DOCSTATUS_Voided ||
                    from.GetDocStatus() == MOrder.DOCSTATUS_Reversed)
                {
                    throw new Exception("Order Not Valid");
                }

                //Document Type against Release Order
                //MDocType dtt = MDocType.Get(GetCtx(), dt.GetDocumentTypeforReleases());
                //if (dtt == null)
                //{
                //    throw new Exception(Msg.GetMsg(GetCtx(), "VIS_ReleaseDocumentnotFound"));
                //}

                // JID_1474 if full quantity of all lines are released from blanket order and user run Release order process then system will not allow to create 
                // Release order and give message: 'All quantity are released'.

                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT SUM(qtyentered) FROM C_OrderLine WHERE C_Order_ID = " + GetRecord_ID(), null, Get_TrxName())) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "VIS_AllQtyReleased");
                }

                //Creating Release PO/SO against blanket Orders
                MOrder rposo = MOrder.CopyFrom(from, from.GetDateAcct(),
                   dt.GetDocumentTypeforReleases(), false, true, Get_TrxName(), false);

                rposo.SetPOReference(Util.GetValueOfString(from.GetDocumentNo()));
                rposo.Set_Value("IsBlanketTrx", false);                
                //1052-- set blanketordertype so that display logic could work if order is created from process
                rposo.Set_Value("BlanketOrderType","BO");              
                // JID_0890: On Blanket Sales/Purchase Order, Create Release Purchase/Sales Order Process was not working.
                //rposo.SetOrig_Order_ID(order_ID);
                rposo.SetC_Order_Blanket(GetRecord_ID()); //Set Blanket Order ID to release SO/PO
                rposo.SetIsSOTrx(dt.IsSOTrx());
                //Set Same Document Sequence FROM Document type-- for Document is Number Controlled
                //Commented because its creating same document no
                //if (dtt.IsDocNoControlled() && dtt.GetDocNoSequence_ID() > 0)
                //{
                //    MSequence seq = new MSequence(GetCtx(), dtt.GetDocNoSequence_ID(), Get_TrxName());
                //    rposo.SetDocumentNo(Util.GetValueOfString(seq.GetCurrentNext()));
                //}
                if (!rposo.Save(Get_TrxName()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    throw new Exception("Could not create new Release Order. " + (pp != null ? pp.GetName() : ""));
                }
                string msg = Msg.GetMsg(GetCtx(), "OrderCreatedSuccessfully");
                return msg + ":" + rposo.GetDocumentNo();
            }
            catch (Exception e)
            {
                //JID_1474 : if exception is found then we have to rollback and return that exception as suggested by Puneet and Gagandeep kaur
                Get_TrxName().Rollback();
                _log.SaveError("Could not create new Release Order", e);
                return e.Message;
            }
        }

        protected override void Prepare()
        {
            order_ID = GetRecord_ID();
        }
    }
}
