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
/** Generated Model for AD_WorkbenchWindow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WorkbenchWindow : PO
{
public X_AD_WorkbenchWindow (Context ctx, int AD_WorkbenchWindow_ID, Trx trxName) : base (ctx, AD_WorkbenchWindow_ID, trxName)
{
/** if (AD_WorkbenchWindow_ID == 0)
{
SetAD_WorkbenchWindow_ID (0);
SetAD_Workbench_ID (0);
SetEntityType (null);	// U
SetIsPrimary (false);
SetSeqNo (0);
}
 */
}
public X_AD_WorkbenchWindow (Ctx ctx, int AD_WorkbenchWindow_ID, Trx trxName) : base (ctx, AD_WorkbenchWindow_ID, trxName)
{
/** if (AD_WorkbenchWindow_ID == 0)
{
SetAD_WorkbenchWindow_ID (0);
SetAD_Workbench_ID (0);
SetEntityType (null);	// U
SetIsPrimary (false);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WorkbenchWindow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WorkbenchWindow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WorkbenchWindow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WorkbenchWindow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366796L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050007L;
/** AD_Table_ID=469 */
public static int Table_ID;
 // =469;

/** TableName=AD_WorkbenchWindow */
public static String Table_Name="AD_WorkbenchWindow";

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
StringBuilder sb = new StringBuilder ("X_AD_WorkbenchWindow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID <= 0) Set_Value ("AD_Form_ID", null);
else
Set_Value ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param AD_Process_ID Process or Report */
public void SetAD_Process_ID (int AD_Process_ID)
{
if (AD_Process_ID <= 0) Set_Value ("AD_Process_ID", null);
else
Set_Value ("AD_Process_ID", AD_Process_ID);
}
/** Get Process.
@return Process or Report */
public int GetAD_Process_ID() 
{
Object ii = Get_Value("AD_Process_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set OS Task.
@param AD_Task_ID Operation System Task */
public void SetAD_Task_ID (int AD_Task_ID)
{
if (AD_Task_ID <= 0) Set_Value ("AD_Task_ID", null);
else
Set_Value ("AD_Task_ID", AD_Task_ID);
}
/** Get OS Task.
@return Operation System Task */
public int GetAD_Task_ID() 
{
Object ii = Get_Value("AD_Task_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);
else
Set_Value ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workbench Window.
@param AD_WorkbenchWindow_ID Workbench Window */
public void SetAD_WorkbenchWindow_ID (int AD_WorkbenchWindow_ID)
{
if (AD_WorkbenchWindow_ID < 1) throw new ArgumentException ("AD_WorkbenchWindow_ID is mandatory.");
Set_ValueNoCheck ("AD_WorkbenchWindow_ID", AD_WorkbenchWindow_ID);
}
/** Get Workbench Window.
@return Workbench Window */
public int GetAD_WorkbenchWindow_ID() 
{
Object ii = Get_Value("AD_WorkbenchWindow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_WorkbenchWindow_ID().ToString());
}
/** Set Workbench.
@param AD_Workbench_ID Collection of windows, reports */
public void SetAD_Workbench_ID (int AD_Workbench_ID)
{
if (AD_Workbench_ID < 1) throw new ArgumentException ("AD_Workbench_ID is mandatory.");
Set_ValueNoCheck ("AD_Workbench_ID", AD_Workbench_ID);
}
/** Get Workbench.
@return Collection of windows, reports */
public int GetAD_Workbench_ID() 
{
Object ii = Get_Value("AD_Workbench_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Primary.
@param IsPrimary Indicates if this is the primary budget */
public void SetIsPrimary (Boolean IsPrimary)
{
Set_Value ("IsPrimary", IsPrimary);
}
/** Get Primary.
@return Indicates if this is the primary budget */
public Boolean IsPrimary() 
{
Object oo = Get_Value("IsPrimary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}
