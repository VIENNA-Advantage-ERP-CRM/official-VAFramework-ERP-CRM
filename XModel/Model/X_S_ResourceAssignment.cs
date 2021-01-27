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
/** Generated Model for VAS_Res_Assignment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAS_Res_Assignment : PO
{
public X_VAS_Res_Assignment (Context ctx, int VAS_Res_Assignment_ID, Trx trxName) : base (ctx, VAS_Res_Assignment_ID, trxName)
{
/** if (VAS_Res_Assignment_ID == 0)
{
SetAssignDateFrom (DateTime.Now);
SetIsConfirmed (false);
SetName (null);
SetVAS_Res_Assignment_ID (0);
SetS_Resource_ID (0);
}
 */
}
public X_VAS_Res_Assignment (Ctx ctx, int VAS_Res_Assignment_ID, Trx trxName) : base (ctx, VAS_Res_Assignment_ID, trxName)
{
/** if (VAS_Res_Assignment_ID == 0)
{
SetAssignDateFrom (DateTime.Now);
SetIsConfirmed (false);
SetName (null);
SetVAS_Res_Assignment_ID (0);
SetS_Resource_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAS_Res_Assignment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAS_Res_Assignment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAS_Res_Assignment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAS_Res_Assignment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383754L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066965L;
/** VAF_TableView_ID=485 */
public static int Table_ID;
 // =485;

/** TableName=VAS_Res_Assignment */
public static String Table_Name="VAS_Res_Assignment";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAS_Res_Assignment[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Assign From.
@param AssignDateFrom Assign resource from */
public void SetAssignDateFrom (DateTime? AssignDateFrom)
{
if (AssignDateFrom == null) throw new ArgumentException ("AssignDateFrom is mandatory.");
Set_ValueNoCheck ("AssignDateFrom", (DateTime?)AssignDateFrom);
}
/** Get Assign From.
@return Assign resource from */
public DateTime? GetAssignDateFrom() 
{
return (DateTime?)Get_Value("AssignDateFrom");
}
/** Set Assign To.
@param AssignDateTo Assign resource until */
public void SetAssignDateTo (DateTime? AssignDateTo)
{
Set_ValueNoCheck ("AssignDateTo", (DateTime?)AssignDateTo);
}
/** Get Assign To.
@return Assign resource until */
public DateTime? GetAssignDateTo() 
{
return (DateTime?)Get_Value("AssignDateTo");
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
/** Set Confirmed.
@param IsConfirmed Assignment is confirmed */
public void SetIsConfirmed (Boolean IsConfirmed)
{
Set_ValueNoCheck ("IsConfirmed", IsConfirmed);
}
/** Get Confirmed.
@return Assignment is confirmed */
public Boolean IsConfirmed() 
{
Object oo = Get_Value("IsConfirmed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_ValueNoCheck ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Assigned Resource.
@param VAS_Res_Assignment_ID Assigned Resource */
public void SetVAS_Res_Assignment_ID (int VAS_Res_Assignment_ID)
{
if (VAS_Res_Assignment_ID < 1) throw new ArgumentException ("VAS_Res_Assignment_ID is mandatory.");
Set_ValueNoCheck ("VAS_Res_Assignment_ID", VAS_Res_Assignment_ID);
}
/** Get Assigned Resource.
@return Assigned Resource */
public int GetVAS_Res_Assignment_ID() 
{
Object ii = Get_Value("VAS_Res_Assignment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Resource.
@param VAS_Resource_ID Resource */
public void SetS_Resource_ID (int VAS_Resource_ID)
{
if (VAS_Resource_ID < 1) throw new ArgumentException ("VAS_Resource_ID is mandatory.");
Set_ValueNoCheck ("VAS_Resource_ID", VAS_Resource_ID);
}
/** Get Resource.
@return Resource */
public int GetS_Resource_ID() 
{
Object ii = Get_Value("VAS_Resource_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetS_Resource_ID().ToString());
}
}

}
