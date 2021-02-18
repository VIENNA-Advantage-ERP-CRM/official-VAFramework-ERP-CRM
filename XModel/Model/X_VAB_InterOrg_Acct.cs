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
/** Generated Model for VAB_InterOrg_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_InterOrg_Acct : PO
{
public X_VAB_InterOrg_Acct (Context ctx, int VAB_InterOrg_Acct_ID, Trx trxName) : base (ctx, VAB_InterOrg_Acct_ID, trxName)
{
/** if (VAB_InterOrg_Acct_ID == 0)
{
SetVAF_OrgTo_ID (0);
SetVAB_AccountBook_ID (0);
SetIntercompanyDueFrom_Acct (0);
SetIntercompanyDueTo_Acct (0);
}
 */
}
public X_VAB_InterOrg_Acct (Ctx ctx, int VAB_InterOrg_Acct_ID, Trx trxName) : base (ctx, VAB_InterOrg_Acct_ID, trxName)
{
/** if (VAB_InterOrg_Acct_ID == 0)
{
SetVAF_OrgTo_ID (0);
SetVAB_AccountBook_ID (0);
SetIntercompanyDueFrom_Acct (0);
SetIntercompanyDueTo_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InterOrg_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InterOrg_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InterOrg_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_InterOrg_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372234L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055445L;
/** VAF_TableView_ID=397 */
public static int Table_ID;
 // =397;

/** TableName=VAB_InterOrg_Acct */
public static String Table_Name="VAB_InterOrg_Acct";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_InterOrg_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_OrgTo_ID VAF_Control_Ref_ID=130 */
public static int VAF_ORGTO_ID_VAF_Control_Ref_ID=130;
/** Set Inter-Organization.
@param VAF_OrgTo_ID Organization valid for intercompany documents */
public void SetVAF_OrgTo_ID (int VAF_OrgTo_ID)
{
if (VAF_OrgTo_ID < 1) throw new ArgumentException ("VAF_OrgTo_ID is mandatory.");
Set_ValueNoCheck ("VAF_OrgTo_ID", VAF_OrgTo_ID);
}
/** Get Inter-Organization.
@return Organization valid for intercompany documents */
public int GetVAF_OrgTo_ID() 
{
Object ii = Get_Value("VAF_OrgTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_AccountBook_ID().ToString());
}
/** Set Intercompany Due From Acct.
@param IntercompanyDueFrom_Acct Intercompany Due From / Receivables Account */
public void SetIntercompanyDueFrom_Acct (int IntercompanyDueFrom_Acct)
{
Set_Value ("IntercompanyDueFrom_Acct", IntercompanyDueFrom_Acct);
}
/** Get Intercompany Due From Acct.
@return Intercompany Due From / Receivables Account */
public int GetIntercompanyDueFrom_Acct() 
{
Object ii = Get_Value("IntercompanyDueFrom_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Intercompany Due To Acct.
@param IntercompanyDueTo_Acct Intercompany Due To / Payable Account */
public void SetIntercompanyDueTo_Acct (int IntercompanyDueTo_Acct)
{
Set_Value ("IntercompanyDueTo_Acct", IntercompanyDueTo_Acct);
}
/** Get Intercompany Due To Acct.
@return Intercompany Due To / Payable Account */
public int GetIntercompanyDueTo_Acct() 
{
Object ii = Get_Value("IntercompanyDueTo_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
