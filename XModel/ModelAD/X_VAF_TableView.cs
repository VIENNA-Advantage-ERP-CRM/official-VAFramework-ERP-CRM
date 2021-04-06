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
    /** Generated Model for VAF_TableView
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_TableView : PO
    {
        /** TableTrxType VAF_Control_Ref_ID=493 */
        public static int TABLETRXTYPE_VAF_Control_Ref_ID = 493;
        /** Mandatory Organization = M */
        public static String TABLETRXTYPE_MandatoryOrganization = "M";
        /** No Organization = N */
        public static String TABLETRXTYPE_NoOrganization = "N";
        /** Optional Organization = O */
        public static String TABLETRXTYPE_OptionalOrganization = "O";


        public X_VAF_TableView(Context ctx, int VAF_TableView_ID, Trx trxName)
            : base(ctx, VAF_TableView_ID, trxName)
        {
            /** if (VAF_TableView_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetAccessLevel (null);	// 4
            SetRecordType (null);	// U
            SetIsChangeLog (false);
            SetIsDeleteable (true);	// Y
            SetIsHighVolume (false);
            SetIsSecurityEnabled (false);
            SetIsView (false);	// N
            SetName (null);
            SetReplicationType (null);	// L
            SetTableName (null);
            }
             */
        }
        public X_VAF_TableView(Ctx ctx, int VAF_TableView_ID, Trx trxName)
            : base(ctx, VAF_TableView_ID, trxName)
        {
            /** if (VAF_TableView_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetAccessLevel (null);	// 4
            SetRecordType (null);	// U
            SetIsChangeLog (false);
            SetIsDeleteable (true);	// Y
            SetIsHighVolume (false);
            SetIsSecurityEnabled (false);
            SetIsView (false);	// N
            SetName (null);
            SetReplicationType (null);	// L
            SetTableName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_TableView(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_TableView(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_TableView(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }


        /** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_TableView()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514364335L;
        /** Last Updated Timestamp 7/29/2010 1:07:27 PM */
        public static long updatedMS = 1280389047546L;
        /** VAF_TableView_ID=100 */
        public static int Table_ID;
        // =100;

        /** TableName=VAF_TableView */
        public static String Table_Name = "VAF_TableView";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(4);
        /** AccessLevel
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
            StringBuilder sb = new StringBuilder("X_VAF_TableView[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID < 1) throw new ArgumentException("VAF_TableView_ID is mandatory.");
            Set_ValueNoCheck("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Dynamic Validation.
        @param VAF_DataVal_Rule_ID Dynamic Validation Rule */
        public void SetVAF_DataVal_Rule_ID(int VAF_DataVal_Rule_ID)
        {
            if (VAF_DataVal_Rule_ID <= 0) Set_Value("VAF_DataVal_Rule_ID", null);
            else
                Set_Value("VAF_DataVal_Rule_ID", VAF_DataVal_Rule_ID);
        }
        /** Get Dynamic Validation.
        @return Dynamic Validation Rule */
        public int GetVAF_DataVal_Rule_ID()
        {
            Object ii = Get_Value("VAF_DataVal_Rule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Window.
        @param VAF_Screen_ID Data entry or display window */
        public void SetVAF_Screen_ID(int VAF_Screen_ID)
        {
            if (VAF_Screen_ID <= 0) Set_Value("VAF_Screen_ID", null);
            else
                Set_Value("VAF_Screen_ID", VAF_Screen_ID);
        }
        /** Get Window.
        @return Data entry or display window */
        public int GetVAF_Screen_ID()
        {
            Object ii = Get_Value("VAF_Screen_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AccessLevel VAF_Control_Ref_ID=5 */
        public static int ACCESSLEVEL_VAF_Control_Ref_ID = 5;
        /** Organization = 1 */
        public static String ACCESSLEVEL_Organization = "1";
        /** Client only = 2 */
        public static String ACCESSLEVEL_ClientOnly = "2";
        /** Client+Organization = 3 */
        public static String ACCESSLEVEL_ClientPlusOrganization = "3";
        /** System only = 4 */
        public static String ACCESSLEVEL_SystemOnly = "4";
        /** System+Client = 6 */
        public static String ACCESSLEVEL_SystemPlusClient = "6";
        /** All = 7 */
        public static String ACCESSLEVEL_All = "7";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsAccessLevelValid(String test)
        {
            return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("6") || test.Equals("7");
        }
        /** Set Data Access Level.
        @param AccessLevel Access Level required */
        public void SetAccessLevel(String AccessLevel)
        {
            if (AccessLevel == null) throw new ArgumentException("AccessLevel is mandatory");
            if (!IsAccessLevelValid(AccessLevel))
                throw new ArgumentException("AccessLevel Invalid value - " + AccessLevel + " - Reference_ID=5 - 1 - 2 - 3 - 4 - 6 - 7");
            if (AccessLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AccessLevel = AccessLevel.Substring(0, 1);
            }
            Set_Value("AccessLevel", AccessLevel);
        }
        /** Get Data Access Level.
        @return Access Level required */
        public String GetAccessLevel()
        {
            return (String)Get_Value("AccessLevel");
        }
        /** Set Constraint Name.
        @param ConstraintName Constraint Name */
        public void SetConstraintName(String ConstraintName)
        {
            throw new ArgumentException("ConstraintName Is virtual column");
        }
        /** Get Constraint Name.
        @return Constraint Name */
        public String GetConstraintName()
        {
            return (String)Get_Value("ConstraintName");
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

        /** RecordType VAF_Control_Ref_ID=389 */
        public static int RecordType_VAF_Control_Ref_ID = 389;
        /** Set Entity Type.
        @param RecordType Dictionary Entity Type;
         Determines ownership and synchronization */
        public void SetRecordType(String RecordType)
        {
            if (RecordType.Length > 4)
            {
                log.Warning("Length > 4 - truncated");
                RecordType = RecordType.Substring(0, 4);
            }
            Set_Value("RecordType", RecordType);
        }
        /** Get Entity Type.
        @return Dictionary Entity Type;
         Determines ownership and synchronization */
        public String GetRecordType()
        {
            return (String)Get_Value("RecordType");
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
        /** Set Import Table.
        @param ImportTable Import Table Columns from Database */
        public void SetImportTable(String ImportTable)
        {
            if (ImportTable != null && ImportTable.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ImportTable = ImportTable.Substring(0, 1);
            }
            Set_Value("ImportTable", ImportTable);
        }
        /** Get Import Table.
        @return Import Table Columns from Database */
        public String GetImportTable()
        {
            return (String)Get_Value("ImportTable");
        }
        /** Set Maintain Change Log.
        @param IsChangeLog Maintain a log of changes */
        public void SetIsChangeLog(Boolean IsChangeLog)
        {
            Set_Value("IsChangeLog", IsChangeLog);
        }
        /** Get Maintain Change Log.
        @return Maintain a log of changes */
        public Boolean IsChangeLog()
        {
            Object oo = Get_Value("IsChangeLog");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Records deleteable.
        @param IsDeleteable Indicates if records can be deleted from the database */
        public void SetIsDeleteable(Boolean IsDeleteable)
        {
            Set_Value("IsDeleteable", IsDeleteable);
        }
        /** Get Records deleteable.
        @return Indicates if records can be deleted from the database */
        public Boolean IsDeleteable()
        {
            Object oo = Get_Value("IsDeleteable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set High Volume.
        @param IsHighVolume Use Search instead of Pick list */
        public void SetIsHighVolume(Boolean IsHighVolume)
        {
            Set_Value("IsHighVolume", IsHighVolume);
        }
        /** Get High Volume.
        @return Use Search instead of Pick list */
        public Boolean IsHighVolume()
        {
            Object oo = Get_Value("IsHighVolume");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Security enabled.
        @param IsSecurityEnabled If security is enabled, user access to data can be restricted via Roles */
        public void SetIsSecurityEnabled(Boolean IsSecurityEnabled)
        {
            Set_Value("IsSecurityEnabled", IsSecurityEnabled);
        }
        /** Get Security enabled.
        @return If security is enabled, user access to data can be restricted via Roles */
        public Boolean IsSecurityEnabled()
        {
            Object oo = Get_Value("IsSecurityEnabled");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set View.
        @param IsView This is a view */
        public void SetIsView(Boolean IsView)
        {
            Set_Value("IsView", IsView);
        }
        /** Get View.
        @return This is a view */
        public Boolean IsView()
        {
            Object oo = Get_Value("IsView");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sequence.
        @param LoadSeq Sequence */
        public void SetLoadSeq(int LoadSeq)
        {
            Set_ValueNoCheck("LoadSeq", LoadSeq);
        }
        /** Get Sequence.
        @return Sequence */
        public int GetLoadSeq()
        {
            Object ii = Get_Value("LoadSeq");
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

        /** PO_Window_ID VAF_Control_Ref_ID=284 */
        public static int PO_WINDOW_ID_VAF_Control_Ref_ID = 284;
        /** Set PO Window.
        @param PO_Window_ID Purchase Order Window */
        public void SetPO_Window_ID(int PO_Window_ID)
        {
            if (PO_Window_ID <= 0) Set_Value("PO_Window_ID", null);
            else
                Set_Value("PO_Window_ID", PO_Window_ID);
        }
        /** Get PO Window.
        @return Purchase Order Window */
        public int GetPO_Window_ID()
        {
            Object ii = Get_Value("PO_Window_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Referenced_Table_ID VAF_Control_Ref_ID=415 */
        public static int REFERENCED_TABLE_ID_VAF_Control_Ref_ID = 415;
        /** Set Referenced Table.
        @param Referenced_Table_ID Referenced Table */
        public void SetReferenced_Table_ID(int Referenced_Table_ID)
        {
            if (Referenced_Table_ID <= 0) Set_Value("Referenced_Table_ID", null);
            else
                Set_Value("Referenced_Table_ID", Referenced_Table_ID);
        }
        /** Get Referenced Table.
        @return Referenced Table */
        public int GetReferenced_Table_ID()
        {
            Object ii = Get_Value("Referenced_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** ReplicationType VAF_Control_Ref_ID=126 */
        public static int REPLICATIONTYPE_VAF_Control_Ref_ID = 126;
        /** Local = L */
        public static String REPLICATIONTYPE_Local = "L";
        /** Merge = M */
        public static String REPLICATIONTYPE_Merge = "M";
        /** Reference = R */
        public static String REPLICATIONTYPE_Reference = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsReplicationTypeValid(String test)
        {
            return test.Equals("L") || test.Equals("M") || test.Equals("R");
        }
        /** Set Replication Type.
        @param ReplicationType Type of Data Replication */
        public void SetReplicationType(String ReplicationType)
        {
            if (ReplicationType == null) throw new ArgumentException("ReplicationType is mandatory");
            if (!IsReplicationTypeValid(ReplicationType))
                throw new ArgumentException("ReplicationType Invalid value - " + ReplicationType + " - Reference_ID=126 - L - M - R");
            if (ReplicationType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ReplicationType = ReplicationType.Substring(0, 1);
            }
            Set_Value("ReplicationType", ReplicationType);
        }
        /** Get Replication Type.
        @return Type of Data Replication */
        public String GetReplicationType()
        {
            return (String)Get_Value("ReplicationType");
        }
        /** Set DB Table Name.
        @param TableName Name of the table in the database */
        public void SetTableName(String TableName)
        {
            if (TableName == null) throw new ArgumentException("TableName is mandatory.");
            if (TableName.Length > 25)
            {
                log.Warning("Length > 25 - truncated");
                TableName = TableName.Substring(0, 25);
            }
            Set_Value("TableName", TableName);
        }
        /** Get DB Table Name.
        @return Name of the table in the database */
        public String GetTableName()
        {
            return (String)Get_Value("TableName");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetTableName());
        }


        /** Base_Table_ID VAF_Control_Ref_ID=415 */
        public static int BASE_TABLE_ID_VAF_Control_Ref_ID = 415;
        /** Set Base Table.
        @param Base_Table_ID Base Table for Sub-Tables */
        public void SetBase_Table_ID(int Base_Table_ID)
        {
            if (Base_Table_ID <= 0)
                Set_Value("Base_Table_ID", null);
            else
                Set_Value("Base_Table_ID", (int)Base_Table_ID);

        }
        /** SubTableType VAF_Control_Ref_ID=447 */
        public static int SUBTABLETYPE_VAF_Control_Ref_ID = 447;
        /** History - Daily = D */
        public static String SUBTABLETYPE_History_Daily = X_Ref_VAF_TableView_SubTableType.HISTORY__DAILY;
        /** History - Each = E */
        public static String SUBTABLETYPE_History_Each = X_Ref_VAF_TableView_SubTableType.HISTORY__EACH;
        /** Delta - System = S */
        public static String SUBTABLETYPE_Delta_System = X_Ref_VAF_TableView_SubTableType.DELTA__SYSTEM;
        /** Delta - User = U */
        public static String SUBTABLETYPE_Delta_User = X_Ref_VAF_TableView_SubTableType.DELTA__USER;


        public static bool IsSubTableTypeValid(String test)
        {
            return X_Ref_VAF_TableView_SubTableType.IsValid(test);

        }

        public void SetSubTableType(String SubTableType)
        {
            if (!IsSubTableTypeValid(SubTableType))
                throw new ArgumentException("SubTableType Invalid value - " + SubTableType + " - Reference_ID=447 - D - E - S - U");
            Set_Value("SubTableType", SubTableType);

        }

        /** Get Sub Table Type.
         @return Type of Sub-Table */
        public String GetSubTableType()
        {
            return (String)Get_Value("SubTableType");

        }
        ///Set Transaction Type.
        ///Table Transaction Type
        /// <writer>Raghu</writer>
        /// <Date>04-March-2011</Date>
        public void SetTableTrxType(String TableTrxType)
        {
            if (TableTrxType == null) throw new ArgumentException("TableTrxType is mandatory");
            if (!IsTableTrxTypeValid(TableTrxType))
                throw new ArgumentException("TableTrxType Invalid value - " + TableTrxType + " - Reference_ID=493 - M - N - O");
            if (TableTrxType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                TableTrxType = TableTrxType.Substring(0, (TableTrxType.Length - 1) - TableTrxType.Length);
            }
            Set_Value("TableTrxType", TableTrxType);
        }

        /** Get Transaction Type.
        @return Table Transaction Type */
        /// <writer>Raghu</writer>
        /// <Date>04-March-2011</Date>
        public String GetTableTrxType()
        {
            return (String)Get_Value("TableTrxType");
        }

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        /// <writer>Raghu</writer>
        /// <Date>04-March-2011</Date>
        public bool IsTableTrxTypeValid(String test)
        {
            return test.Equals("M") || test.Equals("N") || test.Equals("O");
        }

        /** Set Maintain Versions.
       @param IsMaintainVersions Versioning will be maintained for the columns marked as maintain versions */
        public void SetIsMaintainVersions(Boolean IsMaintainVersions)
        {
            Set_Value("IsMaintainVersions", IsMaintainVersions);
        }
        /** Get Maintain Versions.
        @return Versioning will be maintained for the columns marked as maintain versions */
        public Boolean IsMaintainVersions()
        {
            Object oo = Get_Value("IsMaintainVersions");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool))
                    return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

    public class X_Ref_VAF_TableView_SubTableType
    {
        /** History - Daily = D */
        public static string HISTORY__DAILY = "D";
        /** History - Each = E */
        public static string HISTORY__EACH = "E";
        /** Delta - System = S */
        public static string DELTA__SYSTEM = "S";
        /** Delta - User = U */
        public static string DELTA__USER = "U";


        public static int VAF_Control_Ref_ID = 447;
        private String value;
        private X_Ref_VAF_TableView_SubTableType(String value)
        {
            this.value = value;

        }

        public String GetValue()
        {
            return this.value;

        }

        public static bool IsValid(String test)
        {
            if (test == null) return true;

            if (X_Ref_VAF_TableView_SubTableType.HISTORY__DAILY == test)
                return true;
            if (X_Ref_VAF_TableView_SubTableType.HISTORY__EACH == test)
                return true;
            if (X_Ref_VAF_TableView_SubTableType.DELTA__SYSTEM == test)
                return true;
            if (X_Ref_VAF_TableView_SubTableType.DELTA__USER == test)
                return true;

            return false;
        }       
    }
}
