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
/** Generated Model for I_ReportLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_ReportLine : PO
{
public X_I_ReportLine (Context ctx, int I_ReportLine_ID, Trx trxName) : base (ctx, I_ReportLine_ID, trxName)
{
/** if (I_ReportLine_ID == 0)
{
SetI_IsImported (null);	// N
SetI_ReportLine_ID (0);
}
 */
}
public X_I_ReportLine (Ctx ctx, int I_ReportLine_ID, Trx trxName) : base (ctx, I_ReportLine_ID, trxName)
{
/** if (I_ReportLine_ID == 0)
{
SetI_IsImported (null);	// N
SetI_ReportLine_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ReportLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ReportLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_ReportLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_ReportLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377610L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060821L;
/** AD_Table_ID=535 */
public static int Table_ID;
 // =535;

/** TableName=I_ReportLine */
public static String Table_Name="I_ReportLine";

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
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_I_ReportLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AmountType AD_Reference_ID=235 */
public static int AMOUNTTYPE_AD_Reference_ID=235;
/** Period Balance = BP */
public static String AMOUNTTYPE_PeriodBalance = "BP";
/** Total Balance = BT */
public static String AMOUNTTYPE_TotalBalance = "BT";
/** Year Balance = BY */
public static String AMOUNTTYPE_YearBalance = "BY";
/** Period Credit Only = CP */
public static String AMOUNTTYPE_PeriodCreditOnly = "CP";
/** Total Credit Only = CT */
public static String AMOUNTTYPE_TotalCreditOnly = "CT";
/** Year Credit Only = CY */
public static String AMOUNTTYPE_YearCreditOnly = "CY";
/** Period Debit Only = DP */
public static String AMOUNTTYPE_PeriodDebitOnly = "DP";
/** Total Debit Only = DT */
public static String AMOUNTTYPE_TotalDebitOnly = "DT";
/** Year Debit Only = DY */
public static String AMOUNTTYPE_YearDebitOnly = "DY";
/** Period Quantity = QP */
public static String AMOUNTTYPE_PeriodQuantity = "QP";
/** Total Quantity = QT */
public static String AMOUNTTYPE_TotalQuantity = "QT";
/** Year Quantity = QY */
public static String AMOUNTTYPE_YearQuantity = "QY";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAmountTypeValid (String test)
{
return test == null || test.Equals("BP") || test.Equals("BT") || test.Equals("BY") || test.Equals("CP") || test.Equals("CT") || test.Equals("CY") || test.Equals("DP") || test.Equals("DT") || test.Equals("DY") || test.Equals("QP") || test.Equals("QT") || test.Equals("QY");
}
/** Set Amount Type.
@param AmountType Type of amount to report */
public void SetAmountType (String AmountType)
{
if (!IsAmountTypeValid(AmountType))
throw new ArgumentException ("AmountType Invalid value - " + AmountType + " - Reference_ID=235 - BP - BT - BY - CP - CT - CY - DP - DT - DY - QP - QT - QY");
if (AmountType != null && AmountType.Length > 2)
{
log.Warning("Length > 2 - truncated");
AmountType = AmountType.Substring(0,2);
}
Set_Value ("AmountType", AmountType);
}
/** Get Amount Type.
@return Type of amount to report */
public String GetAmountType() 
{
return (String)Get_Value("AmountType");
}
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);
else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CalculationType AD_Reference_ID=236 */
public static int CALCULATIONTYPE_AD_Reference_ID=236;
/** Add (Op1+Op2) = A */
public static String CALCULATIONTYPE_AddOp1PlusOp2 = "A";
/** Percentage (Op1 of Op2) = P */
public static String CALCULATIONTYPE_PercentageOp1OfOp2 = "P";
/** Add Range (Op1 to Op2) = R */
public static String CALCULATIONTYPE_AddRangeOp1ToOp2 = "R";
/** Subtract (Op1-Op2) = S */
public static String CALCULATIONTYPE_SubtractOp1_Op2 = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCalculationTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("P") || test.Equals("R") || test.Equals("S");
}
/** Set Calculation.
@param CalculationType Calculation */
public void SetCalculationType (String CalculationType)
{
if (!IsCalculationTypeValid(CalculationType))
throw new ArgumentException ("CalculationType Invalid value - " + CalculationType + " - Reference_ID=236 - A - P - R - S");
if (CalculationType != null && CalculationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CalculationType = CalculationType.Substring(0,1);
}
Set_Value ("CalculationType", CalculationType);
}
/** Get Calculation.
@return Calculation */
public String GetCalculationType() 
{
return (String)Get_Value("CalculationType");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Element Key.
@param ElementValue Key of the element */
public void SetElementValue (String ElementValue)
{
if (ElementValue != null && ElementValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ElementValue = ElementValue.Substring(0,40);
}
Set_Value ("ElementValue", ElementValue);
}
/** Get Element Key.
@return Key of the element */
public String GetElementValue() 
{
return (String)Get_Value("ElementValue");
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}

/** I_IsImported AD_Reference_ID=420 */
public static int I_ISIMPORTED_AD_Reference_ID=420;
/** Error = E */
public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid (String test)
{
return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (String I_IsImported)
{
if (I_IsImported == null) throw new ArgumentException ("I_IsImported is mandatory");
if (!IsI_IsImportedValid(I_IsImported))
throw new ArgumentException ("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
if (I_IsImported.Length > 1)
{
log.Warning("Length > 1 - truncated");
I_IsImported = I_IsImported.Substring(0,1);
}
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public String GetI_IsImported() 
{
return (String)Get_Value("I_IsImported");
}
/** Set Import Report Line Set.
@param I_ReportLine_ID Import Report Line Set values */
public void SetI_ReportLine_ID (int I_ReportLine_ID)
{
if (I_ReportLine_ID < 1) throw new ArgumentException ("I_ReportLine_ID is mandatory.");
Set_ValueNoCheck ("I_ReportLine_ID", I_ReportLine_ID);
}
/** Get Import Report Line Set.
@return Import Report Line Set values */
public int GetI_ReportLine_ID() 
{
Object ii = Get_Value("I_ReportLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Printed.
@param IsPrinted Indicates if this document / line is printed */
public void SetIsPrinted (Boolean IsPrinted)
{
Set_Value ("IsPrinted", IsPrinted);
}
/** Get Printed.
@return Indicates if this document / line is printed */
public Boolean IsPrinted() 
{
Object oo = Get_Value("IsPrinted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** LineType AD_Reference_ID=241 */
public static int LINETYPE_AD_Reference_ID=241;
/** Calculation = C */
public static String LINETYPE_Calculation = "C";
/** Segment Value = S */
public static String LINETYPE_SegmentValue = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLineTypeValid (String test)
{
return test == null || test.Equals("C") || test.Equals("S");
}
/** Set Line Type.
@param LineType Line Type */
public void SetLineType (String LineType)
{
if (!IsLineTypeValid(LineType))
throw new ArgumentException ("LineType Invalid value - " + LineType + " - Reference_ID=241 - C - S");
if (LineType != null && LineType.Length > 1)
{
log.Warning("Length > 1 - truncated");
LineType = LineType.Substring(0,1);
}
Set_Value ("LineType", LineType);
}
/** Get Line Type.
@return Line Type */
public String GetLineType() 
{
return (String)Get_Value("LineType");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
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
/** Set Report Line Set.
@param PA_ReportLineSet_ID Report Line Set */
public void SetPA_ReportLineSet_ID (int PA_ReportLineSet_ID)
{
if (PA_ReportLineSet_ID <= 0) Set_Value ("PA_ReportLineSet_ID", null);
else
Set_Value ("PA_ReportLineSet_ID", PA_ReportLineSet_ID);
}
/** Get Report Line Set.
@return Report Line Set */
public int GetPA_ReportLineSet_ID() 
{
Object ii = Get_Value("PA_ReportLineSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Line.
@param PA_ReportLine_ID Report Line */
public void SetPA_ReportLine_ID (int PA_ReportLine_ID)
{
if (PA_ReportLine_ID <= 0) Set_Value ("PA_ReportLine_ID", null);
else
Set_Value ("PA_ReportLine_ID", PA_ReportLine_ID);
}
/** Get Report Line.
@return Report Line */
public int GetPA_ReportLine_ID() 
{
Object ii = Get_Value("PA_ReportLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Source.
@param PA_ReportSource_ID Restriction of what will be shown in Report Line */
public void SetPA_ReportSource_ID (int PA_ReportSource_ID)
{
if (PA_ReportSource_ID <= 0) Set_Value ("PA_ReportSource_ID", null);
else
Set_Value ("PA_ReportSource_ID", PA_ReportSource_ID);
}
/** Get Report Source.
@return Restriction of what will be shown in Report Line */
public int GetPA_ReportSource_ID() 
{
Object ii = Get_Value("PA_ReportSource_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PostingType AD_Reference_ID=125 */
public static int POSTINGTYPE_AD_Reference_ID=125;
/** Actual = A */
public static String POSTINGTYPE_Actual = "A";
/** Budget = B */
public static String POSTINGTYPE_Budget = "B";
/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";
/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";
/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType != null && PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_Value ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
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
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
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
/** Set Report Line Set Name.
@param ReportLineSetName Name of the Report Line Set */
public void SetReportLineSetName (String ReportLineSetName)
{
if (ReportLineSetName != null && ReportLineSetName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ReportLineSetName = ReportLineSetName.Substring(0,60);
}
Set_Value ("ReportLineSetName", ReportLineSetName);
}
/** Get Report Line Set Name.
@return Name of the Report Line Set */
public String GetReportLineSetName() 
{
return (String)Get_Value("ReportLineSetName");
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
