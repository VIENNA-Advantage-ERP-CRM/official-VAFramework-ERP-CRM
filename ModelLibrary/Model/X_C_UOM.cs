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
    /** Generated Model for C_UOM
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_UOM : PO
    {
        public X_C_UOM(Context ctx, int C_UOM_ID, Trx trxName)
            : base(ctx, C_UOM_ID, trxName)
        {
            /** if (C_UOM_ID == 0)
            {
            SetC_UOM_ID (0);
            SetCostingPrecision (0);
            SetIsDefault (false);
            SetName (null);
            SetStdPrecision (0);
            SetX12DE355 (null);
            }
             */
        }
        public X_C_UOM(Ctx ctx, int C_UOM_ID, Trx trxName)
            : base(ctx, C_UOM_ID, trxName)
        {
            /** if (C_UOM_ID == 0)
            {
            SetC_UOM_ID (0);
            SetCostingPrecision (0);
            SetIsDefault (false);
            SetName (null);
            SetStdPrecision (0);
            SetX12DE355 (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_UOM(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_UOM(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_UOM(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_UOM()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514375776L;
        /** Last Updated Timestamp 7/29/2010 1:07:38 PM */
        public static long updatedMS = 1280389058987L;
        /** AD_Table_ID=146 */
        public static int Table_ID;
        // =146;

        /** TableName=C_UOM */
        public static String Table_Name = "C_UOM";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
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
            StringBuilder sb = new StringBuilder("X_C_UOM[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
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
        /** Set Costing Precision.
        @param CostingPrecision Rounding used costing calculations */
        public void SetCostingPrecision(int CostingPrecision)
        {
            Set_Value("CostingPrecision", CostingPrecision);
        }
        /** Get Costing Precision.
        @return Rounding used costing calculations */
        public int GetCostingPrecision()
        {
            Object ii = Get_Value("CostingPrecision");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
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
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }
        /** Set Standard Precision.
        @param StdPrecision Rule for rounding  calculated amounts */
        public void SetStdPrecision(int StdPrecision)
        {
            Set_Value("StdPrecision", StdPrecision);
        }
        /** Get Standard Precision.
        @return Rule for rounding  calculated amounts */
        public int GetStdPrecision()
        {
            Object ii = Get_Value("StdPrecision");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Symbol.
        @param UOMSymbol Symbol for a Unit of Measure */
        public void SetUOMSymbol(String UOMSymbol)
        {
            if (UOMSymbol != null && UOMSymbol.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                UOMSymbol = UOMSymbol.Substring(0, 20);
            }
            Set_Value("UOMSymbol", UOMSymbol);
        }
        /** Get Symbol.
        @return Symbol for a Unit of Measure */
        public String GetUOMSymbol()
        {
            return (String)Get_Value("UOMSymbol");
        }
        /** Set UOM Code.
        @param X12DE355 UOM EDI X12 Code */
        public void SetX12DE355(String X12DE355)
        {
            if (X12DE355 == null) throw new ArgumentException("X12DE355 is mandatory.");
            if (X12DE355.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                X12DE355 = X12DE355.Substring(0, 20);
            }
            Set_Value("X12DE355", X12DE355);
        }
        /** Get UOM Code.
        @return UOM EDI X12 Code */
        public String GetX12DE355()
        {
            return (String)Get_Value("X12DE355");
        }

        /** Set Reference UOM ID.
        @param VA007_RefUOM_ID Reference UOM ID */
        public void SetVA007_RefUOM_ID(String VA007_RefUOM_ID)
        {
            if (VA007_RefUOM_ID != null && VA007_RefUOM_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA007_RefUOM_ID = VA007_RefUOM_ID.Substring(0, 50);
            }
            Set_Value("VA007_RefUOM_ID", VA007_RefUOM_ID);
        }
        /** Get Reference UOM ID.
        @return Reference UOM ID */
        public String GetVA007_RefUOM_ID()
        {
            return (String)Get_Value("VA007_RefUOM_ID");
        }
    }

}
