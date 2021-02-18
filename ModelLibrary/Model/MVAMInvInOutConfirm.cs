/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMInvInOutConfirm
 * Purpose        : Shipment Confirmation Model
 * Class Used     : X_VAM_Inv_InOutConfirm, DocAction
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMInvInOutConfirm : X_VAM_Inv_InOutConfirm, DocAction
    {
        /**	Confirm Lines					*/
        private MVAMInvInOutLineConfirm[] _lines = null;
        /** Credit Memo to create			*/
        private MVABInvoice _creditMemo = null;
        /**	Physical Inventory to create	*/
        private MVAMInventory _inventory = null;

        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvInOutConfirm).FullName);

        ValueNamePair pp = null;

        /**
	 * 	Create Confirmation or return existing one
	 *	@param ship shipment
	 *	@param confirmType confirmation type
	 *	@param checkExisting if false, new confirmation is created
	 *	@return Confirmation
	 */
        public static MVAMInvInOutConfirm Create(MVAMInvInOut ship, String confirmType, Boolean checkExisting)
        {
            if (checkExisting)
            {
                MVAMInvInOutConfirm[] confirmations = ship.GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MVAMInvInOutConfirm confirm = confirmations[i];
                    if (confirm.GetConfirmType().Equals(confirmType) && confirm.GetDocStatus() == DOCSTATUS_Drafted
                        || confirm.GetConfirmType().Equals(confirmType) && confirm.GetDocStatus() == DOCSTATUS_InProgress
                        )
                    {
                        _log.Info("create - existing: " + confirm);
                        return confirm;
                    }
                }
            }

            MVAMInvInOutConfirm confirm1 = new MVAMInvInOutConfirm(ship, confirmType);
            confirm1.Save(ship.Get_TrxName());
            MVAMInvInOutLine[] shipLines = ship.GetLines(false);
            for (int i = 0; i < shipLines.Length; i++)
            {
                MVAMInvInOutLine sLine = shipLines[i];
                MVAMInvInOutLineConfirm cLine = new MVAMInvInOutLineConfirm(confirm1);
                cLine.SetInOutLine(sLine);
                //Arpit Start Here to Set UOM from the shipment/Receipt Line to Confirmation Lines
                cLine.SetVAB_UOM_ID(sLine.GetVAB_UOM_ID());
                cLine.SetTargetQty(sLine.GetQtyEntered());
                cLine.SetConfirmedQty(sLine.GetQtyEntered());
                //Arpit 
                cLine.Save(ship.Get_TrxName());
            }
            // Change By Arpit Rai on 24th August,2017 To Check if VA Material Quality Control Module exists or not
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VA010_'", null, null)) > 0)
            {
                if (confirmType == MVAMInvInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm)
                {
                    CreateConfirmParameters(ship, confirm1.GetVAM_Inv_InOutConfirm_ID(), confirm1.GetCtx());
                }
            }
            _log.Info("New: " + confirm1);
            return confirm1;
        }
        #region Quality Control Work (Module Name-VA Material Qwality Control ,Prefix-VA010)
        // Change By Arpit to Create Parameters on Ship/Reciept Confirm Quality Control Tab 24th Aug,2017
        public static void CreateConfirmParameters(MVAMInvInOut ship, int VAM_Inv_InOutConfirm_ID, Ctx ctx)
        {
            MVAMInvInOutLine[] shiplines = ship.GetLines(false);
            int _noOfLines = Util.GetValueOfInt(shiplines.Length);

            String _Sql = @"SELECT inotln.VAM_Inv_InOutLine_ID, iolnconf.VAM_Inv_InOutLineConfirm_ID,  inotln.VAM_Product_id   ,  pr.va010_qualityplan_id    ,  
                          inotln.qtyentered   FROM VAM_Inv_InOutLine inotln
                        INNER JOIN VAM_Product pr   ON (inotln.VAM_Product_ID=pr.VAM_Product_ID) 
                        INNER JOIN VAM_Inv_InOutLineConfirm iolnconf     ON (inotln.VAM_Inv_InOutLine_id= iolnconf.VAM_Inv_InOutLine_id)
                        inner join VAM_Inv_InOutConfirm  inout on  (iolnconf.VAM_Inv_InOutConfirm_id=inout.VAM_Inv_InOutConfirm_id) 
                         WHERE inotln.VAM_Inv_InOut_ID  =" + ship.GetVAM_Inv_InOut_ID() + " ORDER BY Line";
            //            +@" ORDER BY 
            //                          pr.VAM_Product_ID, pr.VA010_QualityPlan_ID ASC,inotln.qtyentered,  Line";
            DataSet ds = new DataSet();

            //int _currentPlanQlty_ID = 0, CurrentLoopQty = 0, currProduct_ID = 0;
            int _currentPlanQlty_ID = 0, CurrentLoopQty = 0;
            List<int> CurrentLoopProduct = new List<int>();
            List<int> ProductQty = new List<int>();
            List<int> InOutConfirmLine_ID = new List<int>();

            try
            {
                ds = DB.ExecuteDataset(_Sql, null, ship.Get_TrxName());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _noOfLines; i++)
                    {
                        //if (_currentPlanQlty_ID == 0)
                        //{
                        _currentPlanQlty_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VA010_QualityPlan_ID"]);
                        //}
                        //currProduct_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]);
                        CurrentLoopProduct.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]));
                        ProductQty.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyEntered"]));
                        CurrentLoopQty = Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyEntered"]);
                        InOutConfirmLine_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOutLineConfirm_ID"]));
                        //if (i < _noOfLines - 1)
                        //{
                        //    if (_currentPlanQlty_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["VA010_QualityPlan_ID"]) &&
                        //          currProduct_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["VAM_Product_ID"]))
                        //    {
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        CreateParameters(CurrentLoopProduct, ProductQty, VAM_Inv_InOutConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, InOutConfirmLine_ID, ctx, ship.Get_TrxName());
                        //        CurrentLoopProduct.Clear();
                        //        ProductQty.Clear();
                        //        _currentPlanQlty_ID = 0;
                        //        CurrentLoopQty = 0;
                        //        InOutConfirmLine_ID.Clear();
                        //    }
                        //}
                        //else
                        //{
                        CreateParameters(CurrentLoopProduct, ProductQty, VAM_Inv_InOutConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, InOutConfirmLine_ID, ctx, ship.Get_TrxName());
                        CurrentLoopProduct.Clear();
                        ProductQty.Clear();
                        _currentPlanQlty_ID = 0;
                        CurrentLoopQty = 0;
                        InOutConfirmLine_ID.Clear();
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
                _noOfLines = CurrentLoopQty = _currentPlanQlty_ID = 0;
                _Sql = string.Empty;
                ds.Dispose();
            }

        }
        //  // Change By Arpit to Create Parameters 24 Aug,2017
        //On the Basis of User defined % for each quantity of Product to verify
        public static void CreateParameters(List<int> _ProductList, List<int> _ProductQty, int VAM_Inv_InOutConfirm_ID, int VA010_QUalityPlan_ID, int CurrentQty, List<int> VAM_Inv_InOutConfirmLine_ID, Ctx ctx, Trx Trx_Name)
        {
            StringBuilder _sql = new StringBuilder();
            DataSet _ds = new DataSet();
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
                _sql.Append(@" SELECT VA010_QualityParameters_ID, VA010_TestPrmtrList_ID FROM va010_AssgndParameters WHERE"
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
                                MVAFTableView table = MVAFTableView.Get(ctx, "VA010_ShipConfParameters");
                                PO pos = table.GetPO(ctx, 0, Trx_Name);
                                pos.Set_ValueNoCheck("VAM_Product_ID", Util.GetValueOfInt(_ProductList[i]));
                                pos.Set_ValueNoCheck("VA010_QualityParameters_ID", Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA010_QualityParameters_ID"]));
                                pos.Set_ValueNoCheck("VAM_Inv_InOutLineConfirm_ID", Util.GetValueOfInt(VAM_Inv_InOutConfirmLine_ID[i]));
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
                            DB.ExecuteQuery(" UPDATE VAM_Inv_InOutLineConfirm SET VA010_QualCheckMark ='Y'  WHERE VAM_Inv_InOutLineConfirm_ID=" + VAM_Inv_InOutConfirmLine_ID[i], null, Trx_Name);
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
        #endregion
        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_Inv_InOutConfirm_ID id
         *	@param trxName transaction
         */
        public MVAMInvInOutConfirm(Ctx ctx, int VAM_Inv_InOutConfirm_ID, Trx trxName) :
            base(ctx, VAM_Inv_InOutConfirm_ID, trxName)
        {

            if (VAM_Inv_InOutConfirm_ID == 0)
            {
                //	setConfirmType (null);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetIsCancelled(false);
                SetIsInDispute(false);
                base.SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MVAMInvInOutConfirm(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param ship shipment
         *	@param confirmType confirmation type
         */
        public MVAMInvInOutConfirm(MVAMInvInOut ship, String confirmType)
            : this(ship.GetCtx(), 0, ship.Get_TrxName())
        {

            SetClientOrg(ship);
            SetVAM_Inv_InOut_ID(ship.GetVAM_Inv_InOut_ID());
            SetConfirmType(confirmType);
        }


        /**
         * 	Get Lines
         *	@param requery requery
         *	@return array of lines
         */
        public MVAMInvInOutLineConfirm[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            String sql = "SELECT * FROM VAM_Inv_InOutLineConfirm "
                + "WHERE VAM_Inv_InOutConfirm_ID=" + GetVAM_Inv_InOutConfirm_ID();
            List<MVAMInvInOutLineConfirm> list = new List<MVAMInvInOutLineConfirm>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMInvInOutLineConfirm(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }


            _lines = new MVAMInvInOutLineConfirm[list.Count];
            _lines = list.ToArray();
            return _lines;
        }	//	getLines

        // JID_0125_1: System should not allow to delete the Ship/Receipt confirmation if Ship/receipt reference is available.
        /// <summary>
        /// Before Delete - do not delete
        ///
        /// </summary>
        /// <returns>false </returns>
        protected override Boolean BeforeDelete()
        {
            if (GetVAM_Inv_InOut_ID() > 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotDelete"));
                return false;
            }
            return true;
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }

        /**
         * 	Get Name of ConfirmType
         *	@return confirm type
         */
        public String GetConfirmTypeName()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), CONFIRMTYPE_VAF_Control_Ref_ID, GetConfirmType());
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInvInOutConfirm[");
            sb.Append(Get_ID()).Append("-").Append(GetSummary())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "VAM_Inv_InOutConfirm_ID") + " " + GetDocumentNo();
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

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

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

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (GetCtx(), ReportEngine.INVOICE, getVAB_Invoice_ID());
            //	if (re == null)
            //return null;
            //	return re.getPDF(file);

            using (StreamWriter sw = file.CreateText())
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }

            return file;
        }

        /**
         * 	Set Approved
         *	@param IsApproved approval
         */
        public new void SetIsApproved(Boolean isApproved)
        {
            if (isApproved && !IsApproved())
            {
                int VAF_UserContact_ID = GetCtx().GetVAF_UserContact_ID();
                MVAFUserContact user = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
                String Info = user.GetName()
                    + ": "
                    + Msg.Translate(GetCtx(), "IsApproved")
                    + " - " + DateTime.Now.ToString();
                AddDescription(Info);
            }
            base.SetIsApproved(isApproved);
        }


        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public Boolean ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public Boolean UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public Boolean InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this,
                ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            /***********Compier comment
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), getVAB_DocTypesTarget_ID());

            //	Std Period open?
            if (!MVABYearPeriod.IsOpen(GetCtx(), getDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            ****/

            MVAMInvInOutLineConfirm[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Set dispute if not fully confirmed
            Boolean difference = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].IsFullyConfirmed())
                {
                    difference = true;
                    break;
                }
            }
            SetIsInDispute(difference);

            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public Boolean ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public Boolean RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /**
         * 	Complete Document
         * 	@return new status (Complete, In Progress, Invalid, Waiting ..)
         */
        public String CompleteIt()
        {
            //////By Sukhwinder on 21 Dec, 2017
            ////#region[Prevent from completing if on hand qty not available as per requirement and disallow negative is true at warehouse. By Sukhwinder on 21 Dec, 2017]
            ////string sql = "";
            ////sql = "SELECT ISDISALLOWNEGATIVEINV FROM VAM_Warehouse WHERE VAM_Warehouse_ID = (SELECT VAM_Warehouse_ID FROM VAM_Inv_InOut WHERE VAM_Inv_InOut_ID = " + Util.GetValueOfInt(GetVAM_Inv_InOut_ID()) + " )";
            ////string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

            ////if (disallow.ToUpper() == "Y")
            ////{
            ////    int[] ioLine = MVAMInvInOutLineConfirm.GetAllIDs("VAM_Inv_InOutLineConfirm", "VAM_Inv_InOutConfirm_ID  = " + GetVAM_Inv_InOutConfirm_ID(), Get_TrxName());                
            ////    int VAM_Locator_id = 0;
            ////    int VAM_Product_id = 0;
            ////    StringBuilder products = new StringBuilder();
            ////    StringBuilder locators = new StringBuilder();
            ////    bool check = false;
            ////    for (int i = 0; i < ioLine.Length; i++)
            ////    {
            ////        MVAMInvInOutLineConfirm iol = new MVAMInvInOutLineConfirm(Env.GetCtx(), ioLine[i], Get_TrxName());
            ////        MVAMInvInOutLine iol1 = new MVAMInvInOutLine(Env.GetCtx(), iol.GetVAM_Inv_InOutLine_ID() , Get_TrxName());
            ////        VAM_Locator_id = Util.GetValueOfInt(iol1.GetVAM_Locator_ID());
            ////        VAM_Product_id = Util.GetValueOfInt(iol1.GetVAM_Product_ID());



            ////        sql = "SELECT VAM_PFeature_Set_ID FROM VAM_Product WHERE VAM_Product_ID = " + VAM_Product_id;
            ////        int VAM_ProductFeature_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////        if (VAM_ProductFeature_ID == 0)
            ////        {
            ////            sql = "SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id;
            ////            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////            int qtyToMove = Util.GetValueOfInt(iol.GetConfirmedQty());
            ////            if (qty < qtyToMove)
            ////            {
            ////                check = true;
            ////                products.Append(VAM_Product_id + ", ");
            ////                locators.Append(VAM_Locator_id + ", ");
            ////                continue;
            ////            }
            ////        }
            ////        else
            ////        {
            ////            sql = "SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id + " AND VAM_PFeature_SetInstance_ID = " + iol1.GetVAM_PFeature_SetInstance_ID();
            ////            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////            int qtyToMove = Util.GetValueOfInt(iol.GetConfirmedQty());
            ////            if (qty < qtyToMove)
            ////            {
            ////                check = true;
            ////                products.Append(VAM_Product_id + ",");
            ////                locators.Append(VAM_Locator_id + ",");
            ////                continue;
            ////            }
            ////        }
            ////    }
            ////    if (check)
            ////    {
            ////        sql = "SELECT SUBSTR (SYS_CONNECT_BY_PATH (value , ', '), 2) CSV FROM (SELECT value , ROW_NUMBER () OVER (ORDER BY value ) rn, COUNT (*) over () CNT FROM "
            ////             + " (SELECT DISTINCT value FROM VAM_Locator WHERE VAM_Locator_ID IN(" + locators.ToString().Trim().Trim(',') + "))) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1";
            ////        string loc = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

            ////        sql = "SELECT SUBSTR (SYS_CONNECT_BY_PATH (Name , ', '), 2) CSV FROM (SELECT Name , ROW_NUMBER () OVER (ORDER BY Name ) rn, COUNT (*) over () CNT FROM "
            ////            + " VAM_Product WHERE VAM_Product_ID IN (" + products.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1";
            ////        string prod = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

            ////        _processMsg = Msg.GetMsg(Env.GetCtx(), "InsufficientQuantityFor: ") + prod + Msg.GetMsg(Env.GetCtx(), "OnLocators: ") + loc;
            ////        return DocActionVariables.STATUS_DRAFTED;
            ////    }
            ////}

            ////#endregion
            //////

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
            log.Info(ToString());
            //
            MVAMInvInOut inout = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            MVAMInvInOutLineConfirm[] lines = GetLines(false);

            /* created by sunil 19/9/2016*/
            if (Env.IsModuleInstalled("DTD001_"))
            {
                MVAMPackaging package = new MVAMPackaging(GetCtx(), inout.GetVAM_Packaging_ID(), Get_Trx());
                if (inout.GetVAM_Packaging_ID() > 0 && !package.IsDTD001_IsPackgConfirm())
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "PleaseConfirMVAMPackaging");
                    return DocActionVariables.STATUS_INVALID;

                }
            }
            //End

            //	Check if we need to split Shipment
            if (IsInDispute())
            {
                MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), inout.GetVAB_DocTypes_ID());
                if (dt.IsSplitWhenDifference())
                {
                    if (dt.GetVAB_DocTypesDifference_ID() == 0)
                    {
                        _processMsg = "No Split Document Type defined for: " + dt.GetName();
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //Arpit
                    decimal? splitQty = 0;
                    for (Int32 j = 0; j < lines.Length; j++)
                    {
                        splitQty += Util.GetValueOfDecimal(lines[j].GetDifferenceQty());
                    }
                    //Arpit to check if there is any difference qty on line of confirmation then we create the difference doc ..google doc issues of standard
                    if (splitQty > 0)
                    {
                        SplitInOut(inout, dt.GetVAB_DocTypesDifference_ID(), lines);
                    }
                    _lines = null;
                }
            }


            #region[Change By Sukhwinder on 10th October,2017 To Check if VA Material Quality Control Module exists or not, and then check if actual value at Quality Control tab exists or not]
            if (Env.IsModuleInstalled("VA010_"))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        sb.Append(lines[i].Get_ID() + ",");
                    }

                    string MVAMInvInOutLinesConfirm = sb.ToString().Trim(',');

                    if (!string.IsNullOrEmpty(MVAMInvInOutLinesConfirm))
                    {
                        string qry = DBFunctionCollection.ShipConfirmNoActualValue(MVAMInvInOutLinesConfirm);
                        string productsNoActualValue = Util.GetValueOfString(DB.ExecuteScalar(qry, null, Get_TrxName()));
                        if (!string.IsNullOrEmpty(productsNoActualValue))
                        {
                            //_processMsg = productsNoActualValue + " is/are not verified with all the Quality Parameters." +
                            //              " Please fill actual value for the missing Quality Parameters in Quality Control. ";
                            Get_TrxName().Rollback();
                            _processMsg = productsNoActualValue + " " + Msg.GetMsg(GetCtx(), "VIS_NoActualValueInQC");
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Get_TrxName().Rollback();
                    log.Severe("Exception When Checking actual value at Quality Control Tab - " + ex.Message);
                }
            }
            #endregion

            // Created By Sunil 17/9/2016
            // Complete Shipment
            //MVAMInvInOut io = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            //if (io.GetDocStatus() == DOCSTATUS_Reversed)
            //{
            //    GetCtx().SetContext("DifferenceQty_", "0");
            //    VoidIt(); //To set document void if MR is aleady in reversed case
            //    SetDocAction(DOCACTION_Void);
            //    return DocActionVariables.STATUS_VOIDED;
            //}
            //else if (io.GetDocStatus() == DOCSTATUS_Completed || io.GetDocStatus() == DOCSTATUS_Closed)
            //{
            //    SetProcessed(true);
            //    //Not to Complete MR/Shipment when it is already comepleted/ handled case when a completed record generates a confirmation
            //}
            //else
            //{
            MVAMInvInOut io = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            SetProcessed(true);
            if (!Save(Get_TrxName()))
            {
                GetCtx().SetContext("DifferenceQty_", "0");
                Get_Trx().Rollback();
                _processMsg = "Confirmation Not Completed";
                return DocActionVariables.STATUS_INVALID;
            }
            string Status_ = io.CompleteIt();
            if (Status_ == "CO")
            {

                io.SetProcessed(true);
                io.SetDocStatus(DocActionVariables.STATUS_COMPLETED);
                io.SetDocAction(DocActionVariables.ACTION_CLOSE);
                if (io.Save(Get_TrxName()))
                {
                    GetCtx().SetContext("DifferenceQty_", "0"); //To set difference Qty zero in context if document get comepleted
                }
                else
                {
                    GetCtx().SetContext("DifferenceQty_", "0");
                    Get_Trx().Rollback();
                    // SI_0713_1 - message should be correct
                    _processMsg = io.GetProcessMsg() + "Shipment Not Completed";
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            else
            {
                GetCtx().SetContext("DifferenceQty_", "0");
                Get_Trx().Rollback();
                // SI_0713_1 - message should be correct
                _processMsg = io.GetProcessMsg() + " Shipment Not Completed";
                return DocActionVariables.STATUS_INVALID;
            }

            //	All lines
            bool internalInventory = false; //Arpit
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvInOutLineConfirm confirmLine = lines[i];

                confirmLine.Set_TrxName(Get_TrxName());
                if (!confirmLine.ProcessLine(inout.IsSOTrx(), GetConfirmType()))
                {
                    GetCtx().SetContext("DifferenceQty_", "0");
                    _processMsg = "ShipLine not saved - " + confirmLine;
                    return DocActionVariables.STATUS_INVALID;
                }
                if (confirmLine.IsFullyConfirmed())
                {
                    confirmLine.SetProcessed(true);
                    confirmLine.Save(Get_TrxName());
                }
                else
                {
                    if (CreateDifferenceDoc(inout, confirmLine))
                    {
                        internalInventory = true;
                        confirmLine.SetProcessed(true);
                        confirmLine.Save(Get_TrxName());
                    }
                    else
                    {
                        GetCtx().SetContext("DifferenceQty_", "0");
                        log.Log(Level.SEVERE, "Scrapped=" + confirmLine.GetScrappedQty()
                            + " - Difference=" + confirmLine.GetDifferenceQty());

                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }	//	for all lines

            //To freeze Quality Control Lines
            FreezeQualityControlLines();

            //Arpit to complete the internal inventory if found any
            if (internalInventory && Util.GetValueOfInt(GetVAM_Inventory_ID()) > 0)
            {
                MVAMInventory intInv = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_TrxName());
                intInv.CompleteIt();
                intInv.SetDocStatus(DOCSTATUS_Completed);
                intInv.SetDocAction(DOCACTION_Close);
                if (!intInv.Save(Get_TrxName()))
                {
                    GetCtx().SetContext("DifferenceQty_", "0");
                    Get_TrxName().Rollback();
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            //End Here 
            if (_creditMemo != null)
                _processMsg += " @VAB_Invoice_ID@=" + _creditMemo.GetDocumentNo();
            if (_inventory != null)
                //   _processMsg += " @VAM_Inventory_ID@=" + _inventory.GetDocumentNo();
                //new 13 jan
                _processMsg += " Internal.Inventory= " + _inventory.GetDocumentNo();


            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this,
                ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                GetCtx().SetContext("DifferenceQty_", "0");
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            // if we don't processed the record then shipment can't be completed
            SetProcessed(true);
            if (!Save(Get_Trx()))
            {
                GetCtx().SetContext("DifferenceQty_", "0");
                Get_Trx().Rollback();
                _processMsg = "Ship/Receipt Confirmation Not saved";
                return DocActionVariables.STATUS_INVALID;
            }

            //}
            //end 
            //SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /**
         * 	Split Shipment into confirmed and dispute
         *	@param original original shipment
         *	@param VAB_DocTypes_ID target DocType
         *	@param confirmLines confirm lines
         */
        private void SplitInOut(MVAMInvInOut original, int VAB_DocTypes_ID, MVAMInvInOutLineConfirm[] confirmLines)
        {
            MVAMInvInOut split = new MVAMInvInOut(original, VAB_DocTypes_ID, original.GetMovementDate());
            split.AddDescription("Splitted from " + original.GetDocumentNo());
            split.SetIsInDispute(true);
            // new 13 jan
            int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT count(*) FROM VAF_Column clm INNER JOIN vaf_tableview tbl on (tbl.vaf_tableview_id=clm.vaf_tableview_id) where tbl.tablename='VAM_Inv_InOutLineConfirm' and clm.columnname = 'VAM_Locator_ID' "));
            //nnayak : Change for bug 1431337
            split.SetRef_InOut_ID(original.Get_ID());

            if (!split.Save(Get_TrxName()))
                throw new Exception("Cannot save Split");
            original.AddDescription("Split: " + split.GetDocumentNo());
            if (!original.Save(Get_TrxName()))
            {
                pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Cannot update original Shipment, " + pp.GetName();
                else
                    _processMsg = "Cannot update original Shipment";
                throw new Exception(_processMsg);
            }

            //	Go through confirmations 
            for (int i = 0; i < confirmLines.Length; i++)
            {
                MVAMInvInOutLineConfirm confirmLine = confirmLines[i];
                Decimal differenceQty = confirmLine.GetDifferenceQty();
                if (differenceQty.CompareTo(Env.ZERO) == 0)
                    continue;
                //
                MVAMInvInOutLine oldLine = confirmLine.GetLine();
                log.Fine("Qty=" + differenceQty + ", Old=" + oldLine);
                //
                MVAMInvInOutLine splitLine = new MVAMInvInOutLine(split);
                splitLine.SetVAB_OrderLine_ID(oldLine.GetVAB_OrderLine_ID());
                splitLine.SetVAB_UOM_ID(oldLine.GetVAB_UOM_ID());
                splitLine.SetDescription(oldLine.GetDescription());
                splitLine.SetIsDescription(oldLine.IsDescription());
                splitLine.SetLine(oldLine.GetLine());
                splitLine.SetVAM_PFeature_SetInstance_ID(oldLine.GetVAM_PFeature_SetInstance_ID());
                //new 13 jan vikas ,assigne by surya sir
                if (_count > 0)
                {
                    if (confirmLine.GetVAM_Locator_ID() > 0)
                    {
                        splitLine.SetVAM_Locator_ID(confirmLine.GetVAM_Locator_ID());
                    }
                    else
                    {
                        splitLine.SetVAM_Locator_ID(oldLine.GetVAM_Locator_ID());
                    }
                }
                else
                {
                    splitLine.SetVAM_Locator_ID(oldLine.GetVAM_Locator_ID());
                }
                //End
                //  splitLine.SetVAM_Locator_ID(oldLine.GetVAM_Locator_ID());
                splitLine.SetVAM_Product_ID(oldLine.GetVAM_Product_ID());
                splitLine.SetVAM_Warehouse_ID(oldLine.GetVAM_Warehouse_ID());
                splitLine.SetRef_InOutLine_ID(oldLine.GetRef_InOutLine_ID());
                splitLine.AddDescription("Split: from " + oldLine.GetMovementQty());
                //	Qtys
                splitLine.SetQty(differenceQty);		//	Entered/Movement

                /* Update QtyEntered/Qtymovement on Shipment Line to whom we are splitting*/
                /** Otherwise system can not save splited line because system founf mote qty to be shipped from Ordered Qty **/
                oldLine.AddDescription("Splitted: from " + oldLine.GetMovementQty());
                MProduct Product_ = new MProduct(GetCtx(), splitLine.GetVAM_Product_ID(), Get_TrxName());
                if (Product_.GetVAB_UOM_ID() != splitLine.GetVAB_UOM_ID())
                {
                    oldLine.SetQty(Decimal.Subtract(oldLine.GetQtyEntered(), differenceQty));
                }
                else
                    oldLine.SetQty(Decimal.Subtract(oldLine.GetMovementQty(), differenceQty));
                if (!oldLine.Save(Get_TrxName()))
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Cannot save Splited Line, " + pp.GetName();
                    else
                        _processMsg = "Cannot save Splited Line";
                    throw new Exception(_processMsg);
                }
                /**End**/

                // Now save Splitted Document
                if (!splitLine.Save(Get_TrxName()))
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Cannot save Split Line, " + pp.GetName();
                    else
                        _processMsg = "Cannot save Split Line";
                    throw new Exception(_processMsg);
                }

                //	Update Confirmation Line
                confirmLine.SetTargetQty(Decimal.Subtract(confirmLine.GetTargetQty(), differenceQty));
                confirmLine.SetDifferenceQty(Env.ZERO);
                if (!confirmLine.Save(Get_TrxName()))
                    throw new Exception("Cannot save Split Confirmation");
            }	//	for all confirmations

            _processMsg = "Split @VAM_Inv_InOut_ID@=" + split.GetDocumentNo()
                + " - @VAM_Inv_InOutConfirm_ID@=";

            //	Create Dispute Confirmation
            split.ProcessIt(DocActionVariables.ACTION_PREPARE);
            //	split.createConfirmation();
            split.Save(Get_TrxName());
            MVAMInvInOutConfirm[] splitConfirms = split.GetConfirmations(true);
            if (splitConfirms.Length > 0)
            {
                int index = 0;
                if (splitConfirms[index].IsProcessed())
                {
                    if (splitConfirms.Length > 1)
                        index++;	//	try just next
                    if (splitConfirms[index].IsProcessed())
                    {
                        _processMsg += splitConfirms[index].GetDocumentNo() + " processed??";
                        return;
                    }
                }
                splitConfirms[index].SetIsInDispute(true);
                splitConfirms[index].Save(Get_TrxName());
                _processMsg += splitConfirms[index].GetDocumentNo();
                //	Set Lines to unconfirmed
                MVAMInvInOutLineConfirm[] splitConfirmLines = splitConfirms[index].GetLines(false);
                for (int i = 0; i < splitConfirmLines.Length; i++)
                {
                    MVAMInvInOutLineConfirm splitConfirmLine = splitConfirmLines[i];
                    splitConfirmLine.SetScrappedQty(Env.ZERO);
                    splitConfirmLine.SetConfirmedQty(Env.ZERO);
                    splitConfirmLine.Save(Get_TrxName());
                }
            }
            else
            {
                _processMsg += "??";
            }

        }


        /**
         * 	Create Difference Document
         * 	@param inout shipment/receipt
         *	@param confirm confirm line
         *	@return true if created
         */
        private bool CreateDifferenceDoc(MVAMInvInOut inout, MVAMInvInOutLineConfirm confirm)
        {
            if (_processMsg == null)
                _processMsg = "";
            else if (_processMsg.Length > 0)
                _processMsg += "; ";
            //	Credit Memo if linked Document
            if (Env.Signum(confirm.GetDifferenceQty()) != 0
                && !inout.IsSOTrx() && !inout.IsReturnTrx() && inout.GetRef_InOut_ID() != 0)
            {
                log.Info("Difference=" + confirm.GetDifferenceQty());
                if (_creditMemo == null)
                {
                    _creditMemo = new MVABInvoice(inout, null);
                    _creditMemo.SetDescription(Msg.Translate(GetCtx(),
                        "VAM_Inv_InOutConfirm_ID") + " " + GetDocumentNo());
                    _creditMemo.SetVAB_DocTypesTarget_ID(MVABMasterDocType.DOCBASETYPE_APCREDITMEMO);
                    if (!_creditMemo.Save(Get_TrxName()))
                    {
                        _processMsg += "Credit Memo not created";
                        return false;
                    }
                    SetVAB_Invoice_ID(_creditMemo.GetVAB_Invoice_ID());
                }
                MVABInvoiceLine line = new MVABInvoiceLine(_creditMemo);
                line.SetShipLine(confirm.GetLine());
                //line.SetQty(confirm.GetDifferenceQty());	//	Entered/Invoiced
                //Arpit for invoice --qty should be converted while saving on invoice line
                MVAMInvInOutLine iol = new MVAMInvInOutLine(GetCtx(), confirm.GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                MProduct _Pro = new MProduct(GetCtx(), iol.GetVAM_Product_ID(), Get_TrxName());
                if (confirm.GetVAB_UOM_ID() != _Pro.GetVAB_UOM_ID())
                {
                    decimal? pc = MUOMConversion.ConvertProductFrom(GetCtx(), iol.GetVAM_Product_ID(), confirm.GetVAB_UOM_ID(), confirm.GetDifferenceQty());
                    line.SetQty(Util.GetValueOfDecimal(pc));	//	Entered/Invoiced
                }
                else
                    line.SetQty(confirm.GetDifferenceQty());	//	Entered/Invoiced
                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Credit Memo Line not created";
                    return false;
                }
                confirm.SetVAB_InvoiceLine_ID(line.GetVAB_InvoiceLine_ID());
            }

            //	Create Inventory Difference
            if (Env.Signum(confirm.GetScrappedQty()) != 0)
            {
                log.Info("Scrapped=" + confirm.GetScrappedQty());
                if (_inventory == null)
                {
                    //MWarehouse wh = MWarehouse.Get(GetCtx(), inout.GetVAM_Warehouse_ID());

                    MWarehouse wh = new MWarehouse(GetCtx(), inout.GetVAM_Warehouse_ID(), Get_TrxName());

                    _inventory = new MVAMInventory(wh);
                    _inventory.SetDescription(Msg.Translate(GetCtx(),
                        "VAM_Inv_InOutConfirm_ID") + " " + GetDocumentNo());
                    //vikas  new 13 jan 2016 1
                    _inventory.SetIsInternalUse(true);
                    if (_inventory.GetVAB_DocTypes_ID() == 0)
                    {
                        MVABDocTypes[] types = MVABDocTypes.GetOfDocBaseType(GetCtx(), MVABMasterDocType.DOCBASETYPE_MATERIALPHYSICALINVENTORY);
                        if (types.Length > 0)
                        {
                            // Get Internal Use Inv Doc Type
                            for (int i = 0; i < types.Length; i++)
                            {
                                int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAB_DocTypes WHERE IsActive='Y' AND  IsInternalUse='Y' AND VAB_DocTypes_ID=" + types[i].GetVAB_DocTypes_ID()));
                                if (_count > 0)
                                {
                                    _inventory.SetVAB_DocTypes_ID(types[i].GetVAB_DocTypes_ID());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @VAB_DocTypes_ID@"));
                            return false;
                        }
                    }
                    // 13 jan End 
                    if (!_inventory.Save(Get_TrxName()))
                    {
                        _processMsg += "Inventory not created";
                        return false;
                    }
                    SetVAM_Inventory_ID(_inventory.GetVAM_Inventory_ID());
                }
                MVAMInvInOutLine ioLine = confirm.GetLine();
                // Removed the code as already handled on before save..
                MVAMInventoryLine line = new MVAMInventoryLine(_inventory, ioLine.GetVAM_Locator_ID(), ioLine.GetVAM_Product_ID(), ioLine.GetVAM_PFeature_SetInstance_ID(),
                   confirm.GetScrappedQty(), Env.ZERO);

                //Below code commented because we have to convert the Quantity for update in wareHouse as mentioned above
                //MVAMInventoryLine line = new MVAMInventoryLine(_inventory,
                //    ioLine.GetVAM_Locator_ID(), ioLine.GetVAM_Product_ID(),
                //    ioLine.GetVAM_PFeature_SetInstance_ID(),
                //    confirm.GetScrappedQty(), Env.ZERO);
                //new 15 jan

                // Added by Bharat on 17 Jan 2019 to set new fields UOM and Qty Entered..
                line.Set_Value("VAB_UOM_ID", ioLine.GetVAB_UOM_ID());
                line.Set_Value("QtyEntered", line.GetQtyBook());
                line.SetQtyInternalUse(line.GetQtyBook());
                line.SetQtyBook(0);
                line.SetIsInternalUse(true);
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("DTD001_", out mInfo))
                {
                    int _charge = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Charge_ID FROM VAB_Charge WHERE isactive='Y' AND  DTD001_ChargeType='INV'"));
                    line.SetVAB_Charge_ID(_charge);
                }
                // End
                if (!line.Save(Get_TrxName()))
                {
                    pp = VLogger.RetrieveError();
                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                    {
                        _processMsg += pp.GetName();
                    }
                    else
                    {
                        _processMsg += "Inventory Line not created";
                    }
                    return false;
                }
                confirm.SetVAM_InventoryLine_ID(line.GetVAM_InventoryLine_ID());
            }

            //
            if (!confirm.Save(Get_TrxName()))
            {
                _processMsg += "Confirmation Line not saved";
                return false;
            }
            return true;
        }

        /**
         * 	Void Document.
         * 	@return false 
         */
        public bool VoidIt()
        {
            log.Info(ToString());

            //Arpit
            try
            {
                if (DOCSTATUS_Closed.Equals(GetDocStatus())
                    || DOCSTATUS_Reversed.Equals(GetDocStatus())
                    || DOCSTATUS_Voided.Equals(GetDocStatus()))
                {
                    _processMsg = "Document Closed: " + GetDocStatus();
                    return false;
                }
                if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                    || DOCSTATUS_Invalid.Equals(GetDocStatus())
                    || DOCSTATUS_InProgress.Equals(GetDocStatus())
                    || DOCSTATUS_Approved.Equals(GetDocStatus())
                    || DOCSTATUS_NotApproved.Equals(GetDocStatus())
                    || DOCSTATUS_Completed.Equals(GetDocStatus()))
                {
                    //	Set lines to 0                     
                    if (VoidConfirmLines())
                    {
                        //To freeze Quality Tabs if existing 
                        FreezeQualityControlLines();

                    }
                    else
                    {
                        Get_TrxName().Rollback();
                        Get_TrxName().Close();
                        return false;
                    }
                }
                SetProcessed(true);
                SetDocStatus(DOCSTATUS_Voided);
                SetDocAction(DOCACTION_None);
                Save(Get_TrxName());
            }
            catch
            {
                Get_TrxName().Rollback();
                Get_TrxName().Close();
                return false;
            }
            return true;
        }

        /**
         * 	void Document.
         * 	no return type 
         */
        //Void the lines of confirmation //Arpit
        public bool VoidConfirmLines()
        {
            MVAMInvInOutLineConfirm[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvInOutLineConfirm line = lines[i];
                line.SetDescription("Void (Confirmed Quantity:" + line.GetConfirmedQty() +
                                         ",Target Quantity:" + line.GetTargetQty() +
                                         ",Difference:" + line.GetDifferenceQty() +
                                         ",Scrapped Quantity:" + line.GetScrappedQty() + ")");
                line.SetConfirmedQty(Env.ZERO);
                line.SetDifferenceQty(Env.ZERO);
                line.SetScrappedQty(Env.ZERO);
                line.SetTargetQty(Env.ZERO);
                line.SetProcessed(true);
                if (line.Save(Get_TrxName()))
                {
                    ;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * 	Close Document.
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());

            //JID_1163: If user close ship/recipt confirm. System should Close "Internal Use Inventory" and "Material Recipt"
            MVAMInvInOut io = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            if (io.GetDocStatus() != DOCACTION_Close)
            {
                DocumentEngine.ProcessIt(io, DOCACTION_Close);
                if (!io.Save())
                {
                    pp = VLogger.RetrieveError();
                    _processMsg = Msg.GetMsg(GetCtx(), "NotSavedInOut") + pp != null ? ": " + pp.GetName() : "";
                }
            }

            if (GetVAM_Inventory_ID() > 0)
            {
                MVAMInventory _Inventory = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_TrxName());
                // If we Close  Ship/Recipt confirmation and Internal Use Inventory  is void or close. No effect.
                if (_Inventory.GetDocStatus() != DOCSTATUS_Voided && _Inventory.GetDocStatus() != DOCSTATUS_Reversed && _Inventory.GetDocStatus() != DOCSTATUS_Closed)
                {
                    DocumentEngine.ProcessIt(_Inventory, DOCACTION_Close);
                    if (!_Inventory.Save())
                    {
                        pp = VLogger.RetrieveError();
                        _processMsg = Msg.GetMsg(GetCtx(), "NotSavedInventory") + pp != null ? ": " + pp.GetName() : "";
                    }
                }
            }

            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return false 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return false;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return false 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return false 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /**
         * 	Get Summary
         *	@return Summary of Document
         */
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

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /**
         * 	Get Document Owner (Responsible)
         *	@return VAF_UserContact_ID
         */
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /**
         * 	Get Document Currency
         *	@return VAB_Currency_ID
         */
        public int GetVAB_Currency_ID()
        {
            //	MPriceList pl = MPriceList.get(getCtx(), getVAM_PriceList_ID());
            //	return pl.getVAB_Currency_ID();
            return 0;
        }
        //Arpit to freeze quality control Lines
        public void FreezeQualityControlLines()
        {
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VA010_'", null, Get_TrxName())) > 0)
            {
                String sql = "Select VA010_ShipConfParameters_ID from VA010_ShipConfParameters Where VAM_Inv_InOutLineConfirm_ID IN ("
                    + " SELECT VAM_Inv_InOutLineConfirm_ID FROM VAM_Inv_InOutLineConfirm Where VAM_Inv_InOutConfirm_ID=" + Get_ID() + ")";
                MVAFTableView table = MVAFTableView.Get(GetCtx(), "VA010_ShipConfParameters");
                PO pos = null;
                try
                {
                    IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr != null)
                    {
                        while (idr.Read())
                        {
                            pos = table.GetPO(GetCtx(), Util.GetValueOfInt(idr["VA010_ShipConfParameters_ID"]), Get_TrxName());
                            pos.Set_ValueNoCheck("IsProcessed", true);
                            if (pos.Save(Get_TrxName()))
                            {
                                ;
                            }
                        }
                    }
                    if (!idr.IsClosed)
                    {
                        idr.Dispose();
                        idr.Close();
                    }
                }
                catch (Exception ex)
                {
                    _log.Log(Level.SEVERE, sql, ex);
                }
            }
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
