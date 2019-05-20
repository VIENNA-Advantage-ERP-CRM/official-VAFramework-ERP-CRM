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
/** Generated Model for C_Subscription
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Subscription : PO
{
public X_C_Subscription (Context ctx, int C_Subscription_ID, Trx trxName) : base (ctx, C_Subscription_ID, trxName)
{
/** if (C_Subscription_ID == 0)
{
SetC_BPartner_ID (0);
SetC_SubscriptionType_ID (0);
SetC_Subscription_ID (0);
SetIsDue (false);
SetM_Product_ID (0);
SetName (null);
SetPaidUntilDate (DateTime.Now);
SetRenewalDate (DateTime.Now);
SetStartDate (DateTime.Now);
}
 */
}
public X_C_Subscription (Ctx ctx, int C_Subscription_ID, Trx trxName) : base (ctx, C_Subscription_ID, trxName)
{
/** if (C_Subscription_ID == 0)
{
SetC_BPartner_ID (0);
SetC_SubscriptionType_ID (0);
SetC_Subscription_ID (0);
SetIsDue (false);
SetM_Product_ID (0);
SetName (null);
SetPaidUntilDate (DateTime.Now);
SetRenewalDate (DateTime.Now);
SetStartDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Subscription (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Subscription()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375259L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058470L;
/** AD_Table_ID=669 */
public static int Table_ID;
 // =669;

/** TableName=C_Subscription */
public static String Table_Name="C_Subscription";

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
StringBuilder sb = new StringBuilder ("X_C_Subscription[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Subscription Type.
@param C_SubscriptionType_ID Type of subscription */
public void SetC_SubscriptionType_ID (int C_SubscriptionType_ID)
{
if (C_SubscriptionType_ID < 1) throw new ArgumentException ("C_SubscriptionType_ID is mandatory.");
Set_Value ("C_SubscriptionType_ID", C_SubscriptionType_ID);
}
/** Get Subscription Type.
@return Type of subscription */
public int GetC_SubscriptionType_ID() 
{
Object ii = Get_Value("C_SubscriptionType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Due.
@param IsDue Subscription Renewal is Due */
public void SetIsDue (Boolean IsDue)
{
Set_Value ("IsDue", IsDue);
}
/** Get Due.
@return Subscription Renewal is Due */
public Boolean IsDue() 
{
Object oo = Get_Value("IsDue");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
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
/** Set Paid Until.
@param PaidUntilDate Subscription is paid/valid until this date */
public void SetPaidUntilDate (DateTime? PaidUntilDate)
{
if (PaidUntilDate == null) throw new ArgumentException ("PaidUntilDate is mandatory.");
Set_Value ("PaidUntilDate", (DateTime?)PaidUntilDate);
}
/** Get Paid Until.
@return Subscription is paid/valid until this date */
public DateTime? GetPaidUntilDate() 
{
return (DateTime?)Get_Value("PaidUntilDate");
}
/** Set Renewal Date.
@param RenewalDate Renewal Date */
public void SetRenewalDate (DateTime? RenewalDate)
{
if (RenewalDate == null) throw new ArgumentException ("RenewalDate is mandatory.");
Set_Value ("RenewalDate", (DateTime?)RenewalDate);
}
/** Get Renewal Date.
@return Renewal Date */
public DateTime? GetRenewalDate() 
{
return (DateTime?)Get_Value("RenewalDate");
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
}

}
