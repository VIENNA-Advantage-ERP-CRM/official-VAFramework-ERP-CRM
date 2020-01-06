namespace ViennaAdvantage.Model
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
/** Generated Model for VAPOS_DayEndReport
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPOS_DayEndReport : PO
{
public X_VAPOS_DayEndReport (Context ctx, int VAPOS_DayEndReport_ID, Trx trxName) : base (ctx, VAPOS_DayEndReport_ID, trxName)
{
/** if (VAPOS_DayEndReport_ID == 0)
{
SetVAPOS_DayEndReport_ID (0);
}
 */
}
public X_VAPOS_DayEndReport (Ctx ctx, int VAPOS_DayEndReport_ID, Trx trxName) : base (ctx, VAPOS_DayEndReport_ID, trxName)
{
/** if (VAPOS_DayEndReport_ID == 0)
{
SetVAPOS_DayEndReport_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReport (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReport (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReport (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPOS_DayEndReport()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27696875816380L;
/** Last Updated Timestamp 10/31/2014 3:44:59 PM */
public static long updatedMS = 1414750499591L;
/** AD_Table_ID=1000562 */
public static int Table_ID;
 // =1000562;

/** TableName=VAPOS_DayEndReport */
public static String Table_Name="VAPOS_DayEndReport";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
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
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAPOS_DayEndReport[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Bona TRO Payment.
@param VAPOS_BonaTro Bona TRO Payment */
public void SetVAPOS_BonaTro (Decimal? VAPOS_BonaTro)
{
Set_Value ("VAPOS_BonaTro", (Decimal?)VAPOS_BonaTro);
}
/** Get Bona TRO Payment.
@return Bona TRO Payment */
public Decimal GetVAPOS_BonaTro() 
{
Object bd =Get_Value("VAPOS_BonaTro");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Cash Payment.
@param VAPOS_CashPaymrnt Cash Payment */
public void SetVAPOS_CashPaymrnt (Decimal? VAPOS_CashPaymrnt)
{
Set_Value ("VAPOS_CashPaymrnt", (Decimal?)VAPOS_CashPaymrnt);
}
/** Get Cash Payment.
@return Cash Payment */
public Decimal GetVAPOS_CashPaymrnt() 
{
Object bd =Get_Value("VAPOS_CashPaymrnt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Credit Card Payment.
@param VAPOS_CreditCardPay Credit Card Payment */
public void SetVAPOS_CreditCardPay (Decimal? VAPOS_CreditCardPay)
{
Set_Value ("VAPOS_CreditCardPay", (Decimal?)VAPOS_CreditCardPay);
}
/** Get Credit Card Payment.
@return Credit Card Payment */
public Decimal GetVAPOS_CreditCardPay() 
{
Object bd =Get_Value("VAPOS_CreditCardPay");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Day End Report.
@param VAPOS_DayEndReport_ID Day End Report */
public void SetVAPOS_DayEndReport_ID (int VAPOS_DayEndReport_ID)
{
if (VAPOS_DayEndReport_ID < 1) throw new ArgumentException ("VAPOS_DayEndReport_ID is mandatory.");
Set_ValueNoCheck ("VAPOS_DayEndReport_ID", VAPOS_DayEndReport_ID);
}
/** Get Day End Report.
@return Day End Report */
public int GetVAPOS_DayEndReport_ID() 
{
Object ii = Get_Value("VAPOS_DayEndReport_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Discount.
@param VAPOS_Discount Discount */
public void SetVAPOS_Discount (Decimal? VAPOS_Discount)
{
Set_Value ("VAPOS_Discount", (Decimal?)VAPOS_Discount);
}
/** Get Discount.
@return Discount */
public Decimal GetVAPOS_Discount() 
{
Object bd =Get_Value("VAPOS_Discount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Home Delivery.
@param VAPOS_HmeDelivery Home Delivery */
public void SetVAPOS_HmeDelivery (Decimal? VAPOS_HmeDelivery)
{
Set_Value ("VAPOS_HmeDelivery", (Decimal?)VAPOS_HmeDelivery);
}
/** Get Home Delivery.
@return Home Delivery */
public Decimal GetVAPOS_HmeDelivery() 
{
Object bd =Get_Value("VAPOS_HmeDelivery");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set NGO Sale.
@param VAPOS_NGOSale NGO Sale */
public void SetVAPOS_NGOSale (Decimal? VAPOS_NGOSale)
{
Set_Value ("VAPOS_NGOSale", (Decimal?)VAPOS_NGOSale);
}
/** Get NGO Sale.
@return NGO Sale */
public Decimal GetVAPOS_NGOSale() 
{
Object bd =Get_Value("VAPOS_NGOSale");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Summary Grand Total.
@param VAPOS_OSummaryGrandTot Order Summary Grand Total */
public void SetVAPOS_OSummaryGrandTot (Decimal? VAPOS_OSummaryGrandTot)
{
Set_Value ("VAPOS_OSummaryGrandTot", (Decimal?)VAPOS_OSummaryGrandTot);
}
/** Get Order Summary Grand Total.
@return Order Summary Grand Total */
public Decimal GetVAPOS_OSummaryGrandTot() 
{
Object bd =Get_Value("VAPOS_OSummaryGrandTot");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set On Credit Payment.
@param VAPOS_OnCreditPay On Credit Payment */
public void SetVAPOS_OnCreditPay (Decimal? VAPOS_OnCreditPay)
{
Set_Value ("VAPOS_OnCreditPay", (Decimal?)VAPOS_OnCreditPay);
}
/** Get On Credit Payment.
@return On Credit Payment */
public Decimal GetVAPOS_OnCreditPay() 
{
Object bd =Get_Value("VAPOS_OnCreditPay");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Grand Total.
@param VAPOS_OrderGrandTotal Order Grand Total */
public void SetVAPOS_OrderGrandTotal (Decimal? VAPOS_OrderGrandTotal)
{
Set_Value ("VAPOS_OrderGrandTotal", (Decimal?)VAPOS_OrderGrandTotal);
}
/** Get Order Grand Total.
@return Order Grand Total */
public Decimal GetVAPOS_OrderGrandTotal() 
{
Object bd =Get_Value("VAPOS_OrderGrandTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set POS Terminal.
@param VAPOS_POSTerminal_ID POS Terminal */
public void SetVAPOS_POSTerminal_ID (int VAPOS_POSTerminal_ID)
{
if (VAPOS_POSTerminal_ID <= 0) Set_Value ("VAPOS_POSTerminal_ID", null);
else
Set_Value ("VAPOS_POSTerminal_ID", VAPOS_POSTerminal_ID);
}
/** Get POS Terminal.
@return POS Terminal */
public int GetVAPOS_POSTerminal_ID() 
{
Object ii = Get_Value("VAPOS_POSTerminal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Grand Total.
@param VAPOS_PayGrandTotal Payment Grand Total */
public void SetVAPOS_PayGrandTotal (Decimal? VAPOS_PayGrandTotal)
{
Set_Value ("VAPOS_PayGrandTotal", (Decimal?)VAPOS_PayGrandTotal);
}
/** Get Payment Grand Total.
@return Payment Grand Total */
public Decimal GetVAPOS_PayGrandTotal() 
{
Object bd =Get_Value("VAPOS_PayGrandTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Pick Order.
@param VAPOS_PickOrder Pick Order */
public void SetVAPOS_PickOrder (Decimal? VAPOS_PickOrder)
{
Set_Value ("VAPOS_PickOrder", (Decimal?)VAPOS_PickOrder);
}
/** Get Pick Order.
@return Pick Order */
public Decimal GetVAPOS_PickOrder() 
{
Object bd =Get_Value("VAPOS_PickOrder");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Return.
@param VAPOS_Return Return */
public void SetVAPOS_Return (Decimal? VAPOS_Return)
{
Set_Value ("VAPOS_Return", (Decimal?)VAPOS_Return);
}
/** Get Return.
@return Return */
public Decimal GetVAPOS_Return() 
{
Object bd =Get_Value("VAPOS_Return");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Sales Order Sub Total.
@param VAPOS_SalesOSubTotal Sales Order Sub Total */
public void SetVAPOS_SalesOSubTotal (Decimal? VAPOS_SalesOSubTotal)
{
Set_Value ("VAPOS_SalesOSubTotal", (Decimal?)VAPOS_SalesOSubTotal);
}
/** Get Sales Order Sub Total.
@return Sales Order Sub Total */
public Decimal GetVAPOS_SalesOSubTotal() 
{
Object bd =Get_Value("VAPOS_SalesOSubTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Tax Collected.
@param VAPOS_TaxCollected Tax Collected */
public void SetVAPOS_TaxCollected (Decimal? VAPOS_TaxCollected)
{
Set_Value ("VAPOS_TaxCollected", (Decimal?)VAPOS_TaxCollected);
}
/** Get Tax Collected.
@return Tax Collected */
public Decimal GetVAPOS_TaxCollected() 
{
Object bd =Get_Value("VAPOS_TaxCollected");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Transaction Date.
@param VAPOS_TrxDate Transaction Date */
public void SetVAPOS_TrxDate (DateTime? VAPOS_TrxDate)
{
Set_Value ("VAPOS_TrxDate", (DateTime?)VAPOS_TrxDate);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetVAPOS_TrxDate() 
{
return (DateTime?)Get_Value("VAPOS_TrxDate");
}
/** Set WareHouse Order.
@param VAPOS_WHOrder WareHouse Order */
public void SetVAPOS_WHOrder (Decimal? VAPOS_WHOrder)
{
Set_Value ("VAPOS_WHOrder", (Decimal?)VAPOS_WHOrder);
}
/** Get WareHouse Order.
@return WareHouse Order */
public Decimal GetVAPOS_WHOrder() 
{
Object bd =Get_Value("VAPOS_WHOrder");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
