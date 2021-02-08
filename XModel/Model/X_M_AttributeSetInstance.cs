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
/** Generated Model for VAM_PFeature_SetInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_PFeature_SetInstance : PO
{
public X_VAM_PFeature_SetInstance (Context ctx, int VAM_PFeature_SetInstance_ID, Trx trxName) : base (ctx, VAM_PFeature_SetInstance_ID, trxName)
{
/** if (VAM_PFeature_SetInstance_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_PFeature_Set_ID (0);
}
 */
}
public X_VAM_PFeature_SetInstance (Ctx ctx, int VAM_PFeature_SetInstance_ID, Trx trxName) : base (ctx, VAM_PFeature_SetInstance_ID, trxName)
{
/** if (VAM_PFeature_SetInstance_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_PFeature_Set_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_PFeature_SetInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378394L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061605L;
/** VAF_TableView_ID=559 */
public static int Table_ID;
 // =559;

/** TableName=VAM_PFeature_SetInstance */
public static String Table_Name="VAM_PFeature_SetInstance";

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
StringBuilder sb = new StringBuilder ("X_VAM_PFeature_SetInstance[").Append(Get_ID()).Append("]");
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
/** Set Guarantee Date.
@param GuaranteeDate Date when guarantee expires */
public void SetGuaranteeDate (DateTime? GuaranteeDate)
{
Set_Value ("GuaranteeDate", (DateTime?)GuaranteeDate);
}
/** Get Guarantee Date.
@return Date when guarantee expires */
public DateTime? GetGuaranteeDate() 
{
return (DateTime?)Get_Value("GuaranteeDate");
}
/** Set Lot No.
@param Lot Lot number (alphanumeric) */
public void SetLot (String Lot)
{
if (Lot != null && Lot.Length > 40)
{
log.Warning("Length > 40 - truncated");
Lot = Lot.Substring(0,40);
}
Set_Value ("Lot", Lot);
}
/** Get Lot No.
@return Lot number (alphanumeric) */
public String GetLot() 
{
return (String)Get_Value("Lot");
}
/** Set Attribute Set Instance.
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
{
if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException ("VAM_PFeature_SetInstance_ID is mandatory.");
Set_ValueNoCheck ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetVAM_PFeature_SetInstance_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_PFeature_SetInstance_ID().ToString());
}
/** Set Attribute Set.
@param VAM_PFeature_Set_ID Product Attribute Set */
public void SetVAM_PFeature_Set_ID (int VAM_PFeature_Set_ID)
{
if (VAM_PFeature_Set_ID < 0) throw new ArgumentException ("VAM_PFeature_Set_ID is mandatory.");
Set_Value ("VAM_PFeature_Set_ID", VAM_PFeature_Set_ID);
}
/** Get Attribute Set.
@return Product Attribute Set */
public int GetVAM_PFeature_Set_ID() 
{
Object ii = Get_Value("VAM_PFeature_Set_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lot.
@param VAM_Lot_ID Product Lot Definition */
public void SetVAM_Lot_ID (int VAM_Lot_ID)
{
if (VAM_Lot_ID <= 0) Set_Value ("VAM_Lot_ID", null);
else
Set_Value ("VAM_Lot_ID", VAM_Lot_ID);
}
/** Get Lot.
@return Product Lot Definition */
public int GetVAM_Lot_ID() 
{
Object ii = Get_Value("VAM_Lot_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}


    //------------Anuj 9/6/2016

/** Set VA007_RefAttSetIns.
@param VA007_RefAttSetIns_ID VA007_RefAttSetIns */
public void SetVA007_RefAttSetIns_ID(String VA007_RefAttSetIns_ID) { if (VA007_RefAttSetIns_ID != null && VA007_RefAttSetIns_ID.Length > 30) { log.Warning("Length > 30 - truncated"); VA007_RefAttSetIns_ID = VA007_RefAttSetIns_ID.Substring(0, 30); } Set_Value("VA007_RefAttSetIns_ID", VA007_RefAttSetIns_ID); }/** Get VA007_RefAttSetIns.
@return VA007_RefAttSetIns */
public String GetVA007_RefAttSetIns_ID() { return (String)Get_Value("VA007_RefAttSetIns_ID"); }


    //End







/** Set Serial No.
@param SerNo Product Serial Number */
public void SetSerNo (String SerNo)
{
if (SerNo != null && SerNo.Length > 40)
{
log.Warning("Length > 40 - truncated");
SerNo = SerNo.Substring(0,40);
}
Set_Value ("SerNo", SerNo);
}
/** Get Serial No.
@return Product Serial Number */
public String GetSerNo() 
{
return (String)Get_Value("SerNo");
}
}

}
