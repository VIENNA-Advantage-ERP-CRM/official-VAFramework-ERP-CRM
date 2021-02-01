using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using ViennaAdvantage.Process;
//////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;


namespace VAdvantage.Process
{
    public class GenerateAccount:SvrProcess
    {
        private int VAB_BusinessPartner_ID = 0;

        protected override void Prepare()
        {
            VAB_BusinessPartner_ID = GetRecord_ID();
        } //prepare

        protected override String DoIt()
        {
            int value = 0;
            VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartner_ID, Get_TrxName());
            //BPartner.SetVAB_Greeting_ID(
            String sqlbp = "update VAB_BusinessPartner set iscustomer='Y', isprospect='N' where VAB_BusinessPartner_id=" + VAB_BusinessPartner_ID + "";
            value = DB.ExecuteQuery(sqlbp, null, Get_TrxName());
            if (value == -1)
            {

            }
            //bp.SetIsCustomer(true);
            //bp.SetIsProspect(false);
          
            return Msg.GetMsg(GetCtx(), "AccountGenerated");
        }
    }
}
