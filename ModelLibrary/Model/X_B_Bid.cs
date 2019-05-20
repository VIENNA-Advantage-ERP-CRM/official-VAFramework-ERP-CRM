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
/** Generated Model for B_Bid
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_Bid : PO
{
public X_B_Bid (Context ctx, int B_Bid_ID, Trx trxName) : base (ctx, B_Bid_ID, trxName)
{
/** if (B_Bid_ID == 0)
{
SetAD_User_ID (0);
SetB_Bid_ID (0);
SetB_BuyerFunds_ID (0);
SetB_Topic_ID (0);
SetIsWillingToCommit (false);
SetName (null);
}
 */
}
public X_B_Bid (Ctx ctx, int B_Bid_ID, Trx trxName) : base (ctx, B_Bid_ID, trxName)
{
/** if (B_Bid_ID == 0)
{
SetAD_User_ID (0);
SetB_Bid_ID (0);
SetB_BuyerFunds_ID (0);
SetB_Topic_ID (0);
SetIsWillingToCommit (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Bid (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Bid (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_Bid (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_Bid()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367297L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050508L;
/** AD_Table_ID=686 */
public static int Table_ID;
 // =686;

/** TableName=B_Bid */
public static String Table_Name="B_Bid";

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
StringBuilder sb = new StringBuilder ("X_B_Bid[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bid.
@param B_Bid_ID Bid for a Topic */
public void SetB_Bid_ID (int B_Bid_ID)
{
if (B_Bid_ID < 1) throw new ArgumentException ("B_Bid_ID is mandatory.");
Set_ValueNoCheck ("B_Bid_ID", B_Bid_ID);
}
/** Get Bid.
@return Bid for a Topic */
public int GetB_Bid_ID() 
{
Object ii = Get_Value("B_Bid_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Buyer Funds.
@param B_BuyerFunds_ID Buyer Funds for Bids on Topics */
public void SetB_BuyerFunds_ID (int B_BuyerFunds_ID)
{
if (B_BuyerFunds_ID < 1) throw new ArgumentException ("B_BuyerFunds_ID is mandatory.");
Set_Value ("B_BuyerFunds_ID", B_BuyerFunds_ID);
}
/** Get Buyer Funds.
@return Buyer Funds for Bids on Topics */
public int GetB_BuyerFunds_ID() 
{
Object ii = Get_Value("B_BuyerFunds_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Topic.
@param B_Topic_ID Auction Topic */
public void SetB_Topic_ID (int B_Topic_ID)
{
if (B_Topic_ID < 1) throw new ArgumentException ("B_Topic_ID is mandatory.");
Set_Value ("B_Topic_ID", B_Topic_ID);
}
/** Get Topic.
@return Auction Topic */
public int GetB_Topic_ID() 
{
Object ii = Get_Value("B_Topic_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Willing to commit.
@param IsWillingToCommit Willing to commit */
public void SetIsWillingToCommit (Boolean IsWillingToCommit)
{
Set_Value ("IsWillingToCommit", IsWillingToCommit);
}
/** Get Willing to commit.
@return Willing to commit */
public Boolean IsWillingToCommit() 
{
Object oo = Get_Value("IsWillingToCommit");
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
/** Set Private Note.
@param PrivateNote Private Note - not visible to the other parties */
public void SetPrivateNote (String PrivateNote)
{
if (PrivateNote != null && PrivateNote.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
PrivateNote = PrivateNote.Substring(0,2000);
}
Set_Value ("PrivateNote", PrivateNote);
}
/** Get Private Note.
@return Private Note - not visible to the other parties */
public String GetPrivateNote() 
{
return (String)Get_Value("PrivateNote");
}
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg != null && TextMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TextMsg = TextMsg.Substring(0,2000);
}
Set_Value ("TextMsg", TextMsg);
}
/** Get Text Message.
@return Text Message */
public String GetTextMsg() 
{
return (String)Get_Value("TextMsg");
}
}

}
