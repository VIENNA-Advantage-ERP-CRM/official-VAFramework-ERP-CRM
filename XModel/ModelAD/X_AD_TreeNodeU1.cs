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
/** Generated Model for AD_TreeNodeU1
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_TreeNodeU1 : PO
{
public X_AD_TreeNodeU1 (Context ctx, int AD_TreeNodeU1_ID, Trx trxName) : base (ctx, AD_TreeNodeU1_ID, trxName)
{
/** if (AD_TreeNodeU1_ID == 0)
{
SetAD_Tree_ID (0);
SetNode_ID (0);
SetParent_ID (0);
SetSeqNo (0);
}
 */
}
public X_AD_TreeNodeU1 (Ctx ctx, int AD_TreeNodeU1_ID, Trx trxName) : base (ctx, AD_TreeNodeU1_ID, trxName)
{
/** if (AD_TreeNodeU1_ID == 0)
{
SetAD_Tree_ID (0);
SetNode_ID (0);
SetParent_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TreeNodeU1 (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TreeNodeU1 (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TreeNodeU1 (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_TreeNodeU1()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365103L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048314L;
/** AD_Table_ID=852 */
public static int Table_ID;
 // =852;

/** TableName=AD_TreeNodeU1 */
public static String Table_Name="AD_TreeNodeU1";

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
StringBuilder sb = new StringBuilder ("X_AD_TreeNodeU1[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tree.
@param AD_Tree_ID Identifies a Tree */
public void SetAD_Tree_ID (int AD_Tree_ID)
{
if (AD_Tree_ID < 1) throw new ArgumentException ("AD_Tree_ID is mandatory.");
Set_ValueNoCheck ("AD_Tree_ID", AD_Tree_ID);
}
/** Get Tree.
@return Identifies a Tree */
public int GetAD_Tree_ID() 
{
Object ii = Get_Value("AD_Tree_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Tree_ID().ToString());
}
/** Set Node_ID.
@param Node_ID Node_ID */
public void SetNode_ID (int Node_ID)
{
if (Node_ID < 0) throw new ArgumentException ("Node_ID is mandatory.");
Set_ValueNoCheck ("Node_ID", Node_ID);
}
/** Get Node_ID.
@return Node_ID */
public int GetNode_ID() 
{
Object ii = Get_Value("Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Parent.
@param Parent_ID Parent of Entity */
public void SetParent_ID (int Parent_ID)
{
if (Parent_ID < 1) throw new ArgumentException ("Parent_ID is mandatory.");
Set_Value ("Parent_ID", Parent_ID);
}
/** Get Parent.
@return Parent of Entity */
public int GetParent_ID() 
{
Object ii = Get_Value("Parent_ID");
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
}

}
