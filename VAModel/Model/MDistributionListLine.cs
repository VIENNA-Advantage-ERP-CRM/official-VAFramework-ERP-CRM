/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionListLine
 * Purpose        :Distribution List Line
 * Class Used     : X_M_DistributionListLine
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
    public class MDistributionListLine : X_M_DistributionListLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DistributionListLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDistributionListLine(Ctx ctx, int M_DistributionListLine_ID, Trx trxName)
            : base(ctx, M_DistributionListLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName">transaction</param>
        public MDistributionListLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Min Qty
        /// </summary>
        /// <returns>min Qty or 0</returns>
        public new Decimal GetMinQty()
        {
            Decimal minQty = base.GetMinQty();
            
            return minQty;
        }

        /// <summary>
        /// Get Ratio
        /// </summary>
        /// <returns>ratio or 0</returns>
        public new Decimal GetRatio()
        {
            Decimal ratio = base.GetRatio();
            //if (ratio == null)
            //{
            //    return Env.ZERO;
            //}
            return ratio;
        }
    }
}
