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
    /** Generated Model for VAB_CashBook
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_CashBook : PO
    {
        public X_VAB_CashBook(Context ctx, int VAB_CashBook_ID, Trx trxName)
            : base(ctx, VAB_CashBook_ID, trxName)
        {
            /** if (VAB_CashBook_ID == 0)
            {
            SetVAB_CashBook_ID (0);
            SetVAB_Currency_ID (0);	// SQL=SELECT cb.VAB_Currency_ID FROM VAB_CashBook cb INNER JOIN VAB_CashJRNL c ON (cb.VAB_CashBook_ID=c.VAB_CashBook_ID) WHERE c.VAB_CashJRNL_ID=@VAB_CashJRNL_ID@
            SetIsDefault (false);
            SetName (null);
            }
             */
        }
        public X_VAB_CashBook(Ctx ctx, int VAB_CashBook_ID, Trx trxName)
            : base(ctx, VAB_CashBook_ID, trxName)
        {
            /** if (VAB_CashBook_ID == 0)
            {
            SetVAB_CashBook_ID (0);
            SetVAB_Currency_ID (0);	// SQL=SELECT cb.VAB_Currency_ID FROM VAB_CashBook cb INNER JOIN VAB_CashJRNL c ON (cb.VAB_CashBook_ID=c.VAB_CashBook_ID) WHERE c.VAB_CashJRNL_ID=@VAB_CashJRNL_ID@
            SetIsDefault (false);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashBook(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashBook(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashBook(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_CashBook()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721596522043L;
        /** Last Updated Timestamp 8/13/2015 6:36:45 PM */
        public static long updatedMS = 1439471205254L;
        /** VAF_TableView_ID=408 */
        public static int Table_ID;
        // =408;

        /** TableName=VAB_CashBook */
        public static String Table_Name = "VAB_CashBook";

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
            StringBuilder sb = new StringBuilder("X_VAB_CashBook[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Cash Book.
        @param VAB_CashBook_ID Cash Book for recording petty cash transactions */
        public void SetVAB_CashBook_ID(int VAB_CashBook_ID)
        {
            if (VAB_CashBook_ID < 1) throw new ArgumentException("VAB_CashBook_ID is mandatory.");
            Set_ValueNoCheck("VAB_CashBook_ID", VAB_CashBook_ID);
        }
        /** Get Cash Book.
        @return Cash Book for recording petty cash transactions */
        public int GetVAB_CashBook_ID()
        {
            Object ii = Get_Value("VAB_CashBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID < 1) throw new ArgumentException("VAB_Currency_ID is mandatory.");
            Set_Value("VAB_Currency_ID", VAB_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVAB_Currency_ID()
        {
            Object ii = Get_Value("VAB_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Completed Balance.
        @param CompletedBalance Completed Balance */
        public void SetCompletedBalance(Decimal? CompletedBalance)
        {
            Set_Value("CompletedBalance", (Decimal?)CompletedBalance);
        }
        /** Get Completed Balance.
        @return Completed Balance */
        public Decimal GetCompletedBalance()
        {
            Object bd = Get_Value("CompletedBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 250)
            {
                log.Warning("Length > 250 - truncated");
                Name = Name.Substring(0, 250);
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
        /** Set Running Balance.
        @param RunningBalance Running Balance */
        public void SetRunningBalance(Decimal? RunningBalance)
        {
            Set_Value("RunningBalance", (Decimal?)RunningBalance);
        }
        /** Get Running Balance.
        @return Running Balance */
        public Decimal GetRunningBalance()
        {
            Object bd = Get_Value("RunningBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
