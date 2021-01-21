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
/** Generated Model for VAF_TreeInfoBar
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_TreeInfoBar : PO
{
public X_VAF_TreeInfoBar (Context ctx, int VAF_TreeInfoBar_ID, Trx trxName) : base (ctx, VAF_TreeInfoBar_ID, trxName)
{
/** if (VAF_TreeInfoBar_ID == 0)
{
SetVAF_TreeInfo_ID (0);
SetAD_User_ID (0);
SetNode_ID (0);
}
 */
}
public X_VAF_TreeInfoBar (Ctx ctx, int VAF_TreeInfoBar_ID, Trx trxName) : base (ctx, VAF_TreeInfoBar_ID, trxName)
{
/** if (VAF_TreeInfoBar_ID == 0)
{
SetVAF_TreeInfo_ID (0);
SetAD_User_ID (0);
SetNode_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoBar (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoBar (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TreeInfoBar (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_TreeInfoBar()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364602L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047813L;
/** VAF_TableView_ID=456 */
public static int Table_ID;
 // =456;

/** TableName=VAF_TreeInfoBar */
public static String Table_Name="VAF_TreeInfoBar";

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
StringBuilder sb = new StringBuilder ("X_VAF_TreeInfoBar[").Append(Get_ID()).Append("]");
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
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
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
}

}
