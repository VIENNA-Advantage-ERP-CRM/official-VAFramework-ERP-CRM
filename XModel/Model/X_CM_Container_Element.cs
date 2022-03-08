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
/** Generated Model for CM_Container_Element
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Container_Element : PO
{
public X_CM_Container_Element (Context ctx, int CM_Container_Element_ID, Trx trxName) : base (ctx, CM_Container_Element_ID, trxName)
{
/** if (CM_Container_Element_ID == 0)
{
SetCM_Container_Element_ID (0);
SetCM_Container_ID (0);
SetIsValid (false);
SetName (null);
}
 */
}
public X_CM_Container_Element (Ctx ctx, int CM_Container_Element_ID, Trx trxName) : base (ctx, CM_Container_Element_ID, trxName)
{
/** if (CM_Container_Element_ID == 0)
{
SetCM_Container_Element_ID (0);
SetCM_Container_ID (0);
SetIsValid (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_Element (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_Element (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Container_Element (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Container_Element()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368974L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052185L;
/** AD_Table_ID=860 */
public static int Table_ID;
 // =860;

/** TableName=CM_Container_Element */
public static String Table_Name="CM_Container_Element";

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
StringBuilder sb = new StringBuilder ("X_CM_Container_Element[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Container Element.
@param CM_Container_Element_ID Container element i.e. Headline, Content, Footer etc. */
public void SetCM_Container_Element_ID (int CM_Container_Element_ID)
{
if (CM_Container_Element_ID < 1) throw new ArgumentException ("CM_Container_Element_ID is mandatory.");
Set_ValueNoCheck ("CM_Container_Element_ID", CM_Container_Element_ID);
}
/** Get Container Element.
@return Container element i.e. Headline, Content, Footer etc. */
public int GetCM_Container_Element_ID() 
{
Object ii = Get_Value("CM_Container_Element_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Container.
@param CM_Container_ID Web Container contains content like images, text etc. */
public void SetCM_Container_ID (int CM_Container_ID)
{
if (CM_Container_ID < 1) throw new ArgumentException ("CM_Container_ID is mandatory.");
Set_ValueNoCheck ("CM_Container_ID", CM_Container_ID);
}
/** Get Web Container.
@return Web Container contains content like images, text etc. */
public int GetCM_Container_ID() 
{
Object ii = Get_Value("CM_Container_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Content HTML.
@param ContentHTML Contains the content itself */
public void SetContentHTML (String ContentHTML)
{
Set_Value ("ContentHTML", ContentHTML);
}
/** Get Content HTML.
@return Contains the content itself */
public String GetContentHTML() 
{
return (String)Get_Value("ContentHTML");
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
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
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
}

}
