using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
   public class ProspectToOpportunity:SvrProcess
    {
        int _C_Lead_ID;
        protected override void Prepare()
        {
            
        }

        protected override string DoIt()
        {
            VAdvantage.Model.X_C_BPartner partner = new VAdvantage.Model.X_C_BPartner(GetCtx(), GetRecord_ID(), null);
            
            string _BPName =partner.GetName();

            string _sql = "Select C_Lead_ID From C_Lead Where BPName='" + _BPName + "'  AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID();
             DataSet ds = new DataSet();
                 ds = DB.ExecuteDataset(_sql.ToString());
                 if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                 {
                     for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                     {
                         _C_Lead_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Lead_ID"]);
                         VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), _C_Lead_ID, Get_TrxName());
                         //  lead.GetRef_BPartner_ID()))
                         //int ExCustomer = lead.GetC_BPartner_ID();
                         int Pospect = lead.GetRef_BPartner_ID();
                         
                       
                         if (Pospect != 0)
                         {
                             VAdvantage.Model.X_C_Project opp = new VAdvantage.Model.X_C_Project(GetCtx(), 0, Get_TrxName());
                             opp.SetC_Lead_ID(lead.GetC_Lead_ID());
                             opp.SetC_BPartnerSR_ID(lead.GetRef_BPartner_ID());
                             opp.SetSalesRep_ID(lead.GetSalesRep_ID());
                             opp.SetDateContract(DateTime.Today);
                             opp.SetC_Campaign_ID(lead.GetC_Campaign_ID());
                             // opp.SetR_Source_ID (lead.GetR_Source_ID());
                             //opp.SetOpportunityStatus ("N");
                             // opp.SetVAF_Client_ID(GetVAF_Client_ID());
                             opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                             VAdvantage.Model.X_C_BPartner bp = new VAdvantage.Model.X_C_BPartner(GetCtx(), Pospect, Get_TrxName());
                             //X_C_BPartner_Location loc = new X_C_BPartner_Location(GetCtx(), Pospect, Get_TrxName());

                             opp.SetName(bp.GetName());
                             opp.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                             opp.SetIsOpportunity(true);


                             if (opp.Save())
                             {
                                 lead.SetC_Project_ID(opp.GetC_Project_ID());
                                 lead.Save();
                                 
                                 bp.SetCreateProject("Y");
                                 if (bp.Save())
                                 { 
                                 }
                                
                                 return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");
                             }
                             else
                             {
                                 return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                             }

                         }
                         //if (ExCustomer != 0)
                         //{
                         //    VAdvantage.Model.X_C_Project opp = new VAdvantage.Model.X_C_Project(GetCtx(), 0, Get_TrxName());
                         //    opp.SetC_Lead_ID(lead.GetC_Lead_ID());
                         //    opp.SetC_BPartner_ID(lead.GetC_BPartner_ID());
                         //    opp.SetSalesRep_ID(lead.GetSalesRep_ID());
                         //    opp.SetDateContract(DateTime.Today);
                         //    opp.SetC_Campaign_ID(lead.GetC_Campaign_ID());
                         //    opp.SetR_Source_ID(lead.GetR_Source_ID());
                         //    opp.SetOpportunityStatus("N");
                         //    opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                         //    VAdvantage.Model.X_C_BPartner bp = new VAdvantage.Model.X_C_BPartner(GetCtx(), ExCustomer, Get_TrxName());
                         //    VAdvantage.Model.X_C_BPartner_Location loc = new VAdvantage.Model.X_C_BPartner_Location(GetCtx(), ExCustomer, Get_TrxName());

                         //    opp.SetName(bp.GetName()); ;
                         //    opp.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                         //    opp.SetIsOpportunity(true);

                         //    if (opp.Save())
                         //    {
                         //        lead.SetC_Project_ID(opp.GetC_Project_ID());
                         //        lead.Save();
                         //        return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");

                         //    }
                         //    else
                         //    {
                         //        return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");

                         //    }
                         //}
                         


                     }

                 }
                 return "";
        }
    }
}
