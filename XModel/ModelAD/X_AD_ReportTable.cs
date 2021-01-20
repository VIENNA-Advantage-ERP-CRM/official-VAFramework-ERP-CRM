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
    /** Generated Model for VAF_ReportTable
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_ReportTable : PO
    {
        public X_VAF_ReportTable(Context ctx, int VAF_ReportTable_ID, Trx trxName)
            : base(ctx, VAF_ReportTable_ID, trxName)
        {
            /** if (VAF_ReportTable_ID == 0)
            {
            SetVAF_ReportLayout_ID (0);
            SetVAF_ReportTable_ID (0);
            }
             */
        }
        public X_VAF_ReportTable(Ctx ctx, int VAF_ReportTable_ID, Trx trxName)
            : base(ctx, VAF_ReportTable_ID, trxName)
        {
            /** if (VAF_ReportTable_ID == 0)
            {
            SetVAF_ReportLayout_ID (0);
            SetVAF_ReportTable_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ReportTable(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ReportTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ReportTable(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_ReportTable()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27659272432890L;
        /** Last Updated Timestamp 8/22/2013 10:21:56 AM */
        public static long updatedMS = 1377147116101L;
        /** VAF_TableView_ID=1000727 */
        public static int Table_ID;
        // =1000727;

        /** TableName=VAF_ReportTable */
        public static String Table_Name = "VAF_ReportTable";

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
            StringBuilder sb = new StringBuilder("X_VAF_ReportTable[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Column.
        @param VAF_Column_ID Column in the table */
        public void SetVAF_Column_ID(int VAF_Column_ID)
        {
            if (VAF_Column_ID <= 0) Set_Value("VAF_Column_ID", null);
            else
                Set_Value("VAF_Column_ID", VAF_Column_ID);
        }
        /** Get Column.
        @return Column in the table */
        public int GetVAF_Column_ID()
        {
            Object ii = Get_Value("VAF_Column_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VAF_ReportLayout_ID.
        @param VAF_ReportLayout_ID VAF_ReportLayout_ID */
        public void SetVAF_ReportLayout_ID(int VAF_ReportLayout_ID)
        {
            if (VAF_ReportLayout_ID < 1) throw new ArgumentException("VAF_ReportLayout_ID is mandatory.");
            Set_ValueNoCheck("VAF_ReportLayout_ID", VAF_ReportLayout_ID);
        }
        /** Get VAF_ReportLayout_ID.
        @return VAF_ReportLayout_ID */
        public int GetVAF_ReportLayout_ID()
        {
            Object ii = Get_Value("VAF_ReportLayout_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VAF_ReportTable_ID.
        @param VAF_ReportTable_ID VAF_ReportTable_ID */
        public void SetVAF_ReportTable_ID(int VAF_ReportTable_ID)
        {
            if (VAF_ReportTable_ID < 1) throw new ArgumentException("VAF_ReportTable_ID is mandatory.");
            Set_ValueNoCheck("VAF_ReportTable_ID", VAF_ReportTable_ID);
        }
        /** Get VAF_ReportTable_ID.
        @return VAF_ReportTable_ID */
        public int GetVAF_ReportTable_ID()
        {
            Object ii = Get_Value("VAF_ReportTable_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID <= 0) Set_Value("VAF_TableView_ID", null);
            else
                Set_Value("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAF_TableView_ID_1 VAF_Control_Ref_ID=1000147 */
        public static int VAF_TABLEVIEW_ID_1_VAF_Control_Ref_ID = 1000147;
        /** Set Parent Table.
        @param VAF_TableView_ID_1 Parent Table */
        public void SetVAF_TableView_ID_1(int VAF_TableView_ID_1)
        {
            Set_Value("VAF_TableView_ID_1", VAF_TableView_ID_1);
        }
        /** Get Parent Table.
        @return Parent Table */
        public int GetVAF_TableView_ID_1()
        {
            Object ii = Get_Value("VAF_TableView_ID_1");
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
