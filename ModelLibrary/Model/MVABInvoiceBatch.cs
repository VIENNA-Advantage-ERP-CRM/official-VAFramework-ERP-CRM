/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABInvoiceBatch
 * Purpose        : Invoice batch line
 * Class Used     : X_VAB_BatchInvoice
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model

{
    public class MVABInvoiceBatch : X_VAB_BatchInvoice
    {
        /**	The Lines						*/
        private MVABInvoiceBatchLine[] _lines = null;

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAB_BatchInvoice_ID id
         *	@param trxName trx
         */
        public MVABInvoiceBatch(Ctx ctx, int VAB_BatchInvoice_ID, Trx trxName) :
            base(ctx, VAB_BatchInvoice_ID, trxName)
        {

            if (VAB_BatchInvoice_ID == 0)
            {
                //	setDocumentNo (null);
                //	setVAB_Currency_ID (0);	// @$VAB_Currency_ID@
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
        public MVABInvoiceBatch(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Lines
         *	@param reload reload data
         *	@return array of lines
         */
        public MVABInvoiceBatchLine[] GetLines(Boolean reload)
        {
            if (_lines != null && !reload)
                return _lines;
            String sql = "SELECT * FROM VAB_BatchInvoiceLine WHERE VAB_BatchInvoice_ID=@VAB_BatchInvoice_ID ORDER BY Line";
            List<MVABInvoiceBatchLine> list = new List<MVABInvoiceBatchLine>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAB_BatchInvoice_ID", GetVAB_BatchInvoice_ID());
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABInvoiceBatchLine(GetCtx(), dr, Get_TrxName()));
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
            _lines = new MVABInvoiceBatchLine[list.Count];
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
                + "' WHERE VAB_BatchInvoice_ID=" + GetVAB_BatchInvoice_ID();
            int noLine = DataBase.DB.ExecuteQuery("UPDATE VAB_BatchInvoiceLine " + set, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }	//	setProcessed
    }
}
