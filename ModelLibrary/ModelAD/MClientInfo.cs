/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MClientInfo
 * Purpose        : Client info using VAF_ClientDetail table
 * Class Used     : X_VAF_ClientDetail
 * Chronological    Development
 * Raghunandan      28-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MClientInfo : X_VAF_ClientDetail
    {
        //Account Schema				
        private MAcctSchema _acctSchema = null;
        //New Record
        private bool _createNew = false;
        /**	Cache						*/
        private static CCache<int, MClientInfo> s_cache = new CCache<int, MClientInfo>("VAF_ClientDetail", 2);
        //private static Dictionary<int, MClientInfo> s_cache = new Dictionary<int, MClientInfo>();

        /// <summary>
        /// Get Client Info
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">id</param>
        /// <returns>Client Info</returns>
        public static MClientInfo Get(Ctx ctx, int VAF_Client_ID)
        {
            return Get(ctx, VAF_Client_ID, null);
        }	//	get

        /// <summary>
        /// Get Client Info
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">id</param>
        /// <param name="trxName">optional trx</param>
        /// <returns>Client Info</returns>
        public static MClientInfo Get(Ctx ctx, int VAF_Client_ID, Trx trxName)
        {
            int key = VAF_Client_ID;
            MClientInfo info = (MClientInfo)s_cache[key];
            if (info != null)
            {
                return info;
            }
            //
            String sql = "SELECT * FROM VAF_ClientDetail WHERE VAF_Client_ID=" + VAF_Client_ID;
            DataSet ds = null;
            try
            {
                VConnection vcon = VConnection.Get();
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    info = new MClientInfo(ctx, dr, null);
                    if (trxName == null)
                    {
                        s_cache.Add(key, info);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            return info;
        }	

        /// <summary>
        ///Get optionally cached client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>client</returns>
        public static MClientInfo Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetVAF_Client_ID(), null);
        }	
        
        //	Logger						
        private static VLogger _log = VLogger.GetVLogger(typeof(MClientInfo).FullName);


        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">VAF_Client_ID ignored</param>
        /// <param name="trxName">transaction</param>
        public MClientInfo(Ctx ctx, int VAF_Client_ID, Trx trxName):base(ctx,VAF_Client_ID,trxName)
        {
            //super(ctx, VAF_Client_ID, trxName);
        }	//	MClientInfo

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MClientInfo(Ctx ctx, DataRow rs, Trx trxName):base(ctx,rs,trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MClientInfo

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="VAF_TreeInfo_Org_ID">org tree</param>
        /// <param name="VAF_TreeInfo_BPartner_ID">bp tree</param>
        /// <param name="VAF_TreeInfo_Project_ID">project tree</param>
        /// <param name="VAF_TreeInfo_SalesRegion_ID">sr tree</param>
        /// <param name="VAF_TreeInfo_Product_ID">product tree</param>
        /// <param name="VAF_TreeInfo_Campaign_ID">campaign tree</param>
        /// <param name="VAF_TreeInfo_Activity_ID">activity tree</param>
        /// <param name="trxName">transaction</param>
        public MClientInfo(MClient client, int VAF_TreeInfo_Org_ID, int VAF_TreeInfo_BPartner_ID,
            int VAF_TreeInfo_Project_ID, int VAF_TreeInfo_SalesRegion_ID, int VAF_TreeInfo_Product_ID,
            int VAF_TreeInfo_Campaign_ID, int VAF_TreeInfo_Activity_ID, Trx trxName):base(client.GetCtx(),0,trxName)
        {
            //super(client.getCtx(), 0, trxName);
            SetVAF_Client_ID(client.GetVAF_Client_ID());	//	to make sure
            SetVAF_Org_ID(0);
            SetIsDiscountLineAmt(false);
            //
            SetVAF_TreeInfo_Menu_ID(10);		//	HARDCODED
            //
            SetVAF_TreeInfo_Org_ID(VAF_TreeInfo_Org_ID);
            SetVAF_TreeInfo_BPartner_ID(VAF_TreeInfo_BPartner_ID);
            SetVAF_TreeInfo_Project_ID(VAF_TreeInfo_Project_ID);
            SetVAF_TreeInfo_SalesRegion_ID(VAF_TreeInfo_SalesRegion_ID);
            SetVAF_TreeInfo_Product_ID(VAF_TreeInfo_Product_ID);
            SetVAF_TreeInfo_Campaign_ID(VAF_TreeInfo_Campaign_ID);
            SetVAF_TreeInfo_Activity_ID(VAF_TreeInfo_Activity_ID);
            //
            SetMatchRequirementI(MATCHREQUIREMENTI_None);
            SetMatchRequirementR(MATCHREQUIREMENTR_None);
            _createNew = true;
        }	


        

        /// <summary>
        ///Get primary Acct Schema
        /// </summary>
        /// <returns>acct schema</returns>
        public MAcctSchema GetMAcctSchema1()
        {
            if (_acctSchema == null && GetC_AcctSchema1_ID() != 0)
                _acctSchema = new MAcctSchema(GetCtx(), GetC_AcctSchema1_ID(), null);
            return _acctSchema;
        }	

        /// <summary>
        ///Get Default Accounting Currency
        /// </summary>
        /// <returns>currency or 0</returns>
        public int GetC_Currency_ID()
        {
            if (_acctSchema == null)
                GetMAcctSchema1();
            if (_acctSchema != null)
                return _acctSchema.GetC_Currency_ID();
            return 0;
        }	//	getC_Currency_ID

        /// <summary>
        ///Get Match Requirement I
        /// </summary>
        /// <returns>invoice Match Req</returns>
        public new string GetMatchRequirementI()
        {
            String s = base.GetMatchRequirementI();
            if (s == null)
            {
                SetMatchRequirementI(MATCHREQUIREMENTI_None);
                return MATCHREQUIREMENTI_None;
            }
            return s;
        }	//	getMatchRequirementI

        /// <summary>
        ///Get Match Requirement R
        /// </summary>
        /// <returns>receipt matcg Req</returns>
        public new  String GetMatchRequirementR()
        {
            String s = base.GetMatchRequirementR();
            if (s == null)
            {
                SetMatchRequirementR(MATCHREQUIREMENTR_None);
                return MATCHREQUIREMENTR_None;
            }
            return s;
        }	//	getMatchRequirementR


        /// <summary>
        ///Overwrite Save
        /// </summary>
        /// <returns>true if saved</returns>
        public override bool Save()
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            GetMatchRequirementI();
            GetMatchRequirementR();
            if (_createNew)
                return base.Save();
            return SaveUpdate();
        }	//	save

    }
}
