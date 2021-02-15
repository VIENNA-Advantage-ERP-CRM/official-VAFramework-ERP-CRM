/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyOrder
 * Purpose        : Copy Order and optionally close
 * Class Used     : SvrProcess
 * Chronological    Development
 * Raghunandan     03-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
////using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class CopyOrder : SvrProcess
    {
        #region Private Variable
        //Order to Copy				
        private int _VAB_Order_ID = 0;
        // Document Type of new Order	
        private int _VAB_DocTypes_ID = 0;
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
                else if (name.Equals("VAB_Order_ID"))
                {
                    _VAB_Order_ID = Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = Util.GetValueOfInt(para[i].GetParameter());//.intValue();
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
            log.Info("VAB_Order_ID=" + _VAB_Order_ID
                + ", VAB_DocTypes_ID=" + _VAB_DocTypes_ID
                + ", CloseDocument=" + _IsCloseDocument);
            if (_VAB_Order_ID == 0)
            {
                throw new ArgumentException("No Order");
            }
            VAdvantage.Model.MVABDocTypes dt = VAdvantage.Model.MVABDocTypes.Get(GetCtx(), _VAB_DocTypes_ID);
            if (dt.Get_ID() == 0)
            {
                throw new ArgumentException("No DocType");
            }
            if (_DateDoc == null)
            {
                _DateDoc = Util.GetValueOfDateTime(DateTime.Now);
            }
            //
            VAdvantage.Model.MVABOrder from = new VAdvantage.Model.MVABOrder(GetCtx(), _VAB_Order_ID, Get_Trx());
            if (from.GetDocStatus() != "DR" && from.GetDocStatus() != "IP" && from.GetDocStatus() != "CO")
            {
                throw new Exception("Order Closed");
            }
            //JID_1799 fromCreateSo is true if DOCBASETYPE='BOO'
            VAdvantage.Model.MVABOrder newOrder = VAdvantage.Model.MVABOrder.CopyFrom(from, _DateDoc, dt.GetVAB_DocTypes_ID(), false, true, null,
                dt.GetDocBaseType().Equals(MVABMasterDocType.DOCBASETYPE_BLANKETSALESORDER) ? true : false);		//	copy ASI 
            newOrder.SetVAB_DocTypesTarget_ID(_VAB_DocTypes_ID);
            int VAB_BusinessPartner_ID = newOrder.GetVAB_BusinessPartner_ID();
            newOrder.Set_Value("IsSalesQuotation", false);

            // Added by Bharat on 05 Jan 2018 to set Values on Blanket Sales Order from Sales Quotation.
            if (dt.GetDocBaseType() == "BOO")
            {
                newOrder.Set_Value("IsBlanketTrx", true);
            }
            else   // Added by Bharat on 29 March 2018 to set Blanket Order zero in case of Sales order Creation.
            {
                newOrder.SetVAB_Order_Blanket(0);
            }
            if (newOrder.Get_ColumnIndex("VAB_Order_Quotation") > 0)
                newOrder.SetVAB_Order_Quotation(_VAB_Order_ID);

            //Update New Order Refrence From Sales Qutation in Sales order
            newOrder.SetPOReference(Util.GetValueOfString(from.GetDocumentNo()));

            // Added by Bharat on 31 Jan 2018 to set Inco Term from Quotation

            if (newOrder.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
            {
                newOrder.SetVAB_IncoTerm_ID(from.GetVAB_IncoTerm_ID());
            }

            String sqlbp = "update VAB_Project set VAB_BusinessPartner_id=" + VAB_BusinessPartner_ID + "  where ref_order_id=" + _VAB_Order_ID + "";
            int value = DB.ExecuteQuery(sqlbp, null, Get_Trx());
            bool OK = newOrder.Save();
            if (!OK)
            {
                //return GetReterivedError( newOrder,  "Could not create new Order"); 
                throw new Exception("Could not create new Order");
            }
            if (OK)
            {
                string sql = "select VAB_Project_id from VAB_Project where VAB_Order_id = " + from.GetVAB_Order_ID();
                int VAB_Project_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (VAB_Project_ID != 0)
                {
                    VAdvantage.Model.X_VAB_Project project = new VAdvantage.Model.X_VAB_Project(GetCtx(), VAB_Project_ID, Get_Trx());
                    project.SetVAB_BusinessPartner_ID(project.GetVAB_BusinessPartnerSR_ID());
                    project.SetVAB_BusinessPartnerSR_ID(0);
                    if (!project.Save())
                    {
                        log.SaveError("Error on " + project.Get_TableName(),"");
                    }
                }
                if (dt.GetDocBaseType() == "BOO")
                {
                    from.SetVAB_Order_Blanket(newOrder.GetVAB_Order_ID());
                }
                else
                {
                    from.SetRef_Order_ID(newOrder.GetVAB_Order_ID());                    
                }
                from.Save();
                int bp = newOrder.GetVAB_BusinessPartner_ID();
                VAdvantage.Model.X_VAB_BusinessPartner prosp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), bp, Get_Trx());
                prosp.SetIsCustomer(true);
                prosp.SetIsProspect(false);
                if (!prosp.Save()) {

                    log.SaveError("Error on " + prosp.Get_TableName(), "");
                }
            }

            //
            if (_IsCloseDocument)
            {
                VAdvantage.Model.MVABOrder original = new VAdvantage.Model.MVABOrder(GetCtx(), _VAB_Order_ID, Get_Trx());
                //Edited by Arpit Rai on 8th of Nov,2017
                if (original.GetDocStatus() != "CO") //to check if document is already completed
                {
                    original.SetDocAction(VAdvantage.Model.MVABOrder.DOCACTION_Complete);
                    original.ProcessIt(VAdvantage.Model.MVABOrder.DOCACTION_Complete);
                    original.Save();
                }
                //Arpit
                original.SetDocAction(VAdvantage.Model.MVABOrder.DOCACTION_Close);
                original.ProcessIt(VAdvantage.Model.MVABOrder.DOCACTION_Close);
                original.Save();
            }
            //
            return Msg.GetMsg(GetCtx(), "OrderCreatedSuccessfully") + " - " + dt.GetName() + ": " + newOrder.GetDocumentNo();
        }
    }
}
