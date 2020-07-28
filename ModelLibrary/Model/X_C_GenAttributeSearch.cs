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
/** Generated Model for C_GenAttributeSearch
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeSearch : PO
{
public X_C_GenAttributeSearch (Context ctx, int C_GenAttributeSearch_ID, Trx trxName) : base (ctx, C_GenAttributeSearch_ID, trxName)
{
/** if (C_GenAttributeSearch_ID == 0)
{
SetC_GenAttributeSearch_ID (0);
SetName (null);
}
 */
}
public X_C_GenAttributeSearch (Ctx ctx, int C_GenAttributeSearch_ID, Trx trxName) : base (ctx, C_GenAttributeSearch_ID, trxName)
{
/** if (C_GenAttributeSearch_ID == 0)
{
SetC_GenAttributeSearch_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSearch (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSearch (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSearch (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeSearch()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169093702L;
/** Last Updated Timestamp 11/21/2013 7:52:56 PM */
public static long updatedMS = 1385043776913L;
/** AD_Table_ID=1000420 */
public static int Table_ID;
 // =1000420;

/** TableName=C_GenAttributeSearch */
public static String Table_Name="C_GenAttributeSearch";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeSearch[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_GenAttributeSearch_ID.
@param C_GenAttributeSearch_ID C_GenAttributeSearch_ID */
public void SetC_GenAttributeSearch_ID (int C_GenAttributeSearch_ID)
{
if (C_GenAttributeSearch_ID < 1) throw new ArgumentException ("C_GenAttributeSearch_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeSearch_ID", C_GenAttributeSearch_ID);
}
/** Get C_GenAttributeSearch_ID.
@return C_GenAttributeSearch_ID */
public int GetC_GenAttributeSearch_ID() 
{
Object ii = Get_Value("C_GenAttributeSearch_ID");
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
}

}
