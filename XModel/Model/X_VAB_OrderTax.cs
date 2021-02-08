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
/** Generated Model for VAB_OrderTax
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_OrderTax : PO
{
public X_VAB_OrderTax (Context ctx, int VAB_OrderTax_ID, Trx trxName) : base (ctx, VAB_OrderTax_ID, trxName)
{
/** if (VAB_OrderTax_ID == 0)
{
SetVAB_Order_ID (0);
SetVAB_TaxRate_ID (0);
SetIsTaxIncluded (false);
SetProcessed (false);	// N
SetTaxAmt (0.0);
SetTaxBaseAmt (0.0);
}
 */
}
public X_VAB_OrderTax (Ctx ctx, int VAB_OrderTax_ID, Trx trxName) : base (ctx, VAB_OrderTax_ID, trxName)
{
/** if (VAB_OrderTax_ID == 0)
{
SetVAB_Order_ID (0);
SetVAB_TaxRate_ID (0);
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
public X_VAB_OrderTax (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_OrderTax (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_OrderTax (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_OrderTax()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373425L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056636L;
/** VAF_TableView_ID=314 */
public static int Table_ID;
 // =314;

/** TableName=VAB_OrderTax */
public static String Table_Name="VAB_OrderTax";

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
StringBuilder sb = new StringBuilder ("X_VAB_OrderTax[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Order.
@param VAB_Order_ID Order */
public void SetVAB_Order_ID (int VAB_Order_ID)
{
if (VAB_Order_ID < 1) throw new ArgumentException ("VAB_Order_ID is mandatory.");
Set_ValueNoCheck ("VAB_Order_ID", VAB_Order_ID);
}
/** Get Order.
@return Order */
public int GetVAB_Order_ID() 
{
Object ii = Get_Value("VAB_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_Order_ID().ToString());
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
