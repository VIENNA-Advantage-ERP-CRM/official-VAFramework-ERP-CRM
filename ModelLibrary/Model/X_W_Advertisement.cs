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
/** Generated Model for W_Advertisement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_Advertisement : PO
{
public X_W_Advertisement (Context ctx, int W_Advertisement_ID, Trx trxName) : base (ctx, W_Advertisement_ID, trxName)
{
/** if (W_Advertisement_ID == 0)
{
SetC_BPartner_ID (0);
SetIsSelfService (true);	// Y
SetName (null);
SetPublishStatus (null);	// U
SetW_Advertisement_ID (0);
}
 */
}
public X_W_Advertisement (Ctx ctx, int W_Advertisement_ID, Trx trxName) : base (ctx, W_Advertisement_ID, trxName)
{
/** if (W_Advertisement_ID == 0)
{
SetC_BPartner_ID (0);
SetIsSelfService (true);	// Y
SetName (null);
SetPublishStatus (null);	// U
SetW_Advertisement_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Advertisement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Advertisement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Advertisement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_Advertisement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384866L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068077L;
/** AD_Table_ID=579 */
public static int Table_ID;
 // =579;

/** TableName=W_Advertisement */
public static String Table_Name="W_Advertisement";

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
StringBuilder sb = new StringBuilder ("X_W_Advertisement[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Advertisement Text.
@param AdText Text of the Advertisement */
public void SetAdText (String AdText)
{
if (AdText != null && AdText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
AdText = AdText.Substring(0,2000);
}
Set_Value ("AdText", AdText);
}
/** Get Advertisement Text.
@return Text of the Advertisement */
public String GetAdText() 
{
return (String)Get_Value("AdText");
}

/** C_BPartner_ID AD_Reference_ID=232 */
public static int C_BPARTNER_ID_AD_Reference_ID=232;
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
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
/** Set Image URL.
@param ImageURL URL of  image */
public void SetImageURL (String ImageURL)
{
if (ImageURL != null && ImageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
ImageURL = ImageURL.Substring(0,120);
}
Set_Value ("ImageURL", ImageURL);
}
/** Get Image URL.
@return URL of  image */
public String GetImageURL() 
{
return (String)Get_Value("ImageURL");
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
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

/** PublishStatus AD_Reference_ID=310 */
public static int PUBLISHSTATUS_AD_Reference_ID=310;
/** Released = R */
public static String PUBLISHSTATUS_Released = "R";
/** Test = T */
public static String PUBLISHSTATUS_Test = "T";
/** Under Revision = U */
public static String PUBLISHSTATUS_UnderRevision = "U";
/** Void = V */
public static String PUBLISHSTATUS_Void = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPublishStatusValid (String test)
{
return test.Equals("R") || test.Equals("T") || test.Equals("U") || test.Equals("V");
}
/** Set Publication Status.
@param PublishStatus Status of Publication */
public void SetPublishStatus (String PublishStatus)
{
if (PublishStatus == null) throw new ArgumentException ("PublishStatus is mandatory");
if (!IsPublishStatusValid(PublishStatus))
throw new ArgumentException ("PublishStatus Invalid value - " + PublishStatus + " - Reference_ID=310 - R - T - U - V");
if (PublishStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
PublishStatus = PublishStatus.Substring(0,1);
}
Set_Value ("PublishStatus", PublishStatus);
}
/** Get Publication Status.
@return Status of Publication */
public String GetPublishStatus() 
{
return (String)Get_Value("PublishStatus");
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
/** Set Version.
@param Version Version of the table definition */
public void SetVersion (int Version)
{
Set_Value ("Version", Version);
}
/** Get Version.
@return Version of the table definition */
public int GetVersion() 
{
Object ii = Get_Value("Version");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Advertisement.
@param W_Advertisement_ID Web Advertisement */
public void SetW_Advertisement_ID (int W_Advertisement_ID)
{
if (W_Advertisement_ID < 1) throw new ArgumentException ("W_Advertisement_ID is mandatory.");
Set_ValueNoCheck ("W_Advertisement_ID", W_Advertisement_ID);
}
/** Get Advertisement.
@return Web Advertisement */
public int GetW_Advertisement_ID() 
{
Object ii = Get_Value("W_Advertisement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Click Count.
@param W_ClickCount_ID Web Click Management */
public void SetW_ClickCount_ID (int W_ClickCount_ID)
{
if (W_ClickCount_ID <= 0) Set_Value ("W_ClickCount_ID", null);
else
Set_Value ("W_ClickCount_ID", W_ClickCount_ID);
}
/** Get Click Count.
@return Web Click Management */
public int GetW_ClickCount_ID() 
{
Object ii = Get_Value("W_ClickCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Counter Count.
@param W_CounterCount_ID Web Counter Count Management */
public void SetW_CounterCount_ID (int W_CounterCount_ID)
{
if (W_CounterCount_ID <= 0) Set_Value ("W_CounterCount_ID", null);
else
Set_Value ("W_CounterCount_ID", W_CounterCount_ID);
}
/** Get Counter Count.
@return Web Counter Count Management */
public int GetW_CounterCount_ID() 
{
Object ii = Get_Value("W_CounterCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Parameter 1.
@param WebParam1 Web Site Parameter 1 (default: header image) */
public void SetWebParam1 (String WebParam1)
{
if (WebParam1 != null && WebParam1.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam1 = WebParam1.Substring(0,2000);
}
Set_Value ("WebParam1", WebParam1);
}
/** Get Web Parameter 1.
@return Web Site Parameter 1 (default: header image) */
public String GetWebParam1() 
{
return (String)Get_Value("WebParam1");
}
/** Set Web Parameter 2.
@param WebParam2 Web Site Parameter 2 (default index page) */
public void SetWebParam2 (String WebParam2)
{
if (WebParam2 != null && WebParam2.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam2 = WebParam2.Substring(0,2000);
}
Set_Value ("WebParam2", WebParam2);
}
/** Get Web Parameter 2.
@return Web Site Parameter 2 (default index page) */
public String GetWebParam2() 
{
return (String)Get_Value("WebParam2");
}
/** Set Web Parameter 3.
@param WebParam3 Web Site Parameter 3 (default left - menu) */
public void SetWebParam3 (String WebParam3)
{
if (WebParam3 != null && WebParam3.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam3 = WebParam3.Substring(0,2000);
}
Set_Value ("WebParam3", WebParam3);
}
/** Get Web Parameter 3.
@return Web Site Parameter 3 (default left - menu) */
public String GetWebParam3() 
{
return (String)Get_Value("WebParam3");
}
/** Set Web Parameter 4.
@param WebParam4 Web Site Parameter 4 (default footer left) */
public void SetWebParam4 (String WebParam4)
{
if (WebParam4 != null && WebParam4.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam4 = WebParam4.Substring(0,2000);
}
Set_Value ("WebParam4", WebParam4);
}
/** Get Web Parameter 4.
@return Web Site Parameter 4 (default footer left) */
public String GetWebParam4() 
{
return (String)Get_Value("WebParam4");
}
}

}
