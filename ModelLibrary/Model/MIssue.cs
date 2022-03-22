/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIssue
 * Purpose        : Issue Report Model
 * Class Used     : X_AD_Issue
 * Chronological    Development
 * Deepak           27-Jan-2010
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
using System.IO;
using System.Web;
using VAdvantage.Print;
using SecureEngineUtility;
namespace VAdvantage.Model
{
    public class MIssue : X_AD_Issue
    { 
       
    /// <summary>
    /// Create and report issue SessionEndAll
    /// </summary>
    /// <param name="record">log record</param>
    /// <returns>reported issue or null</returns>
   public static MIssue Create(LogRecord record)
	{
		_log.Config(record.message);
		MSystem system = MSystem.Get(Env.GetCtx()); 
		if (!DataBase.DB.IsConnected() 
			|| system == null
			|| !system.IsAutoErrorReport())
			return null;
		//
		MIssue issue = new MIssue(record);
		String error = issue.Report();
		issue.Save();
		if (error != null)
			return null;
		return issue;
	}	//	create
	
	 /// <summary>
        ///	Create from decoded hash map string
	    /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="hexInput">hex string</param>
        /// <returns>issue</returns>
    //@SuppressWarnings("unchecked")
      
	public static MIssue Create(Ctx ctx, String hexInput)
	{
		Dictionary<String,String> hmIn = null;
     
		try		//	encode inn report
		{
			byte[] byteArray = Secure.ConvertHexString(hexInput);
            java.io.ByteArrayInputStream bIn = new java.io.ByteArrayInputStream(byteArray);
            //MemoryStream bln = new MemoryStream(byteArray);

            java.io.ObjectInputStream oIn = new java.io.ObjectInputStream(bIn);
            //BufferedStream oIn = new BufferedStream(bln);
            hmIn = (Dictionary<String,String>)oIn.readObject();
		
		}
		catch (Exception e) 
		{
			_log.Log(Level.SEVERE, "",e);
			return null;
		}

		MIssue issue = new MIssue(ctx, (Dictionary<String,String>)hmIn);
		return issue;
	}	//	create
	
	/**	Logger	*/
	private static VLogger _log = VLogger.GetVLogger (typeof(MIssue).FullName);//.class);
	
	/** Answer Delimiter		*/
	public static String	DELIMITER = "|";
	
	/// <summary>
	/// standard constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="AD_Issue_ID">id</param>
	/// <param name="trxName">trx</param>
	public MIssue (Ctx ctx, int AD_Issue_ID, Trx trxName):base(ctx, AD_Issue_ID, trxName)
	{
		
		if (AD_Issue_ID == 0)
		{
			SetProcessed (false);	// N
			SetSystemStatus(SYSTEMSTATUS_Evaluation);
			try
			{
				Init(ctx);
			}
			catch (IOException e)
			{
                e.StackTrace.ToString();
				//e.getStackTrace();
			}
		}
	}	//	MIssue

	/// <summary>
	/// constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
	/// <param name="trxName">trx</param>
	public MIssue (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		
	}	//	MIssue
	
	/// <summary>
    /// Log Record Constructor
	/// </summary>
	/// <param name="record">record</param>
	public MIssue(LogRecord record):this (Env.GetCtx(), 0, null)
	{
		
		String summary = record.message;
		SetSourceClassName(record.sourceClassName);
		SetSourceMethodName(record.sourceMethodName);
		SetLoggerName(record.GetLoggerName());
		//Throwable t = record.getThrown();
		//if (t != null)
		//{
			//if (summary != null && summary.length() > 0)
				//summary = t.toString() + " " + summary;
			//if (summary == null || summary.length() == 0)
				//summary = t.toString();
			//

            String e=record.message.ToString();//  = record.message.ToString();// (Object)record.message;
			StringBuilder error = new StringBuilder();
            StackTraceElement[] tes=null;// =(StackTraceElement[])e.StackTrace;// t.getStackTrace();
			int count = 0;
			for (int i = 0; i < e.Length; i++)
			{
				StackTraceElement element = tes[i];
				String s = element.ToString();
				if (s.IndexOf("vienna") != -1)
				{
					error.Append(s).Append("\n");
					if (count == 0)
					{
						String source = element.GetClassName()
							+ "." + element.GetMethodName();
						SetSourceClassName(source);
						SetLineNo(element.GetLineNumber());
					}
					count++;
				}
				if (count > 5 || error.Length > 2000)
					break;
			}
			SetErrorTrace(error.ToString());
			//	Stack
        			//CharArrayWriter cWriter = new CharArrayWriter();
			//PrintWriter pWriter = new PrintWriter(cWriter);
			//t.printStackTrace(pWriter);
			//setStackTrace(cWriter.toString());
		
		//if (summary == null || summary.length() == 0)
			//summary = "??";
		//setIssueSummary(summary);
		SetRecord_ID(1);
	}	//	MIssue

