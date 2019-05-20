using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /VIS/Chat/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load Existing chat with Current record
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        /// 
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult LoadChat(ChatProperties prop)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ChatModel model = new ChatModel(ct, prop.WindowNo, prop.ChatID, prop.AD_Table_ID, prop.Record_ID, prop.Description, null, prop.page, prop.pageSize);
            ChatInfo chatRecords = model.GetChatData();
            return Json(JsonConvert.SerializeObject(chatRecords), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save new comment with current record.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        /// 
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult SaveChat(ChatProperties prop)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ChatModel model = new ChatModel(ct, prop.ChatID, prop.AD_Table_ID, prop.Record_ID, prop.Description);
            model.Ok(prop.ChatText);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


    }
}
