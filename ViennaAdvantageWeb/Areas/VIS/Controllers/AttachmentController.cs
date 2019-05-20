using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using ViennaAdvantageWeb.Areas.VIS.Models;
using VIS.Filters;

namespace VIS.Controllers
{
    public class AttachmentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //public JsonResult GetFileAttachment(int AD_Table_ID, int Record_ID)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    AttachmentModel am = new AttachmentModel();
        //    return Json(new { result = am.GetFileAttachment(AD_Table_ID, Record_ID, ctx) }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetFileLocations()
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    AttachmentModel am = new AttachmentModel();
        //    return Json(new { result = am.GetFileLocations(ctx) }, JsonRequestBehavior.AllowGet);
        //}


        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetAttachment(int AD_Table_ID, int Record_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentModel am = new AttachmentModel();
            return Json(new { result = am.GetAttachment(AD_Table_ID, Record_ID, ctx) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveFileinTemp(HttpPostedFileBase file, string fileName, string folderKey)
        {

            try
            {
                //string folder = string.Empty;
                //if (folderKey == null)
                //{
                //    folder = DateTime.Now.Ticks.ToString();

                //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/App_Data"), folder));
                //}
                //else
                //{
                //    folder = folderKey.ToString();
                //}

                if (!Directory.Exists(Path.Combine(Server.MapPath("~/TempDownload"), folderKey)))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/TempDownload"), folderKey));
                }


                HttpPostedFileBase hpf = file as HttpPostedFileBase;
                string savedFileName = Path.Combine(Server.MapPath("~/TempDownload/" + folderKey), Path.GetFileName(fileName));
                MemoryStream ms = new MemoryStream();
                hpf.InputStream.CopyTo(ms);
                byte[] byteArray = ms.ToArray();

                if (Directory.GetFiles(Path.Combine(Server.MapPath("~/TempDownload"), folderKey)).Contains(Path.Combine(Server.MapPath("~/TempDownload"), folderKey, fileName)))//Append Content In File
                {
                    using (FileStream fs = new FileStream(savedFileName, FileMode.Append, System.IO.FileAccess.Write))
                    {

                        fs.Write(byteArray, 0, byteArray.Length);
                        ms.Close();
                    }

                }
                else // create new file
                {
                    using (FileStream fs = new FileStream(savedFileName, FileMode.Create, System.IO.FileAccess.Write))
                    {

                        fs.Write(byteArray, 0, byteArray.Length);
                        ms.Close();
                    }
                }

                // hpf.SaveAs(savedFileName);




                return Json(folderKey, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("ERROR:" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveAttachmentEntries(string files, int AD_Attachment_ID, string folderKey, int AD_Table_ID, int Record_ID, string fileLocation, int NewRecord_ID, bool IsDMSAttachment)
        {
            List<AttFileInfo> _files = JsonConvert.DeserializeObject<List<AttFileInfo>>(files);
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentModel am = new AttachmentModel();
            return Json(new { result = am.CreateAttachmentEntries(_files, AD_Attachment_ID, folderKey, ctx, AD_Table_ID, Record_ID, fileLocation, NewRecord_ID, IsDMSAttachment) }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult DownloadAttachment(string fileName, int AD_Attachment_ID, int AD_AttachmentLine_ID)
        {
            //List<AttFileInfo> _files = JsonConvert.DeserializeObject<List<AttFileInfo>>(files);
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentModel am = new AttachmentModel();
            return Json(new { result = am.DownloadAttachment(ctx, fileName, AD_Attachment_ID, AD_AttachmentLine_ID) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult DeleteAttachment(string AttachmentLines)
        {
            AttachmentModel am = new AttachmentModel();
            return Json(new { result = am.DeleteAttachment(AttachmentLines) }, JsonRequestBehavior.AllowGet);
        }
    }
}