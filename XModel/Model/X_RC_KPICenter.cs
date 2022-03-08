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
/** Generated Model for RC_KPICenter
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RC_KPICenter : PO
{
public X_RC_KPICenter (Context ctx, int RC_KPICenter_ID, Trx trxName) : base (ctx, RC_KPICenter_ID, trxName)
{
/** if (RC_KPICenter_ID == 0)
{
SetRC_KPICenter_ID (0);
SetRC_KPIPane_ID (0);
}
 */
}
public X_RC_KPICenter (Ctx ctx, int RC_KPICenter_ID, Trx trxName) : base (ctx, RC_KPICenter_ID, trxName)
{
/** if (RC_KPICenter_ID == 0)
{
SetRC_KPICenter_ID (0);
SetRC_KPIPane_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPICenter (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPICenter (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPICenter (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RC_KPICenter()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634333565125L;
/** Last Updated Timestamp 11/6/2012 6:54:08 PM */
public static long updatedMS = 1352208248336L;
/** AD_Table_ID=1000230 */
public static int Table_ID;
 // =1000230;

/** TableName=RC_KPICenter */
public static String Table_Name="RC_KPICenter";

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
StringBuilder sb = new StringBuilder ("X_RC_KPICenter[").Append(Get_ID()).Append("]");
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
Set_ValueNoCheck ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}

/** Font_Color_ID AD_Reference_ID=266 */
public static int FONT_COLOR_ID_AD_Reference_ID=266;
/** Set Font Color.
@param Font_Color_ID Font Color */
public void SetFont_Color_ID (int Font_Color_ID)
{
if (Font_Color_ID <= 0) Set_Value ("Font_Color_ID", null);
else
Set_Value ("Font_Color_ID", Font_Color_ID);
}
/** Get Font Color.
@return Font Color */
public int GetFont_Color_ID() 
{
Object ii = Get_Value("Font_Color_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set KPI Center.
@param RC_KPICenter_ID KPI Center */
public void SetRC_KPICenter_ID (int RC_KPICenter_ID)
{
if (RC_KPICenter_ID < 1) throw new ArgumentException ("RC_KPICenter_ID is mandatory.");
Set_ValueNoCheck ("RC_KPICenter_ID", RC_KPICenter_ID);
}
/** Get KPI Center.
@return KPI Center */
public int GetRC_KPICenter_ID() 
{
Object ii = Get_Value("RC_KPICenter_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set KPI Pane.
@param RC_KPIPane_ID KPI Pane ID */
public void SetRC_KPIPane_ID (int RC_KPIPane_ID)
{
if (RC_KPIPane_ID < 1) throw new ArgumentException ("RC_KPIPane_ID is mandatory.");
Set_ValueNoCheck ("RC_KPIPane_ID", RC_KPIPane_ID);
}
/** Get KPI Pane.
@return KPI Pane ID */
public int GetRC_KPIPane_ID() 
{
Object ii = Get_Value("RC_KPIPane_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set KPI.
@param RC_KPI_ID KPI */
public void SetRC_KPI_ID (int RC_KPI_ID)
{
if (RC_KPI_ID <= 0) Set_Value ("RC_KPI_ID", null);
else
Set_Value ("RC_KPI_ID", RC_KPI_ID);
}
/** Get KPI.
@return KPI */
public int GetRC_KPI_ID() 
{
Object ii = Get_Value("RC_KPI_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
