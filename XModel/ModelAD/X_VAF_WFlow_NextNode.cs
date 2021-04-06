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
/** Generated Model for VAF_WFlow_NextNode
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlow_NextNode : PO
{
public X_VAF_WFlow_NextNode (Context ctx, int VAF_WFlow_NextNode_ID, Trx trxName) : base (ctx, VAF_WFlow_NextNode_ID, trxName)
{
/** if (VAF_WFlow_NextNode_ID == 0)
{
SetAD_WF_Next_ID (0);
SetVAF_WFlow_NextNode_ID (0);
SetVAF_WFlow_Node_ID (0);
SetRecordType (null);	// U
SetIsStdUserWorkflow (false);
SetSeqNo (0);	// 10
}
 */
}
public X_VAF_WFlow_NextNode (Ctx ctx, int VAF_WFlow_NextNode_ID, Trx trxName) : base (ctx, VAF_WFlow_NextNode_ID, trxName)
{
/** if (VAF_WFlow_NextNode_ID == 0)
{
SetAD_WF_Next_ID (0);
SetVAF_WFlow_NextNode_ID (0);
SetVAF_WFlow_Node_ID (0);
SetRecordType (null);	// U
SetIsStdUserWorkflow (false);
SetSeqNo (0);	// 10
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextNode (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextNode (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_NextNode (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlow_NextNode()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366263L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049474L;
/** VAF_TableView_ID=131 */
public static int Table_ID;
 // =131;

/** TableName=VAF_WFlow_NextNode */
public static String Table_Name="VAF_WFlow_NextNode";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlow_NextNode[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_WF_Next_ID VAF_Control_Ref_ID=109 */
public static int AD_WF_NEXT_ID_VAF_Control_Ref_ID=109;
/** Set Next Node.
@param AD_WF_Next_ID Next Node in workflow */
public void SetAD_WF_Next_ID (int AD_WF_Next_ID)
{
if (AD_WF_Next_ID < 1) throw new ArgumentException ("AD_WF_Next_ID is mandatory.");
Set_Value ("AD_WF_Next_ID", AD_WF_Next_ID);
}
/** Get Next Node.
@return Next Node in workflow */
public int GetAD_WF_Next_ID() 
{
Object ii = Get_Value("AD_WF_Next_ID");
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
/** Set Node.
@param VAF_WFlow_Node_ID Workflow Node (activity), step or process */
public void SetVAF_WFlow_Node_ID (int VAF_WFlow_Node_ID)
{
if (VAF_WFlow_Node_ID < 1) throw new ArgumentException ("VAF_WFlow_Node_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_Node_ID", VAF_WFlow_Node_ID);
}
/** Get Node.
@return Workflow Node (activity), step or process */
public int GetVAF_WFlow_Node_ID() 
{
Object ii = Get_Value("VAF_WFlow_Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_WFlow_Node_ID().ToString());
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

/** RecordType VAF_Control_Ref_ID=389 */
public static int RecordType_VAF_Control_Ref_ID=389;
/** Set Entity Type.
@param RecordType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetRecordType (String RecordType)
{
if (RecordType.Length > 4)
{
log.Warning("Length > 4 - truncated");
RecordType = RecordType.Substring(0,4);
}
Set_Value ("RecordType", RecordType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetRecordType() 
{
return (String)Get_Value("RecordType");
}
/** Set Std User Workflow.
@param IsStdUserWorkflow Standard Manual User Approval Workflow */
public void SetIsStdUserWorkflow (Boolean IsStdUserWorkflow)
{
Set_Value ("IsStdUserWorkflow", IsStdUserWorkflow);
}
/** Get Std User Workflow.
@return Standard Manual User Approval Workflow */
public Boolean IsStdUserWorkflow() 
{
Object oo = Get_Value("IsStdUserWorkflow");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Transition Code.
@param TransitionCode Code resulting in TRUE of FALSE */
public void SetTransitionCode (String TransitionCode)
{
if (TransitionCode != null && TransitionCode.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TransitionCode = TransitionCode.Substring(0,2000);
}
Set_Value ("TransitionCode", TransitionCode);
}
/** Get Transition Code.
@return Code resulting in TRUE of FALSE */
public String GetTransitionCode() 
{
return (String)Get_Value("TransitionCode");
}
}

}
