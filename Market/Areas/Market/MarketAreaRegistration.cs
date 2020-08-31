using System.Web.Mvc;
using System.Web.Optimization;

namespace Market
{
    public class MarketAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Market";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Market_default",
                "Market/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            StyleBundle style = new StyleBundle("~/Areas/Market/Content/Marketstyle");


            //style.Include("~/Areas/Market/Content/Market.css");

            ScriptBundle modScript = new ScriptBundle("~/Areas/Market/Script/Marketjs");
            //modScript.Include(
            //      "~/Areas/Market/Scripts/edialog.js",
            //     "~/Areas/Market/Scripts/market.js",
            //    "~/Areas/Market/Scripts/moduledependency.js",
            //    "~/Areas/Market/Scripts/moduledialog.js",
            //      "~/Areas/Market/Scripts/historydialog.js"
            //    );


            style.Include("~/Areas/Market/Content/Market.all.min.css");
            modScript.Include("~/Areas/Market/Scripts/market.all.min.js");

           VAdvantage.ModuleBundles.RegisterScriptBundle(modScript, "Market", 10);
           VAdvantage.ModuleBundles.RegisterStyleBundle(style, "Market", 10);
        }
    }
}