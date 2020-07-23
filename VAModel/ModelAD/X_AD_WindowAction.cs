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
/** Generated Model for AD_WindowAction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WindowAction : PO
{
public X_AD_WindowAction (Context ctx, int AD_WindowAction_ID, Trx trxName) : base (ctx, AD_WindowAction_ID, trxName)
{
/** if (AD_WindowAction_ID == 0)
{
SetAD_WindowAction_ID (0);
SetAD_Window_ID (0);
}
 */
}
public X_AD_WindowAction (Ctx ctx, int AD_WindowAction_ID, Trx trxName) : base (ctx, AD_WindowAction_ID, trxName)
{
/** if (AD_WindowAction_ID == 0)
{
SetAD_WindowAction_ID (0);
SetAD_Window_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowAction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowAction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowAction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WindowAction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27645730675536L;
/** Last Updated Timestamp 18-03-2013 16:45:58 */
public static long updatedMS = 1363605358747L;
/** AD_Table_ID=1000331 */
public static int Table_ID;
 // =1000331;

/** TableName=AD_WindowAction */
public static String Table_Name="AD_WindowAction";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_WindowAction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_WindowAction_ID.
@param AD_WindowAction_ID AD_WindowAction_ID */
public void SetAD_WindowAction_ID (int AD_WindowAction_ID)
{
if (AD_WindowAction_ID < 1) throw new ArgumentException ("AD_WindowAction_ID is mandatory.");
Set_ValueNoCheck ("AD_WindowAction_ID", AD_WindowAction_ID);
}
/** Get AD_WindowAction_ID.
@return AD_WindowAction_ID */
public int GetAD_WindowAction_ID() 
{
Object ii = Get_Value("AD_WindowAction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_ValueNoCheck ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_ValueNoCheck ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set IsAppointment.
@param IsAppointment IsAppointment */
public void SetIsAppointment (Boolean IsAppointment)
{
Set_Value ("IsAppointment", IsAppointment);
}
/** Get IsAppointment.
@return IsAppointment */
public Boolean IsAppointment() 
{
Object oo = Get_Value("IsAppointment");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsEmail.
@param IsEmail IsEmail */
public void SetIsEmail (Boolean IsEmail)
{
Set_Value ("IsEmail", IsEmail);
}
/** Get IsEmail.
@return IsEmail */
public Boolean IsEmail() 
{
Object oo = Get_Value("IsEmail");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Fax Email.
@param IsFaxEmail Fax Email */
public void SetIsFaxEmail (Boolean IsFaxEmail)
{
Set_Value ("IsFaxEmail", IsFaxEmail);
}
/** Get Fax Email.
@return Fax Email */
public Boolean IsFaxEmail() 
{
Object oo = Get_Value("IsFaxEmail");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsLetter.
@param IsLetter IsLetter */
public void SetIsLetter (Boolean IsLetter)
{
Set_Value ("IsLetter", IsLetter);
}
/** Get IsLetter.
@return IsLetter */
public Boolean IsLetter() 
{
Object oo = Get_Value("IsLetter");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsSms.
@param IsSms IsSms */
public void SetIsSms (Boolean IsSms)
{
Set_Value ("IsSms", IsSms);
}
/** Get IsSms.
@return IsSms */
public Boolean IsSms() 
{
Object oo = Get_Value("IsSms");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsTask.
@param IsTask IsTask */
public void SetIsTask (Boolean IsTask)
{
Set_Value ("IsTask", IsTask);
}
/** Get IsTask.
@return IsTask */
public Boolean IsTask() 
{
Object oo = Get_Value("IsTask");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
