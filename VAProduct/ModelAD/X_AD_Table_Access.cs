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
/** Generated Model for AD_Table_Access
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Table_Access : PO
{
public X_AD_Table_Access (Context ctx, int AD_Table_Access_ID, Trx trxName) : base (ctx, AD_Table_Access_ID, trxName)
{
/** if (AD_Table_Access_ID == 0)
{
SetAD_Role_ID (0);
SetAD_Table_ID (0);
SetAccessTypeRule (null);	// A
SetIsCanExport (false);
SetIsCanReport (false);
SetIsExclude (true);	// Y
SetIsReadOnly (false);
}
 */
}
public X_AD_Table_Access (Ctx ctx, int AD_Table_Access_ID, Trx trxName) : base (ctx, AD_Table_Access_ID, trxName)
{
/** if (AD_Table_Access_ID == 0)
{
SetAD_Role_ID (0);
SetAD_Table_ID (0);
SetAccessTypeRule (null);	// A
SetIsCanExport (false);
SetIsCanReport (false);
SetIsExclude (true);	// Y
SetIsReadOnly (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Table_Access (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Table_Access (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Table_Access (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Table_Access()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364398L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047609L;
/** AD_Table_ID=565 */
public static int Table_ID;
 // =565;

/** TableName=AD_Table_Access */
public static String Table_Name="AD_Table_Access";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_AD_Table_Access[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID < 0) throw new ArgumentException ("AD_Role_ID is mandatory.");
Set_ValueNoCheck ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Role_ID().ToString());
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_ValueNoCheck ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AccessTypeRule AD_Reference_ID=293 */
public static int ACCESSTYPERULE_AD_Reference_ID=293;
/** Accessing = A */
public static String ACCESSTYPERULE_Accessing = "A";
/** Exporting = E */
public static String ACCESSTYPERULE_Exporting = "E";
/** Reporting = R */
public static String ACCESSTYPERULE_Reporting = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccessTypeRuleValid (String test)
{
return test.Equals("A") || test.Equals("E") || test.Equals("R");
}
/** Set Access Type.
@param AccessTypeRule The type of access for this rule */
public void SetAccessTypeRule (String AccessTypeRule)
{
if (AccessTypeRule == null) throw new ArgumentException ("AccessTypeRule is mandatory");
if (!IsAccessTypeRuleValid(AccessTypeRule))
throw new ArgumentException ("AccessTypeRule Invalid value - " + AccessTypeRule + " - Reference_ID=293 - A - E - R");
if (AccessTypeRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccessTypeRule = AccessTypeRule.Substring(0,1);
}
Set_ValueNoCheck ("AccessTypeRule", AccessTypeRule);
}
/** Get Access Type.
@return The type of access for this rule */
public String GetAccessTypeRule() 
{
return (String)Get_Value("AccessTypeRule");
}
/** Set Can Export.
@param IsCanExport Users with this role can export data */
public void SetIsCanExport (Boolean IsCanExport)
{
Set_Value ("IsCanExport", IsCanExport);
}
/** Get Can Export.
@return Users with this role can export data */
public Boolean IsCanExport() 
{
Object oo = Get_Value("IsCanExport");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Can Report.
@param IsCanReport Users with this role can create reports */
public void SetIsCanReport (Boolean IsCanReport)
{
Set_Value ("IsCanReport", IsCanReport);
}
/** Get Can Report.
@return Users with this role can create reports */
public Boolean IsCanReport() 
{
Object oo = Get_Value("IsCanReport");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Exclude.
@param IsExclude Exclude access to the data - if not selected Include access to the data */
public void SetIsExclude (Boolean IsExclude)
{
Set_Value ("IsExclude", IsExclude);
}
/** Get Exclude.
@return Exclude access to the data - if not selected Include access to the data */
public Boolean IsExclude() 
{
Object oo = Get_Value("IsExclude");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (Boolean IsReadOnly)
{
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public Boolean IsReadOnly() 
{
Object oo = Get_Value("IsReadOnly");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
