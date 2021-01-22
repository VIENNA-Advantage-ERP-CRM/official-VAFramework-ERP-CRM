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
/** Generated Model for VAF_UserBPartRights
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_UserBPartRights : PO
{
public X_VAF_UserBPartRights (Context ctx, int VAF_UserBPartRights_ID, Trx trxName) : base (ctx, VAF_UserBPartRights_ID, trxName)
{
/** if (VAF_UserBPartRights_ID == 0)
{
SetVAF_UserBPartRights_ID (0);
SetVAF_UserContact_ID (0);
SetBPAccessType (null);
}
 */
}
public X_VAF_UserBPartRights (Ctx ctx, int VAF_UserBPartRights_ID, Trx trxName) : base (ctx, VAF_UserBPartRights_ID, trxName)
{
/** if (VAF_UserBPartRights_ID == 0)
{
SetVAF_UserBPartRights_ID (0);
SetVAF_UserContact_ID (0);
SetBPAccessType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserBPartRights (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserBPartRights (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserBPartRights (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_UserBPartRights()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365291L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048502L;
/** VAF_TableView_ID=813 */
public static int Table_ID;
 // =813;

/** TableName=VAF_UserBPartRights */
public static String Table_Name="VAF_UserBPartRights";

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
StringBuilder sb = new StringBuilder ("X_VAF_UserBPartRights[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User BP Access.
@param VAF_UserBPartRights_ID User/concat access to Business Partner information and resources */
public void SetVAF_UserBPartRights_ID (int VAF_UserBPartRights_ID)
{
if (VAF_UserBPartRights_ID < 1) throw new ArgumentException ("VAF_UserBPartRights_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserBPartRights_ID", VAF_UserBPartRights_ID);
}
/** Get User BP Access.
@return User/concat access to Business Partner information and resources */
public int GetVAF_UserBPartRights_ID() 
{
Object ii = Get_Value("VAF_UserBPartRights_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** BPAccessType VAF_Control_Ref_ID=358 */
public static int BPACCESSTYPE_VAF_Control_Ref_ID=358;
/** Assets, Download = A */
public static String BPACCESSTYPE_AssetsDownload = "A";
/** Business Documents = B */
public static String BPACCESSTYPE_BusinessDocuments = "B";
/** Requests = R */
public static String BPACCESSTYPE_Requests = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBPAccessTypeValid (String test)
{
return test.Equals("A") || test.Equals("B") || test.Equals("R");
}
/** Set Access Type.
@param BPAccessType Type of Access of the user/contact to Business Partner information and resources */
public void SetBPAccessType (String BPAccessType)
{
if (BPAccessType == null) throw new ArgumentException ("BPAccessType is mandatory");
if (!IsBPAccessTypeValid(BPAccessType))
throw new ArgumentException ("BPAccessType Invalid value - " + BPAccessType + " - Reference_ID=358 - A - B - R");
if (BPAccessType.Length > 1)
{
log.Warning("Length > 1 - truncated");
BPAccessType = BPAccessType.Substring(0,1);
}
Set_Value ("BPAccessType", BPAccessType);
}
/** Get Access Type.
@return Type of Access of the user/contact to Business Partner information and resources */
public String GetBPAccessType() 
{
return (String)Get_Value("BPAccessType");
}

/** DocBaseType VAF_Control_Ref_ID=432 */
public static int DOCBASETYPE_VAF_Control_Ref_ID=432;
/** Set Document BaseType.
@param DocBaseType Logical type of document */
public void SetDocBaseType (String DocBaseType)
{
if (DocBaseType != null && DocBaseType.Length > 3)
{
log.Warning("Length > 3 - truncated");
DocBaseType = DocBaseType.Substring(0,3);
}
Set_Value ("DocBaseType", DocBaseType);
}
/** Get Document BaseType.
@return Logical type of document */
public String GetDocBaseType() 
{
return (String)Get_Value("DocBaseType");
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
