using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MAlertProcessorLog : X_AD_AlertProcessorLog, ViennaProcessorLog
    {
        public MAlertProcessorLog(Ctx ctx, int AD_AlertProcessorLog_ID, Trx trx)
            : base(ctx, AD_AlertProcessorLog_ID, trx)
        {
            
        }	//	MAlertProcessorLog


        public MAlertProcessorLog(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
            
        }	//	MAlertProcessorLog

        public MAlertProcessorLog(MAlertProcessor parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_AlertProcessor_ID(parent.GetAD_AlertProcessor_ID());
            SetSummary(summary);
        }	//	MAlertProcessorLog

    }
}
