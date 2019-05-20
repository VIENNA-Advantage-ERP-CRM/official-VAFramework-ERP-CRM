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
/** Generated Model for RC_ActionView
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RC_ActionView : PO
{
public X_RC_ActionView (Context ctx, int RC_ActionView_ID, Trx trxName) : base (ctx, RC_ActionView_ID, trxName)
{
/** if (RC_ActionView_ID == 0)
{
SetRC_ActionView_ID (0);
}
 */
}
public X_RC_ActionView (Ctx ctx, int RC_ActionView_ID, Trx trxName) : base (ctx, RC_ActionView_ID, trxName)
{
/** if (RC_ActionView_ID == 0)
{
SetRC_ActionView_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_ActionView (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_ActionView (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_ActionView (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RC_ActionView()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27590165827889L;
/** Last Updated Timestamp 6/14/2011 2:05:11 PM */
public static long updatedMS = 1308040511100L;
/** AD_Table_ID=1000189 */
public static int Table_ID;
 // =1000189;

/** TableName=RC_ActionView */
public static String Table_Name="RC_ActionView";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_RC_ActionView[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field.
@param AD_Field_ID Field on a tab in a window */
public void SetAD_Field_ID (int AD_Field_ID)
{
if (AD_Field_ID <= 0) Set_Value ("AD_Field_ID", null);
else
Set_Value ("AD_Field_ID", AD_Field_ID);
}
/** Get Field.
@return Field on a tab in a window */
public int GetAD_Field_ID() 
{
Object ii = Get_Value("AD_Field_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Value.
@param ActionValue Value */
public void SetActionValue (String ActionValue)
{
if (ActionValue != null && ActionValue.Length > 1)
{
log.Warning("Length > 1 - truncated");
ActionValue = ActionValue.Substring(0,1);
}
Set_Value ("ActionValue", ActionValue);
}
/** Get Value.
@return Value */
public String GetActionValue() 
{
return (String)Get_Value("ActionValue");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set RC_ActionView_ID.
@param RC_ActionView_ID RC_ActionView_ID */
public void SetRC_ActionView_ID (int RC_ActionView_ID)
{
if (RC_ActionView_ID < 1) throw new ArgumentException ("RC_ActionView_ID is mandatory.");
Set_ValueNoCheck ("RC_ActionView_ID", RC_ActionView_ID);
}
/** Get RC_ActionView_ID.
@return RC_ActionView_ID */
public int GetRC_ActionView_ID() 
{
Object ii = Get_Value("RC_ActionView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RC_View_ID.
@param RC_View_ID RC_View_ID */
public void SetRC_View_ID (int RC_View_ID)
{
if (RC_View_ID <= 0) Set_Value ("RC_View_ID", null);
else
Set_Value ("RC_View_ID", RC_View_ID);
}
/** Get RC_View_ID.
@return RC_View_ID */
public int GetRC_View_ID() 
{
Object ii = Get_Value("RC_View_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sql Operation.
@param SQLOperation Sql Operation */
public void SetSQLOperation (String SQLOperation)
{
if (SQLOperation != null && SQLOperation.Length > 1)
{
log.Warning("Length > 1 - truncated");
SQLOperation = SQLOperation.Substring(0,1);
}
Set_Value ("SQLOperation", SQLOperation);
}
/** Get Sql Operation.
@return Sql Operation */
public String GetSQLOperation() 
{
return (String)Get_Value("SQLOperation");
}
}

}
