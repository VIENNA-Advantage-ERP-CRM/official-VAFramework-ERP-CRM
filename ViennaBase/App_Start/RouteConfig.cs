using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace ViennaBase
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

           
            /* Default Route Do-Not Change */
           routes.MapRoute(
                               "default",
                               "{controller}/{action}/{id}",
                               new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                               , new[] { "VIS.Controllers" }
                           ).DataTokens["area"] = "VIS";
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;


            /* System Defalult Culture  Do-not change */
            System.Globalization.CultureInfo c1 = new System.Globalization.CultureInfo("en-US");
           System.Globalization.CultureInfo.DefaultThreadCurrentCulture = c1;
           System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = c1;

           // AntiForgeryConfig.SuppressXFrameOptionsHeader = true;


        }
    }
}
