/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PaymentTermValidate
 * Purpose        : Validate Payment Term and Schedule
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
    public class PaymentTermValidate : ProcessEngine.SvrProcess
    {
    /// <summary>
    ///  Prepare - e.g., get Parameters.
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
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Perrform Process.
	/// </summary>
	/// <returns> Message</returns>
	protected override String DoIt() 
	{
		log.Info ("C_PaymentTerm_ID=" + GetRecord_ID());
		MPaymentTerm pt = new MPaymentTerm (GetCtx(), GetRecord_ID(), Get_TrxName());
		String msg = pt.Validate();
		pt.Save();
		//
        if ("@OK@".Equals(msg))
        {
            return msg;
        }
		throw new Exception (msg);
	}	//	doIt

}	//	PaymentTermValidate

}