	/// <summary>
	/// HashMap Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="hmIn">hash map</param>
	public MIssue(Ctx ctx,Dictionary<String,String> hmIn):base(ctx, 0, null)
	{
		
		Load(hmIn);
		SetRecord_ID(0);
	}	//	MIssue

	/// <summary>
    /// Initialize
	/// </summary>
	/// <param name="ctx">context</param>
	private void Init(Ctx ctx) 
	{
		MSystem system = MSystem.Get(ctx);
		SetName(system.GetName());
		SetUserName(system.GetUserName());
		SetDBAddress(system.GetDBAddress(true));
		SetSystemStatus(system.GetSystemStatus());
		SetReleaseNo(system.GetReleaseNo());	//	DB
		
		//setDatabaseInfo(DataBase.getDatabaseInfo());
		
		if (system.IsAllowStatistics())
		{
			SetStatisticsInfo(system.GetStatisticsInfo(true));
			SetProfileInfo(system.GetProfileInfo(true));
		}
	}	//	init
	
	/** Length of Info Fields			*/
	private static  int	INFOLENGTH = 2000;
	
	/// <summary>
	///	Set Issue Summary.Truncate it to 2000 char
	/// </summary>
    /// <param name="IssueSummary">summary</param>
	public new void SetIssueSummary (String IssueSummary)
	{
		if (IssueSummary == null)
			return;
		IssueSummary = IssueSummary.Replace("java.lang.", "");
		IssueSummary = IssueSummary.Replace("java.sql.", "");
		if (IssueSummary.Length > INFOLENGTH)
			IssueSummary = IssueSummary.Substring(0,INFOLENGTH-1);
		base.SetIssueSummary (IssueSummary);
	}	//	setIssueSummary
	
	/// <summary>
    /// 	Set Stack Trace.
	/// </summary>
	/// <param name="StackTrace">stack</param>
	public new void SetStackTrace (String StackTrace)
	{
		if (StackTrace == null)
			return;
		StackTrace = StackTrace.Replace("java.lang.", "");
		StackTrace = StackTrace.Replace("java.sql.", "");
		if (StackTrace.Length > INFOLENGTH)
			StackTrace = StackTrace.Substring(0,INFOLENGTH-1);
		base.SetStackTrace (StackTrace);
	}	//	setStackTrace
	
	
	/// <summary>
    /// Set Error Trace.
	/// </summary>
	/// <param name="ErrorTrace">trace</param>
	public new void SetErrorTrace (String ErrorTrace)
	{
		if (ErrorTrace == null)
			return;
		ErrorTrace = ErrorTrace.Replace("java.lang.", "");
		ErrorTrace = ErrorTrace.Replace("java.sql.", "");
		if (ErrorTrace.Length > INFOLENGTH)
			ErrorTrace = ErrorTrace.Substring(0,INFOLENGTH-1);
		base.SetErrorTrace (ErrorTrace);
	}	//	setErrorTrace

	/// <summary>
    /// Add Comments
	/// </summary>
	/// <param name="Comments">comments</param>
	public void AddComments (String Comments)
	{
		if (Comments == null || Comments.Length == 0)
			return;
		String old = GetComments();
		if (old == null || old.Length == 0)
			SetComments (Comments);
		else if (!old.Equals(Comments) 
			&& old.IndexOf(Comments) == -1)		//	 something new
			SetComments (Comments + " | " + old);
	}	//	addComments
	
