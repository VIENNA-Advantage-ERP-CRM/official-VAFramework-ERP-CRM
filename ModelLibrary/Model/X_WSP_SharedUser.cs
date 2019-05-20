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
    /** Generated Model for WSP_SharedUser
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_WSP_SharedUser : PO
    {
        public X_WSP_SharedUser(Context ctx, int WSP_SharedUser_ID, Trx trxName)
            : base(ctx, WSP_SharedUser_ID, trxName)
        {
            /** if (WSP_SharedUser_ID == 0)
            {
            SetWSP_SharedUser_ID (0);
            }
             */
        }
        public X_WSP_SharedUser(Ctx ctx, int WSP_SharedUser_ID, Trx trxName)
            : base(ctx, WSP_SharedUser_ID, trxName)
        {
            /** if (WSP_SharedUser_ID == 0)
            {
            SetWSP_SharedUser_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_WSP_SharedUser(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_WSP_SharedUser(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_WSP_SharedUser(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_WSP_SharedUser()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27687620859404L;
        /** Last Updated Timestamp 7/16/2014 12:55:42 PM */
        public static long updatedMS = 1405495542615L;
        /** AD_Table_ID=1000465 */
        public static int Table_ID;
        // =1000465;

        /** TableName=WSP_SharedUser */
        public static String Table_Name = "WSP_SharedUser";

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
            StringBuilder sb = new StringBuilder("X_WSP_SharedUser[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
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
        /** Set WSP_SharedUser_ID.
        @param WSP_SharedUser_ID WSP_SharedUser_ID */
        public void SetWSP_SharedUser_ID(int WSP_SharedUser_ID)
        {
            if (WSP_SharedUser_ID < 1) throw new ArgumentException("WSP_SharedUser_ID is mandatory.");
            Set_ValueNoCheck("WSP_SharedUser_ID", WSP_SharedUser_ID);
        }
        /** Get WSP_SharedUser_ID.
        @return WSP_SharedUser_ID */
        public int GetWSP_SharedUser_ID()
        {
            Object ii = Get_Value("WSP_SharedUser_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
