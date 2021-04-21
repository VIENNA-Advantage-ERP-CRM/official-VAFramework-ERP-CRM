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
    using System.Data;/** Generated Model for C_ForecastLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ForecastLine : PO
    {
        public X_C_ForecastLine(Context ctx, int C_ForecastLine_ID, Trx trxName) : base(ctx, C_ForecastLine_ID, trxName)
        {/** if (C_ForecastLine_ID == 0){SetC_ForecastLine_ID (0);SetC_Forecast_ID (0);} */
        }
        public X_C_ForecastLine(Ctx ctx, int C_ForecastLine_ID, Trx trxName) : base(ctx, C_ForecastLine_ID, trxName)
        {/** if (C_ForecastLine_ID == 0){SetC_ForecastLine_ID (0);SetC_Forecast_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ForecastLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ForecastLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ForecastLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ForecastLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27901098140688L;/** Last Updated Timestamp 4/21/2021 2:40:23 AM */
        public static long updatedMS = 1618972823899L;/** AD_Table_ID=1000245 */
        public static int Table_ID; // =1000245;
        /** TableName=C_ForecastLine */
        public static String Table_Name = "C_ForecastLine";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ForecastLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }
        /** BOMUse AD_Reference_ID=348 */
        public static int BOMUSE_AD_Reference_ID = 348;/** Master = A */
        public static String BOMUSE_Master = "A";/** Engineering = E */
        public static String BOMUSE_Engineering = "E";/** Manufacturing = M */
        public static String BOMUSE_Manufacturing = "M";/** Maintenance = N */
        public static String BOMUSE_Maintenance = "N";/** Planning = P */
        public static String BOMUSE_Planning = "P";/** Repair = R */
        public static String BOMUSE_Repair = "R";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsBOMUseValid(String test) { return test == null || test.Equals("A") || test.Equals("E") || test.Equals("M") || test.Equals("N") || test.Equals("P") || test.Equals("R"); }/** Set BOM Use.
@param BOMUse The use of the Bill of Material */
        public void SetBOMUse(String BOMUse)
        {
            if (!IsBOMUseValid(BOMUse))
                throw new ArgumentException("BOMUse Invalid value - " + BOMUse + " - Reference_ID=348 - A - E - M - N - P - R"); if (BOMUse != null && BOMUse.Length > 1) { log.Warning("Length > 1 - truncated"); BOMUse = BOMUse.Substring(0, 1); }
            Set_Value("BOMUse", BOMUse);
        }/** Get BOM Use.
@return The use of the Bill of Material */
        public String GetBOMUse() { return (String)Get_Value("BOMUse"); }/** Set Base Quantity.
@param BaseQty this field ,shows the Forecast Quantity in base UOM */
        public void SetBaseQty(Decimal? BaseQty) { Set_Value("BaseQty", (Decimal?)BaseQty); }/** Get Base Quantity.
@return this field ,shows the Forecast Quantity in base UOM */
        public Decimal GetBaseQty() { Object bd = Get_Value("BaseQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Charge.
@param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }/** Get Charge.
@return Additional document charges */
        public int GetC_Charge_ID() { Object ii = Get_Value("C_Charge_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set C_ForecastLine_ID.
@param C_ForecastLine_ID C_ForecastLine_ID */
        public void SetC_ForecastLine_ID(int C_ForecastLine_ID) { if (C_ForecastLine_ID < 1) throw new ArgumentException("C_ForecastLine_ID is mandatory."); Set_ValueNoCheck("C_ForecastLine_ID", C_ForecastLine_ID); }/** Get C_ForecastLine_ID.
@return C_ForecastLine_ID */
        public int GetC_ForecastLine_ID() { Object ii = Get_Value("C_ForecastLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Forecast.
@param C_Forecast_ID Forecast */
        public void SetC_Forecast_ID(int C_Forecast_ID) { if (C_Forecast_ID < 1) throw new ArgumentException("C_Forecast_ID is mandatory."); Set_ValueNoCheck("C_Forecast_ID", C_Forecast_ID); }/** Get Forecast.
@return Forecast */
        public int GetC_Forecast_ID() { Object ii = Get_Value("C_Forecast_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity Line.
@param C_ProjectLine_ID Task or step in a project */
        public void SetC_ProjectLine_ID(int C_ProjectLine_ID)
        {
            if (C_ProjectLine_ID <= 0) Set_Value("C_ProjectLine_ID", null);
            else
                Set_Value("C_ProjectLine_ID", C_ProjectLine_ID);
        }/** Get Opportunity Line.
@return Task or step in a project */
        public int GetC_ProjectLine_ID() { Object ii = Get_Value("C_ProjectLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity.
@param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }/** Get Opportunity.
@return Business Opportunity */
        public int GetC_Project_ID() { Object ii = Get_Value("C_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Bill of Materials.
@param IsBOM Bill of Materials */
        public void SetIsBOM(Boolean IsBOM) { Set_Value("IsBOM", IsBOM); }/** Get Bill of Materials.
@return Bill of Materials */
        public Boolean IsBOM() { Object oo = Get_Value("IsBOM"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Line No.
@param Line Unique line for this document */
        public void SetLine(int Line) { Set_Value("Line", Line); }/** Get Line No.
@return Unique line for this document */
        public int GetLine() { Object ii = Get_Value("Line"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetM_AttributeSetInstance_ID().ToString()); }/** Set BOM.
@param M_BOM_ID Bill of Material */
        public void SetM_BOM_ID(int M_BOM_ID)
        {
            if (M_BOM_ID <= 0) Set_Value("M_BOM_ID", null);
            else
                Set_Value("M_BOM_ID", M_BOM_ID);
        }/** Get BOM.
@return Bill of Material */
        public int GetM_BOM_ID() { Object ii = Get_Value("M_BOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Standard Price.
@param PriceStd Standard Price */
        public void SetPriceStd(Decimal? PriceStd) { Set_Value("PriceStd", (Decimal?)PriceStd); }/** Get Standard Price.
@return Standard Price */
        public Decimal GetPriceStd() { Object bd = Get_Value("PriceStd"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered) { Set_Value("QtyEntered", (Decimal?)QtyEntered); }/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered() { Object bd = Get_Value("QtyEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total price.
@param TotalPrice Displays the Total Amount calculated based on Unit Price and Base Quantity. */
        public void SetTotalPrice(Decimal? TotalPrice) { Set_Value("TotalPrice", (Decimal?)TotalPrice); }/** Get Total price.
@return Displays the Total Amount calculated based on Unit Price and Base Quantity. */
        public Decimal GetTotalPrice() { Object bd = Get_Value("TotalPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Unit Price.
@param UnitPrice Unit Price */
        public void SetUnitPrice(Decimal? UnitPrice) { Set_Value("UnitPrice", (Decimal?)UnitPrice); }/** Get Unit Price.
@return Unit Price */
        public Decimal GetUnitPrice() { Object bd = Get_Value("UnitPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}