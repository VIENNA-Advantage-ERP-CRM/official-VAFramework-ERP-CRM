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
/** Generated Model for CM_WikiToken
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_WikiToken : PO
{
public X_CM_WikiToken (Context ctx, int CM_WikiToken_ID, Trx trxName) : base (ctx, CM_WikiToken_ID, trxName)
{
/** if (CM_WikiToken_ID == 0)
{
SetCM_WikiToken_ID (0);
SetName (null);
SetTokenType (null);	// I
}
 */
}
public X_CM_WikiToken (Ctx ctx, int CM_WikiToken_ID, Trx trxName) : base (ctx, CM_WikiToken_ID, trxName)
{
/** if (CM_WikiToken_ID == 0)
{
SetCM_WikiToken_ID (0);
SetName (null);
SetTokenType (null);	// I
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WikiToken (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WikiToken (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WikiToken (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_WikiToken()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369350L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052561L;
/** AD_Table_ID=905 */
public static int Table_ID;
 // =905;

/** TableName=CM_WikiToken */
public static String Table_Name="CM_WikiToken";

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
StringBuilder sb = new StringBuilder ("X_CM_WikiToken[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_ValueNoCheck ("AD_Table_ID", null);
else
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
/** Set Wiki Token.
@param CM_WikiToken_ID Wiki Token */
public void SetCM_WikiToken_ID (int CM_WikiToken_ID)
{
if (CM_WikiToken_ID < 1) throw new ArgumentException ("CM_WikiToken_ID is mandatory.");
Set_ValueNoCheck ("CM_WikiToken_ID", CM_WikiToken_ID);
}
/** Get Wiki Token.
@return Wiki Token */
public int GetCM_WikiToken_ID() 
{
Object ii = Get_Value("CM_WikiToken_ID");
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
/** Set Macro.
@param Macro Macro */
public void SetMacro (String Macro)
{
if (Macro != null && Macro.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Macro = Macro.Substring(0,2000);
}
Set_Value ("Macro", Macro);
}
/** Get Macro.
@return Macro */
public String GetMacro() 
{
return (String)Get_Value("Macro");
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
/** Set Sql SELECT.
@param SelectClause SQL SELECT clause */
public void SetSelectClause (String SelectClause)
{
if (SelectClause != null && SelectClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
SelectClause = SelectClause.Substring(0,2000);
}
Set_Value ("SelectClause", SelectClause);
}
/** Get Sql SELECT.
@return SQL SELECT clause */
public String GetSelectClause() 
{
return (String)Get_Value("SelectClause");
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
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

/** TokenType AD_Reference_ID=397 */
public static int TOKENTYPE_AD_Reference_ID=397;
/** External Link = E */
public static String TOKENTYPE_ExternalLink = "E";
/** Internal Link = I */
public static String TOKENTYPE_InternalLink = "I";
/** SQL Command = Q */
public static String TOKENTYPE_SQLCommand = "Q";
/** Style = S */
public static String TOKENTYPE_Style = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTokenTypeValid (String test)
{
return test.Equals("E") || test.Equals("I") || test.Equals("Q") || test.Equals("S");
}
/** Set TokenType.
@param TokenType Wiki Token Type */
public void SetTokenType (String TokenType)
{
if (TokenType == null) throw new ArgumentException ("TokenType is mandatory");
if (!IsTokenTypeValid(TokenType))
throw new ArgumentException ("TokenType Invalid value - " + TokenType + " - Reference_ID=397 - E - I - Q - S");
if (TokenType.Length > 1)
{
log.Warning("Length > 1 - truncated");
TokenType = TokenType.Substring(0,1);
}
Set_Value ("TokenType", TokenType);
}
/** Get TokenType.
@return Wiki Token Type */
public String GetTokenType() 
{
return (String)Get_Value("TokenType");
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause != null && WhereClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WhereClause = WhereClause.Substring(0,2000);
}
Set_Value ("WhereClause", WhereClause);
}
/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() 
{
return (String)Get_Value("WhereClause");
}
}

}
