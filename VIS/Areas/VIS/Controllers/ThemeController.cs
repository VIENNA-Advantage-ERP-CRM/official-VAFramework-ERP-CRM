using System;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{

    public class ThemeController : Controller
    {
        public ActionResult ThemeCnfgtr(int windowNo)
        {

            ViewBag.WindowNumber = windowNo;
          
            return PartialView(ThemeModel.GetThemeData());

        }
    }
}
