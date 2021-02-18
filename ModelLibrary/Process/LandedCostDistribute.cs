﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LandedCostDistribute1
 * Purpose        : Distribute Landed Costs 
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
    public class LandedCostDistribute : ProcessEngine.SvrProcess
    {
        //Parameter		
        private int _VAB_LCost_ID = 0;
        //LC					
        private MVABLCost _lc = null;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            _VAB_LCost_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            _lc = new MVABLCost(GetCtx(), _VAB_LCost_ID, Get_TrxName());
            log.Info(_lc.ToString());
            if (_lc.Get_ID() == 0)
            {
                throw new Exception("@NotFound@: @VAB_LCost_ID@ - " + _VAB_LCost_ID);
            }

            String error = _lc.AllocateCosts();
            if (error == null || error.Length == 0)
            {
                return "@OK@";
            }
            return error;
        }

    }
}
