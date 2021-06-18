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
using VAdvantage.PushNotif;

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
            SetAD_Org_ID(0);
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
            SetAD_Org_ID(0);
        }

        /// <summary>
        ///New Mandatory Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Message_ID">message</param>
        /// <param name="AD_User_ID">targeted user</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, int AD_Message_ID, int AD_User_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetAD_Message_ID(AD_Message_ID);
            SetAD_User_ID(AD_User_ID);
        }

        /// <summary>
        /// New Mandatory Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_MessageValue">message</param>
        /// <param name="AD_User_ID">targeted user</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, string AD_MessageValue, int AD_User_ID, Trx trxName)
            : this(ctx, MMessage.GetAD_Message_ID(ctx, AD_MessageValue), AD_User_ID, trxName)
        {
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetAD_Org_ID(0);
        }

        /// <summary>
        /// Create Note
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Message_ID">message</param>
        /// <param name="AD_User_ID">user</param>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="Record_ID">record</param>
        /// <param name="Reference">reference</param>
        /// <param name="TextMsg">text message</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, int AD_Message_ID, int AD_User_ID,
            int AD_Table_ID, int Record_ID, String Reference, String TextMsg, Trx trxName)
            : this(ctx, AD_Message_ID, AD_User_ID, trxName)
        {
            SetRecord(AD_Table_ID, Record_ID);
            SetReference(Reference);
            SetTextMsg(TextMsg);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetAD_Org_ID(0);
        }


        /// <summary>
        /// Create Note (Added by Jagmohan Bhatt)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Message_ID"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="Reference"></param>
        /// <param name="trxName"></param>
        public MNote(Ctx ctx, int AD_Message_ID, int AD_User_ID, int AD_Client_ID, int AD_Org_ID, int AD_Table_ID, int Record_ID, String Reference, Trx trxName)
            : this(ctx, AD_Message_ID, AD_User_ID, trxName)
        {
            SetClientOrg(AD_Client_ID, AD_Org_ID);
            SetRecord(AD_Table_ID, Record_ID);
            SetReference(Reference);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetAD_Org_ID(0);
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_MessageValue">message</param>
        /// <param name="AD_User_ID">targeteduser</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="trxName">transaction</param>
        public MNote(Ctx ctx, string AD_MessageValue, int AD_User_ID,
            int AD_Client_ID, int AD_Org_ID, Trx trxName)
            : this(ctx, MMessage.GetAD_Message_ID(ctx, AD_MessageValue), AD_User_ID, trxName)
        {
            SetClientOrg(AD_Client_ID, AD_Org_ID);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            SetAD_Org_ID(0);
        }

        /// <summary>
        ///Set Record.
        ///(Ss Button and defaults to String)
        /// </summary>
        /// <param name="AD_Message">AD_Message</param>
        public void SetAD_Message_ID(string AD_Message)
        {
            int AD_Message_ID = DataBase.DB.GetSQLValue(null, "SELECT AD_Message_ID FROM AD_Message WHERE Value=" + AD_Message);
            if (AD_Message_ID != -1)
            {
                base.SetAD_Message_ID(AD_Message_ID);
            }
            else
            {
                base.SetAD_Message_ID(240); //	Error
                log.Log(Level.SEVERE, "setAD_Message_ID - ID not found for '" + AD_Message + "'");
            }
        }

        /// <summary>
        ///	Set AD_Message_ID.
        ///	Looks up No Message Found if 0
        /// </summary>
        /// <param name="AD_Message_ID">id</param>
        public new void SetAD_Message_ID(int AD_Message_ID)
        {
            if (AD_Message_ID == 0)
            {
                base.SetAD_Message_ID(MMessage.GetAD_Message_ID(GetCtx(), "NoMessageFound"));
            }
            else
            {
                base.SetAD_Message_ID(AD_Message_ID);
            }
        }

        /// <summary>
        ///	Get Message
        /// </summary>
        /// <returns>message</returns>
        public string GetMessage()
        {
            int AD_Message_ID = GetAD_Message_ID();
            MMessage msg = MMessage.Get(GetCtx(), AD_Message_ID);
            return msg.GetMsgText();
        }

        /// <summary>
        ///Set Client Org
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        public new void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        ///	Set Record
        /// </summary>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="Record_ID">record</param>
        public void SetRecord(int AD_Table_ID, int Record_ID)
        {
            SetAD_Table_ID(AD_Table_ID);
            SetRecord_ID(Record_ID);
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MNote[")
                .Append(Get_ID()).Append(",AD_Message_ID=").Append(GetAD_Message_ID())
                .Append(",").Append(GetReference())
                .Append(",Processed=").Append(IsProcessed())
                .Append("]");
            return sb.ToString();
        }

        /* 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true if can be saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            if (newRecord)
            {
                // VIS264 - Send push notification

                DateTime now = DateTime.Now;
                DateTime localTime = GetCreated().ToLocalTime();

                string dayName = null;

                if (now.ToShortDateString() == localTime.ToShortDateString())
                {
                    dayName = " (" + Msg.GetMsg(GetCtx(), "Today") + ")";
                }
                else if(now.AddDays(1).ToShortDateString() == localTime.ToShortDateString())
                {
                    dayName = " (" + Msg.GetMsg(GetCtx(), "Tomorrow") + ")";
                }
                else if (now.AddDays(-1).ToShortDateString() == localTime.ToShortDateString())
                {
                    dayName = " (" + Msg.GetMsg(GetCtx(), "Yesterday") + ")"; ;
                }

                string msgBody = Msg.GetMsg(GetCtx(), "Received") + " " + localTime + dayName;

                PushNotification.SendNotificationToUser(GetAD_User_ID(), GetAD_Window_ID(), GetRecord_ID(), Msg.GetMsg(GetCtx(), "Notice"), msgBody , "N");
            }

            return true;
        }
    }
}
