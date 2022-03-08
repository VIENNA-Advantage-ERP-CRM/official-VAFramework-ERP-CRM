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
/** Generated Model for AD_ElementCtx
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ElementCtx : PO
{
public X_AD_ElementCtx (Context ctx, int AD_ElementCtx_ID, Trx trxName) : base (ctx, AD_ElementCtx_ID, trxName)
{
/** if (AD_ElementCtx_ID == 0)
{
SetAD_CtxArea_ID (0);
SetAD_ElementCtx_ID (0);
SetAD_Element_ID (0);
SetEntityType (null);	// U
SetName (null);
SetPrintName (null);
}
 */
}
public X_AD_ElementCtx (Ctx ctx, int AD_ElementCtx_ID, Trx trxName) : base (ctx, AD_ElementCtx_ID, trxName)
{
/** if (AD_ElementCtx_ID == 0)
{
SetAD_CtxArea_ID (0);
SetAD_ElementCtx_ID (0);
SetAD_Element_ID (0);
SetEntityType (null);	// U
SetName (null);
SetPrintName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ElementCtx (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ElementCtx (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ElementCtx (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ElementCtx()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361169L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044380L;
/** AD_Table_ID=927 */
public static int Table_ID;
 // =927;

/** TableName=AD_ElementCtx */
public static String Table_Name="AD_ElementCtx";

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
StringBuilder sb = new StringBuilder ("X_AD_ElementCtx[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Context Area.
@param AD_CtxArea_ID Business Domain Area Terminology */
public void SetAD_CtxArea_ID (int AD_CtxArea_ID)
{
if (AD_CtxArea_ID < 1) throw new ArgumentException ("AD_CtxArea_ID is mandatory.");
Set_ValueNoCheck ("AD_CtxArea_ID", AD_CtxArea_ID);
}
/** Get Context Area.
@return Business Domain Area Terminology */
public int GetAD_CtxArea_ID() 
{
Object ii = Get_Value("AD_CtxArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Element Context.
@param AD_ElementCtx_ID Business Area Context for Element */
public void SetAD_ElementCtx_ID (int AD_ElementCtx_ID)
{
if (AD_ElementCtx_ID < 1) throw new ArgumentException ("AD_ElementCtx_ID is mandatory.");
Set_ValueNoCheck ("AD_ElementCtx_ID", AD_ElementCtx_ID);
}
/** Get Element Context.
@return Business Area Context for Element */
public int GetAD_ElementCtx_ID() 
{
Object ii = Get_Value("AD_ElementCtx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set System Element.
@param AD_Element_ID System Element enables the central maintenance of column description and help. */
public void SetAD_Element_ID (int AD_Element_ID)
{
if (AD_Element_ID < 1) throw new ArgumentException ("AD_Element_ID is mandatory.");
Set_Value ("AD_Element_ID", AD_Element_ID);
}
/** Get System Element.
@return System Element enables the central maintenance of column description and help. */
public int GetAD_Element_ID() 
{
Object ii = Get_Value("AD_Element_ID");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName == null) throw new ArgumentException ("PrintName is mandatory.");
if (PrintName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrintName = PrintName.Substring(0,60);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
}

}
