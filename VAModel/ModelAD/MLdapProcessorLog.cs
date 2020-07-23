/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLdapProcessorLog
 * Purpose        : MLdap ProcessorLog Model
 * Class Used     : X_AD_LdapProcessorLog, ViennaProcessorLog
 * Chronological    Development
 * Deepak           03-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MLdapProcessorLog : X_AD_LdapProcessorLog, ViennaProcessorLog
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_LdapProcessorLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MLdapProcessorLog(Ctx ctx, int AD_LdapProcessorLog_ID,
            Trx trxName)
            : base(ctx, AD_LdapProcessorLog_ID, trxName)
        {

            if (AD_LdapProcessorLog_ID == 0)
            {
                //	setAD_LdapProcessorLog_ID (0);
                //	setAD_LdapProcessor_ID (0);
                SetIsError(false);
            }
        }	//	MLdapProcessorLog

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MLdapProcessorLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MLdapProcessorLog
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MLdapProcessorLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="summary">summary</param>
        public MLdapProcessorLog(MLdapProcessor parent, String summary)
            : this(parent.GetCtx(), 0, null)
        {

            SetClientOrg(parent);
            SetAD_LdapProcessor_ID(parent.GetAD_LdapProcessor_ID());
            SetSummary(summary);
        }	//	MLdapProcessorLog

    }	//	MLdapProcessorLog

}
