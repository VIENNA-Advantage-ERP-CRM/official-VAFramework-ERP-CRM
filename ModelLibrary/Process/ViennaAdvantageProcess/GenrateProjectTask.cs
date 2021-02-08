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
      //  private int VAB_Project_ID = 0;
        /** Project Type Parameter			*/
       // private int VAB_ProjectType_ID = 0;
        private int _VAB_Promotion_ID = 0;
        //private int VAB_Std_Stage_ID = 0;
        //private int VAB_ProjectStage_ID = 0;
        //private int VAB_Std_Task_ID = 0;
       // private int VAB_ProjectJob_ID = 0;
        private int VAB_PromotionType_ID = 0;


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
            _VAB_Promotion_ID = GetRecord_ID();
            VAdvantage.Model.MVABPromotion Campaign = new VAdvantage.Model.MVABPromotion(GetCtx(), _VAB_Promotion_ID, Get_TrxName());
            //VAB_ProjectType_ID = Campaign.GetVAB_ProjectType_ID();
            VAB_PromotionType_ID = Campaign.GetVAB_PromotionType_ID();
            VAdvantage.Model.MVABPromotionType CampaignType = new VAdvantage.Model.MVABPromotionType(GetCtx(), VAB_PromotionType_ID, Get_TrxName());
            //MProjectType type = new MProjectType(GetCtx(), VAB_ProjectType_ID, Get_TrxName());
            VAdvantage.Model.MProject Project = new VAdvantage.Model.MProject(GetCtx(), 0, Get_TrxName());
            VAdvantage.Model.MVABPromotionPhase CampaignPhase = new VAdvantage.Model.MVABPromotionPhase(GetCtx(), 0, Get_TrxName());


            // MPhase Phase = new MPhase(GetCtx(), 0, Get_TrxName());
            VAdvantage.Model.MProjectTypeTask task = new VAdvantage.Model.MProjectTypeTask(GetCtx(), 0, Get_TrxName());

            Project.SetName(CampaignType.GetName());
            Project.SetVAB_Promotion_ID(GetRecord_ID());
            Project.SetSalesRep_ID(GetVAF_UserContact_ID());
            Project.SetDateContract(Campaign.GetStartDate());
            Project.SetDateFinish(Campaign.GetEndDate());
            Project.SetSalesRep_ID(Campaign.GetSalesRep_ID());
            Project.SetVAF_Client_ID(CampaignType.GetVAF_Client_ID());
            Project.SetVAF_Org_ID(CampaignType.GetVAF_Org_ID());
            Project.SetIsCampaign(true);

            if (!Project.Save(Get_TrxName()))
            {
                return GetRetrievedError(Project, "CampaignNotSaved");
                //log.SaveError("CampaignNotSaved", "");
                //return Msg.GetMsg(GetCtx(), "CampaignNotSaved");
            }
            int[] allids = VAdvantage.Model.X_VAB_PromotionPhase.GetAllIDs("VAB_PromotionPhase", "VAB_PromotionType_ID=" + VAB_PromotionType_ID + " and VAF_Client_ID = " + GetVAF_Client_ID() + " Order By SEQNO ", Get_TrxName());
            //int[] allids = X_VAB_Std_Stage.GetAllIDs("VAB_Std_Stage", "VAB_ProjectType_ID=" + VAB_ProjectType_ID, Get_TrxName());
            for (int i = 0; i < allids.Length; i++)
            {
                VAdvantage.Model.X_VAB_PromotionPhase CampPhase = new VAdvantage.Model.X_VAB_PromotionPhase(GetCtx(), allids[i], Get_TrxName());
                //X_VAB_Std_Stage Phase1 = new X_VAB_Std_Stage(GetCtx(), allids[i], Get_TrxName());
                int VAB_PromotionPhase_ID = CampPhase.GetVAB_PromotionPhase_ID();
                //  int VAB_Std_Stage_ID = Phase1.GetVAB_Std_Stage_ID();
                if (VAB_PromotionPhase_ID != 0)
                {
                    VAdvantage.Model.MProjectPhase ProjectPhase = new VAdvantage.Model.MProjectPhase(GetCtx(), 0, Get_TrxName());
                    //ProjectPhase.SetVAF_Client_ID(Phase1.GetVAF_Client_ID());
                    ProjectPhase.SetVAF_Client_ID(CampPhase.GetVAF_Client_ID());
                    //ProjectPhase.SetVAF_Org_ID(Phase1.GetVAF_Org_ID());
                    ProjectPhase.SetVAF_Org_ID(CampPhase.GetVAF_Org_ID());
                    ProjectPhase.SetVAB_Project_ID(Project.GetVAB_Project_ID());
                    //ProjectPhase.SetName(Phase1.GetName());
                    ProjectPhase.SetName(CampPhase.GetName());
                    // ProjectPhase.SetVAM_Product_ID(Phase1.GetVAM_Product_ID());
                    //ProjectPhase.SetVAM_Product_ID(CampPhase.GetVAM_Product_ID());
                    // ProjectPhase.SetQty(Phase1.GetStandardQty());
                    //ProjectPhase.SetQty(CampPhase.GetStandardQty());
                    //ProjectPhase.SetVAB_Std_Stage_ID(Phase1.GetVAB_Std_Stage_ID());
                    //ProjectPhase.SetVAB_Std_Stage_ID(CampPhase.GetVAB_PromotionPhase_ID());
                    ProjectPhase.SetVAB_Project_ID(Project.GetVAB_Project_ID());
                    if (!ProjectPhase.Save(Get_TrxName()))
                    {
                        return GetRetrievedError(ProjectPhase, "CampaignPhasetNotSaved");
                        //log.SaveError("CampaignPhasetNotSaved", "");
                        //return Msg.GetMsg(GetCtx(), "CampaignPhasetNotSaved");
                    }
                    int[] allids1 = VAdvantage.Model.X_VAB_PromotionTask.GetAllIDs("VAB_PromotionTask", "VAB_PromotionPhase_ID=" + VAB_PromotionPhase_ID + " and VAF_Client_ID = " + GetVAF_Client_ID() + " Order By SEQNO ", Get_TrxName());
                    //int[] allids1 = X_VAB_Std_Task.GetAllIDs("VAB_Std_Task", "VAB_Std_Stage_ID=" + VAB_Std_Stage_ID, Get_TrxName());
                    for (int j = 0; j < allids1.Length; j++)
                    {
                        VAdvantage.Model.X_VAB_PromotionTask CampaignTask = new VAdvantage.Model.X_VAB_PromotionTask(GetCtx(), allids1[j], Get_TrxName());
                        //X_VAB_Std_Task task1 = new X_VAB_Std_Task(GetCtx(), allids1[j], Get_TrxName());
                        int VAB_PromotionTask_ID = CampaignTask.GetVAB_PromotionTask_ID();
                        //int VAB_Std_Task_ID = task1.GetVAB_Std_Task_ID();
                        if (VAB_PromotionTask_ID != 0)
                        {
                            VAdvantage.Model.MProjectTask ProjectTask = new VAdvantage.Model.MProjectTask(GetCtx(), 0, Get_TrxName());
                            //ProjectTask.SetVAF_Client_ID(task1.GetVAF_Client_ID());
                          //  ProjectTask.SetVAF_Client_ID(CampaignTask.GetVAF_Client_ID());
                            // ProjectTask.SetVAF_Client_ID(task1.GetVAF_Client_ID());
                          //  ProjectTask.SetVAF_Org_ID(CampaignTask.GetVAF_Client_ID());
                            ProjectTask.SetVAB_ProjectStage_ID(ProjectPhase.GetVAB_ProjectStage_ID());
                            //ProjectTask.SetName(task1.GetName());
                            ProjectTask.SetName(CampaignTask.GetName());
                            // ProjectTask.SetVAM_Product_ID(task1.GetVAM_Product_ID());
                            //ProjectTask.SetVAM_Product_ID(CampaignTask.GetVAM_Product_ID());
                            // ProjectTask.SetQty(task1.GetStandardQty());
                            //ProjectTask.SetQty(CampaignTask.GetStandardQty());
                            //ProjectTask.SetVAB_Std_Task_ID(task1.GetVAB_Std_Task_ID());
                            ProjectTask.SetVAB_ProjectJob_ID(ProjectPhase.GetVAB_ProjectStage_ID());
                            //ProjectTask.SetVAB_Std_Task_ID(ProjectPhase.GetVAB_ProjectStage_ID());
                            // ProjectTask.SetVAB_Std_Task_ID(CampaignTask.GetVAB_PromotionTask_ID());
                            string SQL = "SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_ProjectJob WHERE VAB_ProjectStage_ID=" + ProjectPhase.GetVAB_ProjectStage_ID();
                            object SeqNo = DB.ExecuteScalar(SQL, null, Get_TrxName());
                            ProjectTask.SetSeqNo(Util.GetValueOfInt(SeqNo));
                            if (!ProjectTask.Save(Get_TrxName()))
                            {
                                return GetRetrievedError(ProjectTask, "CampaignTasktNotSaved");

                                //log.SaveError("CampaignTasktNotSaved", "");
                                //return Msg.GetMsg(GetCtx(), "CampaignTasktNotSaved");
                            }

                        }
                    }


                }
            }
            Campaign.SetGenerateProject("Y");
            if (!Campaign.Save(Get_TrxName()))
            {
                return GetRetrievedError(Campaign, "CampaignNotSaved");
                //log.SaveError("CampaignNotSaved", "");
                //return Msg.GetMsg(GetCtx(), "CampaignNotSaved");
            }

            return Msg.GetMsg(GetCtx(), "PlanningGenerationDone");
        }



        //	doIt
    }
}


