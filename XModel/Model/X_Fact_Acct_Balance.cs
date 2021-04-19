namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for Actual_Acct_Balance
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_Actual_Acct_Balance : PO{public X_Actual_Acct_Balance (Context ctx, int Actual_Acct_Balance_ID, Trx trxName) : base (ctx, Actual_Acct_Balance_ID, trxName){/** if (Actual_Acct_Balance_ID == 0){SetAccount_ID (0);SetAmtAcctCr (0.0);SetAmtAcctDr (0.0);SetVAB_AccountBook_ID (0);SetDateAcct (DateTime.Now);SetPostingType (null);SetQty (0.0);} */
}public X_Actual_Acct_Balance (Ctx ctx, int Actual_Acct_Balance_ID, Trx trxName) : base (ctx, Actual_Acct_Balance_ID, trxName){/** if (Actual_Acct_Balance_ID == 0){SetAccount_ID (0);SetAmtAcctCr (0.0);SetAmtAcctDr (0.0);SetVAB_AccountBook_ID (0);SetDateAcct (DateTime.Now);SetPostingType (null);SetQty (0.0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Actual_Acct_Balance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Actual_Acct_Balance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Actual_Acct_Balance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_Actual_Acct_Balance(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27741383653181L;/** Last Updated Timestamp 3/29/2016 7:02:17 PM */
public static long updatedMS = 1459258336392L;/** VAF_TableView_ID=547 */
public static int Table_ID; // =547;
/** TableName=Actual_Acct_Balance */
public static String Table_Name="Actual_Acct_Balance";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_Actual_Acct_Balance[").Append(Get_ID()).Append("]");return sb.ToString();}
/** VAF_OrgTrx_ID VAF_Control_Ref_ID=276 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=276;/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID){if (VAF_OrgTrx_ID <= 0) Set_Value ("VAF_OrgTrx_ID", null);else
Set_Value ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);}/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() {Object ii = Get_Value("VAF_OrgTrx_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Account_ID VAF_Control_Ref_ID=182 */
public static int ACCOUNT_ID_VAF_Control_Ref_ID=182;/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID){if (Account_ID < 1) throw new ArgumentException ("Account_ID is mandatory.");Set_ValueNoCheck ("Account_ID", Account_ID);}/** Get Account.
@return Account used */
public int GetAccount_ID() {Object ii = Get_Value("Account_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr){if (AmtAcctCr == null) throw new ArgumentException ("AmtAcctCr is mandatory.");Set_Value ("AmtAcctCr", (Decimal?)AmtAcctCr);}/** Get Accounted Credit.
@return Accounted Credit Amount */
public Decimal GetAmtAcctCr() {Object bd =Get_Value("AmtAcctCr");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Accounted Debit.
@param AmtAcctDr Accounted Debit Amount */
public void SetAmtAcctDr (Decimal? AmtAcctDr){if (AmtAcctDr == null) throw new ArgumentException ("AmtAcctDr is mandatory.");Set_Value ("AmtAcctDr", (Decimal?)AmtAcctDr);}/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() {Object bd =Get_Value("AmtAcctDr");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID){if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() {Object ii = Get_Value("VAB_AccountBook_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetVAB_AccountBook_ID().ToString());}/** Set Activity.
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
/** VAB_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocFrom_ID_VAF_Control_Ref_ID=133;/** Set Location From.
@param VAB_LocFrom_ID Location that inventory was moved from */
public void SetVAB_LocFrom_ID (int VAB_LocFrom_ID){if (VAB_LocFrom_ID <= 0) Set_Value ("VAB_LocFrom_ID", null);else
Set_Value ("VAB_LocFrom_ID", VAB_LocFrom_ID);}/** Get Location From.
@return Location that inventory was moved from */
public int GetVAB_LocFrom_ID() {Object ii = Get_Value("VAB_LocFrom_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** VAB_LocTo_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocTo_ID_VAF_Control_Ref_ID=133;/** Set Location To.
@param VAB_LocTo_ID Location that inventory was moved to */
public void SetVAB_LocTo_ID (int VAB_LocTo_ID){if (VAB_LocTo_ID <= 0) Set_Value ("VAB_LocTo_ID", null);else
Set_Value ("VAB_LocTo_ID", VAB_LocTo_ID);}/** Get Location To.
@return Location that inventory was moved to */
public int GetVAB_LocTo_ID() {Object ii = Get_Value("VAB_LocTo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Project Phase.
@param VAB_ProjectStage_ID Phase of a Project */
public void SetVAB_ProjectStage_ID (int VAB_ProjectStage_ID){if (VAB_ProjectStage_ID <= 0) Set_ValueNoCheck ("VAB_ProjectStage_ID", null);else
Set_ValueNoCheck ("VAB_ProjectStage_ID", VAB_ProjectStage_ID);}/** Get Project Phase.
@return Phase of a Project */
public int GetVAB_ProjectStage_ID() {Object ii = Get_Value("VAB_ProjectStage_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Project Task.
@param VAB_ProjectJob_ID Actual Plan Task in a Phase */
public void SetVAB_ProjectJob_ID (int VAB_ProjectJob_ID){if (VAB_ProjectJob_ID <= 0) Set_ValueNoCheck ("VAB_ProjectJob_ID", null);else
Set_ValueNoCheck ("VAB_ProjectJob_ID", VAB_ProjectJob_ID);}/** Get Project Task.
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
public void SetVAB_SubAcct_ID (int VAB_SubAcct_ID){if (VAB_SubAcct_ID <= 0) Set_ValueNoCheck ("VAB_SubAcct_ID", null);else
Set_ValueNoCheck ("VAB_SubAcct_ID", VAB_SubAcct_ID);}/** Get Sub Account.
@return Sub account for Element Value */
public int GetVAB_SubAcct_ID() {Object ii = Get_Value("VAB_SubAcct_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct){if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");Set_Value ("DateAcct", (DateTime?)DateAcct);}/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() {return (DateTime?)Get_Value("DateAcct");}/** Set Element_U1.
@param Element_U1 Element_U1 */
public void SetElement_U1 (Boolean Element_U1){throw new ArgumentException ("Element_U1 Is virtual column");}/** Get Element_U1.
@return Element_U1 */
public Boolean IsElement_U1() {Object oo = Get_Value("Element_U1");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Element_U2.
@param Element_U2 Element_U2 */
public void SetElement_U2 (Boolean Element_U2){throw new ArgumentException ("Element_U2 Is virtual column");}/** Get Element_U2.
@return Element_U2 */
public Boolean IsElement_U2() {Object oo = Get_Value("Element_U2");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Element_X1.
@param Element_X1 Element_X1 */
public void SetElement_X1 (Boolean Element_X1){throw new ArgumentException ("Element_X1 Is virtual column");}/** Get Element_X1.
@return Element_X1 */
public Boolean IsElement_X1() {Object oo = Get_Value("Element_X1");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Element_X2.
@param Element_X2 Element_X2 */
public void SetElement_X2 (Boolean Element_X2){throw new ArgumentException ("Element_X2 Is virtual column");}/** Get Element_X2.
@return Element_X2 */
public Boolean IsElement_X2() {Object oo = Get_Value("Element_X2");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set ACTUAL_ACCUMULATION_ID.
@param ACTUAL_ACCUMULATION_ID ACTUAL_ACCUMULATION_ID */
public void SetACTUAL_ACCUMULATION_ID (int ACTUAL_ACCUMULATION_ID){if (ACTUAL_ACCUMULATION_ID <= 0) Set_Value ("ACTUAL_ACCUMULATION_ID", null);else
Set_Value ("ACTUAL_ACCUMULATION_ID", ACTUAL_ACCUMULATION_ID);}/** Get ACTUAL_ACCUMULATION_ID.
@return ACTUAL_ACCUMULATION_ID */
public int GetACTUAL_ACCUMULATION_ID() {Object ii = Get_Value("ACTUAL_ACCUMULATION_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Budget.
@param VAGL_Budget_ID General Ledger Budget */
public void SetVAGL_Budget_ID (int VAGL_Budget_ID){if (VAGL_Budget_ID <= 0) Set_Value ("VAGL_Budget_ID", null);else
Set_Value ("VAGL_Budget_ID", VAGL_Budget_ID);}/** Get Budget.
@return General Ledger Budget */
public int GetVAGL_Budget_ID() {Object ii = Get_Value("VAGL_Budget_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID){if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);else
Set_Value ("VAM_Product_ID", VAM_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() {Object ii = Get_Value("VAM_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** PostingType VAF_Control_Ref_ID=125 */
public static int POSTINGTYPE_VAF_Control_Ref_ID=125;/** Actual = A */
public static String POSTINGTYPE_Actual = "A";/** Budget = B */
public static String POSTINGTYPE_Budget = "B";/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";/** Virtual = V */
public static String POSTINGTYPE_Virtual = "V";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test){return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S") || test.Equals("V");}/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType){if (PostingType == null) throw new ArgumentException ("PostingType is mandatory");if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S - V");if (PostingType.Length > 1){log.Warning("Length > 1 - truncated");PostingType = PostingType.Substring(0,1);}Set_Value ("PostingType", PostingType);}/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() {return (String)Get_Value("PostingType");}/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty){if (Qty == null) throw new ArgumentException ("Qty is mandatory.");Set_Value ("Qty", (Decimal?)Qty);}/** Get Quantity.
@return Quantity */
public Decimal GetQty() {Object bd =Get_Value("Qty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}
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