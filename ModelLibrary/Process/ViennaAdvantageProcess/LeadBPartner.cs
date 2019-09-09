﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LeadBPartner
 * Purpose        : Create BP Contact, Account, Location
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

//namespace VAdvantage.Process
namespace ViennaAdvantage.Process
{
    public class LeadBPartner : VAdvantage.ProcessEngine.SvrProcess
    {
        /** Lead				*/
        private int _C_Lead_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            if (para.Length > 0)
            {
                foreach (ProcessInfoParameter element in para)
                {
                    String name = element.GetParameterName();
                    if (name.Equals("_C_Lead_ID"))
                    {
                        _C_Lead_ID = element.GetParameterAsInt();
                    }
                }
            }
            else
            {
                _C_Lead_ID = GetRecord_ID();
            }
            //_C_Lead_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Create BPartner
        /// </summary>
        /// <returns>BPartner</returns>
        protected override String DoIt()
        {
            log.Info("C_Lead_ID=" + _C_Lead_ID);
            if (_C_Lead_ID == 0)
            {
                throw new Exception("@C_Lead_ID@ ID=0");
            }
            MLead lead = new MLead(GetCtx(), _C_Lead_ID, Get_TrxName());
            if (lead.GetC_BP_Group_ID() == 0)
            {
                return Msg.GetMsg(GetCtx(), "SelectBPGroup");
            }
            if (lead.GetBPName() == null)
            {
                return Msg.GetMsg(GetCtx(), "Please enter Company name, Prospect or Business Partner");
            }
            if (lead.Get_ID() != _C_Lead_ID)
            {
                throw new Exception("@NotFound@: @C_Lead_ID@ ID=" + _C_Lead_ID);
            }
            //
            String retValue = lead.CreateBP();
            if (retValue != null)
            {
                return GetRetrievedError(lead, retValue);
                //throw new SystemException(retValue);
            }
            lead.Save();
            //
            if (lead.GetRef_BPartner_ID() != 0)
            {
                return Msg.GetMsg(GetCtx(), "Prospect generated successfully");
            }
            else
                return Msg.GetMsg(GetCtx(), "Prospect not generated ");
            //MBPartner bp = lead.GetBPartner();
            //if (bp != null)
            //{
            //    return "@C_BPartner_ID@: " + bp.GetName();
            //}
            //MUser user = lead.GetUser();
            //if (user != null)
            //{
            //    return "@AD_User_ID@: " + user.GetName();
            //}
            //return "@SaveError@";
        }	//	doIt

    }

}
