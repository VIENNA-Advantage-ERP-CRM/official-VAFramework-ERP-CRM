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
/** Generated Model for VAF_AttachmentNote
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AttachmentNote : PO
{
public X_VAF_AttachmentNote (Context ctx, int VAF_AttachmentNote_ID, Trx trxName) : base (ctx, VAF_AttachmentNote_ID, trxName)
{
/** if (VAF_AttachmentNote_ID == 0)
{
SetVAF_AttachmentNote_ID (0);
SetVAF_Attachment_ID (0);
SetVAF_UserContact_ID (0);
SetTextMsg (null);
SetTitle (null);
}
 */
}
public X_VAF_AttachmentNote (Ctx ctx, int VAF_AttachmentNote_ID, Trx trxName) : base (ctx, VAF_AttachmentNote_ID, trxName)
{
/** if (VAF_AttachmentNote_ID == 0)
{
SetVAF_AttachmentNote_ID (0);
SetVAF_Attachment_ID (0);
SetVAF_UserContact_ID (0);
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
public X_VAF_AttachmentNote (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AttachmentNote (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AttachmentNote (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AttachmentNote()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360684L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043895L;
/** VAF_TableView_ID=705 */
public static int Table_ID;
 // =705;

/** TableName=VAF_AttachmentNote */
public static String Table_Name="VAF_AttachmentNote";

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
StringBuilder sb = new StringBuilder ("X_VAF_AttachmentNote[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Attachment Note.
@param VAF_AttachmentNote_ID Personal Attachment Note */
public void SetVAF_AttachmentNote_ID (int VAF_AttachmentNote_ID)
{
if (VAF_AttachmentNote_ID < 1) throw new ArgumentException ("VAF_AttachmentNote_ID is mandatory.");
Set_ValueNoCheck ("VAF_AttachmentNote_ID", VAF_AttachmentNote_ID);
}
/** Get Attachment Note.
@return Personal Attachment Note */
public int GetVAF_AttachmentNote_ID() 
{
Object ii = Get_Value("VAF_AttachmentNote_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attachment.
@param VAF_Attachment_ID Attachment for the document */
public void SetVAF_Attachment_ID (int VAF_Attachment_ID)
{
if (VAF_Attachment_ID < 1) throw new ArgumentException ("VAF_Attachment_ID is mandatory.");
Set_ValueNoCheck ("VAF_Attachment_ID", VAF_Attachment_ID);
}
/** Get Attachment.
@return Attachment for the document */
public int GetVAF_Attachment_ID() 
{
Object ii = Get_Value("VAF_Attachment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
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
