using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
//using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class CalloutProcess : SvrProcess
    {
        protected override void Prepare()
        {

        }

        protected override string DoIt()
        {
            int VAB_Order_id = 0;
            int pid = 0;
            VAdvantage.Model.MVABOrder ord = new VAdvantage.Model.MVABOrder(GetCtx(), VAB_Order_id, null);


            VAdvantage.Model.MVABOrderLine ol = new VAdvantage.Model.MVABOrderLine(GetCtx(), 0, null);
            ol.SetVAB_Order_ID(VAB_Order_id);
            ol.SetM_Product_ID(pid);
            ol.SetVAF_Client_ID(ord.GetVAF_Client_ID());
            ol.SetVAF_Org_ID(ord.GetVAF_Org_ID());
            ol.SetQty(1);
            if (!ol.Save())
            {

            }

            return "";
        }

       
    }
}
