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
/** Generated Model for M_Shipper
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Shipper : PO
{
public X_M_Shipper (Context ctx, int M_Shipper_ID, Trx trxName) : base (ctx, M_Shipper_ID, trxName)
{
/** if (M_Shipper_ID == 0)
{
SetM_Shipper_ID (0);
SetName (null);
}
 */
}
public X_M_Shipper (Ctx ctx, int M_Shipper_ID, Trx trxName) : base (ctx, M_Shipper_ID, trxName)
{
/** if (M_Shipper_ID == 0)
{
SetM_Shipper_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Shipper (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Shipper (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Shipper (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Shipper()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381340L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064551L;
/** AD_Table_ID=253 */
public static int Table_ID;
 // =253;

/** TableName=M_Shipper */
public static String Table_Name="M_Shipper";

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
StringBuilder sb = new StringBuilder ("X_M_Shipper[").Append(Get_ID()).Append("]");
return sb.ToString();
}
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
/** Set Freight Carrier.
@param M_Shipper_ID Method or manner of product delivery */
public void SetM_Shipper_ID (int M_Shipper_ID)
{
if (M_Shipper_ID < 1) throw new ArgumentException ("M_Shipper_ID is mandatory.");
Set_ValueNoCheck ("M_Shipper_ID", M_Shipper_ID);
}
/** Get Freight Carrier.
@return Method or manner of product delivery */
public int GetM_Shipper_ID() 
{
Object ii = Get_Value("M_Shipper_ID");
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
/** Set Tracking URL.
@param TrackingURL URL of the shipper to track shipments */
public void SetTrackingURL (String TrackingURL)
{
if (TrackingURL != null && TrackingURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
TrackingURL = TrackingURL.Substring(0,120);
}
Set_Value ("TrackingURL", TrackingURL);
}
/** Get Tracking URL.
@return URL of the shipper to track shipments */
public String GetTrackingURL() 
{
return (String)Get_Value("TrackingURL");
}
}

}
