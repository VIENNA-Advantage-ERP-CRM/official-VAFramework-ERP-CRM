using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
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
            int outvalue = 0;
            ViewBag.scriptt = null;

            //extraInfo=HttpUtility.UrlDecode(extraInfo);

            if (!int.TryParse(result[0], out outvalue))
            {
                string script = result[1] + "JsAPI?clientOrg=" + MClient.Get(ctx).GetValue() + "&" + extraInfo.Replace("@recordid", recID.ToString()) + "&token=" + result[0];
                ViewBag.scriptt = script;
            }
            else
            {
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

        public ActionResult GetHTMLReport(string info)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            //If info contains http, that means a url is send from client. 
            //Changed by Karan VIS007 on 19-May-2021
            if (info.ToLower().Contains("http"))
            {
                ViewBag.scriptt = info;
            }
            else
            {
                ViewBag.Message = info;
            }
            return View();
        }
    }
}