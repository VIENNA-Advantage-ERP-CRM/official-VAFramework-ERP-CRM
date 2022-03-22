
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAModelAD.Model
{
    public class MLocation
    {
        /* Get Method Type */
        static MethodInfo _Get = null;
        /* Get BPLocation */
        static MethodInfo _GetBPLocation = null;
        /* Class String */
        private const string LOC_CLASS = "ModelLibrary";

        /// <summary>
        /// Load Location Type
        /// </summary>
        public static void LoadLocationType()
        {
            Assembly asm = Assembly.Load(LOC_CLASS);
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

        /// <summary>
        /// Get Business Partner Location
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="c_BPartner_Location_ID"> bp location id </param>
        /// <param name="trx">transaction object</param>
        /// <returns>BP Location</returns>
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