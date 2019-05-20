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
/** Generated Model for C_PeriodControl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PeriodControl : PO
{
public X_C_PeriodControl (Context ctx, int C_PeriodControl_ID, Trx trxName) : base (ctx, C_PeriodControl_ID, trxName)
{
/** if (C_PeriodControl_ID == 0)
{
SetC_PeriodControl_ID (0);
SetC_Period_ID (0);
SetDocBaseType (null);
SetPeriodAction (null);	// N
}
 */
}
public X_C_PeriodControl (Ctx ctx, int C_PeriodControl_ID, Trx trxName) : base (ctx, C_PeriodControl_ID, trxName)
{
/** if (C_PeriodControl_ID == 0)
{
SetC_PeriodControl_ID (0);
SetC_Period_ID (0);
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
public X_C_PeriodControl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PeriodControl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PeriodControl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PeriodControl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374084L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057295L;
/** AD_Table_ID=229 */
public static int Table_ID;
 // =229;

/** TableName=C_PeriodControl */
public static String Table_Name="C_PeriodControl";

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
StringBuilder sb = new StringBuilder ("X_C_PeriodControl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Period Control.
@param C_PeriodControl_ID Period Control */
public void SetC_PeriodControl_ID (int C_PeriodControl_ID)
{
if (C_PeriodControl_ID < 1) throw new ArgumentException ("C_PeriodControl_ID is mandatory.");
Set_ValueNoCheck ("C_PeriodControl_ID", C_PeriodControl_ID);
}
/** Get Period Control.
@return Period Control */
public int GetC_PeriodControl_ID() 
{
Object ii = Get_Value("C_PeriodControl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_PeriodControl_ID().ToString());
}
/** Set Period.
@param C_Period_ID Period of the Calendar */
public void SetC_Period_ID (int C_Period_ID)
{
if (C_Period_ID < 1) throw new ArgumentException ("C_Period_ID is mandatory.");
Set_ValueNoCheck ("C_Period_ID", C_Period_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetC_Period_ID() 
{
Object ii = Get_Value("C_Period_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** DocBaseType AD_Reference_ID=432 */
public static int DOCBASETYPE_AD_Reference_ID=432;
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

/** PeriodAction AD_Reference_ID=176 */
public static int PERIODACTION_AD_Reference_ID=176;
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

/** PeriodStatus AD_Reference_ID=177 */
public static int PERIODSTATUS_AD_Reference_ID=177;
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
