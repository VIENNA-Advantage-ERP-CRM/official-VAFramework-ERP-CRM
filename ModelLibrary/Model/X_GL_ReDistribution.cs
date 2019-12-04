namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for GL_ReDistribution
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_ReDistribution : PO{public X_GL_ReDistribution (Context ctx, int GL_ReDistribution_ID, Trx trxName) : base (ctx, GL_ReDistribution_ID, trxName){/** if (GL_ReDistribution_ID == 0){SetC_AcctSchema_ID (0);SetDateAcct (DateTime.Now);// SYSDATE
SetGL_ReDistribution_ID (0);} */
}public X_GL_ReDistribution (Ctx ctx, int GL_ReDistribution_ID, Trx trxName) : base (ctx, GL_ReDistribution_ID, trxName){/** if (GL_ReDistribution_ID == 0){SetC_AcctSchema_ID (0);SetDateAcct (DateTime.Now);// SYSDATE
SetGL_ReDistribution_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_ReDistribution (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_ReDistribution (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_ReDistribution (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_ReDistribution(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27856268167673L;/** Last Updated Timestamp 11/19/2019 11:24:10 AM */
public static long updatedMS = 1574142850884L;/** AD_Table_ID=1000533 */
public static int Table_ID; // =1000533;
/** TableName=GL_ReDistribution */
public static String Table_Name="GL_ReDistribution";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_GL_ReDistribution[").Append(Get_ID()).Append("]");return sb.ToString();}
/** Account_ID AD_Reference_ID=132 */
public static int ACCOUNT_ID_AD_Reference_ID=132;/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID){if (Account_ID <= 0) Set_Value ("Account_ID", null);else
Set_Value ("Account_ID", Account_ID);}/** Get Account.
@return Account used */
public int GetAccount_ID() {Object ii = Get_Value("Account_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** C_AcctSchema1_ID AD_Reference_ID=136 */
public static int C_ACCTSCHEMA1_ID_AD_Reference_ID=136;/** Set Primary Accounting Schema.
@param C_AcctSchema1_ID Primary rules for accounting */
public void SetC_AcctSchema1_ID (int C_AcctSchema1_ID){if (C_AcctSchema1_ID <= 0) Set_Value ("C_AcctSchema1_ID", null);else
Set_Value ("C_AcctSchema1_ID", C_AcctSchema1_ID);}/** Get Primary Accounting Schema.
@return Primary rules for accounting */
public int GetC_AcctSchema1_ID() {Object ii = Get_Value("C_AcctSchema1_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID){if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);}/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() {Object ii = Get_Value("C_AcctSchema_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID){if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);else
Set_Value ("C_Currency_ID", C_Currency_ID);}/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() {Object ii = Get_Value("C_Currency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Period.
@param C_Period_ID Period of the Calendar */
public void SetC_Period_ID (int C_Period_ID){if (C_Period_ID <= 0) Set_Value ("C_Period_ID", null);else
Set_Value ("C_Period_ID", C_Period_ID);}/** Get Period.
@return Period of the Calendar */
public int GetC_Period_ID() {Object ii = Get_Value("C_Period_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct){if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");Set_Value ("DateAcct", (DateTime?)DateAcct);}/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() {return (DateTime?)Get_Value("DateAcct");}/** Set Date From.
@param DateFrom Starting date for a range */
public void SetDateFrom (DateTime? DateFrom){Set_Value ("DateFrom", (DateTime?)DateFrom);}/** Get Date From.
@return Starting date for a range */
public DateTime? GetDateFrom() {return (DateTime?)Get_Value("DateFrom");}/** Set Date To.
@param DateTo End date of a date range */
public void SetDateTo (DateTime? DateTo){Set_Value ("DateTo", (DateTime?)DateTo);}/** Get Date To.
@return End date of a date range */
public DateTime? GetDateTo() {return (DateTime?)Get_Value("DateTo");}/** Set Delete existing Accounting Entries.
@param DeletePosting The selected accounting entries will be deleted!  DANGEROUS !!! */
public void SetDeletePosting (String DeletePosting){if (DeletePosting != null && DeletePosting.Length > 1){log.Warning("Length > 1 - truncated");DeletePosting = DeletePosting.Substring(0,1);}Set_Value ("DeletePosting", DeletePosting);}/** Get Delete existing Accounting Entries.
@return The selected accounting entries will be deleted!  DANGEROUS !!! */
public String GetDeletePosting() {return (String)Get_Value("DeletePosting");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Document No..
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo){if (DocumentNo != null && DocumentNo.Length > 30){log.Warning("Length > 30 - truncated");DocumentNo = DocumentNo.Substring(0,30);}Set_Value ("DocumentNo", DocumentNo);}/** Get Document No..
@return Document sequence number of the document */
public String GetDocumentNo() {return (String)Get_Value("DocumentNo");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set GL Re-Distribution.
@param GL_ReDistribution_ID General Ledger Re-Distribution */
public void SetGL_ReDistribution_ID (int GL_ReDistribution_ID){if (GL_ReDistribution_ID < 1) throw new ArgumentException ("GL_ReDistribution_ID is mandatory.");Set_ValueNoCheck ("GL_ReDistribution_ID", GL_ReDistribution_ID);}/** Get GL Re-Distribution.
@return General Ledger Re-Distribution */
public int GetGL_ReDistribution_ID() {Object ii = Get_Value("GL_ReDistribution_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Copy Balance.
@param IsCopyBalance Copy Balance */
public void SetIsCopyBalance (Boolean IsCopyBalance){Set_Value ("IsCopyBalance", IsCopyBalance);}/** Get Copy Balance.
@return Copy Balance */
public Boolean IsCopyBalance() {Object oo = Get_Value("IsCopyBalance");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}
/** PostingType AD_Reference_ID=125 */
public static int POSTINGTYPE_AD_Reference_ID=125;/** Actual = A */
public static String POSTINGTYPE_Actual = "A";/** Budget = B */
public static String POSTINGTYPE_Budget = "B";/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test){return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");}/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType){if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");if (PostingType != null && PostingType.Length > 1){log.Warning("Length > 1 - truncated");PostingType = PostingType.Substring(0,1);}Set_Value ("PostingType", PostingType);}/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() {return (String)Get_Value("PostingType");}/** Set Total Credit.
@param TotalCr Total Credit in document currency */
public void SetTotalCr (Decimal? TotalCr){Set_Value ("TotalCr", (Decimal?)TotalCr);}/** Get Total Credit.
@return Total Credit in document currency */
public Decimal GetTotalCr() {Object bd =Get_Value("TotalCr");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Total Debit.
@param TotalDr Total debit in document currency */
public void SetTotalDr (Decimal? TotalDr){Set_Value ("TotalDr", (Decimal?)TotalDr);}/** Get Total Debit.
@return Total debit in document currency */
public Decimal GetTotalDr() {Object bd =Get_Value("TotalDr");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}