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
/** Generated Model for C_ProjectLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ProjectLine : PO
{
public X_C_ProjectLine (Context ctx, int C_ProjectLine_ID, Trx trxName) : base (ctx, C_ProjectLine_ID, trxName)
{
/** if (C_ProjectLine_ID == 0)
{
SetC_ProjectLine_ID (0);
SetC_Project_ID (0);
SetInvoicedAmt (0.0);
SetInvoicedQty (0.0);	// 0
SetIsPrinted (true);	// Y
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_ProjectLine WHERE C_Project_ID=@C_Project_ID@
SetPlannedAmt (0.0);
SetPlannedPrice (0.0);
SetPlannedQty (0.0);	// 1
SetProcessed (false);	// N
}
 */
}
public X_C_ProjectLine (Ctx ctx, int C_ProjectLine_ID, Trx trxName) : base (ctx, C_ProjectLine_ID, trxName)
{
/** if (C_ProjectLine_ID == 0)
{
SetC_ProjectLine_ID (0);
SetC_Project_ID (0);
SetInvoicedAmt (0.0);
SetInvoicedQty (0.0);	// 0
SetIsPrinted (true);	// Y
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_ProjectLine WHERE C_Project_ID=@C_Project_ID@
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
public X_C_ProjectLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ProjectLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374287L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057498L;
/** AD_Table_ID=434 */
public static int Table_ID;
 // =434;

/** TableName=C_ProjectLine */
public static String Table_Name="C_ProjectLine";

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
StringBuilder sb = new StringBuilder ("X_C_ProjectLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** C_OrderPO_ID AD_Reference_ID=290 */
public static int C_ORDERPO_ID_AD_Reference_ID=290;
/** Set Purchase Order.
@param C_OrderPO_ID Purchase Order */
public void SetC_OrderPO_ID (int C_OrderPO_ID)
{
if (C_OrderPO_ID <= 0) Set_ValueNoCheck ("C_OrderPO_ID", null);
else
Set_ValueNoCheck ("C_OrderPO_ID", C_OrderPO_ID);
}
/** Get Purchase Order.
@return Purchase Order */
public int GetC_OrderPO_ID() 
{
Object ii = Get_Value("C_OrderPO_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_ValueNoCheck ("C_Order_ID", null);
else
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
/** Set Project Issue.
@param C_ProjectIssue_ID Project Issues (Material, Labor) */
public void SetC_ProjectIssue_ID (int C_ProjectIssue_ID)
{
if (C_ProjectIssue_ID <= 0) Set_ValueNoCheck ("C_ProjectIssue_ID", null);
else
Set_ValueNoCheck ("C_ProjectIssue_ID", C_ProjectIssue_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetC_ProjectIssue_ID() 
{
Object ii = Get_Value("C_ProjectIssue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Line.
@param C_ProjectLine_ID Task or step in a project */
public void SetC_ProjectLine_ID (int C_ProjectLine_ID)
{
if (C_ProjectLine_ID < 1) throw new ArgumentException ("C_ProjectLine_ID is mandatory.");
Set_ValueNoCheck ("C_ProjectLine_ID", C_ProjectLine_ID);
}
/** Get Project Line.
@return Task or step in a project */
public int GetC_ProjectLine_ID() 
{
Object ii = Get_Value("C_ProjectLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Phase.
@param C_ProjectPhase_ID Phase of a Project */
public void SetC_ProjectPhase_ID (int C_ProjectPhase_ID)
{
if (C_ProjectPhase_ID <= 0) Set_Value ("C_ProjectPhase_ID", null);
else
Set_Value ("C_ProjectPhase_ID", C_ProjectPhase_ID);
}
/** Get Project Phase.
@return Phase of a Project */
public int GetC_ProjectPhase_ID() 
{
Object ii = Get_Value("C_ProjectPhase_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Task.
@param C_ProjectTask_ID Actual Project Task in a Phase */
public void SetC_ProjectTask_ID (int C_ProjectTask_ID)
{
if (C_ProjectTask_ID <= 0) Set_Value ("C_ProjectTask_ID", null);
else
Set_Value ("C_ProjectTask_ID", C_ProjectTask_ID);
}
/** Get Project Task.
@return Actual Project Task in a Phase */
public int GetC_ProjectTask_ID() 
{
Object ii = Get_Value("C_ProjectTask_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID < 1) throw new ArgumentException ("C_Project_ID is mandatory.");
Set_ValueNoCheck ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
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
