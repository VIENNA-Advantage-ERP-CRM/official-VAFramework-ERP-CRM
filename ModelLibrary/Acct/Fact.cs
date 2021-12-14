/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Fact
 * Purpose        : Accounting Fact
 * Class Used     : none
 * Chronological    Development
 * Raghunandan      12-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Report;
using System.Data.SqlClient;

namespace VAdvantage.Acct
{
    public sealed class Fact
    {
        #region Private variable
        //Log
        private VLogger log = null;

        //Document            
        private Doc _doc = null;
        // Accounting Schema   
        private MAcctSchema _acctSchema = null;
        // Transaction			
        //private String _trxName;

        private Trx _trx;

        // Posting Type        
        private String _postingType = null;

        // Actual Balance Type 
        public static String POST_Actual = X_Fact_Acct.POSTINGTYPE_Actual;
        // Budget Balance Type 
        public static String POST_Budget = X_Fact_Acct.POSTINGTYPE_Budget;
        // Commitment Posting 
        public static String POST_Commitment = X_Fact_Acct.POSTINGTYPE_Commitment;
        // Reservation Posting 
        public static String POST_Reservation = X_Fact_Acct.POSTINGTYPE_Reservation;
        /** Statistical = S */
        public static String POSTINGTYPE_Statistical = X_Fact_Acct.POSTINGTYPE_Statistical;
        /** Virtual = V */
        public static String POSTINGTYPE_Virtual = X_Fact_Acct.POSTINGTYPE_Virtual;

        // Is Converted
        private bool _converted = false;
        //Lines        
        private List<FactLine> _lines = new List<FactLine>();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="document">pointer to document</param>
        /// <param name="acctSchema">Account Schema to create accounts</param>
        /// <param name="defaultPostingType">the default Posting type (actual,..) for this posting</param>
        public Fact(Doc document, MAcctSchema acctSchema, String defaultPostingType)
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
            _doc = document;
            _acctSchema = acctSchema;
            _postingType = defaultPostingType;
            //
            log.Config(ToString());
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _lines.Clear();
            _lines = null;
        }

