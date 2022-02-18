using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Model;
using VIS.Models;

namespace VIS.Controllers
{
    public class VCreateFromController : Controller
    {
        /// <summary>
        /// get Orders
        /// </summary>
        /// <param name="displays">Display Columns</param>
        /// <param name="columns">Column Name</param>
        /// <param name="C_BPartner_IDs">Business Partner</param>
        /// <param name="isReturnTrxs">Return Transaction</param>
        /// <param name="OrgIds">Organization</param>
        /// <param name="IsDrop">Drop Shipment</param>
        /// <param name="IsSOTrx">Sales Transaction</param>
        /// <param name="forInvoices">For Invoice</param>
        ///  <param name="recordID">C_Invoice_ID</param>
        /// <returns>List of Orders in Json Format</returns>
        public JsonResult VCreateGetOrders(string displays, string columns, int C_BPartner_IDs, bool isReturnTrxs, int OrgIds, bool IsDrop, bool IsSOTrx, bool forInvoices, int recordID)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.VCreateGetOrders(ctx, displays, columns, C_BPartner_IDs, isReturnTrxs, OrgIds, IsDrop, IsSOTrx, forInvoices, recordID);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get c_order_id or c_orderline_id
        /// </summary>
        /// <param name="keyColumnName"></param>
        /// <param name="tableName"></param>
        /// <param name="recordID"></param>
        /// <param name="pageNo"></param>
        /// <param name="forInvoicees"></param>
        /// <param name="C_Ord_IDs"></param>
        /// <param name="isBaseLangess"></param>
        /// <param name="MProductIDss"></param>
        /// <param name="DelivDates"></param>
        /// <param name="adOrgIDSs"></param>
        /// <returns></returns>
        public JsonResult GetOrdersDataCommon(string keyColumnName, string tableName, int recordID, int pageNo, bool forInvoicees, int C_Ord_IDs, string isBaseLangess, string MProductIDss, string DelivDates, int adOrgIDSs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = VcreateFormSqlQry(forInvoicees, C_Ord_IDs, isBaseLangess, MProductIDss, DelivDates, adOrgIDSs);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        private string VcreateFormSqlQry(bool forInvoicees, int C_Ord_IDs, string isBaseLangess, string MProductIDss, string DelivDates, int adOrgIDSs)
        {
            var sql = "SELECT "
                        + "(l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
                        + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ) as QUANTITY,"
                        + "(l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
                        + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ) as QTYENTER,"
                        + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
                        + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,COALESCE(p.Value,c.Value) as PRODUCTSEARCHKEY,"
                        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
                        + " ins.description , "
                        + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "
                        + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                            FROM c_paymentterm LEFT JOIN C_PaySchedule ON (c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) " +
                        // JID_1414 - not to consider or pick In-Active Record
                        "  WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y') AS IsAdvance "
                        + " FROM C_OrderLine l"
                        + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND "
                        + (forInvoicees ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID")
                        + " IS NOT NULL)"
                        + " LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)"
                        + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
                        + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
                        + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)";

            if (isBaseLangess != "")
            {
                sql += " " + isBaseLangess;
            }

            sql += " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
             + " WHERE l.C_Order_ID=" + C_Ord_IDs;

            if (MProductIDss != "")
            {
                sql += " " + MProductIDss;
            }
            if (DelivDates != "")
            {
                sql += " " + DelivDates;
            }
            sql += " AND l.DTD001_Org_ID = " + adOrgIDSs
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                + "l.M_Product_ID,COALESCE(p.Name,c.Name),COALESCE(p.Value,c.Value),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description, l.IsDropShip, o.C_PaymentTerm_ID , t.Name "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") t WHERE QUANTITY > 0";

            return sqlNew;
        }

