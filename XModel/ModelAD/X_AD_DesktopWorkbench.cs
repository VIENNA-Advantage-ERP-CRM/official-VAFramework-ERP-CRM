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
/** Generated Model for AD_DesktopWorkbench
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_DesktopWorkbench : PO
{
public X_AD_DesktopWorkbench (Context ctx, int AD_DesktopWorkbench_ID, Trx trxName) : base (ctx, AD_DesktopWorkbench_ID, trxName)
{
/** if (AD_DesktopWorkbench_ID == 0)
{
SetAD_DesktopWorkbench_ID (0);
SetAD_Desktop_ID (0);
SetAD_Workbench_ID (0);
SetSeqNo (0);
}
 */
}
public X_AD_DesktopWorkbench (Ctx ctx, int AD_DesktopWorkbench_ID, Trx trxName) : base (ctx, AD_DesktopWorkbench_ID, trxName)
{
/** if (AD_DesktopWorkbench_ID == 0)
{
SetAD_DesktopWorkbench_ID (0);
SetAD_Desktop_ID (0);
SetAD_Workbench_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_DesktopWorkbench (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_DesktopWorkbench (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_DesktopWorkbench (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_DesktopWorkbench()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361122L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044333L;
/** AD_Table_ID=459 */
public static int Table_ID;
 // =459;

/** TableName=AD_DesktopWorkbench */
public static String Table_Name="AD_DesktopWorkbench";

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
StringBuilder sb = new StringBuilder ("X_AD_DesktopWorkbench[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Desktop Workbench.
@param AD_DesktopWorkbench_ID Desktop Workbench */
public void SetAD_DesktopWorkbench_ID (int AD_DesktopWorkbench_ID)
{
if (AD_DesktopWorkbench_ID < 1) throw new ArgumentException ("AD_DesktopWorkbench_ID is mandatory.");
Set_ValueNoCheck ("AD_DesktopWorkbench_ID", AD_DesktopWorkbench_ID);
}
/** Get Desktop Workbench.
@return Desktop Workbench */
public int GetAD_DesktopWorkbench_ID() 
{
Object ii = Get_Value("AD_DesktopWorkbench_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Desktop.
@param AD_Desktop_ID Collection of Workbenches */
public void SetAD_Desktop_ID (int AD_Desktop_ID)
{
if (AD_Desktop_ID < 1) throw new ArgumentException ("AD_Desktop_ID is mandatory.");
Set_ValueNoCheck ("AD_Desktop_ID", AD_Desktop_ID);
}
/** Get Desktop.
@return Collection of Workbenches */
public int GetAD_Desktop_ID() 
{
Object ii = Get_Value("AD_Desktop_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workbench.
@param AD_Workbench_ID Collection of windows, reports */
public void SetAD_Workbench_ID (int AD_Workbench_ID)
{
if (AD_Workbench_ID < 1) throw new ArgumentException ("AD_Workbench_ID is mandatory.");
Set_Value ("AD_Workbench_ID", AD_Workbench_ID);
}
/** Get Workbench.
@return Collection of windows, reports */
public int GetAD_Workbench_ID() 
{
Object ii = Get_Value("AD_Workbench_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Workbench_ID().ToString());
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
