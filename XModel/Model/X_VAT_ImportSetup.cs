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
/** Generated Model for VAT_ImportSetup
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_ImportSetup : PO
{
public X_VAT_ImportSetup (Context ctx, int VAT_ImportSetup_ID, Trx trxName) : base (ctx, VAT_ImportSetup_ID, trxName)
{
/** if (VAT_ImportSetup_ID == 0)
{
SetVAT_ImportSetup_ID (0);
}
 */
}
public X_VAT_ImportSetup (Ctx ctx, int VAT_ImportSetup_ID, Trx trxName) : base (ctx, VAT_ImportSetup_ID, trxName)
{
/** if (VAT_ImportSetup_ID == 0)
{
SetVAT_ImportSetup_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_ImportSetup (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_ImportSetup (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_ImportSetup (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_ImportSetup()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384208L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067419L;
/** VAF_TableView_ID=992 */
public static int Table_ID;
 // =992;

/** TableName=VAT_ImportSetup */
public static String Table_Name="VAT_ImportSetup";

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
StringBuilder sb = new StringBuilder ("X_VAT_ImportSetup[").Append(Get_ID()).Append("]");
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
@param VAT_ImportSetup_ID Import Format */
public void SetVAT_ImportSetup_ID (int VAT_ImportSetup_ID)
{
if (VAT_ImportSetup_ID < 1) throw new ArgumentException ("VAT_ImportSetup_ID is mandatory.");
Set_ValueNoCheck ("VAT_ImportSetup_ID", VAT_ImportSetup_ID);
}
/** Get Import Format.
@return Import Format */
public int GetVAT_ImportSetup_ID() 
{
Object ii = Get_Value("VAT_ImportSetup_ID");
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
