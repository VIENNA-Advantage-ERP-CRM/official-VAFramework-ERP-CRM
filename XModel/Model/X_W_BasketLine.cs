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
/** Generated Model for W_BasketLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_BasketLine : PO
{
public X_W_BasketLine (Context ctx, int W_BasketLine_ID, Trx trxName) : base (ctx, W_BasketLine_ID, trxName)
{
/** if (W_BasketLine_ID == 0)
{
SetDescription (null);
SetLine (0);
SetPrice (0.0);
SetProduct (null);
SetQty (0.0);
SetW_BasketLine_ID (0);
SetW_Basket_ID (0);
}
 */
}
public X_W_BasketLine (Ctx ctx, int W_BasketLine_ID, Trx trxName) : base (ctx, W_BasketLine_ID, trxName)
{
/** if (W_BasketLine_ID == 0)
{
SetDescription (null);
SetLine (0);
SetPrice (0.0);
SetProduct (null);
SetQty (0.0);
SetW_BasketLine_ID (0);
SetW_Basket_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_BasketLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_BasketLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_BasketLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_BasketLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384913L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068124L;
/** AD_Table_ID=549 */
public static int Table_ID;
 // =549;

/** TableName=W_BasketLine */
public static String Table_Name="W_BasketLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_W_BasketLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description == null) throw new ArgumentException ("Description is mandatory.");
if (Description.Length > 255)
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetLine().ToString());
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
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
/** Set Product.
@param Product Product */
public void SetProduct (String Product)
{
if (Product == null) throw new ArgumentException ("Product is mandatory.");
if (Product.Length > 40)
{
log.Warning("Length > 40 - truncated");
Product = Product.Substring(0,40);
}
Set_Value ("Product", Product);
}
/** Get Product.
@return Product */
public String GetProduct() 
{
return (String)Get_Value("Product");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Basket Line.
@param W_BasketLine_ID Web Basket Line */
public void SetW_BasketLine_ID (int W_BasketLine_ID)
{
if (W_BasketLine_ID < 1) throw new ArgumentException ("W_BasketLine_ID is mandatory.");
Set_ValueNoCheck ("W_BasketLine_ID", W_BasketLine_ID);
}
/** Get Basket Line.
@return Web Basket Line */
public int GetW_BasketLine_ID() 
{
Object ii = Get_Value("W_BasketLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set W_Basket_ID.
@param W_Basket_ID Web Basket */
public void SetW_Basket_ID (int W_Basket_ID)
{
if (W_Basket_ID < 1) throw new ArgumentException ("W_Basket_ID is mandatory.");
Set_ValueNoCheck ("W_Basket_ID", W_Basket_ID);
}
/** Get W_Basket_ID.
@return Web Basket */
public int GetW_Basket_ID() 
{
Object ii = Get_Value("W_Basket_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
