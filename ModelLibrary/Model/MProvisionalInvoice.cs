/********************************************************
 * Module Name    :    VA Framework
 * Class Name     :    MProvisionalInvoice
 * Purpose        :    Provisional Invoice Model
 * Employee Code  :    209
 * Date           :    14-July-2021
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvanatge.Model;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MProvisionalInvoice : X_C_ProvisionalInvoice, DocAction
    {
        #region Variables
        private static VLogger _log = VLogger.GetVLogger(typeof(MProvisionalInvoice).FullName);
        private MProvisionalInvoiceLine[] _lines;
        ValueNamePair pp = null;
        private bool _justPrepared = false;
        // Process Message
        private string _processMsg = null;
        /** Reversal Indicator			*/
        public const String REVERSE_INDICATOR = "^";
        //	Provisional Invoice Taxes	
        private MProvisionalInvoiceTax[] _taxes;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_ProvisionalInvoice_ID">ProvisionalInvoice</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoice(Ctx ctx, int C_ProvisionalInvoice_ID, Trx trxName) :
         base(ctx, C_ProvisionalInvoice_ID, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoice(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">NewrRecord</param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // If lines are available and user is changing the pricelist/conversiontype on header than we have to restrict it
            if (!newRecord && (Is_ValueChanged("M_PriceList_ID") || Is_ValueChanged("C_ConversionType_ID") || Is_ValueChanged("C_PaymentTerm_ID")))
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_ProvisionalInvoiceLine_ID) FROM C_ProvisionalInvoiceLine
                                WHERE C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID(), null, Get_Trx())) > 0)
                {
                    // Please Delete Lines First
                    log.SaveWarning("VIS_CantChange", "");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns></returns>
        public bool ApproveIt()
        {
            _log.Info(ToString());
            //SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Close Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            _log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public string PrepareIt()
        {
            _log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MProvisionalInvoiceLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public string CompleteIt()
        {
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

            MProvisionalInvoiceLine[] Lines = GetLines(false);
            MProvisionalInvoiceLine _line = null;
            for (int i = 0; i < Lines.Length; i++)
            {
                _line = Lines[i];

                // Costing Calculation
                if (client.IsCostImmediate() && _line.GetM_Product_ID() > 0)
                {
                    if (!CostingCalculation(client, _line, false))
                    {
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }

            //set processed true for all child tabs
            DB.ExecuteQuery("UPDATE C_ProvisionalInvoiceLine SET Processed = 'Y'  WHERE C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID(), null, Get_Trx());
            DB.ExecuteQuery("UPDATE C_ProvisionalInvoiceTax  SET Processed = 'Y'  WHERE C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID(), null, Get_Trx());

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// This function is used to calculate cost for Provisional Invoice
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="line">Provisional Invoice Line</param>
        /// <param name="callingbyProcess">method called from completion or process</param>
        /// <returns>true, when success</returns>
        public bool CostingCalculation(MClient client, MProvisionalInvoiceLine line, bool callingbyProcess)
        {
            String conversionNotFoundInvoice = String.Empty;

            // when QtyInvoiced ==0, then no need to calculate
            if (line != null && line.GetC_ProvisionalInvoice_ID() > 0 && line.GetQtyInvoiced() == 0)
                return true;

            // create product object
            MProduct product1 = new MProduct(GetCtx(), line.GetM_Product_ID(), Get_Trx());

            // when Product Type = Item, and having reference of OrderLine and InOutLine
            if (product1.GetProductType().Equals(MProduct.PRODUCTTYPE_Item) && line.GetM_InOutLine_ID() > 0 && line.GetC_OrderLine_ID() > 0)
            {
                // Is used to check costing method belongs to PO costing method  like (Average PO, Weighted Average PO or Last PO) or not
                bool isPOCostingMethod = MCostElement.IsPOCostingmethod(GetCtx(), line.GetAD_Client_ID(), product1.GetM_Product_ID(), product1.Get_Trx());

                // Get Product Cost
                Decimal ProductLineCost = line.GetProductLineCost(line, isPOCostingMethod);

                // calculate invoice line costing after calculating costing of linked MR line 
                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product1, line.GetM_AttributeSetInstance_ID(),
                      "ProvisionalInvoice", null, null, null, null, line, ProductLineCost, line.GetQtyInvoiced(),
                    Get_Trx(), out conversionNotFoundInvoice, optionalstr: (callingbyProcess ? "process" : "window")))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated"));
                    if ((client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory()) || callingbyProcess)
                    {
                        return false;
                    }
                }
                else
                {
                    //Update IsCostImmediate , IsCostCalculated, IsReversedCostCalculated  as True on Line
                    DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoiceLine SET IsCostImmediate = 'Y' 
                    " + (callingbyProcess ? ", IsCostCalculated = 'Y'" : "") + @""
                      + (callingbyProcess && IsReversal() ? ", IsReversedCostCalculated = 'Y'" : "") + @"
                    WHERE C_ProvisionalInvoiceLine_ID = " + line.GetC_ProvisionalInvoiceLine_ID(), null, Get_Trx());
                }
            }

            return true;
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            return null;
        }

        /// <summary>
        /// Approval Amount
        /// </summary>
        /// <returns>0</returns>
        public decimal GetApprovalAmt()
        {
            return 0;
        }

        /// <summary>
        /// Get Document's DocBaseType
        /// </summary>
        /// <returns>DocBaseType</returns>
        public string GetDocBaseType()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetDocBaseType();
        }

        /// <summary>
        /// Get Document Date
        /// </summary>
        /// <returns>Trx Date</returns>
        public DateTime? GetDocumentDate()
        {
            return GetDateAcct();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public string GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>null</returns>
        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public string GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Set Process Message
        /// </summary>
        /// <param name="ProcessMsg">Process Message</param>
        public void SetProcessMsg(String ProcessMsg)
        {
            _processMsg = ProcessMsg;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            _log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Process Document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(string processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            _log.Info(ToString());
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            _log.Info(ToString());
            // SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            _log.Info(ToString());
            return false;
        }

        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            _log.Info(ToString());

            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }

            // if Payment Refrence exist, then no to void the record
            if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_Payment_ID) FROM C_Payment 
                WHERE DocStatus NOT IN ('RE', 'VO') AND C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID(), null, Get_Trx())) > 0)
            {
                _processMsg = Msg.GetMsg(GetCtx(), "DeletePaymentFirst");
                return false;
            }

            MProvisionalInvoice reversal = new MProvisionalInvoice(GetCtx(), 0, Get_Trx());
            PO.CopyValues(this, reversal, GetAD_Client_ID(), GetAD_Org_ID());

            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);	//	indicate reversals
            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetPosted(false);
            reversal.SetProcessed(false);
            reversal.SetGrandTotal(Decimal.Negate(GetGrandTotal()));
            reversal.SetTotalLines(Decimal.Negate(GetTotalLines()));
            // set Reversal property for identifying, record is reversal or not during saving or for other actions
            reversal.SetIsReversal(true);
            //Set Orignal Document Reference
            reversal.SetReversalDoc_ID(GetC_ProvisionalInvoice_ID());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (!reversal.Save())
            {
                pp = VLogger.RetrieveError();
                string val = pp.GetName();
                if (String.IsNullOrEmpty(val))
                {
                    val = pp.GetValue();
                }
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = Msg.GetMsg(GetCtx(), "NotCreateReversalProvisionalInvoice") + ", " + val;
                else
                    _processMsg = Msg.GetMsg(GetCtx(), "NotCreateReversalProvisionalInvoice");
                return false;
            }
            else
            {
                MProvisionalInvoiceLine oLine = null;
                MProvisionalInvoiceLine rLine = null;

                #region Copy Lines
                MProvisionalInvoiceLine[] Lines = GetLines(false);
                for (int i = 0; i < Lines.Length; i++)
                {
                    oLine = Lines[i];
                    rLine = new MProvisionalInvoiceLine(GetCtx(), 0, Get_Trx());
                    CopyValues(oLine, rLine, oLine.GetAD_Client_ID(), oLine.GetAD_Org_ID());
                    rLine.Set_ValueNoCheck("C_ProvisionalInvoice_ID", reversal.GetC_ProvisionalInvoice_ID());
                    rLine.SetLine(oLine.GetLine());
                    rLine.SetM_InOutLine_ID(oLine.GetM_InOutLine_ID());
                    rLine.SetC_OrderLine_ID(oLine.GetC_OrderLine_ID());
                    rLine.SetQtyEntered(Decimal.Negate(oLine.GetQtyEntered()));
                    rLine.SetQtyInvoiced(Decimal.Negate(oLine.GetQtyInvoiced()));
                    rLine.SetTaxAmt(Decimal.Negate(oLine.GetTaxAmt()));
                    rLine.SetSurchargeAmt(Decimal.Negate(oLine.GetSurchargeAmt()));
                    rLine.SetLineTotalAmt(Decimal.Negate(oLine.GetLineTotalAmt()));
                    rLine.SetLineNetAmt(Decimal.Negate(oLine.GetLineNetAmt()));
                    rLine.SetProcessed(false);

                    // Set Original Line reference
                    rLine.SetReversalDoc_ID(oLine.GetC_ProvisionalInvoiceLine_ID());

                    if (!rLine.Save())
                    {
                        pp = VLogger.RetrieveError();
                        string val = pp.GetName();
                        if (String.IsNullOrEmpty(val))
                        {
                            val = pp.GetValue();
                        }
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = Msg.GetMsg(GetCtx(), "NotCreateReversalProvisionalInvoice") + ", " + val;
                        else
                            _processMsg = Msg.GetMsg(GetCtx(), "NotCreateReversalProvisionalInvoice");
                        return false;
                    }
                }
                #endregion

                // Calculate Provisonal Invoice tax
                reversal.CalculateTaxTotal();

            }
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }

            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.SetProcessing(false);
            reversal.Save(Get_Trx());

            //show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reversal.GetDocumentNo();

            //	Update Reversed (this)
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");

            SetProcessed(true);
            SetProcessing(false);
            SetDocStatus(DOCSTATUS_Reversed);   //	may come from void
            SetDocAction(DOCACTION_None);


            return true;
        }

        /// <summary>
        /// Calculate Tax for all Provisional Lines
        /// </summary>
        /// <returns>true, when success</returns>
        public bool CalculateTaxTotal()
        {
            log.Fine(" Start CalculateTaxTotal fpr Provisional Invoice");
            try
            {
                //	Delete Taxes
                DataBase.DB.ExecuteQuery(@"DELETE FROM C_ProvisionalInvoiceTax 
                WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID(), null, Get_Trx());

                //
                _taxes = null;
                bool istaxinclude = false;

                // Get Provisional Invoice Line
                DataSet dsInvoiceLine = DB.ExecuteDataset(@"SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , 
                                            i.C_Currency_ID , i.DateAcct , i.C_ConversionType_ID , il.C_ProvisionalInvoice_ID, il.C_Tax_ID 
                                           FROM C_ProvisionalInvoiceLine il 
                                           INNER JOIN C_ProvisionalInvoice i ON (il.C_ProvisionalInvoice_ID=i.C_ProvisionalInvoice_ID) 
                                           WHERE il.C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID(), null, Get_Trx());

                //	Lines
                Decimal totalLines = Env.ZERO;
                List<int> taxList = new List<int>();
                MProvisionalInvoiceLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MProvisionalInvoiceLine line = lines[i];
                    if (i == 0)
                    {
                        istaxinclude = line.IsTaxIncluded();
                    }

                    int taxID = (int)line.GetC_Tax_ID();
                    if (!taxList.Contains(taxID))
                    {
                        MProvisionalInvoiceTax iTax = MProvisionalInvoiceTax.Get(line, GetPrecision(),
                            false, Get_Trx());	//	current Tax
                        if (iTax != null)
                        {
                            if (!iTax.CalculateTaxFromLines(dsInvoiceLine))
                                return false;
                            if (!iTax.Save())
                                return false;
                            taxList.Add(taxID);

                            // if Surcharge Tax is selected then calculate Tax for this Surcharge Tax.
                            if (line.Get_ColumnIndex("SurchargeAmt") > 0)
                            {
                                iTax = MProvisionalInvoiceTax.GetSurcharge(line, GetPrecision(), false, Get_Trx());  //	current Tax
                                if (iTax != null)
                                {
                                    if (!iTax.CalculateSurchargeFromLines())
                                        return false;
                                    if (!iTax.Save(Get_Trx()))
                                        return false;
                                }
                            }
                        }
                    }
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }

                //	Taxes
                Decimal grandTotal = totalLines;
                MProvisionalInvoiceTax[] taxes = GetTax(true);
                for (int i = 0; i < taxes.Length; i++)
                {
                    MProvisionalInvoiceTax iTax = taxes[i];
                    MTax tax = iTax.GetTax();
                    if (tax.IsSummary())
                    {
                        MTax[] cTaxes = tax.GetChildTaxes(false);	//	Multiple taxes
                        for (int j = 0; j < cTaxes.Length; j++)
                        {
                            MTax cTax = cTaxes[j];
                            Decimal taxAmt = cTax.CalculateTax(iTax.GetTaxAbleAmt(), false, GetPrecision());
                            //
                            if (taxList.Contains(cTax.GetC_Tax_ID()))
                            {
                                String sql = @"SELECT * FROM C_ProvisionalInvoiceTax 
                                WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID() + " AND C_Tax_ID=" + cTax.GetC_Tax_ID();
                                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                                if (ds != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow dr = ds.Tables[0].Rows[0];
                                    MProvisionalInvoiceTax newITax = new MProvisionalInvoiceTax(GetCtx(), dr, Get_Trx());
                                    newITax.SetTaxAmt(Decimal.Add(newITax.GetTaxAmt(), taxAmt));
                                    newITax.SetTaxAbleAmt(Decimal.Add(newITax.GetTaxAbleAmt(), iTax.GetTaxAbleAmt()));
                                    //if (newITax.Get_ColumnIndex("TaxBaseCurrencyAmt") > 0)
                                    //{
                                    //    Decimal baseTaxAmt = taxAmt;
                                    //    int primaryAcctSchemaCurrency_ = GetCtx().GetContextAsInt("$C_Currency_ID");
                                    //    if (GetC_Currency_ID() != primaryAcctSchemaCurrency_)
                                    //    {
                                    //        baseTaxAmt = MConversionRate.Convert(GetCtx(), taxAmt, primaryAcctSchemaCurrency_, GetC_Currency_ID(),
                                    //                                                                   GetDateAcct(), GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                    //    }
                                    //    newITax.SetTaxBaseCurrencyAmt(Decimal.Add(newITax.GetTaxBaseCurrencyAmt(), baseTaxAmt));
                                    //}
                                    if (!newITax.Save(Get_Trx()))
                                    {
                                        return false;
                                    }
                                }
                                ds = null;
                            }
                            else
                            {
                                MProvisionalInvoiceTax newITax = new MProvisionalInvoiceTax(GetCtx(), 0, Get_Trx());
                                newITax.SetClientOrg(this);
                                newITax.SetC_ProvisionalInvoice_ID(GetC_ProvisionalInvoice_ID());
                                newITax.SetC_Tax_ID(cTax.GetC_Tax_ID());
                                newITax.SetPrecision(GetPrecision());
                                newITax.SetIsTaxIncluded(istaxinclude);
                                newITax.SetTaxAbleAmt(iTax.GetTaxAbleAmt());
                                newITax.SetTaxAmt(taxAmt);
                                //Set Tax Amount (Base Currency) on Invoice Tax Window //Arpit--8 Jan,2018 Puneet 
                                //if (newITax.Get_ColumnIndex("TaxBaseCurrencyAmt") > 0)
                                //{
                                //    decimal? baseTaxAmt = taxAmt;
                                //    int primaryAcctSchemaCurrency_ = GetCtx().GetContextAsInt("$C_Currency_ID");
                                //    if (GetC_Currency_ID() != primaryAcctSchemaCurrency_)
                                //    {
                                //        baseTaxAmt = MConversionRate.Convert(GetCtx(), taxAmt, primaryAcctSchemaCurrency_, GetC_Currency_ID(),
                                //                                                                   GetDateAcct(), GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                //    }
                                //    newITax.Set_Value("TaxBaseCurrencyAmt", baseTaxAmt);
                                //}
                                if (!newITax.Save(Get_Trx()))
                                    return false;
                            }
                            //
                            if (!istaxinclude)
                                grandTotal = Decimal.Add(grandTotal, taxAmt);
                        }
                        if (!iTax.Delete(true, Get_Trx()))
                            return false;
                    }
                    else
                    {
                        if (!istaxinclude)
                            grandTotal = Decimal.Add(grandTotal, iTax.GetTaxAmt());
                    }
                }

                // Update Header 
                DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoice SET GrandTotal=" + grandTotal +
                    ", TotalLines = " + totalLines + @" WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID(), null, Get_Trx());

            }
            catch (Exception e)
            {
                log.Severe(" Exception " + e.Message);
                return false;
            }
            log.Fine(" End CalculateTaxTotal fpr Provisional Invoice");
            return true;
        }

        /// <summary>
        /// Get Standard Precision
        /// </summary>
        /// <returns>Precision Value</returns>
        public int GetPrecision()
        {
            return MCurrency.GetStdPrecision(GetCtx(), GetC_Currency_ID());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true/false</returns>
        public bool UnlockIt()
        {
            _log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            _log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }
            else if (DOCSTATUS_Completed.Equals(GetDocStatus()))
            {
                ReverseCorrectIt();
            }
            else if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus()))
            {
                MProvisionalInvoiceLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MProvisionalInvoiceLine line = lines[i];
                    Decimal old = line.GetQtyInvoiced();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetQtyEntered(Env.ZERO);
                        line.SetQtyInvoiced(Env.ZERO);
                        line.SetTaxAmt(Env.ZERO);
                        line.SetLineNetAmt(Env.ZERO);
                        line.SetLineTotalAmt(Env.ZERO);
                    }

                    line.SetProcessed(true);
                    if (!line.Save(Get_Trx()))
                    {
                        pp = VLogger.RetrieveError();
                        string error = string.Empty;
                        if (pp != null)
                        {
                            error = pp.GetValue();
                            if (string.IsNullOrEmpty(error))
                            {
                                error = pp.GetName();
                                if (string.IsNullOrEmpty(error))
                                {
                                    error = Msg.GetMsg(GetCtx(), "ProvisionalInvoiceNotSave");
                                }
                            }
                        }
                        _processMsg = error;
                        return false;
                    }
                }

                AddDescription(Msg.GetMsg(GetCtx(), "Voided"));
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Provisional Invoice Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>lines</returns>
        public MProvisionalInvoiceLine[] GetLines(bool requery)
        {
            if (_lines == null || _lines.Length == 0 || requery)
                _lines = GetLines(null);
            return _lines;
        }

        /// <summary>
        /// Get Provisional Invoice Lines
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>lines</returns>
        private MProvisionalInvoiceLine[] GetLines(String whereClause)
        {
            List<MProvisionalInvoiceLine> list = new List<MProvisionalInvoiceLine>();
            String sql = "SELECT * FROM C_ProvisionalInvoiceLine WHERE C_ProvisionalInvoice_ID= " + GetC_ProvisionalInvoice_ID();
            if (whereClause != null)
                sql += whereClause;
            sql += " ORDER BY Line";
            try
            {
                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MProvisionalInvoiceLine il = new MProvisionalInvoiceLine(GetCtx(), dr, Get_Trx());
                    list.Add(il);
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getLines", e);
            }

            MProvisionalInvoiceLine[] lines = new MProvisionalInvoiceLine[list.Count];
            lines = list.ToArray();
            return lines;
        }

        /// <summary>
        /// Get Provisional Invoice Tax
        /// </summary>
        /// <param name="requery"></param>
        /// <returns>Array of Records</returns>
        public MProvisionalInvoiceTax[] GetTax(bool requery)
        {
            if (_taxes != null && !requery)
                return _taxes;

            List<MProvisionalInvoiceTax> list = new List<MProvisionalInvoiceTax>();
            String sql = "SELECT * FROM C_ProvisionalInvoiceTax WHERE C_ProvisionalInvoice_ID= " + GetC_ProvisionalInvoice_ID();

            try
            {
                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MProvisionalInvoiceTax iTax = new MProvisionalInvoiceTax(GetCtx(), dr, Get_Trx());
                    list.Add(iTax);
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getLines", e);
            }

            MProvisionalInvoiceTax[] Tax = new MProvisionalInvoiceTax[list.Count];
            Tax = list.ToArray();
            return Tax;
        }

    }
}
