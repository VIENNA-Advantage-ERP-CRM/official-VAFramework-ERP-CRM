namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_SysConfig
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_SysConfig : PO{public X_AD_SysConfig (Context ctx, int AD_SysConfig_ID, Trx trxName) : base (ctx, AD_SysConfig_ID, trxName){/** if (AD_SysConfig_ID == 0){SetAD_SysConfig_ID (0);} */
}public X_AD_SysConfig (Ctx ctx, int AD_SysConfig_ID, Trx trxName) : base (ctx, AD_SysConfig_ID, trxName){/** if (AD_SysConfig_ID == 0){SetAD_SysConfig_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SysConfig (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SysConfig (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SysConfig (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_SysConfig(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27894390433489L;/** Last Updated Timestamp 2/2/2021 11:25:16 AM */
public static long updatedMS = 1612265116700L;/** AD_Table_ID=1000664 */
public static int Table_ID; // =1000664;
/** TableName=AD_SysConfig */
public static String Table_Name="AD_SysConfig";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(4);/** AccessLevel
@return 4 - System 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_SysConfig[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set System Config.
@param AD_SysConfig_ID System Config */
public void SetAD_SysConfig_ID (int AD_SysConfig_ID){if (AD_SysConfig_ID < 1) throw new ArgumentException ("AD_SysConfig_ID is mandatory.");Set_ValueNoCheck ("AD_SysConfig_ID", AD_SysConfig_ID);}/** Get System Config.
@return System Config */
public int GetAD_SysConfig_ID() {Object ii = Get_Value("AD_SysConfig_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** ConfigurationLevel AD_Reference_ID=1000258 */
public static int CONFIGURATIONLEVEL_AD_Reference_ID=1000258;/** Client = C */
public static String CONFIGURATIONLEVEL_Client = "C";/** Organization = O */
public static String CONFIGURATIONLEVEL_Organization = "O";/** System = S */
public static String CONFIGURATIONLEVEL_System = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfigurationLevelValid (String test){return test == null || test.Equals("C") || test.Equals("O") || test.Equals("S");}/** Set Configuration Level.
@param ConfigurationLevel Configuration Level */
public void SetConfigurationLevel (String ConfigurationLevel){if (!IsConfigurationLevelValid(ConfigurationLevel))
throw new ArgumentException ("ConfigurationLevel Invalid value - " + ConfigurationLevel + " - Reference_ID=1000258 - C - O - S");if (ConfigurationLevel != null && ConfigurationLevel.Length > 1){log.Warning("Length > 1 - truncated");ConfigurationLevel = ConfigurationLevel.Substring(0,1);}Set_Value ("ConfigurationLevel", ConfigurationLevel);}/** Get Configuration Level.
@return Configuration Level */
public String GetConfigurationLevel() {return (String)Get_Value("ConfigurationLevel");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}
/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;/** Set Entity Type.
@param EntityType Dictionary Entity Type; Determines ownership and synchronization */
public void SetEntityType (String EntityType){if (EntityType != null && EntityType.Length > 40){log.Warning("Length > 40 - truncated");EntityType = EntityType.Substring(0,40);}Set_Value ("EntityType", EntityType);}/** Get Entity Type.
@return Dictionary Entity Type; Determines ownership and synchronization */
public String GetEntityType() {return (String)Get_Value("EntityType");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name != null && Name.Length > 50){log.Warning("Length > 50 - truncated");Name = Name.Substring(0,50);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value){if (Value != null && Value.Length > 150){log.Warning("Length > 150 - truncated");Value = Value.Substring(0,150);}Set_Value ("Value", Value);}/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() {return (String)Get_Value("Value");}}
}