        /// <summary>
        /// Create and convert Fact Line.
        /// Used to create a DR and/or CR entry
        /// </summary>
        /// <param name="docLine">the document line or null</param>
        /// <param name="account">if null, line is not created</param>
        /// <param name="C_Currency_ID">the currency</param>
        /// <param name="debitAmt">debit amount, can be null</param>
        /// <param name="creditAmt">credit amount, can be null</param>
        /// <returns>Fact Line</returns>
        public FactLine CreateLine(DocLine docLine, MAccount account, int C_Currency_ID, Decimal? debitAmt, Decimal? creditAmt)
        {
            //  Data Check
            if (account == null)
            {
                log.Info("No account for " + docLine + ": Amt=" + debitAmt + "/" + creditAmt
                    + " - " + ToString());
                return null;
            }

            // when on same GL Line, Cr / Dr amount passed, then on reversal record posting overwrite credit amount with ZERO
            if (debitAmt < 0 && creditAmt < 0)
            {
                decimal dAmt = debitAmt.Value;
                debitAmt = Decimal.Negate(creditAmt.Value);
                creditAmt = Decimal.Negate(dAmt);
            }
            else if (debitAmt < 0)
            {
                creditAmt = Decimal.Negate(debitAmt.Value);
                debitAmt = 0;
            }
            else if (creditAmt < 0)
            {
                debitAmt = Decimal.Negate(creditAmt.Value);
                creditAmt = 0;
            }

            //
            FactLine line = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(),
                _doc.Get_ID(),
                docLine == null ? 0 : (docLine.GetPrimaryKeyValue != 0 ? docLine.GetPrimaryKeyValue : docLine.Get_ID()), _doc.GetAD_Window_ID(), _trx);
            // set accounting schema reference 
            line.SetC_AcctSchema_ID(_acctSchema.GetC_AcctSchema_ID());
            //  Set Info & Account
            line.SetDocumentInfo(_doc, docLine);
            line.SetPostingType(_postingType);
            line.SetAccount(_acctSchema, account);
            //  Amounts - one needs to not zero
            if (!line.SetAmtSource(C_Currency_ID, debitAmt, creditAmt))
            {
                if (docLine == null || docLine.GetQty() == null || Env.Signum(Utility.Util.GetValueOfDecimal(docLine.GetQty())) == 0)
                {
                    log.Fine("Both amounts & qty = 0/Null - " + docLine
                        + " - " + ToString());
                    return null;
                }
                log.Fine("Both amounts = 0/Null, Qty=" + docLine.GetQty() + " - " + docLine
                    + " - " + ToString());
            }
            //  Convert
            line.Convert();
            //  Optionally overwrite Acct Amount
            if (docLine != null
                && (docLine.GetAmtAcctDr() != null || docLine.GetAmtAcctCr() != null))
                line.SetAmtAcct(docLine.GetAmtAcctDr(), docLine.GetAmtAcctCr());
            //
            log.Fine(line.ToString());
            Add(line);
            return line;
        }


        /// <summary>
        /// Create and convert Fact Line.
        /// Used to create a DR and/or CR entry
        /// </summary>
        /// <param name="docLine">the document line or null</param>
        /// <param name="account">if null, line is not created</param>
        /// <param name="C_Currency_ID">the currency</param>
        /// <param name="debitAmt">debit amount, can be null</param>
        /// <param name="creditAmt">credit amount, can be null</param>
        /// <returns>Fact Line</returns>
        public FactLine CreateLine(DocLine docLine, MAccount account, int C_Currency_ID, Decimal? debitAmt, Decimal? creditAmt, int AD_Org_ID)
        {
            //  Data Check
            if (account == null)
            {
                log.Info("No account for " + docLine + ": Amt=" + debitAmt + "/" + creditAmt
                    + " - " + ToString());
                return null;
            }
            //
            FactLine line = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(),
                _doc.Get_ID(),
                docLine == null ? 0 : (docLine.GetPrimaryKeyValue != 0 ? docLine.GetPrimaryKeyValue : docLine.Get_ID()), _trx);
            //  Set Info & Account
            line.SetDocumentInfo(_doc, docLine);
            line.SetPostingType(_postingType);
            line.SetAccount(_acctSchema, account);
            if (AD_Org_ID > 0)
            {
                line.SetAD_Org_ID(AD_Org_ID);
            }
            //  Amounts - one needs to not zero
            if (!line.SetAmtSource(C_Currency_ID, debitAmt, creditAmt))
            {
                if (docLine == null || docLine.GetQty() == null || Env.Signum(Utility.Util.GetValueOfDecimal(docLine.GetQty())) == 0)
                {
                    log.Fine("Both amounts & qty = 0/Null - " + docLine
                        + " - " + ToString());
                    return null;
                }
                log.Fine("Both amounts = 0/Null, Qty=" + docLine.GetQty() + " - " + docLine
                    + " - " + ToString());
            }
            //  Convert
            line.Convert();
            //  Optionally overwrite Acct Amount
            if (docLine != null
                && (docLine.GetAmtAcctDr() != null || docLine.GetAmtAcctCr() != null))
                line.SetAmtAcct(docLine.GetAmtAcctDr(), docLine.GetAmtAcctCr());
            //
            log.Fine(line.ToString());
            Add(line);
            return line;
        }
       
        /// <summary>
        ///  Add Fact Line
        /// </summary>
        /// <param name="line">line fact line</param>
        void Add(FactLine line)
        {
            _lines.Add(line);
        }

        /// <summary>
        /// Create and convert Fact Line.
        /// Used to create either a DR or CR entry
        /// </summary>
        /// <param name="docLine">Document Line or null</param>
        /// <param name="accountDr">Account to be used if Amt is DR balance</param>
        /// <param name="accountCr">Account to be used if Amt is CR balance</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="Amt">if negative Cr else Dr</param>
        /// <returns>FactLine</returns>
        public FactLine CreateLine(DocLine docLine, MAccount accountDr, MAccount accountCr, int C_Currency_ID, Decimal? Amt)
        {
            if (Env.Signum(Amt.Value) < 0)
            {
                return CreateLine(docLine, accountCr, C_Currency_ID, null, Math.Abs(Amt.Value));
            }
            else
            {
                return CreateLine(docLine, accountDr, C_Currency_ID, Amt.Value, null);
            }
        }

        /// <summary>
        /// Create and convert Fact Line.
        /// Used to create either a DR or CR entry
        /// </summary>
        /// <param name="docLine">Document line or null</param>
        /// <param name="account">Account to be used</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="Amt">if negative Cr else Dr</param>
        /// <returns>FactLine</returns>
        public FactLine CreateLine(DocLine docLine, MAccount account, int C_Currency_ID, Decimal? Amt)
        {
            if (Env.Signum(Amt.Value) < 0)
            {
                return CreateLine(docLine, account, C_Currency_ID, null, Math.Abs(Amt.Value));
            }
            else
            {
                return CreateLine(docLine, account, C_Currency_ID, Amt.Value, null);
            }
        }

        /// <summary>
        /// Create and convert Fact Line.
        /// Used to create either a DR or CR entry
        /// </summary>
        /// <param name="docLine">Document line or null</param>
        /// <param name="account">Account to be used</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="Amt">if negative Cr else Dr</param>
        /// <param name ="AD_Org_ID">Set Line Org</param>
        /// <returns>FactLine</returns>
        public FactLine CreateLine(DocLine docLine, MAccount account, int C_Currency_ID, Decimal? Amt, int AD_Org_ID)
        {
            if (Env.Signum(Amt.Value) < 0)
            {
                return CreateLine(docLine, account, C_Currency_ID, null, Math.Abs(Amt.Value), AD_Org_ID);
            }
            else
            {
                return CreateLine(docLine, account, C_Currency_ID, Amt.Value, null, AD_Org_ID);
            }
        }
        /// <summary>
        /// Is Posting Type
        /// </summary>
        /// <param name="PostingType"></param>
        /// <returns>true if document is posting type</returns>
        public bool IsPostingType(String PostingType)
        {
            return _postingType.Equals(PostingType);
        }

        /// <summary>
        /// Is converted
        /// </summary>
        /// <returns>true if converted</returns>
        public bool IsConverted()
        {
            return _converted;
        }

        /// <summary>
        /// Get AcctSchema
        /// </summary>
        /// <returns>AcctSchema</returns>
        public MAcctSchema GetAcctSchema()
        {
            return _acctSchema;
        }

        /// <summary>
        /// Are the lines Source Balanced
        /// </summary>
        /// <returns>true if source lines balanced</returns>
        public bool IsSourceBalanced()
        {
            //  No lines -> balanded
            if (_lines.Count == 0)
            {
                return true;
            }
            Decimal balance = GetSourceBalance();
            bool retValue = Env.Signum(balance) == 0;
            if (retValue)
            {
                log.Finer(ToString());
            }
            else
            {
                log.Warning("NO - Diff=" + balance + " - " + ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Return Source Balance
        /// </summary>
        /// <returns>source balance</returns>
        public Decimal GetSourceBalance()
        {
            Decimal result = Env.ZERO;
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine line = (FactLine)_lines[i];
                result = Decimal.Add(result, line.GetSourceBalance());
            }
            return result;
        }

        /// <summary>
        /// Create Source Line for Suspense Balancing.
        /// Only if Suspense Balancing is enabled and not a multi-currency document
        /// (double check as1 otherwise the rule should not have fired)
        /// If not balanced create balancing entry in currency of the document
        /// </summary>
        /// <returns>FactLine</returns>
        public FactLine BalanceSource()
        {
            if (!_acctSchema.IsSuspenseBalancing() || _doc.IsMultiCurrency())
            {
                return null;
            }
            Decimal diff = GetSourceBalance();
            log.Finer("Diff=" + diff);

            //  new line
            FactLine line = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(),
                _doc.Get_ID(), 0, _trx);
            line.SetDocumentInfo(_doc, null);
            line.SetPostingType(_postingType);

            //  Amount
            if (Env.Signum(diff) < 0)   //  negative balance => DR
            {
                line.SetAmtSource(_doc is Doc_GLJournal ? _acctSchema.GetC_Currency_ID() : _doc.GetC_Currency_ID(), Math.Abs(diff), Env.ZERO);
            }
            else                                //  positive balance => CR
            {
                line.SetAmtSource(_doc is Doc_GLJournal ? _acctSchema.GetC_Currency_ID() : _doc.GetC_Currency_ID(), Env.ZERO, diff);
            }

            //	Account
            line.SetAccount(_acctSchema, _acctSchema.GetSuspenseBalancing_Acct());

            // Conversion rate
            if (_lines != null && _lines.Count > 0 && _lines[0].GetConversionRate() > 0)
            {
                line.SetConversionRate(_lines[0].GetConversionRate());
            }

            //  Convert
            line.Convert();
            //
            log.Fine(line.ToString());
            _lines.Add(line);
            return line;
        }


        /// <summary>
        /// Are all segments balanced
        /// </summary>
        /// <returns>true if segments are balanced</returns>
        public bool IsSegmentBalanced()
        {
            if (_lines.Count == 0)
            {
                return true;
            }

            MAcctSchemaElement[] elements = _acctSchema.GetAcctSchemaElements();
            //  check all balancing segments
            for (int i = 0; i < elements.Length; i++)
            {
                MAcctSchemaElement ase = elements[i];
                if (ase.IsBalanced() && !IsSegmentBalanced(ase.GetElementType()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Is Source Segment balanced.
        /// Implemented only for Org
        /// Other sensible candidates are Project, User1/2
        /// </summary>
        /// <param name="segmentType"></param>
        /// <returns>true if segments are balanced</returns>
        public bool IsSegmentBalanced(String segmentType)
        {
            if (segmentType.Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Organization))
            {
                Dictionary<int, Decimal?> map = new Dictionary<int, Decimal?>();
                //  Add up values by key
                for (int i = 0; i < _lines.Count; i++)
                {
                    //FactLine line = (FactLine)_lines[i];
                    //int key = Utility.Util.GetValueOfInt(line.GetAD_Org_ID());
                    //Decimal bal = line.GetSourceBalance();
                    //Decimal oldBal;
                    //if (map.TryGetValue(key, out oldBal))
                    //{
                    //    oldBal = map[key];
                    //    bal = Decimal.Add(bal, oldBal);
                    //    map[key] = bal;//put(key, bal);
                    //}
                    //map.Add(key, bal);//put(key, bal);
                    try
                    {
                        FactLine line = (FactLine)_lines[i];
                        int key = Utility.Util.GetValueOfInt(line.GetAD_Org_ID());
                        Decimal? bal = line.GetSourceBalance();
                        Decimal? oldBal = null;
                        if (map.TryGetValue(key, out oldBal))
                        {
                            oldBal = map[key];
                        }
                        if (oldBal != null)
                        {
                            bal = Decimal.Add(bal.Value, oldBal.Value);
                        }
                        if (!map.Keys.Contains(key))
                        {
                            map.Add(key, bal);
                        }
                        else
                        {
                            map[key] = bal;
                        }
                    }
                    catch { }
                }
                //  check if all keys are zero
                //Iterator values = map.values().iterator();
                IEnumerator values = map.Values.GetEnumerator();
                while (values.MoveNext())
                {
                    Decimal bal = (Decimal)values.Current;
                    if (Env.Signum(bal) != 0)
                    {
                        map.Clear();
                        log.Warning("(" + segmentType + ") NO - " + ToString() + ", Balance=" + bal);
                        return false;
                    }
                }
                map.Clear();
                log.Finer("(" + segmentType + ") - " + ToString());
                return true;
            }
            log.Finer("(" + segmentType + ") (not checked) - " + ToString());
            return true;
        }

        /// <summary>
        /// Balance all segments.
        /// - For all balancing segments
        /// - For all segment values
        /// - If balance <> 0 create dueTo/dueFrom line
        /// overwriting the segment value
        /// </summary>
        public void BalanceSegments()
        {
            MAcctSchemaElement[] elements = _acctSchema.GetAcctSchemaElements();
            //  check all balancing segments
            for (int i = 0; i < elements.Length; i++)
            {
                MAcctSchemaElement ase = elements[i];
                if (ase.IsBalanced())
                {
                    BalanceSegment(ase.GetElementType());
                }
            }
        }

        /// <summary>
        /// Balance Source Segment
        /// </summary>
        /// <param name="elementType"> elementType segment element type</param>
        private void BalanceSegment(String elementType)
        {
            //  no lines -> balanced
            if (_lines.Count == 0)
            {
                return;
            }

            log.Fine("(" + elementType + ") - " + ToString());

            //  Org
            if (elementType.Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Organization))
            {
                Dictionary<int, Balance> map = new Dictionary<int, Balance>();
                //  Add up values by key
                for (int i = 0; i < _lines.Count; i++)
                {
                    FactLine line = (FactLine)_lines[i];
                    int key = Utility.Util.GetValueOfInt(line.GetAD_Org_ID());
                    //	Decimal balance = line.getSourceBalance();
                    Balance oldBalance = null;
                    if (map.TryGetValue(key, out oldBalance))
                    {
                        oldBalance = map[key];
                    }
                    if (oldBalance == null)
                    {
                        oldBalance = new Balance(line.GetAmtSourceDr(), line.GetAmtSourceCr());
                        if (!map.Keys.Contains(key))
                        {
                            map.Add(key, oldBalance);//put(key, oldBalance);
                        }
                        map[key] = oldBalance;
                    }
                    else
                    {
                        oldBalance.Add(line.GetAmtSourceDr(), line.GetAmtSourceCr());
                    }
                }

                //  Create entry for non-zero element
                //Iterator keys = map.keySet().iterator();
                IEnumerator keys = map.Keys.GetEnumerator();
                while (keys.MoveNext())
                {
                    int key = Utility.Util.GetValueOfInt(keys.Current);
                    Balance difference = (Balance)map[key];
                    log.Info(elementType + "=" + key + ", " + difference);
                    //
                    if (!difference.IsZeroBalance())
                    {
                        //  Create Balancing Entry
                        FactLine line = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(), _doc.Get_ID(), 0, _trx);
                        line.SetDocumentInfo(_doc, null);
                        line.SetPostingType(_postingType);
                        //  Amount & Account
                        if (Env.Signum(difference.GetBalance()) < 0)
                        {
                            if (difference.IsReversal())
                            {
                                line.SetAmtSource(_doc.GetC_Currency_ID(), Env.ZERO, difference.GetPostBalance());
                                line.SetAccount(_acctSchema, _acctSchema.GetDueTo_Acct(elementType));
                            }
                            else
                            {
                                line.SetAmtSource(_doc.GetC_Currency_ID(), difference.GetPostBalance(), Env.ZERO);
                                line.SetAccount(_acctSchema, _acctSchema.GetDueFrom_Acct(elementType));
                            }
                        }
                        else
                        {
                            if (difference.IsReversal())
                            {
                                line.SetAmtSource(_doc.GetC_Currency_ID(), difference.GetPostBalance(), Env.ZERO);
                                line.SetAccount(_acctSchema, _acctSchema.GetDueFrom_Acct(elementType));
                            }
                            else
                            {
                                line.SetAmtSource(_doc.GetC_Currency_ID(), Env.ZERO, difference.GetPostBalance());
                                line.SetAccount(_acctSchema, _acctSchema.GetDueTo_Acct(elementType));
                            }
                        }
                        line.Convert();
                        line.SetAD_Org_ID(Utility.Util.GetValueOfInt(key));
                        //
                        _lines.Add(line);
                        log.Fine("(" + elementType + ") - " + line);
                    }
                }
                map.Clear();
            }
        }


        /// <summary>
        /// Are the lines Accounting Balanced
        /// </summary>
        /// <returns>true if accounting lines are balanced</returns>
        public bool IsAcctBalanced()
        {
            //  no lines -> balanced
            if (_lines.Count == 0)
            {
                return true;
            }
            Decimal balance = GetAcctBalance();
            bool retValue = Env.Signum(balance) == 0;
            if (retValue)
            {
                log.Finer(ToString());
            }
            else
            {
                log.Warning("NO - Diff=" + balance + " - " + ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Return Accounting Balance
        /// </summary>
        /// <returns>true if accounting lines are balanced</returns>
        public Decimal GetAcctBalance()
        {
            Decimal result = Env.ZERO;
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine line = (FactLine)_lines[i];
                result = Decimal.Add(result, line.GetAcctBalance());
            }
            //	log.Fine(result.toString());
            return result;
        }

        /// <summary>
        /// Balance Accounting Currency.
        /// If the accounting currency is not balanced,
        /// if Currency balancing is enabled
        /// create a new line using the currency balancing account with zero source balance
        /// or
        /// adjust the line with the largest balance sheet account
        /// or if no balance sheet account exist, the line with the largest amount
        /// </summary>
        /// <returns>FactLine</returns>
        public FactLine BalanceAccounting()
        {
            Decimal diff = GetAcctBalance();		//	DR-CR
            log.Fine("Balance=" + diff
                + ", CurrBal=" + _acctSchema.IsCurrencyBalancing()
                + " - " + ToString());
            FactLine line = null;

            Decimal BSamount = Env.ZERO;
            FactLine BSline = null;
            Decimal PLamount = Env.ZERO;
            FactLine PLline = null;

            //  Find line biggest BalanceSheet or P&L line
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine l = (FactLine)_lines[i];
                Decimal amt = Math.Abs(l.GetAcctBalance());
                if (l.IsBalanceSheet() && amt.CompareTo(BSamount) > 0)
                {
                    BSamount = amt;
                    BSline = l;
                }
                else if (!l.IsBalanceSheet() && amt.CompareTo(PLamount) > 0)
                {
                    PLamount = amt;
                    PLline = l;
                }
            }

            //  Create Currency Balancing Entry
            if (_acctSchema.IsCurrencyBalancing())
            {
                line = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(), _doc.Get_ID(), 0, _trx);
                line.SetDocumentInfo(_doc, null);
                line.SetPostingType(_postingType);
                line.SetAccount(_acctSchema, _acctSchema.GetCurrencyBalancing_Acct());

                //  Amount
                line.SetAmtSource(_doc.GetC_Currency_ID(), Env.ZERO, Env.ZERO);
                line.Convert();
                //	Accounted
                Decimal drAmt = Env.ZERO;
                Decimal crAmt = Env.ZERO;
                bool isDR = Env.Signum(diff) < 0;
                Decimal difference = Math.Abs(diff);
                if (isDR)
                {
                    drAmt = difference;
                }
                else
                {
                    crAmt = difference;
                }
                //	Switch sides
                bool switchIt = BSline != null && ((BSline.IsDrSourceBalance() && isDR) || (!BSline.IsDrSourceBalance() && !isDR));
                if (switchIt)
                {
                    drAmt = Env.ZERO;
                    crAmt = Env.ZERO;
                    if (isDR)
                    {
                        crAmt = Decimal.Negate(difference);
                    }
                    else
                    {
                        drAmt = Decimal.Negate(difference);
                    }
                }
                line.SetAmtAcct(drAmt, crAmt);
                log.Fine(line.ToString());
                _lines.Add(line);
            }
            else	//  Adjust biggest (Balance Sheet) line amount
            {
                if (BSline != null)
                {
                    line = BSline;
                }
                else
                {
                    line = PLline;
                }
                if (line == null)
                {
                    log.Severe("No Line found");
                }
                else
                {
                    log.Fine("Adjusting Amt=" + diff + "; Line=" + line);
                    line.CurrencyCorrect(diff);
                    log.Fine(line.ToString());
                }
            }   //  correct biggest amount

            return line;
        }

        /// <summary>
        /// Check Accounts of Fact Lines
        /// </summary>
        /// <returns>true if success</returns>
        public bool CheckAccounts()
        {
            //  no lines -> nothing to distribute
            if (_lines.Count == 0)
            {
                return true;
            }

            //	For all fact lines
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine line = (FactLine)_lines[i];
                MAccount account = line.GetAccount();
                if (account == null)
                {
                    log.Warning("No Account for " + line);
                    return false;
                }
                MElementValue ev = account.GetAccount();
                if (ev == null)
                {
                    log.Warning("No Element Value for " + account
                        + ": " + line);
                    return false;
                }
                if (ev.IsSummary())
                {
                    log.Warning("Cannot post to Summary Account " + ev
                        + ": " + line);
                    return false;
                }
                if (!ev.IsActive())
                {
                    log.Warning("Cannot post to Inactive Account " + ev
                        + ": " + line);
                    return false;
                }

            }	//	for all lines

            return true;
        }

        /// <summary>
        /// GL Distribution of Fact Lines
        /// </summary>
        /// <returns>true if success</returns>
        public bool Distribute()
        {
            //  no lines -> nothing to distribute
            if (_lines.Count == 0)
            {
                return true;
            }

            List<FactLine> newLines = new List<FactLine>();
            //	For all fact lines
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine dLine = (FactLine)_lines[i];
                MDistribution[] distributions = MDistribution.Get(dLine.GetAccount(),
                    _postingType, _doc.GetC_DocType_ID());
                //	No Distribution for this line
                if (distributions == null || distributions.Length == 0)
                {
                    continue;
                }
                //	Just the first
                if (distributions.Length > 1)
                {
                    log.Warning("More then one Distributiion for " + dLine.GetAccount());
                }
                MDistribution distribution = distributions[0];
                //	Add Reversal
                FactLine reversal = dLine.Reverse(distribution.GetName());
                log.Info("Reversal=" + reversal);
                newLines.Add(reversal);		//	saved in postCommit
                //	Prepare
                distribution.Distribute(dLine.GetAccount(), dLine.GetSourceBalance(), dLine.GetC_Currency_ID());
                MDistributionLine[] lines = distribution.GetLines(false);
                for (int j = 0; j < lines.Length; j++)
                {
                    MDistributionLine dl = lines[j];
                    if (!dl.IsActive() || Env.Signum(dl.GetAmt()) == 0)
                    {
                        continue;
                    }
                    FactLine factLine = new FactLine(_doc.GetCtx(), _doc.Get_Table_ID(),
                        _doc.Get_ID(), 0, _trx);
                    //  Set Info & Account
                    factLine.SetDocumentInfo(_doc, dLine.GetDocLine());
                    factLine.SetAccount(_acctSchema, dl.GetAccount());
                    factLine.SetPostingType(_postingType);
                    if (dl.IsOverwriteOrg())	//	set Org explicitly
                    {
                        factLine.SetAD_Org_ID(dl.GetOrg_ID());
                    }
                    //
                    if (Env.Signum(dl.GetAmt()) < 0)
                    {
                        factLine.SetAmtSource(dLine.GetC_Currency_ID(), null, Math.Abs(dl.GetAmt()));
                    }
                    else
                    {
                        factLine.SetAmtSource(dLine.GetC_Currency_ID(), dl.GetAmt(), null);
                    }
                    //  Convert
                    factLine.Convert();
                    //
                    String description = distribution.GetName() + " #" + dl.GetLine();
                    if (dl.GetDescription() != null)
                    {
                        description += " - " + dl.GetDescription();
                    }
                    factLine.AddDescription(description);
                    //
                    log.Info(factLine.ToString());
                    newLines.Add(factLine);
                }
            }	//	for all lines

            //	Add Lines
            for (int i = 0; i < newLines.Count; i++)
            {
                _lines.Add(newLines[i]);
            }

            return true;
        }


        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>String</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Fact[");
            sb.Append(_doc.ToString());
            sb.Append(",").Append(_acctSchema.ToString());
            sb.Append(",PostType=").Append(_postingType);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <returns>FactLine Array</returns>
        public FactLine[] GetLines()
        {
            FactLine[] temp = new FactLine[_lines.Count];
            temp = _lines.ToArray();
            return temp;
        }

        /// <summary>
        /// Save Fact
        /// </summary>
        /// <param name="trxName"></param>
        /// <returns>true if all lines were saved</returns>
        public bool Save(Trx trxName)
        {
            //  save Lines
            for (int i = 0; i < _lines.Count; i++)
            {
                FactLine fl = (FactLine)_lines[i];
                //	log.Fine("save - " + fl);
                if (!fl.Save(trxName))  //  abort on first error
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <returns>trx</returns>
        public Trx Get_TrxName()
        {
            return _trx;
        }
        public Trx Get_Trx()
        {
            return _trx;
        }

        /// <summary>
        /// Set Transaction name
        /// </summary>
        /// <param name="trxName"></param>
        /// //@SuppressWarnings("unused")
        private void Set_TrxName(Trx trxName)
        {
            _trx = trxName;
        }

        private void Set_Trx(Trx trx)
        {
            _trx = trx;
        }

        /********************************************************************************
            /// <summary>
            /// Fact Balance Utility
            /// </summary>
         ********************************************************************************/
        public class Balance
        {
            // DR Amount
            public Decimal DR = Env.ZERO;
            // CR Amount
            public Decimal CR = Env.ZERO;

            /// <summary>
            /// New Balance
            /// </summary>
            /// <param name="dr">DR</param>
            /// <param name="cr">CR</param>
            public Balance(Decimal dr, Decimal cr)
            {
                DR = dr;
                CR = cr;
            }

            /// <summary>
            /// Add 
            /// </summary>
            /// <param name="dr"></param>
            /// <param name="cr"></param>
            public void Add(Decimal dr, Decimal cr)
            {
                DR = Decimal.Add(DR, dr);
                CR = Decimal.Add(CR, cr);
            }

            /// <summary>
            /// Get Balance
            /// </summary>
            /// <returns>balance</returns>
            public Decimal GetBalance()
            {
                return Decimal.Subtract(DR, CR);
            }

            /// <summary>
            /// Get Post Balance
            /// </summary>
            /// <returns>absolute balance - negative if reversal</returns>
            public Decimal GetPostBalance()
            {
                Decimal bd = Math.Abs(GetBalance());
                if (IsReversal())
                {
                    return Decimal.Negate(bd);
                }
                return bd;
            }

            /// <summary>
            /// Zero Balance
            /// </summary>
            /// <returns>true if 0</returns>
            public bool IsZeroBalance()
            {
                return Env.Signum(GetBalance()) == 0;
            }

            /// <summary>
            /// 	Reversal
            /// </summary>
            /// <returns>true if both DR/CR are negative or zero</returns>
            public bool IsReversal()
            {
                return Env.Signum(DR) <= 0 && Env.Signum(CR) <= 0;
            }

            /// <summary>
            /// 	String Representation
            /// </summary>
            /// <returns>info</returns>
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder("Balance[");
                sb.Append("DR=").Append(DR)
                    .Append("-CR=").Append(CR)
                    .Append(" = ").Append(GetBalance())
                    .Append("]");
                return sb.ToString();
            }

        }
    }
}
