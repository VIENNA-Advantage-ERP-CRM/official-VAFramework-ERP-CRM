/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyFromJournal
 * Purpose        : Copy GL Batch Journal/Lines
 * Class Used     : SvrProcess
 * Chronological    Development
 * Deepak           21-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ViennaAdvantage.Process;
using VAdvantage.Classes;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Report;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CopyFromJournal : SvrProcess
    {
        private int _GL_JournalBatch_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
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
                else if (name.Equals("GL_JournalBatch_ID"))
                {
                    _GL_JournalBatch_ID = Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            int To_GL_JournalBatch_ID = GetRecord_ID();
            log.Info("doIt - From GL_JournalBatch_ID=" + _GL_JournalBatch_ID + " to " + To_GL_JournalBatch_ID);
            if (To_GL_JournalBatch_ID == 0)
            {
                throw new ArgumentException("Target GL_JournalBatch_ID == 0");
            }
            if (_GL_JournalBatch_ID == 0)
            {
                throw new ArgumentException("Source GL_JournalBatch_ID == 0");
            }
            VAdvantage.Model.MJournalBatch from = new VAdvantage.Model.MJournalBatch(GetCtx(), _GL_JournalBatch_ID, Get_Trx());
            VAdvantage.Model.MJournalBatch to = new VAdvantage.Model.MJournalBatch(GetCtx(), To_GL_JournalBatch_ID, Get_Trx());
            //
            int no = to.CopyDetailsFrom(from);
            //
            return "@Copied@=" + no;
        }	//	doIt

    }




}
