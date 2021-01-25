namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAB_DimAmt
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_DimAmt : PO{public X_VAB_DimAmt (Context ctx, int VAB_DimAmt_ID, Trx trxName) : base (ctx, VAB_DimAmt_ID, trxName){/** if (VAB_DimAmt_ID == 0){SetAmount (0.0);SetVAB_DimAmt_ID (0);} */
}public X_VAB_DimAmt (Ctx ctx, int VAB_DimAmt_ID, Trx trxName) : base (ctx, VAB_DimAmt_ID, trxName){/** if (VAB_DimAmt_ID == 0){SetAmount (0.0);SetVAB_DimAmt_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmt (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmt (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DimAmt (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_DimAmt(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27754850704633L;/** Last Updated Timestamp 9/1/2016 3:53:07 PM */
public static long updatedMS = 1472725387844L;/** VAF_TableView_ID=1000759 */
public static int Table_ID; // =1000759;
/** TableName=VAB_DimAmt */
public static String Table_Name="VAB_DimAmt";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAB_DimAmt[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID){if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);}/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() {Object ii = Get_Value("VAF_TableView_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount){if (Amount == null) throw new ArgumentException ("Amount is mandatory.");Set_Value ("Amount", (Decimal?)Amount);}/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() {Object bd =Get_Value("Amount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set VAB_DimAmt_ID.
@param VAB_DimAmt_ID VAB_DimAmt_ID */
public void SetVAB_DimAmt_ID (int VAB_DimAmt_ID){if (VAB_DimAmt_ID < 1) throw new ArgumentException ("VAB_DimAmt_ID is mandatory.");Set_ValueNoCheck ("VAB_DimAmt_ID", VAB_DimAmt_ID);}/** Get VAB_DimAmt_ID.
@return VAB_DimAmt_ID */
public int GetVAB_DimAmt_ID() {Object ii = Get_Value("VAB_DimAmt_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID){if (Record_ID <= 0) Set_Value ("Record_ID", null);else
Set_Value ("Record_ID", Record_ID);}/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() {Object ii = Get_Value("Record_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}