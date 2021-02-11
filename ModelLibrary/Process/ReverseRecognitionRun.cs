using System;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;


namespace VAdvantage.Process
{
    class ReverseRecognitionRun : SvrProcess
    {
        static VLogger log = VLogger.GetVLogger("RevenueRecognitionRun");
        private int _DocType = 0;
        private int _RevenueRecognition_ID = 0;
        private MJournal journal = null;
        private MJournalLine journalLine = null;
        private int C_Period_ID = 0;
        private int _AcctSchema_ID = 0;
        private int _Currency_ID = 0;
        private string DocNo = null;
        private int C_InvoiceLine_ID = 0;
        private string ReversalType = "P";
        private int _orgId = 0;
        private int[] journal_ID = null;
        private string journalIDS = null;
        private int lineno = 0;
        private ValueNamePair pp = null;
        private string errorMsg = "";
        private decimal totalAmt = 0;

        RevenueRecognitionRun RevenueRun = new RevenueRecognitionRun();
        /// <summary>
        /// Preapre
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
                else if (name.Equals("C_DocType_ID"))
                {
                    _DocType = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_RevenueRecognition_ID"))
                {
                    _RevenueRecognition_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_InvoiceLine_ID"))
                {
                    C_InvoiceLine_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                //else if (name.Equals("ReversalType"))
                //{
                //    ReversalType = Util.GetValueOfString(para[i].GetParameter());
                //}
                else if (name.Equals("AD_Org_ID"))
                {
                    _orgId = Util.GetValueOfInt(para[i].GetParameter());
                }
            }
        }
        /// <summary>
        /// Process
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            try
            {
                MRevenueRecognitionPlan revenueRecognitionPlan = null;
                MInvoiceLine invoiceLine = null;
                MInvoice invoice = null;
                MRevenueRecognition mRevenueRecognition = new MRevenueRecognition(GetCtx(), _RevenueRecognition_ID, Get_Trx());

                // Count of  previous date's RevenueRecognition_Run whose journal is not created 
                string sql = "SELECT COUNT(C_RevenueRecognition_Run_ID) FROM C_RevenueRecognition_Run run " +
                    "INNER JOIN C_RevenueRecognition_Plan pl ON pl.C_RevenueRecognition_Plan_ID = run.C_RevenueRecognition_Plan_ID WHERE ";
                if (C_InvoiceLine_ID > 0)
                {
                    sql += "pl.C_invoiceLine_ID = " + C_InvoiceLine_ID + " AND ";
                }

                sql += "pl.C_RevenueRecognition_ID = " + _RevenueRecognition_ID + " AND NVL(GL_Journal_ID, 0)<=0 AND run.RECOGNITIONDATE < " + GlobalVariable.TO_DATE(DateTime.Now, true);

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) == 0)
                {
                    MRevenueRecognitionPlan[] revenueRecognitionPlans = MRevenueRecognitionPlan.GetRecognitionPlans(mRevenueRecognition, C_InvoiceLine_ID, _orgId);
                    journal_ID = new int[revenueRecognitionPlans.Length];

                    if (ReversalType == "P")
                    {
                        for (int i = 0; i < revenueRecognitionPlans.Length; i++)
                        {
                             revenueRecognitionPlan = revenueRecognitionPlans[i];
                             invoiceLine = new MInvoiceLine(GetCtx(), revenueRecognitionPlan.GetC_InvoiceLine_ID(), Get_Trx());
                             invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
                           
                            //get Sum of Amount Whose journal is not yet created 
                            sql = "SELECT SUM(run.Recognizedamt) AS TotalRecognizedAmt FROM C_RevenueRecognition_Run run WHERE " +
                                "C_RevenueRecognition_Plan_ID = " + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID() + " AND NVL(GL_Journal_ID,0) <= 0";
                            
                            totalAmt = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                            if (totalAmt != 0)
                            {
                                //if totalAmount is not 0 then only create Journal 
                                
                                if (revenueRecognitionPlan.GetC_AcctSchema_ID() != _AcctSchema_ID || revenueRecognitionPlan.GetC_Currency_ID() != _Currency_ID)
                                {
                                    if (journal != null)
                                    {
                                        if (DocNo == null)
                                        {
                                            DocNo = journal.GetDocumentNo();
                                        }
                                        else
                                        {
                                            DocNo += "," + journal.GetDocumentNo();
                                        }
                                        journal_ID[i - 1] = journal.GetGL_Journal_ID();
                                    }
                                    journal = new MJournal(GetCtx(), 0, Get_Trx());

                                    journal.SetC_DocType_ID(_DocType);
                                    journal = CreateJournalHDR(revenueRecognitionPlan);
                                    if (journal.Save(Get_TrxName()))
                                    {
                                        _AcctSchema_ID = journal.GetC_AcctSchema_ID();
                                        _Currency_ID = journal.GetC_Currency_ID();
                                        lineno = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0)+10  AS DefaultValue FROM GL_JournalLine WHERE GL_Journal_ID=" + journal.GetGL_Journal_ID(), null, invoice.Get_Trx()));
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
                                revenueRecognitionPlan.SetRecognizedAmt(totalAmt + revenueRecognitionPlan.GetRecognizedAmt());
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

                                for (int k = 0; k < 2; k++)
                                {
                                    journalLine = new MJournalLine(journal);

                                    journalLine = GenerateJounalLine(journal, invoice, invoiceLine, revenueRecognitionPlan, totalAmt, mRevenueRecognition.GetRecognitionType(), k);
                                    if (journalLine.Save(Get_TrxName()))
                                    {
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
                                sql = "UPDATE C_RevenueRecognition_Run  set Gl_Journal_ID= " + journal.GetGL_Journal_ID() +
                                    " WHERE C_RevenueRecognition_Plan_ID= " + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID() + "AND NVL(Gl_Journal_ID,0)=0";
                               int count= DB.ExecuteQuery(sql, null, Get_Trx());
                               log.Log(Level.INFO,(Msg.GetMsg(GetCtx(), "RevenueRecognitionRunUpdated") + count));
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
                                DocNo += "," + journal.GetDocumentNo();
                            }
                            journal_ID[journal_ID.Length - 1] = journal.GetGL_Journal_ID();

                        }

                    }
                    #region Existing Reversal Type
                    //else if (ReversalType == "E")
                    //{
                    //    for (int i = 0; i < revenueRecognitionPlans.Length; i++)
                    //    {
                    //        MRevenueRecognitionPlan revenueRecognitionPlan = revenueRecognitionPlans[i];
                    //        MInvoiceLine invoiceLine = new MInvoiceLine(GetCtx(), revenueRecognitionPlan.GetC_InvoiceLine_ID(), Get_Trx());
                    //        MInvoice invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
                    //        sql = "Select Distinct round(SUM(recognizedamt),5) as RecognizedAmt from C_RevenueRecognition_Run Where C_RevenueRecognition_Plan_ID = " + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID() + " And NVL(GL_Journal_ID,0) > 0 ";
                    //        DataSet ds = new DataSet();
                    //        ds = DB.ExecuteDataset(sql);
                    //        if (ds != null && ds.Tables[0].Rows.Count > 0)
                    //        {
                    //            decimal Amt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["RecognizedAmt"]);
                    //            string sql1 = "Select GL_Journal_ID from C_RevenueRecognition_Run Where C_RevenueRecognition_Plan_ID = " + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID() + " And NVL(GL_Journal_ID,0) > 0 AND RowNum=1";
                    //            int GL_Journal_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
                    //            MJournal journal = new MJournal(GetCtx(), GL_Journal_ID, Get_TrxName());
                    //            log.Info(ToString());
                    //            //	Journal
                    //            MJournal reverse = new MJournal(journal);
                    //            reverse.SetDateDoc(DateTime.Now);
                    //            reverse.SetC_Period_ID(journal.GetC_Period_ID());
                    //            int Period_ID = MPeriod.GetC_Period_ID(GetCtx(), DateTime.Now);
                    //            reverse.SetDateAcct(DateTime.Now);
                    //            if (reverse.Save())
                    //            {
                    //                MJournalLine[] journalLines = journal.GetLines(false);
                    //                if (journalLines.Length > 0)
                    //                {
                    //                    for (int j = 0; j < journalLines.Length; j++)
                    //                    {
                    //                        MJournalLine journalLine = journalLines[j];
                    //                        MJournalLine newjournalLine = new MJournalLine(GetCtx(), 0, Get_TrxName());
                    //                        PO.CopyValues(journalLine, newjournalLine, GetAD_Client_ID(), GetAD_Org_ID());
                    //                        newjournalLine.SetGL_Journal_ID(reverse.GetGL_Journal_ID());

                    //                        newjournalLine.SetDateAcct(DateTime.Now);
                    //                        //	Amounts
                    //                        if (newjournalLine.GetAmtAcctCr() > 0)
                    //                        {
                    //                            newjournalLine.SetAmtAcctDr(0);
                    //                            newjournalLine.SetAmtSourceDr(0);
                    //                            newjournalLine.SetAmtSourceCr(Decimal.Negate(Amt));
                    //                            newjournalLine.SetAmtAcctCr(Decimal.Negate(Amt));
                    //                        }
                    //                        else
                    //                        {
                    //                            newjournalLine.SetAmtAcctDr(Decimal.Negate(Amt));
                    //                            newjournalLine.SetAmtSourceDr(Decimal.Negate(Amt));
                    //                            newjournalLine.SetAmtSourceCr(0);
                    //                            newjournalLine.SetAmtAcctCr(0);
                    //                        }

                    //                        newjournalLine.SetIsGenerated(true);
                    //                        newjournalLine.SetProcessed(false);
                    //                        newjournalLine.Save();
                    //                        if (j == 1)
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //            if (reverse != null && reverse.GetDocStatus() != "CO")
                    //            {
                    //                reverse.CompleteIt();
                    //                reverse.SetProcessed(true);
                    //                reverse.SetDocStatus("CO");
                    //                reverse.SetDocAction("CL");
                    //                reverse.Save(Get_TrxName());
                    //                if (DocNo == null)
                    //                {
                    //                    DocNo = reverse.GetDocumentNo();
                    //                }
                    //                int count = Util.GetValueOfInt(DB.ExecuteQuery("Update C_RevenueRecognition_Run Set GL_Journal_ID=null WHere C_RevenueRecognition_Plan_ID=" + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID()));
                    //                int count1 = Util.GetValueOfInt(DB.ExecuteQuery("Update C_RevenueRecognition_Plan Set RecognizedAmt=RecognizedAmt - " + Amt + " WHere C_RevenueRecognition_Plan_ID=" + revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID()));
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    Get_TrxName().Commit();
                    if (journal_ID != null)
                    {
                        for (int i = 0; i < journal_ID.Length; i++)
                        {
                            if (journal_ID[i] > 0)
                            {
                                string result = RevenueRun.CompleteOrReverse(GetCtx(), journal_ID[i], 169, "CO");
                                if (!String.IsNullOrEmpty(result))
                                {
                                    journalIDS += ", " + journal_ID[i] + " " + result;
                                }
                            }
                        }
                    }

                    if (DocNo == null)
                    {
                        return Msg.GetMsg(GetCtx(), "FoundNoRevenueRecognitionPlan");
                    }
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "NotRecoganized");
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "GLJournalnotallocateddueto") + ex.Message);
                Get_TrxName().Rollback();
                return ex.Message;
            }

