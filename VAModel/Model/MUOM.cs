/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MUOM
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MUOM : X_C_UOM
    {
        #region Private Variable
        //	UOM Cache				
        private static CCache<int, MUOM> s_cache = new CCache<int, MUOM>("C_UOM", 30);
        // X12 Element 355 Code	Minute	
        static String X12_MINUTE = "MJ";
        // X12 Element 355 Code	Hour	
        static String X12_HOUR = "HR";
        // X12 Element 355 Code	Day 	
        static String X12_DAY = "DA";
        // X12 Element 355 Code	Work Day (8 hours / 5days)	 	
        static String X12_DAY_WORK = "WD";
        // X12 Element 355 Code	Week 	
        static String X12_WEEK = "WK";
        // X12 Element 355 Code	Month 	
        static String X12_MONTH = "MO";
        // X12 Element 355 Code	Work Month (20 days / 4 weeks) 	
        static String X12_MONTH_WORK = "WM";
        // X12 Element 355 Code	Year 	
        static String X12_YEAR = "YR";

        //Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MUOM).FullName);
        #endregion

        /// <summary>
        /// Get Minute C_UOM_ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>C_UOM_ID for Minute</returns>
        public static int GetMinute_UOM_ID(Ctx ctx)
        {
            if (Ini.IsClient())
            {
                //Iterator it = s_cache.values().iterator();
                IEnumerator it = s_cache.Values.GetEnumerator();
                while (it.MoveNext())
                {
                    MUOM uom = (MUOM)it.Current;
                    if (uom.IsMinute())
                    {
                        return uom.GetC_UOM_ID();
                    }
                }
            }
            //	Server
            int C_UOM_ID = 0;
            String sql = "SELECT C_UOM_ID FROM C_UOM WHERE IsActive='Y' AND X12DE355='MJ'";	//	HardCoded
            //DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    C_UOM_ID = Utility.Util.GetValueOfInt(idr[0]);
                }
                idr.Close();
                idr = null;
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
            return C_UOM_ID;
        }

        /// <summary>
        /// Get Default C_UOM_ID
        /// </summary>
        /// <param name="ctx">context for AD_Client</param>
        /// <returns>C_UOM_ID</returns>
        public static int GetDefault_UOM_ID(Ctx ctx)
        {
            String sql = "SELECT C_UOM_ID "
                + "FROM C_UOM "
                + "WHERE AD_Client_ID IN (0," + ctx.GetAD_Client_ID() + ") "
                + "ORDER BY IsDefault DESC, AD_Client_ID DESC, C_UOM_ID";

            return Convert.ToInt32(DataBase.DB.ExecuteScalar(sql, null, null));
        }

        /// <summary>
        /// Get UOM from Cache
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_UOM_ID"></param>
        /// <returns>UOM</returns>
        public static MUOM Get(Ctx ctx, int C_UOM_ID)
        {
            if (s_cache.Count == 0)
            {
                LoadUOMs(ctx);
            }
            //
            int ii = C_UOM_ID;
            MUOM uom = (MUOM)s_cache[ii];
            if (uom != null)
            {
                return uom;
            }
            //
            uom = new MUOM(ctx, C_UOM_ID, null);
            s_cache.Add(Utility.Util.GetValueOfInt(C_UOM_ID), uom);
            return uom;
        }

        /// <summary>
        /// Get Precision
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_UOM_ID"></param>
        /// <returns>Precision</returns>
        public static int GetPrecision(Ctx ctx, int C_UOM_ID)
        {
            MUOM uom = Get(ctx, C_UOM_ID);
            return uom.GetStdPrecision();
        }

        /// <summary>
        /// Load All UOMs
        /// </summary>
        /// <param name="ctx"></param>
        private static void LoadUOMs(Ctx ctx)
        {
            String sql = MRole.GetDefault(ctx, false).AddAccessSQL(
                "SELECT * FROM C_UOM "
                + "WHERE IsActive='Y'",
                "C_UOM", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MUOM uom = new MUOM(ctx, dr, null);
                    s_cache.Add(uom.GetC_UOM_ID(), uom);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_UOM_ID"></param>
        /// <param name="trxName"></param>
        public MUOM(Ctx ctx, int C_UOM_ID, Trx trxName)
            : base(ctx, C_UOM_ID, trxName)
        {

            if (C_UOM_ID == 0)
            {
                SetIsDefault(false);
                SetStdPrecision(2);
                SetCostingPrecision(6);
            }
        }

        /// <summary>
        /// Load Constructor.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MUOM(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("UOM[");
            sb.Append("ID=").Append(Get_ID())
                .Append(", Name=").Append(GetName());
            return sb.ToString();
        }


        /// <summary>
        /// Round qty
        /// </summary>
        /// <param name="qty">quantity</param>
        /// <param name="stdPrecision">stdPrecision true if std precisison</param>
        /// <returns>rounded quantity</returns>
        public Decimal Round(Decimal qty, bool stdPrecision)
        {
            int precision = GetStdPrecision();
            if (!stdPrecision)
                precision = GetCostingPrecision();
            if (Env.Scale(qty) > precision)
            {
                //return qty.setScale(GetStdPrecision(), Decimal.ROUND_HALF_UP);
                return Decimal.Round(qty, GetStdPrecision());//, MidpointRounding.AwayFromZero);
            }
            return qty;
        }

        /// <summary>
        /// Minute
        /// </summary>
        /// <returns>true if UOM is minute</returns>
        public bool IsMinute()
        {
            return X12_MINUTE.Equals(GetX12DE355());
        }

        /// <summary>
        /// Hour
        /// </summary>
        /// <returns>true if UOM is hour</returns>
        public bool IsHour()
        {
            return X12_HOUR.Equals(GetX12DE355());
        }

        /// <summary>
        /// Day
        /// </summary>
        /// <returns>true if UOM is Day</returns>
        public bool IsDay()
        {
            return X12_DAY.Equals(GetX12DE355());
        }

        /// <summary>
        /// WorkDay
        /// </summary>
        /// <returns>true if UOM is work day</returns>
        public bool IsWorkDay()
        {
            return X12_DAY_WORK.Equals(GetX12DE355());
        }

        /// <summary>
        /// Week
        /// </summary>
        /// <returns>true if UOM is Week</returns>
        public bool IsWeek()
        {
            return X12_WEEK.Equals(GetX12DE355());
        }

        /// <summary>
        /// Month
        /// </summary>
        /// <returns>true if UOM is Month</returns>
        public bool IsMonth()
        {
            return X12_MONTH.Equals(GetX12DE355());
        }

        /// <summary>
        /// WorkMonth
        /// </summary>
        /// <returns>true if UOM is Work Month</returns>
        public bool IsWorkMonth()
        {
            return X12_MONTH_WORK.Equals(GetX12DE355());
        }

        /// <summary>
        /// 	Year
        /// </summary>
        /// <returns>true if UOM is year</returns>
        public bool IsYear()
        {
            return X12_YEAR.Equals(GetX12DE355());
        }
    }
}
