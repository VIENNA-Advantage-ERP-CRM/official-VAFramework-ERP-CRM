using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIS.Controllers
{
    public class VController : Controller
    {
        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
            if (Session["ctx"] == null || !Request.IsAuthenticated)
            {
                String url = System.Web.Security.FormsAuthentication.LoginUrl + "?X-Requested-With=XMLHttpRequest";
                filterContext.Result = new RedirectResult(url);
            }
            
        }

        //protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        //{
        //    base.OnAuthentication(filterContext);
        //}

        //protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        //{
        //    base.OnAuthentication(filterContext);
        //    // Only do something if we are about to give a HttpUnauthorizedResult and we are in AJAX mode.
        //    if (filterContext.Result is HttpUnauthorizedResult && filterContext.HttpContext.Request.IsAjaxRequest())
        //    {
        //        // TODO: fix the URL building:
        //        // 1- Use some class to build URLs just in case LoginUrl actually has some query already.
        //        // 2- When leaving Result as a HttpUnauthorizedResult, ASP.Net actually does some nice automatic stuff, like adding a ReturnURL, when hardcodding the URL here, that is lost.
                
        //    }
        //}
	}
}