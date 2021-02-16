namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAB_PaymentSchedule
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_PaymentSchedule : PO{public X_VAB_PaymentSchedule (Context ctx, int VAB_PaymentSchedule_ID, Trx trxName) : base (ctx, VAB_PaymentSchedule_ID, trxName){/** if (VAB_PaymentSchedule_ID == 0){SetVAB_PaymentSchedule_ID (0);SetVAB_PaymentTerm_ID (0);SetDiscount (0.0);SetDiscountDays (0);SetGraceDays (0);SetIsValid (false);SetNetDays (0);SetPercentage (0.0);} */
}public X_VAB_PaymentSchedule (Ctx ctx, int VAB_PaymentSchedule_ID, Trx trxName) : base (ctx, VAB_PaymentSchedule_ID, trxName){/** if (VAB_PaymentSchedule_ID == 0){SetVAB_PaymentSchedule_ID (0);SetVAB_PaymentTerm_ID (0);SetDiscount (0.0);SetDiscountDays (0);SetGraceDays (0);SetIsValid (false);SetNetDays (0);SetPercentage (0.0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentSchedule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentSchedule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PaymentSchedule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_PaymentSchedule(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27745177405388L;/** Last Updated Timestamp 5/12/2016 4:51:28 PM */
public static long updatedMS = 1463052088599L;/** VAF_TableView_ID=548 */
public static int Table_ID; // =548;
/** TableName=VAB_PaymentSchedule */
public static String Table_Name="VAB_PaymentSchedule";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAB_PaymentSchedule[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Payment Schedule.
@param VAB_PaymentSchedule_ID Payment Schedule Template */
public void SetVAB_PaymentSchedule_ID (int VAB_PaymentSchedule_ID){if (VAB_PaymentSchedule_ID < 1) throw new ArgumentException ("VAB_PaymentSchedule_ID is mandatory.");Set_ValueNoCheck ("VAB_PaymentSchedule_ID", VAB_PaymentSchedule_ID);}/** Get Payment Schedule.
@return Payment Schedule Template */
public int GetVAB_PaymentSchedule_ID() {Object ii = Get_Value("VAB_PaymentSchedule_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Payment Term.
@param VAB_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetVAB_PaymentTerm_ID (int VAB_PaymentTerm_ID){if (VAB_PaymentTerm_ID < 1) throw new ArgumentException ("VAB_PaymentTerm_ID is mandatory.");Set_ValueNoCheck ("VAB_PaymentTerm_ID", VAB_PaymentTerm_ID);}/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetVAB_PaymentTerm_ID() {Object ii = Get_Value("VAB_PaymentTerm_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetVAB_PaymentTerm_ID().ToString());}/** Set Discount %.
@param Discount Discount in percent */
public void SetDiscount (Decimal? Discount){if (Discount == null) throw new ArgumentException ("Discount is mandatory.");Set_Value ("Discount", (Decimal?)Discount);}/** Get Discount %.
@return Discount in percent */
public Decimal GetDiscount() {Object bd =Get_Value("Discount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Discount Days.
@param DiscountDays Number of days from invoice date to be eligible for discount */
public void SetDiscountDays (int DiscountDays){Set_Value ("DiscountDays", DiscountDays);}/** Get Discount Days.
@return Number of days from invoice date to be eligible for discount */
public int GetDiscountDays() {Object ii = Get_Value("DiscountDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Grace Days.
@param GraceDays Days after due date to send first dunning letter */
public void SetGraceDays (int GraceDays){Set_Value ("GraceDays", GraceDays);}/** Get Grace Days.
@return Days after due date to send first dunning letter */
public int GetGraceDays() {Object ii = Get_Value("GraceDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid){Set_Value ("IsValid", IsValid);}/** Get Valid.
@return Element is valid */
public Boolean IsValid() {Object oo = Get_Value("IsValid");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}
/** NetDay VAF_Control_Ref_ID=167 */
public static int NETDAY_VAF_Control_Ref_ID=167;/** Monday = 1 */
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
public int GetNetDays() {Object ii = Get_Value("NetDays");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Percentage.
@param Percentage Percent of the entire amount */
public void SetPercentage (Decimal? Percentage){if (Percentage == null) throw new ArgumentException ("Percentage is mandatory.");Set_Value ("Percentage", (Decimal?)Percentage);}/** Get Percentage.
@return Percent of the entire amount */
public Decimal GetPercentage() {Object bd =Get_Value("Percentage");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Advance.
@param VA009_Advance Advance */
public void SetVA009_Advance (Boolean VA009_Advance){Set_Value ("VA009_Advance", VA009_Advance);}/** Get Advance.
@return Advance */
public Boolean IsVA009_Advance() {Object oo = Get_Value("VA009_Advance");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}