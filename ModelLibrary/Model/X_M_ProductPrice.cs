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
    /** Generated Model for M_ProductPrice
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_ProductPrice : PO
    {
        public X_M_ProductPrice(Context ctx, int M_ProductPrice_ID, Trx trxName)
            : base(ctx, M_ProductPrice_ID, trxName)
        {
            /** if (M_ProductPrice_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);	// 0
            SetM_PriceList_Version_ID (0);
            SetM_Product_ID (0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetPriceStd (0.0);
            }
             */
        }
        public X_M_ProductPrice(Ctx ctx, int M_ProductPrice_ID, Trx trxName)
            : base(ctx, M_ProductPrice_ID, trxName)
        {
            /** if (M_ProductPrice_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);	// 0
            SetM_PriceList_Version_ID (0);
            SetM_Product_ID (0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetPriceStd (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductPrice(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductPrice(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductPrice(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_ProductPrice()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27695412339691L;
        /** Last Updated Timestamp 10/14/2014 5:13:42 PM */
        public static long updatedMS = 1413287022902L;
        /** AD_Table_ID=251 */
        public static int Table_ID;
        // =251;

        /** TableName=M_ProductPrice */
        public static String Table_Name = "M_ProductPrice";

        protected static KeyNamePair model;
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
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_M_ProductPrice[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            throw new ArgumentException("C_Currency_ID Is virtual column");
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Price includes Tax.
        @param IsTaxIncluded Tax is included in the price  */
        public void SetIsTaxIncluded(Boolean IsTaxIncluded)
        {
            throw new ArgumentException("IsTaxIncluded Is virtual column");
        }
        /** Get Price includes Tax.
        @return Tax is included in the price  */
        public Boolean IsTaxIncluded()
        {
            Object oo = Get_Value("IsTaxIncluded");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Lot No.
        @param Lot Lot number (alphanumeric) */
        public void SetLot(String Lot)
        {
            if (Lot != null && Lot.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Lot = Lot.Substring(0, 50);
            }
            Set_Value("Lot", Lot);
        }
        /** Get Lot No.
        @return Lot number (alphanumeric) */
        public String GetLot()
        {
            return (String)Get_Value("Lot");
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
        /** Set Price List Version.
        @param M_PriceList_Version_ID Identifies a unique instance of a Price List */
        public void SetM_PriceList_Version_ID(int M_PriceList_Version_ID)
        {
            if (M_PriceList_Version_ID < 1) throw new ArgumentException("M_PriceList_Version_ID is mandatory.");
            Set_ValueNoCheck("M_PriceList_Version_ID", M_PriceList_Version_ID);
        }
        /** Get Price List Version.
        @return Identifies a unique instance of a Price List */
        public int GetM_PriceList_Version_ID()
        {
            Object ii = Get_Value("M_PriceList_Version_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetM_PriceList_Version_ID().ToString());
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
        /** Set Limit Price.
        @param PriceLimit Lowest price for a product */
        public void SetPriceLimit(Decimal? PriceLimit)
        {
            if (PriceLimit == null) throw new ArgumentException("PriceLimit is mandatory.");
            Set_Value("PriceLimit", (Decimal?)PriceLimit);
        }
        /** Get Limit Price.
        @return Lowest price for a product */
        public Decimal GetPriceLimit()
        {
            Object bd = Get_Value("PriceLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set List Price.
        @param PriceList List Price */
        public void SetPriceList(Decimal? PriceList)
        {
            if (PriceList == null) throw new ArgumentException("PriceList is mandatory.");
            Set_Value("PriceList", (Decimal?)PriceList);
        }
        /** Get List Price.
        @return List Price */
        public Decimal GetPriceList()
        {
            Object bd = Get_Value("PriceList");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Standard Price.
        @param PriceStd Standard Price */
        public void SetPriceStd(Decimal? PriceStd)
        {
            if (PriceStd == null) throw new ArgumentException("PriceStd is mandatory.");
            Set_Value("PriceStd", (Decimal?)PriceStd);
        }
        /** Get Standard Price.
        @return Standard Price */
        public Decimal GetPriceStd()
        {
            Object bd = Get_Value("PriceStd");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_ValueNoCheck("C_UOM_ID", null);
            else
                Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


    }

}
