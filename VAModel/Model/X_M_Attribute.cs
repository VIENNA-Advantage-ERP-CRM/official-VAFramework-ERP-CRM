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
/** Generated Model for M_Attribute
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Attribute : PO
{
public X_M_Attribute (Context ctx, int M_Attribute_ID, Trx trxName) : base (ctx, M_Attribute_ID, trxName)
{
/** if (M_Attribute_ID == 0)
{
SetAttributeValueType (null);	// S
SetIsInstanceAttribute (false);
SetIsMandatory (false);
SetM_Attribute_ID (0);
SetName (null);
}
 */
}
public X_M_Attribute (Ctx ctx, int M_Attribute_ID, Trx trxName) : base (ctx, M_Attribute_ID, trxName)
{
/** if (M_Attribute_ID == 0)
{
SetAttributeValueType (null);	// S
SetIsInstanceAttribute (false);
SetIsMandatory (false);
SetM_Attribute_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Attribute (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Attribute (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Attribute (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Attribute()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378237L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061448L;
/** AD_Table_ID=562 */
public static int Table_ID;
 // =562;

/** TableName=M_Attribute */
public static String Table_Name="M_Attribute";

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
StringBuilder sb = new StringBuilder ("X_M_Attribute[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AttributeValueType AD_Reference_ID=326 */
public static int ATTRIBUTEVALUETYPE_AD_Reference_ID=326;
/** List = L */
public static String ATTRIBUTEVALUETYPE_List = "L";
/** Number = N */
public static String ATTRIBUTEVALUETYPE_Number = "N";
/** String (max 40) = S */
public static String ATTRIBUTEVALUETYPE_StringMax40 = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAttributeValueTypeValid (String test)
{
return test.Equals("L") || test.Equals("N") || test.Equals("S");
}
/** Set Attribute Value Type.
@param AttributeValueType Type of Attribute Value */
public void SetAttributeValueType (String AttributeValueType)
{
if (AttributeValueType == null) throw new ArgumentException ("AttributeValueType is mandatory");
if (!IsAttributeValueTypeValid(AttributeValueType))
throw new ArgumentException ("AttributeValueType Invalid value - " + AttributeValueType + " - Reference_ID=326 - L - N - S");
if (AttributeValueType.Length > 1)
{
log.Warning("Length > 1 - truncated");
AttributeValueType = AttributeValueType.Substring(0,1);
}
Set_Value ("AttributeValueType", AttributeValueType);
}
/** Get Attribute Value Type.
@return Type of Attribute Value */
public String GetAttributeValueType() 
{
return (String)Get_Value("AttributeValueType");
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
/** Set Instance Attribute.
@param IsInstanceAttribute The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public void SetIsInstanceAttribute (Boolean IsInstanceAttribute)
{
Set_Value ("IsInstanceAttribute", IsInstanceAttribute);
}
/** Get Instance Attribute.
@return The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public Boolean IsInstanceAttribute() 
{
Object oo = Get_Value("IsInstanceAttribute");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory.
@param IsMandatory Data is required in this column */
public void SetIsMandatory (Boolean IsMandatory)
{
Set_Value ("IsMandatory", IsMandatory);
}
/** Get Mandatory.
@return Data is required in this column */
public Boolean IsMandatory() 
{
Object oo = Get_Value("IsMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Attribute Search.
@param M_AttributeSearch_ID Common Search Attribute */
public void SetM_AttributeSearch_ID (int M_AttributeSearch_ID)
{
if (M_AttributeSearch_ID <= 0) Set_Value ("M_AttributeSearch_ID", null);
else
Set_Value ("M_AttributeSearch_ID", M_AttributeSearch_ID);
}
/** Get Attribute Search.
@return Common Search Attribute */
public int GetM_AttributeSearch_ID() 
{
Object ii = Get_Value("M_AttributeSearch_ID");
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
}

}
