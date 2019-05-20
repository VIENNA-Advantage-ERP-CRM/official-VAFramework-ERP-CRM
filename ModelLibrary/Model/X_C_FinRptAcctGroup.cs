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
/** Generated Model for C_FinRptAcctGroup
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_FinRptAcctGroup : PO
{
    public X_C_FinRptAcctGroup(Context ctx, int C_FinRptAcctGroup_ID, Trx trxName)
        : base(ctx, C_FinRptAcctGroup_ID, trxName)
{
/** if (C_FinRptAcctGroup_ID == 0)
{
SetC_FinRptAcctGroup_ID (0);
SetC_FinRptConfig_ID (0);
}
 */
}
    public X_C_FinRptAcctGroup(Ctx ctx, int C_FinRptAcctGroup_ID, Trx trxName)
        : base(ctx, C_FinRptAcctGroup_ID, trxName)
{
/** if (C_FinRptAcctGroup_ID == 0)
{
SetC_FinRptAcctGroup_ID (0);
SetC_FinRptConfig_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptAcctGroup(Context ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptAcctGroup(Ctx ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptAcctGroup(Ctx ctx, IDataReader dr, Trx trxName)
        : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_FinRptAcctGroup()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27692114860046L;
/** Last Updated Timestamp 9/6/2014 1:15:43 PM */
public static long updatedMS = 1409989543257L;
/** AD_Table_ID=1000489 */
public static int Table_ID;
 // =1000489;

/** TableName=C_FinRptAcctGroup */
public static String Table_Name="C_FinRptAcctGroup";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_FinRptAcctGroup[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account Group.
@param C_AccountGroup_ID Account Group */
public void SetC_AccountGroup_ID (int C_AccountGroup_ID)
{
if (C_AccountGroup_ID <= 0) Set_Value ("C_AccountGroup_ID", null);
else
Set_Value ("C_AccountGroup_ID", C_AccountGroup_ID);
}
/** Get Account Group.
@return Account Group */
public int GetC_AccountGroup_ID() 
{
Object ii = Get_Value("C_AccountGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_FinRptAcctGroup_ID.
@param C_FinRptAcctGroup_ID C_FinRptAcctGroup_ID */
public void SetC_FinRptAcctGroup_ID (int C_FinRptAcctGroup_ID)
{
if (C_FinRptAcctGroup_ID < 1) throw new ArgumentException ("C_FinRptAcctGroup_ID is mandatory.");
Set_ValueNoCheck ("C_FinRptAcctGroup_ID", C_FinRptAcctGroup_ID);
}
/** Get C_FinRptAcctGroup_ID.
@return C_FinRptAcctGroup_ID */
public int GetC_FinRptAcctGroup_ID() 
{
Object ii = Get_Value("C_FinRptAcctGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report.
@param C_FinRptConfig_ID Report */
public void SetC_FinRptConfig_ID (int C_FinRptConfig_ID)
{
if (C_FinRptConfig_ID < 1) throw new ArgumentException ("C_FinRptConfig_ID is mandatory.");
Set_ValueNoCheck ("C_FinRptConfig_ID", C_FinRptConfig_ID);
}
/** Get Report.
@return Report */
public int GetC_FinRptConfig_ID() 
{
Object ii = Get_Value("C_FinRptConfig_ID");
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
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Sequence No.
@param Line Unique line for this document */
public void SetLine (String Line)
{
if (Line != null && Line.Length > 14)
{
log.Warning("Length > 14 - truncated");
Line = Line.Substring(0,14);
}
Set_Value ("Line", Line);
}
/** Get Sequence No.
@return Unique line for this document */
public String GetLine() 
{
return (String)Get_Value("Line");
}
}

}
