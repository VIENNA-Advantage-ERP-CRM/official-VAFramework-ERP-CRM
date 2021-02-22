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
    using System.Data;/** Generated Model for VAM_Prod_StockSummary
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_Prod_StockSummary : PO
    {
        public X_VAM_Prod_StockSummary(Context ctx, int VAM_Prod_StockSummary_ID, Trx trxName)
            : base(ctx, VAM_Prod_StockSummary_ID, trxName)
        {/** if (VAM_Prod_StockSummary_ID == 0){SetVAM_Prod_StockSummary_ID (0);SetVAM_Product_ID (0);SetMovementFromDate (DateTime.Now);} */
        }
        public X_VAM_Prod_StockSummary(Ctx ctx, int VAM_Prod_StockSummary_ID, Trx trxName)
            : base(ctx, VAM_Prod_StockSummary_ID, trxName)
        {/** if (VAM_Prod_StockSummary_ID == 0){SetVAM_Prod_StockSummary_ID (0);SetVAM_Product_ID (0);SetMovementFromDate (DateTime.Now);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_Prod_StockSummary(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_Prod_StockSummary(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_Prod_StockSummary(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAM_Prod_StockSummary() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27760365347817L;/** Last Updated Timestamp 11/4/2016 11:43:51 AM */
        public static long updatedMS = 1478240031028L;/** VAF_TableView_ID=1000499 */
        public static int Table_ID; // =1000499;
        /** TableName=VAM_Prod_StockSummary */
        public static String Table_Name = "VAM_Prod_StockSummary";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 4 - System 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAM_Prod_StockSummary[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Product Stock Summary.
@param VAM_Prod_StockSummary_ID Product Stock Summary */
        public void SetVAM_Prod_StockSummary_ID(int VAM_Prod_StockSummary_ID) { if (VAM_Prod_StockSummary_ID < 1) throw new ArgumentException("VAM_Prod_StockSummary_ID is mandatory."); Set_ValueNoCheck("VAM_Prod_StockSummary_ID", VAM_Prod_StockSummary_ID); }/** Get Product Stock Summary.
@return Product Stock Summary */
        public int GetVAM_Prod_StockSummary_ID() { Object ii = Get_Value("VAM_Prod_StockSummary_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID) { if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory."); Set_ValueNoCheck("VAM_Product_ID", VAM_Product_ID); }/** Get Product.
@return Product, Service, Item */
        public int GetVAM_Product_ID() { Object ii = Get_Value("VAM_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Movement From Date.
@param MovementFromDate Movement From Date */
        public void SetMovementFromDate(DateTime? MovementFromDate) { if (MovementFromDate == null) throw new ArgumentException("MovementFromDate is mandatory."); Set_ValueNoCheck("MovementFromDate", (DateTime?)MovementFromDate); }/** Get Movement From Date.
@return Movement From Date */
        public DateTime? GetMovementFromDate() { return (DateTime?)Get_Value("MovementFromDate"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetMovementFromDate().ToString()); }/** Set Movement To Date.
@param MovementToDate Movement To Date */
        public void SetMovementToDate(DateTime? MovementToDate) { Set_Value("MovementToDate", (DateTime?)MovementToDate); }/** Get Movement To Date.
@return Movement To Date */
        public DateTime? GetMovementToDate() { return (DateTime?)Get_Value("MovementToDate"); }/** Set Closing Stock All Organization.
@param QtyCloseStockAllORg Closing Stock All Organization */
        public void SetQtyCloseStockAllORg(Decimal? QtyCloseStockAllORg) { Set_Value("QtyCloseStockAllORg", (Decimal?)QtyCloseStockAllORg); }/** Get Closing Stock All Organization.
@return Closing Stock All Organization */
        public Decimal GetQtyCloseStockAllORg() { Object bd = Get_Value("QtyCloseStockAllORg"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Closing Stock.
@param QtyCloseStockOrg Closing Stock */
        public void SetQtyCloseStockOrg(Decimal? QtyCloseStockOrg) { Set_Value("QtyCloseStockOrg", (Decimal?)QtyCloseStockOrg); }/** Get Closing Stock.
@return Closing Stock */
        public Decimal GetQtyCloseStockOrg() { Object bd = Get_Value("QtyCloseStockOrg"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Opening Stock All Organization.
@param QtyOpenStockAllOrg Opening Stock All Organization */
        public void SetQtyOpenStockAllOrg(Decimal? QtyOpenStockAllOrg) { Set_Value("QtyOpenStockAllOrg", (Decimal?)QtyOpenStockAllOrg); }/** Get Opening Stock All Organization.
@return Opening Stock All Organization */
        public Decimal GetQtyOpenStockAllOrg() { Object bd = Get_Value("QtyOpenStockAllOrg"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Opening Stock.
@param QtyOpenStockOrg Opening Stock */
        public void SetQtyOpenStockOrg(Decimal? QtyOpenStockOrg)
        {
            Set_Value("QtyOpenStockOrg", (Decimal?)QtyOpenStockOrg);
        }
        /** Get Opening Stock.
    @return Opening Stock */
        public Decimal GetQtyOpenStockOrg()
        {
            Object bd = Get_Value("QtyOpenStockOrg"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Stock Summarized.
        @param IsStockSummarized Stock Summarized */
        public void SetIsStockSummarized(Boolean IsStockSummarized)
        {
            Set_Value("IsStockSummarized", IsStockSummarized);
        }
        /** Get Stock Summarized.
        @return Stock Summarized */
        public Boolean IsStockSummarized()
        {
            Object oo = Get_Value("IsStockSummarized"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false;
        }
    }
}