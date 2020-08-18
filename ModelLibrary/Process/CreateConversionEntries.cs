using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using System.ServiceModel;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class CreateConversionEntries : ProcessEngine.SvrProcess
    {
        int baseCurrencyID = 0;
        int C_CurrencySource_ID = 0;
         string KEY =DataBase.GlobalVariable.ACCESSKEY;// "caff4eb4fbd6273e37e8a325e19f0991";
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Currency_ID"))
                {
                    baseCurrencyID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_CurrencySource_ID"))
                {
                    C_CurrencySource_ID = para[i].GetParameterAsInt();
                }
               
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            string status = "OK";
            string baseCurrency = DB.ExecuteScalar("Select ISO_Code from C_Currency Where C_Currency_ID=" + baseCurrencyID).ToString();
            string currencySourceName = DB.ExecuteScalar("Select url from C_CurrencySource Where C_CurrencySource_ID=" + C_CurrencySource_ID).ToString();
            string myCurrency = "";
            int myCurrencyID = 0;
            string sql = @"SELECT ISO_Code,C_Currency_ID FROM C_Currency WHERE IsActive='Y' AND ISMYCURRENCY='Y'";
            DataSet ds = DB.ExecuteDataset(sql);
            Trx trx = Trx.Get("CreateConVersionEnties");
            try
            {
                if (ds != null)
                {
                    String URL = "http://localhost/CloudService55/AccountService.asmx";
                    //String CloudURL = "http://cloudservice.viennaadvantage.com/AccountService.asmx";
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                    {
                        CloseTimeout = new TimeSpan(00, 20, 00),
                        SendTimeout = new TimeSpan(00, 20, 00),
                        OpenTimeout = new TimeSpan(00, 20, 00),
                        ReceiveTimeout = new TimeSpan(00, 20, 00),
                        MaxReceivedMessageSize = int.MaxValue,
                        MaxBufferSize = int.MaxValue
                    };


                    int defaultconversionType = 0;
                    try
                    {
                        defaultconversionType = Convert.ToInt32(DB.ExecuteScalar("select c_conversiontype_id from c_conversiontype where isdefault='Y' and isactive='Y'"));
                    }
                    catch { }
                    MConversionRate conversion = null;
                    Decimal rate1=0;
                    Decimal rate2 = 0;
                    Decimal one = new Decimal(1.0);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        myCurrency = ds.Tables[0].Rows[i]["ISO_Code"].ToString();
                        myCurrencyID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Currency_ID"]);

                        var client = new ModelLibrary.AcctService.AccountServiceSoapClient(binding, new EndpointAddress(URL));
                        if (!String.IsNullOrEmpty(myCurrency)
                        && !String.IsNullOrEmpty(baseCurrency)
                        && !String.IsNullOrEmpty(currencySourceName))
                        {
                            string result = client.GetConvertedCurrencyValue(baseCurrency, myCurrency, currencySourceName, KEY);

                            if (!String.IsNullOrEmpty(result))
                            {
                                conversion = new MConversionRate(GetCtx(), 0, trx);
                                conversion.SetAD_Org_ID(0);
                                conversion.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                                conversion.SetValidFrom(DateTime.Now);
                                conversion.SetValidTo(DateTime.Now);
                                conversion.SetC_ConversionType_ID(defaultconversionType);
                                conversion.SetC_Currency_ID(myCurrencyID);
                                conversion.SetC_Currency_To_ID(baseCurrencyID);
                                //conversion.SetC_Currency_To_ID();
                                 rate1 = Convert.ToDecimal(result);
                                 rate2 = Utility.Env.ZERO;
                                 one = new Decimal(1.0);
                                if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                {
                                    rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);
                                }
                                conversion.SetMultiplyRate(rate1);
                                conversion.SetDivideRate(rate2);
                                if (!conversion.Save(trx))
                                {
                                    status = "ConversionRateNotsaved";

                                }
                            }
                            result = client.GetConvertedCurrencyValue(myCurrency, baseCurrency, currencySourceName, KEY);

                            if (!String.IsNullOrEmpty(result))
                            {
                                conversion = new MConversionRate(GetCtx(), 0, trx);
                                conversion.SetAD_Org_ID(0);
                                conversion.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                                conversion.SetValidFrom(DateTime.Now);
                                conversion.SetValidTo(DateTime.Now);
                                conversion.SetC_ConversionType_ID(defaultconversionType);
                                conversion.SetC_Currency_ID(baseCurrencyID);
                                conversion.SetC_Currency_To_ID(myCurrencyID);
                                //conversion.SetC_Currency_To_ID();
                                rate1 = Convert.ToDecimal(result);
                                rate2 = Utility.Env.ZERO;
                                one = new Decimal(1.0);
                                if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                                {
                                    rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);
                                }
                                conversion.SetMultiplyRate(rate1);
                                conversion.SetDivideRate(rate2);
                                if (!conversion.Save(trx))
                                {
                                    status = "ConversionRateNotsaved";
                                }
                            }


                        }


                    }
                }
            }
            catch(Exception ex)
            {
                status = ex.Message;
            }
            if (status.Equals("OK"))
            {
                trx.Commit();
            }
            else
            {
                trx.Rollback();
            }
            trx.Close();
            return status;
        }
    }
}
