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
/** Generated Model for R_Group
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_Group : PO
{
public X_R_Group (Context ctx, int R_Group_ID, Trx trxName) : base (ctx, R_Group_ID, trxName)
{
/** if (R_Group_ID == 0)
{
SetName (null);
SetR_Group_ID (0);
}
 */
}
public X_R_Group (Ctx ctx, int R_Group_ID, Trx trxName) : base (ctx, R_Group_ID, trxName)
{
/** if (R_Group_ID == 0)
{
SetName (null);
SetR_Group_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Group (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Group (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Group (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_Group()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382751L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065962L;
/** AD_Table_ID=773 */
public static int Table_ID;
 // =773;

/** TableName=R_Group */
public static String Table_Name="R_Group";

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
StringBuilder sb = new StringBuilder ("X_R_Group[").Append(Get_ID()).Append("]");
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
/** Set BOM.
@param M_BOM_ID Bill of Material */
public void SetM_BOM_ID (int M_BOM_ID)
{
if (M_BOM_ID <= 0) Set_Value ("M_BOM_ID", null);
else
Set_Value ("M_BOM_ID", M_BOM_ID);
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
if (M_ChangeNotice_ID <= 0) Set_Value ("M_ChangeNotice_ID", null);
else
Set_Value ("M_ChangeNotice_ID", M_ChangeNotice_ID);
}
/** Get Change Notice.
@return Bill of Materials (Engineering) Change Notice (Version) */
public int GetM_ChangeNotice_ID() 
{
Object ii = Get_Value("M_ChangeNotice_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Group.
@param R_Group_ID Request Group */
public void SetR_Group_ID (int R_Group_ID)
{
if (R_Group_ID < 1) throw new ArgumentException ("R_Group_ID is mandatory.");
Set_ValueNoCheck ("R_Group_ID", R_Group_ID);
}
/** Get Group.
@return Request Group */
public int GetR_Group_ID() 
{
Object ii = Get_Value("R_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
