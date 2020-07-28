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
/** Generated Model for AD_Find
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Find : PO
{
public X_AD_Find (Context ctx, int AD_Find_ID, Trx trxName) : base (ctx, AD_Find_ID, trxName)
{
/** if (AD_Find_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Find_ID (0);
SetAndOr (null);	// A
SetFind_ID (0.0);
SetOperation (null);	// ==
SetValue (null);
}
 */
}
public X_AD_Find (Ctx ctx, int AD_Find_ID, Trx trxName) : base (ctx, AD_Find_ID, trxName)
{
/** if (AD_Find_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Find_ID (0);
SetAndOr (null);	// A
SetFind_ID (0.0);
SetOperation (null);	// ==
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Find (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Find (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Find (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Find()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361326L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044537L;
/** AD_Table_ID=404 */
public static int Table_ID;
 // =404;

/** TableName=AD_Find */
public static String Table_Name="AD_Find";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_Find[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Column_ID AD_Reference_ID=251 */
public static int AD_COLUMN_ID_AD_Reference_ID=251;
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID < 1) throw new ArgumentException ("AD_Column_ID is mandatory.");
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Find.
@param AD_Find_ID Find */
public void SetAD_Find_ID (int AD_Find_ID)
{
if (AD_Find_ID < 1) throw new ArgumentException ("AD_Find_ID is mandatory.");
Set_ValueNoCheck ("AD_Find_ID", AD_Find_ID);
}
/** Get Find.
@return Find */
public int GetAD_Find_ID() 
{
Object ii = Get_Value("AD_Find_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Find_ID().ToString());
}

/** AndOr AD_Reference_ID=204 */
public static int ANDOR_AD_Reference_ID=204;
/** And = A */
public static String ANDOR_And = "A";
/** Or = O */
public static String ANDOR_Or = "O";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAndOrValid (String test)
{
return test.Equals("A") || test.Equals("O");
}
/** Set And/Or.
@param AndOr Logical operation: AND or OR */
public void SetAndOr (String AndOr)
{
if (AndOr == null) throw new ArgumentException ("AndOr is mandatory");
if (!IsAndOrValid(AndOr))
throw new ArgumentException ("AndOr Invalid value - " + AndOr + " - Reference_ID=204 - A - O");
if (AndOr.Length > 1)
{
log.Warning("Length > 1 - truncated");
AndOr = AndOr.Substring(0,1);
}
Set_Value ("AndOr", AndOr);
}
/** Get And/Or.
@return Logical operation: AND or OR */
public String GetAndOr() 
{
return (String)Get_Value("AndOr");
}
/** Set Find_ID.
@param Find_ID Find_ID */
public void SetFind_ID (Decimal? Find_ID)
{
if (Find_ID == null) throw new ArgumentException ("Find_ID is mandatory.");
Set_Value ("Find_ID", (Decimal?)Find_ID);
}
/** Get Find_ID.
@return Find_ID */
public Decimal GetFind_ID() 
{
Object bd =Get_Value("Find_ID");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Operation AD_Reference_ID=205 */
public static int OPERATION_AD_Reference_ID=205;
/** != = != */
public static String OPERATION_NotEq = "!=";
/** < = << */
public static String OPERATION_Le = "<<";
/** <= = <= */
public static String OPERATION_LeEq = "<=";
/** = = == */
public static String OPERATION_Eq = "==";
/** >= = >= */
public static String OPERATION_GtEq = ">=";
/** > = >> */
public static String OPERATION_Gt = ">>";
/** |<x>| = AB */
public static String OPERATION_X = "AB";
/** sql = SQ */
public static String OPERATION_Sql = "SQ";
/** ~ = ~~ */
public static String OPERATION_Like = "~~";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsOperationValid (String test)
{
return test.Equals("!=") || test.Equals("<<") || test.Equals("<=") || test.Equals("==") || test.Equals(">=") || test.Equals(">>") || test.Equals("AB") || test.Equals("SQ") || test.Equals("~~");
}
/** Set Operation.
@param Operation Compare Operation */
public void SetOperation (String Operation)
{
if (Operation == null) throw new ArgumentException ("Operation is mandatory");
if (!IsOperationValid(Operation))
throw new ArgumentException ("Operation Invalid value - " + Operation + " - Reference_ID=205 - != - << - <= - == - >= - >> - AB - SQ - ~~");
if (Operation.Length > 2)
{
log.Warning("Length > 2 - truncated");
Operation = Operation.Substring(0,2);
}
Set_Value ("Operation", Operation);
}
/** Get Operation.
@return Compare Operation */
public String GetOperation() 
{
return (String)Get_Value("Operation");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Set Value To.
@param Value2 Value To */
public void SetValue2 (String Value2)
{
if (Value2 != null && Value2.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value2 = Value2.Substring(0,40);
}
Set_Value ("Value2", Value2);
}
/** Get Value To.
@return Value To */
public String GetValue2() 
{
return (String)Get_Value("Value2");
}
}

}
