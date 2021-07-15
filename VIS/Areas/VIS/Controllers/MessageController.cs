using ModelLibrary.PushNotif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VAdvantage.Utility;

namespace VIS.Controllers
{
    public class MessageController : Controller
    {
        // GET: VIS/Channel
        public ActionResult Get()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string serializedObject = null;
            if (ctx != null)
            {
                string sessionID = ctx.GetAD_Session_ID().ToString();
                JavaScriptSerializer ser = new JavaScriptSerializer();

                //IEnumerable<KeyValuePair<string, string>> newDic = toastrMessage.Where(kvp => kvp.Key.Contains(sessionID));
                var newDic = ModelLibrary.PushNotif.SSEManager.Get().GetMessages(ctx.GetAD_Session_ID());
                if (newDic != null && newDic.Count() > 0)
                {
                    /// for (int i = 0; i < newDic.Count();)
                    // {
                    //    KeyValuePair<string, string> keyVal = newDic.ElementAt(i);
                    //    toastrMessage.Remove(keyVal.Key);
                    serializedObject = ser.Serialize(newDic);
                    return Content(string.Format("data: {0}\n\n", serializedObject), "text/event-stream");
                    //}
                }
            }
            JavaScriptSerializer se1r = new JavaScriptSerializer();
            serializedObject = se1r.Serialize(new { item = 1, message = "" });
            return Content(string.Format("data: {0}\n\n", serializedObject), "text/event-stream");
        }


        public ActionResult Demo(bool IsStop)
        {
            Ctx ctx = Session["ctx"] as Ctx;

            if (ctx != null)
            {
                if (IsStop)
                {
                    SSEManager.Get().AddMessage(ctx.GetAD_Session_ID(), " Stopped  ", "DEMOE", SSEManager.Cast.BroadCast);
                }
                else
                {
                    SSEManager.Get().AddMessage(ctx.GetAD_Session_ID(), " Started  ", "DEMOS", SSEManager.Cast.BroadCast);
                }

            }
            return Json(new { result = "OK" }, JsonRequestBehavior.AllowGet);
        }

    }
}