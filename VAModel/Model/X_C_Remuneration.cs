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
/** Generated Model for C_Remuneration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Remuneration : PO
{
public X_C_Remuneration (Context ctx, int C_Remuneration_ID, Trx trxName) : base (ctx, C_Remuneration_ID, trxName)
{
/** if (C_Remuneration_ID == 0)
{
SetC_Remuneration_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetName (null);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetRemunerationType (null);
SetStandardHours (0);
}
 */
}
public X_C_Remuneration (Ctx ctx, int C_Remuneration_ID, Trx trxName) : base (ctx, C_Remuneration_ID, trxName)
{
/** if (C_Remuneration_ID == 0)
{
SetC_Remuneration_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetName (null);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetRemunerationType (null);
SetStandardHours (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Remuneration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Remuneration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Remuneration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Remuneration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374554L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057765L;
/** AD_Table_ID=792 */
public static int Table_ID;
 // =792;

/** TableName=C_Remuneration */
public static String Table_Name="C_Remuneration";

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
StringBuilder sb = new StringBuilder ("X_C_Remuneration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Remuneration.
@param C_Remuneration_ID Wage or Salary */
public void SetC_Remuneration_ID (int C_Remuneration_ID)
{
if (C_Remuneration_ID < 1) throw new ArgumentException ("C_Remuneration_ID is mandatory.");
Set_ValueNoCheck ("C_Remuneration_ID", C_Remuneration_ID);
}
/** Get Remuneration.
@return Wage or Salary */
public int GetC_Remuneration_ID() 
{
Object ii = Get_Value("C_Remuneration_ID");
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
/** Set Gross Amount.
@param GrossRAmt Gross Remuneration Amount */
public void SetGrossRAmt (Decimal? GrossRAmt)
{
if (GrossRAmt == null) throw new ArgumentException ("GrossRAmt is mandatory.");
Set_Value ("GrossRAmt", (Decimal?)GrossRAmt);
}
/** Get Gross Amount.
@return Gross Remuneration Amount */
public Decimal GetGrossRAmt() 
{
Object bd =Get_Value("GrossRAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Gross Cost.
@param GrossRCost Gross Remuneration Costs */
public void SetGrossRCost (Decimal? GrossRCost)
{
if (GrossRCost == null) throw new ArgumentException ("GrossRCost is mandatory.");
Set_Value ("GrossRCost", (Decimal?)GrossRCost);
}
/** Get Gross Cost.
@return Gross Remuneration Costs */
public Decimal GetGrossRCost() 
{
Object bd =Get_Value("GrossRCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Overtime Amount.
@param OvertimeAmt Hourly Overtime Rate */
public void SetOvertimeAmt (Decimal? OvertimeAmt)
{
if (OvertimeAmt == null) throw new ArgumentException ("OvertimeAmt is mandatory.");
Set_Value ("OvertimeAmt", (Decimal?)OvertimeAmt);
}
/** Get Overtime Amount.
@return Hourly Overtime Rate */
public Decimal GetOvertimeAmt() 
{
Object bd =Get_Value("OvertimeAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Overtime Cost.
@param OvertimeCost Hourly Overtime Cost */
public void SetOvertimeCost (Decimal? OvertimeCost)
{
if (OvertimeCost == null) throw new ArgumentException ("OvertimeCost is mandatory.");
Set_Value ("OvertimeCost", (Decimal?)OvertimeCost);
}
/** Get Overtime Cost.
@return Hourly Overtime Cost */
public Decimal GetOvertimeCost() 
{
Object bd =Get_Value("OvertimeCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** RemunerationType AD_Reference_ID=346 */
public static int REMUNERATIONTYPE_AD_Reference_ID=346;
/** Bi-Weekly = B */
public static String REMUNERATIONTYPE_Bi_Weekly = "B";
/** Daily = D */
public static String REMUNERATIONTYPE_Daily = "D";
/** Hourly = H */
public static String REMUNERATIONTYPE_Hourly = "H";
/** Monthly = M */
public static String REMUNERATIONTYPE_Monthly = "M";
/** Twice Monthly = T */
public static String REMUNERATIONTYPE_TwiceMonthly = "T";
/** Weekly = W */
public static String REMUNERATIONTYPE_Weekly = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRemunerationTypeValid (String test)
{
return test.Equals("B") || test.Equals("D") || test.Equals("H") || test.Equals("M") || test.Equals("T") || test.Equals("W");
}
/** Set Remuneration Type.
@param RemunerationType Type of Remuneration */
public void SetRemunerationType (String RemunerationType)
{
if (RemunerationType == null) throw new ArgumentException ("RemunerationType is mandatory");
if (!IsRemunerationTypeValid(RemunerationType))
throw new ArgumentException ("RemunerationType Invalid value - " + RemunerationType + " - Reference_ID=346 - B - D - H - M - T - W");
if (RemunerationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
RemunerationType = RemunerationType.Substring(0,1);
}
Set_Value ("RemunerationType", RemunerationType);
}
/** Get Remuneration Type.
@return Type of Remuneration */
public String GetRemunerationType() 
{
return (String)Get_Value("RemunerationType");
}
/** Set Standard Hours.
@param StandardHours Standard Work Hours based on Remuneration Type */
public void SetStandardHours (int StandardHours)
{
Set_Value ("StandardHours", StandardHours);
}
/** Get Standard Hours.
@return Standard Work Hours based on Remuneration Type */
public int GetStandardHours() 
{
Object ii = Get_Value("StandardHours");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
