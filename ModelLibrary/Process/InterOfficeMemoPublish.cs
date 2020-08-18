/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : InterOfficeMemoPublish
    * Purpose        : Inter office Memo Publish Process to send the Email notification to all team member as per selected team in process parameter.
    * Class Used     : ProcessEngine.SvrProcess    
    * Sukhwinder on 28-Dec-2017
******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.ProcessEngine;
using System.IO;


namespace VAdvantage.Process
{
    public class InterOfficeMemoPublish : ProcessEngine.SvrProcess
    {
        private string C_Team_IDs = "";
        private int _OfficeMemoID = 0;
        StringBuilder _Sql = new StringBuilder();
        
        VAdvantage.Utility.EMail sendmail = null;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null && para[i].GetParameter_To() == null)
                {
                    ;
                }
                else if (name.Equals("C_Team_ID"))
                {
                    C_Team_IDs = (String)para[i].GetParameter();
                }                                
            }

            _OfficeMemoID = GetRecord_ID();
        }

        protected override string DoIt()
        {
            if (string.IsNullOrEmpty(C_Team_IDs))
            {
                return Msg.GetMsg(GetCtx(), "VIS_SelectTeamFirst");
            }

            try
            {
                _Sql.Append(" SELECT Distinct AU.EMAIL, AU.AD_USER_ID, AU.NOTIFICATIONTYPE   "
                          + "     FROM C_TEAM CT                                             "
                          + "     INNER JOIN C_TEAMMEMBER CTM                                "
                          + "     INNER JOIN AD_USER AU                                      "
                          + "     ON AU.AD_USER_ID   = CTM.AD_USER_ID                        "
                          + "     ON CTM.C_TEAM_ID   = CT.C_TEAM_ID                          "
                          + "     WHERE CTM.ISACTIVE = 'Y'                                   "
                          + "     AND CT.ISACTIVE    = 'Y'                                   "
                          + "     AND AU.ISACTIVE    = 'Y'                                   "
                          + "     AND CT.C_team_ID  IN (" + C_Team_IDs + ")");
                         

               // string EmailIDs = Util.GetValueOfString(DB.ExecuteScalar(_Sql.ToString(), null, Get_TrxName()));

                DataSet ds1 = DB.ExecuteDataset(_Sql.ToString(), null, Get_TrxName());
                if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
                {
                    _Sql.Clear();

                    _Sql.Append("SELECT AD_CLIENT_ID ,"
                                + "   AD_ORG_ID ,      "
                                + "   AD_CLIENT_ID ,   "
                                + "   MESSAGE ,        "
                                + "   NAME ,           "
                                + "   PROCESSED ,      "
                                + "   PROCESSING ,     "
                                + "   PUBLISH ,        "
                                + "   PUBLISHDATE ,    "
                                + "   PUBLISHTIME ,    "
                                + "   SUBJECT          "
                                + " FROM OFFICEMEMO WHERE OFFICEMEMO_ID = " + _OfficeMemoID);

                    DataSet ds = DataBase.DB.ExecuteDataset(_Sql.ToString(), null, Get_TrxName());

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return SendEmailWithAttachment(Util.GetValueOfString(ds.Tables[0].Rows[0]["SUBJECT"]), 
                            Util.GetValueOfString(ds.Tables[0].Rows[0]["MESSAGE"]), 
                            ds1, Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_ORG_ID"]),
                            Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_CLIENT_ID"]));
                    }
                    else
                    {
                        return Msg.GetMsg(GetCtx(), "VIS_NoRecordFound");
                    }
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "VIS_NoEmailsExists");
                }
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }            
        }

        private string SendEmailWithAttachment(string Subject, string Message, DataSet ds1, int AD_Org_ID, int Ad_Client_ID)
        {
            string Check = "";
            int count = 0;
            string res = "";

            List<string> EmailLst = new List<string>(); //!string.IsNullOrEmpty(EmailIDs) ? EmailIDs.Split(',').ToList() : null;

            int UpdatedBy = GetCtx().GetAD_User_ID();            
            sendmail = new VAdvantage.Utility.EMail(GetCtx(), "", "", "", "", "", "", true, false);
            string isConfigExist = sendmail.IsConfigurationExist(GetCtx());
            if (isConfigExist != "OK")
            {
                log.SaveError("Check email configuration", "");
                return Msg.GetMsg(GetCtx(), "VIS_CheckMailConfig");
            }

            if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    if (Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "A" ||
                        Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "B" ||
                        Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "E" ||
                        Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "L" )
                    {
                        sendmail.AddBcc(Util.GetValueOfString(ds1.Tables[0].Rows[i]["EMAIL"]));
                    }
                }

                sendmail.SetSubject(Subject);
                sendmail.SetMessageText(Message);

                MAttachment mAttach = GetFileAttachment(GetTable_ID(), GetRecord_ID(), GetCtx());
                string filePath = "";

                if (mAttach != null && mAttach.AD_Attachment_ID > 0)
                {
                    if (mAttach == null)
                        return "";
                    if (mAttach.IsFromHTML())
                    {
                        for (int i = 0; i < mAttach._lines.Count; i++)
                        {
                            filePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload");
                            filePath = System.IO.Path.Combine(filePath, mAttach.GetFile(mAttach._lines[i].Line_ID));
                            if (filePath.IndexOf("ERROR") > -1)
                            {
                                continue;
                            }
                            filePath = System.IO.Path.Combine(filePath, mAttach._lines[i].FileName);
                            sendmail.AddAttachment(new FileInfo(filePath));
                        }
                    }
                    else
                    {
                        foreach (MAttachmentEntry entry in mAttach.GetEntries())
                        {
                            sendmail.AddAttachment(entry.GetData(), entry.GetName());
                        }
                    }
                }

                Check = sendmail.Send();
                if (Check == "OK")
                {
                    count++;
                }

            }                                  
                
            if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    if (Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "B" ||
                        Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "C" ||
                        Util.GetValueOfString(ds1.Tables[0].Rows[i]["NOTIFICATIONTYPE"]) == "N")
                    {
                        SendNotice(Message, Util.GetValueOfInt(ds1.Tables[0].Rows[i]["AD_USER_ID"]), AD_Org_ID, Ad_Client_ID);
                    }
                }
            }   

            if (count > 0)
            {
                log.Fine("Email Sent Successfully");
                res = Msg.GetMsg(GetCtx(), "VIS_EmailSent");                   

                string sql = "UPDATE OFFICEMEMO SET PUBLISHDATE = " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + ", PUBLISHTIME = " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + ", PROCESSED = 'Y' WHERE OFFICEMEMO_ID = " + _OfficeMemoID;
                DB.ExecuteQuery(sql, null, null);
                                 
            }
            else
            {
                log.SaveError("Email Not Sent", "");
                res = Msg.GetMsg(GetCtx(), "VIS_EmailNotSent");                   
            }

            return res;
        }

        private void SendNotice(string message, int AD_User_ID, int AD_Org_ID, int Ad_Client_ID)
        {
            MNote note = new MNote(GetCtx(), "VIS_InterOfficeMemo", AD_User_ID, Ad_Client_ID, AD_Org_ID, null);            //  Reference
            note.SetReference(ToString());
            // Text
            note.SetTextMsg(message);
            note.Save();
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
    }
}
