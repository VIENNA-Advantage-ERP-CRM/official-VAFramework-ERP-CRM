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
    /** Generated Model for AD_WeatherInfo
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_WeatherInfo : PO
    {
        public X_AD_WeatherInfo(Context ctx, int AD_WeatherInfo_ID, Trx trxName)
            : base(ctx, AD_WeatherInfo_ID, trxName)
        {
            /** if (AD_WeatherInfo_ID == 0)
            {
            SetAD_WeatherInfo_ID (0);
            }
             */
        }
        public X_AD_WeatherInfo(Ctx ctx, int AD_WeatherInfo_ID, Trx trxName)
            : base(ctx, AD_WeatherInfo_ID, trxName)
        {
            /** if (AD_WeatherInfo_ID == 0)
            {
            SetAD_WeatherInfo_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WeatherInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WeatherInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WeatherInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_WeatherInfo()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27603461129658L;
        /** Last Updated Timestamp 11/15/2011 11:13:32 AM */
        public static long updatedMS = 1321335812869L;
        /** AD_Table_ID=1000028 */
        public static int Table_ID;
        // =1000028;

        /** TableName=AD_WeatherInfo */
        public static String Table_Name = "AD_WeatherInfo";

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
            StringBuilder sb = new StringBuilder("X_AD_WeatherInfo[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AD_WeatherInfo_ID.
        @param AD_WeatherInfo_ID AD_WeatherInfo_ID */
        public void SetAD_WeatherInfo_ID(int AD_WeatherInfo_ID)
        {
            if (AD_WeatherInfo_ID < 1) throw new ArgumentException("AD_WeatherInfo_ID is mandatory.");
            Set_ValueNoCheck("AD_WeatherInfo_ID", AD_WeatherInfo_ID);
        }
        /** Get AD_WeatherInfo_ID.
        @return AD_WeatherInfo_ID */
        public int GetAD_WeatherInfo_ID()
        {
            Object ii = Get_Value("AD_WeatherInfo_ID");
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
