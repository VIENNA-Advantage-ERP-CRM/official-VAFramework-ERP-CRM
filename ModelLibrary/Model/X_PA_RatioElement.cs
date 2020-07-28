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
/** Generated Model for PA_RatioElement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_RatioElement : PO
{
public X_PA_RatioElement (Context ctx, int PA_RatioElement_ID, Trx trxName) : base (ctx, PA_RatioElement_ID, trxName)
{
/** if (PA_RatioElement_ID == 0)
{
SetName (null);
SetPA_RatioElement_ID (0);
SetPA_Ratio_ID (0);
SetRatioElementType (null);
SetRatioOperand (null);	// P
SetSeqNo (0);
}
 */
}
public X_PA_RatioElement (Ctx ctx, int PA_RatioElement_ID, Trx trxName) : base (ctx, PA_RatioElement_ID, trxName)
{
/** if (PA_RatioElement_ID == 0)
{
SetName (null);
SetPA_RatioElement_ID (0);
SetPA_Ratio_ID (0);
SetRatioElementType (null);
SetRatioOperand (null);	// P
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_RatioElement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_RatioElement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_RatioElement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_RatioElement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382014L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065225L;
/** AD_Table_ID=836 */
public static int Table_ID;
 // =836;

/** TableName=PA_RatioElement */
public static String Table_Name="PA_RatioElement";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_PA_RatioElement[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** Account_ID AD_Reference_ID=331 */
public static int ACCOUNT_ID_AD_Reference_ID=331;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID <= 0) Set_Value ("Account_ID", null);
else
Set_Value ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Constant Value.
@param ConstantValue Constant value */
public void SetConstantValue (Decimal? ConstantValue)
{
Set_Value ("ConstantValue", (Decimal?)ConstantValue);
}
/** Get Constant Value.
@return Constant value */
public Decimal GetConstantValue() 
{
Object bd =Get_Value("ConstantValue");
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
/** Set Measure Calculation.
@param PA_MeasureCalc_ID Calculation method for measuring performance */
public void SetPA_MeasureCalc_ID (int PA_MeasureCalc_ID)
{
if (PA_MeasureCalc_ID <= 0) Set_Value ("PA_MeasureCalc_ID", null);
else
Set_Value ("PA_MeasureCalc_ID", PA_MeasureCalc_ID);
}
/** Get Measure Calculation.
@return Calculation method for measuring performance */
public int GetPA_MeasureCalc_ID() 
{
Object ii = Get_Value("PA_MeasureCalc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Ratio Element.
@param PA_RatioElement_ID Performance Ratio Element */
public void SetPA_RatioElement_ID (int PA_RatioElement_ID)
{
if (PA_RatioElement_ID < 1) throw new ArgumentException ("PA_RatioElement_ID is mandatory.");
Set_ValueNoCheck ("PA_RatioElement_ID", PA_RatioElement_ID);
}
/** Get Ratio Element.
@return Performance Ratio Element */
public int GetPA_RatioElement_ID() 
{
Object ii = Get_Value("PA_RatioElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PA_RatioUsed_ID AD_Reference_ID=371 */
public static int PA_RATIOUSED_ID_AD_Reference_ID=371;
/** Set Ratio Used.
@param PA_RatioUsed_ID Performace Ratio Used */
public void SetPA_RatioUsed_ID (int PA_RatioUsed_ID)
{
if (PA_RatioUsed_ID <= 0) Set_Value ("PA_RatioUsed_ID", null);
else
Set_Value ("PA_RatioUsed_ID", PA_RatioUsed_ID);
}
/** Get Ratio Used.
@return Performace Ratio Used */
public int GetPA_RatioUsed_ID() 
{
Object ii = Get_Value("PA_RatioUsed_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Ratio.
@param PA_Ratio_ID Performace Ratio */
public void SetPA_Ratio_ID (int PA_Ratio_ID)
{
if (PA_Ratio_ID < 1) throw new ArgumentException ("PA_Ratio_ID is mandatory.");
Set_ValueNoCheck ("PA_Ratio_ID", PA_Ratio_ID);
}
/** Get Ratio.
@return Performace Ratio */
public int GetPA_Ratio_ID() 
{
Object ii = Get_Value("PA_Ratio_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PostingType AD_Reference_ID=125 */
public static int POSTINGTYPE_AD_Reference_ID=125;
/** Actual = A */
public static String POSTINGTYPE_Actual = "A";
/** Budget = B */
public static String POSTINGTYPE_Budget = "B";
/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";
/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";
/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType != null && PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_Value ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
}

/** RatioElementType AD_Reference_ID=372 */
public static int RATIOELEMENTTYPE_AD_Reference_ID=372;
/** Account Value = A */
public static String RATIOELEMENTTYPE_AccountValue = "A";
/** Constant = C */
public static String RATIOELEMENTTYPE_Constant = "C";
/** Ratio = R */
public static String RATIOELEMENTTYPE_Ratio = "R";
/** Calculation = X */
public static String RATIOELEMENTTYPE_Calculation = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRatioElementTypeValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("R") || test.Equals("X");
}
/** Set Element Type.
@param RatioElementType Ratio Element Type */
public void SetRatioElementType (String RatioElementType)
{
if (RatioElementType == null) throw new ArgumentException ("RatioElementType is mandatory");
if (!IsRatioElementTypeValid(RatioElementType))
throw new ArgumentException ("RatioElementType Invalid value - " + RatioElementType + " - Reference_ID=372 - A - C - R - X");
if (RatioElementType.Length > 1)
{
log.Warning("Length > 1 - truncated");
RatioElementType = RatioElementType.Substring(0,1);
}
Set_Value ("RatioElementType", RatioElementType);
}
/** Get Element Type.
@return Ratio Element Type */
public String GetRatioElementType() 
{
return (String)Get_Value("RatioElementType");
}

/** RatioOperand AD_Reference_ID=373 */
public static int RATIOOPERAND_AD_Reference_ID=373;
/** Divide = D */
public static String RATIOOPERAND_Divide = "D";
/** Multiply = M */
public static String RATIOOPERAND_Multiply = "M";
/** Minus = N */
public static String RATIOOPERAND_Minus = "N";
/** Plus = P */
public static String RATIOOPERAND_Plus = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRatioOperandValid (String test)
{
return test.Equals("D") || test.Equals("M") || test.Equals("N") || test.Equals("P");
}
/** Set Operand.
@param RatioOperand Ratio Operand */
public void SetRatioOperand (String RatioOperand)
{
if (RatioOperand == null) throw new ArgumentException ("RatioOperand is mandatory");
if (!IsRatioOperandValid(RatioOperand))
throw new ArgumentException ("RatioOperand Invalid value - " + RatioOperand + " - Reference_ID=373 - D - M - N - P");
if (RatioOperand.Length > 1)
{
log.Warning("Length > 1 - truncated");
RatioOperand = RatioOperand.Substring(0,1);
}
Set_Value ("RatioOperand", RatioOperand);
}
/** Get Operand.
@return Ratio Operand */
public String GetRatioOperand() 
{
return (String)Get_Value("RatioOperand");
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
}

}
