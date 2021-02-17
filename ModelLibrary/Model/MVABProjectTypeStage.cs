/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_Std_Stage
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
    public class MVABProjectTypeStage : X_VAB_Std_Stage
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Std_Stage_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABProjectTypeStage(Ctx ctx, int VAB_Std_Stage_ID, Trx trxName)
            : base(ctx, VAB_Std_Stage_ID, trxName)
        {
            if (VAB_Std_Stage_ID == 0)
            {
                //	setVAB_Std_Stage_ID (0);			//	PK
                //	setVAB_ProjectType_ID (0);	//	Parent
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
        public MVABProjectTypeStage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Project Type Phases
        /// </summary>
        /// <returns>Array of phases</returns>
        public MVABProjectTypeTask[] GetTasks()
        {
            List<MVABProjectTypeTask> list = new List<MVABProjectTypeTask>();
            String sql = "SELECT * FROM VAB_Std_Task WHERE VAB_Std_Stage_ID=" + GetVAB_Std_Stage_ID() + " ORDER BY SeqNo";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVABProjectTypeTask(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            MVABProjectTypeTask[] retValue = new MVABProjectTypeTask[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
