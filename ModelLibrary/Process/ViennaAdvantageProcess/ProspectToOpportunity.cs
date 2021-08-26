using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class ProspectToOpportunity : SvrProcess
    {
        int _C_Lead_ID;
        protected override void Prepare()
        {

        }

        /// <summary>
        /// Doit
        /// </summary>
        /// <returns>message</returns>
        protected override string DoIt()
        {
            X_C_BPartner partner = new X_C_BPartner(GetCtx(), GetRecord_ID(), null);
            string _BPName = partner.GetName();
            string _retVal = "";
            string _sql = "SELECT C_Lead_ID FROM C_Lead Where BPName='" + _BPName + "'  AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(_sql.ToString());
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    _C_Lead_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Lead_ID"]);
                    X_C_Lead lead = new X_C_Lead(GetCtx(), _C_Lead_ID, Get_TrxName());
                    
                    int Pospect = lead.GetRef_BPartner_ID();
                    if (Pospect != 0)
                    {
                        X_C_Project opp = new X_C_Project(GetCtx(), 0, Get_TrxName());
                        X_C_BPartner bp = new X_C_BPartner(GetCtx(), Pospect, Get_TrxName());
                        opp.SetC_Lead_ID(lead.GetC_Lead_ID());
                        opp.SetC_BPartnerSR_ID(lead.GetRef_BPartner_ID());
                        opp.SetName(bp.GetName());
                        opp.SetDescription(lead.GetDescription());
                        opp.SetSalesRep_ID(lead.GetSalesRep_ID());
                        opp.SetDateContract(DateTime.Today);
                        opp.SetC_Campaign_ID(lead.GetC_Campaign_ID());
                        opp.SetAD_User_ID(lead.GetAD_User_ID());                        
                        opp.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                        opp.SetIsOpportunity(true);
                        if (opp.Save())
                        {
                            lead.SetC_Project_ID(opp.GetC_Project_ID());
                            lead.SetProcessed(true);
                            lead.Save();

                            bp.SetCreateProject("Y");
                            if (bp.Save())
                            {
                            }

                            _retVal = Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");

                            // SOTC Specific work to copy mail from lead to BP
                            if (Env.IsModuleInstalled("VA047_"))
                            {
                                int tableID = PO.Get_Table_ID("C_Lead");
                                if (tableID > 0)
                                {
                                    int[] RecordIDS = MMailAttachment1.GetAllIDs("MailAttachment1", "AD_Table_ID=" + tableID + " AND Record_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                                    if (RecordIDS.Length > 0)
                                    {
                                        MMailAttachment1 hist = null;
                                        MMailAttachment1 Oldhist = null;
                                        for (int j = 0; j < RecordIDS.Length; j++)
                                        {
                                            Oldhist = new MMailAttachment1(GetCtx(), RecordIDS[j], Get_TrxName());
                                            hist = new MMailAttachment1(GetCtx(), 0, Get_TrxName());
                                            Oldhist.CopyTo(hist);
                                            if (bp != null)
                                                hist.SetRecord_ID(bp.GetC_BPartner_ID());
                                            if (tableID > 0)
                                                hist.SetAD_Table_ID(tableID);
                                            if (!hist.Save())
                                                log.SaveError("ERROR:", "Error in Copy Email");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _retVal = Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                        }
                    }
                }

            }
            else if (Env.IsModuleInstalled("VA047_"))
            {
                int val = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_User_ID FROM AD_User WHERE C_BPartner_ID=" + partner.GetC_BPartner_ID(), null, Get_TrxName())); ;
                X_C_Project opp = new X_C_Project(GetCtx(), 0, Get_TrxName());
                MUser user = new MUser(GetCtx(), val, Get_TrxName());
                opp.SetSalesRep_ID(partner.GetSalesRep_ID());
                opp.SetName(partner.GetName());
                opp.SetDescription(partner.GetDescription());
                opp.SetC_BPartnerSR_ID(partner.GetC_BPartner_ID());
                opp.SetDateContract(DateTime.Today);
                opp.SetAD_User_ID(user.GetAD_User_ID());
                opp.SetC_BPartner_Location_ID(user.GetC_BPartner_Location_ID());
                opp.SetC_Campaign_ID(partner.GetC_Campaign_ID());
                opp.SetIsOpportunity(true);
                if (opp.Save())
                {
                    partner.SetCreateProject("Y");
                    if (partner.Save())
                    {
                    }
                    _retVal = Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");
                }
                else
                {
                    _retVal = Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                }
            }
            return _retVal;
        }
    }
}
