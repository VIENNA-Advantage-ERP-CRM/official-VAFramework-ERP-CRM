/********************************************************
 * Project Name   : VAdvantage
 * Form Name      : Chat
 * Purpose        : To featch/insert(get/set) data in the CM_CHATENTRY table.
 * Class Used     : MChatEntry (inherits X_CM_ChatEntry class) 
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
    public class MChatEntry : X_CM_ChatEntry
    {
        #region Standard Constructor
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="CM_ChatEntry_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MChatEntry(Ctx ctx, int CM_ChatEntry_ID, Trx trxName)
            : base(ctx, CM_ChatEntry_ID, trxName)
        {
            //super(ctx, CM_ChatEntry_ID, trxName);
            //chatentry in chatentry table is zero
            if (CM_ChatEntry_ID == 0)
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
        public MChatEntry(Ctx ctx, DataRow rs, Trx trxName)
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
        public MChatEntry(MChat chat, String data)
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
        public MChatEntry(MChatEntry peer, String data)
            : this(peer.GetCtx(), 0, peer.Get_TrxName())
        {
            //set chat in MChatEntry 
            SetCM_Chat_ID(peer.GetCM_Chat_ID());
            //parent chat entry
            SetCM_ChatEntryParent_ID(peer.GetCM_ChatEntryParent_ID());
            //	Set GrandParent
            int id = peer.GetCM_ChatEntryGrandParent_ID();
            //if no chat
            if (id == 0)
                id = peer.GetCM_ChatEntryParent_ID();
            //set M_ChatEntryGrandParent_ID
            SetCM_ChatEntryGrandParent_ID(id);
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
