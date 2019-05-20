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
/** Generated Model for I_BankStatement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_BankStatement : PO
{
public X_I_BankStatement (Context ctx, int I_BankStatement_ID, Trx trxName) : base (ctx, I_BankStatement_ID, trxName)
{
/** if (I_BankStatement_ID == 0)
{
SetI_BankStatement_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_BankStatement (Ctx ctx, int I_BankStatement_ID, Trx trxName) : base (ctx, I_BankStatement_ID, trxName)
{
/** if (I_BankStatement_ID == 0)
{
SetI_BankStatement_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BankStatement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BankStatement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BankStatement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_BankStatement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376889L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060100L;
/** AD_Table_ID=600 */
public static int Table_ID;
 // =600;

/** TableName=I_BankStatement */
public static String Table_Name="I_BankStatement";

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
StringBuilder sb = new StringBuilder ("X_I_BankStatement[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Bank Account No.
@param BankAccountNo Bank Account Number */
public void SetBankAccountNo (String BankAccountNo)
{
if (BankAccountNo != null && BankAccountNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
BankAccountNo = BankAccountNo.Substring(0,20);
}
Set_Value ("BankAccountNo", BankAccountNo);
}
/** Get Bank Account No.
@return Bank Account Number */
public String GetBankAccountNo() 
{
return (String)Get_Value("BankAccountNo");
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
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID <= 0) Set_Value ("C_BankAccount_ID", null);
else
Set_Value ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank statement line.
@param C_BankStatementLine_ID Line on a statement from this Bank */
public void SetC_BankStatementLine_ID (int C_BankStatementLine_ID)
{
if (C_BankStatementLine_ID <= 0) Set_Value ("C_BankStatementLine_ID", null);
else
Set_Value ("C_BankStatementLine_ID", C_BankStatementLine_ID);
}
/** Get Bank statement line.
@return Line on a statement from this Bank */
public int GetC_BankStatementLine_ID() 
{
Object ii = Get_Value("C_BankStatementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Statement.
@param C_BankStatement_ID Bank Statement of account */
public void SetC_BankStatement_ID (int C_BankStatement_ID)
{
if (C_BankStatement_ID <= 0) Set_Value ("C_BankStatement_ID", null);
else
Set_Value ("C_BankStatement_ID", C_BankStatement_ID);
}
/** Get Bank Statement.
@return Bank Statement of account */
public int GetC_BankStatement_ID() 
{
Object ii = Get_Value("C_BankStatement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge.
@param C_Charge_ID Additional document charges */
public void SetC_Charge_ID (int C_Charge_ID)
{
if (C_Charge_ID <= 0) Set_Value ("C_Charge_ID", null);
else
Set_Value ("C_Charge_ID", C_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetC_Charge_ID() 
{
Object ii = Get_Value("C_Charge_ID");
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
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);
else
Set_Value ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);
else
Set_Value ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge amount.
@param ChargeAmt Charge Amount */
public void SetChargeAmt (Decimal? ChargeAmt)
{
Set_Value ("ChargeAmt", (Decimal?)ChargeAmt);
}
/** Get Charge amount.
@return Charge Amount */
public Decimal GetChargeAmt() 
{
Object bd =Get_Value("ChargeAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Charge Name.
@param ChargeName Name of the Charge */
public void SetChargeName (String ChargeName)
{
if (ChargeName != null && ChargeName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ChargeName = ChargeName.Substring(0,60);
}
Set_Value ("ChargeName", ChargeName);
}
/** Get Charge Name.
@return Name of the Charge */
public String GetChargeName() 
{
return (String)Get_Value("ChargeName");
}
/** Set Create Payment.
@param CreatePayment Create Payment */
public void SetCreatePayment (String CreatePayment)
{
if (CreatePayment != null && CreatePayment.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreatePayment = CreatePayment.Substring(0,1);
}
Set_Value ("CreatePayment", CreatePayment);
}
/** Get Create Payment.
@return Create Payment */
public String GetCreatePayment() 
{
return (String)Get_Value("CreatePayment");
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
/** Set EFT Amount.
@param EftAmt Electronic Funds Transfer Amount */
public void SetEftAmt (Decimal? EftAmt)
{
Set_Value ("EftAmt", (Decimal?)EftAmt);
}
/** Get EFT Amount.
@return Electronic Funds Transfer Amount */
public Decimal GetEftAmt() 
{
Object bd =Get_Value("EftAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set EFT Check No.
@param EftCheckNo Electronic Funds Transfer Check No */
public void SetEftCheckNo (String EftCheckNo)
{
if (EftCheckNo != null && EftCheckNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftCheckNo = EftCheckNo.Substring(0,20);
}
Set_Value ("EftCheckNo", EftCheckNo);
}
/** Get EFT Check No.
@return Electronic Funds Transfer Check No */
public String GetEftCheckNo() 
{
return (String)Get_Value("EftCheckNo");
}
/** Set EFT Currency.
@param EftCurrency Electronic Funds Transfer Currency */
public void SetEftCurrency (String EftCurrency)
{
if (EftCurrency != null && EftCurrency.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftCurrency = EftCurrency.Substring(0,20);
}
Set_Value ("EftCurrency", EftCurrency);
}
/** Get EFT Currency.
@return Electronic Funds Transfer Currency */
public String GetEftCurrency() 
{
return (String)Get_Value("EftCurrency");
}
/** Set EFT Memo.
@param EftMemo Electronic Funds Transfer Memo */
public void SetEftMemo (String EftMemo)
{
if (EftMemo != null && EftMemo.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
EftMemo = EftMemo.Substring(0,2000);
}
Set_Value ("EftMemo", EftMemo);
}
/** Get EFT Memo.
@return Electronic Funds Transfer Memo */
public String GetEftMemo() 
{
return (String)Get_Value("EftMemo");
}
/** Set EFT Payee.
@param EftPayee Electronic Funds Transfer Payee information */
public void SetEftPayee (String EftPayee)
{
if (EftPayee != null && EftPayee.Length > 255)
{
log.Warning("Length > 255 - truncated");
EftPayee = EftPayee.Substring(0,255);
}
Set_Value ("EftPayee", EftPayee);
}
/** Get EFT Payee.
@return Electronic Funds Transfer Payee information */
public String GetEftPayee() 
{
return (String)Get_Value("EftPayee");
}
/** Set EFT Payee Account.
@param EftPayeeAccount Electronic Funds Transfer Payyee Account Information */
public void SetEftPayeeAccount (String EftPayeeAccount)
{
if (EftPayeeAccount != null && EftPayeeAccount.Length > 40)
{
log.Warning("Length > 40 - truncated");
EftPayeeAccount = EftPayeeAccount.Substring(0,40);
}
Set_Value ("EftPayeeAccount", EftPayeeAccount);
}
/** Get EFT Payee Account.
@return Electronic Funds Transfer Payyee Account Information */
public String GetEftPayeeAccount() 
{
return (String)Get_Value("EftPayeeAccount");
}
/** Set EFT Reference.
@param EftReference Electronic Funds Transfer Reference */
public void SetEftReference (String EftReference)
{
if (EftReference != null && EftReference.Length > 60)
{
log.Warning("Length > 60 - truncated");
EftReference = EftReference.Substring(0,60);
}
Set_Value ("EftReference", EftReference);
}
/** Get EFT Reference.
@return Electronic Funds Transfer Reference */
public String GetEftReference() 
{
return (String)Get_Value("EftReference");
}
/** Set EFT Statement Date.
@param EftStatementDate Electronic Funds Transfer Statement Date */
public void SetEftStatementDate (DateTime? EftStatementDate)
{
Set_Value ("EftStatementDate", (DateTime?)EftStatementDate);
}
/** Get EFT Statement Date.
@return Electronic Funds Transfer Statement Date */
public DateTime? GetEftStatementDate() 
{
return (DateTime?)Get_Value("EftStatementDate");
}
/** Set EFT Statement Line Date.
@param EftStatementLineDate Electronic Funds Transfer Statement Line Date */
public void SetEftStatementLineDate (DateTime? EftStatementLineDate)
{
Set_Value ("EftStatementLineDate", (DateTime?)EftStatementLineDate);
}
/** Get EFT Statement Line Date.
@return Electronic Funds Transfer Statement Line Date */
public DateTime? GetEftStatementLineDate() 
{
return (DateTime?)Get_Value("EftStatementLineDate");
}
/** Set EFT Statement Reference.
@param EftStatementReference Electronic Funds Transfer Statement Reference */
public void SetEftStatementReference (String EftStatementReference)
{
if (EftStatementReference != null && EftStatementReference.Length > 60)
{
log.Warning("Length > 60 - truncated");
EftStatementReference = EftStatementReference.Substring(0,60);
}
Set_Value ("EftStatementReference", EftStatementReference);
}
/** Get EFT Statement Reference.
@return Electronic Funds Transfer Statement Reference */
public String GetEftStatementReference() 
{
return (String)Get_Value("EftStatementReference");
}
/** Set EFT Trx ID.
@param EftTrxID Electronic Funds Transfer Transaction ID */
public void SetEftTrxID (String EftTrxID)
{
if (EftTrxID != null && EftTrxID.Length > 40)
{
log.Warning("Length > 40 - truncated");
EftTrxID = EftTrxID.Substring(0,40);
}
Set_Value ("EftTrxID", EftTrxID);
}
/** Get EFT Trx ID.
@return Electronic Funds Transfer Transaction ID */
public String GetEftTrxID() 
{
return (String)Get_Value("EftTrxID");
}
/** Set EFT Trx Type.
@param EftTrxType Electronic Funds Transfer Transaction Type */
public void SetEftTrxType (String EftTrxType)
{
if (EftTrxType != null && EftTrxType.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftTrxType = EftTrxType.Substring(0,20);
}
Set_Value ("EftTrxType", EftTrxType);
}
/** Get EFT Trx Type.
@return Electronic Funds Transfer Transaction Type */
public String GetEftTrxType() 
{
return (String)Get_Value("EftTrxType");
}
/** Set EFT Effective Date.
@param EftValutaDate Electronic Funds Transfer Valuta (effective) Date */
public void SetEftValutaDate (DateTime? EftValutaDate)
{
Set_Value ("EftValutaDate", (DateTime?)EftValutaDate);
}
/** Get EFT Effective Date.
@return Electronic Funds Transfer Valuta (effective) Date */
public DateTime? GetEftValutaDate() 
{
return (DateTime?)Get_Value("EftValutaDate");
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
/** Set Import Bank Statement.
@param I_BankStatement_ID Import of the Bank Statement */
public void SetI_BankStatement_ID (int I_BankStatement_ID)
{
if (I_BankStatement_ID < 1) throw new ArgumentException ("I_BankStatement_ID is mandatory.");
Set_ValueNoCheck ("I_BankStatement_ID", I_BankStatement_ID);
}
/** Get Import Bank Statement.
@return Import of the Bank Statement */
public int GetI_BankStatement_ID() 
{
Object ii = Get_Value("I_BankStatement_ID");
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
/** Set Interest Amount.
@param InterestAmt Interest Amount */
public void SetInterestAmt (Decimal? InterestAmt)
{
Set_Value ("InterestAmt", (Decimal?)InterestAmt);
}
/** Get Interest Amount.
@return Interest Amount */
public Decimal GetInterestAmt() 
{
Object bd =Get_Value("InterestAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Invoice Document No.
@param InvoiceDocumentNo Document Number of the Invoice */
public void SetInvoiceDocumentNo (String InvoiceDocumentNo)
{
if (InvoiceDocumentNo != null && InvoiceDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
InvoiceDocumentNo = InvoiceDocumentNo.Substring(0,30);
}
Set_Value ("InvoiceDocumentNo", InvoiceDocumentNo);
}
/** Get Invoice Document No.
@return Document Number of the Invoice */
public String GetInvoiceDocumentNo() 
{
return (String)Get_Value("InvoiceDocumentNo");
}
/** Set Reversal.
@param IsReversal This is a reversing transaction */
public void SetIsReversal (Boolean IsReversal)
{
Set_Value ("IsReversal", IsReversal);
}
/** Get Reversal.
@return This is a reversing transaction */
public Boolean IsReversal() 
{
Object oo = Get_Value("IsReversal");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Line Description.
@param LineDescription Description of the Line */
public void SetLineDescription (String LineDescription)
{
if (LineDescription != null && LineDescription.Length > 255)
{
log.Warning("Length > 255 - truncated");
LineDescription = LineDescription.Substring(0,255);
}
Set_Value ("LineDescription", LineDescription);
}
/** Get Line Description.
@return Description of the Line */
public String GetLineDescription() 
{
return (String)Get_Value("LineDescription");
}
/** Set Match Statement.
@param MatchStatement Match Statement */
public void SetMatchStatement (String MatchStatement)
{
if (MatchStatement != null && MatchStatement.Length > 1)
{
log.Warning("Length > 1 - truncated");
MatchStatement = MatchStatement.Substring(0,1);
}
Set_Value ("MatchStatement", MatchStatement);
}
/** Get Match Statement.
@return Match Statement */
public String GetMatchStatement() 
{
return (String)Get_Value("MatchStatement");
}
/** Set Memo.
@param Memo Memo Text */
public void SetMemo (String Memo)
{
if (Memo != null && Memo.Length > 255)
{
log.Warning("Length > 255 - truncated");
Memo = Memo.Substring(0,255);
}
Set_Value ("Memo", Memo);
}
/** Get Memo.
@return Memo Text */
public String GetMemo() 
{
return (String)Get_Value("Memo");
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
/** Set Payment Document No.
@param PaymentDocumentNo Document number of the Payment */
public void SetPaymentDocumentNo (String PaymentDocumentNo)
{
if (PaymentDocumentNo != null && PaymentDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
PaymentDocumentNo = PaymentDocumentNo.Substring(0,30);
}
Set_Value ("PaymentDocumentNo", PaymentDocumentNo);
}
/** Get Payment Document No.
@return Document number of the Payment */
public String GetPaymentDocumentNo() 
{
return (String)Get_Value("PaymentDocumentNo");
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
/** Set Reference No.
@param ReferenceNo Your customer or vendor number at the Business Partner's site */
public void SetReferenceNo (String ReferenceNo)
{
if (ReferenceNo != null && ReferenceNo.Length > 40)
{
log.Warning("Length > 40 - truncated");
ReferenceNo = ReferenceNo.Substring(0,40);
}
Set_Value ("ReferenceNo", ReferenceNo);
}
/** Get Reference No.
@return Your customer or vendor number at the Business Partner's site */
public String GetReferenceNo() 
{
return (String)Get_Value("ReferenceNo");
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
/** Set Statement date.
@param StatementDate Date of the statement */
public void SetStatementDate (DateTime? StatementDate)
{
Set_Value ("StatementDate", (DateTime?)StatementDate);
}
/** Get Statement date.
@return Date of the statement */
public DateTime? GetStatementDate() 
{
return (DateTime?)Get_Value("StatementDate");
}
/** Set Statement Line Date.
@param StatementLineDate Date of the Statement Line */
public void SetStatementLineDate (DateTime? StatementLineDate)
{
Set_Value ("StatementLineDate", (DateTime?)StatementLineDate);
}
/** Get Statement Line Date.
@return Date of the Statement Line */
public DateTime? GetStatementLineDate() 
{
return (DateTime?)Get_Value("StatementLineDate");
}
/** Set Statement amount.
@param StmtAmt Statement Amount */
public void SetStmtAmt (Decimal? StmtAmt)
{
Set_Value ("StmtAmt", (Decimal?)StmtAmt);
}
/** Get Statement amount.
@return Statement Amount */
public Decimal GetStmtAmt() 
{
Object bd =Get_Value("StmtAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Transaction Amount.
@param TrxAmt Amount of a transaction */
public void SetTrxAmt (Decimal? TrxAmt)
{
Set_Value ("TrxAmt", (Decimal?)TrxAmt);
}
/** Get Transaction Amount.
@return Amount of a transaction */
public Decimal GetTrxAmt() 
{
Object bd =Get_Value("TrxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** TrxType AD_Reference_ID=215 */
public static int TRXTYPE_AD_Reference_ID=215;
/** Authorization = A */
public static String TRXTYPE_Authorization = "A";
/** Credit (Payment) = C */
public static String TRXTYPE_CreditPayment = "C";
/** Delayed Capture = D */
public static String TRXTYPE_DelayedCapture = "D";
/** Voice Authorization = F */
public static String TRXTYPE_VoiceAuthorization = "F";
/** Sales = S */
public static String TRXTYPE_Sales = "S";
/** Void = V */
public static String TRXTYPE_Void = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTrxTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("C") || test.Equals("D") || test.Equals("F") || test.Equals("S") || test.Equals("V");
}
/** Set Transaction Type.
@param TrxType Type of credit card transaction */
public void SetTrxType (String TrxType)
{
if (!IsTrxTypeValid(TrxType))
throw new ArgumentException ("TrxType Invalid value - " + TrxType + " - Reference_ID=215 - A - C - D - F - S - V");
if (TrxType != null && TrxType.Length > 20)
{
log.Warning("Length > 20 - truncated");
TrxType = TrxType.Substring(0,20);
}
Set_Value ("TrxType", TrxType);
}
/** Get Transaction Type.
@return Type of credit card transaction */
public String GetTrxType() 
{
return (String)Get_Value("TrxType");
}
/** Set Effective date.
@param ValutaDate Date when money is available */
public void SetValutaDate (DateTime? ValutaDate)
{
Set_Value ("ValutaDate", (DateTime?)ValutaDate);
}
/** Get Effective date.
@return Date when money is available */
public DateTime? GetValutaDate() 
{
return (DateTime?)Get_Value("ValutaDate");
}
}

}
