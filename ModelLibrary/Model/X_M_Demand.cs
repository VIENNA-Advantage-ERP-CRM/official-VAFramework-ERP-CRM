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
/** Generated Model for M_Demand
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Demand : PO
{
public X_M_Demand (Context ctx, int M_Demand_ID, Trx trxName) : base (ctx, M_Demand_ID, trxName)
{
/** if (M_Demand_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetIsDefault (false);
SetM_Demand_ID (0);
SetName (null);
}
 */
}
public X_M_Demand (Ctx ctx, int M_Demand_ID, Trx trxName) : base (ctx, M_Demand_ID, trxName)
{
/** if (M_Demand_ID == 0)
{
SetC_Calendar_ID (0);
SetC_Year_ID (0);
SetIsDefault (false);
SetM_Demand_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Demand (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Demand (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Demand (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Demand()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379020L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062231L;
/** AD_Table_ID=723 */
public static int Table_ID;
 // =723;

/** TableName=M_Demand */
public static String Table_Name="M_Demand";

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
StringBuilder sb = new StringBuilder ("X_M_Demand[").Append(Get_ID()).Append("]");
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
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Demand.
@param M_Demand_ID Material Demand */
public void SetM_Demand_ID (int M_Demand_ID)
{
if (M_Demand_ID < 1) throw new ArgumentException ("M_Demand_ID is mandatory.");
Set_ValueNoCheck ("M_Demand_ID", M_Demand_ID);
}
/** Get Demand.
@return Material Demand */
public int GetM_Demand_ID() 
{
Object ii = Get_Value("M_Demand_ID");
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
}

}
