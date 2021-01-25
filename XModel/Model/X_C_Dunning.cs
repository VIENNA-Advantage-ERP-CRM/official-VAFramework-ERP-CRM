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
/** Generated Model for VAB_Dunning
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Dunning : PO
{
public X_VAB_Dunning (Context ctx, int VAB_Dunning_ID, Trx trxName) : base (ctx, VAB_Dunning_ID, trxName)
{
/** if (VAB_Dunning_ID == 0)
{
SetVAB_Dunning_ID (0);
SetCreateLevelsSequentially (false);
SetIsDefault (false);
SetName (null);
SetSendDunningLetter (false);
}
 */
}
public X_VAB_Dunning (Ctx ctx, int VAB_Dunning_ID, Trx trxName) : base (ctx, VAB_Dunning_ID, trxName)
{
/** if (VAB_Dunning_ID == 0)
{
SetVAB_Dunning_ID (0);
SetCreateLevelsSequentially (false);
SetIsDefault (false);
SetName (null);
SetSendDunningLetter (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Dunning (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Dunning (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Dunning (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Dunning()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371936L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055147L;
/** VAF_TableView_ID=301 */
public static int Table_ID;
 // =301;

/** TableName=VAB_Dunning */
public static String Table_Name="VAB_Dunning";

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
StringBuilder sb = new StringBuilder ("X_VAB_Dunning[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Dunning.
@param VAB_Dunning_ID Dunning Rules for overdue invoices */
public void SetVAB_Dunning_ID (int VAB_Dunning_ID)
{
if (VAB_Dunning_ID < 1) throw new ArgumentException ("VAB_Dunning_ID is mandatory.");
Set_ValueNoCheck ("VAB_Dunning_ID", VAB_Dunning_ID);
}
/** Get Dunning.
@return Dunning Rules for overdue invoices */
public int GetVAB_Dunning_ID() 
{
Object ii = Get_Value("VAB_Dunning_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Create levels sequentially.
@param CreateLevelsSequentially Create Dunning Letter by level sequentially */
public void SetCreateLevelsSequentially (Boolean CreateLevelsSequentially)
{
Set_Value ("CreateLevelsSequentially", CreateLevelsSequentially);
}
/** Get Create levels sequentially.
@return Create Dunning Letter by level sequentially */
public Boolean IsCreateLevelsSequentially() 
{
Object oo = Get_Value("CreateLevelsSequentially");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Send dunning letters.
@param SendDunningLetter Indicates if dunning letters will be sent */
public void SetSendDunningLetter (Boolean SendDunningLetter)
{
Set_Value ("SendDunningLetter", SendDunningLetter);
}
/** Get Send dunning letters.
@return Indicates if dunning letters will be sent */
public Boolean IsSendDunningLetter() 
{
Object oo = Get_Value("SendDunningLetter");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
