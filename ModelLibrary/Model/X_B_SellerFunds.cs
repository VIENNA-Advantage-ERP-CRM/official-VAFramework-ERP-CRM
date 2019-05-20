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
/** Generated Model for B_SellerFunds
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_B_SellerFunds : PO
{
public X_B_SellerFunds (Context ctx, int B_SellerFunds_ID, Trx trxName) : base (ctx, B_SellerFunds_ID, trxName)
{
/** if (B_SellerFunds_ID == 0)
{
SetAD_User_ID (0);
SetB_SellerFunds_ID (0);
SetCommittedAmt (0.0);
SetNonCommittedAmt (0.0);
}
 */
}
public X_B_SellerFunds (Ctx ctx, int B_SellerFunds_ID, Trx trxName) : base (ctx, B_SellerFunds_ID, trxName)
{
/** if (B_SellerFunds_ID == 0)
{
SetAD_User_ID (0);
SetB_SellerFunds_ID (0);
SetCommittedAmt (0.0);
SetNonCommittedAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_SellerFunds (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_SellerFunds (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_B_SellerFunds (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_B_SellerFunds()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367501L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050712L;
/** AD_Table_ID=680 */
public static int Table_ID;
 // =680;

/** TableName=B_SellerFunds */
public static String Table_Name="B_SellerFunds";

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
StringBuilder sb = new StringBuilder ("X_B_SellerFunds[").Append(Get_ID()).Append("]");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
}
/** Set Seller Funds.
@param B_SellerFunds_ID Seller Funds from Offers on Topics */
public void SetB_SellerFunds_ID (int B_SellerFunds_ID)
{
if (B_SellerFunds_ID < 1) throw new ArgumentException ("B_SellerFunds_ID is mandatory.");
Set_ValueNoCheck ("B_SellerFunds_ID", B_SellerFunds_ID);
}
/** Get Seller Funds.
@return Seller Funds from Offers on Topics */
public int GetB_SellerFunds_ID() 
{
Object ii = Get_Value("B_SellerFunds_ID");
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
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);
else
Set_Value ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Committed Amount.
@param CommittedAmt The (legal) commitment amount */
public void SetCommittedAmt (Decimal? CommittedAmt)
{
if (CommittedAmt == null) throw new ArgumentException ("CommittedAmt is mandatory.");
Set_Value ("CommittedAmt", (Decimal?)CommittedAmt);
}
/** Get Committed Amount.
@return The (legal) commitment amount */
public Decimal GetCommittedAmt() 
{
Object bd =Get_Value("CommittedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Not Committed Aount.
@param NonCommittedAmt Amount not committed yet */
public void SetNonCommittedAmt (Decimal? NonCommittedAmt)
{
if (NonCommittedAmt == null) throw new ArgumentException ("NonCommittedAmt is mandatory.");
Set_Value ("NonCommittedAmt", (Decimal?)NonCommittedAmt);
}
/** Get Not Committed Aount.
@return Amount not committed yet */
public Decimal GetNonCommittedAmt() 
{
Object bd =Get_Value("NonCommittedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
