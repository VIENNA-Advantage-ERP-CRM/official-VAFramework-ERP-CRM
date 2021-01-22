/********************************************************
 * Module Name    : 
 * Purpose        : Access Log Model
 * Class Used     : X_VAF_RightsLog
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
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{

    
    /// <summary>
    /// Access Log Model
    /// </summary>
    public class MVAFRightsLog : X_VAF_RightsLog
    {

        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFRightsLog).FullName);


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLineConfirm_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFRightsLog(Context ctx, int M_VAF_RightsLog_ID, Trx trxName)
            : base(ctx, M_VAF_RightsLog_ID, trxName)
	    {
	    }

	    /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MVAFRightsLog(Context ctx, DataRow dr, Trx trxName)
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
        public MVAFRightsLog(Context ctx, String email, String remoteHost, String remoteAddr,
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
        /// <param name="VAF_TableView_ID">table</param>
        /// <param name="VAF_Column_ID">column</param>
        /// <param name="Record_ID">record</param>
        /// <param name="trxName">transaction</param>
        public MVAFRightsLog(Context ctx, String email, int VAF_TableView_ID, int VAF_Column_ID, 
            int Record_ID, Trx trxName)
            : this(new Context(ctx), 0, trxName)
        {
            SetVAF_TableView_ID(VAF_TableView_ID);
            SetVAF_Column_ID(VAF_Column_ID);
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
            int VAF_UserContact_ID = GetVAF_UserContact_ID(email, GetVAF_Client_ID());
            Set_ValueNoCheck("CreatedBy", VAF_UserContact_ID);
            SetUpdatedBy(VAF_UserContact_ID);
            GetCtx().SetContext("##VAF_UserContact_ID", VAF_UserContact_ID.ToString());
            SetVAF_UserContact_ID(VAF_UserContact_ID);
        }


        private int GetVAF_UserContact_ID(String email, int VAF_Client_ID)
        {
            int VAF_UserContact_ID = 0;
            IDataReader idr = null;
            String sql = "SELECT VAF_UserContact_ID FROM VAF_UserContact "
                + "WHERE UPPER(EMail)=@param1"
                + " AND VAF_Client_ID=@param2";
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", email.ToUpper());
                param[1] = new SqlParameter("@param2", VAF_Client_ID);
                idr = CoreLibrary.DataBase.DB.ExecuteReader(sql, param, null);
                while (idr.Read())
                {
                    VAF_UserContact_ID = Utility.Util.GetValueOfInt(idr[0].ToString());//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, email, e);
            }

            return VAF_UserContact_ID;
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
