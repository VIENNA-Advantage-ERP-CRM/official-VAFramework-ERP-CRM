using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarketSvc;
using MarketSvc.MService;
using Newtonsoft.Json;
using VAdvantage.Utility;
using Market.Helper;
using System.Web.SessionState;
using VIS.Filters;

namespace Market.Controllers
{
   [SessionState(SessionStateBehavior.ReadOnly)]
  
    public class ModuleController : Controller
    {

        static MarketSvc.ImportMoule ipm = null;


        //
        // GET: /Market/Market/
        [AjaxSessionFilterAttribute]
        public ActionResult Index()
        {
            ViewBag.contextUrl = @Url.Content("~/");
            return PartialView("_ModulePartial",Session["ctx"] as Ctx);
        }

        [HttpPost]
        [AjaxSessionFilterAttribute]
        public JsonResult GetMarketModules(ModuleContainer mcIn)
        {
            MarketSvc.CustomException cerror = null;
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            var md= bsic.GetMarketModules(mcIn, out cerror);
            return Json(new { result = JsonConvert.SerializeObject(ModuleHelper.GetModuleInfo(md, mcIn.ModType, Session["ctx"] as Ctx)), error = JsonConvert.SerializeObject(cerror) });
        }
        [HttpPost]
        public JsonResult GetModuleInfo(int AD_ModuleInfo_ID, string tokenKey)
        {
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            return Json(new { result = JsonConvert.SerializeObject(ModuleHelper.GetModuleInfo(bsic.GetModuleInfo(AD_ModuleInfo_ID, tokenKey)))});


           // return Json(new { result = JsonConvert.SerializeObject(bsic.GetModuleInfo(AD_ModuleInfo_ID, tokenKey)) });
            
        }

        public JsonResult InitLoginOrValidate(InitLoginInfo info)
        {
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            return Json(JsonConvert.SerializeObject(bsic.InitLogin(info, HttpContext.Request.Url.Authority + HttpRuntime.AppDomainAppVirtualPath)));
        }

        public JsonResult DownloadAndExtractModuleFiles(int AD_ModuleInfo_ID)
        {
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            return Json(bsic.DownloadAndExtractModuleFiles(AD_ModuleInfo_ID));
        }

        public JsonResult ImportModuleFromLocalFile(string moduleName, int[] clientIds, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic)
        {
            if (ipm == null ||ipm.Done)
            {
                ipm = new ImportMoule();
                CallBackDetail message = ipm.ImportModuleFromLocalFile(moduleName, clientIds, langCode, onlyTranslation, ctxDic);
                ipm.Done = true;
                return Json(message);
            }
            else
            {
                 return Json(new CallBackDetail() { Status = "InProgress", Action = "Error", ActionMsg = "------- Module installation already in progress, Please try after some time. -------" });
            }
        }

        public JsonResult PullMessage()
        {
            var ret = "Ok";
            string  res = null;
            if (ipm != null && ipm.Done)
            {
                res =  JsonConvert.SerializeObject(ipm.GetCallBackMsg());
                ipm = null;
            }
            else if (ipm != null)
            {
                res = JsonConvert.SerializeObject(ipm.GetCallBackMsg());
            }
            else 
            {
                ret ="Finish";
                ipm = null;
            }

            return Json(new {status=ret,  result = res });
        }

        public ActionResult ModuleLog(string id, string path, string value)
        {
            ipm = null;


            try
            {
                string name;
                string logs = VWebHelper.VHelper.GetLog(path, id, out name);
                // check for error while importing area files.
                if (ImportMoule.sbError.ToString() != "")
                {
                    logs = ImportMoule.sbError.ToString()+ logs;
                }
                ViewBag.Log = logs;

                ViewBag.Name= name;
            }
            catch (Exception ex)
            {
                ViewBag.Log = ex.Message;
                ViewBag.Name = "Error";
            }

            return View();
        }

        public ActionResult UpdateSubscrption(int AD_Module_ID, int Product_ID, string token, bool isFree, string  version)
        {
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            bsic.UpdateSubscription(AD_Module_ID, Product_ID, token, isFree, version);
            bsic = null;
            return Json("");
        }
        public JsonResult GetModuleVersionHistory(int AD_Module_ID, int PageNo)
        {
            MarketSvc.MarketServiceBasic bsic = new MarketServiceBasic();
            return Json(new { result = JsonConvert.SerializeObject(bsic.GetModuleversionHostory(AD_Module_ID, PageNo)) });
        }
       
	}
}