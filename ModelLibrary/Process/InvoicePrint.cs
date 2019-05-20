/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoicePrint
 * Purpose        : Print Invoices on Paperor send PDFs
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          06-Jan-2010
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
namespace VAdvantage.Process
{
   public class InvoicePrint:ProcessEngine.SvrProcess
    {
    //	Mail PDF			
	private Boolean		_EMailPDF = false;
	// Mail Template		
	private int			_R_MailText_ID = 0;
	
	private DateTime?	_dateInvoiced_From = null;
	private DateTime?	_dateInvoiced_To = null;
	private int			_C_BPartner_ID = 0;
	private int			_C_Invoice_ID = 0;
	private String		_DocumentNo_From = null;
	private String		_DocumentNo_To = null;
	private Boolean		_IsSOTrx = true;
	private Boolean		_IncludeDraftInvoices = true;

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
            else if (name.Equals("DateInvoiced"))
            {
                _dateInvoiced_From = ((DateTime?)para[i].GetParameter());
                _dateInvoiced_To = ((DateTime?)para[i].GetParameter_To());
            }
            else if (name.Equals("EMailPDF"))
            {
                _EMailPDF = "Y".Equals(para[i].GetParameter());
            }
            else if (name.Equals("R_MailText_ID"))
            {
                _R_MailText_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("C_BPartner_ID"))
            {
                _C_BPartner_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("C_Invoice_ID"))
            {
                _C_Invoice_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("DocumentNo"))
            {
                _DocumentNo_From = (String)para[i].GetParameter();
                _DocumentNo_To = (String)para[i].GetParameter_To();
            }
            else if (name.Equals("IsSOTrx"))
            {
                _IsSOTrx = "Y".Equals(para[i].GetParameter());
            }
            else if (name.Equals("IncludeDraftInvoices"))
            {
                _IncludeDraftInvoices = "Y".Equals(para[i].GetParameter());
            }
            else
            {
                log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
        if (_DocumentNo_From != null && _DocumentNo_From.Length == 0)
        {
            _DocumentNo_From = null;
        }
        if (_DocumentNo_To != null && _DocumentNo_To.Length == 0)
        {
            _DocumentNo_To = null;
        }
	}	//	prepare

