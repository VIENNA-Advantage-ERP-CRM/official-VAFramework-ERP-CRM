/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AcctProcessor
 * Purpose        : Accounting Processor
 * Class Used     : ViennaServer
 * Chronological    Development
 * Raghunandan      12-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Server
{
    public class AcctProcessor : ViennaServer
    {
        #region private variable

        //The Concrete Model			
        private MAcctProcessor _modelLocal = null;
        //Last Summary			
        private StringBuilder _summary = new StringBuilder();
        // Client info			
        private MClient _clientLocal = null;
        //Accounting Schemata	
        private MAcctSchema[] _ass = null;
        //Document Base Types	
        private MDocBaseType[] _docBaseTypes = null;

        #endregion

        /// <summary>
        /// Accounting Processor
        /// </summary>
        /// <param name="model">model </param>
        public AcctProcessor(MAcctProcessor model)
            : base(model, 30)	//	30 seconds delay
        {
            _modelLocal = model;
            _clientLocal = MClient.Get(model.GetCtx(), model.GetAD_Client_ID());
        }

        /// <summary>
        /// Work
        /// </summary>
        protected override void DoWork()
        {
            _summary = new StringBuilder();
            //	Get Schemata
            if (_modelLocal.GetC_AcctSchema_ID() == 0)
            {
                _ass = MAcctSchema.GetClientAcctSchema(GetCtx(), _modelLocal.GetAD_Client_ID());
            }
            else	//	only specific accounting schema
            {
                _ass = new MAcctSchema[] { new MAcctSchema(GetCtx(), _modelLocal.GetC_AcctSchema_ID(), null) };
            }
            //
            PostSession();
            MCost.Create(_clientLocal);
            //
            int no = _modelLocal.DeleteLog();
            _summary.Append("Logs deleted=").Append(no);
            //
            MAcctProcessorLog pLog = new MAcctProcessorLog(_modelLocal, _summary.ToString());
            pLog.SetReference("#" + Utility.Util.GetValueOfString(_runCount)// String.valueOf(p_runCount)
                + " - " + TimeUtil.FormatElapsed(_startWork));//new DateTime(_startWork)));
            pLog.Save();
        }

        /// <summary>
        /// Post Session
        /// </summary>
        private void PostSession()
        {
            if (_docBaseTypes == null)
            {
                _docBaseTypes = MDocBaseType.GetAll(_modelLocal.GetCtx());
            }

            for (int i = 0; i < _docBaseTypes.Length; i++)
            {
                MDocBaseType dbt = _docBaseTypes[i];
                int AD_Table_ID = dbt.GetAD_Table_ID();
                String TableName = dbt.GetTableName();
                if (AD_Table_ID == 0 || TableName == null)
                {
                    log.Info("Skipped - Invalid: " + dbt);
                    continue;
                }
                //	Post only special documents
                if (_modelLocal.GetAD_Table_ID() != 0
                    && _modelLocal.GetAD_Table_ID() != AD_Table_ID)
                    continue;
                //  SELECT * FROM table
                StringBuilder sql = new StringBuilder("SELECT * FROM ").Append(TableName)
                    .Append(" WHERE AD_Client_ID=" + _modelLocal.GetAD_Client_ID())	//	Include Document Errors
                    .Append(" AND Processed='Y' AND Posted IN ('N','e') AND IsActive='Y'")
                    .Append(" ORDER BY Created");
                //
                int count = 0;
                int countError = 0;
                IDataReader idr = null;
                DataTable dt = null;
                try
                {
                    //pstmt = DataBase.DB.ExecuteReader(sql.ToString(), null, null);
                    idr = DataBase.DB.ExecuteReader(sql.ToString(), null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    //while (idr.Read())//(!isInterrupted() && rs.next())
                    foreach(DataRow dr in dt.Rows)
                    {
                        count++;
                        bool ok = true;
                        try
                        { 
                            Doc doc = Doc.Get(_ass, AD_Table_ID,dr, null);
                            if (doc == null)
                            {
                                //log.Severe(getName() + ": No Doc for " + TableName);
                                ok = false;
                            }
                            else
                            {
                                String error = doc.Post(false, false);   //  post no force/repost
                                ok = error == null;
                            }
                        }
                        catch (Exception e)
                        {
                            log.Log(Level.SEVERE, GetName() + ": " + TableName, e);
                            ok = false;
                        }
                        if (!ok)
                        {
                            countError++;
                        }
                    }
                    dt = null;
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    if (dt != null)
                    {
                        dt = null;
                    }
                    log.Log(Level.SEVERE, sql.ToString(), e);
                }
                                //
                if (count > 0)
                {
                    _summary.Append(TableName).Append("=").Append(count);
                    if (countError > 0)
                        _summary.Append("(Errors=").Append(countError).Append(")");
                    _summary.Append(" - ");

                    log.Fine(GetName() + ": " + _summary.ToString());
                }
                else
                    log.Fine(GetName() + ": " + TableName + " - no work");
            }
        }

        private string GetName()
        {

            return typeof(Doc).FullName;
        }

        /**
         * 	Get Server Info
         *	@return info
         */
        public override String GetServerInfo()
        {
            return "#" + _runCount + " - Last=" + _summary.ToString();
        }
    }
}
