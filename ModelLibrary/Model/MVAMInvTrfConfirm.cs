/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Movement Confirmation Model
 * Class Used     : X_VAM_InvTrf_Confirm, DocAction(Interface)
 * Chronological Development
 * Veena         27-Oct-2009
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

namespace VAdvantage.Model
{
    /// <summary>
    /// Inventory Movement Confirmation Model
    /// </summary>
    public class MVAMInvTrfConfirm : X_VAM_InvTrf_Confirm, DocAction
    {
        /**	Confirm Lines					*/
        private MVAMInvTrfLineConfirm[] _lines = null;
        /**	Physical Inventory From	*/
        private MVAMInventory _inventoryFrom = null;
        /**	Physical Inventory To	*/
        private MVAMInventory _inventoryTo = null;
        /**	Physical Inventory Info	*/
        private String _inventoryInfo = null;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;

        //Arpit
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvTrfConfirm).FullName);
        //
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_InvTrf_LineConfirm_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMInvTrfConfirm(Ctx ctx, int VAM_InvTrf_LineConfirm_ID, Trx trxName)
            : base(ctx, VAM_InvTrf_LineConfirm_ID, trxName)
        {
            if (VAM_InvTrf_LineConfirm_ID == 0)
            {
                //	SetVAM_InventoryTransfer_ID (0);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                SetIsApproved(false);	// N
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MVAMInvTrfConfirm(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MVAMInvTrfConfirm(MVAMInventoryTransfer move)
            : this(move.GetCtx(), 0, move.Get_TrxName())
        {
            SetClientOrg(move);
            SetVAM_InventoryTransfer_ID(move.GetVAM_InventoryTransfer_ID());
        }

        /// <summary>
        /// Create Confirmation or return existing one
        /// </summary>
        /// <param name="move">movement</param>
        /// <param name="checkExisting">if false, new confirmation is created</param>
        /// <returns>Confirmation</returns>
        public static MVAMInvTrfConfirm Create(MVAMInventoryTransfer move, Boolean checkExisting)
        {
            if (checkExisting)
            {
                MVAMInvTrfConfirm[] confirmations = move.GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MVAMInvTrfConfirm confirm1 = confirmations[i];
                    if (confirm1 != null)
                        return confirm1;
                }
            }

            MVAMInvTrfConfirm confirm = new MVAMInvTrfConfirm(move);
            confirm.Save(move.Get_TrxName());
            MVAMInvTrfLine[] moveLines = move.GetLines(false);
            for (int i = 0; i < moveLines.Length; i++)
            {
                MVAMInvTrfLine mLine = moveLines[i];
                MVAMInvTrfLineConfirm cLine = new MVAMInvTrfLineConfirm(confirm);
                cLine.SetMovementLine(mLine);
                // setting QtyEntered in Target Qty on Confirmation Line
                cLine.SetTargetQty(mLine.GetQtyEntered());
                cLine.SetConfirmedQty(mLine.GetQtyEntered());
                cLine.Save(move.Get_TrxName());
            }
            // Change By Arpit Rai on 24th August,2017 To Check if VA Material Quality Control Module exists or not
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VA010_'", null, null)) > 0)
            {
                CreateConfirmParameters(move, confirm.GetVAM_InvTrf_Confirm_ID(), confirm.GetCtx());
            }
            return confirm;
        }
        #region Quality Control Work (Module Name-VA Material Qwality Control ,Prefix-VA010)
        // Change By Arpit to Create Parameters on Ship/Reciept Confirm Quality Control Tab 24th Aug,2017
        public static void CreateConfirmParameters(MVAMInventoryTransfer move, int M_MoveConfirm_ID, Ctx ctx)
        {
            String _Sql = @" SELECT movln.VAM_InvTrf_Line_ID,  mlnconf.VAM_InvTrf_LineConfirm_ID,  movln.VAM_Product_ID , 
                           pr.VA010_QualityPlan_ID ,  movln.MovementQty  FROM VAM_InvTrf_Line movln INNER JOIN VAM_Product pr  
                             ON (movln.VAM_Product_ID =pr.VAM_Product_ID) INNER JOIN VAM_InvTrf_LineConfirm mlnconf   
                             ON (movln.VAM_InvTrf_Line_id=mlnconf.VAM_InvTrf_Line_id) 
                             inner join VAM_InvTrf_Confirm mconf  on (mlnconf.VAM_InvTrf_Confirm_ID= mconf.VAM_InvTrf_Confirm_id)
                             WHERE movln.VAM_InventoryTransfer_ID =" + move.GetVAM_InventoryTransfer_ID() + " ORDER BY  Line";
            //" ORDER BY movln.VAM_Product_ID, pr.VA010_QualityPlan_ID ASC, movln.MovementQty,  Line";
            DataSet ds = new DataSet();
            //int _currentPlanQlty_ID = 0, CurrentLoopQty = 0, currProduct_ID = 0;
            int _currentPlanQlty_ID = 0, CurrentLoopQty = 0;
            List<int> CurrentLoopProduct = new List<int>();
            List<int> ProductQty = new List<int>();
            List<int> MoveConfirmLine_ID = new List<int>();

            try
            {

                ds = DB.ExecuteDataset(_Sql, null, move.Get_TrxName());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //if (_currentPlanQlty_ID == 0)
                        //{
                        _currentPlanQlty_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VA010_QualityPlan_ID"]);
                        //}
                        //currProduct_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]);
                        CurrentLoopProduct.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]));
                        ProductQty.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["MovementQty"]));
                        CurrentLoopQty = Util.GetValueOfInt(ds.Tables[0].Rows[i]["MovementQty"]);
                        MoveConfirmLine_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InvTrf_LineConfirm_ID"]));
                        //if (i < ds.Tables[0].Rows.Count - 1)
                        //{
                        //    if (_currentPlanQlty_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["VA010_QualityPlan_ID"])
                        //        && currProduct_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["VAM_Product_ID"]))
                        //    {
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        CreateParameters(CurrentLoopProduct, ProductQty, M_MoveConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, MoveConfirmLine_ID, ctx, move.Get_TrxName());
                        //        CurrentLoopProduct.Clear();
                        //        ProductQty.Clear();
                        //        _currentPlanQlty_ID = 0;
                        //        CurrentLoopQty = 0;
                        //        MoveConfirmLine_ID.Clear();
                        //    }
                        //}
                        //else
                        //{
                        CreateParameters(CurrentLoopProduct, ProductQty, M_MoveConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, MoveConfirmLine_ID, ctx, move.Get_TrxName());
                        CurrentLoopProduct.Clear();
                        ProductQty.Clear();
                        _currentPlanQlty_ID = 0;
                        CurrentLoopQty = 0;
                        MoveConfirmLine_ID.Clear();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, _Sql, ex);
            }
            finally
            {
                ds.Dispose();
                CurrentLoopProduct = ProductQty = MoveConfirmLine_ID = null;
                _currentPlanQlty_ID = CurrentLoopQty = 0;
                _Sql = String.Empty;
            }
        }
        //  // Change By Arpit to Create Parameters24th of August,2017
        //On the Basis of User defined % for each quantity of Product to verify
        public static void CreateParameters(List<int> _ProductList, List<int> _ProductQty, int M_MoveConfirm_ID, int VA010_QUalityPlan_ID, int CurrentQty, List<int> M_MoveConfirmLine_ID, Ctx ctx, Trx Trx_Name)
        {
            StringBuilder _sql = new StringBuilder();
            DataSet _ds = null;
            decimal _qtyPercentToVerify = 0;
            decimal _qtyFrom, _qtyTo, _qtyPercent = 0;

            try
            {
                _sql.Clear();
                _sql.Append(@"SELECT NVL(VA010_PercentQtyToVerify,0)VA010_PercentQtyToVerify,
                                NVL(VA010_ReceiptQtyFrom,0) VA010_ReceiptQtyFrom,
                                NVL(VA010_ReceiptQtyTo,0) VA010_ReceiptQtyTo FROM VA010_CheckingQty 
                              WHERE IsActive='Y' AND VA010_Qualityplan_ID=" + VA010_QUalityPlan_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID());


                _ds = DB.ExecuteDataset(_sql.ToString(), null, Trx_Name);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    for (Int32 k = 0; k < _ds.Tables[0].Rows.Count; k++)
                    {
                        _qtyFrom = Util.GetValueOfDecimal(_ds.Tables[0].Rows[k]["VA010_ReceiptQtyFrom"]);
                        _qtyTo = Util.GetValueOfDecimal(_ds.Tables[0].Rows[k]["VA010_ReceiptQtyTo"]);
                        _qtyPercent = Util.GetValueOfDecimal(_ds.Tables[0].Rows[k]["VA010_PercentQtyToVerify"]);
                        if (CurrentQty >= _qtyFrom && _qtyTo == 0)
                        {
                            _qtyPercentToVerify = _qtyPercent;
                            k = _ds.Tables[0].Rows.Count;
                        }
                        else if (CurrentQty >= _qtyFrom && CurrentQty <= _qtyTo)
                        {
                            _qtyPercentToVerify = _qtyPercent;
                            k = _ds.Tables[0].Rows.Count;
                        }
                        else
                        {
                            _qtyPercentToVerify = 100;
                        }

                    }
                }
                else
                {
                    _qtyPercentToVerify = 100;
                }
                _sql.Clear();
                _sql.Append(@"SELECT VA010_QualityParameters_ID, VA010_TestPrmtrList_ID FROM va010_AssgndParameters WHERE"
                + " VA010_QualityPlan_ID=" + VA010_QUalityPlan_ID + " AND IsActive='Y'");
                _ds.Clear();
                _ds = DB.ExecuteDataset(_sql.ToString(), null, Trx_Name);
                int _qty = 0;
                if (_ds != null)
                {
                    if (_ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < _ProductList.Count; i++)
                        {
                            _qty = 0;
                            _qty = (int)Math.Round((_ProductQty[i] * _qtyPercentToVerify) / 100, MidpointRounding.AwayFromZero);
                            if (_qty == 0)
                            {
                                _qty = _ProductQty[i];
                            }
                            if (_qty > _ProductQty[i])
                            {
                                _qty = _ProductQty[i];
                            }
                            for (int j = 0; j < _ds.Tables[0].Rows.Count; j++)
                            {
                                //Created Table object because not to use Mclass of Quality Control Module in our Base
                                MVAFTableView table = MVAFTableView.Get(ctx, "VA010_MoveConfParameters");
                                PO pos = table.GetPO(ctx, 0, Trx_Name);
                                pos.Set_ValueNoCheck("VAM_Product_ID", Util.GetValueOfInt(_ProductList[i]));
                                pos.Set_ValueNoCheck("VA010_QualityParameters_ID", Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA010_QualityParameters_ID"]));
                                pos.Set_ValueNoCheck("VAM_InvTrf_LineConfirm_ID", Util.GetValueOfInt(M_MoveConfirmLine_ID[i]));
                                pos.Set_ValueNoCheck("VA010_TestPrmtrList_ID", Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA010_TestPrmtrList_ID"]));
                                pos.Set_ValueNoCheck("VA010_QuantityToVerify", Util.GetValueOfDecimal(_qty));
                                pos.Set_ValueNoCheck("VAF_Client_ID", ctx.GetVAF_Client_ID());
                                pos.Set_ValueNoCheck("VAF_Org_ID", ctx.GetVAF_Org_ID());

                                if (pos.Save(Trx_Name))
                                {
                                    ;
                                }
                                else
                                {
                                    Trx_Name.Rollback();
                                    Trx_Name.Close();
                                }
                            }
                            DB.ExecuteQuery(" UPDATE VAM_InvTrf_LineConfirm SET VA010_QualCheckMark ='Y'  WHERE VAM_InvTrf_LineConfirm_ID=" + M_MoveConfirmLine_ID[i], null, Trx_Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, _sql.ToString(), ex);
            }
            finally
            {
                _sql.Clear();
                _ds.Dispose();
                _qtyPercentToVerify = _qtyFrom = _qtyTo = _qtyPercent = 0;
            }
        }
        //End Here 24th of August,2017
        #endregion
        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of lines</returns>
        public MVAMInvTrfLineConfirm[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            String sql = "SELECT * FROM VAM_InvTrf_LineConfirm "
                + "WHERE VAM_InvTrf_Confirm_ID=@mcid";
            List<MVAMInvTrfLineConfirm> list = new List<MVAMInvTrfLineConfirm>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@mcid", GetVAM_InvTrf_Confirm_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MVAMInvTrfLineConfirm(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _lines = new MVAMInvTrfLineConfirm[list.Count];
            _lines = list.ToArray();
            return _lines;
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
        /// Set Approved
        /// </summary>
        /// <param name="isApproved">approval</param>
        public new void SetIsApproved(Boolean isApproved)
        {
            if (isApproved && !IsApproved())
            {
                int VAF_UserContact_ID = GetCtx().GetVAF_UserContact_ID();
                MVAFUserContact user = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
                String info = user.GetName()
                    + ": "
                    + Msg.Translate(GetCtx(), "IsApproved")
                    + " - " + DateTime.Now.ToString();
                AddDescription(info);
            }
            base.SetIsApproved(isApproved);
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "VAM_InvTrf_Confirm_ID") + " " + GetDocumentNo();
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
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, GetVAB_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
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
            log.Info("UnlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean InvalidateIt()
        {
            log.Info("InvalidateIt - " + ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MVABYearPeriod.IsOpen(GetCtx(), GetUpdated(), MVABMasterDocType.DOCBASETYPE_MATERIALMOVEMENT, GetVAF_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetUpdated(), GetVAF_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }


            MVAMInvTrfLineConfirm[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //Boolean difference = false;
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    if (!lines[i].IsFullyConfirmed())
            //    {
            //        difference = true;
            //        break;
            //    }
            //}
            Boolean difference = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].GetTargetQty() != (lines[i].GetConfirmedQty() + lines[i].GetDifferenceQty() + lines[i].GetScrappedQty()))
                {
                    difference = true;
                    break;
                }
            }
            //	SetIsInDispute(difference);
            if (difference)
            {
                _processMsg = "@VAM_InvTrf_LineConfirm_ID@ <> @IsFullyConfirmed@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean ApproveIt()
        {
            log.Info("ApproveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean RejectIt()
        {
            log.Info("RejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info("CompleteIt - " + ToString());
            //
            MVAMInventoryTransfer move = new MVAMInventoryTransfer(GetCtx(), GetVAM_InventoryTransfer_ID(), Get_TrxName());
            MVAMInvTrfLineConfirm[] lines = GetLines(false);



            #region[Change By Sukhwinder on 11th October,2017 To Check if VA Material Quality Control Module exists or not, and then check if actual value at Quality Control tab exists or not]
            if (Env.IsModuleInstalled("VA010_"))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        sb.Append(lines[i].Get_ID() + ",");
                    }

                    string MVAMInvTrfLinesConfirm = sb.ToString().Trim(',');

                    if (!string.IsNullOrEmpty(MVAMInvTrfLinesConfirm))
                    {
                        string qry = DBFunctionCollection.MoveConfirmNoActualValue(MVAMInvTrfLinesConfirm);
                        string productsNoActualValue = Util.GetValueOfString(DB.ExecuteScalar(qry));
                        if (!string.IsNullOrEmpty(productsNoActualValue))
                        {
                            //_processMsg = productsNoActualValue + " is/are not verified with all the Quality Parameters." +
                            //              " Please fill actual value for the missing Quality Parameters in Quality Control. ";
                            _processMsg = productsNoActualValue + " " + Msg.GetMsg(GetCtx(), "VIS_NoActualValueInQC");
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Severe("Exception When Checking actual value at Quality Control Tab - " + ex.Message);
                }
            }
            #endregion



            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvTrfLineConfirm confirm = lines[i];
                confirm.Set_TrxName(Get_TrxName());
                //Added By Arpit
                if (GetDescription() != null && GetDescription().Contains("{->"))
                {
                    //if (!confirm.ProcessLineReverse())
                    //{
                    //    _processMsg = "ShipLine not saved - " + confirm;
                    //    return DocActionVariables.STATUS_INVALID;
                    //}
                }
                else if (!confirm.ProcessLine())
                {
                    _processMsg = "ShipLine not saved - " + confirm;
                    return DocActionVariables.STATUS_INVALID;
                }
                if (confirm.IsFullyConfirmed())
                {
                    confirm.SetProcessed(true);
                    confirm.Save(Get_TrxName());
                }
                else
                {
                    if (CreateDifferenceDoc(move, confirm))
                    {
                        confirm.SetProcessed(true);
                        confirm.Save(Get_TrxName());
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "completeIt - Scrapped=" + confirm.GetScrappedQty()
                            + " - Difference=" + confirm.GetDifferenceQty());

                        _processMsg = "Differnce Doc not created";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }	//	for all lines

            if (_inventoryInfo != null)
            {
                _processMsg = " @VAM_Inventory_ID@: " + _inventoryInfo;
                AddDescription(Msg.Translate(GetCtx(), "VAM_Inventory_ID")
                    + ": " + _inventoryInfo);
            }
            //Amit 21-nov-2014 (Reduce reserved quantity from requisition and warehouse distribution center)
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                MVAMInvTrfLine movementLine = null;
                MVAMRequisitionLine requisitionLine = null;
                MVAMStorage storage = null;
                for (int i = 0; i < lines.Length; i++)
                {
                    MVAMInvTrfLineConfirm confirm = lines[i];
                    if (confirm.GetDifferenceQty() > 0)
                    {
                        movementLine = new MVAMInvTrfLine(GetCtx(), confirm.GetVAM_InvTrf_Line_ID(), Get_Trx());
                        if (movementLine.GetVAM_RequisitionLine_ID() > 0)
                        {
                            requisitionLine = new MVAMRequisitionLine(GetCtx(), movementLine.GetVAM_RequisitionLine_ID(), Get_Trx());
                            requisitionLine.SetDTD001_ReservedQty(decimal.Subtract(requisitionLine.GetDTD001_ReservedQty(), confirm.GetDifferenceQty()));
                            if (!requisitionLine.Save(Get_Trx()))
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "DTD001_ReqNotUpdate");
                                // _processMsg = "Requisitionline not updated";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            storage = MVAMStorage.Get(GetCtx(), movementLine.GetVAM_Locator_ID(), movementLine.GetVAM_Product_ID(), movementLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            if (storage == null)
                            {
                                storage = MVAMStorage.Get(GetCtx(), movementLine.GetVAM_Locator_ID(), movementLine.GetVAM_Product_ID(), 0, Get_Trx());
                            }
                            storage.SetQtyReserved(decimal.Subtract(storage.GetQtyReserved(), confirm.GetDifferenceQty()));
                            if (!storage.Save(Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                _processMsg = Msg.GetMsg(GetCtx(), "DTD001_StorageNotUpdate");
                                //_processMsg = "Storage From not updated (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                    }
                }
            }
            //Amit
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            Save(Get_Trx());
            //Adde by Arpit To complete The Inventory Move if Move Confirmation is completed ,16th Dec,2016
            MVAMInventoryTransfer Mov = new MVAMInventoryTransfer(GetCtx(), GetVAM_InventoryTransfer_ID(), Get_Trx());
            if (move.GetDocStatus() != DOCSTATUS_Completed)
            {
                string Status = Mov.CompleteIt();
                if (Status == "CO")
                {
                    move.SetDocStatus(DOCSTATUS_Completed);
                    move.SetDocAction("CL");
                    move.SetProcessed(true);
                    move.Save(Get_Trx());
                }
                else
                {
                    Get_Trx().Rollback();
                    _processMsg = Mov.GetProcessMsg();
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            //End Here

            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Create Difference Document.
        ///	Creates one or two inventory lines
        /// </summary>
        /// <param name="move">movement</param>
        /// <param name="confirm">confirm line</param>
        /// <returns>true if created</returns>
        private Boolean CreateDifferenceDoc(MVAMInventoryTransfer move, MVAMInvTrfLineConfirm confirm)
        {
            string query = "";
            int result = 0;
            decimal currentQty = 0;
            MVAMInvTrfLine mLine = confirm.GetLine();

            //Added By amit 11-jun-2015 
            //Opening Stock , Qunatity Book => CurrentQty From Transaction of MovementDate
            //As On Date Count = Opening Stock - Diff Qty
            //Qty Count = Qty Book - Diff Qty
            query = "SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) + @" 
                           AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID();
            result = Util.GetValueOfInt(DB.ExecuteScalar(query));
            if (result > 0)
            {
                query = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID =
                            (SELECT MAX(VAM_Inv_Trx_ID)   FROM VAM_Inv_Trx
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) + @" 
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID() + @")
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID() + @")
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID();
                currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
            }
            else
            {
                query = "SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) + @" 
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID();
                result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                if (result > 0)
                {
                    query = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID =
                            (SELECT MAX(VAM_Inv_Trx_ID)   FROM VAM_Inv_Trx
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) + @" 
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID() + @")
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID() + @")
                            AND  VAM_Product_ID = " + mLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + mLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + mLine.GetVAM_PFeature_SetInstance_ID();
                    currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
                }
            }
            //End

            //	Difference - Create Inventory Difference for Source Location
            if (Env.ZERO.CompareTo(confirm.GetDifferenceQty()) != 0)
            {
                //	Get Warehouse for Source
                MVAMLocator loc = MVAMLocator.Get(GetCtx(), mLine.GetVAM_Locator_ID());
                if (_inventoryFrom != null
                    && _inventoryFrom.GetVAM_Warehouse_ID() != loc.GetVAM_Warehouse_ID())
                    _inventoryFrom = null;

                if (_inventoryFrom == null)
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), loc.GetVAM_Warehouse_ID());
                    _inventoryFrom = new MVAMInventory(wh);
                    _inventoryFrom.SetDescription(Msg.Translate(GetCtx(), "VAM_InvTrf_Confirm_ID") + " " + GetDocumentNo());
                    if (!_inventoryFrom.Save(Get_TrxName()))
                    {
                        _processMsg += "Inventory not created";
                        return false;
                    }
                    //	First Inventory
                    if (GetVAM_Inventory_ID() == 0)
                    {
                        SetVAM_Inventory_ID(_inventoryFrom.GetVAM_Inventory_ID());
                        _inventoryInfo = _inventoryFrom.GetDocumentNo();
                    }
                    else
                        _inventoryInfo += "," + _inventoryFrom.GetDocumentNo();
                }

                log.Info("createDifferenceDoc - Difference=" + confirm.GetDifferenceQty());

                MVAMInventoryLine line = new MVAMInventoryLine(_inventoryFrom, mLine.GetVAM_Locator_ID(), mLine.GetVAM_Product_ID(),
                        mLine.GetVAM_PFeature_SetInstance_ID(), confirm.GetDifferenceQty(), Env.ZERO);

                line.SetAdjustmentType("D");
                line.SetDifferenceQty(Util.GetValueOfDecimal(confirm.GetDifferenceQty()));
                line.SetQtyBook(currentQty);
                line.SetOpeningStock(currentQty);
                line.SetAsOnDateCount(Decimal.Subtract(Util.GetValueOfDecimal(line.GetOpeningStock()), Util.GetValueOfDecimal(confirm.GetDifferenceQty())));
                line.SetQtyCount(Decimal.Subtract(Util.GetValueOfDecimal(line.GetQtyBook()), Util.GetValueOfDecimal(confirm.GetDifferenceQty())));

                //JID_1185: System does not update the Qunatity and UoM on Physical Inventory Document. 
                line.Set_Value("VAB_UOM_ID", mLine.GetVAB_UOM_ID());
                line.Set_Value("QtyEntered", confirm.GetDifferenceQty());

                line.SetDescription(Msg.Translate(GetCtx(), "DifferenceQty"));
                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Inventory Line not created";
                    return false;
                }
                confirm.SetVAM_InventoryLine_ID(line.GetVAM_InventoryLine_ID());
            }	//	Difference

            //	Scrapped - Create Inventory Difference for TarGet Location
            if (Env.ZERO.CompareTo(confirm.GetScrappedQty()) != 0)
            {
                //	Get Warehouse for TarGet
                MVAMLocator loc = MVAMLocator.Get(GetCtx(), mLine.GetVAM_LocatorTo_ID());
                if (_inventoryTo != null
                    && _inventoryTo.GetVAM_Warehouse_ID() != loc.GetVAM_Warehouse_ID())
                    _inventoryTo = null;

                if (_inventoryTo == null)
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), loc.GetVAM_Warehouse_ID());
                    _inventoryTo = new MVAMInventory(wh);
                    _inventoryTo.SetDescription(Msg.Translate(GetCtx(), "VAM_InvTrf_Confirm_ID") + " " + GetDocumentNo());
                    if (!_inventoryTo.Save(Get_TrxName()))
                    {
                        _processMsg += "Inventory not created";
                        return false;
                    }
                    //	First Inventory
                    if (GetVAM_Inventory_ID() == 0)
                    {
                        SetVAM_Inventory_ID(_inventoryTo.GetVAM_Inventory_ID());
                        _inventoryInfo = _inventoryTo.GetDocumentNo();
                    }
                    else
                        _inventoryInfo += "," + _inventoryTo.GetDocumentNo();
                }

                log.Info("CreateDifferenceDoc - Scrapped=" + confirm.GetScrappedQty());
                MVAMInventoryLine line = new MVAMInventoryLine(_inventoryTo,
                    mLine.GetVAM_LocatorTo_ID(), mLine.GetVAM_Product_ID(), mLine.GetVAM_PFeature_SetInstance_ID(),
                    confirm.GetScrappedQty(), Env.ZERO);
                line.SetDescription(Msg.Translate(GetCtx(), "ScrappedQty"));

                //JID_1185: System does not update the Qunatity and UoM on Physical Inventory Document. 
                line.Set_Value("VAB_UOM_ID", mLine.GetVAB_UOM_ID());
                line.Set_Value("QtyEntered", confirm.GetDifferenceQty());

                // JID_0804 Ship receipt confirm with scrap Qty
                line.SetAdjustmentType("D");
                line.SetDifferenceQty(Util.GetValueOfDecimal(confirm.GetScrappedQty()));
                line.SetQtyBook(currentQty);
                line.SetOpeningStock(currentQty);
                line.SetAsOnDateCount(Decimal.Subtract(Util.GetValueOfDecimal(line.GetOpeningStock()), Util.GetValueOfDecimal(confirm.GetScrappedQty())));
                line.SetQtyCount(Decimal.Subtract(Util.GetValueOfDecimal(line.GetQtyBook()), Util.GetValueOfDecimal(confirm.GetScrappedQty())));

                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Inventory Line not created";
                    return false;
                }
                confirm.SetVAM_InventoryLine_ID(line.GetVAM_InventoryLine_ID());
            }	//	Scrapped

            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>false</returns>
        public Boolean VoidIt()
        {
            //Added BY arpit on 12th Dec,2016
            if (GetDocStatus() == "CO")
            {
                this.SetDocAction(DOCACTION_Void);
                this.SetDocStatus(DOCACTION_Void);
                if (!this.Save(Get_Trx()))
                {
                    _processMsg = "Could not void the document";
                    return false;
                }

                MVAMInvTrfConfirm RevMoveConf = new MVAMInvTrfConfirm(GetCtx(), 0, Get_Trx());
                CopyValues(this, RevMoveConf, GetVAF_Client_ID(), GetVAF_Org_ID());
                RevMoveConf.SetDocumentNo(GetDocumentNo());
                // RevMoveConf.SetDocStatus("RE");
                //  RevMoveConf.SetDocAction(DOCACTION_Void);
                // RevMoveConf.SetProcessed(true);
                RevMoveConf.AddDescription("{->" + GetDocumentNo() + ")");
                if (RevMoveConf.Save(Get_Trx()))
                {
                    MVAMInvTrfLineConfirm[] Lines = GetLines(true);
                    for (int i = 0; i < Lines.Length; i++)
                    {
                        MVAMInvTrfLineConfirm oLines = Lines[i];
                        MVAMInvTrfLineConfirm revLines = new MVAMInvTrfLineConfirm(GetCtx(), 0, Get_TrxName());
                        CopyValues(oLines, revLines, oLines.GetVAF_Client_ID(), oLines.GetVAF_Org_ID());
                        revLines.SetVAM_InvTrf_Confirm_ID(RevMoveConf.GetVAM_InvTrf_Confirm_ID());
                        revLines.SetConfirmedQty(Decimal.Negate(revLines.GetConfirmedQty()));
                        revLines.SetDifferenceQty(Decimal.Negate(revLines.GetDifferenceQty()));
                        revLines.SetScrappedQty(Decimal.Negate(revLines.GetScrappedQty()));
                        revLines.SetTargetQty(Decimal.Negate(revLines.GetTargetQty()));
                        revLines.SetProcessed(true);
                        if (!revLines.Save(Get_Trx()))
                        {
                            Get_Trx().Rollback();
                            _processMsg = "Could not create Movement Comfirmation Reversal Line";
                            return false;
                        }
                    }
                }
                else
                {
                    Get_Trx().Rollback();
                    _processMsg = "Could not create Move Confirmation Reversal";
                    return false;
                }
                if (RevMoveConf.CompleteIt() == DocActionVariables.ACTION_COMPLETE)
                {
                    RevMoveConf.SetDocStatus("RE");
                    RevMoveConf.SetDocAction(DOCACTION_Void);
                    RevMoveConf.SetProcessed(true);
                    RevMoveConf.Save(Get_Trx());
                }
                else
                {
                    _processMsg = "Reversal ERROR: ";
                    return false;
                }
            }
            //End Here
            log.Info("VoidIt - " + ToString());
            return true;
        }

        /// <summary>
        /// Close Document.
        ///	Cancel not delivered Qunatities
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean CloseIt()
        {
            log.Info("CloseIt - " + ToString());

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
            log.Info("ReverseCorrectIt - " + ToString());
            //Move Confirm Reversal Arpit
            MVAMInvTrfConfirm reversal = new MVAMInvTrfConfirm(GetCtx(), 0, Get_TrxName());
            CopyValues(this, reversal, GetVAF_Client_ID(), GetVAF_Org_ID());
            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetIsApproved(false);
            reversal.SetProcessed(false);
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (reversal.Save())
            {
                // Move Confirm Line
                DataSet ds = DB.ExecuteDataset("Select VAM_InvTrf_LineConfirm_ID from VAM_InvTrf_LineConfirm Where VAM_InvTrf_Confirm_ID =" + GetVAM_InvTrf_Confirm_ID());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        MVAMInvTrfLineConfirm linesfrom = new MVAMInvTrfLineConfirm(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InvTrf_LineConfirm_ID"]), Get_TrxName());
                        MVAMInvTrfLineConfirm linesTo = new MVAMInvTrfLineConfirm(GetCtx(), 0, Get_TrxName());
                        CopyValues(linesfrom, linesTo, GetVAF_Client_ID(), GetVAF_Org_ID());
                        linesTo.SetVAM_InvTrf_Confirm_ID(reversal.GetVAM_InvTrf_Confirm_ID());
                        linesTo.SetConfirmedQty(Decimal.Negate(linesfrom.GetConfirmedQty()));
                        linesTo.SetDifferenceQty(Decimal.Negate(linesfrom.GetDifferenceQty()));
                        linesTo.SetScrappedQty(Decimal.Negate(linesfrom.GetScrappedQty()));
                        linesTo.SetTargetQty(Decimal.Negate(linesfrom.GetTargetQty()));
                        linesTo.SetProcessed(true);
                        if (!linesTo.Save(Get_Trx()))
                        {
                            Get_Trx().Rollback();
                            _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                            return false;
                        }

                    }
                }
                if (reversal.CompleteIt() == DOCSTATUS_Completed)
                {
                    reversal.SetDocStatus("RE"); // set 
                    reversal.SetDocAction(DOCACTION_Void);
                    reversal.SetProcessed(true);
                    reversal.Save(Get_Trx());
                }
            }
            else
            {
                _processMsg = "Could not create Movement Confirm Reversal";
                return false;
            }
            SetDocStatus(DOCSTATUS_Voided);
            SetDocAction(DOCACTION_Void);
            Save(Get_Trx());
            //End
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReverseAccrualIt()
        {
            log.Info("ReverseAccrualIt - " + ToString());
            return true;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReActivateIt()
        {
            log.Info("ReActivateIt - " + ToString());
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
        /// <returns>VAF_UserContact_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>VAB_Currency_ID</returns>
        public int GetVAB_Currency_ID()
        {
            //	MVAMPriceList pl = MVAMPriceList.Get(GetCtx(), GetVAM_PriceList_ID());
            //	return pl.GetVAB_Currency_ID();
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

        }



        #endregion
    }
}
