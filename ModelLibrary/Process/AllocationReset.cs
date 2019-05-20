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
using System.Windows.Forms;

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
        private int _C_BP_Group_ID = 0;
        // BPartner				
        private int _C_BPartner_ID = 0;
        // Date Acct From			
        private DateTime? _DateAcct_From = null;
        // Date Acct To			
        private DateTime? _DateAcct_To = null;
        // Allocation directly		
        private int _C_AllocationHdr_ID = 0;
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
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_AllocationHdr_ID"))
                {
                    _C_AllocationHdr_ID = para[i].GetParameterAsInt();
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
            log.Info("C_BP_Group_ID=" + _C_BP_Group_ID + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", DateAcct= " + _DateAcct_From + " - " + _DateAcct_To
                + ", C_AllocationHdr_ID=" + _C_AllocationHdr_ID);

            _m_trx = Trx.Get("AllocReset");
            int count = 0;

            if (_C_AllocationHdr_ID != 0)
            {
                MAllocationHdr hdr = new MAllocationHdr(GetCtx(), _C_AllocationHdr_ID, _m_trx);
                if (Delete(hdr))
                {
                    count++;
                }
                _m_trx.Close();
                return "@Deleted@ #" + count;
            }

            //	Selection
            StringBuilder sql = new StringBuilder("SELECT * FROM C_AllocationHdr ah "
                + "WHERE EXISTS (SELECT * FROM C_AllocationLine al "
                    + "WHERE ah.C_AllocationHdr_ID=al.C_AllocationHdr_ID");
            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND al.C_BPartner_ID=" + _C_BPartner_ID);
            }
            else if (_C_BP_Group_ID != 0 && _C_BP_Group_ID != -1)
            {
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE bp.C_BPartner_ID=al.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");
            }
            else
            {
                sql.Append(" AND AD_Client_ID=" + GetCtx().GetAD_Client_ID());
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
            sql.Append(" AND al.C_CashLine_ID IS NULL)");
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
            UPDATE C_AllocationLine al
            SET C_BPartner_ID=(SELECT C_BPartner_ID FROM C_Payment p WHERE al.C_Payment_ID=p.C_Payment_ID)
            WHERE C_BPartner_ID IS NULL AND C_Payment_ID IS NOT NULL;
            UPDATE C_AllocationLine al
            SET C_BPartner_ID=(SELECT C_BPartner_ID FROM C_Invoice i WHERE al.C_Invoice_ID=i.C_Invoice_ID)
            WHERE C_BPartner_ID IS NULL AND C_Invoice_ID IS NOT NULL;
            UPDATE C_AllocationLine al
            SET C_BPartner_ID=(SELECT C_BPartner_ID FROM C_Order o WHERE al.C_Order_ID=o.C_Order_ID)
            WHERE C_BPartner_ID IS NULL AND C_Order_ID IS NOT NULL;
            COMMIT
            **/
        }

    }

}
