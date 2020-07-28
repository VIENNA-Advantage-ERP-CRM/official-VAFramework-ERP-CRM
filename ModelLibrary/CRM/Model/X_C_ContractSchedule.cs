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
    /** Generated Model for C_ContractSchedule
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ContractSchedule : PO
    {
        public X_C_ContractSchedule(Context ctx, int C_ContractSchedule_ID, Trx trxName)
            : base(ctx, C_ContractSchedule_ID, trxName)
        {
            /** if (C_ContractSchedule_ID == 0)
            {
            SetC_ContractSchedule_ID (0);
            SetPriceEntered (0.0);
            }
             */
        }
        public X_C_ContractSchedule(Ctx ctx, int C_ContractSchedule_ID, Trx trxName)
            : base(ctx, C_ContractSchedule_ID, trxName)
        {
            /** if (C_ContractSchedule_ID == 0)
            {
            SetC_ContractSchedule_ID (0);
            SetPriceEntered (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ContractSchedule(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ContractSchedule(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ContractSchedule(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_ContractSchedule()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27610885153293L;
        /** Last Updated Timestamp 2/9/2012 9:27:16 AM */
        public static long updatedMS = 1328759836504L;
        /** AD_Table_ID=1000257 */
        public static int Table_ID;
        // =1000257;

        /** TableName=C_ContractSchedule */
        public static String Table_Name = "C_ContractSchedule";

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
            StringBuilder sb = new StringBuilder("X_C_ContractSchedule[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set C_ContractSchedule_ID.
        @param C_ContractSchedule_ID C_ContractSchedule_ID */
        public void SetC_ContractSchedule_ID(int C_ContractSchedule_ID)
        {
            if (C_ContractSchedule_ID < 1) throw new ArgumentException("C_ContractSchedule_ID is mandatory.");
            Set_ValueNoCheck("C_ContractSchedule_ID", C_ContractSchedule_ID);
        }
        /** Get C_ContractSchedule_ID.
        @return C_ContractSchedule_ID */
        public int GetC_ContractSchedule_ID()
        {
            Object ii = Get_Value("C_ContractSchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set C_Contract_ID.
        @param C_Contract_ID C_Contract_ID */
        public void SetC_Contract_ID(int C_Contract_ID)
        {
            if (C_Contract_ID <= 0) Set_Value("C_Contract_ID", null);
            else
                Set_Value("C_Contract_ID", C_Contract_ID);
        }
        /** Get C_Contract_ID.
        @return C_Contract_ID */
        public int GetC_Contract_ID()
        {
            Object ii = Get_Value("C_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_Value("C_Invoice_ID", null);
            else
                Set_Value("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /// <summary>
        /// Set Surcharge Amount.
        /// </summary>
        /// <param name="SurchargeAmt">Surcharge Amount for a document</param>        
        public void SetSurchargeAmt(Decimal? SurchargeAmt)
        {
            Set_Value("SurchargeAmt", (Decimal?)SurchargeAmt);
        }
        /// <summary>
        /// Get Surcharge Amount.
        /// </summary>
        /// <returns>Surcharge Amount for a document</returns>
        public Decimal GetSurchargeAmt()
        {
            Object bd = Get_Value("SurchargeAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Complete .
        @param Complete  Complete  */
        public void SetComplete(String Complete)
        {
            if (Complete != null && Complete.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                Complete = Complete.Substring(0, 20);
            }
            Set_Value("Complete ", Complete);
        }
        /** Get Complete .
        @return Complete  */
        public String GetComplete()
        {
            return (String)Get_Value("Complete ");
        }
        /** Set Discount %.
        @param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount)
        {
            Set_Value("Discount", (Decimal?)Discount);
        }
        /** Get Discount %.
        @return Discount in percent */
        public Decimal GetDiscount()
        {
            Object bd = Get_Value("Discount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set End Date.
        @param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate)
        {
            Set_Value("EndDate", (DateTime?)EndDate);
        }
        /** Get End Date.
        @return Last effective date (inclusive) */
        public DateTime? GetEndDate()
        {
            return (DateTime?)Get_Value("EndDate");
        }
        /** Set From Date.
        @param FROMDATE From Date */
        public void SetFROMDATE(DateTime? FROMDATE)
        {
            Set_Value("FROMDATE", (DateTime?)FROMDATE);
        }
        /** Get From Date.
        @return From Date */
        public DateTime? GetFROMDATE()
        {
            return (DateTime?)Get_Value("FROMDATE");
        }
        /** Set Grand Total.
        @param GrandTotal Total amount of document */
        public void SetGrandTotal(Decimal? GrandTotal)
        {
            Set_Value("GrandTotal", (Decimal?)GrandTotal);
        }
        /** Get Grand Total.
        @return Total amount of document */
        public Decimal GetGrandTotal()
        {
            Object bd = Get_Value("GrandTotal");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set No Of Days.
        @param NoOfDays No Of Days */
        public void SetNoOfDays(Decimal? NoOfDays)
        {
            Set_Value("NoOfDays", (Decimal?)NoOfDays);
        }
        /** Get No Of Days.
        @return No Of Days */
        public Decimal GetNoOfDays()
        {
            Object bd = Get_Value("NoOfDays");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price.
        @param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered)
        {
            if (PriceEntered == null) throw new ArgumentException("PriceEntered is mandatory.");
            Set_Value("PriceEntered", (Decimal?)PriceEntered);
        }
        /** Get Price.
        @return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered()
        {
            Object bd = Get_Value("PriceEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Total Amount.
        @param TotalAmt Total Amount */
        public void SetTotalAmt(Decimal? TotalAmt)
        {
            Set_Value("TotalAmt", (Decimal?)TotalAmt);
        }
        /** Get Total Amount.
        @return Total Amount */
        public Decimal GetTotalAmt()
        {
            Object bd = Get_Value("TotalAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Units Delivered.
        @param UnitsDelivered Units Delivered */
        public void SetUnitsDelivered(Decimal? UnitsDelivered)
        {
            Set_Value("UnitsDelivered", (Decimal?)UnitsDelivered);
        }
        /** Get Units Delivered.
        @return Units Delivered */
        public Decimal GetUnitsDelivered()
        {
            Object bd = Get_Value("UnitsDelivered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
