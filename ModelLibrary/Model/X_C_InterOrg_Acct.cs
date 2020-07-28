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
/** Generated Model for C_InterOrg_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_InterOrg_Acct : PO
{
public X_C_InterOrg_Acct (Context ctx, int C_InterOrg_Acct_ID, Trx trxName) : base (ctx, C_InterOrg_Acct_ID, trxName)
{
/** if (C_InterOrg_Acct_ID == 0)
{
SetAD_OrgTo_ID (0);
SetC_AcctSchema_ID (0);
SetIntercompanyDueFrom_Acct (0);
SetIntercompanyDueTo_Acct (0);
}
 */
}
public X_C_InterOrg_Acct (Ctx ctx, int C_InterOrg_Acct_ID, Trx trxName) : base (ctx, C_InterOrg_Acct_ID, trxName)
{
/** if (C_InterOrg_Acct_ID == 0)
{
SetAD_OrgTo_ID (0);
SetC_AcctSchema_ID (0);
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
public X_C_InterOrg_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InterOrg_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InterOrg_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_InterOrg_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372234L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055445L;
/** AD_Table_ID=397 */
public static int Table_ID;
 // =397;

/** TableName=C_InterOrg_Acct */
public static String Table_Name="C_InterOrg_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_InterOrg_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgTo_ID AD_Reference_ID=130 */
public static int AD_ORGTO_ID_AD_Reference_ID=130;
/** Set Inter-Organization.
@param AD_OrgTo_ID Organization valid for intercompany documents */
public void SetAD_OrgTo_ID (int AD_OrgTo_ID)
{
if (AD_OrgTo_ID < 1) throw new ArgumentException ("AD_OrgTo_ID is mandatory.");
Set_ValueNoCheck ("AD_OrgTo_ID", AD_OrgTo_ID);
}
/** Get Inter-Organization.
@return Organization valid for intercompany documents */
public int GetAD_OrgTo_ID() 
{
Object ii = Get_Value("AD_OrgTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
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
