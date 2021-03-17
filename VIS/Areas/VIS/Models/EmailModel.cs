/********************************************************
 * Project Name   : VIS
 * Class Name     : EmailModel
 * Purpose        : Used to perform server side tasks related to  email and letters...
 * Chronological    Development
 * Karan            
  ******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;

using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using VAdvantage.DataBase;
using VAdvantage.MailBox;
using VAdvantage.Model;
using VAdvantage.Utility;

using iTextSharp.text.html.simpleparser;
using VAdvantage.Classes;
using VIS.DataContracts;
using VIS.Helpers;

namespace VIS.Models
{
    public class EmailModel
    {
        Ctx ctx = null;

        string AttachmentsUploadFolderName = "TempDownload";
        // UserInformation userinfo = null;

        public EmailModel(Ctx ctx)
        {
            this.ctx = ctx;
        }


        /// <summary>
        /// Used to send mails .... Fetechs credentails used to send mails...
        /// </summary>
        /// <param name="mails"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="attachment_ID"></param>
        /// <param name="fileNames"></param>
        /// <param name="fileNameForOpenFormat"></param>
        /// <param name="mailFormat"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public string SendMails(List<NewMailMessage> mails, int AD_User_ID, int AD_Client_ID, int AD_Org_ID, int attachment_ID, List<string> fileNames,
            List<string> fileNameForOpenFormat, string mailFormat, bool notify, List<int> lstDocumentIds, int AD_Process_ID, string printformatfileType)
        {




            VAdvantage.Utility.EMail sendmail = new VAdvantage.Utility.EMail(ctx, "", "", "", "", "", "",
                  true, false);
            string isConfigExist = sendmail.IsConfigurationExist(ctx);
            if (isConfigExist != "OK")
            {
                return isConfigExist;
            }

            sendmail = null;


            if (notify)//if want to send mail on server and want notice on home screen.     Else u have to wait, and it will show alert message of return value....
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    SendMailstart(mails, AD_User_ID, AD_Client_ID, AD_Org_ID, attachment_ID, fileNames, fileNameForOpenFormat, mailFormat, notify, sendmail, lstDocumentIds, AD_Process_ID, printformatfileType);
                });
                return "";
            }
            else
            {
                return SendMailstart(mails, AD_User_ID, AD_Client_ID, AD_Org_ID, attachment_ID, fileNames, fileNameForOpenFormat, mailFormat, notify, sendmail, lstDocumentIds, AD_Process_ID, printformatfileType);
            }
        }

        /// <summary>
        /// this method actually send mail, both static and dynamic.... and save info in MailAttachment....
        /// </summary>
        /// <param name="mails"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="attachment_ID"></param>
        /// <param name="fileNames"></param>
        /// <param name="fileNameForOpenFormat"></param>
        /// <param name="mailFormat"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public string SendMailstart(List<NewMailMessage> mails, int AD_User_ID, int AD_Client_ID, int AD_Org_ID, int attachment_ID,
            List<string> fileNames, List<string> fileNameForOpenFormat, string mailFormat, bool notify, VAdvantage.Utility.EMail sendmails,
            List<int> documentID, int Ad_Process_ID,  string printformatfileType)
        {



            if (ctx == null)
            {
                return null;
            }


            UserInformation userinfo = new UserInformation();
            SMTPConfig config = null;
            config = MailConfigMethod.GetUSmtpConfig(AD_User_ID, ctx);
            // var config = "";
            if (config == null)
            {
                MClient client = new MClient(ctx, AD_Client_ID, null);
                userinfo.Email = client.GetRequestEMail();
            }
            else
            {
                //Add user info to list..
                userinfo.Email = config.Email;
            }

            string[] to = null;
            string[] bc = null;

            string[] cc = null;
            string sub = null;
            string message = null;
            // int _record_id = 0;
            int _table_id = 0;

            string[] records = null;

            StringBuilder res = new StringBuilder();

            List<NewMailMessage> mail = mails.GetRange(0, mails.Count);

            for (int j = 0; j < mails.Count; j++)
            {

                VAdvantage.Utility.EMail sendmail = new VAdvantage.Utility.EMail(ctx, "", "", "", "", "", "",
                      true, false);

                to = mails[j].To.Split(';');
                bc = mails[j].Bcc;
                cc = mails[j].Cc.Split(';');
                StringBuilder bcctext = new StringBuilder();
                sub = mails[j].Subject;
                message = mailFormat;
                if (mails[j].Body != null && mails[j].Body.Count > 0)
                {
                    List<string> keysss = mails[j].Body.Keys.ToList();
                    for (int q = 0; q < keysss.Count; q++)
                    {
                        message = message.Replace(keysss[q], mails[j].Body[keysss[q]]);
                    }
                }

                if (mails[j].Recordids != null)          //in case of static mail
                {
                    records = mails[j].Recordids.Split(',');
                }

                _table_id = Convert.ToInt32(mail[j].TableID);

                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(ctx, 0, null);




                if (sub == null || sub.Length == 0 || sub == "")
                    sendmail.SetSubject(".");	//	pass validation
                else
                    sendmail.SetSubject(sub);
                sendmail.SetMessageHTML(message);


                //used to get attachments uploaded by user.....
                if (mail[j].AttachmentFolder != null && mail[j].AttachmentFolder.Trim().Length > 0)
                {
                    string storedAttachmentPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, AttachmentsUploadFolderName + "\\" + mail[j].AttachmentFolder);
                    if (Directory.Exists(storedAttachmentPath))
                    {
                        DirectoryInfo info = new DirectoryInfo(storedAttachmentPath);
                        if (info.GetFiles().Length > 0)
                        {
                            FileInfo[] files = info.GetFiles();

                            for (int a = 0; a < files.Length; a++)
                            {
                                if (fileNames.Contains(files[a].Name))
                                {
                                    FileStream attachmentStream = File.OpenRead(files[a].FullName);
                                    BinaryReader binary = new BinaryReader(attachmentStream);
                                    byte[] buffer = binary.ReadBytes((int)attachmentStream.Length);
                                    sendmail.AddAttachment(buffer, files[a].Name);
                                    _mAttachment.AddEntry(files[a].Name, buffer);
                                }
                            }
                        }
                    }
                }

                //used to get attachments of saved formats.. Currently not Supporting......
                if (attachment_ID > 0)
                {
                    VAdvantage.Model.MMailAttachment1 _mAttachment1 = new VAdvantage.Model.MMailAttachment1(ctx, attachment_ID, null);
                    if (_mAttachment1.GetEntryCount() > 0)
                    {
                        MAttachmentEntry[] entries = _mAttachment1.GetEntries();
                        for (int m = 0; m < entries.Count(); m++)
                        {
                            //if (fileNameForOpenFormat.Contains(entries[m].GetName()))
                            //{
                            byte[] buffer = entries[m].GetData();
                            sendmail.AddAttachment(buffer, entries[m].GetName());
                            _mAttachment.AddEntry(entries[m].GetName(), buffer);
                            //}
                        }
                    }
                }

                if (documentID != null || documentID.Count > 0)
                {
                    for (int i = 0; i < documentID.Count; i++)
                    {
                        try
                        {
                            int attachmentID = AttachmentID(documentID[i]);
                            //MAttachment objAttachment = new MAttachment(ctx, attachmentID, null, Common.GetPassword(), ctx.GetAD_Client_ID());
                            //objAttachment.Force = false;
                            ////objAttachment.AD_Client_ID = ctx.GetAD_Client_ID();
                            //byte[] fileByte = objAttachment.GetEntryData(0);
                            //string fileName = objAttachment.GetEntryName(0);
                            List<AttachedFileInfo> lstAttchments = GetBytes(ctx, attachmentID, "");
                            sendmail.AddAttachment(lstAttchments[0].FileBytes, lstAttchments[0].FileName);

                        }
                        catch (Exception ex)
                        {
                            res.Append(Msg.GetMsg(ctx, "VADMS_AttachmentNotFound for : " + documentID[i]) + ex.Message);
                        }
                    }
                }


                //Lakhwinder
                //AttachPrintFormat
                if (!string.IsNullOrEmpty(printformatfileType) && printformatfileType != "X")
                {
                    ProcessReportInfo rep = AttachPrintFormat(mails[j].TableID, Ad_Process_ID, mails[j].Recordids, 0, printformatfileType);
                    if (!string.IsNullOrEmpty(rep.ReportFilePath))
                    {
                        FileStream attachmentStream = File.OpenRead(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, rep.ReportFilePath));
                        BinaryReader binary = new BinaryReader(attachmentStream);
                        byte[] buffer = binary.ReadBytes((int)attachmentStream.Length);
                        sendmail.AddAttachment(buffer, rep.ReportFilePath.Substring(rep.ReportFilePath.LastIndexOf('\\') + 1));
                    }
                }

                if (to != null)
                {

                    for (int i = 0; i < to.Length; i++)
                    {
                        if (to[i].ToString() != "")
                        {
                            sendmail.AddTo(to[i].ToString(), "");
                            // totext.Append(to[i].ToString() + ",");
                        }
                    }
                }

                if (bc != null)
                {
                    for (int i = 0; i < bc.Length; i++)
                    {
                        if (bc[i].ToString() != "")
                        {
                            sendmail.AddBcc(bc[i].ToString(), "");
                            bcctext.Append(bc[i].ToString() + ",");
                        }
                    }
                }

                if (cc != null)
                {
                    for (int i = 0; i < cc.Length; i++)
                    {
                        if (cc[i].ToString() != "")
                        {
                            sendmail.AddCc(cc[i].ToString(), "");
                            ///  cctext.Append(cc[i].ToString() + ",");
                        }
                    }
                }

                string res1 = sendmail.Send();

                if (records != null && records.Length > 0)//save entery in MailAttachment......
                {
                    for (int k = 0; k < records.Length; k++)
                    {
                        if (records[k] == null || records[k] == "" || records[k] == "0")
                        {
                            continue;
                        }
                        if (res1 != "OK")
                        {
                            _mAttachment.SetIsMailSent(false);
                        }
                        else
                        {
                            _mAttachment.SetIsMailSent(true);
                        }
                        int AD_Client_Id = ctx.GetAD_Client_ID();
                        int iOrgid = ctx.GetAD_Org_ID();

                        _mAttachment.SetAD_Client_ID(AD_Client_Id);
                        _mAttachment.SetAD_Org_ID(iOrgid);
                        _mAttachment.SetAD_Table_ID(_table_id);
                        _mAttachment.IsActive();
                        _mAttachment.SetMailAddress(bcctext.ToString());
                        _mAttachment.SetAttachmentType("M");

                        _mAttachment.SetRecord_ID(Convert.ToInt32(records[k]));

                        _mAttachment.SetTextMsg(message);
                        _mAttachment.SetTitle(sub);

                        _mAttachment.SetMailAddressBcc(bcctext.ToString());
                        _mAttachment.SetMailAddress(mails[j].To);
                        _mAttachment.SetMailAddressCc(mails[j].Cc);
                        _mAttachment.SetMailAddressFrom(userinfo.Email);
                        if (_mAttachment.GetEntries().Length > 0)
                        {
                            _mAttachment.SetIsAttachment(true);
                        }
                        else
                        {
                            _mAttachment.SetIsAttachment(false);
                        }
                        _mAttachment.NewRecord();
                        if (_mAttachment.Save())
                        { }
                        else
                        {
                            // log.SaveError(Msg.GetMsg(Env.GetCtx(), "RecordNotSaved"), "");
                        }
                    }
                }

                if (res1 != "OK")           // if mail not sent....
                {
                    if (res1 == "AuthenticationFailed.")
                    {
                        res.Append("AuthenticationFailed");
                        return res.ToString();
                    }
                    else if (res1 == "ConfigurationIncompleteOrNotFound")
                    {
                        res.Append("ConfigurationIncompleteOrNotFound");
                        return res.ToString();
                    }
                    else
                    {
                        res.Append(" " + Msg.GetMsg(ctx, "MailNotSentTo") + ": " + mails[j].To + " " + bcctext + " " + mails[j].Cc);
                    }


                }
                else
                {
                    {
                        if (!res.ToString().Contains("MailSent"))
                        {
                            res.Append("MailSent");
                        }
                    }

                }
                bcctext = null;
            }

            if (notify)             //  make an entry in Notice window.....
            {
                MNote note = new MNote(ctx, "SentMailNotice", AD_User_ID,
                    AD_Client_ID, AD_Org_ID, null);
                //  Reference
                note.SetReference(ToString());	//	Document
                //	Text
                note.SetTextMsg(res.ToString());
                note.Save();
            }

            userinfo = null;
            cc = null;
            bc = null;
            to = null; records = null;
            sub = null; message = null;
            records = null;
            return res.ToString();
        }

        /// <summary>
        /// Get Attachment Id From Metadata Table When DMS Module Is Installed
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
        private int AttachmentID(int documentID)
        {
            try
            {
                int id = 0;
                id = Convert.ToInt32(DB.ExecuteScalar("SELECT AD_ATTACHMENT_ID FROM VADMS_METADATA WHERE VADMS_DOCUMENT_ID=" + documentID + " and (vadms_innerversion=(SELECT max(vadms_innerversion) FROM vadms_metadata where vadms_document_id=" + documentID + ") or vadms_innerversion is null)"));
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Get File Attchment 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attachmentID"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<AttachedFileInfo> GetBytes(Ctx ctx, int attachmentID, string fileName)
        {
            byte[] byteArray = null;
            string filepath = string.Empty;
            List<AttachedFileInfo> lstAttchment = new List<AttachedFileInfo>();
            MAttachment attachment = new MAttachment(ctx, Convert.ToInt32(attachmentID), null, "", Convert.ToInt32(ctx.GetAD_Client_ID()));
            attachment.FolderKey = "";
            attachment.GetLines();
            if (attachment._lines != null && attachment._lines.Count > 0)
            {
                filepath = Path.Combine(System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", attachment.GetFile(attachment._lines[0].Line_ID)));
                filepath = filepath + "\\" + attachment._lines[0].FileName;
            }
            byteArray = File.ReadAllBytes(filepath);
            if (File.Exists(filepath)) //Delete Folder from TempDownload
            {
                System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(filepath.Substring(0, filepath.LastIndexOf("\\")));

                foreach (FileInfo file in myDirInfo.GetFiles())
                {
                    file.Delete();
                }
                Directory.Delete(filepath.Substring(0, filepath.LastIndexOf("\\")));
            }
            AttachedFileInfo objFiles = new AttachedFileInfo()
            {
                FileName = attachment._lines[0].FileName,
                FileBytes = byteArray
            };
            lstAttchment.Add(objFiles);
            return lstAttchment;

        }

        /// <summary>
        /// Save new mail formats and update existing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="name"></param>
        /// <param name="isDynamic"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="saveforAll"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="folder"></param>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public int SaveFormats(int id, int AD_Client_ID, int AD_Org_ID, string name, bool isDynamic, string subject, string text, bool saveforAll, int AD_Window_ID, string folder, int attachmentID)
        {
            X_AD_TextTemplate _textTemplate = new X_AD_TextTemplate(ctx, id, null);
            _textTemplate.Set_Value("AD_Client_ID", AD_Client_ID);
            _textTemplate.Set_Value("AD_Org_ID", AD_Org_ID);
            _textTemplate.Set_Value("Name", name);
            _textTemplate.Set_Value("IsActive", true);
            _textTemplate.Set_Value("IsHtml", true);
            _textTemplate.Set_Value("IsDynamicContent", isDynamic);
            if (subject.Trim().Length > 0)
            {
                _textTemplate.Set_Value("Subject", subject);
            }
            else
            {
                _textTemplate.Set_Value("Subject", name);
            }

            _textTemplate.Set_Value("MailText", text);

            if (!saveforAll)
            {
                _textTemplate.Set_Value("AD_Window_ID", AD_Window_ID);
            }
            if (_textTemplate.Save())
            {
                //MMailAttachment1 mAttachment = new MMailAttachment1(ctx, attachmentID, null);
                //mAttachment.SetAD_Client_ID(AD_Client_ID);
                //mAttachment.SetAD_Org_ID(AD_Org_ID);
                //mAttachment.SetAD_Table_ID(_textTemplate.Get_Table_ID());
                //mAttachment.IsActive();
                //mAttachment.SetAttachmentType("Z");
                ////get first key coloumn
                //mAttachment.SetRecord_ID(_textTemplate.GetAD_TextTemplate_ID());
                //mAttachment.SetTextMsg(text);
                //mAttachment.SetTitle(name);
                //FileInfo[] files = null;
                //if (folder != null && folder.Trim().Length > 0)
                //{
                //    string storedAttachmentPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, AttachmentsUploadFolderName + "\\" + folder);
                //    if (Directory.Exists(storedAttachmentPath))
                //    {
                //        DirectoryInfo info = new DirectoryInfo(storedAttachmentPath);
                //        if (info.GetFiles().Length > 0)
                //        {
                //            files = info.GetFiles();

                //            for (int a = 0; a < files.Length; a++)
                //            {

                //                FileStream attachmentStream = File.OpenRead(files[a].FullName);
                //                BinaryReader binary = new BinaryReader(attachmentStream);
                //                byte[] buffer = binary.ReadBytes((int)attachmentStream.Length);
                //                mAttachment.AddEntry(files[a].Name, buffer);
                //                if (mAttachment.Save())
                //                {

                //                }
                //            }
                //        }
                //    }
                //}


                //if (files == null || files.Length == 0)
                //{
                //    mAttachment.AddEntry("", null);
                //}
                //files = null;
                return _textTemplate.GetAD_TextTemplate_ID();
            }
            return 0;
        }

        //public SavedAttachmentInfo SavedAttachmentForFormat(int textTemplate_ID)
        //{
        //    List<string> entry = new List<string>();
        //    string sql = "select MailAttachment1_ID from MailAttachment1 where ad_table_id=(Select AD_Table_ID from AD_Table where tablename='AD_TextTemplate') and record_id=" + textTemplate_ID;
        //    int attach = Util.GetValueOfInt(DB.ExecuteScalar(sql));

        //    MMailAttachment1 mattach = new MMailAttachment1(ctx, attach, null);
        //    MAttachmentEntry[] entries = mattach.GetEntries();

        //    List<AttachmentInfo> aifo = new List<AttachmentInfo>();

        //    if (entries.Count() > 0)
        //    {
        //        for (int i = 0; i < entries.Count(); i++)
        //        {
        //            AttachmentInfo inf = new AttachmentInfo();
        //            inf.Name = entries[i].GetName();
        //            inf.Size = entries[i].GetData().Length.ToString();
        //            aifo.Add(inf);
        //        }
        //    }

        //    SavedAttachmentInfo info = new SavedAttachmentInfo();
        //    info.FileNames = aifo;
        //    info.AttachmentID = mattach.GetMailAttachment1_ID();

        //    return info;
        //}



        /// <summary>
        /// used to convert html to pdf ... mainly does makes a string of html with values of fields selected in mail during dynamci mails, otherewise created simple html...
        /// </summary>
        /// <param name="html"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string HtmlToPdf(string html, List<Dictionary<string, string>> values)
        {
            //string html = System.IO.File.ReadAllText(path);

            StringBuilder sHtml = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                string copy = html;
                List<string> keysss = values[i].Keys.ToList();
                for (int q = 0; q < keysss.Count; q++)
                {
                    copy = copy.Replace(keysss[q], values[i][keysss[q]]);
                }
                sHtml.Append(copy).Append("~");
            }

            //byte[] arrays = HtmlToPdfbytes(sHtml.ToString());
            //return Convert.ToBase64String(arrays);

            string filePath = HtmlToPdfbytes(sHtml.ToString(), false);
            return filePath;
        }


        /// <summary>
        /// Actually convert HTMl to PDF and return byte[]...
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public dynamic HtmlToPdfbytes(string html, bool loadByte)
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(sw);

            string htmlContent = "";
            if (ctx.GetIsRightToLeft())
            {
                htmlContent = @"<html dir='rtl'>
                <body>";
            }
            else
            {
                htmlContent = @"<html>
                <body>";
            }

            htmlContent += html;
            html += "</html></body>";
            string[] htmls = html.Split('~');

            byte[] docArray = null;
            StyleSheet style = new StyleSheet();

            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    writer.CloseStream = false;

                    if (ctx.GetIsRightToLeft())
                    {
                        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    }

                    // Write PDF here.

                    document.Open();

                    for (int i = 0; i < htmls.Count(); i++)
                    {
                        var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(htmls[i]), style);
                        foreach (var htmlElement in parsedHtmlElements)
                        {
                            //(((htmlElement as iTextSharp.text.pdf.PdfPTable).Rows[0] as iTextSharp.text.pdf.PdfPRow).GetCells())
                            //bool firstRow = true;
                            if ((htmlElement is iTextSharp.text.pdf.PdfPTable) && (htmlElement as iTextSharp.text.pdf.PdfPTable).Rows != null &&
                                (htmlElement as iTextSharp.text.pdf.PdfPTable).Rows.Count > 0)
                            {
                                (htmlElement as iTextSharp.text.pdf.PdfPTable).SpacingBefore = 5;
                                //(htmlElement as iTextSharp.text.pdf.PdfPTable).DefaultCell.PaddingBottom = 5;
                                foreach (var row in (htmlElement as iTextSharp.text.pdf.PdfPTable).Rows)
                                {
                                    PdfPCell[] cells = (row as iTextSharp.text.pdf.PdfPRow).GetCells();

                                    if (cells.Length > 0)
                                    {
                                        foreach (var cell in cells)
                                        {
                                            if (cell != null)
                                            {
                                                //if (firstRow)
                                                //{
                                                //    cell.Table. = 5;
                                                //    firstRow = false;
                                                //}
                                                cell.BorderColor = new BaseColor(150, 150, 150);
                                                cell.BorderWidth = 1;
                                            }
                                        }
                                    }
                                }
                            }

                            document.Add(htmlElement as IElement);
                        }

                        document.NewPage();
                    }
                    document.Close();

                }


                if (loadByte)
                {
                    docArray = memoryStream.ToArray();
                    return docArray;
                }
                else
                {
                    string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";

                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    filePath = filePath + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".pdf";
                    File.WriteAllBytes(filePath, memoryStream.ToArray());
                    filePath = filePath.Substring(filePath.IndexOf("TempDownload"));
                    return filePath;
                }

                //docArray = memoryStream.ToArray();
            }

            //}
            if (loadByte)
            {
                return docArray;
            }
            else
            {
                return "";
            }

        }

        private static void SetDirection(PdfPTable tbl)
        {
            tbl.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            tbl.HorizontalAlignment = Element.ALIGN_LEFT;
            foreach (PdfPRow pr in tbl.Rows)
            {
                foreach (PdfPCell pc in pr.GetCells())
                {
                    if (pc != null)
                    {
                        pc.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pc.HorizontalAlignment = Element.ALIGN_LEFT;
                        if (pc.CompositeElements != null)
                        {
                            foreach (var element in pc.CompositeElements)
                            {
                                if (element is PdfPTable)
                                {
                                    SetDirection((PdfPTable)element);
                                }
                            }
                        }
                    }
                }
            }
        }



        //Used to save letters as Attachment.... User can see saved letteres from History Form.......
        public string SaveAttachment(string subject, int AD_Table_ID, string html, Dictionary<string, Dictionary<string, string>> values)
        {
            StringBuilder strb = new StringBuilder();
            StringBuilder sHtml = new StringBuilder();
            //for (int i = 0; i < values.Count; i++)
            //{
            List<string> keysss = values.Keys.ToList();
            for (int q = 0; q < keysss.Count; q++)
            {
                string copy = html;

                Dictionary<string, string> val = values[keysss[q]];
                List<string> valKeys = val.Keys.ToList();

                for (int c = 0; c < valKeys.Count; c++)
                {
                    copy = copy.Replace(valKeys[c], val[valKeys[c]]);
                }

                byte[] arrays = HtmlToPdfbytes(copy, true);

                int AD_Client_Id = ctx.GetAD_Client_ID();
                int iOrgid = ctx.GetAD_Org_ID();
                MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, 0, null);
                _mAttachment.AddEntry(subject + ".pdf", arrays);
                _mAttachment.SetAD_Client_ID(AD_Client_Id);
                _mAttachment.SetAD_Org_ID(iOrgid);
                _mAttachment.SetAD_Table_ID(AD_Table_ID);
                _mAttachment.IsActive();
                _mAttachment.SetAttachmentType("L");
                //get first key coloumn

                _mAttachment.SetRecord_ID(Util.GetValueOfInt(keysss[q]));
                _mAttachment.SetTextMsg(copy);
                _mAttachment.SetTitle(subject);
                _mAttachment.NewRecord();
                if (_mAttachment.Save())
                {
                }




            }
            //}


            return strb.ToString();
        }


        // Added by Bharat on 09 June 2017
        public List<Dictionary<string, object>> GetUser(int bpartner)
        {
            List<Dictionary<string, object>> retDic = null;
            //Lakhwinder 17Mar2021 Bugfix apply IsActive check
            //string sql = "Select AD_User_ID, Email FROM AD_User WHERE IsEmail='Y' AND C_BPartner_ID=" + bpartner;
            string sql = "Select AD_User_ID, Email FROM AD_User WHERE IsEmail='Y' AND IsActive='Y' AND C_BPartner_ID=" + bpartner;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_User_ID"]);
                    obj["Email"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Email"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 09 June 2017
        public List<RecordData> GetMailFormat(int window_ID, Ctx ctx)
        {
            List<RecordData> obj = null;
            string sql = @"SELECT NAME, NVL(SUBJECT,' ') AS SUBJECT, NVL(MAILTEXT,' ') as MAILTEXT,
                    NVL(CREATED,sysdate) AS CREATED, AD_CLIENT_ID, AD_ORG_ID, CREATEDBY, 
                    NVL(ISACTIVE,'Y') AS ISACTIVE, NVL(ISHTML,'Y') AS ISHTML, AD_TEXTTEMPLATE_ID, 
                    NVL(UPDATED,sysdate) AS UPDATED, UPDATEDBY, AD_Window_ID, NVL(ISDYNAMICCONTENT,'N') AS ISDYNAMICCONTENT
                    FROM AD_TextTemplate WHERE IsActive = 'Y' AND (AD_Window_ID IS NULL ";
            if (window_ID > 0)
            {
                sql += " OR AD_Window_ID=" + window_ID;
            }
            sql += ")";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_TextTemplate", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new List<RecordData>();
                RecordData item = null;
                List<object> values = null;
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)  //columns
                {
                    item = new RecordData();

                    item.name = ds.Tables[0].Columns[i].ColumnName.ToUpper();
                    values = new List<object>();
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)  //rows
                    {
                        values.Add(ds.Tables[0].Rows[j][ds.Tables[0].Columns[i].ColumnName]);
                    }
                    item.value = values;
                    item.RowCount = ds.Tables[0].Rows.Count;
                    obj.Add(item);
                }
            }
            return obj;
        }

        /// <summary>
        /// Enhancement to provide facility to attacht print format directlty for selected record
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="processID"></param>
        /// <param name="recID"></param>
        /// <param name="windowNo"></param>
        /// <param name="fileType"></param>
        /// <param name="windowName"></param>
        /// <returns>details of print format file</returns>
        private ProcessReportInfo AttachPrintFormat(int tableID, int processID, string recID, int windowNo, string fileType)
        {


            string[] records = recID.Split(',');
            int Record_ID = Convert.ToInt32(records[0]);
            int pID = GetDoctypeBasedReport(tableID, Record_ID);
            if (pID > 0)
            {
                ctx.SetContext("FetchingDocReport", "Y");
                processID = pID;
            }
            ProcessReportInfo rep = null;
            if (records.Length < 2)
            {
                rep = ProcessHelper.GeneratePrint(ctx, processID, "Print", tableID, Record_ID, windowNo, "", fileType, "W", "");
            }
            else { rep = ProcessHelper.GeneratePrint(ctx, processID, "Print", tableID, 0, windowNo, recID, fileType, "W", ""); }
            ctx.SetContext("FetchingDocReport", "N");
            return rep;

        }
        /// <summary>
        /// Select report info based on Document type selected in that particular record.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tableID"></param>
        /// <param name="record_ID"></param>
        /// <returns>print format id on priority based on binded DocBaseType,DocumentType and Tab</returns>
        private int GetDoctypeBasedReport(int tableID, int record_ID)
        {
            #region To Override Default Process With Process Linked To Document Type

            string colName = "C_DocTypeTarget_ID";


            string sql1 = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + tableID + " AND ColumnName   ='C_DocTypeTarget_ID'";
            int id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            if (id < 1)
            {
                colName = "C_DocType_ID";
                sql1 = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + tableID + " AND ColumnName   ='C_DocType_ID'";
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            }

            if (id > 0)
            {

                string tableName = MTable.GetTableName(ctx, tableID);
                sql1 = "SELECT " + colName + ", AD_Org_ID FROM " + tableName + " WHERE " + tableName + "_ID =" + Util.GetValueOfString(record_ID);
                DataSet ds = DB.ExecuteDataset(sql1);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    // Check if Document Sequence has organization Level checked, if yes then get report from there.
                    // If Not, then try to get report from Document Type.
                    sql1 = @"SELECT AD_Sequence_No.Report_ID
                                From Ad_Sequence Ad_Sequence
                                JOIN C_Doctype C_Doctype
                                ON (C_Doctype.Docnosequence_Id =Ad_Sequence.Ad_Sequence_Id 
                                AND C_DocType.ISDOCNOCONTROLLED='Y')  
                                JOIN AD_Sequence_No AD_Sequence_No
                                On (Ad_Sequence_No.Ad_Sequence_Id=Ad_Sequence.Ad_Sequence_Id
                                AND Ad_Sequence_No.AD_Org_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]) + @")
                                JOIN AD_Process ON AD_Process.AD_Process_ID=AD_Sequence_No.Report_ID
                                Where C_Doctype.C_Doctype_Id     = " + Convert.ToInt32(ds.Tables[0].Rows[0][0]) + @"
                                And Ad_Sequence.Isorglevelsequence='Y' AND Ad_Sequence.IsActive='Y' AND AD_Process.IsActive='Y'";

                    object processID = DB.ExecuteScalar(sql1);
                    if (processID == DBNull.Value || processID == null || Convert.ToInt32(processID) == 0)
                    {
                        sql1 = "select Report_ID FRoM C_Doctype WHERE C_Doctype_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                        processID = DB.ExecuteScalar(sql1);
                    }
                    if (processID != DBNull.Value && processID != null && Convert.ToInt32(processID) > 0)
                    {
                        return Convert.ToInt32(processID);
                    }
                }
            }
            return 0;

            #endregion
        }


    }



    public class NewMailMessage
    {
        public string Subject { get; set; }

        public Dictionary<string, string> Body { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Sender { get; set; }

        public string[] Bcc { get; set; }

        public DateTime Date { get; set; }

        public bool IsHtml { get; set; }

        public int TableID { get; set; }

        //  public AttachmentInfo[] Attachments { get; set; }

        public string Recordids { get; set; }

        public string AttachmentFolder { get; set; }
    }



    public class KeyValues
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }



    public class SavedAttachmentInfo
    {
        public List<AttachmentInfo> FileNames { get; set; }
        public int AttachmentID { get; set; }
    }

    public class AttachmentInfo
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }

    public class AttachedFileInfo
    {
        //  [DataMember]
        public string FileName { get; set; }
        //   [DataMember]
        public string FileUID { get; set; }
        //  [DataMember]
        public long Size { get; set; }
        //   [DataMember]
        public string StoredFileName { get; set; }
        //[DataMember]
        //public byte[] Data { get; set; }
        // [DataMember]
        public string FileExtension { get; set; }
        // [DataMember]
        public string FileNameWithoutExtension { get; set; }
        //  [DataMember]
        public byte[] FileBytes { get; set; }
        //  [DataMember]
        public string DocName { get; set; }
        //  [DataMember]
        public string DocComment { get; set; }
        //  [DataMember]
        public string Keyword { get; set; }
        //[DataMember]
        //public string Version { get; set; }
        //  [DataMember]
        public string MetaComment { get; set; }
        //  [DataMember]
        public DateTime? FileCreatedDate { get; set; }
        //  [DataMember]
        public string Description { get; set; }
        //  [DataMember]
        public string EmailUID { get; set; }
        //  [DataMember]
        public int StartIndex { get; set; }
        // [DataMember]
        public int EndIndex { get; set; }
        //  [DataMember]
        public int Length { get; set; }
        public DateTime latestModifiedDate { get; set; }
        public string folderKey { get; set; }
        public double TotalNoOfFileSize { get; set; }
    }

}