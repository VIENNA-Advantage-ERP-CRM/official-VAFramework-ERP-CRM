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
/** Generated Model for VAM_Packaging
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_Packaging : PO
{
public X_VAM_Packaging (Context ctx, int VAM_Packaging_ID, Trx trxName) : base (ctx, VAM_Packaging_ID, trxName)
{
/** if (VAM_Packaging_ID == 0)
{
SetDocumentNo (null);
SetVAM_Inv_InOut_ID (0);
SetVAM_Packaging_ID (0);
SetVAM_ShippingMethod_ID (0);
}
 */
}
public X_VAM_Packaging (Ctx ctx, int VAM_Packaging_ID, Trx trxName) : base (ctx, VAM_Packaging_ID, trxName)
{
/** if (VAM_Packaging_ID == 0)
{
SetDocumentNo (null);
SetVAM_Inv_InOut_ID (0);
SetVAM_Packaging_ID (0);
SetVAM_ShippingMethod_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Packaging (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Packaging (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_Packaging (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_Packaging()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380306L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063517L;
/** VAF_TableView_ID=664 */
public static int Table_ID;
 // =664;

/** TableName=VAM_Packaging */
public static String Table_Name="VAM_Packaging";

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
StringBuilder sb = new StringBuilder ("X_VAM_Packaging[").Append(Get_ID()).Append("]");
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
@param VAM_Inv_InOut_ID Material Shipment Document */
public void SetVAM_Inv_InOut_ID (int VAM_Inv_InOut_ID)
{
if (VAM_Inv_InOut_ID < 1) throw new ArgumentException ("VAM_Inv_InOut_ID is mandatory.");
Set_ValueNoCheck ("VAM_Inv_InOut_ID", VAM_Inv_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetVAM_Inv_InOut_ID() 
{
Object ii = Get_Value("VAM_Inv_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Package.
@param VAM_Packaging_ID Shipment Package */
public void SetVAM_Packaging_ID (int VAM_Packaging_ID)
{
if (VAM_Packaging_ID < 1) throw new ArgumentException ("VAM_Packaging_ID is mandatory.");
Set_ValueNoCheck ("VAM_Packaging_ID", VAM_Packaging_ID);
}
/** Get Package.
@return Shipment Package */
public int GetVAM_Packaging_ID() 
{
Object ii = Get_Value("VAM_Packaging_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Freight Carrier.
@param VAM_ShippingMethod_ID Method or manner of product delivery */
public void SetVAM_ShippingMethod_ID (int VAM_ShippingMethod_ID)
{
if (VAM_ShippingMethod_ID < 1) throw new ArgumentException ("VAM_ShippingMethod_ID is mandatory.");
Set_Value ("VAM_ShippingMethod_ID", VAM_ShippingMethod_ID);
}
/** Get Freight Carrier.
@return Method or manner of product delivery */
public int GetVAM_ShippingMethod_ID() 
{
Object ii = Get_Value("VAM_ShippingMethod_ID");
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
/** VAB_DocTypes_ID VAF_Control_Ref_ID=170 */
public static int VAB_DocTypes_ID_VAF_Control_Ref_ID = 170;
/** Set Document Type.
@param VAB_DocTypes_ID Document type or rules */
public void SetVAB_DocTypes_ID(int VAB_DocTypes_ID)
{
    if (VAB_DocTypes_ID < 0) throw new ArgumentException("VAB_DocTypes_ID is mandatory.");
    Set_ValueNoCheck("VAB_DocTypes_ID", VAB_DocTypes_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetVAB_DocTypes_ID()
{
    Object ii = Get_Value("VAB_DocTypes_ID");
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
