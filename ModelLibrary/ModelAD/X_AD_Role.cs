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
    /** Generated Model for AD_Role
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Role : PO
    {
        public X_AD_Role(Context ctx, int AD_Role_ID, Trx trxName)
            : base(ctx, AD_Role_ID, trxName)
        {
            /** if (AD_Role_ID == 0)
            {
            SetAD_Role_ID (0);
            SetConfirmQueryRecords (0);	// 0
            SetIsAccessAllOrgs (false);	// N
            SetIsAdministrator (false);	// N
            SetIsCanApproveOwnDoc (false);
            SetIsCanExport (true);	// Y
            SetIsCanReport (true);	// Y
            SetIsChangeLog (false);	// N
            SetIsManual (false);
            SetIsPersonalAccess (false);	// N
            SetIsPersonalLock (false);	// N
            SetIsShowAcct (false);	// N
            SetIsUseBPRestrictions (false);	// N
            SetIsUseUserOrgAccess (false);	// N
            SetMaxQueryRecords (0);	// 0
            SetName (null);
            SetOverrideReturnPolicy (false);	// N
            SetOverwritePriceLimit (false);	// N
            SetPreferenceType (null);	// O
            SetUserLevel (null);	// O
            }
             */
        }
        public X_AD_Role(Ctx ctx, int AD_Role_ID, Trx trxName)
            : base(ctx, AD_Role_ID, trxName)
        {
            /** if (AD_Role_ID == 0)
            {
            SetAD_Role_ID (0);
            SetConfirmQueryRecords (0);	// 0
            SetIsAccessAllOrgs (false);	// N
            SetIsAdministrator (false);	// N
            SetIsCanApproveOwnDoc (false);
            SetIsCanExport (true);	// Y
            SetIsCanReport (true);	// Y
            SetIsChangeLog (false);	// N
            SetIsManual (false);
            SetIsPersonalAccess (false);	// N
            SetIsPersonalLock (false);	// N
            SetIsShowAcct (false);	// N
            SetIsUseBPRestrictions (false);	// N
            SetIsUseUserOrgAccess (false);	// N
            SetMaxQueryRecords (0);	// 0
            SetName (null);
            SetOverrideReturnPolicy (false);	// N
            SetOverwritePriceLimit (false);	// N
            SetPreferenceType (null);	// O
            SetUserLevel (null);	// O
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Role(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Role()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514363834L;
        /** Last Updated Timestamp 7/29/2010 1:07:27 PM */
        public static long updatedMS = 1280389047045L;
        /** AD_Table_ID=156 */
        public static int Table_ID;
        // =156;

        /** TableName=AD_Role */
        public static String Table_Name = "AD_Role";

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
            StringBuilder sb = new StringBuilder("X_AD_Role[").Append(Get_ID()).Append("]");
            return sb.ToString();
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

        /** AD_Tree_Menu_ID AD_Reference_ID=184 */
        public static int AD_TREE_MENU_ID_AD_Reference_ID = 184;
        /** Set Menu Tree.
        @param AD_Tree_Menu_ID Tree of the menu */
        public void SetAD_Tree_Menu_ID(int AD_Tree_Menu_ID)
        {
            if (AD_Tree_Menu_ID <= 0) Set_Value("AD_Tree_Menu_ID", null);
            else
                Set_Value("AD_Tree_Menu_ID", AD_Tree_Menu_ID);
        }
        /** Get Menu Tree.
        @return Tree of the menu */
        public int GetAD_Tree_Menu_ID()
        {
            Object ii = Get_Value("AD_Tree_Menu_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Org_ID AD_Reference_ID=184 */
        public static int AD_TREE_ORG_ID_AD_Reference_ID = 184;
        /** Set Organization Tree.
        @param AD_Tree_Org_ID Tree to determine organizational hierarchy */
        public void SetAD_Tree_Org_ID(int AD_Tree_Org_ID)
        {
            if (AD_Tree_Org_ID <= 0) Set_Value("AD_Tree_Org_ID", null);
            else
                Set_Value("AD_Tree_Org_ID", AD_Tree_Org_ID);
        }
        /** Get Organization Tree.
        @return Tree to determine organizational hierarchy */
        public int GetAD_Tree_Org_ID()
        {
            Object ii = Get_Value("AD_Tree_Org_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Approval Amount.
        @param AmtApproval The approval amount limit for this role */
        public void SetAmtApproval(Decimal? AmtApproval)
        {
            Set_Value("AmtApproval", (Decimal?)AmtApproval);
        }
        /** Get Approval Amount.
        @return The approval amount limit for this role */
        public Decimal GetAmtApproval()
        {
            Object bd = Get_Value("AmtApproval");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Confirm Query Records.
        @param ConfirmQueryRecords Require Confirmation if more records will be returned by the query (If not defined 500) */
        public void SetConfirmQueryRecords(int ConfirmQueryRecords)
        {
            Set_Value("ConfirmQueryRecords", ConfirmQueryRecords);
        }
        /** Get Confirm Query Records.
        @return Require Confirmation if more records will be returned by the query (If not defined 500) */
        public int GetConfirmQueryRecords()
        {
            Object ii = Get_Value("ConfirmQueryRecords");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** DisplayClientOrg AD_Reference_ID=427 */
        public static int DISPLAYCLIENTORG_AD_Reference_ID = 427;
        /** Always Tenant, Organziation = A */
        public static String DISPLAYCLIENTORG_AlwaysTenantOrganziation = "A";
        /** No Tenant nor Org = N */
        public static String DISPLAYCLIENTORG_NoTenantNorOrg = "N";
        /** Only Organization = O */
        public static String DISPLAYCLIENTORG_OnlyOrganization = "O";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDisplayClientOrgValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("N") || test.Equals("O");
        }
        /** Set Display.
        @param DisplayClientOrg Display Policy for Tenant and Organization */
        public void SetDisplayClientOrg(String DisplayClientOrg)
        {
            if (!IsDisplayClientOrgValid(DisplayClientOrg))
                throw new ArgumentException("DisplayClientOrg Invalid value - " + DisplayClientOrg + " - Reference_ID=427 - A - N - O");
            if (DisplayClientOrg != null && DisplayClientOrg.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DisplayClientOrg = DisplayClientOrg.Substring(0, 1);
            }
            Set_Value("DisplayClientOrg", DisplayClientOrg);
        }
        /** Get Display.
        @return Display Policy for Tenant and Organization */
        public String GetDisplayClientOrg()
        {
            return (String)Get_Value("DisplayClientOrg");
        }
        /** Set Access all Orgs.
        @param IsAccessAllOrgs Access all Organizations (no org access control) of the client */
        public void SetIsAccessAllOrgs(Boolean IsAccessAllOrgs)
        {
            Set_Value("IsAccessAllOrgs", IsAccessAllOrgs);
        }
        /** Get Access all Orgs.
        @return Access all Organizations (no org access control) of the client */
        public Boolean IsAccessAllOrgs()
        {
            Object oo = Get_Value("IsAccessAllOrgs");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Administrator.
        @param IsAdministrator This is an administrator role */
        public void SetIsAdministrator(Boolean IsAdministrator)
        {
            Set_Value("IsAdministrator", IsAdministrator);
        }
        /** Get Administrator.
        @return This is an administrator role */
        public Boolean IsAdministrator()
        {
            Object oo = Get_Value("IsAdministrator");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Approve own Documents.
        @param IsCanApproveOwnDoc Users with this role can approve their own documents */
        public void SetIsCanApproveOwnDoc(Boolean IsCanApproveOwnDoc)
        {
            Set_Value("IsCanApproveOwnDoc", IsCanApproveOwnDoc);
        }
        /** Get Approve own Documents.
        @return Users with this role can approve their own documents */
        public Boolean IsCanApproveOwnDoc()
        {
            Object oo = Get_Value("IsCanApproveOwnDoc");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Can Export.
        @param IsCanExport Users with this role can export data */
        public void SetIsCanExport(Boolean IsCanExport)
        {
            Set_Value("IsCanExport", IsCanExport);
        }
        /** Get Can Export.
        @return Users with this role can export data */
        public Boolean IsCanExport()
        {
            Object oo = Get_Value("IsCanExport");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Can Report.
        @param IsCanReport Users with this role can create reports */
        public void SetIsCanReport(Boolean IsCanReport)
        {
            Set_Value("IsCanReport", IsCanReport);
        }
        /** Get Can Report.
        @return Users with this role can create reports */
        public Boolean IsCanReport()
        {
            Object oo = Get_Value("IsCanReport");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Maintain Change Log.
        @param IsChangeLog Maintain a log of changes */
        public void SetIsChangeLog(Boolean IsChangeLog)
        {
            Set_Value("IsChangeLog", IsChangeLog);
        }
        /** Get Maintain Change Log.
        @return Maintain a log of changes */
        public Boolean IsChangeLog()
        {
            Object oo = Get_Value("IsChangeLog");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Manual.
        @param IsManual This is a manual process */
        public void SetIsManual(Boolean IsManual)
        {
            Set_Value("IsManual", IsManual);
        }
        /** Get Manual.
        @return This is a manual process */
        public Boolean IsManual()
        {
            Object oo = Get_Value("IsManual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Personal Access.
        @param IsPersonalAccess Allow access to all personal records */
        public void SetIsPersonalAccess(Boolean IsPersonalAccess)
        {
            Set_Value("IsPersonalAccess", IsPersonalAccess);
        }
        /** Get Personal Access.
        @return Allow access to all personal records */
        public Boolean IsPersonalAccess()
        {
            Object oo = Get_Value("IsPersonalAccess");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Personal Lock.
        @param IsPersonalLock Allow users with role to lock access to personal records */
        public void SetIsPersonalLock(Boolean IsPersonalLock)
        {
            Set_Value("IsPersonalLock", IsPersonalLock);
        }
        /** Get Personal Lock.
        @return Allow users with role to lock access to personal records */
        public Boolean IsPersonalLock()
        {
            Object oo = Get_Value("IsPersonalLock");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Show Accounting.
        @param IsShowAcct Users with this role can see accounting information */
        public void SetIsShowAcct(Boolean IsShowAcct)
        {
            Set_Value("IsShowAcct", IsShowAcct);
        }
        /** Get Show Accounting.
        @return Users with this role can see accounting information */
        public Boolean IsShowAcct()
        {
            Object oo = Get_Value("IsShowAcct");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use BP Restrictions.
        @param IsUseBPRestrictions Use BP Restrictions */
        public void SetIsUseBPRestrictions(Boolean IsUseBPRestrictions)
        {
            Set_Value("IsUseBPRestrictions", IsUseBPRestrictions);
        }
        /** Get Use BP Restrictions.
        @return Use BP Restrictions */
        public Boolean IsUseBPRestrictions()
        {
            Object oo = Get_Value("IsUseBPRestrictions");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use User Org Access.
        @param IsUseUserOrgAccess Use Org Access defined by user instead of Role Org Access */
        public void SetIsUseUserOrgAccess(Boolean IsUseUserOrgAccess)
        {
            Set_Value("IsUseUserOrgAccess", IsUseUserOrgAccess);
        }
        /** Get Use User Org Access.
        @return Use Org Access defined by user instead of Role Org Access */
        public Boolean IsUseUserOrgAccess()
        {
            Object oo = Get_Value("IsUseUserOrgAccess");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Max Query Records.
        @param MaxQueryRecords If defined, you cannot query more records as defined - the query criteria needs to be changed to query less records */
        public void SetMaxQueryRecords(int MaxQueryRecords)
        {
            Set_Value("MaxQueryRecords", MaxQueryRecords);
        }
        /** Get Max Query Records.
        @return If defined, you cannot query more records as defined - the query criteria needs to be changed to query less records */
        public int GetMaxQueryRecords()
        {
            Object ii = Get_Value("MaxQueryRecords");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Override Return Policy.
        @param OverrideReturnPolicy Override Return Policy if the policy timeframe is exceeded */
        public void SetOverrideReturnPolicy(Boolean OverrideReturnPolicy)
        {
            Set_Value("OverrideReturnPolicy", OverrideReturnPolicy);
        }
        /** Get Override Return Policy.
        @return Override Return Policy if the policy timeframe is exceeded */
        public Boolean IsOverrideReturnPolicy()
        {
            Object oo = Get_Value("OverrideReturnPolicy");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Overwrite Price Limit.
        @param OverwritePriceLimit Overwrite Price Limit if the Price List  enforces the Price Limit */
        public void SetOverwritePriceLimit(Boolean OverwritePriceLimit)
        {
            Set_Value("OverwritePriceLimit", OverwritePriceLimit);
        }
        /** Get Overwrite Price Limit.
        @return Overwrite Price Limit if the Price List  enforces the Price Limit */
        public Boolean IsOverwritePriceLimit()
        {
            Object oo = Get_Value("OverwritePriceLimit");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** PreferenceType AD_Reference_ID=330 */
        public static int PREFERENCETYPE_AD_Reference_ID = 330;
        /** Client = C */
        public static String PREFERENCETYPE_Client = "C";
        /** None = N */
        public static String PREFERENCETYPE_None = "N";
        /** Organization = O */
        public static String PREFERENCETYPE_Organization = "O";
        /** User = U */
        public static String PREFERENCETYPE_User = "U";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPreferenceTypeValid(String test)
        {
            return test.Equals("C") || test.Equals("N") || test.Equals("O") || test.Equals("U");
        }
        /** Set Preference Level.
        @param PreferenceType Determines what preferences the user can set */
        public void SetPreferenceType(String PreferenceType)
        {
            if (PreferenceType == null) throw new ArgumentException("PreferenceType is mandatory");
            if (!IsPreferenceTypeValid(PreferenceType))
                throw new ArgumentException("PreferenceType Invalid value - " + PreferenceType + " - Reference_ID=330 - C - N - O - U");
            if (PreferenceType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PreferenceType = PreferenceType.Substring(0, 1);
            }
            Set_Value("PreferenceType", PreferenceType);
        }
        /** Get Preference Level.
        @return Determines what preferences the user can set */
        public String GetPreferenceType()
        {
            return (String)Get_Value("PreferenceType");
        }

        /** Supervisor_ID AD_Reference_ID=286 */
        public static int SUPERVISOR_ID_AD_Reference_ID = 286;
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

        /** UserLevel AD_Reference_ID=226 */
        public static int USERLEVEL_AD_Reference_ID = 226;
        /** Organization =   O */
        public static String USERLEVEL_Organization = "  O";
        /** Client =  C  */
        public static String USERLEVEL_Client = " C ";
        /** Client+Organization =  CO */
        public static String USERLEVEL_ClientPlusOrganization = " CO";
        /** System = S   */
        public static String USERLEVEL_System = "S  ";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsUserLevelValid(String test)
        {
            return test.Equals("  O") || test.Equals(" C ") || test.Equals(" CO") || test.Equals("S  ");
        }
        /** Set User Level.
        @param UserLevel System Client Organization */
        public void SetUserLevel(String UserLevel)
        {
            if (UserLevel == null) throw new ArgumentException("UserLevel is mandatory");
            if (!IsUserLevelValid(UserLevel))
                throw new ArgumentException("UserLevel Invalid value - " + UserLevel + " - Reference_ID=226 -   O -  C  -  CO - S  ");
            if (UserLevel.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                UserLevel = UserLevel.Substring(0, 3);
            }
            Set_Value("UserLevel", UserLevel);
        }
        /** Get User Level.
        @return System Client Organization */
        public String GetUserLevel()
        {
            return (String)Get_Value("UserLevel");
        }

        /** WinUserDefLevel AD_Reference_ID=428 */
        public static int WINUSERDEFLEVEL_AD_Reference_ID = 428;
        /** Tenant (or Role or User) = C */
        public static String WINUSERDEFLEVEL_TenantOrRoleOrUser = "C";
        /** None = N */
        public static String WINUSERDEFLEVEL_None = "N";
        /** Role (or User) = R */
        public static String WINUSERDEFLEVEL_RoleOrUser = "R";
        /** User only = U */
        public static String WINUSERDEFLEVEL_UserOnly = "U";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWinUserDefLevelValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("N") || test.Equals("R") || test.Equals("U");
        }
        /** Set Customization Level.
        @param WinUserDefLevel Level for Window Layout Customization */
        public void SetWinUserDefLevel(String WinUserDefLevel)
        {
            if (!IsWinUserDefLevelValid(WinUserDefLevel))
                throw new ArgumentException("WinUserDefLevel Invalid value - " + WinUserDefLevel + " - Reference_ID=428 - C - N - R - U");
            if (WinUserDefLevel != null && WinUserDefLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                WinUserDefLevel = WinUserDefLevel.Substring(0, 1);
            }
            Set_Value("WinUserDefLevel", WinUserDefLevel);
        }
        /** Get Customization Level.
        @return Level for Window Layout Customization */
        public String GetWinUserDefLevel()
        {
            return (String)Get_Value("WinUserDefLevel");
        }
        /** HomePage_ID AD_Reference_ID=1000387 */
        public static int HOMEPAGE_ID_AD_Reference_ID = 1000387;/** Set Home Page.
@param HomePage_ID Home Page */
        public void SetHomePage_ID(int HomePage_ID)
        {
            if (HomePage_ID <= 0) Set_Value("HomePage_ID", null);
            else
                Set_Value("HomePage_ID", HomePage_ID);
        }/** Get Home Page.
@return Home Page */
        public int GetHomePage_ID() { Object ii = Get_Value("HomePage_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Set Disable Menu.
        @param DisableMenu Disable Menu */
        public void SetDisableMenu(Boolean DisableMenu) { Set_Value("DisableMenu", DisableMenu); }/** Get Disable Menu.
@return Disable Menu */
        public Boolean IsDisableMenu() { Object oo = Get_Value("DisableMenu"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Check Document Action Access.
@param CheckDocActionAccess Check document action access based on document type on every transaction. */
        public void SetCheckDocActionAccess(Boolean CheckDocActionAccess) { Set_Value("CheckDocActionAccess", CheckDocActionAccess); }/** Get Check Document Action Access.
@return Check document action access based on document type on every transaction. */
        public Boolean IsCheckDocActionAccess() { Object oo = Get_Value("CheckDocActionAccess"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Can Create New Attribute.
@param IsCanCreateAttribute Users with this role can create new attribute set instance */
        public void SetIsCanCreateAttribute(Boolean IsCanCreateAttribute) { Set_Value("IsCanCreateAttribute", IsCanCreateAttribute); }
        /** Get Can Create New Attribute.
@return Users with this role can create new attribute set instance */
        public Boolean IsCanCreateAttribute() { Object oo = Get_Value("IsCanCreateAttribute"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Can Edit Attribute.
@param IsCanEditAttribute Users with this role can edit attribute set instance */
        public void SetIsCanEditAttribute(Boolean IsCanEditAttribute) { Set_Value("IsCanEditAttribute", IsCanEditAttribute); }
        /** Get Can Edit Attribute.
@return Users with this role can edit attribute set instance */
        public Boolean IsCanEditAttribute() { Object oo = Get_Value("IsCanEditAttribute"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
    }

}






