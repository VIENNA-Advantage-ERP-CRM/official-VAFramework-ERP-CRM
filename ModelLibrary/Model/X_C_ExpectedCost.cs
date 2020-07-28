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
    using System.Data;/** Generated Model for C_ExpectedCost
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ExpectedCost : PO
    {
        public X_C_ExpectedCost(Context ctx, int C_ExpectedCost_ID, Trx trxName) : base(ctx, C_ExpectedCost_ID, trxName)
        {/** if (C_ExpectedCost_ID == 0){SetAmt (0.0);SetC_ExpectedCost_ID (0);SetC_Order_ID (0);SetLandedCostDistribution (null);// Q
SetM_CostElement_ID (0);} */
        }
        public X_C_ExpectedCost(Ctx ctx, int C_ExpectedCost_ID, Trx trxName) : base(ctx, C_ExpectedCost_ID, trxName)
        {/** if (C_ExpectedCost_ID == 0){SetAmt (0.0);SetC_ExpectedCost_ID (0);SetC_Order_ID (0);SetLandedCostDistribution (null);// Q
SetM_CostElement_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCost(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCost(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ExpectedCost(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ExpectedCost() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27857572235483L;/** Last Updated Timestamp 12/4/2019 1:38:38 PM */
        public static long updatedMS = 1575446918694L;/** AD_Table_ID=1000535 */
        public static int Table_ID; // =1000535;
                                    /** TableName=C_ExpectedCost */
        public static String Table_Name = "C_ExpectedCost";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ExpectedCost[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Amount.
@param Amt Amount */
        public void SetAmt(Decimal? Amt) { if (Amt == null) throw new ArgumentException("Amt is mandatory."); Set_Value("Amt", (Decimal?)Amt); }/** Get Amount.
@return Amount */
        public Decimal GetAmt() { Object bd = Get_Value("Amt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Expected Landed Cost.
@param C_ExpectedCost_ID Expected Landed Cost */
        public void SetC_ExpectedCost_ID(int C_ExpectedCost_ID) { if (C_ExpectedCost_ID < 1) throw new ArgumentException("C_ExpectedCost_ID is mandatory."); Set_ValueNoCheck("C_ExpectedCost_ID", C_ExpectedCost_ID); }/** Get Expected Landed Cost.
@return Expected Landed Cost */
        public int GetC_ExpectedCost_ID() { Object ii = Get_Value("C_ExpectedCost_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID) { if (C_Order_ID < 1) throw new ArgumentException("C_Order_ID is mandatory."); Set_ValueNoCheck("C_Order_ID", C_Order_ID); }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }
        /** LandedCostDistribution AD_Reference_ID=339 */
        public static int LANDEDCOSTDISTRIBUTION_AD_Reference_ID = 339;/** Costs = C */
        public static String LANDEDCOSTDISTRIBUTION_Costs = "C";/** Invoice Value = I */
        public static String LANDEDCOSTDISTRIBUTION_InvoiceValue = "I";/** Line = L */
        public static String LANDEDCOSTDISTRIBUTION_Line = "L";/** Quantity = Q */
        public static String LANDEDCOSTDISTRIBUTION_Quantity = "Q";/** Volume = V */
        public static String LANDEDCOSTDISTRIBUTION_Volume = "V";/** Weight = W */
        public static String LANDEDCOSTDISTRIBUTION_Weight = "W";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsLandedCostDistributionValid(String test) { return test.Equals("C") || test.Equals("I") || test.Equals("L") || test.Equals("Q") || test.Equals("V") || test.Equals("W"); }/** Set Cost Distribution.
@param LandedCostDistribution Landed Cost Distribution */
        public void SetLandedCostDistribution(String LandedCostDistribution)
        {
            if (LandedCostDistribution == null) throw new ArgumentException("LandedCostDistribution is mandatory"); if (!IsLandedCostDistributionValid(LandedCostDistribution))
                throw new ArgumentException("LandedCostDistribution Invalid value - " + LandedCostDistribution + " - Reference_ID=339 - C - I - L - Q - V - W"); if (LandedCostDistribution.Length > 1) { log.Warning("Length > 1 - truncated"); LandedCostDistribution = LandedCostDistribution.Substring(0, 1); }
            Set_Value("LandedCostDistribution", LandedCostDistribution);
        }/** Get Cost Distribution.
@return Landed Cost Distribution */
        public String GetLandedCostDistribution() { return (String)Get_Value("LandedCostDistribution"); }/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
        public void SetM_CostElement_ID(int M_CostElement_ID) { if (M_CostElement_ID < 1) throw new ArgumentException("M_CostElement_ID is mandatory."); Set_Value("M_CostElement_ID", M_CostElement_ID); }/** Get Cost Element.
@return Product Cost Element */
        public int GetM_CostElement_ID() { Object ii = Get_Value("M_CostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}