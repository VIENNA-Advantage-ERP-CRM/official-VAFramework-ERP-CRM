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
/** Generated Model for AD_WF_Node_Para
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_Node_Para : PO
{
public X_AD_WF_Node_Para (Context ctx, int AD_WF_Node_Para_ID, Trx trxName) : base (ctx, AD_WF_Node_Para_ID, trxName)
{
/** if (AD_WF_Node_Para_ID == 0)
{
SetAD_WF_Node_ID (0);
SetAD_WF_Node_Para_ID (0);
SetEntityType (null);	// U
}
 */
}
public X_AD_WF_Node_Para (Ctx ctx, int AD_WF_Node_Para_ID, Trx trxName) : base (ctx, AD_WF_Node_Para_ID, trxName)
{
/** if (AD_WF_Node_Para_ID == 0)
{
SetAD_WF_Node_ID (0);
SetAD_WF_Node_Para_ID (0);
SetEntityType (null);	// U
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Node_Para (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Node_Para (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Node_Para (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_Node_Para()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366279L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049490L;
/** AD_Table_ID=643 */
public static int Table_ID;
 // =643;

/** TableName=AD_WF_Node_Para */
public static String Table_Name="AD_WF_Node_Para";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_Node_Para[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Parameter.
@param AD_Process_Para_ID Process Parameter */
public void SetAD_Process_Para_ID (int AD_Process_Para_ID)
{
if (AD_Process_Para_ID <= 0) Set_Value ("AD_Process_Para_ID", null);
else
Set_Value ("AD_Process_Para_ID", AD_Process_Para_ID);
}
/** Get Process Parameter.
@return Process Parameter */
public int GetAD_Process_Para_ID() 
{
Object ii = Get_Value("AD_Process_Para_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Node.
@param AD_WF_Node_ID Workflow Node (activity), step or process */
public void SetAD_WF_Node_ID (int AD_WF_Node_ID)
{
if (AD_WF_Node_ID < 1) throw new ArgumentException ("AD_WF_Node_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Node_ID", AD_WF_Node_ID);
}
/** Get Node.
@return Workflow Node (activity), step or process */
public int GetAD_WF_Node_ID() 
{
Object ii = Get_Value("AD_WF_Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_WF_Node_ID().ToString());
}
/** Set Workflow Node Parameter.
@param AD_WF_Node_Para_ID Workflow Node Execution Parameter */
public void SetAD_WF_Node_Para_ID (int AD_WF_Node_Para_ID)
{
if (AD_WF_Node_Para_ID < 1) throw new ArgumentException ("AD_WF_Node_Para_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Node_Para_ID", AD_WF_Node_Para_ID);
}
/** Get Workflow Node Parameter.
@return Workflow Node Execution Parameter */
public int GetAD_WF_Node_Para_ID() 
{
Object ii = Get_Value("AD_WF_Node_Para_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Name.
@param AttributeName Name of the Attribute */
public void SetAttributeName (String AttributeName)
{
if (AttributeName != null && AttributeName.Length > 60)
{
log.Warning("Length > 60 - truncated");
AttributeName = AttributeName.Substring(0,60);
}
Set_Value ("AttributeName", AttributeName);
}
/** Get Attribute Name.
@return Name of the Attribute */
public String GetAttributeName() 
{
return (String)Get_Value("AttributeName");
}
/** Set Attribute Value.
@param AttributeValue Value of the Attribute */
public void SetAttributeValue (String AttributeValue)
{
if (AttributeValue != null && AttributeValue.Length > 60)
{
log.Warning("Length > 60 - truncated");
AttributeValue = AttributeValue.Substring(0,60);
}
Set_Value ("AttributeValue", AttributeValue);
}
/** Get Attribute Value.
@return Value of the Attribute */
public String GetAttributeValue() 
{
return (String)Get_Value("AttributeValue");
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
}

}
