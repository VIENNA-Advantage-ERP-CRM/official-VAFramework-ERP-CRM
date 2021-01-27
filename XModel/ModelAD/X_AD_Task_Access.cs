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
/** Generated Model for VAF_Task_Rights
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Task_Rights : PO
{
public X_VAF_Task_Rights (Context ctx, int VAF_Task_Rights_ID, Trx trxName) : base (ctx, VAF_Task_Rights_ID, trxName)
{
/** if (VAF_Task_Rights_ID == 0)
{
SetVAF_Role_ID (0);
SetVAF_Task_ID (0);
SetIsReadWrite (false);
}
 */
}
public X_VAF_Task_Rights (Ctx ctx, int VAF_Task_Rights_ID, Trx trxName) : base (ctx, VAF_Task_Rights_ID, trxName)
{
/** if (VAF_Task_Rights_ID == 0)
{
SetVAF_Role_ID (0);
SetVAF_Task_ID (0);
SetIsReadWrite (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Task_Rights (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Task_Rights (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Task_Rights (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Task_Rights()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364523L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047734L;
/** VAF_TableView_ID=199 */
public static int Table_ID;
 // =199;

/** TableName=VAF_Task_Rights */
public static String Table_Name="VAF_Task_Rights";

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
StringBuilder sb = new StringBuilder ("X_VAF_Task_Rights[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID < 0) throw new ArgumentException ("VAF_Role_ID is mandatory.");
Set_ValueNoCheck ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_Role_ID().ToString());
}
/** Set OS Task.
@param VAF_Task_ID Operation System Task */
public void SetVAF_Task_ID (int VAF_Task_ID)
{
if (VAF_Task_ID < 1) throw new ArgumentException ("VAF_Task_ID is mandatory.");
Set_ValueNoCheck ("VAF_Task_ID", VAF_Task_ID);
}
/** Get OS Task.
@return Operation System Task */
public int GetVAF_Task_ID() 
{
Object ii = Get_Value("VAF_Task_ID");
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
