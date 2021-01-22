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
/** Generated Model for VAF_WFlow_DataHandler
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlow_DataHandler : PO
{
public X_VAF_WFlow_DataHandler (Context ctx, int VAF_WFlow_DataHandler_ID, Trx trxName) : base (ctx, VAF_WFlow_DataHandler_ID, trxName)
{
/** if (VAF_WFlow_DataHandler_ID == 0)
{
SetVAF_WFlow_DataHandler_ID (0);
SetVAF_WFlow_Handler_ID (0);
SetAttributeName (null);
}
 */
}
public X_VAF_WFlow_DataHandler (Ctx ctx, int VAF_WFlow_DataHandler_ID, Trx trxName) : base (ctx, VAF_WFlow_DataHandler_ID, trxName)
{
/** if (VAF_WFlow_DataHandler_ID == 0)
{
SetVAF_WFlow_DataHandler_ID (0);
SetVAF_WFlow_Handler_ID (0);
SetAttributeName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_DataHandler (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_DataHandler (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_DataHandler (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlow_DataHandler()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366341L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049552L;
/** VAF_TableView_ID=648 */
public static int Table_ID;
 // =648;

/** TableName=VAF_WFlow_DataHandler */
public static String Table_Name="VAF_WFlow_DataHandler";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlow_DataHandler[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Workflow Process Data.
@param VAF_WFlow_DataHandler_ID Workflow Process Context */
public void SetVAF_WFlow_DataHandler_ID (int VAF_WFlow_DataHandler_ID)
{
if (VAF_WFlow_DataHandler_ID < 1) throw new ArgumentException ("VAF_WFlow_DataHandler_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_DataHandler_ID", VAF_WFlow_DataHandler_ID);
}
/** Get Workflow Process Data.
@return Workflow Process Context */
public int GetVAF_WFlow_DataHandler_ID() 
{
Object ii = Get_Value("VAF_WFlow_DataHandler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Process.
@param VAF_WFlow_Handler_ID Actual Workflow Process Instance */
public void SetVAF_WFlow_Handler_ID (int VAF_WFlow_Handler_ID)
{
if (VAF_WFlow_Handler_ID < 1) throw new ArgumentException ("VAF_WFlow_Handler_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_Handler_ID", VAF_WFlow_Handler_ID);
}
/** Get Workflow Process.
@return Actual Workflow Process Instance */
public int GetVAF_WFlow_Handler_ID() 
{
Object ii = Get_Value("VAF_WFlow_Handler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_WFlow_Handler_ID().ToString());
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
