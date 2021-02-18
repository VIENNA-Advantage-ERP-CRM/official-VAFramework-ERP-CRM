using System;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;


namespace VAdvantage.Process
{
    public class RevenueRecognitionRun : SvrProcess
    {
        static VLogger log = VLogger.GetVLogger("RevenueRecognitionRun");
        private int _DocType = 0;
        private DateTime? _RecognitionDate = null;
        private int _RevenueRecognition_ID = 0;
        private MVAGLJRNL journal = null;
        private MVAGLJRNLLine journalLine = null;
        private int _AcctSchema_ID = 0;
        private int _Currency_ID = 0;
        private int VAB_YearPeriod_ID = 0;
        private string DocNo = null;
        private DateTime? _RecognizeDate = null;
        private int _orgId = 0;
        private int lineno = 0;
        private ValueNamePair pp = null;
        private string errorMsg = "";
        private int[] journal_ID = null;
        private string journalIDS = null;

        /// <summary>
        /// Prepare
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
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _DocType = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("DateAcct"))
                {
                    _RecognitionDate = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Rev_Recognition_ID"))
                {
                    _RevenueRecognition_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    _orgId = Util.GetValueOfInt(para[i].GetParameter());
                }
            }
        }
        
        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override string DoIt()
        {
            if (!Env.IsModuleInstalled("FRPT_")) 
            {
                return Msg.GetMsg(GetCtx(), "InstallFinancialManagement");
            }
            String msg = "";
            MVABRevRecognition mRevenueRecognition = null;
            if (_RecognitionDate <= DateTime.Now)
            {
                if (_RevenueRecognition_ID > 0)
                {
                    mRevenueRecognition = new MVABRevRecognition(GetCtx(), _RevenueRecognition_ID, Get_TrxName());
                    msg = CreateJournals(mRevenueRecognition);
                }
                else
                {
                    MVABRevRecognition[] RevenueRecognitions = MVABRevRecognition.GetRecognitions(GetCtx(), Get_TrxName());
                    if (RevenueRecognitions != null && RevenueRecognitions.Length > 0)
                    {
                        for (int i = 0; i < RevenueRecognitions.Length; i++)
                        {
                            mRevenueRecognition = RevenueRecognitions[i];
                            msg = CreateJournals(mRevenueRecognition);
                        }
                    }
                }
            }
            else
            {
                msg = Msg.GetMsg(GetCtx(), "AccountDateGreater"); 
            }
            return msg;
        }

        /// <summary>
        /// Create GL Journal
        /// </summary>
        /// <param name="mRevenueRecognition">Revenue Recognition</param>
        /// <returns>Message</returns>
        public string CreateJournals(MVABRevRecognition mRevenueRecognition)
        {

            try
            {
                MVABRevRecognitionRun[] mRevenueRecognitionRuns = null;

                MVABRevRecognitionStrtgy revenueRecognitionPlan = null;
                MVABInvoiceLine invoiceLine = null;
                MVABInvoice invoice = null;


                mRevenueRecognitionRuns = MVABRevRecognitionRun.GetRecognitionRuns(mRevenueRecognition, _RecognitionDate, _orgId, false);
                journal_ID = new int[mRevenueRecognitionRuns.Length];
                if (mRevenueRecognitionRuns.Length > 0)
                {
                    for (int j = 0; j < mRevenueRecognitionRuns.Length; j++)
                    {
                        MVABRevRecognitionRun revenueRecognitionRun = mRevenueRecognitionRuns[j];
                        revenueRecognitionPlan = new MVABRevRecognitionStrtgy(GetCtx(), revenueRecognitionRun.GetVAB_Rev_RecognitionStrtgy_ID(), Get_TrxName());
                        invoiceLine = new MVABInvoiceLine(GetCtx(), revenueRecognitionPlan.GetVAB_InvoiceLine_ID(), Get_TrxName());
                        invoice = new MVABInvoice(GetCtx(), invoiceLine.GetVAB_Invoice_ID(), Get_TrxName());

                        if (revenueRecognitionPlan.GetVAB_AccountBook_ID() != _AcctSchema_ID || revenueRecognitionPlan.GetVAB_Currency_ID() != _Currency_ID || revenueRecognitionRun.GetRecognitionDate() != _RecognizeDate)
                        {
                            if (journal != null)
                            {

                                if (DocNo == null)
                                {
                                    DocNo = journal.GetDocumentNo();
                                }
                                else
                                {
                                    DocNo += ", " + journal.GetDocumentNo();

                                }
                                journal_ID[j - 1] = journal.GetVAGL_JRNL_ID();

                            }
                            journal = new MVAGLJRNL(GetCtx(), 0, Get_TrxName());

                            journal.SetVAB_DocTypes_ID(_DocType);
                            journal = CreateJournalHDR(revenueRecognitionPlan, revenueRecognitionRun, mRevenueRecognition.GetRecognitionFrequency());


                            if (journal.Save())
                            {
                                _AcctSchema_ID = journal.GetVAB_AccountBook_ID();
                                _Currency_ID = journal.GetVAB_Currency_ID();
                                _RecognizeDate = revenueRecognitionRun.GetRecognitionDate();
                                lineno = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0)+10  AS DefaultValue FROM VAGL_JRNLLine WHERE VAGL_JRNL_ID=" + journal.GetVAGL_JRNL_ID(), null, invoice.Get_Trx()));

                            }
                            else
                            {
                                pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    errorMsg = pp.GetName();
                                    if (errorMsg == "")
                                    {
                                        errorMsg = pp.GetValue();
                                    }
                                }
                                if (errorMsg == "")
                                {
                                    errorMsg = Msg.GetMsg(GetCtx(), "GLJournalNotCreated");
                                }
                                Get_TrxName().Rollback();
                                return errorMsg;
                            }
                        }
                        for (int k = 0; k < 2; k++)
                        {
                            journalLine = new MVAGLJRNLLine(journal);
                            journalLine = GenerateJounalLine(journal, invoice, invoiceLine, revenueRecognitionPlan, revenueRecognitionRun, mRevenueRecognition.GetRecognitionType(), k);
                            if (journalLine.Save())
                            {
                                revenueRecognitionRun.SetVAGL_JRNL_ID(journal.GetVAGL_JRNL_ID());
                                revenueRecognitionRun.Save();
                                lineno += 10;
                            }
                            else
                            {
                                pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    errorMsg = pp.GetName();
                                    if (errorMsg == "")
                                    {
                                        errorMsg = pp.GetValue();
                                    }
                                }
                                if (errorMsg == "")
                                {
                                    errorMsg = Msg.GetMsg(GetCtx(), "GLJournalNotCreated");
                                }
                                Get_TrxName().Rollback();
                                return errorMsg;
                            }
                        }
                        revenueRecognitionPlan.SetRecognizedAmt(revenueRecognitionRun.GetRecognizedAmt() + revenueRecognitionPlan.GetRecognizedAmt());
                        if (!revenueRecognitionPlan.Save())
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                errorMsg = pp.GetName();
                                if (errorMsg == "")
                                {
                                    errorMsg = pp.GetValue();
                                }
                            }
                            if (errorMsg == "")
                            {
                                errorMsg = Msg.GetMsg(GetCtx(), "GLJournalNotCreated");
                            }
                            Get_TrxName().Rollback();
                            return errorMsg;
                        }
                    }
                    if (journal != null)
                    {
                        if (DocNo == null)
                        {
                            DocNo = journal.GetDocumentNo();
                        }
                        else
                        {
                            DocNo += ", " + journal.GetDocumentNo();

                        }

                        journal_ID[journal_ID.Length - 1] = journal.GetVAGL_JRNL_ID();

                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "GLJournalnotallocateddueto") + ex.Message);
                Get_TrxName().Rollback();
                return ex.Message;
            }
            if (DocNo == null)
            {
               return Msg.GetMsg(GetCtx(), "FoundNoRevenueRecognitionPlan");
            }

            Get_TrxName().Commit();
            if (journal_ID != null)
            {
                for (int i = 0; i < journal_ID.Length; i++)
                {
                    if (journal_ID[i] > 0)
                    {
                        string result = CompleteOrReverse(GetCtx(), journal_ID[i], 169, "CO");
                        if (!String.IsNullOrEmpty(result))
                        {
                            journalIDS += ", " + journal_ID[i];
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(journalIDS))
            {
                return Msg.GetMsg(GetCtx(), "GLJournalCreated") + DocNo + " "+ Msg.GetMsg(GetCtx(), "GLJournalNotCompleted") + journalIDS;
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "GLJournalCreated") + DocNo;
            }
        }

        /// <summary>
        /// Create journal line
        /// </summary>
        /// <param name="journal">VAGL_JRNL</param>
        /// <param name="invoice">Invoice</param>
        /// <param name="invoiceLine">Invoice Line</param>
        /// <param name="revenueRecognitionPlan">Revenue Recognition Plan</param>
        /// <param name="revenueRecognitionRun">Revenue Recognition run</param>
        /// <param name="recognitionType">Recognition type</param>
        /// <param name="k">loop variable</param>
        /// <returns>Journal line object</returns>
        public MVAGLJRNLLine GenerateJounalLine(MVAGLJRNL journal, MVABInvoice invoice, MVABInvoiceLine invoiceLine,
                                            MVABRevRecognitionStrtgy revenueRecognitionPlan, MVABRevRecognitionRun revenueRecognitionRun, string recognitionType, int k)
        {
            try
            {
                int combination_ID = 0;
                if (k == 0)
                {
                    combination_ID = GetCombinationID(invoiceLine.GetVAM_Product_ID(), invoiceLine.GetVAB_Charge_ID(), journal.GetVAB_AccountBook_ID(), invoice.IsSOTrx(), invoice.IsReturnTrx(), revenueRecognitionRun.GetRecognizedAmt(),revenueRecognitionPlan.GetVAB_Rev_Recognition_ID());
                    journalLine.SetLine(lineno);
                    if (recognitionType.Equals("E") && revenueRecognitionRun.GetRecognizedAmt() > 0)
                    {
                        journalLine.SetAmtAcctDr(revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(0);
                        journalLine.SetAmtAcctCr(0);
                    }
                    else if (recognitionType.Equals("E") && revenueRecognitionRun.GetRecognizedAmt() < 0)
                    {
                        journalLine.SetAmtAcctCr(Decimal.Negate(revenueRecognitionRun.GetRecognizedAmt()));
                        journalLine.SetAmtSourceCr(Decimal.Negate(revenueRecognitionRun.GetRecognizedAmt()));
                        journalLine.SetAmtSourceDr(0);
                        journalLine.SetAmtAcctDr(0);
                    }
                    else if (recognitionType.Equals("R") && revenueRecognitionRun.GetRecognizedAmt() > 0)
                    {
                        journalLine.SetAmtAcctCr(revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(0);
                        journalLine.SetAmtAcctDr(0);
                    }
                    else if (recognitionType.Equals("R") && revenueRecognitionRun.GetRecognizedAmt() < 0)
                    {
                        journalLine.SetAmtAcctDr(Decimal.Negate(revenueRecognitionRun.GetRecognizedAmt()));
                        journalLine.SetAmtSourceDr(Decimal.Negate(revenueRecognitionRun.GetRecognizedAmt()));
                        journalLine.SetAmtSourceCr(0);
                        journalLine.SetAmtAcctCr(0);
                    }
                    int account_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Account_ID From VAB_Acct_ValidParameter Where VAB_Acct_ValidParameter_ID=" + combination_ID));
                    journalLine.Set_ValueNoCheck("Account_ID", account_ID);
                    journalLine.Set_ValueNoCheck("VAB_BusinessPartner_ID", invoice.GetVAB_BusinessPartner_ID());
                    journalLine.SetVAF_OrgTrx_ID(invoiceLine.Get_ColumnIndex("VAF_OrgTrx_ID") > 0 ? invoiceLine.GetVAF_OrgTrx_ID() : invoice.GetVAF_OrgTrx_ID());
                    journalLine.Set_ValueNoCheck("VAB_Project_ID", invoiceLine.GetVAB_Project_ID() > 0 ? invoiceLine.GetVAB_Project_ID() : invoice.Get_Value("VAB_ProjectRef_ID"));
                    journalLine.Set_ValueNoCheck("VAB_Promotion_ID", invoiceLine.Get_ColumnIndex("VAB_Promotion_ID") > 0 ? invoiceLine.GetVAB_Promotion_ID() : invoice.GetVAB_Promotion_ID());
                    journalLine.Set_ValueNoCheck("VAB_BillingCode_ID", invoiceLine.Get_ColumnIndex("VAB_BillingCode_ID") > 0 ? invoiceLine.GetVAB_BillingCode_ID() : invoice.GetVAB_BillingCode_ID());
                    journalLine.Set_ValueNoCheck("VAM_Product_ID", invoiceLine.GetVAM_Product_ID());
                                   
                }
                else
                {
                    combination_ID = GetCombinationID(0, 0, revenueRecognitionPlan.GetVAB_AccountBook_ID(), invoice.IsSOTrx(), invoice.IsReturnTrx(), revenueRecognitionRun.GetRecognizedAmt(), revenueRecognitionPlan.GetVAB_Rev_Recognition_ID());
                    int account_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Account_ID From VAB_Acct_ValidParameter Where VAB_Acct_ValidParameter_ID=" + combination_ID));

                    journalLine = GetOrCreate(journal, journalLine, invoiceLine.GetVAM_Product_ID(), invoiceLine.GetVAB_Charge_ID(),
                        invoiceLine.Get_ColumnIndex("VAB_Promotion_ID") > 0 ? invoiceLine.GetVAB_Promotion_ID() : invoice.GetVAB_Promotion_ID(),
                    account_ID, invoiceLine.GetVAB_Project_ID() > 0 ? invoiceLine.GetVAB_Project_ID() : Util.GetValueOfInt(invoice.Get_Value("VAB_ProjectRef_ID")),
                    invoiceLine.Get_ColumnIndex("VAB_BillingCode_ID") > 0 ? invoiceLine.GetVAB_BillingCode_ID() : invoice.GetVAB_BillingCode_ID(), invoice.GetVAB_BusinessPartner_ID(),
                    invoice.GetVAF_Org_ID(), invoiceLine.Get_ColumnIndex("VAF_OrgTrx_ID") > 0 ? invoiceLine.GetVAF_OrgTrx_ID() : invoice.GetVAF_OrgTrx_ID());


                    if (recognitionType.Equals("E") && revenueRecognitionRun.GetRecognizedAmt() > 0)
                    {
                        journalLine.SetAmtAcctCr(journalLine.GetAmtAcctCr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(journalLine.GetAmtSourceCr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(0);
                        journalLine.SetAmtAcctDr(0);
                    }
                    else if (recognitionType.Equals("E") && revenueRecognitionRun.GetRecognizedAmt() < 0)
                    {
                        journalLine.SetAmtAcctDr(journalLine.GetAmtAcctDr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(journalLine.GetAmtSourceDr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(0);
                        journalLine.SetAmtAcctCr(0);
                    }
                    else if (recognitionType.Equals("R") && revenueRecognitionRun.GetRecognizedAmt() > 0)
                    {
                        journalLine.SetAmtAcctDr(journalLine.GetAmtAcctDr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(journalLine.GetAmtSourceDr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(0);
                        journalLine.SetAmtAcctCr(0);
                    }
                    else if (recognitionType.Equals("R") && revenueRecognitionRun.GetRecognizedAmt() < 0)
                    {
                        journalLine.SetAmtAcctCr(journalLine.GetAmtAcctCr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceCr(journalLine.GetAmtSourceCr() + revenueRecognitionRun.GetRecognizedAmt());
                        journalLine.SetAmtSourceDr(0);
                        journalLine.SetAmtAcctDr(0);
                    }
                }
            }
            catch (Exception ex)
            {
                log.SaveError(null, ex);
            }
            return journalLine;
        }

        /// <summary>
        /// Create Gl Journal 
        /// </summary>
        /// <param name="revenueRecognitionPlan">Revenue Recognition Plan</param>
        /// <param name="revenurecognitionRun">Revenue Recognition Run</param>
        /// <param name="recFrequency">Frequency</param>
        /// <returns>Journal object</returns>
        public MVAGLJRNL CreateJournalHDR(MVABRevRecognitionStrtgy revenueRecognitionPlan, MVABRevRecognitionRun revenurecognitionRun, string recFrequency)
        {
            journal.SetClientOrg(revenueRecognitionPlan.GetVAF_Client_ID(), revenueRecognitionPlan.GetVAF_Org_ID());
            journal.SetVAB_AccountBook_ID(revenueRecognitionPlan.GetVAB_AccountBook_ID());
            journal.SetDescription("Revenue Recognition Run");
            journal.SetPostingType(MVAGLJRNL.POSTINGTYPE_Actual);

            int VAGL_Group_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VAGL_Group_ID From VAGL_Group Where CategoryType='M' 
            AND  VAF_Client_ID= " + revenueRecognitionPlan.GetVAF_Client_ID() + " Order by VAGL_Group_ID desc"));
            journal.SetVAGL_Group_ID(VAGL_Group_ID);

            journal.SetDateDoc(DateTime.Now);
            DateTime firstOfNextMonth = new DateTime(revenurecognitionRun.GetRecognitionDate().Value.Year, revenurecognitionRun.GetRecognitionDate().Value.Month, 1).AddMonths(1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddDays(-1);
            if (recFrequency.Equals("D")) //DAY
            {
                //in case of DAY , Account date would be same as Recoganize date
                journal.SetDateAcct(revenurecognitionRun.GetRecognitionDate());
            }
            else
            {
                journal.SetDateAcct(lastOfThisMonth);
            }

            string periodSql = "SELECT VAB_YearPeriod_ID From VAB_YearPeriod pr  INNER JOIN VAB_Year yr ON (yr.VAB_Year_id = pr.VAB_Year_id AND yr.VAB_Calender_id= " +
                "(CASE WHEN (SELECT NVL(VAB_Calender_ID,0) FROM VAF_OrgDetail WHERE vaf_org_ID =" + revenueRecognitionPlan.GetVAF_Org_ID() + " ) =0 THEN (SELECT  NVL(VAB_Calender_ID,0) FROM VAF_ClientDetail WHERE VAF_Client_ID =" + revenueRecognitionPlan.GetVAF_Client_ID() + ") ELSE " +
                "(SELECT NVL(VAB_Calender_ID,0) FROM VAF_OrgDetail WHERE vaf_org_ID =" + revenueRecognitionPlan.GetVAF_Org_ID() + ") END ) ) WHERE " + GlobalVariable.TO_DATE(revenurecognitionRun.GetRecognitionDate(), true) + " BETWEEN StartDate and EndDate";

            VAB_YearPeriod_ID = Util.GetValueOfInt(DB.ExecuteScalar(periodSql));

            journal.SetVAB_YearPeriod_ID(VAB_YearPeriod_ID);
            journal.SetVAB_Currency_ID(revenueRecognitionPlan.GetVAB_Currency_ID());
            int VAB_CurrencyType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_CurrencyType_ID From VAB_CurrencyType WHERE IsDefault='Y'"));
            journal.SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
            journal.SetTotalCr(revenueRecognitionPlan.GetTotalAmt());
            journal.SetTotalDr(revenueRecognitionPlan.GetTotalAmt());
            journal.SetDocStatus("DR");
            journal.SetDocAction("CO");

            return journal;
        }

        /// <summary>
        /// Get Combination_ID from Accounting Default 
        /// </summary>
        /// <param name="Product_ID">Product</param>
        /// <param name="Charge_ID">charge</param>
        /// <param name="AcctSchema_ID">Accounting Schema</param>
        /// <param name="IssoTrx">Sales Transaction</param>
        /// <param name="IsreturnTrx">Return Transaction</param>
        /// <param name="AcctSchema_ID">Accounting Schema</param>
        /// <returns>Combination_ID</returns>
        public int GetCombinationID(int Product_ID, int Charge_ID, int AcctSchema_ID, bool IssoTrx, bool IsreturnTrx, decimal amount, int RevenueRecognition_ID)
        {
            char isSoTrx = IssoTrx ? 'Y' : 'N';
            char isReturnTrx = IsreturnTrx ? 'Y' : 'N';

            string sql = "SELECT VAB_Acct_ValidParameter_ID FROM ";
            
            if (Product_ID > 0)
            {
                sql += "FRPT_Product_Acct WHERE VAM_Product_ID= "+ Product_ID;
            }
            else if (Charge_ID > 0)
            {
                sql += "FRPT_Charge_Acct WHERE VAB_Charge_ID="+ Charge_ID;
            }
            else
            {
                sql += "FRPT_RevenueRecognition_Acct WHERE VAB_Rev_Recognition_ID= "+ RevenueRecognition_ID;
            }
            sql += " AND  VAB_AccountBook_ID=" + AcctSchema_ID + " AND FRPT_AcctDefault_ID = (SELECT FRPT_AcctDefault_ID FROM FRPT_AcctDefault WHERE " +
            "FRPT_RecognizeType= (CASE WHEN " + Product_ID + "> 0 AND " + amount + ">0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='N') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='Y'))  THEN 'IE' " + //Ap/ARC
            "WHEN " + Product_ID + "> 0 AND " + amount + "< 0 AND(('" + isSoTrx + "' = 'N' AND '" + isReturnTrx + "' = 'N') OR('" + isSoTrx + "' = 'Y' AND '" + isReturnTrx + "' = 'Y'))  THEN 'TR' " + //AR/APC
            "WHEN " + Charge_ID + ">0 AND " + amount + ">0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='N') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='Y'))  THEN 'CE' " +      //Ap/ARC
            "WHEN " + Charge_ID + ">0 AND " + amount + "<0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='N') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='Y'))  THEN 'CR' " +      //AR/APC
            "WHEN " + Product_ID + ">0 AND " + amount + "> 0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "' ='Y') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='N'))   THEN 'TR' " +  //AR/APC
            "WHEN " + Product_ID + ">0 AND " + amount + "< 0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "' ='Y') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='N'))   THEN 'IE' " +  //Ap/ARC
            "WHEN " + Charge_ID + ">0 AND " + amount + "> 0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='Y') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='N'))   THEN 'CR' " +    //AR/APC
            "WHEN " + Charge_ID + ">0 AND " + amount + "< 0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='Y') OR ('" + isSoTrx + "'='Y' AND '" + isReturnTrx + "'='N'))   THEN 'CE' " +    //Ap/ARC
            "WHEN " + Product_ID + "=0 AND " + Charge_ID + "=0 AND " + amount + ">0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='N') OR ('" + isSoTrx + "'='Y' AND  '" + isReturnTrx + "'='Y'))   THEN 'PE' " +  //AP/ARC
            "WHEN " + Product_ID + "=0 AND " + Charge_ID + "=0 AND " + amount + ">0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='Y') OR ('" + isSoTrx + "'='Y' AND  '" + isReturnTrx + "'='N'))   THEN 'UE' " +  //AR/APC
            "WHEN " + Product_ID + "=0 AND " + Charge_ID + "=0 AND " + amount + "<0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='N') OR ('" + isSoTrx + "'='Y' AND  '" + isReturnTrx + "'='Y'))   THEN 'UE' " +  //AR/APC
            "WHEN " + Product_ID + "=0 AND " + Charge_ID + "=0 AND " + amount + "<0 AND (('" + isSoTrx + "'='N' AND '" + isReturnTrx + "'='Y') OR ('" + isSoTrx + "'='Y' AND  '" + isReturnTrx + "'='N'))   THEN 'PE' " +  //AP/ARC

            "END) AND FRPT_RelatedTo=(CASE WHEN " + Product_ID + ">0 Then '35' WHEN  " + Charge_ID + ">0 Then '20'  ELSE '80' END ))";

            return Util.GetValueOfInt(DB.ExecuteScalar(sql));

        }

        /// <summary>
        /// Get or Create Gl Journal Line
        /// </summary>
        /// <param name="Journal">GL Journal</param>
        /// <param name="Line">GL Journal Line</param>
        /// <param name="VAM_Product_ID">Product</param>
        /// <param name="VAB_Charge_ID">Charge</param>
        /// <param name="Campaign_ID">Campaign</param>
        /// <param name="Account_ID">Account</param>
        /// <param name="Opprtunity_ID">Opportunity</param>
        /// <param name="Activity_ID">Activity</param>
        /// <param name="BPartner_ID">Business Partner</param>
        /// <param name="Org_Id">Organization</param>
        /// <param name="trxOrg_ID">Transaction Organization</param>
        /// <returns>journal line object</returns>
        public MVAGLJRNLLine GetOrCreate(MVAGLJRNL Journal, MVAGLJRNLLine Line, int VAM_Product_ID, int VAB_Charge_ID, int Campaign_ID, int Account_ID, int Opprtunity_ID, int Activity_ID, int BPartner_ID, int Org_Id, int trxOrg_ID)
        {
            MVAGLJRNLLine retValue = null;
            String sql = "SELECT * FROM VAGL_JRNLLine " +
                         " WHERE  VAGL_JRNL_ID = " + Journal.GetVAGL_JRNL_ID() +
                         " AND NVL(VAM_Product_ID,0)=" + VAM_Product_ID +                        
                         " AND NVL(ACCOUNT_ID,0)=" + Account_ID +
                         " AND NVL(VAB_PROMOTION_ID,0)=" + Campaign_ID +
                         " AND NVL(VAB_PROJECT_ID,0)=" + Opprtunity_ID +
                         " AND NVL(VAB_BILLINGCODE_ID,0)=" + Activity_ID +
                          " AND NVL(VAB_BUSINESSPARTNER_ID,0)=" + BPartner_ID +
                           " AND NVL(VAF_ORGTRX_ID,0)=" + trxOrg_ID +
                            " AND NVL(VAF_ORG_ID,0)=" + Org_Id;


            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAGLJRNLLine(Journal.GetCtx(), dr, Get_TrxName());
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            if (retValue == null)
                retValue = CreateJournalLine(Journal, Line, VAM_Product_ID, VAB_Charge_ID, Campaign_ID, Account_ID, Opprtunity_ID, Activity_ID, BPartner_ID, trxOrg_ID);


            return retValue;
        }

        /// <summary>
        /// Generate Journal Line
        /// </summary>
        /// <param name="Journal">GL Journal</param>
        /// <param name="Line">Gl Journal line</param>
        /// <param name="VAM_Product_ID">Product</param>
        /// <param name="VAB_Charge_ID">Charge</param>
        /// <param name="Campaign_ID">Campaign</param>
        /// <param name="Account_ID">Account</param>
        /// <param name="Opprtunity_ID">Opportunity</param>
        /// <param name="Activity_ID">Activity</param>
        /// <param name="BPartner_ID">Business Partner</param>
        /// <param name="trxOrg_ID">Transaction Organization</param>
        /// <returns>object of journal line</returns>
        public MVAGLJRNLLine CreateJournalLine(MVAGLJRNL Journal, MVAGLJRNLLine Line, int VAM_Product_ID, int VAB_Charge_ID, int Campaign_ID, int Account_ID, int Opprtunity_ID, int Activity_ID, int BPartner_ID, int trxOrg_ID)
        {
            Line = new MVAGLJRNLLine(Journal);
            Line.SetLine(lineno);
            Line.Set_ValueNoCheck("Account_ID", Account_ID);
            Line.Set_ValueNoCheck("VAB_BusinessPartner_ID", BPartner_ID);
            Line.Set_ValueNoCheck("VAM_Product_ID", VAM_Product_ID);
            Line.SetVAF_OrgTrx_ID(trxOrg_ID);
            //Line.Set_ValueNoCheck("VAB_Charge_ID", VAB_Charge_ID);
            Line.Set_ValueNoCheck("VAB_Promotion_ID", Campaign_ID);
            Line.Set_ValueNoCheck("VAB_Project_ID", Opprtunity_ID);
            Line.Set_ValueNoCheck("VAB_BillingCode_ID", Activity_ID);


            return Line;
        }

        /// <summary>
        /// Mehtod added to complete and reverse the document and execute the workflow as well
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="Record_ID">VAGL_JRNL_ID</param>
        /// <param name="Process_ID">Process</param>
        /// <param name="DocAction">Document Action</param>
        /// <returns>result of completion or reversal in a string array</returns>
        private string CompleteOrReverse(Ctx ctx, int Record_ID, int Process_ID, string DocAction)
        {
            string result = "";
            MVAFRole role = MVAFRole.Get(ctx, ctx.GetVAF_Role_ID());
            if (Util.GetValueOfBool(role.GetProcessAccess(Process_ID)))
            {
                DB.ExecuteQuery("UPDATE VAGL_JRNL SET DocAction = '" + DocAction + "' WHERE VAGL_JRNL_ID = " + Record_ID);

                MVAFJob proc = new MVAFJob(ctx, Process_ID, null);
                MVAFJInstance pin = new MVAFJInstance(proc, Record_ID);
                if (!pin.Save())
                {
                    ValueNamePair vnp = VLogger.RetrieveError();
                    string errorMsg = "";
                    if (vnp != null)
                    {
                        errorMsg = vnp.GetName();
                        if (errorMsg == "")
                            errorMsg = vnp.GetValue();
                    }
                    if (errorMsg == "")
                        result = errorMsg = Msg.GetMsg(ctx, "DocNotCompleted");

                    return result;
                }

                MVAFJInstancePara para = new MVAFJInstancePara(pin, 20);
                para.setParameter("DocAction", DocAction);
                if (!para.Save())
                {
                    //String msg = "No DocAction Parameter added";  //  not translated
                }
                ProcessInfo pi = new ProcessInfo("WF", Process_ID);
                pi.SetVAF_UserContact_ID(ctx.GetVAF_UserContact_ID());
                pi.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                pi.SetVAF_JInstance_ID(pin.GetVAF_JInstance_ID());
                pi.SetRecord_ID(Record_ID);
                if (Process_ID == 169)
                {
                    pi.SetTable_ID(224);
                }

                ProcessCtl worker = new ProcessCtl(ctx, null, pi, null);
                worker.Run();

                if (pi.IsError())
                {
                    ValueNamePair vnp = VLogger.RetrieveError();
                    string errorMsg = "";
                    if (vnp != null)
                    {
                        errorMsg = vnp.GetName();
                        if (errorMsg == "")
                            errorMsg = vnp.GetValue();
                    }

                    if (errorMsg == "")
                        errorMsg = pi.GetSummary();

                    if (errorMsg == "")
                        errorMsg = Msg.GetMsg(ctx, "DocNotCompleted");
                    result = errorMsg;
                    return result;
                }
                else
                    result = "";
            }
            else
            {
                result = Msg.GetMsg(ctx, "NoAccess");
                return result;
            }
            return result;
        }
    }
}
