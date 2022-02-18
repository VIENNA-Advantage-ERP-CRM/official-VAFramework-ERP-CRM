/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductCategory
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
using System.Data.SqlClient;


namespace VAdvantage.Model
{
    public class MProductCategory : X_M_Product_Category
    {
        /**	Categopry Cache				*/
        private static CCache<int, MProductCategory> s_cache = new CCache<int, MProductCategory>("M_Product_Category", 20);
        /**	Product Cache				*/
        private static CCache<int, int?> s_products = new CCache<int, int?>("M_Product", 100);
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductCategory).FullName);

        /* 	Get from Cache
        *	@param ctx context
        *	@param M_Product_Category_ID id
        *	@return category
        */
        public static MProductCategory Get(Ctx ctx, int M_Product_Category_ID)
        {
            int ii = M_Product_Category_ID;
            MProductCategory pc = (MProductCategory)s_cache[ii];
            if (pc == null)
            {
                pc = new MProductCategory(ctx, M_Product_Category_ID, null);
                s_cache.Add(ii, pc);
            }
            return pc;
        }

        /**
         * 	Is Product in Category
         *	@param M_Product_Category_ID category
         *	@param M_Product_ID product
         *	@return true if product has category
         */
        public static bool IsCategory(int M_Product_Category_ID, int M_Product_ID)
        {
            if (M_Product_ID == 0 || M_Product_Category_ID == 0)
                return false;
            //	Look up
            int product = (int)M_Product_ID;
            int? category = s_products[product];
            if (category != null)
                return category == M_Product_Category_ID;

            String sql = "SELECT M_Product_Category_ID FROM M_Product WHERE M_Product_ID=" + M_Product_ID;
            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    category = (int?)rs[0];
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            if (category != null)
            {
                //	TODO: LRU logic  
                s_products.Add(product, category);
                //
                _log.Fine("M_Product_ID=" + M_Product_ID + "(" + category
                    + ") in M_Product_Category_ID=" + M_Product_Category_ID
                    + " - " + (category == M_Product_Category_ID));
                return category.Value == M_Product_Category_ID;
            }
            _log.Log(Level.SEVERE, "Not found M_Product_ID=" + M_Product_ID);
            return false;
        }

        /**************************************************************************
         * 	Default Constructor
         *	@param ctx context
         *	@param M_Product_Category_ID id
         *	@param trxName transaction
         */
        public MProductCategory(Ctx ctx, int M_Product_Category_ID, Trx trxName)
            : base(ctx, M_Product_Category_ID, trxName)
        {

            if (M_Product_Category_ID == 0)
            {
                //	setName (null);
                //	setValue (null);
                SetMMPolicy(MMPOLICY_FiFo);	// F
                SetPlannedMargin(Env.ZERO);
                SetIsDefault(false);
                SetIsSelfService(true);	// Y
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MProductCategory(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Product_Category_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                // Get "Related-to" of "Product"
                _sql.Clear();
                _sql.Append(@"SELECT L.Value FROM AD_Ref_List L INNER JOIN AD_Reference r on (R.AD_Reference_ID=L.AD_Reference_ID)
                                    WHERE l.isActive = 'Y' AND r.Name='FRPT_RelatedTo' AND (l.Name='Product')");
                string relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO prdctact = null;
                _client_ID = GetAD_Client_ID();
                _sql.Clear();
                _sql.Append("select C_AcctSchema_ID from C_AcctSchema where AD_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["C_AcctSchema_ID"]);

                        // Get Default Account from Accounting Schema
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,C_Validcombination_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND AD_CLIENT_ID=" + _client_ID + "AND C_Acctschema_Id=" + _AcctSchema_ID);
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(_sql.ToString(), null, Get_Trx());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "")
                                {
                                    if (_relatedTo == relatedtoProduct)
                                    {
                                        _sql.Clear();
                                        _sql.Append(@"Select count(*) From M_Product_Category Bp
                                                       Left Join FRPT_Product_Category_Acct ca On Bp.M_Product_Category_ID=ca.M_Product_Category_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                                       + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + _client_ID +
                                                       " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) +
                                                       " AND Bp.M_Product_Category_ID = " + GetM_Product_Category_ID());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        if (recordFound == 0)
                                        {
                                            prdctact = MTable.GetPO(GetCtx(), "FRPT_Product_Category_Acct", 0, null);
                                            prdctact.Set_ValueNoCheck("AD_Org_ID", 0);
                                            prdctact.Set_ValueNoCheck("M_Product_Category_ID", Util.GetValueOfInt(GetM_Product_Category_ID()));
                                            prdctact.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            prdctact.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                            prdctact.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);
                                            if (!prdctact.Save())
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
                {
                    bool sucs = Insert_Accounting("M_Product_Category_Acct", "C_AcctSchema_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }
                }
                return success;
            }
            return success;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // if finished and semifinished is selected in Product Group then costing method should be standard costing
            if (Env.IsModuleInstalled("VA073_"))
            {
                if (Get_ColumnIndex("VA073_ProductGroup") >= 0)
                {
                    if (Util.GetValueOfString(Get_Value("VA073_ProductGroup")).Equals(MProductCategory.VA073_PRODUCTGROUP_FinishedProduct) ||
                        Util.GetValueOfString(Get_Value("VA073_ProductGroup")).Equals(MProductCategory.VA073_PRODUCTGROUP_Semi_FinishedProduct))
                    {
                        if (Get_ColumnIndex("CostingMethod") >= 0 && (GetCostingMethod() == null || !GetCostingMethod().Equals(MProductCategory.COSTINGMETHOD_StandardCosting)))
                        {
                            log.SaveError("VA073_SelectStdCosting", "");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("M_Product_Category_Acct");
        }

        /**
         * 	FiFo Material Movement Policy
         *	@return true if FiFo
         */
        public bool IsFiFo()
        {
            return MMPOLICY_FiFo.Equals(GetMMPolicy());
        }

        // Added by Mohit 20-8-2015 VAWMS
        public static MProductCategory GetOfProduct(Ctx ctx, int M_Product_ID)
        {
            MProductCategory retValue = null;
            //PreparedStatement pstmt = null;
            //ResultSet rs = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            String sql = "SELECT * FROM M_Product_Category pc "
                        + "WHERE EXISTS (SELECT * FROM M_Product p "
                        + "WHERE p.M_Product_ID=@param1 AND p.M_Product_Category_ID=pc.M_Product_Category_ID)";
            try
            {
                //pstmt = DB.prepareStatement(sql, (Trx)null);
                //pstmt.setInt(1, M_Product_ID);
                //rs = pstmt.executeQuery();
                //if (rs.next())
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", M_Product_ID);
                idr = DB.ExecuteReader(sql, param, null);
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    retValue = new MProductCategory(ctx, dt.Rows[0], null);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            //finally
            //{
            //    DB.closeResultSet(rs);
            //    DB.closeStatement(pstmt);
            //}
            return retValue;
        }
        //END

        // Added by Manjot 20-8-2015 VAMFG
        public MProduct[] GetProductsofCategory(String WhereClause, Trx trx)
        {
            List<MProduct> list = new List<MProduct>();
            StringBuilder sql = new StringBuilder(" SELECT * FROM M_Product WHERE M_Product_Category_ID = @param1 ");

            if (WhereClause != null && WhereClause.Length != 0)
                sql.Append(WhereClause);
            MRole role = MRole.GetDefault(GetCtx(), false);
            String stmt = role.AddAccessSQL(sql.ToString(), "M_Product", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetM_Product_Category_ID());
                idr = DB.ExecuteReader(sql.ToString(), param, null);
                dt.Load(idr);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MProduct product = new MProduct(GetCtx(), dt.Rows[i], Get_TrxName());
                    list.Add(product);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql.ToString(), e);

            }
            MProduct[] retVal = new MProduct[list.Count];
            retVal = list.ToArray();
            return retVal;
        }
        //End
    }
}