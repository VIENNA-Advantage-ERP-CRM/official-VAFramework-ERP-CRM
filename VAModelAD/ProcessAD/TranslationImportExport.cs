using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class TranslationImportExport : ProcessEngine.SvrProcess
    {
        /** The Language				*/
        private String _VAF_Language = null;

        private static String Mode_Import = "I";
        private static String Mode_Export = "E";
        /**	Export or Import Mode		*/
        private String _ImportExport = Mode_Export;

        public static String ExportScope_System = "S";
        /** System Data viewed by Tenants	*/
        public static String ExportScope_SystemUser = "U";
        public static String ExportScope_Tenant = "T";
        /** Export Scope		*/
        private String _ExportScope = ExportScope_System;
        /** Optional Specific Table		*/
        private int _VAF_TableView_ID = 0;
        private int _VAF_ModuleInfo_ID = 0;    // Added by SUkhwinder on 23 June, 2017 for creating module specific translations.

        public static String TranslationLevel_All = "A";
        public static String TranslationLevel_LabelOnly = "L";
        public static String TranslationLevel_LabelDescriptionOnly = "D";
        /** Translation Level			*/
        private String _TranslationLevel = TranslationLevel_All;

        /** Optional Directory			*/
        private String _Directory = null;

        /**/

        private bool _ByExportID = true;


        /// <summary>
        /// Prepare - e.g., get Parameters.	 
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            foreach (ProcessInfoParameter element in para)
            {
                String name = element.GetParameterName();
                if (element.GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAF_Language"))
                    _VAF_Language = (String)element.GetParameter();
                else if (name.Equals("ImportExport"))
                    _ImportExport = (String)element.GetParameter();
                else if (name.Equals("ExportScope"))
                    _ExportScope = (String)element.GetParameter();
                else if (name.Equals("VAF_TableView_ID"))
                    _VAF_TableView_ID = element.GetParameterAsInt();
                else if (name.Equals("TranslationLevel"))
                    _TranslationLevel = (String)element.GetParameter();
                else if (name.Equals("VAF_ModuleInfo_ID"))
                    _VAF_ModuleInfo_ID = element.GetParameterAsInt();
                else if (name.Equals("Directory"))
                    _Directory = (String)element.GetParameter();
                else if (name.Equals("IsActive"))
                    _ByExportID = "Y".Equals((String)element.GetParameter());

                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare



        /// <summary>
        /// messages
        /// </summary>
        /// <returns>string</returns>
        protected override String DoIt()
        {
            log.Info("VAF_Language=" + _VAF_Language
                + ",Mode=" + _ImportExport
                + ",Scope=" + _ExportScope
                + ",VAF_TableView_ID=" + _VAF_TableView_ID
                + ",Level=" + _TranslationLevel
                + ",Directory=" + _Directory);
            //

            TranslationMgr t = new TranslationMgr(GetCtx());
            String msg = t.ValidateLanguage(_VAF_Language);
            if (msg.Length > 0)
                throw new Exception("@LanguageSetupError@ - " + msg);

            t.SetByExportID(_ByExportID);
            //	Mode
            bool imp = Mode_Import.Equals(_ImportExport);
            //	Client
            int VAF_Client_ID = 0;
            if (ExportScope_Tenant.Equals(_ExportScope))
                VAF_Client_ID = GetCtx().GetVAF_Client_ID();
            t.SetExportScope(_ExportScope, VAF_Client_ID);

            //	Directory
            if (Utility.Util.IsEmpty(_Directory))
                _Directory = DataBase.Ini.GetFrameworkHome();//
            
            #region [Module Parameter Check]
            // Added by Sukhwinder on 23 June,2017 for creating translations on the basis of Modules.
            MVAFModuleInfo module = null;
            if (_VAF_ModuleInfo_ID != 0)
            {
                module = new MVAFModuleInfo(GetCtx(), _VAF_ModuleInfo_ID, null);
            }
            #endregion

            int noWords = 0;
            //	All Tables
            if (_VAF_TableView_ID == 0)
            {
                String sql = "SELECT * FROM VAF_TableView WHERE IsActive='Y' AND IsView='N'"
                    + " AND TableName LIKE '%_Trl' AND TableName<>'VAF_Column_TL'";
                if (ExportScope_Tenant.Equals(_ExportScope))
                    sql += " AND AccessLevel<>'4'";	//	System Only
                else
                    sql += " AND AccessLevel NOT IN ('1','2','3')";	// Org/Client/Both
                sql += " ORDER BY TableName";
                List<MVAFTableView> tables = MVAFTableView.GetTablesByQuery(GetCtx(), sql);
                foreach (MVAFTableView table in tables)
                {
                    String tableName = table.GetTableName();
                    msg = null;

                    //msg = imp ? t.ImportTrl(_Directory, table.GetTableName()) : t.ExportTrl(_Directory, table.GetTableName(), _TranslationLevel);   // Commented and added following code in IF-ELSE on the basis of Module parameter.(Sukhwinder)
                    
                    if (module == null)
                    {
                        msg = imp ? t.ImportTrl(_Directory, tableName) : t.ExportTrl(_Directory, tableName, _TranslationLevel);
                    }
                    else
                    {
                        msg = imp ? t.ImportTrl(_Directory, tableName) : t.ExportTrl(_Directory, tableName, _TranslationLevel, module.GetPrefix()); // by Sukhwinder on 23 June, 2017
                    }

                    //if (!msg.Contains("Updated"))
                    //{
                    //    msg = "Updated=0";
                    //}
                    AddLog(msg);
                }
                noWords = t.GetWordCount();
            }
            else	//	single table
            {
                MVAFTableView table = MVAFTableView.Get(GetCtx(), _VAF_TableView_ID);
                msg = null;

                //msg = imp ? t.ImportTrl(_Directory, table.GetTableName()) : t.ExportTrl(_Directory, table.GetTableName(), _TranslationLevel);   // Commented and added following code in IF-ELSE on the basis of Module parameter.(Sukhwinder)

                if (module == null)
                {
                    msg = imp ? t.ImportTrl(_Directory, table.GetTableName()) : t.ExportTrl(_Directory, table.GetTableName(), _TranslationLevel);
                }
                else
                {
                    msg = imp ? t.ImportTrl(_Directory, table.GetTableName()) : t.ExportTrl(_Directory, table.GetTableName(), _TranslationLevel, module.GetPrefix()); // by Sukhwinder on 23 June, 2017
                }
               
                //if (!msg.Contains("Updated"))
                //{
                //    msg = "Updated=0";
                //}
                AddLog(msg);
                noWords = t.GetWordCount();
            }
            //
            
            //return ("Word Count = " + noWords);

             return Msg.GetMsg(GetCtx(), "Word Count") + " = " + noWords;  
        }
    }
}
