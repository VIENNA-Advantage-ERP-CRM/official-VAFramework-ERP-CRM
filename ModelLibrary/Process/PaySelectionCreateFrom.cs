/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PaySelectionCreateFrom
 * Purpose        : Create Payment Selection Lines from AP Invoices
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      16-July-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class PaySelectionCreateFrom : ProcessEngine.SvrProcess
    {
        //Only When Discount		
        private bool _OnlyDiscount = false;
        //Only when Due				
        private bool _OnlyDue = false;
        //Include Disputed		
        private bool _IncludeInDispute = false;
        //Match Requirement		
        private String _MatchRequirementI = X_VAB_Invoice.MATCHREQUIREMENTI_None;
        //Payment Rule			
        private String _PaymentRule = null;
        //BPartner				
        private int _VAB_BusinessPartner_ID = 0;
        //BPartner Group			
        private int _VAB_BPart_Category_ID = 0;
        //Payment Selection		
        private int _VAB_PaymentOption_ID = 0;
        //Document No From		   
        private String _DocumentNo_From = null;
        //Document No To			
        private String _DocumentNo_To = null;

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
                else if (name.Equals("OnlyDiscount"))
                {
                    _OnlyDiscount = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("OnlyDue"))
                {
                    _OnlyDue = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("IncludeInDispute"))
                {
                    _IncludeInDispute = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("MatchRequirementI"))
                {
                    _MatchRequirementI = (String)para[i].GetParameter();
                }
                else if (name.Equals("PaymentRule"))
                {
                    _PaymentRule = (String)para[i].GetParameter();
                }
                else if (name.Equals("DocumentNo"))
                {
                    _DocumentNo_From = (String)para[i].GetParameter();
                    _DocumentNo_To = (String)para[i].GetParameter_To();
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
            _VAB_PaymentOption_ID = GetRecord_ID();
            if (_DocumentNo_From != null && _DocumentNo_From.Length == 0)
            {
                _DocumentNo_From = null;
            }
            if (_DocumentNo_To != null && _DocumentNo_To.Length == 0)
            {
                _DocumentNo_To = null;
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            int count = 0;
            

            log.Info("VAB_PaymentOption_ID=" + _VAB_PaymentOption_ID
                + ", OnlyDiscount=" + _OnlyDiscount + ", OnlyDue=" + _OnlyDue
                + ", IncludeInDispute=" + _IncludeInDispute
                + ", MatchRequirement=" + _MatchRequirementI
                + ", PaymentRule=" + _PaymentRule
                + ", VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ", VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);

            MPaySelection psel = new MPaySelection(GetCtx(), _VAB_PaymentOption_ID, Get_TrxName());
            int VAB_CurrencyTo_ID = psel.GetVAB_Currency_ID();
            if (psel.Get_ID() == 0)
            {
                throw new ArgumentException("Not found VAB_PaymentOption_ID=" + _VAB_PaymentOption_ID);
            }
            if (psel.IsProcessed())
            {
                throw new ArgumentException("@Processed@");
            }
            //	psel.getPayDate();

            String sql = "SELECT VAB_Invoice_ID,"
                //	Open
                + " currencyConvert(invoiceOpen(i.VAB_Invoice_ID, 0)"
                    + ",i.VAB_Currency_ID, " + VAB_CurrencyTo_ID + "," + DataBase.DB.TO_DATE(psel.GetPayDate(), true) + ", i.VAB_CurrencyType_ID,i.VAF_Client_ID,i.VAF_Org_ID),"	//	##1/2 Currency_To,PayDate
                //	Discount
                + " currencyConvert(paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTerm_ID,i.DateInvoiced, " + DataBase.DB.TO_DATE(psel.GetPayDate(), true) + ")"	//	##3 PayDate
                    + ",i.VAB_Currency_ID," + VAB_CurrencyTo_ID + "," + DataBase.DB.TO_DATE(psel.GetPayDate(), true) + ",i.VAB_CurrencyType_ID,i.VAF_Client_ID,i.VAF_Org_ID),"	//	##4/5 Currency_To,PayDate
                + " PaymentRule, IsSOTrx "		//	4..6
                + "FROM VAB_Invoice i "
                + "WHERE";
            if (_PaymentRule != null && _PaymentRule.Equals(X_VAB_Invoice.PAYMENTRULE_DirectDebit))
                {
                    sql += " IsSOTrx='Y'";
                }
                else
                {
                    sql += " IsSOTrx='N'";
                }
            sql+=" AND IsPaid='N' AND DocStatus IN ('CO','CL')" // ##6
                + " AND VAF_Client_ID=" + psel.GetVAF_Client_ID()				//	##7
                //	Existing Payments - Will reselect Invoice if prepared but not paid 
                + " AND NOT EXISTS (SELECT * FROM VAB_PaymentOptionLine psl "
                    + "WHERE i.VAB_Invoice_ID=psl.VAB_Invoice_ID AND psl.IsActive='Y'"
                    + " AND psl.VAB_PaymentOptionCheck_ID IS NOT NULL)";
            count = 7;
            //	Disputed
            if (!_IncludeInDispute)
            {
                sql += " AND i.IsInDispute='N'";
            }
            //	PaymentRule (optional)
            if (_PaymentRule != null && _PaymentRule != " ")
            {
                sql += " AND PaymentRule=" + _PaymentRule;		//	##
                count += 1;
            }
            //	OnlyDiscount
            if (_OnlyDiscount)
            {
                if (_OnlyDue)
                {
                    sql += " AND (";
                }
                else
                {
                    sql += " AND ";
                }
                sql += "paymentTermDiscount(invoiceOpen(VAB_Invoice_ID, 0), VAB_Currency_ID, VAB_PaymentTerm_ID, DateInvoiced, " + DataBase.DB.TO_DATE(psel.GetPayDate(), true) + ") > 0";	//	##
                count += 1;
            }
            //	OnlyDue
            if (_OnlyDue)
            {
                if (_OnlyDiscount)
                {
                    sql += " OR ";
                }
                else
                {
                    sql += " AND ";
                }
                sql += "paymentTermDueDays(VAB_PaymentTerm_ID, DateInvoiced, " + DataBase.DB.TO_DATE(psel.GetPayDate(), true) + ") >= 0";	//	##
                count += 1;
                if (_OnlyDiscount)
                {
                    sql += ")";
                }
            }
            //	Business Partner
            if (_VAB_BusinessPartner_ID != 0 && _VAB_BusinessPartner_ID != -1)
            {
                sql += " AND VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID;	//	##
                count += 1;
            }
            //	Business Partner Group
            else if (_VAB_BPart_Category_ID != 0 && _VAB_BPart_Category_ID != -1)
            {
                sql += " AND EXISTS (SELECT * FROM VAB_BusinessPartner bp "
                    + "WHERE bp.VAB_BusinessPartner_ID=i.VAB_BusinessPartner_ID AND bp.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ")";	//	##
                count += 1;
            }
            //	PO Matching Requiremnent
            if (_MatchRequirementI.Equals(X_VAB_Invoice.MATCHREQUIREMENTI_PurchaseOrder)
                || _MatchRequirementI.Equals(X_VAB_Invoice.MATCHREQUIREMENTI_PurchaseOrderAndReceipt))
            {
                sql += " AND i._MatchRequirementI NOT IN ('N','R')"
                    + " AND EXISTS (SELECT * FROM VAB_InvoiceLine il "
                    + "WHERE i.VAB_Invoice_ID=il.VAB_Invoice_ID"
                    + " AND QtyInvoiced IN (SELECT SUM(Qty) FROM VAM_MatchPO m "
                        + "WHERE il.VAB_InvoiceLine_ID=m.VAB_InvoiceLine_ID))";
            }
            //	Receipt Matching Requiremnent
            if (_MatchRequirementI.Equals(X_VAB_Invoice.MATCHREQUIREMENTI_Receipt)
                || _MatchRequirementI.Equals(X_VAB_Invoice.MATCHREQUIREMENTI_PurchaseOrderAndReceipt))
            {
                sql += " AND i._MatchRequirementI NOT IN ('N','P')"
                    + " AND EXISTS (SELECT * FROM VAB_InvoiceLine il "
                    + "WHERE i.VAB_Invoice_ID=il.VAB_Invoice_ID"
                    + " AND QtyInvoiced IN (SELECT SUM(Qty) FROM VAM_MatchInvoice m "
                        + "WHERE il.VAB_InvoiceLine_ID=m.VAB_InvoiceLine_ID))";
            }

            //	Document No
            else if (_DocumentNo_From != null && _DocumentNo_To != null)
            {
                sql += " AND i.DocumentNo BETWEEN "
                    + DataBase.DB.TO_STRING(_DocumentNo_From) + " AND "
                    + DataBase.DB.TO_STRING(_DocumentNo_To);
            }
            else if (_DocumentNo_From != null)
            {
                sql += " AND ";
                if (_DocumentNo_From.IndexOf('%') == -1)
                {
                    sql += "i.DocumentNo >= "
                        + DataBase.DB.TO_STRING(_DocumentNo_From);
                }
                else
                {
                    sql += "i.DocumentNo LIKE "
                        + DataBase.DB.TO_STRING(_DocumentNo_From);
                }
            }

            //
            int lines = 0;
            IDataReader idr = null;

            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    int VAB_Invoice_ID = Utility.Util.GetValueOfInt(idr[0]);//  rs.getInt(1);
                    Decimal PayAmt = Utility.Util.GetValueOfDecimal(idr[1]); //rs.getBigDecimal(2);
                    if (VAB_Invoice_ID == 0 || Env.ZERO.CompareTo(PayAmt) == 0)
                    {
                        continue;
                    }
                    Decimal DiscountAmt = Utility.Util.GetValueOfDecimal(idr[2]);//rs.getBigDecimal(3);
                    String PaymentRule = Utility.Util.GetValueOfString(idr[3]); //rs.getString(4);
                    bool isSOTrx = "Y".Equals(Utility.Util.GetValueOfString(idr[4]));//rs.getString(5));
                    //
                    lines++;
                    MPaySelectionLine pselLine = new MPaySelectionLine(psel, lines * 10, PaymentRule);
                    pselLine.SetInvoice(VAB_Invoice_ID, isSOTrx,
                        PayAmt, Decimal.Subtract(PayAmt, DiscountAmt), DiscountAmt);
                    if (!pselLine.Save())
                    {
                        if (idr != null)
                        {
                            idr.Close();
                            idr = null;
                        }
                        return GetRetrievedError(pselLine, "Cannot save MPaySelectionLine");
                        //throw new Exception("Cannot save MPaySelectionLine");
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
                log.Log(Level.SEVERE, sql, e);
            }

            return "@VAB_PaymentOptionLine_ID@  - #" + lines;
        }
    }
}
