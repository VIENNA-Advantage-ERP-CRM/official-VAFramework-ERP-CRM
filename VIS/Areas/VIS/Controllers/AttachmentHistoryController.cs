using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;
using VIS.Classes;
using VAdvantage.DataBase;

namespace VIS.Controllers
{
    public class AttachmentHistoryController : Controller
    {
        //
        // GET: /VIS/AttachmentHistory/
        public ActionResult Index()
        {
            return View();
        }



        
        public JsonResult LoadRecordDataCount(string searchText, int _AD_Table_ID, int _Record_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            return Json(JsonConvert.SerializeObject(model.LoadRecordDataCount(ctx, searchText, _AD_Table_ID, _Record_ID)), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult LoadRecordData(string searchText, int _AD_Table_ID, int _Record_ID, int historyPageNo, int pageSize)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            return Json(JsonConvert.SerializeObject(model.LoadRecordData(ctx, searchText, _AD_Table_ID, _Record_ID, historyPageNo, pageSize)), JsonRequestBehavior.AllowGet);
        }


        public JsonResult RelatedHistory(int keyColumnID, int pageSize, int pageNo, string searchText, string keyColName)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            RealtedHistoryInfoDetails hisIfno = model.history(keyColumnID, pageSize, pageNo, ct, searchText, keyColName);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserHistory(int C_BPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            RealtedHistoryInfoDetails hisIfno = model.Userhistory(C_BPartner_ID, pageSize, pageNo, ct, searchText);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadSentMails(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.Sentmails(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadInboxMails(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.InboxMails(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLetters(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.Letters(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadAttachment(int ID, string Name)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Name = Server.HtmlDecode(Name);
            MMailAttachment1 _mAttachment = new MMailAttachment1(ct, ID, null);
            string path = "";
            var fName = "";
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                if (Name.ToUpper() == oMAttachEntry.GetName().ToUpper())
                {
                    fName = DateTime.Now.Ticks.ToString() + Name;
                    path = Path.Combine(Server.MapPath("~/TempDownload"), fName);
                    byte[] bytes = oMAttachEntry.GetData();

                    using (FileStream fs = new FileStream(path, FileMode.Append, System.IO.FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    break;
                }
            }

            return Json(JsonConvert.SerializeObject(fName), JsonRequestBehavior.AllowGet);
        }

        // Call history attachment download work by vinay
        public JsonResult DownloadAttachmentCall(int AttID, int ID, string Name)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Name = Server.HtmlDecode(Name);
            //string path = "";
            var fName = "";

            MAttachment att = new MAttachment(ct, AttID, null);
            fName = att.GetFile(ID);

            if (!fName.StartsWith("ERROR:"))
                fName = att.GetFile(ID) + "/" + Name;
            else
                fName = "";

            return Json(JsonConvert.SerializeObject(fName), JsonRequestBehavior.AllowGet);
        }

        // updated for call history chat by vinay
        public JsonResult ViewChatonHistory(int record_ID, bool isAppointment, bool isCall = false)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            List<ChatInfos> cInfo = his.ViewChatonHistory(ct, record_ID, isAppointment, isCall);

            return Json(JsonConvert.SerializeObject(cInfo), JsonRequestBehavior.AllowGet);

        }

        // updated for call history chat by vinay
        public JsonResult ViewChatonLastHistory(int record_ID, bool isAppointment, bool isCall = false)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            List<ChatInfos> cInfo = his.ViewChatonLastHistory(ct, record_ID, isAppointment, isCall);

            return Json(JsonConvert.SerializeObject(cInfo), JsonRequestBehavior.AllowGet);

        }

        // updated for call history comment by vinay
        public JsonResult SaveComment(int ID, string Text, bool isAppointment, bool isCall = false)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            return Json(JsonConvert.SerializeObject(his.SaveComment(ID, Text, isAppointment, ct, isCall)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 07 June 2017
        public JsonResult GetAppointmentData(string AppQry)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            AppQry = SecureEngineBridge.DecryptByClientKey(AppQry, ct.GetSecureKey());
            return Json(JsonConvert.SerializeObject(his.GetAppointmentData(AppQry)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 07 June 2017
        public JsonResult GetUser(string UserQry)
        {
            Ctx ct = Session["ctx"] as Ctx;
            //UserQry = SecureEngineBridge.DecryptByClientKey(UserQry, ct.GetSecureKey());
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            return Json(JsonConvert.SerializeObject(his.GetUser(UserQry)), JsonRequestBehavior.AllowGet);
        }

        // Load call details for history
        public JsonResult LoadCall(int ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            CallInfo hisInfo = model.Calls(ctx, ID);
            return Json(JsonConvert.SerializeObject(hisInfo), JsonRequestBehavior.AllowGet);
        }

        //// Added by Bharat on 09 June 2017
        //public JsonResult GetRecordDataCount(string DataCountQry)
        //{
        //    Ctx ct = Session["ctx"] as Ctx;
        //    DataCountQry = SecureEngineBridge.DecryptByClientKey(DataCountQry, ct.GetSecureKey());
        //    AttachmentHistoryModel his = new AttachmentHistoryModel();
        //    return Json(JsonConvert.SerializeObject(his.GetRecordDataCount(DataCountQry)), JsonRequestBehavior.AllowGet);
        //}

        //// Added by Bharat on 09 June 2017
        //public JsonResult GetRecordData(string DataQry)
        //{
        //    Ctx ct = Session["ctx"] as Ctx;
        //    DataQry = SecureEngineBridge.DecryptByClientKey(DataQry, ct.GetSecureKey());
        //    AttachmentHistoryModel his = new AttachmentHistoryModel();
        //    return Json(JsonConvert.SerializeObject(his.GetRecordData(DataQry)), JsonRequestBehavior.AllowGet);
        //}
    }
}