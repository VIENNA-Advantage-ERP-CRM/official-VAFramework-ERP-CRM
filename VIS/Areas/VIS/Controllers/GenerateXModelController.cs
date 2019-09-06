using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using ViennaAdvantage.Tool.Model;

namespace VIS.Controllers
{
    public class GenerateXModelController : Controller
    {
        public JsonResult GenXModelGetTable()
        {
            var ctx = Session["ctx"] as Ctx; 
            GenerateXModelModel obj = new GenerateXModelModel();
            var value = obj.GenXModelGetTable(ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
	}
}