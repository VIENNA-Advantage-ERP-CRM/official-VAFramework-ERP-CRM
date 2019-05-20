/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssignLog
 * Purpose        : Assignment Log Model
 * Class Used     : X_AD_AssignLog
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
    public class MAssignLog : X_AD_AssignLog
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_AssignLog_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAssignLog(Ctx ctx, int AD_AssignLog_ID, Trx trxName)
            : base(ctx, AD_AssignLog_ID, trxName)
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
            SetAD_AssignTarget_ID(parent.GetAD_AssignTarget_ID());
        }	//	MAssignLog

    }
}
