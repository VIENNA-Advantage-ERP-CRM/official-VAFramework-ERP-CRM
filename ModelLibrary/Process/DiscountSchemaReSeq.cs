/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DiscountSchemaReSeq
 * Purpose        : Renumber Discount Schema
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     20-Oct-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DiscountSchemaReSeq : ProcessEngine.SvrProcess
    {
        //Discount Schema			
        private int _M_DiscountSchema_ID = 0;

        /// <summary>
        /// Prepare
        /// see VFramwork.Process.SvrProcess#prepare()
        /// </summary>
        protected override void Prepare()
        {
            _M_DiscountSchema_ID = GetRecord_ID();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("M_DiscountSchema_ID=" + _M_DiscountSchema_ID);
            if (_M_DiscountSchema_ID == 0)
            {
                throw new Exception("@M_DiscountSchema_ID@ = 0");
            }
            MDiscountSchema ds = new MDiscountSchema(GetCtx(), _M_DiscountSchema_ID, Get_TrxName());
            if (ds.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ M_DiscountSchema_ID=" + _M_DiscountSchema_ID);
            }
            //
            int updated = ds.ReSeq();
            return "@Updated@ #" + updated;
        }
    }
}
