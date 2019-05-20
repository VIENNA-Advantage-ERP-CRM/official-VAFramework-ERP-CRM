/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ProjectClose
 * Purpose        : Close Project
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           07-Dec-2009
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
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ProjectClose:ProcessEngine.SvrProcess
    {
    /**	Project from Record			*/
	private int 		m_C_Project_ID = 0;

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
			else
            {
				log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
		}
		m_C_Project_ID = GetRecord_ID();
	}	//	prepare

	/// <summary>
	/// Perrform Process.
    /// </summary>
	/// <returns>Message (translated text)</returns>
	protected override String DoIt()
	{
		MProject project = new MProject (GetCtx(), m_C_Project_ID, Get_TrxName());
		log.Info("doIt - " + project);

		MProjectLine[] projectLines = project.GetLines();
		if (MProject.PROJECTCATEGORY_WorkOrderJob.Equals(project.GetProjectCategory())
			|| MProject.PROJECTCATEGORY_AssetProject.Equals(project.GetProjectCategory()))
		{
			/** @todo Check if we should close it */
		}

		//	Close lines
		for (int line = 0; line < projectLines.Length; line++)
		{
			projectLines[line].SetProcessed(true);
			projectLines[line].Save();
		}

		project.SetProcessed(true);
		project.Save();

		return "";
	}	//	doIt

}	//	ProjectClose

}
