using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class GenralAttributeController : Controller
    {
        //
        // GET: /VIS/GenralAttribute/
        public ActionResult Index()
        {
            return PartialView();
        }

        public JsonResult Load(int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo)
        {
            GenralAttributeModel obj = new GenralAttributeModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.LoadInit(mAttributeSetInstanceId, vadms_AttributeSet_ID, windowNo, ctx, Server);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(int windowNoParent, int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo, List<KeyNamePair> lst)
        {
            GenralAttributeModel obj = new GenralAttributeModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.SaveGenAttribute(windowNoParent, mAttributeSetInstanceId, vadms_AttributeSet_ID, windowNo, lst, ctx);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchValues(int windowNoParent, int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo, List<KeyNamePair> lst)
        {
            GenralAttributeModel obj = new GenralAttributeModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.SearchValuesAttribute(windowNoParent, mAttributeSetInstanceId, vadms_AttributeSet_ID, windowNo, lst, ctx);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            } 
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

    }
}