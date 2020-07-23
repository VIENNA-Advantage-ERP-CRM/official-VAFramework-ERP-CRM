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
using System.Data;
    using VAdvantage.Utility;
/** Generated Model for employee
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_employee : PO
{
public X_employee (Context ctx, int employee_ID, Trx trxName) : base (ctx, employee_ID, trxName)
{
/** if (employee_ID == 0)
{
Setemployee_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_employee (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Serial Version No */
//static long serialVersionUID 27523912829005L;
/** Last Updated Timestamp 5/8/2009 6:28:32 PM */
public static long updatedMS = 1241787512216L;
/** AD_Table_ID=1000003 */
public static int Table_ID=1000003;

/** TableName=employee */
public static String Table_Name="employee";

protected static KeyNamePair model = new KeyNamePair(1000003,"employee");

protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return int.Parse(accessLevel.ToString());
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_employee[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Language.
@param AD_Language_ID System Language */
public void SetAD_Language_ID (int AD_Language_ID)
{
if (AD_Language_ID <= 0) Set_Value ("AD_Language_ID", null);
else
Set_Value ("AD_Language_ID", AD_Language_ID);
}
/** Get Language.
@return System Language */
public int GetAD_Language_ID() 
{
int? ii = (int?)Get_Value("AD_Language_ID");
if (ii == null) return 0;
return (int)ii;
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);
else
Set_Value ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
int? ii = (int?)Get_Value("C_Order_ID");
if (ii == null) return 0;
return (int)ii;
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 200)
{
//log.warning("Length > 200 - truncated");
Description = Description.Substring(0,200);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Designation_ID.
@param Designation_ID Designation_ID */
public void SetDesignation_ID (int Designation_ID)
{
if (Designation_ID <= 0) Set_Value ("Designation_ID", null);
else
Set_Value ("Designation_ID", Designation_ID);
}
/** Get Designation_ID.
@return Designation_ID */
public int GetDesignation_ID() 
{
int? ii = (int?)Get_Value("Designation_ID");
if (ii == null) return 0;
return (int)ii;
}
/** Set employee_ID.
@param employee_ID employee_ID */
public void Setemployee_ID (int employee_ID)
{
if (employee_ID < 1) throw new ArgumentException ("employee_ID is mandatory.");
Set_ValueNoCheck ("employee_ID", employee_ID);
}
/** Get employee_ID.
@return employee_ID */
public int Getemployee_ID() 
{
int? ii = (int?)Get_Value("employee_ID");
if (ii == null) return 0;
return (int)ii;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 30)
{
//log.warning("Length > 30 - truncated");
Name = Name.Substring(0,30);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
}

}
