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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BPartnerValidate : ProcessEngine.SvrProcess
    {
        //	BPartner ID		
        int _VAB_BusinessPartner_ID = 0;
        // BPartner Group		*/
        int _VAB_BPart_Category_ID = 0;

        /// <summary>
        ///	Prepare	 
        /// </summary>
        protected override void Prepare()
        {
            _VAB_BusinessPartner_ID = GetRecord_ID();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_BPart_Category_ID"))
                {
                    _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
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
            log.Info("VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID + ", VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID);
            if (_VAB_BPart_Category_ID == -1)
            {
                _VAB_BPart_Category_ID = 0;
            }
            if (_VAB_BusinessPartner_ID == 0 && _VAB_BPart_Category_ID == 0)
            {
                throw new Exception("No Business Partner/Group selected");

            }
            if (_VAB_BPart_Category_ID == 0)
            {
                MBPartner bp = new MBPartner(GetCtx(), _VAB_BusinessPartner_ID, Get_Trx());
                if (bp.Get_ID() == 0)
                {
                    throw new Exception("Business Partner not found - VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);
                }
                CheckBP(bp);
            }
            else
            {
                String sql = "SELECT * FROM VAB_BusinessPartner WHERE VAB_BPart_Category_ID=@Param1 AND IsActive='Y'";
                SqlParameter[] Param = new SqlParameter[1];
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    Param[0] = new SqlParameter("@Param1", _VAB_BPart_Category_ID);
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
            MPayment[] payments = MPayment.GetOfBPartner(GetCtx(), bp.GetVAB_BusinessPartner_ID(), Get_Trx());
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
                    Msg.GetElement(GetCtx(), "VAB_Payment_ID") + " - #" + changed);
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
            MInvoice[] invoices = MInvoice.GetOfBPartner(GetCtx(), bp.GetVAB_BusinessPartner_ID(), Get_Trx());
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
                    Msg.GetElement(GetCtx(), "VAB_Invoice_ID") + " - #" + changed);
            }
        }	//	checkInvoices

    }	//	BPartnerValidate

}
