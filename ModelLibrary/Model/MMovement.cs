/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Movement Model
 * Class Used     : X_M_Movement, DocAction(Interface)
 * Chronological Development
 * Veena         26-Oct-2009
 ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using System.Reflection;//Arpit

namespace VAdvantage.Model
{
    /// <summary>
    /// Inventory Movement Model
    /// </summary>
    public class MMovement : X_M_Movement, DocAction
    {
        /**	Lines						*/
        private MMovementLine[] _lines = null;
        /** Confirmations				*/
        private MMovementConfirm[] _confirms = null;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;
        //	Cache						
        private static CCache<int, MMovement> _cache = new CCache<int, MMovement>("M_Movement", 5, 5);
        private string query = "";
        private Decimal? trxQty = 0;
        private bool isGetFromStorage = false;
        MAsset ast = null;
        private bool isAsset = false;

        MAcctSchema acctSchema = null;
        MProduct product1 = null;
        decimal currentCostPrice = 0;
        string conversionNotFoundInOut = "";
        string conversionNotFoundMovement = "";
        string conversionNotFoundMovement1 = "";
        MCostElement costElement = null;
        ValueNamePair pp = null;
        /**is container applicable */
        private bool isContainerApplicable = false;

        /** Reversal Indicator			*/
        public const String REVERSE_INDICATOR = "^";

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Movement_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMovement(Ctx ctx, int M_Movement_ID, Trx trxName)
            : base(ctx, M_Movement_ID, trxName)
        {
            if (M_Movement_ID == 0)
            {
                //	SetC_DocType_ID (0);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetIsInTransit(false);
                SetMovementDate(new DateTime(CommonFunctions.CurrentTimeMillis()));	// @#Date@
                SetPosted(false);
                base.SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MMovement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of lines</returns>
        public MMovementLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MMovementLine> list = new List<MMovementLine>();
            String sql = "SELECT * FROM M_MovementLine WHERE M_Movement_ID=@moveid ORDER BY Line";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@moveid", GetM_Movement_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MMovementLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetLines", e);
            }

            _lines = new MMovementLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Confirmations
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of confirmations</returns>
        public MMovementConfirm[] GetConfirmations(Boolean requery)
        {
            if (_confirms != null && !requery)
                return _confirms;

            List<MMovementConfirm> list = new List<MMovementConfirm>();
            String sql = "SELECT * FROM M_MovementConfirm WHERE M_Movement_ID=@moveid";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@moveid", GetM_Movement_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MMovementConfirm(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetConfirmations", e);
            }

