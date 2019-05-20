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
/** Generated Model for R_MailText
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_MailText : PO
{
public X_R_MailText (Context ctx, int R_MailText_ID, Trx trxName) : base (ctx, R_MailText_ID, trxName)
{
/** if (R_MailText_ID == 0)
{
SetIsHtml (false);
SetMailHeader (null);
SetMailText (null);
SetName (null);
SetR_MailText_ID (0);
}
 */
}
public X_R_MailText (Ctx ctx, int R_MailText_ID, Trx trxName) : base (ctx, R_MailText_ID, trxName)
{
/** if (R_MailText_ID == 0)
{
SetIsHtml (false);
SetMailHeader (null);
SetMailText (null);
SetName (null);
SetR_MailText_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_MailText (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_MailText (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_MailText (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_MailText()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383017L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066228L;
/** AD_Table_ID=416 */
public static int Table_ID;
 // =416;

/** TableName=R_MailText */
public static String Table_Name="R_MailText";

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
StringBuilder sb = new StringBuilder ("X_R_MailText[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set HTML.
@param IsHtml Text has HTML tags */
public void SetIsHtml (Boolean IsHtml)
{
Set_Value ("IsHtml", IsHtml);
}
/** Get HTML.
@return Text has HTML tags */
public Boolean IsHtml() 
{
Object oo = Get_Value("IsHtml");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Subject.
@param MailHeader Mail Header (Subject) */
public void SetMailHeader (String MailHeader)
{
if (MailHeader == null) throw new ArgumentException ("MailHeader is mandatory.");
if (MailHeader.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MailHeader = MailHeader.Substring(0,2000);
}
Set_Value ("MailHeader", MailHeader);
}
/** Get Subject.
@return Mail Header (Subject) */
public String GetMailHeader() 
{
return (String)Get_Value("MailHeader");
}
/** Set Mail Text.
@param MailText Text used for Mail message */
public void SetMailText (String MailText)
{
if (MailText == null) throw new ArgumentException ("MailText is mandatory.");
if (MailText.Length > 2000)
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
/** Set Mail Text 2.
@param MailText2 Optional second text part used for Mail message */
public void SetMailText2 (String MailText2)
{
if (MailText2 != null && MailText2.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MailText2 = MailText2.Substring(0,2000);
}
Set_Value ("MailText2", MailText2);
}
/** Get Mail Text 2.
@return Optional second text part used for Mail message */
public String GetMailText2() 
{
return (String)Get_Value("MailText2");
}
/** Set Mail Text 3.
@param MailText3 Optional third text part used for Mail message */
public void SetMailText3 (String MailText3)
{
if (MailText3 != null && MailText3.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MailText3 = MailText3.Substring(0,2000);
}
Set_Value ("MailText3", MailText3);
}
/** Get Mail Text 3.
@return Optional third text part used for Mail message */
public String GetMailText3() 
{
return (String)Get_Value("MailText3");
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
/** Set Mail Template.
@param R_MailText_ID Text templates for mailings */
public void SetR_MailText_ID (int R_MailText_ID)
{
if (R_MailText_ID < 1) throw new ArgumentException ("R_MailText_ID is mandatory.");
Set_ValueNoCheck ("R_MailText_ID", R_MailText_ID);
}
/** Get Mail Template.
@return Text templates for mailings */
public int GetR_MailText_ID() 
{
Object ii = Get_Value("R_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
