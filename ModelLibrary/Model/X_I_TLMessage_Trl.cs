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
/** Generated Model for I_TLMessage_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_TLMessage_Trl : PO
{
public X_I_TLMessage_Trl (Context ctx, int I_TLMessage_Trl_ID, Trx trxName) : base (ctx, I_TLMessage_Trl_ID, trxName)
{
/** if (I_TLMessage_Trl_ID == 0)
{
SetI_TLLanguage_ID (0);
SetIsTranslated (false);
SetMsgText (null);
}
 */
}
public X_I_TLMessage_Trl (Ctx ctx, int I_TLMessage_Trl_ID, Trx trxName) : base (ctx, I_TLMessage_Trl_ID, trxName)
{
/** if (I_TLMessage_Trl_ID == 0)
{
SetI_TLLanguage_ID (0);
SetIsTranslated (false);
SetMsgText (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLMessage_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLMessage_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLMessage_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_TLMessage_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638799895608L;
/** Last Updated Timestamp 12/28/2012 11:32:58 AM */
public static long updatedMS = 1356674578819L;
/** AD_Table_ID=1000405 */
public static int Table_ID;
 // =1000405;

/** TableName=I_TLMessage_Trl */
public static String Table_Name="I_TLMessage_Trl";

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
StringBuilder sb = new StringBuilder ("X_I_TLMessage_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Message.
@param AD_Message_ID System Message */
public void SetAD_Message_ID (int AD_Message_ID)
{
//if (AD_Message_ID <= 0) Set_Value ("AD_Message_ID", null);
//else
//Set_Value ("AD_Message_ID", AD_Message_ID);


if (AD_Message_ID < 1) throw new ArgumentException("AD_Message_ID is mandatory.");
Set_ValueNoCheck("AD_Message_ID", AD_Message_ID);
}
/** Get Message.
@return System Message */
public int GetAD_Message_ID() 
{
Object ii = Get_Value("AD_Message_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set I_TLLanguage_ID.
@param I_TLLanguage_ID I_TLLanguage_ID */
public void SetI_TLLanguage_ID (int I_TLLanguage_ID)
{
if (I_TLLanguage_ID < 1) throw new ArgumentException ("I_TLLanguage_ID is mandatory.");
Set_ValueNoCheck ("I_TLLanguage_ID", I_TLLanguage_ID);
}
/** Get I_TLLanguage_ID.
@return I_TLLanguage_ID */
public int GetI_TLLanguage_ID() 
{
Object ii = Get_Value("I_TLLanguage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Translated.
@param IsTranslated This column is translated */
public void SetIsTranslated (Boolean IsTranslated)
{
Set_Value ("IsTranslated", IsTranslated);
}
/** Get Translated.
@return This column is translated */
public Boolean IsTranslated() 
{
Object oo = Get_Value("IsTranslated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Message Text.
@param MsgText Textual Informational, Menu or Error Message */
public void SetMsgText (String MsgText)
{
if (MsgText == null) throw new ArgumentException ("MsgText is mandatory.");
if (MsgText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MsgText = MsgText.Substring(0,2000);
}
Set_Value ("MsgText", MsgText);
}
/** Get Message Text.
@return Textual Informational, Menu or Error Message */
public String GetMsgText() 
{
return (String)Get_Value("MsgText");
}
/** Set Message Tip.
@param MsgTip Additional tip or help for this message */
public void SetMsgTip (String MsgTip)
{
if (MsgTip != null && MsgTip.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
MsgTip = MsgTip.Substring(0,2000);
}
Set_Value ("MsgTip", MsgTip);
}
/** Get Message Tip.
@return Additional tip or help for this message */
public String GetMsgTip() 
{
return (String)Get_Value("MsgTip");
}
/** Set Continue Translation.
@param IsContinueTranslation Continue Translation */
public void SetIsContinueTranslation(Boolean IsContinueTranslation)
{
    Set_Value("IsContinueTranslation", IsContinueTranslation);
}
/** Get Continue Translation.
@return Continue Translation */
public Boolean IsContinueTranslation()
{
    Object oo = Get_Value("IsContinueTranslation");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
}

}
