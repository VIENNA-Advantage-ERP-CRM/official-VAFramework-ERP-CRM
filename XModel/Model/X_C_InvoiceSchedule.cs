namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_InvoiceSchedule
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_InvoiceSchedule : PO
{
public X_C_InvoiceSchedule (Context ctx, int C_InvoiceSchedule_ID, Trx trxName) : base (ctx, C_InvoiceSchedule_ID, trxName)
{
/** if (C_InvoiceSchedule_ID == 0)
{
SetAmt (0.0);
SetC_InvoiceSchedule_ID (0);
SetInvoiceFrequency (null);
SetIsAmount (false);
SetIsDefault (false);
SetName (null);
}
 */
}
public X_C_InvoiceSchedule (Ctx ctx, int C_InvoiceSchedule_ID, Trx trxName) : base (ctx, C_InvoiceSchedule_ID, trxName)
{
/** if (C_InvoiceSchedule_ID == 0)
{
SetAmt (0.0);
SetC_InvoiceSchedule_ID (0);
SetInvoiceFrequency (null);
SetIsAmount (false);
SetIsDefault (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceSchedule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceSchedule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoiceSchedule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_InvoiceSchedule()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372595L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055806L;
/** AD_Table_ID=257 */
public static int Table_ID;
 // =257;

/** TableName=C_InvoiceSchedule */
public static String Table_Name="C_InvoiceSchedule";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_InvoiceSchedule[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Amount.
@param Amt Amount */
public void SetAmt (Decimal? Amt)
{
if (Amt == null) throw new ArgumentException ("Amt is mandatory.");
Set_Value ("Amt", (Decimal?)Amt);
}
/** Get Amount.
@return Amount */
public Decimal GetAmt() 
{
Object bd =Get_Value("Amt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Invoice Schedule.
@param C_InvoiceSchedule_ID Schedule for generating Invoices */
public void SetC_InvoiceSchedule_ID (int C_InvoiceSchedule_ID)
{
if (C_InvoiceSchedule_ID < 1) throw new ArgumentException ("C_InvoiceSchedule_ID is mandatory.");
Set_ValueNoCheck ("C_InvoiceSchedule_ID", C_InvoiceSchedule_ID);
}
/** Get Invoice Schedule.
@return Schedule for generating Invoices */
public int GetC_InvoiceSchedule_ID() 
{
Object ii = Get_Value("C_InvoiceSchedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Invoice on even weeks.
@param EvenInvoiceWeek Send invoices on even weeks */
public void SetEvenInvoiceWeek (Boolean EvenInvoiceWeek)
{
Set_Value ("EvenInvoiceWeek", EvenInvoiceWeek);
}
/** Get Invoice on even weeks.
@return Send invoices on even weeks */
public Boolean IsEvenInvoiceWeek() 
{
Object oo = Get_Value("EvenInvoiceWeek");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Invoice Day.
@param InvoiceDay Day of Invoice Generation */
public void SetInvoiceDay (int InvoiceDay)
{
Set_Value ("InvoiceDay", InvoiceDay);
}
/** Get Invoice Day.
@return Day of Invoice Generation */
public int GetInvoiceDay() 
{
Object ii = Get_Value("InvoiceDay");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice day cut-off.
@param InvoiceDayCutoff Last day for including shipments */
public void SetInvoiceDayCutoff (int InvoiceDayCutoff)
{
Set_Value ("InvoiceDayCutoff", InvoiceDayCutoff);
}
/** Get Invoice day cut-off.
@return Last day for including shipments */
public int GetInvoiceDayCutoff() 
{
Object ii = Get_Value("InvoiceDayCutoff");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** InvoiceFrequency AD_Reference_ID=168 */
public static int INVOICEFREQUENCY_AD_Reference_ID=168;
/** Daily = D */
public static String INVOICEFREQUENCY_Daily = "D";
/** Monthly = M */
public static String INVOICEFREQUENCY_Monthly = "M";
/** Twice Monthly = T */
public static String INVOICEFREQUENCY_TwiceMonthly = "T";
/** Weekly = W */
public static String INVOICEFREQUENCY_Weekly = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsInvoiceFrequencyValid (String test)
{
return test.Equals("D") || test.Equals("M") || test.Equals("T") || test.Equals("W");
}
/** Set Invoice Frequency.
@param InvoiceFrequency How often invoices will be generated */
public void SetInvoiceFrequency (String InvoiceFrequency)
{
if (InvoiceFrequency == null) throw new ArgumentException ("InvoiceFrequency is mandatory");
if (!IsInvoiceFrequencyValid(InvoiceFrequency))
throw new ArgumentException ("InvoiceFrequency Invalid value - " + InvoiceFrequency + " - Reference_ID=168 - D - M - T - W");
if (InvoiceFrequency.Length > 1)
{
log.Warning("Length > 1 - truncated");
InvoiceFrequency = InvoiceFrequency.Substring(0,1);
}
Set_Value ("InvoiceFrequency", InvoiceFrequency);
}
/** Get Invoice Frequency.
@return How often invoices will be generated */
public String GetInvoiceFrequency() 
{
return (String)Get_Value("InvoiceFrequency");
}

/** InvoiceWeekDay AD_Reference_ID=167 */
public static int INVOICEWEEKDAY_AD_Reference_ID=167;
/** Monday = 1 */
public static String INVOICEWEEKDAY_Monday = "1";
/** Tuesday = 2 */
public static String INVOICEWEEKDAY_Tuesday = "2";
/** Wednesday = 3 */
public static String INVOICEWEEKDAY_Wednesday = "3";
/** Thursday = 4 */
public static String INVOICEWEEKDAY_Thursday = "4";
/** Friday = 5 */
public static String INVOICEWEEKDAY_Friday = "5";
/** Saturday = 6 */
public static String INVOICEWEEKDAY_Saturday = "6";
/** Sunday = 7 */
public static String INVOICEWEEKDAY_Sunday = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsInvoiceWeekDayValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7");
}
/** Set Invoice Week Day.
@param InvoiceWeekDay Day to generate invoices */
public void SetInvoiceWeekDay (String InvoiceWeekDay)
{
if (!IsInvoiceWeekDayValid(InvoiceWeekDay))
throw new ArgumentException ("InvoiceWeekDay Invalid value - " + InvoiceWeekDay + " - Reference_ID=167 - 1 - 2 - 3 - 4 - 5 - 6 - 7");
if (InvoiceWeekDay != null && InvoiceWeekDay.Length > 1)
{
log.Warning("Length > 1 - truncated");
InvoiceWeekDay = InvoiceWeekDay.Substring(0,1);
}
Set_Value ("InvoiceWeekDay", InvoiceWeekDay);
}
/** Get Invoice Week Day.
@return Day to generate invoices */
public String GetInvoiceWeekDay() 
{
return (String)Get_Value("InvoiceWeekDay");
}

/** InvoiceWeekDayCutoff AD_Reference_ID=167 */
public static int INVOICEWEEKDAYCUTOFF_AD_Reference_ID=167;
/** Monday = 1 */
public static String INVOICEWEEKDAYCUTOFF_Monday = "1";
/** Tuesday = 2 */
public static String INVOICEWEEKDAYCUTOFF_Tuesday = "2";
/** Wednesday = 3 */
public static String INVOICEWEEKDAYCUTOFF_Wednesday = "3";
/** Thursday = 4 */
public static String INVOICEWEEKDAYCUTOFF_Thursday = "4";
/** Friday = 5 */
public static String INVOICEWEEKDAYCUTOFF_Friday = "5";
/** Saturday = 6 */
public static String INVOICEWEEKDAYCUTOFF_Saturday = "6";
/** Sunday = 7 */
public static String INVOICEWEEKDAYCUTOFF_Sunday = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsInvoiceWeekDayCutoffValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7");
}
/** Set Invoice weekday cutoff.
@param InvoiceWeekDayCutoff Last day in the week for shipments to be included */
public void SetInvoiceWeekDayCutoff (String InvoiceWeekDayCutoff)
{
if (!IsInvoiceWeekDayCutoffValid(InvoiceWeekDayCutoff))
throw new ArgumentException ("InvoiceWeekDayCutoff Invalid value - " + InvoiceWeekDayCutoff + " - Reference_ID=167 - 1 - 2 - 3 - 4 - 5 - 6 - 7");
if (InvoiceWeekDayCutoff != null && InvoiceWeekDayCutoff.Length > 1)
{
log.Warning("Length > 1 - truncated");
InvoiceWeekDayCutoff = InvoiceWeekDayCutoff.Substring(0,1);
}
Set_Value ("InvoiceWeekDayCutoff", InvoiceWeekDayCutoff);
}
/** Get Invoice weekday cutoff.
@return Last day in the week for shipments to be included */
public String GetInvoiceWeekDayCutoff() 
{
return (String)Get_Value("InvoiceWeekDayCutoff");
}
/** Set Amount Limit.
@param IsAmount Send invoices only if the amount exceeds the limit */
public void SetIsAmount (Boolean IsAmount)
{
Set_Value ("IsAmount", IsAmount);
}
/** Get Amount Limit.
@return Send invoices only if the amount exceeds the limit */
public Boolean IsAmount() 
{
Object oo = Get_Value("IsAmount");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
}

}
