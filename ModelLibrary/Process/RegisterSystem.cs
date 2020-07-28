/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RegisterSystem
 * Purpose        : System Registration
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-feb-2010
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
using System.Net;
using System.Web;
using System.IO;
using System.Security.Policy;
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RegisterSystem : ProcessEngine.SvrProcess
    {
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
			else
            {
				log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// DoIt
	/// </summary>
	/// <returns> Message</returns>
	protected override String DoIt()
	{
		int AD_Registration_ID = GetRecord_ID();
		log.Info("doIt - AD_Registration_ID=" + AD_Registration_ID);
		//	Check Ststem
		MSystem sys = MSystem.Get(GetCtx());
		if (sys.GetName().Equals("?") || sys.GetName().Length<2)
        {
			throw new Exception("Set System Name in System Record");
        }
		if (sys.GetUserName().Equals("?") || sys.GetUserName().Length<2)
        {
			throw new Exception("Set User Name (as in Web Store) in System Record");
        }
		if (sys.GetPassword().Equals("?") || sys.GetPassword().Length<2)
        {
			throw new Exception("Set Password (as in Web Store) in System Record");
        }
		//	Registration
		M_Registration reg = new M_Registration (GetCtx(), AD_Registration_ID, Get_TrxName());
		//	Location
		MLocation loc = null;
		if (reg.GetC_Location_ID() > 0)
		{
			loc = new MLocation (GetCtx(), reg.GetC_Location_ID(), Get_TrxName());
			if (loc.GetCity() == null || loc.GetCity().Length < 2)
            {
				throw new Exception("No City in Address");

            }
		}
		if (loc == null)
        {
			throw new Exception("Please enter Address with City");
        }

		//	Create Query String
		//String enc = WebEnv.ENCODING;
		//	Send GET Request
		StringBuilder urlString = new StringBuilder ("http://www.ViennaAdvantage.com")
			.Append("/wstore/registrationServlet?"); 
		//	System Info
		urlString.Append("Name=").Append(HttpUtility.UrlEncode(sys.GetName(),UTF8Encoding.UTF8))
			.Append("&UserName=").Append(HttpUtility.UrlEncode(sys.GetUserName(),UTF8Encoding.UTF8))
			.Append("&Password=").Append(HttpUtility.UrlEncode(sys.GetPassword(), UTF8Encoding.UTF8));
		//	Registration Info
		if (reg.GetDescription() != null && reg.GetDescription().Length > 0)
        {
			urlString.Append("&Description=").Append(HttpUtility.UrlEncode(reg.GetDescription(),UTF8Encoding.UTF8));
        }
		urlString.Append("&IsInProduction=").Append(reg.IsInProduction() ? "Y" : "N");
		if (reg.GetStartProductionDate() != null)
        {
			urlString.Append("&StartProductionDate=").Append(HttpUtility.UrlEncode(Convert.ToString(reg.GetStartProductionDate()),UTF8Encoding.UTF8));
        }
		urlString.Append("&IsAllowPublish=").Append(reg.IsAllowPublish() ? "Y" : "N")
			.Append("&NumberEmployees=").Append(HttpUtility.UrlEncode(Convert.ToString(reg.GetNumberEmployees()), UTF8Encoding.UTF8))
			.Append("&C_Currency_ID=").Append(HttpUtility.UrlEncode(Convert.ToString(reg.GetC_Currency_ID()), UTF8Encoding.UTF8))
			.Append("&SalesVolume=").Append(HttpUtility.UrlEncode(Convert.ToString(reg.GetSalesVolume()),UTF8Encoding.UTF8));
		if (reg.GetIndustryInfo() != null && reg.GetIndustryInfo().Length > 0)
        {
			urlString.Append("&IndustryInfo=").Append(HttpUtility.UrlEncode(reg.GetIndustryInfo(),UTF8Encoding.UTF8));
        }
		if (reg.GetPlatformInfo() != null && reg.GetPlatformInfo().Length > 0)
        {
			urlString.Append("&PlatformInfo=").Append(HttpUtility.UrlEncode(reg.GetPlatformInfo(),UTF8Encoding.UTF8));
        }
		urlString.Append("&IsRegistered=").Append(reg.IsRegistered() ? "Y" : "N")
			.Append("&Record_ID=").Append(HttpUtility.UrlEncode(Convert.ToString(reg.GetRecord_ID()),UTF8Encoding.UTF8));
		//	Address
		urlString.Append("&City=").Append(HttpUtility.UrlEncode(loc.GetCity(),UTF8Encoding.UTF8))
			.Append("&C_Country_ID=").Append(HttpUtility.UrlEncode(Convert.ToString(loc.GetC_Country_ID()),UTF8Encoding.UTF8));
		//	Statistics
		if (reg.IsAllowStatistics())
		{
			urlString.Append("&NumClient=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM AD_Client")), UTF8Encoding.UTF8))
				.Append("&NumOrg=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM AD_Org")), UTF8Encoding.UTF8))
				.Append("&NumBPartner=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM C_BPartner")),UTF8Encoding.UTF8))
				.Append("&NumUser=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM AD_User")), UTF8Encoding.UTF8))
				.Append("&NumProduct=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM M_Product")), UTF8Encoding.UTF8))
				.Append("&NumInvoice=").Append(HttpUtility.UrlEncode(Convert.ToString(
					DataBase.DB.GetSQLValue(null, "SELECT Count(*) FROM C_Invoice")), UTF8Encoding.UTF8));
		}
		log.Fine(urlString.ToString());
	
		//	Send it
		//URL url = new URL (urlString.toString());
       // Url url=new Url(urlString.ToString());
        Uri url = new Uri(urlString.ToString());
		StringBuilder sb = new StringBuilder();
       	try
		{ 
			//URLConnection uc = url.openConnection();
            //System.IO.StreamReader inn = new System.IO.StreamReader(urlString.ToString());
			//InputStreamReader in = new InputStreamReader(uc.getInputStream());
            WebRequest request = WebRequest.Create(url.ToString());
            WebResponse response = (WebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            byte[] buffer=new byte[stream.Length];
			int c;
            int len = Convert.ToInt32(stream.Length);
            String tempstring = null;
            while ((c=stream.Read(buffer,0,len))>0)
            { 
				//sb.Append((char)c);
                tempstring = Encoding.ASCII.GetString(buffer,0,len);
                sb.Append(tempstring);
            }
            
		}
		catch (Exception e)
		{
			log.Log(Level.SEVERE, "Connect - " + e.ToString());
			throw new Exception("Cannot connect to Server - Please try later");
		}
		//
		String info = sb.ToString();
		log.Info("Response=" + info);
		//	Record at the end
		int index = sb.ToString().IndexOf("Record_ID=");
		if (index != -1)
		{
			try
			{
				int Record_ID = Utility.Util.GetValueOfInt(sb.ToString().Substring(index+10));
				reg.SetRecord_ID(Record_ID);
				reg.SetIsRegistered(true);
				reg.Save();
				//
				info = info.Substring(0, index);
			}
			catch (Exception e)
			{
				log.Log(Level.SEVERE, "Record - ", e);
			}
		}
		
		return info;
	}	//	doIt

}	//	RegisterSystem

}
