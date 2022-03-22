using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using CrystalDecisions.CrystalReports.Engine;

using System.Data;
using System.IO;
using VAdvantage.DataBase;
using VAdvantage.Model;
//using System.Data.OracleClient;
using VAdvantage.Logging;
using VAdvantage.Print;
using VAdvantage.Classes;
using Oracle.ManagedDataAccess.Client;
using CrystalDecisions.Shared;
using VAModelAD.DataBase;

namespace VAdvantage.CrystalReport
{
    public class CrystalReportEngine : IReportEngine
    {
        private int AD_Process_ID = 0;
        private string reportName = "", imageField = "", imagePathField = "";
        private string SqlQuery = "";
        private bool isIncludeImage = false;
        private Ctx _ctx;
        private ProcessInfo _pi;
        private static VLogger log = VLogger.GetVLogger(typeof(CrystalReportEngine).FullName);

        public CrystalReportEngine()
        {

        }

        public CrystalReportEngine(Ctx ctx, ProcessInfo pi)
        {
            StartReport(ctx, pi, null);
        }


        public byte[] GenerateCrystalReport()
        {

            string reportPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Reports");
            string reportImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "");

            string[] reportNameArray;
            string[] reportPathArray;

            //string _ReportImagePath = "";
            //string _ReportPath = "";

            string path = reportPath;

            //_ReportImagePath = reportImagePath;

            if (String.IsNullOrEmpty(path))
            {
                throw new MissingFieldException("CrystalReportPathNotSet");

            }
            //if (reportName.IndexOf(";") > 0)
            //{
            //    if (reportName.IndexOf(":") < 0)
            //        reportPath = path + "\\" + reportName;
            //    else
            //        reportPath = reportName;
            //}
            //else
            //{
            //    reportNameArray = reportName.Split(';');
            //    if (reportNameArray.Length > 0)
            //    {
            //        reportPathArray = new string[reportNameArray.Length]; 

            //        for (int i = 0; i < reportNameArray.Length; i++)
            //        {
            //            if (reportNameArray[0].IndexOf(":") < 0)
            //                reportPathArray[i] = path + "\\" + reportNameArray[i];
            //            else
            //                reportPathArray[i] = reportNameArray[i];
            //        }
            //    }
            //}

            reportNameArray = reportName.Split(';');
            reportPathArray = new string[reportNameArray.Length];

            if (reportNameArray.Length > 0)
            {
                for (int i = 0; i < reportNameArray.Length; i++)
                {
                    if (reportNameArray[0].IndexOf(":") < 0)
                        reportPathArray[i] = path + "\\" + reportNameArray[i];
                    else
                        reportPathArray[i] = reportNameArray[i];
                }
            }


