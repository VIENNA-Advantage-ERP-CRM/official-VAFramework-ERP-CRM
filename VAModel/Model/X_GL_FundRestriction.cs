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
/** Generated Model for GL_FundRestriction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_FundRestriction : PO
{
public X_GL_FundRestriction (Context ctx, int GL_FundRestriction_ID, Trx trxName) : base (ctx, GL_FundRestriction_ID, trxName)
{
/** if (GL_FundRestriction_ID == 0)
{
SetC_ElementValue_ID (0);
SetGL_FundRestriction_ID (0);
SetGL_Fund_ID (0);
SetName (null);
}
 */
}
public X_GL_FundRestriction (Ctx ctx, int GL_FundRestriction_ID, Trx trxName) : base (ctx, GL_FundRestriction_ID, trxName)
{
/** if (GL_FundRestriction_ID == 0)
{
SetC_ElementValue_ID (0);
SetGL_FundRestriction_ID (0);
SetGL_Fund_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_FundRestriction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_FundRestriction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_FundRestriction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_FundRestriction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376544L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059755L;
/** AD_Table_ID=824 */
public static int Table_ID;
 // =824;

/** TableName=GL_FundRestriction */
public static String Table_Name="GL_FundRestriction";

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
StringBuilder sb = new StringBuilder ("X_GL_FundRestriction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID < 1) throw new ArgumentException ("C_ElementValue_ID is mandatory.");
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
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
/** Set Fund Restriction.
@param GL_FundRestriction_ID Restriction of Funds */
public void SetGL_FundRestriction_ID (int GL_FundRestriction_ID)
{
if (GL_FundRestriction_ID < 1) throw new ArgumentException ("GL_FundRestriction_ID is mandatory.");
Set_ValueNoCheck ("GL_FundRestriction_ID", GL_FundRestriction_ID);
}
/** Get Fund Restriction.
@return Restriction of Funds */
public int GetGL_FundRestriction_ID() 
{
Object ii = Get_Value("GL_FundRestriction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set GL Fund.
@param GL_Fund_ID General Ledger Funds Control */
public void SetGL_Fund_ID (int GL_Fund_ID)
{
if (GL_Fund_ID < 1) throw new ArgumentException ("GL_Fund_ID is mandatory.");
Set_ValueNoCheck ("GL_Fund_ID", GL_Fund_ID);
}
/** Get GL Fund.
@return General Ledger Funds Control */
public int GetGL_Fund_ID() 
{
Object ii = Get_Value("GL_Fund_ID");
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
}

}
