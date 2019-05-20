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

        public InitAttachment GetAttachment(int AD_Table_ID, int Record_ID, VAdvantage.Utility.Ctx ctx)
        {
            InitAttachment initialData = new InitAttachment();
            initialData.Attachment = GetFileAttachment(AD_Table_ID, Record_ID, ctx);
            initialData.FLocation = GetFileLocations(ctx);
            return initialData;
        }

        private MAttachment GetFileAttachment(int AD_Table_ID, int Record_ID, VAdvantage.Utility.Ctx ctx)
        {
            int AD_Attachment_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Attachment_ID from AD_Attachment WHERE ad_table_id =" + AD_Table_ID + " AND record_id=" + Record_ID, null, null));
            if (AD_Attachment_ID == 0)
            {
                return null;
            }
            MAttachment att = new MAttachment(ctx, AD_Attachment_ID, null);
            att.AD_Attachment_ID = AD_Attachment_ID;
            return att;
        }

        private FileLocation GetFileLocations(Ctx ctx)
        {
            //MAttachment att = new MAttachment(ctx, AD_Table_ID, Record_ID, null);
            //return att;
            FileLocation locations = new FileLocation();
            int AD_Reference_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select AD_Reference_Value_ID from ad_Column WHERE AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='AD_ClientInfo') AND UPPER(ColumnName)='SAVEATTACHMENTON'", null, null));

            locations.values = MRefList.GetList(AD_Reference_ID, false, ctx);
            locations.selectedvalue = Util.GetValueOfString(DB.ExecuteScalar("Select SAVEATTACHMENTON From AD_CLientInfo WHERE Ad_client_ID=" + ctx.GetAD_Client_ID(), null, null));
            return locations;
        }

        public AttachmentInfo CreateAttachmentEntries(List<AttFileInfo> _files, int AD_Attachment_ID, string folderKey, Ctx ctx, int AD_Table_ID, int Record_ID, string fileLocation, int newRecord_ID, bool IsDMSAttachment)
        {
            AttachmentInfo info = new AttachmentInfo();

            MAttachment att = null;
            if (newRecord_ID > 0)           //This is to copy old reocrd's attachment in new record. will work only in case of DMS..
            {
                att = new MAttachment(ctx, 0, null);
                att.SetRecord_ID(newRecord_ID);
            }
            else
            {
                att = new MAttachment(ctx, AD_Attachment_ID, null);
                att.SetRecord_ID(Record_ID);
            }

            if (IsDMSAttachment && newRecord_ID == 0 && AD_Attachment_ID > 0)
            {
                DB.ExecuteQuery("DELETE FROM AD_AttachmentLine WHERE AD_Attachment_ID=" + AD_Attachment_ID, null, null);
            }
            att.SetAD_Table_ID(AD_Table_ID);
            att.SetAttFileInfo(_files);
            att.FolderKey = folderKey;
            att.SetFileLocation(fileLocation);
            att.SetIsFromHTML(true);
            att.Save();
            info.AD_attachment_ID = att.GetAD_Attachment_ID();
            info.Error = att.Error;

            return info;
        }

        private void CopyRecord(MAttachment att, int AD_Table_ID, int newRecord_ID, Ctx ctx)
        {
            MAttachment newAttachment = new MAttachment(ctx, AD_Table_ID, newRecord_ID, null);
            att.CopyTo(newAttachment);

            //for (int i = 0; i < att.GetEntryCount(); i++)
            //{
            //    newAttachment.AddEntry(att.GetEntry(i));
            //}

            att = null;
            att = newAttachment;
        }

        public string DownloadAttachment(Ctx ctx, string fileName, int AD_Attachment_ID, int AD_AttachmentLine_ID)
        {
            MAttachment att = new MAttachment(ctx, AD_Attachment_ID, null);

            return att.GetFile(AD_AttachmentLine_ID);
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
            return DB.ExecuteQuery("DELETE FROM AD_AttachmentLine WHERE AD_AttachmentLine_ID IN (" + AttachmentLines + ")", null, null);


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
        public int AD_attachment_ID { get; set; }
    }

    public class InitAttachment
    {
        public MAttachment Attachment
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