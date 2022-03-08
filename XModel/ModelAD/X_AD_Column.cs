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
    /** Generated Model for AD_Column
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Column : PO
    {
        public X_AD_Column(Context ctx, int AD_Column_ID, Trx trxName) : base(ctx, AD_Column_ID, trxName)
        {
            /** if (AD_Column_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Element_ID (0);
SetAD_Reference_ID (0);
SetAD_Table_ID (0);
SetColumnName (null);
SetEntityType (null);	// U
SetIsAlwaysUpdateable (false);	// N
SetIsEncrypted (null);	// N
SetIsIdentifier (false);
SetIsKey (false);
SetIsMandatory (false);
SetIsMandatoryUI (false);
SetIsParent (false);
SetIsSelectionColumn (false);
SetIsTranslated (false);
SetIsUpdateable (true);	// Y
SetName (null);
SetVersion (0.0);
}
             */
        }
        public X_AD_Column(Ctx ctx, int AD_Column_ID, Trx trxName) : base(ctx, AD_Column_ID, trxName)
        {
            /** if (AD_Column_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Element_ID (0);
SetAD_Reference_ID (0);
SetAD_Table_ID (0);
SetColumnName (null);
SetEntityType (null);	// U
SetIsAlwaysUpdateable (false);	// N
SetIsEncrypted (null);	// N
SetIsIdentifier (false);
SetIsKey (false);
SetIsMandatory (false);
SetIsMandatoryUI (false);
SetIsParent (false);
SetIsSelectionColumn (false);
SetIsTranslated (false);
SetIsUpdateable (true);	// Y
SetName (null);
SetVersion (0.0);
}
             */
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_Column(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_Column(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_Column(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Column()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514360997L;
        /** Last Updated Timestamp 7/29/2010 1:07:24 PM */
        public static long updatedMS = 1280389044208L;
        /** AD_Table_ID=101 */
        public static int Table_ID;
        // =101;

        /** TableName=AD_Column */
        public static String Table_Name = "AD_Column";

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
            StringBuilder sb = new StringBuilder("X_AD_Column[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Column.
@param AD_Column_ID Column in the table */
        public void SetAD_Column_ID(int AD_Column_ID)
        {
            if (AD_Column_ID < 1) throw new ArgumentException("AD_Column_ID is mandatory.");
            Set_ValueNoCheck("AD_Column_ID", AD_Column_ID);
        }
        /** Get Column.
@return Column in the table */
        public int GetAD_Column_ID()
        {
            Object ii = Get_Value("AD_Column_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set System Element.
@param AD_Element_ID System Element enables the central maintenance of column description and help. */
        public void SetAD_Element_ID(int AD_Element_ID)
        {
            if (AD_Element_ID < 1) throw new ArgumentException("AD_Element_ID is mandatory.");
            Set_Value("AD_Element_ID", AD_Element_ID);
        }
        /** Get System Element.
@return System Element enables the central maintenance of column description and help. */
        public int GetAD_Element_ID()
        {
            Object ii = Get_Value("AD_Element_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process.
@param AD_Process_ID Process or Report */
        public void SetAD_Process_ID(int AD_Process_ID)
        {
            if (AD_Process_ID <= 0) Set_Value("AD_Process_ID", null);
            else
                Set_Value("AD_Process_ID", AD_Process_ID);
        }
        /** Get Process.
@return Process or Report */
        public int GetAD_Process_ID()
        {
            Object ii = Get_Value("AD_Process_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Reference_ID AD_Reference_ID=1 */
        public static int AD_REFERENCE_ID_AD_Reference_ID = 1;
        /** Set Reference.
@param AD_Reference_ID System Reference and Validation */
        public void SetAD_Reference_ID(int AD_Reference_ID)
        {
            if (AD_Reference_ID < 1) throw new ArgumentException("AD_Reference_ID is mandatory.");
            Set_Value("AD_Reference_ID", AD_Reference_ID);
        }
        /** Get Reference.
@return System Reference and Validation */
        public int GetAD_Reference_ID()
        {
            Object ii = Get_Value("AD_Reference_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Reference_Value_ID AD_Reference_ID=4 */
        public static int AD_REFERENCE_VALUE_ID_AD_Reference_ID = 4;
        /** Set Reference Key.
@param AD_Reference_Value_ID Required to specify, if data type is Table or List */
        public void SetAD_Reference_Value_ID(int AD_Reference_Value_ID)
        {
            if (AD_Reference_Value_ID <= 0) Set_Value("AD_Reference_Value_ID", null);
            else
                Set_Value("AD_Reference_Value_ID", AD_Reference_Value_ID);
        }
        /** Get Reference Key.
@return Required to specify, if data type is Table or List */
        public int GetAD_Reference_Value_ID()
        {
            Object ii = Get_Value("AD_Reference_Value_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
@param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID < 1) throw new ArgumentException("AD_Table_ID is mandatory.");
            Set_ValueNoCheck("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
@return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Dynamic Validation.
@param AD_Val_Rule_ID Dynamic Validation Rule */
        public void SetAD_Val_Rule_ID(int AD_Val_Rule_ID)
        {
            if (AD_Val_Rule_ID <= 0) Set_Value("AD_Val_Rule_ID", null);
            else
                Set_Value("AD_Val_Rule_ID", AD_Val_Rule_ID);
        }
        /** Get Dynamic Validation.
@return Dynamic Validation Rule */
        public int GetAD_Val_Rule_ID()
        {
            Object ii = Get_Value("AD_Val_Rule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Callout Code.
@param Callout External Callout Code - Fully qualified class names and method - separated by semicolons */
        public void SetCallout(String Callout)
        {
            if (Callout != null && Callout.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                Callout = Callout.Substring(0, 1000);
            }
            Set_Value("Callout", Callout);
        }
        /** Get Callout Code.
@return External Callout Code - Fully qualified class names and method - separated by semicolons */
        public String GetCallout()
        {
            return (String)Get_Value("Callout");
        }
        /** Set Copy.
@param IsCopy Copy contents of this field using the Copy Record function. */
        public void SetIsCopy(Boolean IsCopy)
        {
            Set_Value("IsCopy", IsCopy);
        }
        /** Get Copy.
@return Copy contents of this field using the Copy Record function. */
        public Boolean IsCopy()
        {
            Object oo = Get_Value("IsCopy");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool))
                    return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set DB Column Name.
@param ColumnName Name of the column in the database */
        public void SetColumnName(String ColumnName)
        {
            if (ColumnName == null) throw new ArgumentException("ColumnName is mandatory.");
            if (ColumnName.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                ColumnName = ColumnName.Substring(0, 40);
            }
            Set_Value("ColumnName", ColumnName);
        }
        /** Get DB Column Name.
@return Name of the column in the database */
        public String GetColumnName()
        {
            return (String)Get_Value("ColumnName");
        }
        /** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetColumnName());
        }
        /** Set Column SQL.
@param ColumnSQL Virtual Column (r/o) */
        public void SetColumnSQL(String ColumnSQL)
        {
            if (ColumnSQL != null && ColumnSQL.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                ColumnSQL = ColumnSQL.Substring(0, 2000);
            }
            Set_Value("ColumnSQL", ColumnSQL);
        }
        /** Get Column SQL.
@return Virtual Column (r/o) */
        public String GetColumnSQL()
        {
            return (String)Get_Value("ColumnSQL");
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

        /** ConstraintType AD_Reference_ID=411 */
        public static int CONSTRAINTTYPE_AD_Reference_ID = 411;
        /** Cascade = C */
        public static String CONSTRAINTTYPE_Cascade = "C";
        /** Null = N */
        public static String CONSTRAINTTYPE_Null = "N";
        /** Restrict = R */
        public static String CONSTRAINTTYPE_Restrict = "R";
        /** Do NOT Create = X */
        public static String CONSTRAINTTYPE_DoNOTCreate = "X";
        /** Cascade Trigger = c */
        public static String CONSTRAINTTYPE_CascadeTrigger = "c";
        /** Null Trigger = n */
        public static String CONSTRAINTTYPE_NullTrigger = "n";
        /** Ristrict Trigger = r */
        public static String CONSTRAINTTYPE_RistrictTrigger = "r";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsConstraintTypeValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("N") || test.Equals("R") || test.Equals("X") || test.Equals("c") || test.Equals("n") || test.Equals("r");
        }
        /** Set Constraint Type.
@param ConstraintType Constraint Type */
        public void SetConstraintType(String ConstraintType)
        {
            if (!IsConstraintTypeValid(ConstraintType))
                throw new ArgumentException("ConstraintType Invalid value - " + ConstraintType + " - Reference_ID=411 - C - N - R - X - c - n - r");
            if (ConstraintType != null && ConstraintType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ConstraintType = ConstraintType.Substring(0, 1);
            }
            Set_Value("ConstraintType", ConstraintType);
        }
        /** Get Constraint Type.
@return Constraint Type */
        public String GetConstraintType()
        {
            return (String)Get_Value("ConstraintType");
        }
        /** Set Default Logic.
@param DefaultValue Default value hierarchy, separated by ;
         */
        public void SetDefaultValue(String DefaultValue)
        {
            if (DefaultValue != null && DefaultValue.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                DefaultValue = DefaultValue.Substring(0, 2000);
            }
            Set_Value("DefaultValue", DefaultValue);
        }
        /** Get Default Logic.
@return Default value hierarchy, separated by ;
         */
        public String GetDefaultValue()
        {
            return (String)Get_Value("DefaultValue");
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

        /** EntityType AD_Reference_ID=389 */
        public static int ENTITYTYPE_AD_Reference_ID = 389;
        /** Set Entity Type.
@param EntityType Dictionary Entity Type;
         Determines ownership and synchronization */
        public void SetEntityType(String EntityType)
        {
            if (EntityType.Length > 4)
            {
                log.Warning("Length > 4 - truncated");
                EntityType = EntityType.Substring(0, 4);
            }
            Set_Value("EntityType", EntityType);
        }
        /** Get Entity Type.
@return Dictionary Entity Type;
         Determines ownership and synchronization */
        public String GetEntityType()
        {
            return (String)Get_Value("EntityType");
        }
        /** Set Length.
@param FieldLength Length of the column in the database */
        public void SetFieldLength(int FieldLength)
        {
            Set_Value("FieldLength", FieldLength);
        }
        /** Get Length.
@return Length of the column in the database */
        public int GetFieldLength()
        {
            Object ii = Get_Value("FieldLength");
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
        /** Set Always Updatable.
@param IsAlwaysUpdateable The column is always updatable, even if the record is not active or processed */
        public void SetIsAlwaysUpdateable(Boolean IsAlwaysUpdateable)
        {
            Set_Value("IsAlwaysUpdateable", IsAlwaysUpdateable);
        }
        /** Get Always Updatable.
@return The column is always updatable, even if the record is not active or processed */
        public Boolean IsAlwaysUpdateable()
        {
            Object oo = Get_Value("IsAlwaysUpdateable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Callout.
@param IsCallout This column has implemented a Callout */
        public void SetIsCallout(Boolean IsCallout)
        {
            Set_Value("IsCallout", IsCallout);
        }
        /** Get Callout.
@return This column has implemented a Callout */
        public Boolean IsCallout()
        {
            Object oo = Get_Value("IsCallout");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** IsEncrypted AD_Reference_ID=354 */
        public static int ISENCRYPTED_AD_Reference_ID = 354;
        /** Not Encrypted = N */
        public static String ISENCRYPTED_NotEncrypted = "N";
        /** Encrypted = Y */
        public static String ISENCRYPTED_Encrypted = "Y";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsIsEncryptedValid(String test)
        {
            return test.Equals("N") || test.Equals("Y");
        }
        /** Set Encrypted.
@param IsEncrypted Display or Storage is encrypted */
        public void SetIsEncrypted(String IsEncrypted)
        {
            if (IsEncrypted == null) throw new ArgumentException("IsEncrypted is mandatory");
            if (!IsIsEncryptedValid(IsEncrypted))
                throw new ArgumentException("IsEncrypted Invalid value - " + IsEncrypted + " - Reference_ID=354 - N - Y");
            if (IsEncrypted.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                IsEncrypted = IsEncrypted.Substring(0, 1);
            }
            Set_Value("IsEncrypted", IsEncrypted);
        }
        /** Get Encrypted.
@return Display or Storage is encrypted */
        public String GetIsEncrypted()
        {
            return (String)Get_Value("IsEncrypted");
        }
        /** Set Identifier.
@param IsIdentifier This column is part of the record identifier */
        public void SetIsIdentifier(Boolean IsIdentifier)
        {
            Set_Value("IsIdentifier", IsIdentifier);
        }
        /** Get Identifier.
@return This column is part of the record identifier */
        public Boolean IsIdentifier()
        {
            Object oo = Get_Value("IsIdentifier");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Key column.
@param IsKey This column is the key in this table */
        public void SetIsKey(Boolean IsKey)
        {
            Set_Value("IsKey", IsKey);
        }
        /** Get Key column.
@return This column is the key in this table */
        public Boolean IsKey()
        {
            Object oo = Get_Value("IsKey");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory.
@param IsMandatory Data is required in this column */
        public void SetIsMandatory(Boolean IsMandatory)
        {
            Set_Value("IsMandatory", IsMandatory);
        }
        /** Get Mandatory.
@return Data is required in this column */
        public Boolean IsMandatory()
        {
            Object oo = Get_Value("IsMandatory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory UI.
@param IsMandatoryUI Data entry is required for data entry in the field */
        public void SetIsMandatoryUI(Boolean IsMandatoryUI)
        {
            Set_Value("IsMandatoryUI", IsMandatoryUI);
        }
        /** Get Mandatory UI.
@return Data entry is required for data entry in the field */
        public Boolean IsMandatoryUI()
        {
            Object oo = Get_Value("IsMandatoryUI");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Parent link column.
@param IsParent This column is a link to the parent table (e.g. header from lines) - incl. Association key columns */
        public void SetIsParent(Boolean IsParent)
        {
            Set_Value("IsParent", IsParent);
        }
        /** Get Parent link column.
@return This column is a link to the parent table (e.g. header from lines) - incl. Association key columns */
        public Boolean IsParent()
        {
            Object oo = Get_Value("IsParent");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Recursive FK.
@param IsRecursiveFK Recursive Foreign key */
        public void SetIsRecursiveFK(Boolean IsRecursiveFK)
        {
            Set_Value("IsRecursiveFK", IsRecursiveFK);
        }
        /** Get Recursive FK.
@return Recursive Foreign key */
        public Boolean IsRecursiveFK()
        {
            Object oo = Get_Value("IsRecursiveFK");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Selection Column.
@param IsSelectionColumn Is this column used for finding rows in windows */
        public void SetIsSelectionColumn(Boolean IsSelectionColumn)
        {
            Set_Value("IsSelectionColumn", IsSelectionColumn);
        }
        /** Get Selection Column.
@return Is this column used for finding rows in windows */
        public Boolean IsSelectionColumn()
        {
            Object oo = Get_Value("IsSelectionColumn");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Column.
@param IsSummaryColumn Summary Info Column */
        public void SetIsSummaryColumn(Boolean IsSummaryColumn)
        {
            Set_Value("IsSummaryColumn", IsSummaryColumn);
        }
        /** Get Summary Column.
@return Summary Info Column */
        public Boolean IsSummaryColumn()
        {
            Object oo = Get_Value("IsSummaryColumn");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Synchronize Database.
@param IsSyncDatabase Change database table definition when changing dictionary definition */
        public void SetIsSyncDatabase(String IsSyncDatabase)
        {
            if (IsSyncDatabase != null && IsSyncDatabase.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                IsSyncDatabase = IsSyncDatabase.Substring(0, 1);
            }
            Set_Value("IsSyncDatabase", IsSyncDatabase);
        }
        /** Get Synchronize Database.
@return Change database table definition when changing dictionary definition */
        public String GetIsSyncDatabase()
        {
            return (String)Get_Value("IsSyncDatabase");
        }
        /** Set Translated.
@param IsTranslated This column is translated */
        public void SetIsTranslated(Boolean IsTranslated)
        {
            Set_Value("IsTranslated", IsTranslated);
        }
        /** Get Translated.
@return This column is translated */
        public Boolean IsTranslated()
        {
            Object oo = Get_Value("IsTranslated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Updateable.
@param IsUpdateable Determines, if the field can be updated */
        public void SetIsUpdateable(Boolean IsUpdateable)
        {
            Set_Value("IsUpdateable", IsUpdateable);
        }
        /** Get Updateable.
@return Determines, if the field can be updated */
        public Boolean IsUpdateable()
        {
            Object oo = Get_Value("IsUpdateable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory Logic.
@param MandatoryLogic Logic detremining when a field is required to be entered */
        public void SetMandatoryLogic(String MandatoryLogic)
        {
            if (MandatoryLogic != null && MandatoryLogic.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                MandatoryLogic = MandatoryLogic.Substring(0, 2000);
            }
            Set_Value("MandatoryLogic", MandatoryLogic);
        }
        /** Get Mandatory Logic.
@return Logic detremining when a field is required to be entered */
        public String GetMandatoryLogic()
        {
            return (String)Get_Value("MandatoryLogic");
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
        /** Set Read Only Logic.
@param ReadOnlyLogic Logic to determine if field is read only (applies only when field is read-write) */
        public void SetReadOnlyLogic(String ReadOnlyLogic)
        {
            if (ReadOnlyLogic != null && ReadOnlyLogic.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                ReadOnlyLogic = ReadOnlyLogic.Substring(0, 2000);
            }
            Set_Value("ReadOnlyLogic", ReadOnlyLogic);
        }
        /** Get Read Only Logic.
@return Logic to determine if field is read only (applies only when field is read-write) */
        public String GetReadOnlyLogic()
        {
            return (String)Get_Value("ReadOnlyLogic");
        }
        /** Set Selection Sequence.
@param SelectionSeqNo Sequence in Selection */
        public void SetSelectionSeqNo(int SelectionSeqNo)
        {
            Set_Value("SelectionSeqNo", SelectionSeqNo);
        }
        /** Get Selection Sequence.
@return Sequence in Selection */
        public int GetSelectionSeqNo()
        {
            Object ii = Get_Value("SelectionSeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sequence.
@param SeqNo Method of ordering elements;
         lowest number comes first */
        public void SetSeqNo(int SeqNo)
        {
            Set_Value("SeqNo", SeqNo);
        }
        /** Get Sequence.
@return Method of ordering elements;
         lowest number comes first */
        public int GetSeqNo()
        {
            Object ii = Get_Value("SeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Summary Sequence.
@param SummarySeqNo Sequence in Summary */
        public void SetSummarySeqNo(int SummarySeqNo)
        {
            Set_Value("SummarySeqNo", SummarySeqNo);
        }
        /** Get Summary Sequence.
@return Sequence in Summary */
        public int GetSummarySeqNo()
        {
            Object ii = Get_Value("SummarySeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set TableUID.
@param TableUID TableUID */
        public void SetTableUID(int TableUID)
        {
            Set_Value("TableUID", TableUID);
        }
        /** Get TableUID.
@return TableUID */
        public int GetTableUID()
        {
            Object ii = Get_Value("TableUID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Value Format.
@param VFormat Format of the value;
         Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
        public void SetVFormat(String VFormat)
        {
            if (VFormat != null && VFormat.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                VFormat = VFormat.Substring(0, 60);
            }
            Set_Value("VFormat", VFormat);
        }
        /** Get Value Format.
@return Format of the value;
         Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
        public String GetVFormat()
        {
            return (String)Get_Value("VFormat");
        }
        /** Set Max. Value.
@param ValueMax Maximum Value for a field */
        public void SetValueMax(String ValueMax)
        {
            if (ValueMax != null && ValueMax.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                ValueMax = ValueMax.Substring(0, 20);
            }
            Set_Value("ValueMax", ValueMax);
        }
        /** Get Max. Value.
@return Maximum Value for a field */
        public String GetValueMax()
        {
            return (String)Get_Value("ValueMax");
        }
        /** Set Min. Value.
@param ValueMin Minimum Value for a field */
        public void SetValueMin(String ValueMin)
        {
            if (ValueMin != null && ValueMin.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                ValueMin = ValueMin.Substring(0, 20);
            }
            Set_Value("ValueMin", ValueMin);
        }
        /** Get Min. Value.
@return Minimum Value for a field */
        public String GetValueMin()
        {
            return (String)Get_Value("ValueMin");
        }
        /** Set Version.
@param Version Version of the table definition */
        public void SetVersion(Decimal? Version)
        {
            if (Version == null) throw new ArgumentException("Version is mandatory.");
            Set_Value("Version", (Decimal?)Version);
        }
        /** Get Version.
@return Version of the table definition */
        public Decimal GetVersion()
        {
            Object bd = Get_Value("Version");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        /** Set Special Form.
@param AD_Form_ID Special Form */
        public void SetAD_Form_ID(int AD_Form_ID)
        {
            if (AD_Form_ID <= 0) Set_Value("AD_Form_ID", null);
            else
                Set_Value("AD_Form_ID", AD_Form_ID);
        }/** Get Special Form.
@return Special Form */

        public int GetAD_Form_ID() { Object ii = Get_Value("AD_Form_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        ///<summary>
        /// SetIsLink
        ///</summary>
        ///<param name="IsLink">IsLink</param>
        public void SetIsLink(Boolean IsLink)
        {
            Set_Value("IsLink", IsLink);
        }

        ///<summary>
        /// GetIsLink
        ///</summary>
        ///<returns> IsLink</returns>
        public Boolean IsLink()
        {
            Object oo = Get_Value("IsLink"); if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        ///<summary>
        /// SetIsRightPaneLink
        ///</summary>
        ///<param name="IsRightPaneLink">IsRightPaneLink</param>
        public void SetIsRightPaneLink(Boolean IsRightPaneLink)
        {
            Set_Value("IsRightPaneLink", IsRightPaneLink);
        }

        ///<summary>
        /// GetIsRightPaneLink
        ///</summary>
        ///<returns> IsRightPaneLink</returns>
        public Boolean IsRightPaneLink()
        {
            Object oo = Get_Value("IsRightPaneLink"); if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        ///<summary>
        /// SetExport
        ///</summary>
        ///<param name="Export_ID">Export</param>
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }

        ///<summary>
        /// GetExport
        ///</summary>
        ///<returns> Export</returns>
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }

        /** Set Maintain Versions.
        @param IsMaintainVersions Maintain Versions */
        public void SetIsMaintainVersions(Boolean IsMaintainVersions)
        {
            Set_Value("IsMaintainVersions", IsMaintainVersions);
        }
        
        /** Get Maintain Versions.
        @return Maintain Versions */
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

        /** Set unique column.
@param unique This column is the unique in this table */


        public void SetIsUnique(Boolean IsUnique)
        {
            Set_Value("IsUnique", IsUnique);
        }
        /** Get Unique column.
@return This column is the unique in this table */
        public Boolean IsUnique()
        {
            Object oo = Get_Value("IsUnique");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** ObscureType AD_Reference_ID=291 */
        public static int OBSCURETYPE_AD_Reference_ID = 291;/** Obscure Digits but last 4 = 904 */
        public static String OBSCURETYPE_ObscureDigitsButLast4 = "904";/** Obscure Digits but first/last 4 = 944 */
        public static String OBSCURETYPE_ObscureDigitsButFirstLast4 = "944";/** Obscure AlphaNumeric but last 4 = A04 */
        public static String OBSCURETYPE_ObscureAlphaNumericButLast4 = "A04";/** Obscure AlphaNumeric but first/last 4 = A44 */
        public static String OBSCURETYPE_ObscureAlphaNumericButFirstLast4 = "A44";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsObscureTypeValid(String test) { return test == null || test.Equals("904") || test.Equals("944") || test.Equals("A04") || test.Equals("A44"); }/** Set Obscure.
@param ObscureType Type of obscuring the data (limiting the display) */
        public void SetObscureType(String ObscureType)
        {
            if (!IsObscureTypeValid(ObscureType))
                throw new ArgumentException("ObscureType Invalid value - " + ObscureType + " - Reference_ID=291 - 904 - 944 - A04 - A44"); if (ObscureType != null && ObscureType.Length > 3) { log.Warning("Length > 3 - truncated"); ObscureType = ObscureType.Substring(0, 3); }
            Set_Value("ObscureType", ObscureType);
        }/** Get Obscure.
@return Type of obscuring the data (limiting the display) */
        public String GetObscureType() { return (String)Get_Value("ObscureType"); }

    }

}
