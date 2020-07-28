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
/** Generated Model for C_ProjectPhase
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ProjectPhase : PO
{
public X_C_ProjectPhase (Context ctx, int C_ProjectPhase_ID, Trx trxName) : base (ctx, C_ProjectPhase_ID, trxName)
{
/** if (C_ProjectPhase_ID == 0)
{
SetC_ProjectPhase_ID (0);
SetC_Project_ID (0);
SetCommittedAmt (0.0);
SetIsCommitCeiling (false);
SetIsComplete (false);
SetName (null);
SetPlannedAmt (0.0);
SetPlannedQty (0.0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_ProjectPhase WHERE C_Project_ID=@C_Project_ID@
}
 */
}
public X_C_ProjectPhase (Ctx ctx, int C_ProjectPhase_ID, Trx trxName) : base (ctx, C_ProjectPhase_ID, trxName)
{
/** if (C_ProjectPhase_ID == 0)
{
SetC_ProjectPhase_ID (0);
SetC_Project_ID (0);
SetCommittedAmt (0.0);
SetIsCommitCeiling (false);
SetIsComplete (false);
SetName (null);
SetPlannedAmt (0.0);
SetPlannedQty (0.0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM C_ProjectPhase WHERE C_Project_ID=@C_Project_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectPhase (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectPhase (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectPhase (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ProjectPhase()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374334L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057545L;
/** AD_Table_ID=576 */
public static int Table_ID;
 // =576;

/** TableName=C_ProjectPhase */
public static String Table_Name="C_ProjectPhase";

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
StringBuilder sb = new StringBuilder ("X_C_ProjectPhase[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Standard Phase.
@param C_Phase_ID Standard Phase of the Project Type */
public void SetC_Phase_ID (int C_Phase_ID)
{
if (C_Phase_ID <= 0) Set_ValueNoCheck ("C_Phase_ID", null);
else
Set_ValueNoCheck ("C_Phase_ID", C_Phase_ID);
}
/** Get Standard Phase.
@return Standard Phase of the Project Type */
public int GetC_Phase_ID() 
{
Object ii = Get_Value("C_Phase_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Phase.
@param C_ProjectPhase_ID Phase of a Project */
public void SetC_ProjectPhase_ID (int C_ProjectPhase_ID)
{
if (C_ProjectPhase_ID < 1) throw new ArgumentException ("C_ProjectPhase_ID is mandatory.");
Set_ValueNoCheck ("C_ProjectPhase_ID", C_ProjectPhase_ID);
}
/** Get Project Phase.
@return Phase of a Project */
public int GetC_ProjectPhase_ID() 
{
Object ii = Get_Value("C_ProjectPhase_ID");
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
if (CommittedAmt == null) throw new ArgumentException ("CommittedAmt is mandatory.");
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
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
}
/** Set Generate Order.
@param GenerateOrder Generate Order */
public void SetGenerateOrder (String GenerateOrder)
{
if (GenerateOrder != null && GenerateOrder.Length > 1)
{
log.Warning("Length > 1 - truncated");
GenerateOrder = GenerateOrder.Substring(0,1);
}
Set_Value ("GenerateOrder", GenerateOrder);
}
/** Get Generate Order.
@return Generate Order */
public String GetGenerateOrder() 
{
return (String)Get_Value("GenerateOrder");
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
/** Set Commitment is Ceiling.
@param IsCommitCeiling The commitment amount/quantity is the chargeable ceiling */
public void SetIsCommitCeiling (Boolean IsCommitCeiling)
{
Set_Value ("IsCommitCeiling", IsCommitCeiling);
}
/** Get Commitment is Ceiling.
@return The commitment amount/quantity is the chargeable ceiling */
public Boolean IsCommitCeiling() 
{
Object oo = Get_Value("IsCommitCeiling");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Complete.
@param IsComplete It is complete */
public void SetIsComplete (Boolean IsComplete)
{
Set_Value ("IsComplete", IsComplete);
}
/** Get Complete.
@return It is complete */
public Boolean IsComplete() 
{
Object oo = Get_Value("IsComplete");
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
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
/** Set Unit Price.
@param PriceActual Actual Price */
public void SetPriceActual (Decimal? PriceActual)
{
Set_Value ("PriceActual", (Decimal?)PriceActual);
}
/** Get Unit Price.
@return Actual Price */
public Decimal GetPriceActual() 
{
Object bd =Get_Value("PriceActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** ProjInvoiceRule AD_Reference_ID=383 */
public static int PROJINVOICERULE_AD_Reference_ID=383;
/** None = - */
public static String PROJINVOICERULE_None = "-";
/** Committed Amount = C */
public static String PROJINVOICERULE_CommittedAmount = "C";
/** Product  Quantity = P */
public static String PROJINVOICERULE_ProductQuantity = "P";
/** Time&Material = T */
public static String PROJINVOICERULE_TimeMaterial = "T";
/** Time&Material max Comitted = c */
public static String PROJINVOICERULE_TimeMaterialMaxComitted = "c";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsProjInvoiceRuleValid (String test)
{
return test == null || test.Equals("-") || test.Equals("C") || test.Equals("P") || test.Equals("T") || test.Equals("c");
}
/** Set Invoice Rule.
@param ProjInvoiceRule Invoice Rule for the project */
public void SetProjInvoiceRule (String ProjInvoiceRule)
{
if (!IsProjInvoiceRuleValid(ProjInvoiceRule))
throw new ArgumentException ("ProjInvoiceRule Invalid value - " + ProjInvoiceRule + " - Reference_ID=383 - - - C - P - T - c");
if (ProjInvoiceRule != null && ProjInvoiceRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
ProjInvoiceRule = ProjInvoiceRule.Substring(0,1);
}
Set_Value ("ProjInvoiceRule", ProjInvoiceRule);
}
/** Get Invoice Rule.
@return Invoice Rule for the project */
public String GetProjInvoiceRule() 
{
return (String)Get_Value("ProjInvoiceRule");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetSeqNo().ToString());
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
}

}
