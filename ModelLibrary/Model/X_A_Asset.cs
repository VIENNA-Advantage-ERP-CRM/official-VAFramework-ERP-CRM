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
    /** Generated Model for A_Asset
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_A_Asset : PO
    {
        public X_A_Asset(Context ctx, int A_Asset_ID, Trx trxName)
            : base(ctx, A_Asset_ID, trxName)
        {
            /** if (A_Asset_ID == 0)
            {
            SetA_Asset_Group_ID (0);
            SetA_Asset_ID (0);
            SetIsDepreciated (false);
            SetIsDisposed (false);
            SetIsFullyDepreciated (false);	// N
            SetIsInPosession (false);
            SetIsOwned (false);
            SetName (null);
            SetValue (null);
            }
             */
        }
        public X_A_Asset(Ctx ctx, int A_Asset_ID, Trx trxName)
            : base(ctx, A_Asset_ID, trxName)
        {
            /** if (A_Asset_ID == 0)
            {
            SetA_Asset_Group_ID (0);
            SetA_Asset_ID (0);
            SetIsDepreciated (false);
            SetIsDisposed (false);
            SetIsFullyDepreciated (false);	// N
            SetIsInPosession (false);
            SetIsOwned (false);
            SetName (null);
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_A_Asset()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514366953L;
        /** Last Updated Timestamp 7/29/2010 1:07:30 PM */
        public static long updatedMS = 1280389050164L;
        /** AD_Table_ID=539 */
        public static int Table_ID;
        // =539;

        /** TableName=A_Asset */
        public static String Table_Name = "A_Asset";

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
            StringBuilder sb = new StringBuilder("X_A_Asset[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Asset Group.
        @param A_Asset_Group_ID Group of Assets */
        public void SetA_Asset_Group_ID(int A_Asset_Group_ID)
        {
            if (A_Asset_Group_ID < 1) throw new ArgumentException("A_Asset_Group_ID is mandatory.");
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
        /** Set Asset.
        @param A_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int A_Asset_ID)
        {
            if (A_Asset_ID < 1) throw new ArgumentException("A_Asset_ID is mandatory.");
            Set_ValueNoCheck("A_Asset_ID", A_Asset_ID);
        }
        /** Get Asset.
        @return Asset used internally or by customers */
        public int GetA_Asset_ID()
        {
            Object ii = Get_Value("A_Asset_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Asset Depreciation Date.
        @param AssetDepreciationDate Date of last depreciation */
        public void SetAssetDepreciationDate(DateTime? AssetDepreciationDate)
        {
            Set_Value("AssetDepreciationDate", (DateTime?)AssetDepreciationDate);
        }
        /** Get Asset Depreciation Date.
        @return Date of last depreciation */
        public DateTime? GetAssetDepreciationDate()
        {
            return (DateTime?)Get_Value("AssetDepreciationDate");
        }
        /** Set Asset Disposal Date.
        @param AssetDisposalDate Date when the asset is/was disposed */
        public void SetAssetDisposalDate(DateTime? AssetDisposalDate)
        {
            Set_Value("AssetDisposalDate", (DateTime?)AssetDisposalDate);
        }
        /** Get Asset Disposal Date.
        @return Date when the asset is/was disposed */
        public DateTime? GetAssetDisposalDate()
        {
            return (DateTime?)Get_Value("AssetDisposalDate");
        }
        /** Set In Service Date.
        @param AssetServiceDate Date when Asset was put into service */
        public void SetAssetServiceDate(DateTime? AssetServiceDate)
        {
            Set_Value("AssetServiceDate", (DateTime?)AssetServiceDate);
        }
        /** Get In Service Date.
        @return Date when Asset was put into service */
        public DateTime? GetAssetServiceDate()
        {
            return (DateTime?)Get_Value("AssetServiceDate");
        }

        /** C_BPartnerSR_ID AD_Reference_ID=353 */
        public static int C_BPARTNERSR_ID_AD_Reference_ID = 353;
        /** Set BPartner (Agent).
        @param C_BPartnerSR_ID Business Partner (Agent or Sales Rep) */
        public void SetC_BPartnerSR_ID(int C_BPartnerSR_ID)
        {
            if (C_BPartnerSR_ID <= 0) Set_Value("C_BPartnerSR_ID", null);
            else
                Set_Value("C_BPartnerSR_ID", C_BPartnerSR_ID);
        }
        /** Get BPartner (Agent).
        @return Business Partner (Agent or Sales Rep) */
        public int GetC_BPartnerSR_ID()
        {
            Object ii = Get_Value("C_BPartnerSR_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Partner Location.
        @param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param C_Location_ID Location or Address */
        public void SetC_Location_ID(int C_Location_ID)
        {
            if (C_Location_ID <= 0) Set_Value("C_Location_ID", null);
            else
                Set_Value("C_Location_ID", C_Location_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetC_Location_ID()
        {
            Object ii = Get_Value("C_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
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
        /** Set Guarantee Date.
        @param GuaranteeDate Date when guarantee expires */
        public void SetGuaranteeDate(DateTime? GuaranteeDate)
        {
            Set_Value("GuaranteeDate", (DateTime?)GuaranteeDate);
        }
        /** Get Guarantee Date.
        @return Date when guarantee expires */
        public DateTime? GetGuaranteeDate()
        {
            return (DateTime?)Get_Value("GuaranteeDate");
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
        /** Set Depreciate.
        @param IsDepreciated The asset will be depreciated */
        public void SetIsDepreciated(Boolean IsDepreciated)
        {
            Set_Value("IsDepreciated", IsDepreciated);
        }
        /** Get Depreciate.
        @return The asset will be depreciated */
        public Boolean IsDepreciated()
        {
            Object oo = Get_Value("IsDepreciated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Disposed.
        @param IsDisposed The asset is disposed */
        public void SetIsDisposed(Boolean IsDisposed)
        {
            Set_Value("IsDisposed", IsDisposed);
        }
        /** Get Disposed.
        @return The asset is disposed */
        public Boolean IsDisposed()
        {
            Object oo = Get_Value("IsDisposed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Fully depreciated.
        @param IsFullyDepreciated The asset is fully depreciated */
        public void SetIsFullyDepreciated(Boolean IsFullyDepreciated)
        {
            Set_ValueNoCheck("IsFullyDepreciated", IsFullyDepreciated);
        }
        /** Get Fully depreciated.
        @return The asset is fully depreciated */
        public Boolean IsFullyDepreciated()
        {
            Object oo = Get_Value("IsFullyDepreciated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set In Possession.
        @param IsInPosession The asset is in the possession of the organization */
        public void SetIsInPosession(Boolean IsInPosession)
        {
            Set_Value("IsInPosession", IsInPosession);
        }
        /** Get In Possession.
        @return The asset is in the possession of the organization */
        public Boolean IsInPosession()
        {
            Object oo = Get_Value("IsInPosession");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Owned.
        @param IsOwned The asset is owned by the organization */
        public void SetIsOwned(Boolean IsOwned)
        {
            Set_Value("IsOwned", IsOwned);
        }
        /** Get Owned.
        @return The asset is owned by the organization */
        public Boolean IsOwned()
        {
            Object oo = Get_Value("IsOwned");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set TrialPhase.
        @param IsTrialPhase This is a trial phase */
        public void SetIsTrialPhase(Boolean IsTrialPhase)
        {
            Set_Value("IsTrialPhase", IsTrialPhase);
        }
        /** Get TrialPhase.
        @return This is a trial phase */
        public Boolean IsTrialPhase()
        {
            Object oo = Get_Value("IsTrialPhase");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Last Maintenance.
        @param LastMaintenanceDate Last Maintenance Date */
        public void SetLastMaintenanceDate(DateTime? LastMaintenanceDate)
        {
            Set_Value("LastMaintenanceDate", (DateTime?)LastMaintenanceDate);
        }
        /** Get Last Maintenance.
        @return Last Maintenance Date */
        public DateTime? GetLastMaintenanceDate()
        {
            return (DateTime?)Get_Value("LastMaintenanceDate");
        }
        /** Set Last Note.
        @param LastMaintenanceNote Last Maintenance Note */
        public void SetLastMaintenanceNote(String LastMaintenanceNote)
        {
            if (LastMaintenanceNote != null && LastMaintenanceNote.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                LastMaintenanceNote = LastMaintenanceNote.Substring(0, 60);
            }
            Set_Value("LastMaintenanceNote", LastMaintenanceNote);
        }
        /** Get Last Note.
        @return Last Maintenance Note */
        public String GetLastMaintenanceNote()
        {
            return (String)Get_Value("LastMaintenanceNote");
        }
        /** Set Last Unit.
        @param LastMaintenanceUnit Last Maintenance Unit */
        public void SetLastMaintenanceUnit(int LastMaintenanceUnit)
        {
            Set_Value("LastMaintenanceUnit", LastMaintenanceUnit);
        }
        /** Get Last Unit.
        @return Last Maintenance Unit */
        public int GetLastMaintenanceUnit()
        {
            Object ii = Get_Value("LastMaintenanceUnit");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Lease Termination.
        @param LeaseTerminationDate Lease Termination Date */
        public void SetLeaseTerminationDate(DateTime? LeaseTerminationDate)
        {
            Set_Value("LeaseTerminationDate", (DateTime?)LeaseTerminationDate);
        }
        /** Get Lease Termination.
        @return Lease Termination Date */
        public DateTime? GetLeaseTerminationDate()
        {
            return (DateTime?)Get_Value("LeaseTerminationDate");
        }

        /** Lease_BPartner_ID AD_Reference_ID=192 */
        public static int LEASE_BPARTNER_ID_AD_Reference_ID = 192;
        /** Set Lessor.
        @param Lease_BPartner_ID The Business Partner who rents or leases */
        public void SetLease_BPartner_ID(int Lease_BPartner_ID)
        {
            if (Lease_BPartner_ID <= 0) Set_Value("Lease_BPartner_ID", null);
            else
                Set_Value("Lease_BPartner_ID", Lease_BPartner_ID);
        }
        /** Get Lessor.
        @return The Business Partner who rents or leases */
        public int GetLease_BPartner_ID()
        {
            Object ii = Get_Value("Lease_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Life use.
        @param LifeUseUnits Units of use until the asset is not usable anymore */
        public void SetLifeUseUnits(Decimal? LifeUseUnits)
        {
            Set_Value("LifeUseUnits", (Decimal?)LifeUseUnits);
        }
        /** Get Life use.
        @return Units of use until the asset is not usable anymore */
        public Decimal GetLifeUseUnits()
        {
            Object bd = Get_Value("LifeUseUnits");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Location comment.
        @param LocationComment Additional comments or remarks concerning the location */
        public void SetLocationComment(String LocationComment)
        {
            if (LocationComment != null && LocationComment.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LocationComment = LocationComment.Substring(0, 255);
            }
            Set_Value("LocationComment", LocationComment);
        }
        /** Get Location comment.
        @return Additional comments or remarks concerning the location */
        public String GetLocationComment()
        {
            return (String)Get_Value("LocationComment");
        }
        /** Set Lot No.
        @param Lot Lot number (alphanumeric) */
        public void SetLot(String Lot)
        {
            if (Lot != null && Lot.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Lot = Lot.Substring(0, 255);
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
            if (M_AttributeSetInstance_ID <= 0) Set_ValueNoCheck("M_AttributeSetInstance_ID", null);
            else
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
            if (M_InOutLine_ID <= 0) Set_Value("M_InOutLine_ID", null);
            else
                Set_Value("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
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
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_ValueNoCheck("M_Product_ID", null);
            else
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
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Name = Name.Substring(0, 200);
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
        /** Set Next Maintenence.
        @param NextMaintenenceDate Next Maintenence Date */
        public void SetNextMaintenenceDate(DateTime? NextMaintenenceDate)
        {
            Set_Value("NextMaintenenceDate", (DateTime?)NextMaintenenceDate);
        }
        /** Get Next Maintenence.
        @return Next Maintenence Date */
        public DateTime? GetNextMaintenenceDate()
        {
            return (DateTime?)Get_Value("NextMaintenenceDate");
        }
        /** Set Next Unit.
        @param NextMaintenenceUnit Next Maintenence Unit */
        public void SetNextMaintenenceUnit(int NextMaintenenceUnit)
        {
            Set_Value("NextMaintenenceUnit", NextMaintenenceUnit);
        }
        /** Get Next Unit.
        @return Next Maintenence Unit */
        public int GetNextMaintenenceUnit()
        {
            Object ii = Get_Value("NextMaintenenceUnit");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Quantity.
        @param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            Set_Value("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Serial No.
        @param SerNo Product Serial Number */
        public void SetSerNo(String SerNo)
        {
            if (SerNo != null && SerNo.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                SerNo = SerNo.Substring(0, 255);
            }
            Set_Value("SerNo", SerNo);
        }
        /** Get Serial No.
        @return Product Serial Number */
        public String GetSerNo()
        {
            return (String)Get_Value("SerNo");
        }

        /** SystemStatus AD_Reference_ID=374 */
        public static int SYSTEMSTATUS_AD_Reference_ID = 374;
        /** Evaluation = E */
        public static String SYSTEMSTATUS_Evaluation = "E";
        /** Implementation = I */
        public static String SYSTEMSTATUS_Implementation = "I";
        /** Production = P */
        public static String SYSTEMSTATUS_Production = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSystemStatusValid(String test)
        {
            return test == null || test.Equals("E") || test.Equals("I") || test.Equals("P");
        }
        /** Set System Status.
        @param SystemStatus Status of the system - Support priority depends on system status */
        public void SetSystemStatus(String SystemStatus)
        {
            if (!IsSystemStatusValid(SystemStatus))
                throw new ArgumentException("SystemStatus Invalid value - " + SystemStatus + " - Reference_ID=374 - E - I - P");
            if (SystemStatus != null && SystemStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SystemStatus = SystemStatus.Substring(0, 1);
            }
            Set_Value("SystemStatus", SystemStatus);
        }
        /** Get System Status.
        @return Status of the system - Support priority depends on system status */
        public String GetSystemStatus()
        {
            return (String)Get_Value("SystemStatus");
        }
        /** Set Usable Life - Months.
        @param UseLifeMonths Months of the usable life of the asset */
        public void SetUseLifeMonths(int UseLifeMonths)
        {
            Set_Value("UseLifeMonths", UseLifeMonths);
        }
        /** Get Usable Life - Months.
        @return Months of the usable life of the asset */
        public int GetUseLifeMonths()
        {
            Object ii = Get_Value("UseLifeMonths");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Usable Life - Years.
        @param UseLifeYears Years of the usable life of the asset */
        public void SetUseLifeYears(int UseLifeYears)
        {
            Set_Value("UseLifeYears", UseLifeYears);
        }
        /** Get Usable Life - Years.
        @return Years of the usable life of the asset */
        public int GetUseLifeYears()
        {
            Object ii = Get_Value("UseLifeYears");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Use units.
        @param UseUnits Currently used units of the assets */
        public void SetUseUnits(Decimal? UseUnits)
        {
            Set_ValueNoCheck("UseUnits", (Decimal?)UseUnits);
        }
        /** Get Use units.
        @return Currently used units of the assets */
        public Decimal GetUseUnits()
        {
            Object bd = Get_Value("UseUnits");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Value = Value.Substring(0, 100);
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

        //Added By Pratap 8-4-2015 VAFAM

        /** Set Expected Scrap Value.
        @param ExpectedScrapValue Expected Scrap Value */
        public void SetExpectedScrapValue(Decimal? ExpectedScrapValue)
        {
            Set_Value("ExpectedScrapValue", (Decimal?)ExpectedScrapValue);
        }
        /** Get Expected Scrap Value.
        @return Expected Scrap Value */
        public Decimal GetExpectedScrapValue()
        {
            Object bd = Get_Value("ExpectedScrapValue");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set SLM Depreciation.
       @param VAFAM_SLMDepreciation SLM Depreciation */
        public void SetVAFAM_SLMDepreciation(Decimal? VAFAM_SLMDepreciation)
        {
            Set_Value("VAFAM_SLMDepreciation", (Decimal?)VAFAM_SLMDepreciation);
        }
        /** Get SLM Depreciation.
        @return SLM Depreciation */
        public Decimal GetVAFAM_SLMDepreciation()
        {
            Object bd = Get_Value("VAFAM_SLMDepreciation");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //End

        /** Set Period. @param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID)
        {
            if (C_Period_ID <= 0) Set_Value("C_Period_ID", null);
            else
                Set_Value("C_Period_ID", C_Period_ID);
        }
        /** Get Period.@return Period of the Calendar */
        public int GetC_Period_ID() { Object ii = Get_Value("C_Period_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Year.@param C_Year_ID Calendar Year */
        public void SetC_Year_ID(int C_Year_ID)
        {
            if (C_Year_ID <= 0) Set_Value("C_Year_ID", null);
            else
                Set_Value("C_Year_ID", C_Year_ID);
        }
        /** Get Year.@return Calendar Year */
        public int GetC_Year_ID() { Object ii = Get_Value("C_Year_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        ////////// Neha 9/March/2017

        /** VAFAM_AssetType AD_Reference_ID=1000348 */
        public static int VAFAM_ASSETTYPE_AD_Reference_ID = 1000348;/** Current Asset = C */
        public static String VAFAM_ASSETTYPE_CurrentAsset = "C";/** Intangible Asset = I */
        public static String VAFAM_ASSETTYPE_IntangibleAsset = "I";/** Tangible Asset = T */
        public static String VAFAM_ASSETTYPE_TangibleAsset = "T";/** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAFAM_AssetTypeValid(String test) { return test == null || test.Equals("C") || test.Equals("I") || test.Equals("T"); }/** Set Asset Type.
        @param VAFAM_AssetType Asset Type */
        public void SetVAFAM_AssetType(String VAFAM_AssetType)
        {
            if (!IsVAFAM_AssetTypeValid(VAFAM_AssetType))
                throw new ArgumentException("VAFAM_AssetType Invalid value - " + VAFAM_AssetType + " - Reference_ID=1000348 - C - I - T"); if (VAFAM_AssetType != null && VAFAM_AssetType.Length > 1) { log.Warning("Length > 1 - truncated"); VAFAM_AssetType = VAFAM_AssetType.Substring(0, 1); } Set_Value("VAFAM_AssetType", VAFAM_AssetType);
        }/** Get Asset Type.
        @return Asset Type */
        public String GetVAFAM_AssetType() { return (String)Get_Value("VAFAM_AssetType"); }

        /** Set Amortization Template.                                                                                                                             * 
         * @param VA038_AmortizationTemplate_ID Amortization Template */
        public void SetVA038_AmortizationTemplate_ID(int VA038_AmortizationTemplate_ID)
        {
            if (VA038_AmortizationTemplate_ID <= 0) Set_Value("VA038_AmortizationTemplate_ID", null);
            else
                Set_Value("VA038_AmortizationTemplate_ID", VA038_AmortizationTemplate_ID);
        }/** Get Amortization Template.
        @return Amortization Template */
        public int GetVA038_AmortizationTemplate_ID() { Object ii = Get_Value("VA038_AmortizationTemplate_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        
        ////////// Neha 9/March/2017

        //@param VAFAM_HistoricalCost Historical Cost */
        public void SetVAFAM_HistoricalCost (Decimal? VAFAM_HistoricalCost){Set_Value ("VAFAM_HistoricalCost", (Decimal?)VAFAM_HistoricalCost);}/** Get Historical Cost.
        @return Historical Cost */
        public Decimal GetVAFAM_HistoricalCost() {Object bd =Get_Value("VAFAM_HistoricalCost");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}//** Set Schedule Generated.



    }

}
