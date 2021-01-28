using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using VAdvantage.Acct;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VISModel.Filters;

namespace VIS.Controllers
{
    public class PostingController : Controller
    {
        //
        // GET: /VIS/Posting/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult PostImmediate(int VAF_Client_ID, int VAF_TableView_ID, int Record_ID, bool force)
        {

            Ctx ctx = Session["ctx"] as Ctx;
            string res = "";
            try
            {
                //string clientName = ctx.GetVAF_Org_Name() + "_" + ctx.GetVAF_UserContact_Name();
                //string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
                //storedPath += clientName;
                //VLogMgt.Initialize(true, storedPath);


                MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, VAF_Client_ID);
                res = Doc.PostImmediate(ass, VAF_TableView_ID, Record_ID, force, null);
                if (res == null || res.Trim().Length > 0)
                {
                    res = "OK";
                }
            }
            catch (Exception ex)
            {
                res += ex.Message;
            }
            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult PostByNewLogic(int VAF_Client_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string res = "No";

            MVAFClientDetail info = MVAFClientDetail.Get(ctx, VAF_Client_ID);
            MAcctSchema ass = new MAcctSchema(ctx, info.GetVAB_AccountBook1_ID(), null);
            if (ass.GetFRPT_LocAcct_ID() > 0)
            {
                res = "Yes";
            }

            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }
    }
}