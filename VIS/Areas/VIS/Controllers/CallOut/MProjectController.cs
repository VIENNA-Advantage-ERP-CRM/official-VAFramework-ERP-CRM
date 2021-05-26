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
    public class MProjectController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProjectDetail(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MProjectModel objProjectModel = new MProjectModel();
                retJSON = JsonConvert.SerializeObject(objProjectModel.GetProjectDetail(ctx,fields));              
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
     
        // Added by Bharat on 23 May 2017
        public JsonResult GetProjectPriceLimit(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MProjectModel objProjectModel = new MProjectModel();
                retJSON = JsonConvert.SerializeObject(objProjectModel.GetProjectPriceLimit(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This Function is used to get the Price List Precision
        /// </summary>
        /// <param name="fields">Price List Version ID</param>
        /// <returns>Precision</returns>
        public JsonResult GetPriceListPrecision(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPriceListModel objPriceListModel = new MPriceListModel();
                fields = Util.GetValueOfString(VAdvantage.DataBase.DB.ExecuteScalar("SELECT M_PriceList_ID FROM M_PriceList_Version WHERE M_PriceList_Version_ID = " + fields));
                retJSON = JsonConvert.SerializeObject(objPriceListModel.GetPriceList(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}