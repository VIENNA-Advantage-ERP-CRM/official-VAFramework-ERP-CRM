/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLanguage
 * Purpose        : To add/delete/update language in "_trl " table according to the system language
 * Class Used     : MLanguage inherits X_VAF_Language
 * Chronological    Development
 * Raghunandan      6-April-2009 
  ******************************************************/
using System;
using System.Collections.Generic;

using System.Text;
using VAdvantage.DataBase;

using System.Data;

using VAdvantage.Logging;
using VAdvantage.Utility;



namespace VAdvantage.Model
{
    public class MLanguage : X_VAF_Language
    {
        #region Private Variable
        //Logger
        private static VLogger s_log = VLogger.GetVLogger(typeof(MLanguage).FullName);
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
            String sql = "SELECT TableName FROM VAF_TableView WHERE TableName LIKE '%_Trl' ORDER BY 1";
            DataSet ds = null;
            int retNo = 0;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
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
        ///Get Language Model from VAF_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Language_ID">language e.g. 191</param>
        /// <param name="trxName">trx</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, String VAF_Language, Trx trxName)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM VAF_Language WHERE VAF_Language='" + VAF_Language + "'";
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
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
        ///Get Language Model from VAF_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Language_ID">language e.g. 191</param>
        /// <param name="trxName">trx</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, int VAF_Language_ID, Trx trxName)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM VAF_Language WHERE VAF_Language_ID=" + VAF_Language_ID;
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
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
            return Get(ctx, lang.GetVAF_Language());
        }

        /// <summary>
        ///Get Language Model from VAF_Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Language">language e.g. en_US</param>
        /// <returns>language or null</returns>
        public static MLanguage Get(Ctx ctx, string VAF_Language)
        {
            MLanguage lang = null;
            String sql = "SELECT * FROM VAF_Language WHERE VAF_Language=" + VAF_Language;


            return lang;
        }

        /// <summary>
        ///	Standard Constructor - NO NOT USE
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Language_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLanguage(Ctx ctx, int VAF_Language_ID, Trx trxName)
            : base(ctx, VAF_Language_ID, trxName)
        {
            //super (ctx, VAF_Language_ID, trxName);
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
                + "FROM VAF_Column c"
                + " INNER JOIN VAF_TableView t ON (c.VAF_TableView_ID=t.VAF_TableView_ID) "
                + "WHERE t.TableName='" + baseTable
                + "'  AND c.IsTranslated='Y' AND c.IsActive='Y' "
                + "ORDER BY 1";
            List<String> columns = new List<string>(5);
            IDataReader dr = null;
            try
            {
                dr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, null);
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
            for (int i = 0; i < columns.Count; i++)
            {
                cols.Append(",").Append(columns[i].ToString());
                //cols.Append(",").Append(columns[i].ToString());
            }

            //	Insert Statement
            int AD_User_ID = GetCtx().GetAD_User_ID();
            String keyColumn = baseTable + "_ID";
            String insert = "";
            if (baseTable == "VAF_Client" || baseTable == "VAF_Org")
            {
                insert = "INSERT INTO " + tableName
                    + "(VAF_Language,IsTranslated, VAF_Client_ID,VAF_Org_ID, "
                    + "CreatedBy,UpdatedBy "
                    + cols.ToString() + ") "
                    + "SELECT '" + GetVAF_Language() + "','N', VAF_Client_ID,VAF_Org_ID, "
                    + AD_User_ID + "," + AD_User_ID
                    + cols.ToString()
                    + " FROM " + baseTable
                    + " WHERE " + keyColumn + " NOT IN (SELECT " + keyColumn
                        + " FROM " + tableName
                        + " WHERE VAF_Language='" + GetVAF_Language() + "')";
            }
            else
            {
                insert = "INSERT INTO " + tableName
                    + "(VAF_Language,IsTranslated, VAF_Client_ID,VAF_Org_ID, "
                    + "CreatedBy,UpdatedBy, "
                    + keyColumn + cols.ToString() + ") "
                    + "SELECT '" + GetVAF_Language() + "','N', VAF_Client_ID,VAF_Org_ID, "
                    + AD_User_ID + "," + AD_User_ID + ", "
                    + keyColumn + cols.ToString()
                    + " FROM " + baseTable
                    + " WHERE " + keyColumn + " NOT IN (SELECT " + keyColumn
                        + " FROM " + tableName
                        + " WHERE VAF_Language='" + GetVAF_Language() + "')";
                //	+ " WHERE (" + keyColumn + ",'" + getVAF_Language()+ "') NOT IN (SELECT " 
                //		+ keyColumn + ",VAF_Language FROM " + tableName + ")";
            }

            int no = CoreLibrary.DataBase.DB.ExecuteQuery(insert.ToString(), null, Get_TrxName());
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
                + " WHERE VAF_Language='" + GetVAF_Language() + "'";
            //int no = DataBase.executeUpdate(sql, Get_TrxName());
            int no = CoreLibrary.DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return no;
        }


        public static DataTable GetSystemLanguage()
        {
            DataSet ds = DB.ExecuteDataset("SELECT VAF_Language,Name,Name AS DisplayName FROM VAF_Language WHERE IsSystemLanguage = 'Y' AND IsActive='Y' Order BY  Name asc");
            if (ds != null)
                return ds.Tables[0];
            return null;
        }


        private void SetVAF_Language_ID()
        {
            int VAF_Language_ID = GetVAF_Language_ID();
            if (VAF_Language_ID == 0)
            {
                String sql = "SELECT NVL(MAX(VAF_Language_ID), 999999) FROM VAF_Language WHERE VAF_Language_ID > 1000";
                VAF_Language_ID = DB.GetSQLValue(Get_TrxName(), sql);
                SetVAF_Language_ID(VAF_Language_ID + 1);
            }
        }

        protected override bool BeforeSave(bool newRecord)
        {
            string lang = GetVAF_Language();

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
                SetVAF_Language_ID();
            return base.BeforeSave(newRecord);
        }
    }
}
