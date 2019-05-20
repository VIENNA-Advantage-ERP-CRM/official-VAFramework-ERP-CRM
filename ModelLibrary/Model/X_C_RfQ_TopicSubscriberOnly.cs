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
/** Generated Model for C_RfQ_TopicSubscriberOnly
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQ_TopicSubscriberOnly : PO
{
public X_C_RfQ_TopicSubscriberOnly (Context ctx, int C_RfQ_TopicSubscriberOnly_ID, Trx trxName) : base (ctx, C_RfQ_TopicSubscriberOnly_ID, trxName)
{
/** if (C_RfQ_TopicSubscriberOnly_ID == 0)
{
SetC_RfQ_TopicSubscriberOnly_ID (0);
SetC_RfQ_TopicSubscriber_ID (0);
}
 */
}
public X_C_RfQ_TopicSubscriberOnly (Ctx ctx, int C_RfQ_TopicSubscriberOnly_ID, Trx trxName) : base (ctx, C_RfQ_TopicSubscriberOnly_ID, trxName)
{
/** if (C_RfQ_TopicSubscriberOnly_ID == 0)
{
SetC_RfQ_TopicSubscriberOnly_ID (0);
SetC_RfQ_TopicSubscriber_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriberOnly (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriberOnly (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriberOnly (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQ_TopicSubscriberOnly()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375087L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058298L;
/** AD_Table_ID=747 */
public static int Table_ID;
 // =747;

/** TableName=C_RfQ_TopicSubscriberOnly */
public static String Table_Name="C_RfQ_TopicSubscriberOnly";

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
StringBuilder sb = new StringBuilder ("X_C_RfQ_TopicSubscriberOnly[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set RfQ Topic Subscriber Restriction.
@param C_RfQ_TopicSubscriberOnly_ID Include Subscriber only for certain products or product categories */
public void SetC_RfQ_TopicSubscriberOnly_ID (int C_RfQ_TopicSubscriberOnly_ID)
{
if (C_RfQ_TopicSubscriberOnly_ID < 1) throw new ArgumentException ("C_RfQ_TopicSubscriberOnly_ID is mandatory.");
Set_ValueNoCheck ("C_RfQ_TopicSubscriberOnly_ID", C_RfQ_TopicSubscriberOnly_ID);
}
/** Get RfQ Topic Subscriber Restriction.
@return Include Subscriber only for certain products or product categories */
public int GetC_RfQ_TopicSubscriberOnly_ID() 
{
Object ii = Get_Value("C_RfQ_TopicSubscriberOnly_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Subscriber.
@param C_RfQ_TopicSubscriber_ID Request for Quotation Topic Subscriber */
public void SetC_RfQ_TopicSubscriber_ID (int C_RfQ_TopicSubscriber_ID)
{
if (C_RfQ_TopicSubscriber_ID < 1) throw new ArgumentException ("C_RfQ_TopicSubscriber_ID is mandatory.");
Set_ValueNoCheck ("C_RfQ_TopicSubscriber_ID", C_RfQ_TopicSubscriber_ID);
}
/** Get RfQ Subscriber.
@return Request for Quotation Topic Subscriber */
public int GetC_RfQ_TopicSubscriber_ID() 
{
Object ii = Get_Value("C_RfQ_TopicSubscriber_ID");
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
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID <= 0) Set_Value ("M_Product_Category_ID", null);
else
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Product_Category_ID().ToString());
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
