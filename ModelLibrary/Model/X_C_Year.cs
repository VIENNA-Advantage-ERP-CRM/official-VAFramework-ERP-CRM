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
/** Generated Model for C_Year
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Year : PO
{
public X_C_Year (Context ctx, int C_Year_ID, Trx trxName) : base (ctx, C_Year_ID, trxName)
{
/** if (C_Year_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetFiscalYear (null);
}
 */
}
public X_C_Year (Ctx ctx, int C_Year_ID, Trx trxName) : base (ctx, C_Year_ID, trxName)
{
/** if (C_Year_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetFiscalYear (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Year (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Year()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375933L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059144L;
/** AD_Table_ID=177 */
public static int Table_ID;
 // =177;

/** TableName=C_Year */
public static String Table_Name="C_Year";

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
StringBuilder sb = new StringBuilder ("X_C_Year[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Calendar.
@param C_Calendar_ID Accounting Calendar Name */
public void SetC_Calendar_ID (int C_Calendar_ID)
{
if (C_Calendar_ID < 1) throw new ArgumentException ("C_Calendar_ID is mandatory.");
Set_ValueNoCheck ("C_Calendar_ID", C_Calendar_ID);
}
/** Get Calendar.
@return Accounting Calendar Name */
public int GetC_Calendar_ID() 
{
Object ii = Get_Value("C_Calendar_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Year.
@param C_Year_ID Calendar Year */
public void SetC_Year_ID (int C_Year_ID)
{
if (C_Year_ID < 1) throw new ArgumentException ("C_Year_ID is mandatory.");
Set_ValueNoCheck ("C_Year_ID", C_Year_ID);
}
/** Get Year.
@return Calendar Year */
public int GetC_Year_ID() 
{
Object ii = Get_Value("C_Year_ID");
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
/** Set Year.
@param FiscalYear The Fiscal Year */
public void SetFiscalYear (String FiscalYear)
{
if (FiscalYear == null) throw new ArgumentException ("FiscalYear is mandatory.");
if (FiscalYear.Length > 10)
{
log.Warning("Length > 10 - truncated");
FiscalYear = FiscalYear.Substring(0,10);
}
Set_Value ("FiscalYear", FiscalYear);
}
/** Get Year.
@return The Fiscal Year */
public String GetFiscalYear() 
{
return (String)Get_Value("FiscalYear");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetFiscalYear());
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
