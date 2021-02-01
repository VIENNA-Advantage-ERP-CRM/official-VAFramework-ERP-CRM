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
    /** Generated Model for VAF_OrgDetail
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_OrgDetail : PO
    {
        public X_VAF_OrgDetail(Context ctx, int VAF_OrgDetail_ID, Trx trxName)
            : base(ctx, VAF_OrgDetail_ID, trxName)
        {
            /** if (VAF_OrgDetail_ID == 0)
            {
            SetDUNS (null);	// ?
            SetTaxID (null);
            }
             */
        }
        public X_VAF_OrgDetail(Ctx ctx, int VAF_OrgDetail_ID, Trx trxName)
            : base(ctx, VAF_OrgDetail_ID, trxName)
        {
            /** if (VAF_OrgDetail_ID == 0)
            {
            SetDUNS (null);	// ?
            SetTaxID (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_OrgDetail(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_OrgDetail(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_OrgDetail(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_OrgDetail()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27716672496371L;
        /** Last Updated Timestamp 6/17/2015 6:49:41 PM */
        public static long updatedMS = 1434547179582L;
        /** VAF_TableView_ID=228 */
        public static int Table_ID;
        // =228;

        /** TableName=VAF_OrgDetail */
        public static String Table_Name = "VAF_OrgDetail";

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
            StringBuilder sb = new StringBuilder("X_VAF_OrgDetail[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Organization Type.
        @param VAF_OrgCategory_ID Organization Type allows you to categorize your organizations */
        public void SetVAF_OrgCategory_ID(int VAF_OrgCategory_ID)
        {
            if (VAF_OrgCategory_ID <= 0) Set_Value("VAF_OrgCategory_ID", null);
            else
                Set_Value("VAF_OrgCategory_ID", VAF_OrgCategory_ID);
        }
        /** Get Organization Type.
        @return Organization Type allows you to categorize your organizations */
        public int GetVAF_OrgCategory_ID()
        {
            Object ii = Get_Value("VAF_OrgCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param VAB_Address_ID Location or Address */
        public void SetVAB_Address_ID(int VAB_Address_ID)
        {
            if (VAB_Address_ID <= 0) Set_Value("VAB_Address_ID", null);
            else
                Set_Value("VAB_Address_ID", VAB_Address_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetVAB_Address_ID()
        {
            Object ii = Get_Value("VAB_Address_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set D-U-N-S.
        @param DUNS Creditor Check (Dun & Bradstreet) Number */
        public void SetDUNS(String DUNS)
        {
            if (DUNS == null) throw new ArgumentException("DUNS is mandatory.");
            if (DUNS.Length > 12)
            {
                log.Warning("Length > 12 - truncated");
                DUNS = DUNS.Substring(0, 12);
            }
            Set_Value("DUNS", DUNS);
        }
        /** Get D-U-N-S.
        @return Creditor Check (Dun & Bradstreet) Number */
        public String GetDUNS()
        {
            return (String)Get_Value("DUNS");
        }
        /** Set SIA Code.
        @param ED008_SIACode SIA Code */
        public void SetED008_SIACode(int ED008_SIACode)
        {
            Set_Value("ED008_SIACode", ED008_SIACode);
        }
        /** Get SIA Code.
        @return SIA Code */
        public int GetED008_SIACode()
        {
            Object ii = Get_Value("ED008_SIACode");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set EMail Address.
        @param EMail Electronic Mail Address */
        public void SetEMail(String EMail)
        {
            if (EMail != null && EMail.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                EMail = EMail.Substring(0, 50);
            }
            Set_Value("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
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
        /** Set Fax.
        @param Fax Facsimile number */
        public void SetFax(String Fax)
        {
            if (Fax != null && Fax.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Fax = Fax.Substring(0, 50);
            }
            Set_Value("Fax", Fax);
        }
        /** Get Fax.
        @return Facsimile number */
        public String GetFax()
        {
            return (String)Get_Value("Fax");
        }
        /** Set Logo.
        @param Logo Logo */
        public void SetLogo(Byte[] Logo)
        {
            Set_Value("Logo", Logo);
        }
        /** Get Logo.
        @return Logo */
        public Byte[] GetLogo()
        {
            return (Byte[])Get_Value("Logo");
        }
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Parent_Org_ID VAF_Control_Ref_ID=130 */
        public static int PARENT_ORG_ID_VAF_Control_Ref_ID = 130;
        /** Set Parent Organization.
        @param Parent_Org_ID Parent (superior) Organization  */
        public void SetParent_Org_ID(int Parent_Org_ID)
        {
            if (Parent_Org_ID <= 0) Set_Value("Parent_Org_ID", null);
            else
                Set_Value("Parent_Org_ID", Parent_Org_ID);
        }
        /** Get Parent Organization.
        @return Parent (superior) Organization  */
        public int GetParent_Org_ID()
        {
            Object ii = Get_Value("Parent_Org_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phone.
        @param Phone Identifies a telephone number */
        public void SetPhone(String Phone)
        {
            if (Phone != null && Phone.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Phone = Phone.Substring(0, 50);
            }
            Set_Value("Phone", Phone);
        }
        /** Get Phone.
        @return Identifies a telephone number */
        public String GetPhone()
        {
            return (String)Get_Value("Phone");
        }

        /** Supervisor_ID VAF_Control_Ref_ID=286 */
        public static int SUPERVISOR_ID_VAF_Control_Ref_ID = 286;
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
        /** Set Tax ID.
        @param TaxID Tax Identification */
        public void SetTaxID(String TaxID)
        {
            if (TaxID == null) throw new ArgumentException("TaxID is mandatory.");
            if (TaxID.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                TaxID = TaxID.Substring(0, 20);
            }
            Set_Value("TaxID", TaxID);
        }
        /** Get Tax ID.
        @return Tax Identification */
        public String GetTaxID()
        {
            return (String)Get_Value("TaxID");
        }

        /** VATAX_TaxRule VAF_Control_Ref_ID=1000168 */
        public static int VATAX_TAXRULE_VAF_Control_Ref_ID = 1000168;
        /** Tax Type = T */
        public static String VATAX_TAXRULE_TaxType = "T";
        /** Zip = Z */
        public static String VATAX_TAXRULE_Zip = "Z";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVATAX_TaxRuleValid(String test)
        {
            return test == null || test.Equals("T") || test.Equals("Z");
        }
        /** Set Tax Rule.
        @param VATAX_TaxRule Tax Rule */
        public void SetVATAX_TaxRule(String VATAX_TaxRule)
        {
            if (!IsVATAX_TaxRuleValid(VATAX_TaxRule))
                throw new ArgumentException("VATAX_TaxRule Invalid value - " + VATAX_TaxRule + " - Reference_ID=1000168 - T - Z");
            if (VATAX_TaxRule != null && VATAX_TaxRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_TaxRule = VATAX_TaxRule.Substring(0, 1);
            }
            Set_Value("VATAX_TaxRule", VATAX_TaxRule);
        }
        /** Get Tax Rule.
        @return Tax Rule */
        public String GetVATAX_TaxRule()
        {
            return (String)Get_Value("VATAX_TaxRule");
        }

        /** Set Calendar.
@param VAB_Calender_ID Accounting Calendar Name */
        public void SetVAB_Calender_ID(int VAB_Calender_ID)
        {
            if (VAB_Calender_ID <= 0) Set_Value("VAB_Calender_ID", null);
            else
                Set_Value("VAB_Calender_ID", VAB_Calender_ID);
        }
        /** Get Calendar.
        @return Accounting Calendar Name */
        public int GetVAB_Calender_ID()
        {
            Object ii = Get_Value("VAB_Calender_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Accounting Book.
     @param VAB_AccountBook_ID Rules for accounting */
        public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID)
        {
            if (VAB_AccountBook_ID <= 0)
                Set_Value("VAB_AccountBook_ID", null);
            else
                Set_Value("VAB_AccountBook_ID", VAB_AccountBook_ID);
        }

        /** Get Accounting Book.
        @return Rules for accounting */
        public int GetVAB_AccountBook_ID()
        {
            Object ii = Get_Value("VAB_AccountBook_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }

    }

}