            _confirms = new MMovementConfirm[list.Count];
            _confirms = list.ToArray();
            return _confirms;
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
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>file if success</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, GetC_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetC_DocType_ID() == 0)
            {
                MDocType[] types = MDocType.GetOfDocBaseType(GetCtx(), MDocBaseType.DOCBASETYPE_MATERIALMOVEMENT);
                if (types.Length > 0)	//	Get first
                    SetC_DocType_ID(types[0].GetC_DocType_ID());
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @C_DocType_ID@"));
                    return false;
                }
            }

            // when we have record on movement line - then we can't change warehouse
            if (!newRecord && Is_ValueChanged("M_Warehouse_ID"))
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_MovementLine_ID) FROM M_MovementLine WHERE M_Movement_ID = " + GetM_Movement_ID(), null, Get_Trx())) > 0)
                {
                    log.SaveError("VIS_ToWarehouseCantChange", "");
                    return false;
                }
            }

            //Lakhwinder 1Feb2021
            //Shipment and Inventory Move Module Changes
            //Solution Proposed Puneet 
            //STandard changes Analysed in Gull Implementation
            if (MDocType.Get(GetCtx(), GetC_DocType_ID()).IsInTransit() && GetM_Warehouse_ID() == 0)
            {
                log.SaveError("VIS_ToWarehouseCantNull", "");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set Processed.
        ///	Propergate to Lines/Taxes
        /// </summary>
        /// <param name="processed">processed</param>
        public void SetProcessed(Boolean processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE M_MovementLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE M_Movement_ID=" + GetM_Movement_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine("Processed=" + processed + " - Lines=" + noLine);
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public Boolean ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MMovementLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts

            /* nnayak - Bug 1750251 : check material policy and update storage
               at the line level in completeIt()*/
            //checkMaterialPolicy();

            if (isContainerApplicable && IsReversal())
            {
                // when we reverse record, and movement line having MoveFullContainer = true
                // system will check qty on transaction must be same as qty on movement line
                // if not matched, then we can not allow to user for its reverse, he need to make a new Movement for move container
                string sql = DBFunctionCollection.CheckContainerQty(GetM_Movement_ID());
                string productName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (!string.IsNullOrEmpty(productName))
                {
                    // Qty Alraedy consumed from Container for Product are : 
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_QtyConsumed") + productName;
                    SetProcessMsg(_processMsg);
                    return DocActionVariables.STATUS_INVALID;
                }
            }


            //Lakhwinder 10 Feb 2021
            //Show Error If Confirmation Doc Type not found.
            if (dt.IsInTransit())
            {
                string s = CheckConfimationDocType(dt);
                if (!String.IsNullOrEmpty(s))
                {
                    _processMsg = s;
                    SetProcessMsg(_processMsg);
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //	Confirmation
            if (GetDescription() != null)
            {
                if (GetDescription().Substring(0, 3) != "{->")
                {
                    if (dt.IsInTransit())
                        CreateConfirmation();
                }
            }
            else
            {
                if (dt.IsInTransit())
                    CreateConfirmation();
            }



            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Create Movement Confirmation
        /// </summary>
        private void CreateConfirmation()
        {
            MMovementConfirm[] confirmations = GetConfirmations(false);
            if (confirmations.Length > 0)
                return;

            //	Create Confirmation
            MMovementConfirm.Create(this, false);
        }
        /// <summary>
        /// Check DocTypeConfimation
        /// </summary>
        /// <param name="dt">DocumentType</param>
        /// <returns>error Message if Confiramtion Doct Tpye not Selected</returns>
        private string CheckConfimationDocType(MDocType dt)
        {
            if (dt.Get_ColumnIndex("C_DocTypeConfrimation_ID") > -1)
            {
                int conDocType = Util.GetValueOfInt(dt.Get_Value("C_DocTypeConfrimation_ID"));
                if (conDocType == 0)
                { return Msg.GetMsg(GetCtx(), "VIS_ConfirmationDocNotFound"); }
            }
            return null;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());
            MMovementLine[] lines = GetLines(false);

            #region[Prevent from completing, If on hand quantity of Product not available as per qty entered at line and Disallow negative is true at Warehouse. By Sukhwinder on 22 Dec, 2017. Only if DTD001 Module Installed.]
            if (Env.IsModuleInstalled("DTD001_"))
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT ISDISALLOWNEGATIVEINV FROM M_Warehouse WHERE M_Warehouse_ID = " + Util.GetValueOfInt(GetDTD001_MWarehouseSource_ID()));
                string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                //int[] movementLine = MInOutLine.GetAllIDs("M_MovementLine", "M_Movement_ID = " + GetM_Movement_ID(), Get_TrxName());
                if (disallow.ToUpper().Equals("Y"))
                {
                    int m_locator_id = 0;
                    int m_product_id = 0;
                    StringBuilder products = new StringBuilder();
                    StringBuilder locators = new StringBuilder();
                    bool check = false;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        //MMovementLine mmLine = new MMovementLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        //MInOutLine iol = new MInOutLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        MMovementLine mmLine = lines[i];
                        m_locator_id = Util.GetValueOfInt(mmLine.GetM_Locator_ID());
                        m_product_id = Util.GetValueOfInt(mmLine.GetM_Product_ID());

                        sql.Clear();
                        sql.Append("SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + m_product_id);
                        int m_attribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                        if (m_attribute_ID == 0)
                        {
                            sql.Clear();
                            if (!isContainerApplicable)
                            {
                                sql.Append("SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id);
                            }
                            else
                            {
                                sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, 
                                   t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                   INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                   " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + mmLine.GetM_Locator_ID() +
                                   " AND t.M_Product_ID = " + mmLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + mmLine.GetM_AttributeSetInstance_ID() +
                                   " AND NVL(t.M_ProductContainer_ID, 0) = " + mmLine.GetM_ProductContainer_ID());
                            }
                            decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            decimal qtyToMove = mmLine.GetMovementQty();
                            if (qty < qtyToMove)
                            {
                                check = true;
                                products.Append(m_product_id + ", ");
                                locators.Append(m_locator_id + ", ");
                                continue;
                            }
                        }
                        else
                        {
                            sql.Clear();
                            if (!isContainerApplicable)
                            {
                                sql.Append("SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id + " AND NVL(M_AttributeSetInstance_ID , 0) = " + mmLine.GetM_AttributeSetInstance_ID());
                            }
                            else
                            {
                                sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, 
                                 t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                 INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                 " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + mmLine.GetM_Locator_ID() +
                                 " AND t.M_Product_ID = " + mmLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + mmLine.GetM_AttributeSetInstance_ID() +
                                 " AND NVL(t.M_ProductContainer_ID, 0) = " + mmLine.GetM_ProductContainer_ID());
                            }
                            decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            decimal qtyToMove = mmLine.GetMovementQty();
                            if (qty < qtyToMove)
                            {
                                check = true;
                                products.Append(m_product_id + ",");
                                locators.Append(m_locator_id + ",");
                                continue;
                            }
                        }
                    }

                    if (check)
                    {
                        sql.Clear();
                        sql.Append(DBFunctionCollection.ConcatinateListOfLocators(locators.ToString()));
                        string loc = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                        sql.Clear();
                        sql.Append(DBFunctionCollection.ConcatinateListOfProducts(products.ToString()));
                        string prod = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                        _processMsg = Msg.GetMsg(Env.GetCtx(), "VIS_InsufficientQuantityFor") + prod + Msg.GetMsg(Env.GetCtx(), "VIS_OnLocators") + loc;
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }

                // SI_0750 If difference between the requisition qty and delivered qty is -ve or zero. system should not allow to complete inventory move and internal use.
                if (!IsReversal())
                {
                    StringBuilder delReq = new StringBuilder();
                    bool delivered = false;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        //MMovementLine mmLine = new MMovementLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        MMovementLine mmLine = lines[i];
                        if (mmLine.GetM_RequisitionLine_ID() != 0)
                        {
                            MRequisitionLine reqLine = new MRequisitionLine(GetCtx(), mmLine.GetM_RequisitionLine_ID(), Get_TrxName());
                            if (reqLine.GetQty() - reqLine.GetDTD001_DeliveredQty() <= 0)
                            {
                                delivered = true;
                                delReq.Append(mmLine.GetM_RequisitionLine_ID() + ",");
                            }
                        }
                    }

                    if (delivered)
                    {
                        sql.Clear();
                        sql.Append(DBFunctionCollection.ConcatnatedListOfRequisition(delReq.ToString()));
                        string req = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                        _processMsg = Msg.GetMsg(Env.GetCtx(), "RequisitionAlreadyDone") + ": " + req;
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }
            }
            #endregion

            List<ParentChildContainer> parentChildContainer = null;
            if (isContainerApplicable)
            {
                #region Check Container existence  in specified Warehouse and Locator
                // during completion - system will verify -- container avialble on line is belongs to same warehouse and locator
                // if not then not to complete this record

                // For From Container
                string sqlContainerExistence = DBFunctionCollection.MovementContainerNotMatched(GetM_Movement_ID()); ;
                string containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sqlContainerExistence, null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFound") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }

                // To Container
                sqlContainerExistence = DBFunctionCollection.MovementContainerToNotMatched(GetM_Movement_ID());
                containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sqlContainerExistence, null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFoundTo") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion

                //If User try to complete the Transactions if Movement Date is lesser than Last MovementDate on Product Container then we need to stop that transaction to Complete.
                #region Check MovementDate and Last Inventory Date Neha 31 Aug,2018

                string _qry = DBFunctionCollection.MovementContainerNotAvailable(GetM_Movement_ID());

                string misMatch = Util.GetValueOfString(DB.ExecuteScalar(_qry, null, Get_Trx()));
                if (!String.IsNullOrEmpty(misMatch))
                {
                    SetProcessMsg(misMatch + Msg.GetMsg(GetCtx(), "VIS_ContainerNotAvailable"));
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion


                // when user try to move full container, system will check qty on movement line and on Container.
                // if not matched, then not able to complete this record
                if (!IsReversal() && !IsMoveFullContainerPossible(GetM_Movement_ID()))
                {
                    return DocActionVariables.STATUS_INVALID;
                }

                parentChildContainer = new List<ParentChildContainer>();
                // during full move container - count no of Products on movement line in container must be equal to no. of Products on Tansaction for the same container.
                // even we compare Qty on movementline and transaction 
                if (!IsReversal())
                {
                    parentChildContainer = ProductChildContainer(GetM_Movement_ID());

                    string mismatch = IsMoveContainerProductCount(GetM_Movement_ID(), parentChildContainer);
                    if (!String.IsNullOrEmpty(mismatch))
                    {
                        // Qty in container has been increased/decreased  for Product : 
                        SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_MisMatchProduct") + mismatch);
                        return DocActionVariables.STATUS_INVALID;
                    }
                }

                //if (!IsReversal() && !ParentMoveFromPath(GetM_Movement_ID()))
                //{
                //    return DocActionVariables.STATUS_INVALID;
                //}

                //if (!IsReversal() && !ParentMoveToPath(GetM_Movement_ID()))
                //{
                //    return DocActionVariables.STATUS_INVALID;
                //}
            }

            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            // Set Document Date based on setting on Document Type
            SetCompletedDocumentDate();

            // To check weather future date records are available in Transaction window
            // this check implement after "SetCompletedDocumentDate" function, because this function overwrit movement date
            _processMsg = MTransaction.CheckFutureDateRecord(GetMovementDate(), Get_TableName(), GetM_Movement_ID(), Get_Trx());
            if (!string.IsNullOrEmpty(_processMsg))
            {
                return DocActionVariables.STATUS_INVALID;
            }

            // check column name new 12 jan 0 vikas
            int _count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT Count(AD_Column_ID) FROM AD_Column WHERE columnname = 'DTD001_SourceReserve' "));

            //	Outstanding (not processed) Incoming Confirmations ?
            MMovementConfirm[] confirmations = GetConfirmations(true);
            for (int i = 0; i < confirmations.Length; i++)
            {
                #region Outstanding (not processed) Incoming Confirmations
                MMovementConfirm confirm = confirmations[i];
                if (!confirm.IsProcessed())
                {
                    //SI_0630.1 :  Check status of confimation, if voided then we need to create confirmation again
                    if (confirm.GetDocStatus() == DOCSTATUS_Voided)
                    {
                        // compare confirmation record count, if it is not last record then need to check another record
                        // else create move confirmation again
                        if ((i + 1) != confirmations.Length)
                            continue;
                        else
                        {
                            //SI_0630.1 : create confirmation
                            confirm = MMovementConfirm.Create(this, false);
                        }
                        _processMsg = "Open: @M_MovementConfirm_ID@ - "
                            + confirm.GetDocumentNo();
                        SetProcessMsg(_processMsg);
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                    else if (confirm.GetDocStatus() != DOCSTATUS_Voided ||
                             confirm.GetDocStatus() != DOCSTATUS_Closed ||
                             confirm.GetDocStatus() != DOCSTATUS_Completed)
                    {
                        //SI_0630.2 : display message on UI "Confirmation is already pending for approval"
                        //_processMsg = Msg.GetMsg(GetCtx(), "VIS_ConfirmationPending") + confirm.GetDocumentNo();
                        _processMsg = "Open: @M_MovementConfirm_ID@ - " + confirm.GetDocumentNo();
                        SetProcessMsg(_processMsg);
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }
                // if found any Processed record then break the loop
                // because confirmation found in completed stage
                break;
                #endregion
            }

            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());

            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

            int countKarminati = Env.IsModuleInstalled("VA203_") ? 1 : 0;

            if (isContainerApplicable)
            {
                StringBuilder sqlContainer = new StringBuilder();
                // Update Last Inventory date on To Container in case of full container
                sqlContainer.Append("UPDATE M_ProductContainer SET DateLastInventory = " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                    @" WHERE M_ProductContainer_ID IN 
                                (SELECT DISTINCT NVL(Ref_M_ProductContainerTo_ID, 0) FROM M_MovementLine WHERE IsActive = 'Y' 
                                  AND MoveFullContainer = 'Y' AND M_Movement_ID =  " + GetM_Movement_ID() + ") ");
                DB.ExecuteQuery(sqlContainer.ToString(), null, Get_Trx());

                // need to update Organization / warehouse / locator / DateLastInventory / Parent container reference (if any)
                if (!IsReversal())
                {
                    UpdateContainerLocation(GetM_Movement_ID(), parentChildContainer);
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                MMovementLine line = lines[i];

                /* nnayak - Bug 1750251 : If you have multiple lines for the same product
                in the same Sales Order, or if the generate shipment process was generating
                multiple shipments for the same product in the same run, the first layer 
                was Getting consumed by all the shipments. As a result, the first layer had
                negative Inventory even though there were other positive layers. */
                // Ignore the Material Policy when is Reverse Correction
                if (!IsReversal())
                {
                    CheckMaterialPolicy(line);
                }

                MTransaction trxFrom = null;
                if (line.GetM_AttributeSetInstance_ID() == 0 || line.GetM_AttributeSetInstance_ID() != 0)
                {
                    MMovementLineMA[] mas = MMovementLineMA.Get(GetCtx(),
                        line.GetM_MovementLine_ID(), Get_TrxName());
                    for (int j = 0; j < mas.Length; j++)
                    {
                        Decimal? containerCurrentQty = 0;
                        MMovementLineMA ma = mas[j];
                        //
                        MStorage storageFrom = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                        if (storageFrom == null)
                            storageFrom = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                        //
                        MStorage storageTo = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(),
                            line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                        if (storageTo == null)
                            storageTo = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(),
                                line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                        //
                        // When Locator From and Locator To are different, then need to take impacts on Storage on hand qty
                        if (line.GetM_Locator_ID() != line.GetM_LocatorTo_ID())
                        {
                            storageFrom.SetQtyOnHand(Decimal.Subtract(storageFrom.GetQtyOnHand(), ma.GetMovementQty()));
                        }
                        if (line.GetM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 &&
                        {
                            storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), ma.GetMovementQty()));
                        }
                        if (!storageFrom.Save(Get_TrxName()))
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Storage From not updated (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //
                        // When Locator From and Locator To are different, then need to take impacts on Storage onhand qty
                        if (line.GetM_Locator_ID() != line.GetM_LocatorTo_ID())
                        {
                            storageTo.SetQtyOnHand(Decimal.Add(storageTo.GetQtyOnHand(), ma.GetMovementQty()));
                        }
                        if (!storageTo.Save(Get_TrxName()))
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Storage To not updated (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        #region Update Transaction / Future Date entry for From Locator
                        // Done to Update Current Qty at Transaction
                        Decimal? trxQty = 0;
                        //                        MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                        //                        int attribSet_ID = pro.GetM_AttributeSet_ID();
                        //                        isGetFromStorage = false;
                        //                        if (attribSet_ID > 0)
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM m_transaction
                        //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                        //                                    + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true, line.GetM_Locator_ID());
                        //                                isGetFromStorage = true;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM m_transaction
                        //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                        //                                     + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false, line.GetM_Locator_ID());
                        //                                isGetFromStorage = true;
                        //                            }
                        //                        }
                        //                        if (!isGetFromStorage)
                        //                        {
                        //                            trxQty = GetProductQtyFromStorage(line, line.GetM_Locator_ID());
                        //                        }

                        query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                           " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID();
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                        // get container Current qty from transaction
                        if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate(), line.GetM_Locator_ID(), line.GetM_ProductContainer_ID());
                        }

                        //
                        trxFrom = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                            MTransaction.MOVEMENTTYPE_MovementFrom,
                            line.GetM_Locator_ID(), line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                            Decimal.Negate(ma.GetMovementQty()), GetMovementDate(), Get_TrxName());
                        trxFrom.SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                        trxFrom.SetCurrentQty(trxQty + Decimal.Negate(ma.GetMovementQty()));
                        // set Material Policy Date
                        trxFrom.SetMMPolicyDate(ma.GetMMPolicyDate());
                        if (isContainerApplicable && trxFrom.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            trxFrom.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                            // update containr or withot container qty Current Qty 
                            trxFrom.SetContainerCurrentQty(containerCurrentQty + Decimal.Negate(ma.GetMovementQty()));
                        }
                        if (!trxFrom.Save())
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Transaction From not inserted (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        if (isContainerApplicable && trxFrom.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            String errorMessage = UpdateTransactionContainer(line, trxFrom, trxQty.Value + Decimal.Negate(ma.GetMovementQty()), line.GetM_Locator_ID(), line.GetM_ProductContainer_ID());
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                SetProcessMsg(errorMessage);
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            UpdateTransaction(line, trxFrom, trxQty.Value + Decimal.Negate(ma.GetMovementQty()), line.GetM_Locator_ID());
                        }
                        //UpdateCurrentRecord(line, trxFrom, Decimal.Negate(ma.GetMovementQty()), line.GetM_Locator_ID());
                        #endregion
                        /*************************************************************************************************/
                        if (Env.IsModuleInstalled("DTD001_"))
                        {
                            if (line.GetM_RequisitionLine_ID() > 0)
                            {
                                #region Requisition Case handled
                                decimal reverseRequisitionQty = 0;
                                MRequisitionLine reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                                //MRequisition req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());        // Trx used to handle query stuck problem
                                string reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                                + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                                if (!IsReversal())
                                {
                                    // ((qty Request) - (qty delivered)) >= (Attribute qty) then reduce (Attribute qty) from Requisition Ordered / Reserved qty
                                    if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= ma.GetMovementQty())
                                    {
                                        reverseRequisitionQty = ma.GetMovementQty();
                                    }
                                    // reduce diff ((qty Request) - (qty delivered)) on Requisition Ordered / Reserved qty
                                    else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < ma.GetMovementQty())
                                    {
                                        // when deleiverd is greater than requistion qty then make it as ZERO - no impacts goes to Requisition Ordered / Reserved qty
                                        if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                            reverseRequisitionQty = 0;
                                        else
                                            reverseRequisitionQty = Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty());
                                    }
                                    DB.ExecuteQuery("UPDATE M_MovementLine SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                              @" WHERE M_MovementLine_ID = " + line.GetM_MovementLine_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    // during reversal -- only actual Requisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                                    reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());

                                    // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                                    line.SetActualReqReserved(0);
                                    //reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                                }

                                reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), ma.GetMovementQty()));
                                reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), ma.GetMovementQty()));
                                if (!reqLine.Save())
                                {
                                    Get_Trx().Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = "Requisition Line not updated. " + pp.GetName();
                                    else
                                        _processMsg = "Requisition Line not updated";
                                    return DocActionVariables.STATUS_INVALID;
                                }

                                if (_count > 0)
                                {
                                    // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                                    int ResLocator_ID = 0;
                                    if (reqLine.Get_ColumnIndex("ReserveLocator_ID") > 0)
                                    {
                                        ResLocator_ID = reqLine.GetReserveLocator_ID();
                                    }
                                    else
                                    {
                                        ResLocator_ID = line.GetM_Locator_ID();
                                    }
                                    if (ResLocator_ID > 0 && reqStatus != "CL")
                                    {
                                        // JID_0657: Requistion is without ASI but on move selected the ASI system is minus the Reserved qty from ASI field but not removing the reserved qty without ASI
                                        MStorage ordStorage = MStorage.Get(GetCtx(), ResLocator_ID, line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_TrxName());
                                        ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                        if (!ordStorage.Save(Get_TrxName()))
                                        {
                                            Get_TrxName().Rollback();
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                _processMsg = pp.GetName();
                                            else
                                                _processMsg = "Storage From not updated (MA)";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                }

                                // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                                int OrdLocator_ID = 0;
                                if (reqLine.Get_ColumnIndex("OrderLocator_ID") > 0)
                                {
                                    OrdLocator_ID = reqLine.GetOrderLocator_ID();
                                }
                                else
                                {
                                    OrdLocator_ID = line.GetM_LocatorTo_ID();
                                }
                                if (OrdLocator_ID > 0)
                                {
                                    if (reqStatus != "CL" && countKarminati > 0)
                                    {
                                        MStorage newsg = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                                        //if (newsg == null)
                                        //{
                                        //    newsg = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                                        //}
                                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                                        if (!newsg.Save())
                                        {
                                            Get_Trx().Rollback();               //Arpit
                                            _processMsg = "Storage Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                    else if (reqStatus != "CL")
                                    {
                                        MStorage newsg = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                                        if (!newsg.Save(Get_Trx()))
                                        {
                                            Get_Trx().Rollback(); //Arpit
                                            _processMsg = "Storage Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                }
                                #endregion
                            }
                            #region Asset Work
                            string sql = "SELECT DTD001_ISCONSUMABLE FROM M_Product WHERE M_Product_ID=" + line.GetM_Product_ID();
                            if (Util.GetValueOfString(DB.ExecuteScalar(sql)) == "N")
                            {

                                //sql = "SELECT pcat.A_Asset_Group_ID FROM M_Product prd INNER JOIN M_Product_Category pcat ON prd.M_Product_Category_ID=pcat.M_Product_Category_ID WHERE prd.M_Product_ID=" + line.GetM_Product_ID();
                                //if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)

                                // Check Asset ID instead of Asset Group to consider Asset Movement.
                                if (line.GetA_Asset_ID() > 0)
                                {
                                    isAsset = true;
                                }
                                else
                                {
                                    isAsset = false;
                                }
                            }
                            else
                            {
                                isAsset = false;
                            }

                            if (isAsset == true)
                            {
                                DataSet DSReq = null;
                                if (line.GetM_RequisitionLine_ID() > 0)
                                {
                                    string NEWStr = "SELECT req.c_bpartner_id FROM m_requisitionline rqln INNER JOIN m_requisition req  ON req.m_requisition_id  = rqln.m_requisition_id  WHERE rqln.m_requisitionline_id=" + line.GetM_RequisitionLine_ID();
                                    DSReq = DB.ExecuteDataset(NEWStr, null, null);
                                }
                                if (line.GetA_Asset_ID() > 0)
                                {
                                    ast = new MAsset(GetCtx(), line.GetA_Asset_ID(), Get_Trx());
                                    if (Env.IsModuleInstalled("VAFAM_"))
                                    {
                                        MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                                        ast.CopyTo(aHist);
                                        aHist.SetA_Asset_ID(line.GetA_Asset_ID());
                                        if (!aHist.Save() && !ast.Save())
                                        {
                                            _processMsg = "Asset History Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                    ast.SetC_BPartner_ID(line.GetC_BPartner_ID());
                                    if (DSReq != null)
                                    {
                                        if (DSReq.Tables[0].Rows.Count > 0)
                                        {
                                            ast.SetC_BPartner_ID(Util.GetValueOfInt(DSReq.Tables[0].Rows[0]["c_bpartner_id"]));
                                        }
                                    }

                                    ast.SetM_Locator_ID(line.GetM_LocatorTo_ID());

                                    // VIS0060: Set Trx Organization from Movement Line to Asset
                                    if(Util.GetValueOfInt(line.Get_Value("AD_OrgTrx_ID")) > 0)
                                    {
                                        ast.Set_Value("AD_OrgTrx_ID", Util.GetValueOfInt(line.Get_Value("AD_OrgTrx_ID")));
                                    }
                                    ast.Save();
                                }
                                else
                                {
                                    Get_TrxName().Rollback();               //Arpit
                                    _processMsg = "Asset Not Selected For Movement Line";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            #endregion
                        }

                        #region Update Transaction / Future Date entry for To Locator
                        // Done to Update Current Qty at Transaction Decimal? trxQty = 0;
                        //                        pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                        //                        attribSet_ID = pro.GetM_AttributeSet_ID();
                        //                        isGetFromStorage = false;
                        //                        if (attribSet_ID > 0)
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM m_transaction
                        //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_LocatorTo_ID()
                        //                                    + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true, line.GetM_LocatorTo_ID());
                        //                                isGetFromStorage = true;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM m_transaction
                        //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_LocatorTo_ID()
                        //                                     + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false, line.GetM_LocatorTo_ID());
                        //                                isGetFromStorage = true;
                        //                            }
                        //                        }
                        //                        if (!isGetFromStorage)
                        //                        {
                        //                            trxQty = GetProductQtyFromStorage(line, line.GetM_LocatorTo_ID());
                        //                        }

                        query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_LocatorTo_ID() +
                           " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID();
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                        // get container Current qty from transaction
                        if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // when move full container, then check from container qty else check qty in To Container
                            containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate(), line.GetM_LocatorTo_ID(),
                                line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                        }

                        // Done to Update Current Qty at Transaction
                        // create transaction entry with To Org
                        MTransaction trxTo = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                            MTransaction.MOVEMENTTYPE_MovementTo,
                            line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                            ma.GetMovementQty(), GetMovementDate(), Get_TrxName());
                        trxTo.SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                        trxTo.SetCurrentQty(trxQty.Value + ma.GetMovementQty());
                        // set Material Policy Date
                        trxTo.SetMMPolicyDate(ma.GetMMPolicyDate());
                        if (isContainerApplicable && trxTo.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            // when move full container, then check from container qty else check qty in To Container
                            trxTo.SetM_ProductContainer_ID(line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                            // update containr or withot container qty Current Qty 
                            trxTo.SetContainerCurrentQty(containerCurrentQty + ma.GetMovementQty());
                        }
                        if (!trxTo.Save())
                        {
                            Get_Trx().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Transaction To not inserted (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        if (isContainerApplicable && trxTo.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // when move full container, then check from container qty else check qty in To Container
                            String errorMessage = UpdateTransactionContainer(line, trxTo, trxQty.Value + ma.GetMovementQty(), line.GetM_LocatorTo_ID(),
                                 line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                SetProcessMsg(errorMessage);
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            UpdateTransaction(line, trxTo, trxQty.Value + ma.GetMovementQty(), line.GetM_LocatorTo_ID());
                        }
                        //UpdateCurrentRecord(line, trxTo, ma.GetMovementQty(), line.GetM_LocatorTo_ID());
                        #endregion
                    }
                }
                //	Fallback - We have ASI
                if (trxFrom == null)
                {
                    #region WHEN ASI available on line -- when Data on Attribute Tab not found
                    Decimal? containerCurrentQty = 0;
                    MRequisitionLine reqLine = null;
                    //MRequisition req = null;
                    string reqStatus = "";
                    decimal reverseRequisitionQty = 0;
                    if (line.GetM_RequisitionLine_ID() > 0)
                    {
                        #region Requisition Case Handling
                        reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                        //req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());         // Trx used to handle query stuck problem
                        reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                                + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                        if (!IsReversal())
                        {
                            // ((qty Request) - (qty delivered)) >= (movement qty) then reduce (movement qty) from Requisition Ordered / Reserved qty
                            if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= line.GetMovementQty())
                            {
                                reverseRequisitionQty = line.GetMovementQty();
                            }
                            // reduce diff ((qty Request) - (qty delivered)) on Requisition Ordered / Reserved qty
                            else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < line.GetMovementQty())
                            {
                                // when delivered > request qty then no impact on Requisition Ordered / Reserved qty
                                if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                    reverseRequisitionQty = 0;
                                else
                                    reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                            }
                            DB.ExecuteQuery("UPDATE M_MovementLine SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                          @" WHERE M_MovementLine_ID = " + line.GetM_MovementLine_ID(), null, Get_Trx());
                        }
                        else
                        {
                            // during reversal -- only actual equisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                            reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());
                            // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                            line.SetActualReqReserved(0);
                        }

                        if (Env.IsModuleInstalled("DTD001_"))
                        {
                            reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), line.GetMovementQty()));
                            if (line.GetM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 && 
                            {
                                reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), line.GetMovementQty()));
                            }
                            reqLine.Save();
                        }
                        #endregion
                    }
                    MStorage storageFrom = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    if (storageFrom == null)
                        storageFrom = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    if (line.GetM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 && 
                    {
                        storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), line.GetMovementQty()));
                    }

                    MStorage storageTo = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstanceTo_ID(), Get_TrxName());
                    if (storageTo == null)
                        storageTo = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstanceTo_ID(), Get_TrxName());

                    // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                    if (Env.IsModuleInstalled("DTD001_") && line.GetM_RequisitionLine_ID() > 0)
                    {
                        #region Requisition cases handling
                        //SI_0657: Issue
                        if (_count > 0)
                        {
                            // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                            int ResLocator_ID = 0;
                            if (reqLine.Get_ColumnIndex("ReserveLocator_ID") > 0)
                            {
                                ResLocator_ID = reqLine.GetReserveLocator_ID();
                            }
                            else
                            {
                                ResLocator_ID = line.GetM_LocatorTo_ID();
                            }
                            if (ResLocator_ID > 0 && reqStatus != "CL")
                            {
                                // JID_0657: Requistion is without ASI but on move selected the ASI system is minus the Reserved qty from ASI field but not removing the reserved qty without ASI
                                MStorage ordStorage = MStorage.Get(GetCtx(), ResLocator_ID, line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_TrxName());
                                ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                if (!ordStorage.Save(Get_TrxName()))
                                {
                                    Get_TrxName().Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = pp.GetName();
                                    else
                                        _processMsg = "Storage not updated";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                        }
                        //                        
                        int OrdLocator_ID = 0;
                        if (reqLine.Get_ColumnIndex("OrderLocator_ID") > 0)
                        {
                            OrdLocator_ID = reqLine.GetOrderLocator_ID();
                        }
                        else
                        {
                            OrdLocator_ID = line.GetM_Locator_ID();
                        }
                        if (OrdLocator_ID > 0 && reqStatus != "CL")
                        {
                            #region Commented
                            //Update product Qty at storage and Checks Product have Attribute Set Or Not.
                            //MProduct newproduct = new MProduct(GetCtx(), line.GetM_Product_ID(), Get_Trx());
                            //if (countKarminati > 0 && line.GetM_RequisitionLine_ID() > 0)
                            //{
                            //    if ((newproduct.GetM_AttributeSet_ID() != null) && (newproduct.GetM_AttributeSet_ID() != 0))
                            //    {
                            //        MStorage newsg = null;
                            //        if (reqLine != null)
                            //        {
                            //            newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        else
                            //        {
                            //            newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        if (newsg != null && req != null)
                            //        {
                            //            Tuple<String, String, String> aInfo = null;
                            //            if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //            {
                            //                if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(reqStatus) != "CL")
                            //                {
                            //                    if (line.GetM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //                else if (Util.GetValueOfString(reqStatus) != "CL")
                            //                {
                            //                    if (line.GetM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        MStorage newsg = null;
                            //        if (reqLine != null)
                            //        {
                            //            newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        else
                            //        {
                            //            newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MStorage.GetCreate(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        if (newsg != null && req != null)
                            //        {
                            //            Tuple<String, String, String> aInfo = null;
                            //            if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //            {
                            //                if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(reqStatus) != "CL")
                            //                {
                            //                    if (line.GetM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //                else if (Util.GetValueOfString(reqStatus) != "CL")
                            //                {
                            //                    if (line.GetM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //else if ((newproduct.GetM_AttributeSet_ID() != null) && (newproduct.GetM_AttributeSet_ID() != 0))
                            //{
                            //    MStorage newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                            //    if (newsg != null && req != null)
                            //    {
                            //        Tuple<String, String, String> aInfo = null;
                            //        if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //        {
                            //            if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(reqStatus) != "CL")
                            //            {
                            //                if (line.GetM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //            else if (Util.GetValueOfString(reqStatus) != "CL")
                            //            {
                            //                if (line.GetM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    MStorage newsg = MStorage.Get(GetCtx(), line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), true, Get_TrxName());
                            //    if (newsg != null && req != null)
                            //    {
                            //        Tuple<String, String, String> aInfo = null;
                            //        if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //        {
                            //            if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(reqStatus) != "CL")
                            //            {
                            //                if (line.GetM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //            else if (Util.GetValueOfString(reqStatus) != "CL")
                            //            {
                            //                if (line.GetM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion

                            MStorage newsg = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), reqLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                            newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));

                            if (!newsg.Save(Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                _processMsg = "Storage Not Updated";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        #endregion
                    }

                    // When Locator From and Locator To are different, no need to take impacts on Storage
                    if (line.GetM_Locator_ID() != line.GetM_LocatorTo_ID())
                    {
                        storageFrom.SetQtyOnHand(Decimal.Subtract(storageFrom.GetQtyOnHand(), line.GetMovementQty()));
                    }
                    if (!storageFrom.Save(Get_TrxName()))
                    {
                        Get_TrxName().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Storage From not updated";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //
                    // When Locator From and Locator To are different, no need to take impacts on Storage
                    if (line.GetM_Locator_ID() != line.GetM_LocatorTo_ID())
                    {
                        storageTo.SetQtyOnHand(Decimal.Add(storageTo.GetQtyOnHand(), line.GetMovementQty()));
                    }
                    if (!storageTo.Save(Get_TrxName()))
                    {
                        Get_TrxName().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Storage To not updated";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    /***************************************************/
                    #region Asset Work
                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        string sql = "SELECT DTD001_ISCONSUMABLE FROM M_Product WHERE M_Product_ID=" + line.GetM_Product_ID();
                        if (Util.GetValueOfString(DB.ExecuteScalar(sql)) != "Y")
                        {
                            sql = "SELECT pcat.A_Asset_Group_ID FROM M_Product prd INNER JOIN M_Product_Category pcat ON prd.M_Product_Category_ID=pcat.M_Product_Category_ID WHERE prd.M_Product_ID=" + line.GetM_Product_ID();
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
                            {
                                isAsset = true;
                            }
                            else
                            {
                                isAsset = false;
                            }
                        }

                        else
                        {
                            isAsset = false;
                        }

                        if (isAsset == true)
                        {
                            if (line.GetA_Asset_ID() > 0)
                            {
                                ast = new MAsset(GetCtx(), line.GetA_Asset_ID(), Get_Trx());
                                if (Env.IsModuleInstalled("VAFAM_"))
                                {
                                    MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                                    ast.CopyTo(aHist);
                                    aHist.SetA_Asset_ID(line.GetA_Asset_ID());
                                    if (!aHist.Save() && !ast.Save())
                                    {
                                        _processMsg = "Asset History Not Updated";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                ast.SetC_BPartner_ID(line.GetC_BPartner_ID());
                                ast.SetM_Locator_ID(line.GetM_LocatorTo_ID());

                                // VIS0060: Set Trx Organization from Movement Line to Asset
                                if (Util.GetValueOfInt(line.Get_Value("AD_OrgTrx_ID")) > 0)
                                {
                                    ast.Set_Value("AD_OrgTrx_ID", Util.GetValueOfInt(line.Get_Value("AD_OrgTrx_ID")));
                                }
                                ast.Save();
                            }
                            else
                            {
                                Get_Trx().Rollback();// not to impact on Storare & Transaction of Product Move   ...Arpit
                                _processMsg = "Asset Not Selected For Movement Line";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                    }
                    #endregion
                    /********************************************************/

                    #region Update Transaction / Future Date entry for From Locator
                    // Done to Update Current Qty at Transaction
                    Decimal? trxQty = 0;
                    //                    MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                    //                    int attribSet_ID = pro.GetM_AttributeSet_ID();
                    //                    isGetFromStorage = false;
                    //                    if (attribSet_ID > 0)
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM m_transaction
                    //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //                                + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true, line.GetM_Locator_ID());
                    //                            isGetFromStorage = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM m_transaction
                    //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false, line.GetM_Locator_ID());
                    //                            isGetFromStorage = true;
                    //                        }
                    //                    }
                    //                    if (!isGetFromStorage)
                    //                    {
                    //                        trxQty = GetProductQtyFromStorage(line, line.GetM_Locator_ID());
                    //                    }

                    query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                           " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID();
                    trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                    // get container Current qty from transaction
                    if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate(), line.GetM_Locator_ID(), line.GetM_ProductContainer_ID());
                    }

                    //
                    trxFrom = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                        MTransaction.MOVEMENTTYPE_MovementFrom,
                        line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                        Decimal.Negate(line.GetMovementQty()), GetMovementDate(), Get_TrxName());
                    trxFrom.SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                    trxFrom.SetCurrentQty(trxQty + Decimal.Negate(line.GetMovementQty()));
                    // set Material Policy Date
                    trxFrom.SetMMPolicyDate(GetMovementDate());
                    if (isContainerApplicable && trxFrom.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        // Update Product Container on Transaction
                        trxFrom.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                        // update containr or withot container qty Current Qty 
                        trxFrom.SetContainerCurrentQty(containerCurrentQty + Decimal.Negate(line.GetMovementQty()));
                    }
                    if (!trxFrom.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Transaction From not inserted";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    if (isContainerApplicable && trxFrom.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        String errorMessage = UpdateTransactionContainer(line, trxFrom, trxQty.Value + Decimal.Negate(line.GetMovementQty()), line.GetM_Locator_ID(), line.GetM_ProductContainer_ID());
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            SetProcessMsg(errorMessage);
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        UpdateTransaction(line, trxFrom, trxQty.Value + Decimal.Negate(line.GetMovementQty()), line.GetM_Locator_ID());
                    }
                    //UpdateCurrentRecord(line, trxFrom, Decimal.Negate(line.GetMovementQty()), line.GetM_Locator_ID());
                    #endregion

                    #region Update Transaction / Future Date entry for To Locator
                    // Done to Update Current Qty at Transaction
                    //                    pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                    //                    attribSet_ID = pro.GetM_AttributeSet_ID();
                    //                    isGetFromStorage = false;
                    //                    if (attribSet_ID > 0)
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM m_transaction
                    //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_LocatorTo_ID()
                    //                                + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true, line.GetM_LocatorTo_ID());
                    //                            isGetFromStorage = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM m_transaction
                    //                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_LocatorTo_ID()
                    //                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false, line.GetM_LocatorTo_ID());
                    //                            isGetFromStorage = true;
                    //                        }
                    //                    }
                    //                    if (!isGetFromStorage)
                    //                    {
                    //                        trxQty = GetProductQtyFromStorage(line, line.GetM_LocatorTo_ID());
                    //                    }

                    query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_LocatorTo_ID() +
                           " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID();
                    trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                    // get container Current qty from transaction
                    if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate(), line.GetM_LocatorTo_ID(),
                            line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                    }

                    // Done to Update Current Qty at Transaction
                    // create transaction to with Locator To refernce
                    MTransaction trxTo = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                        MTransaction.MOVEMENTTYPE_MovementTo,
                        line.GetM_LocatorTo_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstanceTo_ID(),
                        line.GetMovementQty(), GetMovementDate(), Get_TrxName());
                    trxTo.SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                    trxTo.SetCurrentQty(trxQty + line.GetMovementQty());
                    // set Material Policy Date
                    trxTo.SetMMPolicyDate(GetMovementDate());
                    if (isContainerApplicable && trxTo.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        // Update Product Container on Transaction
                        trxTo.SetM_ProductContainer_ID(line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                        // update containr or withot container qty Current Qty 
                        trxTo.SetContainerCurrentQty(containerCurrentQty + line.GetMovementQty());
                    }
                    if (!trxTo.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Transaction To not inserted";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    if (isContainerApplicable && trxTo.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        String errorMessage = UpdateTransactionContainer(line, trxTo, trxQty.Value + line.GetMovementQty(), line.GetM_LocatorTo_ID(),
                               line.IsMoveFullContainer() ? line.GetM_ProductContainer_ID() : line.GetRef_M_ProductContainerTo_ID());
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            SetProcessMsg(errorMessage);
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        UpdateTransaction(line, trxTo, trxQty.Value + line.GetMovementQty(), line.GetM_LocatorTo_ID());
                    }
                    //UpdateCurrentRecord(line, trxTo, line.GetMovementQty(), line.GetM_LocatorTo_ID());
                    #endregion

                    #endregion
                }	//	Fallback

                // Enhanced by Amit for Cost Queue 10-12-2015
                if (client.IsCostImmediate())
                {
                    #region Costing Calculation

                    // create object of To Locator where we are moving products
                    MLocator locatorTo = MLocator.Get(GetCtx(), line.GetM_LocatorTo_ID());

                    // check costing method is LIFO or FIFO
                    String costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), line.GetM_Product_ID(), Get_Trx());

                    // is used to maintain cost of "move to" 
                    Decimal toCurrentCostPrice = 0;

                    #region get price from m_cost (Current Cost Price)
                    if (GetDescription() != null && GetDescription().Contains("{->"))
                    {
                        // do not update current cost price during reversal, this time reverse doc contain same amount which are on original document
                    }
                    else
                    {
                        // For From Warehouse
                        currentCostPrice = 0;
                        currentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetDTD001_MWarehouseSource_ID());

                        // For To Warehouse
                        toCurrentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), locatorTo.GetAD_Org_ID(),
                           line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), locatorTo.GetM_Warehouse_ID());

                        DB.ExecuteQuery("UPDATE M_MovementLine SET CurrentCostPrice = " + currentCostPrice + @" , ToCurrentCostPrice = " + toCurrentCostPrice + @"
                                                WHERE M_MovementLine_ID = " + line.GetM_MovementLine_ID(), null, Get_Trx());
                    }
                    #endregion

                    //query = "SELECT AD_Org_ID FROM M_Warehouse WHERE IsActive = 'Y' AND M_Warehouse_ID = " + GetM_Warehouse_ID();
                    // Get Org of "To Warehouse"
                    //int ToWarehouseOrg = MLocator.Get(GetCtx(), line.GetM_LocatorTo_ID()).GetAD_Org_ID();
                    //if (GetAD_Org_ID() != ToWarehouseOrg)
                    //{
                    product1 = new MProduct(GetCtx(), line.GetM_Product_ID(), Get_Trx());
                    if (product1.GetProductType().Equals("I")) // for Item Type product
                    {
                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product1, line.GetM_AttributeSetInstance_ID(),
                          "Inventory Move", null, null, line, null, null, 0, line.GetMovementQty(), Get_TrxName(), out conversionNotFoundInOut, optionalstr: "window"))
                        {
                            if (!conversionNotFoundMovement1.Contains(conversionNotFoundMovement))
                            {
                                conversionNotFoundMovement1 += conversionNotFoundMovement + " , ";
                            }
                            _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                            if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                            {
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else if (!IsReversal()) // not to update cost for reversed document
                        {
                            if (!String.IsNullOrEmpty(costingMethod))
                            {
                                if (line.GetMovementQty() > 0)
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                       line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), 2, line.GetM_MovementLine_ID(), costingMethod,
                                                       GetDTD001_MWarehouseSource_ID(), true, Get_Trx());
                                }

                                if (line.GetMovementQty() < 0)
                                {
                                    toCurrentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                           line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), 2, line.GetM_MovementLine_ID(), costingMethod,
                                                           locatorTo.GetM_Warehouse_ID(), true, Get_Trx());
                                }

                                DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  CASE WHEN MovementQty < 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                 @" END , ToCurrentCostPrice = CASE WHEN MovementQty > 0 THEN ToCurrentCostPrice ELSE" + toCurrentCostPrice + @" END
                                                 , IsCostImmediate = 'Y'
                                                WHERE M_InoutLine_ID = " + line.GetM_MovementLine_ID(), null, Get_Trx());
                            }

                            currentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetDTD001_MWarehouseSource_ID());

                            toCurrentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), locatorTo.GetAD_Org_ID(),
                                                 line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), locatorTo.GetM_Warehouse_ID());

                            DB.ExecuteQuery("UPDATE M_MovementLine SET PostCurrentCostPrice = " + currentCostPrice +
                                                @" , ToPostCurrentCostPrice = " + toCurrentCostPrice + @" , IsCostImmediate = 'Y' 
                                                WHERE M_MovementLine_ID = " + line.GetM_MovementLine_ID(), null, Get_Trx());

                        }
                    }
                    //}
                    #endregion
                }
                //End

            }	//	for all lines
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            // JID_1290: Set the document number from completede document sequence after completed (if needed)
            SetCompletedDocumentNo();

            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Set the document number feom Completed Doxument Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (IsReversal())
            {
                return;
            }

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
            // if Reversal document then no need to get Document no from Completed sequence
            if (IsReversal())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetMovementDate(DateTime.Now.Date);

                //	Std Period open?
                if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetAD_Org_ID()))
                {
                    throw new Exception("@PeriodClosed@");
                }
            }
        }

        /// <summary>
        /// verify - is it possible to move container from one locator to other
        /// </summary>
        /// <param name="movement_Id"></param>
        /// <returns></returns>
        public bool IsMoveFullContainerPossible(int movement_Id)
        {
            // when we complete record, and movement line having MoveFullContainer = true
            // system will check qty on transaction in Container must be same as qty on movement line (which represent qty on transaction  based on movement date)
            // if not matched, then we can not allow to user for its complete, he need to make a new Movement for move container
            //            string sql = @"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM 
            //                               (SELECT PName, ROW_NUMBER () OVER (ORDER BY PName ) RN, COUNT (*) OVER () CNT FROM 
            //                               (
            //                                SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
            //                                FROM m_movementline ml INNER JOIN m_movement m ON m.m_movement_id = ml.m_movement_id
            //                                INNER JOIN m_product p ON p.m_product_id = ml.m_product_id
            //                                LEFT JOIN m_attributesetinstance asi ON NVL(asi.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
            //                                 WHERE ml.MoveFullContainer ='Y' AND m.M_Movement_ID =" + movement_Id + @"
            //                                    AND (ml.movementqty) <>
            //                                     NVL((SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty
            //                                     FROM m_transaction t INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
            //                                      WHERE t.MovementDate <= 
            //                                            (Select MAX(movementdate) from m_transaction where 
            //                                            AD_Client_ID = m.AD_Client_ID  AND M_Locator_ID = ml.M_Locator_ID
            //                                            AND M_Product_ID = ml.M_Product_ID AND NVL(M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID, 0)
            //                                            AND NVL(M_ProductContainer_ID, 0) = NVL(ml.M_ProductContainer_ID, 0) )
            //                                       AND t.AD_Client_ID                     = m.AD_Client_ID
            //                                       AND t.M_Locator_ID                     = ml.M_Locator_ID
            //                                       AND t.M_Product_ID                     = ml.M_Product_ID
            //                                       AND NVL(t.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID, 0)
            //                                       AND NVL(t.M_ProductContainer_ID, 0)    =  NVL(ml.M_ProductContainer_ID, 0)  ), 0) 
            //                                       AND ROWNUM <= 100 )
            //                               ) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ";

            string sql = DBFunctionCollection.CheckMoveContainer(GetM_Movement_ID());
            log.Info(sql);
            string productName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (!string.IsNullOrEmpty(productName))
            {
                // Qty in Continer not matched with qty in container on spcified date : 
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_ContainerQtyNotMatched") + productName;
                SetProcessMsg(_processMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// during full move container - count no of Products on movement line in container must be equal to no. of Products on Tansaction for the same container.
        /// even we compare Qty on movementline and transaction 
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        public string IsMoveContainerProductCount(int movementId, List<ParentChildContainer> listParentChildContainer)
        {
            DataSet ds = null;
            StringBuilder misMatch = new StringBuilder();

            StringBuilder childContainer = new StringBuilder();
            if (listParentChildContainer.Count > 0)
            {
                childContainer.Clear();
                for (int i = 0; i < listParentChildContainer.Count; i++)
                {
                    if (String.IsNullOrEmpty(childContainer.ToString()))
                        childContainer.Append(listParentChildContainer[i].childContainer);
                    else
                        childContainer.Append(" , " + listParentChildContainer[i].childContainer);
                }
            }

            // get data from Transaction
            string sql = @"SELECT M_PRODUCT_ID, M_ATTRIBUTESETINSTANCE_ID, M_ProductContainer_ID, ContainerCurrentQty, Name FROM (
                            SELECT M_PRODUCT_ID, M_ATTRIBUTESETINSTANCE_ID,  M_ProductContainer_ID, ContainerCurrentQty, Name FROM 
                            (SELECT DISTINCT t.M_PRODUCT_ID, NVL(t.M_ATTRIBUTESETINSTANCE_ID, 0) AS M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID ,
                              First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID , t.M_ProductContainer_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC)  AS ContainerCurrentQty , p.Name
                               FROM M_Transaction t INNER JOIN M_Product p ON p.M_Product_ID   = t.M_Product_ID WHERE t.IsActive = 'Y' AND M_ProductContainer_ID IN 
                               (" + childContainer + @" ) )t WHERE ContainerCurrentQty <> 0 
                          UNION ALL
                            SELECT m.m_product_id, NVL(m.m_attributesetinstance_id, 0) AS m_attributesetinstance_id,
                              m.M_ProductContainer_ID, m.movementqty AS ContainerCurrentQty , p.Name
                              FROM m_movementline m INNER JOIN M_Product p ON p.M_Product_ID  = m.M_Product_ID
                              WHERE m_movement_id   = " + movementId + @" AND movefullcontainer = 'Y' ) final
                             GROUP BY M_PRODUCT_ID, M_ATTRIBUTESETINSTANCE_ID, M_ProductContainer_ID, ContainerCurrentQty, Name HAVING COUNT(1) = 1	";
            log.Info(sql);
            ds = DB.ExecuteDataset(sql, null, Get_Trx());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!misMatch.ToString().Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"])))
                    {
                        misMatch.Append(", " + Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                    }
                }
            }
            ds.Dispose();
            return misMatch.ToString();
        }

        /// <summary>
        /// Is used to update container location where we moved
        /// </summary>
        /// <param name="movementId">Record ID</param>
        /// <returns></returns>
        /// <writer>Amit Bansal</writer>
        public bool UpdateContainerLocation(int movementId, List<ParentChildContainer> listParentChildContainer)
        {
            if (listParentChildContainer.Count > 0)
            {
                for (int i = 0; i < listParentChildContainer.Count; i++)
                {
                    // check to warehouse id defined on movement header or not
                    int warehouseTo = Util.GetValueOfInt(listParentChildContainer[i].M_WarehouseTo_ID);
                    MLocator locator = MLocator.Get(GetCtx(), Util.GetValueOfInt(listParentChildContainer[i].M_LocatorTo_ID));
                    if (warehouseTo == 0)
                    {
                        // if to warehouse not defined on header then get warehouse id on behalf of "To Locator"
                        warehouseTo = locator.GetM_Warehouse_ID();
                    }

                    // update warehouse, locator and DateLastInventory on parent and its child record
                    int no = DB.ExecuteQuery(@"UPDATE M_ProductContainer SET AD_Org_ID = " + locator.GetAD_Org_ID() +
                                             @" , M_Warehouse_ID = " + warehouseTo +
                                             @" , M_Locator_ID = " + Util.GetValueOfInt(listParentChildContainer[i].M_LocatorTo_ID) +
                                             @" , DateLastInventory = " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                             @" WHERE M_ProductContainer_ID IN (" + Util.GetValueOfString(listParentChildContainer[i].childContainer) + ")", null, Get_Trx());

                    // update on target container - "Parent Containr" reference where we moved this container
                    no = DB.ExecuteQuery(@"UPDATE M_ProductContainer SET Ref_M_Container_ID = " + (Util.GetValueOfInt(listParentChildContainer[i].M_ProductContainerTo_ID) > 0
                                                                        ? Util.GetValueOfString(listParentChildContainer[i].M_ProductContainerTo_ID) : "null") +
                                             @" WHERE M_ProductContainer_ID = " + Util.GetValueOfInt(listParentChildContainer[i].TagetContainer_ID), null, Get_Trx());
                }
            }
            return true;
        }

        /// <summary>
        /// is used to get detail of all Child container including Target Container
        /// </summary>
        /// <param name="movementId">Record ID</param>
        /// <returns>list(ParentChildContainer) -- which contain All Child containe in movement, ToWarehouse, ToLocator, ToContainer, Target Container</returns>
        /// <writer>Amit Bansal</writer>
        public List<ParentChildContainer> ProductChildContainer(int movementId)
        {
            List<ParentChildContainer> listParentChildContainer = new List<ParentChildContainer>();
            ParentChildContainer parentChildContainer = null;
            bool ispostgerSql = DatabaseType.IsPostgre;
            // Get all UNIQUE movement line 
            DataSet dsMovementLine = DB.ExecuteDataset(@"SELECT DISTINCT ml.TargetContainer_ID AS ParentContainer, m.M_Warehouse_ID AS ToWarehouse,
                                               ml.M_LocatorTo_ID AS ToLocator,  ml.Ref_M_ProductContainerTo_ID AS ToContainer  
                                             FROM M_Movementline ml INNER JOIN M_Movement m ON ml.M_Movement_ID = m.M_Movement_ID 
                                             WHERE ml.MoveFullContainer = 'Y' AND ml.IsActive = 'Y' AND ml.M_Movement_ID = " + movementId, null, Get_Trx());
            if (dsMovementLine != null && dsMovementLine.Tables.Count > 0 && dsMovementLine.Tables[0].Rows.Count > 0)
            {
                StringBuilder childContainerId = new StringBuilder();
                for (int i = 0; i < dsMovementLine.Tables[0].Rows.Count; i++)
                {
                    parentChildContainer = new ParentChildContainer();
                    childContainerId.Clear();
                    string pathContainer = "";
                    String sql = "";
                    // Get path upto "Target Container" (top most parent to "target container")
                    if (ispostgerSql)
                    {
                        sql = @"WITH RECURSIVE pops (M_ProductContainer_id, level, name_path) AS (
                        SELECT  M_ProductContainer_id, 0,  ARRAY[M_ProductContainer_id]
                        FROM    M_ProductContainer
                        WHERE   Ref_M_Container_ID is null
                        UNION ALL
                        SELECT  p.M_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.M_ProductContainer_id)
                        FROM    M_ProductContainer p
                                INNER JOIN pops t0 ON t0.M_ProductContainer_id = p.Ref_M_Container_ID
                    )
                        SELECT    ARRAY_TO_STRING(name_path, '->')
                        FROM    pops  where m_productcontainer_id = " + Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]);
                        pathContainer = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                    }
                    else
                    {
                        pathContainer = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT sys_connect_by_path(m_productcontainer_id,'->') tree
                                            FROM m_productcontainer 
                                           WHERE m_productcontainer_id = " + Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]) + @"
                                            START WITH ref_m_container_id IS NULL CONNECT BY prior m_productcontainer_id = ref_m_container_id
                                           ORDER BY tree", null, Get_Trx()));
                    }

                    #region get All child of Target Container including target container reference
                    DataSet dsChildContainer = null;
                    if (ispostgerSql)
                    {
                        sql = @"WITH RECURSIVE pops (M_ProductContainer_id, level, name_path) AS (
                                SELECT  M_ProductContainer_id, 0,  ARRAY[M_ProductContainer_id]
                                FROM    M_ProductContainer
                                WHERE   Ref_M_Container_ID is null
                                UNION ALL
                                SELECT  p.M_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.M_ProductContainer_id)
                                FROM    M_ProductContainer p
                                        INNER JOIN pops t0 ON t0.M_ProductContainer_id = p.Ref_M_Container_ID )
                            SELECT  M_ProductContainer_id, level,  ARRAY_TO_STRING(name_path, '->')
                            FROM    pops  where ARRAY_TO_STRING(name_path, '->') like '" + pathContainer + "%'";
                        dsChildContainer = DB.ExecuteDataset(sql, null, Get_Trx());
                    }
                    else
                    {
                        dsChildContainer = DB.ExecuteDataset(@"SELECT tree, m_productcontainer_id FROM
                                                        (SELECT sys_connect_by_path(m_productcontainer_id,'->') tree , m_productcontainer_id
                                                         FROM m_productcontainer
                                                         START WITH ref_m_container_id IS NULL
                                                         CONNECT BY prior m_productcontainer_id = ref_m_container_id
                                                         ORDER BY tree  
                                                         )
                                                     WHERE tree LIKE ('" + pathContainer + "%') ", null, Get_Trx());
                    }
                    if (dsChildContainer != null && dsChildContainer.Tables.Count > 0 && dsChildContainer.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsChildContainer.Tables[0].Rows.Count; j++)
                        {
                            if (String.IsNullOrEmpty(childContainerId.ToString()))
                                childContainerId.Append(Util.GetValueOfString(dsChildContainer.Tables[0].Rows[j]["m_productcontainer_id"]));
                            else
                                childContainerId.Append("," + Util.GetValueOfString(dsChildContainer.Tables[0].Rows[j]["m_productcontainer_id"]));
                        }

                        parentChildContainer.childContainer = childContainerId.ToString();
                        parentChildContainer.M_WarehouseTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToWarehouse"]);
                        parentChildContainer.M_LocatorTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToLocator"]);
                        parentChildContainer.M_ProductContainerTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToContainer"]);
                        parentChildContainer.TagetContainer_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]);
                        listParentChildContainer.Add(parentChildContainer);
                    }
                    dsChildContainer.Dispose();
                    #endregion

                }
            }
            dsMovementLine.Dispose();
            return listParentChildContainer;
        }

        /// <summary>
        /// Is Used to get the Parent Container which are to be fully moved with child
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool ParentMoveFromPath(int movementId)
        //        {
        //            String path = null;
        //            // Get Path from Parent upto defined Product Container
        //            string sql = @"SELECT sys_connect_by_path(m_productcontainer_id,'->') tree ,  m_productcontainer_id
        //                            FROM m_productcontainer 
        //                           WHERE (m_productcontainer_id IN 
        //                            (SELECT M_ProductContainer_ID FROM M_MovementLine WHERE IsParentMove= 'Y' AND M_Movement_ID = " + movementId + @" ))
        //                            START WITH ref_m_container_id IS NULL CONNECT BY prior m_productcontainer_id = ref_m_container_id
        //                           ORDER BY tree ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    UpdateFromPath(path, movementId);
        //                }
        //            }
        //            return true;
        //        }

        /// <summary>
        /// update From path on movement header
        /// </summary>
        /// <param name="containerPath"></param>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool UpdateFromPath(String containerPath, int movementId)
        //        {
        //            String path = null;
        //            // Get Path from selected Product Container to all child
        //            string sql = @"SELECT tree FROM
        //                            (SELECT sys_connect_by_path(m_productcontainer_id,'->') tree
        //                             FROM m_productcontainer
        //                             START WITH ref_m_container_id IS NULL
        //                             CONNECT BY prior m_productcontainer_id = ref_m_container_id
        //                             ORDER BY tree  
        //                             )
        //                           WHERE tree LIKE ('" + containerPath + "%') ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (String.IsNullOrEmpty(path))
        //                        path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    else
        //                        path += "," + Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                }
        //            }
        //            int no = DB.ExecuteQuery("UPDATE M_Movement SET FromPath = FromPath || '|'  || '" + path + @"' WHERE M_Movement_ID = " + movementId, null, Get_Trx());
        //            return true;
        //        }

        /// <summary>
        /// Update To path on move header
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool ParentMoveToPath(int movementId)
        //        {
        //            String path = null;
        //            // Get Path from Parent upto defined Product Container To
        //            string sql = @"SELECT sys_connect_by_path(m_productcontainer_id,'->') tree ,  m_productcontainer_id
        //                            FROM m_productcontainer 
        //                           WHERE (m_productcontainer_id IN 
        //                            (SELECT Ref_M_ProductContainerTo_ID FROM M_MovementLine WHERE IsParentMove= 'Y' AND M_Movement_ID = " + movementId + @" ))
        //                            START WITH ref_m_container_id IS NULL CONNECT BY prior m_productcontainer_id = ref_m_container_id
        //                           ORDER BY tree ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (String.IsNullOrEmpty(path))
        //                        path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    else
        //                        path += "|" + Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                }
        //            }

        //            int no = DB.ExecuteQuery("UPDATE M_Movement SET ToPath = '" + path + @"' WHERE M_Movement_ID = " + movementId, null, Get_Trx());

        //            return true;
        //        }

        /// <summary>
        /// Get Parent container
        /// </summary>
        /// <param name="path"></param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        //public int GetContainerId(string path, int containerId)
        //{
        //    // represent array of "parent to child" container path 
        //    String[] splittedParentContainer = GetFromPath().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (splittedParentContainer.Length > 0)
        //    {
        //        String[] ParentToChildPath;
        //        for (int i = 0; i < splittedParentContainer.Length; i++)
        //        {
        //            // represent path of individual container (parent ot child)
        //            ParentToChildPath = splittedParentContainer[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (ParentToChildPath.Length > 0)
        //            {
        //                // find Parent Container Id
        //                int lastIndexValue = ParentToChildPath[0].LastIndexOf("->");
        //                int ParentContaineId = int.Parse(ParentToChildPath[0].Substring(lastIndexValue, ParentToChildPath.Length));
        //                if (ParentContaineId == containerId)
        //                {
        //                    int secondlastIndexValue = lastIndexValue > 0 ? ParentToChildPath[0].LastIndexOf("->", lastIndexValue - 1) : -1;
        //                    if (secondlastIndexValue == -1)
        //                    {
        //                        // itself parent
        //                        return 0;
        //                    }
        //                    else
        //                    {
        //                        // parent container id 
        //                        secondlastIndexValue += 2;
        //                        int parentContainerRefernce = int.Parse(ParentToChildPath[0].Substring(secondlastIndexValue, (lastIndexValue - secondlastIndexValue)));
        //                        return parentContainerRefernce;
        //                    }
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //        }
        //    }
        //    return 0;
        //}


        private void updateCostQueue(MProduct product, int M_ASI_ID, MAcctSchema mas,
          int Org_ID, MCostElement ce, decimal movementQty)
        {
            //MCostQueue[] cQueue = MCostQueue.GetQueue(product1, sLine.GetM_AttributeSetInstance_ID(), acctSchema, GetAD_Org_ID(), costElement, null);
            MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, null);
            if (cQueue != null && cQueue.Length > 0)
            {
                Decimal qty = movementQty;
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    qty = MCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    //if (cq == cQueue.Length - 1 && qty < 0) // last record
                    //{
                    //    queue.SetCurrentQty(qty);
                    //    if (!queue.Save())
                    //    {
                    //        ValueNamePair pp = VLogger.RetrieveError();
                    //        log.Info("Cost Queue not updated for  <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
                    //    }
                    //}
                    if (qty <= 0)
                    {
                        queue.Delete(true);
                        qty = Decimal.Negate(qty);
                    }
                    else
                    {
                        queue.SetCurrentQty(qty);
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Info("Cost Queue not updated for  <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
                        }
                    }
                    if (value)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="trxFrom"></param>
        /// <param name="qtyMove"></param>
        private void UpdateTransaction(MMovementLine line, MTransaction trxFrom, decimal qtyMove, int loc_ID)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;

            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyMove + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID()
                              + " ORDER BY movementdate ASC , m_transaction_id ASC ,  created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyMove + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID
                    //       + " AND M_AttributeSetInstance_ID =  0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate().Value.AddDays(1), true)
                             + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID + " AND M_AttributeSetInstance_ID = 0 "
                             + " ORDER BY movementdate ASC , m_transaction_id ASC ,  created ASC";
                }

                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), Get_TrxName());         // Trx used to handle query stuck problem
                                if (!inventory.IsInternalUse())
                                {
                                    inventoryLine.SetParent(inventory);
                                    inventoryLine.SetQtyBook(qtyMove);
                                    inventoryLine.SetOpeningStock(qtyMove);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                    }
                                    else
                                    {
                                        qtyMove = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != qtyMove)
                                        {
                                            storage.SetQtyOnHand(qtyMove);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(qtyMove + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                            }
                            else
                            {
                                qtyMove = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                           Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != qtyMove)
                                {
                                    storage.SetQtyOnHand(qtyMove);
                                    storage.Save();
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }

        private string UpdateTransactionContainer(MMovementLine sLine, MTransaction mtrx, decimal Qty, int loc_ID, int containerId)
        {
            string errorMessage = null;
            MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            Decimal containerCurrentQty = mtrx.GetContainerCurrentQty();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty , NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID()
                              + " ORDER BY movementdate ASC , m_transaction_id ASC, created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID + " AND M_AttributeSetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }
                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    if (inventoryLine.GetM_ProductContainer_ID() == containerId)
                                    {
                                        inventoryLine.SetParent(inventory);
                                        inventoryLine.SetQtyBook(containerCurrentQty);
                                        inventoryLine.SetOpeningStock(containerCurrentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                        }
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    if (trx.GetM_ProductContainer_ID() == containerId)
                                    {
                                        trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]))));
                                    }
                                    else
                                    {
                                        trx.SetCurrentQty(Decimal.Add(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["movementqty"])));
                                    }
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                        }
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                        if (sLine.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == containerId)
                                            containerCurrentQty = trx.GetContainerCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != Qty)
                                        {
                                            storage.SetQtyOnHand(Qty);
                                            if (!storage.Save())
                                            {
                                                Get_TrxName().Rollback();
                                                ValueNamePair pp = VLogger.RetrieveError();
                                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                {
                                                    _processMsg = pp.GetName();
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                                }
                                                else
                                                {
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == containerId)
                            {
                                trx.SetContainerCurrentQty(containerCurrentQty + trx.GetMovementQty());
                            }
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                Get_TrxName().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                {
                                    _processMsg = pp.GetName();
                                    return _processMsg;
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                }
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                                if (trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == containerId)
                                    containerCurrentQty = trx.GetContainerCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != Qty)
                                {
                                    storage.SetQtyOnHand(Qty);
                                    if (!storage.Save())
                                    {
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch (Exception e)
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
                errorMessage = Msg.GetMsg(GetCtx(), "ExceptionOccureOnUpdateTrx");
            }
            return errorMessage;
        }

        private void UpdateCurrentRecord(MMovementLine line, MTransaction trxM, decimal qtyDiffer, int loc_ID)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID;
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + loc_ID + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + loc_ID + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }

                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID;
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + loc_ID + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + loc_ID + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }
                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                }

                // int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
            }
            catch
            {
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromStorage(MMovementLine line, int loc_ID)
        {
            return 0;
            //MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            //int attribSet_ID = pro.GetM_AttributeSet_ID();
            //string sql = "";

            //if (attribSet_ID > 0)
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID
            //         + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
            //}
            //else
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + loc_ID;
            //}
            //return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// Get Latest Current Quantity based on movementdate
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromTransaction(MMovementLine line, DateTime? movementDate, bool isAttribute, int locatorId)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id  =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + locatorId + "   AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            return result;
        }

        /// <summary>
        /// This function is used to get current Qty based on Product Container
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <returns></returns>
        private Decimal? GetContainerQtyFromTransaction(MMovementLine line, DateTime? movementDate, int locatorId, int containerId)
        {
            Decimal result = 0;
            string sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, 
                        t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty
                           FROM M_Transaction t
                           WHERE t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND t.AD_Client_ID                       = " + line.GetAD_Client_ID() + @"
                           AND t.M_Locator_ID                       = " + locatorId + @"
                           AND t.M_Product_ID                       = " + line.GetM_Product_ID() + @"
                           AND NVL(t.M_AttributeSetInstance_ID , 0) = COALESCE(" + line.GetM_AttributeSetInstance_ID() + @",0)
                           AND NVL(t.M_ProductContainer_ID, 0)              = " + containerId;
            result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
            return result;
        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        private void CheckMaterialPolicy()
        {
            int no = MMovementLineMA.DeleteMovementMA(GetM_Movement_ID(), Get_TrxName());
            if (no > 0)
                log.Config("Delete old #" + no);
            MMovementLine[] lines = GetLines(false);

            MClient client = MClient.Get(GetCtx());

            //	Check Lines
            for (int i = 0; i < lines.Length; i++)
            {
                MMovementLine line = lines[i];

                Boolean needSave = false;

                //	Attribute Set Instance
                if (line.GetM_AttributeSetInstance_ID() == 0)
                {
                    MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
                    MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                        MMPolicy = client.GetMMPolicy();
                    //
                    MStorage[] storages = MStorage.GetAllWithASI(GetCtx(),
                        line.GetM_Product_ID(), line.GetM_Locator_ID(),
                        MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                    Decimal qtyToDeliver = line.GetMovementQty();
                    for (int ii = 0; ii < storages.Length; ii++)
                    {
                        MStorage storage = storages[ii];
                        if (ii == 0)
                        {
                            if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                            {
                                line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                                needSave = true;
                                log.Config("Direct - " + line);
                                qtyToDeliver = Env.ZERO;
                            }
                            else
                            {
                                log.Config("Split - " + line);
                                MMovementLineMA ma = new MMovementLineMA(line,
                                    storage.GetM_AttributeSetInstance_ID(),
                                    storage.GetQtyOnHand());
                                if (!ma.Save())
                                    ;
                                qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                            }
                        }
                        else	//	 create Addl material allocation
                        {
                            MMovementLineMA ma = new MMovementLineMA(line,
                                storage.GetM_AttributeSetInstance_ID(),
                                qtyToDeliver);
                            if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                qtyToDeliver = Env.ZERO;
                            else
                            {
                                ma.SetMovementQty(storage.GetQtyOnHand());
                                qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                            }
                            if (!ma.Save())
                                ;
                            log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                        }
                        if (Env.Signum(qtyToDeliver) == 0)
                            break;
                    }	//	 for all storages

                    //	No AttributeSetInstance found for remainder
                    if (Env.Signum(qtyToDeliver) != 0)
                    {
                        MMovementLineMA ma = new MMovementLineMA(line,
                            0, qtyToDeliver);
                        if (!ma.Save())
                            ;
                        log.Fine("##: " + ma);
                    }
                }	//	attributeSetInstance

                if (needSave && !line.Save())
                    log.Severe("NOT saved " + line);
            }	//	for all lines

        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        /// <param name="line">movement line</param>
        private void CheckMaterialPolicy(MMovementLine line)
        {
            int no = MMovementLineMA.DeleteMovementLineMA(line.GetM_MovementLine_ID(), Get_TrxName());
            if (no > 0)
                log.Config("Delete old #" + no);

            // check is any record available of physical Inventory after date Movement -
            // if available - then not to create Attribute Record - neither to take impacts on Container Storage
            if (isContainerApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM M_ContainerStorage WHERE IsPhysicalInventory = 'Y'
                 AND MMPolicyDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                      @" AND M_Product_ID = " + line.GetM_Product_ID() +
                      @" AND NVL(M_AttributeSetInstance_ID , 0) = " + line.GetM_AttributeSetInstance_ID() +
                      @" AND M_Locator_ID = " + line.GetM_Locator_ID() +
                      @" AND NVL(M_ProductContainer_ID , 0) = " + line.GetM_ProductContainer_ID(), null, Get_Trx())) > 0)
            {
                return;
            }

            MClient client = MClient.Get(GetCtx());
            Boolean needSave = false;
            //bool isLifoChecked = false;

            //	Attribute Set Instance
            //if (line.GetM_AttributeSetInstance_ID() == 0)
            //{
            MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
            MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
            String MMPolicy = pc.GetMMPolicy();
            if (MMPolicy == null || MMPolicy.Length == 0)
                MMPolicy = client.GetMMPolicy();

            //
            dynamic[] storages = null;
            if (isContainerApplicable)
            {
                storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
                 line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                 line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                 MClient.MMPOLICY_FiFo.Equals(MMPolicy), false, Get_TrxName());
            }
            else
            {
                storages = MStorage.GetWarehouse(GetCtx(), GetDTD001_MWarehouseSource_ID(), line.GetM_Product_ID(),
                         line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                            line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                            MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
            }

            Decimal qtyToDeliver = line.GetMovementQty();

            //LIFOManage:
            for (int ii = 0; ii < storages.Length; ii++)
            {
                dynamic storage = storages[ii];

                // when storage qty is less than equal to ZERO then continue to other record
                if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()) <= 0)
                    continue;

                if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()).CompareTo(qtyToDeliver) >= 0)
                {
                    MMovementLineMA ma = MMovementLineMA.GetOrCreate(line,
                    storage.GetM_AttributeSetInstance_ID(),
                    qtyToDeliver, isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Handle exception
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                    qtyToDeliver = Env.ZERO;
                    log.Fine("#" + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                }
                else
                {
                    log.Config("Split - " + line);
                    MMovementLineMA ma = MMovementLineMA.GetOrCreate(line,
                                storage.GetM_AttributeSetInstance_ID(),
                            isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand(),
                            isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Handle exception
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, (isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()));

                    log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                }
                if (Env.Signum(qtyToDeliver) == 0)
                    break;

                if (isContainerApplicable && ii == storages.Length - 1 && !MClient.MMPOLICY_FiFo.Equals(MMPolicy))
                {
                    storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
                                 line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                                 line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                                 MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                    ii = -1;
                }
            }	//	 for all storages

            //if (isContainerApplicable && !MClient.MMPOLICY_FiFo.Equals(MMPolicy) && !isLifoChecked && qtyToDeliver != 0)
            //{
            //    isLifoChecked = true;
            //    // Get Data from Container Storage based on Policy
            //    storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
            //  line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
            //  line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
            //  MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
            //    goto LIFOManage;
            //}

            if (Env.Signum(qtyToDeliver) != 0)
            {
                MMovementLineMA ma = MMovementLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), qtyToDeliver, GetMovementDate());
                if (!ma.Save(line.Get_Trx()))
                {
                    // Handle exception
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                    else
                        throw new ArgumentException("Attribute Tab not saved");
                }
                log.Fine("##: " + ma);
            }
            //}	//	attributeSetInstance


            if (needSave && !line.Save())
                log.Severe("NOT saved " + line);

        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }
            MDocType doctype = new MDocType(GetCtx(), GetC_DocType_ID(), Get_Trx());
            bool InTransit = (bool)doctype.Get_Value("IsInTransit");
            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MMovementLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MMovementLine line = lines[i];
                    Decimal old = line.GetMovementQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        // line.SetMovementQty(Env.ZERO); //can't make Move Qty Zero ..for reference see aftersave (Movement Line)
                        line.SetMovementQty(decimal.Negate(old)); //Arpit To set void record quantity to negative ..asked by Surya Sir
                        line.AddDescription("Void (" + old + ")");
                        line.Save(Get_TrxName());
                    }
                    //Amit 13-nov-2014
                    if (line.GetM_RequisitionLine_ID() > 0)
                    {
                        MRequisitionLine requisitionLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                        requisitionLine.SetDTD001_ReservedQty(Decimal.Subtract(requisitionLine.GetDTD001_ReservedQty(), old));
                        requisitionLine.Save(Get_Trx());

                        MStorage storageFrom = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                        if (storageFrom == null)
                            storageFrom = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), 0, Get_Trx());
                        storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), old));
                        storageFrom.Save(Get_Trx());
                    }
                    //Amit

                }
                // Added By Arpit on 9th Dec,2016 to set Void the document of Move Confirmation if found on the following conditions
                //  MMovement InvMove = new MMovement(GetCtx(), Get_ID(), Get_Trx());
                //  MDocType doctype = new MDocType(GetCtx(), GetC_DocType_ID(), Get_Trx());
                // bool InTransit = (bool)doctype.Get_Value("IsInTransit");
                if (InTransit == true)
                {
                    String Qry = "Select M_MovementConfirm_ID from M_MovementConfirm Where M_Movement_ID=" + Get_ID();
                    int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
                    if (MoveConf_ID > 0)
                    {
                        MMovementConfirm MoveConfirm = new MMovementConfirm(GetCtx(), MoveConf_ID, Get_Trx());
                        MoveConfirm.SetDocStatus(DOCACTION_Void);
                        MoveConfirm.SetDocAction(DOCACTION_Void);
                        MoveConfirm.SetProcessed(true);
                        if (!MoveConfirm.Save(Get_Trx()))
                        {
                            Get_Trx().Rollback();
                            _processMsg = "Error while Processing ";
                            return false;
                            // set Transaction RollBack here if not saved
                        }
                        Qry = string.Empty;
                        Qry = "Select M_MovementLineConfirm_ID from M_MovementLineConfirm Where M_MovementConfirm_ID=" + MoveConf_ID;
                        DataSet ds = DB.ExecuteDataset(Qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            MMovementLineConfirm lineMoveConf = null;
                            for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                lineMoveConf = null;
                                lineMoveConf = new MMovementLineConfirm(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_MovementLineConfirm_ID"]), Get_Trx());
                                lineMoveConf.SetProcessed(true);
                                lineMoveConf.Save(Get_Trx());
                            }
                        }

                    }
                }
                //Arpit
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean CloseIt()
        {
            log.Info(ToString());

            //	Close Not delivered Qty
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReverseCorrectIt()
        {
            log.Info(ToString());

            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }


            // when Inventory move is of Full Container, then not to reverse Inventory Move
            if (isContainerApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM M_MovementLine WHERE MoveFullContainer = 'Y' AND IsActive = 'Y' AND M_Movement_ID = " + GetM_Movement_ID(), null, Get_Trx())) > 0)
            {
                SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_FullContainernotReverse"));
                return false;
            }

            //start Added by Arpit Rai on 9th Dec,2016
            //  MDocType DocType = new MDocType(GetCtx(), GetC_DocType_ID(), Get_Trx());
            bool InTransit = (bool)dt.Get_Value("IsInTransit");
            if (InTransit == true)
            {
                String Qry = "Select M_MovementConfirm_ID from M_MovementConfirm Where M_Movement_ID=" + Get_ID();
                int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
                if (MoveConf_ID > 0)
                {
                    MMovementConfirm MoveConfirm = new MMovementConfirm(GetCtx(), MoveConf_ID, Get_Trx());
                    if (!MoveConfirm.ReverseCorrectIt())
                    {
                        Get_Trx().Rollback();
                        _processMsg = "Reversal ERROR: " + MoveConfirm.GetProcessMsg();
                        return false;
                    };
                    //}
                }
            }
            //end Arpit
            //	Deep Copy

            MMovement reversal = new MMovement(GetCtx(), 0, Get_TrxName());
            CopyValues(this, reversal, GetAD_Client_ID(), GetAD_Org_ID());

            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);	//	indicate reversals

            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetIsApproved(false);
            reversal.SetIsInTransit(false);
            reversal.SetPosted(false);
            reversal.SetProcessed(false);
            if (reversal.Get_ColumnIndex("ReversalDoc_ID") > 0 && reversal.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reversal.SetIsReversal(true);
                // Set Orignal Document Reference
                reversal.SetReversalDoc_ID(GetM_Movement_ID());
            }

            // for reversal document set Temp Document No to empty
            if (reversal.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                reversal.SetTempDocumentNo("");
            }

            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (!reversal.Save())
            {
                pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Could not create Movement Reversal , " + pp.GetName();
                else
                    _processMsg = "Could not create Movement Reversal";
                return false;
            }

            //	Reverse Line Qty
            MMovementLine[] oLines = GetLines(true);
            for (int i = 0; i < oLines.Length; i++)
            {
                MMovementLine oLine = oLines[i];
                MMovementLine rLine = new MMovementLine(GetCtx(), 0, Get_TrxName());
                CopyValues(oLine, rLine, oLine.GetAD_Client_ID(), oLine.GetAD_Org_ID());
                rLine.SetM_Movement_ID(reversal.GetM_Movement_ID());
                rLine.SetParent(reversal);
                rLine.SetMovementQty(Decimal.Negate(rLine.GetMovementQty()));
                rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                rLine.SetTargetQty(Env.ZERO);
                rLine.SetScrappedQty(Env.ZERO);
                rLine.SetConfirmedQty(Env.ZERO);
                rLine.SetProcessed(false);
                if (rLine.Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    // Set Original Line reference
                    rLine.SetReversalDoc_ID(oLine.GetM_MovementLine_ID());
                }
                rLine.SetActualReqReserved(oLine.GetActualReqReserved());
                if (!rLine.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Could not create Movement Reversal Line , " + pp.GetName();
                    else
                        _processMsg = "Could not create Movement Reversal Line";
                    return false;
                }

                //We need to copy Attribute MA
                MMovementLineMA[] mas = MMovementLineMA.Get(GetCtx(),
                        oLine.GetM_MovementLine_ID(), Get_TrxName());
                for (int j = 0; j < mas.Length; j++)
                {
                    MMovementLineMA ma = new MMovementLineMA(rLine,
                            mas[j].GetM_AttributeSetInstance_ID(),
                            Decimal.Negate(mas[j].GetMovementQty()), mas[j].GetMMPolicyDate());
                    if (!ma.Save(rLine.Get_TrxName()))
                    {
                        pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = "Could not create Movement Reversal Attribute , " + pp.GetName();
                        else
                            _processMsg = "Could not create Movement Reversal Attribute";
                        return false;
                    }
                }
            }
            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            MMovementLine[] mlines = GetLines(true);
            for (int i = 0; i < mlines.Length; i++)
            {
                MMovementLine mline = mlines[i];
                if (mline.GetA_Asset_ID() > 0)
                {
                    ast = new MAsset(GetCtx(), mline.GetA_Asset_ID(), Get_Trx());
                    if (Env.IsModuleInstalled("VAFAM_"))
                    {
                        MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                        ast.CopyTo(aHist);
                        aHist.SetA_Asset_ID(mline.GetA_Asset_ID());
                        if (!aHist.Save())
                        {
                            _processMsg = "Asset History Not Updated";
                            return false;
                        }
                    }
                    ast.SetM_Locator_ID(mline.GetM_Locator_ID());
                    ast.Save();
                }
            }
            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_TrxName()); //Pass Transaction Arpit

            //JID_0889: show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reversal.GetDocumentNo();

            //	Update Reversed (this)
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");
            // SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);	//	may come from void
            SetDocAction(DOCACTION_None);
            SetProcessed(true);
            Save(Get_Trx());
            ////start Added by Arpit Rai on 9th Dec,2016
            //MDocType DocType = new MDocType(GetCtx(), GetC_DocType_ID(), Get_Trx());
            //bool InTransit = (bool)DocType.Get_Value("IsInTransit");
            //if (InTransit == true)
            //{
            //    String Qry = "Select M_MovementConfirm_ID from M_MovementConfirm Where M_Movement_ID=" + Get_ID();
            //    int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
            //    if (MoveConf_ID > 0)
            //    {
            //        MMovementConfirm MoveConfirm = new MMovementConfirm(GetCtx(), MoveConf_ID, Get_Trx());
            //        if (!MoveConfirm.ReverseCorrectIt()) {
            //            _processMsg = "Reversal ERROR: " + MoveConfirm.GetProcessMsg();
            //            return false;
            //        };
            //        //}
            //    }
            //}
            ////end Arpit
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMovement[");
            sb.Append(Get_ID())
                .Append("-").Append(GetDocumentNo())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>C_Currency_ID</returns>
        public int GetC_Currency_ID()
        {
            //	MPriceList pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID());
            //	return pl.GetC_Currency_ID();
            return 0;
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
            _processMsg = processMsg;
        }
        #endregion

    }


    public class ParentChildContainer
    {
        public string childContainer { get; set; }
        public int M_WarehouseTo_ID { get; set; }
        public int M_LocatorTo_ID { get; set; }
        public int M_ProductContainerTo_ID { get; set; }
        public int TagetContainer_ID { get; set; }
    }
}