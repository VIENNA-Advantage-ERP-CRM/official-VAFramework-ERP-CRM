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
/** Generated Model for R_Status
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_Status : PO
{
public X_R_Status (Context ctx, int R_Status_ID, Trx trxName) : base (ctx, R_Status_ID, trxName)
{
/** if (R_Status_ID == 0)
{
SetIsClosed (false);	// N
SetIsDefault (false);
SetIsFinalClose (false);	// N
SetIsOpen (false);
SetIsWebCanUpdate (false);
SetName (null);
SetR_StatusCategory_ID (0);
SetR_Status_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM R_Status WHERE R_StatusCategory_ID=@R_StatusCategory_ID@
SetValue (null);
}
 */
}
public X_R_Status (Ctx ctx, int R_Status_ID, Trx trxName) : base (ctx, R_Status_ID, trxName)
{
/** if (R_Status_ID == 0)
{
SetIsClosed (false);	// N
SetIsDefault (false);
SetIsFinalClose (false);	// N
SetIsOpen (false);
SetIsWebCanUpdate (false);
SetName (null);
SetR_StatusCategory_ID (0);
SetR_Status_ID (0);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM R_Status WHERE R_StatusCategory_ID=@R_StatusCategory_ID@
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Status (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Status (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Status (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_Status()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383471L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066682L;
/** AD_Table_ID=776 */
public static int Table_ID;
 // =776;

/** TableName=R_Status */
public static String Table_Name="R_Status";

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
StringBuilder sb = new StringBuilder ("X_R_Status[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Closed Status.
@param IsClosed The status is closed */
public void SetIsClosed (Boolean IsClosed)
{
Set_Value ("IsClosed", IsClosed);
}
/** Get Closed Status.
@return The status is closed */
public Boolean IsClosed() 
{
Object oo = Get_Value("IsClosed");
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
/** Set Final Close.
@param IsFinalClose Entries with Final Close cannot be re-opened */
public void SetIsFinalClose (Boolean IsFinalClose)
{
Set_Value ("IsFinalClose", IsFinalClose);
}
/** Get Final Close.
@return Entries with Final Close cannot be re-opened */
public Boolean IsFinalClose() 
{
Object oo = Get_Value("IsFinalClose");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Open Status.
@param IsOpen The status is closed */
public void SetIsOpen (Boolean IsOpen)
{
Set_Value ("IsOpen", IsOpen);
}
/** Get Open Status.
@return The status is closed */
public Boolean IsOpen() 
{
Object oo = Get_Value("IsOpen");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Web Can Update.
@param IsWebCanUpdate Entry can be updated from the Web */
public void SetIsWebCanUpdate (Boolean IsWebCanUpdate)
{
Set_Value ("IsWebCanUpdate", IsWebCanUpdate);
}
/** Get Web Can Update.
@return Entry can be updated from the Web */
public Boolean IsWebCanUpdate() 
{
Object oo = Get_Value("IsWebCanUpdate");
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

/** Next_Status_ID AD_Reference_ID=345 */
public static int NEXT_STATUS_ID_AD_Reference_ID=345;
/** Set Next Status.
@param Next_Status_ID Move to next status automatically after timeout */
public void SetNext_Status_ID (int Next_Status_ID)
{
if (Next_Status_ID <= 0) Set_Value ("Next_Status_ID", null);
else
Set_Value ("Next_Status_ID", Next_Status_ID);
}
/** Get Next Status.
@return Move to next status automatically after timeout */
public int GetNext_Status_ID() 
{
Object ii = Get_Value("Next_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status Category.
@param R_StatusCategory_ID Request Status Category */
public void SetR_StatusCategory_ID (int R_StatusCategory_ID)
{
if (R_StatusCategory_ID < 1) throw new ArgumentException ("R_StatusCategory_ID is mandatory.");
Set_ValueNoCheck ("R_StatusCategory_ID", R_StatusCategory_ID);
}
/** Get Status Category.
@return Request Status Category */
public int GetR_StatusCategory_ID() 
{
Object ii = Get_Value("R_StatusCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status.
@param R_Status_ID Request Status */
public void SetR_Status_ID (int R_Status_ID)
{
if (R_Status_ID < 1) throw new ArgumentException ("R_Status_ID is mandatory.");
Set_ValueNoCheck ("R_Status_ID", R_Status_ID);
}
/** Get Status.
@return Request Status */
public int GetR_Status_ID() 
{
Object ii = Get_Value("R_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetSeqNo().ToString());
}
/** Set Timeout in Days.
@param TimeoutDays Timeout in Days to change Status automatically */
public void SetTimeoutDays (int TimeoutDays)
{
Set_Value ("TimeoutDays", TimeoutDays);
}
/** Get Timeout in Days.
@return Timeout in Days to change Status automatically */
public int GetTimeoutDays() 
{
Object ii = Get_Value("TimeoutDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Update_Status_ID AD_Reference_ID=345 */
public static int UPDATE_STATUS_ID_AD_Reference_ID=345;
/** Set Update Status.
@param Update_Status_ID Automatically change the status after entry from web */
public void SetUpdate_Status_ID (int Update_Status_ID)
{
if (Update_Status_ID <= 0) Set_Value ("Update_Status_ID", null);
else
Set_Value ("Update_Status_ID", Update_Status_ID);
}
/** Get Update Status.
@return Automatically change the status after entry from web */
public int GetUpdate_Status_ID() 
{
Object ii = Get_Value("Update_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
