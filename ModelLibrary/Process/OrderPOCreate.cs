/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrderPOCreate
 * Purpose        : Generate PO from Sales Order
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     03-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class OrderPOCreate : ProcessEngine.SvrProcess
    {
        #region Private Variables
        //Order Date From	
        private DateTime? _DateOrdered_From = null;
        //Order Date To		
        private DateTime? _DateOrdered_To = null;
        //Customer			
        private int _C_BPartner_ID;
        //Current Vendor			
        private int _Vendor_ID;
        //Sales Order		
        private string _C_Order_ID;
        //Drop Ship			
        private String _IsDropShip;
        // Consolidated PO
        private bool _IsConsolidatedPO = false;
        // list of PO Creation
        List<ConsolidatePO> listConsolidatePO = new List<ConsolidatePO>();
        // list of Consolidate PO Line
        //List<ConsolidatePOLine> listConsolidatePOLine = new List<ConsolidatePOLine>();
        // Display error Message
        StringBuilder messageErrorOrSetting = new StringBuilder();
        // New Vendor
        private int _C_RefVendor_ID;
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("DateOrdered"))
                {
                    _DateOrdered_From = (DateTime?)para[i].GetParameter();
                    _DateOrdered_To = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("Vendor_ID"))
                {
                    _Vendor_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = Util.GetValueOfString(para[i].GetParameter());
                    //acctSchemaRecord = Array.ConvertAll(_C_Order_ID.Split(','), int.Parse);
                    //_C_Order_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("IsDropShip"))
                {
                    _IsDropShip = (String)para[i].GetParameter();
                }
                else if (name.Equals("IsConsolidatePO"))
                {
                    _IsConsolidatedPO = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("C_RefBPartner_ID"))
                {
                    _C_RefVendor_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            log.Info("DateOrdered=" + _DateOrdered_From + " - " + _DateOrdered_To
                + " - C_BPartner_ID=" + _C_BPartner_ID + " - Current Vendor_ID=" + _Vendor_ID
                + " - IsDropShip=" + _IsDropShip + " - C_Order_ID=" + _C_Order_ID + " - Vendor_ID=" + _C_RefVendor_ID);
            if (string.IsNullOrEmpty(_C_Order_ID) && _IsDropShip == null
                && _DateOrdered_From == null && _DateOrdered_To == null
                && _C_BPartner_ID == 0 && _Vendor_ID == 0 && _C_RefVendor_ID == 0)
            {
                throw new Exception("You need to restrict selection");
            }
            // Get Completed Order
            String sql = "SELECT * FROM C_Order o "
                + "WHERE o.IsSOTrx='Y' AND o.IsReturnTrx='N' AND o.IsSalesQuotation = 'N' AND O.DocStatus='" + X_C_Order.DOCACTION_Complete + "'"
                //	No Duplicates
                //	" AND o.Ref_Order_ID IS NULL"
                + " AND NOT EXISTS (SELECT * FROM C_OrderLine ol WHERE o.C_Order_ID=ol.C_Order_ID AND ol.Ref_OrderLine_ID IS NOT NULL)"
                ;
            if (!string.IsNullOrEmpty(_C_Order_ID))
            {
                sql += " AND o.C_Order_ID IN ( " + _C_Order_ID + " ) ";
            }
            else
            {
                if (GetAD_Org_ID() > 0)
                {
                    sql += " AND o.AD_Org_ID=" + GetAD_Org_ID();
                }

                if (_C_BPartner_ID != 0)
                {
                    sql += " AND o.C_BPartner_ID=" + _C_BPartner_ID;
                }
                // Commented by Vivek on 20/09/2017 assigned by Pradeep
                // not to check if dropship is true
                //if (_IsDropShip != null)
                //{
                //    sql += " AND o.IsDropShip='" + _IsDropShip+"'";
                //}

                // Done by Rakesh kumar 25/02/2020
                // When Current vendor selected and vendor not selected
                if (_Vendor_ID != 0 && _C_RefVendor_ID == 0)
                {
                    sql += " AND EXISTS (SELECT * FROM C_OrderLine ol"
                        + " INNER JOIN M_Product_PO po ON (ol.M_Product_ID=po.M_Product_ID) "
                            + "WHERE o.C_Order_ID=ol.C_Order_ID AND po.C_BPartner_ID=" + _Vendor_ID + ")";
                }
                if (_DateOrdered_From != null && _DateOrdered_To != null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') BETWEEN " + GlobalVariable.TO_DATE(_DateOrdered_From, true) + " AND " + GlobalVariable.TO_DATE(_DateOrdered_To, true);
                }
                else if (_DateOrdered_From != null && _DateOrdered_To == null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') >= " + GlobalVariable.TO_DATE(_DateOrdered_From, true);
                }
                else if (_DateOrdered_From == null && _DateOrdered_To != null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') <= " + GlobalVariable.TO_DATE(_DateOrdered_To, true);
                }
            }
            DataTable dt = null;
            IDataReader idr = null;
            int counter = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    counter += CreatePOFromSO(new MOrder(GetCtx(), dr, Get_TrxName()));
                }
                // display price with document no
                if (listConsolidatePO.Count > 0)
                {
                    for (int i = 0; i < listConsolidatePO.Count; i++)
                    {
                        MOrder purchaseOrder = new MOrder(GetCtx(), listConsolidatePO[i].C_Order_ID, Get_Trx());
                        AddLog(0, null, purchaseOrder.GetGrandTotal(), purchaseOrder.GetDocumentNo());
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (counter == 0)
            {
                log.Fine(sql);
            }
            return "@Created@ " + counter + " , " + messageErrorOrSetting;
        }

        /// <summary>
        /// Create PO From SO
        /// </summary>
        /// <param name="so">sales order</param>
        /// <returns>number of POs created basesd on vendor selected or current vendor under product(purchasing tab)</returns>
        private int CreatePOFromSO(MOrder so)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlErrorMessage = new StringBuilder();
            sqlErrorMessage.Clear();
            string _Dropship = "";
            log.Info(so.ToString());
            MOrderLine[] soLines = so.GetLines(true, null);
            if (soLines == null || soLines.Length == 0)
            {
                log.Warning("No Lines - " + so);
                return 0;
            }
            //
            int counter = 0;

            // Changes Done by Rakesh kumar 03/Mar/2021 to fetch record based on vendor selected instead of current vendor
            if (_C_RefVendor_ID > 0)
            {
                //	Get Order Lines with a Product
                sql.Append(@"SELECT DISTINCT OL.M_PRODUCT_ID ,OL.ISDROPSHIP
                FROM C_ORDERLINE OL
                INNER JOIN M_PRODUCT PRD ON (PRD.M_PRODUCT_ID=OL.M_PRODUCT_ID ");

                // Added by Vivek on  20/09/2017 Assigned By Pradeep for drop shipment
                // if drop ship parameter is true then get all records true drop ship lines
                if (_IsDropShip == "Y")
                {
                    sql.Append(@"AND Ol.Isdropship='Y' ");
                }
                // if drop ship parameter is false then get all records false drop ship lines
                else if (_IsDropShip == "N")
                {
                    sql.Append(@"AND Ol.Isdropship='N' ");
                }

                // changes don eby Bharat on 26 June 2018 to handle If purchased Checkbox is false on Finished Good Product, System should not generate Purchase Order.
                sql.Append(@") WHERE ol.C_Order_ID=" + so.GetC_Order_ID() + @" AND prd.IsPurchased='Y'");
                sql.Append(@" ORDER BY ol.Isdropship ");
            }
            else
            {
                //	Order Lines with a Product which has a current vendor 
                sql.Append(@"SELECT DISTINCT po.C_BPartner_ID, po.M_Product_ID ,ol.Isdropship, po.PriceList , po.PricePO , po.C_Currency_ID
                FROM  M_Product_PO po
                INNER JOIN M_Product prd ON po.M_Product_ID=prd.M_Product_ID
                INNER JOIN C_OrderLine ol ON (po.M_Product_ID=ol.M_Product_ID ");       // changes done by bharat on 26 June 2018 If purchased Checkbox is false on Finished Good Product, System should not generate Purchase Order.

                sqlErrorMessage.Append(@"SELECT DISTINCT po.C_BPartner_ID, bp.name AS BPName,  ol.M_Product_ID , p.Name,  ol.Isdropship,  po.C_Currency_ID,  bp.PO_PaymentTerm_ID,  bp.PO_PriceList_ID 
                FROM  C_OrderLine ol INNER JOIN m_product p ON (p.M_Product_ID =ol.M_Product_ID)
                LEFT JOIN M_Product_PO po ON (po.M_Product_ID=ol.M_Product_ID  AND po.isactive = 'Y' AND po.IsCurrentVendor = 'Y' )
                LEFT JOIN c_bpartner bp ON (bp.c_bpartner_id = po.c_bpartner_id ");

                // Added by Vivek on  20/09/2017 Assigned By Pradeep for drop shipment
                // if drop ship parameter is true then get all records true drop ship lines
                if (_IsDropShip == "Y")
                {
                    sql.Append(@"AND Ol.Isdropship='Y' ");
                    sqlErrorMessage.Append(@"AND Ol.Isdropship='Y' ");
                }
                // if drop ship parameter is false then get all records false drop ship lines
                else if (_IsDropShip == "N")
                {
                    sql.Append(@"AND Ol.Isdropship='N' ");
                    sqlErrorMessage.Append(@"AND Ol.Isdropship='N' ");
                }

                // changes don eby Bharat on 26 June 2018 to handle If purchased Checkbox is false on Finished Good Product, System should not generate Purchase Order.
                sql.Append(@") WHERE ol.C_Order_ID=" + so.GetC_Order_ID() + @" AND po.IsCurrentVendor='Y' AND prd.IsPurchased='Y'");
                sqlErrorMessage.Append(@") WHERE ol.C_Order_ID=" + so.GetC_Order_ID());

                if (_Vendor_ID > 0)
                {
                    sql.Append(@" AND po.C_BPartner_ID = " + _Vendor_ID);
                    sqlErrorMessage.Append(@" AND po.C_BPartner_ID = " + _Vendor_ID);
                }
                sql.Append(@" ORDER BY po.c_bpartner_id,ol.Isdropship ");
                sqlErrorMessage.Append(@" ORDER BY po.c_bpartner_id,ol.Isdropship ");

                // get error or setting message
                GetErrorOrSetting(sqlErrorMessage.ToString(), Get_TrxName());
            }


            IDataReader idr = null;
            DataTable dt = null;
            MOrder po = null;
            ConsolidatePO consolidatePO = null;
            //ConsolidatePOLine consolidatePOLine = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int C_BPartner_ID = _C_RefVendor_ID;
                foreach (DataRow dr in dt.Rows)
                {
                    // Changes Done by Rakesh kumar 03/Mar/2021
                    // Assign BPartnerId (VendorId) if only current vendor selected
                    if (_C_RefVendor_ID == 0)
                        C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);

                    // check ANY PO created with same Business Partnet and Drop Shipment
                    if (_IsConsolidatedPO && listConsolidatePO.Count > 0)
                    {
                        ConsolidatePO poRecord;
                        if (listConsolidatePO.Exists(x => (x.C_BPartner_ID == C_BPartner_ID) && (x.IsDropShip == Utility.Util.GetValueOfString(dr["ISDROPSHIP"]))))
                        {
                            poRecord = listConsolidatePO.Find(x => (x.C_BPartner_ID == C_BPartner_ID) && (x.IsDropShip == Utility.Util.GetValueOfString(dr["ISDROPSHIP"])));
                            if (poRecord != null)
                            {
                                po = new MOrder(GetCtx(), poRecord.C_Order_ID, Get_Trx());
                                _Dropship = po.IsDropShip() ? "Y" : "N";
                            }
                        }
                    }

                    // Drop Shipment fucntionality added by Vivek on 20/09/2017 Assigned By Pradeep 
                    if (po == null || po.GetBill_BPartner_ID() != C_BPartner_ID || _Dropship != Utility.Util.GetValueOfString(dr["ISDROPSHIP"]))
                    {
                        // Create PO Header
                        po = CreatePOForVendor(C_BPartner_ID, so, Utility.Util.GetValueOfString(dr["ISDROPSHIP"]));
                        if (po == null)
                            return counter;
                        // AddLog(0, null, null, po.GetDocumentNo());
                        counter++;

                        // maintain list
                        if (po != null && po.GetC_Order_ID() > 0)
                        {
                            consolidatePO = new ConsolidatePO();
                            consolidatePO.C_Order_ID = po.GetC_Order_ID();
                            consolidatePO.C_BPartner_ID = C_BPartner_ID;
                            consolidatePO.IsDropShip = Utility.Util.GetValueOfString(dr["ISDROPSHIP"]);
                            listConsolidatePO.Add(consolidatePO);
                        }
                    }
                    
                    _Dropship = Utility.Util.GetValueOfString(dr["ISDROPSHIP"]);
                    // int M_Product_ID = Utility.Util.GetValueOfInt(dr["M_PRODUCT_ID"]);
                    //	Create PO Line
                    for (int i = 0; i < soLines.Length; i++)
                    {
                        // When Drop ship parameter is yes but SO line does not contains any drop shipment product
                        if (_IsDropShip == "Y" && Util.GetValueOfBool(soLines[i].IsDropShip()) == false)
                        {
                            continue;
                        }
                        // When Drop ship parameter is NO but SO line contains drop shipment product then it also does not generate any 
                        else if (_IsDropShip == "N" && Util.GetValueOfBool(soLines[i].IsDropShip()) == true)
                        {
                            continue;
                        }
                        //When Drop ship parameter is yes and SO line also contains drop shipment product
                        else
                        {
                            String _Drop = "N";
                            if (Util.GetValueOfBool(soLines[i].IsDropShip()))
                            {
                                _Drop = "Y";
                            }
                            if (soLines[i].GetM_Product_ID() == Utility.Util.GetValueOfInt(dr["M_PRODUCT_ID"]) && _Drop == _Dropship)
                            {
                                MOrderLine poLine = new MOrderLine(po);
                                poLine.SetRef_OrderLine_ID(soLines[i].GetC_OrderLine_ID());
                                poLine.SetM_Product_ID(soLines[i].GetM_Product_ID());
                                poLine.SetM_AttributeSetInstance_ID(soLines[i].GetM_AttributeSetInstance_ID());
                                poLine.SetC_UOM_ID(soLines[i].GetC_UOM_ID());
                                poLine.SetQtyEntered(soLines[i].GetQtyEntered());
                                poLine.SetQtyOrdered(soLines[i].GetQtyOrdered());
                                poLine.SetDescription(soLines[i].GetDescription());
                                //poLine.SetDatePromised(soLines[i].GetDatePromised());
                                // Set date promised current date by Rakesh Kumar on 19/Mar/2021 suggested by Mandeep Singh
                                poLine.SetDatePromised(Convert.ToDateTime(DateTime.Now));
                                poLine.SetIsDropShip(soLines[i].IsDropShip());
                                poLine.SetPrice();

                                // Set value in Property From Process to check on Before Save.
                                poLine.SetFromProcess(true);
                                //190 - Set Print Description
                                if (poLine.Get_ColumnIndex("PrintDescription") >= 0)
                                    poLine.Set_Value("PrintDescription", soLines[i].Get_Value("PrintDescription"));

                                if (!poLine.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string msg = string.Empty;
                                    if (pp != null)
                                    {
                                        msg = pp.GetName();
                                        //if GetName is Empty then it will check GetValue
                                        if (string.IsNullOrEmpty(msg))
                                            msg = Msg.GetMsg("", pp.GetValue());
                                    }
                                    if (string.IsNullOrEmpty(msg))
                                        msg = Msg.GetMsg(GetCtx(), "RecordNotSaved");


                                    log.Info("CreatePOfromSO : Not Saved. Error Value : " + msg);
                                    AddLog(0, null, null, msg + " : @DocumentNo@ : " + so.GetDocumentNo());
                                }
                                //else
                                //{
                                //    if (poLine != null && poLine.GetC_OrderLine_ID() > 0)
                                //    {
                                //        consolidatePOLine = new ConsolidatePOLine();
                                //        consolidatePOLine.C_Order_ID = poLine.GetC_Order_ID();
                                //        consolidatePOLine.C_OrderLine_ID = poLine.GetC_OrderLine_ID();
                                //        consolidatePOLine.M_Product_ID = poLine.GetM_Product_ID();
                                //        consolidatePOLine.M_AttributeSetInstance_ID = poLine.GetM_AttributeSetInstance_ID();
                                //        consolidatePOLine.C_UOM_ID = poLine.GetC_UOM_ID();
                                //        consolidatePOLine.IsDropShip = soLines[i].IsDropShip() ? "Y" : "N";
                                //        listConsolidatePOLine.Add(consolidatePOLine);
                                //    }
                                //}
                            }
                        }
                    }

                    if (po.Get_ColumnIndex("ConditionalFlag") > -1)
                    {
                        if (!po.CalculateTaxTotal())   //	setTotals
                        {
                            log.Info(Msg.GetMsg(GetCtx(), "ErrorCalculateTax") + ": " + po.GetDocumentNo().ToString());
                        }

                        // Update order header
                        po.UpdateHeader();

                        DB.ExecuteQuery("UPDATE C_Order SET ConditionalFlag = null WHERE C_Order_ID = " + po.GetC_Order_ID(), null, Get_Trx());
                    }
                }
                //idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //	Set Reference to PO
            if (po != null)
            {
                // Commented by Rakesh Kumar on 03/Mar/2020
                //so.SetRef_Order_ID(po.GetC_Order_ID());
                //so.Save();

                // Update reference of PO in SO By Rakesh Kumar on 03/Mar/2020
                sql.Clear();
                sql.Append("Update C_Order Set REF_ORDER_ID=" + po.GetC_Order_ID() + " Where C_Order_ID=" + so.GetC_Order_ID());
                DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            }
            return counter;
        }

        /// <summary>
        /// Create PO for Vendor
        /// </summary>
        /// <param name="C_BPartner_ID">vendor</param>
        /// <param name="so">sales order</param>
        /// <returns>MOrder</returns>
        public MOrder CreatePOForVendor(int C_BPartner_ID, MOrder so, string _shipDrop)
        {
            MOrder po = new MOrder(GetCtx(), 0, Get_TrxName());
            po.SetClientOrg(so.GetAD_Client_ID(), so.GetAD_Org_ID());
            po.SetRef_Order_ID(so.GetC_Order_ID());
            po.SetIsSOTrx(false);

            // method edited to set unreleased document type for PO
            po.SetC_DocTypeTarget_ID(false);
            //
            po.SetDescription(so.GetDescription());
            po.SetPOReference(so.GetDocumentNo());
            po.SetPriorityRule(so.GetPriorityRule());
            po.SetSalesRep_ID(so.GetSalesRep_ID());
            // Code Commented by Vivek Kumar on 20/09/2017 Assigned By Pradeep for drop shipment
            //po.SetM_Warehouse_ID(so.GetM_Warehouse_ID());
            //	Set Vendor
            MBPartner vendor = new MBPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
            if (Env.IsModuleInstalled("VA009_"))
            {
                // Set PO Payment Method from Vendor
                if (Util.GetValueOfInt(vendor.GetVA009_PO_PaymentMethod_ID()) > 0)
                {
                    po.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(vendor.GetVA009_PO_PaymentMethod_ID()));
                }
                else
                {
                    if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                    {
                        messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_PaymentMethodNotDefined") + " : " + vendor.GetName());
                    }
                    else
                    {
                        messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_PaymentMethodNotDefined") + " : " + vendor.GetName());
                    }
                    po = null;
                    return po;
                }
            }

            //JID_1252: If Vendor do not have Po Pricelist bind. System should give message.
            if (vendor.GetPO_PriceList_ID() > 0)
            {
                po.SetM_PriceList_ID(vendor.GetPO_PriceList_ID());
            }
            else
            {
                if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                {
                    messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_VendorPrcListNotDefine") + " : " + vendor.GetName());
                }
                else
                {
                    messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_VendorPrcListNotDefine") + " : " + vendor.GetName());
                }
                po = null;
                return po;
            }

            // JID_1262: If Payment Term is not bind BP, BP Group and No Default Payment Term. System do not create PO neither give message. 
            if (vendor.GetPO_PaymentTerm_ID() > 0)
            {
                po.SetC_PaymentTerm_ID(vendor.GetPO_PaymentTerm_ID());
            }
            else
            {
                if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                {
                    messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_VendorPaytemNotDefine") + " : " + vendor.GetName());
                }
                else
                {
                    messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_VendorPaytemNotDefine") + " : " + vendor.GetName());
                }
                po = null;
                return po;
            }

            po.SetBPartner(vendor);

            // Code Commented by Vivek Kumar on 20/09/2017 Assigned By Pradeep  for drop shipment
            //	Drop Ship
            //po.SetIsDropShip(so.IsDropShip());
            //if (so.IsDropShip())
            //{
            //    po.SetShip_BPartner_ID(so.GetC_BPartner_ID());
            //    po.SetShip_Location_ID(so.GetC_BPartner_Location_ID());
            //    po.SetShip_User_ID(so.GetAD_User_ID());
            //}

            if (_shipDrop == "Y")
            {
                po.SetIsDropShip(true);
                po.SetShipToPartner_ID(so.GetC_BPartner_ID());
                po.SetShipToLocation_ID(so.GetC_BPartner_Location_ID());
                int _Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select M_WareHouse_ID From M_Warehouse Where AD_Org_ID=" + so.GetAD_Org_ID() + " AND Isdropship='Y' AND IsActive='Y'"));
                if (_Warehouse_ID >= 0)
                {
                    po.SetM_Warehouse_ID(_Warehouse_ID);
                }
            }

            // Added by Bharat on 29 Jan 2018 to set Inco Term from Order

            if (po.Get_ColumnIndex("C_IncoTerm_ID") > 0)
            {
                po.SetC_IncoTerm_ID(so.GetC_IncoTerm_ID());
            }
            //	References
            po.SetC_Activity_ID(so.GetC_Activity_ID());
            po.SetC_Campaign_ID(so.GetC_Campaign_ID());
            po.SetC_Project_ID(so.GetC_Project_ID());
            po.SetUser1_ID(so.GetUser1_ID());
            po.SetUser2_ID(so.GetUser2_ID());            

            //Set VA077 values on header level
            if (Env.IsModuleInstalled("VA077_"))
            {
                //Get the org count of legal entity org
                string sql = @"SELECT Count(AD_Org_ID) FROM AD_Org WHERE IsActive='Y' 
                           AND (IsProfitCenter ='Y' OR IsCostCenter ='Y') AND 
                           AD_Client_Id=" + so.GetAD_Client_ID() + @" AND LegalEntityOrg = " + so.GetAD_Org_ID();
                int result = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (result > 0)
                {
                    po.SetVA077_IsLegalEntity(true);
                }

                /** Updating trx org from so ** Dt:15/04/2021 ** Modified By: Kumar **/
                if (so.GetAD_OrgTrx_ID() > 0)
                {
                    po.SetAD_OrgTrx_ID(so.GetAD_OrgTrx_ID());
                }
            }

            // Set Conditional Flag to skip repeated logic on lines save.
            if (po.Get_ColumnIndex("ConditionalFlag") > -1)
            {
                po.SetConditionalFlag(MOrder.CONDITIONALFLAG_PrepareIt);
            }

            // Handle error done by rakesh kumar on 17/Mar/2021
            if (!po.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                string msg = string.Empty;
                if (pp != null)
                {
                    msg = pp.GetName();
                    //if GetName is Empty then it will check GetValue
                    if (string.IsNullOrEmpty(msg))
                        msg = Msg.GetMsg("", pp.GetValue());
                }
                if (string.IsNullOrEmpty(msg))
                    msg = Msg.GetMsg(GetCtx(), "RecordNotSaved");


                log.Info("CreatePOfromSO : Not Saved. Error Value : " + msg);
                AddLog(0, null, null, msg + " : @DocumentNo@ : " + so.GetDocumentNo());
            }
            return po;
        }

        // getting Error or Setting available for Process
        public void GetErrorOrSetting(String sql, Trx trxName)
        {
            try
            {
                DataSet dsRecod = DB.ExecuteDataset(sql, null, trxName);
                if (dsRecod != null && dsRecod.Tables.Count > 0 && dsRecod.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRecod.Tables[0].Rows.Count; i++)
                    {
                        // check Current vendor available or not
                        if (Util.GetValueOfInt(dsRecod.Tables[0].Rows[i]["C_BPartner_ID"]) == 0)
                        {
                            if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                            {
                                messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_VendorNotFound") + ":" + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["Name"]));
                            }
                            else
                            {
                                messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_VendorNotFound") + ":" + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["Name"]));
                            }
                            continue;
                        }
                        // check Payment term 
                        //if (Util.GetValueOfInt(dsRecod.Tables[0].Rows[i]["PO_PaymentTerm_ID"]) == 0)
                        //{
                        //    if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                        //    {
                        //        messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_VendorPaytemNotDefine") + ":" + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["BPName"]));
                        //    }
                        //    else
                        //    {
                        //        messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_VendorPaytemNotDefine") + ":" + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["BPName"]));
                        //    }
                        //    continue;
                        //}

                        // check price list                        
                        //if (Util.GetValueOfInt(dsRecod.Tables[0].Rows[i]["PO_PriceList_ID"]) == 0)
                        //{
                        //    if (string.IsNullOrEmpty(messageErrorOrSetting.ToString()))
                        //    {
                        //        messageErrorOrSetting.Append(Msg.GetMsg(GetCtx(), "VIS_VendorPrcListNotDefine") + " : " + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["BPName"]));
                        //    }
                        //    else
                        //    {
                        //        messageErrorOrSetting.Append(" , " + Msg.GetMsg(GetCtx(), "VIS_VendorPrcListNotDefine") + " : " + Util.GetValueOfString(dsRecod.Tables[0].Rows[i]["BPName"]));
                        //    }
                        //    continue;
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

    }

    public class ConsolidatePO
    {
        public int C_Order_ID { get; set; }
        public int C_BPartner_ID { get; set; }
        public string IsDropShip { get; set; }
    }

    //public class ConsolidatePOLine
    //{
    //    public int C_Order_ID { get; set; }
    //    public int C_OrderLine_ID { get; set; }
    //    public int M_Product_ID { get; set; }
    //    public int M_AttributeSetInstance_ID { get; set; }
    //    public int C_UOM_ID { get; set; }
    //    public string IsDropShip { get; set; }
    //}
}
