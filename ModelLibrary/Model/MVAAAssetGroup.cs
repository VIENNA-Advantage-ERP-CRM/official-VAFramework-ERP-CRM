/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssetGroup
 * Purpose        : Multi-purpose SO,PO etc
 * Class Used     : X_VAA_AssetGroup
 * Chronological    Development
 * Raghunandan     11-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAAAssetGroup : X_VAA_AssetGroup
    {
        //	Categopry Cache				
        //private static CCache s_cache = new CCache("VAA_AssetGroup", 10);
        private static CCache<int, MVAAAssetGroup> s_cache = new CCache<int, MVAAAssetGroup>("VAA_AssetGroup", 10);

        /* 	Get from Cache
        *	@param ctx context
        *	@param VAA_AssetGroup_ID id
        *	@return category
        */
        public static MVAAAssetGroup Get(Ctx ctx, int VAA_AssetGroup_ID)
        {
            int key = VAA_AssetGroup_ID;
            MVAAAssetGroup pc = (MVAAAssetGroup)s_cache[key];
            if (pc == null)
                pc = new MVAAAssetGroup(ctx, VAA_AssetGroup_ID, null);
            return pc;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAA_AssetGroup_ID id
         *	@param trxName trx
         */
        public MVAAAssetGroup(Ctx ctx, int VAA_AssetGroup_ID, Trx trxName)
            : base(ctx, VAA_AssetGroup_ID, trxName)
        {

            if (VAA_AssetGroup_ID == 0)
            {
                //	setName (null);
                SetIsDepreciated(false);
                SetIsOneAssetPerUOM(false);
                SetIsOwned(false);
                SetIsCreateAsActive(true);
                SetIsTrackIssues(false);
                SetSupportLevel(SUPPORTLEVEL_Unsupported);
            }
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MVAAAssetGroup(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Get SupportLevel
         *	@return support level or Unsupported
         */
        public new String GetSupportLevel()
        {
            String ss = base.GetSupportLevel();
            if (ss == null)
                ss = SUPPORTLEVEL_Unsupported;
            return ss;
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT * FROM VAA_AssetGroup WHERE Name ='" + GetName() + "' AND IsActive ='Y' AND VAF_Client_ID=" + GetVAF_Client_ID(), null, Get_TrxName())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAFAM_AstGrpAlreadyExist"));
                    return false;
                }
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            // create default Account
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Asset_Group_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Asset_Group_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Asset'");
                var relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO assetGroupAcct = null;
                _sql.Clear();
                _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where IsActive = 'Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID());
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID() + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(_sql.ToString(), null, Get_Trx());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (!_relatedTo.Equals("") && _relatedTo.Equals(relatedtoProduct))
                                {
                                    _sql.Clear();
                                    _sql.Append(@"Select count(*) From VAA_AssetGroup Bp
                                                       Left Join FRPT_Asset_Group_Acct  ca On Bp.VAA_AssetGroup_ID=ca.VAA_AssetGroup_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                                   + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + GetVAF_Client_ID() +
                                                   " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) +
                                                   " AND Bp.VAA_AssetGroup_ID = " + GetVAA_AssetGroup_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        assetGroupAcct = MVAFTableView.GetPO(GetCtx(), "FRPT_Asset_Group_Acct", 0, null);
                                        assetGroupAcct.Set_ValueNoCheck("VAF_Org_ID", 0);
                                        assetGroupAcct.Set_ValueNoCheck("VAA_AssetGroup_ID", Util.GetValueOfInt(GetVAA_AssetGroup_ID()));
                                        assetGroupAcct.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        assetGroupAcct.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                        assetGroupAcct.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
                                        if (!assetGroupAcct.Save())
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            log.Log(Level.SEVERE, "Could Not create FRPT_Asset_Groip_Acct. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
