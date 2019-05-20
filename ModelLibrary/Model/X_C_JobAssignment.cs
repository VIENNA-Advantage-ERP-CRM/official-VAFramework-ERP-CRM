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
/** Generated Model for C_JobAssignment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_JobAssignment : PO
{
public X_C_JobAssignment (Context ctx, int C_JobAssignment_ID, Trx trxName) : base (ctx, C_JobAssignment_ID, trxName)
{
/** if (C_JobAssignment_ID == 0)
{
SetAD_User_ID (0);
SetC_JobAssignment_ID (0);
SetC_Job_ID (0);
SetJobAssignmentType (null);	// P
SetValidFrom (DateTime.Now);
}
 */
}
public X_C_JobAssignment (Ctx ctx, int C_JobAssignment_ID, Trx trxName) : base (ctx, C_JobAssignment_ID, trxName)
{
/** if (C_JobAssignment_ID == 0)
{
SetAD_User_ID (0);
SetC_JobAssignment_ID (0);
SetC_Job_ID (0);
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
public X_C_JobAssignment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_JobAssignment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_JobAssignment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_JobAssignment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372704L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055915L;
/** AD_Table_ID=791 */
public static int Table_ID;
 // =791;

/** TableName=C_JobAssignment */
public static String Table_Name="C_JobAssignment";

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
StringBuilder sb = new StringBuilder ("X_C_JobAssignment[").Append(Get_ID()).Append("]");
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
/** Set Position Assignment.
@param C_JobAssignment_ID Assignemt of Employee (User) to Job Position */
public void SetC_JobAssignment_ID (int C_JobAssignment_ID)
{
if (C_JobAssignment_ID < 1) throw new ArgumentException ("C_JobAssignment_ID is mandatory.");
Set_ValueNoCheck ("C_JobAssignment_ID", C_JobAssignment_ID);
}
/** Get Position Assignment.
@return Assignemt of Employee (User) to Job Position */
public int GetC_JobAssignment_ID() 
{
Object ii = Get_Value("C_JobAssignment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position.
@param C_Job_ID Job Position */
public void SetC_Job_ID (int C_Job_ID)
{
if (C_Job_ID < 1) throw new ArgumentException ("C_Job_ID is mandatory.");
Set_ValueNoCheck ("C_Job_ID", C_Job_ID);
}
/** Get Position.
@return Job Position */
public int GetC_Job_ID() 
{
Object ii = Get_Value("C_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Job_ID().ToString());
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

/** JobAssignmentType AD_Reference_ID=400 */
public static int JOBASSIGNMENTTYPE_AD_Reference_ID=400;
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
