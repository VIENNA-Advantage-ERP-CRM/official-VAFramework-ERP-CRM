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
    using System.Data;/** Generated Model for VAGL_LineDimension
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAGL_LineDimension : PO
    {
        public X_VAGL_LineDimension(Context ctx, int VAGL_LineDimension_ID, Trx trxName)
            : base(ctx, VAGL_LineDimension_ID, trxName)
        {/** if (VAGL_LineDimension_ID == 0){SetVAGL_JRNLLine_ID (0);SetVAGL_LineDimension_ID (0);SetLineType (null);// @ElementType@
} */
        }
        public X_VAGL_LineDimension(Ctx ctx, int VAGL_LineDimension_ID, Trx trxName)
            : base(ctx, VAGL_LineDimension_ID, trxName)
        {/** if (VAGL_LineDimension_ID == 0){SetVAGL_JRNLLine_ID (0);SetVAGL_LineDimension_ID (0);SetLineType (null);// @ElementType@
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAGL_LineDimension(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAGL_LineDimension(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAGL_LineDimension(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAGL_LineDimension() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27751729338550L;/** Last Updated Timestamp 7/27/2016 12:50:21 PM */
        public static long updatedMS = 1469604021761L;/** VAF_TableView_ID=1000595 */
        public static int Table_ID; // =1000595;
        /** TableName=VAGL_LineDimension */
        public static String Table_Name = "VAGL_LineDimension";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAGL_LineDimension[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Column.
@param VAF_Column_ID Column in the table */
        public void SetVAF_Column_ID(int VAF_Column_ID)
        {
            if (VAF_Column_ID <= 0) Set_Value("VAF_Column_ID", null);
            else
                Set_Value("VAF_Column_ID", VAF_Column_ID);
        }/** Get Column.
@return Column in the table */
        public int GetVAF_Column_ID() { Object ii = Get_Value("VAF_Column_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Amount.
@param Amount Amount in a defined currency */
        public void SetAmount(Decimal Amount) { Set_Value("Amount", Amount); }/** Get Amount.
@return Amount in a defined currency */
        public Decimal GetAmount() { Object ii = Get_Value("Amount"); if (ii == null) return 0; return Convert.ToDecimal(ii); }/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
        public void SetVAB_BillingCode_ID(int VAB_BillingCode_ID)
        {
            if (VAB_BillingCode_ID <= 0) Set_Value("VAB_BillingCode_ID", null);
            else
                Set_Value("VAB_BillingCode_ID", VAB_BillingCode_ID);
        }/** Get Activity.
@return Business Activity */
        public int GetVAB_BillingCode_ID() { Object ii = Get_Value("VAB_BillingCode_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID() { Object ii = Get_Value("VAB_BusinessPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
        public void SetVAB_Promotion_ID(int VAB_Promotion_ID)
        {
            if (VAB_Promotion_ID <= 0) Set_Value("VAB_Promotion_ID", null);
            else
                Set_Value("VAB_Promotion_ID", VAB_Promotion_ID);
        }/** Get Campaign.
@return Marketing Campaign */
        public int GetVAB_Promotion_ID() { Object ii = Get_Value("VAB_Promotion_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Account Element.
@param VAB_Acct_Element_ID Account Element */
        public void SetVAB_Acct_Element_ID(int VAB_Acct_Element_ID)
        {
            if (VAB_Acct_Element_ID <= 0) Set_Value("VAB_Acct_Element_ID", null);
            else
                Set_Value("VAB_Acct_Element_ID", VAB_Acct_Element_ID);
        }/** Get Account Element.
@return Account Element */
        public int GetVAB_Acct_Element_ID() { Object ii = Get_Value("VAB_Acct_Element_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Element.
@param VAB_Element_ID Accounting Element */
        public void SetVAB_Element_ID(int VAB_Element_ID)
        {
            if (VAB_Element_ID <= 0) Set_Value("VAB_Element_ID", null);
            else
                Set_Value("VAB_Element_ID", VAB_Element_ID);
        }/** Get Element.
@return Accounting Element */
        public int GetVAB_Element_ID() { Object ii = Get_Value("VAB_Element_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Address.
@param VAB_Address_ID Location or Address */
        public void SetVAB_Address_ID(int VAB_Address_ID)
        {
            if (VAB_Address_ID <= 0) Set_Value("VAB_Address_ID", null);
            else
                Set_Value("VAB_Address_ID", VAB_Address_ID);
        }/** Get Address.
@return Location or Address */
        public int GetVAB_Address_ID() { Object ii = Get_Value("VAB_Address_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity.
@param VAB_Project_ID Business Opportunity */
        public void SetVAB_Project_ID(int VAB_Project_ID)
        {
            if (VAB_Project_ID <= 0) Set_Value("VAB_Project_ID", null);
            else
                Set_Value("VAB_Project_ID", VAB_Project_ID);
        }/** Get Opportunity.
@return Business Opportunity */
        public int GetVAB_Project_ID() { Object ii = Get_Value("VAB_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
        public void SetVAB_SalesRegionState_ID(int VAB_SalesRegionState_ID)
        {
            if (VAB_SalesRegionState_ID <= 0) Set_Value("VAB_SalesRegionState_ID", null);
            else
                Set_Value("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
        }/** Get Sales Region.
@return Sales coverage region */
        public int GetVAB_SalesRegionState_ID() { Object ii = Get_Value("VAB_SalesRegionState_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Journal Line.
@param VAGL_JRNLLine_ID General Ledger Journal Line */
        public void SetVAGL_JRNLLine_ID(int VAGL_JRNLLine_ID) { if (VAGL_JRNLLine_ID < 1) throw new ArgumentException("VAGL_JRNLLine_ID is mandatory."); Set_ValueNoCheck("VAGL_JRNLLine_ID", VAGL_JRNLLine_ID); }/** Get Journal Line.
@return General Ledger Journal Line */
        public int GetVAGL_JRNLLine_ID() { Object ii = Get_Value("VAGL_JRNLLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set VAGL_LineDimension_ID.
@param VAGL_LineDimension_ID VAGL_LineDimension_ID */
        public void SetVAGL_LineDimension_ID(int VAGL_LineDimension_ID) { if (VAGL_LineDimension_ID < 1) throw new ArgumentException("VAGL_LineDimension_ID is mandatory."); Set_ValueNoCheck("VAGL_LineDimension_ID", VAGL_LineDimension_ID); }/** Get VAGL_LineDimension_ID.
@return VAGL_LineDimension_ID */
        public int GetVAGL_LineDimension_ID() { Object ii = Get_Value("VAGL_LineDimension_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Line No.
@param Line Unique line for this document */
        public void SetLine(int Line) { Set_Value("Line", Line); }/** Get Line No.
@return Unique line for this document */
        public int GetLine() { Object ii = Get_Value("Line"); if (ii == null) return 0; return Convert.ToInt32(ii); }




        /** LineType VAF_Control_Ref_ID=181 */
        public static int LINETYPE_VAF_Control_Ref_ID = 181;/** Organization = OO */
        public static String LINETYPE_Organization = "OO";/** Account = AC */
        public static String LINETYPE_Account = "AC";/** Product = PR */
        public static String LINETYPE_Product = "PR";/** BPartner = BP */
        public static String LINETYPE_BPartner = "BP";/** Org Trx = OT */
        public static String LINETYPE_OrgTrx = "OT";/** Location From = LF */
        public static String LINETYPE_LocationFrom = "LF";/** Location To = LT */
        public static String LINETYPE_LocationTo = "LT";/** Sales Region = SR */
        public static String LINETYPE_SalesRegion = "SR";/** Project = PJ */
        public static String LINETYPE_Project = "PJ";/** Campaign = MC */
        public static String LINETYPE_Campaign = "MC";/** User List 1 = U1 */
        public static String LINETYPE_UserList1 = "U1";/** User List 2 = U2 */
        public static String LINETYPE_UserList2 = "U2";/** Activity = AY */
        public static String LINETYPE_Activity = "AY";/** Sub Account = SA */
        public static String LINETYPE_SubAccount = "SA";/** User Element 1 = X1 */
        public static String LINETYPE_UserElement1 = "X1";/** User Element 2 = X2 */
        public static String LINETYPE_UserElement2 = "X2";/** User Element 3 = X3 */
        public static String LINETYPE_UserElement3 = "X3";/** User Element 4 = X4 */
        public static String LINETYPE_UserElement4 = "X4";/** User Element 5 = X5 */
        public static String LINETYPE_UserElement5 = "X5";/** User Element 6 = X6 */
        public static String LINETYPE_UserElement6 = "X6";/** User Element 7 = X7 */
        public static String LINETYPE_UserElement7 = "X7";/** User Element 8 = X8 */
        public static String LINETYPE_UserElement8 = "X8";/** User Element 9 = X9 */
        public static String LINETYPE_UserElement9 = "X9";



        /** Is test a valid value.
    @param test testvalue
    @returns true if valid **/
        public bool IsLineTypeValid(String test) { return test.Equals("OO") || test.Equals("AC") || test.Equals("PR") || test.Equals("BP") || test.Equals("OT") || test.Equals("LF") || test.Equals("LT") || test.Equals("SR") || test.Equals("PJ") || test.Equals("MC") || test.Equals("U1") || test.Equals("U2") || test.Equals("AY") || test.Equals("SA") || test.Equals("X1") || test.Equals("X2") || test.Equals("X3") || test.Equals("X4") || test.Equals("X5") || test.Equals("X6") || test.Equals("X7") || test.Equals("X8") || test.Equals("X9"); }/** Set Line Type.
@param LineType ElementType */
        public void SetLineType(String LineType)
        {
            if (LineType == null) throw new ArgumentException("LineType is mandatory"); if (!IsLineTypeValid(LineType))
                throw new ArgumentException("LineType Invalid value - " + LineType + " - Reference_ID=181 - OO - AC - PR - BP - OT - LF - LT - SR - PJ - MC - U1 - U2 - AY - SA - X1 - X2 - X3 - X4 - X5 - X6 - X7 - X8 - X9"); if (LineType.Length > 2) { log.Warning("Length > 2 - truncated"); LineType = LineType.Substring(0, 2); }
            Set_ValueNoCheck("LineType", LineType);
        }/** Get Line Type.
@return ElementType */
        public String GetLineType() { return (String)Get_Value("LineType"); }







        /** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Org_ID VAF_Control_Ref_ID=130 */
        public static int ORG_ID_VAF_Control_Ref_ID = 130;/** Set Organization.
@param Org_ID Organizational entity within client */
        public void SetOrg_ID(int Org_ID)
        {
            if (Org_ID <= 0) Set_Value("Org_ID", null);
            else
                Set_Value("Org_ID", Org_ID);
        }/** Get Organization.
@return Organizational entity within client */
        public int GetOrg_ID() { Object ii = Get_Value("Org_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Quantity.
@param Qty Quantity */
        public void SetQty(Decimal? Qty) { Set_Value("Qty", (Decimal?)Qty); }/** Get Quantity.
@return Quantity */
        public Decimal GetQty() { Object bd = Get_Value("Qty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
        public void SetSeqNo(int SeqNo) { Set_Value("SeqNo", SeqNo); }/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
        public int GetSeqNo() { Object ii = Get_Value("SeqNo"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /// <summary>
        ///  Set Dimension Value.
        /// </summary>
        /// <param name="DimensionValue">Dimension Value </param>
        public void SetDimensionValue(String DimensionValue) { if (DimensionValue != null && DimensionValue.Length > 10) { log.Warning("Length > 10 - truncated"); DimensionValue = DimensionValue.Substring(0, 10); } Set_Value("DimensionValue", DimensionValue); }
        /// <summary>
        /// Get Dimension Value.
        /// </summary>
        /// <returns>Dimension Value</returns>
        public String GetDimensionValue() { return (String)Get_Value("DimensionValue"); }

        /// <summary>
        /// Set Dimension Value.
        /// </summary>
        /// <param name="DimensionName">Dimension Value</param>
        public void SetDimensionName(String DimensionName) { throw new ArgumentException("DimensionName Is virtual column"); }
        /// <summary>
        /// Get Dimension Value.
        /// </summary>
        /// <returns>Dimension Value</returns>
        public String GetDimensionName() { return (String)Get_Value("DimensionName"); }
    }
}