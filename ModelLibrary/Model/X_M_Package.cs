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
/** Generated Model for M_Package
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Package : PO
{
public X_M_Package (Context ctx, int M_Package_ID, Trx trxName) : base (ctx, M_Package_ID, trxName)
{
/** if (M_Package_ID == 0)
{
SetDocumentNo (null);
SetM_InOut_ID (0);
SetM_Package_ID (0);
SetM_Shipper_ID (0);
}
 */
}
public X_M_Package (Ctx ctx, int M_Package_ID, Trx trxName) : base (ctx, M_Package_ID, trxName)
{
/** if (M_Package_ID == 0)
{
SetDocumentNo (null);
SetM_InOut_ID (0);
SetM_Package_ID (0);
SetM_Shipper_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Package (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Package (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Package (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Package()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380306L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063517L;
/** AD_Table_ID=664 */
public static int Table_ID;
 // =664;

/** TableName=M_Package */
public static String Table_Name="M_Package";

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
StringBuilder sb = new StringBuilder ("X_M_Package[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Date received.
@param DateReceived Date a product was received */
public void SetDateReceived (DateTime? DateReceived)
{
Set_Value ("DateReceived", (DateTime?)DateReceived);
}
/** Get Date received.
@return Date a product was received */
public DateTime? GetDateReceived() 
{
return (DateTime?)Get_Value("DateReceived");
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
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_ValueNoCheck ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Set Shipment/Receipt.
@param M_InOut_ID Material Shipment Document */
public void SetM_InOut_ID (int M_InOut_ID)
{
if (M_InOut_ID < 1) throw new ArgumentException ("M_InOut_ID is mandatory.");
Set_ValueNoCheck ("M_InOut_ID", M_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetM_InOut_ID() 
{
Object ii = Get_Value("M_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Package.
@param M_Package_ID Shipment Package */
public void SetM_Package_ID (int M_Package_ID)
{
if (M_Package_ID < 1) throw new ArgumentException ("M_Package_ID is mandatory.");
Set_ValueNoCheck ("M_Package_ID", M_Package_ID);
}
/** Get Package.
@return Shipment Package */
public int GetM_Package_ID() 
{
Object ii = Get_Value("M_Package_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight Carrier.
@param M_Shipper_ID Method or manner of product delivery */
public void SetM_Shipper_ID (int M_Shipper_ID)
{
if (M_Shipper_ID < 1) throw new ArgumentException ("M_Shipper_ID is mandatory.");
Set_Value ("M_Shipper_ID", M_Shipper_ID);
}
/** Get Freight Carrier.
@return Method or manner of product delivery */
public int GetM_Shipper_ID() 
{
Object ii = Get_Value("M_Shipper_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Info Received.
@param ReceivedInfo Information of the receipt of the package (acknowledgement) */
public void SetReceivedInfo (String ReceivedInfo)
{
if (ReceivedInfo != null && ReceivedInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
ReceivedInfo = ReceivedInfo.Substring(0,255);
}
Set_Value ("ReceivedInfo", ReceivedInfo);
}
/** Get Info Received.
@return Information of the receipt of the package (acknowledgement) */
public String GetReceivedInfo() 
{
return (String)Get_Value("ReceivedInfo");
}
/** Set Ship Date.
@param ShipDate Shipment Date/Time */
public void SetShipDate (DateTime? ShipDate)
{
Set_Value ("ShipDate", (DateTime?)ShipDate);
}
/** Get Ship Date.
@return Shipment Date/Time */
public DateTime? GetShipDate() 
{
return (DateTime?)Get_Value("ShipDate");
}
/** Set Tracking Info.
@param TrackingInfo Tracking Info */
public void SetTrackingInfo (String TrackingInfo)
{
if (TrackingInfo != null && TrackingInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
TrackingInfo = TrackingInfo.Substring(0,255);
}
Set_Value ("TrackingInfo", TrackingInfo);
}
/** Get Tracking Info.
@return Tracking Info */
public String GetTrackingInfo() 
{
return (String)Get_Value("TrackingInfo");
}
/** Set IsPackageConfirm.
   @param DTD001_IsPackgConfirm IsPackageConfirm */
public void SetDTD001_IsPackgConfirm(Boolean DTD001_IsPackgConfirm)
{
    Set_Value("DTD001_IsPackgConfirm", DTD001_IsPackgConfirm);
}
/** Get IsPackageConfirm.
@return IsPackageConfirm */
public Boolean IsDTD001_IsPackgConfirm()
{
    Object oo = Get_Value("DTD001_IsPackgConfirm");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** C_DocType_ID AD_Reference_ID=170 */
public static int C_DOCTYPE_ID_AD_Reference_ID = 170;
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID(int C_DocType_ID)
{
    if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
    Set_ValueNoCheck("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID()
{
    Object ii = Get_Value("C_DocType_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Account Date.
   @param DateAcct General Ledger Date */
public void SetDateAcct(DateTime? DateAcct)
{
    Set_Value("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct()
{
    return (DateTime?)Get_Value("DateAcct");
}
/** Set Processed.
   @param Processed The document has been processed */
public void SetProcessed(Boolean Processed)
{
    Set_Value("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed()
{
    Object oo = Get_Value("Processed");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
    /** Set Total Qty.
   @param SAP001_Totalqty Total Qty */
public void SetSAP001_Totalqty(String SAP001_Totalqty)
{
    if (SAP001_Totalqty != null && SAP001_Totalqty.Length > 500)
    {
        log.Warning("Length > 500 - truncated");
        SAP001_Totalqty = SAP001_Totalqty.Substring(0, 500);
    }
    Set_Value("SAP001_Totalqty", SAP001_Totalqty);
}
/** Get Total Qty.
@return Total Qty */
public String GetSAP001_Totalqty()
{
    return (String)Get_Value("SAP001_Totalqty");
}
/** Set Delete Line.
   @param SAP001_DeleteLine Delete Line */
public void SetSAP001_DeleteLine(String SAP001_DeleteLine)
{
    if (SAP001_DeleteLine != null && SAP001_DeleteLine.Length > 2)
    {
        log.Warning("Length > 2 - truncated");
        SAP001_DeleteLine = SAP001_DeleteLine.Substring(0, 2);
    }
    Set_Value("SAP001_DeleteLine", SAP001_DeleteLine);
}
/** Get Delete Line.
@return Delete Line */
public String GetSAP001_DeleteLine()
{
    return (String)Get_Value("SAP001_DeleteLine");
}
/** Set Delivery Note Line.
    @param SAP001_DeliveryNoteL Delivery Note Line */
public void SetSAP001_DeliveryNoteL(String SAP001_DeliveryNoteL)
{
    if (SAP001_DeliveryNoteL != null && SAP001_DeliveryNoteL.Length > 500)
    {
        log.Warning("Length > 500 - truncated");
        SAP001_DeliveryNoteL = SAP001_DeliveryNoteL.Substring(0, 500);
    }
    Set_Value("SAP001_DeliveryNoteL", SAP001_DeliveryNoteL);
}
/** Get Delivery Note Line.
@return Delivery Note Line */
public String GetSAP001_DeliveryNoteL()
{
    return (String)Get_Value("SAP001_DeliveryNoteL");
}
/** Set Copy Lines.
  @param DTD001_CopyLines Copy Lines */
public void SetDTD001_CopyLines(String DTD001_CopyLines)
{
    if (DTD001_CopyLines != null && DTD001_CopyLines.Length > 1)
    {
        log.Warning("Length > 1 - truncated");
        DTD001_CopyLines = DTD001_CopyLines.Substring(0, 1);
    }
    Set_Value("DTD001_CopyLines", DTD001_CopyLines);
}
/** Get Copy Lines.
@return Copy Lines */
public String GetDTD001_CopyLines()
{
    return (String)Get_Value("DTD001_CopyLines");
}

}

}
