namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_SerNoCtl_No
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_SerNoCtl_No : PO{public X_M_SerNoCtl_No (Context ctx, int M_SerNoCtl_No_ID, Trx trxName) : base (ctx, M_SerNoCtl_No_ID, trxName){/** if (M_SerNoCtl_No_ID == 0){SetCurrentNext (0);SetM_SerNoCtl_ID (0);SetM_SerNoCtl_No_ID (0);} */
}public X_M_SerNoCtl_No (Ctx ctx, int M_SerNoCtl_No_ID, Trx trxName) : base (ctx, M_SerNoCtl_No_ID, trxName){/** if (M_SerNoCtl_No_ID == 0){SetCurrentNext (0);SetM_SerNoCtl_ID (0);SetM_SerNoCtl_No_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtl_No (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtl_No (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_SerNoCtl_No (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_SerNoCtl_No(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27877615678585L;/** Last Updated Timestamp 7/23/2020 7:46:01 AM */
public static long updatedMS = 1595490361796L;/** AD_Table_ID=1000543 */
public static int Table_ID; // =1000543;
/** TableName=M_SerNoCtl_No */
public static String Table_Name="M_SerNoCtl_No";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_SerNoCtl_No[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Current Next.
@param CurrentNext The next number to be used */
public void SetCurrentNext (int CurrentNext){Set_Value ("CurrentNext", CurrentNext);}/** Get Current Next.
@return The next number to be used */
public int GetCurrentNext() {Object ii = Get_Value("CurrentNext");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Serial No Control.
@param M_SerNoCtl_ID Product Serial Number Control */
public void SetM_SerNoCtl_ID (int M_SerNoCtl_ID){if (M_SerNoCtl_ID < 1) throw new ArgumentException ("M_SerNoCtl_ID is mandatory.");Set_ValueNoCheck ("M_SerNoCtl_ID", M_SerNoCtl_ID);}/** Get Serial No Control.
@return Product Serial Number Control */
public int GetM_SerNoCtl_ID() {Object ii = Get_Value("M_SerNoCtl_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Serial No.
@param M_SerNoCtl_No_ID Serial No */
public void SetM_SerNoCtl_No_ID (int M_SerNoCtl_No_ID){if (M_SerNoCtl_No_ID < 1) throw new ArgumentException ("M_SerNoCtl_No_ID is mandatory.");Set_ValueNoCheck ("M_SerNoCtl_No_ID", M_SerNoCtl_No_ID);}/** Get Serial No.
@return Serial No */
public int GetM_SerNoCtl_No_ID() {Object ii = Get_Value("M_SerNoCtl_No_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Prefix.
@param Prefix Prefix before the sequence number */
public void SetPrefix (String Prefix){if (Prefix != null && Prefix.Length > 10){log.Warning("Length > 10 - truncated");Prefix = Prefix.Substring(0,10);}Set_Value ("Prefix", Prefix);}/** Get Prefix.
@return Prefix before the sequence number */
public String GetPrefix() {return (String)Get_Value("Prefix");}/** Set Suffix.
@param Suffix Suffix after the number */
public void SetSuffix (String Suffix){if (Suffix != null && Suffix.Length > 10){log.Warning("Length > 10 - truncated");Suffix = Suffix.Substring(0,10);}Set_Value ("Suffix", Suffix);}/** Get Suffix.
@return Suffix after the number */
public String GetSuffix() {return (String)Get_Value("Suffix");}}
}