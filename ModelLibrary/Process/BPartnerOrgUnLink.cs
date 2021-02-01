/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BPartnerOrgUnLink
 * Purpose        : UnLink Business Partner from Organization
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
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BPartnerOrgUnLink : ProcessEngine.SvrProcess
    {
   /** Business Partner		*/
	private int	_VAB_BusinessPartner_ID;
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
			else if (name.Equals("VAB_BusinessPartner_ID"))
            {
				_VAB_BusinessPartner_ID =Utility.Util.GetValueOfInt((Utility.Util.GetValueOfDecimal(para[i].GetParameter())));
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message (text with variables)</returns>
	protected override String DoIt()
	{
		log.Info("doIt - VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);
        if (_VAB_BusinessPartner_ID == 0)
        {
            throw new ArgumentException("No Business Partner ID");
        }
		MVABBusinessPartner bp = new MVABBusinessPartner (GetCtx(), _VAB_BusinessPartner_ID, Get_Trx());
        if (bp.Get_ID() == 0)
        {
            throw new ArgumentException("Business Partner not found - VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);
        }
		//
        if (bp.GetVAF_OrgBP_ID_Int() == 0)
        {
            throw new ArgumentException("Business Partner not linked to an Organization");
        }
		bp.SetVAF_OrgBP_ID(null);
        if (!bp.Save())
        {
            return GetRetrievedError(bp, "Business Partner not changed");
            //throw new ArgumentException("Business Partner not changed");
        }
		
		return "OK";
	}	//	doIt
	
}	//	BPartnerOrgUnLink

}
