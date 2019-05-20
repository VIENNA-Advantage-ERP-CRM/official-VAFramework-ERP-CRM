using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Server;
using System.Web.Hosting;
using System.ServiceModel;


namespace VAdvantage.Server
{
    public class InvokeIISServiceProcessor : ViennaServer
    {
        /**	Last Summary				*/
        private StringBuilder m_summary = new StringBuilder();

        IDSPosting proxy = null;
        BasicHttpBinding binding = null;
        EndpointAddress endpoint = null;


        public InvokeIISServiceProcessor() 
            :base()
        {
        }

        protected override void DoWork()
        {
            m_summary = new StringBuilder();

            //code to call the service
            //IApplicationHost host = HostingEnvironment.ApplicationHost;
            //string siteName = String.Format("https://{0}/DSPosting.svc", host.GetSiteName());
            string siteName = VAdvantage.Server.ViennaServerMgr.invokeIISUrl;
            try
            {
                binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                if (siteName.IndexOf("https://", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;
                }
                endpoint = new EndpointAddress(siteName);
                    
                ChannelFactory<IDSPosting> channel = new ChannelFactory<IDSPosting>(binding, endpoint);
                channel.Open();
                proxy = channel.CreateChannel();
                bool b = proxy.InvokeIISService();
                channel.Close();
                if (b)
                    m_summary.Append("Service invoked");
                else
                    m_summary.Append("Error in invoking service");
            }
            catch (Exception e)
            {
                VAdvantage.Logging.VLogger.Get().Severe("InvokerError ==>" + e.Message);
            }
        }

        public override string GetServerInfo()
        {
            return "#" + _runCount + " - Last=" + m_summary.ToString();
        }
    }
}
