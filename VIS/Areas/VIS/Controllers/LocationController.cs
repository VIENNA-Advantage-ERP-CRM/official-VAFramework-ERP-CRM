using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class LocationController : Controller
    {
        [HttpGet]
        public ActionResult Locations(string windowno, string locationId,string AD_Language)
        {
            ViewBag.WindowNumber = windowno;
            Ctx ctx=new Ctx();
            if (Session["Ctx"] != null)
            {
                 ctx = Session["ctx"] as Ctx;
                ViewBag.lang = ctx.GetAD_Language();
            }
            LocationModel obj = new LocationModel();
            if (Util.GetValueOfInt(locationId) > 0)
            {
                obj = obj.GetAddressfromDataBase(locationId,ctx);
            }
                // Change By Mohit - To get country from login langauge on location form.
            else
            {
                if (!string.IsNullOrEmpty(AD_Language))
                {
                    DefaultCountry result = obj.GetCountryName(AD_Language, ctx);
                    obj.Country = result.CountryName;
                    obj.C_Country_ID = result.CountryID;
                }
            }
            return PartialView(obj);
        }

        /// <summary>
        /// Save locations into database
        /// </summary>
        /// <param name="locationId">locationId if in future want update option</param>
        /// <returns></returns>
        public JsonResult SaveLocation(Dictionary<string, object> pref)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                LocationModel obj = new LocationModel();

                var _location = obj.LocationSave(ctx, pref);
                return Json(new { locationid = _location.Get_ID(), locaddress = _location.ToString() });
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fill countries by name
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public JsonResult GetCountry(string name_startsWith)
        {
            Ctx ctx = new Ctx();
            if (Session["Ctx"] != null)
            {
                ctx = Session["ctx"] as Ctx;               
            }
            LocationModel obj = new LocationModel();
            var keyValues = obj.GetCountryByText(name_startsWith,ctx);
            if (keyValues.Count > 0)
            {
                return Json(new { result = keyValues }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all states by name
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public JsonResult GetStates(string name_startsWith, string countryId)
        {
            LocationModel obj = new LocationModel();
            var keyValues = obj.GetStatesByText(name_startsWith, countryId);
            if (keyValues.Count > 0)
            {
                return Json(new { result = keyValues }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get address for search text
        /// </summary>
        /// <param name="name_startsWith"></param>
        /// <returns></returns>
        public JsonResult GetAddresses(string name_startsWith)
        {
            LocationModel obj = new LocationModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var keyValues = obj.GetAddressesSearch(ctx, name_startsWith);
                if (keyValues.Count > 0)
                {
                    return Json(new { result = keyValues }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #region "Location Lookup"

        public JsonResult GetLocation(int id)
        {
            var name = "";
            LatLng ll = new LatLng(); 
            LocationRecord lr = new LocationRecord();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                MLocation loc = MLocation.Get(ctx, id, null);

                if (loc == null)
                {
                    return Json(lr, JsonRequestBehavior.AllowGet);
                }

                name = loc.ToString();

                ll.Longitude = loc.GetLongitude();
                ll.Latitude = loc.GetLatitude();
            }
            lr.LocItem = new KeyNamePair(id, name);
            lr.LocLatLng = ll;
            
            return Json(lr, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocations()
        {
            string ret = null;
            if (Session["Ctx"] != null)
            {
                return Json(JsonConvert.SerializeObject(LocationModel.GetAllLocations(Session["Ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
            }
            return Json(new { ret }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }

}
