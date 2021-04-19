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
/** Generated Model for VACM_ChatLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VACM_ChatLine : PO
{
public X_VACM_ChatLine (Context ctx, int VACM_ChatLine_ID, Trx trxName) : base (ctx, VACM_ChatLine_ID, trxName)
{
/** if (VACM_ChatLine_ID == 0)
{
SetVACM_ChatLine_ID (0);
SetCM_Chat_ID (0);
SetChatEntryType (null);	// N
SetConfidentialType (null);
}
 */
}
public X_VACM_ChatLine (Ctx ctx, int VACM_ChatLine_ID, Trx trxName) : base (ctx, VACM_ChatLine_ID, trxName)
{
/** if (VACM_ChatLine_ID == 0)
{
SetVACM_ChatLine_ID (0);
SetCM_Chat_ID (0);
SetChatEntryType (null);	// N
SetConfidentialType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VACM_ChatLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368504L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051715L;
/** VAF_TableView_ID=877 */
public static int Table_ID;
 // =877;

/** TableName=VACM_ChatLine */
public static String Table_Name="VACM_ChatLine";

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
StringBuilder sb = new StringBuilder ("X_VACM_ChatLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
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

/** VACM_ChatLineGrandParent_ID VAF_Control_Ref_ID=399 */
public static int VACM_ChatEntryGrandParent_ID_VAF_Control_Ref_ID=399;
/** Set Chat Entry Grandparent.
@param VACM_ChatLineGrandParent_ID Link to Grand Parent (root level) */
public void SetVACM_ChatLineGrandParent_ID (int VACM_ChatLineGrandParent_ID)
{
if (VACM_ChatLineGrandParent_ID <= 0) Set_Value ("VACM_ChatLineGrandParent_ID", null);
else
Set_Value ("VACM_ChatLineGrandParent_ID", VACM_ChatLineGrandParent_ID);
}
/** Get Chat Entry Grandparent.
@return Link to Grand Parent (root level) */
public int GetVACM_ChatLineGrandParent_ID() 
{
Object ii = Get_Value("VACM_ChatLineGrandParent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VACM_ChatLineParent_ID VAF_Control_Ref_ID=399 */
public static int VACM_ChatEntryParent_ID_VAF_Control_Ref_ID=399;
/** Set Chat Entry Parent.
@param VACM_ChatLineParent_ID Link to direct Parent */
public void SetVACM_ChatLineParent_ID (int VACM_ChatLineParent_ID)
{
if (VACM_ChatLineParent_ID <= 0) Set_Value ("VACM_ChatLineParent_ID", null);
else
Set_Value ("VACM_ChatLineParent_ID", VACM_ChatLineParent_ID);
}
/** Get Chat Entry Parent.
@return Link to direct Parent */
public int GetVACM_ChatLineParent_ID() 
{
Object ii = Get_Value("VACM_ChatLineParent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Chat Entry.
@param VACM_ChatLine_ID Individual Chat / Discussion Entry */
public void SetVACM_ChatLine_ID (int VACM_ChatLine_ID)
{
if (VACM_ChatLine_ID < 1) throw new ArgumentException ("VACM_ChatLine_ID is mandatory.");
Set_ValueNoCheck ("VACM_ChatLine_ID", VACM_ChatLine_ID);
}
/** Get Chat Entry.
@return Individual Chat / Discussion Entry */
public int GetVACM_ChatLine_ID() 
{
Object ii = Get_Value("VACM_ChatLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVACM_ChatLine_ID().ToString());
}
/** Set Chat.
@param VACM_Chat_ID Chat or discussion thread */
public void SetCM_Chat_ID (int VACM_Chat_ID)
{
if (VACM_Chat_ID < 1) throw new ArgumentException ("VACM_Chat_ID is mandatory.");
Set_ValueNoCheck ("VACM_Chat_ID", VACM_Chat_ID);
}
/** Get Chat.
@return Chat or discussion thread */
public int GetCM_Chat_ID() 
{
Object ii = Get_Value("VACM_Chat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Character Data.
@param CharacterData Long Character Field */
public void SetCharacterData (String CharacterData)
{
Set_ValueNoCheck ("CharacterData", CharacterData);
}
/** Get Character Data.
@return Long Character Field */
public String GetCharacterData() 
{
return (String)Get_Value("CharacterData");
}

/** ChatEntryType VAF_Control_Ref_ID=398 */
public static int CHATENTRYTYPE_VAF_Control_Ref_ID=398;
/** Forum (threaded) = F */
public static String CHATENTRYTYPE_ForumThreaded = "F";
/** Note (flat) = N */
public static String CHATENTRYTYPE_NoteFlat = "N";
/** Wiki = W */
public static String CHATENTRYTYPE_Wiki = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsChatEntryTypeValid (String test)
{
return test.Equals("F") || test.Equals("N") || test.Equals("W");
}
/** Set Chat Entry Type.
@param ChatEntryType Type of Chat/Forum Entry */
public void SetChatEntryType (String ChatEntryType)
{
if (ChatEntryType == null) throw new ArgumentException ("ChatEntryType is mandatory");
if (!IsChatEntryTypeValid(ChatEntryType))
throw new ArgumentException ("ChatEntryType Invalid value - " + ChatEntryType + " - Reference_ID=398 - F - N - W");
if (ChatEntryType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ChatEntryType = ChatEntryType.Substring(0,1);
}
Set_Value ("ChatEntryType", ChatEntryType);
}
/** Get Chat Entry Type.
@return Type of Chat/Forum Entry */
public String GetChatEntryType() 
{
return (String)Get_Value("ChatEntryType");
}

/** ConfidentialType VAF_Control_Ref_ID=340 */
public static int CONFIDENTIALTYPE_VAF_Control_Ref_ID=340;
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

/** ModeratorStatus VAF_Control_Ref_ID=396 */
public static int MODERATORSTATUS_VAF_Control_Ref_ID=396;
/** Not Displayed = N */
public static String MODERATORSTATUS_NotDisplayed = "N";
/** Published = P */
public static String MODERATORSTATUS_Published = "P";
/** To be reviewed = R */
public static String MODERATORSTATUS_ToBeReviewed = "R";
/** Suspicious = S */
public static String MODERATORSTATUS_Suspicious = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsModeratorStatusValid (String test)
{
return test == null || test.Equals("N") || test.Equals("P") || test.Equals("R") || test.Equals("S");
}
/** Set Moderation Status.
@param ModeratorStatus Status of Moderation */
public void SetModeratorStatus (String ModeratorStatus)
{
if (!IsModeratorStatusValid(ModeratorStatus))
throw new ArgumentException ("ModeratorStatus Invalid value - " + ModeratorStatus + " - Reference_ID=396 - N - P - R - S");
if (ModeratorStatus != null && ModeratorStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
ModeratorStatus = ModeratorStatus.Substring(0,1);
}
Set_Value ("ModeratorStatus", ModeratorStatus);
}
/** Get Moderation Status.
@return Status of Moderation */
public String GetModeratorStatus() 
{
return (String)Get_Value("ModeratorStatus");
}
/** Set Subject.
@param Subject Email Message Subject */
public void SetSubject (String Subject)
{
if (Subject != null && Subject.Length > 255)
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
}

}
