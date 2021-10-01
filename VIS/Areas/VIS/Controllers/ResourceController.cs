using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Classes;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;

namespace VIS.Controllers
{
    public class ResourceController : Controller
    {

        /// <summary>
        /// return javascript file containing application start up  data 
        /// - translated message object
        /// - role object
        /// - context of app
        /// - constant 
        /// -
        /// </summary>
        /// <returns>javascript file result </returns>

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JavaScriptResult Application()
        {
            //var s = Codec.DecryptStringAES();
            StringBuilder sb = new StringBuilder();

            Ctx ctx = Session["ctx"] as Ctx;


            if (ctx == null) // handle null value , sometime session is expired
            {
                sb.Append("; window.location.reload();");
            }
            else
            {
                if (ctx.GetSecureKey() == "")
                    ctx.SetSecureKey(SecureEngineBridge.GetRandomKey());

                //  ctx.SetApplicationUrl(@Url.Content("~/"));
                ctx.SetIsSSL(Request.Url.Scheme == Uri.UriSchemeHttps);

                //lakhwinder
                string fullUrl = Request.Url.AbsoluteUri.Remove(Request.Url.AbsoluteUri.LastIndexOf('/'));
                //fullUrl = fullUrl.Remove(fullUrl.LastIndexOf('/'));
                //fullUrl = fullUrl.Remove(fullUrl.LastIndexOf('/'));
                fullUrl = fullUrl.Remove(fullUrl.IndexOf("VIS/Resource"));
                ctx.SetApplicationUrl(fullUrl);
                ctx.SetContextUrl(@Url.Content("~/"));

                SecureEngine.Encrypt("a");

                CCache<string, string> msgs = Msg.Get().GetMsgMap(ctx.GetAD_Language());

                sb.Append("; var VIS = {");
                sb.Append("Application: {contextUrl:'").Append(@Url.Content("~/")).Append("',").Append(" contextFullUrl:'").Append(fullUrl).Append("',")
                         .Append("isMobile:").Append(Request.Browser.IsMobileDevice ? "1" : "0")
                         .Append(", isRTL:").Append(ctx.GetIsRightToLeft() ? "1" : "0")
                         .Append(", isBasicDB:").Append(ctx.GetIsBasicDB() ? "1" : "0")
                         .Append(", isSSL:").Append((Request.Url.Scheme != Uri.UriSchemeHttps ? "0" : "1")) //TODO
                         .Append(", theme:").Append("'").Append(GetThemeInfo(ctx)).Append("'") //TODO
                         .Append("},");

                sb.Append("I18N: { }, context: { }");
                sb.Append("};");

                sb.Append("VIS.Consts={");
                /* Table */
                sb.Append("'ACCESSLEVEL_Organization' : '1','ACCESSLEVEL_ClientOnly' : '2','ACCESSLEVEL_ClientPlusOrganization' : '3' ,'ACCESSLEVEL_SystemOnly' : '4'");
                sb.Append(", 'ACCESSLEVEL_SystemPlusClient' : '6','ACCESSLEVEL_All' : '7'");
                sb.Append(", 'ACCESSTYPERULE_Accessing' : 'A', 'ACCESSTYPERULE_Exporting' : 'E' , 'ACCESSTYPERULE_Reporting' : 'R'");
                sb.Append("};");

                /* USER */
                sb.Append(" VIS.MUser = {");
                sb.Append("'isAdministrator':'" + MUser.Get(ctx).IsAdministrator() + "', 'isUserEmployee':'" + MUser.GetIsEmployee(ctx, ctx.GetAD_User_ID()) + "' }; ");

                /* ROLE */
                sb.Append(" VIS.MRole =  {");
                sb.Append(" 'vo' : " + Newtonsoft.Json.JsonConvert.SerializeObject(VIS.Helpers.RoleHelper.GetRole(VAdvantage.Model.MRole.GetDefault(ctx, false))) + " , ");
                sb.Append(" 'SQL_RW' : true, 'SQL_RO' : false, 'SQL_FULLYQUALIFIED' : true, 'SQL_NOTQUALIFIED' : false,'SUPERUSER_USER_ID' : 100, 'SYSTEM_USER_ID' : 0 ");
                sb.Append(", 'PREFERENCETYPE_Client':'C', 'PREFERENCETYPE_None':'N', 'PREFERENCETYPE_Organization':'O', 'PREFERENCETYPE_User':'U','isAdministrator':" + (VAdvantage.Model.MRole.GetDefault(ctx, false).IsAdministrator() ? "1" : "0").ToString() + "");

                sb.Append(", columnSynonym : { 'AD_User_ID': 'SalesRep_ID','C_ElementValue_ID':'Account_ID'}");
                sb.Append("};");

                /* CTX */
                SetLoginContext(ctx);
                // VIS0008 Setting other values into Context
                SetOtherContext(ctx);
                sb.Append(" VIS.context.ctx = ").Append(Newtonsoft.Json.JsonConvert.SerializeObject(ctx.GetMap())).Append("; ");

                /* Message */
                sb.Append(" VIS.I18N.labels = { ");


                if (msgs != null)
                {
                    int total = msgs.Keys.Count;
                    foreach (var key in msgs.Keys)
                    {
                        --total;
                        //if (key.Contains('\n') || key.Contains('\'')
                        //   || key.Contains('\"') || key.StartsWith("SC_") || key.Contains('\r'))
                        //{
                        //    continue;
                        //}
                        //if (msgs.Get(key).ToString().Contains('\n') || msgs.Get(key).ToString().Contains('\'')
                        //    || msgs.Get(key).ToString().Contains('\"') || msgs.Get(key).ToString().Contains('\r'))
                        //{
                        //    continue;
                        //}
                        string msg = (string)msgs.Get(key) ?? "";
                        msg = msg.Replace("\n", " ").Replace("\r", " ").Replace("\"", "'");
                        sb.Append("\"").Append(key).Append("\": ").Append("\"").Append(msg).Append("\"");
                        if (total != 0)
                        {
                            sb.Append(",");
                        }

                    }

                }

                /* purpose: right window action translation with search key
                 * VIS0228      08-Aug-2021 
                 */

                ValueNamePair[] refList = MRefList.GetList(435, false, ctx);
                int refListTotal = refList.Length;
                if (refListTotal > 0)
                {
                    if (msgs.Keys.Count > 0)
                    {
                        sb.Append(", ");
                    }

                    for (int i = 0; i < refList.Length; i++)
                    {
                        sb.Append("\"").Append(refList[i].GetValue()).Append("\": ").Append("\"").Append(refList[i].GetName()).Append("\"");
                        if (i != (refListTotal - 1))
                        {
                            sb.Append(", ");
                        }

                    }
                }

                sb.Append("};");
                // sb.Append(" console.log(VIS.I18N.labels)");
                //return View();
                //System.Web.Optimization.JsMinify d = new System.Web.Optimization.JsMinify();
                //d.Process(


                //Update Login Time

                var r = new ResourceManager(fullUrl, ctx.GetAD_Client_ID());
                r.RunAsync();
                r = null;
            }

            return JavaScript(sb.ToString());
        }

