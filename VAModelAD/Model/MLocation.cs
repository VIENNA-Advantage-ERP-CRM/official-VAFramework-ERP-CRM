using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        static MethodInfo _Get = null;
        static MethodInfo _GetBPLocation = null;
        private Context context;
        private int c_Location_ID;
        private object p;

        public static void LoadLocationType()
        {
            Assembly asm = Assembly.Load("VAModel");
            Type _type = Type.GetType("VAdvantage.Model.MLocation");
            if (_type != null)
            {
                _Get = _type.GetMethod("Get", BindingFlags.Static);
                _GetBPLocation = _type.GetMethod("GetBPLocation", BindingFlags.Static);
            }
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
            if (_Get == null)
                LoadLocationType();
            if (_Get != null)
            { 
               dynamic obj = _Get.Invoke(null, new Object[] { ctx, C_Location_ID, trxName });
                return obj;
            }
            return null;
        }

        internal static dynamic GetBPLocation(Context context, int c_BPartner_Location_ID, Trx trx)
        {
            if (_GetBPLocation == null)
                LoadLocationType();
            if (_GetBPLocation != null)
            {
                dynamic obj = _GetBPLocation.Invoke(null, new Object[] { context, c_BPartner_Location_ID, trx });
                return obj;
            }
            return null;
        }
    }
}
