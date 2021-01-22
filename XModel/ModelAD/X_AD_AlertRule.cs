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
/** Generated Model for VAF_AlertSetting
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AlertSetting : PO
{
public X_VAF_AlertSetting (Context ctx, int VAF_AlertSetting_ID, Trx trxName) : base (ctx, VAF_AlertSetting_ID, trxName)
{
/** if (VAF_AlertSetting_ID == 0)
{
SetVAF_AlertSetting_ID (0);
SetVAF_Alert_ID (0);
SetFromClause (null);
SetIsValid (true);	// Y
SetName (null);
SetSelectClause (null);
}
 */
}
public X_VAF_AlertSetting (Ctx ctx, int VAF_AlertSetting_ID, Trx trxName) : base (ctx, VAF_AlertSetting_ID, trxName)
{
/** if (VAF_AlertSetting_ID == 0)
{
SetVAF_AlertSetting_ID (0);
SetVAF_Alert_ID (0);
SetFromClause (null);
SetIsValid (true);	// Y
SetName (null);
SetSelectClause (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertSetting (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertSetting (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertSetting (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AlertSetting()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360401L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043612L;
/** VAF_TableView_ID=593 */
public static int Table_ID;
 // =593;

/** TableName=VAF_AlertSetting */
public static String Table_Name="VAF_AlertSetting";

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
StringBuilder sb = new StringBuilder ("X_VAF_AlertSetting[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Alert Rule.
@param VAF_AlertSetting_ID Definition of the alert element */
public void SetVAF_AlertSetting_ID (int VAF_AlertSetting_ID)
{
if (VAF_AlertSetting_ID < 1) throw new ArgumentException ("VAF_AlertSetting_ID is mandatory.");
Set_ValueNoCheck ("VAF_AlertSetting_ID", VAF_AlertSetting_ID);
}
/** Get Alert Rule.
@return Definition of the alert element */
public int GetVAF_AlertSetting_ID() 
{
Object ii = Get_Value("VAF_AlertSetting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alert.
@param VAF_Alert_ID Vienna Alert */
public void SetVAF_Alert_ID (int VAF_Alert_ID)
{
if (VAF_Alert_ID < 1) throw new ArgumentException ("VAF_Alert_ID is mandatory.");
Set_ValueNoCheck ("VAF_Alert_ID", VAF_Alert_ID);
}
/** Get Alert.
@return Vienna Alert */
public int GetVAF_Alert_ID() 
{
Object ii = Get_Value("VAF_Alert_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Error Msg.
@param ErrorMsg Error Msg */
public void SetErrorMsg (String ErrorMsg)
{
if (ErrorMsg != null && ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ErrorMsg = ErrorMsg.Substring(0,2000);
}
Set_Value ("ErrorMsg", ErrorMsg);
}
/** Get Error Msg.
@return Error Msg */
public String GetErrorMsg() 
{
return (String)Get_Value("ErrorMsg");
}
/** Set Sql FROM.
@param FromClause SQL FROM clause */
public void SetFromClause (String FromClause)
{
if (FromClause == null) throw new ArgumentException ("FromClause is mandatory.");
if (FromClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
FromClause = FromClause.Substring(0,2000);
}
Set_Value ("FromClause", FromClause);
}
/** Get Sql FROM.
@return SQL FROM clause */
public String GetFromClause() 
{
return (String)Get_Value("FromClause");
}
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Other SQL Clause.
@param OtherClause Other SQL Clause */
public void SetOtherClause (String OtherClause)
{
if (OtherClause != null && OtherClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OtherClause = OtherClause.Substring(0,2000);
}
Set_Value ("OtherClause", OtherClause);
}
/** Get Other SQL Clause.
@return Other SQL Clause */
public String GetOtherClause() 
{
return (String)Get_Value("OtherClause");
}
/** Set Post Processing.
@param PostProcessing Process SQL after executing the query */
public void SetPostProcessing (String PostProcessing)
{
if (PostProcessing != null && PostProcessing.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
PostProcessing = PostProcessing.Substring(0,2000);
}
Set_Value ("PostProcessing", PostProcessing);
}
/** Get Post Processing.
@return Process SQL after executing the query */
public String GetPostProcessing() 
{
return (String)Get_Value("PostProcessing");
}
/** Set Pre Processing.
@param PreProcessing Process SQL before executing the query */
public void SetPreProcessing (String PreProcessing)
{
if (PreProcessing != null && PreProcessing.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
PreProcessing = PreProcessing.Substring(0,2000);
}
Set_Value ("PreProcessing", PreProcessing);
}
/** Get Pre Processing.
@return Process SQL before executing the query */
public String GetPreProcessing() 
{
return (String)Get_Value("PreProcessing");
}
/** Set Sql SELECT.
@param SelectClause SQL SELECT clause */
public void SetSelectClause (String SelectClause)
{
if (SelectClause == null) throw new ArgumentException ("SelectClause is mandatory.");
if (SelectClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
SelectClause = SelectClause.Substring(0,2000);
}
Set_Value ("SelectClause", SelectClause);
}
/** Get Sql SELECT.
@return SQL SELECT clause */
public String GetSelectClause() 
{
return (String)Get_Value("SelectClause");
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause != null && WhereClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WhereClause = WhereClause.Substring(0,2000);
}
Set_Value ("WhereClause", WhereClause);
}
/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() 
{
return (String)Get_Value("WhereClause");
}
}

}
