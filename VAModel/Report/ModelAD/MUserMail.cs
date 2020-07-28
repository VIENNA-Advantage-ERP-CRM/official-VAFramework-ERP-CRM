/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MUserMail
 * Purpose        : User Mail Model
 * Class Used     : X_AD_UserMail
 * Chronological    Development
 * Raghunandan      11-Nov-2009
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
using VAdvantage.Logging;
using VAdvantage.Print;
using System.IO;

namespace VAdvantage.Model
{
    public class MUserMail : X_AD_UserMail
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_UserMail_ID">id</param>
        /// <param name="trxName">trx</param>
        public MUserMail(Ctx ctx, int AD_UserMail_ID, Trx trxName)
            : base(ctx, AD_UserMail_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MUserMail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// User Mail
        /// </summary>
        /// <param name="parent">Request Mail Text</param>
        /// <param name="AD_User_ID">recipient user</param>
        /// <param name="mail">email</param>
        public MUserMail(MMailText parent, int AD_User_ID, EMail mail)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetAD_User_ID(AD_User_ID);
            SetR_MailText_ID(parent.GetR_MailText_ID());
            //
            if (mail.IsSentOK())
            {
                SetMessageID(mail.GetMessageID());
            }
            else
            {
                SetMessageID(mail.GetSentMsg());
                SetIsDelivered(ISDELIVERED_No);
            }
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">Mail message</param>
        /// <param name="AD_User_ID">recipient user</param>
        /// <param name="mail"> email</param>
        public MUserMail(MMailMsg parent, int AD_User_ID, EMail mail)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetAD_User_ID(AD_User_ID);
            SetW_MailMsg_ID(parent.GetW_MailMsg_ID());
            //
            if (mail.IsSentOK())
            {
                SetMessageID(mail.GetMessageID());
            }
            else
            {
                SetMessageID(mail.GetSentMsg());
                SetIsDelivered(ISDELIVERED_No);
            }
        }

        /// <summary>
        /// New User Mail (no trx)
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <param name="AD_User_ID">recipient user</param>
        /// <param name="mail">email</param>
        public MUserMail(PO po, int AD_User_ID, EMail mail)
            : this(po.GetCtx(), 0, null)
        {

            SetClientOrg(po);
            SetAD_User_ID(AD_User_ID);
            SetSubject(mail.GetSubject());
            SetMailText(mail.GetMessageCRLF());
            //
            if (mail.IsSentOK())
            {
                SetMessageID(mail.GetMessageID());
            }
            else
            {
                SetMessageID(mail.GetSentMsg());
                SetIsDelivered(ISDELIVERED_No);
            }
        }


        /// <summary>
        /// Is it Delivered
        /// </summary>
        /// <returns>true if yes</returns>
        public bool IsDelivered()
        {
            String s = GetIsDelivered();
            return s != null
                && ISDELIVERED_Yes.Equals(s);
        }

        /// <summary>
        /// Is it not Delivered
        /// </summary>
        /// <returns>true if null or no</returns>
        public bool IsDeliveredNo()
        {
            String s = GetIsDelivered();
            return s == null
                || ISDELIVERED_No.Equals(s);
        }

        /// <summary>
        /// Is Delivered unknown
        /// </summary>
        /// <returns>true if null</returns>
        public bool IsDeliveredUnknown()
        {
            String s = GetIsDelivered();
            return s == null;
        }
    }
}
