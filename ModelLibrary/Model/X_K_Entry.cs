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
/** Generated Model for K_Entry
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_Entry : PO
{
public X_K_Entry (Context ctx, int K_Entry_ID, Trx trxName) : base (ctx, K_Entry_ID, trxName)
{
/** if (K_Entry_ID == 0)
{
SetIsPublic (true);	// Y
SetK_Entry_ID (0);
SetK_Topic_ID (0);
SetName (null);
SetRating (0);
SetTextMsg (null);
}
 */
}
public X_K_Entry (Ctx ctx, int K_Entry_ID, Trx trxName) : base (ctx, K_Entry_ID, trxName)
{
/** if (K_Entry_ID == 0)
{
SetIsPublic (true);	// Y
SetK_Entry_ID (0);
SetK_Topic_ID (0);
SetName (null);
SetRating (0);
SetTextMsg (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Entry (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Entry (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Entry (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_Entry()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377986L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061197L;
/** AD_Table_ID=612 */
public static int Table_ID;
 // =612;

/** TableName=K_Entry */
public static String Table_Name="K_Entry";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_K_Entry[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Session.
@param AD_Session_ID User Session Online or Web */
public void SetAD_Session_ID (int AD_Session_ID)
{
if (AD_Session_ID <= 0) Set_ValueNoCheck ("AD_Session_ID", null);
else
Set_ValueNoCheck ("AD_Session_ID", AD_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetAD_Session_ID() 
{
Object ii = Get_Value("AD_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description URL.
@param DescriptionURL URL for the description */
public void SetDescriptionURL (String DescriptionURL)
{
if (DescriptionURL != null && DescriptionURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
DescriptionURL = DescriptionURL.Substring(0,120);
}
Set_Value ("DescriptionURL", DescriptionURL);
}
/** Get Description URL.
@return URL for the description */
public String GetDescriptionURL() 
{
return (String)Get_Value("DescriptionURL");
}
/** Set Public.
@param IsPublic Public can read entry */
public void SetIsPublic (Boolean IsPublic)
{
Set_Value ("IsPublic", IsPublic);
}
/** Get Public.
@return Public can read entry */
public Boolean IsPublic() 
{
Object oo = Get_Value("IsPublic");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Entry.
@param K_Entry_ID Knowledge Entry */
public void SetK_Entry_ID (int K_Entry_ID)
{
if (K_Entry_ID < 1) throw new ArgumentException ("K_Entry_ID is mandatory.");
Set_ValueNoCheck ("K_Entry_ID", K_Entry_ID);
}
/** Get Entry.
@return Knowledge Entry */
public int GetK_Entry_ID() 
{
Object ii = Get_Value("K_Entry_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Knowledge Source.
@param K_Source_ID Source of a Knowledge Entry */
public void SetK_Source_ID (int K_Source_ID)
{
if (K_Source_ID <= 0) Set_Value ("K_Source_ID", null);
else
Set_Value ("K_Source_ID", K_Source_ID);
}
/** Get Knowledge Source.
@return Source of a Knowledge Entry */
public int GetK_Source_ID() 
{
Object ii = Get_Value("K_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Knowledge Topic.
@param K_Topic_ID Knowledge Topic */
public void SetK_Topic_ID (int K_Topic_ID)
{
if (K_Topic_ID < 1) throw new ArgumentException ("K_Topic_ID is mandatory.");
Set_ValueNoCheck ("K_Topic_ID", K_Topic_ID);
}
/** Get Knowledge Topic.
@return Knowledge Topic */
public int GetK_Topic_ID() 
{
Object ii = Get_Value("K_Topic_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Keywords.
@param Keywords List of Keywords - separated by space, comma or semicolon */
public void SetKeywords (String Keywords)
{
if (Keywords != null && Keywords.Length > 255)
{
log.Warning("Length > 255 - truncated");
Keywords = Keywords.Substring(0,255);
}
Set_Value ("Keywords", Keywords);
}
/** Get Keywords.
@return List of Keywords - separated by space, comma or semicolon */
public String GetKeywords() 
{
return (String)Get_Value("Keywords");
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
/** Set Rating.
@param Rating Classification or Importance */
public void SetRating (int Rating)
{
Set_Value ("Rating", Rating);
}
/** Get Rating.
@return Classification or Importance */
public int GetRating() 
{
Object ii = Get_Value("Rating");
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
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}
