/*****************************************************
 * Module Name   : Workflow
 * Purpose       : 
 * Class Used    : X_VAF_WFlow_Block
 * Chronological    Development
 * Veena Pandey      01-May-2009
 ******************************************************/

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;
namespace VAdvantage.WF
{
    class MVAFWFlowBlock : Model.X_VAF_WFlow_Block
    {
        //	Cache
        private static CCache<int, MVAFWFlowBlock> _cache = new CCache<int, MVAFWFlowBlock>("VAF_WFlow_Block", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Block_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowBlock(Ctx ctx, int VAF_WFlow_Block_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Block_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowBlock(Ctx ctx, System.Data.DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get MWFBlock from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Block_ID">id</param>
        /// <returns>MWFBlock</returns>
        public static MVAFWFlowBlock Get(Ctx ctx, int VAF_WFlow_Block_ID)
        {
            int key = VAF_WFlow_Block_ID;
            MVAFWFlowBlock retValue = (MVAFWFlowBlock)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFWFlowBlock(ctx, VAF_WFlow_Block_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }
    }
}
