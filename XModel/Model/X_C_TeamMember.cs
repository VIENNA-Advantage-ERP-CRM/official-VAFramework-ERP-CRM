namespace ViennaAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_TeamMember
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TeamMember : PO{public X_C_TeamMember (Context ctx, int C_TeamMember_ID, Trx trxName) : base (ctx, C_TeamMember_ID, trxName){/** if (C_TeamMember_ID == 0){SetC_TeamMember_ID (0);SetC_Team_ID (0);} */
}public X_C_TeamMember (Ctx ctx, int C_TeamMember_ID, Trx trxName) : base (ctx, C_TeamMember_ID, trxName){/** if (C_TeamMember_ID == 0){SetC_TeamMember_ID (0);SetC_Team_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TeamMember (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TeamMember (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TeamMember (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TeamMember(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27757001023343L;/** Last Updated Timestamp 9/26/2016 1:11:46 PM */
public static long updatedMS = 1474875706554L;/** AD_Table_ID=1000427 */
public static int Table_ID; // =1000427;
/** TableName=C_TeamMember */
public static String Table_Name="C_TeamMember";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_TeamMember[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID){if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);else
Set_Value ("AD_User_ID", AD_User_ID);}/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() {Object ii = Get_Value("AD_User_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Team Member.
@param C_TeamMember_ID Team Member */
public void SetC_TeamMember_ID (int C_TeamMember_ID){if (C_TeamMember_ID < 1) throw new ArgumentException ("C_TeamMember_ID is mandatory.");Set_ValueNoCheck ("C_TeamMember_ID", C_TeamMember_ID);}/** Get Team Member.
@return Team Member */
public int GetC_TeamMember_ID() {Object ii = Get_Value("C_TeamMember_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Team.
@param C_Team_ID Team */
public void SetC_Team_ID (int C_Team_ID){if (C_Team_ID < 1) throw new ArgumentException ("C_Team_ID is mandatory.");Set_ValueNoCheck ("C_Team_ID", C_Team_ID);}/** Get Team.
@return Team */
public int GetC_Team_ID() {Object ii = Get_Value("C_Team_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Forward Document With No.
@param IsForwardDocumentWithNo Forward Document With No */
public void SetIsForwardDocumentWithNo (Boolean IsForwardDocumentWithNo){Set_Value ("IsForwardDocumentWithNo", IsForwardDocumentWithNo);}/** Get Forward Document With No.
@return Forward Document With No */
public Boolean IsForwardDocumentWithNo() {Object oo = Get_Value("IsForwardDocumentWithNo");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}