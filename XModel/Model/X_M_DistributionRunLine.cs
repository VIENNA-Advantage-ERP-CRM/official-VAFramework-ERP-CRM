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
/** Generated Model for M_DistributionRunLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DistributionRunLine : PO
{
public X_M_DistributionRunLine (Context ctx, int M_DistributionRunLine_ID, Trx trxName) : base (ctx, M_DistributionRunLine_ID, trxName)
{
/** if (M_DistributionRunLine_ID == 0)
{
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_DistributionRunLine WHERE M_DistributionRun_ID=@M_DistributionRun_ID@
SetM_DistributionList_ID (0);
SetM_DistributionRunLine_ID (0);
SetM_DistributionRun_ID (0);
SetM_Product_ID (0);
SetMinQty (0.0);	// 0
SetTotalQty (0.0);
}
 */
}
public X_M_DistributionRunLine (Ctx ctx, int M_DistributionRunLine_ID, Trx trxName) : base (ctx, M_DistributionRunLine_ID, trxName)
{
/** if (M_DistributionRunLine_ID == 0)
{
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_DistributionRunLine WHERE M_DistributionRun_ID=@M_DistributionRun_ID@
SetM_DistributionList_ID (0);
SetM_DistributionRunLine_ID (0);
SetM_DistributionRun_ID (0);
SetM_Product_ID (0);
SetMinQty (0.0);	// 0
SetTotalQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionRunLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionRunLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DistributionRunLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DistributionRunLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379303L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062514L;
/** AD_Table_ID=713 */
public static int Table_ID;
 // =713;

/** TableName=M_DistributionRunLine */
public static String Table_Name="M_DistributionRunLine";

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
StringBuilder sb = new StringBuilder ("X_M_DistributionRunLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
/** Set Distribution List.
@param M_DistributionList_ID Distribution Lists allow to distribute products to a selected list of partners */
public void SetM_DistributionList_ID (int M_DistributionList_ID)
{
if (M_DistributionList_ID < 1) throw new ArgumentException ("M_DistributionList_ID is mandatory.");
Set_Value ("M_DistributionList_ID", M_DistributionList_ID);
}
/** Get Distribution List.
@return Distribution Lists allow to distribute products to a selected list of partners */
public int GetM_DistributionList_ID() 
{
Object ii = Get_Value("M_DistributionList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Distribution Run Line.
@param M_DistributionRunLine_ID Distribution Run Lines defines Distribution List, the Product and Quantities */
public void SetM_DistributionRunLine_ID (int M_DistributionRunLine_ID)
{
if (M_DistributionRunLine_ID < 1) throw new ArgumentException ("M_DistributionRunLine_ID is mandatory.");
Set_ValueNoCheck ("M_DistributionRunLine_ID", M_DistributionRunLine_ID);
}
/** Get Distribution Run Line.
@return Distribution Run Lines defines Distribution List, the Product and Quantities */
public int GetM_DistributionRunLine_ID() 
{
Object ii = Get_Value("M_DistributionRunLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Distribution Run.
@param M_DistributionRun_ID Distribution Run create Orders to distribute products to a selected list of partners */
public void SetM_DistributionRun_ID (int M_DistributionRun_ID)
{
if (M_DistributionRun_ID < 1) throw new ArgumentException ("M_DistributionRun_ID is mandatory.");
Set_ValueNoCheck ("M_DistributionRun_ID", M_DistributionRun_ID);
}
/** Get Distribution Run.
@return Distribution Run create Orders to distribute products to a selected list of partners */
public int GetM_DistributionRun_ID() 
{
Object ii = Get_Value("M_DistributionRun_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_DistributionRun_ID().ToString());
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Minimum Quantity.
@param MinQty Minimum quantity for the business partner */
public void SetMinQty (Decimal? MinQty)
{
if (MinQty == null) throw new ArgumentException ("MinQty is mandatory.");
Set_Value ("MinQty", (Decimal?)MinQty);
}
/** Get Minimum Quantity.
@return Minimum quantity for the business partner */
public Decimal GetMinQty() 
{
Object bd =Get_Value("MinQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Total Quantity.
@param TotalQty Total Quantity */
public void SetTotalQty (Decimal? TotalQty)
{
if (TotalQty == null) throw new ArgumentException ("TotalQty is mandatory.");
Set_Value ("TotalQty", (Decimal?)TotalQty);
}
/** Get Total Quantity.
@return Total Quantity */
public Decimal GetTotalQty() 
{
Object bd =Get_Value("TotalQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
