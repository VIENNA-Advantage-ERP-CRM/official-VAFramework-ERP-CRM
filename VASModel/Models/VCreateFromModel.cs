using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class VCreateFromModel
    {
        /// <summary>
        //// get orders
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="display">Display Columns</param>
        /// <param name="column">Column Name</param>
        /// <param name="VAB_BusinessPartner_ID">Business Partner</param>
        /// <param name="isReturnTrx">Return Transaction</param>       
        /// <param name="OrgId">Organization</param>
        /// <param name="DropShip">Drop Shipment</param>
        /// <param name="IsSOTrx">Sales Transaction</param>
        /// <param name="forInvoices">For Invoice</param>
        /// <param name="InvoiceID">VAB_Invoice_ID</param>
        /// <returns>List<VCreateFromGetCOrder>, List of Orders</returns>

        public List<VCreateFromGetCOrder> VCreateGetOrders(Ctx ctx, string display, string column, int VAB_BusinessPartner_ID, bool isReturnTrx, int OrgId, bool DropShip, bool IsSOTrx, bool forInvoices, int InvoiceID)
        {
            List<VCreateFromGetCOrder> obj = new List<VCreateFromGetCOrder>();
            var dis = display;
            MVAFClient tenant = MVAFClient.Get(ctx);

            // JID_0976
            string whereCondition = "";
            if (InvoiceID > 0)
            {
                DataSet dsInvoice = DB.ExecuteDataset(@"SELECT VAB_Currency_ID  ,
                CASE WHEN NVL(VAB_CurrencyType_ID , 0) != 0  THEN VAB_CurrencyType_ID ELSE 
                (SELECT MAX(VAB_CurrencyType_ID) FROM VAB_CurrencyType WHERE VAB_CurrencyType.VAF_Client_ID IN (0 , VAB_Invoice.VAF_Client_ID )
                AND VAB_CurrencyType.VAF_Org_ID      IN (0 , VAB_Invoice.VAF_Org_ID ) AND VAB_CurrencyType.IsDefault = 'Y') END AS VAB_CurrencyType_ID,
                VAM_PriceList_ID FROM VAB_Invoice WHERE VAB_Invoice_ID = " + InvoiceID);
                if (dsInvoice != null && dsInvoice.Tables.Count > 0 && dsInvoice.Tables[0].Rows.Count > 0)
                {
                    whereCondition = " AND o.VAB_Currency_ID = " + Convert.ToInt32(dsInvoice.Tables[0].Rows[0]["VAB_Currency_ID"]) +
                        " AND o.VAB_CurrencyType_ID = " + Util.GetValueOfInt(dsInvoice.Tables[0].Rows[0]["VAB_CurrencyType_ID"]) +
                        " AND o.VAM_PriceList_ID = " + Convert.ToInt32(dsInvoice.Tables[0].Rows[0]["VAM_PriceList_ID"]);
                }
            }
            //Added O.ISSALESQUOTATION='N' in where condition(Sales quotation will not display in Order dropdown)
            StringBuilder sql = new StringBuilder("SELECT o.VAB_Order_ID," + display + " AS displays FROM VAB_Order o WHERE o.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND o.IsSOTrx ='" + (IsSOTrx ? "Y" : "N")
                + "' AND O.IsBlanketTrx = 'N' AND O.ISSALESQUOTATION='N' AND o.DocStatus IN ('CL','CO') ");

            if (OrgId > 0)
            {
                sql.Append("AND o.VAF_Org_ID = " + OrgId);
            }

            if (!String.IsNullOrEmpty(whereCondition))
            {
                sql.Append(whereCondition);
            }

            // when create lines fom open from VAM_Inv_InOut then pick records from match po having VAM_Inv_InOutLine is not null 
            // when create lines fom open from M_Invoiceline then pick records from match po having m_invoiceline is not null 
            sql.Append("AND o.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' AND o.IsDropShip='" + (DropShip ? "Y" : "N") + "'  AND o.VAB_Order_ID IN "
            + @"(SELECT VAB_Order_ID FROM (SELECT ol.VAB_Order_ID,ol.VAB_OrderLine_ID,ol.QtyOrdered, 
            (SELECT SUM(m.qty) FROM VAM_MatchPO m WHERE ol.VAB_OrderLine_ID=m.VAB_OrderLine_ID AND NVL(" + column + @", 0) != 0 AND m.ISACTIVE = 'Y' ) AS Qty,
            (SELECT SUM(IL.QtyInvoiced)  FROM VAB_INVOICELINE IL INNER JOIN VAB_Invoice I ON I.VAB_INVOICE_ID = IL.VAB_INVOICE_ID
            WHERE il.ISACTIVE = 'Y' AND I.DOCSTATUS NOT IN ('VO','RE') AND OL.VAB_ORDERLINE_ID  =IL.VAB_ORDERLINE_ID) AS QtyInvoiced FROM VAB_OrderLine ol ");

            // Get Orders based on the setting taken on Tenant to allow non item Product
            if (!forInvoices && tenant.Get_ColumnIndex("IsAllowNonItem") > 0 && !tenant.IsAllowNonItem())
            {
                sql.Append("INNER JOIN VAM_Product p ON ol.VAM_Product_ID = p.VAM_Product_ID AND p.ProductType = 'I'");
            }

            sql.Append(") t GROUP BY VAB_Order_ID,VAB_OrderLine_ID,QtyOrdered "
            + "HAVING QtyOrdered > SUM(nvl(Qty,0)) AND QtyOrdered > SUM(NVL(QtyInvoiced,0))) ORDER BY o.DateOrdered, o.DocumentNo");

            DataSet ds = DB.ExecuteDataset(sql.ToString());

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Order_ID"]);
                    objc.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["displays"]);
                    obj.Add(objc);
                }
            }

            return obj;
        }

        /// <summary>
        ///  Get Shipment data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="displays"></param>
        /// <param name="CBPartnerIDs"></param>
        /// <returns></returns>
        public List<VCreateFromGetCOrder> GetShipments(Ctx ctx, string displays, int CBPartnerIDs, bool IsDrop, bool IsSOTrx)
        {
            List<VCreateFromGetCOrder> obj = new List<VCreateFromGetCOrder>();
            string sql = "SELECT s.VAM_Inv_InOut_ID," + displays + " AS dis FROM VAM_Inv_InOut s "
                + "WHERE s.VAB_BusinessPartner_ID=" + CBPartnerIDs + " AND s.IsSOTrx='" + (IsSOTrx ? "Y" : "N") + "' AND s.DocStatus IN ('CL','CO')"
               // New column added to fill invoice which drop ship is true
               + " AND s.IsDropShip='" + (IsDrop ? "Y" : "N") + "' AND s.VAM_Inv_InOut_ID IN "

               // Changes done by Bharat on 06 July 2017 restrict to create invoice if Invoice already created against that for same quantity

               + "(SELECT VAM_Inv_InOut_ID FROM (SELECT sl.VAM_Inv_InOut_ID, sl.VAM_Inv_InOutLine_ID, sl.MovementQty, mi.QtyInvoiced FROM VAM_Inv_InOutLine sl "
               + "LEFT OUTER JOIN (SELECT il.QtyInvoiced, il.VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice I ON I.VAB_INVOICE_ID = il.VAB_INVOICE_ID "
               + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON sl.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) t "
               + "GROUP BY VAM_Inv_InOut_ID, VAM_Inv_InOutLine_ID, MovementQty HAVING MovementQty > SUM(NVL(QtyInvoiced,0))) ORDER BY s.MovementDate, s.DocumentNo";

            //+ "(SELECT VAM_Inv_InOut_ID FROM (SELECT sl.VAM_Inv_InOut_ID,sl.VAM_Inv_InOutLine_ID,sl.MovementQty,mi.Qty,IL.QtyInvoiced FROM VAM_Inv_InOutLine sl "
            //+ "LEFT OUTER JOIN VAM_MatchInvoice mi ON (sl.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) "
            //+ " LEFT OUTER JOIN VAB_INVOICELINE IL    ON (sl.VAB_ORDERLINE_ID =IL.VAB_ORDERLINE_ID)"
            //+ " LEFT OUTER JOIN VAB_Invoice I   ON I.VAB_INVOICE_ID      =IL.VAB_INVOICE_ID "
            //+ " AND I.DOCSTATUS NOT   IN ('VO','RE') "
            //+ " WHERE (sl.MovementQty <> nvl(mi.Qty,0) OR SL.MovementQty     <> NVL(IL.QtyInvoiced,0)"
            //+ "AND mi.VAM_Inv_InOutLine_ID IS NOT NULL) OR mi.VAM_Inv_InOutLine_ID IS NULL ) GROUP BY VAM_Inv_InOut_ID,VAM_Inv_InOutLine_ID,MovementQty "
            //+ "HAVING MovementQty > SUM(nvl(Qty,0)) OR MovementQty    > SUM(NVL(QtyInvoiced,0)) ) ORDER BY s.MovementDate";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOut_ID"]);
                    objc.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["dis"]);
                    obj.Add(objc);
                }
            }
            return obj;
        }

        /// <summary>
        /// Unused Function
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="MVAMInvInOutIDs"></param>
        /// <param name="isBaseLanguageUmos"></param>
        /// <returns></returns>
        public List<GetShipmentDatasProperties> GetShipmentDatas(Ctx ctx, string MVAMInvInOutIDs, string isBaseLanguageUmos)
        {
            List<GetShipmentDatasProperties> obj = new List<GetShipmentDatasProperties>();

            string sql = "SELECT "
                + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QUANTITY,"	//	2
                + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QTYENTER,"	//	2
                + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," // 3..4
                + " l.VAM_Product_ID,p.Name, l.VAM_Inv_InOutLine_ID,l.Line," // 5..8
                + " l.VAB_OrderLine_ID ";

            if (isBaseLanguageUmos != "")
            {
                sql += " " + isBaseLanguageUmos + " ";
            }
            sql += " INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID) " +
                   "  LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) WHERE l.VAM_Inv_InOut_ID=" + MVAMInvInOutIDs +
                   "  GROUP BY l.MovementQty, l.QtyEntered," + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," +
                   "  l.VAM_Product_ID,p.Name, l.VAM_Inv_InOutLine_ID,l.Line,l.VAB_OrderLine_ID ORDER BY l.Line";


            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GetShipmentDatasProperties objc = new GetShipmentDatasProperties();
                    //  objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOut_ID"]);
                    // objc.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["dis"]);
                    obj.Add(objc);
                }
            }


            return obj;
        }

        /// <summary>
        /// Get Invoices
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="displays">Display Columns</param>
        /// <param name="cBPartnerId">Business Partner</param>
        /// <param name="isReturnTrxs">Return Transaction</param>
        /// <param name="IsDrop">Drop Shipemnt</param>
        /// <returns>List<VCreateFromGetCOrder>, List of Invoices</returns>
        public List<VCreateFromGetCOrder> GetInvoicesVCreate(Ctx ctx, string displays, int cBPartnerId, bool isReturnTrxs, bool IsDrop)
        {
            List<VCreateFromGetCOrder> obj = new List<VCreateFromGetCOrder>();
            MVAFClient tenant = MVAFClient.Get(ctx);

            StringBuilder sql = new StringBuilder("SELECT i.VAB_Invoice_ID," + displays + " AS displays FROM VAB_Invoice i INNER JOIN VAB_DocTypes d ON (i.VAB_DocTypes_ID = d.VAB_DocTypes_ID) "
                        // New column added to fill invoice which drop ship is true
                        + "WHERE i.VAB_BusinessPartner_ID=" + cBPartnerId + " AND i.IsSOTrx='N' AND i.IsDropShip='" + (IsDrop ? "Y" : "N") + "' "
                        + "AND d.IsReturnTrx='" + (isReturnTrxs ? "Y" : "N") + "' AND i.DocStatus IN ('CL','CO') "
                        //Invoice vendor record created with Document Type having checkbox 'Treat As Discount' is true will not show on 'Create line From' on window : Return to Vendor.
                        + " AND i.TreatAsDiscount = 'N' "
                        + " AND i.VAB_Invoice_ID IN "
                     + "(SELECT VAB_Invoice_ID FROM (SELECT il.VAB_Invoice_ID,il.VAB_InvoiceLine_ID,il.QtyInvoiced,mi.Qty FROM VAB_InvoiceLine il "
                     + " LEFT OUTER JOIN VAM_MatchInvoice mi ON (il.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) ");

            // Get Invoices based on the setting taken on Tenant to allow non item Product
            if (tenant.Get_ColumnIndex("IsAllowNonItem") > 0 && !tenant.IsAllowNonItem())
            {
                sql.Append(" INNER JOIN VAM_Product Mp On Mp.VAM_Product_ID = il.VAM_Product_ID WHERE Mp.ProductType = 'I' AND");
            }
            else
            {
                sql.Append(" LEFT JOIN VAM_Product Mp On Mp.VAM_Product_ID = il.VAM_Product_ID WHERE");
            }

            sql.Append(" (il.QtyInvoiced <> nvl(mi.Qty,0) AND mi.VAB_InvoiceLine_ID IS NOT NULL And Mp.Iscostadjustmentonlost = 'N') "
                     + " OR (NVL(Mi.Qty,0) = 0 AND Mi.VAB_InvoiceLine_Id IS NOT NULL AND Mp.Iscostadjustmentonlost = 'Y') "
                     + " OR mi.VAB_InvoiceLine_ID IS NULL ) t GROUP BY VAB_Invoice_ID,VAB_InvoiceLine_ID,QtyInvoiced "
                     + " HAVING QtyInvoiced > SUM(nvl(Qty,0))) ORDER BY i.DateInvoiced, i.DocumentNo");

            DataSet ds = DB.ExecuteDataset(sql.ToString());

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Invoice_ID"]);
                    objc.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["displays"]);
                    obj.Add(objc);
                }
            }
            return obj;
        }

        /// <summary>
        /// get data for VAM_Inv_InOut_Candidate_v
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="adClientID"></param>
        /// <param name="mWarehouseIDs"></param>
        /// <param name="cBPartnerIDs"></param>
        /// <param name="cOrderIDSearchs"></param>
        /// <returns></returns>
        public List<ExecuteQueryVinoutgen> ExecuteQueryVinoutgen(Ctx ctx, int adClientID, string mWarehouseIDs, string cBPartnerIDs, string cOrderIDSearchs)
        {
            List<ExecuteQueryVinoutgen> obj = new List<ExecuteQueryVinoutgen>();

            string sql =
                 "SELECT VAB_Order_ID, o.Name as ord, dt.Name as docType, ic.DocumentNo, bp.Name as bpName, ic.DateOrdered, ic.TotalLines "
                 + "FROM VAM_Inv_InOut_Candidate_v ic, VAF_Org o, VAB_BusinessPartner bp, VAB_DocTypes dt "
                 + "WHERE ic.VAF_Org_ID=o.VAF_Org_ID"
                 + " AND ic.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                 + " AND ic.VAB_DocTypes_ID=dt.VAB_DocTypes_ID"
                 + " AND ic.VAF_Client_ID=" + adClientID;

            if (mWarehouseIDs != "")
            {
                sql += " " + mWarehouseIDs + " ";
            }
            if (cBPartnerIDs != "")
            {
                sql += " " + cBPartnerIDs + " ";
            }
            if (cOrderIDSearchs != "")
            {
                sql += " " + cOrderIDSearchs + " ";
            }
            sql += " ORDER BY o.Name,bp.Name,ic.DateOrdered";

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ExecuteQueryVinoutgen objc = new ExecuteQueryVinoutgen();
                    objc.VAB_Order_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Order_id"]);
                    objc.ord = Util.GetValueOfString(ds.Tables[0].Rows[i]["ord"]);
                    objc.doctype = Util.GetValueOfString(ds.Tables[0].Rows[i]["doctype"]);
                    objc.documentno = Util.GetValueOfString(ds.Tables[0].Rows[i]["documentno"]);
                    objc.bpname = Util.GetValueOfString(ds.Tables[0].Rows[i]["bpname"]);
                    objc.dateordered = Convert.ToDateTime(ds.Tables[0].Rows[i]["dateordered"]);
                    obj.Add(objc);
                }
            }


            return obj;
        }

        /// <summary>
        /// get data from VAB_Order
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="adClientID"></param>
        /// <param name="adOrgIDs"></param>
        /// <param name="cBPartnerIDs"></param>
        /// <param name="ordShipmentids"></param>
        /// <returns></returns>
        public List<ExecuteQueryVinoutgen> ExecuteQueryVInvoiceGen(Ctx ctx, int adClientID, string adOrgIDs, string cBPartnerIDs, string ordShipmentids)
        {
            List<ExecuteQueryVinoutgen> obj = new List<ExecuteQueryVinoutgen>();

            var sql = "SELECT VAB_Order_ID, o.Name as ord, dt.Name as docType, ic.DocumentNo, bp.Name as bpName, ic.DateOrdered, ic.TotalLines "
                      + "FROM VAB_Invoice_Candidate_v ic, VAF_Org o, VAB_BusinessPartner bp, VAB_DocTypes dt "
                      + "WHERE ic.VAF_Org_ID=o.VAF_Org_ID"
                      + " AND ic.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                      + " AND ic.VAB_DocTypes_ID=dt.VAB_DocTypes_ID"
                      + " AND ic.VAF_Client_ID=" + adClientID;

            if (adOrgIDs != "")
            {
                sql += " " + adOrgIDs + " ";

            }
            if (cBPartnerIDs != "")
            {
                sql += " " + cBPartnerIDs + " ";
            }
            if (ordShipmentids != "")
            {
                sql += " " + ordShipmentids;
            }
            sql += " ORDER BY o.Name,bp.Name,ic.DateOrdered";


            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ExecuteQueryVinoutgen objc = new ExecuteQueryVinoutgen();
                    objc.VAB_Order_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Order_ID"]);
                    objc.ord = Util.GetValueOfString(ds.Tables[0].Rows[i]["ord"]);
                    objc.doctype = Util.GetValueOfString(ds.Tables[0].Rows[i]["doctype"]);
                    objc.documentno = Util.GetValueOfString(ds.Tables[0].Rows[i]["documentno"]);
                    objc.bpname = Util.GetValueOfString(ds.Tables[0].Rows[i]["bpname"]);
                    objc.dateordered = Convert.ToDateTime(ds.Tables[0].Rows[i]["DateOrdered"].ToString());
                    objc.tLines = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["TotalLines"]);
                    obj.Add(objc);
                }
            }


            return obj;
        }

        /// <summary>
        /// Get match data for Order, Invoice and Ship/Receipt on Matching PO - invoice form.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="displayMATCH_INVOICEs"></param>
        /// <param name="chkIsReturnTrxProps"></param>
        /// <param name="displayMATCH_ORDERs"></param>
        /// <param name="matchToTypeMATCH_SHIPMENTs"></param>
        /// <param name="matchedsss"></param>
        /// <param name="chkSameBPartnerss"></param>
        /// <param name="chkSameProductss"></param>
        /// <param name="chkSameQtyss"></param>
        /// <param name="from_ss"></param>
        /// <param name="fromIfs"></param>
        /// <param name="to_Dats"></param>
        /// <param name="matchToTypes"></param>
        /// <param name="MATCH_SHIPMENTs"></param>
        /// <param name="MATCH_ORDERs"></param>
        /// <param name="onlyProduct_ss"></param>
        /// <param name="onlyVendor_ss"></param>
        /// <param name="MatchToID"></param>
        /// <returns>List of Property Class "GetTableLoadVmatch"</returns>
        public List<GetTableLoadVmatch> GetTableLoadVmatch(Ctx ctx, string displayMATCH_INVOICEs, bool chkIsReturnTrxProps, string displayMATCH_ORDERs,
                                                             string matchToTypeMATCH_SHIPMENTs, bool matchedsss, string chkSameBPartnerss,
                                                             string chkSameProductss, string chkSameQtyss, string from_ss, string fromIfs, string to_Dats, string matchToTypes,
                                                             string MATCH_SHIPMENTs, string MATCH_ORDERs, string onlyProduct_ss, string onlyVendor_ss, string MatchToID)
        {
            List<GetTableLoadVmatch> objj = new List<GetTableLoadVmatch>();

            StringBuilder _sql = new StringBuilder();
            string sql = "";
            var _groupBy = "";
            var matched = matchedsss;
            DataSet ds = null;

            if (displayMATCH_INVOICEs != "")
            {
                _sql.Append(@"SELECT hdr.VAB_Invoice_ID AS IDD,hdr.DocumentNo AS DocNum, hdr.DateInvoiced AS Dates, bp.Name AS BPNames,hdr.VAB_BusinessPartner_ID AS BPartner_ID,
                        lin.Line AS Lines,lin.VAB_InvoiceLine_ID AS lineK, p.Name as Product,lin.VAM_Product_ID AS productk, lin.QtyInvoiced AS qty,SUM(NVL(mi.Qty,0)) AS MATCH
                        FROM VAB_Invoice hdr INNER JOIN VAB_BusinessPartner bp ON (hdr.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) INNER JOIN VAB_InvoiceLine lin ON (hdr.VAB_Invoice_ID=lin.VAB_Invoice_ID)
                        INNER JOIN VAM_Product p ON (lin.VAM_Product_ID=p.VAM_Product_ID) INNER JOIN VAB_DocTypes dt ON (hdr.VAB_DocTypes_ID=dt.VAB_DocTypes_ID and dt.DocBaseType in ('API','APC') 
                        AND dt.IsReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + @" FULL JOIN VAM_MatchInvoice mi ON (lin.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) 
                        WHERE hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? " AND lin.VAM_Inv_InOutLine_ID = " + MatchToID : ""));

                _groupBy = " GROUP BY hdr.VAB_Invoice_ID,hdr.DocumentNo,hdr.DateInvoiced,bp.Name,hdr.VAB_BusinessPartner_ID,"
                    + " lin.Line,lin.VAB_InvoiceLine_ID,p.Name,lin.VAM_Product_ID,lin.QtyInvoiced "
                    + "HAVING "
                    + (matched ? "0" : "lin.QtyInvoiced")
                    + "<>SUM(NVL(mi.Qty,0)) ORDER BY hdr.DocumentNo";
            }
            else if (displayMATCH_ORDERs != "")
            {
                _sql.Append(@"SELECT hdr.VAB_Order_ID AS IDD,hdr.DocumentNo AS Docnum, hdr.DateOrdered AS Dates, bp.Name AS BPNames,hdr.VAB_BusinessPartner_ID AS Bpartner_Id,
                        lin.Line AS Lines,lin.VAB_OrderLine_ID AS Linek, p.Name as Product,lin.VAM_Product_ID AS Productk,lin.QtyOrdered AS qty,SUM(COALESCE(mo.Qty,0)) AS MATCH
                        FROM VAB_Order hdr INNER JOIN VAB_BusinessPartner bp ON (hdr.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) INNER JOIN VAB_OrderLine lin ON (hdr.VAB_Order_ID=lin.VAB_Order_ID)
                        INNER JOIN VAM_Product p ON (lin.VAM_Product_ID=p.VAM_Product_ID) INNER JOIN VAB_DocTypes dt ON (hdr.VAB_DocTypes_ID=dt.VAB_DocTypes_ID AND dt.DocBaseType='POO'
                        AND dt.isReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + @" FULL JOIN VAM_MatchPO mo ON (lin.VAB_OrderLine_ID=mo.VAB_OrderLine_ID)
                        WHERE  hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? " AND mo.VAM_Inv_InOutLine_ID = " + MatchToID : ""));

                //Conneted this condition because of partialy received qty from MR In case of Purcahse Order : Done by Manjot issue assigned by Puneet and Mukesh Sir
                //mo."
                //    + (matchToTypes == MATCH_SHIPMENTs ? "VAM_Inv_InOutLine_ID" : "VAB_InvoiceLine_ID")
                //    + (matched ? " IS NOT NULL" : " IS NULL")
                //    + " AND

                _groupBy = " GROUP BY hdr.VAB_Order_ID,hdr.DocumentNo,hdr.DateOrdered,bp.Name,hdr.VAB_BusinessPartner_ID,"
                    + " lin.Line,lin.VAB_OrderLine_ID,p.Name,lin.VAM_Product_ID,lin.QtyOrdered "
                    + "HAVING "
                    + (matched ? "0" : "lin.QtyOrdered")
                    + "<>SUM(COALESCE(mo.Qty,0)) ORDER BY hdr.DocumentNo";
            }
            else    //  Shipment
            {
                _sql.Append(@"SELECT hdr.VAM_Inv_InOut_ID AS IDD,hdr.DocumentNo AS Docnum, hdr.MovementDate AS Dates, bp.Name AS BPNames,hdr.VAB_BusinessPartner_ID AS Bpartner_Id,
                        lin.Line AS Lines,lin.VAM_Inv_InOutLine_ID AS Linek, p.Name as Product,lin.VAM_Product_ID AS Productk, lin.MovementQty AS qty,SUM(NVL(m.Qty,0)) AS MATCH
                        FROM VAM_Inv_InOut hdr INNER JOIN VAB_BusinessPartner bp ON (hdr.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) INNER JOIN VAM_Inv_InOutLine lin ON (hdr.VAM_Inv_InOut_ID=lin.VAM_Inv_InOut_ID)
                        INNER JOIN VAM_Product p ON (lin.VAM_Product_ID=p.VAM_Product_ID) INNER JOIN VAB_DocTypes dt ON (hdr.VAB_DocTypes_ID = dt.VAB_DocTypes_ID AND dt.DocBaseType='MMR'
                        AND dt.isReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + " FULL JOIN " + (matchToTypes == MATCH_ORDERs ? "VAM_MatchPO" : "VAM_MatchInvoice")
                    + @" m ON (lin.VAM_Inv_InOutLine_ID=m.VAM_Inv_InOutLine_ID) WHERE hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? (matchToTypes == MATCH_ORDERs ? " AND m.VAB_OrderLine_ID = "
                    + MatchToID : " AND m.VAB_InvoiceLine_ID = " + MatchToID) : ""));

                _groupBy = " GROUP BY hdr.VAM_Inv_InOut_ID,hdr.DocumentNo,hdr.MovementDate,bp.Name,hdr.VAB_BusinessPartner_ID,"
                    + " lin.Line,lin.VAM_Inv_InOutLine_ID,p.Name,lin.VAM_Product_ID,lin.MovementQty "
                    + "HAVING "
                    + (matched ? "0" : "lin.MovementQty")
                    + "<>SUM(NVL(m.Qty,0)) ORDER BY hdr.DocumentNo";
            }


            if (onlyProduct_ss != "")
            {
                _sql.Append(onlyProduct_ss);
            }

            if (onlyVendor_ss != "")
            {
                _sql.Append(onlyVendor_ss);
            }
            if (from_ss != "")
            {
                _sql.Append(from_ss);
            }
            else if (fromIfs != "")
            {
                _sql.Append(fromIfs);
            }
            else if (to_Dats != "")
            {
                _sql.Append(to_Dats);
            }

            if (chkSameBPartnerss != "")
            {
                _sql.Append(chkSameBPartnerss);
            }

            if (chkSameProductss != "")
            {
                _sql.Append(chkSameProductss);
            }

            if (chkSameQtyss != "")
            {
                _sql.Append(chkSameQtyss);
            }

            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(_sql.ToString(), "hdr", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO) + _groupBy;
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GetTableLoadVmatch obj = new GetTableLoadVmatch();
                    obj._ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["IDD"]);
                    obj.DocNo = Util.GetValueOfString(ds.Tables[0].Rows[i]["Docnum"]);
                    obj.Date = Convert.ToDateTime(ds.Tables[0].Rows[i]["Dates"]);
                    obj.CBPartnerID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Bpartner_Id"]);
                    obj.CBPartnerIDK = Util.GetValueOfString(ds.Tables[0].Rows[i]["BPNames"]);
                    obj.Line = Util.GetValueOfString(ds.Tables[0].Rows[i]["Lines"]);
                    obj.Line_K = Util.GetValueOfString(ds.Tables[0].Rows[i]["Linek"]);
                    obj.MVAMProductID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Productk"]);
                    obj.MVAMProductIDK = Util.GetValueOfString(ds.Tables[0].Rows[i]["Product"]);
                    obj.Qty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["qty"]);
                    obj.Matched = Util.GetValueOfString(ds.Tables[0].Rows[i]["MATCH"]);
                    objj.Add(obj);
                }
            }
            return objj;
        }




        /// <summary>
        /// used to load product container
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public List<MoveKeyVal> GetContainer(Ctx ctx, int locator)
        {
            List<MoveKeyVal> keyVal = new List<MoveKeyVal>();
            string sql = "SELECT VAM_ProductContainer_ID,Value || '_' || Name AS Value FROM VAM_ProductContainer WHERE IsActive = 'Y' ";
            if (locator > 0)
            {
                sql += "  AND VAM_Locator_ID = " + locator;
            }
            sql += " ORDER BY Value";
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAM_ProductContainer", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO); // fully qualidfied - RO
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new MoveKeyVal()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAM_ProductContainer_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["Value"])
                    });
                }
                ds.Dispose();
            }
            return keyVal;
        }

    }






    public class VCreateFromGetCOrder
    {
        public int key { get; set; }
        public string value { get; set; }
    }

    public class GetShipmentDatasProperties
    {
        public int Quantity { get; set; }
        public int QuantityEntered { get; set; }
        public int VAB_UOM_ID { get; set; }
        public int VAM_Product_ID { get; set; }
        public int VAB_Order_ID { get; set; }
        public int VAM_Inv_InOut_ID { get; set; }
        public int VAB_Invoice_ID { get; set; }
        public int VAB_UOM_ID_K { get; set; }
        public int VAM_Product_ID_K { get; set; }
        public int VAB_Order_ID_K { get; set; }
        public int VAM_Inv_InOut_ID_K { get; set; }
        public int VAB_Invoice_ID_K { get; set; }
    }

    public class ExecuteQueryVinoutgen
    {
        public int VAB_Order_id { get; set; }
        public string ord { get; set; }
        public string doctype { get; set; }
        public string documentno { get; set; }
        public string bpname { get; set; }
        public DateTime dateordered { get; set; }
        public decimal tLines { get; set; }
    }

    public class GetTableLoadVmatch
    {
        public int _ID { get; set; }
        public string DocNo { get; set; }
        public DateTime Date { get; set; }
        public int CBPartnerID { get; set; }
        public string CBPartnerIDK { get; set; }
        public string Line { get; set; }
        public string Line_K { get; set; }
        public int MVAMProductID { get; set; }
        public string MVAMProductIDK { get; set; }
        public decimal Qty { get; set; }
        public string Matched { get; set; }
    }


}