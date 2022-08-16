using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Model;
using VIS.Models;

namespace VIS.Controllers
{
    public class SurveyPanelController : Controller
    {
        /// <summary>
        /// Getting Survey Details
        /// </summary>
        /// <returns>Survey Details Records</returns>
        public JsonResult GetSurveyAssignments(string AD_window_ID, string AD_Tab_ID)
        {
            SurveyPanelModel obj = new SurveyPanelModel();
            return Json(JsonConvert.SerializeObject(obj.GetSurveyAssignments(Util.GetValueOfInt(AD_window_ID), Util.GetValueOfInt(AD_Tab_ID))), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get the survey itesms based on survey ID
        /// </summary>
        /// <param name="AD_Survey_ID">Survey ID</param>
        /// <returns>List of survey items and it's values.</returns>
        public JsonResult GetSurveyItems(string AD_Survey_ID)
        {
            SurveyPanelModel obj = new SurveyPanelModel();
            return Json(JsonConvert.SerializeObject(obj.GetSurveyItems(Util.GetValueOfInt(AD_Survey_ID))), JsonRequestBehavior.AllowGet);
        }

    }
}