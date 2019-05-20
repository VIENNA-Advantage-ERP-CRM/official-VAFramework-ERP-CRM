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
/** Generated Model for AD_Tree
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Tree : PO
{
public X_AD_Tree (Context ctx, int AD_Tree_ID, Trx trxName) : base (ctx, AD_Tree_ID, trxName)
{
/** if (AD_Tree_ID == 0)
{
SetAD_Table_ID (0);
SetAD_Tree_ID (0);
SetIsAllNodes (false);
SetIsDefault (false);	// N
SetName (null);
SetTreeType (null);
}
 */
}
public X_AD_Tree (Ctx ctx, int AD_Tree_ID, Trx trxName) : base (ctx, AD_Tree_ID, trxName)
{
/** if (AD_Tree_ID == 0)
{
SetAD_Table_ID (0);
SetAD_Tree_ID (0);
SetIsAllNodes (false);
SetIsDefault (false);	// N
SetName (null);
SetTreeType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tree (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tree (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tree (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Tree()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364555L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047766L;
/** AD_Table_ID=288 */
public static int Table_ID;
 // =288;

/** TableName=AD_Tree */
public static String Table_Name="AD_Tree";

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
StringBuilder sb = new StringBuilder ("X_AD_Tree[").Append(Get_ID()).Append("]");
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
/** Set Tree.
@param AD_Tree_ID Identifies a Tree */
public void SetAD_Tree_ID (int AD_Tree_ID)
{
if (AD_Tree_ID < 1) throw new ArgumentException ("AD_Tree_ID is mandatory.");
Set_ValueNoCheck ("AD_Tree_ID", AD_Tree_ID);
}
/** Get Tree.
@return Identifies a Tree */
public int GetAD_Tree_ID() 
{
Object ii = Get_Value("AD_Tree_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set All Nodes.
@param IsAllNodes All Nodes are included (Complete Tree) */
public void SetIsAllNodes (Boolean IsAllNodes)
{
Set_Value ("IsAllNodes", IsAllNodes);
}
/** Get All Nodes.
@return All Nodes are included (Complete Tree) */
public Boolean IsAllNodes() 
{
Object oo = Get_Value("IsAllNodes");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** TreeType AD_Reference_ID=120 */
public static int TREETYPE_AD_Reference_ID=120;
/** Activity = AY */
public static String TREETYPE_Activity = "AY";
/** BoM = BB */
public static String TREETYPE_BoM = "BB";
/** BPartner = BP */
public static String TREETYPE_BPartner = "BP";
/** CM Container = CC */
public static String TREETYPE_CMContainer = "CC";
/** CM Media = CM */
public static String TREETYPE_CMMedia = "CM";
/** CM Container Stage = CS */
public static String TREETYPE_CMContainerStage = "CS";
/** CM Template = CT */
public static String TREETYPE_CMTemplate = "CT";
/** Element Value = EV */
public static String TREETYPE_ElementValue = "EV";
/** Campaign = MC */
public static String TREETYPE_Campaign = "MC";
/** Menu = MM */
public static String TREETYPE_Menu = "MM";
/** Organization = OO */
public static String TREETYPE_Organization = "OO";
/** Product Category = PC */
public static String TREETYPE_ProductCategory = "PC";
/** Project = PJ */
public static String TREETYPE_Project = "PJ";
/** Product = PR */
public static String TREETYPE_Product = "PR";
/** Sales Region = SR */
public static String TREETYPE_SalesRegion = "SR";
/** User 1 = U1 */
public static String TREETYPE_User1 = "U1";
/** User 2 = U2 */
public static String TREETYPE_User2 = "U2";
/** User 3 = U3 */
public static String TREETYPE_User3 = "U3";
/** User 4 = U4 */
public static String TREETYPE_User4 = "U4";
/** Other = XX */
public static String TREETYPE_Other = "XX";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTreeTypeValid (String test)
{
return test.Equals("AY") || test.Equals("BB") || test.Equals("BP") || test.Equals("CC") || test.Equals("CM") || test.Equals("CS") || test.Equals("CT") || test.Equals("EV") || test.Equals("MC") || test.Equals("MM") || test.Equals("OO") || test.Equals("PC") || test.Equals("PJ") || test.Equals("PR") || test.Equals("SR") || test.Equals("U1") || test.Equals("U2") || test.Equals("U3") || test.Equals("U4") || test.Equals("XX");
}
/** Set Type | Area.
@param TreeType Element this tree is built on (i.e Product, Business Partner) */
public void SetTreeType (String TreeType)
{
if (TreeType == null) throw new ArgumentException ("TreeType is mandatory");
if (!IsTreeTypeValid(TreeType))
throw new ArgumentException ("TreeType Invalid value - " + TreeType + " - Reference_ID=120 - AY - BB - BP - CC - CM - CS - CT - EV - MC - MM - OO - PC - PJ - PR - SR - U1 - U2 - U3 - U4 - XX");
if (TreeType.Length > 2)
{
log.Warning("Length > 2 - truncated");
TreeType = TreeType.Substring(0,2);
}
Set_ValueNoCheck ("TreeType", TreeType);
}
/** Get Type | Area.
@return Element this tree is built on (i.e Product, Business Partner) */
public String GetTreeType() 
{
return (String)Get_Value("TreeType");
}
}

}
