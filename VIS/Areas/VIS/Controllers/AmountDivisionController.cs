using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class AmountDivisionController : Controller
    {
        AmountDivisionModel model = new AmountDivisionModel();

        // GET: /VIS/AmountDivision/
        public ActionResult Index()
        {
            return View();
        }


        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetDiminsionType()
        {

            return Json(JsonConvert.SerializeObject(model.GetDimensionType(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }
        //[AjaxAuthorizeAttribute]
        //[AjaxSessionFilterAttribute]
        //[HttpPost]
        //[ValidateInput(false)]
        //public JsonResult InsertDimensionAmount(string acctSchema,string elementType, decimal amount, string dimensionLine, int DimAmtId)
        //{
        //    List<AmountDivisionModel> dim = JsonConvert.DeserializeObject<List<AmountDivisionModel>>(dimensionLine);
        //    int[] accountSchema = JsonConvert.DeserializeObject<int[]>(acctSchema);
        //    var temp = accountSchema.Distinct().ToArray();
        //    accountSchema = temp;
        //    string[] dimensionElement = JsonConvert.DeserializeObject<string[]>(elementType);
        //    return Json(JsonConvert.SerializeObject(model.InsertDimensionAmount(accountSchema, dimensionElement, amount, dim, Session["ctx"] as Ctx, DimAmtId)), JsonRequestBehavior.AllowGet);
        //}

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetDiminsionLine(string accountingSchema, int dimensionID, int DimensionLineID = 0, int pageNo = 0, int pSize = 0)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(accountingSchema);
            return Json(JsonConvert.SerializeObject(model.GetDimensionLine(Session["ctx"] as Ctx, accountSchema, dimensionID, DimensionLineID, pageNo, pSize)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert Line on Amount Dimension
        /// </summary>
        /// <param name="recordId">Amount Dimension ID</param>
        /// <param name="totalAmount">Total Amount</param>
        /// <param name="lineAmount">Line Amount</param>
        /// <param name="acctSchemaID">selected Accounting Schema</param>
        /// <param name="elementTypeID">Element Type</param>
        /// <param name="dimensionValue">Dimension Value</param>
        /// <param name="elementID">Element ID</param>
        /// <param name="oldDimensionName">Old Dimension Value</param>
        /// <param name="bpartner_ID">Business Partner</param>
        /// <param name="oldBPartner_ID"Old Business Partner></param>
        /// <returns>Data in JSON format</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult InsertDimensionLine(int recordId, decimal totalAmount, decimal lineAmount, string acctSchemaID, string elementTypeID, int dimensionValue, int elementID, int oldDimensionName, int bpartner_ID, int oldBPartner_ID)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(acctSchemaID);
            return Json(JsonConvert.SerializeObject(model.InsertDimensionLine(Session["ctx"] as Ctx, recordId, totalAmount, lineAmount, accountSchema, elementTypeID, dimensionValue, elementID, oldDimensionName, bpartner_ID, oldBPartner_ID)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetAccountingSchema(int OrgID)
        {

            return Json(JsonConvert.SerializeObject(model.GetAccountingSchema(Session["ctx"] as Ctx, OrgID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Element linked with Account on Accounting Schema Element
        /// </summary>
        /// <param name="accountingSchema">selected Accounting Schema</param>
        /// <returns>Element ID</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetElementID(string accountingSchema)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(accountingSchema);
            return Json(JsonConvert.SerializeObject(model.GetElementID(Session["ctx"] as Ctx, accountSchema)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get the dataset of Excel Records
        /// </summary>
        /// <param name="param">Name of the file</param>
        /// <returns>object of dataset</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetDataFromExcelOrText(List<object> param)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            DataSet _ds = null;
            DataTable dt = new DataTable();
            if (Path.GetExtension(param[0].ToString()) == ".xlsx" || Path.GetExtension(param[0].ToString()) == ".xls")
            {
                int Id = 0;
                VAdvantage.ImpExp.ExcelReader reader = new VAdvantage.ImpExp.ExcelReader();
                _ds = ImportExcelXLS(param[0].ToString(), false);
                dt = _ds.Tables[0];
                dt.Columns.Add("Record_ID", typeof(int));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                decimal totalAmt = 0.0M;
                decimal docAmount = Util.GetValueOfDecimal(param[3]);
                if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Activity))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("C_Activity_ID", "C_Activity", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_BPartner))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("C_Bpartner_ID", "C_Bpartner", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Campaign))
                {
                    dt.Columns.Add("Campaign_ID", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("C_Campaign_ID", "C_Campaign", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Organization) || param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_OrgTrx))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("AD_Org_ID", "AD_Org", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Project))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("C_Project_ID", "C_Project", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Product))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("M_Product_ID", "M_Product", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.DimnesionValue("C_SalesRegion_ID", "C_SalesRegion", dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
               else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_Account) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserList1) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserList2))
                {
                    int bpid = 0;
                    dt.Columns.Add("AccoutId", typeof(int));
                    dt.Columns.Add("C_Element_ID", typeof(int));
                    dt.Columns.Add("AccountValue", typeof(String));
                    dt.Columns.Add("BPartnerId", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            dt = model.GetAcountIdByValue((Util.GetValueOfInt(param[2]) == 0 ? ctx.GetContextAsInt("$C_AcctSchema_ID") : Util.GetValueOfInt(param[2])) , param[1].ToString(), dt.Rows[i][0].ToString(), i, dt);
                            bpid = model.DimnesionValue("C_Bpartner_ID", "C_Bpartner", dt.Rows[i][1].ToString());
                            dt.Rows[i]["BPartnerId"] = bpid;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
               else if (param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement1) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement2) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement3) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement4) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement5) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement6) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement7) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement8) ||
                    param[1].ToString().Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement9))
                {
                    string columName = "";
                    string tableName = "";
                    var sql = "select adt.columnname,adtab.TableName from c_acctschema_element ac inner join ad_column ad on ac.ad_column_id=ad.ad_column_id " +
                " inner join ad_column adt on ad.ad_table_ID=adt.ad_table_ID and adt.isactive='Y' " +
                "  inner join ad_table adtab on adtab.ad_table_id=ad.ad_table_ID " +
                " where ac.c_acctschema_id=" + (Util.GetValueOfInt(param[2]) == 0 ? ctx.GetContextAsInt("$C_AcctSchema_ID") : Util.GetValueOfInt(param[2])) + " and ac.elementtype='" + param[1].ToString() + "' and adt.isidentifier='Y' order by adt.seqno ASC ";
                    DataSet dsColumnDetail = (DB.ExecuteDataset(sql, null, null));
                    if (dsColumnDetail != null && dsColumnDetail.Tables.Count > 0 && dsColumnDetail.Tables[0].Rows.Count > 0)
                    {
                        columName = Util.GetValueOfString(dsColumnDetail.Tables[0].Rows[0]["columnname"]);
                        tableName = Util.GetValueOfString(dsColumnDetail.Tables[0].Rows[0]["TableName"]);
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (docAmount < 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) > 0)
                        {
                            continue;
                        }
                        else if (docAmount > 0 && Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) < 0)
                        {
                            continue;
                        }
                        else if (Util.GetValueOfDecimal(dt.Rows[i]["Amount"]) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            Id = model.UserElementDimnesionValue(columName, tableName, dt.Rows[i][1].ToString());
                            dt.Rows[i]["Record_ID"] = Id;
                            totalAmt = totalAmt + Util.GetValueOfDecimal(dt.Rows[i]["Amount"]);
                        }
                    }
                }
                dt.Rows[0]["TotalAmount"] = totalAmt;
            }

            var jRes = Json(JsonConvert.SerializeObject(new { result = dt }), JsonRequestBehavior.AllowGet);
            jRes.MaxJsonLength = int.MaxValue;
            return jRes;
        }

        /// <summary>
        /// Get the File details
        /// </summary>
        /// <param name="file">value of the file</param>
        /// <param name="fileName">Name of the filename</param>
        /// <param name="folderKey">Path of the file</param>
        /// <param name="orgFileName">Value of TempDownload path</param>
        /// <returns>object of UploadResponse class</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult SaveFileinTemp(HttpPostedFileBase file, string fileName, string folderKey, string orgFileName)
        {
            UploadResponse _obj = new UploadResponse();

            try
            {
                if (!Directory.Exists(Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload")))
                {
                    Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload"));
                }
                //if(File)
                HttpPostedFileBase hpf = file as HttpPostedFileBase;
                string fName = fileName;
                string savedFileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", Path.GetFileName(fileName));
                MemoryStream ms = new MemoryStream();
                hpf.InputStream.CopyTo(ms);
                byte[] byteArray = ms.ToArray();

                if (System.IO.File.Exists(savedFileName))//Append Content In File
                {
                    using (FileStream fs = new FileStream(savedFileName, FileMode.Append, System.IO.FileAccess.Write))
                    {

                        fs.Write(byteArray, 0, byteArray.Length);
                        ms.Close();
                    }

                }
                else // create new file
                {
                    using (FileStream fs = new FileStream(savedFileName, FileMode.Create, System.IO.FileAccess.Write))
                    {

                        fs.Write(byteArray, 0, byteArray.Length);
                        ms.Close();
                    }
                }


                _obj._path = savedFileName;
                _obj._filename = fName;
                _obj._orgfilename = orgFileName;
                //_obj._filename=f_name;
                return Json(_obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _obj._error = "ERROR:" + ex.Message;
                return Json(_obj, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Get the dataset of the Excel File Records
        /// </summary>
        /// <param name="FileName">value of the filename</param>
        /// <param name="hasHeaders">headers of the file</param>
        /// <returns>Object of the dataset</returns>
        public DataSet ImportExcelXLS(string FileName, bool hasHeaders)
        {
            string HDR = hasHeaders ? "Yes" : "No";
            string strConn;

            // check file extension
            if (FileName.Substring(FileName.LastIndexOf('.')).ToLower() == ".xlsx")
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
            }
            else
            {
                strConn = string.Format(@"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""", Path.GetDirectoryName(FileName));
            }

            DataSet output = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();
                    DataTable schemaTable = conn.GetOleDbSchemaTable(
                        OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    bool sheetsFound = false;
                    foreach (DataRow schemaRow in schemaTable.Rows)
                    {
                        sheetsFound = true;
                        string sheet = schemaRow["TABLE_NAME"].ToString();
                        if (!sheet.EndsWith("_"))
                        {
                            try
                            {
                                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                                cmd.CommandType = CommandType.Text;
                                DataTable outputTable = new DataTable(sheet);
                                output.Tables.Add(outputTable);
                                new OleDbDataAdapter(cmd).Fill(outputTable);
                            }
                            catch (Exception ex)
                            {

                                return null;
                            }
                        }
                    }
                    if (!sheetsFound)
                    {

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            return output;
        }
    }
}