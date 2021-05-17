using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Areas.VIS.Controllers.CallOut
{
    public class MTeamForcastController : Controller
    {
        // GET: VIS/MTeamForcast
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Supervisor
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public JsonResult GetSuperVisor(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                MTeamForcastModel objAcctSchema = new MTeamForcastModel();
                retJSON = JsonConvert.SerializeObject(objAcctSchema.GetSupervisor(ctx, Util.GetValueOfInt(fields)));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get BOM details of selected product
        /// </summary>
        /// <param name="fields">Product</param>
        /// <returns>BOM details</returns>
        public JsonResult GetBOMdetails(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                MTeamForcastModel objAcctSchema = new MTeamForcastModel();
                retJSON = JsonConvert.SerializeObject(objAcctSchema.GetBOMDetails(ctx, Util.GetValueOfInt(fields)));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}