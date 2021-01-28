/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WebProjectDeploy
 * Purpose        : Deploy Web Project
 * Class Used     : X_CM_Container
 * Chronological    Development
 * Deepak           12-Feb-2010
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
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class CopyImportFormat : ProcessEngine.SvrProcess
    {
        private int from_VAF_ImportFormat_ID = 0;
        private int to_VAF_ImportFormat_ID = 0;

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAF_ImportFormat_ID"))
                {
                    from_VAF_ImportFormat_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            to_VAF_ImportFormat_ID = GetRecord_ID();
        }	//	prepare


        /// <summary>
        /// Process Copy
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("doIt = From=" + from_VAF_ImportFormat_ID + " To=" + to_VAF_ImportFormat_ID);
            MVAFImportFormat from = new MVAFImportFormat(GetCtx(), from_VAF_ImportFormat_ID, Get_Trx());
            if (from.GetVAF_ImportFormat_ID() != from_VAF_ImportFormat_ID)
            {
                throw new Exception("From Format not found - " + from_VAF_ImportFormat_ID);
            }
            //
            MVAFImportFormat to = new MVAFImportFormat(GetCtx(), to_VAF_ImportFormat_ID, Get_Trx());
            if (to.GetVAF_ImportFormat_ID() != to_VAF_ImportFormat_ID)
            {
                throw new Exception("To Format not found - " + from_VAF_ImportFormat_ID);
            }
            //
            if (from.GetVAF_TableView_ID() != to.GetVAF_TableView_ID())
            {
                throw new Exception("From-To do Not have same Format Table");
            }
            //
            MVAFImportFormatRow[] rows = from.GetRows();	//	incl. inactive
            for (int i = 0; i < rows.Length; i++)
            {
                MVAFImportFormatRow row = rows[i];
                MVAFImportFormatRow copy = new MVAFImportFormatRow(to, row);
                if (!copy.Save())
                {
                    throw new Exception("Copy error");
                }
            }

            String msg = "#" + rows.Length;
            if (!from.GetFormatType().Equals(to.GetFormatType()))
                return msg + " - Note: Format Type different!";
            return msg;
        }	//	doIt

    }
}
