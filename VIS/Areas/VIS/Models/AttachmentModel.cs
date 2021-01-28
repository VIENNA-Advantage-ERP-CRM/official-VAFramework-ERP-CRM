using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace ViennaAdvantageWeb.Areas.VIS.Models
{
    public class AttachmentModel
    {

        public InitAttachment GetAttachment(int VAF_TableView_ID, int Record_ID, VAdvantage.Utility.Ctx ctx)
        {
            InitAttachment initialData = new InitAttachment();
            initialData.Attachment = GetFileAttachment(VAF_TableView_ID, Record_ID, ctx);
            initialData.FLocation = GetFileLocations(ctx);
            return initialData;
        }

        private MVAFAttachment GetFileAttachment(int VAF_TableView_ID, int Record_ID, VAdvantage.Utility.Ctx ctx)
        {
            int VAF_Attachment_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_Attachment_ID from VAF_Attachment WHERE vaf_tableview_id =" + VAF_TableView_ID + " AND record_id=" + Record_ID, null, null));
            if (VAF_Attachment_ID == 0)
            {
                return null;
            }
            MVAFAttachment att = new MVAFAttachment(ctx, VAF_Attachment_ID, null);
            att.VAF_Attachment_ID = VAF_Attachment_ID;
            return att;
        }

        private FileLocation GetFileLocations(Ctx ctx)
        {
            //MAttachment att = new MAttachment(ctx, VAF_TableView_ID, Record_ID, null);
            //return att;
            FileLocation locations = new FileLocation();
            int VAF_Control_Ref_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VAF_Control_Ref_Value_ID from vaf_column WHERE VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName='VAF_ClientDetail') AND UPPER(ColumnName)='SAVEATTACHMENTON'", null, null));

            locations.values = MRefList.GetList(VAF_Control_Ref_ID, false, ctx);
            locations.selectedvalue = Util.GetValueOfString(DB.ExecuteScalar("Select SAVEATTACHMENTON From VAF_ClientDetail WHERE vaf_client_ID=" + ctx.GetVAF_Client_ID(), null, null));
            return locations;
        }

        public AttachmentInfo CreateAttachmentEntries(List<AttFileInfo> _files, int VAF_Attachment_ID, string folderKey, Ctx ctx, int VAF_TableView_ID, int Record_ID, string fileLocation, int newRecord_ID, bool IsDMSAttachment)
        {
            AttachmentInfo info = new AttachmentInfo();

            MVAFAttachment att = null;
            if (newRecord_ID > 0)           //This is to copy old reocrd's attachment in new record. will work only in case of DMS..
            {
                att = new MVAFAttachment(ctx, 0, null);
                att.SetRecord_ID(newRecord_ID);
            }
            else
            {
                att = new MVAFAttachment(ctx, VAF_Attachment_ID, null);
                att.SetRecord_ID(Record_ID);
            }

            if (IsDMSAttachment && newRecord_ID == 0 && VAF_Attachment_ID > 0)
            {
                DB.ExecuteQuery("DELETE FROM VAF_AttachmentLine WHERE VAF_Attachment_ID=" + VAF_Attachment_ID, null, null);
            }
            att.SetVAF_TableView_ID(VAF_TableView_ID);
            att.SetAttFileInfo(_files);
            att.FolderKey = folderKey;
            att.SetFileLocation(fileLocation);
            att.SetIsFromHTML(true);
            att.Save();
            info.VAF_Attachment_ID = att.GetVAF_Attachment_ID();
            info.Error = att.Error;

            return info;
        }

        private void CopyRecord(MVAFAttachment att, int VAF_TableView_ID, int newRecord_ID, Ctx ctx)
        {
            MVAFAttachment newAttachment = new MVAFAttachment(ctx, VAF_TableView_ID, newRecord_ID, null);
            att.CopyTo(newAttachment);

            //for (int i = 0; i < att.GetEntryCount(); i++)
            //{
            //    newAttachment.AddEntry(att.GetEntry(i));
            //}

            att = null;
            att = newAttachment;
        }

        public string DownloadAttachment(Ctx _ctx, string fileName, int VAF_Attachment_ID, int VAF_AttachmentLine_ID, string actionOrigin, string originName, int VAF_TableView_ID, int recordID)
        {
            //Saved Action Log
            VAdvantage.Common.Common.SaveActionLog(_ctx, actionOrigin, originName, VAF_TableView_ID, recordID, 0, "", "", "Attachment Downloaded:->" + fileName, MActionLog.ACTIONTYPE_Download);

            MVAFAttachment att = new MVAFAttachment(_ctx, VAF_Attachment_ID, null);

            return att.GetFile(VAF_AttachmentLine_ID);
        }

        public string DownloadAttachment(Ctx ctx, string fileName, int VAF_Attachment_ID, int VAF_AttachmentLine_ID)
        {
            MVAFAttachment att = new MVAFAttachment(ctx, VAF_Attachment_ID, null);

            return att.GetFile(VAF_AttachmentLine_ID);
        }

        public int DeleteAttachment(string AttachmentLines)
        {
            if (AttachmentLines == null || AttachmentLines.Length == 0)
            {
                return 0;
            }
            if (AttachmentLines.EndsWith(","))
            {
                AttachmentLines = AttachmentLines.Substring(0, AttachmentLines.Length - 1);
            }
            return DB.ExecuteQuery("DELETE FROM VAF_AttachmentLine WHERE VAF_AttachmentLine_ID IN (" + AttachmentLines + ")", null, null);


        }
    }

    public class FileLocation
    {
        public ValueNamePair[] values
        {
            get;
            set;
        }

        public string selectedvalue
        {
            get;
            set;
        }
    }

    public class AttachmentInfo
    {
        public string Error { get; set; }
        public int VAF_Attachment_ID { get; set; }
    }

    public class InitAttachment
    {
        public MVAFAttachment Attachment
        {
            get;
            set;
        }
        public FileLocation FLocation
        {
            get;
            set;
        }
    }

}