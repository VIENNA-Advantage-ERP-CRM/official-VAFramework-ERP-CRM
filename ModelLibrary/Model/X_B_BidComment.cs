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
/** Generated Model for B_BidComment
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_BidComment : PO
{
public X_B_BidComment (Context ctx, int B_BidComment_ID, Trx trxName) : base (ctx, B_BidComment_ID, trxName)
{
/** if (B_BidComment_ID == 0)
{
SetAD_User_ID (0);
SetB_BidComment_ID (0);
SetB_Topic_ID (0);
SetTextMsg (null);
}
 */
}
public X_B_BidComment (Ctx ctx, int B_BidComment_ID, Trx trxName) : base (ctx, B_BidComment_ID, trxName)
{
/** if (B_BidComment_ID == 0)
{
SetAD_User_ID (0);
SetB_BidComment_ID (0);
SetB_Topic_ID (0);
SetTextMsg (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_BidComment (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_BidComment (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_BidComment (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_BidComment()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367313L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050524L;
/** AD_Table_ID=685 */
public static int Table_ID;
 // =685;

/** TableName=B_BidComment */
public static String Table_Name="B_BidComment";

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
StringBuilder sb = new StringBuilder ("X_B_BidComment[").Append(Get_ID()).Append("]");
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
/** Set Bid Comment.
@param B_BidComment_ID Make a comment to a Bid Topic */
public void SetB_BidComment_ID (int B_BidComment_ID)
{
if (B_BidComment_ID < 1) throw new ArgumentException ("B_BidComment_ID is mandatory.");
Set_ValueNoCheck ("B_BidComment_ID", B_BidComment_ID);
}
/** Get Bid Comment.
@return Make a comment to a Bid Topic */
public int GetB_BidComment_ID() 
{
Object ii = Get_Value("B_BidComment_ID");
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
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg == null) throw new ArgumentException ("TextMsg is mandatory.");
if (TextMsg.Length > 2000)
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
