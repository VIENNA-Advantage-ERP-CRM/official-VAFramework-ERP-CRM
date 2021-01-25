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
/** Generated Model for VAB_CurrencySource
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_CurrencySource : PO
{
public X_VAB_CurrencySource (Context ctx, int VAB_CurrencySource_ID, Trx trxName) : base (ctx, VAB_CurrencySource_ID, trxName)
{
/** if (VAB_CurrencySource_ID == 0)
{
SetVAB_CurrencySource_ID (0);
SetName (null);
SetURL (null);
}
 */
}
public X_VAB_CurrencySource (Ctx ctx, int VAB_CurrencySource_ID, Trx trxName) : base (ctx, VAB_CurrencySource_ID, trxName)
{
/** if (VAB_CurrencySource_ID == 0)
{
SetVAB_CurrencySource_ID (0);
SetName (null);
SetURL (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CurrencySource (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CurrencySource (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_CurrencySource (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_CurrencySource()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27628278561903L;
/** Last Updated Timestamp 8/28/2012 4:57:25 PM */
public static long updatedMS = 1346153245114L;
/** VAF_TableView_ID=1000045 */
public static int Table_ID;
 // =1000045;

/** TableName=VAB_CurrencySource */
public static String Table_Name="VAB_CurrencySource";

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
StringBuilder sb = new StringBuilder ("X_VAB_CurrencySource[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_CurrencySource_ID.
@param VAB_CurrencySource_ID VAB_CurrencySource_ID */
public void SetVAB_CurrencySource_ID (int VAB_CurrencySource_ID)
{
if (VAB_CurrencySource_ID < 1) throw new ArgumentException ("VAB_CurrencySource_ID is mandatory.");
Set_ValueNoCheck ("VAB_CurrencySource_ID", VAB_CurrencySource_ID);
}
/** Get VAB_CurrencySource_ID.
@return VAB_CurrencySource_ID */
public int GetVAB_CurrencySource_ID() 
{
Object ii = Get_Value("VAB_CurrencySource_ID");
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 100)
{
log.Warning("Length > 100 - truncated");
Name = Name.Substring(0,100);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennaadvantage.com */
public void SetURL (String URL)
{
if (URL == null) throw new ArgumentException ("URL is mandatory.");
if (URL.Length > 500)
{
log.Warning("Length > 500 - truncated");
URL = URL.Substring(0,500);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
}

}
