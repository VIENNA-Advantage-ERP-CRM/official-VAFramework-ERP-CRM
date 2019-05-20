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
/** Generated Model for C_UserRemuneration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_UserRemuneration : PO
{
public X_C_UserRemuneration (Context ctx, int C_UserRemuneration_ID, Trx trxName) : base (ctx, C_UserRemuneration_ID, trxName)
{
/** if (C_UserRemuneration_ID == 0)
{
SetAD_User_ID (0);
SetC_Remuneration_ID (0);
SetC_UserRemuneration_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_C_UserRemuneration (Ctx ctx, int C_UserRemuneration_ID, Trx trxName) : base (ctx, C_UserRemuneration_ID, trxName)
{
/** if (C_UserRemuneration_ID == 0)
{
SetAD_User_ID (0);
SetC_Remuneration_ID (0);
SetC_UserRemuneration_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UserRemuneration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UserRemuneration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UserRemuneration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_UserRemuneration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375823L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059034L;
/** AD_Table_ID=794 */
public static int Table_ID;
 // =794;

/** TableName=C_UserRemuneration */
public static String Table_Name="C_UserRemuneration";

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
StringBuilder sb = new StringBuilder ("X_C_UserRemuneration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
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
/** Set Employee Remuneration.
@param C_UserRemuneration_ID Employee Wage or Salary Overwrite */
public void SetC_UserRemuneration_ID (int C_UserRemuneration_ID)
{
if (C_UserRemuneration_ID < 1) throw new ArgumentException ("C_UserRemuneration_ID is mandatory.");
Set_ValueNoCheck ("C_UserRemuneration_ID", C_UserRemuneration_ID);
}
/** Get Employee Remuneration.
@return Employee Wage or Salary Overwrite */
public int GetC_UserRemuneration_ID() 
{
Object ii = Get_Value("C_UserRemuneration_ID");
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
if (ValidFrom == null) throw new ArgumentException ("ValidFrom is mandatory.");
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
