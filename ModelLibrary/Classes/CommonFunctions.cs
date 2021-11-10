/***************************************
 *  File Name       :   CommonFunction.cs
 *  Purpose         :   provides common functions to all the pages for common tasks.
 *  Class Used      :   
 *  Chronological Development
 *  
 *************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Threading;
using System.Resources;
using System.Globalization;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.Controller;

namespace VAdvantage.Classes
{
    public class CommonFunctions
    {

        private string _strTableName;
        public string strTableName
        {
            get { return _strTableName; }
            set { _strTableName = value; }
        }

        public enum ToolStripItemType
        {
            New,
            Delete,
            Save,
            Undo,
            First,
            Next,
            Previous,
            Last,
            PrintScreen,
            ScreenShot,
            Exit,
            ProductInfo,
            Preference,
            ActiveWorkflow,
            Help,
            About,
            Toggel
        };

        public enum ButtonType
        {
            OK,
            Cancel
        };


        /// <summary>
        /// Used in workflow
        /// </summary>
        //public enum Calendar
        //{
        //    Minute = 12,
        //    Second = 13,
        //    Hour = 10,
        //    DayOfMonth = 5,
        //    DayOfYear = 6,
        //    Month = 2,
        //    Year = 1
        //};

        //static Context ctx = Utility.Env.GetContext();
        /// <summary>
        /// Get primary key ID of table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <returns>int primary key</returns>
        /// <author>Veena</author>
        /// 
#pragma warning disable 612, 618
        public static int GetPrimaryKeyId(string tableName)
        {
            try
            {
                int nextSeqID = 0;
                int clientID = Utility.Env.GetContext().GetAD_Client_ID();

                //int iOrgid = 1;
                //string strQuery = "SELECT CurrentNext FROM AD_Sequence WHERE Name='" + tableName + "' AND IsActive = 'Y' AND IsTableID = 'Y'";
                //int iOrgid = Utility.Util.GetValueOfInt(ExecuteQuery.ExecuteScalar(strQuery).ToString());
                //string strUpdate = "UPDATE AD_Sequence SET CurrentNext=CurrentNext+IncrementNo, updated=getdate() WHERE Name='" + tableName + "'";
                //ExecuteQuery.ExecuteNonQuery(strUpdate);

                //string strUpdate = "UPDATE AD_Sequence SET CurrentNext=(Select " + strColumn + " from " + tableName+"), updated=getdate() WHERE Name=" + tableName + "";
                //ExecuteQuery.ExecuteNonQuery(strUpdate);

                // get table's sequence id
                string sqlSeqQry = "SELECT AD_Sequence_ID FROM ad_sequence WHERE UPPER(NAME)=UPPER('" + tableName + "')";
                string tableSeqID = ExecuteQuery.ExecuteScalar(sqlSeqQry);

                char isSystem = 'N';
                if (clientID < 12)
                {
                    //isSystem = 'Y';
                }

                string dbConn = DB.GetConnectionString();
                if (DatabaseType.IsOracle)
                {
                    OracleConnection conn = new OracleConnection(dbConn);
                    //Initialize OracleCommand object for insert.
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "NEXTID";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_AD_Sequence_ID", OracleDbType.Decimal).Value = Utility.Util.GetValueOfInt(tableSeqID);
                    cmd.Parameters.Add("p_System", OracleDbType.Char).Value = isSystem;
                    cmd.Parameters.Add("o_NextID", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                    //Open connection and execute insert query.
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    nextSeqID = Utility.Util.GetValueOfInt(cmd.Parameters["o_NextID"].Value.ToString());
                }
                else if (DatabaseType.IsPostgre)
                {
                    //dbConn = "Server = localhost; Port = 5432; User Id = postgres; Password = system; Database = postgres";
                    string procedureName = "nextid";
                    if (GlobalVariable.GetDBName != "")
                    {
                        procedureName = GlobalVariable.GetDBName + "." + procedureName;
                    }
                    string sqlUpdate = "select " + procedureName + "(" + Utility.Util.GetValueOfInt(tableSeqID) + ",'" + isSystem + "')";
                    nextSeqID = Utility.Util.GetValueOfInt(ExecuteQuery.ExecuteScalar(sqlUpdate).ToString());

                    //NpgsqlConnection conn = new NpgsqlConnection(dbConn);
                    ////Initialize SqlCommand object for insert.
                    //NpgsqlCommand cmd = new NpgsqlCommand();
                    //cmd.Connection = conn;
                    //cmd.CommandText = "NEXTID";
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //cmd.Parameters.Add("p_AD_Sequence_ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = Utility.Util.GetValueOfInt(tableSeqID);// 721
                    //cmd.Parameters.Add("p_System", NpgsqlTypes.NpgsqlDbType.Char).Value = isSystem;
                    //cmd.Parameters.Add("o_NextID", NpgsqlTypes.NpgsqlDbType.Integer).Direction = System.Data.ParameterDirection.Output;
                    ////Open connection and execute insert query.
                    //conn.Open();
                    //cmd.ExecuteNonQuery();
                    //conn.Close();
                    //nextSeqID = Utility.Util.GetValueOfInt(cmd.Parameters["o_NextID"].Value.ToString());
                }
                else if (DatabaseType.IsMSSql)
                {
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@p_TableName", tableName);
                    //param[1] = new SqlParameter("@p_ID", Utility.Util.GetValueOfInt(Utility.Env.GetContext().GetAD_Client_ID()));
                    param[1] = new SqlParameter("@p_ID", 1000002);
                    string seqID = ExecuteQuery.ExecuteScalar("AD_Sequence_Next1", param, true);
                    if (seqID != null)
                    {
                        nextSeqID = Utility.Util.GetValueOfInt(seqID);
                    }
                }
                else if (DatabaseType.IsMySql)
                {
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@p_TableName", tableName);
                    //param[1] = new SqlParameter("@p_ID", Utility.Util.GetValueOfInt(Utility.Env.GetContext().GetAD_Client_ID()));
                    param[1] = new SqlParameter("@p_ID", 1000002);
                    string seqID = ExecuteQuery.ExecuteScalar("AD_Sequence_Next1", param, true);
                    if (seqID != null)
                    {
                        nextSeqID = Utility.Util.GetValueOfInt(seqID);
                    }
                }

                return nextSeqID;
            }
            catch
            {
                return 0;
            }
        }

#pragma warning restore 612, 618

        /// <summary>
        /// Get labels message according to search key
        /// </summary>
        /// <param name="strSearchKey"></param>
        /// <returns>string (message text)</returns>
        //public static string GetMessage(string strSearchKey)
        //{
        //    string strqry = "select count(msgtext) from ad_message where value ='" + strSearchKey + "'";

        //    if (Convert.ToInt16(ExecuteQuery.ExecuteScalar(strqry).ToString()) > 0)
        //    {
        //        if (GlobalVariable.AD_LANGUAGE == "en-US")//(GlobalVariable.AD_BASE_LANGUAGE=="en-US")
        //        {

        //            //put the sql query into the variable for Address text 
        //            strqry = "select msgtext FROM ad_message where value ='" + strSearchKey + "'";
        //        }
        //        else
        //        {
        //            strqry = "select msgtext FROM ad_message_trl where ad_message_id =(select ad_message_id FROM ad_message where value ='" + strSearchKey + "') and ad_language=" + "'" + GlobalVariable.AD_LANGUAGE + "'";
        //        }
        //        return ExecuteQuery.ExecuteScalar(strqry).ToString();
        //    }
        //    return "";
        //}

        /// <summary>
        /// Get language specific text
        /// </summary>
        /// <param name="strKeyword">Names of the keyword of Resource file</param>
        /// <returns>string -- text in selected language</returns>
        /// <author>Veena</author>
        //public static string GetCultureText(string strKeyword)
        //{
        //    string strLangCode = GlobalVariable.AD_LANGUAGE;
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strLangCode);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(strLangCode);
        //    // Declare a Resource Manager instance.
        //    ResourceManager LocRM = new ResourceManager("VAdvantage.Properties.Resources", typeof(CommonFunctions).Assembly);
        //    // Assign the string for the "strMessage" key to a message box.
        //    return LocRM.GetString(strKeyword);
        //}

        /// <summary>
        /// Check if data of a column already exists in database
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="columnName">Column Name whose value has to be checked</param>
        /// <param name="columnValue">value to be checked</param>
        /// <returns>int</returns>
        /// <author>Kiran Sangwan</author>
        public static int CheckUniqueName(string tableName, string columnName, string columnValue)
        {
            string sqlQuery = "select count(1) from " + tableName + " where " + columnName + "='" + columnValue + "' and AD_CLIENT_ID=" + Utility.Env.GetContext().GetAD_Client_ID();
            return Utility.Util.GetValueOfInt(ExecuteQuery.ExecuteScalar(sqlQuery).ToString());
        }



        //public static void InsertError(string strExName, string strExMessage)
        //{
        //    int id=CommonFunctions.GetPrimaryKeyId("AD_Error");
        //    string strQuery = "insert into AD_Error(AD_Client_Id,AD_ERROR_ID,AD_Org_Id,AD_LANGUAGE," +
        //        "CODE,NAME) values (" + Utility.Env.GetContext().GetAD_Client_ID() + "," + id + "," + GlobalVariable.AD_ORG_ID + ",'" + GlobalVariable.AD_LANGUAGE.Replace("-","_") + "','" + strExMessage + "','" + strExName + "')";
        //    ExecuteQuery.ExecuteNonQuery(strQuery);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_TREE_ID">Tree ID</param>
        /// <param name="strTable">Table Name</param>
        /// <param name="ChkecSeqno"></param>
        /// <returns></returns>
        /// <author>Kiran Sangwan</author>
        public static int GetTotalMenues(int AD_TREE_ID, string strTable, bool ChkecSeqno)
        {
            string strQuery = "select count(1) from " + strTable + " where AD_Tree_Id=" + AD_TREE_ID;
            if (ChkecSeqno == true)
            {
                strQuery += " and seqno=9999";
            }
            return int.Parse(ExecuteQuery.ExecuteScalar(strQuery));
        }


        /// <summary>
        /// Get Root Node
        /// </summary>
        /// <param name="TreeType">"MM" or "OO"</param>
        /// <returns></returns>
        public static int GetRootNode(string TreeType)
        {
            int AD_Tree_ID = 0;
            if (int.Parse(ExecuteQuery.ExecuteScalar("select count(1) from ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " and treetype='" + TreeType + "' and isAllNodes='Y'")) > 0)
            {
                if (int.Parse(ExecuteQuery.ExecuteScalar("select count(1) from ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " and " +
                "isdefault='Y' and treetype='" + TreeType + "'").ToString()) > 0)
                {
                    AD_Tree_ID = int.Parse(SqlExec.ExecuteQuery.ExecuteScalar("select ad_TREE_ID" +
                   " from ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " and isdefault='Y' AND " +
                   "created <=(select MIN(created) FROM ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " " +
                   "and isdefault='Y' and TREETYPE='" + TreeType + "') and TREETYPE='" + TreeType + "'"));
                }
                else
                {
                    AD_Tree_ID = int.Parse(SqlExec.ExecuteQuery.ExecuteScalar("select ad_TREE_ID" +
                  " from ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " and isAllNodes='Y' AND " +
                  "created <=(select MIN(created) FROM ad_tree where ad_client_id=" + Utility.Env.GetContext().GetAD_Client_ID() + " " +
                  "and isAllNodes='Y' and TREETYPE='" + TreeType + "') and TREETYPE='" + TreeType + "'"));
                }

            }
            return AD_Tree_ID;
        }

        /// <summary>
        /// Return string value
        /// </summary>
        /// <param name="obj">value to convert</param>
        /// <returns>Object</returns>
        /// <Author>Harwinder</Author> 
        public static string GetString(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }
        /// <summary>
        /// return Int value
        /// </summary>
        /// <param name="obj">value to convert</param>
        /// <returns></returns>
        /// <Author>Harwinder</Author> 
        public static int GetInt(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return 0;
            }
            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// Set The cursors
        /// </summary>
        /// <param name="curType"></param>


        /// <summary>
        /// to set shortcut key of menu items
        /// </summary>
        /// <param name="objMenuStrip">object of menustrip</param>
        /// <returns></returns>



        ///// <summary>
        ///// setting on add new click of menustrip 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="objBindingSource">bindingSource object</param>
        /////  <param name="objNavigator">BindingNavigator object</param>
        ///// <param name="objMenuStrip">MenuStrip Object</param>
        ///// <returns></returns>
        //public static void SettingOnAddNewItem(object sender,System.Windows.Forms.BindingSource objBindingSource, System.Windows.Forms.BindingNavigator objNavigator, System.Windows.Forms.MenuStrip objMenuStrip)
        //{
        //    if (sender.GetType().ToString().ToLower().Contains("menu"))
        //    {
        //        objBindingSource.AddNew();
        //    }

        //    objBindingSource.AllowNew = true;

        //    System.Windows.Forms.ToolStripItem[] objToolStripItem = null;
        //    objToolStripItem = objNavigator.Items.Find("bnavAddNewItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiNew", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavDeleteItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiDelete", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavUndoItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiUndo", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavSaveItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiSave", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //}

        ///// <summary>
        ///// setting on save click of menustrip 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="objBindingSource">bindingSource object</param>
        /////  <param name="objNavigator">BindingNavigator object</param>
        ///// <param name="objMenuStrip">MenuStrip Object</param>
        ///// <returns></returns>
        //public static void SettingOnSave(System.Windows.Forms.BindingNavigator objNavigator, System.Windows.Forms.MenuStrip objMenuStrip)
        //{

        //    System.Windows.Forms.ToolStripItem[] objToolStripItem = null;
        //    objToolStripItem = objNavigator.Items.Find("bnavAddNewItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiNew", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavDeleteItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiDelete", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavUndoItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiUndo", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }


        //}

        ///// <summary>
        ///// setting on undo click of menustrip 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="objBindingSource">bindingSource object</param>
        /////  <param name="objNavigator">BindingNavigator object</param>
        ///// <param name="objMenuStrip">MenuStrip Object</param>
        ///// <returns></returns>
        //public static void SettingOnUndo(System.Windows.Forms.BindingSource objBindingSource, System.Windows.Forms.BindingNavigator objNavigator, System.Windows.Forms.MenuStrip objMenuStrip)
        //{
        //   // objBindingSource.CancelEdit();
        //    System.Windows.Forms.ToolStripItem[] objToolStripItem = null;
        //    objToolStripItem = objNavigator.Items.Find("bnavAddNewItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiNew", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavDeleteItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiDelete", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = true;
        //    }
        //    objToolStripItem = objNavigator.Items.Find("bnavUndoItem", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }
        //    objToolStripItem = objMenuStrip.Items.Find("mnustpiUndo", true);
        //    if (objToolStripItem.Length > 0)
        //    {
        //        System.Windows.Forms.ToolStripItem objItem = objToolStripItem[0];
        //        objItem.Enabled = false;
        //    }

        //}





        public static System.Drawing.Color GetBackGroundColor(bool value)
        {
            if (value)
            {
                return System.Drawing.Color.LightSeaGreen;
            }
            return System.Drawing.SystemColors.Window;

        }

        /// <summary>
        /// Gets whether specified value is a number
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true if given string is a number</returns>
        /// <author>Veena</author>
        public static bool IsNumeric(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?$");
        }

        /// <summary>
        /// Gets time in mili seconds since 1970
        /// </summary>
        /// <returns>long</returns>
        public static long CurrentTimeMillis()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// Gets time in mili seconds since 1970
        /// </summary>
        /// <param name="start">datetime</param>
        /// <returns>Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT represented by passed object</returns>
        public static long CurrentTimeMillis(DateTime start)
        {
            TimeSpan ts = (start - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long mils = (long)ts.TotalMilliseconds;
            //long l = dt.Ticks / TimeSpan.TicksPerMillisecond;
            DateTime myDate = new DateTime((mils * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            return mils;
        }

        /// <summary>
        /// covertmillisecond to date
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>DateTime</returns>
        public static DateTime CovertMilliToDate(long milliseconds)
        {
            DateTime myDate = new DateTime((milliseconds * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            //new DateTime(

            return myDate;

        }

        /// <summary>
        /// covertmillisecond to date
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>DateTime</returns>
        public static String CovertMilliToDateString(long milliseconds)
        {
            DateTime myDate = new DateTime((milliseconds * TimeSpan.TicksPerMillisecond) + 621355968000000000);
            //new DateTime(

            return myDate.ToString();

        }




        private static int counter = -1;
        /// <summary>
        /// Generate random Number
        /// </summary>
        /// <returns>randam number</returns>
        public static int GenerateRandomNo()
        {
            if (counter == -1)
            {
                counter = new Random().Next() & 0xffff;
            }
            counter++;
            return counter;
        }

        /// <summary>
        /// Adds specified number of value to current datetime
        /// </summary>
        /// <param name="duration">integer number from CommonFuctions.Calendar enum</param>
        /// <param name="time"></param>
        /// <returns>new date</returns>
        /// <author>Veena</author>
        public static DateTime AddDate(int duration, object time)
        {
            if (duration == GlobalVariable.DayOfYear)
            {
                return DateTime.Now.AddDays(Convert.ToDouble(time));
            }
            else if (duration == GlobalVariable.Month)
            {
                return DateTime.Now.AddMonths(Utility.Util.GetValueOfInt(time.ToString()));
            }
            else if (duration == GlobalVariable.Hour)
            {
                return DateTime.Now.AddHours(Convert.ToDouble(time));
            }
            else if (duration == GlobalVariable.Minute)
            {
                return DateTime.Now.AddMinutes(Convert.ToDouble(time));
            }
            else if (duration == GlobalVariable.Second)
            {
                return DateTime.Now.AddSeconds(Convert.ToDouble(time));
            }
            else if (duration == GlobalVariable.Year)
            {
                return DateTime.Now.AddYears(Utility.Util.GetValueOfInt(time.ToString()));
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Sync column or table passed in the parameter
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="noColumns">OUT parameter, returns 0 if table is being synched for the first time</param>
        /// <returns>string message</returns>
        public static string SyncColumn(MTable table, MColumn column, out int noColumns)
        {
            DatabaseMetaData md = new DatabaseMetaData();
            String catalog = "";
            String schema = DataBase.DB.GetSchema();

            //get table name
            string tableName = table.GetTableName();

            noColumns = 0;
            string sql = null;
            //get columns of a table
            DataSet dt = md.GetColumns(catalog, schema, tableName);
            md.Dispose();
            //for each column
            for (noColumns = 0; noColumns < dt.Tables[0].Rows.Count; noColumns++)
            {
                string columnName = dt.Tables[0].Rows[noColumns]["COLUMN_NAME"].ToString().ToLower();
                if (!columnName.Equals(column.GetColumnName().ToLower()))
                    continue;

                //check if column is null or not
                string dtColumnName = "is_nullable";
                string value = "YES";
                //if database is oracle
                if (DatabaseType.IsOracle)
                {
                    dtColumnName = "NULLABLE";
                    value = "Y";
                }
                bool notNull = false;
                //check if column is null
                if (dt.Tables[0].Rows[noColumns][dtColumnName].ToString() == value)
                    notNull = false;
                else
                    notNull = true;
                //............................

                //if column is virtual column then alter table and drop this column
                if (column.IsVirtualColumn())
                {
                    sql = "ALTER TABLE " + table.GetTableName()
                   + " DROP COLUMN " + columnName;
                }
                else
                {
                    sql = column.GetSQLModify(table, column.IsMandatory() != notNull);
                    noColumns++;
                    break;
                }
            }
            dt = null;
            //while (rs.next())
            //{
            //    noColumns++;
            //    String columnName = rs.getString ("COLUMN_NAME");
            //    if (!columnName.equalsIgnoreCase(column.getColumnName()))
            //        continue;

            //    //	update existing column
            //    boolean notNull = DatabaseMetaData.columnNoNulls == rs.getInt("NULLABLE");
            //    if (column.isVirtualColumn())
            //        sql = "ALTER TABLE " + table.getTableName() 
            //            + " DROP COLUMN " + columnName;
            //    else
            //        sql = column.getSQLModify(table, column.isMandatory() != notNull);
            //    break;
            //}
            //rs.close();
            //rs = null;

            //	No Table
            if (noColumns == 0)
            {
                sql = table.GetSQLCreate();
            }
            //	No existing column
            else if (sql == null)
            {
                if (column.IsVirtualColumn())
                {
                    return "@IsVirtualColumn@";
                }
                sql = column.GetSQLAdd(table);
            }
            return sql;
        }

        /// <summary>
        /// Parse text
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="po">po object</param>
        /// <returns>parsed text</returns>
        public static string Parse(String text, PO po)
        {
            if (po == null || text.IndexOf("@") == -1)
                return text;

            String inStr = text;
            String token;
            StringBuilder outStr = new StringBuilder();

            int i = inStr.IndexOf("@");
            while (i != -1)
            {
                outStr.Append(inStr.Substring(0, i));			// up to @
                inStr = inStr.Substring(i + 1); ///from first @

                int j = inStr.IndexOf("@");						// next @
                if (j < 0)										// no second tag
                {
                    inStr = "@" + inStr;
                    break;
                }

                token = inStr.Substring(0, j);
                if (token == "Tenant")
                {
                    int id = po.GetAD_Client_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM AD_Client WHERE AD_Client_ID=" + id));
                }
                else if (token == "Org")
                {
                    int id = po.GetAD_Org_ID();
                    outStr.Append(DB.ExecuteScalar("Select Name FROM AD_ORG WHERE AD_ORG_ID=" + id));
                }
                else if (token == "BPName")
                {
                    if (po.Get_TableName() == "C_BPartner")
                    {
                        outStr.Append(ParseVariable("Name", po));
                    }
                    else
                    {
                        outStr.Append("@" + token + "@");
                    }
                }
                else
                {
                    outStr.Append(ParseVariable(token, po));		// replace context
                }
                inStr = inStr.Substring(j + 1);
                // from second @
                i = inStr.IndexOf("@");
            }

            outStr.Append(inStr);           					//	add remainder
            return outStr.ToString();
        }

        /// <summary>
        /// Parse Variable
        /// </summary>
        /// <param name="variable">variable</param>
        /// <param name="po">po object</param>
        /// <returns>translated variable or if not found the original tag</returns>
        private static string ParseVariable(String variable, PO po)
        {
            int index = po.Get_ColumnIndex(variable);
            if (index == -1)
                return "@" + variable + "@";	//	keep for next
            //
            Object value = po.Get_Value(index);
            if (value == null)
                return "";

            POInfo _poInfo = POInfo.GetPOInfo(po.GetCtx(), po.Get_Table_ID());

            MColumn column = (new MTable(po.GetCtx(), po.Get_Table_ID(), null)).GetColumn(variable);
            if (column.GetAD_Reference_ID() == DisplayType.Location)
            {
                StringBuilder sb = new StringBuilder();
                DataSet ds = DB.ExecuteDataset(@"SELECT l.address1,
                                                          l.address2,
                                                          l.address3,
                                                          l.address4,
                                                          l.city,
                                                          CASE
                                                            WHEN l.C_City_ID IS NOT NULL
                                                            THEN
                                                              ( SELECT NAME FROM C_City ct WHERE ct.C_City_ID=l.C_City_ID
                                                              )
                                                            ELSE NULL
                                                          END CityName,
                                                          (SELECT NAME FROM C_Country c WHERE c.C_Country_ID=l.C_Country_ID
                                                          ) AS CountryName
                                                        FROM C_Location l WHERE l.C_Location_ID=" + value);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["address1"] != null && ds.Tables[0].Rows[0]["address1"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address1"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address2"] != null && ds.Tables[0].Rows[0]["address2"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address2"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address3"] != null && ds.Tables[0].Rows[0]["address3"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address3"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["address4"] != null && ds.Tables[0].Rows[0]["address4"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["address4"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["city"] != null && ds.Tables[0].Rows[0]["city"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["city"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["CityName"] != null && ds.Tables[0].Rows[0]["CityName"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["CityName"]).Append(",");
                    }
                    if (ds.Tables[0].Rows[0]["CountryName"] != null && ds.Tables[0].Rows[0]["CountryName"] != DBNull.Value)
                    {
                        sb.Append(ds.Tables[0].Rows[0]["CountryName"]).Append(",");
                    }
                    return sb.ToString().TrimEnd(',');

                }
                else
                {
                    return "";
                }

            }

            //Get lookup display column name for ID 
            if (_poInfo != null && _poInfo.getAD_Table_ID() == po.Get_Table_ID() && _poInfo.IsColumnLookup(index) && value != null)
            {
                VLookUpInfo lookup = _poInfo.GetColumnLookupInfo(index); //create lookup info for column
                DataSet ds = DB.ExecuteDataset(lookup.queryDirect.Replace("@key", DB.TO_STRING(value.ToString())), null); //Get Name from data

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    value = ds.Tables[0].Rows[0][2]; //Name Value
                }
            }



            if (column.GetAD_Reference_ID() == DisplayType.Date)
            {
                //return Util.GetValueOfDateTime(value).Value.Date.ToShortDateString();
                return DisplayType.GetDateFormat(column.GetAD_Reference_ID()).Format(value, po.GetCtx().GetContext("#ClientLanguage"), SimpleDateFormat.DATESHORT);
            }

            // Show Amount according to browser culture
            if (column.GetAD_Reference_ID() == DisplayType.Amount || column.GetAD_Reference_ID() == DisplayType.CostPrice)
            {
                return DisplayType.GetNumberFormat(column.GetAD_Reference_ID()).GetFormatAmount(value, po.GetCtx().GetContext("#ClientLanguage"));
            }
            return value.ToString();
        }

        /// <summary>
        /// Get details of card like included columns, conditions, card template etc.
        /// </summary>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Tab_ID"></param>
        /// <param name="AD_CardView_ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public CardViewData GetCardViewDetails(int AD_User_ID, int AD_Tab_ID, int AD_CardView_ID, Ctx ctx)
        {
            DataSet ds = null;
            bool hasDefaultCard = false;
            if (AD_CardView_ID > 0)
            {
                // If fetching specific card
                ds = DataBase.DB.ExecuteDataset(@"SELECT AD_CardView.AD_CardView_ID, AD_CardView.Name, AD_CardView.AD_HeaderLayout_ID,AD_CardView.AD_Field_ID,AD_HeaderLayout.backgroundcolor,AD_HeaderLayout.padding FROM AD_CardView AD_CardView LEFT OUTER JOIN AD_HeaderLayout AD_HeaderLayout
                        ON (AD_CardView.AD_HeaderLayout_ID = AD_HeaderLayout.AD_HeaderLayout_ID) WHERE AD_CardView.AD_CardView_ID = " + AD_CardView_ID);
            }
            else
            {

                //Fetch default card for login user
                ds = DataBase.DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(@" SELECT AD_CardView.AD_CardView_ID, AD_CardView.Name,AD_CardView.AD_HeaderLayout_ID,AD_CardView.AD_Field_ID,AD_HeaderLayout.backgroundcolor,AD_HeaderLayout.padding,
    AD_DefaultCardView.ad_client_id,
    AD_DefaultCardView.ad_user_ID FROM AD_CardView AD_CardView LEFT OUTER JOIN AD_HeaderLayout AD_HeaderLayout
                        ON ( AD_CardView.AD_HeaderLayout_ID = AD_HeaderLayout.AD_HeaderLayout_ID)
                        INNER JOIN AD_DefaultCardView AD_DefaultCardView ON ( AD_DefaultCardView.ad_cardview_id = AD_CardView.ad_cardview_id)
                       WHERE  AD_CardView.AD_Tab_ID=" + AD_Tab_ID + " AND AD_CardView.IsActive = 'Y' AND (AD_CardView.ad_user_id IS NULL OR AD_CardView.ad_user_id = " + ctx.GetAD_User_ID() + @") " +
                        "ORDER BY AD_DefaultCardView.AD_Client_ID Desc", "AD_CardView", true, false));

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    //If no default card found, then load other cards of tABS
                    ds = DataBase.DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(@"SELECT AD_CardView.AD_CardView_ID, AD_CardView.Name,AD_CardView.AD_HeaderLayout_ID,AD_CardView.AD_Field_ID,AD_HeaderLayout.backgroundcolor,AD_HeaderLayout.padding FROM AD_CardView AD_CardView LEFT OUTER JOIN AD_HeaderLayout AD_HeaderLayout
                        ON (AD_CardView.AD_HeaderLayout_ID = AD_HeaderLayout.AD_HeaderLayout_ID) WHERE AD_CardView.AD_Tab_ID =" + AD_Tab_ID + " AND AD_CardView.IsActive='Y' ORDER BY AD_CardView.Name ASC", "AD_CardView", true, false));
                }
                else
                {
                    hasDefaultCard = true;
                }
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow[] rows = null;
                if (hasDefaultCard)
                {
                    // check if any default card is set by login user.
                    rows = ds.Tables[0].Select("AD_User_ID = " + ctx.GetAD_User_ID());
                    if (rows == null || rows.Length == 0)
                    {
                        // If no default card by user, then try to get default card of tanent.
                        rows = ds.Tables[0].Select("AD_Client_ID = " + ctx.GetAD_Client_ID());
                        if (rows == null || rows.Length == 0)
                        {
                            // if no default card found, then try to  get default set by System administrator
                            rows = ds.Tables[0].Select();
                        }
                    }
                }
                else {
                    rows = ds.Tables[0].Select();
                }


                //for (int i = 0; i < rows.Length; i++)
                //{
                    CardViewData card = new CardViewData()
                    {
                        AD_CardView_ID = Convert.ToInt32(rows[0]["AD_CardView_ID"]),
                        IsDefault = true,
                        Name = Util.GetValueOfString(rows[0]["Name"]),
                        AD_HeaderLayout_ID = Util.GetValueOfInt(rows[0]["AD_HeaderLayout_ID"]),
                        FieldGroupID = Util.GetValueOfInt(rows[0]["AD_Field_ID"]),
                        Style = Util.GetValueOfString(rows[0]["backgroundcolor"]),
                        Padding = Util.GetValueOfString(rows[0]["Padding"])
                    };

                    card.IncludedCols = new List<CardViewCol>();
                    card.Conditions = new List<CardViewCondition>();

                    string sql = "";
                    IDataReader dr = null;
                    int AD_CV_ID = card.AD_CardView_ID;
                    if (AD_CV_ID > 0)
                    {
                    // Fetch included columns
                        sql = "SELECT AD_Field_ID, SeqNo, FieldValueStyle FROM AD_CardView_Column WHERE IsActive='Y' AND AD_CardView_ID = " + AD_CV_ID + " ORDER BY SeqNo";
                        dr = DB.ExecuteReader(sql);
                        while (dr.Read())
                        {
                            card.IncludedCols.Add(
                                new CardViewCol()
                                {
                                    AD_Field_ID = VAdvantage.Utility.Util.GetValueOfInt(dr[0]),
                                    SeqNo = VAdvantage.Utility.Util.GetValueOfInt(dr[1]),
                                    HTMLStyle = VAdvantage.Utility.Util.GetValueOfString(dr[2])
                                });
                        }
                        dr.Close();
                    }
                    if (AD_CV_ID > 0)
                    {
                    //Fetch Conditions
                        sql = "SELECT ConditionValue,Color  FROM AD_CardView_Condition WHERE IsActive='Y' AND AD_CardView_ID = " + AD_CV_ID + " ORDER BY AD_CardView_Condition_ID ";
                        dr = DB.ExecuteReader(sql);
                        while (dr.Read())
                        {
                            var cdc = new CardViewCondition();
                            cdc.Color = dr[1].ToString();
                            cdc.ConditionValue = dr[0].ToString();
                            card.Conditions.Add(cdc);
                        }
                        dr.Close();
                    }

                    //Fetch Card Template
                    card.HeaderItems = GetCardTemplateItems(card.AD_HeaderLayout_ID);

                    return card;
               // }
            }
            return null;
        }

        /// <summary>
        /// Get Card Template details
        /// </summary>
        /// <param name="headerLayoutID"></param>
        /// <returns></returns>
        public List<HeaderPanelGrid> GetCardTemplateItems(int headerLayoutID)
        {
            List<HeaderPanelGrid> hitems = new List<HeaderPanelGrid>();
            DataSet dsGridLayout = DataBase.DB.ExecuteDataset("SELECT * FROM AD_GridLayout  WHERE IsActive='Y' AND AD_HeaderLayout_ID=" + headerLayoutID + " ORDER BY SeqNo Asc");
            if (dsGridLayout != null && dsGridLayout.Tables[0].Rows.Count > 0)
            {
                hitems = new List<HeaderPanelGrid>();

                foreach (DataRow dr in dsGridLayout.Tables[0].Rows)
                {
                    HeaderPanelGrid hGrid = new HeaderPanelGrid
                    {

                        HeaderBackColor = Utility.Util.GetValueOfString(dr["BackgroundColor"]),

                        HeaderName = Utility.Util.GetValueOfString(dr["Name"]),

                        HeaderTotalColumn = Utility.Util.GetValueOfInt(dr["TotalColumns"]),

                        HeaderTotalRow = Utility.Util.GetValueOfInt(dr["TotalRows"]),

                        HeaderPadding = Utility.Util.GetValueOfString(dr["Padding"]),

                        AD_GridLayout_ID = Utility.Util.GetValueOfInt(dr["AD_GridLayout_ID"]),
                    };

                    DataSet ds = DataBase.DB.ExecuteDataset("SELECT AlignItems,    ColumnSpan,   Justifyitems,   Rowspan,   Seqno,   Startcolumn,   Startrow," +
                        " AD_GridLayoutItems_ID,BackgroundColor, FontColor, FontSize,padding, ColumnSql,HideFieldIcon, HideFieldText, FieldValueStyle FROM Ad_Gridlayoutitems WHERE IsActive ='Y' AND AD_GridLayout_ID=" + hGrid.AD_GridLayout_ID + " ORDER BY Seqno ");
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        hGrid.HeaderItems = new Dictionary<int, object>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            hGrid.HeaderItems[Convert.ToInt32(row["SeqNo"])] = new HeaderPanelItemsVO
                            {
                                AD_GridLayoutItems_ID = Convert.ToInt32(row["AD_GridLayoutItems_ID"]),
                                ColumnSpan = Convert.ToInt32(row["ColumnSpan"]),
                                AlignItems = Convert.ToString(row["AlignItems"]),
                                JustifyItems = Convert.ToString(row["JustifyItems"]),
                                RowSpan = Convert.ToInt32(row["RowSpan"]),
                                SeqNo = Convert.ToInt32(row["SeqNo"]),
                                StartColumn = Convert.ToInt32(row["StartColumn"]),
                                StartRow = Convert.ToInt32(row["StartRow"]),
                                BackgroundColor = Convert.ToString(row["BackgroundColor"]),
                                FontColor = Convert.ToString(row["FontColor"]),
                                FontSize = Convert.ToString(row["FontSize"]),
                                Padding = Convert.ToString(row["Padding"]),
                                ColSql = Convert.ToString(row["ColumnSql"]),
                                HideFieldIcon = Util.GetValueOfString(row["HideFieldIcon"]) == "Y",
                                HideFieldText = Util.GetValueOfString(row["HideFieldtext"]) == "Y",
                                FieldValueStyle = Convert.ToString(row["FieldValueStyle"])
                            };
                        }
                    }
                    hitems.Add(hGrid);
                }
            }
            return hitems;
        }

        //VIS0008 Added to check table existense in database
        static Classes.CCache<string, bool> _cacheTblName = new Classes.CCache<string, bool>("DBColl_TablesExist", 100);

        /// <summary>
        /// Check whether table exist in database
        /// </summary>
        /// <param name="table_catalog">For Oracle - User_ID, For PostGre -- DataBase Name</param>
        /// <param name="tableName">tableName</param>
        /// <returns>true/false</returns>
        public static bool IsTableExists(string table_catalog, string tableName)
        {
            if (_cacheTblName.ContainsKey(tableName.ToUpper()))
            {
                return _cacheTblName[tableName.ToUpper()];
            }
            else
            {
                bool tblExists = Util.GetValueOfInt(DB.ExecuteScalar(DBFunctionCollection.CheckTableExistence(table_catalog, tableName))) > 0;
                _cacheTblName.Add(tableName.ToUpper(), tblExists);
                return tblExists;
            }
        }
    }

   
}
