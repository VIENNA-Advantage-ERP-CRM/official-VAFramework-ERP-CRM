
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
    /** Generated Model for VAM_Storage
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_Storage : VAdvantage.Model.PO
    {
        public X_VAM_Storage(Context ctx, int VAM_Storage_ID, Trx trxName)
            : base(ctx, VAM_Storage_ID, trxName)
        {
            /** if (VAM_Storage_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetQtyOnHand (0.0);
            SetQtyOrdered (0.0);
            SetQtyReserved (0.0);
            }
             */
        }
        public X_VAM_Storage(Ctx ctx, int VAM_Storage_ID, Trx trxName)
            : base(ctx, VAM_Storage_ID, trxName)
        {
            /** if (VAM_Storage_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetQtyOnHand (0.0);
            SetQtyOrdered (0.0);
            SetQtyReserved (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Storage(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Storage(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Storage(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_Storage()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new VAdvantage.Model.KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514381371L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064582L;
        /** VAF_TableView_ID=250 */
        public static int Table_ID;
        // =250;

        /** TableName=VAM_Storage */
        public static String Table_Name = "VAM_Storage";

        protected static VAdvantage.Model.KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return VAdvantage.Model.PO Info
        */
        protected override VAdvantage.Model.POInfo InitPO(Context ctx)
        {
            VAdvantage.Model.POInfo poi = VAdvantage.Model.POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return VAdvantage.Model.PO Info
        */
        protected override VAdvantage.Model.POInfo InitPO(Ctx ctx)
        {
            VAdvantage.Model.POInfo poi = VAdvantage.Model.POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAM_Storage[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Requisition Reserved Quantity.
        @param DTD001_QtyReserved Requisition Reserved Quantity */
        public void SetDTD001_QtyReserved(Decimal? DTD001_QtyReserved)
        {
            Set_Value("DTD001_QtyReserved", (Decimal?)DTD001_QtyReserved);
        }
        /** Get Requisition Reserved Quantity.
        @return Requisition Reserved Quantity */
        public Decimal GetDTD001_QtyReserved()
        {
            Object bd = Get_Value("DTD001_QtyReserved");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Date last inventory count.
        @param DateLastInventory Date of Last Inventory Count */
        public void SetDateLastInventory(DateTime? DateLastInventory)
        {
            Set_Value("DateLastInventory", (DateTime?)DateLastInventory);
        }
        /** Get Date last inventory count.
        @return Date of Last Inventory Count */
        public DateTime? GetDateLastInventory()
        {
            return (DateTime?)Get_Value("DateLastInventory");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public VAdvantage.Model.KeyNamePair GetKeyNamePair()
        {
            return new VAdvantage.Model.KeyNamePair(Get_ID(), GetVAM_Product_ID().ToString());
        }
        /** Set On Hand Quantity.
        @param QtyOnHand On Hand Quantity */
        public void SetQtyOnHand(Decimal? QtyOnHand)
        {
            if (QtyOnHand == null) throw new ArgumentException("QtyOnHand is mandatory.");
            Set_ValueNoCheck("QtyOnHand", (Decimal?)QtyOnHand);
        }
        /** Get On Hand Quantity.
        @return On Hand Quantity */
        public Decimal GetQtyOnHand()
        {
            Object bd = Get_Value("QtyOnHand");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Ordered Quantity.
        @param QtyOrdered Ordered Quantity */
        public void SetQtyOrdered(Decimal? QtyOrdered)
        {
            if (QtyOrdered == null) throw new ArgumentException("QtyOrdered is mandatory.");
            Set_ValueNoCheck("QtyOrdered", (Decimal?)QtyOrdered);
        }
        /** Get Ordered Quantity.
        @return Ordered Quantity */
        public Decimal GetQtyOrdered()
        {
            Object bd = Get_Value("QtyOrdered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Reserved.
        @param QtyReserved Quantity Reserved */
        public void SetQtyReserved(Decimal? QtyReserved)
        {
            if (QtyReserved == null) throw new ArgumentException("QtyReserved is mandatory.");
            Set_ValueNoCheck("QtyReserved", (Decimal?)QtyReserved);
        }
        /** Get Quantity Reserved.
        @return Quantity Reserved */
        public Decimal GetQtyReserved()
        {
            Object bd = Get_Value("QtyReserved");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        public void SetQtyType(String QtyType)
        {
            if (QtyType == null) throw new ArgumentException("QtyType is mandatory");
            if (!IsQtyTypeValid(QtyType))
                throw new ArgumentException("QtyType Invalid value - " + QtyType + " - Reference_ID=533 - A - D - E - H - O - R");
            if (QtyType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                QtyType = QtyType.Substring(0, 1);
            }
            Set_ValueNoCheck("QtyType", QtyType);
        }
        /** Get Quantity Type.
        @return Quantity Type */
        public String GetQtyType()
        {
            return (String)Get_Value("QtyType");
        }
        /** QtyType VAF_Control_Ref_ID=533 */
        public static int QTYTYPE_VAF_Control_Ref_ID = 533;
        /** Allocated = A */
        public static String QTYTYPE_Allocated = "A";
        /** Dedicated = D */
        public static String QTYTYPE_Dedicated = "D";
        /** Expected = E */
        public static String QTYTYPE_Expected = "E";
        /** On Hand = H */
        public static String QTYTYPE_OnHand = "H";
        /** Ordered = O */
        public static String QTYTYPE_Ordered = "O";
        /** Reserved = R */
        public static String QTYTYPE_Reserved = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsQtyTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("D") || test.Equals("E") || test.Equals("H") || test.Equals("O") || test.Equals("R");
        }
        /** Set Quantity.
@param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            if (Qty == null) throw new ArgumentException("Qty is mandatory.");
            Set_ValueNoCheck("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        //Added By Amit 3-8-2015 VAMRP
        /** Set Quantity Allocated.
      @param QtyAllocated Quantity that has been picked and is awaiting shipment */
        public void SetQtyAllocated(Decimal? QtyAllocated)
        {
            Set_Value("QtyAllocated", (Decimal?)QtyAllocated);
        }
        /** Get Quantity Allocated.
        @return Quantity that has been picked and is awaiting shipment */
        public Decimal GetQtyAllocated()
        {
            Object bd = Get_Value("QtyAllocated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Dedicated.
        @param QtyDedicated Quantity for which there is a pending Warehouse Task */
        public void SetQtyDedicated(Decimal? QtyDedicated)
        {
            Set_Value("QtyDedicated", (Decimal?)QtyDedicated);
        }
        /** Get Quantity Dedicated.
        @return Quantity for which there is a pending Warehouse Task */
        public Decimal GetQtyDedicated()
        {
            Object bd = Get_Value("QtyDedicated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Expected Quantity.
        @param QtyExpected Quantity expected to be received into a locator */
        public void SetQtyExpected(Decimal? QtyExpected)
        {
            Set_Value("QtyExpected", (Decimal?)QtyExpected);
        }
        /** Get Expected Quantity.
        @return Quantity expected to be received into a locator */
        public Decimal GetQtyExpected()
        {
            Object bd = Get_Value("QtyExpected");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /* * @param DTD001_SourceReserve Requisition Reserved Quantity */
        public void SetDTD001_SourceReserve(Decimal? DTD001_SourceReserve)
        {
            Set_Value("DTD001_SourceReserve", (Decimal?)DTD001_SourceReserve);
        }
        /** Get Requisition Reserved Quantity.
        @return Requisition Reserved Quantity */
        public Decimal GetDTD001_SourceReserve()
        {
            Object bd = Get_Value("DTD001_SourceReserve");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        //End

    }

}


//namespace VAdvantage.Model
//{

///** Generated Model - DO NOT CHANGE */
//using System;
//using System.Text;
//using VAdvantage.DataBase;
//using VAdvantage.Common;
//using VAdvantage.Classes;
//using VAdvantage.Process;
//using VAdvantage.Model;
//using VAdvantage.Utility;
//using System.Data;
///** Generated Model for VAM_Storage
// *  @author Jagmohan Bhatt (generated) 
// *  @version Vienna Framework 1.1.1 - $Id$ */
//public class X_VAM_Storage : PO
//{
//public X_VAM_Storage (Context ctx, int VAM_Storage_ID, Trx trxName) : base (ctx, VAM_Storage_ID, trxName)
//{
///** if (VAM_Storage_ID == 0)
//{
//SetVAM_PFeature_SetInstance_ID (0);
//SetVAM_Locator_ID (0);
//SetVAM_Product_ID (0);
//SetQtyOnHand (0.0);
//SetQtyOrdered (0.0);
//SetQtyReserved (0.0);
//}
// */
//}
//public X_VAM_Storage (Ctx ctx, int VAM_Storage_ID, Trx trxName) : base (ctx, VAM_Storage_ID, trxName)
//{
///** if (VAM_Storage_ID == 0)
//{
//SetVAM_PFeature_SetInstance_ID (0);
//SetVAM_Locator_ID (0);
//SetVAM_Product_ID (0);
//SetQtyOnHand (0.0);
//SetQtyOrdered (0.0);
//SetQtyReserved (0.0);
//}
// */
//}
///** Load Constructor 
//@param ctx context
//@param rs result set 
//@param trxName transaction
//*/
//public X_VAM_Storage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
//{
//}
///** Load Constructor 
//@param ctx context
//@param rs result set 
//@param trxName transaction
//*/
//public X_VAM_Storage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
//{
//}
///** Load Constructor 
//@param ctx context
//@param rs result set 
//@param trxName transaction
//*/
//public X_VAM_Storage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
//{
//}
///** Static Constructor 
// Set Table ID By Table Name
// added by ->Harwinder */
//static X_VAM_Storage()
//{
// Table_ID = Get_Table_ID(Table_Name);
// model = new KeyNamePair(Table_ID,Table_Name);
//}
///** Serial Version No */
////static long serialVersionUID 27562514381371L;
///** Last Updated Timestamp 7/29/2010 1:07:44 PM */
//public static long updatedMS = 1280389064582L;
///** VAF_TableView_ID=250 */
//public static int Table_ID;
// // =250;

///** TableName=VAM_Storage */
//public static String Table_Name="VAM_Storage";

//protected static KeyNamePair model;
//protected Decimal accessLevel = new Decimal(3);
///** AccessLevel
//@return 3 - Client - Org 
//*/
//protected override int Get_AccessLevel()
//{
//return Convert.ToInt32(accessLevel.ToString());
//}
///** Load Meta Data
//@param ctx context
//@return PO Info
//*/
//protected override POInfo InitPO (Ctx ctx)
//{
//POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
//return poi;
//}
///** Load Meta Data
//@param ctx context
//@return PO Info
//*/
//protected override POInfo InitPO (Ctx ctx)
//{
//POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
//return poi;
//}
///** Info
//@return info
//*/
//public override String ToString()
//{
//StringBuilder sb = new StringBuilder ("X_VAM_Storage[").Append(Get_ID()).Append("]");
//return sb.ToString();
//}
///** Set Date last inventory count.
//@param DateLastInventory Date of Last Inventory Count */
//public void SetDateLastInventory (DateTime? DateLastInventory)
//{
//Set_Value ("DateLastInventory", (DateTime?)DateLastInventory);
//}
///** Get Date last inventory count.
//@return Date of Last Inventory Count */
//public DateTime? GetDateLastInventory() 
//{
//return (DateTime?)Get_Value("DateLastInventory");
//}
///** Set Attribute Set Instance.
//@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
//public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
//{
//if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException ("VAM_PFeature_SetInstance_ID is mandatory.");
//Set_ValueNoCheck ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
//}
///** Get Attribute Set Instance.
//@return Product Attribute Set Instance */
//public int GetVAM_PFeature_SetInstance_ID() 
//{
//Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Set Locator.
//@param VAM_Locator_ID Warehouse Locator */
//public void SetVAM_Locator_ID (int VAM_Locator_ID)
//{
//if (VAM_Locator_ID < 1) throw new ArgumentException ("VAM_Locator_ID is mandatory.");
//Set_ValueNoCheck ("VAM_Locator_ID", VAM_Locator_ID);
//}
///** Get Locator.
//@return Warehouse Locator */
//public int GetVAM_Locator_ID() 
//{
//Object ii = Get_Value("VAM_Locator_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Set Product.
//@param VAM_Product_ID Product, Service, Item */
//public void SetVAM_Product_ID (int VAM_Product_ID)
//{
//if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
//Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
//}
///** Get Product.
//@return Product, Service, Item */
//public int GetVAM_Product_ID() 
//{
//Object ii = Get_Value("VAM_Product_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Get Record ID/ColumnName
//@return ID/ColumnName pair */
//public KeyNamePair GetKeyNamePair() 
//{
//return new KeyNamePair(Get_ID(), GetVAM_Product_ID().ToString());
//}
///** Set On Hand Quantity.
//@param QtyOnHand On Hand Quantity */
//public void SetQtyOnHand (Decimal? QtyOnHand)
//{
//if (QtyOnHand == null) throw new ArgumentException ("QtyOnHand is mandatory.");
//Set_ValueNoCheck ("QtyOnHand", (Decimal?)QtyOnHand);
//}
///** Get On Hand Quantity.
//@return On Hand Quantity */
//public Decimal GetQtyOnHand() 
//{
//Object bd =Get_Value("QtyOnHand");
//if (bd == null) return Env.ZERO;
//return  Convert.ToDecimal(bd);
//}
///** Set Ordered Quantity.
//@param QtyOrdered Ordered Quantity */
//public void SetQtyOrdered (Decimal? QtyOrdered)
//{
//if (QtyOrdered == null) throw new ArgumentException ("QtyOrdered is mandatory.");
//Set_ValueNoCheck ("QtyOrdered", (Decimal?)QtyOrdered);
//}
///** Get Ordered Quantity.
//@return Ordered Quantity */
//public Decimal GetQtyOrdered() 
//{
//Object bd =Get_Value("QtyOrdered");
//if (bd == null) return Env.ZERO;
//return  Convert.ToDecimal(bd);
//}
///** Set Quantity Reserved.
//@param QtyReserved Quantity Reserved */
//public void SetQtyReserved (Decimal? QtyReserved)
//{
//if (QtyReserved == null) throw new ArgumentException ("QtyReserved is mandatory.");
//Set_ValueNoCheck ("QtyReserved", (Decimal?)QtyReserved);
//}
///** Get Quantity Reserved.
//@return Quantity Reserved */
//public Decimal GetQtyReserved() 
//{
//Object bd =Get_Value("QtyReserved");
//if (bd == null) return Env.ZERO;
//return  Convert.ToDecimal(bd);
//}
//}

//}
