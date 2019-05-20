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
/** Generated Model for PA_SLA_Goal
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_SLA_Goal : PO
{
public X_PA_SLA_Goal (Context ctx, int PA_SLA_Goal_ID, Trx trxName) : base (ctx, PA_SLA_Goal_ID, trxName)
{
/** if (PA_SLA_Goal_ID == 0)
{
SetC_BPartner_ID (0);
SetMeasureActual (0.0);
SetMeasureTarget (0.0);
SetName (null);
SetPA_SLA_Criteria_ID (0);
SetPA_SLA_Goal_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_PA_SLA_Goal (Ctx ctx, int PA_SLA_Goal_ID, Trx trxName) : base (ctx, PA_SLA_Goal_ID, trxName)
{
/** if (PA_SLA_Goal_ID == 0)
{
SetC_BPartner_ID (0);
SetMeasureActual (0.0);
SetMeasureTarget (0.0);
SetName (null);
SetPA_SLA_Criteria_ID (0);
SetPA_SLA_Goal_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Goal (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Goal (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Goal (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_SLA_Goal()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382343L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065554L;
/** AD_Table_ID=745 */
public static int Table_ID;
 // =745;

/** TableName=PA_SLA_Goal */
public static String Table_Name="PA_SLA_Goal";

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
StringBuilder sb = new StringBuilder ("X_PA_SLA_Goal[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Measure Actual.
@param MeasureActual Actual value that has been measured. */
public void SetMeasureActual (Decimal? MeasureActual)
{
if (MeasureActual == null) throw new ArgumentException ("MeasureActual is mandatory.");
Set_Value ("MeasureActual", (Decimal?)MeasureActual);
}
/** Get Measure Actual.
@return Actual value that has been measured. */
public Decimal GetMeasureActual() 
{
Object bd =Get_Value("MeasureActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Measure Target.
@param MeasureTarget Target value for measure */
public void SetMeasureTarget (Decimal? MeasureTarget)
{
if (MeasureTarget == null) throw new ArgumentException ("MeasureTarget is mandatory.");
Set_Value ("MeasureTarget", (Decimal?)MeasureTarget);
}
/** Get Measure Target.
@return Target value for measure */
public Decimal GetMeasureTarget() 
{
Object bd =Get_Value("MeasureTarget");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set SLA Criteria.
@param PA_SLA_Criteria_ID Service Level Agreement Criteria */
public void SetPA_SLA_Criteria_ID (int PA_SLA_Criteria_ID)
{
if (PA_SLA_Criteria_ID < 1) throw new ArgumentException ("PA_SLA_Criteria_ID is mandatory.");
Set_Value ("PA_SLA_Criteria_ID", PA_SLA_Criteria_ID);
}
/** Get SLA Criteria.
@return Service Level Agreement Criteria */
public int GetPA_SLA_Criteria_ID() 
{
Object ii = Get_Value("PA_SLA_Criteria_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set SLA Goal.
@param PA_SLA_Goal_ID Service Level Agreement Goal */
public void SetPA_SLA_Goal_ID (int PA_SLA_Goal_ID)
{
if (PA_SLA_Goal_ID < 1) throw new ArgumentException ("PA_SLA_Goal_ID is mandatory.");
Set_ValueNoCheck ("PA_SLA_Goal_ID", PA_SLA_Goal_ID);
}
/** Get SLA Goal.
@return Service Level Agreement Goal */
public int GetPA_SLA_Goal_ID() 
{
Object ii = Get_Value("PA_SLA_Goal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}
