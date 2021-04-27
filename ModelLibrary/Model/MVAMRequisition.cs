﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMRequisition
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
//using System.Windows.Forms;
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
    public class MVAMRequisition : X_VAM_Requisition, DocAction
    {
        //	Process Message 		
        private String _processMsg = null;
        //	Just Prepared Flag		
        private bool _justPrepared = false;
        //Lines						
        private MVAMRequisitionLine[] _lines = null;
        MVAMStorage storage = null;
        MVAMStorage Swhstorage = null;

        String _budgetMessage = String.Empty;

        /**
        * 	Standard Constructor
        *	@param Ctx context
        *	@param VAM_Requisition_ID id
        */
        public MVAMRequisition(Ctx ctx, int VAM_Requisition_ID, Trx trxName)
            : base(ctx, VAM_Requisition_ID, trxName)
        {
            try
            {

                if (VAM_Requisition_ID == 0)
                {
                    //	setDocumentNo (null);
                    //	setVAF_UserContact_ID (0);
                    //	setVAM_PriceList_ID (0);
                    //	setVAM_Warehouse_ID(0);
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
                // MessageBox.Show("MVAMRequisition--Standard Constructor");
                log.Severe(ex.ToString());
            }
        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         */
        public MVAMRequisition(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /**
         * 	Get Lines
         *	@return array of lines
         */
        public MVAMRequisitionLine[] GetLines()
        {
            try
            {
                if (_lines != null)
                {
                    return _lines;
                }

                List<MVAMRequisitionLine> list = new List<MVAMRequisitionLine>();
                String sql = "SELECT * FROM VAM_RequisitionLine WHERE VAM_Requisition_ID=" + GetVAM_Requisition_ID() + " ORDER BY Line";
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
                        list.Add(new MVAMRequisitionLine(GetCtx(), dr, Get_TrxName()));
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
                _lines = new MVAMRequisitionLine[list.Count];
                _lines = list.ToArray();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MVAMRequisition--GetLines");
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
            StringBuilder sb = new StringBuilder("MVAMRequisition[");
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
            //return Msg.getElement(getContext(), "VAM_Requisition_ID") + " " + getDocumentNo();
            return Msg.GetElement(GetCtx(), "VAM_Requisition_ID") + " " + GetDocumentNo();
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
            /*	ReportEngine re = ReportEngine.get (getContext(), ReportEngine.INVOICE, getVAB_Invoice_ID());
                if (re == null)*/
            return null;
            //return file;
            /*	return re.getPDF(file);*/
        }

        /**
         * 	Set default PriceList
         */
        public void SetVAM_PriceList_ID()
        {
            try
            {
                MVAMPriceList defaultPL = MVAMPriceList.GetDefault(GetCtx(), false);
                if (defaultPL == null)
                {
                    defaultPL = MVAMPriceList.GetDefault(GetCtx(), true);
                }
                if (defaultPL != null)
                {
                    SetVAM_PriceList_ID(defaultPL.GetVAM_PriceList_ID());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MVAMRequisition--SetVAM_PriceList_ID");
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
            if (GetVAM_PriceList_ID() == 0)
            {
                SetVAM_PriceList_ID();
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

                MVAMRequisitionLine[] lines = GetLines();

                // JID_0901: Added by Bharat on 30 Jan 2019 gives message when there are no line on requisition
                if (lines.Length == 0)
                {
                    _processMsg = "@NoLines@";
                    return DocActionVariables.STATUS_INVALID;
                }

                //	Invalid
                if (GetVAF_UserContact_ID() == 0
                    || GetVAM_PriceList_ID() == 0
                    || GetVAM_Warehouse_ID() == 0)
                    return DocActionVariables.STATUS_INVALID;



                //	Std Period open?
                if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateDoc(), MVABMasterDocType.DOCBASETYPE_PURCHASEREQUISITION, GetVAF_Org_ID()))
                {
                    _processMsg = "@PeriodClosed@";
                    return DocActionVariables.STATUS_INVALID;
                }

                // is Non Business Day?
                // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
                if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateDoc(), GetVAF_Org_ID()))
                {
                    _processMsg = Common.Common.NONBUSINESSDAY;
                    return DocActionVariables.STATUS_INVALID;
                }



                //	Add up Amounts
                int precision = MVAMPriceList.GetStandardPrecision(GetCtx(), GetVAM_PriceList_ID());
                Decimal totalLines = Env.ZERO;
                for (int i = 0; i < lines.Length; i++)
                {
                    MVAMRequisitionLine line = lines[i];
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
                // MessageBox.Show("MVAMRequisition--PrepareIt");
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

                if (Env.IsModuleInstalled("FRPT_"))
                {
                    // budget control functionality work when Financial Managemt Module Available
                    try
                    {
                        log.Info("Budget Control Start for Rquisition Document No  " + GetDocumentNo());
                        EvaluateBudgetControlData();
                        if (_budgetMessage.Length > 0)
                        {
                            _processMsg = Msg.GetMsg(GetCtx(), "BudgetExceedFor") + _budgetMessage;
                            SetProcessed(false);
                            return DocActionVariables.STATUS_INPROGRESS;
                        }
                        log.Info("Budget Control Completed for Rquisition Document No  " + GetDocumentNo());
                    }
                    catch (Exception ex)
                    {
                        log.Severe("Budget Control Issue " + ex.Message);
                        SetProcessed(false);
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }

                // JID_1290: Set the document number from completed document sequence after completed (if needed)
                SetCompletedDocumentNo();

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
                //
                SetProcessed(true);
                SetDocAction(DocActionVariables.ACTION_CLOSE);
                /**************************************************************************************************************/
                // Check Column Name  new 6jan 0 vikas
                int _count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT Count(*) FROM VAF_Column WHERE columnname = 'DTD001_SourceReserve' "));

                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("DTD001_", out mInfo))
                {
                    MVAMRequisitionLine[] lines = GetLines();
                    MVAMProduct product = null;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        MVAMRequisitionLine line = lines[i];
                        if (line.GetVAM_Product_ID() > 0)
                            product = MVAMProduct.Get(GetCtx(), line.GetVAM_Product_ID());

                        int loc_id = GetLocation(GetVAM_Warehouse_ID());
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
                        if (line.Get_ColumnIndex("OrderLocator_ID") > 0)
                        {
                            line.SetOrderLocator_ID(loc_id);
                            line.SetReserveLocator_ID(Sourcewhloc_id);
                            if (!line.Save())
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "ReqLineNotSaved");
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
                        // SI_0686_2 :  storage should not update in case of product is other than item type.
                        if (Env.IsModuleInstalled("VA203_") && product != null && product.GetProductType() == X_VAM_Product.PRODUCTTYPE_Item)
                        {
                            storage = MVAMStorage.Get(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            if (storage == null)
                            {
                                storage = MVAMStorage.GetCreate(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            }
                            storage.SetDTD001_QtyReserved((Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)line.GetQty())));
                            if (!storage.Save())
                            {
                                log.Info("Requisition Reserverd Quantity not saved at storage at locator " + loc_id + " and product is " + line.GetVAM_Product_ID());
                            }
                            ///new 6jan 2
                            if (_count > 0)
                            {
                                Swhstorage = MVAMStorage.Get(GetCtx(), Sourcewhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                if (Swhstorage == null)
                                {
                                    Swhstorage = MVAMStorage.GetCreate(GetCtx(), Sourcewhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                }
                                Swhstorage.SetDTD001_SourceReserve((Decimal.Add(Swhstorage.GetDTD001_SourceReserve(), (Decimal)line.GetQty())));
                                if (!Swhstorage.Save())
                                {
                                    log.Info("Requisition Reserverd Quantity not saved at storage at locator " + Sourcewhloc_id + " and product is " + line.GetVAM_Product_ID());
                                }
                            }
                            //End
                        }
                        //(JID_1365)shubham add code below line after && 
                        else if (product != null && product.GetProductType() == X_VAM_Product.PRODUCTTYPE_Item && product.IsStocked())
                        {
                            // SI_0657: consider Attribute also
                            storage = MVAMStorage.Get(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            if (storage == null)
                            {
                                //MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(), loc_id, line.GetVAM_Product_ID(), 0, 0, 0, 0, line.GetQty(), null);
                                MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), line.GetVAM_PFeature_SetInstance_ID(), (Decimal)0, (Decimal)0, (Decimal)0, line.GetQty(), Get_Trx());
                            }
                            else
                            {
                                storage.SetDTD001_QtyReserved((Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)line.GetQty())));
                                storage.Save();
                            }
                            //new 6jan 3
                            if (_count > 0)
                            {
                                Swhstorage = MVAMStorage.Get(GetCtx(), Sourcewhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                if (Swhstorage == null)
                                {

                                    MVAMStorage.Add(GetCtx(), GetDTD001_MWarehouseSource_ID(), Sourcewhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), line.GetVAM_PFeature_SetInstance_ID(), (Decimal)0, (Decimal)0, (Decimal)0, 0, Get_Trx());
                                    MVAMStorage StrgResrvQty = null;
                                    StrgResrvQty = MVAMStorage.GetCreate(GetCtx(), Sourcewhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                    StrgResrvQty.SetDTD001_SourceReserve(Decimal.Add(StrgResrvQty.GetDTD001_SourceReserve(), line.GetQty()));
                                    StrgResrvQty.Save();
                                }
                                else
                                {
                                    Swhstorage.SetDTD001_SourceReserve((Decimal.Add(Swhstorage.GetDTD001_SourceReserve(), (Decimal)line.GetQty())));
                                    Swhstorage.Save();
                                }
                            }
                            //End
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MVAMRequisition--CompleteIt");
                log.Severe(ex.ToString());
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateDoc(DateTime.Now.Date);

                //	Std Period open?
                if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateDoc(), dt.GetDocBaseType(), GetVAF_Org_ID()))
                {
                    throw new Exception("@PeriodClosed@");
                }
            }

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MVAFRecordSeq.GetDocumentNo(GetVAB_DocTypes_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
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
            sql.Append(@"SELECT VAGL_Budget.VAGL_Budget_ID , VAGL_Budget.BudgetControlBasis, VAGL_Budget.VAB_Year_ID , VAGL_Budget.VAB_YearPeriod_ID,VAGL_Budget.Name As BudgetName, 
                  VAGL_BudgetActivation.VAB_AccountBook_ID, VAGL_BudgetActivation.CommitmentType, VAGL_BudgetActivation.BudgetControlScope,  VAGL_BudgetActivation.VAGL_BudgetActivation_ID, VAGL_BudgetActivation.Name AS ControlName 
                FROM VAGL_Budget INNER JOIN VAGL_BudgetActivation ON VAGL_Budget.VAGL_Budget_ID = VAGL_BudgetActivation.VAGL_Budget_ID
                INNER JOIN VAF_ClientDetail ON VAF_ClientDetail.VAF_Client_ID = VAGL_Budget.VAF_Client_ID
                WHERE VAGL_BudgetActivation.IsActive = 'Y' AND VAGL_Budget.IsActive = 'Y' AND VAGL_BudgetActivation.VAF_Org_ID IN (0 , " + GetVAF_Org_ID() + @")  
                   AND VAGL_BudgetActivation.CommitmentType IN ('B' ) AND 
                  (( VAGL_Budget.BudgetControlBasis = 'P' AND VAGL_Budget.VAB_YearPeriod_ID =
                  (SELECT VAB_YearPeriod.VAB_YearPeriod_ID FROM VAB_YearPeriod INNER JOIN VAB_Year ON VAB_Year.VAB_Year_ID      = VAB_YearPeriod.VAB_Year_ID
                  WHERE VAB_YearPeriod.IsActive  = 'Y'  AND VAB_Year.VAB_Calender_ID = VAF_ClientDetail.VAB_Calender_ID
                  AND " + GlobalVariable.TO_DATE(GetDateDoc(), true) + @" BETWEEN VAB_YearPeriod.startdate AND VAB_YearPeriod.enddate )) 
                OR ( VAGL_Budget.BudgetControlBasis = 'A' AND VAGL_Budget.VAB_Year_ID =
                  (SELECT VAB_YearPeriod.VAB_Year_ID FROM VAB_YearPeriod INNER JOIN VAB_Year ON VAB_Year.VAB_Year_ID = VAB_YearPeriod.VAB_Year_ID
                  WHERE VAB_YearPeriod.IsActive  = 'Y'   AND VAB_Year.VAB_Calender_ID = VAF_ClientDetail.VAB_Calender_ID  
                AND " + GlobalVariable.TO_DATE(GetDateDoc(), true) + @" BETWEEN VAB_YearPeriod.startdate AND VAB_YearPeriod.enddate   ) ) ) 
                AND (SELECT COUNT(Actual_Acct_Detail_id) FROM Actual_Acct_Detail
                WHERE VAGL_Budget_ID = VAGL_Budget.VAGL_Budget_ID
                AND (VAB_YearPeriod_id  IN ( NVL(VAGL_Budget.VAB_YearPeriod_ID ,0 ))
                OR VAB_YearPeriod_id    IN (SELECT VAB_YearPeriod_ID FROM VAB_YearPeriod   WHERE VAB_Year_ID = NVL(VAGL_Budget.VAB_Year_ID , 0) ) ) ) > 0");
            DataSet dsBudgetControl = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsBudgetControl != null && dsBudgetControl.Tables.Count > 0 && dsBudgetControl.Tables[0].Rows.Count > 0)
            {
                // get budget control ids
                object[] budgetControlIds = dsBudgetControl.Tables[0].AsEnumerable().Select(r => r.Field<object>("VAGL_BUDGETACTIVATION_ID")).ToArray();
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
                            drBudgetControl = dsBudgetControl.Tables[0].Select("VAB_AccountBook_ID  = " + Util.GetValueOfInt(drRecordData[i]["VAB_AccountBook_ID"]));

                            // loop on Budget which to be controlled 
                            if (drBudgetControl != null)
                            {
                                for (int j = 0; j < drBudgetControl.Length; j++)
                                {
                                    // get budget Dimension datarow 
                                    drBudgetControlDimension = dsBudgetControlDimension.Tables[0].Select("VAGL_BudgetActivation_ID  = "
                                                                + Util.GetValueOfInt(drBudgetControl[j]["VAGL_BudgetActivation_ID"]));

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
            int VAF_Screen_id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_Screen_ID FROM VAF_Screen WHERE  Export_ID = 'VIS_322'"));
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
                                                                Util.GetValueOfInt(GetVAF_Client_ID()),
                                                                Util.GetValueOfInt(X_VAM_Requisition.Table_ID),//MVAFTableView.Get(GetCtx() , "VAM_Requisition").GetVAF_TableView_ID()
                                                                Util.GetValueOfInt(GetVAM_Requisition_ID()),
                                                                true,
                                                                Util.GetValueOfInt(GetVAF_Org_ID()),
                                                                VAF_Screen_id,
                                                                Util.GetValueOfInt(GetVAB_DocTypes_ID()) };
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

            if (_listBudgetControl.Exists(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                              (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.VAF_Org_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["VAF_Org_ID"]) : 0)) &&
                                              (x.VAB_BusinessPartner_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["VAB_BusinessPartner_ID"]) : 0)) &&
                                              (x.VAM_Product_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["VAM_Product_ID"]) : 0)) &&
                                              (x.VAB_BillingCode_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["VAB_BillingCode_ID"]) : 0)) &&
                                              (x.VAB_AddressFrom_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["VAB_AddressFrom_ID"]) : 0)) &&
                                              (x.VAB_AddressTo_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["VAB_AddressTo_ID"]) : 0)) &&
                                              (x.VAB_Promotion_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["VAB_Promotion_ID"]) : 0)) &&
                                              (x.VAF_OrgTrx_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["VAF_OrgTrx_ID"]) : 0)) &&
                                              (x.VAB_Project_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["VAB_Project_ID"]) : 0)) &&
                                              (x.VAB_SalesRegionState_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["VAB_SalesRegionState_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             ))
            {
                _budgetControl = _listBudgetControl.Find(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                              (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.VAF_Org_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["VAF_Org_ID"]) : 0)) &&
                                              (x.VAB_BusinessPartner_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["VAB_BusinessPartner_ID"]) : 0)) &&
                                              (x.VAM_Product_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["VAM_Product_ID"]) : 0)) &&
                                              (x.VAB_BillingCode_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["VAB_BillingCode_ID"]) : 0)) &&
                                              (x.VAB_AddressFrom_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["VAB_AddressFrom_ID"]) : 0)) &&
                                              (x.VAB_AddressTo_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["VAB_AddressTo_ID"]) : 0)) &&
                                              (x.VAB_Promotion_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["VAB_Promotion_ID"]) : 0)) &&
                                              (x.VAF_OrgTrx_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["VAF_OrgTrx_ID"]) : 0)) &&
                                              (x.VAB_Project_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["VAB_Project_ID"]) : 0)) &&
                                              (x.VAB_SalesRegionState_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["VAB_SalesRegionState_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             );
                _budgetControl.ControlledAmount = Decimal.Subtract(_budgetControl.ControlledAmount, Util.GetValueOfDecimal(drDataRecord["Debit"]));
                if (_budgetControl.ControlledAmount < 0)
                {
                    if (!_budgetMessage.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetMessage += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget control Exceed - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                        + Util.GetValueOfString(drBUdgetControl["ControlName"]) + " - (" + _budgetControl.ControlledAmount + ") - Table ID : " +
                                        Util.GetValueOfInt(drDataRecord["LineTable_ID"]) + " - Record ID : " + Util.GetValueOfInt(drDataRecord["Line_ID"]));
                }
            }
            else
            {
                if (_listBudgetControl.Exists(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                             (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                             (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"]))
                                            ))
                {
                    if (!_budgetMessage.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetMessage += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget control not defined for - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
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
                    + "' WHERE VAM_Requisition_ID=" + GetVAM_Requisition_ID();
                int noLine = DataBase.DB.ExecuteQuery("UPDATE VAM_RequisitionLine " + set, null, Get_TrxName());
                _lines = null;
                log.Fine(processed + " - Lines=" + noLine);
            }
        }

        public int GetLocation(int ware_ID)
        {
            //string qry = @"SELECT sg.VAM_Locator_ID FROM VAM_Storage sg INNER JOIN VAM_Locator loc ON sg.VAM_Locator_ID=loc.VAM_Locator_ID INNER JOIN VAM_Warehouse wh ON loc.VAM_Warehouse_ID=wh.VAM_Warehouse_ID where wh.VAM_Warehouse_ID = " + ware_ID + " and sg.VAM_Product_ID = " + prod_ID + " and loc.IsActive = 'Y'";
            //IDataReader idrloc =DB.ExecuteReader(qry);
            int location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Locator_ID FROM VAM_Locator where VAM_Warehouse_ID = " + ware_ID + " and IsActive = 'Y' and IsDefault='Y'"));
            if (location > 0)
            {
                return location;
            }
            else
            {
                //qry = @"SELECT sg.VAM_Locator_ID FROM VAM_Storage sg INNER JOIN VAM_Locator loc ON sg.VAM_Locator_ID=loc.VAM_Locator_ID INNER JOIN VAM_Warehouse wh ON loc.VAM_Warehouse_ID=wh.VAM_Warehouse_ID where wh.VAM_Warehouse_ID = " + ware_ID + " and loc.IsDefault='Y' and loc.IsActive = 'Y'";
                location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Locator_ID FROM VAM_Locator where VAM_Warehouse_ID = " + ware_ID + " and IsActive = 'Y'"));
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
            int Swhlocation = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Locator_ID FROM VAM_Locator where VAM_Warehouse_ID = " + Swh_ID + " and IsActive = 'Y' and IsDefault='Y'"));
            if (Swhlocation > 0)
            {
                return Swhlocation;
            }
            else
            {
                Swhlocation = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Locator_ID FROM VAM_Locator where VAM_Warehouse_ID = " + Swh_ID + " and IsActive = 'Y'"));
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
            // Added by Bharat for JID_1007: void Requisition is not marking fields readonly.
            SetProcessed(true);
            log.Info("voidIt - " + ToString());
            return CloseIt();
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
                MVAMRequisitionLine[] lines = GetLines();
                Decimal totalLines = Env.ZERO;
                MVAMProduct product = null;
                for (int i = 0; i < lines.Length; i++)
                {
                    MVAMRequisitionLine line = lines[i];

                    if (line.GetVAM_Product_ID() > 0)
                        product = MVAMProduct.Get(GetCtx(), line.GetVAM_Product_ID());

                    Decimal finalQty = line.GetQty();
                    if (line.GetVAB_OrderLine_ID() == 0)
                    {
                        finalQty = Env.ZERO;
                    }
                    else
                    {
                        MVABOrderLine ol = new MVABOrderLine(GetCtx(), line.GetVAB_OrderLine_ID(), Get_TrxName());
                        finalQty = ol.GetQtyOrdered();
                    }
                    Tuple<String, String, String> mInfo = null;
                    if (Env.HasModulePrefix("DTD001_", out mInfo))
                    {
                        int quant = Util.GetValueOfInt(line.GetQty() - line.GetDTD001_DeliveredQty());
                        // new 6jan  0
                        int _count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT Count(*) FROM VAF_Column WHERE columnname = 'DTD001_SourceReserve' "));

                        //Update storage requisition reserved qty
                        //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
                        if (Env.IsModuleInstalled("VA203_") && product != null && product.GetProductType() == X_VAM_Product.PRODUCTTYPE_Item)
                        {
                            if (GetDocAction() != "VO" && GetDocStatus() != "DR")
                            {
                                if (quant > 0)
                                {
                                    //int loc_id = GetLocation(GetVAM_Warehouse_ID());
                                    int loc_id = line.GetOrderLocator_ID();
                                    storage = MVAMStorage.Get(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                    if (storage == null)
                                    {
                                        storage = MVAMStorage.GetCreate(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                    }
                                    storage.SetDTD001_QtyReserved((Decimal.Subtract(storage.GetDTD001_QtyReserved(), (Decimal)quant)));
                                    storage.Save();
                                    //new 6jan 5
                                    if (_count > 0)
                                    {
                                        //int Swhloc_id = GetSwhLocation(GetDTD001_MWarehouseSource_ID());
                                        int Swhloc_id = line.GetReserveLocator_ID();
                                        Swhstorage = MVAMStorage.Get(GetCtx(), Swhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                        if (Swhstorage == null)
                                        {
                                            Swhstorage = MVAMStorage.GetCreate(GetCtx(), Swhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                        }
                                        Swhstorage.SetDTD001_SourceReserve((Decimal.Subtract(Swhstorage.GetDTD001_SourceReserve(), (Decimal)quant)));
                                        Swhstorage.Save();
                                    }
                                    //end
                                }
                            }
                        }
                        else if (GetDocAction() != "VO" && GetDocStatus() != "DR" && product != null && product.GetProductType() == X_VAM_Product.PRODUCTTYPE_Item)
                        {
                            if (quant > 0)
                            {
                                int loc_id = 0;
                                if (line.Get_ColumnIndex("OrderLocator_ID") > 0)
                                {
                                    loc_id = line.GetOrderLocator_ID();
                                }
                                else
                                {
                                    loc_id = GetLocation(GetVAM_Warehouse_ID());
                                }
                                storage = MVAMStorage.Get(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                if (storage == null)
                                {
                                    storage = MVAMStorage.GetCreate(GetCtx(), loc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                }
                                storage.SetDTD001_QtyReserved((Decimal.Subtract(storage.GetDTD001_QtyReserved(), (Decimal)quant)));
                                storage.Save();

                                //new 6jan 6
                                if (_count > 0)
                                {
                                    int Swhloc_id = 0;
                                    if (line.Get_ColumnIndex("ReserveLocator_ID") > 0)
                                    {
                                        Swhloc_id = line.GetReserveLocator_ID();
                                    }
                                    else
                                    {
                                        Swhloc_id = GetSwhLocation(GetVAM_Warehouse_ID());
                                    }
                                    Swhstorage = MVAMStorage.Get(GetCtx(), Swhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                    if (Swhstorage == null)
                                    {
                                        Swhstorage = MVAMStorage.GetCreate(GetCtx(), Swhloc_id, line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                    }
                                    Swhstorage.SetDTD001_SourceReserve((Decimal.Subtract(Swhstorage.GetDTD001_SourceReserve(), (Decimal)quant)));
                                    Swhstorage.Save();
                                }
                                //end
                            }
                        }
                    }
                    //	final qty is not line qty
                    if (finalQty.CompareTo(line.GetQty()) != 0)
                    {
                        String description = line.GetDescription();
                        if (description == null)
                            description = "";
                        description += " [" + line.GetQty() + "]";
                        line.SetDescription(description);
                        // Amit 9-feb-2015 
                        // line.SetQty(finalQty);
                        //Amit
                        line.SetLineNetAmt();
                        line.Save();
                    }
                    //get Grand Total or SubTotal
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }
                if (totalLines.CompareTo(GetTotalLines()) != 0)
                {
                    SetTotalLines(totalLines);
                    Save();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MVAMRequisition--CloseIt");
                log.Severe(ex.ToString());
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
            log.Info("reActivateIt - " + ToString());
            //	setProcessed(false);
            if (ReverseCorrectIt())
            {
                return true;
            }
            return false;
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
         *	@return VAF_UserContact_ID
         */
        public int GetDoc_User_ID()
        {
            return GetVAF_UserContact_ID();
        }

        /**
         * 	Get Document Currency
         *	@return VAB_Currency_ID
         */
        public int GetVAB_Currency_ID()
        {
            MVAMPriceList pl = MVAMPriceList.Get(GetCtx(), GetVAM_PriceList_ID(), Get_TrxName());
            return pl.GetVAB_Currency_ID();
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
            return MVAFUserContact.Get(GetCtx(), GetVAF_UserContact_ID()).GetName();
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