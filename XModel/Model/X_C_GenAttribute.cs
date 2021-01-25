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
/** Generated Model for VAB_GenFeature
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_GenFeature : PO
{
public X_VAB_GenFeature (Context ctx, int VAB_GenFeature_ID, Trx trxName) : base (ctx, VAB_GenFeature_ID, trxName)
{
/** if (VAB_GenFeature_ID == 0)
{
SetAttributeValueType (null);	// 5
SetVAB_GenFeature_ID (0);
SetIsInstanceAttribute (false);
SetIsMandatory (false);
SetName (null);
}
 */
}
public X_VAB_GenFeature (Ctx ctx, int VAB_GenFeature_ID, Trx trxName) : base (ctx, VAB_GenFeature_ID, trxName)
{
/** if (VAB_GenFeature_ID == 0)
{
SetAttributeValueType (null);	// 5
SetVAB_GenFeature_ID (0);
SetIsInstanceAttribute (false);
SetIsMandatory (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeature (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeature (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeature (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_GenFeature()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169084760L;
/** Last Updated Timestamp 11/21/2013 7:52:48 PM */
public static long updatedMS = 1385043767971L;
/** VAF_TableView_ID=1000418 */
public static int Table_ID;
 // =1000418;

/** TableName=VAB_GenFeature */
public static String Table_Name="VAB_GenFeature";

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
StringBuilder sb = new StringBuilder ("X_VAB_GenFeature[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AttributeValueType VAF_Control_Ref_ID=326 */
public static int ATTRIBUTEVALUETYPE_VAF_Control_Ref_ID=326;
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
/** Set VAB_GenFeatureSearch_ID.
@param VAB_GenFeatureSearch_ID VAB_GenFeatureSearch_ID */
public void SetVAB_GenFeatureSearch_ID (int VAB_GenFeatureSearch_ID)
{
if (VAB_GenFeatureSearch_ID <= 0) Set_Value ("VAB_GenFeatureSearch_ID", null);
else
Set_Value ("VAB_GenFeatureSearch_ID", VAB_GenFeatureSearch_ID);
}
/** Get VAB_GenFeatureSearch_ID.
@return VAB_GenFeatureSearch_ID */
public int GetVAB_GenFeatureSearch_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSearch_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeature_ID.
@param VAB_GenFeature_ID VAB_GenFeature_ID */
public void SetVAB_GenFeature_ID (int VAB_GenFeature_ID)
{
if (VAB_GenFeature_ID < 1) throw new ArgumentException ("VAB_GenFeature_ID is mandatory.");
Set_ValueNoCheck ("VAB_GenFeature_ID", VAB_GenFeature_ID);
}
/** Get VAB_GenFeature_ID.
@return VAB_GenFeature_ID */
public int GetVAB_GenFeature_ID() 
{
Object ii = Get_Value("VAB_GenFeature_ID");
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
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
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
}

}
