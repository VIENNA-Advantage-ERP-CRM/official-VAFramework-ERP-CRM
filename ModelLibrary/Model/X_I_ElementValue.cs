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
/** Generated Model for I_ElementValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_ElementValue : PO
{
public X_I_ElementValue (Context ctx, int I_ElementValue_ID, Trx trxName) : base (ctx, I_ElementValue_ID, trxName)
{
/** if (I_ElementValue_ID == 0)
{
SetI_ElementValue_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_ElementValue (Ctx ctx, int I_ElementValue_ID, Trx trxName) : base (ctx, I_ElementValue_ID, trxName)
{
/** if (I_ElementValue_ID == 0)
{
SetI_ElementValue_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ElementValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ElementValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ElementValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_ElementValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377014L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060225L;
/** AD_Table_ID=534 */
public static int Table_ID;
 // =534;

/** TableName=I_ElementValue */
public static String Table_Name="I_ElementValue";

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
StringBuilder sb = new StringBuilder ("X_I_ElementValue[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Column_ID AD_Reference_ID=272 */
public static int AD_COLUMN_ID_AD_Reference_ID=272;
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AccountSign AD_Reference_ID=118 */
public static int ACCOUNTSIGN_AD_Reference_ID=118;
/** Credit = C */
public static String ACCOUNTSIGN_Credit = "C";
/** Debit = D */
public static String ACCOUNTSIGN_Debit = "D";
/** Natural = N */
public static String ACCOUNTSIGN_Natural = "N";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccountSignValid (String test)
{
return test == null || test.Equals("C") || test.Equals("D") || test.Equals("N");
}
/** Set Account Sign.
@param AccountSign Indicates the Natural Sign of the Account as a Debit or Credit */
public void SetAccountSign (String AccountSign)
{
if (!IsAccountSignValid(AccountSign))
throw new ArgumentException ("AccountSign Invalid value - " + AccountSign + " - Reference_ID=118 - C - D - N");
if (AccountSign != null && AccountSign.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccountSign = AccountSign.Substring(0,1);
}
Set_Value ("AccountSign", AccountSign);
}
/** Get Account Sign.
@return Indicates the Natural Sign of the Account as a Debit or Credit */
public String GetAccountSign() 
{
return (String)Get_Value("AccountSign");
}

/** AccountType AD_Reference_ID=117 */
public static int ACCOUNTTYPE_AD_Reference_ID=117;
/** Asset = A */
public static String ACCOUNTTYPE_Asset = "A";
/** Expense = E */
public static String ACCOUNTTYPE_Expense = "E";
/** Liability = L */
public static String ACCOUNTTYPE_Liability = "L";
/** Memo = M */
public static String ACCOUNTTYPE_Memo = "M";
/** Owner's Equity = O */
public static String ACCOUNTTYPE_OwnerSEquity = "O";
/** Revenue = R */
public static String ACCOUNTTYPE_Revenue = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccountTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("E") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
}
/** Set Account Type.
@param AccountType Indicates the type of account */
public void SetAccountType (String AccountType)
{
if (!IsAccountTypeValid(AccountType))
throw new ArgumentException ("AccountType Invalid value - " + AccountType + " - Reference_ID=117 - A - E - L - M - O - R");
if (AccountType != null && AccountType.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccountType = AccountType.Substring(0,1);
}
Set_Value ("AccountType", AccountType);
}
/** Get Account Type.
@return Indicates the type of account */
public String GetAccountType() 
{
return (String)Get_Value("AccountType");
}
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);
else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Element.
@param C_Element_ID Accounting Element */
public void SetC_Element_ID (int C_Element_ID)
{
if (C_Element_ID <= 0) Set_Value ("C_Element_ID", null);
else
Set_Value ("C_Element_ID", C_Element_ID);
}
/** Get Element.
@return Accounting Element */
public int GetC_Element_ID() 
{
Object ii = Get_Value("C_Element_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Default Account.
@param Default_Account Name of the Default Account Column */
public void SetDefault_Account (String Default_Account)
{
if (Default_Account != null && Default_Account.Length > 30)
{
log.Warning("Length > 30 - truncated");
Default_Account = Default_Account.Substring(0,30);
}
Set_Value ("Default_Account", Default_Account);
}
/** Get Default Account.
@return Name of the Default Account Column */
public String GetDefault_Account() 
{
return (String)Get_Value("Default_Account");
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
/** Set Element Name.
@param ElementName Name of the Element */
public void SetElementName (String ElementName)
{
if (ElementName != null && ElementName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ElementName = ElementName.Substring(0,60);
}
Set_Value ("ElementName", ElementName);
}
/** Get Element Name.
@return Name of the Element */
public String GetElementName() 
{
return (String)Get_Value("ElementName");
}
/** Set Import Account.
@param I_ElementValue_ID Import Account Value */
public void SetI_ElementValue_ID (int I_ElementValue_ID)
{
if (I_ElementValue_ID < 1) throw new ArgumentException ("I_ElementValue_ID is mandatory.");
Set_ValueNoCheck ("I_ElementValue_ID", I_ElementValue_ID);
}
/** Get Import Account.
@return Import Account Value */
public int GetI_ElementValue_ID() 
{
Object ii = Get_Value("I_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Document Controlled.
@param IsDocControlled Control account - If an account is controlled by a document, you cannot post manually to it */
public void SetIsDocControlled (Boolean IsDocControlled)
{
Set_Value ("IsDocControlled", IsDocControlled);
}
/** Get Document Controlled.
@return Control account - If an account is controlled by a document, you cannot post manually to it */
public Boolean IsDocControlled() 
{
Object oo = Get_Value("IsDocControlled");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}

/** ParentElementValue_ID AD_Reference_ID=182 */
public static int PARENTELEMENTVALUE_ID_AD_Reference_ID=182;
/** Set Parent Account.
@param ParentElementValue_ID The parent (summary) account */
public void SetParentElementValue_ID (int ParentElementValue_ID)
{
if (ParentElementValue_ID <= 0) Set_Value ("ParentElementValue_ID", null);
else
Set_Value ("ParentElementValue_ID", ParentElementValue_ID);
}
/** Get Parent Account.
@return The parent (summary) account */
public int GetParentElementValue_ID() 
{
Object ii = Get_Value("ParentElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Parent Key.
@param ParentValue Key if the Parent */
public void SetParentValue (String ParentValue)
{
if (ParentValue != null && ParentValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ParentValue = ParentValue.Substring(0,40);
}
Set_Value ("ParentValue", ParentValue);
}
/** Get Parent Key.
@return Key if the Parent */
public String GetParentValue() 
{
return (String)Get_Value("ParentValue");
}
/** Set Post Actual.
@param PostActual Actual Values can be posted */
public void SetPostActual (Boolean PostActual)
{
Set_Value ("PostActual", PostActual);
}
/** Get Post Actual.
@return Actual Values can be posted */
public Boolean IsPostActual() 
{
Object oo = Get_Value("PostActual");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Post Budget.
@param PostBudget Budget values can be posted */
public void SetPostBudget (Boolean PostBudget)
{
Set_Value ("PostBudget", PostBudget);
}
/** Get Post Budget.
@return Budget values can be posted */
public Boolean IsPostBudget() 
{
Object oo = Get_Value("PostBudget");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Post Encumbrance.
@param PostEncumbrance Post commitments to this account */
public void SetPostEncumbrance (Boolean PostEncumbrance)
{
Set_Value ("PostEncumbrance", PostEncumbrance);
}
/** Get Post Encumbrance.
@return Post commitments to this account */
public Boolean IsPostEncumbrance() 
{
Object oo = Get_Value("PostEncumbrance");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Post Statistical.
@param PostStatistical Post statistical quantities to this account? */
public void SetPostStatistical (Boolean PostStatistical)
{
Set_Value ("PostStatistical", PostStatistical);
}
/** Get Post Statistical.
@return Post statistical quantities to this account? */
public Boolean IsPostStatistical() 
{
Object oo = Get_Value("PostStatistical");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetValue());
}
}

}
