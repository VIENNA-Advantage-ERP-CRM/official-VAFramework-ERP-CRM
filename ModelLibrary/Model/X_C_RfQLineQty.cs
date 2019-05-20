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
/** Generated Model for C_RfQLineQty
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQLineQty : PO
{
public X_C_RfQLineQty (Context ctx, int C_RfQLineQty_ID, Trx trxName) : base (ctx, C_RfQLineQty_ID, trxName)
{
/** if (C_RfQLineQty_ID == 0)
{
SetBenchmarkPrice (0.0);
SetC_RfQLineQty_ID (0);
SetC_RfQLine_ID (0);
SetC_UOM_ID (0);
SetIsOfferQty (false);
SetIsPurchaseQty (false);
SetIsRfQQty (true);	// Y
SetQty (0.0);	// 1
}
 */
}
public X_C_RfQLineQty (Ctx ctx, int C_RfQLineQty_ID, Trx trxName) : base (ctx, C_RfQLineQty_ID, trxName)
{
/** if (C_RfQLineQty_ID == 0)
{
SetBenchmarkPrice (0.0);
SetC_RfQLineQty_ID (0);
SetC_RfQLine_ID (0);
SetC_UOM_ID (0);
SetIsOfferQty (false);
SetIsPurchaseQty (false);
SetIsRfQQty (true);	// Y
SetQty (0.0);	// 1
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLineQty (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLineQty (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQLineQty (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQLineQty()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374773L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057984L;
/** AD_Table_ID=675 */
public static int Table_ID;
 // =675;

/** TableName=C_RfQLineQty */
public static String Table_Name="C_RfQLineQty";

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
StringBuilder sb = new StringBuilder ("X_C_RfQLineQty[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Benchmark Price.
@param BenchmarkPrice Price to compare responses to */
public void SetBenchmarkPrice (Decimal? BenchmarkPrice)
{
if (BenchmarkPrice == null) throw new ArgumentException ("BenchmarkPrice is mandatory.");
Set_Value ("BenchmarkPrice", (Decimal?)BenchmarkPrice);
}
/** Get Benchmark Price.
@return Price to compare responses to */
public Decimal GetBenchmarkPrice() 
{
Object bd =Get_Value("BenchmarkPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Best Response Amount.
@param BestResponseAmt Best Response Amount */
public void SetBestResponseAmt (Decimal? BestResponseAmt)
{
Set_Value ("BestResponseAmt", (Decimal?)BestResponseAmt);
}
/** Get Best Response Amount.
@return Best Response Amount */
public Decimal GetBestResponseAmt() 
{
Object bd =Get_Value("BestResponseAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set RfQ Line Quantity.
@param C_RfQLineQty_ID Request for Quotation Line Quantity */
public void SetC_RfQLineQty_ID (int C_RfQLineQty_ID)
{
if (C_RfQLineQty_ID < 1) throw new ArgumentException ("C_RfQLineQty_ID is mandatory.");
Set_ValueNoCheck ("C_RfQLineQty_ID", C_RfQLineQty_ID);
}
/** Get RfQ Line Quantity.
@return Request for Quotation Line Quantity */
public int GetC_RfQLineQty_ID() 
{
Object ii = Get_Value("C_RfQLineQty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Line.
@param C_RfQLine_ID Request for Quotation Line */
public void SetC_RfQLine_ID (int C_RfQLine_ID)
{
if (C_RfQLine_ID < 1) throw new ArgumentException ("C_RfQLine_ID is mandatory.");
Set_ValueNoCheck ("C_RfQLine_ID", C_RfQLine_ID);
}
/** Get RfQ Line.
@return Request for Quotation Line */
public int GetC_RfQLine_ID() 
{
Object ii = Get_Value("C_RfQLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_UOM_ID().ToString());
}
/** Set Offer Quantity.
@param IsOfferQty This quantity is used in the Offer to the Customer */
public void SetIsOfferQty (Boolean IsOfferQty)
{
Set_Value ("IsOfferQty", IsOfferQty);
}
/** Get Offer Quantity.
@return This quantity is used in the Offer to the Customer */
public Boolean IsOfferQty() 
{
Object oo = Get_Value("IsOfferQty");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Purchase Quantity.
@param IsPurchaseQty This quantity is used in the Purchase Order to the Supplier */
public void SetIsPurchaseQty (Boolean IsPurchaseQty)
{
Set_Value ("IsPurchaseQty", IsPurchaseQty);
}
/** Get Purchase Quantity.
@return This quantity is used in the Purchase Order to the Supplier */
public Boolean IsPurchaseQty() 
{
Object oo = Get_Value("IsPurchaseQty");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set RfQ Quantity.
@param IsRfQQty The quantity is used when generating RfQ Responses */
public void SetIsRfQQty (Boolean IsRfQQty)
{
Set_Value ("IsRfQQty", IsRfQQty);
}
/** Get RfQ Quantity.
@return The quantity is used when generating RfQ Responses */
public Boolean IsRfQQty() 
{
Object oo = Get_Value("IsRfQQty");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Margin %.
@param Margin Margin for a product as a percentage */
public void SetMargin (Decimal? Margin)
{
Set_Value ("Margin", (Decimal?)Margin);
}
/** Get Margin %.
@return Margin for a product as a percentage */
public Decimal GetMargin() 
{
Object bd =Get_Value("Margin");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Offer Amount.
@param OfferAmt Amount of the Offer */
public void SetOfferAmt (Decimal? OfferAmt)
{
Set_Value ("OfferAmt", (Decimal?)OfferAmt);
}
/** Get Offer Amount.
@return Amount of the Offer */
public Decimal GetOfferAmt() 
{
Object bd =Get_Value("OfferAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
