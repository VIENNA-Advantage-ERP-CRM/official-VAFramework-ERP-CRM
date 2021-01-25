namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for GL_AssignAcctSchema
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_AssignAcctSchema : PO{public X_GL_AssignAcctSchema (Context ctx, int GL_AssignAcctSchema_ID, Trx trxName) : base (ctx, GL_AssignAcctSchema_ID, trxName){/** if (GL_AssignAcctSchema_ID == 0){SetGL_AssignAcctSchema_ID (0);SetGL_Journal_ID (0);} */
}public X_GL_AssignAcctSchema (Ctx ctx, int GL_AssignAcctSchema_ID, Trx trxName) : base (ctx, GL_AssignAcctSchema_ID, trxName){/** if (GL_AssignAcctSchema_ID == 0){SetGL_AssignAcctSchema_ID (0);SetGL_Journal_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_AssignAcctSchema (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_AssignAcctSchema (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_AssignAcctSchema (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_AssignAcctSchema(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27853764428601L;/** Last Updated Timestamp 10/21/2019 11:55:11 AM */
public static long updatedMS = 1571639111812L;/** VAF_TableView_ID=1001269 */
public static int Table_ID; // =1001269;
/** TableName=GL_AssignAcctSchema */
public static String Table_Name="GL_AssignAcctSchema";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_GL_AssignAcctSchema[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID){if (VAB_AccountBook_ID <= 0) Set_Value ("VAB_AccountBook_ID", null);else
Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() {Object ii = Get_Value("VAB_AccountBook_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency Type.
@param VAB_CurrencyType_ID Currency Conversion Rate Type */
public void SetVAB_CurrencyType_ID (int VAB_CurrencyType_ID){if (VAB_CurrencyType_ID <= 0) Set_Value ("VAB_CurrencyType_ID", null);else
Set_Value ("VAB_CurrencyType_ID", VAB_CurrencyType_ID);}/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetVAB_CurrencyType_ID() {Object ii = Get_Value("VAB_CurrencyType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID){if (VAB_Currency_ID <= 0) Set_Value ("VAB_Currency_ID", null);else
Set_Value ("VAB_Currency_ID", VAB_Currency_ID);}/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() {Object ii = Get_Value("VAB_Currency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Rate.
@param CurrencyRate Currency Conversion Rate */
public void SetCurrencyRate (Decimal? CurrencyRate){Set_Value ("CurrencyRate", (Decimal?)CurrencyRate);}/** Get Rate.
@return Currency Conversion Rate */
public Decimal GetCurrencyRate() {Object bd =Get_Value("CurrencyRate");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Assigned Accounting Schema.
@param GL_AssignAcctSchema_ID Assigned Accounting Schema */
public void SetGL_AssignAcctSchema_ID (int GL_AssignAcctSchema_ID){if (GL_AssignAcctSchema_ID < 1) throw new ArgumentException ("GL_AssignAcctSchema_ID is mandatory.");Set_ValueNoCheck ("GL_AssignAcctSchema_ID", GL_AssignAcctSchema_ID);}/** Get Assigned Accounting Schema.
@return Assigned Accounting Schema */
public int GetGL_AssignAcctSchema_ID() {Object ii = Get_Value("GL_AssignAcctSchema_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
public void SetGL_Journal_ID (int GL_Journal_ID){if (GL_Journal_ID < 1) throw new ArgumentException ("GL_Journal_ID is mandatory.");Set_ValueNoCheck ("GL_Journal_ID", GL_Journal_ID);}/** Get Journal.
@return General Ledger Journal */
public int GetGL_Journal_ID() {Object ii = Get_Value("GL_Journal_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}