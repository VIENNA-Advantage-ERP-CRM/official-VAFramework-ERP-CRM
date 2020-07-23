/********************************************************
 * Module Name    : 
 * Purpose        : Access Log Model
 * Class Used     : X_AD_AccessLog
 * Chronological Development
 * Veena         28-Oct-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Access Log Model
    /// </summary>
    public class MAccessLog : X_AD_AccessLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLineConfirm_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAccessLog(Context ctx, int M_AccessLog_ID, Trx trxName)
            : base(ctx, M_AccessLog_ID, trxName)
	    {
	    }

	    /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MAccessLog(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="email">email</param>
        /// <param name="remoteHost">host</param>
        /// <param name="remoteAddr">addr</param>
        /// <param name="textMsg">text message</param>
        /// <param name="trxName">transaction</param>
        public MAccessLog(Context ctx, String email, String remoteHost, String remoteAddr,
            String textMsg, Trx trxName)
            : this(new Context(ctx), 0, trxName)
        {
            SetRemote_Addr(remoteAddr);
            SetRemote_Host(remoteHost);
            SetTextMsg(textMsg);
            SetCreatedBy(email);
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="email">email</param>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="Record_ID">record</param>
        /// <param name="trxName">transaction</param>
        public MAccessLog(Context ctx, String email, int AD_Table_ID, int AD_Column_ID, 
            int Record_ID, Trx trxName)
            : this(new Context(ctx), 0, trxName)
        {
            SetAD_Table_ID(AD_Table_ID);
            SetAD_Column_ID(AD_Column_ID);
            SetRecord_ID(Record_ID);
            SetCreatedBy(email);
        }

        /// <summary>
        /// Set Created/Updated By
        /// </summary>
        /// <param name="email">mail address</param>
        private void SetCreatedBy(String email)
        {
            if (email == null || email.Length == 0)
                return;
            int AD_User_ID = MUser.GetAD_User_ID(email, GetAD_Client_ID());
            Set_ValueNoCheck("CreatedBy", AD_User_ID);
            SetUpdatedBy(AD_User_ID);
            GetCtx().SetContext("##AD_User_ID", AD_User_ID.ToString());
            SetAD_User_ID(AD_User_ID);
        }

        /// <summary>
        /// Add to Reply
        /// </summary>
        /// <param name="reply">reply</param>
        public void AddReply(String reply)
        {
            if (reply == null || reply.Length == 0)
                return;
            String old = GetReply();
            if (old == null || old.Length == 0)
                SetReply(reply);
            else
                SetReply(old + " - " + reply);
        }

    }
}
