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
        private int _VAM_DiscountCalculation_ID = 0;

        /// <summary>
        /// Prepare
        /// see VFramwork.Process.SvrProcess#prepare()
        /// </summary>
        protected override void Prepare()
        {
            _VAM_DiscountCalculation_ID = GetRecord_ID();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("VAM_DiscountCalculation_ID=" + _VAM_DiscountCalculation_ID);
            if (_VAM_DiscountCalculation_ID == 0)
            {
                throw new Exception("@VAM_DiscountCalculation_ID@ = 0");
            }
            MVAMDiscountCalculation ds = new MVAMDiscountCalculation(GetCtx(), _VAM_DiscountCalculation_ID, Get_TrxName());
            if (ds.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ VAM_DiscountCalculation_ID=" + _VAM_DiscountCalculation_ID);
            }
            //
            int updated = ds.ReSeq();
            return "@Updated@ #" + updated;
        }
    }
}
