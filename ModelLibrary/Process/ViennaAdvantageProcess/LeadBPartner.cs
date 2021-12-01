/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LeadBPartner
 * Purpose        : Create BP Contact, Account, Location
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           09-Dec-2009
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
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

//namespace VAdvantage.Process
namespace ViennaAdvantage.Process
{
    public class LeadBPartner : VAdvantage.ProcessEngine.SvrProcess
    {
        /** Lead				*/
        private int _C_Lead_ID = 0;
        int C_BpID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            if (para.Length > 0)
            {
                foreach (ProcessInfoParameter element in para)
                {
                    String name = element.GetParameterName();
                    if (name.Equals("_C_Lead_ID"))
                    {
                        _C_Lead_ID = element.GetParameterAsInt();
                    }
                }
            }
            else
            {
                _C_Lead_ID = GetRecord_ID();
            }
            //_C_Lead_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Create BPartner
        /// </summary>
        /// <returns>BPartner</returns>
        protected override String DoIt()
        {
            log.Info("C_Lead_ID=" + _C_Lead_ID);
            if (_C_Lead_ID == 0)
            {
                throw new Exception("@C_Lead_ID@ ID=0");
            }

            MLead lead = new MLead(GetCtx(), _C_Lead_ID, Get_TrxName());

            // Check IsArchive and Lead Qualification and IsQualified 
            if (Env.IsModuleInstalled("VA047_"))
            {
                string str;
                string checkqualied;
                string Archive = Util.GetValueOfString(lead.Get_Value("IsArchive"));
                if (Archive.Equals("False"))
                {
                    if (lead.GetC_LeadQualification_ID() > 0)
                    {
                        str = "SELECT IsQualified FROM C_LeadQualification WHERE C_LeadQualification_ID =" + lead.GetC_LeadQualification_ID();
                        checkqualied = Util.GetValueOfString(DB.ExecuteScalar(str));
                        if (!checkqualied.Equals("Y"))
                        {
                            return Msg.GetMsg(GetCtx(), "VA047_ProspectNotQualified");
                        }
                    }
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "VA047_ProspectNotArchive");
                }
            }

            if (lead.GetC_BP_Group_ID() == 0)
            {
                return Msg.GetMsg(GetCtx(), "SelectBPGroup");
            }
            if (lead.GetBPName() == null)
            {
                return Msg.GetMsg(GetCtx(), "PleaseEnterProspectDetails");
            }
            if (lead.Get_ID() != _C_Lead_ID)
            {
                throw new Exception("@NotFound@: @C_Lead_ID@ ID=" + _C_Lead_ID);
            }
            //
            String retValue = lead.CreateBP();
            if (retValue != null)
            {
                return GetRetrievedError(lead, retValue);
                //throw new SystemException(retValue);
            }

            // Set Archive and Status value on lead
            if (Env.IsModuleInstalled("VA061_"))
            {
                lead.Set_Value("IsArchive", true);
                int statusId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT R_Status_ID FROM R_Status Where Value = 'CNV' AND IsActive = 'Y' " +
                    "AND R_StatusCategory_ID = (SELECT R_StatusCategory_ID FROM R_RequestType WHERE R_RequestType_ID = (SELECT R_RequestType_ID FROM " +
                    "AD_ClientInfo WHERE AD_Client_ID = " + GetAD_Client_ID() + "))", null, null));

