/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMVAMProductCostElement
 * Purpose        : Product/element cost purpose 
 * Class Used     : X_VAM_ProductCostElement
 * Chronological    Development
 * Raghunandan     18-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMProductCostElement : X_VAM_ProductCostElement
    {
        //Cache					
        private static CCache<int, MVAMProductCostElement> s_cache = new CCache<int, MVAMProductCostElement>("VAM_ProductCostElement", 20);

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductCostElement).FullName);


        /**
         * 	Get Material Cost Element or create it
         *	@param po parent
         *	@param CostingMethod method
         *	@return cost element
         */
        public static MVAMProductCostElement GetMaterialCostElement(PO po, String CostingMethod)
        {
            if (CostingMethod == null || CostingMethod.Length == 0)
            {
                _log.Severe("No CostingMethod");
                return null;
            }
            //
            MVAMProductCostElement retValue = null;
            String sql = "SELECT * FROM VAM_ProductCostElement WHERE VAF_Client_ID=" + po.GetVAF_Client_ID() + " AND CostingMethod=@costingMethod ORDER BY VAF_Org_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@costingMethod", CostingMethod);

                idr = DataBase.DB.ExecuteReader(sql, param, po.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                //bool n = dr.next(); //jz to fix DB2 resultSet closed problem
                //if (n)
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMProductCostElement(po.GetCtx(), dr, po.Get_TrxName());
                }
                //if (n && dr.next())
                //    s_log.warning("More then one Material Cost Element for CostingMethod=" + CostingMethod);
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
                dt = null;
            }

            if (retValue != null)
                return retValue;

            //	Create New
            retValue = new MVAMProductCostElement(po.GetCtx(), 0, po.Get_TrxName());
            retValue.SetClientOrg(po.GetVAF_Client_ID(), 0);
            String name = MVAFCtrlRefList.GetListName(po.GetCtx(), COSTINGMETHOD_VAF_Control_Ref_ID, CostingMethod);
            if (name == null || name.Length == 0)
                name = CostingMethod;
            retValue.SetName(name);
            retValue.SetCostElementType(COSTELEMENTTYPE_Material);
            retValue.SetCostingMethod(CostingMethod);
            retValue.Save();
            return retValue;
        }

        /**
         * 	Get first Material Cost Element
         *	@param ctx context
         *	@param CostingMethod costing method
         *	@return Cost Element or null
         */
        public static MVAMProductCostElement GetMaterialCostElement(Ctx ctx, String CostingMethod)
        {
            MVAMProductCostElement retValue = null;
            String sql = "SELECT * FROM VAM_ProductCostElement WHERE VAF_Client_ID=" + ctx.GetVAF_Client_ID() + " AND CostingMethod=@CostingMethod ORDER BY VAF_Org_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@CostingMethod", CostingMethod);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMProductCostElement(ctx, dr, null);
                }
                //if (dr.next())
                //    s_log.info("More then one Material Cost Element for CostingMethod=" + CostingMethod);
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
                dt = null;
            }

            return retValue;
        }


        /**
         * 	Get active Material Cost Element for client 
         *	@param po parent
         *	@return cost element array
         */
        public static MVAMProductCostElement[] GetCostingMethods(PO po)
        {
            List<MVAMProductCostElement> list = new List<MVAMProductCostElement>();
            String sql = "SELECT * FROM VAM_ProductCostElement "
                + "WHERE VAF_Client_ID=@Client_ID"
                + " AND IsActive='Y' AND CostElementType='M' AND CostingMethod IS NOT NULL";
            DataTable dt = null;
            //IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@Client_ID", po.GetVAF_Client_ID());
                //idr = DataBase.DB.ExecuteReader(sql, param, po.Get_TrxName());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, po.Get_TrxName());
                dt = new DataTable();
                dt = ds.Tables[0];
                //idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMProductCostElement(po.GetCtx(), dr, po.Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                //if (idr != null)
                // {
                //     idr.Close();
                //}
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MVAMProductCostElement[] retValue = new MVAMProductCostElement[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        // By Amit 23-12-2015 not used
        public static MVAMProductCostElement[] GetMaterialCostingMethods(PO po)
        {
            List<MVAMProductCostElement> list = new List<MVAMProductCostElement>();
            String sql = "SELECT * FROM VAM_ProductCostElement "
                + "WHERE VAF_Client_ID=@Client_ID"
                + " AND IsActive='Y' AND CostElementType='M'";
            DataTable dt = null;
            //IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@Client_ID", po.GetVAF_Client_ID());
                //idr = DataBase.DB.ExecuteReader(sql, param, po.Get_TrxName());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, po.Get_TrxName());
                dt = new DataTable();
                dt = ds.Tables[0];
                //idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMProductCostElement(po.GetCtx(), dr, po.Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                //if (idr != null)
                // {
                //     idr.Close();
                //}
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MVAMProductCostElement[] retValue = new MVAMProductCostElement[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Get Cost Element from Cache
         *	@param ctx context
         *	@param VAM_ProductCostElement_ID id
         *	@return Cost Element
         */
        public static MVAMProductCostElement Get(Ctx ctx, int VAM_ProductCostElement_ID)
        {
            int key = VAM_ProductCostElement_ID;
            MVAMProductCostElement retValue = (MVAMProductCostElement)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAMProductCostElement(ctx, VAM_ProductCostElement_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }


        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_ProductCostElement_ID id
         *	@param trxName trx
         */
        public MVAMProductCostElement(Ctx ctx, int VAM_ProductCostElement_ID, Trx trxName)
            : base(ctx, VAM_ProductCostElement_ID, trxName)
        {
            if (VAM_ProductCostElement_ID == 0)
            {
                //	setName (null);
                SetCostElementType(COSTELEMENTTYPE_Material);
                SetIsCalculated(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MVAMProductCostElement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Check Unique Costing Method
            if (COSTELEMENTTYPE_Material.Equals(GetCostElementType())
                && (newRecord || Is_ValueChanged("CostingMethod")))
            {
                String sql = "SELECT COALESCE(MAX(VAM_ProductCostElement_ID),0) FROM VAM_ProductCostElement "
                    + "WHERE VAF_Client_ID=" + GetVAF_Client_ID() + " AND CostingMethod='" + GetCostingMethod() + "'";
                int id = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                if (id > 0 && id != Get_ID())
                {
                    log.SaveError("AlreadyExists", Msg.GetElement(GetCtx(), "CostingMethod"));
                    return false;
                }
            }

            //	Maintain Calclated
            if (COSTELEMENTTYPE_Material.Equals(GetCostElementType()))
            {
                String cm = GetCostingMethod();
                if (cm == null || cm.Length == 0
                    || COSTINGMETHOD_StandardCosting.Equals(cm))
                    SetIsCalculated(false);
                else
                    SetIsCalculated(true);
            }
            else if (!(COSTELEMENTTYPE_CostCombination.Equals(GetCostElementType())))
            {
                if (IsCalculated())
                    SetIsCalculated(false);
                if (GetCostingMethod() != null)
                    SetCostingMethod(null);
            }

            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            return true;
        }

        /**
         * 	Before Delete
         *	@return true if can be deleted
         */
        protected override bool BeforeDelete()
        {
            String cm = GetCostingMethod();
            if (cm == null
                || !COSTELEMENTTYPE_Material.Equals(GetCostElementType()))
                return true;
            // JID_0096: System should not allow to delete the costing element if costing is already calculated against that element
            String qry = "SELECT Count(VAM_ProductCostElement_ID) FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAF_Client_ID=" + GetVAF_Client_ID() + " AND VAM_ProductCostElement_ID=" + GetVAM_ProductCostElement_ID();
            int id = Util.GetValueOfInt(DataBase.DB.ExecuteScalar(qry, null, Get_Trx()));
            if (id > 0)
            {
                log.SaveError("CannotDeleteUsed", Msg.GetElement(GetCtx(), "CostingMethod"));
                return false;
            }

            //	Costing Methods on AS level
            MVABAccountBook[] ass = MVABAccountBook.GetClientAcctSchema(GetCtx(), GetVAF_Client_ID());
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].GetCostingMethod().Equals(GetCostingMethod()))
                {
                    log.SaveError("CannotDeleteUsed", Msg.GetElement(GetCtx(), "VAB_AccountBook_ID")
                       + " - " + ass[i].GetName());
                    return false;
                }
            }

            //	Costing Methods on PC level
            String sql = "SELECT VAM_ProductCategory_ID FROM VAM_ProductCategory_Acct WHERE VAF_Client_ID=" + GetVAF_Client_ID() + " AND CostingMethod=@costingMethod";
            int VAM_ProductCategory_ID = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@costingMethod", GetCostingMethod());

                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    VAM_ProductCategory_ID = Convert.ToInt32(dr[0]);
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
                dt = null;
            }

            if (VAM_ProductCategory_ID != 0)
            {
                log.SaveError("CannotDeleteUsed", Msg.GetElement(GetCtx(), "VAM_ProductCategory_ID")
                   + " (ID=" + VAM_ProductCategory_ID + ")");
                return false;
            }
            return true;
        }

        /**
         * 	Is this a Costing Method
         *	@return true if not Material cost or no costing method.
         */
        public bool IsCostingMethod()
        {
            return COSTELEMENTTYPE_Material.Equals(GetCostElementType())
                && GetCostingMethod() != null;
        }

        /**
         * 	Is Avg Invoice Costing Method
         *	@return true if AverageInvoice
         */
        public bool IsAverageInvoice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_AverageInvoice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Avg PO Costing Method
         *	@return true if AveragePO
         */
        public bool IsAveragePO()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_AveragePO)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is FiFo Costing Method
         *	@return true if Fifo
         */
        public bool IsFifo()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_Fifo)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Last Invoice Costing Method
         *	@return true if LastInvoice
         */
        public bool IsLastInvoice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_LastInvoice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Last PO Costing Method
         *	@return true if LastPOPrice
         */
        public bool IsLastPOPrice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_LastPOPrice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is LiFo Costing Method
         *	@return true if Lifo
         */
        public bool IsLifo()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_Lifo)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Std Costing Method
         *	@return true if StandardCosting
         */
        public bool IsStandardCosting()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_StandardCosting)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is User Costing Method
         *	@return true if User Defined
         */
        public bool IsUserDefined()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_UserDefined)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }


        /**
         * 	Is Weighted Average Cost
         *	@return true if Weighted Average Cost
         */
        public bool IsWeightedAverageCost()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_WeightedAverageCost)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Weighted Average PO
         *	@return true if Weighted Average PO
         */
        public bool IsWeightedAveragePO()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_WeightedAveragePO)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }


        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMVAMProductCostElement[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Type=").Append(GetCostElementType())
                .Append(",Method=").Append(GetCostingMethod())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Cost element
        /// </summary>
        /// <param name="VAF_Client_ID"></param>
        /// <returns></returns>
        public static int GetMfgCostElement(int VAF_Client_ID)
        {
            int ce = 0;
            String sql = " SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE VAF_Client_ID =" + VAF_Client_ID + " AND IsMfgMaterialCost = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                if (idr.Read())
                {
                    ce = Util.GetValueOfInt(idr[0]);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return ce;
        }

        /// <summary>
        /// Is used to check costing method belongs to PO costing method  like (Average PO, Weighted Average PO or Last PO)
        /// Firts we check costing method on Product category, if not found then we will check on Primary Accounting Schema
        /// </summary>
        /// <param name="ctx">current context</param>
        /// <param name="VAF_Client_ID">Client reference</param>
        /// <param name="VAM_Product_ID">Product whom costing method is to be determine</param>
        /// <param name="trxName">Transaction</param>
        /// <returns>True/False</returns>
        public static bool IsPOCostingmethod(Ctx ctx, int VAF_Client_ID, int VAM_Product_ID, Trx trxName)
        {
            MVAMProductCategory pc = null;
            bool isPOcostingMethod = false;
            string costingMethod = null;
            MVAFClient client = MVAFClient.Get(ctx, VAF_Client_ID);
            MVAMProduct product = MVAMProduct.Get(ctx, VAM_Product_ID);

            if (product != null)
            {
                pc = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pc != null)
                {
                    // check costing method from product category
                    costingMethod = pc.GetCostingMethod();
                    if (costingMethod == "C")
                    {
                        costingMethod = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT costingmethod FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_ID IN 
                                        (SELECT CAST(VAM_Ref_CostElement AS INTEGER) FROM VAM_CostElementLine WHERE VAM_ProductCostElement_ID=" + pc.GetVAM_ProductCostElement_ID() + @" )
                                        AND CostingMethod IS NOT NULL", null, trxName));
                    }
                }
                if (String.IsNullOrEmpty(costingMethod))
                {
                    // check costing method against primary accounting schema
                    MVAFClientDetail clientInfo = MVAFClientDetail.Get(ctx, VAF_Client_ID);
                    MVABAccountBook actSchema = MVABAccountBook.Get(ctx, clientInfo.GetVAB_AccountBook1_ID());
                    if (actSchema != null)
                    {
                        costingMethod = actSchema.GetCostingMethod();
                        if (costingMethod == "C")
                        {
                            costingMethod = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT costingmethod FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_ID IN 
                                        (SELECT CAST(VAM_Ref_CostElement AS INTEGER) FROM VAM_CostElementLine WHERE VAM_ProductCostElement_ID=" + actSchema.GetVAM_ProductCostElement_ID() + @" )
                                        AND CostingMethod IS NOT NULL", null, trxName));
                        }
                    }
                }
            }
            if (costingMethod.Equals(COSTINGMETHOD_WeightedAveragePO) ||
                costingMethod.Equals(COSTINGMETHOD_AveragePO) ||
                costingMethod.Equals(COSTINGMETHOD_LastPOPrice))
            {
                isPOcostingMethod = true;
            }
            else
            {
                isPOcostingMethod = false;
            }

            return isPOcostingMethod;
        }
    }
}
