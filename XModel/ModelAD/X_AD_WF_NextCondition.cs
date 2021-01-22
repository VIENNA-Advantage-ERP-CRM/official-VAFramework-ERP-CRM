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
/** Generated Model for VAF_WFlow_NextCondition
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlow_NextCondition : PO
{
public X_VAF_WFlow_NextCondition (Context ctx, int VAF_WFlow_NextCondition_ID, Trx trxName) : base (ctx, VAF_WFlow_NextCondition_ID, trxName)
{
/** if (VAF_WFlow_NextCondition_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_WFlow_NextCondition_ID (0);
SetVAF_WFlow_NextNode_ID (0);
SetAndOr (null);	// O
SetEntityType (null);	// U
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_WFlow_NextCondition WHERE VAF_WFlow_NextNode_ID=@VAF_WFlow_NextNode_ID@
SetValue (null);
}
 */
}
public X_VAF_WFlow_NextCondition (Ctx ctx, int VAF_WFlow_NextCondition_ID, Trx trxName) : base (ctx, VAF_WFlow_NextCondition_ID, trxName)
{
/** if (VAF_WFlow_NextCondition_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_WFlow_NextCondition_ID (0);
SetVAF_WFlow_NextNode_ID (0);
SetAndOr (null);	// O
SetEntityType (null);	// U
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_WFlow_NextCondition WHERE VAF_WFlow_NextNode_ID=@VAF_WFlow_NextNode_ID@
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextCondition (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextCondition (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextCondition (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlow_NextCondition()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366106L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049317L;
/** VAF_TableView_ID=706 */
public static int Table_ID;
 // =706;

/** TableName=VAF_WFlow_NextCondition */
public static String Table_Name="VAF_WFlow_NextCondition";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlow_NextCondition[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param VAF_Column_ID Column in the table */
public void SetVAF_Column_ID (int VAF_Column_ID)
{
if (VAF_Column_ID < 1) throw new ArgumentException ("VAF_Column_ID is mandatory.");
Set_Value ("VAF_Column_ID", VAF_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetVAF_Column_ID() 
{
Object ii = Get_Value("VAF_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Transition Condition.
@param VAF_WFlow_NextCondition_ID Workflow Node Transition Condition */
public void SetVAF_WFlow_NextCondition_ID (int VAF_WFlow_NextCondition_ID)
{
if (VAF_WFlow_NextCondition_ID < 1) throw new ArgumentException ("VAF_WFlow_NextCondition_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_NextCondition_ID", VAF_WFlow_NextCondition_ID);
}
/** Get Transition Condition.
@return Workflow Node Transition Condition */
public int GetVAF_WFlow_NextCondition_ID() 
{
Object ii = Get_Value("VAF_WFlow_NextCondition_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Node Transition.
@param VAF_WFlow_NextNode_ID Workflow Node Transition */
public void SetVAF_WFlow_NextNode_ID (int VAF_WFlow_NextNode_ID)
{
if (VAF_WFlow_NextNode_ID < 1) throw new ArgumentException ("VAF_WFlow_NextNode_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_NextNode_ID", VAF_WFlow_NextNode_ID);
}
/** Get Node Transition.
@return Workflow Node Transition */
public int GetVAF_WFlow_NextNode_ID() 
{
Object ii = Get_Value("VAF_WFlow_NextNode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AndOr VAF_Control_Ref_ID=204 */
public static int ANDOR_VAF_Control_Ref_ID=204;
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

/** EntityType VAF_Control_Ref_ID=389 */
public static int ENTITYTYPE_VAF_Control_Ref_ID=389;
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

/** Operation VAF_Control_Ref_ID=205 */
public static int OPERATION_VAF_Control_Ref_ID=205;
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
