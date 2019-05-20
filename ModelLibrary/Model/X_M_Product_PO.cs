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
/** Generated Model for M_Product_PO
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Product_PO : PO
{
public X_M_Product_PO (Context ctx, int M_Product_PO_ID, Trx trxName) : base (ctx, M_Product_PO_ID, trxName)
{
/** if (M_Product_PO_ID == 0)
{
SetC_BPartner_ID (0);
SetIsCurrentVendor (true);	// Y
SetM_Product_ID (0);	// @M_Product_ID@
SetVendorProductNo (null);	// @Value@
}
 */
}
public X_M_Product_PO (Ctx ctx, int M_Product_PO_ID, Trx trxName) : base (ctx, M_Product_PO_ID, trxName)
{
/** if (M_Product_PO_ID == 0)
{
SetC_BPartner_ID (0);
SetIsCurrentVendor (true);	// Y
SetM_Product_ID (0);	// @M_Product_ID@
SetVendorProductNo (null);	// @Value@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_PO (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_PO (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_PO (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Product_PO()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380917L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064128L;
/** AD_Table_ID=210 */
public static int Table_ID;
 // =210;

/** TableName=M_Product_PO */
public static String Table_Name="M_Product_PO";

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
StringBuilder sb = new StringBuilder ("X_M_Product_PO[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID <= 0) Set_Value ("C_UOM_ID", null);
else
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
/** Set Cost per Order.
@param CostPerOrder Fixed Cost Per Order */
public void SetCostPerOrder (Decimal? CostPerOrder)
{
Set_Value ("CostPerOrder", (Decimal?)CostPerOrder);
}
/** Get Cost per Order.
@return Fixed Cost Per Order */
public Decimal GetCostPerOrder() 
{
Object bd =Get_Value("CostPerOrder");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Actual Delivery Time.
@param DeliveryTime_Actual Actual days between order and delivery */
public void SetDeliveryTime_Actual (int DeliveryTime_Actual)
{
Set_Value ("DeliveryTime_Actual", DeliveryTime_Actual);
}
/** Get Actual Delivery Time.
@return Actual days between order and delivery */
public int GetDeliveryTime_Actual() 
{
Object ii = Get_Value("DeliveryTime_Actual");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Promised Delivery Time.
@param DeliveryTime_Promised Promised days between order and delivery */
public void SetDeliveryTime_Promised (int DeliveryTime_Promised)
{
Set_Value ("DeliveryTime_Promised", DeliveryTime_Promised);
}
/** Get Promised Delivery Time.
@return Promised days between order and delivery */
public int GetDeliveryTime_Promised() 
{
Object ii = Get_Value("DeliveryTime_Promised");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Discontinued.
@param Discontinued This product is no longer available */
public void SetDiscontinued (Boolean Discontinued)
{
Set_Value ("Discontinued", Discontinued);
}
/** Get Discontinued.
@return This product is no longer available */
public Boolean IsDiscontinued() 
{
Object oo = Get_Value("Discontinued");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Discontinued by.
@param DiscontinuedBy Discontinued By */
public void SetDiscontinuedBy (DateTime? DiscontinuedBy)
{
Set_Value ("DiscontinuedBy", (DateTime?)DiscontinuedBy);
}
/** Get Discontinued by.
@return Discontinued By */
public DateTime? GetDiscontinuedBy() 
{
return (DateTime?)Get_Value("DiscontinuedBy");
}
/** Set Current vendor.
@param IsCurrentVendor Use this Vendor for pricing and stock replenishment */
public void SetIsCurrentVendor (Boolean IsCurrentVendor)
{
Set_Value ("IsCurrentVendor", IsCurrentVendor);
}
/** Get Current vendor.
@return Use this Vendor for pricing and stock replenishment */
public Boolean IsCurrentVendor() 
{
Object oo = Get_Value("IsCurrentVendor");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Product_ID().ToString());
}
/** Set Manufacturer.
@param Manufacturer Manufacturer of the Product */
public void SetManufacturer (String Manufacturer)
{
if (Manufacturer != null && Manufacturer.Length > 30)
{
log.Warning("Length > 30 - truncated");
Manufacturer = Manufacturer.Substring(0,30);
}
Set_Value ("Manufacturer", Manufacturer);
}
/** Get Manufacturer.
@return Manufacturer of the Product */
public String GetManufacturer() 
{
return (String)Get_Value("Manufacturer");
}
/** Set Minimum Order Qty.
@param Order_Min Minimum order quantity in UOM */
public void SetOrder_Min (Decimal? Order_Min)
{
Set_Value ("Order_Min", (Decimal?)Order_Min);
}
/** Get Minimum Order Qty.
@return Minimum order quantity in UOM */
public Decimal GetOrder_Min() 
{
Object bd =Get_Value("Order_Min");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Pack Qty.
@param Order_Pack Package order size in UOM (e.g. order set of 5 units) */
public void SetOrder_Pack (Decimal? Order_Pack)
{
Set_Value ("Order_Pack", (Decimal?)Order_Pack);
}
/** Get Order Pack Qty.
@return Package order size in UOM (e.g. order set of 5 units) */
public Decimal GetOrder_Pack() 
{
Object bd =Get_Value("Order_Pack");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Price effective.
@param PriceEffective Effective Date of Price */
public void SetPriceEffective (DateTime? PriceEffective)
{
Set_Value ("PriceEffective", (DateTime?)PriceEffective);
}
/** Get Price effective.
@return Effective Date of Price */
public DateTime? GetPriceEffective() 
{
return (DateTime?)Get_Value("PriceEffective");
}
/** Set Last Invoice Price.
@param PriceLastInv Price of the last invoice for the product */
public void SetPriceLastInv (Decimal? PriceLastInv)
{
Set_ValueNoCheck ("PriceLastInv", (Decimal?)PriceLastInv);
}
/** Get Last Invoice Price.
@return Price of the last invoice for the product */
public Decimal GetPriceLastInv() 
{
Object bd =Get_Value("PriceLastInv");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Last PO Price.
@param PriceLastPO Price of the last purchase order for the product */
public void SetPriceLastPO (Decimal? PriceLastPO)
{
Set_ValueNoCheck ("PriceLastPO", (Decimal?)PriceLastPO);
}
/** Get Last PO Price.
@return Price of the last purchase order for the product */
public Decimal GetPriceLastPO() 
{
Object bd =Get_Value("PriceLastPO");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List Price.
@param PriceList List Price */
public void SetPriceList (Decimal? PriceList)
{
Set_Value ("PriceList", (Decimal?)PriceList);
}
/** Get List Price.
@return List Price */
public Decimal GetPriceList() 
{
Object bd =Get_Value("PriceList");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set PO Price.
@param PricePO Price based on a purchase order */
public void SetPricePO (Decimal? PricePO)
{
Set_Value ("PricePO", (Decimal?)PricePO);
}
/** Get PO Price.
@return Price based on a purchase order */
public Decimal GetPricePO() 
{
Object bd =Get_Value("PricePO");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quality Rating.
@param QualityRating Method for rating vendors */
public void SetQualityRating (int QualityRating)
{
Set_Value ("QualityRating", QualityRating);
}
/** Get Quality Rating.
@return Method for rating vendors */
public int GetQualityRating() 
{
Object ii = Get_Value("QualityRating");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Royalty Amount.
@param RoyaltyAmt (Included) Amount for copyright, etc. */
public void SetRoyaltyAmt (Decimal? RoyaltyAmt)
{
Set_Value ("RoyaltyAmt", (Decimal?)RoyaltyAmt);
}
/** Get Royalty Amount.
@return (Included) Amount for copyright, etc. */
public Decimal GetRoyaltyAmt() 
{
Object bd =Get_Value("RoyaltyAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC (String UPC)
{
if (UPC != null && UPC.Length > 20)
{
log.Warning("Length > 20 - truncated");
UPC = UPC.Substring(0,20);
}
Set_Value ("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC() 
{
return (String)Get_Value("UPC");
}
/** Set Partner Category.
@param VendorCategory Product Category of the Business Partner */
public void SetVendorCategory (String VendorCategory)
{
if (VendorCategory != null && VendorCategory.Length > 30)
{
log.Warning("Length > 30 - truncated");
VendorCategory = VendorCategory.Substring(0,30);
}
Set_Value ("VendorCategory", VendorCategory);
}
/** Get Partner Category.
@return Product Category of the Business Partner */
public String GetVendorCategory() 
{
return (String)Get_Value("VendorCategory");
}
/** Set Partner Product Key.
@param VendorProductNo Product Key of the Business Partner */
public void SetVendorProductNo (String VendorProductNo)
{
if (VendorProductNo == null) throw new ArgumentException ("VendorProductNo is mandatory.");
if (VendorProductNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
VendorProductNo = VendorProductNo.Substring(0,30);
}
Set_Value ("VendorProductNo", VendorProductNo);
}
/** Get Partner Product Key.
@return Product Key of the Business Partner */
public String GetVendorProductNo() 
{
return (String)Get_Value("VendorProductNo");
}
}

}
