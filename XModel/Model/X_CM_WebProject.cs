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
/** Generated Model for CM_WebProject
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_WebProject : PO
{
public X_CM_WebProject (Context ctx, int CM_WebProject_ID, Trx trxName) : base (ctx, CM_WebProject_ID, trxName)
{
/** if (CM_WebProject_ID == 0)
{
SetVAF_TreeInfoCMC_ID (0);
SetVAF_TreeInfoCMM_ID (0);
SetVAF_TreeInfoCMS_ID (0);
SetVAF_TreeInfoCMT_ID (0);
SetCM_WebProject_ID (0);
SetMeta_Author (null);	// @VAF_UserContact_Name@
SetMeta_Content (null);	// 'text/html;
 charset=UTF-8'
SetMeta_Copyright (null);	// @VAF_Client_Name@
SetMeta_Publisher (null);	// @VAF_Client_Name@
SetMeta_RobotsTag (null);	// 'INDEX,FOLLOW'
SetName (null);
}
 */
}
public X_CM_WebProject (Ctx ctx, int CM_WebProject_ID, Trx trxName) : base (ctx, CM_WebProject_ID, trxName)
{
/** if (CM_WebProject_ID == 0)
{
SetVAF_TreeInfoCMC_ID (0);
SetVAF_TreeInfoCMM_ID (0);
SetVAF_TreeInfoCMS_ID (0);
SetVAF_TreeInfoCMT_ID (0);
SetCM_WebProject_ID (0);
SetMeta_Author (null);	// @VAF_UserContact_Name@
SetMeta_Content (null);	// 'text/html;
 charset=UTF-8'
SetMeta_Copyright (null);	// @VAF_Client_Name@
SetMeta_Publisher (null);	// @VAF_Client_Name@
SetMeta_RobotsTag (null);	// 'INDEX,FOLLOW'
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_WebProject()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369256L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052467L;
/** VAF_TableView_ID=853 */
public static int Table_ID;
 // =853;

/** TableName=CM_WebProject */
public static String Table_Name="CM_WebProject";

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
StringBuilder sb = new StringBuilder ("X_CM_WebProject[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_TreeInfoCMC_ID VAF_Control_Ref_ID=184 */
public static int VAF_TreeInfoCMC_ID_VAF_Control_Ref_ID=184;
/** Set Container Tree.
@param VAF_TreeInfoCMC_ID Container Tree */
public void SetVAF_TreeInfoCMC_ID (int VAF_TreeInfoCMC_ID)
{
if (VAF_TreeInfoCMC_ID < 1) throw new ArgumentException ("VAF_TreeInfoCMC_ID is mandatory.");
Set_ValueNoCheck ("VAF_TreeInfoCMC_ID", VAF_TreeInfoCMC_ID);
}
/** Get Container Tree.
@return Container Tree */
public int GetVAF_TreeInfoCMC_ID() 
{
Object ii = Get_Value("VAF_TreeInfoCMC_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_TreeInfoCMM_ID VAF_Control_Ref_ID=184 */
public static int VAF_TreeInfoCMM_ID_VAF_Control_Ref_ID=184;
/** Set Media Tree.
@param VAF_TreeInfoCMM_ID Media Tree */
public void SetVAF_TreeInfoCMM_ID (int VAF_TreeInfoCMM_ID)
{
if (VAF_TreeInfoCMM_ID < 1) throw new ArgumentException ("VAF_TreeInfoCMM_ID is mandatory.");
Set_ValueNoCheck ("VAF_TreeInfoCMM_ID", VAF_TreeInfoCMM_ID);
}
/** Get Media Tree.
@return Media Tree */
public int GetVAF_TreeInfoCMM_ID() 
{
Object ii = Get_Value("VAF_TreeInfoCMM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_TreeInfoCMS_ID VAF_Control_Ref_ID=184 */
public static int VAF_TreeInfoCMS_ID_VAF_Control_Ref_ID=184;
/** Set Stage Tree.
@param VAF_TreeInfoCMS_ID Stage Tree */
public void SetVAF_TreeInfoCMS_ID (int VAF_TreeInfoCMS_ID)
{
if (VAF_TreeInfoCMS_ID < 1) throw new ArgumentException ("VAF_TreeInfoCMS_ID is mandatory.");
Set_ValueNoCheck ("VAF_TreeInfoCMS_ID", VAF_TreeInfoCMS_ID);
}
/** Get Stage Tree.
@return Stage Tree */
public int GetVAF_TreeInfoCMS_ID() 
{
Object ii = Get_Value("VAF_TreeInfoCMS_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_TreeInfoCMT_ID VAF_Control_Ref_ID=184 */
public static int VAF_TreeInfoCMT_ID_VAF_Control_Ref_ID=184;
/** Set Template Tree.
@param VAF_TreeInfoCMT_ID Template Tree */
public void SetVAF_TreeInfoCMT_ID (int VAF_TreeInfoCMT_ID)
{
if (VAF_TreeInfoCMT_ID < 1) throw new ArgumentException ("VAF_TreeInfoCMT_ID is mandatory.");
Set_ValueNoCheck ("VAF_TreeInfoCMT_ID", VAF_TreeInfoCMT_ID);
}
/** Get Template Tree.
@return Template Tree */
public int GetVAF_TreeInfoCMT_ID() 
{
Object ii = Get_Value("VAF_TreeInfoCMT_ID");
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
/** Set Meta Author.
@param Meta_Author Author of the content */
public void SetMeta_Author (String Meta_Author)
{
if (Meta_Author == null) throw new ArgumentException ("Meta_Author is mandatory.");
if (Meta_Author.Length > 2000)
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
if (Meta_Content == null) throw new ArgumentException ("Meta_Content is mandatory.");
if (Meta_Content.Length > 2000)
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
if (Meta_Copyright == null) throw new ArgumentException ("Meta_Copyright is mandatory.");
if (Meta_Copyright.Length > 2000)
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
/** Set Meta Publisher.
@param Meta_Publisher Meta Publisher defines the publisher of the content */
public void SetMeta_Publisher (String Meta_Publisher)
{
if (Meta_Publisher == null) throw new ArgumentException ("Meta_Publisher is mandatory.");
if (Meta_Publisher.Length > 2000)
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
if (Meta_RobotsTag == null) throw new ArgumentException ("Meta_RobotsTag is mandatory.");
if (Meta_RobotsTag.Length > 2000)
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
}

}
