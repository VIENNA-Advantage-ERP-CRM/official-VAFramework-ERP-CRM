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
/** Generated Model for C_RfQResponseLineQty
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQResponseLineQty : PO
{
public X_C_RfQResponseLineQty (Context ctx, int C_RfQResponseLineQty_ID, Trx trxName) : base (ctx, C_RfQResponseLineQty_ID, trxName)
{
/** if (C_RfQResponseLineQty_ID == 0)
{
SetC_RfQLineQty_ID (0);
SetC_RfQResponseLineQty_ID (0);
SetC_RfQResponseLine_ID (0);
SetPrice (0.0);
}
 */
}
public X_C_RfQResponseLineQty (Ctx ctx, int C_RfQResponseLineQty_ID, Trx trxName) : base (ctx, C_RfQResponseLineQty_ID, trxName)
{
/** if (C_RfQResponseLineQty_ID == 0)
{
SetC_RfQLineQty_ID (0);
SetC_RfQResponseLineQty_ID (0);
SetC_RfQResponseLine_ID (0);
SetPrice (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponseLineQty (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponseLineQty (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponseLineQty (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQResponseLineQty()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375008L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058219L;
/** AD_Table_ID=672 */
public static int Table_ID;
 // =672;

/** TableName=C_RfQResponseLineQty */
public static String Table_Name="C_RfQResponseLineQty";

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
StringBuilder sb = new StringBuilder ("X_C_RfQResponseLineQty[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set RfQ Line Quantity.
@param C_RfQLineQty_ID Request for Quotation Line Quantity */
public void SetC_RfQLineQty_ID (int C_RfQLineQty_ID)
{
if (C_RfQLineQty_ID < 1) throw new ArgumentException ("C_RfQLineQty_ID is mandatory.");
Set_ValueNoCheck ("C_RfQLineQty_ID", C_RfQLineQty_ID);
}
/** Get RfQ Line Quantity.
@return Request for Quotation Line Quantity */
public int GetC_RfQLineQty_ID() 
{
Object ii = Get_Value("C_RfQLineQty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response Line Qty.
@param C_RfQResponseLineQty_ID Request for Quotation Response Line Quantity */
public void SetC_RfQResponseLineQty_ID (int C_RfQResponseLineQty_ID)
{
if (C_RfQResponseLineQty_ID < 1) throw new ArgumentException ("C_RfQResponseLineQty_ID is mandatory.");
Set_ValueNoCheck ("C_RfQResponseLineQty_ID", C_RfQResponseLineQty_ID);
}
/** Get RfQ Response Line Qty.
@return Request for Quotation Response Line Quantity */
public int GetC_RfQResponseLineQty_ID() 
{
Object ii = Get_Value("C_RfQResponseLineQty_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response Line.
@param C_RfQResponseLine_ID Request for Quotation Response Line */
public void SetC_RfQResponseLine_ID (int C_RfQResponseLine_ID)
{
if (C_RfQResponseLine_ID < 1) throw new ArgumentException ("C_RfQResponseLine_ID is mandatory.");
Set_ValueNoCheck ("C_RfQResponseLine_ID", C_RfQResponseLine_ID);
}
/** Get RfQ Response Line.
@return Request for Quotation Response Line */
public int GetC_RfQResponseLine_ID() 
{
Object ii = Get_Value("C_RfQResponseLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_RfQResponseLine_ID().ToString());
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
