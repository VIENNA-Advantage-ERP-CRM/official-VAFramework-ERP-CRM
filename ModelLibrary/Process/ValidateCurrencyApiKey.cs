/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ValidateCurrencyApiKey
 * Purpose        : Validate Currency Conversion Api Key
 * Chronological    Development
 * Bharat           25-Nov-2019
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.ServiceModel;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace VAdvantage.Process
{
    public class ValidateCurrencyApiKey : SvrProcess
    {
        #region Variables       
        int C_CurrencySource_ID = 0;
        string sql = "";
        DataSet ds = null;
        #endregion

        protected override void Prepare()
        {
            C_CurrencySource_ID = GetRecord_ID();
        }

        /// <summary>
        /// Validate Currency Conversion Api Key
        /// </summary>
        /// <returns>Message as String</returns>
        protected override string DoIt()
        {
            string status = "OK";
            try
            {
                // Get URL and Api Key from Currency Source
                sql = "SELECT URL, ApiKey FROM C_CurrencySource WHERE C_CurrencySource_ID=" + C_CurrencySource_ID;
                ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string url = Util.GetValueOfString(ds.Tables[0].Rows[0]["URL"]);
                    string apikey = Util.GetValueOfString(ds.Tables[0].Rows[0]["ApiKey"]);
                    if (url.ToUpper().Contains("XE.COM"))
                    {
                        if (String.IsNullOrEmpty(apikey))
                        {
                            return "Invalid Key";
                        }

                        string newUrl = @"https://xecdapi.xe.com/v1/account_info/";
                        WebRequest myReq = WebRequest.Create(newUrl);
                        CredentialCache mycache = new CredentialCache();
                        myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apikey));
                        WebResponse wr = myReq.GetResponse();
                        Stream receiveStream = wr.GetResponseStream();
                        StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                        string content = reader.ReadToEnd();
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(content);
                        if (data != null)
                        {
                            return status;
                        }
                        else
                        {
                            return "Invalid Key";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return status;
        }
    }
}
