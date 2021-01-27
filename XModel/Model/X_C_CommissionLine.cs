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
/** Generated Model for VAB_WorkCommissionLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_WorkCommissionLine : PO
{
public X_VAB_WorkCommissionLine (Context ctx, int VAB_WorkCommissionLine_ID, Trx trxName) : base (ctx, VAB_WorkCommissionLine_ID, trxName)
{
/** if (VAB_WorkCommissionLine_ID == 0)
{
SetAmtMultiplier (0.0);
SetAmtSubtract (0.0);
SetVAB_WorkCommissionLine_ID (0);
SetVAB_WorkCommission_ID (0);
SetCommissionOrders (false);
SetIsPositiveOnly (false);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_WorkCommissionLine WHERE VAB_WorkCommission_ID=@VAB_WorkCommission_ID@
SetQtyMultiplier (0.0);
SetQtySubtract (0.0);
}
 */
}
public X_VAB_WorkCommissionLine (Ctx ctx, int VAB_WorkCommissionLine_ID, Trx trxName) : base (ctx, VAB_WorkCommissionLine_ID, trxName)
{
/** if (VAB_WorkCommissionLine_ID == 0)
{
SetAmtMultiplier (0.0);
SetAmtSubtract (0.0);
SetVAB_WorkCommissionLine_ID (0);
SetVAB_WorkCommission_ID (0);
SetCommissionOrders (false);
SetIsPositiveOnly (false);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_WorkCommissionLine WHERE VAB_WorkCommission_ID=@VAB_WorkCommission_ID@
SetQtyMultiplier (0.0);
SetQtySubtract (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_WorkCommissionLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371388L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054599L;
/** VAF_TableView_ID=431 */
public static int Table_ID;
 // =431;

/** TableName=VAB_WorkCommissionLine */
public static String Table_Name="VAB_WorkCommissionLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_WorkCommissionLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Multiplier Amount.
@param AmtMultiplier Multiplier Amount for generating commissions */
public void SetAmtMultiplier (Decimal? AmtMultiplier)
{
if (AmtMultiplier == null) throw new ArgumentException ("AmtMultiplier is mandatory.");
Set_Value ("AmtMultiplier", (Decimal?)AmtMultiplier);
}
/** Get Multiplier Amount.
@return Multiplier Amount for generating commissions */
public Decimal GetAmtMultiplier() 
{
Object bd =Get_Value("AmtMultiplier");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Subtract Amount.
@param AmtSubtract Subtract Amount for generating commissions */
public void SetAmtSubtract (Decimal? AmtSubtract)
{
if (AmtSubtract == null) throw new ArgumentException ("AmtSubtract is mandatory.");
Set_Value ("AmtSubtract", (Decimal?)AmtSubtract);
}
/** Get Subtract Amount.
@return Subtract Amount for generating commissions */
public Decimal GetAmtSubtract() 
{
Object bd =Get_Value("AmtSubtract");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Business Partner Group.
@param VAB_BPart_Category_ID Business Partner Group */
public void SetVAB_BPart_Category_ID (int VAB_BPart_Category_ID)
{
if (VAB_BPart_Category_ID <= 0) Set_Value ("VAB_BPart_Category_ID", null);
else
Set_Value ("VAB_BPart_Category_ID", VAB_BPart_Category_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetVAB_BPart_Category_ID() 
{
Object ii = Get_Value("VAB_BPart_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_Value ("VAB_BusinessPartner_ID", null);
else
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission Line.
@param VAB_WorkCommissionLine_ID Commission Line */
public void SetVAB_WorkCommissionLine_ID (int VAB_WorkCommissionLine_ID)
{
if (VAB_WorkCommissionLine_ID < 1) throw new ArgumentException ("VAB_WorkCommissionLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommissionLine_ID", VAB_WorkCommissionLine_ID);
}
/** Get Commission Line.
@return Commission Line */
public int GetVAB_WorkCommissionLine_ID() 
{
Object ii = Get_Value("VAB_WorkCommissionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission.
@param VAB_WorkCommission_ID Commission */
public void SetVAB_WorkCommission_ID (int VAB_WorkCommission_ID)
{
if (VAB_WorkCommission_ID < 1) throw new ArgumentException ("VAB_WorkCommission_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommission_ID", VAB_WorkCommission_ID);
}
/** Get Commission.
@return Commission */
public int GetVAB_WorkCommission_ID() 
{
Object ii = Get_Value("VAB_WorkCommission_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_WorkCommission_ID().ToString());
}
/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID)
{
if (VAB_SalesRegionState_ID <= 0) Set_Value ("VAB_SalesRegionState_ID", null);
else
Set_Value ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() 
{
Object ii = Get_Value("VAB_SalesRegionState_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission only specified Orders.
@param CommissionOrders Commission only Orders or Invoices, where this Sales Rep is entered */
public void SetCommissionOrders (Boolean CommissionOrders)
{
Set_Value ("CommissionOrders", CommissionOrders);
}
/** Get Commission only specified Orders.
@return Commission only Orders or Invoices, where this Sales Rep is entered */
public Boolean IsCommissionOrders() 
{
Object oo = Get_Value("CommissionOrders");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Positive only.
@param IsPositiveOnly Do not generate negative commissions */
public void SetIsPositiveOnly (Boolean IsPositiveOnly)
{
Set_Value ("IsPositiveOnly", IsPositiveOnly);
}
/** Get Positive only.
@return Do not generate negative commissions */
public Boolean IsPositiveOnly() 
{
Object oo = Get_Value("IsPositiveOnly");
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

/** Org_ID VAF_Control_Ref_ID=130 */
public static int ORG_ID_VAF_Control_Ref_ID=130;
/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID)
{
if (Org_ID <= 0) Set_Value ("Org_ID", null);
else
Set_Value ("Org_ID", Org_ID);
}
/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() 
{
Object ii = Get_Value("Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Multiplier Quantity.
@param QtyMultiplier Value to multiply quantities by for generating commissions. */
public void SetQtyMultiplier (Decimal? QtyMultiplier)
{
if (QtyMultiplier == null) throw new ArgumentException ("QtyMultiplier is mandatory.");
Set_Value ("QtyMultiplier", (Decimal?)QtyMultiplier);
}
/** Get Multiplier Quantity.
@return Value to multiply quantities by for generating commissions. */
public Decimal GetQtyMultiplier() 
{
Object bd =Get_Value("QtyMultiplier");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Subtract Quantity.
@param QtySubtract Quantity to subtract when generating commissions */
public void SetQtySubtract (Decimal? QtySubtract)
{
if (QtySubtract == null) throw new ArgumentException ("QtySubtract is mandatory.");
Set_Value ("QtySubtract", (Decimal?)QtySubtract);
}
/** Get Subtract Quantity.
@return Quantity to subtract when generating commissions */
public Decimal GetQtySubtract() 
{
Object bd =Get_Value("QtySubtract");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** VAB_WorkCommissionType VAF_Control_Ref_ID=1000180 */
public static int VAB_WORKCOMMISSIONTYPE_VAF_Control_Ref_ID = 1000180;
/** Amount = A */
public static String VAB_WORKCOMMISSIONTYPE_Amount = "A";
/** Quantity = Q */
public static String VAB_WORKCOMMISSIONTYPE_Quantity = "Q";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAB_WorkCommissionTypeValid(String test)
{
    return test == null || test.Equals("A") || test.Equals("Q");
}
/** Set VAB_WorkCommissionType.
@param VAB_WorkCommissionType VAB_WorkCommissionType */
public void SetVAB_WorkCommissionType(String VAB_WorkCommissionType)
{
    if (!IsVAB_WorkCommissionTypeValid(VAB_WorkCommissionType))
        throw new ArgumentException("VAB_WorkCommissionType Invalid value - " + VAB_WorkCommissionType + " - Reference_ID=1000180 - A - Q");
    if (VAB_WorkCommissionType != null && VAB_WorkCommissionType.Length > 1)
    {
        log.Warning("Length > 1 - truncated");
        VAB_WorkCommissionType = VAB_WorkCommissionType.Substring(0, 1);
    }
    Set_Value("VAB_WorkCommissionType", VAB_WorkCommissionType);
}
/** Get VAB_WorkCommissionType.
@return VAB_WorkCommissionType */
public String GetVAB_WorkCommissionType()
{
    return (String)Get_Value("VAB_WorkCommissionType");
}

/** Set C_TargetAmount.
@param C_TargetAmount C_TargetAmount */
public void SetC_TargetAmount(Decimal? C_TargetAmount)
{
    Set_Value("C_TargetAmount", (Decimal?)C_TargetAmount);
}
/** Get C_TargetAmount.
@return C_TargetAmount */
public Decimal GetC_TargetAmount()
{
    Object bd = Get_Value("C_TargetAmount");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_TargetPercentage.
@param C_TargetPercentage C_TargetPercentage */
public void SetC_TargetPercentage(Decimal? C_TargetPercentage)
{
    Set_Value("C_TargetPercentage", (Decimal?)C_TargetPercentage);
}
/** Get C_TargetPercentage.
@return C_TargetPercentage */
public Decimal GetC_TargetPercentage()
{
    Object bd = Get_Value("C_TargetPercentage");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_TargetQuantity.
@param C_TargetQuantity C_TargetQuantity */
public void SetC_TargetQuantity(Decimal? C_TargetQuantity)
{
    Set_Value("C_TargetQuantity", (Decimal?)C_TargetQuantity);
}
/** Get C_TargetQuantity.
@return C_TargetQuantity */
public Decimal GetC_TargetQuantity()
{
    Object bd = Get_Value("C_TargetQuantity");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_ThresholdAmount.
@param C_ThresholdAmount C_ThresholdAmount */
public void SetC_ThresholdAmount(Decimal? C_ThresholdAmount)
{
    Set_Value("C_ThresholdAmount", (Decimal?)C_ThresholdAmount);
}
/** Get C_ThresholdAmount.
@return C_ThresholdAmount */
public Decimal GetC_ThresholdAmount()
{
    Object bd = Get_Value("C_ThresholdAmount");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_ThresholdPercentage.
@param C_ThresholdPercentage C_ThresholdPercentage */
public void SetC_ThresholdPercentage(Decimal? C_ThresholdPercentage)
{
    Set_Value("C_ThresholdPercentage", (Decimal?)C_ThresholdPercentage);
}
/** Get C_ThresholdPercentage.
@return C_ThresholdPercentage */
public Decimal GetC_ThresholdPercentage()
{
    Object bd = Get_Value("C_ThresholdPercentage");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
/** Set C_ThresholdQuantity.
@param C_ThresholdQuantity C_ThresholdQuantity */
public void SetC_ThresholdQuantity(Decimal? C_ThresholdQuantity)
{
    Set_Value("C_ThresholdQuantity", (Decimal?)C_ThresholdQuantity);
}
/** Get C_ThresholdQuantity.
@return C_ThresholdQuantity */
public Decimal GetC_ThresholdQuantity()
{
    Object bd = Get_Value("C_ThresholdQuantity");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}
}

}
