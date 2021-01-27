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
/** Generated Model for VAB_RFQReplyLineQty
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RFQReplyLineQty : PO
{
public X_VAB_RFQReplyLineQty (Context ctx, int VAB_RFQReplyLineQty_ID, Trx trxName) : base (ctx, VAB_RFQReplyLineQty_ID, trxName)
{
/** if (VAB_RFQReplyLineQty_ID == 0)
{
SetVAB_RFQLine_Qty_ID (0);
SetVAB_RFQReplyLineQty_ID (0);
SetVAB_RFQReplyLine_ID (0);
SetPrice (0.0);
}
 */
}
public X_VAB_RFQReplyLineQty (Ctx ctx, int VAB_RFQReplyLineQty_ID, Trx trxName) : base (ctx, VAB_RFQReplyLineQty_ID, trxName)
{
/** if (VAB_RFQReplyLineQty_ID == 0)
{
SetVAB_RFQLine_Qty_ID (0);
SetVAB_RFQReplyLineQty_ID (0);
SetVAB_RFQReplyLine_ID (0);
SetPrice (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLineQty (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLineQty (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLineQty (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RFQReplyLineQty()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375008L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058219L;
/** VAF_TableView_ID=672 */
public static int Table_ID;
 // =672;

/** TableName=VAB_RFQReplyLineQty */
public static String Table_Name="VAB_RFQReplyLineQty";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_RFQReplyLineQty[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set RfQ Line Quantity.
@param VAB_RFQLine_Qty_ID Request for Quotation Line Quantity */
public void SetVAB_RFQLine_Qty_ID (int VAB_RFQLine_Qty_ID)
{
if (VAB_RFQLine_Qty_ID < 1) throw new ArgumentException ("VAB_RFQLine_Qty_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQLine_Qty_ID", VAB_RFQLine_Qty_ID);
}
/** Get RfQ Line Quantity.
@return Request for Quotation Line Quantity */
public int GetVAB_RFQLine_Qty_ID() 
{
Object ii = Get_Value("VAB_RFQLine_Qty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response Line Qty.
@param VAB_RFQReplyLineQty_ID Request for Quotation Response Line Quantity */
public void SetVAB_RFQReplyLineQty_ID (int VAB_RFQReplyLineQty_ID)
{
if (VAB_RFQReplyLineQty_ID < 1) throw new ArgumentException ("VAB_RFQReplyLineQty_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQReplyLineQty_ID", VAB_RFQReplyLineQty_ID);
}
/** Get RfQ Response Line Qty.
@return Request for Quotation Response Line Quantity */
public int GetVAB_RFQReplyLineQty_ID() 
{
Object ii = Get_Value("VAB_RFQReplyLineQty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response Line.
@param VAB_RFQReplyLine_ID Request for Quotation Response Line */
public void SetVAB_RFQReplyLine_ID (int VAB_RFQReplyLine_ID)
{
if (VAB_RFQReplyLine_ID < 1) throw new ArgumentException ("VAB_RFQReplyLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQReplyLine_ID", VAB_RFQReplyLine_ID);
}
/** Get RfQ Response Line.
@return Request for Quotation Response Line */
public int GetVAB_RFQReplyLine_ID() 
{
Object ii = Get_Value("VAB_RFQReplyLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_RFQReplyLine_ID().ToString());
}
/** Set Discount %.
@param Discount Discount in percent */
public void SetDiscount (Decimal? Discount)
{
Set_Value ("Discount", (Decimal?)Discount);
}
/** Get Discount %.
@return Discount in percent */
public Decimal GetDiscount() 
{
Object bd =Get_Value("Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Price.
@param Price Price */
public void SetPrice (Decimal? Price)
{
if (Price == null) throw new ArgumentException ("Price is mandatory.");
Set_Value ("Price", (Decimal?)Price);
}
/** Get Price.
@return Price */
public Decimal GetPrice() 
{
Object bd =Get_Value("Price");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Ranking.
@param Ranking Relative Rank Number */
public void SetRanking (int Ranking)
{
Set_Value ("Ranking", Ranking);
}
/** Get Ranking.
@return Relative Rank Number */
public int GetRanking() 
{
Object ii = Get_Value("Ranking");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