	/// <summary>
	/// Set Comments.	Truncate it to 2000 char
	/// </summary>
	/// <param name="Comments">comments</param>
	public new void SetComments (String Comments)
	{
		if (Comments == null)
			return;
		if (Comments.Length > INFOLENGTH)
			Comments = Comments.Substring(0,INFOLENGTH-1);
		base.SetComments (Comments);
	}	//	setComments
	
	/// <summary>
	/// Set ResponseText.Truncate it to 2000 char
	/// </summary>
	/// <param name="ResponseText">ResponseText</param>
	public new void SetResponseText (String ResponseText)
	{
		if (ResponseText == null)
			return;
		if (ResponseText.Length > INFOLENGTH)
			ResponseText = ResponseText.Substring(0,INFOLENGTH-1);
		base.SetResponseText(ResponseText);
	}	//	setResponseText

	/// <summary>
    /// 	Process Request.
	/// </summary>
	/// <returns>answer</returns>
	public String Process()
	{
		MIssueProject.Get(this);	//	sets also Asset
		MIssueSystem.Get(this);
		MIssueUser.Get(this);
		//
	//	setR_IssueKnown_ID(0);
	//	setR_Request_ID(0);
		return CreateAnswer();
	}	//	process
	
	/// <summary>
    ///  Create Answer to send to User
	/// </summary>
	/// <returns>answer</returns>
	public String CreateAnswer()
	{
		StringBuilder sb = new StringBuilder();
		if (GetA_Asset_ID() != 0)
			sb.Append("Sign up for support at http://www.viennaadvantage.com to receive answers.");
		else
		{
			if (GetR_IssueKnown_ID() != 0)
				sb.Append("Known Issue\n");
			if (GetR_Request_ID() != 0)
				sb.Append("Request: ")
					.Append(GetRequest().GetDocumentNo())
					.Append("\n");
		}
		return sb.ToString();
	}	//	createAnswer
	
	/// <summary>
    /// 	Get Request
	/// </summary>
	/// <returns>request or null</returns>
	public X_R_Request GetRequest()
	{
		if (GetR_Request_ID() == 0)
			return null;
		return new X_R_Request(GetCtx(), GetR_Request_ID(), null);
	}	//	getRequestDocumentNo

	/// <summary>
    /// Get Request Document No
	/// </summary>
    /// <returns> Request Document No</returns>
	public new String GetRequestDocumentNo()
	{
		if (GetR_Request_ID() == 0)
			return "";
		X_R_Request r = GetRequest();
		return r.GetDocumentNo();
	}	//	getRequestDocumentNo
	