                if (statusId > 0)
                {
                    lead.SetR_Status_ID(statusId);
                }
            }

            lead.Save();

            // Work done specific to SOTC Module
            if (Env.IsModuleInstalled("VA047_"))
            {
                MBPartner bp = null;
                if (lead.GetC_BPartner_ID() > 0)
                {
                    MBPartner _cbp = new MBPartner(GetCtx(), lead.GetC_BPartner_ID(), Get_TrxName());
                    _cbp.SetC_Greeting_ID(lead.GetC_Greeting_ID());
                    _cbp.SetDescription(lead.GetDescription());
                    _cbp.SetC_IndustryCode_ID(lead.GetC_IndustryCode_ID());
                    _cbp.SetEMail(lead.GetEMail());
                    _cbp.SetMobile(lead.GetMobile());

                    if (!_cbp.Save())
                        log.SaveError("ERROR:", "Error in Saving Bpartner");
                }
                if (lead.GetAD_User_ID() > 0)
                { // Commented few lines as it is overwriting data and few fields are not available on DB level

                    MUser _user = new MUser(GetCtx(), lead.GetAD_User_ID(), Get_TrxName());
                    // _user.SetName(Util.GetValueOfString(lead.Get_Value("Name2") + " " + Util.GetValueOfString(lead.Get_Value("ContactName"))));
                    //_user.Set_Value("FirstName", Util.GetValueOfString(lead.Get_Value("Name2")));
                    //  _user.Set_Value("LastName", Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_Greeting_ID(lead.GetC_Greeting_ID());
                    //  _user.Set_Value("Value", Util.GetValueOfString(lead.Get_Value("Name2")) + " " + Util.GetValueOfString(lead.Get_Value("ContactName")));
                    // _user.Set_Value("FullName", Util.GetValueOfString(lead.Get_Value("Name2")) + " " + Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                    _user.SetMobile(lead.GetMobile());
                    _user.Set_Value("Phone", Util.GetValueOfString(lead.GetPhone()));
                    _user.Set_Value("Phone2", Util.GetValueOfString(lead.GetPhone2()));
                    // _user.Set_Value("VA047_JobTitle", Util.GetValueOfString(lead.Get_Value("VA047_JobTitle")));
                    _user.SetEMail(lead.GetEMail());
                    _user.SetDescription(lead.GetDescription());
                    if (!_user.Save())
                        log.SaveError("ERROR:", "Error in Saving User");
                }

                #region Customer Screening
                if (lead.GetC_BPartner_ID() == 0)
                {
                    string val = Util.GetValueOfString(DB.ExecuteScalar("SELECT C_Lead_Target FROM C_Lead WHERE C_Lead_ID=" + lead.GetC_Lead_ID(), null, Get_TrxName())); ;

                    if (lead.GetContactName() != string.Empty)
                    {
                        C_BpID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartner_ID FROM C_BPartner WHERE Name='" + lead.GetBPName() + "'", null, Get_TrxName()));
                    }
                    if (C_BpID == 0)
                    {
                        C_BpID = lead.GetRef_BPartner_ID();
                    }
                    bp = new MBPartner(GetCtx(), C_BpID, Get_TrxName());
                    bp.Set_Value("Summary", Util.GetValueOfString(lead.Get_Value("Help")));
                    bp.Set_Value("EmailOptOut", Util.GetValueOfBool(lead.Get_Value("EmailOptOut")));
                    bp.Set_Value("C_Lead_Target", Util.GetValueOfString(lead.Get_Value("C_Lead_Target")));
                    bp.Set_Value("Phone", Util.GetValueOfString(lead.GetPhone()));
                    bp.Set_Value("VA047_JobTitle", Util.GetValueOfString(lead.Get_Value("VA047_JobTitle")));
                    bp.Set_Value("C_City_ID", Util.GetValueOfInt(lead.Get_Value("C_City_ID")));
                    bp.Set_Value("VA047_UsingSystem_ID", Util.GetValueOfInt(lead.Get_Value("VA047_UsingSystem_ID")));
                    bp.Set_Value("DateDiffrence", Util.GetValueOfInt(lead.Get_Value("DateDiffrence")));
                    bp.SetC_Greeting_ID(Util.GetValueOfInt(lead.Get_Value("C_Greeting_ID")));
                    bp.SetC_BP_Status_ID(Util.GetValueOfInt(lead.Get_Value("C_BP_Status_ID")));
                    bp.Set_Value("VA047_LinkedIn", Util.GetValueOfString(lead.Get_Value("VA047_LinkedIn")));

                    //C_Location_ID

                    bp.Save();

                    if (val == "CO")
                    {
                        int[] cs = PO.GetAllIDs("VA047_CustomerScreening", "C_Lead_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                        if (cs.Length > 0 && C_BpID > 0)
                        {
                            MTable tbl = new MTable(GetCtx(), MTable.Get_Table_ID("VA047_CustomerScreening"), null);
                            PO cus = null;
                            MTable tbl1 = new MTable(GetCtx(), MTable.Get_Table_ID("VA047_CustomerScreeningPr"), null);
                            PO cspr = null;

                            for (int i = 0; i < cs.Length; i++)
                            {
                                cus = tbl.GetPO(GetCtx(), cs[i], Get_TrxName());
                                cspr = tbl1.GetPO(GetCtx(), 0, Get_TrxName());
                                cspr.Set_ValueNoCheck("C_BPartner_ID", C_BpID);
                                cspr.Set_Value("M_Product_ID", cus.Get_Value("M_Product_ID"));
                                cspr.Set_Value("C_IndustryCode_ID", cus.Get_Value("C_IndustryCode_ID"));
                                cspr.Set_Value("VA047_UsingSystem_ID", cus.Get_Value("VA047_UsingSystem_ID"));
                                cspr.Set_Value("VA047_BudgetUSD", Util.GetValueOfDecimal(cus.Get_Value("VA047_BudgetUSD")));
                                cspr.Set_Value("VA047_NoOfEmp", Util.GetValueOfDecimal(cus.Get_Value("VA047_NoOfEmp")));
                                cspr.Set_Value("VA047_AnnualTO", Util.GetValueOfDecimal(cus.Get_Value("VA047_AnnualTO")));
                                cspr.Set_Value("VA047_facebook", Util.GetValueOfString(cus.Get_Value("VA047_facebook")));
                                cspr.Set_Value("VA047_Hobbies", Util.GetValueOfString(cus.Get_Value("VA047_Hobbies")));
                                cspr.Set_Value("VA047_LinkedIn", Util.GetValueOfString(cus.Get_Value("VA047_LinkedIn")));
                                cspr.Set_Value("VA047_Skype", Util.GetValueOfString(cus.Get_Value("VA047_Skype")));
                                cspr.Set_Value("VA047_Twitter", Util.GetValueOfString(cus.Get_Value("VA047_Twitter")));
                                cspr.Set_Value("VA047_Ethnicity", Util.GetValueOfString(cus.Get_Value("VA047_Ethnicity")));
                                cspr.Set_Value("VA047_Age", Util.GetValueOfDecimal(cus.Get_Value("VA047_Age")));
                                cspr.Set_Value("VA047_ReasonEVienna_ID", cus.Get_Value("VA047_ReasonEVienna_ID"));
                                cspr.Set_Value("VA047_Option1", Util.GetValueOfInt(cus.Get_Value("VA047_Option1")));
                                cspr.Set_Value("VA047_Option2", Util.GetValueOfInt(cus.Get_Value("VA047_Option2")));
                                cspr.Set_Value("VA047_Option3", Util.GetValueOfInt(cus.Get_Value("VA047_Option3")));
                                cspr.Set_Value("VA047_NoOfUser", Util.GetValueOfInt(cus.Get_Value("VA047_NoOfUser")));
                                if (Util.GetValueOfString(cus.Get_Value("VA047_WealthEvaluation")) != string.Empty)
                                    cspr.Set_Value("VA047_WealthEvaluation", Util.GetValueOfString(cus.Get_Value("VA047_WealthEvaluation")));
                                cspr.Set_Value("Va047_BuyDate", Util.GetValueOfDateTime(cus.Get_Value("Va047_BuyDate")));
                                if (!cspr.Save())
                                    log.SaveError("ERROR:", "Error in Saving CustomerScreening");
                                bp.Set_Value("C_Lead_Target", "CO");
                                bp.Save();
                            }
                        }
                    }
                    else if (val == "PS")
                    {
                        int[] cs = PO.GetAllIDs("VA047_PartnerScreening", "C_Lead_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                        if (cs.Length > 0 && C_BpID > 0)
                        {
                            MTable tbl = new MTable(GetCtx(), MTable.Get_Table_ID("VA047_PartnerScreening"), null);
                            PO cus = null;
                            MTable tbl1 = new MTable(GetCtx(), MTable.Get_Table_ID("VA047_PartnerScreeningPr"), null);
                            PO cspr = null;
                            for (int i = 0; i < cs.Length; i++)
                            {
                                cus = tbl.GetPO(GetCtx(), cs[i], Get_TrxName());
                                cspr = tbl1.GetPO(GetCtx(), 0, Get_TrxName());
                                cspr.Set_ValueNoCheck("C_BPartner_ID", C_BpID);
                                cspr.Set_Value("M_Product_ID", cus.Get_Value("M_Product_ID"));
                                cspr.Set_Value("C_IndustryCode_ID", cus.Get_Value("C_IndustryCode_ID"));
                                cspr.Set_Value("VA047_UsingSystem_ID", cus.Get_Value("VA047_UsingSystem_ID"));
                                cspr.Set_Value("VA047_BudgetUSD", Util.GetValueOfDecimal(cus.Get_Value("VA047_BudgetUSD")));
                                cspr.Set_Value("VA047_NoOfEmp", Util.GetValueOfDecimal(cus.Get_Value("VA047_NoOfEmp")));
                                cspr.Set_Value("VA047_AnnualTO", Util.GetValueOfDecimal(cus.Get_Value("VA047_AnnualTO")));
                                cspr.Set_Value("VA047_ERPExp", Util.GetValueOfDecimal(cus.Get_Value("VA047_ERPExp")));
                                cspr.Set_Value("VA047_facebook", Util.GetValueOfString(cus.Get_Value("VA047_facebook")));
                                cspr.Set_Value("VA047_Hobbies", Util.GetValueOfString(cus.Get_Value("VA047_Hobbies")));
                                cspr.Set_Value("VA047_LinkedIn", Util.GetValueOfString(cus.Get_Value("VA047_LinkedIn")));
                                cspr.Set_Value("VA047_Skype", Util.GetValueOfString(cus.Get_Value("VA047_Skype")));
                                cspr.Set_Value("VA047_Twitter", Util.GetValueOfString(cus.Get_Value("VA047_Twitter")));
                                cspr.Set_Value("VA047_Ethnicity", Util.GetValueOfString(cus.Get_Value("VA047_Ethnicity")));
                                cspr.Set_Value("VA047_Age", Util.GetValueOfDecimal(cus.Get_Value("VA047_Age")));
                                cspr.Set_Value("VA047_ReasonEVienna_ID", cus.Get_Value("VA047_ReasonEVienna_ID"));
                                cspr.Set_Value("VA047_Vertical1", Util.GetValueOfInt(cus.Get_Value("VA047_Vertical1")));
                                cspr.Set_Value("VA047_Vertical2", Util.GetValueOfInt(cus.Get_Value("VA047_Vertical2")));
                                cspr.Set_Value("VA047_Vertical3", Util.GetValueOfInt(cus.Get_Value("VA047_Vertical3")));
                                cspr.Set_Value("VA047_Option1", Util.GetValueOfInt(cus.Get_Value("VA047_Option1")));
                                cspr.Set_Value("VA047_Option2", Util.GetValueOfInt(cus.Get_Value("VA047_Option2")));
                                cspr.Set_Value("VA047_Option3", Util.GetValueOfInt(cus.Get_Value("VA047_Option3")));
                                cspr.Set_Value("VA047_PartnerType", Util.GetValueOfString(cus.Get_Value("VA047_PartnerType")));
                                cspr.Set_Value("VA047_Vertical4", Util.GetValueOfInt(cus.Get_Value("VA047_Vertical4")));
                                cspr.Set_Value("VA047_UsingSystem1", Util.GetValueOfInt(cus.Get_Value("VA047_UsingSystem1")));
                                cspr.Set_Value("VA047_UsingSystem2", Util.GetValueOfInt(cus.Get_Value("VA047_UsingSystem2")));
                                if (Util.GetValueOfString(cus.Get_Value("VA047_WealthEvaluation")) != string.Empty)
                                    cspr.Set_Value("VA047_WealthEvaluation", Util.GetValueOfString(cus.Get_Value("VA047_WealthEvaluation")));
                                cspr.Set_Value("VA047_DecideDate", Util.GetValueOfDateTime(cus.Get_Value("VA047_DecideDate")));
                                if (!cspr.Save())
                                    log.SaveError("ERROR:", "Error in Saving PartnererScreening");
                                bp.Set_Value("C_Lead_Target", "PS");
                                bp.Save();
                            }
                        }
                    }
                    //lead.SetC_BPartner_ID(C_BpID);
                    lead.Set_Value("Ref_BPartner_ID", C_BpID);
                    lead.Save(Get_TrxName());
                }
                #endregion                
            }

            if (C_BpID == 0)
            {
                C_BpID = lead.GetRef_BPartner_ID();
            }

            #region Copy Mail
            int tableID = PO.Get_Table_ID("C_Lead");
            int c_bpTableID = PO.Get_Table_ID("C_BPartner");

            if (tableID > 0)
            {
                int[] RecordIDS = MMailAttachment1.GetAllIDs("MailAttachment1", "AD_Table_ID=" + tableID + "AND Record_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                if (RecordIDS.Length > 0)
                {
                    MMailAttachment1 hist = null;
                    MMailAttachment1 Oldhist = null;
                    for (int i = 0; i < RecordIDS.Length; i++)
                    {
                        Oldhist = new MMailAttachment1(GetCtx(), RecordIDS[i], Get_TrxName());
                        hist = new MMailAttachment1(GetCtx(), 0, Get_TrxName());
                        Oldhist.CopyTo(hist);
                        if (C_BpID != 0)
                            hist.SetRecord_ID(C_BpID);
                        if (c_bpTableID > 0)
                            hist.SetAD_Table_ID(c_bpTableID);
                        if (!hist.Save())
                            log.SaveError("ERROR:", "Error in Copy Email");

                    }

                }

            }
            #endregion

            #region Copy History Records
            if (tableID > 0)
            {
                int[] RecordsIDS = MAppointmentsInfo.GetAllIDs("AppointmentsInfo", "AD_Table_ID=" + tableID + "AND Record_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                if (RecordsIDS.Length > 0)
                {
                    MAppointmentsInfo hist = null;
                    MAppointmentsInfo Oldhist = null;
                    for (int i = 0; i < RecordsIDS.Length; i++)
                    {
                        Oldhist = new MAppointmentsInfo(GetCtx(), RecordsIDS[i], Get_TrxName());
                        hist = new MAppointmentsInfo(GetCtx(), 0, Get_TrxName());
                        Oldhist.CopyTo(hist);
                        hist.SetStartDate(Oldhist.GetStartDate().Value.ToLocalTime());
                        hist.SetEndDate(Oldhist.GetEndDate().Value.ToLocalTime());
                        if (C_BpID != 0)
                            hist.SetRecord_ID(C_BpID);
                        if (c_bpTableID > 0)
                            hist.SetAD_Table_ID(c_bpTableID);
                        if (!hist.Save())
                            log.SaveError("ERROR:", "Error in Copy HistoryRecords");
                    }
                }
            }

            #endregion

            //Copy chat data from Lead window to prospect window
            #region Copy Chat Data
            if (tableID > 0)
            {
                int[] chatID = MChat.GetAllIDs("CM_Chat", "AD_Table_ID=" + tableID + "AND Record_ID=" + lead.GetC_Lead_ID(), Get_TrxName());
                if (chatID.Length > 0)
                {
                    MChatEntry chatEntry = null;
                    MChatEntry oldChatEntry = null;
                    MChat oldchat = new MChat(GetCtx(), chatID[0], Get_TrxName());
                    MChat newchat = new MChat(GetCtx(), 0, Get_TrxName());
                    oldchat.CopyTo(newchat);
                    if (C_BpID != 0)
                        newchat.SetRecord_ID(C_BpID);
                    if (c_bpTableID > 0)
                        newchat.SetAD_Table_ID(c_bpTableID);

                    if (newchat.Save())
                    {
                        string sql = ("SELECT CM_ChatEntry_ID FROM CM_ChatEntry WHERE IsActive='Y' AND CM_Chat_ID=" + chatID[0]);
                        DataSet ds = DB.ExecuteDataset(sql, null, Get_TrxName());

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                oldChatEntry = new MChatEntry(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["CM_ChatEntry_ID"]), Get_TrxName());
                                chatEntry = new MChatEntry(GetCtx(), 0, Get_TrxName());
                                oldChatEntry.CopyTo(chatEntry);
                                chatEntry.SetCM_Chat_ID(newchat.GetCM_Chat_ID());
                                chatEntry.SetCharacterData(oldChatEntry.GetCharacterData());
                                if (!chatEntry.Save())
                                {
                                    log.Severe("VIS_ErrorCopyChatData");
                                }
                            }
                        }
                    }
                    else
                    {
                        log.Severe("VIS_ErrorCopyChatData");
                    }
                }
            }

            #endregion
            //
            if (lead.GetRef_BPartner_ID() != 0)
            {
                return Msg.GetMsg(GetCtx(), "ProspectGenerated");
            }
            else
                return Msg.GetMsg(GetCtx(), "ProspectNotGenerated");

        }	//	doIt

    }

}
