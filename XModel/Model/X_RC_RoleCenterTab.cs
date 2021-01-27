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
/** Generated Model for VARC_RoleCenterTab
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VARC_RoleCenterTab : PO
{
public X_VARC_RoleCenterTab (Context ctx, int VARC_RoleCenterTab_ID, Trx trxName) : base (ctx, VARC_RoleCenterTab_ID, trxName)
{
/** if (VARC_RoleCenterTab_ID == 0)
{
SetVARC_RoleCenterManager_ID (0);
SetVARC_RoleCenterTab_ID (0);
}
 */
}
public X_VARC_RoleCenterTab (Ctx ctx, int VARC_RoleCenterTab_ID, Trx trxName) : base (ctx, VARC_RoleCenterTab_ID, trxName)
{
/** if (VARC_RoleCenterTab_ID == 0)
{
SetVARC_RoleCenterManager_ID (0);
SetVARC_RoleCenterTab_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VARC_RoleCenterTab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VARC_RoleCenterTab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VARC_RoleCenterTab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VARC_RoleCenterTab()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634578404144L;
/** Last Updated Timestamp 11/9/2012 2:54:47 PM */
public static long updatedMS = 1352453087355L;
/** VAF_TableView_ID=1000233 */
public static int Table_ID;
 // =1000233;

/** TableName=VARC_RoleCenterTab */
public static String Table_Name="VARC_RoleCenterTab";

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
StringBuilder sb = new StringBuilder ("X_VARC_RoleCenterTab[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Image.
@param VAF_Image_ID Image or Icon */
public void SetVAF_Image_ID (int VAF_Image_ID)
{
if (VAF_Image_ID <= 0) Set_Value ("VAF_Image_ID", null);
else
Set_Value ("VAF_Image_ID", VAF_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetVAF_Image_ID() 
{
Object ii = Get_Value("VAF_Image_ID");
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


   
    
    /** Set IsPosTab. 
@param VAPOS_IsPosTab IsPosTab */
public void SetVAPOS_IsPosTab (Boolean VAPOS_IsPosTab){Set_Value ("VAPOS_IsPosTab", VAPOS_IsPosTab);}/** Get IsPosTab.
@return IsPosTab */
public Boolean IsVAPOS_IsPosTab() {Object oo = Get_Value("VAPOS_IsPosTab");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}



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
/** Set Role Center Manager.
@param VARC_RoleCenterManager_ID Role Center Manager ID */
public void SetVARC_RoleCenterManager_ID (int VARC_RoleCenterManager_ID)
{
if (VARC_RoleCenterManager_ID < 1) throw new ArgumentException ("VARC_RoleCenterManager_ID is mandatory.");
Set_ValueNoCheck ("VARC_RoleCenterManager_ID", VARC_RoleCenterManager_ID);
}
/** Get Role Center Manager.
@return Role Center Manager ID */
public int GetVARC_RoleCenterManager_ID() 
{
Object ii = Get_Value("VARC_RoleCenterManager_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RoleCenter Tab.
@param VARC_RoleCenterTab_ID RoleCenter Tab */
public void SetVARC_RoleCenterTab_ID (int VARC_RoleCenterTab_ID)
{
if (VARC_RoleCenterTab_ID < 1) throw new ArgumentException ("VARC_RoleCenterTab_ID is mandatory.");
Set_ValueNoCheck ("VARC_RoleCenterTab_ID", VARC_RoleCenterTab_ID);
}
/** Get RoleCenter Tab.
@return RoleCenter Tab */
public int GetVARC_RoleCenterTab_ID() 
{
Object ii = Get_Value("VARC_RoleCenterTab_ID");
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
