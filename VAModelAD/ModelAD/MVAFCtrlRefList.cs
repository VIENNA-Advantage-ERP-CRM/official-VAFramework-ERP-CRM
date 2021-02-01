/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRefList
 * Purpose        : 
 * Class Used     : MRefList inherits X_VAF_CtrlRef_List
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

using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFCtrlRefList : X_VAF_CtrlRef_List
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFCtrlRefList).FullName);
        // Value Cache						
        private static CCache<string, string> s_cache = new CCache<string, string>("VAF_CtrlRef_List", 20);

        /// <summary>
        ///Get Reference List 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Control_Ref_ID">reference</param>
        /// <param name="Value">value</param>
        /// <param name="trxName">transaction</param>
        /// <returns>List or null</returns>
        public static MVAFCtrlRefList Get(Ctx ctx, int VAF_Control_Ref_ID, String Value, Trx trxName)
        {
            MVAFCtrlRefList retValue = null;
            String sql = "SELECT * FROM VAF_CtrlRef_List "
                + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND Value=" + Value;
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MVAFCtrlRefList(ctx, rs, trxName);
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
        /// <param name="VAF_Control_Ref_ID">reference</param>
        /// <param name="Value">value</param>
        /// <returns>List or null</returns>
        public static String GetListName(Ctx ctx, int VAF_Control_Ref_ID, String value)
        {
            //String VAF_Language = Env.getVAF_Language(ctx);
            string VAF_Language = ctx.GetVAF_Language();
            string key = VAF_Language + "_" + VAF_Control_Ref_ID + "_" + value;
            string retValue = s_cache[key];
            if (retValue != null)
                return retValue;

            //bool isBaseLanguage = GlobalVariable.IsBaseLanguage(VAF_Language, "VAF_CtrlRef_List");
            bool isBaseLanguage = Utility.Env.IsBaseLanguage(ctx, "VAF_CtrlRef_List");// GlobalVariable.IsBaseLanguage();
            String sql = isBaseLanguage ?
                "SELECT Name FROM VAF_CtrlRef_List "
                + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND Value='" + value + "'" :
                "SELECT t.Name FROM VAF_CtrlRef_TL t"
                + " INNER JOIN VAF_CtrlRef_List r ON (r.VAF_CtrlRef_List_ID=t.VAF_CtrlRef_List_ID) "
                + "WHERE r.VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND r.Value='" + value + "' AND t.VAF_Language=" + VAF_Language;
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
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
        /// <param name="VAF_Control_Ref_ID">reference</param>
        /// <param name="optional">optional if true add "",""</param>
        /// <returns>List or null</returns>
        public static ValueNamePair[] GetList(int VAF_Control_Ref_ID, bool optional)
        {
           
              string  sql = "SELECT Value, Name FROM VAF_CtrlRef_List "
               + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND IsActive='Y' ORDER BY 1";
            

            DataSet ds = null;
            List<ValueNamePair> list = new List<ValueNamePair>();
            if (optional)
                list.Add(new ValueNamePair("", ""));
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
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
        /// <param name="VAF_Control_Ref_ID">reference</param>
        /// <param name="optional">optional if true add "",""</param>
        /// <returns>List or null</returns>
        public static ValueNamePair[] GetList(int VAF_Control_Ref_ID, bool optional,Ctx ctx)
        {
            bool isBaseLanguage = Utility.Env.IsBaseLanguage(ctx, "VAF_CtrlRef_List");
            string sql = string.Empty;
            if (isBaseLanguage)
            {
                sql = "SELECT Value, Name FROM VAF_CtrlRef_List "
               + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND IsActive='Y' ORDER BY 1";
            }
            else
            {
                sql = @"SELECT rl.Value,
                          rlt.Name AS Name
                        FROM VAF_CtrlRef_List rl
                        INNER JOIN VAF_CtrlRef_TL rlt
                        ON (rlt.VAF_CtrlRef_List_id  =rl.VAF_CtrlRef_List_id
                        AND rlt.VAF_Language     ='" + ctx.GetVAF_Language() + @"')
                        WHERE rl.VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + @"
                        AND rl.IsActive         ='Y'
                        ORDER BY 1";
            }

            DataSet ds = null;
            List<ValueNamePair> list = new List<ValueNamePair>();
            if (optional)
                list.Add(new ValueNamePair("", ""));
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
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
        /// <param name="VAF_CtrlRef_List_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFCtrlRefList(Ctx ctx, int VAF_CtrlRef_List_ID, Trx trxName)
            : base(ctx, VAF_CtrlRef_List_ID, trxName)
        {
            if (VAF_CtrlRef_List_ID == 0)
            {
                //	setVAF_Control_Ref_ID (0);
                //	setVAF_CtrlRef_List_ID (0);
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
        public MVAFCtrlRefList(Ctx ctx, DataRow rs, Trx trxName)
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
