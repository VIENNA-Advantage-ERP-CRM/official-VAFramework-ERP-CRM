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
/** Generated Model for M_Warehouse_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Warehouse_Acct : PO
{
public X_M_Warehouse_Acct (Context ctx, int M_Warehouse_Acct_ID, Trx trxName) : base (ctx, M_Warehouse_Acct_ID, trxName)
{
/** if (M_Warehouse_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetM_Warehouse_ID (0);
SetW_Differences_Acct (0);
SetW_InvActualAdjust_Acct (0);
SetW_Inventory_Acct (0);
SetW_Revaluation_Acct (0);
}
 */
}
public X_M_Warehouse_Acct (Ctx ctx, int M_Warehouse_Acct_ID, Trx trxName) : base (ctx, M_Warehouse_Acct_ID, trxName)
{
/** if (M_Warehouse_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetM_Warehouse_ID (0);
SetW_Differences_Acct (0);
SetW_InvActualAdjust_Acct (0);
SetW_Inventory_Acct (0);
SetW_Revaluation_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Warehouse_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Warehouse_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Warehouse_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Warehouse_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381512L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064723L;
/** AD_Table_ID=191 */
public static int Table_ID;
 // =191;

/** TableName=M_Warehouse_Acct */
public static String Table_Name="M_Warehouse_Acct";

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
StringBuilder sb = new StringBuilder ("X_M_Warehouse_Acct[").Append(Get_ID()).Append("]");
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
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_ValueNoCheck ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse Differences.
@param W_Differences_Acct Warehouse Differences Account */
public void SetW_Differences_Acct (int W_Differences_Acct)
{
Set_Value ("W_Differences_Acct", W_Differences_Acct);
}
/** Get Warehouse Differences.
@return Warehouse Differences Account */
public int GetW_Differences_Acct() 
{
Object ii = Get_Value("W_Differences_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Adjustment.
@param W_InvActualAdjust_Acct Account for Inventory value adjustments for Actual Costing */
public void SetW_InvActualAdjust_Acct (int W_InvActualAdjust_Acct)
{
Set_Value ("W_InvActualAdjust_Acct", W_InvActualAdjust_Acct);
}
/** Get Inventory Adjustment.
@return Account for Inventory value adjustments for Actual Costing */
public int GetW_InvActualAdjust_Acct() 
{
Object ii = Get_Value("W_InvActualAdjust_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set (Not Used).
@param W_Inventory_Acct Warehouse Inventory Asset Account - Currently not used */
public void SetW_Inventory_Acct (int W_Inventory_Acct)
{
Set_Value ("W_Inventory_Acct", W_Inventory_Acct);
}
/** Get (Not Used).
@return Warehouse Inventory Asset Account - Currently not used */
public int GetW_Inventory_Acct() 
{
Object ii = Get_Value("W_Inventory_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Revaluation.
@param W_Revaluation_Acct Account for Inventory Revaluation */
public void SetW_Revaluation_Acct (int W_Revaluation_Acct)
{
Set_Value ("W_Revaluation_Acct", W_Revaluation_Acct);
}
/** Get Inventory Revaluation.
@return Account for Inventory Revaluation */
public int GetW_Revaluation_Acct() 
{
Object ii = Get_Value("W_Revaluation_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
