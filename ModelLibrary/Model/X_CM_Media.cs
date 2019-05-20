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
/** Generated Model for CM_Media
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Media : PO
{
public X_CM_Media (Context ctx, int CM_Media_ID, Trx trxName) : base (ctx, CM_Media_ID, trxName)
{
/** if (CM_Media_ID == 0)
{
SetCM_Media_ID (0);
SetCM_WebProject_ID (0);
SetIsSummary (false);
SetName (null);
}
 */
}
public X_CM_Media (Ctx ctx, int CM_Media_ID, Trx trxName) : base (ctx, CM_Media_ID, trxName)
{
/** if (CM_Media_ID == 0)
{
SetCM_Media_ID (0);
SetCM_WebProject_ID (0);
SetIsSummary (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Media()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369006L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052217L;
/** AD_Table_ID=857 */
public static int Table_ID;
 // =857;

/** TableName=CM_Media */
public static String Table_Name="CM_Media";

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
StringBuilder sb = new StringBuilder ("X_CM_Media[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Media Item.
@param CM_Media_ID Contains media content like images, flash movies etc. */
public void SetCM_Media_ID (int CM_Media_ID)
{
if (CM_Media_ID < 1) throw new ArgumentException ("CM_Media_ID is mandatory.");
Set_ValueNoCheck ("CM_Media_ID", CM_Media_ID);
}
/** Get Media Item.
@return Contains media content like images, flash movies etc. */
public int GetCM_Media_ID() 
{
Object ii = Get_Value("CM_Media_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID < 1) throw new ArgumentException ("CM_WebProject_ID is mandatory.");
Set_ValueNoCheck ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Content.
@param ContentText Content */
public void SetContentText (String ContentText)
{
Set_Value ("ContentText", ContentText);
}
/** Get Content.
@return Content */
public String GetContentText() 
{
return (String)Get_Value("ContentText");
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
/** Set Direct Deploy.
@param DirectDeploy Direct Deploy */
public void SetDirectDeploy (String DirectDeploy)
{
if (DirectDeploy != null && DirectDeploy.Length > 1)
{
log.Warning("Length > 1 - truncated");
DirectDeploy = DirectDeploy.Substring(0,1);
}
Set_Value ("DirectDeploy", DirectDeploy);
}
/** Get Direct Deploy.
@return Direct Deploy */
public String GetDirectDeploy() 
{
return (String)Get_Value("DirectDeploy");
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
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** MediaType AD_Reference_ID=388 */
public static int MEDIATYPE_AD_Reference_ID=388;
/** text/css = CSS */
public static String MEDIATYPE_TextCss = "CSS";
/** image/gif = GIF */
public static String MEDIATYPE_ImageGif = "GIF";
/** image/jpeg = JPG */
public static String MEDIATYPE_ImageJpeg = "JPG";
/** text/js = JS */
public static String MEDIATYPE_TextJs = "JS";
/** application/pdf = PDF */
public static String MEDIATYPE_ApplicationPdf = "PDF";
/** image/png = PNG */
public static String MEDIATYPE_ImagePng = "PNG";
/** text/xml = XML */
public static String MEDIATYPE_TextXml = "XML";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMediaTypeValid (String test)
{
return test == null || test.Equals("CSS") || test.Equals("GIF") || test.Equals("JPG") || test.Equals("JS") || test.Equals("PDF") || test.Equals("PNG") || test.Equals("XML");
}
/** Set Media Type.
@param MediaType Defines the media type for the browser */
public void SetMediaType (String MediaType)
{
if (!IsMediaTypeValid(MediaType))
throw new ArgumentException ("MediaType Invalid value - " + MediaType + " - Reference_ID=388 - CSS - GIF - JPG - JS - PDF - PNG - XML");
if (MediaType != null && MediaType.Length > 3)
{
log.Warning("Length > 3 - truncated");
MediaType = MediaType.Substring(0,3);
}
Set_Value ("MediaType", MediaType);
}
/** Get Media Type.
@return Defines the media type for the browser */
public String GetMediaType() 
{
return (String)Get_Value("MediaType");
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
