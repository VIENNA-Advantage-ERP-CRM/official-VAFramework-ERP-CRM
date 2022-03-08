namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_AlertRuleCondition
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AlertRuleCondition : PO{public X_AD_AlertRuleCondition (Context ctx, int AD_AlertRuleCondition_ID, Trx trxName) : base (ctx, AD_AlertRuleCondition_ID, trxName){/** if (AD_AlertRuleCondition_ID == 0){SetAD_AlertRuleCondition_ID (0);SetAD_AlertRule_ID (0);SetOperator (null);SetReturnValueType (null);SetSequence (0);SetSqlQuery (null);} */
}public X_AD_AlertRuleCondition (Ctx ctx, int AD_AlertRuleCondition_ID, Trx trxName) : base (ctx, AD_AlertRuleCondition_ID, trxName){/** if (AD_AlertRuleCondition_ID == 0){SetAD_AlertRuleCondition_ID (0);SetAD_AlertRule_ID (0);SetOperator (null);SetReturnValueType (null);SetSequence (0);SetSqlQuery (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRuleCondition (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRuleCondition (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRuleCondition (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AlertRuleCondition(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27757102828158L;/** Last Updated Timestamp 9/27/2016 5:28:31 PM */
public static long updatedMS = 1474977511369L;/** AD_Table_ID=1000764 */
public static int Table_ID; // =1000764;
/** TableName=AD_AlertRuleCondition */
public static String Table_Name="AD_AlertRuleCondition";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_AlertRuleCondition[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set AD_AlertRuleCondition_ID.
@param AD_AlertRuleCondition_ID AD_AlertRuleCondition_ID */
public void SetAD_AlertRuleCondition_ID (int AD_AlertRuleCondition_ID){if (AD_AlertRuleCondition_ID < 1) throw new ArgumentException ("AD_AlertRuleCondition_ID is mandatory.");Set_ValueNoCheck ("AD_AlertRuleCondition_ID", AD_AlertRuleCondition_ID);}/** Get AD_AlertRuleCondition_ID.
@return AD_AlertRuleCondition_ID */
public int GetAD_AlertRuleCondition_ID() {Object ii = Get_Value("AD_AlertRuleCondition_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Alert Rule.
@param AD_AlertRule_ID Definition of the alert element */
public void SetAD_AlertRule_ID (int AD_AlertRule_ID){if (AD_AlertRule_ID < 1) throw new ArgumentException ("AD_AlertRule_ID is mandatory.");Set_ValueNoCheck ("AD_AlertRule_ID", AD_AlertRule_ID);}/** Get Alert Rule.
@return Definition of the alert element */
public int GetAD_AlertRule_ID() {Object ii = Get_Value("AD_AlertRule_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Value.
@param AlphaNumValue Value */
public void SetAlphaNumValue (String AlphaNumValue){if (AlphaNumValue != null && AlphaNumValue.Length > 100){log.Warning("Length > 100 - truncated");AlphaNumValue = AlphaNumValue.Substring(0,100);}Set_Value ("AlphaNumValue", AlphaNumValue);}/** Get Value.
@return Value */
public String GetAlphaNumValue() {return (String)Get_Value("AlphaNumValue");}
/** AndOr AD_Reference_ID=204 */
public static int ANDOR_AD_Reference_ID=204;/** And = A */
public static String ANDOR_And = "A";/** Or = O */
public static String ANDOR_Or = "O";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAndOrValid (String test){return test == null || test.Equals("A") || test.Equals("O");}/** Set And/Or.
@param AndOr Logical operation: AND or OR */
public void SetAndOr (String AndOr){if (!IsAndOrValid(AndOr))
throw new ArgumentException ("AndOr Invalid value - " + AndOr + " - Reference_ID=204 - A - O");if (AndOr != null && AndOr.Length > 1){log.Warning("Length > 1 - truncated");AndOr = AndOr.Substring(0,1);}Set_Value ("AndOr", AndOr);}/** Get And/Or.
@return Logical operation: AND or OR */
public String GetAndOr() {return (String)Get_Value("AndOr");}
/** DateOperation AD_Reference_ID=1000331 */
public static int DATEOPERATION_AD_Reference_ID=1000331;/** LastxDays = LD */
public static String DATEOPERATION_LastxDays = "LD";/** LastxMonth = LM */
public static String DATEOPERATION_LastxMonth = "LM";/** LastxYear = LY */
public static String DATEOPERATION_LastxYear = "LY";/** Now = NW */
public static String DATEOPERATION_Now = "NW";/** Today = TO */
public static String DATEOPERATION_Today = "TO";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDateOperationValid (String test){return test == null || test.Equals("LD") || test.Equals("LM") || test.Equals("LY") || test.Equals("NW") || test.Equals("TO");}/** Set Date Operation.
@param DateOperation Date Operation */
public void SetDateOperation (String DateOperation){if (!IsDateOperationValid(DateOperation))
throw new ArgumentException ("DateOperation Invalid value - " + DateOperation + " - Reference_ID=1000331 - LD - LM - LY - NW - TO");if (DateOperation != null && DateOperation.Length > 2){log.Warning("Length > 2 - truncated");DateOperation = DateOperation.Substring(0,2);}Set_Value ("DateOperation", DateOperation);}/** Get Date Operation.
@return Date Operation */
public String GetDateOperation() {return (String)Get_Value("DateOperation");}/** Set Valuation Date.
@param DateValue Date of valuation */
public void SetDateValue (DateTime? DateValue){Set_Value ("DateValue", (DateTime?)DateValue);}/** Get Valuation Date.
@return Date of valuation */
public DateTime? GetDateValue() {return (DateTime?)Get_Value("DateValue");}/** Set Day.
@param Day Day */
public void SetDay (int Day){Set_Value ("Day", Day);}/** Get Day.
@return Day */
public int GetDay() {Object ii = Get_Value("Day");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set IsDynamic.
@param IsDynamic IsDynamic */
public void SetIsDynamic (Boolean IsDynamic){Set_Value ("IsDynamic", IsDynamic);}/** Get IsDynamic.
@return IsDynamic */
public Boolean IsDynamic() {Object oo = Get_Value("IsDynamic");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Month.
@param MONTH Month */
public void SetMONTH (int MONTH){Set_Value ("MONTH", MONTH);}/** Get Month.
@return Month */
public int GetMONTH() {Object ii = Get_Value("MONTH");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Operator AD_Reference_ID=205 */
public static int OPERATOR_AD_Reference_ID=205;/** != = != */
public static String OPERATOR_NotEq = "!=";/** < = << */
public static String OPERATOR_Le = "<<";/** <= = <= */
public static String OPERATOR_LeEq = "<=";/**  = = == */
public static String OPERATOR_Eq = "==";/** >= = >= */
public static String OPERATOR_GtEq = ">=";/** > = >> */
public static String OPERATOR_Gt = ">>";/** |<x>| = AB */
public static String OPERATOR_X = "AB";/** sql = SQ */
public static String OPERATOR_Sql = "SQ";/**  ~ = ~~ */
public static String OPERATOR_Like = "~~";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsOperatorValid (String test){return test.Equals("!=") || test.Equals("<<") || test.Equals("<=") || test.Equals("==") || test.Equals(">=") || test.Equals(">>") || test.Equals("AB") || test.Equals("SQ") || test.Equals("~~");}/** Set Operator.
@param Operator Operator */
public void SetOperator (String Operator){if (Operator == null) throw new ArgumentException ("Operator is mandatory");if (!IsOperatorValid(Operator))
throw new ArgumentException ("Operator Invalid value - " + Operator + " - Reference_ID=205 - != - << - <= - == - >= - >> - AB - SQ - ~~");if (Operator.Length > 2){log.Warning("Length > 2 - truncated");Operator = Operator.Substring(0,2);}Set_Value ("Operator", Operator);}/** Get Operator.
@return Operator */
public String GetOperator() {return (String)Get_Value("Operator");}
/** ReturnValueType AD_Reference_ID=1000330 */
public static int RETURNVALUETYPE_AD_Reference_ID=1000330;/** Date = DT */
public static String RETURNVALUETYPE_Date = "DT";/** Number = NM */
public static String RETURNVALUETYPE_Number = "NM";/** String = ST */
public static String RETURNVALUETYPE_String = "ST";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsReturnValueTypeValid (String test){return test.Equals("DT") || test.Equals("NM") || test.Equals("ST");}/** Set Return Value Type.
@param ReturnValueType Return Value Type */
public void SetReturnValueType (String ReturnValueType){if (ReturnValueType == null) throw new ArgumentException ("ReturnValueType is mandatory");if (!IsReturnValueTypeValid(ReturnValueType))
throw new ArgumentException ("ReturnValueType Invalid value - " + ReturnValueType + " - Reference_ID=1000330 - DT - NM - ST");if (ReturnValueType.Length > 2){log.Warning("Length > 2 - truncated");ReturnValueType = ReturnValueType.Substring(0,2);}Set_Value ("ReturnValueType", ReturnValueType);}/** Get Return Value Type.
@return Return Value Type */
public String GetReturnValueType() {return (String)Get_Value("ReturnValueType");}/** Set Sequence.
@param Sequence Sequence */
public void SetSequence (int Sequence){Set_Value ("Sequence", Sequence);}/** Get Sequence.
@return Sequence */
public int GetSequence() {Object ii = Get_Value("Sequence");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Sql Query.
@param SqlQuery Sql Query */
public void SetSqlQuery (String SqlQuery){if (SqlQuery == null) throw new ArgumentException ("SqlQuery is mandatory.");if (SqlQuery.Length > 2000){log.Warning("Length > 2000 - truncated");SqlQuery = SqlQuery.Substring(0,2000);}Set_Value ("SqlQuery", SqlQuery);}/** Get Sql Query.
@return Sql Query */
public String GetSqlQuery() {return (String)Get_Value("SqlQuery");}/** Set Verify Query.
@param VerifyQuery Verify Query */
public void SetVerifyQuery (String VerifyQuery){if (VerifyQuery != null && VerifyQuery.Length > 2){log.Warning("Length > 2 - truncated");VerifyQuery = VerifyQuery.Substring(0,2);}Set_Value ("VerifyQuery", VerifyQuery);}/** Get Verify Query.
@return Verify Query */
public String GetVerifyQuery() {return (String)Get_Value("VerifyQuery");}/** Set Year.
@param YEAR Year */
public void SetYEAR (int YEAR){Set_Value ("YEAR", YEAR);}/** Get Year.
@return Year */
public int GetYEAR() {Object ii = Get_Value("YEAR");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}