        /// <summary>
        /// Get Org data and other.
        /// </summary>
        /// <param name="keyColumnName">Primary Key Column of Table</param>
        /// <param name="tableName">Table Name</param>
        /// <param name="recordID">Record ID</param>
        /// <param name="pageNo">Page No</param>
        /// <param name="forInvoicees">true if open from Invoice window</param>
        /// <param name="C_Ord_IDs">Order ID</param>
        /// <param name="isBaseLangess">true if Base Language</param>
        /// <param name="MProductIDss">Order ID</param>
        /// <param name="DelivDates">Delivery Date</param>
        /// <returns>Data in Json Format</returns>
        public JsonResult GetOrdersDataCommonOrg(string keyColumnName, string tableName, int recordID, int pageNo, bool forInvoicees, int? C_Ord_IDs, string isBaseLangess, string MProductIDss, string DelivDates)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = VcreateFormSqlQryOrg(forInvoicees, C_Ord_IDs, isBaseLangess, MProductIDss, DelivDates, keyColumnName.Equals("C_ProvisionalInvoice_ID"));
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// create sql query for GetOrdersDataCommonOrg functions
        /// </summary>
        /// <param name="forInvoicees">true if open from Invoice window</param>
        /// <param name="C_Ord_IDs">Order ID</param>
        /// <param name="isBaseLangess">true if Base Language</param>
        /// <param name="MProductIDss">Product ID</param>
        /// <param name="DelivDates">Delivery Date</param>
        /// <param name="isProvisionalInvoice">Is Provisional Invoice</param>
        /// <returns>String, Query</returns>
        private string VcreateFormSqlQryOrg(bool forInvoicees, int? C_Ord_IDs, string isBaseLangess, string MProductIDss, string DelivDates, bool isProvisionalInvoice)
        {
            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");

            // JID_1720 : when login with other than base language then data not coming in create line form
            string precision = "";
            if (isBaseLangess.ToUpper().Contains("C_UOM_TRL"))
            {
                precision = " uom1.stdprecision ";
            }
            else
            {
                precision = " uom.stdprecision ";
            }

            StringBuilder sql = new StringBuilder("SELECT "
               + "ROUND((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ), " + precision + ") as QUANTITY,"
               + "ROUND((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ), " + precision + ") as QTYENTER,"
               + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
               + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,p.Name as PRODUCT, p.Value as PRODUCTSEARCHKEY,"
               + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
               + " ins.description , "
               + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, " + precision + " AS StdPrecision  , l.IsDropShip AS  IsDropShip"
               + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
               // JID_1414 - not to consider or pick In-Active Record
               + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON (c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' )
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
               + @" , l.PriceEntered"
               + " FROM C_OrderLine l"
               + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
               + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
               + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

            sql.Append((forInvoicees && !isProvisionalInvoice) ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");

            // Get lines from Order based on the setting taken on Tenant to allow non item Product
            if (!isAllownonItem)
            {
                sql.Append(" IS NOT NULL) INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)");
            }
            else
            {
                sql.Append(" IS NOT NULL) LEFT JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)");
            }

            if (isBaseLangess != "")
            {
                sql.Append(isBaseLangess);
            }
            //Hanlded case: order not exist for the selected Business partner and on the change/selection of deliverydate excception's coming  missing expression
            sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_Order_ID=" + (C_Ord_IDs == null ? 0 : C_Ord_IDs) + " AND l.M_Product_ID>0");

            // Get lines from Order based on the setting taken on Tenant to allow non item Product
            if (!forInvoicees && !isAllownonItem)
            {
                sql.Append(" AND p.ProductType='I' ");
            }

            if (MProductIDss != "")
            {
                sql.Append(MProductIDss);
            }
            if (DelivDates != "")
            {
                sql.Append(DelivDates);
            }

            //if (isProvisionalInvoice)
            //{
            //    // when qty delivered against order, that tym we will show order
            //    sql.Append(" AND l.QtyDelivered != 0 ");
            //}

            sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                    + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                        + "l.M_Product_ID,p.Name,p.Value, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description,  " + precision + ",l.IsDropShip, o.C_PaymentTerm_ID , t.Name, l.PriceEntered  "); //Arpit on  20th Sept,2017"	            

            // Show Orderline with Charge also, based on the setting for Non Item type on Tenant.
            if (forInvoicees || isAllownonItem || isProvisionalInvoice)
            {
                sql.Append("UNION SELECT "
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "					//	1               
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END )," + precision + ") as QUANTITY,"	//	2
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END )," + precision + ") as QTYENTER,"	//	added by bharat
                  + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as M_PRODUCT_ID , c.Name as PRODUCT, c.Value as PRODUCTSEARCHKEY,"	//	5..6
                  + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
                  + " ins.description , "
                  + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, " + precision + " AS StdPrecision   , l.IsDropShip AS  IsDropShip"			//	7..8 //              
                  + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                  // JID_1414 - not to consider or pick In-Active Record
                  + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                  + @" , l.PriceEntered "
                  + " FROM C_OrderLine l"
                  + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
                  + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN " + (!isProvisionalInvoice ? "C_INVOICELINE" : "C_ProvisionalInvoiceLine") + "  M ON(L.C_OrderLine_ID=M.C_OrderLine_ID) AND ");

                sql.Append(forInvoicees && !isProvisionalInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
                sql.Append(" IS NOT NULL LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

                if (isBaseLangess != "")
                {
                    sql.Append(isBaseLangess);
                }
                //Hanlded case: order not exist for the selected Business partner and on the change/selection of deliverydate excception's coming  missing expression
                sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_Order_ID=" + (C_Ord_IDs == null ? 0 : C_Ord_IDs) + " AND C.C_Charge_ID >0 ");

                if (DelivDates != "")
                {
                    sql.Append(DelivDates);
                }

                sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                      + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                      + "l.M_Product_ID,c.Name,c.Value,l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description, " + precision + ", l.IsDropShip , o.C_PaymentTerm_ID , t.Name, l.PriceEntered");
            }
            // JID_1287: Line number sequence to be maintained when we create lines from the reference of other documents.
            string sqlNew = "SELECT * FROM (" + sql.ToString() + ") t WHERE QUANTITY > 0 " + (isProvisionalInvoice ? ("AND " + GetProvisionalLine()) : "") + " ORDER BY LINE";

            return sqlNew;
        }

