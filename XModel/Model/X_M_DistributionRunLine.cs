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
/** Generated Model for VAM_DistributionRunLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_DistributionRunLine : PO
{
public X_VAM_DistributionRunLine (Context ctx, int VAM_DistributionRunLine_ID, Trx trxName) : base (ctx, VAM_DistributionRunLine_ID, trxName)
{
/** if (VAM_DistributionRunLine_ID == 0)
{
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAM_DistributionRunLine WHERE VAM_DistributionRun_ID=@VAM_DistributionRun_ID@
SetVAM_DistributionList_ID (0);
SetVAM_DistributionRunLine_ID (0);
SetVAM_DistributionRun_ID (0);
SetVAM_Product_ID (0);
SetMinQty (0.0);	// 0
SetTotalQty (0.0);
}
 */
}
public X_VAM_DistributionRunLine (Ctx ctx, int VAM_DistributionRunLine_ID, Trx trxName) : base (ctx, VAM_DistributionRunLine_ID, trxName)
{
/** if (VAM_DistributionRunLine_ID == 0)
{
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAM_DistributionRunLine WHERE VAM_DistributionRun_ID=@VAM_DistributionRun_ID@
SetVAM_DistributionList_ID (0);
SetVAM_DistributionRunLine_ID (0);
SetVAM_DistributionRun_ID (0);
SetVAM_Product_ID (0);
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
public X_VAM_DistributionRunLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_DistributionRunLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_DistributionRunLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_DistributionRunLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379303L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062514L;
/** VAF_TableView_ID=713 */
public static int Table_ID;
 // =713;

/** TableName=VAM_DistributionRunLine */
public static String Table_Name="VAM_DistributionRunLine";

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
StringBuilder sb = new StringBuilder ("X_VAM_DistributionRunLine[").Append(Get_ID()).Append("]");
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
@param VAM_DistributionList_ID Distribution Lists allow to distribute products to a selected list of partners */
public void SetVAM_DistributionList_ID (int VAM_DistributionList_ID)
{
if (VAM_DistributionList_ID < 1) throw new ArgumentException ("VAM_DistributionList_ID is mandatory.");
Set_Value ("VAM_DistributionList_ID", VAM_DistributionList_ID);
}
/** Get Distribution List.
@return Distribution Lists allow to distribute products to a selected list of partners */
public int GetVAM_DistributionList_ID() 
{
Object ii = Get_Value("VAM_DistributionList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Distribution Run Line.
@param VAM_DistributionRunLine_ID Distribution Run Lines defines Distribution List, the Product and Quantities */
public void SetVAM_DistributionRunLine_ID (int VAM_DistributionRunLine_ID)
{
if (VAM_DistributionRunLine_ID < 1) throw new ArgumentException ("VAM_DistributionRunLine_ID is mandatory.");
Set_ValueNoCheck ("VAM_DistributionRunLine_ID", VAM_DistributionRunLine_ID);
}
/** Get Distribution Run Line.
@return Distribution Run Lines defines Distribution List, the Product and Quantities */
public int GetVAM_DistributionRunLine_ID() 
{
Object ii = Get_Value("VAM_DistributionRunLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Distribution Run.
@param VAM_DistributionRun_ID Distribution Run create Orders to distribute products to a selected list of partners */
public void SetVAM_DistributionRun_ID (int VAM_DistributionRun_ID)
{
if (VAM_DistributionRun_ID < 1) throw new ArgumentException ("VAM_DistributionRun_ID is mandatory.");
Set_ValueNoCheck ("VAM_DistributionRun_ID", VAM_DistributionRun_ID);
}
/** Get Distribution Run.
@return Distribution Run create Orders to distribute products to a selected list of partners */
public int GetVAM_DistributionRun_ID() 
{
Object ii = Get_Value("VAM_DistributionRun_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_DistributionRun_ID().ToString());
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
Set_Value ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
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
