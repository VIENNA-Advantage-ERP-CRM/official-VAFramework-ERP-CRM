using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class ModulePrefixModel
    {
        /// <summary>
        /// Get Module Existance
        /// </summary>        
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String,Boolean> GetModulePrefix(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            Dictionary<String, Boolean> _ModulePrifix = new Dictionary<String, Boolean>();
            for (int i = 0; i < paramValue.Length; i++)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='" + paramValue[i] + "'  AND IsActive = 'Y'", null, null)) > 0)
                {
                    _ModulePrifix[paramValue[i]] = true;
                }
                else
                {
                    _ModulePrifix[paramValue[i]] = false;
                }
            }
            return _ModulePrifix;
        }
    }
}