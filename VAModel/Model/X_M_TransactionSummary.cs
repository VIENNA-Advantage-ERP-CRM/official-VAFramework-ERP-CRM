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
    using System.Data;/** Generated Model for M_TransactionSummary
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_TransactionSummary : PO
    {
        public X_M_TransactionSummary(Context ctx, int M_TransactionSummary_ID, Trx trxName)
            : base(ctx, M_TransactionSummary_ID, trxName)
        {/** if (M_TransactionSummary_ID == 0){SetClosingStock (0.0);SetM_AttributeSetInstance_ID (0);SetM_Locator_ID (0);SetM_Product_ID (0);SetM_TransactionSummary_ID (0);SetMovementDate (DateTime.Now);SetOpeningStock (0.0);} */
        }
        public X_M_TransactionSummary(Ctx ctx, int M_TransactionSummary_ID, Trx trxName)
            : base(ctx, M_TransactionSummary_ID, trxName)
        {/** if (M_TransactionSummary_ID == 0){SetClosingStock (0.0);SetM_AttributeSetInstance_ID (0);SetM_Locator_ID (0);SetM_Product_ID (0);SetM_TransactionSummary_ID (0);SetMovementDate (DateTime.Now);SetOpeningStock (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_TransactionSummary(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_TransactionSummary(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_TransactionSummary(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_TransactionSummary()
        {
            Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27759782364854L;
        /** Last Updated Timestamp 10/28/2016 5:47:28 PM */
        public static long updatedMS = 1477657048065L;
        /** AD_Table_ID=1000498 */
        public static int Table_ID; // =1000498;
        /** TableName=M_TransactionSummary */
        public static String Table_Name = "M_TransactionSummary";
        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }
        /** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }
        /** Info
@return info
*/
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_M_TransactionSummary[").Append(Get_ID()).Append("]"); return sb.ToString();
        }
        /** Set Closing Stock.
@param ClosingStock Closing Stock */
        public void SetClosingStock(Decimal? ClosingStock)
        {
            if (ClosingStock == null) throw new ArgumentException("ClosingStock is mandatory."); Set_Value("ClosingStock", (Decimal?)ClosingStock);
        }
        /** Get Closing Stock.
@return Closing Stock */
        public Decimal GetClosingStock()
        {
            Object bd = Get_Value("ClosingStock"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID);
        }
        /** Get Export.
@return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID < 0) throw new ArgumentException("M_AttributeSetInstance_ID is mandatory."); Set_ValueNoCheck("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
        /** Set Locator.
@param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 1) throw new ArgumentException("M_Locator_ID is mandatory."); Set_ValueNoCheck("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
@return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
        /** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory."); Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
        /** Set M_TransactionSummary_ID.
@param M_TransactionSummary_ID M_TransactionSummary_ID */
        public void SetM_TransactionSummary_ID(int M_TransactionSummary_ID)
        {
            if (M_TransactionSummary_ID < 1) throw new ArgumentException("M_TransactionSummary_ID is mandatory."); Set_ValueNoCheck("M_TransactionSummary_ID", M_TransactionSummary_ID);
        }
        /** Get M_TransactionSummary_ID.
@return M_TransactionSummary_ID */
        public int GetM_TransactionSummary_ID()
        {
            Object ii = Get_Value("M_TransactionSummary_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
        /** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
        public void SetMovementDate(DateTime? MovementDate)
        {
            if (MovementDate == null) throw new ArgumentException("MovementDate is mandatory."); Set_ValueNoCheck("MovementDate", (DateTime?)MovementDate);
        }
        /** Get Movement Date.
@return Date a product was moved in or out of inventory */
        public DateTime? GetMovementDate()
        {
            return (DateTime?)Get_Value("MovementDate");
        }
        /** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetMovementDate().ToString()); }
        /** Set Opening Stock.
        @param OpeningStock Opening Stock */
        public void SetOpeningStock(Decimal? OpeningStock)
        {
            if (OpeningStock == null) throw new ArgumentException("OpeningStock is mandatory."); Set_Value("OpeningStock", (Decimal?)OpeningStock);
        }
        /** Get Opening Stock.
        @return Opening Stock */
        public Decimal GetOpeningStock()
        {
            Object bd = Get_Value("OpeningStock"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Customer Return Quantity.
@param QtyCustReturn Customer Return Quantity */
        public void SetQtyCustReturn(Decimal? QtyCustReturn)
        {
            Set_Value("QtyCustReturn", (Decimal?)QtyCustReturn);
        }
        /** Get Customer Return Quantity.
@return Customer Return Quantity */
        public Decimal GetQtyCustReturn()
        {
            Object bd = Get_Value("QtyCustReturn"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Customer Shipment Quantity.
@param QtyCustShipment Customer Shipment Quantity */
        public void SetQtyCustShipment(Decimal? QtyCustShipment)
        {
            Set_Value("QtyCustShipment", (Decimal?)QtyCustShipment);
        }
        /** Get Customer Shipment Quantity.
@return Customer Shipment Quantity */
        public Decimal GetQtyCustShipment()
        {
            Object bd = Get_Value("QtyCustShipment"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Inventory In Quantity.
@param QtyInventoryIn Inventory In Quantity */
        public void SetQtyInventoryIn(Decimal? QtyInventoryIn)
        {
            Set_Value("QtyInventoryIn", (Decimal?)QtyInventoryIn);
        }
        /** Get Inventory In Quantity.
@return Inventory In Quantity */
        public Decimal GetQtyInventoryIn()
        {
            Object bd = Get_Value("QtyInventoryIn"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Inventory Out Quantity.
@param QtyInventoryOut Inventory Out Quantity */
        public void SetQtyInventoryOut(Decimal? QtyInventoryOut)
        {
            Set_Value("QtyInventoryOut", (Decimal?)QtyInventoryOut);
        }
        /** Get Inventory Out Quantity.
@return Inventory Out Quantity */
        public Decimal GetQtyInventoryOut()
        {
            Object bd = Get_Value("QtyInventoryOut"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Vendor Received Quantity.
@param QtyMaterialIn Vendor Received Quantity */
        public void SetQtyMaterialIn(Decimal? QtyMaterialIn)
        {
            Set_Value("QtyMaterialIn", (Decimal?)QtyMaterialIn);
        }
        /** Get Vendor Received Quantity.
@return Vendor Received Quantity */
        public Decimal GetQtyMaterialIn()
        {
            Object bd = Get_Value("QtyMaterialIn"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Vendor Return Quantity.
@param QtyMaterialOut Vendor Return Quantity */
        public void SetQtyMaterialOut(Decimal? QtyMaterialOut)
        {
            Set_Value("QtyMaterialOut", (Decimal?)QtyMaterialOut);
        }
        /** Get Vendor Return Quantity.
@return Vendor Return Quantity */
        public Decimal GetQtyMaterialOut()
        {
            Object bd = Get_Value("QtyMaterialOut"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Movement Out Quantity.
@param QtyMoveOut Movement Out Quantity */
        public void SetQtyMoveOut(Decimal? QtyMoveOut)
        {
            Set_Value("QtyMoveOut", (Decimal?)QtyMoveOut);
        }
        /** Get Movement Out Quantity.
@return Movement Out Quantity */
        public Decimal GetQtyMoveOut()
        {
            Object bd = Get_Value("QtyMoveOut"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Movement To Quantity.
@param QtyMoveTo Movement To Quantity */
        public void SetQtyMoveTo(Decimal? QtyMoveTo)
        {
            Set_Value("QtyMoveTo", (Decimal?)QtyMoveTo);
        }
        /** Get Movement To Quantity.
@return Movement To Quantity */
        public Decimal GetQtyMoveTo()
        {
            Object bd = Get_Value("QtyMoveTo"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Production In Quantity.
@param QtyProductionIn Production In Quantity */
        public void SetQtyProductionIn(Decimal? QtyProductionIn)
        {
            Set_Value("QtyProductionIn", (Decimal?)QtyProductionIn);
        }
        /** Get Production In Quantity.
@return Production In Quantity */
        public Decimal GetQtyProductionIn()
        {
            Object bd = Get_Value("QtyProductionIn"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Production Out Quantity.
@param QtyProductionOut Production Out Quantity */
        public void SetQtyProductionOut(Decimal? QtyProductionOut)
        {
            Set_Value("QtyProductionOut", (Decimal?)QtyProductionOut);
        }
        /** Get Production Out Quantity.
@return Production Out Quantity */
        public Decimal GetQtyProductionOut()
        {
            Object bd = Get_Value("QtyProductionOut"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Work Order in Quantity.
@param QtyWorkOrderIn Work Order in Quantity */
        public void SetQtyWorkOrderIn(Decimal? QtyWorkOrderIn)
        {
            Set_Value("QtyWorkOrderIn", (Decimal?)QtyWorkOrderIn);
        }
        /** Get Work Order in Quantity.
@return Work Order in Quantity */
        public Decimal GetQtyWorkOrderIn()
        {
            Object bd = Get_Value("QtyWorkOrderIn"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
        /** Set Work Order Out Quantity.
@param QtyWorkOrderOut Work Order Out Quantity */
        public void SetQtyWorkOrderOut(Decimal? QtyWorkOrderOut)
        {
            Set_Value("QtyWorkOrderOut", (Decimal?)QtyWorkOrderOut);
        }
        /** Get Work Order Out Quantity.
@return Work Order Out Quantity */
        public Decimal GetQtyWorkOrderOut()
        {
            Object bd = Get_Value("QtyWorkOrderOut"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd);
        }
    }
}