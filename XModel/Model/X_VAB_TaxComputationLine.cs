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
/** Generated Model for VAB_TaxComputationLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_TaxComputationLine : PO
{
public X_VAB_TaxComputationLine (Context ctx, int VAB_TaxComputationLine_ID, Trx trxName) : base (ctx, VAB_TaxComputationLine_ID, trxName)
{
/** if (VAB_TaxComputationLine_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_Currency_ID (0);
SetVAB_TaxComputationLine_ID (0);
SetVAB_TaxRateComputation_ID (0);
SetVAB_TaxRate_ID (0);
SetDateAcct (DateTime.Now);
SetIsManual (true);	// Y
SetLine (0);
SetTaxAmt (0.0);
SetTaxBaseAmt (0.0);
}
 */
}
public X_VAB_TaxComputationLine (Ctx ctx, int VAB_TaxComputationLine_ID, Trx trxName) : base (ctx, VAB_TaxComputationLine_ID, trxName)
{
/** if (VAB_TaxComputationLine_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_Currency_ID (0);
SetVAB_TaxComputationLine_ID (0);
SetVAB_TaxRateComputation_ID (0);
SetVAB_TaxRate_ID (0);
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
public X_VAB_TaxComputationLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TaxComputationLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TaxComputationLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_TaxComputationLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375494L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058705L;
/** VAF_TableView_ID=819 */
public static int Table_ID;
 // =819;

/** TableName=VAB_TaxComputationLine */
public static String Table_Name="VAB_TaxComputationLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_TaxComputationLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Allocation Line.
@param VAB_DocAllocationLine_ID Allocation Line */
public void SetVAB_DocAllocationLine_ID (int VAB_DocAllocationLine_ID)
{
if (VAB_DocAllocationLine_ID <= 0) Set_ValueNoCheck ("VAB_DocAllocationLine_ID", null);
else
Set_ValueNoCheck ("VAB_DocAllocationLine_ID", VAB_DocAllocationLine_ID);
}
/** Get Allocation Line.
@return Allocation Line */
public int GetVAB_DocAllocationLine_ID() 
{
Object ii = Get_Value("VAB_DocAllocationLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
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
/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");
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
/** Set Invoice Line.
@param VAB_InvoiceLine_ID Invoice Detail Line */
public void SetVAB_InvoiceLine_ID (int VAB_InvoiceLine_ID)
{
if (VAB_InvoiceLine_ID <= 0) Set_ValueNoCheck ("VAB_InvoiceLine_ID", null);
else
Set_ValueNoCheck ("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetVAB_InvoiceLine_ID() 
{
Object ii = Get_Value("VAB_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
public void SetVAB_Invoice_ID (int VAB_Invoice_ID)
{
if (VAB_Invoice_ID <= 0) Set_ValueNoCheck ("VAB_Invoice_ID", null);
else
Set_ValueNoCheck ("VAB_Invoice_ID", VAB_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetVAB_Invoice_ID() 
{
Object ii = Get_Value("VAB_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Declaration Line.
@param VAB_TaxComputationLine_ID Tax Declaration Document Information */
public void SetVAB_TaxComputationLine_ID (int VAB_TaxComputationLine_ID)
{
if (VAB_TaxComputationLine_ID < 1) throw new ArgumentException ("VAB_TaxComputationLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_TaxComputationLine_ID", VAB_TaxComputationLine_ID);
}
/** Get Tax Declaration Line.
@return Tax Declaration Document Information */
public int GetVAB_TaxComputationLine_ID() 
{
Object ii = Get_Value("VAB_TaxComputationLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Declaration.
@param VAB_TaxRateComputation_ID Define the declaration to the tax authorities */
public void SetVAB_TaxRateComputation_ID (int VAB_TaxRateComputation_ID)
{
if (VAB_TaxRateComputation_ID < 1) throw new ArgumentException ("VAB_TaxRateComputation_ID is mandatory.");
Set_ValueNoCheck ("VAB_TaxRateComputation_ID", VAB_TaxRateComputation_ID);
}
/** Get Tax Declaration.
@return Define the declaration to the tax authorities */
public int GetVAB_TaxRateComputation_ID() 
{
Object ii = Get_Value("VAB_TaxRateComputation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax.
@param VAB_TaxRate_ID Tax identifier */
public void SetVAB_TaxRate_ID (int VAB_TaxRate_ID)
{
if (VAB_TaxRate_ID < 1) throw new ArgumentException ("VAB_TaxRate_ID is mandatory.");
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
