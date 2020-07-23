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
/** Generated Model for M_DiscountSchemaLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DiscountSchemaLine : PO
{
public X_M_DiscountSchemaLine (Context ctx, int M_DiscountSchemaLine_ID, Trx trxName) : base (ctx, M_DiscountSchemaLine_ID, trxName)
{
/** if (M_DiscountSchemaLine_ID == 0)
{
SetC_ConversionType_ID (0);
SetConversionDate (DateTime.Now);	// @#Date@
SetLimit_AddAmt (0.0);
SetLimit_Base (null);	// X
SetLimit_Discount (0.0);
SetLimit_MaxAmt (0.0);
SetLimit_MinAmt (0.0);
SetLimit_Rounding (null);	// C
SetList_AddAmt (0.0);
SetList_Base (null);	// L
SetList_Discount (0.0);
SetList_MaxAmt (0.0);
SetList_MinAmt (0.0);
SetList_Rounding (null);	// C
SetM_DiscountSchemaLine_ID (0);
SetM_DiscountSchema_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_DiscountSchemaLine WHERE M_DiscountSchema_ID=@M_DiscountSchema_ID@
SetStd_AddAmt (0.0);
SetStd_Base (null);	// S
SetStd_Discount (0.0);
SetStd_MaxAmt (0.0);
SetStd_MinAmt (0.0);
SetStd_Rounding (null);	// C
}
 */
}
public X_M_DiscountSchemaLine (Ctx ctx, int M_DiscountSchemaLine_ID, Trx trxName) : base (ctx, M_DiscountSchemaLine_ID, trxName)
{
/** if (M_DiscountSchemaLine_ID == 0)
{
SetC_ConversionType_ID (0);
SetConversionDate (DateTime.Now);	// @#Date@
SetLimit_AddAmt (0.0);
SetLimit_Base (null);	// X
SetLimit_Discount (0.0);
SetLimit_MaxAmt (0.0);
SetLimit_MinAmt (0.0);
SetLimit_Rounding (null);	// C
SetList_AddAmt (0.0);
SetList_Base (null);	// L
SetList_Discount (0.0);
SetList_MaxAmt (0.0);
SetList_MinAmt (0.0);
SetList_Rounding (null);	// C
SetM_DiscountSchemaLine_ID (0);
SetM_DiscountSchema_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_DiscountSchemaLine WHERE M_DiscountSchema_ID=@M_DiscountSchema_ID@
SetStd_AddAmt (0.0);
SetStd_Base (null);	// S
SetStd_Discount (0.0);
SetStd_MaxAmt (0.0);
SetStd_MinAmt (0.0);
SetStd_Rounding (null);	// C
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DiscountSchemaLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379162L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062373L;
/** AD_Table_ID=477 */
public static int Table_ID;
 // =477;

/** TableName=M_DiscountSchemaLine */
public static String Table_Name="M_DiscountSchemaLine";

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
StringBuilder sb = new StringBuilder ("X_M_DiscountSchemaLine[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID)
{
if (C_ConversionType_ID < 1) throw new ArgumentException ("C_ConversionType_ID is mandatory.");
Set_Value ("C_ConversionType_ID", C_ConversionType_ID);
}
/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetC_ConversionType_ID() 
{
Object ii = Get_Value("C_ConversionType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Conversion Date.
@param ConversionDate Date for selecting conversion rate */
public void SetConversionDate (DateTime? ConversionDate)
{
if (ConversionDate == null) throw new ArgumentException ("ConversionDate is mandatory.");
Set_Value ("ConversionDate", (DateTime?)ConversionDate);
}
/** Get Conversion Date.
@return Date for selecting conversion rate */
public DateTime? GetConversionDate() 
{
return (DateTime?)Get_Value("ConversionDate");
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
/** Set Limit price Surcharge Amount.
@param Limit_AddAmt Amount added to the converted/copied price before multiplying */
public void SetLimit_AddAmt (Decimal? Limit_AddAmt)
{
if (Limit_AddAmt == null) throw new ArgumentException ("Limit_AddAmt is mandatory.");
Set_Value ("Limit_AddAmt", (Decimal?)Limit_AddAmt);
}
/** Get Limit price Surcharge Amount.
@return Amount added to the converted/copied price before multiplying */
public Decimal GetLimit_AddAmt() 
{
Object bd =Get_Value("Limit_AddAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Limit_Base AD_Reference_ID=194 */
public static int LIMIT_BASE_AD_Reference_ID=194;
/** Fixed Price = F */
public static String LIMIT_BASE_FixedPrice = "F";
/** List Price = L */
public static String LIMIT_BASE_ListPrice = "L";
/** Standard Price = S */
public static String LIMIT_BASE_StandardPrice = "S";
/** Limit (PO) Price = X */
public static String LIMIT_BASE_LimitPOPrice = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLimit_BaseValid (String test)
{
return test.Equals("F") || test.Equals("L") || test.Equals("S") || test.Equals("X");
}
/** Set Limit price Base.
@param Limit_Base Base price for calculation of the new price */
public void SetLimit_Base (String Limit_Base)
{
if (Limit_Base == null) throw new ArgumentException ("Limit_Base is mandatory");
if (!IsLimit_BaseValid(Limit_Base))
throw new ArgumentException ("Limit_Base Invalid value - " + Limit_Base + " - Reference_ID=194 - F - L - S - X");
if (Limit_Base.Length > 1)
{
log.Warning("Length > 1 - truncated");
Limit_Base = Limit_Base.Substring(0,1);
}
Set_Value ("Limit_Base", Limit_Base);
}
/** Get Limit price Base.
@return Base price for calculation of the new price */
public String GetLimit_Base() 
{
return (String)Get_Value("Limit_Base");
}
/** Set Limit price Discount %.
@param Limit_Discount Discount in percent to be subtracted from base, if negative it will be added to base price */
public void SetLimit_Discount (Decimal? Limit_Discount)
{
if (Limit_Discount == null) throw new ArgumentException ("Limit_Discount is mandatory.");
Set_Value ("Limit_Discount", (Decimal?)Limit_Discount);
}
/** Get Limit price Discount %.
@return Discount in percent to be subtracted from base, if negative it will be added to base price */
public Decimal GetLimit_Discount() 
{
Object bd =Get_Value("Limit_Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Fixed Limit Price.
@param Limit_Fixed Fixed Limit Price (not calculated) */
public void SetLimit_Fixed (Decimal? Limit_Fixed)
{
Set_Value ("Limit_Fixed", (Decimal?)Limit_Fixed);
}
/** Get Fixed Limit Price.
@return Fixed Limit Price (not calculated) */
public Decimal GetLimit_Fixed() 
{
Object bd =Get_Value("Limit_Fixed");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Limit price max Margin.
@param Limit_MaxAmt Maximum difference to original limit price;
 ignored if zero */
public void SetLimit_MaxAmt (Decimal? Limit_MaxAmt)
{
if (Limit_MaxAmt == null) throw new ArgumentException ("Limit_MaxAmt is mandatory.");
Set_Value ("Limit_MaxAmt", (Decimal?)Limit_MaxAmt);
}
/** Get Limit price max Margin.
@return Maximum difference to original limit price;
 ignored if zero */
public Decimal GetLimit_MaxAmt() 
{
Object bd =Get_Value("Limit_MaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Limit price min Margin.
@param Limit_MinAmt Minimum difference to original limit price;
 ignored if zero */
public void SetLimit_MinAmt (Decimal? Limit_MinAmt)
{
if (Limit_MinAmt == null) throw new ArgumentException ("Limit_MinAmt is mandatory.");
Set_Value ("Limit_MinAmt", (Decimal?)Limit_MinAmt);
}
/** Get Limit price min Margin.
@return Minimum difference to original limit price;
 ignored if zero */
public Decimal GetLimit_MinAmt() 
{
Object bd =Get_Value("Limit_MinAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Limit_Rounding AD_Reference_ID=155 */
public static int LIMIT_ROUNDING_AD_Reference_ID=155;
/** Whole Number .00 = 0 */
public static String LIMIT_ROUNDING_WholeNumber00 = "0";
/** Nickel .05, .10, .15, ... = 5 */
public static String LIMIT_ROUNDING_Nickel051015 = "5";
/** Currency Precision = C */
public static String LIMIT_ROUNDING_CurrencyPrecision = "C";
/** Dime .10, .20, .30, ... = D */
public static String LIMIT_ROUNDING_Dime102030 = "D";
/** No Rounding = N */
public static String LIMIT_ROUNDING_NoRounding = "N";
/** Quarter .25 .50 .75 = Q */
public static String LIMIT_ROUNDING_Quarter255075 = "Q";
/** Ten 10.00, 20.00, .. = T */
public static String LIMIT_ROUNDING_Ten10002000 = "T";
/** Hundred = h */
public static String LIMIT_ROUNDING_Hundred = "h";
/** Thousand = t */
public static String LIMIT_ROUNDING_Thousand = "t";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLimit_RoundingValid (String test)
{
return test.Equals("0") || test.Equals("5") || test.Equals("C") || test.Equals("D") || test.Equals("N") || test.Equals("Q") || test.Equals("T") || test.Equals("h") || test.Equals("t");
}
/** Set Limit price Rounding.
@param Limit_Rounding Rounding of the final result */
public void SetLimit_Rounding (String Limit_Rounding)
{
if (Limit_Rounding == null) throw new ArgumentException ("Limit_Rounding is mandatory");
if (!IsLimit_RoundingValid(Limit_Rounding))
throw new ArgumentException ("Limit_Rounding Invalid value - " + Limit_Rounding + " - Reference_ID=155 - 0 - 5 - C - D - N - Q - T - h - t");
if (Limit_Rounding.Length > 1)
{
log.Warning("Length > 1 - truncated");
Limit_Rounding = Limit_Rounding.Substring(0,1);
}
Set_Value ("Limit_Rounding", Limit_Rounding);
}
/** Get Limit price Rounding.
@return Rounding of the final result */
public String GetLimit_Rounding() 
{
return (String)Get_Value("Limit_Rounding");
}
/** Set List price Surcharge Amount.
@param List_AddAmt List Price Surcharge Amount */
public void SetList_AddAmt (Decimal? List_AddAmt)
{
if (List_AddAmt == null) throw new ArgumentException ("List_AddAmt is mandatory.");
Set_Value ("List_AddAmt", (Decimal?)List_AddAmt);
}
/** Get List price Surcharge Amount.
@return List Price Surcharge Amount */
public Decimal GetList_AddAmt() 
{
Object bd =Get_Value("List_AddAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** List_Base AD_Reference_ID=194 */
public static int LIST_BASE_AD_Reference_ID=194;
/** Fixed Price = F */
public static String LIST_BASE_FixedPrice = "F";
/** List Price = L */
public static String LIST_BASE_ListPrice = "L";
/** Standard Price = S */
public static String LIST_BASE_StandardPrice = "S";
/** Limit (PO) Price = X */
public static String LIST_BASE_LimitPOPrice = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsList_BaseValid (String test)
{
return test.Equals("F") || test.Equals("L") || test.Equals("S") || test.Equals("X");
}
/** Set List price Base.
@param List_Base Price used as the basis for price list calculations */
public void SetList_Base (String List_Base)
{
if (List_Base == null) throw new ArgumentException ("List_Base is mandatory");
if (!IsList_BaseValid(List_Base))
throw new ArgumentException ("List_Base Invalid value - " + List_Base + " - Reference_ID=194 - F - L - S - X");
if (List_Base.Length > 1)
{
log.Warning("Length > 1 - truncated");
List_Base = List_Base.Substring(0,1);
}
Set_Value ("List_Base", List_Base);
}
/** Get List price Base.
@return Price used as the basis for price list calculations */
public String GetList_Base() 
{
return (String)Get_Value("List_Base");
}
/** Set List price Discount %.
@param List_Discount Discount from list price as a percentage */
public void SetList_Discount (Decimal? List_Discount)
{
if (List_Discount == null) throw new ArgumentException ("List_Discount is mandatory.");
Set_Value ("List_Discount", (Decimal?)List_Discount);
}
/** Get List price Discount %.
@return Discount from list price as a percentage */
public Decimal GetList_Discount() 
{
Object bd =Get_Value("List_Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Fixed List Price.
@param List_Fixed Fixes List Price (not calculated) */
public void SetList_Fixed (Decimal? List_Fixed)
{
Set_Value ("List_Fixed", (Decimal?)List_Fixed);
}
/** Get Fixed List Price.
@return Fixes List Price (not calculated) */
public Decimal GetList_Fixed() 
{
Object bd =Get_Value("List_Fixed");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List price max Margin.
@param List_MaxAmt Maximum margin for a product */
public void SetList_MaxAmt (Decimal? List_MaxAmt)
{
if (List_MaxAmt == null) throw new ArgumentException ("List_MaxAmt is mandatory.");
Set_Value ("List_MaxAmt", (Decimal?)List_MaxAmt);
}
/** Get List price max Margin.
@return Maximum margin for a product */
public Decimal GetList_MaxAmt() 
{
Object bd =Get_Value("List_MaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List price min Margin.
@param List_MinAmt Minimum margin for a product */
public void SetList_MinAmt (Decimal? List_MinAmt)
{
if (List_MinAmt == null) throw new ArgumentException ("List_MinAmt is mandatory.");
Set_Value ("List_MinAmt", (Decimal?)List_MinAmt);
}
/** Get List price min Margin.
@return Minimum margin for a product */
public Decimal GetList_MinAmt() 
{
Object bd =Get_Value("List_MinAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** List_Rounding AD_Reference_ID=155 */
public static int LIST_ROUNDING_AD_Reference_ID=155;
/** Whole Number .00 = 0 */
public static String LIST_ROUNDING_WholeNumber00 = "0";
/** Nickel .05, .10, .15, ... = 5 */
public static String LIST_ROUNDING_Nickel051015 = "5";
/** Currency Precision = C */
public static String LIST_ROUNDING_CurrencyPrecision = "C";
/** Dime .10, .20, .30, ... = D */
public static String LIST_ROUNDING_Dime102030 = "D";
/** No Rounding = N */
public static String LIST_ROUNDING_NoRounding = "N";
/** Quarter .25 .50 .75 = Q */
public static String LIST_ROUNDING_Quarter255075 = "Q";
/** Ten 10.00, 20.00, .. = T */
public static String LIST_ROUNDING_Ten10002000 = "T";
/** Hundred = h */
public static String LIST_ROUNDING_Hundred = "h";
/** Thousand = t */
public static String LIST_ROUNDING_Thousand = "t";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsList_RoundingValid (String test)
{
return test.Equals("0") || test.Equals("5") || test.Equals("C") || test.Equals("D") || test.Equals("N") || test.Equals("Q") || test.Equals("T") || test.Equals("h") || test.Equals("t");
}
/** Set List price Rounding.
@param List_Rounding Rounding rule for final list price */
public void SetList_Rounding (String List_Rounding)
{
if (List_Rounding == null) throw new ArgumentException ("List_Rounding is mandatory");
if (!IsList_RoundingValid(List_Rounding))
throw new ArgumentException ("List_Rounding Invalid value - " + List_Rounding + " - Reference_ID=155 - 0 - 5 - C - D - N - Q - T - h - t");
if (List_Rounding.Length > 1)
{
log.Warning("Length > 1 - truncated");
List_Rounding = List_Rounding.Substring(0,1);
}
Set_Value ("List_Rounding", List_Rounding);
}
/** Get List price Rounding.
@return Rounding rule for final list price */
public String GetList_Rounding() 
{
return (String)Get_Value("List_Rounding");
}
/** Set Discount Pricelist.
@param M_DiscountSchemaLine_ID Line of the pricelist trade discount schema */
public void SetM_DiscountSchemaLine_ID (int M_DiscountSchemaLine_ID)
{
if (M_DiscountSchemaLine_ID < 1) throw new ArgumentException ("M_DiscountSchemaLine_ID is mandatory.");
Set_ValueNoCheck ("M_DiscountSchemaLine_ID", M_DiscountSchemaLine_ID);
}
/** Get Discount Pricelist.
@return Line of the pricelist trade discount schema */
public int GetM_DiscountSchemaLine_ID() 
{
Object ii = Get_Value("M_DiscountSchemaLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Standard price Surcharge Amount.
@param Std_AddAmt Amount added to a price as a surcharge */
public void SetStd_AddAmt (Decimal? Std_AddAmt)
{
if (Std_AddAmt == null) throw new ArgumentException ("Std_AddAmt is mandatory.");
Set_Value ("Std_AddAmt", (Decimal?)Std_AddAmt);
}
/** Get Standard price Surcharge Amount.
@return Amount added to a price as a surcharge */
public Decimal GetStd_AddAmt() 
{
Object bd =Get_Value("Std_AddAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Std_Base AD_Reference_ID=194 */
public static int STD_BASE_AD_Reference_ID=194;
/** Fixed Price = F */
public static String STD_BASE_FixedPrice = "F";
/** List Price = L */
public static String STD_BASE_ListPrice = "L";
/** Standard Price = S */
public static String STD_BASE_StandardPrice = "S";
/** Limit (PO) Price = X */
public static String STD_BASE_LimitPOPrice = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsStd_BaseValid (String test)
{
return test.Equals("F") || test.Equals("L") || test.Equals("S") || test.Equals("X");
}
/** Set Standard price Base.
@param Std_Base Base price for calculating new standard price */
public void SetStd_Base (String Std_Base)
{
if (Std_Base == null) throw new ArgumentException ("Std_Base is mandatory");
if (!IsStd_BaseValid(Std_Base))
throw new ArgumentException ("Std_Base Invalid value - " + Std_Base + " - Reference_ID=194 - F - L - S - X");
if (Std_Base.Length > 1)
{
log.Warning("Length > 1 - truncated");
Std_Base = Std_Base.Substring(0,1);
}
Set_Value ("Std_Base", Std_Base);
}
/** Get Standard price Base.
@return Base price for calculating new standard price */
public String GetStd_Base() 
{
return (String)Get_Value("Std_Base");
}
/** Set Standard price Discount %.
@param Std_Discount Discount percentage to subtract from base price */
public void SetStd_Discount (Decimal? Std_Discount)
{
if (Std_Discount == null) throw new ArgumentException ("Std_Discount is mandatory.");
Set_Value ("Std_Discount", (Decimal?)Std_Discount);
}
/** Get Standard price Discount %.
@return Discount percentage to subtract from base price */
public Decimal GetStd_Discount() 
{
Object bd =Get_Value("Std_Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Fixed Standard Price.
@param Std_Fixed Fixed Standard Price (not calculated) */
public void SetStd_Fixed (Decimal? Std_Fixed)
{
Set_Value ("Std_Fixed", (Decimal?)Std_Fixed);
}
/** Get Fixed Standard Price.
@return Fixed Standard Price (not calculated) */
public Decimal GetStd_Fixed() 
{
Object bd =Get_Value("Std_Fixed");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard max Margin.
@param Std_MaxAmt Maximum margin allowed for a product */
public void SetStd_MaxAmt (Decimal? Std_MaxAmt)
{
if (Std_MaxAmt == null) throw new ArgumentException ("Std_MaxAmt is mandatory.");
Set_Value ("Std_MaxAmt", (Decimal?)Std_MaxAmt);
}
/** Get Standard max Margin.
@return Maximum margin allowed for a product */
public Decimal GetStd_MaxAmt() 
{
Object bd =Get_Value("Std_MaxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard price min Margin.
@param Std_MinAmt Minimum margin allowed for a product */
public void SetStd_MinAmt (Decimal? Std_MinAmt)
{
if (Std_MinAmt == null) throw new ArgumentException ("Std_MinAmt is mandatory.");
Set_Value ("Std_MinAmt", (Decimal?)Std_MinAmt);
}
/** Get Standard price min Margin.
@return Minimum margin allowed for a product */
public Decimal GetStd_MinAmt() 
{
Object bd =Get_Value("Std_MinAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Std_Rounding AD_Reference_ID=155 */
public static int STD_ROUNDING_AD_Reference_ID=155;
/** Whole Number .00 = 0 */
public static String STD_ROUNDING_WholeNumber00 = "0";
/** Nickel .05, .10, .15, ... = 5 */
public static String STD_ROUNDING_Nickel051015 = "5";
/** Currency Precision = C */
public static String STD_ROUNDING_CurrencyPrecision = "C";
/** Dime .10, .20, .30, ... = D */
public static String STD_ROUNDING_Dime102030 = "D";
/** No Rounding = N */
public static String STD_ROUNDING_NoRounding = "N";
/** Quarter .25 .50 .75 = Q */
public static String STD_ROUNDING_Quarter255075 = "Q";
/** Ten 10.00, 20.00, .. = T */
public static String STD_ROUNDING_Ten10002000 = "T";
/** Hundred = h */
public static String STD_ROUNDING_Hundred = "h";
/** Thousand = t */
public static String STD_ROUNDING_Thousand = "t";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsStd_RoundingValid (String test)
{
return test.Equals("0") || test.Equals("5") || test.Equals("C") || test.Equals("D") || test.Equals("N") || test.Equals("Q") || test.Equals("T") || test.Equals("h") || test.Equals("t");
}
/** Set Standard price Rounding.
@param Std_Rounding Rounding rule for calculated price */
public void SetStd_Rounding (String Std_Rounding)
{
if (Std_Rounding == null) throw new ArgumentException ("Std_Rounding is mandatory");
if (!IsStd_RoundingValid(Std_Rounding))
throw new ArgumentException ("Std_Rounding Invalid value - " + Std_Rounding + " - Reference_ID=155 - 0 - 5 - C - D - N - Q - T - h - t");
if (Std_Rounding.Length > 1)
{
log.Warning("Length > 1 - truncated");
Std_Rounding = Std_Rounding.Substring(0,1);
}
Set_Value ("Std_Rounding", Std_Rounding);
}
/** Get Standard price Rounding.
@return Rounding rule for calculated price */
public String GetStd_Rounding() 
{
return (String)Get_Value("Std_Rounding");
}
/** Set Formula Based.
@param IsLimitFormula Formula Based */
public void SetIsLimitFormula(Boolean IsLimitFormula)
{
    Set_Value("IsLimitFormula", IsLimitFormula);
}
/** Get Formula Based.
@return Formula Based */
public Boolean IsLimitFormula()
{
    Object oo = Get_Value("IsLimitFormula");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Formula Based.
@param IsListFormula Formula Based */
public void SetIsListFormula(Boolean IsListFormula)
{
    Set_Value("IsListFormula", IsListFormula);
}
/** Get Formula Based.
@return Formula Based */
public Boolean IsListFormula()
{
    Object oo = Get_Value("IsListFormula");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Formula Based.
@param IsStdFormula Formula Based */
public void SetIsStdFormula(Boolean IsStdFormula)
{
    Set_Value("IsStdFormula", IsStdFormula);
}
/** Get Formula Based.
@return Formula Based */
public Boolean IsStdFormula()
{
    Object oo = Get_Value("IsStdFormula");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Limit Price Discount Formula.
@param LimitFormula Limit Price Discount Formula */
public void SetLimitFormula(String LimitFormula)
{
    if (LimitFormula != null && LimitFormula.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        LimitFormula = LimitFormula.Substring(0, 50);
    }
    Set_Value("LimitFormula", LimitFormula);
}
/** Get Limit Price Discount Formula.
@return Limit Price Discount Formula */
public String GetLimitFormula()
{
    return (String)Get_Value("LimitFormula");
}
/** Set List Price Discount Formula.
@param ListFormula List Price Discount Formula */
public void SetListFormula(String ListFormula)
{
    if (ListFormula != null && ListFormula.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        ListFormula = ListFormula.Substring(0, 50);
    }
    Set_Value("ListFormula", ListFormula);
}
/** Get List Price Discount Formula.
@return List Price Discount Formula */
public String GetListFormula()
{
    return (String)Get_Value("ListFormula");
}
/** Set Standard Price Discount Formula.
@param StdFormula Standard Price Discount Formula */
public void SetStdFormula(String StdFormula)
{
    if (StdFormula != null && StdFormula.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        StdFormula = StdFormula.Substring(0, 50);
    }
    Set_Value("StdFormula", StdFormula);
}
/** Get Standard Price Discount Formula.
@return Standard Price Discount Formula */
public String GetStdFormula()
{
    return (String)Get_Value("StdFormula");
}
}

}
