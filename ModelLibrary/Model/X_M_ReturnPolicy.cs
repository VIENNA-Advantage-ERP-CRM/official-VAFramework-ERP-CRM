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
/** Generated Model for M_ReturnPolicy
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ReturnPolicy : PO
{
public X_M_ReturnPolicy (Context ctx, int M_ReturnPolicy_ID, Trx trxName) : base (ctx, M_ReturnPolicy_ID, trxName)
{
/** if (M_ReturnPolicy_ID == 0)
{
SetIsDefault (false);
SetM_ReturnPolicy_ID (0);
SetName (null);
}
 */
}
public X_M_ReturnPolicy (Ctx ctx, int M_ReturnPolicy_ID, Trx trxName) : base (ctx, M_ReturnPolicy_ID, trxName)
{
/** if (M_ReturnPolicy_ID == 0)
{
SetIsDefault (false);
SetM_ReturnPolicy_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicy (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicy (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ReturnPolicy (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ReturnPolicy()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381246L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064457L;
/** AD_Table_ID=985 */
public static int Table_ID;
 // =985;

/** TableName=M_ReturnPolicy */
public static String Table_Name="M_ReturnPolicy";

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
StringBuilder sb = new StringBuilder ("X_M_ReturnPolicy[").Append(Get_ID()).Append("]");
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
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Return Policy.
@param M_ReturnPolicy_ID The Return Policy dictates the timeframe within which goods can be returned. */
public void SetM_ReturnPolicy_ID (int M_ReturnPolicy_ID)
{
if (M_ReturnPolicy_ID < 1) throw new ArgumentException ("M_ReturnPolicy_ID is mandatory.");
Set_ValueNoCheck ("M_ReturnPolicy_ID", M_ReturnPolicy_ID);
}
/** Get Return Policy.
@return The Return Policy dictates the timeframe within which goods can be returned. */
public int GetM_ReturnPolicy_ID() 
{
Object ii = Get_Value("M_ReturnPolicy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 120)
{
log.Warning("Length > 120 - truncated");
Name = Name.Substring(0,120);
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
/** Set TimeFrame (in days).
@param TimeFrame The timeframe dictates the number of days after shipment that the goods can be returned. */
public void SetTimeFrame (int TimeFrame)
{
Set_Value ("TimeFrame", TimeFrame);
}
/** Get TimeFrame (in days).
@return The timeframe dictates the number of days after shipment that the goods can be returned. */
public int GetTimeFrame() 
{
Object ii = Get_Value("TimeFrame");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
