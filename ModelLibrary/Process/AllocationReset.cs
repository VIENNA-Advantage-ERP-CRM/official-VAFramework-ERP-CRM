/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AllocationReset
 * Purpose        : Reset (delete) Allocations	 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          05-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class AllocationReset : ProcessEngine.SvrProcess
    {
        #region Private variable
        // BP Group				
        private int _VAB_BPart_Category_ID = 0;
        // BPartner				
        private int _VAB_BusinessPartner_ID = 0;
        // Date Acct From			
        private DateTime? _DateAcct_From = null;
        // Date Acct To			
        private DateTime? _DateAcct_To = null;
        // Allocation directly		
        private int _VAB_DocAllocation_ID = 0;
        // Transaction				
        private Trx _m_trx = null;

        #endregion

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                log.Fine("prepare - " + para[i]);
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_BPart_Category_ID"))
                {
                    _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_DocAllocation_ID"))
                {
                    _VAB_DocAllocation_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateAcct"))
                {
                    _DateAcct_From = (DateTime?)para[i].GetParameter();
                    _DateAcct_To = (DateTime?)para[i].GetParameter_To();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// 	Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ", VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID
                + ", DateAcct= " + _DateAcct_From + " - " + _DateAcct_To
                + ", VAB_DocAllocation_ID=" + _VAB_DocAllocation_ID);

            _m_trx = Trx.Get("AllocReset");
            int count = 0;

            if (_VAB_DocAllocation_ID != 0)
            {
                MAllocationHdr hdr = new MAllocationHdr(GetCtx(), _VAB_DocAllocation_ID, _m_trx);
                if (Delete(hdr))
                {
                    count++;
                }
                _m_trx.Close();
                return "@Deleted@ #" + count;
            }

            //	Selection
            StringBuilder sql = new StringBuilder("SELECT * FROM VAB_DocAllocation ah "
                + "WHERE EXISTS (SELECT * FROM VAB_DocAllocationLine al "
                    + "WHERE ah.VAB_DocAllocation_ID=al.VAB_DocAllocation_ID");
            if (_VAB_BusinessPartner_ID != 0)
            {
                sql.Append(" AND al.VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);
            }
            else if (_VAB_BPart_Category_ID != 0 && _VAB_BPart_Category_ID != -1)
            {
                sql.Append(" AND EXISTS (SELECT * FROM VAB_BusinessPartner bp "
                    + "WHERE bp.VAB_BusinessPartner_ID=al.VAB_BusinessPartner_ID AND bp.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ")");
            }
            else
            {
                sql.Append(" AND VAF_Client_ID=" + GetCtx().GetVAF_Client_ID());
            }
            if (_DateAcct_From != null)
            {
                sql.Append(" AND TRIM(ah.DateAcct) >=" + GlobalVariable.TO_DATE(_DateAcct_From, true));
            }
            if (_DateAcct_To != null)
            {
                sql.Append(" AND TRIM(ah.DateAcct) <= " + GlobalVariable.TO_DATE(_DateAcct_To, true));
            }
            //	Do not delete Cash Trx
            sql.Append(" AND al.VAB_CashJRNLLine_ID IS NULL)");
            //	Open Period
            sql.Append(" AND EXISTS (SELECT * FROM C_Period p"
                + " INNER JOIN C_PeriodControl pc ON (p.C_Period_ID=pc.C_Period_ID AND pc.DocBaseType='CMA') "
                + "WHERE ah.DateAcct BETWEEN p.StartDate AND p.EndDate)");

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, _m_trx);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    count++;
                    Delete(new MAllocationHdr(GetCtx(), dr, _m_trx));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
                if (idr != null)
                {
                    idr.Close();
                }
                _m_trx.Rollback();
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
                _m_trx.Close();
            }


            return "@Deleted@ #" + count;
        }

        /// <summary>
        /// Delect recordes from allocation
        /// </summary>
        /// <param name="hdr"></param>
        /// <returns>Bool type</returns>
        private Boolean Delete(MAllocationHdr hdr)
        {
            //	_m_trx.start();
            Boolean success = false;

            String msg = null;

            //	Std Period open?
            msg = DocumentEngine.IsPeriodOpen(hdr);
            if (msg != null)
            {
                msg = "@DeleteError@" + hdr.GetDocumentNo() + ": " + msg;
                log.Warning(msg);
                return false;
            }

            if (hdr.Delete(true, _m_trx))
            {
                log.Fine(hdr.ToString());
                success = true;
            }
            if (success)
            {
                _m_trx.Commit();
            }
            else
            {
                _m_trx.Rollback();
            }
            return success;
        }

        /// <summary>
        /// Set BPartner (may not be required
        ///  </summary>
        private void SetBPartner()
        {
            /**
            UPDATE VAB_DocAllocationLine al
            SET VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartner_ID FROM C_Payment p WHERE al.C_Payment_ID=p.C_Payment_ID)
            WHERE VAB_BusinessPartner_ID IS NULL AND C_Payment_ID IS NOT NULL;
            UPDATE VAB_DocAllocationLine al
            SET VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartner_ID FROM VAB_Invoice i WHERE al.VAB_Invoice_ID=i.VAB_Invoice_ID)
            WHERE VAB_BusinessPartner_ID IS NULL AND VAB_Invoice_ID IS NOT NULL;
            UPDATE VAB_DocAllocationLine al
            SET VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartner_ID FROM C_Order o WHERE al.C_Order_ID=o.C_Order_ID)
            WHERE VAB_BusinessPartner_ID IS NULL AND C_Order_ID IS NOT NULL;
            COMMIT
            **/
        }

    }

}