        /// <summary>
        /// This function is used to exclude Orderline 
        /// </summary>
        /// <returns>query</returns>
        public string GetProvisionalLine()
        {
            string sql = @"  NVL(C_Orderline_ID , 0) NOT IN (SELECT C_Orderline_ID FROM
                            (SELECT ol.C_Order_ID, ol.C_Orderline_ID, ol.qtyordered, 
                            (SELECT SUM(il.qtyinvoiced) FROM c_invoiceline il
                                   INNER JOIN c_invoice i ON i.c_invoice_id = il.c_invoice_id
                               WHERE il.isactive = 'Y' AND i.docstatus NOT IN ( 'VO', 'RE' )
                                       AND ol.c_orderline_id = il.c_orderline_id )  AS qtyinvoiced
                            FROM c_orderline ol ) t WHERE nvl(qtyinvoiced, 0) >= qtyordered /*GROUP BY c_order_id, c_orderline_id, qtyordered
                            HAVING SUM(nvl(qtyinvoiced, 0)) != 0*/)";
            return sql;

        }

        /// <summary>
        /// get data for c_orderline with organization
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="C_OrderID"></param>
        /// <param name="orggetVals"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        public JsonResult GetOrderDataCommons(bool forInvoices, string isBaseLangs, int C_OrderID, int orggetVals, string langs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = OrderDataCommonsSqlQry(forInvoices, isBaseLangs, C_OrderID, orggetVals, langs);
            var stValue = obj.GetOrderDataCommons(ctx, sql);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Sql qry fro GetOrderDataCommons function
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="C_OrderID"></param>
        /// <param name="orggetVals"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        private string OrderDataCommonsSqlQry(bool forInvoices, string isBaseLangs, int C_OrderID, int orggetVals, string langs)
        {
            string
            sql = ("SELECT "
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
              + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
              + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
              + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
              + " ins.description , "
                //+ " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW "								//	7..8
                + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "								//	7..8 //Arpit on  20th Sept,2017
              + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
              + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID  AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                + " FROM C_OrderLine l"
               + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
               + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
               + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

            sql += (forInvoices ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
            sql += " IS NOT NULL) LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)";

            if (isBaseLangs != "")
            {
                sql += " " + isBaseLangs;
            }

            sql += " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ";

            sql += " WHERE l.C_Order_ID=" + C_OrderID + " AND l.DTD001_Org_ID = " + orggetVals
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                    + "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description ,l.IsDropShip , o.C_PaymentTerm_ID , t.Name  " //Arpit on  20th Sept,2017
                                                                                                                                                                                    //+ "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description   "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") WHERE QUANTITY > 0";

            return sqlNew;
        }

        /// <summary>
        /// get data for c_orderline without organization
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="C_OrderID"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        public JsonResult GetOrderDataCommonsNotOrg(bool forInvoices, string isBaseLangs, int C_OrderID, string langs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = GetOrderDataCommonsNotOrgSQL(forInvoices, isBaseLangs, C_OrderID, langs);
            var stValue = obj.GetOrderDataCommons(ctx, sql);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry GetOrderDataCommonsNotOrg
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="C_OrderID"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        private string GetOrderDataCommonsNotOrgSQL(bool forInvoices, string isBaseLangs, int C_OrderID, string langs)
        {
            string
           sql = "SELECT "
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"
               + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
               + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"
               + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
               + " ins.description , "
               + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "//Arpit on  20th Sept,2017
                                                                                                                        //+ " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "
               + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
               + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                + " FROM C_OrderLine l"
                + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
               + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
                + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ";

            sql += (forInvoices ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
            sql += " IS NOT NULL) LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)";

            if (isBaseLangs != "")
            {
                sql += " " + isBaseLangs;
            }

            sql += " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ";

            sql += " WHERE l.C_Order_ID=" + C_OrderID			//	#1
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                + "l.M_Product_ID,COALESCE(p.Name,c.Name), l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description ,l.IsDropShip , o.C_PaymentTerm_ID , t.Name " //Arpit on  20th Sept,2017
                                                                                                                                                                               //+ "l.M_Product_ID,COALESCE(p.Name,c.Name), l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") t WHERE QUANTITY > 0";

            return sqlNew;
        }

        public JsonResult GetOrderDataCommonsNotOrg(int M_Product_ID_Ks)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var stValue = obj.GetOrderDataCommonsNotOrg(ctx, M_Product_ID_Ks);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Shipment data (for old signature)
        /// </summary>
        /// <param name="displays"></param>
        /// <param name="CBPartnerIDs">business partner ids</param>
        /// <param name="IsDrop">drop shipment</param>
        /// <param name="IsSOTrx">is trx sales or not</param>
        /// <returns>Shipment data</returns>
        public JsonResult GetShipments(string displays, int CBPartnerIDs, bool IsDrop, bool IsSOTrx)
        {
            return GetShipmentsData(displays, CBPartnerIDs, IsDrop, IsSOTrx);
        }
        /// <summary>
        /// Author:VA230
        /// Get Shipment data
        /// </summary>
        /// <param name="displays">record to disply</param>
        /// <param name="CBPartnerIDs">bp ids</param>
        /// <param name="IsDrop">drop shipment</param>
        /// <param name="IsSOTrx">sales transaction or not</param>
        /// <param name="isReturnTrxs">transaction is returned or not</param>
        /// <param name="isProvisionlInvoices">record selected is Provisionl nvoice or not</param>
        /// <returns>Get shipment data</returns>
        public JsonResult GetShipmentsData(string displays, int CBPartnerIDs, bool IsDrop, bool IsSOTrx, bool isReturnTrxs = false, bool isProvisionlInvoices = false)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var stValue = obj.GetShipments(ctx, displays, CBPartnerIDs, IsDrop, IsSOTrx, isReturnTrxs, isProvisionlInvoices);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get c_order_id or c_orderline_id
        /// </summary>
        /// <param name="keyColumnName"></param>
        /// <param name="tableName"></param>
        /// <param name="recordID"></param>
        /// <param name="pageNo"></param>
        /// <param name="mInOutId"></param>
        /// <param name="isBaseLanguages"></param>
        /// <param name="mProductIDD"></param>
        /// <returns></returns>
        public JsonResult GetDataVCreateFrom(string keyColumnName, string tableName, int recordID, int pageNo, string mInOutId, string isBaseLanguages, string mProductIDD)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var sql = GetDataSqlQueries(mInOutId, isBaseLanguages, mProductIDD, keyColumnName.Equals("C_ProvisionalInvoice_ID"));
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetData function
        /// </summary>
        /// <param name="mInOutId"></param>
        /// <param name="isBaseLanguages"></param>
        /// <param name="mProductIDD"></param>
        /// <param name="isProvisionalInvoice">Is Provisional Invoice</param>
        /// <returns></returns>
        private string GetDataSqlQueries(string mInOutId, string isBaseLanguages, string mProductIDD, bool isProvisionalInvoice)
        {
            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");
            string precision = "3";
            if (isBaseLanguages.ToUpper().Contains("C_UOM_TRL"))
            {
                precision = " uom1.stdprecision ";
            }
            else
            {
                precision = " uom.stdprecision ";
            }
            string sql = "SELECT "
             //+ "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
             // Changes done by Bharat on 07 July 2017 restrict to create invoice if Invoice already created against that for same quantity
             + "ROUND((l.MovementQty-SUM(COALESCE(mi.QtyInvoiced,0))) * "					//	1  
             + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QUANTITY,"    //	2
                                                                                                                            //+ "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
             + "ROUND((l.MovementQty-SUM(COALESCE(mi.QtyInvoiced,0))) * "					//	1  
             + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QTYENTER,"	//	2
             + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
             + " l.M_Product_ID,p.Name as Product,p.Value as PRODUCTSEARCHKEY, l.M_InOutLine_ID,l.Line," // 5..8
             + " l.C_OrderLine_ID, " // 9
             + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
             + " ins.description , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
             + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID  AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance
                , ol.PriceEntered ";
            if (isBaseLanguages != "")
            {
                sql += isBaseLanguages + " ";
            }

            sql += "INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) "
                + " LEFT JOIN C_OrderLine ol ON ol.C_OrderLine_ID = l.c_orderline_id "
                + " LEFT JOIN c_order o ON o.c_order_id = ol.c_order_id LEFT JOIN c_paymentterm pt ON pt.C_Paymentterm_id = o.c_paymentterm_id "
                //+ "LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                + @"LEFT JOIN (SELECT il.QtyInvoiced, il.M_InOutLine_ID FROM " +
                   (!isProvisionalInvoice ? " C_InvoiceLine il INNER JOIN C_Invoice I ON I.C_INVOICE_ID = il.C_INVOICE_ID " :
                                           " C_ProvisionalInvoiceLine il INNER JOIN C_ProvisionalInvoice I ON I.C_ProvisionalINVOICE_ID = il.C_ProvisionalINVOICE_ID ")
                + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                + "LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
                + "WHERE l.M_InOut_ID=" + mInOutId; // #1
            if (mProductIDD != "")
            {
                sql += mProductIDD + " ";
            }
            sql += " GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.M_Product_ID,p.Name,p.Value, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description , o.C_PaymentTerm_ID , pt.Name, ol.PriceEntered ";

            if (isBaseLanguages.ToUpper().Contains("C_UOM_TRL"))
            {
                sql += " ,uom1.stdprecision ";
            }
            else
            {
                sql += " ,uom.stdprecision ";
            }

            //JID_1728 Show Shipmentline with Charge also, based on the setting for Non Item type on Tenant.
            if (isAllownonItem)
            {
                sql += "UNION SELECT "
              //+ "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
              // Changes done by Bharat on 07 July 2017 restrict to create invoice if Invoice already created against that for same quantity
              + "ROUND((l.MovementQty-SUM(COALESCE(mi.QtyInvoiced,0))) * "                    //	1  
              + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QUANTITY,"    //	2
                                                                                                                             //+ "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
              + "ROUND((l.MovementQty-SUM(COALESCE(mi.QtyInvoiced,0))) * "                    //	1  
              + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QTYENTER," //	2
              + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
              + " 0 as M_Product_ID,c.Name as Product,c.Value as PRODUCTSEARCHKEY, l.M_InOutLine_ID,l.Line," // 5..8
              + " l.C_OrderLine_ID, " // 9
              + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
              + " ins.description , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
              + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID  AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance 
                   , ol.PriceEntered ";
                if (isBaseLanguages != "")
                {
                    sql += isBaseLanguages + " ";
                }

                sql += "INNER JOIN C_charge c ON (l.C_charge_ID=c.C_charge_ID) "
                    + " LEFT JOIN C_OrderLine ol ON ol.C_OrderLine_ID = l.c_orderline_id "
                    + " LEFT JOIN c_order o ON o.c_order_id = ol.c_order_id LEFT JOIN c_paymentterm pt ON pt.C_Paymentterm_id = o.c_paymentterm_id "
                    //+ "LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                    + "LEFT JOIN (SELECT il.QtyInvoiced, il.M_InOutLine_ID FROM " +
                   (!isProvisionalInvoice ? " C_InvoiceLine il INNER JOIN C_Invoice I ON I.C_INVOICE_ID = il.C_INVOICE_ID " :
                                           " C_ProvisionalInvoiceLine il INNER JOIN C_ProvisionalInvoice I ON I.C_ProvisionalINVOICE_ID = il.C_ProvisionalINVOICE_ID ")
                    + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                    + "LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
                    + "WHERE l.M_InOut_ID=" + mInOutId; // #1
                if (mProductIDD != "")
                {
                    sql += mProductIDD + " ";
                }
                sql += " GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                    + "l.M_Product_ID,c.Name,c.Value, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description , o.C_PaymentTerm_ID , pt.Name , ol.PriceEntered ";

                if (isBaseLanguages.ToUpper().Contains("C_UOM_TRL"))
                {
                    sql += " ,uom1.stdprecision ";
                }
                else
                {
                    sql += " ,uom.stdprecision ";
                }
            }

            string sqlNew = "SELECT * FROM (" + sql.ToString() + ") t " + (isProvisionalInvoice ? (" WHERE " + GetProvisionalLine()) : "") + " ORDER BY Line";

            return sqlNew;
        }

        /// <summary>
        /// Unused Function
        /// </summary>
        /// <param name="MInOutIDs"></param>
        /// <param name="isBaseLanguageUmos"></param>
        /// <returns></returns>
        public JsonResult GetShipmentDatas(string MInOutIDs, string isBaseLanguageUmos)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var stValue = obj.GetShipmentDatas(ctx, MInOutIDs, isBaseLanguageUmos);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Invoice Data
        /// </summary>
        /// <param name="displays"></param>
        /// <param name="cBPartnerId"></param>
        /// <param name="isReturnTrxs"></param>
        /// <returns></returns>
        public JsonResult GetInvoicesVCreate(string displays, int cBPartnerId, bool isReturnTrxs, bool IsDrops)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var stValue = obj.GetInvoicesVCreate(ctx, displays, cBPartnerId, isReturnTrxs, IsDrops);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get data from common model
        /// </summary>
        /// <param name="keyColumnName"></param>
        /// <param name="tableName"></param>
        /// <param name="recordID"></param>
        /// <param name="pageNo"></param>
        /// <param name="isBaseLangss"></param>
        /// <param name="cInvoiceID"></param>
        /// <param name="mProductIDs"></param>
        /// <returns></returns>
        public JsonResult GetInvoicesDataVCreate(string keyColumnName, string tableName, int recordID, int pageNo, string isBaseLangss, int cInvoiceID, string mProductIDs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = getSQlforGetInvoicesData(isBaseLangss, cInvoiceID, mProductIDs);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetInvoicesDataVCreate function
        /// </summary>
        /// <param name="isBaseLangss"></param>
        /// <param name="cInvoiceID"></param>
        /// <param name="mProductIDs"></param>
        /// <returns></returns>
        private string getSQlforGetInvoicesData(string isBaseLangss, int cInvoiceID, string mProductIDs)
        {
            #region[Commented By Sukhwinder on 17-Nov-2017 and updated the query below to prevent displaying product with "Cost adjustment on loss" with already MR.]
            //string sql = "SELECT "
            //            + "(l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            //            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ) as QUANTITY,"	//	2
            //            + "(l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            //            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ) as QTYENTER,"	//	2
            //            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
            //            + " l.M_Product_ID,p.Name as PRODUCT, l.C_InvoiceLine_ID,l.Line,"      //  5..8
            //            + " l.C_OrderLine_ID,"                   					//  9
            //            + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
            //            + " ins.description ";
            //if (isBaseLangss)
            //{
            //    //sql += " " + isBaseLangss + " ";
            //    sql += "FROM C_UOM uom INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ";
            //}
            //else
            //{
            //    sql += "FROM C_UOM_Trl uom INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='" + Env.GetAD_Language(ctx) + "') ";
            //}

            //sql += "INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) "
            //    + "LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
            //    + "LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
            //    + "WHERE l.C_Invoice_ID=" + cInvoiceID;
            //if (mProductIDs != "")
            //{
            //    sql += " " + mProductIDs + " ";
            //}
            //sql += " GROUP BY l.QtyInvoiced,l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
            //    + "l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description "
            //+ "ORDER BY l.Line";

            //return sql;
            #endregion

            //Updated Query by Sukhwinder for "Cost adjustment on loss"

            string precision = "3";
            if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
            {
                precision = " uom1.stdprecision ";
            }
            else
            {
                precision = " uom.stdprecision ";
            }

            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");

            StringBuilder sql = new StringBuilder("SELECT * FROM  ( "
                        + " SELECT "
                        + " ROUND((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QUANTITY,"	//	2
                        + " ROUND((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QTYENTER,"	//	2
                        + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
                        + " l.M_Product_ID,p.Name as PRODUCT,p.Value as PRODUCTSEARCHKEY, l.C_InvoiceLine_ID,l.Line,"      //  5..8
                        + " l.C_OrderLine_ID,"                   					//  9
                        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
                        + " ins.description, "
                        + " P.Iscostadjustmentonlost, "
                        + " Sum(Coalesce(Mi.Qty,0)) As Miqty, "
                        + " NVL(l.QtyInvoiced,0) as qtyInv , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' )
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ");

            if (isBaseLangss != "")
            {
                sql.Append(isBaseLangss);
            }

            if (isAllownonItem)
            {
                sql.Append(" LEFT JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID");
            }
            else
            {
                sql.Append(" INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID");
            }

            // Get lines from Invoice based on the setting taken on Tenant to allow non item Product
            if (!isAllownonItem)
            {
                sql.Append(" AND p.ProductType = 'I') ");  // JID_0350: In Grid of Material Receipt need to show the items type products only
            }
            else
            {
                sql.Append(") ");
            }

            sql.Append(" LEFT JOIN C_Invoice o ON o.C_Invoice_ID = l.C_Invoice_ID LEFT JOIN C_PaymentTerm pt ON pt.C_PaymentTerm_ID = o.C_PaymentTerm_ID "
             + " LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
             + " "
             + " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
             + " WHERE l.C_Invoice_ID=" + cInvoiceID + " AND l.M_Product_ID>0");

            if (mProductIDs != "")
            {
                sql.Append(mProductIDs);
            }
            sql.Append(" GROUP BY l.QtyInvoiced,l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + " l.M_Product_ID,p.Name, p.Value, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description, "
                + " p.IsCostAdjustmentOnLost, "
                + " L.Qtyinvoiced , o.C_PaymentTerm_ID , pt.Name ");

            if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
            {
                sql.Append(" , uom1.stdprecision ");
            }
            else
            {
                sql.Append(" , uom.stdprecision ");
            }

            // Show Invoice Line with Charge also, based on the setting for Non Item type on Tenant.
            if (isAllownonItem)
            {
                sql.Append(" UNION SELECT "
                  + "round((l.QtyInvoiced-SUM(COALESCE(m.QtyDelivered,0))) * "					//	1               
                  + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QUANTITY,"	//	2
                  + "round((l.QtyInvoiced-SUM(COALESCE(m.QtyDelivered,0))) * "
                  + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QTYENTER,"	//	added by bharat
                  + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as M_PRODUCT_ID, c.Name as PRODUCT,c.Value as PRODUCTSEARCHKEY, l.C_InvoiceLine_ID,l.Line,"	//	5..6
                  + " l.C_OrderLine_ID, l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
                  + " ins.description , "
                  + " 'N' AS Iscostadjustmentonlost, 0 As Miqty, 0 as qtyInv"			//	7..8 //              
                  + " , i.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                  // JID_1414 - not to consider or pick In-Active Record
                  + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =i.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ");

                if (isBaseLangss != "")
                {
                    sql.Append(isBaseLangss);
                }

                sql.Append(" LEFT OUTER JOIN C_Invoice i ON (i.C_Invoice_ID = l.C_Invoice_ID)"
                  + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = i.C_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN C_OrderLine m ON(m.C_OrderLine_ID = l.C_OrderLine_ID) AND m.C_OrderLine_ID IS NOT NULL LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");



                sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_Invoice_ID=" + cInvoiceID + " AND C.C_Charge_ID>0 ");

                sql.Append(" GROUP BY l.QtyInvoiced, l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                      + "l.M_Product_ID,c.Name, c.Value, l.C_InvoiceLine_ID, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description, i.C_PaymentTerm_ID , t.Name");

                if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
                {
                    sql.Append(" , uom1.stdprecision ");
                }
                else
                {
                    sql.Append(" , uom.stdprecision ");
                }
            }

            sql.Append(") t"
                   + " WHERE (   "
                   + "   CASE    "
                   + "     WHEN Iscostadjustmentonlost = 'N' "
                   + "     THEN 1    "
                   + "     ELSE  "
                   + "       CASE    "
                   + "         WHEN (Qtyinv - Miqty) = Qtyinv    "
                   + "         Then 1    "
                   + "         Else  "
                   + "         0 "
                   + "       END "
                   + "   END )=1  ORDER BY Line");
            return sql.ToString();
        }

        /// <summary>
        /// Get Data to Create Statement 
        /// </summary>
        /// <param name="pageNo">Page No</param>
        /// <param name="dates">Statement Date</param>
        /// <param name="trxDatess">Transaction Date of Payment</param>
        /// <param name="trxDatesUnions">Transaction Date for Cash Journa;</param>
        /// <param name="cBPartnerIDs">Business Partner</param>
        /// <param name="DocumentNos">Document No of Payment</param>
        /// <param name="DocumentNoUnions">Document No of Cash Journal</param>
        /// <param name="DepositSlips">Deposit Slip No</param>
        /// <param name="AuthCodes">Authrization Code</param>        
        /// <param name="cBankAccountId">Check No</param>
        /// <param name="cBankAccountId">Amount</param>
        /// <param name="cBankAccountId">Bank Account</param>
        /// <returns>Data in JSON Format</returns>
        public JsonResult GetBankAccountsData(int pageNo, string dates, string trxDatess, string trxDatesUnions, string cBPartnerIDs, string DocumentNos, string DocumentNoUnions, string DepositSlips,
            string AuthCodes, string CheckNos, string Amounts, int cBankAccountId)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = GetBankAccountsDataSql(dates, trxDatess, trxDatesUnions, cBPartnerIDs, DocumentNos, DocumentNoUnions, DepositSlips, AuthCodes, CheckNos, Amounts, cBankAccountId);
            var stValue = obj.GetAccountData(sql, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  create sql qry for GetBankAccountsData function
        /// </summary>
        /// <param name="trxDatess">Transaction Date of Payment</param>
        /// <param name="trxDatesUnions">Transaction Date for Cash Journa;</param>
        /// <param name="cBPartnerIDs">Business Partner</param>
        /// <param name="DocumentNos">Document No of Payment</param>
        /// <param name="DocumentNoUnions">Document No of Cash Journal</param>
        /// <param name="DepositSlips">Deposit Slip No</param>
        /// <param name="AuthCodes">Authrization Code</param>
        /// <param name="cBankAccountId">Check No</param>
        /// <param name="cBankAccountId">Amount</param>
        /// <param name="cBankAccountId">Bank Account</param>
        /// <returns>String Query</returns>
        private string GetBankAccountsDataSql(string dates, string trxDatess, string trxDatesUnions, string cBPartnerIDs, string DocumentNos, string DocumentNoUnions, string DepositSlips,
            string AuthCodes, string CheckNos, string Amounts, int cBankAccountId)
        {
            bool countVA034 = Env.IsModuleInstalled("VA034_"); //Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA034_' AND IsActive = 'Y'"));
            StringBuilder sql = new StringBuilder();
            // JID_0084: Create line from is always picking curreny type that is default. It should pick currency type that is on payment.
            sql.Append("SELECT p.DateAcct AS DateTrx, p.C_Payment_ID, p.DocumentNo, ba.C_Currency_ID, c.ISO_Code, p.PayAmt,"
                // JID_0333: Currency conversion should be based on Payment Account Date and Currency type
                + " currencyConvert(p.PayAmt,p.C_Currency_ID,ba.C_Currency_ID,p.DateAcct,p.C_ConversionType_ID,p.AD_Client_ID,p.AD_Org_ID) AS ConvertedAmt,"   //  #1
                + " pay.Description, bp.Name, 'P' AS Type");

            if (countVA034)
                sql.Append(", p.VA034_DepositSlipNo");

            sql.Append(", p.TrxNo, p.CheckNo, pay.C_ConversionType_ID  FROM C_BankAccount ba"
                + " INNER JOIN C_Payment_v p ON (p.C_BankAccount_ID=ba.C_BankAccount_ID)"
                + " INNER JOIN C_Payment pay ON p.C_Payment_ID=pay.C_Payment_ID"
                + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID)"
                + " LEFT OUTER JOIN C_BPartner bp ON (p.C_BPartner_ID=bp.C_BPartner_ID) "
                + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
                + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
                + " AND p.DateAcct <= " + dates      //JID_1293: it should show only those payment having trx date less than Bank Statment date 
                + " AND p.C_BankAccount_ID = " + cBankAccountId);                           	//  #2
            if (cBPartnerIDs != "0")
            {
                sql.Append(cBPartnerIDs);
            }
            if (DocumentNos != "")
            {
                sql.Append(DocumentNos);
            }
            if (DepositSlips != "")
            {
                sql.Append(" AND p.va034_depositslipno LIKE '" + DepositSlips + "'");
            }
            if (AuthCodes != "")
            {
                sql.Append(" AND p.TrxNo LIKE '" + AuthCodes + "'");
            }
            if (CheckNos != "")
            {
                sql.Append(" AND p.CheckNo LIKE '%" + CheckNos + "%'");
            }
            if (Amounts != "0")
            {
                sql.Append(" AND p.PayAmt = " + Amounts);
            }
            if (trxDatess != "")
            {
                sql.Append(trxDatess);
            }
            sql.Append(" AND NOT EXISTS (SELECT * FROM C_BankStatementLine l WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0)");         //	Voided Bank Statements have 0 StmtAmt

            bool countVA012 = Env.IsModuleInstalled("VA012_");    //Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
            if (countVA012)
            {
                // JID_0084: Create line from is always picking curreny type that is default. It should pick currency type that is on Cash Journal.
                // when cash journal +ve, bank statment to be -ve and vice versa
                sql.Append(" UNION SELECT cs.DateAcct AS DateTrx, cl.C_CashLine_ID AS C_Payment_ID, cs.DocumentNo, ba.C_Currency_ID, c.ISO_Code, (-1)*cl.Amount AS PayAmt,"
                + " currencyConvert((-1)*cl.Amount,cl.C_Currency_ID,ba.C_Currency_ID,cs.DateAcct,cl.C_ConversionType_ID,cs.AD_Client_ID,cs.AD_Org_ID) AS ConvertedAmt,"   //  #1
                + " cl.Description, Null AS Name, 'C' AS Type");

                if (countVA034)
                    sql.Append(", NULL AS VA034_DepositSlipNo");
                //added C_ConversionType_ID to append the value on to the Bank statement Line
                sql.Append(", null AS TrxNo, null AS CheckNo,cl.C_ConversionType_ID FROM C_BankAccount ba"
                         + " INNER JOIN C_CashLine cl ON (cl.C_BankAccount_ID=ba.C_BankAccount_ID)"
                         + " INNER JOIN C_Cash cs ON (cl.C_Cash_ID=cs.C_Cash_ID)"
                         + " INNER JOIN C_Charge chrg ON chrg.C_Charge_ID=cl.C_Charge_ID"
                         + " INNER JOIN C_Currency c ON (cl.C_Currency_ID=c.C_Currency_ID)"
                         + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
                         + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
                         + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
                         + " AND cs.DateAcct <= " + dates       // JID_1293: it should show only those Cash journal having statement date less than Bank Statment date 
                         + " AND cl.C_BankAccount_ID = " + cBankAccountId);                             	//  #2            
                if (DocumentNoUnions != "")
                {
                    sql.Append(DocumentNoUnions);
                }
                if (trxDatesUnions != "")
                {
                    sql.Append(trxDatesUnions);
                }
                sql.Append(" AND NOT EXISTS (SELECT * FROM C_BankStatementLine l WHERE cl.C_CashLine_ID=l.C_CashLine_ID AND l.StmtAmt <> 0)");      //	Voided Bank Statements have 0 StmtAmt
            }

            return sql.ToString();
        }

        /// <summary>
        ///  VInOutGen Search Data
        /// </summary>
        /// <param name="adClientID"></param>
        /// <param name="mWarehouseIDs"></param>
        /// <param name="cBPartnerIDs"></param>
        /// <param name="cOrderIDSearchs"></param>
        /// <returns></returns>
        public JsonResult ExecuteQueryVinoutgen(int adClientID, string mWarehouseIDs, string cBPartnerIDs, string cOrderIDSearchs)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.ExecuteQueryVinoutgen(ctx, adClientID, mWarehouseIDs, cBPartnerIDs, cOrderIDSearchs);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// VInOutGen get data from c_order
        /// </summary>
        /// <param name="adClientID"></param>
        /// <param name="adOrgIDs"></param>
        /// <param name="cBPartnerIDs"></param>
        /// <param name="ordShipmentids"></param>
        /// <returns></returns>
        public JsonResult ExecuteQueryVInvoiceGen(int adClientID, string adOrgIDs, string cBPartnerIDs, string ordShipmentids)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.ExecuteQueryVInvoiceGen(ctx, adClientID, adOrgIDs, cBPartnerIDs, ordShipmentids);
            // return Json(new { result = value }, JsonRequestBehavior.AllowGet);

            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get match data for Order, Invoice and Ship/Receipt on Matching PO - invoice form.
        /// </summary>
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
        /// <returns></returns>
        public JsonResult GetTableLoadVmatch(string displayMATCH_INVOICEs, bool chkIsReturnTrxProps, string displayMATCH_ORDERs, string matchToTypeMATCH_SHIPMENTs,
                                             bool matchedsss, string chkSameBPartnerss, string chkSameProductss, string chkSameQtyss, string from_ss, string fromIfs, string to_Dats, string matchToTypes, string MATCH_SHIPMENTs, string MATCH_ORDERs, string onlyProduct_ss, string onlyVendor_ss, string MatchToID)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var res = obj.GetTableLoadVmatch(ctx, displayMATCH_INVOICEs, chkIsReturnTrxProps, displayMATCH_ORDERs, matchToTypeMATCH_SHIPMENTs, matchedsss,
                          chkSameBPartnerss, chkSameProductss, chkSameQtyss, from_ss, fromIfs, to_Dats, matchToTypes, MATCH_SHIPMENTs, MATCH_ORDERs, onlyProduct_ss, onlyVendor_ss, MatchToID);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }


        ///// <summary>
        ///// use to Get Product Container on the basis of given paameter
        ///// </summary>
        ///// <param name="Name"></param>
        ///// <param name="WarehouseId"></param>
        ///// <param name="LocatorId"></param>
        ///// <returns></returns>
        //public JsonResult GetContainer(int M_Locator_ID)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    VCreateFromModel obj = new VCreateFromModel();
        //    return Json(JsonConvert.SerializeObject(obj.GetContainer(ctx, M_Locator_ID)), JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Get converted Amount with Success or Error message
        /// </summary>
        /// <param name="_paymentId">C_CashLine_ID or C_Payment_ID</param>
        /// <param name="amount">Amount</param>
        /// <param name="currencyId">C_Currency_ID</param>
        /// <param name="convsion_Id">C_ConversionType_ID</param>
        /// <param name="date">Account Date</param>
        /// <param name="paymentType">Payment Type(Payment or CashLine)</param>
        /// <param name="_org_id">AD_Org_ID</param>
        /// <returns>return type list contains ConvetedAmt and message</returns>
        public JsonResult GetConvertedAmount(int _paymentId, decimal? amount, int? currencyId, int? convsion_Id, DateTime? date, string paymentType, int? _org_id)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var res = obj.GetConvertedAmount(ctx, _paymentId, amount, currencyId, convsion_Id, date, paymentType, _org_id);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Provisional Invoices
        /// </summary>
        /// <param name="displays">Display Columns</param>
        /// <param name="C_BPartner_IDs">Business Partner</param>
        /// <param name="isReturnTrxs">Return Transaction</param>
        /// <param name="OrgIds">Organization</param>
        ///  <param name="recordID">C_Invoice_ID</param>
        /// <returns>List of Provisonal in Json Format Bind to Combo</returns>
        public JsonResult VCreateGetProvisionalInvoices(string displays, int C_BPartner_IDs, bool isReturnTrxs, int OrgIds, int recordID)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.VCreateGetProvosionalInvoice(ctx, displays, C_BPartner_IDs, OrgIds, isReturnTrxs, recordID);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get data from common model
        /// </summary>
        /// <param name="keyColumnName">Primary column</param>
        /// <param name="tableName">table name</param>
        /// <param name="recordID">record id</param>
        /// <param name="pageNo">page no</param>
        /// <param name="isBaseLangss">browser language</param>
        /// <param name="cInvoiceID">invoice id</param>
        /// <param name="mProductIDs">products ids</param>
        /// <param name="orgId">orgazination id</param>
        /// <returns></returns>
        public JsonResult GetProvisionalInvoicesDataVCreate(string keyColumnName, string tableName, int recordID, int pageNo, string isBaseLangss, int cInvoiceID, string mProductIDs, int? orgId)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = GetSQlforGetProvisionalInvoicesData(isBaseLangss, cInvoiceID, mProductIDs, orgId);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetProvisionalInvoicesDataVCreate function
        /// </summary>
        /// <param name="isBaseLangss">browser language</param>
        /// <param name="cInvoiceID">invoice id</param>
        /// <param name="mProductIDs">products ids</param>
        /// <returns>sql query</returns>
        private string GetSQlforGetProvisionalInvoicesData(string isBaseLangss, int cInvoiceID, string mProductIDs, int? orgId)
        {
            string precision = "3";
            if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
            {
                precision = " uom1.stdprecision ";
            }
            else
            {
                precision = " uom.stdprecision ";
            }

            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");

            StringBuilder sql = new StringBuilder("SELECT * FROM  ( "
                        + " SELECT "
                        + " ROUND((l.QtyInvoiced) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QUANTITY,"	//	2
                        + " ROUND((l.QtyInvoiced) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QTYENTER,"	//	2
                        + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
                        + " l.M_Product_ID,p.Name as PRODUCT,p.Value as PRODUCTSEARCHKEY, l.C_ProvisionalInvoiceLine_ID,l.Line,"      //  5..8
                        + " l.C_OrderLine_ID,"                   					//  9
                        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
                        + " ins.description, "
                        + " P.Iscostadjustmentonlost, "
                        + " NVL(l.QtyInvoiced,0) as qtyInv , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' )
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance,l.PricePO,l.PriceEntered ");

            if (isBaseLangss != "")
            {
                sql.Append(isBaseLangss);
            }

            if (isAllownonItem)
            {
                sql.Append(" LEFT JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID");
            }
            else
            {
                sql.Append(" INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID");
            }

            // Get lines from Invoice based on the setting taken on Tenant to allow non item Product
            if (!isAllownonItem)
            {
                sql.Append(" AND p.ProductType = 'I') ");  // JID_0350: In Grid of Material Receipt need to show the items type products only
            }
            else
            {
                sql.Append(") ");
            }

            sql.Append(" LEFT JOIN C_ProvisionalInvoice o ON o.C_ProvisionalInvoice_ID = l.C_ProvisionalInvoice_ID LEFT JOIN C_PaymentTerm pt ON pt.C_PaymentTerm_ID = o.C_PaymentTerm_ID "
             + " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
             + " WHERE l.C_ProvisionalInvoice_ID=" + cInvoiceID + " AND l.M_Product_ID>0");

            if (orgId != null)
            {
                sql.Append(" AND L.AD_Org_ID=" + orgId + " ");
            }

            if (mProductIDs != "")
            {
                sql.Append(mProductIDs);
            }

            sql.Append(" GROUP BY l.QtyInvoiced,l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + " l.M_Product_ID,p.Name, p.Value, l.C_ProvisionalInvoiceLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description, "
                + " p.IsCostAdjustmentOnLost, "
                + " L.Qtyinvoiced , o.C_PaymentTerm_ID , pt.Name,l.PricePO,l.PriceEntered ");

            if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
            {
                sql.Append(" , uom1.stdprecision ");
            }
            else
            {
                sql.Append(" , uom.stdprecision ");
            }

            // Show Invoice Line with Charge also, based on the setting for Non Item type on Tenant.
            if (isAllownonItem)
            {
                sql.Append(" UNION SELECT "
                  + "round((l.QtyInvoiced-SUM(COALESCE(m.QtyDelivered,0))) * "					//	1               
                  + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QUANTITY,"	//	2
                  + "round((l.QtyInvoiced-SUM(COALESCE(m.QtyDelivered,0))) * "
                  + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QTYENTER,"	//	added by bharat
                  + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as M_PRODUCT_ID, c.Name as PRODUCT,c.Value as PRODUCTSEARCHKEY, l.C_ProvisionalInvoiceLine_ID,l.Line,"	//	5..6
                  + " l.C_OrderLine_ID, l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
                  + " ins.description , "
                  + " 'N' AS Iscostadjustmentonlost, 0 as qtyInv"			//	7..8 //              
                  + " , i.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                  // JID_1414 - not to consider or pick In-Active Record
                  + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' ) 
                        WHERE c_paymentterm.c_paymentterm_ID =i.C_PaymentTerm_ID AND C_PaymentTerm.IsActive = 'Y' ) AS IsAdvance,l.PricePO,l.PriceEntered ");

                if (isBaseLangss != "")
                {
                    sql.Append(isBaseLangss);
                }

                sql.Append(" LEFT OUTER JOIN C_ProvisionalInvoice i ON (i.C_ProvisionalInvoice_ID = l.C_ProvisionalInvoice_ID)"
                  + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = i.C_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN C_OrderLine m ON(m.C_OrderLine_ID = l.C_OrderLine_ID) AND m.C_OrderLine_ID IS NOT NULL LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");



                sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_ProvisionalInvoice_ID=" + cInvoiceID + " AND C.C_Charge_ID>0");

                if (orgId != null)
                {
                    sql.Append(" AND L.AD_Org_ID=" + orgId + " ");
                }
                sql.Append(" GROUP BY l.QtyInvoiced, l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                      + "l.M_Product_ID,c.Name, c.Value, l.C_ProvisionalInvoiceLine_ID, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description, i.C_PaymentTerm_ID , t.Name,l.PricePO,l.PriceEntered");

                if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
                {
                    sql.Append(" , uom1.stdprecision ");
                }
                else
                {
                    sql.Append(" , uom.stdprecision ");
                }
            }
            // Get Provisional Invoice line records which are not exits in AP Invoice Line and not considered void or reverse invoices
            sql.Append(") t WHERE t.C_PROVISIONALINVOICELINE_ID NOT IN(SELECT C_PROVISIONALINVOICELINE_ID FROM C_InvoiceLine INNER JOIN C_Invoice ON C_Invoice.C_Invoice_ID = C_InvoiceLine.C_Invoice_ID " +
                "WHERE C_InvoiceLine.IsActive = 'Y' AND C_PROVISIONALINVOICELINE_ID > 0 AND C_Invoice.DocStatus NOT IN('VO', 'RE')) ORDER BY Line");
            return sql.ToString();
        }
    }
}