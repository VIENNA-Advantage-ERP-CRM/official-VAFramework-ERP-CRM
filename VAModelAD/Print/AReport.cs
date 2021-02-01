/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     17-June-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

using System.Windows.Forms;
using VAdvantage.Process;

namespace VAdvantage.Print
{
    public class AReport
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="VAF_TableView_ID">Table ID</param>
        /// <param name="invoker">object which invoked the event</param>
        /// <param name="query">query</param>
        public AReport(int VAF_TableView_ID, ToolStripDropDownButton sender, Query query)
        {
            if (!MVAFRole.GetDefault(Env.GetContext()).IsCanReport(VAF_TableView_ID))
            {
                //ShowMessage.Error("AccessCannotReport", true, query.GetTableName());
                return;
            }

            _query = query;
            //_isParent = isParent;

            GetPrintFormats(VAF_TableView_ID, sender);
        }

        /**	The Query						*/
        private Query _query;
        //private bool _isParent;


        //saves the list
        private List<KeyNamePair> _list = new List<KeyNamePair>();

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(AReport).FullName);

        /// <summary>
        /// Get the Print Formats for the table.
        /// Fill the list and the popup menu
        /// </summary>
        /// <param name="VAF_TableView_ID">table</param>
        private void GetPrintFormats(int VAF_TableView_ID, ToolStripDropDownButton sender)
        {
            ToolStripDropDownButton _popup = sender;

            int VAF_Client_ID = Env.GetContext().GetVAF_Client_ID();
            //
            String sql = MVAFRole.GetDefault(Env.GetContext()).AddAccessSQL(
                "SELECT VAF_Print_Rpt_Layout_ID, Name, VAF_Client_ID "
                    + "FROM VAF_Print_Rpt_Layout "
                    + "WHERE VAF_TableView_ID='" + VAF_TableView_ID + "' AND IsTableBased='Y' "
                    + "ORDER BY VAF_Client_ID DESC, IsDefault DESC, Name",		//	Own First
                "VAF_Print_Rpt_Layout", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);

            KeyNamePair pp = null;

            IDataReader dr = DataBase.DB.ExecuteReader(sql);
            try
            {

                if (sender != null)
                    _popup.DropDownItems.Clear();
                while (dr.Read())
                {

                    if (Utility.Util.GetValueOfInt(dr[2].ToString()) == VAF_Client_ID)
                    {
                        pp = new KeyNamePair(Utility.Util.GetValueOfInt(dr[0].ToString()), dr[1].ToString());
                        _list.Add(pp);
                        String actionCommand = pp.ToString();
                        ToolStripMenuItem mi = new System.Windows.Forms.ToolStripMenuItem();
                        mi.Text = actionCommand;
                        mi.Click += new EventHandler(ReportFormat_Click);
                        if (sender != null)
                            _popup.DropDownItems.Add(mi);
                    }

                }
                dr.Close();
                dr = null;

            }
            catch 
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            if (_list.Count == 0)
            {
                if (pp == null)
                    CreateNewFormat(VAF_TableView_ID);		//	calls launch
                else
                    CopyFormat(pp.GetKey(), VAF_Client_ID);
            }
            //	One Format exists or no invoker - show it
            else if (_list.Count == 1 || sender == null)
            {
                if (sender != null)
                    _popup.DropDownItems.Clear();
                LaunchReport((KeyNamePair)_list[0]);
            }
            //	Multiple Formats exist - show selection
            else
                _popup.ShowDropDown();

        }

        /// <summary>
        /// Launch Report
        /// </summary>
        /// <param name="pp">KeyNamePair values</param>
        private void LaunchReport(KeyNamePair pp)
        {
            MVAFPrintRptLayout pf = MVAFPrintRptLayout.Get(Env.GetContext(), pp.GetKey(), true);
            LaunchReport(pf);
        }	//	launchReport


        /// <summary>
        /// Launch Report
        /// </summary>
        /// <param name="pf">PrintFormat Object</param>
        private void LaunchReport(MVAFPrintRptLayout pf)
        {
            //Code to preload the format (not include as user can change setting at the last moment )
            int Record_ID = 0;
            if (_query.GetRestrictionCount() == 1 && (_query.GetCode(0)).GetType() == typeof(int))
                Record_ID = ((int)_query.GetCode(0));
            PrintInfo info = new PrintInfo(pf.GetName(), pf.GetVAF_TableView_ID(), Record_ID);
            info.SetDescription(_query.GetInfo());

            ReportEngine_N re = new ReportEngine_N(Env.GetContext(), pf, _query, info);
            //new Viewer(re);
            //new ReportDialog(pf,  _query);
        }	//	launchReport



        private void ReportFormat_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                KeyNamePair pp = (KeyNamePair)_list[i];
                if (sender.ToString() == pp.GetName())
                {
                    LaunchReport(pp);
                    return;
                }
            }

        }
        /// <summary>
        /// Creates the new format
        /// </summary>
        /// <param name="VAF_TableView_ID">table id</param>
        private void CreateNewFormat(int VAF_TableView_ID)
        {
            MVAFPrintRptLayout pf = MVAFPrintRptLayout.CreateFromTable(Env.GetContext(), VAF_TableView_ID);
            LaunchReport(pf);
        }	//	createNewFormat


        /// <summary>
        /// Copy the existing format
        /// </summary>
        /// <param name="VAF_Print_Rpt_Layout_ID">print format id</param>
        /// <param name="To_Client_ID">client id</param>
        private void CopyFormat(int VAF_Print_Rpt_Layout_ID, int To_Client_ID)
        {
            MVAFPrintRptLayout pf = MVAFPrintRptLayout.CopyToClient(Env.GetContext(), VAF_Print_Rpt_Layout_ID, To_Client_ID);
            LaunchReport(pf);
        }	//	copyFormatFromClient


        /**************************************************************************
         * 	Get VAF_TableView_ID for Table Name
         * 	@param TableName table name
         * 	@return VAF_TableView_ID or 0
         */
        static public int GetVAF_TableView_ID(String TableName)
        {
            int VAF_TableView_ID = 0;
            String sql = "SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName=@tablename";
            IDataReader dr =null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@tablename", TableName);
              dr  = DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                    VAF_TableView_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                dr.Close();
            }
            catch (SqlException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            return VAF_TableView_ID;
        }	//	getVAF_TableView_ID
    }


}
