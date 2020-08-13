/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : KIndexRerun
 * Purpose        : Reindex all Content
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           05-Feb-2010
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
    public class KIndexRerun:ProcessEngine.SvrProcess
    {
  /**	WebProject Parameter		*/
	private int		_CM_WebProject_ID = 0;

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
			else if (name.Equals("CM_WebProject_ID"))
            {
				_CM_WebProject_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
		// ReIndex Container
		int[] containers = MContainer.GetAllIDs("CM_Container","CM_WebProject_ID=" + _CM_WebProject_ID, Get_TrxName());

        if (containers != null)
        {

            for (int i = 0; i < containers.Length; i++)
            {
                MContainer thisContainer = new MContainer(GetCtx(), containers[i], Get_TrxName());
                thisContainer.ReIndex(false);
            }
        }
		// ReIndex News
		int[] newsChannels = MNewsChannel.GetAllIDs("CM_NewsChannel","CM_WebProject_ID=" + _CM_WebProject_ID, Get_TrxName());

        if (newsChannels != null)
        {
            for (int i = 0; i < newsChannels.Length; i++)
            {
                MNewsChannel thisChannel = new MNewsChannel(GetCtx(), newsChannels[i], Get_TrxName());
                thisChannel.ReIndex(false);
                int[] newsItems = MNewsItem.GetAllIDs("CM_NewsItem", "CM_NewsChannel_ID=" + newsChannels[i], Get_TrxName());
                if (newsItems != null)
                {
                    for (int k = 0; k < newsItems.Length; k++)
                    {
                        MNewsItem thisItem = new MNewsItem(GetCtx(), newsItems[k], Get_TrxName());
                        thisItem.ReIndex(false);
                    }
                }
            }
        }
		return "finished...";
	}	//	doIt

}	//	KIndexRerun

}
