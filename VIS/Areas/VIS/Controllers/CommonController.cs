﻿using Newtonsoft.Json;
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
        public JsonResult SaveShipment(List<Dictionary<string, string>> model, string selectedItems, string C_Order_ID, string C_Invoice_ID, string m_locator_id, string M_inout_id, string Container_ID)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveShipmentData(ctx, model, selectedItems, Convert.ToInt32(C_Order_ID), Convert.ToInt32(C_Invoice_ID), Convert.ToInt32(m_locator_id), Convert.ToInt32(M_inout_id), Util.GetValueOfInt(Container_ID));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from shipment form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveInvoice(List<Dictionary<string, string>> model, string selectedItems, string C_Order_ID, string C_Invoice_ID, string M_inout_id)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveInvoiceData(ctx, model, selectedItems, Convert.ToInt32(C_Order_ID), Convert.ToInt32(C_Invoice_ID), Convert.ToInt32(M_inout_id));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from BankStatement form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="selectedItems"></param>
        /// <param name="C_BankStatement_ID"></param>
        /// <returns>return bool value in the form of JSON</returns>
        public JsonResult SaveStatment(List<Dictionary<string, string>> model, string selectedItems, string C_BankStatement_ID)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveStatmentData(ctx, model, selectedItems, Convert.ToInt32(C_BankStatement_ID));
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

        public JsonResult GenerateShipments(string whereClause, string M_Warehouse_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GenerateShipments(ctx, whereClause, M_Warehouse_ID);
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
        /// <summary>
        /// Get Data 
        /// </summary>
        /// <param name="AD_Client_ID">AD_Client_ID</param>
        /// <param name="whereClause">Where Clause</param>
        /// <param name="orderClause">Order Clause</param>
        /// <param name="gr1">group 1</param>
        /// <param name="gr2">group 2</param>
        /// <param name="gr3">group 3</param>
        /// <param name="gr4">group 4</param>
        /// <param name="sort1">sort 1</param>
        /// <param name="sort2">sort 2</param>
        /// <param name="sort3">sort 3</param>
        /// <param name="sort4">sort 4</param>
        /// <param name="displayDocInfo">display Doc Info</param>
        /// <param name="displaySrcAmt">Display Source amount</param>
        /// <param name="displayqty">displacy Quantity</param>
        /// <param name="pageNo">Page No</param>
        /// <returns>List</returns>
        public JsonResult GetDataQuery(int AD_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4,
            String sort1, String sort2, String sort3, String sort4, bool displayDocInfo, bool displaySrcAmt, bool displayqty, int pageNo)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GetDataQuery(ctx, AD_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4, sort1, sort2, sort3, sort4, displayDocInfo, displaySrcAmt, displayqty, pageNo);
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

        #region Generate XClasses

        public JsonResult GenerateXClasses(string directory, bool chkStatus, string tableId, string classType)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                StringBuilder sbTextCopy = new StringBuilder();
                string fileName = string.Empty;

                var msg = VAdvantage.Tool.GenerateModel.StartProcess("ViennaAdvantage.Model", directory, chkStatus, tableId, classType, out sbTextCopy, out fileName);
                string contant = sbTextCopy.ToString();
                return Json(new { contant, fileName, msg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = "Error" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region VAttributeGrid
        public JsonResult GetDataQueryAttribute()
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                List<AttributeGrid> lst = new List<AttributeGrid>();
                var value = MAttribute.GetOfClient(ctx, true, true);

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
                            localObj.GetM_Attribute_ID = attrValue.GetM_Attribute_ID();
                            localObj.GetM_AttributeValue_ID = attrValue.GetM_AttributeValue_ID();
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

        public JsonResult GetGridElement(int xM_Attribute_ID, int xM_AttributeValue_ID, int yM_Attribute_ID, int yM_AttributeValue_ID, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var stValue = obj.GetGridElement(ctx, xM_Attribute_ID, xM_AttributeValue_ID, yM_Attribute_ID, yM_AttributeValue_ID, M_PriceList_Version_ID, M_Warehouse_ID, windowNo);
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
        public JsonResult GetCurrencyFromAccountingSchema(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;
                CommonModel objCommonModel = new CommonModel();
                retJSON = JsonConvert.SerializeObject(objCommonModel.GetCurrencyFromAccountingSchema(ctx, fields));
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

                int AD_Table_ID = MTable.Get_Table_ID(TableName);

                POInfo inf = POInfo.GetPOInfo(ctx, AD_Table_ID);
                // Get SQL Query from PO Info for selected table
                string sqlCol = inf.GetSQLQuery();

                // Append where Clause, passed in the parameter
                string whClause = Util.GetValueOfString(paramValue[2]);
                if (whClause != "")
                    sqlCol += " WHERE " + whClause;

                // Apply Role check
                if (sqlCol.Trim() != "")
                    sqlCol = MRole.GetDefault(ctx).AddAccessSQL(sqlCol, TableName, MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

                // if SQL is being generated for Version table
                if (Util.GetValueOfBool(paramValue[1]))
                {
                    if (sqlCol.Contains("WHERE"))
                    {
                        sqlCol += " AND " + TableName + ".ProcessedVersion = 'N' ";
                    }
                    sqlCol += " ORDER BY " + TableName + ".VERSIONVALIDFROM DESC, " + TableName + ".RecordVersion DESC";
                }
                CommonModel objCommonModel = new CommonModel();
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
        public int GetM_Attribute_ID { get; set; }
        public int GetM_AttributeValue_ID { get; set; }
        public string Name { get; set; }
    }

    public class CommonModel
    {
        #region VAttributeGrid

        public string GetGridElement(Ctx ctx, int xM_Attribute_ID, int xM_AttributeValue_ID, int yM_Attribute_ID, int yM_AttributeValue_ID, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            StringBuilder panel = new StringBuilder();
            String sql = "SELECT * FROM M_Product WHERE IsActive='Y'";
            //	Product Attributes
            if (xM_Attribute_ID > 0)
            {
                sql += " AND M_AttributeSetInstance_ID IN "
                    + "(SELECT M_AttributeSetInstance_ID "
                    + "FROM M_AttributeInstance "
                    + "WHERE M_Attribute_ID=" + xM_Attribute_ID
                    + " AND M_AttributeValue_ID=" + xM_AttributeValue_ID + ")";
            }
            if (yM_Attribute_ID > 0)
            {
                sql += " AND M_AttributeSetInstance_ID IN "
                    + "(SELECT M_AttributeSetInstance_ID "
                    + "FROM M_AttributeInstance "
                    + "WHERE M_Attribute_ID=" + yM_Attribute_ID
                    + " AND M_AttributeValue_ID=" + yM_AttributeValue_ID + ")";
            }
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Product", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
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
                    panel.Append(AddProduct(product, M_PriceList_Version_ID, M_Warehouse_ID, windowNo));
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

        private string AddProduct(MProduct product, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            int M_Product_ID = product.GetM_Product_ID();
            StringBuilder obj = new StringBuilder();
            obj.Append("<table style='width: 100%;'><tr>");

            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblproductVal_" + windowNo + "' class='VIS_Pref_Label_Font'>" + product.GetValue() + "</label>");
            obj.Append("</td>");

            String formatted = "";
            if (M_PriceList_Version_ID != 0)
            {
                MProductPrice pp = MProductPrice.Get(Env.GetContext(), M_PriceList_Version_ID, M_Product_ID, null);
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
            if (M_Warehouse_ID != 0)
            {
                Decimal qty = Util.GetValueOfDecimal(MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, 0, null));
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
            String sql = "SELECT M_PriceList_Version.M_PriceList_Version_ID,"
                + " M_PriceList_Version.Name || ' (' || c.Iso_Code || ')' AS ValueName "
                + "FROM M_PriceList_Version, M_PriceList pl, C_Currency c "
                + "WHERE M_PriceList_Version.M_PriceList_ID=pl.M_PriceList_ID"
                + " AND pl.C_Currency_ID=c.C_Currency_ID"
                + " AND M_PriceList_Version.IsActive='Y' AND pl.IsActive='Y'";
            //	Add Access & Order
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_PriceList_Version", true, false)	// fully qualidfied - RO 
                + " ORDER BY M_PriceList_Version.Name";
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
                sql = "SELECT M_Warehouse_ID, Value || ' - ' || Name AS ValueName "
                    + "FROM M_Warehouse "
                    + "WHERE IsActive='Y'";
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql,
                        "M_Warehouse", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO)
                    + " ORDER BY Value";
                whList.Add(new KeyNamePair(0, ""));
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    KeyNamePair kn = new KeyNamePair(Util.GetValueOfInt(idr["M_Warehouse_ID"]), idr["ValueName"].ToString());
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
                int C_InvoicePaymentTerm_ID = 0; // invoice header payment term
                bool IsInvoicePTAdvance = false; // payment term binded on Invoice is advance or not

                // get invoice header payment tern and is advance or not
                if (keyColumnName == "C_Invoice_ID")
                {
                    DataSet ds = DB.ExecuteDataset(@"SELECT c_paymentterm.c_paymentterm_ID,
                                    SUM(  CASE WHEN c_paymentterm.VA009_Advance!= COALESCE(C_PaySchedule.VA009_Advance,'N') THEN 1 ELSE 0 END) AS IsAdvance
                                    FROM c_paymentterm LEFT JOIN C_PaySchedule ON ( c_paymentterm.c_paymentterm_ID = C_PaySchedule.c_paymentterm_ID AND C_PaySchedule.IsActive ='Y' )
                                    WHERE c_paymentterm.c_paymentterm_ID = (SELECT c_paymentterm_ID FROM C_Invoice WHERE C_Invoice_ID = " + recordID + @" )
                                    GROUP BY c_paymentterm.c_paymentterm_ID ");
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        C_InvoicePaymentTerm_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["c_paymentterm_ID"]);
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
                    item.C_UOM_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["uom"]);
                    item.M_Product_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["product"]);
                    item.M_AttributeSetInstance_ID = Util.GetValueOfInt(data.Tables[0].Rows[i]["m_attributesetinstance_id"]);
                    //Product search key added
                    item.M_Product_SearchKey = Util.GetValueOfString(data.Tables[0].Rows[i]["productsearchkey"]);

                    //
                    if (data.Tables[0].Columns.Contains("C_PaymentTerm_ID"))
                    {
                        if (data.Tables[0].Rows[i]["C_PaymentTerm_ID"] != DBNull.Value)
                        {
                            item.C_PaymentTerm_ID = Convert.ToInt32(data.Tables[0].Rows[i]["C_PaymentTerm_ID"]);
                            item.PaymentTermName = Convert.ToString(data.Tables[0].Rows[i]["PaymentTermName"]);
                            item.IsAdvance = Convert.ToInt32(data.Tables[0].Rows[i]["IsAdvance"]) > 0 ? true : false;
                        }
                    }
                    // 
                    if (keyColumnName == "C_Invoice_ID")
                    {
                        item.C_InvoicePaymentTerm_ID = C_InvoicePaymentTerm_ID;
                        item.IsInvoicePTAdvance = IsInvoicePTAdvance;
                    }

                    //Arpit Rai 20 Sept,2017
                    //item.IsDropShip = Util.GetValueOfString(data.Tables[0].Rows[i]["isdropship"]);

                    if (tableName == "C_OrderLine")
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);
                        if (keyColumnName == "M_InOut_ID")
                        {
                            if (recordid > 0)
                            {
                                qry = "SELECT SUM(QtyEntered) FROM M_InOutLine WHERE M_InOut_ID = " + recordID + " AND C_OrderLine_ID = " + recordid;
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
                                qry = "SELECT QtyEntered FROM M_InOutLine WHERE M_InOut_ID = " + recordID + " AND M_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_product_id"]) +
                                    " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_attributesetinstance_id"]);
                                rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (rec > 0)
                                {
                                    // Change By Mohit 30/06/2016
                                    select = true;
                                    item.QuantityEntered = rec;
                                }
                            }
                        }
                        else if (keyColumnName == "C_Invoice_ID")
                        {
                            recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);

                            if (recordid > 0)
                            {
                                qry = "SELECT SUM(QtyEntered) FROM C_InvoiceLine WHERE C_Invoice_ID = " + recordID + " AND C_OrderLine_ID = " + recordid;
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
                                qry = "SELECT QtyEntered FROM C_InvoiceLine WHERE C_Invoice_ID = " + recordID + " AND M_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_product_id"]) +
                                    " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_attributesetinstance_id"]);
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
                        item.C_Order_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.M_InOut_ID = null;
                        item.C_Invoice_ID = null;
                        item.C_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);
                        item.M_InOut_ID_K = 0;
                        item.C_Invoice_ID_K = 0;
                    }
                    else if (tableName == "C_InvoiceLine")
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);
                        if (recordid > 0)
                        {
                            qry = "SELECT QtyEntered FROM M_InOutLine WHERE M_InOut_ID = " + recordID + " AND C_OrderLine_ID = " + recordid;
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
                            qry = "SELECT QtyEntered FROM M_InOutLine WHERE M_InOut_ID = " + recordID + " AND M_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_product_id"]) +
                                " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_attributesetinstance_id"]);
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        item.Select = select;
                        item.C_Order_ID = null;
                        item.M_InOut_ID = null;
                        item.C_Invoice_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.C_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);
                        item.M_InOut_ID_K = 0;
                        item.C_Invoice_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_invoiceline_id"]);
                    }
                    else
                    {
                        recordid = Util.GetValueOfInt(data.Tables[0].Rows[i]["m_inoutline_id"]);
                        if (recordid > 0)
                        {
                            qry = "SELECT QtyEntered FROM C_InvoiceLine WHERE C_Invoice_ID = " + recordID + " AND M_InOutLine_ID = " + recordid;
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
                            qry = "SELECT QtyEntered FROM C_InvoiceLine WHERE C_Invoice_ID = " + recordID + " AND M_Product_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_product_id"]) +
                                " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(data.Tables[0].Rows[i]["m_attributesetinstance_id"]);
                            rec = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (rec > 0)
                            {
                                // Change By Mohit 30/06/2016
                                select = true;
                                item.QuantityEntered = rec;
                            }
                        }
                        item.Select = select;
                        item.C_Order_ID = null;
                        item.M_InOut_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["line"]);
                        item.C_Invoice_ID = null;
                        item.C_Order_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_orderline_id"]);
                        item.M_InOut_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["m_inoutline_id"]);
                        item.C_Invoice_ID_K = 0;
                    }

                    item.QuantityPending = item.QuantityEntered;
                    item.C_UOM_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["c_uom_id"]);
                    item.M_Product_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["m_product_id"]);

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
                    item.C_Payment_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["DocumentNo"]);
                    item.C_Currency_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["ISO_Code"]);
                    item.Amount = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["PayAmt"]);
                    item.ConvertedAmount = Util.GetValueOfDecimal(data.Tables[0].Rows[i]["ConvertedAmt"]);

                    desc = Util.GetValueOfString(data.Tables[0].Rows[i]["Description"]);
                    if (desc != null && desc.Length > 50)
                    {
                        desc.Substring(0, 50);
                    }
                    item.Description = desc;
                    item.C_BPartner_ID = Util.GetValueOfString(data.Tables[0].Rows[i]["Name"]);
                    item.Type = Util.GetValueOfString(data.Tables[0].Rows[i]["Type"]);
                    item.C_Payment_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["C_Payment_ID"]);
                    item.C_Currency_ID_K = Util.GetValueOfInt(data.Tables[0].Rows[i]["C_Currency_ID"]);


                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix='VA034_' AND IsActive='Y'"));
                    if (count > 0)
                        item.VA034_DepositSlipNo = Util.GetValueOfString(data.Tables[0].Rows[i]["VA034_DepositSlipNo"]);
                    else
                        item.VA034_DepositSlipNo = "";
                    // Change By Mohit To show trx no on create line from on bank statement
                    item.AuthCode = Util.GetValueOfString(data.Tables[0].Rows[i]["TrxNo"]);
                    item.CheckNo = Util.GetValueOfString(data.Tables[0].Rows[i]["CheckNo"]);
                    // Get the C_ConversionType_ID
                    item.C_ConversionType_ID = Util.GetValueOfInt(data.Tables[0].Rows[i]["C_ConversionType_ID"]);
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



        public bool SaveShipmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_Order_ID, int C_Invoice_ID, int M_Locator_ID, int M_InOut_ID, int Container_ID)
        {
            // chck pallet Functionality applicable or not
            bool isContainerApplicable = MTransaction.ProductContainerApplicable(ctx);

            MOrder _order = null;
            if (C_Order_ID > 0)
            {
                _order = new MOrder(ctx, C_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (C_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, C_Invoice_ID, null);
            }


            MInOut inout = null;
            if (M_InOut_ID > 0)
            {
                inout = new MInOut(ctx, M_InOut_ID, null);
            }
            // Added By Bharat for ViennaAdvantage Compatiability. Code not called from ViennaAdvantage.
            MTable tbl = null;
            PO po = null;
            tbl = new MTable(ctx, 320, null);
            /**
             *  Selected        - 0
             *  QtyEntered      - 1
             *  C_UOM_ID        - 2
             *  M_Product_ID    - 3
             *  OrderLine       - 4
             *  ShipmentLine    - 5
             *  InvoiceLine     - 6
             */
            int countVA010 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA010_' AND IsActive = 'Y'"));
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int C_UOM_ID = 0;
                int M_Product_ID = 0;
                int C_OrderLine_ID = 0;
                int C_InvoiceLine_ID = 0;
                int M_AttributeSetInstance_ID = 0;
                int M_InoutLine_ID = 0;
                string SqlIOL = "";
                //Arpit
                String IsDropShip = "N";
                int QualityPlan_ID = 0;
                //
                Double d = 0;
                if (Util.GetValueOfBool(model[i]["Select"]) == true)
                {
                    // Change By Mohit 30/06/2016
                    M_InoutLine_ID = 0;
                    if (model[i].Keys.Contains("QuantityEntered"))
                    {
                        d = Convert.ToDouble(model[i]["QuantityEntered"]);
                    }
                    else if (model[i].Keys.Contains("Quantity"))
                    {
                        d = Convert.ToDouble(model[i]["Quantity"]);
                    }
                    Decimal QtyEnt = Convert.ToDecimal(d);
                    //int precsn = 2;
                    //if (M_Product_ID != 0)
                    //{
                    //    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    //    precsn = product.GetUOMPrecision();
                    //}
                    //QtyEnt = Decimal.Round(QtyEnt, precsn, MidpointRounding.AwayFromZero);
                    // when qty is ZERO -- thn not to update / create (Pallet task list point no : PT-114)
                    if (QtyEnt == 0)
                        continue;

                    if (model[i]["M_Product_ID_K"] != "")
                    {
                        M_Product_ID = Convert.ToInt32((model[i]["M_Product_ID_K"]));       //  3-Product
                        //Arpit to Set Quality Plan if existed the module 
                        if (countVA010 > 0)
                        {
                            MProduct Product_ = MProduct.Get(ctx, M_Product_ID);
                            QualityPlan_ID = Util.GetValueOfInt(Product_.Get_Value("VA010_QualityPlan_ID"));
                        }
                    }
                    if (model[i]["C_Order_ID_K"] != "")
                        C_OrderLine_ID = Convert.ToInt32((model[i]["C_Order_ID_K"]));       //  4-OrderLine
                    if (model[i]["C_Invoice_ID_K"] != "")
                        C_InvoiceLine_ID = Convert.ToInt32((model[i]["C_Invoice_ID_K"]));   //  6-InvoiceLine

                    if (model[i].Keys.Contains("M_AttributeSetInstance_ID"))
                    {
                        if (model[i]["M_AttributeSetInstance_ID"] != "")
                            M_AttributeSetInstance_ID = Convert.ToInt32((model[i]["M_AttributeSetInstance_ID"]));   //  6-InvoiceLine
                    }
                    if (C_OrderLine_ID > 0)
                    {
                        // PO create with Lot enable product, but Lot/Attributesetinstance not define on PO.
                        // when we create MR against PO, with the same product having diffrent LOT/Attribute then system updating qty - not creating new MR line with different lot
                        // In this case system shold create new MR line with same product / same orderline but diffrent Attribute

                        //M_InoutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_INoutLine_ID from m_inoutline where M_InOut_ID=" + M_InOut_ID + " AND c_orderline_id=" + C_OrderLine_ID));
                        if (M_InoutLine_ID == 0)
                        {
                            SqlIOL = "SELECT M_INoutLine_ID FROM M_InOutLine WHERE M_InOut_ID = " + M_InOut_ID + " AND M_Product_ID = " + M_Product_ID + " AND c_orderline_id=" + C_OrderLine_ID;
                            //if (M_AttributeSetInstance_ID > 0)
                            //{ JID_1096: Also check Locator while finding the line on Material receipt
                            SqlIOL += " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_AttributeSetInstance_ID + " AND M_Locator_ID = " + M_Locator_ID;
                            //}
                            if (isContainerApplicable)
                            {
                                // To check with containerID to get Record
                                SqlIOL += " AND NVL(M_ProductContainer_ID , 0) = " + Container_ID;
                            }

                            M_InoutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                            if (M_InoutLine_ID == 0)
                                goto newRecord;
                        }
                        po = tbl.GetPO(ctx, M_InoutLine_ID, null);
                        if (countVA010 > 0 && QualityPlan_ID > 0)
                            po.Set_ValueNoCheck("VA010_QualityPlan_ID", QualityPlan_ID);

                        po.Set_Value("QtyEntered", (Decimal?)QtyEnt);
                        po.Set_Value("MovementQty", (Decimal?)QtyEnt);
                        if (!po.Save())
                        {

                        }
                    }
                    if (C_InvoiceLine_ID > 0)
                    {
                        //M_InoutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_InoutLine_ID FROM C_InvoiceLine WHERE C_InvoiceLine_ID=" + C_InvoiceLine_ID));
                        SqlIOL = "SELECT M_INoutLine_ID FROM C_InvoiceLine WHERE C_InvoiceLine_ID = " + C_InvoiceLine_ID + " AND M_Product_ID = " + M_Product_ID;
                        //if (M_AttributeSetInstance_ID > 0)
                        //{ 
                        SqlIOL += " AND NVL(M_AttributeSetInstance_ID, 0) = " + M_AttributeSetInstance_ID;
                        //}
                        if (isContainerApplicable)
                        {
                            // To check with containerID to get Record
                            SqlIOL += " AND NVL(M_ProductContainer_ID , 0) = " + Container_ID;
                        }

                        M_InoutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                        if (M_InoutLine_ID == 0)
                        {
                            SqlIOL = "SELECT M_INoutLine_ID FROM M_InOutLine WHERE M_InOut_ID = " + M_InOut_ID + " AND M_Product_ID = " + M_Product_ID;
                            //if (M_AttributeSetInstance_ID > 0)
                            //{ JID_1096: Also check Locator while finding the line on Material receipt
                            SqlIOL += " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + " AND M_Locator_ID = " + M_Locator_ID;
                            //}
                            if (isContainerApplicable)
                            {
                                // To check with containerID to get Record
                                SqlIOL += " AND NVL(M_ProductContainer_ID , 0) = " + Container_ID;
                            }

                            M_InoutLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(SqlIOL));
                            if (M_InoutLine_ID == 0)
                                goto newRecord;
                        }
                        po = tbl.GetPO(ctx, M_InoutLine_ID, null);
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

                if (model[i]["C_UOM_ID_K"] != "")
                    C_UOM_ID = Convert.ToInt32((model[i]["C_UOM_ID_K"]));               //  2-UOM
                if (model[i]["M_Product_ID_K"] != "")
                {
                    M_Product_ID = Convert.ToInt32((model[i]["M_Product_ID_K"]));       //  3-Product
                    //Arpit to Set Quality Plan if existed the module 
                    if (countVA010 > 0)
                    {
                        MProduct Product_ = MProduct.Get(ctx, M_Product_ID);
                        QualityPlan_ID = Util.GetValueOfInt(Product_.Get_Value("VA010_QualityPlan_ID"));
                    }

                }
                if (model[i]["C_Order_ID_K"] != "")
                    C_OrderLine_ID = Convert.ToInt32((model[i]["C_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["C_Invoice_ID_K"] != "")
                    C_InvoiceLine_ID = Convert.ToInt32((model[i]["C_Invoice_ID_K"]));   //  6-InvoiceLine

                if (model[i].Keys.Contains("M_AttributeSetInstance_ID"))
                {
                    if (model[i]["M_AttributeSetInstance_ID"] != "")
                        M_AttributeSetInstance_ID = Convert.ToInt32((model[i]["M_AttributeSetInstance_ID"]));   //  6-InvoiceLine
                }

                MInvoiceLine il = null;
                if (C_InvoiceLine_ID != 0)
                {
                    il = new MInvoiceLine(ctx, C_InvoiceLine_ID, null);
                }
                bool isInvoiced = (C_InvoiceLine_ID != 0);
                //	Precision of Qty UOM
                //int precision = 2;
                //if (M_Product_ID != 0)
                //{
                //    MProduct product = MProduct.Get(ctx, M_Product_ID);
                //    precision = product.GetUOMPrecision();
                //}
                //QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero);

                // when qty is ZERO -- thn not to update / create (Pallet task list point no : PT-114)
                if (QtyEntered == 0)
                    continue;

                //	Create new InOut Line
                //MInOutLine iol = new MInOutLine(inout);
                // Added By Bharat for ViennaAdvantage Compatiability. Code not called from ViennaAdvantage.
                po = tbl.GetPO(ctx, 0, null);
                po.Set_ValueNoCheck("M_InOut_ID", M_InOut_ID);
                po.SetClientOrg(inout);

                // set value if the value is non zero
                if (M_Product_ID > 0)
                {
                    po.Set_Value("M_Product_ID", M_Product_ID);
                }

                po.Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
                po.Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
                po.Set_Value("QtyEntered", (Decimal?)QtyEntered);
                po.Set_Value("MovementQty", (Decimal?)QtyEntered);
                //Edited by Arpit Rai 20th Sept,2017
                //To Set Drop Shipment True For Invoice/Shipment/Order

                //if (model[i].Keys.Contains("IsDropShip"))
                //{
                //    IsDropShip = Util.GetValueOfString(model[i]["IsDropShip"]);
                //}
                //po.Set_Value("IsDropShip", IsDropShip);
                if (countVA010 > 0 && M_InOut_ID > 0 && QualityPlan_ID > 0)
                    po.Set_ValueNoCheck("VA010_QualityPlan_ID", QualityPlan_ID);

                //iol.SetM_Product_ID(M_Product_ID, C_UOM_ID, M_AttributeSetInstance_ID);	//	Line UOM
                //iol.SetQty(QtyEntered);							//	Movement/Entered
                //
                MOrderLine ol = null;
                if (C_OrderLine_ID != 0)
                {
                    po.Set_ValueNoCheck("C_OrderLine_ID", C_OrderLine_ID);
                    //iol.SetC_OrderLine_ID(C_OrderLine_ID);
                    ol = new MOrderLine(ctx, C_OrderLine_ID, null);
                    if (ol.GetQtyEntered().CompareTo(ol.GetQtyOrdered()) != 0)
                    {
                        po.Set_Value("MovementQty", Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, ol.GetQtyOrdered()), ol.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        po.Set_ValueNoCheck("C_UOM_ID", ol.GetC_UOM_ID());

                        //iol.SetMovementQty(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, ol.GetQtyOrdered()), ol.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));                        
                        //iol.SetC_UOM_ID(ol.GetC_UOM_ID());
                    }
                    // iol.SetM_AttributeSetInstance_ID(ol.GetM_AttributeSetInstance_ID());
                    //iol.SetM_AttributeSetInstance_ID(0);//zero Becouse create diffrent SetM_AttributeSetInstance_ID for MR agaist one PO
                    // Aded by Vivek on 24/10/2017
                    //To Set Drop Shipment True For Invoice/Shipment/Order
                    po.Set_Value("IsDropShip", ol.IsDropShip());
                    string Description = ol.GetDescription();
                    if (Description != null && Description.Length > 255)
                    {

                        Description = Description.Substring(0, 255);
                    }
                    po.Set_Value("Description", Description);
                    if (ol.GetC_Project_ID() <= 0)
                    {
                        po.Set_Value("C_Project_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_Project_ID", ol.GetC_Project_ID());
                    }
                    if (ol.GetC_ProjectPhase_ID() <= 0)
                    {
                        po.Set_Value("C_ProjectPhase_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_ProjectPhase_ID", ol.GetC_ProjectTask_ID());
                    }
                    if (ol.GetC_ProjectTask_ID() <= 0)
                    {
                        po.Set_Value("C_ProjectTask_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_ProjectTask_ID", ol.GetC_ProjectTask_ID());
                    }
                    if (ol.GetC_Activity_ID() <= 0)
                    {
                        po.Set_Value("C_Activity_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_Activity_ID", ol.GetC_Activity_ID());
                    }
                    if (ol.GetC_Campaign_ID() <= 0)
                    {
                        po.Set_Value("C_Campaign_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_Campaign_ID", ol.GetC_Campaign_ID());
                    }
                    if (ol.GetAD_OrgTrx_ID() <= 0)
                    {
                        po.Set_Value("AD_OrgTrx_ID", null);
                    }
                    else
                    {
                        po.Set_Value("AD_OrgTrx_ID", ol.GetAD_OrgTrx_ID());
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
                    //iol.SetC_Project_ID(ol.GetC_Project_ID());
                    //iol.SetC_ProjectPhase_ID(ol.GetC_ProjectPhase_ID());
                    //iol.SetC_ProjectTask_ID(ol.GetC_ProjectTask_ID());
                    //iol.SetC_Activity_ID(ol.GetC_Activity_ID());
                    //iol.SetC_Campaign_ID(ol.GetC_Campaign_ID());
                    //iol.SetAD_OrgTrx_ID(ol.GetAD_OrgTrx_ID());
                    //iol.SetUser1_ID(ol.GetUser1_ID());
                    //iol.SetUser2_ID(ol.GetUser2_ID());
                }
                else if (il != null)
                {
                    if (il.GetQtyEntered().CompareTo(il.GetQtyInvoiced()) != 0)
                    {
                        //iol.SetQtyEntered(QtyEntered.multiply(il.getQtyInvoiced()).divide(il.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        po.Set_Value("QtyEntered", Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, il.GetQtyInvoiced()), il.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        po.Set_ValueNoCheck("C_UOM_ID", il.GetC_UOM_ID());

                        //iol.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, il.GetQtyInvoiced()), il.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        //iol.SetC_UOM_ID(il.GetC_UOM_ID());
                    }
                    string Description = il.GetDescription();
                    if (Description != null && Description.Length > 255)
                    {

                        Description = Description.Substring(0, 255);
                    }
                    po.Set_Value("Description", Description);
                    if (il.GetC_Project_ID() <= 0)
                    {
                        po.Set_Value("C_Project_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_Project_ID", il.GetC_Project_ID());
                    }
                    if (il.GetC_ProjectPhase_ID() <= 0)
                    {
                        po.Set_Value("C_ProjectPhase_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_ProjectPhase_ID", il.GetC_ProjectTask_ID());
                    }
                    if (il.GetC_ProjectTask_ID() <= 0)
                    {
                        po.Set_Value("C_ProjectTask_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_ProjectTask_ID", il.GetC_ProjectTask_ID());
                    }
                    if (il.GetC_Campaign_ID() <= 0)
                    {

                    }
                    else
                    {
                        po.Set_Value("C_Activity_ID", il.GetC_Activity_ID());
                    }
                    if (il.GetC_Campaign_ID() <= 0)
                    {
                        po.Set_Value("C_Campaign_ID", null);
                    }
                    else
                    {
                        po.Set_Value("C_Campaign_ID", il.GetC_Campaign_ID());
                    }
                    if (il.GetAD_OrgTrx_ID() <= 0)
                    {
                        po.Set_Value("AD_OrgTrx_ID", null);
                    }
                    else
                    {
                        po.Set_Value("AD_OrgTrx_ID", il.GetAD_OrgTrx_ID());
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
                    //iol.SetC_Project_ID(il.GetC_Project_ID());
                    //iol.SetC_ProjectPhase_ID(il.GetC_ProjectPhase_ID());
                    //iol.SetC_ProjectTask_ID(il.GetC_ProjectTask_ID());
                    //iol.SetC_Activity_ID(il.GetC_Activity_ID());
                    //iol.SetC_Campaign_ID(il.GetC_Campaign_ID());
                    //iol.SetAD_OrgTrx_ID(il.GetAD_OrgTrx_ID());
                    //iol.SetUser1_ID(il.GetUser1_ID());
                    //iol.SetUser2_ID(il.GetUser2_ID());
                }
                //	Charge
                if (M_Product_ID == 0)
                {
                    if (ol != null && ol.GetC_Charge_ID() != 0)			//	from order
                    {
                        po.Set_Value("C_Charge_ID", ol.GetC_Charge_ID());
                        //iol.SetC_Charge_ID(ol.GetC_Charge_ID());
                    }
                    else if (il != null && il.GetC_Charge_ID() != 0)	//	from invoice
                    {
                        po.Set_Value("C_Charge_ID", il.GetC_Charge_ID());
                        //iol.SetC_Charge_ID(il.GetC_Charge_ID());
                    }
                }
                po.Set_Value("M_Locator_ID", M_Locator_ID);
                //To set Product Container ID 
                if (isContainerApplicable && Container_ID > 0)
                    po.Set_Value("M_ProductContainer_ID", Container_ID);
                //iol.SetM_Locator_ID(M_Locator_ID);

                if (!po.Save())
                {
                    //s_log.log(Level.SEVERE, "Line NOT created #" + i);
                }
                //	Create Invoice Line Link
                else if (il != null)
                {
                    il.SetM_InOutLine_ID(po.Get_ID());
                    il.Save();
                }

            }   //  for all rows

            /**
             *  Update Header
             *  - if linked to another order/invoice - remove link
             *  - if no link set it
             */
            if (_order != null && _order.GetC_Order_ID() != 0)
            {
                inout.SetC_Order_ID(_order.GetC_Order_ID());
                inout.SetDateOrdered(_order.GetDateOrdered());
                inout.SetAD_OrgTrx_ID(_order.GetAD_OrgTrx_ID());
                inout.SetC_Project_ID(_order.GetC_Project_ID());
                inout.SetC_Campaign_ID(_order.GetC_Campaign_ID());
                inout.SetC_Activity_ID(_order.GetC_Activity_ID());
                inout.SetUser1_ID(_order.GetUser1_ID());
                inout.SetUser2_ID(_order.GetUser2_ID());
                // Change by Mohit asked by Amardeep sir 02/03/2016
                inout.SetPOReference(_order.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (inout.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    inout.SetC_IncoTerm_ID(_order.GetC_IncoTerm_ID());
                }
            }
            if (_invoice != null && _invoice.GetC_Invoice_ID() != 0)
            {
                if (inout.GetC_Order_ID() == 0)
                {
                    inout.SetC_Order_ID(_invoice.GetC_Order_ID());
                }
                inout.SetC_Invoice_ID(_invoice.GetC_Invoice_ID());
                inout.SetDateOrdered(_invoice.GetDateOrdered());
                inout.SetAD_OrgTrx_ID(_invoice.GetAD_OrgTrx_ID());
                inout.SetC_Project_ID(_invoice.GetC_Project_ID());
                inout.SetC_Campaign_ID(_invoice.GetC_Campaign_ID());
                inout.SetC_Activity_ID(_invoice.GetC_Activity_ID());
                inout.SetUser1_ID(_invoice.GetUser1_ID());
                inout.SetUser2_ID(_invoice.GetUser2_ID());
                // Change by Mohit asked by Amardeep sir 02/03/2016
                inout.SetPOReference(_invoice.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (inout.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    inout.SetC_IncoTerm_ID(_invoice.GetC_IncoTerm_ID());
                }
            }
            inout.Save();
            return true;
        }

        /// <summary>
        /// Get Theme list
        /// </summary>
        /// <returns>dynamic list</returns>
        internal List<dynamic> GetThemes()
        {
            List<dynamic> retObj = new List<dynamic>();
            string qry = " SELECT PrimaryColor, OnPrimaryColor, SecondaryColor, OnSecondaryColor " +
                                " , IsDefault, AD_Theme_ID  FROM AD_Theme WHERE IsActive='Y'";
            DataSet ds = DB.ExecuteDataset(qry);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dynamic obj = new ExpandoObject();
                    obj.Id = Util.GetValueOfString(dr["AD_Theme_ID"]);
                    obj.PColor = Util.GetValueOfString(dr["PrimaryColor"]);
                    obj.OnPColor = Util.GetValueOfString(dr["OnPrimaryColor"]);
                    obj.SColor = Util.GetValueOfString(dr["SecondaryColor"]);
                    obj.OnSColor = Util.GetValueOfString(dr["OnSecondaryColor"]);
                    obj.IsDefault = Util.GetValueOfString(dr["IsDefault"]);
                    retObj.Add(obj);
                }

            }
            return retObj;
        }

        public bool SaveInvoiceData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_Order_ID, int C_Invoice_ID, int M_InOut_ID)
        {
            MOrder _order = null;
            if (C_Order_ID > 0)
            {
                _order = new MOrder(ctx, C_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (C_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, C_Invoice_ID, null);
            }


            MInOut _inout = null;
            if (M_InOut_ID > 0)
            {
                _inout = new MInOut(ctx, M_InOut_ID, null);
                // Chnage by Mohit Asked by Amardeep Sir 02/03/2016
                _invoice.SetPOReference(_inout.GetPOReference());

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (_invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    _invoice.SetC_IncoTerm_ID(_inout.GetC_IncoTerm_ID());
                }
                _invoice.Save();
                //end
            }

            if (_order != null)
            {
                _invoice.SetOrder(_order);	//	overwrite header values

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                if (_invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    _invoice.SetC_IncoTerm_ID(_order.GetC_IncoTerm_ID());
                }

                if (Env.IsModuleInstalled("VA077_"))
                {
                    _invoice.Set_Value("VA077_ChangeStartDate", _order.Get_Value("VA077_ChangeStartDate"));
                    _invoice.Set_Value("VA077_PartialAmtCatchUp", _order.Get_Value("VA077_PartialAmtCatchUp"));
                    _invoice.Set_Value("VA077_OldAnnualContractTotal", _order.Get_Value("VA077_OldAnnualContractTotal"));
                    _invoice.Set_Value("VA077_AdditionalAnnualCharge", _order.Get_Value("VA077_AdditionalAnnualCharge"));
                    _invoice.Set_Value("VA077_NewAnnualContractTotal", _order.Get_Value("VA077_NewAnnualContractTotal"));
                    _invoice.Set_Value("VA077_SalesCoWorker", _order.Get_Value("VA077_SalesCoWorker"));
                    _invoice.Set_Value("VA077_SalesCoWorkerPer", _order.Get_Value("VA077_SalesCoWorkerPer"));
                    _invoice.Set_Value("VA077_TotalMarginAmt", _order.Get_Value("VA077_TotalMarginAmt"));
                    _invoice.Set_Value("VA077_TotalPurchaseAmt", _order.Get_Value("VA077_TotalPurchaseAmt"));
                    _invoice.Set_Value("VA077_TotalSalesAmt", _order.Get_Value("VA077_TotalSalesAmt"));
                    _invoice.Set_Value("VA077_MarginPercent", _order.Get_Value("VA077_MarginPercent"));
                    
                }

                _invoice.Save();
            }
            if (_inout != null && _inout.GetM_InOut_ID() != 0
                && _inout.GetC_Invoice_ID() == 0)	//	only first time
            {
                _inout.SetC_Invoice_ID(C_Invoice_ID);
                _inout.Save();
            }
            //DateTime? AmortStartDate = null;
            //DateTime? AmortEndDate = null;
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int C_UOM_ID = 0;
                int M_Product_ID = 0;
                int C_OrderLine_ID = 0;
                int M_InOutLine_ID = 0;

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
                if (model[i]["C_UOM_ID_K"] != "")
                    C_UOM_ID = Convert.ToInt32((model[i]["C_UOM_ID_K"]));               //  2-UOM
                if (model[i]["M_Product_ID_K"] != "")
                    M_Product_ID = Convert.ToInt32((model[i]["M_Product_ID_K"]));       //  3-Product
                if (model[i]["C_Order_ID_K"] != "")
                    C_OrderLine_ID = Convert.ToInt32((model[i]["C_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["M_InOut_ID_K"] != "")
                    M_InOutLine_ID = Convert.ToInt32((model[i]["M_InOut_ID_K"]));   //  5-Shipment

                //	Precision of Qty UOM
                int precision = 2;
                if (M_Product_ID != 0)
                {
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    precision = product.GetUOMPrecision();
                }

                //QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero); //commnted by Bharat as it is already rounded

                //s_log.fine("Line QtyEntered=" + QtyEntered
                //    + ", Product_ID=" + M_Product_ID 
                //    + ", OrderLine_ID=" + C_OrderLine_ID + ", InOutLine_ID=" + M_InOutLine_ID);

                //	Create new Invoice Line
                MInvoiceLine invoiceLine = new MInvoiceLine(_invoice);
                invoiceLine.SetM_Product_ID(M_Product_ID, C_UOM_ID);	//	Line UOM
                invoiceLine.SetQty(QtyEntered);							//	Invoiced/Entered



                //  Info
                MOrderLine orderLine = null;
                if (C_OrderLine_ID != 0)
                    orderLine = new MOrderLine(ctx, C_OrderLine_ID, null);
                MInOutLine inoutLine = null;
                if (M_InOutLine_ID != 0)
                {
                    inoutLine = new MInOutLine(ctx, M_InOutLine_ID, null);
                    if (orderLine == null && inoutLine.GetC_OrderLine_ID() != 0)
                    {
                        C_OrderLine_ID = inoutLine.GetC_OrderLine_ID();
                        orderLine = new MOrderLine(ctx, C_OrderLine_ID, null);
                    }
                }
                else
                {
                    MInOutLine[] lines = MInOutLine.GetOfOrderLine(ctx, C_OrderLine_ID, null, null);
                    //s_log.fine ("Receipt Lines with OrderLine = #" + lines.length);
                    if (lines.Length > 0)
                    {
                        for (int j = 0; j < lines.Length; j++)
                        {
                            MInOutLine line = lines[j];
                            if (line.GetQtyEntered().CompareTo(QtyEntered) == 0)
                            {
                                inoutLine = line;
                                M_InOutLine_ID = inoutLine.GetM_InOutLine_ID();
                                break;
                            }
                        }
                        if (inoutLine == null)
                        {
                            inoutLine = lines[0];	//	first as default
                            M_InOutLine_ID = inoutLine.GetM_InOutLine_ID();
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
                        //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
                        //if (countVA038 > 0)
                        //{

                        //    if (Util.GetValueOfInt(inoutLine.GetM_Product_ID()) > 0)
                        //    {
                        //        MProduct pro = new MProduct(ctx, inoutLine.GetM_Product_ID(), null);
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
                        //    if (Util.GetValueOfInt(inoutLine.GetC_Charge_ID()) > 0)
                        //    {
                        //        MCharge charge = new MCharge(ctx, inoutLine.GetC_Charge_ID(), null);
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
                    invoiceLine.SetClientOrg(orderLine.GetAD_Client_ID(), orderLine.GetAD_Org_ID());
                    if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                    {
                        //invoiceLine.setQtyInvoiced(QtyEntered
                        //    .multiply(orderLine.getQtyOrdered())
                        //    .divide(orderLine.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        invoiceLine.SetQtyInvoiced(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered,
                        orderLine.GetQtyOrdered()),
                        orderLine.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                    }

                    if(Env.IsModuleInstalled("VA077_"))
                    {
                        invoiceLine.Set_Value("VA077_CNAutodesk", orderLine.Get_Value("VA077_CNAutodesk"));
                        invoiceLine.Set_Value("VA077_Duration", orderLine.Get_Value("VA077_Duration"));
                        invoiceLine.Set_Value("VA077_MarginAmt", orderLine.Get_Value("VA077_MarginAmt"));
                        invoiceLine.Set_Value("VA077_MarginPercent", orderLine.Get_Value("VA077_MarginPercent"));
                        invoiceLine.Set_Value("VA077_OldSN", orderLine.Get_Value("VA077_OldSN"));
                        invoiceLine.Set_Value("VA077_ProductInfo", orderLine.Get_Value("VA077_ProductInfo"));
                        invoiceLine.Set_Value("VA077_PurchasePrice", orderLine.Get_Value("VA077_PurchasePrice"));
                        invoiceLine.Set_Value("VA077_RegEmail", orderLine.Get_Value("VA077_RegEmail"));
                        invoiceLine.Set_Value("VA077_SerialNo", orderLine.Get_Value("VA077_SerialNo"));
                        invoiceLine.Set_Value("VA077_UpdateFromVersn", orderLine.Get_Value("VA077_UpdateFromVersn"));
                        invoiceLine.Set_Value("VA077_UserRef_ID", orderLine.Get_Value("VA077_UserRef_ID"));
                        invoiceLine.Set_Value("VA077_StartDate", orderLine.Get_Value("VA077_StartDate"));
                        invoiceLine.Set_Value("VA077_EndDate", orderLine.Get_Value("VA077_EndDate"));
                        invoiceLine.Set_Value("VA077_ServiceContract_ID", orderLine.Get_Value("VA077_ServiceContract_ID"));
                    }



                    // Change By mohit Amortization proces
                    //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
                    //if (countVA038 > 0)
                    //{
                    //    if (Util.GetValueOfInt(orderLine.GetM_Product_ID()) > 0)
                    //    {
                    //        MProduct pro = new MProduct(ctx, orderLine.GetM_Product_ID(), null);
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
                    //    if (Util.GetValueOfInt(orderLine.GetC_Charge_ID()) > 0)
                    //    {
                    //        MCharge charge = new MCharge(ctx, orderLine.GetC_Charge_ID(), null);
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
                        invoiceLine.SetClientOrg(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID());

                        if (Env.IsModuleInstalled("VA077_"))
                        {
                            invoiceLine.Set_Value("VA077_CNAutodesk", inoutLine.Get_Value("VA077_CNAutodesk"));
                            invoiceLine.Set_Value("VA077_Duration", inoutLine.Get_Value("VA077_Duration"));
                            invoiceLine.Set_Value("VA077_MarginAmt", inoutLine.Get_Value("VA077_MarginAmt"));
                            invoiceLine.Set_Value("VA077_MarginPercent", inoutLine.Get_Value("VA077_MarginPercent"));
                            invoiceLine.Set_Value("VA077_OldSN", inoutLine.Get_Value("VA077_OldSN"));
                            invoiceLine.Set_Value("VA077_ProductInfo", inoutLine.Get_Value("VA077_ProductInfo"));
                            invoiceLine.Set_Value("VA077_PurchasePrice", inoutLine.Get_Value("VA077_PurchasePrice"));
                            invoiceLine.Set_Value("VA077_RegEmail", inoutLine.Get_Value("VA077_RegEmail"));
                            invoiceLine.Set_Value("VA077_SerialNo", inoutLine.Get_Value("VA077_SerialNo"));
                            invoiceLine.Set_Value("VA077_UpdateFromVersn", inoutLine.Get_Value("VA077_UpdateFromVersn"));
                            invoiceLine.Set_Value("VA077_UserRef_ID", inoutLine.Get_Value("VA077_UserRef_ID"));
                            invoiceLine.Set_Value("VA077_StartDate", inoutLine.Get_Value("VA077_StartDate"));
                            invoiceLine.Set_Value("VA077_EndDate", inoutLine.Get_Value("VA077_EndDate"));
                            invoiceLine.Set_Value("VA077_ServiceContract_ID", inoutLine.Get_Value("VA077_ServiceContract_ID"));
                        }


                    }

                    invoiceLine.SetPrice();
                    invoiceLine.SetTax();
                    // Change By mohit Amortization proces
                    //int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
                    //if (countVA038 > 0)
                    //{
                    //    if (Util.GetValueOfInt(inoutLine.GetM_Product_ID()) > 0)
                    //    {
                    //        MProduct pro = new MProduct(ctx, inoutLine.GetM_Product_ID(), null);
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
                    //    if (Util.GetValueOfInt(inoutLine.GetC_Charge_ID()) > 0)
                    //    {
                    //        MCharge charge = new MCharge(ctx, inoutLine.GetC_Charge_ID(), null);
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
                    if (C_Order_ID == 0 && C_OrderLine_ID > 0)
                    {
                        MOrderLine ordLine = new MOrderLine(ctx, C_OrderLine_ID, null);
                        C_Order_ID = ordLine.GetC_Order_ID();
                        _order = new MOrder(ctx, C_Order_ID, null);
                        _invoice.SetC_Order_ID(C_Order_ID);
                        _invoice.SetDateOrdered(_order.GetDateOrdered());

                        // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

                        if (_invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                        {
                            _invoice.SetC_IncoTerm_ID(_order.GetC_IncoTerm_ID());
                        }
                        _invoice.Save();
                    }
                }

            }   //  for all rows

            return true;
        }

        /// <summary>
        /// Create the BankStatement Line from the header
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="model">List of details</param>
        /// <param name="selectedItems"></param>
        /// <param name="C_BankStatement_ID">C_BankStatement_ID</param>
        /// <returns>true of false which indicated true if success</returns>
        public bool SaveStatmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_BankStatement_ID)
        {
            MBankStatement bs = new MBankStatement(ctx, C_BankStatement_ID, null);

            int _bpartner_Id = 0;
            int pageno = 1;
            int lineno = 10;
            string _sql = null;
            DataSet _data = null;

            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                DateTime trxDate = Convert.ToDateTime(model[i]["Date"]);          //  1-DateTrx
                int C_Payment_ID = Convert.ToInt32(model[i]["C_Payment_ID_K"]);   //  2-C_Payment_ID
                //int C_Currency_ID = Convert.ToInt32(model[i]["C_Currency_ID_K"]); //  3-Currency
                //Decimal TrxAmt = Convert.ToDecimal(model[i]["Amount"]);           //  4-PayAmt
                int C_Currency_ID = Convert.ToInt32(model[i]["C_Currency_ID_K"]); //  3-Currency                
                Decimal TrxAmt = Convert.ToDecimal(model[i]["ConvertedAmount"]);           //  4-PayAmt
                string type = Util.GetValueOfString(model[i]["Type"]);
                MBankStatementLine bsl = new MBankStatementLine(bs);
                bsl.SetStatementLineDate(trxDate);

                #region Get Page And Line
                //fetch the Line and Page No from the Query
                _sql = @"SELECT MAX(BSL.VA012_PAGE) AS PAGE, MAX(BSL.LINE)+10  AS LINE FROM C_BANKSTATEMENTLINE BSL
                    WHERE BSL.VA012_PAGE=(SELECT MAX(BL.VA012_PAGE) AS PAGE FROM C_BANKSTATEMENTLINE BL WHERE BL.C_BANKSTATEMENT_ID =" + C_BankStatement_ID + @") 
                    AND BSL.C_BANKSTATEMENT_ID =" + C_BankStatement_ID;
                _data = DB.ExecuteDataset(_sql, null, null);
                if (_data != null && _data.Tables[0].Rows.Count > 0)
                {
                    pageno = Util.GetValueOfInt(_data.Tables[0].Rows[0]["PAGE"]);
                    lineno = Util.GetValueOfInt(_data.Tables[0].Rows[0]["LINE"]);
                }
                if (pageno <= 0)
                {
                    pageno = 1;
                }
                if (lineno <= 0)
                {
                    lineno = 10;
                }
                #endregion
                //Set PageNo and LineNo
                bsl.SetVA012_Page(pageno);
                bsl.SetLine(lineno);

                //bsl.SetPayment(new MPayment(ctx, C_Payment_ID, null));
                //MPayment pmt = new MPayment(ctx, C_Payment_ID, null);
                if (type == "P")
                {
                    bsl.SetC_Payment_ID(C_Payment_ID);
                    //Get C_BPartner_ID
                    _bpartner_Id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartner_ID FROM C_Payment WHERE IsActive='Y' AND C_Payment_ID=" + C_Payment_ID, null, null));
                }
                else
                {
                    bsl.SetC_CashLine_ID(C_Payment_ID);
                    //Get C_BPartner_ID
                    _bpartner_Id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartner_ID FROM C_CashLine WHERE IsActive='Y' AND C_CashLine_ID=" + C_Payment_ID, null, null));
                }

                //set C_BPartner_ID
                bsl.SetC_BPartner_ID(_bpartner_Id);
                bsl.SetC_Currency_ID(C_Currency_ID);
                bsl.SetTrxAmt(TrxAmt);
                bsl.SetStmtAmt(TrxAmt);
                //set Description
                bsl.SetDescription(Util.GetValueOfString(model[i]["Description"]));
                bsl.Set_Value("TrxNo", Util.GetValueOfString(model[i]["AuthCode"]).Trim());
                int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix='VA034_' AND IsActive='Y'"));
                if (count > 0)
                    bsl.SetVA012_VoucherNo(Util.GetValueOfString(model[i]["VA034_DepositSlipNo"]));

                //Set C_ConversionType_ID on Bank Statement Line
                bsl.Set_Value("C_ConversionType_ID", Util.GetValueOfInt(model[i]["C_ConversionType_ID"]));
                if (!bsl.Save())
                {
                    //s_log.log(Level.SEVERE, "Line not created #" + i);
                    //Used ValueNamePair to get Error
                    ValueNamePair pp = VLogger.RetrieveError();
                    //some times getting the error pp also
                    string error = pp != null ? pp.ToString() == null ? pp.GetValue() : pp.ToString() : "";
                    if (string.IsNullOrEmpty(error))
                    {
                        error = pp != null ? pp.GetName() : "";
                    }
                    ErrorMsg= !string.IsNullOrEmpty(error) ? error : "VIS.BankStatementLineNotCreated";
                    Logger.global.Info(ErrorMsg);
                    return false;
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
            public string C_Payment_ID { get; set; }
            public string C_Currency_ID { get; set; }
            public decimal Amount { get; set; }
            public decimal ConvertedAmount { get; set; }
            public string Description { get; set; }
            public string C_BPartner_ID { get; set; }
            public string Type { get; set; }
            public int C_Payment_ID_K { get; set; }
            public int C_Currency_ID_K { get; set; }
            public string VA034_DepositSlipNo { get; set; }
            public string AuthCode { get; set; }
            public string CheckNo { get; set; }
            public int C_ConversionType_ID { get; set; }
        }
        public class DataObject
        {
            public int recid { get; set; }
            public bool Select { get; set; }
            public decimal Quantity { get; set; }
            public decimal QuantityPending { get; set; }
            public decimal QuantityEntered { get; set; }
            public string C_UOM_ID { get; set; }
            public string M_Product_ID { get; set; }
            public string C_Order_ID { get; set; }
            public string M_InOut_ID { get; set; }
            public string C_Invoice_ID { get; set; }
            public int C_UOM_ID_K { get; set; }
            public int M_Product_ID_K { get; set; }
            public int M_AttributeSetInstance_ID { get; set; }
            public int C_Order_ID_K { get; set; }
            public int M_InOut_ID_K { get; set; }
            public int C_Invoice_ID_K { get; set; }
            public string IsDropShip { get; set; } // Arpit Rai
            public int C_PaymentTerm_ID { get; set; }
            public string PaymentTermName { get; set; }
            public bool IsAdvance { get; set; }
            public int C_InvoicePaymentTerm_ID { get; set; }
            public bool IsInvoicePTAdvance { get; set; }
            public string M_Product_SearchKey { get; set; }
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
            //String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE IsSelected='Y'"
            //    + " AND AD_Client_ID=" + ctx.GetAD_Client_ID()
            //    + " AND AD_Org_ID=" + ctx.GetAD_Org_ID();

            //int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //log.Config("Reset=" + no); 

            //	Set Selection
            //sql = "UPDATE C_Order SET IsSelected = 'Y' WHERE " + whereClause;
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
            int AD_Process_ID = 134;  // HARDCODED    C_InvoiceCreate
            MPInstance instance = new MPInstance(ctx, AD_Process_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
                return Msg.GetMsg(ctx, "ProcessNoInstance");
            }

            ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

            pi.SetAD_Client_ID(ctx.GetAD_Client_ID());

            //	Add Parameters
            MPInstancePara para = new MPInstancePara(instance, 10);
            //para.setParameter("Selection", "Y");
            para.setParameter("Selection", "N");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MPInstancePara(instance, 20);
            para.setParameter("DocAction", "CO");
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MPInstancePara(instance, 30);
            para.setParameter("C_Order_ID", whereClause);
            if (!para.Save())
            {
                String msg = "No C_Order_ID Parameter added";  //  not translated
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

        public string GenerateShipments(Ctx ctx, string whereClause, string M_Warehouse_ID)
        {

            Trx trx = null;

            //	Reset Selection
            String sql = "UPDATE C_Order SET IsSelected = 'N' "
            + "WHERE IsSelected='Y'"
            + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //	Set Selection
            sql = "UPDATE C_Order SET IsSelected = 'Y' WHERE " + whereClause;
            no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));
            if (no == 0)
            {
                String msg = "No Shipments";     //  not translated!
                lblStatusInfo = msg.ToString();
            }
            lblStatusInfo = Msg.GetMsg(ctx, "InOutGenerateGen");
            statusBar += no.ToString();

            //	Prepare Process
            int AD_Process_ID = 199;	  // M_InOutCreate - Vframwork.Process.InOutGenerate
            MPInstance instance = new MPInstance(ctx, AD_Process_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
            }
            ProcessInfo pi = new ProcessInfo("VInOutGen", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

            pi.SetAD_Client_ID(ctx.GetAD_Client_ID());

            //	Add Parameter - Selection=Y
            MPInstancePara para = new MPInstancePara(instance, 10);
            para.setParameter("Selection", "Y");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
            }
            //	Add Parameter - M_Warehouse_ID=x
            para = new MPInstancePara(instance, 20);
            para.setParameter("M_Warehouse_ID", Util.GetValueOfInt(M_Warehouse_ID));
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
            String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE " + whereClause;
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
            String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE " + whereClause;
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
            MClient client = null;
            decimal qty = 0;
            bool success = false;
            int M_InOutLine_ID = 0;
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
                        M_InOutLine_ID = LineMatched;      //  upper table
                        Line_ID = Util.GetValueOfInt(data[i].Line_K);
                    }
                    else
                    {
                        M_InOutLine_ID = Util.GetValueOfInt(data[i].Line_K); ;    //  lower table
                        Line_ID = LineMatched;
                    }

                    MInOutLine sLine = new MInOutLine(ctx, M_InOutLine_ID, trx);
                    MInOut ship = new MInOut(ctx, sLine.GetM_InOut_ID(), trx);
                    if (invoice)	//	Shipment - Invoice
                    {
                        //	Update Invoice Line
                        MInvoiceLine iLine = new MInvoiceLine(ctx, Line_ID, trx);
                        MInvoice inv = new MInvoice(ctx, iLine.GetC_Invoice_ID(), trx);
                        iLine.SetM_InOutLine_ID(M_InOutLine_ID);
                        if (sLine.GetC_OrderLine_ID() != 0)
                            iLine.SetC_OrderLine_ID(sLine.GetC_OrderLine_ID());
                        if (!iLine.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            msg += "Error: " + pp != null ? pp.GetName() : "";
                            success = false;
                        }
                        else
                        {
                            //client = MClient.Get(ctx, Util.GetValueOfInt(inv.GetAD_Client_ID()));

                            //	Create Shipment - Invoice Link
                            if (iLine.GetM_Product_ID() != 0)
                            {
                                MMatchInv match = new MMatchInv(iLine, inv.GetDateInvoiced(), qty);
                                match.Set_ValueNoCheck("C_BPartner_ID", inv.GetC_BPartner_ID());
                                match.SetM_InOutLine_ID(M_InOutLine_ID);
                                if (match.Save())
                                {
                                    MatchedInv_ID = match.GetDocumentNo();
                                    success = true;
                                    // Check applied by mohit asked by ravikant to restrict the recalcualtion of costing for invoice for which the costing is already calculated.06/07/2017 PMS TaskID=4170
                                    if (iLine.GetC_OrderLine_ID() == 0 && !iLine.IsCostCalculated())
                                    {
                                        // updated by Amit 31-12-2015
                                        MProduct product = new MProduct(ctx, match.GetM_Product_ID(), trx);

                                        // Not returning any value as No effect
                                        MCostQueue.CreateProductCostsDetails(ctx, match.GetAD_Client_ID(), match.GetAD_Org_ID(), product,
                                             match.GetM_AttributeSetInstance_ID(), "Match IV", null, sLine, null, iLine, null,
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
                            if (iLine.GetC_OrderLine_ID() != 0 && iLine.GetM_Product_ID() != 0)
                            {
                                MMatchPO matchPO = MMatchPO.Create(iLine, sLine, inv.GetDateInvoiced(), qty);
                                matchPO.Set_ValueNoCheck("C_BPartner_ID", inv.GetC_BPartner_ID());
                                matchPO.SetC_InvoiceLine_ID(iLine);
                                matchPO.SetM_InOutLine_ID(M_InOutLine_ID);
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
                                    MProduct product = new MProduct(ctx, matchPO.GetM_Product_ID(), trx);

                                    // Not returning any value as No effect
                                    MCostQueue.CreateProductCostsDetails(ctx, matchPO.GetAD_Client_ID(), matchPO.GetAD_Org_ID(), product,
                                          matchPO.GetM_AttributeSetInstance_ID(), "Match IV", null, sLine, null, iLine, null,
                                          Decimal.Multiply(Decimal.Divide(iLine.GetLineNetAmt(), iLine.GetQtyInvoiced()), matchPO.GetQty()),
                                          matchPO.GetQty(), trx, out conversionNotFoundMatch, "window");
                                }
                            }
                        }
                    }
                    else	//	Shipment - Order
                    {
                        //	Update Shipment Line
                        sLine.SetC_OrderLine_ID(Line_ID);
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
                                    //   log.Severe("QtyReserved not updated - C_OrderLine_ID=" + Line_ID);
                                }
                            }

                            //client = MClient.Get(ctx, Util.GetValueOfInt(sLine.GetAD_Client_ID()));

                            //	Create PO - Shipment Link
                            if (sLine.GetM_Product_ID() != 0)
                            {
                                MMatchPO match = new MMatchPO(sLine, ship.GetMovementDate(), qty);
                                match.Set_ValueNoCheck("C_BPartner_ID", ship.GetC_BPartner_ID());
                                if (Util.GetValueOfInt(DB.ExecuteScalar("select count(*) from ad_column where columnname like 'IsMatchPOForm'", null, trx)) > 0)
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
                                    MProduct product = new MProduct(ctx, match.GetM_Product_ID(), trx);

                                    // Not returning any value as No effect
                                    MCostQueue.CreateProductCostsDetails(ctx, match.GetAD_Client_ID(), match.GetAD_Org_ID(), product, match.GetM_AttributeSetInstance_ID(),
                                        "Match PO", null, sLine, null, null, null, oLine.GetC_OrderLine_ID(), match.GetQty(), trx, out conversionNotFoundMatch, "window");
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
                                        success = MStorage.Add(ctx, sLine.GetM_Warehouse_ID(),
                                            sLine.GetM_Locator_ID(),
                                            sLine.GetM_Product_ID(),
                                            sLine.GetM_AttributeSetInstance_ID(), oLine.GetM_AttributeSetInstance_ID(),
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
        public MAcctSchema[] ASchemas = null;
        public MAcctSchema ASchema = null;

        /// <summary>
        /// Get Account Viewer Data
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Client_ID">AD_Client_ID</param>
        /// <param name="whereClause">Where Clause</param>
        /// <param name="orderClause">Order Clause</param>
        /// <param name="gr1">group 1</param>
        /// <param name="gr2">group 2</param>
        /// <param name="gr3">group 3</param>
        /// <param name="gr4">group 4</param>
        /// <param name="sort1">sort 1</param>
        /// <param name="sort2">sort 2</param>
        /// <param name="sort3">sort 3</param>
        /// <param name="sort4">sort 4</param>
        /// <param name="displayDocInfo">display Doc Info</param>
        /// <param name="displaySrcAmt">Display Source amount</param>
        /// <param name="displayqty">displacy Quantity</param>
        /// <returns>List</returns>
        public AccountViewClass GetDataQuery(Ctx ctx, int AD_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4,
            String sort1, String sort2, String sort3, String sort4, bool displayDocInfo, bool displaySrcAmt, bool displayqty)
        {
            return GetDataQuery(ctx, AD_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4,
             sort1, sort2, sort3, sort4, displayDocInfo, displaySrcAmt, displayqty, 0);
        }

        /// <summary>
        /// Get Account Viewer Data
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Client_ID">AD_Client_ID</param>
        /// <param name="whereClause">Where Clause</param>
        /// <param name="orderClause">Order Clause</param>
        /// <param name="gr1">group 1</param>
        /// <param name="gr2">group 2</param>
        /// <param name="gr3">group 3</param>
        /// <param name="gr4">group 4</param>
        /// <param name="sort1">sort 1</param>
        /// <param name="sort2">sort 2</param>
        /// <param name="sort3">sort 3</param>
        /// <param name="sort4">sort 4</param>
        /// <param name="displayDocInfo">display Doc Info</param>
        /// <param name="displaySrcAmt">Display Source amount</param>
        /// <param name="displayqty">displacy Quantity</param>
        /// <param name="pageNo">Page No</param>
        /// <returns>List</returns>
        public AccountViewClass GetDataQuery(Ctx ctx, int AD_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4,
        String sort1, String sort2, String sort3, String sort4, bool displayDocInfo, bool displaySrcAmt, bool displayqty, int pageNo)
        {
            group1 = gr1; group2 = gr2; group3 = gr3; group4 = gr4;
            sortBy1 = sort1; sortBy2 = sort2; sortBy3 = sort3; sortBy4 = sort4;
            ASchemas = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
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

            rm.Query(ctx, whereClause.ToString(), orderClause.ToString(), pageNo);

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
            if (obj.Data != null)
            {
                StringBuilder sql = new StringBuilder(@"SELECT COUNT(Fact_Acct_ID) AS TotalRecord, 
                                    NVL(SUM(AMTACCTCR), 0) AS AMTACCTCR  , NVL(SUM(AMTACCTDR), 0) AS AMTACCTDR ");
                sql.Append(" FROM ").Append(rm.GetTableName()).Append(" ").Append(RModel.TABLE_ALIAS);
                if (whereClause != null && whereClause.Length > 0)
                {
                    sql.Append(" WHERE ").Append(whereClause);
                }
                String finalSQL = MRole.GetDefault(ctx, false).AddAccessSQL(
                    sql.ToString(), RModel.TABLE_ALIAS, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                if (orderClause != null && orderClause.Length > 0)
                {
                    finalSQL += " ORDER BY " + orderClause;
                }

                DataSet dsFactAcct = DB.ExecuteDataset(sql.ToString());
                if (dsFactAcct != null && dsFactAcct.Tables.Count > 0 && dsFactAcct.Tables[0].Rows.Count > 0)
                {
                    int totalRec = Util.GetValueOfInt(dsFactAcct.Tables[0].Rows[0]["TotalRecord"]);
                    decimal amtAcctCr = Util.GetValueOfDecimal(dsFactAcct.Tables[0].Rows[0]["AMTACCTCR"]);
                    decimal amtAcctDr = Util.GetValueOfDecimal(dsFactAcct.Tables[0].Rows[0]["AMTACCTDR"]);
                    obj.DebitandCredit = Msg.GetMsg(ctx, "TotalDRandCR") + DisplayType.GetNumberFormat(DisplayType.Amount).GetFormatAmount(amtAcctDr, ctx.GetContext("#ClientLanguage"))
                        + " / " + DisplayType.GetNumberFormat(DisplayType.Amount).GetFormatAmount(amtAcctCr, ctx.GetContext("#ClientLanguage"));
                    int pageSize = 50;
                    PageSetting pSetting = new PageSetting();
                    pSetting.CurrentPage = pageNo;
                    pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                    obj.pSetting = pSetting;
                }
            }
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
            RModel rm = new RModel("Fact_Acct");
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
                rm.AddColumn(new RColumn(ctx, "C_Currency_ID", DisplayType.TableDir));
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
            if (!keys.Contains("C_Period_ID"))
            {
                rm.AddColumn(new RColumn(ctx, "C_Period_ID", DisplayType.TableDir));
            }
            if (displayQty)
            {
                rm.AddColumn(new RColumn(ctx, "C_UOM_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Qty", DisplayType.Quantity));
            }
            if (displayDocumentInfo)
            {
                rm.AddColumn(new RColumn(ctx, "AD_Table_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Record_ID", DisplayType.ID));
                rm.AddColumn(new RColumn(ctx, "Description", DisplayType.String));
            }
            if (PostingType == null || PostingType.Length == 0)
            {
                rm.AddColumn(new RColumn(ctx, "PostingType", DisplayType.List,
                    MFactAcct.POSTINGTYPE_AD_Reference_ID));
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
            MAcctSchemaElement[] elements = ASchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
                {
                    _leadingColumns = columns.Count;
                }
                //
                MAcctSchemaElement ase = elements[i];
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
            if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
            {
                _leadingColumns = columns.Count;
            }
            return columns;
        }

        #endregion

        #region Archive Viewer

        public bool UpdateArchive(Ctx ctx, string name, string des, string help, int archiveId)
        {
            MArchive ar = new MArchive(ctx, archiveId, null);//  m_archives[m_index];
            ar.SetName(name);
            ar.SetDescription(des);
            ar.SetHelp(help);
            if (ar.Save())
            {
                return true;
            }
            return false;
        }

        public string DownloadPdf(Ctx _ctx, int archiveId)
        {
            MArchive ar = new MArchive(_ctx, archiveId, null);//  m_archives[m_index];
            MSession sess = MSession.Get(_ctx);

            //Save Action Log
            VAdvantage.Common.Common.SaveActionLog(_ctx, MActionLog.ACTION_Form, "Archive Viewer", ar.GetAD_Table_ID(), ar.GetRecord_ID(), 0, "", "", "Report Downloaded:->" + ar.GetName(), MActionLog.ACTIONTYPE_Download);

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
        //public List<Dictionary<string, object>> GetProcess(int AD_Role_ID)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    string sql = "SELECT DISTINCT p.AD_Process_ID, p.Name "
        //            + "FROM AD_Process p INNER JOIN AD_Process_Access pa ON (p.AD_Process_ID=pa.AD_Process_ID) "
        //            + "WHERE pa.AD_Role_ID=" + AD_Role_ID
        //            + " AND p.IsReport='Y' AND p.IsActive='Y' AND pa.IsActive='Y' "
        //            + "ORDER BY 2";
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["AD_Process_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Process_ID"]);
        //            obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
        //            retDic.Add(obj);
        //        }
        //    }
        //    return retDic;
        //}

        // Added by Bharat on 07 June 2017
        //public List<Dictionary<string, object>> GetTable(int AD_Role_ID)
        //{
        //    List<Dictionary<string, object>> retDic = null;
        //    string sql = "SELECT DISTINCT t.AD_Table_ID, t.Name "
        //        + "FROM AD_Table t INNER JOIN AD_Tab tab ON (tab.AD_Table_ID=t.AD_Table_ID)"
        //        + " INNER JOIN AD_Window_Access wa ON (tab.AD_Window_ID=wa.AD_Window_ID) "
        //        + "WHERE wa.AD_Role_ID=" + AD_Role_ID
        //        + " AND t.IsActive='Y' AND tab.IsActive='Y' "
        //        + "ORDER BY 2";
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["AD_Table_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"]);
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
        //    string sql = "SELECT AD_User_ID, Name "
        //        + "FROM AD_User u WHERE EXISTS "
        //            + "(SELECT * FROM AD_User_Roles ur WHERE u.AD_User_ID=ur.AD_User_ID) "
        //        + "ORDER BY 2";
        //    sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_User", MRole.SQL_NOTQUALIFIED, MRole.SQL_RW);
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        retDic = new List<Dictionary<string, object>>();
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            Dictionary<string, object> obj = new Dictionary<string, object>();
        //            obj["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_User_ID"]);
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
        //            obj["CREATEDBY"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT Name FROM AD_User WHERE AD_User_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["CREATEDBY"])));
        //            obj["CREATED"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["CREATED"]);
        //            obj["AD_ARCHIVE_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_ARCHIVE_ID"]);
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
                    obc.C_UOM_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    obc.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    obc.M_AttributeSetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    obc.M_InOut_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InOut_ID"]);
                    obc.C_Invoice_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Invoice_ID"]);
                    obc.C_UOM_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID_K"]);
                    obc.M_Product_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID_K"]);
                    obc.M_AttributeSetInstance_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID_K"]);
                    obc.C_Order_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID_K"]);
                    obc.M_InOut_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InOut_ID_K"]);
                    obc.C_Invoice_ID_K = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Invoice_ID_K"]);
                    obj.Add(obc);
                }
            }
            return obj;
        }


        public int GetOrderDataCommonsNotOrg(Ctx ctx, int M_Product_ID_Ks)
        {
            string st = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID_Ks;

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
                                                                     + "    FROM C_PERIOD P  "
                                                                     + "    INNER JOIN C_YEAR Y                "
                                                                     + "    ON P.C_YEAR_ID    =Y.C_YEAR_ID     "
                                                                     + "    WHERE P.PERIODNO  ='1'             "
                                                                     + "    AND P.C_YEAR_ID   = " + YearId
                                                                     + "    AND Y.AD_CLIENT_ID= " + AdClientID));

                    eDate = Util.GetValueOfDateTime(DB.ExecuteScalar(" SELECT P.ENDDATE AS ENDDATE    "
                                                                     + "    FROM C_PERIOD P  "
                                                                     + "    INNER JOIN C_YEAR Y                "
                                                                     + "    ON P.C_YEAR_ID    =Y.C_YEAR_ID     "
                                                                     + "    WHERE P.PERIODNO  ='12'             "
                                                                     + "    AND P.C_YEAR_ID   = " + YearId
                                                                     + "    AND Y.AD_CLIENT_ID= " + AdClientID));

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
        public Dictionary<string, object> GetCurrencyFromAccountingSchema(Ctx ctx, string fields)
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
                    CurrencyID = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT C_CURRENCY_ID "
                                                                      + "  FROM C_ACCTSCHEMA "
                                                                      + "  WHERE C_ACCTSCHEMA_ID = " + AccountingSchemaId
                                                                      + "  AND AD_CLIENT_ID     = " + AdClientID));



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
        /// Check whether table is deletable on AD_Table window
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="TableName"></param>
        /// <returns>returns Y or N </returns>
        public string CheckTableDeletable(Ctx ctx, string TableName)
        {
            return Util.GetValueOfString(DB.ExecuteScalar("Select IsDeleteable from AD_Table WHERE TableName = '" + TableName + "'"));
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
            PO _po = MTable.GetPO(ctx, TableName, Record_ID, null);
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
                MTable tbl = new MTable(ctx, rowData.AD_Table_ID, null);

                StringBuilder sbSql = new StringBuilder("SELECT COUNT(AD_Table_ID) FROM AD_Table WHERE TableName = '" + rowData.TableName + "_Ver'");

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

        /// <summary>
        /// Get Version information for changed columns
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="od"></param>
        /// <returns></returns>
        public dynamic GetVerDetails(Ctx ctx, dynamic od)
        {
            List<string> colNames = new List<string>();
            List<string> dbColNames = new List<string>();
            List<object> oldValues = new List<object>();
            // create list of default columns for which Version change details not required
            List<string> defColNames = new List<string>(new string[] { "Created", "CreatedBy", "Updated", "UpdatedBy", "Export_ID" });
            dynamic data = new ExpandoObject();
            string TableName = od.TName.Value;
            // Get original table name by removing "_Ver" suffix from the end
            string origTableName = TableName.Substring(0, TableName.Length - 4);
            var RecID = od.RID.Value;
            // Get parent table ID
            int AD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName = '" + origTableName + "'", null, null));
            MTable tbl = new MTable(ctx, AD_Table_ID, null);
            DataSet dsColumns = null;
            // check if Maintain Version is marked on table
            if (tbl.IsMaintainVersions())
                dsColumns = DB.ExecuteDataset("SELECT Name, ColumnName, AD_Column_ID, AD_Reference_ID FROM AD_Column WHERE AD_Table_ID = " + AD_Table_ID);
            // else get columns on which maintain version is marked
            else
                dsColumns = DB.ExecuteDataset("SELECT Name, ColumnName, AD_Column_ID, AD_Reference_ID FROM AD_Column WHERE AD_Table_ID = " + AD_Table_ID + " AND IsMaintainVersions = 'Y'");
            // return if maintain version not found either on table or column level
            if (!(dsColumns != null && dsColumns.Tables[0].Rows.Count > 0))
                return data;

            DataSet dsFields = DB.ExecuteDataset("SELECT Name, AD_Column_ID FROM AD_Field WHERE AD_Tab_ID = " + Util.GetValueOfInt(od.TabID.Value));
            StringBuilder sbSQL = new StringBuilder("");
            // check if table has single key
            if (tbl.IsSingleKey())
                sbSQL.Append(origTableName + "_ID = " + od[(origTableName + "_ID").ToLower()].Value);
            else
            {
                string[] keyCols = tbl.GetKeyColumns();
                for (int w = 0; w < keyCols.Length; w++)
                {
                    if (w == 0)
                    {
                        if (keyCols[w] != null)
                            sbSQL.Append(keyCols[w] + " = " + od[(keyCols[w]).ToLower()]);
                        else
                            sbSQL.Append(" NVL(" + keyCols[w] + ",0) = 0");
                    }
                    else
                    {
                        if (keyCols[w] != null)
                            sbSQL.Append(" AND " + keyCols[w] + " = " + od[(keyCols[w]).ToLower()]);
                        else
                            sbSQL.Append(" AND NVL(" + keyCols[w] + ",0) = 0");
                    }
                }
            }

            POInfo inf = POInfo.GetPOInfo(ctx, Util.GetValueOfInt(od.TblID.Value));
            // Get SQL Query from PO Info for selected table
            string sqlCol = inf.GetSQLQuery();

            //DataSet dsRec = DB.ExecuteDataset("SELECT * FROM " + TableName + " WHERE " + sbSQL.ToString() + " AND RecordVersion < " + od["recordversion"].Value + " ORDER BY RecordVersion DESC");
            //DataSet dsRec = DB.ExecuteDataset("SELECT * FROM " + TableName + " WHERE " + sbSQL.ToString() + " AND RecordVersion = " + Util.GetValueOfInt(od["oldversion"].Value));
            // get data from Version table according to the Record Version 
            DataSet dsRec = DB.ExecuteDataset(sqlCol + " WHERE " + sbSQL.ToString() + " AND RecordVersion = " + Util.GetValueOfInt(od["oldversion"].Value));
            DataRow dr = null;
            if (dsRec != null && dsRec.Tables[0].Rows.Count > 0)
                dr = dsRec.Tables[0].Rows[0];

            StringBuilder sbColName = new StringBuilder("");
            StringBuilder sbColValue = new StringBuilder("");
            for (int i = 0; i < dsColumns.Tables[0].Rows.Count; i++)
            {
                sbColName.Clear();
                sbColValue.Clear();
                sbColName.Append(Util.GetValueOfString(dsColumns.Tables[0].Rows[i]["ColumnName"]));
                if (!dr.Table.Columns.Contains(sbColName.ToString()))
                    continue;

                if (defColNames.Contains(sbColName.ToString()) || (sbColName.ToString() == origTableName + "_ID") || (sbColName.ToString() == origTableName + "_Ver_ID"))
                    continue;

                if (Util.GetValueOfInt(dsColumns.Tables[0].Rows[i]["AD_Reference_ID"]) == 20)
                    dr[sbColName.ToString()] = (Util.GetValueOfString(dr[sbColName.ToString()]) == "Y") ? true : false;

                var val = od[sbColName.ToString().ToLower()];
                if (val != null)
                {
                    if (Util.GetValueOfString(dr[sbColName.ToString()]) != Util.GetValueOfString(od[sbColName.ToString().ToLower()].Value))
                    {
                        colNames.Add(Util.GetValueOfString(dsColumns.Tables[0].Rows[i]["Name"]));
                        dbColNames.Add(sbColName.ToString());
                        // get text for column based on different reference types
                        if (dr.Table.Columns.Contains(sbColName.ToString() + "_TXT"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_TXT"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_LOC"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_LOC"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_LTR"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_LTR"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_ASI"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_ASI"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_ACT"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_ACT"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_CTR"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_CTR"]));
                        else
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString()]));

                        oldValues.Add(sbColValue.ToString());
                    }
                }
            }

            data.ColumnNames = colNames;
            data.OldVals = oldValues;
            data.DBColNames = dbColNames;

            return data;
        }
    }

    public class AccountViewClass
    {
        public List<String> Columns { get; set; }
        public List<List<object>> Data { get; set; }
        public CommonModel.PageSetting pSetting { get; internal set; }
        public string DebitandCredit { get; set; }
    }

    public class GetOrderDataCommonsProperties
    {
        public int Quantity { get; set; }
        public int QuantityEntered { get; set; }
        public int C_UOM_ID { get; set; }
        public int M_Product_ID { get; set; }
        public int M_AttributeSetInstance_ID { get; set; }
        public int M_InOut_ID { get; set; }
        public int C_Invoice_ID { get; set; }
        public int C_UOM_ID_K { get; set; }
        public int M_Product_ID_K { get; set; }
        public int M_AttributeSetInstance_ID_K { get; set; }
        public int C_Order_ID_K { get; set; }
        public int M_InOut_ID_K { get; set; }
        public int C_Invoice_ID_K { get; set; }

    }
}