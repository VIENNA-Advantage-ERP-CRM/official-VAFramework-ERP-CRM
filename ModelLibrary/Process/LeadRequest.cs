/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LeadRequest
 * Purpose        : Create Request from Lead
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           09-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class LeadRequest : ProcessEngine.SvrProcess
    {
        /** Request Type		*/
        //private int _R_RequestType_ID = 0;
        /** Lead				*/
        private int _C_Lead_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            _C_Lead_ID = GetRecord_ID();
        }   //	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>summary</returns>
        protected override String DoIt()
        {
            log.Info("C_Lead_ID=" + _C_Lead_ID);
            if (_C_Lead_ID == 0)
            {
                throw new Exception("@C_Lead_ID@ ID=0");
            }
            MLead lead = new MLead(GetCtx(), _C_Lead_ID, Get_TrxName());
            if (lead.Get_ID() != _C_Lead_ID)
            {
                throw new Exception("@NotFound@: @C_Lead_ID@ ID=" + _C_Lead_ID);
            }
            //
            String retValue = lead.CreateRequest();
            if (retValue != null)
            {
                throw new SystemException(retValue);
            }
            lead.Save();

            // SOTC Specific changes
            if (Env.IsModuleInstalled("VA047_"))
            {
                if (lead.GetC_BPartner_ID() > 0)
                {
                    MBPartner _cbp = new MBPartner(GetCtx(), lead.GetC_BPartner_ID(), Get_TrxName());
                    _cbp.SetC_Greeting_ID(lead.GetC_Greeting_ID());
                    _cbp.SetDescription(lead.GetDescription());
                    _cbp.SetC_IndustryCode_ID(lead.GetC_IndustryCode_ID());
                    _cbp.SetEMail(lead.GetEMail());
                    _cbp.SetMobile(lead.GetMobile());
                    _cbp.Save();
                }
                if (lead.GetAD_User_ID() > 0)
                {
                    MUser _user = new MUser(GetCtx(), lead.GetAD_User_ID(), Get_TrxName());
                    _user.SetName(Util.GetValueOfString(lead.Get_Value("Name2")));
                    _user.Set_Value("FirstName", Util.GetValueOfString(lead.Get_Value("Name2")));
                    _user.Set_Value("LastName", Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_Greeting_ID(lead.GetC_Greeting_ID());
                    _user.Set_Value("FullName", Util.GetValueOfString(lead.Get_Value("Name2")) + " " + Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                    _user.SetMobile(lead.GetMobile());
                    _user.SetEMail(lead.GetEMail());
                    _user.SetDescription(lead.GetDescription());
                    _user.Save();
                }
            }
            MRequest request = lead.GetRequest();
            //
            return "@R_Request_ID@ " + request.GetDocumentNo();
        }   //	doIt

    }	//	LeadRequest

}
