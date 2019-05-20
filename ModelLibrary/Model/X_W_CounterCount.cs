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
/** Generated Model for W_CounterCount
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_CounterCount : PO
{
public X_W_CounterCount (Context ctx, int W_CounterCount_ID, Trx trxName) : base (ctx, W_CounterCount_ID, trxName)
{
/** if (W_CounterCount_ID == 0)
{
SetName (null);
SetPageURL (null);
SetW_CounterCount_ID (0);
}
 */
}
public X_W_CounterCount (Ctx ctx, int W_CounterCount_ID, Trx trxName) : base (ctx, W_CounterCount_ID, trxName)
{
/** if (W_CounterCount_ID == 0)
{
SetName (null);
SetPageURL (null);
SetW_CounterCount_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_CounterCount (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_CounterCount (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_CounterCount (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_CounterCount()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514385007L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068218L;
/** AD_Table_ID=552 */
public static int Table_ID;
 // =552;

/** TableName=W_CounterCount */
public static String Table_Name="W_CounterCount";

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
StringBuilder sb = new StringBuilder ("X_W_CounterCount[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** C_BPartner_ID AD_Reference_ID=232 */
public static int C_BPARTNER_ID_AD_Reference_ID=232;
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Counter.
@param Counter Count Value */
public void SetCounter (int Counter)
{
throw new ArgumentException ("Counter Is virtual column");
}
/** Get Counter.
@return Count Value */
public int GetCounter() 
{
Object ii = Get_Value("Counter");
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
/** Set Page URL.
@param PageURL Page URL */
public void SetPageURL (String PageURL)
{
if (PageURL == null) throw new ArgumentException ("PageURL is mandatory.");
if (PageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
PageURL = PageURL.Substring(0,120);
}
Set_Value ("PageURL", PageURL);
}
/** Get Page URL.
@return Page URL */
public String GetPageURL() 
{
return (String)Get_Value("PageURL");
}
/** Set Counter Count.
@param W_CounterCount_ID Web Counter Count Management */
public void SetW_CounterCount_ID (int W_CounterCount_ID)
{
if (W_CounterCount_ID < 1) throw new ArgumentException ("W_CounterCount_ID is mandatory.");
Set_ValueNoCheck ("W_CounterCount_ID", W_CounterCount_ID);
}
/** Get Counter Count.
@return Web Counter Count Management */
public int GetW_CounterCount_ID() 
{
Object ii = Get_Value("W_CounterCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
