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
/** Generated Model for AD_Desktop
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Desktop : PO
{
public X_AD_Desktop (Context ctx, int AD_Desktop_ID, Trx trxName) : base (ctx, AD_Desktop_ID, trxName)
{
/** if (AD_Desktop_ID == 0)
{
SetAD_Desktop_ID (0);
SetName (null);
}
 */
}
public X_AD_Desktop (Ctx ctx, int AD_Desktop_ID, Trx trxName) : base (ctx, AD_Desktop_ID, trxName)
{
/** if (AD_Desktop_ID == 0)
{
SetAD_Desktop_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Desktop (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Desktop (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Desktop (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Desktop()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361107L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044318L;
/** AD_Table_ID=458 */
public static int Table_ID;
 // =458;

/** TableName=AD_Desktop */
public static String Table_Name="AD_Desktop";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Desktop[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Color.
@param AD_Color_ID Color for backgrounds or indicators */
public void SetAD_Color_ID (int AD_Color_ID)
{
if (AD_Color_ID <= 0) Set_Value ("AD_Color_ID", null);
else
Set_Value ("AD_Color_ID", AD_Color_ID);
}
/** Get System Color.
@return Color for backgrounds or indicators */
public int GetAD_Color_ID() 
{
Object ii = Get_Value("AD_Color_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Desktop.
@param AD_Desktop_ID Collection of Workbenches */
public void SetAD_Desktop_ID (int AD_Desktop_ID)
{
if (AD_Desktop_ID < 1) throw new ArgumentException ("AD_Desktop_ID is mandatory.");
Set_ValueNoCheck ("AD_Desktop_ID", AD_Desktop_ID);
}
/** Get Desktop.
@return Collection of Workbenches */
public int GetAD_Desktop_ID() 
{
Object ii = Get_Value("AD_Desktop_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
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
}

}
