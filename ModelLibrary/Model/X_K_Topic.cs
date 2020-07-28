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
/** Generated Model for K_Topic
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_Topic : PO
{
public X_K_Topic (Context ctx, int K_Topic_ID, Trx trxName) : base (ctx, K_Topic_ID, trxName)
{
/** if (K_Topic_ID == 0)
{
SetIsPublic (true);	// Y
SetIsPublicWrite (true);	// Y
SetK_Topic_ID (0);
SetK_Type_ID (0);
SetName (null);
}
 */
}
public X_K_Topic (Ctx ctx, int K_Topic_ID, Trx trxName) : base (ctx, K_Topic_ID, trxName)
{
/** if (K_Topic_ID == 0)
{
SetIsPublic (true);	// Y
SetIsPublicWrite (true);	// Y
SetK_Topic_ID (0);
SetK_Type_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Topic (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Topic (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Topic (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_Topic()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378174L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061385L;
/** AD_Table_ID=607 */
public static int Table_ID;
 // =607;

/** TableName=K_Topic */
public static String Table_Name="K_Topic";

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
StringBuilder sb = new StringBuilder ("X_K_Topic[").Append(Get_ID()).Append("]");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
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
/** Set Public Write.
@param IsPublicWrite Public can write entries */
public void SetIsPublicWrite (Boolean IsPublicWrite)
{
Set_Value ("IsPublicWrite", IsPublicWrite);
}
/** Get Public Write.
@return Public can write entries */
public Boolean IsPublicWrite() 
{
Object oo = Get_Value("IsPublicWrite");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Knowldge Type.
@param K_Type_ID Knowledge Type */
public void SetK_Type_ID (int K_Type_ID)
{
if (K_Type_ID < 1) throw new ArgumentException ("K_Type_ID is mandatory.");
Set_ValueNoCheck ("K_Type_ID", K_Type_ID);
}
/** Get Knowldge Type.
@return Knowledge Type */
public int GetK_Type_ID() 
{
Object ii = Get_Value("K_Type_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
