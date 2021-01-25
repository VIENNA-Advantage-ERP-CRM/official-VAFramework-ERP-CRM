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
/** Generated Model for VAB_ExchangeRate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ExchangeRate : PO
{
public X_VAB_ExchangeRate (Context ctx, int VAB_ExchangeRate_ID, Trx trxName) : base (ctx, VAB_ExchangeRate_ID, trxName)
{
/** if (VAB_ExchangeRate_ID == 0)
{
SetVAB_CurrencyType_ID (0);
SetVAB_ExchangeRate_ID (0);
SetVAB_Currency_ID (0);
SetVAB_Currency_To_ID (0);
SetDivideRate (0.0);
SetMultiplyRate (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_VAB_ExchangeRate (Ctx ctx, int VAB_ExchangeRate_ID, Trx trxName) : base (ctx, VAB_ExchangeRate_ID, trxName)
{
/** if (VAB_ExchangeRate_ID == 0)
{
SetVAB_CurrencyType_ID (0);
SetVAB_ExchangeRate_ID (0);
SetVAB_Currency_ID (0);
SetVAB_Currency_To_ID (0);
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
public X_VAB_ExchangeRate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ExchangeRate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ExchangeRate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ExchangeRate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371466L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054677L;
/** VAF_TableView_ID=140 */
public static int Table_ID;
 // =140;

/** TableName=VAB_ExchangeRate */
public static String Table_Name="VAB_ExchangeRate";

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
StringBuilder sb = new StringBuilder ("X_VAB_ExchangeRate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Currency Type.
@param VAB_CurrencyType_ID Currency Conversion Rate Type */
public void SetVAB_CurrencyType_ID (int VAB_CurrencyType_ID)
{
if (VAB_CurrencyType_ID < 1) throw new ArgumentException ("VAB_CurrencyType_ID is mandatory.");
Set_Value ("VAB_CurrencyType_ID", VAB_CurrencyType_ID);
}
/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetVAB_CurrencyType_ID() 
{
Object ii = Get_Value("VAB_CurrencyType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Conversion Rate.
@param VAB_ExchangeRate_ID Rate used for converting currencies */
public void SetVAB_ExchangeRate_ID (int VAB_ExchangeRate_ID)
{
if (VAB_ExchangeRate_ID < 1) throw new ArgumentException ("VAB_ExchangeRate_ID is mandatory.");
Set_ValueNoCheck ("VAB_ExchangeRate_ID", VAB_ExchangeRate_ID);
}
/** Get Conversion Rate.
@return Rate used for converting currencies */
public int GetVAB_ExchangeRate_ID() 
{
Object ii = Get_Value("VAB_ExchangeRate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_ExchangeRate_ID().ToString());
}

/** VAB_Currency_ID VAF_Control_Ref_ID=112 */
public static int VAB_CURRENCY_ID_VAF_Control_Ref_ID=112;
/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");
Set_Value ("VAB_Currency_ID", VAB_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() 
{
Object ii = Get_Value("VAB_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Discontinued.
@param VAB_Currency_ID_To Discontinued */
public void SetVAB_Currency_ID_To (int VAB_Currency_ID_To)
{
Set_ValueNoCheck ("VAB_Currency_ID_To", VAB_Currency_ID_To);
}
/** Get Discontinued.
@return Discontinued */
public int GetVAB_Currency_ID_To() 
{
Object ii = Get_Value("VAB_Currency_ID_To");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_Currency_To_ID VAF_Control_Ref_ID=112 */
public static int VAB_CURRENCY_TO_ID_VAF_Control_Ref_ID=112;
/** Set Currency To.
@param VAB_Currency_To_ID Target currency */
public void SetVAB_Currency_To_ID (int VAB_Currency_To_ID)
{
if (VAB_Currency_To_ID < 1) throw new ArgumentException ("VAB_Currency_To_ID is mandatory.");
Set_Value ("VAB_Currency_To_ID", VAB_Currency_To_ID);
}
/** Get Currency To.
@return Target currency */
public int GetVAB_Currency_To_ID() 
{
Object ii = Get_Value("VAB_Currency_To_ID");
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
