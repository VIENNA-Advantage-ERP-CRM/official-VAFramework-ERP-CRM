/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Task
 * Chronological Development
 * Veena Pandey     17-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Project Type Phase Task Model
    /// </summary>
    public class MProjectTypeTask : X_C_Task
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Task_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectTypeTask(Ctx ctx, int C_Task_ID, Trx trxName)
            : base (ctx, C_Task_ID, trxName)
        {
            
            if (C_Task_ID == 0)
            {
                //	setC_Task_ID (0);		//	PK
                //	setC_Phase_ID (0);		//	Parent
                //	setName (null);
                SetSeqNo(0);
                SetStandardQty(Utility.Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MProjectTypeTask(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
