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
    /** Generated Model for VAF_WeatherInfo
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_WeatherInfo : PO
    {
        public X_VAF_WeatherInfo(Context ctx, int VAF_WeatherInfo_ID, Trx trxName)
            : base(ctx, VAF_WeatherInfo_ID, trxName)
        {
            /** if (VAF_WeatherInfo_ID == 0)
            {
            SetVAF_WeatherInfo_ID (0);
            }
             */
        }
        public X_VAF_WeatherInfo(Ctx ctx, int VAF_WeatherInfo_ID, Trx trxName)
            : base(ctx, VAF_WeatherInfo_ID, trxName)
        {
            /** if (VAF_WeatherInfo_ID == 0)
            {
            SetVAF_WeatherInfo_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_WeatherInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_WeatherInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_WeatherInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_WeatherInfo()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27603461129658L;
        /** Last Updated Timestamp 11/15/2011 11:13:32 AM */
        public static long updatedMS = 1321335812869L;
        /** VAF_TableView_ID=1000028 */
        public static int Table_ID;
        // =1000028;

        /** TableName=VAF_WeatherInfo */
        public static String Table_Name = "VAF_WeatherInfo";

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
            StringBuilder sb = new StringBuilder("X_VAF_WeatherInfo[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set User/Contact.
        @param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VAF_WeatherInfo_ID.
        @param VAF_WeatherInfo_ID VAF_WeatherInfo_ID */
        public void SetVAF_WeatherInfo_ID(int VAF_WeatherInfo_ID)
        {
            if (VAF_WeatherInfo_ID < 1) throw new ArgumentException("VAF_WeatherInfo_ID is mandatory.");
            Set_ValueNoCheck("VAF_WeatherInfo_ID", VAF_WeatherInfo_ID);
        }
        /** Get VAF_WeatherInfo_ID.
        @return VAF_WeatherInfo_ID */
        public int GetVAF_WeatherInfo_ID()
        {
            Object ii = Get_Value("VAF_WeatherInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set City Name.
        @param City Identifies a City */
        public void SetCity(String City)
        {
            if (City != null && City.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                City = City.Substring(0, 100);
            }
            Set_Value("City", City);
        }
        /** Get City Name.
        @return Identifies a City */
        public String GetCity()
        {
            return (String)Get_Value("City");
        }
        /** Set Country.
        @param Country Country */
        public void SetCountry(String Country)
        {
            if (Country != null && Country.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Country = Country.Substring(0, 100);
            }
            Set_Value("Country", Country);
        }
        /** Get Country.
        @return Country */
        public String GetCountry()
        {
            return (String)Get_Value("Country");
        }
    }

}
