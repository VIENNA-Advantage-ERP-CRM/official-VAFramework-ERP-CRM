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
/** Generated Model for AD_AttachmentNote
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AttachmentNote : PO
{
public X_AD_AttachmentNote (Context ctx, int AD_AttachmentNote_ID, Trx trxName) : base (ctx, AD_AttachmentNote_ID, trxName)
{
/** if (AD_AttachmentNote_ID == 0)
{
SetAD_AttachmentNote_ID (0);
SetAD_Attachment_ID (0);
SetAD_User_ID (0);
SetTextMsg (null);
SetTitle (null);
}
 */
}
public X_AD_AttachmentNote (Ctx ctx, int AD_AttachmentNote_ID, Trx trxName) : base (ctx, AD_AttachmentNote_ID, trxName)
{
/** if (AD_AttachmentNote_ID == 0)
{
SetAD_AttachmentNote_ID (0);
SetAD_Attachment_ID (0);
SetAD_User_ID (0);
SetTextMsg (null);
SetTitle (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentNote (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentNote (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentNote (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AttachmentNote()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360684L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043895L;
/** AD_Table_ID=705 */
public static int Table_ID;
 // =705;

/** TableName=AD_AttachmentNote */
public static String Table_Name="AD_AttachmentNote";

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
StringBuilder sb = new StringBuilder ("X_AD_AttachmentNote[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Attachment Note.
@param AD_AttachmentNote_ID Personal Attachment Note */
public void SetAD_AttachmentNote_ID (int AD_AttachmentNote_ID)
{
if (AD_AttachmentNote_ID < 1) throw new ArgumentException ("AD_AttachmentNote_ID is mandatory.");
Set_ValueNoCheck ("AD_AttachmentNote_ID", AD_AttachmentNote_ID);
}
/** Get Attachment Note.
@return Personal Attachment Note */
public int GetAD_AttachmentNote_ID() 
{
Object ii = Get_Value("AD_AttachmentNote_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attachment.
@param AD_Attachment_ID Attachment for the document */
public void SetAD_Attachment_ID (int AD_Attachment_ID)
{
if (AD_Attachment_ID < 1) throw new ArgumentException ("AD_Attachment_ID is mandatory.");
Set_ValueNoCheck ("AD_Attachment_ID", AD_Attachment_ID);
}
/** Get Attachment.
@return Attachment for the document */
public int GetAD_Attachment_ID() 
{
Object ii = Get_Value("AD_Attachment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg == null) throw new ArgumentException ("TextMsg is mandatory.");
if (TextMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TextMsg = TextMsg.Substring(0,2000);
}
Set_Value ("TextMsg", TextMsg);
}
/** Get Text Message.
@return Text Message */
public String GetTextMsg() 
{
return (String)Get_Value("TextMsg");
}
/** Set Title.
@param Title Title of the Contact */
public void SetTitle (String Title)
{
if (Title == null) throw new ArgumentException ("Title is mandatory.");
if (Title.Length > 60)
{
log.Warning("Length > 60 - truncated");
Title = Title.Substring(0,60);
}
Set_Value ("Title", Title);
}
/** Get Title.
@return Title of the Contact */
public String GetTitle() 
{
return (String)Get_Value("Title");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetTitle());
}
}

}
