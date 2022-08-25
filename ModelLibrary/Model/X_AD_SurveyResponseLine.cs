namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_SurveyResponseLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_SurveyResponseLine : PO{public X_AD_SurveyResponseLine (Context ctx, int AD_SurveyResponseLine_ID, Trx trxName) : base (ctx, AD_SurveyResponseLine_ID, trxName){/** if (AD_SurveyResponseLine_ID == 0){SetAD_SurveyItem_ID (0);SetAD_SurveyResponseLine_ID (0);SetAD_SurveyResponse_ID (0);} */
}public X_AD_SurveyResponseLine (Ctx ctx, int AD_SurveyResponseLine_ID, Trx trxName) : base (ctx, AD_SurveyResponseLine_ID, trxName){/** if (AD_SurveyResponseLine_ID == 0){SetAD_SurveyItem_ID (0);SetAD_SurveyResponseLine_ID (0);SetAD_SurveyResponse_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponseLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponseLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SurveyResponseLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_SurveyResponseLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27942769760477L;/** Last Updated Timestamp 8/16/2022 3:37:23 PM */
public static long updatedMS = 1660644443688L;/** AD_Table_ID=1001792 */
public static int Table_ID; // =1001792;
/** TableName=AD_SurveyResponseLine */
public static String Table_Name="AD_SurveyResponseLine";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_SurveyResponseLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Questionary.
@param AD_SurveyItem_ID Questionary */
public void SetAD_SurveyItem_ID (int AD_SurveyItem_ID){if (AD_SurveyItem_ID < 1) throw new ArgumentException ("AD_SurveyItem_ID is mandatory.");Set_ValueNoCheck ("AD_SurveyItem_ID", AD_SurveyItem_ID);}/** Get Questionary.
@return Questionary */
public int GetAD_SurveyItem_ID() {Object ii = Get_Value("AD_SurveyItem_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Survey Response Line.
@param AD_SurveyResponseLine_ID Survey Response Line */
public void SetAD_SurveyResponseLine_ID (int AD_SurveyResponseLine_ID){if (AD_SurveyResponseLine_ID < 1) throw new ArgumentException ("AD_SurveyResponseLine_ID is mandatory.");Set_ValueNoCheck ("AD_SurveyResponseLine_ID", AD_SurveyResponseLine_ID);}/** Get Survey Response Line.
@return Survey Response Line */
public int GetAD_SurveyResponseLine_ID() {Object ii = Get_Value("AD_SurveyResponseLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Survey Response.
@param AD_SurveyResponse_ID Survey Response */
public void SetAD_SurveyResponse_ID (int AD_SurveyResponse_ID){if (AD_SurveyResponse_ID < 1) throw new ArgumentException ("AD_SurveyResponse_ID is mandatory.");Set_ValueNoCheck ("AD_SurveyResponse_ID", AD_SurveyResponse_ID);}/** Get Survey Response.
@return Survey Response */
public int GetAD_SurveyResponse_ID() {Object ii = Get_Value("AD_SurveyResponse_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Answer.
@param AD_SurveyValue_ID Answer */
public void SetAD_SurveyValue_ID (String AD_SurveyValue_ID){if (AD_SurveyValue_ID != null && AD_SurveyValue_ID.Length > 500){log.Warning("Length > 500 - truncated");AD_SurveyValue_ID = AD_SurveyValue_ID.Substring(0,500);}Set_Value ("AD_SurveyValue_ID", AD_SurveyValue_ID);}/** Get Answer.
@return Answer */
public String GetAD_SurveyValue_ID() {return (String)Get_Value("AD_SurveyValue_ID");}/** Set Answer.
@param Answer Answer field is used for Answer of Question */
public void SetAnswer (String Answer){if (Answer != null && Answer.Length > 250){log.Warning("Length > 250 - truncated");Answer = Answer.Substring(0,250);}Set_Value ("Answer", Answer);}/** Get Answer.
@return Answer field is used for Answer of Question */
public String GetAnswer() {return (String)Get_Value("Answer");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Question.
@param Question Question is a collection of multiple Question */
public void SetQuestion (String Question){if (Question != null && Question.Length > 250){log.Warning("Length > 250 - truncated");Question = Question.Substring(0,250);}Set_Value ("Question", Question);}/** Get Question.
@return Question is a collection of multiple Question */
public String GetQuestion() {return (String)Get_Value("Question");}}
}