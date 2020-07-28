/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportDelete
 * Purpose        : Delete Data in Import Table
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           05-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportDelete : ProcessEngine.SvrProcess
    {
        /**	Table be deleted		*/
        private int _AD_Table_ID = 0;

        /// <summary>
        /// Process
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("AD_Table_ID"))
                {
                    _AD_Table_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("AD_Table_ID=" + _AD_Table_ID);
            //	get Table Info
            MTable table = new MTable(GetCtx(), _AD_Table_ID, Get_TrxName());
            if (table.Get_ID() == 0)
            {
                throw new ArgumentException("No AD_Table_ID=" + _AD_Table_ID);
            }
            String tableName = table.GetTableName();
            if (!tableName.StartsWith("I"))
            {
                throw new ArgumentException("Not an import table = " + tableName);
            }

            //	Delete
            String sql = "DELETE FROM " + tableName + " WHERE AD_Client_ID=" + GetAD_Client_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            String msg = Msg.Translate(GetCtx(), tableName + "_ID") + " #" + no;
            return msg;
        }	//	ImportDelete

    }

}
