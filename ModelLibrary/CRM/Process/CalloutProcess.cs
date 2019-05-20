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
            int c_order_id = 0;
            int pid = 0;
            VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), c_order_id, null);


            VAdvantage.Model.MOrderLine ol = new VAdvantage.Model.MOrderLine(GetCtx(), 0, null);
            ol.SetC_Order_ID(c_order_id);
            ol.SetM_Product_ID(pid);
            ol.SetAD_Client_ID(ord.GetAD_Client_ID());
            ol.SetAD_Org_ID(ord.GetAD_Org_ID());
            ol.SetQty(1);
            if (!ol.Save())
            {

            }

            return "";
        }

       
    }
}
