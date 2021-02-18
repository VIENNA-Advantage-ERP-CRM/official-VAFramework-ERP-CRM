namespace ViennaAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAB_TeamMember
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_TeamMember : PO{public X_VAB_TeamMember (Context ctx, int VAB_TeamMember_ID, Trx trxName) : base (ctx, VAB_TeamMember_ID, trxName){/** if (VAB_TeamMember_ID == 0){SetVAB_TeamMember_ID (0);SetVAB_Team_ID (0);} */
}public X_VAB_TeamMember (Ctx ctx, int VAB_TeamMember_ID, Trx trxName) : base (ctx, VAB_TeamMember_ID, trxName){/** if (VAB_TeamMember_ID == 0){SetVAB_TeamMember_ID (0);SetVAB_Team_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TeamMember (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TeamMember (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TeamMember (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_TeamMember(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27757001023343L;/** Last Updated Timestamp 9/26/2016 1:11:46 PM */
public static long updatedMS = 1474875706554L;/** VAF_TableView_ID=1000427 */
public static int Table_ID; // =1000427;
/** TableName=VAB_TeamMember */
public static String Table_Name="VAB_TeamMember";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAB_TeamMember[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID){if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);}/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetVAF_UserContact_ID() {Object ii = Get_Value("VAF_UserContact_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Team Member.
@param VAB_TeamMember_ID Team Member */
public void SetVAB_TeamMember_ID (int VAB_TeamMember_ID){if (VAB_TeamMember_ID < 1) throw new ArgumentException ("VAB_TeamMember_ID is mandatory.");Set_ValueNoCheck ("VAB_TeamMember_ID", VAB_TeamMember_ID);}/** Get Team Member.
@return Team Member */
public int GetVAB_TeamMember_ID() {Object ii = Get_Value("VAB_TeamMember_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Team.
@param VAB_Team_ID Team */
public void SetVAB_Team_ID (int VAB_Team_ID){if (VAB_Team_ID < 1) throw new ArgumentException ("VAB_Team_ID is mandatory.");Set_ValueNoCheck ("VAB_Team_ID", VAB_Team_ID);}/** Get Team.
@return Team */
public int GetVAB_Team_ID() {Object ii = Get_Value("VAB_Team_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Forward Document With No.
@param IsForwardDocumentWithNo Forward Document With No */
public void SetIsForwardDocumentWithNo (Boolean IsForwardDocumentWithNo){Set_Value ("IsForwardDocumentWithNo", IsForwardDocumentWithNo);}/** Get Forward Document With No.
@return Forward Document With No */
public Boolean IsForwardDocumentWithNo() {Object oo = Get_Value("IsForwardDocumentWithNo");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}