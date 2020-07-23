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
/** Generated Model for C_InvoiceBatchLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_InvoiceBatchLine : PO
{
public X_C_InvoiceBatchLine (Context ctx, int C_InvoiceBatchLine_ID, Trx trxName) : base (ctx, C_InvoiceBatchLine_ID, trxName)
{
/** if (C_InvoiceBatchLine_ID == 0)
{
SetC_BPartner_ID (0);	// @C_BPartner_ID@
SetC_BPartner_Location_ID (0);	// @C_BPartner_Location_ID@
SetC_Charge_ID (0);
SetC_DocType_ID (0);	// @C_DocType_ID@
SetC_InvoiceBatchLine_ID (0);
SetC_InvoiceBatch_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);	// @DateAcct@;
@DateDoc@
SetDateInvoiced (DateTime.Now);	// @DateInvoiced@;
@DateDoc@
SetDocumentNo (null);	// @DocumentNo@
SetIsTaxIncluded (false);	// @IsTaxIncluded@
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=@C_InvoiceBatch_ID@
SetLineNetAmt (0.0);
SetLineTotalAmt (0.0);
SetPriceEntered (0.0);
SetProcessed (false);	// N
SetQtyEntered (0.0);	// 1
SetTaxAmt (0.0);
}
 */
}
public X_C_InvoiceBatchLine (Ctx ctx, int C_InvoiceBatchLine_ID, Trx trxName) : base (ctx, C_InvoiceBatchLine_ID, trxName)
{
/** if (C_InvoiceBatchLine_ID == 0)
{
SetC_BPartner_ID (0);	// @C_BPartner_ID@
SetC_BPartner_Location_ID (0);	// @C_BPartner_Location_ID@
SetC_Charge_ID (0);
SetC_DocType_ID (0);	// @C_DocType_ID@
SetC_InvoiceBatchLine_ID (0);
SetC_InvoiceBatch_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);	// @DateAcct@;
@DateDoc@
SetDateInvoiced (DateTime.Now);	// @DateInvoiced@;
@DateDoc@
SetDocumentNo (null);	// @DocumentNo@
SetIsTaxIncluded (false);	// @IsTaxIncluded@
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=@C_InvoiceBatch_ID@
SetLineNetAmt (0.0);
SetLineTotalAmt (0.0);
SetPriceEntered (0.0);
SetProcessed (false);	// N
SetQtyEntered (0.0);	// 1
SetTaxAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceBatchLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceBatchLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceBatchLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_InvoiceBatchLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372422L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055633L;
/** AD_Table_ID=768 */
public static int Table_ID;
 // =768;

/** TableName=C_InvoiceBatchLine */
public static String Table_Name="C_InvoiceBatchLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_C_InvoiceBatchLine[").Append(Get_ID()).Append("]");
return sb.ToString();
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
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
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
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID < 1) throw new ArgumentException ("C_BPartner_Location_ID is mandatory.");
Set_Value ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge.
@param C_Charge_ID Additional document charges */
public void SetC_Charge_ID (int C_Charge_ID)
{
if (C_Charge_ID < 1) throw new ArgumentException ("C_Charge_ID is mandatory.");
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
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID < 0) throw new ArgumentException ("C_DocType_ID is mandatory.");
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
/** Set Invoice Batch Line.
@param C_InvoiceBatchLine_ID Expense Invoice Batch Line */
public void SetC_InvoiceBatchLine_ID (int C_InvoiceBatchLine_ID)
{
if (C_InvoiceBatchLine_ID < 1) throw new ArgumentException ("C_InvoiceBatchLine_ID is mandatory.");
Set_ValueNoCheck ("C_InvoiceBatchLine_ID", C_InvoiceBatchLine_ID);
}
/** Get Invoice Batch Line.
@return Expense Invoice Batch Line */
public int GetC_InvoiceBatchLine_ID() 
{
Object ii = Get_Value("C_InvoiceBatchLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Batch.
@param C_InvoiceBatch_ID Expense Invoice Batch Header */
public void SetC_InvoiceBatch_ID (int C_InvoiceBatch_ID)
{
if (C_InvoiceBatch_ID < 1) throw new ArgumentException ("C_InvoiceBatch_ID is mandatory.");
Set_ValueNoCheck ("C_InvoiceBatch_ID", C_InvoiceBatch_ID);
}
/** Get Invoice Batch.
@return Expense Invoice Batch Header */
public int GetC_InvoiceBatch_ID() 
{
Object ii = Get_Value("C_InvoiceBatch_ID");
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
/** Set Tax.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID < 1) throw new ArgumentException ("C_Tax_ID is mandatory.");
Set_Value ("C_Tax_ID", C_Tax_ID);
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
Set_Value ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
}
/** Set Date Invoiced.
@param DateInvoiced Date printed on Invoice */
public void SetDateInvoiced (DateTime? DateInvoiced)
{
if (DateInvoiced == null) throw new ArgumentException ("DateInvoiced is mandatory.");
Set_Value ("DateInvoiced", (DateTime?)DateInvoiced);
}
/** Get Date Invoiced.
@return Date printed on Invoice */
public DateTime? GetDateInvoiced() 
{
return (DateTime?)Get_Value("DateInvoiced");
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
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Set Price includes Tax.
@param IsTaxIncluded Tax is included in the price */
public void SetIsTaxIncluded (Boolean IsTaxIncluded)
{
Set_Value ("IsTaxIncluded", IsTaxIncluded);
}
/** Get Price includes Tax.
@return Tax is included in the price */
public Boolean IsTaxIncluded() 
{
Object oo = Get_Value("IsTaxIncluded");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetLine().ToString());
}
/** Set Line Amount.
@param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
public void SetLineNetAmt (Decimal? LineNetAmt)
{
if (LineNetAmt == null) throw new ArgumentException ("LineNetAmt is mandatory.");
Set_ValueNoCheck ("LineNetAmt", (Decimal?)LineNetAmt);
}
/** Get Line Amount.
@return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
public Decimal GetLineNetAmt() 
{
Object bd =Get_Value("LineNetAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Line Total.
@param LineTotalAmt Total line amount incl. Tax */
public void SetLineTotalAmt (Decimal? LineTotalAmt)
{
if (LineTotalAmt == null) throw new ArgumentException ("LineTotalAmt is mandatory.");
Set_ValueNoCheck ("LineTotalAmt", (Decimal?)LineTotalAmt);
}
/** Get Line Total.
@return Total line amount incl. Tax */
public Decimal GetLineTotalAmt() 
{
Object bd =Get_Value("LineTotalAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Price.
@param PriceEntered Price Entered - the price based on the selected/base UoM */
public void SetPriceEntered (Decimal? PriceEntered)
{
if (PriceEntered == null) throw new ArgumentException ("PriceEntered is mandatory.");
Set_Value ("PriceEntered", (Decimal?)PriceEntered);
}
/** Get Price.
@return Price Entered - the price based on the selected/base UoM */
public Decimal GetPriceEntered() 
{
Object bd =Get_Value("PriceEntered");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_ValueNoCheck ("Processed", Processed);
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
/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
public void SetQtyEntered (Decimal? QtyEntered)
{
if (QtyEntered == null) throw new ArgumentException ("QtyEntered is mandatory.");
Set_Value ("QtyEntered", (Decimal?)QtyEntered);
}
/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
public Decimal GetQtyEntered() 
{
Object bd =Get_Value("QtyEntered");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Tax Amount.
@param TaxAmt Tax Amount for a document */
public void SetTaxAmt (Decimal? TaxAmt)
{
if (TaxAmt == null) throw new ArgumentException ("TaxAmt is mandatory.");
Set_Value ("TaxAmt", (Decimal?)TaxAmt);
}
/** Get Tax Amount.
@return Tax Amount for a document */
public Decimal GetTaxAmt() 
{
Object bd =Get_Value("TaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
