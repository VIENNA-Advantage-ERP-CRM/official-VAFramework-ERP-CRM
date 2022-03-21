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
    /** Generated Model for M_Product
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Product : PO
    {
        public X_M_Product(Context ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            /** if (M_Product_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetC_UOM_ID (0);
            SetIsBOM (false);	// N
            SetIsBasedOnRollup (true);	// Y
            SetIsDropShip (false);
            SetIsExcludeAutoDelivery (false);	// N
            SetIsInvoicePrintDetails (false);
            SetIsManufactured (false);	// N
            SetIsPermitRequired (false);	// N
            SetIsPickListPrintDetails (false);
            SetIsPlannedItem (false);	// N
            SetIsPurchased (true);	// Y
            SetIsPurchasedToOrder (false);	// NTaxCa
            SetIsSelfService (true);	// Y
            SetIsSold (true);	// Y
            SetIsStocked (true);	// Y
            SetIsSummary (false);
            SetIsVerified (false);	// N
            SetIsWebStoreFeatured (false);
            SetM_Product_Category_ID (0);
            SetM_Product_ID (0);
            SetName (null);
            SetProductType (null);	// I
            SetQtyTolerance (0.0);	// 0
            SetValue (null);
            }
             */
        }
        public X_M_Product(Ctx ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            /** if (M_Product_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetC_UOM_ID (0);
            SetIsBOM (false);	// N
            SetIsBasedOnRollup (true);	// Y
            SetIsDropShip (false);
            SetIsExcludeAutoDelivery (false);	// N
            SetIsInvoicePrintDetails (false);
            SetIsManufactured (false);	// N
            SetIsPermitRequired (false);	// N
            SetIsPickListPrintDetails (false);
            SetIsPlannedItem (false);	// N
            SetIsPurchased (true);	// Y
            SetIsPurchasedToOrder (false);	// N
            SetIsSelfService (true);	// Y
            SetIsSold (true);	// Y
            SetIsStocked (true);	// Y
            SetIsSummary (false);
            SetIsVerified (false);	// N
            SetIsWebStoreFeatured (false);
            SetM_Product_Category_ID (0);
            SetM_Product_ID (0);
            SetName (null);
            SetProductType (null);	// I
            SetQtyTolerance (0.0);	// 0
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Product()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721416748765L;
        /** Last Updated Timestamp 8/11/2015 4:40:33 PM */
        public static long updatedMS = 1439291431976L;
        /** AD_Table_ID=208 */
        public static int Table_ID;
        // =208;

        /** TableName=M_Product */
        public static String Table_Name = "M_Product";

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
            StringBuilder sb = new StringBuilder("X_M_Product[").Append(Get_ID()).Append("]");
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

        /** BasisType AD_Reference_ID=1000032 */
        public static int BASISTYPE_AD_Reference_ID = 1000032;
        /** Per Batch = B */
        public static String BASISTYPE_PerBatch = "B";
        /** Per Item = I */
        public static String BASISTYPE_PerItem = "I";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsBasisTypeValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("I");
        }
        /** Set Cost Basis Type.
        @param BasisType Indicates the option to consume and charge materials and resources */
        public void SetBasisType(String BasisType)
        {
            if (!IsBasisTypeValid(BasisType))
                throw new ArgumentException("BasisType Invalid value - " + BasisType + " - Reference_ID=1000032 - B - I");
            if (BasisType != null && BasisType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                BasisType = BasisType.Substring(0, 1);
            }
            Set_Value("BasisType", BasisType);
        }
        /** Get Cost Basis Type.
        @return Indicates the option to consume and charge materials and resources */
        public String GetBasisType()
        {
            return (String)Get_Value("BasisType");
        }
        /** Set Batch Size.
        @param BatchSize Number of items in a batch for the product */
        public void SetBatchSize(int BatchSize)
        {
            Set_Value("BatchSize", BatchSize);
        }
        /** Get Batch Size.
        @return Number of items in a batch for the product */
        public int GetBatchSize()
        {
            Object ii = Get_Value("BatchSize");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Revenue Recognition.
        @param C_RevenueRecognition_ID Method for recording revenue */
        public void SetC_RevenueRecognition_ID(int C_RevenueRecognition_ID)
        {
            if (C_RevenueRecognition_ID <= 0) Set_Value("C_RevenueRecognition_ID", null);
            else
                Set_Value("C_RevenueRecognition_ID", C_RevenueRecognition_ID);
        }
        /** Get Revenue Recognition.
        @return Method for recording revenue */
        public int GetC_RevenueRecognition_ID()
        {
            Object ii = Get_Value("C_RevenueRecognition_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Skill.
        @param C_Skill_ID Skills, certifications and specialization */
        public void SetC_Skill_ID(int C_Skill_ID)
        {
            if (C_Skill_ID <= 0) Set_Value("C_Skill_ID", null);
            else
                Set_Value("C_Skill_ID", C_Skill_ID);
        }
        /** Get Skill.
        @return Skills, certifications and specialization */
        public int GetC_Skill_ID()
        {
            Object ii = Get_Value("C_Skill_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Subscription Type.
        @param C_SubscriptionType_ID Type of subscription */
        public void SetC_SubscriptionType_ID(int C_SubscriptionType_ID)
        {
            if (C_SubscriptionType_ID <= 0) Set_Value("C_SubscriptionType_ID", null);
            else
                Set_Value("C_SubscriptionType_ID", C_SubscriptionType_ID);
        }
        /** Get Subscription Type.
        @return Type of subscription */
        public int GetC_SubscriptionType_ID()
        {
            Object ii = Get_Value("C_SubscriptionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Category.
        @param C_TaxCategory_ID Tax Category */
        public void SetC_TaxCategory_ID(int C_TaxCategory_ID)
        {
            //JID_0128: When we select the Summary level checkbox true, tax category field get hidden. when we try to save the record system giving error for Tax category field is mandatory.
            //if (C_TaxCategory_ID < 1) throw new ArgumentException("C_TaxCategory_ID is mandatory.");
            if (C_TaxCategory_ID <= 0)
                Set_Value("C_TaxCategory_ID", null);
            else
                Set_Value("C_TaxCategory_ID", C_TaxCategory_ID);
        }
        /** Get Tax Category.
        @return Tax Category */
        public int GetC_TaxCategory_ID()
        {
            Object ii = Get_Value("C_TaxCategory_ID");
            if (ii == null)
            {
                //MProductCategory pCat = new MProductCategory(GetCtx(), GetM_Product_Category_ID(), null);
                X_M_Product_Category pCat = new X_M_Product_Category(GetCtx(), GetM_Product_Category_ID(), null);
                return pCat.GetC_TaxCategory_ID() == null ? 0 : pCat.GetC_TaxCategory_ID();
            }
            return Convert.ToInt32(ii);
        }
        /** Set UOM Group.
        @param C_UOMGroup_ID Group for managing sets of Unit of Measure */
        public void SetC_UOMGroup_ID(int C_UOMGroup_ID)
        {
            if (C_UOMGroup_ID <= 0) Set_Value("C_UOMGroup_ID", null);
            else
                Set_Value("C_UOMGroup_ID", C_UOMGroup_ID);
        }
        /** Get UOM Group.
        @return Group for managing sets of Unit of Measure */
        public int GetC_UOMGroup_ID()
        {
            Object ii = Get_Value("C_UOMGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
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

        /** ChargeType AD_Reference_ID=1000031 */
        public static int CHARGETYPE_AD_Reference_ID = 1000031;
        /** Automatic = A */
        public static String CHARGETYPE_Automatic = "A";
        /** Manual = M */
        public static String CHARGETYPE_Manual = "M";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsChargeTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("M");
        }
        /** Set Cost Charge Type.
        @param ChargeType Indicates how the production resource will be charged - automatically or manually */
        public void SetChargeType(String ChargeType)
        {
            if (!IsChargeTypeValid(ChargeType))
                throw new ArgumentException("ChargeType Invalid value - " + ChargeType + " - Reference_ID=1000031 - A - M");
            if (ChargeType != null && ChargeType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ChargeType = ChargeType.Substring(0, 1);
            }
            Set_Value("ChargeType", ChargeType);
        }
        /** Get Cost Charge Type.
        @return Indicates how the production resource will be charged - automatically or manually */
        public String GetChargeType()
        {
            return (String)Get_Value("ChargeType");
        }
        /** Set Classification.
        @param Classification Classification for grouping */
        public void SetClassification(String Classification)
        {
            if (Classification != null && Classification.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Classification = Classification.Substring(0, 1);
            }
            Set_Value("Classification", Classification);
        }
        /** Get Classification.
        @return Classification for grouping */
        public String GetClassification()
        {
            return (String)Get_Value("Classification");
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
        /** Set Direct Delivery to Stores.
        @param DTD001_IsDirectDelivery Direct Delivery to Stores */
        public void SetDTD001_IsDirectDelivery(Boolean DTD001_IsDirectDelivery)
        {
            Set_Value("DTD001_IsDirectDelivery", DTD001_IsDirectDelivery);
        }
        /** Get Direct Delivery to Stores.
        @return Direct Delivery to Stores */
        public Boolean IsDTD001_IsDirectDelivery()
        {
            Object oo = Get_Value("DTD001_IsDirectDelivery");
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
        /** Set Description URL.
        @param DescriptionURL URL for the description */
        public void SetDescriptionURL(String DescriptionURL)
        {
            if (DescriptionURL != null && DescriptionURL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                DescriptionURL = DescriptionURL.Substring(0, 120);
            }
            Set_Value("DescriptionURL", DescriptionURL);
        }
        /** Get Description URL.
        @return URL for the description */
        public String GetDescriptionURL()
        {
            return (String)Get_Value("DescriptionURL");
        }
        /** Set Discontinued.
        @param Discontinued This product is no longer available */
        public void SetDiscontinued(Boolean Discontinued)
        {
            Set_Value("Discontinued", Discontinued);
        }
        /** Get Discontinued.
        @return This product is no longer available */
        public Boolean IsDiscontinued()
        {
            Object oo = Get_Value("Discontinued");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Discontinued by.
        @param DiscontinuedBy Discontinued By */
        public void SetDiscontinuedBy(DateTime? DiscontinuedBy)
        {
            Set_Value("DiscontinuedBy", (DateTime?)DiscontinuedBy);
        }
        /** Get Discontinued by.
        @return Discontinued By */
        public DateTime? GetDiscontinuedBy()
        {
            return (DateTime?)Get_Value("DiscontinuedBy");
        }
        /** Set Document Note.
        @param DocumentNote Additional information for a Document */
        public void SetDocumentNote(String DocumentNote)
        {
            if (DocumentNote != null && DocumentNote.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                DocumentNote = DocumentNote.Substring(0, 2000);
            }
            Set_Value("DocumentNote", DocumentNote);
        }
        /** Get Document Note.
        @return Additional information for a Document */
        public String GetDocumentNote()
        {
            return (String)Get_Value("DocumentNote");
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
        /** Set Guarantee Days.
        @param GuaranteeDays Number of days the product is guaranteed or available */
        public void SetGuaranteeDays(int GuaranteeDays)
        {
            Set_Value("GuaranteeDays", GuaranteeDays);
        }
        /** Get Guarantee Days.
        @return Number of days the product is guaranteed or available */
        public int GetGuaranteeDays()
        {
            Object ii = Get_Value("GuaranteeDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Min Guarantee Days.
        @param GuaranteeDaysMin Minumum number of guarantee days */
        public void SetGuaranteeDaysMin(int GuaranteeDaysMin)
        {
            Set_Value("GuaranteeDaysMin", GuaranteeDaysMin);
        }
        /** Get Min Guarantee Days.
        @return Minumum number of guarantee days */
        public int GetGuaranteeDaysMin()
        {
            Object ii = Get_Value("GuaranteeDaysMin");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Comment.
        @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Image URL.
        @param ImageURL URL of  image */
        public void SetImageURL(String ImageURL)
        {
            if (ImageURL != null && ImageURL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                ImageURL = ImageURL.Substring(0, 120);
            }
            Set_Value("ImageURL", ImageURL);
        }
        /** Get Image URL.
        @return URL of  image */
        public String GetImageURL()
        {
            return (String)Get_Value("ImageURL");
        }
        /** Set Available.
        @param IsAvailable Resource is available */
        public void SetIsAvailable(Boolean IsAvailable)
        {
            Set_Value("IsAvailable", IsAvailable);
        }
        /** Get Available.
        @return Resource is available */
        public Boolean IsAvailable()
        {
            Object oo = Get_Value("IsAvailable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Bill of Materials.
        @param IsBOM Bill of Materials */
        public void SetIsBOM(Boolean IsBOM)
        {
            Set_Value("IsBOM", IsBOM);
        }
        /** Get Bill of Materials.
        @return Bill of Materials */
        public Boolean IsBOM()
        {
            Object oo = Get_Value("IsBOM");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Cost Based on Rollup.
        @param IsBasedOnRollup Indicates that product cost will be re-calculated in the cost rollup process. */
        public void SetIsBasedOnRollup(Boolean IsBasedOnRollup)
        {
            Set_Value("IsBasedOnRollup", IsBasedOnRollup);
        }
        /** Get Cost Based on Rollup.
        @return Indicates that product cost will be re-calculated in the cost rollup process. */
        public Boolean IsBasedOnRollup()
        {
            Object oo = Get_Value("IsBasedOnRollup");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_Value("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Exclude Auto Delivery.
        @param IsExcludeAutoDelivery Exclude from automatic Delivery */
        public void SetIsExcludeAutoDelivery(Boolean IsExcludeAutoDelivery)
        {
            Set_Value("IsExcludeAutoDelivery", IsExcludeAutoDelivery);
        }
        /** Get Exclude Auto Delivery.
        @return Exclude from automatic Delivery */
        public Boolean IsExcludeAutoDelivery()
        {
            Object oo = Get_Value("IsExcludeAutoDelivery");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Print detail records on invoice .
        @param IsInvoicePrintDetails Print detail BOM elements on the invoice */
        public void SetIsInvoicePrintDetails(Boolean IsInvoicePrintDetails)
        {
            Set_Value("IsInvoicePrintDetails", IsInvoicePrintDetails);
        }
        /** Get Print detail records on invoice .
        @return Print detail BOM elements on the invoice */
        public Boolean IsInvoicePrintDetails()
        {
            Object oo = Get_Value("IsInvoicePrintDetails");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Manufactured.
        @param IsManufactured Indicates if the product is manufactured */
        public void SetIsManufactured(Boolean IsManufactured)
        {
            Set_Value("IsManufactured", IsManufactured);
        }
        /** Get Manufactured.
        @return Indicates if the product is manufactured */
        public Boolean IsManufactured()
        {
            Object oo = Get_Value("IsManufactured");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Permit Required.
        @param IsPermitRequired Indicates if a permit or similar authorization is required for use or execution of a product, resource or work order operation. */
        public void SetIsPermitRequired(Boolean IsPermitRequired)
        {
            Set_Value("IsPermitRequired", IsPermitRequired);
        }
        /** Get Permit Required.
        @return Indicates if a permit or similar authorization is required for use or execution of a product, resource or work order operation. */
        public Boolean IsPermitRequired()
        {
            Object oo = Get_Value("IsPermitRequired");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Print detail records on pick list.
        @param IsPickListPrintDetails Print detail BOM elements on the pick list */
        public void SetIsPickListPrintDetails(Boolean IsPickListPrintDetails)
        {
            Set_Value("IsPickListPrintDetails", IsPickListPrintDetails);
        }
        /** Get Print detail records on pick list.
        @return Print detail BOM elements on the pick list */
        public Boolean IsPickListPrintDetails()
        {
            Object oo = Get_Value("IsPickListPrintDetails");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Planned Item.
        @param IsPlannedItem Indicates if the product is available for planning in Vienna MRP */
        public void SetIsPlannedItem(Boolean IsPlannedItem)
        {
            Set_Value("IsPlannedItem", IsPlannedItem);
        }
        /** Get Planned Item.
        @return Indicates if the product is available for planning in Vienna MRP */
        public Boolean IsPlannedItem()
        {
            Object oo = Get_Value("IsPlannedItem");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Purchased.
        @param IsPurchased Organization purchases this product */
        public void SetIsPurchased(Boolean IsPurchased)
        {
            Set_Value("IsPurchased", IsPurchased);
        }
        /** Get Purchased.
        @return Organization purchases this product */
        public Boolean IsPurchased()
        {
            Object oo = Get_Value("IsPurchased");
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
        /** Set Sold.
        @param IsSold Organization sells this product */
        public void SetIsSold(Boolean IsSold)
        {
            Set_Value("IsSold", IsSold);
        }
        /** Get Sold.
        @return Organization sells this product */
        public Boolean IsSold()
        {
            Object oo = Get_Value("IsSold");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Stocked.
        @param IsStocked Organization stocks this product */
        public void SetIsStocked(Boolean IsStocked)
        {
            Set_Value("IsStocked", IsStocked);
        }
        /** Get Stocked.
        @return Organization stocks this product */
        public Boolean IsStocked()
        {
            Object oo = Get_Value("IsStocked");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Level.
        @param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary)
        {
            Set_Value("IsSummary", IsSummary);
        }
        /** Get Summary Level.
        @return This is a summary entity */
        public Boolean IsSummary()
        {
            Object oo = Get_Value("IsSummary");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Verified.
        @param IsVerified The BOM configuration has been verified */
        public void SetIsVerified(Boolean IsVerified)
        {
            Set_ValueNoCheck("IsVerified", IsVerified);
        }
        /** Get Verified.
        @return The BOM configuration has been verified */
        public Boolean IsVerified()
        {
            Object oo = Get_Value("IsVerified");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Featured in Web Store.
        @param IsWebStoreFeatured If selected, the product is displayed in the inital or any empy search */
        public void SetIsWebStoreFeatured(Boolean IsWebStoreFeatured)
        {
            Set_Value("IsWebStoreFeatured", IsWebStoreFeatured);
        }
        /** Get Featured in Web Store.
        @return If selected, the product is displayed in the inital or any empy search */
        public Boolean IsWebStoreFeatured()
        {
            Object oo = Get_Value("IsWebStoreFeatured");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set License Info.
        @param LicenseInfo License Information */
        public void SetLicenseInfo(String LicenseInfo)
        {
            if (LicenseInfo != null && LicenseInfo.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LicenseInfo = LicenseInfo.Substring(0, 255);
            }
            Set_Value("LicenseInfo", LicenseInfo);
        }
        /** Get License Info.
        @return License Information */
        public String GetLicenseInfo()
        {
            return (String)Get_Value("LicenseInfo");
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Freight Category.
        @param M_FreightCategory_ID Category of the Freight */
        public void SetM_FreightCategory_ID(int M_FreightCategory_ID)
        {
            if (M_FreightCategory_ID <= 0) Set_Value("M_FreightCategory_ID", null);
            else
                Set_Value("M_FreightCategory_ID", M_FreightCategory_ID);
        }
        /** Get Freight Category.
        @return Category of the Freight */
        public int GetM_FreightCategory_ID()
        {
            Object ii = Get_Value("M_FreightCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID <= 0) Set_Value("M_Locator_ID", null);
            else
                Set_Value("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Product_Category_ID AD_Reference_ID=163 */
        public static int M_PRODUCT_CATEGORY_ID_AD_Reference_ID = 163;
        /** Set Product Category.
        @param M_Product_Category_ID Category of a Product */
        public void SetM_Product_Category_ID(int M_Product_Category_ID)
        {
            if (M_Product_Category_ID < 1) throw new ArgumentException("M_Product_Category_ID is mandatory.");
            Set_Value("M_Product_Category_ID", M_Product_Category_ID);
        }
        /** Get Product Category.
        @return Category of a Product */
        public int GetM_Product_Category_ID()
        {
            Object ii = Get_Value("M_Product_Category_ID");
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
        /** Set Make.
        @param Make Identifies the manufacturer of the product */
        public void SetMake(String Make)
        {
            if (Make != null && Make.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Make = Make.Substring(0, 60);
            }
            Set_Value("Make", Make);
        }
        /** Get Make.
        @return Identifies the manufacturer of the product */
        public String GetMake()
        {
            return (String)Get_Value("Make");
        }
        /** Set Expected Manufacture Time.
        @param ManufactureTime_Expected Expected number of days required to manufacture the product */
        public void SetManufactureTime_Expected(int ManufactureTime_Expected)
        {
            Set_Value("ManufactureTime_Expected", ManufactureTime_Expected);
        }
        /** Get Expected Manufacture Time.
        @return Expected number of days required to manufacture the product */
        public int GetManufactureTime_Expected()
        {
            Object ii = Get_Value("ManufactureTime_Expected");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Model.
        @param Model Describes the model of the product */
        public void SetModel(String Model)
        {
            if (Model != null && Model.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Model = Model.Substring(0, 60);
            }
            Set_Value("Model", Model);
        }
        /** Get Model.
        @return Describes the model of the product */
        public String GetModel()
        {
            return (String)Get_Value("Model");
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
            return test.Equals("E") || test.Equals("I") || test.Equals("O") || test.Equals("R") || test.Equals("S");
        }
        /** Set Product Type.
        @param ProductType Type of product */
        public void SetProductType(String ProductType)
        {
            if (ProductType == null) throw new ArgumentException("ProductType is mandatory");
            if (!IsProductTypeValid(ProductType))
                throw new ArgumentException("ProductType Invalid value - " + ProductType + " - Reference_ID=270 - E - I - O - R - S");
            if (ProductType.Length > 1)
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
        /** Set Acceptable Qty Difference.
        @param QtyTolerance Acceptable percentage difference between the Book Quantity and the actual onhand Quantity */
        public void SetQtyTolerance(Decimal? QtyTolerance)
        {
            if (QtyTolerance == null) throw new ArgumentException("QtyTolerance is mandatory.");
            Set_Value("QtyTolerance", (Decimal?)QtyTolerance);
        }
        /** Get Acceptable Qty Difference.
        @return Acceptable percentage difference between the Book Quantity and the actual onhand Quantity */
        public Decimal GetQtyTolerance()
        {
            Object bd = Get_Value("QtyTolerance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Mail Template.
        @param R_MailText_ID Text templates for mailings */
        public void SetR_MailText_ID(int R_MailText_ID)
        {
            if (R_MailText_ID <= 0) Set_Value("R_MailText_ID", null);
            else
                Set_Value("R_MailText_ID", R_MailText_ID);
        }
        /** Get Mail Template.
        @return Text templates for mailings */
        public int GetR_MailText_ID()
        {
            Object ii = Get_Value("R_MailText_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Source.
        @param R_Source_ID Source for the Lead or Request */
        public void SetR_Source_ID(int R_Source_ID)
        {
            if (R_Source_ID <= 0) Set_Value("R_Source_ID", null);
            else
                Set_Value("R_Source_ID", R_Source_ID);
        }
        /** Get Source.
        @return Source for the Lead or Request */
        public int GetR_Source_ID()
        {
            Object ii = Get_Value("R_Source_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** ResourceGroup AD_Reference_ID=1000030 */
        public static int RESOURCEGROUP_AD_Reference_ID = 1000030;
        /** Equipment = E */
        public static String RESOURCEGROUP_Equipment = "E";
        /** Other = O */
        public static String RESOURCEGROUP_Other = "O";
        /** Person = P */
        public static String RESOURCEGROUP_Person = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsResourceGroupValid(String test)
        {
            return test == null || test.Equals("E") || test.Equals("O") || test.Equals("P");
        }
        /** Set Resource Group.
        @param ResourceGroup First level grouping of resources into Person, Equipment or Other. */
        public void SetResourceGroup(String ResourceGroup)
        {
            if (!IsResourceGroupValid(ResourceGroup))
                throw new ArgumentException("ResourceGroup Invalid value - " + ResourceGroup + " - Reference_ID=1000030 - E - O - P");
            if (ResourceGroup != null && ResourceGroup.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ResourceGroup = ResourceGroup.Substring(0, 1);
            }
            Set_Value("ResourceGroup", ResourceGroup);
        }
        /** Get Resource Group.
        @return First level grouping of resources into Person, Equipment or Other. */
        public String GetResourceGroup()
        {
            return (String)Get_Value("ResourceGroup");
        }
        /** Set SKU.
        @param SKU Stock Keeping Unit */
        public void SetSKU(String SKU)
        {
            if (SKU != null && SKU.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                SKU = SKU.Substring(0, 30);
            }
            Set_Value("SKU", SKU);
        }
        /** Get SKU.
        @return Stock Keeping Unit */
        public String GetSKU()
        {
            return (String)Get_Value("SKU");
        }
        /** Set Expense Type.
        @param S_ExpenseType_ID Expense report type */
        public void SetS_ExpenseType_ID(int S_ExpenseType_ID)
        {
            if (S_ExpenseType_ID <= 0) Set_ValueNoCheck("S_ExpenseType_ID", null);
            else
                Set_ValueNoCheck("S_ExpenseType_ID", S_ExpenseType_ID);
        }
        /** Get Expense Type.
        @return Expense report type */
        public int GetS_ExpenseType_ID()
        {
            Object ii = Get_Value("S_ExpenseType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Resource.
        @param S_Resource_ID Resource */
        public void SetS_Resource_ID(int S_Resource_ID)
        {
            if (S_Resource_ID <= 0) Set_ValueNoCheck("S_Resource_ID", null);
            else
                Set_ValueNoCheck("S_Resource_ID", S_Resource_ID);
        }
        /** Get Resource.
        @return Resource */
        public int GetS_Resource_ID()
        {
            Object ii = Get_Value("S_Resource_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Sales Rep.
        @param SalesRep_ID Company Agent like Sales Representitive, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Sales Rep.
        @return Company Agent like Sales Representitive, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Depth.
        @param ShelfDepth Shelf depth required */
        public void SetShelfDepth(int ShelfDepth)
        {
            Set_Value("ShelfDepth", ShelfDepth);
        }
        /** Get Shelf Depth.
        @return Shelf depth required */
        public int GetShelfDepth()
        {
            Object ii = Get_Value("ShelfDepth");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Height.
        @param ShelfHeight Shelf height required */
        public void SetShelfHeight(int ShelfHeight)
        {
            Set_Value("ShelfHeight", ShelfHeight);
        }
        /** Get Shelf Height.
        @return Shelf height required */
        public int GetShelfHeight()
        {
            Object ii = Get_Value("ShelfHeight");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Width.
        @param ShelfWidth Shelf width required */
        public void SetShelfWidth(int ShelfWidth)
        {
            Set_Value("ShelfWidth", ShelfWidth);
        }
        /** Get Shelf Width.
        @return Shelf width required */
        public int GetShelfWidth()
        {
            Object ii = Get_Value("ShelfWidth");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Support Units.
        @param SupportUnits Number of Support Units, e.g. Supported Internal Users */
        public void SetSupportUnits(int SupportUnits)
        {
            Set_Value("SupportUnits", SupportUnits);
        }
        /** Get Support Units.
        @return Number of Support Units, e.g. Supported Internal Users */
        public int GetSupportUnits()
        {
            Object ii = Get_Value("SupportUnits");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Trial Phase Days.
        @param TrialPhaseDays Days for a Trail */
        public void SetTrialPhaseDays(int TrialPhaseDays)
        {
            Set_Value("TrialPhaseDays", TrialPhaseDays);
        }
        /** Get Trial Phase Days.
        @return Days for a Trail */
        public int GetTrialPhaseDays()
        {
            Object ii = Get_Value("TrialPhaseDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UPC/EAN.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            if (UPC != null && UPC.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                UPC = UPC.Substring(0, 30);
            }
            Set_Value("UPC", UPC);
        }
        /** Get UPC/EAN.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }
        /** Set Units Per Pallet.
        @param UnitsPerPallet Units Per Pallet */
        public void SetUnitsPerPallet(int UnitsPerPallet)
        {
            Set_Value("UnitsPerPallet", UnitsPerPallet);
        }
        /** Get Units Per Pallet.
        @return Units Per Pallet */
        public int GetUnitsPerPallet()
        {
            Object ii = Get_Value("UnitsPerPallet");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Image.
        @param VAPOS_Image Image */
        public void SetVAPOS_Image(Byte[] VAPOS_Image)
        {
            Set_Value("VAPOS_Image", VAPOS_Image);
        }
        /** Get Image.
        @return Image */
        public Byte[] GetVAPOS_Image()
        {
            return (Byte[])Get_Value("VAPOS_Image");
        }
        /** Set Add Dish.
        @param VAPOS_IsAddDish Add Dish */
        public void SetVAPOS_IsAddDish(Boolean VAPOS_IsAddDish)
        {
            Set_Value("VAPOS_IsAddDish", VAPOS_IsAddDish);
        }
        /** Get Add Dish.
        @return Add Dish */
        public Boolean IsVAPOS_IsAddDish()
        {
            Object oo = Get_Value("VAPOS_IsAddDish");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Discount Not Applicable.
        @param VAPOS_IsDiscountNotApplicable Discount Not Applicable */
        public void SetVAPOS_IsDiscountNotApplicable(Boolean VAPOS_IsDiscountNotApplicable)
        {
            Set_Value("VAPOS_IsDiscountNotApplicable", VAPOS_IsDiscountNotApplicable);
        }
        /** Get Discount Not Applicable.
        @return Discount Not Applicable */
        public Boolean IsVAPOS_IsDiscountNotApplicable()
        {
            Object oo = Get_Value("VAPOS_IsDiscountNotApplicable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set BINARYDESCRIPTION.
       @param BINARYDESCRIPTION BINARYDESCRIPTION */
        public void SetBINARYDESCRIPTION(Byte[] BINARYDESCRIPTION)
        {
            Set_Value("BINARYDESCRIPTION", BINARYDESCRIPTION);
        }
        /** Get BINARYDESCRIPTION.
        @return BINARYDESCRIPTION */
        public Byte[] GetBINARYDESCRIPTION()
        {
            return (Byte[])Get_Value("BINARYDESCRIPTION");
        }

        /** Set UploadWithoutInventory.
@param UploadWithoutInventory UploadWithoutInventory */
        public void SetUploadWithoutInventory(Boolean UploadWithoutInventory)
        {
            Set_Value("UploadWithoutInventory", UploadWithoutInventory);
        }
        /** Get UploadWithoutInventory.
       @return UploadWithoutInventory */
        public Boolean IsUploadWithoutInventory()
        {
            Object oo = Get_Value("UploadWithoutInventory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Fresh Item.
        @param VAPOS_IsFresh Fresh Item */
        public void SetVAPOS_IsFresh(Boolean VAPOS_IsFresh)
        {
            Set_Value("VAPOS_IsFresh", VAPOS_IsFresh);
        }
        /** Get Fresh Item.
        @return Fresh Item */
        public Boolean IsVAPOS_IsFresh()
        {
            Object oo = Get_Value("VAPOS_IsFresh");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }


        /** Set Division.
       @param SAP001_Division_ID Division */
        public void SetSAP001_Division_ID(int SAP001_Division_ID)
        {
            if (SAP001_Division_ID <= 0) Set_Value("SAP001_Division_ID", null);
            else
                Set_Value("SAP001_Division_ID", SAP001_Division_ID);
        }
        /** Get Division.
        @return Division */
        public int GetSAP001_Division_ID()
        {
            Object ii = Get_Value("SAP001_Division_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Sell based on FIFO.
        @param VAPOS_IsSoldFifo Sell based on FIFO */
        public void SetVAPOS_IsSoldFifo(Boolean VAPOS_IsSoldFifo)
        {
            Set_Value("VAPOS_IsSoldFifo", VAPOS_IsSoldFifo);
        }
        /** Get Sell based on FIFO.
        @return Sell based on FIFO */
        public Boolean IsVAPOS_IsSoldFifo()
        {
            Object oo = Get_Value("VAPOS_IsSoldFifo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Variable Weight.
        @param VAPOS_IsVariableWeight Variable Weight */
        public void SetVAPOS_IsVariableWeight(Boolean VAPOS_IsVariableWeight)
        {
            Set_Value("VAPOS_IsVariableWeight", VAPOS_IsVariableWeight);
        }
        /** Get Variable Weight.
        @return Variable Weight */
        public Boolean IsVAPOS_IsVariableWeight()
        {
            Object oo = Get_Value("VAPOS_IsVariableWeight");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set No Scale.
        @param VAPOS_NoScale No Scale */
        public void SetVAPOS_NoScale(Boolean VAPOS_NoScale)
        {
            Set_Value("VAPOS_NoScale", VAPOS_NoScale);
        }
        /** Get No Scale.
        @return No Scale */
        public Boolean IsVAPOS_NoScale()
        {
            Object oo = Get_Value("VAPOS_NoScale");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Packing Weight.
        @param VAPOS_PackingWeight Packing Weight */
        public void SetVAPOS_PackingWeight(Decimal? VAPOS_PackingWeight)
        {
            Set_Value("VAPOS_PackingWeight", (Decimal?)VAPOS_PackingWeight);
        }
        /** Get Packing Weight.
        @return Packing Weight */
        public Decimal GetVAPOS_PackingWeight()
        {
            Object bd = Get_Value("VAPOS_PackingWeight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Scale Code.
        @param VAPOS_ScaleCode Scale Code */
        public void SetVAPOS_ScaleCode(String VAPOS_ScaleCode)
        {
            if (VAPOS_ScaleCode != null && VAPOS_ScaleCode.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VAPOS_ScaleCode = VAPOS_ScaleCode.Substring(0, 50);
            }
            Set_Value("VAPOS_ScaleCode", VAPOS_ScaleCode);
        }
        /** Get Scale Code.
        @return Scale Code */
        public String GetVAPOS_ScaleCode()
        {
            return (String)Get_Value("VAPOS_ScaleCode");
        }
        /** Set Scale Description.
        @param VAPOS_ScaleDescription Scale Description */
        public void SetVAPOS_ScaleDescription(String VAPOS_ScaleDescription)
        {
            if (VAPOS_ScaleDescription != null && VAPOS_ScaleDescription.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VAPOS_ScaleDescription = VAPOS_ScaleDescription.Substring(0, 50);
            }
            Set_Value("VAPOS_ScaleDescription", VAPOS_ScaleDescription);
        }
        /** Get Scale Description.
        @return Scale Description */
        public String GetVAPOS_ScaleDescription()
        {
            return (String)Get_Value("VAPOS_ScaleDescription");
        }
        /** Set Tare Weight.
        @param VAPOS_TareWeight Tare Weight */
        public void SetVAPOS_TareWeight(Decimal? VAPOS_TareWeight)
        {
            Set_Value("VAPOS_TareWeight", (Decimal?)VAPOS_TareWeight);
        }
        /** Get Tare Weight.
        @return Tare Weight */
        public Decimal GetVAPOS_TareWeight()
        {
            Object bd = Get_Value("VAPOS_TareWeight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Version No.
        @param VersionNo Version Number */
        public void SetVersionNo(String VersionNo)
        {
            if (VersionNo != null && VersionNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                VersionNo = VersionNo.Substring(0, 20);
            }
            Set_Value("VersionNo", VersionNo);
        }
        /** Get Version No.
        @return Version Number */
        public String GetVersionNo()
        {
            return (String)Get_Value("VersionNo");
        }
        /** Set Volume.
        @param Volume Volume of a product */
        public void SetVolume(Decimal? Volume)
        {
            Set_Value("Volume", (Decimal?)Volume);
        }
        /** Get Volume.
        @return Volume of a product */
        public Decimal GetVolume()
        {
            Object bd = Get_Value("Volume");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Weight.
        @param Weight Weight of a product */
        public void SetWeight(Decimal? Weight)
        {
            Set_Value("Weight", (Decimal?)Weight);
        }
        /** Get Weight.
        @return Weight of a product */
        public Decimal GetWeight()
        {
            Object bd = Get_Value("Weight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Reference Product ID.
        @param VA007_RefProduct_ID Reference Product ID */
        public void SetVA007_RefProduct_ID(String VA007_RefProduct_ID)
        {
            if (VA007_RefProduct_ID != null && VA007_RefProduct_ID.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                VA007_RefProduct_ID = VA007_RefProduct_ID.Substring(0, 30);
            }
            Set_Value("VA007_RefProduct_ID", VA007_RefProduct_ID);
        }
        /** Get Reference Product ID.
        @return Reference Product ID */
        public String GetVA007_RefProduct_ID()
        {
            return (String)Get_Value("VA007_RefProduct_ID");
        }



        //        /** VA019_ItemType AD_Reference_ID=1000239 */
        //        public static int VA019_ITEMTYPE_AD_Reference_ID = 1000239;/** Combo = C */
        //        public static String VA019_ITEMTYPE_Combo = "C";/** Ingredient = I */
        //        public static String VA019_ITEMTYPE_Ingredient = "I";/** Modifier = M */
        //        public static String VA019_ITEMTYPE_Modifier = "M";/** Product = P */
        //        public static String VA019_ITEMTYPE_Product = "P";/** Recipe = R */
        //        public static String VA019_ITEMTYPE_Recipe = "R";/** Stocked = S */
        //        public static String VA019_ITEMTYPE_Stocked = "S";/** Is test a valid value.
        //@param test testvalue
        //@returns true if valid **/
        //        public bool IsVA019_ItemTypeValid(String test) { return test == null || test.Equals("C") || test.Equals("I") || test.Equals("M") || test.Equals("P") || test.Equals("R") || test.Equals("S"); }/** Set Item Type.
        //@param VA019_ItemType Item Type */
        //        public void SetVA019_ItemType(String VA019_ItemType)
        //        {
        //            if (!IsVA019_ItemTypeValid(VA019_ItemType))
        //                throw new ArgumentException("VA019_ItemType Invalid value - " + VA019_ItemType + " - Reference_ID=1000239 - C - I - M - P - R - S"); if (VA019_ItemType != null && VA019_ItemType.Length > 1) { log.Warning("Length > 1 - truncated"); VA019_ItemType = VA019_ItemType.Substring(0, 1); } Set_Value("VA019_ItemType", VA019_ItemType);
        //        }/** Get Item Type.
        //@return Item Type */
        //        public String GetVA019_ItemType() { return (String)Get_Value("VA019_ItemType"); }

        /** @param VA019_IsKitchenItem Kitchen Item */
        public void SetVA019_IsKitchenItem(Boolean VA019_IsKitchenItem) { Set_Value("VA019_IsKitchenItem", VA019_IsKitchenItem); }/** Get Kitchen Item.
@return Kitchen Item */
        public Boolean IsVA019_IsKitchenItem() { Object oo = Get_Value("VA019_IsKitchenItem"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set IsRecipe.
@param VA019_IsRecipe IsRecipe */
        public void SetVA019_IsRecipe(Boolean VA019_IsRecipe) { Set_Value("VA019_IsRecipe", VA019_IsRecipe); }/** Get IsRecipe.
@return IsRecipe */
        public Boolean IsVA019_IsRecipe() { Object oo = Get_Value("VA019_IsRecipe"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set MeasurePerPortion.
@param VA019_MeasurePerPortion MeasurePerPortion */
        public void SetVA019_MeasurePerPortion(Decimal? VA019_MeasurePerPortion) { Set_Value("VA019_MeasurePerPortion", (Decimal?)VA019_MeasurePerPortion); }/** Get MeasurePerPortion.
@return MeasurePerPortion */
        public Decimal GetVA019_MeasurePerPortion() { Object bd = Get_Value("VA019_MeasurePerPortion"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Modifiers Set.
@param VA019_ModifiersSet_ID Modifiers Set */
        public void SetVA019_ModifiersSet_ID(int VA019_ModifiersSet_ID)
        {
            if (VA019_ModifiersSet_ID <= 0) Set_Value("VA019_ModifiersSet_ID", null);
            else
                Set_Value("VA019_ModifiersSet_ID", VA019_ModifiersSet_ID);
        }/** Get Modifiers Set.
@return Modifiers Set */
        public int GetVA019_ModifiersSet_ID() { Object ii = Get_Value("VA019_ModifiersSet_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set NDBno.
@param VA019_NDBno   */
        public void SetVA019_NDBno(int VA019_NDBno) { Set_Value("VA019_NDBno", VA019_NDBno); }/** Get NDBno.
@return   */
        public int GetVA019_NDBno() { Object ii = Get_Value("VA019_NDBno"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Portion Cost.
@param VA019_PortionCost Portion Cost */
        public void SetVA019_PortionCost(Decimal? VA019_PortionCost) { Set_Value("VA019_PortionCost", (Decimal?)VA019_PortionCost); }/** Get Portion Cost.
@return Portion Cost */
        public Decimal GetVA019_PortionCost() { Object bd = Get_Value("VA019_PortionCost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Portion In Yield.
@param VA019_PortionInYield Portion In Yield */
        public void SetVA019_PortionInYield(Decimal? VA019_PortionInYield) { Set_Value("VA019_PortionInYield", (Decimal?)VA019_PortionInYield); }/** Get Portion In Yield.
@return Portion In Yield */
        public Decimal GetVA019_PortionInYield() { Object bd = Get_Value("VA019_PortionInYield"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** VA019_PortionUOM AD_Reference_ID=114 */
        public static int VA019_PORTIONUOM_AD_Reference_ID = 114;/** Set Portion UOM.
@param VA019_PortionUOM Portion UOM */
        public void SetVA019_PortionUOM(int VA019_PortionUOM) { Set_Value("VA019_PortionUOM", VA019_PortionUOM); }/** Get Portion UOM.
@return Portion UOM */
        public int GetVA019_PortionUOM() { Object ii = Get_Value("VA019_PortionUOM"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Prepartaion Time(In Minutes).
@param VA019_PrepartaionTime Prepartaion Time(In Minutes) */
        public void SetVA019_PrepartaionTime(int VA019_PrepartaionTime) { Set_Value("VA019_PrepartaionTime", VA019_PrepartaionTime); }/** Get Prepartaion Time(In Minutes).
@return Prepartaion Time(In Minutes) */
        public int GetVA019_PrepartaionTime() { Object ii = Get_Value("VA019_PrepartaionTime"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Yield Cost.
@param VA019_YieldCost Yield Cost */
        public void SetVA019_YieldCost(Decimal? VA019_YieldCost) { Set_Value("VA019_YieldCost", (Decimal?)VA019_YieldCost); }/** Get Yield Cost.
@return Yield Cost */
        public Decimal GetVA019_YieldCost() { Object bd = Get_Value("VA019_YieldCost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Yield Quantity.
@param VA019_YieldQuantity Yield Quantity */
        public void SetVA019_YieldQuantity(Decimal? VA019_YieldQuantity) { Set_Value("VA019_YieldQuantity", (Decimal?)VA019_YieldQuantity); }/** Get Yield Quantity.
@return Yield Quantity */
        public Decimal GetVA019_YieldQuantity() { Object bd = Get_Value("VA019_YieldQuantity"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** VA019_YieldUOM AD_Reference_ID=114 */
        public static int VA019_YIELDUOM_AD_Reference_ID = 114;/** Set Yield UOM.
@param VA019_YieldUOM Yield UOM */
        public void SetVA019_YieldUOM(int VA019_YieldUOM) { Set_Value("VA019_YieldUOM", VA019_YieldUOM); }/** Get Yield UOM.
@return Yield UOM */
        public int GetVA019_YieldUOM() { Object ii = Get_Value("VA019_YieldUOM"); if (ii == null) return 0; return Convert.ToInt32(ii); }


        /** VA019_ItemType AD_Reference_ID=1000239 */
        public static int VA019_ITEMTYPE_AD_Reference_ID = 1000239;/** Combo = C */
        public static String VA019_ITEMTYPE_Combo = "C";/** Ingredient = I */
        public static String VA019_ITEMTYPE_Ingredient = "I";/** Modifier = M */
        public static String VA019_ITEMTYPE_Modifier = "M";/** Product = P */
        public static String VA019_ITEMTYPE_Product = "P";/** Recipe = R */
        public static String VA019_ITEMTYPE_Recipe = "R";/** Stocked = S */
        public static String VA019_ITEMTYPE_Stocked = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA019_ItemTypeValid(String test) { return test == null || test.Equals("C") || test.Equals("I") || test.Equals("M") || test.Equals("P") || test.Equals("R") || test.Equals("S"); }/** Set Item Type.
@param VA019_ItemType Item Type */
        public void SetVA019_ItemType(String VA019_ItemType)
        {
            if (!IsVA019_ItemTypeValid(VA019_ItemType))
                throw new ArgumentException("VA019_ItemType Invalid value - " + VA019_ItemType + " - Reference_ID=1000239 - C - I - M - P - R - S"); if (VA019_ItemType != null && VA019_ItemType.Length > 1) { log.Warning("Length > 1 - truncated"); VA019_ItemType = VA019_ItemType.Substring(0, 1); } Set_Value("VA019_ItemType", VA019_ItemType);
        }/** Get Item Type.
@return Item Type */
        public String GetVA019_ItemType() { return (String)Get_Value("VA019_ItemType"); }

        /** Set Total Cost.
@param VA019_TotalCost Total Cost */
        public void SetVA019_TotalCost(Decimal? VA019_TotalCost) { Set_Value("VA019_TotalCost", (Decimal?)VA019_TotalCost); }/** Get Total Cost.
@return Total Cost */
        public Decimal GetVA019_TotalCost() { Object bd = Get_Value("VA019_TotalCost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }


        /** VA019_RecipeType AD_Reference_ID=1000247 */
        public static int VA019_RECIPETYPE_AD_Reference_ID = 1000247;/** A la carte = AL */
        public static String VA019_RECIPETYPE_ALaCarte = "AL";/** Catring = CA */
        public static String VA019_RECIPETYPE_Catring = "CA";/** Portion = PO */
        public static String VA019_RECIPETYPE_Portion = "PO";/** Pre Production = PP */
        public static String VA019_RECIPETYPE_PreProduction = "PP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA019_RecipeTypeValid(String test) { return test == null || test.Equals("AL") || test.Equals("CA") || test.Equals("PO") || test.Equals("PP"); }/** Set Recipe Type.
@param VA019_RecipeType Recipe Type */
        public void SetVA019_RecipeType(String VA019_RecipeType)
        {
            if (!IsVA019_RecipeTypeValid(VA019_RecipeType))
                throw new ArgumentException("VA019_RecipeType Invalid value - " + VA019_RecipeType + " - Reference_ID=1000247 - AL - CA - PO - PP"); if (VA019_RecipeType != null && VA019_RecipeType.Length > 2) { log.Warning("Length > 2 - truncated"); VA019_RecipeType = VA019_RecipeType.Substring(0, 2); } Set_Value("VA019_RecipeType", VA019_RecipeType);
        }/** Get Recipe Type.
@return Recipe Type */
        public String GetVA019_RecipeType() { return (String)Get_Value("VA019_RecipeType"); }

        /** Set Cost Adjustment on Lost.@param IsCostAdjustmentOnLost Cost Adjustment on Lost */
        public void SetIsCostAdjustmentOnLost(Boolean IsCostAdjustmentOnLost) { Set_Value("IsCostAdjustmentOnLost", IsCostAdjustmentOnLost); }
        /** Get Cost Adjustment on Lost.@return Cost Adjustment on Lost */
        public Boolean IsCostAdjustmentOnLost() { Object oo = Get_Value("IsCostAdjustmentOnLost"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

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
