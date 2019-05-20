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
/** Generated Model for M_Freight
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Freight : PO
{
public X_M_Freight (Context ctx, int M_Freight_ID, Trx trxName) : base (ctx, M_Freight_ID, trxName)
{
/** if (M_Freight_ID == 0)
{
SetC_Currency_ID (0);
SetFreightAmt (0.0);
SetM_FreightCategory_ID (0);
SetM_Freight_ID (0);
SetM_Shipper_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_M_Freight (Ctx ctx, int M_Freight_ID, Trx trxName) : base (ctx, M_Freight_ID, trxName)
{
/** if (M_Freight_ID == 0)
{
SetC_Currency_ID (0);
SetFreightAmt (0.0);
SetM_FreightCategory_ID (0);
SetM_Freight_ID (0);
SetM_Shipper_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Freight (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Freight (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Freight (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Freight()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379365L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062576L;
/** AD_Table_ID=596 */
public static int Table_ID;
 // =596;

/** TableName=M_Freight */
public static String Table_Name="M_Freight";

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
StringBuilder sb = new StringBuilder ("X_M_Freight[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Country.
@param C_Country_ID Country */
public void SetC_Country_ID (int C_Country_ID)
{
if (C_Country_ID <= 0) Set_Value ("C_Country_ID", null);
else
Set_Value ("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country */
public int GetC_Country_ID() 
{
Object ii = Get_Value("C_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
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
/** Set Region.
@param C_Region_ID Identifies a geographical Region */
public void SetC_Region_ID (int C_Region_ID)
{
if (C_Region_ID <= 0) Set_Value ("C_Region_ID", null);
else
Set_Value ("C_Region_ID", C_Region_ID);
}
/** Get Region.
@return Identifies a geographical Region */
public int GetC_Region_ID() 
{
Object ii = Get_Value("C_Region_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight Amount.
@param FreightAmt Freight Amount */
public void SetFreightAmt (Decimal? FreightAmt)
{
if (FreightAmt == null) throw new ArgumentException ("FreightAmt is mandatory.");
Set_Value ("FreightAmt", (Decimal?)FreightAmt);
}
/** Get Freight Amount.
@return Freight Amount */
public Decimal GetFreightAmt() 
{
Object bd =Get_Value("FreightAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Freight Category.
@param M_FreightCategory_ID Category of the Freight */
public void SetM_FreightCategory_ID (int M_FreightCategory_ID)
{
if (M_FreightCategory_ID < 1) throw new ArgumentException ("M_FreightCategory_ID is mandatory.");
Set_Value ("M_FreightCategory_ID", M_FreightCategory_ID);
}
/** Get Freight Category.
@return Category of the Freight */
public int GetM_FreightCategory_ID() 
{
Object ii = Get_Value("M_FreightCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight.
@param M_Freight_ID Freight Rate */
public void SetM_Freight_ID (int M_Freight_ID)
{
if (M_Freight_ID < 1) throw new ArgumentException ("M_Freight_ID is mandatory.");
Set_ValueNoCheck ("M_Freight_ID", M_Freight_ID);
}
/** Get Freight.
@return Freight Rate */
public int GetM_Freight_ID() 
{
Object ii = Get_Value("M_Freight_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight Carrier.
@param M_Shipper_ID Method or manner of product delivery */
public void SetM_Shipper_ID (int M_Shipper_ID)
{
if (M_Shipper_ID < 1) throw new ArgumentException ("M_Shipper_ID is mandatory.");
Set_ValueNoCheck ("M_Shipper_ID", M_Shipper_ID);
}
/** Get Freight Carrier.
@return Method or manner of product delivery */
public int GetM_Shipper_ID() 
{
Object ii = Get_Value("M_Shipper_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Shipper_ID().ToString());
}

/** To_Country_ID AD_Reference_ID=156 */
public static int TO_COUNTRY_ID_AD_Reference_ID=156;
/** Set To.
@param To_Country_ID Receiving Country */
public void SetTo_Country_ID (int To_Country_ID)
{
if (To_Country_ID <= 0) Set_Value ("To_Country_ID", null);
else
Set_Value ("To_Country_ID", To_Country_ID);
}
/** Get To.
@return Receiving Country */
public int GetTo_Country_ID() 
{
Object ii = Get_Value("To_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** To_Region_ID AD_Reference_ID=157 */
public static int TO_REGION_ID_AD_Reference_ID=157;
/** Set To.
@param To_Region_ID Receiving Region */
public void SetTo_Region_ID (int To_Region_ID)
{
if (To_Region_ID <= 0) Set_Value ("To_Region_ID", null);
else
Set_Value ("To_Region_ID", To_Region_ID);
}
/** Get To.
@return Receiving Region */
public int GetTo_Region_ID() 
{
Object ii = Get_Value("To_Region_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
