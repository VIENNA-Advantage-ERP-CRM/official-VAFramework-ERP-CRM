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
/** Generated Model for VAGL_FundAllow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_FundAllow : PO
{
public X_VAGL_FundAllow (Context ctx, int VAGL_FundAllow_ID, Trx trxName) : base (ctx, VAGL_FundAllow_ID, trxName)
{
/** if (VAGL_FundAllow_ID == 0)
{
SetVAB_Acct_Element_ID (0);
SetVAGL_FundAllow_ID (0);
SetVAGL_Fund_ID (0);
SetName (null);
}
 */
}
public X_VAGL_FundAllow (Ctx ctx, int VAGL_FundAllow_ID, Trx trxName) : base (ctx, VAGL_FundAllow_ID, trxName)
{
/** if (VAGL_FundAllow_ID == 0)
{
SetVAB_Acct_Element_ID (0);
SetVAGL_FundAllow_ID (0);
SetVAGL_Fund_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_FundAllow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_FundAllow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_FundAllow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_FundAllow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376544L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059755L;
/** VAF_TableView_ID=824 */
public static int Table_ID;
 // =824;

/** TableName=VAGL_FundAllow */
public static String Table_Name="VAGL_FundAllow";

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
StringBuilder sb = new StringBuilder ("X_VAGL_FundAllow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Account Element.
@param VAB_Acct_Element_ID Account Element */
public void SetVAB_Acct_Element_ID (int VAB_Acct_Element_ID)
{
if (VAB_Acct_Element_ID < 1) throw new ArgumentException ("VAB_Acct_Element_ID is mandatory.");
Set_Value ("VAB_Acct_Element_ID", VAB_Acct_Element_ID);
}
/** Get Account Element.
@return Account Element */
public int GetVAB_Acct_Element_ID() 
{
Object ii = Get_Value("VAB_Acct_Element_ID");
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
@param VAGL_FundAllow_ID Restriction of Funds */
public void SetVAGL_FundAllow_ID (int VAGL_FundAllow_ID)
{
if (VAGL_FundAllow_ID < 1) throw new ArgumentException ("VAGL_FundAllow_ID is mandatory.");
Set_ValueNoCheck ("VAGL_FundAllow_ID", VAGL_FundAllow_ID);
}
/** Get Fund Restriction.
@return Restriction of Funds */
public int GetVAGL_FundAllow_ID() 
{
Object ii = Get_Value("VAGL_FundAllow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set GL Fund.
@param VAGL_Fund_ID General Ledger Funds Control */
public void SetVAGL_Fund_ID (int VAGL_Fund_ID)
{
if (VAGL_Fund_ID < 1) throw new ArgumentException ("VAGL_Fund_ID is mandatory.");
Set_ValueNoCheck ("VAGL_Fund_ID", VAGL_Fund_ID);
}
/** Get GL Fund.
@return General Ledger Funds Control */
public int GetVAGL_Fund_ID() 
{
Object ii = Get_Value("VAGL_Fund_ID");
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
