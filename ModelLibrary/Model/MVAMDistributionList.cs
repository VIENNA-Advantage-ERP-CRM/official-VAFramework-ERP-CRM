/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMDistributionList
 * Purpose        : Material Distribution List
 * Class Used     : X_VAM_DistributionList
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
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMDistributionList : X_VAM_DistributionList
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_DistributionList_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMDistributionList(Ctx ctx, int VAM_DistributionList_ID, Trx trxName)
            : base(ctx, VAM_DistributionList_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">Records Row</param>
        /// <param name="trxName">transaction</param>
        public MVAMDistributionList(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Distibution Lines.
        /// Add/Correct also Total Ratio
        /// </summary>
        /// <returns>array of lines</returns>
        public MVAMDistributionListLine[] GetLines()
        {
            List<MVAMDistributionListLine> list = new List<MVAMDistributionListLine>();
            Decimal ratioTotal = Env.ZERO;
            //
            String sql = "SELECT * FROM VAM_DistributionListLine WHERE VAM_DistributionList_ID=" + GetVAM_DistributionList_ID();
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
                    MVAMDistributionListLine line = new MVAMDistributionListLine(GetCtx(), dr, Get_TrxName());
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

            MVAMDistributionListLine[] retValue = new MVAMDistributionListLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
