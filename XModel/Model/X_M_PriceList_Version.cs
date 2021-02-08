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
/** Generated Model for VAM_PriceListVersion
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_PriceListVersion : PO
{
public X_VAM_PriceListVersion (Context ctx, int VAM_PriceListVersion_ID, Trx trxName) : base (ctx, VAM_PriceListVersion_ID, trxName)
{
/** if (VAM_PriceListVersion_ID == 0)
{
SetVAM_DiscountCalculation_ID (0);
SetVAM_PriceList_ID (0);
SetVAM_PriceListVersion_ID (0);
SetName (null);	// @#Date@
SetValidFrom (DateTime.Now);	// @#Date@
}
 */
}
public X_VAM_PriceListVersion (Ctx ctx, int VAM_PriceListVersion_ID, Trx trxName) : base (ctx, VAM_PriceListVersion_ID, trxName)
{
/** if (VAM_PriceListVersion_ID == 0)
{
SetVAM_DiscountCalculation_ID (0);
SetVAM_PriceList_ID (0);
SetVAM_PriceListVersion_ID (0);
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
public X_VAM_PriceListVersion (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PriceListVersion (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PriceListVersion (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_PriceListVersion()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380415L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063626L;
/** VAF_TableView_ID=295 */
public static int Table_ID;
 // =295;

/** TableName=VAM_PriceListVersion */
public static String Table_Name="VAM_PriceListVersion";

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
StringBuilder sb = new StringBuilder ("X_VAM_PriceListVersion[").Append(Get_ID()).Append("]");
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
@param VAM_DiscountCalculation_ID Schema to calculate price lists or the trade discount percentage */
public void SetVAM_DiscountCalculation_ID (int VAM_DiscountCalculation_ID)
{
if (VAM_DiscountCalculation_ID < 1) throw new ArgumentException ("VAM_DiscountCalculation_ID is mandatory.");
Set_Value ("VAM_DiscountCalculation_ID", VAM_DiscountCalculation_ID);
}
/** Get Discount Schema.
@return Schema to calculate price lists or the trade discount percentage */
public int GetVAM_DiscountCalculation_ID() 
{
Object ii = Get_Value("VAM_DiscountCalculation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Price List.
@param VAM_PriceList_ID Unique identifier of a Price List */
public void SetVAM_PriceList_ID (int VAM_PriceList_ID)
{
if (VAM_PriceList_ID < 1) throw new ArgumentException ("VAM_PriceList_ID is mandatory.");
Set_ValueNoCheck ("VAM_PriceList_ID", VAM_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetVAM_PriceList_ID() 
{
Object ii = Get_Value("VAM_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Price List Version.
@param VAM_PriceListVersion_ID Identifies a unique instance of a Price List */
public void SetVAM_PriceListVersion_ID (int VAM_PriceListVersion_ID)
{
if (VAM_PriceListVersion_ID < 1) throw new ArgumentException ("VAM_PriceListVersion_ID is mandatory.");
Set_ValueNoCheck ("VAM_PriceListVersion_ID", VAM_PriceListVersion_ID);
}
/** Get Price List Version.
@return Identifies a unique instance of a Price List */
public int GetVAM_PriceListVersion_ID() 
{
Object ii = Get_Value("VAM_PriceListVersion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAM_PriceListVersion_Base_ID VAF_Control_Ref_ID=188 */
public static int VAM_PriceListVersion_BASE_ID_VAF_Control_Ref_ID=188;
/** Set Base Price List.
@param VAM_PriceListVersion_Base_ID Source for Price list calculations */
public void SetVAM_PriceListVersion_Base_ID (int VAM_PriceListVersion_Base_ID)
{
if (VAM_PriceListVersion_Base_ID <= 0) Set_Value ("VAM_PriceListVersion_Base_ID", null);
else
Set_Value ("VAM_PriceListVersion_Base_ID", VAM_PriceListVersion_Base_ID);
}
/** Get Base Price List.
@return Source for Price list calculations */
public int GetVAM_PriceListVersion_Base_ID() 
{
Object ii = Get_Value("VAM_PriceListVersion_Base_ID");
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
