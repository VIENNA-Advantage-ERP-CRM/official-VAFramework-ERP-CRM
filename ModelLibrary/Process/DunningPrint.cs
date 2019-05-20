/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DunningPrint
 * Purpose        : Dunning Letter Print    
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      11-Nov-2009
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
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;
using System.IO;

using VAdvantage.ProcessEngine;
using VAdvantage.Login;
using VAdvanatge.Report;


namespace VAdvantage.Process
{
    public class DunningPrint : ProcessEngine.SvrProcess
    {
        #region Private Variable
        //	Mail PDF				
        private bool _EMailPDF = false;
        // Mail Template			
        private int _R_MailText_ID = 0;
        // Dunning Run				
        private int _C_DunningRun_ID = 0;
        // Print only Outstanding	
        private bool _IsOnlyIfBPBalance = true;

        // Type of report required. i.e pdf, csv etc.
        string FileType = "P";
        #endregion

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
                else if (name.Equals("EMailPDF"))
                {
                    _EMailPDF = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("R_MailText_ID"))
                {
                    _R_MailText_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_DunningRun_ID"))
                {
                    _C_DunningRun_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("IsOnlyIfBPBalance"))
                {
                    _IsOnlyIfBPBalance = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("C_DunningRun_ID=" + _C_DunningRun_ID + ",R_MailText_ID=" + _R_MailText_ID
                + ", EmailPDF=" + _EMailPDF + ",IsOnlyIfBPBalance=" + _IsOnlyIfBPBalance);

            //	Need to have Template
            if (_EMailPDF && _R_MailText_ID == 0)
            {
                throw new Exception("@NotFound@: @R_MailText_ID@");
            }
            String subject = "";
            MMailText mText = null;
            if (_EMailPDF)
            {
                mText = new MMailText(GetCtx(), _R_MailText_ID, Get_TrxName());
                if (_EMailPDF && mText.Get_ID() == 0)
                {
                    throw new Exception("@NotFound@: @R_MailText_ID@ - " + _R_MailText_ID);
                }
                subject = mText.GetMailHeader();
            }
            //
            MDunningRun run = new MDunningRun(GetCtx(), _C_DunningRun_ID, Get_TrxName());
            if (run.Get_ID() == 0)
            {
                throw new Exception("@NotFound@: @C_DunningRun_ID@ - " + _C_DunningRun_ID);
            }
            //	Print Format on Dunning Level
            MDunningLevel level = new MDunningLevel(GetCtx(), run.GetC_DunningLevel_ID(), Get_TrxName());
            MPrintFormat format = MPrintFormat.Get(GetCtx(), level.GetDunning_PrintFormat_ID(), false);

            MClient client = MClient.Get(GetCtx());

            int count = 0;
            int errors = 0;
            MDunningRunEntry[] entries = run.GetEntries(false);
            for (int i = 0; i < entries.Length; i++)
            {
                MDunningRunEntry entry = entries[i];
                if (_IsOnlyIfBPBalance && Env.Signum(entry.GetAmt()) <= 0)
                {
                    continue;
                }
                //	To BPartner
                MBPartner bp = new MBPartner(GetCtx(), entry.GetC_BPartner_ID(), Get_TrxName());
                if (bp.Get_ID() == 0)
                {
                    AddLog(entry.Get_ID(), null, null, "@NotFound@: @C_BPartner_ID@ " + entry.GetC_BPartner_ID());
                    errors++;
                    continue;
                }
                //	To User
                MUser to = new MUser(GetCtx(), entry.GetAD_User_ID(), Get_TrxName());
                if (_EMailPDF)
                {
                    if (to.Get_ID() == 0)
                    {
                        AddLog(entry.Get_ID(), null, null, "@NotFound@: @AD_User_ID@ - " + bp.GetName());
                        errors++;
                        continue;
                    }
                    else if (to.GetEMail() == null || to.GetEMail().Length == 0)
                    {
                        AddLog(entry.Get_ID(), null, null, "@NotFound@: @EMail@ - " + to.GetName());
                        errors++;
                        continue;
                    }
                }
                //	BP Language
                //Language language =Language.getLoginLanguage();		//	Base Language
                Language language = Env.GetLoginLanguage(GetCtx());

                String tableName = "C_Dunning_Header_v";
                if (client.IsMultiLingualDocument())
                {
                    tableName += "t";
                    String AD_Language = bp.GetAD_Language();
                    if (AD_Language != null)
                    {
                        //language =language.getLanguage(AD_Language);
                    }
                }
               // format.SetLanguage(language);
               // format.SetTranslationLanguage(language);
                //	query
                Query query = new Query(tableName);
                query.AddRestriction("C_DunningRunEntry_ID", Query.EQUAL,
                    entry.GetC_DunningRunEntry_ID());

                //	Engine
                //PrintInfo info = new PrintInfo(
                //    bp.GetName(),
                //    X_C_DunningRunEntry.Table_ID,
                //    entry.GetC_DunningRunEntry_ID(),
                //    entry.GetC_BPartner_ID());
                //info.SetDescription(bp.GetName() + ", Amt=" + entry.GetAmt());
                //ReportEngine re = new ReportEngine(GetCtx(), format, query, info);
                byte[] pdfReport;
                string reportPath = "";

                bool printed = false;
                if (_EMailPDF)
                {
                    EMail email = client.CreateEMail(to.GetEMail(), to.GetName(), null, null);
                    if (email == null || !email.IsValid())
                    {
                        AddLog(entry.Get_ID(), null, null,
                            "@RequestActionEMailError@ Invalid EMail: " + to);
                        errors++;
                        continue;
                    }
                    mText.SetUser(to);	//	variable context
                    mText.SetBPartner(bp);
                    mText.SetPO(entry);
                    String message = mText.GetMailText(true);
                    if (mText.IsHtml())
                    {
                        email.SetMessageHTML(mText.GetMailHeader(), message);
                    }
                    else
                    {
                        email.SetSubject(mText.GetMailHeader());
                        email.SetMessageText(message);
                    }
                   
                    //
                    //File attachment = re.GetPDF(File.createTempFile("Dunning", ".pdf"));
                    //log.Fine(to + " - " + attachment);
                    //email.AddAttachment(attachment);
                    //
                    // Get report that is bound on the dunning entry tab of dunning run window.
                    int ReportProcess_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Process_ID FROM ad_tab WHERE  export_id='VIS_634'"));
                    if (ReportProcess_ID > 0)
                    {
                        
                        Common.Common Com = new Common.Common();
                        Dictionary<string, object> d = Com.GetReport(GetCtx(), ReportProcess_ID, "C_DunningRunEntry", X_C_DunningRunEntry.Table_ID, entry.GetC_DunningRunEntry_ID(), 0, "", FileType, out pdfReport, out reportPath);
                        if (pdfReport != null)
                        {
                            email.AddAttachment(pdfReport, "Dunning" + entry.GetC_DunningRunEntry_ID() + ".pdf");
                        }
                    }
                    String msg = email.Send();
                    MUserMail um = new MUserMail(mText, entry.GetAD_User_ID(), email);
                    um.Save();
                    if (msg.Equals(EMail.SENT_OK))
                    {
                        AddLog(entry.Get_ID(), null, null,
                            bp.GetName() + " @RequestActionEMailOK@");
                        count++;
                        printed = true;
                    }
                    else
                    {
                        AddLog(entry.Get_ID(), null, null,
                            bp.GetName() + " @RequestActionEMailError@ " + msg);
                        errors++;
                    }
                }
                else
                { 
                    //re.print();
                    count++;
                    printed = true;
                }
                if (printed)
                {
                    entry.SetProcessed(true);
                    entry.Save();
                    DunningLevelConsequences(level, entry);
                }

            }	//	for all dunning letters
            if (errors == 0)
            {
                run.SetProcessed(true);
                run.Save();
            }

            if (_EMailPDF)
            {
                return "@Sent@=" + count + " - @Errors@=" + errors;
            }
            return "@Printed@=" + count;
        }

        /// <summary>
        /// Dunning Level Consequences
        /// </summary>
        /// <param name="level"></param>
        /// <param name="entry"></param>
        private void DunningLevelConsequences(MDunningLevel level, MDunningRunEntry entry)
        {
            //	Update Business Partner based on Level
            if (level.IsSetCreditStop() || level.IsSetPaymentTerm())
            {
                MBPartner thisBPartner = new MBPartner(GetCtx(), entry.GetC_BPartner_ID(), Get_TrxName());
                if (level.IsSetCreditStop())
                    thisBPartner.SetSOCreditStatus(X_C_BPartner.SOCREDITSTATUS_CreditStop);
                if (level.IsSetPaymentTerm() && level.GetC_PaymentTerm_ID() != 0)
                {
                    thisBPartner.SetC_PaymentTerm_ID(level.GetC_PaymentTerm_ID());
                }
                thisBPartner.Save();
            }
            //	Update Invoices if not Statement (Statement is hardcoded -9999 see also MDunningLevel)
            if (!level.GetDaysAfterDue().Equals(new Decimal(-9999)) && level.GetInvoiceCollectionType() != null)
            {
                MDunningRunLine[] lines = entry.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    MDunningRunLine line = lines[i];
                    if (line.GetC_Invoice_ID() != 0 && line.IsActive())
                    {
                        MInvoice invoice = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_TrxName());
                        invoice.SetInvoiceCollectionType(level.GetInvoiceCollectionType());
                        invoice.Save();
                    }
                }
            }
        }
    }
}
