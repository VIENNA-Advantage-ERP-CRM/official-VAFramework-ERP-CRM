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
/** Generated Model for C_AcctProcessor
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AcctProcessor : PO
{
public X_C_AcctProcessor (Context ctx, int C_AcctProcessor_ID, Trx trxName) : base (ctx, C_AcctProcessor_ID, trxName)
{
/** if (C_AcctProcessor_ID == 0)
{
SetC_AcctProcessor_ID (0);
SetKeepLogDays (0);	// 7
SetName (null);
SetSupervisor_ID (0);
}
 */
}
public X_C_AcctProcessor (Ctx ctx, int C_AcctProcessor_ID, Trx trxName) : base (ctx, C_AcctProcessor_ID, trxName)
{
/** if (C_AcctProcessor_ID == 0)
{
SetC_AcctProcessor_ID (0);
SetKeepLogDays (0);	// 7
SetName (null);
SetSupervisor_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessor (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessor (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessor (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AcctProcessor()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369382L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052593L;
/** AD_Table_ID=695 */
public static int Table_ID;
 // =695;

/** TableName=C_AcctProcessor */
public static String Table_Name="C_AcctProcessor";

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
StringBuilder sb = new StringBuilder ("X_C_AcctProcessor[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Schedule.
@param AD_Schedule_ID Execution Schedule */
public void SetAD_Schedule_ID (int AD_Schedule_ID)
{
if (AD_Schedule_ID <= 0) Set_Value ("AD_Schedule_ID", null);
else
Set_Value ("AD_Schedule_ID", AD_Schedule_ID);
}
/** Get Schedule.
@return Execution Schedule */
public int GetAD_Schedule_ID() 
{
Object ii = Get_Value("AD_Schedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Processor.
@param C_AcctProcessor_ID Accounting Processor/Server Parameters */
public void SetC_AcctProcessor_ID (int C_AcctProcessor_ID)
{
if (C_AcctProcessor_ID < 1) throw new ArgumentException ("C_AcctProcessor_ID is mandatory.");
Set_ValueNoCheck ("C_AcctProcessor_ID", C_AcctProcessor_ID);
}
/** Get Accounting Processor.
@return Accounting Processor/Server Parameters */
public int GetC_AcctProcessor_ID() 
{
Object ii = Get_Value("C_AcctProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID <= 0) Set_Value ("C_AcctSchema_ID", null);
else
Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_Value ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
}
/** Set Date next run.
@param DateNextRun Date the process will run next */
public void SetDateNextRun (DateTime? DateNextRun)
{
Set_Value ("DateNextRun", (DateTime?)DateNextRun);
}
/** Get Date next run.
@return Date the process will run next */
public DateTime? GetDateNextRun() 
{
return (DateTime?)Get_Value("DateNextRun");
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
/** Set Frequency.
@param Frequency Frequency of events */
public void SetFrequency (int Frequency)
{
Set_Value ("Frequency", Frequency);
}
/** Get Frequency.
@return Frequency of events */
public int GetFrequency() 
{
Object ii = Get_Value("Frequency");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** FrequencyType AD_Reference_ID=221 */
public static int FREQUENCYTYPE_AD_Reference_ID=221;
/** Day = D */
public static String FREQUENCYTYPE_Day = "D";
/** Hour = H */
public static String FREQUENCYTYPE_Hour = "H";
/** Minute = M */
public static String FREQUENCYTYPE_Minute = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFrequencyTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("H") || test.Equals("M");
}
/** Set Frequency Type.
@param FrequencyType Frequency of event */
public void SetFrequencyType (String FrequencyType)
{
if (!IsFrequencyTypeValid(FrequencyType))
throw new ArgumentException ("FrequencyType Invalid value - " + FrequencyType + " - Reference_ID=221 - D - H - M");
if (FrequencyType != null && FrequencyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
FrequencyType = FrequencyType.Substring(0,1);
}
Set_Value ("FrequencyType", FrequencyType);
}
/** Get Frequency Type.
@return Frequency of event */
public String GetFrequencyType() 
{
return (String)Get_Value("FrequencyType");
}
/** Set Days to keep Log.
@param KeepLogDays Number of days to keep the log entries */
public void SetKeepLogDays (int KeepLogDays)
{
Set_Value ("KeepLogDays", KeepLogDays);
}
/** Get Days to keep Log.
@return Number of days to keep the log entries */
public int GetKeepLogDays() 
{
Object ii = Get_Value("KeepLogDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** Supervisor_ID AD_Reference_ID=286 */
public static int SUPERVISOR_ID_AD_Reference_ID=286;
/** Set Supervisor.
@param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
public void SetSupervisor_ID (int Supervisor_ID)
{
if (Supervisor_ID < 1) throw new ArgumentException ("Supervisor_ID is mandatory.");
Set_Value ("Supervisor_ID", Supervisor_ID);
}
/** Get Supervisor.
@return Supervisor for this user/organization - used for escalation and approval */
public int GetSupervisor_ID() 
{
Object ii = Get_Value("Supervisor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
