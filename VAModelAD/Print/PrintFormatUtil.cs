/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     13-July-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Print
{
    public class PrintFormatUtil
    {
        private static VLogger log = VLogger.GetVLogger(typeof(PrintFormatUtil).FullName);
        public PrintFormatUtil(Context ctx)
        {
            _ctx = ctx;
        }	//	PrintFormatUtil

        private Context _ctx;

        /// <summary>
        /// Add Missing Columns for all Print Format
        /// </summary>
        public void AddMissingColumns()
        {
            int total = 0;
            String sql = "SELECT * FROM VAF_Print_Rpt_Layout pf "
                + "ORDER BY Name";
            try
            {

                DataSet ds = DataBase.DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    total += AddMissingColumns(new MVAFPrintRptLayout(_ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                log.Severe(e.ToString());
            }

        }	//	addMissingColumns


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pf"></param>
        /// <returns></returns>
        public int AddMissingColumns(MVAFPrintRptLayout pf)
        {
           
            String sql = "SELECT c.VAF_Column_ID, c.ColumnName "
                + "FROM VAF_Column c "
                + "WHERE NOT EXISTS "
                    + "(SELECT * "
                    + "FROM VAF_Print_Rpt_LItem pfi"
                    + " INNER JOIN VAF_Print_Rpt_Layout pf ON (pfi.VAF_Print_Rpt_Layout_ID=pf.VAF_Print_Rpt_Layout_ID) "
                    + "WHERE pf.VAF_TableView_ID=c.VAF_TableView_ID"
                    + " AND pfi.VAF_Column_ID=c.VAF_Column_ID"
                    + " AND pfi.VAF_Print_Rpt_Layout_ID='" + pf.GetVAF_Print_Rpt_Layout_ID() + "')"	//	1 
                + " AND c.VAF_TableView_ID='" + pf.GetVAF_TableView_ID() + "' "				//	2
                + "ORDER BY 1";
            int counter = 0;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    int VAF_Column_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    String ColumnName = dr.GetString(1);
                    MVAFPrintRptLItem pfi = MVAFPrintRptLItem.CreateFromColumn(pf, VAF_Column_ID, 0);
                    if (pfi.Get_ID() != 0)
                    { //log
                    }
                    else
                    { //log 
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Severe(e.ToString());
            }

            return counter;
        }	//	addMissingColumns

    }
}
