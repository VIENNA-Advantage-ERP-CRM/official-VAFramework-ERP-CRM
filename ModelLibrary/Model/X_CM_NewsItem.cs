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
/** Generated Model for CM_NewsItem
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_NewsItem : PO
{
public X_CM_NewsItem (Context ctx, int CM_NewsItem_ID, Trx trxName) : base (ctx, CM_NewsItem_ID, trxName)
{
/** if (CM_NewsItem_ID == 0)
{
SetCM_NewsChannel_ID (0);
SetCM_NewsItem_ID (0);
}
 */
}
public X_CM_NewsItem (Ctx ctx, int CM_NewsItem_ID, Trx trxName) : base (ctx, CM_NewsItem_ID, trxName)
{
/** if (CM_NewsItem_ID == 0)
{
SetCM_NewsChannel_ID (0);
SetCM_NewsItem_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsItem (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsItem (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_NewsItem (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_NewsItem()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369147L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052358L;
/** AD_Table_ID=871 */
public static int Table_ID;
 // =871;

/** TableName=CM_NewsItem */
public static String Table_Name="CM_NewsItem";

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
StringBuilder sb = new StringBuilder ("X_CM_NewsItem[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Author.
@param Author Author/Creator of the Entity */
public void SetAuthor (String Author)
{
if (Author != null && Author.Length > 255)
{
log.Warning("Length > 255 - truncated");
Author = Author.Substring(0,255);
}
Set_Value ("Author", Author);
}
/** Get Author.
@return Author/Creator of the Entity */
public String GetAuthor() 
{
return (String)Get_Value("Author");
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
/** Set News Item / Article.
@param CM_NewsItem_ID News item or article defines base content */
public void SetCM_NewsItem_ID (int CM_NewsItem_ID)
{
if (CM_NewsItem_ID < 1) throw new ArgumentException ("CM_NewsItem_ID is mandatory.");
Set_ValueNoCheck ("CM_NewsItem_ID", CM_NewsItem_ID);
}
/** Get News Item / Article.
@return News item or article defines base content */
public int GetCM_NewsItem_ID() 
{
Object ii = Get_Value("CM_NewsItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Content HTML.
@param ContentHTML Contains the content itself */
public void SetContentHTML (String ContentHTML)
{
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
/** Set LinkURL.
@param LinkURL Contains URL to a target */
public void SetLinkURL (String LinkURL)
{
if (LinkURL != null && LinkURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
LinkURL = LinkURL.Substring(0,120);
}
Set_Value ("LinkURL", LinkURL);
}
/** Get LinkURL.
@return Contains URL to a target */
public String GetLinkURL() 
{
return (String)Get_Value("LinkURL");
}
/** Set Publication Date.
@param PubDate Date on which this article will / should get published */
public void SetPubDate (DateTime? PubDate)
{
Set_Value ("PubDate", (DateTime?)PubDate);
}
/** Get Publication Date.
@return Date on which this article will / should get published */
public DateTime? GetPubDate() 
{
return (DateTime?)Get_Value("PubDate");
}
/** Set Title.
@param Title Title of the Contact */
public void SetTitle (String Title)
{
if (Title != null && Title.Length > 255)
{
log.Warning("Length > 255 - truncated");
Title = Title.Substring(0,255);
}
Set_Value ("Title", Title);
}
/** Get Title.
@return Title of the Contact */
public String GetTitle() 
{
return (String)Get_Value("Title");
}
}

}
