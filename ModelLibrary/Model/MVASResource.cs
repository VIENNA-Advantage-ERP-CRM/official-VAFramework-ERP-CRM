/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVASResource
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MVASResource : X_VAS_Resource
    {
        // Cached Resource Type	
        private MVASResType _resourceType = null;
        // Cached Product			
        private MVAMProduct _product = null;

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAS_Resource_ID id
         */
        public MVASResource(Ctx ctx, int VAS_Resource_ID, Trx trxName)
            : base(ctx, VAS_Resource_ID, trxName)
        {
            
        }	

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         */
        public MVASResource(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	

        /**
         * 	Get cached Resource Type
         *	@return Resource Type
         */
        public MVASResType GetResourceType()
        {
            if (_resourceType == null && GetVAS_Res_Type_ID() != 0)
                _resourceType = new MVASResType(GetCtx(), GetVAS_Res_Type_ID(), Get_TrxName());
            return _resourceType;
        }	

        /**
         * 	Get Product
         *	@return product
         */
        public MVAMProduct GetProduct()
        {
            if (_product == null)
            {
                MVAMProduct[] products = MVAMProduct.Get(GetCtx(), "VAS_Resource_ID=" + GetS_Resource_ID(), Get_TrxName());
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
            string sql = "select count(*) from VAS_Resource where VAF_UserContact_ID = " + GetVAF_UserContact_ID() + " and isactive = 'Y' and VAS_Resource_ID <> " + GetS_Resource_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (count > 0)
            {
                // ShowMessage.Info("UserAlreadyAssignedToResource", true, null, null);
                log.SaveError("UserAlreadyAssignedToResource", "");
                return false;
            }

            if (newRecord && Convert.ToString(GetVS_CATEGORY()) == "P")
            {

                /**inserted new field as Resource category(VS_Category)on the resource table*/
                if (GetValue() == null || GetValue().Length == 0)
                    SetValue(GetName());
                _product = new MVAMProduct(this, GetResourceType());
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
                MVAMProduct prod = GetProduct();
                if (prod == null)
                {
                    if (GetValue() == null || GetValue().Length == 0)
                        SetValue(GetName());
                    _product = new MVAMProduct(this, GetResourceType());
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
