/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Jagmohan Bhatt
 * Date           : 13 jan 2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using VAdvantage.Process;

namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// SvrProcess check the current process to be executed
    /// </summary>
    public abstract class SvrProcess : ProcessCall
    {
        public SvrProcess()
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);
            //nothing to process in constructor
        }

        private Ctx ctxContext = Utility.Env.GetCtx();
        private ProcessInfo _pi;

        //private VAdvantage.Model.PO objPO = null;

        protected static String msgSaveErrorRowNotFound = "@SaveErrorRowNotFound@";
        protected static String msgInvalidArguments = "@InvalidArguments@";
        private Trx _trx;

        // report engine
        public static List<VAdvantage.Print.ReportEngine_N> listReportEngine = null;

        /**	Logger							*/
        protected internal VLogger log = null;

        private object lockObj = new object();

        /// <summary>
        /// Starts the process
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="pi">ProcessInfo object</param>
        /// <returns></returns>
        public bool StartProcess(Ctx ctx, ProcessInfo pi, Trx trx)
        {
            //  Preparation
            _pi = pi;
            PrepareCtx(ctx);
            //ctxContext = ctx == null ? Utility.Env.GetCtx() : ctx;

            _trx = trx;
            bool localTrx = _trx == null;

            if (localTrx)
            {
                _trx = Trx.GetTrx("SvrProcess");
            }


            //trx = SqlExec.ExecuteQuery.GerServerTransaction();

            String msg = null;
            bool success = true;
            try
            {
                Lock();
                Prepare();
                msg = DoIt();
            }
            catch (Exception e)
            {
                msg = e.Message;
                if (msg == null)
                    msg = e.ToString();
                if (e.Message != null)
                    log.Log(Level.SEVERE, msg);
                else if (VLogMgt.IsLevelFiner())
                    log.Log(Level.WARNING, msg);
                else
                    log.Warning(msg);
                success = false;
            }

            if (localTrx && _trx !=null)
            {
                if (success)
                    _trx.Commit();
                else
                    _trx.Rollback();
                _trx.Close();
                _trx = null;
            }

            //	Parse Variables
            msg = Utility.Msg.ParseTranslation(ctx, msg);
            _pi.SetSummary(msg, !success);
            ProcessInfoUtil.SaveLogToDB(_pi);

            Unlock();
            return success;
        }

        private void PrepareCtx(Ctx ctx)
        {
            lock (lockObj)
            {
                ctxContext = new Ctx(ctx.GetMap());

                if (_pi.GetLocalCtx().Count > 0)
                {
                    foreach (var pair in _pi.GetLocalCtx())
                    {
                        ctxContext.SetContext(pair.Key, pair.Value);
                    }
                }

                ctxContext.SetAD_Client_ID(GetAD_Client_ID());
                if (_pi.GetAD_User_ID().HasValue)
                {
                    ctxContext.SetAD_Client_ID(GetAD_Client_ID());
                }

            }
        }

        /// <summary>
        /// abstract function Prepare
        /// </summary>
        abstract protected void Prepare();

        /// <summary>
        /// abstract function DoIt
        /// </summary>
        /// <returns></returns>
        abstract protected string DoIt();

        public ProcessInfo GetProcessInfo()
        {
            return _pi;
        }

        public Ctx GetCtx()
        {
            return ctxContext;
        }

        protected String GetName()
        {
            return _pi.GetTitle();
        }

        protected int GetAD_PInstance_ID()
        {
            return _pi.GetAD_PInstance_ID();
        }

        protected int GetTable_ID()
        {
            return _pi.GetTable_ID();
        }

        protected int GetRecord_ID()
        {
            return _pi.GetRecord_ID();
        }

        /// <summary>
        /// User ID
        /// </summary>
        /// <returns>return user id</returns>
        protected int GetAD_User_ID()
        {
            if (_pi.GetAD_User_ID() == null || _pi.GetAD_Client_ID() == null)
            {
                String sql = "SELECT AD_User_ID, AD_Client_ID FROM AD_PInstance WHERE AD_PInstance_ID=@instanceid";
                IDataReader dr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@instanceid", _pi.GetAD_PInstance_ID());
                    dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                    while (dr.Read())
                    {
                        _pi.SetAD_User_ID(Utility.Util.GetValueOfInt(dr[0].ToString()));
                        _pi.SetAD_Client_ID(Utility.Util.GetValueOfInt(dr[1].ToString()));
                    }
                    dr.Close();
                }
                catch (SqlException e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Log(Level.SEVERE, e.Message);
                }
            }
            if (_pi.GetAD_User_ID() == 0)
                return 0;
            return (int)_pi.GetAD_User_ID();
        }

        /// <summary>
        /// Gets the client id
        /// </summary>
        /// <returns>return client id </returns>
        protected int GetAD_Client_ID()
        {
            if (_pi.GetAD_Client_ID() == null)
            {
                GetAD_User_ID();	//	Sets also Client
                if (_pi.GetAD_Client_ID() == null)
                    return 0;
            }
            return (int)_pi.GetAD_Client_ID();
        }

        protected int GetAD_Org_ID()
        {
            if (_pi.GetAD_Org_ID() == null)
            {
                return ctxContext.GetAD_Org_ID();
            }
            return (int)_pi.GetAD_Org_ID();
        }


        /// <summary>
        /// Gets the parameter
        /// </summary>
        /// <returns></returns>
        protected ProcessInfoParameter[] GetParameter()
        {
            ProcessInfoParameter[] retValue = _pi.GetParameter();
            if (retValue == null || retValue.Length == 0)
            {
                ProcessInfoUtil.SetParameterFromDB(_pi, ctxContext);
                retValue = _pi.GetParameter();
            }
            return retValue;
        }

        /// <summary>
        /// DO function finally call the class and execute the method required with params
        /// </summary>
        /// <param name="className">Name of the class</param>
        /// <param name="methodName">Name of the Method</param>
        /// <param name="args">Arguements to be passed in a method</param>
        /// <returns>Object</returns>
        public Object DoIt(String className, String methodName, Object[] args)
        {
            object oRes = null; //stores the final result
            try
            {
                Type type = Type.GetType(className);
                if (type.IsClass)
                {
                    if (type.GetMethod(methodName) != null)
                    {
                        object oClass = Activator.CreateInstance(type);
                        MethodInfo method = type.GetMethod(methodName);
                        oRes = method.Invoke(method, args);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "doIt" + ex.Message);
            }
            return oRes;
        }
        /// <summary>
        /// Return the main transaction of the current Process.
        /// Obsolete("Use Get_Trx() instead")]
        /// </summary>
        /// <returns>the transaction name</returns>
        /// 
        public Trx Get_TrxName()
        {
            if (_trx != null)
                return _trx;
            return null;
        }

        /// <summary>
        /// Return the main transaction of the current Process.
        /// </summary>
        /// <returns>the transaction</returns>
        public Trx Get_Trx()
        {
            return _trx;
        }	//	get_Trx

        public void AddLog(int id, DateTime? date, Decimal? number, String msg)
        {
            if (_pi != null)
                _pi.AddLog(id, date, number, msg);
            log.Info(id + " - " + date + " - " + number + " - " + msg);
        }	//	addLog

        public void AddLog(String msg)
        {
            if (msg != null)
                AddLog(0, DateTime.MinValue, null, msg);
        }	//	addLog


        private void Lock()
        {
            log.Fine("AD_PInstance_ID=" + _pi.GetAD_PInstance_ID());
            SqlExec.ExecuteQuery.ExecuteNonQuery("UPDATE AD_PInstance SET IsProcessing='Y' WHERE AD_PInstance_ID="
                + _pi.GetAD_PInstance_ID());		//	outside trx
        }   //  lock


        /// <summary>
        /// Unlock Process Instance.
        /// Update Process Instance DB and write option return message
        /// </summary>
        private void Unlock()
        {
            //MPInstance mpi = new MPInstance(Utility.Env.GetContext(), _pi.GetAD_PInstance_ID(), null);
            MPInstance mpi = new MPInstance(ctxContext, _pi.GetAD_PInstance_ID(), null);
            if (mpi.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "Did not find PInstance " + _pi.GetAD_PInstance_ID());
                return;
            }
            mpi.SetIsProcessing(false);
            mpi.SetResult(_pi.IsError());
            mpi.SetErrorMsg(_pi.GetSummary());
            mpi.Save();
            log.Fine(mpi.ToString());
        }

        /// <summary>
        /// Commit
        /// </summary>
        protected void Commit()
        {
            if (_trx != null)
            {
                _trx.Commit();
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        protected void Rollback()
        {
            if (_trx != null)
                _trx.Rollback();
        }	//	rollback
        // To get last log error while saving any MClass Suggested by Mukesh Sir and written by Manjot
        public virtual string GetRetrievedError(VAdvantage.Model.PO po, string defaultmsg)
        {

            VAdvantage.Model.ValueNamePair vp=VLogger.RetrieveError();
            if (vp != null)
            {
                string tableName = string.Empty;
                string val = vp.GetName();
                string connectionName = DataBase.VConnection.Get().Db_uid.ToUpper();
                if (val.Contains("."))
                {
                    tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT Name FROM AD_Table WHERE AD_Table_ID =" + po.Get_Table_ID()));
                    val = val.Replace(connectionName, string.Empty).Replace(po.Get_TableName().ToUpper(), string.Empty).Replace('"', ' ').Trim();
                    val = val.Substring(val.IndexOf(":") + 1).Replace(".", string.Empty).Replace(@")(", string.Empty);
                }
                log.SaveError(Msg.GetMsg(GetCtx(), "VIS_Erroron") + tableName + " :- ", val);
                return Msg.GetMsg(GetCtx(), "VIS_Erroron") + tableName + " :-" + val;
            }
            log.SaveError(defaultmsg, "");
            return Msg.GetMsg(GetCtx(), defaultmsg);
        }

    }
}
