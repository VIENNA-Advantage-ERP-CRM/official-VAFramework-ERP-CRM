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
/** Generated Model for VAB_YearPeriodControl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_YearPeriodControl : PO
{
public X_VAB_YearPeriodControl (Context ctx, int VAB_YearPeriodControl_ID, Trx trxName) : base (ctx, VAB_YearPeriodControl_ID, trxName)
{
/** if (VAB_YearPeriodControl_ID == 0)
{
SetVAB_YearPeriodControl_ID (0);
SetVAB_YearPeriod_ID (0);
SetDocBaseType (null);
SetPeriodAction (null);	// N
}
 */
}
public X_VAB_YearPeriodControl (Ctx ctx, int VAB_YearPeriodControl_ID, Trx trxName) : base (ctx, VAB_YearPeriodControl_ID, trxName)
{
/** if (VAB_YearPeriodControl_ID == 0)
{
SetVAB_YearPeriodControl_ID (0);
SetVAB_YearPeriod_ID (0);
SetDocBaseType (null);
SetPeriodAction (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriodControl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriodControl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_YearPeriodControl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_YearPeriodControl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374084L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057295L;
/** VAF_TableView_ID=229 */
public static int Table_ID;
 // =229;

/** TableName=VAB_YearPeriodControl */
public static String Table_Name="VAB_YearPeriodControl";

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
StringBuilder sb = new StringBuilder ("X_VAB_YearPeriodControl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Period Control.
@param VAB_YearPeriodControl_ID Period Control */
public void SetVAB_YearPeriodControl_ID (int VAB_YearPeriodControl_ID)
{
if (VAB_YearPeriodControl_ID < 1) throw new ArgumentException ("VAB_YearPeriodControl_ID is mandatory.");
Set_ValueNoCheck ("VAB_YearPeriodControl_ID", VAB_YearPeriodControl_ID);
}
/** Get Period Control.
@return Period Control */
public int GetVAB_YearPeriodControl_ID() 
{
Object ii = Get_Value("VAB_YearPeriodControl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_YearPeriodControl_ID().ToString());
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

/** DocBaseType VAF_Control_Ref_ID=432 */
public static int DOCBASETYPE_VAF_Control_Ref_ID=432;
/** Set Document BaseType.
@param DocBaseType Logical type of document */
public void SetDocBaseType (String DocBaseType)
{
if (DocBaseType.Length > 3)
{
log.Warning("Length > 3 - truncated");
DocBaseType = DocBaseType.Substring(0,3);
}
Set_ValueNoCheck ("DocBaseType", DocBaseType);
}
/** Get Document BaseType.
@return Logical type of document */
public String GetDocBaseType() 
{
return (String)Get_Value("DocBaseType");
}

/** PeriodAction VAF_Control_Ref_ID=176 */
public static int PERIODACTION_VAF_Control_Ref_ID=176;
/** Close Period = C */
public static String PERIODACTION_ClosePeriod = "C";
/** <No Action> = N */
public static String PERIODACTION_NoAction = "N";
/** Open Period = O */
public static String PERIODACTION_OpenPeriod = "O";
/** Permanently Close Period = P */
public static String PERIODACTION_PermanentlyClosePeriod = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPeriodActionValid (String test)
{
return test.Equals("C") || test.Equals("N") || test.Equals("O") || test.Equals("P");
}
/** Set Period Action.
@param PeriodAction Action taken for this period */
public void SetPeriodAction (String PeriodAction)
{
if (PeriodAction == null) throw new ArgumentException ("PeriodAction is mandatory");
if (!IsPeriodActionValid(PeriodAction))
throw new ArgumentException ("PeriodAction Invalid value - " + PeriodAction + " - Reference_ID=176 - C - N - O - P");
if (PeriodAction.Length > 1)
{
log.Warning("Length > 1 - truncated");
PeriodAction = PeriodAction.Substring(0,1);
}
Set_Value ("PeriodAction", PeriodAction);
}
/** Get Period Action.
@return Action taken for this period */
public String GetPeriodAction() 
{
return (String)Get_Value("PeriodAction");
}

/** PeriodStatus VAF_Control_Ref_ID=177 */
public static int PERIODSTATUS_VAF_Control_Ref_ID=177;
/** Closed = C */
public static String PERIODSTATUS_Closed = "C";
/** Never opened = N */
public static String PERIODSTATUS_NeverOpened = "N";
/** Open = O */
public static String PERIODSTATUS_Open = "O";
/** Permanently closed = P */
public static String PERIODSTATUS_PermanentlyClosed = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPeriodStatusValid (String test)
{
return test == null || test.Equals("C") || test.Equals("N") || test.Equals("O") || test.Equals("P");
}
/** Set Period Status.
@param PeriodStatus Current state of this period */
public void SetPeriodStatus (String PeriodStatus)
{
if (!IsPeriodStatusValid(PeriodStatus))
throw new ArgumentException ("PeriodStatus Invalid value - " + PeriodStatus + " - Reference_ID=177 - C - N - O - P");
if (PeriodStatus != null && PeriodStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
PeriodStatus = PeriodStatus.Substring(0,1);
}
Set_ValueNoCheck ("PeriodStatus", PeriodStatus);
}
/** Get Period Status.
@return Current state of this period */
public String GetPeriodStatus() 
{
return (String)Get_Value("PeriodStatus");
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
}

}
