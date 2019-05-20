namespace ViennaAdvantage.Model
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
/** Generated Model for C_AccountGroup
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AccountGroup : PO
{
public X_C_AccountGroup (Context ctx, int C_AccountGroup_ID, Trx trxName) : base (ctx, C_AccountGroup_ID, trxName)
{
/** if (C_AccountGroup_ID == 0)
{
SetC_AccountGroup_ID (0);
SetName (null);
}
 */
}
public X_C_AccountGroup (Ctx ctx, int C_AccountGroup_ID, Trx trxName) : base (ctx, C_AccountGroup_ID, trxName)
{
/** if (C_AccountGroup_ID == 0)
{
SetC_AccountGroup_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AccountGroup()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27640875240292L;
/** Last Updated Timestamp 1/21/2013 12:02:03 PM */
public static long updatedMS = 1358749923503L;
/** AD_Table_ID=1000374 */
public static int Table_ID;
 // =1000374;

/** TableName=C_AccountGroup */
public static String Table_Name="C_AccountGroup";

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
StringBuilder sb = new StringBuilder ("X_C_AccountGroup[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_AccountGroupBatch_ID.
@param C_AccountGroupBatch_ID C_AccountGroupBatch_ID */
public void SetC_AccountGroupBatch_ID (int C_AccountGroupBatch_ID)
{
if (C_AccountGroupBatch_ID <= 0) Set_Value ("C_AccountGroupBatch_ID", null);
else
Set_Value ("C_AccountGroupBatch_ID", C_AccountGroupBatch_ID);
}
/** Get C_AccountGroupBatch_ID.
@return C_AccountGroupBatch_ID */
public int GetC_AccountGroupBatch_ID() 
{
Object ii = Get_Value("C_AccountGroupBatch_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_AccountGroup_ID.
@param C_AccountGroup_ID C_AccountGroup_ID */
public void SetC_AccountGroup_ID (int C_AccountGroup_ID)
{
if (C_AccountGroup_ID < 1) throw new ArgumentException ("C_AccountGroup_ID is mandatory.");
Set_ValueNoCheck ("C_AccountGroup_ID", C_AccountGroup_ID);
}
/** Get C_AccountGroup_ID.
@return C_AccountGroup_ID */
public int GetC_AccountGroup_ID() 
{
Object ii = Get_Value("C_AccountGroup_ID");
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
/** Set Sub Group.
@param HasSubGroup Sub Group */
public void SetHasSubGroup (Boolean HasSubGroup)
{
Set_Value ("HasSubGroup", HasSubGroup);
}
/** Get Sub Group.
@return Sub Group */
public Boolean IsHasSubGroup() 
{
Object oo = Get_Value("HasSubGroup");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set For New Tenant.
@param IsForNewTenant For New Tenant */
public void SetIsForNewTenant (Boolean IsForNewTenant)
{
Set_Value ("IsForNewTenant", IsForNewTenant);
}
/** Get For New Tenant.
@return For New Tenant */
public Boolean IsForNewTenant() 
{
Object oo = Get_Value("IsForNewTenant");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Memo Group.
@param IsMemoGroup Memo Group */
public void SetIsMemoGroup (Boolean IsMemoGroup)
{
Set_Value ("IsMemoGroup", IsMemoGroup);
}
/** Get Memo Group.
@return Memo Group */
public Boolean IsMemoGroup() 
{
Object oo = Get_Value("IsMemoGroup");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Name */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 200)
{
log.Warning("Length > 200 - truncated");
Name = Name.Substring(0,200);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Name */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName != null && PrintName.Length > 200)
{
log.Warning("Length > 200 - truncated");
PrintName = PrintName.Substring(0,200);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
/** Set Show In Balance Sheet.
@param ShowInBalanceSheet Show In Balance Sheet */
public void SetShowInBalanceSheet (Boolean ShowInBalanceSheet)
{
Set_Value ("ShowInBalanceSheet", ShowInBalanceSheet);
}
/** Get Show In Balance Sheet.
@return Show In Balance Sheet */
public Boolean IsShowInBalanceSheet() 
{
Object oo = Get_Value("ShowInBalanceSheet");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show In Cash Flow.
@param ShowInCashFlow Show In Cash Flow */
public void SetShowInCashFlow (Boolean ShowInCashFlow)
{
Set_Value ("ShowInCashFlow", ShowInCashFlow);
}
/** Get Show In Cash Flow.
@return Show In Cash Flow */
public Boolean IsShowInCashFlow() 
{
Object oo = Get_Value("ShowInCashFlow");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show In Profit Loss.
@param ShowInProfitLoss Show In Profit Loss */
public void SetShowInProfitLoss (Boolean ShowInProfitLoss)
{
Set_Value ("ShowInProfitLoss", ShowInProfitLoss);
}
/** Get Show In Profit Loss.
@return Show In Profit Loss */
public Boolean IsShowInProfitLoss() 
{
Object oo = Get_Value("ShowInProfitLoss");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 50)
{
log.Warning("Length > 50 - truncated");
Value = Value.Substring(0,50);
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