        /// <summary>
        /// Load User Preference into Context
        /// </summary>
        /// <param name="_ctx"> application context </param>
        internal void SetLoginContext(Ctx _ctx)
        {
            VAdvantage.Login.LoginProcess process = new VAdvantage.Login.LoginProcess(_ctx);

            if (VAdvantage.Model.MUser.IsSalesRep(_ctx.GetAD_User_ID()))
                _ctx.SetContext("#SalesRep_ID", _ctx.GetAD_User_ID());
            if (_ctx.GetAD_Role_ID() == 0)	//	User is a Sys Admin
                _ctx.SetContext("#SysAdmin", "Y");

            _ctx.SetContext("#IsAdmin", VAdvantage.Model.MRole.GetDefault(_ctx, false).IsAdministrator() ? "Y" : "N");

            // m_ctx.SetContext("#User_Level", dr[0].ToString());  
            process.LoadPreferences(_ctx.GetContext("#Date"), "");


            //return JsonHelper.Serialize(_ctx.GetMap());
        }

        /// <summary>
        /// Load Other Preferences into Context
        /// </summary>
        /// <param name="_ctx"> application context </param>
        internal void SetOtherContext(Ctx _ctx)
        {
            // Fetched DMS form id and set into context
            int AD_Form_ID = Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar("SELECT AD_Form_ID FROM AD_Form WHERE Name = 'VADMS_DMSWeb' AND IsActive = 'Y'"));
            _ctx.SetContext("DMS_Form_ID", AD_Form_ID);
        }

        internal string GetThemeInfo(Ctx _ctx)
        {
            string thms = "";

            //1 first user and client 

            string qry = "SELECT COALESCE(u.AD_Theme_ID,c.AD_Theme_ID) FROM AD_User u INNER JOIN AD_Client c ON c.AD_Client_ID = u.AD_Client_ID " +
                         " WHERE u.AD_User_ID =" + _ctx.GetAD_User_ID();

            int id = Util.GetValueOfInt(DBase.DB.ExecuteScalar(qry, null, null));

            if (id < 1)
            {
                //2 get System default
                id = Util.GetValueOfInt(DBase.DB.ExecuteScalar("SELECT AD_Theme_ID FROM AD_Theme WHERE IsDefault = 'Y' ORDER By Updated DESC", null, null));
            }

            if (id > 0)
            {
                System.Data.DataSet ds = DBase.DB.ExecuteDataset("SELECT SecondaryColor, OnSecondaryColor, PrimaryColor, OnPrimaryColor " +
                                                        " FROM AD_Theme WHERE AD_Theme_ID = " + id, null);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    thms = ds.Tables[0].Rows[0]["PrimaryColor"] + "|" + ds.Tables[0].Rows[0]["OnPrimaryColor"] + "|" + ds.Tables[0].Rows[0]["SecondaryColor"]
                        + "|" + ds.Tables[0].Rows[0]["OnSecondaryColor"];
                }

            }
            return thms;
        }


    }

    public class ResourceManager
    {
        string _url = "";
        int _AD_Client_ID = 0;
        Thread _thread = null;

        public ResourceManager(string url, int AD_Client_ID)
        {
            _url = url;
            _AD_Client_ID = AD_Client_ID;
        }


        public void RunAsync()
        {
            _thread = new Thread(new ThreadStart(Init));
            _thread.Start();
        }

        private void Init()
        {
            try
            {
                UpdateLoginTime(_AD_Client_ID, _url, VAdvantage.DataBase.GlobalVariable.TO_DATE(DateTime.Now, false));
            }
            catch
            {
            }
            _url = "";
            _thread = null;
        }

        public string UpdateLoginTime(int AD_Client_ID, String url, string loginTime)
        {
            var client = VAdvantage.Classes.ServerEndPoint.GetCloudClient();
            string retStr = "";
            string key = VAdvantage.Classes.ServerEndPoint.GetAccesskey();

            if (client != null)
            {
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    retStr = client.SetLastLogin(AD_Client_ID, url, loginTime, key);
                    VAdvantage.Logging.VLogger.Get().Info("Update Login =>" + retStr);
                    client.Close();
                }
                catch
                {
                    client.Close();
                }
            }
            return retStr;
        }
    }


}
