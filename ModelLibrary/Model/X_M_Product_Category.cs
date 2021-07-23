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
    /** Generated Model for M_Product_Category
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Product_Category : PO
    {
        public X_M_Product_Category(Context ctx, int M_Product_Category_ID, Trx trxName)
            : base(ctx, M_Product_Category_ID, trxName)
        {
            /** if (M_Product_Category_ID == 0)
            {
            SetIsDefault (false);
            SetIsPurchasedToOrder (false);	// N
            SetIsSelfService (true);	// Y
            SetMMPolicy (null);	// F
            SetM_Product_Category_ID (0);
            SetName (null);
            SetPlannedMargin (0.0);
            SetValue (null);
            }
             */
        }
        public X_M_Product_Category(Ctx ctx, int M_Product_Category_ID, Trx trxName)
            : base(ctx, M_Product_Category_ID, trxName)
        {
            /** if (M_Product_Category_ID == 0)
            {
            SetIsDefault (false);
            SetIsPurchasedToOrder (false);	// N
            SetIsSelfService (true);	// Y
            SetMMPolicy (null);	// F
            SetM_Product_Category_ID (0);
            SetName (null);
            SetPlannedMargin (0.0);
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product_Category(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product_Category(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product_Category(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Product_Category()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721931421699L;
        /** Last Updated Timestamp 8/17/2015 3:38:25 PM */
        public static long updatedMS = 1439806104910L;
        /** AD_Table_ID=209 */
        public static int Table_ID;
        // =209;

        /** TableName=M_Product_Category */
        public static String Table_Name = "M_Product_Category";

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
            StringBuilder sb = new StringBuilder("X_M_Product_Category[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Image.
        @param AD_Image_ID Image or Icon */
        public void SetAD_Image_ID(int AD_Image_ID)
        {
            if (AD_Image_ID <= 0) Set_Value("AD_Image_ID", null);
            else
                Set_Value("AD_Image_ID", AD_Image_ID);
        }
        /** Get Image.
        @return Image or Icon */
        public int GetAD_Image_ID()
        {
            Object ii = Get_Value("AD_Image_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Color.
        @param AD_PrintColor_ID Color used for printing and display */
        public void SetAD_PrintColor_ID(int AD_PrintColor_ID)
        {
            if (AD_PrintColor_ID <= 0) Set_Value("AD_PrintColor_ID", null);
            else
                Set_Value("AD_PrintColor_ID", AD_PrintColor_ID);
        }
        /** Get Print Color.
        @return Color used for printing and display */
        public int GetAD_PrintColor_ID()
        {
            Object ii = Get_Value("AD_PrintColor_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Asset Group.
        @param A_Asset_Group_ID Group of Assets */
        public void SetA_Asset_Group_ID(int A_Asset_Group_ID)
        {
            if (A_Asset_Group_ID <= 0) Set_Value("A_Asset_Group_ID", null);
            else
                Set_Value("A_Asset_Group_ID", A_Asset_Group_ID);
        }
        /** Get Asset Group.
        @return Group of Assets */
        public int GetA_Asset_Group_ID()
        {
            Object ii = Get_Value("A_Asset_Group_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Category.
        @param C_TaxCategory_ID Tax Category */
        public void SetC_TaxCategory_ID(int C_TaxCategory_ID)
        {
            if (C_TaxCategory_ID <= 0) Set_Value("C_TaxCategory_ID", null);
            else
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
        /** Set Consumable.
        @param DTD001_IsConsumable Consumable */
        public void SetDTD001_IsConsumable(Boolean DTD001_IsConsumable)
        {
            Set_Value("DTD001_IsConsumable", DTD001_IsConsumable);
        }
        /** Get Consumable.
        @return Consumable */
        public Boolean IsDTD001_IsConsumable()
        {
            Object oo = Get_Value("DTD001_IsConsumable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Process.
        @param FRPT_Process Process */
        public void SetFRPT_Process(String FRPT_Process)
        {
            if (FRPT_Process != null && FRPT_Process.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                FRPT_Process = FRPT_Process.Substring(0, 30);
            }
            Set_Value("FRPT_Process", FRPT_Process);
        }
        /** Get Process.
        @return Process */
        public String GetFRPT_Process()
        {
            return (String)Get_Value("FRPT_Process");
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
        /** Set Purchased To Order.
        @param IsPurchasedToOrder Products that are usually not kept in stock, but are purchased whenever there is a demand */
        public void SetIsPurchasedToOrder(Boolean IsPurchasedToOrder)
        {
            Set_Value("IsPurchasedToOrder", IsPurchasedToOrder);
        }
        /** Get Purchased To Order.
        @return Products that are usually not kept in stock, but are purchased whenever there is a demand */
        public Boolean IsPurchasedToOrder()
        {
            Object oo = Get_Value("IsPurchasedToOrder");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Self-Service.
        @param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService)
        {
            Set_Value("IsSelfService", IsSelfService);
        }
        /** Get Self-Service.
        @return This is a Self-Service entry or this entry can be changed via Self-Service */
        public Boolean IsSelfService()
        {
            Object oo = Get_Value("IsSelfService");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** MMPolicy AD_Reference_ID=335 */
        public static int MMPOLICY_AD_Reference_ID = 335;
        /** FiFo = F */
        public static String MMPOLICY_FiFo = "F";
        /** LiFo = L */
        public static String MMPOLICY_LiFo = "L";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMMPolicyValid(String test)
        {
            return test.Equals("F") || test.Equals("L");
        }
        /** Set Material Policy.
        @param MMPolicy Material Movement Policy */
        public void SetMMPolicy(String MMPolicy)
        {
            if (MMPolicy == null) throw new ArgumentException("MMPolicy is mandatory");
            if (!IsMMPolicyValid(MMPolicy))
                throw new ArgumentException("MMPolicy Invalid value - " + MMPolicy + " - Reference_ID=335 - F - L");
            if (MMPolicy.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MMPolicy = MMPolicy.Substring(0, 1);
            }
            Set_Value("MMPolicy", MMPolicy);
        }
        /** Get Material Policy.
        @return Material Movement Policy */
        public String GetMMPolicy()
        {
            return (String)Get_Value("MMPolicy");
        }
        /** Set Attribute Set.
        @param M_AttributeSet_ID Product Attribute Set */
        public void SetM_AttributeSet_ID(int M_AttributeSet_ID)
        {
            if (M_AttributeSet_ID <= 0) Set_Value("M_AttributeSet_ID", null);
            else
                Set_Value("M_AttributeSet_ID", M_AttributeSet_ID);
        }
        /** Get Attribute Set.
        @return Product Attribute Set */
        public int GetM_AttributeSet_ID()
        {
            Object ii = Get_Value("M_AttributeSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Cost Element. @param M_CostElement_ID Product Cost Element */
        public void SetM_CostElement_ID(int M_CostElement_ID)
        {
            if (M_CostElement_ID <= 0) Set_Value("M_CostElement_ID", null);
            else
                Set_Value("M_CostElement_ID", M_CostElement_ID);
        }/** Get Cost Element. @return Product Cost Element */
        public int GetM_CostElement_ID() { Object ii = Get_Value("M_CostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Product Category.
        @param M_Product_Category_ID Category of a Product */
        public void SetM_Product_Category_ID(int M_Product_Category_ID)
        {
            if (M_Product_Category_ID < 1) throw new ArgumentException("M_Product_Category_ID is mandatory.");
            Set_ValueNoCheck("M_Product_Category_ID", M_Product_Category_ID);
        }
        /** Get Product Category.
        @return Category of a Product */
        public int GetM_Product_Category_ID()
        {
            Object ii = Get_Value("M_Product_Category_ID");
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
        /** Set Planned Margin %.
        @param PlannedMargin Project's planned margin as a percentage */
        public void SetPlannedMargin(Decimal? PlannedMargin)
        {
            if (PlannedMargin == null) throw new ArgumentException("PlannedMargin is mandatory.");
            Set_Value("PlannedMargin", (Decimal?)PlannedMargin);
        }
        /** Get Planned Margin %.
        @return Project's planned margin as a percentage */
        public Decimal GetPlannedMargin()
        {
            Object bd = Get_Value("PlannedMargin");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** ProductType AD_Reference_ID=270 */
        public static int PRODUCTTYPE_AD_Reference_ID = 270;
        /** Expense type = E */
        public static String PRODUCTTYPE_ExpenseType = "E";
        /** Item = I */
        public static String PRODUCTTYPE_Item = "I";
        /** Online = O */
        public static String PRODUCTTYPE_Online = "O";
        /** Resource = R */
        public static String PRODUCTTYPE_Resource = "R";
        /** Service = S */
        public static String PRODUCTTYPE_Service = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProductTypeValid(String test)
        {
            return test == null || test.Equals("E") || test.Equals("I") || test.Equals("O") || test.Equals("R") || test.Equals("S");
        }
        /** Set Product Type.
        @param ProductType Type of product */
        public void SetProductType(String ProductType)
        {
            if (!IsProductTypeValid(ProductType))
                throw new ArgumentException("ProductType Invalid value - " + ProductType + " - Reference_ID=270 - E - I - O - R - S");
            if (ProductType != null && ProductType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProductType = ProductType.Substring(0, 1);
            }
            Set_Value("ProductType", ProductType);
        }
        /** Get Product Type.
        @return Type of product */
        public String GetProductType()
        {
            return (String)Get_Value("ProductType");
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

        /** Set Refernce Prod Category ID.
        @param VA007_RefProdCat_ID Refernce Prod Category ID */
        public void SetVA007_RefProdCat_ID(String VA007_RefProdCat_ID)
        {
            if (VA007_RefProdCat_ID != null && VA007_RefProdCat_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA007_RefProdCat_ID = VA007_RefProdCat_ID.Substring(0, 50);
            }
            Set_Value("VA007_RefProdCat_ID", VA007_RefProdCat_ID);
        }
        /** Get Refernce Prod Category ID.
        @return Refernce Prod Category ID */
        public String GetVA007_RefProdCat_ID()
        {
            return (String)Get_Value("VA007_RefProdCat_ID");
        }

        #region FRPT Posting Columns
        /** FRPT_CostingLevel AD_Reference_ID=355 */
        public static int FRPT_COSTINGLEVEL_AD_Reference_ID = 355;/** Batch/Lot = B */
        public static String FRPT_COSTINGLEVEL_BatchLot = "B";/** Client = C */
        public static String FRPT_COSTINGLEVEL_Client = "C";/** Organization = O */
        public static String FRPT_COSTINGLEVEL_Organization = "O";

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFRPT_CostingLevelValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("C") || test.Equals("O");
        }

        /** Set Costing Level.
        @param FRPT_CostingLevel Costing Level */
        public void SetFRPT_CostingLevel(String FRPT_CostingLevel)
        {
            if (!IsFRPT_CostingLevelValid(FRPT_CostingLevel))
                throw new ArgumentException("FRPT_CostingLevel Invalid value - " + FRPT_CostingLevel + " - Reference_ID=355 - B - C - O");
            if (FRPT_CostingLevel != null && FRPT_CostingLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                FRPT_CostingLevel = FRPT_CostingLevel.Substring(0, 1);
            }
            Set_Value("FRPT_CostingLevel", FRPT_CostingLevel);
        }

        /** Get Costing Level.
        @return Costing Level */
        public String GetFRPT_CostingLevel()
        {
            return (String)Get_Value("FRPT_CostingLevel");
        }

        /** FRPT_CostingMethod AD_Reference_ID=122 */
        public static int FRPT_COSTINGMETHOD_AD_Reference_ID = 122;/** Average PO = A */
        public static String FRPT_COSTINGMETHOD_AveragePO = "A";/** Fifo = F */
        public static String FRPT_COSTINGMETHOD_Fifo = "F";/** Average Invoice = I */
        public static String FRPT_COSTINGMETHOD_AverageInvoice = "I";/** Lifo = L */
        public static String FRPT_COSTINGMETHOD_Lifo = "L";/** Standard Costing = S */
        public static String FRPT_COSTINGMETHOD_StandardCosting = "S";/** User Defined = U */
        public static String FRPT_COSTINGMETHOD_UserDefined = "U";/** Last Invoice = i */
        public static String FRPT_COSTINGMETHOD_LastInvoice = "i";/** Last PO Price = p */
        public static String FRPT_COSTINGMETHOD_LastPOPrice = "p";/** _ = x */
        public static String FRPT_COSTINGMETHOD_ = "x";

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFRPT_CostingMethodValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("F") || test.Equals("I") ||
                test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i")
                || test.Equals("p") || test.Equals("x");
        }

        /** Set Costing Method.
        @param FRPT_CostingMethod Costing Method */
        public void SetFRPT_CostingMethod(String FRPT_CostingMethod)
        {
            if (!IsFRPT_CostingMethodValid(FRPT_CostingMethod))
                throw new ArgumentException("FRPT_CostingMethod Invalid value - " + FRPT_CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
            if (FRPT_CostingMethod != null && FRPT_CostingMethod.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                FRPT_CostingMethod = FRPT_CostingMethod.Substring(0, 1);
            }
            Set_Value("FRPT_CostingMethod", FRPT_CostingMethod);
        }

        /** Get Costing Method.
        @return Costing Method */
        public String GetFRPT_CostingMethod()
        {
            return (String)Get_Value("FRPT_CostingMethod");
        }

        #endregion


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
        public static int COSTINGMETHOD_AD_Reference_ID = 122;/** Average PO = A */
        public static String COSTINGMETHOD_AveragePO = "A";/** Cost Combination = C */
        public static String COSTINGMETHOD_CostCombination = "C";/** Fifo = F */
        public static String COSTINGMETHOD_Fifo = "F";/** Average Invoice = I */
        public static String COSTINGMETHOD_AverageInvoice = "I";/** Lifo = L */
        public static String COSTINGMETHOD_Lifo = "L";/** Standard Costing = S */
        public static String COSTINGMETHOD_StandardCosting = "S";/** User Defined = U */
        public static String COSTINGMETHOD_UserDefined = "U";/** Last Invoice = i */
        public static String COSTINGMETHOD_LastInvoice = "i";/** Last PO Price = p */
        public static String COSTINGMETHOD_LastPOPrice = "p";/** _ = x */
        public static String COSTINGMETHOD_ = "x";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCostingMethodValid(String test) { return test == null || test.Equals("A") || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x"); }/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
        public void SetCostingMethod(String CostingMethod)
        {
            if (!IsCostingMethodValid(CostingMethod))
                throw new ArgumentException("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - C - F - I - L - S - U - i - p - x"); if (CostingMethod != null && CostingMethod.Length > 1) { log.Warning("Length > 1 - truncated"); CostingMethod = CostingMethod.Substring(0, 1); } Set_Value("CostingMethod", CostingMethod);
        }/** Get Costing Method.
@return Indicates how Costs will be calculated */
        public String GetCostingMethod() { return (String)Get_Value("CostingMethod"); }
        /** Set Serial No Control.
        @param M_SerNoCtl_ID Product Serial Number Control */
        public void SetM_SerNoCtl_ID(int M_SerNoCtl_ID)
        {
            if (M_SerNoCtl_ID <= 0)
                Set_Value("M_SerNoCtl_ID", null);
            else
                Set_Value("M_SerNoCtl_ID", M_SerNoCtl_ID);
        }
        /** Get Serial No Control.
        @return Product Serial Number Control */
        public int GetM_SerNoCtl_ID()
        {
            Object ii = Get_Value("M_SerNoCtl_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
        /** VA073_ProductGroup AD_Reference_ID=1001193 */
        public static int VA073_PRODUCTGROUP_AD_Reference_ID = 1001193;/** Consumable = C */
        public static String VA073_PRODUCTGROUP_Consumable = "C";/** Finished Product = F */
        public static String VA073_PRODUCTGROUP_FinishedProduct = "F";/** Other = O */
        public static String VA073_PRODUCTGROUP_Other = "O";/** Raw Material = R */
        public static String VA073_PRODUCTGROUP_RawMaterial = "R";/** Semi-Finished Product = S */
        public static String VA073_PRODUCTGROUP_Semi_FinishedProduct = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA073_ProductGroupValid(String test) { return test == null || test.Equals("C") || test.Equals("F") || test.Equals("O") || test.Equals("R") || test.Equals("S"); }/** Set Product Group.
@param VA073_ProductGroup Product Group */
        public void SetVA073_ProductGroup(String VA073_ProductGroup)
        {
            if (!IsVA073_ProductGroupValid(VA073_ProductGroup))
                throw new ArgumentException("VA073_ProductGroup Invalid value - " + VA073_ProductGroup + " - Reference_ID=1001193 - C - F - O - R - S"); if (VA073_ProductGroup != null && VA073_ProductGroup.Length > 1) { log.Warning("Length > 1 - truncated"); VA073_ProductGroup = VA073_ProductGroup.Substring(0, 1); }
            Set_Value("VA073_ProductGroup", VA073_ProductGroup);
        }/** Get Product Group.
@return Product Group */
        public String GetVA073_ProductGroup() { return (String)Get_Value("VA073_ProductGroup"); }
    }

}
