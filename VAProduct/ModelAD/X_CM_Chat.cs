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
/** Generated Model for CM_Chat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Chat : PO
{
public X_CM_Chat (Context ctx, int CM_Chat_ID, Trx trxName) : base (ctx, CM_Chat_ID, trxName)
{
/** if (CM_Chat_ID == 0)
{
SetAD_Table_ID (0);
SetCM_Chat_ID (0);
SetConfidentialType (null);
SetDescription (null);
SetRecord_ID (0);
}
 */
}
public X_CM_Chat (Ctx ctx, int CM_Chat_ID, Trx trxName) : base (ctx, CM_Chat_ID, trxName)
{
/** if (CM_Chat_ID == 0)
{
SetAD_Table_ID (0);
SetCM_Chat_ID (0);
SetConfidentialType (null);
SetDescription (null);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Chat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Chat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Chat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Chat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368473L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051684L;
/** AD_Table_ID=876 */
public static int Table_ID;
 // =876;

/** TableName=CM_Chat */
public static String Table_Name="CM_Chat";

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
StringBuilder sb = new StringBuilder ("X_CM_Chat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_ValueNoCheck ("AD_Table_ID", AD_Table_ID);
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
if (CM_ChatType_ID <= 0) Set_Value ("CM_ChatType_ID", null);
else
Set_Value ("CM_ChatType_ID", CM_ChatType_ID);
}
/** Get Chat Type.
@return Type of discussion / chat */
public int GetCM_ChatType_ID() 
{
Object ii = Get_Value("CM_ChatType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Chat.
@param CM_Chat_ID Chat or discussion thread */
public void SetCM_Chat_ID (int CM_Chat_ID)
{
if (CM_Chat_ID < 1) throw new ArgumentException ("CM_Chat_ID is mandatory.");
Set_ValueNoCheck ("CM_Chat_ID", CM_Chat_ID);
}
/** Get Chat.
@return Chat or discussion thread */
public int GetCM_Chat_ID() 
{
Object ii = Get_Value("CM_Chat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ConfidentialType AD_Reference_ID=340 */
public static int CONFIDENTIALTYPE_AD_Reference_ID=340;
/** Public Information = A */
public static String CONFIDENTIALTYPE_PublicInformation = "A";
/** Partner Confidential = C */
public static String CONFIDENTIALTYPE_PartnerConfidential = "C";
/** Internal = I */
public static String CONFIDENTIALTYPE_Internal = "I";
/** Private Information = P */
public static String CONFIDENTIALTYPE_PrivateInformation = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfidentialTypeValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Confidentiality.
@param ConfidentialType Type of Confidentiality */
public void SetConfidentialType (String ConfidentialType)
{
if (ConfidentialType == null) throw new ArgumentException ("ConfidentialType is mandatory");
if (!IsConfidentialTypeValid(ConfidentialType))
throw new ArgumentException ("ConfidentialType Invalid value - " + ConfidentialType + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialType = ConfidentialType.Substring(0,1);
}
Set_Value ("ConfidentialType", ConfidentialType);
}
/** Get Confidentiality.
@return Type of Confidentiality */
public String GetConfidentialType() 
{
return (String)Get_Value("ConfidentialType");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description == null) throw new ArgumentException ("Description is mandatory.");
if (Description.Length > 255)
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDescription());
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
