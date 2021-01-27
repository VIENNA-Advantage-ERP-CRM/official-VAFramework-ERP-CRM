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
    /** Generated Model for VARC_ViewColumn
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VARC_ViewColumn : PO
    {
        public X_VARC_ViewColumn(Context ctx, int VARC_ViewColumn_ID, Trx trxName)
            : base(ctx, VARC_ViewColumn_ID, trxName)
        {
            /** if (VARC_ViewColumn_ID == 0)
            {
            SetVARC_ViewColumn_ID (0);
            SetVARC_View_ID (0);
            }
             */
        }
        public X_VARC_ViewColumn(Ctx ctx, int VARC_ViewColumn_ID, Trx trxName)
            : base(ctx, VARC_ViewColumn_ID, trxName)
        {
            /** if (VARC_ViewColumn_ID == 0)
            {
            SetVARC_ViewColumn_ID (0);
            SetVARC_View_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_ViewColumn(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_ViewColumn(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_ViewColumn(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VARC_ViewColumn()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27706188629143L;
        /** Last Updated Timestamp 2/16/2015 10:38:32 AM */
        public static long updatedMS = 1424063312354L;
        /** VAF_TableView_ID=1000237 */
        public static int Table_ID;
        // =1000237;

        /** TableName=VARC_ViewColumn */
        public static String Table_Name = "VARC_ViewColumn";

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
            StringBuilder sb = new StringBuilder("X_VARC_ViewColumn[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Field.
        @param VAF_Field_ID Field on a tab in a window */
        public void SetVAF_Field_ID(int VAF_Field_ID)
        {
            if (VAF_Field_ID <= 0) Set_Value("VAF_Field_ID", null);
            else
                Set_Value("VAF_Field_ID", VAF_Field_ID);
        }
        /** Get Field.
        @return Field on a tab in a window */
        public int GetVAF_Field_ID()
        {
            Object ii = Get_Value("VAF_Field_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Description = Description.Substring(0, 50);
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
        /** Set View Column.
        @param VARC_ViewColumn_ID View Column */
        public void SetVARC_ViewColumn_ID(int VARC_ViewColumn_ID)
        {
            if (VARC_ViewColumn_ID < 1) throw new ArgumentException("VARC_ViewColumn_ID is mandatory.");
            Set_ValueNoCheck("VARC_ViewColumn_ID", VARC_ViewColumn_ID);
        }
        /** Get View Column.
        @return View Column */
        public int GetVARC_ViewColumn_ID()
        {
            Object ii = Get_Value("VARC_ViewColumn_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Role Center View.
        @param VARC_View_ID Role Center View */
        public void SetVARC_View_ID(int VARC_View_ID)
        {
            if (VARC_View_ID < 1) throw new ArgumentException("VARC_View_ID is mandatory.");
            Set_ValueNoCheck("VARC_View_ID", VARC_View_ID);
        }
        /** Get Role Center View.
        @return Role Center View */
        public int GetVARC_View_ID()
        {
            Object ii = Get_Value("VARC_View_ID");
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
    }

}
