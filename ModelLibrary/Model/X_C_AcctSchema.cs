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
    /** Generated Model for C_AcctSchema
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_AcctSchema : PO
    {
        public X_C_AcctSchema(Context ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            /** if (C_AcctSchema_ID == 0)
            {
            SetAutoPeriodControl (false);
            SetC_AcctSchema_ID (0);
            SetC_Currency_ID (0);
            SetCommitmentType (null);	// N
            SetCostingLevel (null);	// C
            SetCostingMethod (null);	// S
            SetGAAP (null);
            SetHasAlias (false);
            SetHasCombination (false);
            SetIsAccrual (true);	// Y
            SetIsAdjustCOGS (false);
            SetIsDiscountCorrectsTax (false);
            SetIsExplicitCostAdjustment (false);	// N
            SetIsPostServices (false);	// N
            SetIsTradeDiscountPosted (false);
            SetM_CostType_ID (0);
            SetName (null);
            SetSeparator (null);	// -
            SetTaxCorrectionType (null);	// N
            }
             */
        }
        public X_C_AcctSchema(Ctx ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            /** if (C_AcctSchema_ID == 0)
            {
            SetAutoPeriodControl (false);
            SetC_AcctSchema_ID (0);
            SetC_Currency_ID (0);
            SetCommitmentType (null);	// N
            SetCostingLevel (null);	// C
            SetCostingMethod (null);	// S
            SetGAAP (null);
            SetHasAlias (false);
            SetHasCombination (false);
            SetIsAccrual (true);	// Y
            SetIsAdjustCOGS (false);
            SetIsDiscountCorrectsTax (false);
            SetIsExplicitCostAdjustment (false);	// N
            SetIsPostServices (false);	// N
            SetIsTradeDiscountPosted (false);
            SetM_CostType_ID (0);
            SetName (null);
            SetSeparator (null);	// -
            SetTaxCorrectionType (null);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_AcctSchema()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721080414376L;
        /** Last Updated Timestamp 8/7/2015 7:14:58 PM */
        public static long updatedMS = 1438955097587L;
        /** AD_Table_ID=265 */
        public static int Table_ID;
        // =265;

        /** TableName=C_AcctSchema */
        public static String Table_Name = "C_AcctSchema";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
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
            StringBuilder sb = new StringBuilder("X_C_AcctSchema[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgOnly_ID AD_Reference_ID=322 */
        public static int AD_ORGONLY_ID_AD_Reference_ID = 322;
        /** Set Only Organization.
        @param AD_OrgOnly_ID Create posting entries only for this organization */
        public void SetAD_OrgOnly_ID(int AD_OrgOnly_ID)
        {
            if (AD_OrgOnly_ID <= 0) Set_Value("AD_OrgOnly_ID", null);
            else
                Set_Value("AD_OrgOnly_ID", AD_OrgOnly_ID);
        }
        /** Get Only Organization.
        @return Create posting entries only for this organization */
        public int GetAD_OrgOnly_ID()
        {
            Object ii = Get_Value("AD_OrgOnly_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Automatic Period Control.
        @param AutoPeriodControl If selected, the periods are automatically opened and closed */
        public void SetAutoPeriodControl(Boolean AutoPeriodControl)
        {
            Set_Value("AutoPeriodControl", AutoPeriodControl);
        }
        /** Get Automatic Period Control.
        @return If selected, the periods are automatically opened and closed */
        public Boolean IsAutoPeriodControl()
        {
            Object oo = Get_Value("AutoPeriodControl");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Type.
        @param C_ConversionType_ID Currency Type */
        //public void SetC_ConversionType_ID(int C_ConversionType_ID)
        //{
        //    if (C_ConversionType_ID <= 0) Set_ValueNoCheck("C_ConversionType_ID", null);
        //    else
        //        Set_ValueNoCheck("C_ConversionType_ID", C_ConversionType_ID);
        //}
        /** Get Currency Type.
        @return Currency Type */
        //public int GetC_ConversionType_ID()
        //{
        //    Object ii = Get_Value("C_ConversionType_ID");
        //    if (ii == null) return 0;
        //    return Convert.ToInt32(ii);
        //}
        /** Set Period.
        @param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID)
        {
            if (C_Period_ID <= 0) Set_ValueNoCheck("C_Period_ID", null);
            else
                Set_ValueNoCheck("C_Period_ID", C_Period_ID);
        }
        /** Get Period.
        @return Period of the Calendar */
        public int GetC_Period_ID()
        {
            Object ii = Get_Value("C_Period_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Close Period After.
        @param ClosePeriodAfter Close Period After */
        public void SetClosePeriodAfter(int ClosePeriodAfter)
        {
            Set_Value("ClosePeriodAfter", ClosePeriodAfter);
        }
        /** Get Close Period After.
        @return Close Period After */
        public int GetClosePeriodAfter()
        {
            Object ii = Get_Value("ClosePeriodAfter");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** CommitmentType AD_Reference_ID=359 */
        public static int COMMITMENTTYPE_AD_Reference_ID = 359;
        /** Commitment & Reservation = B */
        public static String COMMITMENTTYPE_CommitmentReservation = "B";
        /** Commitment only = C */
        public static String COMMITMENTTYPE_CommitmentOnly = "C";
        /** None = N */
        public static String COMMITMENTTYPE_None = "N";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCommitmentTypeValid(String test)
        {
            return test.Equals("B") || test.Equals("C") || test.Equals("N");
        }
        /** Set Commitment Type.
        @param CommitmentType Create Commitment and/or Reservations for Budget Control */
        public void SetCommitmentType(String CommitmentType)
        {
            if (CommitmentType == null) throw new ArgumentException("CommitmentType is mandatory");
            if (!IsCommitmentTypeValid(CommitmentType))
                throw new ArgumentException("CommitmentType Invalid value - " + CommitmentType + " - Reference_ID=359 - B - C - N");
            if (CommitmentType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CommitmentType = CommitmentType.Substring(0, 1);
            }
            Set_Value("CommitmentType", CommitmentType);
        }
        /** Get Commitment Type.
        @return Create Commitment and/or Reservations for Budget Control */
        public String GetCommitmentType()
        {
            return (String)Get_Value("CommitmentType");
        }


        /** CostingLevel AD_Reference_ID=355 */
        public static int COSTINGLEVEL_AD_Reference_ID = 355;/** Org + Batch = A */
        public static String COSTINGLEVEL_OrgPlusBatch = "A";/** Batch/Lot = B */
        public static String COSTINGLEVEL_BatchLot = "B";/** Client = C */
        public static String COSTINGLEVEL_Client = "C";/** Warehouse + Batch = D */
        public static String COSTINGLEVEL_WarehousePlusBatch = "D";/** Organization = O */
        public static String COSTINGLEVEL_Organization = "O";/** Warehouse = W */
        public static String COSTINGLEVEL_Warehouse = "W";
        /** Is test a valid value.
    @param test testvalue
    @returns true if valid **/
        public bool IsCostingLevelValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("O") || test.Equals("W");
        }
        /** Set Costing Level.
    @param CostingLevel The lowest level to accumulate Costing Information */
        public void SetCostingLevel(String CostingLevel)
        {
            if (!IsCostingLevelValid(CostingLevel))
                throw new ArgumentException("CostingLevel Invalid value - " + CostingLevel + " - Reference_ID=355 - A - B - C - D - O - W"); if (CostingLevel != null && CostingLevel.Length > 1) { log.Warning("Length > 1 - truncated"); CostingLevel = CostingLevel.Substring(0, 1); } Set_Value("CostingLevel", CostingLevel);
        }
        /** Get Costing Level.
    @return The lowest level to accumulate Costing Information */
        public String GetCostingLevel()
        {
            return (String)Get_Value("CostingLevel");
        }

        /** CostingMethod AD_Reference_ID=122 */
        public static int COSTINGMETHOD_AD_Reference_ID = 122;
        /** Average PO = A */
        public static String COSTINGMETHOD_AveragePO = "A";
        /** Fifo = F */
        public static String COSTINGMETHOD_Fifo = "F";
        /** Average Invoice = I */
        public static String COSTINGMETHOD_AverageInvoice = "I";
        /** Lifo = L */
        public static String COSTINGMETHOD_Lifo = "L";
        /** Standard Costing = S */
        public static String COSTINGMETHOD_StandardCosting = "S";
        /** User Defined = U */
        public static String COSTINGMETHOD_UserDefined = "U";
        /** Last Invoice = i */
        public static String COSTINGMETHOD_LastInvoice = "i";
        /** Last PO Price = p */
        public static String COSTINGMETHOD_LastPOPrice = "p";
        /** _ = x */
        public static String COSTINGMETHOD__ = "x";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCostingMethodValid(String test)
        {
            return test.Equals("A") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x");
        }
        /** Set Costing Method.
        @param CostingMethod Indicates how Costs will be calculated */
        public void SetCostingMethod(String CostingMethod)
        {
            if (CostingMethod == null) throw new ArgumentException("CostingMethod is mandatory");
            if (!IsCostingMethodValid(CostingMethod))
                throw new ArgumentException("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
            if (CostingMethod.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CostingMethod = CostingMethod.Substring(0, 1);
            }
            Set_Value("CostingMethod", CostingMethod);
        }
        /** Get Costing Method.
        @return Indicates how Costs will be calculated */
        public String GetCostingMethod()
        {
            return (String)Get_Value("CostingMethod");
        }

        /** Set Cost Element. @param M_CostElement_ID Product Cost Element */
        public void SetM_CostElement_ID(int M_CostElement_ID)
        {
            if (M_CostElement_ID <= 0) Set_Value("M_CostElement_ID", null);
            else
                Set_Value("M_CostElement_ID", M_CostElement_ID);
        }/** Get Cost Element. @return Product Cost Element */
        public int GetM_CostElement_ID() { Object ii = Get_Value("M_CostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

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
        /** Set Loc Account .
        @param FRPT_LocAcct_ID Loc Account  */
        public void SetFRPT_LocAcct_ID(int FRPT_LocAcct_ID)
        {
            if (FRPT_LocAcct_ID <= 0) Set_Value("FRPT_LocAcct_ID", null);
            else
                Set_Value("FRPT_LocAcct_ID", FRPT_LocAcct_ID);
        }
        /** Get Loc Account .
        @return Loc Account  */
        public int GetFRPT_LocAcct_ID()
        {
            Object ii = Get_Value("FRPT_LocAcct_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process Default Code.
        @param FRPT_Process Process Default Code */
        public void SetFRPT_Process(String FRPT_Process)
        {
            if (FRPT_Process != null && FRPT_Process.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                FRPT_Process = FRPT_Process.Substring(0, 10);
            }
            Set_Value("FRPT_Process", FRPT_Process);
        }
        /** Get Process Default Code.
        @return Process Default Code */
        public String GetFRPT_Process()
        {
            return (String)Get_Value("FRPT_Process");
        }

        /** GAAP AD_Reference_ID=123 */
        public static int GAAP_AD_Reference_ID = 123;
        /** German HGB = DE */
        public static String GAAP_GermanHGB = "DE";
        /** French Accounting Standard = FR */
        public static String GAAP_FrenchAccountingStandard = "FR";
        /** International GAAP = UN */
        public static String GAAP_InternationalGAAP = "UN";
        /** US GAAP = US */
        public static String GAAP_USGAAP = "US";
        /** Custom Accounting Rules = XX */
        public static String GAAP_CustomAccountingRules = "XX";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsGAAPValid(String test)
        {
            return test.Equals("DE") || test.Equals("FR") || test.Equals("UN") || test.Equals("US") || test.Equals("XX");
        }
        /** Set GAAP.
        @param GAAP Generally Accepted Accounting Principles */
        public void SetGAAP(String GAAP)
        {
            if (GAAP == null) throw new ArgumentException("GAAP is mandatory");
            if (!IsGAAPValid(GAAP))
                throw new ArgumentException("GAAP Invalid value - " + GAAP + " - Reference_ID=123 - DE - FR - UN - US - XX");
            if (GAAP.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                GAAP = GAAP.Substring(0, 2);
            }
            Set_Value("GAAP", GAAP);
        }
        /** Get GAAP.
        @return Generally Accepted Accounting Principles */
        public String GetGAAP()
        {
            return (String)Get_Value("GAAP");
        }
        /** Set Use Account Alias.
        @param HasAlias Ability to select (partial) account combinations by an Alias */
        public void SetHasAlias(Boolean HasAlias)
        {
            Set_Value("HasAlias", HasAlias);
        }
        /** Get Use Account Alias.
        @return Ability to select (partial) account combinations by an Alias */
        public Boolean IsHasAlias()
        {
            Object oo = Get_Value("HasAlias");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use Account Combination Control.
        @param HasCombination Combination of account elements are checked */
        public void SetHasCombination(Boolean HasCombination)
        {
            Set_Value("HasCombination", HasCombination);
        }
        /** Get Use Account Combination Control.
        @return Combination of account elements are checked */
        public Boolean IsHasCombination()
        {
            Object oo = Get_Value("HasCombination");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Accrual.
        @param IsAccrual Indicates if Accrual or Cash Based accounting will be used */
        public void SetIsAccrual(Boolean IsAccrual)
        {
            Set_Value("IsAccrual", IsAccrual);
        }
        /** Get Accrual.
        @return Indicates if Accrual or Cash Based accounting will be used */
        public Boolean IsAccrual()
        {
            Object oo = Get_Value("IsAccrual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Adjust COGS.
        @param IsAdjustCOGS Adjust Cost of Good Sold */
        public void SetIsAdjustCOGS(Boolean IsAdjustCOGS)
        {
            Set_Value("IsAdjustCOGS", IsAdjustCOGS);
        }
        /** Get Adjust COGS.
        @return Adjust Cost of Good Sold */
        public Boolean IsAdjustCOGS()
        {
            Object oo = Get_Value("IsAdjustCOGS");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Correct tax for Discounts/Charges.
        @param IsDiscountCorrectsTax Correct the tax for payment discount and charges */
        public void SetIsDiscountCorrectsTax(Boolean IsDiscountCorrectsTax)
        {
            Set_Value("IsDiscountCorrectsTax", IsDiscountCorrectsTax);
        }
        /** Get Correct tax for Discounts/Charges.
        @return Correct the tax for payment discount and charges */
        public Boolean IsDiscountCorrectsTax()
        {
            Object oo = Get_Value("IsDiscountCorrectsTax");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Explicit Cost Adjustment.
        @param IsExplicitCostAdjustment Post the cost adjustment explicitly */
        public void SetIsExplicitCostAdjustment(Boolean IsExplicitCostAdjustment)
        {
            Set_Value("IsExplicitCostAdjustment", IsExplicitCostAdjustment);
        }
        /** Get Explicit Cost Adjustment.
        @return Post the cost adjustment explicitly */
        public Boolean IsExplicitCostAdjustment()
        {
            Object oo = Get_Value("IsExplicitCostAdjustment");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Services Separately.
        @param IsPostServices Differentiate between Services and Product Receivable/Payables */
        public void SetIsPostServices(Boolean IsPostServices)
        {
            Set_Value("IsPostServices", IsPostServices);
        }
        /** Get Post Services Separately.
        @return Differentiate between Services and Product Receivable/Payables */
        public Boolean IsPostServices()
        {
            Object oo = Get_Value("IsPostServices");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Trade Discount.
        @param IsTradeDiscountPosted Generate postings for trade discounts */
        public void SetIsTradeDiscountPosted(Boolean IsTradeDiscountPosted)
        {
            Set_Value("IsTradeDiscountPosted", IsTradeDiscountPosted);
        }
        /** Get Post Trade Discount.
        @return Generate postings for trade discounts */
        public Boolean IsTradeDiscountPosted()
        {
            Object oo = Get_Value("IsTradeDiscountPosted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Cost Type.
        @param M_CostType_ID Type of Cost (e.g. Current, Plan, Future) */
        public void SetM_CostType_ID(int M_CostType_ID)
        {
            if (M_CostType_ID < 1) throw new ArgumentException("M_CostType_ID is mandatory.");
            Set_Value("M_CostType_ID", M_CostType_ID);
        }
        /** Get Cost Type.
        @return Type of Cost (e.g. Current, Plan, Future) */
        public int GetM_CostType_ID()
        {
            Object ii = Get_Value("M_CostType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set NotPostPOVariance.
        @param NotPostPOVariance NotPostPOVariance */
        public void SetNotPostPOVariance(Boolean NotPostPOVariance)
        {
            Set_Value("NotPostPOVariance", NotPostPOVariance);
        }
        /** Get NotPostPOVariance.
        @return NotPostPOVariance */
        public Boolean IsNotPostPOVariance()
        {
            Object oo = Get_Value("NotPostPOVariance");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Open Period Before.
        @param OpenPeriodBefore Open Period Before */
        public void SetOpenPeriodBefore(int OpenPeriodBefore)
        {
            Set_Value("OpenPeriodBefore", OpenPeriodBefore);
        }
        /** Get Open Period Before.
        @return Open Period Before */
        public int GetOpenPeriodBefore()
        {
            Object ii = Get_Value("OpenPeriodBefore");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PeriodControl AD_Reference_ID=1000113 */
        public static int PERIODCONTROL_AD_Reference_ID = 1000113;
        /** Automatic = A */
        public static String PERIODCONTROL_Automatic = "A";
        /** Manual = M */
        public static String PERIODCONTROL_Manual = "M";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPeriodControlValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("M");
        }
        /** Set Period Control.
        @param PeriodControl Period Control */
        public void SetPeriodControl(String PeriodControl)
        {
            if (!IsPeriodControlValid(PeriodControl))
                throw new ArgumentException("PeriodControl Invalid value - " + PeriodControl + " - Reference_ID=1000113 - A - M");
            if (PeriodControl != null && PeriodControl.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PeriodControl = PeriodControl.Substring(0, 1);
            }
            Set_Value("PeriodControl", PeriodControl);
        }
        /** Get Period Control.
        @return Period Control */
        public String GetPeriodControl()
        {
            return (String)Get_Value("PeriodControl");
        }
        /** Set Future Days.
        @param Period_OpenFuture Number of days to be able to post to a future date (based on system date) */
        public void SetPeriod_OpenFuture(int Period_OpenFuture)
        {
            Set_Value("Period_OpenFuture", Period_OpenFuture);
        }
        /** Get Future Days.
        @return Number of days to be able to post to a future date (based on system date) */
        public int GetPeriod_OpenFuture()
        {
            Object ii = Get_Value("Period_OpenFuture");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set History Days.
        @param Period_OpenHistory Number of days to be able to post in the past (based on system date) */
        public void SetPeriod_OpenHistory(int Period_OpenHistory)
        {
            Set_Value("Period_OpenHistory", Period_OpenHistory);
        }
        /** Get History Days.
        @return Number of days to be able to post in the past (based on system date) */
        public int GetPeriod_OpenHistory()
        {
            Object ii = Get_Value("Period_OpenHistory");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Element Separator.
        @param Separator Element Separator */
        public void SetSeparator(String Separator)
        {
            if (Separator == null) throw new ArgumentException("Separator is mandatory.");
            if (Separator.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Separator = Separator.Substring(0, 1);
            }
            Set_Value("Separator", Separator);
        }
        /** Get Element Separator.
        @return Element Separator */
        public String GetSeparator()
        {
            return (String)Get_Value("Separator");
        }

        /** TaxCorrectionType AD_Reference_ID=392 */
        public static int TAXCORRECTIONTYPE_AD_Reference_ID = 392;
        /** Write-off and Discount = B */
        public static String TAXCORRECTIONTYPE_Write_OffAndDiscount = "B";
        /** Discount only = D */
        public static String TAXCORRECTIONTYPE_DiscountOnly = "D";
        /** None = N */
        public static String TAXCORRECTIONTYPE_None = "N";
        /** Write-off only = W */
        public static String TAXCORRECTIONTYPE_Write_OffOnly = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTaxCorrectionTypeValid(String test)
        {
            return test.Equals("B") || test.Equals("D") || test.Equals("N") || test.Equals("W");
        }
        /** Set Tax Correction.
        @param TaxCorrectionType Type of Tax Correction */
        public void SetTaxCorrectionType(String TaxCorrectionType)
        {
            if (TaxCorrectionType == null) throw new ArgumentException("TaxCorrectionType is mandatory");
            if (!IsTaxCorrectionTypeValid(TaxCorrectionType))
                throw new ArgumentException("TaxCorrectionType Invalid value - " + TaxCorrectionType + " - Reference_ID=392 - B - D - N - W");
            if (TaxCorrectionType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                TaxCorrectionType = TaxCorrectionType.Substring(0, 1);
            }
            Set_Value("TaxCorrectionType", TaxCorrectionType);
        }
        /** Get Tax Correction.
        @return Type of Tax Correction */
        public String GetTaxCorrectionType()
        {
            return (String)Get_Value("TaxCorrectionType");
        }

        /** VACTWZ_PeriodEndMonth AD_Reference_ID=1000135 */
        public static int VACTWZ_PERIODENDMONTH_AD_Reference_ID = 1000135;
        /** January = 01 */
        public static String VACTWZ_PERIODENDMONTH_January = "01";
        /** February = 02 */
        public static String VACTWZ_PERIODENDMONTH_February = "02";
        /** March = 03 */
        public static String VACTWZ_PERIODENDMONTH_March = "03";
        /** April = 04 */
        public static String VACTWZ_PERIODENDMONTH_April = "04";
        /** May = 05 */
        public static String VACTWZ_PERIODENDMONTH_May = "05";
        /** June = 06 */
        public static String VACTWZ_PERIODENDMONTH_June = "06";
        /** July = 07 */
        public static String VACTWZ_PERIODENDMONTH_July = "07";
        /** August = 08 */
        public static String VACTWZ_PERIODENDMONTH_August = "08";
        /** September = 09 */
        public static String VACTWZ_PERIODENDMONTH_September = "09";
        /** October = 10 */
        public static String VACTWZ_PERIODENDMONTH_October = "10";
        /** November = 11 */
        public static String VACTWZ_PERIODENDMONTH_November = "11";
        /** December = 12 */
        public static String VACTWZ_PERIODENDMONTH_December = "12";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVACTWZ_PeriodEndMonthValid(String test)
        {
            return test == null || test.Equals("01") || test.Equals("02") || test.Equals("03") || test.Equals("04") || test.Equals("05") || test.Equals("06") || test.Equals("07") || test.Equals("08") || test.Equals("09") || test.Equals("10") || test.Equals("11") || test.Equals("12");
        }
        /** Set Period End Month.
        @param VACTWZ_PeriodEndMonth Period End Month */
        public void SetVACTWZ_PeriodEndMonth(String VACTWZ_PeriodEndMonth)
        {
            if (!IsVACTWZ_PeriodEndMonthValid(VACTWZ_PeriodEndMonth))
                throw new ArgumentException("VACTWZ_PeriodEndMonth Invalid value - " + VACTWZ_PeriodEndMonth + " - Reference_ID=1000135 - 01 - 02 - 03 - 04 - 05 - 06 - 07 - 08 - 09 - 10 - 11 - 12");
            if (VACTWZ_PeriodEndMonth != null && VACTWZ_PeriodEndMonth.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VACTWZ_PeriodEndMonth = VACTWZ_PeriodEndMonth.Substring(0, 2);
            }
            Set_Value("VACTWZ_PeriodEndMonth", VACTWZ_PeriodEndMonth);
        }
        /** Get Period End Month.
        @return Period End Month */
        public String GetVACTWZ_PeriodEndMonth()
        {
            return (String)Get_Value("VACTWZ_PeriodEndMonth");
        }

        /** VACTWZ_PeriodEndsAtDay AD_Reference_ID=1000136 */
        public static int VACTWZ_PERIODENDSATDAY_AD_Reference_ID = 1000136;
        /** 01 = 01 */
        public static String VACTWZ_PERIODENDSATDAY_01 = "01";
        /** 02 = 02 */
        public static String VACTWZ_PERIODENDSATDAY_02 = "02";
        /** 03 = 03 */
        public static String VACTWZ_PERIODENDSATDAY_03 = "03";
        /** 04 = 04 */
        public static String VACTWZ_PERIODENDSATDAY_04 = "04";
        /** 05 = 05 */
        public static String VACTWZ_PERIODENDSATDAY_05 = "05";
        /** 06 = 06 */
        public static String VACTWZ_PERIODENDSATDAY_06 = "06";
        /** 07 = 07 */
        public static String VACTWZ_PERIODENDSATDAY_07 = "07";
        /** 08 = 08 */
        public static String VACTWZ_PERIODENDSATDAY_08 = "08";
        /** 09 = 09 */
        public static String VACTWZ_PERIODENDSATDAY_09 = "09";
        /** 10 = 10 */
        public static String VACTWZ_PERIODENDSATDAY_10 = "10";
        /** 11 = 11 */
        public static String VACTWZ_PERIODENDSATDAY_11 = "11";
        /** 12 = 12 */
        public static String VACTWZ_PERIODENDSATDAY_12 = "12";
        /** 13 = 13 */
        public static String VACTWZ_PERIODENDSATDAY_13 = "13";
        /** 14 = 14 */
        public static String VACTWZ_PERIODENDSATDAY_14 = "14";
        /** 15 = 15 */
        public static String VACTWZ_PERIODENDSATDAY_15 = "15";
        /** 16 = 16 */
        public static String VACTWZ_PERIODENDSATDAY_16 = "16";
        /** 17 = 17 */
        public static String VACTWZ_PERIODENDSATDAY_17 = "17";
        /** 18 = 18 */
        public static String VACTWZ_PERIODENDSATDAY_18 = "18";
        /** 19 = 19 */
        public static String VACTWZ_PERIODENDSATDAY_19 = "19";
        /** 20 = 20 */
        public static String VACTWZ_PERIODENDSATDAY_20 = "20";
        /** 21 = 21 */
        public static String VACTWZ_PERIODENDSATDAY_21 = "21";
        /** 22 = 22 */
        public static String VACTWZ_PERIODENDSATDAY_22 = "22";
        /** 23 = 23 */
        public static String VACTWZ_PERIODENDSATDAY_23 = "23";
        /** 24 = 24 */
        public static String VACTWZ_PERIODENDSATDAY_24 = "24";
        /** 25 = 25 */
        public static String VACTWZ_PERIODENDSATDAY_25 = "25";
        /** 26 = 26 */
        public static String VACTWZ_PERIODENDSATDAY_26 = "26";
        /** 24 = 27 */
        public static String VACTWZ_PERIODENDSATDAY_27 = "27";
        /** 28 = 28 */
        public static String VACTWZ_PERIODENDSATDAY_28 = "28";
        /** 29 = 29 */
        public static String VACTWZ_PERIODENDSATDAY_29 = "29";
        /** 30 = 30 */
        public static String VACTWZ_PERIODENDSATDAY_30 = "30";
        /** 31 = 31 */
        public static String VACTWZ_PERIODENDSATDAY_31 = "31";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVACTWZ_PeriodEndsAtDayValid(String test)
        {
            return test == null || test.Equals("01") || test.Equals("02") || test.Equals("03") || test.Equals("04") || test.Equals("05") || test.Equals("06") || test.Equals("07") || test.Equals("08") || test.Equals("09") || test.Equals("10") || test.Equals("11") || test.Equals("12") || test.Equals("13") || test.Equals("14") || test.Equals("15") || test.Equals("16") || test.Equals("17") || test.Equals("18") || test.Equals("19") || test.Equals("20") || test.Equals("21") || test.Equals("22") || test.Equals("23") || test.Equals("24") || test.Equals("25") || test.Equals("26") || test.Equals("27") || test.Equals("28") || test.Equals("29") || test.Equals("30") || test.Equals("31");
        }
        /** Set Period Ends At Day.
        @param VACTWZ_PeriodEndsAtDay Period Ends At Day */
        public void SetVACTWZ_PeriodEndsAtDay(String VACTWZ_PeriodEndsAtDay)
        {
            if (!IsVACTWZ_PeriodEndsAtDayValid(VACTWZ_PeriodEndsAtDay))
                throw new ArgumentException("VACTWZ_PeriodEndsAtDay Invalid value - " + VACTWZ_PeriodEndsAtDay + " - Reference_ID=1000136 - 01 - 02 - 03 - 04 - 05 - 06 - 07 - 08 - 09 - 10 - 11 - 12 - 13 - 14 - 15 - 16 - 17 - 18 - 19 - 20 - 21 - 22 - 23 - 24 - 25 - 26 - 27 - 28 - 29 - 30 - 31");
            if (VACTWZ_PeriodEndsAtDay != null && VACTWZ_PeriodEndsAtDay.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VACTWZ_PeriodEndsAtDay = VACTWZ_PeriodEndsAtDay.Substring(0, 2);
            }
            Set_Value("VACTWZ_PeriodEndsAtDay", VACTWZ_PeriodEndsAtDay);
        }
        /** Get Period Ends At Day.
        @return Period Ends At Day */
        public String GetVACTWZ_PeriodEndsAtDay()
        {
            return (String)Get_Value("VACTWZ_PeriodEndsAtDay");
        }

        /** VACTWZ_PeriodStartFromDay AD_Reference_ID=1000136 */
        public static int VACTWZ_PERIODSTARTFROMDAY_AD_Reference_ID = 1000136;
        /** 01 = 01 */
        public static String VACTWZ_PERIODSTARTFROMDAY_01 = "01";
        /** 02 = 02 */
        public static String VACTWZ_PERIODSTARTFROMDAY_02 = "02";
        /** 03 = 03 */
        public static String VACTWZ_PERIODSTARTFROMDAY_03 = "03";
        /** 04 = 04 */
        public static String VACTWZ_PERIODSTARTFROMDAY_04 = "04";
        /** 05 = 05 */
        public static String VACTWZ_PERIODSTARTFROMDAY_05 = "05";
        /** 06 = 06 */
        public static String VACTWZ_PERIODSTARTFROMDAY_06 = "06";
        /** 07 = 07 */
        public static String VACTWZ_PERIODSTARTFROMDAY_07 = "07";
        /** 08 = 08 */
        public static String VACTWZ_PERIODSTARTFROMDAY_08 = "08";
        /** 09 = 09 */
        public static String VACTWZ_PERIODSTARTFROMDAY_09 = "09";
        /** 10 = 10 */
        public static String VACTWZ_PERIODSTARTFROMDAY_10 = "10";
        /** 11 = 11 */
        public static String VACTWZ_PERIODSTARTFROMDAY_11 = "11";
        /** 12 = 12 */
        public static String VACTWZ_PERIODSTARTFROMDAY_12 = "12";
        /** 13 = 13 */
        public static String VACTWZ_PERIODSTARTFROMDAY_13 = "13";
        /** 14 = 14 */
        public static String VACTWZ_PERIODSTARTFROMDAY_14 = "14";
        /** 15 = 15 */
        public static String VACTWZ_PERIODSTARTFROMDAY_15 = "15";
        /** 16 = 16 */
        public static String VACTWZ_PERIODSTARTFROMDAY_16 = "16";
        /** 17 = 17 */
        public static String VACTWZ_PERIODSTARTFROMDAY_17 = "17";
        /** 18 = 18 */
        public static String VACTWZ_PERIODSTARTFROMDAY_18 = "18";
        /** 19 = 19 */
        public static String VACTWZ_PERIODSTARTFROMDAY_19 = "19";
        /** 20 = 20 */
        public static String VACTWZ_PERIODSTARTFROMDAY_20 = "20";
        /** 21 = 21 */
        public static String VACTWZ_PERIODSTARTFROMDAY_21 = "21";
        /** 22 = 22 */
        public static String VACTWZ_PERIODSTARTFROMDAY_22 = "22";
        /** 23 = 23 */
        public static String VACTWZ_PERIODSTARTFROMDAY_23 = "23";
        /** 24 = 24 */
        public static String VACTWZ_PERIODSTARTFROMDAY_24 = "24";
        /** 25 = 25 */
        public static String VACTWZ_PERIODSTARTFROMDAY_25 = "25";
        /** 26 = 26 */
        public static String VACTWZ_PERIODSTARTFROMDAY_26 = "26";
        /** 24 = 27 */
        public static String VACTWZ_PERIODSTARTFROMDAY_27 = "27";
        /** 28 = 28 */
        public static String VACTWZ_PERIODSTARTFROMDAY_28 = "28";
        /** 29 = 29 */
        public static String VACTWZ_PERIODSTARTFROMDAY_29 = "29";
        /** 30 = 30 */
        public static String VACTWZ_PERIODSTARTFROMDAY_30 = "30";
        /** 31 = 31 */
        public static String VACTWZ_PERIODSTARTFROMDAY_31 = "31";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVACTWZ_PeriodStartFromDayValid(String test)
        {
            return test == null || test.Equals("01") || test.Equals("02") || test.Equals("03") || test.Equals("04") || test.Equals("05") || test.Equals("06") || test.Equals("07") || test.Equals("08") || test.Equals("09") || test.Equals("10") || test.Equals("11") || test.Equals("12") || test.Equals("13") || test.Equals("14") || test.Equals("15") || test.Equals("16") || test.Equals("17") || test.Equals("18") || test.Equals("19") || test.Equals("20") || test.Equals("21") || test.Equals("22") || test.Equals("23") || test.Equals("24") || test.Equals("25") || test.Equals("26") || test.Equals("27") || test.Equals("28") || test.Equals("29") || test.Equals("30") || test.Equals("31");
        }
        /** Set Period Start From Day.
        @param VACTWZ_PeriodStartFromDay Period Start From Day */
        public void SetVACTWZ_PeriodStartFromDay(String VACTWZ_PeriodStartFromDay)
        {
            if (!IsVACTWZ_PeriodStartFromDayValid(VACTWZ_PeriodStartFromDay))
                throw new ArgumentException("VACTWZ_PeriodStartFromDay Invalid value - " + VACTWZ_PeriodStartFromDay + " - Reference_ID=1000136 - 01 - 02 - 03 - 04 - 05 - 06 - 07 - 08 - 09 - 10 - 11 - 12 - 13 - 14 - 15 - 16 - 17 - 18 - 19 - 20 - 21 - 22 - 23 - 24 - 25 - 26 - 27 - 28 - 29 - 30 - 31");
            if (VACTWZ_PeriodStartFromDay != null && VACTWZ_PeriodStartFromDay.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VACTWZ_PeriodStartFromDay = VACTWZ_PeriodStartFromDay.Substring(0, 2);
            }
            Set_Value("VACTWZ_PeriodStartFromDay", VACTWZ_PeriodStartFromDay);
        }
        /** Get Period Start From Day.
        @return Period Start From Day */
        public String GetVACTWZ_PeriodStartFromDay()
        {
            return (String)Get_Value("VACTWZ_PeriodStartFromDay");
        }

        /** VACTWZ_PeriodStartFromMonth AD_Reference_ID=1000135 */
        public static int VACTWZ_PERIODSTARTFROMMONTH_AD_Reference_ID = 1000135;
        /** January = 01 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_January = "01";
        /** February = 02 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_February = "02";
        /** March = 03 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_March = "03";
        /** April = 04 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_April = "04";
        /** May = 05 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_May = "05";
        /** June = 06 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_June = "06";
        /** July = 07 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_July = "07";
        /** August = 08 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_August = "08";
        /** September = 09 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_September = "09";
        /** October = 10 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_October = "10";
        /** November = 11 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_November = "11";
        /** December = 12 */
        public static String VACTWZ_PERIODSTARTFROMMONTH_December = "12";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVACTWZ_PeriodStartFromMonthValid(String test)
        {
            return test == null || test.Equals("01") || test.Equals("02") || test.Equals("03") || test.Equals("04") || test.Equals("05") || test.Equals("06") || test.Equals("07") || test.Equals("08") || test.Equals("09") || test.Equals("10") || test.Equals("11") || test.Equals("12");
        }
        /** Set Period Start From Month.
        @param VACTWZ_PeriodStartFromMonth Period Start From Month */
        public void SetVACTWZ_PeriodStartFromMonth(String VACTWZ_PeriodStartFromMonth)
        {
            if (!IsVACTWZ_PeriodStartFromMonthValid(VACTWZ_PeriodStartFromMonth))
                throw new ArgumentException("VACTWZ_PeriodStartFromMonth Invalid value - " + VACTWZ_PeriodStartFromMonth + " - Reference_ID=1000135 - 01 - 02 - 03 - 04 - 05 - 06 - 07 - 08 - 09 - 10 - 11 - 12");
            if (VACTWZ_PeriodStartFromMonth != null && VACTWZ_PeriodStartFromMonth.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VACTWZ_PeriodStartFromMonth = VACTWZ_PeriodStartFromMonth.Substring(0, 2);
            }
            Set_Value("VACTWZ_PeriodStartFromMonth", VACTWZ_PeriodStartFromMonth);
        }
        /** Get Period Start From Month.
        @return Period Start From Month */
        public String GetVACTWZ_PeriodStartFromMonth()
        {
            return (String)Get_Value("VACTWZ_PeriodStartFromMonth");
        }

        /** Set Not Post Inter Company.@param FRPT_IsNotPostInterCompany Not Post Inter Company */
        public void SetFRPT_IsNotPostInterCompany(Boolean FRPT_IsNotPostInterCompany) { Set_Value("FRPT_IsNotPostInterCompany", FRPT_IsNotPostInterCompany); }
        /** Get Not Post Inter Company.@return Not Post Inter Company */
        public Boolean IsFRPT_IsNotPostInterCompany() { Object oo = Get_Value("FRPT_IsNotPostInterCompany"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
    }

}
