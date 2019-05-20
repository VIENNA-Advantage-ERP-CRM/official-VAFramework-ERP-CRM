/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PeriodControl
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
    public class MPeriodControl : X_C_PeriodControl
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_PeriodControl_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPeriodControl(Ctx ctx, int C_PeriodControl_ID, Trx trxName)
            : base(ctx, C_PeriodControl_ID, trxName)
        {
            if (C_PeriodControl_ID == 0)
            {
                //	setC_Period_ID (0);
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
        public MPeriodControl(Ctx ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="period">parent</param>
        /// <param name="docBaseType">doc base type</param>
        public MPeriodControl(MPeriod period, String docBaseType)
            : this(period.GetCtx(), period.GetAD_Client_ID(), period.GetC_Period_ID(), docBaseType, 
                period.Get_TrxName())
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="C_Period_ID">period id</param>
        /// <param name="docBaseType">doc base type</param>
        /// <param name="trxName">transaction</param>
        public MPeriodControl(Ctx ctx, int AD_Client_ID, int C_Period_ID, String docBaseType, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetClientOrg(AD_Client_ID, 0);
            SetC_Period_ID(C_Period_ID);
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
            StringBuilder sb = new StringBuilder("MPeriodControl[");
            sb.Append(Get_ID()).Append(",").Append(GetDocBaseType())
                .Append(",Status=").Append(GetPeriodStatus())
                .Append("]");
            return sb.ToString();
        }
    }
}
