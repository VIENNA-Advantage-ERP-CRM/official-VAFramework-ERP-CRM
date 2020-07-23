/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : SystemValidate
 * Purpose        : Validate Support
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-feb-2010
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class SystemValidate:ProcessEngine.SvrProcess
    {
    /// <summary>
    ///	Prepare
    /// </summary>
	protected override void Prepare ()
	{
	}	//	prepare

	/// <summary>
	/// process
	/// </summary>
	/// <returns>Message</returns>
	protected override String DoIt()
	{
		return "";
	}	
    }
}	

