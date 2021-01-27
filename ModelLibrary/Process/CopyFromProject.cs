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
//////using System.Windows.Forms;
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
     private int		_VAB_Project_ID = 0;

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
			else if (name.Equals("VAB_Project_ID"))
            {
				_VAB_Project_ID =Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
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
		int To_VAB_Project_ID = GetRecord_ID();
		log.Info("doIt - From VAB_Project_ID=" + _VAB_Project_ID + " to " + To_VAB_Project_ID);
        if (To_VAB_Project_ID == 0)
        {
            throw new ArgumentException("Target VAB_Project_ID == 0");
        }
        if (_VAB_Project_ID == 0)
        {
            throw new ArgumentException("Source VAB_Project_ID == 0");
        }
		VAdvantage.Model.MProject from = new VAdvantage.Model.MProject (GetCtx(), _VAB_Project_ID, Get_Trx());
		VAdvantage.Model.MProject to = new VAdvantage.Model.MProject (GetCtx(), To_VAB_Project_ID, Get_Trx());
		//
		int no = to.CopyDetailsFrom (from);

		return "@Copied@=" + no;
	}	//	doIt

}	//	CopyFromProject

}
