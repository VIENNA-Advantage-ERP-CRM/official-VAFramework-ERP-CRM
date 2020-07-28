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
/** Generated Model for M_PriceList_Version
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_PriceList_Version : PO
{
public X_M_PriceList_Version (Context ctx, int M_PriceList_Version_ID, Trx trxName) : base (ctx, M_PriceList_Version_ID, trxName)
{
/** if (M_PriceList_Version_ID == 0)
{
SetM_DiscountSchema_ID (0);
SetM_PriceList_ID (0);
SetM_PriceList_Version_ID (0);
SetName (null);	// @#Date@
SetValidFrom (DateTime.Now);	// @#Date@
}
 */
}
public X_M_PriceList_Version (Ctx ctx, int M_PriceList_Version_ID, Trx trxName) : base (ctx, M_PriceList_Version_ID, trxName)
{
/** if (M_PriceList_Version_ID == 0)
{
SetM_DiscountSchema_ID (0);
SetM_PriceList_ID (0);
SetM_PriceList_Version_ID (0);
SetName (null);	// @#Date@
SetValidFrom (DateTime.Now);	// @#Date@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList_Version (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList_Version (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PriceList_Version (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_PriceList_Version()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380415L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063626L;
/** AD_Table_ID=295 */
public static int Table_ID;
 // =295;

/** TableName=M_PriceList_Version */
public static String Table_Name="M_PriceList_Version";

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
StringBuilder sb = new StringBuilder ("X_M_PriceList_Version[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Discount Schema.
@param M_DiscountSchema_ID Schema to calculate price lists or the trade discount percentage */
public void SetM_DiscountSchema_ID (int M_DiscountSchema_ID)
{
if (M_DiscountSchema_ID < 1) throw new ArgumentException ("M_DiscountSchema_ID is mandatory.");
Set_Value ("M_DiscountSchema_ID", M_DiscountSchema_ID);
}
/** Get Discount Schema.
@return Schema to calculate price lists or the trade discount percentage */
public int GetM_DiscountSchema_ID() 
{
Object ii = Get_Value("M_DiscountSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Price List Version.
@param M_PriceList_Version_ID Identifies a unique instance of a Price List */
public void SetM_PriceList_Version_ID (int M_PriceList_Version_ID)
{
if (M_PriceList_Version_ID < 1) throw new ArgumentException ("M_PriceList_Version_ID is mandatory.");
Set_ValueNoCheck ("M_PriceList_Version_ID", M_PriceList_Version_ID);
}
/** Get Price List Version.
@return Identifies a unique instance of a Price List */
public int GetM_PriceList_Version_ID() 
{
Object ii = Get_Value("M_PriceList_Version_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_Pricelist_Version_Base_ID AD_Reference_ID=188 */
public static int M_PRICELIST_VERSION_BASE_ID_AD_Reference_ID=188;
/** Set Base Price List.
@param M_Pricelist_Version_Base_ID Source for Price list calculations */
public void SetM_Pricelist_Version_Base_ID (int M_Pricelist_Version_Base_ID)
{
if (M_Pricelist_Version_Base_ID <= 0) Set_Value ("M_Pricelist_Version_Base_ID", null);
else
Set_Value ("M_Pricelist_Version_Base_ID", M_Pricelist_Version_Base_ID);
}
/** Get Base Price List.
@return Source for Price list calculations */
public int GetM_Pricelist_Version_Base_ID() 
{
Object ii = Get_Value("M_Pricelist_Version_Base_ID");
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
/** Set Create.
@param ProcCreate Create */
public void SetProcCreate (String ProcCreate)
{
if (ProcCreate != null && ProcCreate.Length > 1)
{
log.Warning("Length > 1 - truncated");
ProcCreate = ProcCreate.Substring(0,1);
}
Set_Value ("ProcCreate", ProcCreate);
}
/** Get Create.
@return Create */
public String GetProcCreate() 
{
return (String)Get_Value("ProcCreate");
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
