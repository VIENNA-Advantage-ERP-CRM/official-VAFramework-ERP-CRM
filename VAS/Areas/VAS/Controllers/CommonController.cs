using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VIS.Classes;
using VIS.DataContracts;
using VIS.Models;

namespace VIS.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /VIS/Common/
        public ActionResult Index()
        {
            return View();
        }

        #region Create Line From

        public JsonResult GetData(string sql, string keyColumnName, string tableName, int recordID, int pageNo)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var stValue = obj.GetData(sql, keyColumnName, tableName, recordID, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountData(string sql, int pageNo)
        {
            var ctx = Session["ctx"] as Ctx;
            CommonModel obj = new CommonModel();
            var stValue = obj.GetAccountData(sql, pageNo, ctx);
            return Json(JsonConvert.SerializeObject(stValue), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from shipment form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveShipment(List<Dictionary<string, string>> model, string selectedItems, string VAB_Order_ID, string VAB_Invoice_ID, string VAM_Locator_id, string VAM_Inv_InOut_id, string Container_ID)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveShipmentData(ctx, model, selectedItems, Convert.ToInt32(VAB_Order_ID), Convert.ToInt32(VAB_Invoice_ID), Convert.ToInt32(VAM_Locator_id), Convert.ToInt32(VAM_Inv_InOut_id), Util.GetValueOfInt(Container_ID));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from shipment form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveInvoice(List<Dictionary<string, string>> model, string selectedItems, string VAB_Order_ID, string VAB_Invoice_ID, string VAM_Inv_InOut_id)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveInvoiceData(ctx, model, selectedItems, Convert.ToInt32(VAB_Order_ID), Convert.ToInt32(VAB_Invoice_ID), Convert.ToInt32(VAM_Inv_InOut_id));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from BankStatement form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveStatment(List<Dictionary<string, string>> model, string selectedItems, string VAB_BankingJRNL_ID)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveStatmentData(ctx, model, selectedItems, Convert.ToInt32(VAB_BankingJRNL_ID));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Menual Forms

        public JsonResult GenerateInvoices(string whereClause)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GenerateInvoices(ctx, whereClause);
                return Json(new { obj.ErrorMsg, obj.lblStatusInfo, obj.statusBar, obj.DocumentText }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateShipments(string whereClause, string VAM_Warehouse_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GenerateShipments(ctx, whereClause, VAM_Warehouse_ID);
                return Json(new { obj.ErrorMsg, obj.lblStatusInfo, obj.statusBar, obj.DocumentText }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Match PO


        public JsonResult Consolidate()
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                MMatchPO.Consolidate(ctx);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create Matched Purchase Order or Matched Invoice records by Matching PO - Invoices form
        /// </summary>
        /// <param name="invoice">Matching Invoice</param>
        /// /// <param name="MatchedMode">Mode of Matching</param>
        /// /// <param name="MatchFrom">Matched From</param>
        /// /// <param name="LineMatched">Line which is Matching</param>
        /// /// <param name="ToMatchQty">Qty need to be Matched</param>
        /// /// <param name="SelectedItems">Lines to which Matching is done</param>
        /// <returns>Matched PO No / Matched Invoice No</returns>
        public JsonResult createMatchRecord(bool invoice, string MatchMode, string MatchFrom, string LineMatched, string ToMatchQty, string SelectedItems) //List<GetTableLoadVmatch>
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                List<GetTableLoadVmatch> data = JsonConvert.DeserializeObject<List<GetTableLoadVmatch>>(SelectedItems);
                CommonModel obj = new CommonModel();
                var value = obj.CreateMatchRecord(ctx, invoice, Convert.ToInt32(MatchMode), Convert.ToInt32(MatchFrom), Convert.ToInt32(LineMatched), Convert.ToDecimal(ToMatchQty), data);
                return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Account Viewer

        public JsonResult GetDataQuery(int VAF_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4,
            String sort1, String sort2, String sort3, String sort4, bool displayDocInfo, bool displaySrcAmt, bool displayqty)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GetDataQuery(ctx, VAF_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4, sort1, sort2, sort3, sort4, displayDocInfo, displaySrcAmt, displayqty);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Archive Viewer

        public JsonResult UpdateArchive(string name, string des, string help, int archiveId)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.UpdateArchive(ctx, name, des, help, archiveId);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadPdf(int archiveId)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var base64EncodedPDF = obj.DownloadPdf(ctx, archiveId);

                return Json(new { result = base64EncodedPDF }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 07 June 2017
        //public JsonResult GetProcess(int Role_ID)
        //{
        //    var ctx = Session["ctx"] as Ctx;
        //    CommonModel obj = new CommonModel();
        //    return Json(JsonConvert.SerializeObject(obj.GetProcess(Role_ID)), JsonRequestBehavior.AllowGet);
        //}

        // Added by Bharat on 07 June 2017
        //public JsonResult GetTable(int Role_ID)
        //{
        //    var ctx = Session["ctx"] as Ctx;
        //    CommonModel obj = new CommonModel();
        //    return Json(JsonConvert.SerializeObject(obj.GetTable(Role_ID)), JsonRequestBehavior.AllowGet);
        //}

        // Added by Bharat on 07 June 2017
        //public JsonResult GetUser()
        //{
        //    var ctx = Session["ctx"] as Ctx;
        //    CommonModel obj = new CommonModel();
        //    return Json(JsonConvert.SerializeObject(obj.GetUser(ctx)), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetArchiveData(string ArchiveQry)
        //{
        //    var ctx = Session["ctx"] as Ctx;
        //    CommonModel obj = new CommonModel();
        //    ArchiveQry = SecureEngineBridge.DecryptByClientKey(ArchiveQry, ctx.GetSecureKey());
        //    return Json(JsonConvert.SerializeObject(obj.GetArchiveData(ArchiveQry)), JsonRequestBehavior.AllowGet);
        //}


        #endregion



        #region VAttributeGrid
        public JsonResult GetDataQueryAttribute()
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                List<AttributeGrid> lst = new List<AttributeGrid>();
                var value = MVAMProductFeature.GetOfClient(ctx, true, true);

                for (int i = 0; i < value.Length; i++)
                {
                    AttributeGrid lstObj = new AttributeGrid();
                    lstObj.GetKeyNamePair = value[i].GetKeyNamePair();
                    List<MAttributeValueList> lstAttValueObj = new List<MAttributeValueList>();
                    for (int k = 0; k < value[i].GetMAttributeValues().Length; k++)
                    {
                        var attrValue = value[i].GetMAttributeValues()[k];
                        if (attrValue != null)
                        {
                            MAttributeValueList localObj = new MAttributeValueList();
                            localObj.GetVAM_ProductFeature_ID = attrValue.GetVAM_ProductFeature_ID();
                            localObj.GetVAM_PFeature_Value_ID = attrValue.GetVAM_PFeature_Value_ID();
                            localObj.Name = attrValue.GetName();
                            lstAttValueObj.Add(localObj);
                        }
                    }

                    lstObj.GetMAttributeValues = lstAttValueObj.ToArray();
                    lstObj.GetName = value[i].GetName();
                    lst.Add(lstObj);
                }

                CommonModel obj = new CommonModel();
                List<KeyNamePair> priceList = new List<KeyNamePair>();
                List<KeyNamePair> whList = new List<KeyNamePair>();
                obj.FillPicks(ctx, out priceList, out whList);
                return Json(new { lst, priceList, whList }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGridElement(int xVAM_ProductFeature_ID, int xVAM_PFeature_Value_ID, int yVAM_ProductFeature_ID, int yVAM_PFeature_Value_ID, int VAM_PriceListVersion_ID, int VAM_Warehouse_ID, string windowNo)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var stValue = obj.GetGridElement(ctx, xVAM_ProductFeature_ID, xVAM_PFeature_Value_ID, yVAM_ProductFeature_ID, yVAM_PFeature_Value_ID, VAM_PriceListVersion_ID, VAM_Warehouse_ID, windowNo);
                return Json(new { stValue }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        /// Added by Sukhwinder on 05/Dec/2017
        /// <summary>
        /// Function to get Period dates from year ID and Client ID.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public JsonResult GetPeriodFromYear(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;
                CommonModel objCommonModel = new CommonModel();
                retJSON = JsonConvert.SerializeObject(objCommonModel.GetPeriodFromYear(ctx, fields));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
        //


        /// Added by Sukhwinder on 05/Dec/2017
        /// <summary>
        /// Function to get Period dates from year ID and Client ID.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public JsonResult GetCurrencyFroMVABAccountingSchema(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;
                CommonModel objCommonModel = new CommonModel();
                retJSON = JsonConvert.SerializeObject(objCommonModel.GetCurrencyFroMVABAccountingSchema(ctx, fields));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to get data based on the query generated for table
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public JsonResult GetIDTextData(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;

                // get parameters and split with comma
                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;
                // get table name from first index
                string TableName = Util.GetValueOfString(paramValue[0]);

                // check if second parameter is true for Version Table
                if (Util.GetValueOfBool(paramValue[1]))
                    TableName = TableName + "_Ver";

                int VAF_TableView_ID = MVAFTableView.Get_Table_ID(TableName);

                CommonModel objCommonModel = new CommonModel();

                POInfo inf = POInfo.GetPOInfo(ctx, VAF_TableView_ID);
                // Get SQL Query from PO Info for selected table
                string sqlCol = objCommonModel.GetSQLQuery(ctx, VAF_TableView_ID, inf.GetPoInfoColumns());

                // Append where Clause, passed in the parameter
                string whClause = Util.GetValueOfString(paramValue[2]);
                if (whClause != "")
                    sqlCol += " WHERE " + whClause;

                // Apply Role check
                if (sqlCol.Trim() != "")
                    sqlCol = MVAFRole.GetDefault(ctx).AddAccessSQL(sqlCol, TableName, MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);

                // if SQL is being generated for Version table
                if (Util.GetValueOfBool(paramValue[1]))
                {
                    if (sqlCol.Contains("WHERE"))
                    {
                        sqlCol += " AND " + TableName + ".ProcessedVersion = 'N' ";
                    }
                    sqlCol += " ORDER BY " + TableName + ".VERSIONVALIDFROM DESC, " + TableName + ".RecordVersion DESC";
                }
               
                retJSON = JsonConvert.SerializeObject(objCommonModel.GetIDTextData(ctx, sqlCol));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckTableDeletable(string fields)
        {
            string retJSON = "";
            if (Session["Ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;

                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;
                string TableName = Util.GetValueOfString(paramValue[0]);

                CommonModel objCommonModel = new CommonModel();
                retJSON = JsonConvert.SerializeObject(objCommonModel.CheckTableDeletable(ctx, TableName));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRecord(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;

                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;
                string TableName = Util.GetValueOfString(paramValue[0]);
                int Record_ID = Util.GetValueOfInt(paramValue[1]);

                CommonModel objCommonModel = new CommonModel();
                retJSON = JsonConvert.SerializeObject(objCommonModel.DeleteRecord(ctx, TableName, Record_ID));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckVersions(SaveRecordIn RowData)
        {
            bool hasRecords = false;
            if (Session["Ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                CommonModel cmm = new CommonModel();
                hasRecords = cmm.HasVersions(ctx, RowData);
            }
            return Json(new { result = hasRecords }, JsonRequestBehavior.AllowGet);
        }
    }

    public class AttributeGrid
    {
        public KeyNamePair GetKeyNamePair { get; set; }
        public MAttributeValueList[] GetMAttributeValues { get; set; }
        public String GetName { get; set; }
    }

    public class MAttributeValueList
    {
        public int GetVAM_ProductFeature_ID { get; set; }
        public int GetVAM_PFeature_Value_ID { get; set; }
        public string Name { get; set; }
    }

    public class CommonModel
    {
        #region VAttributeGrid

        public string GetGridElement(Ctx ctx, int xVAM_ProductFeature_ID, int xVAM_PFeature_Value_ID, int yVAM_ProductFeature_ID, int yVAM_PFeature_Value_ID, int VAM_PriceListVersion_ID, int VAM_Warehouse_ID, string windowNo)
        {
            StringBuilder panel = new StringBuilder();
            String sql = "SELECT * FROM VAM_Product WHERE IsActive='Y'";
            //	Product Attributes
            if (xVAM_ProductFeature_ID > 0)
            {
                sql += " AND VAM_PFeature_SetInstance_ID IN "
                    + "(SELECT VAM_PFeature_SetInstance_ID "
                    + "FROM VAM_PFeatue_Instance "
                    + "WHERE VAM_ProductFeature_ID=" + xVAM_ProductFeature_ID
                    + " AND VAM_PFeature_Value_ID=" + xVAM_PFeature_Value_ID + ")";
            }
            if (yVAM_ProductFeature_ID > 0)
            {
                sql += " AND VAM_PFeature_SetInstance_ID IN "
                    + "(SELECT VAM_PFeature_SetInstance_ID "
                    + "FROM VAM_PFeatue_Instance "
                    + "WHERE VAM_ProductFeature_ID=" + yVAM_ProductFeature_ID
                    + " AND VAM_PFeature_Value_ID=" + yVAM_PFeature_Value_ID + ")";
            }
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAM_Product", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);
            DataTable dt = null;
            IDataReader idr = null;
            int noProducts = 0;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                idr = null;
                foreach (DataRow dr in dt.Rows)
                {
                    MProduct product = new MProduct(Env.GetContext(), dr, null);
                    panel.Append(AddProduct(product, VAM_PriceListVersion_ID, VAM_Warehouse_ID, windowNo));
                    noProducts++;
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                idr = null;
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            return panel.ToString();
        }

        private string AddProduct(MProduct product, int VAM_PriceListVersion_ID, int VAM_Warehouse_ID, string windowNo)
        {
            int VAM_Product_ID = product.GetVAM_Product_ID();
            StringBuilder obj = new StringBuilder();
            obj.Append("<table style='width: 100%;'><tr>");

            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblproductVal_" + windowNo + "' class='VIS_Pref_Label_Font'>" + product.GetValue() + "</label>");
            obj.Append("</td>");

            String formatted = "";
            if (VAM_PriceListVersion_ID != 0)
            {
                MProductPrice pp = MProductPrice.Get(Env.GetContext(), VAM_PriceListVersion_ID, VAM_Product_ID, null);
                if (pp != null)
                {
                    Decimal price = pp.GetPriceStd();
                    formatted = price.ToString();// _price.format(price);
                }
                else
                {
                    formatted = "-";
                }
            }

            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblformate_" + windowNo + "' class='VIS_Pref_Label_Font'>" + formatted.ToString() + "</label>");
            obj.Append("</td>");


            obj.Append("</tr>");
            obj.Append("<tr>");

            //	Product Name - Qty
            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblProductName_" + windowNo + "' class='VIS_Pref_Label_Font'>" + product.GetName() + "</label>");
            obj.Append("</td>");

            formatted = "";
            if (VAM_Warehouse_ID != 0)
            {
                Decimal qty = Util.GetValueOfDecimal(MStorage.GetQtyAvailable(VAM_Warehouse_ID, VAM_Product_ID, 0, null));
                if (qty == null)
                {
                    formatted = "-";
                }
                else
                {
                    formatted = qty.ToString();
                }
            }

            obj.Append("</tr>");
            obj.Append("<tr>");

            //	Product Name - Qty
            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblfomatepanel_" + windowNo + "' class='VIS_Pref_Label_Font'>" + formatted.ToString() + "</label>");
            obj.Append("</td>");

            obj.Append("</tr>");
            obj.Append("</table>");

            return obj.ToString();
        }

        public void FillPicks(Ctx ctx, out List<KeyNamePair> priceList, out List<KeyNamePair> whList)
        {
            priceList = new List<KeyNamePair>();
            whList = new List<KeyNamePair>();
            //	Price List
            String sql = "SELECT VAM_PriceListVersion.VAM_PriceListVersion_ID,"
                + " VAM_PriceListVersion.Name || ' (' || c.Iso_Code || ')' AS ValueName "
                + "FROM VAM_PriceListVersion, VAM_PriceList pl, VAB_Currency c "
                + "WHERE VAM_PriceListVersion.VAM_PriceList_ID=pl.VAM_PriceList_ID"
                + " AND pl.VAB_Currency_ID=c.VAB_Currency_ID"
                + " AND VAM_PriceListVersion.IsActive='Y' AND pl.IsActive='Y'";
            //	Add Access & Order
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAM_PriceListVersion", true, false)	// fully qualidfied - RO 
                + " ORDER BY VAM_PriceListVersion.Name";
            System.Data.IDataReader idr = null;
            try
            {
                priceList.Add(new KeyNamePair(0, ""));
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    KeyNamePair kn = new KeyNamePair(Util.GetValueOfInt(idr[0]), idr[1].ToString());
                    priceList.Add(kn);
                }
                idr.Close();
                //	Warehouse
                sql = "SELECT VAM_Warehouse_ID, Value || ' - ' || Name AS ValueName "
                    + "FROM VAM_Warehouse "
                    + "WHERE IsActive='Y'";
                sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql,
                        "VAM_Warehouse", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO)
                    + " ORDER BY Value";
                whList.Add(new KeyNamePair(0, ""));
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    KeyNamePair kn = new KeyNamePair(Util.GetValueOfInt(idr["VAM_Warehouse_ID"]), idr["ValueName"].ToString());
                    whList.Add(kn);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
            }
        }

        #endregion


        #region Create Line From
        /// <summary>
        /// Is used to get data based on sql query
        /// </summary>
        /// <param name="sql">query</param>
        /// <param name="keyColumnName">Header Table Name</param>
        /// <param name="tableName">pick record from the respective table</param>
        /// <param name="recordID">Record ID (Header Table ID)</param>
        /// <param name="pageNo">page no</param>
        /// <param name="ctx">context</param>
        /// <returns>class object of LinesData</returns>
        public LinesData GetData(string sql, string keyColumnName, string tableName, int recordID, int pageNo, VAdvantage.Utility.Ctx ctx)
        {
            LinesData _iData = new LinesData();
            try
            {
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sql + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                DataSet data = DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (data == null)
                {
                    return null;
                }
                List<DataObject> dyndata = new List<DataObject>();
                DataObject item = null;
                List<object> values = null;
                int recid = 0;

                //
                int VAB_InvoicePaymentTerm_ID = 0; // invoice header payment term
                bool IsInvoicePTAdvance = false; // payment term binded on Invoice is advance or not

                // get invoice header payment tern and is advance or not
                if (keyColumnName == "VAB_Invoice_ID")
                {
                    DataSet ds = DB.ExecuteDataset(@"SELECT VAB_Paymentterm.VAB_Paymentterm_ID,
                                    SUM(  CASE WHEN VAB_Paymentterm.VA009_Advance!= COALESCE(VAB_PaymentSchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS IsAdvance
                                    FROM VAB_Paymentterm LEFT JOIN VAB_PaymentSchedule ON ( VAB_Paymentterm.VAB_Paymentterm_ID = VAB_PaymentSchedule.VAB_Paymentterm_ID AND VAB_PaymentSchedule.IsActive ='Y' )
                                    WHERE VAB_Paymentterm.VAB_Paymentterm_ID = (SELECT VAB_Paymentterm_ID FROM VAB_Invoice WHERE VAB_Invoice_ID = " + recordID + @" )
                                    GROUP BY VAB_Paymentterm.VAB_Paymentterm_ID ");
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        VAB_InvoicePaymentTerm_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["VAB_Paymentterm_ID"]);
                        IsInvoicePTAdvance = Convert.ToInt32(ds.Tables[0].Rows[0]["IsAdvance"]) > 0 ? true : false;
                    }
                }

                for (int i = 0; i < data.Tables[0].Rows.Count; i++)  //columns
                {
                    int recordid = 0;
                    int rec = 0;
                    decimal SavedQty = 0;
                    bool select = false;
                    string qry = "";

                    recid += 1;
                    item = new DataObject();
                    item.recid = recid;
                    item.Quantity = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["quantity"]);
                    item.QuantityEntered = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["qtyenter"]);
                    item.VAB_UOM_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["uom"]);
                    item.VAM_Product_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["product"]);
                    item.VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                    //Product search key added
                    item.VAM_Product_SearchKey = Util.GetValueOfString(data.Tables[0].Rows[i]["productsearchkey"]);

                    //
                    if (data.Tables[0].Columns.Contains("VAB_PaymentTerm_ID"))
                    {
                        if (data.Tables[0].Rows[i]["VAB_PaymentTerm_ID"] != DBNull.Value)
                        {
                            item.VAB_PaymentTerm_ID = Convert.ToInt32(data.Tables[0].Rows[i]["VAB_PaymentTerm_ID"]);
                            item.PaymentTermName = Convert.ToString(data.Tables[0].Rows[i]["PaymentTermName"]);
                            item.IsAdvance = Convert.ToInt32(data.Tables[0].Rows[i]["IsAdvance"]) > 0 ? true : false;
                        }
                    }
                    // 
                    if (keyColumnName == "VAB_Invoice_ID")
                    {
                        item.VAB_InvoicePaymentTerm_ID = VAB_InvoicePaymentTerm_ID;
                        item.IsInvoicePTAdvance = IsInvoicePTAdvance;
                    }

                    //Arpit Rai 20 Sept,2017
                    //item.IsDropShip = Util.GetValueOfString(data.Tables[0].Rows[i]["isdropship"]);

                    if (tableName == "VAB_OrderLine")
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);
                        if (keyColumnName == "VAM_Inv_InOut_ID")
                        {
                            if (recordid > 0)
                            {
                                qry = "SELECT SUM(QtyEntered) FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + recordID + " AND VAB_OrderLine_ID = " + recordid;
                                rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (rec > 0)
                                {
                                    // Change By Mohit 30/06/2016
                                    select = true;
                                    item.QuantityEntered -= rec;

                                }
                            }
                            else
                            {
                                qry = "SELECT QtyEntered FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + recordID + " AND VAM_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Product_id"]) +
                                    " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                                rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (rec > 0)
                                {
                                    // Change By Mohit 30/06/2016
                                    select = true;
                                    item.QuantityEntered = rec;
                                }
                            }
                        }
                        else if (keyColumnName == "VAB_Invoice_ID")
                        {
                            recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);

                            if (recordid > 0)
                            {
                                qry = "SELECT SUM(QtyEntered) FROM VAB_InvoiceLine WHERE VAB_Invoice_ID = " + recordID + " AND VAB_OrderLine_ID = " + recordid;
                                rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (rec > 0)
                                {
                                    // Change By Mohit 30/06/2016
                                    select = true;
                                    item.QuantityEntered -= rec;
                                }
                            }
                            else
                            {
                                qry = "SELECT QtyEntered FROM VAB_InvoiceLine WHERE VAB_Invoice_ID = " + recordID + " AND VAM_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Product_id"]) +
                                    " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                                rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (rec > 0)
                                {
                                    // Change By Mohit 30/06/2016
                                    select = true;
                                    item.QuantityEntered = rec;
                                }
                            }
                        }
                        item.Select = select;
                        item.VAB_Order_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.VAM_Inv_InOut_ID = null;
                        item.VAB_Invoice_ID = null;
                        item.VAB_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);
                        item.VAM_Inv_InOut_ID_K = 0;
                        item.VAB_Invoice_ID_K = 0;
                    }
                    else if (tableName == "VAB_InvoiceLine")
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);
                        if (recordid > 0)
                        {
                            qry = "SELECT QtyEntered FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + recordID + " AND VAB_OrderLine_ID = " + recordid;
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        else
                        {
                            qry = "SELECT QtyEntered FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + recordID + " AND VAM_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Product_id"]) +
                                " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        item.Select = select;
                        item.VAB_Order_ID = null;
                        item.VAM_Inv_InOut_ID = null;
                        item.VAB_Invoice_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.VAB_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);
                        item.VAM_Inv_InOut_ID_K = 0;
                        item.VAB_Invoice_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_InvoiceLine_id"]);
                    }
                    else
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Inv_InOutLine_id"]);
                        if (recordid > 0)
                        {
                            qry = "SELECT QtyEntered FROM VAB_InvoiceLine WHERE VAB_Invoice_ID = " + recordID + " AND VAM_Inv_InOutLine_ID = " + recordid;
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        else
                        {
                            qry = "SELECT QtyEntered FROM VAB_InvoiceLine WHERE VAB_Invoice_ID = " + recordID + " AND VAM_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Product_id"]) +
                                " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        item.Select = select;
                        item.VAB_Order_ID = null;
                        item.VAM_Inv_InOut_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.VAB_Invoice_ID = null;
                        item.VAB_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Orderline_id"]);
                        item.VAM_Inv_InOut_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Inv_InOutLine_id"]);
                        item.VAB_Invoice_ID_K = 0;
                    }

                    item.QuantityPending = item.QuantityEntered;
                    item.VAB_UOM_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_UOM_id"]);
                    item.VAM_Product_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAM_Product_id"]);

                    values = new List<object>();
                    dyndata.Add(item);
                }
                _iData.data = dyndata;
                return _iData;
            }
            catch (Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }

        public AccountData GetAccountData(string sql, int pageNo, VAdvantage.Utility.Ctx ctx)
        {
            AccountData _iData = new AccountData();
            try
            {
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sql + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                DataSet data = DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (data == null)
                {
                    return null;
                }
                List<AccountObject> dyndata = new List<AccountObject>();
                AccountObject item = null;
                int recid = 0;
                string desc = "";
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)  //columns
                {
                    //int recordid = 0;
                    //int rec = 0;
                    //bool select = false;
                    //string qry = "";
                    recid += 1;
                    item = new AccountObject();
                    item.recid = recid;
                    //item.Select = false;
                    item.Date = Util.GetValueOfString(data.Tables[0].Rows[i]["DateTrx"]);
                    item.VAB_Payment_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["DocumentNo"]);
                    item.VAB_Currency_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["ISO_Code"]);
                    item.Amount = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["PayAmt"]);
                    item.ConvertedAmount = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["ConvertedAmt"]);

                    desc = Util.GetValueOfString(data.Tables[0].Rows[i]["Description"]);
                    if (desc != null && desc.Length > 50)
                    {
                        desc.Substring(0, 50);
                    }
                    item.Description = desc;
                    item.VAB_BusinessPartner_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["Name"]);
                    item.Type = Util.GetValueOfString(data.Tables[0].Rows[i]["Type"]);
                    item.VAB_Payment_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Payment_ID"]);
                    item.VAB_Currency_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAB_Currency_ID"]);


                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VA034_' AND IsActive='Y'"));
                    if (count > 0)
                        item.VA034_DepositSlipNo = Util.GetValueOfString(data.Tables[0].Rows[i]["VA034_DepositSlipNo"]);
                    else
                        item.VA034_DepositSlipNo = "";
                    // Change By Mohit To show trx no on create line from on bank statement
                    item.AuthCode = Util.GetValueOfString(data.Tables[0].Rows[i]["TrxNo"]);
                    item.CheckNo = Util.GetValueOfString(data.Tables[0].Rows[i]["CheckNo"]);
                    dyndata.Add(item);
                }
                _iData.data = dyndata;
                return _iData;
            }
            catch (Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }



        public bool SaveShipmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int VAB_Order_ID, int VAB_Invoice_ID, int VAM_Locator_ID, int VAM_Inv_InOut_ID, int Container_ID)
        {
            // chck pallet Functionality applicable or not
            bool isContainerApplicable = MTransaction.ProductContainerApplicable(ctx);

            MOrder _order = null;
            if (VAB_Order_ID > 0)
            {
                _order = new MOrder(ctx, VAB_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (VAB_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, VAB_Invoice_ID, null);
            }


            MInOut inout = null;
            if (VAM_Inv_InOut_ID > 0)
            {
                inout = new MInOut(ctx, VAM_Inv_InOut_ID, null);
            }
            // Added By Bharat for ViennaAdvantage Compatiability. Code not called from ViennaAdvantage.
            MVAFTableView tbl = null;
            PO po = null;
            tbl = new MVAFTableView(ctx, 320, null);
            /**
             *  Selected        - 0
             *  QtyEntered      - 1
             *  VAB_UOM_ID        - 2
             *  VAM_Product_ID    - 3
             *  OrderLine       - 4
             *  ShipmentLine    - 5
             *  InvoiceLine     - 6
             */
            int countVA010 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(VAF_ModuleInfo_ID) FROM VAF_ModuleInfo WHERE PREFIX='VA010_' AND IsActive = 'Y'"));
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int VAB_UOM_ID = 0;
                int VAM_Product_ID = 0;
                int VAB_OrderLine_ID = 0;
                int VAB_InvoiceLine_ID = 0;
                int VAM_PFeature_SetInstance_ID = 0;
                int VAM_Inv_InOutLine_ID = 0;
                string SqlIOL = "";
                //Arpit
                String IsDropShip = "N";
                int QualityPlan_ID = 0;
                //
                Double d = 0;
                if (Util.GetValueOfBool(model[i]["Select"]) == true)
                {
                    // Change By Mohit 30/06/2016
                    VAM_Inv_InOutLine_ID = 0;
                    if (model[i].Keys.Contains("QuantityEntered"))
                    {
                        d = Convert.ToDouble(model[i]["QuantityEntered"]);
                    }
                    else if (model[i].Keys.Contains("Quantity"))
                    {
                        d = Convert.ToDouble(model[i]["Quantity"]);
                    }
                    Decimal QtyEnt = Convert.ToDecimal(d);
                    int precsn = 2;
                    if (VAM_Product_ID != 0)
                    {
                        MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                        precsn = product.GetUOMPrecision();
                    }
                    QtyEnt = Decimal.Round(QtyEnt, precsn, MidpointRounding.AwayFromZero);
                    // when qty is ZERO -- thn not to update / create (Pallet task list point no : PT-114)
                    if (QtyEnt == 0)
                        continue;

                    if (model[i]["VAM_Product_ID_K"] != "")
                    {
                        VAM_Product_ID = Convert.ToInt32((model[i]["VAM_Product_ID_K"]));       //  3-Product
                        //Arpit to Set Quality Plan if existed the module 
                        if (countVA010 > 0)
                        {
                            MProduct Product_ = MProduct.Get(ctx, VAM_Product_ID);
                            QualityPlan_ID = Util.GetValueOfInt(Product_.Get_Value("VA010_QualityPlan_ID"));
                        }
                    }
                    if (model[i]["VAB_Order_ID_K"] != "")
                        VAB_OrderLine_ID = Convert.ToInt32((model[i]["VAB_Order_ID_K"]));       //  4-OrderLine
                    if (model[i]["VAB_Invoice_ID_K"] != "")
                        VAB_InvoiceLine_ID = Convert.ToInt32((model[i]["VAB_Invoice_ID_K"]));   //  6-InvoiceLine

                    if (model[i].Keys.Contains("VAM_PFeature_SetInstance_ID"))
                    {
                        if (model[i]["VAM_PFeature_SetInstance_ID"] != "")
                            VAM_PFeature_SetInstance_ID = Convert.ToInt32((model[i]["VAM_PFeature_SetInstance_ID"]));   //  6-InvoiceLine
                    }
                    if (VAB_OrderLine_ID > 0)
                    {
                        // PO create with Lot enable product, but Lot/Attributesetinstance not define on PO.
                        // when we create MR against PO, with the same product having diffrent LOT/Attribute then system updating qty - not creating new MR line with different lot
                        // In this case system shold create new MR line with same product / same orderline but diffrent Attribute

                        //VAM_Inv_InOutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Inv_InOutLine_ID from VAM_Inv_InOutLine where VAM_Inv_InOut_ID=" + VAM_Inv_InOut_ID + " AND VAB_Orderline_id=" + VAB_OrderLine_ID));
                        if (VAM_Inv_InOutLine_ID == 0)
                        {
                            SqlIOL = "SELECT VAM_Inv_InOutLine_ID FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + VAM_Inv_InOut_ID + " AND VAM_Product_ID = " + VAM_Product_ID + " AND VAB_Orderline_id=" + VAB_OrderLine_ID;
                            //if (VAM_PFeature_SetInstance_ID > 0)
                            //{ JID_1096: Also check Locator while finding the line on Material receipt
                            SqlIOL += " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + VAM_PFeature_SetInstance_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID;
                            //}
                            if (isContainerApplicable)
                            {
                                // To check with containerID to get Record
                                SqlIOL += " AND NVL(VAM_ProductContainer_ID , 0) = " + Container_ID;
                            }

                            VAM_Inv_InOutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                            if (VAM_Inv_InOutLine_ID == 0)
                                goto newRecord;
                        }
                        po = tbl.GetPO(ctx, VAM_Inv_InOutLine_ID, null);
                        if (countVA010 > 0 && QualityPlan_ID > 0)
                            po.Set_ValueNoCheck("VA010_QualityPlan_ID", QualityPlan_ID);

                        po.Set_Value("QtyEntered", (Decimal?)QtyEnt);
                        po.Set_Value("MovementQty", (Decimal?)QtyEnt);
                        if (!po.Save())
                        {

                        }
                    }
                    if (VAB_InvoiceLine_ID > 0)
                    {
                        //VAM_Inv_InOutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine WHERE VAB_InvoiceLine_ID=" + VAB_InvoiceLine_ID));
                        SqlIOL = "SELECT VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine WHERE VAB_InvoiceLine_ID = " + VAB_InvoiceLine_ID + " AND VAM_Product_ID = " + VAM_Product_ID;
                        //if (VAM_PFeature_SetInstance_ID > 0)
                        //{ 
                        SqlIOL += " AND NVL(VAM_PFeature_SetInstance_ID, 0) = " + VAM_PFeature_SetInstance_ID;
                        //}
                        if (isContainerApplicable)
                        {
                            // To check with containerID to get Record
                            SqlIOL += " AND NVL(VAM_ProductContainer_ID , 0) = " + Container_ID;
                        }

                        VAM_Inv_InOutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                        if (VAM_Inv_InOutLine_ID == 0)
                        {
                            SqlIOL = "SELECT VAM_Inv_InOutLine_ID FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + VAM_Inv_InOut_ID + " AND VAM_Product_ID = " + VAM_Product_ID;
                            //if (VAM_PFeature_SetInstance_ID > 0)
                            //{ JID_1096: Also check Locator while finding the line on Material receipt
                            SqlIOL += " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID;
                            //}
                            if (isContainerApplicable)
                            {
                                // To check with containerID to get Record
                                SqlIOL += " AND NVL(VAM_ProductContainer_ID , 0) = " + Container_ID;
                            }

                            VAM_Inv_InOutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                            if (VAM_Inv_InOutLine_ID == 0)
                                goto newRecord;
                        }
                        po = tbl.GetPO(ctx, VAM_Inv_InOutLine_ID, null);
                        if (countVA010 > 0 && QualityPlan_ID > 0)
                            po.Set_ValueNoCheck("VA010_QualityPlan_ID", QualityPlan_ID);
                        po.Set_Value("QtyEntered", (Decimal?)QtyEnt);
                        po.Set_Value("MovementQty", (Decimal?)QtyEnt);
                        if (!po.Save())
                        {

                        }
                    }
                    continue;
                }

            newRecord:
                if (model[i].Keys.Contains("QuantityEntered"))
                {
                    d = Convert.ToDouble(model[i]["QuantityEntered"]);
                }
                else if (model[i].Keys.Contains("Quantity"))
                {
                    d = Convert.ToDouble(model[i]["Quantity"]);
                }
                Decimal QtyEntered = Convert.ToDecimal(d);

                if (model[i]["VAB_UOM_ID_K"] != "")
                    VAB_UOM_ID = Convert.ToInt32((model[i]["VAB_UOM_ID_K"]));               //  2-UOM
                if (model[i]["VAM_Product_ID_K"] != "")
                {
                    VAM_Product_ID = Convert.ToInt32((model[i]["VAM_Product_ID_K"]));       //  3-Product
                    //Arpit to Set Quality Plan if existed the module 
                    if (countVA010 > 0)
                    {
                        MProduct Product_ = MProduct.Get(ctx, VAM_Product_ID);
                        QualityPlan_ID = Util.GetValueOfInt(Product_.Get_Value("VA010_QualityPlan_ID"));
                    }

                }
                if (model[i]["VAB_Order_ID_K"] != "")
                    VAB_OrderLine_ID = Convert.ToInt32((model[i]["VAB_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["VAB_Invoice_ID_K"] != "")
                    VAB_InvoiceLine_ID = Convert.ToInt32((model[i]["VAB_Invoice_ID_K"]));   //  6-InvoiceLine

                if (model[i].Keys.Contains("VAM_PFeature_SetInstance_ID"))
                {
                    if (model[i]["VAM_PFeature_SetInstance_ID"] != "")
                        VAM_PFeature_SetInstance_ID = Convert.ToInt32((model[i]["VAM_PFeature_SetInstance_ID"]));   //  6-InvoiceLine
                }

                MInvoiceLine il = null;
                if (VAB_InvoiceLine_ID != 0)
                {
                    il = new MInvoiceLine(ctx, VAB_InvoiceLine_ID, null);
                }
                bool isInvoiced = (VAB_InvoiceLine_ID != 0);
                //	Precision of Qty UOM
                int precision = 2;
                if (VAM_Product_ID != 0)
                {
                    MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                    precision = product.GetUOMPrecision();
                }
                QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero);

                // when qty is ZERO -- thn not to update / create (Pallet task list point no : PT-114)
                if (QtyEntered == 0)
                    continue;

                //	Create new InOut Line
                //MInOutLine iol = new MInOutLine(inout);
                // Added By Bharat for ViennaAdvantage Compatiability. Code not called from ViennaAdvantage.
                po = tbl.GetPO(ctx, 0, null);
                po.Set_ValueNoCheck("VAM_Inv_InOut_ID", VAM_Inv_InOut_ID);
                po.SetClientOrg(inout);

                // set value if the value is non zero
                if (VAM_Product_ID > 0)
                {
                    po.Set_Value("VAM_Product_ID", VAM_Product_ID);
                }

                po.Set_ValueNoCheck("VAB_UOM_ID", VAB_UOM_ID);
                po.Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
                po.Set_Value("QtyEntered", (Decimal?)QtyEntered);
                po.Set_Value("MovementQty", (Decimal?)QtyEntered);
                //Edited by Arpit Rai 20th Sept,2017
                //To Set Drop Shipment True For Invoice/Shipment/Order

                //if (model[i].Keys.Contains("IsDropShip"))
                //{
                //    IsDropShip = Util.GetValueOfString(model[i]["IsDropShip"]);
                //}
                //po.Set_Value("IsDropShip", IsDropShip);
                if (countVA010 > 0 && VAM_Inv_InOut_ID > 0 && QualityPlan_ID > 0)
                    po.Set_ValueNoCheck("VA010_QualityPlan_ID", QualityPlan_ID);

                //iol.SetVAM_Product_ID(VAM_Product_ID, VAB_UOM_ID, VAM_PFeature_SetInstance_ID);	//	Line UOM
                //iol.SetQty(QtyEntered);							//	Movement/Entered
                //
                MOrderLine ol = null;
                if (VAB_OrderLine_ID != 0)
                {
                    po.Set_ValueNoCheck("VAB_OrderLine_ID", VAB_OrderLine_ID);
                    //iol.SetVAB_OrderLine_ID(VAB_OrderLine_ID);
                    ol = new MOrderLine(ctx, VAB_OrderLine_ID, null);
                    if (ol.GetQtyEntered().CompareTo(ol.GetQtyOrdered()) != 0)
                    {
                        po.Set_Value("MovementQty", Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, ol.GetQtyOrdered()), ol.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        po.Set_ValueNoCheck("VAB_UOM_ID", ol.GetVAB_UOM_ID());

                        //iol.SetMovementQty(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, ol.GetQtyOrdered()), ol.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));                        
                        //iol.SetVAB_UOM_ID(ol.GetVAB_UOM_ID());
                    }
                    // iol.SetVAM_PFeature_SetInstance_ID(ol.GetVAM_PFeature_SetInstance_ID());
                    //iol.SetVAM_PFeature_SetInstance_ID(0);//zero Becouse create diffrent SetVAM_PFeature_SetInstance_ID for MR agaist one PO
                    // Aded by Vivek on 24/10/2017
                    //To Set Drop Shipment True For Invoice/Shipment/Order
                    po.Set_Value("IsDropShip", ol.IsDropShip());
                    string Description = ol.GetDescription();
                    if (Description != null && Description.Length > 255)
                    {

                        Description = Description.Substring(0, 255);
                    }
                    po.Set_Value("Description", Description);
                    if (ol.GetVAB_Project_ID() <= 0)
                    {
                        po.Set_Value("VAB_Project_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_Project_ID", ol.GetVAB_Project_ID());
                    }
                    if (ol.GetVAB_ProjectStage_ID() <= 0)
                    {
                        po.Set_Value("VAB_ProjectStage_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_ProjectStage_ID", ol.GetVAB_ProjectJob_ID());
                    }
                    if (ol.GetVAB_ProjectJob_ID() <= 0)
                    {
                        po.Set_Value("VAB_ProjectJob_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_ProjectJob_ID", ol.GetVAB_ProjectJob_ID());
                    }
                    if (ol.GetVAB_BillingCode_ID() <= 0)
                    {
                        po.Set_Value("VAB_BillingCode_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_BillingCode_ID", ol.GetVAB_BillingCode_ID());
                    }
                    if (ol.GetVAB_Promotion_ID() <= 0)
                    {
                        po.Set_Value("VAB_Promotion_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_Promotion_ID", ol.GetVAB_Promotion_ID());
                    }
                    if (ol.GetVAF_OrgTrx_ID() <= 0)
                    {
                        po.Set_Value("VAF_OrgTrx_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAF_OrgTrx_ID", ol.GetVAF_OrgTrx_ID());
                    }
                    if (ol.GetUser1_ID() <= 0)
                    {
                        po.Set_Value("User1_ID", null);
                    }
                    else
                    {
                        po.Set_Value("User1_ID", ol.GetUser1_ID());
                    }
                    if (ol.GetUser2_ID() <= 0)
                    {
                        po.Set_Value("User2_ID", null);
                    }
                    else
                    {
                        po.Set_Value("User2_ID", ol.GetUser2_ID());
                    }

                    //iol.SetDescription(ol.GetDescription());
                    //iol.SetVAB_Project_ID(ol.GetVAB_Project_ID());
                    //iol.SetVAB_ProjectStage_ID(ol.GetVAB_ProjectStage_ID());
                    //iol.SetVAB_ProjectJob_ID(ol.GetVAB_ProjectJob_ID());
                    //iol.SetVAB_BillingCode_ID(ol.GetVAB_BillingCode_ID());
                    //iol.SetVAB_Promotion_ID(ol.GetVAB_Promotion_ID());
                    //iol.SetVAF_OrgTrx_ID(ol.GetVAF_OrgTrx_ID());
                    //iol.SetUser1_ID(ol.GetUser1_ID());
                    //iol.SetUser2_ID(ol.GetUser2_ID());
                }
                else if (il != null)
                {
                    if (il.GetQtyEntered().CompareTo(il.GetQtyInvoiced()) != 0)
                    {
                        //iol.SetQtyEntered(QtyEntered.multiply(il.getQtyInvoiced()).divide(il.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        po.Set_Value("QtyEntered", Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, il.GetQtyInvoiced()), il.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        po.Set_ValueNoCheck("VAB_UOM_ID", il.GetVAB_UOM_ID());

                        //iol.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, il.GetQtyInvoiced()), il.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        //iol.SetVAB_UOM_ID(il.GetVAB_UOM_ID());
                    }
                    string Description = il.GetDescription();
                    if (Description != null && Description.Length > 255)
                    {

                        Description = Description.Substring(0, 255);
                    }
                    po.Set_Value("Description", Description);
                    if (il.GetVAB_Project_ID() <= 0)
                    {
                        po.Set_Value("VAB_Project_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_Project_ID", il.GetVAB_Project_ID());
                    }
                    if (il.GetVAB_ProjectStage_ID() <= 0)
                    {
                        po.Set_Value("VAB_ProjectStage_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_ProjectStage_ID", il.GetVAB_ProjectJob_ID());
                    }
                    if (il.GetVAB_ProjectJob_ID() <= 0)
                    {
                        po.Set_Value("VAB_ProjectJob_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_ProjectJob_ID", il.GetVAB_ProjectJob_ID());
                    }
                    if (il.GetVAB_Promotion_ID() <= 0)
                    {

                    }
                    else
                    {
                        po.Set_Value("VAB_BillingCode_ID", il.GetVAB_BillingCode_ID());
                    }
                    if (il.GetVAB_Promotion_ID() <= 0)
                    {
                        po.Set_Value("VAB_Promotion_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAB_Promotion_ID", il.GetVAB_Promotion_ID());
                    }
                    if (il.GetVAF_OrgTrx_ID() <= 0)
                    {
                        po.Set_Value("VAF_OrgTrx_ID", null);
                    }
                    else
                    {
                        po.Set_Value("VAF_OrgTrx_ID", il.GetVAF_OrgTrx_ID());
                    }
                    if (il.GetUser1_ID() <= 0)
                    {
                        po.Set_Value("User1_ID", null);
                    }
                    else
                    {
                        po.Set_Value("User1_ID", il.GetUser1_ID());
                    }
                    if (il.GetUser2_ID() <= 0)
                    {
                        po.Set_Value("User2_ID", null);
                    }
                    else
                    {
                        po.Set_Value("User2_ID", il.GetUser2_ID());
                    }
                    //iol.SetDescription(il.GetDescription());
                    //iol.SetVAB_Project_ID(il.GetVAB_Project_ID());
                    //iol.SetVAB_ProjectStage_ID(il.GetVAB_ProjectStage_ID());
                    //iol.SetVAB_ProjectJob_ID(il.GetVAB_ProjectJob_ID());
                    //iol.SetVAB_BillingCode_ID(il.GetVAB_BillingCode_ID());
                    //iol.SetVAB_Promotion_ID(il.GetVAB_Promotion_ID());
                    //iol.SetVAF_OrgTrx_ID(il.GetVAF_OrgTrx_ID());
                    //iol.SetUser1_ID(il.GetUser1_ID());
                    //iol.SetUser2_ID(il.GetUser2_ID());
                }
                //	Charge
                if (VAM_Product_ID == 0)
                {
                    if (ol != null && ol.GetVAB_Charge_ID() != 0)			//	from order
                    {
                        po.Set_Value("VAB_Charge_ID", ol.GetVAB_Charge_ID());
                        //iol.SetVAB_Charge_ID(ol.GetVAB_Charge_ID());
                    }
                    else if (il != null && il.GetVAB_Charge_ID() != 0)	//	from invoice
                    {
                        po.Set_Value("VAB_Charge_ID", il.GetVAB_Charge_ID());
                        //iol.SetVAB_Charge_ID(il.GetVAB_Charge_ID());
                    }
                }
                po.Set_Value("VAM_Locator_ID", VAM_Locator_ID);
                //To set Product Container ID 
                if (isContainerApplicable && Container_ID > 0)
                    po.Set_Value("VAM_ProductContainer_ID", Container_ID);
                //iol.SetVAM_Locator_ID(VAM_Locator_ID);

                if (!po.Save())
                {
                    //s_log.log(Level.SEVERE, "Line NOT created #" + i);
                }
                //	Create Invoice Line Link
                else if (il != null)
                {
                    il.SetVAM_Inv_InOutLine_ID(po.Get_ID());
                    il.Save();
                }

            }   //  for all rows

            /**
             *  Update Header
             *  - if linked to another order/invoice - remove link
             *  - if no link set it
             */
            if (_order != null && _order.GetVAB_Order_ID() != 0)
            {
                inout.SetVAB_Order_ID(_order.GetVAB_Order_ID());
                inout.SetDateOrdered(_order.GetDateOrdered());
                inout.SetVAF_OrgTrx_ID(_order.GetVAF_OrgTrx_ID());
                inout.SetVAB_Project_ID(_order.GetVAB_Project_ID());
                inout.SetVAB_Promotion_ID(_order.GetVAB_Promotion_ID());
                inout.SetVAB_BillingCode_ID(_order.GetVAB_BillingCode_ID());
                inout.SetUser1_ID(_order.GetUser1_ID());
                inout.SetUser2_ID(_order.GetUser2_ID());
                // Change by Mohit asked by Amardeep sir 02/03/2016
                inout.SetPOReference(_order.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (inout.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                {
                    inout.SetVAB_IncoTerm_ID(_order.GetVAB_IncoTerm_ID());
                }
            }
            if (_invoice != null && _invoice.GetVAB_Invoice_ID() != 0)
            {
                if (inout.GetVAB_Order_ID() == 0)
                {
                    inout.SetVAB_Order_ID(_invoice.GetVAB_Order_ID());
                }
                inout.SetVAB_Invoice_ID(_invoice.GetVAB_Invoice_ID());
                inout.SetDateOrdered(_invoice.GetDateOrdered());
                inout.SetVAF_OrgTrx_ID(_invoice.GetVAF_OrgTrx_ID());
                inout.SetVAB_Project_ID(_invoice.GetVAB_Project_ID());
                inout.SetVAB_Promotion_ID(_invoice.GetVAB_Promotion_ID());
                inout.SetVAB_BillingCode_ID(_invoice.GetVAB_BillingCode_ID());
                inout.SetUser1_ID(_invoice.GetUser1_ID());
                inout.SetUser2_ID(_invoice.GetUser2_ID());
                // Change by Mohit asked by Amardeep sir 02/03/2016
                inout.SetPOReference(_invoice.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (inout.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                {
                    inout.SetVAB_IncoTerm_ID(_invoice.GetVAB_IncoTerm_ID());
                }
            }
            inout.Save();
            return true;
        }



        public bool SaveInvoiceData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int VAB_Order_ID, int VAB_Invoice_ID, int VAM_Inv_InOut_ID)
        {
            MOrder _order = null;
            if (VAB_Order_ID > 0)
            {
                _order = new MOrder(ctx, VAB_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (VAB_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, VAB_Invoice_ID, null);
            }


            MInOut _inout = null;
            if (VAM_Inv_InOut_ID > 0)
            {
                _inout = new MInOut(ctx, VAM_Inv_InOut_ID, null);
                // Chnage by Mohit Asked by Amardeep Sir 02/03/2016
                _invoice.SetPOReference(_inout.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (_invoice.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                {
                    _invoice.SetVAB_IncoTerm_ID(_inout.GetVAB_IncoTerm_ID());
                }
                _invoice.Save();
                //end
            }

            if (_order != null)
            {
                _invoice.SetOrder(_order);	//	overwrite header values

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (_invoice.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                {
                    _invoice.SetVAB_IncoTerm_ID(_order.GetVAB_IncoTerm_ID());
                }
                _invoice.Save();
            }
            if (_inout != null && _inout.GetVAM_Inv_InOut_ID() != 0
                && _inout.GetVAB_Invoice_ID() == 0)	//	only first time
            {
                _inout.SetVAB_Invoice_ID(VAB_Invoice_ID);
                _inout.Save();
            }
            //DateTime? AmortStartDate = null;
            //DateTime? AmortEndDate = null;
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int VAB_UOM_ID = 0;
                int VAM_Product_ID = 0;
                int VAB_OrderLine_ID = 0;
                int VAM_Inv_InOutLine_ID = 0;

                //Double d = Convert.ToDouble(model[i]["Quantity"]);                      //  1-Qty
                Double d = 0;
                if (Util.GetValueOfBool(model[i]["Select"]) == true)
                {
                    continue;
                }
                if (model[i].Keys.Contains("QuantityEntered"))
                {
                    d = Convert.ToDouble(model[i]["QuantityEntered"]);
                }
                else if (model[i].Keys.Contains("Quantity"))
                {
                    d = Convert.ToDouble(model[i]["Quantity"]);
                }
                Decimal QtyEntered = Convert.ToDecimal(d);
                if (model[i]["VAB_UOM_ID_K"] != "")
                    VAB_UOM_ID = Convert.ToInt32((model[i]["VAB_UOM_ID_K"]));               //  2-UOM
                if (model[i]["VAM_Product_ID_K"] != "")
                    VAM_Product_ID = Convert.ToInt32((model[i]["VAM_Product_ID_K"]));       //  3-Product
                if (model[i]["VAB_Order_ID_K"] != "")
                    VAB_OrderLine_ID = Convert.ToInt32((model[i]["VAB_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["VAM_Inv_InOut_ID_K"] != "")
                    VAM_Inv_InOutLine_ID = Convert.ToInt32((model[i]["VAM_Inv_InOut_ID_K"]));   //  5-Shipment

                //	Precision of Qty UOM
                int precision = 2;
                if (VAM_Product_ID != 0)
                {
                    MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                    precision = product.GetUOMPrecision();
                }

                //QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero); //commnted by Bharat as it is already rounded

                //s_log.fine("Line QtyEntered=" + QtyEntered
                //    + ", Product_ID=" + VAM_Product_ID 
                //    + ", OrderLine_ID=" + VAB_OrderLine_ID + ", InOutLine_ID=" + VAM_Inv_InOutLine_ID);

                //	Create new Invoice Line
                MInvoiceLine invoiceLine = new MInvoiceLine(_invoice);
                invoiceLine.SetVAM_Product_ID(VAM_Product_ID, VAB_UOM_ID);	//	Line UOM
                invoiceLine.SetQty(QtyEntered);							//	Invoiced/Entered
                //  Info
                MOrderLine orderLine = null;
                if (VAB_OrderLine_ID != 0)
                    orderLine = new MOrderLine(ctx, VAB_OrderLine_ID, null);
                MInOutLine inoutLine = null;
                if (VAM_Inv_InOutLine_ID != 0)
                {
                    inoutLine = new MInOutLine(ctx, VAM_Inv_InOutLine_ID, null);
                    if (orderLine == null && inoutLine.GetVAB_OrderLine_ID() != 0)
                    {
                        VAB_OrderLine_ID = inoutLine.GetVAB_OrderLine_ID();
                        orderLine = new MOrderLine(ctx, VAB_OrderLine_ID, null);
                    }
                }
                else
                {
                    MInOutLine[] lines = MInOutLine.GetOfOrderLine(ctx, VAB_OrderLine_ID, null, null);
                    //s_log.fine ("Receipt Lines with OrderLine = #" + lines.length);
                    if (lines.Length > 0)
                    {
                        for (int j = 0; j < lines.Length; j++)
                        {
                            MInOutLine line = lines[j];
                            if (line.GetQtyEntered().CompareTo(QtyEntered) == 0)
                            {
                                inoutLine = line;
                                VAM_Inv_InOutLine_ID = inoutLine.GetVAM_Inv_InOutLine_ID();
                                break;
                            }
                        }
                        if (inoutLine == null)
                        {
                            inoutLine = lines[0];	//	first as default
                            VAM_Inv_InOutLine_ID = inoutLine.GetVAM_Inv_InOutLine_ID();
                        }
                    }
                }	//	get Ship info

                //	Shipment Info
                if (inoutLine != null)
                {
                    invoiceLine.SetShipLine(inoutLine);		//	overwrites
                    if (inoutLine.GetQtyEntered().CompareTo(inoutLine.GetMovementQty()) != 0)
                    {
                        //invoiceLine.setQtyInvoiced(QtyEntered
                        //.multiply(inoutLine.getMovementQty())
                        //.divide(inoutLine.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        invoiceLine.SetQtyInvoiced(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered,
                        inoutLine.GetMovementQty()),
                        inoutLine.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));

                        // Change By mohit Amortization proces
                        //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
                        //if (countVA038 > 0)
                        //{

                        //    if (Util.GetValueOfInt(inoutLine.GetVAM_Product_ID()) > 0)
                        //    {
                        //        MProduct pro = new MProduct(ctx, inoutLine.GetVAM_Product_ID(), null);
                        //        if (Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                        //        {
                        //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                        //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                        //            AmortStartDate = null;
                        //            AmortEndDate = null;
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                        //            {
                        //                AmortStartDate = _invoice.GetDateAcct();
                        //            }
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                        //            {
                        //                AmortStartDate = _invoice.GetDateInvoiced();
                        //            }

                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                        //            {
                        //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                        //            }
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                        //            {
                        //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                        //            }
                        //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                        //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                        //            if (amrtDS != null)
                        //            {
                        //                amrtDS.Dispose();
                        //            }
                        //        }
                        //    }
                        //    if (Util.GetValueOfInt(inoutLine.GetVAB_Charge_ID()) > 0)
                        //    {
                        //        MVABCharge charge = new MVABCharge(ctx, inoutLine.GetVAB_Charge_ID(), null);
                        //        if (Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                        //        {
                        //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                        //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                        //            AmortStartDate = null;
                        //            AmortEndDate = null;
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                        //            {
                        //                AmortStartDate = _invoice.GetDateAcct();
                        //            }
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                        //            {
                        //                AmortStartDate = _invoice.GetDateInvoiced();
                        //            }

                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                        //            {
                        //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                        //            }
                        //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                        //            {
                        //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                        //            }
                        //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                        //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                        //            if (amrtDS != null)
                        //            {
                        //                amrtDS.Dispose();
                        //            }
                        //        }
                        //    }

                        //}

                        // End Amortization process
                    }
                }
                else
                {
                    //s_log.fine("No Receipt Line");
                }

                //	Order Info
                if (orderLine != null)
                {
                    invoiceLine.SetOrderLine(orderLine);	//	overwrites

                    /* nnayak - Bug 1567690. The organization from the Orderline can be different from the organization 
                    on the header */
                    invoiceLine.SetClientOrg(orderLine.GetVAF_Client_ID(), orderLine.GetVAF_Org_ID());
                    if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                    {
                        //invoiceLine.setQtyInvoiced(QtyEntered
                        //    .multiply(orderLine.getQtyOrdered())
                        //    .divide(orderLine.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        invoiceLine.SetQtyInvoiced(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered,
                        orderLine.GetQtyOrdered()),
                        orderLine.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                    }
                    // Change By mohit Amortization proces
                    //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
                    //if (countVA038 > 0)
                    //{
                    //    if (Util.GetValueOfInt(orderLine.GetVAM_Product_ID()) > 0)
                    //    {
                    //        MProduct pro = new MProduct(ctx, orderLine.GetVAM_Product_ID(), null);
                    //        if (Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                    //        {
                    //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            AmortStartDate = null;
                    //            AmortEndDate = null;
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                    //            {
                    //                AmortStartDate = _invoice.GetDateAcct();
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                    //            {
                    //                AmortStartDate = _invoice.GetDateInvoiced();
                    //            }

                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                    //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                    //            if (amrtDS != null)
                    //            {
                    //                amrtDS.Dispose();
                    //            }
                    //        }
                    //    }
                    //    if (Util.GetValueOfInt(orderLine.GetVAB_Charge_ID()) > 0)
                    //    {
                    //        MVABCharge charge = new MVABCharge(ctx, orderLine.GetVAB_Charge_ID(), null);
                    //        if (Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                    //        {
                    //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            AmortStartDate = null;
                    //            AmortEndDate = null;
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                    //            {
                    //                AmortStartDate = _invoice.GetDateAcct();
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                    //            {
                    //                AmortStartDate = _invoice.GetDateInvoiced();
                    //            }

                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                    //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                    //            if (amrtDS != null)
                    //            {
                    //                amrtDS.Dispose();
                    //            }
                    //        }
                    //    }

                    //}

                    // End Amortization process

                }
                else
                {
                    //s_log.fine("No Order Line");

                    /* nnayak - Bug 1567690. The organization from the Receipt can be different from the organization 
                    on the header */
                    if (inoutLine != null)
                    {
                        invoiceLine.SetClientOrg(inoutLine.GetVAF_Client_ID(), inoutLine.GetVAF_Org_ID());
                    }

                    invoiceLine.SetPrice();
                    invoiceLine.SetTax();
                    // Change By mohit Amortization proces
                    //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
                    //if (countVA038 > 0)
                    //{
                    //    if (Util.GetValueOfInt(inoutLine.GetVAM_Product_ID()) > 0)
                    //    {
                    //        MProduct pro = new MProduct(ctx, inoutLine.GetVAM_Product_ID(), null);
                    //        if (Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                    //        {
                    //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(pro.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            AmortStartDate = null;
                    //            AmortEndDate = null;
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                    //            {
                    //                AmortStartDate = _invoice.GetDateAcct();
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                    //            {
                    //                AmortStartDate = _invoice.GetDateInvoiced();
                    //            }

                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                    //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                    //            if (amrtDS != null)
                    //            {
                    //                amrtDS.Dispose();
                    //            }
                    //        }
                    //    }
                    //    if (Util.GetValueOfInt(inoutLine.GetVAB_Charge_ID()) > 0)
                    //    {
                    //        MVABCharge charge = new MVABCharge(ctx, inoutLine.GetVAB_Charge_ID(), null);
                    //        if (Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                    //        {
                    //            invoiceLine.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                    //            AmortStartDate = null;
                    //            AmortEndDate = null;
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                    //            {
                    //                AmortStartDate = _invoice.GetDateAcct();
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                    //            {
                    //                AmortStartDate = _invoice.GetDateInvoiced();
                    //            }

                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                    //            {
                    //                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                    //            }
                    //            invoiceLine.Set_Value("FROMDATE", AmortStartDate);
                    //            invoiceLine.Set_Value("EndDate", AmortEndDate);
                    //            if (amrtDS != null)
                    //            {
                    //                amrtDS.Dispose();
                    //            }
                    //        }
                    //    }

                    //}

                    // End Amortization process
                }
                if (!invoiceLine.Save())
                {
                    //s_log.log(Level.SEVERE, "Line NOT created #" + i);
                }
                else  // Added by Bharat issue given by Sumit - Order ID not set on Invoice Header.
                {
                    if (VAB_Order_ID == 0 && VAB_OrderLine_ID > 0)
                    {
                        MOrderLine ordLine = new MOrderLine(ctx, VAB_OrderLine_ID, null);
                        VAB_Order_ID = ordLine.GetVAB_Order_ID();
                        _order = new MOrder(ctx, VAB_Order_ID, null);
                        _invoice.SetVAB_Order_ID(VAB_Order_ID);
                        _invoice.SetDateOrdered(_order.GetDateOrdered());

                        // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                        if (_invoice.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                        {
                            _invoice.SetVAB_IncoTerm_ID(_order.GetVAB_IncoTerm_ID());
                        }
                        _invoice.Save();
                    }
                }

            }   //  for all rows

            return true;
        }

        public bool SaveStatmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int VAB_BankingJRNL_ID)
        {
            MVABBankingJRNL bs = new MVABBankingJRNL(ctx, VAB_BankingJRNL_ID, null);
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                DateTime trxDate = Convert.ToDateTime(model[i]["Date"]);          //  1-DateTrx
                int VAB_Payment_ID = Convert.ToInt32(model[i]["VAB_Payment_ID_K"]);   //  2-VAB_Payment_ID
                //int VAB_Currency_ID = Convert.ToInt32(model[i]["VAB_Currency_ID_K"]); //  3-Currency
                //Decimal TrxAmt = Convert.ToDecimal(model[i]["Amount"]);           //  4-PayAmt
                int VAB_Currency_ID = Convert.ToInt32(model[i]["VAB_Currency_ID_K"]); //  3-Currency
                Decimal TrxAmt = Convert.ToDecimal(model[i]["ConvertedAmount"]);           //  4-PayAmt
                string type = Util.GetValueOfString(model[i]["Type"]);
                MVABBankingJRNLLine bsl = new MVABBankingJRNLLine(bs);
                bsl.SetStatementLineDate(trxDate);
                //bsl.SetPayment(new MPayment(ctx, VAB_Payment_ID, null));
                MPayment pmt = new MPayment(ctx, VAB_Payment_ID, null);
                if (type == "P")
                {
                    bsl.SetVAB_Payment_ID(VAB_Payment_ID);
                }
                else
                {
                    bsl.SetVAB_CashJRNLLine_ID(VAB_Payment_ID);
                }
                bsl.SetVAB_Currency_ID(VAB_Currency_ID);
                bsl.SetTrxAmt(TrxAmt);
                bsl.SetStmtAmt(TrxAmt);
                bsl.SetDescription(pmt.GetDescription());
                bsl.Set_Value("TrxNo", Util.GetValueOfString(model[i]["AuthCode"]).Trim());
                int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VA034_' AND IsActive='Y'"));
                if (count > 0)
                    bsl.SetVA012_VoucherNo(Util.GetValueOfString(model[i]["VA034_DepositSlipNo"]));

                if (!bsl.Save())
                {
                    //s_log.log(Level.SEVERE, "Line not created #" + i);
                }
            }   //  for all rows
            return true;
        }

        public class LinesData
        {
            public List<DataObject> data
            {
                get;
                set;
            }
            public PageSetting pSetting
            {
                get;
                set;
            }
            public string Error
            {
                get;
                set;
            }
        }

        public class AccountData
        {
            public List<AccountObject> data
            {
                get;
                set;
            }
            public PageSetting pSetting
            {
                get;
                set;
            }
            public string Error
            {
                get;
                set;
            }
        }

        public class AccountObject
        {
            public int recid { get; set; }
            public bool Select { get; set; }
            public string Date { get; set; }
            public string VAB_Payment_ID { get; set; }
            public string VAB_Currency_ID { get; set; }
            public decimal Amount { get; set; }
            public decimal ConvertedAmount { get; set; }
            public string Description { get; set; }
            public string VAB_BusinessPartner_ID { get; set; }
            public string Type { get; set; }
            public int VAB_Payment_ID_K { get; set; }
            public int VAB_Currency_ID_K { get; set; }
            public string VA034_DepositSlipNo { get; set; }
            public string AuthCode { get; set; }
            public string CheckNo { get; set; }
        }
        public class DataObject
        {
            public int recid { get; set; }
            public bool Select { get; set; }
            public decimal Quantity { get; set; }
            public decimal QuantityPending { get; set; }
            public decimal QuantityEntered { get; set; }
            public string VAB_UOM_ID { get; set; }
            public string VAM_Product_ID { get; set; }
            public string VAB_Order_ID { get; set; }
            public string VAM_Inv_InOut_ID { get; set; }
            public string VAB_Invoice_ID { get; set; }
            public int VAB_UOM_ID_K { get; set; }
            public int VAM_Product_ID_K { get; set; }
            public int VAM_PFeature_SetInstance_ID { get; set; }
            public int VAB_Order_ID_K { get; set; }
            public int VAM_Inv_InOut_ID_K { get; set; }
            public int VAB_Invoice_ID_K { get; set; }
            public string IsDropShip { get; set; } // Arpit Rai
            public int VAB_PaymentTerm_ID { get; set; }
            public string PaymentTermName { get; set; }
            public bool IsAdvance { get; set; }
            public int VAB_InvoicePaymentTerm_ID { get; set; }
            public bool IsInvoicePTAdvance { get; set; }
            public string VAM_Product_SearchKey { get; set; }
        }

        public class PageSetting
        {
            public int CurrentPage
            {
                get;
                set;
            }
            public int TotalPage
            {
                get;
                set;
            }
        }

        #endregion

        #region Menual Forms

        public string statusBar { get; set; }
        public string lblStatusInfo { get; set; }
        public string ErrorMsg { get; set; }
        public string DocumentText { get; set; }

        public string GenerateInvoices(Ctx ctx, string whereClause)
        {
            //Changes done for invoice generation when multiple users generate the invoices at the same time. 02-May-18

            //String trxName = null;
            Trx trx = null;

            //	Reset Selection
            //String sql = "UPDATE VAB_Order SET IsSelected = 'N' WHERE IsSelected='Y'"
            //    + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID()
            //    + " AND VAF_Org_ID=" + ctx.GetVAF_Org_ID();

            //int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //log.Config("Reset=" + no); 

            //	Set Selection
            //sql = "UPDATE VAB_Order SET IsSelected = 'Y' WHERE " + whereClause;
            //no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));
            //if (no == 0)
            //{
            //    String msg = "No Invoices";     //  not translated!
            //    //log.Config(msg);
            //    //info.setText(msg);
            //    lblStatusInfo = msg.ToString();
            //    return msg.ToString();
            //}
            // log.Config("Set=" + no);

            lblStatusInfo = Msg.GetMsg(ctx, "InvGenerateGen");
            //statusBar += no.ToString();

            //	Prepare Process
            int VAF_Job_ID = 134;  // HARDCODED    VAB_InvoiceCreate
            MVAFJInstance instance = new MVAFJInstance(ctx, VAF_Job_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
                return Msg.GetMsg(ctx, "ProcessNoInstance");
            }

            ProcessInfo pi = new ProcessInfo("", VAF_Job_ID);
            pi.SetVAF_JInstance_ID(instance.GetVAF_JInstance_ID());

            pi.SetVAF_Client_ID(ctx.GetVAF_Client_ID());

            //	Add Parameters
            MVAFJInstancePara para = new MVAFJInstancePara(instance, 10);
            //para.setParameter("Selection", "Y");
            para.setParameter("Selection", "N");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MVAFJInstancePara(instance, 20);
            para.setParameter("DocAction", "CO");
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MVAFJInstancePara(instance, 30);
            para.setParameter("VAB_Order_ID", whereClause);
            if (!para.Save())
            {
                String msg = "No VAB_Order_ID Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }
            //End Changes done for invoice generation when multiple users generate the invoices at the same time. 02-May-18

            ProcessCtl worker = new ProcessCtl(ctx, null, pi, trx);
            worker.Run();
            GenerateInvoice_complete(ctx, pi, "");
            return "";
        }

        public string GenerateShipments(Ctx ctx, string whereClause, string VAM_Warehouse_ID)
        {

            Trx trx = null;

            //	Reset Selection
            String sql = "UPDATE VAB_Order SET IsSelected = 'N' "
            + "WHERE IsSelected='Y'"
            + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //	Set Selection
            sql = "UPDATE VAB_Order SET IsSelected = 'Y' WHERE " + whereClause;
            no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));
            if (no == 0)
            {
                String msg = "No Shipments";     //  not translated!
                lblStatusInfo = msg.ToString();
            }
            lblStatusInfo = Msg.GetMsg(ctx, "InOutGenerateGen");
            statusBar += no.ToString();

            //	Prepare Process
            int VAF_Job_ID = 199;	  // VAM_Inv_InOutCreate - Vframwork.Process.InOutGenerate
            MVAFJInstance instance = new MVAFJInstance(ctx, VAF_Job_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
            }
            ProcessInfo pi = new ProcessInfo("VInOutGen", VAF_Job_ID);
            pi.SetVAF_JInstance_ID(instance.GetVAF_JInstance_ID());

            pi.SetVAF_Client_ID(ctx.GetVAF_Client_ID());

            //	Add Parameter - Selection=Y
            MVAFJInstancePara para = new MVAFJInstancePara(instance, 10);
            para.setParameter("Selection", "Y");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
            }
            //	Add Parameter - VAM_Warehouse_ID=x
            para = new MVAFJInstancePara(instance, 20);
            para.setParameter("VAM_Warehouse_ID", Util.GetValueOfInt(VAM_Warehouse_ID));
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
            }

            //	Execute Process
            ProcessCtl worker = new ProcessCtl(ctx, null, pi, trx);
            worker.Run();
            GenerateShipments_complete(ctx, pi, "");
            return "";
        }

        //bool waiting = false;

        private void GenerateInvoice_complete(Ctx ctx, ProcessInfo pi, string whereClause)
        {
            ProcessInfoUtil.SetLogFromDB(pi);
            StringBuilder iText = new StringBuilder();
            iText.Append("<b>").Append(pi.GetSummary())
                .Append("</b><br>(")
                .Append(Msg.GetMsg(ctx, "InvGenerateInfo"))
                //Invoices are generated depending on the Invoicing Rule selection in the Order
                .Append(")<br>")
                .Append(pi.GetLogInfo(true));
            //info.setText(iText.toString());
            DocumentText = iText.ToString();

            //	Reset Selection
            String sql = "UPDATE VAB_Order SET IsSelected = 'N' WHERE " + whereClause;
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            //log.Config("Reset=" + no);
            //	Get results
            int[] ids = pi.GetIDs();
            if (ids == null || ids.Length == 0)
            {
                // waiting = true;
                return;
            }
            // waiting = true;
        }

        private void GenerateShipments_complete(Ctx ctx, ProcessInfo pi, string whereClause)
        {
            //  Switch Tabs
            ProcessInfoUtil.SetLogFromDB(pi);
            StringBuilder iText = new StringBuilder();
            iText.Append("<b>").Append(pi.GetSummary())
                .Append("</b><br>(")
                .Append(Msg.GetMsg(ctx, "InOutGenerateInfo"))
                //  Shipments are generated depending on the Delivery Rule selection in the Order
                .Append(")<br>")
                .Append(pi.GetLogInfo(true));

            DocumentText = iText.ToString();

            //	Reset Selection
            String sql = "UPDATE VAB_Order SET IsSelected = 'N' WHERE " + whereClause;
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            //	Get results
            int[] ids = pi.GetIDs();
            if (ids == null || ids.Length == 0)
            {
                // waiting = true;
                return;
            }
            // waiting = true;
        }

        //public void LockUI(ProcessInfo pi)
        //{

        //}

        //public void UnlockUI(ProcessInfo pi)
        //{

        //    if (pi.GetClassName().Contains("InOutGenerate"))
        //    {
        //        GenerateShipments_complete(pi, "");
        //    }
        //    else
        //    {
        //        GenerateInvoice_complete(pi, "");
        //    }
        //}


        //public bool IsUILocked()
        //{
        //    return true;
        //}

        //public void ExecuteASync(ProcessInfo pi)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region Match PO

        /// <summary>
        /// save/create Matched Purchase Order or Matched Invoice records by Matching PO - Invoices form
        /// </summary>
        /// <param name="invoice">Matching Invoice</param>
        /// /// <param name="MatchedMode">Mode of Matching</param>
        /// /// <param name="MatchFrom">Matched From</param>
        /// /// <param name="LineMatched">Line which is Matching</param>
        /// /// <param name="ToMatchQty">Qty need to be Matched</param>
        /// /// <param name="SelectedItems">Lines to which Matching is done</param>
        /// <returns>Info Msg / Matched PO No / Matched Invoice No</returns>
        public dynamic CreateMatchRecord(Ctx ctx, bool invoice, int MatchMode, int MatchFrom, int LineMatched, decimal ToMatchQty, List<GetTableLoadVmatch> data)
        {
            Trx trx = null;     // Trx.GetTrx("MatchPO");
            string msg = "";
            string conversionNotFoundMatch = "";
            MVAFClient client = null;
            decimal qty = 0;
            bool success = false;
            int VAM_Inv_InOutLine_ID = 0;
            int Line_ID = 0;
            string MatchedPO_ID = "";
            string MatchedInv_ID = "";
            dynamic retData = new ExpandoObject();
            //if (qty.CompareTo(Env.ZERO) == 0)
            //    return true;

            decimal toMatchQty = ToMatchQty;
            decimal matchedQty = 0;
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    matchedQty = Util.GetValueOfDecimal(data[i].Matched);
                    if (MatchMode == 0)
                    {
                        qty = Util.GetValueOfDecimal(data[i].Qty) - matchedQty;

                        if (toMatchQty <= 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        qty = 0 - matchedQty;
                    }

                    trx = Trx.GetTrx("MatchPO" + DateTime.Now.Ticks);

                    if (MatchMode == 0)
                    {
                        if (qty > toMatchQty)
                        {
                            qty = toMatchQty;
                        }
                        toMatchQty = toMatchQty - qty;
                    }
                    else
                    {
                        if (qty > matchedQty)
                            qty = qty - matchedQty;
                        matchedQty = matchedQty + qty;
                    }

                    if (MatchFrom == 1)
                    {
                        VAM_Inv_InOutLine_ID = LineMatched;      //  upper table
                        Line_ID = Util.GetValueOfInt(data[i].Line_K);
                    }
                    else
                    {
                        VAM_Inv_InOutLine_ID = Util.GetValueOfInt(data[i].Line_K); ;    //  lower table
                        Line_ID = LineMatched;
                    }

                    MInOutLine sLine = new MInOutLine(ctx, VAM_Inv_InOutLine_ID, trx);
                    MInOut ship = new MInOut(ctx, sLine.GetVAM_Inv_InOut_ID(), trx);
                    if (invoice)	//	Shipment - Invoice
                    {
                        //	Update Invoice Line
                        MInvoiceLine iLine = new MInvoiceLine(ctx, Line_ID, trx);
                        MInvoice inv = new MInvoice(ctx, iLine.GetVAB_Invoice_ID(), trx);
                        iLine.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                        if (sLine.GetVAB_OrderLine_ID() != 0)
                            iLine.SetVAB_OrderLine_ID(sLine.GetVAB_OrderLine_ID());
                        if (!iLine.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            msg += "Error: " + pp != null ? pp.GetName() : "";
                            success = false;
                        }
                        else
                        {
                            //client = MClient.Get(ctx, Util.GetValueOfInt(inv.GetVAF_Client_ID()));

                            //	Create Shipment - Invoice Link
                            if (iLine.GetVAM_Product_ID() != 0)
                            {
                                MMatchInv match = new MMatchInv(iLine, inv.GetDateInvoiced(), qty);
                                match.Set_ValueNoCheck("VAB_BusinessPartner_ID", inv.GetVAB_BusinessPartner_ID());
                                match.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                                if (match.Save())
                                {
                                    MatchedInv_ID = match.GetDocumentNo();
                                    success = true;
                                    // Check applied by mohit asked by ravikant to restrict the recalcualtion of costing for invoice for which the costing is already calculated.06/07/2017 PMS TaskID=4170
                                    if (iLine.GetVAB_OrderLine_ID() == 0 && !iLine.IsCostCalculated())
                                    {
                                        // updated by Amit 31-12-2015
                                        MProduct product = new MProduct(ctx, match.GetVAM_Product_ID(), trx);

                                        // Not returning any value as No effect
                                        MCostQueue.CreateProductCostsDetails(ctx, match.GetVAF_Client_ID(), match.GetVAF_Org_ID(), product,
                                             match.GetVAM_PFeature_SetInstance_ID(), "Match IV", null, sLine, null, iLine, null,
                                             Decimal.Multiply(Decimal.Divide(iLine.GetLineNetAmt(), iLine.GetQtyInvoiced()), match.GetQty()),
                                             match.GetQty(), trx, out conversionNotFoundMatch, "window");
                                    }
                                }
                                else
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    msg += "Error: " + pp != null ? pp.GetName() : "";
                                    success = false;
                                    //VLogger.Get log.Log(Level.SEVERE, "Inv Match not created: " + match);
                                }
                            }
                            else
                            {
                                success = true;
                            }
                            //	Create PO - Invoice Link = corrects PO
                            if (iLine.GetVAB_OrderLine_ID() != 0 && iLine.GetVAM_Product_ID() != 0)
                            {
                                MMatchPO matchPO = MMatchPO.Create(iLine, sLine, inv.GetDateInvoiced(), qty);
                                matchPO.Set_ValueNoCheck("VAB_BusinessPartner_ID", inv.GetVAB_BusinessPartner_ID());
                                matchPO.SetVAB_InvoiceLine_ID(iLine);
                                matchPO.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                                if (!matchPO.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    msg += "Error: " + pp != null ? pp.GetName() : "";
                                    success = false;
                                    //   log.Log(Level.SEVERE, "PO(Inv) Match not created: " + matchPO);
                                }
                                else
                                {
                                    MatchedPO_ID = matchPO.GetDocumentNo();
                                    // updated by Amit 31-12-2015
                                    MProduct product = new MProduct(ctx, matchPO.GetVAM_Product_ID(), trx);

                                    // Not returning any value as No effect
                                    MCostQueue.CreateProductCostsDetails(ctx, matchPO.GetVAF_Client_ID(), matchPO.GetVAF_Org_ID(), product,
                                          matchPO.GetVAM_PFeature_SetInstance_ID(), "Match IV", null, sLine, null, iLine, null,
                                          Decimal.Multiply(Decimal.Divide(iLine.GetLineNetAmt(), iLine.GetQtyInvoiced()), matchPO.GetQty()),
                                          matchPO.GetQty(), trx, out conversionNotFoundMatch, "window");
                                }
                            }
                        }
                    }
                    else	//	Shipment - Order
                    {
                        //	Update Shipment Line
                        sLine.SetVAB_OrderLine_ID(Line_ID);
                        if (!sLine.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            msg += "Error: " + pp != null ? pp.GetName() : "";
                            success = false;
                        }
                        else
                        {
                            //	Update Order Line
                            MOrderLine oLine = new MOrderLine(ctx, Line_ID, trx);
                            if (oLine.Get_ID() != 0)	//	other in MInOut.completeIt
                            {
                                //oLine.SetQtyReserved(oLine.GetQtyReserved().subtract(qty));
                                oLine.SetQtyReserved(Decimal.Subtract(oLine.GetQtyReserved(), qty));
                                if (!oLine.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    msg += "Error: " + pp != null ? pp.GetName() : "";
                                    success = false;
                                    //   log.Severe("QtyReserved not updated - VAB_OrderLine_ID=" + Line_ID);
                                }
                            }

                            //client = MClient.Get(ctx, Util.GetValueOfInt(sLine.GetVAF_Client_ID()));

                            //	Create PO - Shipment Link
                            if (sLine.GetVAM_Product_ID() != 0)
                            {
                                MMatchPO match = new MMatchPO(sLine, ship.GetMovementDate(), qty);
                                match.Set_ValueNoCheck("VAB_BusinessPartner_ID", ship.GetVAB_BusinessPartner_ID());
                                if (Util.GetValueOfInt(DB.ExecuteScalar("select count(*) from vaf_column where columnname like 'IsMatchPOForm'", null, trx)) > 0)
                                    match.SetIsMatchPOForm(true);
                                if (!match.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    msg += "Error: " + pp != null ? pp.GetName() : "";
                                    success = false;
                                    //   log.Log(Level.SEVERE, "PO Match not created: " + match);
                                }
                                else
                                {
                                    success = true;
                                    MatchedPO_ID = match.GetDocumentNo();
                                    // updated by Amit 23-12-2015
                                    MProduct product = new MProduct(ctx, match.GetVAM_Product_ID(), trx);

                                    // Not returning any value as No effect
                                    MCostQueue.CreateProductCostsDetails(ctx, match.GetVAF_Client_ID(), match.GetVAF_Org_ID(), product, match.GetVAM_PFeature_SetInstance_ID(),
                                        "Match PO", null, sLine, null, null, null, oLine.GetVAB_OrderLine_ID(), match.GetQty(), trx, out conversionNotFoundMatch, "window");
                                    if (!string.IsNullOrEmpty(conversionNotFoundMatch))
                                    {

                                    }
                                    else
                                    {
                                        sLine.SetIsCostCalculated(true);
                                        if (!sLine.Save())
                                        {
                                            success = false;
                                        }
                                    }

                                    //	Correct Ordered Qty for Stocked Products (see MOrder.reserveStock / MInOut.processIt)
                                    if (success && sLine.GetProduct() != null && sLine.GetProduct().IsStocked())
                                        success = MStorage.Add(ctx, sLine.GetVAM_Warehouse_ID(),
                                            sLine.GetVAM_Locator_ID(),
                                            sLine.GetVAM_Product_ID(),
                                            sLine.GetVAM_PFeature_SetInstance_ID(), oLine.GetVAM_PFeature_SetInstance_ID(),
                                            null, null, Decimal.Negate(qty), null);
                                }
                            }
                            else
                            {
                                success = true;
                            }
                        }
                    }
                    if (success)
                    {
                        if (trx != null)
                        {
                            trx.Commit();
                        }
                    }
                    else
                    {
                        if (trx != null)
                        {
                            trx.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg += "Error " + e.Message;
                if (trx != null)
                {
                    trx.Rollback();
                }
            }
            finally
            {
                if (trx != null)
                {
                    trx.Close();
                }
            }
            retData.Msg = msg;
            retData.MatchedPO_ID = MatchedPO_ID;
            retData.MatchedInv_ID = MatchedInv_ID;
            return retData;
        }
        #endregion

        #region Account Viewer
        //  Display Info
        //Display Qty			
        public bool displayQty = false;
        //Display Source Surrency
        public bool displaySourceAmt = false;
        //Display Document info	
        public bool displayDocumentInfo = false;
        //
        public String sortBy1 = "";
        public String sortBy2 = "";
        public String sortBy3 = "";
        public String sortBy4 = "";
        //
        public bool group1 = false;
        public bool group2 = false;
        public bool group3 = false;
        public bool group4 = false;

        // Leasing Columns		
        private int _leadingColumns = 0;
        //UserElement1 Reference	
        private String _ref1 = null;
        //UserElement2 Reference	
        private String _ref2 = null;
        //UserElement2 Reference	
        private String _ref3 = null;
        //UserElement2 Reference	
        private String _ref4 = null;
        //UserElement2 Reference	
        private String _ref5 = null;
        //UserElement2 Reference	
        private String _ref6 = null;
        //UserElement2 Reference	
        private String _ref7 = null;
        //UserElement2 Reference	
        private String _ref8 = null;
        //UserElement2 Reference	
        private String _ref9 = null;
        public String PostingType = "";
        public 
            MVABAccountBook[] ASchemas = null;
        public MVABAccountBook ASchema = null;


        public AccountViewClass GetDataQuery(Ctx ctx, int VAF_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4,
            String sort1, String sort2, String sort3, String sort4, bool displayDocInfo, bool displaySrcAmt, bool displayqty)
        {
            group1 = gr1; group2 = gr2; group3 = gr3; group4 = gr4;
            sortBy1 = sort1; sortBy2 = sort2; sortBy3 = sort3; sortBy4 = sort4;
            ASchemas = MVABAccountBook.GetClientAcctSchema(ctx, VAF_Client_ID);
            ASchema = ASchemas[0];
            displayDocumentInfo = displayDocInfo;
            displaySourceAmt = displaySrcAmt;
            displayQty = displayqty;


            RModel rm = GetRModel(ctx);

            //  Groups
            if (group1 && sortBy1.Length > 0)
            {
                rm.SetGroup(sortBy1);
            }
            if (group2 && sortBy2.Length > 0)
            {
                rm.SetGroup(sortBy2);
            }
            if (group3 && sortBy3.Length > 0)
            {
                rm.SetGroup(sortBy3);
            }
            if (group4 && sortBy4.Length > 0)
            {
                rm.SetGroup(sortBy4);
            }

            //  Totals
            rm.SetFunction("AmtAcctDr", RModel.FUNCTION_SUM);
            rm.SetFunction("AmtAcctCr", RModel.FUNCTION_SUM);

            rm.Query(ctx, whereClause.ToString(), orderClause.ToString());

            //return rm;

            AccountViewClass obj = new AccountViewClass();
            int col = rm.GetColumnCount();
            var arrList = new List<string>();
            for (int i = 0; i < col; i++)
            {
                RColumn rc = rm.GetRColumn(i);
                arrList.Add(rc.GetColHeader());
            }
            obj.Columns = arrList;
            obj.Data = rm._data.rows;
            return obj;
        }

        /// <summary>
        /// fillter data and column from RModel;
        /// </summary>
        /// <param name="reportModel"></param>
        public List<string> SetModel(RModel reportModel)
        {
            int col = reportModel.GetColumnCount();
            var arrList = new List<string>();

            for (int i = 0; i < col; i++)
            {
                RColumn rc = reportModel.GetRColumn(i);
                arrList.Add(rc.GetColHeader());
            }


            return arrList;
        }


        /// <summary>
        /// Create Report Model (Columns)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Report Model</returns>
        public RModel GetRModel(Ctx ctx)
        {
            RModel rm = new RModel("Actual_Acct_Detail");
            //  Add Key (Lookups)
            List<String> keys = CreateKeyColumns();
            int max = _leadingColumns;
            if (max == 0)
            {
                max = keys.Count;
            }
            for (int i = 0; i < max; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //  Main Info
            rm.AddColumn(new RColumn(ctx, "AmtAcctDr", DisplayType.Amount));
            rm.AddColumn(new RColumn(ctx, "AmtAcctCr", DisplayType.Amount));
            if (displaySourceAmt)
            {
                if (!keys.Contains("DateTrx"))
                {
                    rm.AddColumn(new RColumn(ctx, "DateTrx", DisplayType.Date));
                }
                rm.AddColumn(new RColumn(ctx, "VAB_Currency_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "AmtSourceDr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "AmtSourceCr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "Rate", DisplayType.Amount,
                    "CASE WHEN (AmtSourceDr + AmtSourceCr) = 0 THEN 0"
                    + " ELSE  Round((AmtAcctDr + AmtAcctCr) / (AmtSourceDr + AmtSourceCr),6) END"));
            }
            //	Remaining Keys
            for (int i = max; i < keys.Count; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column.StartsWith("UserElement"))
                {
                    if (column.IndexOf("1") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref1));
                    }
                    else if (column.IndexOf("2") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref2));
                    }
                    else if (column.IndexOf("3") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref3));
                    }
                    else if (column.IndexOf("4") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref4));
                    }
                    else if (column.IndexOf("5") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref5));
                    }
                    else if (column.IndexOf("6") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref6));
                    }
                    else if (column.IndexOf("7") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref7));
                    }
                    else if (column.IndexOf("8") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref8));
                    }
                    else
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref9));
                    }
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //	Info
            if (!keys.Contains("DateAcct"))
            {
                rm.AddColumn(new RColumn(ctx, "DateAcct", DisplayType.Date));
            }
            if (!keys.Contains("VAB_YearPeriod_ID"))
            {
                rm.AddColumn(new RColumn(ctx, "VAB_YearPeriod_ID", DisplayType.TableDir));
            }
            if (displayQty)
            {
                rm.AddColumn(new RColumn(ctx, "VAB_UOM_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Qty", DisplayType.Quantity));
            }
            if (displayDocumentInfo)
            {
                rm.AddColumn(new RColumn(ctx, "VAF_TableView_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Record_ID", DisplayType.ID));
                rm.AddColumn(new RColumn(ctx, "Description", DisplayType.String));
            }
            if (PostingType == null || PostingType.Length == 0)
            {
                rm.AddColumn(new RColumn(ctx, "PostingType", DisplayType.List,
                    MFactAcct.POSTINGTYPE_VAF_Control_Ref_ID));
            }
            return rm;
        }

        /// <summary>
        /// Create the key columns in sequence
        /// </summary>
        /// <returns>List of Key Columns</returns>
        private List<String> CreateKeyColumns()
        {
            List<String> columns = new List<String>();
            _leadingColumns = 0;
            //  Sorting Fields
            columns.Add(sortBy1);               //  may add ""
            if (!columns.Contains(sortBy2))
            {
                columns.Add(sortBy2);
            }
            if (!columns.Contains(sortBy3))
            {
                columns.Add(sortBy3);
            }
            if (!columns.Contains(sortBy4))
            {
                columns.Add(sortBy4);
            }


            //  Add Account Segments
            MVABAccountBookElement[] elements = ASchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                if (_leadingColumns == 0 && columns.Contains("VAF_Org_ID") && columns.Contains("Account_ID"))
                {
                    _leadingColumns = columns.Count;
                }
                //
                MVABAccountBookElement ase = elements[i];
                String columnName = ase.GetColumnName();
                if (columnName.StartsWith("UserElement"))
                {
                    if (columnName.IndexOf("1") != -1)
                    {
                        _ref1 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("2") != -1)
                    {
                        _ref2 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("3") != -1)
                    {
                        _ref3 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("4") != -1)
                    {
                        _ref4 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("5") != -1)
                    {
                        _ref5 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("6") != -1)
                    {
                        _ref6 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("7") != -1)
                    {
                        _ref7 = ase.GetDisplayColumnName();
                    }
                    else if (columnName.IndexOf("8") != -1)
                    {
                        _ref8 = ase.GetDisplayColumnName();
                    }
                    else
                    {
                        _ref9 = ase.GetDisplayColumnName();
                    }
                }
                if (!columns.Contains(columnName))
                {
                    columns.Add(columnName);

                }
            }
            if (_leadingColumns == 0 && columns.Contains("VAF_Org_ID") && columns.Contains("Account_ID"))
            {
                _leadingColumns = columns.Count;
            }
            return columns;
        }

        #endregion

        #region Archive Viewer

        public bool UpdateArchive(Ctx ctx, string name, string des, string help, int archiveId)
        {
            MVAFArchive ar = new MVAFArchive(ctx, archiveId, null);//  m_archives[m_index];
            ar.SetName(name);
            ar.SetDescription(des);
            ar.SetHelp(help);
            if (ar.Save())
            {
                return true;
            }
            return false;
        }

        public string DownloadPdf(Ctx ctx, int archiveId)
        {
            MVAFArchive ar = new MVAFArchive(ctx, archiveId, null);//  m_archives[m_index];
            MVAFSession sess = MVAFSession.Get(ctx);

            //Save Action Log
            VAdvantage.Common.Common.SaveActionLog(ctx, MActionLog.ACTION_Form, "Archive Viewer", ar.GetVAF_TableView_ID(), ar.GetRecord_ID(), 0, "", "", "Report Downloaded:->" + ar.GetName(), MActionLog.ACTIONTYPE_Download);

            byte[] report = ar.GetBinaryData();
            //if (report != null && (report.Length > 1048576))
            //{
            //    string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload" + "\\temp_" + DateTime.Now.Ticks + ".pdf";
            //    System.IO.File.WriteAllBytes(filePath, report);
            //    return filePath.Substring(filePath.IndexOf("TempDownload"));
            //}
            //else
            //{
            //    return Convert.ToBase64String(ar.GetBinaryData());
            //}

            //Save Byte[] on server side and return path of file
            string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            filePath = filePath + "\\temp_" + VAdvantage.Classes.CommonFunctions.CurrentTimeMillis() + ".pdf";

            System.IO.File.WriteAllBytes(filePath, report);
            return filePath.Substring(filePath.IndexOf("TempDownload"));

            return null;
        }

        // Added by Bharat on 07 June 2017
        //public List<Dictionary<string, object>> GetProcess(int VAF_Role_ID)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    string sql = "SELECT DISTINCT p.VAF_Job_ID, p.Name "
        //            + "FROM VAF_Job p INNER JOIN VAF_Job_Rights pa ON (p.VAF_Job_ID=pa.VAF_Job_ID) "
        //            + "WHERE pa.VAF_Role_ID=" + VAF_Role_ID
        //            + " AND p.IsReport='Y' AND p.IsActive='Y' AND pa.IsActive='Y' "
        //            + "ORDER BY 2";
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["VAF_Job_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Job_ID"]);
        //            obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
        //            retDic.Add(obj);
        //        }
        //    }
        //    return retDic;
        //}

        // Added by Bharat on 07 June 2017
        //public List<Dictionary<string, object>> GetTable(int VAF_Role_ID)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    string sql = "SELECT DISTINCT t.VAF_TableView_ID, t.Name "
        //        + "FROM VAF_TableView t INNER JOIN VAF_Tab tab ON (tab.VAF_TableView_ID=t.VAF_TableView_ID)"
        //        + " INNER JOIN VAF_Screen_Rights wa ON (tab.VAF_Screen_ID=wa.VAF_Screen_ID) "
        //        + "WHERE wa.VAF_Role_ID=" + VAF_Role_ID
        //        + " AND t.IsActive='Y' AND tab.IsActive='Y' "
        //        + "ORDER BY 2";
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["VAF_TableView_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"]);
        //            obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
        //            retDic.Add(obj);
        //        }
        //    }
        //    return retDic;
        //}

        // Added by Bharat on 07 June 2017
        //public List<Dictionary<string, object>> GetUser(Ctx ctx)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    string sql = "SELECT VAF_UserContact_ID, Name "
        //        + "FROM VAF_UserContact u WHERE EXISTS "
        //            + "(SELECT * FROM VAF_UserContact_Roles ur WHERE u.VAF_UserContact_ID=ur.VAF_UserContact_ID) "
        //        + "ORDER BY 2";
        //    sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_UserContact", MRole.SQL_NOTQUALIFIED, MRole.SQL_RW);
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["VAF_UserContact_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_UserContact_ID"]);
        //            obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
        //            retDic.Add(obj);
        //        }
        //    }
        //    return retDic;
        //}

        // Added by Bharat on 07 June 2017
        //public List<Dictionary<string, object>> GetArchiveData(string sql)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["NAME"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["NAME"]);
        //            obj["DESCRIPTION"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["DESCRIPTION"]);
        //            obj["HELP"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["HELP"]);
        //            obj["CREATEDBY"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT Name FROM VAF_UserContact WHERE VAF_UserContact_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["CREATEDBY"])));
        //            obj["CREATED"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["CREATED"]);
        //            obj["VAF_ARCHIVE_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_ARCHIVE_ID"]);
        //            retDic.Add(obj);
        //        }
        //    }
        //    return retDic;
        //}

        #endregion

        public List<GetOrderDataCommonsProperties> GetOrderDataCommons(Ctx ctx, string sql)
        {
            List<GetOrderDataCommonsProperties> obj = new List<GetOrderDataCommonsProperties>();

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GetOrderDataCommonsProperties obc = new GetOrderDataCommonsProperties();
                    obc.Quantity = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Quantity"]);
                    obc.QuantityEntered = Util.GetValueOfInt(ds.Tables[0].Rows[i]["QuantityEntered"]);
                    obc.VAB_UOM_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_UOM_ID"]);
                    obc.VAM_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]);
                    obc.VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                    obc.VAM_Inv_InOut_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOut_ID"]);
                    obc.VAB_Invoice_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Invoice_ID"]);
                    obc.VAB_UOM_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_UOM_ID_K"]);
                    obc.VAM_Product_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID_K"]);
                    obc.VAM_PFeature_SetInstance_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID_K"]);
                    obc.VAB_Order_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Order_ID_K"]);
                    obc.VAM_Inv_InOut_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOut_ID_K"]);
                    obc.VAB_Invoice_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Invoice_ID_K"]);
                    obj.Add(obc);
                }
            }
            return obj;
        }


        public int GetOrderDataCommonsNotOrg(Ctx ctx, int VAM_Product_ID_Ks)
        {
            string st = "SELECT VAM_PFeature_Set_ID FROM VAM_Product WHERE VAM_Product_ID = " + VAM_Product_ID_Ks;

            int mproductIDk = Util.GetValueOfInt(DB.ExecuteScalar(st));

            return mproductIDk;
        }

        /// Added by Sukhwinder on 05/Dec/2017
        /// <summary>
        /// Function  to get Period dates from year ID and Client ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPeriodFromYear(Ctx ctx, string fields)
        {
            Dictionary<string, object> retDic = null;
            try
            {
                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;

                if (paramValue != null)
                {

                    int YearId = Util.GetValueOfInt(paramValue[0]);
                    int AdClientID = Util.GetValueOfInt(paramValue[1]);

                    DateTime? stDate, eDate;
                    stDate = Util.GetValueOfDateTime(DB.ExecuteScalar(" SELECT P.STARTDATE AS STARTDATE    "
                                                                     + "    FROM VAB_YEARPERIOD P  "
                                                                     + "    INNER JOIN VAB_YEAR Y                "
                                                                     + "    ON P.VAB_YEAR_ID    =Y.VAB_YEAR_ID     "
                                                                     + "    WHERE P.PERIODNO  ='1'             "
                                                                     + "    AND P.VAB_YEAR_ID   = " + YearId
                                                                     + "    AND Y.VAF_CLIENT_ID= " + AdClientID));

                    eDate = Util.GetValueOfDateTime(DB.ExecuteScalar(" SELECT P.ENDDATE AS ENDDATE    "
                                                                     + "    FROM VAB_YEARPERIOD P  "
                                                                     + "    INNER JOIN VAB_YEAR Y                "
                                                                     + "    ON P.VAB_YEAR_ID    =Y.VAB_YEAR_ID     "
                                                                     + "    WHERE P.PERIODNO  ='12'             "
                                                                     + "    AND P.VAB_YEAR_ID   = " + YearId
                                                                     + "    AND Y.VAF_CLIENT_ID= " + AdClientID));

                    //DataSet ds = DB.ExecuteDataset(sql, null, null);
                    if (stDate != null && eDate != null)
                    {
                        retDic = new Dictionary<string, object>();
                        retDic["STARTDATE"] = Convert.ToDateTime(stDate).ToLocalTime();
                        retDic["ENDDATE"] = Convert.ToDateTime(eDate).ToLocalTime();
                    }
                }
            }
            catch
            {

            }
            return retDic;
        }
        //

        /// Added by Sukhwinder on 12/Dec/2017
        /// <summary>
        /// Get Currency From Accounting Schema
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetCurrencyFroMVABAccountingSchema(Ctx ctx, string fields)
        {
            Dictionary<string, object> retDic = null;
            try
            {
                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;

                if (paramValue != null)
                {

                    int AccountingSchemaId = Util.GetValueOfInt(paramValue[0]);
                    int AdClientID = Util.GetValueOfInt(paramValue[1]);

                    int CurrencyID = 0;
                    CurrencyID = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT VAB_CURRENCY_ID "
                                                                      + "  FROM VAB_ACCOUNTBOOK "
                                                                      + "  WHERE VAB_ACCOUNTBOOK_ID = " + AccountingSchemaId
                                                                      + "  AND VAF_CLIENT_ID     = " + AdClientID));



                    retDic = new Dictionary<string, object>();
                    retDic["CurrencyID"] = CurrencyID;
                }
            }
            catch
            {

            }
            return retDic;
        }

        /// <summary>
        /// Get Dataset for the query passed in the parameter
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="sql"></param>
        /// <returns>Dataset for query passed in parameter</returns>
        public DataSet GetIDTextData(Ctx ctx, string sql)
        {
            DataSet dsIDText = DB.ExecuteDataset(sql, null, null);
            if (dsIDText != null && dsIDText.Tables[0].Rows.Count > 0)
                return dsIDText;

            return dsIDText;
        }

        /// <summary>
        /// Check whether table is deletable on VAF_TableView window
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="TableName"></param>
        /// <returns>returns Y or N </returns>
        public string CheckTableDeletable(Ctx ctx, string TableName)
        {
            return Util.GetValueOfString(DB.ExecuteScalar("Select IsDeleteable from VAF_TableView WHERE TableName = '" + TableName + "'"));
        }

        /// <summary>
        /// function to delete record from table for ID passed in the parameter
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="TableName"></param>
        /// <param name="Record_ID"></param>
        /// <returns></returns>
        public Dictionary<string, string> DeleteRecord(Ctx ctx, string TableName, int Record_ID)
        {
            Dictionary<string, string> retRes = new Dictionary<string, string>();
            retRes["Success"] = "Y";
            retRes["Msg"] = "";
            PO _po = MVAFTableView.GetPO(ctx, TableName, Record_ID, null);
            // PO delete function
            if (!_po.Delete(true))
            {
                retRes["Success"] = "N";
                ValueNamePair vnp = VLogger.RetrieveError();
                if (vnp != null)
                {
                    if (vnp.GetName() != null)
                        retRes["Msg"] = vnp.GetName();
                    else if (vnp.GetValue() != null)
                        retRes["Msg"] = vnp.GetName();
                }
                else
                    retRes["Msg"] = Msg.GetMsg(ctx, "ErrorDeletingVersion");
                return retRes;
            }
            return retRes;
        }

        /// <summary>
        /// function to check versions against table
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rowData"></param>
        /// <returns>True/False</returns>
        public bool HasVersions(Ctx ctx, SaveRecordIn rowData)
        {
            if (rowData != null)
            {
                MVAFTableView tbl = new MVAFTableView(ctx, rowData.VAF_TableView_ID, null);

                StringBuilder sbSql = new StringBuilder("SELECT COUNT(VAF_TableView_ID) FROM VAF_TableView WHERE TableName = '" + rowData.TableName + "_Ver'");

                int Count = Util.GetValueOfInt(DB.ExecuteScalar(sbSql.ToString(), null, null));

                if (Count > 0)
                {
                    if (tbl.IsSingleKey())
                    {
                        if (rowData.Record_ID > 0)
                        {
                            sbSql.Clear();
                            sbSql.Append(@"SELECT COUNT(" + rowData.TableName + "_ID) FROM " + rowData.TableName + "_Ver " +
                                " WHERE " + rowData.TableName + "_ID = " + rowData.Record_ID + " AND ProcessedVersion = 'N' AND VersionLog IS NULL AND TRUNC(VersionValidFrom) >= TRUNC(SYSDATE)");
                            Count = Util.GetValueOfInt(DB.ExecuteScalar(sbSql.ToString()));
                            if (Count > 0)
                                return true;
                        }
                        return false;
                    }
                    else
                    {
                        sbSql.Clear();

                        string[] keyCols = tbl.GetKeyColumns();
                        bool hasCols = false;
                        for (int w = 0; w < keyCols.Length; w++)
                        {
                            hasCols = true;
                            if (w == 0)
                            {
                                sbSql.Append(@"SELECT COUNT(" + keyCols[w] + ") FROM " + rowData.TableName + "_Ver WHERE ");

                                if (keyCols[w] != null)
                                    sbSql.Append(keyCols[w] + " = " + rowData.RowData[keyCols[w].ToLower()]);
                                else
                                    sbSql.Append(" NVL(" + keyCols[w] + ",0) = 0");
                            }
                            else
                            {
                                if (keyCols[w] != null)
                                    sbSql.Append(" AND " + keyCols[w] + " = " + rowData.RowData[keyCols[w].ToLower()]);
                                else
                                    sbSql.Append(" AND NVL(" + keyCols[w] + ",0) = 0");
                            }
                        }
                        if (hasCols)
                        {
                            sbSql.Append(" AND ProcessedVersion = 'N' AND VersionLog IS NULL AND TRUNC(VersionValidFrom) >= TRUNC(SYSDATE)");
                            Count = Util.GetValueOfInt(DB.ExecuteScalar(sbSql.ToString()));
                            if (Count > 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }


        public string GetSQLQuery(Ctx m_ctx, int _VAF_TableView_ID, POInfoColumn[] m_columns)
        {
            StringBuilder _querySQL = new StringBuilder("");
            if (m_columns.Length > 0)
            {
                _querySQL.Append("SELECT ");
                MVAFTableView tbl = new MVAFTableView(m_ctx, _VAF_TableView_ID, null);
                // append all columns from table and get comma separated string
                _querySQL.Append(tbl.GetSelectColumns());
                foreach (var column in m_columns)
                {
                    // check if column name length is less than 26, then only add this column in selection column
                    // else only ID will be displayed
                    // as limitation in oracle to restrict column name to 30 characters
                    if ((column.ColumnName.Length + 4) < 30)
                    {
                        // for Lookup type of columns
                        if (DisplayType.IsLookup(column.DisplayType))
                        {
                            VLookUpInfo lookupInfo = VLookUpFactory.GetLookUpInfo(m_ctx, 0, column.DisplayType,
                                column.VAF_Column_ID, Env.GetLanguage(m_ctx), column.ColumnName, column.VAF_Control_Ref_Value_ID,
                                column.IsParent, column.ValidationCode);

                            if (lookupInfo != null && lookupInfo.displayColSubQ != null && lookupInfo.displayColSubQ.Trim() != "")
                            {
                                if (lookupInfo.queryDirect.Length > 0)
                                {
                                    // create columnname as columnname_TXT for lookup type of columns
                                    lookupInfo.displayColSubQ = " (SELECT MAX(" + lookupInfo.displayColSubQ + ") " + lookupInfo.queryDirect.Substring(lookupInfo.queryDirect.LastIndexOf(" FROM " + lookupInfo.tableName + " "), lookupInfo.queryDirect.Length - (lookupInfo.queryDirect.LastIndexOf(" FROM " + lookupInfo.tableName + " "))) + ") AS " + column.ColumnName + "_TXT";

                                    lookupInfo.displayColSubQ = lookupInfo.displayColSubQ.Replace("@key", tbl.GetTableName() + "." + column.ColumnName);
                                }
                                _querySQL.Append(", " + lookupInfo.displayColSubQ);
                            }
                        }
                        // case for Location type of columns
                        else if (column.DisplayType == DisplayType.Location)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_LOC");
                        }
                        // case for Locator type of columns
                        else if (column.DisplayType == DisplayType.Locator)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_LTR");
                        }
                        // case for Attribute Set Instance & General Attribute columns
                        else if (column.DisplayType == DisplayType.PAttribute || column.DisplayType == DisplayType.GAttribute)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_ASI");
                        }
                        // case for Account type of columns
                        else if (column.DisplayType == DisplayType.Account)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_ACT");
                        }
                    }
                }
                // Append FROM table name to query
                _querySQL.Append(" FROM " + tbl.GetTableName());
            }
            return _querySQL.ToString();
        }

    }

    public class AccountViewClass
    {
        public List<String> Columns { get; set; }
        public List<List<object>> Data { get; set; }
    }

    public class GetOrderDataCommonsProperties
    {
        public int Quantity { get; set; }
        public int QuantityEntered { get; set; }
        public int VAB_UOM_ID { get; set; }
        public int VAM_Product_ID { get; set; }
        public int VAM_PFeature_SetInstance_ID { get; set; }
        public int VAM_Inv_InOut_ID { get; set; }
        public int VAB_Invoice_ID { get; set; }
        public int VAB_UOM_ID_K { get; set; }
        public int VAM_Product_ID_K { get; set; }
        public int VAM_PFeature_SetInstance_ID_K { get; set; }
        public int VAB_Order_ID_K { get; set; }
        public int VAM_Inv_InOut_ID_K { get; set; }
        public int VAB_Invoice_ID_K { get; set; }

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