using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Model;

namespace VIS.Controllers
{
    public class BiPanelController : Controller
    {
        // GET: VIS/BiPanel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUserBILogin(string extraInfo, int recID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BiPanelModel model = new BiPanelModel(ctx);
            List<string> result = model.GetUserBILogin();
            ViewBag.extraInfo = extraInfo;
            ViewBag.recID = recID;
            //if (result != null && result.Count > 0)
            //{
            //    if (Convert.ToInt16(result[0]) <= 4)
            //    {
            //        return Json(JsonConvert.SerializeObject(result[0]), JsonRequestBehavior.AllowGet);
            //    }

            //string script = result[1] + "JsAPI?reportUUID=" + extraInfo + "&Filtere37a1fac-8d8b-48e0-800b-3f4cc142cc4c=" + recID + "&token=" + result[0];
            int outvalue = 0;
            ViewBag.scriptt = null;
            if (!int.TryParse(result[0], out outvalue))
            {
                string script = result[1] + "JsAPI?" + extraInfo.ToLower().Replace("@recordid", recID.ToString()).Replace("||","&") + "&token=" + result[0];
                ViewBag.scriptt = script;
            }
            else {
                if (outvalue == 1)
                {
                    ViewBag.Message = Msg.GetMsg(ctx, "VA037_BIToolMembership");
                }
                else if (outvalue == 2)
                {
                    ViewBag.Message = Msg.GetMsg(ctx, "VA037_BIUrlNotFound");
                }
                else if (outvalue == 3)
                {
                    ViewBag.Message = Msg.GetMsg(ctx, "VA037_NotBIUser");
                }
                else if (outvalue == 4)
                {
                    ViewBag.Message = Msg.GetMsg(ctx, "VA037_BICallingError");
                }
            }
            
            return View();
            //}
            //return Json(JsonConvert.SerializeObject("4"), JsonRequestBehavior.AllowGet);
        }
    }
}