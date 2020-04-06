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
    using System.Data;/** Generated Model for C_RfQ
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_RfQ : PO
    {
        public X_C_RfQ(Context ctx, int C_RfQ_ID, Trx trxName)
            : base(ctx, C_RfQ_ID, trxName)
        {/** if (C_RfQ_ID == 0){SetC_Currency_ID (0);// @$C_Currency_ID @
SetC_RfQ_ID (0);SetC_RfQ_Topic_ID (0);SetDateResponse (DateTime.Now);SetDocumentNo (null);SetIsInvitedVendorsOnly (false);SetIsQuoteAllQty (false);SetIsQuoteTotalAmt (false);SetIsRfQResponseAccepted (true);// Y
SetIsSelfService (true);// Y
SetName (null);SetProcessed (false);// N
SetQuoteType (null);// S
SetSalesRep_ID (0);} */
        }
        public X_C_RfQ(Ctx ctx, int C_RfQ_ID, Trx trxName)
            : base(ctx, C_RfQ_ID, trxName)
        {/** if (C_RfQ_ID == 0){SetC_Currency_ID (0);// @$C_Currency_ID @
SetC_RfQ_ID (0);SetC_RfQ_Topic_ID (0);SetDateResponse (DateTime.Now);SetDocumentNo (null);SetIsInvitedVendorsOnly (false);SetIsQuoteAllQty (false);SetIsQuoteTotalAmt (false);SetIsRfQResponseAccepted (true);// Y
SetIsSelfService (true);// Y
SetName (null);SetProcessed (false);// N
SetQuoteType (null);// S
SetSalesRep_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RfQ(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RfQ(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RfQ(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_RfQ() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27867938076683L;/** Last Updated Timestamp 4/2/2020 1:02:40 PM */
        public static long updatedMS = 1585812759894L;/** AD_Table_ID=677 */
        public static int Table_ID; // =677;
        /** TableName=C_RfQ */
        public static String Table_Name = "C_RfQ";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_RfQ[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID() { Object ii = Get_Value("AD_User_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID() { Object ii = Get_Value("C_BPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Location.
@param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetC_BPartner_Location_ID() { Object ii = Get_Value("C_BPartner_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID) { if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory."); Set_Value("C_Currency_ID", C_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set RfQ.
@param C_RfQ_ID Request for Quotation */
        public void SetC_RfQ_ID(int C_RfQ_ID) { if (C_RfQ_ID < 1) throw new ArgumentException("C_RfQ_ID is mandatory."); Set_ValueNoCheck("C_RfQ_ID", C_RfQ_ID); }/** Get RfQ.
@return Request for Quotation */
        public int GetC_RfQ_ID() { Object ii = Get_Value("C_RfQ_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set RfQ Topic.
@param C_RfQ_Topic_ID Topic for Request for Quotations */
        public void SetC_RfQ_Topic_ID(int C_RfQ_Topic_ID) { if (C_RfQ_Topic_ID < 1) throw new ArgumentException("C_RfQ_Topic_ID is mandatory."); Set_Value("C_RfQ_Topic_ID", C_RfQ_Topic_ID); }/** Get RfQ Topic.
@return Topic for Request for Quotations */
        public int GetC_RfQ_Topic_ID() { Object ii = Get_Value("C_RfQ_Topic_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Copy Lines.
@param CopyLines Copy Lines */
        public void SetCopyLines(String CopyLines) { if (CopyLines != null && CopyLines.Length > 1) { log.Warning("Length > 1 - truncated"); CopyLines = CopyLines.Substring(0, 1); } Set_Value("CopyLines", CopyLines); }/** Get Copy Lines.
@return Copy Lines */
        public String GetCopyLines() { return (String)Get_Value("CopyLines"); }/** Set Create PO.
@param CreatePO Create Purchase Order */
        public void SetCreatePO(String CreatePO) { if (CreatePO != null && CreatePO.Length > 1) { log.Warning("Length > 1 - truncated"); CreatePO = CreatePO.Substring(0, 1); } Set_Value("CreatePO", CreatePO); }/** Get Create PO.
@return Create Purchase Order */
        public String GetCreatePO() { return (String)Get_Value("CreatePO"); }/** Set Create SO.
@param CreateSO Create SO */
        public void SetCreateSO(String CreateSO) { if (CreateSO != null && CreateSO.Length > 1) { log.Warning("Length > 1 - truncated"); CreateSO = CreateSO.Substring(0, 1); } Set_Value("CreateSO", CreateSO); }/** Get Create SO.
@return Create SO */
        public String GetCreateSO() { return (String)Get_Value("CreateSO"); }/** Set Response Date.
@param DateResponse Date of the Response */
        public void SetDateResponse(DateTime? DateResponse) { if (DateResponse == null) throw new ArgumentException("DateResponse is mandatory."); Set_Value("DateResponse", (DateTime?)DateResponse); }/** Get Response Date.
@return Date of the Response */
        public DateTime? GetDateResponse() { return (DateTime?)Get_Value("DateResponse"); }/** Set Work Complete.
@param DateWorkComplete Date when work is (planned to be) complete */
        public void SetDateWorkComplete(DateTime? DateWorkComplete) { Set_Value("DateWorkComplete", (DateTime?)DateWorkComplete); }/** Get Work Complete.
@return Date when work is (planned to be) complete */
        public DateTime? GetDateWorkComplete() { return (DateTime?)Get_Value("DateWorkComplete"); }/** Set Work Start.
@param DateWorkStart Date when work is (planned to be) started */
        public void SetDateWorkStart(DateTime? DateWorkStart) { Set_Value("DateWorkStart", (DateTime?)DateWorkStart); }/** Get Work Start.
@return Date when work is (planned to be) started */
        public DateTime? GetDateWorkStart() { return (DateTime?)Get_Value("DateWorkStart"); }/** Set Delivery Days.
@param DeliveryDays Number of Days (planned) until Delivery */
        public void SetDeliveryDays(int DeliveryDays) { Set_Value("DeliveryDays", DeliveryDays); }/** Get Delivery Days.
@return Number of Days (planned) until Delivery */
        public int GetDeliveryDays() { Object ii = Get_Value("DeliveryDays"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Document No..
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo == null) throw new ArgumentException("DocumentNo is mandatory."); if (DocumentNo.Length > 30) { log.Warning("Length > 30 - truncated"); DocumentNo = DocumentNo.Substring(0, 30); } Set_Value("DocumentNo", DocumentNo); }/** Get Document No..
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetDocumentNo()); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Comment.
@param Help Comment, Help or Hint */
        public void SetHelp(String Help) { if (Help != null && Help.Length > 2000) { log.Warning("Length > 2000 - truncated"); Help = Help.Substring(0, 2000); } Set_Value("Help", Help); }/** Get Comment.
@return Comment, Help or Hint */
        public String GetHelp() { return (String)Get_Value("Help"); }/** Set Invite User.
@param InviteUser Invite the vendors/users */
        public void SetInviteUser(String InviteUser) { if (InviteUser != null && InviteUser.Length > 1) { log.Warning("Length > 1 - truncated"); InviteUser = InviteUser.Substring(0, 1); } Set_Value("InviteUser", InviteUser); }/** Get Invite User.
@return Invite the vendors/users */
        public String GetInviteUser() { return (String)Get_Value("InviteUser"); }/** Set Invited Vendors Only.
@param IsInvitedVendorsOnly Only invited vendors can respond to an RfQ */
        public void SetIsInvitedVendorsOnly(Boolean IsInvitedVendorsOnly) { Set_Value("IsInvitedVendorsOnly", IsInvitedVendorsOnly); }/** Get Invited Vendors Only.
@return Only invited vendors can respond to an RfQ */
        public Boolean IsInvitedVendorsOnly() { Object oo = Get_Value("IsInvitedVendorsOnly"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quote All Quantities.
@param IsQuoteAllQty Suppliers are requested to provide responses for all quantities */
        public void SetIsQuoteAllQty(Boolean IsQuoteAllQty) { Set_Value("IsQuoteAllQty", IsQuoteAllQty); }/** Get Quote All Quantities.
@return Suppliers are requested to provide responses for all quantities */
        public Boolean IsQuoteAllQty() { Object oo = Get_Value("IsQuoteAllQty"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quote Total Amt.
@param IsQuoteTotalAmt The respnse can have just the total amount for the RfQ */
        public void SetIsQuoteTotalAmt(Boolean IsQuoteTotalAmt) { Set_Value("IsQuoteTotalAmt", IsQuoteTotalAmt); }/** Get Quote Total Amt.
@return The respnse can have just the total amount for the RfQ */
        public Boolean IsQuoteTotalAmt() { Object oo = Get_Value("IsQuoteTotalAmt"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Responses Accepted.
@param IsRfQResponseAccepted Are Resonses to the Request for Quotation accepted */
        public void SetIsRfQResponseAccepted(Boolean IsRfQResponseAccepted) { Set_Value("IsRfQResponseAccepted", IsRfQResponseAccepted); }/** Get Responses Accepted.
@return Are Resonses to the Request for Quotation accepted */
        public Boolean IsRfQResponseAccepted() { Object oo = Get_Value("IsRfQResponseAccepted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService) { Set_Value("IsSelfService", IsSelfService); }/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
        public Boolean IsSelfService() { Object oo = Get_Value("IsSelfService"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Requisition.
@param M_Requisition_ID Material Requisition */
        public void SetM_Requisition_ID(int M_Requisition_ID)
        {
            if (M_Requisition_ID <= 0) Set_Value("M_Requisition_ID", null);
            else
                Set_Value("M_Requisition_ID", M_Requisition_ID);
        }/** Get Requisition.
@return Material Requisition */
        public int GetM_Requisition_ID() { Object ii = Get_Value("M_Requisition_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Margin %.
@param Margin Margin for a product as a percentage */
        public void SetMargin(Decimal? Margin) { Set_Value("Margin", (Decimal?)Margin); }/** Get Margin %.
@return Margin for a product as a percentage */
        public Decimal GetMargin() { Object bd = Get_Value("Margin"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 60) { log.Warning("Length > 60 - truncated"); Name = Name.Substring(0, 60); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Publish RfQ.
@param PublishRfQ Publish RfQ */
        public void SetPublishRfQ(String PublishRfQ) { if (PublishRfQ != null && PublishRfQ.Length > 1) { log.Warning("Length > 1 - truncated"); PublishRfQ = PublishRfQ.Substring(0, 1); } Set_Value("PublishRfQ", PublishRfQ); }/** Get Publish RfQ.
@return Publish RfQ */
        public String GetPublishRfQ() { return (String)Get_Value("PublishRfQ"); }
        /** QuoteType AD_Reference_ID=314 */
        public static int QUOTETYPE_AD_Reference_ID = 314;/** Quote All Lines = A */
        public static String QUOTETYPE_QuoteAllLines = "A";/** Quote Selected Lines = S */
        public static String QUOTETYPE_QuoteSelectedLines = "S";/** Quote Total only = T */
        public static String QUOTETYPE_QuoteTotalOnly = "T";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsQuoteTypeValid(String test) { return test.Equals("A") || test.Equals("S") || test.Equals("T"); }/** Set RfQ Type.
@param QuoteType Request for Quotation Type */
        public void SetQuoteType(String QuoteType)
        {
            if (QuoteType == null) throw new ArgumentException("QuoteType is mandatory"); if (!IsQuoteTypeValid(QuoteType))
                throw new ArgumentException("QuoteType Invalid value - " + QuoteType + " - Reference_ID=314 - A - S - T"); if (QuoteType.Length > 1) { log.Warning("Length > 1 - truncated"); QuoteType = QuoteType.Substring(0, 1); }
            Set_Value("QuoteType", QuoteType);
        }/** Get RfQ Type.
@return Request for Quotation Type */
        public String GetQuoteType() { return (String)Get_Value("QuoteType"); }/** Set Mail Template.
@param R_MailText_ID Text templates for mailings */
        public void SetR_MailText_ID(int R_MailText_ID)
        {
            if (R_MailText_ID <= 0) Set_Value("R_MailText_ID", null);
            else
                Set_Value("R_MailText_ID", R_MailText_ID);
        }/** Get Mail Template.
@return Text templates for mailings */
        public int GetR_MailText_ID() { Object ii = Get_Value("R_MailText_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Rank RfQ.
@param RankRfQ Rank RfQ */
        public void SetRankRfQ(String RankRfQ) { if (RankRfQ != null && RankRfQ.Length > 1) { log.Warning("Length > 1 - truncated"); RankRfQ = RankRfQ.Substring(0, 1); } Set_Value("RankRfQ", RankRfQ); }/** Get Rank RfQ.
@return Rank RfQ */
        public String GetRankRfQ() { return (String)Get_Value("RankRfQ"); }
        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;/** Set Sales Rep.
@param SalesRep_ID Company Agent like Sales Representative, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID) { if (SalesRep_ID < 1) throw new ArgumentException("SalesRep_ID is mandatory."); Set_Value("SalesRep_ID", SalesRep_ID); }/** Get Sales Rep.
@return Company Agent like Sales Representative, Customer Service Representative, ... */
        public int GetSalesRep_ID() { Object ii = Get_Value("SalesRep_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}