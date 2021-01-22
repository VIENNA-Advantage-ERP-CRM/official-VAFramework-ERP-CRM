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
/** Generated Model for VAF_ScreenAction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ScreenAction : PO
{
public X_VAF_ScreenAction (Context ctx, int VAF_ScreenAction_ID, Trx trxName) : base (ctx, VAF_ScreenAction_ID, trxName)
{
/** if (VAF_ScreenAction_ID == 0)
{
SetVAF_ScreenAction_ID (0);
SetVAF_Screen_ID (0);
}
 */
}
public X_VAF_ScreenAction (Ctx ctx, int VAF_ScreenAction_ID, Trx trxName) : base (ctx, VAF_ScreenAction_ID, trxName)
{
/** if (VAF_ScreenAction_ID == 0)
{
SetVAF_ScreenAction_ID (0);
SetVAF_Screen_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenAction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenAction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenAction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ScreenAction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27645730675536L;
/** Last Updated Timestamp 18-03-2013 16:45:58 */
public static long updatedMS = 1363605358747L;
/** VAF_TableView_ID=1000331 */
public static int Table_ID;
 // =1000331;

/** TableName=VAF_ScreenAction */
public static String Table_Name="VAF_ScreenAction";

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
StringBuilder sb = new StringBuilder ("X_VAF_ScreenAction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAF_ScreenAction_ID.
@param VAF_ScreenAction_ID VAF_ScreenAction_ID */
public void SetVAF_ScreenAction_ID (int VAF_ScreenAction_ID)
{
if (VAF_ScreenAction_ID < 1) throw new ArgumentException ("VAF_ScreenAction_ID is mandatory.");
Set_ValueNoCheck ("VAF_ScreenAction_ID", VAF_ScreenAction_ID);
}
/** Get VAF_ScreenAction_ID.
@return VAF_ScreenAction_ID */
public int GetVAF_ScreenAction_ID() 
{
Object ii = Get_Value("VAF_ScreenAction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param VAF_Screen_ID Data entry or display window */
public void SetVAF_Screen_ID (int VAF_Screen_ID)
{
if (VAF_Screen_ID < 1) throw new ArgumentException ("VAF_Screen_ID is mandatory.");
Set_ValueNoCheck ("VAF_Screen_ID", VAF_Screen_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetVAF_Screen_ID() 
{
Object ii = Get_Value("VAF_Screen_ID");
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
