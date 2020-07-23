/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ReportLineSet_Copy
 * Purpose        : Copy Line Set at the end of the Line Set
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           14-Jan-2010
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
//////using VAdvantage.Report;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{

    public class ReportLineSet_Copy : ProcessEngine.SvrProcess
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReportLineSet_Copy()
            : base()
        {

        }	//	ReportLineSet_Copy

        /**	Source Line Set					*/
        private int _PA_ReportLineSet_ID = 0;

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
                else if (name.Equals("PA_ReportLineSet_ID"))
                {
                    _PA_ReportLineSet_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        ///  Perform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            int to_ID = base.GetRecord_ID();
            //log.Info("From PA_ReportLineSet_ID=" + _PA_ReportLineSet_ID + ", To=" + to_ID);
            //if (to_ID < 1)
            //{
            //    throw new Exception(msgSaveErrorRowNotFound);
            //}
            ////
            //MReportLineSet to = new MReportLineSet(GetCtx(), to_ID, Get_TrxName());
            //MReportLineSet rlSet = new MReportLineSet(GetCtx(), _PA_ReportLineSet_ID, Get_TrxName());
            //MReportLine[] rls = rlSet.GetLiness();
            //for (int i = 0; i < rls.Length; i++)
            //{
            //    MReportLine rl = MReportLine.Copy(GetCtx(), to.GetAD_Client_ID(), to.GetAD_Org_ID(), to_ID, rls[i], Get_TrxName());
            //    rl.Save();
            //    MReportSource[] rss = rls[i].GetSources();
            //    if (rss != null)
            //    {
            //        for (int ii = 0; ii < rss.Length; ii++)
            //        {
            //            MReportSource rs = MReportSource.Copy(GetCtx(), to.GetAD_Client_ID(), to.GetAD_Org_ID(), rl.Get_ID(), rss[ii], Get_TrxName());
            //            rs.Save();
            //        }
            //    }
            //    //	Oper 1/2 were set to Null ! 
            //}
            return "@Copied@=" + 0;// rls.Length;
        }	//	doIt

    }	//	ReportLineSet_Copy




}
