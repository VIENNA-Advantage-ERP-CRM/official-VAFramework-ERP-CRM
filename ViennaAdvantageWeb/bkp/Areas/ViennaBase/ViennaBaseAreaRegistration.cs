using System.Web.Mvc;

namespace ViennaBase.Areas.ViennaBase
{
    public class ViennaBaseAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ViennaBase";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ViennaBase_default",
                "ViennaBase/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}