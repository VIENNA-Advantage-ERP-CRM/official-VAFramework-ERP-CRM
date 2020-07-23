/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyFromInvoice
 * Purpose        : Copy Invoice Lines
 * Class Used     : SvrProcess
 * Chronological    Development
 * Deepak           10-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
////using System.Windows.Forms;
////using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
  public class CopyFromInvoice:SvrProcess
    {
   private int		_C_Invoice_ID = 0;

	/// <summary>
	/// Prepare - e.g., get Parameters.
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
            else if (name.Equals("C_Invoice_ID"))
            {
               // _C_Invoice_ID = ((Decimal)para[i].GetParameter()).intValue();
                _C_Invoice_ID =Util.GetValueOfInt(((Decimal?)para[i].GetParameter()));
            }
            else
            {
                log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message </returns>
	protected override String DoIt() 
	{
		int To_C_Invoice_ID = GetRecord_ID();
		log.Info("From C_Invoice_ID=" + _C_Invoice_ID + " to " + To_C_Invoice_ID);
        if (To_C_Invoice_ID == 0)
        {
            throw new Exception("Target C_Invoice_ID == 0");
        }
        if (_C_Invoice_ID == 0)
        {
            throw new Exception("Source C_Invoice_ID == 0");
        }
		VAdvantage.Model.MInvoice from = new VAdvantage.Model.MInvoice (GetCtx(), _C_Invoice_ID, null);
		VAdvantage.Model.MInvoice to = new VAdvantage.Model.MInvoice (GetCtx(), To_C_Invoice_ID, null);
		//
		int no = to.CopyLinesFrom (from, false, false);
		//
		return "@Copied@=" + no;
	}	//	doIt

}	//	CopyFromInvoice

}
