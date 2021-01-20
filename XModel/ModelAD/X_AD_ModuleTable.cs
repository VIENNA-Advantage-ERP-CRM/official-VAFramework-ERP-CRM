namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAF_ModuleTable
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleTable : PO{public X_VAF_ModuleTable (Context ctx, int VAF_ModuleTable_ID, Trx trxName) : base (ctx, VAF_ModuleTable_ID, trxName){/** if (VAF_ModuleTable_ID == 0){SetVAF_ModuleInfo_ID (0);SetVAF_ModuleTable_ID (0);} */
}public X_VAF_ModuleTable (Ctx ctx, int VAF_ModuleTable_ID, Trx trxName) : base (ctx, VAF_ModuleTable_ID, trxName){/** if (VAF_ModuleTable_ID == 0){SetVAF_ModuleInfo_ID (0);SetVAF_ModuleTable_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleTable(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27780777866118L;/** Last Updated Timestamp 6/28/2017 5:52:29 PM */
public static long updatedMS = 1498652549329L;/** VAF_TableView_ID=1000357 */
public static int Table_ID; // =1000357;
/** TableName=VAF_ModuleTable */
public static String Table_Name="VAF_ModuleTable";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAF_ModuleTable[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Module.
@param VAF_ModuleInfo_ID Module */
public void SetVAF_ModuleInfo_ID (int VAF_ModuleInfo_ID){if (VAF_ModuleInfo_ID < 1) throw new ArgumentException ("VAF_ModuleInfo_ID is mandatory.");Set_ValueNoCheck ("VAF_ModuleInfo_ID", VAF_ModuleInfo_ID);}/** Get Module.
@return Module */
public int GetVAF_ModuleInfo_ID() {Object ii = Get_Value("VAF_ModuleInfo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Module Table.
@param VAF_ModuleTable_ID Module Table */
public void SetVAF_ModuleTable_ID (int VAF_ModuleTable_ID){if (VAF_ModuleTable_ID < 1) throw new ArgumentException ("VAF_ModuleTable_ID is mandatory.");Set_ValueNoCheck ("VAF_ModuleTable_ID", VAF_ModuleTable_ID);}/** Get Module Table.
@return Module Table */
public int GetVAF_ModuleTable_ID() {Object ii = Get_Value("VAF_ModuleTable_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID){if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);}/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() {Object ii = Get_Value("VAF_TableView_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}}
}