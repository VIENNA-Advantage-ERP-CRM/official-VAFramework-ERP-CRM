/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssetGroup
 * Purpose        : Multi-purpose SO,PO etc
 * Class Used     : X_A_Asset_Group
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAssetGroup : X_A_Asset_Group
    {
        //	Categopry Cache				
        //private static CCache s_cache = new CCache("A_Asset_Group", 10);
        private static CCache<int, MAssetGroup> s_cache = new CCache<int, MAssetGroup>("A_Asset_Group", 10);

        /* 	Get from Cache
        *	@param ctx context
        *	@param A_Asset_Group_ID id
        *	@return category
        */
        public static MAssetGroup Get(Ctx ctx, int A_Asset_Group_ID)
        {
            int key = A_Asset_Group_ID;
            MAssetGroup pc = (MAssetGroup)s_cache[key];
            if (pc == null)
                pc = new MAssetGroup(ctx, A_Asset_Group_ID, null);
            return pc;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param A_Asset_Group_ID id
         *	@param trxName trx
         */
        public MAssetGroup(Ctx ctx, int A_Asset_Group_ID, Trx trxName)
            : base(ctx, A_Asset_Group_ID, trxName)
        {

            if (A_Asset_Group_ID == 0)
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
        public MAssetGroup(Ctx ctx, DataRow rs, Trx trxName)
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
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT * FROM A_Asset_Group WHERE Name ='" + GetName() + "' AND IsActive ='Y' AND AD_Client_ID=" + GetAD_Client_ID(), null, Get_TrxName())) > 0)
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
                _sql.Append("Select L.Value From Ad_Ref_List L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where r.name='FRPT_RelatedTo' and l.name='Asset'");
                var relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO assetGroupAcct = null;
                _sql.Clear();
                _sql.Append("select C_AcctSchema_ID from C_AcctSchema where IsActive = 'Y' AND AD_CLIENT_ID=" + GetAD_Client_ID());
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["C_AcctSchema_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,C_Validcombination_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND AD_CLIENT_ID=" + GetAD_Client_ID() + "AND C_Acctschema_Id=" + _AcctSchema_ID);
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
                                    _sql.Append(@"Select count(*) From A_Asset_Group Bp
                                                       Left Join FRPT_Asset_Group_Acct  ca On Bp.A_Asset_Group_ID=ca.A_Asset_Group_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                                   + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + GetAD_Client_ID() +
                                                   " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) +
                                                   " AND Bp.A_Asset_Group_ID = " + GetA_Asset_Group_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        assetGroupAcct = MTable.GetPO(GetCtx(), "FRPT_Asset_Group_Acct", 0, null);
                                        assetGroupAcct.Set_ValueNoCheck("AD_Org_ID", 0);
                                        assetGroupAcct.Set_ValueNoCheck("A_Asset_Group_ID", Util.GetValueOfInt(GetA_Asset_Group_ID()));
                                        assetGroupAcct.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        assetGroupAcct.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                        assetGroupAcct.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);
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
