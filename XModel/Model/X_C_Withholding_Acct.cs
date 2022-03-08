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
/** Generated Model for C_Withholding_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Withholding_Acct : PO
{
public X_C_Withholding_Acct (Context ctx, int C_Withholding_Acct_ID, Trx trxName) : base (ctx, C_Withholding_Acct_ID, trxName)
{
/** if (C_Withholding_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Withholding_ID (0);
SetWithholding_Acct (0);
}
 */
}
public X_C_Withholding_Acct (Ctx ctx, int C_Withholding_Acct_ID, Trx trxName) : base (ctx, C_Withholding_Acct_ID, trxName)
{
/** if (C_Withholding_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Withholding_ID (0);
SetWithholding_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Withholding_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Withholding_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375917L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059128L;
/** AD_Table_ID=400 */
public static int Table_ID;
 // =400;

/** TableName=C_Withholding_Acct */
public static String Table_Name="C_Withholding_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_Withholding_Acct[").Append(Get_ID()).Append("]");
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
/** Set Withholding.
@param C_Withholding_ID Withholding type defined */
public void SetC_Withholding_ID (int C_Withholding_ID)
{
if (C_Withholding_ID < 1) throw new ArgumentException ("C_Withholding_ID is mandatory.");
Set_ValueNoCheck ("C_Withholding_ID", C_Withholding_ID);
}
/** Get Withholding.
@return Withholding type defined */
public int GetC_Withholding_ID() 
{
Object ii = Get_Value("C_Withholding_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Withholding.
@param Withholding_Acct Account for Withholdings */
public void SetWithholding_Acct (int Withholding_Acct)
{
Set_Value ("Withholding_Acct", Withholding_Acct);
}
/** Get Withholding.
@return Account for Withholdings */
public int GetWithholding_Acct() 
{
Object ii = Get_Value("Withholding_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
