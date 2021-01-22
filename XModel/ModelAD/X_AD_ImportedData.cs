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
    /** Generated Model for VAF_ImportedData
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_ImportedData : PO
    {
        public X_VAF_ImportedData(Context ctx, int VAF_ImportedData_ID, Trx trxName)
            : base(ctx, VAF_ImportedData_ID, trxName)
        {
            /** if (VAF_ImportedData_ID == 0)
            {
            SetVAF_ImportedData_ID (0);
            }
             */
        }
        public X_VAF_ImportedData(Ctx ctx, int VAF_ImportedData_ID, Trx trxName)
            : base(ctx, VAF_ImportedData_ID, trxName)
        {
            /** if (VAF_ImportedData_ID == 0)
            {
            SetVAF_ImportedData_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ImportedData(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ImportedData(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_ImportedData(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_ImportedData()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27584822386899L;
        /** Last Updated Timestamp 4/13/2011 5:47:50 PM */
        public static long updatedMS = 1302697070110L;
        /** VAF_TableView_ID=1000057 */
        public static int Table_ID;
        // =1000057;

        /** TableName=VAF_ImportedData */
        public static String Table_Name = "VAF_ImportedData";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_VAF_ImportedData[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set VAF_ImportedData_ID.
        @param VAF_ImportedData_ID VAF_ImportedData_ID */
        public void SetVAF_ImportedData_ID(int VAF_ImportedData_ID)
        {
            if (VAF_ImportedData_ID < 1) throw new ArgumentException("VAF_ImportedData_ID is mandatory.");
            Set_ValueNoCheck("VAF_ImportedData_ID", VAF_ImportedData_ID);
        }
        /** Get VAF_ImportedData_ID.
        @return VAF_ImportedData_ID */
        public int GetVAF_ImportedData_ID()
        {
            Object ii = Get_Value("VAF_ImportedData_ID");
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
        /** Set Record Status.
        @param RecordStatus Record Status */
        public void SetRecordStatus(String RecordStatus)
        {
            if (RecordStatus != null && RecordStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                RecordStatus = RecordStatus.Substring(0, 1);
            }
            Set_Value("RecordStatus", RecordStatus);
        }
        /** Get Record Status.
        @return Record Status */
        public String GetRecordStatus()
        {
            return (String)Get_Value("RecordStatus");
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID <= 0) Set_Value("Record_ID", null);
            else
                Set_Value("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
