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
/** Generated Model for RC_TabPanels
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RC_TabPanels : PO
{
public X_RC_TabPanels (Context ctx, int RC_TabPanels_ID, Trx trxName) : base (ctx, RC_TabPanels_ID, trxName)
{
/** if (RC_TabPanels_ID == 0)
{
SetRC_RoleCenterTab_ID (0);
SetRC_TabPanels_ID (0);
}
 */
}
public X_RC_TabPanels (Ctx ctx, int RC_TabPanels_ID, Trx trxName) : base (ctx, RC_TabPanels_ID, trxName)
{
/** if (RC_TabPanels_ID == 0)
{
SetRC_RoleCenterTab_ID (0);
SetRC_TabPanels_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_TabPanels (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_TabPanels (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_TabPanels (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RC_TabPanels()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634578416249L;
/** Last Updated Timestamp 11/9/2012 2:55:00 PM */
public static long updatedMS = 1352453099460L;
/** AD_Table_ID=1000234 */
public static int Table_ID;
 // =1000234;

/** TableName=RC_TabPanels */
public static String Table_Name="RC_TabPanels";

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
StringBuilder sb = new StringBuilder ("X_RC_TabPanels[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User Query.
@param AD_UserQuery_ID Saved User Query */
public void SetAD_UserQuery_ID (int AD_UserQuery_ID)
{
if (AD_UserQuery_ID <= 0) Set_Value ("AD_UserQuery_ID", null);
else
Set_Value ("AD_UserQuery_ID", AD_UserQuery_ID);
}
/** Get User Query.
@return Saved User Query */
public int GetAD_UserQuery_ID() 
{
Object ii = Get_Value("AD_UserQuery_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Colspan.
@param Colspan Colspan */
public void SetColspan (int Colspan)
{
Set_Value ("Colspan", Colspan);
}
/** Get Colspan.
@return Colspan */
public int GetColspan() 
{
Object ii = Get_Value("Colspan");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Role Center Tab.
@param RC_RoleCenterTab_ID Role Center Tab ID */
public void SetRC_RoleCenterTab_ID (int RC_RoleCenterTab_ID)
{
if (RC_RoleCenterTab_ID < 1) throw new ArgumentException ("RC_RoleCenterTab_ID is mandatory.");
Set_ValueNoCheck ("RC_RoleCenterTab_ID", RC_RoleCenterTab_ID);
}
/** Get Role Center Tab.
@return Role Center Tab ID */
public int GetRC_RoleCenterTab_ID() 
{
Object ii = Get_Value("RC_RoleCenterTab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tab Panel.
@param RC_TabPanels_ID Tab Panel */
public void SetRC_TabPanels_ID (int RC_TabPanels_ID)
{
if (RC_TabPanels_ID < 1) throw new ArgumentException ("RC_TabPanels_ID is mandatory.");
Set_ValueNoCheck ("RC_TabPanels_ID", RC_TabPanels_ID);
}
/** Get Tab Panel.
@return Tab Panel */
public int GetRC_TabPanels_ID() 
{
Object ii = Get_Value("RC_TabPanels_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RecordID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get RecordID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** RoleCenterPanels AD_Reference_ID=1000092 */
public static int ROLECENTERPANELS_AD_Reference_ID=1000092;
/** View = 1 */
public static String ROLECENTERPANELS_View = "1";
/** Chart = 2 */
public static String ROLECENTERPANELS_Chart = "2";
/** KPI = 3 */
public static String ROLECENTERPANELS_KPI = "3";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRoleCenterPanelsValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3");
}
/** Set Role Center Panels.
@param RoleCenterPanels Role Center Panels */
public void SetRoleCenterPanels (String RoleCenterPanels)
{
if (!IsRoleCenterPanelsValid(RoleCenterPanels))
throw new ArgumentException ("RoleCenterPanels Invalid value - " + RoleCenterPanels + " - Reference_ID=1000092 - 1 - 2 - 3");
if (RoleCenterPanels != null && RoleCenterPanels.Length > 1)
{
log.Warning("Length > 1 - truncated");
RoleCenterPanels = RoleCenterPanels.Substring(0,1);
}
Set_Value ("RoleCenterPanels", RoleCenterPanels);
}
/** Get Role Center Panels.
@return Role Center Panels */
public String GetRoleCenterPanels() 
{
return (String)Get_Value("RoleCenterPanels");
}
/** Set Rowspan.
@param Rowspan Rowspan  */
public void SetRowspan (int Rowspan)
{
Set_Value ("Rowspan", Rowspan);
}
/** Get Rowspan.
@return Rowspan  */
public int GetRowspan() 
{
Object ii = Get_Value("Rowspan");
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
