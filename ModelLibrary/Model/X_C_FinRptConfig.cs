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
/** Generated Model for C_FinRptConfig
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_FinRptConfig : PO
{
    public X_C_FinRptConfig(Context ctx, int C_FinRptConfig_ID, Trx trxName)
        : base(ctx, C_FinRptConfig_ID, trxName)
{
/** if (C_FinRptConfig_ID == 0)
{
SetC_AccountGroupBatch_ID (0);	// 1
SetC_FinRptConfig_ID (0);
SetC_ReportName (null);
}
 */
}
    public X_C_FinRptConfig(Ctx ctx, int C_FinRptConfig_ID, Trx trxName)
        : base(ctx, C_FinRptConfig_ID, trxName)
{
/** if (C_FinRptConfig_ID == 0)
{
SetC_AccountGroupBatch_ID (0);	// 1
SetC_FinRptConfig_ID (0);
SetC_ReportName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptConfig(Context ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptConfig(Ctx ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_C_FinRptConfig(Ctx ctx, IDataReader dr, Trx trxName)
        : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_FinRptConfig()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27692108111059L;
/** Last Updated Timestamp 9/6/2014 11:23:14 AM */
public static long updatedMS = 1409982794270L;
/** AD_Table_ID=1000488 */
public static int Table_ID;
 // =1000488;

/** TableName=C_FinRptConfig */
public static String Table_Name="C_FinRptConfig";

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
StringBuilder sb = new StringBuilder ("X_C_FinRptConfig[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_AccountGroupBatch_ID.
@param C_AccountGroupBatch_ID C_AccountGroupBatch_ID */
public void SetC_AccountGroupBatch_ID (int C_AccountGroupBatch_ID)
{
if (C_AccountGroupBatch_ID < 1) throw new ArgumentException ("C_AccountGroupBatch_ID is mandatory.");
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
/** Set Copy  all groups.
@param C_CopyAllGroups Copy  all groups */
public void SetC_CopyAllGroups (String C_CopyAllGroups)
{
if (C_CopyAllGroups != null && C_CopyAllGroups.Length > 5)
{
log.Warning("Length > 5 - truncated");
C_CopyAllGroups = C_CopyAllGroups.Substring(0,5);
}
Set_Value ("C_CopyAllGroups", C_CopyAllGroups);
}
/** Get Copy  all groups.
@return Copy  all groups */
public String GetC_CopyAllGroups() 
{
return (String)Get_Value("C_CopyAllGroups");
}
/** Set C_FinRptConfig_ID.
@param C_FinRptConfig_ID C_FinRptConfig_ID */
public void SetC_FinRptConfig_ID (int C_FinRptConfig_ID)
{
if (C_FinRptConfig_ID < 1) throw new ArgumentException ("C_FinRptConfig_ID is mandatory.");
Set_ValueNoCheck ("C_FinRptConfig_ID", C_FinRptConfig_ID);
}
/** Get C_FinRptConfig_ID.
@return C_FinRptConfig_ID */
public int GetC_FinRptConfig_ID() 
{
Object ii = Get_Value("C_FinRptConfig_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Generate Group & Subgroup.
@param C_GenerateGroupSubgroup Generate Group & Subgroup */
public void SetC_GenerateGroupSubgroup (String C_GenerateGroupSubgroup)
{
if (C_GenerateGroupSubgroup != null && C_GenerateGroupSubgroup.Length > 5)
{
log.Warning("Length > 5 - truncated");
C_GenerateGroupSubgroup = C_GenerateGroupSubgroup.Substring(0,5);
}
Set_Value ("C_GenerateGroupSubgroup", C_GenerateGroupSubgroup);
}
/** Get Generate Group & Subgroup.
@return Generate Group & Subgroup */
public String GetC_GenerateGroupSubgroup() 
{
return (String)Get_Value("C_GenerateGroupSubgroup");
}
/** Set Report Description.
@param C_ReportDescription Report Description */
public void SetC_ReportDescription (String C_ReportDescription)
{
if (C_ReportDescription != null && C_ReportDescription.Length > 100)
{
log.Warning("Length > 100 - truncated");
C_ReportDescription = C_ReportDescription.Substring(0,100);
}
Set_Value ("C_ReportDescription", C_ReportDescription);
}
/** Get Report Description.
@return Report Description */
public String GetC_ReportDescription() 
{
return (String)Get_Value("C_ReportDescription");
}
/** Set Report Name.
@param C_ReportName Report Name */
public void SetC_ReportName (String C_ReportName)
{
if (C_ReportName == null) throw new ArgumentException ("C_ReportName is mandatory.");
if (C_ReportName.Length > 50)
{
log.Warning("Length > 50 - truncated");
C_ReportName = C_ReportName.Substring(0,50);
}
Set_Value ("C_ReportName", C_ReportName);
}
/** Get Report Name.
@return Report Name */
public String GetC_ReportName() 
{
return (String)Get_Value("C_ReportName");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_ReportName());
}

/** C_ReportType AD_Reference_ID=1000156 */
public static int C_REPORTTYPE_AD_Reference_ID=1000156;
/** Balance Sheet = B */
public static String C_REPORTTYPE_BalanceSheet = "B";
/** Cashflow Statement = C */
public static String C_REPORTTYPE_CashflowStatement = "C";
/** Others = O */
public static String C_REPORTTYPE_Others = "O";
/** Profit & Loss = P */
public static String C_REPORTTYPE_ProfitLoss = "P";
/** Statement of Accounts = S */
public static String C_REPORTTYPE_StatementOfAccounts = "S";
/** Trial Balance = T */
public static String C_REPORTTYPE_TrialBalance = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsC_ReportTypeValid (String test)
{
return test == null || test.Equals("B") || test.Equals("C") || test.Equals("O") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Report Type.
@param C_ReportType Report Type */
public void SetC_ReportType (String C_ReportType)
{
if (!IsC_ReportTypeValid(C_ReportType))
throw new ArgumentException ("C_ReportType Invalid value - " + C_ReportType + " - Reference_ID=1000156 - B - C - O - P - S - T");
if (C_ReportType != null && C_ReportType.Length > 1)
{
log.Warning("Length > 1 - truncated");
C_ReportType = C_ReportType.Substring(0,1);
}
Set_Value ("C_ReportType", C_ReportType);
}
/** Get Report Type.
@return Report Type */
public String GetC_ReportType() 
{
return (String)Get_Value("C_ReportType");
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
}

}
