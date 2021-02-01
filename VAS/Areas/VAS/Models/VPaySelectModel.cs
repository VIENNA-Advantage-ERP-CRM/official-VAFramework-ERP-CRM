/********************************************************
 * Module Name    : VIS
 * Purpose        : Model class for payment Selection(Manual) Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     14 May 2015
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
//using VIS.DBase;

namespace VIS.Models
{
    public class VPaySelectModel
    {
        private int _VAB_PaymentOption_ID = 0;
        private int VAB_Bank_Acct_ID = 0;
        /// <summary>
        /// Get Detail for Payment Selection Form
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public PaymentSelectionDetail GetDetail(Ctx ctx)
        {
            PaymentSelectionDetail objPSelectDetail = new PaymentSelectionDetail();
            objPSelectDetail.BankAccount = GetBankAccount(ctx);
            objPSelectDetail.BusinessPartner = GetBusinesspartner(ctx);
            objPSelectDetail.PaymentMethod = GetPaymentMethod(ctx, VAB_Bank_Acct_ID);
            return objPSelectDetail;
        }
        /// <summary>
        /// Get Bank Account
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private List<BankAccount> GetBankAccount(Ctx ctx)
        {
            List<BankAccount> lstBankAccount = new List<BankAccount>();
            DataSet ds = new DataSet();
            string sqlBankAccount = "SELECT ba.VAB_Bank_Acct_ID,"                       //  1
                 + "b.Name || ' ' || ba.AccountNo AS Name, "          //  2
                   + "ba.VAB_Currency_ID, c.ISO_Code,"                   //  3..4
                   + "ba.CurrentBalance "                              //  5
                 + "FROM VAB_Bank b "
                 + "INNER JOIN VAB_Bank_Acct ba ON (b.VAB_Bank_ID=ba.VAB_Bank_ID) "
                 + "INNER JOIN VAB_Currency c ON (ba.VAB_Currency_ID=c.VAB_Currency_ID) "
                 + " AND ba.IsActive = 'Y'"
                 + " AND EXISTS (SELECT * FROM VAB_Bank_AcctDoc d WHERE d.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID) "
                 + " AND (ba.VAF_Org_ID IN (SELECT ro.VAF_Org_ID FROM VAF_Role_OrgRights ro"
                 + " WHERE ro.VAF_Role_ID = " + ctx.GetVAF_Role_ID() + " AND ro.IsActive = 'Y')"
                 + " OR (ba.VAF_Org_ID = 0 AND EXISTS (SELECT ro.VAF_Org_ID FROM VAF_Role_OrgRights ro"
                 + " WHERE ro.VAF_Role_ID = " + ctx.GetVAF_Role_ID() + " AND ro.IsActive = 'Y')) "
                 + " OR EXISTS (SELECT NULL FROM VAF_Role WHERE VAF_Role_ID=" + ctx.GetVAF_Role_ID() + " AND IsAccessAllOrgs='Y'))"
                 //+ " AND ba.vaf_org_id = " + ctx.GetVAF_Org_ID()
                 + " ORDER BY 2";
            ds = DB.ExecuteDataset(sqlBankAccount, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    VAB_Bank_Acct_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["VAB_Bank_Acct_ID"]);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstBankAccount.Add(new BankAccount()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_Bank_Acct_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                            ISOCode = Convert.ToString(ds.Tables[0].Rows[i]["ISO_Code"]),
                            CurrentBalance = Convert.ToString(ds.Tables[0].Rows[i]["CurrentBalance"]),
                            Currency_ID = Convert.ToString(ds.Tables[0].Rows[i]["VAB_Currency_ID"])
                        });
                    }
                }
            }
            return lstBankAccount;
        }
        /// <summary>
        /// Get Business Partner
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private List<BusinessPartner> GetBusinesspartner(Ctx ctx)
        {
            List<BusinessPartner> lstBusinessPartner = new List<BusinessPartner>();
            DataSet ds = new DataSet();
            string sqlBPartner = "select bp.VAB_BusinessPartner_ID, bp.Name from VAB_BusinessPartner bp where exists (SELECT * FROM VAB_Invoice i WHERE "
            + " bp.VAB_BusinessPartner_ID=i.VAB_BusinessPartner_ID AND (i.IsSOTrx='N' OR (i.IsSOTrx='Y' AND i.PaymentRule='D'))AND i.IsPaid <>'Y') and "
            + " bp.VAF_Client_ID IN (0, " + ctx.GetVAF_Client_ID() + ") AND (COALESCE(bp.VAF_Org_ID,0) IN(0," + ctx.GetVAF_Org_ID() + ")) ORDER BY 2";
            ds = DB.ExecuteDataset(sqlBPartner, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstBusinessPartner.Add(new BusinessPartner()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_BusinessPartner_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                        });
                    }
                }
            }
            return lstBusinessPartner;
        }
        /// <summary>
        /// GetPayment Method
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<PaymentMethod> GetPaymentMethod(Ctx ctx, int VAB_Bank_Acct_ID)
        {
            List<PaymentMethod> lstPaymentMethod = new List<PaymentMethod>();
            DataSet ds = new DataSet();
            string sqlPaymentRule = "select Name, Value from VAF_CtrlRef_List where VAF_Control_Ref_id = 195 "
                  + " AND VAF_CtrlRef_List.Value IN (SELECT PaymentRule FROM VAB_Bank_AcctDoc WHERE VAB_Bank_Acct_ID=" + VAB_Bank_Acct_ID + ") ORDER BY 2";
            ds = DB.ExecuteDataset(sqlPaymentRule, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstPaymentMethod.Add(new PaymentMethod()
                        {
                            ID = Convert.ToString(ds.Tables[0].Rows[i]["VALUE"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                        });
                    }
                }
            }
            return lstPaymentMethod;
        }
        /// <summary>
        /// Get Grid Data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="bankAccountId"></param>
        /// <param name="VAB_BusinessPartner_ID"></param>
        /// <param name="payDate"></param>
        /// <param name="paymentRule"></param>
        /// <param name="chkDue"></param>
        /// <param name="VAB_Currency_ID"></param>
        /// <returns></returns>
        public List<GridRecords> GetGridData(Ctx ctx, int bankAccountId, int VAB_BusinessPartner_ID, DateTime? payDate, string paymentRule, bool chkDue, int VAB_Currency_ID)
        {
            List<GridRecords> lstGridRecords = new List<GridRecords>();
            string m_sql = "SELECT 'false' as SELECTROW, i.VAB_INVOICE_ID, adddays(i.DateInvoiced, p.NetDays) AS DUEDATE, bp.NAME as BUSINESSPARTNER,i.VAB_BUSINESSPARTNER_ID, i.DOCUMENTNO, c.ISO_CODE as CURRENCY,i.VAB_CURRENCY_ID, i.GRANDTOTAL, "
            + " paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTerm_ID,i.DateInvoiced, @param1) as DISCOUNTAMOUNT, adddays(SYSDATE, -1 * PaymentTermDueDays(i.VAB_PaymentTerm_ID,i.DateInvoiced,SysDate)) as DISCOUNTDATE, "
            + " currencyConvert(invoiceOpen(i.VAB_Invoice_ID,i.VAB_sched_InvoicePayment_ID),i.VAB_Currency_ID, @param2, @param3,i.VAB_CurrencyType_ID, i.VAF_Client_ID,i.VAF_Org_ID) as AMOUNTDUE, "
            + " currencyConvert(invoiceOpen(i.VAB_Invoice_ID,i.VAB_sched_InvoicePayment_ID)-paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTerm_ID,i.DateInvoiced, @param4)  , "
            + " i.VAB_Currency_ID, @param5, @param6,i.VAB_CurrencyType_ID, i.VAF_Client_ID,i.VAF_Org_ID) as PAYMENTAMOUNT FROM VAB_Invoice_v i INNER JOIN VAB_BusinessPartner bp ON "
            + " (i.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) INNER JOIN VAB_Currency c ON (i.VAB_Currency_ID=c.VAB_Currency_ID) INNER JOIN VAB_PaymentTerm p ON "
            + " (i.VAB_PaymentTerm_ID=p.VAB_PaymentTerm_ID)"
            + "   WHERE i.IsSOTrx= @param7 AND IsPaid='N' AND "
            + " i.DocStatus IN ('CO','CL') AND i.VAF_Client_ID= @param8 AND i.VAF_Client_ID IN(0," + ctx.GetVAF_Client_ID() + ") AND (COALESCE(i.VAF_Org_ID,0) IN(0," + ctx.GetVAF_Org_ID() + "))";
            //   string m_sql = "SELECT 'false' as SELECTROW, i.VAB_INVOICE_ID, i.DateInvoiced+p.NetDays AS DUEDATE, bp.NAME as BUSINESSPARTNER,i.VAB_BUSINESSPARTNER_ID, i.DOCUMENTNO, c.ISO_CODE as CURRENCY,i.VAB_CURRENCY_ID, i.GRANDTOTAL, "
            //+ " paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTerm_ID,i.DateInvoiced, null) as DISCOUNTAMOUNT, SysDate-paymentTermDueDays(i.VAB_PaymentTerm_ID,i.DateInvoiced,SysDate) as DISCOUNTDATE, "
            //+ " currencyConvert(invoiceOpen(i.VAB_Invoice_ID,i.VAB_sched_InvoicePayment_ID),i.VAB_Currency_ID, null, null,i.VAB_CurrencyType_ID, i.VAF_Client_ID,i.VAF_Org_ID) as AMOUNTDUE, "
            //+ " currencyConvert(invoiceOpen(i.VAB_Invoice_ID,i.VAB_sched_InvoicePayment_ID)-paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTerm_ID,i.DateInvoiced, null)  , "
            //+ " i.VAB_Currency_ID, null, null,i.VAB_CurrencyType_ID, i.VAF_Client_ID,i.VAF_Org_ID) as PAYMENTAMOUNT FROM VAB_Invoice_v i INNER JOIN VAB_BusinessPartner bp ON "
            //+ " (i.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) INNER JOIN VAB_Currency c ON (i.VAB_Currency_ID=c.VAB_Currency_ID) INNER JOIN VAB_PaymentTerm p ON "
            //+ " (i.VAB_PaymentTerm_ID=p.VAB_PaymentTerm_ID)";
            String sql = m_sql;
            int countParam = 8;
            DateTime payDate1 = (DateTime)payDate;
            string paymentDate = payDate1.ToString("dd-MMM-yyyy");
            int VAB_Bank_Acct_ID = Util.GetValueOfInt(bankAccountId);
            // log.Config("PayDate=" + payDate);
            String isSOTrx = "N";
            //   string paymentRule = Util.GetValueOfString(paymentRule);
            if (paymentRule != "" && X_VAB_Order.PAYMENTRULE_DirectDebit.Equals(paymentRule))
            {
                isSOTrx = "Y";
                sql += " AND i.PaymentRule='" + X_VAB_Order.PAYMENTRULE_DirectDebit + "'";
            }
            if (chkDue)
            {
                sql += " AND i.DateInvoiced+p.NetDays <= @param9";
                countParam++;
            }
            //int VAB_BusinessPartner_ID = Util.GetValueOfInt(cmbBPartner.SelectedValue);
            if (VAB_BusinessPartner_ID != 0)
            {
                sql += " AND i.VAB_BusinessPartner_ID=@param10";
                countParam++;
            }
            sql += " ORDER BY 2,3";
            //log.Finest(sql + " - C_Currecny_ID=" + VAB_Currency_ID + ", VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID);
            ////  Get Open Invoices
            SqlParameter[] para = null;

            para = new SqlParameter[countParam];
            int index = 0;
            para[index++] = new SqlParameter("@param1", paymentDate);
            para[index++] = new SqlParameter("@param2", VAB_Currency_ID);
            para[index++] = new SqlParameter("@param3", paymentDate);
            para[index++] = new SqlParameter("@param4", paymentDate);
            para[index++] = new SqlParameter("@param5", VAB_Currency_ID);
            para[index++] = new SqlParameter("@param6", paymentDate);
            para[index++] = new SqlParameter("@param7", isSOTrx);
            para[index++] = new SqlParameter("@param8", ctx.GetVAF_Client_ID());

            if (chkDue)
            {
                para[index++] = new SqlParameter("@param9", paymentDate);
            }
            if (VAB_BusinessPartner_ID != 0)
            {
                para[index++] = new SqlParameter("@param10", VAB_BusinessPartner_ID);
            }

            DataSet ds = DB.ExecuteDataset(sql, para, null);
            //  DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstGridRecords.Add(new GridRecords()
                        {
                            SELECT = Convert.ToString(ds.Tables[0].Rows[i]["SELECTROW"]),
                            VAB_INVOICE_ID = Convert.ToString(ds.Tables[0].Rows[i]["VAB_INVOICE_ID"]),
                            DUEDATE = Convert.ToString(ds.Tables[0].Rows[i]["DUEDATE"]),
                            BUSINESSPARTNER = Convert.ToString(ds.Tables[0].Rows[i]["BUSINESSPARTNER"]),
                            DOCUMENTNO = Convert.ToString(ds.Tables[0].Rows[i]["DOCUMENTNO"]),
                            CURRENCY = Convert.ToString(ds.Tables[0].Rows[i]["CURRENCY"]),
                            GRANDTOTAL = Convert.ToString(ds.Tables[0].Rows[i]["GRANDTOTAL"]),
                            DISCOUNTAMOUNT = Convert.ToString(ds.Tables[0].Rows[i]["DISCOUNTAMOUNT"]),
                            DISCOUNTDATE = Convert.ToString(ds.Tables[0].Rows[i]["DISCOUNTDATE"]),
                            AMOUNTDUE = Convert.ToString(ds.Tables[0].Rows[i]["AMOUNTDUE"]),
                            PAYMENTAMOUNT = Convert.ToString(ds.Tables[0].Rows[i]["PAYMENTAMOUNT"]),
                            VAB_BUSINESSPARTNER_ID = Convert.ToString(ds.Tables[0].Rows[i]["VAB_BUSINESSPARTNER_ID"]),
                            VAB_CURRENCY_ID = Convert.ToString(ds.Tables[0].Rows[i]["VAB_CURRENCY_ID"])
                        });
                    }
                }
            }
            return lstGridRecords;
        }
        /// <summary>
        ///  Generate PaySelection
        /// </summary>
        public string GeneratePaySelect(Ctx ctx, List<GridRecords> selectedRecords, Decimal? paymentAmt, String paymentRule, int VAB_Bank_Acct_ID, DateTime? payDate)
        {

            Trx trx = null;
            Trx p_trx = null;

            List<int> Invoice_ID = new List<int>();
            List<Decimal?> openAmt = new List<Decimal?>();
            List<Decimal?> payAmt = new List<Decimal?>();

            int rowsSelected = 0;
            Decimal? totalAmt = 0;
            // BindingSource rowSource = vdgvPayment.ItemsSource as BindingSource;
            for (int i = 0; i < selectedRecords.Count; i++)
            {
                if ((Convert.ToBoolean(selectedRecords[i].SELECT)))
                {
                    Decimal? amt = Util.GetValueOfDecimal(selectedRecords[i].PAYMENTAMOUNT);
                    Invoice_ID.Add(Util.GetValueOfInt(selectedRecords[i].VAB_INVOICE_ID));
                    openAmt.Add(Util.GetValueOfDecimal(selectedRecords[i].AMOUNTDUE));
                    payAmt.Add(Util.GetValueOfDecimal(selectedRecords[i].PAYMENTAMOUNT));
                    rowsSelected++;
                }
            }

            if (rowsSelected == 0)
            {
                return "";
            }

            //String paymentRule = Util.GetValueOfString(cmbPaymentRule.SelectedValue);
            //int VAB_Bank_Acct_ID = Util.GetValueOfInt(cmbBankAccount.SelectedValue);
            ////  Create Header
            //DateTime? payDate = Util.GetValueOfDateTime(vdtpPayDate.SelectedDate);


            MPaySelection m_ps = new MPaySelection(ctx, 0, null);
            m_ps.SetName(Msg.GetMsg(ctx, "VPaySelect")
                + " - " + paymentRule
                + " - " + payDate.Value.Date);
            m_ps.SetPayDate(payDate);
            m_ps.SetVAB_Bank_Acct_ID(VAB_Bank_Acct_ID);
            m_ps.SetIsApproved(true);
            if (!m_ps.Save())
            {
                //log.SaveError("SaveError", Msg.Translate(Envs.GetCtx(), "VAB_PaymentOption_ID"));
                m_ps = null;
                return "";
            }

            _VAB_PaymentOption_ID = m_ps.GetVAB_PaymentOption_ID();
            string name = m_ps.GetName();

            //string sqlTableID = "select vaf_tableview_id from vaf_tableview where tablename = 'VAB_PaymentOption'";
            //int VAF_TableView_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlTableID, null, null));

            // log.Config(m_ps.ToString());
            bool isSOTrx = false;
            if (X_VAB_Order.PAYMENTRULE_DirectDebit.Equals(paymentRule))
            {
                isSOTrx = true;
            }

            int line = 0;
            Decimal? pAmt = Decimal.Zero;
            Decimal? oldpAmt = Decimal.Zero;
            for (int j = 0; j < Invoice_ID.Count; j++)
            {
                line = line + 10;
                if (Decimal.Add(pAmt.Value, Util.GetValueOfDecimal(payAmt[j])) > paymentAmt)
                {
                    oldpAmt = Decimal.Subtract(paymentAmt.Value, pAmt.Value);
                }
                pAmt = Decimal.Add(pAmt.Value, Util.GetValueOfDecimal(payAmt[j]));
                // pAmt = Util.GetValueOfDecimal(payAmt[j]);
                MPaySelectionLine psl = new MPaySelectionLine(m_ps, line, paymentRule);
                //psl.SetInvoice(Util.GetValueOfInt(Invoice_ID[j]), isSOTrx, Util.GetValueOfDecimal(openAmt[j]), Util.GetValueOfDecimal(payAmt[j]), Decimal.Subtract(Util.GetValueOfDecimal(openAmt[j]), Util.GetValueOfDecimal(payAmt[j])));
                if (paymentAmt >= pAmt)
                {
                    psl.SetInvoice(Util.GetValueOfInt(Invoice_ID[j]), isSOTrx, Util.GetValueOfDecimal(openAmt[j]), Util.GetValueOfDecimal(payAmt[j]), Decimal.Zero);
                    if (!psl.Save())
                    {
                        // log.SaveError("PaymentSelectionLineNotSaved", "PaymentSelectionLineNotSaved");
                        return "";
                    }
                    // log.Fine("VAB_Invoice_ID=" + Util.GetValueOfInt(Invoice_ID[j]) + ", PayAmt=" + Util.GetValueOfDecimal(payAmt[j]));
                }
                else
                {
                    psl.SetInvoice(Util.GetValueOfInt(Invoice_ID[j]), isSOTrx, Util.GetValueOfDecimal(openAmt[j]), oldpAmt.Value, Decimal.Zero);
                    if (!psl.Save())
                    {
                        //   log.SaveError("PaymentSelectionLineNotSaved", "PaymentSelectionLineNotSaved");
                        return "";
                    }
                    // log.Fine("VAB_Invoice_ID=" + Util.GetValueOfInt(Invoice_ID[j]) + ", PayAmt=" + Util.GetValueOfDecimal(payAmt[j]));
                }

            }


            //if (false.Equals(((Message)sc).DialogResult))
            //{
            //    Dispose();
            //    return;
            //}


            MPaySelection psel = new MPaySelection(ctx, _VAB_PaymentOption_ID, null);
            if (psel.Get_ID() == 0)
            {
                throw new ArgumentException("Not found VAB_PaymentOption_ID=" + _VAB_PaymentOption_ID);
            }
            if (psel.IsProcessed())
            {
                throw new ArgumentException("@Processed@");
            }
            //
            MPaySelectionLine[] lines = psel.GetLines(false);
            List<MPaySelectionCheck> _list = new List<MPaySelectionCheck>();
            for (int i = 0; i < lines.Length; i++)
            {

                MPaySelectionLine payLine = lines[i];
                if (!payLine.IsActive() || payLine.IsProcessed())
                {
                    continue;
                }
                CreateCheck(ctx, payLine, _list);
            }
            //
            psel.SetProcessed(true);
            psel.Save();

            //string sql = "select ad_form_id from ad_form where classname = 'VAdvantage.Apps.AForms.VPayPrint'";
            //int VAF_Page_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

            return "@VAB_PaymentOptionCheck_ID@ - #" + _list.Count;

            //SetBusy(false);
            //Dispose();
            //FormFrame ff = new FormFrame();
            //ff.OpenForm(VAF_Page_ID);







        }   //  generatePaySelect
        /// <summary>
        /// Create Check
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="line"></param>
        /// <param name="_list"></param>
        private void CreateCheck(Ctx ctx, MPaySelectionLine line, List<MPaySelectionCheck> _list)
        {

            string _PaymentRule = "S";
            if (_PaymentRule != null && _PaymentRule.Equals(X_VAB_Order.PAYMENTRULE_DirectDebit))
                _PaymentRule = null;
            ////	Try to find one
            for (int i = 0; i < _list.Count; i++)
            {
                MPaySelectionCheck check = (MPaySelectionCheck)_list[i];
                //	Add to existing
                if (check.GetVAB_BusinessPartner_ID() == line.GetInvoice().GetVAB_BusinessPartner_ID())
                {
                    check.AddLine(line);
                    if (!check.Save())
                    {
                        throw new Exception("Cannot save MPaySelectionCheck");
                    }
                    line.SetVAB_PaymentOptionCheck_ID(check.GetVAB_PaymentOptionCheck_ID());
                    line.SetProcessed(true);
                    if (!line.Save())
                    {
                        throw new Exception("Cannot save MPaySelectionLine");
                    }
                    return;
                }
            }
            ////	Create new
            String PaymentRule = line.GetPaymentRule();
            if (_PaymentRule != null && _PaymentRule != " ")
            {
                if (!X_VAB_Order.PAYMENTRULE_DirectDebit.Equals(PaymentRule))
                {
                    PaymentRule = _PaymentRule;
                }
            }
            MPaySelectionCheck check1 = new MPaySelectionCheck(line, PaymentRule);
            if (!check1.IsValid())
            {
                int VAB_BusinessPartner_ID = check1.GetVAB_BusinessPartner_ID();
                MVABBusinessPartner bp = MVABBusinessPartner.Get(ctx, VAB_BusinessPartner_ID);
                String msg = "@NotFound@ @VAB_BPart_Bank_Acct@: " + bp.GetName();
                throw new Exception(msg);
            }
            if (!check1.Save())
            {
                throw new Exception("Cannot save MPaySelectionCheck");
            }
            line.SetVAB_PaymentOptionCheck_ID(check1.GetVAB_PaymentOptionCheck_ID());
            line.SetProcessed(true);
            if (!line.Save())
            {
                throw new Exception("Cannot save MPaySelectionLine");
            }
            _list.Add(check1);
        }


    }
    //*******************
    //Properties Classes for VPaySelect
    //*******************
    public class PaymentSelectionDetail
    {
        public List<BankAccount> BankAccount { get; set; }
        public List<BusinessPartner> BusinessPartner { get; set; }
        public List<PaymentMethod> PaymentMethod { get; set; }
    }
    public class BankAccount
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public string ISOCode { get; set; }
        public string CurrentBalance { get; set; }
        public string Currency_ID { get; set; }
    }
    public class BusinessPartner
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
    public class PaymentMethod
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }
    public class GridRecords
    {
        public string SELECT { get; set; }
        public string VAB_INVOICE_ID { get; set; }
        public string DUEDATE { get; set; }
        public string BUSINESSPARTNER { get; set; }
        public string DOCUMENTNO { get; set; }
        public string CURRENCY { get; set; }
        public string GRANDTOTAL { get; set; }
        public string DISCOUNTAMOUNT { get; set; }
        public string DISCOUNTDATE { get; set; }
        public string AMOUNTDUE { get; set; }
        public string PAYMENTAMOUNT { get; set; }
        public string VAB_BUSINESSPARTNER_ID { get; set; }
        public string VAB_CURRENCY_ID { get; set; }

    }
    public class PaymentSelection
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
}