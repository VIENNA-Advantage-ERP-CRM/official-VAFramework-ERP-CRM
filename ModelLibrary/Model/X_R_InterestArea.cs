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
/** Generated Model for R_InterestArea
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_InterestArea : PO
{
public X_R_InterestArea (Context ctx, int R_InterestArea_ID, Trx trxName) : base (ctx, R_InterestArea_ID, trxName)
{
/** if (R_InterestArea_ID == 0)
{
SetIsSelfService (true);	// Y
SetName (null);
SetR_InterestArea_ID (0);
SetValue (null);
}
 */
}
public X_R_InterestArea (Ctx ctx, int R_InterestArea_ID, Trx trxName) : base (ctx, R_InterestArea_ID, trxName)
{
/** if (R_InterestArea_ID == 0)
{
SetIsSelfService (true);	// Y
SetName (null);
SetR_InterestArea_ID (0);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_InterestArea (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_InterestArea (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_InterestArea (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_InterestArea()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382798L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066009L;
/** AD_Table_ID=530 */
public static int Table_ID;
 // =530;

/** TableName=R_InterestArea */
public static String Table_Name="R_InterestArea";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_R_InterestArea[").Append(Get_ID()).Append("]");
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
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID < 1) throw new ArgumentException ("R_InterestArea_ID is mandatory.");
Set_ValueNoCheck ("R_InterestArea_ID", R_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("R_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Source.
@param R_Source_ID Source for the Lead or Request */
public void SetR_Source_ID (int R_Source_ID)
{
if (R_Source_ID <= 0) Set_Value ("R_Source_ID", null);
else
Set_Value ("R_Source_ID", R_Source_ID);
}
/** Get Source.
@return Source for the Lead or Request */
public int GetR_Source_ID() 
{
Object ii = Get_Value("R_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
