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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
   public class InvoiceWriteOff:ProcessEngine.SvrProcess
    {
	//	BPartner				
	private int			_VAB_BusinessPartner_ID = 0;
	// BPartner Group			
	private int			_VAB_BPart_Category_ID = 0;
	//	Invoice					
	private int			_VAB_Invoice_ID = 0;
	
	// Max Amt					
	private Decimal	_MaxInvWriteOffAmt = Env.ZERO;
	// AP or AR				
	private String		_APAR = "R";
	private static String	_ONLY_AP = "P";
	private static String	_ONLY_AR = "R";
	
	// Invoice Date From		
	private DateTime?	_DateInvoiced_From = null;
	// Invoice Date To			
	private DateTime?	_DateInvoiced_To = null;
	// Accounting Date			
	private DateTime?	_DateAcct = null;
	// Create Payment			
	private Boolean		_CreatePayment = false;
	// Bank Account			
	private int			_VAB_Bank_Acct_ID = 0;
	// Simulation				
	private Boolean		_IsSimulation = true;

	//	Allocation Hdr			
	private MAllocationHdr	_m_alloc = null;
	//	Payment					
	private MPayment		_m_payment = null;
	
	/// <summary>
    ///   Prepare - e.g., get Parameters.
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
            else if (name.Equals("VAB_BusinessPartner_ID"))
            {
                _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("VAB_BPart_Category_ID"))
            {
                _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("VAB_Invoice_ID"))
            {
                _VAB_Invoice_ID = para[i].GetParameterAsInt();
            }
            //
            else if (name.Equals("MaxInvWriteOffAmt"))
            {
                _MaxInvWriteOffAmt = Utility.Util.GetValueOfDecimal(para[i].GetParameter());
            }
            else if (name.Equals("APAR"))
            {
                _APAR = Utility.Util.GetValueOfString(para[i].GetParameter());
            }
            //
            else if (name.Equals("DateInvoiced"))
            {
                _DateInvoiced_From = Utility.Util.GetValueOfDateTime(para[i].GetParameter());
                _DateInvoiced_To = Utility.Util.GetValueOfDateTime(para[i].GetParameter_To());
            }
            else if (name.Equals("DateAcct"))
            {
                _DateAcct = Utility.Util.GetValueOfDateTime(para[i].GetParameter());
            }
            //
            else if (name.Equals("CreatePayment"))
            {
                _CreatePayment = "Y".Equals(para[i].GetParameter());
            }
            else if (name.Equals("VAB_Bank_Acct_ID"))
            {
                _VAB_Bank_Acct_ID = para[i].GetParameterAsInt();
            }
            //
            else if (name.Equals("IsSimulation"))
            {
                _IsSimulation = "Y".Equals(para[i].GetParameter());
            }
            else
            {
                log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Execute
	/// </summary>
	/// <returns>message</returns>
	protected override String DoIt () 
	{
		log.Info("VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID 
			+ ", VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID
			+ ", VAB_Invoice_ID=" + _VAB_Invoice_ID
			+ "; APAR=" + _APAR
			+ ", " + _DateInvoiced_From + " - " + _DateInvoiced_To
			+ "; CreatePayment=" + _CreatePayment
			+ ", VAB_Bank_Acct_ID=" + _VAB_Bank_Acct_ID);
		//
        if (_VAB_BusinessPartner_ID == 0 && _VAB_Invoice_ID == 0 && _VAB_BPart_Category_ID == 0)
        {
            throw new Exception("@FillMandatory@ @VAB_Invoice_ID@ / @VAB_BusinessPartner_ID@ / ");
        }
		//
        if (_CreatePayment && _VAB_Bank_Acct_ID == 0)
        {
            throw new Exception("@FillMandatory@  @VAB_Bank_Acct_ID@");
        }
		//
		StringBuilder sql = new StringBuilder(
			"SELECT VAB_Invoice_ID,DocumentNo,DateInvoiced,"
			+ "VAB_Currency_ID,GrandTotal,invoiceOpen(VAB_Invoice_ID, 0) AS OpenAmt"
			+ " FROM VAB_Invoice WHERE ");
        if (_VAB_Invoice_ID != 0)
        {
            sql.Append("VAB_Invoice_ID= ").Append(_VAB_Invoice_ID);
        }
        else
        {
            if (_VAB_BusinessPartner_ID != 0)
            {
                sql.Append("VAB_BusinessPartner_ID= ").Append(_VAB_BusinessPartner_ID);
            }
            else
            {
                sql.Append("EXISTS (SELECT * FROM VAB_BusinessPartner bp WHERE VAB_Invoice.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID AND bp.VAB_BPart_Category_ID=")
                               .Append(_VAB_BPart_Category_ID).Append(")");
            }
            //
            if (_ONLY_AR.Equals(_APAR))
            {
                sql.Append(" AND IsSOTrx='Y'");
            }
            else if (_ONLY_AP.Equals(_APAR))
            {
                sql.Append(" AND IsSOTrx='N'");
            }
            //
            if (_DateInvoiced_From != null && _DateInvoiced_To != null)
            {
                sql.Append(" AND TRIM(DateInvoiced) BETWEEN ")
                    .Append(DataBase.DB.TO_DATE(_DateInvoiced_From, true))
                    .Append(" AND ")
                    .Append(DataBase.DB.TO_DATE(_DateInvoiced_To, true));
            }
            else if (_DateInvoiced_From != null)
            {
                sql.Append(" AND TRIM(DateInvoiced) >= ")
                    .Append(DataBase.DB.TO_DATE(_DateInvoiced_From, true));
            }
            else if (_DateInvoiced_To != null)
            {
                sql.Append(" AND TRIM(DateInvoiced) <= ")
                    .Append(DataBase.DB.TO_DATE(_DateInvoiced_To, true));
            }
        }
		sql.Append(" AND IsPaid='N' ORDER BY VAB_Currency_ID, VAB_BusinessPartner_ID,DateInvoiced");
		log.Finer(sql.ToString());
       // DataTable dt = null;
        IDataReader idr = null;
        int _counter = 0;
        //
        try
        {
		
             idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
             while(idr.Read())
             {
               if (writeOff(Utility.Util.GetValueOfInt(idr[0]), Utility.Util.GetValueOfString(idr[1]),Utility.Util.GetValueOfDateTime(idr[2])
                , Utility.Util.GetValueOfInt(idr[3]), Utility.Util.GetValueOfDecimal(idr[5])))
                {
                     _counter++;
                }
            
            }
             idr.Close();
        }
        
        catch (Exception e)
        {
            if (idr != null)
            {
                idr.Close();
            }
            log.Log(Level.SEVERE, sql.ToString(), e);
        }
       
       	ProcessPayment();
		ProcessAllocation();
		return "#" + _counter;
	}	//	doIt

	/// <summary>
	/// Write Off	 
	/// </summary>
	/// <param name="VAB_Invoice_ID"></param>
	/// <param name="DocumentNo"></param>
	/// <param name="DateInvoiced"></param>
	/// <param name="VAB_Currency_ID"></param>
	/// <param name="OpenAmt"></param>
    /// <returns>true if written off</returns>
	private Boolean writeOff (int VAB_Invoice_ID, String DocumentNo, DateTime? DateInvoiced, 
		int VAB_Currency_ID, Decimal OpenAmt)
	{
		//	Nothing to do
        if ( Env.Signum(OpenAmt) == 0)
        {
            return false;
        }
        if (Math.Abs(OpenAmt).CompareTo(_MaxInvWriteOffAmt) >= 0)
        {
            return false;
        }
		//
		if (_IsSimulation)
		{
			AddLog(VAB_Invoice_ID, DateInvoiced, OpenAmt, DocumentNo);
			return true;
		}
		
		//	Invoice
		MInvoice invoice = new MInvoice(GetCtx(), VAB_Invoice_ID, Get_TrxName());
        if (!invoice.IsSOTrx())
        {
            OpenAmt =Decimal.Negate( OpenAmt);
        }
		//	Allocation
		if (_m_alloc == null || VAB_Currency_ID != _m_alloc.GetVAB_Currency_ID())
		{
			ProcessAllocation();
			_m_alloc = new MAllocationHdr (GetCtx(), true, 
				_DateAcct, VAB_Currency_ID,
				GetProcessInfo().GetTitle() + " #" + GetVAF_JInstance_ID(), Get_TrxName());
			_m_alloc.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
			if (!_m_alloc.Save())
			{
				log.Log(Level.SEVERE, "Cannot create allocation header");
				return false;
			}
		}
		//	Payment
		if (_CreatePayment 
			&& (_m_payment == null 
				|| invoice.GetVAB_BusinessPartner_ID() != _m_payment.GetVAB_BusinessPartner_ID()
				|| VAB_Currency_ID != _m_payment.GetVAB_Currency_ID()))
		{
			ProcessPayment();
			_m_payment = new MPayment(GetCtx(), 0, Get_TrxName());
			_m_payment.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
			_m_payment.SetVAB_Bank_Acct_ID(_VAB_Bank_Acct_ID);
			_m_payment.SetTenderType(MPayment.TENDERTYPE_Check);
			_m_payment.SetDateTrx(_DateAcct);
			_m_payment.SetDateAcct(_DateAcct);
			_m_payment.SetDescription(GetProcessInfo().GetTitle() + " #" + GetVAF_JInstance_ID());
			_m_payment.SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());
			_m_payment.SetIsReceipt(true);	//	payments are negative
			_m_payment.SetVAB_Currency_ID(VAB_Currency_ID);
			if (!_m_payment.Save())
			{
				log.Log(Level.SEVERE, "Cannot create payment");
				return false;
			}
		}

		//	Line
		MAllocationLine aLine = null;
		if (_CreatePayment)
		{
			aLine = new MAllocationLine (_m_alloc, OpenAmt,
				Env.ZERO, Env.ZERO, Env.ZERO);
			_m_payment.SetPayAmt(Decimal.Add( _m_payment.GetPayAmt(),OpenAmt));
			aLine.SetVAB_Payment_ID(_m_payment.GetVAB_Payment_ID());
		}
		else
			aLine = new MAllocationLine (_m_alloc, Env.ZERO, 
				Env.ZERO, OpenAmt, Env.ZERO);
		aLine.SetVAB_Invoice_ID(VAB_Invoice_ID);
		if (aLine.Save())
		{
			AddLog(VAB_Invoice_ID, DateInvoiced, OpenAmt, DocumentNo);
			return true;
		}
		//	Error
		log.Log(Level.SEVERE, "Cannot create allocation line for VAB_Invoice_ID=" + VAB_Invoice_ID);
		return false;
	}	//	writeOff

	   /// <summary>
	   /// Process Allocation
	   /// </summary>
       /// <returns>true if processed</returns>
	private Boolean ProcessAllocation()
	{
		if (_m_alloc == null)
			return true;
		ProcessPayment();
		//	Process It
		if (_m_alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE) &&  _m_alloc.Save())
		{
			_m_alloc = null;
			return true;
		}
		//
		_m_alloc = null;
		return false;
	}	//	processAllocation

	/// <summary>
	///	Process Payment
	/// </summary>
    /// <returns> true if processed</returns>
	private Boolean ProcessPayment()
	{
		if (_m_payment == null)
			return true;
		//	Process It
		if (_m_payment.ProcessIt(DocActionVariables.ACTION_COMPLETE) &&  _m_payment.Save())
		{
			_m_payment = null;
			return true;
		}
		//
		_m_payment = null;
		return false;
	}	//	processPayment
	
}	

}
