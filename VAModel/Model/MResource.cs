/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MResource
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     15-Jun-2009
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

namespace VAdvantage.Model
{
    public class MResource : X_S_Resource
    {
        // Cached Resource Type	
        private MResourceType _resourceType = null;
        // Cached Product			
        private MProduct _product = null;

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param S_Resource_ID id
         */
        public MResource(Ctx ctx, int S_Resource_ID, Trx trxName)
            : base(ctx, S_Resource_ID, trxName)
        {
            
        }	

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         */
        public MResource(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	

        /**
         * 	Get cached Resource Type
         *	@return Resource Type
         */
        public MResourceType GetResourceType()
        {
            if (_resourceType == null && GetS_ResourceType_ID() != 0)
                _resourceType = new MResourceType(GetCtx(), GetS_ResourceType_ID(), Get_TrxName());
            return _resourceType;
        }	

        /**
         * 	Get Product
         *	@return product
         */
        public MProduct GetProduct()
        {
            if (_product == null)
            {
                MProduct[] products = MProduct.Get(GetCtx(), "S_Resource_ID=" + GetS_Resource_ID(), Get_TrxName());
                if (products.Length > 0)
                    _product = products[0];
            }
            return _product;
        }	

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if it can be saved
         */


        /** 
        *  @ Modified by Vijayamurugan D
        *  Modified for has not to update the product table 
        *  while creating the new resource 
        *  NOTE: The RESOURCE CATEGORY field should be 'Profile' if it's product it will update on
        *  product window.
        */
        protected override bool BeforeSave(bool newRecord)
        {
            string sql = "select count(*) from s_resource where ad_user_ID = " + GetAD_User_ID() + " and isactive = 'Y' and s_resource_ID <> " + GetS_Resource_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (count > 0)
            {
                // ShowMessage.Info("UserAlreadyAssignedToResource", true, null, null);
                return false;
            }

            if (newRecord && Convert.ToString(GetVS_CATEGORY()) == "P")
            {

                /**inserted new field as Resource category(VS_Category)on the resource table*/
                if (GetValue() == null || GetValue().Length == 0)
                    SetValue(GetName());
                _product = new MProduct(this, GetResourceType());
                return _product.Save(Get_TrxName());
            }
            return true;
        }	

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        /** 
         * @Modified by Vijayamurugan D
         * */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            if (Convert.ToString(GetVS_CATEGORY())=="P")
            {
                MProduct prod = GetProduct();
                if (prod == null)
                {
                    if (GetValue() == null || GetValue().Length == 0)
                        SetValue(GetName());
                    _product = new MProduct(this, GetResourceType());
                    return _product.Save(Get_TrxName());
                }
                else
                    if (prod.SetResource(this))
                        prod.Save(Get_TrxName());
            }
            return success;
        }	
	
    }
}
