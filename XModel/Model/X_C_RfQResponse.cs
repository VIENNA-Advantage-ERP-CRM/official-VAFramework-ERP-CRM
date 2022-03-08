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
/** Generated Model for C_RfQResponse
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQResponse : PO
{
public X_C_RfQResponse (Context ctx, int C_RfQResponse_ID, Trx trxName) : base (ctx, C_RfQResponse_ID, trxName)
{
/** if (C_RfQResponse_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Currency_ID (0);	// @C_Currency_ID@
SetC_RfQResponse_ID (0);
SetC_RfQ_ID (0);
SetIsComplete (false);
SetIsSelectedWinner (false);
SetIsSelfService (false);
SetName (null);
SetPrice (0.0);
SetProcessed (false);	// N
}
 */
}
public X_C_RfQResponse (Ctx ctx, int C_RfQResponse_ID, Trx trxName) : base (ctx, C_RfQResponse_ID, trxName)
{
/** if (C_RfQResponse_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Currency_ID (0);	// @C_Currency_ID@
SetC_RfQResponse_ID (0);
SetC_RfQ_ID (0);
SetIsComplete (false);
SetIsSelectedWinner (false);
SetIsSelfService (false);
SetName (null);
SetPrice (0.0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponse (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponse (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQResponse (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQResponse()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374961L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058172L;
/** AD_Table_ID=674 */
public static int Table_ID;
 // =674;

/** TableName=C_RfQResponse */
public static String Table_Name="C_RfQResponse";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_C_RfQResponse[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_ValueNoCheck ("AD_User_ID", null);
else
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
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
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID < 1) throw new ArgumentException ("C_BPartner_Location_ID is mandatory.");
Set_Value ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);
else
Set_Value ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response.
@param C_RfQResponse_ID Request for Quotation Response from a potential Vendor */
public void SetC_RfQResponse_ID (int C_RfQResponse_ID)
{
if (C_RfQResponse_ID < 1) throw new ArgumentException ("C_RfQResponse_ID is mandatory.");
Set_ValueNoCheck ("C_RfQResponse_ID", C_RfQResponse_ID);
}
/** Get RfQ Response.
@return Request for Quotation Response from a potential Vendor */
public int GetC_RfQResponse_ID() 
{
Object ii = Get_Value("C_RfQResponse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ.
@param C_RfQ_ID Request for Quotation */
public void SetC_RfQ_ID (int C_RfQ_ID)
{
if (C_RfQ_ID < 1) throw new ArgumentException ("C_RfQ_ID is mandatory.");
Set_ValueNoCheck ("C_RfQ_ID", C_RfQ_ID);
}
/** Get RfQ.
@return Request for Quotation */
public int GetC_RfQ_ID() 
{
Object ii = Get_Value("C_RfQ_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Check Complete.
@param CheckComplete Check Complete */
public void SetCheckComplete (String CheckComplete)
{
if (CheckComplete != null && CheckComplete.Length > 1)
{
log.Warning("Length > 1 - truncated");
CheckComplete = CheckComplete.Substring(0,1);
}
Set_Value ("CheckComplete", CheckComplete);
}
/** Get Check Complete.
@return Check Complete */
public String GetCheckComplete() 
{
return (String)Get_Value("CheckComplete");
}
/** Set Invited.
@param DateInvited Date when (last) invitation was sent */
public void SetDateInvited (DateTime? DateInvited)
{
Set_Value ("DateInvited", (DateTime?)DateInvited);
}
/** Get Invited.
@return Date when (last) invitation was sent */
public DateTime? GetDateInvited() 
{
return (DateTime?)Get_Value("DateInvited");
}
/** Set Response Date.
@param DateResponse Date of the Response */
public void SetDateResponse (DateTime? DateResponse)
{
Set_Value ("DateResponse", (DateTime?)DateResponse);
}
/** Get Response Date.
@return Date of the Response */
public DateTime? GetDateResponse() 
{
return (DateTime?)Get_Value("DateResponse");
}
/** Set Work Complete.
@param DateWorkComplete Date when work is (planned to be) complete */
public void SetDateWorkComplete (DateTime? DateWorkComplete)
{
Set_Value ("DateWorkComplete", (DateTime?)DateWorkComplete);
}
/** Get Work Complete.
@return Date when work is (planned to be) complete */
public DateTime? GetDateWorkComplete() 
{
return (DateTime?)Get_Value("DateWorkComplete");
}
/** Set Work Start.
@param DateWorkStart Date when work is (planned to be) started */
public void SetDateWorkStart (DateTime? DateWorkStart)
{
Set_Value ("DateWorkStart", (DateTime?)DateWorkStart);
}
/** Get Work Start.
@return Date when work is (planned to be) started */
public DateTime? GetDateWorkStart() 
{
return (DateTime?)Get_Value("DateWorkStart");
}
/** Set Delivery Days.
@param DeliveryDays Number of Days (planned) until Delivery */
public void SetDeliveryDays (int DeliveryDays)
{
Set_Value ("DeliveryDays", DeliveryDays);
}
/** Get Delivery Days.
@return Number of Days (planned) until Delivery */
public int GetDeliveryDays() 
{
Object ii = Get_Value("DeliveryDays");
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
/** Set Complete.
@param IsComplete It is complete */
public void SetIsComplete (Boolean IsComplete)
{
Set_Value ("IsComplete", IsComplete);
}
/** Get Complete.
@return It is complete */
public Boolean IsComplete() 
{
Object oo = Get_Value("IsComplete");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Selected Winner.
@param IsSelectedWinner The resonse is the selected winner */
public void SetIsSelectedWinner (Boolean IsSelectedWinner)
{
Set_Value ("IsSelectedWinner", IsSelectedWinner);
}
/** Get Selected Winner.
@return The resonse is the selected winner */
public Boolean IsSelectedWinner() 
{
Object oo = Get_Value("IsSelectedWinner");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
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
/** Set Price.
@param Price Price */
public void SetPrice (Decimal? Price)
{
if (Price == null) throw new ArgumentException ("Price is mandatory.");
Set_Value ("Price", (Decimal?)Price);
}
/** Get Price.
@return Price */
public Decimal GetPrice() 
{
Object bd =Get_Value("Price");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Ranking.
@param Ranking Relative Rank Number */
public void SetRanking (int Ranking)
{
Set_Value ("Ranking", Ranking);
}
/** Get Ranking.
@return Relative Rank Number */
public int GetRanking() 
{
Object ii = Get_Value("Ranking");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
