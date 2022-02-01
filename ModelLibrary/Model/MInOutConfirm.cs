/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOutConfirm
 * Purpose        : Shipment Confirmation Model
 * Class Used     : X_M_InOutConfirm, DocAction
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
using System.Windows.Forms;
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
    public class MInOutConfirm : X_M_InOutConfirm, DocAction
    {
        /**	Confirm Lines					*/
        private MInOutLineConfirm[] _lines = null;
        /** Credit Memo to create			*/
        private MInvoice _creditMemo = null;
        /**	Physical Inventory to create	*/
        private MInventory _inventory = null;

        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MInOutConfirm).FullName);

        ValueNamePair pp = null;

        /**
	 * 	Create Confirmation or return existing one
	 *	@param ship shipment
	 *	@param confirmType confirmation type
	 *	@param checkExisting if false, new confirmation is created
	 *	@return Confirmation
	 */
        public static MInOutConfirm Create(MInOut ship, String confirmType, Boolean checkExisting)
        {
            if (checkExisting)
            {
                MInOutConfirm[] confirmations = ship.GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MInOutConfirm confirm = confirmations[i];
                    if (confirm.GetConfirmType().Equals(confirmType) && confirm.GetDocStatus() == DOCSTATUS_Drafted
                        || confirm.GetConfirmType().Equals(confirmType) && confirm.GetDocStatus() == DOCSTATUS_InProgress
                        )
                    {
                        _log.Info("create - existing: " + confirm);
                        return confirm;
                    }
                }
            }

            MInOutConfirm confirm1 = new MInOutConfirm(ship, confirmType);
            confirm1.Save(ship.Get_TrxName());
            MInOutLine[] shipLines = ship.GetLines(false);
            for (int i = 0; i < shipLines.Length; i++)
            {
                MInOutLine sLine = shipLines[i];
                MInOutLineConfirm cLine = new MInOutLineConfirm(confirm1);
                cLine.SetInOutLine(sLine);
                //Arpit Start Here to Set UOM from the shipment/Receipt Line to Confirmation Lines
                cLine.SetC_UOM_ID(sLine.GetC_UOM_ID());
                cLine.SetTargetQty(sLine.GetQtyEntered());
                cLine.SetConfirmedQty(sLine.GetQtyEntered());
                //Arpit 
                cLine.Save(ship.Get_TrxName());
            }
            // Change By Arpit Rai on 24th August,2017 To Check if VA Material Quality Control Module exists or not
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix='VA010_'", null, null)) > 0)
            {
                if (confirmType == MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm)
                {
                    CreateConfirmParameters(ship, confirm1.GetM_InOutConfirm_ID(), confirm1.GetCtx());
                }
            }
            _log.Info("New: " + confirm1);
            return confirm1;
        }
        #region Quality Control Work (Module Name-VA Material Qwality Control ,Prefix-VA010)
        // Change By Arpit to Create Parameters on Ship/Reciept Confirm Quality Control Tab 24th Aug,2017
        public static void CreateConfirmParameters(MInOut ship, int m_InoutConfirm_ID, Ctx ctx)
        {
            MInOutLine[] shiplines = ship.GetLines(false);
            int _noOfLines = Util.GetValueOfInt(shiplines.Length);

            String _Sql = @"SELECT inotln.M_InOutLine_ID, iolnconf.M_InOutLineConfirm_ID,  inotln.m_product_id   ,  pr.va010_qualityplan_id    ,  
                          inotln.qtyentered   FROM M_InOutLine inotln
                        INNER JOIN M_Product pr   ON (inotln.M_Product_ID=pr.M_Product_ID) 
                        INNER JOIN m_inoutlineconfirm iolnconf     ON (inotln.m_inoutline_id= iolnconf.m_inoutline_id)
                        inner join M_InOutConfirm  inout on  (iolnconf.M_InOutConfirm_id=inout.M_InOutConfirm_id) 
                         WHERE inotln.M_InOut_ID  =" + ship.GetM_InOut_ID() + " ORDER BY Line";
            //            +@" ORDER BY 
            //                          pr.M_Product_ID, pr.VA010_QualityPlan_ID ASC,inotln.qtyentered,  Line";
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
                        //currProduct_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                        CurrentLoopProduct.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]));
                        ProductQty.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyEntered"]));
                        CurrentLoopQty = Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyEntered"]);
                        InOutConfirmLine_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InOutLineConfirm_ID"]));
                        //if (i < _noOfLines - 1)
                        //{
                        //    if (_currentPlanQlty_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["VA010_QualityPlan_ID"]) &&
                        //          currProduct_ID == Util.GetValueOfInt(ds.Tables[0].Rows[i + 1]["M_Product_ID"]))
                        //    {
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        CreateParameters(CurrentLoopProduct, ProductQty, m_InoutConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, InOutConfirmLine_ID, ctx, ship.Get_TrxName());
                        //        CurrentLoopProduct.Clear();
                        //        ProductQty.Clear();
                        //        _currentPlanQlty_ID = 0;
                        //        CurrentLoopQty = 0;
                        //        InOutConfirmLine_ID.Clear();
                        //    }
                        //}
                        //else
                        //{
                        CreateParameters(CurrentLoopProduct, ProductQty, m_InoutConfirm_ID, _currentPlanQlty_ID, CurrentLoopQty, InOutConfirmLine_ID, ctx, ship.Get_TrxName());
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
        public static void CreateParameters(List<int> _ProductList, List<int> _ProductQty, int M_InoutConfirm_ID, int VA010_QUalityPlan_ID, int CurrentQty, List<int> M_InOutConfirmLine_ID, Ctx ctx, Trx Trx_Name)
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
                              WHERE IsActive='Y' AND VA010_Qualityplan_ID=" + VA010_QUalityPlan_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID());

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
                                MTable table = MTable.Get(ctx, "VA010_ShipConfParameters");
                                PO pos = table.GetPO(ctx, 0, Trx_Name);
                                pos.Set_ValueNoCheck("M_Product_ID", Util.GetValueOfInt(_ProductList[i]));
                                pos.Set_ValueNoCheck("VA010_QualityParameters_ID", Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA010_QualityParameters_ID"]));
                                pos.Set_ValueNoCheck("M_InOutLineConfirm_ID", Util.GetValueOfInt(M_InOutConfirmLine_ID[i]));
                                pos.Set_ValueNoCheck("VA010_TestPrmtrList_ID", Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA010_TestPrmtrList_ID"]));
                                pos.Set_ValueNoCheck("VA010_QuantityToVerify", Util.GetValueOfDecimal(_qty));
                                pos.Set_ValueNoCheck("AD_Client_ID", ctx.GetAD_Client_ID());
                                pos.Set_ValueNoCheck("AD_Org_ID", ctx.GetAD_Org_ID());

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
                            DB.ExecuteQuery(" UPDATE M_InOutLineConfirm SET VA010_QualCheckMark ='Y'  WHERE M_InOutLineConfirm_ID=" + M_InOutConfirmLine_ID[i], null, Trx_Name);
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
         *	@param M_InOutConfirm_ID id
         *	@param trxName transaction
         */
        public MInOutConfirm(Ctx ctx, int M_InOutConfirm_ID, Trx trxName) :
            base(ctx, M_InOutConfirm_ID, trxName)
        {

            if (M_InOutConfirm_ID == 0)
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
        public MInOutConfirm(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param ship shipment
         *	@param confirmType confirmation type
         */
        public MInOutConfirm(MInOut ship, String confirmType)
            : this(ship.GetCtx(), 0, ship.Get_TrxName())
        {

            SetClientOrg(ship);
            SetM_InOut_ID(ship.GetM_InOut_ID());
            SetConfirmType(confirmType);

            //Lakhwinder 1Feb2021
            //Shipment and Inventory Move Module Changes
            //Solution Proposed Puneed 
            //STandard changes Analysed in Gull Implementation
            MDocType dt = MDocType.Get(GetCtx(), ship.GetC_DocType_ID());
            if (Get_ColumnIndex("C_DocType_ID") > -1 && dt.Get_ColumnIndex("C_DocTypeConfrimation_ID") > -1)
            {
                SetC_DocType_ID(Util.GetValueOfInt(dt.Get_Value("C_DocTypeConfrimation_ID")));
            }

            // VIS0060: Set Trx Org from Shipment/Receipt to Confirmation
            if(ship.GetAD_OrgTrx_ID() > 0)
            {
                Set_Value("AD_OrgTrx_ID", ship.GetAD_OrgTrx_ID());
            }
        }


        /**
         * 	Get Lines
         *	@param requery requery
         *	@return array of lines
         */
        public MInOutLineConfirm[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            String sql = "SELECT * FROM M_InOutLineConfirm "
                + "WHERE M_InOutConfirm_ID=" + GetM_InOutConfirm_ID();
            List<MInOutLineConfirm> list = new List<MInOutLineConfirm>();
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
                    list.Add(new MInOutLineConfirm(GetCtx(), dr, Get_TrxName()));
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


            _lines = new MInOutLineConfirm[list.Count];
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
            if (GetM_InOut_ID() > 0)
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
            return MRefList.GetListName(GetCtx(), CONFIRMTYPE_AD_Reference_ID, GetConfirmType());
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInOutConfirm[");
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
            return Msg.GetElement(GetCtx(), "M_InOutConfirm_ID") + " " + GetDocumentNo();
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
            //	ReportEngine re = ReportEngine.get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
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
                int AD_User_ID = GetCtx().GetAD_User_ID();
                MUser user = MUser.Get(GetCtx(), AD_User_ID);
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
            MDocType dt = MDocType.Get(GetCtx(), getC_DocTypeTarget_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), getDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            ****/

            MInOutLineConfirm[] lines = GetLines(true);
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
            ////sql = "SELECT ISDISALLOWNEGATIVEINV FROM M_Warehouse WHERE M_Warehouse_ID = (SELECT M_WAREHOUSE_ID FROM M_INOUT WHERE M_INOUT_ID = " + Util.GetValueOfInt(GetM_InOut_ID()) + " )";
            ////string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

            ////if (disallow.ToUpper() == "Y")
            ////{
            ////    int[] ioLine = MInOutLineConfirm.GetAllIDs("M_InOutLineConfirm", "M_InOutConfirm_ID  = " + GetM_InOutConfirm_ID(), Get_TrxName());                
            ////    int m_locator_id = 0;
            ////    int m_product_id = 0;
            ////    StringBuilder products = new StringBuilder();
            ////    StringBuilder locators = new StringBuilder();
            ////    bool check = false;
            ////    for (int i = 0; i < ioLine.Length; i++)
            ////    {
            ////        MInOutLineConfirm iol = new MInOutLineConfirm(Env.GetCtx(), ioLine[i], Get_TrxName());
            ////        MInOutLine iol1 = new MInOutLine(Env.GetCtx(), iol.GetM_InOutLine_ID() , Get_TrxName());
            ////        m_locator_id = Util.GetValueOfInt(iol1.GetM_Locator_ID());
            ////        m_product_id = Util.GetValueOfInt(iol1.GetM_Product_ID());



            ////        sql = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + m_product_id;
            ////        int m_attribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////        if (m_attribute_ID == 0)
            ////        {
            ////            sql = "SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id;
            ////            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////            int qtyToMove = Util.GetValueOfInt(iol.GetConfirmedQty());
            ////            if (qty < qtyToMove)
            ////            {
            ////                check = true;
            ////                products.Append(m_product_id + ", ");
            ////                locators.Append(m_locator_id + ", ");
            ////                continue;
            ////            }
            ////        }
            ////        else
            ////        {
            ////            sql = "SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id + " AND M_AttributeSetInstance_ID = " + iol1.GetM_AttributeSetInstance_ID();
            ////            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            ////            int qtyToMove = Util.GetValueOfInt(iol.GetConfirmedQty());
            ////            if (qty < qtyToMove)
            ////            {
            ////                check = true;
            ////                products.Append(m_product_id + ",");
            ////                locators.Append(m_locator_id + ",");
            ////                continue;
            ////            }
            ////        }
            ////    }
            ////    if (check)
            ////    {
            ////        sql = "SELECT SUBSTR (SYS_CONNECT_BY_PATH (value , ', '), 2) CSV FROM (SELECT value , ROW_NUMBER () OVER (ORDER BY value ) rn, COUNT (*) over () CNT FROM "
            ////             + " (SELECT DISTINCT value FROM m_locator WHERE M_Locator_ID IN(" + locators.ToString().Trim().Trim(',') + "))) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1";
            ////        string loc = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

            ////        sql = "SELECT SUBSTR (SYS_CONNECT_BY_PATH (Name , ', '), 2) CSV FROM (SELECT Name , ROW_NUMBER () OVER (ORDER BY Name ) rn, COUNT (*) over () CNT FROM "
            ////            + " M_Product WHERE M_Product_ID IN (" + products.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1";
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
            MInOut inout = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
            MInOutLineConfirm[] lines = GetLines(false);

            /* created by sunil 19/9/2016*/
            if (Env.IsModuleInstalled("DTD001_"))
            {
                MPackage package = new MPackage(GetCtx(), inout.GetM_Package_ID(), Get_Trx());
                if (inout.GetM_Package_ID() > 0 && !package.IsDTD001_IsPackgConfirm())
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "PleaseConfirmPackage");
                    return DocActionVariables.STATUS_INVALID;

                }
            }
            //End

            //	Check if we need to split Shipment
            if (IsInDispute())
            {
                MDocType dt = MDocType.Get(GetCtx(), inout.GetC_DocType_ID());
                if (dt.IsSplitWhenDifference())
                {
                    if (dt.GetC_DocTypeDifference_ID() == 0)
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
                        SplitInOut(inout, dt.GetC_DocTypeDifference_ID(), lines);
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

                    string mInOutLinesConfirm = sb.ToString().Trim(',');

                    if (!string.IsNullOrEmpty(mInOutLinesConfirm))
                    {
                        string qry = DBFunctionCollection.ShipConfirmNoActualValue(mInOutLinesConfirm);
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
            //MInOut io = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
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
            MInOut io = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
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
                MInOutLineConfirm confirmLine = lines[i];

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

            //Lakhwinder 17Feb 2021
            //Stop Inventory to get completed
            //Arpit to complete the internal inventory if found any
            //if (internalInventory && Util.GetValueOfInt(GetM_Inventory_ID()) > 0)
            //{
            //    MInventory intInv = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_TrxName());
            //    intInv.CompleteIt();
            //    intInv.SetDocStatus(DOCSTATUS_Completed);
            //    intInv.SetDocAction(DOCACTION_Close);
            //    if (!intInv.Save(Get_TrxName()))
            //    {
            //        GetCtx().SetContext("DifferenceQty_", "0");
            //        Get_TrxName().Rollback();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //}
            //End Here 
            if (_creditMemo != null)
                _processMsg += " @C_Invoice_ID@=" + _creditMemo.GetDocumentNo();
            if (_inventory != null)
                //   _processMsg += " @M_Inventory_ID@=" + _inventory.GetDocumentNo();
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
         *	@param C_DocType_ID target DocType
         *	@param confirmLines confirm lines
         */
        private void SplitInOut(MInOut original, int C_DocType_ID, MInOutLineConfirm[] confirmLines)
        {
            MInOut split = new MInOut(original, C_DocType_ID, original.GetMovementDate());
            split.AddDescription("Splitted from " + original.GetDocumentNo());
            split.SetIsInDispute(true);
            // new 13 jan
            int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT count(*) FROM AD_Column clm INNER JOIN ad_table tbl on (tbl.ad_table_id=clm.ad_table_id) where tbl.tablename='M_InOutLineConfirm' and clm.columnname = 'M_Locator_ID' "));
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
                MInOutLineConfirm confirmLine = confirmLines[i];
                Decimal differenceQty = confirmLine.GetDifferenceQty();
                if (differenceQty.CompareTo(Env.ZERO) == 0)
                    continue;
                //
                MInOutLine oldLine = confirmLine.GetLine();
                log.Fine("Qty=" + differenceQty + ", Old=" + oldLine);
                //
                MInOutLine splitLine = new MInOutLine(split);
                splitLine.SetC_OrderLine_ID(oldLine.GetC_OrderLine_ID());
                splitLine.SetC_UOM_ID(oldLine.GetC_UOM_ID());
                splitLine.SetDescription(oldLine.GetDescription());
                splitLine.SetIsDescription(oldLine.IsDescription());
                splitLine.SetLine(oldLine.GetLine());
                splitLine.SetM_AttributeSetInstance_ID(oldLine.GetM_AttributeSetInstance_ID());
                //new 13 jan vikas ,assigne by surya sir
                if (_count > 0)
                {
                    if (confirmLine.GetM_Locator_ID() > 0)
                    {
                        splitLine.SetM_Locator_ID(confirmLine.GetM_Locator_ID());
                    }
                    else
                    {
                        splitLine.SetM_Locator_ID(oldLine.GetM_Locator_ID());
                    }
                }
                else
                {
                    splitLine.SetM_Locator_ID(oldLine.GetM_Locator_ID());
                }
                //End
                //  splitLine.SetM_Locator_ID(oldLine.GetM_Locator_ID());
                splitLine.SetM_Product_ID(oldLine.GetM_Product_ID());
                splitLine.SetM_Warehouse_ID(oldLine.GetM_Warehouse_ID());
                splitLine.SetRef_InOutLine_ID(oldLine.GetRef_InOutLine_ID());
                splitLine.AddDescription("Split: from " + oldLine.GetMovementQty());
                //	Qtys
                splitLine.SetQty(differenceQty);        //	Entered/Movement

                //190 - Get Print description and set
                if (splitLine.Get_ColumnIndex("PrintDescription") >= 0)
                    splitLine.Set_Value("PrintDescription", oldLine.Get_Value("PrintDescription"));

                /* Update QtyEntered/Qtymovement on Shipment Line to whom we are splitting*/
                /** Otherwise system can not save splited line because system founf mote qty to be shipped from Ordered Qty **/
                oldLine.AddDescription("Splitted: from " + oldLine.GetMovementQty());
                MProduct Product_ = new MProduct(GetCtx(), splitLine.GetM_Product_ID(), Get_TrxName());
                if (Product_.GetC_UOM_ID() != splitLine.GetC_UOM_ID())
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

            _processMsg = "Split @M_InOut_ID@=" + split.GetDocumentNo()
                + " - @M_InOutConfirm_ID@=";

            //	Create Dispute Confirmation
            split.ProcessIt(DocActionVariables.ACTION_PREPARE);
            //	split.createConfirmation();
            split.Save(Get_TrxName());
            MInOutConfirm[] splitConfirms = split.GetConfirmations(true);
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
                MInOutLineConfirm[] splitConfirmLines = splitConfirms[index].GetLines(false);
                for (int i = 0; i < splitConfirmLines.Length; i++)
                {
                    MInOutLineConfirm splitConfirmLine = splitConfirmLines[i];
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
        private bool CreateDifferenceDoc(MInOut inout, MInOutLineConfirm confirm)
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
                    _creditMemo = new MInvoice(inout, null);
                    _creditMemo.SetDescription(Msg.Translate(GetCtx(),
                        "M_InOutConfirm_ID") + " " + GetDocumentNo());
                    _creditMemo.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                    if (!_creditMemo.Save(Get_TrxName()))
                    {
                        _processMsg += "Credit Memo not created";
                        return false;
                    }
                    SetC_Invoice_ID(_creditMemo.GetC_Invoice_ID());
                }
                MInvoiceLine line = new MInvoiceLine(_creditMemo);
                line.SetShipLine(confirm.GetLine());
                //line.SetQty(confirm.GetDifferenceQty());	//	Entered/Invoiced
                //Arpit for invoice --qty should be converted while saving on invoice line
                MInOutLine iol = new MInOutLine(GetCtx(), confirm.GetM_InOutLine_ID(), Get_TrxName());
                MProduct _Pro = new MProduct(GetCtx(), iol.GetM_Product_ID(), Get_TrxName());
                if (confirm.GetC_UOM_ID() != _Pro.GetC_UOM_ID())
                {
                    decimal? pc = MUOMConversion.ConvertProductFrom(GetCtx(), iol.GetM_Product_ID(), confirm.GetC_UOM_ID(), confirm.GetDifferenceQty());
                    line.SetQty(Util.GetValueOfDecimal(pc));	//	Entered/Invoiced
                }
                else
                    line.SetQty(confirm.GetDifferenceQty());	//	Entered/Invoiced
                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Credit Memo Line not created";
                    return false;
                }
                confirm.SetC_InvoiceLine_ID(line.GetC_InvoiceLine_ID());
            }

            //	Create Inventory Difference
            if (Env.Signum(confirm.GetScrappedQty()) != 0)
            {
                log.Info("Scrapped=" + confirm.GetScrappedQty());
                if (_inventory == null)
                {
                    //MWarehouse wh = MWarehouse.Get(GetCtx(), inout.GetM_Warehouse_ID());

                    MWarehouse wh = new MWarehouse(GetCtx(), inout.GetM_Warehouse_ID(), Get_TrxName());

                    _inventory = new MInventory(wh);
                    _inventory.SetDescription(Msg.Translate(GetCtx(),
                        "M_InOutConfirm_ID") + " " + GetDocumentNo());

                    //Lakhwinder
                    // Set InternalUser true if Internal User Inventory
                    int doctypeSetting = CheckAssociateDocTypeSetting(inout.GetC_DocType_ID(), false);
                    if (doctypeSetting == 1)
                    { _inventory.SetIsInternalUse(true); }
                    else { _inventory.SetIsInternalUse(false); }
                    //vikas  new 13 jan 2016 1
                    // _inventory.SetIsInternalUse(true);
                    if (_inventory.GetC_DocType_ID() == 0)
                    {
                        MDocType[] types = MDocType.GetOfDocBaseType(GetCtx(), MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY);
                        if (types.Length > 0)
                        {
                            // Get Internal Use Inv Doc Type
                            for (int i = 0; i < types.Length; i++)
                            {
                                int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM C_DocType WHERE IsActive='Y' AND  IsInternalUse='Y' AND C_DocType_ID=" + types[i].GetC_DocType_ID()));
                                if (_count > 0)
                                {
                                    _inventory.SetC_DocType_ID(types[i].GetC_DocType_ID());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @C_DocType_ID@"));
                            return false;
                        }
                    }
                    // 13 jan End 
                    if (!_inventory.Save(Get_TrxName()))
                    {
                        _processMsg += "Inventory not created";
                        return false;
                    }
                    SetM_Inventory_ID(_inventory.GetM_Inventory_ID());
                }
                MInOutLine ioLine = confirm.GetLine();
                // Removed the code as already handled on before save..
                MInventoryLine line = new MInventoryLine(_inventory, ioLine.GetM_Locator_ID(), ioLine.GetM_Product_ID(), ioLine.GetM_AttributeSetInstance_ID(),
                   confirm.GetScrappedQty(), Env.ZERO);

                //Below code commented because we have to convert the Quantity for update in wareHouse as mentioned above
                //MInventoryLine line = new MInventoryLine(_inventory,
                //    ioLine.GetM_Locator_ID(), ioLine.GetM_Product_ID(),
                //    ioLine.GetM_AttributeSetInstance_ID(),
                //    confirm.GetScrappedQty(), Env.ZERO);
                //new 15 jan

                // Added by Bharat on 17 Jan 2019 to set new fields UOM and Qty Entered..
                line.Set_Value("C_UOM_ID", ioLine.GetC_UOM_ID());
                line.Set_Value("QtyEntered", line.GetQtyBook());
                line.SetQtyInternalUse(line.GetQtyBook());
                line.SetQtyBook(0);


                //Lakhwinder
                //Behave as per setting on Doctype
                if (_inventory.IsInternalUse())
                {
                    line.SetIsInternalUse(true);
                }
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("DTD001_", out mInfo))
                {
                    int _charge = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Charge_ID FROM C_Charge WHERE isactive='Y' AND  DTD001_ChargeType='INV'"));
                    line.SetC_Charge_ID(_charge);
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
                confirm.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
            }

            //
            if (!confirm.Save(Get_TrxName()))
            {
                _processMsg += "Confirmation Line not saved";
                return false;
            }
            return true;
        }


        /// <summary>
        /// Check Settings on Document Type Whether we need to create Physical Inventory, Internal User Inventory or Physical Inventory
        /// </summary>
        /// <param name="DocTypeID"> Document Type</param>
        /// <param name="isDiff"> Is Differnece Qty</param>
        /// <returns>  
        /// return 0,2 for PhysicalInventory
        /// return 1 for Internal Use inventory
        /// return 3 for MM Reciept
        /// </returns>
        private int CheckAssociateDocTypeSetting(int DocTypeID, bool isDiff)
        {


            MDocType docType = MDocType.Get(GetCtx(), DocTypeID);
            if (docType.Get_ColumnIndex("C_DocTypeScrap_ID") < 0 && docType.Get_ColumnIndex("C_DocTypeDifference_ID") < 0)
            { return 0; }

            int docTypeAssociate = 0;
            if (!isDiff)
            {
                docTypeAssociate = Util.GetValueOfInt(docType.Get_Value("C_DocTypeScrap_ID"));
                if (docTypeAssociate > 0)
                {
                    docType = MDocType.Get(GetCtx(), docTypeAssociate);
                    if (Util.GetValueOfBool(docType.Get_Value("IsInternalUse"))) //Internal Use Inventory
                    {
                        return 1;
                    }
                    else { return 2; }//Physical Inventory
                }
            }
            else
            {
                if (docType.IsSplitWhenDifference())
                {
                    docTypeAssociate = Util.GetValueOfInt(docType.Get_Value("C_DocTypeDifference_ID"));
                    if (docTypeAssociate > 0) { return 3; }
                }
            }
            return 0;
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
            MInOutLineConfirm[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInOutLineConfirm line = lines[i];
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
            MInOut io = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
            if (io.GetDocStatus() != DOCACTION_Close)
            {
                DocumentEngine.ProcessIt(io, DOCACTION_Close);
                if (!io.Save())
                {
                    pp = VLogger.RetrieveError();
                    _processMsg = Msg.GetMsg(GetCtx(), "NotSavedInOut") + pp != null ? ": " + pp.GetName() : "";
                }
            }

            if (GetM_Inventory_ID() > 0)
            {
                MInventory _Inventory = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_TrxName());
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
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /**
         * 	Get Document Currency
         *	@return C_Currency_ID
         */
        public int GetC_Currency_ID()
        {
            //	MPriceList pl = MPriceList.get(getCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();
            return 0;
        }
        //Arpit to freeze quality control Lines
        public void FreezeQualityControlLines()
        {
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix='VA010_'", null, Get_TrxName())) > 0)
            {
                String sql = "Select VA010_ShipConfParameters_ID from VA010_ShipConfParameters Where M_InOutLineConfirm_ID IN ("
                    + " SELECT M_InOutLineConfirm_ID FROM M_InOutLineConfirm Where M_InOutConfirm_ID=" + Get_ID() + ")";
                MTable table = MTable.Get(GetCtx(), "VA010_ShipConfParameters");
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
