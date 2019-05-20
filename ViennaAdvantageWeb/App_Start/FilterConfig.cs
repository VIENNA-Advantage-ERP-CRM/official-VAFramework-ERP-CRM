using System.Web;
using System.Web.Mvc;

namespace ViennaAdvantageWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
           ViennaBase.FilterConfig.RegisterGlobalFilters(filters);
        }
    }
}