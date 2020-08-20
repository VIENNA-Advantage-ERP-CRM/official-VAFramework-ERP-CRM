using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIS.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AjaxSessionFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            // If the browser session or authentication session has expired...
            if (ctx.Session["ctx"] == null || !filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, we're overriding the returned JSON result with a simple string,
                    // indicating to the calling JavaScript code that a redirect should be performed.
                    //filterContext.Result = new JsonResult { Data = "_Logon_" };
                    filterContext.HttpContext.Response.StatusCode = 999;
                    filterContext.Result = new JsonResult { Data = "_Logon_" };
                    filterContext.HttpContext.Response.End();
                }
                //else
                //{
                //    // For round-trip posts, we're forcing a redirect to Home/TimeoutRedirect/, which
                //    // simply displays a temporary 5 second notification that they have timed out, and
                //    // will, in turn, redirect to the logon page.
                //    filterContext.Result = new RedirectToRouteResult(
                //        new RouteValueDictionary {
                //        { "Controller", "Home" },
                //        { "Action", "TimeoutRedirect" }
                //});
                //}
            }
            base.OnActionExecuting(filterContext);
        }
    }
}