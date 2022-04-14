/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRequisition
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     07-July-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;
using System.Reflection;
using ModelLibrary.Classes;

namespace VAdvantage.Model
{
    public class MRequisition : X_M_Requisition, DocAction
    {
        //	Process Message 		
        private String _processMsg = null;
        //	Just Prepared Flag		
        private bool _justPrepared = false;
        //Lines						
        private MRequisitionLine[] _lines = null;
        MStorage storage = null;
        MStorage Swhstorage = null;

        String _budgetMessage = String.Empty;
        private string _budgetNotDefined = string.Empty;

        /**
        * 	Standard Constructor
        *	@param Ctx context
        *	@param M_Requisition_ID id
        */
        public MRequisition(Ctx ctx, int M_Requisition_ID, Trx trxName)
            : base(ctx, M_Requisition_ID, trxName)
        {
            try
            {

                if (M_Requisition_ID == 0)
                {
                    //	setDocumentNo (null);
                    //	setAD_User_ID (0);
                    //	setM_PriceList_ID (0);
                    //	setM_Warehouse_ID(0);
                    //SetDateDoc(new Timestamp(System.currentTimeMillis()));
                    SetDateDoc(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                    //SetDateDoc(Convert.ToDateTime(DateTime.Now));
                    //setDateRequired(new Timestamp(System.currentTimeMillis()));
                    SetDateRequired(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                    //SetDateRequired(Convert.ToDateTime(DateTime.Now));
                    SetDocAction(DocActionVariables.ACTION_COMPLETE);	// CO
                    SetDocStatus(DocActionVariables.STATUS_DRAFTED);		// DR
                    SetPriorityRule(PRIORITYRULE_Medium);	// 5
                    SetTotalLines(Env.ZERO);
                    SetIsApproved(false);
                    SetPosted(false);
                    SetProcessed(false);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MRequisition--Standard Constructor");
                log.Severe(ex.ToString());
            }
        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         */
        public MRequisition(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /**
         * 	Get Lines
         *	@return array of lines
         */
        public MRequisitionLine[] GetLines()
        {
            try
            {
                if (_lines != null)
                {
                    return _lines;
                }

                List<MRequisitionLine> list = new List<MRequisitionLine>();
                String sql = "SELECT * FROM M_RequisitionLine WHERE M_Requisition_ID=" + GetM_Requisition_ID() + " ORDER BY Line";
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new MRequisitionLine(GetCtx(), dr, Get_TrxName()));
                    }
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, "getLines", e);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    dt = null;
                }
                _lines = new MRequisitionLine[list.Count];
                _lines = list.ToArray();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MRequisition--GetLines");
                log.Severe(ex.ToString());
            }
            return _lines;
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRequisition[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",Status=").Append(GetDocStatus()).Append(",Action=").Append(GetDocAction())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info
         */
        public String GetDocumentInfo()
        {
            //return Msg.getElement(getContext(), "M_Requisition_ID") + " " + getDocumentNo();
            return Msg.GetElement(GetCtx(), "M_Requisition_ID") + " " + GetDocumentNo();
        }

        /**
         * 	Create PDF
         *	@return File or null
         */
        public FileInfo CreatePDF()
        {
            //try
            //{
            //    File temp = File.createTempFile(get_TableName() + get_ID() + "_", ".pdf");
            //    return createPDF(temp);
            //}
            //catch (Exception e)
            //{
            //    log.severe("Could not create PDF - " + e.getMessage());
            //}
            //Create a file to write to.
            return null;
        }


        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            /*	ReportEngine re = ReportEngine.get (getContext(), ReportEngine.INVOICE, getC_Invoice_ID());
                if (re == null)*/
            return null;
            //return file;
            /*	return re.getPDF(file);*/
        }

        /**
         * 	Set default PriceList
         */
        public void SetM_PriceList_ID()
        {
            try
            {
                MPriceList defaultPL = MPriceList.GetDefault(GetCtx(), false);
                if (defaultPL == null)
                {
                    defaultPL = MPriceList.GetDefault(GetCtx(), true);
                }
                if (defaultPL != null)
                {
                    SetM_PriceList_ID(defaultPL.GetM_PriceList_ID());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MRequisition--SetM_PriceList_ID");
                log.Severe(ex.ToString());
            }
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            // VIS0060: If lines are available, it will not allow the user to change header information.
            if (!newRecord && (Is_ValueChanged("M_PriceList_ID") || Is_ValueChanged("M_Warehouse_ID") || Is_ValueChanged("DTD001_MWarehouseSource_ID")))
            {
                string sql = "SELECT COUNT(M_RequisitionLine_ID) FROM M_RequisitionLine WHERE M_Requisition_ID=" + GetM_Requisition_ID() + " AND IsActive='Y'";
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                {
                    log.SaveWarning("pleaseDeleteLinesFirst", "");
                    return false;
                }
            }

            if (GetM_PriceList_ID() == 0)
            {
                SetM_PriceList_ID();
            }

            // VIS0060: Show message if Default Price List is not found.
            if (GetM_PriceList_ID() == 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_NoDefaultPriceList"));
                return false;
            }
            return true;
        }

        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
        {
            log.Info("invalidateIt - " + ToString());
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            try
            {
                log.Info(ToString());
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
                if (_processMsg != null)
                    return DocActionVariables.STATUS_INVALID;

                MRequisitionLine[] lines = GetLines();

                // JID_0901: Added by Bharat on 30 Jan 2019 gives message when there are no line on requisition
                if (lines.Length == 0)
                {
                    _processMsg = "@NoLines@";
                    return DocActionVariables.STATUS_INVALID;
                }

                //	Invalid
                if (GetAD_User_ID() == 0
                    || GetM_PriceList_ID() == 0
                    || GetM_Warehouse_ID() == 0)
                    return DocActionVariables.STATUS_INVALID;



                //	Std Period open?
                if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION, GetAD_Org_ID()))
                {
                    _processMsg = "@PeriodClosed@";
                    return DocActionVariables.STATUS_INVALID;
                }

                // is Non Business Day?
                // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
                if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateDoc(), GetAD_Org_ID()))
                {
                    _processMsg = Common.Common.NONBUSINESSDAY;
                    return DocActionVariables.STATUS_INVALID;
                }



