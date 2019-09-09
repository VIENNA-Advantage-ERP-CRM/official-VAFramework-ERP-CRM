﻿using System;
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
        /// <param name="C_BPartner_ID">Business Partner</param>
        /// <param name="isReturnTrx">Return Transaction</param>       
        /// <param name="OrgId">Organization</param>
        /// <param name="DropShip">Drop Shipment</param>
        /// <param name="IsSOTrx">Sales Transaction</param>
        /// <param name="forInvoices">For Invoice</param>
        /// <returns>List<VCreateFromGetCOrder>, List of Orders</returns>

        public List<VCreateFromGetCOrder> VCreateGetOrders(Ctx ctx, string display, string column, int C_BPartner_ID, bool isReturnTrx, int OrgId, bool DropShip, bool IsSOTrx, bool forInvoices)
        {
            List<VCreateFromGetCOrder> obj = new List<VCreateFromGetCOrder>();
            var dis = display;

            StringBuilder sql = new StringBuilder("SELECT o.C_Order_ID," + display + " AS displays FROM C_Order o WHERE o.C_BPartner_ID=" + C_BPartner_ID + " AND o.IsSOTrx ='" + (IsSOTrx ? "Y" : "N")
                + "' AND O.IsBlanketTrx = 'N' AND o.DocStatus IN ('CL','CO') ");

            if (OrgId > 0)
            {
                sql.Append("AND o.AD_Org_ID = " + OrgId);
            }

            // when create lines fom open from M_Inout then pick records from match po having m_inoutline is not null 
            // when create lines fom open from M_Invoiceline then pick records from match po having m_invoiceline is not null 
            sql.Append("AND o.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' AND o.IsDropShip='" + (DropShip ? "Y" : "N") + "'  AND o.C_Order_ID IN "
            + @"(SELECT C_Order_ID FROM (SELECT ol.C_Order_ID,ol.C_OrderLine_ID,ol.QtyOrdered, 
            (SELECT SUM(m.qty) FROM m_matchPO m WHERE ol.C_OrderLine_ID=m.C_OrderLine_ID AND NVL(" + column + @", 0) != 0 AND m.ISACTIVE = 'Y' ) AS Qty,
            (SELECT SUM(IL.QtyInvoiced)  FROM C_INVOICELINE IL INNER JOIN C_Invoice I ON I.C_INVOICE_ID = IL.C_INVOICE_ID
            WHERE il.ISACTIVE = 'Y' AND I.DOCSTATUS NOT IN ('VO','RE') AND OL.C_ORDERLINE_ID  =IL.C_ORDERLINE_ID) AS QtyInvoiced FROM C_OrderLine ol ");

            if (!forInvoices)
            {
                sql.Append("INNER JOIN M_Product p ON ol.M_Product_ID = p.M_Product_ID AND p.ProductType = 'I'");
            }

            sql.Append(") GROUP BY C_Order_ID,C_OrderLine_ID,QtyOrdered "
            + "HAVING QtyOrdered > SUM(nvl(Qty,0)) AND QtyOrdered > SUM(NVL(QtyInvoiced,0))) ORDER BY o.DateOrdered, o.DocumentNo");

            DataSet ds = DB.ExecuteDataset(sql.ToString());

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]);
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
            string sql = "SELECT s.M_InOut_ID," + displays + " AS dis FROM M_InOut s "
                + "WHERE s.C_BPartner_ID=" + CBPartnerIDs + " AND s.IsSOTrx='" + (IsSOTrx ? "Y" : "N") + "' AND s.DocStatus IN ('CL','CO')"
                // New column added to fill invoice which drop ship is true
               + " AND s.IsDropShip='" + (IsDrop ? "Y" : "N") + "' AND s.M_InOut_ID IN "

               // Changes done by Bharat on 06 July 2017 restrict to create invoice if Invoice already created against that for same quantity

               + "(SELECT M_InOut_ID FROM (SELECT sl.M_InOut_ID, sl.M_InOutLine_ID, sl.MovementQty, mi.QtyInvoiced FROM M_InOutLine sl "
               + "LEFT OUTER JOIN (SELECT il.QtyInvoiced, il.M_InOutLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice I ON I.C_INVOICE_ID = il.C_INVOICE_ID "
               + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON sl.M_InOutLine_ID=mi.M_InOutLine_ID) "
               + "GROUP BY M_InOut_ID, M_InOutLine_ID, MovementQty HAVING MovementQty > SUM(NVL(QtyInvoiced,0))) ORDER BY s.MovementDate, s.DocumentNo";

            //+ "(SELECT M_InOut_ID FROM (SELECT sl.M_InOut_ID,sl.M_InOutLine_ID,sl.MovementQty,mi.Qty,IL.QtyInvoiced FROM M_InOutLine sl "
            //+ "LEFT OUTER JOIN M_MatchInv mi ON (sl.M_InOutLine_ID=mi.M_InOutLine_ID) "
            //+ " LEFT OUTER JOIN C_INVOICELINE IL    ON (sl.C_ORDERLINE_ID =IL.C_ORDERLINE_ID)"
            //+ " LEFT OUTER JOIN C_Invoice I   ON I.C_INVOICE_ID      =IL.C_INVOICE_ID "
            //+ " AND I.DOCSTATUS NOT   IN ('VO','RE') "
            //+ " WHERE (sl.MovementQty <> nvl(mi.Qty,0) OR SL.MovementQty     <> NVL(IL.QtyInvoiced,0)"
            //+ "AND mi.M_InOutLine_ID IS NOT NULL) OR mi.M_InOutLine_ID IS NULL ) GROUP BY M_InOut_ID,M_InOutLine_ID,MovementQty "
            //+ "HAVING MovementQty > SUM(nvl(Qty,0)) OR MovementQty    > SUM(NVL(QtyInvoiced,0)) ) ORDER BY s.MovementDate";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InOut_ID"]);
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
        /// <param name="MInOutIDs"></param>
        /// <param name="isBaseLanguageUmos"></param>
        /// <returns></returns>
        public List<GetShipmentDatasProperties> GetShipmentDatas(Ctx ctx, string MInOutIDs, string isBaseLanguageUmos)
        {
            List<GetShipmentDatasProperties> obj = new List<GetShipmentDatasProperties>();

            string sql = "SELECT "
                + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QUANTITY,"	//	2
                + "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END ),2) as QTYENTER,"	//	2
                + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," // 3..4
                + " l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line," // 5..8
                + " l.C_OrderLine_ID ";

            if (isBaseLanguageUmos != "")
            {
                sql += " " + isBaseLanguageUmos + " ";
            }
            sql += " INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) " +
                   "  LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) WHERE l.M_InOut_ID=" + MInOutIDs +
                   "  GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," +
                   "  l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID ORDER BY l.Line";


            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GetShipmentDatasProperties objc = new GetShipmentDatasProperties();
                    //  objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InOut_ID"]);
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

            string sql = "SELECT i.C_Invoice_ID," + displays + " AS displays FROM C_Invoice i INNER JOIN C_DocType d ON (i.C_DocType_ID = d.C_DocType_ID) "
                // New column added to fill invoice which drop ship is true
                        + "WHERE i.C_BPartner_ID=" + cBPartnerId + " AND i.IsSOTrx='N' AND i.IsDropShip='" + (IsDrop ? "Y" : "N") + "' "
                        + "AND d.IsReturnTrx='" + (isReturnTrxs ? "Y" : "N") + "' AND i.DocStatus IN ('CL','CO') "
                //Invoice vendor record created with Document Type having checkbox 'Treat As Discount' is true will not show on 'Create line From' on window : Return to Vendor.
                        + " AND i.TreatAsDiscount = 'N' "
                        + " AND i.C_Invoice_ID IN "
                     + "(SELECT C_Invoice_ID FROM (SELECT il.C_Invoice_ID,il.C_InvoiceLine_ID,il.QtyInvoiced,mi.Qty FROM C_InvoiceLine il "
                     + " LEFT OUTER JOIN M_MatchInv mi ON (il.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
                     + " INNER JOIN M_Product Mp On Mp.M_Product_Id = Il.M_Product_Id  "
                     + " WHERE Mp.ProductType = 'I' AND (il.QtyInvoiced <> nvl(mi.Qty,0) AND mi.C_InvoiceLine_ID IS NOT NULL And Mp.Iscostadjustmentonlost = 'N') "
                     + " OR (NVL(Mi.Qty,0) = 0 AND Mi.C_Invoiceline_Id IS NOT NULL AND Mp.Iscostadjustmentonlost = 'Y') "
                     + " OR mi.C_InvoiceLine_ID IS NULL ) GROUP BY C_Invoice_ID,C_InvoiceLine_ID,QtyInvoiced "
                     + " HAVING QtyInvoiced > SUM(nvl(Qty,0))) ORDER BY i.DateInvoiced, i.DocumentNo";

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    VCreateFromGetCOrder objc = new VCreateFromGetCOrder();
                    objc.key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Invoice_ID"]);
                    objc.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["displays"]);
                    obj.Add(objc);
                }
            }
            return obj;
        }

        /// <summary>
        /// get data for M_InOut_Candidate_v
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
                 "SELECT C_Order_ID, o.Name as ord, dt.Name as docType, ic.DocumentNo, bp.Name as bpName, ic.DateOrdered, ic.TotalLines "
                 + "FROM M_InOut_Candidate_v ic, AD_Org o, C_BPartner bp, C_DocType dt "
                 + "WHERE ic.AD_Org_ID=o.AD_Org_ID"
                 + " AND ic.C_BPartner_ID=bp.C_BPartner_ID"
                 + " AND ic.C_DocType_ID=dt.C_DocType_ID"
                 + " AND ic.AD_Client_ID=" + adClientID;

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
                    objc.c_order_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_order_id"]);
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
        /// get data from c_order
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

            var sql = "SELECT C_Order_ID, o.Name as ord, dt.Name as docType, ic.DocumentNo, bp.Name as bpName, ic.DateOrdered, ic.TotalLines "
                      + "FROM C_Invoice_Candidate_v ic, AD_Org o, C_BPartner bp, C_DocType dt "
                      + "WHERE ic.AD_Org_ID=o.AD_Org_ID"
                      + " AND ic.C_BPartner_ID=bp.C_BPartner_ID"
                      + " AND ic.C_DocType_ID=dt.C_DocType_ID"
                      + " AND ic.AD_Client_ID=" + adClientID;

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
                    objc.c_order_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]);
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
                _sql.Append(@"SELECT hdr.C_Invoice_ID AS IDD,hdr.DocumentNo AS DocNum, hdr.DateInvoiced AS Dates, bp.Name AS BPNames,hdr.C_BPartner_ID AS BPartner_ID,
                        lin.Line AS Lines,lin.C_InvoiceLine_ID AS lineK, p.Name as Product,lin.M_Product_ID AS productk, lin.QtyInvoiced AS qty,SUM(NVL(mi.Qty,0)) AS MATCH
                        FROM C_Invoice hdr INNER JOIN C_BPartner bp ON (hdr.C_BPartner_ID=bp.C_BPartner_ID) INNER JOIN C_InvoiceLine lin ON (hdr.C_Invoice_ID=lin.C_Invoice_ID)
                        INNER JOIN M_Product p ON (lin.M_Product_ID=p.M_Product_ID) INNER JOIN C_DocType dt ON (hdr.C_DocType_ID=dt.C_DocType_ID and dt.DocBaseType in ('API','APC') 
                        AND dt.IsReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + @" FULL JOIN M_MatchInv mi ON (lin.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) 
                        WHERE hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? " AND lin.M_InOutLine_ID = " + MatchToID : ""));

                _groupBy = " GROUP BY hdr.C_Invoice_ID,hdr.DocumentNo,hdr.DateInvoiced,bp.Name,hdr.C_BPartner_ID,"
                    + " lin.Line,lin.C_InvoiceLine_ID,p.Name,lin.M_Product_ID,lin.QtyInvoiced "
                    + "HAVING "
                    + (matched ? "0" : "lin.QtyInvoiced")
                    + "<>SUM(NVL(mi.Qty,0)) ORDER BY hdr.DocumentNo";
            }
            else if (displayMATCH_ORDERs != "")
            {
                _sql.Append(@"SELECT hdr.C_Order_ID AS IDD,hdr.DocumentNo AS Docnum, hdr.DateOrdered AS Dates, bp.Name AS BPNames,hdr.C_BPartner_ID AS Bpartner_Id,
                        lin.Line AS Lines,lin.C_OrderLine_ID AS Linek, p.Name as Product,lin.M_Product_ID AS Productk,lin.QtyOrdered AS qty,SUM(COALESCE(mo.Qty,0)) AS MATCH
                        FROM C_Order hdr INNER JOIN C_BPartner bp ON (hdr.C_BPartner_ID=bp.C_BPartner_ID) INNER JOIN C_OrderLine lin ON (hdr.C_Order_ID=lin.C_Order_ID)
                        INNER JOIN M_Product p ON (lin.M_Product_ID=p.M_Product_ID) INNER JOIN C_DocType dt ON (hdr.C_DocType_ID=dt.C_DocType_ID AND dt.DocBaseType='POO'
                        AND dt.isReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + @" FULL JOIN M_MatchPO mo ON (lin.C_OrderLine_ID=mo.C_OrderLine_ID)
                        WHERE  hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? " AND mo.M_InOutLine_ID = " + MatchToID : ""));

                //Conneted this condition because of partialy received qty from MR In case of Purcahse Order : Done by Manjot issue assigned by Puneet and Mukesh Sir
                //mo."
                //    + (matchToTypes == MATCH_SHIPMENTs ? "M_InOutLine_ID" : "C_InvoiceLine_ID")
                //    + (matched ? " IS NOT NULL" : " IS NULL")
                //    + " AND

                _groupBy = " GROUP BY hdr.C_Order_ID,hdr.DocumentNo,hdr.DateOrdered,bp.Name,hdr.C_BPartner_ID,"
                    + " lin.Line,lin.C_OrderLine_ID,p.Name,lin.M_Product_ID,lin.QtyOrdered "
                    + "HAVING "
                    + (matched ? "0" : "lin.QtyOrdered")
                    + "<>SUM(COALESCE(mo.Qty,0)) ORDER BY hdr.DocumentNo";
            }
            else    //  Shipment
            {
                _sql.Append(@"SELECT hdr.M_InOut_ID AS IDD,hdr.DocumentNo AS Docnum, hdr.MovementDate AS Dates, bp.Name AS BPNames,hdr.C_BPartner_ID AS Bpartner_Id,
                        lin.Line AS Lines,lin.M_InOutLine_ID AS Linek, p.Name as Product,lin.M_Product_ID AS Productk, lin.MovementQty AS qty,SUM(NVL(m.Qty,0)) AS MATCH
                        FROM M_InOut hdr INNER JOIN C_BPartner bp ON (hdr.C_BPartner_ID=bp.C_BPartner_ID) INNER JOIN M_InOutLine lin ON (hdr.M_InOut_ID=lin.M_InOut_ID)
                        INNER JOIN M_Product p ON (lin.M_Product_ID=p.M_Product_ID) INNER JOIN C_DocType dt ON (hdr.C_DocType_ID = dt.C_DocType_ID AND dt.DocBaseType='MMR'
                        AND dt.isReturnTrx = " + (chkIsReturnTrxProps ? "'Y')" : "'N')") + " FULL JOIN " + (matchToTypes == MATCH_ORDERs ? "M_MatchPO" : "M_MatchInv")
                    + @" m ON (lin.M_InOutLine_ID=m.M_InOutLine_ID) WHERE hdr.DocStatus IN ('CO','CL')" + (matched && MatchToID != "" ? (matchToTypes == MATCH_ORDERs ? " AND m.C_OrderLine_ID = "
                    + MatchToID : " AND m.C_InvoiceLine_ID = " + MatchToID) : ""));

                _groupBy = " GROUP BY hdr.M_InOut_ID,hdr.DocumentNo,hdr.MovementDate,bp.Name,hdr.C_BPartner_ID,"
                    + " lin.Line,lin.M_InOutLine_ID,p.Name,lin.M_Product_ID,lin.MovementQty "
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

            sql = MRole.GetDefault(ctx).AddAccessSQL(_sql.ToString(), "hdr", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + _groupBy;
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
                    obj.MProductID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Productk"]);
                    obj.MProductIDK = Util.GetValueOfString(ds.Tables[0].Rows[i]["Product"]);
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
            string sql = "SELECT M_ProductContainer_ID,Value || '_' || Name AS Value FROM M_ProductContainer WHERE IsActive = 'Y' ";
            if (locator > 0)
            {
                sql += "  AND M_Locator_ID = " + locator;
            }
            sql += " ORDER BY Value";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_ProductContainer", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO); // fully qualidfied - RO
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new MoveKeyVal()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]),
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
        public int C_UOM_ID { get; set; }
        public int M_Product_ID { get; set; }
        public int C_Order_ID { get; set; }
        public int M_InOut_ID { get; set; }
        public int C_Invoice_ID { get; set; }
        public int C_UOM_ID_K { get; set; }
        public int M_Product_ID_K { get; set; }
        public int C_Order_ID_K { get; set; }
        public int M_InOut_ID_K { get; set; }
        public int C_Invoice_ID_K { get; set; }
    }

    public class ExecuteQueryVinoutgen
    {
        public int c_order_id { get; set; }
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
        public int MProductID { get; set; }
        public string MProductIDK { get; set; }
        public decimal Qty { get; set; }
        public string Matched { get; set; }
    }


}