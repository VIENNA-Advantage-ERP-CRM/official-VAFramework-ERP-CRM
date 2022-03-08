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
/** Generated Model for C_BP_BankAccount
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_BankAccount : PO
{
public X_C_BP_BankAccount (Context ctx, int C_BP_BankAccount_ID, Trx trxName) : base (ctx, C_BP_BankAccount_ID, trxName)
{
/** if (C_BP_BankAccount_ID == 0)
{
SetC_BP_BankAccount_ID (0);
SetC_BPartner_ID (0);
SetIsACH (false);
}
 */
}
public X_C_BP_BankAccount (Ctx ctx, int C_BP_BankAccount_ID, Trx trxName) : base (ctx, C_BP_BankAccount_ID, trxName)
{
/** if (C_BP_BankAccount_ID == 0)
{
SetC_BP_BankAccount_ID (0);
SetC_BPartner_ID (0);
SetIsACH (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_BankAccount (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_BankAccount (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_BankAccount (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_BankAccount()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369868L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053079L;
/** AD_Table_ID=298 */
public static int Table_ID;
 // =298;

/** TableName=C_BP_BankAccount */
public static String Table_Name="C_BP_BankAccount";

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
StringBuilder sb = new StringBuilder ("X_C_BP_BankAccount[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account City.
@param A_City City or the Credit Card or Account Holder */
public void SetA_City (String A_City)
{
if (A_City != null && A_City.Length > 60)
{
log.Warning("Length > 60 - truncated");
A_City = A_City.Substring(0,60);
}
Set_Value ("A_City", A_City);
}
/** Get Account City.
@return City or the Credit Card or Account Holder */
public String GetA_City() 
{
return (String)Get_Value("A_City");
}
/** Set Account Country.
@param A_Country Country */
public void SetA_Country (String A_Country)
{
if (A_Country != null && A_Country.Length > 40)
{
log.Warning("Length > 40 - truncated");
A_Country = A_Country.Substring(0,40);
}
Set_Value ("A_Country", A_Country);
}
/** Get Account Country.
@return Country */
public String GetA_Country() 
{
return (String)Get_Value("A_Country");
}
/** Set Account EMail.
@param A_EMail Email Address */
public void SetA_EMail (String A_EMail)
{
if (A_EMail != null && A_EMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
A_EMail = A_EMail.Substring(0,60);
}
Set_Value ("A_EMail", A_EMail);
}
/** Get Account EMail.
@return Email Address */
public String GetA_EMail() 
{
return (String)Get_Value("A_EMail");
}
/** Set Driver License.
@param A_Ident_DL Payment Identification - Driver License */
public void SetA_Ident_DL (String A_Ident_DL)
{
if (A_Ident_DL != null && A_Ident_DL.Length > 20)
{
log.Warning("Length > 20 - truncated");
A_Ident_DL = A_Ident_DL.Substring(0,20);
}
Set_Value ("A_Ident_DL", A_Ident_DL);
}
/** Get Driver License.
@return Payment Identification - Driver License */
public String GetA_Ident_DL() 
{
return (String)Get_Value("A_Ident_DL");
}
/** Set Social Security No.
@param A_Ident_SSN Payment Identification - Social Security No */
public void SetA_Ident_SSN (String A_Ident_SSN)
{
if (A_Ident_SSN != null && A_Ident_SSN.Length > 20)
{
log.Warning("Length > 20 - truncated");
A_Ident_SSN = A_Ident_SSN.Substring(0,20);
}
Set_Value ("A_Ident_SSN", A_Ident_SSN);
}
/** Get Social Security No.
@return Payment Identification - Social Security No */
public String GetA_Ident_SSN() 
{
return (String)Get_Value("A_Ident_SSN");
}
/** Set Account Name.
@param A_Name Name on Credit Card or Account holder */
public void SetA_Name (String A_Name)
{
if (A_Name != null && A_Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
A_Name = A_Name.Substring(0,60);
}
Set_Value ("A_Name", A_Name);
}
/** Get Account Name.
@return Name on Credit Card or Account holder */
public String GetA_Name() 
{
return (String)Get_Value("A_Name");
}
/** Set Account State.
@param A_State State of the Credit Card or Account holder */
public void SetA_State (String A_State)
{
if (A_State != null && A_State.Length > 40)
{
log.Warning("Length > 40 - truncated");
A_State = A_State.Substring(0,40);
}
Set_Value ("A_State", A_State);
}
/** Get Account State.
@return State of the Credit Card or Account holder */
public String GetA_State() 
{
return (String)Get_Value("A_State");
}
/** Set Account Street.
@param A_Street Street address of the Credit Card or Account holder */
public void SetA_Street (String A_Street)
{
if (A_Street != null && A_Street.Length > 60)
{
log.Warning("Length > 60 - truncated");
A_Street = A_Street.Substring(0,60);
}
Set_Value ("A_Street", A_Street);
}
/** Get Account Street.
@return Street address of the Credit Card or Account holder */
public String GetA_Street() 
{
return (String)Get_Value("A_Street");
}
/** Set Account Zip/Postal.
@param A_Zip Zip Code of the Credit Card or Account Holder */
public void SetA_Zip (String A_Zip)
{
if (A_Zip != null && A_Zip.Length > 20)
{
log.Warning("Length > 20 - truncated");
A_Zip = A_Zip.Substring(0,20);
}
Set_Value ("A_Zip", A_Zip);
}
/** Get Account Zip/Postal.
@return Zip Code of the Credit Card or Account Holder */
public String GetA_Zip() 
{
return (String)Get_Value("A_Zip");
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
/** Set BBAN.
@param BBAN Basic Bank Account Number */
public void SetBBAN (String BBAN)
{
if (BBAN != null && BBAN.Length > 40)
{
log.Warning("Length > 40 - truncated");
BBAN = BBAN.Substring(0,40);
}
Set_Value ("BBAN", BBAN);
}
/** Get BBAN.
@return Basic Bank Account Number */
public String GetBBAN() 
{
return (String)Get_Value("BBAN");
}

/** BPBankAcctUse AD_Reference_ID=393 */
public static int BPBANKACCTUSE_AD_Reference_ID=393;
/** Both = B */
public static String BPBANKACCTUSE_Both = "B";
/** Direct Debit = D */
public static String BPBANKACCTUSE_DirectDebit = "D";
/** None = N */
public static String BPBANKACCTUSE_None = "N";
/** Direct Deposit = T */
public static String BPBANKACCTUSE_DirectDeposit = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBPBankAcctUseValid (String test)
{
return test == null || test.Equals("B") || test.Equals("D") || test.Equals("N") || test.Equals("T");
}
/** Set Account Usage.
@param BPBankAcctUse Business Partner Bank Account usage */
public void SetBPBankAcctUse (String BPBankAcctUse)
{
if (!IsBPBankAcctUseValid(BPBankAcctUse))
throw new ArgumentException ("BPBankAcctUse Invalid value - " + BPBankAcctUse + " - Reference_ID=393 - B - D - N - T");
if (BPBankAcctUse != null && BPBankAcctUse.Length > 1)
{
log.Warning("Length > 1 - truncated");
BPBankAcctUse = BPBankAcctUse.Substring(0,1);
}
Set_Value ("BPBankAcctUse", BPBankAcctUse);
}
/** Get Account Usage.
@return Business Partner Bank Account usage */
public String GetBPBankAcctUse() 
{
return (String)Get_Value("BPBankAcctUse");
}

/** BankAccountType AD_Reference_ID=216 */
public static int BANKACCOUNTTYPE_AD_Reference_ID=216;
/** Checking = C */
public static String BANKACCOUNTTYPE_Checking = "C";
/** Savings = S */
public static String BANKACCOUNTTYPE_Savings = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBankAccountTypeValid (String test)
{
return test == null || test.Equals("C") || test.Equals("S");
}
/** Set Bank Account Type.
@param BankAccountType Bank Account Type */
public void SetBankAccountType (String BankAccountType)
{
if (!IsBankAccountTypeValid(BankAccountType))
throw new ArgumentException ("BankAccountType Invalid value - " + BankAccountType + " - Reference_ID=216 - C - S");
if (BankAccountType != null && BankAccountType.Length > 1)
{
log.Warning("Length > 1 - truncated");
BankAccountType = BankAccountType.Substring(0,1);
}
Set_Value ("BankAccountType", BankAccountType);
}
/** Get Bank Account Type.
@return Bank Account Type */
public String GetBankAccountType() 
{
return (String)Get_Value("BankAccountType");
}
/** Set Partner Bank Account.
@param C_BP_BankAccount_ID Bank Account of the Business Partner */
public void SetC_BP_BankAccount_ID (int C_BP_BankAccount_ID)
{
if (C_BP_BankAccount_ID < 1) throw new ArgumentException ("C_BP_BankAccount_ID is mandatory.");
Set_ValueNoCheck ("C_BP_BankAccount_ID", C_BP_BankAccount_ID);
}
/** Get Partner Bank Account.
@return Bank Account of the Business Partner */
public int GetC_BP_BankAccount_ID() 
{
Object ii = Get_Value("C_BP_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank.
@param C_Bank_ID Bank */
public void SetC_Bank_ID (int C_Bank_ID)
{
if (C_Bank_ID <= 0) Set_Value ("C_Bank_ID", null);
else
Set_Value ("C_Bank_ID", C_Bank_ID);
}
/** Get Bank.
@return Bank */
public int GetC_Bank_ID() 
{
Object ii = Get_Value("C_Bank_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Bank_ID().ToString());
}
/** Set Exp. Month.
@param CreditCardExpMM Expiry Month */
public void SetCreditCardExpMM (int CreditCardExpMM)
{
Set_Value ("CreditCardExpMM", CreditCardExpMM);
}
/** Get Exp. Month.
@return Expiry Month */
public int GetCreditCardExpMM() 
{
Object ii = Get_Value("CreditCardExpMM");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Exp. Year.
@param CreditCardExpYY Expiry Year */
public void SetCreditCardExpYY (int CreditCardExpYY)
{
Set_Value ("CreditCardExpYY", CreditCardExpYY);
}
/** Get Exp. Year.
@return Expiry Year */
public int GetCreditCardExpYY() 
{
Object ii = Get_Value("CreditCardExpYY");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Number.
@param CreditCardNumber Credit Card Number */
public void SetCreditCardNumber (String CreditCardNumber)
{
if (CreditCardNumber != null && CreditCardNumber.Length > 20)
{
log.Warning("Length > 20 - truncated");
CreditCardNumber = CreditCardNumber.Substring(0,20);
}
Set_Value ("CreditCardNumber", CreditCardNumber);
}
/** Get Number.
@return Credit Card Number */
public String GetCreditCardNumber() 
{
return (String)Get_Value("CreditCardNumber");
}

/** CreditCardType AD_Reference_ID=149 */
public static int CREDITCARDTYPE_AD_Reference_ID=149;
/** Amex = A */
public static String CREDITCARDTYPE_Amex = "A";
/** ATM = C */
public static String CREDITCARDTYPE_ATM = "C";
/** Diners = D */
public static String CREDITCARDTYPE_Diners = "D";
/** MasterCard = M */
public static String CREDITCARDTYPE_MasterCard = "M";
/** Discover = N */
public static String CREDITCARDTYPE_Discover = "N";
/** Purchase Card = P */
public static String CREDITCARDTYPE_PurchaseCard = "P";
/** Visa = V */
public static String CREDITCARDTYPE_Visa = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCreditCardTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("C") || test.Equals("D") || test.Equals("M") || test.Equals("N") || test.Equals("P") || test.Equals("V");
}
/** Set Credit Card.
@param CreditCardType Credit Card (Visa, MC, AmEx) */
public void SetCreditCardType (String CreditCardType)
{
if (!IsCreditCardTypeValid(CreditCardType))
throw new ArgumentException ("CreditCardType Invalid value - " + CreditCardType + " - Reference_ID=149 - A - C - D - M - N - P - V");
if (CreditCardType != null && CreditCardType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreditCardType = CreditCardType.Substring(0,1);
}
Set_Value ("CreditCardType", CreditCardType);
}
/** Get Credit Card.
@return Credit Card (Visa, MC, AmEx) */
public String GetCreditCardType() 
{
return (String)Get_Value("CreditCardType");
}
/** Set IBAN.
@param IBAN International Bank Account Number */
public void SetIBAN (String IBAN)
{
if (IBAN != null && IBAN.Length > 40)
{
log.Warning("Length > 40 - truncated");
IBAN = IBAN.Substring(0,40);
}
Set_Value ("IBAN", IBAN);
}
/** Get IBAN.
@return International Bank Account Number */
public String GetIBAN() 
{
return (String)Get_Value("IBAN");
}
/** Set ACH.
@param IsACH Automatic Clearing House */
public void SetIsACH (Boolean IsACH)
{
Set_Value ("IsACH", IsACH);
}
/** Get ACH.
@return Automatic Clearing House */
public Boolean IsACH() 
{
Object oo = Get_Value("IsACH");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** R_AvsAddr AD_Reference_ID=213 */
public static int R_AVSADDR_AD_Reference_ID=213;
/** No Match = N */
public static String R_AVSADDR_NoMatch = "N";
/** Unavailable = X */
public static String R_AVSADDR_Unavailable = "X";
/** Match = Y */
public static String R_AVSADDR_Match = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsR_AvsAddrValid (String test)
{
return test == null || test.Equals("N") || test.Equals("X") || test.Equals("Y");
}
/** Set Address verified.
@param R_AvsAddr This address has been verified */
public void SetR_AvsAddr (String R_AvsAddr)
{
if (!IsR_AvsAddrValid(R_AvsAddr))
throw new ArgumentException ("R_AvsAddr Invalid value - " + R_AvsAddr + " - Reference_ID=213 - N - X - Y");
if (R_AvsAddr != null && R_AvsAddr.Length > 1)
{
log.Warning("Length > 1 - truncated");
R_AvsAddr = R_AvsAddr.Substring(0,1);
}
Set_ValueNoCheck ("R_AvsAddr", R_AvsAddr);
}
/** Get Address verified.
@return This address has been verified */
public String GetR_AvsAddr() 
{
return (String)Get_Value("R_AvsAddr");
}

/** R_AvsZip AD_Reference_ID=213 */
public static int R_AVSZIP_AD_Reference_ID=213;
/** No Match = N */
public static String R_AVSZIP_NoMatch = "N";
/** Unavailable = X */
public static String R_AVSZIP_Unavailable = "X";
/** Match = Y */
public static String R_AVSZIP_Match = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsR_AvsZipValid (String test)
{
return test == null || test.Equals("N") || test.Equals("X") || test.Equals("Y");
}
/** Set Zip verified.
@param R_AvsZip The Zip Code has been verified */
public void SetR_AvsZip (String R_AvsZip)
{
if (!IsR_AvsZipValid(R_AvsZip))
throw new ArgumentException ("R_AvsZip Invalid value - " + R_AvsZip + " - Reference_ID=213 - N - X - Y");
if (R_AvsZip != null && R_AvsZip.Length > 1)
{
log.Warning("Length > 1 - truncated");
R_AvsZip = R_AvsZip.Substring(0,1);
}
Set_ValueNoCheck ("R_AvsZip", R_AvsZip);
}
/** Get Zip verified.
@return The Zip Code has been verified */
public String GetR_AvsZip() 
{
return (String)Get_Value("R_AvsZip");
}
/** Set Routing No.
@param RoutingNo Bank Routing Number */
public void SetRoutingNo (String RoutingNo)
{
if (RoutingNo != null && RoutingNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
RoutingNo = RoutingNo.Substring(0,20);
}
Set_Value ("RoutingNo", RoutingNo);
}
/** Get Routing No.
@return Bank Routing Number */
public String GetRoutingNo() 
{
return (String)Get_Value("RoutingNo");
}
}

}
