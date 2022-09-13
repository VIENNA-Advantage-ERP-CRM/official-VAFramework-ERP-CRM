namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_Survey
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Survey : PO{public X_AD_Survey (Context ctx, int AD_Survey_ID, Trx trxName) : base (ctx, AD_Survey_ID, trxName){/** if (AD_Survey_ID == 0){SetAD_Survey_ID (0);} */
}public X_AD_Survey (Ctx ctx, int AD_Survey_ID, Trx trxName) : base (ctx, AD_Survey_ID, trxName){/** if (AD_Survey_ID == 0){SetAD_Survey_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Survey (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Survey (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Survey (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Survey(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27944657231808L;/** Last Updated Timestamp 9/7/2022 6:25:15 AM */
public static long updatedMS = 1662531915019L;/** AD_Table_ID=1000557 */
public static int Table_ID; // =1000557;
/** TableName=AD_Survey */
public static String Table_Name="AD_Survey";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_Survey[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Survey.
@param AD_Survey_ID Survey */
public void SetAD_Survey_ID (int AD_Survey_ID){if (AD_Survey_ID < 1) throw new ArgumentException ("AD_Survey_ID is mandatory.");Set_ValueNoCheck ("AD_Survey_ID", AD_Survey_ID);}/** Get Survey.
@return Survey */
public int GetAD_Survey_ID() {Object ii = Get_Value("AD_Survey_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 250){log.Warning("Length > 250 - truncated");Description = Description.Substring(0,250);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Mandatory.
@param IsMandatory Data is required in this column */
public void SetIsMandatory (Boolean IsMandatory){Set_Value ("IsMandatory", IsMandatory);}/** Get Mandatory.
@return Data is required in this column */
public Boolean IsMandatory() {Object oo = Get_Value("IsMandatory");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name != null && Name.Length > 100){log.Warning("Length > 100 - truncated");Name = Name.Substring(0,100);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}
/** SurveyType AD_Reference_ID=1000259 */
public static int SURVEYTYPE_AD_Reference_ID=1000259;/** Check List = CL */
public static String SURVEYTYPE_CheckList = "CL";/** Questionary = QN */
public static String SURVEYTYPE_Questionary = "QN";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSurveyTypeValid (String test){return test == null || test.Equals("CL") || test.Equals("QN");}/** Set Type.
@param SurveyType This Field is used to  select survey type */
public void SetSurveyType (String SurveyType){if (!IsSurveyTypeValid(SurveyType))
throw new ArgumentException ("SurveyType Invalid value - " + SurveyType + " - Reference_ID=1000259 - CL - QN");if (SurveyType != null && SurveyType.Length > 2){log.Warning("Length > 2 - truncated");SurveyType = SurveyType.Substring(0,2);}Set_Value ("SurveyType", SurveyType);}/** Get Type.
@return This Field is used to  select survey type */
public String GetSurveyType() {return (String)Get_Value("SurveyType");}}
}