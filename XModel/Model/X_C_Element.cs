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
/** Generated Model for C_Element
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Element : PO
{
public X_C_Element (Context ctx, int C_Element_ID, Trx trxName) : base (ctx, C_Element_ID, trxName)
{
/** if (C_Element_ID == 0)
{
SetAD_Tree_ID (0);
SetC_Element_ID (0);
SetElementType (null);	// A
SetIsBalancing (false);
SetIsNaturalAccount (false);
SetName (null);
}
 */
}
public X_C_Element (Ctx ctx, int C_Element_ID, Trx trxName) : base (ctx, C_Element_ID, trxName)
{
/** if (C_Element_ID == 0)
{
SetAD_Tree_ID (0);
SetC_Element_ID (0);
SetElementType (null);	// A
SetIsBalancing (false);
SetIsNaturalAccount (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Element (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Element (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Element (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Element()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372109L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055320L;
/** AD_Table_ID=142 */
public static int Table_ID;
 // =142;

/** TableName=C_Element */
public static String Table_Name="C_Element";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_Element[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Element.
@param C_Element_ID Accounting Element */
public void SetC_Element_ID (int C_Element_ID)
{
if (C_Element_ID < 1) throw new ArgumentException ("C_Element_ID is mandatory.");
Set_ValueNoCheck ("C_Element_ID", C_Element_ID);
}
/** Get Element.
@return Accounting Element */
public int GetC_Element_ID() 
{
Object ii = Get_Value("C_Element_ID");
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

/** ElementType AD_Reference_ID=116 */
public static int ELEMENTTYPE_AD_Reference_ID=116;
/** Account = A */
public static String ELEMENTTYPE_Account = "A";
/** User defined = U */
public static String ELEMENTTYPE_UserDefined = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsElementTypeValid (String test)
{
return test.Equals("A") || test.Equals("U");
}
/** Set Type.
@param ElementType Element Type (account or user defined) */
public void SetElementType (String ElementType)
{
if (ElementType == null) throw new ArgumentException ("ElementType is mandatory");
if (!IsElementTypeValid(ElementType))
throw new ArgumentException ("ElementType Invalid value - " + ElementType + " - Reference_ID=116 - A - U");
if (ElementType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ElementType = ElementType.Substring(0,1);
}
Set_ValueNoCheck ("ElementType", ElementType);
}
/** Get Type.
@return Element Type (account or user defined) */
public String GetElementType() 
{
return (String)Get_Value("ElementType");
}
/** Set Balancing.
@param IsBalancing All transactions within an element value must balance (e.g. legal entities) */
public void SetIsBalancing (Boolean IsBalancing)
{
Set_Value ("IsBalancing", IsBalancing);
}
/** Get Balancing.
@return All transactions within an element value must balance (e.g. legal entities) */
public Boolean IsBalancing() 
{
Object oo = Get_Value("IsBalancing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Natural Account.
@param IsNaturalAccount The primary natural account */
public void SetIsNaturalAccount (Boolean IsNaturalAccount)
{
Set_Value ("IsNaturalAccount", IsNaturalAccount);
}
/** Get Natural Account.
@return The primary natural account */
public Boolean IsNaturalAccount() 
{
Object oo = Get_Value("IsNaturalAccount");
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
/** Set Value Format.
@param VFormat Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetVFormat (String VFormat)
{
if (VFormat != null && VFormat.Length > 40)
{
log.Warning("Length > 40 - truncated");
VFormat = VFormat.Substring(0,40);
}
Set_Value ("VFormat", VFormat);
}
/** Get Value Format.
@return Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetVFormat() 
{
return (String)Get_Value("VFormat");
}
}

}
