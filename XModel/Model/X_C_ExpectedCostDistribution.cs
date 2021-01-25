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
    using System.Data;/** Generated Model for VAB_ExpectedCostDis
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_ExpectedCostDis : PO
    {
        public X_VAB_ExpectedCostDis(Context ctx, int VAB_ExpectedCostDis_ID, Trx trxName) : base(ctx, VAB_ExpectedCostDis_ID, trxName)
        {/** if (VAB_ExpectedCostDis_ID == 0){SetAmt (0.0);SetBase (0.0);SetVAB_ExpectedCostDis_ID (0);SetVAB_ExpectedCost_ID (0);SetQty (0.0);} */
        }
        public X_VAB_ExpectedCostDis(Ctx ctx, int VAB_ExpectedCostDis_ID, Trx trxName) : base(ctx, VAB_ExpectedCostDis_ID, trxName)
        {/** if (VAB_ExpectedCostDis_ID == 0){SetAmt (0.0);SetBase (0.0);SetVAB_ExpectedCostDis_ID (0);SetVAB_ExpectedCost_ID (0);SetQty (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_ExpectedCostDis(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_ExpectedCostDis(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_ExpectedCostDis(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_ExpectedCostDis() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27857572242045L;/** Last Updated Timestamp 12/4/2019 1:38:45 PM */
        public static long updatedMS = 1575446925256L;/** VAF_TableView_ID=1000536 */
        public static int Table_ID; // =1000536;
                                    /** TableName=VAB_ExpectedCostDis */
        public static String Table_Name = "VAB_ExpectedCostDis";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_ExpectedCostDis[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Amount.
@param Amt Amount */
        public void SetAmt(Decimal? Amt) { if (Amt == null) throw new ArgumentException("Amt is mandatory."); Set_Value("Amt", (Decimal?)Amt); }/** Get Amount.
@return Amount */
        public Decimal GetAmt() { Object bd = Get_Value("Amt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Base.
@param Base Calculation Base */
        public void SetBase(Decimal? Base) { if (Base == null) throw new ArgumentException("Base is mandatory."); Set_Value("Base", (Decimal?)Base); }/** Get Base.
@return Calculation Base */
        public Decimal GetBase() { Object bd = Get_Value("Base"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set VAB_ExpectedCostDis_ID.
@param VAB_ExpectedCostDis_ID VAB_ExpectedCostDis_ID */
        public void SetVAB_ExpectedCostDis_ID(int VAB_ExpectedCostDis_ID) { if (VAB_ExpectedCostDis_ID < 1) throw new ArgumentException("VAB_ExpectedCostDis_ID is mandatory."); Set_ValueNoCheck("VAB_ExpectedCostDis_ID", VAB_ExpectedCostDis_ID); }/** Get VAB_ExpectedCostDis_ID.
@return VAB_ExpectedCostDis_ID */
        public int GetVAB_ExpectedCostDis_ID() { Object ii = Get_Value("VAB_ExpectedCostDis_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Expected Landed Cost.
@param VAB_ExpectedCost_ID Expected Landed Cost */
        public void SetVAB_ExpectedCost_ID(int VAB_ExpectedCost_ID) { if (VAB_ExpectedCost_ID < 1) throw new ArgumentException("VAB_ExpectedCost_ID is mandatory."); Set_ValueNoCheck("VAB_ExpectedCost_ID", VAB_ExpectedCost_ID); }/** Get Expected Landed Cost.
@return Expected Landed Cost */
        public int GetVAB_ExpectedCost_ID() { Object ii = Get_Value("VAB_ExpectedCost_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Cost Calculated.
@param IsCostCalculated This checkbox will auto set "True", when the cost is calculated for the document. */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }/** Get Cost Calculated.
@return This checkbox will auto set "True", when the cost is calculated for the document. */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param Qty Quantity */
        public void SetQty(Decimal? Qty) { if (Qty == null) throw new ArgumentException("Qty is mandatory."); Set_Value("Qty", (Decimal?)Qty); }/** Get Quantity.
@return Quantity */
        public Decimal GetQty() { Object bd = Get_Value("Qty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}