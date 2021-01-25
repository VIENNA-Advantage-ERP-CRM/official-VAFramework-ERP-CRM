using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using System.Web.Hosting;
using System.Web;
using System.Security.Cryptography;
using System.Web.Security;



/* Process: Generate Invitee List 
 * Writer :Arpit Singh
 * Date   : 20/1/12 
 */
namespace VAdvantage.Process
{

    class CreateInviteeList : SvrProcess
    {
        
        //int Record_ID;
        string url = "";
        protected override void Prepare()
        {
            url = GetCtx().GetContext("#ApplicationURL");
            url = url.ToLower();

            url = url.Replace("http://", "");
            url = url.Replace("https://", ""); 

            //if (url.Contains("https://"))
            //{

            //}
            if (url.Contains("/viennaadvantage.aspx"))
            {
                url = url.Substring(0, url.LastIndexOf("/")).ToString() + "/CampaignInvitee.aspx";
            }
            else
            {
                url = url + "/CampaignInvitee.aspx";
            }

            object o = System.Configuration.ConfigurationManager.AppSettings["IsSSLEnabled"];

            if (o != null && o.ToString() == "Y")
            {
                url = "https://" + url;
            }
            else
            {
                url = "http://" + url;
            }

            
        }

        protected override String DoIt()
        {
            string query = "Select VAB_PromotionTargetList_id from VAB_PromotionTargetList where VAB_Promotion_id=" + GetRecord_ID() + " and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
            IDataReader MainDr = DB.ExecuteReader(query, null, Get_Trx());
            query = "Delete From C_InviteeList  where VAB_Promotion_id=" + GetRecord_ID();
            int value = DB.ExecuteQuery(query);
            while (MainDr.Read())
            {
                query = "Delete From C_InviteeList where   ";

                int id = Util.GetValueOfInt(MainDr[0]);
                VAdvantage.Model.X_VAB_PromotionTargetList MCapTarget = new VAdvantage.Model.X_VAB_PromotionTargetList(GetCtx(), id, null);

                if (MCapTarget.GetC_MasterTargetList_ID() != 0)
                {
                    query = "Select VAB_BusinessPartner_ID from C_TargetList where C_MasterTargetList_ID=" + MCapTarget.GetC_MasterTargetList_ID() + " and VAB_BusinessPartner_ID is not null";
                    IDataReader dr = DB.ExecuteReader(query, null, Get_Trx());
                    while (dr.Read())
                    {
                        invitee(Util.GetValueOfInt(dr[0]));

                    }
                    dr.Close();

                    query = "Select Ref_BPartner_ID from C_TargetList where C_MasterTargetList_ID=" + MCapTarget.GetC_MasterTargetList_ID() + " and Ref_BPartner_ID is not null";
                    dr = DB.ExecuteReader(query, null, Get_Trx());
                    while (dr.Read())
                    {
                        invitee(Util.GetValueOfInt(dr[0]));

                    }
                    dr.Close();

                    query = "Select C_Lead_ID from C_TargetList where C_MasterTargetList_ID=" + MCapTarget.GetC_MasterTargetList_ID() + " and C_Lead_ID is not null";
                    dr = DB.ExecuteReader(query, null, Get_Trx());
                    while (dr.Read())
                    {
                        string sql = "Select C_InviteeList_id from C_InviteeList where C_Lead_id=" + Util.GetValueOfInt(dr[0]);
                        object Leadid = DB.ExecuteScalar(sql, null, Get_Trx());
                        if (Util.GetValueOfInt(Leadid) == 0)
                        {
                            VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), Util.GetValueOfInt(dr[0]), Get_Trx());
                            if (lead.GetVAB_BusinessPartner_ID() != 0)
                            {
                                invitee(lead.GetVAB_BusinessPartner_ID());

                            }
                            else if (lead.GetRef_BPartner_ID() != 0)
                            {
                                invitee(lead.GetRef_BPartner_ID());
                            }

                            else if (lead.GetContactName() != null)
                            {

                                VAdvantage.Model.X_C_InviteeList Invt = new VAdvantage.Model.X_C_InviteeList(GetCtx(), 0, Get_Trx());
                                //Invt.SetC_TargetList_ID(Util.GetValueOfInt(dr[0]));
                                Invt.SetVAB_Promotion_ID(GetRecord_ID());

                                Invt.SetName(lead.GetContactName());
                                Invt.SetEMail(lead.GetEMail());
                                Invt.SetPhone(lead.GetPhone());
                                Invt.SetC_Lead_ID(lead.GetC_Lead_ID());
                                Invt.SetAddress1(lead.GetAddress1());
                                Invt.SetAddress1(lead.GetAddress2());
                                Invt.SetVAB_City_ID(lead.GetVAB_City_ID());
                                Invt.SetCity(lead.GetCity());
                                Invt.SetC_Region_ID(lead.GetC_Region_ID());
                                Invt.SetRegionName(lead.GetRegionName());
                                Invt.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                                Invt.SetPostal(lead.GetPostal());
                                // Invt.SetURL(url);
                                if (!Invt.Save())
                                {
                                    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                }

                                string ID = Invt.GetC_InviteeList_ID().ToString();
                                string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
                                string urlFinal = "";
                                urlFinal = url + "?" + encrypt;
                                sql = "update c_inviteelist set url = '" + urlFinal + "' where c_inviteelist_id = " + Invt.GetC_InviteeList_ID();
                                int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));

                                //Random rand = new Random();
                                //String s = "";
                                //for (int i = 0; i < 9; i++)
                                //    s = String.Concat(s, rand.Next(10).ToString());
                                //string urlFinal = "";
                                //// urlFinal = url + "?" + Invt.GetC_InviteeList_ID().ToString();
                                //urlFinal = url + "?" + s;
                                ////string urlFinal = "";
                                ////urlFinal = url + "?" + Invt.GetC_InviteeList_ID().ToString();
                                //Invt.SetURL(urlFinal);
                                //if (!Invt.Save(Get_Trx()))
                                //{
                                //    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                //}

                            }
                        }

                    }
                    dr.Close();
                }

                if (MCapTarget.GetR_InterestArea_ID() != 0)
                {

                    query = "Select VAB_BusinessPartner_ID from R_ContactInterest where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID() + " and VAB_BusinessPartner_ID is not null";
                    IDataReader dr = DB.ExecuteReader(query, null, Get_Trx());
                    while (dr.Read())
                    {
                        invitee(Util.GetValueOfInt(dr[0]));

                    }
                    dr.Close();

                    //query = "Select VAB_BusinessPartner_ID from C_TargetList where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID();
                    //dr = DB.ExecuteReader(query);
                    //while (dr.Read())
                    //{
                    //    invitee(Util.GetValueOfInt(dr[0]));

                    //}
                    //dr.Close();

                    query = "Select C_Lead_ID from vss_lead_interestarea where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID() + " and C_Lead_ID is not null";
                    dr = DB.ExecuteReader(query, null, Get_Trx());
                    while (dr.Read())
                    {

                        string sql = "Select C_InviteeList_id from C_InviteeList where C_Lead_id=" + Util.GetValueOfInt(dr[0]);
                        object Leadid = DB.ExecuteScalar(sql, null, Get_Trx());
                        if (Util.GetValueOfInt(Leadid) == 0)
                        {

                            VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), Util.GetValueOfInt(dr[0]), Get_Trx());
                            if (lead.GetVAB_BusinessPartner_ID() != 0)
                            {
                                invitee(lead.GetVAB_BusinessPartner_ID());

                            }
                            else if (lead.GetRef_BPartner_ID() != 0)
                            {
                                invitee(lead.GetRef_BPartner_ID());
                            }

                            else if (lead.GetContactName() != null)
                            {

                                VAdvantage.Model.X_C_InviteeList Invt = new VAdvantage.Model.X_C_InviteeList(GetCtx(), 0, Get_Trx());
                                //Invt.SetC_TargetList_ID(Util.GetValueOfInt(dr[0]));
                                Invt.SetVAB_Promotion_ID(GetRecord_ID());
                                Invt.SetName(lead.GetContactName());
                                Invt.SetEMail(lead.GetEMail());
                                Invt.SetPhone(lead.GetPhone());
                                Invt.SetC_Lead_ID(lead.GetC_Lead_ID());
                                Invt.SetAddress1(lead.GetAddress1());
                                Invt.SetAddress1(lead.GetAddress2());
                                Invt.SetVAB_City_ID(lead.GetVAB_City_ID());
                                Invt.SetCity(lead.GetCity());
                                Invt.SetC_Region_ID(lead.GetC_Region_ID());
                                Invt.SetRegionName(lead.GetRegionName());
                                Invt.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                                Invt.SetPostal(lead.GetPostal());
                                //Invt.SetURL(url);
                                if (!Invt.Save(Get_Trx()))
                                {
                                    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                }

                                string ID = Invt.GetC_InviteeList_ID().ToString();
                                string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
                                string urlFinal = "";
                                urlFinal = url + "?" + encrypt;
                                sql = "update c_inviteelist set url = '" + urlFinal + "' where c_inviteelist_id = " + Invt.GetC_InviteeList_ID();
                                int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));


                                //Random rand = new Random();
                                //String s = "";
                                //for (int i = 0; i < 9; i++)
                                //    s = String.Concat(s, rand.Next(10).ToString());
                                //string urlFinal = "";
                                //urlFinal = url + "?" + s;
                                ////string urlFinal = "";
                                ////urlFinal = url + "?" + Invt.GetC_InviteeList_ID().ToString();
                                //Invt.SetURL(urlFinal);
                                //if (!Invt.Save(Get_Trx()))
                                //{
                                //    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                //}


                            }
                        }

                    }
                    dr.Close();
                }
            }
            MainDr.Close();
            return Msg.GetMsg(GetCtx(), "InviteeCteationDone");
        }

        //String query1 = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id=1001864";
        //int AD_Id = Util.GetValueOfInt(DB.ExecuteScalar(query1));
        //X_VAF_UserContact user = new X_VAF_UserContact(GetCtx(), AD_Id, null);
        //return "";




        // query = "Select C_TargetList_id from C_TargetList where C_TargetList_id not in (Select C_TargetList_id from C_InviteeList) and VAB_Promotion_id="+Record_ID;
        //IDataReader dr = DB.ExecuteReader(query);
        //while (dr.Read())
        //{
        //     X_C_TargetList TList = new X_C_TargetList(GetCtx(), Util.GetValueOfInt(dr[0]), Get_Trx());

        //     if (TList.GetVAB_BusinessPartner_ID()!=0)
        //     {
        //         invitee(Util.GetValueOfInt(dr[0]));



        //     }

        //     if (TList.GetRef_BPartner_ID() !=0)
        //     {
        //         invitee( Util.GetValueOfInt(dr[0]));
        //     }
        //     if (TList.GetC_Lead_ID() != 0)
        //     {
        //         X_C_Lead lead = new X_C_Lead(GetCtx(), TList.GetC_Lead_ID(), Get_Trx());
        //         if (lead.GetVAB_BusinessPartner_ID() != 0)
        //         {
        //             invitee(lead.GetVAB_BusinessPartner_ID());
        //         }
        //         else if (lead.GetRef_BPartner_ID() !=0)
        //         {
        //             invitee( Util.GetValueOfInt(dr[0]));
        //         }
        //         else if (lead.GetContactName() !=null)
        //         {
        //             X_C_InviteeList Invt = new X_C_InviteeList(GetCtx(), 0, Get_Trx());
        //             Invt.SetC_TargetList_ID(Util.GetValueOfInt(dr[0]));
        //             Invt.SetName(lead.GetContactName());
        //             Invt.SetEMail(lead.GetEMail());
        //             Invt.SetPhone(lead.GetPhone());
        //             Invt.SetC_Lead_ID(lead.GetC_Lead_ID());
        //             if (!Invt.Save())
        //             {
        //             }
        //         }
        //     }
        //}
        //return "";

        public void invitee(int bpid)
        {
            VAdvantage.Model.X_VAB_BusinessPartner bp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), bpid, Get_Trx());
            String query = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id=" + bpid;
            int AD_Id = Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx()));
            string sql = "Select C_InviteeList_id from C_InviteeList where VAF_UserContact_id=" + AD_Id + " and VAB_Promotion_id=" + GetRecord_ID();
            object id = DB.ExecuteScalar(sql, null, Get_Trx());
            VAdvantage.Model.X_C_InviteeList Invt;
            if (Util.GetValueOfInt(id) != 0)
            {

                Invt = new VAdvantage.Model.X_C_InviteeList(GetCtx(), Util.GetValueOfInt(id), Get_Trx());

            }
            else
            {
                Invt = new VAdvantage.Model.X_C_InviteeList(GetCtx(), 0, Get_Trx());
                Invt.SetVAF_UserContact_ID(AD_Id);
            }
            VAdvantage.Model.X_VAF_UserContact user = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), AD_Id, Get_Trx());
            int BpLoc = user.GetVAB_BPart_Location_ID();
            if (BpLoc != 0)
            {
                String Sql = "Select C_Location_ID From VAB_BPart_Location where VAB_BPart_Location_id=" + BpLoc;
                Invt.SetC_Location_ID(Util.GetValueOfInt(DB.ExecuteScalar(Sql)));
            }
            Invt.SetVAB_Promotion_ID(GetRecord_ID());
            Invt.SetName(user.GetName());
            Invt.SetEMail(user.GetEMail());
            Invt.SetPhone(user.GetPhone());
            // Invt.SetURL(url);
            if (!Invt.Save(Get_Trx()))
            {
                Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
            }

            string ID = Invt.GetC_InviteeList_ID().ToString();
            string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
            string urlFinal = "";
            urlFinal = url + "?" + encrypt;
            sql = "update c_inviteelist set url = '" + urlFinal + "' where c_inviteelist_id = " + Invt.GetC_InviteeList_ID();
            int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));


            //Random rand = new Random();
            //String s = "";
            //for (int i = 0; i < 9; i++)
            //    s = String.Concat(s, rand.Next(10).ToString());
            //string urlFinal = "";
            ////// urlFinal = url + "?" + Invt.GetC_InviteeList_ID().ToString();
            //urlFinal = url + "?" + s;
            //sql = "update c_inviteelist set url = '" + urlFinal + "' where c_inviteelist_id = " + Invt.GetC_InviteeList_ID();
            //int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));
            //Invt.SetURL(urlFinal);
            //if (!Invt.Save(Get_Trx()))
            //{
            //    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
            //}
        }
    }
}
