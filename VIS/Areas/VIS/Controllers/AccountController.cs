using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

using System.Web;
using VIS.Models;
using VIS.Helpers;
using VAdvantage.Model;
using VIS.Filters;
using VAdvantage.Utility;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;

namespace VIS.Controllers
{

    public class AccountController : Controller
    {
        //
        // POST: /Account/JsonLogin

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //if (model.Login1Model.Password == null && model.Login1Model.ResetPwd == false && model.Login1Model.Is2FAEnabled == false)
                //{
                //    ModelState.AddModelError("", "UserPwdError");
                //    return Json(new { errors = GetErrorsFromModelState() });
                //}

                VAdvantage.DataBase.DBConn.SetConnectionString();//Init database conection

                try
                {
                    List<KeyNamePair> roles = null;

                    // On rest pwd OR two FA or stpe 2, get values from temp data.
                    roles = (List<KeyNamePair>)TempData.Peek("roles");
                    model.Login2Model = (Login2Model)TempData.Peek("Login2Model");
                    bool resetPwd = Util.GetValueOfBool(TempData.Peek("ResetPwd"));
                    string TwoFAMethod = Util.GetValueOfString(TempData.Peek("TwoFAMethod"));
                    model.Login1Model.AD_User_ID = Util.GetValueOfInt(TempData.Peek("AD_User_ID"));
                    model.Login1Model.QRFirstTime = Util.GetValueOfBool(TempData.Peek("QRFirstTime"));
                    model.Login1Model.NoLoginSet = Util.GetValueOfBool(TempData.Peek("NoLoginSet"));
                    model.Login1Model.TokenKey2FA = Util.GetValueOfString(TempData.Peek("TokenKey2FA"));
                    model.Login1Model.QRCodeURL = Util.GetValueOfString(TempData.Peek("QRCodeURL"));

                    string password = Util.GetValueOfString(TempData.Peek("Password"));
                    if (!string.IsNullOrEmpty(model.Login1Model.Password))
                    {
                        password = model.Login1Model.Password;
                    }
                    //If value 0, then step1
                    // If value 2, then final login
                    //if value 1, then attemp login 1.
                    int proceedToLogin2 = 0;// First Login

                    // if user refresh page  while On reset pwd page, the system show login 1 page.
                    // In this case, reset pwd is true (picked from tempdata) but newpwd is null. 
                    //So user must try to login 1(processdToLogin = 1)
                    if (resetPwd && model.Login1Model.NewPassword != null)
                    {
                        string validated = LoginHelper.ValidatePassword(password, model.Login1Model.NewPassword, model.Login1Model.ConfirmNewPassword);
                        if (validated.Length > 0)
                        {
                            ModelState.AddModelError("", validated);
                            // If we got this far, something failed
                            return Json(new { errors = GetErrorsFromModelState() });
                        }
                        bool isUpdated = LoginHelper.UpdatePassword(model.Login1Model.NewPassword, model.Login1Model.AD_User_ID);
                        if (isUpdated)
                        {
                            proceedToLogin2 = 2;
                        }
                        if (TwoFAMethod != "" && !model.Login1Model.NoLoginSet)
                        {
                            model.Login1Model.TwoFAMethod = TwoFAMethod;
                            model.Login1Model.ResetPwd = false;
                            TempData.Remove("ResetPwd");
                            return Json(new { step2 = false, redirect = returnUrl, ctx = model.Login1Model });
                        }
                    }
                    else if (resetPwd)
                    {
                        model.Login1Model.Password = password;
                        proceedToLogin2 = 1;
                    }

                    // VIS0008 Changes done to handle VA 2FA from VA mobile app
                    if (TwoFAMethod != "" && proceedToLogin2 != 1)
                    {
                        if (!model.Login1Model.SkipNow)
                        {
                            if (model.Login1Model.Login1DataOTP != null && !model.Login1Model.ResendOTP)
                            {
                                var otp = model.Login1Model.OTP2FA;
                                model.Login1Model = JsonHelper.Deserialize(model.Login1Model.Login1DataOTP, typeof(Login1Model)) as Login1Model;
                                model.Login1Model.OTP2FA = otp;
                                bool valOTP = LoginHelper.Validate2FAOTP(model, TwoFAMethod);
                                if (valOTP)
                                {
                                    proceedToLogin2 = 2;
                                    model.Login1Model.TwoFAMethod = "";
                                }
                                else
                                {
                                    ModelState.AddModelError("", "WrongOTP");
                                    return Json(new { errors = GetErrorsFromModelState() });
                                }
                            }
                        }
                        else
                        {
                            proceedToLogin2 = 2;
                            model.Login1Model.TwoFAMethod = "";
                        }
                    }

                    if (proceedToLogin2 == 2)
                        return Login(model, returnUrl, roles);

                    //Pwd is assigned to tempdata, bcoz if user's passsword setting is encrypted, then code encrypt the pwd to use in query.
                    //But original pwd entered by user is required in reset pwd setup.. to match with new pwd.
                    TempData["Password"] = model.Login1Model.Password;
                    if (LoginHelper.Login(model, out roles))
                    {
                        // ViewBag.QRCodeURL = model.Login1Model.QRCodeURL;
                        TempData["roles"] = roles;
                        TempData["Login2Model"] = model.Login2Model;
                        TempData["ResetPwd"] = model.Login1Model.ResetPwd;
                        TempData["TwoFAMethod"] = model.Login1Model.TwoFAMethod;
                        TempData["AD_User_ID"] = model.Login1Model.AD_User_ID;
                        TempData["QRFirstTime"] = model.Login1Model.QRFirstTime;
                        TempData["NoLoginSet"] = model.Login1Model.NoLoginSet;
                        TempData["TokenKey2FA"] = model.Login1Model.TokenKey2FA;
                        TempData["QRCodeURL"] = model.Login1Model.QRCodeURL;
                        TempData["TwoFAMethod"] = model.Login1Model.TwoFAMethod;
                        //model.Login1Model.Password = null;
                        //if (!model.Login1Model.NoLoginSet && (model.Login1Model.ResetPwd || Util.GetValueOfString(model.Login1Model.TwoFAMethod) != ""))
                        if (model.Login1Model.ResetPwd
                            || (Util.GetValueOfString(model.Login1Model.TwoFAMethod) != "" && !model.Login1Model.NoLoginSet))
                        {
                            return Json(new { step2 = false, redirect = returnUrl, ctx = model.Login1Model });
                        }
                        return Login(model, returnUrl, roles);
                    }
                    else
                    {
                        ModelState.AddModelError("", "UserPwdError");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }
        [NonAction]
        private JsonResult Login(LoginModel model, string returnUrl, List<KeyNamePair> roles)
        {
            if (model.Login2Model != null)
            {
                //If everything is allright, then clear tempdata
                TempData.Clear();
                return JsonLogin2(model, "");
            }
            //System.Threading.Thread.Sleep(10000);
            //FormsAuthentication.SetAuthCookie(model.Login1Model.UserName, false);
            return Json(new { step2 = true, redirect = returnUrl, role = roles, ctx = model.Login1Model });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult JsonLogin2(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //System.Threading.Thread.Sleep(10000);
                bool saveSetting = false;
                if (model.Login1Model == null)
                {
                    model.Login1Model = JsonHelper.Deserialize(model.Login2Model.Login1Data, typeof(Login1Model)) as Login1Model;
                    saveSetting = true;
                }




                if (!string.IsNullOrEmpty(model.Login1Model.UserValue))
                {
                    if (Response != null)
                    {
                        SetAuthCookie(model, Response);
                    }


                    // Determine redirect URL and send user there

                    //string redirUrl = FormsAuthentication.GetRedirectUrl(model.Login1Model.UserName, false);

                    //RedirectToAction("Index", "Home");
                    // return;
                    //Response.Redirect(redirUrl);

                    //FormsAuthentication.SetAuthCookie(login1Model.UserName, false);

                    // TempData["LoginModel"] = model;


                    if (saveSetting)
                    {
                        var amgr = new AccountManager(model);
                        amgr.RunAsync();
                        amgr = null;
                    }


                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", "FillMandatoryFields");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        /// <summary>
        /// set authorize cookie in response object
        /// </summary>
        /// <param name="model">login model</param>
        /// <param name="response">http response</param>
        internal void SetAuthCookie(LoginModel model, HttpResponseBase response)
        {
            LoginContext lCtx = LoginHelper.GetLoginContext(model);
            response.Cookies.Clear();

            DateTime expiryDate = DateTime.Now.AddDays(30);

            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(model.Login1Model.UserValue, model.Login1Model.RememberMe);

            if (model.Login1Model.RememberMe)
            {
                authCookie.Expires = expiryDate;
            }

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, model.Login1Model.RememberMe ? expiryDate : ticket.Expiration, ticket.IsPersistent, JsonHelper.Serialize(lCtx));

            // Update the authCookie's Value to use the encrypted version of newTicket

            authCookie.Value = FormsAuthentication.Encrypt(newTicket);

            response.Cookies.Add(authCookie);
        }

        [AllowAnonymous]
        [HttpPost]
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult JsonChangeAuthentication(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //System.Threading.Thread.Sleep(10000);
                //model.Login1Model = JsonHelper.Deserialize(model.Login2Model.Login1Data, typeof(Login1Model)) as Login1Model;

                LoginContext lCtx = LoginHelper.GetLoginContext(model, Session["Ctx"] as VAdvantage.Utility.Ctx);


                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    //Response.Cookies.Clear();

                    //DateTime expiryDate = DateTime.Now.AddDays(30);

                    HttpCookie authCookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, true);// , User.Identity. .Name);

                    //if (model.Login1Model.RememberMe)
                    //{
                    //    authCookie.Expires = expiryDate;
                    //}

                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                    FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, JsonHelper.Serialize(lCtx));

                    // Update the authCookie's Value to use the encrypted version of newTicket

                    // authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                    //Response.Cookies.  .Add(authCookie);

                    Response.Cookies[".ASPXAUTH"].Value = FormsAuthentication.Encrypt(newTicket);

                    // Determine redirect URL and send user there

                    //string redirUrl = FormsAuthentication.GetRedirectUrl(model.Login1Model.UserName, false);

                    //RedirectToAction("Index", "Home");
                    // return;
                    //Response.Redirect(redirUrl);

                    //FormsAuthentication.SetAuthCookie(login1Model.UserName, false);

                    // TempData["LoginModel"] = model;
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", "FillMandatoryFields");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }



        [AllowAnonymous]
        public JsonResult GetOrgs(int role, int user, int client)
        {
            List<KeyNamePair> lst = LoginHelper.GetOrgs(role, user, client);
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetClients(int id)
        {
            List<KeyNamePair> lst = LoginHelper.GetClients(id);
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetWarehouse(int id)
        {
            List<KeyNamePair> lst = LoginHelper.GetWarehouse(id);
            return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        // [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();
            try
            {
                Ctx ctx = Session["ctx"] as Ctx;
                //VAdvantage.Logging.VLogMgt.Shutdown(ctx);
                // MSession s = MSession.Get(ctx);
                // s.Logout();
                if(ctx !=null)
                VAdvantage.Classes.SessionEventHandler.SessionEnd(ctx);
            }
            catch
            {

            }


            FormsAuthentication.SignOut();
            //if (Session != null)
            //    Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]

        // on close Tab and browser window
        public JsonResult LogOffHandler()
        {
            try
            {
                if (Session.Timeout != 17)
                {
                    Session.Timeout = 1; // expire after one minutes
                }
            }
            catch
            {
            }
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Refresh session after certain minutes(setting)
        /// </summary>
        /// <returns></returns>
        /// 

        public JsonResult Refresh()
        {
            if (Session.Timeout > 15)
                Session.Timeout = 15;
            return Json("OK", JsonRequestBehavior.AllowGet);
        }


        ////
        //// POST: /Account/JsonRegister
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult JsonRegister(RegisterModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Attempt to register the user
        //        try
        //        {
        //            WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
        //            WebSecurity.Login(model.UserName, model.Password);

        //            InitiateDatabaseForNewUser(model.UserName);

        //            FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
        //            return Json(new { success = true, redirect = returnUrl });
        //        }
        //        catch (MembershipCreateUserException e)
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        //        }
        //    }

        //    // If we got this far, something failed
        //    return Json(new { errors = GetErrorsFromModelState() });
        //}

        ///// <summary>
        ///// Initiate a new todo list for new user
        ///// </summary>
        ///// <param name="userName"></param>
        //private static void InitiateDatabaseForNewUser(string userName)
        //{
        //    TodoItemContext db = new TodoItemContext();
        //    TodoList todoList = new TodoList();
        //    todoList.UserId = userName;
        //    todoList.Title = "My Todo List #1";
        //    todoList.Todos = new List<TodoItem>();
        //    db.TodoLists.Add(todoList);
        //    db.SaveChanges();

        //    todoList.Todos.Add(new TodoItem() { Title = "Todo item #1", TodoListId = todoList.TodoListId, IsDone = false });
        //    todoList.Todos.Add(new TodoItem() { Title = "Todo item #2", TodoListId = todoList.TodoListId, IsDone = false });
        //    db.SaveChanges();
        //}

        ////
        //// POST: /Account/Disassociate

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        ////
        //// POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //            bool changePasswordSucceeded;
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a local password so remove any validation errors caused by a missing
        //        // OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            catch (Exception e)
        //            {
        //                ModelState.AddModelError("", e);
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// POST: /Account/ExternalLogin

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current user is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        //{
        //    string provider = null;
        //    string providerUserId = null;

        //    if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Insert a new user into the database
        //        using (UsersContext db = new UsersContext())
        //        {
        //            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
        //            // Check if user already exists
        //            if (user == null)
        //            {
        //                // Insert name into the profile table
        //                db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
        //                db.SaveChanges();

        //                InitiateDatabaseForNewUser(model.UserName);

        //                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
        //                OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

        //                return RedirectToLocal(returnUrl);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
        //            }
        //        }
        //    }

        //    ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// GET: /Account/ExternalLoginFailure

        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult ExternalLoginsList(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveExternalLogins()
        //{
        //    ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
        //    List<ExternalLogin> externalLogins = new List<ExternalLogin>();
        //    foreach (OAuthAccount account in accounts)
        //    {
        //        AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

        //        externalLogins.Add(new ExternalLogin
        //        {
        //            Provider = account.Provider,
        //            ProviderDisplayName = clientData.DisplayName,
        //            ProviderUserId = account.ProviderUserId,
        //        });
        //    }

        //    ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        //}

        //#region Helpers
        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //public enum ManageMessageId
        //{
        //    ChangePasswordSuccess,
        //    SetPasswordSuccess,
        //    RemoveLoginSuccess,
        //}

        //internal class ExternalLoginResult : ActionResult
        //{
        //    public ExternalLoginResult(string provider, string returnUrl)
        //    {
        //        Provider = provider;
        //        ReturnUrl = returnUrl;
        //    }

        //    public string Provider { get; private set; }
        //    public string ReturnUrl { get; private set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        //    }
        //}

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)).Distinct();
        }

        //public void  OnException(ExceptionContext filterContext)
        //{
        //    var exception = filterContext.Exception as HttpAntiForgeryException;
        //    if (exception != null)
        //    {
        //        var routeValues = new System.Web.Routing.RouteValueDictionary();
        //        routeValues["controller"] = "Account";
        //        routeValues["action"] = "LogOff";
        //        filterContext.Result = new RedirectToRouteResult(routeValues);
        //        filterContext.ExceptionHandled = true;
        //    }
        //}

        //private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        //{
        //    // See http://go.microsoft.com/fwlink/?LinkID=177550 for
        //    // a full list of status codes.
        //    switch (createStatus)
        //    {
        //        case MembershipCreateStatus.DuplicateUserName:
        //            return "User name already exists. Please enter a different user name.";

        //        case MembershipCreateStatus.DuplicateEmail:
        //            return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        //        case MembershipCreateStatus.InvalidPassword:
        //            return "The password provided is invalid. Please enter a valid password value.";

        //        case MembershipCreateStatus.InvalidEmail:
        //            return "The e-mail address provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidAnswer:
        //            return "The password retrieval answer provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidQuestion:
        //            return "The password retrieval question provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidUserName:
        //            return "The user name provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.ProviderError:
        //            return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        case MembershipCreateStatus.UserRejected:
        //            return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        default:
        //            return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
        //    }
        //}
        //#endregion
    }

    public class AccountManager
    {
        LoginModel _model = null;
        Thread _thread = null;

        public AccountManager(LoginModel model)
        {
            _model = model;
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
                LoginHelper.SaveLoginSetting(_model);
            }
            catch
            {
            }
            _model = null;
            _thread = null;
        }
    }
}