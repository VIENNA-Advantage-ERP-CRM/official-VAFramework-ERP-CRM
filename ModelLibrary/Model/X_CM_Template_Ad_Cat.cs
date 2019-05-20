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
/** Generated Model for CM_Template_Ad_Cat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Template_Ad_Cat : PO
{
public X_CM_Template_Ad_Cat (Context ctx, int CM_Template_Ad_Cat_ID, Trx trxName) : base (ctx, CM_Template_Ad_Cat_ID, trxName)
{
/** if (CM_Template_Ad_Cat_ID == 0)
{
SetCM_Ad_Cat_ID (0);
SetCM_Template_ID (0);
SetName (null);
}
 */
}
public X_CM_Template_Ad_Cat (Ctx ctx, int CM_Template_Ad_Cat_ID, Trx trxName) : base (ctx, CM_Template_Ad_Cat_ID, trxName)
{
/** if (CM_Template_Ad_Cat_ID == 0)
{
SetCM_Ad_Cat_ID (0);
SetCM_Template_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template_Ad_Cat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template_Ad_Cat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template_Ad_Cat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Template_Ad_Cat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369225L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052436L;
/** AD_Table_ID=872 */
public static int Table_ID;
 // =872;

/** TableName=CM_Template_Ad_Cat */
public static String Table_Name="CM_Template_Ad_Cat";

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
StringBuilder sb = new StringBuilder ("X_CM_Template_Ad_Cat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Advertisement Category.
@param CM_Ad_Cat_ID Advertisement Category like Banner Homepage */
public void SetCM_Ad_Cat_ID (int CM_Ad_Cat_ID)
{
if (CM_Ad_Cat_ID < 1) throw new ArgumentException ("CM_Ad_Cat_ID is mandatory.");
Set_ValueNoCheck ("CM_Ad_Cat_ID", CM_Ad_Cat_ID);
}
/** Get Advertisement Category.
@return Advertisement Category like Banner Homepage */
public int GetCM_Ad_Cat_ID() 
{
Object ii = Get_Value("CM_Ad_Cat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Template.
@param CM_Template_ID Template defines how content is displayed */
public void SetCM_Template_ID (int CM_Template_ID)
{
if (CM_Template_ID < 1) throw new ArgumentException ("CM_Template_ID is mandatory.");
Set_ValueNoCheck ("CM_Template_ID", CM_Template_ID);
}
/** Get Template.
@return Template defines how content is displayed */
public int GetCM_Template_ID() 
{
Object ii = Get_Value("CM_Template_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetCM_Template_ID().ToString());
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
}

}
