namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_Theme
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Theme : PO{public X_AD_Theme (Context ctx, int AD_Theme_ID, Trx trxName) : base (ctx, AD_Theme_ID, trxName){/** if (AD_Theme_ID == 0){SetAD_Theme_ID (0);SetName (null);} */
}public X_AD_Theme (Ctx ctx, int AD_Theme_ID, Trx trxName) : base (ctx, AD_Theme_ID, trxName){/** if (AD_Theme_ID == 0){SetAD_Theme_ID (0);SetName (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Theme (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Theme (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Theme (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Theme(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27870350834848L;/** Last Updated Timestamp 4/30/2020 5:45:18 AM */
public static long updatedMS = 1588225518059L;/** AD_Table_ID=1001056 */
public static int Table_ID; // =1001056;
/** TableName=AD_Theme */
public static String Table_Name="AD_Theme";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(4);/** AccessLevel
@return 4 - System 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_Theme[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set AD_Theme_ID.
@param AD_Theme_ID AD_Theme_ID */
public void SetAD_Theme_ID (int AD_Theme_ID){if (AD_Theme_ID < 1) throw new ArgumentException ("AD_Theme_ID is mandatory.");Set_ValueNoCheck ("AD_Theme_ID", AD_Theme_ID);}/** Get AD_Theme_ID.
@return AD_Theme_ID */
public int GetAD_Theme_ID() {Object ii = Get_Value("AD_Theme_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault){Set_Value ("IsDefault", IsDefault);}/** Get Default.
@return Default value */
public Boolean IsDefault() {Object oo = Get_Value("IsDefault");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 100){log.Warning("Length > 100 - truncated");Name = Name.Substring(0,100);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Set On Primary Color.
@param OnPrimaryColor On Primary Color */
public void SetOnPrimaryColor (String OnPrimaryColor){if (OnPrimaryColor != null && OnPrimaryColor.Length > 20){log.Warning("Length > 20 - truncated");OnPrimaryColor = OnPrimaryColor.Substring(0,20);}Set_Value ("OnPrimaryColor", OnPrimaryColor);}/** Get On Primary Color.
@return On Primary Color */
public String GetOnPrimaryColor() {return (String)Get_Value("OnPrimaryColor");}/** Set On Secondary Color.
@param OnSecondaryColor On Secondary Color */
public void SetOnSecondaryColor (String OnSecondaryColor){if (OnSecondaryColor != null && OnSecondaryColor.Length > 20){log.Warning("Length > 20 - truncated");OnSecondaryColor = OnSecondaryColor.Substring(0,20);}Set_Value ("OnSecondaryColor", OnSecondaryColor);}/** Get On Secondary Color.
@return On Secondary Color */
public String GetOnSecondaryColor() {return (String)Get_Value("OnSecondaryColor");}/** Set Primary Color.
@param PrimaryColor Primary Color */
public void SetPrimaryColor (String PrimaryColor){if (PrimaryColor != null && PrimaryColor.Length > 20){log.Warning("Length > 20 - truncated");PrimaryColor = PrimaryColor.Substring(0,20);}Set_Value ("PrimaryColor", PrimaryColor);}/** Get Primary Color.
@return Primary Color */
public String GetPrimaryColor() {return (String)Get_Value("PrimaryColor");}/** Set Secondary Color.
@param SecondaryColor Secondary Color */
public void SetSecondaryColor (String SecondaryColor){if (SecondaryColor != null && SecondaryColor.Length > 20){log.Warning("Length > 20 - truncated");SecondaryColor = SecondaryColor.Substring(0,20);}Set_Value ("SecondaryColor", SecondaryColor);}/** Get Secondary Color.
@return Secondary Color */
public String GetSecondaryColor() {return (String)Get_Value("SecondaryColor");}}
}