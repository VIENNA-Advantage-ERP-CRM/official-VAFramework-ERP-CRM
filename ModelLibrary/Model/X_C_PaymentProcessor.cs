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
/** Generated Model for C_PaymentProcessor
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PaymentProcessor : PO
{
public X_C_PaymentProcessor (Context ctx, int C_PaymentProcessor_ID, Trx trxName) : base (ctx, C_PaymentProcessor_ID, trxName)
{
/** if (C_PaymentProcessor_ID == 0)
{
SetAcceptAMEX (false);
SetAcceptATM (false);
SetAcceptCheck (false);
SetAcceptCorporate (false);
SetAcceptDiners (false);
SetAcceptDirectDebit (false);
SetAcceptDirectDeposit (false);
SetAcceptDiscover (false);
SetAcceptMC (false);
SetAcceptVisa (false);
SetC_BankAccount_ID (0);
SetC_PaymentProcessor_ID (0);
SetCommission (0.0);
SetCostPerTrx (0.0);
SetName (null);
SetRequireVV (false);
}
 */
}
public X_C_PaymentProcessor (Ctx ctx, int C_PaymentProcessor_ID, Trx trxName) : base (ctx, C_PaymentProcessor_ID, trxName)
{
/** if (C_PaymentProcessor_ID == 0)
{
SetAcceptAMEX (false);
SetAcceptATM (false);
SetAcceptCheck (false);
SetAcceptCorporate (false);
SetAcceptDiners (false);
SetAcceptDirectDebit (false);
SetAcceptDirectDeposit (false);
SetAcceptDiscover (false);
SetAcceptMC (false);
SetAcceptVisa (false);
SetC_BankAccount_ID (0);
SetC_PaymentProcessor_ID (0);
SetCommission (0.0);
SetCostPerTrx (0.0);
SetName (null);
SetRequireVV (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentProcessor (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentProcessor (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentProcessor (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PaymentProcessor()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373990L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057201L;
/** AD_Table_ID=398 */
public static int Table_ID;
 // =398;

/** TableName=C_PaymentProcessor */
public static String Table_Name="C_PaymentProcessor";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_PaymentProcessor[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Sequence_ID AD_Reference_ID=128 */
public static int AD_SEQUENCE_ID_AD_Reference_ID=128;
/** Set Sequence.
@param AD_Sequence_ID Document Sequence */
public void SetAD_Sequence_ID (int AD_Sequence_ID)
{
if (AD_Sequence_ID <= 0) Set_Value ("AD_Sequence_ID", null);
else
Set_Value ("AD_Sequence_ID", AD_Sequence_ID);
}
/** Get Sequence.
@return Document Sequence */
public int GetAD_Sequence_ID() 
{
Object ii = Get_Value("AD_Sequence_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accept AMEX.
@param AcceptAMEX Accept American Express Card */
public void SetAcceptAMEX (Boolean AcceptAMEX)
{
Set_Value ("AcceptAMEX", AcceptAMEX);
}
/** Get Accept AMEX.
@return Accept American Express Card */
public Boolean IsAcceptAMEX() 
{
Object oo = Get_Value("AcceptAMEX");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept ATM.
@param AcceptATM Accept Bank ATM Card */
public void SetAcceptATM (Boolean AcceptATM)
{
Set_Value ("AcceptATM", AcceptATM);
}
/** Get Accept ATM.
@return Accept Bank ATM Card */
public Boolean IsAcceptATM() 
{
Object oo = Get_Value("AcceptATM");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Electronic Check.
@param AcceptCheck Accept ECheck (Electronic Checks) */
public void SetAcceptCheck (Boolean AcceptCheck)
{
Set_Value ("AcceptCheck", AcceptCheck);
}
/** Get Accept Electronic Check.
@return Accept ECheck (Electronic Checks) */
public Boolean IsAcceptCheck() 
{
Object oo = Get_Value("AcceptCheck");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Corporate.
@param AcceptCorporate Accept Corporate Purchase Cards */
public void SetAcceptCorporate (Boolean AcceptCorporate)
{
Set_Value ("AcceptCorporate", AcceptCorporate);
}
/** Get Accept Corporate.
@return Accept Corporate Purchase Cards */
public Boolean IsAcceptCorporate() 
{
Object oo = Get_Value("AcceptCorporate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Diners.
@param AcceptDiners Accept Diner's Club */
public void SetAcceptDiners (Boolean AcceptDiners)
{
Set_Value ("AcceptDiners", AcceptDiners);
}
/** Get Accept Diners.
@return Accept Diner's Club */
public Boolean IsAcceptDiners() 
{
Object oo = Get_Value("AcceptDiners");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Direct Debit.
@param AcceptDirectDebit Accept Direct Debits (vendor initiated) */
public void SetAcceptDirectDebit (Boolean AcceptDirectDebit)
{
Set_Value ("AcceptDirectDebit", AcceptDirectDebit);
}
/** Get Accept Direct Debit.
@return Accept Direct Debits (vendor initiated) */
public Boolean IsAcceptDirectDebit() 
{
Object oo = Get_Value("AcceptDirectDebit");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Direct Deposit.
@param AcceptDirectDeposit Accept Direct Deposit (payee initiated) */
public void SetAcceptDirectDeposit (Boolean AcceptDirectDeposit)
{
Set_Value ("AcceptDirectDeposit", AcceptDirectDeposit);
}
/** Get Accept Direct Deposit.
@return Accept Direct Deposit (payee initiated) */
public Boolean IsAcceptDirectDeposit() 
{
Object oo = Get_Value("AcceptDirectDeposit");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Discover.
@param AcceptDiscover Accept Discover Card */
public void SetAcceptDiscover (Boolean AcceptDiscover)
{
Set_Value ("AcceptDiscover", AcceptDiscover);
}
/** Get Accept Discover.
@return Accept Discover Card */
public Boolean IsAcceptDiscover() 
{
Object oo = Get_Value("AcceptDiscover");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept MasterCard.
@param AcceptMC Accept Master Card */
public void SetAcceptMC (Boolean AcceptMC)
{
Set_Value ("AcceptMC", AcceptMC);
}
/** Get Accept MasterCard.
@return Accept Master Card */
public Boolean IsAcceptMC() 
{
Object oo = Get_Value("AcceptMC");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accept Visa.
@param AcceptVisa Accept Visa Cards */
public void SetAcceptVisa (Boolean AcceptVisa)
{
Set_Value ("AcceptVisa", AcceptVisa);
}
/** Get Accept Visa.
@return Accept Visa Cards */
public Boolean IsAcceptVisa() 
{
Object oo = Get_Value("AcceptVisa");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Payment Processor.
@param C_PaymentProcessor_ID Payment processor for electronic payments */
public void SetC_PaymentProcessor_ID (int C_PaymentProcessor_ID)
{
if (C_PaymentProcessor_ID < 1) throw new ArgumentException ("C_PaymentProcessor_ID is mandatory.");
Set_ValueNoCheck ("C_PaymentProcessor_ID", C_PaymentProcessor_ID);
}
/** Get Payment Processor.
@return Payment processor for electronic payments */
public int GetC_PaymentProcessor_ID() 
{
Object ii = Get_Value("C_PaymentProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission %.
@param Commission Commission stated as a percentage */
public void SetCommission (Decimal? Commission)
{
if (Commission == null) throw new ArgumentException ("Commission is mandatory.");
Set_Value ("Commission", (Decimal?)Commission);
}
/** Get Commission %.
@return Commission stated as a percentage */
public Decimal GetCommission() 
{
Object bd =Get_Value("Commission");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Cost per transaction.
@param CostPerTrx Fixed cost per transaction */
public void SetCostPerTrx (Decimal? CostPerTrx)
{
if (CostPerTrx == null) throw new ArgumentException ("CostPerTrx is mandatory.");
Set_Value ("CostPerTrx", (Decimal?)CostPerTrx);
}
/** Get Cost per transaction.
@return Fixed cost per transaction */
public Decimal GetCostPerTrx() 
{
Object bd =Get_Value("CostPerTrx");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Minimum Amt.
@param MinimumAmt Minumum Amout in Document Currency */
public void SetMinimumAmt (Decimal? MinimumAmt)
{
Set_Value ("MinimumAmt", (Decimal?)MinimumAmt);
}
/** Get Minimum Amt.
@return Minumum Amout in Document Currency */
public Decimal GetMinimumAmt() 
{
Object bd =Get_Value("MinimumAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Partner ID.
@param PartnerID Partner ID or Account for the Payment Processor */
public void SetPartnerID (String PartnerID)
{
if (PartnerID != null && PartnerID.Length > 60)
{
log.Warning("Length > 60 - truncated");
PartnerID = PartnerID.Substring(0,60);
}
Set_Value ("PartnerID", PartnerID);
}
/** Get Partner ID.
@return Partner ID or Account for the Payment Processor */
public String GetPartnerID() 
{
return (String)Get_Value("PartnerID");
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
/** Set Payment Processor Class.
@param PayProcessorClass Payment Processor Java Class */
public void SetPayProcessorClass (String PayProcessorClass)
{
if (PayProcessorClass != null && PayProcessorClass.Length > 60)
{
log.Warning("Length > 60 - truncated");
PayProcessorClass = PayProcessorClass.Substring(0,60);
}
Set_Value ("PayProcessorClass", PayProcessorClass);
}
/** Get Payment Processor Class.
@return Payment Processor Java Class */
public String GetPayProcessorClass() 
{
return (String)Get_Value("PayProcessorClass");
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
/** Set Require CreditCard Verification Code.
@param RequireVV Require 3/4 digit Credit Verification Code */
public void SetRequireVV (Boolean RequireVV)
{
Set_Value ("RequireVV", RequireVV);
}
/** Get Require CreditCard Verification Code.
@return Require 3/4 digit Credit Verification Code */
public Boolean IsRequireVV() 
{
Object oo = Get_Value("RequireVV");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Vendor ID.
@param VendorID Vendor ID for the Payment Processor */
public void SetVendorID (String VendorID)
{
if (VendorID != null && VendorID.Length > 60)
{
log.Warning("Length > 60 - truncated");
VendorID = VendorID.Substring(0,60);
}
Set_Value ("VendorID", VendorID);
}
/** Get Vendor ID.
@return Vendor ID for the Payment Processor */
public String GetVendorID() 
{
return (String)Get_Value("VendorID");
}
}

}
