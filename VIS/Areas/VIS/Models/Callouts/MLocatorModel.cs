using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MLocatorModel
    {
        /// <summary>
        /// GetLocator
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
          public Dictionary<string,string> GetLocator(Ctx ctx,string fields)
          {
            MLocator defaultLocator = MLocator.GetDefaultLocatorOfOrg(ctx, ctx.GetAD_Org_ID());
            int Default_Locator_ID = 0;
            if (defaultLocator != null)
            {
                Default_Locator_ID = defaultLocator.Get_ID();
            }

            Dictionary<string, string> retlst = new Dictionary<string, string>();
            retlst["Default_Locator_ID"] = Default_Locator_ID.ToString();
            return retlst;

        }
    }
}