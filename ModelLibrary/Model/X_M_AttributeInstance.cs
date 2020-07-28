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
/** Generated Model for M_AttributeInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_AttributeInstance : PO
{
public X_M_AttributeInstance (Context ctx, int M_AttributeInstance_ID, Trx trxName) : base (ctx, M_AttributeInstance_ID, trxName)
{
/** if (M_AttributeInstance_ID == 0)
{
SetM_AttributeSetInstance_ID (0);
SetM_Attribute_ID (0);
}
 */
}
public X_M_AttributeInstance (Ctx ctx, int M_AttributeInstance_ID, Trx trxName) : base (ctx, M_AttributeInstance_ID, trxName)
{
/** if (M_AttributeInstance_ID == 0)
{
SetM_AttributeSetInstance_ID (0);
SetM_Attribute_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_AttributeInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378268L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061479L;
/** AD_Table_ID=561 */
public static int Table_ID;
 // =561;

/** TableName=M_AttributeInstance */
public static String Table_Name="M_AttributeInstance";

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
StringBuilder sb = new StringBuilder ("X_M_AttributeInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Value.
@param M_AttributeValue_ID Product Attribute Value */
public void SetM_AttributeValue_ID (int M_AttributeValue_ID)
{
if (M_AttributeValue_ID <= 0) Set_Value ("M_AttributeValue_ID", null);
else
Set_Value ("M_AttributeValue_ID", M_AttributeValue_ID);
}
/** Get Attribute Value.
@return Product Attribute Value */
public int GetM_AttributeValue_ID() 
{
Object ii = Get_Value("M_AttributeValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_AttributeValue_ID().ToString());
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
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
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
/** Set Value.
@param ValueNumber Numeric Value */
public void SetValueNumber (Decimal? ValueNumber)
{
Set_Value ("ValueNumber", (Decimal?)ValueNumber);
}
/** Get Value.
@return Numeric Value */
public Decimal GetValueNumber() 
{
Object bd =Get_Value("ValueNumber");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
