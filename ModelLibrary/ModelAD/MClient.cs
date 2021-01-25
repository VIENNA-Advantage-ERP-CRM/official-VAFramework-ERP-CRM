/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_Client
 * Chronological Development
 * ------
 * Veena Pandey     01-June-2009 Added functions of mail
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.IO;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MClient : VAModelAD.Model.MClient
    {
        //Client Info					
        private MClientInfo _info = null;
        /**	Cache						*/
        private static CCache<int, MClient> s_cache = new CCache<int, MClient>("VAF_Client", 3);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MClient).FullName);

        public MClient(Ctx ctx, int VAF_Client_ID, Trx trxName)
            : this(ctx, VAF_Client_ID, false, trxName)
        {

        }


        public static MClient Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetVAF_Client_ID());
        }

        public static MClient Get(Ctx ctx, int VAF_Client_ID)
        {
            int key = VAF_Client_ID;
            MClient client = (MClient)s_cache[key];
            if (client != null)
                return client;
            client = new MClient(ctx, VAF_Client_ID, null);
            if (VAF_Client_ID == 0)
                client.Load((Trx)null);
            s_cache.Add(key, client);
            return client;
        }


        public static MClient[] GetAll(Ctx ctx)
        {
            List<MClient> list = new List<MClient>();
            String sql = "SELECT * FROM VAF_Client";
            try
            {
                DataSet ds = DB.ExecuteDataset(sql, null, null);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MClient client = new MClient(ctx, dr, null);
                    list.Add(client);
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MClient[] RetValue = new MClient[list.Count()];
            RetValue = list.ToArray();
            return RetValue;
        }   //	getAll

        public MClient(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        private bool m_createNew = false;

        public MClient(Ctx ctx, int VAF_Client_ID, bool createNew, Trx trxName)
            : base(ctx, VAF_Client_ID, trxName)
        {
            m_createNew = createNew;
            if (VAF_Client_ID == 0)
            {
                if (m_createNew)
                {
                    //	setValue (null);
                    //	setName (null);
                    SetVAF_Org_ID(0);
                    SetIsMultiLingualDocument(false);
                    SetIsSmtpAuthorization(false);
                    SetIsUseBetaFunctions(false);
                    SetIsServerEMail(false);
                    SetVAF_Language(GlobalVariable.GetLanguageCode());
                    SetAutoArchive(AUTOARCHIVE_None);
                    SetMMPolicy(MMPOLICY_FiFo);	// F
                    SetIsPostImmediate(false);
                    SetIsCostImmediate(false);
                    SetSmtpPort(25);
                    SetIsSmtpTLS(false);
                }
                else
                    Load(Get_TrxName());
            }
        }


  //      public bool IsAutoUpdateTrl(String strTableName)
  //      {
  //          if (base.IsMultiLingualDocument())
  //              return false;
  //          if (strTableName == null)
  //              return false;
  //          //	Not Multi-Lingual Documents - only Doc Related
  //          if (strTableName.StartsWith("AD"))
  //              return false;
  //          return true;
  //      }

  //      /**
	 //* 	Is Auto Archive on
	 //*	@return true if auto archive
	 //*/
  //      public bool IsAutoArchive()
  //      {
  //          String aa = GetAutoArchive();
  //          return aa != null && !aa.Equals(AUTOARCHIVE_None);
  //      }	//	


        /// <summary>
        /// Get SMTP Host
        /// </summary>
        /// <returns>SMTP or loaclhost</returns>
        public new String GetSmtpHost()
        {
            //String s = super.getSmtpHost();
            String s = base.GetSmtpHost();
            if (s == null)
            {
                s = "localhost";
            }
            return s;
        }	//	getSMTPHost

        /**
	 *	Get SMTP Port
	 *	@return port (default 25)
	 */
        public new int GetSmtpPort()
        {
            int p = base.GetSmtpPort();
            if (p == 0)
                SetSmtpPort(25);
            return base.GetSmtpPort();
        }

        /// <summary>
        /// Get Primary Accounting Schema
        /// </summary>
        /// <returns>Acct Schema or null</returns>
        internal MAcctSchema GetAcctSchema()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetVAF_Client_ID(), Get_TrxName());
            if (_info != null)
            {
                int VAB_AccountBook_ID = _info.GetVAB_AccountBook1_ID();
                if (VAB_AccountBook_ID != 0)
                    return MAcctSchema.Get(GetCtx(), VAB_AccountBook_ID);
            }
            return null;
        }

        /// <summary>
        /// Get Primary Accounting Schema ID
        /// </summary>
        /// <returns></returns>
        public int GetAcctSchemaID()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetVAF_Client_ID(), Get_TrxName());
            if (_info != null)
            {
                int VAB_AccountBook_ID = _info.GetVAB_AccountBook1_ID();
                if (VAB_AccountBook_ID != 0)
                    return VAB_AccountBook_ID;
            }
            return 0;
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>clients</returns>
        //public static MClient[] GetAll(Ctx ctx)
        //{
        //    List<MClient> list = new List<MClient>();
        //    String sql = "SELECT * FROM VAF_Client";
        //    try
        //    {
        //        DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);

        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            MClient client = new MClient(ctx, dr, null);
        //            list.Add(client);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        s_log.Log(Level.SEVERE, sql, e);
        //    }

        //    MClient[] RetValue = new MClient[list.Count()];
        //    RetValue = list.ToArray();
        //    return RetValue;
        //}	//	getAll

        /// <summary>
        /// Get Default Accounting Currency
        /// </summary>
        /// <returns>currency or 0</returns>
        public int GetVAB_Currency_ID()
        {
            if (_info == null)
                GetInfo();
            if (_info != null)
                return _info.GetVAB_Currency_ID();
            return 0;
        }	//	getVAB_Currency_ID
       
        /// <summary>
        ///Get Client Info
        /// </summary>
        /// <returns>Client Info</returns>
        public MClientInfo GetInfo()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetVAF_Client_ID(), Get_Trx());
            return _info;
        }



        private VAdvantage.Login.Language _language = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public VAdvantage.Login.Language GetLanguage()
        //{
        //    if (_language == null)
        //    {
        //        _language = VAdvantage.Login.Language.GetLanguages(GetVAF_Language());
        //        _language = Env.VerifyLanguage(GetCtx(), _language);
        //    }
        //    return _language;
        //}	//	getLanguage

        //public void SetVAF_Language(String VAF_Language)
        //{
        //    _language = null;
        //    base.SetVAF_Language(VAF_Language);
        //}	//	setVAF_Language


        public new String GetVAF_Language()
        {
            String s = base.GetVAF_Language();
            if (s == null)
                return VAdvantage.Login.Language.GetBaseVAF_Language();
            return s;
        }	//	getVAF_Language


      

        public System.Globalization.CultureInfo GetLocale()
        {
            VAdvantage.Login.Language lang = GetLanguage();
            if (lang != null)
                return lang.GetCulture();
            return lang.GetCulture("en_US");
        }

        public bool SetupClientInfo(String language)
        {
            //	Create Trees
            String sql = null;
            if (Env.IsBaseLanguage(language, "VAF_CtrlRef_List"))	//	Get TreeTypes & Name
                sql = "SELECT Value, Name FROM VAF_CtrlRef_List WHERE VAF_Control_Ref_ID=120 AND IsActive='Y'";
            else
                sql = "SELECT l.Value, t.Name FROM VAF_CtrlRef_List l, VAF_CtrlRef_TL t "
                    + "WHERE l.VAF_Control_Ref_ID=120 AND l.VAF_CtrlRef_List_ID=t.VAF_CtrlRef_List_ID AND l.IsActive='Y' AND t.VAF_Language='" + language + "'";

            //  Tree IDs
            int VAF_TreeInfo_Org_ID = 0, VAF_TreeInfo_BPartner_ID = 0, VAF_TreeInfo_Project_ID = 0,
                VAF_TreeInfo_SalesRegion_ID = 0, VAF_TreeInfo_Product_ID = 0,
                VAF_TreeInfo_Campaign_ID = 0, VAF_TreeInfo_Activity_ID = 0;

            bool success = false;
            try
            {
                //IDataReader dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                DataSet dr = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                MTree tree = null;
                for (int i = 0; i <= dr.Tables[0].Rows.Count - 1; i++)
                {

                    try
                    {
                        String treeType = dr.Tables[0].Rows[i]["Value"].ToString();
                        if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Other)
                            || treeType.StartsWith("U"))
                            continue;
                        String name = GetName() + " " + dr.Tables[0].Rows[i]["Name"].ToString();
                        //
                        if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Organization))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_Org_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_BPartner))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_BPartner_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Project))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_Project_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_SalesRegion))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_SalesRegion_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Product))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_Product_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_ElementValue))
                        {
                            //commented by lakhwinder 
                            //do not crete Account Tree While tenant Creation
                            //tree = new MTree(this, name, treeType);
                            //success = tree.Save();
                            //m_VAF_TreeInfo_Account_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Campaign))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_Campaign_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Activity))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            VAF_TreeInfo_Activity_ID = tree.GetVAF_TreeInfo_ID();
                        }
                        else if (treeType.Equals(X_VAF_TreeInfo.TREETYPE_Menu))	//	No Menu
                            success = true;
                        else	//	PC (Product Category), BB (BOM)
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                        }
                        if (!success)
                        {
                            log.Log(VAdvantage.Logging.Level.SEVERE, "Tree NOT created: " + name);
                            break;
                        }
                    }
                    catch { }
                }

                dr.Dispose();
            }
            catch (Exception e1)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, "Trees", e1);
                success = false;
            }

            if (!success)
                return false;

            //	Create ClientInfo
            MClientInfo clientInfo = new MClientInfo(this,
                VAF_TreeInfo_Org_ID, VAF_TreeInfo_BPartner_ID, VAF_TreeInfo_Project_ID,
                VAF_TreeInfo_SalesRegion_ID, VAF_TreeInfo_Product_ID,
                VAF_TreeInfo_Campaign_ID, VAF_TreeInfo_Activity_ID, Get_TrxName());
            success = clientInfo.Save();
            return success;
        }	//	createTrees

        /** Client Info Setup Tree for Account	*/
        private int m_VAF_TreeInfo_Account_ID;

        public int GetSetup_VAF_TreeInfo_Account_ID()
        {
            return m_VAF_TreeInfo_Account_ID;
        }	//	getSetup_VAF_TreeInfo_Account_ID


        /**
     //* 	Save
     //*	@return true if saved
     //*/
        public override bool Save()
        {
            if (Get_ID() == 0 && !m_createNew)
                return SaveUpdate();
            // check costing calculation, if costing not calculate then not able to change record
            if (Get_ID() != 0 && Is_ValueChanged("IsCostImmediate"))
            {
                bool result = checkAllCostCalculated(Get_ID());
                if (!result)
                {
                    log.SaveError("Warning", Msg.GetMsg(GetCtx(), "CostNotCalculateForAllTransaction"));
                    return result;
                }
            }
            return base.Save();
        }

        // check how many records in system whose costing not calculated based on client
        public bool checkAllCostCalculated(int client_ID)
        {
            bool result = true;
            string sql = null;
            int count = 0;
            try
            {
                // check columnname exist in table or not
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT count(*) FROM vaf_column WHERE 
                    vaf_tableview_id = ( SELECT vaf_tableview_id FROM vaf_tableview WHERE tablename LIKE 'M_Inventory' ) AND columnname LIKE 'IsCostCalculated'", null, null)) > 0)
                {
                    // check module exist or not
                    count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VAMFG_' AND Isactive = 'Y' "));

                    // check how many records in system whose costing not calculated based on client
                    sql = @"SELECT SUM(record) FROM (
                            SELECT COUNT(*) AS record FROM m_Inventory WHERE VAF_Client_ID = " + client_ID + @" AND IsActive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL')  AND iscostcalculated = 'N')  OR (docstatus IN ('RE')  AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N'  AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM m_movement WHERE VAF_Client_ID  = " + client_ID + @" AND isactive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' 
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM VAB_Invoice WHERE VAF_Client_ID = " + client_ID + @" AND isactive = 'Y'
                            AND issotrx = 'N' AND isreturntrx = 'N'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM m_inout  WHERE VAF_Client_ID = " + client_ID + @" AND isactive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))";
                    if (count > 0)
                    {
                        sql += @" UNION 
                                 SELECT COUNT(*) AS record FROM VAMFG_M_WrkOdrTransaction WHERE VAF_Client_ID = " + client_ID + @" 
                                 AND VAMFG_WorkOrderTxnType IN ('CI', 'CR') AND  isactive = 'Y' AND
                                 ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' 
                                 AND ISREVERSEDCOSTCALCULATED= 'N' AND VAMFG_description like '%{->%'))";
                    }
                    sql += ") t";
                    count = 0;
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count == 0)
                        result = true;
                    else
                        result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /**
     * 	String Representation
     *	@return info
     */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MClient[")
                .Append(Get_ID()).Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }	//	toString


    }
}
