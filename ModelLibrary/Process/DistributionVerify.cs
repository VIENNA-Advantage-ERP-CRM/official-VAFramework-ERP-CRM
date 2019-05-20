/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DistributionVerify
 * Purpose        : Verify GL Distribution
 * Class Used     : ProcessEngine.SvrProcess class
 * Chronological    Development
 * Deepak           20-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
   public class DistributionVerify:ProcessEngine.SvrProcess
    {
    /// <summary>
    /// Prepare
	/// </summary>
	protected override void Prepare ()
	{
	}	//	prepare

	/// <summary>
	/// Process
	/// </summary>
	/// <returns>message</returns>
	protected override String DoIt()
    {
		log.Info("doIt - GL_Distribution_ID=" + GetRecord_ID());
		MDistribution distribution = new MDistribution (GetCtx(), GetRecord_ID(), Get_TrxName());
        if (distribution.Get_ID() == 0)
        {
            throw new Exception("Not found GL_Distribution_ID=" + GetRecord_ID());
        }
		String error = distribution.Validate();
		Boolean saved = distribution.Save();
        if (error != null)
        {
            throw new Exception(error);
        }
        if (!saved)
        {
            throw new Exception("@NotSaved@");
        }
		return "@OK@";
	}	//	doIt

}	//	DistributionVerify

}
