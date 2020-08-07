namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_HeaderLayout
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_HeaderLayout : PO{public X_AD_HeaderLayout (Context ctx, int AD_HeaderLayout_ID, Trx trxName) : base (ctx, AD_HeaderLayout_ID, trxName){/** if (AD_HeaderLayout_ID == 0){SetAD_HeaderLayout_ID (0);} */
}public X_AD_HeaderLayout (Ctx ctx, int AD_HeaderLayout_ID, Trx trxName) : base (ctx, AD_HeaderLayout_ID, trxName){/** if (AD_HeaderLayout_ID == 0){SetAD_HeaderLayout_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_HeaderLayout (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_HeaderLayout (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_HeaderLayout (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_HeaderLayout(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27858187911942L;/** Last Updated Timestamp 12/11/2019 4:39:55 PM */
public static long updatedMS = 1576062595153L;/** AD_Table_ID=1000537 */
public static int Table_ID; // =1000537;
/** TableName=AD_HeaderLayout */
public static String Table_Name="AD_HeaderLayout";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_HeaderLayout[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Header Layout.
@param AD_HeaderLayout_ID Header Layout */
public void SetAD_HeaderLayout_ID (int AD_HeaderLayout_ID){if (AD_HeaderLayout_ID < 1) throw new ArgumentException ("AD_HeaderLayout_ID is mandatory.");Set_ValueNoCheck ("AD_HeaderLayout_ID", AD_HeaderLayout_ID);}/** Get Header Layout.
@return Header Layout */
public int GetAD_HeaderLayout_ID() {Object ii = Get_Value("AD_HeaderLayout_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Alignment AD_Reference_ID=1000223 */
public static int ALIGNMENT_AD_Reference_ID=1000223;/** Horizontal = H */
public static String ALIGNMENT_Horizontal = "H";/** Vertical = V */
public static String ALIGNMENT_Vertical = "V";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAlignmentValid (String test){return test == null || test.Equals("H") || test.Equals("V");}/** Set Alignment.
@param Alignment Alignment of Panel. */
public void SetAlignment (String Alignment){if (!IsAlignmentValid(Alignment))
throw new ArgumentException ("Alignment Invalid value - " + Alignment + " - Reference_ID=1000223 - H - V");if (Alignment != null && Alignment.Length > 1){log.Warning("Length > 1 - truncated");Alignment = Alignment.Substring(0,1);}Set_Value ("Alignment", Alignment);}/** Get Alignment.
@return Alignment of Panel. */
public String GetAlignment() {return (String)Get_Value("Alignment");}/** Set Background Color.
@param BackgroundColor Background Color of header panel */
public void SetBackgroundColor (String BackgroundColor){if (BackgroundColor != null && BackgroundColor.Length > 50){log.Warning("Length > 50 - truncated");BackgroundColor = BackgroundColor.Substring(0,50);}Set_Value ("BackgroundColor", BackgroundColor);}/** Get Background Color.
@return Background Color of header panel */
public String GetBackgroundColor() {return (String)Get_Value("BackgroundColor");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Height.
@param Height Height */
public void SetHeight (int Height){Set_Value ("Height", Height);}/** Get Height.
@return Height */
public int GetHeight() {Object ii = Get_Value("Height");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name != null && Name.Length > 50){log.Warning("Length > 50 - truncated");Name = Name.Substring(0,50);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Set Padding.
@param Padding Padding for inside content */
public void SetPadding (String Padding){if (Padding != null && Padding.Length > 20){log.Warning("Length > 20 - truncated");Padding = Padding.Substring(0,20);}Set_Value ("Padding", Padding);}/** Get Padding.
@return Padding for inside content */
public String GetPadding() {return (String)Get_Value("Padding");}/** Set Width.
@param Width Width */
public void SetWidth (int Width){Set_Value ("Width", Width);}/** Get Width.
@return Width */
public int GetWidth() {Object ii = Get_Value("Width");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}