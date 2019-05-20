/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ServerBean
 * Class Used     : MarshalByRefObject
 * Chronological    Development
 * Raghunandan     03-Feb-2010  
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.Acct;
using VAdvantage.WF;
using System.IO;
using VAdvantage.Controller;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Server
{
    [Serializable()]
    public class ServerBean : MarshalByRefObject, Server
    {
        #region Private variables
        //Context			
        // private SessionContext 	m_Context;
        /**	Logger				*/
        private static VLogger log = VLogger.GetVLogger(typeof(ServerBean).FullName);
        //
        private static int _sno = 0;
        private int _no = 0;
        //
        private int _windowCount = 0;
        private int _postCount = 0;
        private int _processCount = 0;
        private int _workflowCount = 0;
        private int _paymentCount = 0;
        private int _nextSeqCount = 0;
        private int _rowSetCount = 0;
        private int _updateCount = 0;
        private int _cacheResetCount = 0;
        private int _updateLOBCount = 0;
        #endregion


        /// <summary>
        /// Post Immediate
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx">Client Context</param>
        /// <param name="AD_Client_ID">Client ID of Document</param>
        /// <param name="AD_Table_ID">Table ID of Document</param>
        /// <param name="Record_ID">Record ID of this document</param>
        /// <param name="force">force posting</param>
        /// <param name="trxName">transaction</param>
        /// <returns>null, if success or error message</returns>
        public String PostImmediate(IDictionary<string,string> ctxDic, int AD_Client_ID, int AD_Table_ID, int Record_ID, bool force, Trx trxName)
        {
            //log.Info("[" + _no + "] Table=" + AD_Table_ID + ", Record=" + Record_ID);
            //_postCount++;
            Ctx ctx = new Ctx(ctxDic);
            MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
            return Doc.PostImmediate(ass, AD_Table_ID, Record_ID, force, trxName);
        }

        /// <summary>
        /// Just to try the connection with the server
        /// </summary>
        /// <returns>OK if connection is fine</returns>
        public string TryConnection(out string vconn)
        {
            vconn = Ini.GetProperty(Ini.P_CONNECTION);   
            return "OK";
        }

        /// <summary>
        /// Get next number for Key column = 0 is Error.
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>next no</returns>
        public int GetNextID(int AD_Client_ID, String TableName, Trx trxName)
        {
            int retValue = MSequence.GetNextID(AD_Client_ID, TableName, trxName);
            //log.Finer("[" + _no + "] " + TableName + " = " + retValue);
            _nextSeqCount++;
            return retValue;
        }

        /// <summary>
        /// Get Document No from table
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        public String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName)
        {
            _nextSeqCount++;
            String dn = "";
            //MSequence.GetDocumentNo(AD_Client_ID, TableName, trxName);
            //if (dn == null)		//	try again
            //{
            //    dn = MSequence.GetDocumentNo(AD_Client_ID, TableName, trxName);
            //}
            return dn;
        }

        ///// <summary>
        ///// Get Document No based on Document Type
        ///// @ejb.interface-method view-type="both"
        ///// </summary>
        ///// <param name="C_DocType_ID">document type</param>
        ///// <param name="trxName">optional Transaction Name</param>
        ///// <returns>document no or null</returns>
        public String GetDocumentNo(int C_DocType_ID, Trx trxName)
        {
            _nextSeqCount++;
            String dn = "";
            //MSequence.GetDocumentNo(C_DocType_ID, trxName);
            //if (dn == null)		//	try again
            //{
            //    dn = MSequence.GetDocumentNo(C_DocType_ID, trxName);
            //}
            return dn;
        }

        /// <summary>
        /// Get and create Window Model Value Object
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx">Environment Properties</param>
        /// <param name="WindowNo">number of this window</param>
        /// <param name="AD_Window_ID">the internal number of the window, if not 0, AD_Menu_ID is ignored</param>
        /// <param name="AD_Menu_ID">ine internal menu number, used when AD_Window_ID is 0</param>
        /// <returns>initialized Window Model</returns>
        public GridWindowVO GetWindowVO(Ctx ctx, int WindowNo, int AD_Window_ID, int AD_Menu_ID)
        {
            log.Info("getWindowVO[" + _no + "] Window=" + AD_Window_ID);
            //	log.Fine(ctx);
            GridWindowVO vo = GridWindowVO.Create(ctx, WindowNo, AD_Window_ID, AD_Menu_ID);
            _windowCount++;
            return vo;
        }

        /// <summary>
        /// Process Remote
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="pi">Process Info</param>
        /// <returns>resulting Process Info</returns>
        public ProcessInfo Process(Ctx ctx, ProcessInfo pi)
        {
            String className = pi.GetClassName();
            log.Info(className + " - " + pi);
            _processCount++;
            //	Get Class
            // Class clazz = null;		//	XDoclet: no generics!
            Type clazz = null;
            try
            {
                //clazz = Class.forName (className);
                clazz = Type.GetType(className);
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, className, ex);
                pi.SetSummary("ClassNotFound", true);
                return pi;
            }
            //	Get Process
            SvrProcess process = null;
            try
            {
                //process = (SvrProcess)clazz.newInstance ();
                process = (SvrProcess)Activator.CreateInstance(clazz);
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, "Instance for " + className, ex);
                pi.SetSummary("InstanceError", true);
                return pi;
            }
            //	Start Process
            Trx trx = Trx.Get("ServerPrc");
            try
            {
                bool ok = process.StartProcess(ctx, pi, trx);
                pi = process.GetProcessInfo();
                trx.Commit();
                trx.Close();
            }
            catch (Exception ex1)
            {
                trx.Rollback();
                trx.Close();
                pi.SetSummary("ProcessError=>"+ex1.Message, true);
                return pi;
            }
            return pi;
        }


        /// <summary>
        /// Run Workflow (and wait) on Server
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pi"></param>
        /// <param name="AD_Workflow_ID"></param>
        /// <returns></returns>
        public ProcessInfo Workflow(Ctx ctx, ProcessInfo pi, int AD_Workflow_ID)
        {
            log.Info("[" + _no + "] " + AD_Workflow_ID);
            _workflowCount++;
            MWorkflow wf = MWorkflow.Get(ctx, AD_Workflow_ID);
            MWFProcess wfProcess = null;
            if (pi.IsBatch())
            {
                wfProcess = wf.Start(pi);	//	may return null
            }
            else
            {
                wfProcess = wf.StartWait(pi);	//	may return null
            }
            log.Fine(pi.ToString());
            return pi;
        }

        /// <summary>
        /// Online Payment from Server
        /// @ejb.interface-method view-type="both"
        /// Called from MPayment processOnline
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Payment_ID"></param>
        /// <param name="C_PaymentProcessor_ID"></param>
        /// <param name="trxName"></param>
        /// <returns>true if approvedc</returns>
        public bool PaymentOnline(Ctx ctx, int C_Payment_ID, int C_PaymentProcessor_ID, Trx trxName)
        {
            MPayment payment = new MPayment(ctx, C_Payment_ID, trxName);
            MPaymentProcessor mpp = new MPaymentProcessor(ctx, C_PaymentProcessor_ID, null);
            log.Info("[" + _no + "] " + payment + " - " + mpp);
            _paymentCount++;
            bool approved = false;
            try
            {
                PaymentProcessor pp = PaymentProcessor.Create(mpp, payment);
                if (pp == null)
                {
                    payment.SetErrorMessage("No Payment Processor");
                }
                else
                {
                    approved = pp.ProcessCC();
                    if (approved)
                    {
                        payment.SetErrorMessage(null);
                    }
                    else
                    {
                        payment.SetErrorMessage("From " + payment.GetCreditCardName()
                            + ": " + payment.GetR_RespMsg());
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "", e);
                payment.SetErrorMessage("Payment Processor Error");
            }
            payment.Save();
            return approved;
        }

        /// <summary>
        /// Create EMail from Server (Request User)
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="toEMail">recipient email address</param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns> EMail</returns>
        public EMail CreateEMail(Ctx ctx, int AD_Client_ID, String toEMail, String toName, String subject, String message)
        {
            MClient client = MClient.Get(ctx, AD_Client_ID);
            return client.CreateEMail(toEMail, toName, subject, message);
        }

        /// <summary>
        /// Create EMail from Server (Request User)
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns>EMail or null</returns>
        public EMail CreateEMail(Ctx ctx, int AD_Client_ID, int AD_User_ID,
            String toEMail, String toName, String subject, String message)
        {
            MClient client = MClient.Get(ctx, AD_Client_ID);
            MUser from = new MUser(ctx, AD_User_ID, null);
            return client.CreateEMail(from, toEMail, toName, subject, message);
        }


        /// <summary>
        /// Create EMail from Server (Request User)
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="AD_Task_ID">task </param>
        /// <returns>execution trace</returns>
        public String ExecuteTask(int AD_Task_ID)
        {
            MTask task = new MTask(Env.GetCtx(), AD_Task_ID, null);	//	Server Context
            return task.Execute();
        }


        /// <summary>
        /// Cash Reset
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="Record_ID">record or 0 for all</param>
        /// <returns>number of records reset</returns>
        public int CacheReset(String tableName, int Record_ID)
        {
            log.Config(tableName + " - " + Record_ID);
            _cacheResetCount++;
            return CacheMgt.Get().Reset(tableName, Record_ID);
        }

        /// <summary>
        /// LOB update
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="displayType">display type (i.e. BLOB/CLOB)</param>
        /// <param name="value">the data</param>
        /// <returns>true if updated</returns>
        public bool UpdateLOB(String sql, int displayType, Object value)
        {
            if (sql == null || value == null)
            {
                log.Fine("No sql or data");
                return false;
            }
            log.Fine(sql);
            _updateLOBCount++;
            bool success = true;
            //Connection con = DataBase.createConnection(false, Connection.TRANSACTION_READ_COMMITTED);
            IDbConnection con = DataBase.DB.GetConnection();
            try
            {
                //idr = con.prepareStatement(sql);
                SqlParameter[] param = new SqlParameter[1];

                if (displayType == DisplayType.TextLong)
                {
                    //pstmt.setString(1, (String)value);
                    param[0] = new SqlParameter("@param1", Utility.Util.GetValueOfString(value));
                }
                else
                {
                    //pstmt.setBytes(1, (byte[])value);
                    param[0] = new SqlParameter("@param1", (byte[])value);
                }
                int no = DataBase.DB.ExecuteQuery(sql, param, null);
            }
            catch (Exception e)
            {
                log.Log(Level.FINE, sql, e);
                success = false;
            }
            //	Success - commit local trx
            if (success)
            {
                try
                {
                    //con.commit();
                    //con.close();
                    //con = null;
                    con.Close();
                    con = null;
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "commit", e);
                    success = false;
                }
            }
            //	Error - roll back
            if (!success)
            {
                log.Severe("rollback");
                try
                {
                    //con.rollback();
                    //con.close();
                    //con = null;

                    con.Close();
                    con = null;
                }
                catch (Exception ee)
                {
                    log.Log(Level.SEVERE, "rollback", ee);
                }
            }

            return success;
        }


        /// <summary>
        /// Describes the instance and its content for debugging purpose
        /// @ejb.interface-method view-type="both"
        /// </summary>
        /// <returns>Debugging information about the instance and its content</returns>
        public String GetStatus()
        {
            StringBuilder sb = new StringBuilder("ServerBean[");
            sb.Append(_no)
                .Append("-Window=").Append(_windowCount)
                .Append(",Post=").Append(_postCount)
                .Append(",Process=").Append(_processCount)
                .Append(",NextSeq=").Append(_nextSeqCount)
                .Append(",Workflow=").Append(_workflowCount)
                .Append(",Payment=").Append(_paymentCount)
                .Append(",RowSet=").Append(_rowSetCount)
                .Append(",Update=").Append(_updateCount)
                .Append(",CacheReset=").Append(_cacheResetCount)
                .Append(",UpdateLob=").Append(_updateLOBCount)
                .Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return GetStatus();
        }


        /// <summary>
        /// Create the Session Bean
        /// @ejb.create-method view-type="both"
        /// </summary>
        public void EjbCreate()
        {
            _no = ++_sno;
            try
            {
                //if (!Vienna.startup(false, "ServerBean"))
                //{
                //    throw new Exception("Vienna could not start");
                //}
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "ejbCreate", ex);
                //	throw new CreateException ();
            }
            log.Info("#" + GetStatus());
        }


        // -------------------------------------------------------------------------
        // Framework Callbacks
        // -------------------------------------------------------------------------

        /// <summary>
        /// Method setSessionContext
        /// </summary>
        /// <param name="aContext">SessionContext</param>
        ///@see javax.ejb.SessionBean#setSessionContext(SessionContext) 
        //public void SetSessionContext (SessionContext aContext) 
        //{
        //    m_Context = aContext;
        //}	

        /// <summary>
        /// Method ejbActivate
        /// @see javax.ejb.SessionBean#ejbActivate()
        /// </summary>
        public void EjbActivate()
        {
            if (log == null)
            {
                log = VLogger.GetVLogger(this.GetType().FullName);
            }
            log.Info("ejbActivate " + GetStatus());
        }

        /// <summary>
        /// Method ejbPassivate
        /// @see javax.ejb.SessionBean#ejbPassivate()
        /// </summary>
        public void EjbPassivate()
        {
            log.Info("ejbPassivate " + GetStatus());
        }

        /// <summary>
        /// Method ejbRemove
        /// @see javax.ejb.SessionBean#ejbRemove()
        /// </summary>
        public void EjbRemove()
        {
            log.Info("ejbRemove " + GetStatus());
        }


        /// <summary>
        /// 	Dump SerialVersionUID of class 
        /// </summary>
        /// <param name="clazz">class</param>
        protected static void DumpSVUID(Type clazz)
        {
            //String s = clazz.getName() 
            String s = clazz.Name
            + " ==\nstatic final long serialVersionUID = "
            + java.io.ObjectStreamClass.lookup(clazz).getSerialVersionUID()
            + "L;\n";
            //System.out.println (s);
        }

        //protected VConnection GetDBType()
        //{
        //    string fileName = Ini.GetFrameworkHome() + Path.DirectorySeparatorChar + Ini.VIENNA_ENV_FILE;
        //    if (string.IsNullOrEmpty(fileName))
        //        return false;

        //    FileStream fis = null;
        //    try
        //    {
        //        fis = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        prop.Load(fis);
        //        fis.Close();
        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        return false;
        //    }

        
        //}


    }

}
