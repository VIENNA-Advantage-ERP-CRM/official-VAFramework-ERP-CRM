/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLdapProcessorLog
 * Purpose        : MLdap ProcessorLog Model
 * Class Used     : X_VAF_LdapHandlerLog, ViennaProcessorLog
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
    public class MVAFLdapHandlerLog : X_VAF_LdapHandlerLog, ViennaProcessorLog
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_LdapHandlerLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapHandlerLog(Ctx ctx, int VAF_LdapHandlerLog_ID,
            Trx trxName)
            : base(ctx, VAF_LdapHandlerLog_ID, trxName)
        {

            if (VAF_LdapHandlerLog_ID == 0)
            {
                //	setVAF_LdapHandlerLog_ID (0);
                //	setVAF_LdapHandler_ID (0);
                SetIsError(false);
            }
        }	//	MLdapProcessorLog

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapHandlerLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MLdapProcessorLog
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapHandlerLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="summary">summary</param>
        public MVAFLdapHandlerLog(MVAFLdapHandler parent, String summary)
            : this(parent.GetCtx(), 0, null)
        {

            SetClientOrg(parent);
            SetVAF_LdapHandler_ID(parent.GetVAF_LdapHandler_ID());
            SetSummary(summary);
        }	//	MLdapProcessorLog

    }	//	MLdapProcessorLog

}
