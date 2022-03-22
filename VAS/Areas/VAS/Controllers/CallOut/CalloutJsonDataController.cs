using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;

using VIS.Helpers;
using VIS.DataContracts;
using VIS.Classes;
using VIS.Models;
using System.Web.Mvc;
using VIS.Filters;
using System.Web.SessionState;
using ViennaAdvantage.Model;

namespace VIS.Controllers
{
    /// <summary>
    /// common class to handle json data request 
    /// </summary>
    /// 
    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    [SessionState(SessionStateBehavior.ReadOnly)]
    // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]

    public class CalloutJsonDataController : Controller
    {
        public JsonResult JDataSet(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            sqlIn.sql = Server.HtmlDecode(sqlIn.sql);
            VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
            sqlIn.sql = SecureEngineBridge.DecryptByClientKey(sqlIn.sql, ctx.GetSecureKey());
            object data = h.ExecuteJDataSet(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }
	}
}