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
    /** Generated Model for AD_User
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_User : PO
    {
        public X_AD_User(Context ctx, int AD_User_ID, Trx trxName)
            : base(ctx, AD_User_ID, trxName)
        {
            /** if (AD_User_ID == 0)
            {
            SetAD_User_ID (0);
            SetIsFullBPAccess (true);	// Y
            SetName (null);
            SetNotificationType (null);	// E
            SetValue (null);
            }
             */
        }
        public X_AD_User(Ctx ctx, int AD_User_ID, Trx trxName)
            : base(ctx, AD_User_ID, trxName)
        {
            /** if (AD_User_ID == 0)
            {
            SetAD_User_ID (0);
            SetIsFullBPAccess (true);	// Y
            SetName (null);
            SetNotificationType (null);	// E
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_User(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_User(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_User(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_User()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27626193958544L;
        /** Last Updated Timestamp 8/4/2012 1:54:03 PM */
        public static long updatedMS = 1344068641755L;
        /** AD_Table_ID=114 */
        public static int Table_ID;
        // =114;

        /** TableName=AD_User */
        public static String Table_Name = "AD_User";

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
            StringBuilder sb = new StringBuilder("X_AD_User[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Image.
        @param AD_Image_ID Image or Icon */
        public void SetAD_Image_ID(int AD_Image_ID)
        {
            if (AD_Image_ID <= 0) Set_Value("AD_Image_ID", null);
            else
                Set_Value("AD_Image_ID", AD_Image_ID);
        }
        /** Get Image.
        @return Image or Icon */
        public int GetAD_Image_ID()
        {
            Object ii = Get_Value("AD_Image_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Role.
        @param AD_Role_ID Responsibility Role */
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID <= 0) Set_Value("AD_Role_ID", null);
            else
                Set_Value("AD_Role_ID", AD_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetAD_Role_ID()
        {
            Object ii = Get_Value("AD_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_MenuFavorite_ID AD_Reference_ID=184 */
        public static int AD_TREE_MENUFAVORITE_ID_AD_Reference_ID = 184;
        /** Set Favorite Menu Tree.
        @param AD_Tree_MenuFavorite_ID Tree of the personal Favorite menu */
        public void SetAD_Tree_MenuFavorite_ID(int AD_Tree_MenuFavorite_ID)
        {
            if (AD_Tree_MenuFavorite_ID <= 0) Set_ValueNoCheck("AD_Tree_MenuFavorite_ID", null);
            else
                Set_ValueNoCheck("AD_Tree_MenuFavorite_ID", AD_Tree_MenuFavorite_ID);
        }
        /** Get Favorite Menu Tree.
        @return Tree of the personal Favorite menu */
        public int GetAD_Tree_MenuFavorite_ID()
        {
            Object ii = Get_Value("AD_Tree_MenuFavorite_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_MenuNew_ID AD_Reference_ID=184 */
        public static int AD_TREE_MENUNEW_ID_AD_Reference_ID = 184;
        /** Set New Menu Tree.
        @param AD_Tree_MenuNew_ID Tree of the personal Favorite menu for new items */
        public void SetAD_Tree_MenuNew_ID(int AD_Tree_MenuNew_ID)
        {
            if (AD_Tree_MenuNew_ID <= 0) Set_ValueNoCheck("AD_Tree_MenuNew_ID", null);
            else
                Set_ValueNoCheck("AD_Tree_MenuNew_ID", AD_Tree_MenuNew_ID);
        }
        /** Get New Menu Tree.
        @return Tree of the personal Favorite menu for new items */
        public int GetAD_Tree_MenuNew_ID()
        {
            Object ii = Get_Value("AD_Tree_MenuNew_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID < 1) throw new ArgumentException("AD_User_ID is mandatory.");
            Set_ValueNoCheck("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BLOOD_GROUP.
        @param BLOOD_GROUP BLOOD_GROUP */
        public void SetBLOOD_GROUP(String BLOOD_GROUP)
        {
            if (BLOOD_GROUP != null && BLOOD_GROUP.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                BLOOD_GROUP = BLOOD_GROUP.Substring(0, 10);
            }
            Set_Value("BLOOD_GROUP", BLOOD_GROUP);
        }
        /** Get BLOOD_GROUP.
        @return BLOOD_GROUP */
        public String GetBLOOD_GROUP()
        {
            return (String)Get_Value("BLOOD_GROUP");
        }
        /** Set Birthday.
        @param Birthday Birthday or Anniversary day */
        public void SetBirthday(DateTime? Birthday)
        {
            Set_Value("Birthday", (DateTime?)Birthday);
        }
        /** Get Birthday.
        @return Birthday or Anniversary day */
        public DateTime? GetBirthday()
        {
            return (DateTime?)Get_Value("Birthday");
        }
        /** Set Bounced Info.
        @param BouncedInfo Information about the cause of bounce */
        public void SetBouncedInfo(String BouncedInfo)
        {
            if (BouncedInfo != null && BouncedInfo.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                BouncedInfo = BouncedInfo.Substring(0, 60);
            }
            Set_Value("BouncedInfo", BouncedInfo);
        }
        /** Get Bounced Info.
        @return Information about the cause of bounce */
        public String GetBouncedInfo()
        {
            return (String)Get_Value("BouncedInfo");
        }
        /** Set COMPANY_NAME.
        @param COMPANY_NAME Name Of the Company */
        public void SetCOMPANY_NAME(String COMPANY_NAME)
        {
            if (COMPANY_NAME != null && COMPANY_NAME.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                COMPANY_NAME = COMPANY_NAME.Substring(0, 50);
            }
            Set_Value("COMPANY_NAME", COMPANY_NAME);
        }
        /** Get COMPANY_NAME.
        @return Name Of the Company */
        public String GetCOMPANY_NAME()
        {
            return (String)Get_Value("COMPANY_NAME");
        }
        /** Set Customer/Prospect.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Customer/Prospect.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Location.
        @param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Location.
        @return Identifies the address for this Account/Prospect. */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Greeting.
        @param C_Greeting_ID Greeting to print on correspondence */
        public void SetC_Greeting_ID(int C_Greeting_ID)
        {
            if (C_Greeting_ID <= 0) Set_Value("C_Greeting_ID", null);
            else
                Set_Value("C_Greeting_ID", C_Greeting_ID);
        }
        /** Get Greeting.
        @return Greeting to print on correspondence */
        public int GetC_Greeting_ID()
        {
            Object ii = Get_Value("C_Greeting_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Position.
        @param C_Job_ID Job Position */
        public void SetC_Job_ID(int C_Job_ID)
        {
            if (C_Job_ID <= 0) Set_Value("C_Job_ID", null);
            else
                Set_Value("C_Job_ID", C_Job_ID);
        }
        /** Get Position.
        @return Job Position */
        public int GetC_Job_ID()
        {
            Object ii = Get_Value("C_Job_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param C_Location_ID Location or Address */
        public void SetC_Location_ID(int C_Location_ID)
        {
            if (C_Location_ID <= 0) Set_Value("C_Location_ID", null);
            else
                Set_Value("C_Location_ID", C_Location_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetC_Location_ID()
        {
            Object ii = Get_Value("C_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Comments.
        @param Comments Comments or additional information */
        public void SetComments(String Comments)
        {
            if (Comments != null && Comments.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Comments = Comments.Substring(0, 2000);
            }
            Set_Value("Comments", Comments);
        }
        /** Get Comments.
        @return Comments or additional information */
        public String GetComments()
        {
            return (String)Get_Value("Comments");
        }

        /** ConnectionProfile AD_Reference_ID=364 */
        public static int CONNECTIONPROFILE_AD_Reference_ID = 364;
        /** LAN = L */
        public static String CONNECTIONPROFILE_LAN = "L";
        /** Terminal Server = T */
        public static String CONNECTIONPROFILE_TerminalServer = "T";
        /** VPN = V */
        public static String CONNECTIONPROFILE_VPN = "V";
        /** WAN = W */
        public static String CONNECTIONPROFILE_WAN = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsConnectionProfileValid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("T") || test.Equals("V") || test.Equals("W");
        }
        /** Set Connection Profile.
        @param ConnectionProfile How a Java Client connects to the server(s) */
        public void SetConnectionProfile(String ConnectionProfile)
        {
            if (!IsConnectionProfileValid(ConnectionProfile))
                throw new ArgumentException("ConnectionProfile Invalid value - " + ConnectionProfile + " - Reference_ID=364 - L - T - V - W");
            if (ConnectionProfile != null && ConnectionProfile.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ConnectionProfile = ConnectionProfile.Substring(0, 1);
            }
            Set_Value("ConnectionProfile", ConnectionProfile);
        }
        /** Get Connection Profile.
        @return How a Java Client connects to the server(s) */
        public String GetConnectionProfile()
        {
            return (String)Get_Value("ConnectionProfile");
        }
        /** Set Country Code.
        @param CountryCodeForMobile Country Code */
        public void SetCountryCodeForMobile(String CountryCodeForMobile)
        {
            if (CountryCodeForMobile != null && CountryCodeForMobile.Length > 5)
            {
                log.Warning("Length > 5 - truncated");
                CountryCodeForMobile = CountryCodeForMobile.Substring(0, 5);
            }
            Set_Value("CountryCodeForMobile", CountryCodeForMobile);
        }
        /** Get Country Code.
        @return Country Code */
        public String GetCountryCodeForMobile()
        {
            return (String)Get_Value("CountryCodeForMobile");
        }
        /** Set Joining Date.
        @param DATE_JOINING Joining Date */
        public void SetDATE_JOINING(DateTime? DATE_JOINING)
        {
            Set_Value("DATE_JOINING", (DateTime?)DATE_JOINING);
        }
        /** Get Joining Date.
        @return Joining Date */
        public DateTime? GetDATE_JOINING()
        {
            return (DateTime?)Get_Value("DATE_JOINING");
        }
        /** Set DateRelieved.
        @param DateRelieved DateRelieved */
        public void SetDateRelieved(DateTime? DateRelieved)
        {
            Set_Value("DateRelieved", (DateTime?)DateRelieved);
        }
        /** Get DateRelieved.
        @return DateRelieved */
        public DateTime? GetDateRelieved()
        {
            return (DateTime?)Get_Value("DateRelieved");
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
        /** Set Diploma.
        @param Diploma Diploma */
        public void SetDiploma(String Diploma)
        {
            if (Diploma != null && Diploma.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Diploma = Diploma.Substring(0, 50);
            }
            Set_Value("Diploma", Diploma);
        }
        /** Get Diploma.
        @return Diploma */
        public String GetDiploma()
        {
            return (String)Get_Value("Diploma");
        }
        /** Set EMPLOY_OLDNO.
        @param EMPLOY_OLDNO EMPLOY_OLDNO */
        public void SetEMPLOY_OLDNO(String EMPLOY_OLDNO)
        {
            if (EMPLOY_OLDNO != null && EMPLOY_OLDNO.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                EMPLOY_OLDNO = EMPLOY_OLDNO.Substring(0, 20);
            }
            Set_Value("EMPLOY_OLDNO", EMPLOY_OLDNO);
        }
        /** Get EMPLOY_OLDNO.
        @return EMPLOY_OLDNO */
        public String GetEMPLOY_OLDNO()
        {
            return (String)Get_Value("EMPLOY_OLDNO");
        }
        /** Set EMail Address.
        @param EMail Electronic Mail Address */
        public void SetEMail(String EMail)
        {
            if (EMail != null && EMail.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                EMail = EMail.Substring(0, 100);
            }
            Set_Value("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
        }
        /** Set EMail User ID.
        @param EMailUser User Name (ID) in the Mail System */
        public void SetEMailUser(String EMailUser)
        {
            if (EMailUser != null && EMailUser.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                EMailUser = EMailUser.Substring(0, 100);
            }
            Set_Value("EMailUser", EMailUser);
        }
        /** Get EMail User ID.
        @return User Name (ID) in the Mail System */
        public String GetEMailUser()
        {
            return (String)Get_Value("EMailUser");
        }
        /** Set EMail User Password.
        @param EMailUserPW Password of your email user id */
        public void SetEMailUserPW(String EMailUserPW)
        {
            if (EMailUserPW != null && EMailUserPW.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                EMailUserPW = EMailUserPW.Substring(0, 60);
            }
            Set_Value("EMailUserPW", EMailUserPW);
        }
        /** Get EMail User Password.
        @return Password of your email user id */
        public String GetEMailUserPW()
        {
            return (String)Get_Value("EMailUserPW");
        }
        /** Set Verification Info.
        @param EMailVerify Verification information of EMail Address */
        public void SetEMailVerify(String EMailVerify)
        {
            if (EMailVerify != null && EMailVerify.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                EMailVerify = EMailVerify.Substring(0, 10);
            }
            Set_ValueNoCheck("EMailVerify", EMailVerify);
        }
        /** Get Verification Info.
        @return Verification information of EMail Address */
        public String GetEMailVerify()
        {
            return (String)Get_Value("EMailVerify");
        }
        /** Set EMail Verify.
        @param EMailVerifyDate Date Email was verified */
        public void SetEMailVerifyDate(DateTime? EMailVerifyDate)
        {
            Set_ValueNoCheck("EMailVerifyDate", (DateTime?)EMailVerifyDate);
        }
        /** Get EMail Verify.
        @return Date Email was verified */
        public DateTime? GetEMailVerifyDate()
        {
            return (DateTime?)Get_Value("EMailVerifyDate");
        }
        /** Set Email Opt Out.
        @param EmailOptOut Email Opt Out */
        public void SetEmailOptOut(Boolean EmailOptOut)
        {
            Set_Value("EmailOptOut", EmailOptOut);
        }
        /** Get Email Opt Out.
        @return Email Opt Out */
        public Boolean IsEmailOptOut()
        {
            Object oo = Get_Value("EmailOptOut");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Fax.
        @param Fax Facsimile number */
        public void SetFax(String Fax)
        {
            if (Fax != null && Fax.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Fax = Fax.Substring(0, 40);
            }
            Set_Value("Fax", Fax);
        }
        /** Get Fax.
        @return Facsimile number */
        public String GetFax()
        {
            return (String)Get_Value("Fax");
        }
        /** Set Fax EMail.
        @param FaxEMail Fax EMail */
        public void SetFaxEMail(String FaxEMail)
        {
            if (FaxEMail != null && FaxEMail.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                FaxEMail = FaxEMail.Substring(0, 255);
            }
            Set_Value("FaxEMail", FaxEMail);
        }
        /** Get Fax EMail.
        @return Fax EMail */
        public String GetFaxEMail()
        {
            return (String)Get_Value("FaxEMail");
        }
        /** Set Fax EMail Domain.
        @param FaxEMailDomain Fax EMail Domain */
        public void SetFaxEMailDomain(String FaxEMailDomain)
        {
            if (FaxEMailDomain != null && FaxEMailDomain.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                FaxEMailDomain = FaxEMailDomain.Substring(0, 20);
            }
            Set_Value("FaxEMailDomain", FaxEMailDomain);
        }
        /** Get Fax EMail Domain.
        @return Fax EMail Domain */
        public String GetFaxEMailDomain()
        {
            return (String)Get_Value("FaxEMailDomain");
        }
        /** Set Graduation.
        @param Graduation Graduation */
        public void SetGraduation(String Graduation)
        {
            if (Graduation != null && Graduation.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Graduation = Graduation.Substring(0, 50);
            }
            Set_Value("Graduation", Graduation);
        }
        /** Get Graduation.
        @return Graduation */
        public String GetGraduation()
        {
            return (String)Get_Value("Graduation");
        }

        /** HEALTH AD_Reference_ID=1000011 */
        public static int HEALTH_AD_Reference_ID = 1000011;
        /** Normal = NO */
        public static String HEALTH_Normal = "NO";
        /** Special1 = S1 */
        public static String HEALTH_Special1 = "S1";
        /** Special2 = S2 */
        public static String HEALTH_Special2 = "S2";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsHEALTHValid(String test)
        {
            return test == null || test.Equals("NO") || test.Equals("S1") || test.Equals("S2");
        }
        /** Set HEALTH.
        @param HEALTH HEALTH */
        public void SetHEALTH(String HEALTH)
        {
            if (!IsHEALTHValid(HEALTH))
                throw new ArgumentException("HEALTH Invalid value - " + HEALTH + " - Reference_ID=1000011 - NO - S1 - S2");
            if (HEALTH != null && HEALTH.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                HEALTH = HEALTH.Substring(0, 2);
            }
            Set_Value("HEALTH", HEALTH);
        }
        /** Get HEALTH.
        @return HEALTH */
        public String GetHEALTH()
        {
            return (String)Get_Value("HEALTH");
        }
        /** Set ISEMPLOYEECONTACT.
        @param ISEMPLOYEECONTACT ISEMPLOYEECONTACT */
        public void SetISEMPLOYEECONTACT(Boolean ISEMPLOYEECONTACT)
        {
            Set_Value("ISEMPLOYEECONTACT", ISEMPLOYEECONTACT);
        }
        /** Get ISEMPLOYEECONTACT.
        @return ISEMPLOYEECONTACT */
        public Boolean IsEMPLOYEECONTACT()
        {
            Object oo = Get_Value("ISEMPLOYEECONTACT");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set EMail Bounced.
        @param IsEMailBounced The email delivery bounced */
        public void SetIsEMailBounced(Boolean IsEMailBounced)
        {
            Set_Value("IsEMailBounced", IsEMailBounced);
        }
        /** Get EMail Bounced.
        @return The email delivery bounced */
        public Boolean IsEMailBounced()
        {
            Object oo = Get_Value("IsEMailBounced");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Full BP Access.
        @param IsFullBPAccess The user/contact has full access to Business Partner information and resources */
        public void SetIsFullBPAccess(Boolean IsFullBPAccess)
        {
            Set_Value("IsFullBPAccess", IsFullBPAccess);
        }
        /** Get Full BP Access.
        @return The user/contact has full access to Business Partner information and resources */
        public Boolean IsFullBPAccess()
        {
            Object oo = Get_Value("IsFullBPAccess");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Login User.
        @param IsLoginUser Identifies whether user is a login user */
        public void SetIsLoginUser(Boolean IsLoginUser)
        {
            Set_Value("IsLoginUser", IsLoginUser);
        }
        /** Get Login User.
        @return Identifies whether user is a login user */
        public Boolean IsLoginUser()
        {
            Object oo = Get_Value("IsLoginUser");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set LASTREGISTRATIONREMINDER.
        @param LASTREGISTRATIONREMINDER LASTREGISTRATIONREMINDER */
        public void SetLASTREGISTRATIONREMINDER(DateTime? LASTREGISTRATIONREMINDER)
        {
            Set_Value("LASTREGISTRATIONREMINDER", (DateTime?)LASTREGISTRATIONREMINDER);
        }
        /** Get LASTREGISTRATIONREMINDER.
        @return LASTREGISTRATIONREMINDER */
        public DateTime? GetLASTREGISTRATIONREMINDER()
        {
            return (DateTime?)Get_Value("LASTREGISTRATIONREMINDER");
        }
        /** Set LDAP User Name.
        @param LDAPUser User Name used for authorization via LDAP (directory) services */
        public void SetLDAPUser(String LDAPUser)
        {
            if (LDAPUser != null && LDAPUser.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                LDAPUser = LDAPUser.Substring(0, 60);
            }
            Set_Value("LDAPUser", LDAPUser);
        }
        /** Get LDAP User Name.
        @return User Name used for authorization via LDAP (directory) services */
        public String GetLDAPUser()
        {
            return (String)Get_Value("LDAPUser");
        }
        /** Set Last Contact.
        @param LastContact Date this individual was last contacted */
        public void SetLastContact(DateTime? LastContact)
        {
            Set_Value("LastContact", (DateTime?)LastContact);
        }
        /** Get Last Contact.
        @return Date this individual was last contacted */
        public DateTime? GetLastContact()
        {
            return (DateTime?)Get_Value("LastContact");
        }
        /** Set Last Result.
        @param LastResult Result of last contact */
        public void SetLastResult(String LastResult)
        {
            if (LastResult != null && LastResult.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LastResult = LastResult.Substring(0, 255);
            }
            Set_Value("LastResult", LastResult);
        }
        /** Get Last Result.
        @return Result of last contact */
        public String GetLastResult()
        {
            return (String)Get_Value("LastResult");
        }
        /** Set Masters.
        @param Masters Masters */
        public void SetMasters(String Masters)
        {
            if (Masters != null && Masters.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Masters = Masters.Substring(0, 50);
            }
            Set_Value("Masters", Masters);
        }
        /** Get Masters.
        @return Masters */
        public String GetMasters()
        {
            return (String)Get_Value("Masters");
        }
        /** Set Mobile.
        @param Mobile Identifies the mobile number. */
        public void SetMobile(String Mobile)
        {
            if (Mobile != null && Mobile.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Mobile = Mobile.Substring(0, 50);
            }
            Set_Value("Mobile", Mobile);
        }
        /** Get Mobile.
        @return Identifies the mobile number. */
        public String GetMobile()
        {
            return (String)Get_Value("Mobile");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
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

        /** NotificationType AD_Reference_ID=344 */
        public static int NOTIFICATIONTYPE_AD_Reference_ID = 344;
        /** FaxEMail = A */
        public static String NOTIFICATIONTYPE_FaxEMail = "A";
        /** EMail+Notice = B */
        public static String NOTIFICATIONTYPE_EMailPlusNotice = "B";
        /** Notice+FaxEmail = C */
        public static String NOTIFICATIONTYPE_NoticePlusFaxEMail = "C";
        /** EMail = E */
        public static String NOTIFICATIONTYPE_EMail = "E";
        /** EMail+FaxEMail = L */
        public static String NOTIFICATIONTYPE_EMailPlusFaxEMail = "L";
        /** Notice = N */
        public static String NOTIFICATIONTYPE_Notice = "N";
        /** SMS = S */
        public static String NOTIFICATIONTYPE_SMS = "S";
        /** None = X */
        public static String NOTIFICATIONTYPE_None = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsNotificationTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("E") || test.Equals("L") || test.Equals("N") || test.Equals("S") || test.Equals("X");
        }
        /** Set Notification Type.
        @param NotificationType Type of Notifications */
        public void SetNotificationType(String NotificationType)
        {
            if (NotificationType == null) throw new ArgumentException("NotificationType is mandatory");
            if (!IsNotificationTypeValid(NotificationType))
                throw new ArgumentException("NotificationType Invalid value - " + NotificationType + " - Reference_ID=344 - A - B - E - L - N - S - X");
            if (NotificationType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                NotificationType = NotificationType.Substring(0, 1);
            }
            Set_Value("NotificationType", NotificationType);
        }
        /** Get Notification Type.
        @return Type of Notifications */
        public String GetNotificationType()
        {
            return (String)Get_Value("NotificationType");
        }
        /** Set PassExpDate.
        @param PassExpDate PassExpDate */
        public void SetPassExpDate(DateTime? PassExpDate)
        {
            Set_Value("PassExpDate", (DateTime?)PassExpDate);
        }
        /** Get PassExpDate.
        @return PassExpDate */
        public DateTime? GetPassExpDate()
        {
            return (DateTime?)Get_Value("PassExpDate");
        }
        /** Set PassNo.
        @param PassNo PassNo */
        public void SetPassNo(String PassNo)
        {
            if (PassNo != null && PassNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                PassNo = PassNo.Substring(0, 50);
            }
            Set_Value("PassNo", PassNo);
        }
        /** Get PassNo.
        @return PassNo */
        public String GetPassNo()
        {
            return (String)Get_Value("PassNo");
        }
        /** Set Password.
        @param Password Password of any length (case sensitive) */
        public void SetPassword(String Password)
        {
            if (Password != null && Password.Length > 250)
            {
                log.Warning("Length > 250 - truncated");
                Password = Password.Substring(0, 250);
            }
            Set_Value("Password", Password);
        }
        /** Get Password.
        @return Password of any length (case sensitive) */
        public String GetPassword()
        {
            return (String)Get_Value("Password");
        }
        /** Set DiplomaPercentage.
        @param Percentage_Dip DiplomaPercentage */
        public void SetPercentage_Dip(Decimal? Percentage_Dip)
        {
            Set_Value("Percentage_Dip", (Decimal?)Percentage_Dip);
        }
        /** Get DiplomaPercentage.
        @return DiplomaPercentage */
        public Decimal GetPercentage_Dip()
        {
            Object bd = Get_Value("Percentage_Dip");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set DoctoratePercentage.
        @param Percentage_Doc DoctoratePercentage */
        public void SetPercentage_Doc(Decimal? Percentage_Doc)
        {
            Set_Value("Percentage_Doc", (Decimal?)Percentage_Doc);
        }
        /** Get DoctoratePercentage.
        @return DoctoratePercentage */
        public Decimal GetPercentage_Doc()
        {
            Object bd = Get_Value("Percentage_Doc");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set GraduationPercentage.
        @param Percentage_Gr GraduationPercentage */
        public void SetPercentage_Gr(Decimal? Percentage_Gr)
        {
            Set_Value("Percentage_Gr", (Decimal?)Percentage_Gr);
        }
        /** Get GraduationPercentage.
        @return GraduationPercentage */
        public Decimal GetPercentage_Gr()
        {
            Object bd = Get_Value("Percentage_Gr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set MastersPercentage.
        @param Percentage_MS MastersPercentage */
        public void SetPercentage_MS(Decimal? Percentage_MS)
        {
            Set_Value("Percentage_MS", (Decimal?)Percentage_MS);
        }
        /** Get MastersPercentage.
        @return MastersPercentage */
        public Decimal GetPercentage_MS()
        {
            Object bd = Get_Value("Percentage_MS");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Phone.
        @param Phone Identifies a telephone number */
        public void SetPhone(String Phone)
        {
            if (Phone != null && Phone.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Phone = Phone.Substring(0, 40);
            }
            Set_Value("Phone", Phone);
        }
        /** Get Phone.
        @return Identifies a telephone number */
        public String GetPhone()
        {
            return (String)Get_Value("Phone");
        }
        /** Set 2nd Phone.
        @param Phone2 Identifies an alternate telephone number. */
        public void SetPhone2(String Phone2)
        {
            if (Phone2 != null && Phone2.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Phone2 = Phone2.Substring(0, 40);
            }
            Set_Value("Phone2", Phone2);
        }
        /** Get 2nd Phone.
        @return Identifies an alternate telephone number. */
        public String GetPhone2()
        {
            return (String)Get_Value("Phone2");
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set QUALIFICATION.
        @param QUALIFICATION QUALIFICATION */
        public void SetQUALIFICATION(String QUALIFICATION)
        {
            if (QUALIFICATION != null && QUALIFICATION.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                QUALIFICATION = QUALIFICATION.Substring(0, 50);
            }
            Set_Value("QUALIFICATION", QUALIFICATION);
        }
        /** Get QUALIFICATION.
        @return QUALIFICATION */
        public String GetQUALIFICATION()
        {
            return (String)Get_Value("QUALIFICATION");
        }
        /** Set RELATIONSHIP.
        @param RELATIONSHIP RELATIONSHIP */
        public void SetRELATIONSHIP(String RELATIONSHIP)
        {
            if (RELATIONSHIP != null && RELATIONSHIP.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                RELATIONSHIP = RELATIONSHIP.Substring(0, 50);
            }
            Set_Value("RELATIONSHIP", RELATIONSHIP);
        }
        /** Get RELATIONSHIP.
        @return RELATIONSHIP */
        public String GetRELATIONSHIP()
        {
            return (String)Get_Value("RELATIONSHIP");
        }
        /** Set RelivingReason.
        @param RelivingReason RelivingReason */
        public void SetRelivingReason(String RelivingReason)
        {
            if (RelivingReason != null && RelivingReason.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                RelivingReason = RelivingReason.Substring(0, 50);
            }
            Set_Value("RelivingReason", RelivingReason);
        }
        /** Get RelivingReason.
        @return RelivingReason */
        public String GetRelivingReason()
        {
            return (String)Get_Value("RelivingReason");
        }

        /** Supervisor_ID AD_Reference_ID=110 */
        public static int SUPERVISOR_ID_AD_Reference_ID = 110;
        /** Set Supervisor.
        @param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
        public void SetSupervisor_ID(int Supervisor_ID)
        {
            if (Supervisor_ID <= 0) Set_Value("Supervisor_ID", null);
            else
                Set_Value("Supervisor_ID", Supervisor_ID);
        }
        /** Get Supervisor.
        @return Supervisor for this user/organization - used for escalation and approval */
        public int GetSupervisor_ID()
        {
            Object ii = Get_Value("Supervisor_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Title.
        @param Title Title of the Contact */
        public void SetTitle(String Title)
        {
            if (Title != null && Title.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Title = Title.Substring(0, 40);
            }
            Set_Value("Title", Title);
        }
        /** Get Title.
        @return Title of the Contact */
        public String GetTitle()
        {
            return (String)Get_Value("Title");
        }
        /** Set Transfer Ownership.
        @param TransferOwnership This process is used to transfer ownership of users */
        public void SetTransferOwnership(String TransferOwnership)
        {
            if (TransferOwnership != null && TransferOwnership.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                TransferOwnership = TransferOwnership.Substring(0, 50);
            }
            Set_Value("TransferOwnership", TransferOwnership);
        }
        /** Get Transfer Ownership.
        @return This process is used to transfer ownership of users */
        public String GetTransferOwnership()
        {
            return (String)Get_Value("TransferOwnership");
        }
        /** Set USERPIN.
        @param USERPIN USERPIN */
        public void SetUSERPIN(String USERPIN)
        {
            if (USERPIN != null && USERPIN.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                USERPIN = USERPIN.Substring(0, 20);
            }
            Set_Value("USERPIN", USERPIN);
        }
        /** Get USERPIN.
        @return USERPIN */
        public String GetUSERPIN()
        {
            return (String)Get_Value("USERPIN");
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
        /** Set DiplomaYearofpassing.
        @param Yearofpassing_Dip DiplomaYearofpassing */
        public void SetYearofpassing_Dip(String Yearofpassing_Dip)
        {
            if (Yearofpassing_Dip != null && Yearofpassing_Dip.Length > 15)
            {
                log.Warning("Length > 15 - truncated");
                Yearofpassing_Dip = Yearofpassing_Dip.Substring(0, 15);
            }
            Set_Value("Yearofpassing_Dip", Yearofpassing_Dip);
        }
        /** Get DiplomaYearofpassing.
        @return DiplomaYearofpassing */
        public String GetYearofpassing_Dip()
        {
            return (String)Get_Value("Yearofpassing_Dip");
        }
        /** Set DoctorateYearofpassing.
        @param Yearofpassing_Doc Yearofpassing_Doc */
        public void SetYearofpassing_Doc(String Yearofpassing_Doc)
        {
            if (Yearofpassing_Doc != null && Yearofpassing_Doc.Length > 15)
            {
                log.Warning("Length > 15 - truncated");
                Yearofpassing_Doc = Yearofpassing_Doc.Substring(0, 15);
            }
            Set_Value("Yearofpassing_Doc", Yearofpassing_Doc);
        }
        /** Get DoctorateYearofpassing.
        @return Yearofpassing_Doc */
        public String GetYearofpassing_Doc()
        {
            return (String)Get_Value("Yearofpassing_Doc");
        }
        /** Set GraduationYearofpassing.
        @param Yearofpassing_Gr GraduationYearofpassing */
        public void SetYearofpassing_Gr(String Yearofpassing_Gr)
        {
            if (Yearofpassing_Gr != null && Yearofpassing_Gr.Length > 15)
            {
                log.Warning("Length > 15 - truncated");
                Yearofpassing_Gr = Yearofpassing_Gr.Substring(0, 15);
            }
            Set_Value("Yearofpassing_Gr", Yearofpassing_Gr);
        }
        /** Get GraduationYearofpassing.
        @return GraduationYearofpassing */
        public String GetYearofpassing_Gr()
        {
            return (String)Get_Value("Yearofpassing_Gr");
        }
        /** Set MastersYearofpassing.
        @param Yearofpassing_MS MastersYearofpassing */
        public void SetYearofpassing_MS(String Yearofpassing_MS)
        {
            if (Yearofpassing_MS != null && Yearofpassing_MS.Length > 15)
            {
                log.Warning("Length > 15 - truncated");
                Yearofpassing_MS = Yearofpassing_MS.Substring(0, 15);
            }
            Set_Value("Yearofpassing_MS", Yearofpassing_MS);
        }
        /** Get MastersYearofpassing.
        @return MastersYearofpassing */
        public String GetYearofpassing_MS()
        {
            return (String)Get_Value("Yearofpassing_MS");
        }

        /** Set CR Number.
        @param CRNo CR Number */
        public void SetCRNo(String CRNo)
        {
            if (CRNo != null && CRNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CRNo = CRNo.Substring(0, 50);
            }
            Set_Value("CRNo", CRNo);
        }
        /** Get CR Number.
        @return CR Number */
        public String GetCRNo()
        {
            return (String)Get_Value("CRNo");
        }

        /** Set CprNo.
        @param CprNo CprNo */
        public void SetCprNo(String CprNo)
        {
            if (CprNo != null && CprNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CprNo = CprNo.Substring(0, 50);
            }
            Set_Value("CprNo", CprNo);
        }
        /** Get CprNo.
        @return CprNo */
        public String GetCprNo()
        {
            return (String)Get_Value("CprNo");
        }
        /** Set CrpExpDate.
        @param CrpExpDate CrpExpDate */
        public void SetCrpExpDate(DateTime? CrpExpDate)
        {
            Set_Value("CrpExpDate", (DateTime?)CrpExpDate);
        }
        /** Get CrpExpDate.
        @return CrpExpDate */
        public DateTime? GetCrpExpDate()
        {
            return (DateTime?)Get_Value("CrpExpDate");
        }

        /** Set GosiNo.
        @param GosiNo GosiNo */
        public void SetGosiNo(String GosiNo)
        {
            if (GosiNo != null && GosiNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                GosiNo = GosiNo.Substring(0, 50);
            }
            Set_Value("GosiNo", GosiNo);
        }
        /** Get GosiNo.
        @return GosiNo */
        public String GetGosiNo()
        {
            return (String)Get_Value("GosiNo");
        }

        /** Set ResPerExpDate.
        @param ResPerExpDate ResPerExpDate */
        public void SetResPerExpDate(DateTime? ResPerExpDate)
        {
            Set_Value("ResPerExpDate", (DateTime?)ResPerExpDate);
        }
        /** Get ResPerExpDate.
        @return ResPerExpDate */
        public DateTime? GetResPerExpDate()
        {
            return (DateTime?)Get_Value("ResPerExpDate");
        }
        /** Set ResPerIssDate.
        @param ResPerIssDate ResPerIssDate */
        public void SetResPerIssDate(DateTime? ResPerIssDate)
        {
            Set_Value("ResPerIssDate", (DateTime?)ResPerIssDate);
        }
        /** Get ResPerIssDate.
        @return ResPerIssDate */
        public DateTime? GetResPerIssDate()
        {
            return (DateTime?)Get_Value("ResPerIssDate");
        }
        /** Set ResPerNo.
        @param ResPerNo ResPerNo */
        public void SetResPerNo(String ResPerNo)
        {
            if (ResPerNo != null && ResPerNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ResPerNo = ResPerNo.Substring(0, 50);
            }
            Set_Value("ResPerNo", ResPerNo);
        }
        /** Get ResPerNo.
        @return ResPerNo */
        public String GetResPerNo()
        {
            return (String)Get_Value("ResPerNo");
        }

        /** Set URL.
       @param URL Full URL address - e.g. http://www.viennaadvantage.com */
        public void SetGmail_UID(String Gmail_UID)
        {
            if (Gmail_UID != null && Gmail_UID.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Gmail_UID = Gmail_UID.Substring(0, 200);
            }
            Set_Value("Gmail_UID", Gmail_UID);
        }
        /** Get URL.
        @return Full URL address - e.g. http://www.viennaadvantage.com */
        public String GetGmail_UID()
        {
            return (String)Get_Value("Gmail_UID");
        }

        /** Set IsEmail.
@param IsEmail IsEmail */
        public void SetIsEmail(Boolean IsEmail)
        {
            Set_Value("IsEmail", IsEmail);
        }
        /** Get IsEmail.
        @return IsEmail */
        public Boolean IsEmail()
        {
            Object oo = Get_Value("IsEmail");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsSms.
@param IsSms IsSms */
        public void SetIsSms(Boolean IsSms)
        {
            Set_Value("IsSms", IsSms);
        }
        /** Get IsSms.
@return IsSms */
        public Boolean IsSms()
        {
            Object oo = Get_Value("IsSms");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Distribution Channel.
@param SAP001_DistributionChannel_ID Distribution Channel */
        public void SetSAP001_DistributionChannel_ID(int SAP001_DistributionChannel_ID)
        {
            if (SAP001_DistributionChannel_ID <= 0) Set_Value("SAP001_DistributionChannel_ID", null);
            else
                Set_Value("SAP001_DistributionChannel_ID", SAP001_DistributionChannel_ID);
        }
        /** Get Distribution Channel.
        @return Distribution Channel */
        public int GetSAP001_DistributionChannel_ID()
        {
            Object ii = Get_Value("SAP001_DistributionChannel_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Division.
        @param SAP001_Division_ID Division */
        public void SetSAP001_Division_ID(int SAP001_Division_ID)
        {
            if (SAP001_Division_ID <= 0) Set_Value("SAP001_Division_ID", null);
            else
                Set_Value("SAP001_Division_ID", SAP001_Division_ID);
        }
        /** Get Division.
        @return Division */
        public int GetSAP001_Division_ID()
        {
            Object ii = Get_Value("SAP001_Division_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Counter.
        @param SAP001_PartnerCounter Partner Counter */
        public void SetSAP001_PartnerCounter(String SAP001_PartnerCounter)
        {
            if (SAP001_PartnerCounter != null && SAP001_PartnerCounter.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                SAP001_PartnerCounter = SAP001_PartnerCounter.Substring(0, 50);
            }
            Set_Value("SAP001_PartnerCounter", SAP001_PartnerCounter);
        }
        /** Get Partner Counter.
        @return Partner Counter */
        public String GetSAP001_PartnerCounter()
        {
            return (String)Get_Value("SAP001_PartnerCounter");
        }

        /** SAP001_PartnerFunction AD_Reference_ID=1000194 */
        public static int SAP001_PARTNERFUNCTION_AD_Reference_ID = 1000194;
        /** Sold-to party = AG */
        public static String SAP001_PARTNERFUNCTION_Sold_ToParty = "AG";
        /** Bill-to party = RE */
        public static String SAP001_PARTNERFUNCTION_Bill_ToParty = "RE";
        /** Payer = RG */
        public static String SAP001_PARTNERFUNCTION_Payer = "RG";
        /** Ship-to party = WE */
        public static String SAP001_PARTNERFUNCTION_Ship_ToParty = "WE";
        /** EB Sales Staff = ZS */
        public static String SAP001_PARTNERFUNCTION_EBSalesStaff = "ZS";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSAP001_PartnerFunctionValid(String test)
        {
            return test == null || test.Equals("AG") || test.Equals("RE") || test.Equals("RG") || test.Equals("WE") || test.Equals("ZS");
        }
        /** Set Partner Function.
        @param SAP001_PartnerFunction Partner Function */
        public void SetSAP001_PartnerFunction(String SAP001_PartnerFunction)
        {
            if (!IsSAP001_PartnerFunctionValid(SAP001_PartnerFunction))
                throw new ArgumentException("SAP001_PartnerFunction Invalid value - " + SAP001_PartnerFunction + " - Reference_ID=1000194 - AG - RE - RG - WE - ZS");
            if (SAP001_PartnerFunction != null && SAP001_PartnerFunction.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                SAP001_PartnerFunction = SAP001_PartnerFunction.Substring(0, 2);
            }
            Set_Value("SAP001_PartnerFunction", SAP001_PartnerFunction);
        }
        /** Get Partner Function.
        @return Partner Function */
        public String GetSAP001_PartnerFunction()
        {
            return (String)Get_Value("SAP001_PartnerFunction");
        }
        /** Set BI User.
@param VA037_BIUser BI User */
        public void SetVA037_BIUser(Boolean VA037_BIUser) { Set_Value("VA037_BIUser", VA037_BIUser); }/** Get BI User.
@return BI User */
        public Boolean IsVA037_BIUser() { Object oo = Get_Value("VA037_BIUser"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Get Yellowfin Role.
@param VA037_GetYFRole Get Yellowfin Role */
        public void SetVA037_GetYFRole(String VA037_GetYFRole) { if (VA037_GetYFRole != null && VA037_GetYFRole.Length > 10) { log.Warning("Length > 10 - truncated"); VA037_GetYFRole = VA037_GetYFRole.Substring(0, 10); } Set_Value("VA037_GetYFRole", VA037_GetYFRole); }/** Get Get Yellowfin Role.
@return Get Yellowfin Role */
        public String GetVA037_GetYFRole() { return (String)Get_Value("VA037_GetYFRole"); }/** Set Yellowfin Role.
@param VA037_YellowfinRoles_ID Yellowfin Role */
        public void SetVA037_YellowfinRoles_ID(int VA037_YellowfinRoles_ID)
        {
            if (VA037_YellowfinRoles_ID <= 0) Set_Value("VA037_YellowfinRoles_ID", null);
            else
                Set_Value("VA037_YellowfinRoles_ID", VA037_YellowfinRoles_ID);
        }/** Get Yellowfin Role.
@return Yellowfin Role */
        public int GetVA037_YellowfinRoles_ID() { Object ii = Get_Value("VA037_YellowfinRoles_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */

        /** Set BI User Name.
@param VA037_BIUserName BI User Name */
        public void SetVA037_BIUserName(String VA037_BIUserName) { if (VA037_BIUserName != null && VA037_BIUserName.Length > 50) { log.Warning("Length > 50 - truncated"); VA037_BIUserName = VA037_BIUserName.Substring(0, 50); } Set_Value("VA037_BIUserName", VA037_BIUserName); }/** Get BI User Name.
@return BI User Name */
        public String GetVA037_BIUserName() { return (String)Get_Value("VA037_BIUserName"); }
        /** Set Map Existing BI User.
@param VA037_IsLinkExistingUser Map Existing BI User */
        public void SetVA037_IsLinkExistingUser(Boolean VA037_IsLinkExistingUser) { Set_Value("VA037_IsLinkExistingUser", VA037_IsLinkExistingUser); }/** Get Map Existing BI User.
@return Map Existing BI User */
        public Boolean IsVA037_IsLinkExistingUser() { Object oo = Get_Value("VA037_IsLinkExistingUser"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Jasper User.
@param VA039_IsJasperUser Jasper User */
        public void SetVA039_IsJasperUser(Boolean VA039_IsJasperUser) { Set_Value("VA039_IsJasperUser", VA039_IsJasperUser); }/** Get Jasper User.
@return Jasper User */
        public Boolean IsVA039_IsJasperUser() { Object oo = Get_Value("VA039_IsJasperUser"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }


        /** Set PatternID.
        @param PatternID PatternID */
        public void SetPatternID(String PatternID)
        {
            if (PatternID != null && PatternID.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                PatternID = PatternID.Substring(0, 500);
            }
            Set_Value("PatternID", PatternID);
        }
        /** Get PatternID.
        @return PatternID */
        public String GetPatternID()
        {
            return (String)Get_Value("PatternID");
        }

        /** Set CardID.
       @param CardID CardID */
        public void SetCardID(String CardID)
        {
            if (CardID != null && CardID.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                CardID = CardID.Substring(0, 500);
            }
            Set_Value("CardID", CardID);
        }
        /** Get CardID.
        @return CardID */
        public String GetCardID()
        {
            return (String)Get_Value("CardID");
        }

        /** Set BI Password.
@param VA037_BIPassword BI Password */
        public void SetVA037_BIPassword(String VA037_BIPassword) { if (VA037_BIPassword != null && VA037_BIPassword.Length > 100) { log.Warning("Length > 100 - truncated"); VA037_BIPassword = VA037_BIPassword.Substring(0, 100); } Set_Value("VA037_BIPassword", VA037_BIPassword); }/** Get BI Password.
@return BI Password */
        public String GetVA037_BIPassword() { return (String)Get_Value("VA037_BIPassword"); }


        /** Set Jasper Password.
@param VA039_JasperPwd Jasper Password */
        public void SetVA039_JasperPwd(String VA039_JasperPwd) { if (VA039_JasperPwd != null && VA039_JasperPwd.Length > 100) { log.Warning("Length > 100 - truncated"); VA039_JasperPwd = VA039_JasperPwd.Substring(0, 100); } Set_Value("VA039_JasperPwd", VA039_JasperPwd); }/** Get Jasper Password.
@return Jasper Password */
        public String GetVA039_JasperPwd() { return (String)Get_Value("VA039_JasperPwd"); }


        /** Set Password Expire On.
@param PasswordExpireOn Password Expire On */
        public void SetPasswordExpireOn(DateTime? PasswordExpireOn) { Set_Value("PasswordExpireOn", (DateTime?)PasswordExpireOn); }/** Get Password Expire On.
@return Password Expire On */
        public DateTime? GetPasswordExpireOn() { return (DateTime?)Get_Value("PasswordExpireOn"); }

        /** Set Failed Login Count.
@param FailedLoginCount Failed Login Count */
        public void SetFailedLoginCount(int FailedLoginCount) { Set_Value("FailedLoginCount", FailedLoginCount); }/** Get Failed Login Count.
@return Failed Login Count */
        public int GetFailedLoginCount() { Object ii = Get_Value("FailedLoginCount"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** TwoFAMethod AD_Reference_ID=1000677 */
        public static int TWOFAMETHOD_AD_Reference_ID=1000677;
        /** Google Authenticator = GA */
        public static String TWOFAMETHOD_GoogleAuthenticator = "GA";
        /** VA Mobile App = VA */
        public static String TWOFAMETHOD_VAMobileApp = "VA";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTwoFAMethodValid(String test) 
        { 
            return test == null || test.Equals("GA") || test.Equals("VA"); 
        }
        /** Set 2FA Method.
        @param TwoFAMethod 2FA Method */
        public void SetTwoFAMethod(String TwoFAMethod)
        {
            if (!IsTwoFAMethodValid(TwoFAMethod))
                throw new ArgumentException("TwoFAMethod Invalid value - " + TwoFAMethod + " - Reference_ID=1000677 - GA - VA"); 
            if (TwoFAMethod != null && TwoFAMethod.Length > 2) 
            { 
                log.Warning("Length > 2 - truncated"); 
                TwoFAMethod = TwoFAMethod.Substring(0, 2); 
            }
            Set_Value("TwoFAMethod", TwoFAMethod);
        }
        /** Get 2FA Method.
        @return 2FA Method */
        public String GetTwoFAMethod() 
        { 
            return (String)Get_Value("TwoFAMethod"); 
        }

        /** Set 2FA Token Key.
        @param TokenKey2FA Unique Two Factor Token Key */
        public void SetTokenKey2FA(String TokenKey2FA) 
        { 
            if (TokenKey2FA != null && TokenKey2FA.Length > 500) 
            { 
                log.Warning("Length > 500 - truncated"); 
                TokenKey2FA = TokenKey2FA.Substring(0, 500); 
            } 
            Set_Value("TokenKey2FA", TokenKey2FA); 
        }
        
        /** Get 2FA Token Key.
        @return Unique Two Factor Token Key */
        public String GetTokenKey2FA() 
        { 
            return (String)Get_Value("TokenKey2FA"); 
        }
    }
}
