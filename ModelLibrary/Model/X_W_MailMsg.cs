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
/** Generated Model for W_MailMsg
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_MailMsg : PO
{
public X_W_MailMsg (Context ctx, int W_MailMsg_ID, Trx trxName) : base (ctx, W_MailMsg_ID, trxName)
{
/** if (W_MailMsg_ID == 0)
{
SetMailMsgType (null);
SetMessage (null);
SetName (null);
SetSubject (null);
SetW_MailMsg_ID (0);
SetW_Store_ID (0);
}
 */
}
public X_W_MailMsg (Ctx ctx, int W_MailMsg_ID, Trx trxName) : base (ctx, W_MailMsg_ID, trxName)
{
/** if (W_MailMsg_ID == 0)
{
SetMailMsgType (null);
SetMessage (null);
SetName (null);
SetSubject (null);
SetW_MailMsg_ID (0);
SetW_Store_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_MailMsg (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_MailMsg (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_MailMsg (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_MailMsg()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514385023L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068234L;
/** AD_Table_ID=780 */
public static int Table_ID;
 // =780;

/** TableName=W_MailMsg */
public static String Table_Name="W_MailMsg";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_W_MailMsg[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}

/** MailMsgType AD_Reference_ID=342 */
public static int MAILMSGTYPE_AD_Reference_ID=342;
/** Subscribe = LS */
public static String MAILMSGTYPE_Subscribe = "LS";
/** UnSubscribe = LU */
public static String MAILMSGTYPE_UnSubscribe = "LU";
/** Order Acknowledgement = OA */
public static String MAILMSGTYPE_OrderAcknowledgement = "OA";
/** Payment Acknowledgement = PA */
public static String MAILMSGTYPE_PaymentAcknowledgement = "PA";
/** Payment Error = PE */
public static String MAILMSGTYPE_PaymentError = "PE";
/** User Account = UA */
public static String MAILMSGTYPE_UserAccount = "UA";
/** User Password = UP */
public static String MAILMSGTYPE_UserPassword = "UP";
/** User Verification = UV */
public static String MAILMSGTYPE_UserVerification = "UV";
/** Request = WR */
public static String MAILMSGTYPE_Request = "WR";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMailMsgTypeValid (String test)
{
return test.Equals("LS") || test.Equals("LU") || test.Equals("OA") || test.Equals("PA") || test.Equals("PE") || test.Equals("UA") || test.Equals("UP") || test.Equals("UV") || test.Equals("WR");
}
/** Set Message Type.
@param MailMsgType Mail Message Type */
public void SetMailMsgType (String MailMsgType)
{
if (MailMsgType == null) throw new ArgumentException ("MailMsgType is mandatory");
if (!IsMailMsgTypeValid(MailMsgType))
throw new ArgumentException ("MailMsgType Invalid value - " + MailMsgType + " - Reference_ID=342 - LS - LU - OA - PA - PE - UA - UP - UV - WR");
if (MailMsgType.Length > 2)
{
log.Warning("Length > 2 - truncated");
MailMsgType = MailMsgType.Substring(0,2);
}
Set_Value ("MailMsgType", MailMsgType);
}
/** Get Message Type.
@return Mail Message Type */
public String GetMailMsgType() 
{
return (String)Get_Value("MailMsgType");
}
/** Set Message.
@param Message EMail Message */
public void SetMessage (String Message)
{
if (Message == null) throw new ArgumentException ("Message is mandatory.");
if (Message.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Message = Message.Substring(0,2000);
}
Set_Value ("Message", Message);
}
/** Get Message.
@return EMail Message */
public String GetMessage() 
{
return (String)Get_Value("Message");
}
/** Set Message 2.
@param Message2 Optional second part of the EMail Message */
public void SetMessage2 (String Message2)
{
if (Message2 != null && Message2.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Message2 = Message2.Substring(0,2000);
}
Set_Value ("Message2", Message2);
}
/** Get Message 2.
@return Optional second part of the EMail Message */
public String GetMessage2() 
{
return (String)Get_Value("Message2");
}
/** Set Message 3.
@param Message3 Optional third part of the EMail Message */
public void SetMessage3 (String Message3)
{
if (Message3 != null && Message3.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Message3 = Message3.Substring(0,2000);
}
Set_Value ("Message3", Message3);
}
/** Get Message 3.
@return Optional third part of the EMail Message */
public String GetMessage3() 
{
return (String)Get_Value("Message3");
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
/** Set Subject.
@param Subject Email Message Subject */
public void SetSubject (String Subject)
{
if (Subject == null) throw new ArgumentException ("Subject is mandatory.");
if (Subject.Length > 255)
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
if (W_MailMsg_ID < 1) throw new ArgumentException ("W_MailMsg_ID is mandatory.");
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
/** Set Web Store.
@param W_Store_ID A Web Store of the Client */
public void SetW_Store_ID (int W_Store_ID)
{
if (W_Store_ID < 1) throw new ArgumentException ("W_Store_ID is mandatory.");
Set_Value ("W_Store_ID", W_Store_ID);
}
/** Get Web Store.
@return A Web Store of the Client */
public int GetW_Store_ID() 
{
Object ii = Get_Value("W_Store_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
