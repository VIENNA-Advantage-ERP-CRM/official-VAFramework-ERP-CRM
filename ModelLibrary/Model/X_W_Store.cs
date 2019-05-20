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
/** Generated Model for W_Store
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_Store : PO
{
public X_W_Store (Context ctx, int W_Store_ID, Trx trxName) : base (ctx, W_Store_ID, trxName)
{
/** if (W_Store_ID == 0)
{
SetIsDefault (false);
SetIsMenuAssets (true);	// Y
SetIsMenuContact (true);	// Y
SetIsMenuInterests (true);	// Y
SetIsMenuInvoices (true);	// Y
SetIsMenuOrders (true);	// Y
SetIsMenuPayments (true);	// Y
SetIsMenuRegistrations (true);	// Y
SetIsMenuRequests (true);	// Y
SetIsMenuRfQs (true);	// Y
SetIsMenuShipments (true);	// Y
SetM_PriceList_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetSalesRep_ID (0);
SetURL (null);
SetW_Store_ID (0);
SetWebContext (null);
}
 */
}
public X_W_Store (Ctx ctx, int W_Store_ID, Trx trxName) : base (ctx, W_Store_ID, trxName)
{
/** if (W_Store_ID == 0)
{
SetIsDefault (false);
SetIsMenuAssets (true);	// Y
SetIsMenuContact (true);	// Y
SetIsMenuInterests (true);	// Y
SetIsMenuInvoices (true);	// Y
SetIsMenuOrders (true);	// Y
SetIsMenuPayments (true);	// Y
SetIsMenuRegistrations (true);	// Y
SetIsMenuRequests (true);	// Y
SetIsMenuRfQs (true);	// Y
SetIsMenuShipments (true);	// Y
SetM_PriceList_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetSalesRep_ID (0);
SetURL (null);
SetW_Store_ID (0);
SetWebContext (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Store (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Store (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Store (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_Store()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514385101L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068312L;
/** AD_Table_ID=778 */
public static int Table_ID;
 // =778;

/** TableName=W_Store */
public static String Table_Name="W_Store";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_W_Store[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID)
{
if (C_PaymentTerm_ID <= 0) Set_Value ("C_PaymentTerm_ID", null);
else
Set_Value ("C_PaymentTerm_ID", C_PaymentTerm_ID);
}
/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() 
{
Object ii = Get_Value("C_PaymentTerm_ID");
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
/** Set EMail Footer.
@param EMailFooter Footer added to EMails */
public void SetEMailFooter (String EMailFooter)
{
if (EMailFooter != null && EMailFooter.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
EMailFooter = EMailFooter.Substring(0,2000);
}
Set_Value ("EMailFooter", EMailFooter);
}
/** Get EMail Footer.
@return Footer added to EMails */
public String GetEMailFooter() 
{
return (String)Get_Value("EMailFooter");
}
/** Set EMail Header.
@param EMailHeader Header added to EMails */
public void SetEMailHeader (String EMailHeader)
{
if (EMailHeader != null && EMailHeader.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
EMailHeader = EMailHeader.Substring(0,2000);
}
Set_Value ("EMailHeader", EMailHeader);
}
/** Get EMail Header.
@return Header added to EMails */
public String GetEMailHeader() 
{
return (String)Get_Value("EMailHeader");
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
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Assets.
@param IsMenuAssets Show Menu Assets */
public void SetIsMenuAssets (Boolean IsMenuAssets)
{
Set_Value ("IsMenuAssets", IsMenuAssets);
}
/** Get Menu Assets.
@return Show Menu Assets */
public Boolean IsMenuAssets() 
{
Object oo = Get_Value("IsMenuAssets");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Contact.
@param IsMenuContact Show Menu Contact */
public void SetIsMenuContact (Boolean IsMenuContact)
{
Set_Value ("IsMenuContact", IsMenuContact);
}
/** Get Menu Contact.
@return Show Menu Contact */
public Boolean IsMenuContact() 
{
Object oo = Get_Value("IsMenuContact");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Interests.
@param IsMenuInterests Show Menu Interests */
public void SetIsMenuInterests (Boolean IsMenuInterests)
{
Set_Value ("IsMenuInterests", IsMenuInterests);
}
/** Get Menu Interests.
@return Show Menu Interests */
public Boolean IsMenuInterests() 
{
Object oo = Get_Value("IsMenuInterests");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Invoices.
@param IsMenuInvoices Show Menu Invoices */
public void SetIsMenuInvoices (Boolean IsMenuInvoices)
{
Set_Value ("IsMenuInvoices", IsMenuInvoices);
}
/** Get Menu Invoices.
@return Show Menu Invoices */
public Boolean IsMenuInvoices() 
{
Object oo = Get_Value("IsMenuInvoices");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Orders.
@param IsMenuOrders Show Menu Orders */
public void SetIsMenuOrders (Boolean IsMenuOrders)
{
Set_Value ("IsMenuOrders", IsMenuOrders);
}
/** Get Menu Orders.
@return Show Menu Orders */
public Boolean IsMenuOrders() 
{
Object oo = Get_Value("IsMenuOrders");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Payments.
@param IsMenuPayments Show Menu Payments */
public void SetIsMenuPayments (Boolean IsMenuPayments)
{
Set_Value ("IsMenuPayments", IsMenuPayments);
}
/** Get Menu Payments.
@return Show Menu Payments */
public Boolean IsMenuPayments() 
{
Object oo = Get_Value("IsMenuPayments");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Registrations.
@param IsMenuRegistrations Show Menu Registrations */
public void SetIsMenuRegistrations (Boolean IsMenuRegistrations)
{
Set_Value ("IsMenuRegistrations", IsMenuRegistrations);
}
/** Get Menu Registrations.
@return Show Menu Registrations */
public Boolean IsMenuRegistrations() 
{
Object oo = Get_Value("IsMenuRegistrations");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Requests.
@param IsMenuRequests Show Menu Requests */
public void SetIsMenuRequests (Boolean IsMenuRequests)
{
Set_Value ("IsMenuRequests", IsMenuRequests);
}
/** Get Menu Requests.
@return Show Menu Requests */
public Boolean IsMenuRequests() 
{
Object oo = Get_Value("IsMenuRequests");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu RfQs.
@param IsMenuRfQs Show Menu RfQs */
public void SetIsMenuRfQs (Boolean IsMenuRfQs)
{
Set_Value ("IsMenuRfQs", IsMenuRfQs);
}
/** Get Menu RfQs.
@return Show Menu RfQs */
public Boolean IsMenuRfQs() 
{
Object oo = Get_Value("IsMenuRfQs");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Menu Shipments.
@param IsMenuShipments Show Menu Shipments */
public void SetIsMenuShipments (Boolean IsMenuShipments)
{
Set_Value ("IsMenuShipments", IsMenuShipments);
}
/** Get Menu Shipments.
@return Show Menu Shipments */
public Boolean IsMenuShipments() 
{
Object oo = Get_Value("IsMenuShipments");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID < 1) throw new ArgumentException ("M_PriceList_ID is mandatory.");
Set_Value ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
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

/** SalesRep_ID AD_Reference_ID=190 */
public static int SALESREP_ID_AD_Reference_ID=190;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID < 1) throw new ArgumentException ("SalesRep_ID is mandatory.");
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Stylesheet.
@param Stylesheet CSS (Stylesheet) used */
public void SetStylesheet (String Stylesheet)
{
if (Stylesheet != null && Stylesheet.Length > 60)
{
log.Warning("Length > 60 - truncated");
Stylesheet = Stylesheet.Substring(0,60);
}
Set_Value ("Stylesheet", Stylesheet);
}
/** Get Stylesheet.
@return CSS (Stylesheet) used */
public String GetStylesheet() 
{
return (String)Get_Value("Stylesheet");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennaadvantage.com */
public void SetURL (String URL)
{
if (URL == null) throw new ArgumentException ("URL is mandatory.");
if (URL.Length > 120)
{
log.Warning("Length > 120 - truncated");
URL = URL.Substring(0,120);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
/** Set Web Store EMail.
@param WStoreEMail EMail address used as the sender (From) */
public void SetWStoreEMail (String WStoreEMail)
{
if (WStoreEMail != null && WStoreEMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
WStoreEMail = WStoreEMail.Substring(0,60);
}
Set_Value ("WStoreEMail", WStoreEMail);
}
/** Get Web Store EMail.
@return EMail address used as the sender (From) */
public String GetWStoreEMail() 
{
return (String)Get_Value("WStoreEMail");
}
/** Set WebStore User.
@param WStoreUser User ID of the Web Store EMail address */
public void SetWStoreUser (String WStoreUser)
{
if (WStoreUser != null && WStoreUser.Length > 60)
{
log.Warning("Length > 60 - truncated");
WStoreUser = WStoreUser.Substring(0,60);
}
Set_Value ("WStoreUser", WStoreUser);
}
/** Get WebStore User.
@return User ID of the Web Store EMail address */
public String GetWStoreUser() 
{
return (String)Get_Value("WStoreUser");
}
/** Set WebStore Password.
@param WStoreUserPW Password of the Web Store EMail address */
public void SetWStoreUserPW (String WStoreUserPW)
{
if (WStoreUserPW != null && WStoreUserPW.Length > 20)
{
log.Warning("Length > 20 - truncated");
WStoreUserPW = WStoreUserPW.Substring(0,20);
}
Set_Value ("WStoreUserPW", WStoreUserPW);
}
/** Get WebStore Password.
@return Password of the Web Store EMail address */
public String GetWStoreUserPW() 
{
return (String)Get_Value("WStoreUserPW");
}
/** Set Web Store.
@param W_Store_ID A Web Store of the Client */
public void SetW_Store_ID (int W_Store_ID)
{
if (W_Store_ID < 1) throw new ArgumentException ("W_Store_ID is mandatory.");
Set_ValueNoCheck ("W_Store_ID", W_Store_ID);
}
/** Get Web Store.
@return A Web Store of the Client */
public int GetW_Store_ID() 
{
Object ii = Get_Value("W_Store_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Context.
@param WebContext Web Server Context - e.g. /wstore */
public void SetWebContext (String WebContext)
{
if (WebContext == null) throw new ArgumentException ("WebContext is mandatory.");
if (WebContext.Length > 20)
{
log.Warning("Length > 20 - truncated");
WebContext = WebContext.Substring(0,20);
}
Set_Value ("WebContext", WebContext);
}
/** Get Web Context.
@return Web Server Context - e.g. /wstore */
public String GetWebContext() 
{
return (String)Get_Value("WebContext");
}
/** Set Web Store Info.
@param WebInfo Web Store Header Information */
public void SetWebInfo (String WebInfo)
{
if (WebInfo != null && WebInfo.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebInfo = WebInfo.Substring(0,2000);
}
Set_Value ("WebInfo", WebInfo);
}
/** Get Web Store Info.
@return Web Store Header Information */
public String GetWebInfo() 
{
return (String)Get_Value("WebInfo");
}
/** Set Web Order EMail.
@param WebOrderEMail EMail address to receive notifications when web orders were processed */
public void SetWebOrderEMail (String WebOrderEMail)
{
if (WebOrderEMail != null && WebOrderEMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
WebOrderEMail = WebOrderEMail.Substring(0,60);
}
Set_Value ("WebOrderEMail", WebOrderEMail);
}
/** Get Web Order EMail.
@return EMail address to receive notifications when web orders were processed */
public String GetWebOrderEMail() 
{
return (String)Get_Value("WebOrderEMail");
}
/** Set Web Parameter 1.
@param WebParam1 Web Site Parameter 1 (default: header image) */
public void SetWebParam1 (String WebParam1)
{
if (WebParam1 != null && WebParam1.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam1 = WebParam1.Substring(0,2000);
}
Set_Value ("WebParam1", WebParam1);
}
/** Get Web Parameter 1.
@return Web Site Parameter 1 (default: header image) */
public String GetWebParam1() 
{
return (String)Get_Value("WebParam1");
}
/** Set Web Parameter 2.
@param WebParam2 Web Site Parameter 2 (default index page) */
public void SetWebParam2 (String WebParam2)
{
if (WebParam2 != null && WebParam2.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam2 = WebParam2.Substring(0,2000);
}
Set_Value ("WebParam2", WebParam2);
}
/** Get Web Parameter 2.
@return Web Site Parameter 2 (default index page) */
public String GetWebParam2() 
{
return (String)Get_Value("WebParam2");
}
/** Set Web Parameter 3.
@param WebParam3 Web Site Parameter 3 (default left - menu) */
public void SetWebParam3 (String WebParam3)
{
if (WebParam3 != null && WebParam3.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam3 = WebParam3.Substring(0,2000);
}
Set_Value ("WebParam3", WebParam3);
}
/** Get Web Parameter 3.
@return Web Site Parameter 3 (default left - menu) */
public String GetWebParam3() 
{
return (String)Get_Value("WebParam3");
}
/** Set Web Parameter 4.
@param WebParam4 Web Site Parameter 4 (default footer left) */
public void SetWebParam4 (String WebParam4)
{
if (WebParam4 != null && WebParam4.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam4 = WebParam4.Substring(0,2000);
}
Set_Value ("WebParam4", WebParam4);
}
/** Get Web Parameter 4.
@return Web Site Parameter 4 (default footer left) */
public String GetWebParam4() 
{
return (String)Get_Value("WebParam4");
}
/** Set Web Parameter 5.
@param WebParam5 Web Site Parameter 5 (default footer center) */
public void SetWebParam5 (String WebParam5)
{
if (WebParam5 != null && WebParam5.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam5 = WebParam5.Substring(0,2000);
}
Set_Value ("WebParam5", WebParam5);
}
/** Get Web Parameter 5.
@return Web Site Parameter 5 (default footer center) */
public String GetWebParam5() 
{
return (String)Get_Value("WebParam5");
}
/** Set Web Parameter 6.
@param WebParam6 Web Site Parameter 6 (default footer right) */
public void SetWebParam6 (String WebParam6)
{
if (WebParam6 != null && WebParam6.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WebParam6 = WebParam6.Substring(0,2000);
}
Set_Value ("WebParam6", WebParam6);
}
/** Get Web Parameter 6.
@return Web Site Parameter 6 (default footer right) */
public String GetWebParam6() 
{
return (String)Get_Value("WebParam6");
}
}

}
