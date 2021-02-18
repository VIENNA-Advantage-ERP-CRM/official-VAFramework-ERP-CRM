/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_YearPeriodControl
 * Chronological Development
 * Veena Pandey     07-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVABYearPeriodControl : X_VAB_YearPeriodControl
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_YearPeriodControl_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABYearPeriodControl(Ctx ctx, int VAB_YearPeriodControl_ID, Trx trxName)
            : base(ctx, VAB_YearPeriodControl_ID, trxName)
        {
            if (VAB_YearPeriodControl_ID == 0)
            {
                //	setVAB_YearPeriod_ID (0);
                //	setDocBaseType (null);
                SetPeriodAction(PERIODACTION_NoAction);
                SetPeriodStatus(PERIODSTATUS_NeverOpened);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABYearPeriodControl(Ctx ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="period">parent</param>
        /// <param name="docBaseType">doc base type</param>
        public MVABYearPeriodControl(MVABYearPeriod period, String docBaseType)
            : this(period.GetCtx(), period.GetVAF_Client_ID(), period.GetVAB_YearPeriod_ID(), docBaseType, 
                period.Get_TrxName())
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client id</param>
        /// <param name="VAB_YearPeriod_ID">period id</param>
        /// <param name="docBaseType">doc base type</param>
        /// <param name="trxName">transaction</param>
        public MVABYearPeriodControl(Ctx ctx, int VAF_Client_ID, int VAB_YearPeriod_ID, String docBaseType, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetClientOrg(VAF_Client_ID, 0);
            SetVAB_YearPeriod_ID(VAB_YearPeriod_ID);
            SetDocBaseType(docBaseType);
        }

        /// <summary>
        /// Is Period Open
        /// </summary>
        /// <returns>true if open</returns>
        public bool IsOpen()
        {
            return PERIODSTATUS_Open.Equals(GetPeriodStatus());
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVABYearPeriodControl[");
            sb.Append(Get_ID()).Append(",").Append(GetDocBaseType())
                .Append(",Status=").Append(GetPeriodStatus())
                .Append("]");
            return sb.ToString();
        }
    }
}
