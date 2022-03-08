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
/** Generated Model for AD_WF_Block
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_Block : PO
{
public X_AD_WF_Block (Context ctx, int AD_WF_Block_ID, Trx trxName) : base (ctx, AD_WF_Block_ID, trxName)
{
/** if (AD_WF_Block_ID == 0)
{
SetAD_WF_Block_ID (0);
SetAD_Workflow_ID (0);
SetName (null);
}
 */
}
public X_AD_WF_Block (Ctx ctx, int AD_WF_Block_ID, Trx trxName) : base (ctx, AD_WF_Block_ID, trxName)
{
/** if (AD_WF_Block_ID == 0)
{
SetAD_WF_Block_ID (0);
SetAD_Workflow_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Block (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Block (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Block (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_Block()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366044L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049255L;
/** AD_Table_ID=647 */
public static int Table_ID;
 // =647;

/** TableName=AD_WF_Block */
public static String Table_Name="AD_WF_Block";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_Block[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Workflow Block.
@param AD_WF_Block_ID Workflow Transaction Execution Block */
public void SetAD_WF_Block_ID (int AD_WF_Block_ID)
{
if (AD_WF_Block_ID < 1) throw new ArgumentException ("AD_WF_Block_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Block_ID", AD_WF_Block_ID);
}
/** Get Workflow Block.
@return Workflow Transaction Execution Block */
public int GetAD_WF_Block_ID() 
{
Object ii = Get_Value("AD_WF_Block_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow.
@param AD_Workflow_ID Workflow or combination of tasks */
public void SetAD_Workflow_ID (int AD_Workflow_ID)
{
if (AD_Workflow_ID < 1) throw new ArgumentException ("AD_Workflow_ID is mandatory.");
Set_ValueNoCheck ("AD_Workflow_ID", AD_Workflow_ID);
}
/** Get Workflow.
@return Workflow or combination of tasks */
public int GetAD_Workflow_ID() 
{
Object ii = Get_Value("AD_Workflow_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
}

}
