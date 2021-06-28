using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;
using VIS.Models;
using System.IO;
using VIS.Classes;
//namespace ViennaAdvantageWeb.Areas.VIS.Controllers
namespace VIS.Controllers
{
    public class BPartnerController : Controller
    {
        //
        // GET: /VIS/BPartner/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetBPartner(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                int C_BPartner_ID;

                //Assign parameter value
                C_BPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MBPartner bpartner = new MBPartner(ctx, C_BPartner_ID, null);



                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment




                retDic["M_ReturnPolicy_ID"] = bpartner.GetM_ReturnPolicy_ID().ToString();

                retDic["M_ReturnPolicy_ID"] = bpartner.GetPO_ReturnPolicy_ID().ToString();

                //retDic["DateOrdered", order.GetDateOrdered());


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitBP(int WinNo, int bPartnerID, string bpType)
        {
            ViewBag.WindowNumber = WinNo;
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objBPModel = new BPartnerModel(WinNo, bPartnerID, bpType, ctx);

            return Json(JsonConvert.SerializeObject(objBPModel), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult AddBPartnerInfo(int C_BPartner_ID, string searchKey, string name, string name2, string greeting, string bpGroup, string bpRelation, string bpLocation, 
            string contact, string greeting1, string title, string email, string address, string phoneNo, string phoneNo2, string fax, int windowNo, string BPtype, 
            bool isCustomer, bool isVendor, bool isProspect, string fileName, string mobile, string webUrl, bool isEmployee)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objContactModel = new BPartnerModel();
            string resultMsg = string.Empty;
            string fileUrl = string.Empty;
            if (fileName != string.Empty)
            {
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"), "Temp")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images"), "Temp"));       //Create Thumbnail Folder if doesnot exists
                }

             fileUrl = Path.Combine(Server.MapPath("~/Images/Temp"), fileName);
            }
            if (C_BPartner_ID > 0)
            {
                resultMsg = objContactModel.AddBPartner(searchKey, name, name2, greeting, bpGroup, bpRelation, bpLocation, contact, greeting1, title, email, address, phoneNo, phoneNo2, fax, ctx, windowNo, BPtype, C_BPartner_ID, isCustomer, isVendor, isProspect, fileUrl, mobile, webUrl, isEmployee); // Update Business Partner
            }
            else
            {
                resultMsg = objContactModel.AddBPartner(searchKey, name, name2, greeting, bpGroup, bpRelation, bpLocation, contact, greeting1, title, email, address, phoneNo, phoneNo2, fax, ctx, windowNo, BPtype, C_BPartner_ID, isCustomer, isVendor, isProspect, fileUrl, mobile, webUrl, isEmployee);// Add New Business Partner
            }
            if (resultMsg != string.Empty)
            {
                return Json(JsonConvert.SerializeObject(resultMsg), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject(VAdvantage.Utility.Msg.GetMsg((string)ViewBag.culture, "RecordSaved")), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Business Partner Location 
        /// </summary>
        /// <param name="WinNo"></param>
        /// <param name="bpartnerID"></param>
        /// <returns></returns>
        public JsonResult GetBPLocation(int WinNo, int bpartnerID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objContactModel = new BPartnerModel();
            objContactModel.FillBPLocation(bpartnerID, ctx);
            return Json(JsonConvert.SerializeObject(objContactModel), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveImageAsByte(HttpPostedFileBase file, string fileKey)
        {
            List<string> imgList = new List<string>();
            try
            {
                
                string image = string.Empty;
                byte[] imageByte = null;
                HttpPostedFileBase hpf = file as HttpPostedFileBase;
                string fileNameWithoutExtention = Path.GetFileNameWithoutExtension(hpf.FileName);
                string fileExtension = Path.GetExtension(hpf.FileName);
                string fileName = fileNameWithoutExtention + "_" + fileKey +  fileExtension;
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"), "Temp")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images"), "Temp"));       //Create Thumbnail Folder if doesnot exists
                }
                string savedFileName = Path.Combine(Server.MapPath("~/Images/Temp"), Path.GetFileName(fileName));
                hpf.SaveAs(savedFileName);
                MemoryStream ms = new MemoryStream();
                hpf.InputStream.CopyTo(ms);
                byte[] byteArray = ms.ToArray();
                FileInfo file1 = new FileInfo(savedFileName);
                //if (file1.Exists)
                //{
                //    file1.Delete(); //Delete Temporary file             
                //}
                Ctx ctx = Session["ctx"] as Ctx;
              
                string imgByte = Convert.ToBase64String(byteArray);
                CommonFunctions.ConvertByteArrayToThumbnail(byteArray, fileName);
                image = Convert.ToBase64String(CommonFunctions.GetThumbnailByte(320, 240, fileName));
                //image = Convert.ToBase64String(CommonFunctions.GetThumbnailByte(320, 185, fileName));
                imgList.Add(image);
                imgList.Add(fileName);
             
            }
            catch
            {
                imgList.Add(null);
                imgList.Add(null);
            }
            return Json(JsonConvert.SerializeObject(imgList), JsonRequestBehavior.AllowGet);
        }
    }
}
