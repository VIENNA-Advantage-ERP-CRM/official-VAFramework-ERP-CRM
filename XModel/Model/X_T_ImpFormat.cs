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
/** Generated Model for T_ImpFormat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_ImpFormat : PO
{
public X_T_ImpFormat (Context ctx, int T_ImpFormat_ID, Trx trxName) : base (ctx, T_ImpFormat_ID, trxName)
{
/** if (T_ImpFormat_ID == 0)
{
SetT_ImpFormat_ID (0);
}
 */
}
public X_T_ImpFormat (Ctx ctx, int T_ImpFormat_ID, Trx trxName) : base (ctx, T_ImpFormat_ID, trxName)
{
/** if (T_ImpFormat_ID == 0)
{
SetT_ImpFormat_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ImpFormat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ImpFormat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ImpFormat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_ImpFormat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384208L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067419L;
/** AD_Table_ID=992 */
public static int Table_ID;
 // =992;

/** TableName=T_ImpFormat */
public static String Table_Name="T_ImpFormat";

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
StringBuilder sb = new StringBuilder ("X_T_ImpFormat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set BinaryData.
@param BinaryData Binary Data */
public void SetBinaryData (Byte[] BinaryData)
{
Set_Value ("BinaryData", BinaryData);
}
/** Get BinaryData.
@return Binary Data */
public Byte[] GetBinaryData() 
{
return (Byte[])Get_Value("BinaryData");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 120)
{
log.Warning("Length > 120 - truncated");
Name = Name.Substring(0,120);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Import Format.
@param T_ImpFormat_ID Import Format */
public void SetT_ImpFormat_ID (int T_ImpFormat_ID)
{
if (T_ImpFormat_ID < 1) throw new ArgumentException ("T_ImpFormat_ID is mandatory.");
Set_ValueNoCheck ("T_ImpFormat_ID", T_ImpFormat_ID);
}
/** Get Import Format.
@return Import Format */
public int GetT_ImpFormat_ID() 
{
Object ii = Get_Value("T_ImpFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Session.
@param WebSession Web Session ID */
public void SetWebSession (String WebSession)
{
if (WebSession != null && WebSession.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebSession = WebSession.Substring(0,2000);
}
Set_Value ("WebSession", WebSession);
}
/** Get Web Session.
@return Web Session ID */
public String GetWebSession() 
{
return (String)Get_Value("WebSession");
}
}

}
