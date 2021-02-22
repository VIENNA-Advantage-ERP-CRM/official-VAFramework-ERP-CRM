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
/** Generated Model for VAM_ReturnRule
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ReturnRule : PO
{
public X_VAM_ReturnRule (Context ctx, int VAM_ReturnRule_ID, Trx trxName) : base (ctx, VAM_ReturnRule_ID, trxName)
{
/** if (VAM_ReturnRule_ID == 0)
{
SetIsDefault (false);
SetVAM_ReturnRule_ID (0);
SetName (null);
}
 */
}
public X_VAM_ReturnRule (Ctx ctx, int VAM_ReturnRule_ID, Trx trxName) : base (ctx, VAM_ReturnRule_ID, trxName)
{
/** if (VAM_ReturnRule_ID == 0)
{
SetIsDefault (false);
SetVAM_ReturnRule_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ReturnRule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ReturnRule()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381246L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064457L;
/** VAF_TableView_ID=985 */
public static int Table_ID;
 // =985;

/** TableName=VAM_ReturnRule */
public static String Table_Name="VAM_ReturnRule";

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
StringBuilder sb = new StringBuilder ("X_VAM_ReturnRule[").Append(Get_ID()).Append("]");
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
@param VAM_ReturnRule_ID The Return Policy dictates the timeframe within which goods can be returned. */
public void SetVAM_ReturnRule_ID (int VAM_ReturnRule_ID)
{
if (VAM_ReturnRule_ID < 1) throw new ArgumentException ("VAM_ReturnRule_ID is mandatory.");
Set_ValueNoCheck ("VAM_ReturnRule_ID", VAM_ReturnRule_ID);
}
/** Get Return Policy.
@return The Return Policy dictates the timeframe within which goods can be returned. */
public int GetVAM_ReturnRule_ID() 
{
Object ii = Get_Value("VAM_ReturnRule_ID");
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
