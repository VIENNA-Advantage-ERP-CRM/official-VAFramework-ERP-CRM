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
        int _C_Lead_ID;
        protected override void Prepare()
        {
            _C_Lead_ID = GetRecord_ID();

        }


        protected override String DoIt()
        {
            VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), _C_Lead_ID, Get_TrxName());
            //  lead.GetRef_BPartner_ID()))
            int ExCustomer = lead.GetC_BPartner_ID();
            int Pospect = lead.GetRef_BPartner_ID();
            

            
            if (ExCustomer != 0)
            {
                VAdvantage.Model.X_C_Project opp = new VAdvantage.Model.X_C_Project(GetCtx(), 0, Get_TrxName());
                opp.SetC_Lead_ID(lead.GetC_Lead_ID());
                opp.SetC_BPartner_ID (lead.GetC_BPartner_ID());
                opp.SetSalesRep_ID (lead.GetSalesRep_ID());
                opp.SetDateContract(DateTime.Today);
                opp.SetC_Campaign_ID (lead.GetC_Campaign_ID());
                //opp.SetR_Source_ID (lead.GetR_Source_ID());
               // opp.SetOpportunityStatus("N");
                opp.SetAD_User_ID(lead.GetAD_User_ID());
                VAdvantage.Model.X_C_BPartner bp=new VAdvantage.Model.X_C_BPartner(GetCtx(),ExCustomer,Get_TrxName());
                VAdvantage.Model.X_C_BPartner_Location loc=new VAdvantage.Model.X_C_BPartner_Location (GetCtx(),ExCustomer,Get_TrxName());

                opp.SetName(bp.GetName());;
                opp.SetC_BPartner_Location_ID (loc.GetC_BPartner_Location_ID());
                opp.SetIsOpportunity(true);

                if (opp.Save())
                {
                    lead.SetC_Project_ID(opp.GetC_Project_ID());
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
                VAdvantage.Model.X_C_Project opp = new VAdvantage.Model.X_C_Project(GetCtx(), 0, Get_TrxName());
                opp.SetC_Lead_ID(lead.GetC_Lead_ID());
                opp.SetC_BPartnerSR_ID(lead.GetRef_BPartner_ID());
                opp.SetSalesRep_ID (lead.GetSalesRep_ID());
                opp.SetDateContract(DateTime.Today);
                opp.SetC_Campaign_ID ( lead.GetC_Campaign_ID());
               // opp.SetR_Source_ID (lead.GetR_Source_ID());
                //opp.SetOpportunityStatus ("N");
               // opp.SetAD_Client_ID(GetAD_Client_ID());
                opp.SetAD_User_ID(lead.GetAD_User_ID());
                VAdvantage.Model.X_C_BPartner bp = new VAdvantage.Model.X_C_BPartner(GetCtx(), Pospect, Get_TrxName());
                //X_C_BPartner_Location loc = new X_C_BPartner_Location(GetCtx(), Pospect, Get_TrxName());

                opp.SetName ( bp.GetName()); 
                opp.SetC_BPartner_Location_ID (lead.GetC_BPartner_Location_ID());
                opp.SetIsOpportunity(true);

                if (opp.Save())
                {
                   lead.SetC_Project_ID(opp.GetC_Project_ID());
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
                //CallProcess(_C_Lead_ID);
                callprospect();
                return DoIt(); 
              
            }

            return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
        }
        private void CallProcess(int lead_id)
        {
            string sql = "select ad_process_id from ad_process where name = 'C_Lead BPartner'";
            int AD_Process_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName())); // 1000025;

            MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, GetRecord_ID());
            if (!instance.Save())
            {
                return;
            }

            ProcessInfo pi = new ProcessInfo("VInOutGen", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

            // Add Parameter - Selection=Y
            MPInstancePara para = new MPInstancePara(instance, 10);

            // Add Parameter - M_Warehouse_ID=x
            para = new MPInstancePara(instance, 20);
            para.setParameter("_C_Lead_ID", lead_id);
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
            log.Info("C_Lead_ID=" + _C_Lead_ID);
            if (_C_Lead_ID == 0)
            {
                throw new Exception("@C_Lead_ID@ ID=0");
            }
            VAdvantage.Model.MLead lead = new VAdvantage.Model.MLead(GetCtx(), _C_Lead_ID, Get_TrxName());
            if (lead.Get_ID() != _C_Lead_ID)
            {
                throw new Exception("@NotFound@: @C_Lead_ID@ ID=" + _C_Lead_ID);
            }
            //
            String retValue = lead.CreateBP();
            if (retValue != null)
            {
                throw new SystemException(retValue);
            }
            lead.Save();
            //
            VAdvantage.Model.MBPartner bp = lead.GetBPartner();
            if (bp != null)
            {
                return "@C_BPartner_ID@: " + bp.GetName();
            }
            VAdvantage.Model.MUser user = lead.GetUser();
            if (user != null)
            {
                return "@AD_User_ID@: " + user.GetName();
            }
            return "@SaveError@";
        }	//	doIt

    }


}