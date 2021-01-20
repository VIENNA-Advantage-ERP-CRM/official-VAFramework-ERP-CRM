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
/** Generated Model for VAF_AllotCriteria
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AllotCriteria : PO
{
public X_VAF_AllotCriteria (Context ctx, int VAF_AllotCriteria_ID, Trx trxName) : base (ctx, VAF_AllotCriteria_ID, trxName)
{
/** if (VAF_AllotCriteria_ID == 0)
{
SetVAF_AllotCriteria_ID (0);
SetVAF_AllotTarget_ID (0);
SetAD_SourceColumn_ID (0);
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_AllotCriteria WHERE VAF_AllotTarget_ID=@VAF_AllotTarget_ID@
}
 */
}
public X_VAF_AllotCriteria (Ctx ctx, int VAF_AllotCriteria_ID, Trx trxName) : base (ctx, VAF_AllotCriteria_ID, trxName)
{
/** if (VAF_AllotCriteria_ID == 0)
{
SetVAF_AllotCriteria_ID (0);
SetVAF_AllotTarget_ID (0);
SetAD_SourceColumn_ID (0);
SetOperation (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_AllotCriteria WHERE VAF_AllotTarget_ID=@VAF_AllotTarget_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotCriteria (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotCriteria (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotCriteria (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AllotCriteria()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360448L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043659L;
/** VAF_TableView_ID=932 */
public static int Table_ID;
 // =932;

/** TableName=VAF_AllotCriteria */
public static String Table_Name="VAF_AllotCriteria";

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
StringBuilder sb = new StringBuilder ("X_VAF_AllotCriteria[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Assign Criteria.
@param VAF_AllotCriteria_ID Auto assignment Criteria */
public void SetVAF_AllotCriteria_ID (int VAF_AllotCriteria_ID)
{
if (VAF_AllotCriteria_ID < 1) throw new ArgumentException ("VAF_AllotCriteria_ID is mandatory.");
Set_ValueNoCheck ("VAF_AllotCriteria_ID", VAF_AllotCriteria_ID);
}
/** Get Assign Criteria.
@return Auto assignment Criteria */
public int GetVAF_AllotCriteria_ID() 
{
Object ii = Get_Value("VAF_AllotCriteria_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Assign Target.
@param VAF_AllotTarget_ID Automatic Assignment Target Column */
public void SetVAF_AllotTarget_ID (int VAF_AllotTarget_ID)
{
if (VAF_AllotTarget_ID < 1) throw new ArgumentException ("VAF_AllotTarget_ID is mandatory.");
Set_ValueNoCheck ("VAF_AllotTarget_ID", VAF_AllotTarget_ID);
}
/** Get Assign Target.
@return Automatic Assignment Target Column */
public int GetVAF_AllotTarget_ID() 
{
Object ii = Get_Value("VAF_AllotTarget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_SourceColumn_ID AD_Reference_ID=414 */
public static int AD_SOURCECOLUMN_ID_AD_Reference_ID=414;
/** Set Source Column.
@param AD_SourceColumn_ID The column used as the criteria */
public void SetAD_SourceColumn_ID (int AD_SourceColumn_ID)
{
if (AD_SourceColumn_ID < 1) throw new ArgumentException ("AD_SourceColumn_ID is mandatory.");
Set_Value ("AD_SourceColumn_ID", AD_SourceColumn_ID);
}
/** Get Source Column.
@return The column used as the criteria */
public int GetAD_SourceColumn_ID() 
{
Object ii = Get_Value("AD_SourceColumn_ID");
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
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
/** Set Value.
@param ValueString Value as String */
public void SetValueString (String ValueString)
{
if (ValueString != null && ValueString.Length > 40)
{
log.Warning("Length > 40 - truncated");
ValueString = ValueString.Substring(0,40);
}
Set_Value ("ValueString", ValueString);
}
/** Get Value.
@return Value as String */
public String GetValueString() 
{
return (String)Get_Value("ValueString");
}
}

}
