/******************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVARReqHandlerRoute
 * Purpose        : Request Processor Route
 * Class Used     : X_VAR_Req_Handler_Route
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
//////using System.Windows.Forms;
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
    public class MVARReqHandlerRoute : X_VAR_Req_Handler_Route
    {
        /// <summary>
        /// Standard Constructorbase
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAR_Req_Handler_Route_ID"></param>
        /// <param name="trxName"></param>
        public MVARReqHandlerRoute(Ctx ctx, int VAR_Req_Handler_Route_ID, Trx trxName)
            : base(ctx, VAR_Req_Handler_Route_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVARReqHandlerRoute(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
    }
}
