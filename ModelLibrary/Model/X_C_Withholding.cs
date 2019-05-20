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
/** Generated Model for C_Withholding
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Withholding : PO
{
public X_C_Withholding (Context ctx, int C_Withholding_ID, Trx trxName) : base (ctx, C_Withholding_ID, trxName)
{
/** if (C_Withholding_ID == 0)
{
SetC_PaymentTerm_ID (0);
SetC_Withholding_ID (0);
SetIsPaidTo3Party (false);
SetIsPercentWithholding (false);
SetIsTaxProrated (false);
SetIsTaxWithholding (false);
SetName (null);
}
 */
}
public X_C_Withholding (Ctx ctx, int C_Withholding_ID, Trx trxName) : base (ctx, C_Withholding_ID, trxName)
{
/** if (C_Withholding_ID == 0)
{
SetC_PaymentTerm_ID (0);
SetC_Withholding_ID (0);
SetIsPaidTo3Party (false);
SetIsPercentWithholding (false);
SetIsTaxProrated (false);
SetIsTaxWithholding (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Withholding()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375902L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059113L;
/** AD_Table_ID=304 */
public static int Table_ID;
 // =304;

/** TableName=C_Withholding */
public static String Table_Name="C_Withholding";

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
StringBuilder sb = new StringBuilder ("X_C_Withholding[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** Beneficiary_ID AD_Reference_ID=138 */
public static int BENEFICIARY_ID_AD_Reference_ID=138;
/** Set Beneficiary.
@param Beneficiary_ID Business Partner to whom payment is made */
public void SetBeneficiary_ID (int Beneficiary_ID)
{
if (Beneficiary_ID <= 0) Set_Value ("Beneficiary_ID", null);
else
Set_Value ("Beneficiary_ID", Beneficiary_ID);
}
/** Get Beneficiary.
@return Business Partner to whom payment is made */
public int GetBeneficiary_ID() 
{
Object ii = Get_Value("Beneficiary_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID)
{
if (C_PaymentTerm_ID < 1) throw new ArgumentException ("C_PaymentTerm_ID is mandatory.");
Set_Value ("C_PaymentTerm_ID", C_PaymentTerm_ID);
}
/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() 
{
Object ii = Get_Value("C_PaymentTerm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Withholding.
@param C_Withholding_ID Withholding type defined */
public void SetC_Withholding_ID (int C_Withholding_ID)
{
if (C_Withholding_ID < 1) throw new ArgumentException ("C_Withholding_ID is mandatory.");
Set_ValueNoCheck ("C_Withholding_ID", C_Withholding_ID);
}
/** Get Withholding.
@return Withholding type defined */
public int GetC_Withholding_ID() 
{
Object ii = Get_Value("C_Withholding_ID");
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
/** Set Fix amount.
@param FixAmt Fixed amount to be levied or paid */
public void SetFixAmt (Decimal? FixAmt)
{
Set_Value ("FixAmt", (Decimal?)FixAmt);
}
/** Get Fix amount.
@return Fixed amount to be levied or paid */
public Decimal GetFixAmt() 
{
Object bd =Get_Value("FixAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Paid to third party.
@param IsPaidTo3Party Amount paid to someone other than the Business Partner */
public void SetIsPaidTo3Party (Boolean IsPaidTo3Party)
{
Set_Value ("IsPaidTo3Party", IsPaidTo3Party);
}
/** Get Paid to third party.
@return Amount paid to someone other than the Business Partner */
public Boolean IsPaidTo3Party() 
{
Object oo = Get_Value("IsPaidTo3Party");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Percent withholding.
@param IsPercentWithholding Withholding amount is a percentage of the invoice amount */
public void SetIsPercentWithholding (Boolean IsPercentWithholding)
{
Set_Value ("IsPercentWithholding", IsPercentWithholding);
}
/** Get Percent withholding.
@return Withholding amount is a percentage of the invoice amount */
public Boolean IsPercentWithholding() 
{
Object oo = Get_Value("IsPercentWithholding");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Prorate tax.
@param IsTaxProrated Tax is Prorated */
public void SetIsTaxProrated (Boolean IsTaxProrated)
{
Set_Value ("IsTaxProrated", IsTaxProrated);
}
/** Get Prorate tax.
@return Tax is Prorated */
public Boolean IsTaxProrated() 
{
Object oo = Get_Value("IsTaxProrated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tax withholding.
@param IsTaxWithholding This is a tax related withholding */
public void SetIsTaxWithholding (Boolean IsTaxWithholding)
{
Set_Value ("IsTaxWithholding", IsTaxWithholding);
}
/** Get Tax withholding.
@return This is a tax related withholding */
public Boolean IsTaxWithholding() 
{
Object oo = Get_Value("IsTaxWithholding");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Max Amount.
@param MaxAmt Maximum Amount in invoice currency */
public void SetMaxAmt (Decimal? MaxAmt)
{
Set_Value ("MaxAmt", (Decimal?)MaxAmt);
}
/** Get Max Amount.
@return Maximum Amount in invoice currency */
public Decimal GetMaxAmt() 
{
Object bd =Get_Value("MaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Min Amount.
@param MinAmt Minimum Amount in invoice currency */
public void SetMinAmt (Decimal? MinAmt)
{
Set_Value ("MinAmt", (Decimal?)MinAmt);
}
/** Get Min Amount.
@return Minimum Amount in invoice currency */
public Decimal GetMinAmt() 
{
Object bd =Get_Value("MinAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Percent.
@param PercentWithholding Percent withholding */
public void SetPercentWithholding (Decimal? PercentWithholding)
{
Set_Value ("PercentWithholding", (Decimal?)PercentWithholding);
}
/** Get Percent.
@return Percent withholding */
public Decimal GetPercentWithholding() 
{
Object bd =Get_Value("PercentWithholding");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Threshold max.
@param ThresholdMax Maximum gross amount for withholding calculation  (0=no limit) */
public void SetThresholdMax (Decimal? ThresholdMax)
{
Set_Value ("ThresholdMax", (Decimal?)ThresholdMax);
}
/** Get Threshold max.
@return Maximum gross amount for withholding calculation  (0=no limit) */
public Decimal GetThresholdMax() 
{
Object bd =Get_Value("ThresholdMax");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Threshold min.
@param Thresholdmin Minimum gross amount for withholding calculation */
public void SetThresholdmin (Decimal? Thresholdmin)
{
Set_Value ("Thresholdmin", (Decimal?)Thresholdmin);
}
/** Get Threshold min.
@return Minimum gross amount for withholding calculation */
public Decimal GetThresholdmin() 
{
Object bd =Get_Value("Thresholdmin");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
