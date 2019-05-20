/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RequestInvoice
 * Purpose        : Re-Open Request
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           19-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
   public class RequestReOpen:ProcessEngine.SvrProcess
    {

   /** Request					*/
	private int	_R_Request_ID = 0;
	
	/// <summary>
	///	Prepare
	/// </summary>
	protected override void Prepare ()
	{
		ProcessInfoParameter[] para = GetParameter();
		for (int i = 0; i < para.Length; i++)
		{
			String name = para[i].GetParameterName();
			if (para[i].GetParameter() == null)
            {
				;
            }
			else if (name.Equals("R_Request_ID"))
            {
				_R_Request_ID = para[i].GetParameterAsInt();
            }
			else
				log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
		}
	}	//	prepare

	/// <summary>
	///	Process It
	/// </summary>
	/// <returns>message</returns>
	protected override String DoIt()
	{
		MRequest request = new MRequest (GetCtx(), _R_Request_ID, Get_TrxName());
		log.Info(request.ToString());
        if (request.Get_ID() == 0)
        {
            throw new Exception("@NoRequsetFound@ " + _R_Request_ID);
        }
		
		request.SetR_Status_ID();	//	set default status
		request.SetProcessed(false);
        if (request.Save() && !request.IsProcessed())
        {
            return "@Created Successfully@";
        }
		return "@Error@";
	}	//	doUt

}	//	RequestReOpen

}
