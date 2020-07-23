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
/** Generated Model for CM_ChatType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_ChatType : PO
{
public X_CM_ChatType (Context ctx, int CM_ChatType_ID, Trx trxName) : base (ctx, CM_ChatType_ID, trxName)
{
/** if (CM_ChatType_ID == 0)
{
SetAD_Table_ID (0);
SetCM_ChatType_ID (0);
SetName (null);
}
 */
}
public X_CM_ChatType (Ctx ctx, int CM_ChatType_ID, Trx trxName) : base (ctx, CM_ChatType_ID, trxName)
{
/** if (CM_ChatType_ID == 0)
{
SetAD_Table_ID (0);
SetCM_ChatType_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ChatType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ChatType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ChatType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_ChatType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368535L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051746L;
/** AD_Table_ID=874 */
public static int Table_ID;
 // =874;

/** TableName=CM_ChatType */
public static String Table_Name="CM_ChatType";

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
StringBuilder sb = new StringBuilder ("X_CM_ChatType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Chat Type.
@param CM_ChatType_ID Type of discussion / chat */
public void SetCM_ChatType_ID (int CM_ChatType_ID)
{
if (CM_ChatType_ID < 1) throw new ArgumentException ("CM_ChatType_ID is mandatory.");
Set_ValueNoCheck ("CM_ChatType_ID", CM_ChatType_ID);
}
/** Get Chat Type.
@return Type of discussion / chat */
public int GetCM_ChatType_ID() 
{
Object ii = Get_Value("CM_ChatType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** ModerationType AD_Reference_ID=395 */
public static int MODERATIONTYPE_AD_Reference_ID=395;
/** After Publishing = A */
public static String MODERATIONTYPE_AfterPublishing = "A";
/** Before Publishing = B */
public static String MODERATIONTYPE_BeforePublishing = "B";
/** Not moderated = N */
public static String MODERATIONTYPE_NotModerated = "N";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsModerationTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("B") || test.Equals("N");
}
/** Set Moderation Type.
@param ModerationType Type of moderation */
public void SetModerationType (String ModerationType)
{
if (!IsModerationTypeValid(ModerationType))
throw new ArgumentException ("ModerationType Invalid value - " + ModerationType + " - Reference_ID=395 - A - B - N");
if (ModerationType != null && ModerationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ModerationType = ModerationType.Substring(0,1);
}
Set_Value ("ModerationType", ModerationType);
}
/** Get Moderation Type.
@return Type of moderation */
public String GetModerationType() 
{
return (String)Get_Value("ModerationType");
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
