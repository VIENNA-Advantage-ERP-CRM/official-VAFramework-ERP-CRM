/********************************************************
 * Module Name    : 
 * Purpose        : Commission Copy
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Veena     10-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    /// <summary>
    /// Commission Copy
    /// </summary>
    public class CommissionCopy : ProcessEngine.SvrProcess
    {
        /**	From Commission			*/
        private int _C_Commission_ID = 0;
        /** To Commission			*/
        private int _C_CommissionTo_ID = 0;

        /// <summary>
        /// Prapare - e.g., get Parameters.
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
                else if (name.Equals("C_Commission_ID"))
                    _C_Commission_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
            _C_CommissionTo_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process - copy
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
	    {
		    log.Info("doIt - C_Commission_ID=" + _C_Commission_ID + " - copy to " + _C_CommissionTo_ID);
		    MCommission comFrom = new MCommission (GetCtx(), _C_Commission_ID, Get_Trx());
		    if (comFrom.Get_ID() == 0)
			    throw new Exception ("No From Commission");
		    MCommission comTo = new MCommission (GetCtx(), _C_CommissionTo_ID, Get_Trx());
		    if (comTo.Get_ID() == 0)
			    throw new Exception ("No To Commission");
    		
		    //
		    int no = comTo.CopyLinesFrom(comFrom);
		    return "@Copied@: " + no;
	    }
    }
}
