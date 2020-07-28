namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for MailAttachment1
     *  @author Jagmohan Bhatt (generated) 
        *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_MailAttachment1 : PO
    {
        public X_MailAttachment1(Context ctx, int MailAttachment1_ID, Trx trxName)
            : base(ctx, MailAttachment1_ID, trxName)
        {
            /** if (MailAttachment1_ID == 0)
            {  
            SetMailAttachment1_ID (0);
            }
             */
        }
        public X_MailAttachment1(Ctx ctx, int MailAttachment1_ID, Trx trxName)
            : base(ctx, MailAttachment1_ID, trxName)
        {
            /** if (MailAttachment1_ID == 0)
            {
            SetMailAttachment1_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_MailAttachment1(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_MailAttachment1(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_MailAttachment1(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_MailAttachment1()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27561318348373L;
        /** Last Updated Timestamp 7/15/2010 4:53:51 PM */
        public static long updatedMS = 1279193031584L;
        /** AD_Table_ID=1000004 */
        public static int Table_ID;
        // =1000004;

        /** TableName=MailAttachment1 */
        public static String Table_Name = "MailAttachment1";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_MailAttachment1[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
                Set_Value("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AttachmentType.
        @param AttachmentType AttachmentType */
        public void SetAttachmentType(String AttachmentType)
        {
            if (AttachmentType != null && AttachmentType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AttachmentType = AttachmentType.Substring(0, 1);
            }
            Set_Value("AttachmentType", AttachmentType);
        }
        /** Get AttachmentType.
        @return AttachmentType */
        public String GetAttachmentType()
        {
            return (String)Get_Value("AttachmentType");
        }
        /** Set BinaryData.
        @param BinaryData Binary Data */
        public void SetBinaryData(Byte[] BinaryData)
        {
            Set_Value("BinaryData", BinaryData);
        }
        /** Get BinaryData.
        @return Binary Data */
        public Byte[] GetBinaryData()
        {
            return (Byte[])Get_Value("BinaryData");
        }
        /** Set DateMailReceived.
        @param DateMailReceived DateMailReceived */
        public void SetDateMailReceived(DateTime? DateMailReceived)
        {
            Set_Value("DateMailReceived", (DateTime?)DateMailReceived);
        }
        /** Get DateMailReceived.
        @return DateMailReceived */
        public DateTime? GetDateMailReceived()
        {
            return (DateTime?)Get_Value("DateMailReceived");
        }
        /** Set IsAttachment.
        @param IsAttachment IsAttachment */
        public void SetIsAttachment(Boolean IsAttachment)
        {
            Set_Value("IsAttachment", IsAttachment);
        }
        /** Get IsAttachment.
        @return IsAttachment */
        public Boolean IsAttachment()
        {
            Object oo = Get_Value("IsAttachment");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsMailSent.
        @param IsMailSent IsMailSent */
        public void SetIsMailSent(Boolean IsMailSent)
        {
            Set_Value("IsMailSent", IsMailSent);
        }
        /** Get IsMailSent.
        @return IsMailSent */
        public Boolean IsMailSent()
        {
            Object oo = Get_Value("IsMailSent");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set MailAddress.
        @param MailAddress MailAddress */
        public void SetMailAddress(String MailAddress)
        {
            if (MailAddress != null && MailAddress.Length > 1500)
            {
                log.Warning("Length > 1500 - truncated");
                MailAddress = MailAddress.Substring(0, 1500);
            }
            Set_Value("MailAddress", MailAddress);
        }
        /** Get MailAddress.
        @return MailAddress */
        public String GetMailAddress()
        {
            return (String)Get_Value("MailAddress");
        }
        /** Set MailAddressBcc.
        @param MailAddressBcc MailAddressBcc */
        public void SetMailAddressBcc(String MailAddressBcc)
        {
            if (MailAddressBcc != null && MailAddressBcc.Length > 1500)
            {
                log.Warning("Length > 500 - truncated");
                MailAddressBcc = MailAddressBcc.Substring(0, 1500);
            }
            Set_Value("MailAddressBcc", MailAddressBcc);
        }
        /** Get MailAddressBcc.
        @return MailAddressBcc */
        public String GetMailAddressBcc()
        {
            return (String)Get_Value("MailAddressBcc");
        }
        /** Set MailAddressCc.
        @param MailAddressCc MailAddressCc */
        public void SetMailAddressCc(String MailAddressCc)
        {
            if (MailAddressCc != null && MailAddressCc.Length > 1500)
            {
                log.Warning("Length > 1500 - truncated");
                MailAddressCc = MailAddressCc.Substring(0, 1500);
            }
            Set_Value("MailAddressCc", MailAddressCc);
        }
        /** Get MailAddressCc.
        @return MailAddressCc */
        public String GetMailAddressCc()
        {
            return (String)Get_Value("MailAddressCc");
        }
        /** Set MailAddressFrom.
        @param MailAddressFrom MailAddressFrom */
        public void SetMailAddressFrom(String MailAddressFrom)
        {
            if (MailAddressFrom != null && MailAddressFrom.Length > 1500)
            {
                log.Warning("Length > 1500 - truncated");
                MailAddressFrom = MailAddressFrom.Substring(0, 1500);
            }
            Set_Value("MailAddressFrom", MailAddressFrom);
        }
        /** Get MailAddressFrom.
        @return MailAddressFrom */
        public String GetMailAddressFrom()
        {
            return (String)Get_Value("MailAddressFrom");
        }
        /** Set MailAttachment1_ID.
        @param MailAttachment1_ID MailAttachment1_ID */
        public void SetMailAttachment1_ID(int MailAttachment1_ID)
        {
            if (MailAttachment1_ID < 1) throw new ArgumentException("MailAttachment1_ID is mandatory.");
            Set_ValueNoCheck("MailAttachment1_ID", MailAttachment1_ID);
        }
        /** Get MailAttachment1_ID.
        @return MailAttachment1_ID */
        public int GetMailAttachment1_ID()
        {
            Object ii = Get_Value("MailAttachment1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set MailUID.
        @param MailUID MailUID */
        public void SetMailUID(int MailUID)
        {
            Set_Value("MailUID", MailUID);
        }
        /** Get MailUID.
        @return MailUID */
        public int GetMailUID()
        {
            Object ii = Get_Value("MailUID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set MailUserName.
        @param MailUserName MailUserName */
        public void SetMailUserName(String MailUserName)
        {
            if (MailUserName != null && MailUserName.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                MailUserName = MailUserName.Substring(0, 100);
            }
            Set_Value("MailUserName", MailUserName);
        }
        /** Get MailUserName.
        @return MailUserName */
        public String GetMailUserName()
        {
            return (String)Get_Value("MailUserName");
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID <= 0) Set_Value("Record_ID", null);
            else
                Set_Value("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Text Message.
        @param TextMsg Text Message */
        public void SetTextMsg(String TextMsg)
        {
            Set_Value("TextMsg", TextMsg);
        }
        /** Get Text Message.
        @return Text Message */
        public String GetTextMsg()
        {
            return (String)Get_Value("TextMsg");
        }


        /** Set Title.
        @param Title Title of the Contact */
        public void SetTitle(String Title)
        {
            if (Title != null && Title.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Title = Title.Substring(0, 100);
            }
            Set_Value("Title", Title);
        }

        /** Get Title.
        @return Title of the Contact */
        public String GetTitle()
        {
            return (String)Get_Value("Title");
        }


        /** Set FolderName.
        @param FolderName FolderName */
        public void SetFolderName(String FolderName)
        {
            if (FolderName != null && FolderName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                FolderName = FolderName.Substring(0, 50);
            }
            Set_Value("FolderName", FolderName);
        }
        /** Get FolderName.
        @return FolderName */
        public String GetFolderName()
        {
            return (String)Get_Value("FolderName");
        }

    }

}
