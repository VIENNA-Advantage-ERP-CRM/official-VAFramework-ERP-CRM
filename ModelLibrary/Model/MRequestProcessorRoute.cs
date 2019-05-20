/******************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRequestProcessorRoute
 * Purpose        : Request Processor Route
 * Class Used     : X_R_RequestProcessor_Route
 * Chronological    Development
 * Raghunandan      21-Jan-2010
  *****************************************************/
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
using System.Threading;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MRequestProcessorRoute : X_R_RequestProcessor_Route
    {
        /// <summary>
        /// Standard Constructorbase
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="R_RequestProcessor_Route_ID"></param>
        /// <param name="trxName"></param>
        public MRequestProcessorRoute(Ctx ctx, int R_RequestProcessor_Route_ID, Trx trxName)
            : base(ctx, R_RequestProcessor_Route_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MRequestProcessorRoute(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
    }
}
