namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_ModuleTable
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleTable : PO{public X_AD_ModuleTable (Context ctx, int AD_ModuleTable_ID, Trx trxName) : base (ctx, AD_ModuleTable_ID, trxName){/** if (AD_ModuleTable_ID == 0){SetAD_ModuleInfo_ID (0);SetAD_ModuleTable_ID (0);} */
}public X_AD_ModuleTable (Ctx ctx, int AD_ModuleTable_ID, Trx trxName) : base (ctx, AD_ModuleTable_ID, trxName){/** if (AD_ModuleTable_ID == 0){SetAD_ModuleInfo_ID (0);SetAD_ModuleTable_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleTable(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27780777866118L;/** Last Updated Timestamp 6/28/2017 5:52:29 PM */
public static long updatedMS = 1498652549329L;/** AD_Table_ID=1000357 */
public static int Table_ID; // =1000357;
/** TableName=AD_ModuleTable */
public static String Table_Name="AD_ModuleTable";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_ModuleTable[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Module.
@param AD_ModuleInfo_ID Module */
public void SetAD_ModuleInfo_ID (int AD_ModuleInfo_ID){if (AD_ModuleInfo_ID < 1) throw new ArgumentException ("AD_ModuleInfo_ID is mandatory.");Set_ValueNoCheck ("AD_ModuleInfo_ID", AD_ModuleInfo_ID);}/** Get Module.
@return Module */
public int GetAD_ModuleInfo_ID() {Object ii = Get_Value("AD_ModuleInfo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Module Table.
@param AD_ModuleTable_ID Module Table */
public void SetAD_ModuleTable_ID (int AD_ModuleTable_ID){if (AD_ModuleTable_ID < 1) throw new ArgumentException ("AD_ModuleTable_ID is mandatory.");Set_ValueNoCheck ("AD_ModuleTable_ID", AD_ModuleTable_ID);}/** Get Module Table.
@return Module Table */
public int GetAD_ModuleTable_ID() {Object ii = Get_Value("AD_ModuleTable_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID){if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);else
Set_Value ("AD_Table_ID", AD_Table_ID);}/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() {Object ii = Get_Value("AD_Table_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}}
}