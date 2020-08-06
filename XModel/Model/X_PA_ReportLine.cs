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
    using System.Data;/** Generated Model for PA_ReportLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_PA_ReportLine : PO
    {
        public X_PA_ReportLine(Context ctx, int PA_ReportLine_ID, Trx trxName)
            : base(ctx, PA_ReportLine_ID, trxName)
        {/** if (PA_ReportLine_ID == 0){SetIsPrinted (true);// Y
SetIsSummary (false);SetLineType (null);SetName (null);SetPA_ReportLineSet_ID (0);SetPA_ReportLine_ID (0);SetSeqNo (0);// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM PA_ReportLine WHERE PA_ReportLineSet_ID=@PA_ReportLineSet_ID@
} */
        }
        public X_PA_ReportLine(Ctx ctx, int PA_ReportLine_ID, Trx trxName)
            : base(ctx, PA_ReportLine_ID, trxName)
        {/** if (PA_ReportLine_ID == 0){SetIsPrinted (true);// Y
SetIsSummary (false);SetLineType (null);SetName (null);SetPA_ReportLineSet_ID (0);SetPA_ReportLine_ID (0);SetSeqNo (0);// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM PA_ReportLine WHERE PA_ReportLineSet_ID=@PA_ReportLineSet_ID@
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_PA_ReportLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_PA_ReportLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_PA_ReportLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_PA_ReportLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27741621413794L;/** Last Updated Timestamp 4/1/2016 1:04:57 PM */
        public static long updatedMS = 1459496097005L;/** AD_Table_ID=448 */
        public static int Table_ID; // =448;
        /** TableName=PA_ReportLine */
        public static String Table_Name = "PA_ReportLine";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_PA_ReportLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }
        /** AmountType AD_Reference_ID=235 */
        public static int AMOUNTTYPE_AD_Reference_ID = 235;/** Period Balance = BP */
        public static String AMOUNTTYPE_PeriodBalance = "BP";/** Total Balance = BT */
        public static String AMOUNTTYPE_TotalBalance = "BT";/** Year Balance = BY */
        public static String AMOUNTTYPE_YearBalance = "BY";/** Period Credit Only = CP */
        public static String AMOUNTTYPE_PeriodCreditOnly = "CP";/** Total Credit Only = CT */
        public static String AMOUNTTYPE_TotalCreditOnly = "CT";/** Year Credit Only = CY */
        public static String AMOUNTTYPE_YearCreditOnly = "CY";/** Period Debit Only = DP */
        public static String AMOUNTTYPE_PeriodDebitOnly = "DP";/** Total Debit Only = DT */
        public static String AMOUNTTYPE_TotalDebitOnly = "DT";/** Year Debit Only = DY */
        public static String AMOUNTTYPE_YearDebitOnly = "DY";/** Period Quantity = QP */
        public static String AMOUNTTYPE_PeriodQuantity = "QP";/** Total Quantity = QT */
        public static String AMOUNTTYPE_TotalQuantity = "QT";/** Year Quantity = QY */
        public static String AMOUNTTYPE_YearQuantity = "QY";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsAmountTypeValid(String test) { return test == null || test.Equals("BP") || test.Equals("BT") || test.Equals("BY") || test.Equals("CP") || test.Equals("CT") || test.Equals("CY") || test.Equals("DP") || test.Equals("DT") || test.Equals("DY") || test.Equals("QP") || test.Equals("QT") || test.Equals("QY"); }/** Set Amount Type.
@param AmountType Type of amount to report */
        public void SetAmountType(String AmountType)
        {
            if (!IsAmountTypeValid(AmountType))
                throw new ArgumentException("AmountType Invalid value - " + AmountType + " - Reference_ID=235 - BP - BT - BY - CP - CT - CY - DP - DT - DY - QP - QT - QY"); if (AmountType != null && AmountType.Length > 2) { log.Warning("Length > 2 - truncated"); AmountType = AmountType.Substring(0, 2); } Set_Value("AmountType", AmountType);
        }/** Get Amount Type.
@return Type of amount to report */
        public String GetAmountType() { return (String)Get_Value("AmountType"); }
        /** CalculationType AD_Reference_ID=236 */
        public static int CALCULATIONTYPE_AD_Reference_ID = 236;/** Add (Op1+Op2) = A */
        public static String CALCULATIONTYPE_AddOp1PlusOp2 = "A";/** Percentage (Op1 of Op2) = P */
        public static String CALCULATIONTYPE_PercentageOp1OfOp2 = "P";/** Add Range (Op1 to Op2) = R */
        public static String CALCULATIONTYPE_AddRangeOp1ToOp2 = "R";/** Subtract (Op1-Op2) = S */
        public static String CALCULATIONTYPE_SubtractOp1_Op2 = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCalculationTypeValid(String test) { return test == null || test.Equals("A") || test.Equals("P") || test.Equals("R") || test.Equals("S"); }/** Set Calculation.
@param CalculationType Calculation */
        public void SetCalculationType(String CalculationType)
        {
            if (!IsCalculationTypeValid(CalculationType))
                throw new ArgumentException("CalculationType Invalid value - " + CalculationType + " - Reference_ID=236 - A - P - R - S"); if (CalculationType != null && CalculationType.Length > 1) { log.Warning("Length > 1 - truncated"); CalculationType = CalculationType.Substring(0, 1); } Set_Value("CalculationType", CalculationType);
        }/** Get Calculation.
@return Calculation */
        public String GetCalculationType() { return (String)Get_Value("CalculationType"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }
        /** ElementType AD_Reference_ID=181 */
        public static int ELEMENTTYPE_AD_Reference_ID = 181;/** Account = AC */
        public static String ELEMENTTYPE_Account = "AC";/** Activity = AY */
        public static String ELEMENTTYPE_Activity = "AY";/** BPartner = BP */
        public static String ELEMENTTYPE_BPartner = "BP";/** Location From = LF */
        public static String ELEMENTTYPE_LocationFrom = "LF";/** Location To = LT */
        public static String ELEMENTTYPE_LocationTo = "LT";/** Campaign = MC */
        public static String ELEMENTTYPE_Campaign = "MC";/** Organization = OO */
        public static String ELEMENTTYPE_Organization = "OO";/** Org Trx = OT */
        public static String ELEMENTTYPE_OrgTrx = "OT";/** Project = PJ */
        public static String ELEMENTTYPE_Project = "PJ";/** Product = PR */
        public static String ELEMENTTYPE_Product = "PR";/** Sub Account = SA */
        public static String ELEMENTTYPE_SubAccount = "SA";/** Sales Region = SR */
        public static String ELEMENTTYPE_SalesRegion = "SR";/** User List 1 = U1 */
        public static String ELEMENTTYPE_UserList1 = "U1";/** User List 2 = U2 */
        public static String ELEMENTTYPE_UserList2 = "U2";/** User Element 1 = X1 */
        public static String ELEMENTTYPE_UserElement1 = "X1";/** User Element 2 = X2 */
        public static String ELEMENTTYPE_UserElement2 = "X2";/** User Element 3 = X3 */
        public static String ELEMENTTYPE_UserElement3 = "X3";/** User Element 4 = X4 */
        public static String ELEMENTTYPE_UserElement4 = "X4";/** User Element 5 = X5 */
        public static String ELEMENTTYPE_UserElement5 = "X5";/** User Element 6 = X6 */
        public static String ELEMENTTYPE_UserElement6 = "X6";/** User Element 7 = X7 */
        public static String ELEMENTTYPE_UserElement7 = "X7";/** User Element 8 = X8 */
        public static String ELEMENTTYPE_UserElement8 = "X8";/** User Element 9 = X9 */
        public static String ELEMENTTYPE_UserElement9 = "X9";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsElementTypeValid(String test) { return test == null || test.Equals("AC") || test.Equals("AY") || test.Equals("BP") || test.Equals("LF") || test.Equals("LT") || test.Equals("MC") || test.Equals("OO") || test.Equals("OT") || test.Equals("PJ") || test.Equals("PR") || test.Equals("SA") || test.Equals("SR") || test.Equals("U1") || test.Equals("U2") || test.Equals("X1") || test.Equals("X2") || test.Equals("X3") || test.Equals("X4") || test.Equals("X5") || test.Equals("X6") || test.Equals("X7") || test.Equals("X8") || test.Equals("X9"); }/** Set Type.
@param ElementType Element Type (account or user defined) */
        public void SetElementType(String ElementType)
        {
            if (!IsElementTypeValid(ElementType))
                throw new ArgumentException("ElementType Invalid value - " + ElementType + " - Reference_ID=181 - AC - AY - BP - LF - LT - MC - OO - OT - PJ - PR - SA - SR - U1 - U2 - X1 - X2 - X3 - X4 - X5 - X6 - X7 - X8 - X9"); if (ElementType != null && ElementType.Length > 2) { log.Warning("Length > 2 - truncated"); ElementType = ElementType.Substring(0, 2); } Set_Value("ElementType", ElementType);
        }/** Get Type.
@return Element Type (account or user defined) */
        public String GetElementType() { return (String)Get_Value("ElementType"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }
        /** FRPT_DGLineFormatting AD_Reference_ID=1000224 */
        public static int FRPT_DGLINEFORMATTING_AD_Reference_ID = 1000224;/** Set DimensionGroup Line Formatting.
@param FRPT_DGLineFormatting DimensionGroup Line Formatting */
        public void SetFRPT_DGLineFormatting(int FRPT_DGLineFormatting) { Set_Value("FRPT_DGLineFormatting", FRPT_DGLineFormatting); }/** Get DimensionGroup Line Formatting.
@return DimensionGroup Line Formatting */
        public int GetFRPT_DGLineFormatting() { Object ii = Get_Value("FRPT_DGLineFormatting"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set FRPT_Group_ID.
@param FRPT_Group_ID FRPT_Group_ID */
        public void SetFRPT_Group_ID(int FRPT_Group_ID)
        {
            if (FRPT_Group_ID <= 0) Set_Value("FRPT_Group_ID", null);
            else
                Set_Value("FRPT_Group_ID", FRPT_Group_ID);
        }/** Get FRPT_Group_ID.
@return FRPT_Group_ID */
        public int GetFRPT_Group_ID() { Object ii = Get_Value("FRPT_Group_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** FRPT_LinkWith AD_Reference_ID=1000192 */
        public static int FRPT_LINKWITH_AD_Reference_ID = 1000192;/** Ledger Code = C */
        public static String FRPT_LINKWITH_LedgerCode = "C";/** Ledger Group = G */
        public static String FRPT_LINKWITH_LedgerGroup = "G";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsFRPT_LinkWithValid(String test) { return test == null || test.Equals("C") || test.Equals("G"); }/** Set Link With.
@param FRPT_LinkWith Link With */
        public void SetFRPT_LinkWith(String FRPT_LinkWith)
        {
            if (!IsFRPT_LinkWithValid(FRPT_LinkWith))
                throw new ArgumentException("FRPT_LinkWith Invalid value - " + FRPT_LinkWith + " - Reference_ID=1000192 - C - G"); if (FRPT_LinkWith != null && FRPT_LinkWith.Length > 1) { log.Warning("Length > 1 - truncated"); FRPT_LinkWith = FRPT_LinkWith.Substring(0, 1); } Set_Value("FRPT_LinkWith", FRPT_LinkWith);
        }/** Get Link With.
@return Link With */
        public String GetFRPT_LinkWith() { return (String)Get_Value("FRPT_LinkWith"); }/** Set List Sources.
@param FRPT_ListSources List Sources */
        public void SetFRPT_ListSources(Boolean FRPT_ListSources) { Set_Value("FRPT_ListSources", FRPT_ListSources); }/** Get List Sources.
@return List Sources */
        public Boolean IsFRPT_ListSources() { Object oo = Get_Value("FRPT_ListSources"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set List Transactions.
@param FRPT_ListTrx List Transactions */
        public void SetFRPT_ListTrx(Boolean FRPT_ListTrx) { Set_Value("FRPT_ListTrx", FRPT_ListTrx); }/** Get List Transactions.
@return List Transactions */
        public Boolean IsFRPT_ListTrx() { Object oo = Get_Value("FRPT_ListTrx"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** FRPT_RowLineFormatting AD_Reference_ID=1000224 */
        public static int FRPT_ROWLINEFORMATTING_AD_Reference_ID = 1000224;/** Set Row Line Formatting.
@param FRPT_RowLineFormatting Row Line Formatting */
        public void SetFRPT_RowLineFormatting(int FRPT_RowLineFormatting) { Set_Value("FRPT_RowLineFormatting", FRPT_RowLineFormatting); }/** Get Row Line Formatting.
@return Row Line Formatting */
        public int GetFRPT_RowLineFormatting() { Object ii = Get_Value("FRPT_RowLineFormatting"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** FRPT_SourceLineFormatting AD_Reference_ID=1000224 */
        public static int FRPT_SOURCELINEFORMATTING_AD_Reference_ID = 1000224;/** Set Source Line Formatting.
@param FRPT_SourceLineFormatting Source Line Formatting */
        public void SetFRPT_SourceLineFormatting(int FRPT_SourceLineFormatting) { Set_Value("FRPT_SourceLineFormatting", FRPT_SourceLineFormatting); }/** Get Source Line Formatting.
@return Source Line Formatting */
        public int GetFRPT_SourceLineFormatting() { Object ii = Get_Value("FRPT_SourceLineFormatting"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** FRPT_SubGroupLineFormatting AD_Reference_ID=1000224 */
        public static int FRPT_SUBGROUPLINEFORMATTING_AD_Reference_ID = 1000224;/** Set Sub-Group Line Formatting.
@param FRPT_SubGroupLineFormatting Sub-Group Line Formatting */
        public void SetFRPT_SubGroupLineFormatting(int FRPT_SubGroupLineFormatting) { Set_Value("FRPT_SubGroupLineFormatting", FRPT_SubGroupLineFormatting); }/** Get Sub-Group Line Formatting.
@return Sub-Group Line Formatting */
        public int GetFRPT_SubGroupLineFormatting() { Object ii = Get_Value("FRPT_SubGroupLineFormatting"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** FRPT_TrxLineFormatting AD_Reference_ID=1000224 */
        public static int FRPT_TRXLINEFORMATTING_AD_Reference_ID = 1000224;/** Set Trx Line Formatting.
@param FRPT_TrxLineFormatting Trx Line Formatting */
        public void SetFRPT_TrxLineFormatting(int FRPT_TrxLineFormatting) { Set_Value("FRPT_TrxLineFormatting", FRPT_TrxLineFormatting); }/** Get Trx Line Formatting.
@return Trx Line Formatting */
        public int GetFRPT_TrxLineFormatting() { Object ii = Get_Value("FRPT_TrxLineFormatting"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Budget.
@param GL_Budget_ID General Ledger Budget */
        public void SetGL_Budget_ID(int GL_Budget_ID)
        {
            if (GL_Budget_ID <= 0) Set_Value("GL_Budget_ID", null);
            else
                Set_Value("GL_Budget_ID", GL_Budget_ID);
        }/** Get Budget.
@return General Ledger Budget */
        public int GetGL_Budget_ID() { Object ii = Get_Value("GL_Budget_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Printed.
@param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted) { Set_Value("IsPrinted", IsPrinted); }/** Get Printed.
@return Indicates if this document / line is printed */
        public Boolean IsPrinted() { Object oo = Get_Value("IsPrinted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Summary Level.
@param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary) { Set_Value("IsSummary", IsSummary); }/** Get Summary Level.
@return This is a summary entity */
        public Boolean IsSummary() { Object oo = Get_Value("IsSummary"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** LineType AD_Reference_ID=241 */
        public static int LINETYPE_AD_Reference_ID = 241;/** Calculation = C */
        public static String LINETYPE_Calculation = "C";/** Segment Value = S */
        public static String LINETYPE_SegmentValue = "S";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsLineTypeValid(String test) { return test.Equals("C") || test.Equals("S"); }/** Set Line Type.
@param LineType Line Type */
        public void SetLineType(String LineType)
        {
            if (LineType == null) throw new ArgumentException("LineType is mandatory"); if (!IsLineTypeValid(LineType))
                throw new ArgumentException("LineType Invalid value - " + LineType + " - Reference_ID=241 - C - S"); if (LineType.Length > 1) { log.Warning("Length > 1 - truncated"); LineType = LineType.Substring(0, 1); } Set_Value("LineType", LineType);
        }/** Get Line Type.
@return Line Type */
        public String GetLineType() { return (String)Get_Value("LineType"); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 60) { log.Warning("Length > 60 - truncated"); Name = Name.Substring(0, 60); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetName()); }
        /** Oper_1_ID AD_Reference_ID=240 */
        public static int OPER_1_ID_AD_Reference_ID = 240;/** Set Operand 1.
@param Oper_1_ID First operand for calculation */
        public void SetOper_1_ID(int Oper_1_ID)
        {
            if (Oper_1_ID <= 0) Set_Value("Oper_1_ID", null);
            else
                Set_Value("Oper_1_ID", Oper_1_ID);
        }/** Get Operand 1.
@return First operand for calculation */
        public int GetOper_1_ID() { Object ii = Get_Value("Oper_1_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Oper_2_ID AD_Reference_ID=240 */
        public static int OPER_2_ID_AD_Reference_ID = 240;/** Set Operand 2.
@param Oper_2_ID Second operand for calculation */
        public void SetOper_2_ID(int Oper_2_ID)
        {
            if (Oper_2_ID <= 0) Set_Value("Oper_2_ID", null);
            else
                Set_Value("Oper_2_ID", Oper_2_ID);
        }/** Get Operand 2.
@return Second operand for calculation */
        public int GetOper_2_ID() { Object ii = Get_Value("Oper_2_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Report Line Set.
@param PA_ReportLineSet_ID Report Line Set */
        public void SetPA_ReportLineSet_ID(int PA_ReportLineSet_ID) { if (PA_ReportLineSet_ID < 1) throw new ArgumentException("PA_ReportLineSet_ID is mandatory."); Set_ValueNoCheck("PA_ReportLineSet_ID", PA_ReportLineSet_ID); }/** Get Report Line Set.
@return Report Line Set */
        public int GetPA_ReportLineSet_ID() { Object ii = Get_Value("PA_ReportLineSet_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Report Line.
@param PA_ReportLine_ID Report Line */
        public void SetPA_ReportLine_ID(int PA_ReportLine_ID) { if (PA_ReportLine_ID < 1) throw new ArgumentException("PA_ReportLine_ID is mandatory."); Set_ValueNoCheck("PA_ReportLine_ID", PA_ReportLine_ID); }/** Get Report Line.
@return Report Line */
        public int GetPA_ReportLine_ID() { Object ii = Get_Value("PA_ReportLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** PostingType AD_Reference_ID=125 */
        public static int POSTINGTYPE_AD_Reference_ID = 125;/** Actual = A */
        public static String POSTINGTYPE_Actual = "A";/** Budget = B */
        public static String POSTINGTYPE_Budget = "B";/** Commitment = E */
        public static String POSTINGTYPE_Commitment = "E";/** Reservation = R */
        public static String POSTINGTYPE_Reservation = "R";/** Statistical = S */
        public static String POSTINGTYPE_Statistical = "S";/** Virtual = V */
        public static String POSTINGTYPE_Virtual = "V";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsPostingTypeValid(String test) { return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S") || test.Equals("V"); }/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
        public void SetPostingType(String PostingType)
        {
            if (!IsPostingTypeValid(PostingType))
                throw new ArgumentException("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S - V"); if (PostingType != null && PostingType.Length > 1) { log.Warning("Length > 1 - truncated"); PostingType = PostingType.Substring(0, 1); } Set_Value("PostingType", PostingType);
        }/** Get PostingType.
@return The type of posted amount for the transaction */
        public String GetPostingType() { return (String)Get_Value("PostingType"); }/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
        public void SetSeqNo(int SeqNo) { Set_Value("SeqNo", SeqNo); }/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
        public int GetSeqNo() { Object ii = Get_Value("SeqNo"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}