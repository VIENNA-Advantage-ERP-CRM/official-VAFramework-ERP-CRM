/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLanguage
 * Purpose        : To add/delete/update language in "_trl " table according to the system language
 * Class Used     : MLanguage inherits X_AD_Language
 * Chronological    Development
 * Raghunandan      6-April-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.SqlExec;
using VAdvantage.Common;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Model
{
    public class MLanguage : X_AD_Language
    {
        #region Private Variable
        //Logger
        private static VLogger s_log = VLogger.GetVLogger(typeof(MLanguage).FullName);

        //These tables has display column
        List<String> lstTableHasDisplayCol = new List<string>() { "AD_WINDOW", "AD_FORM", "AD_SHORTCUT" };
        //Locale
        //private Locale m_locale = null;
        //Date Format
        // private SimpleDateFormat m_dateFormat = null;
        #endregion

        /// <summary>
        ///Maintain Translation
        /// </summary>
        /// <param name="add">add if true add missing records - otherwise delete</param>
        /// <returns>number of records deleted/inserted</returns>
        public int Maintain(bool add)
        {
            String sql = "SELECT TableName FROM AD_Table WHERE TableName LIKE '%_Trl' ORDER BY 1";
            DataSet ds = null;
            int retNo = 0;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    if (add)
                    {
                        retNo += AddTable(dr[0].ToString());
                    }
                    else
                    {
                        retNo += DeleteTable(dr[0].ToString());
                    }

                }

                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            return retNo;
        }

        /// <summary>
        ///Get Language Model from AD_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Language_ID">language e.g. 191</param>
        /// <param name="trxName">trx</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, String AD_Language, Trx trxName)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM AD_Language WHERE AD_Language='" + AD_Language + "'";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                DataRow rs;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    rs = ds.Tables[0].Rows[i];
                    lang = new MLanguage(ctx, rs, trxName);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            return lang;
        }

        /// <summary>
        ///Get Language Model from AD_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Language_ID">language e.g. 191</param>
        /// <param name="trxName">trx</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, int AD_Language_ID, Trx trxName)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM AD_Language WHERE AD_Language_ID=" + AD_Language_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                DataRow rs;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    rs = ds.Tables[0].Rows[i];
                    lang = new MLanguage(ctx, rs, trxName);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            return lang;
        }

        /// <summary>
        ///	Get Language Model from Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="lang">language</param>
        /// <returns>language</returns>
        public static MLanguage Get(Ctx ctx, VAdvantage.Login.Language lang)
        {
            return Get(ctx, lang.GetAD_Language());
        }

        /// <summary>
        ///Get Language Model from AD_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Language">language e.g. en_US</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, string AD_Language)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM AD_Language WHERE AD_Language=" + AD_Language;


            return lang;
        }

        /// <summary>
        ///	Standard Constructor - NO NOT USE
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Language_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLanguage(Ctx ctx, int AD_Language_ID, Trx trxName)
            : base(ctx, AD_Language_ID, trxName)
        {
            //super (ctx, AD_Language_ID, trxName);
        }


        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MLanguage(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }
        public MLanguage(Ctx ctx, IDataReader rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }
        /// <summary>
        ///Add Translation to table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns>number of records inserted</returns>
        private int AddTable(String tableName)
        {
            String baseTable = tableName.Substring(0, tableName.Length - 4);
            String sql = "SELECT c.ColumnName "
                + "FROM AD_Column c"
                + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                + "WHERE t.TableName='" + baseTable
                + "'  AND c.IsTranslated='Y' AND c.IsActive='Y' "
                + "ORDER BY 1";
            List<String> columns = new List<string>(5);
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                while (dr.Read())
                {
                    //DataRow dr = ds.Tables[0].Rows[i];
                    //columns.Add(rs.getString(1));
                    //                    columns.Add(ds.Tables[0].Rows[i].ToString());
                    columns.Add(dr[0].ToString());
                }
                //ds = null;
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            //	Columns
            //if (columns.size() == 0)
            if (columns.Count == 0)
            {

                VAdvantage.Logging.VLogger.Get().Log(Level.SEVERE, "No Columns found for " + baseTable);
                //throw new Exception("No Columns found for " + baseTable);
                return 0;
            }

            StringBuilder cols = new StringBuilder();
            //maintain list of display columns
            StringBuilder displayColumns = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                cols.Append(",").Append(columns[i].ToString());

                if (lstTableHasDisplayCol.Contains(baseTable.ToUpper()) && columns[i].ToString().ToUpper().Equals("NAME"))
                {
                    // In case of Name we need to get value from DisplayName.
                    displayColumns.Append(",").Append("DisplayName");
                }
                else
                {
                    displayColumns.Append(",").Append(columns[i].ToString());
                }
            }

            //	Insert Statement
            int AD_User_ID = GetCtx().GetAD_User_ID();
            String keyColumn = baseTable + "_ID";
            String insert = "";
            if (baseTable == "AD_Client" || baseTable == "AD_Org")
            {
                insert = "INSERT INTO " + tableName
                    + "(AD_Language,IsTranslated, AD_Client_ID,AD_Org_ID, "
                    + "CreatedBy,UpdatedBy "
                    + cols.ToString() + ") "
                    + "SELECT '" + GetAD_Language() + "','N', AD_Client_ID,AD_Org_ID, "
                    + AD_User_ID + "," + AD_User_ID
                    + cols.ToString()
                    + " FROM " + baseTable
                    + " WHERE " + keyColumn + " NOT IN (SELECT " + keyColumn
                        + " FROM " + tableName
                        + " WHERE AD_Language='" + GetAD_Language() + "')";
            }
            else
            {
                insert = "INSERT INTO " + tableName
                    + "(AD_Language,IsTranslated, AD_Client_ID,AD_Org_ID, "
                    + "CreatedBy,UpdatedBy, "
                    + keyColumn + cols.ToString() + ") "
                    + "SELECT '" + GetAD_Language() + "','N', AD_Client_ID,AD_Org_ID, "
                    + AD_User_ID + "," + AD_User_ID + ", "
                    + keyColumn + displayColumns.ToString()
                    + " FROM " + baseTable
                    + " WHERE " + keyColumn + " NOT IN (SELECT " + keyColumn
                        + " FROM " + tableName
                        + " WHERE AD_Language='" + GetAD_Language() + "')";
                //	+ " WHERE (" + keyColumn + ",'" + getAD_Language()+ "') NOT IN (SELECT " 
                //		+ keyColumn + ",AD_Language FROM " + tableName + ")";
            }

            int no = DataBase.DB.ExecuteQuery(insert.ToString(), null, Get_TrxName());
            return no;
        }

        /// <summary>][  
        ///Delete Translation
        /// </summary1>
        /// <param name="tableName">table name</param>
        /// <returns>number of records deleted</returns>
        private int DeleteTable(String tableName)
        {
            String sql = "DELETE FROM " + tableName
                + " WHERE AD_Language='" + GetAD_Language() + "'";
            //int no = DataBase.executeUpdate(sql, Get_TrxName());
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return no;
        }


        public static DataTable GetSystemLanguage()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_Language,Name,Name AS DisplayName FROM AD_Language WHERE IsSystemLanguage = 'Y' AND IsActive='Y' Order BY  Name asc");
            if (ds != null)
                return ds.Tables[0];
            return null;
        }


        private void SetAD_Language_ID()
        {
            int AD_Language_ID = GetAD_Language_ID();
            if (AD_Language_ID == 0)
            {
                String sql = "SELECT NVL(MAX(AD_Language_ID), 999999) FROM AD_Language WHERE AD_Language_ID > 1000";
                AD_Language_ID = DB.GetSQLValue(Get_TrxName(), sql);
                SetAD_Language_ID(AD_Language_ID + 1);
            }
        }

        protected override bool BeforeSave(bool newRecord)
        {
            string lang = GetAD_Language();

            if (!lang.Contains("_"))
            {
                log.SaveError("Error?", "Language code must contain _ (undescore)");
                return false;
            }

            try
            {
                new System.Globalization.CultureInfo(lang.Replace("_", "-"));
            }
            catch (Exception ex)
            {
                log.SaveError("Error?", "Language code not supported =>" + ex.Message);
                return false;
            }
            if (newRecord)
                SetAD_Language_ID();
            return base.BeforeSave(newRecord);
        }
    }
}
