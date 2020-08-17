using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAModelAD.Model
{
    public class MClient: VAdvantage.Model.X_AD_Client
    {
        private static CCache<int, MClient> s_cache = new CCache<int, MClient>("AD_Client_AD", 3);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MClient).FullName);
        private VAdvantage.Login.Language _language = null;
        public MClient(Ctx ctx, int AD_Client_ID, Trx trxName)
            : base(ctx, AD_Client_ID, trxName)
        {

        }



        public static MClient Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetAD_Client_ID());
        }
        public static MClient Get(Ctx ctx, int AD_Client_ID)
        {
            int key = AD_Client_ID;
            MClient client = (MClient)s_cache[key];
            if (client != null)
                return client;
            client = new MClient(ctx, AD_Client_ID, null);
            if (AD_Client_ID == 0)
                client.Load((Trx)null);
            s_cache.Add(key, client);
            return client;
        }

        public VAdvantage.Login.Language GetLanguage()
        {
            if (_language == null)
            {
                _language = VAdvantage.Login.Language.GetLanguage(GetAD_Language());
                _language = Env.VerifyLanguage(GetCtx(), _language);
            }
            return _language;
        }   //	getLanguage

        public bool IsAutoUpdateTrl(String strTableName)
        {
            if (base.IsMultiLingualDocument())
                return false;
            if (strTableName == null)
                return false;
            //	Not Multi-Lingual Documents - only Doc Related
            if (strTableName.StartsWith("AD"))
                return false;
            return true;
        }

        /**
	 * 	Is Auto Archive on
	 *	@return true if auto archive
	 */
        public bool IsAutoArchive()
        {
            String aa = GetAutoArchive();
            return aa != null && !aa.Equals(AUTOARCHIVE_None);
        }	//	

    }
}
