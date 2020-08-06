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
/** Generated Model for C_TaxDeclarationLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TaxDeclarationLine : PO
{
public X_C_TaxDeclarationLine (Context ctx, int C_TaxDeclarationLine_ID, Trx trxName) : base (ctx, C_TaxDeclarationLine_ID, trxName)
{
/** if (C_TaxDeclarationLine_ID == 0)
{
SetC_BPartner_ID (0);
SetC_Currency_ID (0);
SetC_TaxDeclarationLine_ID (0);
SetC_TaxDeclaration_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);
SetIsManual (true);	// Y
SetLine (0);
SetTaxAmt (0.0);
SetTaxBaseAmt (0.0);
}
 */
}
public X_C_TaxDeclarationLine (Ctx ctx, int C_TaxDeclarationLine_ID, Trx trxName) : base (ctx, C_TaxDeclarationLine_ID, trxName)
{
/** if (C_TaxDeclarationLine_ID == 0)
{
SetC_BPartner_ID (0);
SetC_Currency_ID (0);
SetC_TaxDeclarationLine_ID (0);
SetC_TaxDeclaration_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);
SetIsManual (true);	// Y
SetLine (0);
SetTaxAmt (0.0);
SetTaxBaseAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxDeclarationLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TaxDeclarationLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375494L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058705L;
/** AD_Table_ID=819 */
public static int Table_ID;
 // =819;

/** TableName=C_TaxDeclarationLine */
public static String Table_Name="C_TaxDeclarationLine";

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
StringBuilder sb = new StringBuilder ("X_C_TaxDeclarationLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Allocation Line.
@param C_AllocationLine_ID Allocation Line */
public void SetC_AllocationLine_ID (int C_AllocationLine_ID)
{
if (C_AllocationLine_ID <= 0) Set_ValueNoCheck ("C_AllocationLine_ID", null);
else
Set_ValueNoCheck ("C_AllocationLine_ID", C_AllocationLine_ID);
}
/** Get Allocation Line.
@return Allocation Line */
public int GetC_AllocationLine_ID() 
{
Object ii = Get_Value("C_AllocationLine_ID");
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
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_ValueNoCheck ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
public void SetC_InvoiceLine_ID (int C_InvoiceLine_ID)
{
if (C_InvoiceLine_ID <= 0) Set_ValueNoCheck ("C_InvoiceLine_ID", null);
else
Set_ValueNoCheck ("C_InvoiceLine_ID", C_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetC_InvoiceLine_ID() 
{
Object ii = Get_Value("C_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_ValueNoCheck ("C_Invoice_ID", null);
else
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Declaration Line.
@param C_TaxDeclarationLine_ID Tax Declaration Document Information */
public void SetC_TaxDeclarationLine_ID (int C_TaxDeclarationLine_ID)
{
if (C_TaxDeclarationLine_ID < 1) throw new ArgumentException ("C_TaxDeclarationLine_ID is mandatory.");
Set_ValueNoCheck ("C_TaxDeclarationLine_ID", C_TaxDeclarationLine_ID);
}
/** Get Tax Declaration Line.
@return Tax Declaration Document Information */
public int GetC_TaxDeclarationLine_ID() 
{
Object ii = Get_Value("C_TaxDeclarationLine_ID");
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
if (C_Tax_ID < 1) throw new ArgumentException ("C_Tax_ID is mandatory.");
Set_ValueNoCheck ("C_Tax_ID", C_Tax_ID);
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
if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");
Set_ValueNoCheck ("DateAcct", (DateTime?)DateAcct);
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
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_ValueNoCheck ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
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
/** Set Tax Amount.
@param TaxAmt Tax Amount for a document */
public void SetTaxAmt (Decimal? TaxAmt)
{
if (TaxAmt == null) throw new ArgumentException ("TaxAmt is mandatory.");
Set_ValueNoCheck ("TaxAmt", (Decimal?)TaxAmt);
}
/** Get Tax Amount.
@return Tax Amount for a document */
public Decimal GetTaxAmt() 
{
Object bd =Get_Value("TaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Taxable Amount.
@param TaxBaseAmt Base for calculating the tax amount */
public void SetTaxBaseAmt (Decimal? TaxBaseAmt)
{
if (TaxBaseAmt == null) throw new ArgumentException ("TaxBaseAmt is mandatory.");
Set_ValueNoCheck ("TaxBaseAmt", (Decimal?)TaxBaseAmt);
}
/** Get Taxable Amount.
@return Base for calculating the tax amount */
public Decimal GetTaxBaseAmt() 
{
Object bd =Get_Value("TaxBaseAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
