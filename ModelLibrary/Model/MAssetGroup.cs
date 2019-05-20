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
    }
}
