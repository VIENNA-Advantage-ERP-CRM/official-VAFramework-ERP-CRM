/********************************************************
 * Class Name     : MJournalLine
 * Purpose        : Journal Line Model
 * Class Used     : X_GL_JournalLine
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
    public class MJournalLine : X_GL_JournalLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_JournalLine_ID">id</param>
        /// <param name="trxName"> transaction</param>
        public MJournalLine(Ctx ctx, int GL_JournalLine_ID, Trx trxName)
            : base(ctx, GL_JournalLine_ID, trxName)
        {
            //super (ctx, GL_JournalLine_ID, trxName);
            if (GL_JournalLine_ID == 0)
            {
                //	setGL_JournalLine_ID (0);		//	PK
                //	setGL_Journal_ID (0);			//	Parent
                //	setVAB_Currency_ID (0);
                //	setC_ValidCombination_ID (0);
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
            SetGL_Journal_ID(parent.GetGL_Journal_ID());
            SetVAB_Currency_ID(parent.GetVAB_Currency_ID());
            SetVAB_CurrencyType_ID(parent.GetVAB_CurrencyType_ID());
            SetDateAcct(parent.GetDateAcct());

        }	//	MJournalLine

        /**	Currency Precision		*/
        private int m_precision = 2;
        /**	Account Combination		*/
        private MAccount m_account = null;
        /** Account Element			*/
        private MElementValue m_accountElement = null;

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
            m_precision = MCurrency.GetStdPrecision(GetCtx(), VAB_Currency_ID);
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
            MAcctSchema ass = MAcctSchema.Get(GetCtx(), VAB_AccountBook_ID.Value);
            int? VAF_Client_ID = GetVAF_Client_ID();
            int? VAF_Org_ID = GetVAF_Org_ID();

            Decimal? CurrencyRate = (Decimal?)MConversionRate.GetRate(VAB_Currency_ID.Value, ass.GetVAB_Currency_ID(),
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
            MAcctSchema ass = MAcctSchema.Get(GetCtx(), VAB_AccountBook_ID.Value);
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
        /// Set C_ValidCombination_ID
        /// </summary>
        /// <param name="C_ValidCombination_ID">id</param>
        public new void SetC_ValidCombination_ID(int C_ValidCombination_ID)
        {
            base.SetC_ValidCombination_ID(C_ValidCombination_ID);
            m_account = null;
            m_accountElement = null;
        }	//	setC_ValidCombination_ID

        /// <summary>
        ///	Set C_ValidCombination_ID
        /// </summary>
        /// <param name="acct">account</param>
        public void SetC_ValidCombination_ID(MAccount acct)
        {
            if (acct == null)
            {
                throw new ArgumentException("Account is null");
            }
            base.SetC_ValidCombination_ID(acct.GetC_ValidCombination_ID());
            m_account = acct;
            m_accountElement = null;
        }	//	setC_ValidCombination_ID

        /// <summary>
        /// Get Account (Valid Combination)
        /// </summary>
        /// <returns> combination or null</returns>
        public MAccount GetAccount()
        {
            if (m_account == null && GetC_ValidCombination_ID() != 0)
                m_account = new MAccount(GetCtx(), GetC_ValidCombination_ID(), Get_TrxName());
            return m_account;
        }	//	getValidCombination

        /// <summary>
        ///	Get Natural Account Element Value
        /// </summary>
        /// <returns> account</returns>
        public MElementValue GetAccountElementValue()
        {
            if (m_accountElement == null)
            {
                MAccount vc = GetAccount();
                if (vc != null && vc.GetAccount_ID() != 0)
                {
                    m_accountElement = new MElementValue(GetCtx(), vc.GetAccount_ID(), Get_TrxName());
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
            MElementValue acct = GetAccountElementValue();
            if (acct == null)
            {
                log.Warning("Account not found for C_ValidCombination_ID=" + GetC_ValidCombination_ID());
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
                            ( SELECT VAB_Currency_id FROM GL_journal WHERE GL_Journal_ID=" + GetGL_Journal_ID() + " )", null, Get_Trx()));

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
                || Is_ValueChanged("C_ValidCombination_ID")
                || Is_ValueChanged("VAF_Org_ID"))
            {
                SetVAF_Org_ID(GetAccount().GetVAF_Org_ID());
            }

            //18/7/2016
            //MAnish before save journal line. if journal line has some dimention and user update debit or credit field,, first chk is there dimention or not.
            string sql = "Select Count(*) FROM GL_LineDimension WHERE GL_JournalLine_ID=" + Get_Value("GL_JournalLine_ID");
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            if (!newRecord && Is_ValueChanged("ElementType") && count > 0)
            {
                log.SaveWarning("DeleteDimention", "");
                log.SaveError("DeleteDimention", "");
                return false;
            }

            if (!newRecord && (Is_ValueChanged("AmtSourceDr") || Is_ValueChanged("AmtSourceCr")))
            {
                string sqlQury = "SELECT SUM(amount) FROM GL_LineDimension WHERE GL_JournalLine_ID=" + Get_Value("GL_JournalLine_ID");
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
            int Account_ID = 0, C_SubAcct_ID = 0, M_Product_ID = 0, VAB_BusinessPartner_ID = 0, VAF_Org_ID = 0, VAF_OrgTrx_ID = 0,
                C_LocFrom_ID = 0, C_LocTo_ID = 0, C_SalesRegion_ID = 0, C_Project_ID = 0, VAB_Promotion_ID = 0,
                VAB_BillingCode_ID = 0, User1_ID = 0, User2_ID = 0;

            if (GetC_ValidCombination_ID() == 0
                    || (!newRecord && (Is_ValueChanged("Account_ID")
                            || Is_ValueChanged("M_Product_ID")
                            || Is_ValueChanged("VAB_BusinessPartner_ID")
                            || Is_ValueChanged("VAF_Org_ID")
                            || Is_ValueChanged("C_Project_ID")
                            || Is_ValueChanged("VAB_Promotion_ID")
                            || Is_ValueChanged("VAB_BillingCode_ID"))))
            {
                MJournal gl = new MJournal(GetCtx(), GetGL_Journal_ID(), Get_TrxName());

                // Validate all mandatory combinations are set
                MAcctSchema asc = MAcctSchema.Get(GetCtx(), gl.GetVAB_AccountBook_ID());
                MAcctSchemaElement[] elements = MAcctSchemaElement.GetAcctSchemaElements(asc);
                for (int i = 0; i < elements.Length; i++)
                {
                    MAcctSchemaElement elem = elements[i];
                    String et = elem.GetElementType();
                    if (MAcctSchemaElement.ELEMENTTYPE_Account.Equals(et) && Get_ColumnIndex("Account_ID") >= 0)
                        Account_ID = Util.GetValueOfInt(Get_Value("Account_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Account.Equals(et) && Get_ColumnIndex("C_SubAcct_ID") > 0)
                        C_SubAcct_ID = Util.GetValueOfInt(Get_Value("C_SubAcct_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Activity.Equals(et) && Get_ColumnIndex("VAB_BillingCode_ID") > 0)
                        VAB_BillingCode_ID = Util.GetValueOfInt(Get_Value("VAB_BillingCode_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_BPartner.Equals(et) && Get_ColumnIndex("VAB_BusinessPartner_ID") > 0)
                        VAB_BusinessPartner_ID = Util.GetValueOfInt(Get_Value("VAB_BusinessPartner_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Campaign.Equals(et) && Get_ColumnIndex("VAB_BusinessPartner_ID") > 0)
                        VAB_BusinessPartner_ID = Util.GetValueOfInt(Get_Value("VAB_BusinessPartner_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Organization.Equals(et))
                        VAF_Org_ID = GetVAF_Org_ID();
                    if (MAcctSchemaElement.ELEMENTTYPE_OrgTrx.Equals(et) && Get_ColumnIndex("VAF_OrgTrx_ID") > 0)
                        VAF_OrgTrx_ID = Util.GetValueOfInt(Get_Value("VAF_OrgTrx_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("C_LocFrom_ID") > 0)
                        C_LocFrom_ID = Util.GetValueOfInt(Get_Value("C_LocFrom_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("C_LocTo_ID") > 0)
                        C_LocTo_ID = Util.GetValueOfInt(Get_Value("C_LocTo_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Product.Equals(et) && Get_ColumnIndex("M_Product_ID") > 0)
                        M_Product_ID = Util.GetValueOfInt(Get_Value("M_Product_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Project.Equals(et) && Get_ColumnIndex("C_Project_ID") > 0)
                        C_Project_ID = Util.GetValueOfInt(Get_Value("C_Project_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_Project.Equals(et) && Get_ColumnIndex("VAB_Promotion_ID") > 0)
                        VAB_Promotion_ID = Util.GetValueOfInt(Get_Value("VAB_Promotion_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_SalesRegion.Equals(et) && Get_ColumnIndex("C_SalesRegion_ID") > 0)
                        C_SalesRegion_ID = Util.GetValueOfInt(Get_Value("C_SalesRegion_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_UserList1.Equals(et) && Get_ColumnIndex("User1_ID") > 0)
                        User1_ID = Util.GetValueOfInt(Get_Value("User1_ID"));
                    if (MAcctSchemaElement.ELEMENTTYPE_UserList2.Equals(et) && Get_ColumnIndex("User2_ID") > 0)
                        User2_ID = Util.GetValueOfInt(Get_Value("User2_ID"));
                }

                MAccount acct = MAccount.Get(GetCtx(), GetVAF_Client_ID(), VAF_Org_ID, gl.GetVAB_AccountBook_ID(), Account_ID,
                        C_SubAcct_ID, M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID, C_LocTo_ID, C_SalesRegion_ID,
                        C_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID, User1_ID, User2_ID, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                if (acct != null)
                {
                    acct.Save(Get_TrxName());	// get ID from transaction
                    SetC_ValidCombination_ID(acct.Get_ID());

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
            if (GetC_ValidCombination_ID() > 0)
            {
                MAccount combi = new MAccount(GetCtx(), GetC_ValidCombination_ID(), Get_TrxName());
                if (Get_ColumnIndex("Account_ID") > 0)
                    Set_Value("Account_ID", combi.GetAccount_ID() > 0 ? combi.GetAccount_ID() : 0);
                if (Get_ColumnIndex("C_SubAcct_ID") > 0)
                    Set_Value("C_SubAcct_ID", combi.GetC_SubAcct_ID() > 0 ? combi.GetC_SubAcct_ID() : 0);
                // setting null in business partner and product search control because if set 0 then it shows <0> in controls.-Mohit-11 May 2020
                if (Get_ColumnIndex("M_Product_ID") > 0)
                {
                    if (combi.GetM_Product_ID() > 0)
                        Set_Value("M_Product_ID", combi.GetM_Product_ID());
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
                if (Get_ColumnIndex("C_SalesRegion_ID") > 0)
                    Set_Value("C_SalesRegion_ID", combi.GetC_SalesRegion_ID() > 0 ? combi.GetC_SalesRegion_ID() : 0);
                if (Get_ColumnIndex("C_Project_ID") > 0)
                    Set_Value("C_Project_ID", combi.GetC_Project_ID() > 0 ? combi.GetC_Project_ID() : 0);
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
            String sql = "UPDATE GL_Journal j"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(AmtAcctDr), SUM(AmtAcctCr)" //jz ", "
                    + " FROM GL_JournalLine jl WHERE jl.IsActive='Y' AND j.GL_Journal_ID=jl.GL_Journal_ID) "
                + "WHERE GL_Journal_ID=" + GetGL_Journal_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("afterSave - Update Journal #" + no);
            }

            // Manish 18/7/2016 .. chk is gl_journalbatch_id there or not.
            string sqlquery = @"SELECT gl_journalbatch_id FROM gl_journal WHERE gl_journal_id IN
                                ( SELECT gl_journal_id FROM GL_JournalLine WHERE GL_JournalLine_ID=" + GetGL_JournalLine_ID() + " )";

            int nooo = Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sqlquery, null, null));
            if (nooo <= 0)
            {
                return no == 1;
            }

            //	Update Batch Total
            sql = "UPDATE GL_JournalBatch jb"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(TotalDr), SUM(TotalCr)" //jz hard coded ", "
                    + " FROM GL_Journal j WHERE jb.GL_JournalBatch_ID=j.GL_JournalBatch_ID) "
                + "WHERE GL_JournalBatch_ID="
                    + "(SELECT DISTINCT GL_JournalBatch_ID FROM GL_Journal WHERE GL_Journal_ID="
                    + GetGL_Journal_ID() + ")";
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
                toLine.SetC_Location_ID(fromLines[i].GetC_Location_ID());
                toLine.SetC_Project_ID(fromLines[i].GetC_Project_ID());
                toLine.SetC_SalesRegion_ID(fromLines[i].GetC_SalesRegion_ID());
                toLine.SetM_Product_ID(fromLines[i].GetM_Product_ID());
                toLine.SetOrg_ID(fromLines[i].GetOrg_ID());
                toLine.SetSeqNo(fromLines[i].GetSeqNo());
                toLine.SetLineType(fromLines[i].GetLineType());
                toLine.SetGL_JournalLine_ID(newlineID);

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
            String sql = "SELECT * FROM GL_LineDimension WHERE GL_JournalLine_ID=@Param1 ORDER BY Line";
            //PreparedStatement pstmt = null;
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, GetGL_JournalLine_ID());
                Param[0] = new SqlParameter("@Param1", GetGL_JournalLine_ID());

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
