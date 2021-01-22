/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_UserPref_Info
 * Chronological Development
 * 
 * Veena Pandey     15-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// User Preference Model
    /// </summary>
    public class MUserPreference : X_VAF_UserPref_Info
    {
        //	Logger						
        private static VLogger _log = VLogger.GetVLogger(typeof(MUserPreference).FullName);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_UserPref_Info_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MUserPreference(Ctx ctx, int VAF_UserPref_Info_ID, Trx trxName)
            : base(ctx, VAF_UserPref_Info_ID, trxName)
        {
            if (VAF_UserPref_Info_ID == 0)
            {
                SetIsAutoCommit(true);
                SetIsShowAcct(false);
                SetIsShowAdvanced(false);
                SetIsShowTrl(false);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MUserPreference(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// 	Get User Preference
        /// </summary>
        /// <param name="user">user </param>
        /// <param name="createNew">create new if not found</param>
        /// <returns>user preference</returns>
        public static MUserPreference GetOfUser(X_VAF_UserContact user, bool createNew)
        {
            MUserPreference retValue = null;
            String sql = "SELECT * FROM VAF_UserPref_Info WHERE VAF_UserContact_ID='" + user.GetVAF_UserContact_ID() + "'";
            try
            {
                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql);
                foreach (DataRow rs in ds.Tables[0].Rows)
                {
                    retValue = new MUserPreference(user.GetCtx(), rs, null);
                }
                ds.Dispose();
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            if (retValue == null && createNew)
            {
                retValue = new MUserPreference(user.GetCtx(), 0, null);
                retValue.SetClientOrg(user);
                retValue.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());
                retValue.Save();
            }
            return retValue;
        }	//	getOfUser


        public static MUserPreference GetOfUser(PO user, bool createNew)
        {
            MUserPreference retValue = null;
            int VAF_UserContact_ID = (int)user.Get_Value("VAF_UserContact_ID");

            String sql = "SELECT * FROM VAF_UserPref_Info WHERE VAF_UserContact_ID='" + VAF_UserContact_ID + "'";
            try
            {
                DataSet ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql);
                foreach (DataRow rs in ds.Tables[0].Rows)
                {
                    retValue = new MUserPreference(user.GetCtx(), rs, null);
                }
                ds.Dispose();
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            if (retValue == null && createNew)
            {
                retValue = new MUserPreference(user.GetCtx(), 0, null);
                retValue.SetClientOrg(user);
                retValue.SetVAF_UserContact_ID(VAF_UserContact_ID);
                retValue.Save();
            }
            return retValue;
        }	//	getOfUser


        /// <summary>
        /// Set User
        /// </summary>
        /// <param name="VAF_UserContact_ID">user id</param>
        public new void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID == 0)
                Set_ValueNoCheck("VAF_UserContact_ID", 0);
            else
                base.SetVAF_UserContact_ID(VAF_UserContact_ID);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MUserPreference[")
                .Append(Get_ID())
                .Append(",VAF_UserContact_ID=").Append(GetVAF_UserContact_ID())
                .Append(",AutoCommit=").Append(IsAutoCommit())
                .Append(",ShowAcct=").Append(IsShowAcct())
                .Append(",ShowAdv=").Append(IsShowAdvanced())
                .Append(",ShowTrl=").Append(IsShowTrl());
            if (GetPrinterName() != null)
                sb.Append(",PrinterName=").Append(GetPrinterName());
            if (GetUITheme() != null)
                sb.Append(",UITheme=").Append(GetUITheme());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
