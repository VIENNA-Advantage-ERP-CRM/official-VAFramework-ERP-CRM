namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;

using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_SalesOffice
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_SalesOffice : PO
{
public X_C_SalesOffice (Context ctx, int C_SalesOffice_ID, Trx trxName) : base (ctx, C_SalesOffice_ID, trxName)
{
/** if (C_SalesOffice_ID == 0)
{
SetC_SalesOffice_ID (0);
SetValue (null);
}
 */
}
public X_C_SalesOffice (Ctx ctx, int C_SalesOffice_ID, Trx trxName) : base (ctx, C_SalesOffice_ID, trxName)
{
/** if (C_SalesOffice_ID == 0)
{
SetC_SalesOffice_ID (0);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_SalesOffice(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_SalesOffice(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_SalesOffice(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_SalesOffice()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27716650921724L;
/** Last Updated Timestamp 6/17/2015 12:50:05 PM */
public static long updatedMS = 1434525604935L;
/** AD_Table_ID=1000653 */
public static int Table_ID;
 // =1000653;

/** TableName=C_SalesOffice */
public static String Table_Name="C_SalesOffice";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_SalesOffice[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_SalesOffice_ID.
@param C_SalesOffice_ID C_SalesOffice_ID */
public void SetC_SalesOffice_ID (int C_SalesOffice_ID)
{
if (C_SalesOffice_ID < 1) throw new ArgumentException ("C_SalesOffice_ID is mandatory.");
Set_ValueNoCheck ("C_SalesOffice_ID", C_SalesOffice_ID);
}
/** Get C_SalesOffice_ID.
@return C_SalesOffice_ID */
public int GetC_SalesOffice_ID() 
{
Object ii = Get_Value("C_SalesOffice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 500)
{
log.Warning("Length > 500 - truncated");
Description = Description.Substring(0,500);
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 200)
{
log.Warning("Length > 200 - truncated");
Name = Name.Substring(0,200);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 50)
{
log.Warning("Length > 50 - truncated");
Value = Value.Substring(0,50);
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
