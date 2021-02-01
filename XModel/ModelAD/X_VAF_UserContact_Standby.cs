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
/** Generated Model for VAF_UserContact_Standby
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_UserContact_Standby : PO
{
public X_VAF_UserContact_Standby (Context ctx, int VAF_UserContact_Standby_ID, Trx trxName) : base (ctx, VAF_UserContact_Standby_ID, trxName)
{
/** if (VAF_UserContact_Standby_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAF_UserContact_Standby_ID (0);
SetName (null);
SetSubstitute_ID (0);
}
 */
}
public X_VAF_UserContact_Standby (Ctx ctx, int VAF_UserContact_Standby_ID, Trx trxName) : base (ctx, VAF_UserContact_Standby_ID, trxName)
{
/** if (VAF_UserContact_Standby_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAF_UserContact_Standby_ID (0);
SetName (null);
SetSubstitute_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserContact_Standby (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserContact_Standby (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserContact_Standby (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_UserContact_Standby()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365667L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048878L;
/** VAF_TableView_ID=642 */
public static int Table_ID;
 // =642;

/** TableName=VAF_UserContact_Standby */
public static String Table_Name="VAF_UserContact_Standby";

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
StringBuilder sb = new StringBuilder ("X_VAF_UserContact_Standby[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Substitute.
@param VAF_UserContact_Standby_ID Substitute of the user */
public void SetVAF_UserContact_Standby_ID (int VAF_UserContact_Standby_ID)
{
if (VAF_UserContact_Standby_ID < 1) throw new ArgumentException ("VAF_UserContact_Standby_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_Standby_ID", VAF_UserContact_Standby_ID);
}
/** Get User Substitute.
@return Substitute of the user */
public int GetVAF_UserContact_Standby_ID() 
{
Object ii = Get_Value("VAF_UserContact_Standby_ID");
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

/** Substitute_ID VAF_Control_Ref_ID=110 */
public static int SUBSTITUTE_ID_VAF_Control_Ref_ID=110;
/** Set Substitute.
@param Substitute_ID Entity which can be used in place of this entity */
public void SetSubstitute_ID (int Substitute_ID)
{
if (Substitute_ID < 1) throw new ArgumentException ("Substitute_ID is mandatory.");
Set_Value ("Substitute_ID", Substitute_ID);
}
/** Get Substitute.
@return Entity which can be used in place of this entity */
public int GetSubstitute_ID() 
{
Object ii = Get_Value("Substitute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}
