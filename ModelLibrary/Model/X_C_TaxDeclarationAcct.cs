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
/** Generated Model for C_TaxDeclarationAcct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TaxDeclarationAcct : PO
{
public X_C_TaxDeclarationAcct (Context ctx, int C_TaxDeclarationAcct_ID, Trx trxName) : base (ctx, C_TaxDeclarationAcct_ID, trxName)
{
/** if (C_TaxDeclarationAcct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_TaxDeclarationAcct_ID (0);
SetC_TaxDeclaration_ID (0);
SetFact_Acct_ID (0);
}
 */
}
public X_C_TaxDeclarationAcct (Ctx ctx, int C_TaxDeclarationAcct_ID, Trx trxName) : base (ctx, C_TaxDeclarationAcct_ID, trxName)
{
/** if (C_TaxDeclarationAcct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_TaxDeclarationAcct_ID (0);
SetC_TaxDeclaration_ID (0);
SetFact_Acct_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationAcct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationAcct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationAcct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TaxDeclarationAcct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375463L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058674L;
/** AD_Table_ID=820 */
public static int Table_ID;
 // =820;

/** TableName=C_TaxDeclarationAcct */
public static String Table_Name="C_TaxDeclarationAcct";

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
StringBuilder sb = new StringBuilder ("X_C_TaxDeclarationAcct[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** Account_ID AD_Reference_ID=331 */
public static int ACCOUNT_ID_AD_Reference_ID=331;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
throw new ArgumentException ("Account_ID Is virtual column");
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
throw new ArgumentException ("AmtAcctCr Is virtual column");
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
throw new ArgumentException ("AmtAcctDr Is virtual column");
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
throw new ArgumentException ("AmtSourceCr Is virtual column");
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
throw new ArgumentException ("AmtSourceDr Is virtual column");
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
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
throw new ArgumentException ("C_BPartner_ID Is virtual column");
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
throw new ArgumentException ("C_Currency_ID Is virtual column");
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Declaration Accounting.
@param C_TaxDeclarationAcct_ID Tax Accounting Reconciliation */
public void SetC_TaxDeclarationAcct_ID (int C_TaxDeclarationAcct_ID)
{
if (C_TaxDeclarationAcct_ID < 1) throw new ArgumentException ("C_TaxDeclarationAcct_ID is mandatory.");
Set_ValueNoCheck ("C_TaxDeclarationAcct_ID", C_TaxDeclarationAcct_ID);
}
/** Get Tax Declaration Accounting.
@return Tax Accounting Reconciliation */
public int GetC_TaxDeclarationAcct_ID() 
{
Object ii = Get_Value("C_TaxDeclarationAcct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Declaration.
@param C_TaxDeclaration_ID Define the declaration to the tax authorities */
public void SetC_TaxDeclaration_ID (int C_TaxDeclaration_ID)
{
if (C_TaxDeclaration_ID < 1) throw new ArgumentException ("C_TaxDeclaration_ID is mandatory.");
Set_ValueNoCheck ("C_TaxDeclaration_ID", C_TaxDeclaration_ID);
}
/** Get Tax Declaration.
@return Define the declaration to the tax authorities */
public int GetC_TaxDeclaration_ID() 
{
Object ii = Get_Value("C_TaxDeclaration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
throw new ArgumentException ("C_Tax_ID Is virtual column");
}
/** Get Tax.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
throw new ArgumentException ("DateAcct Is virtual column");
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
/** Set Accounting Fact.
@param Fact_Acct_ID Accounting Fact */
public void SetFact_Acct_ID (int Fact_Acct_ID)
{
if (Fact_Acct_ID < 1) throw new ArgumentException ("Fact_Acct_ID is mandatory.");
Set_ValueNoCheck ("Fact_Acct_ID", Fact_Acct_ID);
}
/** Get Accounting Fact.
@return Accounting Fact */
public int GetFact_Acct_ID() 
{
Object ii = Get_Value("Fact_Acct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
