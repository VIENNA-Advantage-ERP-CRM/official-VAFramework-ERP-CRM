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
/** Generated Model for I_GLJournal
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_GLJournal : PO
{
public X_I_GLJournal (Context ctx, int I_GLJournal_ID, Trx trxName) : base (ctx, I_GLJournal_ID, trxName)
{
/** if (I_GLJournal_ID == 0)
{
SetI_GLJournal_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_GLJournal (Ctx ctx, int I_GLJournal_ID, Trx trxName) : base (ctx, I_GLJournal_ID, trxName)
{
/** if (I_GLJournal_ID == 0)
{
SetI_GLJournal_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_GLJournal (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_GLJournal (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_GLJournal (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_GLJournal()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377061L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060272L;
/** AD_Table_ID=599 */
public static int Table_ID;
 // =599;

/** TableName=I_GLJournal */
public static String Table_Name="I_GLJournal";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_I_GLJournal[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgDoc_ID AD_Reference_ID=130 */
public static int AD_ORGDOC_ID_AD_Reference_ID=130;
/** Set Document Org.
@param AD_OrgDoc_ID Document Organization (independent from account organization) */
public void SetAD_OrgDoc_ID (int AD_OrgDoc_ID)
{
if (AD_OrgDoc_ID <= 0) Set_Value ("AD_OrgDoc_ID", null);
else
Set_Value ("AD_OrgDoc_ID", AD_OrgDoc_ID);
}
/** Get Document Org.
@return Document Organization (independent from account organization) */
public int GetAD_OrgDoc_ID() 
{
Object ii = Get_Value("AD_OrgDoc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_OrgTrx_ID AD_Reference_ID=130 */
public static int AD_ORGTRX_ID_AD_Reference_ID=130;
/** Set Trx Organization.
@param AD_OrgTrx_ID Performing or initiating organization */
public void SetAD_OrgTrx_ID (int AD_OrgTrx_ID)
{
if (AD_OrgTrx_ID <= 0) Set_Value ("AD_OrgTrx_ID", null);
else
Set_Value ("AD_OrgTrx_ID", AD_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetAD_OrgTrx_ID() 
{
Object ii = Get_Value("AD_OrgTrx_ID");
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
Set_Value ("AccountValue", AccountValue);
}
/** Get Account Key.
@return Key of Account Element */
public String GetAccountValue() 
{
return (String)Get_Value("AccountValue");
}

/** Account_ID AD_Reference_ID=132 */
public static int ACCOUNT_ID_AD_Reference_ID=132;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID <= 0) Set_Value ("Account_ID", null);
else
Set_Value ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Schema Name.
@param AcctSchemaName Name of the Accounting Schema */
public void SetAcctSchemaName (String AcctSchemaName)
{
if (AcctSchemaName != null && AcctSchemaName.Length > 60)
{
log.Warning("Length > 60 - truncated");
AcctSchemaName = AcctSchemaName.Substring(0,60);
}
Set_Value ("AcctSchemaName", AcctSchemaName);
}
/** Get Account Schema Name.
@return Name of the Accounting Schema */
public String GetAcctSchemaName() 
{
return (String)Get_Value("AcctSchemaName");
}
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
Set_Value ("AmtAcctCr", (Decimal?)AmtAcctCr);
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
Set_Value ("AmtAcctDr", (Decimal?)AmtAcctDr);
}
/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() 
{
Object bd =Get_Value("AmtAcctDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Credit.
@param AmtSourceCr Source Credit Amount */
public void SetAmtSourceCr (Decimal? AmtSourceCr)
{
Set_Value ("AmtSourceCr", (Decimal?)AmtSourceCr);
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
Set_Value ("AmtSourceDr", (Decimal?)AmtSourceDr);
}
/** Get Source Debit.
@return Source Debit Amount */
public Decimal GetAmtSourceDr() 
{
Object bd =Get_Value("AmtSourceDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Business Partner Key.
@param BPartnerValue Key of the Business Partner */
public void SetBPartnerValue (String BPartnerValue)
{
if (BPartnerValue != null && BPartnerValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
BPartnerValue = BPartnerValue.Substring(0,40);
}
Set_Value ("BPartnerValue", BPartnerValue);
}
/** Get Business Partner Key.
@return Key of the Business Partner */
public String GetBPartnerValue() 
{
return (String)Get_Value("BPartnerValue");
}
/** Set Batch Description.
@param BatchDescription Description of the Batch */
public void SetBatchDescription (String BatchDescription)
{
if (BatchDescription != null && BatchDescription.Length > 255)
{
log.Warning("Length > 255 - truncated");
BatchDescription = BatchDescription.Substring(0,255);
}
Set_Value ("BatchDescription", BatchDescription);
}
/** Get Batch Description.
@return Description of the Batch */
public String GetBatchDescription() 
{
return (String)Get_Value("BatchDescription");
}
/** Set Batch Document No.
@param BatchDocumentNo Document Number of the Batch */
public void SetBatchDocumentNo (String BatchDocumentNo)
{
if (BatchDocumentNo != null && BatchDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
BatchDocumentNo = BatchDocumentNo.Substring(0,30);
}
Set_Value ("BatchDocumentNo", BatchDocumentNo);
}
/** Get Batch Document No.
@return Document Number of the Batch */
public String GetBatchDocumentNo() 
{
return (String)Get_Value("BatchDocumentNo");
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID <= 0) Set_Value ("C_AcctSchema_ID", null);
else
Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID)
{
if (C_ConversionType_ID <= 0) Set_Value ("C_ConversionType_ID", null);
else
Set_Value ("C_ConversionType_ID", C_ConversionType_ID);
}
/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetC_ConversionType_ID() 
{
Object ii = Get_Value("C_ConversionType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);
else
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_LocFrom_ID AD_Reference_ID=133 */
public static int C_LOCFROM_ID_AD_Reference_ID=133;
/** Set Location From.
@param C_LocFrom_ID Location that inventory was moved from */
public void SetC_LocFrom_ID (int C_LocFrom_ID)
{
if (C_LocFrom_ID <= 0) Set_Value ("C_LocFrom_ID", null);
else
Set_Value ("C_LocFrom_ID", C_LocFrom_ID);
}
/** Get Location From.
@return Location that inventory was moved from */
public int GetC_LocFrom_ID() 
{
Object ii = Get_Value("C_LocFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_LocTo_ID AD_Reference_ID=133 */
public static int C_LOCTO_ID_AD_Reference_ID=133;
/** Set Location To.
@param C_LocTo_ID Location that inventory was moved to */
public void SetC_LocTo_ID (int C_LocTo_ID)
{
if (C_LocTo_ID <= 0) Set_Value ("C_LocTo_ID", null);
else
Set_Value ("C_LocTo_ID", C_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetC_LocTo_ID() 
{
Object ii = Get_Value("C_LocTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Period.
@param C_Period_ID Period of the Calendar */
public void SetC_Period_ID (int C_Period_ID)
{
if (C_Period_ID <= 0) Set_Value ("C_Period_ID", null);
else
Set_Value ("C_Period_ID", C_Period_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetC_Period_ID() 
{
Object ii = Get_Value("C_Period_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID)
{
if (C_SalesRegion_ID <= 0) Set_Value ("C_SalesRegion_ID", null);
else
Set_Value ("C_SalesRegion_ID", C_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() 
{
Object ii = Get_Value("C_SalesRegion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID <= 0) Set_Value ("C_UOM_ID", null);
else
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param C_ValidCombination_ID Valid Account Combination */
public void SetC_ValidCombination_ID (int C_ValidCombination_ID)
{
if (C_ValidCombination_ID <= 0) Set_Value ("C_ValidCombination_ID", null);
else
Set_Value ("C_ValidCombination_ID", C_ValidCombination_ID);
}
/** Get Combination.
@return Valid Account Combination */
public int GetC_ValidCombination_ID() 
{
Object ii = Get_Value("C_ValidCombination_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Category Name.
@param CategoryName Name of the Category */
public void SetCategoryName (String CategoryName)
{
if (CategoryName != null && CategoryName.Length > 60)
{
log.Warning("Length > 60 - truncated");
CategoryName = CategoryName.Substring(0,60);
}
Set_Value ("CategoryName", CategoryName);
}
/** Get Category Name.
@return Name of the Category */
public String GetCategoryName() 
{
return (String)Get_Value("CategoryName");
}
/** Set Tenant Key.
@param ClientValue Key of the Tenant */
public void SetClientValue (String ClientValue)
{
if (ClientValue != null && ClientValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ClientValue = ClientValue.Substring(0,40);
}
Set_Value ("ClientValue", ClientValue);
}
/** Get Tenant Key.
@return Key of the Tenant */
public String GetClientValue() 
{
return (String)Get_Value("ClientValue");
}
/** Set Currency Type Key.
@param ConversionTypeValue Key value for the Currency Conversion Rate Type */
public void SetConversionTypeValue (String ConversionTypeValue)
{
if (ConversionTypeValue != null && ConversionTypeValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ConversionTypeValue = ConversionTypeValue.Substring(0,40);
}
Set_Value ("ConversionTypeValue", ConversionTypeValue);
}
/** Get Currency Type Key.
@return Key value for the Currency Conversion Rate Type */
public String GetConversionTypeValue() 
{
return (String)Get_Value("ConversionTypeValue");
}
/** Set Rate.
@param CurrencyRate Currency Conversion Rate */
public void SetCurrencyRate (Decimal? CurrencyRate)
{
Set_Value ("CurrencyRate", (Decimal?)CurrencyRate);
}
/** Get Rate.
@return Currency Conversion Rate */
public Decimal GetCurrencyRate() 
{
Object bd =Get_Value("CurrencyRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
Set_Value ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
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
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Document Type Name.
@param DocTypeName Name of the Document Type */
public void SetDocTypeName (String DocTypeName)
{
if (DocTypeName != null && DocTypeName.Length > 60)
{
log.Warning("Length > 60 - truncated");
DocTypeName = DocTypeName.Substring(0,60);
}
Set_Value ("DocTypeName", DocTypeName);
}
/** Get Document Type Name.
@return Name of the Document Type */
public String GetDocTypeName() 
{
return (String)Get_Value("DocTypeName");
}
/** Set Budget.
@param GL_Budget_ID General Ledger Budget */
public void SetGL_Budget_ID (int GL_Budget_ID)
{
if (GL_Budget_ID <= 0) Set_Value ("GL_Budget_ID", null);
else
Set_Value ("GL_Budget_ID", GL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetGL_Budget_ID() 
{
Object ii = Get_Value("GL_Budget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set GL Category.
@param GL_Category_ID General Ledger Category */
public void SetGL_Category_ID (int GL_Category_ID)
{
if (GL_Category_ID <= 0) Set_Value ("GL_Category_ID", null);
else
Set_Value ("GL_Category_ID", GL_Category_ID);
}
/** Get GL Category.
@return General Ledger Category */
public int GetGL_Category_ID() 
{
Object ii = Get_Value("GL_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Journal Batch.
@param GL_JournalBatch_ID General Ledger Journal Batch */
public void SetGL_JournalBatch_ID (int GL_JournalBatch_ID)
{
if (GL_JournalBatch_ID <= 0) Set_Value ("GL_JournalBatch_ID", null);
else
Set_Value ("GL_JournalBatch_ID", GL_JournalBatch_ID);
}
/** Get Journal Batch.
@return General Ledger Journal Batch */
public int GetGL_JournalBatch_ID() 
{
Object ii = Get_Value("GL_JournalBatch_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Journal Line.
@param GL_JournalLine_ID General Ledger Journal Line */
public void SetGL_JournalLine_ID (int GL_JournalLine_ID)
{
if (GL_JournalLine_ID <= 0) Set_Value ("GL_JournalLine_ID", null);
else
Set_Value ("GL_JournalLine_ID", GL_JournalLine_ID);
}
/** Get Journal Line.
@return General Ledger Journal Line */
public int GetGL_JournalLine_ID() 
{
Object ii = Get_Value("GL_JournalLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
public void SetGL_Journal_ID (int GL_Journal_ID)
{
if (GL_Journal_ID <= 0) Set_Value ("GL_Journal_ID", null);
else
Set_Value ("GL_Journal_ID", GL_Journal_ID);
}
/** Get Journal.
@return General Ledger Journal */
public int GetGL_Journal_ID() 
{
Object ii = Get_Value("GL_Journal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set ISO Currency Code.
@param ISO_Code Three letter ISO 4217 Code of the Currency */
public void SetISO_Code (String ISO_Code)
{
if (ISO_Code != null && ISO_Code.Length > 3)
{
log.Warning("Length > 3 - truncated");
ISO_Code = ISO_Code.Substring(0,3);
}
Set_Value ("ISO_Code", ISO_Code);
}
/** Get ISO Currency Code.
@return Three letter ISO 4217 Code of the Currency */
public String GetISO_Code() 
{
return (String)Get_Value("ISO_Code");
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}
/** Set Import GL Journal.
@param I_GLJournal_ID Import General Ledger Journal */
public void SetI_GLJournal_ID (int I_GLJournal_ID)
{
if (I_GLJournal_ID < 1) throw new ArgumentException ("I_GLJournal_ID is mandatory.");
Set_ValueNoCheck ("I_GLJournal_ID", I_GLJournal_ID);
}
/** Get Import GL Journal.
@return Import General Ledger Journal */
public int GetI_GLJournal_ID() 
{
Object ii = Get_Value("I_GLJournal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetI_GLJournal_ID().ToString());
}

/** I_IsImported AD_Reference_ID=420 */
public static int I_ISIMPORTED_AD_Reference_ID=420;
/** Error = E */
public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid (String test)
{
return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (String I_IsImported)
{
if (I_IsImported == null) throw new ArgumentException ("I_IsImported is mandatory");
if (!IsI_IsImportedValid(I_IsImported))
throw new ArgumentException ("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
if (I_IsImported.Length > 1)
{
log.Warning("Length > 1 - truncated");
I_IsImported = I_IsImported.Substring(0,1);
}
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public String GetI_IsImported() 
{
return (String)Get_Value("I_IsImported");
}
/** Set Create New Batch.
@param IsCreateNewBatch If selected a new batch is created */
public void SetIsCreateNewBatch (Boolean IsCreateNewBatch)
{
Set_Value ("IsCreateNewBatch", IsCreateNewBatch);
}
/** Get Create New Batch.
@return If selected a new batch is created */
public Boolean IsCreateNewBatch() 
{
Object oo = Get_Value("IsCreateNewBatch");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Create New Journal.
@param IsCreateNewJournal If selected a new journal within the batch is created */
public void SetIsCreateNewJournal (Boolean IsCreateNewJournal)
{
Set_Value ("IsCreateNewJournal", IsCreateNewJournal);
}
/** Get Create New Journal.
@return If selected a new journal within the batch is created */
public Boolean IsCreateNewJournal() 
{
Object oo = Get_Value("IsCreateNewJournal");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Journal Document No.
@param JournalDocumentNo Document number of the Journal */
public void SetJournalDocumentNo (String JournalDocumentNo)
{
if (JournalDocumentNo != null && JournalDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
JournalDocumentNo = JournalDocumentNo.Substring(0,30);
}
Set_Value ("JournalDocumentNo", JournalDocumentNo);
}
/** Get Journal Document No.
@return Document number of the Journal */
public String GetJournalDocumentNo() 
{
return (String)Get_Value("JournalDocumentNo");
}
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Trx Org Key.
@param OrgTrxValue Key of the Transaction Organization */
public void SetOrgTrxValue (String OrgTrxValue)
{
if (OrgTrxValue != null && OrgTrxValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
OrgTrxValue = OrgTrxValue.Substring(0,40);
}
Set_Value ("OrgTrxValue", OrgTrxValue);
}
/** Get Trx Org Key.
@return Key of the Transaction Organization */
public String GetOrgTrxValue() 
{
return (String)Get_Value("OrgTrxValue");
}
/** Set Organization Key.
@param OrgValue Key of the Organization */
public void SetOrgValue (String OrgValue)
{
if (OrgValue != null && OrgValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
OrgValue = OrgValue.Substring(0,40);
}
Set_Value ("OrgValue", OrgValue);
}
/** Get Organization Key.
@return Key of the Organization */
public String GetOrgValue() 
{
return (String)Get_Value("OrgValue");
}

/** PostingType AD_Reference_ID=125 */
public static int POSTINGTYPE_AD_Reference_ID=125;
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
return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType != null && PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_Value ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product Key.
@param ProductValue Key of the Product */
public void SetProductValue (String ProductValue)
{
if (ProductValue != null && ProductValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProductValue = ProductValue.Substring(0,40);
}
Set_Value ("ProductValue", ProductValue);
}
/** Get Product Key.
@return Key of the Product */
public String GetProductValue() 
{
return (String)Get_Value("ProductValue");
}
/** Set Project Key.
@param ProjectValue Key of the Project */
public void SetProjectValue (String ProjectValue)
{
if (ProjectValue != null && ProjectValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProjectValue = ProjectValue.Substring(0,40);
}
Set_Value ("ProjectValue", ProjectValue);
}
/** Get Project Key.
@return Key of the Project */
public String GetProjectValue() 
{
return (String)Get_Value("ProjectValue");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set SKU.
@param SKU Stock Keeping Unit */
public void SetSKU (String SKU)
{
if (SKU != null && SKU.Length > 30)
{
log.Warning("Length > 30 - truncated");
SKU = SKU.Substring(0,30);
}
Set_Value ("SKU", SKU);
}
/** Get SKU.
@return Stock Keeping Unit */
public String GetSKU() 
{
return (String)Get_Value("SKU");
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC (String UPC)
{
if (UPC != null && UPC.Length > 30)
{
log.Warning("Length > 30 - truncated");
UPC = UPC.Substring(0,30);
}
Set_Value ("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC() 
{
return (String)Get_Value("UPC");
}

/** User1_ID AD_Reference_ID=134 */
public static int USER1_ID_AD_Reference_ID=134;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_Value ("User1_ID", null);
else
Set_Value ("User1_ID", User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() 
{
Object ii = Get_Value("User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User2_ID AD_Reference_ID=137 */
public static int USER2_ID_AD_Reference_ID=137;
/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID)
{
if (User2_ID <= 0) Set_Value ("User2_ID", null);
else
Set_Value ("User2_ID", User2_ID);
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
