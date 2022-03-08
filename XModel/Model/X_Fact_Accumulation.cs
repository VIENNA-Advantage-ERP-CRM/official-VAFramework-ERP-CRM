namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for Fact_Accumulation
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_Fact_Accumulation : PO{public X_Fact_Accumulation (Context ctx, int Fact_Accumulation_ID, Trx trxName) : base (ctx, Fact_Accumulation_ID, trxName){/** if (Fact_Accumulation_ID == 0){SetFACT_ACCUMULATION_ID (0);} */
}public X_Fact_Accumulation (Ctx ctx, int Fact_Accumulation_ID, Trx trxName) : base (ctx, Fact_Accumulation_ID, trxName){/** if (Fact_Accumulation_ID == 0){SetFACT_ACCUMULATION_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Accumulation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Accumulation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Accumulation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_Fact_Accumulation(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27741383602286L;/** Last Updated Timestamp 3/29/2016 7:01:27 PM */
public static long updatedMS = 1459258285497L;/** AD_Table_ID=1000178 */
public static int Table_ID; // =1000178;
/** TableName=Fact_Accumulation */
public static String Table_Name="Fact_Accumulation";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(2);/** AccessLevel
@return 2 - Client 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_Fact_Accumulation[").Append(Get_ID()).Append("]");return sb.ToString();}
/** BALANCEACCUMULATION AD_Reference_ID=1000073 */
public static int BALANCEACCUMULATION_AD_Reference_ID=1000073;/** Daily = D */
public static String BALANCEACCUMULATION_Daily = "D";/** Calendar Month = M */
public static String BALANCEACCUMULATION_CalendarMonth = "M";/** Period of VA Calendar = P */
public static String BALANCEACCUMULATION_PeriodOfAViennaCalendar = "P";/** Calendar Week = W */
public static String BALANCEACCUMULATION_CalendarWeek = "W";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsBALANCEACCUMULATIONValid (String test){return test == null || test.Equals("D") || test.Equals("M") || test.Equals("P") || test.Equals("W");}/** Set BALANCEACCUMULATION.
@param BALANCEACCUMULATION BALANCEACCUMULATION */
public void SetBALANCEACCUMULATION (String BALANCEACCUMULATION){if (!IsBALANCEACCUMULATIONValid(BALANCEACCUMULATION))
throw new ArgumentException ("BALANCEACCUMULATION Invalid value - " + BALANCEACCUMULATION + " - Reference_ID=1000073 - D - M - P - W");if (BALANCEACCUMULATION != null && BALANCEACCUMULATION.Length > 1){log.Warning("Length > 1 - truncated");BALANCEACCUMULATION = BALANCEACCUMULATION.Substring(0,1);}Set_Value ("BALANCEACCUMULATION", BALANCEACCUMULATION);}/** Get BALANCEACCUMULATION.
@return BALANCEACCUMULATION */
public String GetBALANCEACCUMULATION() {return (String)Get_Value("BALANCEACCUMULATION");}/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID){if (C_AcctSchema_ID <= 0) Set_Value ("C_AcctSchema_ID", null);else
Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() {Object ii = Get_Value("C_AcctSchema_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Calendar.
@param C_Calendar_ID Accounting Calendar Name */
public void SetC_Calendar_ID (int C_Calendar_ID){if (C_Calendar_ID <= 0) Set_Value ("C_Calendar_ID", null);else
Set_Value ("C_Calendar_ID", C_Calendar_ID);}/** Get Calendar.
@return Accounting Calendar Name */
public int GetC_Calendar_ID() {Object ii = Get_Value("C_Calendar_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Date From.
@param DateFrom Starting date for a range */
public void SetDateFrom (DateTime? DateFrom){Set_Value ("DateFrom", (DateTime?)DateFrom);}/** Get Date From.
@return Starting date for a range */
public DateTime? GetDateFrom() {return (DateTime?)Get_Value("DateFrom");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set FACT_ACCUMULATION_ID.
@param FACT_ACCUMULATION_ID FACT_ACCUMULATION_ID */
public void SetFACT_ACCUMULATION_ID (int FACT_ACCUMULATION_ID){if (FACT_ACCUMULATION_ID < 1) throw new ArgumentException ("FACT_ACCUMULATION_ID is mandatory.");Set_ValueNoCheck ("FACT_ACCUMULATION_ID", FACT_ACCUMULATION_ID);}/** Get FACT_ACCUMULATION_ID.
@return FACT_ACCUMULATION_ID */
public int GetFACT_ACCUMULATION_ID() {Object ii = Get_Value("FACT_ACCUMULATION_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set ISACTIVITY.
@param ISACTIVITY ISACTIVITY */
public void SetISACTIVITY (Boolean ISACTIVITY){Set_Value ("ISACTIVITY", ISACTIVITY);}/** Get ISACTIVITY.
@return ISACTIVITY */
public Boolean IsACTIVITY() {Object oo = Get_Value("ISACTIVITY");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISBUDGET.
@param ISBUDGET ISBUDGET */
public void SetISBUDGET (Boolean ISBUDGET){Set_Value ("ISBUDGET", ISBUDGET);}/** Get ISBUDGET.
@return ISBUDGET */
public Boolean IsBUDGET() {Object oo = Get_Value("ISBUDGET");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISBUSINESSPARTNER.
@param ISBUSINESSPARTNER ISBUSINESSPARTNER */
public void SetISBUSINESSPARTNER (Boolean ISBUSINESSPARTNER){Set_Value ("ISBUSINESSPARTNER", ISBUSINESSPARTNER);}/** Get ISBUSINESSPARTNER.
@return ISBUSINESSPARTNER */
public Boolean IsBUSINESSPARTNER() {Object oo = Get_Value("ISBUSINESSPARTNER");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISCAMPAIGN.
@param ISCAMPAIGN ISCAMPAIGN */
public void SetISCAMPAIGN (Boolean ISCAMPAIGN){Set_Value ("IsCampaign", ISCAMPAIGN);}/** Get ISCAMPAIGN.
@return ISCAMPAIGN */
public Boolean IsCAMPAIGN() {Object oo = Get_Value("IsCampaign");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISLOCATIONFROM.
@param ISLOCATIONFROM ISLOCATIONFROM */
public void SetISLOCATIONFROM (Boolean ISLOCATIONFROM){Set_Value ("ISLOCATIONFROM", ISLOCATIONFROM);}/** Get ISLOCATIONFROM.
@return ISLOCATIONFROM */
public Boolean IsLOCATIONFROM() {Object oo = Get_Value("ISLOCATIONFROM");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISLOCATIONTO.
@param ISLOCATIONTO ISLOCATIONTO */
public void SetISLOCATIONTO (Boolean ISLOCATIONTO){Set_Value ("ISLOCATIONTO", ISLOCATIONTO);}/** Get ISLOCATIONTO.
@return ISLOCATIONTO */
public Boolean IsLOCATIONTO() {Object oo = Get_Value("ISLOCATIONTO");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISPRODUCT.
@param ISPRODUCT ISPRODUCT */
public void SetISPRODUCT (Boolean ISPRODUCT){Set_Value ("ISPRODUCT", ISPRODUCT);}/** Get ISPRODUCT.
@return ISPRODUCT */
public Boolean IsPRODUCT() {Object oo = Get_Value("ISPRODUCT");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISPROJECT.
@param ISPROJECT ISPROJECT */
public void SetISPROJECT (Boolean ISPROJECT){Set_Value ("ISPROJECT", ISPROJECT);}/** Get ISPROJECT.
@return ISPROJECT */
public Boolean IsPROJECT() {Object oo = Get_Value("ISPROJECT");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISSALESREGION.
@param ISSALESREGION ISSALESREGION */
public void SetISSALESREGION (Boolean ISSALESREGION){Set_Value ("ISSALESREGION", ISSALESREGION);}/** Get ISSALESREGION.
@return ISSALESREGION */
public Boolean IsSALESREGION() {Object oo = Get_Value("ISSALESREGION");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISUSERELEMENT1.
@param ISUSERELEMENT1 ISUSERELEMENT1 */
public void SetISUSERELEMENT1 (Boolean ISUSERELEMENT1){Set_Value ("ISUSERELEMENT1", ISUSERELEMENT1);}/** Get ISUSERELEMENT1.
@return ISUSERELEMENT1 */
public Boolean IsUSERELEMENT1() {Object oo = Get_Value("ISUSERELEMENT1");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISUSERELEMENT2.
@param ISUSERELEMENT2 ISUSERELEMENT2 */
public void SetISUSERELEMENT2 (Boolean ISUSERELEMENT2){Set_Value ("ISUSERELEMENT2", ISUSERELEMENT2);}/** Get ISUSERELEMENT2.
@return ISUSERELEMENT2 */
public Boolean IsUSERELEMENT2() {Object oo = Get_Value("ISUSERELEMENT2");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISUSERLIST1.
@param ISUSERLIST1 ISUSERLIST1 */
public void SetISUSERLIST1 (Boolean ISUSERLIST1){Set_Value ("ISUSERLIST1", ISUSERLIST1);}/** Get ISUSERLIST1.
@return ISUSERLIST1 */
public Boolean IsUSERLIST1() {Object oo = Get_Value("ISUSERLIST1");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set ISUSERLIST2.
@param ISUSERLIST2 ISUSERLIST2 */
public void SetISUSERLIST2 (Boolean ISUSERLIST2){Set_Value ("ISUSERLIST2", ISUSERLIST2);}/** Get ISUSERLIST2.
@return ISUSERLIST2 */
public Boolean IsUSERLIST2() {Object oo = Get_Value("ISUSERLIST2");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault){Set_Value ("IsDefault", IsDefault);}/** Get Default.
@return Default value */
public Boolean IsDefault() {Object oo = Get_Value("IsDefault");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 3.
@param IsUserElement3 User Element 3 */
public void SetIsUserElement3 (Boolean IsUserElement3){Set_Value ("IsUserElement3", IsUserElement3);}/** Get User Element 3.
@return User Element 3 */
public Boolean IsUserElement3() {Object oo = Get_Value("IsUserElement3");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 4.
@param IsUserElement4 User Element 4 */
public void SetIsUserElement4 (Boolean IsUserElement4){Set_Value ("IsUserElement4", IsUserElement4);}/** Get User Element 4.
@return User Element 4 */
public Boolean IsUserElement4() {Object oo = Get_Value("IsUserElement4");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 5.
@param IsUserElement5 User Element 5 */
public void SetIsUserElement5 (Boolean IsUserElement5){Set_Value ("IsUserElement5", IsUserElement5);}/** Get User Element 5.
@return User Element 5 */
public Boolean IsUserElement5() {Object oo = Get_Value("IsUserElement5");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 6.
@param IsUserElement6 User Element 6 */
public void SetIsUserElement6 (Boolean IsUserElement6){Set_Value ("IsUserElement6", IsUserElement6);}/** Get User Element 6.
@return User Element 6 */
public Boolean IsUserElement6() {Object oo = Get_Value("IsUserElement6");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 7.
@param IsUserElement7 User Element 7 */
public void SetIsUserElement7 (Boolean IsUserElement7){Set_Value ("IsUserElement7", IsUserElement7);}/** Get User Element 7.
@return User Element 7 */
public Boolean IsUserElement7() {Object oo = Get_Value("IsUserElement7");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 8.
@param IsUserElement8 User Element 8 */
public void SetIsUserElement8 (Boolean IsUserElement8){Set_Value ("IsUserElement8", IsUserElement8);}/** Get User Element 8.
@return User Element 8 */
public Boolean IsUserElement8() {Object oo = Get_Value("IsUserElement8");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set User Element 9.
@param IsUserElement9 User Element 9 */
public void SetIsUserElement9 (Boolean IsUserElement9){Set_Value ("IsUserElement9", IsUserElement9);}/** Get User Element 9.
@return User Element 9 */
public Boolean IsUserElement9() {Object oo = Get_Value("IsUserElement9");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}