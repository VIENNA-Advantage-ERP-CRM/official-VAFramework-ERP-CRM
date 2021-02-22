/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MUOMConversion
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Drawing;
using System.Globalization;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABUOMConversion : X_VAB_UOM_Conversion
    {
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABUOMConversion).FullName);
        //	Indicator for Rate				
        private static Decimal GETRATE = new Decimal(123.456);
        //	Conversion Map: Key=Point(from,to) Value=Decimal	
        private static CCache<Point, Decimal> _conversions = null;
        // Product Conversion Map					
        private static CCache<int, MVABUOMConversion[]> _conversionProduct = new CCache<int, MVABUOMConversion[]>("MVABUOMConversion", 20);


        /// <summary>
        /// Convert qty to target UOM and round.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_ID">from UOM</param>
        /// <param name="VAB_UOM_To_ID">to UOM</param>
        /// <param name="qty"></param>
        /// <returns>converted qty (std precision)</returns>
        static public Decimal? Convert(Ctx ctx, int VAB_UOM_ID, int VAB_UOM_To_ID, Decimal? qty)
        {
            if (qty == null || qty.Equals(Env.ZERO) || VAB_UOM_ID == VAB_UOM_To_ID)
            {
                return qty;
            }
            Decimal? retValue = GetRate(ctx, VAB_UOM_ID, VAB_UOM_To_ID);
            if (retValue != null)
            {
                MVABUOM uom = MVABUOM.Get(ctx, VAB_UOM_To_ID);
                if (uom != null)
                {
                    return uom.Round(Decimal.Multiply(retValue.Value, qty.Value), true);
                }
                return Decimal.Multiply(retValue.Value, qty.Value);
            }
            return null;
        }

        /// <summary>
        /// Get Multiplier Rate to target UOM
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="VAB_UOM_To_ID"></param>
        /// <returns>multiplier</returns>
        static public Decimal GetRate(Ctx ctx, int VAB_UOM_ID, int VAB_UOM_To_ID)
        {
            //	nothing to do
            if (VAB_UOM_ID == VAB_UOM_To_ID)
            {
                return Env.ONE;
            }
            //
            Point p = new Point(VAB_UOM_ID, VAB_UOM_To_ID);
            //	get conversion
            Decimal retValue = GetRate(ctx, p);
            return retValue;
        }


        /// <summary>
        /// Convert qty to target UOM and round.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="qty"></param>
        /// <returns>minutes - 0 if not found</returns>
        static public int ConvertToMinutes(Ctx ctx, int VAB_UOM_ID, Decimal? qty)
        {
            if (qty == null)
            {
                return 0;
            }
            int VAB_UOM_To_ID = MVABUOM.GetMinute_UOM_ID(ctx);
            if (VAB_UOM_ID == VAB_UOM_To_ID)
            {
                return Decimal.ToInt32(qty.Value);
            }

            Decimal? result = (Decimal?)Convert(ctx, VAB_UOM_ID, VAB_UOM_To_ID, qty);
            if (result == null)
            {
                return 0;
            }
            return Decimal.ToInt32(result.Value);
        }

        /// <summary>
        /// Calculate End Date based on start date and qty
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="startDate"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="qty"></param>
        /// <returns>end date</returns>
        public static DateTime? GetEndDate(Ctx ctx, DateTime? startDate, int VAB_UOM_ID, Decimal? qty)
        {
            //GregorianCalendar endDate = new GregorianCalendar();
            DateTime? endDate = new DateTime();
            //endDate.setTime(startDate);
            endDate = startDate;
            int minutes = MVABUOMConversion.ConvertToMinutes(ctx, VAB_UOM_ID, qty);
            //endDate.add(Calendar.MINUTE, minutes);
            //endDate.AddMinutes((Double)minutes);
            endDate = endDate.Value.AddMinutes(minutes);
            //DateTime retValue = new DateTime(endDate.getTimeInMillis());
            DateTime? retValue = endDate;
            //log.config( "TimeUtil.getEndDate", "Start=" + startDate
            //    + ", Qty=" + qty + ", End=" + retValue);
            return retValue;
        }

        /// <summary>
        /// Get Conversion Multiplier Rate, try to derive it if not found directly
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="p">Point with from(x) - to(y) VAB_UOM_ID</param>
        /// <returns>conversion multiplier or null</returns>
        static private Decimal GetRate(Ctx ctx, Point p)
        {
            Decimal? retValue = null;

            if (Ini.IsClient())
            {
                if (_conversions == null || _conversions.Keys.Count() == 0)
                {
                    CreateRates(ctx);
                }

                decimal result;
                if (_conversions.TryGetValue(p, out result))
                {
                    retValue = result;
                }
            }
            else
            {
                retValue = GetRate(int.Parse(p.X.ToString()), int.Parse(p.Y.ToString()));
            }
            if (retValue != null)
            {
                return (Decimal)retValue;
            }
            //	try to derive
            return (Decimal)DeriveRate(ctx, int.Parse(p.X.ToString()), int.Parse(p.Y.ToString()));
        }

        /// <summary>
        /// Create Conversion Matrix (Client)
        /// </summary>
        /// <param name="ctx"></param>
        private static void CreateRates(Ctx ctx)
        {
            _conversions = new CCache<Point, Decimal>("VAB_UOMConversion", 20);
            //
            String sql = MVAFRole.GetDefault(ctx, false).AddAccessSQL(
                "SELECT VAB_UOM_ID, VAB_UOM_To_ID, MultiplyRate, DivideRate "
                + "FROM VAB_UOM_Conversion "
                + "WHERE IsActive='Y' AND VAM_Product_ID IS NULL",
                "VAB_UOM_Conversion", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);

            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Point p = new Point(Utility.Util.GetValueOfInt(dr[0]), Utility.Util.GetValueOfInt(dr[1]));
                    Decimal mr = Utility.Util.GetValueOfDecimal(dr[2]);
                    Decimal deci = Utility.Util.GetValueOfDecimal(dr[3]);
                    if (mr != null)
                    {
                        _conversions.Add(p, mr);
                    }
                    //	reverse
                    if (deci == null && mr != null)
                    {
                        deci = Decimal.Round(Decimal.Divide(Env.ONE, mr), 2);//, MidpointRounding.AwayFromZero);
                    }
                    if (deci != null)
                    {
                        _conversions.Add(new Point(p.Y, p.X), deci);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
        }

        /// <summary>
        /// Derive Standard Conversions
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="VAB_UOM_To_ID"></param>
        /// <returns>Conversion or null</returns>
        public static Decimal? DeriveRate(Ctx ctx, int VAB_UOM_ID, int VAB_UOM_To_ID)
        {
            if (VAB_UOM_ID == VAB_UOM_To_ID)
            {
                return Env.ONE;
            }
            //	get Info
            MVABUOM from = MVABUOM.Get(ctx, VAB_UOM_ID);
            MVABUOM to = MVABUOM.Get(ctx, VAB_UOM_To_ID);
            if (from == null || to == null)
            {
                return null;
            }

            //	Time - Minute
            if (from.IsMinute())
            {
                if (to.IsHour())
                {
                    return new Decimal(1.0 / 60.0);
                }
                if (to.IsDay())
                {
                    return new Decimal(1.0 / 1440.0);		//	24 * 60
                }
                if (to.IsWorkDay())
                {
                    return new Decimal(1.0 / 480.0);		//	8 * 60
                }
                if (to.IsWeek())
                {
                    return new Decimal(1.0 / 10080.0);		//	7 * 24 * 60
                }
                if (to.IsMonth())
                {
                    return new Decimal(1.0 / 43200.0);		//	30 * 24 * 60
                }
                if (to.IsWorkMonth())
                {
                    return new Decimal(1.0 / 9600.0);		//	4 * 5 * 8 * 60
                }
                if (to.IsYear())
                {
                    return new Decimal(1.0 / 525600.0);	//	365 * 24 * 60
                }
            }
            //	Time - Hour
            if (from.IsHour())
            {
                if (to.IsMinute())
                {
                    return new Decimal(60.0);
                }
                if (to.IsDay())
                {
                    return new Decimal(1.0 / 24.0);
                }
                if (to.IsWorkDay())
                {
                    return new Decimal(1.0 / 8.0);
                }
                if (to.IsWeek())
                {
                    return new Decimal(1.0 / 168.0);		//	7 * 24
                }
                if (to.IsMonth())
                {
                    return new Decimal(1.0 / 720.0);		//	30 * 24
                }
                if (to.IsWorkMonth())
                {
                    return new Decimal(1.0 / 160.0);		//	4 * 5 * 8
                }
                if (to.IsYear())
                {
                    return new Decimal(1.0 / 8760.0);		//	365 * 24
                }
            }
            //	Time - Day
            if (from.IsDay())
            {
                if (to.IsMinute())
                    return new Decimal(1440.0);			//	24 * 60
                if (to.IsHour())
                    return new Decimal(24.0);
                if (to.IsWorkDay())
                    return new Decimal(3.0);				//	24 / 8
                if (to.IsWeek())
                    return new Decimal(1.0 / 7.0);			//	7
                if (to.IsMonth())
                    return new Decimal(1.0 / 30.0);		//	30
                if (to.IsWorkMonth())
                    return new Decimal(1.0 / 20.0);		//	4 * 5
                if (to.IsYear())
                    return new Decimal(1.0 / 365.0);		//	365
            }
            //	Time - WorkDay
            if (from.IsWorkDay())
            {
                if (to.IsMinute())
                    return new Decimal(480.0);			//	8 * 60
                if (to.IsHour())
                    return new Decimal(8.0);				//	8
                if (to.IsDay())
                    return new Decimal(1.0 / 3.0);			//	24 / 8
                if (to.IsWeek())
                    return new Decimal(1.0 / 5);			//	5
                if (to.IsMonth())
                    return new Decimal(1.0 / 20.0);		//	4 * 5
                if (to.IsWorkMonth())
                    return new Decimal(1.0 / 20.0);		//	4 * 5
                if (to.IsYear())
                    return new Decimal(1.0 / 240.0);		//	4 * 5 * 12
            }
            //	Time - Week
            if (from.IsWeek())
            {
                if (to.IsMinute())
                    return new Decimal(10080.0);			//	7 * 24 * 60
                if (to.IsHour())
                    return new Decimal(168.0);			//	7 * 24
                if (to.IsDay())
                    return new Decimal(7.0);
                if (to.IsWorkDay())
                    return new Decimal(5.0);
                if (to.IsMonth())
                    return new Decimal(1.0 / 4.0);			//	4
                if (to.IsWorkMonth())
                    return new Decimal(1.0 / 4.0);			//	4
                if (to.IsYear())
                    return new Decimal(1.0 / 50.0);		//	50
            }
            //	Time - Month
            if (from.IsMonth())
            {
                if (to.IsMinute())
                    return new Decimal(43200.0);			//	30 * 24 * 60
                if (to.IsHour())
                    return new Decimal(720.0);			//	30 * 24
                if (to.IsDay())
                    return new Decimal(30.0);			//	30
                if (to.IsWorkDay())
                    return new Decimal(20.0);			//	4 * 5
                if (to.IsWeek())
                    return new Decimal(4.0);				//	4
                if (to.IsWorkMonth())
                    return new Decimal(1.5);				//	30 / 20
                if (to.IsYear())
                    return new Decimal(1.0 / 12.0);		//	12
            }
            //	Time - WorkMonth
            if (from.IsWorkMonth())
            {
                if (to.IsMinute())
                    return new Decimal(9600.0);			//	4 * 5 * 8 * 60
                if (to.IsHour())
                    return new Decimal(160.0);			//	4 * 5 * 8
                if (to.IsDay())
                    return new Decimal(20.0);			//	4 * 5
                if (to.IsWorkDay())
                    return new Decimal(20.0);			//	4 * 5
                if (to.IsWeek())
                    return new Decimal(4.0);				//	4
                if (to.IsMonth())
                    return new Decimal(20.0 / 30.0);		//	20 / 30
                if (to.IsYear())
                    return new Decimal(1.0 / 12.0);		//	12
            }
            //	Time - Year
            if (from.IsYear())
            {
                if (to.IsMinute())
                    return new Decimal(518400.0);		//	12 * 30 * 24 * 60
                if (to.IsHour())
                    return new Decimal(8640.0);			//	12 * 30 * 24
                if (to.IsDay())
                    return new Decimal(365.0);			//	365
                if (to.IsWorkDay())
                    return new Decimal(240.0);			//	12 * 4 * 5
                if (to.IsWeek())
                    return new Decimal(50.0);			//	52
                if (to.IsMonth())
                    return new Decimal(12.0);			//	12
                if (to.IsWorkMonth())
                    return new Decimal(12.0);			//	12
            }
            //
            return null;
        }

        /// <summary>
        /// Get Conversion Multiplier Rate from Server
        /// </summary>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="VAB_UOM_To_ID"></param>
        /// <returns>conversion multiplier or null</returns>
        public static Decimal GetRate(int VAB_UOM_ID, int VAB_UOM_To_ID)
        {
            return (Decimal)Convert(VAB_UOM_ID, VAB_UOM_To_ID, GETRATE, false);
        }

        /// <summary>
        /// Get Converted Qty from Server (no cache)
        /// </summary>
        /// <param name="VAB_UOM_From_ID">The VAB_UOM_ID of the qty</param>
        /// <param name="VAB_UOM_To_ID">The targeted UOM</param>
        /// <param name="qty">The quantity to be converted</param>
        /// <param name="StdPrecision">if true, standard precision, if false costing precision</param>
        /// <returns>should not be used</returns>
        public static Decimal? Convert(int VAB_UOM_From_ID, int VAB_UOM_To_ID, Decimal qty, bool StdPrecision)
        {
            //  Nothing to do
            if (qty == null || qty.Equals(Env.ZERO) || VAB_UOM_From_ID == VAB_UOM_To_ID)
            {
                return qty;
            }
            //
            Decimal? retValue = null;
            int precision = 2;
            String sql = "SELECT c.MultiplyRate, uomTo.StdPrecision, uomTo.CostingPrecision "
                + "FROM	VAB_UOM_Conversion c"
                + " INNER JOIN VAB_UOM uomTo ON (c.VAB_UOM_To_ID=uomTo.VAB_UOM_ID) "
                + "WHERE c.IsActive='Y' AND c.VAB_UOM_ID=" + VAB_UOM_From_ID + " AND c.VAB_UOM_To_ID=" + VAB_UOM_To_ID		//	#1/2
                + " AND c.VAM_Product_ID IS NULL"
                + " ORDER BY c.VAF_Client_ID DESC, c.VAF_Org_ID DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Utility.Util.GetValueOfDecimal(dr[0]);
                    precision = Utility.Util.GetValueOfInt(dr[StdPrecision ? 2 : 3]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
            if (retValue == null)
            {
                _log.Info("NOT found - FromUOM=" + VAB_UOM_From_ID + ", ToUOM=" + VAB_UOM_To_ID);
                return null;
            }

            //	Just get Rate
            if (GETRATE.Equals(qty))
            {
                return (Decimal)retValue;
            }

            //	Calculate & Scale
            retValue = Decimal.Multiply((Decimal)retValue, qty);
            if (Env.Scale((Decimal)retValue) > precision)
            {
                retValue = Decimal.Round((Decimal)retValue, precision);//, MidpointRounding.AwayFromZero);
            }
            return (Decimal)retValue;
        }

        /// <summary>
        /// Convert Qty/Amt from entered UOM TO product UoM and round.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_UOM_To_ID">entered UOM</param>
        /// <param name="qtyPrice">entered quantity or price</param>
        /// <returns>Product: Qty/Amt in product UoM (precision rounded)</returns>
        public static Decimal? ConvertProductTo(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID, Decimal? qtyPrice)
        {
            if (qtyPrice == null || qtyPrice == 0 || VAM_Product_ID == 0 || VAB_UOM_To_ID == 0)
            {
                return qtyPrice;
            }

            Decimal? retValue = (Decimal?)(GetProductRateTo(ctx, VAM_Product_ID, VAB_UOM_To_ID));
            if (retValue != null)
            {
                if (Env.ONE.CompareTo(retValue) == 0)
                {
                    return qtyPrice;
                }
                MVABUOM uom = MVABUOM.Get(ctx, VAB_UOM_To_ID);
                if (uom != null)
                {
                    return uom.Round(Decimal.Multiply(Utility.Util.GetValueOfDecimal(retValue), Utility.Util.GetValueOfDecimal(qtyPrice)), true);
                }
                return Decimal.Multiply(Utility.Util.GetValueOfDecimal(retValue), Utility.Util.GetValueOfDecimal(qtyPrice));
            }
            return null;
        }

        /// <summary>
        /// Get Multiplier Rate from entered UOM TO product UoM
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_UOM_To_ID">entered UOM</param>
        /// <returns>multiplier or null</returns>
        public static Decimal? GetProductRateTo(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID)
        {
            if (VAM_Product_ID == 0)
            {
                return null;
            }
            MVABUOMConversion[] rates = GetProductConversions(ctx, VAM_Product_ID);
            if (rates.Length == 0)
            {
                _log.Fine("None found");
                return null;
            }

            for (int i = 0; i < rates.Length; i++)
            {
                MVABUOMConversion rate = rates[i];
                if (rate.GetVAB_UOM_To_ID() == VAB_UOM_To_ID)
                {
                    return rate.GetMultiplyRate();
                }
            }
            _log.Fine("None applied");
            return null;
        }

        /// <summary>
        /// Convert Qty/Amt FROM product UOM to entered UOM and round.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_UOM_To_ID">quantity or price</param>
        /// <param name="qtyPrice">quantity or price</param>
        /// <returns>Entered: Qty in entered UoM (precision rounded)</returns>
        //public static Decimal? ConvertProductFrom(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID, Decimal? qtyPrice,bool _ProductToConversion=false)
        public static Decimal? ConvertProductFrom(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID, Decimal? qtyPrice)
        {
            //	No conversion
            // Arpit to Pass a Parametrized Constructor so that we can have the reverse conversion rate for the defined product
            // bool ProductToConversion = _ProductToConversion;
            //  Arpit
            if (qtyPrice == null || qtyPrice.Equals(Env.ZERO) || VAB_UOM_To_ID == 0 || VAM_Product_ID == 0)
            {
                _log.Fine("No Conversion - QtyPrice=" + qtyPrice);
                return qtyPrice;
            }
            //Decimal? retValue = (Decimal?)GetProductRateFrom(ctx, VAM_Product_ID, VAB_UOM_To_ID, ProductToConversion);
            Decimal? retValue = (Decimal?)GetProductRateFrom(ctx, VAM_Product_ID, VAB_UOM_To_ID);
            if (retValue != null)
            {
                if (Env.ONE.CompareTo(retValue.Value) == 0)
                {
                    return qtyPrice;
                }
                MVABUOM uom = MVABUOM.Get(ctx, VAB_UOM_To_ID);
                if (uom != null)
                {
                    return uom.Round(Decimal.Multiply(retValue.Value, qtyPrice.Value), true);
                }
                //return retValue.multiply(qtyPrice);
                return Decimal.Multiply(retValue.Value, (Decimal)qtyPrice);
            }
            _log.Fine("No Rate VAM_Product_ID=" + VAM_Product_ID);
            return null;
        }

        /// <summary>
        /// Get Divide Rate FROM product UOM to entered UOM and round.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAB_UOM_To_ID"></param>
        /// <returns>divisor or null</returns>
        //public static Decimal? GetProductRateFrom(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID,bool _ProductToConversion=false)
        public static Decimal? GetProductRateFrom(Ctx ctx, int VAM_Product_ID, int VAB_UOM_To_ID)
        {
            MVABUOMConversion[] rates = GetProductConversions(ctx, VAM_Product_ID);
            if (rates.Length == 0)
            {
                _log.Fine("getProductRateFrom - none found");
                return null;
            }

            for (int i = 0; i < rates.Length; i++)
            {
                MVABUOMConversion rate = rates[i];
                if (rate.GetVAB_UOM_To_ID() == VAB_UOM_To_ID)
                {
                    return rate.GetDivideRate();
                }
                //Arpit  -- to Pass a Parametrized Constructor so that we can have the reverse conversion rate for the defined product
                //if (_ProductToConversion)
                //{
                //    return rate.GetMultiplyRate();
                //}
                //Arpit
            }
            _log.Fine("None applied");
            return null;
        }


        /// <summary>
        /// Get Product Conversions (cached)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <returns>array of conversions</returns>
        public static MVABUOMConversion[] GetProductConversions(Ctx ctx, int VAM_Product_ID)
        {
            if (VAM_Product_ID == 0)
            {
                return new MVABUOMConversion[0];
            }
            int key = VAM_Product_ID;
            MVABUOMConversion[] result = (MVABUOMConversion[])_conversionProduct[key];
            if (result != null)
            {
                return result;
            }

            List<MVABUOMConversion> list = new List<MVABUOMConversion>();

            //
            String sql = "SELECT * FROM VAB_UOM_Conversion c "
                + "WHERE c.VAM_Product_ID=" + VAM_Product_ID
                + " AND EXISTS (SELECT * FROM VAM_Product p "
                    + "WHERE c.VAM_Product_ID=p.VAM_Product_ID AND c.VAB_UOM_ID=p.VAB_UOM_ID)"
                + " AND c.IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            DataTable dt1 = null;
            IDataReader idr1 = null;
            try
            {

                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABUOMConversion(ctx, dr, null));
                }

                if (list.Count == 0)
                {
                    sql = "SELECT * FROM VAB_UOM_Conversion c "
                + "WHERE EXISTS (SELECT * FROM VAM_Product p "
                    + "WHERE c.VAB_UOM_ID=p.VAB_UOM_ID AND p.VAM_Product_ID=" + VAM_Product_ID + ")"
                + " AND c.IsActive='Y'";

                    idr1 = DB.ExecuteReader(sql, null, null);
                    dt1 = new DataTable();
                    dt1.Load(idr1);
                    idr1.Close();
                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        list.Add(new MVABUOMConversion(ctx, dr1, null));
                    }
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (idr1 != null)
                {
                    idr1.Close();
                    idr1 = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (idr1 != null)
                {
                    idr1.Close();
                    idr1 = null;
                }
                dt = null;
                dt1 = null;
            }
            //	Add default conversion
            if (list.Count == 0)
            {
                MVABUOMConversion defRate = new MVABUOMConversion(MVAMProduct.Get(ctx, VAM_Product_ID));
                list.Add(defRate);
            }

            //	Convert & save
            result = new MVABUOMConversion[list.Count];
            result = list.ToArray();
            _conversionProduct.Add(key, result);
            _log.Fine("GetProductConversions - VAM_Product_ID=" + VAM_Product_ID + " #" + result.Length);
            return result;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_Conversion_ID"></param>
        /// <param name="trxName"></param>
        public MVABUOMConversion(Ctx ctx, int VAB_UOM_Conversion_ID, Trx trxName)
            : base(ctx, VAB_UOM_Conversion_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABUOMConversion(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// 	Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        public MVABUOMConversion(MVABUOM parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAB_UOM_ID(parent.GetVAB_UOM_ID());
            SetVAM_Product_ID(0);
            SetVAB_UOM_To_ID(parent.GetVAB_UOM_ID());
            SetMultiplyRate(Env.ONE);
            SetDivideRate(Env.ONE);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        public MVABUOMConversion(MVAMProduct parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAB_UOM_ID(parent.GetVAB_UOM_ID());
            SetVAM_Product_ID(parent.GetVAM_Product_ID());
            SetVAB_UOM_To_ID(parent.GetVAB_UOM_ID());
            SetMultiplyRate(Env.ONE);
            SetDivideRate(Env.ONE);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	From - To is the same
            //Commented by arpit asked by sachin Sir 29 Jan,2018--to save records with same UOM also
            //if (GetVAB_UOM_ID() == GetVAB_UOM_To_ID())
            //{
            //    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@VAB_UOM_ID@ = @VAB_UOM_ID@"));
            //    return false;
            //}
            //	Nothing to convert
            if (GetMultiplyRate().CompareTo(Env.ZERO) <= 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "ProductUOMConversionRateError"));
                return false;
            }
            //	Enforce Product UOM
            if (GetVAM_Product_ID() != 0 && (newRecord || Is_ValueChanged("VAM_Product_ID")))
            {
                MVAMProduct product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());
                if (product.GetVAB_UOM_ID() != GetVAB_UOM_ID())
                {
                    MVABUOM uom = MVABUOM.Get(GetCtx(), product.GetVAB_UOM_ID());
                    log.SaveError("ProductUOMConversionUOMError", uom.GetName());
                    return false;
                }
            }

            //	The Product UoM needs to be the smallest UoM - Multiplier  must be > 0
            if (GetVAM_Product_ID() != 0 && GetDivideRate().CompareTo(Env.ONE) < 0)
            {
                // JID_0239: Currenly system show message when multiple rate is less than one like below "Product UOM Coversion rate error"
                log.SaveError("", Msg.GetMsg(GetCtx(), "ProductUOMConversionRateError"));
                return false;
            }
            if (!String.IsNullOrEmpty(GetUPC()) &&
                     Util.GetValueOfString(Get_ValueOld("UPC")) != GetUPC())
            {
                //string sql = "SELECT UPCUNIQUE('c','" + GetUPC() + "') as productID FROM Dual";
                //int manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                //if (manu_ID != 0 && manu_ID != GetVAM_Product_ID())

                int manu_ID = MVAMProduct.UpcUniqueClientWise(GetVAF_Client_ID(), GetUPC());
                if (manu_ID > 0)
                {
                    _log.SaveError("UPCUnique", "");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MUOMConversion[");
            sb.Append(Get_ID()).Append("-VAB_UOM_ID=").Append(GetVAB_UOM_ID())
                .Append(",VAB_UOM_To_ID=").Append(GetVAB_UOM_To_ID())
                .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append("-Multiply=").Append(GetMultiplyRate())
                .Append("/Divide=").Append(GetDivideRate())
                .Append("]");
            return sb.ToString();
        }

    }
}
