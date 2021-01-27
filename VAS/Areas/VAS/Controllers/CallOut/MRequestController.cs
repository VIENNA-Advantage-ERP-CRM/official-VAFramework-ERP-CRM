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
    public class MRequestController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDefaultVAR_Req_Status_ID(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAR_Req_Type_ID;

                //Assign parameter value
                VAR_Req_Type_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MRequestType rt = MRequestType.Get(ctx, VAR_Req_Type_ID);
                int VAR_Req_Status_ID = rt.GetDefaultVAR_Req_Status_ID();


                //List<Decimal> retlst = new List<Decimal>();
                //retlst.Add(currentBalance);               

                retJSON = JsonConvert.SerializeObject(VAR_Req_Status_ID);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetMailData(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                int VAR_MailTemplate_ID;
                VAR_MailTemplate_ID = Util.GetValueOfInt(fields);
                MMailText mt = new MMailText(ctx, VAR_MailTemplate_ID, null);
                string mailText = mt.GetMailText();
                retJSON = JsonConvert.SerializeObject(mailText);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetResponse(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                int VAR_Req_StandardReply_ID;
                VAR_Req_StandardReply_ID = Util.GetValueOfInt(fields);
                MStandardResponse mt = new MStandardResponse(ctx, VAR_Req_StandardReply_ID, null);
                string mailText = mt.GetResponseText();
                retJSON = JsonConvert.SerializeObject(mailText);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}