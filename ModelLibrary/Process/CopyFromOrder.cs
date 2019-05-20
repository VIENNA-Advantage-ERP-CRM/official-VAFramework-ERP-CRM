/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyFromOrder
 * Purpose        : Copy Order Lines
 * Class Used     : SvrProcess
 * Chronological    Development
 * Karan            21-May-2011
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class CopyFromOrder:SvrProcess
    {
    /**	The Order				*/
	private int		_C_Order_ID = 0;

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
			else if (name.Equals("C_Order_ID") || name.Equals("RefOrder"))
            {
				_C_Order_ID =Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
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
	/// <returns>Message (clear text)</returns>
	protected override String DoIt()
	{
		int To_C_Order_ID = GetRecord_ID();
		log.Info("From C_Order_ID=" + _C_Order_ID + " to " + To_C_Order_ID);
        if (To_C_Order_ID == 0)
        { 
            throw new ArgumentException("Target C_Order_ID == 0");
        }
        if (_C_Order_ID == 0)
        {
            throw new ArgumentException("Source C_Order_ID == 0");
        }
		MOrder from = new MOrder (GetCtx(), _C_Order_ID, Get_Trx());
		MOrder to = new MOrder (GetCtx(), To_C_Order_ID, Get_Trx());
		//
		int no = to.CopyLinesFrom (from, false, false);		//	no Attributes
		//
		return "@Copied@=" + no;
	}	//	doIt

}	//	CopyFromOrder

}
