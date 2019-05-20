/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MatchPODelete
    * Purpose        : Delete PO Match
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Raghunandan     19-Nov-2009
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class MatchPODelete : ProcessEngine.SvrProcess
    {
        //ID				
        private int _M_MatchPO_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            _M_MatchPO_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("M_MatchPO_ID=" + _M_MatchPO_ID);
            MMatchPO po = new MMatchPO(GetCtx(), _M_MatchPO_ID, Get_TrxName());
            if (po.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @M_MatchPO_ID@ " + _M_MatchPO_ID);
            }
            if (po.Delete(true))
            {
                return "@OK@";
            }
            po.Save();
            return "@Error@";
        }
    }
}
