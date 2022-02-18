/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAsset
 * Purpose        : used for A_Asset table
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Globalization;
using System.Web.UI;
using VAdvantage.Logging;
using System.Net.Http.Headers;

namespace VAdvantage.Model
{
    public class MAsset : X_A_Asset
    {
        #region private variables
        //	Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MAsset).FullName);
        //	Product Info					
        private MProduct _product = null;
        #endregion

        /// <summary>
        /// Get Asset From Shipment.
        /// (Order.reverseCorrect)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InOutLine_ID">shipment line</param>
        /// <param name="trxName">transaction</param>
        /// <returns>asset or null</returns>
        public static List<MAsset> GetFromShipment(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            List<MAsset> retValue = new List<MAsset>();
            String sql = "SELECT * FROM A_Asset WHERE M_InOutLine_ID=" + M_InOutLine_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue.Add(new MAsset(ctx, dr, trxName));
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
        public static MAsset GetTrial(Ctx ctx, MUser user, String entityType)
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
            MProduct product = MProduct.GetTrial(ctx, entityType);
            if (product == null)
            {
                _log.Warning("No Trial for Entity Type=" + entityType);
                return null;
            }
            //
            DateTime now = Convert.ToDateTime(CommonFunctions.CurrentTimeMillis());
            //
            MAsset asset = new MAsset(ctx, 0, null);
            asset.SetClientOrg(user);
            asset.SetAssetServiceDate(now);
            asset.SetIsOwned(false);
            asset.SetIsTrialPhase(true);
            //
            MBPartner partner = new MBPartner(ctx, user.GetC_BPartner_ID(), null);
            String documentNo = "Trial";
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            asset.SetValue(value);
            //	Name		MProduct.afterSave
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
            asset.SetAD_User_ID(user.GetAD_User_ID());
            asset.SetC_BPartner_ID(user.GetC_BPartner_ID());
            //	Product
            asset.SetM_Product_ID(product.GetM_Product_ID());
            asset.SetA_Asset_Group_ID(product.GetA_Asset_Group_ID());
            asset.SetQty(new Decimal(product.GetSupportUnits()));
            //	Guarantee & Version
            asset.SetGuaranteeDate(TimeUtil.AddDays(now, product.GetTrialPhaseDays()));
            asset.SetVersionNo(product.GetVersionNo());
            //
            return asset;
        }

        /* 	Asset Constructor
     *	@param ctx context
     *	@param A_Asset_ID asset
     *	@param trxName transaction name 
     */
        public MAsset(Ctx ctx, int A_Asset_ID, Trx trxName)
            : base(ctx, A_Asset_ID, trxName)
        {

            if (A_Asset_ID == 0)
            {
                SetIsDepreciated(false);
                SetIsFullyDepreciated(false);
                SetIsInPosession(false);
                SetIsOwned(false);
                SetIsDisposed(false);
                SetM_AttributeSetInstance_ID(0);
                SetQty(Env.ONE);
                SetIsTrialPhase(false);
            }
        }

