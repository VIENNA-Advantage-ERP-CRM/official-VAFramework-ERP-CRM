/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProduct
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     04-Jun-2009
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
using VAdvantage.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace VAdvantage.Model
{
    public class MProduct : X_VAM_Product
    {
        //	UOM Precision			
        private int? _precision = null;
        // Additional Downloads				
        private MProductDownload[] _downloads = null;
        //	Cache						
        private static CCache<int, MProduct> s_cache = new CCache<int, MProduct>("VAM_Product", 40, 5);	//	5 minutes
        //	Static Logger	*
        private static VLogger _log = VLogger.GetVLogger(typeof(MProduct).FullName);

        /// <summary>
        /// Get MProduct from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">id</param>
        /// <returns>MProduct</returns>
        public static MProduct Get(Ctx ctx, int VAM_Product_ID)
        {
            int key = VAM_Product_ID;
            MProduct retValue = (MProduct)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProduct(ctx, VAM_Product_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /*	Get MProduct from Cache
        *	@param ctx context
        *	@param whereClause sql where clause
        *	@param trxName trx
        *	@return MProduct
        */
        public static MProduct[] Get(Ctx ctx, String whereClause, Trx trxName)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAM_Product";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE VAF_Client_ID=" + VAF_Client_ID + " AND " + whereClause;
            List<MProduct> list = new List<MProduct>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProduct(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MProduct[] retValue = new MProduct[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Trial Products for Entity Type
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="entityType">entity type</param>
        /// <returns>trial product or null</returns>
        public static MProduct GetTrial(Ctx ctx, String entityType)
        {
            if (Utility.Util.IsEmpty(entityType))
            {
                _log.Warning("No Entity Type");
                return null;
            }
            MProduct retValue = null;
            String sql = "SELECT * FROM VAM_Product "
                + "WHERE LicenseInfo LIKE '%" + entityType + "%' AND TrialPhaseDays > 0 AND IsActive='Y'";
            //String entityTypeLike = "%" + entityType + "%";
            //pstmt.setString(1, entityTypeLike);
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MProduct(ctx, dr, null);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            if (retValue != null && retValue.GetVAF_Client_ID() != ctx.GetVAF_Client_ID())
            {
                _log.Warning("ProductClient_ID=" + retValue.GetVAF_Client_ID() + " <> EnvClient_ID=" + ctx.GetVAF_Client_ID());
            }
            if (retValue != null && retValue.GetVAA_AssetGroup_ID() == 0)
            {
                _log.Warning("Product has no Asset Group - " + retValue);
                return null;
            }
            if (retValue == null)
            {
                _log.Warning("No Product for EntityType - " + entityType);
            }
            return retValue;
        }

        /*	Is Product Stocked
        * 	@param ctx context
        *	@param VAM_Product_ID id
        *	@return true if found and stocked - false otherwise
        */
        public static bool IsProductStocked(Ctx ctx, int VAM_Product_ID)
        {
            MProduct product = Get(ctx, VAM_Product_ID);
            return product.IsStocked();
        }

        /* 	Standard Constructor
         *	@param ctx context
         *	@param VAM_Product_ID id
         *	@param trxName transaction
         */
        public MProduct(Ctx ctx, int VAM_Product_ID, Trx trxName)
            : base(ctx, VAM_Product_ID, trxName)
        {
            if (VAM_Product_ID == 0)
            {
                //	setValue (null);
                //	setName (null);
                //	setVAM_ProductCategory_ID (0);
                //	setVAB_TaxCategory_ID (0);
                //	setVAB_UOM_ID (0);
                //
                SetProductType(PRODUCTTYPE_Item);	// I
                SetIsBOM(false);	// N
                SetIsInvoicePrintDetails(false);
                SetIsPickListPrintDetails(false);
                SetIsPurchased(true);	// Y
                SetIsSold(true);	// Y
                SetIsStocked(true);	// Y
                SetIsSummary(false);
                SetIsVerified(false);	// N
                SetIsWebStoreFeatured(false);
                SetIsSelfService(true);
                SetIsExcludeAutoDelivery(false);
                SetProcessing(false);	// N
                SetIsDropShip(false); // N
                SetSupportUnits(1);
            }
        }

        /**
         * 	Load constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MProduct(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param et parent
         */
        public MProduct(MExpenseType et)
            : this(et.GetCtx(), 0, et.Get_TrxName())
        {

            SetProductType(X_VAM_Product.PRODUCTTYPE_ExpenseType);
            SetExpenseType(et);
        }

        /**
         * 	Parent Constructor
         *	@param resource parent
         *	@param resourceType resource type
         */
        public MProduct(MResource resource, MResourceType resourceType)
            : this(resource.GetCtx(), 0, resource.Get_TrxName())
        {

            SetProductType(X_VAM_Product.PRODUCTTYPE_Resource);
            SetResource(resource);
            SetResource(resourceType);
        }

        /**
         * 	Import Constructor
         *	@param impP import
         */
        public MProduct(X_I_Product impP)
            : this(impP.GetCtx(), 0, impP.Get_TrxName())
        {
            PO.CopyValues(impP, this, impP.GetVAF_Client_ID(), impP.GetVAF_Org_ID());
        }


        /*	Set Expense Type
        *	@param parent expense type
        *	@return true if changed
        */
        public bool SetExpenseType(MExpenseType parent)
        {
            bool changed = false;
            if (!PRODUCTTYPE_ExpenseType.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_ExpenseType);
                changed = true;
            }
            if (parent.GetVAS_ChargeType_ID() != GetVAS_ChargeType_ID())
            {
                SetVAS_ChargeType_ID(parent.GetVAS_ChargeType_ID());
                changed = true;
            }
            if (parent.IsActive() != IsActive())
            {
                SetIsActive(parent.IsActive());
                changed = true;
            }
            //
            if (!parent.GetValue().Equals(GetValue()))
            {
                SetValue(parent.GetValue());
                changed = true;
            }
            if (!parent.GetName().Equals(GetName()))
            {
                SetName(parent.GetName());
                changed = true;
            }
            if ((parent.GetDescription() == null && GetDescription() != null)
                || (parent.GetDescription() != null && !parent.GetDescription().Equals(GetDescription())))
            {
                SetDescription(parent.GetDescription());
                changed = true;
            }
            if (parent.GetVAB_UOM_ID() != GetVAB_UOM_ID())
            {
                SetVAB_UOM_ID(parent.GetVAB_UOM_ID());
                changed = true;
            }
            if (parent.GetVAM_ProductCategory_ID() != GetVAM_ProductCategory_ID())
            {
                SetVAM_ProductCategory_ID(parent.GetVAM_ProductCategory_ID());
                changed = true;
            }
            if (parent.GetVAB_TaxCategory_ID() != GetVAB_TaxCategory_ID())
            {
                SetVAB_TaxCategory_ID(parent.GetVAB_TaxCategory_ID());
                changed = true;
            }
            //
            return changed;
        }

        /**
         * 	Set Resource
         *	@param parent resource
         *	@return true if changed
         */
        public bool SetResource(MResource parent)
        {
            bool changed = false;
            if (!PRODUCTTYPE_Resource.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_Resource);
                changed = true;
            }
            if (parent.GetS_Resource_ID() != GetS_Resource_ID())
            {
                SetS_Resource_ID(parent.GetS_Resource_ID());
                changed = true;
            }
            if (parent.IsActive() != IsActive())
            {
                SetIsActive(parent.IsActive());
                changed = true;
            }
            //
            if (!parent.GetValue().Equals(GetValue()))
            {
                SetValue(parent.GetValue());
                changed = true;
            }
            if (!parent.GetName().Equals(GetName()))
            {
                SetName(parent.GetName());
                changed = true;
            }
            if ((parent.GetDescription() == null && GetDescription() != null)
                || (parent.GetDescription() != null && !parent.GetDescription().Equals(GetDescription())))
            {
                SetDescription(parent.GetDescription());
                changed = true;
            }
            //
            return changed;
        }

        /**
         * 	Set Resource Type
         *	@param parent resource type
         *	@return true if changed
         */
        public bool SetResource(MResourceType parent)
        {
            bool changed = false;
            if (PRODUCTTYPE_Resource.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_Resource);
                changed = true;
            }
            //
            if (parent.GetVAB_UOM_ID() != GetVAB_UOM_ID())
            {
                SetVAB_UOM_ID(parent.GetVAB_UOM_ID());
                changed = true;
            }
            if (parent.GetVAM_ProductCategory_ID() != GetVAM_ProductCategory_ID())
            {
                SetVAM_ProductCategory_ID(parent.GetVAM_ProductCategory_ID());
                changed = true;
            }
            if (parent.GetVAB_TaxCategory_ID() != GetVAB_TaxCategory_ID())
            {
                SetVAB_TaxCategory_ID(parent.GetVAB_TaxCategory_ID());
                changed = true;
            }
            //
            return changed;
        }

        /**
       * 	Get UOM Standard Precision
       *	@return UOM Standard Precision
       */
        public int GetUOMPrecision()
        {
            if (_precision == null)
            {
                int VAB_UOM_ID = GetVAB_UOM_ID();
                if (VAB_UOM_ID == 0)
                    return 0;	//	EA
                _precision = (int)MUOM.GetPrecision(GetCtx(), VAB_UOM_ID);
            }
            return (int)_precision;
        }

        /**
        * 	Create Asset Group for this product
        *	@return asset group id
        */
        public int GetVAA_AssetGroup_ID()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetVAM_ProductCategory_ID());
            return pc.GetVAA_AssetGroup_ID();
        }


        /**
        * 	Create Asset for this product
        *	@return true if asset is created
        */
        public bool IsCreateAsset()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetVAM_ProductCategory_ID());
            return pc.GetVAA_AssetGroup_ID() != 0;
        }

        /* 	Get Attribute Set
        *	@return set or null
        */
        public MAttributeSet GetAttributeSet()
        {
            if (GetVAM_PFeature_Set_ID() != 0)
                return MAttributeSet.Get(GetCtx(), GetVAM_PFeature_Set_ID());
            return null;
        }


        /*	Has the Product Instance Attribute
         *	@return true if instance attributes
         */
        public bool IsInstanceAttribute()
        {
            if (GetVAM_PFeature_Set_ID() == 0)
                return false;
            MAttributeSet mas = MAttributeSet.Get(GetCtx(), GetVAM_PFeature_Set_ID());
            return mas.IsInstanceAttribute();
        }

        /**
        * 	Create One Asset Per UOM
        *	@return individual asset
        */
        public bool IsOneAssetPerUOM()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetVAM_ProductCategory_ID());
            if (pc.GetVAA_AssetGroup_ID() == 0)
                return false;
            MVAAAssetGroup ag = MVAAAssetGroup.Get(GetCtx(), pc.GetVAA_AssetGroup_ID());
            return ag.IsOneAssetPerUOM();

        }

        /* Product is Item
        *	@return true if item
        */
        public bool IsItem()
        {
            return PRODUCTTYPE_Item.Equals(GetProductType());
        }

        /**
         * 	Product is an Item and Stocked
         *	@return true if stocked and item
         */
        public new bool IsStocked()
        {
            return base.IsStocked() && IsItem();
        }

        /**
         * 	Is Service
         *	@return true if service (resource, online)
         */
        public bool IsService()
        {
            //	PRODUCTTYPE_Service, PRODUCTTYPE_Resource, PRODUCTTYPE_Online
            return !IsItem();	//	
        }

        /*	Get UOM Symbol
         *	@return UOM Symbol
         */
        public String GetUOMSymbol()
        {
            int VAB_UOM_ID = GetVAB_UOM_ID();
            if (VAB_UOM_ID == 0)
                return "";
            return MUOM.Get(GetCtx(), VAB_UOM_ID).GetUOMSymbol();
        }

        /**
        * 	Get Active(!) Product Downloads
        * 	@param requery requery
        *	@return array of downloads
        */
        public MProductDownload[] GetProductDownloads(bool requery)
        {
            if (_downloads != null && !requery)
                return _downloads;
            //
            List<MProductDownload> list = new List<MProductDownload>();
            String sql = "SELECT * FROM VAM_ProductDownload "
                + "WHERE VAM_Product_ID=" + GetVAM_Product_ID() + " AND IsActive='Y' ORDER BY Name";
            //
            DataTable dt = null;
            IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProductDownload(GetCtx(), dr, Get_TrxName()));
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

            _downloads = new MProductDownload[list.Count];
            _downloads = list.ToArray();
            return _downloads;
        }

        /**
        * 	Does the product have downloads
        *	@return true if downloads exists
        */
        public bool HasDownloads()
        {
            GetProductDownloads(false);
            return _downloads != null && _downloads.Length > 0;
        }

        /*	Get SupportUnits
        *	@return units per UOM
        */
        public new int GetSupportUnits()
        {
            int ii = base.GetSupportUnits();
            if (ii < 1)
                ii = 1;
            return ii;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProduct[");
            sb.Append(Get_ID()).Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Check Storage
            if (!newRecord && 	//	
                ((Is_ValueChanged("IsActive") && !IsActive())		//	now not active 
                || (Is_ValueChanged("IsStocked") && !IsStocked())	//	now not stocked
                || (Is_ValueChanged("ProductType") 					//	from Item
                    && PRODUCTTYPE_Item.Equals(Get_ValueOld("ProductType")))))
            {
                MStorage[] storages = MStorage.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
                Decimal OnHand = Env.ZERO;
                Decimal Ordered = Env.ZERO;
                Decimal Reserved = Env.ZERO;
                for (int i = 0; i < storages.Length; i++)
                {
                    OnHand = Decimal.Add(OnHand, (storages[i].GetQtyOnHand()));
                    Ordered = Decimal.Add(OnHand, (storages[i].GetQtyOrdered()));
                    Reserved = Decimal.Add(OnHand, (storages[i].GetQtyReserved()));
                }
                String errMsg = "";
                if (Env.Signum(OnHand) != 0)
                    errMsg = "@QtyOnHand@ = " + OnHand;
                if (Env.Signum(Ordered) != 0)
                    errMsg += " - @QtyOrdered@ = " + Ordered;
                if (Env.Signum(Reserved) != 0)
                    errMsg += " - @QtyReserved@" + Reserved;
                if (errMsg.Length > 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), errMsg));
                    return false;
                }
            }	//	storage

            //	Reset Stocked if not Item
            if (IsStocked() && !PRODUCTTYPE_Item.Equals(GetProductType()))
                SetIsStocked(false);

            //	UOM reset
            if (_precision != null && Is_ValueChanged("VAB_UOM_ID"))
                _precision = null;
            if (Util.GetValueOfInt(Env.GetCtx().GetContext("#VAF_UserContact_ID")) != 100)
            {
                if (Is_ValueChanged("VAB_UOM_ID") || Is_ValueChanged("VAM_PFeature_Set_ID"))
                {
                    string uqry = "SELECT SUM(cc) as count FROM  (SELECT COUNT(*) AS cc FROM VAM_InvTrf_Line WHERE VAM_Product_ID = " + GetVAM_Product_ID() + @"  UNION
                SELECT COUNT(*) AS cc FROM VAM_InventoryLine WHERE VAM_Product_ID = " + GetVAM_Product_ID() + " UNION SELECT COUNT(*) AS cc FROM VAB_OrderLine WHERE VAM_Product_ID = " + GetVAM_Product_ID() +
                    " UNION  SELECT COUNT(*) AS cc FROM VAM_Inv_InOutLine WHERE VAM_Product_ID = " + GetVAM_Product_ID() + ") t";
                    int no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                    if (no == 0 || IsBOM())
                    {
                        uqry = "SELECT count(*) FROM VAM_ProductionPlan WHERE VAM_Product_ID = " + GetVAM_Product_ID();
                        no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                    }
                    if (no > 0)
                    {
                        log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "Could not Save Record. Transactions available in System."));
                        return false;
                    }
                }
            }
            if (newRecord)
            {
                //string sql = "SELECT UPCUNIQUE('p','" + GetUPC() + "') as productID FROM Dual";
                //int manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                int manu_ID = UpcUniqueClientWise(GetVAF_Client_ID(), GetUPC());
                if (manu_ID > 0)
                {
                    _log.SaveError("UPCUnique", "");
                    return false;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(GetUPC()) &&
                   Util.GetValueOfString(Get_ValueOld("UPC")) != GetUPC())
                {
                    //string sql = "SELECT UPCUNIQUE('p','" + GetUPC() + "') as productID FROM Dual";
                    //int manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    //if (manu_ID != 0 && manu_ID != GetVAM_Product_ID())

                    int manu_ID = UpcUniqueClientWise(GetVAF_Client_ID(), GetUPC());
                    if (manu_ID > 0)
                    {
                        _log.SaveError("UPCUnique", "");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check Unique UPC client wise
        /// </summary>
        /// <param name="VAF_Client_ID">Client ID</param>
        /// <param name="upc">UPC of Product</param>
        /// <returns>VAM_Product_ID, if records found</returns>
        public static int UpcUniqueClientWise(int VAF_Client_ID, string upc)
        {
            int VAM_Product_ID = 0;
            string sql = @"SELECT p.VAM_Product_ID FROM VAM_Product p LEFT JOIN VAM_Manufacturer m ON m.VAM_Product_ID = p.VAM_Product_ID 
                            LEFT JOIN VAM_ProductFeatures a ON a.VAM_Product_ID = p.VAM_Product_ID LEFT JOIN VAB_UOM_Conversion c ON c.VAM_Product_ID = p.VAM_Product_ID 
                            LEFT JOIN VAB_UOVAM_ProductBarcode b ON b.VAM_Product_ID = p.VAM_Product_ID WHERE p.VAF_Client_ID = " + VAF_Client_ID
                            + " AND ( p.UPC = '" + upc + "' OR m.UPC = '" + upc + "' OR a.UPC = '" + upc + "' OR c.UPC = '" + upc + "' OR b.UPC = '" + upc + "')";
            VAM_Product_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return VAM_Product_ID;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            StringBuilder _sql = new StringBuilder("");
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_Product_Category_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Product_Category_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Product_Category_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                PO obj = null;
                //MFRPTProductAcct obj = null;
                int _MProduct_ID = GetVAM_Product_ID();
                int _PCategory_ID = GetVAM_ProductCategory_ID();
                string sql = "SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where   r.name='FRPT_RelatedTo' and l.name='Product'";
                //"select VALUE from VAF_CtrlRef_List where name='Product'";
                string _RelatedToProduct = Convert.ToString(DB.ExecuteScalar(sql));
                //string _RelatedToProduct = X_FRPT_AcctDefault.FRPT_RELATEDTO_Product.ToString();

                _sql.Clear();
                _sql.Append("Select Count(*) From FRPT_Product_Acct  where VAM_Product_ID=" + _MProduct_ID + " AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (value < 1)
                {
                    _sql.Clear();
                    _sql.Append("Select  PCA.VAB_AccountBook_id, PCA.VAB_Acct_ValidParameter_id, PCA.frpt_acctdefault_id From FRPT_product_category_acct PCA inner join frpt_acctdefault ACC ON acc.frpt_acctdefault_id= PCA.frpt_acctdefault_id where PCA.VAM_ProductCategory_id=" + _PCategory_ID + " and acc.frpt_relatedto='" + _RelatedToProduct + "' AND PCA.IsActive = 'Y' AND PCA.VAF_Client_ID = " + GetVAF_Client_ID());
                    //_sql.Append("Select VAB_AccountBook_ID, VAB_Acct_ValidParameter_ID, FRPT_AcctDefault_ID from FRPT_product_category_acct where VAM_ProductCategory_id =" + _PCategory_ID);

                    DataSet ds = DB.ExecuteDataset(_sql.ToString());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //obj = new MFRPTProductAcct(GetCtx(), 0, null);
                            obj = MVAFTableView.GetPO(GetCtx(), "FRPT_Product_Acct", 0, null);
                            obj.Set_ValueNoCheck("VAF_Org_ID", 0);
                            obj.Set_ValueNoCheck("VAM_Product_ID", _MProduct_ID);
                            obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]));
                            obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_ID"]));
                            obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                            if (!obj.Save())
                            { }
                        }
                    }
                }
                // Change by mohit amortization process
                //int _CountVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_'  AND IsActive = 'Y'"));
                //if (_CountVA038 > 0)
                //{
                //    if (GetProductType() == "E" || GetProductType() == "S")
                //    {
                //        if (Util.GetValueOfInt(Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                //        {
                //            DataSet _dsAcct = DB.ExecuteDataset("SELECT VAB_AccountBook_ID, FRPT_AcctDefault_ID, VAB_Acct_ValidParameter_ID, SEQNO FROM VA038_Amortization_Acct "
                //                              + "WHERE IsActive='Y' AND  VA038_AmortizationTemplate_ID=" + Util.GetValueOfInt(Get_Value("VA038_AmortizationTemplate_ID")));
                //            if (_dsAcct != null && _dsAcct.Tables[0].Rows.Count > 0)
                //            {
                //                for (int j = 0; j < _dsAcct.Tables[0].Rows.Count; j++)
                //                {
                //                    obj = MVAFTableView.GetPO(GetCtx(), "FRPT_Product_Acct", 0, null);
                //                    obj.Set_ValueNoCheck("VAF_Org_ID", 0);
                //                    obj.Set_ValueNoCheck("VAM_Product_ID", _MProduct_ID);
                //                    obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(_dsAcct.Tables[0].Rows[j]["VAB_AccountBook_ID"]));
                //                    obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(_dsAcct.Tables[0].Rows[j]["VAB_Acct_ValidParameter_ID"]));
                //                    obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(_dsAcct.Tables[0].Rows[j]["FRPT_AcctDefault_ID"]));
                //                    if (!obj.Save())
                //                    { }
                //                }
                //            }
                //        }
                //    }
                //}
                // End amortization process
            }
            else
            {
                // By Amit 
                //if (!success)
                //    return success;

                //	Value/Name change in Account
                if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                    MVABAccount.UpdateValueDescription(GetCtx(), "VAM_Product_ID=" + GetVAM_Product_ID(), Get_TrxName());

                //	Name/Description Change in Asset	MAsset.setValueNameDescription
                if (!newRecord && (Is_ValueChanged("Name") || Is_ValueChanged("Description")))
                {
                    String sql = " UPDATE VAA_Asset a SET Name=(SELECT SUBSTR(bp.Name || ' - ' || p.Name,1,60) FROM VAM_Product p, VAB_BusinessPartner bp  WHERE p.VAM_Product_ID=a.VAM_Product_ID AND bp.VAB_BusinessPartner_ID=a.VAB_BusinessPartner_ID)," +
      "Description=(SELECT  p.Description FROM VAM_Product p, VAB_BusinessPartner bp WHERE p.VAM_Product_ID=a.VAM_Product_ID AND bp.VAB_BusinessPartner_ID=a.VAB_BusinessPartner_ID)" +
      "WHERE IsActive='Y'  AND VAM_Product_ID=" + GetVAM_Product_ID();

                    int no = 0;
                    try
                    {
                        no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                    }
                    catch { }
                    log.Fine("Asset Description updated #" + no);
                }
                //	New - Acct, Tree, Old Costing
                if (newRecord)
                {
                    if (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y")
                    {
                        bool sucs = Insert_Accounting("VAM_Product_Acct", "VAM_ProductCategory_Acct",
                              "p.VAM_ProductCategory_ID=" + GetVAM_ProductCategory_ID());

                        //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                        // Before this, data was being saved but giving message "record not saved".
                        if (!sucs)
                        {
                            log.SaveWarning("AcctNotSaved", "");
                        }
                    }

                    //
                    MVABAccountBook[] mass = MVABAccountBook.GetClientAcctSchema(GetCtx(), GetVAF_Client_ID(), Get_TrxName());
                    for (int i = 0; i < mass.Length; i++)
                    {
                        //	Old
                        MProductCosting pcOld = new MProductCosting(this, mass[i].GetVAB_AccountBook_ID());
                        pcOld.Save();
                    }
                }
                //	New Costing
                // by Amit 22-12-2015
                //if (newRecord || Is_ValueChanged("VAM_ProductCategory_ID"))
                //{
                //    MCost.Create(this);
                //}
            }

            //22-12-2015
            //by Amit for creating records ffor product foe all Costing Element whose costing elemnt type is 'Material'
            //if (newRecord || Is_ValueChanged("VAM_ProductCategory_ID"))
            //{
            //    MCost.CreateRecords(this);
            //}
            //20-12-2016
            //By Vivek Chauhan saving Nutrition value against product...........
            object ModuleId = DB.ExecuteScalar("select VAF_ModuleInfo_id from VAF_ModuleInfo where prefix='VA019_' and isactive='Y'");
            if (ModuleId != null && ModuleId != DBNull.Value)
            {
                object objNDBNo = DB.ExecuteScalar("select va019_ndbno from VAM_Product where VAM_Product_ID=" + GetVAM_Product_ID() + "");
                if (objNDBNo != null && objNDBNo != DBNull.Value)
                {
                    CallNutritionApi(Convert.ToString(objNDBNo), GetVAM_Product_ID());
                }
            }
            return success;
        }

        /**
         * 	Before Delete
         *	@return true if it can be deleted
         */
        protected override bool BeforeDelete()
        {
            //	Check Storage
            if (IsStocked() || PRODUCTTYPE_Item.Equals(GetProductType()))
            {
                MStorage[] storages = MStorage.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
                Decimal OnHand = Env.ZERO;
                Decimal Ordered = Env.ZERO;
                Decimal Reserved = Env.ZERO;
                for (int i = 0; i < storages.Length; i++)
                {
                    OnHand = Decimal.Add(OnHand, (storages[i].GetQtyOnHand()));
                    Ordered = Decimal.Add(OnHand, (storages[i].GetQtyOrdered()));
                    Reserved = Decimal.Add(OnHand, (storages[i].GetQtyReserved()));
                }
                String errMsg = "";
                if (Env.Signum(OnHand) != 0)
                    errMsg = "@QtyOnHand@ = " + OnHand;
                if (Env.Signum(Ordered) != 0)
                    errMsg += " - @QtyOrdered@ = " + Ordered;
                if (Env.Signum(Reserved) != 0)
                    errMsg += " - @QtyReserved@" + Reserved;
                if (errMsg.Length > 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), errMsg));
                    return false;
                }

            }
            //	delete costing           
            MProductCosting[] costings = MProductCosting.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
            for (int i = 0; i < costings.Length; i++)
                costings[i].Delete(true, Get_TrxName());
            string sql = "DELETE FROM VAM_ProductCost WHERE VAM_Product_ID = " + Get_ID();
            int no = DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no < 0)
            {
                return false;
            }
            return Delete_Accounting("VAM_Product_Acct");
        }
        #region Class To Get Nutrition Json Response
        public class rootObject
        {
            public report report { get; set; }
        }
        public class report
        {
            //public int sr { get; set; }
            //public string type { get; set; }
            public food food { get; set; }
        }
        public class food
        {
            //public string ndbno { get; set; }
            //public string name { get; set; }
            //public string sd { get; set; }
            //public string fg { get; set; }
            //public string sn { get; set; }
            //public string cn { get; set; }
            //public string menu { get; set; }
            //public decimal nf { get; set; }
            //public decimal cf { get; set; }
            //public decimal ff { get; set; }
            //public decimal pf { get; set; }
            //public string r { get; set; }
            //public string rd { get; set; }
            //public string ds { get; set; }
            public List<nutrients> nutrients { get; set; }
        }
        public class nutrients
        {
            public string group { get; set; }
            public string name { get; set; }
            public int nutrient_id { get; set; }
            public string unit { get; set; }
            public decimal value { get; set; }
        }
        #endregion
        public void CallNutritionApi(string NDBNo, int ProductID)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://api.nal.usda.gov/ndb/reports/?format=json&type=b&api_key=HBS3lRZUgBIxXOSF1DQBKW7GJw6M6e2J4cFMzSSP&ndbno=" + NDBNo + "");
                client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObject = response.Content.ReadAsAsync<rootObject>().Result;
                    foreach (nutrients nutrient in dataObject.report.food.nutrients)
                    {
                        int TblPrimaryKey = 0;
                        object objNT = DB.ExecuteScalar("select va019_nutrition_id from va019_nutrition where VAM_Product_id=" + ProductID + " and va019_nutrition_key=" + nutrient.nutrient_id + "");
                        if (objNT != null && objNT != DBNull.Value)
                        {
                            TblPrimaryKey = Convert.ToInt32(objNT);
                        }
                        var Dll = Assembly.Load("VA019Svc");
                        var X_VA019_Nutrition = Dll.GetType("ViennaAdvantage.Model.MVA019Nutrition");
                        ConstructorInfo conInfo = X_VA019_Nutrition.GetConstructor(new[] { typeof(Ctx), typeof(int), typeof(Trx), typeof(int), typeof(string), typeof(int), typeof(string), typeof(decimal) });
                        conInfo.Invoke(new object[] { p_ctx, TblPrimaryKey, null, ProductID, nutrient.name, nutrient.nutrient_id, nutrient.unit, nutrient.value });
                    }

                }
                else
                {
                    log.SaveError("Nutretion API Response Falier", "");
                }
            }
            catch (Exception e)
            {
                log.SaveError("Nutretion API Error", e);
            }
        }

    }
}
