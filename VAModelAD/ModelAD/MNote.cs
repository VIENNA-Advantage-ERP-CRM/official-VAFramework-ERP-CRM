/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MNote
 * Purpose        : 
 * Class Used     : X_AD_Note
 * Chronological    Development
 * Raghunandan       27-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MNote : X_AD_Note
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Note_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, int AD_Note_ID, Trx trxName)
            : base(ctx, AD_Note_ID, trxName)
        {
            if (AD_Note_ID == 0)
            {
                SetProcessed(false);
                SetProcessing(false);
            }
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">set</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }

        /// <summary>
        ///New Mandatory Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Msg_Lable_ID">message</param>
        /// <param name="AD_User_ID">targeted user</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, int VAF_Msg_Lable_ID, int AD_User_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetVAF_Msg_Lable_ID(VAF_Msg_Lable_ID);
            SetAD_User_ID(AD_User_ID);
        }

        /// <summary>
        /// New Mandatory Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Msg_LableValue">message</param>
        /// <param name="AD_User_ID">targeted user</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, string VAF_Msg_LableValue, int AD_User_ID, Trx trxName)
            : this(ctx, MMessage.GetVAF_Msg_Lable_ID(ctx, VAF_Msg_LableValue), AD_User_ID, trxName)
        {
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }

        /// <summary>
        /// Create Note
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Msg_Lable_ID">message</param>
        /// <param name="AD_User_ID">user</param>
        /// <param name="VAF_TableView_ID">table</param>
        /// <param name="Record_ID">record</param>
        /// <param name="Reference">reference</param>
        /// <param name="TextMsg">text message</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, int VAF_Msg_Lable_ID, int AD_User_ID,
            int VAF_TableView_ID, int Record_ID, String Reference, String TextMsg, Trx trxName)
            : this(ctx, VAF_Msg_Lable_ID, AD_User_ID, trxName)
        {
            SetRecord(VAF_TableView_ID, Record_ID);
            SetReference(Reference);
            SetTextMsg(TextMsg);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }


        /// <summary>
        /// Create Note (Added by Jagmohan Bhatt)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAF_Msg_Lable_ID"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="VAF_Client_ID"></param>
        /// <param name="VAF_TableView_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="Reference"></param>
        /// <param name="trxName"></param>
        public MNote(Ctx ctx, int VAF_Msg_Lable_ID, int AD_User_ID, int VAF_Client_ID, int VAF_Org_ID, int VAF_TableView_ID, int Record_ID, String Reference, Trx trxName)
            : this(ctx,VAF_Msg_Lable_ID, AD_User_ID, trxName)
        {
            SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            SetRecord(VAF_TableView_ID, Record_ID);
            SetReference(Reference);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Msg_LableValue">message</param>
        /// <param name="AD_User_ID">targeteduser</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, string VAF_Msg_LableValue, int AD_User_ID,
            int VAF_Client_ID, int VAF_Org_ID, Trx trxName)
            : this(ctx, MMessage.GetVAF_Msg_Lable_ID(ctx, VAF_Msg_LableValue), AD_User_ID, trxName)
        {
            SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetVAF_Org_ID(0);
        }

        /// <summary>
        ///Set Record.
        ///(Ss Button and defaults to String)
        /// </summary>
        /// <param name="VAF_Msg_Lable">VAF_Msg_Lable</param>
        public void SetVAF_Msg_Lable_ID(string VAF_Msg_Lable)
        {
            int VAF_Msg_Lable_ID = CoreLibrary.DataBase.DB.GetSQLValue(null, "SELECT VAF_Msg_Lable_ID FROM VAF_Msg_Lable WHERE Value=" + VAF_Msg_Lable);
            if (VAF_Msg_Lable_ID != -1)
            {
                base.SetVAF_Msg_Lable_ID(VAF_Msg_Lable_ID);
            }
            else
            {
                base.SetVAF_Msg_Lable_ID(240); //	Error
                log.Log(Level.SEVERE, "setVAF_Msg_Lable_ID - ID not found for '" + VAF_Msg_Lable + "'");
            }
        }

        /// <summary>
        ///	Set VAF_Msg_Lable_ID.
        ///	Looks up No Message Found if 0
        /// </summary>
        /// <param name="VAF_Msg_Lable_ID">id</param>
        public new void SetVAF_Msg_Lable_ID(int VAF_Msg_Lable_ID)
        {
            if (VAF_Msg_Lable_ID == 0)
            {
                base.SetVAF_Msg_Lable_ID(MMessage.GetVAF_Msg_Lable_ID(GetCtx(), "NoMessageFound"));
            }
            else
            {
                base.SetVAF_Msg_Lable_ID(VAF_Msg_Lable_ID);
            }
        }

        /// <summary>
        ///	Get Message
        /// </summary>
        /// <returns>message</returns>
        public string GetMessage()
        {
            int VAF_Msg_Lable_ID = GetVAF_Msg_Lable_ID();
            MMessage msg = MMessage.Get(GetCtx(), VAF_Msg_Lable_ID);
            return msg.GetMsgText();
        }

        /// <summary>
        ///Set Client Org
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        public new void SetClientOrg(int VAF_Client_ID, int VAF_Org_ID)
        {
            base.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
        }

        /// <summary>
        ///	Set Record
        /// </summary>
        /// <param name="VAF_TableView_ID">table</param>
        /// <param name="Record_ID">record</param>
        public void SetRecord(int VAF_TableView_ID, int Record_ID)
        {
            SetVAF_TableView_ID(VAF_TableView_ID);
            SetRecord_ID(Record_ID);
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MNote[")
                .Append(Get_ID()).Append(",VAF_Msg_Lable_ID=").Append(GetVAF_Msg_Lable_ID())
                .Append(",").Append(GetReference())
                .Append(",Processed=").Append(IsProcessed())
                .Append("]");
            return sb.ToString();
        }
    }
}
