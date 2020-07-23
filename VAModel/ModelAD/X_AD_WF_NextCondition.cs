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
/** Generated Model for AD_WF_NextCondition
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_NextCondition : PO
{
public X_AD_WF_NextCondition (Context ctx, int AD_WF_NextCondition_ID, Trx trxName) : base (ctx, AD_WF_NextCondition_ID, trxName)
{
/** if (AD_WF_NextCondition_ID == 0)
{
SetAD_Column_ID (0);
SetAD_WF_NextCondition_ID (0);
SetAD_WF_NodeNext_ID (0);
SetAndOr (null);	// O
SetEntityType (null);	// U
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_WF_NextCondition WHERE AD_WF_NodeNext_ID=@AD_WF_NodeNext_ID@
SetValue (null);
}
 */
}
public X_AD_WF_NextCondition (Ctx ctx, int AD_WF_NextCondition_ID, Trx trxName) : base (ctx, AD_WF_NextCondition_ID, trxName)
{
/** if (AD_WF_NextCondition_ID == 0)
{
SetAD_Column_ID (0);
SetAD_WF_NextCondition_ID (0);
SetAD_WF_NodeNext_ID (0);
SetAndOr (null);	// O
SetEntityType (null);	// U
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_WF_NextCondition WHERE AD_WF_NodeNext_ID=@AD_WF_NodeNext_ID@
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_NextCondition (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_NextCondition (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_NextCondition (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_NextCondition()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366106L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049317L;
/** AD_Table_ID=706 */
public static int Table_ID;
 // =706;

/** TableName=AD_WF_NextCondition */
public static String Table_Name="AD_WF_NextCondition";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_AD_WF_NextCondition[").Append(Get_ID()).Append("]");
return sb.ToString();
}
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
/** Set Transition Condition.
@param AD_WF_NextCondition_ID Workflow Node Transition Condition */
public void SetAD_WF_NextCondition_ID (int AD_WF_NextCondition_ID)
{
if (AD_WF_NextCondition_ID < 1) throw new ArgumentException ("AD_WF_NextCondition_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_NextCondition_ID", AD_WF_NextCondition_ID);
}
/** Get Transition Condition.
@return Workflow Node Transition Condition */
public int GetAD_WF_NextCondition_ID() 
{
Object ii = Get_Value("AD_WF_NextCondition_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Node Transition.
@param AD_WF_NodeNext_ID Workflow Node Transition */
public void SetAD_WF_NodeNext_ID (int AD_WF_NodeNext_ID)
{
if (AD_WF_NodeNext_ID < 1) throw new ArgumentException ("AD_WF_NodeNext_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_NodeNext_ID", AD_WF_NodeNext_ID);
}
/** Get Node Transition.
@return Workflow Node Transition */
public int GetAD_WF_NodeNext_ID() 
{
Object ii = Get_Value("AD_WF_NodeNext_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetValue());
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



/** Set C_GenAttributeSetInstance_ID.
@param C_GenAttributeSetInstance_ID C_GenAttributeSetInstance_ID */
public void SetC_GenAttributeSetInstance_ID(Object C_GenAttributeSetInstance_ID)
{
    Set_Value("C_GenAttributeSetInstance_ID", C_GenAttributeSetInstance_ID);
}
/** Get C_GenAttributeSetInstance_ID.
@return C_GenAttributeSetInstance_ID */
public Object GetC_GenAttributeSetInstance_ID()
{
    return Get_Value("C_GenAttributeSetInstance_ID");
}
/** Set C_GenAttributeSet_ID.
@param C_GenAttributeSet_ID C_GenAttributeSet_ID */
public void SetC_GenAttributeSet_ID(int C_GenAttributeSet_ID)
{
    if (C_GenAttributeSet_ID <= 0) Set_Value("C_GenAttributeSet_ID", null);
    else
        Set_Value("C_GenAttributeSet_ID", C_GenAttributeSet_ID);
}
/** Get C_GenAttributeSet_ID.
@return C_GenAttributeSet_ID */
public int GetC_GenAttributeSet_ID()
{
    Object ii = Get_Value("C_GenAttributeSet_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
