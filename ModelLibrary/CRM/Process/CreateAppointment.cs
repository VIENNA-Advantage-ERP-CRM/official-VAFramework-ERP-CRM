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

namespace VAdvantage.Process
{
    public class CreateAppointment : SvrProcess
    {
        #region Private Variables
        string msg = "";
        string sql = "";
        int C_ProjectTask_ID = 0;
        int C_Project_ID = 0;
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
            sql = "select tablename from ad_table where ad_table_id = " + GetTable_ID();
            string tableName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (tableName.ToUpper() == "C_PROJECTTASK")
            {
                C_ProjectTask_ID = GetRecord_ID();
                GenerateAppointment();
            }
            else if (tableName.ToUpper() == "C_PROJECT")
            {
                C_Project_ID = GetRecord_ID();
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
            sql = "select C_ProjectPhase_ID from c_projectphase where c_project_id = " + C_Project_ID + " and isactive = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    sql = "select c_projecttask_id from c_projecttask where c_projectphase_id = " + Util.GetValueOfInt(idr[0]) + " and isactive = 'Y'";
                    DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            C_ProjectTask_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectTask_ID"]);
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
            if (C_ProjectTask_ID != 0)
            {
                sql = "select name from c_campaign where c_campaign_id = (select c_campaign_id from c_project where c_project_id = (select c_project_id "
                + " from c_projectphase where c_projectphase_id = (select c_projectphase_id from c_projecttask where c_projecttask_id = " + C_ProjectTask_ID + ")))";
                string name = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                VAdvantage.Model.MProjectTask task = new VAdvantage.Model.MProjectTask(GetCtx(), C_ProjectTask_ID, Get_Trx());

                int AppointmentsInfo_ID = Util.GetValueOfInt(task.GetAppointmentsInfo_ID());
                sql = "select count(*) from AppointmentsInfo where AppointmentsInfo_ID = " + AppointmentsInfo_ID;
                int res = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (res == 0)
                {
                    string subject = name + "_" + task.GetName();
                    string desc = task.GetDescription();
                    int AD_User_ID = task.GetSalesRep_ID();
                    //DateTime? sDate = task.
                    if (AD_User_ID != 0)
                    {
                        VAdvantage.Model.X_AppointmentsInfo appoint = new VAdvantage.Model.X_AppointmentsInfo(GetCtx(), 0, Get_Trx());
                        appoint.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                        appoint.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                        appoint.SetSubject(subject);
                        appoint.SetStartDate(task.GetStartDate());
                        appoint.SetEndDate(task.GetEndDate());
                        appoint.SetStatus(3);
                        appoint.SetDescription(task.GetDescription());
                        appoint.SetAppointmentCategory_ID(1000000);
                        appoint.SetIsRead(true);
                        appoint.SetAD_User_ID(AD_User_ID);
                        appoint.SetIsPrivate(false);
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
                    int AD_User_ID = task.GetSalesRep_ID();
                    //DateTime? sDate = task.
                    if (AD_User_ID != 0)
                    {
                        VAdvantage.Model.X_AppointmentsInfo appoint = new VAdvantage.Model.X_AppointmentsInfo(GetCtx(), Util.GetValueOfInt(task.GetAppointmentsInfo_ID()), Get_Trx());
                        appoint.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                        appoint.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                        appoint.SetSubject(subject);
                        appoint.SetStartDate(task.GetStartDate());
                        appoint.SetEndDate(task.GetEndDate());
                        appoint.SetStatus(3);
                        appoint.SetDescription(task.GetDescription());
                        appoint.SetAppointmentCategory_ID(1000000);
                        appoint.SetIsRead(true);
                        appoint.SetAD_User_ID(AD_User_ID);
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
