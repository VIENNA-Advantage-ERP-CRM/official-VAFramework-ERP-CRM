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
/** Generated Model for AD_ClientInfo
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_ClientInfo : PO
    {
        public X_AD_ClientInfo(Context ctx, int AD_ClientInfo_ID, Trx trxName)
            : base(ctx, AD_ClientInfo_ID, trxName)
        {
            /** if (AD_ClientInfo_ID == 0)
            {
            SetAD_Tree_Product_ID (0);
            SetIsDiscountLineAmt (false);
            }
             */
        }
        public X_AD_ClientInfo(Ctx ctx, int AD_ClientInfo_ID, Trx trxName)
            : base(ctx, AD_ClientInfo_ID, trxName)
        {
            /** if (AD_ClientInfo_ID == 0)
            {
            SetAD_Tree_Product_ID (0);
            SetIsDiscountLineAmt (false);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ClientInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ClientInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_ClientInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_ClientInfo()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514360872L;
        /** Last Updated Timestamp 7/29/2010 1:07:24 PM */
        public static long updatedMS = 1280389044083L;
        /** AD_Table_ID=227 */
        public static int Table_ID;
        // =227;

        /** TableName=AD_ClientInfo */
        public static String Table_Name = "AD_ClientInfo";

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
            StringBuilder sb = new StringBuilder("X_AD_ClientInfo[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_Tree_Activity_ID AD_Reference_ID=184 */
        public static int AD_TREE_ACTIVITY_ID_AD_Reference_ID = 184;
        /** Set Activity Tree.
        @param AD_Tree_Activity_ID Tree to determine activity hierarchy */
        public void SetAD_Tree_Activity_ID(int AD_Tree_Activity_ID)
        {
            if (AD_Tree_Activity_ID <= 0) Set_Value("AD_Tree_Activity_ID", null);
            else
                Set_Value("AD_Tree_Activity_ID", AD_Tree_Activity_ID);
        }
        /** Get Activity Tree.
        @return Tree to determine activity hierarchy */
        public int GetAD_Tree_Activity_ID()
        {
            Object ii = Get_Value("AD_Tree_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_BPartner_ID AD_Reference_ID=184 */
        public static int AD_TREE_BPARTNER_ID_AD_Reference_ID = 184;
        /** Set BPartner Tree.
        @param AD_Tree_BPartner_ID Tree to determine business partner hierarchy */
        public void SetAD_Tree_BPartner_ID(int AD_Tree_BPartner_ID)
        {
            if (AD_Tree_BPartner_ID <= 0) Set_Value("AD_Tree_BPartner_ID", null);
            else
                Set_Value("AD_Tree_BPartner_ID", AD_Tree_BPartner_ID);
        }
        /** Get BPartner Tree.
        @return Tree to determine business partner hierarchy */
        public int GetAD_Tree_BPartner_ID()
        {
            Object ii = Get_Value("AD_Tree_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Campaign_ID AD_Reference_ID=184 */
        public static int AD_TREE_CAMPAIGN_ID_AD_Reference_ID = 184;
        /** Set Campaign Tree.
        @param AD_Tree_Campaign_ID Tree to determine marketing campaign hierarchy */
        public void SetAD_Tree_Campaign_ID(int AD_Tree_Campaign_ID)
        {
            if (AD_Tree_Campaign_ID <= 0) Set_Value("AD_Tree_Campaign_ID", null);
            else
                Set_Value("AD_Tree_Campaign_ID", AD_Tree_Campaign_ID);
        }
        /** Get Campaign Tree.
        @return Tree to determine marketing campaign hierarchy */
        public int GetAD_Tree_Campaign_ID()
        {
            Object ii = Get_Value("AD_Tree_Campaign_ID");
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

        /** AD_Tree_Product_ID AD_Reference_ID=184 */
        public static int AD_TREE_PRODUCT_ID_AD_Reference_ID = 184;
        /** Set Product Tree.
        @param AD_Tree_Product_ID Tree to determine product hierarchy */
        public void SetAD_Tree_Product_ID(int AD_Tree_Product_ID)
        {
           // if (AD_Tree_Product_ID < 1) throw new ArgumentException("AD_Tree_Product_ID is mandatory.");
            if (AD_Tree_Product_ID < 1)
            {
                Set_Value("AD_Tree_Product_ID", null);
            }
            else
            {
                Set_Value("AD_Tree_Product_ID", AD_Tree_Product_ID);
            }
        }
        /** Get Product Tree.
        @return Tree to determine product hierarchy */
        public int GetAD_Tree_Product_ID()
        {
            Object ii = Get_Value("AD_Tree_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Project_ID AD_Reference_ID=184 */
        public static int AD_TREE_PROJECT_ID_AD_Reference_ID = 184;
        /** Set Project Tree.
        @param AD_Tree_Project_ID Tree to determine project hierarchy */
        public void SetAD_Tree_Project_ID(int AD_Tree_Project_ID)
        {
            if (AD_Tree_Project_ID <= 0) Set_Value("AD_Tree_Project_ID", null);
            else
                Set_Value("AD_Tree_Project_ID", AD_Tree_Project_ID);
        }
        /** Get Project Tree.
        @return Tree to determine project hierarchy */
        public int GetAD_Tree_Project_ID()
        {
            Object ii = Get_Value("AD_Tree_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_SalesRegion_ID AD_Reference_ID=184 */
        public static int AD_TREE_SALESREGION_ID_AD_Reference_ID = 184;
        /** Set Sales Region Tree.
        @param AD_Tree_SalesRegion_ID Tree to determine sales regional hierarchy */
        public void SetAD_Tree_SalesRegion_ID(int AD_Tree_SalesRegion_ID)
        {
            if (AD_Tree_SalesRegion_ID <= 0) Set_Value("AD_Tree_SalesRegion_ID", null);
            else
                Set_Value("AD_Tree_SalesRegion_ID", AD_Tree_SalesRegion_ID);
        }
        /** Get Sales Region Tree.
        @return Tree to determine sales regional hierarchy */
        public int GetAD_Tree_SalesRegion_ID()
        {
            Object ii = Get_Value("AD_Tree_SalesRegion_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Verification Class.
        @param BankVerificationClass Bank Data Verification Class */
        public void SetBankVerificationClass(String BankVerificationClass)
        {
            if (BankVerificationClass != null && BankVerificationClass.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                BankVerificationClass = BankVerificationClass.Substring(0, 60);
            }
            Set_Value("BankVerificationClass", BankVerificationClass);
        }
        /** Get Bank Verification Class.
        @return Bank Data Verification Class */
        public String GetBankVerificationClass()
        {
            return (String)Get_Value("BankVerificationClass");
        }

        /** C_AcctSchema1_ID AD_Reference_ID=136 */
        public static int C_ACCTSCHEMA1_ID_AD_Reference_ID = 136;
        /** Set Primary Accounting Schema.
        @param C_AcctSchema1_ID Primary rules for accounting */
        public void SetC_AcctSchema1_ID(int C_AcctSchema1_ID)
        {
            if (C_AcctSchema1_ID <= 0) Set_ValueNoCheck("C_AcctSchema1_ID", null);
            else
                Set_ValueNoCheck("C_AcctSchema1_ID", C_AcctSchema1_ID);
        }
        /** Get Primary Accounting Schema.
        @return Primary rules for accounting */
        public int GetC_AcctSchema1_ID()
        {
            Object ii = Get_Value("C_AcctSchema1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_BPartnerCashTrx_ID AD_Reference_ID=138 */
        public static int C_BPARTNERCASHTRX_ID_AD_Reference_ID = 138;
        /** Set Template B.Partner.
        @param C_BPartnerCashTrx_ID Business Partner used for creating new Business Partners on the fly */
        public void SetC_BPartnerCashTrx_ID(int C_BPartnerCashTrx_ID)
        {
            if (C_BPartnerCashTrx_ID <= 0) Set_Value("C_BPartnerCashTrx_ID", null);
            else
                Set_Value("C_BPartnerCashTrx_ID", C_BPartnerCashTrx_ID);
        }
        /** Get Template B.Partner.
        @return Business Partner used for creating new Business Partners on the fly */
        public int GetC_BPartnerCashTrx_ID()
        {
            Object ii = Get_Value("C_BPartnerCashTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Calendar.
        @param C_Calendar_ID Accounting Calendar Name */
        public void SetC_Calendar_ID(int C_Calendar_ID)
        {
            if (C_Calendar_ID <= 0) Set_Value("C_Calendar_ID", null);
            else
                Set_Value("C_Calendar_ID", C_Calendar_ID);
        }
        /** Get Calendar.
        @return Accounting Calendar Name */
        public int GetC_Calendar_ID()
        {
            Object ii = Get_Value("C_Calendar_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_UOM_Length_ID AD_Reference_ID=114 */
        public static int C_UOM_LENGTH_ID_AD_Reference_ID = 114;
        /** Set UOM for Length.
        @param C_UOM_Length_ID Standard Unit of Measure for Length */
        public void SetC_UOM_Length_ID(int C_UOM_Length_ID)
        {
            if (C_UOM_Length_ID <= 0) Set_Value("C_UOM_Length_ID", null);
            else
                Set_Value("C_UOM_Length_ID", C_UOM_Length_ID);
        }
        /** Get UOM for Length.
        @return Standard Unit of Measure for Length */
        public int GetC_UOM_Length_ID()
        {
            Object ii = Get_Value("C_UOM_Length_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_UOM_Time_ID AD_Reference_ID=114 */
        public static int C_UOM_TIME_ID_AD_Reference_ID = 114;
        /** Set UOM for Time.
        @param C_UOM_Time_ID Standard Unit of Measure for Time */
        public void SetC_UOM_Time_ID(int C_UOM_Time_ID)
        {
            if (C_UOM_Time_ID <= 0) Set_Value("C_UOM_Time_ID", null);
            else
                Set_Value("C_UOM_Time_ID", C_UOM_Time_ID);
        }
        /** Get UOM for Time.
        @return Standard Unit of Measure for Time */
        public int GetC_UOM_Time_ID()
        {
            Object ii = Get_Value("C_UOM_Time_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_UOM_Volume_ID AD_Reference_ID=114 */
        public static int C_UOM_VOLUME_ID_AD_Reference_ID = 114;
        /** Set UOM for Volume.
        @param C_UOM_Volume_ID Standard Unit of Measure for Volume */
        public void SetC_UOM_Volume_ID(int C_UOM_Volume_ID)
        {
            if (C_UOM_Volume_ID <= 0) Set_Value("C_UOM_Volume_ID", null);
            else
                Set_Value("C_UOM_Volume_ID", C_UOM_Volume_ID);
        }
        /** Get UOM for Volume.
        @return Standard Unit of Measure for Volume */
        public int GetC_UOM_Volume_ID()
        {
            Object ii = Get_Value("C_UOM_Volume_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_UOM_Weight_ID AD_Reference_ID=114 */
        public static int C_UOM_WEIGHT_ID_AD_Reference_ID = 114;
        /** Set UOM for Weight.
        @param C_UOM_Weight_ID Standard Unit of Measure for Weight */
        public void SetC_UOM_Weight_ID(int C_UOM_Weight_ID)
        {
            if (C_UOM_Weight_ID <= 0) Set_Value("C_UOM_Weight_ID", null);
            else
                Set_Value("C_UOM_Weight_ID", C_UOM_Weight_ID);
        }
        /** Get UOM for Weight.
        @return Standard Unit of Measure for Weight */
        public int GetC_UOM_Weight_ID()
        {
            Object ii = Get_Value("C_UOM_Weight_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Discount calculated from Line Amounts.
        @param IsDiscountLineAmt Payment Discount calculation does not include Taxes and Charges */
        public void SetIsDiscountLineAmt(Boolean IsDiscountLineAmt)
        {
            Set_Value("IsDiscountLineAmt", IsDiscountLineAmt);
        }
        /** Get Discount calculated from Line Amounts.
        @return Payment Discount calculation does not include Taxes and Charges */
        public Boolean IsDiscountLineAmt()
        {
            Object oo = Get_Value("IsDiscountLineAmt");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Days to keep Log.
        @param KeepLogDays Number of days to keep the log entries */
        public void SetKeepLogDays(int KeepLogDays)
        {
            Set_Value("KeepLogDays", KeepLogDays);
        }
        /** Get Days to keep Log.
        @return Number of days to keep the log entries */
        public int GetKeepLogDays()
        {
            Object ii = Get_Value("KeepLogDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_ProductFreight_ID AD_Reference_ID=162 */
        public static int M_PRODUCTFREIGHT_ID_AD_Reference_ID = 162;
        /** Set Product for Freight.
        @param M_ProductFreight_ID Product for Freight */
        public void SetM_ProductFreight_ID(int M_ProductFreight_ID)
        {
            if (M_ProductFreight_ID <= 0) Set_Value("M_ProductFreight_ID", null);
            else
                Set_Value("M_ProductFreight_ID", M_ProductFreight_ID);
        }
        /** Get Product for Freight.
        @return Product for Freight */
        public int GetM_ProductFreight_ID()
        {
            Object ii = Get_Value("M_ProductFreight_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** MatchRequirementI AD_Reference_ID=360 */
        public static int MATCHREQUIREMENTI_AD_Reference_ID = 360;
        /** Purchase Order and Receipt = B */
        public static String MATCHREQUIREMENTI_PurchaseOrderAndReceipt = "B";
        /** None = N */
        public static String MATCHREQUIREMENTI_None = "N";
        /** Purchase Order = P */
        public static String MATCHREQUIREMENTI_PurchaseOrder = "P";
        /** Receipt = R */
        public static String MATCHREQUIREMENTI_Receipt = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMatchRequirementIValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("N") || test.Equals("P") || test.Equals("R");
        }
        /** Set Invoice Match Requirement.
        @param MatchRequirementI Matching Requirement for Invoice */
        public void SetMatchRequirementI(String MatchRequirementI)
        {
            if (!IsMatchRequirementIValid(MatchRequirementI))
                throw new ArgumentException("MatchRequirementI Invalid value - " + MatchRequirementI + " - Reference_ID=360 - B - N - P - R");
            if (MatchRequirementI != null && MatchRequirementI.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MatchRequirementI = MatchRequirementI.Substring(0, 1);
            }
            Set_Value("MatchRequirementI", MatchRequirementI);
        }
        /** Get Invoice Match Requirement.
        @return Matching Requirement for Invoice */
        public String GetMatchRequirementI()
        {
            return (String)Get_Value("MatchRequirementI");
        }

        /** MatchRequirementR AD_Reference_ID=410 */
        public static int MATCHREQUIREMENTR_AD_Reference_ID = 410;
        /** Purchase Order and Invoice = B */
        public static String MATCHREQUIREMENTR_PurchaseOrderAndInvoice = "B";
        /** Invoice = I */
        public static String MATCHREQUIREMENTR_Invoice = "I";
        /** None = N */
        public static String MATCHREQUIREMENTR_None = "N";
        /** Purchase Order = P */
        public static String MATCHREQUIREMENTR_PurchaseOrder = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMatchRequirementRValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("I") || test.Equals("N") || test.Equals("P");
        }
        /** Set Receipt Match Requirement.
        @param MatchRequirementR Matching Requirement for Receipts */
        public void SetMatchRequirementR(String MatchRequirementR)
        {
            if (!IsMatchRequirementRValid(MatchRequirementR))
                throw new ArgumentException("MatchRequirementR Invalid value - " + MatchRequirementR + " - Reference_ID=410 - B - I - N - P");
            if (MatchRequirementR != null && MatchRequirementR.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MatchRequirementR = MatchRequirementR.Substring(0, 1);
            }
            Set_Value("MatchRequirementR", MatchRequirementR);
        }
        /** Get Receipt Match Requirement.
        @return Matching Requirement for Receipts */
        public String GetMatchRequirementR()
        {
            return (String)Get_Value("MatchRequirementR");
        }
        /** Set Pricing Engine Class.
        @param PricingEngineClass Class used for calculating Prices */
        public void SetPricingEngineClass(String PricingEngineClass)
        {
            if (PricingEngineClass != null && PricingEngineClass.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                PricingEngineClass = PricingEngineClass.Substring(0, 60);
            }
            Set_Value("PricingEngineClass", PricingEngineClass);
        }
        /** Get Pricing Engine Class.
        @return Class used for calculating Prices */
        public String GetPricingEngineClass()
        {
            return (String)Get_Value("PricingEngineClass");
        }
        /** Set Request Type.
        @param R_RequestType_ID Type of request (e.g. Inquiry, Complaint, ..) */
        public void SetR_RequestType_ID(int R_RequestType_ID)
        {
            if (R_RequestType_ID <= 0) Set_Value("R_RequestType_ID", null);
            else
                Set_Value("R_RequestType_ID", R_RequestType_ID);
        }
        /** Get Request Type.
        @return Type of request (e.g. Inquiry, Complaint, ..) */
        public int GetR_RequestType_ID()
        {
            Object ii = Get_Value("R_RequestType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** Get Crystal Report Path */
        public String GetCrystalReportPath()
        {
            return (String)Get_Value("CrystalReportPath");
        }

        /** Get Crystal Report Path */
        public String GetCrystalImagePath()
        {
            return (String)Get_Value("CrystalImagePath");
        }
        /** SaveAttachmentOn AD_Reference_ID=1000154 */
        public static int SAVEATTACHMENTON_AD_Reference_ID = 1000154;
        /** Database = DB */
        public static String SAVEATTACHMENTON_Database = "DB";
        /** FTP Location = FT */
        public static String SAVEATTACHMENTON_FTPLocation = "FT";
        /** Server File System = SR */
        public static String SAVEATTACHMENTON_ServerFileSystem = "SR";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSaveAttachmentOnValid(String test)
        {
            return test.Equals("DB") || test.Equals("FT") || test.Equals("SR");
        }
        /** Set Save Attachment On.
        @param SaveAttachmentOn Save Attachment On */
        public void SetSaveAttachmentOn(String SaveAttachmentOn)
        {
            if (SaveAttachmentOn == null) throw new ArgumentException("SaveAttachmentOn is mandatory");
            if (!IsSaveAttachmentOnValid(SaveAttachmentOn))
                throw new ArgumentException("SaveAttachmentOn Invalid value - " + SaveAttachmentOn + " - Reference_ID=1000154 - DB - FT - SR");
            if (SaveAttachmentOn.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                SaveAttachmentOn = SaveAttachmentOn.Substring(0, 2);
            }
            Set_Value("SaveAttachmentOn", SaveAttachmentOn);
        }
        /** Get Save Attachment On.
        @return Save Attachment On */
        public String GetSaveAttachmentOn()
        {
            return (String)Get_Value("SaveAttachmentOn");
        }
        /** Set FTP Folder.
@param FTPFolder FTP Folder */
        public void SetFTPFolder(String FTPFolder)
        {
            if (FTPFolder != null && FTPFolder.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                FTPFolder = FTPFolder.Substring(0, 60);
            }
            Set_Value("FTPFolder", FTPFolder);
        }
        /** Get FTP Folder.
        @return FTP Folder */
        public String GetFTPFolder()
        {
            return (String)Get_Value("FTPFolder");
        }
        /** Set FTP Password.
        @param FTPPwd FTP Password */
        public void SetFTPPwd(String FTPPwd)
        {
            if (FTPPwd != null && FTPPwd.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                FTPPwd = FTPPwd.Substring(0, 255);
            }
            Set_Value("FTPPwd", FTPPwd);
        }
        /** Get FTP Password.
        @return FTP Password */
        public String GetFTPPwd()
        {
            return (String)Get_Value("FTPPwd");
        }
        /** Set FTP Url.
        @param FTPUrl FTP Url */
        public void SetFTPUrl(String FTPUrl)
        {
            if (FTPUrl != null && FTPUrl.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                FTPUrl = FTPUrl.Substring(0, 255);
            }
            Set_Value("FTPUrl", FTPUrl);
        }
        /** Get FTP Url.
        @return FTP Url */
        public String GetFTPUrl()
        {
            return (String)Get_Value("FTPUrl");
        }
        /** Set FTP Username.
        @param FTPUsername FTP Username */
        public void SetFTPUsername(String FTPUsername)
        {
            if (FTPUsername != null && FTPUsername.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                FTPUsername = FTPUsername.Substring(0, 255);
            }
            Set_Value("FTPUsername", FTPUsername);
        }
        /** Get FTP Username.
        @return FTP Username */
        public String GetFTPUsername()
        {
            return (String)Get_Value("FTPUsername");
        }
    }

}
