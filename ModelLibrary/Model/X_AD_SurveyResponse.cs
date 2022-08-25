namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_SurveyResponse
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_SurveyResponse : PO{public X_AD_SurveyResponse (Context ctx, int AD_SurveyResponse_ID, Trx trxName) : base (ctx, AD_SurveyResponse_ID, trxName){/** if (AD_SurveyResponse_ID == 0){SetAD_SurveyResponse_ID (0);} */
}public X_AD_SurveyResponse (Ctx ctx, int AD_SurveyResponse_ID, Trx trxName) : base (ctx, AD_SurveyResponse_ID, trxName){/** if (AD_SurveyResponse_ID == 0){SetAD_SurveyResponse_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponse (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponse (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponse (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_SurveyResponse(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27942769746052L;/** Last Updated Timestamp 8/16/2022 3:37:09 PM */
public static long updatedMS = 1660644429263L;/** AD_Table_ID=1001791 */
public static int Table_ID; // =1001791;
/** TableName=AD_SurveyResponse */
public static String Table_Name="AD_SurveyResponse";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_SurveyResponse[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Survey Response.
@param AD_SurveyResponse_ID Survey Response */
public void SetAD_SurveyResponse_ID (int AD_SurveyResponse_ID){if (AD_SurveyResponse_ID < 1) throw new ArgumentException ("AD_SurveyResponse_ID is mandatory.");Set_ValueNoCheck ("AD_SurveyResponse_ID", AD_SurveyResponse_ID);}/** Get Survey Response.
@return Survey Response */
public int GetAD_SurveyResponse_ID() {Object ii = Get_Value("AD_SurveyResponse_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Survey.
@param AD_Survey_ID Survey */
public void SetAD_Survey_ID (int AD_Survey_ID){if (AD_Survey_ID <= 0) Set_Value ("AD_Survey_ID", null);else
Set_Value ("AD_Survey_ID", AD_Survey_ID);}/** Get Survey.
@return Survey */
public int GetAD_Survey_ID() {Object ii = Get_Value("AD_Survey_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Table/View.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID){if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);else
Set_Value ("AD_Table_ID", AD_Table_ID);}/** Get Table/View.
@return Database Table information */
public int GetAD_Table_ID() {Object ii = Get_Value("AD_Table_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID){if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);else
Set_Value ("AD_User_ID", AD_User_ID);}/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() {Object ii = Get_Value("AD_User_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID){if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);else
Set_Value ("AD_Window_ID", AD_Window_ID);}/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() {Object ii = Get_Value("AD_Window_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID){if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);else
Set_Value ("C_DocType_ID", C_DocType_ID);}/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() {Object ii = Get_Value("C_DocType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Record .
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID){if (Record_ID <= 0) Set_Value ("Record_ID", null);else
Set_Value ("Record_ID", Record_ID);}/** Get Record .
@return Direct internal record ID */
public int GetRecord_ID() {Object ii = Get_Value("Record_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}