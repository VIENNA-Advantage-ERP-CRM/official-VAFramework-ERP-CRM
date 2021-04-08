using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VIS.Models
{
    public class UserSetting
    {
        public string Msg { get; set; }
        public bool IsSaved { get; set; }
    }
    public class LoginData
    {
        public string Name { get; set; }
        public int RecKey { get; set; }
    }
    public class DefaultLoginData
    {
        public int VAF_Role_ID { get; set; }
        public int VAF_Client_ID { get; set; }
        public int VAF_Org_ID { get; set; }
        public int VAM_Warehouse_ID { get; set; }
    }
    public class UserPreferenceModel
    {
        public bool AutoCommitControl { get; set; }
        public bool AutoLoginControl { get; set; }
        public bool ShowAccountingTabControl { get; set; }
        public bool ShowAdvancedTabControl { get; set; }
        public bool ShowTranslationTabControl { get; set; }
        public bool StorePasswordControl { get; set; }
        public bool AutoNewRecordControl { get; set; }
        public bool CahceWindowControl { get; set; }
        public bool PreviewPrintControl { get; set; }
        public string PrinterControl { get; set; }
        public bool DictionaryMaintenenceControl { get; set; }
        public bool TraceFileControl { get; set; }
        public ListBox ListBoxControl { get; set; }

        public string ConnectionDetail { get; set; }
        public bool ErrorOnly { get; set; }
        public DateTime? Date { get; set; }
        public String[] Context { get; set; }
        //user setting property
        public bool SMS { get; set; }
        public bool Email { get; set; }
        public bool Notice { get; set; }
        public bool Fax { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPws { get; set; }


        public UserPreferenceModel GetUserSettings(Ctx ctx, int VAF_UserContact_Id)
        {
            MVAFUserContact user = new MVAFUserContact(ctx, VAF_UserContact_Id, null);
            UserPreferenceModel obj = new UserPreferenceModel();
            obj.EmailUserName = user.GetEMailUser();
            obj.EmailPws = user.GetEMailUserPW();
            var type = user.GetNotificationType();
            obj.SMS = user.IsSms();
            obj.Email = user.IsEmail();

            if (type == X_VAF_UserContact.NOTIFICATIONTYPE_NoticePlusFaxEMail)
            {
                obj.Notice = true;
                obj.Fax = true;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice)
            {
                obj.Email = true;
                obj.Notice = true;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusFaxEMail)
            {
                obj.Email = true;
                obj.Fax = true;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_None)
            {
                obj.Notice = false;
                obj.Fax = false;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_Notice)
            {
                obj.Notice = true;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_FaxEMail)
            {
                obj.Fax = true;
            }
            else if (type == X_VAF_UserContact.NOTIFICATIONTYPE_EMail)
            {
                obj.Email = true;
            }
            return obj;
        }


        /// <summary>
        /// save prefrence into database as well as in server context
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pref"></param>
        /// <returns></returns>
        public bool SavePrefrence(Ctx ctx, Dictionary<string, object> pref)
        {
            MVAFUserPrefInfo preference = null;
            MVAFUserContact user = MVAFUserContact.Get(ctx);
            preference = user.GetPreference();
            preference.SetIsAutoCommit(pref["IsAutoCommit"].Equals("Y") ? true : false);
            ctx.SetAutoCommit(pref["IsAutoCommit"].Equals("Y") ? true : false);
            preference.SetIsShowAcct(pref["IsShowAcct"].Equals("Y") ? true : false);
            ctx.SetContext("#ShowAcct", pref["IsShowAcct"].Equals("Y") ? true : false);
            //	Show Trl Tab
            preference.SetIsShowTrl(pref["IsShowTrl"].Equals("Y") ? true : false);
            ctx.SetContext("#ShowTrl", pref["IsShowTrl"].Equals("Y") ? true : false);
            //	Show Advanced Tab
            preference.SetIsShowAdvanced(pref["IsShowAdvanced"].Equals("Y") ? true : false);
            ctx.SetContext("#ShowAdvanced", pref["IsShowAdvanced"].Equals("Y") ? true : false);
            return preference.Save();
        }
        public UserSetting SaveChangePassword(Ctx ctx, int VAF_UserContact_ID, string currentPws, string newPws)
        {
            UserSetting obj = new UserSetting();
            obj.Msg = Msg.GetMsg(ctx, "RecordSaved");
            MVAFUserContact user = new MVAFUserContact(ctx, VAF_UserContact_ID, null);
            obj.IsSaved = false;
            if (currentPws.Length > 0 && newPws.Length > 0)
            {
                if (user.GetPassword().ToUpper() != currentPws.ToUpper())
                {
                    obj.Msg = Msg.GetMsg(ctx, "InCorrectPassword!");
                    obj.IsSaved = false;
                    return obj;
                }
                user.SetPassword(newPws);
            }
            if (user.Save())
            {
                obj.IsSaved = true;
            }
            return obj;
        }
        //save user settings
        public UserSetting SaveUserSettings(Ctx ctx, int VAF_UserContact_ID, string currentPws, string newPws, bool chkEmail, bool chkNotice,
           bool chkSMS, bool chkFax, string emailUserName, string emailPws, int VAF_Role_ID, int VAF_Client_ID, int VAF_Org_ID, int VAM_Warehouse_ID)
        {
            UserSetting obj = new UserSetting();
            obj.Msg = Msg.GetMsg(ctx, "RecordSaved");
            MVAFUserContact user = new MVAFUserContact(ctx, VAF_UserContact_ID, null);
            obj.IsSaved = false;
            //if (currentPws.Length > 0 && newPws.Length > 0)
            //{
            //    if (user.GetPassword().ToUpper() != currentPws.ToUpper())
            //    {
            //        obj.Msg = Msg.GetMsg(ctx, "InCorrectPassword!");
            //        obj.IsSaved = false;
            //        return obj;
            //    }
            //    user.SetPassword(newPws);
            //}

            //user.SetIsEmail(chkEmail);
            //user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMail);
            ////notice
            //user.SetIsSms(chkSMS);
            //user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_SMS);

            //if (chkNotice && chkFax)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_NoticePlusFaxEMail);
            //}
            //else if (chkEmail && chkNotice)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice);
            //}
            //else if (chkEmail && chkFax)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusFaxEMail);
            //}
            //else if (chkEmail && chkSMS && !chkNotice && !chkFax)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_None);
            //}
            //else if (chkNotice && chkSMS)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_Notice);
            //}
            //else if (chkFax && chkSMS)
            //{
            //    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_FaxEMail);
            //}

            user.SetIsEmail(chkEmail);
            user.SetIsSms(chkSMS);

            if (chkEmail)
            {
                user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMail);

                if (chkNotice)
                {
                    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice);
                }
                if (chkFax)
                {
                    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusFaxEMail);
                }
            }
            if (chkSMS)
            {
                user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_SMS);

                if (chkNotice)
                {
                    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_Notice);
                }
                if (chkFax)
                {
                    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_FaxEMail);
                }
            }
            if (chkFax)
            {
                user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_FaxEMail);
            }
            if (chkNotice)
            {
                user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_Notice);
                if (chkFax)
                {
                    user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_NoticePlusFaxEMail);
                }
            }

            if (!chkNotice && !chkFax)
            {
                user.SetNotificationType(X_VAF_UserContact.NOTIFICATIONTYPE_None);
            }

            //fax
            user.SetEMailUser(emailUserName);
            user.SetEMailUserPW(emailPws);
            if (user.Save())
            {
                obj.IsSaved = true;
            }

            try
            {
                //LoginSettings
                int VAF_LoginSetting_ID = Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar("SELECT VAF_LoginSetting_ID FROM VAF_LoginSetting WHERE IsActive='Y' AND VAF_UserContact_ID=" + VAF_UserContact_ID, null, null));
                StringBuilder sql = new StringBuilder("");
                if (VAF_LoginSetting_ID > 0)//UPdate
                {
                    sql.Append("UPDATE VAF_LoginSetting SET VAF_Role_ID=" + VAF_Role_ID + ",VAF_Client_ID=" + VAF_Client_ID + ",VAF_Org_ID=" + VAF_Org_ID + " ,VAM_Warehouse_ID=");
                    if (VAM_Warehouse_ID == 0)
                        sql.Append("NULL");
                    else
                        sql.Append(VAM_Warehouse_ID);

                    sql.Append(" WHERE VAF_LoginSetting_ID=" + VAF_LoginSetting_ID);

                }
                else//Insert
                {
                    VAF_LoginSetting_ID = MVAFRecordSeq.GetNextID(ctx.GetVAF_Client_ID(), "VAF_LoginSetting", null);
                    sql.Append("INSERT INTO VAF_LoginSetting (VAF_CLIENT_ID,VAF_LOGINSETTING_ID,VAF_ORG_ID,VAF_ROLE_ID,VAF_USERCONTACT_ID,CREATED,CREATEDBY,EXPORT_ID,VAM_Warehouse_ID,UPDATED,UPDATEDBY)");
                    sql.Append(" VALUES (" + VAF_Client_ID + "," + VAF_LoginSetting_ID + "," + VAF_Org_ID + "," + VAF_Role_ID + "," + VAF_UserContact_ID + ",");
                    sql.Append(GlobalVariable.TO_DATE(DateTime.Now, false) + "," + ctx.GetVAF_UserContact_ID() + ",NULL,");
                    if (VAM_Warehouse_ID == 0)
                        sql.Append("NULL");
                    else
                        sql.Append(VAM_Warehouse_ID);

                    sql.Append("," + GlobalVariable.TO_DATE(DateTime.Now, false) + "," + ctx.GetVAF_UserContact_ID() + ")");


                }


                int s = VAdvantage.DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
                if (s == -1)
                {
                    obj.IsSaved = false;
                }
            }
            catch
            {
            }


            return obj;
        }


        //**********Added By Sarab************
        public string Action(Ctx ctx, bool chkIsSyncTask, bool isTask, string authCode, bool isRemoveLink)
        {
            bool isSyncTask = chkIsSyncTask;

            string sql = @"Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where isActive='Y' and VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID() +
            //sql = MRole.GetDefault().AddAccessSQL(sql, "WS_GmailConfiguration", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            " and VAF_Client_ID=" + ctx.GetVAF_Client_ID();// +" and VAF_Org_ID=" + Envs.GetCtx().GetVAF_Org_ID();
            DataSet ds = DB.ExecuteDataset(sql, null);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                X_WSP_GmailConfiguration gConfigs = new X_WSP_GmailConfiguration(ctx, 0, null);
                if (isTask)
                {
                    gConfigs.SetWSP_IsSyncTaskBackGround(isSyncTask);
                    // gConfig.SetIsExportTaskBackGround(isExportask);
                }
                else
                {
                    gConfigs.SetWSP_IsSyncCalendarBackground(isSyncTask);
                }

                //    gConfigs.SetWSP_TaskRefreshToken(authCode);  //Added By Sarab
                if (!gConfigs.Save())
                {
                    return "false";
                    //log.Warning("Gmail Config not saved");
                }


                //if (isSaving)
                //{
                //  //  this.DialogResult = true;
                //    //ShowMessage.Info("PleaseRefreshLatestTasks", true, null, null);
                //}
                //this.DialogResult = true;
                // ShowMessage.Info("PleaseRefreshLatestTasks", true, null, null);

                //log.Info("No Settings Found");
                return "true";
            }
            int config_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"]);
            X_WSP_GmailConfiguration gConfig = new X_WSP_GmailConfiguration(ctx, config_ID, null);
            if (isTask)
            {
                gConfig.SetWSP_IsSyncTaskBackGround(isSyncTask);
                if (isRemoveLink)
                {
                    gConfig.SetWSP_TaskRefreshToken(null);
                    gConfig.SetWSP_IsSyncTaskBackGround(false);
                }
            }
            else
            {
                gConfig.SetWSP_IsSyncCalendarBackground(isSyncTask);
                if (isRemoveLink)
                {
                    gConfig.SetWSP_CalendarRefreshToken(null);
                    gConfig.SetWSP_IsSyncCalendarBackground(false);
                }
            }

            //  gConfig.SetWSP_TaskRefreshToken(authCode);
            if (!gConfig.Save())
            {
                return "false";
            }
            return "true";
        }

        public Dictionary<string, string> GetSavedDetail(Ctx ctx, bool isTask)
        {
            Dictionary<string, string> retDic = new Dictionary<string, string>();
            string sql = @"Select WSP_isSyncTaskBackground,WSP_IsSyncCalendarBackground,WSP_TaskRefreshToken,WSP_CalendarRefreshToken,WSP_ContactRefreshToken from WSP_GmailConfiguration where isActive='Y' and VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID()
                     + " and VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql, null);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                retDic["IsExisting"] = "false";
                retDic["IsSyncInBackground"] = "false";
                retDic["TaskAuthCode"] = "";
                retDic["CalendarAuthCode"] = "";
                retDic["IsCalSyncInBackground"] = "";
                retDic["ContactAuthCode"] = "";
            }
            else
            {
                string isSyncbackground = "false";
                if (ds.Tables[0].Rows[0]["WSP_ISSYNCTASKBACKGROUND"].ToString().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    isSyncbackground = "true";
                }
                else
                {
                    isSyncbackground = "false";
                }
                retDic["IsExisting"] = "true";
                retDic["IsSyncInBackground"] = isSyncbackground;
                if (Util.GetValueOfString(ds.Tables[0].Rows[0]["WSP_TaskRefreshToken"]).Length > 0)
                {
                    retDic["TaskAuthCode"] = "*********" + Msg.GetMsg(ctx, "AlreadyLinked") + "***********";
                }
                if (Util.GetValueOfString(ds.Tables[0].Rows[0]["WSP_CalendarRefreshToken"]).Length > 0)
                {
                    retDic["CalendarAuthCode"] = "*********" + Msg.GetMsg(ctx, "AlreadyLinked") + "***********";
                }
                if (Util.GetValueOfString(ds.Tables[0].Rows[0]["WSP_ContactRefreshToken"]).Length > 0)
                {
                    retDic["ContactAuthCode"] = "*********" + Msg.GetMsg(ctx, "AlreadyLinked") + "***********";
                }
                if (ds.Tables[0].Rows[0]["WSP_ISSYNCCALENDARBACKGROUND"].ToString().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    retDic["IsCalSyncInBackground"] = "true";
                }
                else
                {
                    retDic["IsCalSyncInBackground"] = "false";
                }

            }

            return retDic;
        }
        //************************************


        //**********Added By Lakhwinder************
        public List<LoginData> GetLoginData(string sql)
        {
            try
            {
                List<LoginData> ld = null;
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ld = new List<LoginData>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ld.Add(new LoginData() { Name = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i][0]), RecKey = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i][1]) });
                    }

                }
                return ld;
            }
            catch
            {
                return null;
            }
        }

        public DefaultLoginData GetDefaultLogin(int VAF_UserContact_ID)
        {
            string sql = "SELECT VAF_Role_ID,VAF_Client_ID,VAF_Org_ID,VAM_Warehouse_ID FROM VAF_LoginSetting WHERE IsActive='Y' AND VAF_UserContact_ID=" + VAF_UserContact_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            DefaultLoginData dfd = null;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dfd = new DefaultLoginData();
                dfd.VAF_Role_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][0]);
                dfd.VAF_Client_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][1]);
                dfd.VAF_Org_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][2]);
                dfd.VAM_Warehouse_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][3]);
            }
            return dfd;
        }

        //************************************

        // Added by Bharat on 12 June 2017, updated by vinay bhatt on 18 oct 2018
        public int GetWindowID(string windowName)
        {
            string sql = "SELECT VAF_Screen_ID FROM VAF_Screen WHERE IsActive='Y' AND Name = '" + windowName + "'";
            int windowID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return windowID;
        }

        /// <summary>
        /// function to filter log files for selected date passed in parameter
        /// and zip those files in one folder, if found log for date passed in the parameter
        /// </summary>
        /// <param name="logDate"></param>
        /// <returns></returns>
        public string DownloadServerLog(DateTime? logDate)
        {
            // Log files path
            string logpath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "log";
            // Temp download folder path
            string tempDownPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";
            // filter files based on the date passed in the parameter
            var todayFiles = Directory.GetFiles(logpath).Where(x => new FileInfo(x).LastWriteTime.Date.ToShortDateString() == logDate.Value.ToShortDateString());

            var zipfileName = "SLog_" + System.DateTime.Now.Ticks + ".zip";

            using (ZipFile zip = new ZipFile())
            {
                var logsFound = false;
                foreach (string fl in todayFiles)
                {
                    zip.AddFile(fl, "LOGS");
                    logsFound = true;
                }
                if (!logsFound)
                    zipfileName = "";
                else
                {
                    // check if folder exists on hosting path (TempDownload)
                    if (!Directory.Exists(tempDownPath))
                        Directory.CreateDirectory(tempDownPath);
                    if (Directory.Exists(tempDownPath))
                        zip.Save(tempDownPath + "\\" + zipfileName);
                }
            }

            return zipfileName;
        }
    }
}