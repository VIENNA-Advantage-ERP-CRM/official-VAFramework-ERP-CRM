using System;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Models;

namespace VIS.Controllers
{

    public class ThemeController : Controller
    {
        public ActionResult ThemeCnfgtr(int windowNo)
        {
            ViewBag.WindowNumber = windowNo;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                ViewBag.lang = ctx.GetAD_Language();
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult GetList()
        {
            return Json(ThemeModel.GetThemeData());
        }

        [HttpPost]
        public ActionResult Save(ThemeData thd)
        {
            ThemeModel tm = new ThemeModel();
            return Json(tm.SaveTheme(thd));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            ThemeModel tm = new ThemeModel();
            return Json(tm.Delete(id));
        }

        [HttpPost]
        public ActionResult SetDefault(int id)
        {
            ThemeModel tm = new ThemeModel();
            return Json(tm.SetDefaultTheme(id));
        }


        [HttpPost]
        public ActionResult SaveForUser(int id,int uid)
        {
            ThemeModel tm = new ThemeModel();
            return Json(tm.UpdateForUser(id, uid));
        }

        [HttpPost]
        public ActionResult UpdateForClient(int id, int cid)
        {
            ThemeModel tm = new ThemeModel();
            return Json(tm.UpdateForClient(id, cid));
        }
    }
}
