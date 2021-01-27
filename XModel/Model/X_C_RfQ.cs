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
    using System.Data;/** Generated Model for VAB_RFQ
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_RFQ : PO
    {
        public X_VAB_RFQ(Context ctx, int VAB_RFQ_ID, Trx trxName)
            : base(ctx, VAB_RFQ_ID, trxName)
        {/** if (VAB_RFQ_ID == 0){SetVAB_Currency_ID (0);// @$VAB_Currency_ID @
SetVAB_RFQ_ID (0);SetVAB_RFQ_Subject_ID (0);SetDateResponse (DateTime.Now);SetDocumentNo (null);SetIsInvitedVendorsOnly (false);SetIsQuoteAllQty (false);SetIsQuoteTotalAmt (false);SetIsRfQResponseAccepted (true);// Y
SetIsSelfService (true);// Y
SetName (null);SetProcessed (false);// N
SetQuoteType (null);// S
SetSalesRep_ID (0);} */
        }
        public X_VAB_RFQ(Ctx ctx, int VAB_RFQ_ID, Trx trxName)
            : base(ctx, VAB_RFQ_ID, trxName)
        {/** if (VAB_RFQ_ID == 0){SetVAB_Currency_ID (0);// @$VAB_Currency_ID @
SetVAB_RFQ_ID (0);SetVAB_RFQ_Subject_ID (0);SetDateResponse (DateTime.Now);SetDocumentNo (null);SetIsInvitedVendorsOnly (false);SetIsQuoteAllQty (false);SetIsQuoteTotalAmt (false);SetIsRfQResponseAccepted (true);// Y
SetIsSelfService (true);// Y
SetName (null);SetProcessed (false);// N
SetQuoteType (null);// S
SetSalesRep_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_RFQ() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27867938076683L;/** Last Updated Timestamp 4/2/2020 1:02:40 PM */
        public static long updatedMS = 1585812759894L;/** VAF_TableView_ID=677 */
        public static int Table_ID; // =677;
        /** TableName=VAB_RFQ */
        public static String Table_Name = "VAB_RFQ";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_RFQ[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetVAF_UserContact_ID() { Object ii = Get_Value("VAF_UserContact_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID() { Object ii = Get_Value("VAB_BusinessPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Location.
@param VAB_BPart_Location_ID Identifies the address for this Account/Prospect. */
        public void SetVAB_BPart_Location_ID(int VAB_BPart_Location_ID)
        {
            if (VAB_BPart_Location_ID <= 0) Set_Value("VAB_BPart_Location_ID", null);
            else
                Set_Value("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetVAB_BPart_Location_ID() { Object ii = Get_Value("VAB_BPart_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID) { if (VAB_Currency_ID < 1) throw new ArgumentException("VAB_Currency_ID is mandatory."); Set_Value("VAB_Currency_ID", VAB_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetVAB_Currency_ID() { Object ii = Get_Value("VAB_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param VAB_Order_ID Sales Order */
        public void SetVAB_Order_ID(int VAB_Order_ID)
        {
            if (VAB_Order_ID <= 0) Set_Value("VAB_Order_ID", null);
            else
                Set_Value("VAB_Order_ID", VAB_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetVAB_Order_ID() { Object ii = Get_Value("VAB_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set RfQ.
@param VAB_RFQ_ID Request for Quotation */
        public void SetVAB_RFQ_ID(int VAB_RFQ_ID) { if (VAB_RFQ_ID < 1) throw new ArgumentException("VAB_RFQ_ID is mandatory."); Set_ValueNoCheck("VAB_RFQ_ID", VAB_RFQ_ID); }/** Get RfQ.
@return Request for Quotation */
        public int GetVAB_RFQ_ID() { Object ii = Get_Value("VAB_RFQ_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set RfQ Topic.
@param VAB_RFQ_Subject_ID Topic for Request for Quotations */
        public void SetVAB_RFQ_Subject_ID(int VAB_RFQ_Subject_ID) { if (VAB_RFQ_Subject_ID < 1) throw new ArgumentException("VAB_RFQ_Subject_ID is mandatory."); Set_Value("VAB_RFQ_Subject_ID", VAB_RFQ_Subject_ID); }/** Get RfQ Topic.
@return Topic for Request for Quotations */
        public int GetVAB_RFQ_Subject_ID() { Object ii = Get_Value("VAB_RFQ_Subject_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Copy Lines.
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
        /** QuoteType VAF_Control_Ref_ID=314 */
        public static int QUOTETYPE_VAF_Control_Ref_ID = 314;/** Quote All Lines = A */
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
@param VAR_MailTemplate_ID Text templates for mailings */
        public void SetVAR_MailTemplate_ID(int VAR_MailTemplate_ID)
        {
            if (VAR_MailTemplate_ID <= 0) Set_Value("VAR_MailTemplate_ID", null);
            else
                Set_Value("VAR_MailTemplate_ID", VAR_MailTemplate_ID);
        }/** Get Mail Template.
@return Text templates for mailings */
        public int GetVAR_MailTemplate_ID() { Object ii = Get_Value("VAR_MailTemplate_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Rank RfQ.
@param RankRfQ Rank RfQ */
        public void SetRankRfQ(String RankRfQ) { if (RankRfQ != null && RankRfQ.Length > 1) { log.Warning("Length > 1 - truncated"); RankRfQ = RankRfQ.Substring(0, 1); } Set_Value("RankRfQ", RankRfQ); }/** Get Rank RfQ.
@return Rank RfQ */
        public String GetRankRfQ() { return (String)Get_Value("RankRfQ"); }
        /** SalesRep_ID VAF_Control_Ref_ID=190 */
        public static int SALESREP_ID_VAF_Control_Ref_ID = 190;/** Set Sales Rep.
@param SalesRep_ID Company Agent like Sales Representative, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID) { if (SalesRep_ID < 1) throw new ArgumentException("SalesRep_ID is mandatory."); Set_Value("SalesRep_ID", SalesRep_ID); }/** Get Sales Rep.
@return Company Agent like Sales Representative, Customer Service Representative, ... */
        public int GetSalesRep_ID() { Object ii = Get_Value("SalesRep_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}