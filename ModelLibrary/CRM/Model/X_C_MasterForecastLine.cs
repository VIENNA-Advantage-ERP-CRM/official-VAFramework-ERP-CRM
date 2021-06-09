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
    using System.Data;/** Generated Model for C_MasterForecastLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_MasterForecastLine : PO
    {
        public X_C_MasterForecastLine(Context ctx, int C_MasterForecastLine_ID, Trx trxName) : base(ctx, C_MasterForecastLine_ID, trxName)
        {/** if (C_MasterForecastLine_ID == 0){SetC_MasterForecastLine_ID (0);SetC_MasterForecast_ID (0);} */
        }
        public X_C_MasterForecastLine(Ctx ctx, int C_MasterForecastLine_ID, Trx trxName) : base(ctx, C_MasterForecastLine_ID, trxName)
        {/** if (C_MasterForecastLine_ID == 0){SetC_MasterForecastLine_ID (0);SetC_MasterForecast_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_MasterForecastLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27900679065602L;/** Last Updated Timestamp 4/16/2021 6:15:48 AM */
        public static long updatedMS = 1618553748813L;/** AD_Table_ID=1000248 */
        public static int Table_ID; // =1000248;
        /** TableName=C_MasterForecastLine */
        public static String Table_Name = "C_MasterForecastLine";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_MasterForecastLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Adjusted Quantity.
@param AdjustedQty Adjusted Quantity */
        public void SetAdjustedQty(Decimal? AdjustedQty) { Set_Value("AdjustedQty", (Decimal?)AdjustedQty); }/** Get Adjusted Quantity.
@return Adjusted Quantity */
        public Decimal GetAdjustedQty() { Object bd = Get_Value("AdjustedQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
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
        public String GetBOMUse() { return (String)Get_Value("BOMUse"); }/** Set Charge.
@param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }/** Get Charge.
@return Additional document charges */
        public int GetC_Charge_ID() { Object ii = Get_Value("C_Charge_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set C_MasterForecastLine_ID.
@param C_MasterForecastLine_ID C_MasterForecastLine_ID */
        public void SetC_MasterForecastLine_ID(int C_MasterForecastLine_ID) { if (C_MasterForecastLine_ID < 1) throw new ArgumentException("C_MasterForecastLine_ID is mandatory."); Set_ValueNoCheck("C_MasterForecastLine_ID", C_MasterForecastLine_ID); }/** Get C_MasterForecastLine_ID.
@return C_MasterForecastLine_ID */
        public int GetC_MasterForecastLine_ID() { Object ii = Get_Value("C_MasterForecastLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Forecast.
@param C_MasterForecast_ID Master Forecast */
        public void SetC_MasterForecast_ID(int C_MasterForecast_ID) { if (C_MasterForecast_ID < 1) throw new ArgumentException("C_MasterForecast_ID is mandatory."); Set_ValueNoCheck("C_MasterForecast_ID", C_MasterForecast_ID); }/** Get Forecast.
@return Master Forecast */
        public int GetC_MasterForecast_ID() { Object ii = Get_Value("C_MasterForecast_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set UOM.
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
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Forecast Qty..
@param ForcastQty Master forecast quantity */
        public void SetForcastQty(Decimal? ForcastQty) { Set_Value("ForcastQty", (Decimal?)ForcastQty); }/** Get Forecast Qty..
@return Master forecast quantity */
        public Decimal GetForcastQty() { Object bd = Get_Value("ForcastQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Bill of Materials.
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
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set BOM.
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
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opp Qty..
@param OppQty Master forecase opportunity quantities */
        public void SetOppQty(Decimal? OppQty) { Set_Value("OppQty", (Decimal?)OppQty); }/** Get Opp Qty..
@return Master forecase opportunity quantities */
        public Decimal GetOppQty() { Object bd = Get_Value("OppQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Planned Revenue.
@param PlannedRevenue Planned Revenue */
        public void SetPlannedRevenue(Decimal? PlannedRevenue) { Set_Value("PlannedRevenue", (Decimal?)PlannedRevenue); }/** Get Planned Revenue.
@return Planned Revenue */
        public Decimal GetPlannedRevenue() { Object bd = Get_Value("PlannedRevenue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Price.
@param Price Price */
        public void SetPrice(Decimal? Price) { Set_Value("Price", (Decimal?)Price); }/** Get Price.
@return Price */
        public Decimal GetPrice() { Object bd = Get_Value("Price"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Sales Order Qty..
@param SalesOrderQty In this field user will get the details of the sales order. */
        public void SetSalesOrderQty(Decimal? SalesOrderQty) { Set_Value("SalesOrderQty", (Decimal?)SalesOrderQty); }/** Get Sales Order Qty..
@return In this field user will get the details of the sales order. */
        public Decimal GetSalesOrderQty() { Object bd = Get_Value("SalesOrderQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Quantity.
@param TotalQty Total Quantity */
        public void SetTotalQty(Decimal? TotalQty) { Set_Value("TotalQty", (Decimal?)TotalQty); }/** Get Total Quantity.
@return Total Quantity */
        public Decimal GetTotalQty() { Object bd = Get_Value("TotalQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}