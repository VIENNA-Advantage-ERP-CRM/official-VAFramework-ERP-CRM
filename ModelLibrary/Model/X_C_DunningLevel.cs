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
/** Generated Model for C_DunningLevel
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_DunningLevel : PO
{
public X_C_DunningLevel (Context ctx, int C_DunningLevel_ID, Trx trxName) : base (ctx, C_DunningLevel_ID, trxName)
{
/** if (C_DunningLevel_ID == 0)
{
SetC_DunningLevel_ID (0);
SetC_Dunning_ID (0);
SetChargeFee (false);
SetChargeInterest (false);
SetDaysAfterDue (0.0);
SetDaysBetweenDunning (0);
SetIsSetCreditStop (false);
SetIsSetPaymentTerm (false);
SetIsShowAllDue (false);
SetIsShowNotDue (false);
SetName (null);
SetPrintName (null);
}
 */
}
public X_C_DunningLevel (Ctx ctx, int C_DunningLevel_ID, Trx trxName) : base (ctx, C_DunningLevel_ID, trxName)
{
/** if (C_DunningLevel_ID == 0)
{
SetC_DunningLevel_ID (0);
SetC_Dunning_ID (0);
SetChargeFee (false);
SetChargeInterest (false);
SetDaysAfterDue (0.0);
SetDaysBetweenDunning (0);
SetIsSetCreditStop (false);
SetIsSetPaymentTerm (false);
SetIsShowAllDue (false);
SetIsShowNotDue (false);
SetName (null);
SetPrintName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningLevel (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningLevel (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningLevel (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_DunningLevel()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371983L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055194L;
/** AD_Table_ID=331 */
public static int Table_ID;
 // =331;

/** TableName=C_DunningLevel */
public static String Table_Name="C_DunningLevel";

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
StringBuilder sb = new StringBuilder ("X_C_DunningLevel[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Dunning Level.
@param C_DunningLevel_ID Dunning Level */
public void SetC_DunningLevel_ID (int C_DunningLevel_ID)
{
if (C_DunningLevel_ID < 1) throw new ArgumentException ("C_DunningLevel_ID is mandatory.");
Set_ValueNoCheck ("C_DunningLevel_ID", C_DunningLevel_ID);
}
/** Get Dunning Level.
@return Dunning Level */
public int GetC_DunningLevel_ID() 
{
Object ii = Get_Value("C_DunningLevel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dunning.
@param C_Dunning_ID Dunning Rules for overdue invoices */
public void SetC_Dunning_ID (int C_Dunning_ID)
{
if (C_Dunning_ID < 1) throw new ArgumentException ("C_Dunning_ID is mandatory.");
Set_ValueNoCheck ("C_Dunning_ID", C_Dunning_ID);
}
/** Get Dunning.
@return Dunning Rules for overdue invoices */
public int GetC_Dunning_ID() 
{
Object ii = Get_Value("C_Dunning_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID)
{
if (C_PaymentTerm_ID <= 0) Set_Value ("C_PaymentTerm_ID", null);
else
Set_Value ("C_PaymentTerm_ID", C_PaymentTerm_ID);
}
/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() 
{
Object ii = Get_Value("C_PaymentTerm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge fee.
@param ChargeFee Indicates if fees will be charged for overdue invoices */
public void SetChargeFee (Boolean ChargeFee)
{
Set_Value ("ChargeFee", ChargeFee);
}
/** Get Charge fee.
@return Indicates if fees will be charged for overdue invoices */
public Boolean IsChargeFee() 
{
Object oo = Get_Value("ChargeFee");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Charge Interest.
@param ChargeInterest Indicates if interest will be charged on overdue invoices */
public void SetChargeInterest (Boolean ChargeInterest)
{
Set_Value ("ChargeInterest", ChargeInterest);
}
/** Get Charge Interest.
@return Indicates if interest will be charged on overdue invoices */
public Boolean IsChargeInterest() 
{
Object oo = Get_Value("ChargeInterest");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Days after due date.
@param DaysAfterDue Days after due date to dun (if negative days until due) */
public void SetDaysAfterDue (Decimal? DaysAfterDue)
{
if (DaysAfterDue == null) throw new ArgumentException ("DaysAfterDue is mandatory.");
Set_Value ("DaysAfterDue", (Decimal?)DaysAfterDue);
}
/** Get Days after due date.
@return Days after due date to dun (if negative days until due) */
public Decimal GetDaysAfterDue() 
{
Object bd =Get_Value("DaysAfterDue");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Days between dunning.
@param DaysBetweenDunning Days between sending dunning notices */
public void SetDaysBetweenDunning (int DaysBetweenDunning)
{
Set_Value ("DaysBetweenDunning", DaysBetweenDunning);
}
/** Get Days between dunning.
@return Days between sending dunning notices */
public int GetDaysBetweenDunning() 
{
Object ii = Get_Value("DaysBetweenDunning");
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

/** Dunning_PrintFormat_ID AD_Reference_ID=259 */
public static int DUNNING_PRINTFORMAT_ID_AD_Reference_ID=259;
/** Set Dunning Print Format.
@param Dunning_PrintFormat_ID Print Format for printing Dunning Letters */
public void SetDunning_PrintFormat_ID (int Dunning_PrintFormat_ID)
{
if (Dunning_PrintFormat_ID <= 0) Set_Value ("Dunning_PrintFormat_ID", null);
else
Set_Value ("Dunning_PrintFormat_ID", Dunning_PrintFormat_ID);
}
/** Get Dunning Print Format.
@return Print Format for printing Dunning Letters */
public int GetDunning_PrintFormat_ID() 
{
Object ii = Get_Value("Dunning_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Fee Amount.
@param FeeAmt Fee amount in invoice currency */
public void SetFeeAmt (Decimal? FeeAmt)
{
Set_Value ("FeeAmt", (Decimal?)FeeAmt);
}
/** Get Fee Amount.
@return Fee amount in invoice currency */
public Decimal GetFeeAmt() 
{
Object bd =Get_Value("FeeAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Interest in percent.
@param InterestPercent Percentage interest to charge on overdue invoices */
public void SetInterestPercent (Decimal? InterestPercent)
{
Set_Value ("InterestPercent", (Decimal?)InterestPercent);
}
/** Get Interest in percent.
@return Percentage interest to charge on overdue invoices */
public Decimal GetInterestPercent() 
{
Object bd =Get_Value("InterestPercent");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** InvoiceCollectionType AD_Reference_ID=394 */
public static int INVOICECOLLECTIONTYPE_AD_Reference_ID=394;
/** Collection Agency = C */
public static String INVOICECOLLECTIONTYPE_CollectionAgency = "C";
/** Dunning = D */
public static String INVOICECOLLECTIONTYPE_Dunning = "D";
/** Legal Procedure = L */
public static String INVOICECOLLECTIONTYPE_LegalProcedure = "L";
/** Uncollectable = U */
public static String INVOICECOLLECTIONTYPE_Uncollectable = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsInvoiceCollectionTypeValid (String test)
{
return test == null || test.Equals("C") || test.Equals("D") || test.Equals("L") || test.Equals("U");
}
/** Set Collection Status.
@param InvoiceCollectionType Invoice Collection Status */
public void SetInvoiceCollectionType (String InvoiceCollectionType)
{
if (!IsInvoiceCollectionTypeValid(InvoiceCollectionType))
throw new ArgumentException ("InvoiceCollectionType Invalid value - " + InvoiceCollectionType + " - Reference_ID=394 - C - D - L - U");
if (InvoiceCollectionType != null && InvoiceCollectionType.Length > 1)
{
log.Warning("Length > 1 - truncated");
InvoiceCollectionType = InvoiceCollectionType.Substring(0,1);
}
Set_Value ("InvoiceCollectionType", InvoiceCollectionType);
}
/** Get Collection Status.
@return Invoice Collection Status */
public String GetInvoiceCollectionType() 
{
return (String)Get_Value("InvoiceCollectionType");
}
/** Set Credit Stop.
@param IsSetCreditStop Set the business partner to credit stop */
public void SetIsSetCreditStop (Boolean IsSetCreditStop)
{
Set_Value ("IsSetCreditStop", IsSetCreditStop);
}
/** Get Credit Stop.
@return Set the business partner to credit stop */
public Boolean IsSetCreditStop() 
{
Object oo = Get_Value("IsSetCreditStop");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Set Payment Term.
@param IsSetPaymentTerm Set the payment term of the Business Partner */
public void SetIsSetPaymentTerm (Boolean IsSetPaymentTerm)
{
Set_Value ("IsSetPaymentTerm", IsSetPaymentTerm);
}
/** Get Set Payment Term.
@return Set the payment term of the Business Partner */
public Boolean IsSetPaymentTerm() 
{
Object oo = Get_Value("IsSetPaymentTerm");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show All Due.
@param IsShowAllDue Show/print all due invoices */
public void SetIsShowAllDue (Boolean IsShowAllDue)
{
Set_Value ("IsShowAllDue", IsShowAllDue);
}
/** Get Show All Due.
@return Show/print all due invoices */
public Boolean IsShowAllDue() 
{
Object oo = Get_Value("IsShowAllDue");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show Not Due.
@param IsShowNotDue Show/print all invoices which are not due (yet). */
public void SetIsShowNotDue (Boolean IsShowNotDue)
{
Set_Value ("IsShowNotDue", IsShowNotDue);
}
/** Get Show Not Due.
@return Show/print all invoices which are not due (yet). */
public Boolean IsShowNotDue() 
{
Object oo = Get_Value("IsShowNotDue");
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
/** Set Note.
@param Note Optional additional user defined information */
public void SetNote (String Note)
{
if (Note != null && Note.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Note = Note.Substring(0,2000);
}
Set_Value ("Note", Note);
}
/** Get Note.
@return Optional additional user defined information */
public String GetNote() 
{
return (String)Get_Value("Note");
}
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName == null) throw new ArgumentException ("PrintName is mandatory.");
if (PrintName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrintName = PrintName.Substring(0,60);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
}

}
