/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionRunDetail
 * Purpose        : Distribution Run Detail
 * Class Used     : X_M_DistributionListLine
 * Chronological    Development
 * Raghunandan     04-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDistributionRunDetail : X_T_DistributionRunDetail
    {
        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MDistributionRunDetail).FullName);
        //Precision		
        private int _precision = 0;

        /// <summary>
        /// Get Distribution Dun details
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DistributionRun_ID">id</param>
        /// <param name="orderBP">if true ordered by Business Partner otherwise Run Line</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of details</returns>
        public static MDistributionRunDetail[] Get(Ctx ctx, int M_DistributionRun_ID,
                bool orderBP, Trx trxName)
        {
            List<MDistributionRunDetail> list = new List<MDistributionRunDetail>();
            String sql = "SELECT * FROM T_DistributionRunDetail WHERE M_DistributionRun_ID=" + M_DistributionRun_ID;
            if (orderBP)
            {
                sql += " ORDER BY C_BPartner_ID, C_BPartner_Location_ID";
            }
            else
            {
                sql += " ORDER BY M_DistributionRunLine_ID";
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                //idr = DataBase.DB.ExecuteReader(sql);//, null, trxName);
                //while(idr.Read())
                //{
                //}
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MDistributionRunDetail(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MDistributionRunDetail[] retValue = new MDistributionRunDetail[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="T_DistributionRunDetail_ID">id</param>
        /// <param name="trxName">trx</param>
        public MDistributionRunDetail(Ctx ctx, int T_DistributionRunDetail_ID, Trx trxName)
            : base(ctx, T_DistributionRunDetail_ID, trxName)
        {

        }

        /// <summary>
        /// 	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDistributionRunDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }



        /// <summary>
        /// Round MinQty & Qty
        /// </summary>
        /// <param name="precision">precision (saved)</param>
        public void Round(int precision)
        {
            bool dirty = false;
            _precision = precision;
            Decimal min = GetMinQty();
            if (Env.Scale(min) > _precision)
            {
                SetMinQty(Decimal.Round(min, _precision, MidpointRounding.AwayFromZero));
                dirty = true;
            }
            Decimal qty = GetQty();
            if (Env.Scale(qty)> _precision)
            {
                SetQty(Decimal.Round(qty,_precision, MidpointRounding.AwayFromZero));
                dirty = true;
            }
            if (dirty)
            {
                Save();
            }
        }

        /// <summary>
        /// We can adjust Allocation Qty
        /// </summary>
        /// <returns>true if qty > min</returns>
        public bool IsCanAdjust()
        {
            return (GetQty().CompareTo(GetMinQty()) > 0);
        }

        /// <summary>
        /// Get Actual Allocation Qty
        /// </summary>
        /// <returns>the greater of the min/qty</returns>
        public Decimal GetActualAllocation()
        {
            if (GetQty().CompareTo(GetMinQty()) > 0)
            {
                return GetQty();
            }
            else
            {
                return GetMinQty();
            }
        }

        /// <summary>
        /// Adjust the Quantity maintaining UOM precision
        /// </summary>
        /// <param name="difference">difference</param>
        /// <returns>remaining difference (because under Min or rounding)</returns>
        public Decimal AdjustQty(Decimal difference)
        {
            Decimal diff = Decimal.Round(difference, _precision, MidpointRounding.AwayFromZero);
            Decimal qty = GetQty();
            Decimal max = Decimal.Subtract(GetMinQty(), qty);
            Decimal remaining = Env.ZERO;
            if (max.CompareTo(diff) > 0)	//	diff+max are negative
            {
                remaining = Decimal.Subtract(diff, max);
                SetQty(Decimal.Add(qty, max));
            }
            else
            {
                SetQty(Decimal.Add(qty, diff));
            }
            log.Fine("Qty=" + qty + ", Min=" + GetMinQty()
                + ", Max=" + max + ", Diff=" + diff + ", newQty=" + GetQty()
                + ", Remaining=" + remaining);
            return remaining;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDistributionRunDetail[")
                .Append(Get_ID())
                .Append(";M_DistributionListLine_ID=").Append(GetM_DistributionListLine_ID())
                .Append(";Qty=").Append(GetQty())
                .Append(";Ratio=").Append(GetRatio())
                .Append(";MinQty=").Append(GetMinQty())
                .Append("]");
            return sb.ToString();
        }

    }
}
