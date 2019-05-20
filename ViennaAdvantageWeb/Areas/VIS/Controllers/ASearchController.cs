using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    [AjaxAuthorize]
    [AjaxSessionFilter]

    public class ASearchController : Controller
    {
        //
        // GET: /VIS/ASearch/
        public ActionResult Index(int windowNo)
        {
            ViewBag.WindowNo = windowNo;

            return PartialView(Session["ctx"] as VAdvantage.Utility.Ctx);
        }

        [HttpPost]
        public ActionResult DeleteQuery(int id)
        {
            int no = -1;

            var uq = new VAdvantage.Model.MUserQuery(Session["ctx"] as VAdvantage.Utility.Ctx, id, null);
            if (uq != null)
            {
                // get name of the query
                string name = uq.GetName();
                // delete query
                if (uq.Delete(true))
                {
                    no = 1;
                }
            }
            return Json(no);
        }

        [HttpPost]
        public ActionResult InsertOrUpdateQuery(int id, string name, string where, int tabid, int tid, List<QueryModel> qLines)
        {
            int no = -1;


            if (id == 0)
            {
                string sql = "SELECT Count(*) FROM AD_UserQuery WHERE AD_Table_ID=" + tid + " AND AD_Tab_ID=" + tabid + " AND Upper(Name)='" + name.ToUpper() + "'";
                int count = Convert.ToInt32(DB.ExecuteScalar(MRole.GetDefault(Session["ctx"] as VAdvantage.Utility.Ctx).AddAccessSQL(sql, "AD_UserQuery", true, false)));
                if (count > 0)
                {
                    return Json(-5);
                }

            }


            var uq = new VAdvantage.Model.MUserQuery(Session["ctx"] as VAdvantage.Utility.Ctx, id, null);
            //set query name
            if (name != null && name.Length > 0)
                uq.SetName(name);
            // set query code
            uq.SetCode(where);
            // set tab id
            uq.SetAD_Tab_ID(tabid);
            // set table id
            uq.SetAD_Table_ID(tid);
            // save the values in database
            if (uq.Save())
            {
                no = uq.Get_ID();
                // delete existing query lines
                uq.DeleteLines();
                // if no lines then return
                if (qLines == null || qLines.Count < 1)
                {
                    ;
                }
                else
                {
                    int totCnt = qLines.Count;
                    // save each the query lines in the dataset
                    for (int i = 0; i < totCnt; i++)
                    {
                        QueryModel m = qLines[i];
                        // set values
                        VAdvantage.Model.MUserQueryLine line = new VAdvantage.Model.MUserQueryLine(uq, (i + 1) * 10, m.KEYNAME ?? "",
                                m.KEYVALUE ?? "", m.OPERATORNAME ?? "",
                                m.VALUE1NAME ?? "", m.VALUE1VALUE ?? "",
                                m.VALUE2NAME ?? "", m.VALUE2VALUE ?? "",
                                m.FULLDAY == "Y" ? true : false);
                        // save query line
                        line.Save();
                    }
                }
            }
            return Json(no);
        }

        // Added by Bharat on 05 june 2017
        public JsonResult GetData(string ColumnName, int Tab_ID, int Table_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ASearchModel mod = new ASearchModel();
            return Json(JsonConvert.SerializeObject(mod.GetData(ColumnName, Tab_ID, Table_ID, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 05 june 2017
        public JsonResult GetQueryLines(int UserQuery_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ASearchModel mod = new ASearchModel();
            return Json(JsonConvert.SerializeObject(mod.GetQueryLines(UserQuery_ID, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 05 june 2017
        public JsonResult GetQueryDefault(int UserQuery_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ASearchModel mod = new ASearchModel();
            return Json(JsonConvert.SerializeObject(mod.GetQueryDefault(UserQuery_ID, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 05 june 2017
        public JsonResult GetNoOfRecrds(string RecQuery)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            RecQuery = SecureEngineBridge.DecryptByClientKey(RecQuery, ctx.GetSecureKey());
            ASearchModel mod = new ASearchModel();
            return Json(JsonConvert.SerializeObject(mod.GetNoOfRecrds(RecQuery, ctx)), JsonRequestBehavior.AllowGet);
        }

        public class QueryModel
        {
            public string KEYNAME { get; set; }
            public string KEYVALUE { get; set; }
            public string OPERATORNAME { get; set; }
            public string VALUE1NAME { get; set; }
            public string VALUE1VALUE { get; set; }
            public string VALUE2NAME { get; set; }
            public string VALUE2VALUE { get; set; }
            public string AD_USERQUERYLINE_ID { get; set; }
            public string OPERATOR { get; set; }
            public string FULLDAY { get; set; }
        }
    }
}