            if (!String.IsNullOrEmpty(journalIDS))
            {
                return Msg.GetMsg(GetCtx(), "GLJournalCreated") + DocNo + ", " + Msg.GetMsg(GetCtx(), "GLJournalNotCompleted") + journalIDS;
            }         
            else
            {
                return Msg.GetMsg(GetCtx(), "GLJournalCreated") + DocNo;
            }
        }

        /// <summary>
        ///  Create Gl Journal
        /// </summary>
        /// <param name="revenueRecognitionPlan">Revenue Recognition Plan</param>
        /// <returns>Journal object</returns>
        public MJournal CreateJournalHDR(MRevenueRecognitionPlan revenueRecognitionPlan)
        {
            journal.SetClientOrg(revenueRecognitionPlan.GetAD_Client_ID(), revenueRecognitionPlan.GetAD_Org_ID());
            journal.SetC_AcctSchema_ID(revenueRecognitionPlan.GetC_AcctSchema_ID());
            journal.SetDescription(Msg.GetMsg(GetCtx(),"ReversedRecognitionRun"));
            journal.SetPostingType(MJournal.POSTINGTYPE_Actual);

            int GL_Category_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT GL_Category_ID From GL_Category Where CategoryType='M' 
            AND  AD_Client_ID= " + revenueRecognitionPlan.GetAD_Client_ID() + " AND IsActive='Y' ORDER BY GL_Category_ID desc"));
            journal.SetGL_Category_ID(GL_Category_ID);
            journal.SetDateDoc(DateTime.Now);
            journal.SetDateAcct(DateTime.Now);
            DateTime? Today = DateTime.Now;


            string periodSql = "SELECT C_Period_ID From C_Period pr  INNER JOIN c_year yr ON (yr.c_year_id = pr.c_year_id AND yr.c_calendar_id= " +
                "(CASE WHEN (SELECT NVL(C_Calendar_ID,0) FROM AD_Orginfo WHERE AD_org_ID =" + revenueRecognitionPlan.GetAD_Org_ID() + " ) =0 THEN (SELECT  NVL(C_Calendar_ID,0) FROM AD_ClientInfo WHERE AD_Client_ID =" + revenueRecognitionPlan.GetAD_Client_ID() + ") ELSE " +
                "(SELECT NVL(C_Calendar_ID,0) FROM AD_Orginfo WHERE AD_org_ID =" + revenueRecognitionPlan.GetAD_Org_ID() + ") END ) ) WHERE " + GlobalVariable.TO_DATE(Today, true) + " BETWEEN StartDate and EndDate";

            C_Period_ID = Util.GetValueOfInt(DB.ExecuteScalar(periodSql));
            journal.SetC_Period_ID(C_Period_ID);
            journal.SetC_Currency_ID(revenueRecognitionPlan.GetC_Currency_ID());
            int C_ConversionType_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select C_ConversionType_ID From C_ConversionType where IsDefault='Y'"));
            journal.SetC_ConversionType_ID(C_ConversionType_ID);
            journal.SetTotalCr(revenueRecognitionPlan.GetTotalAmt());
            journal.SetTotalDr(revenueRecognitionPlan.GetTotalAmt());
            journal.SetDocStatus("DR");
            journal.SetDocAction("CO");

            return journal;
        }

