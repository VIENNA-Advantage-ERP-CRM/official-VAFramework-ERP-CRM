using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class GroupController : Controller
    {
        //
        // GET: /VIS/Group/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserInfo(string searchText, int sortBy, int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            List<UserInfo> users = model.GetUserInfo(searchText, sortBy, pageNo, pageSize);
            return Json(JsonConvert.SerializeObject(users), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActiveUser(int AD_User_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            return Json(JsonConvert.SerializeObject(model.ActiveUser(AD_User_ID)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InActiveUser(int AD_User_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            return Json(JsonConvert.SerializeObject(model.InActiveUser(AD_User_ID)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleInfo(int AD_User_ID,string name)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            List<RolesInfo> roles = model.GetRoleInfo(AD_User_ID,name);
            return Json(JsonConvert.SerializeObject(roles), JsonRequestBehavior.AllowGet);
        }

            [HttpPost]
        public JsonResult UpdateUserRoles(int AD_User_ID, string roles)
        {
            Ctx ct = Session["ctx"] as Ctx;
            List<RolesInfo> rInfo = null;
            if (roles != null && roles.Length > 0)
            {
                rInfo = JsonConvert.DeserializeObject<List<RolesInfo>>(roles);
            }
            Group model = new Group(ct);
            model.UpdateUserRoles(AD_User_ID, rInfo);
            return Json(JsonConvert.SerializeObject(""), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGroupInfo(int AD_Role_ID, string name)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            List<GroupInfo> roles = model.GetGroupInfo(AD_Role_ID, name);
            return Json(JsonConvert.SerializeObject(roles), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUserGroups(int AD_Role_ID, string groups)
        {
            Ctx ct = Session["ctx"] as Ctx;
            List<GroupInfo> gInfo = null;
            //List<RolesInfo> rInfo = null;
            if (groups != null && groups.Length > 0)
            {
                gInfo = JsonConvert.DeserializeObject<List<GroupInfo>>(groups);
            }
            //if (AD_Role_ID != null && AD_Role_ID.Length > 0)
            //{
            //    rInfo = JsonConvert.DeserializeObject<List<RolesInfo>>(AD_Role_ID);
            //}


            Group model = new Group(ct);
            model.UpdateUserGroup(AD_Role_ID, gInfo);
            return Json(JsonConvert.SerializeObject(""), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNewUser(string Name, string Email, string Value, string password, string mobile, string OrgID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            List<int> oID = null;
            if (OrgID != null && OrgID.Length > 0)
            {
                oID = JsonConvert.DeserializeObject<List<int>>(OrgID);
            }
            CreateUser user = new CreateUser(ct);
            return Json(JsonConvert.SerializeObject(user.SaveNewUser(Name, Email, Value, password, mobile, oID)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgAccess()
        {
            Ctx ct = Session["ctx"] as Ctx;
            CreateUser user = new CreateUser(ct);
            return Json(JsonConvert.SerializeObject(user.GetOrgAccess()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWindowIds()
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            return Json(JsonConvert.SerializeObject(model.GetWindowIds()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGroupChildInfo(int groupID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            return Json(JsonConvert.SerializeObject(model.GetGroupInfo(groupID)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserLevel()
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            return Json(JsonConvert.SerializeObject(model.GetUserLevel()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNewRole(string name, string userLevel, string OrgID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Group model = new Group(ct);
            List<int> oID = null;
            if (OrgID != null && OrgID.Length > 0)
            {
                oID = JsonConvert.DeserializeObject<List<int>>(OrgID);
            }
            return Json(JsonConvert.SerializeObject(model.AddNewRole(name, userLevel, oID)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InviteUsers(string email, string roles)
        {
            Ctx ct = Session["ctx"] as Ctx;
            CreateUser model = new CreateUser(ct);

            List<RolesInfo> infos = JsonConvert.DeserializeObject<List<RolesInfo>>(roles);

            return Json(JsonConvert.SerializeObject(model.InviteUsers(email,infos)), JsonRequestBehavior.AllowGet);
        }

    }
}