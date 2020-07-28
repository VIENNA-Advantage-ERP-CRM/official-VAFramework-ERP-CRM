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
/** Generated Model for AD_UserBPAccess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserBPAccess : PO
{
public X_AD_UserBPAccess (Context ctx, int AD_UserBPAccess_ID, Trx trxName) : base (ctx, AD_UserBPAccess_ID, trxName)
{
/** if (AD_UserBPAccess_ID == 0)
{
SetAD_UserBPAccess_ID (0);
SetAD_User_ID (0);
SetBPAccessType (null);
}
 */
}
public X_AD_UserBPAccess (Ctx ctx, int AD_UserBPAccess_ID, Trx trxName) : base (ctx, AD_UserBPAccess_ID, trxName)
{
/** if (AD_UserBPAccess_ID == 0)
{
SetAD_UserBPAccess_ID (0);
SetAD_User_ID (0);
SetBPAccessType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserBPAccess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserBPAccess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserBPAccess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserBPAccess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365291L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048502L;
/** AD_Table_ID=813 */
public static int Table_ID;
 // =813;

/** TableName=AD_UserBPAccess */
public static String Table_Name="AD_UserBPAccess";

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
StringBuilder sb = new StringBuilder ("X_AD_UserBPAccess[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User BP Access.
@param AD_UserBPAccess_ID User/concat access to Business Partner information and resources */
public void SetAD_UserBPAccess_ID (int AD_UserBPAccess_ID)
{
if (AD_UserBPAccess_ID < 1) throw new ArgumentException ("AD_UserBPAccess_ID is mandatory.");
Set_ValueNoCheck ("AD_UserBPAccess_ID", AD_UserBPAccess_ID);
}
/** Get User BP Access.
@return User/concat access to Business Partner information and resources */
public int GetAD_UserBPAccess_ID() 
{
Object ii = Get_Value("AD_UserBPAccess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** BPAccessType AD_Reference_ID=358 */
public static int BPACCESSTYPE_AD_Reference_ID=358;
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

/** DocBaseType AD_Reference_ID=432 */
public static int DOCBASETYPE_AD_Reference_ID=432;
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
