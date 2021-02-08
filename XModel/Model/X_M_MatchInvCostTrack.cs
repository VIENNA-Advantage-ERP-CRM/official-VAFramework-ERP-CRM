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
    using System.Data;/** Generated Model for VAM_MatchInvoiceoiceCostTrack
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_MatchInvoiceoiceCostTrack : PO
    {
        public X_VAM_MatchInvoiceoiceCostTrack(Context ctx, int VAM_MatchInvoiceoiceCostTrack_ID, Trx trxName)
            : base(ctx, VAM_MatchInvoiceoiceCostTrack_ID, trxName)
        {/** if (VAM_MatchInvoiceoiceCostTrack_ID == 0){SetVAM_MatchInvoiceoiceCostTrack_ID (0);} */
        }
        public X_VAM_MatchInvoiceoiceCostTrack(Ctx ctx, int VAM_MatchInvoiceoiceCostTrack_ID, Trx trxName)
            : base(ctx, VAM_MatchInvoiceoiceCostTrack_ID, trxName)
        {/** if (VAM_MatchInvoiceoiceCostTrack_ID == 0){SetVAM_MatchInvoiceoiceCostTrack_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_MatchInvoiceoiceCostTrack(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_MatchInvoiceoiceCostTrack(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_MatchInvoiceoiceCostTrack(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAM_MatchInvoiceoiceCostTrack() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27793156050767L;/** Last Updated Timestamp 11/19/2017 12:15:34 AM */
        public static long updatedMS = 1511030733978L;/** VAF_TableView_ID=1000806 */
        public static int Table_ID; // =1000806;
        /** TableName=VAM_MatchInvoiceoiceCostTrack */
        public static String Table_Name = "VAM_MatchInvoiceoiceCostTrack";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAM_MatchInvoiceoiceCostTrack[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Invoice Line.
@param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_Value("VAB_InvoiceLine_ID", null);
            else
                Set_Value("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }/** Get Invoice Line.
@return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID() { Object ii = Get_Value("VAB_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Shipment/Receipt Line.
@param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_Value("VAM_Inv_InOutLine_ID", null);
            else
                Set_Value("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID() { Object ii = Get_Value("VAM_Inv_InOutLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set VAM_MatchInvoiceoiceCostTrack_ID.
@param VAM_MatchInvoiceoiceCostTrack_ID VAM_MatchInvoiceoiceCostTrack_ID */
        public void SetVAM_MatchInvoiceoiceCostTrack_ID(int VAM_MatchInvoiceoiceCostTrack_ID) { if (VAM_MatchInvoiceoiceCostTrack_ID < 1) throw new ArgumentException("VAM_MatchInvoiceoiceCostTrack_ID is mandatory."); Set_ValueNoCheck("VAM_MatchInvoiceoiceCostTrack_ID", VAM_MatchInvoiceoiceCostTrack_ID); }/** Get VAM_MatchInvoiceoiceCostTrack_ID.
@return VAM_MatchInvoiceoiceCostTrack_ID */
        public int GetVAM_MatchInvoiceoiceCostTrack_ID() { Object ii = Get_Value("VAM_MatchInvoiceoiceCostTrack_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID <= 0) Set_Value("VAM_Product_ID", null);
            else
                Set_Value("VAM_Product_ID", VAM_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetVAM_Product_ID() { Object ii = Get_Value("VAM_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Quantity.
@param Qty Quantity */
        public void SetQty(Decimal? Qty) { Set_Value("Qty", (Decimal?)Qty); }/** Get Quantity.
@return Quantity */
        public Decimal GetQty() { Object bd = Get_Value("Qty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Cost Calculated.
        @param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }/** Get Cost Calculated.
@return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Cost Immediately.
@param IsCostImmediate Update Costs immediately for testing */
        public void SetIsCostImmediate(Boolean IsCostImmediate) { Set_Value("IsCostImmediate", IsCostImmediate); }/** Get Cost Immediately.
@return Update Costs immediately for testing */
        public Boolean IsCostImmediate() { Object oo = Get_Value("IsCostImmediate"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Match Invoice.
        @param VAM_MatchInvoice_ID Match Shipment/Receipt to Invoice */
        public void SetVAM_MatchInvoice_ID(int VAM_MatchInvoice_ID)
        {
            if (VAM_MatchInvoice_ID <= 0) Set_Value("VAM_MatchInvoice_ID", null);
            else
                Set_Value("VAM_MatchInvoice_ID", VAM_MatchInvoice_ID);
        }/** Get Match Invoice.
@return Match Shipment/Receipt to Invoice */
        public int GetVAM_MatchInvoice_ID() { Object ii = Get_Value("VAM_MatchInvoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Rev_VAB_InvoiceLine_ID VAF_Control_Ref_ID=1000371 */
        public static int REV_VAB_INVOICELINE_ID_VAF_Control_Ref_ID = 1000371;/** Set Reverse Invoice Line.
@param Rev_VAB_InvoiceLine_ID Reverse Invoice Line */
        public void SetRev_VAB_InvoiceLine_ID(int Rev_VAB_InvoiceLine_ID)
        {
            if (Rev_VAB_InvoiceLine_ID <= 0) Set_Value("Rev_VAB_InvoiceLine_ID", null);
            else
                Set_Value("Rev_VAB_InvoiceLine_ID", Rev_VAB_InvoiceLine_ID);
        }/** Get Reverse Invoice Line.
@return Reverse Invoice Line */
        public int GetRev_VAB_InvoiceLine_ID() { Object ii = Get_Value("Rev_VAB_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}