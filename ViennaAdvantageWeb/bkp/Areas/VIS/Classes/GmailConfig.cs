using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VIS.Classes
{
    public class GmailConfig
    {
        private CalendarService _calenderService;
        private TasksService _service; // We don't need individual service instances for each client.
        private int AD_User_ID = 0;
        private int AD_Client_ID = 0;
        private int AD_Org_ID = 0;
        private bool isTask = true;
        private bool isContact = false;
        string authCode = string.Empty;
        private OAuth2Authenticator<WebServerClient> _authenticator;

        //#region Properties
        ///// <summary>
        ///// Returns the authorization state which was either cached or set for this session.
        ///// </summary>
        //private IAuthorizationState AuthState
        //{
        //    get
        //    {
        //        return _state ?? HttpContext.Current.Session["AUTH_STATE"] as IAuthorizationState;
        //    }
        //}
        //#endregion


        public GmailConfig(int AD_user_ID, int AD_Client_ID, int AD_Org_ID, string authCode, bool isTask,bool isContact)
        {
            this.authCode = authCode;
            this.isTask = isTask;
            this.isContact = isContact;
            this.AD_User_ID = AD_user_ID;
            this.AD_Client_ID = AD_Client_ID;
            this.AD_Org_ID = AD_Org_ID;

            //SecureEngine.Decrypt("asdas");

        }
        public GmailConfig()
        {      
        }
        StringBuilder msg = new StringBuilder();

        public string Start(Ctx ctx)
        {
            try
            {
                if (authCode.Equals(""))
                {
                    if (isTask)
                    {
                        if (_service == null)
                        {
                            _service = new TasksService(_authenticator = CreateAuthenticator(TasksService.Scopes.Tasks.ToString()));
                        }
                    }
                    else
                    {
                        if (_calenderService == null)
                        {
                            _calenderService = new CalendarService(_authenticator = CreateAuthenticator(CalendarService.Scopes.Calendar.ToString()));
                        }
                    }
                    // Check if we received OAuth2 credentials with this request; if yes: parse it.
                    if (HttpContext.Current.Request["code"] != null)
                    {
                        _authenticator.LoadAccessToken();
                    }
                    // Change the button depending on our auth-state.

                    if (isTask)
                    {
                        FetchTaskslists(_service,ctx);
                    }
                    else if (isContact)
                    {
                    }
                    else
                    {
                        FetchCalendarLists(_calenderService, ctx);

                    }
                }
                else
                {
                    Authentication(ctx);
                }
            }
            catch (Exception ewx)
            {
                msg.Append(ewx.Message);
                return ewx.Message;
            }
            state = null;
            _authenticator = null;
            _calenderService = null;
            _service = null;
            return "LinkingDone.PleaseClosethiswindow." + msg.ToString();
        }



        public void Authentication(Ctx ctx)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientCredentials.ClientID;
            provider.ClientSecret = ClientCredentials.ClientSecret;
            // Create the service. This will automatically call the authenticator.

            if (isTask)
            {
                var service = new TasksService(new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication));
                Google.Apis.Tasks.v1.TasklistsResource.ListRequest clrq = service.Tasklists.List();
                clrq.MaxResults = "1000";

                TaskLists taskslist = clrq.Fetch();


                FetchingTasks(service, taskslist,ctx);
            }
            else
            {
                var service = new CalendarService(new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication));
                Google.Apis.Calendar.v3.CalendarListResource.ListRequest clrq = service.CalendarList.List();
                var result = clrq.Fetch();
                FetchingCalendar(result, service,ctx);
            }
        }

        IAuthorizationState state = null;

        private IAuthorizationState GetAuthentication(NativeApplicationClient arg)
        {
            try
            {
                if (state != null)
                {
                    return state;
                }
                if (isTask)
                {
                  //  state = new AuthorizationState(new[] { "https://www.google.com/Tasks/feeds" });
                    state = new AuthorizationState(new[] { "https://www.googleapis.com/auth/tasks" });
                }
                else
                {
                    state = new AuthorizationState(new[] { "https://www.google.com/Calendar/feeds" });
                }
                state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
                Uri authUri = arg.RequestUserAuthorization(state);

                // Request authorization from the user (by opening a browser window):

                // Retrieve the access token by using the authorization code:
                state = arg.ProcessUserAuthorization(authCode, state);

                if (state != null && (!string.IsNullOrEmpty(state.AccessToken) || !string.IsNullOrEmpty(state.RefreshToken)))
                {
                    // Store and return the credentials.
                    if (isTask)
                    {
                        Save(true, state.RefreshToken,false);
                    }
                    else if (isContact)
                    {
                        Save(false, state.RefreshToken,true);
                    }
                    else
                    {
                        Save(false, state.RefreshToken,false);
                    }

                }
            }
            catch (Exception eX)
            {
                msg.Append(eX.Message);
            }
            return state;
        }



        string scope = string.Empty;
        public OAuth2Authenticator<WebServerClient> CreateAuthenticator(string scope)
        {
            // Register the authenticator.

            this.scope = scope;
            var provider = new WebServerClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientCredentials.ClientID;
            provider.ClientSecret = ClientCredentials.ClientSecret;
            var authenticator =
                new OAuth2Authenticator<WebServerClient>(provider, GetAuthorization) { NoCaching = true };
            return authenticator;
        }

        private IAuthorizationState GetAuthorization(WebServerClient client)
        {
            // If this user is already authenticated, then just return the auth state.
            IAuthorizationState state = null;
            if (state != null)
            {
                return state;
            }
            // Check if an authorization request already is in progress.
            state = client.ProcessUserAuthorization(new HttpRequestInfo(HttpContext.Current.Request));

            string scope = this.scope;

            client.RequestUserAuthorization(new[] { scope }, null, new Uri("urn:ietf:wg:oauth:2.0:oob", UriKind.RelativeOrAbsolute));

            return null;
        }



        #region Tasks
        /// <summary>
        /// Fetches the TasksLists of the user.
        /// </summary>
        public void FetchTaskslists(TasksService _service,Ctx ctx)
        {
            try
            {
                this._service = _service;
                // Fetch all TasksLists of the user asynchronously.
                TaskLists response = _service.Tasklists.List().Fetch();
                //foreach (TaskList list in response.Items)
                //{
                //    FetchTasks(_service, list);
                //}
                FetchTasks(_service, "@default",ctx);
            }
            catch (ThreadAbortException)
            {
                // User was not yet authenticated and is being forwarded to the authorization page.
                throw;
            }
            catch (Exception)
            {

            }
        }

        public void FetchingTasks(TasksService service, TaskLists taskslist,Ctx ctx)
        {

            //StringBuilder sql = new StringBuilder();
            //for (int i = 0; i < taskslist.Items.Count; i++)
            //{
            //    FetchTasks(service, taskslist.Items[i]);
            //}
            FetchTasks(service, "@default",ctx);
        }


        //  private void FetchTasks(TasksService _service, TaskList taskList)
        private void FetchTasks(TasksService _service, string taskList,Ctx ctx)
        {
            string sqlCategory = "SELECT APPOINTMENTCATEGORY_ID FROM APPOINTMENTCATEGORY WHERE VALUE='Task' AND ISACTIVE='Y'";
            int appointCategoryID = Util.GetValueOfInt(DB.ExecuteScalar(sqlCategory));
            var tasks = _service.Tasks.List(taskList).Fetch();
            if (tasks.Items == null)
            {
                //return "<i>No items</i>";
                return;
            }

            StringBuilder sql = new StringBuilder();

            for (int i = 0; i < tasks.Items.Count; i++)
            {
                sql.Clear();
                sql.Append("select  appointmentsinfo_id From APPOINTMENTSINFO ")
                            .Append(" where utaskid='" + tasks.Items[i].Id + "' and AD_User_ID=" + AD_User_ID);
                DataSet ds = DB.ExecuteDataset(sql.ToString());
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    if (tasks.Items[i].Title != null && tasks.Items[i].Title != "" && tasks.Items[i].Completed == null)
                    {
                        VAdvantage.Model.MAppointmentsInfo ainfo = new VAdvantage.Model.MAppointmentsInfo(ctx, 0, null);
                        ainfo.SetAD_Org_ID(AD_Org_ID);
                        ainfo.SetAD_Client_ID(AD_Client_ID);
                        ainfo.SetIsTask(true);
                        ainfo.SetAppointmentCategory_ID(appointCategoryID);
                        if (tasks.Items[i].Completed == null)
                        {
                            ainfo.SetStatus(0);
                        }
                        else
                        {
                            ainfo.SetIsClosed(true);
                            ainfo.SetStatus(10);
                            ainfo.SetTaskStatus(10);
                        }
                        ainfo.SetAD_User_ID(AD_User_ID);
                        ainfo.SetSubject(tasks.Items[i].Title);
                        if (tasks.Items[i].Notes != null)
                        {
                            ainfo.SetDescription(tasks.Items[i].Notes);
                        }
                        ainfo.SetUTaskID(tasks.Items[i].Id);
                        if (tasks.Items[i].Due != null)
                        {
                            if (Convert.ToDateTime(tasks.Items[i].Updated) < Convert.ToDateTime(tasks.Items[i].Due))
                            {
                                ainfo.SetStartDate(Convert.ToDateTime(tasks.Items[i].Updated));
                            }
                            else
                            {
                                ainfo.SetStartDate(Convert.ToDateTime(tasks.Items[i].Due));
                            }
                        }
                        else
                        {
                            ainfo.SetStartDate(Convert.ToDateTime(tasks.Items[i].Updated));
                        }




                        if (tasks.Items[i].Due != null)
                        {
                            ainfo.SetEndDate(Convert.ToDateTime(tasks.Items[i].Due));
                        }
                        else
                        {
                            ainfo.SetEndDate(Convert.ToDateTime(tasks.Items[i].Updated));
                        }

                        if (ainfo.Save())
                        {
                            sql.Clear();
                            sql.Append("Update APPOINTMENTSINFO set LastLocalUpdated=" + SetTime(ainfo.GetUpdated()) +
                                " , Updated=" + SetTime(ainfo.GetUpdated()) + ", CreatedBY= " + AD_User_ID + ", UpdatedBy=" + AD_User_ID +
                                ",LastGmailUpdated=" + GlobalVariable.TO_DATE(Convert.ToDateTime(tasks.Items[i].Updated), false) + " where APPOINTMENTSINFO_ID=" + ainfo.GetAppointmentsInfo_ID());
                            int result = DB.ExecuteQuery(sql.ToString());
                        }
                    }
                }
            }
        }
        #endregion



        #region Calendar
        public void FetchCalendarLists(CalendarService cal,Ctx ctx)
        {
            try
            {
                this._calenderService = cal;
                // Fetch all CalenderLists of the user asynchronously.
                CalendarList response = _calenderService.CalendarList.List().Fetch();
                //foreach (CalendarListEntry list in response.Items)
                //{
                //    if (list.AccessRole == "owner")
                //    {
                //        FetchCalender(_calenderService, list);
                //    }
                //}
                FetchCalender(_calenderService,ctx);
            }
            catch (ThreadAbortException)
            {
                // User was not yet authenticated and is being forwarded to the authorization page.
                throw;
            }
            catch (Exception)
            {

            }
        }


        public void FetchingCalendar(CalendarList result, CalendarService service,Ctx ctx)
        {
            //foreach (CalendarListEntry calendar in result.Items)
            //{
            //    if (calendar.AccessRole == "owner")
            //    {
            //        FetchCalender(service, calendar);
            //    }
            //}
            FetchCalender(service,ctx);
        }

        /// <summary>
        /// This Function fetch Calendar detail from Gmail and insert in Appointment Table
        /// </summary>
        /// <param name="_calenderSer"></param>
        private void FetchCalender(CalendarService _calenderSer,Ctx ctx)
        {
            string sqlCategory = "SELECT APPOINTMENTCATEGORY_ID FROM APPOINTMENTCATEGORY WHERE VALUE='Appointment' AND ISACTIVE='Y'";
            int appointCategoryID = Util.GetValueOfInt(DB.ExecuteScalar(sqlCategory));
            try
            {
                Google.Apis.Calendar.v3.EventsResource ev = new EventsResource(_calenderSer, _authenticator);
                //ev.List(CalendarList.Id).SingleEvents = true;
                Events e = null;
                try
                {
                    e = ev.List("primary").Fetch(); //Fetch Calendar Events From Gmail which are Primary
                }
                catch (Exception ex)
                {
                    msg.Append(ex.Message);
                }

                if (e.Items == null)
                {
                    msg.Append("no Item Found");
                    return;
                }
                StringBuilder sql = new StringBuilder();

                if (e.Items.Count == 0)
                {
                    msg.Append("no Item Found");
                    return;
                }

                for (int i = 0; i < e.Items.Count; i++)
                {
                    try
                    {
                        VAdvantage.Model.MAppointmentsInfo ainfo = new VAdvantage.Model.MAppointmentsInfo(ctx, 0, null);                     

                        sql.Clear();
                        sql.Append("select  appointmentsinfo_id From APPOINTMENTSINFO ")
                                   .Append(" where utaskid='" + e.Items[i].Id + "' and AD_User_ID=" + AD_User_ID);
                        DataSet ds = DB.ExecuteDataset(sql.ToString());
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            ainfo.SetAD_Org_ID(AD_Org_ID);
                            ainfo.SetAD_Client_ID(AD_Client_ID);
                            ainfo.SetAD_User_ID(AD_User_ID);
                            ainfo.SetIsTask(false);
                            ainfo.SetUTaskID(e.Items[i].Id);
                            ainfo.SetAppointmentCategory_ID(appointCategoryID);
                            ainfo.SetStatus(3);

                            if (e.Items[i].Summary != null)
                            {
                                ainfo.SetSubject(e.Items[i].Summary);
                            }
                            if (e.Items[i].Description != null)
                            {
                                ainfo.SetDescription(e.Items[i].Description);
                            }
                            if (e.Items[i].Location != null)
                            {
                                ainfo.SetLocation(e.Items[i].Location);
                            }


                            if (e.Items[i].Visibility != null)
                            {
                                if (e.Items[i].Visibility.ToString().Equals("Private", StringComparison.OrdinalIgnoreCase))
                                {
                                    ainfo.SetIsPrivate(true);                                   
                                }
                                else
                                {
                                    ainfo.SetIsPrivate(false);
                                }
                            }
                            else
                            {
                                ainfo.SetIsPrivate(false);
                            }

                            if (e.Items[i].Reminders != null)
                            {
                                Event.RemindersData rem = e.Items[i].Reminders;
                                if (rem != null)
                                {
                                    if (rem.Overrides != null)
                                    {
                                        IList<EventReminder> resmim = rem.Overrides;

                                        foreach (EventReminder r in resmim)
                                        {
                                            ainfo.SetReminderInfo(r.Minutes.ToString());                                           
                                        }
                                    }
                                }
                            }
                            if (e.Items[i].Recurrence == null || e.Items[i].Recurrence.Count < 1)
                            {
                                EventDateTime dateStart = e.Items[i].Start;
                                if (dateStart.Date == null)
                                {
                                    DateTime startDate = Convert.ToDateTime(dateStart.DateTime);
                                    ainfo.SetStartDate(startDate.ToUniversalTime());
                                    ainfo.SetAllDay(false);                                  
                                }
                                else
                                {
                                    ainfo.SetStartDate(Convert.ToDateTime(dateStart.Date).ToUniversalTime());
                                    ainfo.SetAllDay(true);                                   
                                }

                                EventDateTime dateend = e.Items[i].End;
                                if (dateend.Date == null)
                                {
                                    DateTime endDate = Convert.ToDateTime(dateend.DateTime);
                                    ainfo.SetEndDate(endDate.ToUniversalTime());
                                    ainfo.SetAllDay(false);                                 
                                }
                                else
                                {
                                    ainfo.SetEndDate(Convert.ToDateTime(dateend.Date).ToUniversalTime());
                                    ainfo.SetAllDay(true);                                  
                                }

                            }
                            else
                            {

                                if (e.Items[i].Recurrence != null && e.Items[i].Recurrence.Count > 0)
                                {
                                     EventDateTime dateStart = e.Items[i].Start;
                                     EventDateTime dateend = e.Items[i].End;

                                    if (e.Items[i].Start.Date == null)
                                    {
                                        DateTime sdate = Convert.ToDateTime(dateStart.DateTime);                                   
                                        ainfo.SetStartDate(sdate.ToUniversalTime());                              
                                        ainfo.SetAllDay(false);
                                  }
                                    else
                                    {
                                        DateTime sdate = Convert.ToDateTime(dateStart.Date);                                       
                                        ainfo.SetStartDate(sdate.ToUniversalTime());                                       
                                        ainfo.SetAllDay(true);
                                    }

                                    if (e.Items[i].End.Date == null)
                                    {
                                        ainfo.SetEndDate(Convert.ToDateTime(dateend.DateTime).ToUniversalTime());                                        
                                        ainfo.SetAllDay(false);
                                    }
                                    else
                                    {                                        
                                        ainfo.SetEndDate(Convert.ToDateTime(dateend.Date).ToUniversalTime());                                      
                                        ainfo.SetAllDay(true);
                                    }
                                    string recurenceRule = e.Items[i].Recurrence[0].Replace("RRULE:", "");
                                    ainfo.SetRecurrenceRule(recurenceRule);  //Save Recurrence rule
                                }
                            }
                            if (ainfo.Save())
                            {
                                sql.Clear();
                                sql.Append("Update APPOINTMENTSINFO set LastLocalUpdated=" + SetTime(ainfo.GetUpdated()) +
                                    " , Updated=" + SetTime(ainfo.GetUpdated()) + ", CreatedBY= " + AD_User_ID + ", UpdatedBy=" + AD_User_ID +
                                    ",LastGmailUpdated=" + GlobalVariable.TO_DATE(Convert.ToDateTime(e.Items[i].Updated), false) + " where APPOINTMENTSINFO_ID=" + ainfo.GetAppointmentsInfo_ID());
                                int result = DB.ExecuteQuery(sql.ToString());
                            }
                        }
                    }
                    catch (Exception exq)
                    {
                        msg.Append(exq.Message + " At no. " + i);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Append(ex.Message);
            }
        }


        #endregion
        /// <summary>
        /// This function saves the RefreshToken for task as well as calendar
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="token"></param>
        public void Save(bool flag, string token,bool isContact)
        {
            //If flag=true means it is task
            //flag=IsTask
            if (flag)
            {
                string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where isActive='Y' and AD_User_ID=" + AD_User_ID + " and AD_Client_ID=" + AD_Client_ID;
                object configID = DB.ExecuteScalar(sql);
                if (configID != null && configID != DBNull.Value)
                {
                    sql = "update WSP_GmailConfiguration set WSP_Taskrefreshtoken='" + token + "' where WSP_GmailConfiguration_id=" + Convert.ToInt32(configID);
                    int i = DB.ExecuteQuery(sql);
                }
                else
                {
                    int ID = DB.GetNextID(0, "WSP_GmailConfiguration", null);
                    sql = @"Insert into WSP_GmailConfiguration (ad_Org_ID,AD_Client_ID,IsActive,CreatedBy,UpdatedBy,WSP_GmailConfiguration_ID,AD_User_ID,WSP_TASKrefreshtoken)
                                         VALUES(" + AD_Org_ID + "," + AD_Client_ID + ",'Y'," + AD_User_ID + "," + AD_User_ID + "," + ID + "," + AD_User_ID + ",'" + token + "')";
                    int res = DB.ExecuteQuery(sql, null);
                }
            }
            else if(isContact)
            {
                string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where isActive='Y' and AD_User_ID=" + AD_User_ID + " and AD_Client_ID=" + AD_Client_ID;
                object configID = DB.ExecuteScalar(sql);
                if (configID != null && configID != DBNull.Value)
                {
                    sql = "update WSP_GmailConfiguration set WSP_Contactrefreshtoken='" + token + "' where WSP_GmailConfiguration_id=" + Convert.ToInt32(configID);
                    int i = DB.ExecuteQuery(sql);
                }
                else
                {
                    int ID = DB.GetNextID(0, "WSP_GmailConfiguration", null);
                    sql = @"Insert into WSP_GmailConfiguration (ad_Org_ID,AD_Client_ID,IsActive,CreatedBy,UpdatedBy,WSP_GmailConfiguration_ID,AD_User_ID,WSP_Contactrefreshtoken)
                                         VALUES(" + AD_Org_ID + "," + AD_Client_ID + ",'Y'," + AD_User_ID + "," + AD_User_ID + "," + ID + "," + AD_User_ID + ",'" + token + "')";
                    int res = DB.ExecuteQuery(sql, null);
                }
             }
            else
            {
                string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where isActive='Y' and AD_User_ID=" + AD_User_ID + " and AD_Client_ID=" + AD_Client_ID;
                object configID = DB.ExecuteScalar(sql);
                if (configID != null && configID != DBNull.Value)
                {
                    sql = "update WSP_GmailConfiguration set WSP_CalendarRefreshToken='" + token + "' where WSP_GmailConfiguration_id=" + Convert.ToInt32(configID);
                    int i = DB.ExecuteQuery(sql);
                }
                else
                {
                    int ID = DB.GetNextID(0, "WSP_GmailConfiguration", null);
                    sql = @"Insert into WSP_GmailConfiguration (ad_Org_ID,AD_Client_ID,IsActive,CreatedBy,UpdatedBy,WSP_GmailConfiguration_ID,AD_User_ID,WSP_CalendarRefreshToken)
                                         VALUES(" + AD_Org_ID + "," + AD_Client_ID + ",'Y'," + AD_User_ID + "," + AD_User_ID + "," + ID + "," + AD_User_ID + ",'" + token + "')";
                    int res = DB.ExecuteQuery(sql, null);
                }
            }

        }


        public string SetTime(DateTime time)
        {
            StringBuilder dateString = new StringBuilder("TO_DATE('");
            dateString.Append(time.ToString("yyyy-MM-dd HH:mm:ss"));	//	cut off miliseconds
            dateString.Append("','YYYY-MM-DD HH24:MI:SS')");
            return dateString.ToString();
        }

    }
}