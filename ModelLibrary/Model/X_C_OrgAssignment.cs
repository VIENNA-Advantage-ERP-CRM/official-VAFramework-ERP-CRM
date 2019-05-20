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
/** Generated Model for C_OrgAssignment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_OrgAssignment : PO
{
public X_C_OrgAssignment (Context ctx, int C_OrgAssignment_ID, Trx trxName) : base (ctx, C_OrgAssignment_ID, trxName)
{
/** if (C_OrgAssignment_ID == 0)
{
SetAD_User_ID (0);
SetC_OrgAssignment_ID (0);
SetOrgAssignmentType (null);
SetValidFrom (DateTime.Now);
SetValidTo (DateTime.Now);
}
 */
}
public X_C_OrgAssignment (Ctx ctx, int C_OrgAssignment_ID, Trx trxName) : base (ctx, C_OrgAssignment_ID, trxName)
{
/** if (C_OrgAssignment_ID == 0)
{
SetAD_User_ID (0);
SetC_OrgAssignment_ID (0);
SetOrgAssignmentType (null);
SetValidFrom (DateTime.Now);
SetValidTo (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_OrgAssignment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_OrgAssignment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_OrgAssignment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_OrgAssignment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373441L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056652L;
/** AD_Table_ID=585 */
public static int Table_ID;
 // =585;

/** TableName=C_OrgAssignment */
public static String Table_Name="C_OrgAssignment";

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
StringBuilder sb = new StringBuilder ("X_C_OrgAssignment[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Org Assignment.
@param C_OrgAssignment_ID Assigment to (transaction) Organization */
public void SetC_OrgAssignment_ID (int C_OrgAssignment_ID)
{
if (C_OrgAssignment_ID < 1) throw new ArgumentException ("C_OrgAssignment_ID is mandatory.");
Set_ValueNoCheck ("C_OrgAssignment_ID", C_OrgAssignment_ID);
}
/** Get Org Assignment.
@return Assigment to (transaction) Organization */
public int GetC_OrgAssignment_ID() 
{
Object ii = Get_Value("C_OrgAssignment_ID");
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

/** OrgAssignmentType AD_Reference_ID=401 */
public static int ORGASSIGNMENTTYPE_AD_Reference_ID=401;
/** Matrix = M */
public static String ORGASSIGNMENTTYPE_Matrix = "M";
/** Other = O */
public static String ORGASSIGNMENTTYPE_Other = "O";
/** Primary = P */
public static String ORGASSIGNMENTTYPE_Primary = "P";
/** Secondary = S */
public static String ORGASSIGNMENTTYPE_Secondary = "S";
/** Temporary = T */
public static String ORGASSIGNMENTTYPE_Temporary = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsOrgAssignmentTypeValid (String test)
{
return test.Equals("M") || test.Equals("O") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Assignment Type.
@param OrgAssignmentType Organization Assignment Type */
public void SetOrgAssignmentType (String OrgAssignmentType)
{
if (OrgAssignmentType == null) throw new ArgumentException ("OrgAssignmentType is mandatory");
if (!IsOrgAssignmentTypeValid(OrgAssignmentType))
throw new ArgumentException ("OrgAssignmentType Invalid value - " + OrgAssignmentType + " - Reference_ID=401 - M - O - P - S - T");
if (OrgAssignmentType.Length > 1)
{
log.Warning("Length > 1 - truncated");
OrgAssignmentType = OrgAssignmentType.Substring(0,1);
}
Set_Value ("OrgAssignmentType", OrgAssignmentType);
}
/** Get Assignment Type.
@return Organization Assignment Type */
public String GetOrgAssignmentType() 
{
return (String)Get_Value("OrgAssignmentType");
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
if (ValidTo == null) throw new ArgumentException ("ValidTo is mandatory.");
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
