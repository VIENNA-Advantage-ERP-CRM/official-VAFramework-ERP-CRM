

using System;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MActionLog : X_AD_ActionLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_ActionLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MActionLog(Ctx ctx, int AD_ActionLog_ID, Trx trxName)
            : base(ctx, AD_ActionLog_ID, trxName)
        {
            //super(ctx, VAF_DBQueryLog_ID, trxName);
            if (AD_ActionLog_ID == 0)
            {
                int VAF_Role_ID = ctx.GetVAF_Role_ID();
                SetVAF_Role_ID(VAF_Role_ID);
            }
        }

        /// <summary>
        /// Action log
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Session_ID">session id</param>
        /// <param name="VAF_Client_ID"> client id</param>
        /// <param name="VAF_Org_ID">org id</param>
        /// <param name="action">menu action </param>
        /// <param name="actionType">action type for logging</param>
        /// <param name="actionOrigin">origin of action</param>
        /// <param name="desc">additional info</param>
        /// <param name="VAF_TableView_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        public MActionLog(Ctx ctx, int AD_Session_ID,
    int VAF_Client_ID, int VAF_Org_ID,
    String action, string actionType, String actionOrigin, string desc, int VAF_TableView_ID, int Record_ID)
        : this(ctx, 0, null)
        {
            //	out of trx
            SetAD_Session_ID(AD_Session_ID);
            SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            SetAction(action);
            SetActionType(actionType);
            SetActionOrigin(actionOrigin);
            SetDescription(desc);
            if (VAF_TableView_ID > 0)
                SetVAF_TableView_ID(VAF_TableView_ID);
            if (Record_ID > 0)
                SetRecord_ID(Record_ID);
            SetVAF_Role_ID(ctx.GetVAF_Role_ID());
        }
    }
}
