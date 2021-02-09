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
/** Generated Model for VAM_ProductCost
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ProductCost : PO
{
public X_VAM_ProductCost (Context ctx, int VAM_ProductCost_ID, Trx trxName) : base (ctx, VAM_ProductCost_ID, trxName)
{
/** if (VAM_ProductCost_ID == 0)
{
SetA_Asset_ID (0);	// 0
SetBasisType (null);	// I
SetVAB_AccountBook_ID (0);
SetCurrentCostPrice (0.0);
SetCurrentQty (0.0);
SetIsThisLevel (null);	// Y
SetLastCostPrice (0.0);	// 0
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_ProductCostElement_ID (0);
SetVAM_ProductCostType_ID (0);
SetVAM_Product_ID (0);
}
 */
}
public X_VAM_ProductCost (Ctx ctx, int VAM_ProductCost_ID, Trx trxName) : base (ctx, VAM_ProductCost_ID, trxName)
{
/** if (VAM_ProductCost_ID == 0)
{
SetA_Asset_ID (0);	// 0
SetBasisType (null);	// I
SetVAB_AccountBook_ID (0);
SetCurrentCostPrice (0.0);
SetCurrentQty (0.0);
SetIsThisLevel (null);	// Y
SetLastCostPrice (0.0);	// 0
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_ProductCostElement_ID (0);
SetVAM_ProductCostType_ID (0);
SetVAM_Product_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCost (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCost (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCost (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ProductCost()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27694963715596L;
/** Last Updated Timestamp 10/9/2014 12:36:39 PM */
public static long updatedMS = 1412838398807L;
/** VAF_TableView_ID=771 */
public static int Table_ID;
 // =771;

/** TableName=VAM_ProductCost */
public static String Table_Name="VAM_ProductCost";

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
StringBuilder sb = new StringBuilder ("X_VAM_ProductCost[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int VAA_Asset_ID)
{
if (VAA_Asset_ID < 1) throw new ArgumentException ("VAA_Asset_ID is mandatory.");
Set_ValueNoCheck ("VAA_Asset_ID", VAA_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("VAA_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** BasisType VAF_Control_Ref_ID=1000032 */
public static int BASISTYPE_VAF_Control_Ref_ID=1000032;
/** Per Batch = B */
public static String BASISTYPE_PerBatch = "B";
/** Per Item = I */
public static String BASISTYPE_PerItem = "I";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBasisTypeValid (String test)
{
return test.Equals("B") || test.Equals("I");
}
/** Set Cost Basis Type.
@param BasisType Indicates the option to consume and charge materials and resources */
public void SetBasisType (String BasisType)
{
if (BasisType == null) throw new ArgumentException ("BasisType is mandatory");
if (!IsBasisTypeValid(BasisType))
throw new ArgumentException ("BasisType Invalid value - " + BasisType + " - Reference_ID=1000032 - B - I");
if (BasisType.Length > 1)
{
log.Warning("Length > 1 - truncated");
BasisType = BasisType.Substring(0,1);
}
Set_ValueNoCheck ("BasisType", BasisType);
}
/** Get Cost Basis Type.
@return Indicates the option to consume and charge materials and resources */
public String GetBasisType() 
{
return (String)Get_Value("BasisType");
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param VAB_UOM_ID Unit of Measure */
public void SetVAB_UOM_ID (int VAB_UOM_ID)
{
throw new ArgumentException ("VAB_UOM_ID Is virtual column");
}
/** Get UOM.
@return Unit of Measure */
public int GetVAB_UOM_ID() 
{
Object ii = Get_Value("VAB_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CostingMethod VAF_Control_Ref_ID=122 */
public static int COSTINGMETHOD_VAF_Control_Ref_ID=122;
/** Average PO = A */
public static String COSTINGMETHOD_AveragePO = "A";
/** Fifo = F */
public static String COSTINGMETHOD_Fifo = "F";
/** Average Invoice = I */
public static String COSTINGMETHOD_AverageInvoice = "I";
/** Lifo = L */
public static String COSTINGMETHOD_Lifo = "L";
/** Standard Costing = S */
public static String COSTINGMETHOD_StandardCosting = "S";
/** User Defined = U */
public static String COSTINGMETHOD_UserDefined = "U";
/** Last Invoice = i */
public static String COSTINGMETHOD_LastInvoice = "i";
/** Last PO Price = p */
public static String COSTINGMETHOD_LastPOPrice = "p";
/** _ = x */
public static String COSTINGMETHOD__ = "x";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCostingMethodValid (String test)
{
return test == null || test.Equals("A") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x");
}
/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
public void SetCostingMethod (String CostingMethod)
{
if (!IsCostingMethodValid(CostingMethod))
throw new ArgumentException ("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
throw new ArgumentException ("CostingMethod Is virtual column");
}
/** Get Costing Method.
@return Indicates how Costs will be calculated */
public String GetCostingMethod() 
{
return (String)Get_Value("CostingMethod");
}
/** Set Accumulated Amt.
@param CumulatedAmt Total Amount */
public void SetCumulatedAmt (Decimal? CumulatedAmt)
{
Set_ValueNoCheck ("CumulatedAmt", (Decimal?)CumulatedAmt);
}
/** Get Accumulated Amt.
@return Total Amount */
public Decimal GetCumulatedAmt() 
{
Object bd =Get_Value("CumulatedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accumulated Qty.
@param CumulatedQty Total Quantity */
public void SetCumulatedQty (Decimal? CumulatedQty)
{
Set_ValueNoCheck ("CumulatedQty", (Decimal?)CumulatedQty);
}
/** Get Accumulated Qty.
@return Total Quantity */
public Decimal GetCumulatedQty() 
{
Object bd =Get_Value("CumulatedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Current Cost.
@param CurrentCostPrice The currently used cost price */
public void SetCurrentCostPrice (Decimal? CurrentCostPrice)
{
if (CurrentCostPrice == null) throw new ArgumentException ("CurrentCostPrice is mandatory.");
Set_Value ("CurrentCostPrice", (Decimal?)CurrentCostPrice);
}
/** Get Current Cost.
@return The currently used cost price */
public Decimal GetCurrentCostPrice() 
{
Object bd =Get_Value("CurrentCostPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Current Quantity.
@param CurrentQty Current Quantity */
public void SetCurrentQty (Decimal? CurrentQty)
{
if (CurrentQty == null) throw new ArgumentException ("CurrentQty is mandatory.");
Set_Value ("CurrentQty", (Decimal?)CurrentQty);
}
/** Get Current Quantity.
@return Current Quantity */
public Decimal GetCurrentQty() 
{
Object bd =Get_Value("CurrentQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_ValueNoCheck ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Future Cost.
@param FutureCostPrice Future Cost */
public void SetFutureCostPrice (Decimal? FutureCostPrice)
{
Set_Value ("FutureCostPrice", (Decimal?)FutureCostPrice);
}
/** Get Future Cost.
@return Future Cost */
public Decimal GetFutureCostPrice() 
{
Object bd =Get_Value("FutureCostPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Asset Cost.
@param IsAssetCost Asset Cost */
public void SetIsAssetCost (Boolean IsAssetCost)
{
Set_Value ("IsAssetCost", IsAssetCost);
}
/** Get Asset Cost.
@return Asset Cost */
public Boolean IsAssetCost() 
{
Object oo = Get_Value("IsAssetCost");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** IsThisLevel VAF_Control_Ref_ID=319 */
public static int ISTHISLEVEL_VAF_Control_Ref_ID=319;
/** No = N */
public static String ISTHISLEVEL_No = "N";
/** Yes = Y */
public static String ISTHISLEVEL_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsThisLevelValid (String test)
{
return test.Equals("N") || test.Equals("Y");
}
/** Set IsThisLevel.
@param IsThisLevel IsThisLevel */
public void SetIsThisLevel (String IsThisLevel)
{
if (IsThisLevel == null) throw new ArgumentException ("IsThisLevel is mandatory");
if (!IsIsThisLevelValid(IsThisLevel))
throw new ArgumentException ("IsThisLevel Invalid value - " + IsThisLevel + " - Reference_ID=319 - N - Y");
if (IsThisLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsThisLevel = IsThisLevel.Substring(0,1);
}
Set_Value ("IsThisLevel", IsThisLevel);
}
/** Get IsThisLevel.
@return IsThisLevel */
public String GetIsThisLevel() 
{
return (String)Get_Value("IsThisLevel");
}

/** IsUserDefined VAF_Control_Ref_ID=319 */
public static int ISUSERDEFINED_VAF_Control_Ref_ID=319;
/** No = N */
public static String ISUSERDEFINED_No = "N";
/** Yes = Y */
public static String ISUSERDEFINED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsUserDefinedValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set IsUserDefined.
@param IsUserDefined IsUserDefined */
public void SetIsUserDefined (String IsUserDefined)
{
if (!IsIsUserDefinedValid(IsUserDefined))
throw new ArgumentException ("IsUserDefined Invalid value - " + IsUserDefined + " - Reference_ID=319 - N - Y");
if (IsUserDefined != null && IsUserDefined.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsUserDefined = IsUserDefined.Substring(0,1);
}
Set_Value ("IsUserDefined", IsUserDefined);
}
/** Get IsUserDefined.
@return IsUserDefined */
public String GetIsUserDefined() 
{
return (String)Get_Value("IsUserDefined");
}
/** Set LastCostPrice.
@param LastCostPrice LastCostPrice */
public void SetLastCostPrice (Decimal? LastCostPrice)
{
if (LastCostPrice == null) throw new ArgumentException ("LastCostPrice is mandatory.");
Set_Value ("LastCostPrice", (Decimal?)LastCostPrice);
}
/** Get LastCostPrice.
@return LastCostPrice */
public Decimal GetLastCostPrice() 
{
Object bd =Get_Value("LastCostPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Attribute Set Instance.
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
{
if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException ("VAM_PFeature_SetInstance_ID is mandatory.");
Set_ValueNoCheck ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetVAM_PFeature_SetInstance_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Element.
@param VAM_ProductCostElement_ID Product Cost Element */
public void SetVAM_ProductCostElement_ID (int VAM_ProductCostElement_ID)
{
if (VAM_ProductCostElement_ID < 1) throw new ArgumentException ("VAM_ProductCostElement_ID is mandatory.");
Set_ValueNoCheck ("VAM_ProductCostElement_ID", VAM_ProductCostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetVAM_ProductCostElement_ID() 
{
Object ii = Get_Value("VAM_ProductCostElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Type.
@param VAM_ProductCostType_ID Type of Cost (e.g. Current, Plan, Future) */
public void SetVAM_ProductCostType_ID (int VAM_ProductCostType_ID)
{
if (VAM_ProductCostType_ID < 1) throw new ArgumentException ("VAM_ProductCostType_ID is mandatory.");
Set_ValueNoCheck ("VAM_ProductCostType_ID", VAM_ProductCostType_ID);
}
/** Get Cost Type.
@return Type of Cost (e.g. Current, Plan, Future) */
public int GetVAM_ProductCostType_ID() 
{
Object ii = Get_Value("VAM_ProductCostType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Percent.
@param PercentCost Percent Cost */
public void SetPercentCost (Decimal? PercentCost)
{
Set_Value ("PercentCost", (Decimal?)PercentCost);
}
/** Get Percent.
@return Percent Cost */
public Decimal GetPercentCost() 
{
Object bd =Get_Value("PercentCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
throw new ArgumentException ("Processed Is virtual column");
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


/** Set Warehouse.
@param VAM_Warehouse_ID Storage Warehouse and Service Point */
public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID) { if (VAM_Warehouse_ID < 0) throw new ArgumentException("VAM_Warehouse_ID is mandatory."); Set_ValueNoCheck("VAM_Warehouse_ID", VAM_Warehouse_ID); }/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetVAM_Warehouse_ID() { Object ii = Get_Value("VAM_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
}

}
