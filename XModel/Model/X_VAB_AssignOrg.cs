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
/** Generated Model for VAB_AssignOrg
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_AssignOrg : PO
{
public X_VAB_AssignOrg (Context ctx, int VAB_AssignOrg_ID, Trx trxName) : base (ctx, VAB_AssignOrg_ID, trxName)
{
/** if (VAB_AssignOrg_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_AssignOrg_ID (0);
SetOrgAssignmentType (null);
SetValidFrom (DateTime.Now);
SetValidTo (DateTime.Now);
}
 */
}
public X_VAB_AssignOrg (Ctx ctx, int VAB_AssignOrg_ID, Trx trxName) : base (ctx, VAB_AssignOrg_ID, trxName)
{
/** if (VAB_AssignOrg_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_AssignOrg_ID (0);
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
public X_VAB_AssignOrg (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AssignOrg (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AssignOrg (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_AssignOrg()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373441L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056652L;
/** VAF_TableView_ID=585 */
public static int Table_ID;
 // =585;

/** TableName=VAB_AssignOrg */
public static String Table_Name="VAB_AssignOrg";

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
StringBuilder sb = new StringBuilder ("X_VAB_AssignOrg[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Org Assignment.
@param VAB_AssignOrg_ID Assigment to (transaction) Organization */
public void SetVAB_AssignOrg_ID (int VAB_AssignOrg_ID)
{
if (VAB_AssignOrg_ID < 1) throw new ArgumentException ("VAB_AssignOrg_ID is mandatory.");
Set_ValueNoCheck ("VAB_AssignOrg_ID", VAB_AssignOrg_ID);
}
/** Get Org Assignment.
@return Assigment to (transaction) Organization */
public int GetVAB_AssignOrg_ID() 
{
Object ii = Get_Value("VAB_AssignOrg_ID");
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

/** OrgAssignmentType VAF_Control_Ref_ID=401 */
public static int ORGASSIGNMENTTYPE_VAF_Control_Ref_ID=401;
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
