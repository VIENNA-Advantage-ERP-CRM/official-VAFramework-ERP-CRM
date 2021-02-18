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
/** Generated Model for VAPA_Evaluate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_Evaluate : PO
{
public X_VAPA_Evaluate (Context ctx, int VAPA_Evaluate_ID, Trx trxName) : base (ctx, VAPA_Evaluate_ID, trxName)
{
/** if (VAPA_Evaluate_ID == 0)
{
SetMeasureDataType (null);	// T
SetMeasureType (null);	// M
SetName (null);
SetVAPA_Evaluate_ID (0);
}
 */
}
public X_VAPA_Evaluate (Ctx ctx, int VAPA_Evaluate_ID, Trx trxName) : base (ctx, VAPA_Evaluate_ID, trxName)
{
/** if (VAPA_Evaluate_ID == 0)
{
SetMeasureDataType (null);	// T
SetMeasureType (null);	// M
SetName (null);
SetVAPA_Evaluate_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Evaluate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Evaluate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Evaluate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_Evaluate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381920L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065131L;
/** VAF_TableView_ID=441 */
public static int Table_ID;
 // =441;

/** TableName=VAPA_Evaluate */
public static String Table_Name="VAPA_Evaluate";

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
StringBuilder sb = new StringBuilder ("X_VAPA_Evaluate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Project Type.
@param VAB_ProjectType_ID Type of the project */
public void SetVAB_ProjectType_ID (int VAB_ProjectType_ID)
{
if (VAB_ProjectType_ID <= 0) Set_Value ("VAB_ProjectType_ID", null);
else
Set_Value ("VAB_ProjectType_ID", VAB_ProjectType_ID);
}
/** Get Project Type.
@return Type of the project */
public int GetVAB_ProjectType_ID() 
{
Object ii = Get_Value("VAB_ProjectType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Calculation Class.
@param CalculationClass Java Class for calculation, implementing Interface Measure */
public void SetCalculationClass (String CalculationClass)
{
if (CalculationClass != null && CalculationClass.Length > 60)
{
log.Warning("Length > 60 - truncated");
CalculationClass = CalculationClass.Substring(0,60);
}
Set_Value ("CalculationClass", CalculationClass);
}
/** Get Calculation Class.
@return Java Class for calculation, implementing Interface Measure */
public String GetCalculationClass() 
{
return (String)Get_Value("CalculationClass");
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
/** Set Manual Actual.
@param ManualActual Manually entered actual value */
public void SetManualActual (Decimal? ManualActual)
{
Set_Value ("ManualActual", (Decimal?)ManualActual);
}
/** Get Manual Actual.
@return Manually entered actual value */
public Decimal GetManualActual() 
{
Object bd =Get_Value("ManualActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Note.
@param ManualNote Note for manual entry */
public void SetManualNote (String ManualNote)
{
if (ManualNote != null && ManualNote.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ManualNote = ManualNote.Substring(0,2000);
}
Set_Value ("ManualNote", ManualNote);
}
/** Get Note.
@return Note for manual entry */
public String GetManualNote() 
{
return (String)Get_Value("ManualNote");
}

/** MeasureDataType VAF_Control_Ref_ID=369 */
public static int MEASUREDATATYPE_VAF_Control_Ref_ID=369;
/** Status Qty/Amount = S */
public static String MEASUREDATATYPE_StatusQtyAmount = "S";
/** Qty/Amount in Time = T */
public static String MEASUREDATATYPE_QtyAmountInTime = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMeasureDataTypeValid (String test)
{
return test.Equals("S") || test.Equals("T");
}
/** Set Measure Data Type.
@param MeasureDataType Type of data - Status or in Time */
public void SetMeasureDataType (String MeasureDataType)
{
if (MeasureDataType == null) throw new ArgumentException ("MeasureDataType is mandatory");
if (!IsMeasureDataTypeValid(MeasureDataType))
throw new ArgumentException ("MeasureDataType Invalid value - " + MeasureDataType + " - Reference_ID=369 - S - T");
if (MeasureDataType.Length > 1)
{
log.Warning("Length > 1 - truncated");
MeasureDataType = MeasureDataType.Substring(0,1);
}
Set_Value ("MeasureDataType", MeasureDataType);
}
/** Get Measure Data Type.
@return Type of data - Status or in Time */
public String GetMeasureDataType() 
{
return (String)Get_Value("MeasureDataType");
}

/** MeasureType VAF_Control_Ref_ID=231 */
public static int MEASURETYPE_VAF_Control_Ref_ID=231;
/** Achievements = A */
public static String MEASURETYPE_Achievements = "A";
/** Calculated = C */
public static String MEASURETYPE_Calculated = "C";
/** Manual = M */
public static String MEASURETYPE_Manual = "M";
/** Project = P */
public static String MEASURETYPE_Project = "P";
/** Request = Q */
public static String MEASURETYPE_Request = "Q";
/** Ratio = R */
public static String MEASURETYPE_Ratio = "R";
/** User defined = U */
public static String MEASURETYPE_UserDefined = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMeasureTypeValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("M") || test.Equals("P") || test.Equals("Q") || test.Equals("R") || test.Equals("U");
}
/** Set Measure Type.
@param MeasureType Determines how the actual performance is derived */
public void SetMeasureType (String MeasureType)
{
if (MeasureType == null) throw new ArgumentException ("MeasureType is mandatory");
if (!IsMeasureTypeValid(MeasureType))
throw new ArgumentException ("MeasureType Invalid value - " + MeasureType + " - Reference_ID=231 - A - C - M - P - Q - R - U");
if (MeasureType.Length > 1)
{
log.Warning("Length > 1 - truncated");
MeasureType = MeasureType.Substring(0,1);
}
Set_Value ("MeasureType", MeasureType);
}
/** Get Measure Type.
@return Determines how the actual performance is derived */
public String GetMeasureType() 
{
return (String)Get_Value("MeasureType");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
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
/** Set Benchmark.
@param VAPA_Std_BM_ID Performance Benchmark */
public void SetVAPA_Std_BM_ID (int VAPA_Std_BM_ID)
{
if (VAPA_Std_BM_ID <= 0) Set_Value ("VAPA_Std_BM_ID", null);
else
Set_Value ("VAPA_Std_BM_ID", VAPA_Std_BM_ID);
}
/** Get Benchmark.
@return Performance Benchmark */
public int GetVAPA_Std_BM_ID() 
{
Object ii = Get_Value("VAPA_Std_BM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Reporting Hierarchy.
@param VAPA_FinancialReportingOrder_ID Optional Reporting Hierarchy - If not selected the default hierarchy trees are used. */
public void SetVAPA_FinancialReportingOrder_ID (int VAPA_FinancialReportingOrder_ID)
{
if (VAPA_FinancialReportingOrder_ID <= 0) Set_Value ("VAPA_FinancialReportingOrder_ID", null);
else
Set_Value ("VAPA_FinancialReportingOrder_ID", VAPA_FinancialReportingOrder_ID);
}
/** Get Reporting Hierarchy.
@return Optional Reporting Hierarchy - If not selected the default hierarchy trees are used. */
public int GetVAPA_FinancialReportingOrder_ID() 
{
Object ii = Get_Value("VAPA_FinancialReportingOrder_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Measure Calculation.
@param VAPA_EvaluateCalc_ID Calculation method for measuring performance */
public void SetVAPA_EvaluateCalc_ID (int VAPA_EvaluateCalc_ID)
{
if (VAPA_EvaluateCalc_ID <= 0) Set_Value ("VAPA_EvaluateCalc_ID", null);
else
Set_Value ("VAPA_EvaluateCalc_ID", VAPA_EvaluateCalc_ID);
}
/** Get Measure Calculation.
@return Calculation method for measuring performance */
public int GetVAPA_EvaluateCalc_ID() 
{
Object ii = Get_Value("VAPA_EvaluateCalc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Measure.
@param VAPA_Evaluate_ID Concrete Performance Measurement */
public void SetVAPA_Evaluate_ID (int VAPA_Evaluate_ID)
{
if (VAPA_Evaluate_ID < 1) throw new ArgumentException ("VAPA_Evaluate_ID is mandatory.");
Set_ValueNoCheck ("VAPA_Evaluate_ID", VAPA_Evaluate_ID);
}
/** Get Measure.
@return Concrete Performance Measurement */
public int GetVAPA_Evaluate_ID() 
{
Object ii = Get_Value("VAPA_Evaluate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Ratio.
@param VAPA_Ratio_ID Performace Ratio */
public void SetVAPA_Ratio_ID (int VAPA_Ratio_ID)
{
if (VAPA_Ratio_ID <= 0) Set_Value ("VAPA_Ratio_ID", null);
else
Set_Value ("VAPA_Ratio_ID", VAPA_Ratio_ID);
}
/** Get Ratio.
@return Performace Ratio */
public int GetVAPA_Ratio_ID() 
{
Object ii = Get_Value("VAPA_Ratio_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param VAR_Req_Type_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetVAR_Req_Type_ID (int VAR_Req_Type_ID)
{
if (VAR_Req_Type_ID <= 0) Set_Value ("VAR_Req_Type_ID", null);
else
Set_Value ("VAR_Req_Type_ID", VAR_Req_Type_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetVAR_Req_Type_ID() 
{
Object ii = Get_Value("VAR_Req_Type_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
