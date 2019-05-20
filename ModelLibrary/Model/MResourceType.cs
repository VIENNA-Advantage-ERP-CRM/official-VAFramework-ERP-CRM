/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MResourceType
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
    public class MResourceType : X_S_ResourceType
    {
        /* 	Standard Constructor
         *	@param ctx context
         *	@param S_ResourceType_ID id
         */
        public MResourceType(Ctx ctx, int S_ResourceType_ID, Trx trxName)
            : base(ctx, S_ResourceType_ID, trxName)
        {
            
        }	

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         */
        public MResourceType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            //	Update Products
            if (!newRecord)
            {
                MProduct[] products = MProduct.Get(GetCtx(), "S_Resource_ID IN "
                    + "(SELECT S_Resource_ID FROM S_Resource WHERE S_ResourceType_ID="
                    + GetS_ResourceType_ID() + ")", Get_TrxName());
                for (int i = 0; i < products.Length; i++)
                {
                    MProduct product = products[i];
                    if (product.SetResource(this))
                        product.Save(Get_TrxName());
                }
            }

            return success;
        }

    }
}