	/// <summary>
	/// Perrform Process.
	/// </summary>
    /// <returns>Message</returns>
	protected override String DoIt() 
	{
		//	Need to have Template
        if (_EMailPDF && _R_MailText_ID == 0)
        {
            throw new Exception("@NotFound@: @R_MailText_ID@");
        }
		log.Info ("C_BPartner_ID=" + _C_BPartner_ID
			+ ", C_Invoice_ID=" + _C_Invoice_ID
			+ ", IsSOTrx=" + _IsSOTrx
			+ ", EmailPDF=" + _EMailPDF + ",R_MailText_ID=" + _R_MailText_ID
			+ ", DateInvoiced=" + _dateInvoiced_From + "-" + _dateInvoiced_To
			+ ", DocumentNo=" + _DocumentNo_From + "-" + _DocumentNo_To
			+ ", IncludeDrafts="+_IncludeDraftInvoices);
		
		MMailText mText = null;
		if (_R_MailText_ID != 0)
		{
			mText = new MMailText(GetCtx(), _R_MailText_ID, Get_TrxName());
            if (mText.Get_ID() != _R_MailText_ID)
            {
                throw new Exception("@NotFound@: @R_MailText_ID@ - " + _R_MailText_ID);
            }
		}

		//	Too broad selection
        if (_C_BPartner_ID == 0 && _C_Invoice_ID == 0 && _dateInvoiced_From == null && _dateInvoiced_To == null
            && _DocumentNo_From == null && _DocumentNo_To == null)
        {
            throw new Exception("@RestrictSelection@");
        }

		MClient client = MClient.Get(GetCtx());
		
		//	Get Info
		StringBuilder sql = new StringBuilder (
			"SELECT i.C_Invoice_ID,bp.AD_Language,c.IsMultiLingualDocument,"		//	1..3
			//	Prio: 1. BPartner 2. DocType, 3. PrintFormat (Org)	//	see ReportCtl+MInvoice
			+ " COALESCE(bp.Invoice_PrintFormat_ID, dt.AD_PrintFormat_ID, pf.Invoice_PrintFormat_ID),"	//	4 
			+ " dt.DocumentCopies+bp.DocumentCopies,"								//	5
			+ " bpc.AD_User_ID, i.DocumentNo,"										//	6..7
			+ " bp.C_BPartner_ID "													//	8
			+ "FROM C_Invoice i"
			+ " INNER JOIN C_BPartner bp ON (i.C_BPartner_ID=bp.C_BPartner_ID)"
			+ " LEFT OUTER JOIN AD_User bpc ON (i.AD_User_ID=bpc.AD_User_ID)"
			+ " INNER JOIN AD_Client c ON (i.AD_Client_ID=c.AD_Client_ID)"
			+ " INNER JOIN AD_PrintForm pf ON (i.AD_Client_ID=pf.AD_Client_ID)"
			+ " INNER JOIN C_DocType dt ON (i.C_DocType_ID=dt.C_DocType_ID)")
			.Append(" WHERE pf.AD_Org_ID IN (0,i.AD_Org_ID) AND ");	//	more them 1 PF
        if (_C_Invoice_ID != 0)
        {
            sql.Append("i.C_Invoice_ID=").Append(_C_Invoice_ID);
        }
        else
        {
            if (_IsSOTrx)
            {
                sql.Append("i.IsSOTrx='Y'");
            }
            else
            {
                sql.Append("i.IsSOTrx='N'");
            }

            if (!_IncludeDraftInvoices)
            {
                sql.Append("AND i.DocStatus NOT IN ('DR')");
            }
            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND i.C_BPartner_ID=").Append(_C_BPartner_ID);
            }
            //	Date Invoiced 
            if (_dateInvoiced_From != null && _dateInvoiced_To != null)
            {
                sql.Append(" AND TRUNC(i.DateInvoiced,'DD') BETWEEN ")
                    .Append(DataBase.DB.TO_DATE(_dateInvoiced_From, true)).Append(" AND ")
                    .Append(DataBase.DB.TO_DATE(_dateInvoiced_To, true));
            }
            else if (_dateInvoiced_From != null)
            {
                sql.Append(" AND TRUNC(i.DateInvoiced,'DD') >= ")
                    .Append(DataBase.DB.TO_DATE(_dateInvoiced_From, true));
            }
            else if (_dateInvoiced_To != null)
            {
                sql.Append(" AND TRUNC(i.DateInvoiced,'DD') <= ")
                    .Append(DataBase.DB.TO_DATE(_dateInvoiced_To, true));
            }
            //	Document No
            else if (_DocumentNo_From != null && _DocumentNo_To != null)
            {
                sql.Append(" AND i.DocumentNo BETWEEN ")
                    .Append(DataBase.DB.TO_STRING(_DocumentNo_From)).Append(" AND ")
                    .Append(DataBase.DB.TO_STRING(_DocumentNo_To));
            }
            else if (_DocumentNo_From != null)
            {
                sql.Append(" AND ");
                if (_DocumentNo_From.IndexOf('%') == -1)
                {
                    sql.Append("i.DocumentNo >= ")
                        .Append(DataBase.DB.TO_STRING(_DocumentNo_From));
                }
                else
                {
                    sql.Append("i.DocumentNo LIKE ")
                        .Append(DataBase.DB.TO_STRING(_DocumentNo_From));
                }
            }
        }
		sql.Append(" ORDER BY i.C_Invoice_ID, pf.AD_Org_ID DESC");	//	more than 1 PF record
		log.Finer(sql.ToString());

