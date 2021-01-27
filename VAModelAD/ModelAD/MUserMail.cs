/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MUserMail
 * Purpose        : User Mail Model
 * Class Used     : X_VAF_UserMailLog
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
    public class MUserMail : X_VAF_UserMailLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_UserMailLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MUserMail(Ctx ctx, int VAF_UserMailLog_ID, Trx trxName)
            : base(ctx, VAF_UserMailLog_ID, trxName)
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
        /// <param name="VAF_UserContact_ID">recipient user</param>
        /// <param name="mail">email</param>
        public MUserMail(MMailText parent, int VAF_UserContact_ID, EMail mail)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetVAF_UserContact_ID(VAF_UserContact_ID);
            SetVAR_MailTemplate_ID(parent.GetVAR_MailTemplate_ID());
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
        /// <param name="VAF_UserContact_ID">recipient user</param>
        /// <param name="mail"> email</param>
        public MUserMail(MMailMsg parent, int VAF_UserContact_ID, EMail mail)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetVAF_UserContact_ID(VAF_UserContact_ID);
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
        /// <param name="VAF_UserContact_ID">recipient user</param>
        /// <param name="mail">email</param>
        public MUserMail(PO po, int VAF_UserContact_ID, EMail mail)
            : this(po.GetCtx(), 0, null)
        {

            SetClientOrg(po);
            SetVAF_UserContact_ID(VAF_UserContact_ID);
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
