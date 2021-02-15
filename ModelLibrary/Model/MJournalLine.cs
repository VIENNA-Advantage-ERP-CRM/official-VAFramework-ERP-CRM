/********************************************************
 * Class Name     : MJournalLine
 * Purpose        : Journal Line Model
 * Class Used     : X_VAGL_JRNLLine
 * Chronological    Development
 * Deepak           15-JAN-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MJournalLine : X_VAGL_JRNLLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAGL_JRNLLine_ID">id</param>
        /// <param name="trxName"> transaction</param>
        public MJournalLine(Ctx ctx, int VAGL_JRNLLine_ID, Trx trxName)
            : base(ctx, VAGL_JRNLLine_ID, trxName)
        {
            //super (ctx, VAGL_JRNLLine_ID, trxName);
            if (VAGL_JRNLLine_ID == 0)
            {
                //	setVAGL_JRNLLine_ID (0);		//	PK
                //	setVAGL_JRNL_ID (0);			//	Parent
                //	setVAB_Currency_ID (0);
                //	setVAB_Acct_ValidParameter_ID (0);
                SetLine(0);
                SetAmtAcctCr(Env.ZERO);
                SetAmtAcctDr(Env.ZERO);
                SetAmtSourceCr(Env.ZERO);
                SetAmtSourceDr(Env.ZERO);
                SetCurrencyRate(Env.ONE);
                //	setVAB_CurrencyType_ID (0);
                //SetDateAcct (new Timestamp(System.currentTimeMillis()));
                SetDateAcct(DateTime.Now);
                SetIsGenerated(true);
            }
        }	//	MJournalLine

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MJournalLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MJournalLine

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">journal</param>
        public MJournalLine(MJournal parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this (parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(parent);
            SetVAGL_JRNL_ID(parent.GetVAGL_JRNL_ID());
            SetVAB_Currency_ID(parent.GetVAB_Currency_ID());
            SetVAB_CurrencyType_ID(parent.GetVAB_CurrencyType_ID());
            SetDateAcct(parent.GetDateAcct());

        }	//	MJournalLine

        /**	Currency Precision		*/
        private int m_precision = 2;
        /**	Account Combination		*/
        private MVABAccount m_account = null;
        /** Account Element			*/
        private MVABAcctElement m_accountElement = null;

        /// <summary>
        /// Set Currency Info
        /// </summary>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="VAB_CurrencyType_ID">type</param>
        /// <param name="CurrencyRate">rate</param>
        public void SetCurrency(int VAB_Currency_ID, int VAB_CurrencyType_ID, Decimal? CurrencyRate)
        {
            SetVAB_Currency_ID(VAB_Currency_ID);
            if (VAB_CurrencyType_ID != 0)
            {
                SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
            }
            //if (CurrencyRate != null && CurrencyRate.signum() == 0)
            if (CurrencyRate != null && Env.Signum(CurrencyRate.Value) == 0)
            {
                SetCurrencyRate(CurrencyRate);
            }
        }	//	setCurrency

        /// <summary>
        /// Set VAB_Currency_ID and precision
        /// </summary>
        /// <param name="VAB_Currency_ID">currency</param>
        public new void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID == 0)
            {
                return;
            }
            base.SetVAB_Currency_ID(VAB_Currency_ID);
            m_precision = MVABCurrency.GetStdPrecision(GetCtx(), VAB_Currency_ID);
        }	//	setVAB_Currency_ID

        /// <summary>
        ///	Get Currency Precision
        /// </summary>
        /// <returns></returns>
        public int GetPrecision()
        {
            return m_precision;
        }	//	getPrecision

        /// <summary>
        /// Set Currency Rate
        /// </summary>
        /// <param name="CurrencyRate">CurrencyRate check for null (->one)</param>
        public new void SetCurrencyRate(Decimal? CurrencyRate)
        {
            if (CurrencyRate == null)
            {
                log.Warning("was NULL - set to 1");
                base.SetCurrencyRate(Env.ONE);
            }
            //else if (CurrencyRate.signum() < 0)
            else if (Env.Signum(CurrencyRate.Value) < 0)
            {
                log.Warning("negative - " + CurrencyRate + " - set to 1");
                base.SetCurrencyRate(Env.ONE);
            }
            else
            {
                base.SetCurrencyRate(CurrencyRate);
            }
        }	//	setCurrencyRate


        /// <summary>
        /// Set Rate - Callout.
        /// </summary>
        /// <param name="oldVAB_CurrencyType_ID">old</param>
        /// <param name="newVAB_CurrencyType_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetVAB_CurrencyType_ID(String oldVAB_CurrencyType_ID,
               String newVAB_CurrencyType_ID, int windowNo)
        {
            if (newVAB_CurrencyType_ID == null || newVAB_CurrencyType_ID.Length == 0)
            {
                return;
            }
            int? VAB_CurrencyType_ID = Utility.Util.GetValueOfInt(newVAB_CurrencyType_ID);
            if (VAB_CurrencyType_ID == 0)
            {
                return;
            }
            SetVAB_CurrencyType_ID(VAB_CurrencyType_ID.Value);
            SetRate(windowNo);
        }	//	setVAB_CurrencyType_ID

        /// <summary>
        /// Set Currency - Callout.
        /// </summary>
        /// <param name="oldVAB_Currency_ID">old</param>
        /// <param name="newVAB_Currency_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetVAB_Currency_ID(String oldVAB_Currency_ID,
               String newVAB_Currency_ID, int windowNo)
        {
            if (newVAB_Currency_ID == null || newVAB_Currency_ID.Length == 0)
            {
                return;
            }
            int? VAB_Currency_ID = Utility.Util.GetValueOfInt(newVAB_Currency_ID);
            if (VAB_Currency_ID == 0)
            {
                return;
            }
            SetVAB_Currency_ID(VAB_Currency_ID.Value);
            SetRate(windowNo);
        }	//	setVAB_Currency_ID

        /// <summary>
        /// set rate
        /// </summary>
        /// <param name="windowNo"></param>
        private void SetRate(int windowNo)
        {
            //  Source info
            int? VAB_Currency_ID = GetVAB_Currency_ID();
            int? VAB_CurrencyType_ID = GetVAB_CurrencyType_ID();
            if (VAB_Currency_ID == 0 || VAB_CurrencyType_ID == 0)
            {
                return;
            }
            DateTime? DateAcct = GetDateAcct();
            if (DateAcct == null)
            {
                DateAcct = DateTime.Now;// new Timestamp(System.currentTimeMillis());
            }
            //
            int? VAB_AccountBook_ID = GetCtx().GetContextAsInt(windowNo, "VAB_AccountBook_ID");
            MVABAccountBook ass = MVABAccountBook.Get(GetCtx(), VAB_AccountBook_ID.Value);
            int? VAF_Client_ID = GetVAF_Client_ID();
            int? VAF_Org_ID = GetVAF_Org_ID();

            Decimal? CurrencyRate = (Decimal?)MVABExchangeRate.GetRate(VAB_Currency_ID.Value, ass.GetVAB_Currency_ID(),
                DateAcct, VAB_CurrencyType_ID.Value, VAF_Client_ID.Value, VAF_Org_ID.Value);
            log.Fine("rate = " + CurrencyRate);
            if (CurrencyRate == null)
            {
                CurrencyRate = Env.ZERO;
            }
            SetCurrencyRate(CurrencyRate);
            SetAmt(windowNo);
        }	//	setRate


        /// <summary>
        /// Set Accounted Amounts only if not 0.Amounts overwritten in beforeSave - set conversion rate
        /// </summary>
        /// <param name="AmtAcctDr">dr</param>
        /// <param name="AmtAcctCr">cr</param>
        public void SetAmtAcct(Decimal AmtAcctDr, Decimal AmtAcctCr)
        {
            //	setConversion
            Double? rateDR = 0;
            if (Env.Signum(AmtAcctDr) != 0)
            {
                rateDR = Utility.Util.GetValueOfDouble(AmtAcctDr) / Utility.Util.GetValueOfDouble(GetAmtSourceDr());
                base.SetAmtAcctDr(AmtAcctDr);

            }
            Double? rateCR = 0;
            if (Env.Signum(AmtAcctCr) != 0)
            {
                rateCR = Utility.Util.GetValueOfDouble(AmtAcctCr) / Utility.Util.GetValueOfDouble(GetAmtSourceCr());
                base.SetAmtAcctCr(AmtAcctCr);
            }
            if (rateDR != 0 && rateCR != 0 && rateDR != rateCR)
            {
                log.Warning("Rates Different DR=" + rateDR + "(used) <> CR=" + rateCR + "(ignored)");
                rateCR = 0;
            }
            if (rateDR < 0 || Double.IsInfinity(rateDR.Value) || Double.IsNaN(rateDR.Value))
            {
                log.Warning("DR Rate ignored - " + rateDR);
                return;
            }
            if (rateCR < 0 || Double.IsInfinity(rateCR.Value) || Double.IsNaN(rateCR.Value))
            {
                log.Warning("CR Rate ignored - " + rateCR);
                return;
            }

            if (rateDR != 0)
            {
                SetCurrencyRate(Utility.Util.GetValueOfDecimal(rateDR));
            }
            if (rateCR != 0)
            {
                SetCurrencyRate(Utility.Util.GetValueOfDecimal(rateCR));
            }
        }	//	setAmtAcct

        /// <summary>
        /// Set AmtSourceCr - Callout
        /// </summary>
        /// <param name="oldAmtSourceCr">old value</param>
        /// <param name="newAmtSourceCr">new value</param>
        /// <param name="windowNo">window no</param>
        public void SetAmtSourceCr(String oldAmtSourceCr,
               String newAmtSourceCr, int windowNo)
        {
            if (newAmtSourceCr == null || newAmtSourceCr.Length == 0)
            {
                return;
            }
            Decimal AmtSourceCr = Utility.Util.GetValueOfDecimal(newAmtSourceCr);
            base.SetAmtSourceCr(AmtSourceCr);
            SetAmt(windowNo);
        }	//	SetAmtSourceCr

        /// <summary>
        ///	Set AmtSourceDr - Callout
        /// </summary>
        /// <param name="oldAmtSourceDr">old</param>
        /// <param name="newAmtSourceDr">new</param>
        /// <param name="windowNo">window no</param>
        public void SetAmtSourceDr(String oldAmtSourceDr,
               String newAmtSourceDr, int windowNo)
        {
            if (newAmtSourceDr == null || newAmtSourceDr.Length == 0)
            {
                return;
            }
            Decimal? AmtSourceDr = Utility.Util.GetValueOfDecimal(newAmtSourceDr);
            base.SetAmtSourceDr(AmtSourceDr);
            SetAmt(windowNo);
        }	//	setAmtSourceDr

        /// <summary>
        /// Set CurrencyRate - Callout
        /// </summary>
        /// <param name="oldCurrencyRate">old</param>
        /// <param name="newCurrencyRate">new</param>
        /// <param name="windowNo">window no</param>
        public void SetCurrencyRate(String oldCurrencyRate,
               String newCurrencyRate, int windowNo)
        {
            if (newCurrencyRate == null || newCurrencyRate.Length == 0)
            {
                return;
            }
            Decimal? CurrencyRate = Utility.Util.GetValueOfDecimal(newCurrencyRate);
            base.SetCurrencyRate(CurrencyRate);
            SetAmt(windowNo);
        }	//	setCurrencyRate


        /// <summary>
        /// 	Set Accounted Amounts
        /// </summary>
        /// <param name="windowNo">window no</param>
        private void SetAmt(int windowNo)
        {
            //  Get Target Currency & Precision from VAB_AccountBook.VAB_Currency_ID
            int? VAB_AccountBook_ID = GetCtx().GetContextAsInt(windowNo, "VAB_AccountBook_ID");
            MVABAccountBook ass = MVABAccountBook.Get(GetCtx(), VAB_AccountBook_ID.Value);
            int? Precision = ass.GetStdPrecision();

            Decimal? CurrencyRate = GetCurrencyRate();
            if (CurrencyRate == null)
            {
                CurrencyRate = Env.ONE;
                SetCurrencyRate(CurrencyRate);
            }

            //  AmtAcct = AmtSource * CurrencyRate  ==> Precision
            Decimal? AmtSourceDr = GetAmtSourceDr();
            if (AmtSourceDr == null)
            {
                AmtSourceDr = Env.ZERO;
            }
            Decimal? AmtSourceCr = GetAmtSourceCr();
            if (AmtSourceCr == null)
            {
                AmtSourceCr = Env.ZERO;
            }

            Decimal? AmtAcctDr = (Decimal.Multiply(AmtSourceDr.Value, CurrencyRate.Value));
            //AmtAcctDr = AmtAcctDr.setScale(Precision, BigDecimal.ROUND_HALF_UP);
            AmtAcctDr = Decimal.Round(AmtAcctDr.Value, Precision.Value, MidpointRounding.AwayFromZero);
            SetAmtAcctDr(AmtAcctDr);
            Decimal? AmtAcctCr = Decimal.Multiply(AmtSourceCr.Value, CurrencyRate.Value);
            AmtAcctCr = Decimal.Round(AmtAcctCr.Value, Precision.Value, MidpointRounding.AwayFromZero);
            SetAmtAcctCr(AmtAcctCr);
        }	//	setAmt


        /// <summary>
        /// Set VAB_Acct_ValidParameter_ID
        /// </summary>
        /// <param name="VAB_Acct_ValidParameter_ID">id</param>
        public new void SetVAB_Acct_ValidParameter_ID(int VAB_Acct_ValidParameter_ID)
        {
            base.SetVAB_Acct_ValidParameter_ID(VAB_Acct_ValidParameter_ID);
            m_account = null;
            m_accountElement = null;
        }	//	setVAB_Acct_ValidParameter_ID

        /// <summary>
        ///	Set VAB_Acct_ValidParameter_ID
        /// </summary>
        /// <param name="acct">account</param>
        public void SetVAB_Acct_ValidParameter_ID(MVABAccount acct)
        {
            if (acct == null)
            {
                throw new ArgumentException("Account is null");
            }
            base.SetVAB_Acct_ValidParameter_ID(acct.GetVAB_Acct_ValidParameter_ID());
            m_account = acct;
            m_accountElement = null;
        }	//	setVAB_Acct_ValidParameter_ID

        /// <summary>
        /// Get Account (Valid Combination)
        /// </summary>
        /// <returns> combination or null</returns>
        public MVABAccount GetAccount()
        {
            if (m_account == null && GetVAB_Acct_ValidParameter_ID() != 0)
                m_account = new MVABAccount(GetCtx(), GetVAB_Acct_ValidParameter_ID(), Get_TrxName());
            return m_account;
        }	//	getValidCombination

        /// <summary>
        ///	Get Natural Account Element Value
        /// </summary>
        /// <returns> account</returns>
        public MVABAcctElement GetAccountElementValue()
        {
            if (m_accountElement == null)
            {
                MVABAccount vc = GetAccount();
                if (vc != null && vc.GetAccount_ID() != 0)
                {
                    m_accountElement = new MVABAcctElement(GetCtx(), vc.GetAccount_ID(), Get_TrxName());
                }
            }
            return m_accountElement;
        }	//	getAccountElement

        /// <summary>
        /// Is it posting to a Control Acct
        /// </summary>
        /// <returns> true if control acct</returns>
        public Boolean IsDocControlled()
        {
            MVABAcctElement acct = GetAccountElementValue();
            if (acct == null)
            {
                log.Warning("Account not found for VAB_Acct_ValidParameter_ID=" + GetVAB_Acct_ValidParameter_ID());
                return false;
            }
            return acct.IsDocControlled();
        }	//	isDocControlled


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Acct Amts
            Decimal? rate = GetCurrencyRate();

            // set precision value based on cuurenncy define on GL Journal
            m_precision = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT stdprecision FROM VAB_Currency WHERE VAB_Currency_id = 
                            ( SELECT VAB_Currency_id FROM VAGL_JRNL WHERE VAGL_JRNL_ID=" + GetVAGL_JRNL_ID() + " )", null, Get_Trx()));

            Decimal valuesRate = 0;
            Decimal valuesRateCredit = 0;
            if (rate == 0)
            {
                valuesRate = 1;
            }
            else
            {
                valuesRate = rate.Value;
            }

            Decimal? amt = Decimal.Multiply(valuesRate, GetAmtSourceDr());
            //            Decimal? amt = Decimal.Multiply(rate.Value, GetAmtSourceDr());
            if (Env.Scale(amt.Value) > GetPrecision())
            {
                amt = Decimal.Round(amt.Value, GetPrecision(), MidpointRounding.AwayFromZero);
            }
            SetAmtAcctDr(amt.Value);

            if (rate == 0)
            {
                valuesRateCredit = 1;
            }
            else
            {
                valuesRateCredit = rate.Value;
            }

            amt = Decimal.Multiply(valuesRateCredit, GetAmtSourceCr());
            //            amt = Decimal.Multiply(rate.Value, GetAmtSourceCr());

            if (Env.Scale(amt.Value) > GetPrecision())
            {
                amt = Decimal.Round(amt.Value, GetPrecision(), MidpointRounding.AwayFromZero);
            }
            SetAmtAcctCr(amt.Value);

            // Added by Bharat on 18 July 2018 to Generate Account Combination as discussed with Mukesh Sir.
            GetOrCreateCombination(newRecord);

            //	Set Line Org to Acct Org
            if (newRecord
                || Is_ValueChanged("VAB_Acct_ValidParameter_ID")
                || Is_ValueChanged("VAF_Org_ID"))
            {
                SetVAF_Org_ID(GetAccount().GetVAF_Org_ID());
            }

            //18/7/2016
            //MAnish before save journal line. if journal line has some dimention and user update debit or credit field,, first chk is there dimention or not.
            string sql = "Select Count(*) FROM VAGL_LineDimension WHERE VAGL_JRNLLine_ID=" + Get_Value("VAGL_JRNLLine_ID");
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            if (!newRecord && Is_ValueChanged("ElementType") && count > 0)
            {
                log.SaveWarning("DeleteDimention", "");
                log.SaveError("DeleteDimention", "");
                return false;
            }

            if (!newRecord && (Is_ValueChanged("AmtSourceDr") || Is_ValueChanged("AmtSourceCr")))
            {
                string sqlQury = "SELECT SUM(amount) FROM VAGL_LineDimension WHERE VAGL_JRNLLine_ID=" + Get_Value("VAGL_JRNLLine_ID");
                int countQuery = Util.GetValueOfInt(DB.ExecuteScalar(sqlQury));

                if (Is_ValueChanged("AmtSourceDr"))
                {
                    var drAmount = GetAmtSourceDr();
                    if (drAmount < countQuery)
                    {
                        log.SaveWarning("ValueNotBeLessThen", "");
                        log.SaveError("ValueNotBeLessThen", "");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (Is_ValueChanged("AmtSourceCr"))
                {
                    var drAmount = GetAmtSourceCr();
                    if (drAmount < countQuery)
                    {
                        log.SaveWarning("ValueNotBeLessThen", "");
                        log.SaveError("ValueNotBeLessThen", "");
                        return false;
                    }
                }
            }
            //end 18/7/2016.            
            return true;
        }	//	beforeSave

        /// <summary>
        /// After Save.	Update Journal/Batch Total
        /// </summary>
        /// <param name="newRecord">true if new record</param>
        /// <param name="success"> true if success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
            {
                return success;
            }
            return UpdateJournalTotal();
        }	//	afterSave


        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">true if deleted</param>
        /// <returns>true if success</returns>
        protected override Boolean AfterDelete(Boolean success)
        {
            if (!success)
            {
                return success;
            }
            return UpdateJournalTotal();
        }	//	afterDelete


        /** Update combination and optionally **/
        private bool GetOrCreateCombination(Boolean newRecord)
        {
            int Account_ID = 0, VAB_SubAcct_ID = 0, VAM_Product_ID = 0, VAB_BusinessPartner_ID = 0, VAF_Org_ID = 0, VAF_OrgTrx_ID = 0,
                C_LocFrom_ID = 0, C_LocTo_ID = 0, VAB_SalesRegionState_ID = 0, VAB_Project_ID = 0, VAB_Promotion_ID = 0,
                VAB_BillingCode_ID = 0, User1_ID = 0, User2_ID = 0;

            if (GetVAB_Acct_ValidParameter_ID() == 0
                    || (!newRecord && (Is_ValueChanged("Account_ID")
                            || Is_ValueChanged("VAM_Product_ID")
                            || Is_ValueChanged("VAB_BusinessPartner_ID")
                            || Is_ValueChanged("VAF_Org_ID")
                            || Is_ValueChanged("VAB_Project_ID")
                            || Is_ValueChanged("VAB_Promotion_ID")
                            || Is_ValueChanged("VAB_BillingCode_ID"))))
            {
                MJournal gl = new MJournal(GetCtx(), GetVAGL_JRNL_ID(), Get_TrxName());

                // Validate all mandatory combinations are set
                MVABAccountBook asc = MVABAccountBook.Get(GetCtx(), gl.GetVAB_AccountBook_ID());
                MVABAccountBookElement[] elements = MVABAccountBookElement.GetAcctSchemaElements(asc);
                for (int i = 0; i < elements.Length; i++)
                {
                    MVABAccountBookElement elem = elements[i];
                    String et = elem.GetElementType();
                    if (MVABAccountBookElement.ELEMENTTYPE_Account.Equals(et) && Get_ColumnIndex("Account_ID") >= 0)
                        Account_ID = Util.GetValueOfInt(Get_Value("Account_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Account.Equals(et) && Get_ColumnIndex("VAB_SubAcct_ID") > 0)
                        VAB_SubAcct_ID = Util.GetValueOfInt(Get_Value("VAB_SubAcct_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Activity.Equals(et) && Get_ColumnIndex("VAB_BillingCode_ID") > 0)
                        VAB_BillingCode_ID = Util.GetValueOfInt(Get_Value("VAB_BillingCode_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_BPartner.Equals(et) && Get_ColumnIndex("VAB_BusinessPartner_ID") > 0)
                        VAB_BusinessPartner_ID = Util.GetValueOfInt(Get_Value("VAB_BusinessPartner_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Campaign.Equals(et) && Get_ColumnIndex("VAB_BusinessPartner_ID") > 0)
                        VAB_BusinessPartner_ID = Util.GetValueOfInt(Get_Value("VAB_BusinessPartner_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Organization.Equals(et))
                        VAF_Org_ID = GetVAF_Org_ID();
                    if (MVABAccountBookElement.ELEMENTTYPE_OrgTrx.Equals(et) && Get_ColumnIndex("VAF_OrgTrx_ID") > 0)
                        VAF_OrgTrx_ID = Util.GetValueOfInt(Get_Value("VAF_OrgTrx_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("C_LocFrom_ID") > 0)
                        C_LocFrom_ID = Util.GetValueOfInt(Get_Value("C_LocFrom_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("C_LocTo_ID") > 0)
                        C_LocTo_ID = Util.GetValueOfInt(Get_Value("C_LocTo_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("VAM_Product_ID") > 0)
                        VAM_Product_ID = Util.GetValueOfInt(Get_Value("VAM_Product_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Project.Equals(et) && Get_ColumnIndex("VAB_Project_ID") > 0)
                        VAB_Project_ID = Util.GetValueOfInt(Get_Value("VAB_Project_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_Project.Equals(et) && Get_ColumnIndex("VAB_Promotion_ID") > 0)
                        VAB_Promotion_ID = Util.GetValueOfInt(Get_Value("VAB_Promotion_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_SalesRegion.Equals(et) && Get_ColumnIndex("VAB_SalesRegionState_ID") > 0)
                        VAB_SalesRegionState_ID = Util.GetValueOfInt(Get_Value("VAB_SalesRegionState_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_UserList1.Equals(et) && Get_ColumnIndex("User1_ID") > 0)
                        User1_ID = Util.GetValueOfInt(Get_Value("User1_ID"));
                    if (MVABAccountBookElement.ELEMENTTYPE_UserList2.Equals(et) && Get_ColumnIndex("User2_ID") > 0)
                        User2_ID = Util.GetValueOfInt(Get_Value("User2_ID"));
                }

                MVABAccount acct = MVABAccount.Get(GetCtx(), GetVAF_Client_ID(), VAF_Org_ID, gl.GetVAB_AccountBook_ID(), Account_ID,
                        VAB_SubAcct_ID, VAM_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID, C_LocTo_ID, VAB_SalesRegionState_ID,
                        VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID, User1_ID, User2_ID, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                if (acct != null)
                {
                    acct.Save(Get_TrxName());	// get ID from transaction
                    SetVAB_Acct_ValidParameter_ID(acct.Get_ID());

                    //if (acct.GetAlias() != null && acct.GetAlias().length > 0)
                    //    setAlias_ValidCombination_ID(acct.get_ID());
                    //else
                    //    setAlias_ValidCombination_ID(0);
                }
            }
            else
            {
                fillDimensionsFromCombination();                
               
            }
            return true;
        }

        /** Fill Accounting Dimensions from line combination **/
        private void fillDimensionsFromCombination()
        {
            if (GetVAB_Acct_ValidParameter_ID() > 0)
            {
                MVABAccount combi = new MVABAccount(GetCtx(), GetVAB_Acct_ValidParameter_ID(), Get_TrxName());
                if (Get_ColumnIndex("Account_ID") > 0)
                    Set_Value("Account_ID", combi.GetAccount_ID() > 0 ? combi.GetAccount_ID() : 0);
                if (Get_ColumnIndex("VAB_SubAcct_ID") > 0)
                    Set_Value("VAB_SubAcct_ID", combi.GetVAB_SubAcct_ID() > 0 ? combi.GetVAB_SubAcct_ID() : 0);
                // setting null in business partner and product search control because if set 0 then it shows <0> in controls.-Mohit-11 May 2020
                if (Get_ColumnIndex("VAM_Product_ID") > 0)
                {
                    if (combi.GetVAM_Product_ID() > 0)
                        Set_Value("VAM_Product_ID", combi.GetVAM_Product_ID());
                }
                if (Get_ColumnIndex("VAB_BusinessPartner_ID") > 0)
                {
                    if (combi.GetVAB_BusinessPartner_ID() > 0)
                        Set_Value("VAB_BusinessPartner_ID", combi.GetVAB_BusinessPartner_ID());
                }
                if (Get_ColumnIndex("VAF_OrgTrx_ID") > 0)
                    Set_Value("VAF_OrgTrx_ID", combi.GetVAF_OrgTrx_ID() > 0 ? combi.GetVAF_OrgTrx_ID() : 0);
                if (Get_ColumnIndex("VAF_Org_ID") > 0)
                    Set_Value("VAF_Org_ID", combi.GetVAF_Org_ID() > 0 ? combi.GetVAF_Org_ID() : 0);
                if (Get_ColumnIndex("C_LocFrom_ID") > 0)
                    Set_Value("C_LocFrom_ID", combi.GetC_LocFrom_ID() > 0 ? combi.GetC_LocFrom_ID() : 0);
                if (Get_ColumnIndex("C_LocTo_ID") > 0)
                    Set_Value("C_LocTo_ID", combi.GetC_LocTo_ID() > 0 ? combi.GetC_LocTo_ID() : 0);
                if (Get_ColumnIndex("VAB_SalesRegionState_ID") > 0)
                    Set_Value("VAB_SalesRegionState_ID", combi.GetVAB_SalesRegionState_ID() > 0 ? combi.GetVAB_SalesRegionState_ID() : 0);
                if (Get_ColumnIndex("VAB_Project_ID") > 0)
                    Set_Value("VAB_Project_ID", combi.GetVAB_Project_ID() > 0 ? combi.GetVAB_Project_ID() : 0);
                if (Get_ColumnIndex("VAB_Promotion_ID") > 0)
                    Set_Value("VAB_Promotion_ID", combi.GetVAB_Promotion_ID() > 0 ? combi.GetVAB_Promotion_ID() : 0);
                if (Get_ColumnIndex("VAB_BillingCode_ID") > 0)
                    Set_Value("VAB_BillingCode_ID", combi.GetVAB_BillingCode_ID() > 0 ? combi.GetVAB_BillingCode_ID() : 0);
                if (Get_ColumnIndex("User1_ID") > 0)
                    Set_Value("User1_ID", combi.GetUser1_ID() > 0 ? combi.GetUser1_ID() : 0);
                if (Get_ColumnIndex("User2_ID") > 0)
                    Set_Value("User2_ID", combi.GetUser2_ID() > 0 ? combi.GetUser2_ID() : 0);
            }
        }

        /// <summary>
        ///	Update Journal and Batch Total
        /// </summary>
        /// <returns>true if success</returns>
        private Boolean UpdateJournalTotal()
        {
            //	Update Journal Total
            String sql = "UPDATE VAGL_JRNL j"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(AmtAcctDr), SUM(AmtAcctCr)" //jz ", "
                    + " FROM VAGL_JRNLLine jl WHERE jl.IsActive='Y' AND j.VAGL_JRNL_ID=jl.VAGL_JRNL_ID) "
                + "WHERE VAGL_JRNL_ID=" + GetVAGL_JRNL_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("afterSave - Update Journal #" + no);
            }

            // Manish 18/7/2016 .. chk is VAGL_BatchJRNL_id there or not.
            string sqlquery = @"SELECT VAGL_BatchJRNL_id FROM VAGL_JRNL WHERE VAGL_JRNL_id IN
                                ( SELECT VAGL_JRNL_id FROM VAGL_JRNLLine WHERE VAGL_JRNLLine_ID=" + GetVAGL_JRNLLine_ID() + " )";

            int nooo = Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sqlquery, null, null));
            if (nooo <= 0)
            {
                return no == 1;
            }

            //	Update Batch Total
            sql = "UPDATE VAGL_BatchJRNL jb"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(TotalDr), SUM(TotalCr)" //jz hard coded ", "
                    + " FROM VAGL_JRNL j WHERE jb.VAGL_BatchJRNL_ID=j.VAGL_BatchJRNL_ID) "
                + "WHERE VAGL_BatchJRNL_ID="
                    + "(SELECT DISTINCT VAGL_BatchJRNL_ID FROM VAGL_JRNL WHERE VAGL_JRNL_ID="
                    + GetVAGL_JRNL_ID() + ")";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("Update Batch #" + no);
            }
            return no == 1;
        }   //	updateJournalTotal



        /// <summary>
        /// This functionis used to create Line Dimension Line
        /// </summary>
        /// <param name="fromJournal">object of old GL Journal Line from where we need to pick lines</param>
        /// <param name="newlineID">GL Journal Line ID - new record agsint which we copy lines</param>
        /// <param name="typeCR">Optional Parameter - When 'C' - its means Reverse record - amount to be negate in this case</param>
        /// <returns>count of line - which is created</returns>
        public int CopyLinesFrom(MJournalLine fromJournal, int newlineID, char typeCR = 'X')
        {
            if (IsProcessed() || fromJournal == null)
            {
                return 0;
            }
            int count = 0;
            int lineCount = 0;
            MLineDimension[] fromLines = fromJournal.GetLines(false);
            for (int i = 0; i < fromLines.Length; i++)
            {
                MLineDimension toLine = new MLineDimension(GetCtx(), 0, fromJournal.Get_TrxName());
                PO.CopyValues(fromLines[i], toLine, GetVAF_Client_ID(), GetVAF_Org_ID());

                toLine.SetVAB_BusinessPartner_ID(fromLines[i].GetVAB_BusinessPartner_ID());
                toLine.SetVAB_Promotion_ID(fromLines[i].GetVAB_Promotion_ID());
                toLine.SetVAB_Acct_Element_ID(fromLines[i].GetVAB_Acct_Element_ID());
                toLine.SetVAB_Element_ID(fromLines[i].GetVAB_Element_ID());
                toLine.SetVAB_Address_ID(fromLines[i].GetVAB_Address_ID());
                toLine.SetVAB_Project_ID(fromLines[i].GetVAB_Project_ID());
                toLine.SetVAB_SalesRegionState_ID(fromLines[i].GetVAB_SalesRegionState_ID());
                toLine.SetVAM_Product_ID(fromLines[i].GetVAM_Product_ID());
                toLine.SetOrg_ID(fromLines[i].GetOrg_ID());
                toLine.SetSeqNo(fromLines[i].GetSeqNo());
                toLine.SetLineType(fromLines[i].GetLineType());
                toLine.SetVAGL_JRNLLine_ID(newlineID);

                if (typeCR == 'C')
                {
                    toLine.SetAmount(Decimal.Negate(fromLines[i].GetAmount()));
                }
                else
                {
                    toLine.SetAmount(fromLines[i].GetAmount());
                }
                toLine.SetLine(fromLines[i].GetLine());

                if (toLine.Save(fromJournal.Get_TrxName()))
                {
                    count++;
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - LinesDimensionl=" + fromLines.Length + " <> Saved=" + count);
            }
            return lineCount;
        }





        public MLineDimension[] GetLines(Boolean requery)
        {
            //ArrayList<MJournalLine> list = new ArrayList<MJournalLine>();
            List<MLineDimension> list = new List<MLineDimension>();
            String sql = "SELECT * FROM VAGL_LineDimension WHERE VAGL_JRNLLine_ID=@Param1 ORDER BY Line";
            //PreparedStatement pstmt = null;
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, GetVAGL_JRNLLine_ID());
                Param[0] = new SqlParameter("@Param1", GetVAGL_JRNLLine_ID());

                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                //while (rs.next())
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MLineDimension(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                log.Log(Level.SEVERE, "getLines", ex);
            }
            //
            MLineDimension[] retValue = new MLineDimension[list.Count];
            //list.toArray(retValue);
            retValue = list.ToArray();
            return retValue;
        }	//	getLines

        // end 18/7/2016






    }	//	MJournalLine

}
