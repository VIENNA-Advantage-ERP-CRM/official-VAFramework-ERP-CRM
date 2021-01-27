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
/** Generated Model for VAT_Spool
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_Spool : PO
{
public X_VAT_Spool (Context ctx, int VAT_Spool_ID, Trx trxName) : base (ctx, VAT_Spool_ID, trxName)
{
/** if (VAT_Spool_ID == 0)
{
SetVAF_JInstance_ID (0);
SetMsgText (null);
SetSeqNo (0);
}
 */
}
public X_VAT_Spool (Ctx ctx, int VAT_Spool_ID, Trx trxName) : base (ctx, VAT_Spool_ID, trxName)
{
/** if (VAT_Spool_ID == 0)
{
SetVAF_JInstance_ID (0);
SetMsgText (null);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Spool (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Spool (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Spool (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_Spool()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384506L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067717L;
/** VAF_TableView_ID=365 */
public static int Table_ID;
 // =365;

/** TableName=VAT_Spool */
public static String Table_Name="VAT_Spool";

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
StringBuilder sb = new StringBuilder ("X_VAT_Spool[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param VAF_JInstance_ID Instance of the process */
public void SetVAF_JInstance_ID (int VAF_JInstance_ID)
{
if (VAF_JInstance_ID < 1) throw new ArgumentException ("VAF_JInstance_ID is mandatory.");
Set_ValueNoCheck ("VAF_JInstance_ID", VAF_JInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetVAF_JInstance_ID() 
{
Object ii = Get_Value("VAF_JInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_JInstance_ID().ToString());
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
Set_ValueNoCheck ("MsgText", MsgText);
}
/** Get Message Text.
@return Textual Informational, Menu or Error Message */
public String GetMsgText() 
{
return (String)Get_Value("MsgText");
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_ValueNoCheck ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
