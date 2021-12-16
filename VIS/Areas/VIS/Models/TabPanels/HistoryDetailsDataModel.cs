/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    History Details Tab Panel
 * Employee Code  :    60
 * Date           :    17-August-2021
  ******************************************************/

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HistoryDetailsDataModel
    {
        /// <summary>
        /// Getting History Records by passing parameters
        /// </summary>
        /// <param name="RecordId">Record ID</param>
        /// <param name="_AD_Table_ID">Table ID</param>
        /// <param name="CurrentPage">Page No</param>
        /// <returns>History Records</returns>
        public List<HistoryDetails> GetHistoryDetails(int RecordId, int _AD_Table_ID, string CurrentPage)
        {
            string sql = string.Empty;
            HistoryDetails lst = new HistoryDetails();
            List<HistoryDetails> LstHistory = new List<HistoryDetails>();

            sql = @"SELECT ID, AD_TABLE_ID, RECORD_ID, CREATED, FROMUSER, TYPE, SUBJECT, NAME, TO_CHAR(CREATED, 'DD/MM/YYYY HH12:MI:SS AM' ) AS CREATEDDATETIME, HASATTACHMENT, ISTASKCLOSED FROM "
                        + " ( "
                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'EMAIL' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'M' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'INBOX' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'I' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ch.CM_CHAT_ID AS ID, ch.AD_TABLE_ID, ch.RECORD_ID, ch.CREATED, au.NAME AS FROMUSER, 'CHAT' AS TYPE, CAST('Chat' AS NVARCHAR2(50)) AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM CM_CHAT ch "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ch.CREATEDBY "
                        + "   WHERE ch.ISACTIVE = 'Y' "
                        + "   AND ch.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ch.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'LETTER' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'L' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ai.APPOINTMENTSINFO_ID AS ID, ai.AD_TABLE_ID, ai.RECORD_ID, ai.CREATED, au.NAME AS FROMUSER, 'APPOINTMENT' AS TYPE, ai.SUBJECT AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM APPOINTMENTSINFO ai "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ai.CREATEDBY "
                        + "   WHERE (ai.AttendeeInfo IS NOT NULL OR ai.RefAppointmentsInfo_ID IS NULL) AND ai.ISACTIVE = 'Y' AND ai.ISTASK = 'N' "
                        + "   AND ai.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ai.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ai.APPOINTMENTSINFO_ID AS ID, ai.AD_TABLE_ID, ai.RECORD_ID, ai.CREATED, au.NAME AS FROMUSER, 'TASK' AS TYPE, ai.SUBJECT AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, ai.ISCLOSED AS ISTASKCLOSED "
                        + "   FROM APPOINTMENTSINFO ai "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ai.CREATEDBY "
                        + "   WHERE ai.ISACTIVE = 'Y' AND ai.ISTASK = 'Y' "
                        + "   AND ai.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ai.RECORD_ID = " + RecordId

                        + "   UNION"

                        + "   SELECT aa.AD_ATTACHMENT_ID AS ID, aa.AD_TABLE_ID, aa.RECORD_ID, aa.CREATED, au.NAME AS FROMUSER, 'ATTACHMENT' AS TYPE, CAST('Attachment' AS NVARCHAR2(50)) AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM AD_ATTACHMENT aa "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=aa.CREATEDBY "
                        + "   WHERE aa.ISACTIVE = 'Y' "
                        + "   AND aa.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND aa.RECORD_ID = " + RecordId;

            if (Env.IsModuleInstalled("VA048_"))
            {
                sql += "   UNION "

                + "   SELECT cd.VA048_CALLDETAILS_ID AS ID, cd.AD_TABLE_ID, cd.RECORD_ID, cd.CREATED, cd.VA048_FROM AS FROMUSER, 'CALL' AS TYPE, cd.VA048_TO AS SUBJECT, au.NAME, (CASE WHEN cd.VA048_RECORDINGS IS NOT NULL THEN 'Y' ELSE 'N' END) AS HASATTACHMENT, '' AS ISTASKCLOSED "
                + "   FROM VA048_CALLDETAILS cd "
                + "   JOIN AD_USER au ON au.AD_USER_ID=cd.CREATEDBY "
                + "   WHERE cd.VA048_TO IS NOT NULL AND cd.ISACTIVE = 'Y' "
                + "   AND cd.AD_TABLE_ID = " + _AD_Table_ID
                + "   AND cd.RECORD_ID = " + RecordId;
            }

            sql += " ) t"
                + " ORDER BY CREATED DESC";

            DataSet _dsHistory = VIS.DBase.DB.ExecuteDatasetPaging(sql, (Util.GetValueOfInt(CurrentPage) > 0 ? Util.GetValueOfInt(CurrentPage) : 1), 10);
            if (_dsHistory != null && _dsHistory.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dt in _dsHistory.Tables[0].Rows)
                {
                    LstHistory.Add(new HistoryDetails
                    {
                        ID = Util.GetValueOfInt(dt["ID"]),
                        ADTableID = Util.GetValueOfInt(dt["AD_TABLE_ID"]),
                        Created = DateTime.SpecifyKind(Convert.ToDateTime(dt["CREATED"]), DateTimeKind.Utc),
                        FromUser = Util.GetValueOfString(dt["FROMUSER"]),
                        Type = Util.GetValueOfString(dt["TYPE"]),
                        Subject = Util.GetValueOfString(dt["SUBJECT"]),
                        Name = (Util.GetValueOfBool(Util.GetValueOfString(dt["HASATTACHMENT"]).Equals("Y")) ? "<img src='./Areas/VIS/Images/email-attachment.png' alt='Attachment'>" : string.Empty) + "</br></br><span style='font-size:smaller;'>By: " + Util.GetValueOfString(dt["NAME"]) + "</span>",
                        UserName = Util.GetValueOfString(dt["NAME"]),
                        //CreatedDateTime = "<span style='font-weight:bold' >" + Util.GetValueOfString(dt["CREATEDDATETIME"]) + "</span></br></br><span style='font-size:smaller;'>" + Util.GetValueOfString(dt["SUBJECT"]) + "</span>",
                        CreatedDateTime = Util.GetValueOfString(dt["CREATEDDATETIME"]),
                        RecordId = Util.GetValueOfInt(dt["RECORD_ID"]),
                        HasAttachment = Util.GetValueOfBool(Util.GetValueOfString(dt["HASATTACHMENT"]).Equals("Y")),
                        IsTaskClosed = Util.GetValueOfBool(Util.GetValueOfString(dt["ISTASKCLOSED"]).Equals("Y"))
                    });
                }
            }

            return LstHistory;
        }

        /// <summary>
        /// Getting History Record count by passing parameters
        /// </summary>       
        /// <param name="RecordId">Record ID</param> 
        /// <param name="_AD_Table_ID">Table ID</param>
        /// <returns>History Record count</returns>
        public int GetHistoryRecordsCount(int RecordId, int _AD_Table_ID)
        {
            string sql = string.Empty;

            sql = @"SELECT COUNT(1) FROM "
                        + " ( "
                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'EMAIL' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'M' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'INBOX' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'I' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ch.CM_CHAT_ID AS ID, ch.AD_TABLE_ID, ch.RECORD_ID, ch.CREATED, au.NAME AS FROMUSER, 'CHAT' AS TYPE, CAST('Chat' AS NVARCHAR2(50)) AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM CM_CHAT ch "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ch.CREATEDBY "
                        + "   WHERE ch.ISACTIVE = 'Y' "
                        + "   AND ch.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ch.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ma.MAILATTACHMENT1_ID AS ID, ma.AD_TABLE_ID, ma.RECORD_ID, ma.CREATED, ma.MAILADDRESSFROM AS FROMUSER, 'LETTER' AS TYPE, ma.TITLE AS SUBJECT, au.NAME, ma.ISATTACHMENT AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM MAILATTACHMENT1 ma "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ma.CREATEDBY "
                        + "   WHERE ma.ISACTIVE = 'Y' AND ma.ATTACHMENTTYPE = 'L' "
                        + "   AND ma.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ma.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ai.APPOINTMENTSINFO_ID AS ID, ai.AD_TABLE_ID, ai.RECORD_ID, ai.CREATED, au.NAME AS FROMUSER, 'APPOINTMENT' AS TYPE, ai.SUBJECT AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM APPOINTMENTSINFO ai "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ai.CREATEDBY "
                        + "   WHERE (ai.AttendeeInfo IS NOT NULL OR ai.RefAppointmentsInfo_ID IS NULL) AND ai.ISACTIVE = 'Y' AND ai.ISTASK = 'N' "
                        + "   AND ai.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ai.RECORD_ID = " + RecordId

                        + "   UNION "

                        + "   SELECT ai.APPOINTMENTSINFO_ID AS ID, ai.AD_TABLE_ID, ai.RECORD_ID, ai.CREATED, au.NAME AS FROMUSER, 'TASK' AS TYPE, ai.SUBJECT AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM APPOINTMENTSINFO ai "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=ai.CREATEDBY "
                        + "   WHERE ai.ISACTIVE = 'Y' AND ai.ISTASK = 'Y' "
                        + "   AND ai.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND ai.RECORD_ID = " + RecordId

                        + "   UNION"

                        + "   SELECT aa.AD_ATTACHMENT_ID AS ID, aa.AD_TABLE_ID, aa.RECORD_ID, aa.CREATED, au.NAME AS FROMUSER, 'ATTACHMENT' AS TYPE, CAST('Attachment' AS NVARCHAR2(50)) AS SUBJECT, au.NAME, 'N' AS HASATTACHMENT, '' AS ISTASKCLOSED "
                        + "   FROM AD_ATTACHMENT aa "
                        + "   JOIN AD_USER au ON au.AD_USER_ID=aa.CREATEDBY "
                        + "   WHERE aa.ISACTIVE = 'Y' "
                        + "   AND aa.AD_TABLE_ID = " + _AD_Table_ID
                        + "   AND aa.RECORD_ID = " + RecordId;

            if (Env.IsModuleInstalled("VA048_"))
            {
                sql += "   UNION "

                 + "   SELECT cd.VA048_CALLDETAILS_ID AS ID, cd.AD_TABLE_ID, cd.RECORD_ID, cd.CREATED, cd.VA048_FROM AS FROMUSER, 'CALL' AS TYPE, cd.VA048_TO AS SUBJECT, au.NAME, (CASE WHEN cd.VA048_RECORDINGS IS NOT NULL THEN 'Y' ELSE 'N' END) AS HASATTACHMENT, '' AS ISTASKCLOSED "
                 + "   FROM VA048_CALLDETAILS cd "
                 + "   JOIN AD_USER au ON au.AD_USER_ID=cd.CREATEDBY "
                 + "   WHERE cd.VA048_TO IS NOT NULL AND cd.ISACTIVE = 'Y' "
                 + "   AND cd.AD_TABLE_ID = " + _AD_Table_ID
                 + "   AND cd.RECORD_ID = " + RecordId;
            }

            sql += " ) t";

            return Util.GetValueOfInt(DB.ExecuteScalar(sql));
        }

        /// <summary>
        /// when user click on sent mail record from histort form's Grid, then fetch its information like attachment, to , bccc etc...
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="ctx">Context</param>
        /// <returns>Email Data</returns>
        public MailDetails GetMailDetails(int ID, Ctx ctx)
        {
            MailDetails info = new MailDetails();

            string strSql = "SELECT MAILADDRESSFROM, MAILADDRESS,TITLE,MailAddressBcc,MailAddressCc,CREATED," +
                          "ISATTACHMENT, MAILATTACHMENT1_ID, AD_CLIENT_ID, AD_ORG_ID, AD_TABLE_ID, RECORD_ID," +
                          " CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg FROM mailattachment1 " +
                          " WHERE mailattachment1_ID=" + ID;

            using (DataSet ds = DB.ExecuteDataset(strSql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                    info.To = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddress"]);
                    info.From = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressfrom"]);
                    info.Date = DateTime.SpecifyKind(Convert.ToDateTime(ds.Tables[0].Rows[0]["created"]), DateTimeKind.Utc);
                    info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                    info.Bcc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressbcc"]);
                    info.Cc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddresscc"]);
                    info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ad_table_id"]);
                    info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                    info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                    info.IsMail = true;
                };
            }

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos
                {
                    Name = oMAttachEntry.GetName(),
                    ID = ID
                };
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on letter record from histort form's Grid, then fetch its information like attachment, subject etc...
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="ctx">Context</param>
        /// <returns>Letter Data</returns>
        public MailDetails GetLetterDetails(int ID, Ctx ctx)
        {
            MailDetails info = new MailDetails();

            string strSql = "SELECT TITLE ,CREATED" +
                 ", ISATTACHMENT, MAILATTACHMENT1_ID, AD_CLIENT_ID, AD_ORG_ID, AD_TABLE_ID, RECORD_ID," +
                 "CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg FROM mailattachment1 WHERE mailattachment1_ID=" + ID;

            using (DataSet ds = DB.ExecuteDataset(strSql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                    info.Date = DateTime.SpecifyKind(Convert.ToDateTime(ds.Tables[0].Rows[0]["created"]), DateTimeKind.Utc);
                    info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                    //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                    info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ad_table_id"]);
                    info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                    info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                    info.IsMail = true;
                };
            }
            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos
                {
                    Name = oMAttachEntry.GetName(),
                    ID = ID
                };
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on history record, then fetch its information like details, attachment etc...
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="ctx">Context</param>
        /// <returns>Call Data</returns>
        public CallInfo GetCallDetails(Ctx ctx, int ID)
        {
            CallInfo info = new CallInfo();

            string fullNameColumn = string.Empty;
            int getfullnamecolumn = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(1) FROM AD_Column WHERE AD_Table_ID = 114 AND ColumnName = 'FullName'", null, null));

            if (getfullnamecolumn > 0)
                fullNameColumn = "CASE WHEN ad.FullName IS NULL THEN ad.Name || ' ' || ad.LastName ELSE ad.FullName END ";
            else
                fullNameColumn = "ad.Name || ' ' || ad.LastName ";

            string strSql = @"SELECT cd.VA048_IsConference, NVL(cd.VA048_From, '-') as VA048_From, " +
                " NVL(cd.VA048_To, '-') as VA048_To, cd.VA048_Duration, cd.VA048_Price, " +
                "NVL(cd.VA048_Price_Unit, '-') as VA048_Price_Unit, NVL(cd.VA048_Status, '-') as VA048_Status," +
                " cd.AD_Table_ID, cd.Record_ID, cd.VA048_CallNotes, cd.Created, " +
                fullNameColumn + @" AS VA048_FullName FROM VA048_CallDetails cd " +
                " LEFT JOIN AD_User ad on cd.CreatedBy = ad.AD_User_ID " +
                " WHERE cd.IsActive = 'Y' and cd.VA048_CallDetails_ID = " + ID;

            using (DataSet ds = DB.ExecuteDataset(strSql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string attachemntsql = @"SELECT att.AD_Attachment_ID, att.FileLocation, " + "atl.AD_AttachmentLine_ID, atl.FileName, atl.FileType FROM AD_Attachment att " +
                      " LEFT JOIN AD_AttachmentLine atl on att.AD_Attachment_ID = atl.AD_Attachment_ID " +
                      " WHERE att.AD_table_ID = " + MTable.Get_Table_ID("VA048_CallDetails") +
                      @" and att.Record_ID = " + ID;

                    using (DataSet dsatt = DB.ExecuteDataset(attachemntsql))
                    {
                        if (dsatt != null && dsatt.Tables.Count > 0 && dsatt.Tables[0].Rows.Count > 0)
                        {
                            info.Attach = new List<AttachmentInfos>();
                            AttachmentInfos ai = new AttachmentInfos();
                            for (int i = 0; i < dsatt.Tables[0].Rows.Count; i++)
                            {
                                ai.Name = Util.GetValueOfString(dsatt.Tables[0].Rows[i]["FileName"]);
                                ai.ID = Util.GetValueOfInt(dsatt.Tables[0].Rows[i]["AD_AttachmentLine_ID"]);
                                ai.AttID = Util.GetValueOfInt(dsatt.Tables[0].Rows[i]["AD_Attachment_ID"]);
                                info.Attach.Add(ai);
                            }
                        }
                    }
                    info.VA048_IsConference = Util.GetValueOfChar(ds.Tables[0].Rows[0]["VA048_IsConference"].ToString());
                    info.VA048_From = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_From"]);
                    info.VA048_To = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_To"]);
                    info.VA048_Duration = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Duration"]);
                    info.VA048_Price = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["VA048_Price"].ToString().Replace("-", ""));
                    info.VA048_Price_Unit = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Price_Unit"]);
                    info.VA048_Status = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Status"]);
                    info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                    info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Record_ID"]);
                    info.VA048_CallNotes = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_CallNotes"]);
                    info.Created = DateTime.SpecifyKind(Convert.ToDateTime(ds.Tables[0].Rows[0]["Created"]), DateTimeKind.Utc);
                    info.VA048_FullName = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_FullName"]);
                };
            }
            return info;
        }

        /// <summary>
        /// when user click on history record, then fetch its information like details, attachment etc...
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="appointmentId">Appointment ID</param>
        /// <returns>Appointment Data</returns>
        public Dictionary<string, object> GetAppointmentDetails(Ctx ctx, int appointmentId)
        {
            string _sql = "SELECT AI.AppointmentsInfo_ID, AI.Subject,AI.Location,AI.Description,AI.AD_Table_ID, AI.Record_ID, " +
                " ( " +
                " CASE Label " +
                "   WHEN 1   " +
                "  THEN  '" + Msg.GetMsg(ctx, "Important", true) + "' " +
                "   WHEN 2 " +
                "   THEN '" + Msg.GetMsg(ctx, "Business", true) + "' " +
                "   WHEN 3 " +
                "   THEN   '" + Msg.GetMsg(ctx, "Personal", true) + "' " +
                "   WHEN 4 " +
                "   THEN '" + Msg.GetMsg(ctx, "Vacation", true) + "' " +
                "   WHEN 5 " +
                "   THEN '" + Msg.GetMsg(ctx, "MustAttend", true) + "' " +
                "   WHEN 6 " +
                "   THEN '" + Msg.GetMsg(ctx, "TravelRequired", true) + "' " +
                "   WHEN 7 " +
                "   THEN '" + Msg.GetMsg(ctx, "NeedsPreparation", true) + "' " +
                "   WHEN 8 " +
                "   THEN '" + Msg.GetMsg(ctx, "BirthDay", true) + "' " +
                "   WHEN 9 " +
                "   THEN '" + Msg.GetMsg(ctx, "Anniversary", true) + "' " +
                "   WHEN 10 " +
                "   Then '" + Msg.GetMsg(ctx, "PhoneCall", true) + "' " +
                "   ELSE '" + Msg.GetMsg(ctx, "None", true) + "' " +
                " END ) AS label , " +
                " AI.StartDate , AI.EndDate, " +
                " ( " +
                " CASE AI.AllDay " +
                "   WHEN 'Y' " +
                "       THEN '" + Msg.GetMsg(ctx, "Yes", true) + "' " +
                "       ELSE '" + Msg.GetMsg(ctx, "No", true) + "' " +
                " END) AS Allday , " +
                " ( " +
                " CASE AI.Status " +
                "   WHEN 1 " +
                "   THEN '" + Msg.GetMsg(ctx, "Tentative", true) + "'  " +
                "   WHEN 2 " +
                "   THEN '" + Msg.GetMsg(ctx, "Busy", true) + "' " +
                "   WHEN 3 " +
                "   THEN '" + Msg.GetMsg(ctx, "OutOfOffice", true) + "' " +
                "   Else " +
                "    '" + Msg.GetMsg(ctx, "Free", true) + "' " +
                "    End) as Status, " +
                "     AI.ReminderInfo, COALESCE(ai.AttendeeInfo, CAST(ai.Ad_User_Id AS VARCHAR(10))) AS AttendeeInfo, " +
                "     AI.RecurrenceInfo , " +
                "     (  " +
                "     CASE AI.IsPrivate " +
                "       WHEN 'Y' " +
                "       THEN '" + Msg.GetMsg(ctx, "Yes") + "' " +
                "       ELSE '" + Msg.GetMsg(ctx, "No") + "' " +
                "     END) AS IsPrivate,AI.comments ,ac.Name as caname" +
                "   FROM AppointmentsInfo ai LEFT OUTER JOIN appointmentcategory AC " +
                " ON AI.appointmentcategory_id=ac.appointmentcategory_id WHERE ai.ISACTIVE='Y' AND AI.AppointmentsInfo_ID=" + appointmentId;

            Dictionary<string, object> obj = null;
            DataSet ds = DB.ExecuteDataset(_sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["AppointmentsInfo_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AppointmentsInfo_ID"]);
                obj["Subject"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Subject"]);
                obj["Location"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Location"]);
                obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                obj["AD_Table_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Table_ID"]);
                obj["Record_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Record_ID"]);
                obj["label"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["label"]);
                //obj["StartDate"] = (ds.Tables[0].Rows[0]["StartDate"]);
                //obj["EndDate"] = (ds.Tables[0].Rows[0]["EndDate"]);
                DateTime _StartDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]);
                DateTime _format = DateTime.SpecifyKind(new DateTime(_StartDate.Year, _StartDate.Month, _StartDate.Day, _StartDate.Hour, _StartDate.Minute, _StartDate.Second), DateTimeKind.Utc);
                _StartDate = _format;
                obj["StartDate"] = _StartDate;

                DateTime _EndDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]);
                _format = DateTime.SpecifyKind(new DateTime(_EndDate.Year, _EndDate.Month, _EndDate.Day, _EndDate.Hour, _EndDate.Minute, _EndDate.Second), DateTimeKind.Utc);
                _EndDate = _format;
                obj["EndDate"] = _EndDate;
                obj["Allday"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Allday"]);
                obj["Status"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Status"]);
                obj["ReminderInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["ReminderInfo"]);
                obj["AttendeeInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["AttendeeInfo"]);
                obj["RecurrenceInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["RecurrenceInfo"]);
                obj["IsPrivate"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsPrivate"]);
                obj["comments"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                obj["caname"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["caname"]);
            }
            return obj;
        }

        /// <summary>
        /// Get Task Details
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>Task Data</returns>
        public TaskDetails GetTaskDetails(int ID)
        {
            string sql = " SELECT AI.AppointmentsInfo_ID, AI.Subject,AI.Location,AI.Description,AI.AD_Table_ID, AI.Record_ID, " +
                          " AI.StartDate, AI.EndDate, AI.TASKSTATUS, AI.RESULT, " +
                          " (SELECT Name FROM AD_USER WHERE AD_USER_ID = AI.ad_user_id) AS AssignedTo, " +
                          " AI.ReminderInfo, COALESCE(ai.AttendeeInfo, CAST(ai.ad_user_id AS VARCHAR(10))) AS AttendeeInfo,  AI.RecurrenceInfo , " +
                          " AI.PRIORITYKEY, AI.comments ,ac.Name as CategoryName, ai.ISCLOSED AS ISTASKCLOSED" +
                          " FROM AppointmentsInfo ai LEFT OUTER JOIN appointmentcategory AC " +
                          " ON AI.appointmentcategory_id=ac.appointmentcategory_id " +
                          " WHERE ai.ISACTIVE='Y' AND ISTASK='Y' AND AI.AppointmentsInfo_ID=" + Util.GetValueOfInt(ID);

            TaskDetails lstTask = new TaskDetails();

            using (DataSet _dsTask = DB.ExecuteDataset(sql))
            {
                if (_dsTask != null && _dsTask.Tables[0].Rows.Count > 0)
                {
                    lstTask.AppointmentsInfo_ID = Util.GetValueOfInt(_dsTask.Tables[0].Rows[0]["AppointmentsInfo_ID"]);
                    lstTask.Subject = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["Subject"]);
                    lstTask.Location = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["Location"]);
                    lstTask.Description = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["Description"]);
                    lstTask.AD_Table_ID = Util.GetValueOfInt(_dsTask.Tables[0].Rows[0]["AD_Table_ID"]);
                    lstTask.Record_ID = Util.GetValueOfInt(_dsTask.Tables[0].Rows[0]["Record_ID"]);
                    //lstTask.StartDate = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["StartDate"]);
                    //lstTask.EndDate = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["EndDate"]);
                    DateTime _StartDate = Convert.ToDateTime(_dsTask.Tables[0].Rows[0]["StartDate"]);
                    DateTime _format = DateTime.SpecifyKind(new DateTime(_StartDate.Year, _StartDate.Month, _StartDate.Day, _StartDate.Hour, _StartDate.Minute, _StartDate.Second), DateTimeKind.Utc);
                    _StartDate = _format;
                    lstTask.StartDate = _StartDate;

                    DateTime _EndDate = Convert.ToDateTime(_dsTask.Tables[0].Rows[0]["EndDate"]);
                    _format = DateTime.SpecifyKind(new DateTime(_EndDate.Year, _EndDate.Month, _EndDate.Day, _EndDate.Hour, _EndDate.Minute, _EndDate.Second), DateTimeKind.Utc);
                    _EndDate = _format;
                    lstTask.EndDate = _EndDate;

                    lstTask.TaskStatus = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["TASKSTATUS"]);
                    lstTask.Result = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["RESULT"]);
                    lstTask.AttendeeInfo = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["AttendeeInfo"]);
                    lstTask.RecurrenceInfo = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["RecurrenceInfo"]);
                    lstTask.PriorityKey = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["PRIORITYKEY"]);
                    lstTask.Comments = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["comments"]);
                    lstTask.CategoryName = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["CategoryName"]);
                    lstTask.IsTaskClosed = Util.GetValueOfBool(Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["ISTASKCLOSED"]).Equals("Y"));
                    lstTask.AssignedTo = Util.GetValueOfString(_dsTask.Tables[0].Rows[0]["AssignedTo"]);
                }
            }

            return lstTask;
        }

        /// <summary>
        /// Get Attachment Details
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>Attachment Data</returns>
        public List<AttachmentDetails> GetAttachmentDetails(int ID)
        {
            string sql = " SELECT att.AD_Attachment_ID, att.FileLocation, atl.AD_AttachmentLine_ID, atl.FileName, atl.FileType, (atl.FileSize/1024) AS FileSize, att.AD_Table_ID, att.Record_ID, atl.Created AS CreatedOn, au.Name AS CreatedBy " +
                          " FROM AD_Attachment att LEFT JOIN AD_AttachmentLine atl on att.AD_Attachment_ID = atl.AD_Attachment_ID " +
                          " LEFT JOIN AD_USER au ON au.AD_USER_ID = atl.CREATEDBY " +
                          " WHERE att.ISACTIVE='Y' AND att.AD_Attachment_ID=" + Util.GetValueOfInt(ID) +
                          " ORDER BY atl.CREATED DESC";

            List<AttachmentDetails> lstAttach = new List<AttachmentDetails>();

            using (DataSet _dsAttach = DB.ExecuteDataset(sql))
            {
                if (_dsAttach != null && _dsAttach.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in _dsAttach.Tables[0].Rows)
                    {
                        lstAttach.Add(new AttachmentDetails
                        {
                            ID = Util.GetValueOfInt(row["AD_AttachmentLine_ID"]),
                            AD_Attachment_ID = Util.GetValueOfInt(row["AD_Attachment_ID"]),
                            AD_AttachmentLine_ID = Util.GetValueOfInt(row["AD_AttachmentLine_ID"]),
                            FileName = Util.GetValueOfString(row["FileName"]),
                            FileType = Util.GetValueOfString(row["FileType"]),
                            FileSize = Util.GetValueOfInt(row["FileSize"]),
                            FileLocation = Util.GetValueOfString(row["FileLocation"]),
                            CreatedOn = DateTime.SpecifyKind(Convert.ToDateTime(row["CreatedOn"]), DateTimeKind.Utc),
                            CreatedBy = Util.GetValueOfString(row["CreatedBy"]),
                            AD_Table_ID = Util.GetValueOfInt(row["AD_Table_ID"]),
                            Record_ID = Util.GetValueOfInt(row["Record_ID"])
                        });
                    }
                }
            }

            return lstAttach;
        }

        /// <summary>
        /// Get Chat Details
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="ID">ID</param>
        /// <returns>Chat Data</returns>
        public List<ChatDetails> GetChatDetails(Ctx ctx, int ID)
        {
            string sql = @"SELECT ce.CM_Chat_Id, ce.CM_ChatEntry_Id, ce.CharacterData, au.Name AS NAME,ce.Created, ai.AD_Image_Id, ai.BinaryData AS UsrImg, CH.CreatedBy, ch.Record_ID 
                        FROM CM_Chat ch JOIN CM_ChatEntry ce ON ch.CM_Chat_Id= ce.CM_Chat_Id
                        JOIN AD_User au ON au.AD_User_Id= ce.CreatedBy LEFT JOIN AD_Image ai ON ai.AD_Image_Id=au.AD_Image_Id
                        WHERE ch.ISACTIVE='Y' AND ch.CM_Chat_Id = " + Util.GetValueOfInt(ID) + " ORDER BY ce.Created";

            List<ChatDetails> lstChat = new List<ChatDetails>();
            DateTime _format = new DateTime(); ;
            string imgfll = string.Empty;

            using (DataSet _dsChat = DB.ExecuteDataset(sql))
            {
                if (_dsChat != null && _dsChat.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in _dsChat.Tables[0].Rows)
                    {
                        if (row["Created"] != null && Util.GetValueOfString(row["Created"]) != "")
                        {
                            DateTime _createdDate = Convert.ToDateTime(row["Created"].ToString());
                            _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                        }

                        int uimgId = Util.GetValueOfInt(row["AD_Image_Id"]);
                        imgfll = string.Empty;
                        if (uimgId > 0)
                        {
                            MImage mimg = new MImage(ctx, uimgId, null);
                            imgfll = mimg.GetThumbnailURL(46, 46);
                        }

                        if (Util.GetValueOfString(imgfll) != "NoRecordFound" && Util.GetValueOfString(imgfll) != "FileDoesn'tExist" && !string.IsNullOrEmpty(imgfll))
                            imgfll = @"<img  class='userAvatar-Feeds'  src='" + imgfll + "'>"; //"?" + DateTime.Now + 
                        else
                            imgfll = @"<i class='fa fa-user'></i>"; //<img src='' alt='profile image'>";

                        lstChat.Add(new ChatDetails
                        {
                            ID = Util.GetValueOfInt(row["CM_ChatEntry_Id"]),
                            ChatID = Util.GetValueOfInt(row["CM_Chat_Id"]),
                            EntryID = Util.GetValueOfInt(row["CM_ChatEntry_Id"]),
                            AD_Image_ID = Util.GetValueOfInt(row["Ad_Image_Id"]),
                            CreatedBy = Util.GetValueOfInt(row["CreatedBy"]),
                            UserName = Util.GetValueOfString(row["NAME"]),
                            UserImage = Util.GetValueOfString(imgfll),
                            CharacterData = Util.GetValueOfString(row["CharacterData"]),
                            Created = _format,
                            Record_ID = Util.GetValueOfInt(row["Record_ID"])
                        });
                    }
                }
            }

            return lstChat;
        }

        /// <summary>
        /// history attachment download
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="AttID">Attachment ID</param>
        /// <param name="ID">File ID</param>
        /// <param name="Name">File Name</param>
        /// <returns>Attachment path</returns>
        public string GetHistoryAttachmentName(Ctx ctx, int AttID, int ID, string Name)
        {
            string fName = string.Empty;

            MAttachment att = new MAttachment(ctx, AttID, null);
            fName = att.GetFile(ID);

            if (!fName.StartsWith("ERROR:"))
                fName = fName + "/" + Name;
            else
                fName = string.Empty;

            return Util.GetValueOfString(fName);
        }

        /// <summary>
        /// Download Attachment
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetAttachmentName(Ctx ctx, int ID, string Name, string TempDownloadPath)
        {
            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            string path = string.Empty;
            var fName = string.Empty;

            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                if (Name.ToUpper() == oMAttachEntry.GetName().ToUpper())
                {
                    fName = DateTime.Now.Ticks.ToString() + Name;
                    path = Path.Combine(TempDownloadPath, fName);
                    byte[] bytes = oMAttachEntry.GetData();

                    using (FileStream fs = new FileStream(path, FileMode.Append, System.IO.FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    break;
                }
            }

            return Util.GetValueOfString(fName);
        }

        /// <summary>
        /// GetAllAttachmentsZipName
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetAllAttachmentsZipName(Ctx ctx, int ID, string Name, string TempDownloadPath)
        {
            string path = string.Empty;
            string fName = string.Empty;
            string zipfileName = string.Empty;
            string tempDownPath = string.Empty;

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            MAttachmentEntry[] _mAttachEntries;

            if (_mAttachment != null)
            {
                _mAttachEntries = _mAttachment.GetEntries();
                if (_mAttachEntries.Length > 0)
                {
                    tempDownPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";
                    zipfileName = ID + "_" + System.DateTime.Now.Ticks + ".zip";

                    using (ZipFile zip = new ZipFile())
                    {
                        foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
                        {
                            fName = DateTime.Now.Ticks.ToString() + oMAttachEntry.GetName();
                            path = Path.Combine(TempDownloadPath, fName);
                            byte[] bytes = oMAttachEntry.GetData();

                            using (FileStream fs = new FileStream(path, FileMode.Append, System.IO.FileAccess.Write))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                            }
                            zip.AddFile(path, "DownloadAll");
                        }

                        // check if folder exists on hosting path (TempDownload)
                        if (!Directory.Exists(tempDownPath))
                            Directory.CreateDirectory(tempDownPath);
                        if (Directory.Exists(tempDownPath))
                            zip.Save(tempDownPath + "\\" + zipfileName);
                    }
                }
            }
            return Util.GetValueOfString(zipfileName);
        }
    }

    // Declare the History Properties
    public class GetHistoryDetails
    {
        public string DocumentNo { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string CmpName { get; set; }
        public string EmailAddress { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string RStatus { get; set; }
        public int RecordId { get; set; }
        public string SalesRep { get; set; }
    }

    /// <summary>
    /// Declare the History Properties
    /// </summary>
    public class HistoryDetails
    {
        public int ID { get; set; }
        public int ADTableID { get; set; }
        public string DocumentNo { get; set; }
        public DateTime Created { get; set; }
        public string FromUser { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string CreatedDateTime { get; set; }
        public int RecordId { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsTaskClosed { get; set; }
    }

    /// <summary>
    /// Email Details
    /// </summary>
    public class EmailDetails
    {
        public int ID { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public string CreatedDateTime { get; set; }
        public int RecordId { get; set; }
        public int NoOfAttachments { get; set; }
        public string Attachments { get; set; }
    }

    public class MailDetails
    {
        public string Title { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public string Detail { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public string Comments { get; set; }
        public bool IsMail { get; set; }
        public bool IsLetter { get; set; }
        public List<AttachmentInfos> Attach { get; set; }
        public int AD_Table_ID { get; set; }
        public int Record_ID { get; set; }
        public int ID { get; set; }
    }
    /// <summary>
    /// Call Details
    /// </summary>
    public class CallDetails
    {
        public int ID { get; set; }
        public string FromName { get; set; }
        public string FromPhone { get; set; }
        public string ToPhone { get; set; }
        public string Duration { get; set; }
        public string CreatedDateTime { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public string PriceUnit { get; set; }
        public string Category { get; set; }
        public string Attachments { get; set; }
        public int RecordId { get; set; }
    }

    /// <summary>
    /// Task Details
    /// </summary>
    public class TaskDetails
    {
        public int ID { get; set; }
        public int AppointmentsInfo_ID { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int AD_Table_ID { get; set; }
        public int Record_ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TaskStatus { get; set; }
        public string Result { get; set; }
        public string AttendeeInfo { get; set; }
        public string RecurrenceInfo { get; set; }
        public string PriorityKey { get; set; }
        public string Comments { get; set; }
        public string CategoryName { get; set; }
        public bool IsTaskClosed { get; set; }
        public string AssignedTo { get; set; }
    }

    /// <summary>
    /// Attachment Details
    /// </summary>
    public class AttachmentDetails
    {
        public int ID { get; set; }
        public int AD_Attachment_ID { get; set; }
        public int AD_AttachmentLine_ID { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public int AD_Table_ID { get; set; }
        public int Record_ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// Chat Details
    /// </summary>
    public class ChatDetails
    {
        public int ID { get; set; }
        public int ChatID { get; set; }
        public int EntryID { get; set; }
        public int AD_Image_ID { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string CharacterData { get; set; }
        public DateTime Created { get; set; }
        public int Record_ID { get; set; }
    }
}

