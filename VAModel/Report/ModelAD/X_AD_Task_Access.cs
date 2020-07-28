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
/** Generated Model for AD_Task_Access
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Task_Access : PO
{
public X_AD_Task_Access (Context ctx, int AD_Task_Access_ID, Trx trxName) : base (ctx, AD_Task_Access_ID, trxName)
{
/** if (AD_Task_Access_ID == 0)
{
SetAD_Role_ID (0);
SetAD_Task_ID (0);
SetIsReadWrite (false);
}
 */
}
public X_AD_Task_Access (Ctx ctx, int AD_Task_Access_ID, Trx trxName) : base (ctx, AD_Task_Access_ID, trxName)
{
/** if (AD_Task_Access_ID == 0)
{
SetAD_Role_ID (0);
SetAD_Task_ID (0);
SetIsReadWrite (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task_Access (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task_Access (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task_Access (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Task_Access()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364523L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047734L;
/** AD_Table_ID=199 */
public static int Table_ID;
 // =199;

/** TableName=AD_Task_Access */
public static String Table_Name="AD_Task_Access";

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
StringBuilder sb = new StringBuilder ("X_AD_Task_Access[").Append(Get_ID()).Append("]");
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
/** Set OS Task.
@param AD_Task_ID Operation System Task */
public void SetAD_Task_ID (int AD_Task_ID)
{
if (AD_Task_ID < 1) throw new ArgumentException ("AD_Task_ID is mandatory.");
Set_ValueNoCheck ("AD_Task_ID", AD_Task_ID);
}
/** Get OS Task.
@return Operation System Task */
public int GetAD_Task_ID() 
{
Object ii = Get_Value("AD_Task_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Read Write.
@param IsReadWrite Field is read / write */
public void SetIsReadWrite (Boolean IsReadWrite)
{
Set_Value ("IsReadWrite", IsReadWrite);
}
/** Get Read Write.
@return Field is read / write */
public Boolean IsReadWrite() 
{
Object oo = Get_Value("IsReadWrite");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
