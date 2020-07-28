/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyOrder
 * Purpose        : Copy Order and optionally close
 * Class Used     : SvrProcess
 * Chronological    Development
 * Karan            18-May-2011
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CopyOrder : SvrProcess
    {
        #region Private Variable 
        //Order to Copy				
        private int _C_Order_ID = 0;
        // Document Type of new Order	
        private int _C_DocType_ID = 0;
        // New Doc Date				
        private DateTime? _DateDoc = null;
        //Close/Process Old Order		
        private bool _IsCloseDocument = false;
        #endregion

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
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID =Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID =Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("DateDoc"))
                {
                    _DateDoc = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("IsCloseDocument"))
                {
                    _IsCloseDocument = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            //log.Info("C_Order_ID=" + _C_Order_ID
            //    + ", C_DocType_ID=" + _C_DocType_ID
            //    + ", CloseDocument=" + _IsCloseDocument);
            //if (_C_Order_ID == 0)
            //{
            //    throw new ArgumentException("No Order");
            //}
            //MDocType dt = MDocType.Get(GetCtx(), _C_DocType_ID);
            //if (dt.Get_ID() == 0)
            //{
            //    throw new ArgumentException("No DocType");
            //}
            //if (_DateDoc == null)
            //{
            //    _DateDoc = DateTime.Now;
            //  //Util.GetValueOfDateTime(new DateTime(CommonFunctions.CurrentTimeMillis()));
            //}
            ////
            //MOrder from = new MOrder(GetCtx(), _C_Order_ID, Get_Trx());
            //MOrder newOrder = MOrder.CopyFrom(from, _DateDoc,
            //    dt.GetC_DocType_ID(), false, true, null);		//	copy ASI
            //newOrder.SetC_DocTypeTarget_ID(_C_DocType_ID);
            //bool OK = newOrder.Save ();
            //if (!OK)
            //{
            //    throw new Exception("Could not create new Order");
            //}
            ////
            //if (_IsCloseDocument)
            //{
            //    MOrder original = new MOrder(GetCtx(), _C_Order_ID, Get_Trx());
            //    original.SetDocAction(MOrder.DOCACTION_Complete);
            //    original.ProcessIt(MOrder.DOCACTION_Complete);
            //    original.Save();
            //    original.SetDocAction(MOrder.DOCACTION_Close);
            //    original.ProcessIt(MOrder.DOCACTION_Close);
            //    original.Save();
            //}
            ////
            //return dt.GetName() + ": " + newOrder.GetDocumentNo();


            log.Info("C_Order_ID=" + _C_Order_ID
                + ", C_DocType_ID=" + _C_DocType_ID
                + ", CloseDocument=" + _IsCloseDocument);
            if (_C_Order_ID == 0)
            {
                throw new ArgumentException("No Order");
            }
            VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), _C_DocType_ID);
            if (dt.Get_ID() == 0)
            {
                throw new ArgumentException("No DocType");
            }
            if (_DateDoc == null)
            {
                _DateDoc = Util.GetValueOfDateTime(DateTime.Now);
            }
            //
            VAdvantage.Model.MOrder from = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
            MOrder newOrder = MOrder.CopyFrom(from, _DateDoc,
                dt.GetC_DocType_ID(), false, true, null, true);//Pass optional parameter as True that we are going to create Order from Create Sales Order Process on Sales Quotation window---Neha
            newOrder.SetC_DocTypeTarget_ID(_C_DocType_ID);
            //Update New Order Refrence From Sales Qutation in Sales order
            newOrder.SetPOReference(Util.GetValueOfString(from.GetDocumentNo()));
            int C_Bpartner_ID = newOrder.GetC_BPartner_ID();
            newOrder.Set_Value("IsSalesQuotation", false);

            // Added by Bharat on 31 Jan 2018 to set Inco Term from Quotation

            if (newOrder.Get_ColumnIndex("C_IncoTerm_ID") > 0)
            {
                newOrder.SetC_IncoTerm_ID(from.GetC_IncoTerm_ID());
            }
            String sqlbp = "update c_project set c_bpartner_id=" + C_Bpartner_ID + "  where ref_order_id=" + _C_Order_ID + "";
            int value = DB.ExecuteQuery(sqlbp, null, Get_Trx());
            bool OK = newOrder.Save();
            if (!OK)
            {
                throw new Exception("Could not create new Order");
            }
            if (OK)
            {
                string sql = "select C_Project_id from c_project where c_order_id = " + from.GetC_Order_ID();
                int C_Project_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (C_Project_ID != 0)
                {
                    VAdvantage.Model.X_C_Project project = new VAdvantage.Model.X_C_Project(GetCtx(), C_Project_ID, Get_Trx());
                    project.SetC_BPartner_ID(project.GetC_BPartnerSR_ID());
                    project.SetC_BPartnerSR_ID(0);
                    if (!project.Save())
                    {

                    }
                }
                from.SetRef_Order_ID(newOrder.GetC_Order_ID());
                from.Save();
                int bp = newOrder.GetC_BPartner_ID();
                VAdvantage.Model.X_C_BPartner prosp = new VAdvantage.Model.X_C_BPartner(GetCtx(), bp, Get_Trx());
                prosp.SetIsCustomer(true);
                prosp.SetIsProspect(false);
                prosp.Save();
            }

            //
            if (_IsCloseDocument)
            {
                VAdvantage.Model.MOrder original = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
                //Edited by Arpit Rai on 8th of Nov,2017
                if (original.GetDocStatus() != "CO") //to check if document is already completed
                {
                    original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Complete);
                    original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Complete);
                    original.Save();
                }
                //Arpit
                original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Close);
                original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Close);
                original.Save();
            }
            //
            return Msg.GetMsg(GetCtx(), "OrderCreatedSuccessfully") + " - " + dt.GetName() + ": " + newOrder.GetDocumentNo();
        }
    }
}
