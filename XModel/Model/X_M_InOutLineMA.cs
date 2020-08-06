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
    /** Generated Model for M_InOutLineMA
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_InOutLineMA : PO
    {
        public X_M_InOutLineMA(Context ctx, int M_InOutLineMA_ID, Trx trxName)
            : base(ctx, M_InOutLineMA_ID, trxName)
        {
            /** if (M_InOutLineMA_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_InOutLine_ID (0);
            SetMovementQty (0.0);
            }
             */
        }
        public X_M_InOutLineMA(Ctx ctx, int M_InOutLineMA_ID, Trx trxName)
            : base(ctx, M_InOutLineMA_ID, trxName)
        {
            /** if (M_InOutLineMA_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_InOutLine_ID (0);
            SetMovementQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLineMA(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLineMA(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLineMA(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_InOutLineMA()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379694L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062905L;
        /** AD_Table_ID=762 */
        public static int Table_ID;
        // =762;

        /** TableName=M_InOutLineMA */
        public static String Table_Name = "M_InOutLineMA";

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
            StringBuilder sb = new StringBuilder("X_M_InOutLineMA[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
            if (M_InOutLine_ID < 1) throw new ArgumentException("M_InOutLine_ID is mandatory.");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetM_InOutLine_ID().ToString());
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_Value("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        // Added By Mohit VAWMS 20-8-2015

        /** Set Quantity Allocated.
        @param QtyAllocated Quantity that has been picked and is awaiting shipment */
        public void SetQtyAllocated(Decimal? QtyAllocated)
        {
            if (QtyAllocated == null) throw new ArgumentException("QtyAllocated is mandatory.");
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
        //END

        /// <summary>
        ///  Set Material Policy Date.
        /// </summary>
        /// <param name="MMPolicyDate">Time used for LIFO and FIFO Material Policy</param>
        public void SetMMPolicyDate(DateTime? MMPolicyDate) { if (MMPolicyDate == null) throw new ArgumentException("MMPolicyDate is mandatory."); Set_ValueNoCheck("MMPolicyDate", (DateTime?)MMPolicyDate); }
        /// <summary>
        /// Get Material Policy Date.
        /// </summary>
        /// <returns>Time used for LIFO and FIFO Material Policy</returns>
        public DateTime? GetMMPolicyDate() { return (DateTime?)Get_Value("MMPolicyDate"); }
    }

}
