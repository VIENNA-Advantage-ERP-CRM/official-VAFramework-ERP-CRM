using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MarketSvc
{
   internal class ServiceEndPoint
    {
        internal static MarketSvc.MService.MServiceClient GetMServiceClient()
        {
            string mUrl = GetDNSName() + "MService.svc";
            
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                CloseTimeout = new TimeSpan(00, 10, 00),
                SendTimeout = new TimeSpan(00, 10, 00),
                OpenTimeout = new TimeSpan(00, 10, 00),
                ReceiveTimeout = new TimeSpan(00, 10, 00),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue, MaxDepth = int.MaxValue, MaxStringContentLength = int.MaxValue }

            };

            if (mUrl.ToString().IndexOf("https://", StringComparison.OrdinalIgnoreCase) != -1)
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            return new MService.MServiceClient(binding, new EndpointAddress(mUrl));
        }

       /// <summary>
       /// Market Service Dns
       /// </summary>
       /// <returns></returns>
        internal static string GetDNSName()
        {

            string svcpath = "http://cloudservice.softwareonthecloud.com/";
            //string svcpath = "http://sbcloudservice.softwareonthecloud.com/";
            //string svcpath = "http://localhost:4644/";
            //string svcpath =   "http://cloudservicedemo.softwareonthecloud.com/"; 
            //string svcpath = "http://cloudservice.softwareonthecloud.com/";

            //string svcpath = "http://192.168.0.162/CloudService55/";
            //string svcpath = "http://81.169.187.204:93/";  
            //MarketServer CloudServiceURL
            //string svcpath = "http://cloudservicedemo.softwareonthecloud.com/";

            //string svcpath = "http://81.169.133.121:786/";
            //string svcpath = "http://138.201.234.235/"; 
            //string svcpath = "http://sbhtml.softwareonthecloud.com/";

           // string svcpath = "http://202.164.37.9:93/CloudService55/";

            return svcpath;
        }


       /// <summary>
       /// Live server Service url
       /// </summary>
        /// <returns>Return service host Url for Key</returns>
        internal static string GetDNSNameForTokenKey()
        {

            string svcpath = "http://cloudservice.softwareonthecloud.com/";
            //string svcpath = "http://sbcloudservice.softwareonthecloud.com/";
            //string svcpath = "http://localhost:4644/";
            // string svcpath = "http://192.168.0.162/CloudService12/";
            //string svcpath = "http://cloudservicedemo.softwareonthecloud.com/";
            //string svcpath = "http://cloudservice.softwareonthecloud.com/";
            //string svcpath = "http://192.168.0.162/CloudService55/";
            //string svcpath = "http://81.169.187.204:93/";  
            //MarketServer CloudServiceURL
            //string svcpath = "http://cloudservicedemo.softwareonthecloud.com/";
            //string svcpath = "http://81.169.133.121:786/"; 
            //string svcpath = "http://138.201.234.235/"; 
           // string svcpath = "http://sbhtml.softwareonthecloud.com/";
            //string svcpath = "http://202.164.37.9:93/CloudService55/";
            return svcpath;
        }


        internal static string GetDNSNameForPing()
        {
            string pingPath = "188.138.92.6";
            //string pingPath = "sbcloudservice.softwareonthecloud.com";
            //string svcpath = "http://cloudservicedemo.softwareonthecloud.com/";
            //string svcpath = "http://cloudservice.softwareonthecloud.com/";
            //string svcpath = "http://localhost/CloudService55/";
            //string svcpath = "http://81.169.187.204:93/";  
            //MarketServer CloudServiceURL
            //string svcpath = "http://cloudservicedemo.softwareonthecloud.com/";
            //string pingPath = "81.169.133.121";
            //string pingPath = "192.168.0.161";
            //string pingPath = "85.214.78.248";
            return pingPath;
        }


        internal static bool CheckServerConnection(string hostNameOrAddress=null)
        {
            bool pingStatus = false;

            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                string data = "aaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                try
                {
                    System.Net.NetworkInformation.PingReply reply = p.Send(hostNameOrAddress ?? GetDNSNameForPing(), timeout, buffer);
                    pingStatus = (reply.Status == System.Net.NetworkInformation.IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }
            return pingStatus;
        }



    }
}
