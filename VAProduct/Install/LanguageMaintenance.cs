/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : LanguageMaintenance
 * Purpose        : To add/delete/update language in "_trl " table according to the system language
 * Class Used     : LanguageMaintenance inherits SvrProcess and MLanguage class
 * Chronological    Development
 * Raghunandan      6-April-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Logging;
using System.IO;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Install
{
   public class LanguageMaintenance : ProcessEngine.SvrProcess
    {
        #region Private variable  
        private int _AD_Language_ID = 0;//The Language ID			
        private String _MaintenanceMode = null;// Maintenance Mode
        public static String MAINTENANCEMODE_Add = "A";//Add
        public static String MAINTENANCEMODE_Delete = "D";// Delete
        public static String MAINTENANCEMODE_ReCreate = "R";// Re-Create				
        private MLanguage _language = null;//	The Language			
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        override protected void Prepare()
        { 
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                { }
                else if (name.Equals("MaintenanceMode"))
                {
                    _MaintenanceMode = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _AD_Language_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        ///Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        /// <exception cref="Exception">if not successful</exception>
        override protected String DoIt()
        {
            //IDbTransaction trx = ExecuteQuery.GerServerTransaction();
            //_language = MLanguage.Get(GetCtx(), _AD_Language_ID, trx.ToString());
            _language = MLanguage.Get(GetCtx(), _AD_Language_ID, Get_TrxName());
            log.Info("Mode=" + _MaintenanceMode + ", ID=" + _AD_Language_ID + " - " + _language);

            if (_language.IsBaseLanguage())
            {
                throw new Exception("Base Language has no Translations");
            }
            int deleteNo = 0;
            int insertNo = 0;

            //	Delete the recods of the _trl table
            if (MAINTENANCEMODE_Delete.Equals(_MaintenanceMode)
                || MAINTENANCEMODE_ReCreate.Equals(_MaintenanceMode))
            {
                deleteNo = _language.Maintain(false);
            }
            //	Add the records from the _trl
            if (MAINTENANCEMODE_Add.Equals(_MaintenanceMode)
                || MAINTENANCEMODE_ReCreate.Equals(_MaintenanceMode))
            {
                if (_language.IsActive() && _language.IsSystemLanguage())
                {
                    insertNo = _language.Maintain(true);
                }
                else
                {
                    //MessageBox.Show("Set System Language");
                    throw new Exception("Language not active System Language");
                }
            }
            //	Delete complete the records from _trl table
            if (MAINTENANCEMODE_Delete.Equals(_MaintenanceMode))
            {
                if (_language.IsSystemLanguage())
                {
                    _language.SetIsSystemLanguage(false);
                    _language.Save();
                }
            }
            //MessageBox.Show("Inserted Records:" + insertNo + "\n Deleted Records:" + deleteNo);
            return "@Deleted@=" + deleteNo + " - @Inserted@=" + insertNo;
        }
    }
}
