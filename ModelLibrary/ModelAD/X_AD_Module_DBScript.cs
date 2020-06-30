namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_Module_DBScript
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Module_DBScript : PO{public X_AD_Module_DBScript (Context ctx, int AD_Module_DBScript_ID, Trx trxName) : base (ctx, AD_Module_DBScript_ID, trxName){/** if (AD_Module_DBScript_ID == 0){SetAD_ModuleInfo_ID (0);SetAD_Module_DBScript_ID (0);} */
}public X_AD_Module_DBScript (Ctx ctx, int AD_Module_DBScript_ID, Trx trxName) : base (ctx, AD_Module_DBScript_ID, trxName){/** if (AD_Module_DBScript_ID == 0){SetAD_ModuleInfo_ID (0);SetAD_Module_DBScript_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module_DBScript (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module_DBScript (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module_DBScript (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Module_DBScript(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27874610760145L;/** Last Updated Timestamp 6/18/2020 1:04:03 PM */
public static long updatedMS = 1592485443356L;/** AD_Table_ID=1000361 */
public static int Table_ID; // =1000361;
/** TableName=AD_Module_DBScript */
public static String Table_Name="AD_Module_DBScript";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_Module_DBScript[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set ModuleInfo.
@param AD_ModuleInfo_ID ModuleInfo */
public void SetAD_ModuleInfo_ID (int AD_ModuleInfo_ID){if (AD_ModuleInfo_ID < 1) throw new ArgumentException ("AD_ModuleInfo_ID is mandatory.");Set_ValueNoCheck ("AD_ModuleInfo_ID", AD_ModuleInfo_ID);}/** Get ModuleInfo.
@return ModuleInfo */
public int GetAD_ModuleInfo_ID() {Object ii = Get_Value("AD_ModuleInfo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Module DBScript.
@param AD_Module_DBScript_ID Module DBScript */
public void SetAD_Module_DBScript_ID (int AD_Module_DBScript_ID){if (AD_Module_DBScript_ID < 1) throw new ArgumentException ("AD_Module_DBScript_ID is mandatory.");Set_ValueNoCheck ("AD_Module_DBScript_ID", AD_Module_DBScript_ID);}/** Get Module DBScript.
@return Module DBScript */
public int GetAD_Module_DBScript_ID() {Object ii = Get_Value("AD_Module_DBScript_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** DBType AD_Reference_ID=1000008 */
public static int DBTYPE_AD_Reference_ID=1000008;/** Oracle = 1 */
public static String DBTYPE_Oracle = "1";/** Postgre SQL = 2 */
public static String DBTYPE_PostgreSQL = "2";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDBTypeValid (String test){return test == null || test.Equals("1") || test.Equals("2");}/** Set Database Type.
@param DBType Database Type */
public void SetDBType (String DBType){if (!IsDBTypeValid(DBType))
throw new ArgumentException ("DBType Invalid value - " + DBType + " - Reference_ID=1000008 - 1 - 2");if (DBType != null && DBType.Length > 1){log.Warning("Length > 1 - truncated");DBType = DBType.Substring(0,1);}Set_Value ("DBType", DBType);}/** Get Database Type.
@return Database Type */
public String GetDBType() {return (String)Get_Value("DBType");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Pre Execute Script.
@param IsPreExecuteScript Pre Execute Script */
public void SetIsPreExecuteScript (Boolean IsPreExecuteScript){Set_Value ("IsPreExecuteScript", IsPreExecuteScript);}/** Get Pre Execute Script.
@return Pre Execute Script */
public Boolean IsPreExecuteScript() {Object oo = Get_Value("IsPreExecuteScript");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Script.
@param Script Dynamic Java Language Script to calculate result */
public void SetScript (String Script){Set_Value ("Script", Script);}/** Get Script.
@return Dynamic Java Language Script to calculate result */
public String GetScript() {return (String)Get_Value("Script");}}
}