using System.Web.Mvc;

namespace VAS.Areas.VAS
{
    public class VASAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VAS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VAS_default",
                "VAS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}