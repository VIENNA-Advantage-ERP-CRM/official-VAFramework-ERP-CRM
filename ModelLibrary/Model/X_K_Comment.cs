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
/** Generated Model for K_Comment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_Comment : PO
{
public X_K_Comment (Context ctx, int K_Comment_ID, Trx trxName) : base (ctx, K_Comment_ID, trxName)
{
/** if (K_Comment_ID == 0)
{
SetIsPublic (true);	// Y
SetK_Comment_ID (0);
SetK_Entry_ID (0);
SetRating (0);
SetTextMsg (null);
}
 */
}
public X_K_Comment (Ctx ctx, int K_Comment_ID, Trx trxName) : base (ctx, K_Comment_ID, trxName)
{
/** if (K_Comment_ID == 0)
{
SetIsPublic (true);	// Y
SetK_Comment_ID (0);
SetK_Entry_ID (0);
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
public X_K_Comment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Comment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Comment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_Comment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377970L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061181L;
/** AD_Table_ID=613 */
public static int Table_ID;
 // =613;

/** TableName=K_Comment */
public static String Table_Name="K_Comment";

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
StringBuilder sb = new StringBuilder ("X_K_Comment[").Append(Get_ID()).Append("]");
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
/** Set Entry Comment.
@param K_Comment_ID Knowledge Entry Comment */
public void SetK_Comment_ID (int K_Comment_ID)
{
if (K_Comment_ID < 1) throw new ArgumentException ("K_Comment_ID is mandatory.");
Set_ValueNoCheck ("K_Comment_ID", K_Comment_ID);
}
/** Get Entry Comment.
@return Knowledge Entry Comment */
public int GetK_Comment_ID() 
{
Object ii = Get_Value("K_Comment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetK_Comment_ID().ToString());
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
}

}
