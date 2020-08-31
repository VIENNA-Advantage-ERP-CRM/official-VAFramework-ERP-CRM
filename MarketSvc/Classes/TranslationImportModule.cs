using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Process;

namespace MarketSvc
{
    public class TranslationImportModule
    {
        /** The Language				*/
        private String _AD_Language = null;

        private static String Mode_Import = "I";
       // private static String Mode_Export = "E";
        /**	Export or Import Mode		*/
        private String _ImportExport = Mode_Import;

        public static String ExportScope_System = "S";
        /** System Data viewed by Tenants	*/
        public static String ExportScope_SystemUser = "U";
        public static String ExportScope_Tenant = "T";
        /** Export Scope		*/
        private String _ExportScope = ExportScope_System;
        /** Optional Specific Table		*/
        private int _AD_Table_ID = 0;

        public static String TranslationLevel_All = "A";
        public static String TranslationLevel_LabelOnly = "L";
        public static String TranslationLevel_LabelDescriptionOnly = "D";
        /** Translation Level			*/
        private String _TranslationLevel = TranslationLevel_All;

        /** Optional Directory			*/
        private String _Directory = null;

        private VAdvantage.Utility.Ctx _ctx = null;

        //private string _prefix = "";

        IMarketCallback _callback = null;
        StringBuilder _sbLog = null;


        /// <summary>
        /// Prepare - e.g., get Parameters.	 
        /// </summary>
        public void Prepare(string langcode, int AD_Table_ID, string ImportExport, String dir, Ctx ctx, IMarketCallback callback,StringBuilder sbLog)
        {
            _AD_Language = langcode;
            _AD_Table_ID = AD_Table_ID;
            _ctx = ctx;
            _Directory = dir;
            _callback = callback;
         _ImportExport = Mode_Import;
         _sbLog = sbLog;
        }	//	prepare

        /// <summary>
        /// messages
        /// </summary>
        /// <returns>string</returns>
        public  String DoIt()
        {
            if (string.IsNullOrEmpty(_Directory))
            {
                throw new Exception("@Error@ - folder path not found");
            }

            if (!System.IO.Directory.Exists(_Directory))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(_Directory);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            TranslationMgr t = new TranslationMgr(_ctx);
            t.SetByExportID(true);

            String msg = t.ValidateLanguage(_AD_Language,true);
            if (msg.Length > 0)
                throw new Exception("@LanguageSetupError@ - " + msg);

            //	Mode
            bool imp = Mode_Import.Equals(_ImportExport);
            //	Client
            int AD_Client_ID = 0;
            if (ExportScope_Tenant.Equals(_ExportScope))
                AD_Client_ID = _ctx.GetAD_Client_ID();
            t.SetExportScope(_ExportScope, AD_Client_ID);

            //	Directory
            if (Util.IsEmpty(_Directory))
                _Directory = VAdvantage.DataBase.Ini.GetFrameworkHome();//

            int noWords = 0;
            //	All Tables
            if (_AD_Table_ID == 0)
            {
                String sql = "SELECT * FROM AD_Table WHERE IsActive='Y' AND IsView='N'"
                    + " AND TableName LIKE '%_Trl' AND TableName<>'AD_Column_Trl'";
                if (ExportScope_Tenant.Equals(_ExportScope))
                    sql += " AND AccessLevel<>'4'";	//	System Only
                else
                    sql += " AND AccessLevel NOT IN ('1','2','3')";	// Org/Client/Both
                sql += " ORDER BY TableName";
                List<MTable> tables = MTable.GetTablesByQuery(_ctx, sql);
                foreach (MTable table in tables)
                {
                    String tableName = table.GetTableName();
                    msg = null;
                    msg = imp
                        ? t.ImportTrl(_Directory, tableName)
                        : t.ExportTrl(_Directory, tableName, _TranslationLevel);
                    AddLog(tableName + "==> " + msg);
                }
                noWords = t.GetWordCount();
            }
            else	//	single table
            {
                MTable table = MTable.Get(_ctx, _AD_Table_ID);
                msg = null;
                msg = imp
                    ? t.ImportTrl(_Directory, table.GetTableName())
                    : t.ExportTrl(_Directory, table.GetTableName(), _TranslationLevel);
                AddLog(table.GetTableName() + " ==> " + msg);
                noWords = t.GetWordCount();
            }
            //
            sb.Append("Word Count = " + noWords);

           

            return sb.ToString();
        }

        StringBuilder sb = new StringBuilder("");
        private void AddLog(string msg)
        {
            
            //sb.Append(msg).Append("\n");

            SendMsg(msg);
            //throw new NotImplementedException();
        }

        private void SendMsg(string msg)
        {
            SendMsg(msg, false);

        }

        private void SendMsg(string msg, bool isDone)
        {
            if (_callback != null)
            {
                _callback.QueryExecuted(new CallBackDetail() { Status = msg, Action = isDone ? "Done" : "" });
            }

            if (string.IsNullOrEmpty(logfileName))
            {
                if (_sbLog != null)
                {
                    _sbLog.Append("\n").Append(msg);
                }
            }
            else
            { //Direct from Market
                MarketSvc.InstallModule.CommonFunctions.WriteLog(msg, logfileName);
            }
        }


        public string logfileName = null;

    }
}
