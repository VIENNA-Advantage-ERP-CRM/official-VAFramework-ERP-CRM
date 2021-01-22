/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttachmentNote
 * Purpose        : To show attachment note 
 * Class Used     : MAttachmentNote inherits X_VAF_AttachmentNote class
 * Chronological    Development
 * Raghunandan      14-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAttachmentNote : X_VAF_AttachmentNote
    {
        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_AttachmentNote_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAttachmentNote(Ctx ctx, int VAF_AttachmentNote_ID, Trx trxName)
            : base(ctx, VAF_AttachmentNote_ID, trxName)
        {
            /**
            if (VAF_AttachmentNote_ID == 0)
            {
                setVAF_Attachment_ID (0);
                setVAF_UserContact_ID (0);
                setTextMsg (null);
                setTitle (null);
            }
            /**/
        }

        /// <summary>
        ///Load Constructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set(data row)</param>
        /// <param name="trxName">transaction</param>
        public MAttachmentNote(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///Parent Constructor.
        ///Sets current user.
        /// </summary>
        /// <param name="attach">attachment</param>
        /// <param name="Title">title</param>
        /// <param name="TextMsg">text message</param>
        public MAttachmentNote(MAttachment attach, string Title, string TextMsg)
            : this(attach.GetCtx(), 0, attach.Get_TrxName())
        {
            SetClientOrg(attach);
            SetVAF_Attachment_ID(attach.GetVAF_Attachment_ID());
            SetVAF_UserContact_ID(attach.GetCtx().GetVAF_UserContact_ID());
            SetTitle(Title);
            SetTextMsg(TextMsg);
        }

    }
}
