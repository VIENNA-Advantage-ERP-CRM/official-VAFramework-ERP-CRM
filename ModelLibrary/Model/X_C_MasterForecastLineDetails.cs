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
    using System.Data;/** Generated Model for C_MasterForecastLineDetails
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_MasterForecastLineDetails : PO
    {
        public X_C_MasterForecastLineDetails(Context ctx, int C_MasterForecastLineDetails_ID, Trx trxName) : base(ctx, C_MasterForecastLineDetails_ID, trxName)
        {/** if (C_MasterForecastLineDetails_ID == 0){SetC_MasterForecastLineDetails_ID (0);} */
        }
        public X_C_MasterForecastLineDetails(Ctx ctx, int C_MasterForecastLineDetails_ID, Trx trxName) : base(ctx, C_MasterForecastLineDetails_ID, trxName)
        {/** if (C_MasterForecastLineDetails_ID == 0){SetC_MasterForecastLineDetails_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLineDetails(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLineDetails(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_MasterForecastLineDetails(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_MasterForecastLineDetails() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27901233458462L;/** Last Updated Timestamp 4/22/2021 4:15:41 PM */
        public static long updatedMS = 1619108141673L;/** AD_Table_ID=1000550 */
        public static int Table_ID; // =1000550;
        /** TableName=C_MasterForecastLineDetails */
        public static String Table_Name = "C_MasterForecastLineDetails";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_MasterForecastLineDetails[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Charge.
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
        public void SetC_ForecastLine_ID(int C_ForecastLine_ID)
        {
            if (C_ForecastLine_ID <= 0) Set_Value("C_ForecastLine_ID", null);
            else
                Set_Value("C_ForecastLine_ID", C_ForecastLine_ID);
        }/** Get C_ForecastLine_ID.
@return C_ForecastLine_ID */
        public int GetC_ForecastLine_ID() { Object ii = Get_Value("C_ForecastLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** C_Forecast_ID AD_Reference_ID=1000241 */
        public static int C_FORECAST_ID_AD_Reference_ID = 1000241;/** Set Forecast.
@param C_Forecast_ID Forecast */
        public void SetC_Forecast_ID(int C_Forecast_ID)
        {
            if (C_Forecast_ID <= 0) Set_Value("C_Forecast_ID", null);
            else
                Set_Value("C_Forecast_ID", C_Forecast_ID);
        }/** Get Forecast.
@return Forecast */
        public int GetC_Forecast_ID() { Object ii = Get_Value("C_Forecast_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set C_MasterForecastLineDetails_ID.
@param C_MasterForecastLineDetails_ID C_MasterForecastLineDetails_ID */
        public void SetC_MasterForecastLineDetails_ID(int C_MasterForecastLineDetails_ID) { if (C_MasterForecastLineDetails_ID < 1) throw new ArgumentException("C_MasterForecastLineDetails_ID is mandatory."); Set_ValueNoCheck("C_MasterForecastLineDetails_ID", C_MasterForecastLineDetails_ID); }/** Get C_MasterForecastLineDetails_ID.
@return C_MasterForecastLineDetails_ID */
        public int GetC_MasterForecastLineDetails_ID() { Object ii = Get_Value("C_MasterForecastLineDetails_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set C_MasterForecastLine_ID.
@param C_MasterForecastLine_ID C_MasterForecastLine_ID */
        public void SetC_MasterForecastLine_ID(int C_MasterForecastLine_ID)
        {
            if (C_MasterForecastLine_ID <= 0) Set_Value("C_MasterForecastLine_ID", null);
            else
                Set_Value("C_MasterForecastLine_ID", C_MasterForecastLine_ID);
        }/** Get C_MasterForecastLine_ID.
@return C_MasterForecastLine_ID */
        public int GetC_MasterForecastLine_ID() { Object ii = Get_Value("C_MasterForecastLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** C_Order_ID AD_Reference_ID=290 */
        public static int C_ORDER_ID_AD_Reference_ID = 290;/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Project Line.
@param C_ProjectLine_ID Task or step in a project */
        public void SetC_ProjectLine_ID(int C_ProjectLine_ID)
        {
            if (C_ProjectLine_ID <= 0) Set_Value("C_ProjectLine_ID", null);
            else
                Set_Value("C_ProjectLine_ID", C_ProjectLine_ID);
        }/** Get Project Line.
@return Task or step in a project */
        public int GetC_ProjectLine_ID() { Object ii = Get_Value("C_ProjectLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** C_Project_ID AD_Reference_ID=1000156 */
        public static int C_PROJECT_ID_AD_Reference_ID = 1000156;/** Set Project.
@param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }/** Get Project.
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
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Line.
@param LineNo Line No */
        public void SetLineNo(int LineNo) { Set_Value("LineNo", LineNo); }/** Get Line.
@return Line No */
        public int GetLineNo() { Object ii = Get_Value("LineNo"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** M_Product_ID AD_Reference_ID=1000240 */
        public static int M_PRODUCT_ID_AD_Reference_ID = 1000240;/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Price.
@param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered) { Set_Value("PriceEntered", (Decimal?)PriceEntered); }/** Get Price.
@return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered() { Object bd = Get_Value("PriceEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered) { Set_Value("QtyEntered", (Decimal?)QtyEntered); }/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered() { Object bd = Get_Value("QtyEntered"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Amount.
@param TotaAmt Total Amount */
        public void SetTotaAmt(Decimal? TotaAmt) { Set_Value("TotaAmt", (Decimal?)TotaAmt); }/** Get Total Amount.
@return Total Amount */
        public Decimal GetTotaAmt() { Object bd = Get_Value("TotaAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}