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
    /** Generated Model for AD_ReportTable
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_ReportTable : PO
    {
        public X_AD_ReportTable(Context ctx, int AD_ReportTable_ID, Trx trxName)
            : base(ctx, AD_ReportTable_ID, trxName)
        {
            /** if (AD_ReportTable_ID == 0)
            {
            SetAD_ReportFormat_ID (0);
            SetAD_ReportTable_ID (0);
            }
             */
        }
        public X_AD_ReportTable(Ctx ctx, int AD_ReportTable_ID, Trx trxName)
            : base(ctx, AD_ReportTable_ID, trxName)
        {
            /** if (AD_ReportTable_ID == 0)
            {
            SetAD_ReportFormat_ID (0);
            SetAD_ReportTable_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ReportTable(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ReportTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ReportTable(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_ReportTable()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27659272432890L;
        /** Last Updated Timestamp 8/22/2013 10:21:56 AM */
        public static long updatedMS = 1377147116101L;
        /** AD_Table_ID=1000727 */
        public static int Table_ID;
        // =1000727;

        /** TableName=AD_ReportTable */
        public static String Table_Name = "AD_ReportTable";

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
            StringBuilder sb = new StringBuilder("X_AD_ReportTable[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Column.
        @param AD_Column_ID Column in the table */
        public void SetAD_Column_ID(int AD_Column_ID)
        {
            if (AD_Column_ID <= 0) Set_Value("AD_Column_ID", null);
            else
                Set_Value("AD_Column_ID", AD_Column_ID);
        }
        /** Get Column.
        @return Column in the table */
        public int GetAD_Column_ID()
        {
            Object ii = Get_Value("AD_Column_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AD_ReportFormat_ID.
        @param AD_ReportFormat_ID AD_ReportFormat_ID */
        public void SetAD_ReportFormat_ID(int AD_ReportFormat_ID)
        {
            if (AD_ReportFormat_ID < 1) throw new ArgumentException("AD_ReportFormat_ID is mandatory.");
            Set_ValueNoCheck("AD_ReportFormat_ID", AD_ReportFormat_ID);
        }
        /** Get AD_ReportFormat_ID.
        @return AD_ReportFormat_ID */
        public int GetAD_ReportFormat_ID()
        {
            Object ii = Get_Value("AD_ReportFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AD_ReportTable_ID.
        @param AD_ReportTable_ID AD_ReportTable_ID */
        public void SetAD_ReportTable_ID(int AD_ReportTable_ID)
        {
            if (AD_ReportTable_ID < 1) throw new ArgumentException("AD_ReportTable_ID is mandatory.");
            Set_ValueNoCheck("AD_ReportTable_ID", AD_ReportTable_ID);
        }
        /** Get AD_ReportTable_ID.
        @return AD_ReportTable_ID */
        public int GetAD_ReportTable_ID()
        {
            Object ii = Get_Value("AD_ReportTable_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
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

        /** AD_Table_ID_1 AD_Reference_ID=1000147 */
        public static int AD_TABLE_ID_1_AD_Reference_ID = 1000147;
        /** Set Parent Table.
        @param AD_Table_ID_1 Parent Table */
        public void SetAD_Table_ID_1(int AD_Table_ID_1)
        {
            Set_Value("AD_Table_ID_1", AD_Table_ID_1);
        }
        /** Get Parent Table.
        @return Parent Table */
        public int GetAD_Table_ID_1()
        {
            Object ii = Get_Value("AD_Table_ID_1");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
            Set_Value("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
    }

}
