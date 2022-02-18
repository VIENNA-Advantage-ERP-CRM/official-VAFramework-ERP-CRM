/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LeadProject
 * Purpose        : Create Project from Lead
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class LeadProject : ProcessEngine.SvrProcess
    {
        /** Project Type		*/
        private int _C_ProjectType_ID = 0;
        /** Lead				*/
        private int _C_Lead_ID = 0;

        /// <summary>
        /// Prepare
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
                else if (name.Equals("C_ProjectType_ID"))
                {
                    _C_ProjectType_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_Lead_ID = GetRecord_ID();
        }   //	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>summary</returns>
        protected override String DoIt()
        {
            log.Info("C_Lead_ID=" + _C_Lead_ID + ",C_ProjectType_ID=" + _C_ProjectType_ID);
            if (_C_Lead_ID == 0)
            {
                throw new Exception("@C_Lead_ID@ ID=0");
            }
            if (_C_ProjectType_ID == 0)
            {
                throw new Exception("@C_ProjectType_ID@ ID=0");

            }

            MLead lead = new MLead(GetCtx(), _C_Lead_ID, Get_TrxName());
            if (lead.Get_ID() != _C_Lead_ID)
            {
                throw new Exception("@NotFound@: @C_Lead_ID@ ID=" + _C_Lead_ID);
            }

            #region Create Prospects Before Opportunity
            if (Env.IsModuleInstalled("VA047_") && lead.GetRef_BPartner_ID() == 0 && lead.GetC_BPartner_ID() == 0)
            {
                MBPartner bp = null;
                String rValue = lead.CreateBP();
                if (rValue != null)
                {
                    throw new SystemException(rValue);
                }
                lead.Save();

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
                {
                    MUser _user = new MUser(GetCtx(), lead.GetAD_User_ID(), Get_TrxName());
                    _user.SetName(Util.GetValueOfString(lead.Get_Value("Name2") + " " + Util.GetValueOfString(lead.Get_Value("ContactName"))));
                    _user.Set_Value("FirstName", Util.GetValueOfString(lead.Get_Value("Name2")));
                    _user.Set_Value("LastName", Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_Greeting_ID(lead.GetC_Greeting_ID());
                    _user.Set_Value("Value", Util.GetValueOfString(lead.Get_Value("Name2")) + " " + Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.Set_Value("FullName", Util.GetValueOfString(lead.Get_Value("Name2")) + " " + Util.GetValueOfString(lead.Get_Value("ContactName")));
                    _user.SetC_BPartner_Location_ID(lead.GetC_BPartner_Location_ID());
                    _user.SetMobile(lead.GetMobile());
                    _user.SetEMail(lead.GetEMail());
                    _user.SetDescription(lead.GetDescription());
                    if (!_user.Save())
                        log.SaveError("ERROR:", "Error in Saving User");
                }
                #region Customer Screening
                if (lead.GetC_BPartner_ID() == 0)
                {

                    int C_BpID = 0;
                    string val = Util.GetValueOfString(DB.ExecuteScalar("SELECT C_Lead_Target FROM C_Lead WHERE C_Lead_ID=" + lead.GetC_Lead_ID(), null, Get_TrxName())); ;
                    if (lead.GetContactName() != string.Empty)
                    {
                        C_BpID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartner_ID FROM C_BPartner WHERE Name='" + lead.GetBPName() + "'", null, Get_TrxName()));
                    }
                    if (C_BpID == 0)
                    {
                        C_BpID = lead.GetRef_BPartner_ID();
                    }
                    bp = new VAdvantage.Model.MBPartner(GetCtx(), C_BpID, Get_TrxName());
                    bp.Set_Value("C_SalesRegion_ID", Util.GetValueOfInt(lead.Get_Value("C_SalesRegion_ID")));
                    bp.SetC_Country_ID(Util.GetValueOfInt(lead.Get_Value("C_Country_ID")));
                    bp.Set_Value("R_Source_ID", Util.GetValueOfInt(lead.Get_Value("R_Source_ID")));
                    bp.Set_Value("R_Status_ID", Util.GetValueOfInt(lead.Get_Value("R_Status_ID")));
                    bp.Set_Value("NextStep", Util.GetValueOfString(lead.Get_Value("NextStep")));
                    bp.Set_Value("C_Followupdate", Util.GetValueOfDateTime(lead.Get_Value("C_Followupdate")));
                    bp.Set_Value("LeadRating", Util.GetValueOfString(lead.Get_Value("LeadRating")));
                    bp.Set_Value("C_LeadQualification_ID", Util.GetValueOfInt(lead.Get_Value("C_LeadQualification_ID")));
                    bp.Set_Value("C_BPartnerSR_ID", Util.GetValueOfInt(lead.Get_Value("C_BPartnerSR_ID")));
                    bp.Set_Value("Summary", Util.GetValueOfString(lead.Get_Value("Help")));
                    bp.Set_Value("EmailOptOut", Util.GetValueOfBool(lead.Get_Value("EmailOptOut")));
                    bp.SetC_Greeting_ID(Util.GetValueOfInt(lead.Get_Value("C_Greeting_ID")));
                    bp.SetC_BP_Status_ID(Util.GetValueOfInt(lead.Get_Value("C_BP_Status_ID")));
                    bp.SetEMail(Util.GetValueOfString(lead.Get_Value("EMail")));
                    bp.SetMobile(Util.GetValueOfString(lead.Get_Value("Mobile")));
                    bp.SetC_Campaign_ID(Util.GetValueOfInt(lead.Get_Value("C_Campaign_ID")));
                    bp.SetSalesRep_ID(Util.GetValueOfInt(lead.Get_Value("SalesRep_ID")));
                    bp.SetDescription(Util.GetValueOfString(lead.Get_Value("Description")));
                    bp.Set_Value("C_Lead_Target", val);
                    bp.Set_Value("Created", lead.GetCreated());
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
                                cspr.Set_Value("VA047_ERPExp", Util.GetValueOfDateTime(cus.Get_Value("VA047_ERPExp")));
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
                    // lead.SetC_BPartner_ID(C_BpID);
                    lead.Set_Value("Ref_BPartner_ID", C_BpID);
                    lead.Save(Get_TrxName());
                }
                #endregion

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
                            if (bp != null)
                                hist.SetRecord_ID(bp.GetC_BPartner_ID());
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
                            if (bp != null)
                                hist.SetRecord_ID(bp.GetC_BPartner_ID());
                            if (c_bpTableID > 0)
                                hist.SetAD_Table_ID(c_bpTableID);
                            if (!hist.Save())
                                log.SaveError("ERROR:", "Error in Copy HistoryRecords");
                        }

                    }

                }

                #endregion
                //
            }
            #endregion
            //
            String retValue = lead.CreateProject(_C_ProjectType_ID);
            if (retValue != null)
            {
                throw new SystemException(retValue);
            }
            lead.Save();
            MProject project = lead.GetProject();

            // SOTC specific work to set data on Opportunity
            if (Env.IsModuleInstalled("VA047_"))
            {
                project.SetC_Lead_ID(lead.GetC_Lead_ID());

                if (lead.GetC_BPartner_ID() > 0)
                    project.SetC_BPartner_ID(lead.GetC_BPartner_ID());
                else
                    project.SetC_BPartnerSR_ID(lead.GetRef_BPartner_ID());

                project.SetIsOpportunity(true);
                project.Set_Value("Created", lead.GetCreated());
                project.Save();
            }
            //
            return "@C_Project_ID@ " + project.GetName();
        }   //	doIt

    }	//	LeadProject

}
