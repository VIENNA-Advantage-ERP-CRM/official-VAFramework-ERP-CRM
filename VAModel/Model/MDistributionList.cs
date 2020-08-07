/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionList
 * Purpose        : Material Distribution List
 * Class Used     : X_M_DistributionList
 * Chronological    Development
 * Raghunandan     03-Nov-2009
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
    public class MDistributionList : X_M_DistributionList
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DistributionList_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDistributionList(Ctx ctx, int M_DistributionList_ID, Trx trxName)
            : base(ctx, M_DistributionList_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">Records Row</param>
        /// <param name="trxName">transaction</param>
        public MDistributionList(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Distibution Lines.
        /// Add/Correct also Total Ratio
        /// </summary>
        /// <returns>array of lines</returns>
        public MDistributionListLine[] GetLines()
        {
            List<MDistributionListLine> list = new List<MDistributionListLine>();
            Decimal ratioTotal = Env.ZERO;
            //
            String sql = "SELECT * FROM M_DistributionListLine WHERE M_DistributionList_ID=" + GetM_DistributionList_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MDistributionListLine line = new MDistributionListLine(GetCtx(), dr, Get_TrxName());
                    list.Add(line);
                    Decimal ratio = line.GetRatio();
                    if (ratio != 0)
                    {
                        ratioTotal = Decimal.Add(ratioTotal, ratio);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "getLines", e);
            }
            finally
            {
                dt = null;
            }

            //	Update Ratio
            if (ratioTotal.CompareTo(GetRatioTotal()) != 0)
            {
                log.Info("getLines - Set RatioTotal from " + GetRatioTotal() + " to " + ratioTotal);
                SetRatioTotal(ratioTotal);
                Save();
            }

            MDistributionListLine[] retValue = new MDistributionListLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
