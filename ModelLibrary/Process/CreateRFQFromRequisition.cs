using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class CreateRFQFromRequisition : SvrProcess
    {
        private int VAF_Org_ID = 0; // Variable for rfq organization input.
        private int Requisition_Org_ID = 0; // Variable for filtering requisition by organization.
        private int Warehouse_ID = 0; // Variable for filtering requisition by warehouse.
        private string Req = ""; // variable for processing only selected requisition.        
        private int RfQTopic_ID = 0; // Variable for rfq topic input.
        private string RfQtype = ""; // Variable for rfq type input.
        DateTime? DocDateFrom = null, DocDateTo = null; // Document date filter on requisition.
        DateTime? ReqDateFrom = null, ReqDateTo = null; // Requested date filter on requisition.
        DateTime? DateResponse = null;
        private bool isConsolidate = false; // Create consolidated document or not.
        private int VAB_Currency_ID = 0; // Currency to map on rfq header.


        protected override string DoIt()
        {
            // Passed parameters info
            log.Info("Process start - Parameter - VAF_Org_ID=" + VAF_Org_ID
                + ", Requisition Organization=" + Requisition_Org_ID
                 + ", Warehouse=" + Warehouse_ID
                 + " Requisition=" + Req
                 + " RfQ Topic=" + RfQTopic_ID
                 + " RfQ Type=" + RfQtype
                + ", DocDate=" + DocDateFrom + "/" + DocDateTo
                + ", DateRequired=" + ReqDateFrom + "/" + ReqDateTo
                + ", Currency =" + VAB_Currency_ID
                + ", ConsolidateDocument" + isConsolidate);

            StringBuilder Sql = new StringBuilder();
            DataSet _dsReq = null;
            string Result = "";

            Sql.Append(@"SELECT 
                        ReqLine.VAM_Requisition_ID,
                        ReqLine.VAM_RequisitionLine_ID,
                        reqline.VAM_Product_ID,
                        reqline.VAM_PFeature_SetInstance_ID,
                        reqline.Description,
                        reqline.VAB_UOM_ID,
                        (reqline.Qty - reqline.DTD001_DeliveredQty) as Qty ,
                        req.DateRequired
                        FROM VAM_RequisitionLine ReqLine
                        INNER JOIN VAM_Requisition req
                        ON (reqline.VAM_Requisition_ID  =req.VAM_Requisition_ID)
                        WHERE ReqLine.IsActive        ='Y' AND req.IsActive        ='Y'
                        AND ReqLine.vaf_org_ID         =" + Requisition_Org_ID + " AND req.DocStatus='CO' AND reqline.Qty!=reqline.DTD001_DeliveredQty  ");
            // Requisition selection check
            if (!string.IsNullOrEmpty(Req))
            {
                Sql.Append(" AND ReqLine.VAM_Requisition_ID IN (" + Req + ") ");
            }
            // Warehouse selection check
            if (Warehouse_ID > 0)
            {
                Sql.Append(" AND Req.VAM_Warehouse_ID        =" + Warehouse_ID + " ");
            }
            // Document Date Check
            if (DocDateFrom != null && DocDateTo != null)
            {
                Sql.Append(" AND TRUNC( Req.DateDoc) BETWEEN " + GlobalVariable.TO_DATE(DocDateFrom, true) + "  AND " + GlobalVariable.TO_DATE(DocDateTo, true) + " ");
            }
            else if (DocDateFrom != null)
            {
                Sql.Append(" AND TRUNC( Req.DateDoc) >=" + GlobalVariable.TO_DATE(DocDateFrom, true) + " ");
            }
            else if (DocDateTo != null)
            {
                Sql.Append(" AND TRUNC( Req.DateDoc) =< " + GlobalVariable.TO_DATE(DocDateTo, true) + "");
            }
            // Required Date Check
            if (ReqDateFrom != null && ReqDateTo != null)
            {
                Sql.Append(" AND TRUNC( Req.DateRequired ) BETWEEN " + GlobalVariable.TO_DATE(ReqDateFrom, true) + "  AND " + GlobalVariable.TO_DATE(ReqDateTo, true) + " ");
            }
            else if (ReqDateFrom != null)
            {
                Sql.Append(" AND TRUNC( Req.DateRequired ) >=" + GlobalVariable.TO_DATE(ReqDateFrom, true) + " ");
            }
            else if (ReqDateTo != null)
            {
                Sql.Append(" AND TRUNC( Req.DateRequired ) =< " + GlobalVariable.TO_DATE(ReqDateTo, true) + "");
            }
            if (!isConsolidate)
            {
                Sql.Append(" ORDER BY ReqLine.VAM_Requisition_ID ");
            }
            else
            {
                Sql.Append(" ORDER BY req.DateRequired ASC ");
            }
            try
            {
                _dsReq = DB.ExecuteDataset(Sql.ToString());
                if (_dsReq != null && _dsReq.Tables[0].Rows.Count > 0)
                {
                    Result = CreateRfQ(_dsReq);
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "NoReqFound");
                }

            }
            catch (Exception e)
            {
                if (_dsReq != null)
                {
                    _dsReq.Dispose();
                    _dsReq = null;
                }
                Get_TrxName().Rollback();
                return e.Message;
            }

            if (_dsReq != null)
            {
                _dsReq.Dispose();
                _dsReq = null;
            }

            log.Info(Result);
            return Result;
        }
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
                else if (name.Equals("VAF_Org_ID"))
                {
                    VAF_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("OrgColumn"))
                {
                    Requisition_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_Warehouse_ID"))
                {
                    Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_Currency_ID"))
                {
                    VAB_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAM_Requisition_ID"))
                {
                    Req = para[i].GetParameter().ToString();
                }

                else if (name.Equals("DateDoc"))
                {
                    DocDateFrom = (DateTime?)para[i].GetParameter();
                    DocDateTo = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("DateRequired"))
                {
                    ReqDateFrom = (DateTime?)para[i].GetParameter();
                    ReqDateTo = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("QuoteType"))
                {
                    RfQtype = (String)para[i].GetParameter();
                }
                else if (name.Equals("VAB_RFQ_Subject_ID"))
                {
                    RfQTopic_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateResponse"))
                {
                    DateResponse = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    isConsolidate = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="_ds"></param> dataset contains all the records according to selection criteria.
        /// <returns></returns> returns the final message.
        private string CreateRfQ(DataSet _ds)
        {
            MVABRfQ rfq = null;
            int Requisition_ID = 0, LineNo = 0;
            string message = "";
            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {

                // If document is not consolidated
                if (!isConsolidate)
                {
                    if (rfq == null || Requisition_ID != Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_Requisition_ID"]))
                    {
                        LineNo = 0;
                        Requisition_ID = Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_Requisition_ID"]);
                        rfq = new MVABRfQ(GetCtx(), 0, Get_TrxName());
                        rfq.SetVAF_Org_ID(VAF_Org_ID);
                        rfq.SetName("Name");
                        rfq.SetSalesRep_ID(GetCtx().GetVAF_UserContact_ID());
                        rfq.SetVAB_RFQ_Subject_ID(RfQTopic_ID);
                        rfq.SetVAM_Requisition_ID(Requisition_ID);
                        rfq.SetDateWorkStart(System.DateTime.Now);
                        rfq.SetDateResponse(DateResponse);      // Added by Bharat on 15 Jan 2019 as asked by Puneet
                        if (Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["DateRequired"]) >= System.DateTime.Now)
                        {
                            rfq.SetDateWorkComplete(Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["DateRequired"]));
                        }
                        if (string.IsNullOrEmpty(RfQtype))
                        {
                            rfq.SetQuoteType("S");
                        }
                        else
                        {
                            rfq.SetQuoteType(RfQtype);
                        }
                        rfq.SetIsInvitedVendorsOnly(true);
                        rfq.SetIsQuoteAllQty(true);
                        rfq.SetIsRfQResponseAccepted(true);
                        rfq.SetVAB_Currency_ID(VAB_Currency_ID);
                        if (rfq.Save())
                        {
                            DB.ExecuteQuery("UPDATE VAB_RFQ SET Name='" + rfq.GetDocumentNo() + "' WHERE VAB_RFQ_ID= " + rfq.GetVAB_RFQ_ID(), null, Get_TrxName());
                            if (message == "")
                            {
                                message = Msg.GetMsg(GetCtx(), "RfQGeneratedSuccess") + " =" + rfq.GetDocumentNo();
                            }
                            else
                            {
                                message = message + "," + rfq.GetDocumentNo();
                            }
                        }
                        else
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "RfQHeadNotSaved") + "- " + vp.Name;
                            }
                            else
                            {
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "RfQHeadNotSaved");
                            }

                        }
                    }
                }
                // If document is consolidated
                else
                {
                    if (rfq == null)
                    {
                        rfq = new MVABRfQ(GetCtx(), 0, Get_TrxName());
                        rfq.SetVAF_Org_ID(VAF_Org_ID);
                        rfq.SetName("Name");
                        rfq.SetSalesRep_ID(GetCtx().GetVAF_UserContact_ID());
                        rfq.SetVAB_RFQ_Subject_ID(RfQTopic_ID);
                        rfq.SetDateWorkStart(System.DateTime.Now);
                        rfq.SetDateResponse(DateResponse);      // Added by Bharat on 15 Jan 2019 as asked by Puneet
                        if (Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["DateRequired"]) >= System.DateTime.Now)
                        {
                            rfq.SetDateWorkComplete(Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["DateRequired"]));
                        }
                        if (string.IsNullOrEmpty(RfQtype))
                        {
                            rfq.SetQuoteType("S");
                        }
                        else
                        {
                            rfq.SetQuoteType(RfQtype);
                        }
                        rfq.SetIsInvitedVendorsOnly(true);
                        rfq.SetIsQuoteAllQty(true);
                        rfq.SetIsRfQResponseAccepted(true);
                        rfq.SetVAB_Currency_ID(VAB_Currency_ID);
                        if (!rfq.Save())
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                Get_TrxName().Rollback();
                                return "RFQ Not Saved - " + vp.Name;
                            }
                            else
                            {
                                Get_TrxName().Rollback();
                                return "RFQ Not Saved";
                            }
                        }
                        else
                        {
                            DB.ExecuteQuery("UPDATE VAB_RFQ SET Name='" + rfq.GetDocumentNo() + "' WHERE VAB_RFQ_ID= " + rfq.GetVAB_RFQ_ID(), null, Get_TrxName());
                        }
                        message = "RfQ Generated =" + rfq.GetDocumentNo();

                    }
                }
                LineNo = LineNo + 10;
                // Create RfQ line
                MVABRfQLine RfqLine = new MVABRfQLine(rfq);
                RfqLine.SetLine(LineNo);
                RfqLine.SetVAM_RequisitionLine_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_RequisitionLine_ID"]));
                RfqLine.SetVAM_Product_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_Product_ID"]));
                if (Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]) > 0)
                {
                    RfqLine.SetVAM_PFeature_SetInstance_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]));
                }
                RfqLine.SetDescription(Util.GetValueOfString(_ds.Tables[0].Rows[i]["Description"]));
                if (RfqLine.Save())
                {
                    // Create RfQ Qty
                    MVABRfQLineQty RfQLineQty = new MVABRfQLineQty(RfqLine);
                    RfQLineQty.SetVAB_UOM_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAB_UOM_ID"]));
                    RfQLineQty.SetQty(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["Qty"]));
                    RfQLineQty.SetIsPurchaseQty(true);
                    RfQLineQty.SetIsRfQQty(true);
                    if (!RfQLineQty.Save())
                    {
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            Get_TrxName().Rollback();
                            return Msg.GetMsg(GetCtx(), "RfQLineQtyNotSaved") + "- " + vp.Name;
                        }
                        else
                        {
                            Get_TrxName().Rollback();
                            return Msg.GetMsg(GetCtx(), "RfQLineQtyNotSaved");
                        }

                    }
                }
                else
                {
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        Get_TrxName().Rollback();
                        return Msg.GetMsg(GetCtx(), "RfQLineNotSaved") + "- " + vp.Name;
                    }
                    else
                    {
                        Get_TrxName().Rollback();
                        return Msg.GetMsg(GetCtx(), "RfQLineNotSaved");
                    }
                }

            }

            return message;
        }


    }
}
