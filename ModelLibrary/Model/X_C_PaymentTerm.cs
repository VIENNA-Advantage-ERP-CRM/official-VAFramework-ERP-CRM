namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_PaymentTerm
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PaymentTerm : PO{public X_C_PaymentTerm (Context ctx, int C_PaymentTerm_ID, Trx trxName) : base (ctx, C_PaymentTerm_ID, trxName){/** if (C_PaymentTerm_ID == 0){SetAfterDelivery (false);SetC_PaymentTerm_ID (0);SetDiscount (0.0);SetDiscount2 (0.0);SetDiscountDays (0);SetDiscountDays2 (0);SetGraceDays (0);SetIsDueFixed (false);SetIsValid (false);SetName (null);SetNetDays (0);SetValue (null);} */
}public X_C_PaymentTerm (Ctx ctx, int C_PaymentTerm_ID, Trx trxName) : base (ctx, C_PaymentTerm_ID, trxName){/** if (C_PaymentTerm_ID == 0){SetAfterDelivery (false);SetC_PaymentTerm_ID (0);SetDiscount (0.0);SetDiscount2 (0.0);SetDiscountDays (0);SetDiscountDays2 (0);SetGraceDays (0);SetIsDueFixed (false);SetIsValid (false);SetName (null);SetNetDays (0);SetValue (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PaymentTerm(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27745177343589L;/** Last Updated Timestamp 5/12/2016 4:50:26 PM */
public static long updatedMS = 1463052026800L;/** AD_Table_ID=113 */
public static int Table_ID; // =113;
/** TableName=C_PaymentTerm */
public static String Table_Name="C_PaymentTerm";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_PaymentTerm[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set After Delivery.
@param AfterDelivery Due after delivery rather than after invoicing */
public void SetAfterDelivery (Boolean AfterDelivery){Set_Value ("AfterDelivery", AfterDelivery);}/** Get After Delivery.
@return Due after delivery rather than after invoicing */
public Boolean IsAfterDelivery() {Object oo = Get_Value("AfterDelivery");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID){if (C_PaymentTerm_ID < 1) throw new ArgumentException ("C_PaymentTerm_ID is mandatory.");Set_ValueNoCheck ("C_PaymentTerm_ID", C_PaymentTerm_ID);}/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() {Object ii = Get_Value("C_PaymentTerm_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Discount %.
@param Discount Discount in percent */
public void SetDiscount (Decimal? Discount){if (Discount == null) throw new ArgumentException ("Discount is mandatory.");Set_Value ("Discount", (Decimal?)Discount);}/** Get Discount %.
@return Discount in percent */
public Decimal GetDiscount() {Object bd =Get_Value("Discount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Discount 2 %.
@param Discount2 Discount in percent */
public void SetDiscount2 (Decimal? Discount2){if (Discount2 == null) throw new ArgumentException ("Discount2 is mandatory.");Set_Value ("Discount2", (Decimal?)Discount2);}/** Get Discount 2 %.
@return Discount in percent */
public Decimal GetDiscount2() {Object bd =Get_Value("Discount2");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Discount Days.
@param DiscountDays Number of days from invoice date to be eligible for discount */
public void SetDiscountDays (int DiscountDays){Set_Value ("DiscountDays", DiscountDays);}/** Get Discount Days.
@return Number of days from invoice date to be eligible for discount */
public int GetDiscountDays() {Object ii = Get_Value("DiscountDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Discount Days 2.
@param DiscountDays2 Number of days from invoice date to be eligible for discount */
public void SetDiscountDays2 (int DiscountDays2){Set_Value ("DiscountDays2", DiscountDays2);}/** Get Discount Days 2.
@return Number of days from invoice date to be eligible for discount */
public int GetDiscountDays2() {Object ii = Get_Value("DiscountDays2");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Document Note.
@param DocumentNote Additional information for a Document */
public void SetDocumentNote (String DocumentNote){if (DocumentNote != null && DocumentNote.Length > 2000){log.Warning("Length > 2000 - truncated");DocumentNote = DocumentNote.Substring(0,2000);}Set_Value ("DocumentNote", DocumentNote);}/** Get Document Note.
@return Additional information for a Document */
public String GetDocumentNote() {return (String)Get_Value("DocumentNote");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Fix month cutoff.
@param FixMonthCutoff Last day to include for next due date */
public void SetFixMonthCutoff (int FixMonthCutoff){Set_Value ("FixMonthCutoff", FixMonthCutoff);}/** Get Fix month cutoff.
@return Last day to include for next due date */
public int GetFixMonthCutoff() {Object ii = Get_Value("FixMonthCutoff");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Fix month day.
@param FixMonthDay Day of the month of the due date */
public void SetFixMonthDay (int FixMonthDay){Set_Value ("FixMonthDay", FixMonthDay);}/** Get Fix month day.
@return Day of the month of the due date */
public int GetFixMonthDay() {Object ii = Get_Value("FixMonthDay");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Fix month offset.
@param FixMonthOffset Number of months (0=same, 1=following) */
public void SetFixMonthOffset (int FixMonthOffset){Set_Value ("FixMonthOffset", FixMonthOffset);}/** Get Fix month offset.
@return Number of months (0=same, 1=following) */
public int GetFixMonthOffset() {Object ii = Get_Value("FixMonthOffset");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Grace Days.
@param GraceDays Days after due date to send first dunning letter */
public void SetGraceDays (int GraceDays){Set_Value ("GraceDays", GraceDays);}/** Get Grace Days.
@return Days after due date to send first dunning letter */
public int GetGraceDays() {Object ii = Get_Value("GraceDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault){Set_Value ("IsDefault", IsDefault);}/** Get Default.
@return Default value */
public Boolean IsDefault() {Object oo = Get_Value("IsDefault");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Fixed due date.
@param IsDueFixed Payment is due on a fixed date */
public void SetIsDueFixed (Boolean IsDueFixed){Set_Value ("IsDueFixed", IsDueFixed);}/** Get Fixed due date.
@return Payment is due on a fixed date */
public Boolean IsDueFixed() {Object oo = Get_Value("IsDueFixed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Next Business Day.
@param IsNextBusinessDay Payment due on the next business day */
public void SetIsNextBusinessDay (Boolean IsNextBusinessDay){Set_Value ("IsNextBusinessDay", IsNextBusinessDay);}/** Get Next Business Day.
@return Payment due on the next business day */
public Boolean IsNextBusinessDay() {Object oo = Get_Value("IsNextBusinessDay");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid){Set_Value ("IsValid", IsValid);}/** Get Valid.
@return Element is valid */
public Boolean IsValid() {Object oo = Get_Value("IsValid");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 60){log.Warning("Length > 60 - truncated");Name = Name.Substring(0,60);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetName());}
/** NetDay AD_Reference_ID=167 */
public static int NETDAY_AD_Reference_ID=167;/** Monday = 1 */
public static String NETDAY_Monday = "1";/** Tuesday = 2 */
public static String NETDAY_Tuesday = "2";/** Wednesday = 3 */
public static String NETDAY_Wednesday = "3";/** Thursday = 4 */
public static String NETDAY_Thursday = "4";/** Friday = 5 */
public static String NETDAY_Friday = "5";/** Saturday = 6 */
public static String NETDAY_Saturday = "6";/** Sunday = 7 */
public static String NETDAY_Sunday = "7";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsNetDayValid (String test){return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7");}/** Set Net Day.
@param NetDay Day when payment is due net */
public void SetNetDay (String NetDay){if (!IsNetDayValid(NetDay))
throw new ArgumentException ("NetDay Invalid value - " + NetDay + " - Reference_ID=167 - 1 - 2 - 3 - 4 - 5 - 6 - 7");if (NetDay != null && NetDay.Length > 1){log.Warning("Length > 1 - truncated");NetDay = NetDay.Substring(0,1);}Set_Value ("NetDay", NetDay);}/** Get Net Day.
@return Day when payment is due net */
public String GetNetDay() {return (String)Get_Value("NetDay");}/** Set Net Days.
@param NetDays Net Days in which payment is due */
public void SetNetDays (int NetDays){Set_Value ("NetDays", NetDays);}/** Get Net Days.
@return Net Days in which payment is due */
public int GetNetDays() {Object ii = Get_Value("NetDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing){Set_Value ("Processing", Processing);}/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() {Object oo = Get_Value("Processing");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Advance.
@param VA009_Advance Advance */
public void SetVA009_Advance (Boolean VA009_Advance){Set_Value ("VA009_Advance", VA009_Advance);}/** Get Advance.
@return Advance */
public Boolean IsVA009_Advance() {Object oo = Get_Value("VA009_Advance");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;} /** Set Week Offset.
@param WeekOffset Number of weeks (0=same, 1=following) */
public void SetWeekOffset(int WeekOffset) { Set_Value("WeekOffset", WeekOffset); }/** Get Week Offset.
@return Number of weeks (0=same, 1=following) */
public int GetWeekOffset() { Object ii = Get_Value("WeekOffset"); if (ii == null) return 0; return Convert.ToInt32(ii); } /** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
    public void SetValue (String Value){if (Value == null) throw new ArgumentException ("Value is mandatory.");if (Value.Length > 40){log.Warning("Length > 40 - truncated");Value = Value.Substring(0,40);}Set_Value ("Value", Value);}/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() {return (String)Get_Value("Value");}}
}