        /// <summary>
        /// Create Gl Journal Line
        /// </summary>
        /// <param name="Journal">GL Journal</param>
        /// <param name="Invoice">Invoice</param>
        /// <param name="InvoiceLine">Invoice Line</param>
        /// <param name="RevenueRecognitionPlan">Revenue Recognition Plan</param>
        /// <param name="TotalAmt">Sum of RecognizeAmt</param>
        /// <param name="RecognitionType">Recognition Type</param>
        /// <param name="k">loop variable</param>
        /// <returns>>Journal line object</returns>
        public MJournalLine GenerateJounalLine(MJournal Journal, MInvoice Invoice, MInvoiceLine InvoiceLine,
                                            MRevenueRecognitionPlan RevenueRecognitionPlan, Decimal TotalAmt, string RecognitionType, int k)
        {
            int combination_ID = 0;
            if (k == 0)
            {
                combination_ID = RevenueRun.GetCombinationID(InvoiceLine.GetM_Product_ID(), InvoiceLine.GetC_Charge_ID(), journal.GetC_AcctSchema_ID(), Invoice.IsSOTrx(), Invoice.IsReturnTrx(), totalAmt, RevenueRecognitionPlan.GetC_RevenueRecognition_ID());
                journalLine.SetLine(lineno);
                if (RecognitionType.Equals("E") && TotalAmt > 0)
                {
                    journalLine.SetAmtAcctDr(TotalAmt);
                    journalLine.SetAmtSourceDr(TotalAmt);
                    journalLine.SetAmtSourceCr(0);
                    journalLine.SetAmtAcctCr(0);
                }
                else if (RecognitionType.Equals("E") && TotalAmt < 0)
                {
                    journalLine.SetAmtAcctCr(Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceCr(Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceDr(0);
                    journalLine.SetAmtAcctDr(0);
                }
                else if (RecognitionType.Equals("R") && TotalAmt > 0)
                {
                    journalLine.SetAmtAcctCr(TotalAmt);
                    journalLine.SetAmtSourceCr(TotalAmt);
                    journalLine.SetAmtSourceDr(0);
                    journalLine.SetAmtAcctDr(0);
                }
                else if (RecognitionType.Equals("R") && TotalAmt < 0)
                {
                    journalLine.SetAmtAcctDr(Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceDr(Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceCr(0);
                    journalLine.SetAmtAcctCr(0);
                }
                int account_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Account_ID From C_ValidCombination Where C_ValidCombination_ID=" + combination_ID));
                journalLine.Set_ValueNoCheck("Account_ID", account_ID);
                journalLine.Set_ValueNoCheck("C_BPartner_ID", Invoice.GetC_BPartner_ID());
                journalLine.SetAD_OrgTrx_ID(InvoiceLine.Get_ColumnIndex("AD_OrgTrx_ID") > 0 ? InvoiceLine.GetAD_OrgTrx_ID() : Invoice.GetAD_OrgTrx_ID());
                journalLine.Set_ValueNoCheck("C_Project_ID", InvoiceLine.GetC_Project_ID() > 0 ? InvoiceLine.GetC_Project_ID() : Invoice.Get_Value("C_ProjectRef_ID"));
                journalLine.Set_ValueNoCheck("C_Campaign_ID", InvoiceLine.Get_ColumnIndex("C_Campaign_ID") > 0 ? InvoiceLine.GetC_Campaign_ID() : Invoice.GetC_Campaign_ID());
                journalLine.Set_ValueNoCheck("C_Activity_ID", InvoiceLine.Get_ColumnIndex("C_Activity_ID") > 0 ? InvoiceLine.GetC_Activity_ID() : Invoice.GetC_Activity_ID());
                journalLine.Set_ValueNoCheck("M_Product_ID", InvoiceLine.GetM_Product_ID());

            }
            else
            {
                combination_ID = RevenueRun.GetCombinationID(0, 0, RevenueRecognitionPlan.GetC_AcctSchema_ID(), Invoice.IsSOTrx(), Invoice.IsReturnTrx(), totalAmt, RevenueRecognitionPlan.GetC_RevenueRecognition_ID());
                int account_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Account_ID From C_ValidCombination Where C_ValidCombination_ID=" + combination_ID));

                journalLine = RevenueRun.GetOrCreate(journal, journalLine, InvoiceLine.GetM_Product_ID(), InvoiceLine.GetC_Charge_ID(),
                    InvoiceLine.Get_ColumnIndex("C_Campaign_ID") > 0 ? InvoiceLine.GetC_Campaign_ID() : Invoice.GetC_Campaign_ID(),
                account_ID, InvoiceLine.GetC_Project_ID() > 0 ? InvoiceLine.GetC_Project_ID() : Util.GetValueOfInt(Invoice.Get_Value("C_ProjectRef_ID")),
                InvoiceLine.Get_ColumnIndex("C_Activity_ID") > 0 ? InvoiceLine.GetC_Activity_ID() : Invoice.GetC_Activity_ID(), Invoice.GetC_BPartner_ID(),
                Invoice.GetAD_Org_ID(), InvoiceLine.Get_ColumnIndex("AD_OrgTrx_ID") > 0 ? InvoiceLine.GetAD_OrgTrx_ID() : Invoice.GetAD_OrgTrx_ID());

                journalLine.SetLine(lineno);

                if (RecognitionType.Equals("E") && TotalAmt > 0)
                {
                    journalLine.SetAmtAcctCr(journalLine.GetAmtAcctCr() + TotalAmt);
                    journalLine.SetAmtSourceCr(journalLine.GetAmtSourceCr() + TotalAmt);
                    journalLine.SetAmtSourceDr(0);
                    journalLine.SetAmtAcctDr(0);
                }
                else if (RecognitionType.Equals("E") && TotalAmt < 0)
                {
                    journalLine.SetAmtAcctDr(journalLine.GetAmtAcctDr() + Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceDr(journalLine.GetAmtSourceDr() + Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceCr(0);
                    journalLine.SetAmtAcctCr(0);
                }
                else if (RecognitionType.Equals("R") && TotalAmt > 0)
                {
                    journalLine.SetAmtAcctDr(journalLine.GetAmtAcctDr() + TotalAmt);
                    journalLine.SetAmtSourceDr(journalLine.GetAmtSourceDr() + TotalAmt);
                    journalLine.SetAmtSourceCr(0);
                    journalLine.SetAmtAcctCr(0);
                }
                else if (RecognitionType.Equals("R") && TotalAmt < 0)
                {
                    journalLine.SetAmtAcctCr(journalLine.GetAmtAcctCr() + Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceCr(journalLine.GetAmtSourceCr() + Decimal.Negate(TotalAmt));
                    journalLine.SetAmtSourceDr(0);
                    journalLine.SetAmtAcctDr(0);
                }
            }
            return journalLine;
        }
    }
}
