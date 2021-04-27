﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;
namespace ViennaAdvantageWeb.Areas.VIS.Controllers
{
    public class InOutController : Controller
    {
        //
        // GET: /VIS/InOut/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetInOut(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                int Orig_InOut_ID;

                //Assign parameter value
                Orig_InOut_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MVAMInvInOut io = new MVAMInvInOut(ctx, Orig_InOut_ID, null);


                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment



               
                retDic["VAB_Project_ID"]= io.GetVAB_Project_ID().ToString();
                retDic["VAB_Promotion_ID"]= io.GetVAB_Promotion_ID().ToString();
                retDic["VAB_BillingCode_ID"]= io.GetVAB_BillingCode_ID().ToString();
                retDic["VAF_OrgTrx_ID"]= io.GetVAF_OrgTrx_ID().ToString();
                retDic["User1_ID"]= io.GetUser1_ID().ToString();
                retDic["User2_ID"]= io.GetUser2_ID().ToString();
                //retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
         public JsonResult GetInOutLine(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                Dictionary<String, String> retDic = new Dictionary<string, string>();
                int id;

                //Assign parameter value
                id = Util.GetValueOfInt(paramValue[0].ToString());
                MVAMInvInOutLine Orig_InOutLine = new MVAMInvInOutLine(ctx, id, null);
                retDic["MovementQty"] = Orig_InOutLine.GetMovementQty().ToString();
                retDic["VAB_Project_ID"]= Orig_InOutLine.GetVAB_Project_ID().ToString();
                retDic["VAB_Promotion_ID"]= Orig_InOutLine.GetVAB_Promotion_ID().ToString();
                retDic["VAM_Product_ID"]= Orig_InOutLine.GetVAM_Product_ID().ToString();
                retDic["VAM_PFeature_SetInstance_ID"]= Orig_InOutLine.GetVAM_PFeature_SetInstance_ID().ToString();
                retDic["VAB_UOM_ID"]= Orig_InOutLine.GetVAB_UOM_ID().ToString();
                //retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
    }
}