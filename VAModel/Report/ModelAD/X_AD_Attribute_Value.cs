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
/** Generated Model for AD_Attribute_Value
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Attribute_Value : PO
{
public X_AD_Attribute_Value (Context ctx, int AD_Attribute_Value_ID, Trx trxName) : base (ctx, AD_Attribute_Value_ID, trxName)
{
/** if (AD_Attribute_Value_ID == 0)
{
SetAD_Attribute_ID (0);
SetRecord_ID (0);
}
 */
}
public X_AD_Attribute_Value (Ctx ctx, int AD_Attribute_Value_ID, Trx trxName) : base (ctx, AD_Attribute_Value_ID, trxName)
{
/** if (AD_Attribute_Value_ID == 0)
{
SetAD_Attribute_ID (0);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute_Value (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute_Value (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute_Value (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Attribute_Value()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360762L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043973L;
/** AD_Table_ID=406 */
public static int Table_ID;
 // =406;

/** TableName=AD_Attribute_Value */
public static String Table_Name="AD_Attribute_Value";

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
StringBuilder sb = new StringBuilder ("X_AD_Attribute_Value[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Attribute.
@param AD_Attribute_ID System Attribute */
public void SetAD_Attribute_ID (int AD_Attribute_ID)
{
if (AD_Attribute_ID < 1) throw new ArgumentException ("AD_Attribute_ID is mandatory.");
Set_ValueNoCheck ("AD_Attribute_ID", AD_Attribute_ID);
}
/** Get System Attribute.
@return System Attribute */
public int GetAD_Attribute_ID() 
{
Object ii = Get_Value("AD_Attribute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Attribute_ID().ToString());
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set V_Date.
@param V_Date V_Date */
public void SetV_Date (DateTime? V_Date)
{
Set_Value ("V_Date", (DateTime?)V_Date);
}
/** Get V_Date.
@return V_Date */
public DateTime? GetV_Date() 
{
return (DateTime?)Get_Value("V_Date");
}
/** Set V_Number.
@param V_Number V_Number */
public void SetV_Number (Decimal? V_Number)
{
Set_Value ("V_Number", (Decimal?)V_Number);
}
/** Get V_Number.
@return V_Number */
public Decimal GetV_Number() 
{
Object bd =Get_Value("V_Number");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set V_String.
@param V_String V_String */
public void SetV_String (String V_String)
{
if (V_String != null && V_String.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
V_String = V_String.Substring(0,2000);
}
Set_Value ("V_String", V_String);
}
/** Get V_String.
@return V_String */
public String GetV_String() 
{
return (String)Get_Value("V_String");
}
}

}
