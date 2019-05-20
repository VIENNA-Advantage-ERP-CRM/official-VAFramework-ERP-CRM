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
    using System.Data;/** Generated Model for C_OrderlineHistory
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_OrderlineHistory : PO
    {
        public X_C_OrderlineHistory(Context ctx, int C_OrderlineHistory_ID, Trx trxName)
            : base(ctx, C_OrderlineHistory_ID, trxName)
        {/** if (C_OrderlineHistory_ID == 0){SetC_OrderLine_ID (0);SetC_OrderlineHistory_ID (0);SetC_Tax_ID (0);SetLineNetAmt (0.0);SetPriceActual (0.0);SetPriceEntered (0.0);SetPriceList (0.0);SetProcessed (false);// N
SetQtyEntered (0.0);// 1
} */
        }
        public X_C_OrderlineHistory(Ctx ctx, int C_OrderlineHistory_ID, Trx trxName)
            : base(ctx, C_OrderlineHistory_ID, trxName)
        {/** if (C_OrderlineHistory_ID == 0){SetC_OrderLine_ID (0);SetC_OrderlineHistory_ID (0);SetC_Tax_ID (0);SetLineNetAmt (0.0);SetPriceActual (0.0);SetPriceEntered (0.0);SetPriceList (0.0);SetProcessed (false);// N
SetQtyEntered (0.0);// 1
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_OrderlineHistory(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_OrderlineHistory(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_OrderlineHistory(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_OrderlineHistory() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27785509045284L;/** Last Updated Timestamp 8/22/2017 12:05:28 PM */
        public static long updatedMS = 1503383728495L;/** AD_Table_ID=1000511 */
        public static int Table_ID; // =1000511;
        /** TableName=C_OrderlineHistory */
        public static String Table_Name = "C_OrderlineHistory";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_OrderlineHistory[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Charge.
@param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }/** Get Charge.
@return Additional document charges */
        public int GetC_Charge_ID() { Object ii = Get_Value("C_Charge_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Billing Frequency.
@param C_Frequency_ID Identifies the billing frequency i.e, monthly, quaterly etc. */
        public void SetC_Frequency_ID(int C_Frequency_ID)
        {
            if (C_Frequency_ID <= 0) Set_Value("C_Frequency_ID", null);
            else
                Set_Value("C_Frequency_ID", C_Frequency_ID);
        }/** Get Billing Frequency.
@return Identifies the billing frequency i.e, monthly, quaterly etc. */
        public int GetC_Frequency_ID() { Object ii = Get_Value("C_Frequency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID) { if (C_OrderLine_ID < 1) throw new ArgumentException("C_OrderLine_ID is mandatory."); Set_ValueNoCheck("C_OrderLine_ID", C_OrderLine_ID); }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Line history.
@param C_OrderlineHistory_ID Line history */
        public void SetC_OrderlineHistory_ID(int C_OrderlineHistory_ID) { if (C_OrderlineHistory_ID < 1) throw new ArgumentException("C_OrderlineHistory_ID is mandatory."); Set_ValueNoCheck("C_OrderlineHistory_ID", C_OrderlineHistory_ID); }/** Get Line history.
@return Line history */
        public int GetC_OrderlineHistory_ID() { Object ii = Get_Value("C_OrderlineHistory_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tax.
@param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID) { if (C_Tax_ID < 1) throw new ArgumentException("C_Tax_ID is mandatory."); Set_Value("C_Tax_ID", C_Tax_ID); }/** Get Tax.
@return Tax identifier */
        public int GetC_Tax_ID() { Object ii = Get_Value("C_Tax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Date Ordered.
@param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered) { Set_Value("DateOrdered", (DateTime?)DateOrdered); }/** Get Date Ordered.
@return Date of Order */
        public DateTime? GetDateOrdered() { return (DateTime?)Get_Value("DateOrdered"); }/** Set Date Promised.
@param DatePromised Date Order was promised */
        public void SetDatePromised(DateTime? DatePromised) { Set_Value("DatePromised", (DateTime?)DatePromised); }/** Get Date Promised.
@return Date Order was promised */
        public DateTime? GetDatePromised() { return (DateTime?)Get_Value("DatePromised"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Discount %.
@param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount) { Set_Value("Discount", (Decimal?)Discount); }/** Get Discount %.
@return Discount in percent */
        public Decimal GetDiscount() { Object bd = Get_Value("Discount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set End Date.
@param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate) { Set_Value("EndDate", (DateTime?)EndDate); }/** Get End Date.
@return Last effective date (inclusive) */
        public DateTime? GetEndDate() { return (DateTime?)Get_Value("EndDate"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Line No.
@param Line Unique line for this document */
        public void SetLine(int Line) { Set_Value("Line", Line); }/** Get Line No.
@return Unique line for this document */
        public int GetLine() { Object ii = Get_Value("Line"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Line Amount.
@param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt) { if (LineNetAmt == null) throw new ArgumentException("LineNetAmt is mandatory."); Set_Value("LineNetAmt", (Decimal?)LineNetAmt); }/** Get Line Amount.
@return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt() { Object bd = Get_Value("LineNetAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Freight Carrier.
@param M_Shipper_ID Method or manner of product delivery */
        public void SetM_Shipper_ID(int M_Shipper_ID)
        {
            if (M_Shipper_ID <= 0) Set_Value("M_Shipper_ID", null);
            else
                Set_Value("M_Shipper_ID", M_Shipper_ID);
        }/** Get Freight Carrier.
@return Method or manner of product delivery */
        public int GetM_Shipper_ID() { Object ii = Get_Value("M_Shipper_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set No of Cycle.
@param NoofCycle Number of cycle */
        public void SetNoofCycle(int NoofCycle) { Set_Value("NoofCycle", NoofCycle); }/** Get No of Cycle.
@return Number of cycle */
        public int GetNoofCycle() { Object ii = Get_Value("NoofCycle"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Unit Price.
@param PriceActual Actual Price  */
        public void SetPriceActual(Decimal? PriceActual) { if (PriceActual == null) throw new ArgumentException("PriceActual is mandatory."); Set_Value("PriceActual", (Decimal?)PriceActual); }/** Get Unit Price.
@return Actual Price  */
        public Decimal GetPriceActual() { Object bd = Get_Value("PriceActual"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Cost Price.
@param PriceCost Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public void SetPriceCost(Decimal? PriceCost) { Set_Value("PriceCost", (Decimal?)PriceCost); }/** Get Cost Price.
@return Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public Decimal GetPriceCost() { Object bd = Get_Value("PriceCost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Price.
@param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered) { if (PriceEntered == null) throw new ArgumentException("PriceEntered is mandatory."); Set_Value("PriceEntered", (Decimal?)PriceEntered); }/** Get Price.
@return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered() { Object bd = Get_Value("PriceEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set List Price.
@param PriceList List Price */
        public void SetPriceList(Decimal? PriceList) { if (PriceList == null) throw new ArgumentException("PriceList is mandatory."); Set_Value("PriceList", (Decimal?)PriceList); }/** Get List Price.
@return List Price */
        public Decimal GetPriceList() { Object bd = Get_Value("PriceList"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered) { if (QtyEntered == null) throw new ArgumentException("QtyEntered is mandatory."); Set_Value("QtyEntered", (Decimal?)QtyEntered); }/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered() { Object bd = Get_Value("QtyEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Quantity Per Cycle.
@param QtyPerCycle Identifies quantity per cycle to be billed */
        public void SetQtyPerCycle(Decimal? QtyPerCycle) { Set_Value("QtyPerCycle", (Decimal?)QtyPerCycle); }/** Get Quantity Per Cycle.
@return Identifies quantity per cycle to be billed */
        public Decimal GetQtyPerCycle() { Object bd = Get_Value("QtyPerCycle"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Start Date.
@param StartDate First effective day (inclusive) */
        public void SetStartDate(DateTime? StartDate) { Set_Value("StartDate", (DateTime?)StartDate); }/** Get Start Date.
@return First effective day (inclusive) */
        public DateTime? GetStartDate() { return (DateTime?)Get_Value("StartDate"); }
        /** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Quantity Ordered.
@param QtyOrdered Ordered Quantity */
        public void SetQtyOrdered(Decimal? QtyOrdered)
        {
            Set_Value("QtyOrdered", (Decimal?)QtyOrdered);
        }
        /** Get Quantity Ordered.
    @return Ordered Quantity */
        public Decimal GetQtyOrdered()
        {
            Object bd = Get_Value("QtyOrdered");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }
}