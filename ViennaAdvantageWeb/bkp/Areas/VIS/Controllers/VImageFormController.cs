using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class VImageFormController : Controller
    {
        //
        // GET: /VIS/VImageForm/
        public ActionResult Index(string windowno, int ad_image_id)
        {
            VImageModel obj = new VImageModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (ad_image_id > 0)
                {
                    obj.GetImage(ctx, Convert.ToInt32(ad_image_id), 0, 0, ctx.GetApplicationUrl());
                }
            }

            ViewBag.WindowNumber = windowno;
            return PartialView(obj);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveImage(HttpPostedFileBase file, bool isDatabaseSave, string ad_image_id)
        {
            // check if its not altered
            if (file == null && ad_image_id == "0")
            {
                Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

            VImageModel obj = new VImageModel();
            var value = 0;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"), "RecordImages")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images"), "RecordImages"));
                }
                value = obj.SaveImage(ctx, Server.MapPath("~/Images/RecordImages"), file, Convert.ToInt32(ad_image_id), isDatabaseSave);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To delete image, this controller function will delete image from folder as well as forcely from database too
        /// </summary>
        /// <param name="ad_image_id">Image id from database</param>
        /// <param name="imageUrl">Image path of folder</param>
        /// <returns></returns>
        public JsonResult DeleteImage(int ad_image_id, string imageUrl)
        {
            VImageModel obj = new VImageModel();
            var value = 0;
            if (Session["Ctx"] != null)
            {
                value = obj.DeleteImage(Session["ctx"] as Ctx, ad_image_id);
            }
            return Json(new { result = "null" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetImageAsByte(int ad_image_id)
        {
            VImageModel obj = new VImageModel();
            ImagePathInfo img = null;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                img = obj.GetImage(ctx, Convert.ToInt32(ad_image_id), 500, 375, ctx.GetApplicationUrl());
            }
            return Json(JsonConvert.SerializeObject(img), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileByteArray(HttpPostedFileBase file)
        {
            VImageModel obj = new VImageModel();
            var value = obj.GetArrayFromFile(Server.MapPath("~/Images/RecordImages"), file);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
    }

}