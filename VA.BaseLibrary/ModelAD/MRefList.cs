/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRefList
 * Purpose        : 
 * Class Used     : MRefList inherits X_AD_Ref_List
 * Chronological    Development
 * Raghunandan      04-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MRefList : X_AD_Ref_List
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MRefList).FullName);
        // Value Cache						
        private static CCache<string, string> s_cache = new CCache<string, string>("AD_Ref_List", 20);

        /// <summary>
        ///Get Reference List 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">reference</param>
        /// <param name="Value">value</param>
        /// <param name="trxName">transaction</param>
        /// <returns>List or null</returns>
        public static MRefList Get(Ctx ctx, int AD_Reference_ID, String Value, Trx trxName)
        {
            MRefList retValue = null;
            String sql = "SELECT * FROM AD_Ref_List "
                + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND Value=" + Value;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MRefList(ctx, rs, trxName);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;

            return retValue;
        }

        /// <summary>
        ///Get Reference List Value Name (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">reference</param>
        /// <param name="Value">value</param>
        /// <returns>List or null</returns>
        public static String GetListName(Ctx ctx, int AD_Reference_ID, String value)
        {
            //String AD_Language = Env.getAD_Language(ctx);
            string AD_Language = ctx.GetAD_Language();
            string key = AD_Language + "_" + AD_Reference_ID + "_" + value;
            string retValue = s_cache[key];
            if (retValue != null)
                return retValue;

            //bool isBaseLanguage = GlobalVariable.IsBaseLanguage(AD_Language, "AD_Ref_List");
            bool isBaseLanguage = Utility.Env.IsBaseLanguage(ctx, "AD_Ref_List");// GlobalVariable.IsBaseLanguage();
            String sql = isBaseLanguage ?
                "SELECT Name FROM AD_Ref_List "
                + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND Value='" + value + "'" :
                "SELECT t.Name FROM AD_Ref_List_Trl t"
                + " INNER JOIN AD_Ref_List r ON (r.AD_Ref_List_ID=t.AD_Ref_List_ID) "
                + "WHERE r.AD_Reference_ID=" + AD_Reference_ID + " AND r.Value='" + value + "' AND t.AD_Language=" + AD_Language;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    //retValue = rs.getString(1);
                    retValue = rs[0].ToString();//.getString(1);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql + " - " + key, ex);
            }
            ds = null;

            //	Save into Cache
            if (retValue == null)
            {
                retValue = "";
                _log.Warning("Not found " + key);
            }
            s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Reference List
        /// </summary>
        /// <param name="AD_Reference_ID">reference</param>
        /// <param name="optional">optional if true add "",""</param>
        /// <returns>List or null</returns>
        public static ValueNamePair[] GetList(int AD_Reference_ID, bool optional)
        {
           
              string  sql = "SELECT Value, Name FROM AD_Ref_List "
               + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND IsActive='Y' ORDER BY 1";
            

            DataSet ds = null;
            List<ValueNamePair> list = new List<ValueNamePair>();
            if (optional)
                list.Add(new ValueNamePair("", ""));
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    //list.add(new ValueNamePair(rs.getString(1), rs.getString(2)));
                    list.Add(new ValueNamePair(rs[0].ToString(), rs[1].ToString()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            ValueNamePair[] retValue = new ValueNamePair[list.Count];
            //list.toArray(retValue);
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Get Reference List
        /// </summary>
        /// <param name="AD_Reference_ID">reference</param>
        /// <param name="optional">optional if true add "",""</param>
        /// <returns>List or null</returns>
        public static ValueNamePair[] GetList(int AD_Reference_ID, bool optional,Ctx ctx)
        {
            bool isBaseLanguage = Utility.Env.IsBaseLanguage(ctx, "AD_Ref_List");
            string sql = string.Empty;
            if (isBaseLanguage)
            {
                sql = "SELECT Value, Name FROM AD_Ref_List "
               + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND IsActive='Y' ORDER BY 1";
            }
            else
            {
                sql = @"SELECT rl.Value,
                          rlt.Name AS Name
                        FROM AD_Ref_List rl
                        INNER JOIN AD_Ref_List_trl rlt
                        ON (rlt.ad_ref_list_id  =rl.ad_ref_list_id
                        AND rlt.ad_language     ='" + ctx.GetAD_Language() + @"')
                        WHERE rl.AD_Reference_ID=" + AD_Reference_ID + @"
                        AND rl.IsActive         ='Y'
                        ORDER BY 1";
            }

            DataSet ds = null;
            List<ValueNamePair> list = new List<ValueNamePair>();
            if (optional)
                list.Add(new ValueNamePair("", ""));
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    //list.add(new ValueNamePair(rs.getString(1), rs.getString(2)));
                    list.Add(new ValueNamePair(rs[0].ToString(), rs[1].ToString()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            ValueNamePair[] retValue = new ValueNamePair[list.Count];
            //list.toArray(retValue);
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Ref_List_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MRefList(Ctx ctx, int AD_Ref_List_ID, Trx trxName)
            : base(ctx, AD_Ref_List_ID, trxName)
        {
            if (AD_Ref_List_ID == 0)
            {
                //	setAD_Reference_ID (0);
                //	setAD_Ref_List_ID (0);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                //	setName (null);
                //	setValue (null);
            }
        }

        /// <summary>
        ///Load Contructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result</param>
        /// <param name="trxName">transaction</param>
        public MRefList(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>Name</returns>
        public override String ToString()
        {
            return GetName();
        }

        /// <summary>
        ///String Info
        /// </summary>
        /// <returns>info</returns>
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("MRefList[");
            sb.Append(Get_ID())
                .Append("-").Append(GetValue()).Append("=").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}