            ReportDocument rptBurndown = new ReportDocument();
            if (reportPathArray != null && reportPathArray.Length > 0 && File.Exists(reportPathArray[0]))   //Check if the crystal report file exists in a specified location.
            {
                try
                {
                    rptBurndown.Load(reportPathArray[0]);

                    //Set Connection Info
                    //ConnectionInfo.Get().SetAttributes(System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"]);


                    //Application will pick database info from the property file.
                    //CrystalDecisions.Shared.ConnectionInfo crDbConnection = new CrystalDecisions.Shared.ConnectionInfo();
                    //crDbConnection.IntegratedSecurity = false;
                    //crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                    //crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                    //crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                    //crDbConnection.Type = ConnectionInfoType.Unknown;
                    //crDbConnection.ServerName = ConnectionInfo.Get().Db_host;
                    CrystalDecisions.CrystalReports.Engine.Database crDatabase = rptBurndown.Database;
                    CrystalDecisions.Shared.TableLogOnInfo oCrTableLoginInfo;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table oCrTable in crDatabase.Tables)
                    {
                        //crDbConnection.IntegratedSecurity = false;
                        //crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                        //crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                        //crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                        //crDbConnection.Type = ConnectionInfoType.Unknown;
                        //crDbConnection.ServerName = ConnectionInfo.Get().Db_host;

                        oCrTableLoginInfo = oCrTable.LogOnInfo;
                        //oCrTableLoginInfo.ConnectionInfo = crDbConnection;
                        oCrTable.ApplyLogOnInfo(oCrTableLoginInfo);
                    }

                    //Create Parameter query

                    string[] sqls = SqlQuery.Split(';'); ;

                    string sql = sqls[0];

                    StringBuilder sb = new StringBuilder(" WHERE ");

                    if (_pi.GetRecord_ID() > 0 && _pi.GetTable_ID() > 0)
                    {
                        string tableName = DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_TABLE_ID =" + _pi.GetTable_ID()).ToString();
                        sb.Append(tableName).Append("_ID = ").Append(_pi.GetRecord_ID());
                    }

                    else
                    {
                        PO pos = null;
                        ProcessInfoUtil.SetParameterFromDB(_pi, _ctx);
                        ProcessInfoParameter[] parameters = _pi.GetParameter();
                        if (parameters.Count() > 0)
                        {
                            MTable table = MTable.Get(_ctx, "T_CrystalParameters");
                            pos = table.GetPO(_ctx, 0, null);
                            int loopCount = 0;
                            int paraCount = 1;
                            int incrementCount = 1;

                            string sqla = "SELECT ColumnName, IsRange, AD_Reference_ID FROM AD_Process_Para WHERE IsActive='Y' AND AD_Process_ID=" + _pi.GetAD_Process_ID() + " ORDER BY SEQNO ";
                            DataSet dsPara = DB.ExecuteDataset(sqla);
                            List<string> paraList = new List<string>();
                            if (dsPara != null && dsPara.Tables[0].Rows.Count > 0)
                            {
                                for (int c = 0; c < dsPara.Tables[0].Rows.Count; c++)
                                {
                                    //paraList.Add(dsPara.Tables[0].Rows[c]["ColumnName"].ToString().ToUpper());
                                    //paraRangeList.Add(dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper());
                                    var columnFound = false;
                                    for (int para = 0; para <= parameters.Count() - 1; para++)
                                    {
                                        string paramName = parameters[para].GetParameterName();

                                        if (!paramName.ToUpper().Equals(dsPara.Tables[0].Rows[c]["ColumnName"].ToString().ToUpper()))
                                        {
                                            continue;
                                        }
                                        columnFound = true;
                                        string sInfo = parameters[para].GetInfo();
                                        string sInfoTo = parameters[para].GetInfo_To();
                                        if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                                        {
                                            continue;
                                        }

                                        //if (loopCount > 0)
                                        //    sb.Append(" AND ");
                                        //string paramName = parameters[para].GetParameterName();
                                        object paramValue = parameters[para].GetParameter();
                                        object paramValueTo = parameters[para].GetParameter_To();

                                        //if parameter value is not null then only we need to append it in query.
                                        if (loopCount > 0 && (paramValue != null || paramValueTo != null))
                                        {
                                            sb.Append(" AND ");
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        if (paramValue is DateTime || paramValueTo is DateTime)
                                        {
                                            if (Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.Date)
                                            {
                                                if (paramValue != null && paramValueTo != null)
                                                {
                                                    sb.Append("TRUNC(" + paramName + ",'DD')").Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                    sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                }
                                                else if (paramValue != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" >= ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" = ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                    }
                                                }
                                                else if (paramValueTo != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" <= ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" = ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                    }
                                                }
                                            }
                                            else if (Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.DateTime ||
                                                Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.Time)
                                            {
                                                if (paramValue != null && paramValueTo != null)
                                                {
                                                    sb.Append("TRUNC(" + paramName + ",'MI')").Append(" BETWEEN ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                }
                                                else if (paramValue != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" >= ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" = ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    }
                                                }
                                                else if (paramValueTo != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" <= ").Append("TO_DATE('" + Convert.ToDateTime(paramValueTo).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" = ").Append("TO_DATE('" + Convert.ToDateTime(paramValueTo).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    }
                                                }
                                            }
                                            //else if (Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.Time)
                                            //{
                                            //    if (paramValue != null && paramValueTo != null)
                                            //    {
                                            //        sb.Append("TO_char(" + paramName + ",'HH24:MI')").Append(" BETWEEN ").Append("'" + Convert.ToDateTime(paramValue).ToString("HH:mm") + "'");
                                            //        sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
                                            //    }
                                            //    else if (paramValue != null)
                                            //    {
                                            //        if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            //        {
                                            //            sb.Append("TO_char(" + paramName + ",'HH24:MI')").Append(" >= ").Append("'" + Convert.ToDateTime(paramValue).ToString("HH:mm") + "'");
                                            //            incrementCount++;
                                            //        }
                                            //        else
                                            //        {
                                            //            sb.Append("TO_char(" + paramName + ",'HH24:MI')").Append(" = ").Append("'" + Convert.ToDateTime(paramValue).ToString("HH:mm") + "'");
                                            //        }
                                            //    }
                                            //    else if (paramValueTo != null)
                                            //    {
                                            //        if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            //        {
                                            //            sb.Append("TO_char(" + paramName + ",'HH24:MI')").Append(" <= ").Append("'" + Convert.ToDateTime(paramValueTo).ToString("HH:mm") + "'");
                                            //            incrementCount++;
                                            //        }
                                            //        else
                                            //        {
                                            //            sb.Append("TO_char(" + paramName + ",'HH24:MI')").Append(" = ").Append("'" + Convert.ToDateTime(paramValueTo).ToString("HH:mm") + "'");
                                            //        }
                                            //    }
                                            //}

                                        }
                                        else if (paramValue != null && paramValue.ToString().Contains(','))
                                        {
                                            sb.Append(paramName).Append(" IN (")
                                                .Append(paramValue.ToString()).Append(")");
                                        }
                                        else if (DisplayType.IsID(Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"])))
                                        {
                                            sb.Append(paramName).Append(" = ")
                                                .Append(paramValue.ToString());
                                        }
                                        else
                                        {
                                            sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                                                .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                                        }

                                        if (paramValue is DateTime || paramValueTo is DateTime)
                                        {
                                            if (paramValue != null)
                                            {
                                                string val = Convert.ToDateTime(parameters[para].GetInfo()).ToString();

                                                if (val.Contains("12:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("12:00:00"));
                                                }
                                                if (val.Contains("00:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("00:00:00"));
                                                }
                                                if (val.Contains(" "))
                                                {
                                                    val = val.Substring(0, val.IndexOf(" "));
                                                }


                                                pos.Set_Value("Col_" + (paraCount++), val);
                                            }
                                            else if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            {
                                                paraCount++;
                                            }

                                            if (!String.IsNullOrEmpty(sInfoTo))
                                            {
                                                string val = Convert.ToDateTime(parameters[para].GetInfo_To()).ToString();
                                                //incrementCount++;

                                                if (val.Contains("12:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("12:00:00"));
                                                }
                                                if (val.Contains("00:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("00:00:00"));
                                                }
                                                if (val.Contains(" "))
                                                {
                                                    val = val.Substring(0, val.IndexOf(" "));
                                                }

                                                pos.Set_Value("Col_" + (paraCount++), val);
                                            }
                                            else if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            {
                                                paraCount++;
                                            }
                                        }
                                        else
                                        {
                                            pos.Set_Value("Col_" + (paraCount++), parameters[para].GetInfo());

                                            if (!String.IsNullOrEmpty(sInfoTo))
                                            {
                                                incrementCount++;
                                                pos.Set_Value("Col_" + (paraCount++), parameters[para].GetInfo_To());
                                            }
                                        }

                                        loopCount++;
                                    }

                                    if (!columnFound)
                                    {
                                        if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                        {
                                            paraCount++;
                                            paraCount++;
                                        }
                                        else
                                        {
                                            paraCount++;
                                        }
                                    }
                                }
                            }


                            if (pos != null)
                            {
                                pos.Set_Value("AD_PInstance_ID", _pi.GetAD_PInstance_ID());
                                pos.Save();
                            }

                            //int loopCount = 0;
                            //for (int para = 0; para <= parameters.Count() - 1; para++)
                            //{
                            //    string sInfo = parameters[para].GetInfo();
                            //    string sInfoTo = parameters[para].GetInfo_To();
                            //    if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                            //    {
                            //        continue;
                            //    }

                            //    if (loopCount > 0)
                            //        sb.Append(" AND ");
                            //    string paramName = parameters[para].GetParameterName();
                            //    object paramValue = parameters[para].GetParameter();
                            //    object paramValueTo = parameters[para].GetParameter_To();

                            //    if (paramValue is DateTime)
                            //    {
                            //        sb.Append(paramName).Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                            //        if (paramValueTo != null && paramValueTo.ToString() != String.Empty)
                            //            sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
                            //        else
                            //            sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValue).AddDays(1), true));

                            //    }
                            //    else if (paramValue != null && paramValue.ToString().Contains(','))
                            //    {
                            //        sb.Append(paramName).Append(" IN (")
                            //            .Append(paramValue.ToString()).Append(")");
                            //    }
                            //    else
                            //    {
                            //        sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                            //            .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                            //    }

                            //    loopCount++;
                            //}
                        }
                        //{
                        //    int loopCount = 0;
                        //    for (int para = 0; para <= parameters.Count() - 1; para++)
                        //    {
                        //        string sInfo = parameters[para].GetInfo();
                        //        string sInfoTo = parameters[para].GetInfo_To();
                        //        if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                        //        {
                        //            continue;
                        //        }

                        //        if (loopCount > 0)
                        //            sb.Append(" AND ");
                        //        string paramName = parameters[para].GetParameterName();
                        //        object paramValue = parameters[para].GetParameter();
                        //        object paramValueTo = parameters[para].GetParameter_To();

                        //        if (paramValue is DateTime)
                        //        {
                        //            sb.Append(paramName).Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                        //            if (paramValueTo != null && paramValueTo.ToString() != String.Empty)
                        //                sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
                        //            else
                        //                sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValue).AddDays(1), true));

                        //        }
                        //        else if (paramValue != null && paramValue.ToString().Contains(','))
                        //        {
                        //            sb.Append(paramName).Append(" IN (")
                        //                .Append(paramValue.ToString()).Append(")");
                        //        }
                        //        else
                        //        {
                        //            sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                        //                .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                        //        }

                        //        loopCount++;
                        //    }
                        //}
                    }




                    sql = GetObscureSql(sql);

                    if (sb.Length > 7)
                        sql = sql + sb.ToString();

                    //if (form.IsIncludeProcedure())
                    //{
                    //    bool result = StartDBProcess(form.GetProcedureName(), parameters);
                    //}

                    //sqls[0] = sql;

                    //sql = string.Join(";", sqls);

                    //DataSet ds = DB.ExecuteDataset(sql);

                    sqls[0] = sql;

                    sql = string.Join(";", sqls);

                    string sqlQ = sqls[0].ToUpper();

                    if (sqlQ.Contains("T_CRYSTALPARAMETERS") || sqlQ.Contains("AD_PINSTANCE_ID"))
                    {
                        // sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                        if (sqlQ.Contains("WHERE"))
                        {
                            sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                        }
                        else
                        {
                            sqlQ = sqlQ + " WHERE AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                        }
                    }
                    sqlQ = sqlQ.ToLower();
                    DataSet ds = DB.ExecuteDataset(sqlQ);
                    log.Severe("CrystalReport Query: " + sqlQ);
                    if (ds == null)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        throw new Exception(error.GetValue() + "BlankReportWillOpen");
                    }

                    DataSet dsOrglogo = null;
                    bool imageError = false;
                    if (isIncludeImage)
                    {
                        //for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
                        //{
                        //    String ImagePath = "";
                        //    String ImageField = "";
                        //    if (ds.Tables[0].Rows[i_img][imagePathField] != null)
                        //    {
                        //        ImagePath = ds.Tables[0].Rows[i_img][imagePathField].ToString();
                        //        ImageField = imageField;

                        //        if (ds.Tables[0].Columns.Contains(ImageField))
                        //        {
                        //            if (File.Exists(reportImagePath + "\\" + ImagePath))
                        //            {
                        //                byte[] b = StreamFile(reportImagePath + "\\" + ImagePath);
                        //                ds.Tables[0].Rows[i_img][ImageField] = b;
                        //            }
                        //            else
                        //            {
                        //                //ds.Tables[0].Rows.RemoveAt(i_img);
                        //                imageError = true;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            imageError = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        imageError = true;
                        //    }
                        //}

                        log.Severe("CrystalReport Info: Loading Images");
                        string[] imgField = imageField.Split(';');
                        string[] imgFPath = imagePathField.Split(';');
                        byte[] orgLogo = null;
                        if (imgField.Contains("#ORGLOGO", StringComparer.OrdinalIgnoreCase))
                        {
                            object orglogo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + _ctx.GetAD_Org_ID());
                            if (orglogo != null && orglogo != DBNull.Value)
                            {
                                orgLogo = (byte[])orglogo;
                            }
                        }
                        else if (imgField.Contains("ORGLOGO", StringComparer.OrdinalIgnoreCase))
                        {
                            dsOrglogo = DB.ExecuteDataset("SELECT LOGO,AD_Org_ID FROM AD_OrgInfo WHERE ISACTIVE='Y'");
                        }

                        for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
                        {
                            for (int j_Img = 0; j_Img < imgField.Count(); j_Img++)
                            {
                                String ImagePath = "";
                                String ImageField = "";
                                if (imgFPath[j_Img] != null && imgFPath[j_Img] != "" && ds.Tables[0].Rows[i_img][imgFPath[j_Img]] != null)
                                {
                                    ImagePath = ds.Tables[0].Rows[i_img][imgFPath[j_Img]].ToString();
                                    ImageField = imgField[j_Img].ToUpper();
                                    if (ImageField == "#ORGLOGO")
                                    {
                                        if (orgLogo != null && orgLogo.Length > 0)
                                        {
                                            ds.Tables[0].Rows[i_img]["ORGLOGO"] = orgLogo;
                                        }
                                        else
                                        {
                                            imageError = true;
                                        }
                                    }
                                    else if (ds.Tables[0].Columns.Contains(ImageField))
                                    {
                                        if (File.Exists(reportImagePath + "\\" + ImagePath))
                                        {
                                            byte[] b = StreamFile(reportImagePath + "\\" + ImagePath);
                                            ds.Tables[0].Rows[i_img][ImageField] = b;
                                        }
                                        else
                                        {
                                            imageError = true;
                                        }
                                    }
                                    else
                                    {
                                        imageError = true;
                                    }
                                }
                                else if (imgField[j_Img].ToUpper() == "#ORGLOGO")
                                {
                                    if (orgLogo != null && orgLogo.Length > 0)
                                    {
                                        ds.Tables[0].Rows[i_img]["ORGLOGO"] = orgLogo;
                                    }
                                    else
                                    {
                                        imageError = true;
                                    }
                                }
                                else if (imgField[j_Img].ToUpper() == "ORGLOGO"
                                    && dsOrglogo != null && dsOrglogo.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.IndexOf("AD_ORG_ID") > 0)
                                {
                                    if (dsOrglogo.Tables[0].Select("AD_Org_ID=" + ds.Tables[0].Rows[i_img]["AD_Org_ID"]) != null)
                                    {
                                        ds.Tables[0].Rows[i_img]["ORGLOGO"] = dsOrglogo.Tables[0].Select("AD_Org_ID=" + ds.Tables[0].Rows[i_img]["AD_Org_ID"])[0]["LOGO"];
                                    }
                                }
                                else
                                {
                                    imageError = true;
                                }
                            }

                        }
                        log.Severe("CrystalReport Info: Loading Images Done");
                    }

                    if (imageError)
                    {
                        //   ShowMessage.Error("ErrorLoadingSomeImages", true);
                    }

                    //crystalReportViewer1.ReportSource = rptBurndown;
                    //crystalReportViewer1.Refresh();

                    System.IO.Stream oStream;
                    byte[] byteArray = null;

                    string[] imgField1 = imageField.Split(';');
                    string[] imgFPath1 = imagePathField.Split(';');

                    if (ds.Tables[0].Rows.Count == 0 && imgField1.Contains("#ORGLOGO", StringComparer.OrdinalIgnoreCase))
                    {
                        byte[] orgLogo = null;
                        object orglogo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + _ctx.GetAD_Org_ID());
                        if (orglogo != null && orglogo != DBNull.Value)
                        {
                            orgLogo = (byte[])orglogo;
                        }

                        DataRow dr = ds.Tables[0].NewRow();
                        dr["ORGLOGO"] = orgLogo;
                        ds.Tables[0].Rows.Add(dr);

                    }

                    rptBurndown.SetDataSource(ds.Tables[0]);                //By karan approveed by lokesh......

                    //if (reportNameArray.Length > 1)
                    //{
                    //    for (int k = 1; k < reportNameArray.Length; k++)
                    //    {
                    //        rptBurndown.Subreports[reportNameArray[k]].Load(reportPathArray[k]);
                    //        rptBurndown.Subreports[reportNameArray[k]].SetDataSource(ds.Tables[k]);
                    //    }
                    //}

                    int k = 1;
                    foreach (ReportObject repOp in rptBurndown.ReportDefinition.ReportObjects)
                    {
                        if (repOp.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)repOp).SubreportName;
                            ReportDocument subRepDoc = rptBurndown.Subreports[SubRepName];

                            int index = Array.IndexOf(reportNameArray, SubRepName);
                            if (index > -1)
                            {
                                sqlQ = sqls[index].ToUpper();



                                sqlQ = GetObscureSql(sqlQ);

                                //if (sqlQ.ToUpper().Contains("T_CrystalParameters"))
                                //{
                                //    sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                                //}
                                if (sqlQ.Contains("T_CRYSTALPARAMETERS") || sqlQ.Contains("AD_PINSTANCE_ID"))
                                {
                                    if (sqlQ.Contains("WHERE"))
                                    {
                                        sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                                    }
                                    else
                                    {
                                        sqlQ = sqlQ + " WHERE AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                                    }
                                }
                                sqlQ = sqlQ.ToLower();
                                ds = DB.ExecuteDataset(sqlQ);
                                log.Severe("CrystalReport Query: " + sqlQ);
                                subRepDoc.SetDataSource(ds.Tables[0]);
                            }
                        }
                    }


                    //rptBurndown.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(100, 360, 100, 360));
                    oStream = rptBurndown.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    byteArray = new byte[oStream.Length];
                    oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length));
                    rptBurndown.Close();
                    rptBurndown.Dispose();
                    GC.Collect();
                    rptBurndown = null;
                    return byteArray;

                    //if (form.IsDirectPrint())
                    //{
                    //    // rptBurndown.PrintOptions.PrinterName = Env.GetCtx().GetPrinterName();
                    //    //rptBurndown.PrintToPrinter(1, false, 0, 0);
                    //}
                }
                catch (Exception ex)
                {
                    if (rptBurndown != null)
                    {
                        rptBurndown.Close();
                        rptBurndown.Dispose();
                        GC.Collect();
                    }
                    throw ex;

                }
            }
            else
            {
                throw new MissingFieldException("CouldNotFindTheCrystalReport");
            }
        }


