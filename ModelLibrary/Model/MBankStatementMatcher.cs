/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatementMatcher
    * Purpose        : Bank Statement Matcher Algorithm
    * Class Used     : X_C_BankStatementMatcher
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

//using VAdvantage.ImpExp;
namespace VAdvantage.Model
{
    public class MBankStatementMatcher : X_C_BankStatementMatcher
    {
        private BankStatementMatcherInterface _matcher = null;
        private Boolean _matcherValid = false;
        //Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MBankStatementMatcher).FullName);

        /// <summary>
        /// Get Bank Statement Matcher Algorithms
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="trxName"></param>
        /// <returns>matchers</returns>
        public static MBankStatementMatcher[] GetMatchers(Ctx ctx, Trx trxName)
        {
            List<MBankStatementMatcher> list = new List<MBankStatementMatcher>();
            String sql = MRole.GetDefault(ctx, false).AddAccessSQL(
                "SELECT * FROM C_BankStatementMatcher ORDER BY SeqNo",
                "C_BankStatementMatcher", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            int AD_Client_ID = ctx.GetAD_Client_ID();
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
                    list.Add(new MBankStatementMatcher(ctx, dr, trxName));
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
            MBankStatementMatcher[] retValue = new MBankStatementMatcher[list.Count];
            retValue = list.ToArray();
            return retValue;
        }



        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_BankStatementMatcher_ID"></param>
        /// <param name="trxName"></param>
        public MBankStatementMatcher(Ctx ctx, int C_BankStatementMatcher_ID, Trx trxName)
            : base(ctx, C_BankStatementMatcher_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MBankStatementMatcher(Ctx ctx, DataRow dr, Trx trxName)
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
