namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_CardView_Column
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_CardView_Column : PO{public X_AD_CardView_Column (Context ctx, int AD_CardView_Column_ID, Trx trxName) : base (ctx, AD_CardView_Column_ID, trxName){/** if (AD_CardView_Column_ID == 0){SetAD_CardView_Column_ID (0);SetAD_CardView_ID (0);} */
}public X_AD_CardView_Column (Ctx ctx, int AD_CardView_Column_ID, Trx trxName) : base (ctx, AD_CardView_Column_ID, trxName){/** if (AD_CardView_Column_ID == 0){SetAD_CardView_Column_ID (0);SetAD_CardView_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CardView_Column (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CardView_Column (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CardView_Column (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_CardView_Column(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27719840950449L;/** Last Updated Timestamp 7/24/2015 10:57:13 AM */
public static long updatedMS = 1437715633660L;/** AD_Table_ID=1000564 */
public static int Table_ID; // =1000564;
/** TableName=AD_CardView_Column */
public static String Table_Name="AD_CardView_Column";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_CardView_Column[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set AD_CardView_Column_ID.
@param AD_CardView_Column_ID AD_CardView_Column_ID */
public void SetAD_CardView_Column_ID (int AD_CardView_Column_ID){if (AD_CardView_Column_ID < 1) throw new ArgumentException ("AD_CardView_Column_ID is mandatory.");Set_ValueNoCheck ("AD_CardView_Column_ID", AD_CardView_Column_ID);}/** Get AD_CardView_Column_ID.
@return AD_CardView_Column_ID */
public int GetAD_CardView_Column_ID() {Object ii = Get_Value("AD_CardView_Column_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set AD_CardView_ID.
@param AD_CardView_ID AD_CardView_ID */
public void SetAD_CardView_ID (int AD_CardView_ID){if (AD_CardView_ID < 1) throw new ArgumentException ("AD_CardView_ID is mandatory.");Set_ValueNoCheck ("AD_CardView_ID", AD_CardView_ID);}/** Get AD_CardView_ID.
@return AD_CardView_ID */
public int GetAD_CardView_ID() {Object ii = Get_Value("AD_CardView_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Field.
@param AD_Field_ID Field on a tab in a window */
public void SetAD_Field_ID (int AD_Field_ID){if (AD_Field_ID <= 0) Set_Value ("AD_Field_ID", null);else
Set_Value ("AD_Field_ID", AD_Field_ID);}/** Get Field.
@return Field on a tab in a window */
public int GetAD_Field_ID() {Object ii = Get_Value("AD_Field_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
public void SetSeqNo (int SeqNo){Set_Value ("SeqNo", SeqNo);}/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
public int GetSeqNo() {Object ii = Get_Value("SeqNo");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}