using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Web.Hosting;
using System.Web;
using System.Security.Cryptography;
using System.Web.Security;
using VAdvantage.Model;
namespace VAdvantage.Process
{
    public class InviteeCopyToSelectedCampaign : SvrProcess
    {
        int _Campaign_ID = 0;
        protected override void Prepare()
        {

            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null && para[i].GetParameter_To() == null)
                {
                    ;
                }
                else if (name.Equals("C_Campaign_ID"))
                {
                    _Campaign_ID = para[i].GetParameterAsInt();
                }
            }
        }
        protected override String DoIt()
        {
            StringBuilder _sql = new StringBuilder();
            X_C_InviteeList invi = null;
            //_sql.Clear();
            _sql.Append(" Select c_inviteelist_ID, Name, email From c_inviteelist where c_campaign_id=" + _Campaign_ID);
            DataSet ds1 = DB.ExecuteDataset(_sql.ToString());



            int _SelectedInvitee = GetRecord_ID();
            _sql.Clear();
            _sql.Append(@"Select C_Campaign_ID FROM C_InviteeList Where C_InviteeList_ID=" + _SelectedInvitee);
            int _SelectedCampaign = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));


            if (_SelectedCampaign != _Campaign_ID)
            {
                //StringBuilder _sql = new StringBuilder();
                _sql.Clear();
                _sql.Append(@"Select * From C_InviteeList ");
                _sql.Append(@" Where IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + " AND AD_Org_ID = " + GetAD_Org_ID());
                if (_Campaign_ID > 0)
                {
                    _sql.Append("  And C_Campaign_ID=" + _SelectedCampaign);
                }
                DataSet ds = new DataSet();
                ds = DB.ExecuteDataset(_sql.ToString(), null, Get_TrxName());
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string _name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                        string _mail = Util.GetValueOfString(ds.Tables[0].Rows[i]["Email"]);


                        _sql.Clear();
                        _sql.Append(@"Select C_InviteeList_ID FROM C_InviteeList Where (Name='" + _name + "' AND Email ='" + _mail + "') And c_campaign_id=" + _Campaign_ID);
                        int _recordID = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));

                        invi = new X_C_InviteeList(Env.GetCtx(), _recordID, null);
                        invi.SetAD_Client_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Client_ID"]));
                        invi.SetAD_Org_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                        invi.SetC_Campaign_ID(_Campaign_ID);
                        invi.SetAD_User_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_User_ID"]));
                        invi.SetName(Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                        invi.SetC_Lead_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Lead_ID"]));
                        invi.SetPhone(Util.GetValueOfString(ds.Tables[0].Rows[i]["Phone"]));
                        invi.SetEMail(Util.GetValueOfString(ds.Tables[0].Rows[i]["Email"]));
                        invi.SetAddress1(Util.GetValueOfString(ds.Tables[0].Rows[i]["Address1"]));
                        invi.SetAddress2(Util.GetValueOfString(ds.Tables[0].Rows[i]["Address2"]));
                        invi.SetC_Location_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Location_ID"]));
                        invi.SetCity(Util.GetValueOfString(ds.Tables[0].Rows[i]["City"]));
                        invi.SetC_City_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_City_ID"]));
                        invi.SetRegionName(Util.GetValueOfString(ds.Tables[0].Rows[i]["RegionName"]));
                        invi.SetC_Region_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Region_ID"]));
                        invi.SetC_Country_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Country_ID"]));
                        invi.SetPostal(Util.GetValueOfString(ds.Tables[0].Rows[i]["Postal"]));
                        invi.SetInviteeResponse(Util.GetValueOfString(ds.Tables[0].Rows[i]["InviteeResponse"]));
                        invi.SetRemark(Util.GetValueOfString(ds.Tables[0].Rows[i]["Remark"]));
                        invi.SetResponseDate(Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["ResponseDate"]));
                        invi.SetURL(Util.GetValueOfString(ds.Tables[0].Rows[i]["URL"]));
                        //invi.setIsPurchased(Util.GetValueOfBool(ds.Tables[0].Rows[0]["IsPurchased"]));
                        if (!invi.Save())
                        {
                            return GetRetrievedError(invi, "DatanotSaved");
                            //return Msg.GetMsg(GetCtx(), "DatanotSaved");
                            //return " DatanotSaved ";

                        }

                    }
                    
                }
                 
            }
            else
            {
               return Msg.GetMsg(GetCtx(), "PleaseSelectAnotherCampaign");
                
            }

            return Msg.GetMsg(GetCtx(), "AllinviteeSaved");
        }
      
    }
}
