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
/** Generated Model for VAB_Greeting
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Greeting : PO
{
public X_VAB_Greeting (Context ctx, int VAB_Greeting_ID, Trx trxName) : base (ctx, VAB_Greeting_ID, trxName)
{
/** if (VAB_Greeting_ID == 0)
{
SetVAB_Greeting_ID (0);
SetIsDefault (false);
SetIsFirstNameOnly (false);
SetName (null);
}
 */
}
public X_VAB_Greeting (Ctx ctx, int VAB_Greeting_ID, Trx trxName) : base (ctx, VAB_Greeting_ID, trxName)
{
/** if (VAB_Greeting_ID == 0)
{
SetVAB_Greeting_ID (0);
SetIsDefault (false);
SetIsFirstNameOnly (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Greeting (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Greeting (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Greeting (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Greeting()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372172L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055383L;
/** VAF_TableView_ID=346 */
public static int Table_ID;
 // =346;

/** TableName=VAB_Greeting */
public static String Table_Name="VAB_Greeting";

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
StringBuilder sb = new StringBuilder ("X_VAB_Greeting[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Greeting.
@param VAB_Greeting_ID Greeting to print on correspondence */
public void SetVAB_Greeting_ID (int VAB_Greeting_ID)
{
if (VAB_Greeting_ID < 1) throw new ArgumentException ("VAB_Greeting_ID is mandatory.");
Set_ValueNoCheck ("VAB_Greeting_ID", VAB_Greeting_ID);
}
/** Get Greeting.
@return Greeting to print on correspondence */
public int GetVAB_Greeting_ID() 
{
Object ii = Get_Value("VAB_Greeting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Greeting.
@param Greeting For letters, e.g. "Dear 
{
0}
" or "Dear Mr. 
{
0}
" - At runtime, "
{
0}
" is replaced by the name */
public void SetGreeting (String Greeting)
{
if (Greeting != null && Greeting.Length > 60)
{
log.Warning("Length > 60 - truncated");
Greeting = Greeting.Substring(0,60);
}
Set_Value ("Greeting", Greeting);
}
/** Get Greeting.
@return For letters, e.g. "Dear 
{
0}
" or "Dear Mr. 
{
0}
" - At runtime, "
{
0}
" is replaced by the name */
public String GetGreeting() 
{
return (String)Get_Value("Greeting");
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
/** Set First name only.
@param IsFirstNameOnly Print only the first name in greetings */
public void SetIsFirstNameOnly (Boolean IsFirstNameOnly)
{
Set_Value ("IsFirstNameOnly", IsFirstNameOnly);
}
/** Get First name only.
@return Print only the first name in greetings */
public Boolean IsFirstNameOnly() 
{
Object oo = Get_Value("IsFirstNameOnly");
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
