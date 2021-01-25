using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace VAdvantage.Process
{
    class SubscribeCustomerInterestArea : SvrProcess
    {

        int _VAB_BusinessPartner_ID;
        int R_InterestArea_ID;
        protected override void Prepare()
        {
            _VAB_BusinessPartner_ID = GetRecord_ID();
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
            if (!(R_InterestArea_ID > 0))
            {
                return Msg.GetMsg(GetCtx(), "NoInterestAreaIsSelected");
            }
            else
            {
                X_R_ContactInterest customer = new X_R_ContactInterest(GetCtx(), 0, Get_TrxName());
                customer.SetR_InterestArea_ID(R_InterestArea_ID);
                customer.SetVAB_BusinessPartner_ID(_VAB_BusinessPartner_ID);
                String query = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id= " + _VAB_BusinessPartner_ID;
                int UserId = Util.GetValueOfInt(DB.ExecuteScalar(query));
                customer.SetVAF_UserContact_ID(UserId);
                query = "Select VAB_BPart_Location_id from VAB_BPart_Location where VAB_BusinessPartner_id= " + _VAB_BusinessPartner_ID;

                int Id = Util.GetValueOfInt(DB.ExecuteScalar(query));
                VAdvantage.Model.X_VAB_BPart_Location loc = new VAdvantage.Model.X_VAB_BPart_Location(GetCtx(), Id, Get_TrxName());
                customer.SetVAB_BPart_Location_ID(Id);
                customer.SetPhone(loc.GetPhone());
                customer.SetFax(loc.GetFax());

                VAdvantage.Model.X_VAF_UserContact us = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), UserId, Get_TrxName());
                customer.SetC_Job_ID(us.GetC_Job_ID());
                customer.SetSubscribeDate(DateTime.Today);
                query = "Select Email from VAF_UserContact where VAF_UserContact_id= " + UserId;
                String mail = Util.GetValueOfString(DB.ExecuteScalar(query));
                customer.SetEMail(mail);
                if (!customer.Save())
                {
                    return Msg.GetMsg(GetCtx(), "NotSubscribed");

                }
                return Msg.GetMsg(GetCtx(), "SubscribedDone");
            }
        }
    }
}
