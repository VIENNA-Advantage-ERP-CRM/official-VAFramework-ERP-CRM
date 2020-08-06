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
/** Generated Model for M_DiscountSchema
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DiscountSchema : PO
{
public X_M_DiscountSchema (Context ctx, int M_DiscountSchema_ID, Trx trxName) : base (ctx, M_DiscountSchema_ID, trxName)
{
/** if (M_DiscountSchema_ID == 0)
{
SetDiscountType (null);
SetIsBPartnerFlatDiscount (false);
SetIsQuantityBased (true);	// Y
SetM_DiscountSchema_ID (0);
SetName (null);
SetValidFrom (DateTime.Now);
}
 */
}
public X_M_DiscountSchema (Ctx ctx, int M_DiscountSchema_ID, Trx trxName) : base (ctx, M_DiscountSchema_ID, trxName)
{
/** if (M_DiscountSchema_ID == 0)
{
SetDiscountType (null);
SetIsBPartnerFlatDiscount (false);
SetIsQuantityBased (true);	// Y
SetM_DiscountSchema_ID (0);
SetName (null);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchema (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchema (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchema (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DiscountSchema()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379083L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062294L;
/** AD_Table_ID=475 */
public static int Table_ID;
 // =475;

/** TableName=M_DiscountSchema */
public static String Table_Name="M_DiscountSchema";

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
StringBuilder sb = new StringBuilder ("X_M_DiscountSchema[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** CumulativeLevel AD_Reference_ID=246 */
public static int CUMULATIVELEVEL_AD_Reference_ID=246;
/** Line = L */
public static String CUMULATIVELEVEL_Line = "L";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCumulativeLevelValid (String test)
{
return test == null || test.Equals("L");
}
/** Set Accumulation Level.
@param CumulativeLevel Level for accumulative calculations */
public void SetCumulativeLevel (String CumulativeLevel)
{
if (!IsCumulativeLevelValid(CumulativeLevel))
throw new ArgumentException ("CumulativeLevel Invalid value - " + CumulativeLevel + " - Reference_ID=246 - L");
if (CumulativeLevel != null && CumulativeLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
CumulativeLevel = CumulativeLevel.Substring(0,1);
}
Set_Value ("CumulativeLevel", CumulativeLevel);
}
/** Get Accumulation Level.
@return Level for accumulative calculations */
public String GetCumulativeLevel() 
{
return (String)Get_Value("CumulativeLevel");
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

/** DiscountType AD_Reference_ID=247 */
public static int DISCOUNTTYPE_AD_Reference_ID=247;
/** Breaks = B */
public static String DISCOUNTTYPE_Breaks = "B";
/** Combination = C */
public static String DISCOUNTTYPE_Combination = "C";
/** Flat Percent = F */
public static String DISCOUNTTYPE_FlatPercent = "F";
/** Pricelist = P */
public static String DISCOUNTTYPE_Pricelist = "P";
/** Formula = S */
public static String DISCOUNTTYPE_Formula = "S";
/** Break and Combination = T */
public static String DISCOUNTTYPE_BreakAndCombination = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDiscountTypeValid (String test)
{
return test.Equals("B") || test.Equals("C") || test.Equals("F") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Discount Type.
@param DiscountType Type of trade discount calculation */
public void SetDiscountType (String DiscountType)
{
if (DiscountType == null) throw new ArgumentException ("DiscountType is mandatory");
if (!IsDiscountTypeValid(DiscountType))
throw new ArgumentException("DiscountType Invalid value - " + DiscountType + " - Reference_ID=247 - B - C - F - P - S - T");
if (DiscountType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DiscountType = DiscountType.Substring(0,1);
}
Set_Value ("DiscountType", DiscountType);
}
/** Get Discount Type.
@return Type of trade discount calculation */
public String GetDiscountType() 
{
return (String)Get_Value("DiscountType");
}
/** Set Flat Discount %.
@param FlatDiscount Flat discount percentage */
public void SetFlatDiscount (Decimal? FlatDiscount)
{
Set_Value ("FlatDiscount", (Decimal?)FlatDiscount);
}
/** Get Flat Discount %.
@return Flat discount percentage */
public Decimal GetFlatDiscount() 
{
Object bd =Get_Value("FlatDiscount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set B.Partner Flat Discount.
@param IsBPartnerFlatDiscount Use flat discount defined on Business Partner Level */
public void SetIsBPartnerFlatDiscount (Boolean IsBPartnerFlatDiscount)
{
Set_Value ("IsBPartnerFlatDiscount", IsBPartnerFlatDiscount);
}
/** Get B.Partner Flat Discount.
@return Use flat discount defined on Business Partner Level */
public Boolean IsBPartnerFlatDiscount() 
{
Object oo = Get_Value("IsBPartnerFlatDiscount");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Quantity based.
@param IsQuantityBased Trade discount break level based on Quantity (not value) */
public void SetIsQuantityBased (Boolean IsQuantityBased)
{
Set_Value ("IsQuantityBased", IsQuantityBased);
}
/** Get Quantity based.
@return Trade discount break level based on Quantity (not value) */
public Boolean IsQuantityBased() 
{
Object oo = Get_Value("IsQuantityBased");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Discount Schema.
@param M_DiscountSchema_ID Schema to calculate price lists or the trade discount percentage */
public void SetM_DiscountSchema_ID (int M_DiscountSchema_ID)
{
if (M_DiscountSchema_ID < 1) throw new ArgumentException ("M_DiscountSchema_ID is mandatory.");
Set_ValueNoCheck ("M_DiscountSchema_ID", M_DiscountSchema_ID);
}
/** Get Discount Schema.
@return Schema to calculate price lists or the trade discount percentage */
public int GetM_DiscountSchema_ID() 
{
Object ii = Get_Value("M_DiscountSchema_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
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
/** Set Script.
@param Script Dynamic Java Language Script to calculate result */
public void SetScript (String Script)
{
if (Script != null && Script.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Script = Script.Substring(0,2000);
}
Set_Value ("Script", Script);
}
/** Get Script.
@return Dynamic Java Language Script to calculate result */
public String GetScript() 
{
return (String)Get_Value("Script");
}
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
if (ValidFrom == null) throw new ArgumentException ("ValidFrom is mandatory.");
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
}

}
