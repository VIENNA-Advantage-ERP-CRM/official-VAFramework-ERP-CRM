/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssignLog
 * Purpose        : Assignment Log Model
 * Class Used     : X_VAF_AllotLog
 * Chronological    Development
 * Deepak           12-Feb-2010
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
    public class MAssignLog : X_VAF_AllotLog
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_AllotLog_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAssignLog(Ctx ctx, int VAF_AllotLog_ID, Trx trxName)
            : base(ctx, VAF_AllotLog_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MAssignLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MAssignLog(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MAssignLog(MAssignTarget parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetVAF_AllotTarget_ID(parent.GetVAF_AllotTarget_ID());
        }	//	MAssignLog

    }
}
