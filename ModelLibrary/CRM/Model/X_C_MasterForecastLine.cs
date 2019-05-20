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
    using System.Data;
    /** Generated Model for C_MasterForecastLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_MasterForecastLine : PO
    {
        public X_C_MasterForecastLine(Context ctx, int C_MasterForecastLine_ID, Trx trxName)
            : base(ctx, C_MasterForecastLine_ID, trxName)
        {
            /** if (C_MasterForecastLine_ID == 0)
            {
            SetC_MasterForecastLine_ID (0);
            SetC_MasterForecast_ID (0);
            }
             */
        }
        public X_C_MasterForecastLine(Ctx ctx, int C_MasterForecastLine_ID, Trx trxName)
            : base(ctx, C_MasterForecastLine_ID, trxName)
        {
            /** if (C_MasterForecastLine_ID == 0)
            {
            SetC_MasterForecastLine_ID (0);
            SetC_MasterForecast_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_MasterForecastLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_MasterForecastLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_MasterForecastLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_MasterForecastLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27609451526566L;
        /** Last Updated Timestamp 1/23/2012 7:13:29 PM */
        public static long updatedMS = 1327326209777L;
        /** AD_Table_ID=1000248 */
        public static int Table_ID;
        // =1000248;

        /** TableName=C_MasterForecastLine */
        public static String Table_Name = "C_MasterForecastLine";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_C_MasterForecastLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set C_MasterForecastLine_ID.
        @param C_MasterForecastLine_ID C_MasterForecastLine_ID */
        public void SetC_MasterForecastLine_ID(int C_MasterForecastLine_ID)
        {
            if (C_MasterForecastLine_ID < 1) throw new ArgumentException("C_MasterForecastLine_ID is mandatory.");
            Set_ValueNoCheck("C_MasterForecastLine_ID", C_MasterForecastLine_ID);
        }
        /** Get C_MasterForecastLine_ID.
        @return C_MasterForecastLine_ID */
        public int GetC_MasterForecastLine_ID()
        {
            Object ii = Get_Value("C_MasterForecastLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Forecast.
        @param C_MasterForecast_ID Forecast */
        public void SetC_MasterForecast_ID(int C_MasterForecast_ID)
        {
            if (C_MasterForecast_ID < 1) throw new ArgumentException("C_MasterForecast_ID is mandatory.");
            Set_ValueNoCheck("C_MasterForecast_ID", C_MasterForecast_ID);
        }
        /** Get Forecast.
        @return Forecast */
        public int GetC_MasterForecast_ID()
        {
            Object ii = Get_Value("C_MasterForecast_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Forcast Qty..
        @param ForcastQty Forcast Qty. */
        public void SetForcastQty(Decimal? ForcastQty)
        {
            Set_Value("ForcastQty", (Decimal?)ForcastQty);
        }
        /** Get Forcast Qty..
        @return Forcast Qty. */
        public Decimal GetForcastQty()
        {
            Object bd = Get_Value("ForcastQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Opp Qty..
        @param OppQty Opp Qty. */
        public void SetOppQty(Decimal? OppQty)
        {
            Set_Value("OppQty", (Decimal?)OppQty);
        }
        /** Get Opp Qty..
        @return Opp Qty. */
        public Decimal GetOppQty()
        {
            Object bd = Get_Value("OppQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price.
        @param Price Price */
        public void SetPrice(Decimal? Price)
        {
            Set_Value("Price", (Decimal?)Price);
        }
        /** Get Price.
        @return Price */
        public Decimal GetPrice()
        {
            Object bd = Get_Value("Price");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Total Quantity.
        @param TotalQty Total Quantity */
        public void SetTotalQty(Decimal? TotalQty)
        {
            Set_Value("TotalQty", (Decimal?)TotalQty);
        }
        /** Get Total Quantity.
        @return Total Quantity */
        public Decimal GetTotalQty()
        {
            Object bd = Get_Value("TotalQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Planned Revenue.
        @param PlannedRevenue Planned Revenue */
        public void SetPlannedRevenue(Decimal? PlannedRevenue)
        {
            Set_Value("PlannedRevenue", (Decimal?)PlannedRevenue);
        }
        /** Get Planned Revenue.
        @return Planned Revenue */
        public Decimal GetPlannedRevenue()
        {
            Object bd = Get_Value("PlannedRevenue");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
