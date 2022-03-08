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
/** Generated Model for AD_WF_ProcessData
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_ProcessData : PO
{
public X_AD_WF_ProcessData (Context ctx, int AD_WF_ProcessData_ID, Trx trxName) : base (ctx, AD_WF_ProcessData_ID, trxName)
{
/** if (AD_WF_ProcessData_ID == 0)
{
SetAD_WF_ProcessData_ID (0);
SetAD_WF_Process_ID (0);
SetAttributeName (null);
}
 */
}
public X_AD_WF_ProcessData (Ctx ctx, int AD_WF_ProcessData_ID, Trx trxName) : base (ctx, AD_WF_ProcessData_ID, trxName)
{
/** if (AD_WF_ProcessData_ID == 0)
{
SetAD_WF_ProcessData_ID (0);
SetAD_WF_Process_ID (0);
SetAttributeName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_ProcessData (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_ProcessData (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_ProcessData (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_ProcessData()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366341L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049552L;
/** AD_Table_ID=648 */
public static int Table_ID;
 // =648;

/** TableName=AD_WF_ProcessData */
public static String Table_Name="AD_WF_ProcessData";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_ProcessData[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Workflow Process Data.
@param AD_WF_ProcessData_ID Workflow Process Context */
public void SetAD_WF_ProcessData_ID (int AD_WF_ProcessData_ID)
{
if (AD_WF_ProcessData_ID < 1) throw new ArgumentException ("AD_WF_ProcessData_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_ProcessData_ID", AD_WF_ProcessData_ID);
}
/** Get Workflow Process Data.
@return Workflow Process Context */
public int GetAD_WF_ProcessData_ID() 
{
Object ii = Get_Value("AD_WF_ProcessData_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Process.
@param AD_WF_Process_ID Actual Workflow Process Instance */
public void SetAD_WF_Process_ID (int AD_WF_Process_ID)
{
if (AD_WF_Process_ID < 1) throw new ArgumentException ("AD_WF_Process_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Process_ID", AD_WF_Process_ID);
}
/** Get Workflow Process.
@return Actual Workflow Process Instance */
public int GetAD_WF_Process_ID() 
{
Object ii = Get_Value("AD_WF_Process_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_WF_Process_ID().ToString());
}
/** Set Attribute Name.
@param AttributeName Name of the Attribute */
public void SetAttributeName (String AttributeName)
{
if (AttributeName == null) throw new ArgumentException ("AttributeName is mandatory.");
if (AttributeName.Length > 60)
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
}

}
