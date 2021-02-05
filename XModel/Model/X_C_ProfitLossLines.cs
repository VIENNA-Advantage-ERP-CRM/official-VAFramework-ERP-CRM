namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAB_ProfitLossLines
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProfitLossLines : PO{public X_VAB_ProfitLossLines (Context ctx, int VAB_ProfitLossLines_ID, Trx trxName) : base (ctx, VAB_ProfitLossLines_ID, trxName){/** if (VAB_ProfitLossLines_ID == 0){SetVAB_AccountBook_ID (0);SetVAB_ProfitLossLines_ID (0);SetVAB_ProfitLoss_ID (0);SetPostingType (null);} */
}public X_VAB_ProfitLossLines (Ctx ctx, int VAB_ProfitLossLines_ID, Trx trxName) : base (ctx, VAB_ProfitLossLines_ID, trxName){/** if (VAB_ProfitLossLines_ID == 0){SetVAB_AccountBook_ID (0);SetVAB_ProfitLossLines_ID (0);SetVAB_ProfitLoss_ID (0);SetPostingType (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProfitLossLines (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProfitLossLines (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProfitLossLines (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProfitLossLines(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27764013083488L;/** Last Updated Timestamp 12/16/2016 4:59:30 PM */
public static long updatedMS = 1481887766699L;/** VAF_TableView_ID=1000457 */
public static int Table_ID; // =1000457;
/** TableName=VAB_ProfitLossLines */
public static String Table_Name="VAB_ProfitLossLines";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAB_ProfitLossLines[").Append(Get_ID()).Append("]");return sb.ToString();}
/** VAF_OrgTrx_ID VAF_Control_Ref_ID=276 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=276;/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID){if (VAF_OrgTrx_ID <= 0) Set_Value ("VAF_OrgTrx_ID", null);else
Set_Value ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);}/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() {Object ii = Get_Value("VAF_OrgTrx_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Account Credit.
@param AccountCredit Account Credit */
public void SetAccountCredit (Decimal? AccountCredit){Set_Value ("AccountCredit", (Decimal?)AccountCredit);}/** Get Account Credit.
@return Account Credit */
public Decimal GetAccountCredit() {Object bd =Get_Value("AccountCredit");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Account Debit.
@param AccountDebit Account Debit */
public void SetAccountDebit (Decimal? AccountDebit){Set_Value ("AccountDebit", (Decimal?)AccountDebit);}/** Get Account Debit.
@return Account Debit */
public Decimal GetAccountDebit() {Object bd =Get_Value("AccountDebit");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}
/** Account_ID VAF_Control_Ref_ID=182 */
public static int ACCOUNT_ID_VAF_Control_Ref_ID=182;/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID){if (Account_ID <= 0) Set_Value ("Account_ID", null);else
Set_Value ("Account_ID", Account_ID);}/** Get Account.
@return Account used */
public int GetAccount_ID() {Object ii = Get_Value("Account_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID){if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() {Object ii = Get_Value("VAB_AccountBook_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID){if (VAB_BillingCode_ID <= 0) Set_Value ("VAB_BillingCode_ID", null);else
Set_Value ("VAB_BillingCode_ID", VAB_BillingCode_ID);}/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() {Object ii = Get_Value("VAB_BillingCode_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID){if (VAB_BusinessPartner_ID <= 0) Set_Value ("VAB_BusinessPartner_ID", null);else
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetVAB_BusinessPartner_ID() {Object ii = Get_Value("VAB_BusinessPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID){if (VAB_Promotion_ID <= 0) Set_Value ("VAB_Promotion_ID", null);else
Set_Value ("VAB_Promotion_ID", VAB_Promotion_ID);}/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() {Object ii = Get_Value("VAB_Promotion_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** C_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int C_LOCFROM_ID_VAF_Control_Ref_ID=133;/** Set Location From.
@param C_LocFrom_ID Location that inventory was moved from */
public void SetC_LocFrom_ID (int C_LocFrom_ID){if (C_LocFrom_ID <= 0) Set_Value ("C_LocFrom_ID", null);else
Set_Value ("C_LocFrom_ID", C_LocFrom_ID);}/** Get Location From.
@return Location that inventory was moved from */
public int GetC_LocFrom_ID() {Object ii = Get_Value("C_LocFrom_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** C_LocTo_ID VAF_Control_Ref_ID=133 */
public static int C_LOCTO_ID_VAF_Control_Ref_ID=133;/** Set Location To.
@param C_LocTo_ID Location that inventory was moved to */
public void SetC_LocTo_ID (int C_LocTo_ID){if (C_LocTo_ID <= 0) Set_Value ("C_LocTo_ID", null);else
Set_Value ("C_LocTo_ID", C_LocTo_ID);}/** Get Location To.
@return Location that inventory was moved to */
public int GetC_LocTo_ID() {Object ii = Get_Value("C_LocTo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Profit Dimension.
@param VAB_ProfitAndLoss_ID Profit Dimension */
public void SetVAB_ProfitAndLoss_ID (int VAB_ProfitAndLoss_ID){if (VAB_ProfitAndLoss_ID <= 0) Set_Value ("VAB_ProfitAndLoss_ID", null);else
Set_Value ("VAB_ProfitAndLoss_ID", VAB_ProfitAndLoss_ID);}/** Get Profit Dimension.
@return Profit Dimension */
public int GetVAB_ProfitAndLoss_ID() {Object ii = Get_Value("VAB_ProfitAndLoss_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Profit Loss Line.
@param VAB_ProfitLossLines_ID Profit Loss Line */
public void SetVAB_ProfitLossLines_ID (int VAB_ProfitLossLines_ID){if (VAB_ProfitLossLines_ID < 1) throw new ArgumentException ("VAB_ProfitLossLines_ID is mandatory.");Set_ValueNoCheck ("VAB_ProfitLossLines_ID", VAB_ProfitLossLines_ID);}/** Get Profit Loss Line.
@return Profit Loss Line */
public int GetVAB_ProfitLossLines_ID() {Object ii = Get_Value("VAB_ProfitLossLines_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Profit Loss.
@param VAB_ProfitLoss_ID Profit Loss */
public void SetVAB_ProfitLoss_ID (int VAB_ProfitLoss_ID){if (VAB_ProfitLoss_ID < 1) throw new ArgumentException ("VAB_ProfitLoss_ID is mandatory.");Set_ValueNoCheck ("VAB_ProfitLoss_ID", VAB_ProfitLoss_ID);}/** Get Profit Loss.
@return Profit Loss */
public int GetVAB_ProfitLoss_ID() {Object ii = Get_Value("VAB_ProfitLoss_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Project Phase.
@param VAB_ProjectStage_ID Phase of a Project */
public void SetVAB_ProjectStage_ID (int VAB_ProjectStage_ID){if (VAB_ProjectStage_ID <= 0) Set_Value ("VAB_ProjectStage_ID", null);else
Set_Value ("VAB_ProjectStage_ID", VAB_ProjectStage_ID);}/** Get Project Phase.
@return Phase of a Project */
public int GetVAB_ProjectStage_ID() {Object ii = Get_Value("VAB_ProjectStage_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Project Task.
@param VAB_ProjectJob_ID Actual Plan Task in a Phase */
public void SetVAB_ProjectJob_ID (int VAB_ProjectJob_ID){if (VAB_ProjectJob_ID <= 0) Set_Value ("VAB_ProjectJob_ID", null);else
Set_Value ("VAB_ProjectJob_ID", VAB_ProjectJob_ID);}/** Get Project Task.
@return Actual Plan Task in a Phase */
public int GetVAB_ProjectJob_ID() {Object ii = Get_Value("VAB_ProjectJob_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Opportunity.
@param VAB_Project_ID Business Opportunity */
public void SetVAB_Project_ID (int VAB_Project_ID){if (VAB_Project_ID <= 0) Set_Value ("VAB_Project_ID", null);else
Set_Value ("VAB_Project_ID", VAB_Project_ID);}/** Get Opportunity.
@return Business Opportunity */
public int GetVAB_Project_ID() {Object ii = Get_Value("VAB_Project_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID){if (VAB_SalesRegionState_ID <= 0) Set_Value ("VAB_SalesRegionState_ID", null);else
Set_Value ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);}/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() {Object ii = Get_Value("VAB_SalesRegionState_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Sub Account.
@param VAB_SubAcct_ID Sub account for Element Value */
public void SetVAB_SubAcct_ID (int VAB_SubAcct_ID){if (VAB_SubAcct_ID <= 0) Set_Value ("VAB_SubAcct_ID", null);else
Set_Value ("VAB_SubAcct_ID", VAB_SubAcct_ID);}/** Get Sub Account.
@return Sub account for Element Value */
public int GetVAB_SubAcct_ID() {Object ii = Get_Value("VAB_SubAcct_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Budget.
@param VAGL_Budget_ID General Ledger Budget */
public void SetVAGL_Budget_ID (int VAGL_Budget_ID){if (VAGL_Budget_ID <= 0) Set_Value ("VAGL_Budget_ID", null);else
Set_Value ("VAGL_Budget_ID", VAGL_Budget_ID);}/** Get Budget.
@return General Ledger Budget */
public int GetVAGL_Budget_ID() {Object ii = Get_Value("VAGL_Budget_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Ledger Code.
@param LedgerCode Ledger Code */
public void SetLedgerCode (String LedgerCode){if (LedgerCode != null && LedgerCode.Length > 20){log.Warning("Length > 20 - truncated");LedgerCode = LedgerCode.Substring(0,20);}Set_Value ("LedgerCode", LedgerCode);}/** Get Ledger Code.
@return Ledger Code */
public String GetLedgerCode() {return (String)Get_Value("LedgerCode");}/** Set Ledger Name.
@param LedgerName Ledger Name */
public void SetLedgerName (String LedgerName){if (LedgerName != null && LedgerName.Length > 100){log.Warning("Length > 100 - truncated");LedgerName = LedgerName.Substring(0,100);}Set_Value ("LedgerName", LedgerName);}/** Get Ledger Name.
@return Ledger Name */
public String GetLedgerName() {return (String)Get_Value("LedgerName");}/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line){Set_Value ("Line", Line);}/** Get Line No.
@return Unique line for this document */
public int GetLine() {Object ii = Get_Value("Line");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID){if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);else
Set_Value ("VAM_Product_ID", VAM_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() {Object ii = Get_Value("VAM_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** PostingType VAF_Control_Ref_ID=125 */
public static int POSTINGTYPE_VAF_Control_Ref_ID=125;/** Actual = A */
public static String POSTINGTYPE_Actual = "A";/** Budget = B */
public static String POSTINGTYPE_Budget = "B";/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test){return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("S") || test.Equals("R");}/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType){if (PostingType == null) throw new ArgumentException ("PostingType is mandatory");if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - S - R");if (PostingType.Length > 1){log.Warning("Length > 1 - truncated");PostingType = PostingType.Substring(0,1);}Set_Value ("PostingType", PostingType);}/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() {return (String)Get_Value("PostingType");}/** Set Source Credit.
@param SourceCredit Source Credit */
public void SetSourceCredit (Decimal? SourceCredit){Set_Value ("SourceCredit", (Decimal?)SourceCredit);}/** Get Source Credit.
@return Source Credit */
public Decimal GetSourceCredit() {Object bd =Get_Value("SourceCredit");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Source Debit.
@param SourceDebit Source Debit */
public void SetSourceDebit (Decimal? SourceDebit){Set_Value ("SourceDebit", (Decimal?)SourceDebit);}/** Get Source Debit.
@return Source Debit */
public Decimal GetSourceDebit() {Object bd =Get_Value("SourceDebit");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}
/** User1_ID VAF_Control_Ref_ID=134 */
public static int USER1_ID_VAF_Control_Ref_ID=134;/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID){if (User1_ID <= 0) Set_Value ("User1_ID", null);else
Set_Value ("User1_ID", User1_ID);}/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() {Object ii = Get_Value("User1_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** User2_ID VAF_Control_Ref_ID=137 */
public static int USER2_ID_VAF_Control_Ref_ID=137;/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID){if (User2_ID <= 0) Set_Value ("User2_ID", null);else
Set_Value ("User2_ID", User2_ID);}/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() {Object ii = Get_Value("User2_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 1.
@param UserElement1_ID User defined accounting Element */
public void SetUserElement1_ID (int UserElement1_ID){if (UserElement1_ID <= 0) Set_Value ("UserElement1_ID", null);else
Set_Value ("UserElement1_ID", UserElement1_ID);}/** Get User Element 1.
@return User defined accounting Element */
public int GetUserElement1_ID() {Object ii = Get_Value("UserElement1_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 2.
@param UserElement2_ID User defined accounting Element */
public void SetUserElement2_ID (int UserElement2_ID){if (UserElement2_ID <= 0) Set_Value ("UserElement2_ID", null);else
Set_Value ("UserElement2_ID", UserElement2_ID);}/** Get User Element 2.
@return User defined accounting Element */
public int GetUserElement2_ID() {Object ii = Get_Value("UserElement2_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 3.
@param UserElement3_ID User defined accounting Element */
public void SetUserElement3_ID (int UserElement3_ID){if (UserElement3_ID <= 0) Set_Value ("UserElement3_ID", null);else
Set_Value ("UserElement3_ID", UserElement3_ID);}/** Get User Element 3.
@return User defined accounting Element */
public int GetUserElement3_ID() {Object ii = Get_Value("UserElement3_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 4.
@param UserElement4_ID User defined accounting Element */
public void SetUserElement4_ID (int UserElement4_ID){if (UserElement4_ID <= 0) Set_Value ("UserElement4_ID", null);else
Set_Value ("UserElement4_ID", UserElement4_ID);}/** Get User Element 4.
@return User defined accounting Element */
public int GetUserElement4_ID() {Object ii = Get_Value("UserElement4_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 5.
@param UserElement5_ID User defined accounting Element */
public void SetUserElement5_ID (int UserElement5_ID){if (UserElement5_ID <= 0) Set_Value ("UserElement5_ID", null);else
Set_Value ("UserElement5_ID", UserElement5_ID);}/** Get User Element 5.
@return User defined accounting Element */
public int GetUserElement5_ID() {Object ii = Get_Value("UserElement5_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 6.
@param UserElement6_ID User defined accounting Element */
public void SetUserElement6_ID (int UserElement6_ID){if (UserElement6_ID <= 0) Set_Value ("UserElement6_ID", null);else
Set_Value ("UserElement6_ID", UserElement6_ID);}/** Get User Element 6.
@return User defined accounting Element */
public int GetUserElement6_ID() {Object ii = Get_Value("UserElement6_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 7.
@param UserElement7_ID User defined accounting Element */
public void SetUserElement7_ID (int UserElement7_ID){if (UserElement7_ID <= 0) Set_Value ("UserElement7_ID", null);else
Set_Value ("UserElement7_ID", UserElement7_ID);}/** Get User Element 7.
@return User defined accounting Element */
public int GetUserElement7_ID() {Object ii = Get_Value("UserElement7_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 8.
@param UserElement8_ID User defined accounting Element */
public void SetUserElement8_ID (int UserElement8_ID){if (UserElement8_ID <= 0) Set_Value ("UserElement8_ID", null);else
Set_Value ("UserElement8_ID", UserElement8_ID);}/** Get User Element 8.
@return User defined accounting Element */
public int GetUserElement8_ID() {Object ii = Get_Value("UserElement8_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User Element 9.
@param UserElement9_ID User defined accounting Element */
public void SetUserElement9_ID (int UserElement9_ID){if (UserElement9_ID <= 0) Set_Value ("UserElement9_ID", null);else
Set_Value ("UserElement9_ID", UserElement9_ID);}/** Get User Element 9.
@return User defined accounting Element */
public int GetUserElement9_ID() {Object ii = Get_Value("UserElement9_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}