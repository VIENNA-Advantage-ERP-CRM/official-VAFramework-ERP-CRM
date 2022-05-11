using System.Web.Mvc;
using System.Web.Optimization;

namespace VAS
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

            StyleBundle style = new StyleBundle("~/Areas/VAS/Content/VASstyle");

            ScriptBundle modScript = new ScriptBundle("~/Areas/VAS/Scripts/VASjs");

            modScript.Include(
                "~/Areas/VAS/Scripts/app/forms/vcreatefrom.js",
                "~/Areas/VAS/Scripts/app/forms/vcreatefrominvoice.js",
                "~/Areas/VAS/Scripts/app/forms/vcreatefromshipment.js",
                "~/Areas/VAS/Scripts/app/forms/vcreatefromstatement.js",
                "~/Areas/VAS/Scripts/app/forms/acctviewer.js",
                "~/Areas/VAS/Scripts/app/forms/acctviewermenu.js",
                "~/Areas/VAS/Scripts/app/forms/vinoutgen.js",
                "~/Areas/VAS/Scripts/app/forms/vinvoicegen.js",
                "~/Areas/VAS/Scripts/app/forms/vmatch.js",
                "~/Areas/VAS/Scripts/app/forms/vcharge.js",
                 "~/Areas/VAS/Scripts/app/forms/vattributegrid.js",
                 "~/Areas/VAS/Scripts/app/forms/vpayselect.js",
                  "~/Areas/VAS/Scripts/app/forms/vpayprint.js",
                  "~/Areas/VAS/Scripts/app/forms/vBOMdrop.js",
                  "~/Areas/VAS/Scripts/app/forms/vtrxmaterial.js",
                   "~/Areas/VAS/Scripts/model/Callouts.js");


            style.Include("~/Areas/VAS/Content/PaymentRule.css",
                "~/Areas/VAS/Content/VPaySelect.css");

            style.Include("~/Areas/VAS/Content/VIS.rtl.css");

            //style.Include("~/Areas/VAS/Content/VAS.all.min.css");


            //modScript.Include("~/Areas/VAS/Scripts/VAS.all.min.js");



            VAdvantage.ModuleBundles.RegisterScriptBundle(modScript, "VAS", -9);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VAS", -9);

        }
    }
}