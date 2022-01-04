using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;
using Newtonsoft.Json;
using VIS.Helpers;

using System.IO;
using System.Net;

using System.Web.Helpers;
using System.Web.Hosting;

using VIS.Filters;

using System.Web.Optimization;
using VAdvantage.Login;
using VAdvantage.Logging;
using System.Data;

namespace VIS.Controllers
{
    /// <summary>
    /// app controller
    /// </summary>

    public class HomeController : Controller
    {
        /// <summary>
        /// entry point of app
        /// - show login screen if not logged in
        /// - else show home page with login menu
        /// </summary>
        /// <param name="form">form collection</param>
        /// <returns>main view [ login page or home page]</returns>
        /// 
        HomeHelper objHomeHelp = null;

        private static readonly object _lock = new object();



        //public ActionResult Index(string param )
        //{
        //   // FormCollection fc = null;
        //    if (!string.IsNullOrEmpty(param))
        //    {
        //     //   fc = new FormCollection();
        //        TempData["param"] =  param;
        //        RedirectToAction("Index");
        //    }
        //    return Home(null);
        //}

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        /// <summary>
        /// Entry Point of Framework
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public ActionResult Index(FormCollection form)
        {
            if (Request.QueryString.Count > 0)
            {
                // string user = Request.QueryString["U"];
                // string pwd = Request.QueryString["P"];
                // AccountController ac = new AccountController();
                // LoginModel md = new LoginModel();
                // md.Login1Model = new Login1Model();
                // md.Login1Model.UserValue = user;
                // md.Login1Model.Password = pwd;
                //JsonResult jr =  ac.JsonLogin(md, "");
                // ac.SetAuthCookie(md, Response); //AutoLogin if all passed
                // return RedirectToAction("Index");
            }

            //if (!User.Identity.IsAuthenticated)
            //{
            //    // Required to allow javascript redirection through to browser
            //    this.Response.TrySkipIisCustomErrors = true;
            //    this.Response.Status = "401 Unauthorized";
            //    this.Response.StatusCode = 401;
            //    // note that the following line is .NET 4.5 or later only
            //    // otherwise you have to suppress the return URL etc manually!
            //    this.Response.SuppressFormsAuthenticationRedirect = true;
            //    // If we got this far, something failed

            //}


            var url = CloudLogin.IsAllowedToLogin(Request.Url.ToString());
            if (!string.IsNullOrEmpty(url))
            {
                return RedirectPermanent(url);
            }


            VAdvantage.DataBase.DBConn.SetConnectionString();//Init database conection
            Language.GetLanguages();
            LoginModel model = null;
            if (User.Identity.IsAuthenticated)
            {

                if (Request.QueryString.Count > 0) /* if has value */
                {
                    return RedirectToAction("Index"); /*redirect to same url without querystring*/
                }
                try
                {
                    //var conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                    //  SessionStateSection section = (SessionStateSection) conf.GetSection("system.web/sessionState");
                    // int timeout = (int) section.Timeout.TotalMinutes;
                    Session.Timeout = 20; // ideal timout
                }
                catch
                {
                }


                //AccountController a = new AccountController();
                //a.LogOff();
                FormsIdentity ident = User.Identity as FormsIdentity;
                Ctx ctx = null;
                if (ident != null)
                {
                    FormsAuthenticationTicket ticket = ident.Ticket;
                    string loginContextString = ticket.UserData; // get login context string from Form Ticket
                    LoginContext lCtx = JsonHelper.Deserialize(loginContextString, typeof(LoginContext)) as LoginContext;
                    IDataReader dr = null;





                    //create class from string  
                    string key = "";
                    if (Session["ctx"] != null)
                    {
                        ctx = Session["ctx"] as Ctx;

                        //Update Old Session
                        MSession session = MSession.Get(ctx, false);
                        if (session != null)
                            session.Logout();

                        key = ctx.GetSecureKey();

                        //if (Session.Timeout < 2)
                        //{
                        SessionEventHandler.SessionEnd(ctx);
                        Session.Timeout = 17;
                        //}
                        Session["ctx"] = null;

                    }
                    ctx = new Ctx(lCtx.ctxMap); //cretae new context

                    /* fix for User Value Null value */

                    if (string.IsNullOrEmpty(ctx.GetContext("##AD_User_Value")))
                    {
                        return new AccountController().LogOff();

                    }

                    if (key != "")
                    {
                        ctx.SetSecureKey(key);
                    }
                    Session["ctx"] = ctx;

                    //get login Language object on server
                    var loginLang = ctx.GetAD_Language();

                    Language l = Language.GetLanguage(ctx.GetAD_Language()); //Language.GetLoginLanguage();
                    l = VAdvantage.Utility.Env.VerifyLanguage(ctx, l);

                    ctx.SetContext(VAdvantage.Utility.Env.LANGUAGE, l.GetAD_Language());
                    ctx.SetContext(VAdvantage.Utility.Env.ISRIGHTTOLEFT, VAdvantage.Utility.Env.IsRightToLeft(loginLang) ? "Y" : "N");
                    new VAdvantage.Login.LoginProcess(ctx).LoadSysConfig();
                    LoginHelper.SetSysConfigInContext(ctx);

                    ViewBag.culture = ctx.GetAD_Language();
                    ViewBag.direction = ctx.GetIsRightToLeft() ? "rtl" : "ltr";

                    //Change Authentication
                    model = new LoginModel();
                    model.Login1Model = new Login1Model();
                    model.Login2Model = new Login2Model();
                    model.Login1Model.UserValue = ctx.GetContext("##AD_User_Value");
                    model.Login1Model.DisplayName = ctx.GetContext("##AD_User_Name");
                    model.Login1Model.LoginLanguage = ctx.GetAD_Language();

                    model.Login2Model.Role = ctx.GetAD_Role_ID().ToString();
                    model.Login2Model.Client = ctx.GetAD_Client_ID().ToString();
                    model.Login2Model.Org = ctx.GetAD_Org_ID().ToString();
                    model.Login2Model.Warehouse = ctx.GetAD_Warehouse_ID().ToString();


                    var RoleList = new List<KeyNamePair>();
                    var ClientList = new List<KeyNamePair>();
                    var OrgList = new List<KeyNamePair>();
                    var WareHouseList = new List<KeyNamePair>();
                    string username = "";
                    IDataReader drRoles = LoginHelper.GetRoles(model.Login1Model.UserValue, false, false);
                    int AD_User_ID = 0;
                    if (drRoles.Read())
                    {
                        do  //	read all roles
                        {
                            AD_User_ID = Util.GetValueOfInt(drRoles[0].ToString());
                            int AD_Role_ID = Util.GetValueOfInt(drRoles[1].ToString());
                            String Name = drRoles[2].ToString();
                            KeyNamePair p = new KeyNamePair(AD_Role_ID, Name);
                            RoleList.Add(p);
                            username = Util.GetValueOfString(drRoles["username"].ToString());
                        }
                        while (drRoles.Read());
                    }
                    drRoles.Close();

                    model.Login1Model.AD_User_ID = AD_User_ID;
                    model.Login1Model.DisplayName = username;

                    //string diableMenu = ctx.GetContext("#DisableMenu");
                    Helpers.MenuHelper mnuHelper = new Helpers.MenuHelper(ctx); // inilitilize menu class

                    bool disableMenu = MRole.GetDefault(ctx).IsDisableMenu();
                    ctx.SetIsBasicDB(mnuHelper.GetIsBasicDB());


                    // If Home page not linked OR home page Linked BUT Menu is not disabled , then show home page.
                    // If Home is linked as well as menu is disabled then don't load Default Home Page Settings
                    if (MRole.GetDefault(ctx).GetHomePage_ID() == 0 || (MRole.GetDefault(ctx).GetHomePage_ID() > 0 && !disableMenu))
                    {
                        HomeModels hm = new HomeModels();
                        objHomeHelp = new HomeHelper();
                        hm = objHomeHelp.getLoginUserInfo(ctx, 32, 32);
                        ViewBag.UserPic = hm.UsrImage;
                    }
                    ViewBag.DisplayName = model.Login1Model.DisplayName;

                    if (!disableMenu) // if menu is not disabled, only then load menu.
                    {
                        //get current user info
                        ViewBag.Menu = mnuHelper.GetMenuTree(); // create tree
                        Session["barNodes"] = ViewBag.Menu.GetBarNodes(); /* add is session to get it in favourite call */
                        ViewBag.IsMobile = Request.Browser.IsMobileDevice;
                        if (Request.Browser.IsMobileDevice)
                        {
                            ViewBag.TreeHtml = mnuHelper.GetMobileMenuTreeUI(ViewBag.Menu.GetRootNode(), @Url.Content("~/"));
                        }
                        else
                        {
                            ViewBag.TreeHtml = mnuHelper.GetNewMenuTreeUI(ViewBag.Menu.GetRootNode(), @Url.Content("~/"));
                        }
                    }

                    ViewBag.disableMenu = disableMenu;

                    mnuHelper.dispose();

                    //  LoginHelper.GetClients(id)

                    ClientList = LoginHelper.GetClients(ctx.GetAD_Role_ID());// .Add(new KeyNamePair(ctx.GetAD_Client_ID(), ctx.GetAD_Client_Name()));
                    OrgList = LoginHelper.GetOrgs(ctx.GetAD_Role_ID(), ctx.GetAD_User_ID(), ctx.GetAD_Client_ID());// .Add(new KeyNamePair(ctx.GetAD_Org_ID(), ctx.GetAD_Org_Name()));
                    WareHouseList = LoginHelper.GetWarehouse(ctx.GetAD_Org_ID());// .Add(new KeyNamePair(ctx.GetAD_Warehouse_ID(), ctx.GetContext("#M_Warehouse_Name")));


                    ViewBag.RoleList = RoleList;
                    ViewBag.ClientList = ClientList;
                    ViewBag.OrgList = OrgList;
                    ViewBag.WarehouseList = WareHouseList;
                    lock (_lock)    // Locked bundle Object and session Creation to handle concurrent requests.
                    {
                        //Cretae new Sessin
                        MSession sessionNew = MSession.Get(ctx, true, GetVisitorIPAddress(true));
                        ModelLibrary.PushNotif.SessionData sessionData = new ModelLibrary.PushNotif.SessionData();
                        sessionData.UserId = ctx.GetAD_User_ID();
                        sessionData.Name = ctx.GetAD_User_Name();
                        ModelLibrary.PushNotif.SessionManager.Get().AddSession(ctx.GetAD_Session_ID(), sessionData);

                        var lst = VAdvantage.ModuleBundles.GetStyleBundles(); //Get All Style Bundle
                        foreach (var b in lst)
                        {
                            if (!BundleTable.Bundles.Contains(b))
                            {
                                BundleTable.Bundles.Add(b); //Add in Mvc Bundle Table
                            }
                        }

                        var lstRTLStyle = VAdvantage.ModuleBundles.GetRTLStyleBundles(); //Get All Script Bundle

                        foreach (var b in lstRTLStyle)
                        {
                            if (!BundleTable.Bundles.Contains(b))
                            {
                                BundleTable.Bundles.Add(b); //Add in Mvc Bundlw Table
                            }
                        }

                        var lstScript = VAdvantage.ModuleBundles.GetScriptBundles(); //Get All Script Bundle

                        foreach (var b in lstScript)
                        {
                            if (!BundleTable.Bundles.Contains(b))
                            {
                                BundleTable.Bundles.Add(b); //Add in Mvc Bundlw Table
                            }
                        }

                        ViewBag.LibSuffix = "";
                        ViewBag.FrameSuffix = "_v1";
                        int libFound = 0;
                        foreach (Bundle b in BundleTable.Bundles)
                        {
                            if (b.Path.Contains("ViennaBase") && b.Path.Contains("_v") && ViewBag.LibSuffix == "")
                            {
                                ViewBag.LibSuffix = Util.GetValueOfInt(ctx.GetContext("#FRONTEND_LIB_VERSION")) > 2
                                                      ? "_v3" : "_v2";
                                libFound++;
                            }

                            if (b.Path.Contains("VIS") && b.Path.Contains("_v"))
                            {
                                ViewBag.FrameSuffix = Util.GetValueOfInt(ctx.GetContext("#FRAMEWORK_VERSION")) > 1
                                                      ? "_v2" : "_v1";
                                libFound++;
                            }
                            if (libFound >= 2)
                            {
                                break;
                            }
                        }
                        //check system setting// set to skipped lib


                    }

                    /// VIS0008
                    /// Check applied for adding message to toastr if 2FA method is VA and VA App is not linked with device
                    if (!LoginHelper.IsDeviceLinked(ctx, AD_User_ID))
                        ModelLibrary.PushNotif.SSEManager.Get().AddMessage(ctx.GetAD_Session_ID(), Msg.GetMsg(ctx, "PlzLinkVAApp"));

                    VAdvantage.Classes.ThreadInstance.Get().Start();
                }
            }

            else
            {

                model = new LoginModel();
                model.Login1Model = new Login1Model();
                if (Request.QueryString.Count > 0) /* if query has values*/
                {
                    try
                    {
                        // VIS0008
                        // Changes done to handle TOKEN in querystring
                        string TokenKey = "";
                        foreach (string key in Request.QueryString.AllKeys)
                        {
                            if (key.ToLower() == "token")
                                TokenKey = Request.QueryString[key];
                        }
                        if (TokenKey != "")
                        {
                            Dictionary<string, object> tokDetails = LoginHelper.GetTokenDetails(TokenKey);
                            if (Util.GetValueOfBool(tokDetails["Success"]))
                            {
                                TempData["user"] = tokDetails["User"]; //get uservalue
                                TempData["pwd"] = SecureEngine.Decrypt(tokDetails["Password"]);//get userpwd
                            }
                        }
                        else
                        {
                            TempData["user"] = SecureEngine.Decrypt(Request.QueryString["U"]); //get uservalue
                            TempData["pwd"] = SecureEngine.Decrypt(Request.QueryString["P"]);//get userpwd
                        }
                    }
                    catch
                    {
                        TempData.Clear();
                    }
                    return RedirectToAction("Index"); // redirect to same url to remove cookie
                }

                if (TempData.ContainsKey("user"))
                {
                    model.Login1Model.UserValue = TempData["user"].ToString() + "^Y^" + TempData["pwd"].ToString();
                    // model.Login1Model.Password = TempData.Peek("pwd").ToString();
                }

                model.Login1Model.LoginLanguage = "en_US";
                model.Login2Model = new Login2Model();

                ViewBag.RoleList = new List<KeyNamePair>();
                ViewBag.OrgList = new List<KeyNamePair>();
                ViewBag.WarehouseList = new List<KeyNamePair>();
                ViewBag.ClientList = new List<KeyNamePair>();

                ViewBag.Languages = Language.GetLanguages();

                Session["ctx"] = null;
                ViewBag.direction = "ltr";

                ViewBag.LibSuffix = "";
                foreach (Bundle b in BundleTable.Bundles)
                {
                    if (b.Path.Contains("ViennaBase") && b.Path.Contains("_v"))
                    {
                        ViewBag.LibSuffix = "_v2";
                        break;
                    }
                }
            }
            return View(model);

        }

