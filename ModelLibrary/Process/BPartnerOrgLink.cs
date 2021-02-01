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
//using System.Windows.Forms;
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
	private int			_VAF_Org_ID;
	/** Info for New Org		*/
	private int			_VAF_OrgCategory_ID;
	/** Business Partner		*/
	private int			_VAB_BusinessPartner_ID;
	/** Role					*/
	private int			_VAF_Role_ID;
	
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
			else if (name.Equals("VAF_Org_ID"))
            {
				_VAF_Org_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("VAF_OrgCategory_ID"))
            {
				_VAF_OrgCategory_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("VAF_Role_ID"))
            {
				_VAF_Role_ID = para[i].GetParameterAsInt();
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
		_VAB_BusinessPartner_ID = GetRecord_ID();
	}	//	prepare

	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message (text with variables)</returns>
	protected override String DoIt()
	{
		log.Info("VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID 
			+ ", VAF_Org_ID=" + _VAF_Org_ID
			+ ", VAF_OrgCategory_ID=" + _VAF_OrgCategory_ID
			+ ", VAF_Role_ID=" + _VAF_Role_ID);
        if (_VAB_BusinessPartner_ID == 0)
        {
            throw new Exception("No Business Partner ID");
        }
		MVABBusinessPartner bp = new MVABBusinessPartner (GetCtx(), _VAB_BusinessPartner_ID, Get_Trx());
        if (bp.Get_ID() == 0)
        {
            throw new Exception("Business Partner not found - VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);
        }
		//	BP Location
		MVABBPartLocation[] locs = bp.GetLocations(false);
        if (locs == null || locs.Length == 0)
        {
            throw new ArgumentException("Business Partner has no Location");
        }
		//	Location
		int VAB_Address_ID = locs[0].GetVAB_Address_ID();
        if (VAB_Address_ID == 0)
        {
            throw new ArgumentException("Business Partner Location has no Address");
        }
		
		//	Create Org
		Boolean newOrg = _VAF_Org_ID == 0; 
		MVAFOrg org = new MVAFOrg (GetCtx(), _VAF_Org_ID, Get_Trx());
		if (newOrg)
		{
			org.SetValue (bp.GetValue());
			org.SetName (bp.GetName());
			org.SetDescription (bp.GetDescription());
            if (!org.Save())
            {
                return GetRetrievedError(org, "Organization not saved");
                //throw new Exception("Organization not saved");
            }
		}
		else	//	check if linked to already
		{
			int VAB_BusinessPartner_ID = org.GetLinkedVAB_BusinessPartner_ID();
            if (VAB_BusinessPartner_ID > 0)
            {
                throw new ArgumentException("Organization '" + org.GetName()
                    + "' already linked (to VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + ")");
            }
		}
		_VAF_Org_ID = org.GetVAF_Org_ID();
		
		//	Update Org Info
		MVAFOrgDetail oInfo = org.GetInfo();
		oInfo.SetVAF_OrgCategory_ID (_VAF_OrgCategory_ID);
        if (newOrg)
        {
            oInfo.SetVAB_Address_ID(VAB_Address_ID);
        }
		
		//	Create Warehouse
		MWarehouse wh = null;
		if (!newOrg)
		{
			MWarehouse[] whs = MWarehouse.GetForOrg(GetCtx(), _VAF_Org_ID);
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
                return GetRetrievedError(wh, "Warehouse not saved");
                //throw new Exception("Warehouse not saved");
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
            return GetRetrievedError(oInfo, "Organization Info not saved");
            //throw new Exception("Organization Info not saved");
        }
		
		//	Update BPartner
		bp.SetVAF_OrgBP_ID(_VAF_Org_ID);
        if (bp.GetVAF_Org_ID() != 0)
        {
            bp.SetClientOrg(bp.GetVAF_Client_ID(), 0);	//	Shared BPartner
        }
		
		//	Save BP
        if (!bp.Save())
        {
            return GetRetrievedError(bp, "Business Partner not updated");
            //throw new Exception("Business Partner not updated");
        }
		
		//	Limit to specific Role
		if (_VAF_Role_ID != 0)	
		{
			Boolean found = false;
			MVAFRoleOrgRights[] orgAccesses = MVAFRoleOrgRights.GetOfOrg (GetCtx(), _VAF_Org_ID);
			//	delete all accesses except the specific
			for (int i = 0; i < orgAccesses.Length; i++)
			{
                if (orgAccesses[i].GetVAF_Role_ID() == _VAF_Role_ID)
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
				MVAFRoleOrgRights orgAccess = new MVAFRoleOrgRights (org, _VAF_Role_ID);
				orgAccess.Save();
			}
		}
		
		//	Reset Client Role
		MVAFRole.GetDefault(GetCtx(), true);
		
		return "Business Partner - Organization Link created";
	}	//	doIt

}	//	BPartnerOrgLink

}
