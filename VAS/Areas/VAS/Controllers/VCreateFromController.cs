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
        /// <param name="VAB_BusinessPartner_IDs">Business Partner</param>
        /// <param name="isReturnTrxs">Return Transaction</param>
        /// <param name="OrgIds">Organization</param>
        /// <param name="IsDrop">Drop Shipment</param>
        /// <param name="IsSOTrx">Sales Transaction</param>
        /// <param name="forInvoices">For Invoice</param>
        ///  <param name="recordID">VAB_Invoice_ID</param>
        /// <returns>List of Orders in Json Format</returns>
        public JsonResult VCreateGetOrders(string displays, string columns, int VAB_BusinessPartner_IDs, bool isReturnTrxs, int OrgIds, bool IsDrop, bool IsSOTrx, bool forInvoices , int recordID)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var value = obj.VCreateGetOrders(ctx, displays, columns, VAB_BusinessPartner_IDs, isReturnTrxs, OrgIds, IsDrop, IsSOTrx, forInvoices , recordID);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get VAB_Order_id or VAB_Orderline_id
        /// </summary>
        /// <param name="keyColumnName"></param>
        /// <param name="tableName"></param>
        /// <param name="recordID"></param>
        /// <param name="pageNo"></param>
        /// <param name="forInvoicees"></param>
        /// <param name="C_Ord_IDs"></param>
        /// <param name="isBaseLangess"></param>
        /// <param name="MVAMProductIDss"></param>
        /// <param name="DelivDates"></param>
        /// <param name="adOrgIDSs"></param>
        /// <returns></returns>
        public JsonResult GetOrdersDataCommon(string keyColumnName, string tableName, int recordID, int pageNo, bool forInvoicees, int C_Ord_IDs, string isBaseLangess, string MVAMProductIDss, string DelivDates, int adOrgIDSs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = VcreateFormSqlQry(forInvoicees, C_Ord_IDs, isBaseLangess, MVAMProductIDss, DelivDates, adOrgIDSs);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        private string VcreateFormSqlQry(bool forInvoicees, int C_Ord_IDs, string isBaseLangess, string MVAMProductIDss, string DelivDates, int adOrgIDSs)
        {
            var sql = "SELECT "
                        + "(l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
                        + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ) as QUANTITY,"
                        + "(l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
                        + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ) as QTYENTER,"
                        + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
                        + " COALESCE(l.VAM_Product_ID,0) as VAM_Product_ID ,COALESCE(p.Name,c.Name) as PRODUCT,COALESCE(p.Value,c.Value) as PRODUCTSEARCHKEY,"
                        + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID ,"
                        + " ins.description , "
                        + " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "
                        + " , o.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                            FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON (VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) " +
                        // JID_1414 - not to consider or pick In-Active Record
                        "  WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y') AS IsAdvance "
                        + " FROM VAB_OrderLine l"
                        + " LEFT OUTER JOIN VAM_MatchPO m ON (l.VAB_OrderLine_ID=m.VAB_OrderLine_ID AND "
                        + (forInvoicees ? "m.VAB_InvoiceLine_ID" : "m.VAM_Inv_InOutLine_ID")
                        + " IS NOT NULL)"
                        + " LEFT OUTER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID)"
                        + " LEFT OUTER JOIN VAB_Order o ON (o.VAB_Order_ID = l.VAB_Order_ID)"
                        + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID)"
                        + " LEFT OUTER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID)";

            if (isBaseLangess != "")
            {
                sql += " " + isBaseLangess;
            }

            sql += " LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) "
             + " WHERE l.VAB_Order_ID=" + C_Ord_IDs;

            if (MVAMProductIDss != "")
            {
                sql += " " + MVAMProductIDss;
            }
            if (DelivDates != "")
            {
                sql += " " + DelivDates;
            }
            sql += " AND l.DTD001_Org_ID = " + adOrgIDSs
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                + "l.VAM_Product_ID,COALESCE(p.Name,c.Name),COALESCE(p.Value,c.Value),l.VAM_PFeature_SetInstance_ID , l.Line,l.VAB_OrderLine_ID, ins.description, l.IsDropShip, o.VAB_PaymentTerm_ID , t.Name "
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
        /// <param name="MVAMProductIDss">Order ID</param>
        /// <param name="DelivDates">Delivery Date</param>
        /// <returns>Data in Json Format</returns>
        public JsonResult GetOrdersDataCommonOrg(string keyColumnName, string tableName, int recordID, int pageNo, bool forInvoicees, int? C_Ord_IDs, string isBaseLangess, string MVAMProductIDss, string DelivDates)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = VcreateFormSqlQryOrg(forInvoicees, C_Ord_IDs, isBaseLangess, MVAMProductIDss, DelivDates);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// create sql query for GetOrdersDataCommonOrg functions
        /// </summary>
        /// <param name="forInvoicees">true if open from Invoice window</param>
        /// <param name="C_Ord_IDs">Order ID</param>
        /// <param name="isBaseLangess">true if Base Language</param>
        /// <param name="MVAMProductIDss">Product ID</param>
        /// <param name="DelivDates">Delivery Date</param>
        /// <returns>String, Query</returns>
        private string VcreateFormSqlQryOrg(bool forInvoicees, int? C_Ord_IDs, string isBaseLangess, string MVAMProductIDss, string DelivDates)
        {
            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");

            // JID_1720 : when login with other than base language then data not coming in create line form
            string precision = "";
            if (isBaseLangess.ToUpper().Contains("VAB_UOM_TL"))
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
               + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
               + " COALESCE(l.VAM_Product_ID,0) as VAM_Product_ID ,p.Name as PRODUCT, p.Value as PRODUCTSEARCHKEY,"
               + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID ,"
               + " ins.description , "
               + " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, " + precision + " AS StdPrecision  , l.IsDropShip AS  IsDropShip"
               + " , o.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
               // JID_1414 - not to consider or pick In-Active Record
               + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON (VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' )
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
               + " FROM VAB_OrderLine l"
               + " LEFT OUTER JOIN VAB_Order o ON (o.VAB_Order_ID = l.VAB_Order_ID)"
               + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID)"
               + " LEFT OUTER JOIN VAM_MatchPO m ON (l.VAB_OrderLine_ID=m.VAB_OrderLine_ID AND ");

            sql.Append(forInvoicees ? "m.VAB_InvoiceLine_ID" : "m.VAM_Inv_InOutLine_ID");

            // Get lines from Order based on the setting taken on Tenant to allow non item Product
            if (!isAllownonItem)
            {
                sql.Append(" IS NOT NULL) INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID)");
            }
            else
            {
                sql.Append(" IS NOT NULL) LEFT JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID)");
            }

            if (isBaseLangess != "")
            {
                sql.Append(isBaseLangess);
            }
            sql.Append(" LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) WHERE l.VAB_Order_ID=" + C_Ord_IDs + " AND l.VAM_Product_ID>0");

            // Get lines from Order based on the setting taken on Tenant to allow non item Product
            if (!forInvoicees && !isAllownonItem)
            {
                sql.Append(" AND p.ProductType='I' ");
            }

            if (MVAMProductIDss != "")
            {
                sql.Append(MVAMProductIDss);
            }
            if (DelivDates != "")
            {
                sql.Append(DelivDates);
            }
            sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                    + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                        + "l.VAM_Product_ID,p.Name,p.Value, l.VAM_PFeature_SetInstance_ID, l.Line,l.VAB_OrderLine_ID, ins.description,  " + precision + ",l.IsDropShip, o.VAB_PaymentTerm_ID , t.Name  "); //Arpit on  20th Sept,2017"	            

            // Show Orderline with Charge also, based on the setting for Non Item type on Tenant.
            if (forInvoicees || isAllownonItem)
            {
                sql.Append("UNION SELECT "
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "					//	1               
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END )," + precision + ") as QUANTITY,"	//	2
                  + "round((l.QtyOrdered-SUM(COALESCE(m.QtyInvoiced,0))) * "
                  + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END )," + precision + ") as QTYENTER,"	//	added by bharat
                  + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as VAM_Product_ID , c.Name as PRODUCT, c.Value as PRODUCTSEARCHKEY,"	//	5..6
                  + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID ,"
                  + " ins.description , "
                  + " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW, " + precision + " AS StdPrecision   , l.IsDropShip AS  IsDropShip"			//	7..8 //              
                  + " , o.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
                  // JID_1414 - not to consider or pick In-Active Record
                  + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                  + " FROM VAB_OrderLine l"
                  + " LEFT OUTER JOIN VAB_Order o ON (o.VAB_Order_ID = l.VAB_Order_ID)"
                  + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN VAB_INVOICELINE M ON(L.VAB_OrderLine_ID=M.VAB_OrderLine_ID) AND ");

                sql.Append(forInvoicees ? "m.VAB_InvoiceLine_ID" : "m.VAM_Inv_InOutLine_ID");
                sql.Append(" IS NOT NULL LEFT OUTER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID)");

                if (isBaseLangess != "")
                {
                    sql.Append(isBaseLangess);
                }

                sql.Append(" LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) WHERE l.VAB_Order_ID=" + C_Ord_IDs + " AND C.VAB_Charge_ID >0 ");

                if (DelivDates != "")
                {
                    sql.Append(DelivDates);
                }

                sql.Append(" GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                      + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                      + "l.VAM_Product_ID,c.Name,c.Value,l.VAM_PFeature_SetInstance_ID, l.Line,l.VAB_OrderLine_ID, ins.description, " + precision + ", l.IsDropShip , o.VAB_PaymentTerm_ID , t.Name");
            }
            // JID_1287: Line number sequence to be maintained when we create lines from the reference of other documents.
            string sqlNew = "SELECT * FROM (" + sql.ToString() + ") t WHERE QUANTITY > 0 ORDER BY LINE";

            return sqlNew;
        }

        /// <summary>
        /// get data for VAB_Orderline with organization
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="VAB_OrderID"></param>
        /// <param name="orggetVals"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        public JsonResult GetOrderDataCommons(bool forInvoices, string isBaseLangs, int VAB_OrderID, int orggetVals, string langs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = OrderDataCommonsSqlQry(forInvoices, isBaseLangs, VAB_OrderID, orggetVals, langs);
            var stValue = obj.GetOrderDataCommons(ctx, sql);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Sql qry fro GetOrderDataCommons function
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="VAB_OrderID"></param>
        /// <param name="orggetVals"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        private string OrderDataCommonsSqlQry(bool forInvoices, string isBaseLangs, int VAB_OrderID, int orggetVals, string langs)
        {
            string
            sql = ("SELECT "
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
              + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
              + " COALESCE(l.VAM_Product_ID,0) as VAM_Product_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
              + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID ,"
              + " ins.description , "
                //+ " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW "								//	7..8
                + " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "								//	7..8 //Arpit on  20th Sept,2017
              + " , o.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
              + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID  AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                + " FROM VAB_OrderLine l"
               + " LEFT OUTER JOIN VAB_Order o ON (o.VAB_Order_ID = l.VAB_Order_ID)"
               + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID)"
               + " LEFT OUTER JOIN VAM_MatchPO m ON (l.VAB_OrderLine_ID=m.VAB_OrderLine_ID AND ");

            sql += (forInvoices ? "m.VAB_InvoiceLine_ID" : "m.VAM_Inv_InOutLine_ID");
            sql += " IS NOT NULL) LEFT OUTER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID)" + " LEFT OUTER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID)";

            if (isBaseLangs != "")
            {
                sql += " " + isBaseLangs;
            }

            sql += " LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) ";

            sql += " WHERE l.VAB_Order_ID=" + VAB_OrderID + " AND l.DTD001_Org_ID = " + orggetVals
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                    + "l.VAM_Product_ID,COALESCE(p.Name,c.Name),l.VAM_PFeature_SetInstance_ID , l.Line,l.VAB_OrderLine_ID, ins.description ,l.IsDropShip , o.VAB_PaymentTerm_ID , t.Name  " //Arpit on  20th Sept,2017
                                                                                                                                                                                    //+ "l.VAM_Product_ID,COALESCE(p.Name,c.Name),l.VAM_PFeature_SetInstance_ID , l.Line,l.VAB_OrderLine_ID, ins.description   "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") WHERE QUANTITY > 0";

            return sqlNew;
        }

        /// <summary>
        /// get data for VAB_Orderline without organization
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="VAB_OrderID"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        public JsonResult GetOrderDataCommonsNotOrg(bool forInvoices, string isBaseLangs, int VAB_OrderID, string langs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = GetOrderDataCommonsNotOrgSQL(forInvoices, isBaseLangs, VAB_OrderID, langs);
            var stValue = obj.GetOrderDataCommons(ctx, sql);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry GetOrderDataCommonsNotOrg
        /// </summary>
        /// <param name="forInvoices"></param>
        /// <param name="isBaseLangs"></param>
        /// <param name="VAB_OrderID"></param>
        /// <param name="langs"></param>
        /// <returns></returns>
        private string GetOrderDataCommonsNotOrgSQL(bool forInvoices, string isBaseLangs, int VAB_OrderID, string langs)
        {
            string
           sql = "SELECT "
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"
               + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"
               + " COALESCE(l.VAM_Product_ID,0) as VAM_Product_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"
               + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID ,"
               + " ins.description , "
               + " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW ,l.IsDropShip AS IsDropShip "//Arpit on  20th Sept,2017
                                                                                                                        //+ " l.VAB_OrderLine_ID as VAB_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "
               + " , o.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
               + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance "
                + " FROM VAB_OrderLine l"
                + " LEFT OUTER JOIN VAB_Order o ON (o.VAB_Order_ID = l.VAB_Order_ID)"
               + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID)"
                + " LEFT OUTER JOIN VAM_MatchPO m ON (l.VAB_OrderLine_ID=m.VAB_OrderLine_ID AND ";

            sql += (forInvoices ? "m.VAB_InvoiceLine_ID" : "m.VAM_Inv_InOutLine_ID");
            sql += " IS NOT NULL) LEFT OUTER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID)" + " LEFT OUTER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID)";

            if (isBaseLangs != "")
            {
                sql += " " + isBaseLangs;
            }

            sql += " LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) ";

            sql += " WHERE l.VAB_Order_ID=" + VAB_OrderID			//	#1
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                + "l.VAM_Product_ID,COALESCE(p.Name,c.Name), l.VAM_PFeature_SetInstance_ID, l.Line,l.VAB_OrderLine_ID, ins.description ,l.IsDropShip , o.VAB_PaymentTerm_ID , t.Name " //Arpit on  20th Sept,2017
                                                                                                                                                                               //+ "l.VAM_Product_ID,COALESCE(p.Name,c.Name), l.VAM_PFeature_SetInstance_ID, l.Line,l.VAB_OrderLine_ID, ins.description "
                + "ORDER BY l.Line";

            string sqlNew = "SELECT * FROM (" + sql + ") t WHERE QUANTITY > 0";

            return sqlNew;
        }

        public JsonResult GetOrderDataCommonsNotOrg(int VAM_Product_ID_Ks)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var stValue = obj.GetOrderDataCommonsNotOrg(ctx, VAM_Product_ID_Ks);
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
        /// get VAB_Order_id or VAB_Orderline_id
        /// </summary>
        /// <param name="keyColumnName"></param>
        /// <param name="tableName"></param>
        /// <param name="recordID"></param>
        /// <param name="pageNo"></param>
        /// <param name="MVAMInvInOutId"></param>
        /// <param name="isBaseLanguages"></param>
        /// <param name="MVAMProductIDD"></param>
        /// <returns></returns>
        public JsonResult GetDataVCreateFrom(string keyColumnName, string tableName, int recordID, int pageNo, string MVAMInvInOutId, string isBaseLanguages, string MVAMProductIDD)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var sql = GetDataSqlQueries(MVAMInvInOutId, isBaseLanguages, MVAMProductIDD);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetData function
        /// </summary>
        /// <param name="MVAMInvInOutId"></param>
        /// <param name="isBaseLanguages"></param>
        /// <param name="MVAMProductIDD"></param>
        /// <returns></returns>
        private string GetDataSqlQueries(string MVAMInvInOutId, string isBaseLanguages, string MVAMProductIDD)
        {
            var ctx = Session["ctx"] as Ctx;
            bool isAllownonItem = Util.GetValueOfString(ctx.GetContext("$AllowNonItem")).Equals("Y");
            string precision = "3";
            if (isBaseLanguages.ToUpper().Contains("VAB_UOM_TL"))
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
             + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
             + " l.VAM_Product_ID,p.Name as Product,p.Value as PRODUCTSEARCHKEY, l.VAM_Inv_InOutLine_ID,l.Line," // 5..8
             + " l.VAB_OrderLine_ID, " // 9
             + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
             + " ins.description , o.VAB_PaymentTerm_ID , pt.Name AS PaymentTermName "
             + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID  AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ";
            if (isBaseLanguages != "")
            {
                sql += isBaseLanguages + " ";
            }

            sql += "INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID) "
                + " LEFT JOIN VAB_OrderLine ol ON ol.VAB_OrderLine_ID = l.VAB_Orderline_id "
                + " LEFT JOIN VAB_Order o ON o.VAB_Order_id = ol.VAB_Order_id LEFT JOIN VAB_Paymentterm pt ON pt.VAB_Paymentterm_id = o.VAB_Paymentterm_id "
                //+ "LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) "
                + "LEFT JOIN (SELECT il.QtyInvoiced, il.VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice I ON I.VAB_INVOICE_ID = il.VAB_INVOICE_ID "
                + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON (l.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) "
                + "LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) "
                + "WHERE l.VAM_Inv_InOut_ID=" + MVAMInvInOutId; // #1
            if (MVAMProductIDD != "")
            {
                sql += MVAMProductIDD + " ";
            }
            sql += " GROUP BY l.MovementQty, l.QtyEntered," + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.VAM_Product_ID,p.Name,p.Value, l.VAM_Inv_InOutLine_ID,l.Line,l.VAB_OrderLine_ID,l.VAM_PFeature_SetInstance_ID,ins.description , o.VAB_PaymentTerm_ID , pt.Name ";

            if (isBaseLanguages.ToUpper().Contains("VAB_UOM_TL"))
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
              + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM," // 3..4
              + " 0 as VAM_Product_ID,c.Name as Product,c.Value as PRODUCTSEARCHKEY, l.VAM_Inv_InOutLine_ID,l.Line," // 5..8
              + " l.VAB_OrderLine_ID, " // 9
              + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
              + " ins.description , o.VAB_PaymentTerm_ID , pt.Name AS PaymentTermName "
              + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID  AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ";
                if (isBaseLanguages != "")
                {
                    sql += isBaseLanguages + " ";
                }

                sql += "INNER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID) "
                    + " LEFT JOIN VAB_OrderLine ol ON ol.VAB_OrderLine_ID = l.VAB_Orderline_id "
                    + " LEFT JOIN VAB_Order o ON o.VAB_Order_id = ol.VAB_Order_id LEFT JOIN VAB_Paymentterm pt ON pt.VAB_Paymentterm_id = o.VAB_Paymentterm_id "
                    //+ "LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) "
                    + "LEFT JOIN (SELECT il.QtyInvoiced, il.VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice I ON I.VAB_INVOICE_ID = il.VAB_INVOICE_ID "
                    + "WHERE i.DocStatus NOT IN ('VO','RE')) mi ON (l.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID) "
                    + "LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) "
                    + "WHERE l.VAM_Inv_InOut_ID=" + MVAMInvInOutId; // #1
                if (MVAMProductIDD != "")
                {
                    sql += MVAMProductIDD + " ";
                }
                sql += " GROUP BY l.MovementQty, l.QtyEntered," + "l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                    + "l.VAM_Product_ID,c.Name,c.Value, l.VAM_Inv_InOutLine_ID,l.Line,l.VAB_OrderLine_ID,l.VAM_PFeature_SetInstance_ID,ins.description , o.VAB_PaymentTerm_ID , pt.Name ";

                if (isBaseLanguages.ToUpper().Contains("VAB_UOM_TL"))
                {
                    sql += " ,uom1.stdprecision ";
                }
                else
                {
                    sql += " ,uom.stdprecision ";
                }
            }

            string sqlNew = "SELECT * FROM (" + sql.ToString() + ") t ORDER BY Line";

            return sqlNew;
        }

        /// <summary>
        /// Unused Function
        /// </summary>
        /// <param name="MVAMInvInOutIDs"></param>
        /// <param name="isBaseLanguageUmos"></param>
        /// <returns></returns>
        public JsonResult GetShipmentDatas(string MVAMInvInOutIDs, string isBaseLanguageUmos)
        {
            var ctx = Session["ctx"] as Ctx;
            VCreateFromModel obj = new VCreateFromModel();
            var stValue = obj.GetShipmentDatas(ctx, MVAMInvInOutIDs, isBaseLanguageUmos);
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
        /// <param name="MVAMProductIDs"></param>
        /// <returns></returns>
        public JsonResult GetInvoicesDataVCreate(string keyColumnName, string tableName, int recordID, int pageNo, string isBaseLangss, int cInvoiceID, string MVAMProductIDs)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            string sql = getSQlforGetInvoicesData(isBaseLangss, cInvoiceID, MVAMProductIDs);
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// create sql qry for GetInvoicesDataVCreate function
        /// </summary>
        /// <param name="isBaseLangss"></param>
        /// <param name="cInvoiceID"></param>
        /// <param name="MVAMProductIDs"></param>
        /// <returns></returns>
        private string getSQlforGetInvoicesData(string isBaseLangss, int cInvoiceID, string MVAMProductIDs)
        {
            #region[Commented By Sukhwinder on 17-Nov-2017 and updated the query below to prevent displaying product with "Cost adjustment on loss" with already MR.]
            //string sql = "SELECT "
            //            + "(l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            //            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ) as QUANTITY,"	//	2
            //            + "(l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            //            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ) as QTYENTER,"	//	2
            //            + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
            //            + " l.VAM_Product_ID,p.Name as PRODUCT, l.VAB_InvoiceLine_ID,l.Line,"      //  5..8
            //            + " l.VAB_OrderLine_ID,"                   					//  9
            //            + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
            //            + " ins.description ";
            //if (isBaseLangss)
            //{
            //    //sql += " " + isBaseLangss + " ";
            //    sql += "FROM VAB_UOM uom INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID) ";
            //}
            //else
            //{
            //    sql += "FROM VAB_UOM_TL uom INNER JOIN VAB_InvoiceLine l ON (l.VAB_UOM_ID=uom.VAB_UOM_ID AND uom.VAF_Language='" + Env.GetVAF_Language(ctx) + "') ";
            //}

            //sql += "INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID) "
            //    + "LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) "
            //    + "LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) "
            //    + "WHERE l.VAB_Invoice_ID=" + cInvoiceID;
            //if (MVAMProductIDs != "")
            //{
            //    sql += " " + MVAMProductIDs + " ";
            //}
            //sql += " GROUP BY l.QtyInvoiced,l.QtyEntered, l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
            //    + "l.VAM_Product_ID,p.Name, l.VAB_InvoiceLine_ID,l.Line,l.VAB_OrderLine_ID,l.VAM_PFeature_SetInstance_ID,ins.description "
            //+ "ORDER BY l.Line";

            //return sql;
            #endregion

            //Updated Query by Sukhwinder for "Cost adjustment on loss"

            string precision = "3";
            if (isBaseLangss.ToUpper().Contains("VAB_UOM_TL"))
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
                        + " l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//  3..4
                        + " l.VAM_Product_ID,p.Name as PRODUCT,p.Value as PRODUCTSEARCHKEY, l.VAB_InvoiceLine_ID,l.Line,"      //  5..8
                        + " l.VAB_OrderLine_ID,"                   					//  9
                        + " l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
                        + " ins.description, "
                        + " P.Iscostadjustmentonlost, "
                        + " Sum(Coalesce(Mi.Qty,0)) As Miqty, "
                        + " NVL(l.QtyInvoiced,0) as qtyInv , o.VAB_PaymentTerm_ID , pt.Name AS PaymentTermName "
                        + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' )
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =o.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ");

            if (isBaseLangss != "")
            {
                sql.Append(isBaseLangss);
            }

            if (isAllownonItem)
            {
                sql.Append(" LEFT JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID");
            }
            else
            {
                sql.Append(" INNER JOIN VAM_Product p ON (l.VAM_Product_ID=p.VAM_Product_ID");
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

            sql.Append(" LEFT JOIN VAB_Invoice o ON o.VAB_Invoice_ID = l.VAB_Invoice_ID LEFT JOIN VAB_PaymentTerm pt ON pt.VAB_PaymentTerm_ID = o.VAB_PaymentTerm_ID "
             + " LEFT OUTER JOIN VAM_MatchInvoice mi ON (l.VAB_InvoiceLine_ID=mi.VAB_InvoiceLine_ID) "
             + " "
             + " LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) "
             + " WHERE l.VAB_Invoice_ID=" + cInvoiceID + " AND l.VAM_Product_ID>0");

            if (MVAMProductIDs != "")
            {
                sql.Append(MVAMProductIDs);
            }
            sql.Append(" GROUP BY l.QtyInvoiced,l.QtyEntered, l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + " l.VAM_Product_ID,p.Name, p.Value, l.VAB_InvoiceLine_ID,l.Line,l.VAB_OrderLine_ID,l.VAM_PFeature_SetInstance_ID,ins.description, "
                + " p.IsCostAdjustmentOnLost, "
                + " L.Qtyinvoiced , o.VAB_PaymentTerm_ID , pt.Name ");

            if (isBaseLangss.ToUpper().Contains("VAB_UOM_TL"))
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
                  + " l.VAB_UOM_ID  as VAB_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
                  + " 0 as VAM_Product_ID, c.Name as PRODUCT,c.Value as PRODUCTSEARCHKEY, l.VAB_InvoiceLine_ID,l.Line,"	//	5..6
                  + " l.VAB_OrderLine_ID, l.VAM_PFeature_SetInstance_ID AS VAM_PFeature_SetInstance_ID,"
                  + " ins.description , "
                  + " 'N' AS Iscostadjustmentonlost, 0 As Miqty, 0 as qtyInv"			//	7..8 //              
                  + " , i.VAB_PaymentTerm_ID , t.Name AS PaymentTermName "
                  // JID_1414 - not to consider or pick In-Active Record
                  + @", (SELECT SUM( CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS isAdvance
                        FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' ) 
                        WHERE VAB_Paymentterm.VAB_Paymentterm_ID =i.VAB_PaymentTerm_ID AND VAB_PaymentTerm.IsActive = 'Y' ) AS IsAdvance ");

                if (isBaseLangss != "")
                {
                    sql.Append(isBaseLangss);
                }

                sql.Append(" LEFT OUTER JOIN VAB_Invoice i ON (i.VAB_Invoice_ID = l.VAB_Invoice_ID)"
                  + " LEFT OUTER JOIN VAB_PaymentTerm t ON (t.VAB_PaymentTerm_ID = i.VAB_PaymentTerm_ID)"
                  + " LEFT OUTER JOIN VAB_OrderLine m ON(m.VAB_OrderLine_ID = l.VAB_OrderLine_ID) AND m.VAB_OrderLine_ID IS NOT NULL LEFT OUTER JOIN VAB_Charge c ON (l.VAB_Charge_ID=c.VAB_Charge_ID)");



                sql.Append(" LEFT OUTER JOIN VAM_PFeature_SetInstance ins ON (ins.VAM_PFeature_SetInstance_ID =l.VAM_PFeature_SetInstance_ID) WHERE l.VAB_Invoice_ID=" + cInvoiceID + " AND C.VAB_Charge_ID>0 ");

                sql.Append(" GROUP BY l.QtyInvoiced, l.QtyEntered, l.VAB_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                      + "l.VAM_Product_ID,c.Name, c.Value, l.VAB_InvoiceLine_ID, l.VAM_PFeature_SetInstance_ID, l.Line,l.VAB_OrderLine_ID, ins.description, i.VAB_PaymentTerm_ID , t.Name");

                if (isBaseLangss.ToUpper().Contains("VAB_UOM_TL"))
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
            bool countVA034 = Env.IsModuleInstalled("VA034_"); //Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(VAF_ModuleInfo_ID) FROM VAF_ModuleInfo WHERE PREFIX='VA034_' AND IsActive = 'Y'"));
            StringBuilder sql = new StringBuilder();
            // JID_0084: Create line from is always picking curreny type that is default. It should pick currency type that is on payment.
            sql.Append("SELECT p.DateAcct AS DateTrx, p.VAB_Payment_ID, p.DocumentNo, ba.VAB_Currency_ID, c.ISO_Code, p.PayAmt,"
                // JID_0333: Currency conversion should be based on Payment Account Date and Currency type
                + " currencyConvert(p.PayAmt,p.VAB_Currency_ID,ba.VAB_Currency_ID,p.DateAcct,p.VAB_CurrencyType_ID,p.VAF_Client_ID,p.VAF_Org_ID) AS ConvertedAmt,"   //  #1
                + " pay.Description, bp.Name, 'P' AS Type");

            if (countVA034)
                sql.Append(", p.VA034_DepositSlipNo");

            sql.Append(", p.TrxNo, p.CheckNo  FROM VAB_Bank_Acct ba"
                + " INNER JOIN VAB_Payment_V p ON (p.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID)"
                + " INNER JOIN VAB_Payment pay ON p.VAB_Payment_ID=pay.VAB_Payment_ID"
                + " INNER JOIN VAB_Currency c ON (p.VAB_Currency_ID=c.VAB_Currency_ID)"
                + " LEFT OUTER JOIN VAB_BusinessPartner bp ON (p.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) "
                + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
                + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
                + " AND p.DateAcct <= " + dates      //JID_1293: it should show only those payment having trx date less than Bank Statment date 
                + " AND p.VAB_Bank_Acct_ID = " + cBankAccountId);                           	//  #2
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
            sql.Append(" AND NOT EXISTS (SELECT * FROM VAB_BankingJRNLLine l WHERE p.VAB_Payment_ID=l.VAB_Payment_ID AND l.StmtAmt <> 0)");         //	Voided Bank Statements have 0 StmtAmt

            bool countVA012 = Env.IsModuleInstalled("VA012_");    //Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(VAF_ModuleInfo_ID) FROM VAF_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
            if (countVA012)
            {
                // JID_0084: Create line from is always picking curreny type that is default. It should pick currency type that is on Cash Journal.
                sql.Append(" UNION SELECT cs.DateAcct AS DateTrx, cl.VAB_CashJRNLLine_ID AS VAB_Payment_ID, cs.DocumentNo, ba.VAB_Currency_ID, c.ISO_Code, (-1)*cl.Amount AS PayAmt,"
                + " currencyConvert((-1)*cl.Amount,cl.VAB_Currency_ID,ba.VAB_Currency_ID,cs.DateAcct,cl.VAB_CurrencyType_ID,cs.VAF_Client_ID,cs.VAF_Org_ID) AS ConvertedAmt,"   //  #1
                + " cl.Description, Null AS Name, 'C' AS Type");

                if (countVA034)
                    sql.Append(", NULL AS VA034_DepositSlipNo");

                sql.Append(", null AS TrxNo, null AS CheckNo FROM VAB_Bank_Acct ba"
                         + " INNER JOIN VAB_CashJRNLLine cl ON (cl.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID)"
                         + " INNER JOIN VAB_CashJRNL cs ON (cl.VAB_CashJRNL_ID=cs.VAB_CashJRNL_ID)"
                         + " INNER JOIN VAB_Charge chrg ON chrg.VAB_Charge_ID=cl.VAB_Charge_ID"
                         + " INNER JOIN VAB_Currency c ON (cl.VAB_Currency_ID=c.VAB_Currency_ID)"
                         + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
                         + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
                         + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
                         + " AND cs.DateAcct <= " + dates       // JID_1293: it should show only those Cash journal having statement date less than Bank Statment date 
                         + " AND cl.VAB_Bank_Acct_ID = " + cBankAccountId);                             	//  #2            
                if (DocumentNoUnions != "")
                {
                    sql.Append(DocumentNoUnions);
                }
                if (trxDatesUnions != "")
                {
                    sql.Append(trxDatesUnions);
                }
                sql.Append(" AND NOT EXISTS (SELECT * FROM VAB_BankingJRNLLine l WHERE cl.VAB_CashJRNLLine_ID=l.VAB_CashJRNLLine_ID AND l.StmtAmt <> 0)");      //	Voided Bank Statements have 0 StmtAmt
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
        /// VInOutGen get data from VAB_Order
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
        //public JsonResult GetContainer(int VAM_Locator_ID)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    VCreateFromModel obj = new VCreateFromModel();
        //    return Json(JsonConvert.SerializeObject(obj.GetContainer(ctx, VAM_Locator_ID)), JsonRequestBehavior.AllowGet);
        //}


    }
}