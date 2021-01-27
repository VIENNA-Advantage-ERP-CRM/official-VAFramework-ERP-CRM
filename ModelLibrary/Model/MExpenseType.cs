/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MExpenseType
 * Purpose        : Expense Type Model
 * Class Used     : X_VAS_ChargeType
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
    public class MExpenseType : X_VAS_ChargeType
    {
        // Cached Product			
        private MProduct _product = null;

        /* 	Default Constructor
        *	@param ctx context
        *	@param VAS_ChargeType_ID id
        *	@param trxName transaction
        */
        public MExpenseType(Ctx ctx, int VAS_ChargeType_ID, Trx trxName)
            : base(ctx, VAS_ChargeType_ID, trxName)
        {

        }

        /**
         * 	MExpenseType
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MExpenseType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Product
         *	@return product
         */
        public MProduct GetProduct()
        {
            if (_product == null)
            {
                MProduct[] products = MProduct.Get(GetCtx(), "VAS_ChargeType_ID=" + GetVAS_ChargeType_ID(), Get_TrxName());
                if (products.Length > 0)
                    _product = products[0];
            }
            return _product;
        }

        /**
         * 	beforeSave
         *	@see Model.PO#beforeSave(bool)
         *	@param newRecord
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                //commented by arpit on 5 Jan,2015 Mentis issue no. 0000274
                //if (GetValue() == null || GetValue().Length == 0)
                //    SetValue(GetName());
                //_product = new MProduct(this);
                //return _product.Save(Get_TrxName());
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
            //added by arpit on Jan 5,2015 Mentis issue no. 0000274       
            _product = new MProduct(this);
            _product.SetVAS_ChargeType_ID(GetVAS_ChargeType_ID());
            if (!_product.Save(Get_TrxName()))
            {

            }

            MProduct prod = GetProduct();
            if (prod.SetExpenseType(this))
                prod.Save(Get_TrxName());

            return success;
        }
    }
}
