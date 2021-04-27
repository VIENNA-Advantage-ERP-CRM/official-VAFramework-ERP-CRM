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
/** Generated Model for VAB_DunningExeEntry
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_DunningExeEntry : PO
{
public X_VAB_DunningExeEntry (Context ctx, int VAB_DunningExeEntry_ID, Trx trxName) : base (ctx, VAB_DunningExeEntry_ID, trxName)
{
/** if (VAB_DunningExeEntry_ID == 0)
{
SetAmt (0.0);
SetVAB_BusinessPartner_ID (0);
SetVAB_BPart_Location_ID (0);
SetVAB_Currency_ID (0);
SetVAB_DunningExeEntry_ID (0);
SetVAB_DunningExe_ID (0);
SetProcessed (false);	// N
SetQty (0.0);
SetSalesRep_ID (0);
}
 */
}
public X_VAB_DunningExeEntry (Ctx ctx, int VAB_DunningExeEntry_ID, Trx trxName) : base (ctx, VAB_DunningExeEntry_ID, trxName)
{
/** if (VAB_DunningExeEntry_ID == 0)
{
SetAmt (0.0);
SetVAB_BusinessPartner_ID (0);
SetVAB_BPart_Location_ID (0);
SetVAB_Currency_ID (0);
SetVAB_DunningExeEntry_ID (0);
SetVAB_DunningExe_ID (0);
SetProcessed (false);	// N
SetQty (0.0);
SetSalesRep_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DunningExeEntry (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DunningExeEntry (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_DunningExeEntry (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_DunningExeEntry()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372030L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055241L;
/** VAF_TableView_ID=527 */
public static int Table_ID;
 // =527;

/** TableName=VAB_DunningExeEntry */
public static String Table_Name="VAB_DunningExeEntry";

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
StringBuilder sb = new StringBuilder ("X_VAB_DunningExeEntry[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Amount.
@param Amt Amount */
public void SetAmt (Decimal? Amt)
{
if (Amt == null) throw new ArgumentException ("Amt is mandatory.");
Set_Value ("Amt", (Decimal?)Amt);
}
/** Get Amount.
@return Amount */
public Decimal GetAmt() 
{
Object bd =Get_Value("Amt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Partner Location.
@param VAB_BPart_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetVAB_BPart_Location_ID (int VAB_BPart_Location_ID)
{
if (VAB_BPart_Location_ID < 1) throw new ArgumentException ("VAB_BPart_Location_ID is mandatory.");
Set_Value ("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetVAB_BPart_Location_ID() 
{
Object ii = Get_Value("VAB_BPart_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");
Set_Value ("VAB_Currency_ID", VAB_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() 
{
Object ii = Get_Value("VAB_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dunning Run Entry.
@param VAB_DunningExeEntry_ID Dunning Run Entry */
public void SetVAB_DunningExeEntry_ID (int VAB_DunningExeEntry_ID)
{
if (VAB_DunningExeEntry_ID < 1) throw new ArgumentException ("VAB_DunningExeEntry_ID is mandatory.");
Set_ValueNoCheck ("VAB_DunningExeEntry_ID", VAB_DunningExeEntry_ID);
}
/** Get Dunning Run Entry.
@return Dunning Run Entry */
public int GetVAB_DunningExeEntry_ID() 
{
Object ii = Get_Value("VAB_DunningExeEntry_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dunning Run.
@param VAB_DunningExe_ID Dunning Run */
public void SetVAB_DunningExe_ID (int VAB_DunningExe_ID)
{
if (VAB_DunningExe_ID < 1) throw new ArgumentException ("VAB_DunningExe_ID is mandatory.");
Set_ValueNoCheck ("VAB_DunningExe_ID", VAB_DunningExe_ID);
}
/** Get Dunning Run.
@return Dunning Run */
public int GetVAB_DunningExe_ID() 
{
Object ii = Get_Value("VAB_DunningExe_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_DunningExe_ID().ToString());
}
/** Set Note.
@param Note Optional additional user defined information */
public void SetNote (String Note)
{
if (Note != null && Note.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Note = Note.Substring(0,2000);
}
Set_Value ("Note", Note);
}
/** Get Note.
@return Optional additional user defined information */
public String GetNote() 
{
return (String)Get_Value("Note");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** SalesRep_ID VAF_Control_Ref_ID=190 */
public static int SALESREP_ID_VAF_Control_Ref_ID=190;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID < 1) throw new ArgumentException ("SalesRep_ID is mandatory.");
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}