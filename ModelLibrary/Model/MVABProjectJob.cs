/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_ProjectJob
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
    public class MVABProjectJob : X_VAB_ProjectJob
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_ProjectJob_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABProjectJob(Ctx ctx, int VAB_ProjectJob_ID, Trx trxName)
            : base(ctx, VAB_ProjectJob_ID, trxName)
        {
            if (VAB_ProjectJob_ID == 0)
            {
                //	setVAB_ProjectJob_ID (0);	//	PK
                //	setVAB_ProjectStage_ID (0);	//	Parent
                //	setVAB_Std_Task_ID (0);			//	FK
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
        public MVABProjectJob(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="phase">parent</param>
        public MVABProjectJob(MVABProjectStage phase)
            : this(phase.GetCtx(), 0, phase.Get_TrxName())
        {
            SetClientOrg(phase);
            SetVAB_ProjectStage_ID(phase.GetVAB_ProjectStage_ID());
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="phase">parent</param>
        /// <param name="task">type copy</param>
        public MVABProjectJob(MVABProjectStage phase, MVABProjectTypeTask task)
            : this(phase)
        {
            SetVAB_Std_Task_ID(task.GetVAB_Std_Task_ID());			//	FK
            SetSeqNo(task.GetSeqNo());
            SetName(task.GetName());
            SetDescription(task.GetDescription());
            SetHelp(task.GetHelp());
            if (task.GetVAM_Product_ID() != 0)
                SetVAM_Product_ID(task.GetVAM_Product_ID());
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
            string Sql = "SELECT VAB_Project_ID FROM VAB_ProjectStage WHERE VAB_ProjectStage_ID in(select VAB_ProjectStage_ID FROM" +
                    " VAB_ProjectJob WHERE VAB_ProjectJob_ID =" + GetVAB_ProjectJob_ID() + ")";
            projID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            
            if (projID != 0)
            {
                isOpp = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsOpportunity FROM VAB_Project WHERE VAB_Project_ID = " + projID));
                isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM VAB_Project WHERE VAB_Project_ID = " + projID));
            }
            if (isCam.Equals("Y"))                             // Campaign Window
            {
                MVABProject prj = new MVABProject(GetCtx(), projID, null);
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(PlannedAmt),0)  FROM VAB_ProjectJob WHERE IsActive = 'Y' AND " +
                    "VAB_ProjectStage_ID in (SELECT VAB_ProjectStage_ID FROM VAB_ProjectStage WHERE VAB_Project_ID = " + projID + ")"));
                prj.SetPlannedAmt(plnAmt);
                prj.Save();
            }
            //Amit
            else if (isOpp.Equals("N") && isCam.Equals("N"))
            {
                // set sum of total amount of task tab to phase tab, similalary Commitment amount
                MVABProjectStage phase = new MVABProjectStage(GetCtx(), GetVAB_ProjectStage_ID(), null);
                phase.SetPlannedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectJob pl WHERE pl.IsActive = 'Y' AND pl.VAB_ProjectStage_ID = " + GetVAB_ProjectStage_ID())));
                phase.SetCommittedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.CommittedAmt),0)  FROM VAB_ProjectJob pl WHERE pl.IsActive = 'Y' AND pl.VAB_ProjectStage_ID = " + GetVAB_ProjectStage_ID())));
                if (!phase.Save())
                {

                }
            }
            //Amit
        }

    }
}
