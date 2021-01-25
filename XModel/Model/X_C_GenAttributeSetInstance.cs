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
/** Generated Model for VAB_GenFeatureSetInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_GenFeatureSetInstance : PO
{
public X_VAB_GenFeatureSetInstance (Context ctx, int VAB_GenFeatureSetInstance_ID, Trx trxName) : base (ctx, VAB_GenFeatureSetInstance_ID, trxName)
{
/** if (VAB_GenFeatureSetInstance_ID == 0)
{
SetVAB_GenFeatureSetInstance_ID (0);
SetVAB_GenFeatureSet_ID (0);
}
 */
}
public X_VAB_GenFeatureSetInstance (Ctx ctx, int VAB_GenFeatureSetInstance_ID, Trx trxName) : base (ctx, VAB_GenFeatureSetInstance_ID, trxName)
{
/** if (VAB_GenFeatureSetInstance_ID == 0)
{
SetVAB_GenFeatureSetInstance_ID (0);
SetVAB_GenFeatureSet_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_GenFeatureSetInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169111866L;
/** Last Updated Timestamp 11/21/2013 7:53:15 PM */
public static long updatedMS = 1385043795077L;
/** VAF_TableView_ID=1000423 */
public static int Table_ID;
 // =1000423;

/** TableName=VAB_GenFeatureSetInstance */
public static String Table_Name="VAB_GenFeatureSetInstance";

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
StringBuilder sb = new StringBuilder ("X_VAB_GenFeatureSetInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_GenFeatureSetInstance_ID.
@param VAB_GenFeatureSetInstance_ID VAB_GenFeatureSetInstance_ID */
public void SetVAB_GenFeatureSetInstance_ID (int VAB_GenFeatureSetInstance_ID)
{
if (VAB_GenFeatureSetInstance_ID < 1) throw new ArgumentException ("VAB_GenFeatureSetInstance_ID is mandatory.");
Set_ValueNoCheck ("VAB_GenFeatureSetInstance_ID", VAB_GenFeatureSetInstance_ID);
}
/** Get VAB_GenFeatureSetInstance_ID.
@return VAB_GenFeatureSetInstance_ID */
public int GetVAB_GenFeatureSetInstance_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeatureSet_ID.
@param VAB_GenFeatureSet_ID VAB_GenFeatureSet_ID */
public void SetVAB_GenFeatureSet_ID (int VAB_GenFeatureSet_ID)
{
if (VAB_GenFeatureSet_ID < 1) throw new ArgumentException ("VAB_GenFeatureSet_ID is mandatory.");
Set_Value ("VAB_GenFeatureSet_ID", VAB_GenFeatureSet_ID);
}
/** Get VAB_GenFeatureSet_ID.
@return VAB_GenFeatureSet_ID */
public int GetVAB_GenFeatureSet_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSet_ID");
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
/** Set Lot.
@param M_Lot_ID Product Lot Definition */
public void SetM_Lot_ID (int M_Lot_ID)
{
if (M_Lot_ID <= 0) Set_Value ("M_Lot_ID", null);
else
Set_Value ("M_Lot_ID", M_Lot_ID);
}
/** Get Lot.
@return Product Lot Definition */
public int GetM_Lot_ID() 
{
Object ii = Get_Value("M_Lot_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Serial No.
@param SerNo Product Serial Number  */
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
@return Product Serial Number  */
public String GetSerNo() 
{
return (String)Get_Value("SerNo");
}
}

}