#pragma warning disable 612, 618

        private bool StartDBProcess(String procedureName, ProcessInfoParameter[] list)
        {
            if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
            {
                return false;
            }

            //  execute on this thread/connection
            //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
            try
            {
                //only oracle procedure are supported
                OracleCommand comm = new OracleCommand();
                OracleConnection conn = (OracleConnection)VAdvantage.DataBase.DB.GetConnection();
                conn.Open();
                comm.Connection = conn;
                comm.CommandText = procedureName;
                comm.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(comm);
                OracleParameter[] param = new OracleParameter[comm.Parameters.Count];
                int i = 0;
                StringBuilder orclParams = new StringBuilder();
                bool isDateTo = false;
                foreach (OracleParameter orp in comm.Parameters)
                {
                    if (isDateTo)
                    {
                        isDateTo = false;
                        continue;
                    }
                    Object paramvalue = list[i].GetParameter();
                    if (paramvalue != null)
                    {
                        if (orp.DbType == System.Data.DbType.DateTime)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                            }
                            param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                            if (list[i].GetParameter_To().ToString().Length > 0)
                            {
                                paramvalue = list[i].GetParameter_To();
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                                param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                i++;
                                isDateTo = true;
                                continue;
                            }
                            else
                            {
                                if (comm.Parameters.Count > (i + 1))
                                {
                                    if (comm.Parameters[i + 1].ParameterName.Equals(comm.Parameters[i].ParameterName + "_TO", StringComparison.OrdinalIgnoreCase))
                                    {
                                        param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                        isDateTo = true;
                                        continue;
                                    }
                                }
                            }
                        }
                        else if (orp.DbType == System.Data.DbType.VarNumeric)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                //continue;
                            }
                            else
                                paramvalue = 0;
                        }
                        else
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = GlobalVariable.TO_STRING(paramvalue.ToString());
                            }
                        }

                    }
                    param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                    //orclParams.Append(orp.ParameterName).Append(": ").Append(_curTab.GetValue(list[i]));
                    //if (i < comm.Parameters.Count - 1)
                    //    orclParams.Append(", ");
                    i++;
                }

                //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                int res = VAdvantage.SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
                //DataBase.DB.ExecuteQuery(sql, null);
            }
            catch (Exception e)
            {
                VLogger.Get().SaveError(e.Message, e);
                //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                return false;

            }
            //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
            return true;
        }

