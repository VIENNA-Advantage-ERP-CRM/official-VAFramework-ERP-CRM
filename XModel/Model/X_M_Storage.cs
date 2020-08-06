
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
    /** Generated Model for M_Storage
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Storage : VAdvantage.Model.PO
    {
        public X_M_Storage(Context ctx, int M_Storage_ID, Trx trxName)
            : base(ctx, M_Storage_ID, trxName)
        {
            /** if (M_Storage_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_Locator_ID (0);
            SetM_Product_ID (0);
            SetQtyOnHand (0.0);
            SetQtyOrdered (0.0);
            SetQtyReserved (0.0);
            }
             */
        }
        public X_M_Storage(Ctx ctx, int M_Storage_ID, Trx trxName)
            : base(ctx, M_Storage_ID, trxName)
        {
            /** if (M_Storage_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_Locator_ID (0);
            SetM_Product_ID (0);
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
        public X_M_Storage(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Storage(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Storage(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Storage()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new VAdvantage.Model.KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514381371L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064582L;
        /** AD_Table_ID=250 */
        public static int Table_ID;
        // =250;

        /** TableName=M_Storage */
        public static String Table_Name = "M_Storage";

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
            StringBuilder sb = new StringBuilder("X_M_Storage[").Append(Get_ID()).Append("]");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public VAdvantage.Model.KeyNamePair GetKeyNamePair()
        {
            return new VAdvantage.Model.KeyNamePair(Get_ID(), GetM_Product_ID().ToString());
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
        /** QtyType AD_Reference_ID=533 */
        public static int QTYTYPE_AD_Reference_ID = 533;
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
///** Generated Model for M_Storage
// *  @author Jagmohan Bhatt (generated) 
// *  @version Vienna Framework 1.1.1 - $Id$ */
//public class X_M_Storage : PO
//{
//public X_M_Storage (Context ctx, int M_Storage_ID, Trx trxName) : base (ctx, M_Storage_ID, trxName)
//{
///** if (M_Storage_ID == 0)
//{
//SetM_AttributeSetInstance_ID (0);
//SetM_Locator_ID (0);
//SetM_Product_ID (0);
//SetQtyOnHand (0.0);
//SetQtyOrdered (0.0);
//SetQtyReserved (0.0);
//}
// */
//}
//public X_M_Storage (Ctx ctx, int M_Storage_ID, Trx trxName) : base (ctx, M_Storage_ID, trxName)
//{
///** if (M_Storage_ID == 0)
//{
//SetM_AttributeSetInstance_ID (0);
//SetM_Locator_ID (0);
//SetM_Product_ID (0);
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
//public X_M_Storage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
//{
//}
///** Load Constructor 
//@param ctx context
//@param rs result set 
//@param trxName transaction
//*/
//public X_M_Storage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
//{
//}
///** Load Constructor 
//@param ctx context
//@param rs result set 
//@param trxName transaction
//*/
//public X_M_Storage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
//{
//}
///** Static Constructor 
// Set Table ID By Table Name
// added by ->Harwinder */
//static X_M_Storage()
//{
// Table_ID = Get_Table_ID(Table_Name);
// model = new KeyNamePair(Table_ID,Table_Name);
//}
///** Serial Version No */
////static long serialVersionUID 27562514381371L;
///** Last Updated Timestamp 7/29/2010 1:07:44 PM */
//public static long updatedMS = 1280389064582L;
///** AD_Table_ID=250 */
//public static int Table_ID;
// // =250;

///** TableName=M_Storage */
//public static String Table_Name="M_Storage";

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
//StringBuilder sb = new StringBuilder ("X_M_Storage[").Append(Get_ID()).Append("]");
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
//@param M_AttributeSetInstance_ID Product Attribute Set Instance */
//public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
//{
//if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
//Set_ValueNoCheck ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
//}
///** Get Attribute Set Instance.
//@return Product Attribute Set Instance */
//public int GetM_AttributeSetInstance_ID() 
//{
//Object ii = Get_Value("M_AttributeSetInstance_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Set Locator.
//@param M_Locator_ID Warehouse Locator */
//public void SetM_Locator_ID (int M_Locator_ID)
//{
//if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");
//Set_ValueNoCheck ("M_Locator_ID", M_Locator_ID);
//}
///** Get Locator.
//@return Warehouse Locator */
//public int GetM_Locator_ID() 
//{
//Object ii = Get_Value("M_Locator_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Set Product.
//@param M_Product_ID Product, Service, Item */
//public void SetM_Product_ID (int M_Product_ID)
//{
//if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
//Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
//}
///** Get Product.
//@return Product, Service, Item */
//public int GetM_Product_ID() 
//{
//Object ii = Get_Value("M_Product_ID");
//if (ii == null) return 0;
//return Convert.ToInt32(ii);
//}
///** Get Record ID/ColumnName
//@return ID/ColumnName pair */
//public KeyNamePair GetKeyNamePair() 
//{
//return new KeyNamePair(Get_ID(), GetM_Product_ID().ToString());
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
