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
/** Generated Model for AD_Form
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Form : PO
{
public X_AD_Form (Context ctx, int AD_Form_ID, Trx trxName) : base (ctx, AD_Form_ID, trxName)
{
/** if (AD_Form_ID == 0)
{
SetAD_Form_ID (0);
SetAccessLevel (null);
SetEntityType (null);	// U
SetImageField (null);
SetImagePathField (null);
SetIsBetaFunctionality (false);
SetName (null);
SetProcedureName (null);
SetReportPath (null);
SetSqlQuery (null);
}
 */
}
public X_AD_Form (Ctx ctx, int AD_Form_ID, Trx trxName) : base (ctx, AD_Form_ID, trxName)
{
/** if (AD_Form_ID == 0)
{
SetAD_Form_ID (0);
SetAccessLevel (null);
SetEntityType (null);	// U
SetImageField (null);
SetImagePathField (null);
SetIsBetaFunctionality (false);
SetName (null);
SetProcedureName (null);
SetReportPath (null);
SetSqlQuery (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Form (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Form (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Form (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Form()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27590957907743L;
/** Last Updated Timestamp 6/23/2011 6:06:31 PM */
public static long updatedMS = 1308832590954L;
/** AD_Table_ID=376 */
public static int Table_ID;
 // =376;

/** TableName=AD_Form */
public static String Table_Name="AD_Form";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Form[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Context Area.
@param AD_CtxArea_ID Business Domain Area Terminology */
public void SetAD_CtxArea_ID (int AD_CtxArea_ID)
{
if (AD_CtxArea_ID <= 0) Set_Value ("AD_CtxArea_ID", null);
else
Set_Value ("AD_CtxArea_ID", AD_CtxArea_ID);
}
/** Get Context Area.
@return Business Domain Area Terminology */
public int GetAD_CtxArea_ID() 
{
Object ii = Get_Value("AD_CtxArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID < 1) throw new ArgumentException ("AD_Form_ID is mandatory.");
Set_ValueNoCheck ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AccessLevel AD_Reference_ID=5 */
public static int ACCESSLEVEL_AD_Reference_ID=5;
/** Organization = 1 */
public static String ACCESSLEVEL_Organization = "1";
/** Client only = 2 */
public static String ACCESSLEVEL_ClientOnly = "2";
/** Client+Organization = 3 */
public static String ACCESSLEVEL_ClientPlusOrganization = "3";
/** System only = 4 */
public static String ACCESSLEVEL_SystemOnly = "4";
/** System+Client = 6 */
public static String ACCESSLEVEL_SystemPlusClient = "6";
/** All = 7 */
public static String ACCESSLEVEL_All = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccessLevelValid (String test)
{
return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("6") || test.Equals("7");
}
/** Set Data Access Level.
@param AccessLevel Access Level required */
public void SetAccessLevel (String AccessLevel)
{
if (AccessLevel == null) throw new ArgumentException ("AccessLevel is mandatory");
if (!IsAccessLevelValid(AccessLevel))
throw new ArgumentException ("AccessLevel Invalid value - " + AccessLevel + " - Reference_ID=5 - 1 - 2 - 3 - 4 - 6 - 7");
if (AccessLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccessLevel = AccessLevel.Substring(0,1);
}
Set_Value ("AccessLevel", AccessLevel);
}
/** Get Data Access Level.
@return Access Level required */
public String GetAccessLevel() 
{
return (String)Get_Value("AccessLevel");
}
/** Set Classname.
@param Classname Java Classname */
public void SetClassname (String Classname)
{
if (Classname != null && Classname.Length > 60)
{
log.Warning("Length > 60 - truncated");
Classname = Classname.Substring(0,60);
}
Set_Value ("Classname", Classname);
}
/** Get Classname.
@return Java Classname */
public String GetClassname() 
{
return (String)Get_Value("Classname");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
/** Set Image Field.
@param ImageField Image Field */
public void SetImageField (String ImageField)
{
if (ImageField == null) throw new ArgumentException ("ImageField is mandatory.");
if (ImageField.Length > 100)
{
log.Warning("Length > 100 - truncated");
ImageField = ImageField.Substring(0,100);
}
Set_Value ("ImageField", ImageField);
}
/** Get Image Field.
@return Image Field */
public String GetImageField() 
{
return (String)Get_Value("ImageField");
}
/** Set Image Path Field.
@param ImagePathField Image Path Field */
public void SetImagePathField (String ImagePathField)
{
if (ImagePathField == null) throw new ArgumentException ("ImagePathField is mandatory.");
if (ImagePathField.Length > 100)
{
log.Warning("Length > 100 - truncated");
ImagePathField = ImagePathField.Substring(0,100);
}
Set_Value ("ImagePathField", ImagePathField);
}
/** Get Image Path Field.
@return Image Path Field */
public String GetImagePathField() 
{
return (String)Get_Value("ImagePathField");
}
/** Set IncludeI mage.
@param IncludeImage IncludeI mage */
public void SetIncludeImage (Boolean IncludeImage)
{
Set_Value ("IncludeImage", IncludeImage);
}
/** Get IncludeI mage.
@return IncludeI mage */
public Boolean IsIncludeImage() 
{
Object oo = Get_Value("IncludeImage");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IncludeProcedure.
@param IncludeProcedure IncludeProcedure */
public void SetIncludeProcedure (Boolean IncludeProcedure)
{
Set_Value ("IncludeProcedure", IncludeProcedure);
}
/** Get IncludeProcedure.
@return IncludeProcedure */
public Boolean IsIncludeProcedure() 
{
Object oo = Get_Value("IncludeProcedure");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Beta Functionality.
@param IsBetaFunctionality This functionality is considered Beta */
public void SetIsBetaFunctionality (Boolean IsBetaFunctionality)
{
Set_Value ("IsBetaFunctionality", IsBetaFunctionality);
}
/** Get Beta Functionality.
@return This functionality is considered Beta */
public Boolean IsBetaFunctionality() 
{
Object oo = Get_Value("IsBetaFunctionality");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Direct Print.
@param IsDirectPrint Print without dialog */
public void SetIsDirectPrint (Boolean IsDirectPrint)
{
Set_Value ("IsDirectPrint", IsDirectPrint);
}
/** Get Direct Print.
@return Print without dialog */
public Boolean IsDirectPrint() 
{
Object oo = Get_Value("IsDirectPrint");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Report.
@param IsReport Indicates a Report record */
public void SetIsReport (Boolean IsReport)
{
Set_Value ("IsReport", IsReport);
}
/** Get Report.
@return Indicates a Report record */
public Boolean IsReport() 
{
Object oo = Get_Value("IsReport");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set jsp URL.
@param JSPURL Web URL of the jsp function */
public void SetJSPURL (String JSPURL)
{
if (JSPURL != null && JSPURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
JSPURL = JSPURL.Substring(0,120);
}
Set_Value ("JSPURL", JSPURL);
}
/** Get jsp URL.
@return Web URL of the jsp function */
public String GetJSPURL() 
{
return (String)Get_Value("JSPURL");
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
/** Set Procedure.
@param ProcedureName Name of the Database Procedure */
public void SetProcedureName (String ProcedureName)
{
if (ProcedureName == null) throw new ArgumentException ("ProcedureName is mandatory.");
if (ProcedureName.Length > 100)
{
log.Warning("Length > 100 - truncated");
ProcedureName = ProcedureName.Substring(0,100);
}
Set_Value ("ProcedureName", ProcedureName);
}
/** Get Procedure.
@return Name of the Database Procedure */
public String GetProcedureName() 
{
return (String)Get_Value("ProcedureName");
}
/** Set Report Path.
@param ReportPath Report Path */
public void SetReportPath (String ReportPath)
{
if (ReportPath == null) throw new ArgumentException ("ReportPath is mandatory.");
if (ReportPath.Length > 100)
{
log.Warning("Length > 100 - truncated");
ReportPath = ReportPath.Substring(0,100);
}
Set_Value ("ReportPath", ReportPath);
}
/** Get Report Path.
@return Report Path */
public String GetReportPath() 
{
return (String)Get_Value("ReportPath");
}
/** Set SqlQuery.
@param SqlQuery SqlQuery */
public void SetSqlQuery (String SqlQuery)
{
if (SqlQuery == null) throw new ArgumentException ("SqlQuery is mandatory.");
if (SqlQuery.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
SqlQuery = SqlQuery.Substring(0,2000);
}
Set_Value ("SqlQuery", SqlQuery);
}
/** Get SqlQuery.
@return SqlQuery */
public String GetSqlQuery() 
{
return (String)Get_Value("SqlQuery");
}
}

}
