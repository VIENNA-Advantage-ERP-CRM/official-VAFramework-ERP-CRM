/********************************************************
 * Project Name   : VAdvantage
 * Form Name      : Chat
 * Purpose        : To featch/insert(get/set) data in the VACM_ChatLine table.
 * Class Used     : MChatEntry (inherits X_VACM_ChatLine class) 
 * Chronological    Development
 * Raghunandan      13-March-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVACMChatLine : X_VACM_ChatLine
    {
        #region Standard Constructor
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="VACM_ChatLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVACMChatLine(Ctx ctx, int VACM_ChatLine_ID, Trx trxName)
            : base(ctx, VACM_ChatLine_ID, trxName)
        {
            //super(ctx, VACM_ChatLine_ID, trxName);
            //chatentry in chatentry table is zero
            if (VACM_ChatLine_ID == 0)
            {
                //set chat type N
                SetChatEntryType(CHATENTRYTYPE_NoteFlat);	// N
                //set confidentail chat type
                SetConfidentialType(CONFIDENTIALTYPE_PublicInformation);
            }
        }
        #endregion

        #region Load Constructor
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVACMChatLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }
        #endregion

        #region Parent Constructor
        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="chat">chat parent</param>
        /// <param name="data">data text</param>
        public MVACMChatLine(MVACMChat chat, String data)
            : this(chat.GetCtx(), 0, chat.Get_TrxName())
        {
            //set chatID
            SetCM_Chat_ID(chat.GetCM_Chat_ID());
            //set chat confidential type
            SetConfidentialType(chat.GetConfidentialType());
            //set entered chat text 
            SetCharacterData(data);
            //chat entry type "N"
            SetChatEntryType(CHATENTRYTYPE_NoteFlat);	// N
        }
        #endregion

        #region Thread Constructor
        /// <summary>
        ///	Thread Constructor
        /// </summary>
        /// <param name="peer">entry peer</param>
        /// <param name="data">data text</param>
        public MVACMChatLine(MVACMChatLine peer, String data)
            : this(peer.GetCtx(), 0, peer.Get_TrxName())
        {
            //set chat in MChatEntry 
            SetCM_Chat_ID(peer.GetCM_Chat_ID());
            //parent chat entry
            SetVACM_ChatLineParent_ID(peer.GetVACM_ChatLineParent_ID());
            //	Set GrandParent
            int id = peer.GetVACM_ChatLineGrandParent_ID();
            //if no chat
            if (id == 0)
                id = peer.GetVACM_ChatLineParent_ID();
            //set M_ChatEntryGrandParent_ID
            SetVACM_ChatLineGrandParent_ID(id);
            //confidential chat type
            SetConfidentialType(peer.GetConfidentialType());
            //chat text
            SetCharacterData(data);
            //chat entry
            SetChatEntryType(CHATENTRYTYPE_ForumThreaded);
        }
        #endregion

        #region Confidential Type
        /// <summary>
        /// Can be published
        /// </summary>
        /// <param name="confidentialType">minimum confidential type</param>
        /// <returns>BoolType(true if withing confidentiality)</returns>
        public Boolean isConfidentialType(String confidentialType)
        {
            //get chat type
            String ct = GetConfidentialType();
            //when no chat type set
            if (confidentialType == null
                || CONFIDENTIALTYPE_PublicInformation.Equals(ct))
                return true;
            //parent chat type
            if (CONFIDENTIALTYPE_PartnerConfidential.Equals(ct))
            {
                return CONFIDENTIALTYPE_PartnerConfidential.Equals(confidentialType);
            }
            //chat info
            else if (CONFIDENTIALTYPE_PrivateInformation.Equals(ct))
            {
                return CONFIDENTIALTYPE_Internal.Equals(confidentialType)
                    || CONFIDENTIALTYPE_PrivateInformation.Equals(confidentialType);
            }
            //internal chat
            else if (CONFIDENTIALTYPE_Internal.Equals(ct))
            {
                return CONFIDENTIALTYPE_Internal.Equals(confidentialType);
            }
            return false;
        }
        #endregion
    }
}
