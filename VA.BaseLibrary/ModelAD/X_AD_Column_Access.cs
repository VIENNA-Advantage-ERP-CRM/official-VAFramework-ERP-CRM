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
/** Generated Model for AD_Column_Access
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Column_Access : PO
{
public X_AD_Column_Access (Context ctx, int AD_Column_Access_ID, Trx trxName) : base (ctx, AD_Column_Access_ID, trxName)
{
/** if (AD_Column_Access_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Role_ID (0);
SetIsExclude (true);	// Y
SetIsReadOnly (false);
}
 */
}
public X_AD_Column_Access (Ctx ctx, int AD_Column_Access_ID, Trx trxName) : base (ctx, AD_Column_Access_ID, trxName)
{
/** if (AD_Column_Access_ID == 0)
{
SetAD_Column_ID (0);
SetAD_Role_ID (0);
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
public X_AD_Column_Access (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Column_Access (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Column_Access (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Column_Access()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361060L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044271L;
/** AD_Table_ID=571 */
public static int Table_ID;
 // =571;

/** TableName=AD_Column_Access */
public static String Table_Name="AD_Column_Access";

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
StringBuilder sb = new StringBuilder ("X_AD_Column_Access[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID < 1) throw new ArgumentException ("AD_Column_ID is mandatory.");
Set_ValueNoCheck ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
