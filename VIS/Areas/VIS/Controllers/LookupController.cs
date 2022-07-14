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

            VLookUpInfo lInfo = vo.GetTabs().Where(a => a.AD_Tab_ID == AD_Tab_ID).FirstOrDefault().GetFields().Where(x => x.AD_Field_ID == AD_Field_ID).FirstOrDefault().lookupInfo;
            string lookupQuery = lInfo.query;
            string validation = lInfo.validationCode;
            if (!string.IsNullOrEmpty(validation))
            {
                if (!string.IsNullOrEmpty(Values))
                {
                    List<LookUpData> data = JsonConvert.DeserializeObject<List<LookUpData>>(Values);

                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            validation = validation.Replace("@" + data[i].Key + "@", Convert.ToString(data[i].Value));
                        }
                    }
                }

                var posFrom = lookupQuery.LastIndexOf(" FROM ");
                var hasWhere = lookupQuery.IndexOf(" WHERE ", posFrom) != -1;
                //
                var posOrder = lookupQuery.LastIndexOf(" ORDER BY ");
                if (posOrder != -1)
                    lookupQuery = lookupQuery.Substring(0, posOrder) + (hasWhere ? " AND " : " WHERE ") + validation + lookupQuery.Substring(posOrder);
                else
                    lookupQuery += (hasWhere ? " AND " : " WHERE ") + validation;
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

    public class LookUpData
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
}