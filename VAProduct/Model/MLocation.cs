using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAModelAD.Model
{

    public class MLocation  //Comparator<PO>
    {
        

        public MLocation(Context context, int c_Location_ID, Trx trx)
        {
            
        }




        /// <summary>
        /// Get Location from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Location_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>MLocation</returns>
        public static dynamic Get(Ctx ctx, int C_Location_ID, Trx trxName)
        {
            return null;
        }

        internal static dynamic GetBPLocation(Context context, int c_BPartner_Location_ID, object p)
        {
            throw new NotImplementedException();
        }
    }
}
