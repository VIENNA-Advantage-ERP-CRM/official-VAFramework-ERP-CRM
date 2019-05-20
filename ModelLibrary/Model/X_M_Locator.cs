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
    /** Generated Model for M_Locator
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Locator : PO
    {
        public X_M_Locator(Context ctx, int M_Locator_ID, Trx trxName)
            : base(ctx, M_Locator_ID, trxName)
        {
            /** if (M_Locator_ID == 0)
            {
            SetIsDefault (false);
            SetM_Locator_ID (0);
            SetM_Warehouse_ID (0);
            SetPriorityNo (0);	// 50
            SetValue (null);
            }
             */
        }
        public X_M_Locator(Ctx ctx, int M_Locator_ID, Trx trxName)
            : base(ctx, M_Locator_ID, trxName)
        {
            /** if (M_Locator_ID == 0)
            {
            SetIsDefault (false);
            SetM_Locator_ID (0);
            SetM_Warehouse_ID (0);
            SetPriorityNo (0);	// 50
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Locator(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Locator(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Locator(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Locator()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379851L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063062L;
        /** AD_Table_ID=207 */
        public static int Table_ID;
        // =207;

        /** TableName=M_Locator */
        public static String Table_Name = "M_Locator";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_M_Locator[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_ValueNoCheck("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetM_Warehouse_ID().ToString());
        }
        /** Set Relative Priority.
        @param PriorityNo Where inventory should be picked from first */
        public void SetPriorityNo(int PriorityNo)
        {
            Set_Value("PriorityNo", PriorityNo);
        }
        /** Get Relative Priority.
        @return Where inventory should be picked from first */
        public int GetPriorityNo()
        {
            Object ii = Get_Value("PriorityNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
        /** Set Aisle (X).
        @param X X dimension, e.g., Aisle */
        public void SetX(String X)
        {
            if (X != null && X.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                X = X.Substring(0, 60);
            }
            Set_Value("X", X);
        }
        /** Get Aisle (X).
        @return X dimension, e.g., Aisle */
        public String GetX()
        {
            return (String)Get_Value("X");
        }
        /** Set Bin (Y).
        @param Y Y dimension, e.g., Bin */
        public void SetY(String Y)
        {
            if (Y != null && Y.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Y = Y.Substring(0, 60);
            }
            Set_Value("Y", Y);
        }
        /** Get Bin (Y).
        @return Y dimension, e.g., Bin */
        public String GetY()
        {
            return (String)Get_Value("Y");
        }
        /** Set Level (Z).
        @param Z Z dimension, e.g., Level */
        public void SetZ(String Z)
        {
            if (Z != null && Z.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Z = Z.Substring(0, 60);
            }
            Set_Value("Z", Z);
        }
        /** Get Level (Z).
        @return Z dimension, e.g., Level */
        public String GetZ()
        {
            return (String)Get_Value("Z");
        }
        /** Set SAP Locator.
        @param SAP001_Locator SAP Locator */
        public void SetSAP001_Locator(String SAP001_Locator)
        {
            if (SAP001_Locator != null && SAP001_Locator.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                SAP001_Locator = SAP001_Locator.Substring(0, 50);
            }
            Set_Value("SAP001_Locator", SAP001_Locator);
        }
        /** Get SAP Locator.
        @return SAP Locator */
        public String GetSAP001_Locator()
        {
            return (String)Get_Value("SAP001_Locator");
        }
        /** Set Locator.
        @param LocatorCombination Delimited combination of locator segments */
        public void SetLocatorCombination(String LocatorCombination)
        {
            if (LocatorCombination != null && LocatorCombination.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                LocatorCombination = LocatorCombination.Substring(0, 60);
            }
            Set_Value("LocatorCombination", LocatorCombination);
        }
        /** Get Locator.
        @return Delimited combination of locator segments */
        public String GetLocatorCombination()
        {
            return (String)Get_Value("LocatorCombination");
        }

        //Added By Mohit VAWMS 20-8-2015

        /* Picking_UOM_ID AD_Reference_ID=114 /
         public static int PICKING_UOM_ID_AD_Reference_ID = 114;
        /** Set Picking UOM.
        @param Picking_UOM_ID Picking UOM of locator */
        public void SetPicking_UOM_ID(int Picking_UOM_ID)
        {
            if (Picking_UOM_ID <= 0) Set_Value("Picking_UOM_ID", null);
            else
                Set_Value("Picking_UOM_ID", Picking_UOM_ID);
        }
        /** Get Picking UOM.
        @return Picking UOM of locator */
        public int GetPicking_UOM_ID()
        {
            Object ii = Get_Value("Picking_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Bin.
        @param Bin Locator Bin segment */
        public void SetBin(String Bin)
        {
            if (Bin != null && Bin.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Bin = Bin.Substring(0, 60);
            }
            Set_Value("Bin", Bin);
        }
        /** Get Bin.
        @return Locator Bin segment */
        public String GetBin()
        {
            return (String)Get_Value("Bin");
        }

        /** Set Position.
        @param POSITION Position */
        public void SetPOSITION(String POSITION)
        {
            if (POSITION != null && POSITION.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                POSITION = POSITION.Substring(0, 60);
            }
            Set_Value("POSITION", POSITION);
        }
        /** Get Position.
        @return Position */
        public String GetPOSITION()
        {
            return (String)Get_Value("POSITION");
        }

        /** Set Available For Allocation.
        @param IsAvailableForAllocation Stock in this locator is available for allocation */
        public void SetIsAvailableForAllocation(Boolean IsAvailableForAllocation)
        {
            Set_Value("IsAvailableForAllocation", IsAvailableForAllocation);
        }
        /** Get Available For Allocation.
        @return Stock in this locator is available for allocation */
        public Boolean IsAvailableForAllocation()
        {
            Object oo = Get_Value("IsAvailableForAllocation");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Available To Promise.
        @param IsAvailableToPromise Stock in this locator is available to promise */
        public void SetIsAvailableToPromise(Boolean IsAvailableToPromise)
        {
            Set_Value("IsAvailableToPromise", IsAvailableToPromise);
        }
        /** Get Available To Promise.
        @return Stock in this locator is available to promise */
        public Boolean IsAvailableToPromise()
        {
            Object oo = Get_Value("IsAvailableToPromise");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Stocking UOM.
        @param Stocking_UOM_ID Stocking UOM of locator */
        public void SetStocking_UOM_ID(int Stocking_UOM_ID)
        {
            if (Stocking_UOM_ID <= 0) Set_Value("Stocking_UOM_ID", null);
            else
                Set_Value("Stocking_UOM_ID", Stocking_UOM_ID);
        }
        /** Get Stocking UOM.
        @return Stocking UOM of locator */
        public int GetStocking_UOM_ID()
        {
            Object ii = Get_Value("Stocking_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Max Stocking Quantity.
        @param MaxQuantity Maximum stocking capacity of the locator in units */
        public void SetMaxQuantity(Decimal? MaxQuantity)
        {
            Set_Value("MaxQuantity", (Decimal?)MaxQuantity);
        }
        /** Get Max Stocking Quantity.
        @return Maximum stocking capacity of the locator in units */
        public Decimal GetMaxQuantity()
        {
            Object bd = Get_Value("MaxQuantity");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //End
    }

}
