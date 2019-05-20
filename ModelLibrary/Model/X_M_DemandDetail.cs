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
/** Generated Model for M_DemandDetail
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DemandDetail : PO
{
public X_M_DemandDetail (Context ctx, int M_DemandDetail_ID, Trx trxName) : base (ctx, M_DemandDetail_ID, trxName)
{
/** if (M_DemandDetail_ID == 0)
{
SetM_DemandDetail_ID (0);
SetM_DemandLine_ID (0);
}
 */
}
public X_M_DemandDetail (Ctx ctx, int M_DemandDetail_ID, Trx trxName) : base (ctx, M_DemandDetail_ID, trxName)
{
/** if (M_DemandDetail_ID == 0)
{
SetM_DemandDetail_ID (0);
SetM_DemandLine_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DemandDetail (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DemandDetail (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DemandDetail (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DemandDetail()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379036L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062247L;
/** AD_Table_ID=721 */
public static int Table_ID;
 // =721;

/** TableName=M_DemandDetail */
public static String Table_Name="M_DemandDetail";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_M_DemandDetail[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Demand Detail.
@param M_DemandDetail_ID Material Demand Line Source Detail */
public void SetM_DemandDetail_ID (int M_DemandDetail_ID)
{
if (M_DemandDetail_ID < 1) throw new ArgumentException ("M_DemandDetail_ID is mandatory.");
Set_ValueNoCheck ("M_DemandDetail_ID", M_DemandDetail_ID);
}
/** Get Demand Detail.
@return Material Demand Line Source Detail */
public int GetM_DemandDetail_ID() 
{
Object ii = Get_Value("M_DemandDetail_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_DemandDetail_ID().ToString());
}
/** Set Demand Line.
@param M_DemandLine_ID Material Demand Line */
public void SetM_DemandLine_ID (int M_DemandLine_ID)
{
if (M_DemandLine_ID < 1) throw new ArgumentException ("M_DemandLine_ID is mandatory.");
Set_ValueNoCheck ("M_DemandLine_ID", M_DemandLine_ID);
}
/** Get Demand Line.
@return Material Demand Line */
public int GetM_DemandLine_ID() 
{
Object ii = Get_Value("M_DemandLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Forecast Line.
@param M_ForecastLine_ID Forecast Line */
public void SetM_ForecastLine_ID (int M_ForecastLine_ID)
{
if (M_ForecastLine_ID <= 0) Set_Value ("M_ForecastLine_ID", null);
else
Set_Value ("M_ForecastLine_ID", M_ForecastLine_ID);
}
/** Get Forecast Line.
@return Forecast Line */
public int GetM_ForecastLine_ID() 
{
Object ii = Get_Value("M_ForecastLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Requisition Line.
@param M_RequisitionLine_ID Material Requisition Line */
public void SetM_RequisitionLine_ID (int M_RequisitionLine_ID)
{
if (M_RequisitionLine_ID <= 0) Set_Value ("M_RequisitionLine_ID", null);
else
Set_Value ("M_RequisitionLine_ID", M_RequisitionLine_ID);
}
/** Get Requisition Line.
@return Material Requisition Line */
public int GetM_RequisitionLine_ID() 
{
Object ii = Get_Value("M_RequisitionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
