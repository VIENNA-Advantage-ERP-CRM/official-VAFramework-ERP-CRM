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
/** Generated Model for VAF_UserMailLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_UserMailLog : PO
{
public X_VAF_UserMailLog (Context ctx, int VAF_UserMailLog_ID, Trx trxName) : base (ctx, VAF_UserMailLog_ID, trxName)
{
/** if (VAF_UserMailLog_ID == 0)
{
SetVAF_UserMailLog_ID (0);
SetVAF_UserContact_ID (0);
}
 */
}
public X_VAF_UserMailLog (Ctx ctx, int VAF_UserMailLog_ID, Trx trxName) : base (ctx, VAF_UserMailLog_ID, trxName)
{
/** if (VAF_UserMailLog_ID == 0)
{
SetVAF_UserMailLog_ID (0);
SetVAF_UserContact_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserMailLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserMailLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserMailLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_UserMailLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365432L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048643L;
/** VAF_TableView_ID=782 */
public static int Table_ID;
 // =782;

/** TableName=VAF_UserMailLog */
public static String Table_Name="VAF_UserMailLog";

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
StringBuilder sb = new StringBuilder ("X_VAF_UserMailLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User Mail.
@param VAF_UserMailLog_ID Mail sent to the user */
public void SetVAF_UserMailLog_ID (int VAF_UserMailLog_ID)
{
if (VAF_UserMailLog_ID < 1) throw new ArgumentException ("VAF_UserMailLog_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserMailLog_ID", VAF_UserMailLog_ID);
}
/** Get User Mail.
@return Mail sent to the user */
public int GetVAF_UserMailLog_ID() 
{
Object ii = Get_Value("VAF_UserMailLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_UserContact_ID().ToString());
}
/** Set Delivery Confirmation.
@param DeliveryConfirmation EMail Delivery confirmation */
public void SetDeliveryConfirmation (String DeliveryConfirmation)
{
if (DeliveryConfirmation != null && DeliveryConfirmation.Length > 120)
{
log.Warning("Length > 120 - truncated");
DeliveryConfirmation = DeliveryConfirmation.Substring(0,120);
}
Set_ValueNoCheck ("DeliveryConfirmation", DeliveryConfirmation);
}
/** Get Delivery Confirmation.
@return EMail Delivery confirmation */
public String GetDeliveryConfirmation() 
{
return (String)Get_Value("DeliveryConfirmation");
}

/** IsDelivered VAF_Control_Ref_ID=319 */
public static int ISDELIVERED_VAF_Control_Ref_ID=319;
/** No = N */
public static String ISDELIVERED_No = "N";
/** Yes = Y */
public static String ISDELIVERED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsDeliveredValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Delivered.
@param IsDelivered Delivered */
public void SetIsDelivered (String IsDelivered)
{
if (!IsIsDeliveredValid(IsDelivered))
throw new ArgumentException ("IsDelivered Invalid value - " + IsDelivered + " - Reference_ID=319 - N - Y");
if (IsDelivered != null && IsDelivered.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsDelivered = IsDelivered.Substring(0,1);
}
Set_ValueNoCheck ("IsDelivered", IsDelivered);
}
/** Get Delivered.
@return Delivered */
public String GetIsDelivered() 
{
return (String)Get_Value("IsDelivered");
}
/** Set Mail Text.
@param MailText Text used for Mail message */
public void SetMailText (String MailText)
{
if (MailText != null && MailText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MailText = MailText.Substring(0,2000);
}
Set_Value ("MailText", MailText);
}
/** Get Mail Text.
@return Text used for Mail message */
public String GetMailText() 
{
return (String)Get_Value("MailText");
}
/** Set Message ID.
@param MessageID EMail Message ID */
public void SetMessageID (String MessageID)
{
if (MessageID != null && MessageID.Length > 120)
{
log.Warning("Length > 120 - truncated");
MessageID = MessageID.Substring(0,120);
}
Set_ValueNoCheck ("MessageID", MessageID);
}
/** Get Message ID.
@return EMail Message ID */
public String GetMessageID() 
{
return (String)Get_Value("MessageID");
}
/** Set Mail Template.
@param VAR_MailTemplate_ID Text templates for mailings */
public void SetVAR_MailTemplate_ID (int VAR_MailTemplate_ID)
{
if (VAR_MailTemplate_ID <= 0) Set_ValueNoCheck ("VAR_MailTemplate_ID", null);
else
Set_ValueNoCheck ("VAR_MailTemplate_ID", VAR_MailTemplate_ID);
}
/** Get Mail Template.
@return Text templates for mailings */
public int GetVAR_MailTemplate_ID() 
{
Object ii = Get_Value("VAR_MailTemplate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Subject.
@param Subject Email Message Subject */
public void SetSubject (String Subject)
{
if (Subject != null && Subject.Length > 255)
{
log.Warning("Length > 255 - truncated");
Subject = Subject.Substring(0,255);
}
Set_Value ("Subject", Subject);
}
/** Get Subject.
@return Email Message Subject */
public String GetSubject() 
{
return (String)Get_Value("Subject");
}
/** Set Mail Message.
@param W_MailMsg_ID Web Store Mail Message Template */
public void SetW_MailMsg_ID (int W_MailMsg_ID)
{
if (W_MailMsg_ID <= 0) Set_ValueNoCheck ("W_MailMsg_ID", null);
else
Set_ValueNoCheck ("W_MailMsg_ID", W_MailMsg_ID);
}
/** Get Mail Message.
@return Web Store Mail Message Template */
public int GetW_MailMsg_ID() 
{
Object ii = Get_Value("W_MailMsg_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