        /// <summary>
        /// method to get Client ip address
        /// </summary>
        /// <param name="GetLan"> set to true if want to get local(LAN) Connected ip address</param>
        /// <returns></returns>
        public string GetVisitorIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                GetLan = true;
                visitorIPAddress = string.Empty;
            }

            if (GetLan)
            {
                if (string.IsNullOrEmpty(visitorIPAddress))
                {
                    //This is for Local(LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    //Get Ip Host Entry
                    IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                    //Get Ip Address From The Ip Host Entry Address List
                    IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                    try
                    {
                        visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                    }
                    catch
                    {
                        try
                        {
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                visitorIPAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }
            return visitorIPAddress;
        }



        /* Main Screen */



        //To Load first time partial view ex:- Home
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Home()
        {



            Ctx ct = Session["ctx"] as Ctx;
            ViewBag.Current_Ad_Lang = ct.GetAD_Language();
            objHomeHelp = new HomeHelper();
            HomeModels HM = objHomeHelp.getHomeAlrtCount(ct);

            objHomeHelp = new HomeHelper();
            HomeFolloUpsInfo fllInfo = objHomeHelp.getFolloUps(ct, 10, 1);
            HM.HomeFolloUpsInfo = fllInfo;
            ViewBag.lang = ct.GetAD_Language();
            ViewBag.User_ID = ct.GetAD_User_ID();
            ViewBag.isRTL = ct.GetIsRightToLeft();

            string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
            storedPath += "LOG_";
            VLogMgt.Initialize(true, storedPath);

            return PartialView(HM);
        }

        //public ActionResult Manifest()
        //{
        //    Response.ContentType = "text/cache-manifest";
        //    Response.ContentEncoding = System.Text.Encoding.UTF8;
        //    Response.Cache.SetCacheability(
        //        System.Web.HttpCacheability.NoCache);
        //    return View();
        //}


        #region Follups start
        /*----------------Folloups Strat-----------------------*/
        // Get Folloups
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetJSONFllups(int fllPageSize, int fllPage, Boolean isRef)
        {
            HomeFolloUpsInfo fllInfo = null;

            objHomeHelp = new HomeHelper();
            if (Session["ctx"] != null)
            {
                Ctx ct = Session["ctx"] as Ctx;

                fllInfo = new HomeFolloUpsInfo();
                fllInfo = objHomeHelp.getFolloUps(ct, fllPageSize, fllPage);
                if (isRef)
                {
                    fllInfo.FllCnt = objHomeHelp.getFllCnt(ct);
                }
            }
            return Json(JsonConvert.SerializeObject(fllInfo), JsonRequestBehavior.AllowGet);
        }

        //GetFolloups Cmnt
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetJSONFllupsCmnt(string FllupsID, int fllCmntPageSize, int fllCmntPage)
        {
            objHomeHelp = new HomeHelper();
            Ctx ct = Session["ctx"] as Ctx;
            HomeFolloUpsInfo fllInfo = null;
            if (Session["ctx"] != null)
            {
                int usr_Role_ID = ct.GetAD_Role_ID();
                objHomeHelp = new HomeHelper();
                string[] arr = FllupsID.Split('-');
                int ChatID = Util.GetValueOfInt(arr[0]);
                int RecordID = Util.GetValueOfInt(arr[1]);
                int SubscriberID = Util.GetValueOfInt(arr[2]);
                int TableID = Util.GetValueOfInt(arr[3]);
                int WinID = Util.GetValueOfInt(arr[4]);
                fllInfo = new HomeFolloUpsInfo();
                fllInfo = objHomeHelp.getFolloUpsCmnt(ct, ChatID, RecordID, SubscriberID, TableID, WinID, usr_Role_ID, fllCmntPageSize, fllCmntPage);
            }
            return Json(JsonConvert.SerializeObject(fllInfo), JsonRequestBehavior.AllowGet);
        }

        //Save Folloups comment
        [HttpPost]
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult PostFllupsCmnt(int fllChatID, int fllSubscriberID, string cmntTxt)
        {

            objHomeHelp = new HomeHelper();
            Ctx ctx = Session["ctx"] as Ctx;
            int usr_Role_ID = ctx.GetAD_Role_ID();
            objHomeHelp.SaveFllupsCmnt(ctx, fllChatID, fllSubscriberID, cmntTxt);
            objHomeHelp = new HomeHelper();
            HomeModels hm = new HomeModels();
            hm = objHomeHelp.getLoginUserInfo(ctx, 46, 46);
            // ViewBag.UserPic = hm.UsrImage;
            return Json(hm.UsrImage, JsonRequestBehavior.AllowGet);
            //return Json(JsonConvert.SerializeObject(hm.UsrImage), JsonRequestBehavior.AllowGet);
        }
        /*----------------Folloups End-----------------------*/
        #endregion

        #region Notice
        //Get Notice List
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetJSONHomeNotice(int pagesize, int page, Boolean isTabDataRef)
        {
            List<HomeNotice> lst = null;
            int count = 0;
            string error = "";
            if (Session["ctx"] != null)
            {
                objHomeHelp = new HomeHelper();
                lst = new List<HomeNotice>();
                Ctx ct = Session["ctx"] as Ctx;
                if (isTabDataRef)
                {
                    count = objHomeHelp.getNoticeCnt(ct);
                }
                lst = objHomeHelp.getHomeNotice(ct, pagesize, page);

            }
            else
            {
                error = "Session Expired";
            }
            return Json(new { count = count, data = JsonConvert.SerializeObject(lst), error = error }, JsonRequestBehavior.AllowGet);
        }

        //Approve Notice
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult ApproveNotice(int Ad_Note_ID, bool isAcknowldge)
        {
            objHomeHelp = new HomeHelper();
            Ctx ct = Session["ctx"] as Ctx;
            var res = objHomeHelp.ApproveNotice(ct, Ad_Note_ID, isAcknowldge);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Request
        //get request
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetJSONHomeRequest(int pagesize, int page, Boolean isTabDataRef)
        {
            int count = 0;
            List<HomeRequest> lst = null;
            string error = "";
            if (Session["ctx"] != null)
            {
                objHomeHelp = new HomeHelper();
                Ctx ct = Session["ctx"] as Ctx;
                lst = new List<HomeRequest>();
                if (isTabDataRef)
                {
                    count = objHomeHelp.getRequestCnt(ct);
                }
                lst = objHomeHelp.getHomeRequest(ct, pagesize, page);
            }
            else
            {
                error = "Session Expired";
            }
            return Json(new { count = count, data = JsonConvert.SerializeObject(lst), error = error }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Save User Image
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult SaveImageAsByte(HttpPostedFileBase file)
        {
            try
            {
                HttpPostedFileBase hpf = file as HttpPostedFileBase;
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"), "Temp")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images"), "Temp"));       //Create Thumbnail Folder if doesnot exists
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Images/Temp"), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName);
                MemoryStream ms = new MemoryStream();
                hpf.InputStream.CopyTo(ms);
                byte[] byteArray = ms.ToArray();
                FileInfo file1 = new FileInfo(savedFileName);
                if (file1.Exists)
                {
                    file1.Delete(); //Delete Temporary file             
                }
                Ctx ctx = Session["ctx"] as Ctx;
                //string imgByte = Convert.ToBase64String(byteArray);
                HomeModels objHomeModel = new HomeModels();
                int imageID = objHomeModel.SaveUserImage(ctx, byteArray, hpf.FileName, false);
                if (imageID <= 0)
                {
                    return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);
                }
                MImage objImage = new MImage(ctx, imageID, null);
                string imgByte = Convert.ToBase64String(objImage.GetThumbnailByte(140, 120));
                return Json(JsonConvert.SerializeObject(imgByte), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);
            }

        }

        public byte[] ConvertStringToByteArray(string byteArray)
        {
            string[] abc = byteArray.Split(':', '"', '\\', ',', '{', '}', '[', ' ', ']');
            List<string> array = new List<string>();
            List<int> finalList = new List<int>();
            for (int i = 0; i < abc.Length; i++)
            {
                if (!(abc[i] == ""))
                {
                    array.Add(abc[i]);
                }
            }
            for (int i = 0; i < array.Count; i++)
            {
                if (i % 2 != 0)
                {
                    finalList.Add(Convert.ToInt32(array[i]));

                }
            }
            byte[] finalArray = new byte[finalList.Count];
            for (int i = 0; i < finalList.Count; i++)
            {
                finalArray[i] = (byte)(finalList[i]);
            }
            return finalArray;
        }
        string format = string.Empty;
        public byte[] ConvertByteArrayToThumbnail(byte[] imageBytes)
        {
            Ctx ct = Session["ctx"] as Ctx;
            WebImage wi = new WebImage(imageBytes);
            wi.Resize(30, 30);
            string filepath = Path.Combine(HostingEnvironment.MapPath(@"~/Images/30by30"), ct.GetAD_User_ID().ToString());
            wi.Save(filepath);
            wi = new WebImage(imageBytes);
            wi.Resize(100, 100);
            filepath = Path.Combine(HostingEnvironment.MapPath(@"~/Images/100by100"), ct.GetAD_User_ID().ToString());
            wi.Save(filepath);
            format = "." + wi.ImageFormat;
            return wi.GetBytes();

        }

        #endregion


        #region "Shortcut Favourite"

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public ActionResult GetShortcutItems()
        {
            // List<ShortcutItemModel> lst =  ShortcutHelper.GetShortcutItems(Session["ctx"] as Ctx);
            return Json(JsonConvert.SerializeObject(ShortcutHelper.GetShortcutItems(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public ActionResult GetSettingItems(int AD_Shortcut_ID)
        {
            return Json(JsonConvert.SerializeObject(ShortcutHelper.GetSettingItems(Session["ctx"] as Ctx, AD_Shortcut_ID)), JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult TimeoutRedirect()
        {
            return View();
        }


        #region "Favourite Node"
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetFavouriteNode()
        {
            try
            {
                //Helpers.MenuHelper mnuHelper = new Helpers.MenuHelper(Session["ctx"] as Ctx); // inilitilize menu class
                Helpers.HomeHelper homeHelper = new HomeHelper();
                var nodes = Session["barNodes"] as List<VTreeNode>;

                if (nodes == null)
                {
                    Ctx ctx = Session["ctx"] as Ctx;
                    //string diableMenu = ctx.GetContext("#DisableMenu");
                    Helpers.MenuHelper mnuHelper = new Helpers.MenuHelper(ctx); // inilitilize menu class
                    ViewBag.Menu = mnuHelper.GetMenuTree(); // create tree
                    nodes = ViewBag.Menu.GetBarNodes();
                }
                Session["barNodes"] = null;
                return Json(new { result = homeHelper.GetBarNodes(nodes) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SetNodeFavourite(int nodeID)
        {
            try
            {

                Helpers.HomeHelper homeHelper = new HomeHelper();
                return Json(new { result = homeHelper.SetNodeFavourite(nodeID, Session["ctx"] as Ctx) }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return null;
            }
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult RemoveNodeFavourite(int nodeID)
        {
            try
            {

                Helpers.HomeHelper homeHelper = new HomeHelper();
                return Json(new { result = homeHelper.RemoveNodeFavourite(nodeID, Session["ctx"] as Ctx) }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Save User Image
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult SaveStatus(string status)
        {
            Ctx ctx = null;
            if (Session["Ctx"] != null)
            {
                ctx = Session["ctx"] as Ctx;

            }
            MUser objUser = new MUser(ctx, ctx.GetAD_User_ID(), null);
            objUser.SetComments(status);
            if (!objUser.Save())
            {
                return Json(JsonConvert.SerializeObject("Error"), JsonRequestBehavior.AllowGet);
            }
            return Json(JsonConvert.SerializeObject(status), JsonRequestBehavior.AllowGet);
        }
        #endregion

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetSubscriptionDaysLeft()
        {
            HomeHelper hel = new HomeHelper();
            return Json(JsonConvert.SerializeObject(hel.GetSubscriptionDaysLeft(Request.UrlReferrer.ToString())), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReports()
        {
            List<KeyNamePair> lst = new List<KeyNamePair>();
            var dr = VAdvantage.DataBase.DB.ExecuteReader("SELECT AD_PROCESS_ID, Name FROM AD_PROCESS WHERE IsReport='Y' AND IsActive='Y'");
            if (dr != null)
            {
                while (dr.Read())
                {
                    int AD_Org_ID = Util.GetValueOfInt(dr[0].ToString());
                    String Name = dr[1].ToString();
                    KeyNamePair p = new KeyNamePair(AD_Org_ID, Name);
                    if (!lst.Contains(p))
                        lst.Add(p);
                }
            }
            dr.Close();

            return Json(JsonConvert.SerializeObject(lst), JsonRequestBehavior.AllowGet);
        }

    }




}
