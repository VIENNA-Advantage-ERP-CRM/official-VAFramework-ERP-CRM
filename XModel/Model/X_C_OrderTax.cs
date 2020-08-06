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
/** Generated Model for C_OrderTax
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_OrderTax : PO
{
public X_C_OrderTax (Context ctx, int C_OrderTax_ID, Trx trxName) : base (ctx, C_OrderTax_ID, trxName)
{
/** if (C_OrderTax_ID == 0)
{
SetC_Order_ID (0);
SetC_Tax_ID (0);
SetIsTaxIncluded (false);
SetProcessed (false);	// N
SetTaxAmt (0.0);
SetTaxBaseAmt (0.0);
}
 */
}
public X_C_OrderTax (Ctx ctx, int C_OrderTax_ID, Trx trxName) : base (ctx, C_OrderTax_ID, trxName)
{
/** if (C_OrderTax_ID == 0)
{
SetC_Order_ID (0);
SetC_Tax_ID (0);
SetIsTaxIncluded (false);
SetProcessed (false);	// N
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
public X_C_OrderTax (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_OrderTax (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_OrderTax (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_OrderTax()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373425L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056636L;
/** AD_Table_ID=314 */
public static int Table_ID;
 // =314;

/** TableName=C_OrderTax */
public static String Table_Name="C_OrderTax";

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
StringBuilder sb = new StringBuilder ("X_C_OrderTax[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID < 1) throw new ArgumentException ("C_Order_ID is mandatory.");
Set_ValueNoCheck ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Order_ID().ToString());
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
