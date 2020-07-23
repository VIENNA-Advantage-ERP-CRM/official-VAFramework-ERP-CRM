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
    using System.Data;/** Generated Model for M_MatchInvCostTrack
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_MatchInvCostTrack : PO
    {
        public X_M_MatchInvCostTrack(Context ctx, int M_MatchInvCostTrack_ID, Trx trxName)
            : base(ctx, M_MatchInvCostTrack_ID, trxName)
        {/** if (M_MatchInvCostTrack_ID == 0){SetM_MatchInvCostTrack_ID (0);} */
        }
        public X_M_MatchInvCostTrack(Ctx ctx, int M_MatchInvCostTrack_ID, Trx trxName)
            : base(ctx, M_MatchInvCostTrack_ID, trxName)
        {/** if (M_MatchInvCostTrack_ID == 0){SetM_MatchInvCostTrack_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_MatchInvCostTrack(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_MatchInvCostTrack(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_MatchInvCostTrack(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_MatchInvCostTrack() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27793156050767L;/** Last Updated Timestamp 11/19/2017 12:15:34 AM */
        public static long updatedMS = 1511030733978L;/** AD_Table_ID=1000806 */
        public static int Table_ID; // =1000806;
        /** TableName=M_MatchInvCostTrack */
        public static String Table_Name = "M_MatchInvCostTrack";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_MatchInvCostTrack[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
        public void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            if (C_InvoiceLine_ID <= 0) Set_Value("C_InvoiceLine_ID", null);
            else
                Set_Value("C_InvoiceLine_ID", C_InvoiceLine_ID);
        }/** Get Invoice Line.
@return Invoice Detail Line */
        public int GetC_InvoiceLine_ID() { Object ii = Get_Value("C_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_Value("M_InOutLine_ID", null);
            else
                Set_Value("M_InOutLine_ID", M_InOutLine_ID);
        }/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID() { Object ii = Get_Value("M_InOutLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set M_MatchInvCostTrack_ID.
@param M_MatchInvCostTrack_ID M_MatchInvCostTrack_ID */
        public void SetM_MatchInvCostTrack_ID(int M_MatchInvCostTrack_ID) { if (M_MatchInvCostTrack_ID < 1) throw new ArgumentException("M_MatchInvCostTrack_ID is mandatory."); Set_ValueNoCheck("M_MatchInvCostTrack_ID", M_MatchInvCostTrack_ID); }/** Get M_MatchInvCostTrack_ID.
@return M_MatchInvCostTrack_ID */
        public int GetM_MatchInvCostTrack_ID() { Object ii = Get_Value("M_MatchInvCostTrack_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Quantity.
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
        @param M_MatchInv_ID Match Shipment/Receipt to Invoice */
        public void SetM_MatchInv_ID(int M_MatchInv_ID)
        {
            if (M_MatchInv_ID <= 0) Set_Value("M_MatchInv_ID", null);
            else
                Set_Value("M_MatchInv_ID", M_MatchInv_ID);
        }/** Get Match Invoice.
@return Match Shipment/Receipt to Invoice */
        public int GetM_MatchInv_ID() { Object ii = Get_Value("M_MatchInv_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Rev_C_InvoiceLine_ID AD_Reference_ID=1000371 */
        public static int REV_C_INVOICELINE_ID_AD_Reference_ID = 1000371;/** Set Reverse Invoice Line.
@param Rev_C_InvoiceLine_ID Reverse Invoice Line */
        public void SetRev_C_InvoiceLine_ID(int Rev_C_InvoiceLine_ID)
        {
            if (Rev_C_InvoiceLine_ID <= 0) Set_Value("Rev_C_InvoiceLine_ID", null);
            else
                Set_Value("Rev_C_InvoiceLine_ID", Rev_C_InvoiceLine_ID);
        }/** Get Reverse Invoice Line.
@return Reverse Invoice Line */
        public int GetRev_C_InvoiceLine_ID() { Object ii = Get_Value("Rev_C_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}