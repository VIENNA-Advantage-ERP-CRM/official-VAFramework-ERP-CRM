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
/** Generated Model for VAR_Req_Handler_Route
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAR_Req_Handler_Route : PO
{
public X_VAR_Req_Handler_Route (Context ctx, int VAR_Req_Handler_Route_ID, Trx trxName) : base (ctx, VAR_Req_Handler_Route_ID, trxName)
{
/** if (VAR_Req_Handler_Route_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAR_Req_Handler_ID (0);
SetVAR_Req_Handler_Route_ID (0);
SetSeqNo (0);
}
 */
}
public X_VAR_Req_Handler_Route (Ctx ctx, int VAR_Req_Handler_Route_ID, Trx trxName) : base (ctx, VAR_Req_Handler_Route_ID, trxName)
{
/** if (VAR_Req_Handler_Route_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAR_Req_Handler_ID (0);
SetVAR_Req_Handler_Route_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_Handler_Route (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_Handler_Route (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_Handler_Route (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAR_Req_Handler_Route()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383283L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066494L;
/** VAF_TableView_ID=474 */
public static int Table_ID;
 // =474;

/** TableName=VAR_Req_Handler_Route */
public static String Table_Name="VAR_Req_Handler_Route";

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
StringBuilder sb = new StringBuilder ("X_VAR_Req_Handler_Route[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
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
@param VAR_Req_Handler_ID Processor for Requests */
public void SetVAR_Req_Handler_ID (int VAR_Req_Handler_ID)
{
if (VAR_Req_Handler_ID < 1) throw new ArgumentException ("VAR_Req_Handler_ID is mandatory.");
Set_ValueNoCheck ("VAR_Req_Handler_ID", VAR_Req_Handler_ID);
}
/** Get Request Processor.
@return Processor for Requests */
public int GetVAR_Req_Handler_ID() 
{
Object ii = Get_Value("VAR_Req_Handler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Routing.
@param VAR_Req_Handler_Route_ID Automatic routing of requests */
public void SetVAR_Req_Handler_Route_ID (int VAR_Req_Handler_Route_ID)
{
if (VAR_Req_Handler_Route_ID < 1) throw new ArgumentException ("VAR_Req_Handler_Route_ID is mandatory.");
Set_ValueNoCheck ("VAR_Req_Handler_Route_ID", VAR_Req_Handler_Route_ID);
}
/** Get Request Routing.
@return Automatic routing of requests */
public int GetVAR_Req_Handler_Route_ID() 
{
Object ii = Get_Value("VAR_Req_Handler_Route_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param VAR_Req_Type_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetVAR_Req_Type_ID (int VAR_Req_Type_ID)
{
if (VAR_Req_Type_ID <= 0) Set_Value ("VAR_Req_Type_ID", null);
else
Set_Value ("VAR_Req_Type_ID", VAR_Req_Type_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetVAR_Req_Type_ID() 
{
Object ii = Get_Value("VAR_Req_Type_ID");
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
