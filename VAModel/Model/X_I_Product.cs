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
/** Generated Model for I_Product
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_Product : PO
{
public X_I_Product (Context ctx, int I_Product_ID, Trx trxName) : base (ctx, I_Product_ID, trxName)
{
/** if (I_Product_ID == 0)
{
SetI_IsImported (null);	// N
SetI_Product_ID (0);
}
 */
}
public X_I_Product (Ctx ctx, int I_Product_ID, Trx trxName) : base (ctx, I_Product_ID, trxName)
{
/** if (I_Product_ID == 0)
{
SetI_IsImported (null);	// N
SetI_Product_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Product (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Product (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Product (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_Product()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377532L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060743L;
/** AD_Table_ID=532 */
public static int Table_ID;
 // =532;

/** TableName=I_Product */
public static String Table_Name="I_Product";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_I_Product[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner Key.
@param BPartner_Value The Key of the Business Partner */
public void SetBPartner_Value (String BPartner_Value)
{
if (BPartner_Value != null && BPartner_Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
BPartner_Value = BPartner_Value.Substring(0,40);
}
Set_Value ("BPartner_Value", BPartner_Value);
}
/** Get Business Partner Key.
@return The Key of the Business Partner */
public String GetBPartner_Value() 
{
return (String)Get_Value("BPartner_Value");
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
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
/** Set Classification.
@param Classification Classification for grouping */
public void SetClassification (String Classification)
{
if (Classification != null && Classification.Length > 1)
{
log.Warning("Length > 1 - truncated");
Classification = Classification.Substring(0,1);
}
Set_Value ("Classification", Classification);
}
/** Get Classification.
@return Classification for grouping */
public String GetClassification() 
{
return (String)Get_Value("Classification");
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
/** Set Description URL.
@param DescriptionURL URL for the description */
public void SetDescriptionURL (String DescriptionURL)
{
if (DescriptionURL != null && DescriptionURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
DescriptionURL = DescriptionURL.Substring(0,120);
}
Set_Value ("DescriptionURL", DescriptionURL);
}
/** Get Description URL.
@return URL for the description */
public String GetDescriptionURL() 
{
return (String)Get_Value("DescriptionURL");
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
/** Set Document Note.
@param DocumentNote Additional information for a Document */
public void SetDocumentNote (String DocumentNote)
{
if (DocumentNote != null && DocumentNote.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DocumentNote = DocumentNote.Substring(0,2000);
}
Set_Value ("DocumentNote", DocumentNote);
}
/** Get Document Note.
@return Additional information for a Document */
public String GetDocumentNote() 
{
return (String)Get_Value("DocumentNote");
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set ISO Currency Code.
@param ISO_Code Three letter ISO 4217 Code of the Currency */
public void SetISO_Code (String ISO_Code)
{
if (ISO_Code != null && ISO_Code.Length > 3)
{
log.Warning("Length > 3 - truncated");
ISO_Code = ISO_Code.Substring(0,3);
}
Set_Value ("ISO_Code", ISO_Code);
}
/** Get ISO Currency Code.
@return Three letter ISO 4217 Code of the Currency */
public String GetISO_Code() 
{
return (String)Get_Value("ISO_Code");
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}

/** I_IsImported AD_Reference_ID=420 */
public static int I_ISIMPORTED_AD_Reference_ID=420;
/** Error = E */
public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid (String test)
{
return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (String I_IsImported)
{
if (I_IsImported == null) throw new ArgumentException ("I_IsImported is mandatory");
if (!IsI_IsImportedValid(I_IsImported))
throw new ArgumentException ("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
if (I_IsImported.Length > 1)
{
log.Warning("Length > 1 - truncated");
I_IsImported = I_IsImported.Substring(0,1);
}
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public String GetI_IsImported() 
{
return (String)Get_Value("I_IsImported");
}
/** Set Import Product.
@param I_Product_ID Import Item or Service */
public void SetI_Product_ID (int I_Product_ID)
{
if (I_Product_ID < 1) throw new ArgumentException ("I_Product_ID is mandatory.");
Set_ValueNoCheck ("I_Product_ID", I_Product_ID);
}
/** Get Import Product.
@return Import Item or Service */
public int GetI_Product_ID() 
{
Object ii = Get_Value("I_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image URL.
@param ImageURL URL of  image */
public void SetImageURL (String ImageURL)
{
if (ImageURL != null && ImageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
ImageURL = ImageURL.Substring(0,120);
}
Set_Value ("ImageURL", ImageURL);
}
/** Get Image URL.
@return URL of  image */
public String GetImageURL() 
{
return (String)Get_Value("ImageURL");
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID <= 0) Set_Value ("M_Product_Category_ID", null);
else
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Minimum Order Qty.
@param Order_Min Minimum order quantity in UOM */
public void SetOrder_Min (int Order_Min)
{
Set_Value ("Order_Min", Order_Min);
}
/** Get Minimum Order Qty.
@return Minimum order quantity in UOM */
public int GetOrder_Min() 
{
Object ii = Get_Value("Order_Min");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order Pack Qty.
@param Order_Pack Package order size in UOM (e.g. order set of 5 units) */
public void SetOrder_Pack (int Order_Pack)
{
Set_Value ("Order_Pack", Order_Pack);
}
/** Get Order Pack Qty.
@return Package order size in UOM (e.g. order set of 5 units) */
public int GetOrder_Pack() 
{
Object ii = Get_Value("Order_Pack");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Limit Price.
@param PriceLimit Lowest price for a product */
public void SetPriceLimit (Decimal? PriceLimit)
{
Set_Value ("PriceLimit", (Decimal?)PriceLimit);
}
/** Get Limit Price.
@return Lowest price for a product */
public Decimal GetPriceLimit() 
{
Object bd =Get_Value("PriceLimit");
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
/** Set Standard Price.
@param PriceStd Standard Price */
public void SetPriceStd (Decimal? PriceStd)
{
Set_Value ("PriceStd", (Decimal?)PriceStd);
}
/** Get Standard Price.
@return Standard Price */
public Decimal GetPriceStd() 
{
Object bd =Get_Value("PriceStd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product Category Key.
@param ProductCategory_Value Product Category Key */
public void SetProductCategory_Value (String ProductCategory_Value)
{
if (ProductCategory_Value != null && ProductCategory_Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProductCategory_Value = ProductCategory_Value.Substring(0,40);
}
Set_Value ("ProductCategory_Value", ProductCategory_Value);
}
/** Get Product Category Key.
@return Product Category Key */
public String GetProductCategory_Value() 
{
return (String)Get_Value("ProductCategory_Value");
}

/** ProductType AD_Reference_ID=270 */
public static int PRODUCTTYPE_AD_Reference_ID=270;
/** Expense type = E */
public static String PRODUCTTYPE_ExpenseType = "E";
/** Item = I */
public static String PRODUCTTYPE_Item = "I";
/** Online = O */
public static String PRODUCTTYPE_Online = "O";
/** Resource = R */
public static String PRODUCTTYPE_Resource = "R";
/** Service = S */
public static String PRODUCTTYPE_Service = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsProductTypeValid (String test)
{
return test == null || test.Equals("E") || test.Equals("I") || test.Equals("O") || test.Equals("R") || test.Equals("S");
}
/** Set Product Type.
@param ProductType Type of product */
public void SetProductType (String ProductType)
{
if (!IsProductTypeValid(ProductType))
throw new ArgumentException ("ProductType Invalid value - " + ProductType + " - Reference_ID=270 - E - I - O - R - S");
if (ProductType != null && ProductType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ProductType = ProductType.Substring(0,1);
}
Set_Value ("ProductType", ProductType);
}
/** Get Product Type.
@return Type of product */
public String GetProductType() 
{
return (String)Get_Value("ProductType");
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
/** Set SKU.
@param SKU Stock Keeping Unit */
public void SetSKU (String SKU)
{
if (SKU != null && SKU.Length > 30)
{
log.Warning("Length > 30 - truncated");
SKU = SKU.Substring(0,30);
}
Set_Value ("SKU", SKU);
}
/** Get SKU.
@return Stock Keeping Unit */
public String GetSKU() 
{
return (String)Get_Value("SKU");
}
/** Set Shelf Depth.
@param ShelfDepth Shelf depth required */
public void SetShelfDepth (int ShelfDepth)
{
Set_Value ("ShelfDepth", ShelfDepth);
}
/** Get Shelf Depth.
@return Shelf depth required */
public int GetShelfDepth() 
{
Object ii = Get_Value("ShelfDepth");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shelf Height.
@param ShelfHeight Shelf height required */
public void SetShelfHeight (int ShelfHeight)
{
Set_Value ("ShelfHeight", ShelfHeight);
}
/** Get Shelf Height.
@return Shelf height required */
public int GetShelfHeight() 
{
Object ii = Get_Value("ShelfHeight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shelf Width.
@param ShelfWidth Shelf width required */
public void SetShelfWidth (int ShelfWidth)
{
Set_Value ("ShelfWidth", ShelfWidth);
}
/** Get Shelf Width.
@return Shelf width required */
public int GetShelfWidth() 
{
Object ii = Get_Value("ShelfWidth");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC (String UPC)
{
if (UPC != null && UPC.Length > 30)
{
log.Warning("Length > 30 - truncated");
UPC = UPC.Substring(0,30);
}
Set_Value ("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC() 
{
return (String)Get_Value("UPC");
}
/** Set Units Per Pallet.
@param UnitsPerPallet Units Per Pallet */
public void SetUnitsPerPallet (int UnitsPerPallet)
{
Set_Value ("UnitsPerPallet", UnitsPerPallet);
}
/** Get Units Per Pallet.
@return Units Per Pallet */
public int GetUnitsPerPallet() 
{
Object ii = Get_Value("UnitsPerPallet");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetValue());
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
/** Set Volume.
@param Volume Volume of a product */
public void SetVolume (int Volume)
{
Set_Value ("Volume", Volume);
}
/** Get Volume.
@return Volume of a product */
public int GetVolume() 
{
Object ii = Get_Value("Volume");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Weight.
@param Weight Weight of a product */
public void SetWeight (int Weight)
{
Set_Value ("Weight", Weight);
}
/** Get Weight.
@return Weight of a product */
public int GetWeight() 
{
Object ii = Get_Value("Weight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM Code.
@param X12DE355 UOM EDI X12 Code */
public void SetX12DE355 (String X12DE355)
{
if (X12DE355 != null && X12DE355.Length > 2)
{
log.Warning("Length > 2 - truncated");
X12DE355 = X12DE355.Substring(0,2);
}
Set_Value ("X12DE355", X12DE355);
}
/** Get UOM Code.
@return UOM EDI X12 Code */
public String GetX12DE355() 
{
return (String)Get_Value("X12DE355");
}

/** Set ISLOTNO.
@param ISLOTNO ISLOTNO */
public void SetISLOTNO(Boolean ISLOTNO)
{
    Set_Value("ISLOTNO", ISLOTNO);
}
/** Get ISLOTNO.
@return ISLOTNO */
public Boolean IsLOTNO()
{
    Object oo = Get_Value("ISLOTNO");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}

/** Set ISSERIALNO.
@param ISSERIALNO ISSERIALNO */
public void SetISSERIALNO(Boolean ISSERIALNO)
{
    Set_Value("ISSERIALNO", ISSERIALNO);
}
/** Get ISSERIALNO.
@return ISSERIALNO */
public Boolean IsSERIALNO()
{
    Object oo = Get_Value("ISSERIALNO");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}

/** Set Purchased.
@param IsPurchased Organization purchases this product */
public void SetIsPurchased(Boolean IsPurchased)
{
    Set_Value("IsPurchased", IsPurchased);
}
/** Get Purchased.
@return Organization purchases this product */
public Boolean IsPurchased()
{
    Object oo = Get_Value("IsPurchased");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Sold.
@param IsSold Organization sells this product */
public void SetIsSold(Boolean IsSold)
{
    Set_Value("IsSold", IsSold);
}
/** Get Sold.
@return Organization sells this product */
public Boolean IsSold()
{
    Object oo = Get_Value("IsSold");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Stocked.
@param IsStocked Organization stocks this product */
public void SetIsStocked(Boolean IsStocked)
{
    Set_Value("IsStocked", IsStocked);
}
/** Get Stocked.
@return Organization stocks this product */
public Boolean IsStocked()
{
    Object oo = Get_Value("IsStocked");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}

/** Set PRODUCTCOST.
@param PRODUCTCOST PRODUCTCOST */
public void SetPRODUCTCOST(Decimal? PRODUCTCOST)
{
    Set_Value("PRODUCTCOST", (Decimal?)PRODUCTCOST);
}
/** Get PRODUCTCOST.
@return PRODUCTCOST */
public Decimal GetPRODUCTCOST()
{
    Object bd = Get_Value("PRODUCTCOST");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}

/** Set STOCKQUANTITIES.
@param STOCKQUANTITIES STOCKQUANTITIES */
public void SetSTOCKQUANTITIES(Decimal? STOCKQUANTITIES)
{
    Set_Value("STOCKQUANTITIES", (Decimal?)STOCKQUANTITIES);
}
/** Get STOCKQUANTITIES.
@return STOCKQUANTITIES */
public Decimal GetSTOCKQUANTITIES()
{
    Object bd = Get_Value("STOCKQUANTITIES");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set STOCKVALUE.
@param STOCKVALUE STOCKVALUE */
public void SetSTOCKVALUE(Decimal? STOCKVALUE)
{
    Set_Value("STOCKVALUE", (Decimal?)STOCKVALUE);
}
/** Get STOCKVALUE.
@return STOCKVALUE */
public Decimal GetSTOCKVALUE()
{
    Object bd = Get_Value("STOCKVALUE");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}

/** Set TAXCATEGORY_VALUE.
@param TAXCATEGORY_VALUE TAXCATEGORY_VALUE */
public void SetTAXCATEGORY_VALUE(String TAXCATEGORY_VALUE)
{
    if (TAXCATEGORY_VALUE != null && TAXCATEGORY_VALUE.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        TAXCATEGORY_VALUE = TAXCATEGORY_VALUE.Substring(0, 50);
    }
    Set_Value("TAXCATEGORY_VALUE", TAXCATEGORY_VALUE);
}
/** Get TAXCATEGORY_VALUE.
@return TAXCATEGORY_VALUE */
public String GetTAXCATEGORY_VALUE()
{
    return (String)Get_Value("TAXCATEGORY_VALUE");
}

}

}
