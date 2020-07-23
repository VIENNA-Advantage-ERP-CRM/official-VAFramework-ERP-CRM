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
/** Generated Model for A_Asset_Retirement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_A_Asset_Retirement : PO
{
public X_A_Asset_Retirement (Context ctx, int A_Asset_Retirement_ID, Trx trxName) : base (ctx, A_Asset_Retirement_ID, trxName)
{
/** if (A_Asset_Retirement_ID == 0)
{
SetA_Asset_ID (0);
SetA_Asset_Retirement_ID (0);
SetAssetMarketValueAmt (0.0);
SetAssetValueAmt (0.0);
}
 */
}
public X_A_Asset_Retirement (Ctx ctx, int A_Asset_Retirement_ID, Trx trxName) : base (ctx, A_Asset_Retirement_ID, trxName)
{
/** if (A_Asset_Retirement_ID == 0)
{
SetA_Asset_ID (0);
SetA_Asset_Retirement_ID (0);
SetAssetMarketValueAmt (0.0);
SetAssetValueAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Retirement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Retirement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Retirement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_A_Asset_Retirement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367094L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050305L;
/** AD_Table_ID=540 */
public static int Table_ID;
 // =540;

/** TableName=A_Asset_Retirement */
public static String Table_Name="A_Asset_Retirement";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_A_Asset_Retirement[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID < 1) throw new ArgumentException ("A_Asset_ID is mandatory.");
Set_ValueNoCheck ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset Retirement.
@param A_Asset_Retirement_ID Internally used asset is not longer used. */
public void SetA_Asset_Retirement_ID (int A_Asset_Retirement_ID)
{
if (A_Asset_Retirement_ID < 1) throw new ArgumentException ("A_Asset_Retirement_ID is mandatory.");
Set_ValueNoCheck ("A_Asset_Retirement_ID", A_Asset_Retirement_ID);
}
/** Get Asset Retirement.
@return Internally used asset is not longer used. */
public int GetA_Asset_Retirement_ID() 
{
Object ii = Get_Value("A_Asset_Retirement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetA_Asset_Retirement_ID().ToString());
}
/** Set Market value Amount.
@param AssetMarketValueAmt Market value of the asset */
public void SetAssetMarketValueAmt (Decimal? AssetMarketValueAmt)
{
if (AssetMarketValueAmt == null) throw new ArgumentException ("AssetMarketValueAmt is mandatory.");
Set_Value ("AssetMarketValueAmt", (Decimal?)AssetMarketValueAmt);
}
/** Get Market value Amount.
@return Market value of the asset */
public Decimal GetAssetMarketValueAmt() 
{
Object bd =Get_Value("AssetMarketValueAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Asset value.
@param AssetValueAmt Book Value of the asset */
public void SetAssetValueAmt (Decimal? AssetValueAmt)
{
if (AssetValueAmt == null) throw new ArgumentException ("AssetValueAmt is mandatory.");
Set_Value ("AssetValueAmt", (Decimal?)AssetValueAmt);
}
/** Get Asset value.
@return Book Value of the asset */
public Decimal GetAssetValueAmt() 
{
Object bd =Get_Value("AssetValueAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
public void SetC_InvoiceLine_ID (int C_InvoiceLine_ID)
{
if (C_InvoiceLine_ID <= 0) Set_Value ("C_InvoiceLine_ID", null);
else
Set_Value ("C_InvoiceLine_ID", C_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetC_InvoiceLine_ID() 
{
Object ii = Get_Value("C_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
