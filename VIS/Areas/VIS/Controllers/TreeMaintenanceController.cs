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

    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class TreeMaintenanceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult TreeDataForCombo()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.TreeDataForCombo();
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindTree(string treeType, int AD_Tree_ID, string isAllNodes, bool isSummary)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.BindTree(ctx, treeType, AD_Tree_ID, isAllNodes, isSummary);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMenuData(int pageLength, int pageNo, int treeID, string searchtext, string demandsMenu)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.LoadMenuData(ctx, pageLength, pageNo, treeID, searchtext, demandsMenu);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveDataOnDrop(int summaryid, int nodid, int treeID, int dragMenuNodeID)
        public JsonResult SaveDataOnDrop(int summaryid, int nodid, int treeID, string dragMenuNodeID, bool checkMorRdragable, string IsExistItem)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.SaveDataOnDrop(ctx, summaryid, nodid, treeID, dragMenuNodeID, checkMorRdragable, IsExistItem);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataTreeNodeSelect(int nodeID, int treeID, int pageNo, int pageLength, string searchChildNode, string getTreeNodeChkValue)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.GetDataTreeNodeSelect(ctx, nodeID, treeID, pageNo, pageLength, searchChildNode, getTreeNodeChkValue);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodePath(int node_ID, int treeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.GetNodePath(ctx, node_ID, treeID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveTreeDragDrop(int treeID, int NodeID, int ParentID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.SaveTreeDragDrop(ctx, treeID, NodeID, ParentID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
        ////unlinkchild
        public JsonResult DeleteNodeFromTree(int nodeid, int treeID, string unlinkchild, string menuArray)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.DeleteNodeFromTree(ctx, nodeid, treeID, unlinkchild, menuArray);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteNodeFromBottom(string nodeid, int treeID, string menuArray)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.DeleteNodeFromBottom(ctx, nodeid, treeID, menuArray);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult TreeTableName(int treeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.TreeTableName(ctx, treeID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateItemSeqNo(int treeID, string itemsid, int ParentID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.UpdateItemSeqNo(ctx, treeID, itemsid, ParentID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }





        public JsonResult SelectAllChildNodes(string TableName, int treeID, string nodeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.SelectAllChildNodes(ctx,  TableName,  treeID,  nodeID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }




        public JsonResult FillSequenceDailog(int nodeID, int treeID, int pageNo, int pageLength, string searchChildNode, string getTreeNodeChkValue)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.FillSequenceDailog(ctx, nodeID, treeID, pageNo, pageLength, searchChildNode, getTreeNodeChkValue);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }



        public JsonResult RemoveLinkedItemFromTree(int treeID, string menuId)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            TreeMaintenanceModel obj = new TreeMaintenanceModel(ctx);
            var result = obj.RemoveLinkedItemFromTree(ctx, treeID, menuId);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }





    }
}