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
/** Generated Model for VAB_YearPeriod
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_YearPeriod : PO
{
public X_VAB_YearPeriod (Context ctx, int VAB_YearPeriod_ID, Trx trxName) : base (ctx, VAB_YearPeriod_ID, trxName)
{
/** if (VAB_YearPeriod_ID == 0)
{
SetVAB_YearPeriod_ID (0);
SetVAB_Year_ID (0);
SetName (null);
SetPeriodNo (0);
SetPeriodType (null);	// S
SetStartDate (DateTime.Now);
}
 */
}
public X_VAB_YearPeriod (Ctx ctx, int VAB_YearPeriod_ID, Trx trxName) : base (ctx, VAB_YearPeriod_ID, trxName)
{
/** if (VAB_YearPeriod_ID == 0)
{
SetVAB_YearPeriod_ID (0);
SetVAB_Year_ID (0);
SetName (null);
SetPeriodNo (0);
SetPeriodType (null);	// S
SetStartDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriod (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriod (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriod (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_YearPeriod()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374068L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057279L;
/** VAF_TableView_ID=145 */
public static int Table_ID;
 // =145;

/** TableName=VAB_YearPeriod */
public static String Table_Name="VAB_YearPeriod";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_VAB_YearPeriod[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Period.
@param VAB_YearPeriod_ID Period of the Calendar */
public void SetVAB_YearPeriod_ID (int VAB_YearPeriod_ID)
{
if (VAB_YearPeriod_ID < 1) throw new ArgumentException ("VAB_YearPeriod_ID is mandatory.");
Set_ValueNoCheck ("VAB_YearPeriod_ID", VAB_YearPeriod_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetVAB_YearPeriod_ID() 
{
Object ii = Get_Value("VAB_YearPeriod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Year.
@param VAB_Year_ID Calendar Year */
public void SetVAB_Year_ID (int VAB_Year_ID)
{
if (VAB_Year_ID < 1) throw new ArgumentException ("VAB_Year_ID is mandatory.");
Set_ValueNoCheck ("VAB_Year_ID", VAB_Year_ID);
}
/** Get Year.
@return Calendar Year */
public int GetVAB_Year_ID() 
{
Object ii = Get_Value("VAB_Year_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
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
/** Set Period No.
@param PeriodNo Unique Period Number */
public void SetPeriodNo (int PeriodNo)
{
Set_Value ("PeriodNo", PeriodNo);
}
/** Get Period No.
@return Unique Period Number */
public int GetPeriodNo() 
{
Object ii = Get_Value("PeriodNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PeriodType VAF_Control_Ref_ID=115 */
public static int PERIODTYPE_VAF_Control_Ref_ID=115;
/** Adjustment Period = A */
public static String PERIODTYPE_AdjustmentPeriod = "A";
/** Standard Calendar Period = S */
public static String PERIODTYPE_StandardCalendarPeriod = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPeriodTypeValid (String test)
{
return test.Equals("A") || test.Equals("S");
}
/** Set Period Type.
@param PeriodType Period Type */
public void SetPeriodType (String PeriodType)
{
if (PeriodType == null) throw new ArgumentException ("PeriodType is mandatory");
if (!IsPeriodTypeValid(PeriodType))
throw new ArgumentException ("PeriodType Invalid value - " + PeriodType + " - Reference_ID=115 - A - S");
if (PeriodType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PeriodType = PeriodType.Substring(0,1);
}
Set_ValueNoCheck ("PeriodType", PeriodType);
}
/** Get Period Type.
@return Period Type */
public String GetPeriodType() 
{
return (String)Get_Value("PeriodType");
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
}

}
