﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : GetCurrencyConversionRate
 * Purpose        : Currency Conversion Rate
 * Chronological    Development
 * Arpit            08-July-2016
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
using System.IO;


namespace ViennaAdvantage.Process
{
    public class GetCurrencyConversionRate : SvrProcess
    {

        #region Variables
        List<CurrencyProp> _lstCurr = null;
        int C_CurrencySource_ID = 0;
        string myCurrency = "";
        int myCurrencyID = 0;
        string sql = "";
        DataSet ds = new DataSet();
        #endregion

        protected override void Prepare()
        {
            //ProcessInfoParameter[] para = GetParameter();
            //for (int i = 0; i < para.Length; i++)
            //{
            //    String name = para[i].GetParameterName();
            //    //	log.fine("prepare - " + para[i]);
            //    if (para[i].GetParameter() == null)
            //    {
            //        ;
            //    }
            //    else if (name.Equals("C_CurrencySource_ID"))
            //    {
            //        C_CurrencySource_ID = para[i].GetParameterAsInt();
            //    }
            //    else
            //    {
            //        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            //    }
            //}
        }

        protected override string DoIt()
        {
            string status = "OK";
           // Trx trx = Get_Trx();
            ds.Clear();
            try
            {

                ds = DB.ExecuteDataset(@"SELECT distinct cl.AD_Client_ID,
                                          cl.AD_Org_ID,
                                          cl.CurrencyRateUpdateFrequency,
                                          acct.C_Currency_ID
                                          ,cr.ISO_Code,cl.C_CurrencySource_ID
                                        FROM ad_client cl
                                        INNER JOIN AD_CLientinfo ci
                                        ON ci.ad_client_ID=cl.ad_client_ID
                                        INNER JOIN C_AcctSchema acct
                                        ON acct.C_AcctSchema_ID             =ci.C_AcctSchema1_ID
                                        Left Join C_Currency cr on cr.C_Currency_ID=acct.C_Currency_ID
                                        WHERE cl.ad_client_Id!              =0 AND cl.UpdateCurrencyRate='A' AND cl.IsMultiCurrency='Y'
                                        AND cl.currencyrateupdatefrequency IS NOT NULL");

                //                                                        Where ci.AD_CLient_ID= " + GetAD_Client_ID());
                //in this DataSet we'll get CLient's Base Currency & the Currency ID
                // int clientCount = ds.Tables[0].Rows.Count;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    _lstCurr = new List<CurrencyProp>();
                }
                else
                {
                    return VAdvantage.Utility.Msg.GetMsg(GetCtx(), "NoMultiCurrencySettingsFound");
                }

                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    CurrencyProp _curr = new CurrencyProp();
                    _curr.baseCurrency = Convert.ToString(ds.Tables[0].Rows[j]["ISO_Code"]);
                    _curr.baseCurrencyID = Convert.ToInt32(ds.Tables[0].Rows[j]["C_Currency_ID"]);
                    _curr.frequency = Convert.ToString(ds.Tables[0].Rows[j]["CurrencyRateUpdateFrequency"]);
                    _curr.AD_Client_ID = Convert.ToInt32(ds.Tables[0].Rows[j]["AD_Client_ID"]);
                    _curr.AD_Org_ID = Convert.ToInt32(ds.Tables[0].Rows[j]["AD_Org_ID"]);
                    _curr.CurrencySource = Convert.ToInt32(ds.Tables[0].Rows[j]["C_CurrencySource_ID"]);
                    _lstCurr.Add(_curr);
                }
                ds.Clear();
                sql = @"Select Cur.ISO_Code, Cur.C_Currency_ID From C_Currency Cur  Where Cur.IsMyCurrency='Y' And Cur.IsActive='Y' ";
                ds = DB.ExecuteDataset(sql); // Here we get all currencies in which our Client is in dealing with 
                //String frequency;// = DB.ExecuteScalar("Select currencyrateupdatefrequency from AD_Client where IsActive='Y' AND currencyrateupdatefrequency is not null").ToString();
                // getting the frequency(TimePeriod) for the converted rate

                if (ds != null)
                {
                    for (Int32 k = 0; k < _lstCurr.Count; k++)
                    {

                        string currencySourceName = DB.ExecuteScalar("Select url from C_CurrencySource Where C_CurrencySource_ID=" + _lstCurr[k].CurrencySource).ToString();

                        //int defaultconversionType = 0;
                        //try
                        //{
                        //    defaultconversionType = Convert.ToInt32(DB.ExecuteScalar(@"SELECT C_ConversionType_id, Surchargepercentage,Surchargevalue FROM c_conversiontype WHERE autocalculate='Y' AND isactive   ='Y'"));
                        //}
                        //catch
                        //{

                        //}

                        DataSet dsConversion = DB.ExecuteDataset(@"SELECT C_ConversionType_id, Surchargepercentage,Surchargevalue,CurrencyRateUpdateFrequency FROM c_conversiontype WHERE isautocalculate='Y' AND isactive   ='Y'");
                        if (dsConversion != null && dsConversion.Tables[0].Rows.Count > 0)
                        {
                            for (int x = 0; x < dsConversion.Tables[0].Rows.Count; x++)
                            {
                                int defaultconversionType = 0;
                                defaultconversionType = Convert.ToInt32(dsConversion.Tables[0].Rows[x]["C_ConversionType_id"]);

                                MConversionRate conversion = null;
                                Decimal rate1 = 0;
                                Decimal rate2 = 0;
                                Decimal one = new Decimal(1.0);

                                string updateFrequency = _lstCurr[k].frequency;

                                if (dsConversion.Tables[0].Rows[x]["CurrencyRateUpdateFrequency"] != null && dsConversion.Tables[0].Rows[x]["CurrencyRateUpdateFrequency"] != DBNull.Value
                                                   && Convert.ToString(dsConversion.Tables[0].Rows[x]["CurrencyRateUpdateFrequency"]) != "")
                                {
                                    updateFrequency = Convert.ToString(dsConversion.Tables[0].Rows[x]["CurrencyRateUpdateFrequency"]);
                                }


                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    myCurrency = ds.Tables[0].Rows[i]["ISO_Code"].ToString();
                                    myCurrencyID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]);
                                    sql = String.Empty;
                                    sql = @"  Select ValidTo from C_Conversion_Rate  where IsActive='Y' AND C_ConversionType_id=" + defaultconversionType + " AND  C_Currency_ID=" + _lstCurr[k].baseCurrencyID + " AND C_Currency_To_ID=" + myCurrencyID
                                         + "  AND Created=(SELECT Max(Created) FROM C_Conversion_Rate  WHERE isactive      ='Y' AND C_ConversionType_id=" + defaultconversionType + " AND "
                                         + "  C_Currency_ID   =" + _lstCurr[k].baseCurrencyID + "  AND C_Currency_To_ID=" + myCurrencyID + ") AND AD_Client_ID = " + _lstCurr[k].AD_Client_ID + "AND AD_Org_ID= " + _lstCurr[k].AD_Org_ID;
                                    //the Maximum date from Converted rate of every currency
                                    object validDate = DB.ExecuteScalar(sql.Trim(), null, null);
                                    //Check if valid date available.. and less than current date..
                                    //By Karan 22 June
                                    if (validDate != null && validDate != DBNull.Value && DateTime.Now.Date > Convert.ToDateTime(validDate).Date)
                                    {
                                        if (!String.IsNullOrEmpty(myCurrency) && !String.IsNullOrEmpty(_lstCurr[k].baseCurrency)
                                              && !String.IsNullOrEmpty(currencySourceName) && (myCurrencyID != _lstCurr[k].baseCurrencyID))
                                        {
                                            String result = GetConvertedCurrencyValue(_lstCurr[k].baseCurrency, myCurrency, currencySourceName);
                                            if (!String.IsNullOrEmpty(result))
                                            {
                                                conversion = new MConversionRate(GetCtx(), 0, null);
                                                conversion.SetAD_Org_ID((_lstCurr[k].AD_Org_ID));
                                                conversion.SetAD_Client_ID(_lstCurr[k].AD_Client_ID);
                                                //conversion.SetValidFrom(DateTime.Now.AddDays(-1));
                                                conversion.SetValidFrom(DateTime.Now);
                                                if (updateFrequency.Equals("D"))
                                                {
                                                    conversion.SetValidTo(DateTime.Now);
                                                }
                                                else if (updateFrequency.Equals("W"))
                                                {
                                                    conversion.SetValidTo(DateTime.Now.AddDays(7));
                                                }
                                                else if (updateFrequency.Equals("M"))
                                                {
                                                    conversion.SetValidTo(DateTime.Now.AddMonths(1));
                                                }

                                                conversion.SetC_ConversionType_ID(defaultconversionType);
                                                conversion.SetC_Currency_ID(_lstCurr[k].baseCurrencyID);
                                                conversion.SetC_Currency_To_ID(myCurrencyID);

                                                rate2 = VAdvantage.Utility.Env.ZERO;
                                                one = new Decimal(1.0);

                                                //if (dsConversion.Tables[0].Rows[x]["Surchargepercentage"] != null && dsConversion.Tables[0].Rows[x]["Surchargepercentage"] != DBNull.Value
                                                //    && Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargepercentage"]) != 0)
                                                //{
                                                //    rate1 = (Convert.ToDecimal(result) + (Convert.ToDecimal(result) * (Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargepercentage"]) / 100)));
                                                //    if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                                //    {
                                                //        rate2 = Decimal.Round(Decimal.Divide(one, Convert.ToDecimal(result)), 12);// MidpointRounding.AwayFromZero);
                                                //    }
                                                //    rate2 = (rate2 + rate2 * (Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargepercentage"]) / 100));
                                                //}
                                                //else if (dsConversion.Tables[0].Rows[x]["Surchargevalue"] != null && dsConversion.Tables[0].Rows[x]["Surchargevalue"] != DBNull.Value
                                                //    && Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargevalue"]) != 0)
                                                //{
                                                //    rate1 = (Convert.ToDecimal(result) + Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargevalue"]));
                                                //    if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                                //    {
                                                //        rate2 = Decimal.Round(Decimal.Divide(one, Convert.ToDecimal(result)), 12);// MidpointRounding.AwayFromZero);
                                                //    }
                                                //    rate2 = (rate2 + Convert.ToDecimal(dsConversion.Tables[0].Rows[x]["Surchargevalue"]));
                                                //}
                                                //else 
                                                //{
                                                //    rate1 = Convert.ToDecimal(result);
                                                //    if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                                //    {
                                                //        rate2 = Decimal.Round(Decimal.Divide(one, Convert.ToDecimal(result)), 12);// MidpointRounding.AwayFromZero);
                                                //    }
                                                //}

                                                rate1 = Convert.ToDecimal(result);

                                                //if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                                //{
                                                //    rate2 = Decimal.Round(Decimal.Divide(one, Convert.ToDecimal(result)), 12);// MidpointRounding.AwayFromZero);
                                                //}
                                                conversion.SetMultiplyRate(rate1);
                                                //conversion.SetDivideRate(rate2);
                                                if (!conversion.Save())
                                                {
                                                    //status = "ConversionRateNotsaved";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //trx.Rollback();
                //status = ex.Message;
                //trx.Close();
                return status;
            }
            //trx.Commit();
            //trx.Close();
            ds.Dispose();
            _lstCurr = null;
            return status;
        }
        //Added by Arpit To Get Converted Amount from xe.com or google.com Accordingly parameter
        public static string GetConvertedCurrencyValue(string from, string to, string url)
        {
            string response = "";
            if (url.ToUpper().Contains("XE.COM"))
            {
                if (from == to)
                {
                    response = Convert.ToString(1);
                    return response;
                }
                string newUrl = @"http://www.xe.com/ucc/convert/?Amount=1&From=" + from.ToString() + "&To=" + to.ToString() + "";
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(newUrl);
                HttpWebResponse resp = (HttpWebResponse)webReq.GetResponse();//send sms
                StreamReader responseReader = new StreamReader(resp.GetResponseStream());//read the response 
                response = responseReader.ReadToEnd();//get result                  
                response = response.Substring(response.IndexOf("<td width=\"47%\" align=\"left\" class=\"rightCol\">"));
                response = response.Substring(response.IndexOf("rightCol\">") + 10); // +10 to remove unwanted things before the converted value

                response = response.Substring(0, response.IndexOf('&'));
                responseReader.Close();
                resp.Close();
                return response;

            }
            else if (url.ToUpper().Contains("GOOGLE.COM"))
            {
                if (from == to)
                {
                    response = Convert.ToString(1);
                    return response;
                }
                WebClient web = new WebClient();
                string newUrl = @"http://www.google.com/finance/converter?a=1&from=" + from.ToString().ToUpper() + "&to=" + to.ToString().ToUpper() + "";
                response = web.DownloadString(newUrl);
                if (response.Contains("currency_converter_result"))
                {
                    response = response.Substring(response.IndexOf("currency_converter_result"), 70);
                    response = response.Substring(response.IndexOf(">") + 1);
                    response = response.Substring(response.IndexOf(">") + 1);
                    response = response.Substring(0, response.IndexOf(" "));
                    return response;
                }
            }

            return response;
        }
        // End here

    }
    public class CurrencyProp
    {
        // Created a property class 
        public int baseCurrencyID { get; set; }
        public String baseCurrency { get; set; }
        public String frequency { get; set; }
        public int AD_Client_ID { get; set; }
        public int AD_Org_ID { get; set; }
        public int CurrencySource { get; set; }
    }
}
