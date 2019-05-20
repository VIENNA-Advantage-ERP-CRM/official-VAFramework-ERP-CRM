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
/** Generated Model for R_RequestType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_RequestType : PO
{
public X_R_RequestType (Context ctx, int R_RequestType_ID, Trx trxName) : base (ctx, R_RequestType_ID, trxName)
{
/** if (R_RequestType_ID == 0)
{
SetConfidentialType (null);	// C
SetDueDateTolerance (0);	// 7
SetIsAutoChangeRequest (false);
SetIsConfidentialInfo (false);	// N
SetIsDefault (false);	// N
SetIsEMailWhenDue (false);
SetIsEMailWhenOverdue (false);
SetIsIndexed (false);
SetIsSelfService (true);	// Y
SetName (null);
SetR_RequestType_ID (0);
SetR_StatusCategory_ID (0);
}
 */
}
public X_R_RequestType (Ctx ctx, int R_RequestType_ID, Trx trxName) : base (ctx, R_RequestType_ID, trxName)
{
/** if (R_RequestType_ID == 0)
{
SetConfidentialType (null);	// C
SetDueDateTolerance (0);	// 7
SetIsAutoChangeRequest (false);
SetIsConfidentialInfo (false);	// N
SetIsDefault (false);	// N
SetIsEMailWhenDue (false);
SetIsEMailWhenOverdue (false);
SetIsIndexed (false);
SetIsSelfService (true);	// Y
SetName (null);
SetR_RequestType_ID (0);
SetR_StatusCategory_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_RequestType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383315L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066526L;
/** AD_Table_ID=529 */
public static int Table_ID;
 // =529;

/** TableName=R_RequestType */
public static String Table_Name="R_RequestType";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_R_RequestType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Auto Due Date Days.
@param AutoDueDateDays Automatic Due Date Days */
public void SetAutoDueDateDays (int AutoDueDateDays)
{
Set_Value ("AutoDueDateDays", AutoDueDateDays);
}
/** Get Auto Due Date Days.
@return Automatic Due Date Days */
public int GetAutoDueDateDays() 
{
Object ii = Get_Value("AutoDueDateDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ConfidentialType AD_Reference_ID=340 */
public static int CONFIDENTIALTYPE_AD_Reference_ID=340;
/** Public Information = A */
public static String CONFIDENTIALTYPE_PublicInformation = "A";
/** Partner Confidential = C */
public static String CONFIDENTIALTYPE_PartnerConfidential = "C";
/** Internal = I */
public static String CONFIDENTIALTYPE_Internal = "I";
/** Private Information = P */
public static String CONFIDENTIALTYPE_PrivateInformation = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfidentialTypeValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Confidentiality.
@param ConfidentialType Type of Confidentiality */
public void SetConfidentialType (String ConfidentialType)
{
if (ConfidentialType == null) throw new ArgumentException ("ConfidentialType is mandatory");
if (!IsConfidentialTypeValid(ConfidentialType))
throw new ArgumentException ("ConfidentialType Invalid value - " + ConfidentialType + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialType = ConfidentialType.Substring(0,1);
}
Set_Value ("ConfidentialType", ConfidentialType);
}
/** Get Confidentiality.
@return Type of Confidentiality */
public String GetConfidentialType() 
{
return (String)Get_Value("ConfidentialType");
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
/** Set Due Date Tolerance.
@param DueDateTolerance Tolerance in days between the Date Next Action and the date the request is regarded as overdue */
public void SetDueDateTolerance (int DueDateTolerance)
{
Set_Value ("DueDateTolerance", DueDateTolerance);
}
/** Get Due Date Tolerance.
@return Tolerance in days between the Date Next Action and the date the request is regarded as overdue */
public int GetDueDateTolerance() 
{
Object ii = Get_Value("DueDateTolerance");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Create Change Request.
@param IsAutoChangeRequest Automatically create BOM (Engineering) Change Request */
public void SetIsAutoChangeRequest (Boolean IsAutoChangeRequest)
{
Set_Value ("IsAutoChangeRequest", IsAutoChangeRequest);
}
/** Get Create Change Request.
@return Automatically create BOM (Engineering) Change Request */
public Boolean IsAutoChangeRequest() 
{
Object oo = Get_Value("IsAutoChangeRequest");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Confidential Info.
@param IsConfidentialInfo Can enter confidential information */
public void SetIsConfidentialInfo (Boolean IsConfidentialInfo)
{
Set_Value ("IsConfidentialInfo", IsConfidentialInfo);
}
/** Get Confidential Info.
@return Can enter confidential information */
public Boolean IsConfidentialInfo() 
{
Object oo = Get_Value("IsConfidentialInfo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set EMail when Due.
@param IsEMailWhenDue Send EMail when Request becomes due */
public void SetIsEMailWhenDue (Boolean IsEMailWhenDue)
{
Set_Value ("IsEMailWhenDue", IsEMailWhenDue);
}
/** Get EMail when Due.
@return Send EMail when Request becomes due */
public Boolean IsEMailWhenDue() 
{
Object oo = Get_Value("IsEMailWhenDue");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set EMail when Overdue.
@param IsEMailWhenOverdue Send EMail when Request becomes overdue */
public void SetIsEMailWhenOverdue (Boolean IsEMailWhenOverdue)
{
Set_Value ("IsEMailWhenOverdue", IsEMailWhenOverdue);
}
/** Get EMail when Overdue.
@return Send EMail when Request becomes overdue */
public Boolean IsEMailWhenOverdue() 
{
Object oo = Get_Value("IsEMailWhenOverdue");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Indexed.
@param IsIndexed Index the document for the internal search engine */
public void SetIsIndexed (Boolean IsIndexed)
{
Set_Value ("IsIndexed", IsIndexed);
}
/** Get Indexed.
@return Index the document for the internal search engine */
public Boolean IsIndexed() 
{
Object oo = Get_Value("IsIndexed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Invoiced.
@param IsInvoiced Is this invoiced? */
public void SetIsInvoiced (Boolean IsInvoiced)
{
Set_Value ("IsInvoiced", IsInvoiced);
}
/** Get Invoiced.
@return Is this invoiced? */
public Boolean IsInvoiced() 
{
Object oo = Get_Value("IsInvoiced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Request Type.
@param R_RequestType_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetR_RequestType_ID (int R_RequestType_ID)
{
if (R_RequestType_ID < 1) throw new ArgumentException ("R_RequestType_ID is mandatory.");
Set_ValueNoCheck ("R_RequestType_ID", R_RequestType_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetR_RequestType_ID() 
{
Object ii = Get_Value("R_RequestType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status Category.
@param R_StatusCategory_ID Request Status Category */
public void SetR_StatusCategory_ID (int R_StatusCategory_ID)
{
if (R_StatusCategory_ID < 1) throw new ArgumentException ("R_StatusCategory_ID is mandatory.");
Set_Value ("R_StatusCategory_ID", R_StatusCategory_ID);
}
/** Get Status Category.
@return Request Status Category */
public int GetR_StatusCategory_ID() 
{
Object ii = Get_Value("R_StatusCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
