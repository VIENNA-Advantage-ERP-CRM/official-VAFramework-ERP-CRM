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
/** Generated Model for VAB_BatchInvoiceLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_BatchInvoiceLine : PO
{
public X_VAB_BatchInvoiceLine (Context ctx, int VAB_BatchInvoiceLine_ID, Trx trxName) : base (ctx, VAB_BatchInvoiceLine_ID, trxName)
{
/** if (VAB_BatchInvoiceLine_ID == 0)
{
SetVAB_BusinessPartner_ID (0);	// @VAB_BusinessPartner_ID@
SetVAB_BPart_Location_ID (0);	// @VAB_BPart_Location_ID@
SetVAB_Charge_ID (0);
SetVAB_DocTypes_ID (0);	// @VAB_DocTypes_ID@
SetVAB_BatchInvoiceLine_ID (0);
SetVAB_BatchInvoice_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);	// @DateAcct@;
@DateDoc@
SetDateInvoiced (DateTime.Now);	// @DateInvoiced@;
@DateDoc@
SetDocumentNo (null);	// @DocumentNo@
SetIsTaxIncluded (false);	// @IsTaxIncluded@
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_BatchInvoiceLine WHERE VAB_BatchInvoice_ID=@VAB_BatchInvoice_ID@
SetLineNetAmt (0.0);
SetLineTotalAmt (0.0);
SetPriceEntered (0.0);
SetProcessed (false);	// N
SetQtyEntered (0.0);	// 1
SetTaxAmt (0.0);
}
 */
}
public X_VAB_BatchInvoiceLine (Ctx ctx, int VAB_BatchInvoiceLine_ID, Trx trxName) : base (ctx, VAB_BatchInvoiceLine_ID, trxName)
{
/** if (VAB_BatchInvoiceLine_ID == 0)
{
SetVAB_BusinessPartner_ID (0);	// @VAB_BusinessPartner_ID@
SetVAB_BPart_Location_ID (0);	// @VAB_BPart_Location_ID@
SetVAB_Charge_ID (0);
SetVAB_DocTypes_ID (0);	// @VAB_DocTypes_ID@
SetVAB_BatchInvoiceLine_ID (0);
SetVAB_BatchInvoice_ID (0);
SetC_Tax_ID (0);
SetDateAcct (DateTime.Now);	// @DateAcct@;
@DateDoc@
SetDateInvoiced (DateTime.Now);	// @DateInvoiced@;
@DateDoc@
SetDocumentNo (null);	// @DocumentNo@
SetIsTaxIncluded (false);	// @IsTaxIncluded@
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_BatchInvoiceLine WHERE VAB_BatchInvoice_ID=@VAB_BatchInvoice_ID@
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
public X_VAB_BatchInvoiceLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BatchInvoiceLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BatchInvoiceLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_BatchInvoiceLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372422L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055633L;
/** VAF_TableView_ID=768 */
public static int Table_ID;
 // =768;

/** TableName=VAB_BatchInvoiceLine */
public static String Table_Name="VAB_BatchInvoiceLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_BatchInvoiceLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_OrgTrx_ID VAF_Control_Ref_ID=130 */
public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID=130;
/** Set Trx Organization.
@param VAF_OrgTrx_ID Performing or initiating organization */
public void SetVAF_OrgTrx_ID (int VAF_OrgTrx_ID)
{
if (VAF_OrgTrx_ID <= 0) Set_Value ("VAF_OrgTrx_ID", null);
else
Set_Value ("VAF_OrgTrx_ID", VAF_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetVAF_OrgTrx_ID() 
{
Object ii = Get_Value("VAF_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID)
{
if (VAB_BillingCode_ID <= 0) Set_Value ("VAB_BillingCode_ID", null);
else
Set_Value ("VAB_BillingCode_ID", VAB_BillingCode_ID);
}
/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() 
{
Object ii = Get_Value("VAB_BillingCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Partner Location.
@param VAB_BPart_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetVAB_BPart_Location_ID (int VAB_BPart_Location_ID)
{
if (VAB_BPart_Location_ID < 1) throw new ArgumentException ("VAB_BPart_Location_ID is mandatory.");
Set_Value ("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetVAB_BPart_Location_ID() 
{
Object ii = Get_Value("VAB_BPart_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge.
@param VAB_Charge_ID Additional document charges */
public void SetVAB_Charge_ID (int VAB_Charge_ID)
{
if (VAB_Charge_ID < 1) throw new ArgumentException ("VAB_Charge_ID is mandatory.");
Set_Value ("VAB_Charge_ID", VAB_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetVAB_Charge_ID() 
{
Object ii = Get_Value("VAB_Charge_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param VAB_DocTypes_ID Document type or rules */
public void SetVAB_DocTypes_ID (int VAB_DocTypes_ID)
{
if (VAB_DocTypes_ID < 0) throw new ArgumentException ("VAB_DocTypes_ID is mandatory.");
Set_Value ("VAB_DocTypes_ID", VAB_DocTypes_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetVAB_DocTypes_ID() 
{
Object ii = Get_Value("VAB_DocTypes_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Batch Line.
@param VAB_BatchInvoiceLine_ID Expense Invoice Batch Line */
public void SetVAB_BatchInvoiceLine_ID (int VAB_BatchInvoiceLine_ID)
{
if (VAB_BatchInvoiceLine_ID < 1) throw new ArgumentException ("VAB_BatchInvoiceLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_BatchInvoiceLine_ID", VAB_BatchInvoiceLine_ID);
}
/** Get Invoice Batch Line.
@return Expense Invoice Batch Line */
public int GetVAB_BatchInvoiceLine_ID() 
{
Object ii = Get_Value("VAB_BatchInvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Batch.
@param VAB_BatchInvoice_ID Expense Invoice Batch Header */
public void SetVAB_BatchInvoice_ID (int VAB_BatchInvoice_ID)
{
if (VAB_BatchInvoice_ID < 1) throw new ArgumentException ("VAB_BatchInvoice_ID is mandatory.");
Set_ValueNoCheck ("VAB_BatchInvoice_ID", VAB_BatchInvoice_ID);
}
/** Get Invoice Batch.
@return Expense Invoice Batch Header */
public int GetVAB_BatchInvoice_ID() 
{
Object ii = Get_Value("VAB_BatchInvoice_ID");
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

/** User1_ID VAF_Control_Ref_ID=134 */
public static int USER1_ID_VAF_Control_Ref_ID=134;
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

/** User2_ID VAF_Control_Ref_ID=137 */
public static int USER2_ID_VAF_Control_Ref_ID=137;
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
