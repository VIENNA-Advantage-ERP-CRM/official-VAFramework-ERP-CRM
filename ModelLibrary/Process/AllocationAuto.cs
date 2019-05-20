/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AllocationAuto
 * Purpose        : Automatic Allocation Process	 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          06-Nov-2009
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
    public class AllocationAuto : ProcessEngine.SvrProcess
    {
        #region
        //	BP Group					
        private int _C_BP_Group_ID = 0;
        // BPartner					
        private int _C_BPartner_ID = 0;
        // Allocate Oldest Setting		
        private Boolean _AllocateOldest = true;
        //Only AP/AR Transactions		
        private String _APAR = "A";

        private static String _ONLY_AP = "P";
        private static String _ONLY_AR = "R";

        //	Payments				
        private MPayment[] _payments = null;
        // Invoices				
        private MInvoice[] _invoices = null;
        //	Allocation				
        private MAllocationHdr _allocation = null;
        #endregion

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
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("_C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("AllocateOldest"))
                {
                    _AllocateOldest = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("APAR"))
                {
                    _APAR = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        ///Process         
        /// </summary>
        /// <returns> message</returns>
        /// 
        protected override String DoIt()
        {
            log.Info("C_BP_Group_ID=" + _C_BP_Group_ID
                + ", _C_BPartner_ID=" + _C_BPartner_ID
                + ", Oldest=" + _AllocateOldest
                + ", AP/AR=" + _APAR);
            int _countBP = 0;
            int _countAlloc = 0;
            if (_C_BPartner_ID != 0)
            {
                _countAlloc = AllocateBP(_C_BPartner_ID);
                if (_countAlloc > 0)
                    _countBP++;
            }
            else if (_C_BP_Group_ID != 0 && _C_BP_Group_ID != -1)
            {
                String sql = "SELECT C_BPartner_ID FROM C_BPartner WHERE C_BP_Group_ID=@param1 ORDER BY Value";
                SqlParameter[] param = new SqlParameter[1];
                DataTable dt = null;
                IDataReader idr = null;

                try
                {
                    param[0] = new SqlParameter("@param1", _C_BP_Group_ID);
                    idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        int C_BPartner_ID = Utility.Util.GetValueOfInt(dr[0]);
                        int _count = AllocateBP(C_BPartner_ID);
                        if (_count > 0)
                        {
                            _countBP++;
                            _countAlloc += _count;
                            Commit();
                        }


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
            else
            {
                String sql = "SELECT C_BPartner_ID FROM C_BPartner WHERE AD_Client_ID=@param1 ORDER BY Value";
                SqlParameter[] param = new SqlParameter[1];
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    param[0] = new SqlParameter("@param1", GetCtx().GetAD_Client_ID());

                    idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        int C_BPartner_ID = Utility.Util.GetValueOfInt(dr[0]);
                        int _count = AllocateBP(C_BPartner_ID);
                        if (_count > 0)
                        {
                            _countBP++;
                            _countAlloc += _count;
                            Commit();
                        }
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
            //
            return "@Created@ #" + _countBP + "/" + _countAlloc;
        }	//	doIt

        /// <summary>
        ///	Allocate BP       
        /// </summary>
        /// <param name="_C_BPartner_ID"></param>
        /// <returns>number of allocations</returns>
        private int AllocateBP(int _C_BPartner_ID)
        {
            GetPayments(_C_BPartner_ID);
            GetInvoices(_C_BPartner_ID);
            log.Info("(1) - _C_BPartner_ID=" + _C_BPartner_ID
                + " - #Payments=" + _payments.Length + ", #Invoices=" + _invoices.Length);
            if (_payments.Length + _invoices.Length < 2)
            {
                return 0;
            }
            //	Payment Info - Invoice or Pay Selection
            int _count = allocateBPPaymentWithInfo();
            if (_count != 0)
            {
                GetPayments(_C_BPartner_ID);		//	for next
                GetInvoices(_C_BPartner_ID);
                log.Info("(2) - _C_BPartner_ID=" + _C_BPartner_ID
                    + " - #Payments=" + _payments.Length + ", #Invoices=" + _invoices.Length);
                if (_payments.Length + _invoices.Length < 2)
                {
                    return _count;
                }
            }

            //	All
            int _newCount = AllocateBPartnerAll();
            if (_newCount != 0)
            {
                _count += _newCount;
                GetPayments(_C_BPartner_ID);		//	for next
                GetInvoices(_C_BPartner_ID);
                ProcessAllocation();
                log.Info("(3) - _C_BPartner_ID=" + _C_BPartner_ID
                    + " - #Payments=" + _payments.Length + ", #Invoices=" + _invoices.Length);
                if (_payments.Length + _invoices.Length < 2)
                {
                    return _count;
                }
            }

            //	One:One
            _newCount = AllocateBPOneToOne();
            if (_newCount != 0)
            {
                _count += _newCount;
                GetPayments(_C_BPartner_ID);		//	for next
                GetInvoices(_C_BPartner_ID);
                ProcessAllocation();
                log.Info("(4) - _C_BPartner_ID=" + _C_BPartner_ID
                    + " - #Payments=" + _payments.Length + ", #Invoices=" + _invoices.Length);
                if (_payments.Length + _invoices.Length < 2)
                {
                    return _count;
                }
            }

            //	Oldest First
            if (_AllocateOldest)
            {
                _newCount = AllocateBPOldestFirst();
                if (_newCount != 0)
                {
                    _count += _newCount;
                    GetPayments(_C_BPartner_ID);		//	for next
                    GetInvoices(_C_BPartner_ID);
                    ProcessAllocation();
                    log.Info("(5) - _C_BPartner_ID=" + _C_BPartner_ID
                        + " - #Payments=" + _payments.Length + ", #Invoices=" + _invoices.Length);
                    if (_payments.Length + _invoices.Length < 2)
                    {
                        return _count;
                    }
                }
            }

            //	Other, e.g.
            //	Allocation if "close" % and $

            return _count;
        }	//	alloc

        /// <summary>
        ///	Get Payments of BP
        /// </summary>
        /// <param name="_C_BPartner_ID"></param>
        /// <returns>unallocated payments</returns>
        private MPayment[] GetPayments(int _C_BPartner_ID)
        {
            //ArrayList<MPayment> list = new ArrayList<MPayment>();
            List<MPayment> list = new List<MPayment>();
            String sql = "SELECT * FROM C_Payment "
                + "WHERE IsAllocated='N' AND Processed='Y' AND C_BPartner_ID=@param1"// + _C_BPartner_ID
                + " AND IsPrepayment='N' AND C_Charge_ID IS NULL ";
            if (_ONLY_AP.Equals(_APAR))
            {
                sql += "AND IsReceipt='N' ";
            }
            else if (_ONLY_AR.Equals(_APAR))
            {
                sql += "AND IsReceipt='Y' ";
            }
            sql += "ORDER BY DateTrx";
            SqlParameter[] param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@param1", _C_BPartner_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                //idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MPayment _payment = new MPayment(GetCtx(), dr, Get_Trx());
                    Decimal allocated = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                    if (allocated.CompareTo(_payment.GetPayAmt()) == 0)
                    {
                        _payment.SetIsAllocated(true);
                        _payment.Save();
                    }
                    else
                    {
                        list.Add(_payment);
                    }
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

            _payments = new MPayment[list.Count];
            _payments = list.ToArray();
            return _payments;
        }

        /// <summary>
        /// Get Invoices of BP
        /// </summary>
        /// <param name="_C_BPartner_ID"></param>
        /// <returns>unallocated Invoices</returns>
        private MInvoice[] GetInvoices(int _C_BPartner_ID)
        {
            //ArrayList<MInvoice> list = new ArrayList<MInvoice>();
            List<MInvoice> list = new List<MInvoice>();
            String sql = "SELECT * FROM C_Invoice "
                + "WHERE IsPaid='N' AND Processed='Y' AND C_BPartner_ID=@param1 ";
            if (_ONLY_AP.Equals(_APAR))
                sql += "AND IsSOTrx='N' ";
            else if (_ONLY_AR.Equals(_APAR))
                sql += "AND IsSOTrx='Y' ";
            sql += "ORDER BY DateInvoiced";
            ;
            SqlParameter[] param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@param1", _C_BPartner_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MInvoice _invoice = new MInvoice(GetCtx(), dr, Get_Trx());
                    if (Env.Signum(Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(false, null))) == 0)
                    {
                        _invoice.SetIsPaid(true);
                        _invoice.Save();
                    }
                    else
                    {
                        list.Add(_invoice);
                    }
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
            _invoices = new MInvoice[list.Count()];
            _invoices = list.ToArray();
            return _invoices;
        }



        /// <summary>
        ///  	Allocate Individual Payments with _payment references
        /// </summary>
        /// <returns>number of allocations</returns>

        private int allocateBPPaymentWithInfo()
        {
            int _count = 0;

            //****	See if there is a direct link (Invoice or Pay Selection)
            for (int p = 0; p < _payments.Length; p++)
            {
                MPayment _payment = _payments[p];
                if (_payment.IsAllocated())
                {
                    continue;
                }
                Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                log.Info(_payment + ", Allocated=" + _allocatedAmt);
                if (Env.Signum(_allocatedAmt) != 0)
                    continue;
                Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt(),

                    _payment.GetDiscountAmt()),
                    _payment.GetWriteOffAmt()),
                    _payment.GetOverUnderAmt());
                if (!_payment.IsReceipt())
                {
                    _availableAmt = Decimal.Negate(_availableAmt);
                }
                log.Fine("Available=" + _availableAmt);
                //
                if (_payment.GetC_Invoice_ID() != 0)
                {
                    for (int i = 0; i < _invoices.Length; i++)
                    {
                        MInvoice _invoice = _invoices[i];
                        if (_invoice.IsPaid())
                        {
                            continue;
                        }
                        //	log.fine("allocateIndividualPayments - " + _invoice);
                        if (_payment.GetC_Invoice_ID() == _invoice.GetC_Invoice_ID())
                        {
                            if (_payment.GetC_Currency_ID() == _invoice.GetC_Currency_ID())
                            {
                                Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                                if (!_invoice.IsSOTrx())
                                {
                                    _openAmt = Decimal.Negate(_openAmt);
                                }
                                log.Fine(_invoice + ", Open=" + _openAmt);
                                //	With Discount, etc.
                                if (_availableAmt.CompareTo(_openAmt) == 0)
                                {
                                    if (_payment.AllocateIt())
                                    {
                                        AddLog(0, _payment.GetDateAcct(), _openAmt, _payment.GetDocumentNo() + " [1]");
                                        _count++;
                                    }
                                    break;
                                }
                            }
                            else	//	Mixed Currency
                            {
                            }
                        }	//	_invoice found
                    }	//	for all invoices
                }	//	_payment has _invoice
                else	//	No direct _invoice
                {
                    MPaySelectionCheck psCheck = MPaySelectionCheck.GetOfPayment(GetCtx(), _payment.GetC_Payment_ID(), Get_Trx());
                    if (psCheck == null)
                    {
                        continue;
                    }
                    //
                    Decimal _totalInvoice = Env.ZERO;
                    MPaySelectionLine[] _psLines = psCheck.GetPaySelectionLines(false);
                    for (int i = 0; i < _psLines.Length; i++)
                    {
                        MPaySelectionLine _line = _psLines[i];
                        MInvoice _invoice = _line.GetInvoice();
                        if (_payment.GetC_Currency_ID() == _invoice.GetC_Currency_ID())
                        {
                            Decimal _invoiceAmt = Utility.Util.GetValueOfDecimal((_invoice.GetOpenAmt(true, null)));
                            Decimal _overUnder = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(_line.GetOpenAmt(), _line.GetPayAmt()),
                                _line.GetDiscountAmt()), _line.GetDifferenceAmt());
                            _invoiceAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(_invoiceAmt, _line.GetDiscountAmt()),
                                _line.GetDifferenceAmt()), _overUnder);
                            if (!_invoice.IsSOTrx())
                                _invoiceAmt = Decimal.Negate(_invoiceAmt);
                            log.Fine(_invoice + ", Invoice=" + _invoiceAmt);
                            _totalInvoice = Decimal.Add(_totalInvoice, _invoiceAmt);
                        }
                        else	//	Multi-Currency
                        {
                        }
                    }
                    if (_availableAmt.CompareTo(_totalInvoice) == 0)
                    {
                        if (_payment.AllocateIt())
                        {
                            AddLog(0, _payment.GetDateAcct(), _availableAmt, _payment.GetDocumentNo() + " [n]");
                            _count++;
                        }
                    }
                }	//	No direct _invoice
            }
            //	See if there is a direct link

            return _count;
        }	//	allocateIndividualPayments


        /// <summary>
        ///Allocate Payment:Invoice 1:1
        /// </summary>
        /// <returns>allocations</returns>
        private int AllocateBPOneToOne()
        {
            int _count = 0;
            for (int p = 0; p < _payments.Length; p++)
            {
                MPayment _payment = _payments[p];
                if (_payment.IsAllocated())
                {
                    continue;
                }
                Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                log.Info(_payment + ", Allocated=" + _allocatedAmt);
                if ( Env.Signum(_allocatedAmt) != 0)
                {
                    continue;
                }
                Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt()
                    , _payment.GetDiscountAmt())
                    , _payment.GetWriteOffAmt())
                    , _payment.GetOverUnderAmt());
                if (!_payment.IsReceipt())
                {
                    _availableAmt = Decimal.Negate(_availableAmt);
                }
                log.Fine("Available=" + _availableAmt);
                for (int i = 0; i < _invoices.Length; i++)
                {
                    MInvoice _invoice = _invoices[i];
                    if (_invoice == null || _invoice.IsPaid())
                        continue;
                    if (_payment.GetC_Currency_ID() == _invoice.GetC_Currency_ID())
                    {
                        //	log.fine("allocateBPartnerAll - " + _invoice);
                        Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                        if (!_invoice.IsSOTrx())
                        {
                            _openAmt = Decimal.Negate(_openAmt);
                        }
                        Decimal _difference = Decimal.Subtract(_availableAmt, Math.Abs((_openAmt)));
                        log.Fine(_invoice + ", Open=" + _openAmt + " - Difference=" + _difference);
                        if (Env.Signum(_difference) == 0)
                        {
                            DateTime? _dateAcct = _payment.GetDateAcct();
                            if (_invoice.GetDateAcct() > _dateAcct)
                            {
                                _dateAcct = _invoice.GetDateAcct();
                            }
                            if (!CreateAllocation(_payment.GetC_Currency_ID(), "1:1 (" + _availableAmt + ")",
                                _dateAcct, _availableAmt, null, null, null,
                                _invoice.GetC_BPartner_ID(), _payment.GetC_Payment_ID(),
                                _invoice.GetC_Invoice_ID(), _invoice.GetAD_Org_ID()))
                            {
                                throw new Exception("Cannot create Allocation");
                            }
                            ProcessAllocation();
                            _count++;
                            _invoices[i] = null;		//	remove _invoice
                            _payments[p] = null;
                            _payment = null;
                            break;
                        }
                    }
                    else	//	Multi-Currency
                    {
                    }
                }	//	for all invoices
            }	//	for all payments
            return _count;
        }	//	allocateOneToOne

        /// <summary>
        /// 	Allocate all Payments/Invoices using Accounting currency
        /// </summary>
        /// <returns>allocations</returns>
        private int AllocateBPartnerAll()
        {
            int _C_Currency_ID = MClient.Get(GetCtx()).GetC_Currency_ID();
            DateTime? _dateAcct = null;
            //	Payments
            Decimal _totalPayments = Env.ZERO;
            for (int p = 0; p < _payments.Length; p++)
            {
                MPayment _payment = _payments[p];
                if (_payment.IsAllocated())
                {
                    continue;
                }
                Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                //	log.info("allocateBPartnerAll - " + _payment + ", Allocated=" + _allocatedAmt);
                if (Env.Signum(_allocatedAmt) != 0)
                    continue;
                Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt(),
                    _payment.GetDiscountAmt()),
                    _payment.GetWriteOffAmt()),
                    _payment.GetOverUnderAmt());
                if (!_payment.IsReceipt())
                {
                    _availableAmt = Decimal.Negate(_availableAmt);
                }
                //	Foreign currency
                if (_payment.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                //	log.fine("allocateBPartnerAll - Available=" + _availableAmt);
                if (_dateAcct == null || _payment.GetDateAcct() > (_dateAcct))
                {
                    _dateAcct = _payment.GetDateAcct();
                }
                _totalPayments = Decimal.Add(_totalPayments, _availableAmt);
            }
            //	Invoices
            Decimal _totalInvoices = Env.ZERO;
            for (int i = 0; i < _invoices.Length; i++)
            {
                MInvoice _invoice = _invoices[i];
                if (_invoice.IsPaid())
                {
                    continue;
                }
                //	log.info("allocateBPartnerAll - " + _invoice);
                Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                if (!_invoice.IsSOTrx())
                {
                    _openAmt = Decimal.Negate(_openAmt);
                }
                //	Foreign currency
                if (_invoice.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                //	log.fine("allocateBPartnerAll - Open=" + _openAmt);
                if (_dateAcct == null || _invoice.GetDateAcct() > (_dateAcct))
                {
                    _dateAcct = _invoice.GetDateAcct();
                }
                _totalInvoices = Decimal.Add(_totalInvoices, _openAmt);
            }

            Decimal _difference = Decimal.Subtract(_totalInvoices, _totalPayments);
            log.Info("= Invoices=" + _totalInvoices
                + " - Payments=" + _totalPayments
                + " = Difference=" + _difference);

            if (Env.Signum(_difference) == 0)
            {
                for (int p = 0; p < _payments.Length; p++)
                {
                    MPayment _payment = _payments[p];
                    if (_payment.IsAllocated())
                    {
                        continue;
                    }
                    Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                    if (Env.Signum(_allocatedAmt) != 0)
                    {
                        continue;
                    }
                    Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt(),
                        _payment.GetDiscountAmt()),
                        _payment.GetWriteOffAmt()),
                        _payment.GetOverUnderAmt());
                    if (!_payment.IsReceipt())
                    {
                        _availableAmt = Decimal.Negate(_availableAmt);
                    }
                    //	Foreign currency
                    if (_payment.GetC_Currency_ID() != _C_Currency_ID)
                    {
                        continue;
                    }
                    if (!CreateAllocation(_C_Currency_ID, "BP All",
                        _dateAcct, _availableAmt, null, null, null,
                        _payment.GetC_BPartner_ID(), _payment.GetC_Payment_ID(), 0, _payment.GetAD_Org_ID()))
                    {
                        throw new Exception("Cannot create Allocation");
                    }
                }	//	for all payments
                //
                for (int i = 0; i < _invoices.Length; i++)
                {
                    MInvoice _invoice = _invoices[i];
                    if (_invoice.IsPaid())
                    {
                        continue;
                    }
                    Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                    if (!_invoice.IsSOTrx())
                    {
                        _openAmt = Decimal.Negate(_openAmt);
                    }
                    //	Foreign currency
                    if (_invoice.GetC_Currency_ID() != _C_Currency_ID)
                    {
                        continue;
                    }
                    if (!CreateAllocation(_C_Currency_ID, "BP All",
                        _dateAcct, _openAmt, null, null, null,
                        _invoice.GetC_BPartner_ID(), 0, _invoice.GetC_Invoice_ID(), _invoice.GetAD_Org_ID()))
                    {
                        throw new Exception("Cannot create Allocation");
                    }
                }	//	for all invoices
                ProcessAllocation();
                return 1;
            }	//	Difference OK

            return 0;
        }	//	allocateBPartnerAll


        /// <summary>
        ///	Allocate Oldest First using Accounting currency                
        /// </summary>
        /// <returns>allocations</returns>
        private int AllocateBPOldestFirst()
        {
            int _C_Currency_ID = MClient.Get(GetCtx()).GetC_Currency_ID();
            DateTime? _dateAcct = null;
            //	Payments
            Decimal _totalPayments = Env.ZERO;
            for (int p = 0; p < _payments.Length; p++)
            {
                MPayment _payment = _payments[p];
                if (_payment.IsAllocated())
                {
                    continue;
                }
                if (_payment.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                log.Info(_payment + ", Allocated=" + _allocatedAmt);
                Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt(),
                    _payment.GetDiscountAmt()),
                    _payment.GetWriteOffAmt()),
                    _payment.GetOverUnderAmt());
                if (!_payment.IsReceipt())
                {
                    _availableAmt = Decimal.Negate(_availableAmt);
                }
                log.Fine("Available=" + _availableAmt);
                if (_dateAcct == null || _payment.GetDateAcct() > (_dateAcct))
                {
                    _dateAcct = Utility.Util.GetValueOfDateTime(_payment.GetDateAcct());
                }
                _totalPayments = Decimal.Add(_totalPayments, (_availableAmt));
            }
            //	Invoices
            Decimal _totalInvoices = Env.ZERO;
            for (int i = 0; i < _invoices.Length; i++)
            {
                MInvoice _invoice = _invoices[i];
                if (_invoice.IsPaid())
                {
                    continue;
                }
                if (_invoice.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                log.Fine("" + _invoice);
                if (!_invoice.IsSOTrx())
                {
                    _openAmt = Decimal.Negate(_openAmt);
                }
                //	Foreign currency
                log.Fine("Open=" + _openAmt);
                if (_dateAcct == null || _invoice.GetDateAcct() > (_dateAcct))
                {
                    _dateAcct = _invoice.GetDateAcct();
                }
                _totalInvoices = Decimal.Add(_totalInvoices, _openAmt);
            }

            //	must be either AP or AR balance
            if (Env.Signum(_totalInvoices) != Env.Signum(_totalPayments))
            {
                log.Fine("Signum - Invoices=" + Env.Signum(_totalInvoices)
                    + " <> Payments=" + Env.Signum(_totalPayments));
                return 0;
            }

            Decimal _difference = Decimal.Subtract(_totalInvoices, _totalPayments);
            //Decimal _maxAmt = _totalInvoices.abs().min(_totalPayments.abs());
            Decimal _maxAmt = Math.Min(Math.Abs(_totalInvoices), Math.Abs(_totalPayments));
            if (Env.Signum(_totalInvoices) < 0)
            {
                _maxAmt = Decimal.Negate(_maxAmt);
            }
            log.Info("= Invoices=" + _totalInvoices
                + " - Payments=" + _totalPayments
                + " = Difference=" + _difference + " - Max=" + _maxAmt);


            //	Allocate Payments up to max
            Decimal _allocatedPayments = Env.ZERO;
            for (int p = 0; p < _payments.Length; p++)
            {
                MPayment _payment = _payments[p];
                if (_payment.IsAllocated())
                {
                    continue;
                }
                if (_payment.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                Decimal _allocatedAmt = Utility.Util.GetValueOfDecimal(_payment.GetAllocatedAmt());
                if ( Env.Signum(_allocatedAmt) != 0)
                {
                    continue;
                }
                Decimal _availableAmt = Decimal.Add(Decimal.Add(Decimal.Add(_payment.GetPayAmt(),
                    _payment.GetDiscountAmt()),
                    _payment.GetWriteOffAmt()),
                    _payment.GetOverUnderAmt());
                if (!_payment.IsReceipt())
                {
                    _availableAmt = Decimal.Negate(_availableAmt);
                }
                _allocatedPayments = Decimal.Add(_allocatedPayments, _availableAmt);
                if ((Env.Signum(_totalInvoices) > 0 && _allocatedPayments.CompareTo(_maxAmt) > 0)
                    || (Env.Signum(_totalInvoices) < 0 && _allocatedPayments.CompareTo(_maxAmt) < 0))
                {
                    Decimal _diff = Decimal.Subtract(_allocatedPayments, _maxAmt);
                    _availableAmt = Decimal.Subtract(_availableAmt, _diff);
                    _allocatedPayments = Decimal.Subtract(_allocatedPayments, _diff);
                }
                log.Fine("Payment Allocated=" + _availableAmt);
                if (!CreateAllocation(_C_Currency_ID, "BP Oldest (" + Math.Abs(_difference) + ")",
                    _dateAcct, _availableAmt, null, null, null,
                    _payment.GetC_BPartner_ID(), _payment.GetC_Payment_ID(), 0, _payment.GetAD_Org_ID()))
                {
                    throw new Exception("Cannot create Allocation");
                }
                if (_allocatedPayments.CompareTo(_maxAmt) == 0)
                {
                    break;
                }
            }	//	for all payments
            //	Allocated Invoices up to max
            Decimal _allocatedInvoices = Env.ZERO;
            for (int i = 0; i < _invoices.Length; i++)
            {
                MInvoice _invoice = _invoices[i];
                if (_invoice.IsPaid())
                {
                    continue;
                }
                if (_invoice.GetC_Currency_ID() != _C_Currency_ID)
                {
                    continue;
                }
                Decimal _openAmt = Utility.Util.GetValueOfDecimal(_invoice.GetOpenAmt(true, null));
                if (!_invoice.IsSOTrx())
                {
                    _openAmt = Decimal.Negate(_openAmt);
                }
                _allocatedInvoices = Decimal.Add(_allocatedInvoices, _openAmt);
                if ((Env.Signum(_totalInvoices) > 0 && _allocatedInvoices.CompareTo(_maxAmt) > 0)
                    || (Env.Signum(_totalInvoices) < 0 && _allocatedInvoices.CompareTo(_maxAmt) < 0))
                {
                    Decimal _diff = Decimal.Subtract(_allocatedInvoices, _maxAmt);
                    _openAmt = Decimal.Subtract(_openAmt, _diff);
                    _allocatedInvoices = Decimal.Subtract(_allocatedInvoices, _diff);
                }
                if (Env.Signum(_openAmt) == 0)
                {
                    break;
                }
                log.Fine("Invoice Allocated=" + _openAmt);
                if (!CreateAllocation(_C_Currency_ID, "BP Oldest (" + Math.Abs(_difference) + ")",
                    _dateAcct, _openAmt, null, null, null,
                    _invoice.GetC_BPartner_ID(), 0, _invoice.GetC_Invoice_ID(), _invoice.GetAD_Org_ID()))
                {
                    throw new Exception("Cannot create Allocation");
                }
                if (_allocatedInvoices.CompareTo(_maxAmt) == 0)
                {
                    break;
                }
            }	//	for all invoices

            if (_allocatedPayments.CompareTo(_allocatedInvoices) != 0)
            {
                throw new Exception("Allocated Payments=" + _allocatedPayments
                    + " <> Invoices=" + _allocatedInvoices);
            }
            ProcessAllocation();
            return 1;
        }	//	allocateOldestFirst


        /// <summary>
        /// Create Allocation allocation
        /// </summary>
        /// <param name="_C_Currency_ID"></param>
        /// <param name="description"></param>
        /// <param name="_dateAcct"></param>
        /// <param name="Amount"></param>
        /// <param name="DiscountAmt"></param>
        /// <param name="WriteOffAmt"></param>
        /// <param name="OverUnderAmt"></param>
        /// <param name="_C_BPartner_ID"></param>
        /// <param name="C_Payment_ID"></param>
        /// <param name="C_Invoice_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <returns>true if created</returns>
        private Boolean CreateAllocation(int _C_Currency_ID, String description,
            DateTime? _dateAcct, Decimal Amount,
            Decimal? DiscountAmt, Decimal? WriteOffAmt, Decimal? OverUnderAmt,
            int _C_BPartner_ID, int C_Payment_ID, int C_Invoice_ID, int AD_Org_ID)
        {
            //	Process old Allocation 
            if (_allocation != null
                && _allocation.GetC_Currency_ID() != _C_Currency_ID)
            {
                ProcessAllocation();
            }
            //	New Allocation
            if (_allocation == null)
            {
                _allocation = new MAllocationHdr(GetCtx(), false, _dateAcct,	//	automatic 
                    _C_Currency_ID, "Auto " + description, Get_Trx());
                _allocation.SetAD_Org_ID(AD_Org_ID);
                if (!_allocation.Save())
                {
                    return false;
                }
            }

            //	New Allocation Line
            MAllocationLine aLine = new MAllocationLine(_allocation, Amount,
                DiscountAmt, WriteOffAmt, OverUnderAmt);
            aLine.SetC_BPartner_ID(_C_BPartner_ID);
            aLine.SetC_Payment_ID(C_Payment_ID);
            aLine.SetC_Invoice_ID(C_Invoice_ID);
            return aLine.Save();
        }	//	createAllocation

        /// <summary>
        /// Process Allocation      
        /// </summary>
        /// <returns>true if processes/saved or none</returns>
        private Boolean ProcessAllocation()
        {
            if (_allocation == null)
                return true;
            Boolean _success = _allocation.ProcessIt(MAllocationHdr.DOCACTION_Complete);
            if (_success)
            {
                _success = _allocation.Save();
            }
            else
            {
                _allocation.Save();
            }
            AddLog(0, _allocation.GetDateAcct(), null, _allocation.GetDescription());
            _allocation = null;
            return _success;
        }	//	processAllocation

    }
}
