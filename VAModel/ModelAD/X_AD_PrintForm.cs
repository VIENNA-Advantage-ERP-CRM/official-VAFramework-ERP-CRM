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
/** Generated Model for AD_PrintForm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintForm : PO
{
public X_AD_PrintForm (Context ctx, int AD_PrintForm_ID, Trx trxName) : base (ctx, AD_PrintForm_ID, trxName)
{
/** if (AD_PrintForm_ID == 0)
{
SetAD_PrintForm_ID (0);
SetName (null);
}
 */
}
public X_AD_PrintForm (Ctx ctx, int AD_PrintForm_ID, Trx trxName) : base (ctx, AD_PrintForm_ID, trxName)
{
/** if (AD_PrintForm_ID == 0)
{
SetAD_PrintForm_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintForm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintForm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintForm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintForm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362721L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045932L;
/** AD_Table_ID=454 */
public static int Table_ID;
 // =454;

/** TableName=AD_PrintForm */
public static String Table_Name="AD_PrintForm";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_AD_PrintForm[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Form.
@param AD_PrintForm_ID Form */
public void SetAD_PrintForm_ID (int AD_PrintForm_ID)
{
if (AD_PrintForm_ID < 1) throw new ArgumentException ("AD_PrintForm_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintForm_ID", AD_PrintForm_ID);
}
/** Get Print Form.
@return Form */
public int GetAD_PrintForm_ID() 
{
Object ii = Get_Value("AD_PrintForm_ID");
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

/** Inventory_MailText_ID AD_Reference_ID=274 */
public static int INVENTORY_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Inventory Mail Text.
@param Inventory_MailText_ID Email text used for sending physical inventory */
public void SetInventory_MailText_ID (int Inventory_MailText_ID)
{
if (Inventory_MailText_ID <= 0) Set_Value ("Inventory_MailText_ID", null);
else
Set_Value ("Inventory_MailText_ID", Inventory_MailText_ID);
}
/** Get Inventory Mail Text.
@return Email text used for sending physical inventory */
public int GetInventory_MailText_ID() 
{
Object ii = Get_Value("Inventory_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Inventory_PrintFormat_ID AD_Reference_ID=404 */
public static int INVENTORY_PRINTFORMAT_ID_AD_Reference_ID=404;
/** Set Inventory Print Format.
@param Inventory_PrintFormat_ID Print Format for printing Physical Inventory */
public void SetInventory_PrintFormat_ID (int Inventory_PrintFormat_ID)
{
if (Inventory_PrintFormat_ID <= 0) Set_Value ("Inventory_PrintFormat_ID", null);
else
Set_Value ("Inventory_PrintFormat_ID", Inventory_PrintFormat_ID);
}
/** Get Inventory Print Format.
@return Print Format for printing Physical Inventory */
public int GetInventory_PrintFormat_ID() 
{
Object ii = Get_Value("Inventory_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Invoice_MailText_ID AD_Reference_ID=274 */
public static int INVOICE_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Invoice Mail Text.
@param Invoice_MailText_ID Email text used for sending invoices */
public void SetInvoice_MailText_ID (int Invoice_MailText_ID)
{
if (Invoice_MailText_ID <= 0) Set_Value ("Invoice_MailText_ID", null);
else
Set_Value ("Invoice_MailText_ID", Invoice_MailText_ID);
}
/** Get Invoice Mail Text.
@return Email text used for sending invoices */
public int GetInvoice_MailText_ID() 
{
Object ii = Get_Value("Invoice_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Invoice_PrintFormat_ID AD_Reference_ID=261 */
public static int INVOICE_PRINTFORMAT_ID_AD_Reference_ID=261;
/** Set Invoice Print Format.
@param Invoice_PrintFormat_ID Print Format for printing Invoices */
public void SetInvoice_PrintFormat_ID (int Invoice_PrintFormat_ID)
{
if (Invoice_PrintFormat_ID <= 0) Set_Value ("Invoice_PrintFormat_ID", null);
else
Set_Value ("Invoice_PrintFormat_ID", Invoice_PrintFormat_ID);
}
/** Get Invoice Print Format.
@return Print Format for printing Invoices */
public int GetInvoice_PrintFormat_ID() 
{
Object ii = Get_Value("Invoice_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Movement_MailText_ID AD_Reference_ID=274 */
public static int MOVEMENT_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Movement Mail Text.
@param Movement_MailText_ID Email text used for sending Movements */
public void SetMovement_MailText_ID (int Movement_MailText_ID)
{
if (Movement_MailText_ID <= 0) Set_Value ("Movement_MailText_ID", null);
else
Set_Value ("Movement_MailText_ID", Movement_MailText_ID);
}
/** Get Movement Mail Text.
@return Email text used for sending Movements */
public int GetMovement_MailText_ID() 
{
Object ii = Get_Value("Movement_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Movement_PrintFormat_ID AD_Reference_ID=403 */
public static int MOVEMENT_PRINTFORMAT_ID_AD_Reference_ID=403;
/** Set Movement Print Format.
@param Movement_PrintFormat_ID Print Format for using Movements */
public void SetMovement_PrintFormat_ID (int Movement_PrintFormat_ID)
{
if (Movement_PrintFormat_ID <= 0) Set_Value ("Movement_PrintFormat_ID", null);
else
Set_Value ("Movement_PrintFormat_ID", Movement_PrintFormat_ID);
}
/** Get Movement Print Format.
@return Print Format for using Movements */
public int GetMovement_PrintFormat_ID() 
{
Object ii = Get_Value("Movement_PrintFormat_ID");
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

/** Order_MailText_ID AD_Reference_ID=274 */
public static int ORDER_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Order Mail Text.
@param Order_MailText_ID Email text used for sending order acknowledgements or quotations */
public void SetOrder_MailText_ID (int Order_MailText_ID)
{
if (Order_MailText_ID <= 0) Set_Value ("Order_MailText_ID", null);
else
Set_Value ("Order_MailText_ID", Order_MailText_ID);
}
/** Get Order Mail Text.
@return Email text used for sending order acknowledgements or quotations */
public int GetOrder_MailText_ID() 
{
Object ii = Get_Value("Order_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Order_PrintFormat_ID AD_Reference_ID=262 */
public static int ORDER_PRINTFORMAT_ID_AD_Reference_ID=262;
/** Set Order Print Format.
@param Order_PrintFormat_ID Print Format for Orders, Quotes, Offers */
public void SetOrder_PrintFormat_ID (int Order_PrintFormat_ID)
{
if (Order_PrintFormat_ID <= 0) Set_Value ("Order_PrintFormat_ID", null);
else
Set_Value ("Order_PrintFormat_ID", Order_PrintFormat_ID);
}
/** Get Order Print Format.
@return Print Format for Orders, Quotes, Offers */
public int GetOrder_PrintFormat_ID() 
{
Object ii = Get_Value("Order_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Project_MailText_ID AD_Reference_ID=274 */
public static int PROJECT_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Project Mail Text.
@param Project_MailText_ID Standard text for Project EMails */
public void SetProject_MailText_ID (int Project_MailText_ID)
{
if (Project_MailText_ID <= 0) Set_Value ("Project_MailText_ID", null);
else
Set_Value ("Project_MailText_ID", Project_MailText_ID);
}
/** Get Project Mail Text.
@return Standard text for Project EMails */
public int GetProject_MailText_ID() 
{
Object ii = Get_Value("Project_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Project_PrintFormat_ID AD_Reference_ID=402 */
public static int PROJECT_PRINTFORMAT_ID_AD_Reference_ID=402;
/** Set Project Print Format.
@param Project_PrintFormat_ID Standard Project Print Format */
public void SetProject_PrintFormat_ID (int Project_PrintFormat_ID)
{
if (Project_PrintFormat_ID <= 0) Set_Value ("Project_PrintFormat_ID", null);
else
Set_Value ("Project_PrintFormat_ID", Project_PrintFormat_ID);
}
/** Get Project Print Format.
@return Standard Project Print Format */
public int GetProject_PrintFormat_ID() 
{
Object ii = Get_Value("Project_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Remittance_MailText_ID AD_Reference_ID=274 */
public static int REMITTANCE_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Remittance Mail Text.
@param Remittance_MailText_ID Email text used for sending payment remittances */
public void SetRemittance_MailText_ID (int Remittance_MailText_ID)
{
if (Remittance_MailText_ID <= 0) Set_Value ("Remittance_MailText_ID", null);
else
Set_Value ("Remittance_MailText_ID", Remittance_MailText_ID);
}
/** Get Remittance Mail Text.
@return Email text used for sending payment remittances */
public int GetRemittance_MailText_ID() 
{
Object ii = Get_Value("Remittance_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Remittance_PrintFormat_ID AD_Reference_ID=268 */
public static int REMITTANCE_PRINTFORMAT_ID_AD_Reference_ID=268;
/** Set Remittance Print Format.
@param Remittance_PrintFormat_ID Print Format for separate Remittances */
public void SetRemittance_PrintFormat_ID (int Remittance_PrintFormat_ID)
{
if (Remittance_PrintFormat_ID <= 0) Set_Value ("Remittance_PrintFormat_ID", null);
else
Set_Value ("Remittance_PrintFormat_ID", Remittance_PrintFormat_ID);
}
/** Get Remittance Print Format.
@return Print Format for separate Remittances */
public int GetRemittance_PrintFormat_ID() 
{
Object ii = Get_Value("Remittance_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Shipment_MailText_ID AD_Reference_ID=274 */
public static int SHIPMENT_MAILTEXT_ID_AD_Reference_ID=274;
/** Set Shipment Mail Text.
@param Shipment_MailText_ID Email text used for sending delivery notes */
public void SetShipment_MailText_ID (int Shipment_MailText_ID)
{
if (Shipment_MailText_ID <= 0) Set_Value ("Shipment_MailText_ID", null);
else
Set_Value ("Shipment_MailText_ID", Shipment_MailText_ID);
}
/** Get Shipment Mail Text.
@return Email text used for sending delivery notes */
public int GetShipment_MailText_ID() 
{
Object ii = Get_Value("Shipment_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Shipment_PrintFormat_ID AD_Reference_ID=263 */
public static int SHIPMENT_PRINTFORMAT_ID_AD_Reference_ID=263;
/** Set Shipment Print Format.
@param Shipment_PrintFormat_ID Print Format for Shipments, Receipts, Pick Lists */
public void SetShipment_PrintFormat_ID (int Shipment_PrintFormat_ID)
{
if (Shipment_PrintFormat_ID <= 0) Set_Value ("Shipment_PrintFormat_ID", null);
else
Set_Value ("Shipment_PrintFormat_ID", Shipment_PrintFormat_ID);
}
/** Get Shipment Print Format.
@return Print Format for Shipments, Receipts, Pick Lists */
public int GetShipment_PrintFormat_ID() 
{
Object ii = Get_Value("Shipment_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
