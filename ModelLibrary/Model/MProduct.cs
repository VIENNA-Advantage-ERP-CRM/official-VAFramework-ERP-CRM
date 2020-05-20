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
using System.Windows.Forms;
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
    public class MProduct : X_M_Product
    {
        //	UOM Precision			
        private int? _precision = null;
        // Additional Downloads				
        private MProductDownload[] _downloads = null;
        //	Cache						
        private static CCache<int, MProduct> s_cache = new CCache<int, MProduct>("M_Product", 40, 5);	//	5 minutes
        //	Static Logger	*
        private static VLogger _log = VLogger.GetVLogger(typeof(MProduct).FullName);

        /// <summary>
        /// Get MProduct from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">id</param>
        /// <returns>MProduct</returns>
        public static MProduct Get(Ctx ctx, int M_Product_ID)
        {
            int key = M_Product_ID;
            MProduct retValue = (MProduct)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProduct(ctx, M_Product_ID, null);
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
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM M_Product";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE AD_Client_ID=" + AD_Client_ID + " AND " + whereClause;
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
            String sql = "SELECT * FROM M_Product "
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

            if (retValue != null && retValue.GetAD_Client_ID() != ctx.GetAD_Client_ID())
            {
                _log.Warning("ProductClient_ID=" + retValue.GetAD_Client_ID() + " <> EnvClient_ID=" + ctx.GetAD_Client_ID());
            }
            if (retValue != null && retValue.GetA_Asset_Group_ID() == 0)
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
        *	@param M_Product_ID id
        *	@return true if found and stocked - false otherwise
        */
        public static bool IsProductStocked(Ctx ctx, int M_Product_ID)
        {
            MProduct product = Get(ctx, M_Product_ID);
            return product.IsStocked();
        }

        /* 	Standard Constructor
         *	@param ctx context
         *	@param M_Product_ID id
         *	@param trxName transaction
         */
        public MProduct(Ctx ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            if (M_Product_ID == 0)
            {
                //	setValue (null);
                //	setName (null);
                //	setM_Product_Category_ID (0);
                //	setC_TaxCategory_ID (0);
                //	setC_UOM_ID (0);
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

            SetProductType(X_M_Product.PRODUCTTYPE_ExpenseType);
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

            SetProductType(X_M_Product.PRODUCTTYPE_Resource);
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
            PO.CopyValues(impP, this, impP.GetAD_Client_ID(), impP.GetAD_Org_ID());
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
            if (parent.GetS_ExpenseType_ID() != GetS_ExpenseType_ID())
            {
                SetS_ExpenseType_ID(parent.GetS_ExpenseType_ID());
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
            if (parent.GetC_UOM_ID() != GetC_UOM_ID())
            {
                SetC_UOM_ID(parent.GetC_UOM_ID());
                changed = true;
            }
            if (parent.GetM_Product_Category_ID() != GetM_Product_Category_ID())
            {
                SetM_Product_Category_ID(parent.GetM_Product_Category_ID());
                changed = true;
            }
            if (parent.GetC_TaxCategory_ID() != GetC_TaxCategory_ID())
            {
                SetC_TaxCategory_ID(parent.GetC_TaxCategory_ID());
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
            if (parent.GetC_UOM_ID() != GetC_UOM_ID())
            {
                SetC_UOM_ID(parent.GetC_UOM_ID());
                changed = true;
            }
            if (parent.GetM_Product_Category_ID() != GetM_Product_Category_ID())
            {
                SetM_Product_Category_ID(parent.GetM_Product_Category_ID());
                changed = true;
            }
            if (parent.GetC_TaxCategory_ID() != GetC_TaxCategory_ID())
            {
                SetC_TaxCategory_ID(parent.GetC_TaxCategory_ID());
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
                int C_UOM_ID = GetC_UOM_ID();
                if (C_UOM_ID == 0)
                    return 0;	//	EA
                _precision = (int)MUOM.GetPrecision(GetCtx(), C_UOM_ID);
            }
            return (int)_precision;
        }

        /**
        * 	Create Asset Group for this product
        *	@return asset group id
        */
        public int GetA_Asset_Group_ID()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            return pc.GetA_Asset_Group_ID();
        }


        /**
        * 	Create Asset for this product
        *	@return true if asset is created
        */
        public bool IsCreateAsset()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            return pc.GetA_Asset_Group_ID() != 0;
        }

        /* 	Get Attribute Set
        *	@return set or null
        */
        public MAttributeSet GetAttributeSet()
        {
            if (GetM_AttributeSet_ID() != 0)
                return MAttributeSet.Get(GetCtx(), GetM_AttributeSet_ID());
            return null;
        }


        /*	Has the Product Instance Attribute
         *	@return true if instance attributes
         */
        public bool IsInstanceAttribute()
        {
            if (GetM_AttributeSet_ID() == 0)
                return false;
            MAttributeSet mas = MAttributeSet.Get(GetCtx(), GetM_AttributeSet_ID());
            return mas.IsInstanceAttribute();
        }

        /**
        * 	Create One Asset Per UOM
        *	@return individual asset
        */
        public bool IsOneAssetPerUOM()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            if (pc.GetA_Asset_Group_ID() == 0)
                return false;
            MAssetGroup ag = MAssetGroup.Get(GetCtx(), pc.GetA_Asset_Group_ID());
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
            int C_UOM_ID = GetC_UOM_ID();
            if (C_UOM_ID == 0)
                return "";
            return MUOM.Get(GetCtx(), C_UOM_ID).GetUOMSymbol();
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
            String sql = "SELECT * FROM M_ProductDownload "
                + "WHERE M_Product_ID=" + GetM_Product_ID() + " AND IsActive='Y' ORDER BY Name";
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
            if (_precision != null && Is_ValueChanged("C_UOM_ID"))
                _precision = null;
            if (Util.GetValueOfInt(Env.GetCtx().GetContext("#AD_User_ID")) != 100)
            {
                if (Is_ValueChanged("C_UOM_ID") || Is_ValueChanged("M_AttributeSet_ID"))
                {
                    string uqry = "SELECT SUM(cc) as count FROM  (SELECT COUNT(*) AS cc FROM M_MovementLine WHERE M_Product_ID = " + GetM_Product_ID() + @"  UNION
                SELECT COUNT(*) AS cc FROM M_InventoryLine WHERE M_Product_ID = " + GetM_Product_ID() + " UNION SELECT COUNT(*) AS cc FROM C_OrderLine WHERE M_Product_ID = " + GetM_Product_ID() +
                    " UNION  SELECT COUNT(*) AS cc FROM M_InOutLine WHERE M_Product_ID = " + GetM_Product_ID() + ") t";
                    int no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                    if (no == 0 || IsBOM())
                    {
                        uqry = "SELECT count(*) FROM M_ProductionPlan WHERE M_Product_ID = " + GetM_Product_ID();
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
                string sql = "SELECT UPCUNIQUE('p','" + GetUPC() + "') as productID FROM Dual";
                int manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (manu_ID > 0)
                {
                    _log.SaveError("UPC is Unique", "");
                    return false;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(GetUPC()) &&
                   Util.GetValueOfString(Get_ValueOld("UPC")) != GetUPC())
                {
                    string sql = "SELECT UPCUNIQUE('p','" + GetUPC() + "') as productID FROM Dual";
                    int manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    //if (manu_ID != 0 && manu_ID != GetM_Product_ID())
                    if (manu_ID > 0)
                    {
                        _log.SaveError("UPC is Unique", "");
                        return false;
                    }
                }
            }
            return true;
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
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_Product_Category_Acct'");
            _sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Product_Category_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                PO obj = null;
                //MFRPTProductAcct obj = null;
                int _MProduct_ID = GetM_Product_ID();
                int _PCategory_ID = GetM_Product_Category_ID();
                string sql = "SELECT L.VALUE FROM AD_REF_LIST L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where   r.name='FRPT_RelatedTo' and l.name='Product'";
                //"select VALUE from AD_Ref_List where name='Product'";
                string _RelatedToProduct = Convert.ToString(DB.ExecuteScalar(sql));
                //string _RelatedToProduct = X_FRPT_AcctDefault.FRPT_RELATEDTO_Product.ToString();

                _sql.Clear();
                _sql.Append("Select Count(*) From FRPT_Product_Acct  where M_Product_ID=" + _MProduct_ID + " AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (value < 1)
                {
                    _sql.Clear();
                    _sql.Append("Select  PCA.c_acctschema_id, PCA.c_validcombination_id, PCA.frpt_acctdefault_id From FRPT_product_category_acct PCA inner join frpt_acctdefault ACC ON acc.frpt_acctdefault_id= PCA.frpt_acctdefault_id where PCA.m_product_category_id=" + _PCategory_ID + " and acc.frpt_relatedto=" + _RelatedToProduct + " AND PCA.IsActive = 'Y' AND PCA.AD_Client_ID = " + GetAD_Client_ID());
                    //_sql.Append("Select C_AcctSchema_ID, C_ValidCombination_ID, FRPT_AcctDefault_ID from FRPT_product_category_acct where m_product_category_id =" + _PCategory_ID);

                    DataSet ds = DB.ExecuteDataset(_sql.ToString());
                    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //obj = new MFRPTProductAcct(GetCtx(), 0, null);
                            obj = MTable.GetPO(GetCtx(), "FRPT_Product_Acct", 0, null);
                            obj.Set_ValueNoCheck("AD_Org_ID", 0);
                            obj.Set_ValueNoCheck("M_Product_ID", _MProduct_ID);
                            obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                            obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ValidCombination_ID"]));
                            obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                            if (!obj.Save())
                            { }
                        }
                    }
                }
                // Change by mohit amortization process
                //int _CountVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_'  AND IsActive = 'Y'"));
                //if (_CountVA038 > 0)
                //{
                //    if (GetProductType() == "E" || GetProductType() == "S")
                //    {
                //        if (Util.GetValueOfInt(Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                //        {
                //            DataSet _dsAcct = DB.ExecuteDataset("SELECT C_AcctSchema_ID, FRPT_AcctDefault_ID, C_VALIDCOMBINATION_ID, SEQNO FROM VA038_Amortization_Acct "
                //                              + "WHERE IsActive='Y' AND  VA038_AmortizationTemplate_ID=" + Util.GetValueOfInt(Get_Value("VA038_AmortizationTemplate_ID")));
                //            if (_dsAcct != null && _dsAcct.Tables[0].Rows.Count > 0)
                //            {
                //                for (int j = 0; j < _dsAcct.Tables[0].Rows.Count; j++)
                //                {
                //                    obj = MTable.GetPO(GetCtx(), "FRPT_Product_Acct", 0, null);
                //                    obj.Set_ValueNoCheck("AD_Org_ID", 0);
                //                    obj.Set_ValueNoCheck("M_Product_ID", _MProduct_ID);
                //                    obj.Set_ValueNoCheck("C_AcctSchema_ID", Util.GetValueOfInt(_dsAcct.Tables[0].Rows[j]["C_AcctSchema_ID"]));
                //                    obj.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(_dsAcct.Tables[0].Rows[j]["C_ValidCombination_ID"]));
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
                    MAccount.UpdateValueDescription(GetCtx(), "M_Product_ID=" + GetM_Product_ID(), Get_TrxName());

                //	Name/Description Change in Asset	MAsset.setValueNameDescription
                if (!newRecord && (Is_ValueChanged("Name") || Is_ValueChanged("Description")))
                {
                    String sql = " UPDATE A_Asset a SET Name=(SELECT SUBSTR(bp.Name || ' - ' || p.Name,1,60) FROM M_Product p, C_BPartner bp  WHERE p.M_Product_ID=a.M_Product_ID AND bp.C_BPartner_ID=a.C_BPartner_ID)," +
      "Description=(SELECT  p.Description FROM M_Product p, C_BPartner bp WHERE p.M_Product_ID=a.M_Product_ID AND bp.C_BPartner_ID=a.C_BPartner_ID)" +
      "WHERE IsActive='Y'  AND M_Product_ID=" + GetM_Product_ID();

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
                        bool sucs = Insert_Accounting("M_Product_Acct", "M_Product_Category_Acct",
                              "p.M_Product_Category_ID=" + GetM_Product_Category_ID());

                        //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                        // Before this, data was being saved but giving message "record not saved".
                        if (!sucs)
                        {
                            log.SaveWarning("AcctNotSaved", "");
                        }
                    }

                    //
                    MAcctSchema[] mass = MAcctSchema.GetClientAcctSchema(GetCtx(), GetAD_Client_ID(), Get_TrxName());
                    for (int i = 0; i < mass.Length; i++)
                    {
                        //	Old
                        MProductCosting pcOld = new MProductCosting(this, mass[i].GetC_AcctSchema_ID());
                        pcOld.Save();
                    }
                }
                //	New Costing
                // by Amit 22-12-2015
                //if (newRecord || Is_ValueChanged("M_Product_Category_ID"))
                //{
                //    MCost.Create(this);
                //}
            }

            //22-12-2015
            //by Amit for creating records ffor product foe all Costing Element whose costing elemnt type is 'Material'
            if (newRecord || Is_ValueChanged("M_Product_Category_ID"))
            {
                MCost.CreateRecords(this);
            }
            //20-12-2016
            //By Vivek Chauhan saving Nutrition value against product...........
            object ModuleId = DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where prefix='VA019_' and isactive='Y'");
            if (ModuleId != null && ModuleId != DBNull.Value)
            {
                object objNDBNo = DB.ExecuteScalar("select va019_ndbno from M_Product where m_product_ID=" + GetM_Product_ID() + "");
                if (objNDBNo != null && objNDBNo != DBNull.Value)
                {
                    CallNutritionApi(Convert.ToString(objNDBNo), GetM_Product_ID());
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
            string sql = "DELETE FROM M_Cost WHERE M_Product_ID = " + Get_ID();
            int no = DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no < 0)
            {
                return false;
            }
            return Delete_Accounting("M_Product_Acct");
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
                        object objNT = DB.ExecuteScalar("select va019_nutrition_id from va019_nutrition where m_product_id=" + ProductID + " and va019_nutrition_key=" + nutrient.nutrient_id + "");
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