		MPrintFormat format = null;
		int old_AD_PrintFormat_ID = -1;
		int old_C_Invoice_ID = -1;
		int C_BPartner_ID = 0;
		int count = 0;
		int errors = 0;

        
        IDataReader idr = null;
        try
        {
            idr = DataBase.DB.ExecuteReader(sql.ToString(), null, null);

            while (idr.Read())
            {
                int C_Invoice_ID = Utility.Util.GetValueOfInt(idr[0]);
                if (C_Invoice_ID == old_C_Invoice_ID)//Multiple pg Records
                {
                    continue;
                }
                old_C_Invoice_ID = C_Invoice_ID;


                //	Set Language when enabled
                //Language language = Language.getLoginLanguage();
                Language language = Env.GetLoginLanguage(GetCtx());
                //	Base Language
                //    String AD_Language = rs.getString(2);
                String AD_Language = Utility.Util.GetValueOfString(idr[1]);
                //    if (AD_Language != null && "Y".equals(rs.getString(3)))
                if (AD_Language != null && "Y".Equals(idr[2]))
                {
                    //language = Language.getLanguage(AD_Language);
                    language = Language.GetLanguage(AD_Language);
                }


                //    int AD_PrintFormat_ID = rs.getInt(4);
                int AD_PrintFormat_ID = Utility.Util.GetValueOfInt(idr[3]);
                //    int copies = rs.getInt(5);
                int copies = Utility.Util.GetValueOfInt(idr[4]);
                if (copies == 0)
                {
                    copies = 1;
                }
                //    int AD_User_ID = rs.getInt(6);
                int AD_User_ID = Utility.Util.GetValueOfInt(idr[5]);
                //    MUser to = new MUser (getCtx(), AD_User_ID, get_TrxName());
                MUser to = new MUser(GetCtx(), AD_User_ID, Get_TrxName());
                //    String DocumentNo = rs.getString(7);
                String DocumentNo = Utility.Util.GetValueOfString(idr[6]);
                //    C_BPartner_ID = rs.getInt(8);
                C_BPartner_ID = Utility.Util.GetValueOfInt(idr[7]);
                //    //
                //    String documentDir = client.getDocumentDir();
                String documentDir = client.GetDocumentDir();
                //    if (documentDir == null || documentDir.length() == 0)
                if (documentDir == null || documentDir.Length == 0)
                {
                    documentDir = ".";
                }

                if (_EMailPDF && (to.Get_ID() == 0 || to.GetEMail() == null || to.GetEMail().Length == 0))
                {
                    AddLog(C_Invoice_ID, null, null, DocumentNo + " @RequestActionEMailNoTo@");
                    errors++;
                    continue;
                }
                if (AD_PrintFormat_ID == 0)
                {
                    AddLog(C_Invoice_ID, null, null, DocumentNo + " No Print Format");
                    errors++;
                    continue;
                }
                //	Get Format & Data
                if (AD_PrintFormat_ID != old_AD_PrintFormat_ID)
                {
                    format = MPrintFormat.Get(GetCtx(), AD_PrintFormat_ID, false);
                    old_AD_PrintFormat_ID = AD_PrintFormat_ID;
                }

                format.SetLanguage(language);
                format.SetTranslationLanguage(language);
                //    //	query
                Query query = new Query("C_Invoice_Header_v");
                query.AddRestriction("C_Invoice_ID", Query.EQUAL, C_Invoice_ID);

                //	Engine
                //PrintInfo info = new PrintInfo(
                //    DocumentNo,
                //    X_C_Invoice.Table_ID,
                //    C_Invoice_ID,
                //    C_BPartner_ID);
                //info.SetCopies(copies);   
                //ReportEngine re = new ReportEngine(GetCtx(), format, query, info);
              
                Boolean printed = false;
                if (_EMailPDF)
                {
                    String subject = mText.GetMailHeader() + " - " + DocumentNo;
                    EMail email = client.CreateEMail(to.GetEMail(), to.GetName(), subject, null);
                    if (email == null || !email.IsValid())
                    {
                        AddLog(C_Invoice_ID, null, null,
                          DocumentNo + " @RequestActionEMailError@ Invalid EMail: " + to);
                        errors++;
                        continue;
                    }
                    mText.SetUser(to);					//	Context
                    mText.SetBPartner(C_BPartner_ID);	//	Context
                    mText.SetPO(new MInvoice(GetCtx(), C_Invoice_ID, Get_TrxName()));
                    String message = mText.GetMailText(true);
                    if (mText.IsHtml())
                    {
                        email.SetMessageHTML(subject, message);
                    }
                    else
                    {
                        email.SetSubject(subject);
                        email.SetMessageText(message);
                    }
                    //
                    //File invoice = null;
                    FileInfo invoice = null;
                    if (!Ini.IsClient())
                    {
                        //invoice = new File(MInvoice.GetPDFFileName(documentDir, C_Invoice_ID));
                        invoice = new FileInfo(MInvoice.GetPDFFileName(documentDir, C_Invoice_ID));
                    }
                    //File attachment = re.getPDF(invoice);
                    //FileInfo attachment = re.GetPDF(invoice);
                   
                   // log.Fine(to + " - " + attachment);
                   // email.AddAttachment(attachment);

                    //
                    String msg = email.Send();
                    MUserMail um = new MUserMail(mText, GetAD_User_ID(), email);
                    um.Save();
                    if (msg.Equals(EMail.SENT_OK))
                    {
                        AddLog(C_Invoice_ID, null, null,
                          DocumentNo + " @RequestActionEMailOK@ - " + to.GetEMail());
                        count++;
                        printed = true;
                     
                    }
                    else
                    {
                        AddLog(C_Invoice_ID, null, null,
                          DocumentNo + " @RequestActionEMailError@ " + msg
                          + " - " + to.GetEMail());
                        errors++;
                    }
                }
                else
                {
                   // re.Print();
                    count++;
                    printed = true;
                }
                //	Print Confirm
                if (printed)
                {
                    StringBuilder sb = new StringBuilder("UPDATE C_Invoice "
                        + "SET DatePrinted=SysDate, IsPrinted='Y' WHERE C_Invoice_ID=")
                        .Append(C_Invoice_ID);
                    //int no = DataBase.DB.ExecuteUpdateMultiple(sb.ToString(), Get_TrxName());
                    int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
                }
            }	//	for all entries

        }
        catch (Exception e)
        {
            log.Log(Level.SEVERE, sql.ToString(), e);
            //throw new Exception (e);
        }
        finally
        {
            idr.Close();
        }
		//
        if (_EMailPDF)
        {
            return "@Sent@=" + count + " - @Errors@=" + errors;
        }
		return "@Printed@=" + count;
	}	//	doIt

}	//	InvoicePrint

}
