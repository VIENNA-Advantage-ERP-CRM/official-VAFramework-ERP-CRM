using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
namespace VIS.Controllers
{
    public class InfoGeneralController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]       
        public JsonResult GetSearchColumns(string tableName, string VAF_Language, bool isBaseLangage)
        {
            //Change by mohit-to handle translation in general info.
            //Added 2 new parametere- string VAF_Language, bool isBaseLangage.
            //Asked by mukesh sir- 09/03/2018

            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            return Json(new { result = model.GetSchema(tableName, VAF_Language, isBaseLangage) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDispalyColumns(int VAF_TableView_ID, string VAF_Language, bool IsBaseLangage, string TableName)
        {
            //Change by mohit-to handle translation in general info.
            //Added 2 new parametere- string VAF_Language, bool IsBaseLangage.
            //Asked by mukesh sir - 09/03/2018

            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            return Json(new { result = model.GetDisplayCol(VAF_TableView_ID, VAF_Language,IsBaseLangage,TableName) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string sql, string tableName, int pageNo)
        {
            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();
            //model.GetSchema(VAF_QuickSearchWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(sql, tableName,pageNo, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

    }
}