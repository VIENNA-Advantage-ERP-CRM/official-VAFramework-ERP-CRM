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
/** Generated Model for VAB_ProjectLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProjectLine : PO
{
public X_VAB_ProjectLine (Context ctx, int VAB_ProjectLine_ID, Trx trxName) : base (ctx, VAB_ProjectLine_ID, trxName)
{
/** if (VAB_ProjectLine_ID == 0)
{
SetVAB_ProjectLine_ID (0);
SetVAB_Project_ID (0);
SetInvoicedAmt (0.0);
SetInvoicedQty (0.0);	// 0
SetIsPrinted (true);	// Y
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_ProjectLine WHERE VAB_Project_ID=@VAB_Project_ID@
SetPlannedAmt (0.0);
SetPlannedPrice (0.0);
SetPlannedQty (0.0);	// 1
SetProcessed (false);	// N
}
 */
}
public X_VAB_ProjectLine (Ctx ctx, int VAB_ProjectLine_ID, Trx trxName) : base (ctx, VAB_ProjectLine_ID, trxName)
{
/** if (VAB_ProjectLine_ID == 0)
{
SetVAB_ProjectLine_ID (0);
SetVAB_Project_ID (0);
SetInvoicedAmt (0.0);
SetInvoicedQty (0.0);	// 0
SetIsPrinted (true);	// Y
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_ProjectLine WHERE VAB_Project_ID=@VAB_Project_ID@
SetPlannedAmt (0.0);
SetPlannedPrice (0.0);
SetPlannedQty (0.0);	// 1
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374287L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057498L;
/** VAF_TableView_ID=434 */
public static int Table_ID;
 // =434;

/** TableName=VAB_ProjectLine */
public static String Table_Name="VAB_ProjectLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAB_OrderPO_ID VAF_Control_Ref_ID=290 */
public static int VAB_ORDERPO_ID_VAF_Control_Ref_ID=290;
/** Set Purchase Order.
@param VAB_OrderPO_ID Purchase Order */
public void SetVAB_OrderPO_ID (int VAB_OrderPO_ID)
{
if (VAB_OrderPO_ID <= 0) Set_ValueNoCheck ("VAB_OrderPO_ID", null);
else
Set_ValueNoCheck ("VAB_OrderPO_ID", VAB_OrderPO_ID);
}
/** Get Purchase Order.
@return Purchase Order */
public int GetVAB_OrderPO_ID() 
{
Object ii = Get_Value("VAB_OrderPO_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param VAB_Order_ID Order */
public void SetVAB_Order_ID (int VAB_Order_ID)
{
if (VAB_Order_ID <= 0) Set_ValueNoCheck ("VAB_Order_ID", null);
else
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
/** Set Project Issue.
@param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
public void SetVAB_ProjectSupply_ID (int VAB_ProjectSupply_ID)
{
if (VAB_ProjectSupply_ID <= 0) Set_ValueNoCheck ("VAB_ProjectSupply_ID", null);
else
Set_ValueNoCheck ("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetVAB_ProjectSupply_ID() 
{
Object ii = Get_Value("VAB_ProjectSupply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Line.
@param VAB_ProjectLine_ID Task or step in a project */
public void SetVAB_ProjectLine_ID (int VAB_ProjectLine_ID)
{
if (VAB_ProjectLine_ID < 1) throw new ArgumentException ("VAB_ProjectLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectLine_ID", VAB_ProjectLine_ID);
}
/** Get Project Line.
@return Task or step in a project */
public int GetVAB_ProjectLine_ID() 
{
Object ii = Get_Value("VAB_ProjectLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Phase.
@param VAB_ProjectStage_ID Phase of a Project */
public void SetVAB_ProjectStage_ID (int VAB_ProjectStage_ID)
{
if (VAB_ProjectStage_ID <= 0) Set_Value ("VAB_ProjectStage_ID", null);
else
Set_Value ("VAB_ProjectStage_ID", VAB_ProjectStage_ID);
}
/** Get Project Phase.
@return Phase of a Project */
public int GetVAB_ProjectStage_ID() 
{
Object ii = Get_Value("VAB_ProjectStage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Task.
@param VAB_ProjectJob_ID Actual Project Task in a Phase */
public void SetVAB_ProjectJob_ID (int VAB_ProjectJob_ID)
{
if (VAB_ProjectJob_ID <= 0) Set_Value ("VAB_ProjectJob_ID", null);
else
Set_Value ("VAB_ProjectJob_ID", VAB_ProjectJob_ID);
}
/** Get Project Task.
@return Actual Project Task in a Phase */
public int GetVAB_ProjectJob_ID() 
{
Object ii = Get_Value("VAB_ProjectJob_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID < 1) throw new ArgumentException ("VAB_Project_ID is mandatory.");
Set_ValueNoCheck ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Committed Amount.
@param CommittedAmt The (legal) commitment amount */
public void SetCommittedAmt (Decimal? CommittedAmt)
{
Set_Value ("CommittedAmt", (Decimal?)CommittedAmt);
}
/** Get Committed Amount.
@return The (legal) commitment amount */
public Decimal GetCommittedAmt() 
{
Object bd =Get_Value("CommittedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Committed Quantity.
@param CommittedQty The (legal) commitment Quantity */
public void SetCommittedQty (Decimal? CommittedQty)
{
Set_Value ("CommittedQty", (Decimal?)CommittedQty);
}
/** Get Committed Quantity.
@return The (legal) commitment Quantity */
public Decimal GetCommittedQty() 
{
Object bd =Get_Value("CommittedQty");
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
/** Set Discount %.
@param Discount Discount in percent */
public void SetDiscount (Decimal? Discount)
{
Set_Value ("Discount", (Decimal?)Discount);
}
/** Get Discount %.
@return Discount in percent */
public Decimal GetDiscount() 
{
Object bd =Get_Value("Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Pricing.
@param DoPricing Pricing */
public void SetDoPricing (String DoPricing)
{
if (DoPricing != null && DoPricing.Length > 1)
{
log.Warning("Length > 1 - truncated");
DoPricing = DoPricing.Substring(0,1);
}
Set_Value ("DoPricing", DoPricing);
}
/** Get Pricing.
@return Pricing */
public String GetDoPricing() 
{
return (String)Get_Value("DoPricing");
}
/** Set Invoiced Amount.
@param InvoicedAmt The amount invoiced */
public void SetInvoicedAmt (Decimal? InvoicedAmt)
{
if (InvoicedAmt == null) throw new ArgumentException ("InvoicedAmt is mandatory.");
Set_Value ("InvoicedAmt", (Decimal?)InvoicedAmt);
}
/** Get Invoiced Amount.
@return The amount invoiced */
public Decimal GetInvoicedAmt() 
{
Object bd =Get_Value("InvoicedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity Invoiced.
@param InvoicedQty The quantity invoiced */
public void SetInvoicedQty (Decimal? InvoicedQty)
{
if (InvoicedQty == null) throw new ArgumentException ("InvoicedQty is mandatory.");
Set_Value ("InvoicedQty", (Decimal?)InvoicedQty);
}
/** Get Quantity Invoiced.
@return The quantity invoiced */
public Decimal GetInvoicedQty() 
{
Object bd =Get_Value("InvoicedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Printed.
@param IsPrinted Indicates if this document / line is printed */
public void SetIsPrinted (Boolean IsPrinted)
{
Set_Value ("IsPrinted", IsPrinted);
}
/** Get Printed.
@return Indicates if this document / line is printed */
public Boolean IsPrinted() 
{
Object oo = Get_Value("IsPrinted");
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
/** Set Product Category.
@param VAM_ProductCategory_ID Category of a Product */
public void SetVAM_ProductCategory_ID (int VAM_ProductCategory_ID)
{
if (VAM_ProductCategory_ID <= 0) Set_Value ("VAM_ProductCategory_ID", null);
else
Set_Value ("VAM_ProductCategory_ID", VAM_ProductCategory_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetVAM_ProductCategory_ID() 
{
Object ii = Get_Value("VAM_ProductCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);
else
Set_Value ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Planned Amount.
@param PlannedAmt Planned amount for this project */
public void SetPlannedAmt (Decimal? PlannedAmt)
{
if (PlannedAmt == null) throw new ArgumentException ("PlannedAmt is mandatory.");
Set_Value ("PlannedAmt", (Decimal?)PlannedAmt);
}
/** Get Planned Amount.
@return Planned amount for this project */
public Decimal GetPlannedAmt() 
{
Object bd =Get_Value("PlannedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Planned Date.
@param PlannedDate Date projected */
public void SetPlannedDate (DateTime? PlannedDate)
{
Set_Value ("PlannedDate", (DateTime?)PlannedDate);
}
/** Get Planned Date.
@return Date projected */
public DateTime? GetPlannedDate() 
{
return (DateTime?)Get_Value("PlannedDate");
}
/** Set Planned Margin.
@param PlannedMarginAmt Project's planned margin amount */
public void SetPlannedMarginAmt (Decimal? PlannedMarginAmt)
{
Set_Value ("PlannedMarginAmt", (Decimal?)PlannedMarginAmt);
}
/** Get Planned Margin.
@return Project's planned margin amount */
public Decimal GetPlannedMarginAmt() 
{
Object bd =Get_Value("PlannedMarginAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Planned Price.
@param PlannedPrice Planned price for this project line */
public void SetPlannedPrice (Decimal? PlannedPrice)
{
if (PlannedPrice == null) throw new ArgumentException ("PlannedPrice is mandatory.");
Set_Value ("PlannedPrice", (Decimal?)PlannedPrice);
}
/** Get Planned Price.
@return Planned price for this project line */
public Decimal GetPlannedPrice() 
{
Object bd =Get_Value("PlannedPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Planned Quantity.
@param PlannedQty Planned quantity for this project */
public void SetPlannedQty (Decimal? PlannedQty)
{
if (PlannedQty == null) throw new ArgumentException ("PlannedQty is mandatory.");
Set_Value ("PlannedQty", (Decimal?)PlannedQty);
}
/** Get Planned Quantity.
@return Planned quantity for this project */
public Decimal GetPlannedQty() 
{
Object bd =Get_Value("PlannedQty");
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
/** Set Probability.
@param Probability Probability in Percent */
public void SetProbability (int Probability)
{
Set_Value ("Probability", Probability);
}
/** Get Probability.
@return Probability in Percent */
public int GetProbability() 
{
Object ii = Get_Value("Probability");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
