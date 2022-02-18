/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInventory
 * Purpose        : Physical Inventory Model
 * Class Used     : X_M_Inventory, DocAction
 * Chronological    Development
 * Raghunandan     22-Jun-2009
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
    public class MInventory : X_M_Inventory, DocAction
    {
        #region VAriables
        //	Process Message 			
        private String _processMsg = null;
        //	Just Prepared Flag			
        private bool _justPrepared = false;
        //	Cache						
        private static CCache<int, MInventory> _cache = new CCache<int, MInventory>("M_Inventory", 5, 5);
        //	Lines						
        private MInventoryLine[] _lines = null;

        private string sql = "";
        private Decimal? trxQty = 0;
        private bool isGetFromStorage = false;

        MAcctSchema acctSchema = null;
        MProduct product1 = null;
        decimal currentCostPrice = 0;
        string conversionNotFoundInOut = "";
        string conversionNotFoundInventory = "";
        string conversionNotFoundInventory1 = "";
        MCostElement costElement = null;
        ValueNamePair pp = null;

        /**is container applicable */
        private bool isContainerApplicable = false;

        /** Reversal Indicator			*/
        public const String REVERSE_INDICATOR = "^";
       
        #endregion


        /*	Get Inventory from Cache
	 *	@param ctx context
	 *	@param M_Inventory_ID id
	 *	@return MInventory
	 */
        public static MInventory Get(Ctx ctx, int M_Inventory_ID)
        {
            int key = M_Inventory_ID;
            MInventory retValue = (MInventory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MInventory(ctx, M_Inventory_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context 
         *	@param M_Inventory_ID id
         *	@param trxName transaction
         */
        public MInventory(Ctx ctx, int M_Inventory_ID, Trx trxName)
            : base(ctx, M_Inventory_ID, trxName)
        {

            if (M_Inventory_ID == 0)
            {
                //	setName (null);
                //  setM_Warehouse_ID (0);		//	FK
                SetMovementDate(DateTime.Now);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetMovementDate(DateTime.Now);	// @#Date@
                SetPosted(false);
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MInventory(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Warehouse Constructor
         *	@param wh warehouse
         */
        public MInventory(MWarehouse wh)
            : this(wh.GetCtx(), 0, wh.Get_TrxName())
        {
            SetClientOrg(wh);
            SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
        }



        /**
         * 	Get Lines
         *	@param requery requery
         *	@return array of lines
         */
        public MInventoryLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MInventoryLine> list = new List<MInventoryLine>();
            String sql = "SELECT * FROM M_InventoryLine WHERE M_Inventory_ID=" + GetM_Inventory_ID() + " ORDER BY Line";
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
                    list.Add(new MInventoryLine(GetCtx(), dr, Get_TrxName()));
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
            finally { dt = null; }
            _lines = new MInventoryLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /**
         * 	Overwrite Client/Org - from Import.
         * 	@param AD_Client_ID client
         * 	@param AD_Org_ID org
         */
        public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInventory[");
            sb.Append(Get_ID())
                .Append("-").Append(GetDocumentNo())
                .Append(",M_Warehouse_ID=").Append(GetM_Warehouse_ID())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
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
            //try
            //{
            //    string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
            //                        + ".txt"; //.pdf
            //    string filePath = Path.GetTempPath() + fileName;

            //    //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
            //    //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            //    FileInfo temp = new FileInfo(filePath);
            //    if (!temp.Exists)
            //    {
            //        return CreatePDF(temp);
            //    }
            //}
            //catch (Exception e)
            //{
            //    log.Severe("Could not create PDF - " + e.Message);
            //}
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
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetC_DocType_ID() == 0)
            {
                MDocType[] types = MDocType.GetOfDocBaseType(GetCtx(), MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY);
                if (types.Length > 0)	//	get first
                    SetC_DocType_ID(types[0].GetC_DocType_ID());
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @C_DocType_ID@"));
                    return false;
                }
            }

            //Set Is Adjusted
            if (!IsInternalUse())
            {
                if (Is_ValueChanged("MovementDate"))
                {
                    SetIsAdjusted(false);
                }
            }

            //	Warehouse Org
            if (newRecord
                || Is_ValueChanged("AD_Org_ID") || Is_ValueChanged("M_Warehouse_ID"))
            {
                MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                if (wh.GetAD_Org_ID() != GetAD_Org_ID())
                {
                    log.SaveError("WarehouseOrgConflict", "");
                    return false;
                }
                if (Is_ValueChanged("M_Warehouse_ID"))
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InventoryLine_ID) FROM M_InventoryLine WHERE M_Inventory_ID = " + GetM_Inventory_ID(), null, Get_Trx())) > 0)
                    {
                        log.SaveError("VIS_WarehouseCantChange", "");
                        return false;
                    }
                }
            }


            return true;
        }


        /**
         * 	Set Processed.
         * 	Propergate to Lines/Taxes
         *	@param processed processed
         */
        public void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE M_InventoryLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE M_Inventory_ID=" + GetM_Inventory_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine("Processed=" + processed + " - Lines=" + noLine);
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
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
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
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY, GetAD_Org_ID()))
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

            //MInventoryLine[] lines = GetLines(false);
            //Added by Bharat on 30/12/2016 for optimization
            String sql = "SELECT COUNT(M_InventoryLine_ID) FROM M_InventoryLine WHERE M_Inventory_ID=" + GetM_Inventory_ID();
            int lines = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            //if (lines.Length == 0)
            if (lines == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	TODO: Add up Amounts
            //	setApprovalAmt();


            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }


        public void InsertReceipeItem(Ctx ctx, MInventoryLine ol, int M_Product_ID, string NoModifier)
        {
            string sql = @"SELECT pb.M_Product_ID, pb.C_UOM_ID, pb.BOMQTY, pb.M_ProductBOM_ID, p.name, p.IsStocked, p.VA019_ItemType FROM M_Product_BOM pb 
                        INNER JOIN M_Product p ON (p.M_Product_ID = pb.M_ProductBOM_ID) WHERE pb.IsActive = 'Y' AND pb.M_Product_ID = " + M_Product_ID;
            DataSet dsIngre = null;
            try
            {
                dsIngre = DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < dsIngre.Tables[0].Rows.Count; i++)
                {
                    if (Util.GetValueOfString(dsIngre.Tables[0].Rows[i]["IsStocked"]) == "Y")
                    {
                        //if (!checkNoModifier(NoModifier, Convert.ToInt32(dsIngre.Tables[0].Rows[i]["M_ProductBOM_ID"])))
                        //   {
                        InsertUsedItem(ctx, ol, Util.GetValueOfInt(dsIngre.Tables[0].Rows[i]["M_ProductBOM_ID"]), "I", Util.GetValueOfDecimal(ol.GetQtyInternalUse()), Util.GetValueOfDecimal(dsIngre.Tables[0].Rows[i]["BOMQty"]), Util.GetValueOfInt(dsIngre.Tables[0].Rows[i]["C_UOM_ID"]));
                        //  }
                    }
                    else
                    {
                        if (Util.GetValueOfString(dsIngre.Tables[0].Rows[i]["VA019_ItemType"]) == "R")
                        {
                            InsertReceipeItem(ctx, ol, Util.GetValueOfInt(dsIngre.Tables[0].Rows[i]["M_ProductBOM_ID"]), NoModifier);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dsIngre = null;
            }
        }

        public bool InsertUsedItem(Ctx ctx, MInventoryLine ol, int M_Product_ID, String ItemType, Decimal? olQty, Decimal? Qty, int C_UOM_ID, Decimal? usedQty = 1)
        {
            object Tbl = DB.ExecuteScalar("select AD_Table_ID from ad_table where tablename='VA019_UsedItem_Quantity'");
            if (Tbl != null)
            {
                int Table_ID = Util.GetValueOfInt(Tbl);
                MTable table = MTable.Get(GetCtx(), Table_ID);
                PO po = null;
                po = table.GetPO(GetCtx(), 0, null);
                po.Set_Value("M_InventoryLine_ID", Util.GetValueOfInt(ol.GetM_InventoryLine_ID()));
                po.Set_Value("M_Product_ID", M_Product_ID);


                if (ItemType == "I" || ItemType == "PKG")
                {
                    po.Set_Value("Qty", olQty);
                    if (ItemType == "PKG")
                    {
                        po.Set_Value("VA019_ItemType", "P");
                    }
                    else
                    {
                        po.Set_Value("VA019_ItemType", ItemType);
                    }
                }
                else if (ItemType == "M")
                {
                    po.Set_Value("Qty", Qty * olQty);
                    po.Set_Value("VA019_ItemType", ItemType);
                }
                else
                {
                    po.Set_Value("Qty", olQty);
                    po.Set_Value("VA019_ItemType", ItemType);
                }
                po.Set_Value("QtyCalculated", Qty * olQty * usedQty);
                po.Set_Value("C_UOM_ID", C_UOM_ID);
                if (!po.Save())
                {
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        public void Add_UsedItemQuantity(MInventoryLine InventoryLine)
        {

            DataSet dsProds = DB.ExecuteDataset("SELECT IsStocked, VA019_IsRecipe, VA019_ItemType,c_UOM_ID FROM m_Product WHERE M_Product_ID = " + InventoryLine.GetM_Product_ID());

            if (dsProds.Tables.Count > 0)
            {
                DataRow drProd = dsProds.Tables[0].Rows[0];
                if (Util.GetValueOfString(drProd["IsStocked"]) == "Y")
                {
                    InsertUsedItem(GetCtx(), InventoryLine, InventoryLine.GetM_Product_ID(), "P", Util.GetValueOfDecimal(InventoryLine.GetQtyInternalUse()), 1, Util.GetValueOfInt(drProd["c_UOM_ID"]));
                }
                else
                {
                    if (Util.GetValueOfString(drProd["VA019_ItemType"]) == "R")
                    {
                        InsertReceipeItem(GetCtx(), InventoryLine, InventoryLine.GetM_Product_ID(), "");
                    }
                }
            }

        }


        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            #region[Prevent from completing if on hand qty not available as per requirement and disallow negative is true at warehouse.]
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ISDISALLOWNEGATIVEINV FROM M_Warehouse WHERE M_Warehouse_ID = " + GetM_Warehouse_ID());
            string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
            //int[] invLine = MInventoryLine.GetAllIDs("M_InventoryLine", "M_Inventory_ID  = " + GetM_Inventory_ID(), Get_TrxName());
            MInventoryLine[] lines = GetLines(false);
            log.Info("total Lines=" + lines.Count());
            if (disallow.ToUpper().Equals("Y"))
            {
                int m_locator_id = 0;
                int m_product_id = 0;
                StringBuilder products = new StringBuilder();
                StringBuilder locators = new StringBuilder();

                bool check = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    //MInventoryLine inLine = new MInventoryLine(Env.GetCtx(), invLine[i], Get_TrxName());
                    MInventoryLine inLine = lines[i];
                    m_locator_id = Util.GetValueOfInt(inLine.GetM_Locator_ID());
                    m_product_id = Util.GetValueOfInt(inLine.GetM_Product_ID());
                    Decimal qtyToMove = 0;
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
                            sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) 
                                        ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                        INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                        " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + inLine.GetM_Locator_ID() +
                                        " AND t.M_Product_ID = " + inLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + inLine.GetM_AttributeSetInstance_ID() +
                                        " AND NVL(t.M_ProductContainer_ID, 0) = " + inLine.GetM_ProductContainer_ID());
                        }
                        decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                        //int qtyToMove = Util.GetValueOfInt(inLine.GetQtyInternalUse());
                        if (inLine.IsInternalUse())
                        {
                            qtyToMove = Util.GetValueOfDecimal(inLine.GetQtyInternalUse());
                        }
                        else
                        {
                            qtyToMove = Util.GetValueOfDecimal(inLine.GetDifferenceQty());
                        }
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
                            sql.Append("SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id + " AND M_AttributeSetInstance_ID = " + inLine.GetM_AttributeSetInstance_ID());
                        }
                        else
                        {
                            sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) 
                                        ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                        INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                        " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + inLine.GetM_Locator_ID() +
                                        " AND t.M_Product_ID = " + inLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + inLine.GetM_AttributeSetInstance_ID() +
                                        " AND NVL(t.M_ProductContainer_ID, 0) = " + inLine.GetM_ProductContainer_ID());
                        }
                        decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                        //int qtyToMove = Util.GetValueOfInt(inLine.GetQtyInternalUse());
                        if (inLine.IsInternalUse())
                        {
                            qtyToMove = Util.GetValueOfDecimal(inLine.GetQtyInternalUse());
                        }
                        else
                        {
                            qtyToMove = Util.GetValueOfDecimal(inLine.GetDifferenceQty());
                        }
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

                    _processMsg = Msg.GetMsg(Env.GetCtx(), "InsufficientQuantityFor: ") + prod + Msg.GetMsg(Env.GetCtx(), "OnLocators: ") + loc;
                    return DocActionVariables.STATUS_DRAFTED;
                }
            }

            // SI_0750 If difference between the requisition qty and delivered qty is -ve or zero. system should not allow to complete inventory move and internal use.
            if (!IsReversal())
            {
                if (Env.IsModuleInstalled("DTD001_"))
                {
                    StringBuilder delReq = new StringBuilder();
                    bool delivered = false;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        //MInventoryLine inLine = new MInventoryLine(Env.GetCtx(), invLine[i], Get_TrxName());
                        MInventoryLine inLine = lines[i];
                        if (inLine.GetM_RequisitionLine_ID() != 0)
                        {
                            MRequisitionLine reqLine = new MRequisitionLine(GetCtx(), inLine.GetM_RequisitionLine_ID(), Get_TrxName());
                            if (reqLine.GetQty() - reqLine.GetDTD001_DeliveredQty() <= 0)
                            {
                                delivered = true;
                                delReq.Append(inLine.GetM_RequisitionLine_ID() + ",");
                            }
                        }
                    }

                    if (delivered)
                    {
                        sql.Clear();
                        sql.Append(DBFunctionCollection.ConcatnatedListOfRequisition(delReq.ToString()));
                        sql = sql.Replace("'", "''");
                        string req = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                        _processMsg = Msg.GetMsg(Env.GetCtx(), "RequisitionAlreadyDone") + ": " + req;
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }
            }
            #endregion

            if (isContainerApplicable)
            {
                #region Check Container existence  in specified Warehouse and Locator
                // during completion - system will verify 
                // if container avialble on line is belongs to same warehouse and locator
                // if not then not to complete this record
                sql.Clear();
                sql.Append(DBFunctionCollection.MInventoryContainerNotMatched(GetM_Inventory_ID()));
                string containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFound") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion

                #region Movement Date is less than Last MovementDate on Product Container, then not to complete
                // If User try to complete the Transactions if Movement Date is lesser than Last MovementDate on Product Container
                // then we need to stop that transaction to Complete.
                StringBuilder _qry = new StringBuilder();
                _qry.Append(DBFunctionCollection.MInventoryContainerNotAvailable(GetM_Inventory_ID()));
                string misMatch = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (!String.IsNullOrEmpty(misMatch))
                {
                    SetProcessMsg(misMatch + Msg.GetMsg(GetCtx(), "VIS_ContainerNotAvailable"));
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion
            }

            //
            string val = "";
            Tuple<String, String, String> moduleInfo = null;
            if (Env.HasModulePrefix("VA019_", out moduleInfo))
            {
                string qry = "select va019_iswastage from c_Doctype where isinternaluse='Y' and c_doctype_id=" + GetC_DocType_ID();
                val = Util.GetValueOfString(DB.ExecuteScalar(qry));
            }
            else
            {
                val = "N";
            }

            //Apply module check..................

            if (!IsInternalUse())
            {
                #region Update QtyCount
                //Added by Bharat on 30/12/2016 for optimization
                log.Info("update Inventory Started");
                sql.Clear();
                if (!isContainerApplicable)
                {
                    sql.Append(@"SELECT m.M_InventoryLine_ID, m.M_Locator_ID, m.M_Product_ID, m.M_AttributeSetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM M_InventoryLine m LEFT JOIN (SELECT DISTINCT t.M_Locator_ID, t.M_Product_ID, t.M_AttributeSetInstance_ID, 
                FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM M_Transaction t
                INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND l.AD_Org_ID = " + GetAD_Org_ID() +
               @") mt ON m.M_Product_ID = mt.M_Product_ID AND nvl(m.M_AttributeSetInstance_ID, 0) = nvl(mt.M_AttributeSetInstance_ID, 0) 
                AND m.M_Locator_ID = mt.M_Locator_ID WHERE m.M_Inventory_ID = " + Get_ID() + " ORDER BY m.Line");
                }
                else
                {
                    sql.Append(@"SELECT m.M_InventoryLine_ID, m.M_Locator_ID, m.M_Product_ID, m.M_AttributeSetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM M_InventoryLine m LEFT JOIN (SELECT DISTINCT t.M_Locator_ID, t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_ProductContainer_ID, 
                FIRST_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID , NVL(t.m_productcontainer_id, 0) ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM M_Transaction t
                INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                " AND t.AD_Client_ID = " + GetAD_Client_ID() + @") mt ON m.M_Product_ID = mt.M_Product_ID AND nvl(m.M_AttributeSetInstance_ID, 0) = nvl(mt.M_AttributeSetInstance_ID, 0) 
                AND m.M_Locator_ID = mt.M_Locator_ID AND nvl(m.M_ProductContainer_ID, 0) = nvl(mt.M_ProductContainer_ID, 0) WHERE m.M_Inventory_ID = " + Get_ID() + " ORDER BY m.Line");
                }
                DataSet ds = null;
                StringBuilder updateSql = new StringBuilder();
                try
                {
                    ds = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        //updateSql.Append("BEGIN ");
                        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        //{
                        //    int line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]);
                        //    int locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][1]);
                        //    int product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][2]);
                        //    string AdjustType = Util.GetValueOfString(ds.Tables[0].Rows[i][4]);
                        //    decimal AsonDateCount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i][5]);
                        //    decimal DiffQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i][6]);
                        //    decimal currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i][7]);
                        //    string updateQry = UpdateInventoryLine(line_ID, product_ID, locator_ID, currentQty, AdjustType, AsonDateCount, DiffQty);
                        //    if (updateQry != "")
                        //    {
                        //        updateSql.Append(updateQry);
                        //    }
                        //}
                        //ds.Dispose();
                        //updateSql.Append(" END;");
                        //int cnt = DB.ExecuteQuery(updateSql.ToString(), null, null);

                        string updateQry = DBFunctionCollection.UpdateInventoryLine(GetCtx(), ds, Get_Trx());
                        ds.Dispose();
                        int cnt = DB.ExecuteQuery(updateQry, null, Get_Trx());

                        if (cnt > 0)
                        {
                            //MInventory inventory = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_TrxName());
                            //inventory.SetIsAdjusted(true);
                            //inventory.Save();
                            int no = DB.ExecuteQuery("UPDATE M_Inventory SET IsAdjusted = 'Y' WHERE M_Inventory_ID = " + GetM_Inventory_ID(), null, Get_Trx());
                        }
                    }
                    log.Info("update Inventory End");
                }
                catch (Exception e)
                {
                    if (ds != null)
                    {
                        ds.Dispose();
                    }
                    log.Log(Level.SEVERE, sql.ToString(), e);
                }
                #endregion
            }

            #region Quantity Count can not be -ve -- In Case of DisAllow negative is True
            sql.Clear();
            sql.Append(@"SELECT COUNT(i.M_Inventory_ID) FROM M_Inventory i
                    INNER JOIN m_inventoryline il ON i.M_Inventory_ID = il.M_Inventory_ID 
                    INNER JOIN m_storage s ON (il.m_product_id  = s.m_product_id AND il.m_locator_id  = s.m_locator_id
                    AND NVL(il.M_AttributeSetInstance_ID , 0) = NVL(s.M_AttributeSetInstance_ID , 0))
                    INNER JOIN m_warehouse w ON w.m_warehouse_id  = i.M_Warehouse_ID
                    WHERE w.isdisallownegativeinv = 'Y' AND il.isActive = 'Y' AND s.IsActive = 'Y' AND (( 1=
                    CASE  WHEN i.isinternaluse = 'N' AND il.DifferenceQty > 0 AND (s.qtyonhand-il.DifferenceQty) < 0 THEN 1  ELSE 0  END )
                    OR (1 = CASE WHEN i.isinternaluse  = 'Y' AND (s.qtyonhand-il.qtyinternaluse) < 0 THEN 1 ELSE 0 END)) 
                    AND i.M_Inventory_ID = " + GetM_Inventory_ID());
            int countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
            if (countRecord > 0)
            {
                _processMsg = "Quantity Count can not be -ve";
                return DocActionVariables.STATUS_INVALID;
            }
            #endregion

            int countVA024 = Env.IsModuleInstalled("VA024_") ? 1 : 0;
            sql.Clear();
            sql.Append(@"SELECT AD_TABLE_ID FROM AD_TABLE WHERE tablename LIKE 'VA024_T_ObsoleteInventory' AND IsActive = 'Y'");
            int tableId = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

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
            _processMsg = MTransaction.CheckFutureDateRecord(GetMovementDate(), Get_TableName(), GetM_Inventory_ID(), Get_Trx());
            if (!string.IsNullOrEmpty(_processMsg))
            {
                return DocActionVariables.STATUS_INVALID;
            }

            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());

            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

            lines = GetLines(true);
            //log.Info("total Lines=" + lines.Count());
            //MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            //
            log.Info("Lines Loop Started");
            for (int i = 0; i < lines.Length; i++)
            {
                MInventoryLine line = lines[i];
                Decimal? containerCurrentQty = 0;

                if (!line.IsActive())
                    continue;

                if (val == "Y")
                {
                    //Add Inventry line in used item quantity if DocType isWaste............(Vivek)
                    Add_UsedItemQuantity(line);
                }

                //line.CreateMA(false);

                // Material Policy should be apply on Orignal document - not on reverse document
                if (!IsReversal())
                {
                    Decimal qtyDif = Env.ZERO;
                    if (IsInternalUse())
                        qtyDif = Decimal.Negate(line.GetQtyInternalUse());
                    else if (!IsInternalUse())
                        qtyDif = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    CheckMaterialPolicy(line, qtyDif);
                }

                MTransaction trx = null;
                if (line.GetM_AttributeSetInstance_ID() == 0 || line.GetM_AttributeSetInstance_ID() != 0)
                {
                    Decimal qtyDiff = Decimal.Negate(line.GetQtyInternalUse());

                    if (Env.Signum(qtyDiff) == 0)
                        qtyDiff = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    //
                    if (Env.Signum(qtyDiff) > 0)
                    {
                        MInventoryLineMA[] mas = MInventoryLineMA.Get(GetCtx(),
                            line.GetM_InventoryLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            MInventoryLineMA ma = mas[j];

                            // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            MRequisitionLine reqLine = null;
                            //MRequisition req = null;
                            string reqStatus = "";
                            decimal reverseRequisitionQty = 0;
                            if (line.GetM_RequisitionLine_ID() > 0 && IsInternalUse())
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                                //req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());
                                reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                                + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                                if (!IsReversal())
                                {
                                    if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= ma.GetMovementQty())
                                    {
                                        reverseRequisitionQty = ma.GetMovementQty();
                                    }
                                    else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < ma.GetMovementQty())
                                    {
                                        if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                            reverseRequisitionQty = 0;
                                        else
                                            reverseRequisitionQty = Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty());
                                    }
                                    DB.ExecuteQuery("UPDATE M_InventoryLine SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                             @" WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    // during reversal -- only actual Requisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                                    reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());

                                    // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                                    line.SetActualReqReserved(0);
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
                                #endregion
                            }
                            //	Storage
                            MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                                    line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            if (storage == null)
                                storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            //Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                            Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), (ma.GetMovementQty() < 0 ? Decimal.Negate(ma.GetMovementQty()) : ma.GetMovementQty()));
                            log.Fine("Diff=" + qtyDiff
                               + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                            storage.SetQtyOnHand(qtyNew);
                            // SI_0337 : not to update last date inventory count through internal use inventory 
                            // JID_1095: If last inventory count date is greater than the Physical inventory date system should not update the Last Inventory dtae on storage.
                            if (!IsInternalUse())
                            {
                                if (storage.GetDateLastInventory() != null && GetMovementDate() < storage.GetDateLastInventory())
                                {

                                }
                                else
                                {
                                    storage.SetDateLastInventory(GetMovementDate());
                                }
                            }
                            //SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            else if (Env.IsModuleInstalled("DTD001_") && line.GetM_RequisitionLine_ID() > 0)
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                //storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), line.GetQtyInternalUse()));
                                storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), ma.GetMovementQty()));

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
                                    MStorage ordStorage = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                                    //ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), line.GetQtyInternalUse()));
                                    ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                    ordStorage.SetDTD001_QtyReserved(Decimal.Subtract(ordStorage.GetDTD001_QtyReserved(), reverseRequisitionQty));

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
                                #endregion
                            }

                            if (!storage.Save(Get_TrxName()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Storage not updated(1)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            log.Fine(storage.ToString());

                            #region  Update Transaction / Future Date entry
                            sql.Clear();
                            sql.Append(@"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                    " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                                " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID());
                            trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                            if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // get container Current qty from transaction
                                containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate());
                            }

                            // Done to Update Current Qty at Transaction

                            //	Transaction
                            trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                                MTransaction.MOVEMENTTYPE_InventoryIn,
                                    line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                     //qtyDiff, GetMovementDate(), Get_TrxName());
                                     (ma.GetMovementQty() < 0 ? Decimal.Negate(ma.GetMovementQty()) : ma.GetMovementQty()), GetMovementDate(), Get_TrxName());
                            trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                            //trx.SetCurrentQty(trxQty + qtyDiff);
                            trx.SetCurrentQty(trxQty + (ma.GetMovementQty() < 0 ? Decimal.Negate(ma.GetMovementQty()) : ma.GetMovementQty()));
                            // set Material Policy Date
                            trx.SetMMPolicyDate(ma.GetMMPolicyDate());
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // Update Product Container on Transaction
                                trx.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                                // update containr Current Qty 
                                trx.SetContainerCurrentQty(containerCurrentQty + qtyDiff);
                                trx.SetContainerCurrentQty(containerCurrentQty + (ma.GetMovementQty() < 0 ? Decimal.Negate(ma.GetMovementQty()) : ma.GetMovementQty()));
                            }
                            if (!trx.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Transaction not inserted(1)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            Decimal currentQty = (ma.GetMovementQty() < 0 ? Decimal.Negate(ma.GetMovementQty()) : ma.GetMovementQty()) + trxQty.Value;
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                String errorMessage = UpdateTransactionContainer(line, trx, currentQty);
                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    SetProcessMsg(errorMessage);
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                UpdateTransaction(line, trx, currentQty);
                            }
                            //UpdateCurrentRecord(line, trx, qtyDiff);
                            #endregion
                        }
                    }
                    else	//	negative qty
                    {
                        MInventoryLineMA[] mas = MInventoryLineMA.Get(GetCtx(),
                            line.GetM_InventoryLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            MInventoryLineMA ma = mas[j];

                            // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            //MRequisition req = null;
                            MRequisitionLine reqLine = null;
                            decimal reverseRequisitionQty = 0;
                            string reqStatus = "";
                            if (Env.IsModuleInstalled("DTD001_") && IsInternalUse() && line.GetM_RequisitionLine_ID() > 0)
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                                //req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());        // Trx used to handle query stuck problem
                                reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                                + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                                if (!IsReversal())
                                {
                                    if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= ma.GetMovementQty())
                                    {
                                        reverseRequisitionQty = ma.GetMovementQty();
                                    }
                                    else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < ma.GetMovementQty())
                                    {
                                        // when deleiverd is greater than requistion qty then make it as ZERO - no impacts goes to Requisition Ordered / Reserved qty
                                        if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                            reverseRequisitionQty = 0;
                                        else
                                            reverseRequisitionQty = Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty());
                                    }
                                    DB.ExecuteQuery("UPDATE M_InventoryLine SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                                 @" WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    // during reversal -- only actual Requisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                                    reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());

                                    // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                                    line.SetActualReqReserved(0);
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
                                #endregion
                            }

                            //	Storage
                            MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            if (storage == null)
                                storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                    line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            //
                            Decimal maxDiff = qtyDiff;
                            if (Env.Signum(maxDiff) < 0
                                && ma.GetMovementQty().CompareTo(Decimal.Negate(maxDiff)) < 0)
                                //maxDiff = Decimal.Negate(ma.GetMovementQty());
                                maxDiff = ma.GetMovementQty() < 0 ? ma.GetMovementQty() : Decimal.Negate(ma.GetMovementQty());
                            //Decimal qtyNew = Decimal.Add(ma.GetMovementQty(), maxDiff);	//	Storage+Diff
                            Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), maxDiff);	//	Storage+Diff

                            log.Fine("MA Qty=" + ma.GetMovementQty()
                                + ",Diff=" + qtyDiff + "|" + maxDiff
                                + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew
                                + " {" + ma.GetM_AttributeSetInstance_ID() + "}");
                            //
                            storage.SetQtyOnHand(qtyNew);
                            // SI_0337 : not to update last date inventory count through internal use inventory 
                            // JID_1095: If last inventory count date is greater than the Physical inventory date system should not update the Last Inventory dtae on storage.
                            if (!IsInternalUse())
                            {
                                if (storage.GetDateLastInventory() != null && GetMovementDate() < storage.GetDateLastInventory())
                                {

                                }
                                else
                                {
                                    storage.SetDateLastInventory(GetMovementDate());
                                }
                            }

                            //SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            else if (Env.IsModuleInstalled("DTD001_") && line.GetM_RequisitionLine_ID() > 0)
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), ma.GetMovementQty()));

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
                                    MStorage ordStorage = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                                    //ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), ma.GetMovementQty()));
                                    //ordStorage.SetDTD001_QtyReserved(Decimal.Subtract(ordStorage.GetDTD001_QtyReserved(), ma.GetMovementQty()));
                                    ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                    ordStorage.SetDTD001_QtyReserved(Decimal.Subtract(ordStorage.GetDTD001_QtyReserved(), reverseRequisitionQty));
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
                                #endregion
                            }

                            if (!storage.Save(Get_TrxName()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Storage not updated (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            log.Fine(storage.ToString());

                            #region Update Transaction / Future Date entry
                            sql.Clear();
                            sql.Append(@"SELECT DISTINCT FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                            " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND l.AD_Org_ID = " + GetAD_Org_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                            " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID());
                            trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                            if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // get container Current qty from transaction
                                containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate());
                            }

                            //	Transaction
                            trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                                MTransaction.MOVEMENTTYPE_InventoryIn,
                                line.GetM_Locator_ID(), line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                                maxDiff, GetMovementDate(), Get_TrxName());
                            trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                            //trx.SetCurrentQty(trxQty + qtyDiff);
                            trx.SetCurrentQty(trxQty + maxDiff);
                            // set Material Policy Date
                            trx.SetMMPolicyDate(ma.GetMMPolicyDate());
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // Update Product Container on Transaction
                                trx.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                                // update containr Current Qty 
                                //trx.SetContainerCurrentQty(containerCurrentQty + qtyDiff);
                                trx.SetContainerCurrentQty(containerCurrentQty + maxDiff);
                            }
                            if (!trx.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Transaction not inserted (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            //Decimal currentQty = qtyDiff + trxQty.Value;
                            Decimal currentQty = maxDiff + trxQty.Value;
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                String errorMessage = UpdateTransactionContainer(line, trx, currentQty);
                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    SetProcessMsg(errorMessage);
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                UpdateTransaction(line, trx, currentQty);
                            }
                            //UpdateCurrentRecord(line, trx, qtyDiff);                           
                            #endregion

                            qtyDiff = Decimal.Subtract(qtyDiff, maxDiff);
                            if (Env.Signum(qtyDiff) == 0)
                                break;
                        }
                        // nnayak - if the quantity issued was greator than the quantity onhand, we need to create a transaction 
                        // for the remaining quantity
                        if (Env.Signum(qtyDiff) != 0 && false)
                        {
                            log.Info("Executing Block -- quantity issued was greator than the quantity onhand, we need to create a transaction for the remaining quantity");
                            #region not to be execute this block -- because we are creating M_InventoryLineMA lines with full qty
                            // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            MRequisitionLine reqLine = null;
                            //MRequisition req = null;
                            string reqStatus = "";
                            decimal reverseRequisitionQty = 0;
                            if (line.GetM_RequisitionLine_ID() > 0 && IsInternalUse())
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                                //req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());
                                reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                                + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                                if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= line.GetQtyInternalUse())
                                {
                                    reverseRequisitionQty = line.GetQtyInternalUse();
                                }
                                else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < line.GetQtyInternalUse())
                                {
                                    reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                                }
                                if (Env.IsModuleInstalled("DTD001_"))
                                {
                                    reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), line.GetQtyInternalUse()));
                                    reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), line.GetQtyInternalUse()));
                                    if (!reqLine.Save())
                                    {
                                        _processMsg = Msg.GetMsg(GetCtx(), "ReqLineNotSaved");
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                    #endregion
                                }
                            }

                            MStorage storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                    line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                            log.Fine("Count=" + line.GetQtyCount()
                                + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff
                                + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                            //
                            storage.SetQtyOnHand(qtyNew);
                            // SI_0337 : not to update last date inventory count through internal use inventory 
                            // JID_1095: If last inventory count date is greater than the Physical inventory date system should not update the Last Inventory dtae on storage.
                            if (!IsInternalUse())
                            {
                                if (storage.GetDateLastInventory() != null && GetMovementDate() < storage.GetDateLastInventory())
                                {

                                }
                                else
                                {
                                    storage.SetDateLastInventory(GetMovementDate());
                                }
                            }

                            //SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                            else if (Env.IsModuleInstalled("DTD001_") && line.GetM_RequisitionLine_ID() > 0)
                            {
                                #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                                storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), line.GetQtyInternalUse()));

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
                                    MStorage ordStorage = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                                    ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), line.GetQtyInternalUse()));
                                    ordStorage.SetDTD001_QtyReserved(Decimal.Subtract(ordStorage.GetDTD001_QtyReserved(), reverseRequisitionQty));

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
                                #endregion
                            }

                            if (!storage.Save(Get_TrxName()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Storage not updated (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            log.Fine(storage.ToString());

                            #region Update Transaction / Future Date entry
                            sql.Clear();
                            sql.Append(@"SELECT DISTINCT FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                            " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                            " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID());
                            trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                            if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // get container Current qty from transaction
                                containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate());
                            }

                            //	Transaction
                            trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                                MTransaction.MOVEMENTTYPE_InventoryIn,
                                line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                qtyDiff, GetMovementDate(), Get_TrxName());
                            trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                            trx.SetCurrentQty(trxQty + qtyDiff);
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // Update Product Container on Transaction
                                trx.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                                // update containr Current Qty 
                                trx.SetContainerCurrentQty(containerCurrentQty + qtyDiff);
                            }
                            if (!trx.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Transaction not inserted (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            Decimal currentQty = qtyDiff + trxQty.Value;
                            if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                String errorMessage = UpdateTransactionContainer(line, trx, currentQty);
                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    SetProcessMsg(errorMessage);
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                UpdateTransaction(line, trx, currentQty);
                            }
                            //UpdateCurrentRecord(line, trx, qtyDiff);
                            //
                            #endregion

                            #endregion
                        }
                    }	//	negative qty
                }

                //	Fallback
                if (trx == null)
                {
                    #region this will execute when we did physical inventory in current date and try to do back date entry -- bcz we are not creating MA Line in that case
                    // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                    MRequisitionLine reqLine = null;
                    //MRequisition req = null;
                    string reqStatus = "";
                    decimal reverseRequisitionQty = 0;
                    if (line.GetM_RequisitionLine_ID() > 0)
                    {
                        #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                        reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                        //req = new MRequisition(GetCtx(), reqLine.GetM_Requisition_ID(), Get_Trx());         // Trx used to handle query stuck problem
                        reqStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM M_Requisition WHERE M_Requisition_ID="
                                               + reqLine.GetM_Requisition_ID(), null, Get_Trx()));
                        if (!IsReversal())
                        {
                            if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= line.GetQtyInternalUse())
                            {
                                reverseRequisitionQty = line.GetQtyInternalUse();
                            }
                            else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < line.GetQtyInternalUse())
                            {
                                // when delivered > request qty then no impact on Requisition Ordered / Reserved qty
                                if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                    reverseRequisitionQty = 0;
                                else
                                    reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                            }
                            DB.ExecuteQuery("UPDATE M_InventoryLine SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                         @" WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
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
                            reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), line.GetQtyInternalUse()));
                            reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), line.GetQtyInternalUse()));
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
                        }
                        #endregion
                    }

                    //	Storage
                    MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    if (storage == null)
                        storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    //
                    Decimal qtyDiff = Decimal.Negate(line.GetQtyInternalUse());
                    if (Env.ZERO.CompareTo(qtyDiff) == 0)
                        qtyDiff = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                    log.Fine("Count=" + line.GetQtyCount()
                       + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff
                        + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                    //
                    storage.SetQtyOnHand(qtyNew);
                    // SI_0337 : not to update last date inventory count through internal use inventory 
                    // JID_1095: If last inventory count date is greater than the Physical inventory date system should not update the Last Inventory dtae on storage.
                    if (!IsInternalUse())
                    {
                        if (storage.GetDateLastInventory() != null && GetMovementDate() < storage.GetDateLastInventory())
                        {

                        }
                        else
                        {
                            storage.SetDateLastInventory(GetMovementDate());
                        }
                    }

                    //SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                    else if (Env.IsModuleInstalled("DTD001_") && line.GetM_RequisitionLine_ID() > 0)
                    {
                        #region Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                        storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), line.GetQtyInternalUse()));

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
                            MStorage ordStorage = MStorage.Get(GetCtx(), OrdLocator_ID, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            //ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), line.GetQtyInternalUse()));
                            ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                            ordStorage.SetDTD001_QtyReserved(Decimal.Subtract(ordStorage.GetDTD001_QtyReserved(), reverseRequisitionQty));

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
                        #endregion
                    }

                    if (!storage.Save(Get_TrxName()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Storage not updated(2)";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    log.Fine(storage.ToString());

                    #region  Update Transaction / Future Date entry
                    sql.Clear();
                    sql.Append(@"SELECT DISTINCT FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                            " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND l.AD_Org_ID = " + GetAD_Org_ID() + " AND t.M_Locator_ID = " + line.GetM_Locator_ID() +
                            " AND t.M_Product_ID = " + line.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + line.GetM_AttributeSetInstance_ID());
                    trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                    if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        // get container Current qty from transaction
                        containerCurrentQty = GetContainerQtyFromTransaction(line, GetMovementDate());
                    }

                    //	Transaction
                    trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                        MTransaction.MOVEMENTTYPE_InventoryIn,
                        line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                        qtyDiff, GetMovementDate(), Get_TrxName());
                    trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                    trx.SetCurrentQty(trxQty + qtyDiff);
                    if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        // Update Product Container on Transaction
                        trx.SetM_ProductContainer_ID(line.GetM_ProductContainer_ID());
                        // update containr Current Qty 
                        trx.SetContainerCurrentQty(containerCurrentQty + qtyDiff);
                    }
                    if (!trx.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Transaction not inserted(2)";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    Decimal currentQty = qtyDiff + trxQty.Value;
                    if (isContainerApplicable && trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        String errorMessage = UpdateTransactionContainer(line, trx, currentQty);
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            SetProcessMsg(errorMessage);
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        UpdateTransaction(line, trx, currentQty);
                    }
                    //UpdateCurrentRecord(line, trx, qtyDiff);
                    #endregion

                    #endregion
                }	//	Fallback

                //By Amit for Cost Queue Management
                if (client.IsCostImmediate())
                {
                    #region Costing Calculation

                    // check costing method is LIFO or FIFO
                    String costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), line.GetM_Product_ID(), Get_Trx());

                    product1 = new MProduct(GetCtx(), line.GetM_Product_ID(), Get_Trx());
                    if (product1.GetProductType() == "I") // for Item Type product
                    {
                        #region get price from m_cost (Current Cost Price)
                        if (GetDescription() != null && GetDescription().Contains("{->"))
                        {
                            // do not update current cost price during reversal, this time reverse doc contain same amount which are on original document
                        }
                        else
                        {
                            // get price from m_cost (Current Cost Price)
                            currentCostPrice = 0;
                            //if (IsInternalUse() || (!IsInternalUse() && Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook()) < 0))
                            //{
                            currentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                            //}
                            //else if (!IsInternalUse())
                            //{
                            //    // in Case of physical Inventory, when we increase stock - get price of costing method, which is defined into cost combination
                            //    currentCostPrice = MCost.GetproductCostAndQtyMaterial(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                            //      line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                            //}
                            DB.ExecuteQuery("UPDATE M_InventoryLine SET CurrentCostPrice = " + currentCostPrice + @"
                                                WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
                        }
                        #endregion

                        decimal quantity = 0;
                        if (!IsInternalUse()) // Physical Inventory
                        {
                            quantity = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product1, line.GetM_AttributeSetInstance_ID(),
                           "Physical Inventory", line, null, null, null, null, 0, quantity, Get_TrxName(), out conversionNotFoundInOut, optionalstr: "window"))
                            {
                                if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                {
                                    conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                }
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                {
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                if (costingMethod != "" && quantity < 0)
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                       line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), 1, line.GetM_InventoryLine_ID(), costingMethod,
                                                       GetM_Warehouse_ID(), true, Get_Trx());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                                     @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
                                }

                                // Post Current Cost Price
                                currentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                    line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                                DB.ExecuteQuery("UPDATE M_InventoryLine SET PostCurrentCostPrice = " + currentCostPrice + @" , IsCostImmediate = 'Y' 
                                                WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());

                            }
                        }
                        else // Internal Use Inventory
                        {
                            quantity = Decimal.Negate(line.GetQtyInternalUse());
                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product1, line.GetM_AttributeSetInstance_ID(),
                           "Internal Use Inventory", line, null, null, null, null, 0, quantity, Get_TrxName(), out conversionNotFoundInOut, optionalstr: "window"))
                            {
                                if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                {
                                    conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                }
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                {
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                if (costingMethod != "" && quantity < 0)
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                           line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), 1, line.GetM_InventoryLine_ID(), costingMethod,
                                                           GetM_Warehouse_ID(), true, Get_Trx());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                                     @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());
                                }

                                // Post Current Cost Price
                                currentCostPrice = MCost.GetproductCosts(line.GetAD_Client_ID(), line.GetAD_Org_ID(),
                                                    line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                                DB.ExecuteQuery("UPDATE M_InventoryLine SET PostCurrentCostPrice = " + currentCostPrice + @" , IsCostImmediate = 'Y' 
                                                WHERE M_InventoryLine_ID = " + line.GetM_InventoryLine_ID(), null, Get_Trx());

                            }
                        }
                    }
                    #endregion
                }

                //by Amit -> For Obsolete Inventory (16-05-2016)
                if (countVA024 > 0)
                {
                    #region For Obsolete Inventory (16-05-2016)
                    // shipment Or Return To Vendor
                    if ((line.GetM_Product_ID() > 0 && IsInternalUse()) || (line.GetM_Product_ID() > 0 && !IsInternalUse() && line.GetDifferenceQty() > 0) ||
                        (GetDescription() != null && GetDescription().Contains("{->") && line.GetM_Product_ID() > 0 && !IsInternalUse() && line.GetDifferenceQty() < 0))
                    {
                        log.Info("Obsolute Inventory Work Started");
                        sql.Clear();
                        sql.Append(@"SELECT * FROM va024_t_obsoleteinventory
                                 WHERE ad_client_id    = " + GetAD_Client_ID() + " AND ad_org_id = " + GetAD_Org_ID() +
                                 " AND M_Product_ID = " + line.GetM_Product_ID() +
                                 " AND NVL(M_AttributeSetInstance_ID , 0) = " + line.GetM_AttributeSetInstance_ID());
                        //" AND M_Warehouse_Id = " + GetM_Warehouse_ID();
                        if (GetDescription() != null && !GetDescription().Contains("{->"))
                        {
                            sql.Append(" AND va024_isdelivered = 'N' ");
                        }
                        sql.Append(" ORDER BY va024_t_obsoleteinventory_id DESC ");
                        DataSet dsObsoleteInventory = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                        if (dsObsoleteInventory != null && dsObsoleteInventory.Tables.Count > 0 && dsObsoleteInventory.Tables[0].Rows.Count > 0)
                        {
                            Decimal remainigQty = 0;
                            if ((GetDescription() != null && GetDescription().Contains("{->") && line.GetM_Product_ID() > 0 && !IsInternalUse() && line.GetDifferenceQty() < 0) || (line.GetM_Product_ID() > 0 && !IsInternalUse() && line.GetDifferenceQty() > 0))
                            {
                                remainigQty = line.GetDifferenceQty();
                            }
                            else if (line.GetM_Product_ID() > 0 && IsInternalUse())
                            {
                                remainigQty = line.GetQtyInternalUse();
                            }
                            else
                            {
                                remainigQty = 0;
                            }

                            MTable tbl = new MTable(GetCtx(), tableId, null);
                            PO po = null;
                            for (int oi = 0; oi < dsObsoleteInventory.Tables[0].Rows.Count; oi++)
                            {
                                po = tbl.GetPO(GetCtx(), Util.GetValueOfInt(dsObsoleteInventory.Tables[0].Rows[oi]["va024_t_obsoleteinventory_ID"]), Get_Trx());
                                if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) < remainigQty)
                                {
                                    po.Set_Value("VA024_RemainingQty", 0);
                                    po.Set_Value("VA024_IsDelivered", true);
                                    po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty"))));
                                    remainigQty = Decimal.Subtract(remainigQty, Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")));
                                    if (!po.Save(Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                        _processMsg = "Not able to Reduce Obsolete Inventory";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) == remainigQty)
                                {
                                    po.Set_Value("VA024_RemainingQty", 0);
                                    po.Set_Value("VA024_IsDelivered", true);
                                    po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), remainigQty));
                                    if (!po.Save(Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                        _processMsg = "Not able to Reduce Obsolete Inventory";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) > remainigQty)
                                {
                                    po.Set_Value("VA024_RemainingQty", Decimal.Subtract(Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")), remainigQty));
                                    po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), remainigQty));
                                    po.Set_Value("VA024_IsDelivered", false);
                                    if (!po.Save(Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                        _processMsg = "Not able to Reduce Obsolete Inventory";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        dsObsoleteInventory.Dispose();
                        log.Info("Obsolute Inventory Work End");
                    }
                    #endregion
                }

            }	//	for all lines
            log.Info("Lines Loop Ended");
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            // Set the document number from completed document sequence after completed (if needed)
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

                //  Get current next from Completed document sequence defined on Document Type
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
        /// Create/Add to Inventory Line Query
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="currentQty">quantity</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <returns>lines added</returns>
        private string UpdateInventoryLine(int M_InventoryLine_ID, int M_Product_ID, int M_Locator_ID, Decimal currentQty, string AdjustType, Decimal AsOnDateCount, Decimal DiffQty)
        {
            string qry = "select m_warehouse_id from m_locator where m_locator_id=" + M_Locator_ID;
            int M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_Trx()));
            MWarehouse wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (currentQty < 0)
                {
                    return "";
                }
            }
            MProduct product = MProduct.Get(GetCtx(), M_Product_ID);
            if (product != null)
            {
                int precision = product.GetUOMPrecision();
                if (Env.Signum(currentQty) != 0)
                    currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
            }
            if (AdjustType == "A")
            {
                DiffQty = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, AsOnDateCount));
            }
            else if (AdjustType == "D")
            {
                AsOnDateCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));
            }
            Decimal QtyCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));
            // Changes by Bharat on 02 Aug 2017 as issue given by Ravikant
            string sql = @"UPDATE M_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsOnDateCount +
                ",DifferenceQty = " + DiffQty + " WHERE M_InventoryLine_ID = " + M_InventoryLine_ID;

            string updateQry = " SELECT ExecuteImmediate('" + sql.Replace("'", "''") + "') FROM DUAL;";
            return updateQry;
        }


        // amit not used 24-12-2015
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

        //this method is used to get opening balance from transaction based on parameter
        private decimal GetCurrentQty(int M_Product_ID, int M_Locator_ID, int M_AttributeSetInstance_ID, DateTime? movementDate)
        {
            decimal currentQty = 0;
            string query = "";
            int result = 0;
            query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            result = Util.GetValueOfInt(DB.ExecuteScalar(query));
            if (result > 0)
            {
                query = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
            }
            else
            {
                query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                if (result > 0)
                {
                    query = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                    currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
                }
            }
            return currentQty;
        }

        /// <summary>
        /// Update Transaction Tab to set Current Qty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="trx"></param>
        /// <param name="qtyDiff"></param>
        private String UpdateTransactionContainer(MInventoryLine line, MTransaction trxM, decimal qtyDiffer)
        {
            string errorMessage = null;
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            Decimal containerCurrentQty = trxM.GetContainerCurrentQty();
            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID()
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }
                else
                {
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }

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
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), Get_TrxName());
                                if (!inventory.IsInternalUse())
                                {
                                    #region update Physical Inventory
                                    if (inventoryLine.GetM_ProductContainer_ID() == line.GetM_ProductContainer_ID())
                                    {
                                        inventoryLine.SetParent(inventory);
                                        inventoryLine.SetQtyBook(containerCurrentQty);
                                        inventoryLine.SetOpeningStock(containerCurrentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                            Get_TrxName().Rollback();
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                            {
                                                _processMsg = pp.GetName();
                                                return Msg.GetMsg(GetCtx(), "VIS_InventoryLineNotSaved") + _processMsg;
                                            }
                                            else
                                            {
                                                return Msg.GetMsg(GetCtx(), "VIS_InventoryLineNotSaved");
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update Transaction
                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    if (trx.GetM_ProductContainer_ID() == line.GetM_ProductContainer_ID())
                                    {
                                        trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]))));
                                    }
                                    else
                                    {
                                        trx.SetCurrentQty(Decimal.Add(qtyDiffer, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["movementqty"])));
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
                                        qtyDiffer = trx.GetCurrentQty();
                                        if (line.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == line.GetM_ProductContainer_ID())
                                            containerCurrentQty = trx.GetContainerCurrentQty();
                                    }
                                    #endregion

                                    #region update Storage if last record of loop
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != qtyDiffer)
                                        {
                                            storage.SetQtyOnHand(qtyDiffer);
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
                                    #endregion

                                    continue;
                                }
                            }

                            #region Update Transaction
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(qtyDiffer + trx.GetMovementQty());
                            // update container qty - when the record having same container which is on transaction
                            if (line.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == line.GetM_ProductContainer_ID())
                                trx.SetContainerCurrentQty(containerCurrentQty + trx.GetMovementQty());
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
                                qtyDiffer = trx.GetCurrentQty();
                                if (line.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == line.GetM_ProductContainer_ID())
                                    containerCurrentQty = trx.GetContainerCurrentQty();
                            }
                            #endregion

                            #region update Storage if last record of loop
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != qtyDiffer)
                                {
                                    storage.SetQtyOnHand(qtyDiffer);
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
                            #endregion
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
                errorMessage = Msg.GetMsg(GetCtx(), "ExceptionOccureOnUpdateTrx");
            }
            return errorMessage;
        }

        /// <summary>
        /// Update Transaction Tab to set Current Qty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="trx"></param>
        /// <param name="qtyDiff"></param>
        private void UpdateTransaction(MInventoryLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyDiffer + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID()
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyDiffer + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //    + " AND M_AttributeSetInstance_ID = 0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }

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
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), Get_TrxName());
                                if (!inventory.IsInternalUse())
                                {
                                    inventoryLine.SetParent(inventory);
                                    inventoryLine.SetQtyBook(qtyDiffer);
                                    inventoryLine.SetOpeningStock(qtyDiffer);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(qtyDiffer, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(qtyDiffer, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                    }
                                    else
                                    {
                                        qtyDiffer = trx.GetCurrentQty();
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
                                        if (storage.GetQtyOnHand() != qtyDiffer)
                                        {
                                            storage.SetQtyOnHand(qtyDiffer);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(qtyDiffer + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                            }
                            else
                            {
                                qtyDiffer = trx.GetCurrentQty();
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
                                if (storage.GetQtyOnHand() != qtyDiffer)
                                {
                                    storage.SetQtyOnHand(qtyDiffer);
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

        private void UpdateCurrentRecord(MInventoryLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

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
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

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
        private decimal? GetProductQtyFromStorage(MInventoryLine line)
        {
            return 0;
            //MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            //int attribSet_ID = pro.GetM_AttributeSet_ID();
            //string sql = "";

            //if (attribSet_ID > 0)
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
            //         + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
            //}
            //else
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
            //}
            //return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromTransaction(MInventoryLine line, DateTime? movementDate, bool isAttribute)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id  =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id  =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                         AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
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
        private Decimal? GetContainerQtyFromTransaction(MInventoryLine line, DateTime? movementDate)
        {
            Decimal result = 0;
            string sql = @"SELECT DISTINCT FIRST_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty
                           FROM M_Transaction t
                           WHERE t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND t.AD_Client_ID                       = " + line.GetAD_Client_ID() + @"
                           AND t.M_Locator_ID                       = " + line.GetM_Locator_ID() + @"
                           AND t.M_Product_ID                       = " + line.GetM_Product_ID() + @"
                           AND NVL(t.M_AttributeSetInstance_ID , 0) = COALESCE(" + line.GetM_AttributeSetInstance_ID() + @",0)
                           AND NVL(t.M_ProductContainer_ID, 0)              = " + line.GetM_ProductContainer_ID();
            result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
            return result;
        }

        /**
         * 	Check Material Policy.
         * 	Sets line ASI
         */
        private void CheckMaterialPolicy(MInventoryLine line, Decimal qtyDiff)
        {
            int no = MInventoryLineMA.DeleteInventoryMA(line.GetM_InventoryLine_ID(), Get_TrxName());
            if (no > 0)
            {
                log.Config("Delete old #" + no);
            }

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

            //	Incoming Trx
            MClient client = MClient.Get(GetCtx());
            bool needSave = false;

            MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());

            log.Fine("Count=" + line.GetQtyCount()
               + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff);

            MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
            String MMPolicy = pc.GetMMPolicy();
            if (MMPolicy == null || MMPolicy.Length == 0)
                MMPolicy = client.GetMMPolicy();

            //In Trx 
            if (qtyDiff > 0)
            {
                Decimal qtyToReceive = qtyDiff;
                //auto balance negative on hand
                if (isContainerApplicable)
                {
                    qtyToReceive = autoBalanceNegative(line, product, qtyDiff);
                }

                //Allocate remaining qty.
                if (qtyToReceive.CompareTo(Env.ZERO) > 0)
                {
                    MInventoryLineMA ma = MInventoryLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), qtyToReceive, GetMovementDate());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Handle exception
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                }
            }
            // Out Trx
            else
            {
                //bool isLifoChecked = false;
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
                    storages = MStorage.GetWarehouse(GetCtx(), GetM_Warehouse_ID(), line.GetM_Product_ID(),
                        line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                           line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                           MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                }

                Decimal qtyToDeliver = qtyDiff;

                //LIFOManage:
                for (int ii = 0; ii < storages.Length; ii++)
                {
                    dynamic storage = storages[ii];

                    // when storage qty is less than equal to ZERO then continue to other record
                    if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()) <= 0)
                        continue;

                    if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()).CompareTo(Decimal.Negate(qtyToDeliver)) >= 0)
                    {
                        MInventoryLineMA ma = MInventoryLineMA.GetOrCreate(line, storage.GetM_AttributeSetInstance_ID(),
                            Decimal.Negate(qtyToDeliver), isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
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
                        MInventoryLineMA ma = MInventoryLineMA.GetOrCreate(line,
                               storage.GetM_AttributeSetInstance_ID(),
                               isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand(),
                               isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                        if (!ma.Save(line.Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (!String.IsNullOrEmpty(pp.GetName()))
                                throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                            else
                                throw new ArgumentException("Attribute Tab not saved");
                        }
                        qtyToDeliver = Decimal.Add(qtyToDeliver, isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand());
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
                }

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
                    MInventoryLineMA ma = MInventoryLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), Decimal.Negate(qtyToDeliver), GetMovementDate());
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
            }

            if (needSave && !line.Save())
            {
                log.Severe("NOT saved " + line);
            }
        }

        /// <summary>
        /// Is used to make Container Storage Record Qty as Positive when we Receive Qty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="product"></param>
        /// <param name="qtyToReceive"></param>
        /// <returns></returns>
        protected Decimal autoBalanceNegative(MInventoryLine line, MProduct product, Decimal qtyToReceive)
        {
            // Is Used to get Material Policy
            MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());

            // Is Used to get all Negative records from Contaner Storage
            X_M_ContainerStorage[] storages = MProductContainer.GetContainerStorageNegative(GetCtx(), GetM_Warehouse_ID(), line.GetM_Locator_ID(),
                                              line.GetM_ProductContainer_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                    null, MClient.MMPOLICY_FiFo.Equals(pc.GetMMPolicy()), Get_Trx());

            DateTime? dateMPolicy = null;

            for (int ii = 0; ii < storages.Length; ii++)
            {
                X_M_ContainerStorage storage = storages[ii];
                if (storage.GetQty() < 0 && qtyToReceive.CompareTo(Env.ZERO) > 0)
                {
                    dateMPolicy = storage.GetMMPolicyDate();
                    Decimal lineMAQty = qtyToReceive;
                    if (lineMAQty.CompareTo(Decimal.Negate(storage.GetQty())) > 0)
                        lineMAQty = Decimal.Negate(storage.GetQty());

                    //Using ASI from storage record
                    MInventoryLineMA ma = MInventoryLineMA.GetOrCreate(line, storage.GetM_AttributeSetInstance_ID(), lineMAQty, dateMPolicy);
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Exception need to handle
                    }
                    qtyToReceive = Decimal.Subtract(qtyToReceive, lineMAQty);
                }
            }
            return qtyToReceive;
        }


        /**
         * 	Void Document.
         * 	@return false 
         */
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MInventoryLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInventoryLine line = lines[i];
                    Decimal oldCount = line.GetQtyCount();
                    Decimal oldInternal = line.GetQtyInternalUse();
                    if (oldCount.CompareTo(line.GetQtyBook()) != 0
                        || Env.Signum(oldInternal) != 0)
                    {
                        line.SetQtyInternalUse(Env.ZERO);
                        line.SetQtyCount(line.GetQtyBook());
                        line.AddDescription("Void (" + oldCount + "/" + oldInternal + ")");
                        line.Save(Get_TrxName());
                    }

                    // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
                    if (line.GetM_RequisitionLine_ID() > 0 && IsInternalUse())
                    {
                        MRequisitionLine requisitionLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                        requisitionLine.SetDTD001_ReservedQty(Decimal.Subtract(requisitionLine.GetDTD001_ReservedQty(), oldInternal));
                        requisitionLine.Save(Get_Trx());

                        MStorage storageFrom = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_Trx());
                        if (storageFrom == null)
                            storageFrom = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), 0, Get_Trx());
                        storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), oldInternal));
                        storageFrom.Save(Get_Trx());
                    }

                }
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Close Document.
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());

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


            //	Deep Copy
            MInventory reversal = new MInventory(GetCtx(), 0, Get_TrxName());
            CopyValues(this, reversal, GetAD_Client_ID(), GetAD_Org_ID());

            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);	//	indicate reversals

            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetIsApproved(false);
            reversal.SetPosted(false);
            reversal.SetProcessed(false);
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (reversal.Get_ColumnIndex("ReversalDoc_ID") > 0 && reversal.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reversal.SetIsReversal(true);
                // Set Orignal Document Reference
                reversal.SetReversalDoc_ID(GetM_Inventory_ID());
            }

            // for reversal document set Temp Document No to empty
            if (reversal.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                reversal.SetTempDocumentNo("");
            }

            if (!reversal.Save())
            {
                pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Could not create Inventory Reversal, " + pp.GetName();
                else
                    _processMsg = "Could not create Inventory Reversal";
                return false;
            }

            //	Reverse Line Qty
            MInventoryLine[] oLines = GetLines(true);
            for (int i = 0; i < oLines.Length; i++)
            {
                MInventoryLine oLine = oLines[i];
                MInventoryLine rLine = new MInventoryLine(GetCtx(), 0, Get_TrxName());
                CopyValues(oLine, rLine, oLine.GetAD_Client_ID(), oLine.GetAD_Org_ID());
                rLine.SetM_Inventory_ID(reversal.GetM_Inventory_ID());
                rLine.SetParent(reversal);
                //set container reference(not a copy record)
                rLine.SetM_ProductContainer_ID(oLine.GetM_ProductContainer_ID());
                //
                rLine.SetQtyBook(oLine.GetQtyCount());		//	switch
                rLine.SetQtyCount(oLine.GetQtyBook());
                rLine.SetQtyInternalUse(Decimal.Negate(oLine.GetQtyInternalUse()));
                rLine.SetOpeningStock(oLine.GetAsOnDateCount());
                rLine.SetAsOnDateCount(oLine.GetOpeningStock());
                rLine.SetDifferenceQty(rLine.GetOpeningStock() - rLine.GetAsOnDateCount());
                if (rLine.Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    // Set Orignal Document Reference
                    rLine.SetReversalDoc_ID(oLine.GetM_InventoryLine_ID());
                }
                rLine.SetActualReqReserved(oLine.GetActualReqReserved());
                if (!rLine.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Could not create Inventory Reversal Line, " + pp.GetName();
                    else
                        _processMsg = "Could not create Inventory Reversal Line";
                    return false;
                }

                //We need to copy Attribute MA
                MInventoryLineMA[] mas = MInventoryLineMA.Get(GetCtx(), oLines[i].GetM_InventoryLine_ID(), Get_TrxName());
                for (int j = 0; j < mas.Length; j++)
                {
                    MInventoryLineMA ma = new MInventoryLineMA(rLine,
                            mas[j].GetM_AttributeSetInstance_ID(),
                            Decimal.Negate(mas[j].GetMovementQty()), mas[j].GetMMPolicyDate());
                    if (!ma.Save(rLine.Get_TrxName()))
                    {
                        pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = "Could not create Inventory Reversal Attribute , " + pp.GetName();
                        else
                            _processMsg = "Could not create Inventory Reversal Attribute";
                        return false;
                    }
                }
            }
            if (!IsInternalUse())
            {
                reversal.SetIsAdjusted(true);
                if (!reversal.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Could not create Ship Reversal , " + pp.GetName();
                    else
                        _processMsg = "Could not update Inventory Reversal";
                    return false;
                }
            }
            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save();

            //JID_0889: show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reversal.GetDocumentNo();

            //	Update Reversed (this)
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");
            SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);	//	may come from void
            SetDocAction(DOCACTION_None);

            return true;
        }

        /**
         * 	Reverse Accrual
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


        /*
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            //Added by Bharat on 30/12/2016 for optimization
            String sql = "SELECT COUNT(M_InventoryLine_ID) FROM M_InventoryLine WHERE M_Inventory_ID=" + GetM_Inventory_ID();
            int lines = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(lines).Append(")");
            //.Append(" (#").Append(GetLines(false).Length).Append(")");
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
            //	MPriceList pl = MPriceList.get(GetCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();
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
}
