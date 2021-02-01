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
                ViewBag.lang = ctx.GetVAF_Language();
                ViewBag.IsAdmin = ctx.GetVAF_Role_ID() == 0 && (ctx.GetVAF_UserContact_ID() == 100 || ctx.GetVAF_UserContact_ID() == 0)
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
        public JsonResult SaveUserSettings(int VAF_UserContact_ID, string currentPws, string newPws, bool chkEmail, bool chkNotice,
            bool chkSMS, bool chkFax, string emailUserName, string emailPws,int VAF_Role_ID,int VAF_Client_ID,int VAF_Org_ID, int M_Warehouse_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                UserPreferenceModel obj = new UserPreferenceModel();
                var val = obj.SaveUserSettings(ctx, VAF_UserContact_ID, currentPws, newPws, chkEmail, chkNotice, chkSMS, chkFax, emailUserName, emailPws,VAF_Role_ID,VAF_Client_ID,VAF_Org_ID, M_Warehouse_ID);
                return Json(new { result = val }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveChangePassword(int VAF_UserContact_ID, string currentPws, string newPws)
        //{
        //    if (Session["Ctx"] != null)
        //    {
        //        var ctx = Session["ctx"] as Ctx;
        //        UserPreferenceModel obj = new UserPreferenceModel();
        //        var val = obj.SaveChangePassword(ctx, VAF_UserContact_ID, currentPws, newPws);
        //        return Json(new { result = val }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Change login user password 
        /// </summary>
        /// <param name="VAF_UserContact_ID"></param>
        /// <param name="currentPws"></param>
        /// <param name="newPws"></param>
        /// <returns></returns>
        public JsonResult SaveChangePassword(int VAF_UserContact_ID, string currentPws, string newPws)
        {
            string message = string.Empty;
            if (Session["Ctx"] != null)
            {
                String msg = "";
                var ctx = Session["ctx"] as Ctx;
                
                var VAF_Job_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_JOB_ID  FROM VAF_JOB WHERE NAME = 'Reset Your Password'", null, null)); // Get Reset Your Password process id 
                // Prepare Process
                MVAFJInstance instance = new MVAFJInstance(ctx, VAF_Job_ID, 0); // create object of MPInstance
                if (!instance.Save())
                {
                    msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
                }
                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo("ChangePassword", VAF_Job_ID);
                pi.SetVAF_JInstance_ID(instance.GetVAF_JInstance_ID());
                pi.SetVAF_UserContact_ID(VAF_UserContact_ID);
                pi.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                // Add Parameter - CurrentPassword
                MVAFJInstancePara para = new MVAFJInstancePara(instance, 10);

                para.setParameter("CurrentPassword", currentPws);
                if (!para.Save())
                {
                     msg = "No Selection Parameter added";  //  not translated
                   // msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);

                }
                // Add Parameter - NewPassword
                para = new MVAFJInstancePara(instance, 20);
                para.setParameter("NewPassword", newPws);
                if (!para.Save())
                {
                     msg = "No DocAction Parameter added";  //  not translated
                   // msg = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
                }
                para = new MVAFJInstancePara(instance, 30);
                para.setParameter("VAF_UserContact_ID", VAF_UserContact_ID);
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
           

             int VAF_UserContact_ID=ctx.GetVAF_UserContact_ID();
             int VAF_Client_ID=ctx.GetVAF_Client_ID();
             int VAF_Org_ID = ctx.GetVAF_Org_ID();
             if (isRemoveLink)
             {
                 UserPreferenceModel objUPModel1 = new UserPreferenceModel();
                 message = objUPModel1.Action(ctx, isSyncInBackground, isTask, authCodes, isRemoveLink);
                 retJSON = JsonConvert.SerializeObject(message);
                 return Json(retJSON, JsonRequestBehavior.AllowGet);
             }
             GmailConfig objGmailConfig = new GmailConfig(VAF_UserContact_ID, VAF_Client_ID, VAF_Org_ID, authCodes, isTask,isContact);
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

         public JsonResult GetDefaultLogin(int VAF_UserContact_ID)
         {

             UserPreferenceModel objUPModel = new UserPreferenceModel();
             return Json(JsonConvert.SerializeObject(objUPModel.GetDefaultLogin(VAF_UserContact_ID)), JsonRequestBehavior.AllowGet);
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
