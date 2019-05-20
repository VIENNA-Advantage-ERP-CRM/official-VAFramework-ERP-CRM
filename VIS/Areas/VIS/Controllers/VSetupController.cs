using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class VSetupController : Controller
    {
        //
        // GET: /VIS/VSetup/
        public ActionResult Index(string windowno)
        {
                    
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetInitialData()
        {
            VSetupModel model = new VSetupModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.GetInitialData(ctx) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult InitailizeClientSetup(string clientName,string orgName, string userClient,string userOrg,string city,
            int currencyID, string currencyName, int countryID, string countryName, int regionID, string regionName,
            bool cfProduct, bool cfBPartner, bool cfProject,bool cfMCampaign,bool cfSRegion, string fileName,string folderKey)
        {
            VSetupModel model = new VSetupModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.InitailizeClientSetup(clientName,orgName, userClient,userOrg,city,
            currencyID, currencyName, countryID, countryName, regionID, regionName,
            cfProduct, cfBPartner, cfProject,cfMCampaign,cfSRegion, fileName,folderKey,ctx) }, JsonRequestBehavior.AllowGet);
        }
	}
}