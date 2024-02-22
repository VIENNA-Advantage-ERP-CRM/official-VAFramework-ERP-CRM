namespace VAdvantage.Model
{
    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;/** Generated Model for M_InventoryRevaluation
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_InventoryRevaluation : PO
    {
        public X_M_InventoryRevaluation(Context ctx, int M_InventoryRevaluation_ID, Trx trxName) : base(ctx, M_InventoryRevaluation_ID, trxName)
        {/** if (M_InventoryRevaluation_ID == 0){SetC_AcctSchema_ID (0);SetC_Currency_ID (0);SetC_DocType_ID (0);// -1
SetCostingLevel (null);SetCostingMethod (null);SetDateAcct (DateTime.Now);// @#Date@
SetDateDoc (DateTime.Now);// @Date@
SetM_InventoryRevaluation_ID (0);SetRevaluationType (null);} */
        }
        public X_M_InventoryRevaluation(Ctx ctx, int M_InventoryRevaluation_ID, Trx trxName) : base(ctx, M_InventoryRevaluation_ID, trxName)
        {/** if (M_InventoryRevaluation_ID == 0){SetC_AcctSchema_ID (0);SetC_Currency_ID (0);SetC_DocType_ID (0);// -1
SetCostingLevel (null);SetCostingMethod (null);SetDateAcct (DateTime.Now);// @#Date@
SetDateDoc (DateTime.Now);// @Date@
SetM_InventoryRevaluation_ID (0);SetRevaluationType (null);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_InventoryRevaluation(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_InventoryRevaluation(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_InventoryRevaluation(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_InventoryRevaluation() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27960466290496L;/** Last Updated Timestamp 3/9/2023 5:49:33 AM */
        public static long updatedMS = 1678340973707L;/** AD_Table_ID=1000574 */
        public static int Table_ID; // =1000574;
        /** TableName=M_InventoryRevaluation */
        public static String Table_Name = "M_InventoryRevaluation";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_InventoryRevaluation[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID) { if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory."); Set_Value("C_AcctSchema_ID", C_AcctSchema_ID); }/** Get Accounting Schema.
@return Rules for accounting */
        public int GetC_AcctSchema_ID() { Object ii = Get_Value("C_AcctSchema_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency Rate Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID <= 0) Set_Value("C_ConversionType_ID", null);
            else
                Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }/** Get Currency Rate Type.
@return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID() { Object ii = Get_Value("C_ConversionType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID) { if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory."); Set_Value("C_Currency_ID", C_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Type.
@param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID) { if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory."); Set_Value("C_DocType_ID", C_DocType_ID); }/** Get Document Type.
@return Document type or rules */
        public int GetC_DocType_ID() { Object ii = Get_Value("C_DocType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set  Revaluation Period.
@param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID)
        {
            if (C_Period_ID <= 0) Set_Value("C_Period_ID", null);
            else
                Set_Value("C_Period_ID", C_Period_ID);
        }/** Get  Revaluation Period.
@return Period of the Calendar */
        public int GetC_Period_ID() { Object ii = Get_Value("C_Period_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** CostingLevel AD_Reference_ID=355 */
        public static int COSTINGLEVEL_AD_Reference_ID = 355;/** Org + Batch = A */
        public static String COSTINGLEVEL_OrgPlusBatch = "A";/** Batch/Lot = B */
        public static String COSTINGLEVEL_BatchLot = "B";/** Client = C */
        public static String COSTINGLEVEL_Client = "C";/** Warehouse + Batch = D */
        public static String COSTINGLEVEL_WarehousePlusBatch = "D";/** Organization = O */
        public static String COSTINGLEVEL_Organization = "O";/** Warehouse = W */
        public static String COSTINGLEVEL_Warehouse = "W";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCostingLevelValid(String test) { return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("O") || test.Equals("W"); }/** Set Costing Level.
@param CostingLevel The lowest level to accumulate Costing Information */
        public void SetCostingLevel(String CostingLevel)
        {
            if (CostingLevel == null) throw new ArgumentException("CostingLevel is mandatory"); if (!IsCostingLevelValid(CostingLevel))
                throw new ArgumentException("CostingLevel Invalid value - " + CostingLevel + " - Reference_ID=355 - A - B - C - D - O - W"); if (CostingLevel.Length > 1) { log.Warning("Length > 1 - truncated"); CostingLevel = CostingLevel.Substring(0, 1); }
            Set_Value("CostingLevel", CostingLevel);
        }/** Get Costing Level.
@return The lowest level to accumulate Costing Information */
        public String GetCostingLevel() { return (String)Get_Value("CostingLevel"); }
        /** CostingMethod AD_Reference_ID=122 */
        public static int COSTINGMETHOD_AD_Reference_ID = 122;/** Average PO = A */
        public static String COSTINGMETHOD_AveragePO = "A";/** Provisional Weighted Average = B */
        public static String COSTINGMETHOD_ProvisionalWeightedAverage = "B";/** Cost Combination = C */
        public static String COSTINGMETHOD_CostCombination = "C";/** Fifo = F */
        public static String COSTINGMETHOD_Fifo = "F";/** Average Invoice = I */
        public static String COSTINGMETHOD_AverageInvoice = "I";/** Lifo = L */
        public static String COSTINGMETHOD_Lifo = "L";/** Weighted Average PO = O */
        public static String COSTINGMETHOD_WeightedAveragePO = "O";/** Standard Costing = S */
        public static String COSTINGMETHOD_StandardCosting = "S";/** User Defined = U */
        public static String COSTINGMETHOD_UserDefined = "U";/** Weighted Average Cost = W */
        public static String COSTINGMETHOD_WeightedAverageCost = "W";/** Last Invoice = i */
        public static String COSTINGMETHOD_LastInvoice = "i";/** Last PO Price = p */
        public static String COSTINGMETHOD_LastPOPrice = "p";/** _ = x */
        public static String COSTINGMETHOD_ = "x";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCostingMethodValid(String test) { return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("O") || test.Equals("S") || test.Equals("U") || test.Equals("W") || test.Equals("i") || test.Equals("p") || test.Equals("x"); }/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
        public void SetCostingMethod(String CostingMethod)
        {
            if (CostingMethod == null) throw new ArgumentException("CostingMethod is mandatory"); if (!IsCostingMethodValid(CostingMethod))
                throw new ArgumentException("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - B - C - F - I - L - O - S - U - W - i - p - x"); if (CostingMethod.Length > 1) { log.Warning("Length > 1 - truncated"); CostingMethod = CostingMethod.Substring(0, 1); }
            Set_Value("CostingMethod", CostingMethod);
        }/** Get Costing Method.
@return Indicates how Costs will be calculated */
        public String GetCostingMethod() { return (String)Get_Value("CostingMethod"); }/** Set Account Date.
@param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct) { if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory."); Set_Value("DateAcct", (DateTime?)DateAcct); }/** Get Account Date.
@return General Ledger Date */
        public DateTime? GetDateAcct() { return (DateTime?)Get_Value("DateAcct"); }/** Set Document Date.
@param DateDoc Date of the Document */
        public void SetDateDoc(DateTime? DateDoc) { if (DateDoc == null) throw new ArgumentException("DateDoc is mandatory."); Set_Value("DateDoc", (DateTime?)DateDoc); }/** Get Document Date.
@return Date of the Document */
        public DateTime? GetDateDoc() { return (DateTime?)Get_Value("DateDoc"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }
        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;/** <None> = -- */
        public static String DOCACTION_None = "--";/** Approve = AP */
        public static String DOCACTION_Approve = "AP";/** Close = CL */
        public static String DOCACTION_Close = "CL";/** Complete = CO */
        public static String DOCACTION_Complete = "CO";/** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";/** Post = PO */
        public static String DOCACTION_Post = "PO";/** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";/** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";/** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";/** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";/** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";/** Void = VO */
        public static String DOCACTION_Void = "VO";/** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";/** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocActionValid(String test) { return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL"); }/** Set Document Action.
@param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL"); if (DocAction != null && DocAction.Length > 2) { log.Warning("Length > 2 - truncated"); DocAction = DocAction.Substring(0, 2); }
            Set_Value("DocAction", DocAction);
        }/** Get Document Action.
@return The targeted status of the document */
        public String GetDocAction() { return (String)Get_Value("DocAction"); }
        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;/** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";/** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";/** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";/** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";/** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";/** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";/** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";/** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";/** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";/** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";/** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";/** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocStatusValid(String test) { return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP"); }/** Set Document Status.
@param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP"); if (DocStatus != null && DocStatus.Length > 2) { log.Warning("Length > 2 - truncated"); DocStatus = DocStatus.Substring(0, 2); }
            Set_Value("DocStatus", DocStatus);
        }/** Get Document Status.
@return The current status of the document */
        public String GetDocStatus() { return (String)Get_Value("DocStatus"); }/** Set Document No..
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo != null && DocumentNo.Length > 100) { log.Warning("Length > 100 - truncated"); DocumentNo = DocumentNo.Substring(0, 100); } Set_Value("DocumentNo", DocumentNo); }/** Get Document No..
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Generate Revaluations Lines.
@param GenerateRevaluationsLines Generate revaluation lines allows  you to generate the revaluation lines based on the selected parameter like Costing level, Costing Method, Product, Product category and Warehouse. */
        public void SetGenerateRevaluationsLines(String GenerateRevaluationsLines) { if (GenerateRevaluationsLines != null && GenerateRevaluationsLines.Length > 1) { log.Warning("Length > 1 - truncated"); GenerateRevaluationsLines = GenerateRevaluationsLines.Substring(0, 1); } Set_Value("GenerateRevaluationsLines", GenerateRevaluationsLines); }/** Get Generate Revaluations Lines.
@return Generate revaluation lines allows  you to generate the revaluation lines based on the selected parameter like Costing level, Costing Method, Product, Product category and Warehouse. */
        public String GetGenerateRevaluationsLines() { return (String)Get_Value("GenerateRevaluationsLines"); }/** Set Approved.
@param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved) { Set_Value("IsApproved", IsApproved); }/** Get Approved.
@return Indicates if this document requires approval */
        public Boolean IsApproved() { Object oo = Get_Value("IsApproved"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set M_InventoryRevaluation_ID.
@param M_InventoryRevaluation_ID M_InventoryRevaluation_ID */
        public void SetM_InventoryRevaluation_ID(int M_InventoryRevaluation_ID) { if (M_InventoryRevaluation_ID < 1) throw new ArgumentException("M_InventoryRevaluation_ID is mandatory."); Set_ValueNoCheck("M_InventoryRevaluation_ID", M_InventoryRevaluation_ID); }/** Get M_InventoryRevaluation_ID.
@return M_InventoryRevaluation_ID */
        public int GetM_InventoryRevaluation_ID() { Object ii = Get_Value("M_InventoryRevaluation_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
        public void SetM_Product_Category_ID(int M_Product_Category_ID)
        {
            if (M_Product_Category_ID <= 0) Set_Value("M_Product_Category_ID", null);
            else
                Set_Value("M_Product_Category_ID", M_Product_Category_ID);
        }/** Get Product Category.
@return Category of a Product */
        public int GetM_Product_Category_ID() { Object ii = Get_Value("M_Product_Category_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }/** Get Warehouse.
@return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID() { Object ii = Get_Value("M_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Posted.
@param Posted Posting status */
        public void SetPosted(Boolean Posted) { Set_Value("Posted", Posted); }/** Get Posted.
@return Posting status */
        public Boolean IsPosted() { Object oo = Get_Value("Posted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** RevaluationType AD_Reference_ID=1000270 */
        public static int REVALUATIONTYPE_AD_Reference_ID = 1000270;/** On Available Quantity = A */
        public static String REVALUATIONTYPE_OnAvailableQuantity = "A";/** On Sold/Consumed Quantity = S */
        public static String REVALUATIONTYPE_OnSoldConsumedQuantity = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsRevaluationTypeValid(String test) { return test.Equals("A") || test.Equals("S"); }/** Set Revaluation Type.
@param RevaluationType Revaluation type allows to revalue the inventory based on the following methods: 1. On Available Quantity - Revalue Inventory on the available stock. 2. On Sold/Consumed Quantity - Revalue the Sold/Consumed quantities. */
        public void SetRevaluationType(String RevaluationType)
        {
            if (RevaluationType == null) throw new ArgumentException("RevaluationType is mandatory"); if (!IsRevaluationTypeValid(RevaluationType))
                throw new ArgumentException("RevaluationType Invalid value - " + RevaluationType + " - Reference_ID=1000270 - A - S"); if (RevaluationType.Length > 1) { log.Warning("Length > 1 - truncated"); RevaluationType = RevaluationType.Substring(0, 1); }
            Set_Value("RevaluationType", RevaluationType);
        }/** Get Revaluation Type.
@return Revaluation type allows to revalue the inventory based on the following methods: 1. On Available Quantity - Revalue Inventory on the available stock. 2. On Sold/Consumed Quantity - Revalue the Sold/Consumed quantities. */
        public String GetRevaluationType() { return (String)Get_Value("RevaluationType"); }/** Set Total Difference.
@param TotalDifference Difference/unit * In Stock Qty */
        public void SetTotalDifference(Decimal? TotalDifference) { Set_Value("TotalDifference", (Decimal?)TotalDifference); }/** Get Total Difference.
@return Difference/unit * In Stock Qty */
        public Decimal GetTotalDifference() { Object bd = Get_Value("TotalDifference"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}