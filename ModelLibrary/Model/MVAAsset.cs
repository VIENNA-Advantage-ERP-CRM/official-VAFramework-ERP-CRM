/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAsset
 * Purpose        : used for VAA_Asset table
 * Class Used     : X_A_Asset
 * Chronological    Development
 * Raghunandan     08-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Globalization;
//using System.Web.UI;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAAsset : X_VAA_Asset
    {
        #region private variables
        //	Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAAsset).FullName);
        //	Product Info					
        private MVAMProduct _product = null;
        #endregion

        /// <summary>
        /// Get Asset From Shipment.
        /// (Order.reverseCorrect)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Inv_InOutLine_ID">shipment line</param>
        /// <param name="trxName">transaction</param>
        /// <returns>asset or null</returns>
        public static List<MVAAsset> GetFromShipment(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            List<MVAAsset> retValue = new List<MVAAsset>();
            String sql = "SELECT * FROM VAA_Asset WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue.Add(new MVAAsset(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /**
         * 	Create Trial Asset
         *	@param ctx context
         *	@param user user
         *	@param entityType entity type
         *	@return asset or null if no product found
         */
        public static MVAAsset GetTrial(Ctx ctx, MVAFUserContact user, String entityType)
        {
            if (user == null)
            {
                _log.Warning("Cannot create Trial - No User");
                return null;
            }
            if (Utility.Util.IsEmpty(entityType))
            {
                _log.Warning("Cannot create Trial - No Entity Type");
                return null;
            }
            MVAMProduct product = MVAMProduct.GetTrial(ctx, entityType);
            if (product == null)
            {
                _log.Warning("No Trial for Entity Type=" + entityType);
                return null;
            }
            //
            DateTime now = Convert.ToDateTime(CommonFunctions.CurrentTimeMillis());
            //
            MVAAsset asset = new MVAAsset(ctx, 0, null);
            asset.SetClientOrg(user);
            asset.SetAssetServiceDate(now);
            asset.SetIsOwned(false);
            asset.SetIsTrialPhase(true);
            //
            MVABBusinessPartner partner = new MVABBusinessPartner(ctx, user.GetVAB_BusinessPartner_ID(), null);
            String documentNo = "Trial";
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            asset.SetValue(value);
            //	Name		MVAMProduct.afterSave
            String name = "Trial " + partner.GetName() + " - " + product.GetName();
            if (name.Length > 60)
            {
                name = name.Substring(0, 60);
            }
            asset.SetName(name);
            //	Description
            String description = product.GetDescription();
            asset.SetDescription(description);

            //	User
            asset.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());
            asset.SetVAB_BusinessPartner_ID(user.GetVAB_BusinessPartner_ID());
            //	Product
            asset.SetVAM_Product_ID(product.GetVAM_Product_ID());
            asset.SetVAA_AssetGroup_ID(product.GetVAA_AssetGroup_ID());
            asset.SetQty(new Decimal(product.GetSupportUnits()));
            //	Guarantee & Version
            asset.SetGuaranteeDate(TimeUtil.AddDays(now, product.GetTrialPhaseDays()));
            asset.SetVersionNo(product.GetVersionNo());
            //
            return asset;
        }

        /* 	Asset Constructor
     *	@param ctx context
     *	@param VAA_Asset_ID asset
     *	@param trxName transaction name 
     */
        public MVAAsset(Ctx ctx, int VAA_Asset_ID, Trx trxName)
            : base(ctx, VAA_Asset_ID, trxName)
        {

            if (VAA_Asset_ID == 0)
            {
                SetIsDepreciated(false);
                SetIsFullyDepreciated(false);
                SetIsInPosession(false);
                SetIsOwned(false);
                SetIsDisposed(false);
                SetVAM_PFeature_SetInstance_ID(0);
                SetQty(Env.ONE);
                SetIsTrialPhase(false);
            }
        }

        /**
         * 	Discontinued Asset Constructor - DO NOT USE (but don't delete either)
         *	@param ctx context
         *	@param VAA_Asset_ID asset
         *	@deprecated
         */
        public MVAAsset(Ctx ctx, int VAA_Asset_ID)
            : this(ctx, VAA_Asset_ID, null)
        {

        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param dr result set record
         *	@param trxName transaction
         */
        public MVAAsset(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MVAAsset(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /**
        * 	Shipment Constructor
        * 	@param shipment shipment
        *	@param shipLine shipment line
        *	@param deliveryCount 0 or number of delivery
        */
        public MVAAsset(MVAMInvInOut shipment, MVAMInvInOutLine shipLine, int deliveryCount)
            : this(shipment.GetCtx(), 0, shipment.Get_TrxName())
        {

            SetClientOrg(shipment);

            SetValueNameDescription(shipment, shipLine, deliveryCount);
            //	Header

            // SetIsOwned(true);
            SetVAB_BusinessPartner_ID(shipment.GetVAB_BusinessPartner_ID());
            SetVAB_BPart_Location_ID(shipment.GetVAB_BPart_Location_ID());
            SetVAF_UserContact_ID(shipment.GetVAF_UserContact_ID());
            SetVAM_Locator_ID(shipLine.GetVAM_Locator_ID());
            SetIsInPosession(true);
            SetAssetServiceDate(shipment.GetDateAcct());

            //	Line
            MVAMProduct product = shipLine.GetProduct();
            SetVAM_Product_ID(product.GetVAM_Product_ID());
            SetVAA_AssetGroup_ID(product.GetVAA_AssetGroup_ID());

            //////////////////////////////*
            //Changes for vafam
            // SetAssetServiceDate(shipment.GetMovementDate());
            //SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            MVAAAssetGroup _assetGroup = new MVAAAssetGroup(GetCtx(), GetVAA_AssetGroup_ID(), shipment.Get_TrxName());
            if (_assetGroup.IsOwned())
            {
                SetIsOwned(true);
                //SetVAB_BusinessPartner_ID(0);
            }
            if (_assetGroup.IsDepreciated())
            {
                SetIsDepreciated(true);
                SetIsFullyDepreciated(false);
            }

            //Change by Sukhwinder for setting Asset type and amortization template on Asset window, MANTIS ID:1762
            int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
            int countVAFAM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VAFAM_' "));
            if (countVA038 > 0)
            {
                Set_Value("VA038_AmortizationTemplate_ID", Utility.Util.GetValueOfInt(_assetGroup.Get_Value("VA038_AmortizationTemplate_ID")));
            }
            if (countVAFAM > 0)
            {
                Set_Value("VAFAM_AssetType", _assetGroup.Get_Value("VAFAM_AssetType").ToString());
                Set_Value("VAFAM_DepreciationType_ID", Utility.Util.GetValueOfInt(_assetGroup.Get_Value("VAFAM_DepreciationType_ID")));

            }
            ////////////////////////////////////


            //	Guarantee & Version
            SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            SetVersionNo(product.GetVersionNo());
            if (shipLine.GetVAM_PFeature_SetInstance_ID() != 0)		//	Instance
            {
                MVAMPFeatureSetInstance asi = new MVAMPFeatureSetInstance(GetCtx(), shipLine.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                SetVAM_PFeature_SetInstance_ID(asi.GetVAM_PFeature_SetInstance_ID());
                SetLot(asi.GetLot());
                SetSerNo(asi.GetSerNo());
            }
            SetHelp(shipLine.GetDescription());
            //	Qty
            int units = product.GetSupportUnits();
            if (units == 0)
                units = 1;
            if (deliveryCount != 0)		//	one asset per UOM
                SetQty(shipLine.GetMovementQty(), units);
            else
                SetQty((Decimal)units);
            SetVAM_Inv_InOutLine_ID(shipLine.GetVAM_Inv_InOutLine_ID());

            //	Activate
            MVAAAssetGroup ag = MVAAAssetGroup.Get(GetCtx(), GetVAA_AssetGroup_ID());
            if (!ag.IsCreateAsActive())
                SetIsActive(false);
        }

        /**
        * 	Set Value Name Description
        *	@param shipment shipment
        *	@param line line
        *	@param deliveryCount
        */
        public void SetValueNameDescription(MVAMInvInOut shipment, MVAMInvInOutLine line, int deliveryCount)
        {
            MVAMProduct product = line.GetProduct();
            MVABBusinessPartner partner = shipment.GetBPartner();
            SetValueNameDescription(shipment, deliveryCount, product, partner);
        }

        /**
        * 	Set Value, Name, Description
        *	@param shipment shipment
        *	@param deliveryCount count
        *	@param product product
        *	@param partner partner
        */
        public void SetValueNameDescription(MVAMInvInOut shipment,
            int deliveryCount, MVAMProduct product, MVABBusinessPartner partner)
        {
            String documentNo = "_" + shipment.GetDocumentNo();
            if (deliveryCount > 1)
                documentNo += "_" + deliveryCount;
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            // Change to set Value from Document Sequence
            // SetValue(value);

            // Change to set only name of product as value in Asset 
            //	Name		MVAMProduct.afterSave
            // String name = partner.GetName() + " - " + product.GetName();

            String name = product.GetName();
            if (name.Length > 60)
                name = name.Substring(0, 60);
            SetName(name);
            //	Description
            String description = product.GetDescription();
            SetDescription(description);
        }

        /**
       * 	Add to Description
       *	@param description text
       */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        // Create Asset From Invoice Mohit 
        /**
        * 	Shipment Constructor
        * 	@param Invoice
        *	@param shipLine shipment line
        *	@param deliveryCount 0 or number of delivery
        */
        public MVAAsset(MVABInvoice invoice, MVABInvoiceLine invoiceline, int deliveryCount)
            : this(invoiceline.GetCtx(), 0, invoiceline.Get_TrxName())
        {

            SetClientOrg(invoiceline);

            SetValueNameDescription(invoice, invoiceline, deliveryCount);
            //	Header

            //SetIsOwned(true);
            SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());
            SetVAB_BPart_Location_ID(invoice.GetVAB_BPart_Location_ID());
            SetVAF_UserContact_ID(invoice.GetVAF_UserContact_ID());
            //SetVAM_Locator_ID(invoice.GetVAM_Locator_ID());
            SetIsInPosession(true);


            SetAssetServiceDate(invoice.GetDateAcct());


            //	Line
            MVAMProduct product = invoiceline.GetProduct();
            SetVAM_Product_ID(product.GetVAM_Product_ID());
            SetVAA_AssetGroup_ID(product.GetVAA_AssetGroup_ID());

            //////////////////////////////*
            //Changes for vafam
            // SetAssetServiceDate(shipment.GetMovementDate());
            //SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            MVAAAssetGroup _assetGroup = new MVAAAssetGroup(GetCtx(), GetVAA_AssetGroup_ID(), invoice.Get_TrxName());
            if (_assetGroup.IsOwned())
            {
                SetIsOwned(true);
                //SetVAB_BusinessPartner_ID(0);
            }
            if (_assetGroup.IsDepreciated())
            {
                SetIsDepreciated(true);
                SetIsFullyDepreciated(false);
            }
            ////////////////////////////////////
            //Change by Sukhwinder for setting Asset type and amortization template on Asset window, MANTIS ID:1762
            int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
            int countVAFAM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VAFAM_' "));

            if (countVA038 > 0)
            {
                Set_Value("VA038_AmortizationTemplate_ID", Utility.Util.GetValueOfInt(_assetGroup.Get_Value("VA038_AmortizationTemplate_ID")));
            }
            if (countVAFAM > 0)
            {
                Set_Value("VAFAM_AssetType", _assetGroup.Get_Value("VAFAM_AssetType").ToString());
                Set_Value("VAFAM_DepreciationType_ID", Utility.Util.GetValueOfInt(_assetGroup.Get_Value("VAFAM_DepreciationType_ID")));
            }

            ////////////////////////////////////

            //	Guarantee & Version
            SetGuaranteeDate(TimeUtil.AddDays(invoice.GetDateInvoiced(), product.GetGuaranteeDays()));
            SetVersionNo(product.GetVersionNo());
            if (invoiceline.GetVAM_PFeature_SetInstance_ID() != 0)		//	Instance
            {
                MVAMPFeatureSetInstance asi = new MVAMPFeatureSetInstance(GetCtx(), invoiceline.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                SetVAM_PFeature_SetInstance_ID(asi.GetVAM_PFeature_SetInstance_ID());
                SetLot(asi.GetLot());
                SetSerNo(asi.GetSerNo());
            }
            SetHelp(invoiceline.GetDescription());
            //	Qty
            int units = product.GetSupportUnits();
            if (units == 0)
                units = 1;
            if (deliveryCount != 0)		//	one asset per UOM
                SetQty(invoiceline.GetQtyEntered(), units);
            else
                SetQty((Decimal)units);
            SetVAM_Inv_InOutLine_ID(invoiceline.GetVAM_Inv_InOutLine_ID());
            Set_Value("VAB_InvoiceLine_ID", invoiceline.GetVAB_InvoiceLine_ID());

            //	Activate
            MVAAAssetGroup ag = MVAAAssetGroup.Get(GetCtx(), GetVAA_AssetGroup_ID());
            if (!ag.IsCreateAsActive())
                SetIsActive(false);
        }
        /**
        * 	Set Value Name Description
        *	@param Invoice
        *	@param line line
        *	@param deliveryCount
        */
        public void SetValueNameDescription(MVABInvoice invoice, MVABInvoiceLine line, int deliveryCount)
        {
            MVAMProduct product = line.GetProduct();
            MVABBusinessPartner partner = new MVABBusinessPartner(GetCtx(), invoice.GetVAB_BusinessPartner_ID(), null);
            SetValueNameDescription(invoice, deliveryCount, product, partner);
        }

        /**
        * 	Set Value, Name, Description
        *	@param invoice
        *	@param deliveryCount count
        *	@param product product
        *	@param partner partner
        */
        public void SetValueNameDescription(MVABInvoice invoice,
            int deliveryCount, MVAMProduct product, MVABBusinessPartner partner)
        {
            String documentNo = "_" + invoice.GetDocumentNo();
            if (deliveryCount > 1)
                documentNo += "_" + deliveryCount;
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            // Change to set Value from Document Sequence
            // SetValue(value);

            // Change to set only name of product as value in Asset 
            //	Name		MVAMProduct.afterSave
            // String name = partner.GetName() + " - " + product.GetName();

            String name = product.GetName();
            if (name.Length > 60)
                name = name.Substring(0, 60);
            SetName(name);
            //	Description
            String description = product.GetDescription();
            SetDescription(description);
        }
        // End---------------

        /* 	Get Qty
	 *	@return 1 or Qty
	 */
        public new Decimal GetQty()
        {
            Decimal qty = base.GetQty();
            // In Case of Disposal, no need to set Asset Qty to One.
            if (qty.Equals(Env.ZERO) && !IsDisposed())
                SetQty(Env.ONE);
            return base.GetQty();
        }

        /**
         * 	Set Qty
         *	@param Qty quantity
         *	@param multiplier support units
         */
        public void SetQty(Decimal Qty, int multiplier)
        {
            if (multiplier == 0)
                multiplier = 1;
            Decimal mm = new Decimal(multiplier);
            base.SetQty(Decimal.Multiply(Qty, mm));
        }

        /**
         * 	Set Qty based on product * shipment line if exists
         */
        public void SetQty()
        {
            //	UPDATE VAM_Product SET SupportUnits=1 WHERE SupportUnits IS NULL OR SupportUnits<1;
            //	UPDATE VAA_Asset a SET Qty = (SELECT l.MovementQty * p.SupportUnits FROM VAM_Inv_InOutLine l, VAM_Product p WHERE a.VAM_Inv_InOutLine_ID=l.VAM_Inv_InOutLine_ID AND a.VAM_Product_ID=p.VAM_Product_ID) WHERE a.VAM_Product_ID IS NOT NULL AND a.VAM_Inv_InOutLine_ID IS NOT NULL;
            Decimal Qty = Env.ONE;
            if (GetVAM_Inv_InOutLine_ID() != 0)
            {
                MVAMInvInOutLine line = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                Qty = line.GetMovementQty();
            }
            int multiplier = GetProduct().GetSupportUnits();
            Decimal mm = new Decimal(multiplier);
            base.SetQty(Decimal.Multiply(Qty, mm));
        }

        /**
         * 	String representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAsset[")
                .Append(Get_ID())
                .Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }

        /* 	Get Deliveries
    * 	@return deliveries
    */
        public MVAAAssetDelivery[] GetDeliveries()
        {
            List<MVAAAssetDelivery> list = new List<MVAAAssetDelivery>();

            String sql = "SELECT * FROM VAA_AssetDelivery WHERE VAA_Asset_ID=" + GetA_Asset_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAAAssetDelivery(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            //
            MVAAAssetDelivery[] retValue = new MVAAAssetDelivery[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Delivery count
         * 	@return delivery count
         */
        public int GetDeliveryCount()
        {
            String sql = "SELECT COUNT(*) FROM VAA_AssetDelivery WHERE VAA_Asset_ID=" + GetA_Asset_ID();
            return Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /*
         * 	Can we download.
         * 	Based on guarantee date and availability of download
         * 	@return true if downloadable
         */
        public bool IsDownloadable()
        {
            if (IsActive(true))
            {
                GetProduct();
                return _product != null
                    && _product.HasDownloads();
            }
            //
            return false;
        }	//	isDownloadable

        /**
         * 	Is Active 
         *	@param checkDate check guarantee date
         *	@return true if active and within guarantee
         */
        public bool IsActive(bool checkDate)
        {
            if (!checkDate)
                return IsActive();
            if (!IsActive())
                return false;

            //	Guarantee Date
            DateTime? guarantee = GetGuaranteeDate();
            if (guarantee == null)
            {
                return false;
            }
            guarantee = TimeUtil.GetDay(guarantee);
            DateTime now = TimeUtil.GetDay(DateTime.Now);
            //	valid
            //return !now.after(guarantee);	//	not after guarantee date
            return !(now > guarantee);	//	not after guarantee date
        }

        /*
         * 	Get Product Version No
         *	@return VersionNo
         */
        public String GetProductVersionNo()
        {
            return GetProduct().GetVersionNo();
        }

        /**
         * 	Get Product VAR_MailTemplate_ID
         *	@return VAR_MailTemplate_ID
         */
        public int GetProductVAR_MailTemplate_ID()
        {
            return GetProduct().GetVAR_MailTemplate_ID();
        }

        /**
         * 	Get Product Info
         * 	@return product
         */
        private MVAMProduct GetProduct()
        {
            if (_product == null)
                _product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());
            return _product;
        }

        /* 	Get Active Addl. Product Downloads
        *	@return array of downloads
        */
        public MVAMProductDownload[] GetProductDownloads()
        {
            if (_product == null)
                GetProduct();
            if (_product != null)
                return _product.GetProductDownloads(false);
            return null;
        }

        /**
         * 	Get Additional Download Names
         *	@return names
         */
        public String[] GetDownloadNames()
        {
            MVAMProductDownload[] dls = GetProductDownloads();
            if (dls != null && dls.Length > 0)
            {
                String[] retValue = new String[dls.Length];
                for (int i = 0; i < retValue.Length; i++)
                    retValue[i] = dls[i].GetName();
                log.Fine("#" + dls.Length);
                return retValue;
            }
            return new String[] { };
        }

        /**
         * 	Get Additional Download URLs
         *	@return URLs
         */
        public String[] GetDownloadURLs()
        {
            MVAMProductDownload[] dls = GetProductDownloads();
            if (dls != null && dls.Length > 0)
            {
                String[] retValue = new String[dls.Length];
                for (int i = 0; i < retValue.Length; i++)
                {
                    String url = dls[i].GetDownloadURL();
                    int pos = Math.Max(url.LastIndexOf('/'), url.LastIndexOf('\\'));
                    if (pos != -1)
                        url = url.Substring(pos + 1);
                    retValue[i] = url;
                }
                return retValue;
            }
            return new String[] { };
        }

        /**
         * 	Get Asset Group
         *	@return asset Group
         */
        public MVAAAssetGroup GetAssetGroup()
        {
            return MVAAAssetGroup.Get(GetCtx(), GetVAA_AssetGroup_ID());
        }

        /**
         * 	Get SupportLevel
         *	@return support level or Unsupported
         */
        public String GetSupportLevel()
        {
            return GetAssetGroup().GetSupportLevel();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            GetQty();		//	set to 1
            MVAAAssetGroup astGrp = new MVAAAssetGroup(GetCtx(), GetVAA_AssetGroup_ID(), Get_TrxName());
            if (newRecord && astGrp.Get_ColumnIndex("VAM_CtlSerialNo_ID") > 0)
            {
                string name = "";
                MVAMCtlSerialNo ctl = new MVAMCtlSerialNo(GetCtx(), astGrp.GetVAM_CtlSerialNo_ID(), Get_TrxName());

                // if Organization level check box is true on Serila No Control, then Get Current next from Serila No tab.
                if (ctl.Get_ColumnIndex("IsOrgLevelSequence") >= 0)
                {
                    name = ctl.CreateDefiniteSerNo(this);
                }
                else
                {
                    name = ctl.CreateSerNo();
                }
                SetValue(name);
            }
            #region Fixed Asset Management
            /*to Set Written Down Value on Asset i.e. Gross value of Asset-Depreciated/ Amortized Value Arpit*/
            if (Util.GetValueOfInt(DB.ExecuteScalar("select VAF_ModuleInfo_id from VAF_ModuleInfo where prefix='VAFAM_' and isactive='Y'")) > 0)
            {
                if (Get_ColumnIndex("VAFAM_WrittenDownValue") > 0 &&
                    Get_ColumnIndex("VAFAM_AssetGrossValue") > 0 && Get_ColumnIndex("VAFAM_SLMDepreciation") > 0)
                {
                    Set_Value("VAFAM_WrittenDownValue",
                        Decimal.Subtract(Util.GetValueOfDecimal(Get_Value("VAFAM_AssetGrossValue")),
                        Util.GetValueOfDecimal(Get_Value("VAFAM_SLMDepreciation"))));
                }
            }
            #endregion 

            return true;
        }
        /// <summary>
        /// After save logic for asset
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //Cost Code Commented - As not required on Asset Save
            //if (newRecord)
            //{
            //    UpdateAssetCost();
            //}

            // create default Account
            StringBuilder _sql = new StringBuilder("");
            // check table exist or not
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Asset_Group_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Asset_Group_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                PO obj = null;
                int assetId = GetA_Asset_ID();
                int assetGroupId = GetVAA_AssetGroup_ID();
                // get related to value agaisnt asset = 75
                string sql = "SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where   r.name='FRPT_RelatedTo' and l.name='Asset'";
                string _RelatedToProduct = Convert.ToString(DB.ExecuteScalar(sql));

                _sql.Clear();
                _sql.Append("Select Count(*) From FRPT_Asset_Acct   where VAA_Asset_ID=" + assetId + " AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (value < 1)
                {
                    _sql.Clear();
                    _sql.Append(@"Select  PCA.VAB_AccountBook_id, PCA.VAB_Acct_ValidParameter_id, PCA.frpt_acctdefault_id " +
                        " From FRPT_Asset_Group_Acct PCA " +
                        " inner join frpt_acctdefault ACC ON acc.frpt_acctdefault_id= PCA.frpt_acctdefault_id " +
                        " where PCA.VAA_AssetGroup_ID=" + assetGroupId +
                        " and acc.frpt_relatedto='" + _RelatedToProduct +
                        "' AND PCA.IsActive = 'Y' AND PCA.VAF_Client_ID = " + GetVAF_Client_ID());

                    DataSet ds = DB.ExecuteDataset(_sql.ToString());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            obj = MVAFTableView.GetPO(GetCtx(), "FRPT_Asset_Acct", 0, null);
                            obj.Set_ValueNoCheck("VAF_Org_ID", 0);
                            obj.Set_ValueNoCheck("VAA_Asset_ID", assetId);
                            obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]));
                            obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_ID"]));
                            obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                            if (!obj.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Log(Level.SEVERE, "Could Not create FRPT_Asset_Acct. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                            }
                        }
                    }
                }
            }

            return true;
        }

        /**
         * 	Confirm Asset EMail Delivery
         *	@param email email sent
         * 	@param VAF_UserContact_ID recipient
         * 	@return asset delivery
         */
        public MVAAAssetDelivery ConfirmDelivery(EMail email, int VAF_UserContact_ID)
        {
            SetVersionNo(GetProductVersionNo());
            MVAAAssetDelivery ad = new MVAAAssetDelivery(this, email, null, VAF_UserContact_ID);
            return ad;
        }

        /* 	Confirm Asset Download Delivery
        *	@param request request
        * 	@param VAF_UserContact_ID recipient
        * 	@return asset delivery
        */
        //public MAssetDelivery ConfirmDelivery(HttpServletRequest request, int VAF_UserContact_ID)
        //{
        //    SetVersionNo(GetProductVersionNo());
        //    SetLifeUseUnits(GetLifeUseUnits().add(Env.ONE));
        //    MAssetDelivery ad = new MAssetDelivery(this, request, VAF_UserContact_ID);
        //    return ad;
        //}



        private void UpdateAssetCost()
        {
            //Cost Code Commented - As not required on Asset Save
            //Pratap- For Asset Cost
            //            #region Update Asset Cost

            //            if (GetVAM_Product_ID() > 0)
            //            {
            //                DataSet _dsAsset = null;
            //                StringBuilder _sql = new StringBuilder();
            //                _sql.Clear();
            //                _sql.Clear();
            //                _sql.Append(@" SELECT VAM_ProductCost.VAF_CLIENT_ID,
            //                                          VAM_ProductCost.VAF_ORG_ID,
            //                                          VAM_ProductCost.VAM_ProductCostElement_ID,
            //                                          VAM_Product.VAM_Product_ID,
            //                                          VAM_ProductCost.VAB_ACCOUNTBOOK_ID,
            //                                          VAM_ProductCost.VAM_ProductCostType_ID,
            //                                          VAM_ProductCost.BASISTYPE,
            //                                          VAM_Product.VAB_UOM_ID,
            //                                          VAM_ProductCostElement.COSTINGMETHOD,
            //                                          VAM_ProductCost.VAM_PFeature_SetInstance_ID,
            //                                          VAM_ProductCost.CURRENTCOSTPRICE,
            //                                          VAM_ProductCost.FUTURECOSTPRICE
            //                                        FROM VAM_ProductCost
            //                                        INNER JOIN VAM_Product
            //                                        ON(VAM_Product.VAM_Product_ID=VAM_ProductCost.VAM_Product_ID)
            //                                        INNER JOIN VAM_ProductCostElement
            //                                        ON(VAM_ProductCostElement.VAM_ProductCostElement_ID  =VAM_ProductCost.VAM_ProductCostElement_ID)
            //                                        WHERE VAM_ProductCostElement.COSTELEMENTTYPE='M'
            //                                        AND VAM_ProductCost.VAF_CLIENT_ID            =" + GetVAF_Client_ID() +
            //                            @" AND VAM_ProductCost.ISACTIVE                ='Y' AND VAM_ProductCost.IsAssetCost='N'
            //                                        AND VAM_Product.VAM_Product_ID         =" + GetVAM_Product_ID());

            //                _dsAsset = DB.ExecuteDataset(_sql.ToString());
            //                _sql.Clear();
            //                if (_dsAsset != null)
            //                {
            //                    if (_dsAsset.Tables[0].Rows.Count > 0)
            //                    {
            //                        for (int k = 0; k < _dsAsset.Tables[0].Rows.Count; k++)
            //                        {
            //                            MVAMInvInOutLine _inOutLine = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
            //                            MVAMProduct _product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());
            //                            MVAMVAMProductCostElement _costElement = new MVAMVAMProductCostElement(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_ProductCostElement_ID"]), Get_TrxName());
            //                            MAcctSchema _acctsch = null;
            //                            MVAMVAMProductCost _cost = null;
            //                            if (GetVAM_Inv_InOutLine_ID() > 0)
            //                            {
            //                                if (_inOutLine.GetVAB_OrderLine_ID() > 0)
            //                                {
            //                                    if (_costElement.IsLastPOPrice() || _costElement.IsAveragePO())
            //                                    {
            //                                        _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAB_ACCOUNTBOOK_ID"]), Get_TrxName());
            //                                        _cost = new MVAMVAMProductCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_PFeature_SetInstance_ID"]),
            //                                            _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAF_ORG_ID"]),
            //                                            Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_ProductCostElement_ID"]),
            //                                            GetA_Asset_ID());
            //                                        _cost.SetBasisType(Util.GetValueOfString(_dsAsset.Tables[0].Rows[k]["BASISTYPE"]));
            //                                        _cost.SetCurrentCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["CURRENTCOSTPRICE"]));
            //                                        _cost.SetFutureCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["FUTURECOSTPRICE"]));
            //                                        _cost.SetIsAssetCost(true);
            //                                        _cost.SetCurrentQty(GetQty());
            //                                        if (!_cost.Save(Get_TrxName()))
            //                                        {
            //                                            _log.Info("Asset Cost Line Not Saved For Asset " + GetValue());
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    if (_costElement.IsFifo() || _costElement.IsLifo() || _costElement.IsStandardCosting() || _costElement.IsAveragePO() || _costElement.IsAverageInvoice())
            //                                    {
            //                                        _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAB_ACCOUNTBOOK_ID"]), Get_TrxName());
            //                                        _cost = new MVAMVAMProductCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_PFeature_SetInstance_ID"]),
            //                                            _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAF_ORG_ID"]),
            //                                            Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_ProductCostElement_ID"]),
            //                                            GetA_Asset_ID());
            //                                        _cost.SetBasisType(Util.GetValueOfString(_dsAsset.Tables[0].Rows[k]["BASISTYPE"]));
            //                                        _cost.SetCurrentCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["CURRENTCOSTPRICE"]));
            //                                        _cost.SetFutureCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["FUTURECOSTPRICE"]));
            //                                        _cost.SetIsAssetCost(true);
            //                                        _cost.SetCurrentQty(GetQty());
            //                                        if (!_cost.Save(Get_TrxName()))
            //                                        {
            //                                            _log.Info("Asset Cost Line Not Saved For Asset " + GetValue());
            //                                        }
            //                                    }

            //                                }

            //                            }
            //                            else
            //                            {
            //                                if (_costElement.IsLastPOPrice() || _costElement.IsAveragePO() || _costElement.IsFifo() || _costElement.IsLifo() || _costElement.IsStandardCosting() || _costElement.IsLastInvoice() || _costElement.IsAverageInvoice())
            //                                {
            //                                    _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAB_ACCOUNTBOOK_ID"]), Get_TrxName());
            //                                    _cost = new MVAMVAMProductCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_PFeature_SetInstance_ID"]),
            //                                        _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAF_ORG_ID"]),
            //                                        Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["VAM_ProductCostElement_ID"]),
            //                                        GetA_Asset_ID());
            //                                    _cost.SetBasisType(Util.GetValueOfString(_dsAsset.Tables[0].Rows[k]["BASISTYPE"]));
            //                                    _cost.SetCurrentCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["CURRENTCOSTPRICE"]));
            //                                    _cost.SetFutureCostPrice(Util.GetValueOfDecimal(_dsAsset.Tables[0].Rows[k]["FUTURECOSTPRICE"]));
            //                                    _cost.SetIsAssetCost(true);
            //                                    _cost.SetCurrentQty(GetQty());
            //                                    if (!_cost.Save(Get_TrxName()))
            //                                    {
            //                                        _log.Info("Asset Cost Line Not Saved For Asset " + GetValue());
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                }
            //                if (_dsAsset != null)
            //                {
            //                    _dsAsset.Dispose();
            //                }
            //            }
            //            #endregion Update Asset Cost
            //

        }

    }
}
