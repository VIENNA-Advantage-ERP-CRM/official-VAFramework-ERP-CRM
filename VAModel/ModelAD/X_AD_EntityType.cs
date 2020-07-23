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
/** Generated Model for AD_EntityType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_EntityType : PO
{
public X_AD_EntityType (Context ctx, int AD_EntityType_ID, Trx trxName) : base (ctx, AD_EntityType_ID, trxName)
{
/** if (AD_EntityType_ID == 0)
{
SetAD_EntityType_ID (0);	// @SQL=SELECT NVL(MAX(AD_EntityType_ID),999999)+1 FROM AD_EntityType WHERE AD_EntityType_ID > 1000
SetEntityType (null);	// U
SetName (null);
}
 */
}
public X_AD_EntityType (Ctx ctx, int AD_EntityType_ID, Trx trxName) : base (ctx, AD_EntityType_ID, trxName)
{
/** if (AD_EntityType_ID == 0)
{
SetAD_EntityType_ID (0);	// @SQL=SELECT NVL(MAX(AD_EntityType_ID),999999)+1 FROM AD_EntityType WHERE AD_EntityType_ID > 1000
SetEntityType (null);	// U
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_EntityType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_EntityType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_EntityType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_EntityType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361201L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044412L;
/** AD_Table_ID=882 */
public static int Table_ID;
 // =882;

/** TableName=AD_EntityType */
public static String Table_Name="AD_EntityType";

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
StringBuilder sb = new StringBuilder ("X_AD_EntityType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Entity Type.
@param AD_EntityType_ID System Entity Type */
public void SetAD_EntityType_ID (int AD_EntityType_ID)
{
if (AD_EntityType_ID < 1) throw new ArgumentException ("AD_EntityType_ID is mandatory.");
Set_ValueNoCheck ("AD_EntityType_ID", AD_EntityType_ID);
}
/** Get Entity Type.
@return System Entity Type */
public int GetAD_EntityType_ID() 
{
Object ii = Get_Value("AD_EntityType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BinaryData.
@param BinaryData Binary Data */
public void SetBinaryData (Byte[] BinaryData)
{
Set_Value ("BinaryData", BinaryData);
}
/** Get BinaryData.
@return Binary Data */
public Byte[] GetBinaryData() 
{
return (Byte[])Get_Value("BinaryData");
}
/** Set Classpath.
@param Classpath Extension Classpath */
public void SetClasspath (String Classpath)
{
if (Classpath != null && Classpath.Length > 255)
{
log.Warning("Length > 255 - truncated");
Classpath = Classpath.Substring(0,255);
}
Set_Value ("Classpath", Classpath);
}
/** Get Classpath.
@return Extension Classpath */
public String GetClasspath() 
{
return (String)Get_Value("Classpath");
}
/** Set Create Component.
@param CreateComponent Create Component */
public void SetCreateComponent (String CreateComponent)
{
if (CreateComponent != null && CreateComponent.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateComponent = CreateComponent.Substring(0,1);
}
Set_Value ("CreateComponent", CreateComponent);
}
/** Get Create Component.
@return Create Component */
public String GetCreateComponent() 
{
return (String)Get_Value("CreateComponent");
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
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType == null) throw new ArgumentException ("EntityType is mandatory.");
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_ValueNoCheck ("EntityType", EntityType);
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
/** Set License Text.
@param LicenseText Text of the License of the Component */
public void SetLicenseText (String LicenseText)
{
Set_Value ("LicenseText", LicenseText);
}
/** Get License Text.
@return Text of the License of the Component */
public String GetLicenseText() 
{
return (String)Get_Value("LicenseText");
}
/** Set ModelPackage.
@param ModelPackage Java Package of the model classes */
public void SetModelPackage (String ModelPackage)
{
if (ModelPackage != null && ModelPackage.Length > 255)
{
log.Warning("Length > 255 - truncated");
ModelPackage = ModelPackage.Substring(0,255);
}
Set_Value ("ModelPackage", ModelPackage);
}
/** Get ModelPackage.
@return Java Package of the model classes */
public String GetModelPackage() 
{
return (String)Get_Value("ModelPackage");
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_ValueNoCheck ("Record_ID", null);
else
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Summary.
@param Summary Textual summary of this request */
public void SetSummary (String Summary)
{
if (Summary != null && Summary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Summary = Summary.Substring(0,2000);
}
Set_Value ("Summary", Summary);
}
/** Get Summary.
@return Textual summary of this request */
public String GetSummary() 
{
return (String)Get_Value("Summary");
}
/** Set Version.
@param Version Version of the table definition */
public void SetVersion (String Version)
{
if (Version != null && Version.Length > 20)
{
log.Warning("Length > 20 - truncated");
Version = Version.Substring(0,20);
}
Set_Value ("Version", Version);
}
/** Get Version.
@return Version of the table definition */
public String GetVersion() 
{
return (String)Get_Value("Version");
}
}

}
