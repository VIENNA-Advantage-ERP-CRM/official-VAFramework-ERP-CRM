using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
//using ViennaAdvantage.Model;


 /* Writer :Arpit Singh
 * Date   : 30/12/11 
 */
namespace VAdvantage.Process
{
    class LeadToOpportunity : SvrProcess
    {
        int _VAB_Lead_ID;
        protected override void Prepare()
        {
            _VAB_Lead_ID = GetRecord_ID();

        }


        protected override String DoIt()
        {
            VAdvantage.Model.X_VAB_Lead lead = new VAdvantage.Model.X_VAB_Lead(GetCtx(), _VAB_Lead_ID, Get_TrxName());
            //  lead.GetRef_BPartner_ID()))
            int ExCustomer = lead.GetVAB_BusinessPartner_ID();
            int Pospect = lead.GetRef_BPartner_ID();
            

            
            if (ExCustomer != 0)
            {
                VAdvantage.Model.X_VAB_Project opp = new VAdvantage.Model.X_VAB_Project(GetCtx(), 0, Get_TrxName());
                opp.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                opp.SetVAB_BusinessPartner_ID (lead.GetVAB_BusinessPartner_ID());
                opp.SetSalesRep_ID (lead.GetSalesRep_ID());
                opp.SetDateContract(DateTime.Today);
                opp.SetVAB_Promotion_ID (lead.GetVAB_Promotion_ID());
                //opp.SetVAR_Source_ID (lead.GetVAR_Source_ID());
               // opp.SetOpportunityStatus("N");
                opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                VAdvantage.Model.X_VAB_BusinessPartner bp=new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(),ExCustomer,Get_TrxName());
                VAdvantage.Model.X_VAB_BPart_Location loc=new VAdvantage.Model.X_VAB_BPart_Location (GetCtx(),ExCustomer,Get_TrxName());

                opp.SetName(bp.GetName());;
                opp.SetVAB_BPart_Location_ID (loc.GetVAB_BPart_Location_ID());
                opp.SetIsOpportunity(true);

                if (opp.Save())
                {
                    lead.SetVAB_Project_ID(opp.GetVAB_Project_ID());
                    lead.Save();
                    return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");
                    
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                   
                }
            }
            if (Pospect != 0)
            {
                VAdvantage.Model.X_VAB_Project opp = new VAdvantage.Model.X_VAB_Project(GetCtx(), 0, Get_TrxName());
                opp.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                opp.SetVAB_BusinessPartnerSR_ID(lead.GetRef_BPartner_ID());
                opp.SetSalesRep_ID (lead.GetSalesRep_ID());
                opp.SetDateContract(DateTime.Today);
                opp.SetVAB_Promotion_ID ( lead.GetVAB_Promotion_ID());
               // opp.SetVAR_Source_ID (lead.GetVAR_Source_ID());
                //opp.SetOpportunityStatus ("N");
               // opp.SetVAF_Client_ID(GetVAF_Client_ID());
                opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                VAdvantage.Model.X_VAB_BusinessPartner bp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), Pospect, Get_TrxName());
                //X_VAB_BPart_Location loc = new X_VAB_BPart_Location(GetCtx(), Pospect, Get_TrxName());

                opp.SetName ( bp.GetName()); 
                opp.SetVAB_BPart_Location_ID (lead.GetVAB_BPart_Location_ID());
                opp.SetIsOpportunity(true);

                if (opp.Save())
                {
                   lead.SetVAB_Project_ID(opp.GetVAB_Project_ID());
                   lead.Save();
                   return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                }

            }
            if (ExCustomer == 0 && Pospect == 0)
            {
                //CallProcess(_VAB_Lead_ID);
                callprospect();
                return DoIt(); 
              
            }

            return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
        }
        private void CallProcess(int lead_id)
        {
            string sql = "select VAF_Job_id from VAF_Job where name = 'VAB_Lead BPartner'";
            int VAF_Job_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName())); // 1000025;

            MVAFJInstance instance = new MVAFJInstance(GetCtx(), VAF_Job_ID, GetRecord_ID());
            if (!instance.Save())
            {
                return;
            }

            ProcessInfo pi = new ProcessInfo("VInOutGen", VAF_Job_ID);
            pi.SetVAF_JInstance_ID(instance.GetVAF_JInstance_ID());

            // Add Parameter - Selection=Y
            MVAFJInstancePara para = new MVAFJInstancePara(instance, 10);

            // Add Parameter - M_Warehouse_ID=x
            para = new MVAFJInstancePara(instance, 20);
            para.setParameter("_VAB_Lead_ID", lead_id);
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                // lblStatusInfo.Text = msg.ToString();
                log.Log(Level.SEVERE, msg);
                return;
            }

             //Execute Process
            //ASyncProcess asp=null;
            //ProcessCtl worker = new ProcessCtl(asp,pi, null);
            //worker.Run();     //  complete tasks in unlockUI / generateInvoice_complete

        }


        public String callprospect()
        {
            log.Info("VAB_Lead_ID=" + _VAB_Lead_ID);
            if (_VAB_Lead_ID == 0)
            {
                throw new Exception("@VAB_Lead_ID@ ID=0");
            }
            VAdvantage.Model.MVABLead lead = new VAdvantage.Model.MVABLead(GetCtx(), _VAB_Lead_ID, Get_TrxName());
            if (lead.Get_ID() != _VAB_Lead_ID)
            {
                throw new Exception("@NotFound@: @VAB_Lead_ID@ ID=" + _VAB_Lead_ID);
            }
            //
            String retValue = lead.CreateBP();
            if (retValue != null)
            {
                throw new SystemException(retValue);
            }
            lead.Save();
            //
            VAdvantage.Model.MVABBusinessPartner bp = lead.GetBPartner();
            if (bp != null)
            {
                return "@VAB_BusinessPartner_ID@: " + bp.GetName();
            }
            VAdvantage.Model.MVAFUserContact user = lead.GetUser();
            if (user != null)
            {
                return "@VAF_UserContact_ID@: " + user.GetName();
            }
            return "@SaveError@";
        }	//	doIt

    }


}