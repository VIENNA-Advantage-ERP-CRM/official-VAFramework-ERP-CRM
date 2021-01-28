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
/** Generated Model for VAF_Group_Form
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Group_Form : PO
{
    public X_VAF_Group_Form(Context ctx, int VAF_Group_Form_ID, Trx trxName)
        : base(ctx, VAF_Group_Form_ID, trxName)
{
/** if (VAF_Group_Form_ID == 0)
{
SetVAF_GroupInfo_ID (0);
SetVAF_Group_Form_ID (0);
}
 */
}
    public X_VAF_Group_Form(Ctx ctx, int VAF_Group_Form_ID, Trx trxName)
        : base(ctx, VAF_Group_Form_ID, trxName)
{
/** if (VAF_Group_Form_ID == 0)
{
SetVAF_GroupInfo_ID (0);
SetVAF_Group_Form_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAF_Group_Form(Context ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAF_Group_Form(Ctx ctx, DataRow rs, Trx trxName)
        : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
    public X_VAF_Group_Form(Ctx ctx, IDataReader dr, Trx trxName)
        : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Group_Form()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27712845016601L;
/** Last Updated Timestamp 5/4/2015 11:38:19 AM */
public static long updatedMS = 1430719699812L;
/** VAF_TableView_ID=1000488 */
public static int Table_ID;
 // =1000488;

/** TableName=VAF_Group_Form */
public static String Table_Name="VAF_Group_Form";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAF_Group_Form[").Append(Get_ID()).Append("]");
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
/** Set VAF_GroupInfo_ID.
@param VAF_GroupInfo_ID VAF_GroupInfo_ID */
public void SetVAF_GroupInfo_ID (int VAF_GroupInfo_ID)
{
if (VAF_GroupInfo_ID < 1) throw new ArgumentException ("VAF_GroupInfo_ID is mandatory.");
Set_ValueNoCheck ("VAF_GroupInfo_ID", VAF_GroupInfo_ID);
}
/** Get VAF_GroupInfo_ID.
@return VAF_GroupInfo_ID */
public int GetVAF_GroupInfo_ID() 
{
Object ii = Get_Value("VAF_GroupInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_Group_Form_ID.
@param VAF_Group_Form_ID VAF_Group_Form_ID */
public void SetVAF_Group_Form_ID (int VAF_Group_Form_ID)
{
if (VAF_Group_Form_ID < 1) throw new ArgumentException ("VAF_Group_Form_ID is mandatory.");
Set_ValueNoCheck ("VAF_Group_Form_ID", VAF_Group_Form_ID);
}
/** Get VAF_Group_Form_ID.
@return VAF_Group_Form_ID */
public int GetVAF_Group_Form_ID() 
{
Object ii = Get_Value("VAF_Group_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
}

}
