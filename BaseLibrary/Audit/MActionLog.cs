

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
            //super(ctx, AD_QueryLog_ID, trxName);
            if (AD_ActionLog_ID == 0)
            {
                int AD_Role_ID = ctx.GetAD_Role_ID();
                SetAD_Role_ID(AD_Role_ID);
            }
        }

        /// <summary>
        /// Action log
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Session_ID">session id</param>
        /// <param name="AD_Client_ID"> client id</param>
        /// <param name="AD_Org_ID">org id</param>
        /// <param name="action">menu action </param>
        /// <param name="actionType">action type for logging</param>
        /// <param name="actionOrigin">origin of action</param>
        /// <param name="desc">additional info</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        public MActionLog(Ctx ctx, int AD_Session_ID,
    int AD_Client_ID, int AD_Org_ID,
    String action, string actionType, String actionOrigin, string desc, int AD_Table_ID, int Record_ID)
        : this(ctx, 0, null)
        {
            //	out of trx
            SetAD_Session_ID(AD_Session_ID);
            SetClientOrg(AD_Client_ID, AD_Org_ID);
            SetAction(action);
            SetActionType(actionType);
            SetActionOrigin(actionOrigin);
            SetDescription(desc);
            if (AD_Table_ID > 0)
                SetAD_Table_ID(AD_Table_ID);
            if (Record_ID > 0)
                SetRecord_ID(Record_ID);
            SetAD_Role_ID(ctx.GetAD_Role_ID());
        }
    }
}