        /**
         * 	Discontinued Asset Constructor - DO NOT USE (but don't delete either)
         *	@param ctx context
         *	@param A_Asset_ID asset
         *	@deprecated
         */
        public MAsset(Ctx ctx, int A_Asset_ID)
            : this(ctx, A_Asset_ID, null)
        {

        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param dr result set record
         *	@param trxName transaction
         */
        public MAsset(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MAsset(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /**
        * 	Shipment Constructor
        * 	@param shipment shipment
        *	@param shipLine shipment line
        *	@param deliveryCount 0 or number of delivery
        */
        public MAsset(MInOut shipment, MInOutLine shipLine, int deliveryCount)
            : this(shipment.GetCtx(), 0, shipment.Get_TrxName())
        {

            SetClientOrg(shipment);

            SetValueNameDescription(shipment, shipLine, deliveryCount);
            //	Header

            // SetIsOwned(true);
            SetC_BPartner_ID(shipment.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(shipment.GetC_BPartner_Location_ID());
            SetAD_User_ID(shipment.GetAD_User_ID());
            SetM_Locator_ID(shipLine.GetM_Locator_ID());
            SetIsInPosession(true);

            // VIS0060: Set Trx Organization on Asset from MR Line.
            if (shipLine.GetAD_OrgTrx_ID() > 0)
            {
                Set_Value("AD_OrgTrx_ID", shipLine.GetAD_OrgTrx_ID());
            }
            //	Line
            MProduct product = shipLine.GetProduct();
            SetM_Product_ID(product.GetM_Product_ID());
            SetA_Asset_Group_ID(product.GetA_Asset_Group_ID());

            //////////////////////////////*
            //Changes for vafam
            // SetAssetServiceDate(shipment.GetMovementDate());
            //SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            MAssetGroup _assetGroup = new MAssetGroup(GetCtx(), GetA_Asset_Group_ID(), shipment.Get_TrxName());
            if (_assetGroup.IsOwned())
            {
                SetIsOwned(true);
                //SetC_BPartner_ID(0);
            }
            if (_assetGroup.IsDepreciated())
            {
                SetIsDepreciated(true);
                SetIsFullyDepreciated(false);
            }

            //VIS0060: Set Value of WIP, Month/Year and LIfe Units from AssetGroup to Asset.
            if (_assetGroup.Get_ColumnIndex("VAFAM_IsWIP") >= 0 && Util.GetValueOfBool(_assetGroup.Get_Value("VAFAM_IsWIP")))
            {
                Set_Value("VAFAM_IsWIP", true);
            }
            else
            {
                SetAssetServiceDate(shipment.GetDateAcct());
            }

            if (_assetGroup.Get_ColumnIndex("VAFAM_LifeUseUnit") >= 0 && !String.IsNullOrEmpty(Util.GetValueOfString(_assetGroup.Get_Value("VAFAM_LifeUseUnit"))))
            {
                Set_Value("VAFAM_LifeUseUnit", Util.GetValueOfString(_assetGroup.Get_Value("VAFAM_LifeUseUnit")));
                Set_Value("LifeUseUnits", Util.GetValueOfDecimal(_assetGroup.Get_Value("VAFAM_AssetGroupLife")));
            }

            //Change by Sukhwinder for setting Asset type and amortization template on Asset window, MANTIS ID:1762
            int countVA038 = Env.IsModuleInstalled("VA038_") ? 1 : 0;
            int countVAFAM = Env.IsModuleInstalled("VAFAM_") ? 1 : 0;
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
            if (shipLine.GetM_AttributeSetInstance_ID() != 0)		//	Instance
            {
                MAttributeSetInstance asi = new MAttributeSetInstance(GetCtx(), shipLine.GetM_AttributeSetInstance_ID(), Get_TrxName());
                SetM_AttributeSetInstance_ID(asi.GetM_AttributeSetInstance_ID());
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
            SetM_InOutLine_ID(shipLine.GetM_InOutLine_ID());

            //	Activate
            //MAssetGroup ag = MAssetGroup.Get(GetCtx(), GetA_Asset_Group_ID());
            if (!_assetGroup.IsCreateAsActive())
                SetIsActive(false);

            //Check if the Software Industry module installed, update following fields on Asset window
            if (Env.IsModuleInstalled("VA077_"))
            {
                //Set default values
                SetIsInPosession(false);
                SetIsOwned(false);
                SetIsActive(true);
                SetIsDisposed(false);

                Set_Value("VA077_SerialNo", shipLine.Get_Value("VA077_SerialNo"));
                Set_Value("VA077_CNAutodesk", shipLine.Get_Value("VA077_CNAutodesk"));
                Set_Value("VA077_RegEmail", shipLine.Get_Value("VA077_RegEmail"));
                Set_Value("VA077_IsCustAsset", "Y");
                Set_Value("VA077_OldSN", shipLine.Get_Value("VA077_OldSN"));
                Set_Value("VA077_UserRef_ID", shipLine.Get_Value("VA077_UserRef_ID"));
                Set_Value("VA077_ProductInfo", shipLine.Get_Value("VA077_ProductInfo"));
                Set_Value("VA077_ServiceContract_ID", shipLine.Get_Value("VA077_ServiceContract_ID"));
                Set_Value("AD_OrgTrx_ID", shipLine.Get_Value("AD_OrgTrx_ID"));

                if (Util.GetValueOfBool(product.Get_Value("VA077_LicenceTracked")))
                    Set_Value("VA077_LicenceTracked", "Y");
                else
                    Set_Value("VA077_LicenceTracked", "N");
            }
        }

        /**
        * 	Set Value Name Description
        *	@param shipment shipment
        *	@param line line
        *	@param deliveryCount
        */
        public void SetValueNameDescription(MInOut shipment, MInOutLine line, int deliveryCount)
        {
            MProduct product = line.GetProduct();
            MBPartner partner = shipment.GetBPartner();
            SetValueNameDescription(shipment, deliveryCount, product, partner);
        }

        /**
        * 	Set Value, Name, Description
        *	@param shipment shipment
        *	@param deliveryCount count
        *	@param product product
        *	@param partner partner
        */
        public void SetValueNameDescription(MInOut shipment,
            int deliveryCount, MProduct product, MBPartner partner)
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
            //	Name		MProduct.afterSave
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
        public MAsset(MInvoice invoice, MInvoiceLine invoiceline, int deliveryCount)
            : this(invoiceline.GetCtx(), 0, invoiceline.Get_TrxName())
        {

            SetClientOrg(invoiceline);

            SetValueNameDescription(invoice, invoiceline, deliveryCount);
            //	Header

            //SetIsOwned(true);
            SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(invoice.GetC_BPartner_Location_ID());
            SetAD_User_ID(invoice.GetAD_User_ID());
            //SetM_Locator_ID(invoice.GetM_Locator_ID());
            SetIsInPosession(true);


            SetAssetServiceDate(invoice.GetDateAcct());


            //	Line
            MProduct product = invoiceline.GetProduct();
            SetM_Product_ID(product.GetM_Product_ID());
            SetA_Asset_Group_ID(product.GetA_Asset_Group_ID());

            //////////////////////////////*
            //Changes for vafam
            // SetAssetServiceDate(shipment.GetMovementDate());
            //SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            MAssetGroup _assetGroup = new MAssetGroup(GetCtx(), GetA_Asset_Group_ID(), invoice.Get_TrxName());
            if (_assetGroup.IsOwned())
            {
                SetIsOwned(true);
                //SetC_BPartner_ID(0);
            }
            if (_assetGroup.IsDepreciated())
            {
                SetIsDepreciated(true);
                SetIsFullyDepreciated(false);
            }
            ////////////////////////////////////
            //Change by Sukhwinder for setting Asset type and amortization template on Asset window, MANTIS ID:1762
            int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
            int countVAFAM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VAFAM_' "));

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
            if (invoiceline.GetM_AttributeSetInstance_ID() != 0)		//	Instance
            {
                MAttributeSetInstance asi = new MAttributeSetInstance(GetCtx(), invoiceline.GetM_AttributeSetInstance_ID(), Get_TrxName());
                SetM_AttributeSetInstance_ID(asi.GetM_AttributeSetInstance_ID());
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
            SetM_InOutLine_ID(invoiceline.GetM_InOutLine_ID());
            Set_Value("C_InvoiceLine_ID", invoiceline.GetC_InvoiceLine_ID());

            //	Activate
            MAssetGroup ag = MAssetGroup.Get(GetCtx(), GetA_Asset_Group_ID());
            if (!ag.IsCreateAsActive())
                SetIsActive(false);
        }
        /**
        * 	Set Value Name Description
        *	@param Invoice
        *	@param line line
        *	@param deliveryCount
        */
        public void SetValueNameDescription(MInvoice invoice, MInvoiceLine line, int deliveryCount)
        {
            MProduct product = line.GetProduct();
            MBPartner partner = new MBPartner(GetCtx(), invoice.GetC_BPartner_ID(), null);
            SetValueNameDescription(invoice, deliveryCount, product, partner);
        }

        /**
        * 	Set Value, Name, Description
        *	@param invoice
        *	@param deliveryCount count
        *	@param product product
        *	@param partner partner
        */
        public void SetValueNameDescription(MInvoice invoice,
            int deliveryCount, MProduct product, MBPartner partner)
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
            //	Name		MProduct.afterSave
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
            //	UPDATE M_Product SET SupportUnits=1 WHERE SupportUnits IS NULL OR SupportUnits<1;
            //	UPDATE A_Asset a SET Qty = (SELECT l.MovementQty * p.SupportUnits FROM M_InOutLine l, M_Product p WHERE a.M_InOutLine_ID=l.M_InOutLine_ID AND a.M_Product_ID=p.M_Product_ID) WHERE a.M_Product_ID IS NOT NULL AND a.M_InOutLine_ID IS NOT NULL;
            Decimal Qty = Env.ONE;
            if (GetM_InOutLine_ID() != 0)
            {
                MInOutLine line = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
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
        public MAssetDelivery[] GetDeliveries()
        {
            List<MAssetDelivery> list = new List<MAssetDelivery>();

            String sql = "SELECT * FROM A_Asset_Delivery WHERE A_Asset_ID=" + GetA_Asset_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MAssetDelivery(GetCtx(), dr, Get_TrxName()));
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
            MAssetDelivery[] retValue = new MAssetDelivery[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Delivery count
         * 	@return delivery count
         */
        public int GetDeliveryCount()
        {
            String sql = "SELECT COUNT(*) FROM A_Asset_Delivery WHERE A_Asset_ID=" + GetA_Asset_ID();
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
         * 	Get Product R_MailText_ID
         *	@return R_MailText_ID
         */
        public int GetProductR_MailText_ID()
        {
            return GetProduct().GetR_MailText_ID();
        }

        /**
         * 	Get Product Info
         * 	@return product
         */
        private MProduct GetProduct()
        {
            if (_product == null)
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            return _product;
        }

        /* 	Get Active Addl. Product Downloads
        *	@return array of downloads
        */
        public MProductDownload[] GetProductDownloads()
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
            MProductDownload[] dls = GetProductDownloads();
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
            MProductDownload[] dls = GetProductDownloads();
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
        public MAssetGroup GetAssetGroup()
        {
            return MAssetGroup.Get(GetCtx(), GetA_Asset_Group_ID());
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
            MAssetGroup astGrp = new MAssetGroup(GetCtx(), GetA_Asset_Group_ID(), Get_TrxName());
            if (newRecord && astGrp.Get_ColumnIndex("M_SerNoCtl_ID") >= 0 && astGrp.GetM_SerNoCtl_ID() > 0 && String.IsNullOrEmpty(GetValue()))
            {
                string name = "";
                MSerNoCtl ctl = new MSerNoCtl(GetCtx(), astGrp.GetM_SerNoCtl_ID(), Get_TrxName());

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
            if (Env.IsModuleInstalled("VAFAM_"))
            {
                // VIS0060: Handle case when Life related data updated on Asset.
                if (!newRecord && (Is_ValueChanged("LifeUseUnits") || Is_ValueChanged("VAFAM_LifeUseUnit") || Is_ValueChanged("VAFAM_DepreciationType_ID")))
                {
                    var LastSchedDate = DB.ExecuteScalar("SELECT MAX(VAFAM_DepDate) FROM VAFAM_ASSETSCHEDULE WHERE IsActive='Y' AND VAFAM_DepAmor='Y' AND A_Asset_ID="
                        + Get_ID(), null, Get_TrxName());
                    if (LastSchedDate != null)
                    {
                        // Check if SLM-Rate Based depreciation, Then don not check Life with Schedule date.
                        string rateBased = Util.GetValueOfString(DB.ExecuteScalar("SELECT VAFAM_IsRateBased FROM VAFAM_DepreciationType WHERE VAFAM_DepreciationType = 'SLM'" +
                            " AND VAFAM_DepreciationType_ID = " + Util.GetValueOfInt(Get_Value("VAFAM_DepreciationType_ID")), null, Get_TrxName()));
                        if (!rateBased.Equals("Y"))
                        {
                            DateTime assetDate = Util.GetValueOfString(Get_Value("VAFAM_LifeUseUnit")).Equals("Y") ?
                                GetAssetServiceDate().Value.AddYears(Util.GetValueOfInt(GetLifeUseUnits())) :
                                GetAssetServiceDate().Value.AddMonths(Util.GetValueOfInt(GetLifeUseUnits()));

                            // VIS0060: Life of Asset can not be less than the date of last charged depreciation
                            if (assetDate < Util.GetValueOfDateTime(LastSchedDate))
                            {
                                log.Info("Life of Asset can not be less than the date of last charged depreciation. - Asset Service Date : " + GetAssetServiceDate().Value.Date +
                                    " - Last Schedule Date : " + Util.GetValueOfDateTime(LastSchedDate).Value.Date);
                                log.SaveError("VAFAM_LifeUseLess", "");
                                return false;
                            }
                        }
                    }
                }

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
                int assetGroupId = GetA_Asset_Group_ID();
                // get related to value agaisnt asset = 75
                string sql = "SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where   r.name='FRPT_RelatedTo' and l.name='Asset'";
                string _RelatedToProduct = Convert.ToString(DB.ExecuteScalar(sql));

                _sql.Clear();
                _sql.Append("Select Count(*) From FRPT_Asset_Acct   where A_Asset_ID=" + assetId + " AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (value < 1)
                {
                    _sql.Clear();
                    _sql.Append(@"Select  PCA.c_acctschema_id, PCA.c_validcombination_id, PCA.frpt_acctdefault_id " +
                        " From FRPT_Asset_Group_Acct PCA " +
                        " inner join frpt_acctdefault ACC ON acc.frpt_acctdefault_id= PCA.frpt_acctdefault_id " +
                        " where PCA.A_Asset_Group_ID=" + assetGroupId +
                        " and acc.frpt_relatedto='" + _RelatedToProduct +
                        "' AND PCA.IsActive = 'Y' AND PCA.AD_Client_ID = " + GetAD_Client_ID());

                    DataSet ds = DB.ExecuteDataset(_sql.ToString());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            obj = MTable.GetPO(GetCtx(), "FRPT_Asset_Acct", 0, null);
                            obj.Set_ValueNoCheck("AD_Org_ID", 0);
                            obj.Set_ValueNoCheck("A_Asset_ID", assetId);
                            obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                            obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ValidCombination_ID"]));
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
         * 	@param AD_User_ID recipient
         * 	@return asset delivery
         */
        public MAssetDelivery ConfirmDelivery(EMail email, int AD_User_ID)
        {
            SetVersionNo(GetProductVersionNo());
            MAssetDelivery ad = new MAssetDelivery(this, email, null, AD_User_ID);
            return ad;
        }

        /* 	Confirm Asset Download Delivery
        *	@param request request
        * 	@param AD_User_ID recipient
        * 	@return asset delivery
        */
        //public MAssetDelivery ConfirmDelivery(HttpServletRequest request, int AD_User_ID)
        //{
        //    SetVersionNo(GetProductVersionNo());
        //    SetLifeUseUnits(GetLifeUseUnits().add(Env.ONE));
        //    MAssetDelivery ad = new MAssetDelivery(this, request, AD_User_ID);
        //    return ad;
        //}



        private void UpdateAssetCost()
        {
            //Cost Code Commented - As not required on Asset Save
            //Pratap- For Asset Cost
            //            #region Update Asset Cost

            //            if (GetM_Product_ID() > 0)
            //            {
            //                DataSet _dsAsset = null;
            //                StringBuilder _sql = new StringBuilder();
            //                _sql.Clear();
            //                _sql.Clear();
            //                _sql.Append(@" SELECT M_COST.AD_CLIENT_ID,
            //                                          M_COST.AD_ORG_ID,
            //                                          M_COST.M_COSTELEMENT_ID,
            //                                          M_PRODUCT.M_PRODUCT_ID,
            //                                          M_COST.C_ACCTSCHEMA_ID,
            //                                          M_COST.M_COSTTYPE_ID,
            //                                          M_COST.BASISTYPE,
            //                                          M_PRODUCT.C_UOM_ID,
            //                                          M_COSTELEMENT.COSTINGMETHOD,
            //                                          M_COST.M_ATTRIBUTESETINSTANCE_ID,
            //                                          M_COST.CURRENTCOSTPRICE,
            //                                          M_COST.FUTURECOSTPRICE
            //                                        FROM M_COST
            //                                        INNER JOIN M_PRODUCT
            //                                        ON(M_PRODUCT.M_PRODUCT_ID=M_COST.M_PRODUCT_ID)
            //                                        INNER JOIN M_COSTELEMENT
            //                                        ON(M_COSTELEMENT.M_COSTELEMENT_ID  =M_COST.M_COSTELEMENT_ID)
            //                                        WHERE M_COSTELEMENT.COSTELEMENTTYPE='M'
            //                                        AND M_COST.AD_CLIENT_ID            =" + GetAD_Client_ID() +
            //                            @" AND M_COST.ISACTIVE                ='Y' AND M_COST.IsAssetCost='N'
            //                                        AND M_PRODUCT.M_PRODUCT_ID         =" + GetM_Product_ID());

            //                _dsAsset = DB.ExecuteDataset(_sql.ToString());
            //                _sql.Clear();
            //                if (_dsAsset != null)
            //                {
            //                    if (_dsAsset.Tables[0].Rows.Count > 0)
            //                    {
            //                        for (int k = 0; k < _dsAsset.Tables[0].Rows.Count; k++)
            //                        {
            //                            MInOutLine _inOutLine = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
            //                            MProduct _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            //                            MCostElement _costElement = new MCostElement(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_COSTELEMENT_ID"]), Get_TrxName());
            //                            MAcctSchema _acctsch = null;
            //                            MCost _cost = null;
            //                            if (GetM_InOutLine_ID() > 0)
            //                            {
            //                                if (_inOutLine.GetC_OrderLine_ID() > 0)
            //                                {
            //                                    if (_costElement.IsLastPOPrice() || _costElement.IsAveragePO())
            //                                    {
            //                                        _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["C_ACCTSCHEMA_ID"]), Get_TrxName());
            //                                        _cost = new MCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"]),
            //                                            _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["AD_ORG_ID"]),
            //                                            Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_COSTELEMENT_ID"]),
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
            //                                        _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["C_ACCTSCHEMA_ID"]), Get_TrxName());
            //                                        _cost = new MCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"]),
            //                                            _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["AD_ORG_ID"]),
            //                                            Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_COSTELEMENT_ID"]),
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
            //                                    _acctsch = new MAcctSchema(GetCtx(), Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["C_ACCTSCHEMA_ID"]), Get_TrxName());
            //                                    _cost = new MCost(_product, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"]),
            //                                        _acctsch, Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["AD_ORG_ID"]),
            //                                        Util.GetValueOfInt(_dsAsset.Tables[0].Rows[k]["M_COSTELEMENT_ID"]),
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
