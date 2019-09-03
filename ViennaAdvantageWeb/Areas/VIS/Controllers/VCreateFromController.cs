using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Utility;
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
        /// <returns>List of Orders in Json Format</returns>
        public JsonResult VCreateGetOrders(string displays, string columns, int C_BPartner_IDs, bool isReturnTrxs, int OrgIds, bool IsDrop, bool IsSOTrx, bool forInvoices)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.VCreateGetOrders(ctx, displays, columns, C_BPartner_IDs, isReturnTrxs, OrgIds, IsDrop, IsSOTrx, forInvoices);
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
                        + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"
                        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
                        + " ins.description , "
                //+ " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW "//Arpit on  20th Sept,2017
                  + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip " //Arpit
                  + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                  + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance "
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
                + "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description, l.IsDropShip  " //Arpit on  20th Sept,2017
                //+ "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description  "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") WHERE QUANTITY > 0";

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
            string sql = VcreateFormSqlQryOrg(forInvoicees, C_Ord_IDs, isBaseLangess, MProductIDss, DelivDates);
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
        /// <returns>String, Query</returns>
        private string VcreateFormSqlQryOrg(bool forInvoicees, int? C_Ord_IDs, string isBaseLangess, string MProductIDss, string DelivDates)
        {
            StringBuilder sql = new StringBuilder("SELECT "
               + "ROUND((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ), uom.stdprecision) as QUANTITY,"
               + "ROUND((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ), uom.stdprecision) as QTYENTER,"
               + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
               + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,p.Name as PRODUCT,"
               + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
               + " ins.description , "
               + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, uom.stdprecision AS StdPrecision  , l.IsDropShip AS  IsDropShip" //Arpit on  20th Sept,2017
               + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
               + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance "
               + " FROM C_OrderLine l"
               + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
               + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
               + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

            sql.Append(forInvoicees ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
            sql.Append(" IS NOT NULL) INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)");

            if (isBaseLangess != "")
            {
                sql.Append(isBaseLangess);
            }
            sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_Order_ID=" + C_Ord_IDs 
                + " AND l.M_Product_ID>0");

            if (!forInvoicees)
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
            sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                    + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                        + "l.M_Product_ID,p.Name, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description,  uom.stdprecision,l.IsDropShip, o.C_PaymentTerm_ID , t.Name  "); //Arpit on  20th Sept,2017"	            

            if (forInvoicees)
            {
                sql.Append("UNION SELECT "
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "					//	1               
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),uom.stdprecision) as QUANTITY,"	//	2
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),uom.stdprecision) as QTYENTER,"	//	added by bharat
                  + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as M_PRODUCT_ID , c.Name as PRODUCT,"	//	5..6
                  + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
                  + " ins.description , "
                  + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, uom.stdprecision AS StdPrecision   , l.IsDropShip AS  IsDropShip"			//	7..8 //              
                  + " , o.C_PaymentTerm_ID , t.Name AS PaymentTermName "
                  + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance "
                  + " FROM C_OrderLine l"
                  + " LEFT OUTER JOIN C_Order o ON (o.C_Order_ID = l.C_Order_ID)"
                  + " LEFT OUTER JOIN C_PaymentTerm t ON (t.C_PaymentTerm_ID = o.C_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN C_INVOICELINE M ON(L.C_OrderLine_ID=M.C_OrderLine_ID) AND ");

                sql.Append(forInvoicees ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
                sql.Append(" IS NOT NULL LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

                if (isBaseLangess != "")
                {
                    sql.Append(isBaseLangess);
                }

                sql.Append(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) WHERE l.C_Order_ID=" + C_Ord_IDs + " AND C.C_Charge_ID >0 ");

                if (DelivDates != "")
                {
                    sql.Append(DelivDates);
                }
                sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                        + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                               + "l.M_Product_ID,c.Name, l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description, uom.stdprecision, l.IsDropShip , o.C_PaymentTerm_ID , t.Name  ");
            }

            string sqlNew = "SELECT * FROM (" + sql.ToString() + ") WHERE QUANTITY > 0";

            return sqlNew;
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
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance "
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
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance "
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

            string sqlNew = "SELECT * FROM (" + sql + ") WHERE QUANTITY > 0";

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
        /// Get Shipment data
        /// </summary>
        /// <param name="displays"></param>
        /// <param name="CBPartnerIDs"></param>
        /// <returns></returns>
        public JsonResult GetShipments(string displays, int CBPartnerIDs, bool IsDrop, bool IsSOTrx)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var stValue = obj.GetShipments(ctx, displays, CBPartnerIDs, IsDrop, IsSOTrx);
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
            var sql = GetDataSqlQueries(mInOutId, isBaseLanguages, mProductIDD);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetData function
        /// </summary>
        /// <param name="mInOutId"></param>
        /// <param name="isBaseLanguages"></param>
        /// <param name="mProductIDD"></param>
        /// <returns></returns>
        private string GetDataSqlQueries(string mInOutId, string isBaseLanguages, string mProductIDD)
        {
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
             + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QUANTITY,"	//	2
                //+ "round((l.MovementQty-SUM(COALESCE(mi.Qty,0))) * "					//	1               
             + "ROUND((l.MovementQty-SUM(COALESCE(mi.QtyInvoiced,0))) * "					//	1  
             + "(CASE WHEN l.MovementQty=0 THEN 0 ELSE l.QtyEntered/l.MovementQty END )," + precision + ") as QTYENTER,"	//	2
             + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
             + " l.M_Product_ID,p.Name as Product, l.M_InOutLine_ID,l.Line," // 5..8
             + " l.C_OrderLine_ID, " // 9
             + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
             + " ins.description , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
             + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance ";
            if (isBaseLanguages != "")
            {
                sql += isBaseLanguages + " ";
            }

            sql += "INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) "
                + " LEFT JOIN C_OrderLine ol ON ol.C_OrderLine_ID = l.c_orderline_id "
                + " LEFT JOIN c_order o ON o.c_order_id = ol.c_order_id LEFT JOIN c_paymentterm pt ON pt.C_Paymentterm_id = o.c_paymentterm_id "
                //+ "LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                + "LEFT JOIN (SELECT il.QtyInvoiced, il.M_InOutLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice I ON I.C_INVOICE_ID = il.C_INVOICE_ID "
                + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) "
                + "LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
                + "WHERE l.M_InOut_ID=" + mInOutId; // #1
            if (mProductIDD != "")
            {
                sql += mProductIDD + " ";
            }
            sql += " GROUP BY l.MovementQty, l.QtyEntered," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description , o.C_PaymentTerm_ID , pt.Name ";

            if (isBaseLanguages.ToUpper().Contains("C_UOM_TRL"))
            {
                sql += " ,uom1.stdprecision ORDER BY l.Line";
            }
            else
            {
                sql += " ,uom.stdprecision ORDER BY l.Line";
            }



            return sql;
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

            string sql = "SELECT * FROM  ( "

                        + " SELECT "
                        + " ROUND((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QUANTITY,"	//	2
                        + " ROUND((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
                        + " (CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END )," + precision + ") as QTYENTER,"	//	2
                        + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
                        + " l.M_Product_ID,p.Name as PRODUCT, l.C_InvoiceLine_ID,l.Line,"      //  5..8
                        + " l.C_OrderLine_ID,"                   					//  9
                        + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID,"
                        + " ins.description, "
                        + " P.Iscostadjustmentonlost, "
                        + " Sum(Coalesce(Mi.Qty,0)) As Miqty, "
                        + " NVL(l.QtyInvoiced,0) as qtyInv , o.C_PaymentTerm_ID , pt.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM c_paymentterm LEFT JOIN C_PaySchedule ON c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID
                        WHERE c_paymentterm.c_paymentterm_ID =o.C_PaymentTerm_ID ) AS IsAdvance ";

            if (isBaseLangss != "")
            {
                sql += " " + isBaseLangss + " ";
            }

            sql += " INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) "
                  + " LEFT JOIN C_Invoice o ON o.C_Invoice_ID = l.C_Invoice_ID LEFT JOIN C_PaymentTerm pt ON pt.C_PaymentTerm_ID = o.C_PaymentTerm_ID "
                + " LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
                + " "
                + " LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) "
                + " WHERE l.C_Invoice_ID=" + cInvoiceID;
            if (mProductIDs != "")
            {
                sql += " " + mProductIDs + " ";
            }
            sql += " GROUP BY l.QtyInvoiced,l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID,l.M_AttributeSetInstance_ID,ins.description, "
                + " p.IsCostAdjustmentOnLost, "
                + " L.Qtyinvoiced , o.C_PaymentTerm_ID , pt.Name ";

            if (isBaseLangss.ToUpper().Contains("C_UOM_TRL"))
            {
                sql += " , uom1.stdprecision ORDER BY l.Line";
            }
            else
            {
                sql += " , uom.stdprecision ORDER BY l.Line";
            }


            sql += ") "
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
                   + "   END )=1 ";
            return sql;
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

            sql.Append(", p.TrxNo, p.CheckNo  FROM C_BankAccount ba"
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
                sql.Append(" UNION SELECT cs.DateAcct AS DateTrx, cl.C_CashLine_ID AS C_Payment_ID, cs.DocumentNo, ba.C_Currency_ID, c.ISO_Code, cl.Amount AS PayAmt,"
                + " currencyConvert(cl.Amount,cl.C_Currency_ID,ba.C_Currency_ID,cs.DateAcct,cl.C_ConversionType_ID,cs.AD_Client_ID,cs.AD_Org_ID) AS ConvertedAmt,"   //  #1
                + " cl.Description, Null AS Name, 'C' AS Type");

                if (countVA034)
                    sql.Append(", NULL AS VA034_DepositSlipNo");

                sql.Append(", null AS TrxNo, null AS CheckNo FROM C_BankAccount ba"
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


    }
}