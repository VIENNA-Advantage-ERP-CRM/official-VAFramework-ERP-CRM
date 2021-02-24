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
/** Generated Model for VAPA_SLA_Target
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_SLA_Target : PO
{
public X_VAPA_SLA_Target (Context ctx, int VAPA_SLA_Target_ID, Trx trxName) : base (ctx, VAPA_SLA_Target_ID, trxName)
{
/** if (VAPA_SLA_Target_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetMeasureActual (0.0);
SetMeasureTarget (0.0);
SetName (null);
SetVAPA_SLA_Creteria_ID (0);
SetVAPA_SLA_Target_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_VAPA_SLA_Target (Ctx ctx, int VAPA_SLA_Target_ID, Trx trxName) : base (ctx, VAPA_SLA_Target_ID, trxName)
{
/** if (VAPA_SLA_Target_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetMeasureActual (0.0);
SetMeasureTarget (0.0);
SetName (null);
SetVAPA_SLA_Creteria_ID (0);
SetVAPA_SLA_Target_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Target (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Target (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Target (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_SLA_Target()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382343L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065554L;
/** VAF_TableView_ID=745 */
public static int Table_ID;
 // =745;

/** TableName=VAPA_SLA_Target */
public static String Table_Name="VAPA_SLA_Target";

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
StringBuilder sb = new StringBuilder ("X_VAPA_SLA_Target[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
Set_ValueNoCheck ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
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
@param VAPA_SLA_Creteria_ID Service Level Agreement Criteria */
public void SetVAPA_SLA_Creteria_ID (int VAPA_SLA_Creteria_ID)
{
if (VAPA_SLA_Creteria_ID < 1) throw new ArgumentException ("VAPA_SLA_Creteria_ID is mandatory.");
Set_Value ("VAPA_SLA_Creteria_ID", VAPA_SLA_Creteria_ID);
}
/** Get SLA Criteria.
@return Service Level Agreement Criteria */
public int GetVAPA_SLA_Creteria_ID() 
{
Object ii = Get_Value("VAPA_SLA_Creteria_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set SLA Goal.
@param VAPA_SLA_Target_ID Service Level Agreement Goal */
public void SetVAPA_SLA_Target_ID (int VAPA_SLA_Target_ID)
{
if (VAPA_SLA_Target_ID < 1) throw new ArgumentException ("VAPA_SLA_Target_ID is mandatory.");
Set_ValueNoCheck ("VAPA_SLA_Target_ID", VAPA_SLA_Target_ID);
}
/** Get SLA Goal.
@return Service Level Agreement Goal */
public int GetVAPA_SLA_Target_ID() 
{
Object ii = Get_Value("VAPA_SLA_Target_ID");
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
