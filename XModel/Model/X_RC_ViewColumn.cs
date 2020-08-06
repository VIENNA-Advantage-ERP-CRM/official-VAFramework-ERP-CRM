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
    /** Generated Model for RC_ViewColumn
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RC_ViewColumn : PO
    {
        public X_RC_ViewColumn(Context ctx, int RC_ViewColumn_ID, Trx trxName)
            : base(ctx, RC_ViewColumn_ID, trxName)
        {
            /** if (RC_ViewColumn_ID == 0)
            {
            SetRC_ViewColumn_ID (0);
            SetRC_View_ID (0);
            }
             */
        }
        public X_RC_ViewColumn(Ctx ctx, int RC_ViewColumn_ID, Trx trxName)
            : base(ctx, RC_ViewColumn_ID, trxName)
        {
            /** if (RC_ViewColumn_ID == 0)
            {
            SetRC_ViewColumn_ID (0);
            SetRC_View_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewColumn(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewColumn(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewColumn(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RC_ViewColumn()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27706188629143L;
        /** Last Updated Timestamp 2/16/2015 10:38:32 AM */
        public static long updatedMS = 1424063312354L;
        /** AD_Table_ID=1000237 */
        public static int Table_ID;
        // =1000237;

        /** TableName=RC_ViewColumn */
        public static String Table_Name = "RC_ViewColumn";

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
            StringBuilder sb = new StringBuilder("X_RC_ViewColumn[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Field.
        @param AD_Field_ID Field on a tab in a window */
        public void SetAD_Field_ID(int AD_Field_ID)
        {
            if (AD_Field_ID <= 0) Set_Value("AD_Field_ID", null);
            else
                Set_Value("AD_Field_ID", AD_Field_ID);
        }
        /** Get Field.
        @return Field on a tab in a window */
        public int GetAD_Field_ID()
        {
            Object ii = Get_Value("AD_Field_ID");
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
        @param RC_ViewColumn_ID View Column */
        public void SetRC_ViewColumn_ID(int RC_ViewColumn_ID)
        {
            if (RC_ViewColumn_ID < 1) throw new ArgumentException("RC_ViewColumn_ID is mandatory.");
            Set_ValueNoCheck("RC_ViewColumn_ID", RC_ViewColumn_ID);
        }
        /** Get View Column.
        @return View Column */
        public int GetRC_ViewColumn_ID()
        {
            Object ii = Get_Value("RC_ViewColumn_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Role Center View.
        @param RC_View_ID Role Center View */
        public void SetRC_View_ID(int RC_View_ID)
        {
            if (RC_View_ID < 1) throw new ArgumentException("RC_View_ID is mandatory.");
            Set_ValueNoCheck("RC_View_ID", RC_View_ID);
        }
        /** Get Role Center View.
        @return Role Center View */
        public int GetRC_View_ID()
        {
            Object ii = Get_Value("RC_View_ID");
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
