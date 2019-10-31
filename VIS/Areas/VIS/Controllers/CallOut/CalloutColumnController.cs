using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VIS.Filters;

namespace VIS.Controllers
{
    [AjaxAuthorizeAttribute] // redirect to login page if reques`t is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class CalloutColumnController : Controller
    {
        // GET: VIS/CalloutTable
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult GetDBColunName(int AD_Element_ID)
        {
            string sql = "SELECT ColumnName from AD_Element WHERE AD_Element_ID="+AD_Element_ID;
            return Content(DB.ExecuteScalar(sql).ToString());
        }
    }
}