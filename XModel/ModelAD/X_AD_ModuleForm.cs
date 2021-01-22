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
/** Generated Model for VAF_ModuleForm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleForm : PO
{
public X_VAF_ModuleForm (Context ctx, int VAF_ModuleForm_ID, Trx trxName) : base (ctx, VAF_ModuleForm_ID, trxName)
{
/** if (VAF_ModuleForm_ID == 0)
{
SetVAF_ModuleForm_ID (0);
SetVAF_ModuleInfo_ID (0);
}
 */
}
public X_VAF_ModuleForm (Ctx ctx, int VAF_ModuleForm_ID, Trx trxName) : base (ctx, VAF_ModuleForm_ID, trxName)
{
/** if (VAF_ModuleForm_ID == 0)
{
SetVAF_ModuleForm_ID (0);
SetVAF_ModuleInfo_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleForm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleForm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleForm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleForm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811914920L;
/** Last Updated Timestamp 6/26/2012 10:26:38 AM */
public static long updatedMS = 1340686598131L;
/** VAF_TableView_ID=1000058 */
public static int Table_ID;
 // =1000058;

/** TableName=VAF_ModuleForm */
public static String Table_Name="VAF_ModuleForm";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleForm[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param VAF_Page_ID Special Form */
public void SetVAF_Page_ID (int VAF_Page_ID)
{
if (VAF_Page_ID <= 0) Set_Value ("VAF_Page_ID", null);
else
Set_Value ("VAF_Page_ID", VAF_Page_ID);
}
/** Get Special Form.
@return Special Form */
public int GetVAF_Page_ID() 
{
Object ii = Get_Value("VAF_Page_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleForm_ID.
@param VAF_ModuleForm_ID VAF_ModuleForm_ID */
public void SetVAF_ModuleForm_ID (int VAF_ModuleForm_ID)
{
if (VAF_ModuleForm_ID < 1) throw new ArgumentException ("VAF_ModuleForm_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleForm_ID", VAF_ModuleForm_ID);
}
/** Get VAF_ModuleForm_ID.
@return VAF_ModuleForm_ID */
public int GetVAF_ModuleForm_ID() 
{
Object ii = Get_Value("VAF_ModuleForm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Module.
@param VAF_ModuleInfo_ID Module */
public void SetVAF_ModuleInfo_ID (int VAF_ModuleInfo_ID)
{
if (VAF_ModuleInfo_ID < 1) throw new ArgumentException ("VAF_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleInfo_ID", VAF_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetVAF_ModuleInfo_ID() 
{
Object ii = Get_Value("VAF_ModuleInfo_ID");
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
