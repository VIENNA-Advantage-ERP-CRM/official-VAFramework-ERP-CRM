using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
using System.Data;
//using ViennaAdvantage.Model;

namespace VAdvantage.Process
{
    public class OpportunityGeneration : SvrProcess
    {
        private int _VAB_Promotion_ID = 0;
        private int _M_PriceList_Version_ID = 0;
        private int _Probability = 0;

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
                else if (name.Equals("VAB_Promotion_ID"))
                {
                    _VAB_Promotion_ID = Util.GetValueOfInt(para[i].GetParameter());
                    
                }
                else if (name.Equals("M_PriceList_Version_ID"))
                {
                    _M_PriceList_Version_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("Probability"))
                {
                    _Probability = Util.GetValueOfInt(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            String opportunity = "";
            int Customer = 0;
            int User = 0;
            int Location = 0;
            DateTime? startDate=null;
            int Pricelist = 0;
            int Currency = 0;
            int VAB_BusinessPartner_id = 0;
            int VAB_BusinessPartnerSR_id = 0;

            String sql = "select bp.iscustomer,bp.isprospect,bp.name,bp.VAB_BusinessPartner_id,au.VAF_UserContact_id, bpl.VAB_BPart_Location_id"
                           + " from VAB_BusinessPartner bp left join VAB_BPart_Location bpl on(bpl.VAB_BusinessPartner_id= bp.VAB_BusinessPartner_id)"
                           + " left JOIN VAF_UserContact au on(au.VAB_BusinessPartner_id= bp.VAB_BusinessPartner_id) where bp.VAB_BusinessPartner_id=" + "1001892" + " and bp.vaf_client_id=" + GetCtx().GetVAF_Client_ID();
            IDataReader idr = null;
            idr = DB.ExecuteReader(sql);
            if (idr.Read())
            {
                Customer= Util.GetValueOfInt(idr["VAB_BusinessPartner_ID"]);
                opportunity = Util.GetValueOfString(idr["Name"]);
                User = Util.GetValueOfInt(idr["VAF_UserContact_ID"]);
                Location = Util.GetValueOfInt(idr["VAB_BPart_Location_ID"]);
                startDate = Util.GetValueOfDateTime(System.DateTime.Now);
                if (Util.GetValueOfString(idr["IsCustomer"]) == "Y")
                {
                    VAB_BusinessPartner_id = Util.GetValueOfInt(idr["VAB_BusinessPartner_ID"]);
                }
                else if (Util.GetValueOfString(idr["IsProspect"]) == "Y")
                {
                    VAB_BusinessPartnerSR_id = Util.GetValueOfInt(idr["VAB_BusinessPartner_ID"]);
                }
                sql = "select mp.m_pricelist_id,cc.VAB_Currency_id from m_pricelist_version mpv join m_pricelist mp on(mp.m_pricelist_id=mpv.m_pricelist_id) join VAB_Currency cc on(cc.VAB_Currency_id= mp.VAB_Currency_id) where mpv.m_pricelist_version_id=" + _M_PriceList_Version_ID + " and mp.vaf_client_id=" + GetCtx().GetVAF_Client_ID();
                IDataReader idr1 = null;
                idr1 = DB.ExecuteReader(sql);
                if (idr1.Read())
                {
                    Pricelist = Util.GetValueOfInt(idr1["M_PriceList_ID"]);
                    Currency = Util.GetValueOfInt(idr1["VAB_Currency_ID"]);
                }
                idr1.Close();

                int Owner = Util.GetValueOfInt(GetCtx().GetVAF_UserContact_ID());
                //Util.GetValueOfString(GetCtx().GetVAF_UserContact_ID());

                VAdvantage.Model.X_VAB_Project opp = new VAdvantage.Model.X_VAB_Project(GetCtx(), 0, Get_TrxName());
                opp.SetIsOpportunity(true);
                opp.SetName(opportunity);
                opp.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_id);
                opp.SetVAB_BusinessPartnerSR_ID(VAB_BusinessPartnerSR_id);
                opp.SetVAF_UserContact_ID(User);
                opp.SetSalesRep_ID(Owner);
                opp.SetDateContract(startDate.Value.Date);
                opp.SetVAB_BPart_Location_ID(Location);
                opp.SetVAB_Promotion_ID(_VAB_Promotion_ID);
                opp.SetProbability(_Probability);
                opp.SetM_PriceList_Version_ID(_M_PriceList_Version_ID);
                opp.SetVAB_Currency_ID(Currency);
                opp.SetM_PriceList_ID(Pricelist);
                opp.Save();
            }
            idr.Close();


            
            return "Opportunity Generated";
        }
    }
}
