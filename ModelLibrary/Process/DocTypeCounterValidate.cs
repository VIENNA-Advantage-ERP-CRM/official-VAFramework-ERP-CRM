/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocTypeCounterValidate
 * Purpose        : Validate Counter Document
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           02-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DocTypeCounterValidate : ProcessEngine.SvrProcess
    {
   /**	Counter Document		*/
	private int					_C_DocTypeCounter_ID = 0;
	/**	Document Relationship	*/
	private MDocTypeCounter		_counter = null;
	/// <summary>
	///	Prepare
	/// </summary>
	protected override void Prepare ()
	{
		_C_DocTypeCounter_ID = GetRecord_ID();
	}	//	prepare
    /// <summary>
    /// Do It
	/// </summary>
    /// <returns>message</returns>
	protected override String DoIt()
	{
		log.Info("C_DocTypeCounter_ID=" + _C_DocTypeCounter_ID);
		_counter = new MDocTypeCounter (GetCtx(), _C_DocTypeCounter_ID, Get_TrxName());
        if (_counter == null || _counter.Get_ID() == 0)
        {
            throw new ArgumentException("Not found C_DocTypeCounter_ID=" + _C_DocTypeCounter_ID);
        }
		//
		String error = _counter.Validate();
		_counter.Save();
        if (error != null)
        {
            throw new Exception(error);
        }
		return "OK";
	}	//	doIt
	
}	//	DocTypeCounterValidate
}
