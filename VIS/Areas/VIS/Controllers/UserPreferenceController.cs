using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Newtonsoft.Json;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VIS.Classes;
using VIS.Models;

namespace VIS.Controllers
{
    public class UserPreferenceController : Controller
    {
        ////
        //// GET: /VIS/UserPreference/
        //[HttpGet]
        //[AllowAnonymous]
        public ActionResult Index(string windowno, string adUserId)
        {
            ViewBag.WindowNumber = windowno;
            
            UserPreferenceModel obj = new UserPreferenceModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                obj = obj.GetUserSettings(ctx, Convert.ToInt32(adUserId));
                ViewBag.lang = ctx.GetAD_Language();
                ViewBag.IsAdmin = ctx.GetAD_Role_ID() == 0 && (ctx.GetAD_User_ID() == 100 || ctx.GetAD_User_ID() == 0)
                    && Util.GetValueOfInt(ctx.GetContext("#FRAMEWORK_VERSION")) > 1;
            }
            
            return PartialView(obj);
        }

        /// <summary>
        /// save prefrences into database
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SavePrefrence(Dictionary<string, object> pref)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                UserPreferenceModel obj = new UserPreferenceModel();
                obj.SavePrefrence(ctx, pref);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save User settings into database
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveUserSettings(int AD_User_ID, string currentPws, string newPws, bool chkEmail, bool chkNotice,
            bool chkSMS, bool chkFax, string emailUserName, string emailPws,int AD_Role_ID,int AD_Client_ID,int AD_Org_ID, int M_Warehouse_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                UserPreferenceModel obj = new UserPreferenceModel();
                var val = obj.SaveUserSettings(ctx, AD_User_ID, currentPws, newPws, chkEmail, chkNotice, chkSMS, chkFax, emailUserName, emailPws,AD_Role_ID,AD_Client_ID,AD_Org_ID, M_Warehouse_ID);
                return Json(new { result = val }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveChangePassword(int AD_User_ID, string currentPws, string newPws)
        //{
        //    if (Session["Ctx"] != null)
        //    {
        //        var ctx = Session["ctx"] as Ctx;
        //        UserPreferenceModel obj = new UserPreferenceModel();
        //        var val = obj.SaveChangePassword(ctx, AD_User_ID, currentPws, newPws);
        //        return Json(new { result = val }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Change login user password 
        /// </summary>
        /// <param name="AD_User_ID"></param>
        /// <param name="currentPws"></param>
        /// <param name="newPws"></param>
        /// <returns></returns>
        public JsonResult SaveChangePassword(int AD_User_ID, string currentPws, string newPws)
        {
            string message = string.Empty;
            if (Session["Ctx"] != null)
            {
                String msg = "";
                var ctx = Session["ctx"] as Ctx;
                
                var AD_Process_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_PROCESS_ID  FROM AD_PROCESS WHERE NAME = 'Reset Your Password'", null, null)); // Get Reset Your Password process id 
                // Prepare Process
                MPInstance instance = new MPInstance(ctx, AD_Process_ID, 0); // create object of MPInstance
                if (!instance.Save())
                {
                    msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
                }
                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo("ChangePassword", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());
                pi.SetAD_User_ID(AD_User_ID);
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                // Add Parameter - CurrentPassword
                MPInstancePara para = new MPInstancePara(instance, 10);

                para.setParameter("CurrentPassword", currentPws);
                if (!para.Save())
                {
                     msg = "No Selection Parameter added";  //  not translated
                   // msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);

                }
                // Add Parameter - NewPassword
                para = new MPInstancePara(instance, 20);
                para.setParameter("NewPassword", newPws);
                if (!para.Save())
                {
                     msg = "No DocAction Parameter added";  //  not translated
                   // msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
                }
                para = new MPInstancePara(instance, 30);
                para.setParameter("AD_User_ID", AD_User_ID);
                if (!para.Save())
                {
                    msg = "No DocAction Parameter added";  //  not translated
                    //msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
                }
                ASyncProcess _parent = null;
                // Execute Process
                 ProcessCtl worker = new ProcessCtl( ctx,_parent, pi, null);
               
                worker.Run();     //  complete tasks in unlockUI / generateInvoice_complete
                message = pi.GetSummary();
                
              
            }
            return Json(new { result = message }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSavedDetail(bool isTask)
        {
            
            string retJSON = "";
            Ctx ctx = null;
            if (Session["Ctx"] != null)
            {
                ctx = Session["ctx"] as Ctx;

            }
          
            UserPreferenceModel objUPModel = new UserPreferenceModel();
            retJSON = JsonConvert.SerializeObject(objUPModel.GetSavedDetail(ctx,isTask));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
         
        }
         public JsonResult GetGmailAuthorizationCode(bool isTask,bool isContact)
         {
             string scope = string.Empty;
             if (isTask)
             {
                 scope = "https://www.googleapis.com/auth/tasks";
             }
             else if (isContact)
             {
                 scope = "https://www.googleapis.com/auth/contacts.readonly";
             }
             else
             {
                 scope = "https://www.googleapis.com/auth/calendar";
             }
             OAuth2Parameters parameters = new OAuth2Parameters()
             {
                 //Client 
                 ClientId = ClientCredentials.ClientID,
                 ClientSecret = ClientCredentials.ClientSecret,
                 RedirectUri = "urn:ietf:wg:oauth:2.0:oob",
               //  Scope = "https://www.googleapis.com/auth/tasks",
                 Scope = scope,
             };

             //User clicks this auth url and will then be sent to your redirect url with a code parameter
             var authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
             //string retError = "";
             string retJSON = "";
             retJSON = JsonConvert.SerializeObject(authorizationUrl);
             return Json(retJSON, JsonRequestBehavior.AllowGet);
            
         }

         public JsonResult SaveGmailAccountSettings(string authCodes,bool isTask,bool isSyncInBackground,bool isRemoveLink,bool isContact)
         {
             string scope = string.Empty;
             if (isTask)
             {
                 scope = "https://www.googleapis.com/auth/tasks";
             }
             else if (isContact)
             {
                 scope = "https://www.googleapis.com/auth/contacts.readonly";
             }
             else
             {
                 scope = "https://www.googleapis.com/auth/calendar";
             }
             string message = string.Empty;
             Ctx ctx = null;
             
             string retJSON = "";
             if (Session["Ctx"] != null)
             {
                  ctx = Session["ctx"] as Ctx;

             }

             if (authCodes == "")
             {
                 OAuth2Parameters parameters = new OAuth2Parameters()
                 {
                     //Client 
                     ClientId = ClientCredentials.ClientID,
                     ClientSecret = ClientCredentials.ClientSecret,
                     RedirectUri = "urn:ietf:wg:oauth:2.0:oob",
                     //Scope = "https://www.googleapis.com/auth/tasks",
                     Scope = scope,
                 };

                 //User clicks this auth url and will then be sent to your redirect url with a code parameter
                 var authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);                 
                 retJSON = JsonConvert.SerializeObject(authorizationUrl);
                 return Json(retJSON, JsonRequestBehavior.AllowGet);
             }
           

             int AD_User_ID=ctx.GetAD_User_ID();
             int AD_Client_ID=ctx.GetAD_Client_ID();
             int AD_Org_ID = ctx.GetAD_Org_ID();
             if (isRemoveLink)
             {
                 UserPreferenceModel objUPModel1 = new UserPreferenceModel();
                 message = objUPModel1.Action(ctx, isSyncInBackground, isTask, authCodes, isRemoveLink);
                 retJSON = JsonConvert.SerializeObject(message);
                 return Json(retJSON, JsonRequestBehavior.AllowGet);
             }
             GmailConfig objGmailConfig = new GmailConfig(AD_User_ID, AD_Client_ID, AD_Org_ID, authCodes, isTask,isContact);
             message = objGmailConfig.Start(ctx);
             if (message.Contains("error"))
             {
                 return Json("false", JsonRequestBehavior.AllowGet);
             }
             UserPreferenceModel objUPModel = new UserPreferenceModel();
             message= objUPModel.Action(ctx, isSyncInBackground, isTask,authCodes,isRemoveLink);
             retJSON = JsonConvert.SerializeObject(message);
             return Json(retJSON, JsonRequestBehavior.AllowGet);
         }

         public JsonResult GetLoginData(string sql)
         {

             UserPreferenceModel objUPModel = new UserPreferenceModel();
             return Json(JsonConvert.SerializeObject(objUPModel.GetLoginData(sql)), JsonRequestBehavior.AllowGet);
         }

         public JsonResult GetDefaultLogin(int AD_User_ID)
         {

             UserPreferenceModel objUPModel = new UserPreferenceModel();
             return Json(JsonConvert.SerializeObject(objUPModel.GetDefaultLogin(AD_User_ID)), JsonRequestBehavior.AllowGet);
         }

         // Added by Bharat on 12 June 2017
         public JsonResult GetWindowID(string WindowName)
         {
             Ctx ct = Session["ctx"] as Ctx;
             UserPreferenceModel model = new UserPreferenceModel();
             return Json(JsonConvert.SerializeObject(model.GetWindowID(WindowName)), JsonRequestBehavior.AllowGet);
         }

        /// <summary>
        /// Controller function to zip log file for the selected date passed as parameter
        /// </summary>
        /// <param name="logDate">Log Date</param>
        /// <returns>String (name of zip file)</returns>
         public JsonResult DownloadServerLog(DateTime? logDate)
         {
             Ctx ct = Session["ctx"] as Ctx;
             UserPreferenceModel model = new UserPreferenceModel();
             return Json(model.DownloadServerLog(logDate), JsonRequestBehavior.AllowGet);
         }
    }
}
