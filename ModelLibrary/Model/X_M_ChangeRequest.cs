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
/** Generated Model for M_ChangeRequest
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ChangeRequest : PO
{
public X_M_ChangeRequest (Context ctx, int M_ChangeRequest_ID, Trx trxName) : base (ctx, M_ChangeRequest_ID, trxName)
{
/** if (M_ChangeRequest_ID == 0)
{
SetDocumentNo (null);
SetIsApproved (false);	// N
SetM_BOM_ID (0);
SetM_ChangeRequest_ID (0);
SetName (null);
SetProcessed (false);	// N
}
 */
}
public X_M_ChangeRequest (Ctx ctx, int M_ChangeRequest_ID, Trx trxName) : base (ctx, M_ChangeRequest_ID, trxName)
{
/** if (M_ChangeRequest_ID == 0)
{
SetDocumentNo (null);
SetIsApproved (false);	// N
SetM_BOM_ID (0);
SetM_ChangeRequest_ID (0);
SetName (null);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ChangeRequest (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ChangeRequest (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ChangeRequest (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ChangeRequest()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378566L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061777L;
/** AD_Table_ID=800 */
public static int Table_ID;
 // =800;

/** TableName=M_ChangeRequest */
public static String Table_Name="M_ChangeRequest";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_M_ChangeRequest[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Detail Information.
@param DetailInfo Additional Detail Information */
public void SetDetailInfo (String DetailInfo)
{
Set_Value ("DetailInfo", DetailInfo);
}
/** Get Detail Information.
@return Additional Detail Information */
public String GetDetailInfo() 
{
return (String)Get_Value("DetailInfo");
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDocumentNo());
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved (Boolean IsApproved)
{
Set_Value ("IsApproved", IsApproved);
}
/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved() 
{
Object oo = Get_Value("IsApproved");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set BOM.
@param M_BOM_ID Bill of Material */
public void SetM_BOM_ID (int M_BOM_ID)
{
if (M_BOM_ID < 1) throw new ArgumentException ("M_BOM_ID is mandatory.");
Set_ValueNoCheck ("M_BOM_ID", M_BOM_ID);
}
/** Get BOM.
@return Bill of Material */
public int GetM_BOM_ID() 
{
Object ii = Get_Value("M_BOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Change Notice.
@param M_ChangeNotice_ID Bill of Materials (Engineering) Change Notice (Version) */
public void SetM_ChangeNotice_ID (int M_ChangeNotice_ID)
{
if (M_ChangeNotice_ID <= 0) Set_ValueNoCheck ("M_ChangeNotice_ID", null);
else
Set_ValueNoCheck ("M_ChangeNotice_ID", M_ChangeNotice_ID);
}
/** Get Change Notice.
@return Bill of Materials (Engineering) Change Notice (Version) */
public int GetM_ChangeNotice_ID() 
{
Object ii = Get_Value("M_ChangeNotice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Change Request.
@param M_ChangeRequest_ID BOM (Engineering) Change Request */
public void SetM_ChangeRequest_ID (int M_ChangeRequest_ID)
{
if (M_ChangeRequest_ID < 1) throw new ArgumentException ("M_ChangeRequest_ID is mandatory.");
Set_ValueNoCheck ("M_ChangeRequest_ID", M_ChangeRequest_ID);
}
/** Get Change Request.
@return BOM (Engineering) Change Request */
public int GetM_ChangeRequest_ID() 
{
Object ii = Get_Value("M_ChangeRequest_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_FixChangeNotice_ID AD_Reference_ID=351 */
public static int M_FIXCHANGENOTICE_ID_AD_Reference_ID=351;
/** Set Fixed in.
@param M_FixChangeNotice_ID Fixed in Change Notice */
public void SetM_FixChangeNotice_ID (int M_FixChangeNotice_ID)
{
if (M_FixChangeNotice_ID <= 0) Set_ValueNoCheck ("M_FixChangeNotice_ID", null);
else
Set_ValueNoCheck ("M_FixChangeNotice_ID", M_FixChangeNotice_ID);
}
/** Get Fixed in.
@return Fixed in Change Notice */
public int GetM_FixChangeNotice_ID() 
{
Object ii = Get_Value("M_FixChangeNotice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
