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
/** Generated Model for C_CurrencySource
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CurrencySource : PO
{
public X_C_CurrencySource (Context ctx, int C_CurrencySource_ID, Trx trxName) : base (ctx, C_CurrencySource_ID, trxName)
{
/** if (C_CurrencySource_ID == 0)
{
SetC_CurrencySource_ID (0);
SetName (null);
SetURL (null);
}
 */
}
public X_C_CurrencySource (Ctx ctx, int C_CurrencySource_ID, Trx trxName) : base (ctx, C_CurrencySource_ID, trxName)
{
/** if (C_CurrencySource_ID == 0)
{
SetC_CurrencySource_ID (0);
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
public X_C_CurrencySource (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CurrencySource (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CurrencySource (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CurrencySource()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27628278561903L;
/** Last Updated Timestamp 8/28/2012 4:57:25 PM */
public static long updatedMS = 1346153245114L;
/** AD_Table_ID=1000045 */
public static int Table_ID;
 // =1000045;

/** TableName=C_CurrencySource */
public static String Table_Name="C_CurrencySource";

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
StringBuilder sb = new StringBuilder ("X_C_CurrencySource[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_CurrencySource_ID.
@param C_CurrencySource_ID C_CurrencySource_ID */
public void SetC_CurrencySource_ID (int C_CurrencySource_ID)
{
if (C_CurrencySource_ID < 1) throw new ArgumentException ("C_CurrencySource_ID is mandatory.");
Set_ValueNoCheck ("C_CurrencySource_ID", C_CurrencySource_ID);
}
/** Get C_CurrencySource_ID.
@return C_CurrencySource_ID */
public int GetC_CurrencySource_ID() 
{
Object ii = Get_Value("C_CurrencySource_ID");
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
