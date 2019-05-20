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
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class MatchInvDelete : ProcessEngine.SvrProcess
    {
        //ID					
        private int _M_MatchInv_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            _M_MatchInv_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("M_MatchInv_ID=" + _M_MatchInv_ID);
            MMatchInv inv = new MMatchInv(GetCtx(), _M_MatchInv_ID, Get_TrxName());
            if (inv.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @M_MatchInv_ID@ " + _M_MatchInv_ID);
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
