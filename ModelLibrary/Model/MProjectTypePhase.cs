/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Phase
 * Chronological Development
 * Veena Pandey     17-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Project Type Phase Model
    /// </summary>
    public class MProjectTypePhase : X_C_Phase
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Phase_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectTypePhase(Ctx ctx, int C_Phase_ID, Trx trxName)
            : base(ctx, C_Phase_ID, trxName)
        {
            if (C_Phase_ID == 0)
            {
                //	setC_Phase_ID (0);			//	PK
                //	setC_ProjectType_ID (0);	//	Parent
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
        public MProjectTypePhase(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Project Type Phases
        /// </summary>
        /// <returns>Array of phases</returns>
        public MProjectTypeTask[] GetTasks()
        {
            List<MProjectTypeTask> list = new List<MProjectTypeTask>();
            String sql = "SELECT * FROM C_Task WHERE C_Phase_ID=" + GetC_Phase_ID() + " ORDER BY SeqNo";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MProjectTypeTask(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            MProjectTypeTask[] retValue = new MProjectTypeTask[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
