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
/** Generated Model for C_Conversion_Rate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Conversion_Rate : PO
{
public X_C_Conversion_Rate (Context ctx, int C_Conversion_Rate_ID, Trx trxName) : base (ctx, C_Conversion_Rate_ID, trxName)
{
/** if (C_Conversion_Rate_ID == 0)
{
SetC_ConversionType_ID (0);
SetC_Conversion_Rate_ID (0);
SetC_Currency_ID (0);
SetC_Currency_To_ID (0);
SetDivideRate (0.0);
SetMultiplyRate (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_C_Conversion_Rate (Ctx ctx, int C_Conversion_Rate_ID, Trx trxName) : base (ctx, C_Conversion_Rate_ID, trxName)
{
/** if (C_Conversion_Rate_ID == 0)
{
SetC_ConversionType_ID (0);
SetC_Conversion_Rate_ID (0);
SetC_Currency_ID (0);
SetC_Currency_To_ID (0);
SetDivideRate (0.0);
SetMultiplyRate (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Conversion_Rate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Conversion_Rate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Conversion_Rate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Conversion_Rate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371466L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054677L;
/** AD_Table_ID=140 */
public static int Table_ID;
 // =140;

/** TableName=C_Conversion_Rate */
public static String Table_Name="C_Conversion_Rate";

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
StringBuilder sb = new StringBuilder ("X_C_Conversion_Rate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID)
{
if (C_ConversionType_ID < 1) throw new ArgumentException ("C_ConversionType_ID is mandatory.");
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
if (C_Conversion_Rate_ID < 1) throw new ArgumentException ("C_Conversion_Rate_ID is mandatory.");
Set_ValueNoCheck ("C_Conversion_Rate_ID", C_Conversion_Rate_ID);
}
/** Get Conversion Rate.
@return Rate used for converting currencies */
public int GetC_Conversion_Rate_ID() 
{
Object ii = Get_Value("C_Conversion_Rate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Conversion_Rate_ID().ToString());
}

/** C_Currency_ID AD_Reference_ID=112 */
public static int C_CURRENCY_ID_AD_Reference_ID=112;
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
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
/** Set Discontinued.
@param C_Currency_ID_To Discontinued */
public void SetC_Currency_ID_To (int C_Currency_ID_To)
{
Set_ValueNoCheck ("C_Currency_ID_To", C_Currency_ID_To);
}
/** Get Discontinued.
@return Discontinued */
public int GetC_Currency_ID_To() 
{
Object ii = Get_Value("C_Currency_ID_To");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_Currency_To_ID AD_Reference_ID=112 */
public static int C_CURRENCY_TO_ID_AD_Reference_ID=112;
/** Set Currency To.
@param C_Currency_To_ID Target currency */
public void SetC_Currency_To_ID (int C_Currency_To_ID)
{
if (C_Currency_To_ID < 1) throw new ArgumentException ("C_Currency_To_ID is mandatory.");
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
/** Set Divide Rate.
@param DivideRate To convert Source number to Target number, the Source is divided */
public void SetDivideRate (Decimal? DivideRate)
{
if (DivideRate == null) throw new ArgumentException ("DivideRate is mandatory.");
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
/** Set Multiply Rate.
@param MultiplyRate Rate to multiple the source by to calculate the target. */
public void SetMultiplyRate (Decimal? MultiplyRate)
{
if (MultiplyRate == null) throw new ArgumentException ("MultiplyRate is mandatory.");
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
if (ValidFrom == null) throw new ArgumentException ("ValidFrom is mandatory.");
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
