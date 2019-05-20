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
/** Generated Model for C_RfQ_TopicSubscriber
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RfQ_TopicSubscriber : PO
{
public X_C_RfQ_TopicSubscriber (Context ctx, int C_RfQ_TopicSubscriber_ID, Trx trxName) : base (ctx, C_RfQ_TopicSubscriber_ID, trxName)
{
/** if (C_RfQ_TopicSubscriber_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_RfQ_TopicSubscriber_ID (0);
SetC_RfQ_Topic_ID (0);
}
 */
}
public X_C_RfQ_TopicSubscriber (Ctx ctx, int C_RfQ_TopicSubscriber_ID, Trx trxName) : base (ctx, C_RfQ_TopicSubscriber_ID, trxName)
{
/** if (C_RfQ_TopicSubscriber_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_RfQ_TopicSubscriber_ID (0);
SetC_RfQ_Topic_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriber (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriber (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RfQ_TopicSubscriber (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RfQ_TopicSubscriber()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375055L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058266L;
/** AD_Table_ID=670 */
public static int Table_ID;
 // =670;

/** TableName=C_RfQ_TopicSubscriber */
public static String Table_Name="C_RfQ_TopicSubscriber";

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
StringBuilder sb = new StringBuilder ("X_C_RfQ_TopicSubscriber[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
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
/** Set RfQ Topic.
@param C_RfQ_Topic_ID Topic for Request for Quotations */
public void SetC_RfQ_Topic_ID (int C_RfQ_Topic_ID)
{
if (C_RfQ_Topic_ID < 1) throw new ArgumentException ("C_RfQ_Topic_ID is mandatory.");
Set_ValueNoCheck ("C_RfQ_Topic_ID", C_RfQ_Topic_ID);
}
/** Get RfQ Topic.
@return Topic for Request for Quotations */
public int GetC_RfQ_Topic_ID() 
{
Object ii = Get_Value("C_RfQ_Topic_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_RfQ_Topic_ID().ToString());
}
/** Set Opt-out Date.
@param OptOutDate Date the contact opted out */
public void SetOptOutDate (DateTime? OptOutDate)
{
Set_Value ("OptOutDate", (DateTime?)OptOutDate);
}
/** Get Opt-out Date.
@return Date the contact opted out */
public DateTime? GetOptOutDate() 
{
return (DateTime?)Get_Value("OptOutDate");
}
/** Set Subscribe Date.
@param SubscribeDate Date the contact actively subscribed */
public void SetSubscribeDate (DateTime? SubscribeDate)
{
Set_Value ("SubscribeDate", (DateTime?)SubscribeDate);
}
/** Get Subscribe Date.
@return Date the contact actively subscribed */
public DateTime? GetSubscribeDate() 
{
return (DateTime?)Get_Value("SubscribeDate");
}
}

}
