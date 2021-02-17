/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : CreateAppointment
 * Purpose        : 
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   15-Mar-2012
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using VAdvantage.ProcessEngine;
//using ViennaAdvantage.Model;

namespace ViennaAdvantageServer.Process
{
    public class CreateAppointment : SvrProcess
    {
        #region Private Variables
        string msg = "";
        string sql = "";
        int VAB_ProjectJob_ID = 0;
        int VAB_Project_ID = 0;
        int count = 0;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void Prepare()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            sql = "select tablename from vaf_tableview where vaf_tableview_id = " + GetTable_ID();
            string tableName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (tableName.ToUpper() == "VAB_PROJECTJOB")
            {
                VAB_ProjectJob_ID = GetRecord_ID();
                GenerateAppointment();
            }
            else if (tableName.ToUpper() == "VAB_PROJECT")
            {
                VAB_Project_ID = GetRecord_ID();
                GetAllTasks();
                if (count > 0)
                {
                    msg = Msg.GetMsg(GetCtx(), "AppointmentsSaved");
                }
                else
                {
                    msg = Msg.GetMsg(GetCtx(), "NoAppointmentsToBeSaved");
                }
            }
            return msg;
        }

        /// <summary>
        /// Get All Tasks if Process run from Planning Tab.
        /// </summary>
        private void GetAllTasks()
        {
            sql = "select VAB_ProjectStage_ID from VAB_ProjectStage where VAB_Project_ID = " + VAB_Project_ID + " and isactive = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    sql = "select VAB_ProjectJob_id from VAB_ProjectJob where VAB_ProjectStage_id = " + Util.GetValueOfInt(idr[0]) + " and isactive = 'Y'";
                    DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            VAB_ProjectJob_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_ProjectJob_ID"]);
                            GenerateAppointment();
                        }
                    }
                }
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            catch
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
        }

        /// <summary>
        /// Generate Appointments if Owner is selected at Task Tab.
        /// </summary>
        private void GenerateAppointment()
        {
            if (VAB_ProjectJob_ID != 0)
            {
                sql = "select name from VAB_Promotion where VAB_Promotion_id = (select VAB_Promotion_id from VAB_Project where VAB_Project_ID = (select VAB_Project_ID "
                + " from VAB_ProjectStage where VAB_ProjectStage_id = (select VAB_ProjectStage_id from VAB_ProjectJob where VAB_ProjectJob_id = " + VAB_ProjectJob_ID + ")))";
                string name = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                VAdvantage.Model.MVABProjectJob task = new VAdvantage.Model.MVABProjectJob(GetCtx(), VAB_ProjectJob_ID, Get_Trx());

                int AppointmentsInfo_ID = Util.GetValueOfInt(task.GetAppointmentsInfo_ID());
                sql = "select count(*) from AppointmentsInfo where AppointmentsInfo_ID = " + AppointmentsInfo_ID;
                int res = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (res == 0)
                {
                    string subject = name + "_" + task.GetName();
                    string desc = task.GetDescription();
                    int VAF_UserContact_ID = task.GetSalesRep_ID();
                    //DateTime? sDate = task.
                    if (VAF_UserContact_ID != 0)
                    {
                        VAdvantage.Model.X_AppointmentsInfo appoint = new VAdvantage.Model.X_AppointmentsInfo(GetCtx(), 0, Get_Trx());
                        appoint.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                        appoint.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                        appoint.SetSubject(subject);
                        appoint.SetStartDate(task.GetStartDate());
                        appoint.SetEndDate(task.GetEndDate());
                        appoint.SetStatus(3);
                        appoint.SetDescription(task.GetDescription());
                        appoint.SetAppointmentCategory_ID(1000000);
                        appoint.SetIsRead(true);
                        appoint.SetVAF_UserContact_ID(VAF_UserContact_ID);
                        appoint.SetIsPrivate(false);
                        appoint.SetPriorityKey(5);
                        appoint.SetIsTask(true);
                        if (!appoint.Save(Get_Trx()))
                        {
                            log.SaveError("AppointmentNotSaved", "AppointmentNotSaved");
                            msg = Msg.GetMsg(GetCtx(), "AppointmentNotSaved");
                            //return msg;
                        }
                        task.SetAppointmentsInfo_ID(appoint.GetAppointmentsInfo_ID());
                        if (!task.Save(Get_Trx()))
                        {
                            log.SaveError("TaskNotSaved", "TaskNotSaved");
                        }
                        count = count++;
                        msg = Msg.GetMsg(GetCtx(), "AppointmentSaved");
                    }
                    else
                    {
                        msg = Msg.GetMsg(GetCtx(), "OwnerNotSelected");
                        // return msg;
                    }
                }
                else
                {
                    string subject = name + "_" + task.GetName();
                    string desc = task.GetDescription();
                    int VAF_UserContact_ID = task.GetSalesRep_ID();
                    //DateTime? sDate = task.
                    if (VAF_UserContact_ID != 0)
                    {
                        VAdvantage.Model.X_AppointmentsInfo appoint = new VAdvantage.Model.X_AppointmentsInfo(GetCtx(), Util.GetValueOfInt(task.GetAppointmentsInfo_ID()), Get_Trx());
                        appoint.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                        appoint.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                        appoint.SetSubject(subject);
                        appoint.SetStartDate(task.GetStartDate());
                        appoint.SetEndDate(task.GetEndDate());
                        appoint.SetStatus(3);
                        appoint.SetDescription(task.GetDescription());
                        appoint.SetAppointmentCategory_ID(1000000);
                        appoint.SetIsRead(true);
                        appoint.SetVAF_UserContact_ID(VAF_UserContact_ID);
                        appoint.SetIsPrivate(false);
                        appoint.SetIsTask(true);
                        if (!appoint.Save(Get_Trx()))
                        {
                            log.SaveError("AppointmentNotSaved", "AppointmentNotSaved");
                            msg = Msg.GetMsg(GetCtx(), "AppointmentNotSaved");
                            // return msg;
                        }
                        task.SetAppointmentsInfo_ID(appoint.GetAppointmentsInfo_ID());
                        if (!task.Save(Get_Trx()))
                        {
                            log.SaveError("TaskNotSaved", "TaskNotSaved");
                        }
                        count = count + 1;
                        msg = Msg.GetMsg(GetCtx(), "AppointmentSaved");
                    }
                    else
                    {
                        msg = Msg.GetMsg(GetCtx(), "OwnerNotSelected");
                        // return msg;
                    }
                }
            }
        }
    }
}
