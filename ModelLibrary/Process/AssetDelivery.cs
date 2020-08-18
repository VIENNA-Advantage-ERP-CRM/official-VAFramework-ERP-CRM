/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AssetDelivery
 * Purpose        : Deliver Assets Electronically
 * Class Used     : SvrProcess
 * Chronological    Development
 * Deepak           13-July-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class AssetDelivery:SvrProcess
    {
     	private VAdvantage.Model.MClient		_client = null;

	private int			_A_Asset_Group_ID = 0;
	private int			_M_Product_ID = 0;
	private int			_C_BPartner_ID = 0;
	private int			_A_Asset_ID = 0;
	private DateTime?	_GuaranteeDate = null;
	private int			_NoGuarantee_MailText_ID = 0;
	private bool		_AttachAsset = false;
	//
	private VAdvantage.Model.MMailText	_MailText = null;


	/// <summary>
	/// Prepare - e.g., get Parameters.
	/// </summary>
	protected override void Prepare()
	{
		ProcessInfoParameter[] para = GetParameter();
		foreach (ProcessInfoParameter element in para) 
        {
			String name = element.GetParameterName();
			if (element.GetParameter() == null)
            {
				;
            }
			else if (name.Equals("A_Asset_Group_ID"))
				_A_Asset_Group_ID = element.GetParameterAsInt();
			else if (name.Equals("M_Product_ID"))
				_M_Product_ID = element.GetParameterAsInt();
			else if (name.Equals("C_BPartner_ID"))
				_C_BPartner_ID = element.GetParameterAsInt();
			else if (name.Equals("A_Asset_ID"))
				_A_Asset_ID = element.GetParameterAsInt();
			else if (name.Equals("GuaranteeDate"))
				_GuaranteeDate = (DateTime?)element.GetParameter();
			else if (name.Equals("NoGuarantee_MailText_ID"))
				_NoGuarantee_MailText_ID = element.GetParameterAsInt();
			else if (name.Equals("AttachAsset"))
				_AttachAsset = "Y".Equals(element.GetParameter());
			else
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
		}
		if (_GuaranteeDate == null)
			_GuaranteeDate =DateTime.Now;//(  new Timestamp (System.currentTimeMillis());
		//
		_client = VAdvantage.Model.MClient.Get(GetCtx());
	}	//	prepare

	/// <summary>
	/// Perrform process.	 
	/// </summary>
	/// <returns>Message to be translated</returns>
	protected override String DoIt()
	{
		log.Info("");
        long start = CommonFunctions.CurrentTimeMillis();// System.currentTimeMillis();

		//	Test
		if (_client.GetSmtpHost() == null || _client.GetSmtpHost().Length == 0)
			throw new Exception ("No Client SMTP Info");
		if (_client.GetRequestEMail() == null)
			throw new Exception ("No Client Request User");

		//	Asset selected
		if (_A_Asset_ID != 0)
		{
			String msg = DeliverIt(_A_Asset_ID);
			AddLog (_A_Asset_ID, null, null, msg);
			return msg;
		}
		//
		StringBuilder sql = new StringBuilder ("SELECT A_Asset_ID, GuaranteeDate "
			+ "FROM A_Asset a"
			+ " INNER JOIN M_Product p ON (a.M_Product_ID=p.M_Product_ID) "
			+ "WHERE ");
		if (_A_Asset_Group_ID != 0 && _A_Asset_Group_ID!=-1)
			sql.Append("a.A_Asset_Group_ID=").Append(_A_Asset_Group_ID).Append(" AND ");
		if (_M_Product_ID != 0)
			sql.Append("p.M_Product_ID=").Append(_M_Product_ID).Append(" AND ");
		if (_C_BPartner_ID != 0)
			sql.Append("a.C_BPartner_ID=").Append(_C_BPartner_ID).Append(" AND ");
		String s = sql.ToString();
		if (s.EndsWith(" WHERE "))
			throw new Exception ("@RestrictSelection@");
		//	No mail to expired
		if (_NoGuarantee_MailText_ID == 0)
		{
			sql.Append("TRUNC(GuaranteeDate,'DD') >= ").Append(DB.TO_DATE(_GuaranteeDate, true));
			s = sql.ToString();
		}
		//	Clean up
		if (s.EndsWith(" AND "))
			s = sql.ToString().Substring(0, sql.Length-5);
		//		
		int count = 0;
		int errors = 0;
		int reminders = 0;
        IDataReader idr = null;
        try
        {
            idr = DB.ExecuteReader(s);
            while (idr.Read())
            {
                int A_Asset_ID = Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                DateTime? GuaranteeDate = Util.GetValueOfDateTime(idr[1]);// rs.getTimestamp(2);

                //	Guarantee Expired
                //if (GuaranteeDate.Value != null && GuaranteeDate.before(m_GuaranteeDate))
                if (GuaranteeDate.Value != null && GuaranteeDate.Value < _GuaranteeDate)
                {
                    if (_NoGuarantee_MailText_ID != 0)
                    {
                        SendNoGuaranteeMail(A_Asset_ID, _NoGuarantee_MailText_ID, Get_Trx());
                        reminders++;
                    }
                }
                else	//	Guarantee valid
                {
                    String msg = DeliverIt(A_Asset_ID);
                    AddLog(A_Asset_ID, null, null, msg);
                    if (msg.StartsWith("** "))
                        errors++;
                    else
                        count++;
                }
            }
            idr.Close();
        }
        catch (Exception e)
        {
            if (idr != null)
            {
                idr.Close();
            }
            log.Log(Level.SEVERE, s, e);
        }	

		log.Info("Count=" + count + ", Errors=" + errors + ", Reminder=" + reminders
			+ " - " + (CommonFunctions.CurrentTimeMillis() -start) + "ms");
		return "@Sent@=" + count + " - @Errors@=" + errors;
	}	//	doIt


	/// <summary>
	/// Send No Guarantee EMail	 
	/// </summary>
	/// <param name="A_Asset_ID">asset</param>
	/// <param name="R_MailText_ID">mail to send</param>
	/// <param name="trxName">trx</param>
	/// <returns>message - delivery errors start with **</returns>
	private String SendNoGuaranteeMail (int A_Asset_ID, int R_MailText_ID, Trx trxName)
	{
		MAsset asset = new MAsset (GetCtx(), A_Asset_ID, trxName);
		if (asset.GetAD_User_ID() == 0)
			return "** No Asset User";
		VAdvantage.Model.MUser user = new VAdvantage.Model.MUser (GetCtx(), asset.GetAD_User_ID(), Get_Trx());
		if (user.GetEMail() == null || user.GetEMail().Length == 0)
			return "** No Asset User Email";
		if (_MailText == null || _MailText.GetR_MailText_ID() != R_MailText_ID)
			_MailText = new VAdvantage.Model.MMailText (GetCtx(), R_MailText_ID, Get_Trx());
		if (_MailText.GetMailHeader() == null || _MailText.GetMailHeader().Length == 0)
			return "** No Subject";

		//	Create Mail
		EMail email = _client.CreateEMail(user.GetEMail(), user.GetName(), null, null);
		if (email == null)
			return "** Invalid: " + user.GetEMail();
		_MailText.SetPO(user);
		_MailText.SetPO(asset);
		String message = _MailText.GetMailText(true);
		if (_MailText.IsHtml())
			email.SetMessageHTML(_MailText.GetMailHeader(), message);
		else
		{
			email.SetSubject (_MailText.GetMailHeader());
			email.SetMessageText (message);
		}
		String msg = email.Send();
		new MUserMail(_MailText, asset.GetAD_User_ID(), email).Save();
		if (!EMail.SENT_OK.Equals(msg))
			return "** Not delivered: " + user.GetEMail() + " - " + msg;
		//
		return user.GetEMail();
	}	//	sendNoGuaranteeMail

	
	/// <summary>
	/// Deliver Asset	
	/// </summary>
	/// <param name="A_Asset_ID">asset</param>
	/// <returns>message - delivery errors start with **</returns>
	private String DeliverIt (int A_Asset_ID)
	{
		log.Fine("A_Asset_ID=" + A_Asset_ID);
		long start =CommonFunctions.CurrentTimeMillis();
		//
		MAsset asset = new MAsset (GetCtx(), A_Asset_ID, Get_Trx());
		if (asset.GetAD_User_ID() == 0)
			return "** No Asset User";
		VAdvantage.Model.MUser user = new VAdvantage.Model.MUser (GetCtx(), asset.GetAD_User_ID(), Get_Trx());
		if (user.GetEMail() == null || user.GetEMail().Length == 0)
			return "** No Asset User Email";
		if (asset.GetProductR_MailText_ID() == 0)
			return "** Product Mail Text";
		if (_MailText == null || _MailText.GetR_MailText_ID() != asset.GetProductR_MailText_ID())
			_MailText = new VAdvantage.Model.MMailText (GetCtx(), asset.GetProductR_MailText_ID(), Get_Trx());
		if (_MailText.GetMailHeader() == null || _MailText.GetMailHeader().Length == 0)
			return "** No Subject";

		//	Create Mail
		EMail email = _client.CreateEMail(user.GetEMail(), user.GetName(), null, null);
		if (email == null || !email.IsValid())
		{
			asset.SetHelp(asset.GetHelp() + " - Invalid EMail");
			asset.SetIsActive(false);
			return "** Invalid EMail: " + user.GetEMail() + " - " + email;
		}
		if (_client.IsSmtpAuthorization())
			email.CreateAuthenticator(_client.GetRequestUser(), _client.GetRequestUserPW());
		_MailText.SetUser(user);
		_MailText.SetPO(asset);
		String message = _MailText.GetMailText(true);
		if (_MailText.IsHtml() || _AttachAsset)
			email.SetMessageHTML(_MailText.GetMailHeader(), message);
		else
		{
			email.SetSubject (_MailText.GetMailHeader());
			email.SetMessageText (message);
		}
		if (_AttachAsset)
		{
			MProductDownload[] pdls = asset.GetProductDownloads();
			if (pdls != null)
			{
				foreach (MProductDownload element in pdls) 
                { 
					//URL url = element.getDownloadURL(m_client.getDocumentDir());
                    Url url = element.GetDownloadURL(_client.GetDocumentDir());
					if (url != null)
						email.AddAttachment(url.Value);
				}
			}
			else
				log.Warning("No DowloadURL for A_Asset_ID=" + A_Asset_ID);
		}
		String msg = email.Send();
		new MUserMail(_MailText, asset.GetAD_User_ID(), email).Save();
		if (!EMail.SENT_OK.Equals(msg))
			return "** Not delivered: " + user.GetEMail() + " - " + msg;

		MAssetDelivery ad = asset.ConfirmDelivery(email, user.GetAD_User_ID());
		ad.Save();
		asset.Save();
		//
		log.Fine((CommonFunctions.CurrentTimeMillis() -start) + " ms");
		//	success
		return user.GetEMail() + " - " + asset.GetProductVersionNo();
	}
    }
}
