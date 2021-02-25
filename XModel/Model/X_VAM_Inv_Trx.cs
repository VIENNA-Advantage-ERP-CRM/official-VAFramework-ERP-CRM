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
    /** Generated Model for VAM_Inv_Trx
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_Inv_Trx : PO
    {
        public X_VAM_Inv_Trx(Context ctx, int VAM_Inv_Trx_ID, Trx trxName)
            : base(ctx, VAM_Inv_Trx_ID, trxName)
        {
            /** if (VAM_Inv_Trx_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_Inv_Trx_ID (0);
            SetMovementDate (DateTime.Now);
            SetMovementQty (0.0);
            SetMovementType (null);
            }
             */
        }
        public X_VAM_Inv_Trx(Ctx ctx, int VAM_Inv_Trx_ID, Trx trxName)
            : base(ctx, VAM_Inv_Trx_ID, trxName)
        {
            /** if (VAM_Inv_Trx_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_Inv_Trx_ID (0);
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
        public X_VAM_Inv_Trx(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Inv_Trx(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Inv_Trx(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_Inv_Trx()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514381403L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064614L;
        /** VAF_TableView_ID=329 */
        public static int Table_ID;
        // =329;

        /** TableName=VAM_Inv_Trx */
        public static String Table_Name = "VAM_Inv_Trx";

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
            StringBuilder sb = new StringBuilder("X_VAM_Inv_Trx[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Project Issue.
        @param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
        public void SetVAB_ProjectSupply_ID(int VAB_ProjectSupply_ID)
        {
            if (VAB_ProjectSupply_ID <= 0) Set_ValueNoCheck("VAB_ProjectSupply_ID", null);
            else
                Set_ValueNoCheck("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
        }
        /** Get Project Issue.
        @return Project Issues (Material, Labor) */
        public int GetVAB_ProjectSupply_ID()
        {
            Object ii = Get_Value("VAB_ProjectSupply_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException("VAM_PFeature_SetInstance_ID is mandatory.");
            Set_ValueNoCheck("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID()
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_ValueNoCheck("VAM_Inv_InOutLine_ID", null);
            else
                Set_ValueNoCheck("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys Inventory Line.
        @param VAM_InventoryLine_ID Unique line in an Inventory document */
        public void SetVAM_InventoryLine_ID(int VAM_InventoryLine_ID)
        {
            if (VAM_InventoryLine_ID <= 0) Set_ValueNoCheck("VAM_InventoryLine_ID", null);
            else
                Set_ValueNoCheck("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetVAM_InventoryLine_ID()
        {
            Object ii = Get_Value("VAM_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param VAM_Locator_ID Warehouse Locator */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID < 1) throw new ArgumentException("VAM_Locator_ID is mandatory.");
            Set_ValueNoCheck("VAM_Locator_ID", VAM_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetVAM_Locator_ID()
        {
            Object ii = Get_Value("VAM_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Move Line.
        @param VAM_InvTrf_Line_ID Inventory Move document Line */
        public void SetVAM_InvTrf_Line_ID(int VAM_InvTrf_Line_ID)
        {
            if (VAM_InvTrf_Line_ID <= 0) Set_ValueNoCheck("VAM_InvTrf_Line_ID", null);
            else
                Set_ValueNoCheck("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
        }
        /** Get Move Line.
        @return Inventory Move document Line */
        public int GetVAM_InvTrf_Line_ID()
        {
            Object ii = Get_Value("VAM_InvTrf_Line_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory.");
            Set_ValueNoCheck("VAM_Product_ID", VAM_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetVAM_Product_ID()
        {
            Object ii = Get_Value("VAM_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Line.
        @param VAM_ProductionLine_ID Document Line representing a production */
        public void SetVAM_ProductionLine_ID(int VAM_ProductionLine_ID)
        {
            if (VAM_ProductionLine_ID <= 0) Set_ValueNoCheck("VAM_ProductionLine_ID", null);
            else
                Set_ValueNoCheck("VAM_ProductionLine_ID", VAM_ProductionLine_ID);
        }
        /** Get Production Line.
        @return Document Line representing a production */
        public int GetVAM_ProductionLine_ID()
        {
            Object ii = Get_Value("VAM_ProductionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Transaction.
        @param VAM_Inv_Trx_ID Inventory Transaction */
        public void SetVAM_Inv_Trx_ID(int VAM_Inv_Trx_ID)
        {
            if (VAM_Inv_Trx_ID < 1) throw new ArgumentException("VAM_Inv_Trx_ID is mandatory.");
            Set_ValueNoCheck("VAM_Inv_Trx_ID", VAM_Inv_Trx_ID);
        }
        /** Get Inventory Transaction.
        @return Inventory Transaction */
        public int GetVAM_Inv_Trx_ID()
        {
            Object ii = Get_Value("VAM_Inv_Trx_ID");
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

        /** MovementType VAF_Control_Ref_ID=189 */
        public static int MOVEMENTTYPE_VAF_Control_Ref_ID = 189;
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
          @param VAMFG_VAM_WorkOrder_ID Work Order */
        public void SetVAMFG_VAM_WorkOrder_ID(int VAMFG_VAM_WorkOrder_ID)
        {
            if (VAMFG_VAM_WorkOrder_ID <= 0) Set_ValueNoCheck("VAMFG_VAM_WorkOrder_ID", null);
            else
                Set_ValueNoCheck("VAMFG_VAM_WorkOrder_ID", VAMFG_VAM_WorkOrder_ID);
        }
        /** Get Work Order .
        @return Work Order */
        public int GetVAMFG_VAM_WorkOrder_ID()
        {
            Object ii = Get_Value("VAMFG_VAM_WorkOrder_ID");
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

        /** Set Product Container.@param VAM_ProductContainer_ID Product Container */
        public void SetVAM_ProductContainer_ID(int VAM_ProductContainer_ID)
        {
            if (VAM_ProductContainer_ID <= 0) Set_Value("VAM_ProductContainer_ID", null);
            else
                Set_Value("VAM_ProductContainer_ID", VAM_ProductContainer_ID);
        }
        /** Get Product Container.@return Product Container */
        public int GetVAM_ProductContainer_ID() { Object ii = Get_Value("VAM_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
