using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Helpers;

namespace VIS.Areas.VIS.Controllers
{
    public class LookupController : Controller
    {
        // GET: VIS/Lookup
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetLookupData(int WindowNo, int AD_Window_ID, int AD_Tab_ID, int AD_Field_ID, string Values, int PageSize)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            GridWindowVO vo = AEnv.GetMWindowVO(ctx, WindowNo, AD_Window_ID, 0);

            string lookupQuery = vo.GetTabs().Where(a => a.AD_Tab_ID == AD_Tab_ID).FirstOrDefault().GetFields().Where(x => x.AD_Field_ID == AD_Field_ID).FirstOrDefault().lookupInfo.query;

            if (!string.IsNullOrEmpty(Values))
            {
                List<KeyValuePair<string, object>> data = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(Values);

                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        lookupQuery.Replace(data[i].Key, Convert.ToString(data[i].Value));
                    }
                }
            }

            //DataSet ds = DB.ExecuteDataset(lookupQuery);
            SqlHelper h = new SqlHelper();
            SqlParamsIn sqlIn = new SqlParamsIn()
            {
                sql = lookupQuery,
            };

            if (PageSize > 0)
                sqlIn.pageSize = PageSize;

            object result = h.ExecuteJDataSet(sqlIn);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
    }

    //public class LookUpData
    //{
    //    public string Col1 { get; set; }
    //    public string Col2 { get; set; }
    //    public string Col3 { get; set; }
    //    public string Col4 { get; set; }
    //    public string Col5 { get; set; }
    //    public string Col6 { get; set; }
    //}
}