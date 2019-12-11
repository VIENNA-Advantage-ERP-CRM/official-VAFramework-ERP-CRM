using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class GlJournallController : Controller
    {
        /// <summary>
        /// Is Used to get ColumnName
        /// </summary>
        /// <param name="fields">Ad Column ID</param>
        /// <returns>Column Name</returns>
        public JsonResult ColumnName(String fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            String columnName = VAdvantage.Model.MColumn.GetColumnName(ctx, Util.GetValueOfInt(fields));
            return Json(JsonConvert.SerializeObject(columnName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Is used to update Dimension value 
        /// </summary>
        /// <param name="fields">contain reference -- Record_Id (GL_LineDimension_ID) , Dimension_Value</param>
        /// <returns> 1, 0 , -1</returns>
        public JsonResult SaveDimensionReference(String fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string[] paramValue = fields.Split(',');
            int recordId = Util.GetValueOfInt(paramValue[0]);
            int dimenssionId = Util.GetValueOfInt(paramValue[1]);
            int no = VAdvantage.DataBase.DB.ExecuteQuery("Update GL_LineDimension SET DimensionValue = " + dimenssionId +
                                                @" WHERE GL_LineDimension_ID = " + recordId);
            return Json(JsonConvert.SerializeObject(no), JsonRequestBehavior.AllowGet);
        }
    }
}