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
/** Generated Model for C_GenAttributeSetInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeSetInstance : PO
{
public X_C_GenAttributeSetInstance (Context ctx, int C_GenAttributeSetInstance_ID, Trx trxName) : base (ctx, C_GenAttributeSetInstance_ID, trxName)
{
/** if (C_GenAttributeSetInstance_ID == 0)
{
SetC_GenAttributeSetInstance_ID (0);
SetC_GenAttributeSet_ID (0);
}
 */
}
public X_C_GenAttributeSetInstance (Ctx ctx, int C_GenAttributeSetInstance_ID, Trx trxName) : base (ctx, C_GenAttributeSetInstance_ID, trxName)
{
/** if (C_GenAttributeSetInstance_ID == 0)
{
SetC_GenAttributeSetInstance_ID (0);
SetC_GenAttributeSet_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSetInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeSetInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169111866L;
/** Last Updated Timestamp 11/21/2013 7:53:15 PM */
public static long updatedMS = 1385043795077L;
/** AD_Table_ID=1000423 */
public static int Table_ID;
 // =1000423;

/** TableName=C_GenAttributeSetInstance */
public static String Table_Name="C_GenAttributeSetInstance";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeSetInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_GenAttributeSetInstance_ID.
@param C_GenAttributeSetInstance_ID C_GenAttributeSetInstance_ID */
public void SetC_GenAttributeSetInstance_ID (int C_GenAttributeSetInstance_ID)
{
if (C_GenAttributeSetInstance_ID < 1) throw new ArgumentException ("C_GenAttributeSetInstance_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeSetInstance_ID", C_GenAttributeSetInstance_ID);
}
/** Get C_GenAttributeSetInstance_ID.
@return C_GenAttributeSetInstance_ID */
public int GetC_GenAttributeSetInstance_ID() 
{
Object ii = Get_Value("C_GenAttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_GenAttributeSet_ID.
@param C_GenAttributeSet_ID C_GenAttributeSet_ID */
public void SetC_GenAttributeSet_ID (int C_GenAttributeSet_ID)
{
if (C_GenAttributeSet_ID < 1) throw new ArgumentException ("C_GenAttributeSet_ID is mandatory.");
Set_Value ("C_GenAttributeSet_ID", C_GenAttributeSet_ID);
}
/** Get C_GenAttributeSet_ID.
@return C_GenAttributeSet_ID */
public int GetC_GenAttributeSet_ID() 
{
Object ii = Get_Value("C_GenAttributeSet_ID");
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
