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
/** Generated Model for K_Index
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_Index : PO
{
public X_K_Index (Context ctx, int K_Index_ID, Trx trxName) : base (ctx, K_Index_ID, trxName)
{
/** if (K_Index_ID == 0)
{
SetAD_Table_ID (0);
SetK_Index_ID (0);
SetKeyword (null);
SetRecord_ID (0);
SetSourceUpdated (DateTime.Now);
}
 */
}
public X_K_Index (Ctx ctx, int K_Index_ID, Trx trxName) : base (ctx, K_Index_ID, trxName)
{
/** if (K_Index_ID == 0)
{
SetAD_Table_ID (0);
SetK_Index_ID (0);
SetKeyword (null);
SetRecord_ID (0);
SetSourceUpdated (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Index (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Index (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Index (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_Index()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378033L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061244L;
/** AD_Table_ID=900 */
public static int Table_ID;
 // =900;

/** TableName=K_Index */
public static String Table_Name="K_Index";

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
StringBuilder sb = new StringBuilder ("X_K_Index[").Append(Get_ID()).Append("]");
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
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID <= 0) Set_ValueNoCheck ("CM_WebProject_ID", null);
else
Set_ValueNoCheck ("CM_WebProject_ID", CM_WebProject_ID);
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
if (C_DocType_ID <= 0) Set_ValueNoCheck ("C_DocType_ID", null);
else
Set_ValueNoCheck ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Excerpt.
@param Excerpt Surrounding text of the keyword */
public void SetExcerpt (String Excerpt)
{
if (Excerpt != null && Excerpt.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Excerpt = Excerpt.Substring(0,2000);
}
Set_ValueNoCheck ("Excerpt", Excerpt);
}
/** Get Excerpt.
@return Surrounding text of the keyword */
public String GetExcerpt() 
{
return (String)Get_Value("Excerpt");
}
/** Set Index.
@param K_Index_ID Text Search Index */
public void SetK_Index_ID (int K_Index_ID)
{
if (K_Index_ID < 1) throw new ArgumentException ("K_Index_ID is mandatory.");
Set_ValueNoCheck ("K_Index_ID", K_Index_ID);
}
/** Get Index.
@return Text Search Index */
public int GetK_Index_ID() 
{
Object ii = Get_Value("K_Index_ID");
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
Set_ValueNoCheck ("Keyword", Keyword);
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
if (R_RequestType_ID <= 0) Set_ValueNoCheck ("R_RequestType_ID", null);
else
Set_ValueNoCheck ("R_RequestType_ID", R_RequestType_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetR_RequestType_ID() 
{
Object ii = Get_Value("R_RequestType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Source Updated.
@param SourceUpdated Date the source document was updated */
public void SetSourceUpdated (DateTime? SourceUpdated)
{
if (SourceUpdated == null) throw new ArgumentException ("SourceUpdated is mandatory.");
Set_Value ("SourceUpdated", (DateTime?)SourceUpdated);
}
/** Get Source Updated.
@return Date the source document was updated */
public DateTime? GetSourceUpdated() 
{
return (DateTime?)Get_Value("SourceUpdated");
}
}

}
