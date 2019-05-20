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
/** Generated Model for M_Cost
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Cost : PO
{
public X_M_Cost (Context ctx, int M_Cost_ID, Trx trxName) : base (ctx, M_Cost_ID, trxName)
{
/** if (M_Cost_ID == 0)
{
SetA_Asset_ID (0);	// 0
SetBasisType (null);	// I
SetC_AcctSchema_ID (0);
SetCurrentCostPrice (0.0);
SetCurrentQty (0.0);
SetIsThisLevel (null);	// Y
SetLastCostPrice (0.0);	// 0
SetM_AttributeSetInstance_ID (0);
SetM_CostElement_ID (0);
SetM_CostType_ID (0);
SetM_Product_ID (0);
}
 */
}
public X_M_Cost (Ctx ctx, int M_Cost_ID, Trx trxName) : base (ctx, M_Cost_ID, trxName)
{
/** if (M_Cost_ID == 0)
{
SetA_Asset_ID (0);	// 0
SetBasisType (null);	// I
SetC_AcctSchema_ID (0);
SetCurrentCostPrice (0.0);
SetCurrentQty (0.0);
SetIsThisLevel (null);	// Y
SetLastCostPrice (0.0);	// 0
SetM_AttributeSetInstance_ID (0);
SetM_CostElement_ID (0);
SetM_CostType_ID (0);
SetM_Product_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Cost (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Cost (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Cost (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Cost()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27694963715596L;
/** Last Updated Timestamp 10/9/2014 12:36:39 PM */
public static long updatedMS = 1412838398807L;
/** AD_Table_ID=771 */
public static int Table_ID;
 // =771;

/** TableName=M_Cost */
public static String Table_Name="M_Cost";

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
StringBuilder sb = new StringBuilder ("X_M_Cost[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID < 1) throw new ArgumentException ("A_Asset_ID is mandatory.");
Set_ValueNoCheck ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** BasisType AD_Reference_ID=1000032 */
public static int BASISTYPE_AD_Reference_ID=1000032;
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
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
throw new ArgumentException ("C_UOM_ID Is virtual column");
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CostingMethod AD_Reference_ID=122 */
public static int COSTINGMETHOD_AD_Reference_ID=122;
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

/** IsThisLevel AD_Reference_ID=319 */
public static int ISTHISLEVEL_AD_Reference_ID=319;
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

/** IsUserDefined AD_Reference_ID=319 */
public static int ISUSERDEFINED_AD_Reference_ID=319;
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
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID (int M_CostElement_ID)
{
if (M_CostElement_ID < 1) throw new ArgumentException ("M_CostElement_ID is mandatory.");
Set_ValueNoCheck ("M_CostElement_ID", M_CostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() 
{
Object ii = Get_Value("M_CostElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Type.
@param M_CostType_ID Type of Cost (e.g. Current, Plan, Future) */
public void SetM_CostType_ID (int M_CostType_ID)
{
if (M_CostType_ID < 1) throw new ArgumentException ("M_CostType_ID is mandatory.");
Set_ValueNoCheck ("M_CostType_ID", M_CostType_ID);
}
/** Get Cost Type.
@return Type of Cost (e.g. Current, Plan, Future) */
public int GetM_CostType_ID() 
{
Object ii = Get_Value("M_CostType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
