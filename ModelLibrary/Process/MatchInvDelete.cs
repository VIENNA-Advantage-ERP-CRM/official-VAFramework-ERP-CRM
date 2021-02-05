/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MatchInvDelete
 * Purpose        : Delete Inv Match LandedCostDistribute 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      11-Dec-2009
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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class MatchInvDelete : ProcessEngine.SvrProcess
    {
        //ID					
        private int _VAM_MatchInvoice_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            _VAM_MatchInvoice_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("VAM_MatchInvoice_ID=" + _VAM_MatchInvoice_ID);
            MMatchInv inv = new MMatchInv(GetCtx(), _VAM_MatchInvoice_ID, Get_TrxName());
            if (inv.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @VAM_MatchInvoice_ID@ " + _VAM_MatchInvoice_ID);
            }
            if (inv.Delete(true))
            {
                return "@OK@";
            }
            inv.Save();
            return "@Error@";
        }

    }
}
