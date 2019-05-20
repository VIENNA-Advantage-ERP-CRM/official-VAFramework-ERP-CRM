/********************************************************
 * Project Name   : VIS
 * Class Name     : SmsModel
 * Purpose        : Used to perform server side tasks related to  SMS...
 * Chronological    Development
 * Karan            
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class SmsModel
    {
        Ctx ct = null;
        public SmsModel(Ctx ctx)
        {
            ct = ctx;
        }


        /// <summary>
        /// Used to send SMS 
        /// </summary>
        /// <param name="smss"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public SmsResponse Send(List<SmsHelper> smss, string format)
        {
            StringBuilder result = new StringBuilder("");
            SmsResponse reponse = new SmsResponse();
            reponse.Response = new List<SmsResponsehelper>();
            IDataReader idr = null;
            idr = VAdvantage.DataBase.DB.ExecuteReader("select * from ad_smsconfiguration WHERE isactive='Y'");
            DataTable dt = new DataTable();
            dt.Load(idr);
            idr.Close();
            if (dt.Rows.Count == 0)
            {
                reponse.ResultMessage = "SmsConfigurationNotFound";
                return reponse;
            }


            //create URL with configuration from SMS Configuration winow....

            string strUrl = dt.Rows[0]["url"].ToString() + "?" + dt.Rows[0]["userKeyword"].ToString() + "=" +
              dt.Rows[0]["username"].ToString() + "&" + dt.Rows[0]["PasswordKeyword"].ToString() + "=" +
              dt.Rows[0]["password"].ToString() + "&" +
              dt.Rows[0]["senderKeyword"].ToString() + "=" + dt.Rows[0]["sender"].ToString() + "&" +
              dt.Rows[0]["messageKeyword"].ToString() + "=" +
              "@@Messag@@" + "&" + dt.Rows[0]["MobilenumberKeyword"].ToString() + "=" + "@@Mobile@@" + "&";
            strUrl += dt.Rows[0]["priorityKeyword"].ToString() + "=" + dt.Rows[0]["priorityValue"].ToString();
            if (dt.Rows[0]["unicodeValue"].ToString().Length > 0 || dt.Rows[0]["dndValue"].ToString().Length > 0)
            {
                strUrl += "&" + dt.Rows[0]["dndKeyword"].ToString() + "=" + dt.Rows[0]["dndValue"].ToString() +
                    "&" + dt.Rows[0]["unicodeKeyword"].ToString() + "=" + dt.Rows[0]["unicodeValue"].ToString();
            }

            for (int i = 0; i < smss.Count; i++)
            {
                SmsHelper mbNumber = smss[i];
                string message = format;

                if (mbNumber.Body != null && mbNumber.Body.Count > 0)
                {
                    List<string> keysss = mbNumber.Body.Keys.ToList();
                    for (int q = 0; q < keysss.Count; q++)
                    {
                        message = message.Replace(keysss[q], mbNumber.Body[keysss[q]]);
                    }
                }


                List<string> subNumberList = mbNumber.MobileNumbers;


                //send mail to each number one by one....
                for (int j = 0; j < subNumberList.Count(); j++)
                {
                    try
                    {


                        message = Replace(message);
                        string repMob = Replace(subNumberList[j]);
                        string uri = strUrl.Replace("@@Messag@@", message);
                        uri = uri.Replace("@@Mobile@@", repMob);
                        // uri += mbNumber;

                        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                        HttpWebResponse resp = (HttpWebResponse)webReq.GetResponse();//send sms
                        StreamReader responseReader = new StreamReader(resp.GetResponseStream());//read the response 
                        string res = responseReader.ReadToEnd();//get result
                        reponse.Response.Add(new SmsResponsehelper { mobileNumber = subNumberList[j], response = res });
                        responseReader.Close();
                        resp.Close();
                    }
                    catch (Exception e)
                    {
                        reponse.Response.Add(new SmsResponsehelper { mobileNumber = subNumberList[j], response = e.Message });
                    }
                }

            }
            return reponse;
        }

        private string Replace(string str)
        {
            StringBuilder replaceSpecilChar = new StringBuilder();
            replaceSpecilChar.Append(str);
            // replaceSpecilChar.Replace("%", "%25");
            replaceSpecilChar.Replace("#", "%23");
            replaceSpecilChar.Replace("$", "%24");
            replaceSpecilChar.Replace("&", "%26");
            replaceSpecilChar.Replace("+", "%2B");
            replaceSpecilChar.Replace(",", "%2C");
            replaceSpecilChar.Replace(":", "%3A");
            replaceSpecilChar.Replace(";", "%3B");
            replaceSpecilChar.Replace("=", "%3D");
            replaceSpecilChar.Replace("?", "%3F");
            replaceSpecilChar.Replace("@", "%40");
            replaceSpecilChar.Replace("<", "%3C");
            replaceSpecilChar.Replace(">", "%3E");
            replaceSpecilChar.Replace("{", "%7B");
            replaceSpecilChar.Replace("}", "%7D");
            replaceSpecilChar.Replace("|", "%7C");
            replaceSpecilChar.Replace("\\", "%5C");
            replaceSpecilChar.Replace("^", "%5E");
            replaceSpecilChar.Replace("~", "%7E");
            replaceSpecilChar.Replace("[", "%5B");
            replaceSpecilChar.Replace("]", "%5D");
            replaceSpecilChar.Replace("`", "%60");

            return replaceSpecilChar.ToString();
        }
    }

    public class SmsHelper
    {
        public List<string> MobileNumbers { get; set; }
        public Dictionary<string, string> Body { get; set; }
    }

    public class SmsResponse
    {
        public string ResultMessage { get; set; }
        public List<SmsResponsehelper> Response { get; set; }
    }

    public class SmsResponsehelper
    {
        public string mobileNumber { get; set; }
        public string response { get; set; }
    }
}