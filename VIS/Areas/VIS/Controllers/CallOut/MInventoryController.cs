using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MInventoryController:Controller
    {
        //
        // GET: /VIS/CalloutOrder/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMInventoryLine(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int M_InventoryLine_ID;

                //Assign parameter value
                M_InventoryLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MInventoryLine iLine = new MInventoryLine(ctx, M_InventoryLine_ID, null);
                int M_Product_ID = iLine.GetM_Product_ID();
                int M_Locator_ID = iLine.GetM_Locator_ID();


                List<int> retlst = new List<int>();

                retlst.Add(M_Product_ID);
                retlst.Add(M_Locator_ID);

                retJSON = JsonConvert.SerializeObject(retlst);
            }           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
           // return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMInventory(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInventoryModel objInventoryLine = new MInventoryModel();
                retJSON = JsonConvert.SerializeObject(objInventoryLine.GetMInventory(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);            
        }     
    }
}