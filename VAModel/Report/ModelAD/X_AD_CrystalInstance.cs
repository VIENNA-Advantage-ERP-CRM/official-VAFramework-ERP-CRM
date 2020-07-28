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
/** Generated Model for AD_CrystalInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_CrystalInstance : PO
{
public X_AD_CrystalInstance (Context ctx, int AD_CrystalInstance_ID, Trx trxName) : base (ctx, AD_CrystalInstance_ID, trxName)
{
/** if (AD_CrystalInstance_ID == 0)
{
SetAD_CrystalInstance_ID (0);
SetIsProcessing (false);
}
 */
}
public X_AD_CrystalInstance (Ctx ctx, int AD_CrystalInstance_ID, Trx trxName) : base (ctx, AD_CrystalInstance_ID, trxName)
{
/** if (AD_CrystalInstance_ID == 0)
{
SetAD_CrystalInstance_ID (0);
SetIsProcessing (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CrystalInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CrystalInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_CrystalInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_CrystalInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27582899055799L;
/** Last Updated Timestamp 3/22/2011 11:32:19 AM */
public static long updatedMS = 1300773739010L;
/** AD_Table_ID=1000177 */
public static int Table_ID;
 // =1000177;

/** TableName=AD_CrystalInstance */
public static String Table_Name="AD_CrystalInstance";

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
StringBuilder sb = new StringBuilder ("X_AD_CrystalInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_CrystalInstance_ID.
@param AD_CrystalInstance_ID AD_CrystalInstance_ID */
public void SetAD_CrystalInstance_ID (int AD_CrystalInstance_ID)
{
if (AD_CrystalInstance_ID < 1) throw new ArgumentException ("AD_CrystalInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_CrystalInstance_ID", AD_CrystalInstance_ID);
}
/** Get AD_CrystalInstance_ID.
@return AD_CrystalInstance_ID */
public int GetAD_CrystalInstance_ID() 
{
Object ii = Get_Value("AD_CrystalInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID <= 0) Set_Value ("AD_Form_ID", null);
else
Set_Value ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Error Message.
@param ErrorMsg Error Message */
public void SetErrorMsg (String ErrorMsg)
{
if (ErrorMsg != null && ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ErrorMsg = ErrorMsg.Substring(0,2000);
}
Set_Value ("ErrorMsg", ErrorMsg);
}
/** Get Error Message.
@return Error Message */
public String GetErrorMsg() 
{
return (String)Get_Value("ErrorMsg");
}
/** Set Processing.
@param IsProcessing Processing */
public void SetIsProcessing (Boolean IsProcessing)
{
Set_Value ("IsProcessing", IsProcessing);
}
/** Get Processing.
@return Processing */
public Boolean IsProcessing() 
{
Object oo = Get_Value("IsProcessing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
