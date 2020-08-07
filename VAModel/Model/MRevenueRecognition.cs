/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRevenueRecognition
 * Purpose        : Revenue Recognition Model
 * Class Used     : X_C_RevenueRecognition
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Model
{
    /// <summary>
    /// Revenue Recognition Model
    /// </summary>
    public class MRevenueRecognition : X_C_RevenueRecognition
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RevenueRecognition_ID"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognition(Ctx ctx, int C_RevenueRecognition_ID, Trx trxName)
            : base(ctx, C_RevenueRecognition_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognition(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

    }
}
