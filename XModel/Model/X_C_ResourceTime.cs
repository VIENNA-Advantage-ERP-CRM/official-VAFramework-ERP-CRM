namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
//using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_ResourceTime
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ResourceTime : PO
{
public X_C_ResourceTime (Context ctx, int C_ResourceTime_ID, Trx trxName) : base (ctx, C_ResourceTime_ID, trxName)
{
/** if (C_ResourceTime_ID == 0)
{
SetC_ResourcePeriod_ID (0);
SetC_ResourceTime_ID (0);
}
 */
}
public X_C_ResourceTime (Ctx ctx, int C_ResourceTime_ID, Trx trxName) : base (ctx, C_ResourceTime_ID, trxName)
{
/** if (C_ResourceTime_ID == 0)
{
SetC_ResourcePeriod_ID (0);
SetC_ResourceTime_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourceTime (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourceTime (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ResourceTime (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ResourceTime()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27617312401352L;
/** Last Updated Timestamp 4/23/2012 6:48:04 PM */
public static long updatedMS = 1335187084563L;
/** AD_Table_ID=1000337 */
public static int Table_ID;
 // =1000337;

/** TableName=C_ResourceTime */
public static String Table_Name="C_ResourceTime";

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
StringBuilder sb = new StringBuilder ("X_C_ResourceTime[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set ActualHrs.
@param ActualHrs ActualHrs */
public void SetActualHrs (Decimal? ActualHrs)
{
Set_Value ("ActualHrs", (Decimal?)ActualHrs);
}
/** Get ActualHrs.
@return ActualHrs */
public Decimal GetActualHrs() 
{
Object bd =Get_Value("ActualHrs");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Line.
@param C_OrderLine_ID Order Line */
public void SetC_OrderLine_ID (int C_OrderLine_ID)
{
if (C_OrderLine_ID <= 0) Set_Value ("C_OrderLine_ID", null);
else
Set_Value ("C_OrderLine_ID", C_OrderLine_ID);
}
/** Get Order Line.
@return Order Line */
public int GetC_OrderLine_ID() 
{
Object ii = Get_Value("C_OrderLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);
else
Set_Value ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_ResourcePeriod_ID.
@param C_ResourcePeriod_ID C_ResourcePeriod_ID */
public void SetC_ResourcePeriod_ID (int C_ResourcePeriod_ID)
{
if (C_ResourcePeriod_ID < 1) throw new ArgumentException ("C_ResourcePeriod_ID is mandatory.");
Set_ValueNoCheck ("C_ResourcePeriod_ID", C_ResourcePeriod_ID);
}
/** Get C_ResourcePeriod_ID.
@return C_ResourcePeriod_ID */
public int GetC_ResourcePeriod_ID() 
{
Object ii = Get_Value("C_ResourcePeriod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_ResourceTime_ID.
@param C_ResourceTime_ID C_ResourceTime_ID */
public void SetC_ResourceTime_ID (int C_ResourceTime_ID)
{
if (C_ResourceTime_ID < 1) throw new ArgumentException ("C_ResourceTime_ID is mandatory.");
Set_ValueNoCheck ("C_ResourceTime_ID", C_ResourceTime_ID);
}
/** Get C_ResourceTime_ID.
@return C_ResourceTime_ID */
public int GetC_ResourceTime_ID() 
{
Object ii = Get_Value("C_ResourceTime_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID <= 0) Set_Value ("C_UOM_ID", null);
else
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date.
@param Date1 Date when business is not conducted */
public void SetDate1 (DateTime? Date1)
{
Set_Value ("Date1", (DateTime?)Date1);
}
/** Get Date.
@return Date when business is not conducted */
public DateTime? GetDate1() 
{
return (DateTime?)Get_Value("Date1");
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Planned Quantity.
@param PlannedQty Planned quantity for this project */
public void SetPlannedQty (Decimal? PlannedQty)
{
Set_Value ("PlannedQty", (Decimal?)PlannedQty);
}
/** Get Planned Quantity.
@return Planned quantity for this project */
public Decimal GetPlannedQty() 
{
Object bd =Get_Value("PlannedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
