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
    /** Generated Model for C_Charge
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Charge : PO
    {
        public X_C_Charge(Context ctx, int C_Charge_ID, Trx trxName)
            : base(ctx, C_Charge_ID, trxName)
        {
            /** if (C_Charge_ID == 0)
            {
            SetC_Charge_ID (0);
            SetC_TaxCategory_ID (0);
            SetChargeAmt (0.0);
            SetIsSameCurrency (false);
            SetIsSameTax (false);
            SetIsTaxIncluded (false);	// N
            SetName (null);
            }
             */
        }
        public X_C_Charge(Ctx ctx, int C_Charge_ID, Trx trxName)
            : base(ctx, C_Charge_ID, trxName)
        {
            /** if (C_Charge_ID == 0)
            {
            SetC_Charge_ID (0);
            SetC_TaxCategory_ID (0);
            SetChargeAmt (0.0);
            SetIsSameCurrency (false);
            SetIsSameTax (false);
            SetIsTaxIncluded (false);	// N
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Charge(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Charge(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Charge(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_Charge()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721596364918L;
        /** Last Updated Timestamp 8/13/2015 6:34:08 PM */
        public static long updatedMS = 1439471048129L;
        /** AD_Table_ID=313 */
        public static int Table_ID;
        // =313;

        /** TableName=C_Charge */
        public static String Table_Name = "C_Charge";

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
            StringBuilder sb = new StringBuilder("X_C_Charge[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID < 1) throw new ArgumentException("C_Charge_ID is mandatory.");
            Set_ValueNoCheck("C_Charge_ID", C_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetC_Charge_ID()
        {
            Object ii = Get_Value("C_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Category.
        @param C_TaxCategory_ID Tax Category */
        public void SetC_TaxCategory_ID(int C_TaxCategory_ID)
        {
            if (C_TaxCategory_ID < 1) throw new ArgumentException("C_TaxCategory_ID is mandatory.");
            Set_Value("C_TaxCategory_ID", C_TaxCategory_ID);
        }
        /** Get Tax Category.
        @return Tax Category */
        public int GetC_TaxCategory_ID()
        {
            Object ii = Get_Value("C_TaxCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge amount.
        @param ChargeAmt Charge Amount */
        public void SetChargeAmt(Decimal? ChargeAmt)
        {
            if (ChargeAmt == null) throw new ArgumentException("ChargeAmt is mandatory.");
            Set_Value("ChargeAmt", (Decimal?)ChargeAmt);
        }
        /** Get Charge amount.
        @return Charge Amount */
        public Decimal GetChargeAmt()
        {
            Object bd = Get_Value("ChargeAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** DTD001_ChargeType AD_Reference_ID=1000312 */
        public static int DTD001_CHARGETYPE_AD_Reference_ID = 1000312;
        /** AVL = AVL */
        public static String DTD001_CHARGETYPE_AVL = "AVL";
        /** Indemnity = IDM */
        public static String DTD001_CHARGETYPE_Indemnity = "IDM";
        /** Inventory Expense = INV */
        public static String DTD001_CHARGETYPE_InventoryExpense = "INV";
        /** Loan = LON */
        public static String DTD001_CHARGETYPE_Loan = "LON";
        /** Other = OTH */
        public static String DTD001_CHARGETYPE_Other = "OTH";
        /** Ticket = TKT */
        public static String DTD001_CHARGETYPE_Ticket = "TKT";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDTD001_ChargeTypeValid(String test)
        {
            return test == null || test.Equals("AVL") || test.Equals("IDM") || test.Equals("INV") || test.Equals("LON") || test.Equals("OTH") || test.Equals("TKT");
        }
        /** Set Charge Type.
        @param DTD001_ChargeType Charge Type */
        public void SetDTD001_ChargeType(String DTD001_ChargeType)
        {
            if (!IsDTD001_ChargeTypeValid(DTD001_ChargeType))
                throw new ArgumentException("DTD001_ChargeType Invalid value - " + DTD001_ChargeType + " - Reference_ID=1000312 - AVL - IDM - INV - LON - OTH - TKT");
            if (DTD001_ChargeType != null && DTD001_ChargeType.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                DTD001_ChargeType = DTD001_ChargeType.Substring(0, 3);
            }
            Set_Value("DTD001_ChargeType", DTD001_ChargeType);
        }
        /** Get Charge Type.
        @return Charge Type */
        public String GetDTD001_ChargeType()
        {
            return (String)Get_Value("DTD001_ChargeType");
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
        /** Set Advance Charge.
        @param IsAdvanceCharge Advance Charge */
        public void SetIsAdvanceCharge(Boolean IsAdvanceCharge)
        {
            Set_Value("IsAdvanceCharge", IsAdvanceCharge);
        }
        /** Get Advance Charge.
        @return Advance Charge */
        public Boolean IsAdvanceCharge()
        {
            Object oo = Get_Value("IsAdvanceCharge");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Same Currency.
        @param IsSameCurrency Same Currency */
        public void SetIsSameCurrency(Boolean IsSameCurrency)
        {
            Set_Value("IsSameCurrency", IsSameCurrency);
        }
        /** Get Same Currency.
        @return Same Currency */
        public Boolean IsSameCurrency()
        {
            Object oo = Get_Value("IsSameCurrency");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Same Tax.
        @param IsSameTax Use the same tax as the main transaction */
        public void SetIsSameTax(Boolean IsSameTax)
        {
            Set_Value("IsSameTax", IsSameTax);
        }
        /** Get Same Tax.
        @return Use the same tax as the main transaction */
        public Boolean IsSameTax()
        {
            Object oo = Get_Value("IsSameTax");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsShippingCharge.
        @param IsShippingCharge IsShippingCharge */
        public void SetIsShippingCharge(Boolean IsShippingCharge)
        {
            Set_Value("IsShippingCharge", IsShippingCharge);
        }
        /** Get IsShippingCharge.
        @return IsShippingCharge */
        public Boolean IsShippingCharge()
        {
            Object oo = Get_Value("IsShippingCharge");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Price includes Tax.
        @param IsTaxIncluded Tax is included in the price  */
        public void SetIsTaxIncluded(Boolean IsTaxIncluded)
        {
            Set_Value("IsTaxIncluded", IsTaxIncluded);
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

        /** Set Withholding Category.
@param C_WithholdingCategory_ID This field represents the withholding category linked with respective withholding tax. */
        public void SetC_WithholdingCategory_ID(int C_WithholdingCategory_ID)
        {
            if (C_WithholdingCategory_ID <= 0) Set_Value("C_WithholdingCategory_ID", null);
            else
                Set_Value("C_WithholdingCategory_ID", C_WithholdingCategory_ID);
        }/** Get Withholding Category.
@return This field represents the withholding category linked with respective withholding tax. */
        public int GetC_WithholdingCategory_ID() { Object ii = Get_Value("C_WithholdingCategory_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }


    }

}
