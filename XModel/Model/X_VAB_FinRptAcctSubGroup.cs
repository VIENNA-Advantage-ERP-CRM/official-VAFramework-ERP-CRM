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
/** Generated Model for VAB_FinRptAcctSubGroup
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_FinRptAcctSubGroup : PO
{
    public X_VAB_FinRptAcctSubGroup(Context ctx, int VAB_FinRptAcctSubGroup_ID, Trx trxName)
        : base(ctx, VAB_FinRptAcctSubGroup_ID, trxName)
{
/** if (VAB_FinRptAcctSubGroup_ID == 0)
{
SetVAB_AccountSubGroup_ID (0);
SetVAB_FinRptAcctGroup_ID (0);
SetVAB_FinRptAcctSubGroup_ID (0);
}
 */
}
    public X_VAB_FinRptAcctSubGroup(Ctx ctx, int VAB_FinRptAcctSubGroup_ID, Trx trxName)
        : base(ctx, VAB_FinRptAcctSubGroup_ID, trxName)
{
/** if (VAB_FinRptAcctSubGroup_ID == 0)
{
SetVAB_AccountSubGroup_ID (0);
SetVAB_FinRptAcctGroup_ID (0);
SetVAB_FinRptAcctSubGroup_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAB_FinRptAcctSubGroup(Context ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAB_FinRptAcctSubGroup(Ctx ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAB_FinRptAcctSubGroup(Ctx ctx, IDataReader dr, Trx trxName)
        : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_FinRptAcctSubGroup()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27692114875238L;
/** Last Updated Timestamp 9/6/2014 1:15:58 PM */
public static long updatedMS = 1409989558449L;
/** VAF_TableView_ID=1000490 */
public static int Table_ID;
 // =1000490;

/** TableName=VAB_FinRptAcctSubGroup */
public static String Table_Name="VAB_FinRptAcctSubGroup";

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
StringBuilder sb = new StringBuilder ("X_VAB_FinRptAcctSubGroup[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account Sub Group.
@param VAB_AccountSubGroup_ID Account Sub Group */
public void SetVAB_AccountSubGroup_ID (int VAB_AccountSubGroup_ID)
{
if (VAB_AccountSubGroup_ID < 1) throw new ArgumentException ("VAB_AccountSubGroup_ID is mandatory.");
Set_Value ("VAB_AccountSubGroup_ID", VAB_AccountSubGroup_ID);
}
/** Get Account Sub Group.
@return Account Sub Group */
public int GetVAB_AccountSubGroup_ID() 
{
Object ii = Get_Value("VAB_AccountSubGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Group.
@param VAB_FinRptAcctGroup_ID Account Group */
public void SetVAB_FinRptAcctGroup_ID (int VAB_FinRptAcctGroup_ID)
{
if (VAB_FinRptAcctGroup_ID < 1) throw new ArgumentException ("VAB_FinRptAcctGroup_ID is mandatory.");
Set_ValueNoCheck ("VAB_FinRptAcctGroup_ID", VAB_FinRptAcctGroup_ID);
}
/** Get Account Group.
@return Account Group */
public int GetVAB_FinRptAcctGroup_ID() 
{
Object ii = Get_Value("VAB_FinRptAcctGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_FinRptAcctSubGroup_ID.
@param VAB_FinRptAcctSubGroup_ID VAB_FinRptAcctSubGroup_ID */
public void SetVAB_FinRptAcctSubGroup_ID (int VAB_FinRptAcctSubGroup_ID)
{
if (VAB_FinRptAcctSubGroup_ID < 1) throw new ArgumentException ("VAB_FinRptAcctSubGroup_ID is mandatory.");
Set_ValueNoCheck ("VAB_FinRptAcctSubGroup_ID", VAB_FinRptAcctSubGroup_ID);
}
/** Get VAB_FinRptAcctSubGroup_ID.
@return VAB_FinRptAcctSubGroup_ID */
public int GetVAB_FinRptAcctSubGroup_ID() 
{
Object ii = Get_Value("VAB_FinRptAcctSubGroup_ID");
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
/** Set Line No.
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
/** Get Line No.
@return Unique line for this document */
public String GetLine() 
{
return (String)Get_Value("Line");
}
}

}
