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
/** Generated Model for VAB_PositionAssignment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_PositionAssignment : PO
{
public X_VAB_PositionAssignment (Context ctx, int VAB_PositionAssignment_ID, Trx trxName) : base (ctx, VAB_PositionAssignment_ID, trxName)
{
/** if (VAB_PositionAssignment_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_PositionAssignment_ID (0);
SetVAB_Position_ID (0);
SetJobAssignmentType (null);	// P
SetValidFrom (DateTime.Now);
}
 */
}
public X_VAB_PositionAssignment (Ctx ctx, int VAB_PositionAssignment_ID, Trx trxName) : base (ctx, VAB_PositionAssignment_ID, trxName)
{
/** if (VAB_PositionAssignment_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_PositionAssignment_ID (0);
SetVAB_Position_ID (0);
SetJobAssignmentType (null);	// P
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PositionAssignment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PositionAssignment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_PositionAssignment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_PositionAssignment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372704L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055915L;
/** VAF_TableView_ID=791 */
public static int Table_ID;
 // =791;

/** TableName=VAB_PositionAssignment */
public static String Table_Name="VAB_PositionAssignment";

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
StringBuilder sb = new StringBuilder ("X_VAB_PositionAssignment[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position Assignment.
@param VAB_PositionAssignment_ID Assignemt of Employee (User) to Job Position */
public void SetVAB_PositionAssignment_ID (int VAB_PositionAssignment_ID)
{
if (VAB_PositionAssignment_ID < 1) throw new ArgumentException ("VAB_PositionAssignment_ID is mandatory.");
Set_ValueNoCheck ("VAB_PositionAssignment_ID", VAB_PositionAssignment_ID);
}
/** Get Position Assignment.
@return Assignemt of Employee (User) to Job Position */
public int GetVAB_PositionAssignment_ID() 
{
Object ii = Get_Value("VAB_PositionAssignment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position.
@param VAB_Position_ID Job Position */
public void SetVAB_Position_ID (int VAB_Position_ID)
{
if (VAB_Position_ID < 1) throw new ArgumentException ("VAB_Position_ID is mandatory.");
Set_ValueNoCheck ("VAB_Position_ID", VAB_Position_ID);
}
/** Get Position.
@return Job Position */
public int GetVAB_Position_ID() 
{
Object ii = Get_Value("VAB_Position_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_Position_ID().ToString());
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

/** JobAssignmentType VAF_Control_Ref_ID=400 */
public static int JOBASSIGNMENTTYPE_VAF_Control_Ref_ID=400;
/** Other = O */
public static String JOBASSIGNMENTTYPE_Other = "O";
/** Primary = P */
public static String JOBASSIGNMENTTYPE_Primary = "P";
/** Secondary = S */
public static String JOBASSIGNMENTTYPE_Secondary = "S";
/** Temporary = T */
public static String JOBASSIGNMENTTYPE_Temporary = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsJobAssignmentTypeValid (String test)
{
return test.Equals("O") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Assignment Type.
@param JobAssignmentType Job Assignment Type */
public void SetJobAssignmentType (String JobAssignmentType)
{
if (JobAssignmentType == null) throw new ArgumentException ("JobAssignmentType is mandatory");
if (!IsJobAssignmentTypeValid(JobAssignmentType))
throw new ArgumentException ("JobAssignmentType Invalid value - " + JobAssignmentType + " - Reference_ID=400 - O - P - S - T");
if (JobAssignmentType.Length > 1)
{
log.Warning("Length > 1 - truncated");
JobAssignmentType = JobAssignmentType.Substring(0,1);
}
Set_Value ("JobAssignmentType", JobAssignmentType);
}
/** Get Assignment Type.
@return Job Assignment Type */
public String GetJobAssignmentType() 
{
return (String)Get_Value("JobAssignmentType");
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
