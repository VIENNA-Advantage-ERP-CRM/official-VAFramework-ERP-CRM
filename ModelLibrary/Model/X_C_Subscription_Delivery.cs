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
/** Generated Model for C_Subscription_Delivery
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Subscription_Delivery : PO
{
public X_C_Subscription_Delivery (Context ctx, int C_Subscription_Delivery_ID, Trx trxName) : base (ctx, C_Subscription_Delivery_ID, trxName)
{
/** if (C_Subscription_Delivery_ID == 0)
{
SetC_Subscription_Delivery_ID (0);
SetC_Subscription_ID (0);
}
 */
}
public X_C_Subscription_Delivery (Ctx ctx, int C_Subscription_Delivery_ID, Trx trxName) : base (ctx, C_Subscription_Delivery_ID, trxName)
{
/** if (C_Subscription_Delivery_ID == 0)
{
SetC_Subscription_Delivery_ID (0);
SetC_Subscription_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription_Delivery (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription_Delivery (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription_Delivery (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Subscription_Delivery()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375306L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058517L;
/** AD_Table_ID=667 */
public static int Table_ID;
 // =667;

/** TableName=C_Subscription_Delivery */
public static String Table_Name="C_Subscription_Delivery";

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
StringBuilder sb = new StringBuilder ("X_C_Subscription_Delivery[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Subscription Delivery.
@param C_Subscription_Delivery_ID Optional Delivery Record for a Subscription */
public void SetC_Subscription_Delivery_ID (int C_Subscription_Delivery_ID)
{
if (C_Subscription_Delivery_ID < 1) throw new ArgumentException ("C_Subscription_Delivery_ID is mandatory.");
Set_ValueNoCheck ("C_Subscription_Delivery_ID", C_Subscription_Delivery_ID);
}
/** Get Subscription Delivery.
@return Optional Delivery Record for a Subscription */
public int GetC_Subscription_Delivery_ID() 
{
Object ii = Get_Value("C_Subscription_Delivery_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Subscription_Delivery_ID().ToString());
}
/** Set Subscription.
@param C_Subscription_ID Subscription of a Business Partner of a Product to renew */
public void SetC_Subscription_ID (int C_Subscription_ID)
{
if (C_Subscription_ID < 1) throw new ArgumentException ("C_Subscription_ID is mandatory.");
Set_ValueNoCheck ("C_Subscription_ID", C_Subscription_ID);
}
/** Get Subscription.
@return Subscription of a Business Partner of a Product to renew */
public int GetC_Subscription_ID() 
{
Object ii = Get_Value("C_Subscription_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
