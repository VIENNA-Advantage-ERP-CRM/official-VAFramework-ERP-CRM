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
/** Generated Model for M_AttributeValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_AttributeValue : PO
{
public X_M_AttributeValue (Context ctx, int M_AttributeValue_ID, Trx trxName) : base (ctx, M_AttributeValue_ID, trxName)
{
/** if (M_AttributeValue_ID == 0)
{
SetM_AttributeValue_ID (0);
SetM_Attribute_ID (0);
SetName (null);
SetValue (null);
}
 */
}
public X_M_AttributeValue (Ctx ctx, int M_AttributeValue_ID, Trx trxName) : base (ctx, M_AttributeValue_ID, trxName)
{
/** if (M_AttributeValue_ID == 0)
{
SetM_AttributeValue_ID (0);
SetM_Attribute_ID (0);
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_AttributeValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378425L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061636L;
/** AD_Table_ID=558 */
public static int Table_ID;
 // =558;

/** TableName=M_AttributeValue */
public static String Table_Name="M_AttributeValue";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_M_AttributeValue[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Attribute Value.
@param M_AttributeValue_ID Product Attribute Value */
public void SetM_AttributeValue_ID (int M_AttributeValue_ID)
{
if (M_AttributeValue_ID < 1) throw new ArgumentException ("M_AttributeValue_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeValue_ID", M_AttributeValue_ID);
}
/** Get Attribute Value.
@return Product Attribute Value */
public int GetM_AttributeValue_ID() 
{
Object ii = Get_Value("M_AttributeValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute.
@param M_Attribute_ID Product Attribute */
public void SetM_Attribute_ID (int M_Attribute_ID)
{
if (M_Attribute_ID < 1) throw new ArgumentException ("M_Attribute_ID is mandatory.");
Set_ValueNoCheck ("M_Attribute_ID", M_Attribute_ID);
}
/** Get Attribute.
@return Product Attribute */
public int GetM_Attribute_ID() 
{
Object ii = Get_Value("M_Attribute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
