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
/** Generated Model for CM_NewsChannel
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_NewsChannel : PO
{
public X_CM_NewsChannel (Context ctx, int CM_NewsChannel_ID, Trx trxName) : base (ctx, CM_NewsChannel_ID, trxName)
{
/** if (CM_NewsChannel_ID == 0)
{
SetCM_NewsChannel_ID (0);
SetCM_WebProject_ID (0);
SetDescription (null);
SetName (null);
}
 */
}
public X_CM_NewsChannel (Ctx ctx, int CM_NewsChannel_ID, Trx trxName) : base (ctx, CM_NewsChannel_ID, trxName)
{
/** if (CM_NewsChannel_ID == 0)
{
SetCM_NewsChannel_ID (0);
SetCM_WebProject_ID (0);
SetDescription (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsChannel (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsChannel (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsChannel (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_NewsChannel()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369131L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052342L;
/** AD_Table_ID=870 */
public static int Table_ID;
 // =870;

/** TableName=CM_NewsChannel */
public static String Table_Name="CM_NewsChannel";

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
StringBuilder sb = new StringBuilder ("X_CM_NewsChannel[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set News Channel.
@param CM_NewsChannel_ID News channel for rss feed */
public void SetCM_NewsChannel_ID (int CM_NewsChannel_ID)
{
if (CM_NewsChannel_ID < 1) throw new ArgumentException ("CM_NewsChannel_ID is mandatory.");
Set_ValueNoCheck ("CM_NewsChannel_ID", CM_NewsChannel_ID);
}
/** Get News Channel.
@return News channel for rss feed */
public int GetCM_NewsChannel_ID() 
{
Object ii = Get_Value("CM_NewsChannel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID < 1) throw new ArgumentException ("CM_WebProject_ID is mandatory.");
Set_Value ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description == null) throw new ArgumentException ("Description is mandatory.");
if (Description.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Description = Description.Substring(0,2000);
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
/** Set Link.
@param Link Contains URL to a target */
public void SetLink (String Link)
{
if (Link != null && Link.Length > 255)
{
log.Warning("Length > 255 - truncated");
Link = Link.Substring(0,255);
}
Set_Value ("Link", Link);
}
/** Get Link.
@return Contains URL to a target */
public String GetLink() 
{
return (String)Get_Value("Link");
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
