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
    /** Generated Model for C_BPartner_Location
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_BPartner_Location : PO
    {
        public X_C_BPartner_Location(Context ctx, int C_BPartner_Location_ID, Trx trxName) : base(ctx, C_BPartner_Location_ID, trxName)
        {
            /** if (C_BPartner_Location_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Location_ID (0);
SetIsBillTo (true);	// Y
SetIsPayFrom (true);	// Y
SetIsRemitTo (true);	// Y
SetIsShipTo (true);	// Y
SetName (null);	// .
}
             */
        }
        public X_C_BPartner_Location(Ctx ctx, int C_BPartner_Location_ID, Trx trxName) : base(ctx, C_BPartner_Location_ID, trxName)
        {
            /** if (C_BPartner_Location_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Location_ID (0);
SetIsBillTo (true);	// Y
SetIsPayFrom (true);	// Y
SetIsRemitTo (true);	// Y
SetIsShipTo (true);	// Y
SetName (null);	// .
}
             */
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_BPartner_Location(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_BPartner_Location(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_BPartner_Location(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_BPartner_Location()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514370542L;
        /** Last Updated Timestamp 7/29/2010 1:07:33 PM */
        public static long updatedMS = 1280389053753L;
        /** AD_Table_ID=293 */
        public static int Table_ID;
        // =293;

        /** TableName=C_BPartner_Location */
        public static String Table_Name = "C_BPartner_Location";

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
            StringBuilder sb = new StringBuilder("X_C_BPartner_Location[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
            Set_ValueNoCheck("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
@return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID < 1) throw new ArgumentException("C_BPartner_Location_ID is mandatory.");
            Set_ValueNoCheck("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
@param C_Location_ID Location or Address */
        public void SetC_Location_ID(int C_Location_ID)
        {
            if (C_Location_ID < 1) throw new ArgumentException("C_Location_ID is mandatory.");
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
        /** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
        public void SetC_SalesRegion_ID(int C_SalesRegion_ID)
        {
            if (C_SalesRegion_ID <= 0) Set_Value("C_SalesRegion_ID", null);
            else
                Set_Value("C_SalesRegion_ID", C_SalesRegion_ID);
        }
        /** Get Sales Region.
@return Sales coverage region */
        public int GetC_SalesRegion_ID()
        {
            Object ii = Get_Value("C_SalesRegion_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** CreditStatusSettingOn AD_Reference_ID=1000178 */
        public static int CREDITSTATUSSETTINGON_AD_Reference_ID = 1000178;
        /** Customer Header = CH */
        public static String CREDITSTATUSSETTINGON_CustomerHeader = "CH";
        /** Customer Location = CL */
        public static String CREDITSTATUSSETTINGON_CustomerLocation = "CL";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCreditStatusSettingOnValid(String test)
        {
            return test == null || test.Equals("CH") || test.Equals("CL");
        }
        /** Set Credit Status Setting On.
@param CreditStatusSettingOn Credit Status Setting on represents that on which level needs to define the credit details of the customer. */
        public void SetCreditStatusSettingOn(String CreditStatusSettingOn)
        {
            if (!IsCreditStatusSettingOnValid(CreditStatusSettingOn))
                throw new ArgumentException("CreditStatusSettingOn Invalid value - " + CreditStatusSettingOn + " - Reference_ID=1000178 - CH - CL");
            if (CreditStatusSettingOn != null && CreditStatusSettingOn.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                CreditStatusSettingOn = CreditStatusSettingOn.Substring(0, 2);
            }
            Set_Value("CreditStatusSettingOn", CreditStatusSettingOn);
        }
        /** Get Credit Status Setting On.
@return Credit Status Setting on represents that on which level needs to define the credit details of the customer. */
        public String GetCreditStatusSettingOn()
        {
            return (String)Get_Value("CreditStatusSettingOn");
        }

        /** CreditValidation AD_Reference_ID=1000179 */
        public static int CREDITVALIDATION_AD_Reference_ID = 1000179;
        /** Stop SO = A */
        public static String CREDITVALIDATION_StopSO = "A";
        /** Stop Shipment = B */
        public static String CREDITVALIDATION_StopShipment = "B";
        /** Stop Invoice = C */
        public static String CREDITVALIDATION_StopInvoice = "C";
        /** Stop All = D */
        public static String CREDITVALIDATION_StopAll = "D";
        /** Stop SO & Shipment = E */
        public static String CREDITVALIDATION_StopSOShipment = "E";
        /** Stop Shipment & Invoice = F */
        public static String CREDITVALIDATION_StopShipmentInvoice = "F";
        /** Warning on SO = G */
        public static String CREDITVALIDATION_WarningOnSO = "G";
        /** Warning on Shipment = H */
        public static String CREDITVALIDATION_WarningOnShipment = "H";
        /** Warning on Invoice = I */
        public static String CREDITVALIDATION_WarningOnInvoice = "I";
        /** Warning on All = J */
        public static String CREDITVALIDATION_WarningOnAll = "J";
        /** Warning on SO & Shipment = K */
        public static String CREDITVALIDATION_WarningOnSOShipment = "K";
        /** Warning on Shipment & Invoice = L */
        public static String CREDITVALIDATION_WarningOnShipmentInvoice = "L";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCreditValidationValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("E") || test.Equals("F") || test.Equals("G") || test.Equals("H") || test.Equals("I") || test.Equals("J") || test.Equals("K") || test.Equals("L");
        }
        /** Set Credit Validation.
@param CreditValidation Credit Validation field indicates to raise either the warning or to stop generating the transactions (Sales Order & Invoices) */
        public void SetCreditValidation(String CreditValidation)
        {
            if (!IsCreditValidationValid(CreditValidation))
                throw new ArgumentException("CreditValidation Invalid value - " + CreditValidation + " - Reference_ID=1000179 - A - B - C - D - E - F - G - H - I - J - K - L");
            if (CreditValidation != null && CreditValidation.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreditValidation = CreditValidation.Substring(0, 1);
            }
            Set_Value("CreditValidation", CreditValidation);
        }
        /** Get Credit Validation.
@return Credit Validation field indicates to raise either the warning or to stop generating the transactions (Sales Order & Invoices) */
        public String GetCreditValidation()
        {
            return (String)Get_Value("CreditValidation");
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
        /** Set ISDN.
@param ISDN ISDN or modem line */
        public void SetISDN(String ISDN)
        {
            if (ISDN != null && ISDN.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                ISDN = ISDN.Substring(0, 40);
            }
            Set_Value("ISDN", ISDN);
        }
        /** Get ISDN.
@return ISDN or modem line */
        public String GetISDN()
        {
            return (String)Get_Value("ISDN");
        }
        /** Set Invoice Address.
@param IsBillTo Business Partner Invoice/Bill Address */
        public void SetIsBillTo(Boolean IsBillTo)
        {
            Set_Value("IsBillTo", IsBillTo);
        }
        /** Get Invoice Address.
@return Business Partner Invoice/Bill Address */
        public Boolean IsBillTo()
        {
            Object oo = Get_Value("IsBillTo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Pay-From Address.
@param IsPayFrom Business Partner pays from that address and we'll send dunning letters there */
        public void SetIsPayFrom(Boolean IsPayFrom)
        {
            Set_Value("IsPayFrom", IsPayFrom);
        }
        /** Get Pay-From Address.
@return Business Partner pays from that address and we'll send dunning letters there */
        public Boolean IsPayFrom()
        {
            Object oo = Get_Value("IsPayFrom");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Remit-To Address.
@param IsRemitTo Business Partner payment address */
        public void SetIsRemitTo(Boolean IsRemitTo)
        {
            Set_Value("IsRemitTo", IsRemitTo);
        }
        /** Get Remit-To Address.
@return Business Partner payment address */
        public Boolean IsRemitTo()
        {
            Object oo = Get_Value("IsRemitTo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Ship Address.
@param IsShipTo Business Partner Shipment Address */
        public void SetIsShipTo(Boolean IsShipTo)
        {
            Set_Value("IsShipTo", IsShipTo);
        }
        /** Get Ship Address.
@return Business Partner Shipment Address */
        public Boolean IsShipTo()
        {
            Object oo = Get_Value("IsShipTo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                Name = Name.Substring(0, 500);
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
        /** SOCreditStatus AD_Reference_ID=289 */
        public static int SOCREDITSTATUS_AD_Reference_ID = 289;
        /** Credit Hold = H */
        public static String SOCREDITSTATUS_CreditHold = "H";
        /** Credit OK = O */
        public static String SOCREDITSTATUS_CreditOK = "O";
        /** Credit Stop = S */
        public static String SOCREDITSTATUS_CreditStop = "S";
        /** Credit Watch = W */
        public static String SOCREDITSTATUS_CreditWatch = "W";
        /** No Credit Check = X */
        public static String SOCREDITSTATUS_NoCreditCheck = "X";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsSOCreditStatusValid(String test)
        {
            return test == null || test.Equals("H") || test.Equals("O") || test.Equals("S") || test.Equals("W") || test.Equals("X");
        }
        /** Set Credit Status.
@param SOCreditStatus Partner's Credit Status */
        public void SetSOCreditStatus(String SOCreditStatus)
        {
            if (!IsSOCreditStatusValid(SOCreditStatus))
                throw new ArgumentException("SOCreditStatus Invalid value - " + SOCreditStatus + " - Reference_ID=289 - H - O - S - W - X");
            if (SOCreditStatus != null && SOCreditStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SOCreditStatus = SOCreditStatus.Substring(0, 1);
            }
            Set_Value("SOCreditStatus", SOCreditStatus);
        }
        /** Get Credit Status.
@return Partner's Credit Status */
        public String GetSOCreditStatus()
        {
            return (String)Get_Value("SOCreditStatus");
        }
        /** Set Credit Limit.
@param SO_CreditLimit Total outstanding invoice amounts allowed */
        public void SetSO_CreditLimit(Decimal? SO_CreditLimit)
        {
            Set_Value("SO_CreditLimit", (Decimal?)SO_CreditLimit);
        }
        /** Get Credit Limit.
@return Total outstanding invoice amounts allowed */
        public Decimal GetSO_CreditLimit()
        {
            Object bd = Get_Value("SO_CreditLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Credit Used.
@param SO_CreditUsed Current open balance */
        public void SetSO_CreditUsed(Decimal? SO_CreditUsed)
        {
            Set_Value("SO_CreditUsed", (Decimal?)SO_CreditUsed);
        }
        /** Get Credit Used.
@return Current open balance */
        public Decimal GetSO_CreditUsed()
        {
            Object bd = Get_Value("SO_CreditUsed");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Open Balance.
@param TotalOpenBalance Total Open Balance Amount in primary Accounting Currency */
        public void SetTotalOpenBalance(Decimal? TotalOpenBalance)
        {
            Set_Value("TotalOpenBalance", (Decimal?)TotalOpenBalance);
        }
        /** Get Open Balance.
@return Total Open Balance Amount in primary Accounting Currency */
        public Decimal GetTotalOpenBalance()
        {
            Object bd = Get_Value("TotalOpenBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Tax Type.
@param VATAX_TaxType_ID Tax Type */
        public void SetVATAX_TaxType_ID(int VATAX_TaxType_ID)
        {
            if (VATAX_TaxType_ID <= 0) Set_Value("VATAX_TaxType_ID", null);
            else
                Set_Value("VATAX_TaxType_ID", VATAX_TaxType_ID);
        }
        /** Get Tax Type.
@return Tax Type */
        public int GetVATAX_TaxType_ID()
        {
            Object ii = Get_Value("VATAX_TaxType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /**@param VA077_AutoDesAccNo Autodesk Account Number*/
        public void SetVA077_AutoDesAccNo(String VA077_AutoDesAccNo) { if (VA077_AutoDesAccNo != null && VA077_AutoDesAccNo.Length > 255) { log.Warning("Length > 255 - truncated"); VA077_AutoDesAccNo = VA077_AutoDesAccNo.Substring(0, 255); } Set_Value("VA077_AutoDesAccNo", VA077_AutoDesAccNo); }/** Get Autodesk Account Number.
@return Autodesk Account Number */
        public String GetVA077_AutoDesAccNo() { return (String)Get_Value("VA077_AutoDesAccNo"); }/** Set Mailing Address.
@param VA077_IsMailAdd Mailing Address */
        public void SetVA077_IsMailAdd(Boolean VA077_IsMailAdd) { Set_Value("VA077_IsMailAdd", VA077_IsMailAdd); }/** Get Mailing Address.
@return Mailing Address */
        public Boolean IsVA077_IsMailAdd() { Object oo = Get_Value("VA077_IsMailAdd"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Customer Location Number.
@param VA077_LocNo Identifies the Location Number for the customer address */
        public void SetVA077_LocNo(String VA077_LocNo) { if (VA077_LocNo != null && VA077_LocNo.Length > 60) { log.Warning("Length > 60 - truncated"); VA077_LocNo = VA077_LocNo.Substring(0, 60); } Set_Value("VA077_LocNo", VA077_LocNo); }/** Get Customer Location Number.
@return Identifies the Location Number for the customer address */
        public String GetVA077_LocNo() { return (String)Get_Value("VA077_LocNo"); }

    }

}
