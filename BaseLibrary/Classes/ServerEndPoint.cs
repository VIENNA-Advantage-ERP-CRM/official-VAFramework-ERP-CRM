using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using VAdvantage.Utility;



namespace VAdvantage.Classes
{
    public class ServerEndPoint
    {

        public static BaseLibrary.CloudService.ServiceSoapClient GetCloudClient()
        {
            object CloudURL = System.Configuration.ConfigurationManager.AppSettings["CloudURL"];
            if (CloudURL != null && CloudURL.ToString() != "")
            {
                BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                {
                    CloseTimeout = new TimeSpan(00, 05, 00),
                    SendTimeout = new TimeSpan(00, 05, 00),
                    OpenTimeout = new TimeSpan(00, 05, 00),
                    ReceiveTimeout = new TimeSpan(00, 05, 00),
                    MaxReceivedMessageSize = int.MaxValue,
                    MaxBufferSize = int.MaxValue
                };

                if (CloudURL.ToString().IndexOf("https://", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                }

                return new BaseLibrary.CloudService.ServiceSoapClient(binding, new EndpointAddress(CloudURL.ToString()));
            }
            else
            {
                return null;
            }
        }

        //public static CloudSchedularService.CloudSchedularServiceSoapClient GetRemoteServerClient(string RemoteServerURL)
        //{
        //    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
        //    {
        //        CloseTimeout = new TimeSpan(00, 20, 00),
        //        SendTimeout = new TimeSpan(00, 20, 00),
        //        OpenTimeout = new TimeSpan(00, 20, 00),
        //        ReceiveTimeout = new TimeSpan(00, 20, 00),
        //        MaxReceivedMessageSize = int.MaxValue,
        //        MaxBufferSize = int.MaxValue
        //    };

        //    if (RemoteServerURL.IndexOf("https://", StringComparison.OrdinalIgnoreCase) != -1)
        //    {
        //        binding.Security.Mode = BasicHttpSecurityMode.Transport;

        //    }

        //    return new CloudSchedularService.CloudSchedularServiceSoapClient(binding, new EndpointAddress(RemoteServerURL));

        //}


        //public static CloudService.ServiceSoapClient GetOnLineHelpClient()
        //{
        //    object key = System.Configuration.ConfigurationSettings.AppSettings["OnlineHelpURL"];
        //    if (key != null && key.ToString() !="")
        //    {
        //        string url = key.ToString() + "Service.asmx";

        //        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
        //        {
        //            CloseTimeout = new TimeSpan(00, 10, 00),
        //            SendTimeout = new TimeSpan(00, 10, 00),
        //            OpenTimeout = new TimeSpan(00, 10, 00),
        //            ReceiveTimeout = new TimeSpan(00, 10, 00),
        //            MaxReceivedMessageSize = int.MaxValue,
        //            MaxBufferSize = int.MaxValue,
        //            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
        //            { 
        //              MaxArrayLength = int.MaxValue, 
        //              MaxStringContentLength = int.MaxValue, 
        //              MaxDepth = int.MaxValue, 
        //              MaxBytesPerRead = int.MaxValue,
        //              MaxNameTableCharCount= int.MaxValue
        //            }
        //        };
        //        return new CloudService.ServiceSoapClient(binding, new EndpointAddress(url));
        //    }
        //    return null;

        //}

        //public static SpeechService.SpeechServiceClient GetSpeechClient()
        //{
        //    object key = System.Configuration.ConfigurationSettings.AppSettings["SpeechServicehURL"];
        //    if (key != null && key.ToString() != "")
        //    {
        //        string url = key.ToString();

        //        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
        //        {
        //            CloseTimeout = new TimeSpan(00, 10, 00),
        //            SendTimeout = new TimeSpan(00, 10, 00),
        //            OpenTimeout = new TimeSpan(00, 10, 00),
        //            ReceiveTimeout = new TimeSpan(00, 10, 00),
        //            MaxReceivedMessageSize = int.MaxValue,
        //            MaxBufferSize = int.MaxValue,
        //            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue, MaxDepth = int.MaxValue, MaxBytesPerRead = int.MaxValue }
        //        };
        //        return new SpeechService.SpeechServiceClient(binding, new EndpointAddress(url));
        //    }
        //    return null;
        //}

        /// <summary>
        /// Get Access key
        /// </summary>
        /// <returns>path</returns>
        public static string GetAccesskey()
        {
            string url = "";
            try
            {
                url = SecureEngine.Encrypt(System.Configuration.ConfigurationManager.AppSettings["accesskey"].ToString());

            }
            catch { }
            return url;
        }

        public static bool IsSSLEnabled()
        {
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"] != null && System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"].ToString() != "")
            {
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"].ToString() == "Y")
                {
                    return true;
                }
            }
            return false;
        }

        internal static String GetDeveloperKey()
        {

            object key = System.Web.Configuration.WebConfigurationManager.AppSettings["YTDK"];
            if (key != null && key.ToString() != "")
            {
                return key.ToString();
            }
            return "AI39si5hQDyiYIrNzkULCWan_5MrAiGRThwgxv9SFeqbTgv2end5DHYd5TUaYUVsIK14arKU2XF_XJ7b6EQWRakuH7PBbPS7dA";
        }

        internal static string GetYTUser()
        {
            object key = System.Web.Configuration.WebConfigurationManager.AppSettings["YTUser"];
            if (key != null && key.ToString() != "")
            {
                return key.ToString();
            }
            return "softwareonthecloud";
        }

        //public static MarketSerivce.ModuleMarketServiceClient GetCloudClient()
        //{
        //    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
        //    {
        //        CloseTimeout = new TimeSpan(00, 10, 00),
        //        SendTimeout = new TimeSpan(00, 10, 00),
        //        OpenTimeout = new TimeSpan(00, 10, 00),
        //        ReceiveTimeout = new TimeSpan(00, 10, 00),
        //        MaxReceivedMessageSize = int.MaxValue,
        //        MaxBufferSize = int.MaxValue,
        //        ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue, MaxDepth = int.MaxValue, MaxStringContentLength = int.MaxValue }
        //    };

        //    string url = "http://localhost:84/ModuleMarketService.svc";

        //    return new CloudService.ModuleMarketServiceClient(binding, new EndpointAddress(url));

        //}


    }
}