                //	Add up Amounts
                int precision = MPriceList.GetStandardPrecision(GetCtx(), GetM_PriceList_ID());
                Decimal totalLines = Env.ZERO;
                for (int i = 0; i < lines.Length; i++)
                {
                    MRequisitionLine line = lines[i];
                    Decimal lineNet = Decimal.Multiply(line.GetQty(), line.GetPriceActual());
                    lineNet = Decimal.Round(lineNet, precision);//, MidpointRounding.AwayFromZero);
                    if (lineNet.CompareTo(line.GetLineNetAmt()) != 0)
                    {
                        line.SetLineNetAmt(lineNet);
                        line.Save();
                    }
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }
                if (totalLines.CompareTo(GetTotalLines()) != 0)
                {
                    SetTotalLines(totalLines);
                    Save();
                }
                _justPrepared = true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MRequisition--PrepareIt");
                log.Severe(ex.ToString());
            }
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /**
         * 	Complete Document
         * 	@return new status (Complete, In Progress, Invalid, Waiting ..)
         */
        public String CompleteIt()
        {
            try
            {
                //	Re-Check
                if (!_justPrepared)
                {
                    String status = PrepareIt();
                    if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                        return status;
                }

                // Set Document Date based on setting on Document Type
                SetCompletedDocumentDate();

                if (Env.IsModuleInstalled("FRPT_"))
                {
                    // Once budget breach approved do not check budget breach functionality again
                    if (!IsBudgetBreachApproved())
                    {
                        // budget control functionality work when Financial Managemt Module Available
                        try
                        {
                            log.Info("Budget Control Start for Rquisition Document No  " + GetDocumentNo());
                            EvaluateBudgetControlData();
                            // If Budget Exceeded or Not Defined By Rakesh Kumar 29/Apr/2021
                            if (_budgetMessage.Length > 0 || _budgetNotDefined.Length > 0)
                            {
                                // Done by Rakesh Kumar On 29/Apr/2021
                                // When budget exceeded
                                if (_budgetMessage.Length > 0)
                                {
                                    _processMsg = Msg.GetMsg(GetCtx(), "BudgetExceedFor") + _budgetMessage;
                                }
                                if (_budgetNotDefined.Length > 0)
                                {
                                    _processMsg = _processMsg + "" + Msg.GetMsg(GetCtx(), "BudgetNotDefinedFor") + _budgetNotDefined;
                                }
                                SetProcessed(false);
                                // Set Budget Breach only in case when budget exceeded
                                // Done by Rakesh 29/Apr/2021
                                if (_budgetMessage.Length > 0 && _budgetNotDefined.Length == 0)
                                {
                                    // Done by rakesh 19/Feb/2020
                                    SetIsBudgetBreach(true);
                                    SetIsBudgetBreachApproved(false);
                                }
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                            SetIsBudgetBreach(false);
                            SetIsBudgetBreachApproved(true);
                            log.Info("Budget Control Completed for Rquisition Document No  " + GetDocumentNo());
                        }
                        catch (Exception ex)
                        {
                            log.Severe("Budget Control Issue " + ex.Message);
                            SetProcessed(false);
                            SetIsBudgetBreach(false);
                            return DocActionVariables.STATUS_INPROGRESS;
                        }
                    }
                }

                //	Implicit Approval
                if (!IsApproved())
                {
                    ApproveIt();
                }
                log.Info(ToString());

                //	User Validation
                String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                if (valid != null)
                {
                    _processMsg = valid;
                    return DocActionVariables.STATUS_INVALID;
                }

                if (Env.IsModuleInstalled("DTD001_"))
                {
                    MRequisitionLine[] lines = GetLines();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        MRequisitionLine line = lines[i];
                        MProduct product = null;
                        if (line.GetM_Product_ID() > 0)
                            product = MProduct.Get(GetCtx(), line.GetM_Product_ID());

                        int loc_id = GetLocation(GetM_Warehouse_ID());
                        //new 6jan 1
                        int Sourcewhloc_id = GetSwhLocation(GetDTD001_MWarehouseSource_ID());
                        if (Sourcewhloc_id == 0)
                        {       // JID_1098: done by Bharat on 31 Jan 2019, need to correct these messages
                            _processMsg = Msg.GetMsg(GetCtx(), "DTD001_DefineSrcLocator"); //"Define Locator For That SourceWarehouse";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //End
                        if (loc_id == 0)
                        {
                            //return Msg.GetMsg(GetCtx(),"MMPM_DefineLocator");
                            _processMsg = Msg.GetMsg(GetCtx(), "DTD001_DefineLocator"); //"Define Locator For That Warehouse";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        Decimal difference = 0;

                        if (line.Get_ColumnIndex("QtyReserved") > 0)
                        {
                            difference = Decimal.Subtract(line.GetQty(), line.GetQtyReserved());
                        }
                        else
                        {
                            difference = line.GetQty();
                        }

                        if (line.Get_ColumnIndex("OrderLocator_ID") > 0)
                        {
                            line.SetOrderLocator_ID(loc_id);
                            line.SetReserveLocator_ID(Sourcewhloc_id);

                            if (line.Get_ColumnIndex("QtyReserved") > 0)
                            {
                                line.SetQtyReserved(Decimal.Add(line.GetQtyReserved(), difference));
                            }
                            if (!line.Save())
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "ReqLineNotSaved");
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        // SI_0686_2 :  storage should not update in case of product is other than item type.
                        if (product != null && product.GetProductType() == X_M_Product.PRODUCTTYPE_Item && product.IsStocked())
                        {
                            // SI_0657: consider Attribute also
                            storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            if (storage == null)
                            {
                                //MStorage.Add(GetCtx(), GetM_Warehouse_ID(), loc_id, line.GetM_Product_ID(), 0, 0, 0, 0, line.GetQty(), null);
                                MStorage.Add(GetCtx(), GetM_Warehouse_ID(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                    line.GetM_AttributeSetInstance_ID(), (Decimal)0, (Decimal)0, (Decimal)0, difference, Get_Trx());
                            }
                            else
                            {
                                storage.SetDTD001_QtyReserved(Decimal.Add(storage.GetDTD001_QtyReserved(), difference));
                                storage.Save();
                            }

                            Swhstorage = MStorage.Get(GetCtx(), Sourcewhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            if (Swhstorage == null)
                            {
                                MStorage.Add(GetCtx(), GetDTD001_MWarehouseSource_ID(), Sourcewhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                    line.GetM_AttributeSetInstance_ID(), (Decimal)0, (Decimal)0, (Decimal)0, 0, Get_Trx());
                                MStorage StrgResrvQty = null;
                                StrgResrvQty = MStorage.GetCreate(GetCtx(), Sourcewhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                StrgResrvQty.SetDTD001_SourceReserve(Decimal.Add(StrgResrvQty.GetDTD001_SourceReserve(), difference));
                                StrgResrvQty.Save();
                            }
                            else
                            {
                                Swhstorage.SetDTD001_SourceReserve(Decimal.Add(Swhstorage.GetDTD001_SourceReserve(), difference));
                                Swhstorage.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MRequisition--CompleteIt");
                log.Severe(ex.ToString());
                _processMsg = ex.Message.ToString();
                return DocActionVariables.STATUS_INVALID;
            }

            // Set the document number from completed document sequence after completed (if needed)
            SetCompletedDocumentNo();
            SetProcessed(true);
            SetDocAction(DocActionVariables.ACTION_CLOSE);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MSequence.GetDocumentNo(GetC_DocType_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        /// Overwrite the document date based on setting on Document Type
        /// </summary>
        private void SetCompletedDocumentDate()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateDoc(DateTime.Now.Date);

                //	Std Period open?
                if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), dt.GetDocBaseType(), GetAD_Org_ID()))
                {
                    throw new Exception("@PeriodClosed@");
                }
            }
        }

        /// <summary>
        /// This function is used to check, document is budget control or not.
        /// </summary>
        /// <returns>True, when budget controlled or not applicable</returns>
        private bool EvaluateBudgetControlData()
        {
            DataSet dsRecordData;
            DataRow[] drRecordData = null;
            DataRow[] drBudgetControl = null;
            DataSet dsBudgetControlDimension;
            DataRow[] drBudgetControlDimension = null;
            List<BudgetControl> _budgetControl = new List<BudgetControl>();
            StringBuilder sql = new StringBuilder();
            BudgetCheck budget = new BudgetCheck();

            sql.Clear();
            sql.Append(@"SELECT GL_Budget.GL_Budget_ID , GL_Budget.BudgetControlBasis, GL_Budget.C_Year_ID , GL_Budget.C_Period_ID,GL_Budget.Name As BudgetName, 
                  GL_BudgetControl.C_AcctSchema_ID, GL_BudgetControl.CommitmentType, GL_BudgetControl.BudgetControlScope,  GL_BudgetControl.GL_BudgetControl_ID, GL_BudgetControl.Name AS ControlName,GL_BudgetControl.BudgetBreachPercent
                FROM GL_Budget INNER JOIN GL_BudgetControl ON GL_Budget.GL_Budget_ID = GL_BudgetControl.GL_Budget_ID
                INNER JOIN Ad_ClientInfo ON Ad_ClientInfo.AD_Client_ID = GL_Budget.AD_Client_ID
                WHERE GL_BudgetControl.IsActive = 'Y' AND GL_Budget.IsActive = 'Y' AND GL_BudgetControl.AD_Org_ID IN (0 , " + GetAD_Org_ID() + @")  
                   AND GL_BudgetControl.CommitmentType IN ('B' ) AND 
                  (( GL_Budget.BudgetControlBasis = 'P' AND GL_Budget.C_Period_ID =
                  (SELECT C_Period.C_Period_ID FROM C_Period INNER JOIN c_year ON c_year.c_year_ID      = C_Period.c_year_ID
                  WHERE C_Period.IsActive  = 'Y'  AND c_year.C_Calendar_ID = Ad_ClientInfo.C_Calendar_ID
                  AND " + GlobalVariable.TO_DATE(GetDateDoc(), true) + @" BETWEEN C_Period.startdate AND C_Period.enddate )) 
                OR ( GL_Budget.BudgetControlBasis = 'A' AND GL_Budget.C_Year_ID =
                  (SELECT C_Period.C_Year_ID FROM C_Period INNER JOIN c_year ON c_year.c_year_ID = C_Period.c_year_ID
                  WHERE C_Period.IsActive  = 'Y'   AND c_year.C_Calendar_ID = Ad_ClientInfo.C_Calendar_ID  
                AND " + GlobalVariable.TO_DATE(GetDateDoc(), true) + @" BETWEEN C_Period.startdate AND C_Period.enddate   ) ) ) 
                AND (SELECT COUNT(fact_acct_id) FROM fact_acct
                WHERE gl_budget_id = GL_Budget.GL_Budget_ID
                AND (c_period_id  IN ( NVL(GL_Budget.C_Period_ID ,0 ))
                OR c_period_id    IN (SELECT C_Period_ID FROM C_Period   WHERE C_Year_ID = NVL(GL_Budget.C_Year_ID , 0) ) ) ) > 0");
            DataSet dsBudgetControl = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsBudgetControl != null && dsBudgetControl.Tables.Count > 0 && dsBudgetControl.Tables[0].Rows.Count > 0)
            {
                // get budget control ids
                object[] budgetControlIds = dsBudgetControl.Tables[0].AsEnumerable().Select(r => r.Field<object>("GL_BUDGETCONTROL_ID")).ToArray();
                string result = string.Join(",", budgetControlIds);
                dsBudgetControlDimension = budget.GetBudgetDimension(result);

                // get record posting data 
                dsRecordData = BudgetControlling();
                if (dsRecordData != null && dsRecordData.Tables.Count > 0)
                {
                    // datarows of Debit values which to be controlled
                    drRecordData = dsRecordData.Tables[0].Select("Debit > 0 ", " Account_ID ASC");
                    if (drRecordData != null)
                    {
                        // loop on PO record data which is to be debited only 
                        for (int i = 0; i < drRecordData.Length; i++)
                        {
                            // datarows of Budget, of selected accouting schema
                            drBudgetControl = dsBudgetControl.Tables[0].Select("C_AcctSchema_ID  = " + Util.GetValueOfInt(drRecordData[i]["C_AcctSchema_ID"]));

                            // loop on Budget which to be controlled 
                            if (drBudgetControl != null)
                            {
                                for (int j = 0; j < drBudgetControl.Length; j++)
                                {
                                    // get budget Dimension datarow 
                                    drBudgetControlDimension = dsBudgetControlDimension.Tables[0].Select("GL_BudgetControl_ID  = "
                                                                + Util.GetValueOfInt(drBudgetControl[j]["GL_BudgetControl_ID"]));

                                    // get BUdgeted Controlled Value based on dimension
                                    _budgetControl = budget.GetBudgetControlValue(drRecordData[i], drBudgetControl[j], drBudgetControlDimension,
                                        GetDateDoc(), _budgetControl, Get_Trx(), 'R', 0);

                                    // Reduce amount from Budget controlled value
                                    _budgetControl = ReduceAmountFromBudget(drRecordData[i], drBudgetControl[j], drBudgetControlDimension, _budgetControl);

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //  no recod found for budget control 
                return true;
            }
            return true;
        }

        /// <summary>
        /// This Function is used to get data based on Posting Logic, which is to be posted after completion.
        /// </summary>
        /// <returns>DataSet of Posting Records</returns>
        private DataSet BudgetControlling()
        {
            int ad_window_id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Window_ID FROM AD_Window WHERE  Export_ID = 'VIS_322'"));
            DataSet result = new DataSet();
            Type type = null;
            MethodInfo methodInfo = null;
            string className = "FRPTSvc.Controllers.PostAccLocalizationVO";
            type = ClassTypeContainer.GetClassType(className, "FRPTSvc");
            if (type != null)
            {
                methodInfo = type.GetMethod("BudgetControlled");
                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == 8)
                    {
                        object[] parametersArray = new object[] { GetCtx(),
                                                                Util.GetValueOfInt(GetAD_Client_ID()),
                                                                Util.GetValueOfInt(X_M_Requisition.Table_ID),//MTable.Get(GetCtx() , "M_Requisition").GetAD_Table_ID()
                                                                Util.GetValueOfInt(GetM_Requisition_ID()),
                                                                true,
                                                                Util.GetValueOfInt(GetAD_Org_ID()),
                                                                ad_window_id,
                                                                Util.GetValueOfInt(GetC_DocType_ID()) };
                        result = (DataSet)methodInfo.Invoke(null, parametersArray);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// This Function is used to Reduce From Budget controlled amount
        /// </summary>
        /// <param name="drDataRecord">document Posting Record</param>
        /// <param name="drBUdgetControl">BUdget Control information</param>
        /// <param name="drBudgetComtrolDimension">Budget Control dimension which is applicable</param>
        /// <param name="_listBudgetControl">list of Budget controls</param>
        /// <returns>modified list Budget Control</returns>
        public List<BudgetControl> ReduceAmountFromBudget(DataRow drDataRecord, DataRow drBUdgetControl, DataRow[] drBudgetComtrolDimension, List<BudgetControl> _listBudgetControl)
        {
            BudgetControl _budgetControl = null;
            List<String> selectedDimension = new List<string>();
            if (drBudgetComtrolDimension != null)
            {
                for (int i = 0; i < drBudgetComtrolDimension.Length; i++)
                {
                    selectedDimension.Add(Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementType"]));
                }
            }

            if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             ))
            {
                _budgetControl = _listBudgetControl.Find(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             );
                _budgetControl.ControlledAmount = Decimal.Subtract(_budgetControl.ControlledAmount, Util.GetValueOfDecimal(drDataRecord["Debit"]));
                if (_budgetControl.ControlledAmount < 0)
                {
                    if (!_budgetMessage.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetMessage += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget Exceed - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                        + Util.GetValueOfString(drBUdgetControl["ControlName"]) + " - (" + _budgetControl.ControlledAmount + ") - Table ID : " +
                                        Util.GetValueOfInt(drDataRecord["LineTable_ID"]) + " - Record ID : " + Util.GetValueOfInt(drDataRecord["Line_ID"]));
                }
            }
            else
            {
                if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                             (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                             (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"]))
                                            ))
                {
                    // If budget not defined then add error message in _budgetNotDefined message variable
                    // Done by rakesh on 29/Apr/2021 Messsage Variable changed from _budgetMessage to _budgetNotDefined
                    if (!_budgetNotDefined.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetNotDefined += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget not defined for - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                        + Util.GetValueOfString(drBUdgetControl["ControlName"]) + " - Table ID : " +
                                        Util.GetValueOfInt(drDataRecord["LineTable_ID"]) + " - Record ID : " + Util.GetValueOfInt(drDataRecord["Line_ID"]) +
                                        " - Account ID : " + Util.GetValueOfInt(drDataRecord["Account_ID"]));
                }
            }

            return _listBudgetControl;
        }

        /*	Set Processed.
        * 	Propergate to Lines/Taxes
        *	@param processed processed
        */
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            //SI_0737 : System is allowing to delete the requisition lines even if document is completed.
            if (Get_ColumnIndex("Processed") >= 0)
            {
                String set = "SET Processed='"
                    + (processed ? "Y" : "N")
                    + "' WHERE M_Requisition_ID=" + GetM_Requisition_ID();
                int noLine = DataBase.DB.ExecuteQuery("UPDATE M_RequisitionLine " + set, null, Get_TrxName());
                _lines = null;
                log.Fine(processed + " - Lines=" + noLine);
            }
        }

        public int GetLocation(int ware_ID)
        {
            //string qry = @"SELECT sg.M_Locator_ID FROM M_Storage sg INNER JOIN M_Locator loc ON sg.M_Locator_ID=loc.M_Locator_ID INNER JOIN M_Warehouse wh ON loc.M_Warehouse_ID=wh.M_Warehouse_ID where wh.M_Warehouse_ID = " + ware_ID + " and sg.M_Product_ID = " + prod_ID + " and loc.IsActive = 'Y'";
            //IDataReader idrloc =DB.ExecuteReader(qry);
            int location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + ware_ID + " and IsActive = 'Y' and IsDefault='Y'"));
            if (location > 0)
            {
                return location;
            }
            else
            {
                //qry = @"SELECT sg.M_Locator_ID FROM M_Storage sg INNER JOIN M_Locator loc ON sg.M_Locator_ID=loc.M_Locator_ID INNER JOIN M_Warehouse wh ON loc.M_Warehouse_ID=wh.M_Warehouse_ID where wh.M_Warehouse_ID = " + ware_ID + " and loc.IsDefault='Y' and loc.IsActive = 'Y'";
                location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + ware_ID + " and IsActive = 'Y'"));
                if (location > 0)
                {
                    return location;
                }
                else
                {
                    return 0;
                }
            }
        }

        // Get Locator_ID for SourceWarehouse  //new 6 jan 3 4
        public int GetSwhLocation(int Swh_ID)
        {
            int Swhlocation = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + Swh_ID + " and IsActive = 'Y' and IsDefault='Y'"));
            if (Swhlocation > 0)
            {
                return Swhlocation;
            }
            else
            {
                Swhlocation = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + Swh_ID + " and IsActive = 'Y'"));
                if (Swhlocation > 0)
                {
                    return Swhlocation;
                }
                else
                {
                    return 0;
                }
            }
        }
        //End


        /**
         * 	Void Document.
         * 	Same as Close.
         * 	@return true if success 
         */
        public bool VoidIt()
        {
            log.Info("VoidIt - " + ToString());
            StringBuilder sql = new StringBuilder();
            //	Not Processed
            if (!(DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus())))
            {
                //VIS0060: To check if associated PO is exist but not reversed
                sql.Append("SELECT COUNT(M_RequisitionLine_ID) FROM M_RequisitionLine WHERE M_Requisition_ID=" + GetM_Requisition_ID() +
                " AND NVL(C_OrderLine_ID, 0) > 0");

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName())) > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_VoidPOFirst");
                    return false;
                }

                //VIS0060: To check if associated Movement is exist but not reversed, If Reserved Qty on Requisition line, system should not allow to Void the record.
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_RequisitionLine_ID) FROM M_RequisitionLine WHERE M_Requisition_ID =" + GetM_Requisition_ID()
                    + " AND (DTD001_ReservedQty > 0 OR DTD001_DeliveredQty > 0)", null, Get_TrxName())) > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_VoidMovementFirst");
                    return false;
                }

                MRequisitionLine[] lines = GetLines();
                Decimal totalLines = Env.ZERO;
                for (int i = 0; i < lines.Length; i++)
                {
                    MRequisitionLine line = lines[i];
                    MProduct product = null;
                    if (line.GetM_Product_ID() > 0)
                        product = MProduct.Get(GetCtx(), line.GetM_Product_ID());

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        //Update storage requisition reserved qty                        
                        if (GetDocStatus() != "DR" && product != null && product.GetProductType() == X_M_Product.PRODUCTTYPE_Item && product.IsStocked())
                        {
                            if (line.Get_ColumnIndex("QtyReserved") >= 0 && line.GetQtyReserved() != 0)
                            {
                                int loc_id = 0;
                                if (line.Get_ColumnIndex("OrderLocator_ID") > 0)
                                {
                                    loc_id = line.GetOrderLocator_ID();
                                }
                                else
                                {
                                    loc_id = GetLocation(GetM_Warehouse_ID());
                                }
                                storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                }
                                storage.SetDTD001_QtyReserved(Decimal.Subtract(storage.GetDTD001_QtyReserved(), line.GetQtyReserved()));
                                storage.Save();

                                int Swhloc_id = 0;
                                if (line.Get_ColumnIndex("ReserveLocator_ID") > 0)
                                {
                                    Swhloc_id = line.GetReserveLocator_ID();
                                }
                                else
                                {
                                    Swhloc_id = GetSwhLocation(GetM_Warehouse_ID());
                                }
                                Swhstorage = MStorage.Get(GetCtx(), Swhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                if (Swhstorage == null)
                                {
                                    Swhstorage = MStorage.GetCreate(GetCtx(), Swhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                }
                                Swhstorage.SetDTD001_SourceReserve(Decimal.Subtract(Swhstorage.GetDTD001_SourceReserve(), line.GetQtyReserved()));
                                Swhstorage.Save();
                            }
                        }
                    }

                    String description = line.GetDescription();
                    if (description == null)
                        description = "";
                    description += Msg.GetMsg(Env.GetContext(), "Voided", true) + " (" + line.GetQty() + ")";
                    line.SetDescription(description);
                    line.SetQty(0);
                    line.SetQtyEntered(0);
                    if (line.Get_ColumnIndex("QtyReserved") > 0)
                    {
                        line.SetQtyReserved(Env.ZERO);
                    }

                    // VIS0060: Remove Sales Order Line reference from Requisition Line in case of Void.
                    line.Set_Value("Ref_OrderLine_ID", null);

                    // VIS0060: Remove Production Order reference from Requisition Line in case of Void.
                    if (Env.IsModuleInstalled("VAMFG_"))
                    {
                        line.Set_Value("VAMFG_M_WorkOrderComponent_ID", null);
                        line.Set_Value("VAMFG_M_WorkOrder_ID", null);
                    }
                    line.SetLineNetAmt(Env.ZERO);
                    if (!line.Save())
                    {
                        _processMsg = Msg.GetMsg(GetCtx(), "ReqLineNotSaved");
                        return false;
                    }
                }

                // VIS0060: Remove Requisition Line reference from Sales Order line in case of void.
                DB.ExecuteQuery(@"UPDATE C_OrderLine SET M_RequisitionLine_ID = NULL WHERE M_RequisitionLine_ID IN (SELECT M_RequisitionLine_ID 
                            FROM M_RequisitionLine WHERE M_Requisition_ID = " + GetM_Requisition_ID() + ")", null, Get_TrxName());

                SetProcessed(true);
                SetDocAction(DOCACTION_None);
                log.Info("voidIt - " + ToString());
                return true;
            }
            if (!CloseIt())
            {
                return false;
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            log.Info("voidIt - " + ToString());
            return true;
        }

        /**
         * 	Close Document.
         * 	Cancel not delivered Qunatities
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            try
            {
                log.Info("closeIt - " + ToString());
                //	Close Not delivered Qty
                MRequisitionLine[] lines = GetLines();

                //If there Reserved Qty on Requisition line, system should not allow to close the record.
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_RequisitionLine_ID) FROM M_RequisitionLine WHERE M_Requisition_ID =" + GetM_Requisition_ID()
                    + " AND DTD001_ReservedQty > 0", null, Get_TrxName())) > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_CannotClose");
                    return false;
                }

                // If there is any PO line reference is available on Req line and that PO is in drafted and in Progress stage 
                // then system will not allow to close the record.
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(rl.M_RequisitionLine_ID) FROM M_RequisitionLine rl INNER JOIN C_OrderLine ol ON  rl.C_OrderLine_ID = ol.C_OrderLine_ID
                    INNER JOIN C_Order o ON o.C_Order_ID=ol.C_Order_ID WHERE rl.M_Requisition_ID = " + GetM_Requisition_ID()
                    + " AND o.DocStatus IN ('DR', 'IP')", null, Get_TrxName())) > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_DraftedOrder");
                    return false;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    MRequisitionLine line = lines[i];
                    MProduct product = null;
                    if (line.GetM_Product_ID() > 0)
                        product = MProduct.Get(GetCtx(), line.GetM_Product_ID());

                    Decimal finalQty = line.GetQty();
                    Decimal unprocessQty = 0;
                    if (line.GetC_OrderLine_ID() == 0)
                    {
                        finalQty = Env.ZERO;
                    }
                    else
                    {
                        MOrderLine ol = new MOrderLine(GetCtx(), line.GetC_OrderLine_ID(), Get_TrxName());
                        finalQty = ol.GetQtyOrdered();
                    }

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        unprocessQty = line.GetQty() - line.GetDTD001_DeliveredQty();

                        //Update storage requisition reserved qty                        
                        if (GetDocAction() != "VO" && GetDocStatus() != "DR" && product != null && product.GetProductType() == X_M_Product.PRODUCTTYPE_Item && product.IsStocked())
                        {
                            if (unprocessQty > 0)
                            {
                                int loc_id = 0;
                                if (line.Get_ColumnIndex("OrderLocator_ID") > 0)
                                {
                                    loc_id = line.GetOrderLocator_ID();
                                }
                                else
                                {
                                    loc_id = GetLocation(GetM_Warehouse_ID());
                                }
                                storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                }
                                storage.SetDTD001_QtyReserved(Decimal.Subtract(storage.GetDTD001_QtyReserved(), unprocessQty));
                                storage.Save();

                                int Swhloc_id = 0;
                                if (line.Get_ColumnIndex("ReserveLocator_ID") > 0)
                                {
                                    Swhloc_id = line.GetReserveLocator_ID();
                                }
                                else
                                {
                                    Swhloc_id = GetSwhLocation(GetM_Warehouse_ID());
                                }
                                Swhstorage = MStorage.Get(GetCtx(), Swhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                if (Swhstorage == null)
                                {
                                    Swhstorage = MStorage.GetCreate(GetCtx(), Swhloc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                                }
                                Swhstorage.SetDTD001_SourceReserve(Decimal.Subtract(Swhstorage.GetDTD001_SourceReserve(), unprocessQty));
                                Swhstorage.Save();
                            }
                        }
                    }

                    // Set UnProcesses Qty on Requisition Line.
                    if (line.Get_ColumnIndex("UnProcessQty") >= 0 && unprocessQty > 0)
                    {
                        String description = line.GetDescription();
                        if (description == null)
                            description = "";
                        description += " [" + line.GetDTD001_DeliveredQty() + "]";
                        line.SetDescription(description);
                        line.SetUnProcessQty(unprocessQty);
                        line.SetLineNetAmt(Decimal.Multiply(line.GetDTD001_DeliveredQty(), line.GetPriceActual()));

                        // VIS0060: Remove Sales Order Line reference from Requisition Line in case of Void.
                        if (GetDocAction().Equals(DOCACTION_Void))
                        {
                            line.Set_Value("Ref_OrderLine_ID", null);

                            // VIS0060: Remove Production Order reference from Requisition Line in case of Void.
                            if (Env.IsModuleInstalled("VAMFG_"))
                            {
                                line.Set_Value("VAMFG_M_WorkOrderComponent_ID", null);
                                line.Set_Value("VAMFG_M_WorkOrder_ID", null);
                            }
                        }
                        if (!line.Save())
                        {
                            _processMsg = Msg.GetMsg(GetCtx(), "ReqLineNotSaved");
                            return false;
                        }
                    }
                }

                // VIS0060: Remove Requisition Line reference from Sales Order line in case of void.
                if (GetDocAction().Equals(DOCACTION_Void))
                {
                    DB.ExecuteQuery(@"UPDATE C_OrderLine SET M_RequisitionLine_ID = NULL WHERE M_RequisitionLine_ID IN (SELECT M_RequisitionLine_ID 
                            FROM M_RequisitionLine WHERE M_Requisition_ID = " + GetM_Requisition_ID() + ")", null, Get_TrxName());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MRequisition--CloseIt");
                log.Severe(ex.ToString());
                _processMsg = ex.Message.ToString();
                return false;
            }
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return true if success 
         */
        public bool ReverseCorrectIt()
        {
            log.Info("reverseCorrectIt - " + ToString());
            return false;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return true if success 
         */
        public bool ReverseAccrualIt()
        {
            log.Info("reverseAccrualIt - " + ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return true if success 
         */
        public bool ReActivateIt()
        {
            log.Info("ReActivateIT - " + ToString());
            StringBuilder sql = new StringBuilder();
            // In case of purchase order reverse budget breach
            // Done by Rakesh Kumar 19/Feb/2020
            if (Env.IsModuleInstalled("FRPT_"))
            {
                SetIsBudgetBreach(false);
                SetIsBudgetBreachApproved(false);
            }

            //VIS0060: To check if associated PO is exist but not reversed
            sql.Append("SELECT COUNT(M_RequisitionLine_ID) FROM M_RequisitionLine WHERE M_Requisition_ID=" + GetM_Requisition_ID() +
                " AND (NVL(C_OrderLine_ID, 0) > 0");                // OR NVL(Ref_OrderLine_ID, 0) > 0");

            //if (Env.IsModuleInstalled("VAMFG_"))
            //{
            //    sql.Append("OR NVL(VAMFG_M_WorkOrder_ID, 0) > 0");
            //}

            sql.Append(")");

            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName())) > 0)
            {
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_VoidPOFirst");
                return false;
            }

            SetDocAction(DOCACTION_Complete);
            SetProcessed(false);
            return true;
        }

        /*
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	 - User
            sb.Append(" - ").Append(GetUserName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ").
                Append(Msg.Translate(GetCtx(), "TotalLines")).Append("=").Append(GetTotalLines())
                .Append(" (#").Append(GetLines().Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /**
         * 	Get Document Owner
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetAD_User_ID();
        }

        /**
         * 	Get Document Currency
         *	@return C_Currency_ID
         */
        public int GetC_Currency_ID()
        {
            MPriceList pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), Get_TrxName());
            return pl.GetC_Currency_ID();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount
         */
        public Decimal GetApprovalAmt()
        {
            return GetTotalLines();
        }

        /**
         * 	Get User Name
         *	@return user name
         */
        public String GetUserName()
        {
            return MUser.Get(GetCtx(), GetAD_User_ID()).GetName();
        }
        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }
        public string GetDocBaseType()
        {
            return null;
        }


        public void SetProcessMsg(string processMsg)
        {

        }



        #endregion
    }
}
