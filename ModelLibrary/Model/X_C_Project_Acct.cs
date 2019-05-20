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
/** Generated Model for C_Project_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Project_Acct : PO
{
public X_C_Project_Acct (Context ctx, int C_Project_Acct_ID, Trx trxName) : base (ctx, C_Project_Acct_ID, trxName)
{
/** if (C_Project_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Project_ID (0);
SetPJ_Asset_Acct (0);
SetPJ_WIP_Acct (0);
}
 */
}
public X_C_Project_Acct (Ctx ctx, int C_Project_Acct_ID, Trx trxName) : base (ctx, C_Project_Acct_ID, trxName)
{
/** if (C_Project_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Project_ID (0);
SetPJ_Asset_Acct (0);
SetPJ_WIP_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Project_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Project_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Project_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Project_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374397L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057608L;
/** AD_Table_ID=204 */
public static int Table_ID;
 // =204;

/** TableName=C_Project_Acct */
public static String Table_Name="C_Project_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_Project_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID < 1) throw new ArgumentException ("C_Project_ID is mandatory.");
Set_ValueNoCheck ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Asset.
@param PJ_Asset_Acct Project Asset Account */
public void SetPJ_Asset_Acct (int PJ_Asset_Acct)
{
Set_Value ("PJ_Asset_Acct", PJ_Asset_Acct);
}
/** Get Project Asset.
@return Project Asset Account */
public int GetPJ_Asset_Acct() 
{
Object ii = Get_Value("PJ_Asset_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Work In Progress.
@param PJ_WIP_Acct Account for Work in Progress */
public void SetPJ_WIP_Acct (int PJ_WIP_Acct)
{
Set_Value ("PJ_WIP_Acct", PJ_WIP_Acct);
}
/** Get Work In Progress.
@return Account for Work in Progress */
public int GetPJ_WIP_Acct() 
{
Object ii = Get_Value("PJ_WIP_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
