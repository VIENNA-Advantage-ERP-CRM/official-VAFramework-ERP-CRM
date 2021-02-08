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
    /** Generated Model for VAB_Address
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_Address : PO
    {
        public X_VAB_Address(Context ctx, int VAB_Address_ID, Trx trxName)
            : base(ctx, VAB_Address_ID, trxName)
        {
            /** if (VAB_Address_ID == 0)
            {
            SetVAB_Country_ID (0);
            SetVAB_Address_ID (0);
            }
             */
        }
        public X_VAB_Address(Ctx ctx, int VAB_Address_ID, Trx trxName)
            : base(ctx, VAB_Address_ID, trxName)
        {
            /** if (VAB_Address_ID == 0)
            {
            SetVAB_Country_ID (0);
            SetVAB_Address_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Address(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Address(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Address(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_Address()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514373065L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056276L;
        /** VAF_TableView_ID=162 */
        public static int Table_ID;
        // =162;

        /** TableName=VAB_Address */
        public static String Table_Name = "VAB_Address";

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
            StringBuilder sb = new StringBuilder("X_VAB_Address[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Address 1.
        @param Address1 Address line 1 for this location */
        public void SetAddress1(String Address1)
        {
            if (Address1 != null && Address1.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Address1 = Address1.Substring(0, 200);
            }
            Set_Value("Address1", Address1);
        }
        /** Get Address 1.
        @return Address line 1 for this location */
        public String GetAddress1()
        {
            return (String)Get_Value("Address1");
        }
        /** Set Address 2.
        @param Address2 Address line 2 for this location */
        public void SetAddress2(String Address2)
        {
            if (Address2 != null && Address2.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address2 = Address2.Substring(0, 60);
            }
            Set_Value("Address2", Address2);
        }
        /** Get Address 2.
        @return Address line 2 for this location */
        public String GetAddress2()
        {
            return (String)Get_Value("Address2");
        }
        /** Set Address 3.
        @param Address3 Address Line 3 for the location */
        public void SetAddress3(String Address3)
        {
            if (Address3 != null && Address3.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address3 = Address3.Substring(0, 60);
            }
            Set_Value("Address3", Address3);
        }
        /** Get Address 3.
        @return Address Line 3 for the location */
        public String GetAddress3()
        {
            return (String)Get_Value("Address3");
        }
        /** Set Address 4.
        @param Address4 Address Line 4 for the location */
        public void SetAddress4(String Address4)
        {
            if (Address4 != null && Address4.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address4 = Address4.Substring(0, 60);
            }
            Set_Value("Address4", Address4);
        }
        /** Get Address 4.
        @return Address Line 4 for the location */
        public String GetAddress4()
        {
            return (String)Get_Value("Address4");
        }
        /** Set City.
        @param VAB_City_ID City */
        public void SetVAB_City_ID(int VAB_City_ID)
        {
            if (VAB_City_ID <= 0) Set_Value("VAB_City_ID", null);
            else
                Set_Value("VAB_City_ID", VAB_City_ID);
        }
        /** Get City.
        @return City */
        public int GetVAB_City_ID()
        {
            Object ii = Get_Value("VAB_City_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Country.
        @param VAB_Country_ID Country */
        public void SetVAB_Country_ID(int VAB_Country_ID)
        {
            if (VAB_Country_ID < 1) throw new ArgumentException("VAB_Country_ID is mandatory.");
            Set_Value("VAB_Country_ID", VAB_Country_ID);
        }
        /** Get Country.
        @return Country */
        public int GetVAB_Country_ID()
        {
            Object ii = Get_Value("VAB_Country_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param VAB_Address_ID Location or Address */
        public void SetVAB_Address_ID(int VAB_Address_ID)
        {
            if (VAB_Address_ID < 1) throw new ArgumentException("VAB_Address_ID is mandatory.");
            Set_ValueNoCheck("VAB_Address_ID", VAB_Address_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetVAB_Address_ID()
        {
            Object ii = Get_Value("VAB_Address_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Region.
        @param VAB_RegionState_ID Identifies a geographical Region */
        public void SetVAB_RegionState_ID(int VAB_RegionState_ID)
        {
            if (VAB_RegionState_ID <= 0) Set_Value("VAB_RegionState_ID", null);
            else
                Set_Value("VAB_RegionState_ID", VAB_RegionState_ID);
        }
        /** Get Region.
        @return Identifies a geographical Region */
        public int GetVAB_RegionState_ID()
        {
            Object ii = Get_Value("VAB_RegionState_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set City.
        @param City Identifies a City */
        public void SetCity(String City)
        {
            if (City != null && City.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                City = City.Substring(0, 60);
            }
            Set_Value("City", City);
        }
        /** Get City.
        @return Identifies a City */
        public String GetCity()
        {
            return (String)Get_Value("City");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetCity());
        }
        /** Set ZIP.
        @param Postal Postal code */
        public void SetPostal(String Postal)
        {
            if (Postal != null && Postal.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                Postal = Postal.Substring(0, 10);
            }
            Set_Value("Postal", Postal);
        }
        /** Get ZIP.
        @return Postal code */
        public String GetPostal()
        {
            return (String)Get_Value("Postal");
        }
        /** Set -.
        @param Postal_Add Additional ZIP or Postal code */
        public void SetPostal_Add(String Postal_Add)
        {
            if (Postal_Add != null && Postal_Add.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                Postal_Add = Postal_Add.Substring(0, 10);
            }
            Set_Value("Postal_Add", Postal_Add);
        }
        /** Get -.
        @return Additional ZIP or Postal code */
        public String GetPostal_Add()
        {
            return (String)Get_Value("Postal_Add");
        }
        /** Set Region.
        @param RegionName Name of the Region */
        public void SetRegionName(String RegionName)
        {
            if (RegionName != null && RegionName.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                RegionName = RegionName.Substring(0, 40);
            }
            Set_Value("RegionName", RegionName);
        }
        /** Get Region.
        @return Name of the Region */
        public String GetRegionName()
        {
            return (String)Get_Value("RegionName");
        }
        /** Set Address no.
        @param SAP001_AddressNo Address no */
        public void SetSAP001_AddressNo(String SAP001_AddressNo)
        {
            if (SAP001_AddressNo != null && SAP001_AddressNo.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                SAP001_AddressNo = SAP001_AddressNo.Substring(0, 200);
            }
            Set_Value("SAP001_AddressNo", SAP001_AddressNo);
        }
        /** Get Address no.
        @return Address no */
        public String GetSAP001_AddressNo()
        {
            return (String)Get_Value("SAP001_AddressNo");
        }


        /** Set Latitude.
        @param Latitude Latitude */
        public void SetLatitude(String Latitude) { if (Latitude != null && Latitude.Length > 100) { log.Warning("Length > 100 - truncated"); Latitude = Latitude.Substring(0, 100); } Set_Value("Latitude", Latitude); }/** Get Latitude.
@return Latitude */
        public String GetLatitude() { return (String)Get_Value("Latitude"); }/** Set Longitude.
@param Longitude Longitude */
        public void SetLongitude(String Longitude) { if (Longitude != null && Longitude.Length > 100) { log.Warning("Length > 100 - truncated"); Longitude = Longitude.Substring(0, 100); } Set_Value("Longitude", Longitude); }/** Get Longitude.
@return Longitude */
        public String GetLongitude() { return (String)Get_Value("Longitude"); }

    }
}
