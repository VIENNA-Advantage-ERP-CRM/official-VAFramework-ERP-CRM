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
/** Generated Model for CM_CStage
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_CStage : PO
{
public X_CM_CStage (Context ctx, int CM_CStage_ID, Trx trxName) : base (ctx, CM_CStage_ID, trxName)
{
/** if (CM_CStage_ID == 0)
{
SetCM_CStage_ID (0);
SetCM_WebProject_ID (0);
SetIsIndexed (true);	// Y
SetIsModified (false);
SetIsSecure (false);
SetIsSummary (false);
SetName (null);
}
 */
}
public X_CM_CStage (Ctx ctx, int CM_CStage_ID, Trx trxName) : base (ctx, CM_CStage_ID, trxName)
{
/** if (CM_CStage_ID == 0)
{
SetCM_CStage_ID (0);
SetCM_WebProject_ID (0);
SetIsIndexed (true);	// Y
SetIsModified (false);
SetIsSecure (false);
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
public X_CM_CStage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_CStage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_CStage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_CStage()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368379L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051590L;
/** AD_Table_ID=866 */
public static int Table_ID;
 // =866;

/** TableName=CM_CStage */
public static String Table_Name="CM_CStage";

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
StringBuilder sb = new StringBuilder ("X_CM_CStage[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** CM_CStageLink_ID AD_Reference_ID=387 */
public static int CM_CSTAGELINK_ID_AD_Reference_ID=387;
/** Set Container Link.
@param CM_CStageLink_ID Stage Link to another Container in the Web Project */
public void SetCM_CStageLink_ID (int CM_CStageLink_ID)
{
if (CM_CStageLink_ID <= 0) Set_Value ("CM_CStageLink_ID", null);
else
Set_Value ("CM_CStageLink_ID", CM_CStageLink_ID);
}
/** Get Container Link.
@return Stage Link to another Container in the Web Project */
public int GetCM_CStageLink_ID() 
{
Object ii = Get_Value("CM_CStageLink_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Container Stage.
@param CM_CStage_ID Web Container Stage contains the staging content like images, text etc. */
public void SetCM_CStage_ID (int CM_CStage_ID)
{
if (CM_CStage_ID < 1) throw new ArgumentException ("CM_CStage_ID is mandatory.");
Set_ValueNoCheck ("CM_CStage_ID", CM_CStage_ID);
}
/** Get Web Container Stage.
@return Web Container Stage contains the staging content like images, text etc. */
public int GetCM_CStage_ID() 
{
Object ii = Get_Value("CM_CStage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Template.
@param CM_Template_ID Template defines how content is displayed */
public void SetCM_Template_ID (int CM_Template_ID)
{
if (CM_Template_ID <= 0) Set_Value ("CM_Template_ID", null);
else
Set_Value ("CM_Template_ID", CM_Template_ID);
}
/** Get Template.
@return Template defines how content is displayed */
public int GetCM_Template_ID() 
{
Object ii = Get_Value("CM_Template_ID");
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
/** Set External Link (URL).
@param ContainerLinkURL External Link (URL) for the Container */
public void SetContainerLinkURL (String ContainerLinkURL)
{
if (ContainerLinkURL != null && ContainerLinkURL.Length > 60)
{
log.Warning("Length > 60 - truncated");
ContainerLinkURL = ContainerLinkURL.Substring(0,60);
}
Set_Value ("ContainerLinkURL", ContainerLinkURL);
}
/** Get External Link (URL).
@return External Link (URL) for the Container */
public String GetContainerLinkURL() 
{
return (String)Get_Value("ContainerLinkURL");
}

/** ContainerType AD_Reference_ID=385 */
public static int CONTAINERTYPE_AD_Reference_ID=385;
/** Document = D */
public static String CONTAINERTYPE_Document = "D";
/** Internal Link = L */
public static String CONTAINERTYPE_InternalLink = "L";
/** External URL = U */
public static String CONTAINERTYPE_ExternalURL = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsContainerTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("L") || test.Equals("U");
}
/** Set Web Container Type.
@param ContainerType Web Container Type */
public void SetContainerType (String ContainerType)
{
if (!IsContainerTypeValid(ContainerType))
throw new ArgumentException ("ContainerType Invalid value - " + ContainerType + " - Reference_ID=385 - D - L - U");
if (ContainerType != null && ContainerType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ContainerType = ContainerType.Substring(0,1);
}
Set_Value ("ContainerType", ContainerType);
}
/** Get Web Container Type.
@return Web Container Type */
public String GetContainerType() 
{
return (String)Get_Value("ContainerType");
}
/** Set ContainerXML.
@param ContainerXML Autogenerated Containerdefinition as XML Code */
public void SetContainerXML (String ContainerXML)
{
if (ContainerXML != null && ContainerXML.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ContainerXML = ContainerXML.Substring(0,2000);
}
Set_ValueNoCheck ("ContainerXML", ContainerXML);
}
/** Get ContainerXML.
@return Autogenerated Containerdefinition as XML Code */
public String GetContainerXML() 
{
return (String)Get_Value("ContainerXML");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 2000)
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
/** Set Indexed.
@param IsIndexed Index the document for the internal search engine */
public void SetIsIndexed (Boolean IsIndexed)
{
Set_Value ("IsIndexed", IsIndexed);
}
/** Get Indexed.
@return Index the document for the internal search engine */
public Boolean IsIndexed() 
{
Object oo = Get_Value("IsIndexed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Modified.
@param IsModified The record is modified */
public void SetIsModified (Boolean IsModified)
{
Set_ValueNoCheck ("IsModified", IsModified);
}
/** Get Modified.
@return The record is modified */
public Boolean IsModified() 
{
Object oo = Get_Value("IsModified");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Secure content.
@param IsSecure Defines whether content needs to get encrypted */
public void SetIsSecure (Boolean IsSecure)
{
Set_Value ("IsSecure", IsSecure);
}
/** Get Secure content.
@return Defines whether content needs to get encrypted */
public Boolean IsSecure() 
{
Object oo = Get_Value("IsSecure");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_ValueNoCheck ("IsSummary", IsSummary);
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
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Meta Author.
@param Meta_Author Author of the content */
public void SetMeta_Author (String Meta_Author)
{
if (Meta_Author != null && Meta_Author.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Author = Meta_Author.Substring(0,2000);
}
Set_Value ("Meta_Author", Meta_Author);
}
/** Get Meta Author.
@return Author of the content */
public String GetMeta_Author() 
{
return (String)Get_Value("Meta_Author");
}
/** Set Meta Content Type.
@param Meta_Content Defines the type of content i.e. "text/html;
 charset=UTF-8" */
public void SetMeta_Content (String Meta_Content)
{
if (Meta_Content != null && Meta_Content.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Content = Meta_Content.Substring(0,2000);
}
Set_Value ("Meta_Content", Meta_Content);
}
/** Get Meta Content Type.
@return Defines the type of content i.e. "text/html;
 charset=UTF-8" */
public String GetMeta_Content() 
{
return (String)Get_Value("Meta_Content");
}
/** Set Meta Copyright.
@param Meta_Copyright Contains Copyright information for the content */
public void SetMeta_Copyright (String Meta_Copyright)
{
if (Meta_Copyright != null && Meta_Copyright.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Copyright = Meta_Copyright.Substring(0,2000);
}
Set_Value ("Meta_Copyright", Meta_Copyright);
}
/** Get Meta Copyright.
@return Contains Copyright information for the content */
public String GetMeta_Copyright() 
{
return (String)Get_Value("Meta_Copyright");
}
/** Set Meta Description.
@param Meta_Description Meta info describing the contents of the page */
public void SetMeta_Description (String Meta_Description)
{
if (Meta_Description != null && Meta_Description.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Description = Meta_Description.Substring(0,2000);
}
Set_Value ("Meta_Description", Meta_Description);
}
/** Get Meta Description.
@return Meta info describing the contents of the page */
public String GetMeta_Description() 
{
return (String)Get_Value("Meta_Description");
}
/** Set Meta Keywords.
@param Meta_Keywords Contains the keywords for the content */
public void SetMeta_Keywords (String Meta_Keywords)
{
if (Meta_Keywords != null && Meta_Keywords.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Keywords = Meta_Keywords.Substring(0,2000);
}
Set_Value ("Meta_Keywords", Meta_Keywords);
}
/** Get Meta Keywords.
@return Contains the keywords for the content */
public String GetMeta_Keywords() 
{
return (String)Get_Value("Meta_Keywords");
}
/** Set Meta Language.
@param Meta_Language Language HTML Meta Tag */
public void SetMeta_Language (String Meta_Language)
{
if (Meta_Language != null && Meta_Language.Length > 2)
{
log.Warning("Length > 2 - truncated");
Meta_Language = Meta_Language.Substring(0,2);
}
Set_Value ("Meta_Language", Meta_Language);
}
/** Get Meta Language.
@return Language HTML Meta Tag */
public String GetMeta_Language() 
{
return (String)Get_Value("Meta_Language");
}
/** Set Meta Publisher.
@param Meta_Publisher Meta Publisher defines the publisher of the content */
public void SetMeta_Publisher (String Meta_Publisher)
{
if (Meta_Publisher != null && Meta_Publisher.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_Publisher = Meta_Publisher.Substring(0,2000);
}
Set_Value ("Meta_Publisher", Meta_Publisher);
}
/** Get Meta Publisher.
@return Meta Publisher defines the publisher of the content */
public String GetMeta_Publisher() 
{
return (String)Get_Value("Meta_Publisher");
}
/** Set Meta RobotsTag.
@param Meta_RobotsTag RobotsTag defines how search robots should handle this content */
public void SetMeta_RobotsTag (String Meta_RobotsTag)
{
if (Meta_RobotsTag != null && Meta_RobotsTag.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Meta_RobotsTag = Meta_RobotsTag.Substring(0,2000);
}
Set_Value ("Meta_RobotsTag", Meta_RobotsTag);
}
/** Get Meta RobotsTag.
@return RobotsTag defines how search robots should handle this content */
public String GetMeta_RobotsTag() 
{
return (String)Get_Value("Meta_RobotsTag");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Relative URL.
@param RelativeURL Contains the relative URL for the container */
public void SetRelativeURL (String RelativeURL)
{
if (RelativeURL != null && RelativeURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
RelativeURL = RelativeURL.Substring(0,120);
}
Set_Value ("RelativeURL", RelativeURL);
}
/** Get Relative URL.
@return Contains the relative URL for the container */
public String GetRelativeURL() 
{
return (String)Get_Value("RelativeURL");
}
/** Set StructureXML.
@param StructureXML Autogenerated Containerdefinition as XML Code */
public void SetStructureXML (String StructureXML)
{
if (StructureXML != null && StructureXML.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
StructureXML = StructureXML.Substring(0,2000);
}
Set_Value ("StructureXML", StructureXML);
}
/** Get StructureXML.
@return Autogenerated Containerdefinition as XML Code */
public String GetStructureXML() 
{
return (String)Get_Value("StructureXML");
}
/** Set Title.
@param Title Title of the Contact */
public void SetTitle (String Title)
{
if (Title != null && Title.Length > 60)
{
log.Warning("Length > 60 - truncated");
Title = Title.Substring(0,60);
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
