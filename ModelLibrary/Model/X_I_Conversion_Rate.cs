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
/** Generated Model for I_Conversion_Rate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_Conversion_Rate : PO
{
public X_I_Conversion_Rate (Context ctx, int I_Conversion_Rate_ID, Trx trxName) : base (ctx, I_Conversion_Rate_ID, trxName)
{
/** if (I_Conversion_Rate_ID == 0)
{
SetI_Conversion_Rate_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_Conversion_Rate (Ctx ctx, int I_Conversion_Rate_ID, Trx trxName) : base (ctx, I_Conversion_Rate_ID, trxName)
{
/** if (I_Conversion_Rate_ID == 0)
{
SetI_Conversion_Rate_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Conversion_Rate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Conversion_Rate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Conversion_Rate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_Conversion_Rate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376983L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060194L;
/** AD_Table_ID=641 */
public static int Table_ID;
 // =641;

/** TableName=I_Conversion_Rate */
public static String Table_Name="I_Conversion_Rate";

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
StringBuilder sb = new StringBuilder ("X_I_Conversion_Rate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID)
{
if (C_ConversionType_ID <= 0) Set_Value ("C_ConversionType_ID", null);
else
Set_Value ("C_ConversionType_ID", C_ConversionType_ID);
}
/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetC_ConversionType_ID() 
{
Object ii = Get_Value("C_ConversionType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Conversion Rate.
@param C_Conversion_Rate_ID Rate used for converting currencies */
public void SetC_Conversion_Rate_ID (int C_Conversion_Rate_ID)
{
if (C_Conversion_Rate_ID <= 0) Set_Value ("C_Conversion_Rate_ID", null);
else
Set_Value ("C_Conversion_Rate_ID", C_Conversion_Rate_ID);
}
/** Get Conversion Rate.
@return Rate used for converting currencies */
public int GetC_Conversion_Rate_ID() 
{
Object ii = Get_Value("C_Conversion_Rate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_Currency_To_ID AD_Reference_ID=112 */
public static int C_CURRENCY_TO_ID_AD_Reference_ID=112;
/** Set Currency To.
@param C_Currency_To_ID Target currency */
public void SetC_Currency_To_ID (int C_Currency_To_ID)
{
if (C_Currency_To_ID <= 0) Set_Value ("C_Currency_To_ID", null);
else
Set_Value ("C_Currency_To_ID", C_Currency_To_ID);
}
/** Get Currency To.
@return Target currency */
public int GetC_Currency_To_ID() 
{
Object ii = Get_Value("C_Currency_To_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency Type Key.
@param ConversionTypeValue Key value for the Currency Conversion Rate Type */
public void SetConversionTypeValue (String ConversionTypeValue)
{
if (ConversionTypeValue != null && ConversionTypeValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ConversionTypeValue = ConversionTypeValue.Substring(0,40);
}
Set_Value ("ConversionTypeValue", ConversionTypeValue);
}
/** Get Currency Type Key.
@return Key value for the Currency Conversion Rate Type */
public String GetConversionTypeValue() 
{
return (String)Get_Value("ConversionTypeValue");
}
/** Set Create Reciprocal Rate.
@param CreateReciprocalRate Create Reciprocal Rate from current information */
public void SetCreateReciprocalRate (Boolean CreateReciprocalRate)
{
Set_Value ("CreateReciprocalRate", CreateReciprocalRate);
}
/** Get Create Reciprocal Rate.
@return Create Reciprocal Rate from current information */
public Boolean IsCreateReciprocalRate() 
{
Object oo = Get_Value("CreateReciprocalRate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Divide Rate.
@param DivideRate To convert Source number to Target number, the Source is divided */
public void SetDivideRate (Decimal? DivideRate)
{
Set_Value ("DivideRate", (Decimal?)DivideRate);
}
/** Get Divide Rate.
@return To convert Source number to Target number, the Source is divided */
public Decimal GetDivideRate() 
{
Object bd =Get_Value("DivideRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set ISO Currency Code.
@param ISO_Code Three letter ISO 4217 Code of the Currency */
public void SetISO_Code (String ISO_Code)
{
if (ISO_Code != null && ISO_Code.Length > 3)
{
log.Warning("Length > 3 - truncated");
ISO_Code = ISO_Code.Substring(0,3);
}
Set_Value ("ISO_Code", ISO_Code);
}
/** Get ISO Currency Code.
@return Three letter ISO 4217 Code of the Currency */
public String GetISO_Code() 
{
return (String)Get_Value("ISO_Code");
}
/** Set ISO Currency To Code.
@param ISO_Code_To Three letter ISO 4217 Code of the To Currency */
public void SetISO_Code_To (String ISO_Code_To)
{
if (ISO_Code_To != null && ISO_Code_To.Length > 3)
{
log.Warning("Length > 3 - truncated");
ISO_Code_To = ISO_Code_To.Substring(0,3);
}
Set_Value ("ISO_Code_To", ISO_Code_To);
}
/** Get ISO Currency To Code.
@return Three letter ISO 4217 Code of the To Currency */
public String GetISO_Code_To() 
{
return (String)Get_Value("ISO_Code_To");
}
/** Set Import Conversion Rate.
@param I_Conversion_Rate_ID Import Currency Conversion Rate */
public void SetI_Conversion_Rate_ID (int I_Conversion_Rate_ID)
{
if (I_Conversion_Rate_ID < 1) throw new ArgumentException ("I_Conversion_Rate_ID is mandatory.");
Set_ValueNoCheck ("I_Conversion_Rate_ID", I_Conversion_Rate_ID);
}
/** Get Import Conversion Rate.
@return Import Currency Conversion Rate */
public int GetI_Conversion_Rate_ID() 
{
Object ii = Get_Value("I_Conversion_Rate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetI_Conversion_Rate_ID().ToString());
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
/** Set Multiply Rate.
@param MultiplyRate Rate to multiple the source by to calculate the target. */
public void SetMultiplyRate (Decimal? MultiplyRate)
{
Set_Value ("MultiplyRate", (Decimal?)MultiplyRate);
}
/** Get Multiply Rate.
@return Rate to multiple the source by to calculate the target. */
public Decimal GetMultiplyRate() 
{
Object bd =Get_Value("MultiplyRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}
