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
/** Generated Model for VAM_Manufacturer
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_Manufacturer : PO
{
public X_VAM_Manufacturer (Context ctx, int VAM_Manufacturer_ID, Trx trxName) : base (ctx, VAM_Manufacturer_ID, trxName)
{
/** if (VAM_Manufacturer_ID == 0)
{
SetVAM_Manufacturer_ID (0);
SetVAM_Product_ID (0);
}
 */
}
public X_VAM_Manufacturer (Ctx ctx, int VAM_Manufacturer_ID, Trx trxName) : base (ctx, VAM_Manufacturer_ID, trxName)
{
/** if (VAM_Manufacturer_ID == 0)
{
SetVAM_Manufacturer_ID (0);
SetVAM_Product_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Manufacturer (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Manufacturer (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Manufacturer (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_Manufacturer()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27709584990499L;
/** Last Updated Timestamp 3/27/2015 6:04:33 PM */
public static long updatedMS = 1427459673710L;
/** VAF_TableView_ID=1000451 */
public static int Table_ID;
 // =1000451;

/** TableName=VAM_Manufacturer */
public static String Table_Name="VAM_Manufacturer";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAM_Manufacturer[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set Instance.
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
{
if (VAM_PFeature_SetInstance_ID <= 0) Set_Value ("VAM_PFeature_SetInstance_ID", null);
else
Set_Value ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetVAM_PFeature_SetInstance_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAM_Manufacturer_ID.
@param VAM_Manufacturer_ID VAM_Manufacturer_ID */
public void SetVAM_Manufacturer_ID (int VAM_Manufacturer_ID)
{
if (VAM_Manufacturer_ID < 1) throw new ArgumentException ("VAM_Manufacturer_ID is mandatory.");
Set_ValueNoCheck ("VAM_Manufacturer_ID", VAM_Manufacturer_ID);
}
/** Get VAM_Manufacturer_ID.
@return VAM_Manufacturer_ID */
public int GetVAM_Manufacturer_ID() 
{
Object ii = Get_Value("VAM_Manufacturer_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Manufacturer.
@param Manufacturer Manufacturer of the Product */
public void SetManufacturer (String Manufacturer)
{
if (Manufacturer != null && Manufacturer.Length > 250)
{
log.Warning("Length > 250 - truncated");
Manufacturer = Manufacturer.Substring(0,250);
}
Set_Value ("Manufacturer", Manufacturer);
}
/** Get Manufacturer.
@return Manufacturer of the Product */
public String GetManufacturer() 
{
return (String)Get_Value("Manufacturer");
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC (String UPC)
{
if (UPC != null && UPC.Length > 50)
{
log.Warning("Length > 50 - truncated");
UPC = UPC.Substring(0,50);
}
Set_Value ("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC() 
{
return (String)Get_Value("UPC");
}
}

}
