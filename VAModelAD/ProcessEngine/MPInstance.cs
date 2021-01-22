/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPInstance
 * Purpose        : Process Instance Model
 * Class Used     : X_VAF_JInstance
 * Chronological    Development
 * Raghunandan     27-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.IO;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;

namespace VAdvantage.ProcessEngine
{
    public class MPInstance : X_VAF_JInstance
    {
        #region private Variable
        // Result OK = 1			
        public static int RESULT_OK = 1;
        // Result FALSE = 0		
        public static int RESULT_ERROR = 0;
        MPInstancePara[] _parameter = null;
        //	Log Entries				
        private List<MPInstanceLog> _log = new List<MPInstanceLog>();

        //private string recIds = "";

        #endregion

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_JInstance_ID">instance or 0</param>
        /// <param name="ignored">no transaction support</param>
        public MPInstance(Ctx ctx, int VAF_JInstance_ID, string ignored)
            : base(ctx, VAF_JInstance_ID, null)
        {
            if (VAF_JInstance_ID == 0)
            {
                int VAF_Role_ID = ctx.GetVAF_Role_ID();
                if (VAF_Role_ID != 0)
                    SetVAF_Role_ID(VAF_Role_ID);
                SetIsProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">dataRow</param>
        /// <param name="ignored">no transaction support</param>
        public MPInstance(Ctx ctx, DataRow dr, String ignored)
            : base(ctx, dr, null)
        {
        }

        /// <summary>
        /// Create Process Instance from Process and create parameters
        /// </summary>
        /// <param name="process">process</param>
        /// <param name="Record_ID">Record</param>
        public MPInstance(MProcess process, int Record_ID)
            : this(process.GetCtx(), 0, null)
        {
            if (process.Get_ID() <= 0)
                return;
            SetVAF_Job_ID(process.GetVAF_Job_ID());
            SetRecord_ID(Record_ID);
            SetVAF_UserContact_ID(process.GetCtx().GetVAF_UserContact_ID());

            // PO Save below
            if (!Save())		//	need to save for parameters
                throw new ArgumentException("Cannot Save");

            //	Set Parameter Base Info
            MProcessPara[] para = process.GetParameters();
            for (int i = 0; i < para.Length; i++)
            {
                MPInstancePara pip = new MPInstancePara(this, para[i].GetSeqNo());
                pip.SetParameterName(para[i].GetColumnName());
                pip.SetInfo(para[i].GetName());
                pip.Save(); //  PO Save
            }
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Job_ID">Process ID</param>
        /// <param name="Record_ID">record</param>
        public MPInstance(Ctx ctx, int VAF_Job_ID, int Record_ID)
            : this(ctx, 0, null)
        {
            SetVAF_Job_ID(VAF_Job_ID);
            SetRecord_ID(Record_ID);
            SetVAF_UserContact_ID(ctx.GetVAF_UserContact_ID());
            SetIsProcessing(false);
        }

        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <returns>parameter array</returns>
        public MPInstancePara[] GetParameters()
        {
            if (_parameter != null)
                return _parameter;

            List<MPInstancePara> list = new List<MPInstancePara>();
            string strSql = "SELECT * FROM VAF_JInstance_Para WHERE VAF_JInstance_ID=@instanceid";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@instanceid", GetVAF_JInstance_ID());
            DataSet ds = DataBase.DB.ExecuteDataset(strSql, param);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(new MPInstancePara(GetCtx(), dr, null));
            }

            _parameter = new MPInstancePara[list.Count];
            _parameter = list.ToArray();
            return _parameter;
        }

        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>array of logs</returns>
        public MPInstanceLog[] GetLog()
        {
            //	load it from DB
            _log.Clear();
            String sql = "SELECT * FROM VAF_JInstance_Log WHERE VAF_JInstance_ID=" + GetVAF_JInstance_ID() + " ORDER BY Log_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    _log.Add(new MPInstanceLog(dr));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MPInstanceLog[] retValue = new MPInstanceLog[_log.Count];
            retValue = _log.ToArray();
            return retValue;
        }

        /// <summary>
        /// @param P_Date 
        /// </summary>
        /// <param name="P_Date">date</param>
        /// <param name="P_ID">id</param>
        /// <param name="P_Number">Number</param>
        /// <param name="P_Msg">msg</param>
        public void AddLog(DateTime P_Date, int P_ID, Decimal P_Number, String msg)
        {
            MPInstanceLog logEntry = new MPInstanceLog(GetVAF_JInstance_ID(), _log.Count + 1,
                P_Date, P_ID, P_Number, msg);
            _log.Add(logEntry);
            //	save it to DB ?
            //	log.save();
        }

        /// <summary>
        /// Set VAF_Job_ID.
        /// Check Role if process can be performed
        /// </summary>
        /// <param name="VAF_Job_ID">process</param>
        public new void SetVAF_Job_ID(int VAF_Job_ID)
        {
            if (VAF_Job_ID <= 0)
                return;
            Console.WriteLine(VAF_Job_ID.ToString());
            int VAF_Role_ID = GetCtx().GetVAF_Role_ID();
            if (VAF_Role_ID != 0)
            {
                MRole role = MRole.Get(GetCtx(), VAF_Role_ID);
                bool? access = role.GetProcessAccess(VAF_Job_ID, VAF_Role_ID);
                if (access == null)
                    throw new Exception("Cannot access Process " + VAF_Job_ID
                        + " with Role: " + role.Get_Value("Name"));
            }
            base.SetVAF_Job_ID(VAF_Job_ID);
        }

        /// <summary>
        /// Set Record ID.
        /// direct internal record ID
        /// </summary>
        /// <param name="Record_ID">record</param>
        public new void SetRecord_ID(int Record_ID)
        {
            if (Record_ID < 0)
            {
                Record_ID = 0;
            }
            Set_ValueNoCheck("Record_ID", (int)Record_ID);
        }

        //public void SetRecordIds(string recIDs)
        //{
        //    this.recIds = recIds;
        //}

        //public string GetRecordIds()
        //{
        //    return recIds;
        //}

        /// <summary>
        /// String Representation
        /// @see java.lang.Object#toString()
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPInstance[")
                .Append(Get_ID())
                .Append(",OK=").Append(IsOK());
            String msg = GetErrorMsg();
            if (msg != null && msg.Length > 0)
                sb.Append(msg);
            sb.Append("]");
            return sb.ToString();

        }

        /// <summary>
        /// Dump Log
        /// </summary>
        public void Log()
        {
            log.Info(ToString());
            MPInstanceLog[] pil = GetLog();
            for (int i = 0; i < pil.Length; i++)
            {
                log.Info(i + "=" + pil[i]);
            }
        }

        /// <summary>
        /// Is it OK
        /// </summary>
        /// <returns>Result == OK</returns>
        public bool IsOK()
        {
            return GetResult() == RESULT_OK;
        }

        /// <summary>
        /// 	Set Result
        /// </summary>
        /// <param name="ok">ok </param>
        public void SetResult(bool ok)
        {
            base.SetResult(ok ? RESULT_OK : RESULT_ERROR);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord && GetAD_Session_ID() == 0)
            {
                MSession session = MSession.Get(GetCtx(), true);
                int AD_Session_ID = session.GetAD_Session_ID();
                SetAD_Session_ID(AD_Session_ID);
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Update Statistics
            if (!newRecord
                && !IsProcessing()
                && Is_ValueChanged("IsProcessing"))
            {
                //long ms = System.currentTimeMillis() - GetCreated().getTime();
                long ms = CommonFunctions.CurrentTimeMillis() - GetCreated().Millisecond;
                int seconds = (int)(ms / 1000);
                if (seconds < 1)
                {
                    seconds = 1;
                }
                MProcess prc = MProcess.Get(GetCtx(), GetVAF_Job_ID());
                prc.AddStatistics(seconds);
                if (prc.Get_ID() != 0 && prc.Save())
                {
                    log.Fine("afterSave - Process Statistics updated Sec=" + seconds);
                }
                else
                {
                    log.Warning("afterSave - Process Statistics not updated");
                }
            }
            return success;
        }

    }

}
