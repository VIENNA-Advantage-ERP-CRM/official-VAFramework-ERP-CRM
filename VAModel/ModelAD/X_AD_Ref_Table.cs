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
    /** Generated Model for AD_Ref_Table
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Ref_Table : PO
    {
        public X_AD_Ref_Table(Context ctx, int AD_Ref_Table_ID, Trx trxName)
            : base(ctx, AD_Ref_Table_ID, trxName)
        {
            /** if (AD_Ref_Table_ID == 0)
            {
            SetAD_Reference_ID (0);
            SetAD_Table_ID (0);
            SetColumn_Display_ID (0);
            SetColumn_Key_ID (0);
            SetEntityType (null);	// U
            SetIsValueDisplayed (false);
            }
             */
        }
        public X_AD_Ref_Table(Ctx ctx, int AD_Ref_Table_ID, Trx trxName)
            : base(ctx, AD_Ref_Table_ID, trxName)
        {
            /** if (AD_Ref_Table_ID == 0)
            {
            SetAD_Reference_ID (0);
            SetAD_Table_ID (0);
            SetColumn_Display_ID (0);
            SetColumn_Key_ID (0);
            SetEntityType (null);	// U
            SetIsValueDisplayed (false);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Ref_Table(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Ref_Table(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Ref_Table(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Ref_Table()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27624024687701L;
        /** Last Updated Timestamp 7/10/2012 11:19:30 AM */
        public static long updatedMS = 1341899370912L;
        /** AD_Table_ID=103 */
        public static int Table_ID;
        // =103;

        /** TableName=AD_Ref_Table */
        public static String Table_Name = "AD_Ref_Table";

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
            StringBuilder sb = new StringBuilder("X_AD_Ref_Table[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Reference.
        @param AD_Reference_ID System Reference and Validation */
        public void SetAD_Reference_ID(int AD_Reference_ID)
        {
            if (AD_Reference_ID < 1) throw new ArgumentException("AD_Reference_ID is mandatory.");
            Set_ValueNoCheck("AD_Reference_ID", AD_Reference_ID);
        }
        /** Get Reference.
        @return System Reference and Validation */
        public int GetAD_Reference_ID()
        {
            Object ii = Get_Value("AD_Reference_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetAD_Reference_ID().ToString());
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID < 1) throw new ArgumentException("AD_Table_ID is mandatory.");
            Set_Value("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Column_Display_ID AD_Reference_ID=3 */
        public static int COLUMN_DISPLAY_ID_AD_Reference_ID = 3;
        /** Set Display column.
        @param Column_Display_ID Column that will display */
        public void SetColumn_Display_ID(int Column_Display_ID)
        {
            if (Column_Display_ID < 1) throw new ArgumentException("Column_Display_ID is mandatory.");
            Set_Value("Column_Display_ID", Column_Display_ID);
        }
        /** Get Display column.
        @return Column that will display */
        public int GetColumn_Display_ID()
        {
            Object ii = Get_Value("Column_Display_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Column_Key_ID AD_Reference_ID=3 */
        public static int COLUMN_KEY_ID_AD_Reference_ID = 3;
        /** Set Key column.
        @param Column_Key_ID Unique identifier of a record */
        public void SetColumn_Key_ID(int Column_Key_ID)
        {
            if (Column_Key_ID < 1) throw new ArgumentException("Column_Key_ID is mandatory.");
            Set_Value("Column_Key_ID", Column_Key_ID);
        }
        /** Get Key column.
        @return Unique identifier of a record */
        public int GetColumn_Key_ID()
        {
            Object ii = Get_Value("Column_Key_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Display Value.
        @param IsValueDisplayed Displays Value column with the Display column */
        public void SetIsValueDisplayed(Boolean IsValueDisplayed)
        {
            Set_Value("IsValueDisplayed", IsValueDisplayed);
        }
        /** Get Display Value.
        @return Displays Value column with the Display column */
        public Boolean IsValueDisplayed()
        {
            Object oo = Get_Value("IsValueDisplayed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sql ORDER BY.
        @param OrderByClause Fully qualified ORDER BY clause */
        public void SetOrderByClause(String OrderByClause)
        {
            if (OrderByClause != null && OrderByClause.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                OrderByClause = OrderByClause.Substring(0, 2000);
            }
            Set_Value("OrderByClause", OrderByClause);
        }
        /** Get Sql ORDER BY.
        @return Fully qualified ORDER BY clause */
        public String GetOrderByClause()
        {
            return (String)Get_Value("OrderByClause");
        }
        /** Set Sql WHERE.
        @param WhereClause Fully qualified SQL WHERE clause */
        public void SetWhereClause(String WhereClause)
        {
            if (WhereClause != null && WhereClause.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                WhereClause = WhereClause.Substring(0, 2000);
            }
            Set_Value("WhereClause", WhereClause);
        }
        /** Get Sql WHERE.
        @return Fully qualified SQL WHERE clause */
        public String GetWhereClause()
        {
            return (String)Get_Value("WhereClause");
        }
    }

}
