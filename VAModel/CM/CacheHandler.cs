/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CacheHandler
 * Purpose        : The CacheHandler handles deployment and clean of internal and external caches
 * Class Used     : 
 * Chronological    Development
 * Deepak           05-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;

using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using System.Net;
using System.IO;
using VAdvantage.Model;


namespace VAdvantage.CM
{
    public class CacheHandler
    {
        protected String[] cacheURLs;
        protected VLogger log;

        /// <summary>
        ///CacheHandler for single URL environment
        /// </summary>
        /// <param name="thisURL">URL of this Server</param>
        /// <param name="tLog">thisLogger</param>
        /// <param name="ctx">Propertie Context</param>
        /// <param name="trxName">trx</param>
        public CacheHandler(String thisURL, VLogger tLog, Ctx ctx, Trx trxName)
        {
            int[] theseServers = X_CM_BroadcastServer.GetAllIDs("CM_BroadcastServer", "CM_WebProject_ID=0", trxName);
            if (theseServers != null && theseServers.Length > 0)
            {
                String[] thisURLs = new String[theseServers.Length];
                for (int i = 0; i < theseServers.Length; i++)
                {
                    X_CM_BroadcastServer thisServer = new X_CM_BroadcastServer(ctx, theseServers[i], trxName);
                    thisURLs[i] = thisServer.GetIP_Address();
                }
                cacheURLs = thisURLs;
            }
            else
            {
                // Okay we can't find any cluster config, so we will use the AppsServer from Client
                String[] thisURLs = new String[1];
                thisURLs[0] = thisURL;
                cacheURLs = thisURLs;
            }
            log = tLog;
        }

        /// <summary>
        ///CacheHandler form multiple URLs
        /// </summary>
        /// <param name="thisURLs">Array of Cache Server URLs</param>
        /// <param name="tLog">Logger</param>
        public CacheHandler(String[] thisURLs, VLogger tLog)
        {
            log = tLog;
            cacheURLs = thisURLs;
        }

        /// <summary>
        ///Clean Template cache for this ID
        /// </summary>
        /// <param name="ID">ID of template to clean</param>
        public void CleanTemplate(int ID)
        {
            CleanTemplate("" + ID);
        }

        /// <summary>
        ///Clean Template cache for this ID
        /// </summary>
        /// <param name="ID">ID of template to clean</param>
        public void CleanTemplate(String ID)
        {
            RunURLRequest("Template", ID);
        }

        /// <summary>
        /// Empty all Template Caches
        /// </summary>
        public void EmptyTemplate()
        {
            RunURLRequest("Template", "0");
        }

        /// <summary>
        ///Clean ContainerCache for this ID
        /// </summary>
        /// <param name="ID">ID for Container to clean</param>
        public void CleanContainer(int ID)
        {
            CleanContainer("" + ID);
        }

        /// <summary>
        ///  Clean ContainerCache for this ID
        /// </summary>
        /// <param name="ID">id</param>
        public void CleanContainer(String ID)
        {
            RunURLRequest("Container", ID);
        }

        /// <summary>
        /// Clean ContainerTreeCache for this WebProjectID
        /// </summary>
        /// <param name="ID"></param>
        public void CleanContainerTree(int ID)
        {
            CleanContainerTree("" + ID);
        }

        /// <summary>
        /// Clean ContainerTreeCache for this WebProjectID
        /// </summary>
        /// <param name="ID"></param>
        public void CleanContainerTree(String ID)
        {
            RunURLRequest("ContainerTree", ID);
        }

        /// <summary>
        /// Clean Container Element for this ID
        /// </summary>
        /// <param name="ID">id</param>
        public void CleanContainerElement(int ID)
        {
            CleanContainerElement("" + ID);
        }

        /// <summary>
        ///  Clean Container Element for this ID
        /// </summary>
        /// <param name="ID">id</param>
        public void CleanContainerElement(String ID)
        {
            RunURLRequest("ContainerElement", ID);
        }

        private void RunURLRequest(String cache, String ID)
        {
            String thisURL = null;
            for (int i = 0; i < cacheURLs.Length; i++)
            {
                try
                {
                   // cacheURLs[i] = Dns.GetHostName();
                    thisURL = "http://" + cacheURLs[i] + "/cache/Service?Cache=" + cache + "&ID=" + ID;
                    //URL url = new URL(thisURL);
                    Uri url = new Uri(thisURL);
                    //Url url = new Url(thisURL);
                     //Proxy thisProxy = Proxy.NO_PROXY;
                     //WebProxy thisProxy = WebProxy. GetDefaultProxy();
                    //System.Net.Cache.RequestCachePolicy v = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    ///WebRequst.DefaultCachePolicy = v;
                    //request =new we // = WebRequest.Create(url);                         
                    //request.CachePolicy = v;
                    WebRequest request = WebRequest.Create(url);
                    System.Net.Cache.RequestCachePolicy v = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    //WebRequst.DefaultCachePolicy = v;
                    WebRequest.DefaultCachePolicy = v;
                    WebResponse response = (WebResponse)request.GetResponse();
                    //URLConnection urlConn = url.openConnection(thisProxy);
                    //urlConn.setUseCaches(false);
                    //urlConn.connect();
                    //Reader stream = new java.io.InputStreamReader(
                    //urlConn.getInputStream());
                    

                    Stream stream = response.GetResponseStream();
                    BinaryReader reader = new BinaryReader(stream,UTF8Encoding.UTF8);
                    StringBuilder srvOutput = new StringBuilder();

                    byte[] RecvBuffer = new byte[stream.Length];
                    int lenth=Convert.ToInt32(stream.Length);
                    
                    try
                    {
                         
                        int c;
                        string tempstring = null;
                        while ((c=stream.Read(RecvBuffer,0,lenth))>0)
                        {
                            //srvOutput.Append((char)c);
                            tempstring = Encoding.ASCII.GetString(RecvBuffer,0,lenth);

                            // continue building the string
                            srvOutput.Append(tempstring);

                        }
                    }
                    catch (Exception E2)
                    {
                        E2.StackTrace.ToString();// .printStackTrace();
                    }
                }
                catch (Exception E)
                {
                    if (log != null)
                        log.Warning("Can't clean cache at:" + thisURL + " be carefull, your deployment server may use invalid or old cache data!"+E.Message);
                }
            }
        }
        /// <summary>
        ///Converts JNP URL to http URL for cache cleanup
        /// </summary>
        /// <param name="JNPURL"> String with JNP URL from Context</param>
        /// <returns>clean servername</returns>
        public static String ConvertJNPURLToCacheURL(String JNPURL)
        {
            if (JNPURL.IndexOf("jnp://") >= 0)
            {
                JNPURL = JNPURL.Substring(JNPURL.IndexOf("jnp://") + 6);
            }
            if (JNPURL.IndexOf(":") >= 0)
            {
                JNPURL = JNPURL.Substring(0, JNPURL.IndexOf(":"));
            }
            if (JNPURL.Length > 0)
            {
                return JNPURL;
            }
            else
            {
                return null;
            }
        }
    }





}
