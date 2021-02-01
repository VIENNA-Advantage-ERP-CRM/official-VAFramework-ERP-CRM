/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatementMatcher
    * Purpose        : Bank Statement Matcher Algorithm
    * Class Used     : X_VAB_BankingJRNLMatcher
    * Chronological    Development
    * Raghunandan     26-Nov-2009
******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;
using VAdvantage.Interface;

//using VAdvantage.ImpExp;
namespace VAdvantage.Model
{
    public class MVABBankingJRNLMatcher : X_VAB_BankingJRNLMatcher
    {
        private BankStatementMatcherInterface _matcher = null;
        private Boolean _matcherValid = false;
        //Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABBankingJRNLMatcher).FullName);

        /// <summary>
        /// Get Bank Statement Matcher Algorithms
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="trxName"></param>
        /// <returns>matchers</returns>
        public static MVABBankingJRNLMatcher[] GetMatchers(Ctx ctx, Trx trxName)
        {
            List<MVABBankingJRNLMatcher> list = new List<MVABBankingJRNLMatcher>();
            String sql = MVAFRole.GetDefault(ctx, false).AddAccessSQL(
                "SELECT * FROM VAB_BankingJRNLMatcher ORDER BY SeqNo",
                "VAB_BankingJRNLMatcher", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABBankingJRNLMatcher(ctx, dr, trxName));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            //	Convert		
            MVABBankingJRNLMatcher[] retValue = new MVABBankingJRNLMatcher[list.Count];
            retValue = list.ToArray();
            return retValue;
        }



        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_BankingJRNLMatcher_ID"></param>
        /// <param name="trxName"></param>
        public MVABBankingJRNLMatcher(Ctx ctx, int VAB_BankingJRNLMatcher_ID, Trx trxName)
            : base(ctx, VAB_BankingJRNLMatcher_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABBankingJRNLMatcher(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Is Matcher Valid
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsMatcherValid()
        {
            if (!_matcherValid)//==null
            {
                GetMatcher();
            }
            return _matcherValid;//.booleanValue();
        }

        /// <summary>
        /// Get Matcher 
        /// </summary>
        /// <returns>Matcher Instance</returns>
        public BankStatementMatcherInterface GetMatcher()
        {
            if (_matcher != null || ( _matcherValid))//.booleanValue()
            {
                return _matcher;
            }

            String className = GetClassname();
            if (className == null || className.Length == 0)
            {
                return null;
            }

            try
            {
                //Class matcherClass = Class.forName(className);
                Type matcherClass = Type.GetType(className);
                //_matcher = (BankStatementMatcherInterface)matcherClass.newInstance();
                _matcher = (BankStatementMatcherInterface)Activator.CreateInstance(matcherClass);
                _matcherValid = true;//  Boolean.TRUE;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, className, e);
                _matcher = null;
                _matcherValid = false; //Boolean.FALSE;
            }
            return _matcher;
        }
    }
}
