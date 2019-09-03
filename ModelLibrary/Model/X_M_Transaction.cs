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
    using System.Data;
    /** Generated Model for M_Transaction
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Transaction : PO
    {
        public X_M_Transaction(Context ctx, int M_Transaction_ID, Trx trxName)
            : base(ctx, M_Transaction_ID, trxName)
        {
            /** if (M_Transaction_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_Locator_ID (0);
            SetM_Product_ID (0);
            SetM_Transaction_ID (0);
            SetMovementDate (DateTime.Now);
            SetMovementQty (0.0);
            SetMovementType (null);
            }
             */
        }
        public X_M_Transaction(Ctx ctx, int M_Transaction_ID, Trx trxName)
            : base(ctx, M_Transaction_ID, trxName)
        {
            /** if (M_Transaction_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_Locator_ID (0);
            SetM_Product_ID (0);
            SetM_Transaction_ID (0);
            SetMovementDate (DateTime.Now);
            SetMovementQty (0.0);
            SetMovementType (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Transaction(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Transaction(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Transaction(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Transaction()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514381403L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064614L;
        /** AD_Table_ID=329 */
        public static int Table_ID;
        // =329;

        /** TableName=M_Transaction */
        public static String Table_Name = "M_Transaction";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_M_Transaction[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Project Issue.
        @param C_ProjectIssue_ID Project Issues (Material, Labor) */
        public void SetC_ProjectIssue_ID(int C_ProjectIssue_ID)
        {
            if (C_ProjectIssue_ID <= 0) Set_ValueNoCheck("C_ProjectIssue_ID", null);
            else
                Set_ValueNoCheck("C_ProjectIssue_ID", C_ProjectIssue_ID);
        }
        /** Get Project Issue.
        @return Project Issues (Material, Labor) */
        public int GetC_ProjectIssue_ID()
        {
            Object ii = Get_Value("C_ProjectIssue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID < 0) throw new ArgumentException("M_AttributeSetInstance_ID is mandatory.");
            Set_ValueNoCheck("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_ValueNoCheck("M_InOutLine_ID", null);
            else
                Set_ValueNoCheck("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys Inventory Line.
        @param M_InventoryLine_ID Unique line in an Inventory document */
        public void SetM_InventoryLine_ID(int M_InventoryLine_ID)
        {
            if (M_InventoryLine_ID <= 0) Set_ValueNoCheck("M_InventoryLine_ID", null);
            else
                Set_ValueNoCheck("M_InventoryLine_ID", M_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetM_InventoryLine_ID()
        {
            Object ii = Get_Value("M_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 1) throw new ArgumentException("M_Locator_ID is mandatory.");
            Set_ValueNoCheck("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Move Line.
        @param M_MovementLine_ID Inventory Move document Line */
        public void SetM_MovementLine_ID(int M_MovementLine_ID)
        {
            if (M_MovementLine_ID <= 0) Set_ValueNoCheck("M_MovementLine_ID", null);
            else
                Set_ValueNoCheck("M_MovementLine_ID", M_MovementLine_ID);
        }
        /** Get Move Line.
        @return Inventory Move document Line */
        public int GetM_MovementLine_ID()
        {
            Object ii = Get_Value("M_MovementLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
            Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Line.
        @param M_ProductionLine_ID Document Line representing a production */
        public void SetM_ProductionLine_ID(int M_ProductionLine_ID)
        {
            if (M_ProductionLine_ID <= 0) Set_ValueNoCheck("M_ProductionLine_ID", null);
            else
                Set_ValueNoCheck("M_ProductionLine_ID", M_ProductionLine_ID);
        }
        /** Get Production Line.
        @return Document Line representing a production */
        public int GetM_ProductionLine_ID()
        {
            Object ii = Get_Value("M_ProductionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Transaction.
        @param M_Transaction_ID Inventory Transaction */
        public void SetM_Transaction_ID(int M_Transaction_ID)
        {
            if (M_Transaction_ID < 1) throw new ArgumentException("M_Transaction_ID is mandatory.");
            Set_ValueNoCheck("M_Transaction_ID", M_Transaction_ID);
        }
        /** Get Inventory Transaction.
        @return Inventory Transaction */
        public int GetM_Transaction_ID()
        {
            Object ii = Get_Value("M_Transaction_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Movement Date.
        @param MovementDate Date a product was moved in or out of inventory */
        public void SetMovementDate(DateTime? MovementDate)
        {
            if (MovementDate == null) throw new ArgumentException("MovementDate is mandatory.");
            Set_ValueNoCheck("MovementDate", (DateTime?)MovementDate);
        }
        /** Get Movement Date.
        @return Date a product was moved in or out of inventory */
        public DateTime? GetMovementDate()
        {
            return (DateTime?)Get_Value("MovementDate");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetMovementDate().ToString());
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_ValueNoCheck("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** MovementType AD_Reference_ID=189 */
        public static int MOVEMENTTYPE_AD_Reference_ID = 189;
        /** Customer Returns = C+ */
        public static String MOVEMENTTYPE_CustomerReturns = "C+";
        /** Customer Shipment = C- */
        public static String MOVEMENTTYPE_CustomerShipment = "C-";
        /** Inventory In = I+ */
        public static String MOVEMENTTYPE_InventoryIn = "I+";
        /** Inventory Out = I- */
        public static String MOVEMENTTYPE_InventoryOut = "I-";
        /** Movement To = M+ */
        public static String MOVEMENTTYPE_MovementTo = "M+";
        /** Movement From = M- */
        public static String MOVEMENTTYPE_MovementFrom = "M-";
        /** Production + = P+ */
        public static String MOVEMENTTYPE_ProductionPlus = "P+";
        /** Production - = P- */
        public static String MOVEMENTTYPE_Production_ = "P-";
        /** Vendor Receipts = V+ */
        public static String MOVEMENTTYPE_VendorReceipts = "V+";
        /** Vendor Returns = V- */
        public static String MOVEMENTTYPE_VendorReturns = "V-";
        /** Work Order + = W+ */
        public static String MOVEMENTTYPE_WorkOrderPlus = "W+";
        /** Work Order - = W- */
        public static String MOVEMENTTYPE_WorkOrder_ = "W-";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMovementTypeValid(String test)
        {
            return test.Equals("C+") || test.Equals("C-") || test.Equals("I+") || test.Equals("I-") || test.Equals("M+") || test.Equals("M-") || test.Equals("P+") || test.Equals("P-") || test.Equals("V+") || test.Equals("V-") || test.Equals("W+") || test.Equals("W-");
        }
        /** Set Movement Type.
        @param MovementType Method of moving the inventory */
        public void SetMovementType(String MovementType)
        {
            if (MovementType == null) throw new ArgumentException("MovementType is mandatory");
            if (!IsMovementTypeValid(MovementType))
                throw new ArgumentException("MovementType Invalid value - " + MovementType + " - Reference_ID=189 - C+ - C- - I+ - I- - M+ - M- - P+ - P- - V+ - V- - W+ - W-");
            if (MovementType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                MovementType = MovementType.Substring(0, 2);
            }
            Set_ValueNoCheck("MovementType", MovementType);
        }
        /** Get Movement Type.
        @return Method of moving the inventory */
        public String GetMovementType()
        {
            return (String)Get_Value("MovementType");
        }
        /** Set Current Quantity.
        @param CurrentQty Current Quantity */
        public void SetCurrentQty(Decimal? CurrentQty)
        {
            Set_Value("CurrentQty", (Decimal?)CurrentQty);
        }
        /** Get Current Quantity.
        @return Current Quantity */
        public Decimal GetCurrentQty()
        {
            Object bd = Get_Value("CurrentQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        // Amit 31-7-2015 MRP
        /** Set Work Order.
          @param VAMFG_M_WorkOrder_ID Work Order */
        public void SetVAMFG_M_WorkOrder_ID(int VAMFG_M_WorkOrder_ID)
        {
            if (VAMFG_M_WorkOrder_ID <= 0) Set_ValueNoCheck("VAMFG_M_WorkOrder_ID", null);
            else
                Set_ValueNoCheck("VAMFG_M_WorkOrder_ID", VAMFG_M_WorkOrder_ID);
        }
        /** Get Work Order .
        @return Work Order */
        public int GetVAMFG_M_WorkOrder_ID()
        {
            Object ii = Get_Value("VAMFG_M_WorkOrder_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Work Order Transaction Line.
           @param VAMFG_M_WrkOdrTrnsctionLine_ID Work Order */
        public void SetVAMFG_M_WrkOdrTrnsctionLine_ID(int VAMFG_M_WrkOdrTrnsctionLine_ID)
        {
            if (VAMFG_M_WrkOdrTrnsctionLine_ID <= 0) Set_ValueNoCheck("VAMFG_M_WrkOdrTrnsctionLine_ID", null);
            else
                Set_ValueNoCheck("VAMFG_M_WrkOdrTrnsctionLine_ID", VAMFG_M_WrkOdrTrnsctionLine_ID);
        }
        /** Get Work Order Transaction Line.
        @return Work Order Transaction Line*/
        public int GetVAMFG_M_WrkOdrTrnsctionLine_ID()
        {
            Object ii = Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Work Order Transaction.
        @param VAMFG_M_WrkOdrTransaction_ID Work Order Transaction*/
        public void SetVAMFG_M_WrkOdrTransaction_ID(int VAMFG_M_WrkOdrTransaction_ID)
        {
            if (VAMFG_M_WrkOdrTransaction_ID <= 0) Set_ValueNoCheck("VAMFG_M_WrkOdrTransaction_ID", null);
            else
                Set_ValueNoCheck("VAMFG_M_WrkOdrTransaction_ID", VAMFG_M_WrkOdrTransaction_ID);
        }
        /** Get Work Order Transaction.
        @return Work Order Transaction*/
        public int GetVAMFG_M_WrkOdrTransaction_ID()
        {
            Object ii = Get_Value("VAMFG_M_WrkOdrTransaction_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        //End Amit

        // Added by Mohit VAWMS 20-8-2015

        /** Set Warehouse Task.
@param VAWMS_WarehouseTask_ID A Warehouse Task represents a basic warehouse operation such as putaway, picking or replenishment. */
        public void SetVAWMS_WarehouseTask_ID(int VAWMS_WarehouseTask_ID)
        {
            if (VAWMS_WarehouseTask_ID <= 0) Set_Value("VAWMS_WarehouseTask_ID", null);
            else
                Set_Value("VAWMS_WarehouseTask_ID", VAWMS_WarehouseTask_ID);
        }
        /** Get Warehouse Task.
        @return A Warehouse Task represents a basic warehouse operation such as putaway, picking or replenishment. */
        public int GetVAWMS_WarehouseTask_ID()
        {
            Object ii = Get_Value("VAWMS_WarehouseTask_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        //END

        /** Set Container Current Qty.@param ContainerCurrentQty Container Current Qty */
        public void SetContainerCurrentQty(Decimal? ContainerCurrentQty) { Set_Value("ContainerCurrentQty", (Decimal?)ContainerCurrentQty); }
        /** Get Container Current Qty.@return Container Current Qty */
        public Decimal GetContainerCurrentQty() { Object bd = Get_Value("ContainerCurrentQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Product Container.@param M_ProductContainer_ID Product Container */
        public void SetM_ProductContainer_ID(int M_ProductContainer_ID)
        {
            if (M_ProductContainer_ID <= 0) Set_Value("M_ProductContainer_ID", null);
            else
                Set_Value("M_ProductContainer_ID", M_ProductContainer_ID);
        }
        /** Get Product Container.@return Product Container */
        public int GetM_ProductContainer_ID() { Object ii = Get_Value("M_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
