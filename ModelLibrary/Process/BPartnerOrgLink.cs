/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BPartnerOrgLink
 * Purpose        : Link Business Partner to Organization,Either to existing or create new one
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           28-Dec-2009
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
    public class BPartnerOrgLink : ProcessEngine.SvrProcess
    {
    /**	Existing Org			*/
	private int			_AD_Org_ID;
	/** Info for New Org		*/
	private int			_AD_OrgType_ID;
	/** Business Partner		*/
	private int			_C_BPartner_ID;
	/** Role					*/
	private int			_AD_Role_ID;
	
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
			else if (name.Equals("AD_Org_ID"))
            {
				_AD_Org_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("AD_OrgType_ID"))
            {
				_AD_OrgType_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("AD_Role_ID"))
            {
				_AD_Role_ID = para[i].GetParameterAsInt();
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
		_C_BPartner_ID = GetRecord_ID();
	}	//	prepare

	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message (text with variables)</returns>
	protected override String DoIt()
	{
		log.Info("C_BPartner_ID=" + _C_BPartner_ID 
			+ ", AD_Org_ID=" + _AD_Org_ID
			+ ", AD_OrgType_ID=" + _AD_OrgType_ID
			+ ", AD_Role_ID=" + _AD_Role_ID);
        if (_C_BPartner_ID == 0)
        {
            throw new Exception("No Business Partner ID");
        }
		MBPartner bp = new MBPartner (GetCtx(), _C_BPartner_ID, Get_Trx());
        if (bp.Get_ID() == 0)
        {
            throw new Exception("Business Partner not found - C_BPartner_ID=" + _C_BPartner_ID);
        }
		//	BP Location
		MBPartnerLocation[] locs = bp.GetLocations(false);
        if (locs == null || locs.Length == 0)
        {
            throw new ArgumentException("Business Partner has no Location");
        }
		//	Location
		int C_Location_ID = locs[0].GetC_Location_ID();
        if (C_Location_ID == 0)
        {
            throw new ArgumentException("Business Partner Location has no Address");
        }
		
		//	Create Org
		Boolean newOrg = _AD_Org_ID == 0; 
		MOrg org = new MOrg (GetCtx(), _AD_Org_ID, Get_Trx());
		if (newOrg)
		{
			org.SetValue (bp.GetValue());
			org.SetName (bp.GetName());
			org.SetDescription (bp.GetDescription());
            if (!org.Save())
            {
                throw new Exception("Organization not saved");
            }
		}
		else	//	check if linked to already
		{
			int C_BPartner_ID = org.GetLinkedC_BPartner_ID();
            if (C_BPartner_ID > 0)
            {
                throw new ArgumentException("Organization '" + org.GetName()
                    + "' already linked (to C_BPartner_ID=" + C_BPartner_ID + ")");
            }
		}
		_AD_Org_ID = org.GetAD_Org_ID();
		
		//	Update Org Info
		MOrgInfo oInfo = org.GetInfo();
		oInfo.SetAD_OrgType_ID (_AD_OrgType_ID);
        if (newOrg)
        {
            oInfo.SetC_Location_ID(C_Location_ID);
        }
		
		//	Create Warehouse
		MWarehouse wh = null;
		if (!newOrg)
		{
			MWarehouse[] whs = MWarehouse.GetForOrg(GetCtx(), _AD_Org_ID);
            if (whs != null && whs.Length > 0)
            {
                wh = whs[0];	//	pick first
            }
		}
		//	New Warehouse
		if (wh == null)
		{
			wh = new MWarehouse(org);
            if (!wh.Save())
            {
                throw new Exception("Warehouse not saved");
            }
		}
		//	Create Locator
		MLocator mLoc = wh.GetDefaultLocator();
		if (mLoc == null)
		{
			mLoc = new MLocator (wh, "Standard");
			mLoc.SetIsDefault(true);
			mLoc.Save();
		}
		
		//	Update/Save Org Info
		oInfo.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
        if (!oInfo.Save(Get_Trx()))
        {
            throw new Exception("Organization Info not saved");
        }
		
		//	Update BPartner
		bp.SetAD_OrgBP_ID(_AD_Org_ID);
        if (bp.GetAD_Org_ID() != 0)
        {
            bp.SetClientOrg(bp.GetAD_Client_ID(), 0);	//	Shared BPartner
        }
		
		//	Save BP
        if (!bp.Save())
        {
            throw new Exception("Business Partner not updated");
        }
		
		//	Limit to specific Role
		if (_AD_Role_ID != 0)	
		{
			Boolean found = false;
			MRoleOrgAccess[] orgAccesses = MRoleOrgAccess.GetOfOrg (GetCtx(), _AD_Org_ID);
			//	delete all accesses except the specific
			for (int i = 0; i < orgAccesses.Length; i++)
			{
                if (orgAccesses[i].GetAD_Role_ID() == _AD_Role_ID)
                {
                    found = true;
                }
                else
                {
                    orgAccesses[i].Delete(true);
                }
			}
			//	create access
			if (!found)
			{
				MRoleOrgAccess orgAccess = new MRoleOrgAccess (org, _AD_Role_ID);
				orgAccess.Save();
			}
		}
		
		//	Reset Client Role
		MRole.GetDefault(GetCtx(), true);
		
		return "Business Partner - Organization Link created";
	}	//	doIt

}	//	BPartnerOrgLink

}
