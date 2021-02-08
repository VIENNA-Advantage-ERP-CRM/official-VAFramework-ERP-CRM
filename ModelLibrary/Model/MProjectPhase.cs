/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_ProjectStage
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
using VAdvantage.Logging;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MProjectPhase : X_VAB_ProjectStage
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_ProjectStage_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectPhase(Ctx ctx, int VAB_ProjectStage_ID, Trx trxName)
            : base(ctx, VAB_ProjectStage_ID, trxName)
        {
            if (VAB_ProjectStage_ID == 0)
            {
                //	setVAB_ProjectStage_ID (0);	//	PK
                //	setVAB_Project_ID (0);		//	Parent
                //	setVAB_Std_Stage_ID (0);			//	FK
                SetCommittedAmt(Env.ZERO);
                SetIsCommitCeiling(false);
                SetIsComplete(false);
                SetSeqNo(0);
                //	setName (null);
                SetPlannedAmt(Env.ZERO);
                SetPlannedQty(Env.ZERO);
                SetQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProjectPhase(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="project">parent</param>
        public MProjectPhase(MProject project)
            : this(project.GetCtx(), 0, project.Get_TrxName())
        {
            SetClientOrg(project);
            SetVAB_Project_ID(project.GetVAB_Project_ID());
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="project">parent</param>
        /// <param name="phase">copy</param>
        public MProjectPhase(MProject project, MProjectTypePhase phase)
            : this(project)
        {
            //
            SetVAB_Std_Stage_ID(phase.GetVAB_Std_Stage_ID());			//	FK
            SetName(phase.GetName());
            SetSeqNo(phase.GetSeqNo());
            SetDescription(phase.GetDescription());
            SetHelp(phase.GetHelp());
            if (phase.GetVAM_Product_ID() != 0)
                SetVAM_Product_ID(phase.GetVAM_Product_ID());
            SetQty(phase.GetStandardQty());
        }

        /// <summary>
        /// Copy Tasks from other Phase
        /// </summary>
        /// <param name="fromPhase">from phase</param>
        /// <returns>number of tasks copied</returns>
        public int CopyTasksFrom(MProjectPhase fromPhase)
        {
            if (fromPhase == null)
                return 0;
            int count = 0;
            //
            MProjectTask[] myTasks = GetTasks();
            MProjectTask[] fromTasks = fromPhase.GetTasks();
            //	Copy Project Tasks
            for (int i = 0; i < fromTasks.Length; i++)
            {
                //	Check if Task already exists
                int VAB_Std_Task_ID = fromTasks[i].GetVAB_Std_Task_ID();
                bool exists = false;
                if (VAB_Std_Task_ID == 0)
                    exists = false;
                else
                {
                    for (int ii = 0; ii < myTasks.Length; ii++)
                    {
                        if (myTasks[ii].GetVAB_Std_Task_ID() == VAB_Std_Task_ID)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                //	Phase exist
                if (exists)
                {
                    log.Info("Task already exists here, ignored - " + fromTasks[i]);
                }
                else
                {
                    MProjectTask toTask = new MProjectTask(GetCtx(), 0, Get_TrxName());
                    PO.CopyValues(fromTasks[i], toTask, GetVAF_Client_ID(), GetVAF_Org_ID());
                    toTask.SetVAB_ProjectStage_ID(GetVAB_ProjectStage_ID());
                    if (toTask.Save())
                        count++;
                }
            }
            if (fromTasks.Length != count)
            {
                log.Warning("Count difference - ProjectPhase=" + fromTasks.Length + " <> Saved=" + count);
            }

            return count;
        }

        /// <summary>
        /// Copy Tasks from other Phase
        /// </summary>
        /// <param name="fromPhase">from phase</param>
        /// <returns>number of tasks copied</returns>
        public int CopyTasksFrom(MProjectTypePhase fromPhase)
        {
            if (fromPhase == null)
                return 0;
            int count = 0;
            //	Copy Type Tasks
            MProjectTypeTask[] fromTasks = fromPhase.GetTasks();
            for (int i = 0; i < fromTasks.Length; i++)
            {
                MProjectTask toTask = new MProjectTask(this, fromTasks[i]);
                if (toTask.Save())
                    count++;
            }
            log.Fine("#" + count + " - " + fromPhase);
            if (fromTasks.Length != count)
            {
                log.Log(Level.SEVERE, "Count difference - TypePhase=" + fromTasks.Length + " <> Saved=" + count);
            }

            return count;
        }

        /// <summary>
        /// Get Project Phase Tasks.
        /// </summary>
        /// <returns>Array of tasks</returns>
        public MProjectTask[] GetTasks()
        {
            List<MProjectTask> list = new List<MProjectTask>();
            String sql = "SELECT * FROM VAB_ProjectJob WHERE VAB_ProjectStage_ID=" + GetVAB_ProjectStage_ID() + " ORDER BY SeqNo";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[0].Rows)
                    {
                        list.Add(new MProjectTask(GetCtx(), rs, Get_TrxName()));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            MProjectTask[] retValue = new MProjectTask[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProjectPhase[");
            sb.Append(Get_ID())
                .Append("-").Append(GetSeqNo())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetEndDate() < GetStartDate())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EnddategrtrthnStartdate"));
                return false;
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            string isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM VAB_Project WHERE VAB_Project_ID = " + GetVAB_Project_ID()));
            string isOpp = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsOpportunity FROM VAB_Project WHERE VAB_Project_ID = " + GetVAB_Project_ID()));

            if (isOpp.Equals("N") && isCam.Equals("N"))
            {
                // set sum of total amount of phase tab to project tab, similalary Commitment amount
                MProject project = new MProject(GetCtx(), GetVAB_Project_ID(), null);
                project.SetPlannedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectStage pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID())));
                project.SetCommittedAmt(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.CommittedAmt),0)  FROM VAB_ProjectStage pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID())));
                if (!project.Save())
                {

                }

                // set sum of planned margin from task line to project tab
                string Sql = @"SELECT SUM(pl.PlannedMarginAmt) ,  SUM(pl.PlannedQty)
                          FROM VAB_Project p INNER JOIN VAB_ProjectStage pp ON p.VAB_Project_ID = pp.VAB_Project_ID
                          INNER JOIN VAB_ProjectJob pt ON pp.VAB_ProjectStage_id = pt.VAB_ProjectStage_id INNER JOIN VAB_ProjectLine pl ON pl.VAB_ProjectJob_id = pt.VAB_ProjectJob_id
                          WHERE p.VAB_Project_ID   = " + project.GetVAB_Project_ID();
                DataSet ds = new DataSet();
                ds = DB.ExecuteDataset(Sql, null, null);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        project.SetPlannedMarginAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0][0]));
                        project.SetPlannedQty(Util.GetValueOfDecimal(ds.Tables[0].Rows[0][1]));
                        project.Save();
                    }
                }
                ds.Dispose();

                //Set Total Amount Of Phase Line on Phase Tab
                MProjectPhase prjph = null;
                if (GetVAB_Project_ID() != 0)
                {
                    isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM VAB_Project WHERE VAB_Project_ID = " + GetVAB_Project_ID()));
                }
                if (isCam.Equals("N"))                             // Project Window
                {
                    prjph = new MProjectPhase(GetCtx(), GetVAB_ProjectStage_ID(), null);
                    decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(PlannedAmt),0) FROM VAB_ProjectLine WHERE IsActive= 'Y' AND VAB_ProjectStage_ID= " + GetVAB_ProjectStage_ID()));
                    DB.ExecuteQuery("UPDATE VAB_ProjectStage SET PlannedAmt=" + plnAmt + " WHERE VAB_ProjectStage_ID=" + GetVAB_ProjectStage_ID());
                }
            }
            return true;
        }

    }
}
