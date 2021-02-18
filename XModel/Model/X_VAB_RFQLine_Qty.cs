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
/** Generated Model for VAB_RFQLine_Qty
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RFQLine_Qty : PO
{
public X_VAB_RFQLine_Qty (Context ctx, int VAB_RFQLine_Qty_ID, Trx trxName) : base (ctx, VAB_RFQLine_Qty_ID, trxName)
{
/** if (VAB_RFQLine_Qty_ID == 0)
{
SetBenchmarkPrice (0.0);
SetVAB_RFQLine_Qty_ID (0);
SetVAB_RFQLine_ID (0);
SetVAB_UOM_ID (0);
SetIsOfferQty (false);
SetIsPurchaseQty (false);
SetIsRfQQty (true);	// Y
SetQty (0.0);	// 1
}
 */
}
public X_VAB_RFQLine_Qty (Ctx ctx, int VAB_RFQLine_Qty_ID, Trx trxName) : base (ctx, VAB_RFQLine_Qty_ID, trxName)
{
/** if (VAB_RFQLine_Qty_ID == 0)
{
SetBenchmarkPrice (0.0);
SetVAB_RFQLine_Qty_ID (0);
SetVAB_RFQLine_ID (0);
SetVAB_UOM_ID (0);
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
public X_VAB_RFQLine_Qty (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQLine_Qty (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQLine_Qty (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RFQLine_Qty()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374773L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057984L;
/** VAF_TableView_ID=675 */
public static int Table_ID;
 // =675;

/** TableName=VAB_RFQLine_Qty */
public static String Table_Name="VAB_RFQLine_Qty";

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
StringBuilder sb = new StringBuilder ("X_VAB_RFQLine_Qty[").Append(Get_ID()).Append("]");
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
@param VAB_RFQLine_Qty_ID Request for Quotation Line Quantity */
public void SetVAB_RFQLine_Qty_ID (int VAB_RFQLine_Qty_ID)
{
if (VAB_RFQLine_Qty_ID < 1) throw new ArgumentException ("VAB_RFQLine_Qty_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQLine_Qty_ID", VAB_RFQLine_Qty_ID);
}
/** Get RfQ Line Quantity.
@return Request for Quotation Line Quantity */
public int GetVAB_RFQLine_Qty_ID() 
{
Object ii = Get_Value("VAB_RFQLine_Qty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Line.
@param VAB_RFQLine_ID Request for Quotation Line */
public void SetVAB_RFQLine_ID (int VAB_RFQLine_ID)
{
if (VAB_RFQLine_ID < 1) throw new ArgumentException ("VAB_RFQLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQLine_ID", VAB_RFQLine_ID);
}
/** Get RfQ Line.
@return Request for Quotation Line */
public int GetVAB_RFQLine_ID() 
{
Object ii = Get_Value("VAB_RFQLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param VAB_UOM_ID Unit of Measure */
public void SetVAB_UOM_ID (int VAB_UOM_ID)
{
if (VAB_UOM_ID < 1) throw new ArgumentException ("VAB_UOM_ID is mandatory.");
Set_Value ("VAB_UOM_ID", VAB_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetVAB_UOM_ID() 
{
Object ii = Get_Value("VAB_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_UOM_ID().ToString());
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
