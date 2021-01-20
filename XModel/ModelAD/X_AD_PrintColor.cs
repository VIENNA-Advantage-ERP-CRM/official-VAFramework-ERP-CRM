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
/** Generated Model for VAF_Print_Rpt_Colour
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Print_Rpt_Colour : PO
{
public X_VAF_Print_Rpt_Colour (Context ctx, int VAF_Print_Rpt_Colour_ID, Trx trxName) : base (ctx, VAF_Print_Rpt_Colour_ID, trxName)
{
/** if (VAF_Print_Rpt_Colour_ID == 0)
{
SetVAF_Print_Rpt_Colour_ID (0);
SetCode (null);
SetIsDefault (false);
SetName (null);
}
 */
}
public X_VAF_Print_Rpt_Colour (Ctx ctx, int VAF_Print_Rpt_Colour_ID, Trx trxName) : base (ctx, VAF_Print_Rpt_Colour_ID, trxName)
{
/** if (VAF_Print_Rpt_Colour_ID == 0)
{
SetVAF_Print_Rpt_Colour_ID (0);
SetCode (null);
SetIsDefault (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Print_Rpt_Colour (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Print_Rpt_Colour (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Print_Rpt_Colour (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Print_Rpt_Colour()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362517L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045728L;
/** VAF_TableView_ID=490 */
public static int Table_ID;
 // =490;

/** TableName=VAF_Print_Rpt_Colour */
public static String Table_Name="VAF_Print_Rpt_Colour";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAF_Print_Rpt_Colour[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Color.
@param VAF_Print_Rpt_Colour_ID Color used for printing and display */
public void SetVAF_Print_Rpt_Colour_ID (int VAF_Print_Rpt_Colour_ID)
{
if (VAF_Print_Rpt_Colour_ID < 1) throw new ArgumentException ("VAF_Print_Rpt_Colour_ID is mandatory.");
Set_ValueNoCheck ("VAF_Print_Rpt_Colour_ID", VAF_Print_Rpt_Colour_ID);
}
/** Get Print Color.
@return Color used for printing and display */
public int GetVAF_Print_Rpt_Colour_ID() 
{
Object ii = Get_Value("VAF_Print_Rpt_Colour_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Code.
@param Code Code to execute or to validate */
public void SetCode (String Code)
{
if (Code == null) throw new ArgumentException ("Code is mandatory.");
if (Code.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Code = Code.Substring(0,2000);
}
Set_Value ("Code", Code);
}
/** Get Code.
@return Code to execute or to validate */
public String GetCode() 
{
return (String)Get_Value("Code");
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
