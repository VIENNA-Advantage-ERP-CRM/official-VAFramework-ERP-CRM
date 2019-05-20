using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
  public  class MProductAttributeImage : X_M_ProductAttributeImage
    {/// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VA022_BPartnerLocaton_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributeImage(Ctx ctx, int M_ProductAttributeImage_ID, Trx trxName)
            : base(ctx, M_ProductAttributeImage_ID, trxName)
        {
            
        }	

        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributeImage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }
    }
}
