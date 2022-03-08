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
/** Generated Model for M_PriceList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_PriceList : PO
{
public X_M_PriceList (Context ctx, int M_PriceList_ID, Trx trxName) : base (ctx, M_PriceList_ID, trxName)
{
/** if (M_PriceList_ID == 0)
{
SetC_Currency_ID (0);	// @$C_Currency_ID@
SetEnforcePriceLimit (false);
SetIsDefault (false);
SetIsSOPriceList (false);
SetIsTaxIncluded (false);
SetM_PriceList_ID (0);
SetName (null);
SetPricePrecision (0);	// 2
}
 */
}
public X_M_PriceList (Ctx ctx, int M_PriceList_ID, Trx trxName) : base (ctx, M_PriceList_ID, trxName)
{
/** if (M_PriceList_ID == 0)
{
SetC_Currency_ID (0);	// @$C_Currency_ID@
SetEnforcePriceLimit (false);
SetIsDefault (false);
SetIsSOPriceList (false);
SetIsTaxIncluded (false);
SetM_PriceList_ID (0);
SetName (null);
SetPricePrecision (0);	// 2
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_PriceList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380384L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063595L;
/** AD_Table_ID=255 */
public static int Table_ID;
 // =255;

/** TableName=M_PriceList */
public static String Table_Name="M_PriceList";

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
StringBuilder sb = new StringBuilder ("X_M_PriceList[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** BasePriceList_ID AD_Reference_ID=166 */
public static int BASEPRICELIST_ID_AD_Reference_ID=166;
/** Set Base Pricelist.
@param BasePriceList_ID Pricelist to be used, if product not found on this pricelist */
public void SetBasePriceList_ID (int BasePriceList_ID)
{
if (BasePriceList_ID <= 0) Set_Value ("BasePriceList_ID", null);
else
Set_Value ("BasePriceList_ID", BasePriceList_ID);
}
/** Get Base Pricelist.
@return Pricelist to be used, if product not found on this pricelist */
public int GetBasePriceList_ID() 
{
Object ii = Get_Value("BasePriceList_ID");
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
/** Set Enforce price limit.
@param EnforcePriceLimit Do not allow prices below the limit price */
public void SetEnforcePriceLimit (Boolean EnforcePriceLimit)
{
Set_Value ("EnforcePriceLimit", EnforcePriceLimit);
}
/** Get Enforce price limit.
@return Do not allow prices below the limit price */
public Boolean IsEnforcePriceLimit() 
{
Object oo = Get_Value("EnforcePriceLimit");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sales Price list.
@param IsSOPriceList This is a Sales Price List */
public void SetIsSOPriceList (Boolean IsSOPriceList)
{
Set_Value ("IsSOPriceList", IsSOPriceList);
}
/** Get Sales Price list.
@return This is a Sales Price List */
public Boolean IsSOPriceList() 
{
Object oo = Get_Value("IsSOPriceList");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price includes Tax.
@param IsTaxIncluded Tax is included in the price */
public void SetIsTaxIncluded (Boolean IsTaxIncluded)
{
Set_Value ("IsTaxIncluded", IsTaxIncluded);
}
/** Get Price includes Tax.
@return Tax is included in the price */
public Boolean IsTaxIncluded() 
{
Object oo = Get_Value("IsTaxIncluded");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID < 1) throw new ArgumentException ("M_PriceList_ID is mandatory.");
Set_ValueNoCheck ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Price Precision.
@param PricePrecision Precision (number of decimals) for the Price */
public void SetPricePrecision (int PricePrecision)
{
Set_Value ("PricePrecision", PricePrecision);
}
/** Get Price Precision.
@return Precision (number of decimals) for the Price */
public int GetPricePrecision() 
{
Object ii = Get_Value("PricePrecision");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** VA008_SalesRegion_ID AD_Reference_ID=1000177 */
public static int VA008_SALESREGION_ID_AD_Reference_ID = 1000177;
/** Set Sales Region.
@param VA008_SalesRegion_ID Sales Region */
public void SetVA008_SalesRegion_ID(int VA008_SalesRegion_ID)
{
    if (VA008_SalesRegion_ID <= 0) Set_Value("VA008_SalesRegion_ID", null);
    else
        Set_Value("VA008_SalesRegion_ID", VA008_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales Region */
public int GetVA008_SalesRegion_ID()
{
    Object ii = Get_Value("VA008_SalesRegion_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
