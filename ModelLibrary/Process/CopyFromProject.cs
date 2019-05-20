/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyFromProject
 * Purpose        : Copy Project Details
 * Class Used     : SvrProcess
 * Chronological    Development
 * Deepak           07-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;



namespace VAdvantage.Process
{
    public class CopyFromProject : SvrProcess
    {
     private int		_C_Project_ID = 0;

	/**
	 *  Prepare - e.g., get Parameters.
	 */
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
			else if (name.Equals("C_Project_ID"))
            {
				_C_Project_ID =Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
            }
			else
            {
				log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Perrform Process.
    /// </summary>
	/// <returns>Message (clear text)</returns>
	protected override String DoIt() 
    {
		int To_C_Project_ID = GetRecord_ID();
		log.Info("doIt - From C_Project_ID=" + _C_Project_ID + " to " + To_C_Project_ID);
        if (To_C_Project_ID == 0)
        {
            throw new ArgumentException("Target C_Project_ID == 0");
        }
        if (_C_Project_ID == 0)
        {
            throw new ArgumentException("Source C_Project_ID == 0");
        }
		VAdvantage.Model.MProject from = new VAdvantage.Model.MProject (GetCtx(), _C_Project_ID, Get_Trx());
		VAdvantage.Model.MProject to = new VAdvantage.Model.MProject (GetCtx(), To_C_Project_ID, Get_Trx());
		//
		int no = to.CopyDetailsFrom (from);

		return "@Copied@=" + no;
	}	//	doIt

}	//	CopyFromProject

}
