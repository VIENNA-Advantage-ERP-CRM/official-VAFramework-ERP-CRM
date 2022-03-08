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
/** Generated Model for AD_TextTemplate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_TextTemplate : PO
{
public X_AD_TextTemplate (Context ctx, int AD_TextTemplate_ID, Trx trxName) : base (ctx, AD_TextTemplate_ID, trxName)
{
/** if (AD_TextTemplate_ID == 0)
{
SetAD_TextTemplate_ID (0);
}
 */
}
public X_AD_TextTemplate (Ctx ctx, int AD_TextTemplate_ID, Trx trxName) : base (ctx, AD_TextTemplate_ID, trxName)
{
/** if (AD_TextTemplate_ID == 0)
{
SetAD_TextTemplate_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TextTemplate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TextTemplate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TextTemplate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_TextTemplate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364539L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047750L;
/** AD_Table_ID=1000003 */
public static int Table_ID;
 // =1000003;

/** TableName=AD_TextTemplate */
public static String Table_Name="AD_TextTemplate";

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
StringBuilder sb = new StringBuilder ("X_AD_TextTemplate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_TextTemplate_ID.
@param AD_TextTemplate_ID AD_TextTemplate_ID */
public void SetAD_TextTemplate_ID (int AD_TextTemplate_ID)
{
if (AD_TextTemplate_ID < 1) throw new ArgumentException ("AD_TextTemplate_ID is mandatory.");
Set_ValueNoCheck ("AD_TextTemplate_ID", AD_TextTemplate_ID);
}
/** Get AD_TextTemplate_ID.
@return AD_TextTemplate_ID */
public int GetAD_TextTemplate_ID() 
{
Object ii = Get_Value("AD_TextTemplate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Mail Text.
@param MailText Text used for Mail message */
public void SetMailText (String MailText)
{
Set_Value ("MailText", MailText);
}
/** Get Mail Text.
@return Text used for Mail message */
public String GetMailText() 
{
return (String)Get_Value("MailText");
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
/** Set Subject.
@param Subject Email Message Subject */
public void SetSubject (String Subject)
{
if (Subject != null && Subject.Length > 50)
{
log.Warning("Length > 50 - truncated");
Subject = Subject.Substring(0,50);
}
Set_Value ("Subject", Subject);
}
/** Get Subject.
@return Email Message Subject */
public String GetSubject() 
{
return (String)Get_Value("Subject");
}
}

}
