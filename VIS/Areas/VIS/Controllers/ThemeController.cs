using System;
using System.Web.Mvc;
using VAdvantage.Utility;

namespace VIS.Controllers
{

    public class ThemeController : Controller
    {
        public ActionResult ThemeCnfgtr(int windowNo)
        {

            ViewBag.WindowNumber = windowNo;
            //UserPreferenceModel obj = new UsrPreferenceModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                //obj = obj.GetUserSettings(ctx, Convert.ToInt32(adUserId));
                ViewBag.lang = ctx.GetAD_Language();
            }

            return PartialView(null);

        }
    }
}
