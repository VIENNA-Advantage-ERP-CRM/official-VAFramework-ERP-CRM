using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
   public class MSysConfig:X_AD_SysConfig
    {
		private static VLogger _log = VLogger.GetVLogger(typeof(MSysConfig).FullName);

		/** Cache			*/
		private static CCache<String, String> cache = new CCache<String, String>("MSysConfig", 3);

		public MSysConfig(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor */


		public MSysConfig(Ctx ctx, int AD_SysConfig_ID, Trx trxName) : base(ctx, AD_SysConfig_ID, trxName) { }/** if (AD_SysConfig_ID == 0){SetAD_SysConfig_ID (0);} */


		public static String GetValue(String Name)
			{
				String str = Util.GetValueOfString(cache.Get(Name));
				if (str != null)
					return str;
				

				//
				String sql = "SELECT Value FROM AD_SysConfig"
								+ " WHERE IsActive='Y' AND Name='"+ Name + "'"
								+ " ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";
				IDataReader dr = null;
				try
				{
					dr = DB.ExecuteReader(sql, null);
					while (dr.Read())
						str = dr.GetString(1);
				}
				catch (Exception e)
				{
				_log.Log(Level.SEVERE, "getValue", e);
				}
				finally
				{
					if (dr != null)
						dr.Close();
					dr = null;
				}
				//
				if (str != null)
				{
					cache[Name] =str;
					return str;
				}
				else
				{
					// anyways, put the not found key as null
					cache[Name] =null;
					return null;
				}
			
		}
}
}
