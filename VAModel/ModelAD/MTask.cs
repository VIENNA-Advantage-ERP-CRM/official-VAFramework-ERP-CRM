/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTask
 * Purpose        : Operating Task Model
 * Class Used     : X_AD_Task
 * Chronological    Development
 * Deepak           29-Jan-2010
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
using System.Threading;

namespace VAdvantage.Model
{
    public class MTask : X_AD_Task
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Task_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTask(Ctx ctx, int AD_Task_ID, Trx trxName)
            : base(ctx, AD_Task_ID, trxName)
        {
            
        }	//	MTask

       /// <summary>
        /// Load Cosntructor 
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="rs">datarow</param>
       /// <param name="trxName">transaction</param>
        public MTask(Ctx ctx, DataRow dr, Trx trxName):base(ctx,dr, trxName)
        {
            
        }	//	MTask

        /**	Actual Task			*/
        private Task _task = null;
        private Thread task = null;
        /// <summary>
        /// Execute Task and wait
        /// </summary>
        /// <returns>info</returns>
        public String Execute()
        {
            String cmd = Msg.ParseTranslation(GetCtx(), GetOS_Command()).Trim();
            if (cmd == null || cmd.Equals(""))
                return "Cannot execute '" + GetOS_Command() + "'";
            //
            if (IsServerProcess())
                return ExecuteRemote(cmd);
            return ExecuteLocal(cmd);
        }	//	execute

        /// <summary>
        /// Execute Task locally and wait
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>info</returns>
        public String ExecuteLocal(String cmd)
        {
            log.Config(cmd);
            if (_task != null && task.IsAlive)
               task.Interrupt();
               
            _task = new Task(cmd);
            _task.Start();

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                //  Give it a bit of time
                try
                {
                    Thread.Sleep(500);
                }
                catch (Exception ioe)
                {
                    log.Log(Level.SEVERE, cmd, ioe);
                }
                //  Info to user
                sb.Append(_task.GetOut())
                    .Append("\n-----------\n")
                    .Append(_task.GetErr())
                    .Append("\n-----------");

                //  Are we done?
                if (!_task.IsAlive())
                    break;
            }
            log.Config("done");
            return sb.ToString();
        }	//	executeLocal

        /// <summary>
        /// Execute Task locally and wait
        /// </summary>
        /// <param name="cmd">command</param>
        /// <returns>info</returns>
        public String ExecuteRemote(String cmd)
        {
            log.Config(cmd);
            return "Remote:\n";
        }	//	executeRemote


        /// <summary>
        ///	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTask[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(";Server=").Append(IsServerProcess())
                .Append(";").Append(GetOS_Command())
                .Append("]");
            return sb.ToString();
        }	//	toString

    }	//	MTask

}
