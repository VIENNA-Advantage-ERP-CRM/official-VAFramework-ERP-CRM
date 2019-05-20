/********************************************************
 * Module Name    : 
 * Purpose        : Create Periods of year
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological Development
 * Veena Pandey     25-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Model;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    /// <summary>
    /// Create Periods of year
    /// </summary>
    public class YearCreatePeriods : ProcessEngine.SvrProcess
    {
        private int _cYearId = 0;
        private string Month_ID = null;


        /// <summary>
        /// Prepare
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
                else if (name.Equals("Month_ID"))
                    Month_ID = Convert.ToString(para[i].GetParameter());
            }

            _cYearId = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            MYear year = new MYear(GetCtx(), _cYearId, Get_Trx());
            if (_cYearId == 0 || year.Get_ID() != _cYearId)
                throw new Exception("@NotFound@: @C_Year_ID@ - " + _cYearId);
            //log.info(year.ToString());
            //
            if (Month_ID != null)
            {
                if (year.CreateCustomPeriods(Month_ID))
                {
                    return "@Periods Created Successfully@";
                }
            }
            else
            {
                if (year.CreateStdPeriods(null))
                {
                    return "@Periods Created Successfully@";
                }
            }
            return "@Periods Not Created@";
        }
    }
}
