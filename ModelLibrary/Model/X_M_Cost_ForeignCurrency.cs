namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_Cost_ForeignCurrency
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Cost_ForeignCurrency : PO{public X_M_Cost_ForeignCurrency (Context ctx, int M_Cost_ForeignCurrency_ID, Trx trxName) : base (ctx, M_Cost_ForeignCurrency_ID, trxName){/** if (M_Cost_ForeignCurrency_ID == 0){SetM_Cost_ForeignCurrency_ID (0);} */
}public X_M_Cost_ForeignCurrency (Ctx ctx, int M_Cost_ForeignCurrency_ID, Trx trxName) : base (ctx, M_Cost_ForeignCurrency_ID, trxName){/** if (M_Cost_ForeignCurrency_ID == 0){SetM_Cost_ForeignCurrency_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction  
*/
public X_M_Cost_ForeignCurrency (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Cost_ForeignCurrency (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Cost_ForeignCurrency (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Cost_ForeignCurrency(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27763325247458L;/** Last Updated Timestamp 12/8/2016 5:55:30 PM */
public static long updatedMS = 1481199930669L;/** AD_Table_ID=1000686 */
public static int Table_ID; // =1000686;
/** TableName=M_Cost_ForeignCurrency */
public static String Table_Name="M_Cost_ForeignCurrency";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_Cost_ForeignCurrency[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID){if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);else
Set_Value ("C_BPartner_ID", C_BPartner_ID);}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() {Object ii = Get_Value("C_BPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID){if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);else
Set_Value ("C_Currency_ID", C_Currency_ID);}/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() {Object ii = Get_Value("C_Currency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID){if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);else
Set_Value ("C_Invoice_ID", C_Invoice_ID);}/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() {Object ii = Get_Value("C_Invoice_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Order.
@param C_Order_ID Sales Order */
public void SetC_Order_ID (int C_Order_ID){if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);else
Set_Value ("C_Order_ID", C_Order_ID);}/** Get Order.
@return Sales Order */
public int GetC_Order_ID() {Object ii = Get_Value("C_Order_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set CostPerUnit.
@param CostPerUnit CostPerUnit */
public void SetCostPerUnit (Decimal? CostPerUnit){Set_Value ("CostPerUnit", (Decimal?)CostPerUnit);}/** Get CostPerUnit.
@return CostPerUnit */
public Decimal GetCostPerUnit() {Object bd =Get_Value("CostPerUnit");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Accumulated Amt.
@param CumulatedAmt Total Amount */
public void SetCumulatedAmt (Decimal? CumulatedAmt){Set_Value ("CumulatedAmt", (Decimal?)CumulatedAmt);}/** Get Accumulated Amt.
@return Total Amount */
public Decimal GetCumulatedAmt() {Object bd =Get_Value("CumulatedAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Accumulated Qty.
@param CumulatedQty Total Quantity */
public void SetCumulatedQty (Decimal? CumulatedQty){Set_Value ("CumulatedQty", (Decimal?)CumulatedQty);}/** Get Accumulated Qty.
@return Total Quantity */
public Decimal GetCumulatedQty() {Object bd =Get_Value("CumulatedQty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID){if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);}/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() {Object ii = Get_Value("M_AttributeSetInstance_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID (int M_CostElement_ID){if (M_CostElement_ID <= 0) Set_Value ("M_CostElement_ID", null);else
Set_Value ("M_CostElement_ID", M_CostElement_ID);}/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() {Object ii = Get_Value("M_CostElement_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set M_Cost_ForeignCurrency_ID.
@param M_Cost_ForeignCurrency_ID M_Cost_ForeignCurrency_ID */
public void SetM_Cost_ForeignCurrency_ID (int M_Cost_ForeignCurrency_ID){if (M_Cost_ForeignCurrency_ID < 1) throw new ArgumentException ("M_Cost_ForeignCurrency_ID is mandatory.");Set_ValueNoCheck ("M_Cost_ForeignCurrency_ID", M_Cost_ForeignCurrency_ID);}/** Get M_Cost_ForeignCurrency_ID.
@return M_Cost_ForeignCurrency_ID */
public int GetM_Cost_ForeignCurrency_ID() {Object ii = Get_Value("M_Cost_ForeignCurrency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}