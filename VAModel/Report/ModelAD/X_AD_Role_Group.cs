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
    /** Generated Model for AD_Role_Group
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Role_Group : PO
    {
        public X_AD_Role_Group(Context ctx, int AD_Role_Group_ID, Trx trxName)
            : base(ctx, AD_Role_Group_ID, trxName)
        {
            /** if (AD_Role_Group_ID == 0)
            {
            SetAD_GroupInfo_ID (0);
            SetAD_Role_ID (0);
            SetAD_User_ID (0);
            }
             */
        }
        public X_AD_Role_Group(Ctx ctx, int AD_Role_Group_ID, Trx trxName)
            : base(ctx, AD_Role_Group_ID, trxName)
        {
            /** if (AD_Role_Group_ID == 0)
            {
            SetAD_GroupInfo_ID (0);
            SetAD_Role_ID (0);
            SetAD_User_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role_Group(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role_Group(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role_Group(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Role_Group()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27713103313685L;
        /** Last Updated Timestamp 5/7/2015 11:23:17 AM */
        public static long updatedMS = 1430977996896L;
        /** AD_Table_ID=1000495 */
        public static int Table_ID;
        // =1000495;

        /** TableName=AD_Role_Group */
        public static String Table_Name = "AD_Role_Group";

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
            StringBuilder sb = new StringBuilder("X_AD_Role_Group[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set AD_GroupInfo_ID.
        @param AD_GroupInfo_ID AD_GroupInfo_ID */
        public void SetAD_GroupInfo_ID(int AD_GroupInfo_ID)
        {
            if (AD_GroupInfo_ID < 1) throw new ArgumentException("AD_GroupInfo_ID is mandatory.");
            Set_ValueNoCheck("AD_GroupInfo_ID", AD_GroupInfo_ID);
        }
        /** Get AD_GroupInfo_ID.
        @return AD_GroupInfo_ID */
        public int GetAD_GroupInfo_ID()
        {
            Object ii = Get_Value("AD_GroupInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Role.
        @param AD_Role_ID Responsibility Role */
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID < 0) throw new ArgumentException("AD_Role_ID is mandatory.");
            Set_ValueNoCheck("AD_Role_ID", AD_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetAD_Role_ID()
        {
            Object ii = Get_Value("AD_Role_ID");
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
