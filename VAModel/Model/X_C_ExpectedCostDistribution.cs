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
    using System.Data;/** Generated Model for C_ExpectedCostDistribution
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ExpectedCostDistribution : PO
    {
        public X_C_ExpectedCostDistribution(Context ctx, int C_ExpectedCostDistribution_ID, Trx trxName) : base(ctx, C_ExpectedCostDistribution_ID, trxName)
        {/** if (C_ExpectedCostDistribution_ID == 0){SetAmt (0.0);SetBase (0.0);SetC_ExpectedCostDistribution_ID (0);SetC_ExpectedCost_ID (0);SetQty (0.0);} */
        }
        public X_C_ExpectedCostDistribution(Ctx ctx, int C_ExpectedCostDistribution_ID, Trx trxName) : base(ctx, C_ExpectedCostDistribution_ID, trxName)
        {/** if (C_ExpectedCostDistribution_ID == 0){SetAmt (0.0);SetBase (0.0);SetC_ExpectedCostDistribution_ID (0);SetC_ExpectedCost_ID (0);SetQty (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCostDistribution(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCostDistribution(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCostDistribution(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ExpectedCostDistribution() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27857572242045L;/** Last Updated Timestamp 12/4/2019 1:38:45 PM */
        public static long updatedMS = 1575446925256L;/** AD_Table_ID=1000536 */
        public static int Table_ID; // =1000536;
                                    /** TableName=C_ExpectedCostDistribution */
        public static String Table_Name = "C_ExpectedCostDistribution";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ExpectedCostDistribution[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Amount.
@param Amt Amount */
        public void SetAmt(Decimal? Amt) { if (Amt == null) throw new ArgumentException("Amt is mandatory."); Set_Value("Amt", (Decimal?)Amt); }/** Get Amount.
@return Amount */
        public Decimal GetAmt() { Object bd = Get_Value("Amt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Base.
@param Base Calculation Base */
        public void SetBase(Decimal? Base) { if (Base == null) throw new ArgumentException("Base is mandatory."); Set_Value("Base", (Decimal?)Base); }/** Get Base.
@return Calculation Base */
        public Decimal GetBase() { Object bd = Get_Value("Base"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set C_ExpectedCostDistribution_ID.
@param C_ExpectedCostDistribution_ID C_ExpectedCostDistribution_ID */
        public void SetC_ExpectedCostDistribution_ID(int C_ExpectedCostDistribution_ID) { if (C_ExpectedCostDistribution_ID < 1) throw new ArgumentException("C_ExpectedCostDistribution_ID is mandatory."); Set_ValueNoCheck("C_ExpectedCostDistribution_ID", C_ExpectedCostDistribution_ID); }/** Get C_ExpectedCostDistribution_ID.
@return C_ExpectedCostDistribution_ID */
        public int GetC_ExpectedCostDistribution_ID() { Object ii = Get_Value("C_ExpectedCostDistribution_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Expected Landed Cost.
@param C_ExpectedCost_ID Expected Landed Cost */
        public void SetC_ExpectedCost_ID(int C_ExpectedCost_ID) { if (C_ExpectedCost_ID < 1) throw new ArgumentException("C_ExpectedCost_ID is mandatory."); Set_ValueNoCheck("C_ExpectedCost_ID", C_ExpectedCost_ID); }/** Get Expected Landed Cost.
@return Expected Landed Cost */
        public int GetC_ExpectedCost_ID() { Object ii = Get_Value("C_ExpectedCost_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
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