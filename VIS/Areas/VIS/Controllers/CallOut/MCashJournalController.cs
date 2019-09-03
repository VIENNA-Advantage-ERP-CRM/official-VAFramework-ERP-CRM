using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MCashJournalController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}
        // Added by Arpit on 13/Dec/2017
        public JsonResult GetBPLocation(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MCashJournalModel objCJModel = new MCashJournalModel();
            retJSON = JsonConvert.SerializeObject(objCJModel.GetLocationData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        ///  <summary>
        /// Get account no and routing no against selected bank account
        /// </summary>        
        /// <param name="fields"> ID of bank account  </param>
        /// <returns>Account number and Routing Number</returns> //Added by manjot on 22/02/2019 
        public JsonResult GetBankAccountData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MCashJournalModel objCJModel = new MCashJournalModel();
            retJSON = JsonConvert.SerializeObject(objCJModel.GetAccountData(ctx, Util.GetValueOfInt(fields)));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}