#pragma warning restore 612, 618

        private byte[] StreamFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];

            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

            //Close the File Stream
            fs.Close();
            return ImageData; //return the byte data
        }


        public byte[] GetReportBytes()
        {
            return GenerateCrystalReport();
        }

        public String GenerateCrystalFilePath(bool fetchBytes, out byte[] bytes, string filetype)//Pratap- Added Parameter isCsv  26-2-16
        {
            ReportDocument rptBurndown = GetReportDocument();

            if (rptBurndown != null)
            {
                System.IO.Stream oStream;

                if (fetchBytes)
                {
                    try
                    {
                        log.Severe("CrystalReport Info: Creating Stream");
                        oStream = rptBurndown.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        bytes = new byte[oStream.Length];
                        oStream.Read(bytes, 0, Convert.ToInt32(oStream.Length));
                        oStream.Close();
                        log.Severe("CrystalReport Info: Creating Stream End");
                    }
                    catch (Exception exx)
                    {
                        if (rptBurndown != null)
                        {
                            rptBurndown.Close();
                            rptBurndown.Dispose();
                            GC.Collect();
                        }
                        bytes = null;
                        log.Severe(exx.Message);
                    }
                }
                else
                {
                    bytes = null;
                }

                string FILE_PATH = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";

                if (!Directory.Exists(FILE_PATH))
                    Directory.CreateDirectory(FILE_PATH);

                //Pratap 26-2-16
                string filePath = null;
                if (filetype == "C")
                {

                    filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".csv";
                    rptBurndown.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);
                }
                else if (filetype == "P")
                {
                    filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".pdf";
                    rptBurndown.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
                }
                else if (filetype == "R")
                {
                    filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".rtf";



                    rptBurndown.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.RichText, filePath);
                }
                //
                rptBurndown.Close();
                rptBurndown.Dispose();
                GC.Collect();
                rptBurndown = null;
                log.Severe("CrystalReport Info:File Saved");
                return filePath.Substring(filePath.IndexOf("TempDownload"));
            }
            else
            {
                throw new MissingFieldException("CouldNotFindTheCrystalReport");
            }


        }

        public ReportDocument GetReportDocument()
        {
            ReportDocument rptBurndown = null;

            string reportPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Reports");
            string reportImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "");

            string[] reportNameArray;
            string[] reportPathArray;
            string path = reportPath;


            if (String.IsNullOrEmpty(path))
            {
                throw new MissingFieldException("CrystalReportPathNotSet");

            }

            reportNameArray = reportName.Split(';');
            reportPathArray = new string[reportNameArray.Length];

            if (reportNameArray.Length > 0)
            {
                for (int i = 0; i < reportNameArray.Length; i++)
                {
                    if (reportNameArray[0].IndexOf(":") < 0)
                        reportPathArray[i] = path + "\\" + reportNameArray[i];
                    else
                        reportPathArray[i] = reportNameArray[i];
                }
            }
            if (reportPathArray != null && reportPathArray.Length > 0 && File.Exists(reportPathArray[0]))   //Check if the crystal report file exists in a specified location.
            {
                try
                {
                    rptBurndown = new ReportDocument();
                    rptBurndown.Load(reportPathArray[0]);

                    //Set Connection Info
                    //ConnectionInfo.Get().SetAttributes(System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"]);


                    //Application will pick database info from the property file.
                    //CrystalDecisions.Shared.ConnectionInfo crDbConnection = new CrystalDecisions.Shared.ConnectionInfo();
                    //crDbConnection.IntegratedSecurity = false;
                    //crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                    //crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                    //crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                    //crDbConnection.ServerName = ConnectionInfo.Get().Db_host;
                    CrystalDecisions.CrystalReports.Engine.Database crDatabase = rptBurndown.Database;
                    CrystalDecisions.Shared.TableLogOnInfo oCrTableLoginInfo;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table oCrTable in crDatabase.Tables)
                    {
                        //crDbConnection.IntegratedSecurity = false;
                        //crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                        //crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                        //crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                        //crDbConnection.ServerName = ConnectionInfo.Get().Db_host;

                        oCrTableLoginInfo = oCrTable.LogOnInfo;
                        //oCrTableLoginInfo.ConnectionInfo = crDbConnection;
                        oCrTable.ApplyLogOnInfo(oCrTableLoginInfo);
                    }




                    //Create Parameter query
                    string[] sqls = SqlQuery.Split(';'); ;

                    string sql = sqls[0];


                    StringBuilder sb = new StringBuilder(" WHERE ");

                    if (_pi.GetRecord_ID() > 0 && _pi.GetTable_ID() > 0)
                    {
                        string tableName = DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_TABLE_ID =" + _pi.GetTable_ID()).ToString();
                        sb.Append(tableName).Append("_ID = ").Append(_pi.GetRecord_ID());
                    }

                    else
                    {

                        ProcessInfoUtil.SetParameterFromDB(_pi, _ctx);
                        ProcessInfoParameter[] parameters = _pi.GetParameter();
                        log.Severe("CrystalReport Query: Parameters found: " + parameters.Count());


                        PO pos = null;

                        if (parameters.Count() > 0)
                        {
                            MTable table = MTable.Get(_ctx, "T_CrystalParameters");
                            pos = table.GetPO(_ctx, 0, null);
                            int loopCount = 0;
                            int paraCount = 1;
                            int incrementCount = 1;

                            string sqla = "SELECT ColumnName, IsRange, AD_Reference_ID FROM AD_Process_Para WHERE IsActive='Y' AND AD_Process_ID=" + _pi.GetAD_Process_ID() + " ORDER BY SEQNO ";
                            DataSet dsPara = DB.ExecuteDataset(sqla);
                            List<string> paraList = new List<string>();
                            List<string> paraRangeList = new List<string>();
                            if (dsPara != null && dsPara.Tables[0].Rows.Count > 0)
                            {
                                for (int c = 0; c < dsPara.Tables[0].Rows.Count; c++)
                                {
                                    var columnFound = false;
                                    for (int para = 0; para <= parameters.Count() - 1; para++)
                                    {
                                        string paramName = parameters[para].GetParameterName();

                                        if (!paramName.ToUpper().Equals(dsPara.Tables[0].Rows[c]["ColumnName"].ToString().ToUpper()))
                                        {
                                            continue;
                                        }
                                        columnFound = true;
                                        string sInfo = parameters[para].GetInfo();
                                        string sInfoTo = parameters[para].GetInfo_To();
                                        if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                                        {
                                            continue;
                                        }

                                        if (loopCount > 0)
                                            sb.Append(" AND ");
                                        object paramValue = parameters[para].GetParameter();
                                        object paramValueTo = parameters[para].GetParameter_To();

                                        if (paramValue is DateTime || paramValueTo is DateTime)
                                        {
                                            if (Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.Date)
                                            {
                                                if (paramValue != null && paramValueTo != null)
                                                {
                                                    sb.Append("TRUNC(" + paramName + ",'DD')").Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                    sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                }
                                                else if (paramValue != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" >= ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" = ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                                    }
                                                }
                                                else if (paramValueTo != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" <= ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'DD')").Append(" = ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                    }
                                                }
                                            }
                                            else if (Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.DateTime ||
                                                Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"]) == DisplayType.Time)
                                            {
                                                if (paramValue != null && paramValueTo != null)
                                                {
                                                    sb.Append("TRUNC(" + paramName + ",'MI')").Append(" BETWEEN ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo), true));
                                                }
                                                else if (paramValue != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" >= ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" = ").Append("TO_DATE('" + Convert.ToDateTime(paramValue).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    }
                                                }
                                                else if (paramValueTo != null)
                                                {
                                                    if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" <= ").Append("TO_DATE('" + Convert.ToDateTime(paramValueTo).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                        incrementCount++;
                                                    }
                                                    else
                                                    {
                                                        sb.Append("TRUNC(" + paramName + ",'MI')").Append(" = ").Append("TO_DATE('" + Convert.ToDateTime(paramValueTo).ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')");
                                                    }
                                                }
                                            }
                                        }
                                        else if (paramValue != null && paramValue.ToString().Contains(','))
                                        {
                                            sb.Append(paramName).Append(" IN (")
                                                .Append(paramValue.ToString()).Append(")");
                                        }
                                        else if (DisplayType.IsID(Convert.ToInt32(dsPara.Tables[0].Rows[c]["AD_Reference_ID"])))
                                        {
                                            sb.Append(paramName).Append(" = ")
                                                .Append(paramValue.ToString());
                                        }
                                        else
                                        {
                                            sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                                                .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                                        }

                                        if (paramValue is DateTime || paramValueTo is DateTime)
                                        {
                                            if (paramValue != null)
                                            {
                                                string val = Convert.ToDateTime(parameters[para].GetInfo()).ToString();

                                                if (val.Contains("12:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("12:00:00"));
                                                }
                                                if (val.Contains("00:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("00:00:00"));
                                                }
                                                if (val.Contains(" "))
                                                {
                                                    val = val.Substring(0, val.IndexOf(" "));
                                                }


                                                pos.Set_Value("Col_" + (paraCount++), val);
                                            }
                                            else if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            {
                                                paraCount++;
                                            }

                                            if (!String.IsNullOrEmpty(sInfoTo))
                                            {
                                                string val = Convert.ToDateTime(parameters[para].GetInfo_To()).ToString();

                                                if (val.Contains("12:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("12:00:00"));
                                                }
                                                if (val.Contains("00:00:00"))
                                                {
                                                    val = val.Substring(0, val.IndexOf("00:00:00"));
                                                }
                                                if (val.Contains(" "))
                                                {
                                                    val = val.Substring(0, val.IndexOf(" "));
                                                }

                                                pos.Set_Value("Col_" + (paraCount++), val);
                                            }
                                            else if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                            {
                                                paraCount++;
                                            }
                                        }
                                        else
                                        {
                                            pos.Set_Value("Col_" + (paraCount++), parameters[para].GetInfo());

                                            if (!String.IsNullOrEmpty(sInfoTo))
                                            {
                                                incrementCount++;
                                                pos.Set_Value("Col_" + (paraCount++), parameters[para].GetInfo_To());
                                            }
                                        }

                                        loopCount++;
                                    }

                                    if (!columnFound)
                                    {
                                        if (dsPara.Tables[0].Rows[c]["IsRange"].ToString().ToUpper() == "Y")
                                        {
                                            paraCount++;
                                            paraCount++;
                                        }
                                        else
                                        {
                                            paraCount++;
                                        }
                                    }
                                }
                            }




                            if (pos != null)
                            {
                                pos.Set_Value("AD_PInstance_ID", _pi.GetAD_PInstance_ID());
                                pos.Save();
                            }

                        }
                    }

                    sql = GetObscureSql(sql);

                    if (sb.Length > 7)
                        sql = sql + sb.ToString();

                    sqls[0] = sql;

                    sql = string.Join(";", sqls);
                    string sqlQ = sqls[0].ToUpper();

                    if (sqlQ.Contains("T_CRYSTALPARAMETERS") || sqlQ.Contains("AD_PINSTANCE_ID"))
                    {
                        if (sqlQ.Contains("WHERE"))
                        {
                            sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                        }
                        else
                        {
                            sqlQ = sqlQ + " WHERE AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                        }
                    }
                    sqlQ = sqlQ.ToLower();
                    DataSet ds = DB.ExecuteDataset(sqlQ);
                    log.Severe("CrystalReport Query: " + sqlQ);

                    if (ds == null)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        log.Severe("CrystalReport Info: " + error.GetValue() + "BlankReportWillOpen");
                        throw new Exception(error.GetValue() + "BlankReportWillOpen");
                    }
                    DataSet dsOrglogo = null;
                    bool imageError = false;
                    if (isIncludeImage)
                    {

                        log.Severe("CrystalReport Info: Loading Images");
                        string[] imgField = imageField.Split(';');
                        string[] imgFPath = imagePathField.Split(';');
                        byte[] orgLogo = null;
                        if (imgField.Contains("#ORGLOGO", StringComparer.OrdinalIgnoreCase))
                        {
                            object orglogo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + _ctx.GetAD_Org_ID());
                            if (orglogo != null && orglogo != DBNull.Value)
                            {
                                orgLogo = (byte[])orglogo;
                            }
                        }
                        else if (imgField.Contains("ORGLOGO", StringComparer.OrdinalIgnoreCase))
                        {
                            dsOrglogo = DB.ExecuteDataset("SELECT LOGO,AD_Org_ID FROM AD_OrgInfo WHERE ISACTIVE='Y'");
                        }

                        for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
                        {
                            for (int j_Img = 0; j_Img < imgField.Count(); j_Img++)
                            {
                                String ImagePath = "";
                                String ImageField = "";
                                if (imgFPath[j_Img] != null && imgFPath[j_Img] != "" && ds.Tables[0].Rows[i_img][imgFPath[j_Img]] != null)
                                {
                                    ImagePath = ds.Tables[0].Rows[i_img][imgFPath[j_Img]].ToString();
                                    ImageField = imgField[j_Img].ToUpper();
                                    if (ImageField == "#ORGLOGO")
                                    {
                                        if (orgLogo != null && orgLogo.Length > 0)
                                        {
                                            ds.Tables[0].Rows[i_img]["ORGLOGO"] = orgLogo;
                                        }
                                        else
                                        {
                                            imageError = true;
                                        }
                                    }
                                    else if (ds.Tables[0].Columns.Contains(ImageField))
                                    {
                                        if (File.Exists(reportImagePath + "\\" + ImagePath))
                                        {
                                            byte[] b = StreamFile(reportImagePath + "\\" + ImagePath);
                                            ds.Tables[0].Rows[i_img][ImageField] = b;
                                        }
                                        else
                                        {
                                            imageError = true;
                                        }
                                    }
                                    else
                                    {
                                        imageError = true;
                                    }
                                }
                                else if (imgField[j_Img].ToUpper() == "#ORGLOGO")
                                {
                                    if (orgLogo != null && orgLogo.Length > 0)
                                    {
                                        ds.Tables[0].Rows[i_img]["ORGLOGO"] = orgLogo;
                                    }
                                    else
                                    {
                                        imageError = true;
                                    }
                                }
                                else if (imgField[j_Img].ToUpper() == "ORGLOGO"
                                    && dsOrglogo != null && dsOrglogo.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.IndexOf("AD_ORG_ID") > 0)
                                {
                                    if (dsOrglogo.Tables[0].Select("AD_Org_ID=" + ds.Tables[0].Rows[i_img]["AD_Org_ID"]) != null)
                                    {
                                        ds.Tables[0].Rows[i_img]["ORGLOGO"] = dsOrglogo.Tables[0].Select("AD_Org_ID=" + ds.Tables[0].Rows[i_img]["AD_Org_ID"])[0]["LOGO"];
                                    }
                                }
                                else
                                {
                                    imageError = true;
                                }
                            }

                        }
                        log.Severe("CrystalReport Info: Loading Images Done");
                    }

                    if (imageError)
                    {
                        //   ShowMessage.Error("ErrorLoadingSomeImages", true);
                    }



                    string[] imgField1 = imageField.Split(';');
                    string[] imgFPath1 = imagePathField.Split(';');

                    if (ds.Tables[0].Rows.Count == 0 && imgField1.Contains("#ORGLOGO", StringComparer.OrdinalIgnoreCase))
                    {
                        byte[] orgLogo = null;
                        object orglogo = DB.ExecuteScalar("SELECT LOGO FROM AD_OrgInfo WHERE ISACTIVE='Y' AND AD_Org_ID=" + _ctx.GetAD_Org_ID());
                        if (orglogo != null && orglogo != DBNull.Value)
                        {
                            orgLogo = (byte[])orglogo;
                        }

                        DataRow dr = ds.Tables[0].NewRow();

                        dr["ORGLOGO"] = orgLogo;
                        ds.Tables[0].Rows.Add(dr);

                    }

                    rptBurndown.SetDataSource(ds.Tables[0]);                //By karan approveed by lokesh......
                    log.Severe("CrystalReport Info: Data Source Assigned. Rows" + ds.Tables[0].Rows.Count);

                    //Get sub report and assign datatable1 to that report 
                    foreach (ReportObject repOp in rptBurndown.ReportDefinition.ReportObjects)
                    {
                        if (repOp.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)repOp).SubreportName;
                            ReportDocument subRepDoc = rptBurndown.Subreports[SubRepName];
                            int index = Array.IndexOf(reportNameArray, SubRepName);
                            if (index > -1)
                            {
                                sqlQ = sqls[index];

                                sqlQ = GetObscureSql(sqlQ);

                                //Check if AD_PInstance_ID exist in query, only then apply AD_PInstance_ID in where clause.
                                sqlQ = sqlQ.ToUpper();

                                if (sqlQ.Contains("T_CRYSTALPARAMETERS") || sqlQ.Contains("AD_PINSTANCE_ID"))
                                {
                                    if (sqlQ.Contains("WHERE"))
                                    {
                                        sqlQ = sqlQ + " AND AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                                    }
                                    else
                                    {
                                        sqlQ = sqlQ + " WHERE AD_PInstance_ID=" + _pi.GetAD_PInstance_ID();
                                    }
                                }
                                sqlQ = sqlQ.ToLower();
                                ds = DB.ExecuteDataset(sqlQ);
                                log.Severe("CrystalReport Query: " + sqlQ);
                                subRepDoc.SetDataSource(ds.Tables[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Severe("CrystalReport Error: " + ex.Message);
                    throw ex;

                }
                return rptBurndown;
            }
            else
            {
                throw new MissingFieldException("CouldNotFindTheCrystalReport");
            }
        }

        public string GetReportString()
        {
            return null;
        }
        public String GetReportFilePath(bool fetchBytes, out byte[] bytes)
        {
            return GenerateCrystalFilePath(fetchBytes, out bytes, "P");
        }

        public string GetCsvReportFilePath(string data)
        {
            // return null;
            byte[] bytes;
            return GenerateCrystalFilePath(false, out bytes, "C");//Pratap 26-2-16
        }
        public string GetRtfReportFilePath(string data)
        {
            byte[] bytes;
            return GenerateCrystalFilePath(false, out bytes, "R");
        }


        public bool StartReport(Ctx ctx, ProcessInfo pi, Trx trx)
        {
            _ctx = ctx;
            _pi = pi;

            //	Get AD_Table_ID and TableName
            String sql = "SELECT p.AD_Process_ID, p.ReportPath,p.SqlQuery,p.IncludeImage,p.ImageField,p.ImagePathField "
                + " FROM AD_PInstance pi"
                + " INNER JOIN AD_Process p ON (pi.AD_Process_ID=p.AD_Process_ID)"
                + " WHERE pi.AD_PInstance_ID='" + pi.GetAD_PInstance_ID() + "' ";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                //	Just get first 
                if (dr.Read())
                {
                    AD_Process_ID = Utility.Util.GetValueOfInt(dr[0].ToString());		//	required
                    reportName = dr[1].ToString();
                    SqlQuery = dr[2].ToString();

                    imageField = Utility.Util.GetValueOfString(dr[4].ToString());
                    imagePathField = dr[5].ToString();
                    isIncludeImage = "Y".Equals(dr[3].ToString());	//	required
                }

                dr.Close();
            }
            catch (Exception e1)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw e1;
            }
            return true;
        }

        /// <summary>
        /// Check if a column is marked as obscure and add regaular expression.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>SQL</returns>
        private string GetObscureSql(string sql)
        {
            string tableName = sql.Substring(sql.IndexOf("FROM") + 4).Trim().Split(' ')[0].ToString().Trim();

            MTable table = MTable.Get(_ctx, tableName);
            if (table == null)
                return sql;

            MColumn[] columns = table.GetColumns(true);
            List<MColumn> cols = columns.Where(a => a.GetObscureType() != null && a.GetObscureType().Length > 0).ToList<MColumn>();
            if (cols != null && cols.Count > 0)
            {
                foreach (MColumn col in cols)
                {
                    string Name = col.GetColumnName();
                    if (sql.Contains(Name) && !MRole.GetDefault(_ctx).IsColumnAccess(col.GetAD_Table_ID(), col.GetAD_Column_ID(), false))
                    {
                        string obscureColumn = DBFunctionCollections.GetObscureColumn(col.GetObscureType(), tableName, Name) + " as " + Name;
                        sql = sql.Replace(Name, obscureColumn);
                    }
                }
            }
            return sql;
        }

    }


    //public class HierarchyValues
    //{
    //    public string GetParametersFromHierarchy(string columnName, int ID)
    //    {


    //        return "";
    //    }
    //}


}