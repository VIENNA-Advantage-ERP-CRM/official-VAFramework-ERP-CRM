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
    /** Generated Model for C_NonBusinessDay
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_NonBusinessDay : PO
    {
        public X_C_NonBusinessDay(Context ctx, int C_NonBusinessDay_ID, Trx trxName)
            : base(ctx, C_NonBusinessDay_ID, trxName)
        {
            /** if (C_NonBusinessDay_ID == 0)
            {
            SetC_Calendar_ID (0);
            SetC_NonBusinessDay_ID (0);
            SetDate1 (DateTime.Now);
            }
             */
        }
        public X_C_NonBusinessDay(Ctx ctx, int C_NonBusinessDay_ID, Trx trxName)
            : base(ctx, C_NonBusinessDay_ID, trxName)
        {
            /** if (C_NonBusinessDay_ID == 0)
            {
            SetC_Calendar_ID (0);
            SetC_NonBusinessDay_ID (0);
            SetDate1 (DateTime.Now);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_NonBusinessDay(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_NonBusinessDay(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_NonBusinessDay(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_NonBusinessDay()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514373081L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056292L;
        /** AD_Table_ID=163 */
        public static int Table_ID;
        // =163;

        /** TableName=C_NonBusinessDay */
        public static String Table_Name = "C_NonBusinessDay";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
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
            StringBuilder sb = new StringBuilder("X_C_NonBusinessDay[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Calendar.
        @param C_Calendar_ID Accounting Calendar Name */
        public void SetC_Calendar_ID(int C_Calendar_ID)
        {
            if (C_Calendar_ID < 1) throw new ArgumentException("C_Calendar_ID is mandatory.");
            Set_ValueNoCheck("C_Calendar_ID", C_Calendar_ID);
        }
        /** Get Calendar.
        @return Accounting Calendar Name */
        public int GetC_Calendar_ID()
        {
            Object ii = Get_Value("C_Calendar_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Non Business Day.
        @param C_NonBusinessDay_ID Day on which business is not transacted */
        public void SetC_NonBusinessDay_ID(int C_NonBusinessDay_ID)
        {
            if (C_NonBusinessDay_ID < 1) throw new ArgumentException("C_NonBusinessDay_ID is mandatory.");
            Set_ValueNoCheck("C_NonBusinessDay_ID", C_NonBusinessDay_ID);
        }
        /** Get Non Business Day.
        @return Day on which business is not transacted */
        public int GetC_NonBusinessDay_ID()
        {
            Object ii = Get_Value("C_NonBusinessDay_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Date.
        @param Date1 Date when business is not conducted */
        public void SetDate1(DateTime? Date1)
        {
            if (Date1 == null) throw new ArgumentException("Date1 is mandatory.");
            Set_Value("Date1", (DateTime?)Date1);
        }
        /** Get Date.
        @return Date when business is not conducted */
        public DateTime? GetDate1()
        {
            return (DateTime?)Get_Value("Date1");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 60)
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
    }

}
