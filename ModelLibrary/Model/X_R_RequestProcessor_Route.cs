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
/** Generated Model for R_RequestProcessor_Route
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_RequestProcessor_Route : PO
{
public X_R_RequestProcessor_Route (Context ctx, int R_RequestProcessor_Route_ID, Trx trxName) : base (ctx, R_RequestProcessor_Route_ID, trxName)
{
/** if (R_RequestProcessor_Route_ID == 0)
{
SetAD_User_ID (0);
SetR_RequestProcessor_ID (0);
SetR_RequestProcessor_Route_ID (0);
SetSeqNo (0);
}
 */
}
public X_R_RequestProcessor_Route (Ctx ctx, int R_RequestProcessor_Route_ID, Trx trxName) : base (ctx, R_RequestProcessor_Route_ID, trxName)
{
/** if (R_RequestProcessor_Route_ID == 0)
{
SetAD_User_ID (0);
SetR_RequestProcessor_ID (0);
SetR_RequestProcessor_Route_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessor_Route (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessor_Route (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessor_Route (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_RequestProcessor_Route()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383283L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066494L;
/** AD_Table_ID=474 */
public static int Table_ID;
 // =474;

/** TableName=R_RequestProcessor_Route */
public static String Table_Name="R_RequestProcessor_Route";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_R_RequestProcessor_Route[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Keyword.
@param Keyword Case insensitive keyword */
public void SetKeyword (String Keyword)
{
if (Keyword != null && Keyword.Length > 60)
{
log.Warning("Length > 60 - truncated");
Keyword = Keyword.Substring(0,60);
}
Set_Value ("Keyword", Keyword);
}
/** Get Keyword.
@return Case insensitive keyword */
public String GetKeyword() 
{
return (String)Get_Value("Keyword");
}
/** Set Request Processor.
@param R_RequestProcessor_ID Processor for Requests */
public void SetR_RequestProcessor_ID (int R_RequestProcessor_ID)
{
if (R_RequestProcessor_ID < 1) throw new ArgumentException ("R_RequestProcessor_ID is mandatory.");
Set_ValueNoCheck ("R_RequestProcessor_ID", R_RequestProcessor_ID);
}
/** Get Request Processor.
@return Processor for Requests */
public int GetR_RequestProcessor_ID() 
{
Object ii = Get_Value("R_RequestProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Routing.
@param R_RequestProcessor_Route_ID Automatic routing of requests */
public void SetR_RequestProcessor_Route_ID (int R_RequestProcessor_Route_ID)
{
if (R_RequestProcessor_Route_ID < 1) throw new ArgumentException ("R_RequestProcessor_Route_ID is mandatory.");
Set_ValueNoCheck ("R_RequestProcessor_Route_ID", R_RequestProcessor_Route_ID);
}
/** Get Request Routing.
@return Automatic routing of requests */
public int GetR_RequestProcessor_Route_ID() 
{
Object ii = Get_Value("R_RequestProcessor_Route_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param R_RequestType_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetR_RequestType_ID (int R_RequestType_ID)
{
if (R_RequestType_ID <= 0) Set_Value ("R_RequestType_ID", null);
else
Set_Value ("R_RequestType_ID", R_RequestType_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetR_RequestType_ID() 
{
Object ii = Get_Value("R_RequestType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetSeqNo().ToString());
}
}

}
