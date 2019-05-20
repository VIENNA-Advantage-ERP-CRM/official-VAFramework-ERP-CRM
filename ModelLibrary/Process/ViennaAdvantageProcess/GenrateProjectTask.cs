using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using ViennaAdvantage.Process;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
//using ViennaAdvantageServer.Model;

namespace ViennaAdvantage.Process
{
    public class GenrateProjectTask : SvrProcess
    {
        /**	Project directly from Project	*/
      //  private int C_Project_ID = 0;
        /** Project Type Parameter			*/
       // private int C_ProjectType_ID = 0;
        private int _C_Campaign_ID = 0;
        //private int C_Phase_ID = 0;
        //private int C_ProjectPhase_ID = 0;
        //private int C_Task_ID = 0;
       // private int C_ProjectTask_ID = 0;
        private int C_CampaignType_ID = 0;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        /// 
        protected override void Prepare()
        {


        }	//	prepare

        protected override String DoIt()
        {

            //String msg = "";
            _C_Campaign_ID = GetRecord_ID();
            VAdvantage.Model.MCampaign Campaign = new VAdvantage.Model.MCampaign(GetCtx(), _C_Campaign_ID, Get_TrxName());
            //C_ProjectType_ID = Campaign.GetC_ProjectType_ID();
            C_CampaignType_ID = Campaign.GetC_CampaignType_ID();
            VAdvantage.Model.MCampaignType CampaignType = new VAdvantage.Model.MCampaignType(GetCtx(), C_CampaignType_ID, Get_TrxName());
            //MProjectType type = new MProjectType(GetCtx(), C_ProjectType_ID, Get_TrxName());
            VAdvantage.Model.MProject Project = new VAdvantage.Model.MProject(GetCtx(), 0, Get_TrxName());
            VAdvantage.Model.MCampaignPhase CampaignPhase = new VAdvantage.Model.MCampaignPhase(GetCtx(), 0, Get_TrxName());


            // MPhase Phase = new MPhase(GetCtx(), 0, Get_TrxName());
            VAdvantage.Model.MProjectTypeTask task = new VAdvantage.Model.MProjectTypeTask(GetCtx(), 0, Get_TrxName());

            Project.SetName(CampaignType.GetName());
            Project.SetC_Campaign_ID(GetRecord_ID());
            Project.SetSalesRep_ID(GetAD_User_ID());
            Project.SetDateContract(Campaign.GetStartDate());
            Project.SetDateFinish(Campaign.GetEndDate());
            Project.SetSalesRep_ID(Campaign.GetSalesRep_ID());
            Project.SetAD_Client_ID(CampaignType.GetAD_Client_ID());
            Project.SetAD_Org_ID(CampaignType.GetAD_Org_ID());
            Project.SetIsCampaign(true);

            if (!Project.Save(Get_TrxName()))
            {
                log.SaveError("CampaignNotSaved", "");
                return Msg.GetMsg(GetCtx(), "CampaignNotSaved");
            }
            int[] allids = VAdvantage.Model.X_C_CampaignPhase.GetAllIDs("C_CampaignPhase", "C_CampaignType_ID=" + C_CampaignType_ID + " and AD_Client_ID = " + GetAD_Client_ID() + " Order By SEQNO ", Get_TrxName());
            //int[] allids = X_C_Phase.GetAllIDs("C_Phase", "C_ProjectType_ID=" + C_ProjectType_ID, Get_TrxName());
            for (int i = 0; i < allids.Length; i++)
            {
                VAdvantage.Model.X_C_CampaignPhase CampPhase = new VAdvantage.Model.X_C_CampaignPhase(GetCtx(), allids[i], Get_TrxName());
                //X_C_Phase Phase1 = new X_C_Phase(GetCtx(), allids[i], Get_TrxName());
                int C_CampaignPhase_ID = CampPhase.GetC_CampaignPhase_ID();
                //  int C_Phase_ID = Phase1.GetC_Phase_ID();
                if (C_CampaignPhase_ID != 0)
                {
                    VAdvantage.Model.MProjectPhase ProjectPhase = new VAdvantage.Model.MProjectPhase(GetCtx(), 0, Get_TrxName());
                    //ProjectPhase.SetAD_Client_ID(Phase1.GetAD_Client_ID());
                    ProjectPhase.SetAD_Client_ID(CampPhase.GetAD_Client_ID());
                    //ProjectPhase.SetAD_Org_ID(Phase1.GetAD_Org_ID());
                    ProjectPhase.SetAD_Org_ID(CampPhase.GetAD_Org_ID());
                    ProjectPhase.SetC_Project_ID(Project.GetC_Project_ID());
                    //ProjectPhase.SetName(Phase1.GetName());
                    ProjectPhase.SetName(CampPhase.GetName());
                    // ProjectPhase.SetM_Product_ID(Phase1.GetM_Product_ID());
                    //ProjectPhase.SetM_Product_ID(CampPhase.GetM_Product_ID());
                    // ProjectPhase.SetQty(Phase1.GetStandardQty());
                    //ProjectPhase.SetQty(CampPhase.GetStandardQty());
                    //ProjectPhase.SetC_Phase_ID(Phase1.GetC_Phase_ID());
                    //ProjectPhase.SetC_Phase_ID(CampPhase.GetC_CampaignPhase_ID());
                    ProjectPhase.SetC_Project_ID(Project.GetC_Project_ID());
                    if (!ProjectPhase.Save(Get_TrxName()))
                    {
                        log.SaveError("CampaignPhasetNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignPhasetNotSaved");
                    }
                    int[] allids1 = VAdvantage.Model.X_C_CampaignTask.GetAllIDs("C_CampaignTask", "C_CampaignPhase_ID=" + C_CampaignPhase_ID + " and AD_Client_ID = " + GetAD_Client_ID() + " Order By SEQNO ", Get_TrxName());
                    //int[] allids1 = X_C_Task.GetAllIDs("C_Task", "C_Phase_ID=" + C_Phase_ID, Get_TrxName());
                    for (int j = 0; j < allids1.Length; j++)
                    {
                        VAdvantage.Model.X_C_CampaignTask CampaignTask = new VAdvantage.Model.X_C_CampaignTask(GetCtx(), allids1[j], Get_TrxName());
                        //X_C_Task task1 = new X_C_Task(GetCtx(), allids1[j], Get_TrxName());
                        int C_CampaignTask_ID = CampaignTask.GetC_CampaignTask_ID();
                        //int C_Task_ID = task1.GetC_Task_ID();
                        if (C_CampaignTask_ID != 0)
                        {
                            VAdvantage.Model.MProjectTask ProjectTask = new VAdvantage.Model.MProjectTask(GetCtx(), 0, Get_TrxName());
                            //ProjectTask.SetAD_Client_ID(task1.GetAD_Client_ID());
                          //  ProjectTask.SetAD_Client_ID(CampaignTask.GetAD_Client_ID());
                            // ProjectTask.SetAD_Client_ID(task1.GetAD_Client_ID());
                          //  ProjectTask.SetAD_Org_ID(CampaignTask.GetAD_Client_ID());
                            ProjectTask.SetC_ProjectPhase_ID(ProjectPhase.GetC_ProjectPhase_ID());
                            //ProjectTask.SetName(task1.GetName());
                            ProjectTask.SetName(CampaignTask.GetName());
                            // ProjectTask.SetM_Product_ID(task1.GetM_Product_ID());
                            //ProjectTask.SetM_Product_ID(CampaignTask.GetM_Product_ID());
                            // ProjectTask.SetQty(task1.GetStandardQty());
                            //ProjectTask.SetQty(CampaignTask.GetStandardQty());
                            //ProjectTask.SetC_Task_ID(task1.GetC_Task_ID());
                            ProjectTask.SetC_ProjectTask_ID(ProjectPhase.GetC_ProjectPhase_ID());
                            //ProjectTask.SetC_Task_ID(ProjectPhase.GetC_ProjectPhase_ID());
                            // ProjectTask.SetC_Task_ID(CampaignTask.GetC_CampaignTask_ID());
                            string SQL = "SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM C_ProjectTask WHERE C_ProjectPhase_ID=" + ProjectPhase.GetC_ProjectPhase_ID();
                            object SeqNo = DB.ExecuteScalar(SQL, null, Get_TrxName());
                            ProjectTask.SetSeqNo(Util.GetValueOfInt(SeqNo));
                            if (!ProjectTask.Save(Get_TrxName()))
                            {
                                log.SaveError("CampaignTasktNotSaved", "");
                                return Msg.GetMsg(GetCtx(), "CampaignTasktNotSaved");
                            }

                        }
                    }


                }
            }
            Campaign.SetGenerateProject("Y");
            if (!Campaign.Save(Get_TrxName()))
            {
                log.SaveError("CampaignNotSaved", "");
                return Msg.GetMsg(GetCtx(), "CampaignNotSaved");
            }

            return Msg.GetMsg(GetCtx(), "PlanningGenerationDone");
        }



        //	doIt
    }
}


