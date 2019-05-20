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
/** Generated Model for C_BankStatementLoader
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankStatementLoader : PO
{
public X_C_BankStatementLoader (Context ctx, int C_BankStatementLoader_ID, Trx trxName) : base (ctx, C_BankStatementLoader_ID, trxName)
{
/** if (C_BankStatementLoader_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_BankStatementLoader_ID (0);
SetName (null);
}
 */
}
public X_C_BankStatementLoader (Ctx ctx, int C_BankStatementLoader_ID, Trx trxName) : base (ctx, C_BankStatementLoader_ID, trxName)
{
/** if (C_BankStatementLoader_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_BankStatementLoader_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLoader (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLoader (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLoader (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankStatementLoader()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370918L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054129L;
/** AD_Table_ID=640 */
public static int Table_ID;
 // =640;

/** TableName=C_BankStatementLoader */
public static String Table_Name="C_BankStatementLoader";

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
StringBuilder sb = new StringBuilder ("X_C_BankStatementLoader[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account No.
@param AccountNo Account Number */
public void SetAccountNo (String AccountNo)
{
if (AccountNo != null && AccountNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
AccountNo = AccountNo.Substring(0,20);
}
Set_Value ("AccountNo", AccountNo);
}
/** Get Account No.
@return Account Number */
public String GetAccountNo() 
{
return (String)Get_Value("AccountNo");
}
/** Set Branch ID.
@param BranchID Bank Branch ID */
public void SetBranchID (String BranchID)
{
if (BranchID != null && BranchID.Length > 20)
{
log.Warning("Length > 20 - truncated");
BranchID = BranchID.Substring(0,20);
}
Set_Value ("BranchID", BranchID);
}
/** Get Branch ID.
@return Bank Branch ID */
public String GetBranchID() 
{
return (String)Get_Value("BranchID");
}
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID < 1) throw new ArgumentException ("C_BankAccount_ID is mandatory.");
Set_ValueNoCheck ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Statement Loader.
@param C_BankStatementLoader_ID Definition of Bank Statement Loader (SWIFT, OFX) */
public void SetC_BankStatementLoader_ID (int C_BankStatementLoader_ID)
{
if (C_BankStatementLoader_ID < 1) throw new ArgumentException ("C_BankStatementLoader_ID is mandatory.");
Set_ValueNoCheck ("C_BankStatementLoader_ID", C_BankStatementLoader_ID);
}
/** Get Bank Statement Loader.
@return Definition of Bank Statement Loader (SWIFT, OFX) */
public int GetC_BankStatementLoader_ID() 
{
Object ii = Get_Value("C_BankStatementLoader_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date Format.
@param DateFormat Date format used in the imput format */
public void SetDateFormat (String DateFormat)
{
if (DateFormat != null && DateFormat.Length > 20)
{
log.Warning("Length > 20 - truncated");
DateFormat = DateFormat.Substring(0,20);
}
Set_Value ("DateFormat", DateFormat);
}
/** Get Date Format.
@return Date format used in the imput format */
public String GetDateFormat() 
{
return (String)Get_Value("DateFormat");
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_Value ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
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
/** Set File Name.
@param FileName Name of the local file or URL */
public void SetFileName (String FileName)
{
if (FileName != null && FileName.Length > 120)
{
log.Warning("Length > 120 - truncated");
FileName = FileName.Substring(0,120);
}
Set_Value ("FileName", FileName);
}
/** Get File Name.
@return Name of the local file or URL */
public String GetFileName() 
{
return (String)Get_Value("FileName");
}
/** Set Financial Institution ID.
@param FinancialInstitutionID The ID of the Financial Institution / Bank */
public void SetFinancialInstitutionID (String FinancialInstitutionID)
{
if (FinancialInstitutionID != null && FinancialInstitutionID.Length > 20)
{
log.Warning("Length > 20 - truncated");
FinancialInstitutionID = FinancialInstitutionID.Substring(0,20);
}
Set_Value ("FinancialInstitutionID", FinancialInstitutionID);
}
/** Get Financial Institution ID.
@return The ID of the Financial Institution / Bank */
public String GetFinancialInstitutionID() 
{
return (String)Get_Value("FinancialInstitutionID");
}
/** Set Host Address.
@param HostAddress Host Address URL or DNS */
public void SetHostAddress (String HostAddress)
{
if (HostAddress != null && HostAddress.Length > 60)
{
log.Warning("Length > 60 - truncated");
HostAddress = HostAddress.Substring(0,60);
}
Set_Value ("HostAddress", HostAddress);
}
/** Get Host Address.
@return Host Address URL or DNS */
public String GetHostAddress() 
{
return (String)Get_Value("HostAddress");
}
/** Set Host port.
@param HostPort Host Communication Port */
public void SetHostPort (int HostPort)
{
Set_Value ("HostPort", HostPort);
}
/** Get Host port.
@return Host Communication Port */
public int GetHostPort() 
{
Object ii = Get_Value("HostPort");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set PIN.
@param PIN Personal Identification Number */
public void SetPIN (String PIN)
{
if (PIN != null && PIN.Length > 20)
{
log.Warning("Length > 20 - truncated");
PIN = PIN.Substring(0,20);
}
Set_Value ("PIN", PIN);
}
/** Get PIN.
@return Personal Identification Number */
public String GetPIN() 
{
return (String)Get_Value("PIN");
}
/** Set Password.
@param Password Password of any length (case sensitive) */
public void SetPassword (String Password)
{
if (Password != null && Password.Length > 60)
{
log.Warning("Length > 60 - truncated");
Password = Password.Substring(0,60);
}
Set_Value ("Password", Password);
}
/** Get Password.
@return Password of any length (case sensitive) */
public String GetPassword() 
{
return (String)Get_Value("Password");
}
/** Set Proxy address.
@param ProxyAddress Address of your proxy server */
public void SetProxyAddress (String ProxyAddress)
{
if (ProxyAddress != null && ProxyAddress.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProxyAddress = ProxyAddress.Substring(0,60);
}
Set_Value ("ProxyAddress", ProxyAddress);
}
/** Get Proxy address.
@return Address of your proxy server */
public String GetProxyAddress() 
{
return (String)Get_Value("ProxyAddress");
}
/** Set Proxy logon.
@param ProxyLogon Logon of your proxy server */
public void SetProxyLogon (String ProxyLogon)
{
if (ProxyLogon != null && ProxyLogon.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProxyLogon = ProxyLogon.Substring(0,60);
}
Set_Value ("ProxyLogon", ProxyLogon);
}
/** Get Proxy logon.
@return Logon of your proxy server */
public String GetProxyLogon() 
{
return (String)Get_Value("ProxyLogon");
}
/** Set Proxy password.
@param ProxyPassword Password of your proxy server */
public void SetProxyPassword (String ProxyPassword)
{
if (ProxyPassword != null && ProxyPassword.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProxyPassword = ProxyPassword.Substring(0,60);
}
Set_Value ("ProxyPassword", ProxyPassword);
}
/** Get Proxy password.
@return Password of your proxy server */
public String GetProxyPassword() 
{
return (String)Get_Value("ProxyPassword");
}
/** Set Proxy port.
@param ProxyPort Port of your proxy server */
public void SetProxyPort (int ProxyPort)
{
Set_Value ("ProxyPort", ProxyPort);
}
/** Get Proxy port.
@return Port of your proxy server */
public int GetProxyPort() 
{
Object ii = Get_Value("ProxyPort");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Statement Loader Class.
@param StmtLoaderClass Class name of the bank statement loader */
public void SetStmtLoaderClass (String StmtLoaderClass)
{
if (StmtLoaderClass != null && StmtLoaderClass.Length > 60)
{
log.Warning("Length > 60 - truncated");
StmtLoaderClass = StmtLoaderClass.Substring(0,60);
}
Set_Value ("StmtLoaderClass", StmtLoaderClass);
}
/** Get Statement Loader Class.
@return Class name of the bank statement loader */
public String GetStmtLoaderClass() 
{
return (String)Get_Value("StmtLoaderClass");
}
/** Set User ID.
@param UserID User ID or account number */
public void SetUserID (String UserID)
{
if (UserID != null && UserID.Length > 60)
{
log.Warning("Length > 60 - truncated");
UserID = UserID.Substring(0,60);
}
Set_Value ("UserID", UserID);
}
/** Get User ID.
@return User ID or account number */
public String GetUserID() 
{
return (String)Get_Value("UserID");
}
}

}
