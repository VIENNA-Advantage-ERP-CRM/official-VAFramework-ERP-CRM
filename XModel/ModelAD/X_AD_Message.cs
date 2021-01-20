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
/** Generated Model for VAF_Msg_Lable
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Msg_Lable : PO
{
public X_VAF_Msg_Lable (Context ctx, int VAF_Msg_Lable_ID, Trx trxName) : base (ctx, VAF_Msg_Lable_ID, trxName)
{
/** if (VAF_Msg_Lable_ID == 0)
{
SetVAF_Msg_Lable_ID (0);
SetEntityType (null);	// U
SetMsgText (null);
SetMsgType (null);	// I
SetValue (null);
}
 */
}
public X_VAF_Msg_Lable (Ctx ctx, int VAF_Msg_Lable_ID, Trx trxName) : base (ctx, VAF_Msg_Lable_ID, trxName)
{
/** if (VAF_Msg_Lable_ID == 0)
{
SetVAF_Msg_Lable_ID (0);
SetEntityType (null);	// U
SetMsgText (null);
SetMsgType (null);	// I
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Msg_Lable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Msg_Lable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Msg_Lable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Msg_Lable()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362204L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045415L;
/** VAF_TableView_ID=109 */
public static int Table_ID;
 // =109;

/** TableName=VAF_Msg_Lable */
public static String Table_Name="VAF_Msg_Lable";

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
StringBuilder sb = new StringBuilder ("X_VAF_Msg_Lable[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Message.
@param VAF_Msg_Lable_ID System Message */
public void SetVAF_Msg_Lable_ID (int VAF_Msg_Lable_ID)
{
if (VAF_Msg_Lable_ID < 1) throw new ArgumentException ("VAF_Msg_Lable_ID is mandatory.");
Set_ValueNoCheck ("VAF_Msg_Lable_ID", VAF_Msg_Lable_ID);
}
/** Get Message.
@return System Message */
public int GetVAF_Msg_Lable_ID() 
{
Object ii = Get_Value("VAF_Msg_Lable_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Message Text.
@param MsgText Textual Informational, Menu or Error Message */
public void SetMsgText (String MsgText)
{
if (MsgText == null) throw new ArgumentException ("MsgText is mandatory.");
if (MsgText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MsgText = MsgText.Substring(0,2000);
}
Set_Value ("MsgText", MsgText);
}
/** Get Message Text.
@return Textual Informational, Menu or Error Message */
public String GetMsgText() 
{
return (String)Get_Value("MsgText");
}
/** Set Message Tip.
@param MsgTip Additional tip or help for this message */
public void SetMsgTip (String MsgTip)
{
if (MsgTip != null && MsgTip.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MsgTip = MsgTip.Substring(0,2000);
}
Set_Value ("MsgTip", MsgTip);
}
/** Get Message Tip.
@return Additional tip or help for this message */
public String GetMsgTip() 
{
return (String)Get_Value("MsgTip");
}

/** MsgType AD_Reference_ID=103 */
public static int MSGTYPE_AD_Reference_ID=103;
/** Error = E */
public static String MSGTYPE_Error = "E";
/** Information = I */
public static String MSGTYPE_Information = "I";
/** Menu = M */
public static String MSGTYPE_Menu = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMsgTypeValid (String test)
{
return test.Equals("E") || test.Equals("I") || test.Equals("M");
}
/** Set Message Type.
@param MsgType Type of message (Informational, Menu or Error) */
public void SetMsgType (String MsgType)
{
if (MsgType == null) throw new ArgumentException ("MsgType is mandatory");
if (!IsMsgTypeValid(MsgType))
throw new ArgumentException ("MsgType Invalid value - " + MsgType + " - Reference_ID=103 - E - I - M");
if (MsgType.Length > 1)
{
log.Warning("Length > 1 - truncated");
MsgType = MsgType.Substring(0,1);
}
Set_Value ("MsgType", MsgType);
}
/** Get Message Type.
@return Type of message (Informational, Menu or Error) */
public String GetMsgType() 
{
return (String)Get_Value("MsgType");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetValue());
}
}

}
