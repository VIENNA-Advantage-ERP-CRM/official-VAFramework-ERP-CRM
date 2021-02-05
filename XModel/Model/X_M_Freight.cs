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
/** Generated Model for VAM_Freight
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_Freight : PO
{
public X_VAM_Freight (Context ctx, int VAM_Freight_ID, Trx trxName) : base (ctx, VAM_Freight_ID, trxName)
{
/** if (VAM_Freight_ID == 0)
{
SetVAB_Currency_ID (0);
SetFreightAmt (0.0);
SetVAM_FreightCategory_ID (0);
SetVAM_Freight_ID (0);
SetVAM_ShippingMethod_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_VAM_Freight (Ctx ctx, int VAM_Freight_ID, Trx trxName) : base (ctx, VAM_Freight_ID, trxName)
{
/** if (VAM_Freight_ID == 0)
{
SetVAB_Currency_ID (0);
SetFreightAmt (0.0);
SetVAM_FreightCategory_ID (0);
SetVAM_Freight_ID (0);
SetVAM_ShippingMethod_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Freight (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Freight (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Freight (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_Freight()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379365L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062576L;
/** VAF_TableView_ID=596 */
public static int Table_ID;
 // =596;

/** TableName=VAM_Freight */
public static String Table_Name="VAM_Freight";

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
StringBuilder sb = new StringBuilder ("X_VAM_Freight[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Country.
@param VAB_Country_ID Country */
public void SetVAB_Country_ID (int VAB_Country_ID)
{
if (VAB_Country_ID <= 0) Set_Value ("VAB_Country_ID", null);
else
Set_Value ("VAB_Country_ID", VAB_Country_ID);
}
/** Get Country.
@return Country */
public int GetVAB_Country_ID() 
{
Object ii = Get_Value("VAB_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
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
/** Set Region.
@param VAB_RegionState_ID Identifies a geographical Region */
public void SetVAB_RegionState_ID (int VAB_RegionState_ID)
{
if (VAB_RegionState_ID <= 0) Set_Value ("VAB_RegionState_ID", null);
else
Set_Value ("VAB_RegionState_ID", VAB_RegionState_ID);
}
/** Get Region.
@return Identifies a geographical Region */
public int GetVAB_RegionState_ID() 
{
Object ii = Get_Value("VAB_RegionState_ID");
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
@param VAM_FreightCategory_ID Category of the Freight */
public void SetVAM_FreightCategory_ID (int VAM_FreightCategory_ID)
{
if (VAM_FreightCategory_ID < 1) throw new ArgumentException ("VAM_FreightCategory_ID is mandatory.");
Set_Value ("VAM_FreightCategory_ID", VAM_FreightCategory_ID);
}
/** Get Freight Category.
@return Category of the Freight */
public int GetVAM_FreightCategory_ID() 
{
Object ii = Get_Value("VAM_FreightCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight.
@param VAM_Freight_ID Freight Rate */
public void SetVAM_Freight_ID (int VAM_Freight_ID)
{
if (VAM_Freight_ID < 1) throw new ArgumentException ("VAM_Freight_ID is mandatory.");
Set_ValueNoCheck ("VAM_Freight_ID", VAM_Freight_ID);
}
/** Get Freight.
@return Freight Rate */
public int GetVAM_Freight_ID() 
{
Object ii = Get_Value("VAM_Freight_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight Carrier.
@param VAM_ShippingMethod_ID Method or manner of product delivery */
public void SetVAM_ShippingMethod_ID (int VAM_ShippingMethod_ID)
{
if (VAM_ShippingMethod_ID < 1) throw new ArgumentException ("VAM_ShippingMethod_ID is mandatory.");
Set_ValueNoCheck ("VAM_ShippingMethod_ID", VAM_ShippingMethod_ID);
}
/** Get Freight Carrier.
@return Method or manner of product delivery */
public int GetVAM_ShippingMethod_ID() 
{
Object ii = Get_Value("VAM_ShippingMethod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_ShippingMethod_ID().ToString());
}

/** To_Country_ID VAF_Control_Ref_ID=156 */
public static int TO_COUNTRY_ID_VAF_Control_Ref_ID=156;
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

/** To_Region_ID VAF_Control_Ref_ID=157 */
public static int TO_REGION_ID_VAF_Control_Ref_ID=157;
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
