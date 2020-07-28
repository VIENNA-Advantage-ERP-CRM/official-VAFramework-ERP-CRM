/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrderOpen
 * Purpose        : Re-Open Order Process (from Closed to Completed)
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     31-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrderOpen : ProcessEngine.SvrProcess
    {
        //The Order				
        private int _C_Order_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message</returns>
        protected override String DoIt()
        {
            log.Info("Open C_Order_ID=" + _C_Order_ID);
            if (_C_Order_ID == 0)
            {
                throw new Exception("C_Order_ID == 0");
            }
            //
            MOrder order = new MOrder(GetCtx(), _C_Order_ID, Get_TrxName());
            if (MOrder.DOCSTATUS_Closed.Equals(order.GetDocStatus()))
            {
                order.SetDocStatus(MOrder.DOCSTATUS_Completed);
                return order.Save() ? "@OK@" : "@Error@";
            }
            else
            {
                throw new Exception("Order is not closed");
            }
        }
    }
}
