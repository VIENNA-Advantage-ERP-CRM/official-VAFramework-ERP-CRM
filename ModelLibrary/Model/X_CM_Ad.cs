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
/** Generated Model for CM_Ad
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Ad : PO
{
public X_CM_Ad (Context ctx, int CM_Ad_ID, Trx trxName) : base (ctx, CM_Ad_ID, trxName)
{
/** if (CM_Ad_ID == 0)
{
SetActualClick (0);
SetActualImpression (0);
SetCM_Ad_Cat_ID (0);
SetCM_Ad_ID (0);
SetCM_Media_ID (0);
SetIsAdFlag (false);
SetIsLogged (false);
SetMaxClick (0);
SetMaxImpression (0);
SetName (null);
SetStartDate (DateTime.Now);
SetStartImpression (0);
SetTarget_Frame (null);
}
 */
}
public X_CM_Ad (Ctx ctx, int CM_Ad_ID, Trx trxName) : base (ctx, CM_Ad_ID, trxName)
{
/** if (CM_Ad_ID == 0)
{
SetActualClick (0);
SetActualImpression (0);
SetCM_Ad_Cat_ID (0);
SetCM_Ad_ID (0);
SetCM_Media_ID (0);
SetIsAdFlag (false);
SetIsLogged (false);
SetMaxClick (0);
SetMaxImpression (0);
SetName (null);
SetStartDate (DateTime.Now);
SetStartImpression (0);
SetTarget_Frame (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Ad (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Ad (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Ad (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Ad()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368222L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051433L;
/** AD_Table_ID=858 */
public static int Table_ID;
 // =858;

/** TableName=CM_Ad */
public static String Table_Name="CM_Ad";

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
StringBuilder sb = new StringBuilder ("X_CM_Ad[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Actual Click Count.
@param ActualClick How many clicks have been counted */
public void SetActualClick (int ActualClick)
{
Set_Value ("ActualClick", ActualClick);
}
/** Get Actual Click Count.
@return How many clicks have been counted */
public int GetActualClick() 
{
Object ii = Get_Value("ActualClick");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Actual Impression Count.
@param ActualImpression How many impressions have been counted */
public void SetActualImpression (int ActualImpression)
{
Set_Value ("ActualImpression", ActualImpression);
}
/** Get Actual Impression Count.
@return How many impressions have been counted */
public int GetActualImpression() 
{
Object ii = Get_Value("ActualImpression");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Advertisement.
@param CM_Ad_ID An Advertisement is something like a banner */
public void SetCM_Ad_ID (int CM_Ad_ID)
{
if (CM_Ad_ID < 1) throw new ArgumentException ("CM_Ad_ID is mandatory.");
Set_ValueNoCheck ("CM_Ad_ID", CM_Ad_ID);
}
/** Get Advertisement.
@return An Advertisement is something like a banner */
public int GetCM_Ad_ID() 
{
Object ii = Get_Value("CM_Ad_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Media Item.
@param CM_Media_ID Contains media content like images, flash movies etc. */
public void SetCM_Media_ID (int CM_Media_ID)
{
if (CM_Media_ID < 1) throw new ArgumentException ("CM_Media_ID is mandatory.");
Set_Value ("CM_Media_ID", CM_Media_ID);
}
/** Get Media Item.
@return Contains media content like images, flash movies etc. */
public int GetCM_Media_ID() 
{
Object ii = Get_Value("CM_Media_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Content HTML.
@param ContentHTML Contains the content itself */
public void SetContentHTML (String ContentHTML)
{
if (ContentHTML != null && ContentHTML.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ContentHTML = ContentHTML.Substring(0,2000);
}
Set_Value ("ContentHTML", ContentHTML);
}
/** Get Content HTML.
@return Contains the content itself */
public String GetContentHTML() 
{
return (String)Get_Value("ContentHTML");
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
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
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
/** Set Special AD Flag.
@param IsAdFlag Do we need to specially mention this ad? */
public void SetIsAdFlag (Boolean IsAdFlag)
{
Set_Value ("IsAdFlag", IsAdFlag);
}
/** Get Special AD Flag.
@return Do we need to specially mention this ad? */
public Boolean IsAdFlag() 
{
Object oo = Get_Value("IsAdFlag");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Logging.
@param IsLogged Do we need to log the banner impressions and clicks? (needs much performance) */
public void SetIsLogged (Boolean IsLogged)
{
Set_Value ("IsLogged", IsLogged);
}
/** Get Logging.
@return Do we need to log the banner impressions and clicks? (needs much performance) */
public Boolean IsLogged() 
{
Object oo = Get_Value("IsLogged");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Max Click Count.
@param MaxClick Maximum Click Count until banner is deactivated */
public void SetMaxClick (int MaxClick)
{
Set_Value ("MaxClick", MaxClick);
}
/** Get Max Click Count.
@return Maximum Click Count until banner is deactivated */
public int GetMaxClick() 
{
Object ii = Get_Value("MaxClick");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Max Impression Count.
@param MaxImpression Maximum Impression Count until banner is deactivated */
public void SetMaxImpression (int MaxImpression)
{
Set_Value ("MaxImpression", MaxImpression);
}
/** Get Max Impression Count.
@return Maximum Impression Count until banner is deactivated */
public int GetMaxImpression() 
{
Object ii = Get_Value("MaxImpression");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Set Start Count Impression.
@param StartImpression For rotation we need a start count */
public void SetStartImpression (int StartImpression)
{
Set_Value ("StartImpression", StartImpression);
}
/** Get Start Count Impression.
@return For rotation we need a start count */
public int GetStartImpression() 
{
Object ii = Get_Value("StartImpression");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Target URL.
@param TargetURL URL for the Target */
public void SetTargetURL (String TargetURL)
{
if (TargetURL != null && TargetURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
TargetURL = TargetURL.Substring(0,120);
}
Set_Value ("TargetURL", TargetURL);
}
/** Get Target URL.
@return URL for the Target */
public String GetTargetURL() 
{
return (String)Get_Value("TargetURL");
}
/** Set Target Frame.
@param Target_Frame Which target should be used if user clicks? */
public void SetTarget_Frame (String Target_Frame)
{
if (Target_Frame == null) throw new ArgumentException ("Target_Frame is mandatory.");
if (Target_Frame.Length > 20)
{
log.Warning("Length > 20 - truncated");
Target_Frame = Target_Frame.Substring(0,20);
}
Set_Value ("Target_Frame", Target_Frame);
}
/** Get Target Frame.
@return Which target should be used if user clicks? */
public String GetTarget_Frame() 
{
return (String)Get_Value("Target_Frame");
}
}

}
