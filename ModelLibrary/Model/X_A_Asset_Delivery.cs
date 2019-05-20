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
/** Generated Model for A_Asset_Delivery
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_A_Asset_Delivery : PO
{
public X_A_Asset_Delivery (Context ctx, int A_Asset_Delivery_ID, Trx trxName) : base (ctx, A_Asset_Delivery_ID, trxName)
{
/** if (A_Asset_Delivery_ID == 0)
{
SetA_Asset_Delivery_ID (0);
SetA_Asset_ID (0);
SetMovementDate (DateTime.Now);
}
 */
}
public X_A_Asset_Delivery (Ctx ctx, int A_Asset_Delivery_ID, Trx trxName) : base (ctx, A_Asset_Delivery_ID, trxName)
{
/** if (A_Asset_Delivery_ID == 0)
{
SetA_Asset_Delivery_ID (0);
SetA_Asset_ID (0);
SetMovementDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Delivery (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Delivery (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Asset_Delivery (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_A_Asset_Delivery()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367000L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050211L;
/** AD_Table_ID=541 */
public static int Table_ID;
 // =541;

/** TableName=A_Asset_Delivery */
public static String Table_Name="A_Asset_Delivery";

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
StringBuilder sb = new StringBuilder ("X_A_Asset_Delivery[").Append(Get_ID()).Append("]");
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
/** Set Asset Delivery.
@param A_Asset_Delivery_ID Delivery of Asset */
public void SetA_Asset_Delivery_ID (int A_Asset_Delivery_ID)
{
if (A_Asset_Delivery_ID < 1) throw new ArgumentException ("A_Asset_Delivery_ID is mandatory.");
Set_ValueNoCheck ("A_Asset_Delivery_ID", A_Asset_Delivery_ID);
}
/** Get Asset Delivery.
@return Delivery of Asset */
public int GetA_Asset_Delivery_ID() 
{
Object ii = Get_Value("A_Asset_Delivery_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID < 1) throw new ArgumentException ("A_Asset_ID is mandatory.");
Set_ValueNoCheck ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Delivery Confirmation.
@param DeliveryConfirmation EMail Delivery confirmation */
public void SetDeliveryConfirmation (String DeliveryConfirmation)
{
if (DeliveryConfirmation != null && DeliveryConfirmation.Length > 120)
{
log.Warning("Length > 120 - truncated");
DeliveryConfirmation = DeliveryConfirmation.Substring(0,120);
}
Set_Value ("DeliveryConfirmation", DeliveryConfirmation);
}
/** Get Delivery Confirmation.
@return EMail Delivery confirmation */
public String GetDeliveryConfirmation() 
{
return (String)Get_Value("DeliveryConfirmation");
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
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail = EMail.Substring(0,60);
}
Set_ValueNoCheck ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
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
/** Set Lot No.
@param Lot Lot number (alphanumeric) */
public void SetLot (String Lot)
{
if (Lot != null && Lot.Length > 255)
{
log.Warning("Length > 255 - truncated");
Lot = Lot.Substring(0,255);
}
Set_ValueNoCheck ("Lot", Lot);
}
/** Get Lot No.
@return Lot number (alphanumeric) */
public String GetLot() 
{
return (String)Get_Value("Lot");
}
/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
public void SetM_InOutLine_ID (int M_InOutLine_ID)
{
if (M_InOutLine_ID <= 0) Set_ValueNoCheck ("M_InOutLine_ID", null);
else
Set_ValueNoCheck ("M_InOutLine_ID", M_InOutLine_ID);
}
/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
public int GetM_InOutLine_ID() 
{
Object ii = Get_Value("M_InOutLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Download.
@param M_ProductDownload_ID Product downloads */
public void SetM_ProductDownload_ID (int M_ProductDownload_ID)
{
if (M_ProductDownload_ID <= 0) Set_Value ("M_ProductDownload_ID", null);
else
Set_Value ("M_ProductDownload_ID", M_ProductDownload_ID);
}
/** Get Product Download.
@return Product downloads */
public int GetM_ProductDownload_ID() 
{
Object ii = Get_Value("M_ProductDownload_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Message ID.
@param MessageID EMail Message ID */
public void SetMessageID (String MessageID)
{
if (MessageID != null && MessageID.Length > 120)
{
log.Warning("Length > 120 - truncated");
MessageID = MessageID.Substring(0,120);
}
Set_ValueNoCheck ("MessageID", MessageID);
}
/** Get Message ID.
@return EMail Message ID */
public String GetMessageID() 
{
return (String)Get_Value("MessageID");
}
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
if (MovementDate == null) throw new ArgumentException ("MovementDate is mandatory.");
Set_ValueNoCheck ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetMovementDate().ToString());
}
/** Set Referrer.
@param Referrer Referring web address */
public void SetReferrer (String Referrer)
{
if (Referrer != null && Referrer.Length > 255)
{
log.Warning("Length > 255 - truncated");
Referrer = Referrer.Substring(0,255);
}
Set_ValueNoCheck ("Referrer", Referrer);
}
/** Get Referrer.
@return Referring web address */
public String GetReferrer() 
{
return (String)Get_Value("Referrer");
}
/** Set Remote Addr.
@param Remote_Addr Remote Address */
public void SetRemote_Addr (String Remote_Addr)
{
if (Remote_Addr != null && Remote_Addr.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Addr = Remote_Addr.Substring(0,60);
}
Set_ValueNoCheck ("Remote_Addr", Remote_Addr);
}
/** Get Remote Addr.
@return Remote Address */
public String GetRemote_Addr() 
{
return (String)Get_Value("Remote_Addr");
}
/** Set Remote Host.
@param Remote_Host Remote host Info */
public void SetRemote_Host (String Remote_Host)
{
if (Remote_Host != null && Remote_Host.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Host = Remote_Host.Substring(0,60);
}
Set_ValueNoCheck ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}
/** Set Serial No.
@param SerNo Product Serial Number */
public void SetSerNo (String SerNo)
{
if (SerNo != null && SerNo.Length > 40)
{
log.Warning("Length > 40 - truncated");
SerNo = SerNo.Substring(0,40);
}
Set_ValueNoCheck ("SerNo", SerNo);
}
/** Get Serial No.
@return Product Serial Number */
public String GetSerNo() 
{
return (String)Get_Value("SerNo");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.ViennaAdvnatge.com */
public void SetURL (String URL)
{
if (URL != null && URL.Length > 120)
{
log.Warning("Length > 120 - truncated");
URL = URL.Substring(0,120);
}
Set_ValueNoCheck ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
/** Set Version No.
@param VersionNo Version Number */
public void SetVersionNo (String VersionNo)
{
if (VersionNo != null && VersionNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
VersionNo = VersionNo.Substring(0,20);
}
Set_ValueNoCheck ("VersionNo", VersionNo);
}
/** Get Version No.
@return Version Number */
public String GetVersionNo() 
{
return (String)Get_Value("VersionNo");
}
}

}
