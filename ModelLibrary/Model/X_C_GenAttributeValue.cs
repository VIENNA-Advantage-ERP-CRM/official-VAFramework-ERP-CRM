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
/** Generated Model for C_GenAttributeValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeValue : PO
{
public X_C_GenAttributeValue (Context ctx, int C_GenAttributeValue_ID, Trx trxName) : base (ctx, C_GenAttributeValue_ID, trxName)
{
/** if (C_GenAttributeValue_ID == 0)
{
SetC_GenAttributeValue_ID (0);
SetC_GenAttribute_ID (0);
SetName (null);
SetValue (null);
}
 */
}
public X_C_GenAttributeValue (Ctx ctx, int C_GenAttributeValue_ID, Trx trxName) : base (ctx, C_GenAttributeValue_ID, trxName)
{
/** if (C_GenAttributeValue_ID == 0)
{
SetC_GenAttributeValue_ID (0);
SetC_GenAttribute_ID (0);
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
public X_C_GenAttributeValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169123797L;
/** Last Updated Timestamp 11/21/2013 7:53:27 PM */
public static long updatedMS = 1385043807008L;
/** AD_Table_ID=1000425 */
public static int Table_ID;
 // =1000425;

/** TableName=C_GenAttributeValue */
public static String Table_Name="C_GenAttributeValue";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeValue[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_GenAttributeValue_ID.
@param C_GenAttributeValue_ID C_GenAttributeValue_ID */
public void SetC_GenAttributeValue_ID (int C_GenAttributeValue_ID)
{
if (C_GenAttributeValue_ID < 1) throw new ArgumentException ("C_GenAttributeValue_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeValue_ID", C_GenAttributeValue_ID);
}
/** Get C_GenAttributeValue_ID.
@return C_GenAttributeValue_ID */
public int GetC_GenAttributeValue_ID() 
{
Object ii = Get_Value("C_GenAttributeValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_GenAttribute_ID.
@param C_GenAttribute_ID C_GenAttribute_ID */
public void SetC_GenAttribute_ID (int C_GenAttribute_ID)
{
if (C_GenAttribute_ID < 1) throw new ArgumentException ("C_GenAttribute_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttribute_ID", C_GenAttribute_ID);
}
/** Get C_GenAttribute_ID.
@return C_GenAttribute_ID */
public int GetC_GenAttribute_ID() 
{
Object ii = Get_Value("C_GenAttribute_ID");
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
