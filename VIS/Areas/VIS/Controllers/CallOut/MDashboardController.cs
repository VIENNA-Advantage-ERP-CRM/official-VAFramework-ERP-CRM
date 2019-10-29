using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MDashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// To update groupby on the basis of view column
        /// </summary>
        /// <param name="fields">RC_View_ID, RC_ViewColumn_ID</param>
        /// <returns></returns>
        public JsonResult GroupByChecked(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MDashboardModel objDashboard = new MDashboardModel();
                string[] paramValue = fields.Split(',');
                retJSON = JsonConvert.SerializeObject(objDashboard.GroupByChecked(ctx, Util.GetValueOfInt(paramValue[0]), Util.GetValueOfInt(paramValue[1])));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}