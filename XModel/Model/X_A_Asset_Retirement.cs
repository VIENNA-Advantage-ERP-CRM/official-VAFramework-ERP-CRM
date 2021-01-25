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
/** Generated Model for VAA_AssetRetirement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAA_AssetRetirement : PO
{
public X_VAA_AssetRetirement (Context ctx, int VAA_AssetRetirement_ID, Trx trxName) : base (ctx, VAA_AssetRetirement_ID, trxName)
{
/** if (VAA_AssetRetirement_ID == 0)
{
SetA_Asset_ID (0);
SetVAA_AssetRetirement_ID (0);
SetAssetMarketValueAmt (0.0);
SetAssetValueAmt (0.0);
}
 */
}
public X_VAA_AssetRetirement (Ctx ctx, int VAA_AssetRetirement_ID, Trx trxName) : base (ctx, VAA_AssetRetirement_ID, trxName)
{
/** if (VAA_AssetRetirement_ID == 0)
{
SetA_Asset_ID (0);
SetVAA_AssetRetirement_ID (0);
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
public X_VAA_AssetRetirement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAA_AssetRetirement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAA_AssetRetirement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAA_AssetRetirement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367094L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050305L;
/** VAF_TableView_ID=540 */
public static int Table_ID;
 // =540;

/** TableName=VAA_AssetRetirement */
public static String Table_Name="VAA_AssetRetirement";

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
StringBuilder sb = new StringBuilder ("X_VAA_AssetRetirement[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int VAA_Asset_ID)
{
if (VAA_Asset_ID < 1) throw new ArgumentException ("VAA_Asset_ID is mandatory.");
Set_ValueNoCheck ("VAA_Asset_ID", VAA_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("VAA_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset Retirement.
@param VAA_AssetRetirement_ID Internally used asset is not longer used. */
public void SetVAA_AssetRetirement_ID (int VAA_AssetRetirement_ID)
{
if (VAA_AssetRetirement_ID < 1) throw new ArgumentException ("VAA_AssetRetirement_ID is mandatory.");
Set_ValueNoCheck ("VAA_AssetRetirement_ID", VAA_AssetRetirement_ID);
}
/** Get Asset Retirement.
@return Internally used asset is not longer used. */
public int GetVAA_AssetRetirement_ID() 
{
Object ii = Get_Value("VAA_AssetRetirement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAA_AssetRetirement_ID().ToString());
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
