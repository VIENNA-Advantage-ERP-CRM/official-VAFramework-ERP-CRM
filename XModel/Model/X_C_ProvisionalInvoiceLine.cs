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
    using System.Data;/** Generated Model for C_ProvisionalInvoiceLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ProvisionalInvoiceLine : PO
    {
        public X_C_ProvisionalInvoiceLine(Context ctx, int C_ProvisionalInvoiceLine_ID, Trx trxName) : base(ctx, C_ProvisionalInvoiceLine_ID, trxName)
        {/** if (C_ProvisionalInvoiceLine_ID == 0){SetC_ProvisionalInvoiceLine_ID (0);SetC_ProvisionalInvoice_ID (0);SetLine (0);// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_ProvisionalInvoiceLine WHERE C_ProvisionalInvoice_ID=@C_ProvisionalInvoice_ID@
} */
        }
        public X_C_ProvisionalInvoiceLine(Ctx ctx, int C_ProvisionalInvoiceLine_ID, Trx trxName) : base(ctx, C_ProvisionalInvoiceLine_ID, trxName)
        {/** if (C_ProvisionalInvoiceLine_ID == 0){SetC_ProvisionalInvoiceLine_ID (0);SetC_ProvisionalInvoice_ID (0);SetLine (0);// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_ProvisionalInvoiceLine WHERE C_ProvisionalInvoice_ID=@C_ProvisionalInvoice_ID@
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ProvisionalInvoiceLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27909664124342L;/** Last Updated Timestamp 7/29/2021 6:06:47 AM */
        public static long updatedMS = 1627538807553L;/** AD_Table_ID=1000552 */
        public static int Table_ID; // =1000552;
        /** TableName=C_ProvisionalInvoiceLine */
        public static String Table_Name = "C_ProvisionalInvoiceLine";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ProvisionalInvoiceLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Available Stock.
@param AvailableStock Available Stock into respective warehouse */
        public void SetAvailableStock(Decimal? AvailableStock) { Set_Value("AvailableStock", (Decimal?)AvailableStock); }/** Get Available Stock.
@return Available Stock into respective warehouse */
        public Decimal GetAvailableStock() { Object bd = Get_Value("AvailableStock"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Charge.
@param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }/** Get Charge.
@return Additional document charges */
        public int GetC_Charge_ID() { Object ii = Get_Value("C_Charge_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Provisional InvoiceLine.
@param C_ProvisionalInvoiceLine_ID Provisional InvoiceLine */
        public void SetC_ProvisionalInvoiceLine_ID(int C_ProvisionalInvoiceLine_ID) { if (C_ProvisionalInvoiceLine_ID < 1) throw new ArgumentException("C_ProvisionalInvoiceLine_ID is mandatory."); Set_ValueNoCheck("C_ProvisionalInvoiceLine_ID", C_ProvisionalInvoiceLine_ID); }/** Get Provisional InvoiceLine.
@return Provisional InvoiceLine */
        public int GetC_ProvisionalInvoiceLine_ID() { Object ii = Get_Value("C_ProvisionalInvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Provisional Invoice.
@param C_ProvisionalInvoice_ID Provisional Invoice */
        public void SetC_ProvisionalInvoice_ID(int C_ProvisionalInvoice_ID) { if (C_ProvisionalInvoice_ID < 1) throw new ArgumentException("C_ProvisionalInvoice_ID is mandatory."); Set_ValueNoCheck("C_ProvisionalInvoice_ID", C_ProvisionalInvoice_ID); }/** Get Provisional Invoice.
@return Provisional Invoice */
        public int GetC_ProvisionalInvoice_ID() { Object ii = Get_Value("C_ProvisionalInvoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tax.
@param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }/** Get Tax.
@return Tax identifier */
        public int GetC_Tax_ID() { Object ii = Get_Value("C_Tax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Discount %.
@param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount) { Set_Value("Discount", (Decimal?)Discount); }/** Get Discount %.
@return Discount in percent */
        public Decimal GetDiscount() { Object bd = Get_Value("Discount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Cost Calculated.
@param IsCostCalculated This checkbox will auto set "True", when the cost is calculated for the document. */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }/** Get Cost Calculated.
@return This checkbox will auto set "True", when the cost is calculated for the document. */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Cost Immediately.
@param IsCostImmediate If selected, costs are calculated immediately on completion of the document. */
        public void SetIsCostImmediate(Boolean IsCostImmediate) { Set_Value("IsCostImmediate", IsCostImmediate); }/** Get Cost Immediately.
@return If selected, costs are calculated immediately on completion of the document. */
        public Boolean IsCostImmediate() { Object oo = Get_Value("IsCostImmediate"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Reversed Cost Calculated.
@param IsReversedCostCalculated This checkbox will auto set "True", when the impact of cost is reversed for the document if any. */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }/** Get Reversed Cost Calculated.
@return This checkbox will auto set "True", when the impact of cost is reversed for the document if any. */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Line No.
@param Line Unique line for this document */
        public void SetLine(int Line) { Set_Value("Line", Line); }/** Get Line No.
@return Unique line for this document */
        public int GetLine() { Object ii = Get_Value("Line"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Line Amount.
@param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt) { Set_Value("LineNetAmt", (Decimal?)LineNetAmt); }/** Get Line Amount.
@return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt() { Object bd = Get_Value("LineNetAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Line Total.
@param LineTotalAmt Total line amount incl. Tax */
        public void SetLineTotalAmt(Decimal? LineTotalAmt) { Set_Value("LineTotalAmt", (Decimal?)LineTotalAmt); }/** Get Line Total.
@return Total line amount incl. Tax */
        public Decimal GetLineTotalAmt() { Object bd = Get_Value("LineTotalAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_Value("M_InOutLine_ID", null);
            else
                Set_Value("M_InOutLine_ID", M_InOutLine_ID);
        }/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID() { Object ii = Get_Value("M_InOutLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Per Unit Difference.
@param PerUnitDifference Per Unit Difference */
        public void SetPerUnitDifference(Decimal? PerUnitDifference) { Set_Value("PerUnitDifference", (Decimal?)PerUnitDifference); }/** Get Per Unit Difference.
@return Per Unit Difference */
        public Decimal GetPerUnitDifference() { Object bd = Get_Value("PerUnitDifference"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Price.
@param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered) { Set_Value("PriceEntered", (Decimal?)PriceEntered); }/** Get Price.
@return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered() { Object bd = Get_Value("PriceEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set List Price.
@param PriceList List Price */
        public void SetPriceList(Decimal? PriceList) { Set_Value("PriceList", (Decimal?)PriceList); }/** Get List Price.
@return List Price */
        public Decimal GetPriceList() { Object bd = Get_Value("PriceList"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set PO Price.
@param PricePO Price based on a purchase order */
        public void SetPricePO(Decimal? PricePO) { Set_Value("PricePO", (Decimal?)PricePO); }/** Get PO Price.
@return Price based on a purchase order */
        public Decimal GetPricePO() { Object bd = Get_Value("PricePO"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered) { Set_Value("QtyEntered", (Decimal?)QtyEntered); }/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered() { Object bd = Get_Value("QtyEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Quantity Invoiced.
@param QtyInvoiced Invoiced Quantity */
        public void SetQtyInvoiced(Decimal? QtyInvoiced) { Set_Value("QtyInvoiced", (Decimal?)QtyInvoiced); }/** Get Quantity Invoiced.
@return Invoiced Quantity */
        public Decimal GetQtyInvoiced() { Object bd = Get_Value("QtyInvoiced"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** ReversalDoc_ID AD_Reference_ID=1000248 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 1000248;/** Set Reversal Document.
@param ReversalDoc_ID Reference of its original document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }/** Get Reversal Document.
@return Reference of its original document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Surcharge Amount.
@param SurchargeAmt Surcharge Amount for a document */
        public void SetSurchargeAmt(Decimal? SurchargeAmt) { Set_Value("SurchargeAmt", (Decimal?)SurchargeAmt); }/** Get Surcharge Amount.
@return Surcharge Amount for a document */
        public Decimal GetSurchargeAmt() { Object bd = Get_Value("SurchargeAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Tax Amount.
@param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt) { Set_Value("TaxAmt", (Decimal?)TaxAmt); }/** Get Tax Amount.
@return Tax Amount for a document */
        public Decimal GetTaxAmt() { Object bd = Get_Value("TaxAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Taxable Amount.
@param TaxBaseAmt Base for calculating the tax amount */
        public void SetTaxBaseAmt(Decimal? TaxBaseAmt) { Set_Value("TaxBaseAmt", (Decimal?)TaxBaseAmt); }/** Get Taxable Amount.
@return Base for calculating the tax amount */
        public Decimal GetTaxBaseAmt() { Object bd = Get_Value("TaxBaseAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Difference.
@param TotalDifference Total Difference */
        public void SetTotalDifference(Decimal? TotalDifference) { Set_Value("TotalDifference", (Decimal?)TotalDifference); }/** Get Total Difference.
@return Total Difference */
        public Decimal GetTotalDifference() { Object bd = Get_Value("TotalDifference"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Unit Price.
@param UnitPrice Unit Price */
        public void SetUnitPrice(Decimal? UnitPrice) { Set_Value("UnitPrice", (Decimal?)UnitPrice); }/** Get Unit Price.
@return Unit Price */
        public Decimal GetUnitPrice() { Object bd = Get_Value("UnitPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}