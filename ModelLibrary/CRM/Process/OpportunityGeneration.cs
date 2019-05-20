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
        private int _C_Campaign_ID = 0;
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
                else if (name.Equals("C_Campaign_ID"))
                {
                    _C_Campaign_ID = Util.GetValueOfInt(para[i].GetParameter());
                    
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
            int c_bpartner_id = 0;
            int c_bpartnerSR_id = 0;

            String sql = "select bp.iscustomer,bp.isprospect,bp.name,bp.c_bpartner_id,au.ad_user_id, bpl.c_bpartner_location_id"
                           + " from c_bpartner bp left join c_bpartner_location bpl on(bpl.c_bpartner_id= bp.c_bpartner_id)"
                           + " left JOIN ad_user au on(au.c_bpartner_id= bp.c_bpartner_id) where bp.c_bpartner_id=" + "1001892" + " and bp.ad_client_id=" + GetCtx().GetAD_Client_ID();
            IDataReader idr = null;
            idr = DB.ExecuteReader(sql);
            if (idr.Read())
            {
                Customer= Util.GetValueOfInt(idr["C_BPartner_ID"]);
                opportunity = Util.GetValueOfString(idr["Name"]);
                User = Util.GetValueOfInt(idr["AD_User_ID"]);
                Location = Util.GetValueOfInt(idr["C_BPartner_Location_ID"]);
                startDate = Util.GetValueOfDateTime(System.DateTime.Now);
                if (Util.GetValueOfString(idr["IsCustomer"]) == "Y")
                {
                    c_bpartner_id = Util.GetValueOfInt(idr["C_BPartner_ID"]);
                }
                else if (Util.GetValueOfString(idr["IsProspect"]) == "Y")
                {
                    c_bpartnerSR_id = Util.GetValueOfInt(idr["C_BPartner_ID"]);
                }
                sql = "select mp.m_pricelist_id,cc.c_currency_id from m_pricelist_version mpv join m_pricelist mp on(mp.m_pricelist_id=mpv.m_pricelist_id) join c_currency cc on(cc.c_currency_id= mp.c_currency_id) where mpv.m_pricelist_version_id=" + _M_PriceList_Version_ID + " and mp.ad_client_id=" + GetCtx().GetAD_Client_ID();
                IDataReader idr1 = null;
                idr1 = DB.ExecuteReader(sql);
                if (idr1.Read())
                {
                    Pricelist = Util.GetValueOfInt(idr1["M_PriceList_ID"]);
                    Currency = Util.GetValueOfInt(idr1["C_Currency_ID"]);
                }
                idr1.Close();

                int Owner = Util.GetValueOfInt(GetCtx().GetAD_User_ID());
                //Util.GetValueOfString(GetCtx().GetAD_User_ID());

                VAdvantage.Model.X_C_Project opp = new VAdvantage.Model.X_C_Project(GetCtx(), 0, Get_TrxName());
                opp.SetIsOpportunity(true);
                opp.SetName(opportunity);
                opp.SetC_BPartner_ID(c_bpartner_id);
                opp.SetC_BPartnerSR_ID(c_bpartnerSR_id);
                opp.SetAD_User_ID(User);
                opp.SetSalesRep_ID(Owner);
                opp.SetDateContract(startDate.Value.Date);
                opp.SetC_BPartner_Location_ID(Location);
                opp.SetC_Campaign_ID(_C_Campaign_ID);
                opp.SetProbability(_Probability);
                opp.SetM_PriceList_Version_ID(_M_PriceList_Version_ID);
                opp.SetC_Currency_ID(Currency);
                opp.SetM_PriceList_ID(Pricelist);
                opp.Save();
            }
            idr.Close();


            
            return "Opportunity Generated";
        }
    }
}
