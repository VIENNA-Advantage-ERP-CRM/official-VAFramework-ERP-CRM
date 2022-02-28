/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPInstance
 * Purpose        : Process Instance Model
 * Class Used     : X_AD_PInstance
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
    public class MPInstance : X_AD_PInstance
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
        /// <param name="AD_PInstance_ID">instance or 0</param>
        /// <param name="ignored">no transaction support</param>
        public MPInstance(Ctx ctx, int AD_PInstance_ID, string ignored)
            : base(ctx, AD_PInstance_ID, null)
        {
            if (AD_PInstance_ID == 0)
            {
                int AD_Role_ID = ctx.GetAD_Role_ID();
                if (AD_Role_ID != 0)
                    SetAD_Role_ID(AD_Role_ID);
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
            SetAD_Process_ID(process.GetAD_Process_ID());
            SetRecord_ID(Record_ID);
            SetAD_User_ID(process.GetCtx().GetAD_User_ID());

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
        /// <param name="AD_Process_ID">Process ID</param>
        /// <param name="Record_ID">record</param>
        public MPInstance(Ctx ctx, int AD_Process_ID, int Record_ID)
            : this(ctx, 0, null)
        {
            SetAD_Process_ID(AD_Process_ID);
            SetRecord_ID(Record_ID);
            SetAD_User_ID(ctx.GetAD_User_ID());
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
            string strSql = "SELECT * FROM AD_PInstance_Para WHERE AD_PInstance_ID=@instanceid";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@instanceid", GetAD_PInstance_ID());
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
            String sql = "SELECT * FROM AD_PInstance_Log WHERE AD_PInstance_ID=" + GetAD_PInstance_ID() + " ORDER BY Log_ID";
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
            MPInstanceLog logEntry = new MPInstanceLog(GetAD_PInstance_ID(), _log.Count + 1,
                P_Date, P_ID, P_Number, msg);
            _log.Add(logEntry);
            //	save it to DB ?
            //	log.save();
        }

        /// <summary>
        /// Set AD_Process_ID.
        /// Check Role if process can be performed
        /// </summary>
        /// <param name="AD_Process_ID">process</param>
        public new void SetAD_Process_ID(int AD_Process_ID)
        {
            if (AD_Process_ID <= 0)
                return;
            Console.WriteLine(AD_Process_ID.ToString());
            int AD_Role_ID = GetCtx().GetAD_Role_ID();
            if (AD_Role_ID != 0)
            {
                MRole role = MRole.Get(GetCtx(), AD_Role_ID);
                bool? access = role.GetProcessAccess(AD_Process_ID, AD_Role_ID);
                if (access == null)
                    throw new Exception("Cannot access Process " + AD_Process_ID
                        + " with Role: " + role.Get_Value("Name"));
            }
            base.SetAD_Process_ID(AD_Process_ID);
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
                long ms =DateTime.Now.Millisecond - GetCreated().Millisecond;
                long seconds = (long)(ms / 1000);
                if (seconds < 1)
                {
                    seconds = 1;
                }
                MProcess prc = MProcess.Get(GetCtx(), GetAD_Process_ID());
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
