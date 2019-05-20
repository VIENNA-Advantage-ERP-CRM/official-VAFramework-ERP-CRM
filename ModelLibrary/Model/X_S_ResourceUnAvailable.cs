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
/** Generated Model for S_ResourceUnAvailable
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_S_ResourceUnAvailable : PO
{
public X_S_ResourceUnAvailable (Context ctx, int S_ResourceUnAvailable_ID, Trx trxName) : base (ctx, S_ResourceUnAvailable_ID, trxName)
{
/** if (S_ResourceUnAvailable_ID == 0)
{
SetDateFrom (DateTime.Now);
SetS_ResourceUnAvailable_ID (0);
SetS_Resource_ID (0);
}
 */
}
public X_S_ResourceUnAvailable (Ctx ctx, int S_ResourceUnAvailable_ID, Trx trxName) : base (ctx, S_ResourceUnAvailable_ID, trxName)
{
/** if (S_ResourceUnAvailable_ID == 0)
{
SetDateFrom (DateTime.Now);
SetS_ResourceUnAvailable_ID (0);
SetS_Resource_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceUnAvailable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceUnAvailable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceUnAvailable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_S_ResourceUnAvailable()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383801L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067012L;
/** AD_Table_ID=482 */
public static int Table_ID;
 // =482;

/** TableName=S_ResourceUnAvailable */
public static String Table_Name="S_ResourceUnAvailable";

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
StringBuilder sb = new StringBuilder ("X_S_ResourceUnAvailable[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Date From.
@param DateFrom Starting date for a range */
public void SetDateFrom (DateTime? DateFrom)
{
if (DateFrom == null) throw new ArgumentException ("DateFrom is mandatory.");
Set_Value ("DateFrom", (DateTime?)DateFrom);
}
/** Get Date From.
@return Starting date for a range */
public DateTime? GetDateFrom() 
{
return (DateTime?)Get_Value("DateFrom");
}
/** Set Date To.
@param DateTo End date of a date range */
public void SetDateTo (DateTime? DateTo)
{
Set_Value ("DateTo", (DateTime?)DateTo);
}
/** Get Date To.
@return End date of a date range */
public DateTime? GetDateTo() 
{
return (DateTime?)Get_Value("DateTo");
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
/** Set Resource Unavailability.
@param S_ResourceUnAvailable_ID Resource Unavailability */
public void SetS_ResourceUnAvailable_ID (int S_ResourceUnAvailable_ID)
{
if (S_ResourceUnAvailable_ID < 1) throw new ArgumentException ("S_ResourceUnAvailable_ID is mandatory.");
Set_ValueNoCheck ("S_ResourceUnAvailable_ID", S_ResourceUnAvailable_ID);
}
/** Get Resource Unavailability.
@return Resource Unavailability */
public int GetS_ResourceUnAvailable_ID() 
{
Object ii = Get_Value("S_ResourceUnAvailable_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Resource.
@param S_Resource_ID Resource */
public void SetS_Resource_ID (int S_Resource_ID)
{
if (S_Resource_ID < 1) throw new ArgumentException ("S_Resource_ID is mandatory.");
Set_ValueNoCheck ("S_Resource_ID", S_Resource_ID);
}
/** Get Resource.
@return Resource */
public int GetS_Resource_ID() 
{
Object ii = Get_Value("S_Resource_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetS_Resource_ID().ToString());
}
}

}
