using System;
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
                MInOut io = new MInOut(ctx, Orig_InOut_ID, null);


                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment



               
                retDic["C_Project_ID"]= io.GetC_Project_ID().ToString();
                retDic["C_Campaign_ID"]= io.GetC_Campaign_ID().ToString();
                retDic["C_Activity_ID"]= io.GetC_Activity_ID().ToString();
                retDic["AD_OrgTrx_ID"]= io.GetAD_OrgTrx_ID().ToString();
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
                MInOutLine Orig_InOutLine = new MInOutLine(ctx, id, null);
                retDic["MovementQty"] = Orig_InOutLine.GetMovementQty().ToString();
                retDic["C_Project_ID"]= Orig_InOutLine.GetC_Project_ID().ToString();
                retDic["C_Campaign_ID"]= Orig_InOutLine.GetC_Campaign_ID().ToString();
                retDic["M_Product_ID"]= Orig_InOutLine.GetM_Product_ID().ToString();
                retDic["M_AttributeSetInstance_ID"]= Orig_InOutLine.GetM_AttributeSetInstance_ID().ToString();
                retDic["C_UOM_ID"]= Orig_InOutLine.GetC_UOM_ID().ToString();
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
