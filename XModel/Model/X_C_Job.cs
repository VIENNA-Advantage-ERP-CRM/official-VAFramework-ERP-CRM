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
/** Generated Model for VAB_Position
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Position : PO
{
public X_VAB_Position (Context ctx, int VAB_Position_ID, Trx trxName) : base (ctx, VAB_Position_ID, trxName)
{
/** if (VAB_Position_ID == 0)
{
SetVAB_PositionCategory_ID (0);
SetVAB_Position_ID (0);
SetIsEmployee (true);	// Y
SetName (null);
}
 */
}
public X_VAB_Position (Ctx ctx, int VAB_Position_ID, Trx trxName) : base (ctx, VAB_Position_ID, trxName)
{
/** if (VAB_Position_ID == 0)
{
SetVAB_PositionCategory_ID (0);
SetVAB_Position_ID (0);
SetIsEmployee (true);	// Y
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Position (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Position (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Position (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Position()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372657L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055868L;
/** VAF_TableView_ID=789 */
public static int Table_ID;
 // =789;

/** TableName=VAB_Position */
public static String Table_Name="VAB_Position";

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
StringBuilder sb = new StringBuilder ("X_VAB_Position[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Position Category.
@param VAB_PositionCategory_ID Job Position Category */
public void SetVAB_PositionCategory_ID (int VAB_PositionCategory_ID)
{
if (VAB_PositionCategory_ID < 1) throw new ArgumentException ("VAB_PositionCategory_ID is mandatory.");
Set_Value ("VAB_PositionCategory_ID", VAB_PositionCategory_ID);
}
/** Get Position Category.
@return Job Position Category */
public int GetVAB_PositionCategory_ID() 
{
Object ii = Get_Value("VAB_PositionCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position.
@param VAB_Position_ID Job Position */
public void SetVAB_Position_ID (int VAB_Position_ID)
{
if (VAB_Position_ID < 1) throw new ArgumentException ("VAB_Position_ID is mandatory.");
Set_ValueNoCheck ("VAB_Position_ID", VAB_Position_ID);
}
/** Get Position.
@return Job Position */
public int GetVAB_Position_ID() 
{
Object ii = Get_Value("VAB_Position_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Employee.
@param IsEmployee Indicates if  this Business Partner is an employee */
public void SetIsEmployee (Boolean IsEmployee)
{
Set_Value ("IsEmployee", IsEmployee);
}
/** Get Employee.
@return Indicates if  this Business Partner is an employee */
public Boolean IsEmployee() 
{
Object oo = Get_Value("IsEmployee");
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
}

}
