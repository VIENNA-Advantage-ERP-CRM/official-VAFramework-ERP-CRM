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
/** Generated Model for AD_ModuleForm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleForm : PO
{
public X_AD_ModuleForm (Context ctx, int AD_ModuleForm_ID, Trx trxName) : base (ctx, AD_ModuleForm_ID, trxName)
{
/** if (AD_ModuleForm_ID == 0)
{
SetAD_ModuleForm_ID (0);
SetAD_ModuleInfo_ID (0);
}
 */
}
public X_AD_ModuleForm (Ctx ctx, int AD_ModuleForm_ID, Trx trxName) : base (ctx, AD_ModuleForm_ID, trxName)
{
/** if (AD_ModuleForm_ID == 0)
{
SetAD_ModuleForm_ID (0);
SetAD_ModuleInfo_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleForm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleForm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleForm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleForm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811914920L;
/** Last Updated Timestamp 6/26/2012 10:26:38 AM */
public static long updatedMS = 1340686598131L;
/** AD_Table_ID=1000058 */
public static int Table_ID;
 // =1000058;

/** TableName=AD_ModuleForm */
public static String Table_Name="AD_ModuleForm";

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
StringBuilder sb = new StringBuilder ("X_AD_ModuleForm[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set AD_ModuleForm_ID.
@param AD_ModuleForm_ID AD_ModuleForm_ID */
public void SetAD_ModuleForm_ID (int AD_ModuleForm_ID)
{
if (AD_ModuleForm_ID < 1) throw new ArgumentException ("AD_ModuleForm_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleForm_ID", AD_ModuleForm_ID);
}
/** Get AD_ModuleForm_ID.
@return AD_ModuleForm_ID */
public int GetAD_ModuleForm_ID() 
{
Object ii = Get_Value("AD_ModuleForm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Module.
@param AD_ModuleInfo_ID Module */
public void SetAD_ModuleInfo_ID (int AD_ModuleInfo_ID)
{
if (AD_ModuleInfo_ID < 1) throw new ArgumentException ("AD_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetAD_ModuleInfo_ID() 
{
Object ii = Get_Value("AD_ModuleInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 200)
{
log.Warning("Length > 200 - truncated");
Description = Description.Substring(0,200);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
}

}
