/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Jagmohan Bhatt
 * Date           : 3-may-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using System.Threading;
using System.Data;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Login;

namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// Utility function for process
    /// </summary>
    public class ProcessInfoUtil
    {
        //Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(ProcessInfoUtil).FullName);


        /// <summary>
        /// Sets the summary from database
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static void SetSummaryFromDB(ProcessInfo pi)
        {
            int sleepTime = 2000;	//	2 secomds
            int noRetry = 5;        //  10 seconds total
            //
            String sql = "SELECT Result, ErrorMsg FROM AD_PInstance "
                + "WHERE AD_PInstance_ID=@instanceid"
                + " AND Result IS NOT NULL";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                for (int noTry = 0; noTry < noRetry; noTry++)
                {
                    param[0] = new SqlParameter("@instanceid", pi.GetAD_PInstance_ID());
                    dr = DataBase.DB.ExecuteReader(sql, param, null);
                    while (dr.Read())
                    {
                        //	we have a result
                        int i = Utility.Util.GetValueOfInt(dr[0].ToString());
                        if (i == 1)
                        {
                            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Success", true));
                        }
                        else
                        {
                            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Failure", true));
                        }

                        String Message = dr[1].ToString();
                        dr.Close();
                        //
                        if (Message != null)
                        {
                            if (Message != "")
                                pi.AddSummary("  (" + Utility.Msg.ParseTranslation(Utility.Env.GetContext(), Message) + ")");
                        }
                        return;
                    }

                    dr.Close();
                    //	sleep
                    try
                    {
                        Thread.Sleep(sleepTime);
                    }
                    catch (Exception ie)
                    {
                        if (dr != null)
                        {
                            dr.Close();
                        }
                        _log.Log(Level.SEVERE, "Sleep Thread", ie);
                    }
                }

            }
            catch (SqlException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                pi.SetSummary(e.Message, true);
                return;
            }
            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Timeout", true));
        }	//	setSummaryFromDB




        /// <summary>
        /// Set param from db
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static void SetParameterFromDB(ProcessInfo pi, Ctx ctx)
        {
            List<ProcessInfoParameter> list = new List<ProcessInfoParameter>();

            String sql = @"SELECT ip.ParameterName,
                                      ip.P_String,
                                      ip.P_String_To,
                                      ip.P_Number,
                                      ip.P_Number_To,
                                      ip.P_Date,
                                      ip.P_Date_To,
                                      ip.Info,
                                      ip.Info_To,
                                      i.AD_Client_ID,
                                      i.AD_Org_ID,
                                      i.AD_User_ID,
                                      NVL(PP.LOADRECURSIVEDATA,'N') as LOADRECURSIVEDATA,
                                     nvl(pp.ShowChildOfSelected,'N') as ShowChildOfSelected,nvl(pp.AD_Reference_ID,0) as AD_Reference_ID
                                    FROM AD_PInstance_Para ip JOIN AD_PInstance i ON (ip.AD_PINstance_ID=i.AD_PINstance_ID)
                                   Left Outer JOIN AD_Process_Para pp
                                        ON (pp.AD_Process_Para_ID=ip.AD_Process_Para_ID
                                        AND pp.AD_Process_ID=i.AD_Process_ID)
                                    WHERE ip.AD_PInstance_ID =@pinstanceid";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", pi.GetAD_PInstance_ID());
                //param[0] = new SqlParameter("@pinstanceid", 1000296);

                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    String ParameterName = dr[0].ToString();
                    //	String
                    Object Parameter = dr[1].ToString();
                    Object Parameter_To = dr[2].ToString();

                    Parameter = Parameter.ToString() == "" ? null : Parameter;
                    Parameter_To = Parameter_To.ToString() == "" ? null : Parameter_To;

                    //int displayType = 0;
                    //if (dr[16] != null && dr[16] != DBNull.Value)
                    //{
                    //    displayType = Util.GetValueOfInt(dr[16]);
                    //}

                    //	Big Decimal
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {

                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter = Utility.Util.GetValueOfDecimal(dr[3]);
                        }
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter_To = Utility.Util.GetValueOfDecimal(dr[4]);
                        }
                    }
                    //	Timestamp
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        //if (displayType == 0)
                        //{
                        //    if (!(dr[5] == DBNull.Value))
                        //    {
                        //        Parameter = DateTime.Parse(dr[5].ToString());
                        //    }
                        //    if (!(dr[6] == DBNull.Value))
                        //    {
                        //        Parameter_To = DateTime.Parse(dr[6].ToString());
                        //    }
                        //}
                        //else
                        //{
                        //if (displayType == DisplayType.Date)
                        //{
                        if (dr[5] != null && dr[5] != DBNull.Value)
                        {
                            Parameter = DateTime.Parse(dr[5].ToString());
                        }
                        if (dr[6] != null && dr[6] != DBNull.Value)
                        {
                            Parameter_To = DateTime.Parse(dr[6].ToString());
                        }
                        //}
                        //else if (displayType == DisplayType.DateTime)
                        //{
                        //if (dr[12] != null && dr[12] != DBNull.Value)
                        //{
                        //    Parameter = DateTime.Parse(dr[12].ToString());
                        //}
                        //if (dr[13] != null && dr[13] != DBNull.Value)
                        //{
                        //    Parameter_To = DateTime.Parse(dr[13].ToString());
                        //}
                        ////}
                        ////else if (displayType == DisplayType.Time)
                        ////{
                        //if (dr[14] != null && dr[14] != DBNull.Value)
                        //{
                        //    Parameter = DateTime.Parse(dr[14].ToString());
                        //}
                        //if (dr[15] != null && dr[15] != DBNull.Value)
                        //{
                        //    Parameter_To = DateTime.Parse(dr[15].ToString());
                        //}
                        //}
                        //}
                    }
                    //	Info
                    String Info = dr[7].ToString();
                    String Info_To = dr[8].ToString();



                    if (dr[12].ToString().Equals("Y") && ((DisplayType.IsID(Utility.Util.GetValueOfInt(dr[14])) || DisplayType.MultiKey == Utility.Util.GetValueOfInt(dr[14]))))
                    {
                        string result = Parameter.ToString();
                        string recResult = GetRecursiveParameterValue(ctx, ParameterName, result.ToString(), ref result, dr[13].ToString().Equals("Y"));
                        if (!string.IsNullOrEmpty(recResult))
                        {
                            Info = Info + ", " + recResult;
                        }
                        Parameter = result;


                        if (Parameter_To != null && Parameter_To.ToString().Length > 0)
                        {
                            result = Parameter_To.ToString();
                            recResult = GetRecursiveParameterValue(ctx, ParameterName, result.ToString(), ref result, dr[13].ToString().Equals("Y"));
                            if (!string.IsNullOrEmpty(recResult))
                            {
                                Info_To = Info_To + ", " + recResult;
                            }
                            Parameter_To = result;
                        }

                    }

                    if (Parameter_To != null && Parameter_To.ToString().EndsWith(","))
                    {
                        Parameter_To = Parameter_To.ToString().Substring(0, Parameter_To.ToString().Length - 1);
                    }

                    if (Parameter != null && Parameter.ToString().EndsWith(","))
                    {
                        Parameter = Parameter.ToString().Substring(0, Parameter.ToString().Length - 1);
                    }


                    //
                    list.Add(new ProcessInfoParameter(ParameterName, Parameter, Parameter_To, Info, Info_To));
                    //
                    if (pi.GetAD_Client_ID() == null)
                        pi.SetAD_Client_ID(int.Parse(dr[9].ToString()));
                    if (pi.GetAD_User_ID() == null)
                        pi.SetAD_User_ID(int.Parse(dr[11].ToString()));
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Severe(e.ToString());
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //
            ProcessInfoParameter[] pars = new ProcessInfoParameter[list.Count()];
            pars = list.ToArray();
            pi.SetParameter(pars);
        }   //  setParameterFromDB

        /// <summary>
        /// function will accept columnName and Ids selected. Will Fetch information from Default tree hierarchy and get child records accordingly.
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns>Name of orgs separated bY commas, and IDS in Reference Object</returns>
        private static string GetRecursiveParameterValue(Ctx _ctx, string columnName, string value, ref string result, bool ShowChildOfSelected)
        {

            string tableName = columnName.Substring(0, columnName.Length - 3);

            string[] values = value.Split(',');
            String eSql = "";
            string result1 = "";
            StringBuilder finalResult = new StringBuilder();
            StringBuilder nonSummaryResult = new StringBuilder();
            if (values.Length > 0)
            {

                // Get Default Heirarchy
                string sqla = @"SELECT PA_HIERARCHY_id FROM PA_Hierarchy WHERE ISACTIVE ='Y' 
                       ORDER BY ISDEFAULT DESC ,PA_HIERARCHY_id ASC";
                sqla = MRole.GetDefault(_ctx).AddAccessSQL(sqla, "PA_Hierarchy", true, true);
                object ID = DB.ExecuteScalar(sqla);
                int _PA_Hierarchy_ID = 0;
                if (ID != null && ID != DBNull.Value)
                {
                    _PA_Hierarchy_ID = Util.GetValueOfInt(ID);
                }
                Language _language = Language.GetLanguage(_ctx.GetAD_Language());

                //Get Query to fetch identifier value from table based on column selected. it will be used to display identifires on for parameter in report.
                eSql = VLookUpFactory.GetLookup_TableDirEmbed(_language, columnName, columnName.Substring(0, columnName.Length - 3));

                for (int i = 0; i < values.Length; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        //try
                        //{
                        string sqlCheckSummary = "SELECT IsSummary FROM " + tableName + " WHERE " + columnName + "=" + values[i];
                        object val = DB.ExecuteScalar(sqlCheckSummary);
                        if (val != null && val != DBNull.Value)
                        {

                            if (val.ToString().Equals("N"))     // If non-summary is selected then add it string and continue to next ID
                            {
                                if (nonSummaryResult.Length > 0)
                                {
                                    nonSummaryResult.Append(", " + values[i]);
                                }
                                else
                                {
                                    nonSummaryResult.Append(values[i]);
                                }
                                continue;
                            }
                        }
                        //}
                        //catch
                        //{
                        //    result = "";
                        //    continue;
                        //}

                        // Fetch child records from tree hierarchy based on ID selected.
                        result1 = Query.GetTreeWhereClause(_ctx, columnName, _PA_Hierarchy_ID, Convert.ToInt32(values[i]));


                        if (result1.IndexOf("(") > -1)
                        {
                            result1 = result1.Substring(result1.IndexOf("(") + 1);
                            result1 = result1.Substring(0, result1.IndexOf(")"));
                        }
                        else
                        {
                            result1 = result1.Substring(result1.IndexOf("=") + 1);
                        }

                        //create list of sleected IDs in stringbuilder
                        if (result1 != values[i] && result1.Length > 0)
                        {
                            if (finalResult.Length > 0)
                            {
                                finalResult.Append(", " + result1);
                            }
                            else
                            {
                                finalResult.Append(result1);
                            }
                        }
                    }
                }
            }




            StringBuilder identifiedsName = new StringBuilder();
            if (finalResult.Length > 0)
            {
                if (finalResult.ToString().IndexOf(",") > -1)
                {
                    eSql = eSql + " AND " + columnName + " IN (" + finalResult.ToString() + ")";
                }
                else
                {
                    eSql = eSql + " AND " + columnName + " = " + finalResult.ToString();
                }
                //eSql = eSql + " AND " + result1;


                //if (!string.IsNullOrEmpty(finalResult.ToString()))
                //{
                //    result = result + "," + finalResult.ToString();
                //}

                DataSet dsIdeintifiers = DB.ExecuteDataset(eSql);



                if (ShowChildOfSelected && (dsIdeintifiers != null && dsIdeintifiers.Tables[0].Rows.Count > 0))
                {
                    for (int s = 0; s < dsIdeintifiers.Tables[0].Rows.Count; s++)
                    {
                        if (identifiedsName.Length > 0)
                        {
                            identifiedsName.Append(", ");
                        }
                        identifiedsName.Append(dsIdeintifiers.Tables[0].Rows[s][0]);
                    }
                }

            }
            if (nonSummaryResult.Length > 0 || finalResult.Length > 0)
            {
                if (nonSummaryResult.Length > 0)
                {
                    result = nonSummaryResult.ToString();
                }

                if (finalResult.Length > 0)
                {
                    if (result.Length > 0)
                    {
                        result += ", ";
                    }
                    result += finalResult.ToString();
                }

            }



            if (identifiedsName != null)
            {
                return identifiedsName.ToString();
            }

            return "";
        }


        /// <summary>
        /// Set param from db
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static void SetParameterFromDB(ProcessInfo pi)
        {
            List<ProcessInfoParameter> list = new List<ProcessInfoParameter>();
            String sql = "SELECT p.ParameterName,"         			    	//  1
                   + " p.P_String,p.P_String_To, p.P_Number,p.P_Number_To,"    //  2/3 4/5
                   + " p.P_Date,p.P_Date_To, p.Info,p.Info_To, "               //  6/7 8/9
                   + " i.AD_Client_ID, i.AD_Org_ID, i.AD_User_ID "				//	10..12
                // +" p.P_Date_Time,p.P_Date_Time_To,p.P_Time, p.P_Time_To "
                   + "FROM AD_PInstance_Para p"
                   + " INNER JOIN AD_PInstance i ON (p.AD_PInstance_ID=i.AD_PInstance_ID) "
                   + "WHERE p.AD_PInstance_ID=@pinstanceid "
                   + "ORDER BY p.SeqNo";


            //            String sql = @"SELECT ip.ParameterName,
            //                                      ip.P_String,
            //                                      ip.P_String_To,
            //                                      ip.P_Number,
            //                                      ip.P_Number_To,
            //                                      ip.P_Date,
            //                                      ip.P_Date_To,
            //                                      ip.Info,
            //                                      ip.Info_To,
            //                                      i.AD_Client_ID,
            //                                      i.AD_Org_ID,
            //                                      i.AD_User_ID,
            //                                      ip.P_Date_Time,
            //                                      ip.P_Date_Time_To,
            //                                      ip.P_Time,
            //                                      ip.P_Time_To ,
            //                                     pp.AD_REFERENCE_ID
            //                                    FROM AD_PInstance_Para ip JOIN AD_PInstance i ON (ip.AD_PINstance_ID=i.AD_PINstance_ID)
            //                                    LEFT  outer JOIN   AD_Process_Para pp ON (pp.AD_Process_ID=i.AD_Process_ID)
            //                                    WHERE ip.AD_PInstance_ID =@pinstanceid";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", pi.GetAD_PInstance_ID());
                //param[0] = new SqlParameter("@pinstanceid", 1000296);

                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    String ParameterName = dr[0].ToString();
                    //	String
                    Object Parameter = dr[1].ToString();
                    Object Parameter_To = dr[2].ToString();

                    Parameter = Parameter.ToString() == "" ? null : Parameter;
                    Parameter_To = Parameter_To.ToString() == "" ? null : Parameter_To;

                    //int displayType = 0;
                    //if (dr[16] != null && dr[16] != DBNull.Value)
                    //{
                    //    displayType = Util.GetValueOfInt(dr[16]);
                    //}

                    //	Big Decimal
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {

                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter = Utility.Util.GetValueOfDecimal(dr[3]);
                        }
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter_To = Utility.Util.GetValueOfDecimal(dr[4]);
                        }
                    }
                    //	Timestamp
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        //if (displayType == 0)
                        //{
                        //    if (!(dr[5] == DBNull.Value))
                        //    {
                        //        Parameter = DateTime.Parse(dr[5].ToString());
                        //    }
                        //    if (!(dr[6] == DBNull.Value))
                        //    {
                        //        Parameter_To = DateTime.Parse(dr[6].ToString());
                        //    }
                        //}
                        //else
                        //{
                        //if (displayType == DisplayType.Date)
                        //{
                        if (dr[5] != null && dr[5] != DBNull.Value)
                        {
                            Parameter = DateTime.Parse(dr[5].ToString());
                        }
                        if (dr[6] != null && dr[6] != DBNull.Value)
                        {
                            Parameter_To = DateTime.Parse(dr[6].ToString());
                        }
                        //}
                        //else if (displayType == DisplayType.DateTime)
                        //{
                        //if (dr[12] != null && dr[12] != DBNull.Value)
                        //{
                        //    Parameter = DateTime.Parse(dr[12].ToString());
                        //}
                        //if (dr[13] != null && dr[13] != DBNull.Value)
                        //{
                        //    Parameter_To = DateTime.Parse(dr[13].ToString());
                        //}
                        ////}
                        ////else if (displayType == DisplayType.Time)
                        ////{
                        //if (dr[14] != null && dr[14] != DBNull.Value)
                        //{
                        //    Parameter = DateTime.Parse(dr[14].ToString());
                        //}
                        //if (dr[15] != null && dr[15] != DBNull.Value)
                        //{
                        //    Parameter_To = DateTime.Parse(dr[15].ToString());
                        //}
                        //}
                        //}
                    }
                    //	Info
                    String Info = dr[7].ToString();
                    String Info_To = dr[8].ToString();
                    //
                    list.Add(new ProcessInfoParameter(ParameterName, Parameter, Parameter_To, Info, Info_To));
                    //
                    if (pi.GetAD_Client_ID() == null)
                        pi.SetAD_Client_ID(int.Parse(dr[9].ToString()));
                    if (pi.GetAD_User_ID() == null)
                        pi.SetAD_User_ID(int.Parse(dr[11].ToString()));
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Severe(e.ToString());
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //
            ProcessInfoParameter[] pars = new ProcessInfoParameter[list.Count()];
            pars = list.ToArray();
            pi.SetParameter(pars);
        }   //  setParameterFromDB


        /// <summary>
        /// Set param from db
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static ProcessInfoParameter[] SetCrystalParameterFromDB(int id)
        {
            List<ProcessInfoParameter> list = new List<ProcessInfoParameter>();
            String sql = "SELECT p.ParameterName,"         			    	//  1
                + " p.P_String,p.P_String_To, p.P_Number,p.P_Number_To,"    //  2/3 4/5
                + " p.P_Date,p.P_Date_To, p.Info,p.Info_To, "               //  6/7 8/9
                + " i.AD_Client_ID, i.AD_Org_ID "				//	10..12
                + " FROM AD_CrystalInstance_Para p"
                + " INNER JOIN AD_CrystalInstance i ON (p.AD_CrystalInstance_ID=i.AD_CrystalInstance_ID) "
                + "WHERE p.AD_CrystalInstance_ID=@pinstanceid "
                + "ORDER BY p.SeqNo";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", id);
                //param[0] = new SqlParameter("@pinstanceid", 1000296);

                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    String ParameterName = dr[0].ToString();
                    //	String
                    Object Parameter = dr[1].ToString();
                    Object Parameter_To = dr[2].ToString();
                    //	Big Decimal
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter = Utility.Util.GetValueOfDecimal(dr[3]);
                        }
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter_To = Utility.Util.GetValueOfDecimal(dr[4]);
                        }
                    }
                    //	Timestamp
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        if (!(dr[5] == DBNull.Value))
                        {
                            Parameter = DateTime.Parse(dr[5].ToString());
                        }
                        if (!(dr[6] == DBNull.Value))
                        {
                            Parameter_To = DateTime.Parse(dr[6].ToString());
                        }
                    }
                    //	Info
                    String Info = dr[7].ToString();
                    String Info_To = dr[8].ToString();
                    //
                    list.Add(new ProcessInfoParameter(ParameterName, Parameter, Parameter_To, Info, Info_To));
                    //
                    //if (pi.GetAD_Client_ID() == null)
                    //    pi.SetAD_Client_ID(int.Parse(dr[9].ToString()));
                    //if (pi.GetAD_User_ID() == null)
                    //    pi.SetAD_User_ID(int.Parse(dr[11].ToString()));
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Severe(e.ToString());
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //
            ProcessInfoParameter[] pars = new ProcessInfoParameter[list.Count()];
            pars = list.ToArray();
            return pars;
            //pi.SetParameter(pars);
        }   //  setParameterFromDB

        public static void SetLogFromDB(ProcessInfo pi)
        {
            String sql = "SELECT Log_ID, P_ID, P_Date, P_Number, P_Msg "
                + "FROM AD_PInstance_Log "
                + "WHERE AD_PInstance_ID=@instanceid "
                + "ORDER BY Log_ID";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@instanceid", pi.GetAD_PInstance_ID());
                dr = DataBase.DB.ExecuteReader(sql, param);

                int? ival;
                while (dr.Read())
                {
                    if (dr[1].ToString() == "")
                    {
                        ival = null;
                    }
                    else
                    {
                        ival = (int?)Utility.Util.GetValueOfInt(dr[1]);
                    }
                    pi.AddLog(Utility.Util.GetValueOfInt(dr[0]), ival, Utility.Util.GetValueOfDateTime(dr[2]), Utility.Util.GetValueOfDecimal(dr[3]), dr[4].ToString());
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, "setLogFromDB", e);
            }



        }

        public static void SaveLogToDB(ProcessInfo pi)
        {
            Context p_ctx = Env.GetContext();
            var org = System.Threading.Thread.CurrentThread.CurrentCulture;

            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            ProcessInfoLog[] logs = pi.GetLogs();
            if (logs == null || logs.Length == 0)
            {
                _log.Fine("No Log");
                return;
            }
            if (pi.GetAD_PInstance_ID() == 0)
            {
                _log.Log(Level.WARNING, "AD_PInstance_ID==0");
                return;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());

            for (int i = 0; i < logs.Length; i++)
            {
                StringBuilder sql = new StringBuilder("INSERT INTO AD_PInstance_Log "
                    + "(AD_PInstance_ID, Log_ID, P_Date, P_ID, P_Number, P_Msg)"
                    + " VALUES (");
                sql.Append(pi.GetAD_PInstance_ID()).Append(",")
                    .Append(logs[i].GetLog_ID()).Append(",");
                if (logs[i].GetP_Date() == null)
                    sql.Append("NULL");
                else
                {
                    //true is for only date saved in log
                    sql.Append(GlobalVariable.TO_DATE(logs[i].GetP_Date(), true));
                }
                sql.Append(",");
                if (logs[i].GetP_ID() == 0)
                    sql.Append("NULL");
                else
                    sql.Append(logs[i].GetP_ID());
                sql.Append(",");
                if (logs[i].GetP_Number() == null)
                    sql.Append("NULL");
                else
                    sql.Append(logs[i].GetP_Number());
                sql.Append(",");
                if (logs[i].GetP_Msg() == null)
                    sql.Append("NULL)");
                else
                {
                    sql.Append(GlobalVariable.TO_STRING(logs[i].GetP_Msg(), 2000)).Append(")");
                }

                SqlExec.ExecuteQuery.ExecuteNonQuery(sql.ToString());
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = org;// Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = org;// Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            pi.SetLogList(null);	//	otherwise log entries are twice
        }

    }
}
