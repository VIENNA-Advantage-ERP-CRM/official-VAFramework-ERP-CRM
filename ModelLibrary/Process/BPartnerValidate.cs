/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BPartnerValidate
 * Purpose        : Validate Business Partner	 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           07-Nov-2009
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BPartnerValidate : ProcessEngine.SvrProcess
    {
        //	BPartner ID		
        int _C_BPartner_ID = 0;
        // BPartner Group		*/
        int _C_BP_Group_ID = 0;

        /// <summary>
        ///	Prepare	 
        /// </summary>
        protected override void Prepare()
        {
            _C_BPartner_ID = GetRecord_ID();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("C_BPartner_ID=" + _C_BPartner_ID + ", C_BP_Group_ID=" + _C_BP_Group_ID);
            if (_C_BP_Group_ID == -1)
            {
                _C_BP_Group_ID = 0;
            }
            if (_C_BPartner_ID == 0 && _C_BP_Group_ID == 0)
            {
                throw new Exception("No Business Partner/Group selected");

            }
            if (_C_BP_Group_ID == 0)
            {
                MBPartner bp = new MBPartner(GetCtx(), _C_BPartner_ID, Get_Trx());
                if (bp.Get_ID() == 0)
                {
                    throw new Exception("Business Partner not found - C_BPartner_ID=" + _C_BPartner_ID);
                }
                CheckBP(bp);
            }
            else
            {
                String sql = "SELECT * FROM C_BPartner WHERE C_BP_Group_ID=@Param1 AND IsActive='Y'";
                SqlParameter[] Param = new SqlParameter[1];
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    Param[0] = new SqlParameter("@Param1", _C_BP_Group_ID);
                    idr = DataBase.DB.ExecuteReader(sql, Param, Get_Trx());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        MBPartner bp = new MBPartner(GetCtx(), dr, Get_Trx());

                        CheckBP(bp);
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
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
            }
           
            return "OK";
        }

        /// <summary>
        /// Check BP
        /// </summary>
        /// <param name="bp">bp bp</param>
        private void CheckBP(MBPartner bp)
        {
            AddLog(0, null, null, bp.GetName() + ":");
            //	See also VMerge.postMerge
            CheckPayments(bp);
            CheckInvoices(bp);
            //	
            bp.SetTotalOpenBalance();
            bp.SetActualLifeTimeValue();
            bp.Save();
            //
            //	if (bp.getSO_CreditUsed().signum() != 0)
            AddLog(0, null, bp.GetSO_CreditUsed(), Msg.GetElement(GetCtx(), "SO_CreditUsed"));
            AddLog(0, null, bp.GetTotalOpenBalance(), Msg.GetElement(GetCtx(), "TotalOpenBalance"));
            AddLog(0, null, bp.GetActualLifeTimeValue(), Msg.GetElement(GetCtx(), "ActualLifeTimeValue"));
            //
            Commit();
            
        }	//	checkBP


        /// <summary>
        /// Check Payments
        /// </summary>
        /// <param name="bp">bp business partner</param>
        private void CheckPayments(MBPartner bp)
        {
            //	See also VMerge.postMerge
            int changed = 0;
            MPayment[] payments = MPayment.GetOfBPartner(GetCtx(), bp.GetC_BPartner_ID(), Get_Trx());
            for (int i = 0; i < payments.Length; i++)
            {
                MPayment payment = payments[i];
                if (payment.TestAllocation())
                {
                    payment.Save();
                    changed++;
                }
            }
            if (changed != 0)
            {
                AddLog(0, null, new Decimal(payments.Length),
                    Msg.GetElement(GetCtx(), "C_Payment_ID") + " - #" + changed);
            }
        }	//	checkPayments

        /// <summary>
        /// Check Invoices
        /// </summary>
        /// <param name="bp">bp business partner</param>
        private void CheckInvoices(MBPartner bp)
        {
            //	See also VMerge.postMerge
            int changed = 0;
            MInvoice[] invoices = MInvoice.GetOfBPartner(GetCtx(), bp.GetC_BPartner_ID(), Get_Trx());
            for (int i = 0; i < invoices.Length; i++)
            {
                MInvoice invoice = invoices[i];
                if (invoice.TestAllocation())
                {
                    invoice.Save();
                    changed++;
                }
            }
            if (changed != 0)
            {
                AddLog(0, null, new Decimal(invoices.Length),
                    Msg.GetElement(GetCtx(), "C_Invoice_ID") + " - #" + changed);
            }
        }	//	checkInvoices

    }	//	BPartnerValidate

}
