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
/** Generated Model for AD_Error
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Error : PO
{
public X_AD_Error (Context ctx, int AD_Error_ID, Trx trxName) : base (ctx, AD_Error_ID, trxName)
{
/** if (AD_Error_ID == 0)
{
SetAD_Error_ID (0);
SetName (null);
}
 */
}
public X_AD_Error (Ctx ctx, int AD_Error_ID, Trx trxName) : base (ctx, AD_Error_ID, trxName)
{
/** if (AD_Error_ID == 0)
{
SetAD_Error_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Error (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Error (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Error (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Error()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361263L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044474L;
/** AD_Table_ID=380 */
public static int Table_ID;
 // =380;

/** TableName=AD_Error */
public static String Table_Name="AD_Error";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_AD_Error[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Error.
@param AD_Error_ID Error */
public void SetAD_Error_ID (int AD_Error_ID)
{
if (AD_Error_ID < 1) throw new ArgumentException ("AD_Error_ID is mandatory.");
Set_ValueNoCheck ("AD_Error_ID", AD_Error_ID);
}
/** Get Error.
@return Error */
public int GetAD_Error_ID() 
{
Object ii = Get_Value("AD_Error_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set Code.
@param Code Code to execute or to validate */
public void SetCode (String Code)
{
if (Code != null && Code.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Code = Code.Substring(0,2000);
}
Set_Value ("Code", Code);
}
/** Get Code.
@return Code to execute or to validate */
public String GetCode() 
{
return (String)Get_Value("Code");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
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
}

}
