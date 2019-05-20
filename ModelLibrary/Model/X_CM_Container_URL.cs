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
/** Generated Model for CM_Container_URL
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Container_URL : PO
{
public X_CM_Container_URL (Context ctx, int CM_Container_URL_ID, Trx trxName) : base (ctx, CM_Container_URL_ID, trxName)
{
/** if (CM_Container_URL_ID == 0)
{
SetCM_Container_ID (0);
SetCM_Container_URL_ID (0);
SetChecked (DateTime.Now);
SetLast_Result (null);
SetStatus (null);
}
 */
}
public X_CM_Container_URL (Ctx ctx, int CM_Container_URL_ID, Trx trxName) : base (ctx, CM_Container_URL_ID, trxName)
{
/** if (CM_Container_URL_ID == 0)
{
SetCM_Container_ID (0);
SetCM_Container_URL_ID (0);
SetChecked (DateTime.Now);
SetLast_Result (null);
SetStatus (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_URL (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_URL (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_URL (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Container_URL()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368990L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052201L;
/** AD_Table_ID=865 */
public static int Table_ID;
 // =865;

/** TableName=CM_Container_URL */
public static String Table_Name="CM_Container_URL";

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
StringBuilder sb = new StringBuilder ("X_CM_Container_URL[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Web Container.
@param CM_Container_ID Web Container contains content like images, text etc. */
public void SetCM_Container_ID (int CM_Container_ID)
{
if (CM_Container_ID < 1) throw new ArgumentException ("CM_Container_ID is mandatory.");
Set_Value ("CM_Container_ID", CM_Container_ID);
}
/** Get Web Container.
@return Web Container contains content like images, text etc. */
public int GetCM_Container_ID() 
{
Object ii = Get_Value("CM_Container_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Container URL.
@param CM_Container_URL_ID Contains info on used URLs */
public void SetCM_Container_URL_ID (int CM_Container_URL_ID)
{
if (CM_Container_URL_ID < 1) throw new ArgumentException ("CM_Container_URL_ID is mandatory.");
Set_ValueNoCheck ("CM_Container_URL_ID", CM_Container_URL_ID);
}
/** Get Container URL.
@return Contains info on used URLs */
public int GetCM_Container_URL_ID() 
{
Object ii = Get_Value("CM_Container_URL_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Last Checked.
@param Checked Info when we did the latest check */
public void SetChecked (DateTime? Checked)
{
if (Checked == null) throw new ArgumentException ("Checked is mandatory.");
Set_Value ("Checked", (DateTime?)Checked);
}
/** Get Last Checked.
@return Info when we did the latest check */
public DateTime? GetChecked() 
{
return (DateTime?)Get_Value("Checked");
}
/** Set Last Result.
@param Last_Result Contains data on the last check result */
public void SetLast_Result (String Last_Result)
{
if (Last_Result == null) throw new ArgumentException ("Last_Result is mandatory.");
if (Last_Result.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Last_Result = Last_Result.Substring(0,2000);
}
Set_Value ("Last_Result", Last_Result);
}
/** Get Last Result.
@return Contains data on the last check result */
public String GetLast_Result() 
{
return (String)Get_Value("Last_Result");
}
/** Set Status.
@param Status Status of the currently running check */
public void SetStatus (String Status)
{
if (Status == null) throw new ArgumentException ("Status is mandatory.");
if (Status.Length > 2)
{
log.Warning("Length > 2 - truncated");
Status = Status.Substring(0,2);
}
Set_Value ("Status", Status);
}
/** Get Status.
@return Status of the currently running check */
public String GetStatus() 
{
return (String)Get_Value("Status");
}
}

}
