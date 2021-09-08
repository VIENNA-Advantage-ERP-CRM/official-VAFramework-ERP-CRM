/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocWorkflowManager
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      06-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.WF
{
    public class DocWorkflowManager : DocWorkflowMgr
    {
        #region Private variable
        //	Document Workflow Manager		
        private static DocWorkflowManager _mgr = null;
        private int _noCalled = 0;
        private int _noStarted = 0;
        //	Logger			
        private static VLogger log = VLogger.GetVLogger(typeof(DocWorkflowManager).FullName);
        #endregion

        /// <summary>
        ///Get Document Workflow Manager
        /// </summary>
        /// <returns>mgr</returns>
        public static DocWorkflowManager Get()
        {
            if (_mgr == null)
                _mgr = new DocWorkflowManager();
            return _mgr;
        }

        //	Set PO Workflow Manager
        //static 
        //{
        //    PO.SetDocWorkflowMgr(Get());
        //}
        static DocWorkflowManager()
        {
            PO.SetDocWorkflowMgr(Get());
        }


        /// <summary>
        /// Doc Workflow Manager
        /// </summary>
        private DocWorkflowManager() : base()
        {
            if (_mgr == null)
                _mgr = this;
        }

        /// <summary>
        /// Process Document Value Workflow
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="AD_Table_ID">table</param>
        /// <returns>true if WF started</returns>
        public bool Process(PO document, int AD_Table_ID)
        {
            _noCalled++;
            MWorkflow[] wfs = MWorkflow.GetDocValue(document.GetCtx(),
                document.GetAD_Client_ID(), AD_Table_ID);
            if (wfs == null || wfs.Length == 0)
                return false;

            bool started = false;
            for (int i = 0; i < wfs.Length; i++)
            {
                MWorkflow wf = wfs[i];
                //	We have a Document Workflow
                String logic = wf.GetDocValueLogic();
                if (logic == null || logic.Length == 0)
                {
                    log.Severe("Workflow has no Logic - " + wf.GetName());
                    continue;
                }

                //	Re-check: Document must be same Client as workflow
                if (wf.GetAD_Client_ID() != document.GetAD_Client_ID())
                    continue;

                //	Check Logic
                bool sql = logic.StartsWith("SQL=");
                if (sql && !TestStart(wf, document))
                {
                    log.Fine("SQL Logic evaluated to false (" + logic + ")");
                    continue;
                }
                if (!sql && !Evaluator.EvaluateLogic(document, logic))
                {
                    log.Fine("Logic evaluated to false (" + logic + ")");
                    continue;
                }

                if (document.Get_Trx() != null)
                {

                    ManageSkippedWF.Add(document.Get_Trx().SetUniqueTrxName(Trx.CreateTrxName("WFDV")), document);
                    log.Severe("Not started: " + wf);
                    continue;
                }
                //	Start Workflow
                log.Fine(logic);
                int AD_Process_ID = 305;		//	HARDCODED
                ProcessInfo pi = new ProcessInfo(wf.GetName(), AD_Process_ID, AD_Table_ID, document.Get_ID());
                pi.SetAD_User_ID(document.GetCtx().GetAD_User_ID());
                pi.SetAD_Client_ID(document.GetAD_Client_ID());

                // vinay bhatt for window id
                pi.SetAD_Window_ID(document.GetAD_Window_ID());
                //

                wf.GetCtx().SetContext("#AD_Client_ID", pi.GetAD_Client_ID().ToString());
                MWFProcess retVal = wf.Start(pi);
                if (retVal != null)
                {
                    log.Config(wf.GetName());
                    _noStarted++;
                    started = true;

                    // VIS0060: work done to Show Message from workflow Process
                    document.SetDocWFMsg(retVal.GetProcessMsg());
                }
            }
            return started;
        }

        /// <summary>
        ///Test Start condition
        /// </summary>
        /// <param name="wf">workflow</param>
        /// <param name="document">document</param>
        /// <returns>true if WF should be started</returns>
        private bool TestStart(MWorkflow wf, PO document)
        {
            bool retValue = false;
            String logic = wf.GetDocValueLogic();
            logic = logic.Substring(4);		//	"SQL="
            String tableName = document.Get_TableName();
            String[] keyColumns = document.Get_KeyColumns();
            if (keyColumns.Length != 1)
            {
                //this is notice for column length
                log.Severe("Tables with more then one key column not supported - "
                    + tableName + " = " + keyColumns.Length);
                return false;
            }
            String keyColumn = keyColumns[0];
            StringBuilder sql = new StringBuilder("SELECT ")
                .Append(keyColumn).Append(" FROM ").Append(tableName)
                .Append(" WHERE AD_Client_ID=" + wf.GetAD_Client_ID() + " AND ")		//	#1
                    .Append(keyColumn).Append("=" + document.Get_ID() + " AND ")	//	#2
                .Append(logic)
                //	Duplicate Open Workflow test
                .Append(" AND NOT EXISTS (SELECT * FROM AD_WF_Process wfp ")
                    .Append("WHERE wfp.AD_Table_ID=" + document.Get_Table_ID() + " AND wfp.Record_ID=")	//	#3
                    .Append(tableName).Append(".").Append(keyColumn)
                    .Append(" AND wfp.AD_Workflow_ID=" + wf.GetAD_Workflow_ID())	//	#4
                    .Append(" AND SUBSTR(wfp.WFState,1,1)='O')");
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, document.Get_Trx());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    retValue = true;
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Logic=" + logic + " - SQL=" + sql.ToString(), e);
            }
            return retValue;
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("DocWorkflowManager[");
            sb.Append("Called=").Append(_noCalled)
                .Append(",Stated=").Append(_noStarted)
                .Append("]");
            return sb.ToString();
        }
    }
}
