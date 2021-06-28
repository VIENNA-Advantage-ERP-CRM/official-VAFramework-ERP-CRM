/********************************************************
 * Project Name   : VIS
 * Class Name     : PaymentModel
 * Purpose        : Used for payment , depending which payment method is choosen...
 * Chronological    Development
 * Karan            
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class PaymentModel
    {
        Ctx ctx = null;
        //List<string> values = null;
        //this.Title =VIS.Msg.getmsg("Payment");
        bool _isSOTrx = false;
        //	Data from Order/Invoice
        string _DocStatus = null;
        // Start Payment Rule       
        string _PaymentRule = "";
        // Invoice Currency              
        int _C_Currency_ID = 0;
        // Start Acct Date          
        DateTime? _DateAcct = null;
        // Start Payment Term       
        int _C_PaymentTerm_ID = 0;
        // Start Payment            
        int _C_Payment_ID = 0;
        // Start CashBook Line      
        private int _C_CashLine_ID = 0;
        private MCashLine _cashLine = null;
        // Start CashBook           
        private int _C_CashBook_ID = 0;
        int _C_BankAccount_ID = 0;
        //log
        //this.log = VIS.Logging.VLogger.getVLogger("vPayment");
        int _AD_Client_ID = 0;
        int _AD_Org_ID = 0;
        int _C_BPartner_ID = 0;
        private Decimal _Amount = Env.ZERO;	//	Payment Amount
        private MPayment _mPayment = null;
        private MPayment _mPaymentOriginal = null;
        //private static IDictionary<int, KeyNamePair> _Currencies = null;	//	EMU Currencies
        KeyNamePair kp = null;
        PaymentMetohdDetails pDetails = null;
        TabDetails details = null;

        public PaymentModel(Ctx _ctx)
        {
            ctx = _ctx;
        }

        public PaymentModel(Ctx _ctx, PaymentInputValues inputValues)
        {
            ctx = _ctx;
            // values = inputValues.values;

            // values = JsonConvert.DeserializeObject<List<string>>(inputValues.values.ToString());

            _isSOTrx = inputValues._isSOTrx;
            _DocStatus = inputValues._DocStatus;
            _PaymentRule = inputValues._PaymentRule;
            _C_Currency_ID = inputValues._C_Currency_ID;
            _DateAcct = Convert.ToDateTime(inputValues._DateAcct);
            _C_PaymentTerm_ID = inputValues._C_PaymentTerm_ID;
            _C_Payment_ID = inputValues._C_Payment_ID;
            _AD_Client_ID = inputValues._AD_Client_ID;
            _AD_Org_ID = inputValues._AD_Org_ID;
            _C_BPartner_ID = inputValues._C_BPartner_ID;
            _Amount = inputValues._Amount;
            _C_CashLine_ID = inputValues._C_CashLine_ID;
        }




        public PaymentMetohdDetails DynInit()
        {
            pDetails = new PaymentMetohdDetails();

            SetPayments(_C_Payment_ID, _C_Currency_ID, _PaymentRule);


            if (_C_CashLine_ID == 0)
            {
                _cashLine = null;
            }
            else
            {
                _cashLine = new MCashLine(ctx, _C_CashLine_ID, null);
                pDetails._DateAcct = DateTime.SpecifyKind(Convert.ToDateTime(_cashLine.GetStatementDate()), DateTimeKind.Utc);
            }
            if (pDetails._Currencies == null)
            {
                LoadCurrencies();
            }



            //Load Payment Terms
            String SQL = MRole.GetDefault(ctx).AddAccessSQL(
                "SELECT C_PaymentTerm_ID, Name FROM C_PaymentTerm WHERE IsActive='Y' ORDER BY Name",
                "C_PaymentTerm", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

            IDataReader idr = null;
            DataTable dt = null;
            try
            {

                idr = DB.ExecuteReader(SQL, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int key = Util.GetValueOfInt(dr[0]);//.getInt(1);
                    String name = dr[1].ToString();//.getString(2);
                    KeyNamePair pp = new KeyNamePair(key, name);
                    //cmbPTerm.Items.Add(pp);
                    if (pDetails.loadPaymentTerms == null)
                    {
                        pDetails.loadPaymentTerms = new List<KeyNamePair>();
                    }
                    pDetails.loadPaymentTerms.Add(pp);

                    if (key == _C_PaymentTerm_ID)
                    {
                        kp = pp;
                    }
                }

            }
            catch (Exception)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                // log.Log(Level.SEVERE, SQL, ept);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }


            //Load Accounts
            SQL = "SELECT a.C_BP_BankAccount_ID, NVL(b.Name, ' ')||a.AccountNo AS Acct "
                + "FROM C_BP_BankAccount a,C_Bank b "
                + "WHERE C_BPartner_ID=" + _C_BPartner_ID + " AND a.IsActive='Y'";
            kp = null;
            IDataReader idr1 = null;
            try
            {

                idr1 = DB.ExecuteReader(SQL, null, null);
                dt = new DataTable();
                dt.Load(idr1);
                idr1.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int key = Util.GetValueOfInt(dr[0]);// dr.getInt(1);
                    String name = dr[1].ToString();//.getString(2);
                    KeyNamePair pp = new KeyNamePair(key, name);
                    // cmbTAccount.Items.Add(pp);

                    if (pDetails.loadAccounts == null)
                    {
                        pDetails.loadAccounts = new List<KeyNamePair>();
                    }

                    pDetails.loadAccounts.Add(pp);
                    /*			kp = pp;*/
                }

            }
            catch (Exception )
            {
                if (idr1 != null)
                {
                    idr1.Close();
                }
                // log.Log(Level.SEVERE, SQL, eac);
            }
            finally
            {
                if (idr1 != null)
                {
                    idr1.Close();
                }
                dt = null;
            }



            //Load Credit Cards
            pDetails.ccs = _mPayment.GetCreditCards();

            //Load Bank Accounts
            SQL = MRole.GetDefault(ctx).AddAccessSQL(
                "SELECT C_BankAccount_ID, Name || ' ' || AccountNo, IsDefault "
                + "FROM C_BankAccount ba"
                + " INNER JOIN C_Bank b ON (ba.C_Bank_ID=b.C_Bank_ID) "
                + "WHERE b.IsActive='Y'",
                "ba", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            kp = null;
            IDataReader idr2 = null;
            try
            {

                idr2 = DB.ExecuteReader(SQL, null, null);
                dt = new DataTable();
                dt.Load(idr2);
                idr2.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int key = Util.GetValueOfInt(dr[0]);// dr.getInt(1);
                    String name = dr[1].ToString();//.getString(2);
                    KeyNamePair pp = new KeyNamePair(key, name);
                    //cmbSBankAccount.Items.Add(pp);

                    if (pDetails.loadBankAccounts == null)
                    {
                        pDetails.loadBankAccounts = new List<KeyNamePair>();
                    }

                    pDetails.loadBankAccounts.Add(pp);
                    if (key == _C_BankAccount_ID)
                    {
                        pDetails.Checkbook_ID = key;
                    }

                }

                if ( pDetails.Checkbook_ID == 0)    //  Default
                {
                    DataRow[] drs = dt.Select("IsDefault='Y'");
                    if (drs != null && drs.Count() > 0)
                    {
                        pDetails.Checkbook_ID = Convert.ToInt32(drs.ElementAt(0)["C_BankAccount_ID"]);
                    }
                }


            }
            catch (Exception )
            {
                if (idr2 != null)
                {
                    idr2.Close();
                }
                //    log.Log(Level.SEVERE, SQL, ept);
            }
            finally
            {
                if (idr2 != null)
                {
                    idr2.Close();
                }
                dt = null;
            }



            //Load Cash Books
            SQL = MRole.GetDefault(ctx).AddAccessSQL(
                "SELECT C_CashBook_ID, Name, AD_Org_ID FROM C_CashBook WHERE IsActive='Y'",
                "C_CashBook", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            kp = null;
            IDataReader idr3 = null;
            try
            {

                idr3 = DB.ExecuteReader(SQL, null, null);
                dt = new DataTable();
                dt.Load(idr3);
                idr3.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int key = Util.GetValueOfInt(dr[0]);// dr.getInt(1);
                    String name = dr[1].ToString();//.getString(2);
                    KeyNamePair pp = new KeyNamePair(key, name);
                    //cmbBCashBook.Items.Add(pp);

                    if (pDetails.loadCashBooks == null)
                    {
                        pDetails.loadCashBooks = new List<KeyNamePair>();
                    }

                    pDetails.loadCashBooks.Add(pp);
                    if (key == _C_CashBook_ID)
                    {
                        kp = pp;
                    }
                    if (kp == null && key == _AD_Org_ID)       //  Default Org
                    {
                        kp = pp;
                    }
                }

            }
            catch (Exception)
            {
                if (idr3 != null)
                {
                    idr3.Close();
                }
                // log.Log(Level.SEVERE, SQL, epc);
            }
            finally
            {
                if (idr3 != null)
                {
                    idr3.Close();
                }
                dt = null;
            }

            return pDetails;
        }

        private void SetPayments(int C_Payment_ID, int C_Currency_ID, string _PaymentRule)
        {
            if (pDetails == null)
            {
                pDetails = new PaymentMetohdDetails();
            }
            //  Existing Payment
            if ( C_Payment_ID != 0)
            {
                if (C_Payment_ID != 0)
                {
                    _mPayment = new MPayment(ctx, C_Payment_ID, null);
                    _mPaymentOriginal = new MPayment(ctx, _C_Payment_ID, null);	//	full copy
                    //  CreditCard
                    pDetails.CCType = _mPayment.GetCreditCardType();
                    pDetails.StrKNumber = _mPayment.GetCreditCardNumber();
                    pDetails.StrKExp = _mPayment.GetCreditCardExp(null);
                    pDetails.StrKApproval = _mPayment.GetVoiceAuthCode();
                    pDetails.StrKStatus = _mPayment.GetR_PnRef();
                    pDetails.CanChange = !_mPayment.IsApproved() || !(_PaymentRule == MOrder.PAYMENTRULE_CreditCard);
                    //	if approved/paid, don't let it change

                    //  Check
                    _C_BankAccount_ID = _mPayment.GetC_BankAccount_ID();
                    pDetails.StrSRouting = _mPayment.GetRoutingNo();
                    pDetails.StrSNumber = _mPayment.GetAccountNo();
                    pDetails.StrSCheck = _mPayment.GetCheckNo();
                    pDetails.StrSStatus = _mPayment.GetR_PnRef();
                    //  Transfer
                    pDetails.StrTStatus = _mPayment.GetR_PnRef();
                }
            }
            if (_mPayment == null)
            {
                _mPayment = new MPayment(ctx, 0, null);
                _mPayment.SetAD_Org_ID(_AD_Org_ID);
                _mPayment.SetIsReceipt(_isSOTrx);
                _mPayment.SetAmount(C_Currency_ID, _Amount);
            }
        }

        /// <summary>
        /// Fill _Currencies with EMU currencies
        /// </summary>
        private void LoadCurrencies()
        {
            //_Currencies = new Hashtable<Integer, KeyNamePair>(12);	//	Currenly only 10+1
            pDetails._Currencies = new Dictionary<int, KeyNamePair>();	//	Currenly only 10+1
            String SQL = "SELECT C_Currency_ID, ISO_Code FROM C_Currency "
                + "WHERE (IsEMUMember='Y' AND EMUEntryDate<SysDate) OR IsEuro='Y' "
                + "ORDER BY 2";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DB.ExecuteReader(SQL, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int id = Util.GetValueOfInt(dr[0]);//.getInt(1);
                    String name = dr[1].ToString();//.getString(2);
                    pDetails._Currencies.Add(id, new KeyNamePair(id, name));
                }

            }
            catch (Exception)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                //  log.Log(Level.SEVERE, SQL, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
        }


        public TabDetails SaveChanges(PaymentInputValues inputs)
        {
            if (CheckMandatory(inputs))
            {
                return Save(inputs);
            }

            return details;
        }


        private TabDetails Save(PaymentInputValues inputs)
        {
           
            try
            {
                StringBuilder errorMsg = new StringBuilder();
                string SucessMsg = "";
                String newPaymentRule = inputs.cmbPayment;
                inputs._DateAcct = Convert.ToDateTime(inputs._DateAcct);
                DateTime? newDateAcct = Convert.ToDateTime(inputs._DateAcct);
                int newC_PaymentTerm_ID = inputs._C_PaymentTerm_ID;
                int newC_CashLine_ID = inputs._C_CashLine_ID;
                int newC_CashBook_ID = inputs._C_CashBook_ID;
                String newCCType = inputs.CCType;
                int newC_BankAccount_ID = 0;

                //	B (Cash)		(Currency)
                if (newPaymentRule.Equals(X_C_Order.PAYMENTRULE_Cash))
                {
                    newC_CashBook_ID = inputs.cmbBCashBook;
                    //newDateAcct = (DateTime)bDateField.GetValue();
                    newDateAcct = Convert.ToDateTime(inputs.bDateField);
                }

                //	K (CreditCard)  Type, Number, Exp, Approval
                else if (newPaymentRule.Equals(X_C_Order.PAYMENTRULE_CreditCard))
                {
                    newCCType = inputs.cmbKType;
                }

                //	T (Transfer)	BPartner_Bank
                else if (newPaymentRule.Equals(MOrder.PAYMENTRULE_DirectDeposit)
                    || newPaymentRule.Equals(MOrder.PAYMENTRULE_DirectDebit))
                {
                    // KeyNamePair myObj = (KeyNamePair)cmbTAccount.SelectedItem;
                }

                //	P (PaymentTerm)	PaymentTerm
                else if (newPaymentRule.Equals(X_C_Order.PAYMENTRULE_OnCredit))
                {
                    newC_PaymentTerm_ID = inputs.cmbPTerm;
                }

                //	S (Check)		(Currency) CheckNo, Routing
                else if (newPaymentRule.Equals(X_C_Order.PAYMENTRULE_Check))
                {
                    //	cmbSCurrency.SelectedItem;
                    newC_BankAccount_ID = inputs.cmbSBankAccount;

                }
                else
                {
                    return details;
                }


                SetPayments(inputs._C_Payment_ID, inputs._C_Currency_ID, inputs._PaymentRule);


                //  find Bank Account if not qualified yet
                if ("KTSD".IndexOf(newPaymentRule) != -1 && newC_BankAccount_ID == 0)
                {
                    String tender = MPayment.TENDERTYPE_CreditCard;
                    if (newPaymentRule.Equals(MOrder.PAYMENTRULE_DirectDeposit))
                    {
                        tender = MPayment.TENDERTYPE_DirectDeposit;
                    }
                    else if (newPaymentRule.Equals(MOrder.PAYMENTRULE_DirectDebit))
                    {
                        tender = MPayment.TENDERTYPE_DirectDebit;
                    }
                    else if (newPaymentRule.Equals(MOrder.PAYMENTRULE_Check))
                    {
                        tender = MPayment.TENDERTYPE_Check;
                    }
                }

                /***********************
     *  Changed PaymentRule
     */
                if (!newPaymentRule.Equals(inputs._PaymentRule))
                {
                    //log.fine("Changed PaymentRule: " + _PaymentRule + " -> " + newPaymentRule);
                    //  We had a CashBook Entry
                    if (inputs._PaymentRule.Equals(X_C_Order.PAYMENTRULE_Cash))
                    {
                        //log.fine("Old Cash - " + _cashLine);
                        if (_cashLine != null)
                        {
                            MCashLine cl = _cashLine.CreateReversal();
                            if (cl.Save())
                            {
                                // log.Config("CashCancelled");
                            }
                            else
                            {

                                errorMsg.Append(Msg.GetMsg(ctx, "CashNotCancelled", true) + Environment.NewLine);
                                //   ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), "CashNotCancelled", true).ToString());
                            }
                        }
                        newC_CashLine_ID = 0;      //  reset
                    }
                    //  We had a change in Payment type (e.g. Check to CC)
                    else if ("KTSD".IndexOf(_PaymentRule) != -1 && "KTSD".IndexOf(newPaymentRule) != -1 && _mPaymentOriginal != null)
                    {
                        //log.fine("Old Payment(1) - " + _mPaymentOriginal);
                        _mPaymentOriginal.SetDocAction(DocActionVariables.ACTION_REVERSE_CORRECT);
                        bool ok = _mPaymentOriginal.ProcessIt(DocActionVariables.ACTION_REVERSE_CORRECT);
                        _mPaymentOriginal.Save();
                        if (ok)
                        {
                            // log.Info("Payment Canecelled - " + _mPaymentOriginal);
                        }
                        else
                        {
                            errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "PaymentNotCancelled", true) + Environment.NewLine);
                            // ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "PaymentNotCancelled", true).ToString() + _mPaymentOriginal.GetDocumentNo());
                        }
                        _mPayment.ReSetNew();
                    }
                    //	We had a Payment and something else (e.g. Check to Cash)
                    else if ("KTSD".IndexOf(_PaymentRule) != -1 && "KTSD".IndexOf(newPaymentRule) == -1)
                    {
                        //log.fine("Old Payment(2) - " + _mPaymentOriginal);
                        if (_mPaymentOriginal != null)
                        {
                            _mPaymentOriginal.SetDocAction(DocActionVariables.ACTION_REVERSE_CORRECT);
                            bool ok = _mPaymentOriginal.ProcessIt(DocActionVariables.ACTION_REVERSE_CORRECT);
                            _mPaymentOriginal.Save();
                            if (ok)        //  Cancel Payment
                            {
                                ///log.Fine("PaymentCancelled " + _mPayment.GetDocumentNo());
                                //_mTab.getTableModel().dataSave(true);
                               // DataRow datarow = null;
                                // _mTab.GetTableObj().DataSave(true, datarow);//**********************************2nd parameter
                                _mPayment.ReSetNew();
                                _mPayment.SetAmount(inputs._C_Currency_ID, inputs._Amount);
                            }
                            else
                            {
                                errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "PaymentNotCancelled", true) + _mPayment.GetDocumentNo() + Environment.NewLine);
                                //ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "PaymentNotCancelled", true).ToString() + _mPayment.GetDocumentNo());
                            }
                        }
                    }
                }


                //  Get Order and optionally Invoice
                //int C_Order_ID = ctx.GetContextAsInt(inputs.WindowNo, "C_Order_ID");
                //int C_Invoice_ID = ctx.GetContextAsInt(inputs.WindowNo, "C_Invoice_ID");
                if (inputs.C_Invoice_ID == 0 && inputs._DocStatus.Equals("CO"))
                {
                    inputs.C_Invoice_ID = GetInvoiceID(inputs.C_Order_ID);
                }
                //  Amount sign negative, if ARC (Credit Memo) or API (AP Invoice)
                bool negateAmt = false;
                MInvoice invoice = null;
                if (inputs.C_Invoice_ID != 0)
                {
                    invoice = new MInvoice(ctx, inputs.C_Invoice_ID, null);
                    negateAmt = invoice.IsCreditMemo();
                }
                MOrder order = null;
                if (invoice == null && inputs.C_Order_ID != 0)
                {
                    order = new MOrder(ctx, inputs.C_Order_ID, null);
                }
                Decimal payAmount = inputs._Amount;
                if (negateAmt)
                {
                    payAmount = Decimal.Negate(inputs._Amount);//.negate();
                }
                // Info
                //    log.Config("C_Order_ID=" + C_Order_ID + ", C_Invoice_ID=" + C_Invoice_ID + ", NegateAmt=" + negateAmt);


                /***********************
                 *  CashBook
                 */
                if (newPaymentRule.Equals(X_C_Order.PAYMENTRULE_Cash))
                {
                    //log.fine("Cash");
                    String description = inputs.Description;

                    if (inputs.C_Invoice_ID == 0 && order == null)
                    {
                        //log.Config("No Invoice!");
                        //     ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "CashNotCreated", true).ToString());
                        errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "CashNotCreated", true) + Environment.NewLine);
                    }
                    else
                    {
                        //  Changed Amount
                        if (inputs._C_CashLine_ID > 0)
                        {
                            _cashLine = new MCashLine(ctx, inputs._C_CashLine_ID, null);
                        }


                        if (_cashLine != null
                            && payAmount.CompareTo(_cashLine.GetAmount()) != 0)
                        {
                            // log.Config("Changed CashBook Amount");
                            _cashLine.SetAmount(payAmount);
                            if (_cashLine.Save())
                            {
                                //     log.Config("CashAmt Changed");
                            }
                        }
                        //	Different Date/CashBook
                        if (_cashLine != null
                            && (newC_CashBook_ID != inputs._C_CashBook_ID
                                || !TimeUtil.IsSameDay(_cashLine.GetStatementDate(), newDateAcct)))
                        {
                            //log.Config("Changed CashBook/Date: " + _C_CashBook_ID + "->" + newC_CashBook_ID);
                            MCashLine reverse = _cashLine.CreateReversal();
                            if (!reverse.Save())
                            {
                                errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "CashNotCancelled", true) + Environment.NewLine);
                                //ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "CashNotCancelled", true).ToString());
                            }
                            _cashLine = null;
                        }

                        //	Create new
                        if (_cashLine == null)
                        {
                            // log.Config("New CashBook");
                            int C_Currency_ID = 0;
                            if (invoice != null)
                            {
                                C_Currency_ID = invoice.GetC_Currency_ID();
                            }
                            if (C_Currency_ID == 0 && order != null)
                            {
                                C_Currency_ID = order.GetC_Currency_ID();
                            }
                            MCash cash = null;
                            if (newC_CashBook_ID != 0)
                            {
                                cash = MCash.Get(ctx, newC_CashBook_ID, newDateAcct, null);
                            }
                            else	//	Default
                            {
                                cash = MCash.Get(ctx, inputs._AD_Org_ID, newDateAcct, C_Currency_ID, null);
                            }
                            if (cash == null || cash.Get_ID() == 0)
                            {
                                errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "CashNotCreated", true) + Environment.NewLine);
                                //ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "CashNotCreated", true).ToString());
                            }
                            else
                            {
                                MCashLine cl = new MCashLine(cash);
                                if (invoice != null)
                                {
                                    cl.SetInvoice(invoice);
                                }
                                if (order != null)
                                {
                                    cl.SetOrder(order, null);
                                    //_needSave = true;
                                }
                                if (cl.Save())
                                {
                                    //log.Config("CashCreated");
                                }
                                else
                                {
                                    errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "CashNotCreated", true) + Environment.NewLine);
                                    //ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "CashNotCreated", true).ToString());
                                }
                            }
                        }
                    }	//	have invoice
                }

                /***********************
                *  Payments
                */
                if ("KS".IndexOf(newPaymentRule) != -1)
                {
                    //log.fine("Payment - " + newPaymentRule);
                    //  Set Amount
                    _mPayment.SetAmount(inputs._C_Currency_ID, payAmount);
                    if (newPaymentRule.Equals(MOrder.PAYMENTRULE_CreditCard))
                    {
                        _mPayment.SetCreditCard(MPayment.TRXTYPE_Sales, newCCType,
                           inputs.txtKNumber, "", inputs.txtKExp);
                        _mPayment.SetPaymentProcessor();
                    }
                    else if (newPaymentRule.Equals(MOrder.PAYMENTRULE_Check))
                    {
                        _mPayment.SetBankCheck(newC_BankAccount_ID, inputs._isSOTrx, inputs.txtSRouting,
                            inputs.txtSNumber, inputs.txtSCheck);
                    }
                    _mPayment.SetC_BPartner_ID(inputs._C_BPartner_ID);
                    _mPayment.SetC_Invoice_ID(inputs.C_Invoice_ID);
                    if (order != null)
                    {
                        _mPayment.SetC_Order_ID(inputs.C_Order_ID);
                        //     _needSave = true;
                    }
                    _mPayment.SetDateTrx(DateTime.SpecifyKind(Convert.ToDateTime(inputs._DateAcct), DateTimeKind.Utc));
                    _mPayment.SetDateAcct(DateTime.SpecifyKind(Convert.ToDateTime(inputs._DateAcct), DateTimeKind.Utc));
                    _mPayment.Save();

                    //  Save/Post
                    if (MPayment.DOCSTATUS_Drafted.Equals(_mPayment.GetDocStatus()))
                    {
                        bool ok = _mPayment.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                        _mPayment.Save();
                        if (ok)
                        {
                            SucessMsg = _mPayment.GetDocumentNo().ToString() + Msg.GetMsg(ctx, "PaymentCreated");
                            //ShowMessage.Info(_mPayment.GetDocumentNo().ToString(), true, "PaymentCreated", "");
                        }
                        else
                        {
                            errorMsg.Append("PaymentError---" + Msg.GetMsg(ctx, "PaymentNotCreated", true) + Environment.NewLine);
                            //ShowMessage.Error("", true, "PaymentError" + Msg.GetMsg(Envs.GetContext(), "PaymentNotCreated", true).ToString());
                        }
                    }
                    else
                    {
                        // log.Fine("NotDraft " + _mPayment);
                    }
                }
                if (details == null)
                {
                    details = new TabDetails();
                }
                details.newPaymentRule = newPaymentRule;
                details.newDateAcct = newDateAcct;
                details._C_CashLine_ID = inputs._C_CashLine_ID;
                details._C_Payment_ID = inputs._C_Payment_ID;
                details.newC_PaymentTerm_ID = inputs.cmbPTerm;
                details.ErrorMsg = errorMsg.ToString();
                details.SucessMsg = SucessMsg;
            }
            catch
            {

            }
            return details;
        }


        /// <summary>
        /// Get Invoice ID for Order
        /// </summary>
        /// <param name="C_Order_ID">order</param>
        /// <returns>C_Invoice_ID or 0 if not found</returns>
        private int GetInvoiceID(int C_Order_ID)
        {
            int retValue = 0;
            String sql = "SELECT C_Invoice_ID FROM C_Invoice WHERE C_Order_ID=" + C_Order_ID
                + "ORDER BY C_Invoice_ID DESC";     //  last invoice
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Util.GetValueOfInt(dr[0]);//.getInt(1);
                }

            }
            catch (Exception)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                //log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return retValue;
        }



        /// <summary>
        /// Check Mandatory
        /// </summary>
        /// <returns>true if all mandatory items are OK</returns>
        private bool CheckMandatory(PaymentInputValues inputs)
        {
            if (details == null)
            {
                details = new TabDetails();
            }
            bool dataOK = true;
            StringBuilder msg = new StringBuilder();
            try
            {
                //	log.config( "VPayment.checkMandatory");
                
                String PaymentRule = inputs.cmbPayment;
                if (PaymentRule == null)
                {
                    return false;
                }
                //  only Payment Rule
                if (inputs.OnlyRule)
                {
                    return true;
                }

                DateTime? DateAcct = inputs._DateAcct;
                int C_PaymentTerm_ID = inputs._C_PaymentTerm_ID;
                int C_CashBook_ID = inputs._C_CashBook_ID;
                String CCType = inputs.CCType;
                //
                int C_BankAccount_ID = 0;

                /***********************
                 *	Mandatory Data Check
                 */

                //	B (Cash)		(Currency)
                if (PaymentRule.Equals(MOrder.PAYMENTRULE_Cash))
                {
                    C_CashBook_ID = inputs.cmbBCashBook;
                    DateAcct = Convert.ToDateTime(inputs.bDateField);
                }

                //	K (CreditCard)  Type, Number, Exp, Approval
                else if (PaymentRule.Equals(MOrder.PAYMENTRULE_CreditCard))
                {
                    CCType = inputs.cmbKType;
                    //
                    String error = MPaymentValidate.ValidateCreditCardNumber(inputs.txtKNumber, CCType);
                    if (error.Length != 0)
                    {

                        //txtKNumber.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        if (error.IndexOf("?") == -1)
                        {
                            msg.Append(Msg.GetMsg(ctx, error)+Environment.NewLine);
                            //  ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), error, true).ToString());
                            dataOK = false;
                        }
                        else    //  warning
                        {
                            //if (!ShowMessage.Ask("", true, error))
                            //{
                            dataOK = false;
                            //}
                        }
                    }
                    error = MPaymentValidate.ValidateCreditCardExp(inputs.txtKExp);
                    if (error.Length != 0)
                    {
                        //txtKExp.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        msg.Append(Msg.GetMsg(ctx, error) + Environment.NewLine);
                        //ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), error, true).ToString());
                        dataOK = false;
                    }
                }

                //	T (Transfer)	BPartner_Bank
                else if (PaymentRule.Equals(X_C_Order.PAYMENTRULE_DirectDeposit)
                    || PaymentRule.Equals(X_C_Order.PAYMENTRULE_DirectDebit))
                {
                    string bpba = inputs.cmbTAccount;
                    if (bpba == null || bpba == "")
                    {
                        msg.Append(Msg.GetMsg(ctx, "PaymentBPBankNotFound") + Environment.NewLine);
                     //   details.ErrorMsg = Msg.GetMsg(ctx, "PaymentBPBankNotFound");
                        //cmbTAccount.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        //ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), "PaymentBPBankNotFound", true).ToString());
                        dataOK = false;
                    }
                }	//	Direct

                //	P (PaymentTerm)	PaymentTerm
                else if (PaymentRule.Equals(X_C_Order.PAYMENTRULE_OnCredit))
                {
                    C_PaymentTerm_ID = inputs.cmbPTerm;

                }

                //	S (Check)		(Currency) CheckNo, Routing
                else if (PaymentRule.Equals(MOrder.PAYMENTRULE_Check))
                {
                    //	cmbSCurrency.SelectedItem;
                    C_BankAccount_ID = inputs.cmbSBankAccount;

                    String error = MPaymentValidate.ValidateRoutingNo(inputs.txtSRouting);
                    if (error.Length != 0)
                    {
                        msg.Append(Msg.GetMsg(ctx, error) + Environment.NewLine);
                        //    txtSRouting.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        //   ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), error, true).ToString());
                        dataOK = false;
                    }
                    error = MPaymentValidate.ValidateAccountNo(inputs.txtSNumber);
                    if (error.Length != 0)
                    {
                        msg.Append(Msg.GetMsg(ctx, error) + Environment.NewLine);
                        // txtSNumber.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        // ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), error, true).ToString());
                        dataOK = false;
                    }
                    error = MPaymentValidate.ValidateCheckNo(inputs.txtSCheck);
                    if (error.Length != 0)
                    {
                        msg.Append(Msg.GetMsg(ctx, error) + Environment.NewLine);
                        // txtSCheck.Background = new SolidColorBrush(GlobalVariable.MANDATORY_TEXT_BACK_COLOR);
                        // ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), error, true).ToString());
                        dataOK = false;
                    }
                }
                else
                {
                    //log.Log(Level.SEVERE, "Unknown PaymentRule " + PaymentRule);
                    return false;
                }

                //  find Bank Account if not qualified yet
                if ("KTSD".IndexOf(PaymentRule) != -1 && C_BankAccount_ID == 0)
                {
                    String tender = MPayment.TENDERTYPE_CreditCard;
                    if (PaymentRule.Equals(MOrder.PAYMENTRULE_DirectDeposit))
                    {
                        tender = MPayment.TENDERTYPE_DirectDeposit;
                    }
                    else if (PaymentRule.Equals(MOrder.PAYMENTRULE_DirectDebit))
                    {
                        tender = MPayment.TENDERTYPE_DirectDebit;
                    }
                    else if (PaymentRule.Equals(MOrder.PAYMENTRULE_Check))
                    {
                        tender = MPayment.TENDERTYPE_Check;
                    }
                    //	Check must have a bank account
                    if (C_BankAccount_ID == 0 && "S".Equals(PaymentRule))
                    {
                        msg.Append(Msg.GetMsg(ctx, "PaymentNoProcessor") + Environment.NewLine);
                        //  ShowMessage.Error("", true, Msg.GetMsg(Envs.GetContext(), "PaymentNoProcessor", true).ToString());
                        dataOK = false;
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Append(Msg.GetMsg(ctx, ex.Message) + Environment.NewLine);
                //    log.Severe(ex.ToString());
            }
            //log.Config("OK=" + dataOK);
            details.ErrorMsg = msg.ToString();
            return dataOK;

        }
    }

    /// <summary>
    /// Used to get input from  View
    /// </summary>
    public class PaymentInputValues
    {
        public string values { get; set; }
        //this.Title =VIS.Msg.getmsg("Payment");
        public bool _isSOTrx { get; set; }
        //	Data from Order/Invoice
        public string _DocStatus { get; set; }
        // Start Payment Rule       
        public string _PaymentRule { get; set; }
        // Invoice Currency              
        public int _C_Currency_ID { get; set; }
        // Start Acct Date          
        public DateTime? _DateAcct { get; set; }
        // Start Payment Term       
        public int _C_PaymentTerm_ID { get; set; }
        // Start Payment            
        public int _C_Payment_ID { get; set; }
        //log
        public int _AD_Client_ID { get; set; }
        public int _AD_Org_ID { get; set; }
        public int _C_BPartner_ID { get; set; }
        public decimal _Amount { get; set; }
        public int _C_CashLine_ID { get; set; }
        public int _C_CashBook_ID { get; set; }
        public string CCType { get; set; }

        public string cmbKType { get; set; }
        public string cmbPayment { get; set; }
        public string cmbBCurrency { get; set; }
        public int cmbBCashBook { get; set; }
        public string bDateField { get; set; }
        public string txtKNumber { get; set; }
        public string txtKExp { get; set; }
        public string txtKApproval { get; set; }
        public string cmbTAccount { get; set; }
        public int cmbPTerm { get; set; }
        public int cmbSBankAccount { get; set; }
        public string txtSRouting { get; set; }
        public string txtSNumber { get; set; }
        public string txtSCheck { get; set; }
        public string cmbSCurrency { get; set; }
        public bool OnlyRule { get; set; }
        public int WindowNo { get; set; }
        public int C_Order_ID { get; set; }
        public int C_Invoice_ID { get; set; }
        public string Description { get; set; }

    }

    /// <summary>
    /// Used to set details 
    /// </summary>
    public class PaymentMetohdDetails
    {
        public string CCType { get; set; }
        public string StrKNumber { get; set; }
        public string StrKExp { get; set; }
        public string StrKApproval { get; set; }
        public string StrKStatus { get; set; }
        public bool CanChange { get; set; }
        public string StrSRouting { get; set; }
        public string StrSNumber { get; set; }
        public string StrSCheck { get; set; }
        public string StrSStatus { get; set; }
        public string StrTStatus { get; set; }
        public ValueNamePair vp { get; set; }
        public KeyNamePair kp { get; set; }
        public int C_BankAccount_ID { get; set; }
        public List<KeyNamePair> loadPaymentTerms { get; set; }
        public List<KeyNamePair> loadAccounts { get; set; }
        public List<KeyNamePair> loadBankAccounts { get; set; }
        public List<KeyNamePair> loadCashBooks { get; set; }
        public Dictionary<int, KeyNamePair> _Currencies { get; set; }
        public ValueNamePair[] ccs { get; set; }
        public DateTime? _DateAcct { get; set; }
        public int _cashLine { get; set; }
        public int Checkbook_ID { get; set; }
    }

    public class TabDetails
    {
        public string newPaymentRule { get; set; }
        public DateTime? newDateAcct { get; set; }
        public int newC_PaymentTerm_ID { get; set; }
        public int _C_Payment_ID { get; set; }
        public int _C_CashLine_ID { get; set; }
        public string ErrorMsg { get; set; }
        public string SucessMsg { get; set; }
    }








}