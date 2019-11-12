namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_TabPanel
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_TabPanel : PO{public X_AD_TabPanel (Context ctx, int AD_TabPanel_ID, Trx trxName) : base (ctx, AD_TabPanel_ID, trxName){/** if (AD_TabPanel_ID == 0){SetAD_TabPanel_ID (0);SetAD_Tab_ID (0);SetClassname (null);SetName (null);} */
}public X_AD_TabPanel (Ctx ctx, int AD_TabPanel_ID, Trx trxName) : base (ctx, AD_TabPanel_ID, trxName){/** if (AD_TabPanel_ID == 0){SetAD_TabPanel_ID (0);SetAD_Tab_ID (0);SetClassname (null);SetName (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TabPanel (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TabPanel (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TabPanel (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_TabPanel(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27845577416566L;/** Last Updated Timestamp 7/18/2019 5:44:59 PM */
public static long updatedMS = 1563452099777L;/** AD_Table_ID=1001001 */
public static int Table_ID; // =1001001;
/** TableName=AD_TabPanel */
public static String Table_Name="AD_TabPanel";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_TabPanel[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Tab Panel.
@param AD_TabPanel_ID Tab Panel */
public void SetAD_TabPanel_ID (int AD_TabPanel_ID){if (AD_TabPanel_ID < 1) throw new ArgumentException ("AD_TabPanel_ID is mandatory.");Set_ValueNoCheck ("AD_TabPanel_ID", AD_TabPanel_ID);}/** Get Tab Panel.
@return Tab Panel */
public int GetAD_TabPanel_ID() {Object ii = Get_Value("AD_TabPanel_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Tab.
@param AD_Tab_ID Tab within a Window */
public void SetAD_Tab_ID (int AD_Tab_ID){if (AD_Tab_ID < 1) throw new ArgumentException ("AD_Tab_ID is mandatory.");Set_ValueNoCheck ("AD_Tab_ID", AD_Tab_ID);}/** Get Tab.
@return Tab within a Window */
public int GetAD_Tab_ID() {Object ii = Get_Value("AD_Tab_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Classname.
@param Classname Java Classname */
public void SetClassname (String Classname){if (Classname == null) throw new ArgumentException ("Classname is mandatory.");if (Classname.Length > 200){log.Warning("Length > 200 - truncated");Classname = Classname.Substring(0,200);}Set_Value ("Classname", Classname);}/** Get Classname.
@return Java Classname */
public String GetClassname() {return (String)Get_Value("Classname");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Icon Path.
@param IconPath Icon Path */
public void SetIconPath (String IconPath){if (IconPath != null && IconPath.Length > 200){log.Warning("Length > 200 - truncated");IconPath = IconPath.Substring(0,200);}Set_Value ("IconPath", IconPath);}/** Get Icon Path.
@return Icon Path */
public String GetIconPath() {return (String)Get_Value("IconPath");}/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault){Set_Value ("IsDefault", IsDefault);}/** Get Default.
@return Default value */
public Boolean IsDefault() {Object oo = Get_Value("IsDefault");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 60){log.Warning("Length > 60 - truncated");Name = Name.Substring(0,60);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
public void SetSeqNo (int SeqNo){Set_Value ("SeqNo", SeqNo);}/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
public int GetSeqNo() {Object ii = Get_Value("SeqNo");if (ii == null) return 0;return Convert.ToInt32(ii);}}
} 