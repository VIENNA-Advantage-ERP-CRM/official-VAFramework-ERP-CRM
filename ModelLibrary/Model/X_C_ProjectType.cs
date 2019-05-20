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
/** Generated Model for C_ProjectType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_ProjectType : PO
{
public X_C_ProjectType (Context ctx, int C_ProjectType_ID, Trx trxName) : base (ctx, C_ProjectType_ID, trxName)
{
/** if (C_ProjectType_ID == 0)
{
SetC_ProjectType_ID (0);
SetName (null);
SetProjectCategory (null);	// N
}
 */
}
public X_C_ProjectType (Ctx ctx, int C_ProjectType_ID, Trx trxName) : base (ctx, C_ProjectType_ID, trxName)
{
/** if (C_ProjectType_ID == 0)
{
SetC_ProjectType_ID (0);
SetName (null);
SetProjectCategory (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_ProjectType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_ProjectType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374381L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057592L;
/** AD_Table_ID=575 */
public static int Table_ID;
 // =575;

/** TableName=C_ProjectType */
public static String Table_Name="C_ProjectType";

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
StringBuilder sb = new StringBuilder ("X_C_ProjectType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Project Type.
@param C_ProjectType_ID Type of the project */
public void SetC_ProjectType_ID (int C_ProjectType_ID)
{
if (C_ProjectType_ID < 1) throw new ArgumentException ("C_ProjectType_ID is mandatory.");
Set_ValueNoCheck ("C_ProjectType_ID", C_ProjectType_ID);
}
/** Get Project Type.
@return Type of the project */
public int GetC_ProjectType_ID() 
{
Object ii = Get_Value("C_ProjectType_ID");
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

/** ProjectCategory AD_Reference_ID=288 */
public static int PROJECTCATEGORY_AD_Reference_ID=288;
/** Asset Project = A */
public static String PROJECTCATEGORY_AssetProject = "A";
/** General = N */
public static String PROJECTCATEGORY_General = "N";
/** Service (Charge) Project = S */
public static String PROJECTCATEGORY_ServiceChargeProject = "S";
/** Work Order (Job) = W */
public static String PROJECTCATEGORY_WorkOrderJob = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsProjectCategoryValid (String test)
{
return test.Equals("A") || test.Equals("N") || test.Equals("S") || test.Equals("W");
}
/** Set Project Category.
@param ProjectCategory Project Category */
public void SetProjectCategory (String ProjectCategory)
{
if (ProjectCategory == null) throw new ArgumentException ("ProjectCategory is mandatory");
if (!IsProjectCategoryValid(ProjectCategory))
throw new ArgumentException ("ProjectCategory Invalid value - " + ProjectCategory + " - Reference_ID=288 - A - N - S - W");
if (ProjectCategory.Length > 1)
{
log.Warning("Length > 1 - truncated");
ProjectCategory = ProjectCategory.Substring(0,1);
}
Set_ValueNoCheck ("ProjectCategory", ProjectCategory);
}
/** Get Project Category.
@return Project Category */
public String GetProjectCategory() 
{
return (String)Get_Value("ProjectCategory");
}
}

}
