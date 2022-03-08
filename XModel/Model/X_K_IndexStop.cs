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
/** Generated Model for K_IndexStop
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_IndexStop : PO
{
public X_K_IndexStop (Context ctx, int K_IndexStop_ID, Trx trxName) : base (ctx, K_IndexStop_ID, trxName)
{
/** if (K_IndexStop_ID == 0)
{
SetIsManual (true);	// Y
SetK_IndexStop_ID (0);
SetKeyword (null);
}
 */
}
public X_K_IndexStop (Ctx ctx, int K_IndexStop_ID, Trx trxName) : base (ctx, K_IndexStop_ID, trxName)
{
/** if (K_IndexStop_ID == 0)
{
SetIsManual (true);	// Y
SetK_IndexStop_ID (0);
SetKeyword (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexStop (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexStop (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_IndexStop (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_IndexStop()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378080L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061291L;
/** AD_Table_ID=901 */
public static int Table_ID;
 // =901;

/** TableName=K_IndexStop */
public static String Table_Name="K_IndexStop";

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
StringBuilder sb = new StringBuilder ("X_K_IndexStop[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID <= 0) Set_Value ("CM_WebProject_ID", null);
else
Set_Value ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);
else
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_Value ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Index Stop.
@param K_IndexStop_ID Keyword not to be indexed */
public void SetK_IndexStop_ID (int K_IndexStop_ID)
{
if (K_IndexStop_ID < 1) throw new ArgumentException ("K_IndexStop_ID is mandatory.");
Set_ValueNoCheck ("K_IndexStop_ID", K_IndexStop_ID);
}
/** Get Index Stop.
@return Keyword not to be indexed */
public int GetK_IndexStop_ID() 
{
Object ii = Get_Value("K_IndexStop_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Keyword.
@param Keyword Case insensitive keyword */
public void SetKeyword (String Keyword)
{
if (Keyword == null) throw new ArgumentException ("Keyword is mandatory.");
if (Keyword.Length > 255)
{
log.Warning("Length > 255 - truncated");
Keyword = Keyword.Substring(0,255);
}
Set_Value ("Keyword", Keyword);
}
/** Get Keyword.
@return Case insensitive keyword */
public String GetKeyword() 
{
return (String)Get_Value("Keyword");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetKeyword());
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
}

}
