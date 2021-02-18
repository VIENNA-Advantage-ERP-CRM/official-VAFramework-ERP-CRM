/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyFroMVAGLJRNL
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


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CopyFroMVAGLJRNL : SvrProcess
    {
        private int _VAGL_BatchJRNL_ID = 0;

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
                else if (name.Equals("VAGL_BatchJRNL_ID"))
                {
                    _VAGL_BatchJRNL_ID = Util.GetValueOfInt((Decimal)para[i].GetParameter());
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
            int To_VAGL_BatchJRNL_ID = GetRecord_ID();
            log.Info("doIt - From VAGL_BatchJRNL_ID=" + _VAGL_BatchJRNL_ID + " to " + To_VAGL_BatchJRNL_ID);
            if (To_VAGL_BatchJRNL_ID == 0)
            {
                throw new ArgumentException("Target VAGL_BatchJRNL_ID == 0");
            }
            if (_VAGL_BatchJRNL_ID == 0)
            {
                throw new ArgumentException("Source VAGL_BatchJRNL_ID == 0");
            }
            VAdvantage.Model.MVAGLBatchJRNL from = new VAdvantage.Model.MVAGLBatchJRNL(GetCtx(), _VAGL_BatchJRNL_ID, Get_Trx());
            VAdvantage.Model.MVAGLBatchJRNL to = new VAdvantage.Model.MVAGLBatchJRNL(GetCtx(), To_VAGL_BatchJRNL_ID, Get_Trx());
            //
            int no = to.CopyDetailsFrom(from);
            //
            return "@Copied@=" + no;
        }	//	doIt

    }




}
