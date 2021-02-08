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
/** Generated Model for VAB_BPart_Product
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_BPart_Product : PO
{
public X_VAB_BPart_Product (Context ctx, int VAB_BPart_Product_ID, Trx trxName) : base (ctx, VAB_BPart_Product_ID, trxName)
{
/** if (VAB_BPart_Product_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAM_Product_ID (0);
SetShelfLifeMinDays (0);
SetShelfLifeMinPct (0);
}
 */
}
public X_VAB_BPart_Product (Ctx ctx, int VAB_BPart_Product_ID, Trx trxName) : base (ctx, VAB_BPart_Product_ID, trxName)
{
/** if (VAB_BPart_Product_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAM_Product_ID (0);
SetShelfLifeMinDays (0);
SetShelfLifeMinPct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BPart_Product (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BPart_Product (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BPart_Product (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_BPart_Product()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370589L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053800L;
/** VAF_TableView_ID=632 */
public static int Table_ID;
 // =632;

/** TableName=VAB_BPart_Product */
public static String Table_Name="VAB_BPart_Product";

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
StringBuilder sb = new StringBuilder ("X_VAB_BPart_Product[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_Product_ID().ToString());
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
/** Set Quality Rating.
@param QualityRating Method for rating vendors */
public void SetQualityRating (Decimal? QualityRating)
{
Set_Value ("QualityRating", (Decimal?)QualityRating);
}
/** Get Quality Rating.
@return Method for rating vendors */
public Decimal GetQualityRating() 
{
Object bd =Get_Value("QualityRating");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Min Shelf Life Days.
@param ShelfLifeMinDays Minimum Shelf Life in days based on Product Instance Guarantee Date */
public void SetShelfLifeMinDays (int ShelfLifeMinDays)
{
Set_Value ("ShelfLifeMinDays", ShelfLifeMinDays);
}
/** Get Min Shelf Life Days.
@return Minimum Shelf Life in days based on Product Instance Guarantee Date */
public int GetShelfLifeMinDays() 
{
Object ii = Get_Value("ShelfLifeMinDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Min Shelf Life %.
@param ShelfLifeMinPct Minimum Shelf Life in percent based on Product Instance Guarantee Date */
public void SetShelfLifeMinPct (int ShelfLifeMinPct)
{
Set_Value ("ShelfLifeMinPct", ShelfLifeMinPct);
}
/** Get Min Shelf Life %.
@return Minimum Shelf Life in percent based on Product Instance Guarantee Date */
public int GetShelfLifeMinPct() 
{
Object ii = Get_Value("ShelfLifeMinPct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
if (VendorProductNo != null && VendorProductNo.Length > 30)
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
