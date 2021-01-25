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

namespace ViennaAdvantage.Process
{
    class SubscribeInterestArea : SvrProcess
    {

        int _C_Lead_ID;
        int R_InterestArea_ID;
        protected override void Prepare()
        {
            _C_Lead_ID = GetRecord_ID();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("R_InterestArea_ID"))
                {
                    R_InterestArea_ID = Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else
                {

                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }

        }
        protected override String DoIt()
        {

            VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), _C_Lead_ID, Get_Trx());
            if (!(R_InterestArea_ID > 0))
            {
                return Msg.GetMsg(GetCtx(), "NoInterestAreaIsSelected");
               
            }
            
            if (lead.GetVAB_BusinessPartner_ID() != 0)
            {
                
                VAdvantage.Model.X_R_ContactInterest customer = new VAdvantage.Model.X_R_ContactInterest(GetCtx(), 0, Get_Trx());
                customer.SetR_InterestArea_ID(R_InterestArea_ID);
                customer.SetVAB_BusinessPartner_ID(lead.GetVAB_BusinessPartner_ID());
                String query = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id= " + lead.GetVAB_BusinessPartner_ID();
                int UserId=Util.GetValueOfInt( DB.ExecuteScalar(query));
                customer.SetVAF_UserContact_ID(UserId);
                query = "Select VAB_BPart_Location_id from VAB_BPart_Location where VAB_BusinessPartner_id= " + lead.GetVAB_BusinessPartner_ID();

                int Id = Util.GetValueOfInt(DB.ExecuteScalar(query));
                VAdvantage.Model.X_VAB_BPart_Location loc = new VAdvantage.Model.X_VAB_BPart_Location(GetCtx(), Id, Get_Trx());
                customer.SetVAB_BPart_Location_ID(Id);
                customer.SetPhone(loc.GetPhone());
                customer.SetFax(loc.GetFax());

                VAdvantage.Model.X_VAF_UserContact us = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), UserId, Get_Trx());
                customer.SetVAB_Position_ID(us.GetVAB_Position_ID());
                customer.SetSubscribeDate(DateTime.Today);
                query = "Select Email from VAF_UserContact where VAF_UserContact_id= " + UserId;
                String mail = Util.GetValueOfString( DB.ExecuteScalar(query));
                customer.SetEMail(mail);
                if (!customer.Save())
                {
                    return GetRetrievedError(customer, "NotSubscribed");
                    //return Msg.GetMsg(GetCtx(), "NotSubscribed");
                   
                }
                lead.SetR_InterestArea_ID(customer.GetR_InterestArea_ID());
                if(lead.Save())
                {
                }
                return Msg.GetMsg(GetCtx(), "SubscribedDone");
                

                
            }
            else if (lead.GetRef_BPartner_ID() != 0)
            {
                VAdvantage.Model.X_R_ContactInterest Prospect = new VAdvantage.Model.X_R_ContactInterest(GetCtx(), 0, Get_Trx());
                Prospect.SetR_InterestArea_ID(R_InterestArea_ID);
                Prospect.SetVAB_BusinessPartner_ID(lead.GetRef_BPartner_ID());
                String query = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id= " + lead.GetRef_BPartner_ID();
                int UserId = Util.GetValueOfInt(DB.ExecuteScalar(query));
                Prospect.SetVAF_UserContact_ID(UserId);
                query = "Select VAB_BPart_Location_id from VAB_BPart_Location where VAB_BusinessPartner_id= " + lead.GetRef_BPartner_ID();

                int Id = Util.GetValueOfInt(DB.ExecuteScalar(query));
                VAdvantage.Model.X_VAB_BPart_Location loc = new VAdvantage.Model.X_VAB_BPart_Location(GetCtx(), Id, Get_Trx());
                Prospect.SetVAB_BPart_Location_ID(Id);
                Prospect.SetPhone(loc.GetPhone());
                Prospect.SetFax(loc.GetFax());

                VAdvantage.Model.X_VAF_UserContact us = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), UserId, Get_Trx());
                Prospect.SetVAB_Position_ID(us.GetVAB_Position_ID());
                Prospect.SetSubscribeDate(DateTime.Today);
                query = "Select Email from VAF_UserContact where VAF_UserContact_id= " + UserId;
                String mail = Util.GetValueOfString(DB.ExecuteScalar(query));
                Prospect.SetEMail(mail);
                if (!Prospect.Save())
                {
                    return GetRetrievedError(Prospect, "NotSubscribed");
                    //return Msg.GetMsg(GetCtx(), "NotSubscribed");
                    
                }
                lead.SetR_InterestArea_ID(Prospect.GetR_InterestArea_ID());
                if (lead.Save())
                {
                }
                return Msg.GetMsg(GetCtx(), "SubscribedDone");
            }
            else
            {

                VAdvantage.Model.X_vss_lead_interestarea Ilead = new VAdvantage.Model.X_vss_lead_interestarea(GetCtx(), 0, Get_Trx());
                Ilead.SetC_Lead_ID(lead.GetC_Lead_ID());
                Ilead.SetR_InterestArea_ID(R_InterestArea_ID);
                Ilead.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                Ilead.SetBPName(lead.GetBPName());
                Ilead.SetEMail(lead.GetEMail());
                Ilead.SetPhone(lead.GetPhone());
                Ilead.SetFax(lead.GetFax());
                Ilead.SetAddress1(lead.GetAddress1());
                Ilead.SetAddress2(lead.GetAddress2());
                Ilead.SetVAB_City_ID(lead.GetVAB_City_ID());
                Ilead.SetCity(lead.GetCity());
                Ilead.SetC_Region_ID(lead.GetC_Region_ID());
                Ilead.SetRegionName(lead.GetRegionName());
                Ilead.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                Ilead.SetPostal(lead.GetPostal());
                Ilead.SetSubscribeDate(DateTime.Today);
                if (!Ilead.Save())
                {
                    return GetRetrievedError(Ilead, "NotSubscribed");
                    //return Msg.GetMsg(GetCtx(), "NotSubscribed");
                  
                }
                lead.SetR_InterestArea_ID(Ilead.GetR_InterestArea_ID());
                if (lead.Save())
                {
                }
                return Msg.GetMsg(GetCtx(), "SubscribedDone");
                

            }

        
        }




    }
}
