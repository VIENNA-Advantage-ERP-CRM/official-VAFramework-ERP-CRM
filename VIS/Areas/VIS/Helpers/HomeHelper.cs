using System;
using System.Collections.Generic;
using System.Linq;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VIS.Models;
using System.Text;
using VIS.DataContracts;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using VAdvantage.Classes;
namespace VIS.Helpers
{

    public class HomeHelper
    {
        DataSet dsData = new DataSet();
        string strQuery = "";

        //To get the All Alert Count
        public HomeModels getHomeAlrtCount(Ctx ctx)
        {
            HomeModels objHome = new HomeModels();
            try
            {
                #region Request Count
                //To Get Request count
                strQuery = " SELECT  count(R_Request.r_request_id) FROM R_Request  inner join  r_requesttype rt on R_Request.r_requesttype_id=rt.r_requesttype_ID";
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "R_Request", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += " AND ( R_Request.SalesRep_ID =" + ctx.GetAD_User_ID() + " OR R_Request.AD_Role_ID =" + ctx.GetAD_Role_ID() + ")"
                 + " AND R_Request.Processed ='N'"
                + " AND (R_Request.R_Status_ID IS NULL OR R_Request.R_Status_ID IN (SELECT R_Status_ID FROM R_Status WHERE IsClosed='N'))";
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nRequest = 0;
                if (dsData != null)
                {
                    nRequest = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }

                #endregion

                # region Notice Count
                //To get Notice Count
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL("SELECT count(AD_Note_ID) FROM AD_Note "
                    , "AD_Note", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += " AND AD_User_ID IN (" + ctx.GetAD_User_ID() + ")"
                  + " AND Processed='N'";
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nNotice = 0;
                if (dsData != null)
                {
                    nNotice = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }
                #endregion

                #region WorkFlow Count
                //To Get Work flow Count
                //strQuery = "select count(a.AD_WF_Activity_ID) FROM AD_WF_Activity a Left Outer JOin AD_WF_NOde node ON a.ad_wf_node_id=node.ad_wf_node_id  "
                //     + "WHERE a.Processed='N' AND a.WFState='OS' AND a.AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND ("
                //    // Owner of Activity
                //      + " a.AD_User_ID=" + ctx.GetAD_User_ID() // #1
                //    // Invoker (if no invoker = all)
                //      + " OR EXISTS (SELECT * FROM AD_WF_Responsible r WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID"
                //      + " AND COALESCE(r.AD_User_ID,0)=0 AND (a.AD_User_ID=" + ctx.GetAD_User_ID() + " OR a.AD_User_ID IS NULL))" // #2
                //    // Responsible User
                //      + " OR EXISTS (SELECT * FROM AD_WF_Responsible r WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID"
                //      + " AND r.AD_User_ID=" + ctx.GetAD_User_ID() + ")"  // #3
                //    // Responsible Role
                //      + " OR EXISTS (SELECT * FROM AD_WF_Responsible r INNER JOIN AD_User_Roles ur ON (r.AD_Role_ID=ur.AD_Role_ID)"
                //      + " WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID AND ur.AD_User_ID=" + ctx.GetAD_User_ID() + " and a.AD_Client_ID=" + ctx.GetAD_Client_ID() + " and a.AD_Org_ID=" + ctx.GetAD_Org_ID() + ")" // #4
                //    //
                //      + ") ORDER BY a.Priority DESC, a.Created";

                strQuery = @"SELECT COUNT(*)
                            FROM AD_WF_Activity a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.AD_Client_ID =" + ctx.GetAD_Client_ID() + @"
                            AND ( (a.AD_User_ID=" + ctx.GetAD_User_ID() + @"
                            OR a.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND COALESCE(r.AD_User_ID,0)=0
                              AND (a.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IS NULL
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (r.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              INNER JOIN AD_User_Roles ur
                              ON (r.AD_Role_ID            =ur.AD_Role_ID)
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (ur.AD_User_ID          =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H'
                              ) )
                           ";
                // Applied Role access on workflow Activities
                strQuery = MRole.GetDefault(ctx).AddAccessSQL(strQuery, "a", true, true);
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nWorkFlow = 0;
                if (dsData != null)
                {
                    nWorkFlow = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }
                #endregion

                #region FollowUps
                //To get the Folloups count
                StringBuilder SqlQuery = new StringBuilder();
                SqlQuery.Append("SELECT COUNT(inn.ChatID) As Count");

                SqlQuery.Append(" FROM (SELECT * FROM (SELECT CH.cm_chat_id AS ChatID,  max(CE.cm_chatentry_id) AS EntryID")
                         .Append("  FROM cm_chatentry CE JOIN cm_chat CH ON (CE.cm_chat_id= CH.cm_chat_id) ")
                         .Append("  JOIN cm_subscribe CS  ON (CH.ad_table_id= CS.ad_table_id) AND (CH.record_id = CS.record_id)")
                         .Append("  WHERE cs.createdby=" + ctx.GetAD_User_ID() + " GROUP BY CH.cm_chat_id ORDER BY entryID )inn1) inn ")
                         .Append("  JOIN cm_chatentry CH ON inn.ChatID= ch.cm_chat_id ")
                         .Append("  JOIN cm_chat CMH ON (cmh.cm_chat_id= inn.chatid)")
                         .Append("  JOIN cm_subscribe CS  ON (CMH.ad_table_id= CS.ad_table_id) AND (CMH.record_id = CS.record_id)")
                         .Append(" JOIN ad_user Au ON au.ad_user_id= CH.createdBy")
                         .Append(" LEFT OUTER JOIN ad_image AI ON (ai.ad_image_id=au.ad_image_id)")
                         .Append("  JOIN ad_window AW ON (cs.ad_window_id= aw.ad_window_id) LEFT OUTER  JOIN ad_image adi ON (adi.ad_image_id= aw.ad_image_id)  WHERE cs.createdby=" + ctx.GetAD_User_ID())
                         .Append(" AND ch.cm_chatentry_ID =(SELECT max(cm_chatentry_ID) FROM cm_chatentry WHERE CM_Chat_ID= ch.cm_chat_id)");
                       // .Append("  order by inn.EntryID desc,ch.cm_chatentry_id asc");
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(SqlQuery.ToString());
                int nTotalFollow = 0;
                if (dsData != null)
                {
                    nTotalFollow = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }

                #endregion

                #region Notes
                //To get The Notes count
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL("SELECT COUNT(wsp_note_id) As NCount FROM WSP_Note", "WSP_Note", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " AND AD_USER_ID=" + ctx.GetAD_User_ID() + " Order BY Created DESC";
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nNotes = 0;
                if (dsData != null)
                {
                    nNotes = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }


                #endregion

                #region Appointments Count

                strQuery = "SELECT AppointmentsInfo.Appointmentsinfo_id AS ID,AppointmentsInfo.AD_Client_ID,AppointmentsInfo.AD_Org_ID"
                    //+ "  FROM AppointmentsInfo JOIN AD_User ON AD_User.AD_User_ID =AppointmentsInfo.CreatedBy WHERE AppointmentsInfo.IsRead='N' AND AppointmentsInfo.istask ='N' AND  AppointmentsInfo.CreatedBy  !=" + ctx.GetAD_User_ID() + " AND AppointmentsInfo.AD_User_ID  = " + ctx.GetAD_User_ID() + ""
                          + "  FROM AppointmentsInfo AppointmentsInfo JOIN AD_User AD_User ON AD_User.AD_User_ID =AppointmentsInfo.CreatedBy WHERE AppointmentsInfo.IsRead='N' AND AppointmentsInfo.istask ='N'  AND AppointmentsInfo.AD_User_ID  = " + ctx.GetAD_User_ID() + ""
                         + "  UNION SELECT AppointmentsInfo.Appointmentsinfo_id AS ID, AppointmentsInfo.AD_Client_ID,AppointmentsInfo.AD_Org_ID from AppointmentsInfo AppointmentsInfo "
                         + "  JOIN AD_User AD_User ON AD_User.AD_User_ID = AppointmentsInfo.CreatedBy  WHERE (AppointmentsInfo.IsRead ='Y'   AND AppointmentsInfo.AD_User_ID = " + ctx.GetAD_User_ID() + " )  AND AppointmentsInfo.istask ='N'  AND AppointmentsInfo.startDate BETWEEN to_date('";
                //DateTime.Now.ToShortDateString()
                strQuery += DateTime.Now.AddDays(-1).ToString("M/dd/yy");
                // Use current time
                strQuery += @"','mm/dd/yy') and to_date('";
                strQuery += DateTime.Now.AddDays(7).ToString("M/dd/yy");
                //DateTime.UtcNow.AddDays(1).ToShortDateString() 
                strQuery += " 23.59','mm/dd/yy HH24:MI') "
                          + "  OR  to_date('" + DateTime.Now.ToString("M/dd/yy") + "','mm/dd/yy')  BETWEEN  AppointmentsInfo.startDate  AND AppointmentsInfo.endDate  AND  AppointmentsInfo.CreatedBy  !=" + ctx.GetAD_User_ID() + " AND AppointmentsInfo.AD_User_ID  = " + ctx.GetAD_User_ID() ;
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "AppointmentsInfo", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


                strQuery = "SELECT COUNT( AppointmentsInfo.ID)  FROM (" + strQuery + ") AppointmentsInfo";
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nTotalAppnt = 0;
                if (dsData != null)
                {
                    nTotalAppnt = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }

                //dsData = new DataSet();
                //dsData = DB.ExecuteDataset(strQuery);
                //int nAppt = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());

                #endregion

                #region Task Assign By me count

                strQuery = " SELECT COUNT(AppointmentsInfo.Appointmentsinfo_id)   FROM AppointmentsInfo ";
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "AppointmentsInfo", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += "  AND  AppointmentsInfo.IsRead='N' AND AppointmentsInfo.istask ='Y'  AND AppointmentsInfo.isClosed ='N'  AND  AppointmentsInfo.CreatedBy =" + ctx.GetAD_User_ID() + "  AND  AppointmentsInfo.AD_User_ID !=" + ctx.GetAD_User_ID() + "";

                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nTaskAssignByMe = 0;
                if (dsData != null)
                {
                    nTaskAssignByMe = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }

                #endregion

                #region My task count


                strQuery = " SELECT COUNT(AppointmentsInfo.Appointmentsinfo_id)   FROM AppointmentsInfo ";
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "AppointmentsInfo", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += "  AND   AppointmentsInfo.IsRead='N' AND AppointmentsInfo.istask ='Y' AND AppointmentsInfo.isClosed ='N'  AND AppointmentsInfo.AD_User_ID =" + ctx.GetAD_User_ID() + " ";

                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                int nMyTask = 0;
                if (dsData != null)
                {
                    nMyTask = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
                }



                #endregion

                #region To do Count

                int nTodo = 0;
                #endregion

                #region Incommin req Count
                int nIncmngReq = 0;
                #endregion

                #region KPI COUNT
                int userid = ctx.GetAD_User_ID();
                int roleid = ctx.GetAD_Role_ID();

                string sql = @"SELECT Distinct  kpi.RC_KPI_ID
                                        FROM RC_KPI kpi
                                        
                                        WHERE kpi.IsActive      = 'Y'
                                        AND kpi.KPITYPE         ='Te'

                                        AND kpi.RC_KPI_ID  IN
                                          (SELECT record_ID
                                          FROM ad_userhomepagesetting
                                          WHERE ISActive ='Y'
                                          AND AD_Table_ID=
                                            (SELECT AD_Table_ID FROM AD_Table WHERE TableName='RC_KPI'
                                            )
                                          AND AD_User_ID=" + userid + ") ";
                //sql += " AND ( (acc.AD_User_ID = " + userid + @")   OR ( acc.AD_Role_ID = " + roleid + @") ) ";



                dsData = new DataSet();
                dsData = DB.ExecuteDataset(sql);
                int KPICount = 0;
                if (dsData != null)
                {
                    KPICount = dsData.Tables[0].Rows.Count;
                }

                #endregion

                objHome.RequestCnt = nRequest;
                objHome.NoticeCnt = nNotice;
                objHome.WorkFlowCnt = nWorkFlow;

                objHome.FollowUpCnt = nTotalFollow;
                objHome.AppointmentCnt = nTotalAppnt;
                objHome.ToDoCnt = nTodo;
                objHome.NotesCnt = nNotes;
                objHome.IncommingRequestCnt = nIncmngReq;
                objHome.TaskAssignByMeCnt = nTaskAssignByMe;
                objHome.MyTaskCnt = nMyTask;
                objHome.KPICnt = KPICount;
                #region Current User Info
                strQuery = "SELECT au.name,au.AD_Image_ID,au.email,ai.binarydata,ai.imageurl,au.comments FROM ad_user au LEFT OUTER JOIN ad_image ai  ON (ai.ad_image_id= au.ad_image_id) WHERE ad_user_id=" + ctx.GetAD_User_ID();
                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                if (dsData != null)
                {
                    objHome.UsrName = dsData.Tables[0].Rows[0]["name"].ToString();
                    objHome.UsrEmail = dsData.Tables[0].Rows[0]["email"].ToString();
                    if (Util.GetValueOfString(dsData.Tables[0].Rows[0]["comments"]).Length > 0)
                        objHome.UsrStatus = dsData.Tables[0].Rows[0]["comments"].ToString();
                    if (dsData.Tables[0].Rows[0]["binarydata"].ToString() != "")
                    {
                        objHome.UsrImage = Convert.ToBase64String((byte[])dsData.Tables[0].Rows[0]["binarydata"]);
                    }
                    else
                    {
                        //***** Added By Sarab

                        //***** If User Image saved in FileSystem
                        try
                        {
                            MImage objImage = new MImage(ctx, Util.GetValueOfInt(dsData.Tables[0].Rows[0]["AD_Image_ID"]), null);
                            objHome.UsrImage = Convert.ToBase64String(objImage.GetThumbnailByte(140, 120));
                        }
                        catch
                        {
                        }
                        //FileStream stream = null;
                        //try
                        //{
                        //    string filepath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, dsData.Tables[0].Rows[0]["imageurl"].ToString());
                        //    if(File.Exists(filepath))
                        //    {
                        //    stream = File.OpenRead(filepath);
                        //    byte[] fileBytes = new byte[stream.Length];
                        //    stream.Read(fileBytes, 0, fileBytes.Length);
                        //    objHome.UsrImage = Convert.ToBase64String(fileBytes);
                        //    stream.Close();
                        //    }
                        //}
                        //catch
                        //{
                        //}
                        //finally
                        //{
                        //    if(stream!=null)
                        //    stream.Close();
                        //}

                    }
                    //ShowGreeting(objHome, ctx);
                }

                #endregion

                return objHome;
            }
            catch (Exception)
            {

            }
            return objHome;
        }

        #region get Login User Info
        //Get login user info
        public HomeModels getLoginUserInfo(Ctx ctx, int height, int width)
        {
            HomeModels objHome = null;
            string strUserQuery = "SELECT au.name,au.AD_Image_ID,au.email,ai.binarydata,ai.imageurl FROM ad_user au LEFT OUTER JOIN ad_image ai  ON (ai.ad_image_id= au.ad_image_id) WHERE ad_user_id=" + ctx.GetAD_User_ID();
            dsData = new DataSet();
            dsData = DB.ExecuteDataset(strUserQuery);
            objHome = new HomeModels();
            if (dsData != null)
            {
                objHome.UsrName = dsData.Tables[0].Rows[0]["name"].ToString();
                objHome.UsrEmail = dsData.Tables[0].Rows[0]["email"].ToString();
                var uimgId = Util.GetValueOfInt(dsData.Tables[0].Rows[0]["AD_Image_ID"].ToString());

                MImage mimg = new MImage(ctx, uimgId, null);
                var imgfll = mimg.GetThumbnailURL(height, width);
                objHome.UsrImage = imgfll;

                //if (dsData.Tables[0].Rows[0]["binarydata"].ToString() != "")
                //{
                //    objHome.UsrImage = Convert.ToBase64String((byte[])dsData.Tables[0].Rows[0]["binarydata"]);
                //}
                //else
                //{
                //    try
                //    {
                //        MImage objImage = new MImage(ctx, Util.GetValueOfInt(dsData.Tables[0].Rows[0]["AD_Image_ID"]), null);

                //        objHome.UsrImage = Convert.ToBase64String(objImage.GetThumbnailByte(height, width));
                //    }
                //    catch
                //    {
                //    }
                //}
            }
            return objHome;
        }

        #endregion

        #region Show Greeting
        /// <summary>
        /// Show Greeting according to time
        /// </summary>
        /// <param name="objHome"></param>
        /// <param name="ctx"></param>
        //private void ShowGreeting(HomeModels objHome, Ctx ctx)  //Added By Sarab
        //{
        //    if (DateTime.Now.Hour < 12)
        //    {
        //        objHome.Greeting = Msg.GetMsg(ctx, "GoodMorning");
        //    }
        //    else if (DateTime.Now.Hour < 17)
        //    {
        //        objHome.Greeting = Msg.GetMsg(ctx, "GoodAfternoon");
        //    }
        //    else
        //    {
        //        objHome.Greeting = Msg.GetMsg(ctx, "GoodEvening");

        //    }
        //}
        #endregion

        # region Follups start

        public int getFllCnt(Ctx ctx)
        {
            //To get the Folloups count
            StringBuilder SqlQuery = new StringBuilder();
            SqlQuery.Append("select COUNT(inn.ChatID) As Count");

            SqlQuery.Append(" from (select * from (select CH.cm_chat_id as ChatID,  max(CE.cm_chatentry_id)as EntryID")
                     .Append("  from cm_chatentry CE join cm_chat CH on CE.cm_chat_id= CH.cm_chat_id ")
                     .Append("  JOIN cm_subscribe CS  ON (CH.ad_table_id= CS.ad_table_id) AND (CH.record_id = CS.record_id)")
                     .Append("  where cs.createdby=" + ctx.GetAD_User_ID() + " group by CH.cm_chat_id order by entryID )inn1) inn ")
                     .Append("  JOIN cm_chatentry CH on inn.ChatID= ch.cm_chat_id ")
                     .Append("  JOIN cm_chat CMH on (cmh.cm_chat_id= inn.chatid)")
                     .Append("  JOIN cm_subscribe CS  ON (CMH.ad_table_id= CS.ad_table_id) AND (CMH.record_id = CS.record_id)")
                     .Append(" Join ad_user Au on au.ad_user_id= CH.createdBy")
                     .Append(" left outer JOIN ad_image AI on(ai.ad_image_id=au.ad_image_id)")
                     .Append("  join ad_window AW on(cs.ad_window_id= aw.ad_window_id) left outer  JOIN ad_image adi on(adi.ad_image_id= aw.ad_image_id)  where cs.createdby=" + ctx.GetAD_User_ID())
                     .Append(" and ch.cm_chatentry_ID =(Select max(cm_chatentry_ID) from cm_chatentry where CM_Chat_ID= ch.cm_chat_id)")
                     .Append("  order by inn.EntryID desc,ch.cm_chatentry_id asc");
            try
            {
                int nTotalFollow = Util.GetValueOfInt(DB.ExecuteScalar(SqlQuery.ToString()));
                return nTotalFollow;
            }
            catch
            {
                return 0;
            }


        }

        //Get Folloups
        public HomeFolloUpsInfo getFolloUps(Ctx ctx, int PageSize, int page)
        {
            List<HomeFolloUps> lstFollUps = new List<HomeFolloUps>();
            List<FllUsrImages> lstUImg = new List<FllUsrImages>();
            HomeFolloUpsInfo objFllupsInfo = new HomeFolloUpsInfo();
            //List<HomeFolloUpsInfo> objFllups = new List<HomeFolloUpsInfo>();
            try
            {
                // To get the Folloups details
                StringBuilder SqlQuery = new StringBuilder();
                SqlQuery.Append("select inn.ChatID, inn.EntryID,  CH.characterdata, ch.cm_chatentry_id,");
                if (ctx.GetAD_Language() != Env.GetBaseAD_Language())
                {
                    SqlQuery.Append("(Select name from AD_Window_Trl where AD_Window_ID= Aw.AD_Window_ID and AD_Language='" + ctx.GetAD_Language() + "') as WINNAME,");
                }
                else
                {
                    SqlQuery.Append("aw.DisplayName as WINNAME,");
                }
                SqlQuery.Append("  At.TableName, aw.AD_Window_ID,cs.AD_Table_ID,cs.RECOrd_ID, aw.help,au.Name AS NAME,cs.cm_subscribe_ID, ch.created,ai.ad_image_id ,ai.binarydata as UsrImg,  adi.binarydata as WinImg,CH.createdby  from (select * from (select CH.cm_chat_id as ChatID,  max(CE.cm_chatentry_id)as EntryID")
                        .Append("  from cm_chatentry CE join cm_chat CH on CE.cm_chat_id= CH.cm_chat_id ")
                        .Append("  JOIN cm_subscribe CS  ON (CH.ad_table_id= CS.ad_table_id) AND (CH.record_id = CS.record_id)")
                        .Append("  where cs.createdby=" + ctx.GetAD_User_ID() + " group by CH.cm_chat_id order by entryID )inn1) inn ")
                        .Append("  JOIN cm_chatentry CH on inn.ChatID= ch.cm_chat_id ")
                        .Append("  JOIN cm_chat CMH on (cmh.cm_chat_id= inn.chatid)")
                        .Append("  JOIN cm_subscribe CS  ON (CMH.ad_table_id= CS.ad_table_id) AND (CMH.record_id = CS.record_id)")
                        .Append("  Join ad_user Au on au.ad_user_id= CH.createdBy")
                        .Append("  Join ad_Table At on at.ad_Table_id= CS.ad_table_id")
                        .Append("  left outer JOIN ad_image AI on(ai.ad_image_id=au.ad_image_id)")
                        .Append("  join ad_window AW on(cs.ad_window_id= aw.ad_window_id) left outer  JOIN ad_image adi on(adi.ad_image_id= aw.ad_image_id)")
                        .Append("  where cs.createdby=" + ctx.GetAD_User_ID())
                        .Append("  and ch.cm_chatentry_ID =(Select max(cm_chatentry_ID) from cm_chatentry where CM_Chat_ID= ch.cm_chat_id)")
                        .Append("  order by inn.EntryID desc,ch.cm_chatentry_id asc");

                SqlParamsIn objSP = new SqlParamsIn();
                dsData = new DataSet();
                objSP.page = page;
                objSP.pageSize = PageSize;
                objSP.sql = SqlQuery.ToString();
                dsData = VIS.DBase.DB.ExecuteDatasetPaging(objSP.sql, objSP.page, objSP.pageSize);
                if (dsData != null)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        var Fllps = new HomeFolloUps();
                        Fllps.ChatID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ChatID"].ToString());
                        Fllps.ChatEntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_chatentry_id"].ToString());
                        Fllps.EntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["EntryID"].ToString());
                        Fllps.WinID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Window_ID"].ToString());
                        Fllps.TableID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Table_ID"].ToString());
                        Fllps.RecordID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["Record_ID"].ToString());
                        Fllps.SubscriberID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_subscribe_ID"].ToString());
                        Fllps.ChatData = dsData.Tables[0].Rows[i]["characterdata"].ToString();
                        Fllps.TableName = dsData.Tables[0].Rows[i]["TableName"].ToString();
                        Fllps.Name = dsData.Tables[0].Rows[i]["NAME"].ToString();
                        Fllps.WinName = dsData.Tables[0].Rows[i]["WINNAME"].ToString();
                        Fllps.AD_User_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["CREATEDBY"].ToString());
                        DateTime _createdDate = new DateTime();
                        if (dsData.Tables[0].Rows[i]["created"].ToString() != null && dsData.Tables[0].Rows[i]["created"].ToString() != "")
                        {
                            _createdDate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["created"].ToString());
                            DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                            _createdDate = _format;

                            Fllps.Cdate = _format;
                        }
                        int uimgId = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ad_image_id"].ToString());
                        Fllps.AD_Image_ID = uimgId;
                        if (lstUImg.Where(a => a.AD_Image_ID == uimgId).Count() == 0)
                        {
                            var uimsg = new FllUsrImages();
                            uimsg.AD_Image_ID = uimgId;
                            MImage mimg = new MImage(ctx, uimgId, null);
                            var imgfll = mimg.GetThumbnailURL(46, 46);
                            uimsg.UserImg = imgfll;
                            lstUImg.Add(uimsg);

                            //var imgfll = "data:image/jpg;base64," + Convert.ToBase64String(mimg.GetThumbnailByte(46, 46));
                            //if (imgfll.ToString() == "FileDoesn'tExist" || imgfll.ToString() == "NoRecordFound")
                            //{
                            //}
                            //else
                            //{
                            //    uimsg.UserImg = imgfll;
                            //}
                            //lstUImg.Add(uimsg);
                            //if (dsData.Tables[0].Rows[i]["UsrImg"].ToString() != "")
                            //{
                            //    uimsg.UserImg = Convert.ToBase64String((byte[])dsData.Tables[0].Rows[i]["UsrImg"]);
                            //}   
                        }


                        /**************** IDENTIFIER **************/

                        string sql = "select ColumnName from AD_Column where AD_Table_ID=" + Fllps.TableID + " and isidentifier='Y'  order by seqno";
                        DataSet ds = DB.ExecuteDataset(sql);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string columns = "";
                            for (int l = 0; l < ds.Tables[0].Rows.Count; l++)
                            {
                                if (columns.Length > 0)
                                {
                                    columns += "|| '_' || " + ds.Tables[0].Rows[l]["ColumnName"].ToString();
                                }
                                else
                                {
                                    columns += ds.Tables[0].Rows[l]["ColumnName"].ToString();
                                }
                            }

                            sql = "SELECT distinct " + columns + " FROM " + Fllps.TableName + " WHERE " + Fllps.TableName + "_ID = " + Fllps.RecordID;
                            object identifier = DB.ExecuteScalar(sql);
                            if (identifier != null && identifier != DBNull.Value)
                            {
                                Fllps.Identifier = identifier.ToString();
                            }
                        }

                        /***************  END IDENTIFIER  *************/



                        lstFollUps.Add(Fllps);
                    }

                }
                objFllupsInfo.lstUserImg = lstUImg;
                objFllupsInfo.lstFollowups = lstFollUps;
            }
            catch (Exception)
            {

            }
            return objFllupsInfo;
        }

        //Get  lastest  Folloups comment
        public HomeFolloUpsInfo getLatestFolloUps(Ctx ctx, int ChatID)
        {
            List<HomeFolloUps> lstFollUps = new List<HomeFolloUps>();
            List<FllUsrImages> lstUImg = new List<FllUsrImages>();
            HomeFolloUpsInfo objFllupsInfo = new HomeFolloUpsInfo();
            try
            {
                // To get the Folloups details
                StringBuilder SqlQuery = new StringBuilder();
                SqlQuery.Append("select inn.ChatID, inn.EntryID,  CH.characterdata, ch.cm_chatentry_id,");
                if (ctx.GetAD_Language() != Env.GetBaseAD_Language())
                {
                    SqlQuery.Append("(Select name from AD_Window_Trl where AD_Window_ID= Aw.AD_Window_ID and AD_Language='" + ctx.GetAD_Language() + "') as WINNAME,");
                }
                else
                {
                    SqlQuery.Append("aw.DisplayName as WINNAME,");
                }
                SqlQuery.Append("  At.TableName, aw.AD_Window_ID,cs.AD_Table_ID,cs.RECOrd_ID, aw.help,au.Name AS NAME,cs.cm_subscribe_ID, ch.created,ai.ad_image_id ,ai.binarydata as UsrImg,  adi.binarydata as WinImg  from (select * from (select CH.cm_chat_id as ChatID,  max(CE.cm_chatentry_id)as EntryID")
                        .Append("  from cm_chatentry CE join cm_chat CH on CE.cm_chat_id= CH.cm_chat_id ")
                        .Append("  JOIN cm_subscribe CS  ON (CH.ad_table_id= CS.ad_table_id) AND (CH.record_id = CS.record_id)")
                        .Append("  where cs.createdby=" + ctx.GetAD_User_ID() + " group by CH.cm_chat_id order by entryID )inn1) inn ")
                        .Append("  JOIN cm_chatentry CH on inn.ChatID= ch.cm_chat_id ")
                        .Append("  JOIN cm_chat CMH on (cmh.cm_chat_id= inn.chatid)")
                        .Append("  JOIN cm_subscribe CS  ON (CMH.ad_table_id= CS.ad_table_id) AND (CMH.record_id = CS.record_id)")
                        .Append("  Join ad_user Au on au.ad_user_id= CH.createdBy")
                        .Append("  Join ad_Table At on at.ad_Table_id= CS.ad_table_id")
                        .Append("  left outer JOIN ad_image AI on(ai.ad_image_id=au.ad_image_id)")
                        .Append("  join ad_window AW on(cs.ad_window_id= aw.ad_window_id) left outer  JOIN ad_image adi on(adi.ad_image_id= aw.ad_image_id)")
                        .Append("  where cs.createdby=" + ctx.GetAD_User_ID())
                        .Append("  and ch.cm_chatentry_ID =(Select max(cm_chatentry_ID) from cm_chatentry where CM_Chat_ID= ch.cm_chat_id)")
                        .Append("  order by inn.EntryID desc,ch.cm_chatentry_id asc");

                SqlParamsIn objSP = new SqlParamsIn();
                dsData = new DataSet();

                dsData = DB.ExecuteDataset(objSP.sql);
                if (dsData != null)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        var Fllps = new HomeFolloUps();
                        Fllps.ChatID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ChatID"].ToString());
                        Fllps.ChatEntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_chatentry_id"].ToString());
                        Fllps.EntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["EntryID"].ToString());
                        Fllps.WinID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Window_ID"].ToString());
                        Fllps.TableID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Table_ID"].ToString());
                        Fllps.RecordID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["Record_ID"].ToString());
                        Fllps.SubscriberID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_subscribe_ID"].ToString());
                        Fllps.ChatData = dsData.Tables[0].Rows[i]["characterdata"].ToString();
                        Fllps.TableName = dsData.Tables[0].Rows[i]["TableName"].ToString();
                        Fllps.Name = dsData.Tables[0].Rows[i]["NAME"].ToString();
                        Fllps.WinName = dsData.Tables[0].Rows[i]["WINNAME"].ToString();
                        if (dsData.Tables[0].Rows[i]["created"].ToString() != "")
                        {
                            Fllps.Cdate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["created"]);
                        }
                        int uimgId = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ad_image_id"].ToString());
                        Fllps.AD_Image_ID = uimgId;
                        if (lstUImg.Where(a => a.AD_Image_ID == uimgId).Count() == 0)
                        {
                            var uimsg = new FllUsrImages();
                            uimsg.AD_Image_ID = uimgId;
                            MImage mimg = new MImage(ctx, uimgId, null);
                            var imgfll = mimg.GetThumbnailURL(46, 46);
                            uimsg.UserImg = imgfll;
                            lstUImg.Add(uimsg);

                            //var imgfll = "data:image/jpg;base64," + Convert.ToBase64String(mimg.GetThumbnailByte(46, 46));
                            //if (imgfll.ToString() == "FileDoesn'tExist" || imgfll.ToString() == "NoRecordFound")
                            //{
                            //}
                            //else
                            //{
                            //    uimsg.UserImg = imgfll;
                            //}
                            //lstUImg.Add(uimsg);
                            //if (dsData.Tables[0].Rows[i]["UsrImg"].ToString() != "")
                            //{
                            //    uimsg.UserImg = Convert.ToBase64String((byte[])dsData.Tables[0].Rows[i]["UsrImg"]);
                            //}   
                        }

                        lstFollUps.Add(Fllps);
                    }
                    objFllupsInfo.lstUserImg = lstUImg;
                    objFllupsInfo.lstFollowups = lstFollUps;
                }
            }
            catch (Exception)
            {

            }
            return objFllupsInfo;
        }


        //Get Followups cmnt
        public HomeFolloUpsInfo getFolloUpsCmnt(Ctx ctx, int ChatID, int RecordID, int SubscriberID, int TableID, int WinID, int RoleID, int PageSize, int page)
        {
            List<HomeFolloUps> lstFollUps = new List<HomeFolloUps>();
            List<FllUsrImages> lstUImg = new List<FllUsrImages>();
            HomeFolloUpsInfo objFllupsInfo = new HomeFolloUpsInfo();
            //List<HomeFolloUpsInfo> objFllups = new List<HomeFolloUpsInfo>();
            try
            {
                // To get the Folloups details
                StringBuilder SqlQuery = new StringBuilder();
                SqlQuery.Append("select inn.ChatID, inn.EntryID,  CH.characterdata, ch.cm_chatentry_id,");
                if (ctx.GetAD_Language() != Env.GetBaseAD_Language())
                {
                    SqlQuery.Append("(Select name from AD_Window_Trl where AD_Window_ID= Aw.AD_Window_ID and AD_Language='" + ctx.GetAD_Language() + "') as WINNAME,");
                }
                else
                {
                    SqlQuery.Append("aw.DisplayName as WINNAME,");
                }

                SqlQuery.Append("  aw.AD_Window_ID,cs.AD_Table_ID,cs.RECOrd_ID, aw.help,au.Name AS NAME,cs.cm_subscribe_ID, ch.created,ai.ad_image_id, ai.binarydata as UsrImg,  adi.binarydata as WinImg ,CH.createdby from (select * from (select CH.cm_chat_id as ChatID,  max(CE.cm_chatentry_id)as EntryID")
                        .Append("  from cm_chatentry CE join cm_chat CH on CE.cm_chat_id= CH.cm_chat_id ")
                        .Append("  JOIN cm_subscribe CS  ON (CH.ad_table_id= CS.ad_table_id) AND (CH.record_id = CS.record_id)")
                        .Append("  where cs.createdby=" + ctx.GetAD_User_ID() + " group by CH.cm_chat_id order by entryID )inn1 ) inn ")
                        .Append("  JOIN cm_chatentry CH on inn.ChatID= ch.cm_chat_id ")
                        .Append("  JOIN cm_chat CMH on (cmh.cm_chat_id= inn.chatid)")
                        .Append("  JOIN cm_subscribe CS  ON (CMH.ad_table_id= CS.ad_table_id) AND (CMH.record_id = CS.record_id)")
                        .Append("  Join ad_user Au on au.ad_user_id= CH.createdBy")
                        .Append("  left outer JOIN ad_image AI on(ai.ad_image_id=au.ad_image_id)")
                        .Append("  join ad_window AW on(cs.ad_window_id= aw.ad_window_id) left outer  JOIN ad_image adi on(adi.ad_image_id= aw.ad_image_id)  where cs.createdby=" + ctx.GetAD_User_ID())
                        .Append("  AND cmh.cm_chat_id=" + ChatID)
                        .Append("  order by inn.EntryID desc,ch.cm_chatentry_id asc");

                dsData = new DataSet();
                dsData = DB.ExecuteDataset(SqlQuery.ToString());
                if (dsData != null)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        var Fllps = new HomeFolloUps();
                        Fllps.ChatID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ChatID"].ToString());
                        Fllps.ChatEntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_chatentry_id"].ToString());
                        Fllps.EntryID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["EntryID"].ToString());
                        Fllps.WinID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Window_ID"].ToString());
                        Fllps.TableID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Table_ID"].ToString());
                        Fllps.RecordID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["Record_ID"].ToString());
                        Fllps.SubscriberID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["cm_subscribe_ID"].ToString());
                        Fllps.ChatData = dsData.Tables[0].Rows[i]["characterdata"].ToString();
                        Fllps.Name = dsData.Tables[0].Rows[i]["NAME"].ToString();
                        Fllps.WinName = dsData.Tables[0].Rows[i]["WINNAME"].ToString();
                        Fllps.AD_User_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["CREATEDBY"].ToString());

                        DateTime _createdDate = new DateTime();
                        if (dsData.Tables[0].Rows[i]["created"].ToString() != null && dsData.Tables[0].Rows[i]["created"].ToString() != "")
                        {
                            _createdDate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["created"].ToString());
                            DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                            _createdDate = _format;

                            Fllps.Cdate = _format;
                        }

                        //if (dsData.Tables[0].Rows[i]["created"].ToString() != "")
                        //{
                        //    Fllps.Cdate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["created"]);
                        //}

                        //if (lstUImg.Where(a => a.AD_Image_ID == uimgId).Count() == 0)
                        //{
                        //    var uimsg = new FllUsrImages();
                        //    uimsg.AD_Image_ID = uimgId;
                        //    MImage mimg = new MImage(ctx, uimgId, null);
                        //    var imgfll = mimg.GetThumbnailURL(46, 46);
                        //    if (imgfll.ToString() == "FileDoesn'tExist" || imgfll.ToString() == "NoRecordFound")
                        //    {

                        //    }
                        //    else
                        //    {
                        //        uimsg.UserImg = imgfll;
                        //    }
                        //    lstUImg.Add(uimsg);
                        //}

                        int uimgId = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["ad_image_id"].ToString());
                        Fllps.AD_Image_ID = uimgId;

                        if (lstUImg.Where(a => a.AD_Image_ID == uimgId).Count() == 0)
                        {
                            var uimsg = new FllUsrImages();
                            uimsg.AD_Image_ID = uimgId;
                            MImage mimg = new MImage(ctx, uimgId, null);
                            var imgfll = mimg.GetThumbnailURL(46, 46);
                            uimsg.UserImg = imgfll;
                            lstUImg.Add(uimsg);
                        }

                        lstFollUps.Add(Fllps);
                    }
                    objFllupsInfo.lstUserImg = lstUImg;
                    objFllupsInfo.lstFollowups = lstFollUps;
                }
            }

            catch (Exception)
            {

            }
            return objFllupsInfo;
        }

        //Save Folloups Comment
        public void SaveFllupsCmnt(Ctx ctx, int ChatID, int SubscriberID, string txt)
        {
            MChat _chat = new MChat(ctx, ChatID, null);
            MChatEntry entry = new MChatEntry(_chat, txt);
            if (entry.Save())
            {
                //strQuery = "  UPDATE cm_subscribe SET isRead='N' WHERE isRead='Y' AND cm_subscribe_id=" + SubscriberID;
                //DB.ExecuteQuery(strQuery);
            }

        }

        #endregion

        # region Start Notice
        //Count of notice
        public int getNoticeCnt(Ctx ctx)
        {
            int ncnt = 0;
            try
            {
                //To get Notice Count
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL("SELECT count(AD_Note_ID) FROM AD_Note "
                    , "AD_Note", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += " AND AD_User_ID IN (" + ctx.GetAD_User_ID() + ")"
                  + " AND Processed='N'";

                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                ncnt = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception)
            {

            }
            return ncnt;
        }
        //List of Notice
        public List<HomeNotice> getHomeNotice(Ctx ctx, int PageSize, int page)
        {
            List<HomeNotice> lstNts = new List<HomeNotice>();
            try
            {
                //Notice
                //strQuery = "SELECT  substr(AD_Note.textmsg,0,100) as Title, AD_Note.textmsg as Description , AD_Note.Created  as dbDate,AD_Note.ad_table_id"
                //+ ",AD_Table.tablename,ad_note_id as ID   FROM AD_Note JOIN AD_Table on Ad_Table.Ad_Table_ID=Ad_Note.Ad_Table_ID ";

                //                strQuery = @"SELECT SUBSTR(AD_Note.textmsg,0,100) AS Title,
                //                            AD_Note.textmsg    AS Description ,
                //                            AD_Note.Created    AS dbDate,
                //                            AD_Message.msgtext as MsgType,
                //                            AD_Note.AD_Table_ID , 
                //                            AD_Note.Record_ID,
                //                            (SELECT  AD_Table.TableName FROM  AD_Table WHERE  AD_Table.TableName='AD_Note') TableName,
                //                            (SELECT  AD_Table.Ad_Window_ID FROM  AD_Table WHERE  AD_Table.TableName='AD_Note') AD_Window_ID,
                //                            AD_Note.AD_Note_ID
                //                            FROM AD_Note INNER JOIN AD_Message ON AD_Message.AD_Message_ID=AD_Note.AD_Message_ID";
                strQuery = @"SELECT SUBSTR(AD_Note.textmsg,0,100) AS Title,
                              AD_Note.textmsg                    AS Description ,
                              AD_Note.Created                    AS dbDate,
                              AD_Message.msgtext                 AS MsgType,
                              AD_Note.AD_Table_ID ,
                              AD_Note.Record_ID,
                              (SELECT AD_Table.TableName FROM AD_Table WHERE AD_Table.TableName='AD_Note'
                              ) TableName,
                              (SELECT AD_Table.Ad_Window_ID
                              FROM AD_Table
                              WHERE AD_Table.TableName='AD_Note'
                              ) AD_Window_ID,
                              AD_Note.AD_Note_ID
                            FROM AD_Note
                            INNER JOIN AD_Message
                            ON AD_Message.AD_Message_ID         =AD_Note.AD_Message_ID";
                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "AD_Note", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

                strQuery += "  AND AD_Note.AD_User_ID IN (0," + ctx.GetAD_User_ID() + ")"
                + " AND AD_Note.Processed='N' ORDER BY AD_Note.Created DESC";

                int PResultTableID = MTable.Get_Table_ID("AD_PInstance_Result");

                dsData = VIS.DBase.DB.ExecuteDatasetPaging(strQuery, page, PageSize);
                dsData = VAdvantage.DataBase.DB.SetUtcDateTime(dsData);

                object windowID = DB.ExecuteScalar("SELECT AD_Window_ID FROM AD_Window WHERE Name='Process Result'");

                if (dsData != null)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        var Alrt = new HomeNotice();
                        Alrt.AD_Note_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Note_ID"].ToString());
                        Alrt.AD_Table_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Table_ID"].ToString());
                        Alrt.AD_Window_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Window_ID"].ToString());
                        Alrt.Record_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["Record_ID"].ToString());
                        Alrt.MsgType = dsData.Tables[0].Rows[i]["MsgType"].ToString();
                        //below code is to  remove HTML tags from Title
                        string patternTitle = "(</?([^>/]*)/?>)";
                        Alrt.Title = dsData.Tables[0].Rows[i]["Title"].ToString();
                        MatchCollection matchesTitle = Regex.Matches(Alrt.Title, patternTitle);
                        if (matchesTitle.Count > 0)
                        {
                            Alrt.Title = Regex.Replace(Alrt.Title, patternTitle, string.Empty);
                        }
                            Alrt.TableName = dsData.Tables[0].Rows[i]["TableName"].ToString();
                        if (PResultTableID == Alrt.AD_Table_ID)
                        {
                            Alrt.ProcessWindowID = Util.GetValueOfInt(windowID);
                            Alrt.ProcessTableName = "AD_PInstance_Result";
                            Alrt.SpecialTable = true;
                        }
                        else
                        {
                            Alrt.SpecialTable = false;
                        }

                        //below code is to  remove HTML tags from Description
                        string patternDesc = "(</?([^>/]*)/?>)";
                        Alrt.Description = dsData.Tables[0].Rows[i]["Description"].ToString();
                        MatchCollection matchesDesc = Regex.Matches(Alrt.Description, patternDesc);
                        if (matchesDesc.Count > 0)
                        {
                            Alrt.Description = Regex.Replace(Alrt.Description, patternDesc, string.Empty);
                         }


                            DateTime _createdDate = new DateTime();
                        if (dsData.Tables[0].Rows[i]["dbDate"].ToString() != null && dsData.Tables[0].Rows[i]["dbDate"].ToString() != "")
                        {
                            _createdDate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["dbDate"].ToString());
                            DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                            _createdDate = _format;
                            Alrt.CDate = _format;
                        }
                        lstNts.Add(Alrt);
                    }
                }
            }
            catch (Exception)
            {
            }
            return lstNts;
        }
        //Approve Notice
        public bool ApproveNotice(Ctx ctx, int Ad_Note_ID, bool isAcknowldge)
        {
            MNote objNote = new MNote(ctx, Ad_Note_ID, null);
            objNote.SetProcessed(isAcknowldge);
            if (objNote.Save())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region StartRequest
        //count of request
        public int getRequestCnt(Ctx ctx)
        {
            int ncnt = 0;
            try
            {
                //To Get Request count
                //strQuery = " SELECT  count(R_Request.r_request_id) FROM R_Request  inner join  r_requesttype rt on R_Request.r_requesttype_id=rt.r_requesttype_ID";
                strQuery = @" SELECT  count(R_Request.r_request_id) FROM R_Request
                        LEFT OUTER JOIN C_BPartner
                        ON R_Request.C_BPartner_ID=C_BPartner.C_BPartner_ID
                        LEFT OUTER JOIN r_requesttype rt
                        ON R_Request.r_requesttype_id = rt.r_requesttype_ID
                        LEFT OUTER JOIN R_Status rs
                        ON rs.R_Status_ID=R_request.R_Status_ID
                        LEFT OUTER JOIN ad_ref_list adl
                        ON adl.Value=R_Request.Priority
                        JOIN AD_reference adr
                        ON adr.AD_Reference_ID=adl.AD_Reference_ID ";

                strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "R_Request", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                strQuery += "  AND adr.Name='_PriorityRule'  AND ( R_Request.SalesRep_ID =" + ctx.GetAD_User_ID() + " OR R_Request.AD_Role_ID =" + ctx.GetAD_Role_ID() + ")"
                 + " AND R_Request.Processed ='N'"
                + " AND (R_Request.R_Status_ID IS NULL OR R_Request.R_Status_ID IN (SELECT R_Status_ID FROM R_Status WHERE IsClosed='N'))";


                dsData = new DataSet();
                dsData = DB.ExecuteDataset(strQuery);
                ncnt = Util.GetValueOfInt(dsData.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception)
            {

            }
            return ncnt;
        }
        //List of Request
        public List<HomeRequest> getHomeRequest(Ctx ctx, int PageSize, int page)
        {
            List<HomeRequest> lstAlerts = new List<HomeRequest>();


            //strQuery = "SELECT C_BPartner.Name ,rt.Name As CaseType,R_Request.DocumentNo , R_Request.Summary ,R_Request.StartDate ,R_Request.DateNextAction,R_Request.Created,"
            //+ "R_Request.R_Request_ID,R_Request.Priority as PriorityID,adl.Name as Priority,rs.name As Status,"
            //+ "(SELECT  AD_Table.TableName FROM  AD_Table WHERE  AD_Table.TableName='R_Request') TableName,"
            //+ "(SELECT  AD_Table.Ad_Window_ID FROM  AD_Table WHERE  AD_Table.TableName='R_Request') AD_Window_ID  FROM R_Request"
            //+ " INNER JOIN C_BPartner on R_Request.C_BPartner_ID=C_BPartner.C_BPartner_ID"
            //+ " INNER JOIN r_requesttype rt ON R_Request.r_requesttype_id = rt.r_requesttype_ID"
            //+ " Left outer JOIN  R_Status rs on rs.R_Status_ID=R_request.R_Status_ID"
            //+ " Left Outer JOIN  ad_ref_list adl on adl.Value=R_Request.Priority"
            //+ " JOIN  AD_reference adr on adr.AD_Reference_ID=adl.AD_Reference_ID";

            //            strQuery = @" SELECT C_BPartner.Name ,
            //                          rt.Name AS CaseType,
            //                          R_Request.DocumentNo ,
            //                          R_Request.Summary ,
            //                          R_Request.StartDate ,
            //                          R_Request.DateNextAction,
            //                          R_Request.Created,
            //                          R_Request.R_Request_ID,
            //                          R_Request.Priority AS PriorityID,
            //                          adl.Name           AS Priority,
            //                          rs.name            AS Status,
            //                          (SELECT AD_Table.TableName FROM AD_Table WHERE AD_Table.TableName='R_Request'
            //                          ) TableName,
            //                          (SELECT AD_Table.Ad_Window_ID
            //                          FROM AD_Table
            //                          WHERE AD_Table.TableName='R_Request'
            //                          ) AD_Window_ID
            //                        FROM R_Request
            //                        INNER JOIN C_BPartner
            //                        ON R_Request.C_BPartner_ID=C_BPartner.C_BPartner_ID
            //                        INNER JOIN r_requesttype rt
            //                        ON R_Request.r_requesttype_id = rt.r_requesttype_ID
            //                        LEFT OUTER JOIN R_Status rs
            //                        ON rs.R_Status_ID=R_request.R_Status_ID
            //                        LEFT OUTER JOIN ad_ref_list adl
            //                        ON adl.Value=R_Request.Priority
            //                        JOIN AD_reference adr
            //                        ON adr.AD_Reference_ID=adl.AD_Reference_ID ";


            strQuery = @" SELECT C_BPartner.Name ,
                          rt.Name AS CaseType,
                          R_Request.DocumentNo ,
                          R_Request.Summary ,
                          R_Request.StartDate ,
                          R_Request.DateNextAction,
                          R_Request.Created,
                          R_Request.R_Request_ID,
                          R_Request.Priority AS PriorityID,
                          adl.Name           AS Priority,
                          rs.name            AS Status,
                          (SELECT AD_Table.TableName FROM AD_Table WHERE AD_Table.TableName='R_Request'
                          ) TableName,
                          (SELECT AD_Table.Ad_Window_ID
                          FROM AD_Table
                          WHERE AD_Table.TableName='R_Request'
                          ) AD_Window_ID
                        FROM R_Request
                        LEFT OUTER JOIN C_BPartner
                        ON R_Request.C_BPartner_ID=C_BPartner.C_BPartner_ID
                        LEFT OUTER JOIN r_requesttype rt
                        ON R_Request.r_requesttype_id = rt.r_requesttype_ID
                        LEFT OUTER JOIN R_Status rs
                        ON rs.R_Status_ID=R_request.R_Status_ID
                        LEFT OUTER JOIN ad_ref_list adl
                        ON adl.Value=R_Request.Priority
                        JOIN AD_reference adr
                        ON adr.AD_Reference_ID=adl.AD_Reference_ID ";



            strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "R_Request", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            strQuery += "  AND adr.Name='_PriorityRule' AND ( R_Request.SalesRep_ID =" + ctx.GetAD_User_ID() + " OR R_Request.AD_Role_ID =" + ctx.GetAD_Role_ID() + ")"
            + " AND R_Request.Processed ='N'  AND (R_Request.R_Status_ID IS NULL OR R_Request.R_Status_ID IN (SELECT R_Status_ID FROM R_Status WHERE IsClosed='N')) ORDER By R_Request.Updated, R_Request.Priority ";
            // change to sort Requests based on updated date and time


            //Request
            //strQuery = " SELECT rt.Name ,R_Request.Summary , R_Request.StartDate ,R_Request.DateNextAction,DateLastAction.Created"
            // + " R_Request.R_Request_ID  FROM R_Request  inner join  r_requesttype rt on R_Request.r_requesttype_id=rt.r_requesttype_ID";
            //strQuery = MRole.Get(ctx, ctx.GetAD_Role_ID()).AddAccessSQL(strQuery, "R_Request", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            //strQuery += " AND ( R_Request.SalesRep_ID =" + ctx.GetAD_User_ID() + " OR R_Request.AD_Role_ID =" + ctx.GetAD_Role_ID() + ")"
            //+ " AND R_Request.Processed ='N' AND (R_Request.DateNextAction IS NULL OR TRUNC(R_Request.DateNextAction, 'DD') <= TRUNC(SysDate, 'DD'))"
            //+ " AND (R_Request.R_Status_ID IS NULL OR R_Request.R_Status_ID IN (SELECT R_Status_ID FROM R_Status WHERE IsClosed='N'))";

            SqlParamsIn objSP = new SqlParamsIn();
            dsData = new DataSet();
            objSP.page = page;
            objSP.pageSize = PageSize;
            objSP.sql = strQuery;
            dsData = VIS.DBase.DB.ExecuteDatasetPaging(objSP.sql, objSP.page, objSP.pageSize);
            if (dsData != null)
            {
                dsData = VAdvantage.DataBase.DB.SetUtcDateTime(dsData);
                for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                {
                    var Alrt = new HomeRequest();
                    Alrt.R_Request_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["R_Request_ID"].ToString());
                    Alrt.AD_Window_ID = Util.GetValueOfInt(dsData.Tables[0].Rows[i]["AD_Window_ID"].ToString());
                    Alrt.TableName = dsData.Tables[0].Rows[i]["TableName"].ToString();
                    Alrt.Name = dsData.Tables[0].Rows[i]["Name"].ToString();
                    Alrt.CaseType = dsData.Tables[0].Rows[i]["CaseType"].ToString();
                    Alrt.DocumentNo = dsData.Tables[0].Rows[i]["DocumentNo"].ToString();
                    Alrt.Status = dsData.Tables[0].Rows[i]["Status"].ToString();
                    Alrt.Priority = dsData.Tables[0].Rows[i]["Priority"].ToString();
                    Alrt.Summary = dsData.Tables[0].Rows[i]["Summary"].ToString();


                    DateTime _DateNextAction = new DateTime();
                    if (dsData.Tables[0].Rows[i]["DateNextAction"].ToString() != null && dsData.Tables[0].Rows[i]["DateNextAction"].ToString() != "")
                    {
                        _DateNextAction = Convert.ToDateTime(dsData.Tables[0].Rows[i]["DateNextAction"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_DateNextAction.Year, _DateNextAction.Month, _DateNextAction.Day, _DateNextAction.Hour, _DateNextAction.Minute, _DateNextAction.Second), DateTimeKind.Utc);
                        _DateNextAction = _format;
                        Alrt.NextActionDate = _format;
                    }

                    DateTime _createdDate = new DateTime();
                    if (dsData.Tables[0].Rows[i]["created"].ToString() != null && dsData.Tables[0].Rows[i]["created"].ToString() != "")
                    {
                        _createdDate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["created"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                        _createdDate = _format;
                        Alrt.CreatedDate = _format;
                    }



                    DateTime _StartDate = new DateTime();
                    if (dsData.Tables[0].Rows[i]["StartDate"].ToString() != null && dsData.Tables[0].Rows[i]["StartDate"].ToString() != "")
                    {
                        _StartDate = Convert.ToDateTime(dsData.Tables[0].Rows[i]["StartDate"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_StartDate.Year, _StartDate.Month, _StartDate.Day, _StartDate.Hour, _StartDate.Minute, _StartDate.Second), DateTimeKind.Utc);
                        _StartDate = _format;
                        Alrt.StartDate = _format;
                    }
                    lstAlerts.Add(Alrt);
                }
            }
            return lstAlerts;
        }
        #endregion



        public List<FavNode> GetBarNodes(List<VTreeNode> nodes)
        {
            List<FavNode> items = new List<FavNode>();
            FavNode itm = null;

            if (nodes == null || nodes.Count == 0)
                return items;

            for (int i = 0; i < nodes.Count; i++)
            {
                itm = new FavNode();
                itm.Name = nodes[i].SetName;
                itm.Action = nodes[i].GetAction();
                itm.WindowID = nodes[i].AD_Window_ID;
                itm.FormID = nodes[i].AD_Form_ID;
                itm.ProcessID = nodes[i].AD_Process_ID;
                itm.NodeID = nodes[i].GetNode_ID();
                items.Add(itm);
            }
            return items;


        }


        public string SetNodeFavourite(int nodeID, Ctx ctx)
        {
            int AD_Tree_ID = DB.GetSQLValue(null,
                        "SELECT COALESCE(r.AD_Tree_Menu_ID, ci.AD_Tree_Menu_ID)"
                       + "FROM AD_ClientInfo ci"
                       + " INNER JOIN AD_Role r ON (ci.AD_Client_ID=r.AD_Client_ID) "
                       + "WHERE AD_Role_ID=" + ctx.GetAD_Role_ID());
            string sql = "INSERT INTO AD_TreeBar "
                                 + "(AD_Tree_ID,AD_User_ID,Node_ID, "
                                 + "AD_Client_ID,AD_Org_ID, "
                                 + "IsActive,Created,CreatedBy,Updated,UpdatedBy)VALUES (" + AD_Tree_ID + "," + ctx.GetAD_User_ID() + "," + nodeID + ","
                                 + ctx.GetAD_Client_ID() + "," + ctx.GetAD_Org_ID() + ","
                                 + "'Y',SysDate," + ctx.GetAD_User_ID() + ",SysDate," + ctx.GetAD_User_ID() + ")";
            //	if already exist, will result in ORA-00001: unique constraint 
            return DB.ExecuteQuery(sql, null).ToString();

        }

        public string RemoveNodeFavourite(int nodeID, Ctx ctx)
        {
            int AD_Tree_ID = DB.GetSQLValue(null,
                        "SELECT COALESCE(r.AD_Tree_Menu_ID, ci.AD_Tree_Menu_ID)"
                       + "FROM AD_ClientInfo ci"
                       + " INNER JOIN AD_Role r ON (ci.AD_Client_ID=r.AD_Client_ID) "
                       + "WHERE AD_Role_ID=" + ctx.GetAD_Role_ID());
            string sql = sql = "DELETE FROM AD_TreeBar WHERE AD_Tree_ID=" + AD_Tree_ID + " AND AD_User_ID=" + ctx.GetAD_User_ID()
                       + " AND Node_ID=" + nodeID;
            return DB.ExecuteQuery(sql, null).ToString();

        }

        public string GetSubscriptionDaysLeft(string url)
        {
            object key = null;
            try
            {
                key = System.Web.Configuration.WebConfigurationManager.AppSettings["TrialMessage"];

                if (key == null || key.ToString() == "" || key.ToString() == "N")
                {
                    return "True";
                }

            }
            catch
            {
                return "True";
            }

            string retUrl = "";
            BaseLibrary.CloudService.ServiceSoapClient cloud = null;

            try
            {
                cloud = VAdvantage.Classes.ServerEndPoint.GetCloudClient();

                if (cloud == null || cloud.ToString() == "")
                {
                    //Response.Redirect("http://demo.viennaadvantage.com",true);
                    retUrl = GenerateUrl(url);
                    return retUrl;
                }
            }
            catch
            {
            }
            try
            {
                //System.Net.ServicePointManager.Expect100Continue = false;
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    retUrl = cloud.GetSubscriptionDays(url, SecureEngine.Encrypt(System.Web.Configuration.WebConfigurationManager.AppSettings["accesskey"].ToString()));
                }
                catch
                {

                }
                cloud.Close();
            }
            catch
            {

                return retUrl;
            }

            return retUrl;
        }

        private static string GenerateUrl(string urlIn)
        {
            string urlOut = "";

            if (VAdvantage.Classes.ServerEndPoint.IsSSLEnabled())
            {

                if (urlIn.Contains("http://"))
                {
                    urlOut = urlIn.Replace("http://", "https://");
                    //  Response.Redirect(url, false);
                }
                else if (urlIn.Contains("https://"))
                {
                    urlOut = "";
                }
                else
                {
                    urlOut = "https://" + urlIn;
                }
            }
            return urlOut;
        }

        public void InitializeLog(Ctx ct)
        {
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset("SELECT MaintainAccording,TraceLevel FROM AD_ClientInfo WHERE AD_Client_ID=" + ct.GetAD_Client_ID());
            }
            catch
            {
            }
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            string maintainAccording = Util.GetValueOfString(ds.Tables[0].Rows[0][0]);
            if (string.IsNullOrEmpty(maintainAccording))
            {
                return;
            }
            else if (maintainAccording == "T")
            {
                string traceLevel = Util.GetValueOfString(ds.Tables[0].Rows[0][1]);
                if (!string.IsNullOrEmpty(traceLevel) && Util.GetValueOfInt(traceLevel) < 9999)
                {
                    long hid = DateTime.Now.Ticks;
                    ct.SetContext("#LogHandler", hid.ToString());
                // VAdvantage.Logging.VLogMgt.Initialize(true, Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, (ct.GetAD_Org_Name() == "*" ? "Star" : ct.GetAD_Org_Name()) + "_" + ct.GetAD_User_Name()), Util.GetValueOfInt(traceLevel), hid, ct);
                }
            }
            else if (maintainAccording == "U")
            {
                string traceLevel = ct.GetContext("TraceLevel");
                if (!string.IsNullOrEmpty(traceLevel) && Util.GetValueOfInt(traceLevel) < 9999)
                {
                    long hid = DateTime.Now.Ticks;
                    ct.SetContext("#LogHandler", hid.ToString());
              //   VAdvantage.Logging.VLogMgt.Initialize(true, Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, (ct.GetAD_Org_Name() == "*" ? "Star" : ct.GetAD_Org_Name()) + "_" + ct.GetAD_User_Name()), Util.GetValueOfInt(traceLevel), hid, ct);
                }
            }

        }
    }


    public class FavNode
    {
        public string Name
        {
            get;
            set;

        }
        public string Action
        {
            get;
            set;

        }
        public int WindowID
        {
            get;
            set;

        }
        public int FormID
        {
            get;
            set; 

        }
        public int ProcessID
        {
            get;
            set;

        }
        public int NodeID
        {
            get;
            set;
        }


    }
}