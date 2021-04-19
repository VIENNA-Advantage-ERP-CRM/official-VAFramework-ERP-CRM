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
/** Generated Model for VAT_TrailBalance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_TrailBalance : PO
{
public X_VAT_TrailBalance (Context ctx, int VAT_TrailBalance_ID, Trx trxName) : base (ctx, VAT_TrailBalance_ID, trxName)
{
/** if (VAT_TrailBalance_ID == 0)
{
SetVAF_JInstance_ID (0);
SetAmtAcctBalance (0.0);
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetVAB_AccountBook_ID (0);
SetDateAcct (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetPostingType (null);
}
 */
}
public X_VAT_TrailBalance (Ctx ctx, int VAT_TrailBalance_ID, Trx trxName) : base (ctx, VAT_TrailBalance_ID, trxName)
{
/** if (VAT_TrailBalance_ID == 0)
{
SetVAF_JInstance_ID (0);
SetAmtAcctBalance (0.0);
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetVAB_AccountBook_ID (0);
SetDateAcct (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetPostingType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_TrailBalance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_TrailBalance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_TrailBalance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_TrailBalance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384616L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067827L;
/** VAF_TableView_ID=753 */
public static int Table_ID;
 // =753;

/** TableName=VAT_TrailBalance */
public static String Table_Name="VAT_TrailBalance";

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
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAT_TrailBalance[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_OrgTrx_ID VAF_Control_Ref_ID=130 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=130;
/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID)
{
if (VAF_OrgTrx_ID <= 0) Set_ValueNoCheck ("VAF_OrgTrx_ID", null);
else
Set_ValueNoCheck ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() 
{
Object ii = Get_Value("VAF_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process Instance.
@param VAF_JInstance_ID Instance of the process */
public void SetVAF_JInstance_ID (int VAF_JInstance_ID)
{
if (VAF_JInstance_ID < 1) throw new ArgumentException ("VAF_JInstance_ID is mandatory.");
Set_ValueNoCheck ("VAF_JInstance_ID", VAF_JInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetVAF_JInstance_ID() 
{
Object ii = Get_Value("VAF_JInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_ValueNoCheck ("VAF_TableView_ID", null);
else
Set_ValueNoCheck ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int VAA_Asset_ID)
{
if (VAA_Asset_ID <= 0) Set_ValueNoCheck ("VAA_Asset_ID", null);
else
Set_ValueNoCheck ("VAA_Asset_ID", VAA_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("VAA_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Key.
@param AccountValue Key of Account Element */
public void SetAccountValue (String AccountValue)
{
if (AccountValue != null && AccountValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
AccountValue = AccountValue.Substring(0,40);
}
Set_ValueNoCheck ("AccountValue", AccountValue);
}
/** Get Account Key.
@return Key of Account Element */
public String GetAccountValue() 
{
return (String)Get_Value("AccountValue");
}

/** Account_ID VAF_Control_Ref_ID=132 */
public static int ACCOUNT_ID_VAF_Control_Ref_ID=132;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID <= 0) Set_ValueNoCheck ("Account_ID", null);
else
Set_ValueNoCheck ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounted Balance.
@param AmtAcctBalance Accounted Balance Amount */
public void SetAmtAcctBalance (Decimal? AmtAcctBalance)
{
if (AmtAcctBalance == null) throw new ArgumentException ("AmtAcctBalance is mandatory.");
Set_ValueNoCheck ("AmtAcctBalance", (Decimal?)AmtAcctBalance);
}
/** Get Accounted Balance.
@return Accounted Balance Amount */
public Decimal GetAmtAcctBalance() 
{
Object bd =Get_Value("AmtAcctBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
if (AmtAcctCr == null) throw new ArgumentException ("AmtAcctCr is mandatory.");
Set_ValueNoCheck ("AmtAcctCr", (Decimal?)AmtAcctCr);
}
/** Get Accounted Credit.
@return Accounted Credit Amount */
public Decimal GetAmtAcctCr() 
{
Object bd =Get_Value("AmtAcctCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounted Debit.
@param AmtAcctDr Accounted Debit Amount */
public void SetAmtAcctDr (Decimal? AmtAcctDr)
{
if (AmtAcctDr == null) throw new ArgumentException ("AmtAcctDr is mandatory.");
Set_ValueNoCheck ("AmtAcctDr", (Decimal?)AmtAcctDr);
}
/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() 
{
Object bd =Get_Value("AmtAcctDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Balance.
@param AmtSourceBalance Source Balance Amount */
public void SetAmtSourceBalance (Decimal? AmtSourceBalance)
{
Set_ValueNoCheck ("AmtSourceBalance", (Decimal?)AmtSourceBalance);
}
/** Get Source Balance.
@return Source Balance Amount */
public Decimal GetAmtSourceBalance() 
{
Object bd =Get_Value("AmtSourceBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Credit.
@param AmtSourceCr Source Credit Amount */
public void SetAmtSourceCr (Decimal? AmtSourceCr)
{
Set_ValueNoCheck ("AmtSourceCr", (Decimal?)AmtSourceCr);
}
/** Get Source Credit.
@return Source Credit Amount */
public Decimal GetAmtSourceCr() 
{
Object bd =Get_Value("AmtSourceCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Debit.
@param AmtSourceDr Source Debit Amount */
public void SetAmtSourceDr (Decimal? AmtSourceDr)
{
Set_ValueNoCheck ("AmtSourceDr", (Decimal?)AmtSourceDr);
}
/** Get Source Debit.
@return Source Debit Amount */
public Decimal GetAmtSourceDr() 
{
Object bd =Get_Value("AmtSourceDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID)
{
if (VAB_BillingCode_ID <= 0) Set_ValueNoCheck ("VAB_BillingCode_ID", null);
else
Set_ValueNoCheck ("VAB_BillingCode_ID", VAB_BillingCode_ID);
}
/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() 
{
Object ii = Get_Value("VAB_BillingCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_ValueNoCheck ("VAB_BusinessPartner_ID", null);
else
Set_ValueNoCheck ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID <= 0) Set_ValueNoCheck ("VAB_Promotion_ID", null);
else
Set_ValueNoCheck ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID <= 0) Set_ValueNoCheck ("VAB_Currency_ID", null);
else
Set_ValueNoCheck ("VAB_Currency_ID", VAB_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() 
{
Object ii = Get_Value("VAB_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_LocFrom_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocFrom_ID_VAF_Control_Ref_ID=133;
/** Set Location From.
@param VAB_LocFrom_ID Location that inventory was moved from */
public void SetVAB_LocFrom_ID (int VAB_LocFrom_ID)
{
if (VAB_LocFrom_ID <= 0) Set_ValueNoCheck ("VAB_LocFrom_ID", null);
else
Set_ValueNoCheck ("VAB_LocFrom_ID", VAB_LocFrom_ID);
}
/** Get Location From.
@return Location that inventory was moved from */
public int GetVAB_LocFrom_ID() 
{
Object ii = Get_Value("VAB_LocFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_LocTo_ID VAF_Control_Ref_ID=133 */
public static int VAB_LocTo_ID_VAF_Control_Ref_ID=133;
/** Set Location To.
@param VAB_LocTo_ID Location that inventory was moved to */
public void SetVAB_LocTo_ID (int VAB_LocTo_ID)
{
if (VAB_LocTo_ID <= 0) Set_ValueNoCheck ("VAB_LocTo_ID", null);
else
Set_ValueNoCheck ("VAB_LocTo_ID", VAB_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetVAB_LocTo_ID() 
{
Object ii = Get_Value("VAB_LocTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Period.
@param VAB_YearPeriod_ID Period of the Calendar */
public void SetVAB_YearPeriod_ID (int VAB_YearPeriod_ID)
{
if (VAB_YearPeriod_ID <= 0) Set_ValueNoCheck ("VAB_YearPeriod_ID", null);
else
Set_ValueNoCheck ("VAB_YearPeriod_ID", VAB_YearPeriod_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetVAB_YearPeriod_ID() 
{
Object ii = Get_Value("VAB_YearPeriod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID <= 0) Set_ValueNoCheck ("VAB_Project_ID", null);
else
Set_ValueNoCheck ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID)
{
if (VAB_SalesRegionState_ID <= 0) Set_ValueNoCheck ("VAB_SalesRegionState_ID", null);
else
Set_ValueNoCheck ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() 
{
Object ii = Get_Value("VAB_SalesRegionState_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax.
@param VAB_TaxRate_ID Tax identifier */
public void SetVAB_TaxRate_ID (int VAB_TaxRate_ID)
{
if (VAB_TaxRate_ID <= 0) Set_ValueNoCheck ("VAB_TaxRate_ID", null);
else
Set_ValueNoCheck ("VAB_TaxRate_ID", VAB_TaxRate_ID);
}
/** Get Tax.
@return Tax identifier */
public int GetVAB_TaxRate_ID() 
{
Object ii = Get_Value("VAB_TaxRate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param VAB_UOM_ID Unit of Measure */
public void SetVAB_UOM_ID (int VAB_UOM_ID)
{
if (VAB_UOM_ID <= 0) Set_ValueNoCheck ("VAB_UOM_ID", null);
else
Set_ValueNoCheck ("VAB_UOM_ID", VAB_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetVAB_UOM_ID() 
{
Object ii = Get_Value("VAB_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");
Set_ValueNoCheck ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
}
/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx)
{
Set_ValueNoCheck ("DateTrx", (DateTime?)DateTrx);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() 
{
return (DateTime?)Get_Value("DateTrx");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_ValueNoCheck ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Accounting Fact.
@param Actual_Acct_Detail_ID Accounting Fact */
public void SetActual_Acct_Detail_ID (int Actual_Acct_Detail_ID)
{
if (Actual_Acct_Detail_ID < 1) throw new ArgumentException ("Actual_Acct_Detail_ID is mandatory.");
Set_ValueNoCheck ("Actual_Acct_Detail_ID", Actual_Acct_Detail_ID);
}
/** Get Accounting Fact.
@return Accounting Fact */
public int GetActual_Acct_Detail_ID() 
{
Object ii = Get_Value("Actual_Acct_Detail_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Budget.
@param VAGL_Budget_ID General Ledger Budget */
public void SetVAGL_Budget_ID (int VAGL_Budget_ID)
{
if (VAGL_Budget_ID <= 0) Set_ValueNoCheck ("VAGL_Budget_ID", null);
else
Set_ValueNoCheck ("VAGL_Budget_ID", VAGL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetVAGL_Budget_ID() 
{
Object ii = Get_Value("VAGL_Budget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set GL Category.
@param VAGL_Group_ID General Ledger Category */
public void SetVAGL_Group_ID (int VAGL_Group_ID)
{
if (VAGL_Group_ID <= 0) Set_ValueNoCheck ("VAGL_Group_ID", null);
else
Set_ValueNoCheck ("VAGL_Group_ID", VAGL_Group_ID);
}
/** Get GL Category.
@return General Ledger Category */
public int GetVAGL_Group_ID() 
{
Object ii = Get_Value("VAGL_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Line ID.
@param Line_ID Transaction line ID (internal) */
public void SetLine_ID (int Line_ID)
{
if (Line_ID <= 0) Set_ValueNoCheck ("Line_ID", null);
else
Set_ValueNoCheck ("Line_ID", Line_ID);
}
/** Get Line ID.
@return Transaction line ID (internal) */
public int GetLine_ID() 
{
Object ii = Get_Value("Line_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param VAM_Locator_ID Warehouse Locator */
public void SetVAM_Locator_ID (int VAM_Locator_ID)
{
if (VAM_Locator_ID <= 0) Set_ValueNoCheck ("VAM_Locator_ID", null);
else
Set_ValueNoCheck ("VAM_Locator_ID", VAM_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetVAM_Locator_ID() 
{
Object ii = Get_Value("VAM_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_ValueNoCheck ("VAM_Product_ID", null);
else
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PostingType VAF_Control_Ref_ID=125 */
public static int POSTINGTYPE_VAF_Control_Ref_ID=125;
/** Actual = A */
public static String POSTINGTYPE_Actual = "A";
/** Budget = B */
public static String POSTINGTYPE_Budget = "B";
/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";
/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";
/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (PostingType == null) throw new ArgumentException ("PostingType is mandatory");
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_ValueNoCheck ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_ValueNoCheck ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_ValueNoCheck ("Record_ID", null);
else
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User1_ID VAF_Control_Ref_ID=134 */
public static int USER1_ID_VAF_Control_Ref_ID=134;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_ValueNoCheck ("User1_ID", null);
else
Set_ValueNoCheck ("User1_ID", User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() 
{
Object ii = Get_Value("User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User2_ID VAF_Control_Ref_ID=137 */
public static int USER2_ID_VAF_Control_Ref_ID=137;
/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID)
{
if (User2_ID <= 0) Set_ValueNoCheck ("User2_ID", null);
else
Set_ValueNoCheck ("User2_ID", User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() 
{
Object ii = Get_Value("User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
