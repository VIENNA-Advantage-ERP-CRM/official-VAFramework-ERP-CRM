/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMProductCategory
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;


namespace VAdvantage.Model
{
    public class MVAMProductCategory : X_VAM_ProductCategory
    {
        /**	Categopry Cache				*/
        private static CCache<int, MVAMProductCategory> s_cache = new CCache<int, MVAMProductCategory>("VAM_ProductCategory", 20);
        /**	Product Cache				*/
        private static CCache<int, int?> s_products = new CCache<int, int?>("VAM_Product", 100);
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductCategory).FullName);

        /* 	Get from Cache
        *	@param ctx context
        *	@param VAM_ProductCategory_ID id
        *	@return category
        */
        public static MVAMProductCategory Get(Ctx ctx, int VAM_ProductCategory_ID)
        {
            int ii = VAM_ProductCategory_ID;
            MVAMProductCategory pc = (MVAMProductCategory)s_cache[ii];
            if (pc == null)
                pc = new MVAMProductCategory(ctx, VAM_ProductCategory_ID, null);
            return pc;
        }

        /**
         * 	Is Product in Category
         *	@param VAM_ProductCategory_ID category
         *	@param VAM_Product_ID product
         *	@return true if product has category
         */
        public static bool IsCategory(int VAM_ProductCategory_ID, int VAM_Product_ID)
        {
            if (VAM_Product_ID == 0 || VAM_ProductCategory_ID == 0)
                return false;
            //	Look up
            int product = (int)VAM_Product_ID;
            int? category = s_products[product];
            if (category != null)
                return category == VAM_ProductCategory_ID;

            String sql = "SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE VAM_Product_ID=" + VAM_Product_ID;
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
                _log.Fine("VAM_Product_ID=" + VAM_Product_ID + "(" + category
                    + ") in VAM_ProductCategory_ID=" + VAM_ProductCategory_ID
                    + " - " + (category == VAM_ProductCategory_ID));
                return category.Value == VAM_ProductCategory_ID;
            }
            _log.Log(Level.SEVERE, "Not found VAM_Product_ID=" + VAM_Product_ID);
            return false;
        }

        /**************************************************************************
         * 	Default Constructor
         *	@param ctx context
         *	@param VAM_ProductCategory_ID id
         *	@param trxName transaction
         */
        public MVAMProductCategory(Ctx ctx, int VAM_ProductCategory_ID, Trx trxName)
            : base(ctx, VAM_ProductCategory_ID, trxName)
        {

            if (VAM_ProductCategory_ID == 0)
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
        public MVAMProductCategory(Ctx ctx, DataRow rs, Trx trxName)
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
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_Product_Category_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Product_Category_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Product_Category_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Product'");
                var relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO prdctact = null;
                _client_ID = GetVAF_Client_ID();
                _sql.Clear();
                _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where VAF_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + _client_ID + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(_sql.ToString(), null, Get_Trx());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                // DataSet ds2 = new DataSet();
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "")
                                {

                                    if (_relatedTo == relatedtoProduct)
                                    {
                                        _sql.Clear();
                                        //                                        _sql.Append(@"Select Bp.VAM_ProductCategory_ID,ca.Frpt_Acctdefault_Id From VAM_ProductCategory Bp
                                        //                                                                                               Left Join FRPT_Product_Category_Acct ca On Bp.VAM_ProductCategory_ID=ca.VAM_ProductCategory_ID 
                                        //                                                                                                And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                        //                                                        + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID +
                                        //                                                        " AND VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                        _sql.Append(@"Select count(*) From VAM_ProductCategory Bp
                                                       Left Join FRPT_Product_Category_Acct ca On Bp.VAM_ProductCategory_ID=ca.VAM_ProductCategory_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                                       + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID +
                                                       " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) +
                                                       " AND Bp.VAM_ProductCategory_ID = " + GetVAM_ProductCategory_ID());
                                        //ds2 = DB.ExecuteDataset(_sql.ToString(), null , Get_Trx());
                                        int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                        //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                        //{
                                        //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                        //    {
                                        //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                        //        if (value == 0)
                                        //        {
                                        //prdctact = new X_FRPT_Product_Category_Acct(GetCtx(), 0, null);
                                        if (recordFound == 0)
                                        {
                                            prdctact = MVAFTableView.GetPO(GetCtx(), "FRPT_Product_Category_Acct", 0, null);
                                            prdctact.Set_ValueNoCheck("VAF_Org_ID", 0);
                                            //prdctact.Set_ValueNoCheck("VAM_ProductCategory_ID", Util.GetValueOfInt(ds2.Tables[0].Rows[j]["VAM_ProductCategory_ID"]));
                                            prdctact.Set_ValueNoCheck("VAM_ProductCategory_ID", Util.GetValueOfInt(GetVAM_ProductCategory_ID()));
                                            prdctact.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                            prdctact.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                            prdctact.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
                                            if (!prdctact.Save())
                                            {

                                            }
                                        }
                                        //}
                                        //}
                                        //}
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
                    bool sucs = Insert_Accounting("VAM_ProductCategory_Acct", "VAB_AccountBook_Default", null);

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

        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("VAM_ProductCategory_Acct");
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
        public static MVAMProductCategory GetOfProduct(Ctx ctx, int VAM_Product_ID)
        {
            MVAMProductCategory retValue = null;
            //PreparedStatement pstmt = null;
            //ResultSet rs = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            String sql = "SELECT * FROM VAM_ProductCategory pc "
                        + "WHERE EXISTS (SELECT * FROM VAM_Product p "
                        + "WHERE p.VAM_Product_ID=@param1 AND p.VAM_ProductCategory_ID=pc.VAM_ProductCategory_ID)";
            try
            {
                //pstmt = DB.prepareStatement(sql, (Trx)null);
                //pstmt.setInt(1, VAM_Product_ID);
                //rs = pstmt.executeQuery();
                //if (rs.next())
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", VAM_Product_ID);
                idr = DB.ExecuteReader(sql, param, null);
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    retValue = new MVAMProductCategory(ctx, dt.Rows[0], null);
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
        public MVAMProduct[] GetProductsofCategory(String WhereClause, Trx trx)
        {
            List<MVAMProduct> list = new List<MVAMProduct>();
            StringBuilder sql = new StringBuilder(" SELECT * FROM VAM_Product WHERE VAM_ProductCategory_ID = @param1 ");

            if (WhereClause != null && WhereClause.Length != 0)
                sql.Append(WhereClause);
            MVAFRole role = MVAFRole.GetDefault(GetCtx(), false);
            String stmt = role.AddAccessSQL(sql.ToString(), "VAM_Product", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO);

            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetVAM_ProductCategory_ID());
                idr = DB.ExecuteReader(sql.ToString(), param, null);
                dt.Load(idr);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MVAMProduct product = new MVAMProduct(GetCtx(), dt.Rows[i], Get_TrxName());
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
            MVAMProduct[] retVal = new MVAMProduct[list.Count];
            retVal = list.ToArray();
            return retVal;
        }
        //End
    }
}