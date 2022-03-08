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
/** Generated Model for M_DiscountSchemaBreak
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DiscountSchemaBreak : PO
{
public X_M_DiscountSchemaBreak (Context ctx, int M_DiscountSchemaBreak_ID, Trx trxName) : base (ctx, M_DiscountSchemaBreak_ID, trxName)
{
/** if (M_DiscountSchemaBreak_ID == 0)
{
SetBreakDiscount (0.0);
SetBreakValue (0.0);
SetIsBPartnerFlatDiscount (false);	// N
SetM_DiscountSchemaBreak_ID (0);
SetM_DiscountSchema_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_DiscountSchemaBreak WHERE M_DiscountSchema_ID=@M_DiscountSchema_ID@
}
 */
}
public X_M_DiscountSchemaBreak (Ctx ctx, int M_DiscountSchemaBreak_ID, Trx trxName) : base (ctx, M_DiscountSchemaBreak_ID, trxName)
{
/** if (M_DiscountSchemaBreak_ID == 0)
{
SetBreakDiscount (0.0);
SetBreakValue (0.0);
SetIsBPartnerFlatDiscount (false);	// N
SetM_DiscountSchemaBreak_ID (0);
SetM_DiscountSchema_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_DiscountSchemaBreak WHERE M_DiscountSchema_ID=@M_DiscountSchema_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaBreak (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaBreak (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DiscountSchemaBreak (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DiscountSchemaBreak()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379114L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062325L;
/** AD_Table_ID=476 */
public static int Table_ID;
 // =476;

/** TableName=M_DiscountSchemaBreak */
public static String Table_Name="M_DiscountSchemaBreak";

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
StringBuilder sb = new StringBuilder ("X_M_DiscountSchemaBreak[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Break Discount %.
@param BreakDiscount Trade Discount in Percent for the break level */
public void SetBreakDiscount (Decimal? BreakDiscount)
{
if (BreakDiscount == null) throw new ArgumentException ("BreakDiscount is mandatory.");
Set_Value ("BreakDiscount", (Decimal?)BreakDiscount);
}
/** Get Break Discount %.
@return Trade Discount in Percent for the break level */
public Decimal GetBreakDiscount() 
{
Object bd =Get_Value("BreakDiscount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Break Value.
@param BreakValue Low Value of trade discount break level */
public void SetBreakValue (Decimal? BreakValue)
{
if (BreakValue == null) throw new ArgumentException ("BreakValue is mandatory.");
Set_Value ("BreakValue", (Decimal?)BreakValue);
}
/** Get Break Value.
@return Low Value of trade discount break level */
public Decimal GetBreakValue() 
{
Object bd =Get_Value("BreakValue");
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
/** Set Discount Schema Break.
@param M_DiscountSchemaBreak_ID Trade Discount Break */
public void SetM_DiscountSchemaBreak_ID (int M_DiscountSchemaBreak_ID)
{
if (M_DiscountSchemaBreak_ID < 1) throw new ArgumentException ("M_DiscountSchemaBreak_ID is mandatory.");
Set_ValueNoCheck ("M_DiscountSchemaBreak_ID", M_DiscountSchemaBreak_ID);
}
/** Get Discount Schema Break.
@return Trade Discount Break */
public int GetM_DiscountSchemaBreak_ID() 
{
Object ii = Get_Value("M_DiscountSchemaBreak_ID");
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

    //-----------------------------------
    /** Set Discount Amount.
@param VA025_DiscountAmount Discount Amount */
public void SetVA025_DiscountAmount (Decimal? VA025_DiscountAmount){Set_Value ("VA025_DiscountAmount", (Decimal?)VA025_DiscountAmount);}/** Get Discount Amount.
@return Discount Amount */
public Decimal GetVA025_DiscountAmount() {Object bd =Get_Value("VA025_DiscountAmount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Discount Percentage(Amt).
@param VA025_DiscountPAmt Discount Percentage(Amt) */
public void SetVA025_DiscountPAmt (String VA025_DiscountPAmt){if (VA025_DiscountPAmt != null && VA025_DiscountPAmt.Length > 50){log.Warning("Length > 50 - truncated");VA025_DiscountPAmt = VA025_DiscountPAmt.Substring(0,50);}Set_Value ("VA025_DiscountPAmt", VA025_DiscountPAmt);}/** Get Discount Percentage(Amt).
@return Discount Percentage(Amt) */
public String GetVA025_DiscountPAmt() {return (String)Get_Value("VA025_DiscountPAmt");}
/** VA025_DiscountType AD_Reference_ID=1000485 */
public static int VA025_DISCOUNTTYPE_AD_Reference_ID=1000485;/** Amount = A */
public static String VA025_DISCOUNTTYPE_Amount = "A";/** Qty And Amount = B */
public static String VA025_DISCOUNTTYPE_QtyAndAmount = "B";/** Quantity = Q */
public static String VA025_DISCOUNTTYPE_Quantity = "Q";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVA025_DiscountTypeValid (String test){return test == null || test.Equals("A") || test.Equals("B") || test.Equals("Q");}/** Set Discount Type.
@param VA025_DiscountType Discount Type */
public void SetVA025_DiscountType (String VA025_DiscountType){if (!IsVA025_DiscountTypeValid(VA025_DiscountType))
throw new ArgumentException ("VA025_DiscountType Invalid value - " + VA025_DiscountType + " - Reference_ID=1000485 - A - B - Q");if (VA025_DiscountType != null && VA025_DiscountType.Length > 1){log.Warning("Length > 1 - truncated");VA025_DiscountType = VA025_DiscountType.Substring(0,1);}Set_Value ("VA025_DiscountType", VA025_DiscountType);}/** Get Discount Type.
@return Discount Type */
public String GetVA025_DiscountType() {return (String)Get_Value("VA025_DiscountType");}/** Set From Date.
@param VA025_FromDate From Date */
public void SetVA025_FromDate (DateTime? VA025_FromDate){Set_Value ("VA025_FromDate", (DateTime?)VA025_FromDate);}/** Get From Date.
@return From Date */
public DateTime? GetVA025_FromDate() {return (DateTime?)Get_Value("VA025_FromDate");}/** Set To Date.
@param VA025_ToDate To Date */
public void SetVA025_ToDate (DateTime? VA025_ToDate){Set_Value ("VA025_ToDate", (DateTime?)VA025_ToDate);}/** Get To Date.
@return To Date */
public DateTime? GetVA025_ToDate() {return (DateTime?)Get_Value("VA025_ToDate");}

}

}
