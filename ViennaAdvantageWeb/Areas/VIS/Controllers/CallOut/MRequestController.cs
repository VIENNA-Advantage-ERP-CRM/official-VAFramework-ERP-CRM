using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Controllers
{
    public class MRequestController: Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDefaultR_Status_ID(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int R_RequestType_ID;

                //Assign parameter value
                R_RequestType_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MRequestType rt = MRequestType.Get(ctx, R_RequestType_ID);
                int R_Status_ID = rt.GetDefaultR_Status_ID();
              

                //List<Decimal> retlst = new List<Decimal>();
                //retlst.Add(currentBalance);               

                retJSON = JsonConvert.SerializeObject(R_Status_ID);
            }           
            return Json(retJSON , JsonRequestBehavior.AllowGet);
        }
     

    }
}