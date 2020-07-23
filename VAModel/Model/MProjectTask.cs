/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_ProjectTask
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
    /// Project Phase Task Model
    /// </summary>
    public class MProjectTask : X_C_ProjectTask
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ProjectTask_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectTask(Ctx ctx, int C_ProjectTask_ID, Trx trxName)
            : base(ctx, C_ProjectTask_ID, trxName)
        {
            if (C_ProjectTask_ID == 0)
            {
                //	setC_ProjectTask_ID (0);	//	PK
                //	setC_ProjectPhase_ID (0);	//	Parent
                //	setC_Task_ID (0);			//	FK
                SetSeqNo(0);
                //	setName (null);
                SetQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MProjectTask(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="phase">parent</param>
        public MProjectTask(MProjectPhase phase)
            : this(phase.GetCtx(), 0, phase.Get_TrxName())
        {
            SetClientOrg(phase);
            SetC_ProjectPhase_ID(phase.GetC_ProjectPhase_ID());
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="phase">parent</param>
        /// <param name="task">type copy</param>
        public MProjectTask(MProjectPhase phase, MProjectTypeTask task)
            : this(phase)
        {
            SetC_Task_ID(task.GetC_Task_ID());			//	FK
            SetSeqNo(task.GetSeqNo());
            SetName(task.GetName());
            SetDescription(task.GetDescription());
            SetHelp(task.GetHelp());
            if (task.GetM_Product_ID() != 0)
                SetM_Product_ID(task.GetM_Product_ID());
            SetQty(task.GetStandardQty());
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProjectTask[");
            sb.Append(Get_ID())
                .Append("-").Append(GetSeqNo())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            UpdateHeader();
            return success;
        }

        private void UpdateHeader()
        {
            int projID = 0;
            string isCam = "";
            string isOpp = "";
            string Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_ID in(select C_ProjectPhase_ID FROM" +
                    " C_ProjectTask WHERE C_ProjectTask_ID =" + GetC_ProjectTask_ID() + ")";
            projID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            
            if (projID != 0)
            {
                isOpp = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsOpportunity FROM C_Project WHERE C_Project_ID = " + projID));
                isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM C_Project WHERE C_Project_ID = " + projID));
            }
            if (isCam.Equals("Y"))                             // Campaign Window
            {
                MProject prj = new MProject(GetCtx(), projID, null);
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(PlannedAmt),0)  FROM C_ProjectTask WHERE IsActive = 'Y' AND " +
                    "C_ProjectPhase_ID in (SELECT C_ProjectPhase_ID FROM C_ProjectPhase WHERE C_Project_ID = " + projID + ")"));
                prj.SetPlannedAmt(plnAmt);
                prj.Save();
            }
            //Amit
            else if (isOpp.Equals("N") && isCam.Equals("N"))
            {
                // set sum of total amount of task tab to phase tab, similalary Commitment amount
                MProjectPhase phase = new MProjectPhase(GetCtx(), GetC_ProjectPhase_ID(), null);
                phase.SetPlannedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectTask pl WHERE pl.IsActive = 'Y' AND pl.C_ProjectPhase_ID = " + GetC_ProjectPhase_ID())));
                phase.SetCommittedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.CommittedAmt),0)  FROM C_ProjectTask pl WHERE pl.IsActive = 'Y' AND pl.C_ProjectPhase_ID = " + GetC_ProjectPhase_ID())));
                if (!phase.Save())
                {

                }
            }
            //Amit
        }

    }
}