	/// <summary>
	/// Get System Status
	/// </summary>
    /// <returns>system status</returns>
	public new String GetSystemStatus ()
	{
		String s = base.GetSystemStatus ();
		if (s == null || s.Length == 0)
			s = SYSTEMSTATUS_Evaluation;
		return s;
	}	//	getSystemStatus
	
	
	/// <summary>
    /// Report/Update Issue.
	/// </summary>
	/// <returns>error message</returns>
	public String Report()
	{
        if (true)
        {
            return "-";
        }
        //StringBuilder parameter = new StringBuilder("?");
        //if (GetRecord_ID() == 0)	//	don't report
        //    return "ID=0";
        //if (GetRecord_ID() ==1)	//	new
        //{
        //    parameter.Append("ISSUE=");
        //    //Dictionary<String,String> htOut = Get_HashMap();
        //   HashMap<String,String> htOut=Get_HashMap();
			
        //    try		//	deserializing inn create
        //    {
        //       //java.io.ByteArrayOutputStream bOut = new java.io.ByteArrayOutputStream();             
        //       //java.io.ObjectOutput oOut = new java.io.ObjectOutputStream(bOut);

        //       MemoryStream ms = new MemoryStream(htOut.Count);
               
        //       // oOut.writeObject(htOut);                
        //       ms.Flush();
        //       // oOut.flush();
        //       // String hexString = Secure.ConvertToHexString(bOut.toByteArray());
        //       String hexString = Secure.ConvertToHexString(ms.ToArray());
        //        parameter.Append(hexString);
        //    }
        //    catch (Exception e) 
        //    {
        //        log.Severe(e.Message);
        //        return "New-" + e.Message;
        //    }
        //}
        //else	//	existing
        //{
        //    try
        //    {
        //        parameter.Append("RECORDID=").Append(GetRecord_ID());
        //        parameter.Append("&DBADDRESS=").Append(HttpUtility.UrlEncode(GetDBAddress(), UTF8Encoding.UTF8));// "UTF-8"));
        //        parameter.Append("&COMMENTS=").Append(HttpUtility.UrlEncode(GetComments(), UTF8Encoding.UTF8));// "UTF-8"));
        //    }
        //    catch (Exception e) 
        //    {
        //        log.Severe(e.Message);
        //        return "Update-" + e.Message;
        //    }
        //}
		
        //StreamReader inn = null;
        //String target = "http://dev1/wstore/issueReportServlet";
        //try		//	Send GET Request
        //{
        //    StringBuilder urlString = new StringBuilder(target)
        //        .Append(parameter);
        //    //Url url = new Url(urlString.ToString());
        //    Uri url = new Uri(urlString.ToString());
        //    //URLConnection uc = url.openConnection();
        //    System.Net.WebRequest request = System.Net.WebRequest.Create(url);
        //    //inn = new StreamReader(uc.getInputStream());
        //    System.Net.WebResponse response = (System.Net.WebResponse)request.GetResponse();
        //    //Stream stream = response.GetResponseStream();
        //    inn = new StreamReader(response.GetResponseStream());
        
        //}
        //catch (Exception e)
        //{
        //    String msg = "Cannot connect to http://" + target; 
        //    if (e is FileNotFoundException)// || e is Exception)
        //        msg += "\nServer temporarily down - Please try again later";
        //    else
        //    {
        //        msg += "\nCheck connection - " + e.Message;
        //        log.Log(Level.FINE, msg);
        //    }
        //    return msg;
        //}
        //return ReadResponse(inn);
	}	//	report
	
	/// <summary>
	///	Read Response
	/// </summary>
	/// <param name="inn">input stream</param>
	/// <returns>error message</returns>
	private String ReadResponse(StreamReader inn)
	{
		StringBuilder sb = new StringBuilder();
		int Record_ID = 0;
		String ResponseText = null;
		String RequestDocumentNo = null;
		try		//	Get Answer
		{
			int c;
			while ((c = inn.Read()) != -1)
				sb.Append((char)c);
			inn.Close();
			log.Fine(sb.ToString());
			String clear =HttpUtility.UrlEncode(sb.ToString(),UTF8Encoding.UTF8);//  "UTF-8");
			log.Fine(clear);
			//	Interpret Data
			StringTokenizer st = new StringTokenizer(clear, DELIMITER);
			while (st.HasMoreElements())
			{
				String pair = st.NextToken();
				try
				{
					int index = pair.IndexOf("=");
					if (pair.StartsWith("RECORDID="))
					{
						String info = pair.Substring(index+1);
						Record_ID =Utility.Util.GetValueOfInt(info);
					}
					else if (pair.StartsWith("RESPONSE="))
						ResponseText = pair.Substring(index+1);
					else if (pair.StartsWith("DOCUMENTNO="))
						RequestDocumentNo = pair.Substring(index+1);
				}
				catch (Exception e)
				{
					log.Warning(pair + " - " + e.Message);
				}
			}
		}
		catch (Exception ex)
		{
			log.Log(Level.FINE, "", ex);
			return "Reading-" + ex.Message;
		}

		if (Record_ID != 0)
			SetRecord_ID(Record_ID);
		if (ResponseText != null)
			SetResponseText(ResponseText);
		if (RequestDocumentNo != null)
			SetRequestDocumentNo(RequestDocumentNo);
		return null;
	}	//	readResponse
	
	/// <summary>
    /// 	String Representation
	/// </summary>
	/// <returns>info</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MIssue[");
		sb.Append (Get_ID())
			.Append ("-").Append (GetIssueSummary())
			.Append (",Record=").Append (GetRecord_ID())
			.Append ("]");
		return sb.ToString ();
	}	//	toString
	
	
}	//	MIssue

}