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
/** Generated Model for VAF_TreeInfoChild
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_TreeInfoChild : PO
{
public X_VAF_TreeInfoChild (Context ctx, int VAF_TreeInfoChild_ID, Trx trxName) : base (ctx, VAF_TreeInfoChild_ID, trxName)
{
/** if (VAF_TreeInfoChild_ID == 0)
{
SetVAF_TreeInfo_ID (0);
SetNode_ID (0);
SetSeqNo (0);
}
 */
}
public X_VAF_TreeInfoChild (Ctx ctx, int VAF_TreeInfoChild_ID, Trx trxName) : base (ctx, VAF_TreeInfoChild_ID, trxName)
{
/** if (VAF_TreeInfoChild_ID == 0)
{
SetVAF_TreeInfo_ID (0);
SetNode_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoChild (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoChild (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoChild (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_TreeInfoChild()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364617L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047828L;
/** VAF_TableView_ID=289 */
public static int Table_ID;
 // =289;

/** TableName=VAF_TreeInfoChild */
public static String Table_Name="VAF_TreeInfoChild";

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
StringBuilder sb = new StringBuilder ("X_VAF_TreeInfoChild[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tree.
@param VAF_TreeInfo_ID Identifies a Tree */
public void SetVAF_TreeInfo_ID (int VAF_TreeInfo_ID)
{
if (VAF_TreeInfo_ID < 1) throw new ArgumentException ("VAF_TreeInfo_ID is mandatory.");
Set_ValueNoCheck ("VAF_TreeInfo_ID", VAF_TreeInfo_ID);
}
/** Get Tree.
@return Identifies a Tree */
public int GetVAF_TreeInfo_ID() 
{
Object ii = Get_Value("VAF_TreeInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_TreeInfo_ID().ToString());
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
//if (Parent_ID <= 0) Set_Value ("Parent_ID", null);
    //Manish
    if (Parent_ID < 0) Set_Value("Parent_ID", null);
        //end
else
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
