/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInvoiceBatch
 * Purpose        : Invoice batch line
 * Class Used     : X_C_InvoiceBatch
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model

{
    public class MInvoiceBatch : X_C_InvoiceBatch
    {
        /**	The Lines						*/
        private MInvoiceBatchLine[] _lines = null;

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_InvoiceBatch_ID id
         *	@param trxName trx
         */
        public MInvoiceBatch(Ctx ctx, int C_InvoiceBatch_ID, Trx trxName) :
            base(ctx, C_InvoiceBatch_ID, trxName)
        {

            if (C_InvoiceBatch_ID == 0)
            {
                //	setDocumentNo (null);
                //	setC_Currency_ID (0);	// @$C_Currency_ID@
                SetControlAmt(Env.ZERO);	// 0
                SetDateDoc(DateTime.Now);	// @#Date@
                SetDocumentAmt(Env.ZERO);
                SetIsSOTrx(false);	// N
                SetProcessed(false);
                //	setSalesRep_ID (0);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MInvoiceBatch(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Lines
         *	@param reload reload data
         *	@return array of lines
         */
        public MInvoiceBatchLine[] GetLines(Boolean reload)
        {
            if (_lines != null && !reload)
                return _lines;
            String sql = "SELECT * FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=@C_InvoiceBatch_ID ORDER BY Line";
            List<MInvoiceBatchLine> list = new List<MInvoiceBatchLine>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_InvoiceBatch_ID", GetC_InvoiceBatch_ID());
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInvoiceBatchLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            _lines = new MInvoiceBatchLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }


        /**
         * 	Set Processed
         *	@param processed processed
         */
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String set = "SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_InvoiceBatch_ID=" + GetC_InvoiceBatch_ID();
            int noLine = DataBase.DB.ExecuteQuery("UPDATE C_InvoiceBatchLine " + set, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }	//	setProcessed
    }
}
