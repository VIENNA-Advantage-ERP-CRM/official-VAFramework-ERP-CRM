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
    /** Generated Model for AD_ModuleInfo
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_ModuleInfo : PO
    {
        public X_AD_ModuleInfo(Context ctx, int AD_ModuleInfo_ID, Trx trxName)
            : base(ctx, AD_ModuleInfo_ID, trxName)
        {
            /** if (AD_ModuleInfo_ID == 0)
            {
            SetAD_ModuleInfo_ID (0);
            SetAssemblyName (null);
            SetNameSpace (null);
            SetPrefix (null);
            SetVersionID (0.0);
            SetVersionNo (null);
            }
             */
        }
        public X_AD_ModuleInfo(Ctx ctx, int AD_ModuleInfo_ID, Trx trxName)
            : base(ctx, AD_ModuleInfo_ID, trxName)
        {
            /** if (AD_ModuleInfo_ID == 0)
            {
            SetAD_ModuleInfo_ID (0);
            SetAssemblyName (null);
            SetNameSpace (null);
            SetPrefix (null);
            SetVersionID (0.0);
            SetVersionNo (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ModuleInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_ModuleInfo()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27631471166925L;
        /** Last Updated Timestamp 10/4/2012 3:47:30 PM */
        public static long updatedMS = 1349345850136L;
        /** AD_Table_ID=1000350 */
        public static int Table_ID;
        // =1000350;

        /** TableName=AD_ModuleInfo */
        public static String Table_Name = "AD_ModuleInfo";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(4);
        /** AccessLevel
        @return 4 - System 
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
            StringBuilder sb = new StringBuilder("X_AD_ModuleInfo[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set AD_ModuleCategory_ID.
        @param AD_ModuleCategory_ID AD_ModuleCategory_ID */
        public void SetAD_ModuleCategory_ID(int AD_ModuleCategory_ID)
        {
            if (AD_ModuleCategory_ID <= 0) Set_Value("AD_ModuleCategory_ID", null);
            else
                Set_Value("AD_ModuleCategory_ID", AD_ModuleCategory_ID);
        }
        /** Get AD_ModuleCategory_ID.
        @return AD_ModuleCategory_ID */
        public int GetAD_ModuleCategory_ID()
        {
            Object ii = Get_Value("AD_ModuleCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Module.
        @param AD_ModuleInfo_ID Module */
        public void SetAD_ModuleInfo_ID(int AD_ModuleInfo_ID)
        {
            if (AD_ModuleInfo_ID < 1) throw new ArgumentException("AD_ModuleInfo_ID is mandatory.");
            Set_ValueNoCheck("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
        }
        /** Get Module.
        @return Module */
        public int GetAD_ModuleInfo_ID()
        {
            Object ii = Get_Value("AD_ModuleInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AssemblyName.
        @param AssemblyName AssemblyName */
        public void SetAssemblyName(String AssemblyName)
        {
            if (AssemblyName == null) throw new ArgumentException("AssemblyName is mandatory.");
            if (AssemblyName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                AssemblyName = AssemblyName.Substring(0, 50);
            }
            Set_Value("AssemblyName", AssemblyName);
        }
        /** Get AssemblyName.
        @return AssemblyName */
        public String GetAssemblyName()
        {
            return (String)Get_Value("AssemblyName");
        }
        /** Set Author.
        @param Author Author/Creator of the Entity */
        public void SetAuthor(String Author)
        {
            if (Author != null && Author.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Author = Author.Substring(0, 100);
            }
            Set_Value("Author", Author);
        }
        /** Get Author.
        @return Author/Creator of the Entity */
        public String GetAuthor()
        {
            return (String)Get_Value("Author");
        }
        /** Set BinaryData.
        @param BinaryData Binary Data */
        public void SetBinaryData(Byte[] BinaryData)
        {
            Set_Value("BinaryData", BinaryData);
        }
        /** Get BinaryData.
        @return Binary Data */
        public Byte[] GetBinaryData()
        {
            return (Byte[])Get_Value("BinaryData");
        }
        /** Set Business Partner .
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner .
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Prepare Export Database Schema.
        @param DatabaseSchema Prepare Export Database Schema */
        public void SetDatabaseSchema(String DatabaseSchema)
        {
            if (DatabaseSchema != null && DatabaseSchema.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                DatabaseSchema = DatabaseSchema.Substring(0, 50);
            }
            Set_Value("DatabaseSchema", DatabaseSchema);
        }
        /** Get Prepare Export Database Schema.
        @return Prepare Export Database Schema */
        public String GetDatabaseSchema()
        {
            return (String)Get_Value("DatabaseSchema");
        }
        /** Set DependencyInfo.
        @param DependencyInfo DependencyInfo */
        public void SetDependencyInfo(String DependencyInfo)
        {
            if (DependencyInfo != null && DependencyInfo.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                DependencyInfo = DependencyInfo.Substring(0, 200);
            }
            Set_Value("DependencyInfo", DependencyInfo);
        }
        /** Get DependencyInfo.
        @return DependencyInfo */
        public String GetDependencyInfo()
        {
            return (String)Get_Value("DependencyInfo");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Description = Description.Substring(0, 200);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set DevelopmentStatus.
        @param DevelopmentStatus DevelopmentStatus */
        public void SetDevelopmentStatus(String DevelopmentStatus)
        {
            if (DevelopmentStatus != null && DevelopmentStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DevelopmentStatus = DevelopmentStatus.Substring(0, 2);
            }
            Set_Value("DevelopmentStatus", DevelopmentStatus);
        }
        /** Get DevelopmentStatus.
        @return DevelopmentStatus */
        public String GetDevelopmentStatus()
        {
            return (String)Get_Value("DevelopmentStatus");
        }
        /** Set EstimatedDay.
        @param EstimatedDay EstimatedDay */
        public void SetEstimatedDay(int EstimatedDay)
        {
            Set_Value("EstimatedDay", EstimatedDay);
        }
        /** Get EstimatedDay.
        @return EstimatedDay */
        public int GetEstimatedDay()
        {
            Object ii = Get_Value("EstimatedDay");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Estimated Development Cost.
        @param Estimated_D_Cost Estimated Development Cost */
        public void SetEstimated_D_Cost(Decimal? Estimated_D_Cost)
        {
            Set_Value("Estimated_D_Cost", (Decimal?)Estimated_D_Cost);
        }
        /** Get Estimated Development Cost.
        @return Estimated Development Cost */
        public Decimal GetEstimated_D_Cost()
        {
            Object bd = Get_Value("Estimated_D_Cost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Estimated Feasibility Study Cost.
        @param Estimated_FS_Cost Estimated Feasibility Study Cost */
        public void SetEstimated_FS_Cost(Decimal? Estimated_FS_Cost)
        {
            Set_Value("Estimated_FS_Cost", (Decimal?)Estimated_FS_Cost);
        }
        /** Get Estimated Feasibility Study Cost.
        @return Estimated Feasibility Study Cost */
        public Decimal GetEstimated_FS_Cost()
        {
            Object bd = Get_Value("Estimated_FS_Cost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Feature.
        @param Feature Feature */
        public void SetFeature(String Feature)
        {
            if (Feature != null && Feature.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                Feature = Feature.Substring(0, 1000);
            }
            Set_Value("Feature", Feature);
        }
        /** Get Feature.
        @return Feature */
        public String GetFeature()
        {
            return (String)Get_Value("Feature");
        }
        /** Set InstallationInstruction.
        @param InstallationInstruction InstallationInstruction */
        public void SetInstallationInstruction(String InstallationInstruction)
        {
            if (InstallationInstruction != null && InstallationInstruction.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                InstallationInstruction = InstallationInstruction.Substring(0, 1000);
            }
            Set_Value("InstallationInstruction", InstallationInstruction);
        }
        /** Get InstallationInstruction.
        @return InstallationInstruction */
        public String GetInstallationInstruction()
        {
            return (String)Get_Value("InstallationInstruction");
        }
        /** Set IsFree.
        @param IsFree IsFree */
        public void SetIsFree(Boolean IsFree)
        {
            Set_Value("IsFree", IsFree);
        }
        /** Get IsFree.
        @return IsFree */
        public Boolean IsFree()
        {
            Object oo = Get_Value("IsFree");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Paid.
        @param IsPaid The document is paid */
        public void SetIsPaid(Boolean IsPaid)
        {
            Set_Value("IsPaid", IsPaid);
        }
        /** Get Paid.
        @return The document is paid */
        public Boolean IsPaid()
        {
            Object oo = Get_Value("IsPaid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsPlanned.
        @param IsPlanned IsPlanned */
        public void SetIsPlanned(Boolean IsPlanned)
        {
            Set_Value("IsPlanned", IsPlanned);
        }
        /** Get IsPlanned.
        @return IsPlanned */
        public Boolean IsPlanned()
        {
            Object oo = Get_Value("IsPlanned");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set ProfessionalFree.
        @param IsProfessionalFree ProfessionalFree */
        public void SetIsProfessionalFree(Boolean IsProfessionalFree)
        {
            Set_Value("IsProfessionalFree", IsProfessionalFree);
        }
        /** Get ProfessionalFree.
        @return ProfessionalFree */
        public Boolean IsProfessionalFree()
        {
            Object oo = Get_Value("IsProfessionalFree");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set ProfessionalPaid.
        @param IsProfessionalPaid ProfessionalPaid */
        public void SetIsProfessionalPaid(Boolean IsProfessionalPaid)
        {
            Set_Value("IsProfessionalPaid", IsProfessionalPaid);
        }
        /** Get ProfessionalPaid.
        @return ProfessionalPaid */
        public Boolean IsProfessionalPaid()
        {
            Object oo = Get_Value("IsProfessionalPaid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Released.
        @param IsReleased Released */
        public void SetIsReleased(Boolean IsReleased)
        {
            Set_Value("IsReleased", IsReleased);
        }
        /** Get Released.
        @return Released */
        public Boolean IsReleased()
        {
            Object oo = Get_Value("IsReleased");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }


        /** Set BetaVersion .
        @param IsBeta BetaVersion*/
        public void SetIsBeta(Boolean IsBeta)
        {
            Set_Value("IsBeta", IsBeta);
        }
        /** Get Released.
        @return Released */
        public Boolean IsBeta()
        {
            Object oo = Get_Value("IsBeta");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set LogSummary.
        @param LogSummary LogSummary */
        public void SetLogSummary(String LogSummary)
        {
            Set_Value("LogSummary", LogSummary);
        }
        /** Get LogSummary.
        @return LogSummary */
        public String GetLogSummary()
        {
            return (String)Get_Value("LogSummary");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Name = Name.Substring(0, 100);
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
        /** Set NameSpace.
        @param NameSpace NameSpace */
        public void SetNameSpace(String NameSpace)
        {
            if (NameSpace == null) throw new ArgumentException("NameSpace is mandatory.");
            if (NameSpace.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                NameSpace = NameSpace.Substring(0, 50);
            }
            Set_Value("NameSpace", NameSpace);
        }
        /** Get NameSpace.
        @return NameSpace */
        public String GetNameSpace()
        {
            return (String)Get_Value("NameSpace");
        }
        /** Set Prefix Folder Path.
        @param Path Prefix Folder Path */
        public void SetPath(String Path)
        {
            if (Path != null && Path.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Path = Path.Substring(0, 200);
            }
            Set_Value("Path", Path);
        }
        /** Get Prefix Folder Path.
        @return Prefix Folder Path */
        public String GetPath()
        {
            return (String)Get_Value("Path");
        }
        /** Set Prefix.
        @param Prefix Prefix before the sequence number */
        public void SetPrefix(String Prefix)
        {
            if (Prefix == null) throw new ArgumentException("Prefix is mandatory.");
            if (Prefix.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Prefix = Prefix.Substring(0, 50);
            }
            Set_Value("Prefix", Prefix);
        }
        /** Get Prefix.
        @return Prefix before the sequence number */
        public String GetPrefix()
        {
            return (String)Get_Value("Prefix");
        }
        /** Set Rating.
        @param Rating Classification or Importance */
        public void SetRating(int Rating)
        {
            Set_Value("Rating", Rating);
        }
        /** Get Rating.
        @return Classification or Importance */
        public int GetRating()
        {
            Object ii = Get_Value("Rating");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Received Development Cost.
        @param Received_D_Cost Received Development Cost */
        public void SetReceived_D_Cost(Decimal? Received_D_Cost)
        {
            Set_Value("Received_D_Cost", (Decimal?)Received_D_Cost);
        }
        /** Get Received Development Cost.
        @return Received Development Cost */
        public Decimal GetReceived_D_Cost()
        {
            Object bd = Get_Value("Received_D_Cost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Received Feasibility Study Cost.
        @param Received_FS_Cost Received Feasibility Study Cost */
        public void SetReceived_FS_Cost(Decimal? Received_FS_Cost)
        {
            Set_Value("Received_FS_Cost", (Decimal?)Received_FS_Cost);
        }
        /** Get Received Feasibility Study Cost.
        @return Received Feasibility Study Cost */
        public Decimal GetReceived_FS_Cost()
        {
            Object bd = Get_Value("Received_FS_Cost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set SubscriptionDay.
        @param SubscriptionDay SubscriptionDay */
        public void SetSubscriptionDay(int SubscriptionDay)
        {
            Set_Value("SubscriptionDay", SubscriptionDay);
        }
        /** Get SubscriptionDay.
        @return SubscriptionDay */
        public int GetSubscriptionDay()
        {
            Object ii = Get_Value("SubscriptionDay");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Term & Condition.
        @param TermCondition Term & Condition */
        public void SetTermCondition(String TermCondition)
        {
            if (TermCondition != null && TermCondition.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                TermCondition = TermCondition.Substring(0, 200);
            }
            Set_Value("TermCondition", TermCondition);
        }
        /** Get Term & Condition.
        @return Term & Condition */
        public String GetTermCondition()
        {
            return (String)Get_Value("TermCondition");
        }
        /** Set URL.
        @param URL Full URL address - e.g. http://www.viennasolutions.com */
        public void SetURL(String URL)
        {
            if (URL != null && URL.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                URL = URL.Substring(0, 100);
            }
            Set_Value("URL", URL);
        }
        /** Get URL.
        @return Full URL address - e.g. http://www.viennasolutions.com */
        public String GetURL()
        {
            return (String)Get_Value("URL");
        }
        /** Set UpgradeInfo.
        @param UpgradeInfo UpgradeInfo */
        public void SetUpgradeInfo(String UpgradeInfo)
        {
            if (UpgradeInfo != null && UpgradeInfo.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                UpgradeInfo = UpgradeInfo.Substring(0, 200);
            }
            Set_Value("UpgradeInfo", UpgradeInfo);
        }
        /** Get UpgradeInfo.
        @return UpgradeInfo */
        public String GetUpgradeInfo()
        {
            return (String)Get_Value("UpgradeInfo");
        }
        /** Set Version ID.
        @param VersionID Version ID */
        public void SetVersionID(Decimal? VersionID)
        {
            if (VersionID == null) throw new ArgumentException("VersionID is mandatory.");
            Set_Value("VersionID", (Decimal?)VersionID);
        }
        /** Get Version ID.
        @return Version ID */
        public Decimal GetVersionID()
        {
            Object bd = Get_Value("VersionID");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Version No.
        @param VersionNo Version Number */
        public void SetVersionNo(String VersionNo)
        {
            if (VersionNo == null) throw new ArgumentException("VersionNo is mandatory.");
            if (VersionNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VersionNo = VersionNo.Substring(0, 50);
            }
            Set_Value("VersionNo", VersionNo);
        }
        /** Get Version No.
        @return Version Number */
        public String GetVersionNo()
        {
            return (String)Get_Value("VersionNo");
        }
    }

}
