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
/** Generated Model for AD_UserQueryLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserQueryLine : PO
{
public X_AD_UserQueryLine (Context ctx, int AD_UserQueryLine_ID, Trx trxName) : base (ctx, AD_UserQueryLine_ID, trxName)
{
/** if (AD_UserQueryLine_ID == 0)
{
SetAD_UserQueryLine_ID (0);
SetAD_UserQuery_ID (0);
SetIsAnd (true);	// Y
SetKeyName (null);
SetKeyValue (null);
SetOperator (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_UserQueryLine WHERE AD_UserQuery_ID=@AD_UserQuery_ID@
SetValue1Name (null);
}
 */
}
public X_AD_UserQueryLine (Ctx ctx, int AD_UserQueryLine_ID, Trx trxName) : base (ctx, AD_UserQueryLine_ID, trxName)
{
/** if (AD_UserQueryLine_ID == 0)
{
SetAD_UserQueryLine_ID (0);
SetAD_UserQuery_ID (0);
SetIsAnd (true);	// Y
SetKeyName (null);
SetKeyValue (null);
SetOperator (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_UserQueryLine WHERE AD_UserQuery_ID=@AD_UserQuery_ID@
SetValue1Name (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserQueryLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserQueryLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserQueryLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserQueryLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365589L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048800L;
/** AD_Table_ID=981 */
public static int Table_ID;
 // =981;

/** TableName=AD_UserQueryLine */
public static String Table_Name="AD_UserQueryLine";

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
StringBuilder sb = new StringBuilder ("X_AD_UserQueryLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User Query Line.
@param AD_UserQueryLine_ID Line of the user query */
public void SetAD_UserQueryLine_ID (int AD_UserQueryLine_ID)
{
if (AD_UserQueryLine_ID < 1) throw new ArgumentException ("AD_UserQueryLine_ID is mandatory.");
Set_ValueNoCheck ("AD_UserQueryLine_ID", AD_UserQueryLine_ID);
}
/** Get User Query Line.
@return Line of the user query */
public int GetAD_UserQueryLine_ID() 
{
Object ii = Get_Value("AD_UserQueryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Query.
@param AD_UserQuery_ID Saved User Query */
public void SetAD_UserQuery_ID (int AD_UserQuery_ID)
{
if (AD_UserQuery_ID < 1) throw new ArgumentException ("AD_UserQuery_ID is mandatory.");
Set_ValueNoCheck ("AD_UserQuery_ID", AD_UserQuery_ID);
}
/** Get User Query.
@return Saved User Query */
public int GetAD_UserQuery_ID() 
{
Object ii = Get_Value("AD_UserQuery_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_UserQuery_ID().ToString());
}
/** Set And.
@param IsAnd Use AND Logic to concatinate lines (not OR) */
public void SetIsAnd (Boolean IsAnd)
{
Set_Value ("IsAnd", IsAnd);
}
/** Get And.
@return Use AND Logic to concatinate lines (not OR) */
public Boolean IsAnd() 
{
Object oo = Get_Value("IsAnd");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Key Name.
@param KeyName Key Name */
public void SetKeyName (String KeyName)
{
if (KeyName == null) throw new ArgumentException ("KeyName is mandatory.");
if (KeyName.Length > 60)
{
log.Warning("Length > 60 - truncated");
KeyName = KeyName.Substring(0,60);
}
Set_Value ("KeyName", KeyName);
}
/** Get Key Name.
@return Key Name */
public String GetKeyName() 
{
return (String)Get_Value("KeyName");
}
/** Set Key Value.
@param KeyValue Key Value */
public void SetKeyValue (String KeyValue)
{
if (KeyValue == null) throw new ArgumentException ("KeyValue is mandatory.");
if (KeyValue.Length > 255)
{
log.Warning("Length > 255 - truncated");
KeyValue = KeyValue.Substring(0,255);
}
Set_Value ("KeyValue", KeyValue);
}
/** Get Key Value.
@return Key Value */
public String GetKeyValue() 
{
return (String)Get_Value("KeyValue");
}
/** Set Operator.
@param Operator Operator */
public void SetOperator (String Operator)
{
if (Operator == null) throw new ArgumentException ("Operator is mandatory.");
if (Operator.Length > 20)
{
log.Warning("Length > 20 - truncated");
Operator = Operator.Substring(0,20);
}
Set_Value ("Operator", Operator);
}
/** Get Operator.
@return Operator */
public String GetOperator() 
{
return (String)Get_Value("Operator");
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
/** Set Value Name.
@param Value1Name Value Name */
public void SetValue1Name (String Value1Name)
{
if (Value1Name == null) throw new ArgumentException ("Value1Name is mandatory.");
if (Value1Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Value1Name = Value1Name.Substring(0,60);
}
Set_Value ("Value1Name", Value1Name);
}
/** Get Value Name.
@return Value Name */
public String GetValue1Name() 
{
return (String)Get_Value("Value1Name");
}
/** Set Value.
@param Value1Value Value */
public void SetValue1Value (String Value1Value)
{
if (Value1Value != null && Value1Value.Length > 255)
{
log.Warning("Length > 255 - truncated");
Value1Value = Value1Value.Substring(0,255);
}
Set_Value ("Value1Value", Value1Value);
}
/** Get Value.
@return Value */
public String GetValue1Value() 
{
return (String)Get_Value("Value1Value");
}
/** Set Value 2 Name.
@param Value2Name Value 2 Name */
public void SetValue2Name (String Value2Name)
{
if (Value2Name != null && Value2Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Value2Name = Value2Name.Substring(0,60);
}
Set_Value ("Value2Name", Value2Name);
}
/** Get Value 2 Name.
@return Value 2 Name */
public String GetValue2Name() 
{
return (String)Get_Value("Value2Name");
}
/** Set Value 2.
@param Value2Value Value 2 */
public void SetValue2Value (String Value2Value)
{
if (Value2Value != null && Value2Value.Length > 255)
{
log.Warning("Length > 255 - truncated");
Value2Value = Value2Value.Substring(0,255);
}
Set_Value ("Value2Value", Value2Value);
}
/** Get Value 2.
@return Value 2 */
public String GetValue2Value() 
{
return (String)Get_Value("Value2Value");
}

//<summary>
/// SetFull Day
///</summary>
///<param name="IsFullDay">Full Day</param>
public void SetIsFullDay(Boolean IsFullDay)
{
    Set_Value("IsFullDay", IsFullDay);
}

///<summary>
/// GetFull Day
///</summary>
///<returns> Full Day</returns>
public Boolean IsFullDay()
{
    Object oo = Get_Value("IsFullDay"); if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}

}

}
