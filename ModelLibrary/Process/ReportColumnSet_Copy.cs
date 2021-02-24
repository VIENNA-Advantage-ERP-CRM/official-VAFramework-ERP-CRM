/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ReportColumnSet_Copy
 * Purpose        : Copy Column Set at the end of the Column Set
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

    public class ReportColumnSet_Copy : ProcessEngine.SvrProcess
    {
        /// <summary>
        ///	Constructor
        /// </summary>
        public ReportColumnSet_Copy()
            : base()
        {

        }	//	ReportColumnSet_Copy

        /**	Source Line Set					*/
        private int _VAPA_FR_ColumnSet_ID = 0;

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
                else if (name.Equals("VAPA_FR_ColumnSet_ID"))
                {
                    _VAPA_FR_ColumnSet_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
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
        /// <returns>Message</returns>
        protected override String DoIt()
        {
            int to_ID = base.GetRecord_ID();
            log.Info("From VAPA_FR_ColumnSet_ID=" + _VAPA_FR_ColumnSet_ID + ", To=" + to_ID);
            if (to_ID < 1)
            {
                throw new Exception(msgSaveErrorRowNotFound);
            }
            //
            //MVAPAFRColumnSet to = new MVAPAFRColumnSet(GetCtx(), to_ID, Get_TrxName());
            //MVAPAFRColumnSet rcSet = new MVAPAFRColumnSet(GetCtx(), _VAPA_FR_ColumnSet_ID, Get_TrxName());
            //MVAPAFRColumn[] rcs = rcSet.GetColumns();
            //for (int i = 0; i < rcs.Length; i++)
            //{
            //    MVAPAFRColumn rc = MVAPAFRColumn.Copy(GetCtx(), to.GetVAF_Client_ID(), to.GetVAF_Org_ID(), to_ID, rcs[i], Get_TrxName());
            //    rc.Save();
            //}
            //	Oper 1/2 were set to Null !
            return "@Copied@=" + 0;// rcs.Length;
        }	//	doIt

    }	//	ReportColumnSet_Copy


}
